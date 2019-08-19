using System;

namespace wavynet.vm
{
    /* 
     * Represents a vm core
     * Performs fde cycle
    */
    class Core
    {
        // Holds information about the current state of this core
        private CoreState state;
        // Program counter
        public int pc;
        // Store the operations the core has carried out
        public TraceBack traceback;
        // The bytecode that the core is executing
        public BytecodeInstance[] bytecode;
        // The function stack that this core is using
        public FuncStack func_stack;

        public Core()
        {
            this.state = CoreState.setup();
            this.pc = 0;
            this.traceback = new TraceBack();
            this.func_stack = new FuncStack();

            setup_func_stack();
        }

        // Setup the core's function stack
        private void setup_func_stack()
        {
            // We first enter the main func state
            // Do we want this? Should be use a global exec stack for global state?
            // this.call_trace("main");
        }

        // Register a bytecode sequence to the core
        public void register_bytecode_seq(BytecodeInstance[] sequence)
        {
            this.bytecode = sequence;
            this.state.opcode_count = sequence.Length;
        }

        // Main fde cycle
        public CoreState evaluate_sequence()
        {
            // Check we have not reached the end
            while (!end())
            {
                BytecodeInstance opcode = get_next();

                int op = get_op(opcode);
                int arg = get_arg(opcode);

                Console.WriteLine(op.ToString());
                switch (op)
                {
                    case (int)Bytecode.Types.END:
                        {
                            // Return an END state
                            this.state.currently_interpreting = false;
                            return this.state;
                        }
                    case (int)Bytecode.Types.NOP:
                        {
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
                // Check if we have had an error
                if (this.state.had_err)
                { 
                    this.state.err_handler.say_latest();
                    break;
                }
            }
            return close_core();
        }

        // Called when we are done with this core
        private CoreState close_core()
        {
            // Return an END state
            this.state.currently_interpreting = false;
            return this.state;
        }

        // Push an error to the cores' error handler
        private void push_err(ErrorType type, string msg)
        {
            // Register the error with the handler
            this.state.err_handler.register_err(this.state, this.traceback, type, msg);
            this.state.had_err = true;
        }

        // Push an error to the cores' error handler
        private void push_err(ErrorType type)
        {
            // Register the error with the handler
            this.state.err_handler.register_err(this.state, this.traceback, type);
            this.state.had_err = true;
        }

        // Perform a function call with a trace
        // WARNING: This is a dev version, it should not take a string, it should take a ref to a WavyFunction
        private void call_trace(string name)
        {
            // First create a new FuncFrame to push to the function stack
            FuncFrame frame = new FuncFrame(name);
            // Then push the frame to the FuncStack
            this.func_stack.push(frame);
            // Then create a new Trace instance referencing that frame
            Trace trace = new Trace(frame);
            // Push the trace to the traceback
            this.traceback.push_call_trace(trace);
        }

        // Check if we have reached the end
        public bool end()
        {
            return (pc >= state.opcode_count) || (bytecode[pc].op == (int)Bytecode.Types.END);
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
        public ErrorHandler err_handler;
        public int opcode_count;
        public bool currently_interpreting;
        public int func_depth;
        public bool had_err;
        
        public CoreState(ErrorHandler err_handler, int opcode_count, bool currently_interpreting, int func_depth)
        {
            this.err_handler = err_handler;
            this.opcode_count = opcode_count;
            this.currently_interpreting = currently_interpreting;
            this.func_depth = func_depth;
        }

        // Create a fresh CoreState instance
        public static CoreState setup()
        {
            return new CoreState(new ErrorHandler(), 0, false, 0);
        }
    }
}