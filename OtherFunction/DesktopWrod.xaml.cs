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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OtherFunction
{
    /// <summary>
    /// DesktopLyric.xaml 的交互逻辑
    /// </summary>
    public partial class DesktopWord : Window
    {

        private int fontsize = 24;
        private Color fontColor= Colors.LightBlue;

        private DesktopWord()
        {
            InitializeComponent();
            ResetWordForecolor();
        }

        public static void Show(string line0,string line1)
        {
            var dw=new DesktopWord();
            dw.Show();
            dw.ChangeWord(line0, line1);
            dw.ShowWordWithAnimation(1, 1);
        }

        private ImageSource GetImageSource(string newSource)
        {
            BitmapImage bit = new BitmapImage();
            bit.BeginInit();
            bit.UriSource = new Uri(newSource, UriKind.RelativeOrAbsolute);
            bit.EndInit();
            return bit;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ChangeUIElementOpacity(TextBlock tb, int from, int to)
        {
            DoubleAnimation da = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(888));
            tb.BeginAnimation(OpacityProperty, da);
        }

        private void ChangeUIElementFontSize(TextBlock tb, int from, int to)
        {
            DoubleAnimation daF = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(222));
            tb.BeginAnimation(FontSizeProperty, daF);
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            GradientStopCollection gsc = new GradientStopCollection();
            gsc.Add(new GradientStop(Color.FromArgb(0xA0, 0x3D, 
                0xB4, 0x83),0));
            gsc.Add(new GradientStop(Color.FromArgb(0xA0, 0x8E,
                0xCD, 0xEA), 1));
            gsc.Add(new GradientStop(Color.FromArgb(0xA0, 0xB9,
                0xDE, 0xDA), 0.50));
            LinearGradientBrush lgb = new LinearGradientBrush(gsc);
            mainBorder.Background = lgb;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            //ChangeUIElementOpacity(mainBorder, 1, 0);
            mainBorder.Background = new SolidColorBrush(Colors.Transparent);
        }


        private void SetWordForecolor(TextBlock textBlock,Color color0, double offset0, Color color1, double offset1)
        {

            GradientStopCollection gsc = new GradientStopCollection();
            gsc.Add(new GradientStop(color0, offset0));
            gsc.Add(new GradientStop(color1, offset1));

            LinearGradientBrush lgb = new LinearGradientBrush(gsc, 0.0);
            textBlock.Foreground = lgb;
        }


        private void ResetWordForecolor()
        {
            SetWordForecolor(this.lineOne,
                fontColor, 0,
                fontColor, 0);
            SetWordForecolor(this.lineTwo,
                fontColor, 0,
                fontColor, 0
                );
        }
        

        public void ChangeWord(string text0, string text1)
        {
            ResetWordForecolor();
            this.lineOne.Text = text0;
            this.lineTwo.Text = text1;
        }

        private void ShowWordWithAnimation(int line0, int line1)
        {
            if (line0 > 0)
            {
                ChangeUIElementOpacity(lineOne, 0, 1);
                ChangeUIElementFontSize(lineOne, 0, fontsize);
            }
            if (line1 > 0)
            {
                ChangeUIElementOpacity(lineTwo, 0, 1);
                ChangeUIElementFontSize(lineTwo, 0, fontsize);
            }
        }

        /// <summary>
        /// 改变偏移量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="offset"></param>
        public void ChangeStopOffset(int id, double offset)
        {
            switch (id)
            {
                case 0:
                    {
                        SetWordForecolor(this.lineOne,
                        fontColor, 0,
                        fontColor, offset);
                    }
                    break;
                case 1:
                    {
                        SetWordForecolor(this.lineTwo,
                        fontColor, 0,
                        fontColor, offset);
                    }
                    break;
                default:
                    break;
            }
        }

        internal void ChangeFontSize(int dskLrcFontSize)
        {
            try
            {
                fontsize = dskLrcFontSize;
                lineOne.FontSize = dskLrcFontSize;
                lineTwo.FontSize = dskLrcFontSize;
            }
            catch { }
        }

        internal void ChangeFontForeColor(Color dskLrcPlayedForecolor, Color dskLrcUnplayedForecolor)
        {
            fontColor = dskLrcPlayedForecolor;
        }

        internal int GetFontSize()
        {
            return this.fontsize;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
