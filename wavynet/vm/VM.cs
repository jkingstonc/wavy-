﻿/*
* James Clarke
* 22/08/19
*/

using System;
using System.Threading;
using wavynet.profile;
using wavynet.vm.data;

namespace wavynet.vm
{
    public class VM
    {
        public Thread thread;
        public VMState state;
        public CoreManager core_manager;
        public BankManager bank_manager;
        public LinkManager link_manager;

        public WCProfile wc_profile;

        // Should the vm emulate multi threading using multiple cores
        public static bool MULTI_CORE = true;
        // Should we cache the value retrieved from the bank for use in multiple cores
        public static bool MULTI_CORE_BANK_CACHING = true;

        // FOR DEBUGGING
        public static bool CORE_DEBUG = true;
        public static bool INSTR_DEBUG = false;
        public static bool TRACE_DEBUG = false;


        public VM()
        {
            this.thread = new Thread(() => run());
        }

        public void setup(WCProfile wc_profile)
        {
            this.state = VMState.setup();
            this.bank_manager = new BankManager();
            this.link_manager = new LinkManager();
            this.link_manager.bind_all_dll(new string[] { @"F:\OneDrive - Lancaster University\programming\c#\DLLTest\bin\Debug\netstandard2.0\DLLTest.dll" });
        }

        public void start()
        {
            this.thread.Start();
        }

        public void close()
        {
            Console.WriteLine("# closing vm #");
        }

        private void run()
        {
            try
            {
                int count = 2;
                Int32[] sequence = new Int32[count * 6];
                for (var i = 0; i < count; i += 6)
                {
                    sequence[i] = (Int32)Opcode.END;
                }

                this.core_manager = new CoreManager(this);
                // Create and run the main core
                this.core_manager.new_core_event += this.core_manager.create_and_run;
                this.core_manager.new_core_event?.Invoke(this, new CoreCreateEventArgs(-1, sequence));
                this.core_manager.new_core_event?.Invoke(this, new CoreCreateEventArgs(-1, sequence));
                // Join all core threads to this (wait for all cores to finish)
                this.core_manager.join_all_cores();
            }
            // This should actually catch CoreErrExceptions aswel as we need to handle those appropriately
            catch (VMErrException)
            {
                this.core_manager.abort_all();
                this.state.err_handler.say_latest();
            }
            close();
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
            VMError err = new VMError(this.state, type, msg);
            // Register the error with the handler
            this.state.err_handler.register_err(err);
            this.state.had_err = true;
            throw new VMErrException(err);
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