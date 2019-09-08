/*
James Clarke
06/09/2019
*/

#include "CoreManager.h"
#include "VM.h"
#include "NativeEnv.h"

CoreManager::CoreManager()
{
	this->component_id = std::string("CoreManager");
	this->core_pool = std::map<uint8_t, std::shared_ptr<Core>>();
	this->next_id = 0;
}

CoreManager::~CoreManager()
{

}

std::shared_ptr<Core> CoreManager::CreateCore()
{
	std::shared_ptr<Core> new_core = std::make_shared<Core>(this->vm, this->next_id);
	this->core_pool.insert({ this->next_id, new_core });
	this->next_id++;
	return new_core;
}

void CoreManager::CloseAll()
{
	this->core_pool.clear();
}

void CoreManager::Close()
{
	Component::Close();
	CloseAll();
}