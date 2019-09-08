#include "NativeEnv.h"

NativeEnv::NativeEnv()
{
	Component::component_id = "NativeInterface";
}

NativeEnv::~NativeEnv()
{

}

void NativeEnv::Start()
{
	Component::Start();
	this->vm->Start();
}

void NativeEnv::Run()
{
	Component::Run();
	this->vm->Run();
}

void NativeEnv::Close()
{
	Component::Close();
	this->vm->Close();
}

void BindNativeVM(std::shared_ptr<VM> vm, std::shared_ptr<NativeEnv> nenv, std::shared_ptr<WCProfile> wc, VMArgs args)
{
	nenv->vm = vm;
	vm->wc_profile = wc;
	vm->args = args;
}