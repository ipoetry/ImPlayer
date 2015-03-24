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

namespace ImPlayer.DownloadMoudle
{
    /// <summary>
    /// DiyContextMenu.xaml 的交互逻辑
    /// </summary>
    public partial class DiyContextMenu : Window
    {
        public DiyContextMenu(DateTime dt)
        {
            InitializeComponent();
            CreateTime = dt;
        }

        private DateTime createTime;
        public DateTime CreateTime
        { get { return createTime; } set { createTime = value; } }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {

        }

    }
}
