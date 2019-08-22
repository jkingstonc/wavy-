/*
 * James Clarke
 * 22/08/19
 */

using System;
using System.Collections.Generic;

namespace wavynet.vm
{
    public class ErrorHandler
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
            Console.WriteLine("** ERROR **");
            Console.WriteLine("'"+errs[count].get_msg()+ "'");
            if(VM.TRACE_DEBUG)
                errs[count].get_traceback().display();
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
        INVALID_PC_RANGE,   // Program counter is out of range of the bytecode
        INVALID_OP,         // Opcode is an invalid integer
        INVALID_SP_RANGE,   // Stack pointer is out of range
        STACK_OVERFLOW,     // Stack upper bound has exceeded
        STACK_UNDERFLOW,    // Stack lower bound has exceeded
        INVALID_JUMP,       // When a jump is not to valid areas
    }
}
