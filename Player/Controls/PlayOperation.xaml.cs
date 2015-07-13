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
using Lyrics;

namespace Player
{
    /// <summary>
    /// PlayOperation.xaml 的交互逻辑
    /// </summary>
    public partial class PlayOperation : UserControl
    {
        public PlayOperation()
        {
            InitializeComponent();
        }

        private void btnPlayMode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {         
            int mode = Convert.ToInt16(btnPlayMode.Tag);
            mode++;
            mode = mode > 3 ? 1 : mode;
            switch (mode)
            {
                case 1:
                    btnPlayMode.Tag = 1;
                    btnPlayMode.ToolTip = "循环播放";
                    btnPlayMode.Style = (Style)this.FindResource("circle");
                    PlayController.PlayMode = 1;
                    break;
                case 2:
                    btnPlayMode.Tag = 2;
                    btnPlayMode.ToolTip = "随机播放";
                    btnPlayMode.Style = (Style)this.FindResource("random");
                    PlayController.PlayMode = 2;
                    break;
                case 3:
                    btnPlayMode.Tag = 3;
                    btnPlayMode.ToolTip = "单曲播放";
                    btnPlayMode.Style = (Style)this.FindResource("single");
                    PlayController.PlayMode = 3;
                    break;
            }
        }

        public void btnPre_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(PlayController.PlayPrevent);
            task.Start();
        }

        public void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(PlayController.BtnPlayOperation);
            task.Start();           
        }

        public void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(PlayController.PlayNext);
            task.Start();
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void ChanelLength_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition((Rectangle)sender);
            double pos = p.X / ChanelLength.Width * (PlayController.bassEng.ChannelLength.TotalSeconds <= 0 ? 300 : PlayController.bassEng.ChannelLength.TotalSeconds);
            Canvas.SetLeft(thumb2, p.X);
        }

        private void CurLen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition((Rectangle)sender);
            double pos = p.X / ChanelLength.Width * (PlayController.bassEng.ChannelLength.TotalSeconds <= 0 ? 300 : PlayController.bassEng.ChannelLength.TotalSeconds);
            Canvas.SetLeft(thumb2, p.X);
        }

        private void Thumb2_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (Canvas.GetLeft(thumb2) + e.HorizontalChange >= 0 && Canvas.GetLeft(thumb2) + e.HorizontalChange <= 500)
            {
                double pos = (Canvas.GetLeft(thumb2) + e.HorizontalChange) / ChanelLength.Width * (PlayController.bassEng.ChannelLength.TotalSeconds <= 0 ? 300 : PlayController.bassEng.ChannelLength.TotalSeconds);
                PlayController.bassEng.ChannelPosition = TimeSpan.FromSeconds(pos);
                Canvas.SetLeft(thumb2, Canvas.GetLeft(thumb2) + e.HorizontalChange);
            }
        }

        public void btnMute_Click(object sender, RoutedEventArgs e)
        {
            PlayController.setMute();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            thumb2.DataContext = Player.PlayController.bassEng;
            BindingOperations.SetBinding(CurLen, Rectangle.WidthProperty,
                new Binding
                {
                    Source = thumb2,
                    Path = new PropertyPath(Canvas.LeftProperty),
                    // Mode=BindingMode.TwoWay
                });
            BindingOperations.SetBinding(soundSlider, Slider.ValueProperty,
               new Binding
               {
                   Source = PlayController.bassEng,
                   Path = new PropertyPath("Volume"),
                   Mode=BindingMode.TwoWay
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

        private void btnLrcShow_MouseDown(object sender, MouseButtonEventArgs e)
        {
           AppPropertys.SetLrcShow();
        }

        private void btnPicShow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Task task = new Task(playPicAnom);
            task.Start();
        }
        public void playPicAnom()
        {
            AppPropertys.mainWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (AppPropertys.mainWindow.isPPTPlaying||PlayController.CurrentSong == null)
                {
                    AppPropertys.mainWindow.StopPlayPPT();
                    return;
                }
                else
                {
                    AppPropertys.mainWindow.PlayPPT(PlayController.CurrentSong);
                }
            }));   
        }

    }
}
