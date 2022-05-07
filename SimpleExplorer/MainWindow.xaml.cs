using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly string DefaultDir = "Root";

        private string _CurrentFolder;

        public string CurrentFolder
        {
            get { return _CurrentFolder; }
            set
            {
                _CurrentFolder = value;
                OnPropertyChanged();
            }
        }

            #pragma warning disable CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.
        public event PropertyChangedEventHandler PropertyChanged;
            #pragma warning restore CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.

        #region BorderlessMethods

        private void CaptionBar_CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CaptionBar_RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CaptionBar_MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void RefreshMaximizeRestoreButton()
        {
            if (WindowState == WindowState.Maximized)
            {
                CaptionBar_MaximizeButton.Visibility = Visibility.Collapsed;
                CaptionBar_RestoreButton.Visibility = Visibility.Visible;
            }
            else
            {
                CaptionBar_MaximizeButton.Visibility = Visibility.Visible;
                CaptionBar_RestoreButton.Visibility = Visibility.Collapsed;
            }
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            RefreshMaximizeRestoreButton();
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook(HookProc);
        }

        public static IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_GETMINMAXINFO)
            {
                // We need to tell the system what our size should be when maximized. Otherwise it will cover the whole screen,
                // including the task bar.
                MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

                // Adjust the maximized size and position to fit the work area of the correct monitor
                IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

                if (monitor != IntPtr.Zero)
                {
                    MONITORINFO monitorInfo = new();
                    monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                    GetMonitorInfo(monitor, ref monitorInfo);
                    RECT rcWorkArea = monitorInfo.rcWork;
                    RECT rcMonitorArea = monitorInfo.rcMonitor;
                    mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                    mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                    mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                    mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
                }

                Marshal.StructureToPtr(mmi, lParam, true);
            }

            return IntPtr.Zero;
        }

        private const int WM_GETMINMAXINFO = 0x0024;

        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }
        #endregion

        public MainWindow()
        { 
            InitializeComponent();
            LoadObj(DefaultDir);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void LoadObj(string Path)
        {
            ObjDisp.Children.Clear();
            if (!Equals("Root", Path))
            {
                CurrentFolder = Path;
                string[] dirList = Directory.GetDirectories(CurrentFolder);
                string[] fileList = Directory.GetFiles(CurrentFolder);
                foreach (string dir in dirList)
                    ObjDisp.Children.Add(new UI_Directory(dir));
                foreach (string file in fileList)
                    ObjDisp.Children.Add(new UI_File(file));
            }
            else
            {
                string[] driveList = Directory.GetLogicalDrives();
                foreach (string drive in driveList)
                    ObjDisp.Children.Add(new UI_Disk(drive));
            }
            AddressBar.Text = Path;
        }

        private void NavButton_Up_Click(object sender, RoutedEventArgs e)
        {
            if (CountSlashes(CurrentFolder) > 1)
                LoadObj(CurrentFolder[..CurrentFolder.LastIndexOf('\\')]);
            else
                LoadObj("Root");
        }

        private int CountSlashes(string str)
        {
            int Slashes = 0;
            for (int i = 0; i < str.Length; i++)
                if (str[i] == '\\') Slashes++;
            return Slashes;
        }

        private void NavButton_Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadObj(CurrentFolder);
        }

        private void AddressBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Directory.Exists(AddressBar.Text))
                {
                    LoadObj(AddressBar.Text);
                }
                else
                {
                    MessageBox.Show(this, $"'{AddressBar.Text}' not found! Please check URL and try again.", "Directory Not Found!",MessageBoxButton.OK, MessageBoxImage.Error);
                    AddressBar.Text = CurrentFolder;
                    AddressBar.Focus();
                }

            }

        }

    }
}
