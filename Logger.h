/*
James Clarke
06/09/2019
*/

/*
THIS IS TEMPORARY
WE SHOULD USE A LOGGING LIBRARY
SUCH AS github.com/gabime/spdlog
*/

#pragma once

#include <iostream>
#include <windows.h>   // WinApi header

#define DEFAULT_COLOUR 15

#define LOG(msg) std::cout << "LOG: " << msg << std::endl;

#define WARN(msg) std::cout << "WARN: " << msg << std::endl;

#define ERR(msg) std::cout << "ERR: " << msg << std::endl;