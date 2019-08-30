/*
 * James Clarke
 * 22/08/19
 */

using System;
using System.Threading;
using wavynet.vm.data.items;
using wavynet.vm.native;

namespace wavynet.vm
{
    /* 
     * Represents a vm core
     * Performs fde cycle
    */
    public class Core
    {
        private VM vm;
        private NativeInterface native_interface;
        public Thread thread;
        // Holds information about the current state of this core
        public CoreState state;
        // Program counter
        public int pc;
        // Store the operations the core has carried out
        public TraceBack traceback;
        // The bytecode that the core is executing
        public Int32[] bytecode;
        // The function stack that this core is using
        public FuncStack func_stack;
        // The execution stack that this core is using
        public ExecStack exec_stack;

        // The max Program Counter, this determines program length
        public const int MAX_PC = 65536; // 2^16

        private System.Diagnostics.Stopwatch watch;

        // FOR DEBUGGING
        // Do we want to display the currently executing instruction
        public static bool INSTR_DEBUG = false;
        // Do we want function calls to generate a traceback
        public static bool TRACE_DEBUG = true;

        public Core(VM vm, int id)
        {
            this.vm = vm;
            this.native_interface = new NativeInterface(this.vm, this);
            this.state = CoreState.setup(id);
            this.pc = 0;
            this.traceback = new TraceBack();
            this.func_stack = new FuncStack(this);
            this.exec_stack = new ExecStack(this);
        }

        // Setup the core thread
        public void setup(Int32[] sequence)
        {
            this.bytecode = sequence;
            this.state.opcode_count = sequence.Length;
            this.state.multi_core_state = MultiCoreState.READY;
            // Create a new EventWaitHandle to allow us to check when this core is finished
            // Create a new Thread and accociate the handle with it
            this.thread = new Thread(() => {
                this.evaluate_sequence();
                });
        }

        // Run the core & start executing code
        public void run()
        {
            this.thread.Start();
            this.watch = System.Diagnostics.Stopwatch.StartNew();
        }

        // When we want the core to suspend execution
        public void suspend()
        {
            this.ASSERT_ERR(this.state.multi_core_state == MultiCoreState.SUSPENDED, CoreErrorType.INVALID_MULTICORE_STATE, "Cannot suspend core that has already been suspended!");
            this.state.multi_core_state = MultiCoreState.SUSPENDED;

        }

        // When we want to resume the core operation
        public void resume()
        {
            this.ASSERT_ERR(this.state.multi_core_state != MultiCoreState.SUSPENDED, CoreErrorType.INVALID_MULTICORE_STATE, "Cannot resume core that has not been suspended!");
            this.state.multi_core_state = MultiCoreState.RUNNING;
        }

        // Called when we want to close this core thread
        public CoreState close()
        {
            this.watch.Stop();
            // Return an END state
            this.state.currently_interpreting = false;
            // Change the state of the multi_core_state
            this.state.multi_core_state = MultiCoreState.DONE;
            this.vm.core_manager.close_core(this.state.id);
            return this.state;
        }

