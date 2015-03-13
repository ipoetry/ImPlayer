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

namespace Player
{
    /// <summary>
    /// PBar.xaml 的交互逻辑
    /// </summary>
    public partial class PBar : Window
    {
        public PBar()
        {
            InitializeComponent();
        }

        public void SetNotifyInfo(int percent, string message)
        {
            this.labTips.Content = message;
            this.pb.Value = percent;
        }   
    }
}
