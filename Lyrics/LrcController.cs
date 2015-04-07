using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
namespace Lyrics
{
    public class LrcController
    {
        private static System.Drawing.Color bgDownColor = System.Drawing.Color.DarkGray;
        private static System.Drawing.Color bgUPColor = System.Drawing.Color.LightGray;
        private static Font defaultFont = new Font("微软雅黑", 24f, FontStyle.Bold, GraphicsUnit.Pixel);
        private static string defaultText = "Love Life，Love Music！";
        public static LrcToolBar lrcToolBar;
        public static string[] LrcTypes = new string[] { "*.Qrc", "*.Ksc", "*.Lrc" };
        public static LrcWindow lrcWindow;
        private static Lrc lyric = new Lrc();
        private static System.Drawing.Color maskDownColor = System.Drawing.Color.DarkRed;
        private static System.Drawing.Color maskUPColor = System.Drawing.Color.DarkOrange;
        public static int offsetTime = 0;
        private static System.Drawing.Color penColor = System.Drawing.Color.Black;
        private static int penWidth = 2;
        public static int SkinIndex = 0;

        #region
        public static System.Drawing.Color BgDownColor
        {
            get
            {
                return bgDownColor;
            }
            set
            {
                bgDownColor = value;
            }
        }

        public static System.Drawing.Color BgUPColor
        {
            get
            {
                return bgUPColor;
            }
            set
            {
                bgUPColor = value;
            }
        }

        public static Font DefaultFont
        {
            get
            {
                return defaultFont;
            }
            set
            {
                defaultFont = value;
            }
        }

        public static Lrc Lyric
        {
            get
            {
                return lyric;
            }
            set
            {
                lyric = value;
            }
        }

        public static System.Drawing.Color MaskDownColor
        {
            get
            {
                return maskDownColor;
            }
            set
            {
                maskDownColor = value;
            }
        }

        public static System.Drawing.Color MaskUPColor
        {
            get
            {
                return maskUPColor;
            }
            set
            {
                maskUPColor = value;
            }
        }

        public static System.Drawing.Color PenColor
        {
            get
            {
                return penColor;
            }
            set
            {
                penColor = value;
            }
        }

        public static int PenWidth
        {
            get
            {
                return penWidth;
            }
            set
            {
                penWidth = value;
            }
        }
        #endregion

        public static event ButtonChangedHandle ButtonChanged;

        public static void ChangeColor(int index)
        {
            switch (index)
            {
                case 0:
                    bgUPColor = System.Drawing.Color.LightGray;
                    bgDownColor = System.Drawing.Color.DarkGray;
                    break;

                case 1:
                    bgUPColor = System.Drawing.Color.LightBlue;
                    bgDownColor = System.Drawing.Color.DarkBlue;
                    break;

                case 2:
                    bgUPColor = System.Drawing.Color.LightGreen;
                    bgDownColor = System.Drawing.Color.DarkGreen;
                    break;
            }
            if (lrcWindow != null)
            {
                lrcWindow.Update();
            }
        }

        public static Bitmap getBGBMP(string text)
        {
            Bitmap bitmap;
            string[] strArray = text.Split(new char[] { ',' });
            int width = 0;
            int height = 0;
            int num3 = 0;
            int num4 = 0;
            List<Bitmap> list = new List<Bitmap>();
            for (int i = 0; i < strArray.Length; i++)
            {
                string str = strArray[i];
                if (string.IsNullOrEmpty(str.Trim()))
                {
                    str = "*";
                }
                PointF origin = new PointF(0f, 0f);
                CharacterRange[] ranges = new CharacterRange[] { new CharacterRange(0, str.Length) };
                new StringFormat().SetMeasurableCharacterRanges(ranges);
                int emHeight = defaultFont.FontFamily.GetEmHeight(defaultFont.Style);
                int cellAscent = defaultFont.FontFamily.GetCellAscent(defaultFont.Style);
                int cellDescent = defaultFont.FontFamily.GetCellDescent(defaultFont.Style);
                int lineSpacing = defaultFont.FontFamily.GetLineSpacing(defaultFont.Style);
                int num10 = (int)Math.Round((double)((defaultFont.Size * (lineSpacing - cellAscent)) / ((float)emHeight)));
                GraphicsPath path = new GraphicsPath();
                path.AddString(str, defaultFont.FontFamily, (int)defaultFont.Style, (defaultFont.Size * 96f) / 72f, origin, StringFormat.GenericDefault);
                Console.WriteLine( str + path.GetBounds());
                System.Drawing.Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(path.GetBounds(), bgUPColor, bgDownColor, LinearGradientMode.Vertical);
                if (i < (strArray.Length - 1))
                {
                    num3 = (int)Math.Round((double)((path.GetBounds().Width + path.GetBounds().X) + (path.GetBounds().X / 2f)));
                }
                else
                {
                    num3 = (int)Math.Round((double)(path.GetBounds().Width + (path.GetBounds().X * 2f)));
                }
                num4 = (int)Math.Round((double)(path.GetBounds().Height + (path.GetBounds().Y * 2f)));
                bitmap = new Bitmap(num3, num4);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawPath(new System.Drawing.Pen(penColor, (float)penWidth), path);
                graphics.FillPath(brush, path);
                path.Dispose();
                brush.Dispose();
                graphics.Dispose();
                list.Add(bitmap);
                if (num4 > height)
                {
                    height = num4;
                }
                width += num3;
            }
            bitmap = new Bitmap(width, height);
            Graphics graphics2 = Graphics.FromImage(bitmap);
            width = 0;
            foreach (Bitmap bitmap2 in list)
            {
                graphics2.DrawImage(bitmap2, new Point(width, 0));
                width += bitmap2.Width;
                bitmap2.Dispose();
            }
            graphics2.Dispose();
            list.Clear();
            return bitmap;
        }

