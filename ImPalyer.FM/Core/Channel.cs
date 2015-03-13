using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using ImPlayer.FM;

namespace ImPalyer.FM.Core
{
    public class Channel
    {
        public  ImPlayer.FM.Models.ChannelList GetChannelList()
        {
            var conn = new ConnectionBase();
            var json = conn.Get("http://doubanfmcloud-channelinfo.stor.sinaapp.com/channelinfo");
            return Deserialize<ImPlayer.FM.Models.ChannelList>(json);
        }
        public static T Deserialize<T>(string json)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                return (T)ser.ReadObject(stream);
            }
        }
    }
}
