/*
 * James Clarke
 * 27/08/19
 */

#define VM_MULTI_CORE

using System;
using System.Collections.Generic;
using System.Threading;

namespace wavynet.vm.core
{
    /* 
     * Handles threading of cores
    */
    public class CoreManager : VMComponent
    {
        // This is the event which handles adding a new core to the core_pool
        public EventHandler new_core_event;

        // The event which is triggered when the vm should end
        ManualResetEvent end_vm_event = new ManualResetEvent(false);

        private VM vm;
        private Dictionary<int, Core> core_pool;
        private int next_id = 0;

        public CoreManager(VM vm) : base("CoreManager")
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
        public void create_and_run(object sender, EventArgs args)
        {
            int id = add_core();
            LOG("creating & starting new core '" + id + "'");
            setup_core(id, ((CoreCreateEventArgs)args).bytecode);
            start_core(id);
        }

        // Add a new core to the pool
        public int add_core()
        {
#if VM_MULTI_CORE
#else
            ASSERT_ERR(this.next_id > 0, VMErrorType.INVALID_CORE_COUNT, "Cannot have multiple cores in single core mode!");
#endif
            int id = gen_id();
            this.core_pool.Add(id, new Core(this.vm, id));
            this.next_id++;
            return id;
        }

        // Setup the core, after it has been registered to the pool
        public int setup_core(int id, Int32[] sequence)
        {
            ASSERT_ERR(!(this.core_pool.ContainsKey(id)), VMErrorType.INVALID_CORE_ID, "id: "+id);
            this.core_pool[id].setup(sequence);
            return id;
        }

        // Start the core running
        public int start_core(int id)
        {
            ASSERT_ERR(!(this.core_pool.ContainsKey(id)), VMErrorType.INVALID_CORE_ID, "id: " + id);
            this.core_pool[id].start();
            return id;
        }

        // Close the core running
        public void close_core(int id)
        {
            LOG("closing core '" + id + "'");
            this.core_pool.Remove(id);
            // If we have done with all cores, trigger the close vm event
            if (this.core_pool.Count == 0)
                this.end_vm_event.Set();
        }

        // This is used to join the vm thread to the core pool
        // We wait until the core_pool is empty and then we can close the vm
        public void join_all_cores()
        {
            LOG("joining thread to all cores");
            // This thread will block here until the close_vm_event is sent.
            end_vm_event.WaitOne();
            end_vm_event.Reset();
        }

        // Generate a new id
        private int gen_id()
        {
            return next_id;
        }
    }

    /* 
     * Contains information about the creation of a new core
    */
    public class CoreCreateEventArgs : EventArgs
    {
        public int creator_id { get; set; }
        public Int32[] bytecode { get; set; }

        public CoreCreateEventArgs(int creator_id, Int32[] bytecode)
        {
            this.creator_id = creator_id;
            this.bytecode = bytecode;
        }
    }
}