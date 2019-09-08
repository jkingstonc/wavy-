/*
James Clarke
06/09/2019
*/


#pragma once

#include <iostream>
#include <cstdint>
#include <vector>
#include <variant>

#define TO_WBOOL(b) (WITEM(b))
#define TO_WINT(i) (WITEM(i))
#define TO_WDOUBLE(d) (WITEM(d))
#define TO_WSTRING(s) (WITEM(s))

typedef bool wbool;
typedef int32_t wint;
typedef double_t wdouble;
typedef std::string wstring;

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

// should these be pointers?
#define WITEM std::variant<wbool, wint, wdouble, wstring, Wfunc, Wclass, Wobject>

#define GET_BOOL(item) return std::get<wbool>(item)
#define GET_INT(item) return std::get<wint>(item)
#define GET_DOUBLE(item) return std::get<wdouble>(item)
#define GET_STR(item) return std::get<wstring>(item)
#define GET_FUNC(item) return std::get<Wfunc>(item)
#define GET_CLASS(item) return std::get<Wclass>(item)
#define GET_OBJ(item) return std::get<Wobject>(item)

#define IS_BOOL(item) (item.index() == 0)
#define IS_INT(item) (item.index() == 1)
#define IS_DOUBLE(item) (item.index() == 2)
#define IS_STRING(item) (item.index() == 3)
#define IS_FUNC(item) (item.index() == 4)
#define IS_CLASS(item) (item.index() == 5)
#define IS_OBJECT(item) (item.index() == 6)
#define IS_NUMERIC(item) (IS_INT(item) || IS_DOUBLE(item))