using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using Lyrics;
using FormControl;
using Un4seen.Bass.AddOn.Tags;
using System.ComponentModel;
using System.Runtime.InteropServices;
namespace Player
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : DiyWindow
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
        BackgroundWorker worker=null;
        string XmlListPath = AppDomain.CurrentDomain.BaseDirectory + "PlayList.pldb";
        SongList sl = new SongList();
        private PBar notifyForm = new PBar(); 
        public MainWindow()
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
         
        private void DiyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            mouse.OnMouseActivity += new System.Windows.Forms.MouseEventHandler(mouse_OnMouseActivity);
            mouse.Start(); 

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
        }

        private void LoadSongList(string path)
        {
            if (path != "")
            {
                XmlListPath = path;
            }
            sl.LoadXml(XmlListPath);
            PlayController.Songs = sl.getICSongList();
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
                        
                        if (PlayController.Songs.Contains(new Song(fi.FullName,Common.Common.getTitleFromPath(fi.FullName))))
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

        private void btn_Play(object sender,EventArgs e)
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

        private XmlElement CreateElement(XmlDocument xmlDoc,Song s)
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
                    root.AppendChild(CreateElement(xmlDoc,s));
                    PlayController.Songs.Add(s);  
                }
                xmlDoc.Save(XmlListPath);
            }
        }

        private void folderAdd_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() ==System.Windows.Forms.DialogResult.OK)
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
            sl.RemoveNode(filenames.ToArray());
        }

        private void delFromDisk_Click(object sender, EventArgs e)
        {
            int temp =playListBox.SelectedIndex;
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
            if (OFD.ShowDialog()==System.Windows.Forms.DialogResult.OK)
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
                sInfo.Size = string.Format("{0:F}M",new FileInfo(sInfo.FileUrl).Length / Math.Pow(1024,2));
                sInfo.Album = tagInfo.album;
                sInfo.Artist = tagInfo.artist;
                sInfo.Title = tagInfo.title;
                sInfo.PublicDate = tagInfo.year;
                sInfo.Company = tagInfo.publisher;
                sInfo.PicUrl = sInfo.Album == "" ? sInfo.FileName : sInfo.Album;
                sInfo.Duration = TimeSpan.FromSeconds(tagInfo.duration);
                if (tagInfo.PictureGetImage(0)!= null)
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
        private void SavePicToDisk(System.Drawing.Image Pic,string PicType,string Album)
        {
            string dir = Common.Common.GetRunDir() + @"Album\";
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir); }
                Pic.Save(dir + Album+".png", System.Drawing.Imaging.ImageFormat.Png);
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
                        root.AppendChild(CreateElement(xmlDoc,s));
                        playListBox.Dispatcher.Invoke(new Action(()=>PlayController.Songs.Add(s)));

                        count++;
                    }
                    worker.ReportProgress(count * 100 / files.Count(), Common.Common.getTitleFromPath(file.FullName));
                    System.Threading.Thread.Sleep(0);
                }
            xmlDoc.Save(XmlListPath);
        }

        private ManualResetEvent manualReset = new ManualResetEvent(true);
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

    }
}
