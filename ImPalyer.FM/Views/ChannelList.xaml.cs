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
using ImPlayer.FM.Models;
using ImPlayer.FM;
namespace ImPlayer.FM.Views
{
    /// <summary>
    /// ChannelList.xaml 的交互逻辑
    /// </summary>
    public partial class ChannelList : UserControl
    {
        public ChannelList()
        {
            InitializeComponent();
            PublicChannelList = new ObservableCollection<Channel>();
            DJChannelList = new ObservableCollection<Channel>();
        }
        /// <summary>
        /// 所有频道
        /// </summary>
        public Models.ChannelList AllChannel { get; private set; }       
        /// <summary>
        /// 公共频道列表
        /// </summary>
        public ObservableCollection<Models.Channel> PublicChannelList { get; private set; }
        /// <summary>
        /// DJ频道列表
        /// </summary>
        public ObservableCollection<Models.Channel> DJChannelList { get; private set; }
        /// <summary>
        /// 频道操作类
        /// </summary>
        public ImPalyer.FM.Core.Channel ChannelBLL { get; private set; }

        private Channel _currentChannel;
        /// <summary>
        /// 当前频道
        /// </summary>
        public Channel CurrentChannel
        {
            get { return _currentChannel; }
            set
            {
                if (_currentChannel != value)
                {
                    _currentChannel = null;
                    //this.RaisePropertyChanged("CurrentChannel");
                    //_currentChannel = value;
                    //this.RaisePropertyChanged("CurrentChannel");

                    //this.EventAggregator.GetEvent<ChangeChannelEvent>().Publish(value);
                }
            }
        }
        /// <summary>
        /// 频道操作类
        /// </summary>

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ChannelBLL = new ImPalyer.FM.Core.Channel();
            LoadChannels();
            PublicChannels.ItemsSource = PublicChannelList;
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

        /// <summary>
        /// 加载所有频道
        /// </summary>
        public void LoadChannels()
        {
            if (this.AllChannel != null) return;
            var action = new Func<Models.ChannelList>(this.ChannelBLL.GetChannelList);
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
        public void OnChooseSearchResult(Models.SearchResult result)
        {
            if (result == null) return;
            var channel = new Models.Channel() { Id = 0, Name = result.Title, Context = result.Context };
            this.CurrentChannel = channel;
        }

        #region 方法
        protected void InvokeOnUIDispatcher(Delegate callback, params object[] args)
        {
            Application.Current.Dispatcher.BeginInvoke(callback, args);
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
