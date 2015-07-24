using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyDownloader.Core;
using MyDownloader.Core.Common;
using ImPlayer.Toast;

namespace ImPlayer.DownloadMoudle
{
    /// <summary>
    /// DownloadListView.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadListView : UserControl
    {

        delegate void ActionDownloader(Downloader d, System.Windows.Controls.ListViewItem item);

        Hashtable mapDownloadToObj = new Hashtable();//映射对象到对象
        Hashtable mapObjToDownload = new Hashtable();//对象到映射对象

        Hashtable mapObjToCurrentState = new Hashtable();
        System.Windows.Controls.ListViewItem lastSelection = null;


        public ObservableCollection<DownLoadFileInfo> downloadingList = new ObservableCollection<DownLoadFileInfo>();
        public ObservableCollection<DownLoadFileInfo> DownloadingList
        {
            get { return downloadingList; }
            set { downloadingList = value; }
        }

        [DllImport("user32.dll")]
        public static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

        public DownloadListView()
        {
            InitializeComponent();
            DownloadManager.Instance.DownloadAdded += new EventHandler<DownloaderEventArgs>(Instance_DownloadAdded);
            DownloadManager.Instance.DownloadRemoved += new EventHandler<DownloaderEventArgs>(Instance_DownloadRemoved);

            for (int i = 0; i < DownloadManager.Instance.Downloads.Count; i++)
            {
                AddDownload(DownloadManager.Instance.Downloads[i]);
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DownloadManager.Instance.BeginAddBatchDownloads += new EventHandler(Instance_BeginAddBatchDownloads);
            DownloadManager.Instance.EndAddBatchDownloads += new EventHandler(Instance_EndAddBatchDownloads);
            DownloadManager.Instance.DownloadEnded += new EventHandler<DownloaderEventArgs>(Instance_EndDownloads);
            dlIngList.Items.Clear();
            dlIngList.ItemsSource = DownloadingList;
        }

        
        void Instance_EndAddBatchDownloads(object sender, EventArgs e)
        {
            //this.BeginInvoke((MethodInvoker)lvwDownloads.EndUpdate);

        }

        void Instance_EndDownloads(object sender, EventArgs e)
        {

        }

        void Instance_BeginAddBatchDownloads(object sender, EventArgs e)
        {
            //  this.BeginInvoke((MethodInvoker)lvwDownloads.BeginUpdate);
        }


        #region
        public void StartSelections()
        {
            DownloadsAction(
                delegate(Downloader d, ListViewItem item)
                {
                    d.Start();
                }
            );
        }

        public void Pause()
        {
            DownloadsAction(
                delegate(Downloader d, ListViewItem item)
                {
                    d.Pause();
                }
            );
        }

        public void PauseAll()
        {
            DownloadManager.Instance.PauseAll();
            UpdateList();
        }

        public void RemoveSelections()
        {
            if (MessageBox.Show("确定移除?", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    dlIngList.SelectionChanged -= lvwDownloads_ItemSelectionChanged;

                    DownloadManager.Instance.DownloadRemoved -= new EventHandler<DownloaderEventArgs>(Instance_DownloadRemoved);

                    DownloadsAction(
                        delegate(Downloader d, ListViewItem item)
                        {
                            DownloadingList.Remove(mapDownloadToObj[d] as DownLoadFileInfo);
                            DownloadManager.Instance.RemoveDownload(d);
                        }
                    );
                }
                finally
                {
                    dlIngList.SelectionChanged += lvwDownloads_ItemSelectionChanged;
                    lvwDownloads_ItemSelectionChanged(null, null);

                    DownloadManager.Instance.DownloadRemoved += new EventHandler<DownloaderEventArgs>(Instance_DownloadRemoved);
                }
            }
        }

        public void SelectAll()
        {
            using (DownloadManager.Instance.LockDownloadList(false))
            {

                try
                {
                    dlIngList.SelectionChanged -= lvwDownloads_ItemSelectionChanged;
                    dlIngList.SelectAll();
                }
                finally
                {
                    dlIngList.SelectionChanged += lvwDownloads_ItemSelectionChanged;
                    lvwDownloads_ItemSelectionChanged(null, null);
                }
            }
        }

        public void RemoveCompleted()
        {

            try
            {
                DownloadManager.Instance.ClearEnded();
                UpdateList();
            }
            catch
            {

            }
        }

        public void AddDownloadURLs(ResourceLocation[] args, int segments, string path, int nrOfSubfolders)
        {
            if (args == null) return;
            if (path == null)
            {
                path = PathHelper.GetWithBackslash(MyDownloader.Core.Settings.Default.DownloadFolder);
            }
            else
            {
                path = PathHelper.GetWithBackslash(path);
            }

            try
            {
                DownloadManager.Instance.OnBeginAddBatchDownloads();

                foreach (ResourceLocation rl in args)
                {
                    Uri uri = new Uri(rl.URL);

                    string fileName = uri.Segments[uri.Segments.Length - 1];
                    fileName = HttpUtility.UrlDecode(fileName).Replace("/", "\\");

                    DownloadManager.Instance.Add(
                        rl,
                        null,
                        path + fileName,
                        segments,
                        false);
                }
            }
            finally
            {
                DownloadManager.Instance.OnEndAddBatchDownloads();
            }
        }

        private void lvwDownloads_ItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSegments();
            UpdateUI();
        }

        public void UpdateUI()
        {

            bool isSelected = dlIngList.SelectedItems.Count > 0;

            isSelected = dlIngList.SelectedItems.Count == 1;

            OnSelectionChange();
        }

        public event EventHandler SelectionChange;

        protected virtual void OnSelectionChange()
        {
            if (SelectionChange != null)
            {
                SelectionChange(this, EventArgs.Empty);
            }
        }

        private void copyURLToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(SelectedDownloaders[0].ResourceLocation.URL);
        }

