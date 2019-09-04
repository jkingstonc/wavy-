/*
* James Clarke
* 22/08/19
*/

#define VM_MULTI_CORE

using System.Threading;
using wavynet.file.wcformat;
using wavynet.vm.core;
using wavynet.vm.data;
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
        public WC wc_profile;

        public VM(WC wc_profile) : base("VM")
        {
            this.thread = new Thread(() => run());
            this.wc_profile = wc_profile;
        }

        // On setup, the vm initialises the VM state
        // It then initialises the BankManager, which in turn registers all the file bank data to the Bank's in memory
        // It then creates a LinkManager, which in turn generates all Assemblies for required dll files
        public override void setup()
        {
            LOG("setting up '" + this.wc_profile.filename+"'");
            VM.state = VMState.setup(this.wc_profile.filename);
            this.core_manager = new CoreManager(this);
            this.bank_manager = new BankManager(this.wc_profile.cbank.ToArray());
            this.link_manager = new LinkManager();
            if (this.wc_profile.require_dll)
                this.link_manager.add_dll(this.wc_profile.filename, this.wc_profile.path);
            this.core_manager.setup();
            this.bank_manager.setup();
            this.link_manager.setup();
        }

        public override void start()
        {
            base.start();
            this.core_manager.start();
            this.bank_manager.start();
            this.link_manager.start();
            this.thread.Start();
        }

        public override void run()
        {
            base.run();
            try
            {
                ASSERT_ERR(this.wc_profile.magic != WC.MAGIC_SEQUENCE, VMErrorType.INVALID_WC, "Magic sequence incorrect for file '"+this.wc_profile.filename+"'!");
                // Create and run the main core
                this.core_manager.new_core_event += this.core_manager.create_and_run;
                this.core_manager.new_core_event?.Invoke(this, new CoreCreateEventArgs(-1, this.wc_profile.bytecode));
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