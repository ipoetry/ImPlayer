using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Player.FileTypeAssocion
{
    public class TypeRegsiter
    {
        #region public enum HChangeNotifyFlags
        [Flags]
        public enum HChangeNotifyFlags
        {
            SHCNF_DWORD = 0x0003,
            SHCNF_IDLIST = 0x0000,
            SHCNF_PATHA = 0x0001,
            SHCNF_PATHW = 0x0005,
            SHCNF_PRINTERA = 0x0002,
            SHCNF_PRINTERW = 0x0006,
            SHCNF_FLUSH = 0x1000,
            SHCNF_FLUSHNOWAIT = 0x2000
        }
        #endregion//enum HChangeNotifyFlags
        #region enum HChangeNotifyEventID
        [Flags]
        public enum HChangeNotifyEventID
        {
            SHCNE_ALLEVENTS = 0x7FFFFFFF,

            SHCNE_ASSOCCHANGED = 0x08000000,

            SHCNE_ATTRIBUTES = 0x00000800,

            SHCNE_CREATE = 0x00000002,

            SHCNE_DELETE = 0x00000004,

            SHCNE_DRIVEADD = 0x00000100,

            SHCNE_DRIVEADDGUI = 0x00010000,

            SHCNE_DRIVEREMOVED = 0x00000080,

            SHCNE_EXTENDED_EVENT = 0x04000000,

            SHCNE_FREESPACE = 0x00040000,

            SHCNE_MEDIAINSERTED = 0x00000020,

            SHCNE_MEDIAREMOVED = 0x00000040,

            SHCNE_MKDIR = 0x00000008,

            SHCNE_NETSHARE = 0x00000200,

            SHCNE_NETUNSHARE = 0x00000400,

            SHCNE_RENAMEFOLDER = 0x00020000,

            SHCNE_RENAMEITEM = 0x00000001,

            SHCNE_RMDIR = 0x00000010,

            SHCNE_SERVERDISCONNECT = 0x00004000,

            SHCNE_UPDATEDIR = 0x00001000,

            SHCNE_UPDATEIMAGE = 0x00008000,
        }
        #endregion
        [DllImport("shell32.dll")]
        public static extern void SHChangeNotify(HChangeNotifyEventID wEventId, HChangeNotifyFlags uFlags, IntPtr dwItem1, IntPtr dwItem2);
        /// <summary>
        /// 获取.exe,.dll中的ico [PrivateExtractIconsW]是编码方式
        /// </summary>
        /// <param name="lpszFile">文件路径</param>
        /// <param name="nIconIndex">索引</param>
        /// <param name="cxIcon">宽</param>
        /// <param name="cyIcon">高</param>
        /// <param name="phicon">ico句柄</param>
        /// <param name="piconid">ico Id</param>
        /// <param name="nIcons">个数</param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "PrivateExtractIconsW", CharSet = CharSet.Unicode)]
        public static extern int PrivateExtractIcons(string lpszFile, int nIconIndex, int cxIcon, int cyIcon, int[] phicon, int[] piconid, int nIcons, int flags);

        public static Dictionary<string, FileTip> IcoIndex;
        
        /// <summary>
        /// 注册文件关联_调试时使用
        /// </summary>
        /// <param name="exePath">程序</param>
        /// <param name="ResoulePath">关联图标库文件</param>
        /// <param name="fileTypes">要关联的文件类型集合</param>
        public static void Regsiter(string exePath, string ResoulePath, List<string> fileTypes)
        {
            FileTypeRegInfo fti = null;
            FileTip ft = null;
            foreach (string type in fileTypes)
            {
                ft = GetIcoReflect(type);
                string icoPath = ResoulePath + "," + ft.IcoIndex;
                fti = new FileTypeRegInfo { ExePath = exePath, ExtendName = type, IcoPath = icoPath, Description = "Cup Player:" + ft.TypeDescription };
                FileTypeRegister.RegisterFileType(fti);
            }
            FileTypeRegister.RegisterFileType(new FileTypeRegInfo{ExePath = exePath, ExtendName = ".pldb", IcoPath = AppDomain.CurrentDomain.BaseDirectory+"/logo.ico", Description = "Cup Player:播放列表文件"});
            SHChangeNotify(HChangeNotifyEventID.SHCNE_ASSOCCHANGED, HChangeNotifyFlags.SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
        }
        /// <summary}
        /// 注册文件关联_正式使用
        /// </summary>
        /// <param name="fileTypes">欲关联的类型</param>
        public static void Regsiter(List<string> fileTypes)
        {
            string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string ResoulePath = AppDomain.CurrentDomain.BaseDirectory + "Resouce\\Symbian_Anna.dll";

            FileTypeRegInfo fti = null;
            FileTip ft = null;
            foreach (string type in fileTypes)
            {
                ft = GetIcoReflect(type);
                string icoPath = ResoulePath + "," + ft.IcoIndex;
                fti = new FileTypeRegInfo { ExePath = exePath, ExtendName = type, IcoPath = icoPath, Description = "Cup Player:" + ft.TypeDescription };
                FileTypeRegister.RegisterFileType(fti);
            }
            FileTypeRegister.RegisterFileType(new FileTypeRegInfo { ExePath = exePath, ExtendName = "pldb", IcoPath = AppDomain.CurrentDomain.BaseDirectory + "/logo.ico", Description = "Cup Player:播放列表文件" });
            SHChangeNotify(HChangeNotifyEventID.SHCNE_ASSOCCHANGED, HChangeNotifyFlags.SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
        }
        public static FileTip GetIcoReflect(string key)
        {
            if (IcoIndex == null) {
                IcoIndex = new Dictionary<string, FileTip>();
                IcoIndex.Add(".aac", new FileTip(0,"Advanced Audio Codec"));
                IcoIndex.Add(".ape", new FileTip(1, "Monkey's Audio"));
                IcoIndex.Add(".flac",new FileTip(2, "Free Lossless Audio"));
                IcoIndex.Add(".m4a", new FileTip(4, "Apple Lossless Audio Codec"));
                IcoIndex.Add(".mp3", new FileTip(6, "MPEG Layer 3"));
                IcoIndex.Add(".ogg", new FileTip(8, "Ogg Vorbies Audio"));
                IcoIndex.Add(".wav", new FileTip(11, "Winodws Wave"));
                IcoIndex.Add(".wma", new FileTip(12, "Windows Media Audio"));
            }
            return IcoIndex[key];
        }
    }

    public class FileTip {

        public FileTip(int index,string typeDes)
        {
            this.IcoIndex = index;
            this.TypeDescription = typeDes;
        }
        public int IcoIndex { get; set; }
        public string TypeDescription { get; set; }
    }
}
