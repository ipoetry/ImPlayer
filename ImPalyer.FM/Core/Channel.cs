using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using ImPlayer.FM;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImPlayer.FM.Core
{
    public class Channel
    {
        public async Task<ImPlayer.FM.Models.ChannelList> GetChannelListAsync()
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage message = await client.GetAsync("http://doubanfmcloud-channelinfo.stor.sinaapp.com/channelinfo");
                using (StreamReader sr = new StreamReader(await message.Content.ReadAsStreamAsync(), Encoding.UTF8))
                    return Deserialize<ImPlayer.FM.Models.ChannelList>(sr.ReadToEnd());
            }
            catch {  }
            return null;
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
