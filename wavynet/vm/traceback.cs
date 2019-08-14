using System.Collections.Generic;

namespace wavynet.vm
{
    // A traceback contains information about wavycore actions
    public class TraceBack
    {
        public List<Trace> trace_back = new List<Trace>();
    }

    // A trace con
    public struct Trace
    {
        private ExecFrame frame;

        public Trace(ExecFrame frame)
        {
            this.frame = frame;
        }

        public ExecFrame get_frame()
        {
            return this.frame;
        }
    }
}
