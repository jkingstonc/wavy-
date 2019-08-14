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
The wavy core is a sub-system of the wavy vm. It contains a Func Stack, and importantly
a pointer to a wavy vm. This way, it can access the global Bank to retreive literals.

## Threads
Wavy should support multithreadded code. However, threads can only access data
from the bank

### AccessAssistance
Subsystem that handles access/modification requests to scopes in multi-threadded scenarios

## Registers
Each WavyCore contains:
- PC: Current bytecode being executed (starts at 0)
- SP: The current stack frame of execution

## Memory

### Scope
All values are stored within a scope object. This is to make it easier for the wavy runtime
to deal with memory. Scopes can store any object.
Values in a scope only are reachable from the current scope, and downwards.
E.g.

X is reachable as x is visible from it's own scope, downwards
var x = 2
func test
{
	x*=2
}

#### ValueTable
Each scope contains a ValueTable object, this contains the name and value of variables

### Bank
The bank is a global scope that is accessed globally by all cores and is located in the vm instance.
It contains 4 bank types

### MBank
Contains a scope of WavyClass definitions (stores class information), WavyFunction definitions (stores function information)
and var defintions

### LBank
Contains a scope of literal instances (stores literal information). As these literals aren't accociated with a var
label, they are given identifiers

#### BankManager
The bank manager is responsible for managing the Bank (Access Assistance & Garbage Collection)

### FuncStack
This stack stores stack frames for function calls

#### FuncFrame
The FuncStack contains instances of FuncFrames. Each func frame contains a scope, a FrameInfo object
and an ExecutionStack

### ExecutionStack
The execution stack is used to perform vm operations. Such as pushing params for a call

##### FrameInfo
Contains information about the current FuncFrame (function name, should return, is method)

## Imports
The vm can support dynamic importing of files





# Bytecode

## Compiled files
Wavy compiles w~ files to wc~ files, these contain all the information the vm requires

## wc~
wc~ contain the following: A LinkingProfile, a BankProfile, and a bytecode sequence
The file must contain the phrase in hex at the start "WATERCLOSET" -> 5741544552434c4f534554
(this is a reference to the wc~ extension)
wc~ files are stored in a folder called wavy_cache. The wavy runtime first checks if this exists
if so it sends the contents to the vm, if not it will recompile

### LinkingProfile
This contains information about external data that the w~ requires

### BankProfile
Contains a table of bank information for the vm to generate a Bank instance of

### Bytecode Sequence
Binary sequence of bytecode


## Bytecode Spec
(required on the stack)
{result on stack}

// Unary operations always pop the top, and push back the result
- UN_NOT		 -> (a)			    {~a} Invert the truth of a value
- UN_POS		 -> (a)			    {+a} Convert numeric value to positive

// Binary operations always pop the top 2, and push back the result
- BIN_ADD	     -> (a, b)			{a+b}
- BIN_SUB	     -> (a, b)			{a-b}
- BIN_MUL	     -> (a, b)			{a*b}
- BIN_DIV	     -> (a, b)			{a/b}
- BIN_DIV	     -> (a, b)			{a/b}
- BIN_MOD	     -> (a, b)			{a mod b}

- INVOKE_FUNC id -> (func, params)  {} Invoke a function call
- OBJ_GET 	  

- RETURN
- CONTINUE
- BREAK

- LOAD_GLOB   id -> 			    {} Load a global onto the stack from the bank
- LOAD_LIT    id -> 			    {} LOAD_GLOB but with literals
- LOAD_LOC    id ->					{} LOAD_GLOB but from the local scope


# Operations order

## Interpreting
If no wavy_cache wc~ file exists...

## Lexing
We generate a token stream and send it to the parser

## Parsing

## Scope Resolving & Linking checks

### Scope Resolving
This resolves the ids of identifiers from the BankProfile

### Linking Checks
If an identifier doesn't resolve, it will be added to the LinkingProfile.
This is not checked at compile time, and thus could be invalid

## Bytecode Emission

## Interpreting continued...
If a wavy_cache wc~ file does exist...

## WavyVM instantiation
A new WavyVM is instantiated and a thread is started
The code is then ran















