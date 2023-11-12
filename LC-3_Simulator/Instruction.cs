using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace LC_3_Simulator
{
    internal class Instruction
    {
        private bool isValid;
        private int opCode;
        private int DR;
        private int SR1;
        private int SR2;
        private int immediateValue;
        private int offset;
        private string description;
        // BaseR使用SR2 BaseR uses SR2
        // nzp使用SR2 nzp uses SR2

        public Instruction(int instruction, Symbol symbol, int addedPC)
        {
            // 初始化变量 initialize variables
            isValid = false;
            opCode = -1;
            DR = -1;
            SR1 = -1;
            SR2 = -1;
            immediateValue = 0;
            offset = 0;
            description = "";

            // 获得执行码 get op code
            int address = 0;
            opCode = (instruction & 0xFFFF) >> 12;
            switch (opCode)
            {
                // ADD
                case 0b0001:
                // AND
                case 0b0101:
                    DR = instruction >> 9 & 0x7;
                    SR1 = instruction >> 6 & 0x7;
                    if ((instruction & 0x20) == 0)
                    {
                        SR2 = instruction & 0x7;
                    }
                    else
                    {
                        immediateValue = Utils.SignExtend(instruction);
                    }
                    if (opCode == 0b0001)
                    {
                        description = "ADD R" + DR.ToString() + ", R" + SR1.ToString() + ", " + (SR2==-1 ? immediateValue.ToString() : "R"+SR2.ToString());
                    }
                    else if (opCode == 0b0101)
                    {
                        description = "AND R" + DR.ToString() + ", R" + SR1.ToString() + ", " + (SR2==-1 ? immediateValue.ToString() : "R"+SR2.ToString());
                    }
                    break;

                // NOT
                case 0b1001:
                    if ((instruction & 0x3F) != 0x3F)
                    {
                        isValid = false;
                        return;
                    }
                    DR = instruction >> 9 & 0x7;
                    SR1 = instruction >> 6 & 0x7;
                    description = "NOT R" + DR.ToString() + ", R" + SR1.ToString();
                    break;

                // LD
                case 0b0010:
                //LDI
                case 0b1010:
                // LEA
                case 0b1110:
                    DR = instruction >> 9 & 0x7;
                    offset = Utils.SignExtend(instruction, 9);
                    address = addedPC + offset;
                    if (opCode == 0b0010)
                    {
                        description = "LD R" + DR + ", " + symbol.GetSymbol(address, true);
                    }
                    else if (opCode == 0b1010)
                    {
                        description = "LDI R" + DR + ", " + symbol.GetSymbol(address, true);
                    }
                    else if (opCode == 0b1110)
                    {
                        description = "LEA R" + DR + ", " + symbol.GetSymbol(address, true);
                    }
                    break;

                // LDR
                case 0b0110:
                    DR = instruction >> 9 & 0x7;
                    SR2 = instruction >> 6 & 0x7;
                    offset = Utils.SignExtend(instruction, 6);
                    description = "LDR R" + DR + ", R" + SR2 + ", #" + offset;
                    break;

                // ST
                case 0b0011:
                // STI
                case 0b1011:
                    SR1 = instruction >> 9 & 0x7;
                    offset = Utils.SignExtend(instruction, 9);
                    address = addedPC + offset;
                    if (opCode == 0b0011)
                    {
                        description = "ST R" + SR1 + ", " + symbol.GetSymbol(address, true);
                    }
                    else if (opCode == 0b1011)
                    {
                        description = "STI R" + SR1 + ", " + symbol.GetSymbol(address, true);
                    }
                    break;

                // STR
                case 0b0111:
                    SR1 = instruction >> 9 & 0x7;
                    SR2 = instruction >> 6 & 0x7;
                    offset = Utils.SignExtend(instruction, 6);
                    description = "STR R" + SR1 + ", R" + SR2 + ", #" + offset;
                    break;

                // BRx
                case 0b0000:
                    SR2 = instruction >> 9 & 0x7;
                    if (SR2 == 0)
                    {
                        isValid = false;
                        return;
                    }
                    offset = Utils.SignExtend(instruction, 9);
                    address = addedPC + offset;
                    description = "BR" + DecodeCondition(SR2) + " " + symbol.GetSymbol(address, true);
                    break;

                // 1101未使用的操作码 unused opCode
                default:
                    isValid = false;
                    return;
            }
            isValid = true;
        }

        public bool Execute(Memory memory, Register register)
        {
            if (!isValid)
            {
                return false;
            }

            // 执行指令 execute instruction
            int result, address;
            switch (opCode)
            {
                // ADD
                case 0b0001:
                    if (SR2 == -1)
                    {
                        result = register.ReadRegister(SR1) + immediateValue;
                    }
                    else
                    {
                        result = register.ReadRegister(SR1) + register.ReadRegister(SR2);
                    }
                    register.WriteRegister(DR, result, true);
                    break;

                // AND
                case 0b0101:
                    if (SR2 == -1)
                    {
                        result = register.ReadRegister(SR1) & immediateValue;
                    }
                    else
                    {
                        result = register.ReadRegister(SR1) & register.ReadRegister(SR2);
                    }
                    register.WriteRegister(DR, result, true);
                    break;

                // NOT
                case 0b1001:
                    result = ~register.ReadRegister(SR1);
                    register.WriteRegister(DR, result, true);
                    break;

                // LD
                case 0b0010:
                    address = register.ReadPC() + offset;
                    result = memory.ReadMemory(address);
                    register.WriteRegister(DR, result, true);
                    break;

                // LDI
                case 0b1010:
                    address = memory.ReadMemory(register.ReadPC() + offset);
                    result = memory.ReadMemory(address);
                    register.WriteRegister(DR, result, true);
                    break;

                // LDR
                case 0b0110:
                    address = register.ReadRegister(SR2) + offset;
                    result = memory.ReadMemory(address);
                    register.WriteRegister(DR, result, true);
                    break;

                // LEA
                case 0b1110:
                    result = register.ReadPC() + offset;
                    register.WriteRegister(DR, result, true);
                    break;

                // ST
                case 0b0011:
                    address = register.ReadPC() + offset;
                    result = register.ReadRegister(SR1);
                    memory.WriteMemory(address, result);
                    break;

                // STI
                case 0b1011:
                    address = memory.ReadMemory(register.ReadPC() + offset);
                    result = register.ReadRegister(SR1);
                    memory.WriteMemory(address, result);
                    break;

                // STR
                case 0b0111:
                    address = register.ReadRegister(SR2) + offset;
                    result = register.ReadRegister(SR1);
                    memory.WriteMemory(address, result);
                    break;

                // BRx
                case 0b0000:
                    address = register.ReadPC() + offset;
                    register.WritePC(address);
                    break;

                // Not valid instruction
                default:
                    return false;
            }
            return true;
        }

        public string GetDescription()
        {
            return description;
        }

        private string DecodeCondition(int condition)
        {
            switch (condition)
            {
                case 0b100:
                    return "n";
                case 0b010:
                    return "z";
                case 0b001:
                    return "p";
                case 0b110:
                    return "nz";
                case 0b011:
                    return "zp";
                case 0b101:
                    return "np";
                default:
                    return "";
            }
        }
    }
}
