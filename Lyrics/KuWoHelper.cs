using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Lyrics
{
    class KuWoHelper
    {
        public async static Task<string> GetLyricUrl(string key)
        {
            string id = await GetSongId(key);
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.GetAsync(string.Format("http://player.kuwo.cn/webmusic/st/getNewMuiseByRid?rid=MUSIC_{0}", id));
            string result = await message.Content.ReadAsStringAsync();
            result = result.Replace("&", "&amp;");
            XmlDocument document = new XmlDocument();
            document.LoadXml(result);
            string code = document.SelectSingleNode("//lyric").InnerText;
            return "" + code;

        }

        public async static Task<string> GetLestionUrl(string key)
        {
            string id = await GetSongId(key);
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.GetAsync(string.Format("http://player.kuwo.cn/webmusic/st/getNewMuiseByRid?rid=MUSIC_{0}", id));
            string result = await message.Content.ReadAsStringAsync();
            result = result.Replace("&", "&amp;");
            XmlDocument document = new XmlDocument();
            document.LoadXml(result);
            return  document.SelectSingleNode("//mp3dl").InnerText + "/resource/" + document.SelectSingleNode("//mp3path").InnerText;
        }

        private async static Task<string> GetSongId(string key)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.GetAsync(string.Format("http://sou.kuwo.cn/ws/NSearch?type=music&key={0}", key));
            string result = await message.Content.ReadAsStringAsync();
            HtmlAgilityPack.HtmlDocument hd = new HtmlAgilityPack.HtmlDocument();
            hd.LoadHtml(result);
            HtmlAgilityPack.HtmlNodeCollection li = hd.DocumentNode.SelectNodes("//p [@class='number']");
            string id = li[0].ChildNodes[0].Attributes["value"].Value;
            return id;
        }
    }
}
