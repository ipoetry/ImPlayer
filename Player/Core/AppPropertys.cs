using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Lyrics;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace Player
{
    public partial class AppPropertys
    {
        public static MainPage mainWindow;
        public static NotifyIcon notifyIcon = new NotifyIcon();
        public static DiyContextMenu DiyCM;
        public static string appPath = AppDomain.CurrentDomain.BaseDirectory;
        public static string logoText = "ImPlayer";
        public static string lrcInitText = "Love Life，Love Music！";
        public static bool IsShowWindow = true;
        public static bool isLrcShow = false;

        [DllImport("kernel32.dll")]
        public static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

        public static void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }


        public static void Initialize()
        {
            notifyIcon.Text = lrcInitText;
            notifyIcon.Visible = true;
            notifyIcon.Icon = Properties.Resources._2628;
            notifyIcon.MouseClick += notifyIcon_MouseClick;
            notifyIcon.MouseDoubleClick+=notifyIcon_MouseDoubleClick;
        }

        /// <summary>
        /// 更改托盘图标
        /// </summary>
        /// <param name="state">状态（0：常态，1：播放，2：暂停）</param>
        public static void ChangeNotifyIcon(int state)
        {
            switch (state)
            {
                case 0:
                    notifyIcon.Icon = Properties.Resources._2628;
                    break;
                case 1:
                    notifyIcon.Icon = Properties.Resources.pause;
                    break;
                case 2:
                    notifyIcon.Icon = Properties.Resources.play;
                    break;
            }
        }

        /// <summary>
        /// 双击托盘区事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void notifyIcon_MouseDoubleClick(object sender, EventArgs e)
        {
            mainWindow.Show();
            mainWindow.Activate();
            IsShowWindow = true;
        }

        /// <summary>
        /// 单击托盘区“退出”菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void notifyExit_Click(object sender, EventArgs e)
        {
            setFreeNotifyIcon();
            Environment.Exit(0);
        }

        /// <summary>
        /// 释放托盘区图标
        /// </summary>
        public static void setFreeNotifyIcon()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
        }
        /// <summary>
        /// 打开或关闭 Desktop Lyrics
        /// </summary>
        static void SetLrcShow()
        {
            if (isLrcShow)
            {
                LrcController.lrcWindow.Show();
                isLrcShow = true;
            }
            else
            {
                LrcController.lrcWindow.Hide();
                isLrcShow = false;
            }
            
        }

        public static void nofityPlay_Click(object sender, EventArgs e)
        {
            if (PlayController.bassEng.IsPlaying)
            {
                PlayController.bassEng.Pause();
            }
            else
            {
                PlayController.bassEng.Play();
            }
        }

        public static void nofityStop_Click(object sender, EventArgs e)
        {
            PlayController.bassEng.Stop();
        }

        public static void nofityPlayPrevent_Click(object sender, EventArgs e)
        {
            PlayController.PlayPrevent();
        }

        public static void nofityPlayNext_Click(object sender, EventArgs e)
        {
            PlayController.PlayNext();
        }

        private static void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Player.MainPage.POINT pit = new Player.MainPage.POINT();
                Player.MainPage.GetCursorPos(out pit);
                if (DiyCM == null || DiyCM.IsLoaded)
                {
                    DiyCM = new DiyContextMenu();
                    DiyCM.WindowStartupLocation = WindowStartupLocation.Manual;
                    DiyCM.Left = pit.X - DiyCM.Width;
                    DiyCM.Top = pit.Y - DiyCM.Height - 10;
                    DiyCM.Show();
                }
                else
                { DiyCM.Close(); DiyCM = null; }
            }
        }
        
    }
}
