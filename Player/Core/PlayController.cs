﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Lyrics;
using Player.Controls;
using BassCore;
using System.Collections.ObjectModel;
using Player.NotifyBall;
using ArtistPic;
using System.IO;
namespace Player
{
  public partial class PlayController:DependencyObject
    {
      public static PlayOperation playControl;
      public static DiyContextMenu contextMenu;
      public static DispatcherTimer DT = new DispatcherTimer();
      public static BassEngine bassEng;
      public static SongList sl;
      public static EqualizerSet EQS;
      public class OC_Songs:ObservableCollection<Song>{};
      public static int PlayMode = 1;
      public static bool isFM = false;

      //public static readonly DependencyProperty CurrentSongProperty = DependencyProperty.Register("CurrentSong", typeof(Song), typeof(PlayController), new PropertyMetadata(new PropertyChangedCallback((d, e) =>
      //{
      //    (d as PlayController).RaiseCurrentSongChangedEvent();
      //})));

      public  readonly DependencyProperty CurrentPostionProperty = DependencyProperty.Register("CurrentPostion", typeof(double), typeof(PlayController));

      public static void Initialize()
      {
          EQS = new EqualizerSet();
          bassEng = BassEngine.Instance;
          playControl = AppPropertys.mainWindow.playControl1;
          playControl.btnPlay.Style =(Style)playControl.FindResource("play");
          playControl.btnMute.Style = (Style)playControl.FindResource("notMute");
          sl = new SongList();
          bassEng.TrackEnded += bassPlayer_TrackEnded;
          DT.Interval = TimeSpan.FromMilliseconds(100);
          DT.Tick += new EventHandler(DT_Tick);
          LrcController.ButtonChanged+=new LrcController.ButtonChangedHandle(LrcController_ButtonChanged);
          bassEng.OpenSucceeded += new EventHandler(StartPlay);
      }

      private static void LrcController_ButtonChanged(object sender, LrcController.ButtonChangeEventArgs e)
      {
          switch (e.ButtonIndex)
          {
              case 0:
                  AppPropertys.notifyIcon_MouseDoubleClick(sender, null);
                  break;
              case 1:
                  PlayPrevent();
                  break;
              case 2:
                 Pause();
                  break;
              case 3:
                   PlayMusic();
                  break;
              case 4:
                  PlayNext();
                  break;
              case -1:
                  AppPropertys.isLrcShow=false;
                  LrcController.CloseLrc();
                  break;
          }
      }
      private static void DT_Tick(object sender, EventArgs e)
      {
            if(bassEng.IsPlaying)
            {
                StartPosition = bassEng.ChannelPosition.TotalSeconds;
                if (startPosition != 0)
                {
                    if(AppPropertys.isLrcShow)
                    LrcController.lrcWindow.showLrc(startPosition * 1000 + LrcController.offsetTime);
                }
         }
      }

      static bool isEQShow = false;
      public static void ShowSetEQ()
      {
          EQS.ShowInTaskbar = false;
          if (EQS == null || EQS.IsLoaded) { EQS = new EqualizerSet(); }
          if (!isEQShow)
          {
              EQS.Show(); isEQShow = true;
          }
          else
          {
              EQS.Hide(); isEQShow = false;
          }
      }
     static void  bassPlayer_TrackEnded(object sender, EventArgs e)
      {
          #region 复位控件
          ReSet();
          #endregion
          PlayNext();
      }



      private static OC_Songs songs = new OC_Songs();
      /// <summary>
      /// 继承自ObservableCollection
      /// </summary>
      public static OC_Songs Songs
      {
          get
          {
              return songs;
          }
          set
          {
              songs = value;
          }
      }

      ///// <summary>
      ///// 当前歌曲
      ///// </summary>
      //public  Song CurrentSong
      //{
      //    get { return (Song)GetValue(CurrentSongProperty); }
      //    protected set { SetValue(CurrentSongProperty, value); }
      //}
      private static Song currentSong;
      public static Song CurrentSong
      {
          get{return currentSong;}
          set { currentSong = value; AppPropertys.mainWindow.CurrentShow.Dispatcher.Invoke(new Action(() => { AppPropertys.mainWindow.CurrentShow.FontSize = 16; AppPropertys.mainWindow.CurrentShow.Text = CurrentSong.ArtSong; })); }
      }

      /// <summary>
      /// 当前位置  
      /// </summary>
      public Double CurrentPostion
      {
          get { return (Double)GetValue(CurrentPostionProperty); }
          protected set { SetValue(CurrentPostionProperty, value); }
      }

      private static int playIndex = 0;
      public static int PlayIndex
      {
          get
          {
              return playIndex;
          }

          set
          {
              playIndex = value;
          }
      }

      private static double startPosition = 0;
      public static double StartPosition
      {
          get
          {
              return startPosition;
          }
          set
          {
              startPosition = value;
              if (duringTime > 0)
              {
                  double f = startPosition / duringTime * playControl.ChanelLength.Width;
                  Canvas.SetLeft(playControl.thumb2, f);
                  playControl.CurLen.Width = f;
              }
            //  playControl.startTime.Text = formatTime(startPosition);
          }
      }

