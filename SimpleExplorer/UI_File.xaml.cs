using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleExplorer
{
    /// <summary>
    /// Interaction logic for UI_File.xaml
    /// </summary>
    public partial class UI_File : UserControl
    {
        private readonly FileInfo fileInfo;

        public UI_File()
        {
            InitializeComponent();
            fileInfo = new FileInfo("C:\\Windows\\");
        }

        public UI_File(string Path)
        {
            InitializeComponent();
            fileInfo = new FileInfo(Path);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(fileInfo.FullName);
        }
    }
}
