using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtistPic
{
    class JsonFor
    {
        public string queryEnc { get; set; }
        public int listNum { get; set; }
        public int displayNum { get; set; }
        public string bdFmtDispNum { get; set; }
        public string bdSearchTime { get; set; }
        public string bdIsClustered { get; set; }
        public List<WebImg> data { get; set; }
    }
}
