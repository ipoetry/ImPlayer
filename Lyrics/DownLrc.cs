using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Lyrics
{
    class DownLrc
    {
        private const string lrcFilter = @"http://lrc.aspxp.net/?ar={0:s}&al=&ti={1:s}&f=1&x=30&y=9";//API
        #region Baidu
        private const string lrcSerach_Baidu = "http://ttlrc.qianqian.com/dll/lyricsvr.dll?sh?Artist={0}&Title={1}&From=BaiduMusic&Version=9.1.7.2&Flags=2&ci=63CCB91F1571C8CDC07BB106E22D7FA6";
        private const string lrcDownload_Baidu = "http://ttlrc.qianqian.com/dll/lyricsvr.dll?dl?Id={0}&Code=3242031272&ci=63CCB91F1571C8CDC07BB106E22D7FA6";
        #endregion
        #region QianQian
        private const string lrcSerach_Qian = "http://ttlrcct2.qianqian.com/dll/lyricsvr.dll?sh?Artist={0}&Title={1}&Flags=0";
        private const string lrcDownload_Qian = "http://ttlrcct2.qianqian.com/dll/lyricsvr.dll?dl?Id={0}&Code={1}";
        #endregion
        #region 检查网络状态

        //检测网络状态  
        [DllImport("wininet.dll")]
        extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        /// <summary>  
        /// 检测网络状态  
        /// </summary>  
        static bool isConnected{get{int I = 0;return InternetGetConnectedState(out I, 0);}}

        #endregion  
        static string LrcText = "";
        public delegate void DownCompleteNotice(bool isSuccess,string lrcPath);
        public static event DownCompleteNotice CompletedNoticeEventHandler; 
   
        private async static Task<Uri> GetLrcUrlByFileName(string FileName,string artist="")
        {
            try
            {
                string url = string.Format(lrcFilter, GetGb2312(artist), GetGb2312(FileName)); //utf-8
                HttpClient client = new HttpClient(new PCHttpClienHanlder());
                HttpResponseMessage content = await client.GetAsync(url);//DownLossless
                byte[] bs = await content.Content.ReadAsByteArrayAsync();
                string result = Encoding.GetEncoding("GB2312").GetString(bs);//= await content.Content.ReadAsStringAsync();
                int beginPos = result.IndexOf("<div id=\"down\">");
                int endPos = result.IndexOf("</div>", beginPos);
                string lrcInfo = result.Substring(beginPos, endPos - beginPos);
                beginPos = lrcInfo.IndexOf("</a>");
                beginPos = lrcInfo.IndexOf("\"", beginPos);
                endPos = lrcInfo.IndexOf("\"", beginPos + 1);
                string lrcUrl = lrcInfo.Substring(beginPos + 1, endPos - beginPos - 1);
                return new Uri(@"http://lrc.aspxp.net/" + lrcUrl);
            }
            catch
            {
                throw;
            }
        }

        private async static Task<Uri> GetLrcUrl(string Title, string Artist)
        {
            HttpClient client = new HttpClient(new PCHttpClienHanlder());
            HttpResponseMessage content = await client.GetAsync(string.Format(lrcSerach_Qian, GetUnicode(Artist), GetUnicode(Title)));
          //  byte[] bs = await content.Content.ReadAsByteArrayAsync();
            string result = await content.Content.ReadAsStringAsync();
           // result = Encoding.UTF8.GetString(bs);
            if (string.IsNullOrEmpty(result)) { Console.WriteLine("error:(百度)搜索歌词时 返回为null"); return null; }
            XmlDocument docuemnt = new XmlDocument();
            docuemnt.LoadXml(result);
            XmlNode xn=  docuemnt.SelectSingleNode(".//lrc");
            if (xn==null) { Console.WriteLine("没有搜索到歌词：百度服务器"); return null; }
            int Id = int.Parse(xn.Attributes["id"].Value);
            string url = string.Format(lrcDownload_Qian,Id , GenerateCode(Artist,Title,Id));
            Console.WriteLine(url);
            return new Uri(url);
        }
        public async static void BeginDownloadLrc(Song song,int isFirst=0)
        {
            if (!isConnected) { Console.WriteLine("网络未连接，无法下载歌词"); CompletedNoticeEventHandler(false, null); return; };
            WebClient wc = new WebClient();
            string[] info = GetTitleAndArtistByFilename(song);
            Uri dlUrl=null;
            if (isFirst == 1) { dlUrl = await GetLrcUrlByFileName(info[0], info.Length > 1 ? info[1] : "");}
            dlUrl = info.Length > 1 ? await GetLrcUrl(info[0], info[1]) : await GetLrcUrlByFileName(info[0]);
            if (dlUrl == null || !isConnected) { CompletedNoticeEventHandler(false, null); return; };
            try
            {
                song.Company = dlUrl.ToString();
                wc.DownloadDataCompleted += new DownloadDataCompletedEventHandler(DownCompletePush);
               // wc.DownloadFileCompleted +=  wc_DownloadFileCompleted;
                wc.Credentials = CredentialCache.DefaultCredentials;
                wc.DownloadDataAsync(dlUrl, song);
              //  string lrcSavePath = song.FileUrl.Remove(song.FileUrl.LastIndexOf(".")) + ".lrc";
               // wc.DownloadFileAsync(dlUrl, lrcSavePath, lrcSavePath);
            }
            catch (Exception)
            {
                CompletedNoticeEventHandler(false,null);
            }
        }
        private static void wc_DownloadFileCompleted(object sender,System.ComponentModel.AsyncCompletedEventArgs e)
        {
            CompletedNoticeEventHandler(true, e.UserState.ToString());
        }
        private static void DownCompletePush(object sender,DownloadDataCompletedEventArgs e)
        {
            try
            {
                Song song = (Song)e.UserState;
                Byte[] pageData = e.Result;
                Encoding enc;
                if (song.Company.Contains("http://lrc.aspxp.net/"))
                {
                    enc = Encoding.GetEncoding("GB2312");
                }
                else
                {
                    enc = Encoding.UTF8;
                }
                LrcContent = enc.GetString(pageData);

                if (LrcText.Contains("errmsg") && LrcText.Contains("errcode"))
                {
                    BeginDownloadLrc(song, 1); return;
                }
                string lrcSavePath=song.FileUrl.Remove(song.FileUrl.LastIndexOf(".")) + ".lrc";
                StreamWriter sw = new StreamWriter(lrcSavePath, false, Encoding.UTF8);
                sw.Write(LrcText);
                sw.Flush();
                sw.Close();
                CompletedNoticeEventHandler(true,lrcSavePath);
            }
            catch { CompletedNoticeEventHandler(false,null); }
        }

        class PCHttpClienHanlder : HttpClientHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 Safari/537.36");
                return base.SendAsync(request, cancellationToken);
            }
        }

        private static string GetUnicode(string str)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            string result = "";
            foreach (byte b in bytes)
            {
                result += b.ToString("x").PadLeft(2, '0');
            }
            return result;
        }

        private static string GetGb2312(string str)
        {
            string code = "";
            byte[] bytes=Encoding.GetEncoding("GB2312").GetBytes(str);
            foreach (byte b in bytes)
            {
                code += '%' + b.ToString("X2");
            }
            return code;
        }

        public static string[] GetTitleAndArtistByFilename(Song song)
        {
            string[] info = {song.Title,song.Artist };
            if (song.Title == "" || song.Artist == "")
            {
                info = song.FileName.Split('-');
                if (info.Length <= 1 && song.Title != "") { info[0] = song.Title; }
            }
            return info;
        }
        #region From Internet author:none
        public static string GenerateCode(string singer, string title, int lrcId)
        {
            string qqHexStr = ToQQHexString(singer + title, Encoding.UTF8);
            int length = qqHexStr.Length / 2;
            int[] song = new int[length];
            for (int i = 0; i < length; i++)
            {
                song[i] = int.Parse(qqHexStr.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            int t1 = 0, t2 = 0, t3 = 0;
            t1 = (lrcId & 0x0000FF00) >> 8;

            t3 = (lrcId & 0x00FF0000) == 0 ? 0x000000FF & ~t1 : 0x000000FF & ((lrcId & 0x00FF0000) >> 16);
            
            t3 = t3 | ((0x000000FF & lrcId) << 8);
            t3 = t3 << 8;
            t3 = t3 | (0x000000FF & t1);
            t3 = t3 << 8;

            t3=(lrcId & 0xFF000000) == 0?t3 | (0x000000FF & (~lrcId)):t3 | (0x000000FF & (lrcId >> 24));

            int j = length - 1;
            while (j >= 0)
            {
                int c = song[j];
                if (c >= 0x80) c = c - 0x100;

                t1 = (int)((c + t2) & 0x00000000FFFFFFFF);
                t2 = (int)((t2 << (j % 2 + 4)) & 0x00000000FFFFFFFF);
                t2 = (int)((t1 + t2) & 0x00000000FFFFFFFF);
                j -= 1;
            }
            j = 0;
            t1 = 0;
            while (j <= length - 1)
            {
                int c = song[j];
                if (c >= 128) c = c - 256;
                int t4 = (int)((c + t1) & 0x00000000FFFFFFFF);
                t1 = (int)((t1 << (j % 2 + 3)) & 0x00000000FFFFFFFF);
                t1 = (int)((t1 + t4) & 0x00000000FFFFFFFF);
                j += 1;
            }

            int t5 = (int)Conv(t2 ^ t3);
            t5 = (int)Conv(t5 + (t1 | lrcId));
            t5 = (int)Conv(t5 * (t1 | t3));
            t5 = (int)Conv(t5 * (t2 ^ lrcId));

            long t6 = (long)t5;
            if (t6 > 2147483648)
                t5 = (int)(t6 - 4294967296);
            return t5.ToString();
        }

        public static long Conv(int i)
        {
            long r = i % 4294967296;
            if (i >= 0 && r > 2147483648)
                r = r - 4294967296;

            if (i < 0 && r < 2147483648)
                r = r + 4294967296;
            return r;
        }

        public static string ToQQHexString(string s, Encoding encoding)
        {
            StringBuilder sb = new StringBuilder();
            byte[] bytes = encoding.GetBytes(s);
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("X").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
        #endregion

        public static string LrcContent { get; set; }
    }
}
