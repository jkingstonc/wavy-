/*
James Clarke
06/09/2019
*/

#pragma once

#include "Utils.h"
#include "WavyTypes.h"

template <class T>
class Stack
{
public:
	Stack()
	{
		this->sp = 0;
		this->vec = std::vector<T>();
	}
	~Stack()
	{
		this->vec.clear();
	}
	void Push(T item)
	{
		this->vec.push_back(item);
	}
	T Pop()
	{
		WITEM item = this->vec.back();
		this->vec.pop_back();
		return item;
	}
	T Peek()
	{
		return this->vec.back();
	}
private:
	uint32_t sp;
	std::vector<T> vec;
};

// Used for func frames
class FuncFrame 
{
public:
	uint32_t pc;
	std::vector<WITEM> locals;
	Stack<WITEM> * exec_stack;
    std::vector<int32_t> bytecode;
private:
};