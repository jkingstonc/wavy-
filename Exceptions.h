/*
James Clarke
06/09/2019
*/

#pragma once

#include <exception>
#include "Core.h"

class CoreRuntimeException : public std::exception
{
public:
	CoreRuntimeException(std::string msg, CoreState* state)
	{
		this->msg = msg;
		this->state = state;
	}
	std::string GetMsg()
	{
		return this->msg;
	}
	CoreState* GetState()
	{
		return this->state;
	}
private:
	std::string msg;
	CoreState* state;
};