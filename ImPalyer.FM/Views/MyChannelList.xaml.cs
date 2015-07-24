using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImPlayer.FM.Core;
using ImPlayer.FM;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ImPlayer.FM.Views
{
    
    /// <summary>
    /// MyChannelList.xaml 的交互逻辑
    /// </summary>
    public partial class MyChannelList : UserControl
    {        
        public   delegate void StartPlayDel();
        public  event StartPlayDel StartPlayEventHandler;

        public MyChannelList()
        {
            InitializeComponent(); PublicChannelList = new ObservableCollection<ImPlayer.FM.Models.Channel>();
            DJChannelList = new ObservableCollection<ImPlayer.FM.Models.Channel>();
        }

        public static readonly DependencyProperty CurrentChannelProperty = DependencyProperty.Register("CurrentChannel", typeof(Channel), typeof(MyChannelList));

        /// <summary>
        /// 所有频道
        /// </summary>
        public ImPlayer.FM.Models.ChannelList AllChannel { get; private set; }       
        /// <summary>
        /// 公共频道列表
        /// </summary>
        public ObservableCollection<ImPlayer.FM.Models.Channel> PublicChannelList { get; private set; }
        /// <summary>
        /// DJ频道列表
        /// </summary>
        public ObservableCollection<ImPlayer.FM.Models.Channel> DJChannelList { get; private set; }
        /// <summary>
        /// 频道操作类
        /// </summary>
        public ImPlayer.FM.Core.Channel ChannelBLL { get; private set; }
        /// <summary>
        /// 歌曲操作类
        /// </summary>
        public Core.Song SongBLL { get; set; }

        private ImPlayer.FM.Models.Channel _currentChannel;
        /// <summary>
        /// 当前频道
        /// </summary>
        public ImPlayer.FM.Models.Channel CurrentChannel
        {
            get { return _currentChannel; }
            set
            {
                if (_currentChannel != value)
                {
                   // _currentChannel = null;
                    //this.RaisePropertyChanged("CurrentChannel");
                    _currentChannel = value;
                    //this.RaisePropertyChanged("CurrentChannel");

                    //this.EventAggregator.GetEvent<ChangeChannelEvent>().Publish(value);
                }
            }
        }

        private ImPlayer.FM.Models.FMSong _currentSong;
        /// <summary>
        /// 当前在听的歌曲
        /// </summary>
        public ImPlayer.FM.Models.FMSong CurrentSong
        {
            get { return _currentSong; }
            set
            {
                if (_currentSong != value)
                {
                    _currentSong = value;
                }
            }
        }

        /// <summary>
        /// 临时保存的歌曲列表
        /// </summary>
        public static List<ImPlayer.FM.Models.FMSong> TempSongList
        {
            get;
            set;
        }


        /// <summary>
        /// 频道操作类
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ChannelBLL = new ImPlayer.FM.Core.Channel();
            PublicChannels.ItemsSource = PublicChannelList;
            TempSongList = new List<ImPlayer.FM.Models.FMSong>();
            this.SongBLL = new Core.Song();
        }

        /// <summary>
        /// 加载所有频道
        /// </summary>
        public async Task<bool> LoadChannels()
        {
            if (this.AllChannel != null||! await ImPlayer.Toast.PopupTip.CheckNetWork()) 
                return false;
            var channelList = await this.ChannelBLL.GetChannelListAsync();
            if (channelList == null)
            { 
                Console.WriteLine("获取频道列表出错"); 
                return false;
            }
            this.AllChannel = channelList;
            this.InvokeOnUIDispatcher(new Action(() =>
            {

                foreach (var item in channelList.PublicChannelList)
                {
                    this.PublicChannelList.Add(item);
                }
                //默认加载一个频道
                this.CurrentChannel = this.PublicChannelList.OrderBy(t => Guid.NewGuid()).FirstOrDefault();
            }));
            return true;   
        }

        /// <summary>
        /// 清除频道
        /// </summary>
        /// <returns></returns>
        public void ClearChannels()
        {
            if (this.PublicChannelList.Count > 0) { this.AllChannel = null; this.PublicChannelList.Clear(); }
        }
        /// <summary>
        /// 搜索电台
        /// </summary>
        /// <param name="keywords"></param>
        public void SearchChannel(string keywords)
        {
            if (this.AllChannel == null) return;
            if (string.IsNullOrEmpty(keywords)) return;
            this.DJChannelList.Clear();
            var result = this.AllChannel.DJChannelList.Where(t => t.Name.Contains(keywords));
            foreach (var item in result)
            {
                this.DJChannelList.Add(item);
            }
        }
        /// <summary>
        /// 在选中搜索结果时触发的方法
        /// </summary>
        /// <param name="result"></param>
        public void OnChooseSearchResult(ImPlayer.FM.Models.SearchResult result)
        {
            if (result == null) return;
            var channel = new ImPlayer.FM.Models.Channel() { Id= 0, Name = result.Title, Context = result.Context };
            this.CurrentChannel = channel;
        }


        #region 方法
        protected void InvokeOnUIDispatcher(Delegate callback, params object[] args)
        {
            Application.Current.Dispatcher.BeginInvoke(callback, args);
        }

        /// <summary>
		/// 更换公共频道
		/// </summary>
		private void PublicChannels_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (PublicChannels.SelectedItem != null)
                CurrentChannel = (ImPlayer.FM.Models.Channel)PublicChannels.SelectedItem;
              LoadSongAsync();
               
		}

        /// <summary>
        /// 异步加载歌曲
        /// </summary>
        public async void LoadSongAsync()
        {
            string songId = this.CurrentSong != null?CurrentSong.SongId:null;
            var channelId = this.CurrentChannel.Id;
            var songList = await this.SongBLL.GetSongListAsync(channelId, songId);
            if (songList == null) { Console.WriteLine("加载频道歌曲列表出错"); return; }
            TempSongList.Clear();
            TempSongList.AddRange(songList.Songs);
            this.CurrentSong = TempSongList.FirstOrDefault();
            if (StartPlayEventHandler != null)
            {
                StartPlayEventHandler();
            }
        }
        #endregion
    }
}
