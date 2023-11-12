using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LC_3_Simulator
{
    class Symbol
    {
        private Dictionary<string, string> symbolList;
        private List<string> instructionAddressList;

        public Symbol()
        {
            symbolList = new Dictionary<string, string>();
            instructionAddressList = new List<string>();
        }

        public void ReadSymFile(string filename)
        {
            // 检查文件是否存在 check if file exists
            if (!File.Exists(filename))
            {
                return;
            }

            // 打开文件 open file
            StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read), Encoding.UTF8);

            // 解析文件 parse file
            string line;
            bool start = false;
            while ((line = sr.ReadLine()) != null)
            {
                // 寻找开始的行 find starting line
                if (line.Contains("----"))
                {
                    start = true;
                    continue;
                }
                // 开始解析 start parsing
                if (start)
                {
                    line = line.Trim();
                    if (line.StartsWith("//"))
                    {
                        line = line.Substring(2).Trim();
                        string symbol = line.Substring(0, line.IndexOf(' '));
                        string address = line.Substring(symbol.Length).Trim();
                        // 检查是符号还是指令 check if symbol or instruction
                        if (symbol == "$")
                        {
                            instructionAddressList.Add(address);
                        }
                        else
                        {
                            symbolList.Add(address, symbol);
                        }
                    }
                }
            }

            // 关闭文件 close file
            sr.Close();
        }

        public string GetSymbol(int address, bool returnAddress=false)
        {
            string strAddress = Utils.Int16ToString(address);
            if (symbolList.ContainsKey(strAddress))
            {
                return symbolList[strAddress];
            }
            return returnAddress ? "x"+strAddress : "";
        }

        public bool IsInstruction(int address)
        {
            string strAddress = Utils.Int16ToString(address);
            return instructionAddressList.Contains(strAddress);
        }
    }
}
