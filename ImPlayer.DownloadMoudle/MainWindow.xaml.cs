using ImPlayer.DownloadMoudle.BaiduMusic;
using MyDownloader.Extension.PersistedList;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImPlayer.DownloadMoudle
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(string downloadFolder)
        {
            InitializeComponent();
            this.DataContext = this;
            DownloadFolder = downloadFolder;
        }

        public static string DownloadFolder = string.Empty;
        QualitySelect qs = null;
        string searchStr = string.Empty;
        int cp = 0;

        public int currentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); cp = value; }
        }
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("currentPage", typeof(int), typeof(MainWindow), new UIPropertyMetadata(1));

        public int totalPage
        {
            get { return (int)GetValue(TotalPageProperty); }
            set { SetValue(TotalPageProperty, value); }
        }
        public static readonly DependencyProperty TotalPageProperty =
            DependencyProperty.Register("totalPage", typeof(int), typeof(MainWindow), new UIPropertyMetadata(1));


        private void downloadBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn != null)
            {
                string ID = btn.Tag.ToString();
                TaskAsyncHelper.RunAsync(new Action(() => ShowLlink(ID)), TaskCompleted);
            }
        }

        public async void ShowLlink(string Id)
        {
            SearchSongResultById dsr = await BaiduMusicOp.GetMusicUrl(Id);
            this.Dispatcher.Invoke(new Action(() =>
            {
                qs = new QualitySelect(dsr);
                qs.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                qs.ShowDialog();
            }));
        }

        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            //Button btn = (Button)sender;
            //SearchSongResultById dsr = await BaiduMusicOp.GetMusicUrl(btn.Tag.ToString());
            //System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory+"//Player.exe",dsr.Bitrate[0].file_link);
            Win8Toast.PopupTip.ShowPopUp("暂时不提供……");
        }

        List<BdSong> Songs = null;
        private async void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            searchStr = SearchText.Text;
            if (searchStr == ""|| !await Win8Toast.PopupTip.CheckNetWork()) return;
            Tip.Visibility = Visibility.Collapsed;
            this.dd.ItemsSource = null;
            searchBtn.IsEnabled = false;
            TaskAsyncHelper.RunAsync(new Action(() => startAsyncSearch(0)), TaskCompleted);
        }

        private async void startAsyncSearch(int pageNum)
        {
            nvpb.Dispatcher.Invoke(new Action(() => { nvpb.Visibility = Visibility.Visible; }));
            SearchSongResultByKey bsr = await BaiduMusicOp.SearchMusicByKey(searchStr, pageNum);
            if (bsr == null || bsr.Song_List == null)
            {
                Tip.Dispatcher.Invoke(new Action(() => { Tip.Visibility = Visibility.Visible; }));
                return;
            }
            Songs = bsr.Song_List;
            this.Dispatcher.Invoke(new Action(() =>
            {
                totalPage = bsr.Pages.Total / 10;
                this.dd.ItemsSource = Songs;
            }));
        }

        private void TaskCompleted()
        {
            nvpb.Dispatcher.Invoke(new Action(() => { nvpb.Visibility = Visibility.Collapsed; }));
            searchBtn.Dispatcher.Invoke(new Action(() => { searchBtn.IsEnabled = true; }));
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Save()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (Common.downloadPage != null && Common.downloadPage.IsLoaded)
                    Common.downloadPage.xD.PauseAll();
                ((PersistedListExtension)MyApp.Instance.GetExtensionByType(typeof(PersistedListExtension))).PersistList(null);
                Console.WriteLine("下载模块相关配置本地化……");
            }));
        }

        private void datagrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int temp = int.Parse(tbxPageNum.Text);
            if (temp >= 1 && temp <= totalPage)
            {
                currentPage = temp;
                TaskAsyncHelper.RunAsync(new Action(() => startAsyncSearch(cp)), TaskCompleted);
            }
        }

        private void btnPre_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage = currentPage - 1;
                TaskAsyncHelper.RunAsync(new Action(() => startAsyncSearch(cp)), TaskCompleted);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPage)
            {
                currentPage = currentPage + 1;
                TaskAsyncHelper.RunAsync(new Action(() => startAsyncSearch(cp)), TaskCompleted);
            }
        }

        #region 判断输入框是否输入的额数字
        private void tbxPageNum_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!isNumberic(text))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }

        public static bool isNumberic(string _string)
        {
            if (string.IsNullOrEmpty(_string))
                return false;
            foreach (char c in _string)
            {
                if (!char.IsDigit(c))
                    //if(c<'0' c="">'9')//最好的方法,在下面测试数据中再加一个0，然后这种方法效率会搞10毫秒左右
                    return false;
            }
            return true;
        }

        private void tbxPageNum_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void tbxPageNum_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumberic(e.Text))
            {
                e.Handled = true;
            }
            else
                e.Handled = false;
        }
        #endregion

        private void DownLoadPage_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(()=>{
                if (Common.downloadPage == null || !Common.downloadPage.IsLoaded)
                Common.downloadPage = new DownloadPage();
                Common.downloadPage.Show();
            })); 
        }

        private void Label_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = this.WindowState != WindowState.Minimized ? WindowState.Minimized : WindowState.Normal;
        }
        public void ShowWindow(string key)
        {
            this.SearchText.Text = key;
            this.Show();
            this.searchBtn_Click(null,null);
        }
    }
}
