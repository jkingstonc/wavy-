/*
* James Clarke
* 22/08/19
*/

using System;
using System.Threading;
using wavynet.profile;
using wavynet.vm.core;
using wavynet.vm.data;
using wavynet.vm.data.gc;
using wavynet.vm.native;

namespace wavynet.vm
{
    public class VM : VMComponent
    {
        public Thread thread;
        public static VMState state;
        public CoreManager core_manager;
        public BankManager bank_manager;
        public LinkManager link_manager;
        public GarbageManager garbage_manager;

        // Should the vm emulate multi threading using multiple cores
        public static bool MULTI_CORE = false;
        // Should we cache the value retrieved from the bank for use in multiple cores
        public static bool MULTI_CORE_BANK_CACHING = true;
        // Should the VM handle gc rather than the .NET runtime
        public static bool GARBAGE_COLLECTION = true;

        public VM() : base("VM")
        {
            this.thread = new Thread(() => run());
        }

        // On setup, the vm initialises the VM state
        // It then initialises the BankManager, which in turn registers all the file bank data to the Bank's in memory
        // It then creates a LinkManager, which in turn generates all Assemblies for required dll files
        public void setup(string current_file, WCProfile wc_profile)
        {
            LOG("setting up '" + current_file+"'");
            VM.state = VMState.setup(current_file);
            this.core_manager = new CoreManager(this);
            this.bank_manager = new BankManager(wc_profile.bank_profile);
            this.link_manager = new LinkManager();
            this.garbage_manager = new GarbageManager(this.bank_manager);
            this.core_manager.setup();
            this.bank_manager.setup();
            this.link_manager.setup();
            this.garbage_manager.setup();
        }

        public override void start()
        {
            base.start();
            this.core_manager.start();
            this.bank_manager.start();
            this.link_manager.start();
            if(VM.GARBAGE_COLLECTION)
                this.garbage_manager.start();
            this.thread.Start();
        }

        public override void run()
        {
            base.run();
            try
            {
                Int32[] sequence = new Int32[] 
                { 
                    (Int32)Opcode.END,
                };
                if (VM.GARBAGE_COLLECTION)
                    this.garbage_manager.run();
                // Create and run the main core
                this.core_manager.new_core_event += this.core_manager.create_and_run;
                this.core_manager.new_core_event?.Invoke(this, new CoreCreateEventArgs(-1, sequence));
                // Join all core threads to this (wait for all cores to finish)
                this.core_manager.join_all_cores();
            }
            // This should actually catch CoreErrExceptions aswel as we need to handle those appropriately
            catch (VMErrException)
            {
                this.core_manager.abort_all();
                VM.state.err_handler.say_latest();
            }
            close();
        }

        public override void close()
        {
            base.close();
            this.core_manager.close();
            this.bank_manager.close();
            this.link_manager.close();
            if (VM.GARBAGE_COLLECTION)
                this.garbage_manager.close();
        }
    }

    // Represents a state of the vm at a particular time
    public class VMState
    {
        public string current_file;
        public ErrorHandler err_handler;
        public bool had_err;

        public VMState(string current_file, ErrorHandler err_handler)
        {
            this.current_file = current_file;
            this.err_handler = err_handler;
            this.had_err = false;
        }

        // Create a fresh VMState instance
        public static VMState setup(string current_file)
        {
            return new VMState(current_file, new VMErrorHandler());
        }
    }
}