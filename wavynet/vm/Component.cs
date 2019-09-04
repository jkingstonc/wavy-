/*
 * James Clarke
 * 22/08/19
 */
using System;
using wavynet.vm.core;

namespace wavynet.vm
{
    /*
     * Component represents an abstract base class
     * for a wavy runtime component to implement
     */
    public abstract class Component
    {
        protected string component_id;

        public Component(string component_id)
        {
            this.component_id = component_id;
        }

        // Called on component setup
        public virtual void setup()
        {
            Wavy.logger.log("[" + this.component_id + "] " + "setting up");
        }

        // Called on component start
        public virtual void start()
        {
            Wavy.logger.log("[" + this.component_id + "] " + "starting");
        }

        // Called on component run
        public virtual void run()
        {
            Wavy.logger.log("[" + this.component_id + "] " + "running");
        }

        // Called on component close
        public virtual void close()
        {
            Wavy.logger.log("[" + this.component_id + "] " + "closing");
        }

        // Utility for the component to log to the runtime logger
        protected void LOG(string msg)
        {
            Wavy.logger.log("[" + this.component_id + "] " + msg);
        }
    }

    /*
     * CoreComponent represents an abstract base
     * class for a Core to be implemented in
     */
    public abstract class CoreComponent : Component
    {
        public CoreState state;
        public TraceBack traceback;

        public CoreComponent(string component_id, int core_id) : base(component_id+core_id)
        {

        }

        public override void setup()
        {
            base.setup();
        }

        // Used when we may need to register an error (for convenience like a macro)
        public void ASSERT_ERR(bool condition, CoreErrorType err_type, string msg = null)
        {
            if (condition)
                push_err(err_type, msg);
        }

        // Push an error to the cores' error handler
        public void push_err(CoreErrorType err_type, string msg = null)
        {
            CoreError err = new CoreError(this.state, this.traceback, err_type, msg);
            // Register the error with the handler
            this.state.err_handler.register_err(err);
            this.state.had_err = true;
            throw new CoreErrException(err);
        }
    }

    /*
     * VMComponent represents an abstract base class for a component
     * that the VM requires to operate to be implemented in
     */
    public class VMComponent : Component
    {
        public VMComponent(string component_id) : base(component_id)
        {

        }

        // Used when we may need to register an error (for convenience like a macro)
        public void ASSERT_ERR(bool condition, VMErrorType err_type, string msg = null)
        {
            if (condition)
                push_err(err_type, msg);
        }

        // Push an error to the cores' error handler
        public void push_err(VMErrorType err_type, string msg = null)
        {
            VMError err = new VMError(VM.state, err_type, msg);
            // Register the error with the handler
            VM.state.err_handler.register_err(err);
            VM.state.had_err = true;
            throw new VMErrException(err);
        }
    }
}