using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using Lyrics;
using Un4seen.Bass.AddOn.Tags;
using WPFSoundVisualizationLib;
using ImPlayer.FM.Models;
using ImPalyer.FM.Views;

namespace Player
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Window
    {
        #region
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);
        MouseHook mouse = new MouseHook();

        #endregion

        [DllImport("user32.dll")]
        public static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

        BackgroundWorker worker = null;
        string XmlListPath = AppDomain.CurrentDomain.BaseDirectory + "PlayList.pldb";
        private PBar notifyForm = new PBar(); 
        private System.Timers.Timer timerClock = new System.Timers.Timer(1000);
        private int tick = 2;

        public MainPage()
        {
            InitializeComponent();
            AppPropertys.mainWindow = this;
            AppPropertys.Initialize();
            PlayController.Initialize();
            LrcController.Initialize();
            LoadSongList("");
            
        }

        /// <summary>
        /// 显示在最上层
        /// </summary>
        public void ShowFront()
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mouse.OnMouseActivity += new System.Windows.Forms.MouseEventHandler(mouse_OnMouseActivity);
            mouse.Start();

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            if (PlayController.bassEng != null)
            {
                SpectrumAnalyzer.RegisterSoundPlayer(PlayController.bassEng);
            }
        }

        private void playListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PlayController.PlayIndex != playListBox.SelectedIndex)
            {
                PlayController.PlayIndex = playListBox.SelectedIndex;
                PlayController.PlayMusic();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //拖动窗体
            try
            {
                this.DragMove();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 用来触发面板翻转事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            StartCount();
        }
        /// <summary>
        /// 开始计数。
        /// </summary>
        private void StartCount()
        {
            timerClock.Enabled = true;
            timerClock.Elapsed += timerClock_Elapsed;
            timerClock.Start();
        }

        void timerClock_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(
                () =>
                {
                    if (tick > 0)
                    {
                        tick--;
                        if (tick == 0)
                        {
                            //释放定时器资源。
                            //进行面板的翻转。
                            FlipUIElement(mainGrid);
                            FlipUIElement(playlistGrid);
                            tick = 2;
                            //停止定时器
                            if (timerClock != null)
                            {
                                timerClock.Enabled = false;
                                timerClock.Stop();
                                timerClock.Elapsed -= timerClock_Elapsed;
                            }
                        }
                    }
                }));
        }

        /// <summary>
        /// 播放器按钮等以动画方式显示和消失。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="enter"></param>
        private void ShowUIElementWithAnimation(object sender, bool enter)
        {
           // if (false)//isFirstTimeRunning == 
            {
                Grid grid = sender as Grid;
                if (grid != null)
                {
                    string str = grid.Tag.ToString();

                    if (str == "0")
                    {
                        double from = 0, to = 0;
                        double from1 = 0, to1 = 0;
                        if (enter)
                        {
                            from = 0;
                            to = 1;
                            from1 = 1;
                            to1 = 0.4;
                        }
                        else
                        {
                            from = 1;
                            to = 0;
                            from1 = 0.4;
                            to1 = 1;
                        }
                        DoubleAnimation da = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(500));
                        DoubleAnimation da1 = new DoubleAnimation(from1, to1, TimeSpan.FromMilliseconds(500));
                    }
                    else if (str == "1")
                    {
                        double from = 0, to = 0;
                        if (enter)
                        {
                            from = 0;
                            to = 1;
                        }
                        else
                        {
                            from = 1;
                            to = 0;
                        }
                        DoubleAnimation da = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(500));
                    }
                }
            }
        }

        /// <summary>
        /// 动画翻转面板。
        /// </summary>
        /// <param name="grid"></param>
        private void FlipUIElement(Grid grid)
        {
            //翻转动作开始
            #region 翻转动画代码段
            Storyboard story = null;
            if (grid.Name == mainGrid.Name)
            {
                grid.Visibility = System.Windows.Visibility.Visible;
                //如果是主面板。
                if (grid.Opacity == 1)
                {
                    //此时应当翻转该面板并且置透明度为0
                    story = BeginStoryBoardNow(grid, 1, -1, 1, 0);
                }
                else
                {
                    story = BeginStoryBoardNow(grid, -1, 1, 0, 1);
                }
            }
            else
            {
                grid.Visibility = System.Windows.Visibility.Visible;
                //如果是歌曲播放列表。
                if (grid.Opacity == 1)
                {
                    story = BeginStoryBoardNow(grid, 1, -1, 1, 0);
                }
                else
                {
                    story = BeginStoryBoardNow(grid, -1, 1, 0, 1);
                }
            }
            story.Begin(grid);
            #endregion
        }

        private Storyboard BeginStoryBoardNow(Grid grid, int scaleXF, int scaleXT, int opacityF, int opacityT)
        {
            Storyboard story = new Storyboard();
            story.Completed += story_Completed;
            ScaleTransform scaleTransform = new ScaleTransform(1, 1);
            TransformGroup group = new TransformGroup();
            group.Children.Add(scaleTransform);
            grid.RenderTransform = group;
            DoubleAnimation daKeyF = new DoubleAnimation(scaleXF, scaleXT, TimeSpan.FromSeconds(1));
            DoubleAnimation daOp = new DoubleAnimation(opacityF, opacityT, TimeSpan.FromSeconds(1));
            Storyboard.SetTargetName(daKeyF, grid.Name);
            Storyboard.SetTargetProperty(daKeyF,
                new PropertyPath("(Grid.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetName(daOp, grid.Name);
            Storyboard.SetTargetProperty(daOp, new PropertyPath(Grid.OpacityProperty));
            story.Children.Add(daKeyF);
            story.Children.Add(daOp);
            return story;
        }

        /// <summary>
        /// 当动画结束时，改变透明度。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void story_Completed(object sender, EventArgs e)
        {
            if (mainGrid.Opacity == 0)
            {
                mainGrid.Visibility = System.Windows.Visibility.Hidden;
                playlistGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                mainGrid.Visibility = System.Windows.Visibility.Visible;
                playlistGrid.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        #region 事件响应业务代码块


        /// <summary>
        /// 用来触发面板翻转事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        private void Rectangle_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }


        /// <summary>
        /// 当在区域二中移动鼠标时，则将控件设置为可见，否则当静止等状态时隐藏控件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ShowUIElementWithAnimation(sender, true);
        }

        /// <summary>
        /// 移除区域２时则隐藏控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ShowUIElementWithAnimation(sender, false);
        }

        /// <summary>
        /// 将设置信息反映回来。
        /// </summary>
        void sForm_SettingValueChangedEventHandler(object sender, EventArgs e)
        {
            //BorderImage.Opacity = e.OpacityValue;
            //borderOpacity = BorderImage.Opacity;
            //showSpectrum = (bool)e.ShowSpectrum;
            //autoLoadLyricFile = (bool)e.AutoLoadLyricFile;
            //saveConfig = (bool)e.SaveConfig;
            //saveSongList = (bool)e.SaveSongList;
            //rememberExitPosition = (bool)e.RememberExitPosition;
            //dskLrcFontSize = e.DesktopLyricFontSize;
            //nowPlayingSong.Foreground = e.SongNameForeColor;
            //preLyricTextBlock.Foreground = e.UnplayedLyricForeColor;
            //nowLyricTextBlock.Foreground = e.PlayedLyricForeColor;
            //dskLrcPlayedForecolor = e.DesktopPlayedLyricForeColor;
            //dskLrcUnplayedForecolor = e.DesktopUnplayedLyricForeColor;
            ////主动改变桌面歌词
            //if (dskLrc != null && dskLrc.IsVisible)
            //{
            //    dskLrc.ChangeFontSize(dskLrcFontSize);
            //    dskLrc.ChangeFontForeColor(dskLrcPlayedForecolor, dskLrcUnplayedForecolor);
            //}
            ////判断编码方式改变没有，如果改变。则重新加载歌词
            //if (encoding != e.FileEncoding)
            //{
            //    encoding = e.FileEncoding;
            //    lyric = null;
            //    InitializeMyLyric(lyricFilePath);
            //}
        }


        /// <summary>
        /// 应用程序响应键盘按下事件，并判断做出相关动作。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Key downKey = e.Key;
            switch (downKey)
            {
                case Key.Space:
                    //空格键按下时，就暂停或播放歌曲。
                 //   PlayerButtonClickDown(mediaControlBt);
                    break;
                case Key.Left:
                    //当左方向键按下时，则切换到上一曲。
                  //  PlayerButtonClickDown(previousSongBt);
                    break;
                case Key.Right:
                    //当右方向键按下时，则切换到下一曲。
                  //  PlayerButtonClickDown(nextSongBt);
                    break;
                case Key.Up:
                    //当上方向键按下时，则增加音量。
                   // volumeValuePrb.Value += 5;
                    break;
                case Key.Down:
                    //当下方向键按下时，则减小音量。
                 //   volumeValuePrb.Value -= 5;
                    break;
                default: break;
            }
        }



        #endregion

        private void SongAdd_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void SongRemove_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnMini_MouseDown(object sender,System.Windows.Input.MouseEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AppPropertys.setFreeNotifyIcon();
            Environment.Exit(0);
        }


        private void LoadSongList(string path)
        {
            if (path != "")
            {
                XmlListPath = path;
            }
            
            PlayController.sl.LoadXml(XmlListPath);
            PlayController.Songs = PlayController.sl.getICSongList();
            playListBox.Dispatcher.Invoke(new Action(() => playListBox.ItemsSource = PlayController.Songs));
        }

        private void playListBox_Drop(object sender, System.Windows.DragEventArgs e)
        {
            XmlDocument xmlDoc = InitXml();
            XmlNode root = xmlDoc.SelectSingleNode("SongList");
            if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop))
            {
                string[] FullName = ((string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop));
                foreach (string t in FullName)
                {
                    FileInfo fi = new FileInfo(t);
                    if (Common.Common.SupportFormat.Contains(fi.Extension.ToLower()))
                    {

                        if (PlayController.Songs.Contains(new Song(fi.FullName, Common.Common.getTitleFromPath(fi.FullName))))
                        {
                            playListBox.SelectedItem = playListBox.Items[PlayController.Songs.IndexOf(new Song(fi.FullName, Common.Common.getTitleFromPath(fi.FullName)))];
                        }
                        else
                        {
                            Song s = new Song(fi.FullName, Common.Common.getTitleFromPath(fi.FullName));
                            ReadInfoFromFile(s);
                            root.AppendChild(CreateElement(xmlDoc, s));
                            PlayController.Songs.Add(s);
                        }
                    }
                    else
                    {
                        Process.Start(fi.FullName);
                    }
                }
                xmlDoc.Save(XmlListPath);
            }
        }

        private void playListBox_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop))
                e.Effects = System.Windows.DragDropEffects.Copy;
            else e.Effects = System.Windows.DragDropEffects.None;
        }

        private void btn_Play(object sender, EventArgs e)
        {
            PlayController.PlayMusic();
        }

        private void btn_OpenPath(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Explorer.exe", "/select," + PlayController.Songs[playListBox.SelectedIndex].FileUrl);
        }

        private XmlDocument InitXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(XmlListPath);
            return xmlDoc;
        }

        private XmlElement CreateElement(XmlDocument xmlDoc, Song s)
        {
            XmlElement xe = xmlDoc.CreateElement("Song");
            xe.SetAttribute("url", s.FileUrl);
            xe.SetAttribute("artist", s.Artist);
            xe.SetAttribute("album", s.Album);
            xe.SetAttribute("title", s.Title);
            xe.SetAttribute("size", s.Size);
            xe.SetAttribute("pic", s.PicUrl);
            xe.SetAttribute("duration", s.Duration.TotalSeconds.ToString("0.00"));
            xe.InnerText = s.FileName;
            return xe;
        }

        private void fileAdd_Click(object sender, EventArgs e)
        {
            string[] files = File_Open("所有文件|*.*|MP3|*.MP3|WAV|*.WAV|WMA|*.WMA|APE|*.APE|FLAC|*.FLAC|ACC|*.ACC|M4a|*.M4a|OGG|*.OGG", true);
            if (files != null)
            {
                XmlDocument xmlDoc = InitXml();
                XmlNode root = xmlDoc.SelectSingleNode("SongList");
                foreach (string file in files)
                {
                    Song s = new Song(file, Common.Common.getTitleFromPath(file));
                    ReadInfoFromFile(s);
                    root.AppendChild(CreateElement(xmlDoc, s));
                    PlayController.Songs.Add(s);
                }
                xmlDoc.Save(XmlListPath);
            }
        }

        private void folderAdd_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Folder_Open(fbd.SelectedPath);
            }
        }

        private void playlistAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Multiselect = true;
            OFD.Filter = "播放列表文件|*.pldb";
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadSongList(OFD.FileName);
            }
            OFD.Dispose();
        }

        private void delFromList_Click(object sender, EventArgs e)
        {
            List<string> filenames = new List<string>();
            var list = playListBox.SelectedItems;
            for (int i = 0; i < list.Count; i++)
            {
                Song song = list[i] as Song;
                if (song != null)
                    PlayController.Songs.Remove(song);
                filenames.Add(song.FileName);
            }
            //foreach (var lbi in list)
            //{
            //    Song song = lbi as Song;
            //    if (song != null)
            //        PlayController.Songs.Remove(song);
            //    filenames.Add(song.FileName);
            //}
            PlayController.sl.RemoveNode(filenames.ToArray());
        }

        private void delFromDisk_Click(object sender, EventArgs e)
        {
            int temp = playListBox.SelectedIndex;
            if (temp != -1)
            {
                if (System.Windows.Forms.MessageBox.Show("确定从磁盘移除此文件吗？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
                {
                    File.Delete(PlayController.Songs[temp].FileUrl); //必须在RemoveAt前
                    PlayController.Songs.RemoveAt(temp);
                }
            }
        }

        private string[] File_Open(string filter, bool multiselect)
        {
            string[] files = null;
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Multiselect = multiselect;
            OFD.Filter = filter;
            OFD.Title = "ImPlayer";
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                files = OFD.FileNames;
            }
            OFD.Dispose();
            return files;
        }

        private void Folder_Open(string path)  //没实现多级遍历 
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            //FileInfo[] files = dir.GetFiles("*.mp3", SearchOption.AllDirectories);
            FileInfo[] files = dir.GetFiles();
            if (files != null)
            {
                //   notifyForm.ShowDialog();
                worker.RunWorkerAsync(files);
                Debug.Write("Async Start……");
            }
        }

        private void ReadInfoFromFile(Song sInfo)
        {
            TAG_INFO tagInfo = new TAG_INFO(sInfo.FileUrl);
            try
            {
                tagInfo = BassTags.BASS_TAG_GetFromFile(sInfo.FileUrl);
                sInfo.Size = string.Format("{0:F}M", new FileInfo(sInfo.FileUrl).Length / Math.Pow(1024, 2));
                sInfo.Album = tagInfo.album;
                sInfo.Artist = tagInfo.artist;
                sInfo.Title = tagInfo.title;
                sInfo.PublicDate = tagInfo.year;
                sInfo.Company = tagInfo.publisher;
                sInfo.PicUrl = sInfo.Album == "" ? sInfo.FileName : sInfo.Album;
                sInfo.Duration = TimeSpan.FromSeconds(tagInfo.duration);
                if (tagInfo.PictureGetImage(0) != null)
                {
                    SavePicToDisk(tagInfo.PictureGetImage(0), tagInfo.PictureGetType(0), sInfo.PicUrl);
                }
            }
            catch (Exception ex) { Debug.Write(ex.Message); }
        }
        /// <summary>
        /// Album为空，用名字
        /// </summary>
        /// <param name="Pic"></param>
        /// <param name="Album"></param>
        private void SavePicToDisk(System.Drawing.Image Pic, string PicType, string Album)
        {
            string dir = Common.Common.GetRunDir() + @"Album\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            Pic.Save(dir + Album + ".png", System.Drawing.Imaging.ImageFormat.Png);
        }

        #region  异步加载
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // notifyForm.SetNotifyInfo(e.ProgressPercentage, e.UserState.ToString());  
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                //  GC.Collect();
                //  this.Refresh();
                //  Application.DoEvents()
                System.Windows.MessageBox.Show("用户取消了操作");
            }
            else
            {
                //notifyForm.Close();
                System.Windows.MessageBox.Show("加载完成");
            }

        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            FileInfo[] files = (FileInfo[])e.Argument;
            XmlDocument xmlDoc = InitXml();
            XmlNode root = xmlDoc.SelectSingleNode("SongList");
            int count = 0;
            foreach (FileInfo file in files)
            {
                if (Common.Common.SupportFormat.Contains(file.Extension.ToLower()))
                {
                    string fInfo = file.FullName;
                    Song s = new Song(fInfo, Common.Common.getTitleFromPath(fInfo));
                    ReadInfoFromFile(s);
                    root.AppendChild(CreateElement(xmlDoc, s));
                    playListBox.Dispatcher.Invoke(new Action(() => PlayController.Songs.Add(s)));

                    count++;
                }
                worker.ReportProgress(count * 100 / files.Count(), Common.Common.getTitleFromPath(file.FullName));
                System.Threading.Thread.Sleep(0);
            }
            xmlDoc.Save(XmlListPath);
        }

      //  private ManualResetEvent manualReset = new ManualResetEvent(true);
        #endregion
        void mouse_OnMouseActivity(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.None)
            {

                if (AppPropertys.DiyCM != null)
                {
                    if (!(AppPropertys.DiyCM.Left < e.X && e.X < AppPropertys.DiyCM.Left + AppPropertys.DiyCM.Width && AppPropertys.DiyCM.Top < e.Y && e.Y < AppPropertys.DiyCM.Top + AppPropertys.DiyCM.Height))
                    {
                        AppPropertys.DiyCM.Close();
                        AppPropertys.DiyCM = null;
                    }
                }
            }
        }

        private void NotifyIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {

        }

        private void Change_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (PlayController.isFM) { FMSongLb.ClearChannels(); LoadSongList(""); PlayController.isFM = false; return; }
            PlayController.isFM =  FMSongLb.LoadChannels();
            FMSongLb.StartPlayEventHandler += new ImPalyer.FM.Views.MyChannelList.StartPlayDel(FMLoad);
        }

        private void FMLoad()
        {
            List<FMSong> FMlist = MyChannelList.TempSongList;
            Song song=null;
            SongsClear();
        //    PlayController.Songs.Clear();
            foreach(FMSong fs in FMlist)
            {
               song=new Song{Artist=fs.Artist,Album=fs.AlbumTitle, Title=fs.Title, FileUrl=fs.Url.ToString(),Duration=TimeSpan.FromSeconds(fs.Length),PicUrl=fs.Picture.ToString()};
               watcher_update(song);   
            }
            playControl1.btnPlay_Click(null,null);
        }

        #region  添加歌曲
        public  delegate void AddSsongDelegate(Song newSong);
        public  void watcher_update(Song newSong)
        {
            try
            {
                AddSsongDelegate addSsongDelegate = new AddSsongDelegate(update);
                this.Dispatcher.Invoke(addSsongDelegate,newSong);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private  void update(Song song)
        {
           PlayController.Songs.Add(song);
        }
        #endregion

        #region
        public delegate void ClearSongs();
        public void SongsClear()
        {
            try
            {
                ClearSongs clearSongs = new ClearSongs(Clear);
                this.Dispatcher.Invoke(clearSongs);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void Clear()
        {
            PlayController.Songs.Clear();
        }
        #endregion

        private void test(object sender, MouseButtonEventArgs e)
        {
            PlayController.ShowSetEQ();
        }
    }
}
