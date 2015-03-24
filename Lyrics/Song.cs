using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Lyrics
{
	/// <summary>
	/// 歌曲
	/// </summary>
    public class Song : ICloneable, IEquatable<Song>,INotifyPropertyChanged  
	{
		/// <summary>
		/// 音乐文件路径
		/// </summary>
		public string FileUrl { get; set; }
        /// <summary>
        /// 音乐文件名
        /// </summary>
        public string FileName { get; set; }
		/// <summary>
		/// 标题
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 音乐家
		/// </summary>
		public string Artist { get; set; }
		/// <summary>
		/// 专辑
		/// </summary>
		public string Album { get; set; }
		/// <summary>
		/// 唱片公司
		/// </summary>
		public string Company { get; set; }
		/// <summary>
		/// 封面URL
		/// </summary>
		public string PicUrl { get; set; }
        ///// <summary>
        ///// 封面
        ///// </summary>
        //public byte[] Picture { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        public BitmapImage Picture { get; set; }
		/// <summary>
		/// 长度
		/// </summary>
        public TimeSpan Duration { get; set; }
		/// <summary>
		/// 发行日期
		/// </summary>
		public string PublicDate { get; set; }
		/// <summary>
		/// 爱好标记
		/// </summary>
		public bool isLike { get; set; }
        /// <summary>
        /// 比特率
        /// </summary>
        public string Rate { get; set; }
        /// <summary>
        /// 文件大小 MB
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// 文件大小 MB
        /// </summary>
        private string artSong;
        public string ArtSong { get { if (Title == "" && Artist == "") { return FileName; } return Artist + " - " + Title; } }

        public Song()
        {

        }

        public Song(string path,string name)
        {
            this.FileUrl = path;
            this.FileName = name;
        }

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public bool Equals(Song other)
		{
			if (Object.ReferenceEquals(other, null))
				return false;
			if (Object.ReferenceEquals(this, other))
				return true;
			return this.FileUrl == other.FileUrl;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Song);
		}

		public override int GetHashCode()
		{
			if (FileUrl == null) return 0;
			return FileUrl.GetHashCode();
		}

        public event PropertyChangedEventHandler PropertyChanged;
    }
}