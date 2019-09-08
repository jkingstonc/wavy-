/*
James Clarke
06/09/2019
*/

#pragma once

#include <iostream>
#include <cstdint>
#include <vector>
#include <variant>

#define ITEM_DEBUG(item) (item_type_lookup[item->info.type])

static std::string item_type_lookup[] =
{
	"WBOOL", "WINT", "WDOUBLE", "WSTRING", "WFUNC", "WCLASS", "WOBJECT"
};

enum ItemType
{
	WBOOL, WINT, WDOUBLE, WSTRING, WFUNC, WCLASS, WOBJ,
};

typedef struct WItemInfo{
	ItemType type;
}WitemInfo;

class WItem
{
public:
	WItemInfo info;
};


class WBool : public WItem
{
public:
	WBool(bool data)
	{
		this->data = data;
		this->info.type = WBOOL;
	}
	bool data;
};


class WInt : public WItem
{
public:
	WInt(int32_t data)
	{
		this->data = data;
		this->info.type = WINT;
	}
	int32_t data;
};


class WDouble : public WItem
{
public:
	WDouble(double data)
	{
		this->data = data;
		this->info.type = WDOUBLE;
	}
	double data;
};


class WString : public WItem
{
public:
	WString(std::string data)
	{
		this->data = data;
		this->info.type = WSTRING;
	}
	std::string data;
};


class WFunc : public WItem
{
public:
	WFunc()
	{
		this->info.type = WFUNC;
	}
	const char* name;
	uint8_t arg_count;
	uint8_t local_count;
	std::byte flags;

private:

};

class WClass : public WItem
{
public:
	WClass()
	{
		this->info.type = WCLASS;
	}
private:

};

class WObject : public WItem
{
public:
	WObject()
	{
		this->info.type = WOBJ;
	}
private:

};