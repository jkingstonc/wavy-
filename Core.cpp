/*
James Clarke
06/09/2019
*/


#include "Core.h"
#include "VM.h"
#include "Exceptions.h"

Core::Core(std::shared_ptr<VM> vm, uint8_t id)
{
	this->component_id = std::string("Core " + id);
	this->vm = vm;
	this->state.id = id;
	this->pc = 0;
	this->exec_stack = Stack<std::shared_ptr<WItem>>();
	this->func_stack = Stack<FuncFrame>();
	this->locals = std::vector<std::shared_ptr<WItem>>();
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

			#ifdef INSTRUCTION_DEBUG
				std::cout << "Next: ";
				std::cout << next << std::endl;
			#endif
			
			switch (next)
			{
				case END:
					break;
				case NOP:
					break;
				case PRINT:
				{
					std::cout << ITEM_DEBUG(PEEK_EXEC()) << std::endl;
					break;
				}
				case POP:
					POP_EXEC(); break;
				case PEEK:
					PEEK_EXEC(); break;
				case LD_CONST:
				{
					// Load the shared_ptr from the cbank, & create a copy for the stack
					// This is becuase the cbank is immuatble
					std::shared_ptr<WItem> item = REQ_C_ITEM(GET_ARG());
					PUSH_EXEC(std::make_shared<WItem>(*item)); break;
				}
				case LD_VAR:
				{
					// We don't create a copy, as the mbank is mutable
					std::shared_ptr<WItem> item = REQ_M_ITEM(GET_ARG());
					PUSH_EXEC(item); break;
				}
				case LD_LOCAL:
				{
					PUSH_EXEC(GET_LOCAL(GET_ARG())); break;
				}
				case LD_ZERO:
					PUSH_EXEC(std::make_shared<WInt>(0)); break;
				case DEFINE_VAR:
					DEFINE_MITEM(GET_ARG(), POP_EXEC());  break;
				case ASSIGN_VAR:
					ASSIGN_MITEM(GET_ARG(), POP_EXEC());  break;
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