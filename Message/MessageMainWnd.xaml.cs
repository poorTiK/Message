using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Message.ViewModel;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace Message
{
    /// <summary>
    /// Interaction logic for MessageMainWnd.xaml
    /// </summary>
    public partial class MessageMainWnd : Window, IMessaging
    {
        #region Win32 Magic

        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }
            return (IntPtr)0;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>x coordinate of point.</summary>
            public int x;

            /// <summary>y coordinate of point.</summary>
            public int y;

            /// <summary>Construct a point of coordinates (x,y).</summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
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
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public static readonly RECT Empty = new RECT();
            public int Width { get { return Math.Abs(right - left); } }
            public int Height { get { return bottom - top; } }

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }

            public bool IsEmpty { get { return left >= right || top >= bottom; } }

            public override string ToString()
            {
                if (this == Empty)
                { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Rect))
                { return false; }
                return (this == (RECT)obj);
            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode() => left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();

            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2) { return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom); }

            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2) { return !(rect1 == rect2); }
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #endregion Win32 Magic

        public List<MessageT> MessageList { get; set; }

        public MessageMainWnd()
        {
            InitializeComponent();

            DataContext = new MessageMainVM(this);

            init();
        }

        public MessageMainWnd(User user)
        {
            InitializeComponent();

            DataContext = new MessageMainVM(this, user);

            init();

            MessageList = new List<MessageT>();
        }

        private void init()
        {
            SourceInitialized += (s, e) =>
            {
                IntPtr handle = (new WindowInteropHelper(this)).Handle;
                HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
            };

            ScrollV.ScrollToEnd();

            MinimizeButton.Click += (s, e) => WindowState = WindowState.Minimized;
            MaximizeMinimizeButton.Click += (s, e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            CloseButton.Click += (s, e) => Application.Current.Shutdown(0);
        }

        public void AnimatedResize(int h, int w)
        {
        }

        public void CloseWindow()
        {
            Application.Current.Shutdown(0);
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            Storyboard storyboard = new Storyboard();

            DoubleAnimation daWidth = new DoubleAnimation(SideMenu.ActualWidth, 200, new Duration(TimeSpan.FromSeconds(0.4)));

            storyboard.Children.Add(daWidth);

            Storyboard.SetTarget(daWidth, SideMenu);
            Storyboard.SetTargetProperty(daWidth, new PropertyPath(WidthProperty));

            SideMenu.BeginStoryboard(storyboard);

            SideMenu.Focus();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Storyboard storyboard = new Storyboard();

            DoubleAnimation daWidth = new DoubleAnimation(SideMenu.ActualWidth, 0, new Duration(TimeSpan.FromSeconds(0.4)));

            storyboard.Children.Add(daWidth);

            Storyboard.SetTarget(daWidth, SideMenu);
            Storyboard.SetTargetProperty(daWidth, new PropertyPath(WidthProperty));

            SideMenu.BeginStoryboard(storyboard);
        }

        private void SideMenu_LostFocus(object sender, RoutedEventArgs e)
        {
            ButtonClose_Click(null, null);
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        public void SetOpacity(double opacity)
        {
            //MessageMainWindow.Opacity = opacity;
        }

        public void UpdateMessageList()
        {
            MessageControl.Children.Clear();

            foreach (var message in MessageList)
            {
                if (message.SenderId == GlobalBase.CurrentUser.Id)
                {
                    MessageControl.Children.Add(new SendMessage(GlobalBase.Base64Decode(message.Content), message.DateOfSending));
                }
                else
                {
                    MessageControl.Children.Add(new ReceiveMessage(GlobalBase.Base64Decode(message.Content), message.DateOfSending));
                }
            }

            ScrollV.ScrollToEnd();
        }

        public void Hide(bool isVisible)
        {
            if (isVisible)
            {
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Hidden;
            }
        }

        private void ScrollV_OnGotFocus(object sender, RoutedEventArgs e)
        {
            ButtonClose_Click(null, null);
        }
    }
}