        private void showInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", String.Format("/select,{0}", SelectedDownloaders[0].LocalFile));
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(SelectedDownloaders[0].LocalFile);
            }
            catch (Exception)
            {
            }
        }

        public int SelectedCount
        {
            get
            {
                return dlIngList.SelectedItems.Count;
            }
        }

        public Downloader[] SelectedDownloaders
        {
            get
            {
                if (dlIngList.SelectedItems.Count > 0)
                {
                    Downloader[] downloaders = new Downloader[dlIngList.SelectedItems.Count];
                    for (int i = 0; i < downloaders.Length; i++)
                    {
                        downloaders[i] = mapObjToDownload[dlIngList.SelectedItems[i] as DownLoadFileInfo] as Downloader;
                    }
                    return downloaders;
                }

                return null;
            }
        }

        void Instance_DownloadRemoved(object sender, DownloaderEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                DownLoadFileInfo item = mapDownloadToObj[e.Downloader] as DownLoadFileInfo;
                if (item != null)
                {
                    DownloadingList.Remove(item);
                    mapDownloadToObj[e.Downloader] = null;
                    mapObjToDownload[item] = null;
                }
            }
            ));
        }

        void Instance_DownloadAdded(object sender, DownloaderEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => { AddDownload(e.Downloader); }));
        }

        private void AddDownload(Downloader d)
        {

            string ext = System.IO.Path.GetExtension(d.LocalFile);

            DownLoadFileInfo dInfo = new DownLoadFileInfo();
            dInfo.FileName = System.IO.Path.GetFileName(d.LocalFile);
            dInfo.FileSize = ByteFormatter.ToString(d.FileSize);
            dInfo.DownloadProcess = d.Progress;
            dInfo.FileLink = d.ResourceLocation.URL;
            dInfo.FileAlbum = d.ResourceLocation.Password;
            dInfo.DownloadState = d.State;
            mapDownloadToObj[d] = dInfo;
            mapObjToDownload[dInfo] = d;
            mapObjToCurrentState[dInfo] = d.State;
            DownloadingList.Add(dInfo);
        }

        private static string GetResumeStr(Downloader d)
        {
            return (d.RemoteFileInfo != null && d.RemoteFileInfo.AcceptRanges ? "Yes" : "No");
        }

        public void UpdateList()
        {
            for (int i = 0; i < DownloadingList.Count; i++)
            {

                DownLoadFileInfo di = DownloadingList[i];
                Downloader d = mapObjToDownload[di] as Downloader;
                if (d == null) return;

                DownloaderState state = DownloaderState.Working;


                if (state != d.State ||
                    state == DownloaderState.Working ||
                    state == DownloaderState.WaitingForReconnect)
                {
                    di.DownloadProcess = d.Progress;
                    if ((DownloaderState)mapObjToCurrentState[di] != d.State)
                    {
                        di.DownloadState = d.State;
                        mapObjToCurrentState[di] = d.State;
                    }
                }
            }
            UpdateSegments();
        }

        private void UpdateSegments()
        {
            try
            {

                if (dlIngList.SelectedItems.Count == 1)
                {
                    DownLoadFileInfo newSelection = dlIngList.SelectedItems[0] as DownLoadFileInfo;
                    Downloader d = mapObjToDownload[newSelection] as Downloader;

                }
                else
                {
                    lastSelection = null;
                }
            }
            finally
            {

            }
        }

        private void DownloadsAction(ActionDownloader action)
        {
            if (dlIngList.SelectedItems.Count > 0)
            {
                try
                {
                    dlIngList.SelectionChanged -= new SelectionChangedEventHandler(lvwDownloads_ItemSelectionChanged);

                    for (int i = dlIngList.SelectedItems.Count - 1; i >= 0; i--)
                    {
                        DownLoadFileInfo item = dlIngList.SelectedItems[i] as DownLoadFileInfo;
                        action((Downloader)mapObjToDownload[item], null);
                    }

                    dlIngList.SelectionChanged += new SelectionChangedEventHandler(lvwDownloads_ItemSelectionChanged);
                    lvwDownloads_ItemSelectionChanged(null, null);
                }
                finally
                {
                    UpdateSegments();
                }
            }
        }


        private void newDownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //  NewFileDownload(null, true);
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartSelections();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pause();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelections();
        }

        private void lvwDownloads_DoubleClick(object sender, EventArgs e)
        {
            UpdateUI();
            openFileToolStripMenuItem_Click(sender, e);
        }
        #endregion

        private async void bgBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DownLoadFileInfo dl = dlIngList.SelectedItem as DownLoadFileInfo;
                if (dl == null || !await PopupTip.CheckNetWork()) { return; }
                System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;
                string content = btn.Content.ToString();
                if (content == "开始" || content == "继续")
                {
                    StartSelections();
                }
                else if (content == "播放")
                {
                    System.Diagnostics.Process.Start(MainWindow.DownloadFolder + dl.FileName);
                }
                else
                {
                    Pause();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        private void dlBtn_Click(object sender, RoutedEventArgs e)
        {
            DownLoadFileInfo dl = dlIngList.SelectedItem as DownLoadFileInfo;
            if (dl == null) { return; }
            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;
            if (btn.Content.ToString() == "移除")
            {
                RemoveSelections();
            }
            else
            {
                try
                {
                    System.IO.File.Delete(dl.FileLink);
                }
                catch (System.IO.IOException ex)
                { 
                    Console.WriteLine(ex);
                }
            }
        }

        private void dlIngList_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            object item = GetElementFromPoint((ItemsControl)sender, e.GetPosition((ItemsControl)sender));
            if (item != null)
            {
                object obj = this.TryFindResource("MyCM");
                if (obj != null)
                {
                    ContextMenu cm = (ContextMenu)obj;
                    dlIngList.ContextMenu = cm;
                }
            }
            else
            {
                dlIngList.ContextMenu = null;
            }
        }


        public void StartAll()
        {
            DownloadManager.Instance.StartAll();
            UpdateList();
        }

        public void RemoveAll()
        {
            DownloadManager.Instance.RemoveAll();
            UpdateList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemsControl"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private object GetElementFromPoint(ItemsControl itemsControl, Point point)
        {
            UIElement element = itemsControl.InputHitTest(point) as UIElement;
            while (element != null)
            {
                if (element == itemsControl)
                    return null;
                object item = itemsControl.ItemContainerGenerator.ItemFromContainer(element);
                if (!item.Equals(DependencyProperty.UnsetValue))
                    return item;
                element = (UIElement)VisualTreeHelper.GetParent(element);
            }
            return null;
        }
    }
}
