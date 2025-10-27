using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Text.Unicode;
using System.Runtime.InteropServices;

namespace LC_3_Simulator
{
    internal class Control
    {
        private Memory memory;
        private Register register;
        public WriteableBitmap image;
        private DataGrid dgMemory;
        private DataGrid dgRegister;
        private Image ioImage;
        private TextBlock ioText;
        
        private Symbol symbol;

        public Control(DataGrid dgMemory, DataGrid dgRegister, Image ioImage, TextBlock ioText)
        {
            // 保存参数 save parameters
            this.dgMemory = dgMemory;
            this.dgRegister = dgRegister;
            this.ioImage = ioImage;
            this.ioText = ioText;

            Reset();
        }

        public void ReadFile(string filename)
        {
            // 读取同名符号文件 check symbol file with the same name
            symbol.ReadSymFile(Path.ChangeExtension(filename, ".sym"));
            // 读取目标文件 read object file
            memory.ReadObjFile(filename, register, symbol);

            // 显示在表格中 show in grid
            dgMemory.SelectedIndex = register.ReadPC();
            dgMemory.ScrollIntoView(dgMemory.Items.GetItemAt(dgMemory.SelectedIndex + 15));
            dgMemory.UpdateLayout();
        }

        public void ReadOS()
        {
            // 读取同名符号文件 check symbol file with the same name
            using (Stream stream = General.assembly.GetManifestResourceStream("LC_3_Simulator.os.lc3os.sym"))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string text = reader.ReadToEnd();
                    symbol.ReadSymText(text);
                }
            }
            // 读取目标文件 read object file
            using (Stream stream = General.assembly.GetManifestResourceStream("LC_3_Simulator.os.lc3os.obj"))
            {
                memory.ReadObjFile(stream, register, symbol);
            }
            // 显示在表格中 show in grid
            dgMemory.SelectedIndex = register.ReadPC();
            dgMemory.ScrollIntoView(dgMemory.Items.GetItemAt(dgMemory.SelectedIndex + 15));
            dgMemory.UpdateLayout();
        }

        public void Step()
        {
            // 读取指令地址 read instruction address
            int PC = register.ReadPC();
            register.AddPC();
            // 读取指令 read instruction
            int data = memory.ReadMemory(PC);
            Instruction instruction = new Instruction(data, symbol, register.ReadPC());

            // 执行指令 execute instruction
            if (!instruction.Execute(memory, register, image, ioText))
            {
                MessageBox.Show("Not a valid instruction", "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        public void UpdateUI()
        {
            // 更新表 update grid
            dgMemory.SelectedIndex = register.ReadPC();
            dgMemory.Items.Refresh();
            dgRegister.Items.Refresh();
        }

        public void Reset()
        {
            // 初始化内存和寄存器 initialize memory and register
            memory = new Memory();
            dgMemory.ItemsSource = memory.memoryGrid;
            register = new Register();
            dgRegister.ItemsSource = register.registerGrid;
            // 初始化输出图像 initialize output image
            image = new WriteableBitmap(128, 124, 96, 96, PixelFormats.Bgr24, null);
            ioImage.Source = image;
            // 初始化控制台文字 initialze console text
            ioText.Text = "";
            ioText.IsEnabled = false;

            // 初始化符号 initialize symbol
            symbol = new Symbol();
        }
    }
}