        public static ImageBrush getImageBrush(Bitmap bmp)
        {
            Bitmap bitmap = bmp;
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            ImageSourceConverter converter = new ImageSourceConverter();
            return new ImageBrush { ImageSource = (ImageSource)converter.ConvertFrom(stream), Stretch = Stretch.None, TileMode = TileMode.None };
        }

        public static ImageSource getImageSource(Bitmap bmp)
        {
            Bitmap bitmap = bmp;
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            ImageSourceConverter converter = new ImageSourceConverter();
            return (ImageSource)converter.ConvertFrom(stream);
        }

        public static Bitmap getMaskBMP(string text)
        {
            Bitmap bitmap;
            string[] strArray = text.Split(new char[] { ',' });
            int width = 0;
            int height = 0;
            int num3 = 0;
            int num4 = 0;
            List<Bitmap> list = new List<Bitmap>();
            for (int i = 0; i < strArray.Length; i++)
            {
                string str = strArray[i].Trim();
                if (string.IsNullOrEmpty(str)||str=="\n"||str=="\r"||str=="\n\r")
                {
                    str = "*";
                }
                PointF origin = new PointF(0f, 0f);
                CharacterRange[] ranges = new CharacterRange[] { new CharacterRange(0, str.Length) };
                new StringFormat().SetMeasurableCharacterRanges(ranges);
                int emHeight = defaultFont.FontFamily.GetEmHeight(defaultFont.Style);
                int cellAscent = defaultFont.FontFamily.GetCellAscent(defaultFont.Style);
                int cellDescent = defaultFont.FontFamily.GetCellDescent(defaultFont.Style);
                int lineSpacing = defaultFont.FontFamily.GetLineSpacing(defaultFont.Style);
                int num10 = (int)Math.Round((double)((defaultFont.Size * (lineSpacing - cellAscent)) / ((float)emHeight)));
                GraphicsPath path = new GraphicsPath();
                path.AddString(str, defaultFont.FontFamily, (int)defaultFont.Style, (defaultFont.Size * 96f) / 72f, origin, StringFormat.GenericDefault);
                System.Drawing.Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(path.GetBounds(), maskUPColor, maskDownColor, LinearGradientMode.Vertical);
                if (i < (strArray.Length - 1))
                {
                    num3 = (int)Math.Round((double)((path.GetBounds().Width + path.GetBounds().X) + (path.GetBounds().X / 2f)));
                }
                else
                {
                    num3 = (int)Math.Round((double)(path.GetBounds().Width + (path.GetBounds().X * 2f)));
                }
                num4 = (int)Math.Round((double)(path.GetBounds().Height + (path.GetBounds().Y * 2f)));
                bitmap = new Bitmap(num3, num4);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawPath(new System.Drawing.Pen(penColor, (float)penWidth), path);
                graphics.FillPath(brush, path);
                path.Dispose();
                brush.Dispose();
                graphics.Dispose();
                list.Add(bitmap);
                if (num4 > height)
                {
                    height = num4;
                }
                width += num3;
            }
            bitmap = new Bitmap(width, height);
            Graphics graphics2 = Graphics.FromImage(bitmap);
            width = 0;
            foreach (Bitmap bitmap2 in list)
            {
                graphics2.DrawImage(bitmap2, new Point(width, 0));
                width += bitmap2.Width;
                bitmap2.Dispose();
            }
            graphics2.Dispose();
            list.Clear();
            return bitmap;
        }

