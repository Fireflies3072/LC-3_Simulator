using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Threading;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace LC_3_Simulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                SystemCommands.MaximizeWindow(this);
            }
            else
            {
                SystemCommands.RestoreWindow(this);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void gridHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnNewFile_Click(object sender, RoutedEventArgs e)
        {
            // 检查是否打开文件夹 check if folder is open
            if (!General.folderOpen)
            {
                MessageBox.Show("Please open a folder first", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            // 显示输入文件名对话框 show filename input dialog
            NewFileDialog dialog = new NewFileDialog();
            if (dialog.ShowDialog() == true)
            {
                if (!General.newFilename.EndsWith(".asm"))
                {
                    General.newFilename += ".asm";
                }
                string filename = Path.Join(General.folderPath, General.newFilename);
                if (File.Exists(filename))
                {
                    MessageBox.Show("File with the same name exists", "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // 创建并打开文件 create and open file
                File.WriteAllText(filename, "", Encoding.UTF8);
                OpenFile(filename);
            }
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Code|*.asm;*.obj;*.sym";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // 打开文件夹 open folder
                OpenFolder(Path.GetDirectoryName(ofd.FileName));
                // 打开文件 open file
                OpenFile(ofd.FileName);
            }
        }

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // 打开文件夹 open folder
                OpenFolder(fbd.SelectedPath);
            }
        }

        private void btnCloseFolder_Click(object sender, RoutedEventArgs e)
        {
            if (!General.folderOpen) { return; }
            // 清空文件列表 clear file list
            lstFolder.Items.Clear();
            // 显示隐藏组件 show and hide components
            lstFolder.Visibility = Visibility.Hidden;
            btnOpenFolder2.Visibility = Visibility.Visible;
            // 更新状态 update status
            General.folderOpen = false;
            General.folderPath = "";
        }

        private void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSaveAll_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void OpenFolder(string? folderPath)
        {
            // 检查错误 check error
            if (folderPath == null) { return; }
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("Folder not exists", "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // 显示隐藏组件 show and hide components
            lstFolder.Visibility = Visibility.Visible;
            btnOpenFolder2.Visibility = Visibility.Hidden;
            // 清空文件列表 clear file list
            lstFolder.Items.Clear();
            // 读取文件夹内信息 read info inside folder
            lblFolder.Content = Path.GetFileName(folderPath);
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            // 添加文件夹 add folder
            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                Label label = new Label();
                label.Content = directory.Name;
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.Margin = new Thickness(24, 0, 0, 0);
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("pack://application:,,,/image/folder.png"));
                image.Width = 16;
                image.Height = 16;
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.Margin = new Thickness(5, 0, 0, 0);
                Grid grid = new Grid();
                grid.Children.Add(image);
                grid.Children.Add(label);
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = grid;
                lstFolder.Items.Add(listBoxItem);
            }
            // 添加文件 add file
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                Label label = new Label();
                label.Content = file.Name;
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.Margin = new Thickness(24, 0, 0, 0);
                string imageName;
                if (file.Name.EndsWith(".asm"))
                {
                    imageName = "code.png";
                }
                else if (file.Name.EndsWith(".sym"))
                {
                    imageName = "document.png";
                }
                else if (file.Name.EndsWith(".obj"))
                {
                    imageName = "binary.png";
                }
                else
                {
                    continue;
                }
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("pack://application:,,,/image/" + imageName));
                image.Width = 16;
                image.Height = 16;
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.Margin = new Thickness(5, 0, 0, 0);
                Grid grid = new Grid();
                grid.Children.Add(image);
                grid.Children.Add(label);
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = grid;
                lstFolder.Items.Add(listBoxItem);
            }
            // 更新状态 update status
            General.folderOpen = true;
            General.folderPath = folderPath;
        }

        private void OpenFile(string? filename)
        {
            // 检查错误 check error
            if (filename == null) { return; }
            string extension = Path.GetExtension(filename);
            if (!File.Exists(filename))
            {
                MessageBox.Show("File not exists", "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(extension != ".asm" && extension != ".obj" && extension != ".sym")
            {
                MessageBox.Show("File type not supported", "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 读取文件 read file
            try
            {
                NewTab(Path.GetFileName(filename));
                General.filenameList.Add(filename);
                if (Path.GetExtension(filename) == ".asm")
                {
                    General.codeEditorList[General.tabIndex].Text = File.ReadAllText(filename, Encoding.UTF8);
                }
                else if (Path.GetExtension(filename) == ".obj")
                {
                    General.controlList[General.tabIndex].ReadFile(filename);
                }
                else if (Path.GetExtension(filename) == ".sym")
                {
                    General.codeEditorList[General.tabIndex].Text = File.ReadAllText(filename, Encoding.UTF8);
                }
            }
            catch
            {
                MessageBox.Show("Cannot read file", "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void NewTab(string tabName)
        {
            // 设置代码编辑器 set code editor
            TextEditor codeEditor = new TextEditor();
            codeEditor.Margin = new Thickness(20, 20, 0, 20);
            codeEditor.FontFamily = new FontFamily("Consolas");
            codeEditor.FontSize = 16;
            codeEditor.ShowLineNumbers = true;
            codeEditor.LineNumbersForeground = new SolidColorBrush(Colors.Black);
            // 读取LC-3高亮语法文件 read LC-3 highlighting syntax file
            using (Stream s = General.assembly.GetManifestResourceStream("LC_3_Simulator.syntax.lc-3.xshd"))
            {
                using (var reader = new System.Xml.XmlTextReader(s))
                {
                    codeEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            // 设置内存表 set memory sheet
            DataGrid dgMemory = new DataGrid();
            dgMemory.Margin = new Thickness(20, 20, 20, 20);
            dgMemory.IsReadOnly = true;
            dgMemory.AutoGenerateColumns = false;
            dgMemory.RowHeaderWidth = 0;
            dgMemory.CanUserAddRows = false;
            dgMemory.CanUserDeleteRows = false;
            dgMemory.CanUserReorderColumns = false;
            dgMemory.CanUserResizeColumns = false;
            dgMemory.CanUserResizeRows = false;
            dgMemory.CanUserSortColumns = false;
            dgMemory.Columns.Add(new DataGridCheckBoxColumn() { Header = "BP", Binding = new Binding("BP"), Width = new DataGridLength(1.0, DataGridLengthUnitType.Star) });
            dgMemory.Columns.Add(new DataGridTextColumn() { Header = "Address", Binding = new Binding("Address"), Width = new DataGridLength(3.0, DataGridLengthUnitType.Star) });
            dgMemory.Columns.Add(new DataGridTextColumn() { Header = "Value", Binding = new Binding("Value"), Width = new DataGridLength(2.0, DataGridLengthUnitType.Star) });
            dgMemory.Columns.Add(new DataGridTextColumn() { Header = "Description", Binding = new Binding("Description"), Width = new DataGridLength(4.0, DataGridLengthUnitType.Star) });

            // 设置寄存器表 set register sheet
            DataGrid dgRegister = new DataGrid();
            dgRegister.Margin = new Thickness(20, 20, 0, 0);
            dgRegister.Height = 225;
            dgRegister.RowHeight = 20;
            dgRegister.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            dgRegister.VerticalAlignment = VerticalAlignment.Top;
            dgRegister.IsReadOnly = true;
            dgRegister.AutoGenerateColumns = false;
            dgRegister.RowHeaderWidth = 0;
            dgRegister.CanUserAddRows = false;
            dgRegister.CanUserDeleteRows = false;
            dgRegister.CanUserReorderColumns = false;
            dgRegister.CanUserResizeColumns = false;
            dgRegister.CanUserResizeRows = false;
            dgRegister.CanUserSortColumns = false;
            dgRegister.Columns.Add(new DataGridTextColumn() { Header = "Name", Binding = new Binding("Name"), Width = new DataGridLength(1.0, DataGridLengthUnitType.Star) });
            dgRegister.Columns.Add(new DataGridTextColumn() { Header = "Value", Binding = new Binding("Value"), Width = new DataGridLength(2.0, DataGridLengthUnitType.Star) });

            // 设置图像 set image
            Image ioImage = new Image();
            ioImage.Margin = new Thickness(20, 20, 0, 0);

            // 设置控制台 set console
            TextBlock ioText = new TextBlock();
            ioText.Text = "apple\njfd";
            Border border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Black;
            border.Margin = new Thickness(20, 20, 0, 20);
            border.Child = ioText;

            // 把创建的组件保存为全局变量 save the created components as global variables
            Control control = new Control(dgMemory, dgRegister, ioImage, ioText);
            General.controlList.Add(control);
            General.codeEditorList.Add(codeEditor);
            General.dgMemoryList.Add(dgMemory);
            General.dgRegisterList.Add(dgRegister);
            General.ioImageList.Add(ioImage);
            General.ioTextList.Add(ioText);

            // 设置组件位置 set components' position
            Grid grid2 = new Grid();
            grid2.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid2.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid2.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(dgRegister, 0);
            grid2.Children.Add(dgRegister);
            Grid.SetRow(ioImage, 1);
            grid2.Children.Add(ioImage);
            Grid.SetRow(border, 2);
            grid2.Children.Add(border);

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2.0, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1.0, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2.0, GridUnitType.Star) });
            Grid.SetColumn(codeEditor, 0);
            grid.Children.Add(codeEditor);
            Grid.SetColumn(grid2, 1);
            grid.Children.Add(grid2);
            Grid.SetColumn(dgMemory, 2);
            grid.Children.Add(dgMemory);

            // 添加标签 add tab
            CloseableTabItem tabItem = new CloseableTabItem(tabName);
            tabItem.Content = grid;
            tc1.Items.Add(tabItem);
            tc1.SelectedItem = tabItem;
        }

        private void btnCompile_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            unsafe
            {
                General.controlList[General.tabIndex].image.Lock();
                byte[] data = new byte[128*124*3];
                random.NextBytes(data);
                Marshal.Copy(data, 0, General.controlList[General.tabIndex].image.BackBuffer, data.Length);
                General.controlList[General.tabIndex].image.AddDirtyRect(new System.Windows.Int32Rect(0, 0, 128, 124));
                General.controlList[General.tabIndex].image.Unlock();
            }
        }

        private void btnLoadOS_Click(object sender, RoutedEventArgs e)
        {
            if (tc1.Items.Count > 0)
            {
                General.controlList[General.tabIndex].ReadOS();
            }
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            // 读取文件 read file
            try
            {
                General.controlList[General.tabIndex].Reset();
                string filename = General.filenameList[General.tabIndex];
                if (Path.GetExtension(filename) == ".asm")
                {
                    General.codeEditorList[General.tabIndex].Text = File.ReadAllText(filename, Encoding.UTF8);
                }
                else if (Path.GetExtension(filename) == ".obj")
                {
                    General.controlList[General.tabIndex].ReadFile(filename);
                }
                else if (Path.GetExtension(filename) == ".sym")
                {
                    General.codeEditorList[General.tabIndex].Text = File.ReadAllText(filename, Encoding.UTF8);
                }
            }
            catch
            {
                MessageBox.Show("Cannot read file", "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            if (tc1.Items.Count > 0)
            {
                General.running = true;

                General.worker = new BackgroundWorker();
                General.worker.WorkerReportsProgress = true;
                //General.worker.WorkerSupportsCancellation = true;
                General.worker.DoWork += Continue;
                General.worker.ProgressChanged += ContinueProgress;
                General.worker.RunWorkerAsync();
            }
        }
        void Continue(object sender, DoWorkEventArgs e)
        {
            while (General.running)
            {
                for (int i = 0; i < 10; i++)
                {
                    General.controlList[General.tabIndex].Step();
                }
                General.worker.ReportProgress(0);
                Thread.Sleep(100);
            }
        }

        void ContinueProgress(object sender, ProgressChangedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                General.controlList[General.tabIndex].UpdateUI();
            }));
        }

        private void btnStepOver_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(thd);
            thread.Start();
        }

        private void thd()
        {
            General.controlList[General.tabIndex].Step();
            if (General.running)
            {
                Thread thread = new Thread(thd);
                thread.Start();
            }
            this.Dispatcher.Invoke(() =>
            {
                General.controlList[General.tabIndex].UpdateUI();
            });
        }

        private void btnStepInto_Click(object sender, RoutedEventArgs e)
        {
            if (tc1.Items.Count > 0)
            {
                General.controlList[General.tabIndex].Step();
                General.controlList[General.tabIndex].UpdateUI();
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            StopThread();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            General.controlList[General.tabIndex].Reset();
        }

        private void StopThread()
        {
            General.running = false;
        }

        private void tc1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            General.tabIndex = tc1.SelectedIndex;
            if (General.tabIndex != General.lastTabIndex || (General.tabIndex != General.lastTabIndex && tc1.Items.Count != General.lastTabCount))
            {
                General.lastTabIndex = General.tabIndex;
                General.lastTabCount = tc1.Items.Count;
                StopThread();
            }
        }

        private void lstFolder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (General.folderOpen && lstFolder.SelectedItems.Count > 0)
            {
                // 读取文件
                ListBoxItem lstItem = (ListBoxItem)lstFolder.SelectedItem;
                Grid grid = (Grid)lstItem.Content;
                Label lbl = (Label)grid.Children[1];
                string? filename = lbl.Content.ToString();
                filename = Path.Join(General.folderPath, filename);
                if (Directory.Exists(filename))
                {
                    OpenFolder(filename);
                }
                if (File.Exists(filename))
                {
                    OpenFile(filename);
                }
            }
        }
    }

    public class CloseableTabItem : TabItem
    {
        public CloseableTabItem(string header)
        {
            // 设置关闭按钮 set close button
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/image/close.png"));
            image.Width = 12;
            image.Height = 12;
            Button button = new Button();
            button.Content = image;
            button.Background = new SolidColorBrush(Colors.Transparent);
            button.BorderThickness = new Thickness(0);
            button.Width = 20;
            button.Height = 20;
            button.Click +=
                (sender, e) =>
                {
                    var tabControl = Parent as TabControl;
                    General.dgMemoryList.RemoveAt(tabControl.SelectedIndex);
                    General.dgRegisterList.RemoveAt(tabControl.SelectedIndex);
                    General.controlList.RemoveAt(tabControl.SelectedIndex);
                    General.filenameList.RemoveAt(tabControl.SelectedIndex);
                    tabControl.Items.RemoveAt(tabControl.SelectedIndex);
                };
            // 设置标签文字 set tab text
            Label label = new Label();
            label.Content = header;
            label.Width = 120;
            label.HorizontalAlignment = HorizontalAlignment.Left;
            // 设置标签 set tab
            DockPanel dockPanel = new DockPanel();
            dockPanel.Children.Add(label);
            dockPanel.Children.Add(button);
            Header = dockPanel;
        }
    }
}
