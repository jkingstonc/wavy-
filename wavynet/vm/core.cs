using System;

namespace wavynet.vm
{
    /* 
     * Represents a vm core
     * Performs fde cycle
    */
    public class Core
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

        public const int MAX_PC = 2048;

        // FOR DEBUGGING
        const bool INSTR_DEBUG = true;

        public Core()
        {
            this.state = CoreState.setup();
            this.pc = 0;
            this.traceback = new TraceBack();
            this.func_stack = new FuncStack(this);

            setup_func_stack();
        }

        // Setup the core's function stack
        private void setup_func_stack()
        {
            // We first enter the main func state
            // Do we want this? Should be use a global exec stack for global state?
            this.call_trace("main");
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

                if (INSTR_DEBUG)
                {
                    if(has_arg(bytecode))
                        Console.WriteLine("op: " + (Bytecode.Opcode)op + "arg: " + arg);
                    else
                        Console.WriteLine("op: " + op);
                }

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
                            Console.WriteLine("pop exec!");
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
        public void push_err(ErrorType type, string msg)
        {
            // Register the error with the handler
            this.state.err_handler.register_err(this.state, this.traceback, type, msg);
            this.state.had_err = true;
        }

        // Push an error to the cores' error handler
        public void push_err(ErrorType type)
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
            FuncFrame frame = new FuncFrame(this, name);
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
            // We peek the top of the func_stack [the currently executing function]
            ExecStack exec_stack = top_f_frame().get_stack();
            // We then get the stack and pop the top value
            WavyItem item = exec_stack.pop();
            if (item != null)
                return item;
            return null;
        }

        // Pop from the current execution stack in use
        public WavyItem peek_exec()
        {
            // We peek the top of the func_stack [the currently executing function]
            // We then get the stack and peek the top value
            return top_f_frame().get_stack().peek();
        }

        // Pop from the current execution stack in use
        public void push_exec(WavyItem item)
        {
            // We peek the top of the func_stack [the currently executing function]
            // We then get the stack and pop the top value
            top_f_frame().get_stack().push(item);
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