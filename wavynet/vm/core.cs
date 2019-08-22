using System;

namespace wavynet.vm
{
    /* 
     * Represents a vm core
     * Performs fde cycle
    */
    public class Core
    {
        private VM vm;
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
        // The execution stack that this core is using
        public ExecStack exec_stack;

        // The max Program Counter, this determines program length
        public const int MAX_PC = 2048;

        public Core(VM vm)
        {
            this.vm = vm;
            this.state = CoreState.setup();
            this.pc = 0;
            this.traceback = new TraceBack();
            this.func_stack = new FuncStack(this);
            this.exec_stack = new ExecStack(this);

            setup_func_stack();
        }

        // Setup the core's function stack
        private void setup_func_stack()
        {
            // We first enter the main func state
            // Do we want this? Should be use a global exec stack for global state?
            //if(TRACE_DEBUG)
            //  this.func_call_trace("main");
            //else
            //  this.func_call("main")
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
                BytecodeInstance bytecode = get_next();

                int op = get_op(bytecode);
                int arg=-1;

                if(has_arg(bytecode))
                    arg = get_arg(bytecode);

                if (VM.INSTR_DEBUG)
                {
                    if(has_arg(bytecode))
                        Console.WriteLine("op: " + (Bytecode.Opcode)op + "arg: " + arg);
                    else
                        Console.WriteLine("op: " + op);
                }

                // Surround the execute phase in a try catch, this is so the vm can return safely to the error handling
                // without breaking other vm components
                try
                {
                    switch (op)
                    {
                        case (int)Bytecode.Opcode.END:
                            {
                                // Return an END state
                                this.state.currently_interpreting = false;
                                return this.state;
                            }
                        case (int)Bytecode.Opcode.NOP:
                            {
                                goto_next();
                                break;
                            }
                        case (int)Bytecode.Opcode.POP_EXEC:
                            {
                                pop_exec();
                                goto_next();
                                break;
                            }
                        case (int)Bytecode.Opcode.BIN_ADD:
                            {
                                WavyItem left = pop_exec();
                                WavyItem right = pop_exec();
                                goto_next();
                                break;
                            }
                        default:
                            {
                                // We have an invalid opcode
                                push_err(ErrorType.INVALID_OP, "Invalid opcode: " + op);
                                break;
                            }
                    }

                    // Program counter is out of range
                    if (pc < 0 || pc > MAX_PC)
                    {
                        push_err(ErrorType.INVALID_PC_RANGE, "Program Counter is out of range!");
                    }
                }
                // Currently, we have no use of the VMErrException
                catch(CoreErrException err_exception)
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
        public void push_err(ErrorType type, string msg)
        {
            // Register the error with the handler
            this.state.err_handler.register_err(this.state, this.traceback, type, msg);
            this.state.had_err = true;
            throw new CoreErrException();
        }

        // Push an error to the cores' error handler
        public void push_err(ErrorType type)
        {
            // Register the error with the handler
            this.state.err_handler.register_err(this.state, this.traceback, type);
            this.state.had_err = true;
            throw new CoreErrException();
        }

        // Perform a function call with a trace
        // WARNING: This is a dev version, it should not take a string, it should take a ref to a WavyFunction
        private Trace func_call_trace(string name)
        {
            // Then create a new Trace instance referencing that frame
            Trace trace = new Trace(func_call(name));
            // Push the trace to the traceback
            this.traceback.push_call_trace(trace);
            return trace;
        }

        // Perform a function call
        // WARNING: This is a dev version, it should not take a string, it should take a ref to a WavyFunction
        private FuncFrame func_call(string name)
        {
            // First create a new FuncFrame to push to the function stack
            FuncFrame frame = new FuncFrame(this, name, ExecStack.deep_copy(this, this.exec_stack));
            // Then push the frame to the FuncStack
            this.func_stack.push(frame);
            return frame;
        }

        // Check if we have reached the end
        public bool end()
        {
            return (pc >= state.opcode_count) || (bytecode[pc].op == (int)Bytecode.Opcode.END);
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
        public int get_op(BytecodeInstance bytecode)
        {
            return bytecode.op;
        }

        // Get the next argument
        public int get_arg(BytecodeInstance bytecode)
        {
            return bytecode.arg;
        }

        public bool has_arg(BytecodeInstance bytecode)
        {
            return bytecode.has_arg;
        }

        // Pop from the current execution stack in use
        public WavyItem pop_exec()
        {
            // We then get the stack and pop the top value
            WavyItem item = exec_stack.pop();
            if (item != null)
                return item;
            return null;
        }

        // Pop from the current execution stack in use
        public WavyItem peek_exec()
        {
            return exec_stack.peek();
        }

        // Pop from the current execution stack in use
        public void push_exec(WavyItem item)
        {
            exec_stack.push(item);
        }

        // Get the top Function Frame 
        public FuncFrame top_f_frame()
        {
            return this.func_stack.peek();
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