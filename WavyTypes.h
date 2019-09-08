/*
James Clarke
06/09/2019
*/


#pragma once

#include <iostream>
#include <cstdint>
#include <vector>
#include <variant>

#define WBOOL(b) (WITEM(b))
#define WINT(i) (WITEM(i))
#define WDOUBLE(d) (WITEM(d))
#define WSTRING(s) (WITEM(s))

typedef bool wbool;
typedef int32_t wint;
typedef double_t wdouble;
typedef std::string wstring;

// should these be pointers?
#define WITEM std::variant<wbool, wint, wdouble, wstring, Wfunc, Wclass, Wobject>

class Wfunc
{
public:
	const char* name;
	uint8_t arg_count;
	uint8_t local_count;
	std::byte flags;

private:

};

class Wclass
{
public:

private:

};

class Wobject
{
public:

private:

};