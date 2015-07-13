using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using playerSetting;
using RegisterHotKey;

namespace PlayerSetting
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Items.Add(new DataItem() { Name = "AAC 音频文件(.aac)" });
            Items.Add(new DataItem() { Name = "MP4 音频文件(.m4a)" });
            Items.Add(new DataItem() { Name = "MP3 音频文件(.mp3)" });
            Items.Add(new DataItem() { Name = "Money's Audio 音频文件(.ape)" });
            Items.Add(new DataItem() { Name = "FLAC音频文件(.flac)" });
            Items.Add(new DataItem() { Name = "Windows Media 音频文件(.wma)" });
            Items.Add(new DataItem() { Name = "Wave Audio 音频文件(.wav)" });
            Items.Add(new DataItem() { Name = "Voribs/OGG 音频文件(.ogg)" });
            
            InitializeComponent();
        }
        private ObservableCollection<DataItem> items = new ObservableCollection<DataItem>();

        public ObservableCollection<DataItem> Items
        {
            get { return items; }
            set { items = value; }
        }
        KeyboardHook kh;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kh = new KeyboardHook();
            kh.SetHook();
            kh.OnKeyDownEvent += kh_OnKeyDownEvent;
            IMEHelper.IMEShield(this,1);

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }

        HotKey.KeyFlags kf;
        List<string> IMEkeys = new List<string>();
        
        void kh_OnKeyDownEvent(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            string keyString = e.KeyData.ToString();
            string showResult = "";
            kf = HotKey.KeyFlags.None;
            IMEkeys = keyString.Split(',').ToList();
            if (IMEkeys.Contains("Control") || keyString.Contains("Control"))
            { 
                showResult += "Ctrl+"; 
                IMEkeys.Remove("Control"); kf = kf|HotKey.KeyFlags.MOD_CONTROL;
            }
            if (IMEkeys.Contains("Shift") || keyString.Contains("Shift"))
            {
                showResult += "Shift+"; IMEkeys.Remove("Shift");
                kf = kf | HotKey.KeyFlags.MOD_SHIFT;
            }
            if (IMEkeys.Contains("Alt") || keyString.Contains("Alt"))
            {
                showResult += "Alt+"; IMEkeys.Remove("Alt");
                kf = kf | HotKey.KeyFlags.MOD_ALT;
            }
            showResult = IMEkeys[0] == "Back" ? "无" : showResult + IMEkeys[0];
            TextBox  tb = Keyboard.FocusedElement as TextBox;
            if (tb != null)
            {
                tb.Text = showResult;
            }
        
        }

        private void playControlKey_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        HotKey playHotKey = null;
        string lastInput = string.Empty;
        private void nextControlKey_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IMEkeys.Count()<=0) return;
            TextBox t = (TextBox)sender;
            if (lastInput != string.Empty && lastInput == t.Text.Trim()) { return; }  //如果内容没有变化，则返回
            lastInput = t.Text;
            Window win = this;
            switch (t.Name)
            {
                case "playControlKey": 
                    if(playHotKey!=null)  //如果事件已被关联，则解除关联，并释放之前的对象
                    {
                        playHotKey.OnHotKey -= playHotKey_OnHotKey;
                        GC.SuppressFinalize(playHotKey);
                    }
                    playHotKey = new HotKey(win,kf, (System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), IMEkeys[0]));
                    {
                        try
                        {
                            playHotKey.RegisterHotKey();
                            playHotKey.OnHotKey += playHotKey_OnHotKey;
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                    }
                    break;
                    
            }
            
        }

        private void playHotKey_OnHotKey()
        {
            MessageBox.Show("呵呵，注册成功。");
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            xx.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectAll.IsChecked = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SelectAll.IsChecked = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
