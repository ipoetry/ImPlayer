using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using Microsoft.Win32;
using Un4seen.Bass;

namespace Player
{
    /// <summary>
    /// EqualizerSet.xaml 的交互逻辑
    /// </summary>
    public partial class EqualizerSet : Window
    {
        public EqualizerSet()
        {
            InitializeComponent();
            EnumSliderControl();
            if (PlayController.EQState) { Switch.IsChecked = true; }
        }

        private int[] _fxEQ = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        #region 预设
        private int[] _recom = { 4, 2, 0, -3, -6, -6, -3, 0, 3, 5 };
        private int[] _pop = { 3, 1, 0, -2, -4, -4, -2, 0, 1, 2 };
        private int[] _rock = { -2, 0, 2, 4, -2, -2, 0, 0, 4, 4 };
        private int[] _dance = { -2, 3, 4, 1, -2, -2, 0, 0, 4, 4 };
        private int[] _elect = { -6, 1, 4, -2, -2, -4, 0, 0, 6, 6 };
        private int[] _country = { -2, 0, 0, 2, 2, 0, 0, 0, 4, 4 };
        private int[] _jazz = { 0, 0, 0, 4, 4, 4, 0, 2, 3, 4 };
        private int[] _classical = { 0, 8, 8, 4, 0, 0, 0, 0, 2, 2 };
        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        private void EnumSliderControl()
        {
            foreach (UIElement element in EqArea.Children)
            {
                Slider slider = element as Slider;
                if (slider != null)
                {
                    slider.Maximum = 150;
                    slider.Minimum = -150;
                    slider.Value = 0;
                    slider.SmallChange = 10;
                    slider.LargeChange = 50;
                    slider.ValueChanged += slider_ValueChanged;
                }
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sr = (Slider)sender;
            int xx = int.Parse(sr.Name.Remove(0, 6));
            if ((bool)Switch.IsChecked)
            {
                UpdateEQ(xx, (float)e.NewValue / 10f);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //this.Close();
            this.Hide();
        }

        private void UpdateEQ(int band, float gain)
        {
            BASS_DX8_PARAMEQ eq = new BASS_DX8_PARAMEQ();
            if (Bass.BASS_FXGetParameters(Player.PlayController.bassEng._fxEQ[band], eq))
            { 
                eq.fGain = gain;
                Bass.BASS_FXSetParameters(Player.PlayController.bassEng._fxEQ[band], eq);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //开启
           // MessageBox.Show(Switch.IsChecked==true?"已经开启":"已经关闭");
            PlayController.EQState = true;
        }

        private void Switch_Unchecked(object sender, RoutedEventArgs e)
        {
            //关闭均衡
            //调用 Bass.BASS_ChannelRemoveFX
            // MessageBox.Show(Switch.IsChecked == true ? "已经开启" : "已经关闭");
            PlayController.EQState = false;
        }

        private void Label_Initialized(object sender, EventArgs e)
        {
            this.lbSet.ContextMenu = null;
        }

        private void Label_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.contextMenu.PlacementTarget = this.lbSet;
            this.contextMenu.IsOpen = true;
        }

        private void Label_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            SetValues(_fxEQ);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Equlizer Profile|*.eq_cfg|TTPlayer Equlizer Profile|*.tteq_cfg";
            ofd.Title = "载入配置文件";
            if ((bool)ofd.ShowDialog())
            {
                LoadConfig(ofd.FileName);
            }
        }


        private void LoadConfig(string path)
        {
            XmlDocument dom = new XmlDocument();
            dom.Load(path);
            XmlElement root = dom.DocumentElement;
            XmlElement rootn = (XmlElement)((XmlNode)root).SelectSingleNode("Equalizer");
            string[] input=rootn.GetAttribute("Custom").Remove(0,2).Split(new char[]{',','，'});
            int[] values = Array.ConvertAll(input, s => int.Parse(s));
            SetValues(values);
        }

        private void SetValues(int[] values)
        {
            int count = 0;
            foreach (UIElement element in EqArea.Children)
            {
                Slider slider = element as Slider;
                if (slider != null)
                {
                    slider.Value = values[count++]*10f;
                }
            }
        }

        private void SetUnchecked()
        {

             FindVisualChild<MenuItem>(lbSet);

        }

        public void FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T )
                {
                    ((MenuItem)child).IsChecked = false;
                }
                else
                {
                    FindVisualChild<T>(child);
                }
            }
        }
        /// <summary>
        /// 保存当前选中的item
        /// </summary>
        private MenuItem OldSelect = null;

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            SetUnchecked();
            if (OldSelect != null) { OldSelect.IsChecked = false; }
            MenuItem mi=(MenuItem)sender;
            int xx = int.Parse(mi.Name.Remove(0,1));
            int[] fre=new int[]{};
            switch(xx)
            {
                case 1: fre = _recom; break;
                case 2: fre = _pop; break;
                case 3: fre = _rock; break;
                case 4: fre = _dance; break;
                case 5: fre = _elect; break;
                case 6: fre = _country; break;
                case 7: fre = _jazz; break;
                case 8: fre = _classical; break;
            }
            SetValues(fre);
            mi.IsChecked = true;
            OldSelect = mi;
        }
    }
}
