﻿/*
 * James Clarke
 * 22/08/19
 */

#define CORE_INSTR_DEBUG
#define CORE_TRACE_DEBUG

using System;
using System.Threading;
using wavynet.vm.data.items;
using wavynet.vm.native;

namespace wavynet.vm.core
{
    /* 
     * Represents a vm core
     * Performs fde cycle
    */
    public class Core : CoreComponent
    {
        private VM vm;
        private NativeInterface native_interface;
        public Thread thread;
        // Program counter
        public int pc;
        // The array of locals that the current function operates on
        public WItem[] locals;
        // The bytecode that the core is executing
        public Int32[] bytecode;
        // The function stack that this core is using
        public FuncStack func_stack;
        // The execution stack that this core is using
        public ExecStack exec_stack;

        // The max Program Counter, this determines program length
        public const int MAX_PC = 65536; // 2^16
        // Maximum ammount of allowed recursion
        public const int MAX_RECURSION = 100;

        private System.Diagnostics.Stopwatch watch;

        public Core(VM vm, int id) : base("Core", id)
        {
            this.vm = vm;
            this.native_interface = new NativeInterface(this.vm, this);
            this.state = new CoreState(id, new CoreErrorHandler(), 0, false, 0);
            this.pc = 0;
            this.locals = new WItem[0];
            this.traceback = new TraceBack();
            this.func_stack = new FuncStack(this);
            this.exec_stack = new ExecStack(this);
        }

        // Setup the core thread
        public void setup(Int32[] sequence)
        {
            base.setup();
            this.bytecode = sequence;
            this.state.opcode_count = sequence.Length;
            this.state.multi_core_state = MultiCoreState.READY;
            // Create a new EventWaitHandle to allow us to check when this core is finished
            // Create a new Thread and accociate the handle with it
            this.thread = new Thread(() => {
                this.evaluate_sequence();
                });
        }

        public override void start()
        {
            base.start();
            this.run();
        }

        // Run the core & start executing code
        public override void run()
        {
            base.run();
            this.thread.Start();
            this.watch = System.Diagnostics.Stopwatch.StartNew();
        }

        // When we want the core to suspend execution
        public void suspend()
        {
            ASSERT_ERR(this.state.multi_core_state == MultiCoreState.SUSPENDED, CoreErrorType.INVALID_MULTICORE_STATE, "Cannot suspend core that has already been suspended!");
            this.state.multi_core_state = MultiCoreState.SUSPENDED;

        }

        // When we want to resume the core operation
        public void resume()
        {
            ASSERT_ERR(this.state.multi_core_state != MultiCoreState.SUSPENDED, CoreErrorType.INVALID_MULTICORE_STATE, "Cannot resume core that has not been suspended!");
            this.state.multi_core_state = MultiCoreState.RUNNING;
        }

        // Main fde cycle
        public void evaluate_sequence()
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

#if CORE_INSTR_DEBUG
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("op: " + (Opcode)op);
                    Console.ForegroundColor = ConsoleColor.Yellow;
#endif