        // Main fde cycle
        public CoreState evaluate_sequence()
        {
            // Initialise the state of the multi_core_state
            this.state.multi_core_state = MultiCoreState.RUNNING;

            // Check we have not reached the end
            while (!end())
            {
                // Only execute if the core has not been suspended
                if (this.state.multi_core_state != MultiCoreState.SUSPENDED)
                {
                    Int32 op = get_next();

                    if (INSTR_DEBUG)
                        Console.WriteLine("op: " + op);

                    // Surround the execute phase in a try catch, this is so the vm can return safely to the error handling
                    // without breaking other vm components
                    try
                    {
                        switch ((Opcode)op)
                        {
                            case Opcode.END:
                                {
                                    // Return an END state
                                    this.state.currently_interpreting = false;
                                    return this.state;
                                }
                            case Opcode.NOP:
                                {
                                    goto_next();
                                    break;
                                }
                            case Opcode.POP_EXEC:
                                {
                                    pop_exec();
                                    goto_next();
                                    break;
                                }
                            case Opcode.LD_LIT:
                                {
                                    Int32 id = get_arg();
                                    push_exec(request_bank_item(data.Bank.Type.LBank, id));
                                    release_bank_item(data.Bank.Type.LBank, id);
                                    goto_next();
                                    break;
                                }
                            case Opcode.LD_VAR:
                                {
                                    Int32 id = get_arg();
                                    push_exec(request_bank_item(data.Bank.Type.MBank, id));
                                    release_bank_item(data.Bank.Type.MBank, id);
                                    goto_next();
                                    break;
                                }
                            case Opcode.INVOKE_FUNC:
                                {
                                    /* WARNING
                                    * This should not work on multiple threads, becuase we are only releasing the function from the bank
                                    * when we are done with the function call. We should handle bank requests smarter than this!
                                     */
                                    Int32 id = get_arg();
                                    // Currently, this does not work with methods, only functions
                                    WavyFunction func = (WavyFunction)request_bank_item(data.Bank.Type.MBank, id);
                                    // Check for the number of required arguments, and get them from the stack
                                    WavyItem[] args = new WavyItem[func.args()];
                                    for (int i = 0; i < func.args(); i++)
                                        args[i] = pop_exec();
                                    if(TRACE_DEBUG)
                                    {
                                        func_call_trace(func, args);
                                    }
                                    else
                                    {
                                        func_call(func, args);
                                    }
                                    release_bank_item(data.Bank.Type.MBank, id);
                                    goto_next();
                                    break;
                                }
                            case Opcode.GOTO:
                                {
                                    this.pc += get_arg();
                                    break;
                                }
                            case Opcode.IF_ZERO:
                                {
                                    Int32 arg = get_arg();
                                    if (expect_numeric(pop_exec()) == 0)
                                    {
                                        goto_next(); break;
                                    }
                                    this.pc += arg; break;
                                }
                            case Opcode.IF_NZERO:
                                {
                                    Int32 arg = get_arg();
                                    if (expect_numeric(pop_exec()) != 0)
                                    {
                                        goto_next(); break;
                                    }
                                    this.pc += arg; break;
                                }
                            case Opcode.IF_GRT:
                                {
                                    Int32 arg = get_arg();
                                    if (expect_numeric(pop_exec()) > expect_numeric(pop_exec()))
                                    {
                                        goto_next(); break;
                                    }
                                    this.pc += arg; break;
                                }
                            case Opcode.IF_GRTE:
                                {
                                    Int32 arg = get_arg();
                                    if (expect_numeric(pop_exec()) >= expect_numeric(pop_exec()))
                                    {
                                        goto_next(); break;
                                    }
                                    this.pc += arg; break;
                                }
                            case Opcode.IF_LT:
                                {
                                    Int32 arg = get_arg();
                                    if (expect_numeric(pop_exec()) < expect_numeric(pop_exec()))
                                    {
                                        goto_next(); break;
                                    }
                                    this.pc += arg; break;
                                }
                            case Opcode.IF_LTE:
                                {
                                    Int32 arg = get_arg();
                                    if (expect_numeric(pop_exec()) <= expect_numeric(pop_exec()))
                                    {
                                        goto_next(); break;
                                    }
                                    this.pc += arg; break;
                                }
                            default:
                                {
                                    // We have an invalid opcode
                                    push_err(CoreErrorType.INVALID_OP, "op: " + op);
                                    break;
                                }
                        }

                        // Program counter is out of range
                        ASSERT_ERR(pc < 0 || pc > MAX_PC, CoreErrorType.INVALID_PC_RANGE);
                    }
                    catch (CoreErrException e)
                    {
                        if (e.err.is_fatal())
                            this.vm.core_manager.isolate(this.state.id);
                        this.state.err_handler.say_latest();
                        break;
                    }
                }
            }
            return close();
        }

        // We expect a WavyItem to be a numeric
        private dynamic expect_numeric(WavyItem item)
        {
            ASSERT_ERR(item.type != ItemType.INT && item.type != ItemType.DOUBLE, CoreErrorType.UNEXPECTED_TYPE, "Expected numeric, but got: " + item.type);
            return item.value;
        }

        // We expect a WavyItem to be an int
        private dynamic expect_int(WavyItem item)
        {
            ASSERT_ERR(item.type != ItemType.INT, CoreErrorType.UNEXPECTED_TYPE, "Expected int, but got: " + item.type);
            return (int)item.value;
        }

        // We expect a WavyItem to be a WavyFunction
        private dynamic expect_wfunc(WavyItem item)
        {
            ASSERT_ERR(!(item.type != ItemType.FUNC), CoreErrorType.UNEXPECTED_TYPE, "Expected WavyFunction, but got: " + item.type);
            return item;
        }

