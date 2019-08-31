/*
 * James Clarke
 * 22/08/19
 */

using System;
using System.Collections.Generic;
using wavynet.vm.core;

namespace wavynet.vm
{
    public abstract class ErrorHandler
    {
        public List<Error> errs = new List<Error>();
        public int count = -1;

        public void register_err(Error error)
        {
            this.errs.Add(error);
            this.count++;
        }

        public abstract void say_latest();
    }

    public class CoreErrorHandler : ErrorHandler
    {
        public override void say_latest()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("** CORE "+ ((CoreError)errs[count]).get_state().id+ " ERROR ["+ ((CoreError)errs[count]).get_type()+"]**");
            if (errs[count].get_msg() != null)
                Console.WriteLine("'" + errs[count].get_msg() + "'");
            if (Core.TRACE_DEBUG)
                ((CoreError)errs[count]).get_traceback().display();
        }
    }

    public class VMErrorHandler : ErrorHandler
    {
        public override void say_latest()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("** VM ERROR [" + ((VMError)errs[count]).get_type() + "]**");
            if(errs[count].get_msg() != null)
                Console.WriteLine("'" + errs[count].get_msg() + "'");
        }
    }

    public class Error
    {
        protected string msg;

        public string get_msg()
        {
            return this.msg;
        }
    }

    public class CoreError : Error
    {
        private CoreState state;
        private TraceBack traceback;
        private CoreErrorType type;
        private bool fatal;

        public CoreError(CoreState state, TraceBack traceback, CoreErrorType type, string msg = null)
        {
            this.state = state;
            this.traceback = traceback;
            this.type = type;
            // Check if the error is fatal by checking the error type
            this.fatal = ((int)type) > 0;
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

        public CoreErrorType get_type()
        {
            return this.type;
        }

        public bool is_fatal()
        {
            return this.fatal;
        }
    }

    public class VMError : Error
    {
        private VMState state;
        private VMErrorType type;

        public VMError(VMState state, VMErrorType type, string msg = null)
        {
            this.state = state;
            this.type = type;
            this.msg = msg;
        }

        public VMState get_state()
        {
            return this.state;
        }

        public VMErrorType get_type()
        {
            return this.type;
        }
    }


    public enum CoreErrorType
    {
        INVALID_PC_RANGE,           // Program counter is out of range of the bytecode
        INVALID_OP,                 // Opcode is an invalid integer
        INVALID_SP_RANGE,           // Stack pointer is out of range
        STACK_OVERFLOW,             // Stack upper bound has exceeded
        STACK_UNDERFLOW,            // Stack lower bound has exceeded
        INVALID_JUMP,               // When a jump is not to valid areas

        INVALID_BANK_ID,            // When an item is requested from the bank doesn't exist
        INVALID_MULTICORE_STATE,    // When a multicore operation fails due to an invalid multicore state
        UNEXPECTED_TYPE,            // When the core expects a WavyItem to have a certian type
        INVALID_LOCAL,
    }

    public enum VMErrorType
    {
        INVALID_CORE_ID,            // For when accessing cores uses an invalid id
        INVALID_CORE_COUNT,         // Invalid number of cores created
    }
}
