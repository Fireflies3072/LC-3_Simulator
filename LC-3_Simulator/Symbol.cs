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

            // 读取文件 read file
            string text = File.ReadAllText(filename, Encoding.UTF8);
            ReadSymText(text);
        }

        public void ReadSymText(string text)
        {
            // 解析文件 parse file
            bool start = false;
            string[] lineList = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string line;
            for (int i = 0; i < lineList.Length; i++)
            {
                // 寻找开始的行 find starting line
                line = lineList[i];
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
        }

        public string GetSymbol(int address, bool returnAddress=true)
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
