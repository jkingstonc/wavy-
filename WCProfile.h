#pragma once

#include "WavyTypes.h"
#include <vector>

typedef struct WCProfile{
	std::string magic;
	std::shared_ptr<std::vector<int32_t>> bytecode;
	std::shared_ptr<std::vector<WITEM>> c_profile;
}WCProfile;