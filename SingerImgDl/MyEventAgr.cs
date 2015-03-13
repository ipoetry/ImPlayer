using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ArtistPic
{
   public class MyEventAgr:EventArgs
    {
       public string SName { get;set;}
       public WebClient WC { get; set; }
    }
}
