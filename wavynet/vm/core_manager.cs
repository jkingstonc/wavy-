/*
 * James Clarke
 * 27/08/19
 */

using System.Collections.Generic;
using System.Threading;

namespace wavynet.vm
{
    /* 
     * Handles threading of cores
    */
    public class CoreManager
    {
        private VM vm;
        private Dictionary<int, Core> core_pool;
        private int next_id = 0;

        public CoreManager(VM vm)
        {
            this.vm = vm;
            this.core_pool = new Dictionary<int, Core>();
        }

        // Abort all active cores if we encounter a fatal error
        public void abort_all(int id = -1)
        {
            // If the id doesn't match, stop the core by flagging it's multi core state to ABORTED
            foreach(KeyValuePair<int, Core> pair in this.core_pool)
            {
                if (id != pair.Key)
                {
                    pair.Value.state.multi_core_state = MultiCoreState.ABORTED;
                }
            }
        }

        // Called by an executing core to stop all other cores
        public void isolate(int id)
        {
            abort_all(id);
        }

        // Create and run a new core instance
        public void create_and_run(BytecodeInstance[] sequence)
        {
            int id = add_core();
            setup_core(id, sequence);
            start_core(id);
            WaitHandle.WaitAll(new WaitHandle[] { get_core(id).handle });
        }

        // Add a new core to the pool
        public int add_core()
        {
            vm.ASSERT_ERR(!VM.MULTI_CORE && this.next_id > 0, VMErrorType.INVALID_CORE_COUNT, "Cannot have multiple cores in single core mode!");
            int id = gen_id();
            this.core_pool.Add(id, new Core(this.vm, id));
            this.next_id++;
            return id;
        }

        // Setup the core, after it has been registered to the pool
        public int setup_core(int id, BytecodeInstance[] sequence)
        {
            this.core_pool[id].setup(sequence);
            return id;
        }

        // Start the core running
        public int start_core(int id)
        {
            this.core_pool[id].run();
            return id;
        }

        // Close the core running
        public void close_core(int id)
        {
            this.core_pool[id].close();
        }

        // This is used when the vm initially wants to spawn multiple cores
        // This should not be used from within a core
        public void join_all_cores()
        {

            WaitHandle[] handles = new WaitHandle[this.core_pool.Count];
            int counter = 0;
            foreach (KeyValuePair<int, Core> pair in this.core_pool)
            {
                handles[counter] = pair.Value.handle;
                counter++;
            }
            WaitHandle.WaitAll(handles);
        }

        // Generate a new id
        private int gen_id()
        {
            return next_id;
        }

        private Core get_core(int id)
        {
            return this.core_pool[id];
        }
    }
}