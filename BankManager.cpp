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
	this->cbank = std::make_shared<Bank>();
	this->mbank = std::make_shared<Bank>();
}

void BankManager::Run()
{
	Component::Run();
}

void BankManager::Close()
{
	Component::Close();
}


void BankManager::BindCProfile(std::shared_ptr<std::vector<std::shared_ptr<WItem>>> items)
{
	LOG("Binding C Profile...");
	// Foreach item in the cbank in the wcprofile, copy it to the cbank
	for (uint32_t i = 0; i < items->size(); i++)
		this->cbank->items.insert(std::pair<uint32_t, std::shared_ptr<WItem>>(i, items->at(i)));
}

void BankManager::DefineItem(BANK_ID id, uint8_t bank_type, std::shared_ptr<WItem> item)
{
	auto bank = GET_BANK(bank_type);
	// Check if the cbank doesn't contain the item id
	if (!(bank->items.count(id)))
		bank->items[id] = item;
}

void BankManager::AssignItem(BANK_ID id, uint8_t bank_type, std::shared_ptr<WItem> item)
{
	auto bank = GET_BANK(bank_type);
	// Check if the cbank contains the item id
	if (bank->items.count(id))
		bank->items[id] = item;
}

std::shared_ptr<WItem> BankManager::RequestItem(BANK_ID id, uint8_t bank_type)
{
	auto bank = GET_BANK(bank_type);
	// Check if the cbank contains the item id
	if (bank->items.count(id))
		return bank->items.at(id);
	return std::make_shared<WInt>(0);
}