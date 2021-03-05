using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TarkovToolboxV2.Utils
{
    static class ProcessWatcher
    {
        public static void StartWatcher()
        {
            _timer = new Timer(100);
            _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            _timer.Start();
        }
        public static void StopWatcher()
        {
            _timer.Dispose();
        }

        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            setLastActive();
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();


        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private static IntPtr LastHandle
        {
            get
            {
                return _previousToLastHandle;
            }
        }

        private static void setLastActive()
        {
            IntPtr currentHandle = GetForegroundWindow();
            if (currentHandle != _previousHandle)
            {
                _previousToLastHandle = _previousHandle;
                _previousHandle = currentHandle;
            }
        }

        public static bool LastActiveWindowWasTarkov()
        {
            return TestWindowText();

            bool TestWindowText()
            {
                const int nChars = 256;
                StringBuilder Buff = new StringBuilder(nChars);
                if (GetWindowText(_previousToLastHandle, Buff, nChars) > 0)
                    return Buff.ToString() == "EscapeFromTarkov";
                else
                    return false;
            }
        }
        public static bool LastActiveWindowWasToolbox()
        {
            return TestWindowText();

            bool TestWindowText()
            {
                const int nChars = 256;
                StringBuilder Buff = new StringBuilder(nChars);
                if (GetWindowText(_previousToLastHandle, Buff, nChars) > 0)
                    return Buff.ToString() == "TarkovToolboxV2";
                else
                    return false;
            }
        }

        private static Timer _timer;
        private static IntPtr _previousHandle = IntPtr.Zero;
        private static IntPtr _previousToLastHandle = IntPtr.Zero;
    }
}
