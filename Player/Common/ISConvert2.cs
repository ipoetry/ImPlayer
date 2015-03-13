using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Player.Common
{
    [ValueConversion(typeof(String), typeof(String))]
    public class ISConvert2 : IValueConverter
    {
        #region IValueConverter 成员

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string path = value.ToString();
            if (!path.StartsWith("http:"))
            { 
                if (path == "" || !File.Exists(path))
                {
                   // path = AppPropertys.appPath + @"..Images\NoCover.png";
                    path = "/Player;component/Images/NoCover.png";
                }
            }
            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
