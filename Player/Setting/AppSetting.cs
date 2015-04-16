using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Player.Setting
{
    [Serializable]
    public class AppSetting
    {
        public double Volume { get; set; }
        public Font LrcFont { get; set; }
        public int SkinIndex { get; set; }
        public string DownloadFolder { get; set; }
        public bool UseEq { get; set; }
        public int EqPreset  { get; set; }

        public void Save()
        {
            try
            {
                if (!Directory.Exists(AppPropertys.dataFolder))
                    Directory.CreateDirectory(AppPropertys.dataFolder);
                using (FileStream stream = File.OpenWrite(System.IO.Path.Combine(AppPropertys.dataFolder, "AppSetting.dat")))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                }
            }
            catch { }
        }
        public static AppSetting Load()
        {
            AppSetting appSetting = null;
            try
            {
                using (FileStream stream = File.OpenRead(System.IO.Path.Combine(AppPropertys.dataFolder, "AppSetting.dat")))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    appSetting = (AppSetting)formatter.Deserialize(stream);
                }
                return appSetting;
            }
            catch
            {
                return new AppSetting { LrcFont=new Font("微软雅黑", 24f, FontStyle.Bold, GraphicsUnit.Pixel), Volume=0.2, DownloadFolder="D:\\", SkinIndex=0, UseEq=false, EqPreset=0};
            }
        }
    }
}
