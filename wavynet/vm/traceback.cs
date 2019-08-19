using System;
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

        public void display()
        {
            // First check if we are not in a function / traceback is empty
            if (this.trace_back.Count > 0)
            {
                Console.WriteLine("Traceback");
                Console.WriteLine("---------");
                for (int i = 0; i < this.trace_back.Count; i++)
                {
                    Console.Write(i + ": ");
                    this.trace_back[i].display();
                }
            }
            else
            {
                Console.WriteLine("Empty traceback");
            }
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

        public void display()
        {
            Console.WriteLine(this.frame.get_func_name());
        }

        public FuncFrame get_frame()
        {
            return this.frame;
        }
    }
}
