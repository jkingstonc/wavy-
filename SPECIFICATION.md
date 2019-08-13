# W~ Specification

## Data Structures

### PRIMITIVES
- boolean: true/false
- int: 32 (default) or 16 bit
- float: 64 (default) or 32 bit
- strings: Not C like, they exist as a literal

### Variables
- variables are declared with 'var'
- Only exist within their scope, globals exist in the global scope

### Classes
- Can inherit
- Contain 'at-methods' that are accessed by internal vm functionality
- Body only contains methods, members must be declared within methods

## Scope
- Scopes declared with {}
- Scopes can access data in above scope

## Conditionals
- If, Elseif, Else

## Loops
- While, For




# Wavy VM Specification

## vm instances
When a wavy runtime is instantated, a wavy core is created

## WavyCore
The wavy core is a sub-system of the wavy vm. It contains an Execution & Func Stack, and importantly
a pointer to a wavy vm. This way, it can access the global Bank to retreive literals.

## Threads

### AccessAssistance
Subsystem that handles access/modification requests to scopes

## Registers
Each WavyCore contains:
- PC: Current bytecode being executed (starts at 0)
- SP: The current stack frame of execution

## Memory

### Scope
All values are stored within a scope object. This is to make it easier for the wavy runtime
to deal with memory. Scopes can store any object

#### ValueTable
Each scope contains a ValueTable object, this contains the name and value of variables

### Bank
The bank is a global scope that is accessed globally by all cores and is located in the vm instance.
It contains 3 bank types

### CBank
Contains a scope of WavyClass instances (stores class information)

### FBank
Contains a scope of WavyFunction instances (stores function information)

### LBank
Contains a scope of literal instances (stores literal information). As these literals aren't accociated with a var
label, they are given identifiers

#### BankManager
The bank manager is responsible for managing the Bank (Access Assistance & Garbage Collection)

### ExecutionStack
The execution stack is used to perform vm operations. Such as pushing params for a call

### FuncStack
This stack stores stack frames for function calls

#### FuncFrame
The FuncStack contains instances of FuncFrames. Each func frame contains a scope, and a FrameInfo object

##### FrameInfo
Contains information about the current FuncFrame (function name, should return, is method)

## Imports
The vm can support dynamic importing of files