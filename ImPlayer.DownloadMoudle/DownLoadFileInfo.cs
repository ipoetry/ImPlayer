using MyDownloader.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImPlayer.DownloadMoudle
{
    public class DownLoadFileInfo:MyNotifyPropertyChaged
    {
        public delegate void DownLoadCompleted();
        public event DownLoadCompleted OnDownloadCompletedHandler;

        private string songId;
        public string SongId { get { return songId; } set { songId = value; NotifyPropertyChanged("songId"); } }
        private string fileName;
        public string FileName { get { return fileName; } set { fileName = value; NotifyPropertyChanged("FileName"); } }
        private string fileSize;
        public string FileSize { get { return fileSize; } set { fileSize = value; NotifyPropertyChanged("FileSize"); } }
        private string fileLink;
        public string FileLink { get { return fileLink; } set { fileLink = value; NotifyPropertyChanged("FileLink"); } }
        private string fileAlbum;
        public string FileAlbum { get { return fileAlbum; } set { fileAlbum = value; NotifyPropertyChanged("FileAlbum"); } }
        private double downloadProces;
        public double DownloadProcess
        {
            get { return downloadProces; }
            set
            {
                downloadProces = value; NotifyPropertyChanged("DownloadProcess");
                if (value == 100d && OnDownloadCompletedHandler != null)
                {
                    OnDownloadCompletedHandler();
                }
            }
        }
        private DownloaderState downloadState;
        public DownloaderState DownloadState { get { return downloadState; } set { downloadState = value; NotifyPropertyChanged("DownloadState"); } }
    }

    // [Serializable, DataContract(IsReference = true)]
    public class MyNotifyPropertyChaged : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

    }
}
