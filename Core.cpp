/*
James Clarke
06/09/2019
*/


#include "Core.h"
#include "VM.h"
#include "Exceptions.h"

Core::Core(std::shared_ptr<VM> vm, uint8_t id)
{
	this->vm = vm;
	this->component_id = std::string("Core "+id);
	this->state.id = id;
	this->pc = 0;
	this->exec_stack = Stack<WITEM>();
	this->func_stack = Stack<FuncFrame>();
	this->locals = std::vector<WITEM>();
}

Core::~Core()
{
	
}

void Core::Start(std::shared_ptr<std::vector<int32_t>> bytecode)
{
	Component::Start();
	this->bytecode = bytecode;
}

void Core::Run()
{
	Component::Run();
	this->Eval();
	this->Close();
}

void Core::Close()
{
	Component::Close();
	vm->Close();
}

void Core::Eval()
{
	try {
		while (!END())
		{
			int32_t next = GOTO_NEXT();
			switch (next)
			{
			case END:
				break;
			case NOP:
				break;
			case PRINT:
				PrintOP(); break;
			case LD_CONST:
			{
				WITEM item = this->vm->GetBankManager()->RequestItem(GET_ARG(), C_BANK);
				PUSH_EXEC(item);
			}
			default:
				break;
			}
		}
	}
	catch (CoreRuntimeException* e)
	{
		ERR(e->GetMsg());
		delete e;
	}
}