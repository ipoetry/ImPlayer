using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ImPlayer.DownloadMoudle
{
    /// <summary>
    /// DownloadPage.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadPage : Window
    {
        private DispatcherTimer dispatcherTimer = null;
        public static bool isDownloading = false;
        public DownloadPage()
        {
            InitializeComponent();
            MyApp.Instance.InitExtensions();
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(OnTimedEvent);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }
        private void OnTimedEvent(object sender, EventArgs e)
        {
            xD.UpdateList();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }


        private void AllBeginBtn_Click(object sender, RoutedEventArgs e)
        {
            xD.StartAll();
        }

        private void AllPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            xD.PauseAll();
        }

        private void AllRemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            xD.RemoveAll();
        }

        public static void PauseAll()
        {
            MyDownloader.Core.DownloadManager.Instance.PauseAll();
            ((MyDownloader.Extension.PersistedList.PersistedListExtension)MyApp.Instance.GetExtensionByType(typeof(MyDownloader.Extension.PersistedList.PersistedListExtension))).PersistList(null);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show("关闭窗体后继续下载？", "提示", MessageBoxButton.YesNo))
            {
                isDownloading = true;
            }
            else
            {
                PauseAll();
                isDownloading = false;
            }
        }
    }
}
