using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImPlayer.DownloadMoudle.BaiduMusic
{
    class BaiduMusicOp
    {

        #region  Online

        string BaseApi = "http://tingapi.ting.baidu.com/v1/restserver/ting?from=webapp_music&method=";

        string LrcApi = "baidu.ting.song.lry&songid={0}";
        /// <summary>
        /// type: //1、新歌榜，2、热歌榜，11、摇滚榜，12、爵士，16、流行21、欧美金曲榜，22、经典老歌榜，23、情歌对唱榜，24、影视金曲榜，25、网络歌曲榜

        ///size: 10 //返回条目数量

        ///offset: 0 //获取偏移
        /// </summary>
        string CollectionApi = "baidu.ting.billboard.billList&type={0}&size={1}&offset={2}";
        /// <summary>
        /// 推荐列表 num是返回数量
        /// </summary>
        string RecommandListApi = "baidu.ting.song.getRecommandSongList&song_id={0}&num={1}";
        /// <summary>
        /// baidu.ting.song.play  {songid: id}
        ///baidu.ting.song.playAAC  {songid: id}
        /// </summary>
        #endregion

        #region
        static string SrarchApi = "http://tingapi.ting.baidu.com/v1/restserver/ting?from=webapp_music&method=baidu.ting.search.catalogSug&format=json&callback=&query={0}&_=1413017198449";

        static string SerachApi2 = "http://tingapi.ting.baidu.com/v1/restserver/ting?method=baidu.ting.search.common&query={0}&page_size={1}&page_no={2}&format=json"; //&from=ios&version=2.1.1
        static string DownLoadApi = "http://tingapi.ting.baidu.com/v1/restserver/ting?from=webapp_music&method=baidu.ting.song.downWeb&songid={0}&bit={1}&_t={2}";

        static string LosslessUrl = "http://music.baidu.com/data/music/fmlink?songIds={0}&rate=320&type={1}&_t={2}&format=json&callback=__.cb_download&_={3}";
        #endregion

        public static async Task<SearchSongResultByKey> SearchMusicByKey(string key, int pageNum)
        {
            SearchSongResultByKey ssr = null;
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage content = await client.GetAsync(string.Format(SerachApi2, key, 10, pageNum));
                string strResult = await content.Content.ReadAsStringAsync();
                ssr = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchSongResultByKey>(strResult);
                //   List<bdSong2> Songs = ssr.Song_List;
                foreach (BdSong bdSong in ssr.Song_List)
                {
                    bdSong.Title = bdSong.Title.Replace("<em>", "").Replace("</em>", "");
                    bdSong.Author = bdSong.Author.Replace("<em>", "").Replace("</em>", "");
                    bdSong.Album_Title = bdSong.Album_Title.Replace("<em>", "").Replace("</em>", "");
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return ssr;
        }

        public static async Task<SearchSongResultById> GetMusicUrl(string songId)
        {
            HttpClient client = new HttpClient(new PCHttpClienHanlder());
            HttpResponseMessage content = await client.GetAsync(string.Format(DownLoadApi, songId, "flac", DateTime.Now.ToString()));//DownLossless
            string result = await content.Content.ReadAsStringAsync();
            SearchSongResultById dsr = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchSongResultById>(result);
            return dsr;
        }

        static long reqCount = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="songId"></param>
        /// <param name="bitrate">flac or mp3</param>
        /// <returns></returns>
        public static async Task<List<HightQualitySongInfo>> GetLosslessMusicUrl(string songId, string bitrate = "")
        {
            // bitrate = bitrate == "mp3" ? "" : "flac,mp3";
            long CurrentTotalMS = GetTotalMilliseconds();
            reqCount = reqCount == 0 ? CurrentTotalMS - 3000 : reqCount + 1;
            HttpClient client = new HttpClient(new PCHttpClienHanlder());
            HttpResponseMessage content = await client.GetAsync(string.Format(LosslessUrl, songId, bitrate, CurrentTotalMS, reqCount));//DownLossless
            string result = await content.Content.ReadAsStringAsync();
            result = result.Substring(result.IndexOf("(") + 1);
            result = result.Substring(0, result.LastIndexOf(")"));
            var rObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(rObj["data"].ToString());
            if (data == null) { return null; }
            return JsonConvert.DeserializeObject<List<HightQualitySongInfo>>(data["songList"].ToString());
        }

        public static long GetTotalMilliseconds()
        {
            DateTime dt1970 = new DateTime(1970, 1, 1);
            return (long)(DateTime.Now - dt1970).TotalMilliseconds;
        }

        class PCHttpClienHanlder : HttpClientHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 Safari/537.36");
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}
