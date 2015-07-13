using Microsoft.Win32;
using Player.FileTypeAssocion;
using Player.HotKey;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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

namespace Player.Setting
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public delegate void SettingReloadDelegate(HotKeys HotKeys);
    public partial class SettingPage : Window
    {
        internal HotKeys HotKeys { get; private set; }
        public event SettingReloadDelegate SettingReloadHandler;
        private ObservableCollection<DataItem> items = new ObservableCollection<DataItem>();

        public ObservableCollection<DataItem> Items
        {
            get { return items; }
            set { items = value; }
        }
        public SettingPage(HotKeys hotKeys)
        {
            #region FileType Init
            FileRegisterLoad();
            #endregion
            InitializeComponent();
            #region 读取热键
            HotKeys = hotKeys;
            if (hotKeys != null)
            {
                foreach (var child in this.yy.Children)
                {
                    if (child is HotKeySettingControl)
                    {
                        HotKeySettingControl setting = child as HotKeySettingControl;
                        if (hotKeys.ContainsKey(setting.Command))
                            setting.HotKey = hotKeys[setting.Command];
                    }
                }
            }
            #endregion
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            #region 保存热键
            HotKeys.Clear();
            foreach (var child in this.yy.Children)
            {
                if (child is HotKeySettingControl)
                {
                    HotKeySettingControl setting = child as HotKeySettingControl;
                    if (setting.HotKey != null)
                        HotKeys.Add(setting.Command, setting.HotKey);
                }
            }
            #endregion

            #region 类型关联
            List<string> fileTypes=new List<string>();
            //foreach (DataItem d in Items)
            //{
            //    if (d.IsEnabled) { fileTypes.Add(d.Name.Substring(d.Name.IndexOf('(').TrimEnd(')'))); }
            //}
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            fileTypes.AddRange(Items.Where(item => item.IsEnabled).Select(ss => ss.Name.Substring(ss.Name.IndexOf('(')+1).Trim(')')));
            TypeRegsiter.Regsiter(dir + "\\Player.exe", dir + "Resouce\\Symbian_Anna.dll", fileTypes);
          //  TypeRegsiter.Regsiter(fileTypes);  //TODO
            FileRegisterSave();
            #endregion
            this.Close();

            if (SettingReloadHandler != null)
            {
                SettingReloadHandler(HotKeys);
            }
        }

        private void playControlKey_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            SelectAll.IsChecked = true;
        }

        private void btnNoSelectAll_Click(object sender, RoutedEventArgs e)
        {
            SelectAll.IsChecked = false;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            defaultFolder.Text = AppPropertys.appSetting.DownloadFolder;
            currentVersion.Text = "当前版本："+GetCurrentVersion();
        }

        public static string GetCurrentVersion()
        {
           return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        #region

        /// <summary>
        /// 加载文件类型关联设置
        /// </summary>
         void FileRegisterLoad()
        {
            try
            {
                using (FileStream stream = File.OpenRead(System.IO.Path.Combine(AppPropertys.dataFolder, "FileTypeRegister.dat")))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    Items = (ObservableCollection<DataItem>)formatter.Deserialize(stream);
                }
            }
            catch
            {
                Items.Add(new DataItem() { Name = "AAC 音频文件(.aac)" });
                Items.Add(new DataItem() { Name = "MP4 音频文件(.m4a)" });
                Items.Add(new DataItem() { Name = "MP3 音频文件(.mp3)" });
                Items.Add(new DataItem() { Name = "Money's Audio 音频文件(.ape)" });
                Items.Add(new DataItem() { Name = "FLAC音频文件(.flac)" });
                Items.Add(new DataItem() { Name = "Windows Media 音频文件(.wma)" });
                Items.Add(new DataItem() { Name = "Wave Audio 音频文件(.wav)" });
                Items.Add(new DataItem() { Name = "Voribs/OGG 音频文件(.ogg)" });
            }
        }
        /// <summary>
         /// 保存文件类型关联设置
        /// </summary>
         void FileRegisterSave()
        {
            try
            {
                if (!Directory.Exists(AppPropertys.dataFolder))
                    Directory.CreateDirectory(AppPropertys.dataFolder);
                using (FileStream stream = File.OpenWrite(System.IO.Path.Combine(AppPropertys.dataFolder, "FileTypeRegister.dat")))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, Items);
                }
            }
            catch { }
        }

        #endregion

         private void Button_Click(object sender, RoutedEventArgs e)
         {
             this.Close();
         }

         private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
         {
             this.Close();
         }

         private void isAutoStart_Checked(object sender, RoutedEventArgs e)
         {
             if (!Win8Toast.SystemHepler.IsWin8OrHeigher())
             {
                 string path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                 RegistryKey rKey = Registry.LocalMachine;
                 RegistryKey rKey2 = rKey.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                 if ((bool)isAutoStart.IsChecked)
                 {
                     rKey2.SetValue("Cup Player", path);
                 }
                 else
                 {
                     rKey2.DeleteValue("Cup Player", false);
                 }
                 rKey2.Close();
                 rKey.Close();
             }
             else 
             {
                 if (!File.Exists(Win8Toast.ToastTip.shortcutPath))
                     Win8Toast.ToastTip.TryCreateShortcut();
                     File.Copy(Win8Toast.ToastTip.shortcutPath,Win8Toast.SystemHepler.autoStart+"Cup Player.lnk");
             }
         }

         private void Button_Click_1(object sender, RoutedEventArgs e)
         {
             Task task = new Task(SetPath);
             task.Start();
         }

         public void SetPath()
         {
             this.Dispatcher.BeginInvoke(new Action(() => {
                 string path = FileOpenDialog.ShowDialog();
                 if (path != string.Empty)
                 {
                     AppPropertys.appSetting.DownloadFolder = path + "\\";
                     defaultFolder.Text = AppPropertys.appSetting.DownloadFolder;
                 }
             }));
            
         }
    }
}
