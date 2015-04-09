using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass.AddOn.Tags;

namespace Player
{
    class CommonProperty
    {
        #region
        private static string _supportFormat=".mp3.wma.wav.ape.flac.acc.mp4.ogg.m4a";
        public static string SupportFormat { get { return _supportFormat; } }
        #endregion

        /// <summary>
        /// 判断当前系统是否是Win8或更高版本
        /// </summary>
        /// <returns></returns>
        public static bool getOpVer()
        {
         Version currentVersion = System.Environment.OSVersion.Version;
         Version comParVersion=new Version("6.2");
         return currentVersion.CompareTo(comParVersion)>=0?true:false;
        }
        /// <summary>
        /// 从路径中提取文件名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string getTitleFromPath(string path)
        {
            int index = path.LastIndexOf("\\")+1;
            return path.Substring(index);
        }

        public static string RunDir { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        public static string AlbumPicPath { get { return AppDomain.CurrentDomain.BaseDirectory+@"Data\Album\"; } }
        public static string SingerPicPath { get { return AppDomain.CurrentDomain.BaseDirectory + @"Data\Portray\"; } }
    }
}
