/*
James Clarke
06/09/2019
*/

#include <memory>
#include "NativeEnv.h"

int main()
{
	std::shared_ptr<NativeEnv> nenv = std::make_shared<NativeEnv>();
	std::shared_ptr<VM> vm = std::make_shared<VM>();
	VMArgs args = 
	{
		"test",
		(VERSION)0
	};

	std::shared_ptr<WCProfile> wc = std::make_shared<WCProfile>();
	wc->magic = std::string("WATERCLOSET");
	wc->bytecode = std::make_shared<std::vector<wint>>(
		std::vector<int32_t>
	{
		0x2, 0x3, 2, 0,
	});
	wc->c_profile = std::make_shared<std::vector<WITEM>>(
		std::vector<WITEM>
	{
		WINT(123), WINT(456), Wfunc(),
	});

	// Bind the vm to the native enviroment

	BindNativeVM(vm, nenv, wc, args);

	nenv->Start();
	nenv->Run();
	nenv->Close();
	
	std::cin.get();
}