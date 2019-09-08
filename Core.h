/*
James Clarke
06/09/2019
*/

#pragma once

#include "Utils.h"
#include "WavyTypes.h"
#include "Stack.h"
#include "Component.h"
#include "Bytecode.h"

#define MAX_PC 2^32
#define MAX_RECURSION 2^8
#define MAX_CORE_COUNT 2^8

#define PUSH_EXEC(item) this->exec_stack.Push(item)
#define POP_EXEC(item) this->exec_stack.Pop()
#define PEEK_EXEC(item) this->exec_stack.Peek()

#define END() (this->bytecode->at(this->pc)==END)

#define GOTO_NEXT() this->bytecode->at(this->pc++)
#define GET_ARG() GOTO_NEXT()

typedef struct CoreState
{
	uint8_t id;
	uint8_t func_depth;
}CoreState;

class VM;

class Core : public Component
{
public:
	Core(std::shared_ptr<VM> vm, uint8_t id);
	~Core();
	virtual void Start(std::shared_ptr<std::vector<int32_t>> bytecode);
	virtual void Run();
	virtual void Close();
	void Eval();
private:
	std::shared_ptr<VM> vm;
	uint32_t pc;
	std::shared_ptr<std::vector<wint>> bytecode;
	Stack<WITEM> exec_stack;
	Stack<FuncFrame> func_stack;
	std::vector<WITEM> locals;
	CoreState state;
};