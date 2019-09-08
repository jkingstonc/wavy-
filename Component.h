/*
James Clarke
06/09/2019
*/


#pragma once

#include "Logger.h"
#include <string>

class Component
{
public:
	virtual void Start()
	{
		LOG(this->component_id + ": Starting!");
	}
	virtual void Run()
	{
		LOG(this->component_id + ": Running!");
	}
	virtual void Close()
	{
		LOG(this->component_id + ": Closing!");
	}
protected:
	std::string component_id;
};