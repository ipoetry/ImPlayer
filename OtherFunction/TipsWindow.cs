using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace OtherFunction
{
    public class TipsWindow
    {
        [DllImport("User32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll")]
        static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private class CloseState
        {
            private int _Timeout;

            /// <summary>
            /// In millisecond
            /// </summary>
            public int Timeout
            {
                get
                {
                    return _Timeout;
                }
            }

            private string _Caption;

            /// <summary>
            /// Caption of dialog
            /// </summary>
            public string Caption
            {
                get
                {
                    return _Caption;
                }
            }

            public CloseState(string caption, int timeout)
            {
                _Timeout = timeout;
                _Caption = caption;
            }
        }

        public static void Show(string text, string caption, int timeout)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(CloseMessageBox),
                new CloseState("TipsWindow", timeout));
            DesktopWord.Show(text,caption);
        }

        private static void CloseMessageBox(object state)
        {
            CloseState closeState = state as CloseState;

            Thread.Sleep(closeState.Timeout);
            IntPtr dlg = FindWindow(null, closeState.Caption);
            if (dlg != IntPtr.Zero)
            {
                PostMessage(dlg, 0x10, System.IntPtr.Zero, System.IntPtr.Zero);
            }
        }
    }
}
