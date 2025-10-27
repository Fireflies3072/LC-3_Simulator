using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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

        public void WriteMemory(int address, int data, Symbol? symbol=null, WriteableBitmap? image=null)
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
            memoryGrid[address].Address = "x" + Utils.Int16ToString(address) + " " + symbol.GetSymbol(address, false);
            memoryGrid[address].Value = "x" + Utils.Int16ToString(data);
            // 更新描述 update description
            Instruction instruction = new Instruction(data, symbol, address + 1);
            memoryGrid[address].Description = instruction.GetDescription(symbol.IsInstruction(address));
            // 更新图像 update image
            if (image != null && address >=0xC000 && address <= 0xFDFF)
            {
                General.ioImageList[General.tabIndex].Dispatcher.Invoke(new Action(() =>
                {
                    DrawImage(image);
                }));
                
            }            
        }

        public void ReadObjFile(string filename, Register register, Symbol symbol)
        {
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                ReadObjFile(stream, register, symbol);
            }
        }

        public void ReadObjFile(Stream stream, Register register, Symbol symbol)
        {
            // 打开文件 open file
            BinaryReader br = new BinaryReader(stream);

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
                WriteMemory(address, value, symbol);
                address++;
            }

            // 关闭文件
            br.Close();
        }

        public void DrawImage(WriteableBitmap image)
        {
            unsafe
            {
                image.Lock();
                byte[] data = new byte[128 * 124 * 3];

                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        int address = 0xC000 + y * 0x80 + x;
                        int color = memory[address];
                        int red = (int)((double)((color >> 10) & 0x001F) / 31.0 * 255.0);
                        int green = (int)((double)((color >> 5) & 0x001F) / 31.0 * 255.0);
                        int blue = (int)((double)(color & 0x001F) / 31.0 * 255.0);
                        data[(y * (int)image.Width + x) * 3] = (byte)blue;
                        data[(y * (int)image.Width + x) * 3 + 1] = (byte)green;
                        data[(y * (int)image.Width + x) * 3 + 2] = (byte)red;
                    }
                }

                Marshal.Copy(data, 0, General.controlList[General.tabIndex].image.BackBuffer, data.Length);
                image.AddDirtyRect(new System.Windows.Int32Rect(0, 0, 128, 124));
                image.Unlock();
            }
        }
        
        //public void DrawImage(WriteableBitmap image)
        //{
        //    image.Lock();
        //    int width = (int)image.Width;
        //    int height = (int)image.Height;
        //    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height, image.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format24bppRgb, image.BackBuffer);

        //    for (int x = 0; x < width; x++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            int address = 0xC000 + y * 0x80 + x;
        //            int color = memory[address];
        //            int red = (int)((double)((color >> 10) & 0x001F) / 31.0 * 255.0);
        //            int green = (int)((double)((color >> 5) & 0x001F) / 31.0 * 255.0);
        //            int blue = (int)((double)(color & 0x001F) / 31.0 * 255.0);
        //            bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(red, green, blue));
        //        }
        //    }
        //    bitmap.Dispose();
        //    bitmap = null;

        //    image.AddDirtyRect(new Int32Rect(0, 0, width, height));
        //    image.Unlock();
        //}
    }
}
