using System.Collections.Generic;

namespace wavynet.vm
{
    class vm_stack<T>
    {
        // SP always points to the top item
        private int sp;
        private List<T> stack;

        public vm_stack()
        {
            this.sp = -1;
            this.stack = new List<T>();
        }

        public void push(T item)
        {
            this.sp++;
            this.stack[this.sp] = item;          
        }

        public T pop()
        {
            return this.stack[this.sp--];
        }

        public T peek()
        {
            return this.stack[this.sp];
        }

        // Set the top item on the stack
        public void set_top(T item)
        {
            this.stack[this.sp] = item;
        }

        // Set the value of the stack at a given depth
        public void set_n(T item, int n)
        {
            if (n > 0 && n < sp)
            {
                this.stack[n] = item;
            }
            else
            {
                // depth out of bounds
            }
        }
    }

    public class ExecStack
    {
        public Stack<ExecFrame> stack = new Stack<ExecFrame>();
    }

    public class ExecFrame
    {

    }
}
