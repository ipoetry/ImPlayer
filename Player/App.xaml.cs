using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Player
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        //public struct COPYDATASTRUCT
        //{
        //    public IntPtr dwData;
        //    public int cbData;
        //    [MarshalAs(UnmanagedType.LPStr)]
        //    public string lpData;
        //}

        //[DllImport("User32.dll")]
        //private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        //[DllImport("User32.dll")]
        //private static extern bool SetForegroundWindow(IntPtr hWnd);

        //[DllImport("User32.dll", EntryPoint = "SendMessage")]
        //private static extern int SendMessage(
        //int hWnd, // handle to destination window
        //int Msg, // message
        //int wParam, // first message parameter
        //ref COPYDATASTRUCT lParam // second message parameter
        //);

        //[DllImport("User32.dll", EntryPoint = "FindWindow")]
        //private static extern int FindWindow(string lpClassName, string lpWindowName);

        //private const int WS_HIDE = 0;
        //private const int WS_SHOWNORMAL = 1;
        //private const int WS_SHOWMINIMIZED = 2;
        //private const int WS_SHOWMAXIMIZED = 3;
        //private const int WS_MAXIMIZE = 3;
        //private const int WS_SHOWNOACTIVATE = 4;
        //private const int WS_SHOW = 5;
        //private const int WS_MINIMIZE = 6;
        //private const int WS_SHOWMINNOACTIVE = 7;
        //private const int WS_SHOWNA = 8;
        //private const int WS_RESTORE = 9;

        //public static Process RunningInstance()
        //{
        //    Process current = Process.GetCurrentProcess();
        //    Process[] processes = Process.GetProcessesByName(current.ProcessName);
        //    //Loop   through   the   running   processes   in   with   the   same   name  
        //    foreach (Process process in processes)
        //    {
        //        //Ignore   the   current   process  
        //        if (process.Id != current.Id)
        //        {
        //            //Make   sure   that   the   process   is   running   from   the   exe   file.  
        //            if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
        //            {
        //                //Return   the   other   process   instance.  
        //                return process;
        //            }
        //        }
        //    }
        //    //No   other   instance   was   found,   return   null.
        //    return null;
        //}

        //public static void HandleRunningInstance(Process instance)
        //{
        //    //Make   sure   the   window   is   not   minimized   or   maximized  
        //    ShowWindowAsync(instance.MainWindowHandle, WS_SHOWMAXIMIZED);
        //    //Set   the   real   intance   to   foreground   window
        //    SetForegroundWindow(instance.MainWindowHandle);
        //}
        //[STAThread]
        //static void Main(string[] args)
        //{

        //    Process instance = RunningInstance();
        //    bool bExist = true;
        //    string ss = string.Empty;
        //    Mutex MyMutex = new Mutex(true, "ONLYONETASK", out bExist);

        //    if (!bExist)
        //    {

        //        //Make   sure   the   window   is   not   minimized   or   maximized  
        //        ShowWindowAsync(instance.MainWindowHandle, WS_SHOWMAXIMIZED);

        //        //Set   the   real   intance   to   foreground   window
        //        SetForegroundWindow(instance.MainWindowHandle);

        //        string[] cmds = Environment.GetCommandLineArgs();
        //        List<string> cmd2s = cmds.ToList();
        //        cmd2s.RemoveAt(0);
        //        string argstrs = String.Join(" ", cmd2s);

        //        if (cmds.Length > 0)
        //        {
        //            int WINDOW_HANDLER = FindWindow(null, @"Form1");

        //            if (WINDOW_HANDLER != 0)
        //            {
        //                COPYDATASTRUCT cds;
        //                cds.dwData = (IntPtr)100;
        //                cds.lpData = argstrs;//cmds[1];
        //                cds.cbData = argstrs.Length * 2;//cmds[1].Length * 2;
        //                SendMessage(WINDOW_HANDLER, 0x004A, 0, ref cds);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Application app = new Application();
                
        //        MainPage mianWindow = new MainPage();
        //        string[] cmds = Environment.GetCommandLineArgs();
        //        List<string> cmd2s = cmds.ToList();
        //        cmd2s.RemoveAt(0);
        //        string argstrs = String.Join(" ", cmd2s);
        //        MessageBox.Show(argstrs);
        //        if (cmds.Length > 0)
        //        {
        //            int WINDOW_HANDLER = FindWindow(null, @"Form1");

        //            if (WINDOW_HANDLER != 0)
        //            {
        //                COPYDATASTRUCT cds;
        //                cds.dwData = (IntPtr)100;
        //                cds.lpData = argstrs;//cmds[1];
        //                cds.cbData = argstrs.Length * 2;//cmds[1].Length * 2;
        //                SendMessage(WINDOW_HANDLER, 0x004A, 0, ref cds);
        //            }
        //        }
        //        app.Run(mianWindow);

        //    }
        //}
    }
}
