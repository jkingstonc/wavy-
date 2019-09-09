/*
James Clarke
07/09/2019
*/

#include "BankManager.h"

BankManager::BankManager()
{
	this->component_id = "BankManager";
}

BankManager::~BankManager()
{
}

void BankManager::Start()
{
	Component::Start();
	this->cbank = std::make_shared<Bank>(0);
	this->mbank = std::make_shared<Bank>(1);
}

void BankManager::Run()
{
	Component::Run();
}

void BankManager::Close()
{
	Component::Close();
}


void BankManager::BindCProfile(std::shared_ptr<std::vector<WItem*>> items)
{
	LOG("Binding C Profile...");
	// Foreach item in the cbank in the wcprofile, copy it to the cbank
	for (uint32_t i = 0; i < items->size(); i++)
		this->cbank->items.insert(std::pair<uint32_t, WItem*>(i, items->at(i)));
}

void BankManager::FreeBanks()
{
	this->cbank->Free();
	this->mbank->Free();
}

void BankManager::DefineItem(BANK_ID id, uint8_t bank_type, WItem* item)
{
	auto bank = GET_BANK(bank_type);
	// Check if the cbank doesn't contain the item id
	if (!(bank->items.count(id)))
		bank->items[id] = item;
}

void BankManager::AssignItem(BANK_ID id, uint8_t bank_type, WItem* item)
{
	auto bank = GET_BANK(bank_type);
	// Check if the cbank contains the item id
	if (bank->items.count(id))
		bank->items[id] = item;
}

WItem* BankManager::RequestItem(BANK_ID id, uint8_t bank_type)
{
	auto bank = GET_BANK(bank_type);
	// Check if the cbank contains the item id
	auto iter = bank->items.find(id);
	if (iter != bank->items.end())
	{
		return iter->second;
	}
	return nullptr;
}


void Bank::Free()
{
	/*// Foreach item in the cbank in the wcprofile, delete it
	for (uint32_t i = 0; i < this->items.size(); i++)
		delete this->items.at(i);*/

	for (auto it = this->items.begin(); it != this->items.end(); /* don't increment here*/) {
		delete it->second;
		it = this->items.erase(it);  // update here
	}
}