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
#define C_BANK 1
#define M_BANK 0
#define GET_BANK(bank_type) ((bank_type > 0) ? this->cbank : this->mbank)

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
	void BindCProfile(std::shared_ptr<std::vector<std::shared_ptr<WItem>>> items);
	void DefineItem(BANK_ID id, uint8_t bank_type, std::shared_ptr<WItem> item);
	void AssignItem(BANK_ID id, uint8_t bank_type, std::shared_ptr<WItem> item);
	std::shared_ptr<WItem> RequestItem(BANK_ID id, uint8_t bank_type);
private:
	std::shared_ptr<Bank> cbank;
	std::shared_ptr<Bank> mbank;
};

class Bank
{
public:
	int bank_type : 1;
	std::map<BANK_ID, std::shared_ptr<WItem>> items;
};