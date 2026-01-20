using Linearstar.Windows.RawInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
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
using System.Windows.Shapes;

namespace MapTP.App
{
    /// <summary>
    /// AdvancedWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AdvancedWindow : Window
    {
        private HwndSource MainWindowHwnd;
        private bool init;

        public AdvancedWindow(HwndSource _MainWindowHwnd)
        {
            InitializeComponent();
            MainWindowHwnd = _MainWindowHwnd;
            MainWindowHwnd.AddHook(WndProc);
            init = false;
        }

        public void Log(string message)
        {
            if (!init)
            {
                init = true;
                LogBox.Text = "";
            }
            if (OneLineCB.IsChecked.Value) LogBox.Text = "";
            LogBox.Text += message + "\n";
            if (AutoScrollCB.IsChecked.Value) LogBox.ScrollToEnd();
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
                            switch (x.Identifier)
                            {
                                case 0:
                                    if (TPInputCB.IsChecked.Value)
                                        Log($"TP#{x.Identifier} x:{x.X} y:{x.Y} Tip:{x.IsButtonDown}");
                                    break;
                                default:
                                    if (TPInputCB.IsChecked.Value)
                                    {
                                        if (!TPFirstCB.IsChecked.Value)
                                        {
                                            Log($"TP#{x.Identifier} x:{x.X} y:{x.Y} Tip:{x.IsButtonDown}");
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }
            return IntPtr.Zero;
        }

    }
}

