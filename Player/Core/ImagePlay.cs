using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Player.Common;
using ArtistPic;
namespace Player
{
    /// <summary>
    /// PPT放映时代码段。
    /// </summary>
    public partial class MainPage : Window
    {
        /// <summary>
        /// 图片路径
        /// </summary>
        private List<string> imageList;
        ///// <summary>
        ///// 用来存放幻灯片放映前的背景。
        ///// </summary>
        private string preBackImage = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        private int index = 0;
        /// <summary>
        /// 
        /// </summary>
        private bool stopPlayPPT = true;
        /// <summary>
        /// 动画启动前，需要保存border的透明度。以便恢复。
        /// </summary>
        private double borderOpacity = 1d;

        public  bool isPPTPlaying = false;
        private List<string> EnumerateImageFiles(string path)
        {
            List<string> imageFiles = new List<string>();
            imageFiles.AddRange(
                Directory.EnumerateFiles(path, "*.jpg", SearchOption.TopDirectoryOnly));
            
            imageFiles.AddRange(
                Directory.EnumerateFiles(path, "*.png", SearchOption.TopDirectoryOnly));
            
            imageFiles.AddRange(
                Directory.EnumerateFiles(path, "*.bmp", SearchOption.TopDirectoryOnly));

            imageFiles.AddRange(
               Directory.EnumerateFiles(path, "*.jpeg", SearchOption.TopDirectoryOnly));
            return imageFiles;
        }


        //改动
        public void PlayPPT(Lyrics.Song song)
        {
            string sName = song.Artist;
            if (sName == "")
            {
                string[] info = song.FileName.Split('-');
                if (info.Length <= 1) 
                { 
                    if (isPPTPlaying)
                    {
                        StopPlayPPT();
                    } 
                    Console.WriteLine("无法识别，取消下载图片"); 
                    return;
                }
                sName = info[0];
            }
            isPPTPlaying = true;
            SingerPic.CompletedNoticeEventHandler += new SingerPic.CompletedNotice(StartPlay);
            SingerPic.BeginDownload(CommonProperty.SingerPicPath,sName);
        }
        //改动
        private void StartPlay(MyEventAgr e)
        {

            imageList = EnumerateImageFiles(CommonProperty.SingerPicPath + e.SName + "\\");
            if (imageList.Count <= 0) { if (isPPTPlaying) StopPlayPPT(); return; }
            //   borderOpacity = BorderImage.Opacity;
            BorderImage.Opacity = 0;
            prePPTImage.Visibility = System.Windows.Visibility.Visible;
            nowPPTImage.Visibility = System.Windows.Visibility.Visible;
            stopPlayPPT = false;
            ChangeImageWithAnimation(index);
        }

        /// <summary>
        /// 停止放映
        /// </summary>
        /// <returns></returns>
        public bool StopPlayPPT()
        {
            bool successful = false;
            stopPlayPPT = true;
            isPPTPlaying = false;
            BorderImage.Opacity=borderOpacity ;
            prePPTImage.Opacity = 0;
            prePPTImage.Visibility = System.Windows.Visibility.Collapsed;
            nowPPTImage.Visibility = System.Windows.Visibility.Collapsed;
            nowPPTImage.Opacity = 0;
            successful = true;
            AppPropertys.mainWindow.BorderImage.ImageSource = PictureHelp.GetBitmapImage("Img_Background.jpg");
            return successful;
        }

        /// <summary>
        /// 动画方式切换图片：通过改变透明度来实现的一种动画效果。
        /// </summary>
        private void ChangeImageWithAnimation(int imageIndex)
        {
            if (!stopPlayPPT)
            {
                DoubleAnimation daPrePPTImage = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(5000));
                daPrePPTImage.Completed += daWindowImage_Completed;
                DoubleAnimation daNowPPTImage = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(6000));
                daNowPPTImage.Completed += daBorderImage_Completed;
                if (imageIndex < imageList.Count)
                {
                    prePPTImage.Source = nowPPTImage.Source;
                    nowPPTImage.Source = GetBitmapImage(imageList[imageIndex]);
                }
                else
                {
                    prePPTImage.Source = nowPPTImage.Source;
                    nowPPTImage.Source = GetBitmapImage(imageList[0]);
                }
                prePPTImage.BeginAnimation(OpacityProperty, daPrePPTImage);
                nowPPTImage.BeginAnimation(OpacityProperty, daNowPPTImage);
            }
            else
            {
                prePPTImage.Opacity = 0;
                nowPPTImage.Opacity = 0;
            }
        }

        void daBorderImage_Completed(object sender, EventArgs e)
        {
            if (stopPlayPPT)
            {
                prePPTImage.Opacity = 0;
                nowPPTImage.Opacity = 0;
            }
            else
            {
                ChangeImageWithAnimation(index++);
                if (index >= imageList.Count)
                {
                    index = 0;
                }
            }
        }

        void daWindowImage_Completed(object sender, EventArgs e)
        {
            
        }
        private BitmapImage GetBitmapImage(string imagePath)
        {
            BitmapImage bit = null;
            if (imagePath.Trim() != "")
            {
                try
                {
                    bit = new BitmapImage();
                    bit.BeginInit();
                    bit.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
                    bit.EndInit();
                }
                catch (Exception)
                {
                    bit = null;
                }
            }
            return bit;
        }
    }
}
