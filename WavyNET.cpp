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
	wc->bytecode = std::make_shared<std::vector<int32_t>>(
		std::vector<int32_t>
	{
		LD_CONST, 1, 
		DEFINE_VAR, 0, 
		LD_VAR, 0,
		PRINT,
		END,
	});	
	wc->c_profile = std::make_shared<std::vector<WItem*>>(
		std::vector<WItem*>
		{
			&WInt(123),&WInt(456),
		});

	// Bind the vm to the native enviroment

	BindNativeVM(vm, nenv, wc, args);

	nenv->Start();
	nenv->Run();
	nenv->Close();
	
	std::cin.get();
}