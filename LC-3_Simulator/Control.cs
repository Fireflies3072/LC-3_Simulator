using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using unvell.ReoGrid;
using unvell.ReoGrid.IO.OpenXML.Schema;

namespace LC_3_Simulator
{
    internal class Control
    {
        private Memory memory;
        private Register register;
        private DataGrid dgMemory;
        private DataGrid dgRegister;
        
        private Symbol symbol;

        public Control(DataGrid dgMemory, DataGrid dgRegister)
        {
            // 保存参数 save parameters
            this.dgMemory = dgMemory;
            this.dgRegister = dgRegister;
            
            reset();
        }

        public bool ReadFile(string filename)
        {
            try
            {
                reset();

                // 读取同名符号文件 check symbol file with the same name
                symbol.ReadSymFile(Path.ChangeExtension(filename, ".sym"));
                // 读取目标文件 read object file
                memory.ReadObjFile(filename, register, symbol);

                // 显示在表格中 show in grid
                dgMemory.SelectedIndex = register.ReadPC();
                dgMemory.ScrollIntoView(dgMemory.Items.GetItemAt(dgMemory.SelectedIndex + 15));
                dgMemory.UpdateLayout();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
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
            if (!instruction.Execute(memory, register))
            {
                MessageBox.Show("Not a valid instruction", "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // 更新表 update grid
            dgMemory.SelectedIndex = register.ReadPC();
            dgMemory.Items.Refresh();
            dgRegister.Items.Refresh();
        }

        public void reset()
        {
            // 初始化内存和寄存器 initialize memory and register
            memory = new Memory();
            dgMemory.ItemsSource = memory.memoryGrid;
            register = new Register();
            dgRegister.ItemsSource = register.registerGrid;

            // 初始化符号
            symbol = new Symbol();
        }
    }
}
