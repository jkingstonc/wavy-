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
        public void abort_all()
        {
            foreach(KeyValuePair<int, Core> pair in this.core_pool)
            {
                pair.Value.thread.Abort();
            }
        }

        // Add a new core to the pool
        public int add_core()
        {
            vm.ASSERT_ERR(VM.MULTI_CORE && this.next_id > 0, VMErrorType.INVALID_CORE_COUNT, "Cannot have multiple cores in single core mode!");
            int id = gen_id();
            this.core_pool.Add(id, new Core(this.vm, this, id));
            this.next_id++;
            return id;
        }

        public int setup_core(int id, BytecodeInstance[] sequence)
        {
            this.core_pool[id].setup(sequence);
            return id;
        }

        // Start the core running
        public int start_core(int id)
        {
            this.core_pool[id].thread.Start();
            return id;
        }

        public void close_core(int id)
        {
            this.core_pool[id].close();
        }

        // Generate a new id
        private int gen_id()
        {
            return next_id;
        }
    }
}