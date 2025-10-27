using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LC_3_Simulator
{
    internal class Register
    {
        private int[] registerList;
        public RegisterGridLine[] registerGrid;

        public Register()
        {
            // 初始化寄存器 initialize register
            registerList = new int[10];

            // 初始化寄存器表 initialize register grid
            registerGrid = new RegisterGridLine[registerList.Length];
            for (int i = 0; i < 8; i++)
            {
                registerGrid[i] = new RegisterGridLine()
                {
                    Name = $"R{i}",
                    Value = "x0000"
                };
            }
            registerGrid[8] = new RegisterGridLine() { Name = "PC", Value = "x0000" };
            registerGrid[9] = new RegisterGridLine() { Name = "CC", Value = "Zero" };
        }

        public int ReadRegister(int index)
        {
            if (index >= 0 && index <= 7)
            {
                return registerList[index];
            }
            else
            {
                throw new IndexOutOfRangeException("Try to read from a nonexistent register.");
            }
        }

        public void WriteRegister(int index, int data, bool updateCC=false)
        {
            data = Utils.NormalizeInt16(data);
            if (index >= 0 && index <= 7)
            {
                registerList[index] = data;
                registerGrid[index].Value = "x" + Utils.Int16ToString(data);
                if (updateCC)
                {
                    WriteCC(data);
                }
            }
            else
            {
                throw new IndexOutOfRangeException("Try to write to a nonexistent register.");
            }
        }

        public int ReadPC()
        {
            return registerList[8];
        }

        public void WritePC(int data)
        {
            registerList[8] = data & 0xFFFF;
            registerGrid[8].Value = "x" + Utils.Int16ToString(ReadPC());
        }

        public void AddPC()
        {
            WritePC(ReadPC() + 1);
        }

        public int ReadCC()
        {
            return registerList[9];
        }

        public void WriteCC(int value)
        {
            value = Utils.SignExtend(value, 16);
            if (value > 0)
            {
                registerList[9] = 1;
                registerGrid[9].Value = "Positive";
            }
            else if (value == 0)
            {
                registerList[9] = 0;
                registerGrid[9].Value = "Zero";
            }
            else
            {
                registerList[9] = -1;
                registerGrid[9].Value = "Negative";
            }
        }
    }
}
