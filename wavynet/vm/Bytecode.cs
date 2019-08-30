/*
 * James Clarke
 * 22/08/19
 */

namespace wavynet.vm
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
        BANK_CLASS,       // Define a class [add a class definition to the MBank]
        BANK_FUNCTION,    // Define a function [add a function definition to the MBank]
        BANK_ASSIGN,      // Assign a value to a variable in the bank

        NEW,              // Create a new WavyObject instance and push to the stack
        INVOKE_FUNC,      // Invoke a function
        INVOKE_METHOD,    // Invoke a method

        GOTO,             // Goto a given offset instruction
        IF_ZERO,          // If the val on the stack is zero, branch to the offset instruction
        IF_NZERO,         // If the val on the stack is not zero, branch to the offset instruction
        IF_GRT,           // If the val1 (top) on the stack is greater than val2, branch to the offset instruction
        IF_GRTE,          // If the val1 (top) on the stack is greater or equal than val2, branch to the offset instruction
        IF_LT,            // If the val1 (top) on the stack is less than val2, branch to the offset instruction
        IF_LTE,           // If the val1 (top) on the stack is less or equal than val2, branch to the offset instruction
    }
}