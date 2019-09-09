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
	this->exec_stack = Stack<WItem*>();
	this->func_stack = Stack<FuncFrame>();
	this->locals = std::vector<WItem*>();
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
					std::cout << "Print Debug: " << ITEM_DEBUG(PEEK_EXEC()) << std::endl;
					break;
				}
				case POP:
					POP_EXEC(); break;
				case PEEK:
					PEEK_EXEC(); break;
				// On load operations, we create a copy of the pointer on the stack
				// rather than heap allocation to boost performance. When performing
				// define/assign operations, we heap allocate a new pointer & copy the
				// stack pointer to it.
				case LD_CONST:
				{
					WItem* c = REQ_C_ITEM(GET_ARG());
					WItem* cpy = &WItem();
					*cpy = *c;
					PUSH_EXEC(cpy);
					break;
				}
				case LD_VAR:
				{
					WItem* m = REQ_M_ITEM(GET_ARG());
					WItem* cpy = &WItem();
					*cpy = *m;
					PUSH_EXEC(cpy);
					break;
				}
				case LD_LOCAL:
				{
					WItem* l = GET_LOCAL(GET_ARG());
					WItem* cpy = &WItem();
					*cpy = *l;
					PUSH_EXEC(cpy); break;
				}
				case LD_ZERO:
					/*PUSH_EXEC(std::make_shared<WInt>(0)); break;*/
					PUSH_EXEC(&WInt(0));
					break;
				case DEFINE_VAR: 
				{
					// Create a copy of the stack pointer
					WItem* cpy = new WItem();
					*cpy = *POP_EXEC();
					DEFINE_MITEM(GET_ARG(), cpy);
					break; 
				}
				case ASSIGN_VAR: 
				{
					// Create a copy of the stack pointer
					WItem* cpy = new WItem();
					*cpy = *POP_EXEC();
					ASSIGN_MITEM(GET_ARG(), cpy);
					break;
				}
					//ASSIGN_MITEM(GET_ARG(), POP_EXEC());  break;
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