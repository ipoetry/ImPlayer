using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImPlayer.FM;
using ImPlayer.FM.Helpers;

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
        public ImPlayer.FM.Models.FMSongList GetSongList(int channelId, string songId)
        {
            Parameters parameters = new Parameters();
            parameters["from"] = "mainsite";
           // parameters["context"] = channel.Context;
          //  parameters["sid"] = song != null ? song.SongId : null;
            parameters["sid"] = songId;
            parameters["channel"] = channelId.ToString();
          //  parameters["channel"] = channel.ID.ToString();
            parameters["type"] =songId== null ? "n" : "s";
            Random rnd = new Random();
            var number = rnd.NextDouble();
            parameters["r"] = number.ToString();

            string url = ConnectionBase.ConstructUrlWithParameters("http://douban.fm/j/mine/playlist", parameters);
            //获取列表
            string json = new ConnectionBase().Get(url, @"application/json, text/javascript, */*; q=0.01", @"http://douban.fm");
            var songList=JsonHelper.Deserialize<ImPlayer.FM.Models.FMSongList>(json);
            //将小图更换为大图
            foreach (var s in songList.Songs)
            {
                s.Picture = new Uri(s.Picture.ToString().Replace("/mpic/", "/lpic/").Replace("//otho.", "//img3."));
            }

            //去广告
            songList.Songs.RemoveAll(s => s.IsAd);
            return songList;
        }
    }
}
