using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LC_3_Simulator
{
    internal class Memory
    {
        private int[] memory;
        public MemoryGridLine[] memoryGrid;

        public Memory()
        {
            // 初始化内存 initialize memory
            memory = new int[65536];

            // 初始化内存表 initialize memory grid
            memoryGrid = new MemoryGridLine[memory.Length];
            for (int i = 0; i < memory.Length; i++)
            {
                memoryGrid[i] = new MemoryGridLine()
                {
                    BP = false,
                    Address = "x" + Convert.ToString(i, 16).PadLeft(4, '0').ToUpper(),
                    Value = "x0000",
                    Description = ".FILL x0000"
                };
            }
        }

        public int ReadMemory(int address)
        {
            return memory[address & 0xFFFF];
        }

        public void WriteMemory(int address, int data, bool isInstruction=false, Symbol? symbol=null)
        {
            if (symbol == null)
            {
                symbol = new Symbol();
            }
            // 标准化数据 normalize data
            data = Utils.NormalizeInt16(data);
            address = address & 0xFFFF;
            // 更新内存和表 update memory and grid
            memory[address] = data;
            memoryGrid[address].Address = "x" + Utils.Int16ToString(address) + " " + symbol.GetSymbol(address);
            memoryGrid[address].Value = "x" + Utils.Int16ToString(data);
            // 更新描述 update description
            if (isInstruction)
            {
                Instruction instruction = new Instruction(data, symbol, address + 1);
                memoryGrid[address].Description = instruction.GetDescription();
            }
            else
            {
                memoryGrid[address].Description = ".FILL x" + Utils.Int16ToString(data);
            }
        }

        public void ReadObjFile(string filename, Register register, Symbol symbol)
        {
            // 打开文件 open file
            BinaryReader br = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));

            // 读取起始地址 read origin address
            byte[] data = br.ReadBytes(2);
            byte[] temp = new byte[4] { data[1], data[0], 0, 0 };
            int value = BitConverter.ToInt32(temp);
            register.WritePC(value);
            int address = value;

            // 读取指令 read instruction
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                // 读取2个字节并逆序 read 2 bytes and reverse
                data = br.ReadBytes(2);
                temp = new byte[4] { data[1], data[0], 0, 0 };
                value = BitConverter.ToInt32(temp);
                // 写入内存 write to memory
                WriteMemory(address, value, symbol.IsInstruction(address), symbol);
                address++;
            }

            // 关闭文件
            br.Close();
        }
    }
}
