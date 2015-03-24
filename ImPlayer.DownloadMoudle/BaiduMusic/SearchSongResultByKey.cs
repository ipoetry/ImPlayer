using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImPlayer.DownloadMoudle.BaiduMusic
{
    public class SearchSongResultByKey
    {
        public string query { get; set; }
        public bool Is_Artist { get; set; }
        public bool Is_Album { get; set; }
        public string Rs_words { get; set; }
        public BdPage Pages { get; set; }
        public List<BdSong> Song_List { get; set; }
        public List<BdAlbum> Album { get; set; }
    }
}
