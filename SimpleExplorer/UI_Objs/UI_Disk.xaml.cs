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
    /// Interaction logic for UI_Disk.xaml
    /// </summary>
    public partial class UI_Disk : UserControl
    {
        private readonly DriveInfo driveInfo;
        private double RatioUsed;
        private static readonly string[] SizeSuffixes = { "b", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public UI_Disk()
        {
            InitializeComponent();
            driveInfo = new DriveInfo("C:\\");
        }

        public UI_Disk(string Path)
        {
            InitializeComponent();
            driveInfo = new DriveInfo(Path);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow owner = (MainWindow)Window.GetWindow(this);
            owner.LoadObj(driveInfo.Name);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DiskName_Display.Text = $"{driveInfo.Name} {driveInfo.VolumeLabel}";
            RatioUsed = 100 * (1 - (double)driveInfo.TotalFreeSpace / driveInfo.TotalSize);
            DiskFillProgressBar.Value = RatioUsed;
        }

        private static string SizeSuffix(long value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException(nameof(decimalPlaces)); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
    }
}
