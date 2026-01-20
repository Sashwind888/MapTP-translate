using Linearstar.Windows.RawInput;
using System;
using System.Windows;
using System.Windows.Interop;

namespace MapTP.App
{
    /// <summary>
    /// CalibrateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CalibrateWindow : Window
    {
        private int X = 0, Y = 0;
        private HwndSource MainWindowHwnd;

        public CalibrateWindow()
        {
            InitializeComponent();
            throw (new Exception());
        }

        public CalibrateWindow(HwndSource _MainWindowHwnd) : base()
        {
            InitializeComponent();
            MainWindowHwnd = _MainWindowHwnd;
            MainWindowHwnd.AddHook(WndProc);
        }



        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_INPUT = 0x00FF;
            switch (msg)
            {
                case WM_INPUT:
                    var data = RawInputData.FromHandle(lParam);
                    if (data is RawInputDigitizerData digitizerData)
                    {
                        foreach (var x in digitizerData.Contacts)
                        {
                            if (x.Identifier == 0) // limiting ContactId(Identifier) to 0 is to read the first finger
                            {
                                X = (int)Math.Ceiling((double)(x.X / 100f)) * 100;
                                Y = (int)Math.Ceiling((double)(x.Y / 100f)) * 100;
                                TouchpadSize.Text = $"触控板大小: {X} x {Y}";
                            }
                        }
                    }
                    break;
            }
            return IntPtr.Zero;
        }


        public delegate void SendSize(int X, int Y);
        public SendSize sendSize;

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindowHwnd.RemoveHook(WndProc);
            if (X == 0 && Y == 0) DialogResult = false;
            else
            {
                sendSize(X, Y);
                DialogResult = true;
            }
            Close();
        }
    }
}
