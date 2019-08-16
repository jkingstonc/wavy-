using System;

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
        public TraceBack traceback;

        public BytecodeInstance[] bytecode;

        public FuncStack func_stack;

        public Core(ErrorHandler err_handler)
        {
            this.pc = 0;
            this.err_handler = err_handler;
            this.traceback = new TraceBack();
            this.func_stack = new FuncStack();

            setup_func_stack();
        }

        private void setup_func_stack()
        {
            this.func_stack.push_new_frame();
        }

        public void register_bytecode_seq(BytecodeInstance[] sequence)
        {
            this.bytecode = sequence;
        }

        public CoreState evaluate_sequence()
        {
            while(!end())
            {
                BytecodeInstance opcode = get_next();
                int op = get_op(opcode);
                int arg = get_arg(opcode);

                switch(op)
                {
                    case (int)Bytecode.Types.END:
                        {
                            // Return an END state
                            return make_corestate(CoreState.StateFlag.END);
                        }
                    case (int)Bytecode.Types.NOP:
                        {
                            Console.WriteLine("NOP");
                            goto_next();
                            break;
                        }
                    case (int)Bytecode.Types.BIN_ADD:
                        {
                            WavyItem left = pop_execstack();
                            WavyItem right = pop_execstack();
                            goto_next();
                            break;
                        }
                    default:
                        {
                            // We have an invalid opcode
                            push_err(ErrorType.INVALID_OP, "Invalid opcode: " + op);
                            goto_next();
                            break;
                        }
                }
            }
            // Return an END state
            return make_corestate(CoreState.StateFlag.END);
        }

        private void push_err(ErrorType type, string msg)
        {
            // Register the error with the handler
            err_handler.register_err(make_corestate(CoreState.StateFlag.ERR), this.traceback, type, msg);
            // Say the latest error
            err_handler.say_latest();
        }

        private void push_err(ErrorType type)
        {
            // Register the error with the handler
            err_handler.register_err(make_corestate(CoreState.StateFlag.ERR), this.traceback, type);
            // Say the latest error
            err_handler.say_latest();
        }

        // Generate a corestate
        private CoreState make_corestate(CoreState.StateFlag flag)
        {
            return new CoreState(flag);
        }

        // Perform a function call with a trace
        private void call_trace()
        {
            Trace trace = new Trace(this.func_stack.peek());
            this.traceback.push_call_trace(trace);
        }

        // Check if we have reached the end
        public bool end()
        {
            return (pc >= bytecode.Length) || (bytecode[pc].op == (int)Bytecode.Types.END);
        }

        // Called once the current bytecode execution has been completed
        public void goto_next()
        {
            pc++;
        }

        // Returns the next bytecode instance
        public BytecodeInstance get_next()
        {
            return bytecode[pc];
        }

        // Get the next opcode
        public int get_op(BytecodeInstance opcode)
        {
            return opcode.op;
        }

        // Get the next argument
        public int get_arg(BytecodeInstance opcode)
        {
            return opcode.arg;
        }

        // Pop from the current execution stack in use
        public WavyItem pop_execstack()
        {
            // We peek the top of the func_stack [the currently executing function]
            // We then get the stack and pop the top value
            return this.func_stack.peek().get_stack().pop();
        }

        // Pop from the current execution stack in use
        public WavyItem peek_execstack()
        {
            // We peek the top of the func_stack [the currently executing function]
            // We then get the stack and peek the top value
            return this.func_stack.peek().get_stack().peek();
        }

        // Pop from the current execution stack in use
        public void push_execstack(WavyItem item)
        {
            // We peek the top of the func_stack [the currently executing function]
            // We then get the stack and pop the top value
            this.func_stack.peek().get_stack().push(item);
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

        public StateFlag state_flag;

        public CoreState(StateFlag state_flag)
        {
            this.state_flag = state_flag;
        }
    }
}
