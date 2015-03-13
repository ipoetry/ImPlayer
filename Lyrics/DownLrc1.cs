using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lyrics
{
    class DownLrc
    {
        private const string lrcFilter = @"http://lrc.aspxp.net/?ar={0:s}&al=&ti={1:s}&f=1&x=30&y=9";//API
        static string LrcText = "";
        public delegate void DownCompleteNotice(bool isSuccess,string lrcPath);
        public static event DownCompleteNotice CompletedNoticeEventHandler;    
        private async static Task<Uri> GetLrcPath(string Title, string Artist)
        {
            try
            {
                string url = string.Format(lrcFilter, Artist, Title);
                HttpClient Client = new HttpClient();
                HttpResponseMessage response = await Client.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync(); 
                int beginPos = content.IndexOf("<div id=\"down\">");
                int endPos = content.IndexOf("</div>", beginPos);
                string lrcInfo = content.Substring(beginPos, endPos - beginPos);
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
        public async static void DownloadLrc(Song song)
        {
            WebClient wc = new WebClient();
            try
            {
                wc.DownloadDataCompleted += new DownloadDataCompletedEventHandler(DownCompletePush);
                wc.Credentials = CredentialCache.DefaultCredentials;
                wc.DownloadDataAsync(await GetLrcPath(song.Title == "" ? song.FileName : song.Title, song.Artist),song);
            }
            catch (Exception)
            {
                CompletedNoticeEventHandler(false,null);
            }

        }

        private static void DownCompletePush(object sender,DownloadDataCompletedEventArgs e)
        {
            Encoding enc = Encoding.GetEncoding("GB2312");
            try
            {
                Song song = (Song)e.UserState;
                Byte[] pageData = e.Result;
                LrcText = enc.GetString(pageData);
                string lrcPa=song.FileUrl.Remove(song.FileUrl.LastIndexOf(".")) + ".lrc";
                StreamWriter sw = new StreamWriter(lrcPa, false, Encoding.UTF8);
                sw.Write(LrcText);
                sw.Flush();
                sw.Close();
                CompletedNoticeEventHandler(true,lrcPa);
            }
            catch { CompletedNoticeEventHandler(false,null); }
        }
    }
}
