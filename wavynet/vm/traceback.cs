using System.Collections.Generic;

namespace wavynet.vm
{
    // A traceback contains information about wavycore actions
    public class TraceBack
    {
        public List<Trace> trace_back = new List<Trace>();

        // Add a new trace to the trace_back
        public void push_call_trace(Trace trace)
        {
            this.trace_back.Add(trace);
        }
    }

    // A Trace is a representation of when the vm enters a new state, such as a function call, exception interruption etc
    public struct Trace
    {
        private FuncFrame frame;

        public Trace(FuncFrame frame)
        {
            this.frame = frame;
        }

        public FuncFrame get_frame()
        {
            return this.frame;
        }
    }
}
