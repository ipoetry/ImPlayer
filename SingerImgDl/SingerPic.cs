using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ArtistPic
{
   public class SingerPic
    {
        public delegate void CompletedNotice(MyEventAgr e);
        public static event CompletedNotice CompletedNoticeEventHandler;
        static string BDAPI = "http://image.baidu.com/i?tn=baiduimagejson&ct=201326592&cl=2&lm=-1&st=-1&fm=result&fr=&sf=1&fmq=1349413075627_R&pv=&ic=0&nc=1&z=&se=1&showtab=0&fb=0&width=&height=&face=0&istype=2&word={0}&rn=20&pn=1";
        static string KuwoApi = "http://artistpicserver.kuwo.cn/pic.web?type=big_artist_pic&pictype=url&content=list&&id=0&name={0}&rid=312611&from=pc&json=1&version=1&width=1366&height=768";

        private async static Task<string> GetPicURL(string sName)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage content = await client.GetAsync(string.Format(BDAPI, sName + " 写真"));
                string result = await content.Content.ReadAsStringAsync();
                return result;
            }
            catch
            {
                throw;
            }

        }

        private async static Task<List<string>> GetPicsURL(string ArtistName)
        {
            try
            {
                //TODO读取用户配置文件,获取背景图片质量设置
                HttpClient client = new HttpClient();
                string picQuality = "bkurl";//wpurl
                HttpResponseMessage content = await client.GetAsync(string.Format(KuwoApi, ArtistName));
                string result = await content.Content.ReadAsStringAsync();
                Dictionary<string, object> con1 = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                List<dynamic> dyObjs = JsonConvert.DeserializeObject<List<dynamic>>(con1["array"].ToString());
                List<string> urls = new List<string>();
                foreach (dynamic dy in dyObjs)
                {
                    string cc = dy[picQuality];
                    if(cc!=null)
                        urls.Add(cc);
                }
                picsCount = urls.Count();
                return urls;
            }
            catch
            {
                throw;
            }

        }

        public async static void BeginDownloads(string sName)
        {
            int count = 1;
            
            string dir = AppDomain.CurrentDomain.BaseDirectory+"\\Portray\\"+sName+"\\";
            if (System.IO.Directory.Exists(dir)) { CompletedNoticeEventHandler(new MyEventAgr {SName=sName,WC=null}); return; }              
            JsonFor li = JsonConvert.DeserializeObject<JsonFor>(await GetPicURL(sName));
            li.data.RemoveAt(li.data.Count - 1);
            li.data.Sort(delegate(WebImg w1, WebImg w2)
            {
                if (string.IsNullOrEmpty(w1.filesize) || string.IsNullOrEmpty(w2.filesize)) return 0;
                else
                {
                    if (int.Parse(w2.filesize) >= int.Parse(w1.filesize))
                        return 1;
                    else
                        return - 1;
                }//按所占空间从大到小排序
              // return w2.filesize.CompareTo(w1.filesize);
            });
            li.data.RemoveRange(5,li.data.Count-5);
            foreach (WebImg wi in li.data)
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
                webClient.Credentials = CredentialCache.DefaultCredentials;
                if (wi.filesize != null)
                {                  
                    if (webClient.IsBusy)
                    {
                        webClient.CancelAsync();
                    }
                    System.IO.Directory.CreateDirectory(dir);
                    webClient.DownloadFileAsync(new Uri(wi.objURL), dir + sName + "_" + count++ + "." + wi.type.ToLower(), new MyEventAgr { SName=sName,WC=webClient});
                }
            }
        }
        static int picsCount = 0;
        private static void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null) { ((MyEventAgr)e.UserState).WC.Dispose(); }
            //throw new Exception(e.Error.Message);
            picsCount--;
            if (picsCount<1) //全部下载完成后发出通知
            {
                CompletedNoticeEventHandler((MyEventAgr)e.UserState);
            }
        }
        
       /// <summary>
        /// 开始异步下载
        /// </summary>
        /// <param name="ArtistName"></param>
        public async static void BeginDownload(string ArtistName)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "\\Portray\\" + ArtistName + "\\";
            if (System.IO.Directory.Exists(dir))
            {
                CompletedNoticeEventHandler(new MyEventAgr { SName = ArtistName, WC = null }); 
                return; 
            }
            if (! await Win8Toast.PopupTip.CheckNetWork()) { Console.WriteLine("error:网络未连接，无法下载歌手图片"); return; }
            List<string> urlList = await GetPicsURL(ArtistName);
            foreach (string picUrl in urlList)
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
                webClient.Credentials = CredentialCache.DefaultCredentials;
                System.IO.Directory.CreateDirectory(dir);
                webClient.DownloadFileAsync(new Uri(picUrl), dir + ArtistName + "_" + urlList.IndexOf(picUrl) + picUrl.Substring(picUrl.LastIndexOf('.')), new MyEventAgr { SName = ArtistName, WC = webClient });
            }
        }
    }
}
