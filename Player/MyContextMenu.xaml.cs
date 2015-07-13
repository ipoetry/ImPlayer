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
using Lyrics;
using Player.Setting;

namespace Player
{
    /// <summary>
    /// DiyContextMenu.xaml 的交互逻辑
    /// </summary>
    public partial class DiyContextMenu : Window
    {
        public DiyContextMenu()
        {
            InitializeComponent();
        }

        private void btnPre_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => {
                PlayController.PlayPrevent();
            }));     
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                PlayController.BtnPlayOperation();
            }));
            
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                PlayController.PlayNext();
            }));
            
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            AppPropertys.setFreeNotifyIcon();
            //Environment.Exit(0);
            Application.Current.Shutdown(-1);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AppPropertys.SetLrcShow();
        }

        private void btnMute_Click(object sender, RoutedEventArgs e)
        {
            PlayController.setMute();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BindingOperations.SetBinding(soundSlider, Slider.ValueProperty,
                         new Binding
                         {
                             Source = PlayController.bassEng,
                             Path = new PropertyPath("Volume"),
                             Mode = BindingMode.TwoWay
                         });
            BindingOperations.SetBinding(btnPlay, RadioButton.IsCheckedProperty,
               new Binding
               {
                   Source = PlayController.bassEng,
                   Path = new PropertyPath("IsPlaying"),
                   Mode = BindingMode.OneWay
               });
            BindingOperations.SetBinding(btnMute, RadioButton.IsCheckedProperty,
              new Binding
              {
                  Source = PlayController.bassEng,
                  Path = new PropertyPath("IsMuted"),
                  Mode = BindingMode.OneWay
              });
        }
        SettingPage setingPage = null;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Task task = new Task(OpenSettingWindow);
            task.Start();
        }

        public void OpenSettingWindow()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (setingPage == null || !setingPage.IsLoaded)
                    setingPage = new SettingPage(AppPropertys.HotKeys);
                setingPage.SettingReloadHandler += new SettingReloadDelegate(AppPropertys.mainWindow.SaveConfig);
                setingPage.Show();
            }));
        }
        
    }
}
