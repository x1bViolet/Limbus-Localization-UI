using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace LCLocalizationInterface.Instruments
{
    #pragma warning disable CA1041

    [Obsolete("(For WindowStyle=\"None\" Windows) Something like creating another transparent window with just a border that has drop shadow and synced to the owner............................. But I dont know how to/can't place it behind the owner window", true)]
    public class VisualizedDropShadowBgWindow : Window
    {
        #pragma warning disable CA1806
        #pragma warning disable SYSLIB1054

        #pragma warning disable CS1030

        private static readonly IntPtr HWND_BOTTOM = 1;

        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_SHOWWINDOW = 0x0040;

        private const int GWL_EXSTYLE = -20;

        public VisualizedDropShadowBgWindow(Window OwnerWindow)
        {
            this.Owner = OwnerWindow;
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.Background = Brushes.Transparent;
            this.ShowInTaskbar = false;
            this.Topmost = false;

            DropShadowEffect WindowDropShadow = new()
            {
                ShadowDepth = 0,
                Color = Colors.Black,
                BlurRadius = 25
            };
            Border VisualBorder = new() { BorderThickness = new Thickness(1), Margin = new Thickness(25), Effect = WindowDropShadow };
            //T.SetBinding(Border.CornerRadiusProperty, new Binding() { Source = MainWindowInstance.VisualBorder, Path = new PropertyPath(nameof(ClippingBorder.CornerRadius)) });
            //T.SetBinding(Border.BorderBrushProperty, new Binding() { Source = MainWindowInstance.VisualBorder, Path = new PropertyPath(nameof(ClippingBorder.BorderBrush)) });
            this.Content = VisualBorder;

            this.Loaded += (_, _) => SetWindowStyles();

            OwnerWindow.LocationChanged += (_, _) => SyncWithOwner();
            OwnerWindow.SizeChanged += (_, _) => SyncWithOwner();
            OwnerWindow.StateChanged += (_, _) => SyncState();

            SyncWithOwner();

            this.Show();
        }

        private void SetWindowStyles()
        {
            IntPtr ThisHWND = new WindowInteropHelper(this).Handle;
            int ExStyle = GetWindowLong(ThisHWND, GWL_EXSTYLE);

            SetWindowLong(ThisHWND, GWL_EXSTYLE, ExStyle | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW | WS_EX_TRANSPARENT);

            /*--------------------------------------------------------------------------------------*/
            #warning SetWindowPos should put this window behind the owner window but it doesn't.. idk
            SetWindowPos(ThisHWND, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
            /*--------------------------------------------------------------------------------------*/
        }

        private void SyncWithOwner()
        {
            this.Left = this.Owner.Left - 25;
            this.Top = this.Owner.Top - 25;

            this.Width = this.Owner.Width + 50;
            this.Height = this.Owner.Height + 50;
        }

        private void SyncState()
        {
            if (this.Owner.WindowState.EqualsToOneOf(WindowState.Minimized, WindowState.Maximized))
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }


        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);


        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    }
}
