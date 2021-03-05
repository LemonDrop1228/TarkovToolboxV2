using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace TarkovToolboxV2.Utils
{
    public static class AltTabHelper
    {

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EX_STYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;

        internal static void RemoveFromAltTab(Window w)
        {
            //Variable to hold the handle for the form
            var helper = new WindowInteropHelper(w).Handle;
            //Performing some magic to hide the form from Alt+Tab
            SetWindowLong(helper, GWL_EX_STYLE, (GetWindowLong(helper, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);
        }

    }
}
