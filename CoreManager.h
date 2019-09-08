/*
James Clarke
06/09/2019
*/

#pragma once

#include <map>
#include "Core.h"

class VM;

class CoreManager : public Component
{
public:
	CoreManager();
	~CoreManager();
	std::shared_ptr<Core> CreateCore();
	void CloseAll();
	void Close();
	std::shared_ptr<VM> vm;
private:
	std::map<uint8_t, std::shared_ptr<Core>> core_pool;
	uint8_t next_id;
};