/*
 * James Clarke
 * 22/08/19
 */

using System;
using System.Collections.Generic;
using wavynet.vm.core;

namespace wavynet.vm
{
    public class VM_Stack<T>
    {
        // SP always points to the top item
        private int sp;
        private List<T> stack;

        private Core core;

        public VM_Stack(Core core)
        {
            this.sp = -1;
            this.stack = new List<T>();
            this.core = core;
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
            core.ASSERT_ERR(empty(), CoreErrorType.STACK_UNDERFLOW, "Stack underflow!");
            return this.stack[this.sp--];
        }

        public T peek()
        {
            core.ASSERT_ERR(empty(), CoreErrorType.STACK_UNDERFLOW, "Stack underflow!");
            return this.stack[this.sp];
        }

        // Set the top item on the stack
        public void set_top(T item)
        {
            this.stack[this.sp] = item;
        }

        public int get_sp()
        {
            return this.sp;
        }

        // Set the value of the stack at a given depth
        public void set_n(T item, int n)
        {
            core.ASSERT_ERR(n < 0 || n > sp, CoreErrorType.INVALID_SP_RANGE, "Stack Pointer is out of range!");
            this.stack[n] = item;
        }

        // Check if the stack is empty
        public bool empty()
        {
            return this.sp < 0;
        }

        public int size()
        {
            return this.stack.Count;
        }
    }

    // Stack of function frames, containing info about function calls
    public class FuncStack
    {
        private VM_Stack<FuncFrame> stack;
        public const int MAX_SP = 1024;

        public FuncStack(Core core)
        {
             this.stack = new VM_Stack<FuncFrame>(core);
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

        // Get the Stack Pointer for the frame stack
        public int get_sp()
        {
            return this.stack.get_sp();
        }
    }

    // Frame that sits on the FuncStack
    public struct FuncFrame
    {
        public string func_name;
        public int pc;
        public WItem[] locals;
        public ExecStack exec_stack;
        public Int32[] bytecode;

        public FuncFrame(Core core, string func_name, int pc, WItem[] locals, ExecStack exec_stack, Int32[] bytecode)
        {
            this.func_name = func_name;
            this.pc = pc;
            this.locals = locals;
            this.exec_stack = exec_stack;
            this.bytecode = bytecode;
        }
    }

    // Stack of operand frames, containing operands used for opcode actions
    public class ExecStack
    {
        public VM_Stack<ExecFrame> stack;

        public ExecStack(Core core)
        {
            this.stack = new VM_Stack<ExecFrame>(core);
        }

        // Pop the top item from the execution stack
        public WItem pop()
        {
            ExecFrame frame = this.stack.pop();
            if(frame != null)
            {
                return frame.get_obj();
            }
            return null;
        }

        // Peek the top item from the execution stack
        public WItem peek()
        {
            return this.stack.peek().get_obj();
        }

        // Push an item to the stack
        public void push(WItem item)
        {
            this.stack.push(new ExecFrame(item));
        }

        public static ExecStack deep_copy(Core core, ExecStack original)
        {
            ExecStack copy = new ExecStack(core);
            copy.stack = original.stack;
            return copy;
        }
    }

    // Frame that sits on the ExecStack
    public class ExecFrame
    {
        private WItem item;

        public ExecFrame(WItem item)
        {
            this.item = item;
        }

        public WItem get_obj()
        {
            return this.item;
        }
    }
}