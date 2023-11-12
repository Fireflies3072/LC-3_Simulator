using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Windows.Shapes;
using Microsoft.Win32;
using unvell.ReoGrid;

namespace LC_3_Simulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Control control;

        public MainWindow()
        {
            InitializeComponent();
            
            control = new Control(dgMemory, dgRegister);
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "(*.obj)|*.obj";
            if (ofd.ShowDialog() == true)
            {
                control.ReadFile(ofd.FileName);
            }
        }

        private void btnStep_Click(object sender, RoutedEventArgs e)
        {
            control.Step();
        }
    }
}
