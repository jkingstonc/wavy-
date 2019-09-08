/*
James Clarke
06/09/2019
*/

#pragma once

enum Opcode
{
	END = 0x0,
	NOP = 0x1,
	PRINT = 0x2,
	LD_CONST = 0x3,
};

void PrintOP();