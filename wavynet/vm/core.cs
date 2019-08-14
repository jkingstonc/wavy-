using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wavynet.vm
{
    /* 
     * Represents a vm core
     * Performs fde cycle
    */
    class Core
    {
        // Program counter
        public int pc;
        // Shared error handler
        public ErrorHandler err_handler;
        // Store the operations the core has carried out
        public TraceBack traceback_state;

        public OpcodeInstance[] bytecode;

        public Core(ErrorHandler err_handler)
        {
            this.err_handler = err_handler;
        }

        public void register_sequence(OpcodeInstance[] sequence)
        {
            this.bytecode = sequence;
        }

        public CoreState evaluate_sequence()
        {
            while(!end())
            {
                OpcodeInstance opcode = get_next();
                int op = get_op(opcode);
                int arg = get_arg(opcode);

                switch(op)
                {
                    case (int)Bytecode.Types.END:
                        {
                            // Return an END state
                            return make_corestate(CoreState.StateFlag.END);
                        }
                    default:
                        {
                            // We have an invalid opcode
                            err_handler.register_err(make_corestate(CoreState.StateFlag.ERR), ErrorType.INVALID_OP, "Invalid opcode: "+op);
                            err_handler.say_latest();
                            break;
                        }
                }
            }
            // Return an END state
            return make_corestate(CoreState.StateFlag.END);
        }

        // Generate a corestate
        private CoreState make_corestate(CoreState.StateFlag flag)
        {
            return new CoreState(flag, this.traceback_state);
        }

        // Check if we have reached the end
        public bool end()
        {
            return (pc >= bytecode.Length) || (bytecode[pc].op == (int)Bytecode.Types.END);
        }

        // Returns the next bytecode instance
        public OpcodeInstance get_next()
        {
            return bytecode[pc++];
        }

        // Get the next opcode
        public int get_op(OpcodeInstance opcode)
        {
            return opcode.op;
        }

        // Get the next argument
        public int get_arg(OpcodeInstance opcode)
        {
            return opcode.arg;
        }
    }

    // Represents a state of the core at a particular time
    public class CoreState
    {
        // Indicates the state when the CoreState was generated
        public enum StateFlag
        {
            END = 0,
            ERR = 1,
        }

        public TraceBack traceback;
        public StateFlag state_flag;

        public CoreState(StateFlag state_flag, TraceBack traceback)
        {
            this.state_flag = state_flag;
            this.traceback = traceback;
        }
    }
}
