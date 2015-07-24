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
using ImPlayer.FM.Views;
using Player.HotKey;
using ImPlayer.DownloadMoudle;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Interop;

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
        private System.Timers.Timer timerClock = new System.Timers.Timer(1000);

        public MainPage(string arge)
        {
            InitializeComponent();
            AppPropertys.mainWindow = this;
            AppPropertys.Initialize();
            PlayController.Initialize();
            LrcController.Initialize(AppPropertys.appSetting.LrcFont,AppPropertys.appSetting.SkinIndex);
            LoadSongList("");
            if (arge != "") { AddFileAndPlay(arge); }
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
            Win32InfoRegister();
            SpectrumAnalyzer.IsEnabled = false;
            InitTaskBar();
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

        #region 反转动画
        /// <summary>
        /// 用来触发面板翻转事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
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
            this.Dispatcher.BeginInvoke(new Action(
                () =>
                {
                    FlipUIElement(mainGrid);
                    FlipUIElement(playlistGrid);
                    if (timerClock != null)
                    {
                        timerClock.Enabled = false;
                        timerClock.Stop();
                        timerClock.Elapsed -= timerClock_Elapsed;
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
        #endregion

        #region 事件响应业务代码块
        /// <summary>
        /// 用来触发面板翻转事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
           // StartCount();
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
                    if (PlayController.bassEng.CanPlay)  
                        PlayController.Play(); 
                    else 
                        PlayController.PlayMusic(); 
                    break;
                case Key.Left:
                    PlayController.PlayPrevent();
                    break;
                case Key.Right:
                    PlayController.PlayNext();
                    break;
                case Key.Up:
                    PlayController.bassEng.Volume += 0.1;
                    break;
                case Key.Down:
                    PlayController.bassEng.Volume -= 0.1;
                    break;
                default: break;
            }
        }



        #endregion

        private void btnMini_MouseDown(object sender,System.Windows.Input.MouseEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
          //  Environment.Exit(0); 强制退出
            System.Windows.Application.Current.Shutdown(-1);
        }

        private void LoadSongList(string path)
        {
            if (path != "")
            {
                XmlListPath = path;
            }
            if (!File.Exists(XmlListPath)) { return; }
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
                    if (CommonProperty.SupportFormat.Contains(fi.Extension.ToLower()))
                    {
                        Song song=PlayController.Songs.FirstOrDefault(s=>s.FileUrl==fi.FullName);
                        if (song==null)
                        {
                            song = new Song(fi.FullName, CommonProperty.getTitleFromPath(fi.FullName));
                            ReadInfoFromFile(song);
                            root.AppendChild(CreateElement(xmlDoc, song));
                            PlayController.Songs.Add(song);
                        }
                        playListBox.ScrollIntoView(song);
                        playListBox.SelectedValue = song;
                        
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
            XmlElement xe = xmlDoc.CreateElement("SongList");
            xe.SetAttribute("copyright", "wrox1226@live.com");
            xmlDoc.AppendChild(xe);
            if (!File.Exists(XmlListPath)) {  xmlDoc.Save(XmlListPath); }
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
            List<CommonFileDialogFilter> cfdf = new List<CommonFileDialogFilter>();
            cfdf.Add(new CommonFileDialogFilter("所有文件", "*.*"));
            cfdf.Add(new CommonFileDialogFilter("MP3", ".MP3"));
            cfdf.Add(new CommonFileDialogFilter("WAV", ".WAV"));
            cfdf.Add(new CommonFileDialogFilter("WMA", ".WMA"));
            cfdf.Add(new CommonFileDialogFilter("APE", ".APE"));
            cfdf.Add(new CommonFileDialogFilter("FLAC", ".FLAC"));
            cfdf.Add(new CommonFileDialogFilter("AAC", ".AAC"));
            cfdf.Add(new CommonFileDialogFilter("M4a", ".M4A"));
            cfdf.Add(new CommonFileDialogFilter("MP4", ".MP4"));
            cfdf.Add(new CommonFileDialogFilter("OGG", ".OGG"));
            List<string> files = FileOpenDialog.ShowDialog("Cup Player", false, cfdf);
            if (files != null)
            {
                XmlDocument xmlDoc = InitXml();
                XmlNode root = xmlDoc.SelectSingleNode("SongList");
                foreach (string file in files)
                {
                    Song s = new Song(file, CommonProperty.getTitleFromPath(file));
                    ReadInfoFromFile(s);
                    root.AppendChild(CreateElement(xmlDoc, s));
                    PlayController.Songs.Add(s);
                }
                xmlDoc.Save(XmlListPath);
            }
        }

        private void folderAdd_Click(object sender, EventArgs e)
        {
            List<string> result = FileOpenDialog.ShowDialog("Cup Player");
            if (result!=null)
            {
                Folder_Open(result[0],FileOpenDialog.isContainSubfolder);
            }
        }

        private void playlistAdd_Click(object sender, EventArgs e)
        {

            List<CommonFileDialogFilter> cfdf = new List<CommonFileDialogFilter>();
            cfdf.Add(new CommonFileDialogFilter("播放列表文件","*.pldb"));
            List<string>  result =  FileOpenDialog.ShowDialog("Cup Player",false,cfdf);
            if (result !=null)
            {
                LoadSongList(result[0]);
            }

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

            PlayController.sl.RemoveNode(filenames.ToArray());
        }

        private void delFromDisk_Click(object sender, EventArgs e)
        {
            int temp = playListBox.SelectedIndex;
            if (temp != -1)
            {
                if (MessageBox.Show("确定从磁盘移除此文件吗？", "警告", MessageBoxButton.YesNo,MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    File.Delete(PlayController.Songs[temp].FileUrl); //必须在RemoveAt前
                    PlayController.Songs.RemoveAt(temp);
                }
            }
        }


        private void Folder_Open(string path, bool IsContainSubfolder) 
        {
            FileInfo[] files=null;

            if (IsContainSubfolder)
            {
                EnumAllFiles(path);
                files = AllFiles.ToArray();
            }
            else
            {
                DirectoryInfo Dir = new DirectoryInfo(path);
                files = Dir.GetFiles();
            }
            
            if (files != null)
            {
                worker.RunWorkerAsync(files);
                Debug.Write("Async Start……");
            }
        }

        #region  多级遍历
        private List<FileInfo> AllFiles = new List<FileInfo>();
        private  void EnumAllFiles(string dirPath)
        {
            List<string> ChildDir = new List<string>();
            DirectoryInfo Dir = new DirectoryInfo(dirPath);
            try
            {
                foreach (DirectoryInfo d in Dir.GetDirectories())
                {
                    EnumAllFiles(Dir + "\\" + d.ToString());
                    ChildDir.Add(Dir + "\\" + d.ToString());
                }
                foreach (FileInfo f in Dir.GetFiles())
                {
                    AllFiles.Add(f);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 读取和保存信息
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
                    SavePicToLocal(tagInfo.PictureGetImage(0), tagInfo.PictureGetType(0), sInfo.PicUrl);
                }
                else
                {
                    SavePicToLocal(sInfo.FileUrl, sInfo.PicUrl);
                }
            }
            catch (Exception ex) { Debug.Write(ex.Message); }
        }
        /// <summary>
        /// Album为空，用名字
        /// </summary>
        /// <param name="Pic"></param>
        /// <param name="AlbumName"></param>
        private void SavePicToLocal(System.Drawing.Image Pic, string PicType, string AlbumName)
        {
            if (!Directory.Exists(CommonProperty.AlbumPicPath))
            {
                Directory.CreateDirectory(CommonProperty.AlbumPicPath);
            }
            Pic.Save(CommonProperty.AlbumPicPath + AlbumName + ".jp", System.Drawing.Imaging.ImageFormat.Png);
        }

        private void SavePicToLocal(string path,string AlbumName)
        {
            try
            {
                TagLib.File file = TagLib.File.Create(path);
                if (file.Tag.Pictures.Length > 0)
                {
                    var bin = file.Tag.Pictures[0].Data.Data;
                    using (MemoryStream ms = new MemoryStream(bin))
                    {
                        System.Drawing.Image Pic = System.Drawing.Image.FromStream(ms).GetThumbnailImage
                        (300, 300, null, IntPtr.Zero);
                        Pic.Save(CommonProperty.AlbumPicPath + AlbumName + ".jp", System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
            catch { Console.WriteLine("reading Pic error"); }

        } 

        #endregion

        #region  异步加载
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            loadProcess.Dispatcher.Invoke(new Action(() => {
                loadProcess.Maximum = 100;
                loadProcess.Visibility = Visibility.Visible;
                loadProcess.Value = e.ProgressPercentage;
                loadProcess.ToolTip = "已完成 "+e.ProgressPercentage.ToString()+"%";
            }));
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                System.Windows.MessageBox.Show("用户取消了操作");
            }
            else
            {
                LoadSongList("");
                loadProcess.Dispatcher.Invoke(new Action(() =>
                {
                    loadProcess.Value = 100;
                    loadProcess.Visibility = Visibility.Collapsed;
                }));
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
                if (CommonProperty.SupportFormat.Contains(file.Extension.ToLower()))
                {
                    string fInfo = file.FullName;
                    Song s = new Song(fInfo, CommonProperty.getTitleFromPath(fInfo));
                    ReadInfoFromFile(s);
                    root.AppendChild(CreateElement(xmlDoc, s));
                    playListBox.Dispatcher.Invoke(new Action(() => PlayController.Songs.Add(s)));

                    count++;
                }
                worker.ReportProgress(count * 100 / files.Count(), CommonProperty.getTitleFromPath(file.FullName));
                System.Threading.Thread.Sleep(10);
            }
            xmlDoc.Save(XmlListPath);
        }

        #endregion

        void mouse_OnMouseActivity(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.None)
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

        private async void Change_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (PlayController.isFM) 
            { 
                FMSongLb.ClearChannels();
                LoadSongList("");
                PlayController.isFM = false;
                return; 
            }
            PlayController.isFM = await FMSongLb.LoadChannels();
            FMSongLb.StartPlayEventHandler += new ImPlayer.FM.Views.MyChannelList.StartPlayDel(InitFm);
        }

        private void InitFm()
        {
            Task task = new Task(LoadFm);
            task.Start();
           
        }

        public void LoadFm()
        {
            List<FMSong> FMlist = MyChannelList.TempSongList;
            if (FMlist == null) { return; }
            Song song = null;
            SongsClear();
            foreach (FMSong fs in FMlist)
            {
                song = new Song { Artist = fs.Artist, Album = fs.AlbumTitle, Title = fs.Title, FileUrl = fs.Url.ToString(), Duration = TimeSpan.FromSeconds(fs.Length), PicUrl = fs.Picture.ToString() };
                watcher_update(song);
            }
            playControl1.btnPlay_Click(null, null);
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PlayController.ShowSetEQ();
        }

        private void Window_Closed(object sender, EventArgs e)
        {              
            new TaskFactory().StartNew(new Action(() => {
                PlayController.bassEng.Stop();
                if (AppPropertys.HotKeys != null)
                {
                    AppPropertys.HotKeys.UnRegister();
                }
                AppPropertys.setFreeNotifyIcon();

                //一般配置
                AppPropertys.appSetting.LrcFont = LrcController.DefaultFont;
                AppPropertys.appSetting.SkinIndex = LrcController.SkinIndex;
                AppPropertys.appSetting.Volume = PlayController.bassEng.Volume;
                AppPropertys.appSetting.Save();

                //下载模块的配置
                SaveDownloadConfig();
            }));
            Console.WriteLine("保存用户配置…");
        }

        public void SaveDownloadConfig()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (ImPlayer.DownloadMoudle.DownloadPage.isDownloadAfterClose)
                {
                    Console.WriteLine("暂停下载任务…");
                    ImPlayer.DownloadMoudle.DownloadPage.PauseAll();
                }
            }));
        }

        #region  歌曲操作
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

        #region 文件类型关联启动
        /// <summary>
        /// 给热键绑定操作
        /// </summary>
        public void AddLogicToHotKeys(HotKeys hotKeys)
        {
            foreach (var keyValue in hotKeys)
            {
                HotKey.HotKey hotKey = keyValue.Value;
                switch (keyValue.Key)
                {
                    case Player.PlayController.Commands.None:
                        break;

                    case Player.PlayController.Commands.PlayPause:
                        hotKey.OnHotKey += delegate { playControl1.btnPlay_Click(null,null); };
                        break;

                    case Player.PlayController.Commands.PreSong:
                        hotKey.OnHotKey += delegate { playControl1.btnPre_Click(null,null);};
                        break;

                    case Player.PlayController.Commands.NextSong:
                        hotKey.OnHotKey += delegate { playControl1.btnNext_Click(null, null); };
                        break;

                    case Player.PlayController.Commands.MuteSwitch:
                        hotKey.OnHotKey += delegate { playControl1.btnMute_Click(null,null); };
                        break;

                    case Player.PlayController.Commands.VolumeUp:
                        hotKey.OnHotKey += delegate { PlayController.bassEng.Volume += 0.1; };
                        break;

                    case Player.PlayController.Commands.VolumeDown:
                        hotKey.OnHotKey += delegate { PlayController.bassEng.Volume -= 0.1; };
                        break;

                    case Player.PlayController.Commands.SetEQ:
                        hotKey.OnHotKey += delegate { Grid_MouseLeftButtonDown(null, null); };
                        break;

                    case Player.PlayController.Commands.Exit:
                        hotKey.OnHotKey += delegate { btnClose_MouseDown(null,null); };
                        break;
                }
            }
        }
        
        public struct COPYDATASTRUCT
            {
                public IntPtr dwData;
                public int cbData;
                [MarshalAs(UnmanagedType.LPStr)]
                public string lpData;
            }

        public void Win32InfoRegister()
        {
            (PresentationSource.FromVisual(this) as System.Windows.Interop.HwndSource)
                .AddHook(new System.Windows.Interop.HwndSourceHook(WndProc));
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x004A)
            {
                COPYDATASTRUCT copydata = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));
                string dicom_file = copydata.lpData;
                FileInfo fi = new FileInfo(dicom_file);
                if (fi.Extension != "pldb"&&dicom_file != null)
                {
                    AddFileAndPlay(dicom_file);
                }
                handled = true;
            }
            return hwnd;
        }

        public void AddFileAndPlay(string path)
        {
            Song song = PlayController.Songs.FirstOrDefault(s => s.FileUrl.Replace(" ","") == path);
            if (song == null)
            {
                XmlDocument xmlDoc = InitXml();
                XmlNode root = xmlDoc.SelectSingleNode("SongList");
                song = new Song(path, CommonProperty.getTitleFromPath(path));
                ReadInfoFromFile(song);
                root.AppendChild(CreateElement(xmlDoc, song));
                PlayController.Songs.Add(song);
                xmlDoc.Save(XmlListPath);
            }
            playListBox.ScrollIntoView(song);
            playListBox.SelectedValue = song;
            PlayController.PlayMusic(playListBox.SelectedIndex); 
        }
        #endregion

        #region 设置保存
        public void SaveConfig(HotKeys HotKeys)
        {
            
            #region 热键设置
            AppPropertys.HotKeys.UnRegister();
            AppPropertys.HotKeys = HotKeys;
            AddLogicToHotKeys(HotKeys);
            AppPropertys.HotKeys.Register(this);
            AppPropertys.HotKeys.Save();
            #endregion
        }
        #endregion

        #region 搜索
        private void btnLocalSerach_Click(object sender, RoutedEventArgs e)
        {
            string content = SearchTeacBox.Text;
            if (string.IsNullOrEmpty(content)) { return; }
            Song song = PlayController.Songs.FirstOrDefault(s =>s.FileName.Contains(content)||s.Artist.Contains(content));
            if (song != null) {  playListBox.ScrollIntoView(song);playListBox.SelectedValue=song; }
        }
        MainWindow SearchWindow = null;
        private void btnInternetSearch_Click(object sender, RoutedEventArgs e)
        {
            if (SearchWindow == null || !SearchWindow.IsLoaded)
            {
                SearchWindow = new MainWindow(AppPropertys.appSetting.DownloadFolder);
            }
            SearchWindow.ShowWindow(SearchTeacBox.Text);
        }
        #endregion

        private void btn_Play(object sender, RoutedEventArgs e)
        {
            playListBox_MouseDoubleClick(null,null);
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartCount();
        }

        private void CurrentShow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartCount();
        }

        #region 任务栏增强

        Microsoft.WindowsAPICodePack.Taskbar.ThumbnailToolBarButton buttonPlayPause = null;
        public void InitTaskBar()
        {
            Microsoft.WindowsAPICodePack.Taskbar.ThumbnailToolBarButton buttonPrevious = new Microsoft.WindowsAPICodePack.Taskbar.ThumbnailToolBarButton(Properties.Resources.Button_First, "上一首");  
           // Microsoft.WindowsAPICodePack.Taskbar.TabbedThumbnail newPreview = new Microsoft.WindowsAPICodePack.Taskbar.TabbedThumbnail(this, img, new Vector(10,10));
           buttonPrevious.Click+=buttonPrevious_Click;
           buttonPlayPause=new Microsoft.WindowsAPICodePack.Taskbar.ThumbnailToolBarButton(Properties.Resources.Button_Play,"播放");
           buttonPlayPause.Click +=buttonPlayPause_Click;
           Microsoft.WindowsAPICodePack.Taskbar.ThumbnailToolBarButton buttonNext = new Microsoft.WindowsAPICodePack.Taskbar.ThumbnailToolBarButton(Properties.Resources.Button_Last, "下一首");
           buttonNext.Click+=buttonNext_Click;

           Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.ThumbnailToolBars.AddButtons((new WindowInteropHelper(Application.Current.MainWindow)).Handle, buttonPrevious, buttonPlayPause, buttonNext);
        }


        private void buttonPrevious_Click(object sender,  Microsoft.WindowsAPICodePack.Taskbar.ThumbnailButtonClickedEventArgs e)
        {
            playControl1.btnPre_Click(null,null);
        }
        private void buttonPlayPause_Click(object sender, Microsoft.WindowsAPICodePack.Taskbar.ThumbnailButtonClickedEventArgs e)
        {
            if (PlayController.bassEng.IsPlaying)
            {
                buttonPlayPause.Icon = Properties.Resources.Button_Pause;
            }
            else
            {
                buttonPlayPause.Icon = Properties.Resources.Button_Play;
            }
            playControl1.btnPlay_Click(null, null);
        }
        private void buttonNext_Click(object sender, Microsoft.WindowsAPICodePack.Taskbar.ThumbnailButtonClickedEventArgs e)
        {
            playControl1.btnNext_Click(null,null);
        }
        #endregion
  
    }
        
}