      public static double duringTime = 0;
      public static double DuringTime
      {
          get
          {
              return duringTime;
          }
          set
          {
              duringTime = value;
          }
      }

      public static void ReSet()
      {
          DT.Stop();
          startPosition = 0;
          duringTime = 0;
          playControl.thumb2.Dispatcher.Invoke(new Action(()=>Canvas.SetLeft(playControl.thumb2, 0)));
         // playControl.startTime.Text = "00:00";
        //  playControl.totalTime.Text = "00:00";
          LrcController.offsetTime = 0;
          AppPropertys.FlushMemory();
         
      }

      public static void setMute()
      {
       //   bassEng.IsMuted = !bassEng.IsMuted;
          if (bassEng.IsMuted)
          {
              playControl.btnMute.Style = (Style)playControl.FindResource("notMute");
              playControl.btnMute.ToolTip = "打开声音";
              bassEng.IsMuted = false;
              bassEng.Volume = 35;
          }
          else
          {

              playControl.btnMute.Style = (Style)playControl.FindResource("Mute");
              playControl.btnMute.ToolTip = "静音";
              bassEng.IsMuted = true;
              bassEng.Volume = 35;
          }
      }

      public static void PlayMusic()
      {
          if (songs.Count > playIndex && playIndex > -1)
          {
              ReSet();
              CurrentSong = songs[playIndex];
              if (CurrentSong.FileUrl.StartsWith("http:"))
              {
                  bassEng.OpenUrlAsync(CurrentSong.FileUrl);
              }
              else
              {
                  if (!RemoveNotExitsFile(CurrentSong)) { return; }
                  bassEng.OpenFile(CurrentSong.FileUrl);
              }
             
          }

      }
      private static void StartPlay(object sender,EventArgs e)
      { 
            Play();
            LrcController.searchLrc(CurrentSong);
            AppPropertys.ChangeNotifyIcon(2);
      }
      private static bool RemoveNotExitsFile(Song rSong)
      {
          if (!File.Exists(rSong.FileUrl))
          { 
              sl.RemoveNode(new string[]{rSong.FileName});
              Songs.Remove(rSong);
              return false;
          }
          return true;
      }
      public static void Play()
      {
          bassEng.Play();
          DT.Start();
          playControl.btnPlay.Style = (Style)playControl.FindResource("pause");
          BalloonSongInfo bsi = new BalloonSongInfo();
          bsi.ShowCoverSmooth();
          AppPropertys.mainWindow.playListBox.SelectedIndex = playIndex;
          AppPropertys.mainWindow.playListBox.ScrollIntoView(AppPropertys.mainWindow.playListBox.Items[playIndex]);
          string notifyIconText = "正在播放：" + CurrentSong.Artist + " - " + CurrentSong.Title;
          if (notifyIconText.Length >= 64) { notifyIconText.Substring(0,63); }
          AppPropertys.notifyIcon.Text = notifyIconText;
          playControl.btnPlay.ToolTip = "停止";
          LrcController.setPause();
          if (AppPropertys.mainWindow.isPPTPlaying)
          {
              AppPropertys.mainWindow.PlayPPT(CurrentSong);
          }
          try
          {
              Player.NotifyBall.BalloonSongInfo ss = new Player.NotifyBall.BalloonSongInfo();
              AppPropertys.mainWindow.NotifyTip.ShowCustomBalloon(ss, System.Windows.Controls.Primitives.PopupAnimation.Fade, 5000);
          }
          catch (Exception ex) { Console.WriteLine(ex.ToString()); }
          
      }

      public static void Stop()
      {
          bassEng.Stop();
          LrcController.setPlay();
          playControl.btnPlay.ToolTip = "播放";
          AppPropertys.notifyIcon.Text = AppPropertys.logoText;
      }

      public static void Pause()
      {
          bassEng.Pause();
          playControl.btnPlay.Dispatcher.Invoke(new Action(()=>playControl.btnPlay.Style = (Style)playControl.FindResource("play")));
          playControl.btnPlay.Dispatcher.Invoke(new Action(()=>playControl.btnPlay.ToolTip = "播放"));
          LrcController.setPlay();
          AppPropertys.ChangeNotifyIcon(1);
      }

      public static void PlayPrevent()
      {
          playIndex--;
          if (playIndex < 0)
              playIndex = songs.Count - 1;
          PlayMusic();
      }

      public static void PlayNext()
      {
          switch (PlayMode)
          {
              case 1:
                  playIndex++;
                  if (playIndex > songs.Count - 1)
                      playIndex = 0;
                  break;
              case 2:
                  Random rand = new Random();
                  playIndex = rand.Next(songs.Count);
                  break;
              case 3:
                  break;

          } 
          PlayMusic();
      }

      public static string formatTime(double seconds)
      {
          int m = (int)seconds / 60;
          int s = (int)seconds % 60;
          string t = (m > 9 ? m.ToString() : "0" + m.ToString()) + ":" + (s > 9 ? s.ToString() : "0" + s.ToString());
          return t;
      }



    }
}