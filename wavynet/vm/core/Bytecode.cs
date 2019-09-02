/*
 * James Clarke
 * 22/08/19
 */

namespace wavynet.vm.core
{
    public enum Opcode
    {
        PRINT = -1,
        END = 0x0,      // End of a bytecode sequence
        NOP = 0x1,      // No-Operations [we skip this]

        POP_EXEC = 0x2,       // Pop the top item from the execution stack

        UN_NOT = 0x3,         // Binary not
        UN_POSITIVE = 0x4,    // Positive value of a numeric value
        UN_NEGATIVE = 0x5,    // Negative value of a numeric value

        BIN_ADD = 0x6,
        BIN_SUB = 0x7,
        BIN_MUL = 0x8,
        BIN_DIV = 0x9,
        BIN_MOD = 0xA,
        BIN_REM = 0xB,

        LD_LIT = 0xC,           // Load a literal from the LBank to the stack
        LD_VAR = 0xD,           // Load a var from the MBank to the stack
        LD_LOC = 0xE,           // Load a local variable to the stack

        BANK_VAR = 0xF,         // Define a variable to the bank
        BANK_ASSIGN = 0x10,      // Assign a value to a variable in the bank
        LOCAL_ASSIGN = 0x11,     // Store a value to a local variable index

        MAKE_CLASS = 0x12,       // Takes a class specification from the lbank, and generates a WavyClass which gets loaded to the MBank
        MAKE_FUNC = 0x13,        // Takes a function specification from the lbank, and generates a WavyFunction which gets loaded to the MBank

        NEW = 0x14,              // Create a new WavyObject instance and push to the stack
        INVOKE_FUNC = 0x15,      // Invoke a function or method
        RETURN = 0x16,           // Return from a function (always returns a value)

        GOTO,             // Goto a given offset instruction
        IF_ZERO,          // If the val on the stack is zero, branch to the offset instruction
        IF_NZERO,         // If the val on the stack is not zero, branch to the offset instruction
        IF_GRT,           // If the val1 (top) on the stack is greater than val2, branch to the offset instruction
        IF_GRTE,          // If the val1 (top) on the stack is greater or equal than val2, branch to the offset instruction
        IF_LT,            // If the val1 (top) on the stack is less than val2, branch to the offset instruction
        IF_LTE,           // If the val1 (top) on the stack is less or equal than val2, branch to the offset instruction


        INCREMENT,          // Increments in place
        DECREMENT,          // Decrements in place

        PSH_ZERO,       // Push zero onto the stack
        PSH_NULL,       // Push null onto the stack
    }
}