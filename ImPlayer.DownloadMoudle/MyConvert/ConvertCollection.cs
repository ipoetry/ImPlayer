using MyDownloader.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ImPlayer.DownloadMoudle.MyConvert
{
    class DoubleToWidth : IValueConverter
    {

        #region IValueConverter 成员
         
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value!=null?double.Parse(value.ToString()) / 180:0d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    class DoubleToString : IValueConverter
    {

        #region IValueConverter 成员

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return  value.ToString()+"%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    class MyConvert : IValueConverter
    {

        #region IValueConverter 成员

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int bit=int.Parse(value.ToString());
            if(bit>320){return "SQ无损音质";}
            else if(bit==256){return "高品质";}
            else if(bit==320){return "HQ超高音质";}
            else {return "一般";}
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    class ByteToMB : IValueConverter
    {
        #region IValueConverter 成员

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (double.Parse(value.ToString()) / 1024 / 1024).ToString("F2") + "MB";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    class LinkToMark : IMultiValueConverter
    {

        #region IMultiValueConverter 成员

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.IsNullOrEmpty(values[0].ToString())?values[1].ToString():values[0].ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    class CurrentState : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DownloaderState ds = ((DownloaderState)value);
            string currentStateDescribe = string.Empty;
            switch (ds)
            {
                case DownloaderState.NeedToPrepare: currentStateDescribe = "开始"; break;
                case DownloaderState.WaitingForReconnect:
                case DownloaderState.Preparing: currentStateDescribe = "准备中…"; break;
                case DownloaderState.Prepared: currentStateDescribe = "开始"; break;
                case DownloaderState.Pausing: currentStateDescribe = "暂停中…"; break;
                case DownloaderState.Paused: currentStateDescribe = "继续"; break;
                case DownloaderState.Working: currentStateDescribe = "暂停"; break;
                case DownloaderState.Ended: currentStateDescribe = "播放"; break;
                case DownloaderState.EndedWithError: currentStateDescribe = "发生错误…"; break;
            }
            return currentStateDescribe;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class IsEnded : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DownloaderState ds = ((DownloaderState)value);
            string currentStateDescribe = string.Empty;
            switch (ds)
            {
                case DownloaderState.NeedToPrepare:
                case DownloaderState.WaitingForReconnect:
                case DownloaderState.Preparing:
                case DownloaderState.Prepared:
                case DownloaderState.Pausing:
                case DownloaderState.Paused:
                case DownloaderState.Working: currentStateDescribe = "移除"; break;
                case DownloaderState.Ended:
                case DownloaderState.EndedWithError: currentStateDescribe = "删除"; break;
            }
            return currentStateDescribe;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class AddKbps : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "[" +value.ToString() + "kbps]";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
