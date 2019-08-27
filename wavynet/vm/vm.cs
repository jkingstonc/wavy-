/*
 * James Clarke
 * 22/08/19
 */

using wavynet.vm.data;

namespace wavynet.vm
{
    public class VM
    {
        public VMState state;
        public CoreManager core_manager;
        public BankManager bank_manager;

        // Should the vm emulate multi threading using multiple cores
        public static bool MULTI_CORE = true;
        // Should we cache the value retrieved from the bank for use in multiple cores
        public static bool MULTI_CORE_BANK_CACHING = true;

        // FOR DEBUGGING
        public static bool INSTR_DEBUG = false;
        public static bool TRACE_DEBUG = false;
        

        public VM()
        {
            try
            {
                this.state = VMState.setup();
                this.bank_manager = new BankManager();
                this.bank_manager.setup_l_test();

                BytecodeInstance[] sequence = new BytecodeInstance[]
                {
                new BytecodeInstance(-1),
                new BytecodeInstance(-1),
                new BytecodeInstance(-1),
                new BytecodeInstance(-1),
                new BytecodeInstance(-1),
                new BytecodeInstance(-1),
                new BytecodeInstance(-1),
                new BytecodeInstance(-1),
                new BytecodeInstance(-1),
                };


                this.core_manager = new CoreManager(this);
                this.core_manager.start_core(core_manager.setup_core(core_manager.add_core(), sequence));
                this.core_manager.start_core(core_manager.setup_core(core_manager.add_core(), sequence));
            }                
            catch (CoreErrException)
            {
                this.core_manager.abort_all();
                this.state.err_handler.say_latest();
            }
        }

        // Used when we may need to register an error (for convenience like a macro)
        public void ASSERT_ERR(bool condition, VMErrorType type, string msg = null)
        {
            if (condition)
                push_err(type, msg);
        }

        // Push an error to the cores' error handler
        public void push_err(VMErrorType type, string msg = null)
        {
            // Register the error with the handler
            this.state.err_handler.register_err(new VMError(this.state, type, msg));
            this.state.had_err = true;
            throw new CoreErrException();
        }
    }

    // Represents a state of the vm at a particular time
    public class VMState
    {
        public ErrorHandler err_handler;
        public bool had_err;

        public VMState(ErrorHandler err_handler)
        {
            this.err_handler = err_handler;
            this.had_err = false;
        }

        // Create a fresh VMState instance
        public static VMState setup()
        {
            return new VMState(new VMErrorHandler());
        }
    }
}