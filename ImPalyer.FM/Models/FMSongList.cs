using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ImPlayer.FM.Models
{
    [DataContract]
    public class FMSongList
    {
        [DataMember(Name = "r")]
        public int R { get; set; }
        [DataMember(Name = "song")]
        public List<FMSong> Songs { get; set; }
    }
}
