/*
James Clarke
07/09/2019
*/

#pragma once

#include "Component.h"
#include "WavyTypes.h"
#include <variant>
#include <map>

#define MAX_BANK_SIZE 2^32
#define C_BANK 0
#define M_BANK 1
#define GET_BANK(bank_type) ((bank_type == 0) ? this->cbank : this->mbank)

typedef uint32_t BANK_ID;

class Bank;

class BankManager : public Component
{
public:
	BankManager();
	~BankManager();
	void Start();
	void Run();
	void Close();
	void BindCProfile(std::shared_ptr<std::vector<WItem*>> items);
	void DefineItem(BANK_ID id, uint8_t bank_type, WItem* item);
	void AssignItem(BANK_ID id, uint8_t bank_type, WItem* item);
	WItem* RequestItem(BANK_ID id, uint8_t bank_type);
private:
	std::shared_ptr<Bank> cbank;
	std::shared_ptr<Bank> mbank;
};

class Bank
{
public:
	Bank(uint8_t bank_type)
	{
		this->bank_type = bank_type;
	}
	uint8_t bank_type;
	std::map<BANK_ID, WItem*> items;
};