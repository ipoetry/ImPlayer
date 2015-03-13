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
            btnPlay.Style = PlayController.bassEng.IsPlaying ? (Style)this.FindResource("pause") : (Style)this.FindResource("play");
            btnMute.Style = PlayController.bassEng.IsMuted ? (Style)this.FindResource("Mute") : (Style)this.FindResource("notMute");

        }

        private void volumeBG_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition((Rectangle)sender);
           // volumeMask.Width = p.X;
            Canvas.SetLeft(thumb, p.X);
        }
        private void volumeMask_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition((Rectangle)sender);
           // volumeMask.Width = p.X;
            Canvas.SetLeft(thumb, p.X);
        }
        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (Canvas.GetLeft(thumb) + e.HorizontalChange >= 0 && Canvas.GetLeft(thumb) + e.HorizontalChange <= 100)
            { 
               // volumeMask.Width = Canvas.GetLeft(thumb) + e.HorizontalChange;
                double pos = (Canvas.GetLeft(thumb) + e.HorizontalChange) / volumeBG.Width * 110;
                PlayController.bassEng.Volume = pos;
                Canvas.SetLeft(thumb, pos);

            }
        }

        private void btnPre_Click(object sender, RoutedEventArgs e)
        {
            PlayController.PlayPrevent();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (PlayController.bassEng.IsPlaying)
            {
                PlayController.Pause();
                btnPlay.Style = (Style)this.FindResource("play");
            }
            else
            {
                PlayController.PlayMusic();
                btnPlay.Style = (Style)this.FindResource("pause");
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            PlayController.PlayNext();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            AppPropertys.setFreeNotifyIcon();
            Environment.Exit(0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            
            if (!AppPropertys.isLrcShow)
            {
                LrcController.lrcWindow.Show();
                AppPropertys.isLrcShow=true;
            }
            else
            {
                LrcController.lrcWindow.Hide();
                AppPropertys.isLrcShow = false;
            }
        }

        private void btnMute_Click(object sender, RoutedEventArgs e)
        {
            PlayController.setMute();
            btnMute.Style = PlayController.bassEng.IsMuted ? (Style)this.FindResource("Mute") : (Style)this.FindResource("notMute");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas.SetLeft(thumb, PlayController.bassEng.Volume);
            BindingOperations.SetBinding(volumeMask, Rectangle.WidthProperty,
               new Binding
               {
                   Source = thumb,
                   Path = new PropertyPath(Canvas.LeftProperty)
               });
        }
    }
}
