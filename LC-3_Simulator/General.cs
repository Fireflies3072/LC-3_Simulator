using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LC_3_Simulator
{
    class General
    {
        public static Assembly assembly = Assembly.GetExecutingAssembly();

        public static List<Control> controlList = new List<Control>();
        public static List<TextEditor> codeEditorList = new List<TextEditor>();
        public static List<DataGrid> dgMemoryList = new List<DataGrid>();
        public static List<DataGrid> dgRegisterList = new List<DataGrid>();
        public static List<Image> ioImageList = new List<Image>();
        public static List<TextBlock> ioTextList = new List<TextBlock>();

        public static int tabIndex = 0;
        public static BackgroundWorker worker;
        public static bool running = false;
        public static int lastTabIndex = -1;
        public static int lastTabCount = 0;

        public static bool folderOpen = false;
        public static string folderPath = "";
        public static List<string> filenameList = new List<string>();

        public static string newFilename = "";
    }
}