                    // Surround the execute phase in a try catch, this is so the vm can return safely to the error handling
                    // without breaking other vm components
                    try
                    {
                        switch ((Opcode)op)
                        {
                            case Opcode.PRINT:
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
#if VM_MULTI_CORE
                                    Console.WriteLine("{core "+this.state.id+"} "+peek_exec());
#else
                                        Console.WriteLine(peek_exec());
#endif
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    goto_next();
                                    break;
                                }
                            case Opcode.END:
                                {
                                    // Return an END state
                                    this.state.currently_interpreting = false;
                                    break;
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
                            case Opcode.LD_CONST:
                                {
                                    Int32 id = get_arg();
                                    push_exec(this.vm.bank_manager.request_c_item(this, id));
                                    this.vm.bank_manager.release_c_item(this, id);
                                    goto_next();
                                    break;
                                }
                            case Opcode.LD_VAR:
                                {
                                    Int32 id = get_arg();
                                    push_exec(this.vm.bank_manager.request_m_item(this, id));
                                    this.vm.bank_manager.release_m_item(this, id);
                                    goto_next();
                                    break;
                                }
                            case Opcode.LD_LOC:
                                {
                                    // Check if we are actually in a function
                                    ASSERT_ERR(this.state.func_depth > 0, CoreErrorType.INVALID_LOCAL, "Cannot load local from non-function state!");
                                    Int32 index = get_arg();
                                    // Check if we have a valid local index
                                    ASSERT_ERR(index > this.locals.Length-1, CoreErrorType.INVALID_LOCAL, "Local index is larger than locals size!");
                                    push_exec(this.locals[index]);
                                    goto_next();
                                    break;
                                }
                            case Opcode.BANK_VAR:
                                {
                                    Int32 id = get_arg();
                                    this.vm.bank_manager.request_m_item(this, id);
                                    this.vm.bank_manager.assign_item(this, id, pop_exec());
                                    this.vm.bank_manager.request_m_item(this, id);
                                    goto_next();
                                    break;
                                }
                            case Opcode.BANK_ASSIGN:
                                {
                                    goto_next();
                                    break;
                                }
                            case Opcode.LOCAL_ASSIGN:
                                {
                                    goto_next();
                                    break;
                                }
                            case Opcode.MAKE_CLASS:
                                {
                                    break;
                                }
                            case Opcode.MAKE_FUNC:
                                {
                                    break;
                                }
                            case Opcode.NEW:
                            {
                                break;
                            }
                            case Opcode.INVOKE_FUNC:
                                {
                                    WFunction func = expect_wfunc(pop_exec());
#if CORE_TRACE_DEBUG
                                    func_call_trace(func);
#else

                                    func_call(func);
#endif
                                    break;
                                }
                            case Opcode.RETURN:
                                {
                                    func_return();
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
                            case Opcode.INCREMENT:
                                {
                                    WItem num = expect_numeric(peek_exec());
                                    num.value = num.value + 1;
                                    goto_next();
                                    break;
                                }
                            case Opcode.DECREMENT:
                                {
                                    WItem num = expect_numeric(peek_exec());
                                    num.value = num.value - 1;
                                    goto_next();
                                    break;
                                }
                            case Opcode.PSH_ZERO:
                                {
                                    push_exec((Wint)0);
                                    goto_next();
                                    break;
                                }
                            case Opcode.PSH_NULL:
                                {
                                    push_exec((Wnull)null);
                                    goto_next();
                                    break;
                                }
                            default:
                                {
                                    // We have an invalid opcode
                                    push_err(CoreErrorType.INVALID_OP, "op: " + op);
                                    break;
                                }
                        }

                        // Program counter is out of range
                        ASSERT_ERR( pc < 0 || pc > MAX_PC, CoreErrorType.INVALID_PC_RANGE);
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
            close();
        }

        // Called when we want to close this core thread
        public override void close()
        {
            this.watch.Stop();
            LOG("closing after " + watch.ElapsedMilliseconds + " millis");
            // Return an END state
            this.state.currently_interpreting = false;
            // Change the state of the multi_core_state
            this.state.multi_core_state = MultiCoreState.DONE;
            this.vm.core_manager.close_core(this.state.id);
        }

        // We expect a WavyItem to be a numeric
        private dynamic expect_numeric(WItem item)
        {
            ASSERT_ERR(!(item is Wint) && !(item is Wdouble), CoreErrorType.UNEXPECTED_TYPE, "Expected numeric, but got: " + item.GetType());
            return item;
        }

        // We expect a WavyItem to be an int
        private dynamic expect_int(WItem item)
        {
            ASSERT_ERR(item is Wint, CoreErrorType.UNEXPECTED_TYPE, "Expected int, but got: " + item.GetType());
            return (Wint)item;
        }

        // We expect a WavyItem to be a WavyFunction
        private dynamic expect_wfunc(WItem item)
        {
            ASSERT_ERR(!(item is WFunction), CoreErrorType.UNEXPECTED_TYPE, "Expected WavyFunction, but got: " + item.GetType());
            return item;
        }

        // We expect a WavyItem to be a WavyObject
        private dynamic expect_wobject(WItem item)
        {
            ASSERT_ERR(!(item is WObject), CoreErrorType.UNEXPECTED_TYPE, "Expected WavyObject, but got: " + item.GetType());
            return item;
        }

        // Perform a function call with a trace
        private Trace func_call_trace(WFunction func)
        {
            // Then create a new Trace instance referencing that frame
            Trace trace = new Trace(func_call(func));
            // Push the trace to the traceback
            this.traceback.push_call_trace(trace);
            return trace;
        }

        // Perform a function call
        private FuncFrame func_call(WFunction func)
        {
            ASSERT_ERR(this.state.func_depth > MAX_RECURSION, CoreErrorType.MAX_RECURSION, "Maxmimum recursion depth achieved!");
            this.state.func_depth++;
            // Set the size of the locals to the args count & locals count
            this.locals = new WItem[func.args_size + func.locals_size];
            // Then define the function arguments passed in that were on the exec stack
            for (int i = 0; i < func.args_size; i++)
                this.locals[i] = exec_stack.pop();
            // First create a new FuncFrame to push to the function stack
            FuncFrame frame = new FuncFrame(this, func.name, this.pc, this.locals, this.exec_stack, this.bytecode);
            // Then push the frame to the FuncStack
            this.func_stack.push(frame);
            // Check if the function is native
            if (func.is_native)
            {
                // Natively call the func
                push_exec(this.native_interface.call_native_func(VM.state.current_file, func.name, this.locals));
                // Becuase native functions can't implement Opcode.RETURN, we need to manually return & advance
                func_return();
                goto_next();
            }
            else
            {
                this.pc = 0;
                this.exec_stack = new ExecStack(this);
                this.bytecode = func.bytecode;
                this.state.opcode_count = this.bytecode.Length;
            }
            return frame;
        }

        private void func_return()
        {
            this.state.func_depth--;
            // First get the return value, by poping it from the exec stack
            WItem return_value = this.exec_stack.pop();
            // Then restore the state of the core
            FuncFrame previous_frame = this.func_stack.pop();
            this.pc = previous_frame.pc;
            this.locals = previous_frame.locals;
            this.exec_stack = previous_frame.exec_stack;
            this.bytecode = previous_frame.bytecode;
            this.state.opcode_count = this.bytecode.Length;
            // Then push the returned value to the exec stack
            this.exec_stack.push(return_value);
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
            return (Int32)bytecode[pc];
        }

        // Get the next argument
        public Int32 get_arg()
        {
            goto_next();
            return (Int32)bytecode[pc];
        }

        // Pop from the current execution stack in use
        public WItem pop_exec()
        {
            // We then get the stack and pop the top value
            WItem item = exec_stack.pop();
            if (item != null)
                return item;
            return null;
        }

        // Pop from the current execution stack in use
        public WItem peek_exec()
        {
            return exec_stack.peek();
        }

        // Pop from the current execution stack in use
        public void push_exec(WItem item)
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
    public struct CoreState
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
    }
}