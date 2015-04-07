using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImPlayer.FM;
using ImPlayer.FM.Helpers;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace ImPalyer.FM.Core
{
    public class Song
    {
       
        /// <summary>
        /// 根据频道和歌曲,得到歌曲列表
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="song"></param>
        /// <param name="type">n-New</param>
        /// <returns></returns>
        public async Task<ImPlayer.FM.Models.FMSongList> GetSongListAsync(int channelId, string songId)
        {
            Parameters parameters = new Parameters();
            parameters["from"] = "mainsite";
            parameters["sid"] = songId;
            parameters["channel"] = channelId.ToString();
            parameters["type"] = songId == null ? "n" : "s";
            Random rnd = new Random();
            var number = rnd.NextDouble();
            parameters["r"] = number.ToString();

            string url = ConstructUrlWithParameters("http://douban.fm/j/mine/playlist", parameters);
            //获取列表
            HttpClient client = new HttpClient(new PCHttpClienHanlder());
            HttpResponseMessage message = await client.GetAsync(url);
            using (StreamReader sr = new StreamReader(await message.Content.ReadAsStreamAsync(), Encoding.UTF8))
            {
                string json = sr.ReadToEnd();
                var songList = JsonHelper.Deserialize<ImPlayer.FM.Models.FMSongList>(json);
                //将小图更换为大图
                foreach (var s in songList.Songs)
                {
                    s.Picture = new Uri(s.Picture.ToString().Replace("/mpic/", "/lpic/").Replace("//otho.", "//img3."));
                }
                songList.Songs.RemoveAll(s => s.IsAd);
                return songList;
            }
        }


        class PCHttpClienHanlder : HttpClientHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Referrer = new Uri("http://douban.fm");
                request.Headers.Add("Accpet", "application/json, text/javascript, */*; q=0.01");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)"); 
                return base.SendAsync(request, cancellationToken);
            }
        }

        public string ConstructUrlWithParameters(string baseUrl, Parameters parameters)
        {
            if (parameters == null || parameters.Count() == 0)
                return baseUrl;
            return baseUrl + "?" + parameters;
        }
    }
}
