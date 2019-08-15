namespace wavynet.vm
{
    class Bytecode
    {
        public enum Types
        {
            END = 0,    // End of a bytecode sequence
            NOP = 1,    // No-Operations [we skip this]
            BIN_ADD,
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
