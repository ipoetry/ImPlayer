using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImPlayer.DownloadMoudle.BaiduMusic
{
    public class Bitrate : IComparable
    {
        public int file_bitrate { get; set; }
        public string file_link { get; set; }
        public string file_extension { get; set; }
        public string original { get; set; }
        public string file_size { get; set; }
        public string file_duration { get; set; }
        public string show_link { get; set; }
        public string song_file_id { get; set; }
        public string replay_gain { get; set; }

        #region IComparable 成员

        public int CompareTo(object obj)
        {

            try
            {
                Bitrate bitrate = obj as Bitrate;
                return this.file_bitrate > bitrate.file_bitrate ? -1 : 1;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        #endregion
    }  
}
