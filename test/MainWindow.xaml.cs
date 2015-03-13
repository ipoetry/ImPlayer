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
using System.Windows.Navigation;
using System.Windows.Shapes;
using OtherFunction;
namespace test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
          
            

            //Baidu1 baidu1 = new Baidu1("xxxxxx@qq.com", "xxxxxx");

            //List<BaiDuFile> list = baidu1.GetFileDir();

            //string path = "/data/data1.txt";
            //string localPath = System.Environment.CurrentDirectory + "/data/data_" + DateTime.Now.ToString("MMddHHmmss") + ".txt";
            //FileOper fo = baidu1.DownFile(path, localPath);

            //path = "/data/00.jpg";
            //string fileToUpload = System.Environment.CurrentDirectory + "/data/00.jpg";
            //fo = baidu1.UpLoadFile(path, fileToUpload);
        }

        private void Comp(ArtistPic.MyEventAgr sName)
        {
            MessageBox.Show("完成了！");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
         //   TipsWindow.Show("Hello"," Wrold",2000);
           // Snow.Snow.Show();
            Snow.Tips.Show(80000);


            //DateTime dt = DateTime.Now;
            //ChineseCalendar cc = new ChineseCalendar(dt);
            //Console.WriteLine("阳历：" + cc.DateString);
            //Console.WriteLine("农历：" + cc.ChineseDateString);
            //Console.WriteLine("节气：" + cc.ChineseTwentyFourDay);
            //Console.WriteLine("节日：" + cc.DateHoliday);


        }

        private void NotifyIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
          //  if (this.IsVisible)
              //  this.HideMinimized();
           // else
               // this.ShowFront();
        }
    }
}
