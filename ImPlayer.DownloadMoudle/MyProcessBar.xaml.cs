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

namespace ImPlayer.DownloadMoudle
{
    /// <summary>
    /// MyProcessBar.xaml 的交互逻辑
    /// </summary>
    public partial class MyProcessBar : UserControl
    {
        public MyProcessBar()
        {
            InitializeComponent();
            grid.Width = 0;
        }

        public static DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", //属性名称
        typeof(double), //属性类型
        typeof(MyProcessBar), //该属性所有者，即将该属性注册到那个类上
        new PropertyMetadata(0d, new PropertyChangedCallback(OnValueChanged))); //属性默认值
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MyProcessBar g = d as MyProcessBar;
            if (g != null)
            {
                g.grid.Width = ((double)e.NewValue / 100d) * g.ActualWidth;
                g.Process.Content = "%" + ((double)e.NewValue).ToString("F1");
            }
            Console.WriteLine("ValueChanged new value is {0}", e.NewValue);
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static DependencyProperty CurrentStateProperty = DependencyProperty.Register("CurrentState", //属性名称
        typeof(bool), //属性类型
        typeof(MyProcessBar), //该属性所有者，即将该属性注册到那个类上
        new PropertyMetadata(false, new PropertyChangedCallback(OnValueChanged2)));


        private static void OnValueChanged2(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MyProcessBar g = d as MyProcessBar;
            if (g != null)
            {
                g.State.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
            Console.WriteLine("ValueChanged new value is {0}", e.NewValue);
        }

        public bool CurrentState
        {
            get { return (bool)GetValue(CurrentStateProperty); }
            set { SetValue(CurrentStateProperty, value); }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CreatBlock();
        }

        void CreatBlock()
        {
            stackPanel.Children.Clear();
            for (int i = 0; i < (ActualWidth / 10) + 3; i++)
            {
                var tempRect = new Rectangle
                {
                    Width = 10,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Fill = new SolidColorBrush(Colors.LightBlue)
                };
                stackPanel.Children.Add(tempRect);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        } 
    }
}
