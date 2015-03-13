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
using ImPalyer.FM.Core;
using ImPalyer.FM;
using Microsoft.Practices.Prism.Commands;
using System.Runtime.InteropServices;
namespace ImPalyer.FM.Views
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
        public ImPalyer.FM.Core.Channel ChannelBLL { get; private set; }
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
            ChannelBLL = new ImPalyer.FM.Core.Channel();
            PublicChannels.ItemsSource = PublicChannelList;
            TempSongList = new List<ImPlayer.FM.Models.FMSong>();
            this.SongBLL = new Core.Song();
        }

        //public void LoadChannels()
        //{
           
        //    Models.ChannelList channelList = null;
        //    ImPalyer.FM.Core.Channel ChannelBLL = new ImPalyer.FM.Core.Channel();
        //    channelList = ChannelBLL.GetChannelList();
        //    foreach (var item in channelList.PublicChannelList)
        //    {
        //        PublicChannelList.Add(item);
        //    }

        //}
        #region 检查网络状态

        //检测网络状态  
        [DllImport("wininet.dll")]
        extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        /// <summary>  
        /// 检测网络状态  
        /// </summary>  
        static bool IsConnected { get { int I = 0; return InternetGetConnectedState(out I, 0); } }

        #endregion   
        /// <summary>
        /// 加载所有频道
        /// </summary>
        public bool LoadChannels()
        {
            if (this.AllChannel != null) return false;
            if (!IsConnected) { Console.WriteLine("网络未连接，无法读取FM"); return false; }
            var action = new Func<ImPlayer.FM.Models.ChannelList>(this.ChannelBLL.GetChannelList);
            action.BeginInvoke(ar =>
            {
                var channelList = action.EndInvoke(ar);
                this.AllChannel = channelList;
                this.InvokeOnUIDispatcher(new Action(() =>
                {

                    foreach (var item in channelList.PublicChannelList)
                    {
                        this.PublicChannelList.Add(item);
                       // PublicChannels.Items.Add(item);
                    }

                    //默认加载一个频道
                    this.CurrentChannel = this.PublicChannelList.OrderBy(t => Guid.NewGuid()).FirstOrDefault();
                }));
            }, null);
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
        /// 搜索DJ电台
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



        #region 命令
        private DelegateCommand<string> _searchCommand;
        /// <summary>
        /// 搜索命令
        /// </summary>
        public DelegateCommand<string> SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new DelegateCommand<string>(this.SearchChannel);
                }
                return _searchCommand;
            }
        }
        private DelegateCommand _reloadCommand;
        /// <summary>
        /// 重新加载频道命令
        /// </summary>
        //public DelegateCommand ReloadCommand
        //{
        //    get
        //    {
        //        if (_reloadCommand == null)
        //        {
        //            _reloadCommand = new DelegateCommand(this.LoadChannels);
        //        }
        //        return _reloadCommand;
        //    }
        //}
        #endregion


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
                LoadSong();
               
		}
        /// <summary>
        /// 加载歌曲
        /// </summary>
        public void LoadSong()
        {
            string songId = null;
            if (this.CurrentSong != null)
            {
                songId = this.CurrentSong.SongId;
            }
            var channelId = this.CurrentChannel.Id;
            var action = new Func<int, string, ImPlayer.FM.Models.FMSongList>(this.SongBLL.GetSongList);
            action.BeginInvoke(channelId, songId, ar =>
            {
                var songList = action.EndInvoke(ar);
                if (songList == null) return;
                TempSongList.Clear();
                TempSongList.AddRange(songList.Songs);
                this.CurrentSong = TempSongList.FirstOrDefault();
                if (StartPlayEventHandler != null)
                {
                    StartPlayEventHandler();
                }
            }, null);
        }

       
        //public static void InitContainer(IUnityContainer unityContainer)
        //{
        //    if (StaticUnityContainer == null)
        //    {
        //        StaticUnityContainer = unityContainer;
        //    }
        //}
        #endregion
    }
}
