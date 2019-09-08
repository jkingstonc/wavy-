#pragma once

#include "Core.h"
#include "Component.h"
#include "CoreManager.h"
#include "BankManager.h"
#include "Version.h"
#include "WCProfile.h"

typedef struct VMArgs
{
	const char* filename;
	VERSION version;
}VMArgs;

typedef struct VMState
{
}VMState;

class VM : public Component, public std::enable_shared_from_this<VM>
{
public:
	VM();
	~VM();
	void Start(); 
	void Run();
	void Close();
	void SpawnRoot();
	std::shared_ptr<CoreManager> GetCoreManager();
	std::shared_ptr<BankManager> GetBankManager();
	VMArgs args;
	std::shared_ptr<WCProfile> wc_profile;
private:
	std::shared_ptr<CoreManager> core_manager;
	std::shared_ptr<BankManager> bank_manager;
	VMState state;
};