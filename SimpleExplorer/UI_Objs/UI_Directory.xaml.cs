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
    /// Interaction logic for UI_Directory.xaml
    /// </summary>
    public partial class UI_Directory : UserControl
    {
        private readonly DirectoryInfo dirInfo;
        private MainWindow Owner;

        public UI_Directory()
        {
            InitializeComponent();
            dirInfo = new DirectoryInfo("C:\\Windows");
            Owner=new MainWindow();
        }

        public UI_Directory(string Path)
        {
            InitializeComponent();
            dirInfo = new DirectoryInfo(Path);
            Owner = new MainWindow();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if ((MainWindow)Window.GetWindow(this) is not null) Owner = (MainWindow)Window.GetWindow(this);
            FolderName_Display.Text = dirInfo.Name;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Owner.LoadObj(dirInfo.FullName);
        }

        private void MenuItem_AddToFavorites_Click(object sender, RoutedEventArgs e)
        {
            Owner.LeftPane.Children.Add(new LeftPaneItem(0, dirInfo.FullName));
        }
    }
}
