using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Xml;
using Lyrics;

namespace Player
{
   public  class SongList
    {
        private XmlDocument dom;  //Xml文档
        private bool isError=false;   //是否有错误
        private string errorText="";   //错误信息
        private string filePath=string.Empty;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filename">音乐列表文件名</param>
        public SongList(string filename)
        {

            dom = new XmlDocument();
            LoadXml(filename);
        }
        /// <summary>
        /// 构造函数重载+1
        /// </summary>
        public SongList()
        {
            dom = new XmlDocument();
        }
        /// <summary>
        /// 载入指定的音乐列表xml文件
        /// </summary>
        /// <param name="path">列表文件名</param>
        public void LoadXml(string path)
        {
            if (!File.Exists(path))
            {
                return;
            } 
            dom.Load(path);
        }
         /// <summary>
        /// 设置列表错误信息
        /// </summary>
        /// <param name="errortext"></param>
        /// <returns></returns>
        public void SetErrorInformation(string errortext)
        {
            isError = true;
            errorText =errortext;
        }
        /// <summary>
        /// 获取列表错误信息
        /// </summary>
        /// <returns></returns>
        public string GetErrorInformation()
        {
            if (isError == true)
            {
                return errorText;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取文档根节点
        /// </summary>
        /// <returns></returns>
        private XmlElement GetXmlRootElement()
        {
           return dom.DocumentElement;
        }
        /// <summary>
        /// 获取列表的名称
        /// </summary>
        public string GetXmlMusicListName()
        {
            XmlElement root=GetXmlRootElement();
            return root.GetAttribute("name");
        }
        /// <summary>
        /// 获取xml文件中所有的音乐子项
        /// </summary>
        /// <returns></returns>
        private XmlNodeList GetXmlMusicItems()
        {
          XmlElement root= GetXmlRootElement();//获取根节点
          XmlNode rootn = XmlConverter(root);//转换为XmlNode
         // XmlNode xn= rootn.SelectSingleNode("SongList");
          return rootn.SelectNodes("Song");//选定返回所有Music节点
        }
        /// <summary>
        /// 获取列表中的音乐总数
        /// </summary>
        /// <returns></returns>
        public int GetXmlCount()
        {

            XmlElement xe= GetXmlRootElement();
            return xe.ChildNodes.Count;
        }

        /// <summary>
        /// 获取指定音乐节点
        /// </summary>
        /// <param name="index">序号</param>
        /// <returns>返回XmlNode对象</returns>
        private XmlNode  GetXmlMusicItemByIndex(int index)
        {
            XmlNodeList xnl = GetXmlMusicItems();
            return xnl.Item(index);
        }
        /// <summary>
        /// 获取指定音乐节点的所有信息
        /// </summary>
        /// <param name="index">序号</param>
        /// <returns>返回XmlListProduct对象</returns>
        public Song GetXmlMusicItemInformation(int index)
        {
            XmlNode xn=  GetXmlMusicItemByIndex(index);
            XmlElement xe = XmlConverter(xn);
            Song song = new Song(xe.GetAttribute("url"), xe.InnerText);
            return song;
        }
        public Song GetXmlMusicItemInformation(XmlNode xn)
        {
            XmlElement xe = XmlConverter(xn);
            Song xlp = new Song(xe.GetAttribute("url"), xe.InnerText);
            xlp.Album = xe.GetAttribute("album");
            xlp.Artist = xe.GetAttribute("artist");
            xlp.Title = xe.GetAttribute("title");
            xlp.Duration = TimeSpan.FromSeconds(Convert.ToDouble(xe.GetAttribute("duration")));
            xlp.PicUrl =Common.Common.GetRunDir()+@"Album\"+ xe.GetAttribute("pic");
            xlp.Size = xe.GetAttribute("size");
            return xlp;
        }


        /// <summary>
        /// 将XmlElement转换为XmlNode
        /// </summary>
        /// <param name="xe">要转换的XmlElement</param>
        /// <returns></returns>
        private XmlNode XmlConverter(XmlElement xe)
        {
            return (XmlNode)xe;
        }
        /// <summary>
        /// 将XmlNode转换为XmlElement
        /// </summary>
        /// <param name="xn">要转换的XmlNode</param>
        /// <returns></returns>
        private XmlElement XmlConverter(XmlNode xn)
        {
            return (XmlElement)xn;
        }
        /// <summary>
        /// 获取List类型列表
        /// </summary>
        /// <returns></returns>
        public  List<Song> getSongList()
        {
            List<Song> List = new List<Song>();
            foreach (XmlNode xn in GetXmlMusicItems())
            {
               List.Add(GetXmlMusicItemInformation(xn));
            }
            return List;
        }
        /// <summary>
        /// 获取继承自ObservableCollection类型类表
        /// </summary>
        /// <returns></returns>
        public Player.PlayController.OC_Songs getICSongList()
        {
            Player.PlayController.OC_Songs songs = new PlayController.OC_Songs();
            foreach (XmlNode xn in GetXmlMusicItems())
            {
                songs.Add(GetXmlMusicItemInformation(xn));
            }
            return songs;
        }

        private Byte[] ImageToStream(Image Pic)
        {
            using(System.IO.MemoryStream Ms = new MemoryStream())
            {
                Pic.Save(Ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] img = new byte[Ms.Length];
                Ms.Position = 0;
                Ms.Read(img, 0, Convert.ToInt32(Ms.Length));
                Ms.Close();
                return img;
            }
        }

        public void RemoveNode(string[] xns)
        {
            foreach (XmlNode xn in GetXmlMusicItems())
            {
                if (xns.Contains(xn.InnerText))
                {
                   xn.ParentNode.RemoveChild(xn);
                }
            }
            dom.Save(AppPropertys.appPath + @"\PlayList.pldb");
        }
    }
}
