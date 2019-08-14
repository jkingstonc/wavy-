using System;
using System.Collections.Generic;

namespace wavynet.vm
{
    class ErrorHandler
    {
        public List<Error> errs = new List<Error>();
        public int count = -1;

        public void register_err(CoreState state, ErrorType type)
        {
            this.errs.Add(new Error(state, type));
            this.count++;
        }

        public void register_err(CoreState state, ErrorType type, string msg)
        {
            this.errs.Add(new Error(state, type, msg));
            this.count++;
        }

        public void say_latest()
        {
            Console.WriteLine(errs[count].get_msg());
        }
    }

    public class Error
    {
        private CoreState state;
        private ErrorType type;
        private string msg= "";

        public Error(CoreState state, ErrorType type)
        {
            this.state = state;
            this.type = type;
        }

        public Error(CoreState state, ErrorType type, string msg)
        {
            this.state = state;
            this.type = type;
            this.msg = msg;
        }

        public CoreState get_state()
        {
            return this.state;
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
