namespace wavynet.vm
{
    class Bytecode
    {
        public enum Types
        {
            END = 0,    // End of a bytecode sequence
            NOP = 1,    // No-Operations [we skip this]

            UN_NOT,         // Binary not
            UN_POSITIVE,    // Positive value of a numeric value
            UN_NEGATIVE,    // Negative value of a numeric value

            BIN_ADD,
            BIN_SUB,
            BIN_MUL,
            BIN_DIV,
            BIN_MOD,
            BIN_REM,

            BANK_VAR,         // Define a variable to the bank
            ASSIGN_VAR,       // Assign a value to a variable in the bank

            BANK_CLASS,       // Define a class [add a class definition to the MBank]
            BANK_FUNCTION,    // Define a function [add a function definition to the MBank]
        }
    }

    // An instance of a loaded bytecode instruction
    struct BytecodeInstance
    {
        public int op;
        public int arg;

        // Instance without arg
        public BytecodeInstance(int op)
        {
            this.op = op;
            this.arg = 0;
        }

        // Instance with arg
        public BytecodeInstance(int op, int arg)
        {
            this.op = op;
            this.arg = arg;
        }
    }
}
