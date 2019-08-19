﻿using System.Collections.Generic;

namespace wavynet.vm
{
    public class VM_Stack<T>
    {
        // SP always points to the top item
        private int sp;
        private List<T> stack;

        public VM_Stack()
        {
            this.sp = -1;
            this.stack = new List<T>();
        }

        public void push(T item)
        {
            this.sp++;
            if(this.sp >= this.stack.Count)
                this.stack.Add(item);
            else
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

    // Stack of function frames, containing info about function calls
    public class FuncStack
    {
        private VM_Stack<FuncFrame> stack;

        public FuncStack()
        {
             this.stack = new VM_Stack<FuncFrame>();
        }

        // Pop the top frame from the execution stack
        public FuncFrame pop()
        {
            return this.stack.pop();
        }

        // Peek the top frame from the execution stack
        public FuncFrame peek()
        {
            return this.stack.peek();
        }

        // Push a frame to the stack
        public void push(FuncFrame frame)
        {
            this.stack.push(frame);
        }
    }

    // Frame that sits on the FuncStack
    public class FuncFrame
    {
        private string func_name;
        private ExecStack exec_stack;

        public FuncFrame(string func_name)
        {
            this.func_name = func_name;
            this.exec_stack = new ExecStack();
        }

        public string get_func_name()
        {
            return this.func_name;
        }

        public ExecStack get_stack()
        {
            return this.exec_stack;
        }
    }

    // Stack of operand frames, containing operands used for opcode actions
    public class ExecStack
    {
        public VM_Stack<ExecFrame> stack;

        public ExecStack()
        {
            this.stack = new VM_Stack<ExecFrame>();
        }

        // Pop the top item from the execution stack
        public WavyItem pop()
        {
            return this.stack.pop().get_obj();
        }

        // Peek the top item from the execution stack
        public WavyItem peek()
        {
            return this.stack.peek().get_obj();
        }

        // Push an item to the stack
        public void push(WavyItem item)
        {
            this.stack.push(new ExecFrame(item));
        }
    }

    // Frame that sits on the ExecStack
    public class ExecFrame
    {
        private WavyItem item;

        public ExecFrame(WavyItem item)
        {
            this.item = item;
        }

        public WavyItem get_obj()
        {
            return this.item;
        }
    }
}