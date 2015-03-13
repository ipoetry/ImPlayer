using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using Lyrics;
namespace Player.Controls
{
    /// <summary>
    /// LrcControl.xaml 的交互逻辑
    /// </summary>
    public partial class LrcControl : UserControl
    {
        public LrcControl()
        {
            InitializeComponent();
        }
        public class TimeChangeEventArgs : EventArgs
        {
            private double time = 0;
            public TimeChangeEventArgs(double i)
            {
                this.time = i;
            }

            public double Time
            {
                get {return time;}
            }
        }
        public delegate void TimeChangedHandle(object sender, TimeChangeEventArgs e);

        public void showLrc(double time)
        {

            if (LrcController.Lyric != null)
                if (LrcController.Lyric.isLoad)
                    for (int i = 0; i < LrcController.Lyric.LrcList.Count; i++)
                    {
                        if (time >= LrcController.Lyric.LrcList[i].StartTime && time < (int)LrcController.Lyric.LrcList[i].EndTime)
                        {
                            index = i;
                            setLrc(i);
                            return;
                        }
                    }
        }

        private int index = -1;
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
                setLrc(index);
            }
        }

        private void setLrc(int i)
        {
            showBox.SelectedIndex = i + (int)showBox.ActualHeight / 56;
        }
        private string formatTime(double seconds)
        {
            int m = (int)seconds / 60;
            int s = (int)seconds % 60;
            string t = (m > 9 ? m.ToString() : "0" + m.ToString()) + ":" + (s > 9 ? s.ToString() : "0" + s.ToString());
            return t;
        }

        public void drawLrc()
        {
            if (LrcController.Lyric.isLoad)
            {
                showBox.Items.Clear();
                showText.Visibility = Visibility.Hidden;
                for (int i = 0; i < (int)showBox.ActualHeight / 56; i++)
                    showBox.Items.Add(new OneLineLrc());
                for (int i = 0; i < LrcController.Lyric.LrcList.Count; i++)
                {
                    string text = System.Text.RegularExpressions.Regex.Replace(LrcController.Lyric.LrcList[i].LrcText, ",", "");
                    OneLineLrc t = new OneLineLrc();
                    t.LrcText = text;
                    t.StartTimeStr = formatTime(LrcController.Lyric.LrcList[i].StartTime / 1000);
                    t.StartTime = LrcController.Lyric.LrcList[i].StartTime / 1000;
                    showBox.Items.Add(t);
                }
                for (int i = 0; i < (int)showBox.ActualHeight / 56; i++)
                    showBox.Items.Add(new OneLineLrc());
                showBox.ScrollIntoView(showBox.Items[0]);
            }
        }

        private void showBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            drawLrc();
        }

        private void lrcText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            PlayController.DT.Stop();
            PlayController.bassEng.ChannelPosition =TimeSpan.FromSeconds(Convert.ToDouble(tb.Tag));
            PlayController.DT.Start();
        }

    }
}
