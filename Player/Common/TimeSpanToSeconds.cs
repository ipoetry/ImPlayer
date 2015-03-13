using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Player.Common
{
     [ValueConversion(typeof(TimeSpan), typeof(double))]
    public class TimeSpanToSeconds:IValueConverter
    {

        #region IValueConverter 成员

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan date = (TimeSpan)value;
            double duration = date.TotalSeconds;
            return duration*(510/PlayController.bassEng.ChannelLength.TotalSeconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return TimeSpan.FromSeconds((double)value * (PlayController.bassEng.ChannelLength.TotalSeconds / 510));
        }

        #endregion
    }
}
