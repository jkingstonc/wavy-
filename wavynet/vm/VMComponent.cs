namespace wavynet.vm
{
    public abstract class Component
    {
        // Push an error to the correct error handler
        public abstract void push_err(dynamic err_type, string msg = null);
    }

    public class VMComponent : Component
    {
        protected string component_id;

        public virtual void setup()
        {
            Wavy.logger.log("[" + this.component_id + "] " + "setting up");
        }

        public virtual void start()
        {
            Wavy.logger.log("[" + this.component_id + "] " + "starting");
        }

        public virtual void run()
        {
            Wavy.logger.log("[" + this.component_id + "] " + "running");
        }

        public virtual void close()
        {
            Wavy.logger.log("[" + this.component_id + "] " + "closing");
        }

        // Used when we may need to register an error (for convenience like a macro)
        public void ASSERT_ERR(bool condition, dynamic err_type, string msg = null)
        {
            if (condition)
                push_err(err_type, msg);
        }

        // Push an error to the cores' error handler
        public override void push_err(dynamic err_type, string msg = null)
        {
            VMError err = new VMError(VM.state, err_type, msg);
            // Register the error with the handler
            VM.state.err_handler.register_err(err);
            VM.state.had_err = true;
            throw new VMErrException(err);
        }
    }
}