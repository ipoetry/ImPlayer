using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lyrics
{
    /// <summary>
    /// LrcWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LrcWindow : Window
    {
        private string _lrcText;
        private int drawIndex = -1;
        private double h = SystemParameters.PrimaryScreenHeight;
        private int lrcIndex = -1;
        private List<int> lrcWidths = new List<int>();
        private double w = SystemParameters.PrimaryScreenWidth;
        public LrcWindow()
        {
            InitializeComponent();
            LrcController.lrcWindow = this;
        }
        private void DrowLrcText(int index, double time)
        {
            if ((index > -1) && (index < LrcController.Lyric.LrcList.Count))
            {
                if (this.drawIndex != index)
                {
                    this.drawIndex = index;
                    if ((this.drawIndex != 0) && (this.imgMask.Width < base.Width))
                    {
                        this.imgMask.Width = base.Width;
                        Thread.Sleep(100);
                    }
                    this.lrcWidths.Clear();
                    this._lrcText = LrcController.Lyric.LrcList[this.drawIndex].LrcText;
                    this.lrcWidths = LrcController.GetWidth(this._lrcText);
                    this.imgMask.Width = 0.0;
                    this.showLrcText();
                    this.showMaskLrcText();
                }
                else
                {
                    string[] strArray = LrcController.Lyric.LrcList[this.drawIndex].MidTime.Split(new char[] { ',' });
                    int num = 0;
                    double num2 = 0.0;
                    num = 0;
                    while (num < (strArray.Length - 1))
                    {
                        if (num < this.lrcWidths.Count)
                        {
                            num2 += (double)this.lrcWidths[num];
                        }
                        double num3 = Convert.ToDouble(strArray[num]);
                        double num4 = Convert.ToDouble(strArray[num + 1]);
                        if ((time >= num3) && (time <= num4))
                        {
                            this.imgMask.Width = num2 - (((num4 - time) / (num4 - num3)) * ((double)this.lrcWidths[num]));
                            return;
                        }
                        num++;
                    }
                    if (time >= Convert.ToDouble(strArray[num]))
                    {
                        if ((LrcController.Lyric.LrcList[this.drawIndex].EndTime - Convert.ToDouble(strArray[num])) > 0.0)
                        {
                            this.imgMask.Width = num2 + (((time - Convert.ToDouble(strArray[num])) / (LrcController.Lyric.LrcList[this.drawIndex].EndTime - Convert.ToDouble(strArray[num]))) * ((double)this.lrcWidths[num]));
                        }
                        else
                        {
                            this.imgMask.Width = num2 + ((double)this.lrcWidths[num]);
                        }
                    }
                }
            }
        }

        public void Init(string lrcText)
        {
            this._lrcText = lrcText;
            this.imgMask.Width = 0.0;
            this.showLrcText();
        }
        private void imgBG_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

        public void showLrc(double time)
        {
            if (LrcController.Lyric.isLoad)
            {
                for (int i = 0; i < LrcController.Lyric.LrcList.Count; i++)
                {
                    if ((time >= LrcController.Lyric.LrcList[i].StartTime) && (time < LrcController.Lyric.LrcList[i].EndTime))
                    {
                        this.lrcIndex = i;
                        break;
                    }
                }
                this.DrowLrcText(this.lrcIndex, time);
            }
        }

        private void showLrcText()
        {
            Bitmap bmp = LrcController.getBGBMP(this._lrcText);
            base.Height = bmp.Height;
            base.Width = bmp.Width;
            base.Left = (this.w - base.Width) / 2.0;
            base.Top = (this.h - base.Height) - 50.0;
            this.imgBG.Source = LrcController.getImageSource(bmp);
            this.imgBG.Width = base.Width;
            this.imgBG.Height = base.Height;
            bmp.Dispose();
        }

        private void showMaskLrcText()
        {
            Bitmap bmp = LrcController.getMaskBMP(this._lrcText);
            this.imgMask.Source = LrcController.getImageSource(bmp);
            bmp.Dispose();
        }

        public void showText(string text)
        {
            this.imgMask.Width = 0.0;
            this._lrcText = text;
            this.showLrcText();
        }
        public void Update()
        {
            this.lrcWidths = LrcController.GetWidth(this._lrcText);
            this.imgMask.Width = 0.0;
            this.showLrcText();
            this.showMaskLrcText();
            LrcController.lrcToolBar.Top = (base.Top - LrcController.lrcToolBar.Height) + 5.0;
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            LrcController.lrcToolBar.dt.Stop();
            LrcController.lrcToolBar.Show();
            LrcController.lrcToolBar.Top = (base.Top - LrcController.lrcToolBar.Height) + 5.0;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            LrcController.lrcToolBar.dt.Start();
        }
    }
}
