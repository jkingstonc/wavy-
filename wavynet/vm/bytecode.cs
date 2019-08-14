using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wavynet.vm
{
    class Bytecode
    {
        public enum Types
        {
            // End of a bytecode sequence
            END = 0,
        }
    }

    // An instance of a loaded opcode instruction
    struct OpcodeInstance
    {
        public int op;
        public int arg;

        public OpcodeInstance(int op, int arg)
        {
            this.op = op;
            this.arg = arg;
        }
    }
}
