using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win8Toast
{
    public class SystemHepler
    {
        public static bool IsWin8OrHeigher()
        {
            Version currentVersion = Environment.OSVersion.Version;
            return currentVersion.CompareTo(new Version("6.2")) >= 0?true:false;
        }

        public static string autoStart = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\";
    }
}
