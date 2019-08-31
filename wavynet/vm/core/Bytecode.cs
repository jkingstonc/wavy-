/*
 * James Clarke
 * 22/08/19
 */

namespace wavynet.vm.core
{
    public enum Opcode
    {
        END = 0x0,      // End of a bytecode sequence
        NOP = 0x1,      // No-Operations [we skip this]

        POP_EXEC,       // Pop the top item from the execution stack

        UN_NOT,         // Binary not
        UN_POSITIVE,    // Positive value of a numeric value
        UN_NEGATIVE,    // Negative value of a numeric value

        BIN_ADD,
        BIN_SUB,
        BIN_MUL,
        BIN_DIV,
        BIN_MOD,
        BIN_REM,

        LD_LIT,           // Load a literal from the LBank to the stack
        LD_VAR,           // Load a var from the MBank to the stack
        LD_LOC,           // Load a local variable to the stack

        BANK_VAR,         // Define a variable to the bank
        BANK_ASSIGN,      // Assign a value to a variable in the bank
        LOCAL_ASSIGN,     // Store a value to a local variable index

        MAKE_CLASS,       // Takes a class specification, and generates a WavyClass which gets loaded to the MBank
        MAKE_FUNC,        // Takes a function specification, and generates a WavyFunction which gets loaded to the MBank

        NEW,              // Create a new WavyObject instance and push to the stack
        INVOKE_FUNC,      // Invoke a function or method

        GOTO,             // Goto a given offset instruction
        IF_ZERO,          // If the val on the stack is zero, branch to the offset instruction
        IF_NZERO,         // If the val on the stack is not zero, branch to the offset instruction
        IF_GRT,           // If the val1 (top) on the stack is greater than val2, branch to the offset instruction
        IF_GRTE,          // If the val1 (top) on the stack is greater or equal than val2, branch to the offset instruction
        IF_LT,            // If the val1 (top) on the stack is less than val2, branch to the offset instruction
        IF_LTE,           // If the val1 (top) on the stack is less or equal than val2, branch to the offset instruction
    }
}