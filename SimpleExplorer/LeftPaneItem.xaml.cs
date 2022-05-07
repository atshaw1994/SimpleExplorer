using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for LeftPaneItem.xaml
    /// </summary>
    public partial class LeftPaneItem : UserControl
    {
        private string Path;
        private int Type;
        private MainWindow Owner;
        public LeftPaneItem()
        {
            InitializeComponent();
            Type = 0;
            Path = "C:\\Users";
            Owner = new MainWindow();
        }
        public LeftPaneItem(int Type, string Path)
        {
            InitializeComponent();
            this.Type = Type;
            this.Path = Path;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if ((MainWindow)Window.GetWindow(this) is not null) Owner = (MainWindow)Window.GetWindow(this);
            ItemName_Display.Text = System.IO.Path.GetFileName(Path);
            if (Type == 0) ItemType0.Visibility = Visibility.Visible;
            else if (Type == 1) ItemType1.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Owner.LoadObj(Path);
        }
    }
}