        // We expect a WavyItem to be a WavyObject
        private dynamic expect_wobject(WavyItem item)
        {
            ASSERT_ERR(!(item.type != ItemType.OBJECT), CoreErrorType.UNEXPECTED_TYPE, "Expected WavyObject, but got: " + item.type);
            return item;
        }

        // Request a WavyItem from the BankManager
        private WavyItem request_bank_item(data.Bank.Type type, int id)
        {
            // First attempt to request the item from the bank manager
            WavyItem item = this.vm.bank_manager.request_item(this, type, id);
            while(this.state.multi_core_state == MultiCoreState.BLOCKED)
            {
                // This is a very bad implementation, but currently, we try again until we are not blocked
                item = this.vm.bank_manager.request_item(this, type, id);
            }
            return item;
        }

        // Release a WavyItem from the BankManager
        private void release_bank_item(data.Bank.Type type, int id)
        {
            this.vm.bank_manager.release_item(this, type, id);
        }

        // Used when we may need to register an error (for convenience like a macro)
        public void ASSERT_ERR(bool condition, CoreErrorType type, string msg = null)
        {
            if (condition)
                push_err(type, msg);
        }

        // Push an error to the cores' error handler
        public void push_err(CoreErrorType type, string msg = null)
        {
            CoreError err = new CoreError(this.state, this.traceback, type, msg);
            // Register the error with the handler
            this.state.err_handler.register_err(err);
            this.state.had_err = true;
            throw new CoreErrException(err);
        }

        // Perform a function call with a trace
        private Trace func_call_trace(WavyFunction func, WavyItem[] args)
        {
            // Then create a new Trace instance referencing that frame
            Trace trace = new Trace(func_call(func, args));
            // Push the trace to the traceback
            this.traceback.push_call_trace(trace);
            return trace;
        }

        // Perform a function call
        private FuncFrame func_call(WavyFunction func, WavyItem[] args)
        {
            // Check if the function is native
            if(func.is_native)
            {
                this.native_interface.call_native_func(this.vm.state.current_file, func.name, args);
                return null;
            }
            else
            {
                // First create a new FuncFrame to push to the function stack
                FuncFrame frame = new FuncFrame(this, func.name, ExecStack.deep_copy(this, this.exec_stack));
                // Then push the frame to the FuncStack
                this.func_stack.push(frame);
                return frame;
            }
        }

        // Check if we have reached the end
        public bool end()
        {
            return (this.state.multi_core_state == MultiCoreState.ABORTED) || (this.state.multi_core_state == MultiCoreState.DONE) || (pc >= state.opcode_count) || (bytecode[pc] == (Int32)Opcode.END);
        }

        // Called once the current bytecode execution has been completed
        public void goto_next()
        {
            pc++;
        }

        // Returns the next bytecode instance
        public Int32 get_next()
        {
            return bytecode[pc];
        }

        // Get the next argument
        public Int32 get_arg()
        {
            goto_next();
            return get_next();
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

    public enum MultiCoreState
    {
        
        BLOCKED,    // For when the core has requested data, and has been denied and is waiting
        NIL,        // For when the core has not registered the bytecode sequence
        READY,      // For when the core has been created, but is not running code yet
        RUNNING,    // For when the core is running code
        DONE,       // For when the core is done execution
        ABORTED,
        SUSPENDED,
    }

    // Represents a state of the core at a particular time
    public class CoreState
    {
        public int id;
        public MultiCoreState multi_core_state;
        public ErrorHandler err_handler;
        public int opcode_count;
        public bool currently_interpreting;
        public int func_depth;
        public bool had_err;
        
        public CoreState(int id, ErrorHandler err_handler, int opcode_count, bool currently_interpreting, int func_depth)
        {
            this.id = id;
            this.multi_core_state = MultiCoreState.NIL; // Initialise the multi_core_state to NIL
            this.err_handler = err_handler;
            this.opcode_count = opcode_count;
            this.currently_interpreting = currently_interpreting;
            this.func_depth = func_depth;
            this.had_err = false;
        }

        // Create a fresh CoreState instance
        public static CoreState setup(int id)
        {
            return new CoreState(id, new CoreErrorHandler(), 0, false, 0);
        }
    }
}