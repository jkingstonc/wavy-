#include "VM.h"

VM::VM()
{
	this->component_id = std::string("VM");
	this->core_manager = std::make_shared<CoreManager>();
	this->bank_manager = std::make_shared<BankManager>();
}

VM::~VM()
{

}

void VM::Start()
{
	Component::Start();
	this->core_manager->vm = shared_from_this();
	this->core_manager->Start();
	this->bank_manager->Start();
	this->bank_manager->BindCProfile(this->wc_profile->c_profile);
}

void VM::Run()
{
	Component::Run();
	this->core_manager->Run();
	this->bank_manager->Run();
	this->SpawnRoot();
}

void VM::Close()
{
	this->core_manager->Close();
	this->bank_manager->Close();
}

void VM::SpawnRoot()
{
	// Create the root core
	std::shared_ptr<Core> root_core = this->core_manager->CreateCore();
	root_core->Start(this->wc_profile->bytecode);
	root_core->Run();
}

std::shared_ptr<CoreManager> VM::GetCoreManager()
{
	return this->core_manager;
}

std::shared_ptr<BankManager> VM::GetBankManager()
{
	return this->bank_manager;
}