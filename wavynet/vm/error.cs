using System;
using System.Collections.Generic;

namespace wavynet.vm
{
    class ErrorHandler
    {
        public List<Error> errs = new List<Error>();
        public int count = -1;

        public void register_err(CoreState state, TraceBack traceback, ErrorType type)
        {
            this.errs.Add(new Error(state, traceback, type));
            this.count++;
        }

        public void register_err(CoreState state, TraceBack traceback, ErrorType type, string msg)
        {
            this.errs.Add(new Error(state, traceback, type, msg));
            this.count++;
        }

        public void say_latest()
        {
            Console.WriteLine(errs[count].get_msg());
            Console.WriteLine("Traceback:" + errs[count].get_traceback());
        }
    }

    public class Error
    {
        private CoreState state;
        private TraceBack traceback;
        private ErrorType type;
        private string msg= "";

        public Error(CoreState state, TraceBack traceback, ErrorType type)
        {
            this.state = state;
            this.traceback = traceback;
            this.type = type;
        }

        public Error(CoreState state, TraceBack traceback, ErrorType type, string msg)
        {
            this.state = state;
            this.traceback = traceback;
            this.type = type;
            this.msg = msg;
        }

        public CoreState get_state()
        {
            return this.state;
        }

        public TraceBack get_traceback()
        {
            return this.traceback;
        }

        public ErrorType get_type()
        {
            return this.type;
        }

        public string get_msg()
        {
            return this.msg;
        }
    }
        

    public enum ErrorType
    {
        // Opcode is an invalid integer
        INVALID_OP,
    }
}
