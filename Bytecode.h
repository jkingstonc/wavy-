/*
James Clarke
06/09/2019
*/

#pragma once

enum Opcode
{
	END,
	NOP,
	PRINT,
	POP,
	PEEK,
	LD_CONST,
	LD_VAR,
	LD_LOCAL,
	LD_ZERO,
	DEFINE_VAR,
	ASSIGN_VAR,
};