using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImPlayer.DownloadMoudle.BaiduMusic
{
   public class BdSong
    {
        public string Title { get; set; }
        public string Song_Id { get; set; }
        public string Author { get; set; }
        public string Artist_Id { get; set; }
        public string All_Artist_Id { get; set; }
        public string Album_Title { get; set; }
        public string Appendix { get; set; }
        public string Album_Id { get; set; }
        public string Lrclink { get; set; }
        public string Resource_Type { get; set; }
        public string Content { get; set; }
        public string Relate_Status { get; set; }
        public int Havehigh { get; set; }
        public int Copy_Type { get; set; }
        public string All_Rate { get; set; }
        public bool Has_mv { get; set; }
        public bool Has_mv_Mobile { get; set; }
        public string Charge { get; set; }
        public string Toneid { get; set; }
        public string Info { get; set; }
        public string Data_Source { get; set; }
        public string Learn { get; set; }
    }
}
