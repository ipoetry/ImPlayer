using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImPlayer.DownloadMoudle.BaiduMusic
{
    public class SearchSongResultById
    {
        public int Error_code { get; set; }
        public List<Bitrate> Bitrate { get; set; }
        public SongInfo songInfo { get; set; }
    }
}
