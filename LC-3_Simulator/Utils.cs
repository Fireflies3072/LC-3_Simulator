using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC_3_Simulator
{
    internal class Utils
    {
        public static int NormalizeUInt16(int value)
        {
            return (value + (1 - (value / 0x10000)) * 0x10000) % 0x10000;
        }

        public static int NormalizeInt16(int value)
        {
            return NormalizeUInt16(value + 0x8000) - 0x8000;
        }

        public static string Int16ToString(int value)
        {
            return Convert.ToString(value & 0xFFFF, 16).PadLeft(4, '0').ToUpper();
        }

        public static int SignExtend(int instruction, int lastBits = 5)
        {
            // 取instruction的最后几位 get the last bits of instruction
            int value = ~(~0 << lastBits) & instruction;
            // 负数补一 fill 1 if negative
            if ((instruction & (1 << (lastBits - 1))) != 0)
            {
                value |= ~0 << lastBits;
            }
            return value;
        }
    }
}