        public static List<int> GetWidth(string text)
        {
            string[] strArray = text.Split(new char[] { ',' });
            List<int> list = new List<int>();
            int item = 0;
            for (int i = 0; i < strArray.Length; i++)
            {
                string str = strArray[i].Trim();
                if (string.IsNullOrEmpty(str) || str == "\n" || str == "\r" || str == "\n\r")
                {
                    str = "*";
                }
                PointF origin = new PointF(0f, 0f);
                CharacterRange[] ranges = new CharacterRange[] { new CharacterRange(0, str.Length) };
                new StringFormat().SetMeasurableCharacterRanges(ranges);
                int emHeight = defaultFont.FontFamily.GetEmHeight(defaultFont.Style);
                int cellAscent = defaultFont.FontFamily.GetCellAscent(defaultFont.Style);
                int cellDescent = defaultFont.FontFamily.GetCellDescent(defaultFont.Style);
                int lineSpacing = defaultFont.FontFamily.GetLineSpacing(defaultFont.Style);
                int num7 = (int)Math.Round((double)((defaultFont.Size * (lineSpacing - cellAscent)) / ((float)emHeight)));
                GraphicsPath path = new GraphicsPath();
                path.AddString(str, defaultFont.FontFamily, (int)defaultFont.Style, (defaultFont.Size * 96f) / 72f, origin, StringFormat.GenericDefault);
                if (i < (strArray.Length - 1))
                {
                    item = (int)Math.Round((double)((path.GetBounds().Width + path.GetBounds().X) + (path.GetBounds().X / 2f)));
                }
                else
                {
                    item = (int)Math.Round((double)(path.GetBounds().Width + (path.GetBounds().X * 2f)));
                }
                list.Add(item);
            }
            return list;
        }

        public static void Initialize(Font lrcFont,int skinIndex)
        {
            DefaultFont = lrcFont;
            SkinIndex = skinIndex;
            ChangeColor(SkinIndex);
            lrcToolBar = new LrcToolBar();
            lrcWindow = new LrcWindow();
        }

        public static void ShowLrc()
        {
            if (lrcWindow != null)
            { 
                lrcWindow.Init(defaultText);
                lrcWindow.Show(); 
            }
            else
            {
                ChangeColor(SkinIndex);
                lrcToolBar = new LrcToolBar();
                lrcWindow = new LrcWindow(); 
                lrcWindow.Init(defaultText);
                lrcWindow.Show(); 
            }
        }

        public static void CloseLrc()
        {
            if (lrcWindow != null || lrcWindow.IsLoaded)
            {
                lrcWindow.Close();
                lrcWindow = null;
            }
        }

        public async static void SearchLrc(Song song)
        {
            bool isExist = false;
            lyric.Clear();
            string fileName = Common.getTitleFromPath(song.FileUrl);
            try
            {
                if (lrcWindow != null&&lrcWindow.IsLoaded) { lrcWindow.Init(fileName);}
                foreach (string str in LrcTypes)
                {
                    FileInfo[] files = new DirectoryInfo(song.FileUrl).Parent.GetFiles(str, SearchOption.TopDirectoryOnly);
                    if (files != null)
                    {
                        foreach (FileInfo info2 in files)
                        {
                            string fullName = info2.FullName;
                            if (Regex.IsMatch(fullName, fileName))
                            {
                                lyric = new Lrc(fullName);
                                isExist = true;
                                break;
                            }
                        }
                    }
                }
                if (!isExist)
                {
                    string lrcPath = await DownLrc.DownloadLrcAsync(song);
                    if (string.IsNullOrEmpty(lrcPath)) { lrcPath = await DownLrc.DownloadLrcAsyncFromQian(song); }
                    if (!string.IsNullOrEmpty(lrcPath)&&lrcWindow != null && lrcWindow.IsLoaded) {  lyric = new Lrc(lrcPath); }
                }
            }
            catch(Exception ex) { Debug.Write(ex.ToString()); }
        }

        public static void SetButtonChanged(object sender, int index)
        {
            ButtonChanged(sender, new ButtonChangeEventArgs(index));
        }

        public static void SetPause()
        {
            lrcToolBar.btnPlay.Text = ";";
            lrcToolBar.btnPlay.ToolTip = "暂停";
        }

        public static void SetPlay()
        {
            lrcToolBar.btnPlay.Dispatcher.Invoke(new Action(()=>
                {
                    lrcToolBar.btnPlay.Text = "4";
                    lrcToolBar.btnPlay.ToolTip = "播放";
                }));          
        }

        public static void UpdateSize(float size)
        {
            if (((defaultFont.Size + size) >= 12f) && ((defaultFont.Size + size) <= 48f))
            {
                defaultFont = new Font(defaultFont.FontFamily, defaultFont.Size + size, FontStyle.Bold, GraphicsUnit.Pixel);
            }
            if (!string.IsNullOrEmpty(lyric.fileName))
            {
                lock (lyric)
                {
                    lyric = new Lrc(lyric.fileName);
                }
            }
            lrcWindow.Update();
        }

        public static void UpdateSkin()
        {
            lrcWindow.Update();
        }

        public delegate void ButtonChangedHandle(object sender, LrcController.ButtonChangeEventArgs e);

        public class ButtonChangeEventArgs : EventArgs
        {
            private int buttonIndex = -1;

            public ButtonChangeEventArgs(int b)
            {
                this.buttonIndex = b;
            }

            public int ButtonIndex
            {
                get
                {
                    return this.buttonIndex;
                }
            }
        }
    }
}
