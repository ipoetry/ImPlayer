using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Lyrics
{
    /// <summary>
    /// LrcToolBar.xaml 的交互逻辑
    /// </summary>
    public partial class LrcToolBar : Window
    {
        public DispatcherTimer dt = new DispatcherTimer();
        public LrcToolBar()
        {
            InitializeComponent();
            base.Left = (SystemParameters.PrimaryScreenWidth - base.Width) / 2.0;
            base.Top = (SystemParameters.PrimaryScreenHeight - base.Height) - 50.0;
            this.dt.Interval = TimeSpan.FromMilliseconds(3000.0);
            this.dt.Tick += new EventHandler(this.dt_Tick);
        }

        private void btnPlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (btnPlay.ToolTip.ToString() != "播放")
            {
                LrcController.SetButtonChanged(sender, 2);
                LrcController.SetPlay();
            }
            else
            {
                LrcController.SetButtonChanged(sender, 3);
                LrcController.SetPause();
            }
        }

        private void dt_Tick(object sender, EventArgs e)
        {
            base.Hide();
            this.dt.Stop();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LrcController.SetButtonChanged(sender, 0);
        }

        private void skin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LrcController.SkinIndex = this.skin.SelectedIndex;
            LrcController.ChangeColor(this.skin.SelectedIndex);
            return;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LrcController.UpdateSize(4f);
            return;
        }

        private void TextBlock_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            LrcController.UpdateSize(-4f);
            return;
        }

        private void TextBlock_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            LrcController.lrcWindow.Hide();
            LrcController.SetButtonChanged(sender, -1);
            base.Hide();
        }

 


    }
}
