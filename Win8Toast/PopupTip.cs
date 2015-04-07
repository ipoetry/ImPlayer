using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Net;


namespace Win8Toast
{
    public class PopupTip
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgText"></param>
        public static void ShowPopUp(string msgText)
        {
            //Create a Window
            Window popUp = new Window();
            popUp.Name = "PopUp";
            popUp.AllowsTransparency = true;
            popUp.Background = Brushes.Transparent;
            popUp.WindowStyle = WindowStyle.None;
            popUp.ShowInTaskbar = false;
            popUp.Topmost = true;
            popUp.Height = 100;
            popUp.Width = 200;
            popUp.Left = Screen.PrimaryScreen.Bounds.Width - 310;
            popUp.Top = Screen.PrimaryScreen.Bounds.Height - 120;

            Grid grid = new Grid();

            Image img = new Image();
            img.Stretch = Stretch.Fill;
            img.Source = BitmapToImageSource(Properties.Resources.bg);
            img.Effect = new System.Windows.Media.Effects.DropShadowEffect();
            grid.Children.Add(img);
            TextBlock msg = new TextBlock();
            msg.Padding = new Thickness(20);
            msg.VerticalAlignment = VerticalAlignment.Center;
            msg.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            msg.TextWrapping = TextWrapping.Wrap;
            msg.Text = msgText;
            msg.FontSize = 15;
            msg.FontStyle = FontStyles.Italic;
            grid.Children.Add(msg);

            popUp.Content = grid;
            NameScope.SetNameScope(popUp, new NameScope());
            //Register the window's name, this is necessary for creating Storyboard using codes instead of XAML
            popUp.RegisterName(popUp.Name, popUp);
            
            //Create the fade in & fade out animation
            DoubleAnimation winFadeAni = new DoubleAnimation();
            winFadeAni.From = 0;
            winFadeAni.To = 1;
            winFadeAni.Duration = new Duration(TimeSpan.FromSeconds(1.5));		//Duration for 1.5 seconds
            winFadeAni.AutoReverse = true;
            winFadeAni.Completed += delegate(object sender, EventArgs e)			//Close the window when this animation is completed
            {
                popUp.Close();
            };

            Storyboard.SetTargetName(winFadeAni, popUp.Name);
            Storyboard.SetTargetProperty(winFadeAni, new PropertyPath(Window.OpacityProperty));

            Storyboard winFadeStoryBoard = new Storyboard();
            winFadeStoryBoard.Children.Add(winFadeAni);
            popUp.Loaded += delegate(object sender, RoutedEventArgs e)
            {
                winFadeStoryBoard.Begin(popUp);
            };
            popUp.Show();
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
        /// <summary>
        /// 从bitmap转换成ImageSource
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        private static ImageSource BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            if (!DeleteObject(hBitmap))//记得要进行内存释放。否则会有内存不足的报错。
            {
                throw new System.ComponentModel.Win32Exception();
            }
            return wpfBitmap;
        }


        #region 检查网络状态

        //检测网络状态  
        [DllImport("wininet.dll")]
        extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        /// <summary>  
        /// 检测网络状态  
        /// </summary>  
        public static bool IsConnected { get { int I = 0; return InternetGetConnectedState(out I, 0); } }

        #endregion
        static bool isWin8OrHeigh = SystemHepler.IsWin8OrHeigher();
        public async static Task<bool> CheckNetWork()
        {
            bool curentNetState = IsConnected;
            if (!curentNetState) {
                if (isWin8OrHeigh)
                    ToastTip.ShowToast();
                else
                    ShowPopUp("网络已断开，请检查");
                return IsConnected; }
            curentNetState = await PingBaidu();
            if (!curentNetState){
                if (isWin8OrHeigh)
                    ToastTip.ShowToast();
                else
                    ShowPopUp("网络已断开，请检查");
            }
            return curentNetState;
        }

        #region
        public delegate void NetWorkCheckDelegate();
        public event NetWorkCheckDelegate NetWrokCheckCompleted;
        public void BeginCheck()
        {
            for (int i = 0; i < 3; i++)
            {
                Ping ping = new Ping();
                ping.PingCompleted += new PingCompletedEventHandler(PingComplectedPush);
                ping.SendAsync("202.108.22.5", 3000, null);
            }
        }
        int pingCount = 0;
        int successCount = 0;
        public void PingComplectedPush(object sender, PingCompletedEventArgs e)
        {
            pingCount++;
            if (e.Reply.Status == IPStatus.Success)
            {
                successCount++;
                if (successCount >= 2)
                {
                    if (NetWrokCheckCompleted != null)
                    {
                        NetWrokCheckCompleted();
                        successCount = 0;
                        pingCount = 0;
                    }
                }
            }
            else if (pingCount >= 3 && successCount <= 1)
            {
                ShowPopUp("网络未连接，请检查");
                successCount = 0;
                pingCount = 0;
            }
        }
        #endregion

        public static async Task<bool> PingDesignatedIP()
        {
            Ping ping = new Ping();
            PingOptions poptions = new PingOptions();
            poptions.DontFragment = true;
            string data = string.Empty;
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            int timeout = 1200;

            PingReply reply = await ping.SendPingAsync(IPAddress.Parse("74.125.71.104"), timeout, buffer, poptions);

            return reply.Status == IPStatus.Success?true:false;

        }

        public static async Task<bool> PingBaidu()
        {
            Ping ping = new Ping();
            PingOptions poptions = new PingOptions();
            poptions.DontFragment = true;
            string data = string.Empty;
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            PingReply reply = await ping.SendPingAsync(IPAddress.Parse("220.181.6.18"), 1000, buffer, poptions);

            return reply.Status == IPStatus.Success ? true : false;

        }

    }
}
