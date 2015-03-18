using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Player.HotKey
{
    class Win32
    {

        internal enum WH
        {
            JOURNALRECORD = 0,
            JOURNALPLAYBACK = 1,
            KEYBOARD = 2,
            GETMESSAGE = 3,
            CALLWNDPROC = 4,
            CBT = 5,
            SYSMSGFILTER = 6,
            MOUSE = 7,
            HARDWARE = 8,
            DEBUG = 9,
            SHELL = 10,
            FOREGROUNDIDLE = 11,
            CALLWNDPROCRET = 12,
            KEYBOARD_LL = 13,
            MOUSE_LL = 14
        }
        internal delegate IntPtr HOOKPROC(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);
        internal struct KBDLLHOOKSTRUCT
        {
            internal UInt32 vkCode;
            internal UInt32 scanCode;
            internal UInt32 flags;
            internal UInt32 time;
            internal IntPtr extraInfo;
        }
        [Flags]
        internal enum SWP : uint
        {
            // ReSharper disable InconsistentNaming

            /// <summary>
            ///     If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
            /// </summary>
            ASYNCWINDOWPOS = 0x4000,

            /// <summary>
            ///     Prevents generation of the WM_SYNCPAINT message.
            /// </summary>
            DEFERERASE = 0x2000,

            /// <summary>
            ///     Draws a frame (defined in the window's class description) around the window.
            /// </summary>
            DRAWFRAME = 0x0020,

            /// <summary>
            ///     Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
            /// </summary>
            FRAMECHANGED = 0x0020,

            /// <summary>
            ///     Hides the window.
            /// </summary>
            HIDEWINDOW = 0x0080,

            /// <summary>
            ///     Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
            /// </summary>
            NOACTIVATE = 0x0010,

            /// <summary>
            ///     Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
            /// </summary>
            NOCOPYBITS = 0x0100,

            /// <summary>
            ///     Retains the current position (ignores X and Y parameters).
            /// </summary>
            NOMOVE = 0x0002,

            /// <summary>
            ///     Does not change the owner window's position in the Z order.
            /// </summary>
            NOOWNERZORDER = 0x0200,

            /// <summary>
            ///     Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
            /// </summary>
            NOREDRAW = 0x0008,

            /// <summary>
            ///     Same as the SWP_NOOWNERZORDER flag.
            /// </summary>
            NOREPOSITION = 0x0200,

            /// <summary>
            ///     Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            /// </summary>
            NOSENDCHANGING = 0x0400,

            /// <summary>
            ///     Retains the current size (ignores the cx and cy parameters).
            /// </summary>
            NOSIZE = 0x0001,

            /// <summary>
            ///     Retains the current Z order (ignores the hWndInsertAfter parameter).
            /// </summary>
            NOZORDER = 0x0004,

            /// <summary>
            ///     Displays the window.
            /// </summary>
            SHOWWINDOW = 0x0040,

            // ReSharper restore InconsistentNaming
        }

        #region HotKey
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint controlKey, uint virtualKey);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        #region Hook
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(WH idHook, HOOKPROC lpfn, IntPtr hMod, int dwThreadId);
        [DllImport("user32.dll")]
        internal static extern int UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll")]
        internal static extern IntPtr CallNextHookEx(IntPtr hook, int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);
        #endregion

        #region Window
        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SWP uFlags);
        #endregion

        #region Region
        [DllImport("Gdi32.dll")]
        internal static extern IntPtr CreateRectRgn([In] int nLeftRect, [In] int nTopRect, [In] int nRightRect, [In] int nBottomRect);
        [DllImport("Gdi32.dll")]
        internal static extern bool DeleteObject([In] IntPtr hObject);
        #endregion

        #region Others
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion
    }
}
