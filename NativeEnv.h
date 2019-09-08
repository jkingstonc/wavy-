/*
James Clarke
07/09/2019
*/

#pragma once

#include <iostream>
#include "VM.h"
#include "WavyTypes.h"
#include "WCProfile.h"

/*
NativeEnv provides functionality to operate the VM from a native C/C++ enviroment
The VM should never be directly called from a native envrioment, everything should go through
the NativeEnv
*/
class NativeEnv : public Component
{
public:
	NativeEnv();
	~NativeEnv();
	void Start();
	void Run();
	void Close();
	std::shared_ptr<VM> vm;
};

void BindNativeVM(std::shared_ptr<VM> vm, std::shared_ptr<NativeEnv> nenv, std::shared_ptr<WCProfile> wc, VMArgs args);