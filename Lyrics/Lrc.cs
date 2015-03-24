using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Lyrics
{
    public class Lrc
    {
        private string _album;
        private string _ar;
        private string _by;
        private string _fileName;
        private bool _isLoad;
        private List<OneLineLrc> _lrcList;
        private string _title;
        private string LrcTextString;

        #region
        private string album
        {
            get
            {
                return this._album;
            }
        }

        public string ar
        {
            get
            {
                return this.ar;
            }
        }

        public string by
        {
            get
            {
                return this._by;
            }
        }

        public string fileName
        {
            get
            {
                return this._fileName;
            }
            set
            {
                this._fileName = value;
            }
        }

        public int GetMaxTime
        {
            get
            {
                if (this._lrcList.Count < 1)
                {
                    return 0;
                }
                return this._lrcList[this._lrcList.Count - 1].StartTime;
            }
        }

        public bool isLoad
        {
            get
            {
                return this._isLoad;
            }
            set
            {
                this._isLoad = value;
            }
        }

        public List<OneLineLrc> LrcList
        {
            get
            {
                return this._lrcList;
            }
            set
            {
                this._lrcList = value;
            }
        }

        public string title
        {
            get
            {
                return this._title;
            }
        }
        #endregion

        public Lrc()
        {
            this._title = "";
            this._album = "";
            this._ar = "";
            this._by = "";
            this._lrcList = new List<OneLineLrc>();
            this._fileName = "";
            this._isLoad = false;
            this.LrcTextString = "";
        }

        public Lrc(string file)
        {
            this._title = "";
            this._album = "";
            this._ar = "";
            this._by = "";
            this._lrcList = new List<OneLineLrc>();
            this._fileName = "";
            this._isLoad = false;
            this.LrcTextString = "";
            this.InitLrc(file);
        }

        public Lrc(string lrcString, string ext)
        {
            this._title = "";
            this._album = "";
            this._ar = "";
            this._by = "";
            this._lrcList = new List<OneLineLrc>();
            this._fileName = "";
            this._isLoad = false;
            this.LrcTextString = "";
            this.Clear();
            string str = ext;
            if (str != null)
            {
                if (!(str == "LRC"))
                {
                    if (str == "KSC")
                    {
                        this.ImportKSC(this.LrcTextString);
                    }
                    else if (str == "QRC")
                    {
                        this.ImportQRC(this.LrcTextString);
                    }
                }
                else
                {
                    this.ImportLRC(this.LrcTextString);
                }
            }
        }

        public void Clear()
        {
            this._fileName = "";
            this._lrcList.Clear();
            this.LrcTextString = "";
            this._isLoad = false;
        }

        private string CheckLrc(string str)
        {
            if (str.Length < 3)
            {
                return "";
            }
            string str2 = "";
            string str3 = str;
            Regex regex = new Regex("[a-zA-Z0-9]|＇");
            for (int i = 0; i < (str3.Length - 1); i++)
            {
                string input = str3.Substring(i, 1);
                str2 = str2 + input;
                if (i == (str3.Length - 2))
                {
                    return str2;
                }
                string str5 = str3.Substring(i + 1, 1);
                if ((!regex.Match(input).Success || !regex.Match(str5).Success) && !(str5 == " "))
                {
                    str2 = str2 + ",";
                }
            }
            return str2;
        }

        private int FormatTime(string time)
        {
            string[] strArray = time.Substring(1, time.Length - 2).Split(new char[] { ':' });
            return (((int)((Convert.ToDouble(strArray[0]) * 60.0) * 1000.0)) + (((int)Convert.ToDouble(strArray[1])) * 0x3e8));
        }

        private Encoding GetEncoding(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            Encoding type = this.GetType(fs);
            fs.Close();
            return type;
        }

        private string GetFileExt(string file)
        {
            return file.Substring(file.LastIndexOf('.') + 1).ToUpper();
        }

        private Encoding GetType(FileStream fs)
        {
            int num;
            byte[] buffer = new byte[] { 0xff, 0xfe, 0x41 };
            byte[] buffer5 = new byte[3];
            buffer5[0] = 0xfe;
            buffer5[1] = 0xff;
            byte[] buffer2 = buffer5;
            byte[] buffer3 = new byte[] { 0xef, 0xbb, 0xbf };
            Encoding bigEndianUnicode = Encoding.Default;
            BinaryReader reader = new BinaryReader(fs, Encoding.Default);
            int.TryParse(fs.Length.ToString(), out num);
            byte[] data = reader.ReadBytes(num);
            if (this.IsUTF8Bytes(data) || (((data[0] == 0xef) && (data[1] == 0xbb)) && (data[2] == 0xbf)))
            {
                bigEndianUnicode = Encoding.UTF8;
            }
            else if (((data[0] == 0xfe) && (data[1] == 0xff)) && (data[2] == 0))
            {
                bigEndianUnicode = Encoding.BigEndianUnicode;
            }
            else if (((data[0] == 0xff) && (data[1] == 0xfe)) && (data[2] == 0x41))
            {
                bigEndianUnicode = Encoding.Unicode;
            }
            reader.Close();
            return bigEndianUnicode;
        }

        public void ImportKSC(string text)
        {
            Regex regex = new Regex("'[^']+'");
            Regex regex2 = new Regex(@"\d{2}:\d{2}");
            Regex regex3 = new Regex(@"karaoke\.add\([^\r]+\r");
            Regex regex4 = new Regex("蓝天阁");
            try
            {
                MatchCollection matchs = regex3.Matches(text);
                foreach (Match match in matchs)
                {
                    if (!regex4.IsMatch(match.ToString()))
                    {
                        MatchCollection matchs2 = regex.Matches(match.ToString());
                        if (matchs2.Count == 4)
                        {
                            OneLineLrc item = new OneLineLrc
                            {
                                StartTime = this.FormatTime(matchs2[0].ToString()),
                                EndTime = this.FormatTime(matchs2[1].ToString()),
                                LrcText = this.CheckLrc(matchs2[2].ToString().Substring(1, matchs2[2].ToString().Length - 2) + "\r")
                            };
                            string[] strArray = matchs2[3].ToString().Substring(1, matchs2[3].ToString().Length - 2).Split(new char[] { ',' });
                            string str = "";
                            int startTime = item.StartTime;
                            foreach (string str2 in strArray)
                            {
                                int num2 = Convert.ToInt32(str2);
                                str = str + ((startTime + num2)).ToString() + ",";
                            }
                            item.MidTime = str.Substring(0, str.Length - 1);
                            this._lrcList.Add(item);
                        }
                    }
                }
                this._isLoad = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void ImportLRC(string text)
        {
            Regex regex = new Regex(@"\[ti:[^\r]+\r");
            Regex regex2 = new Regex(@"\[ar:[^\r]+\r");
            Regex regex3 = new Regex(@"\[al:[^\r]+\r");
            Regex regex4 = new Regex(@"\[by:[^\r]+\r");
            Regex regex5 = new Regex(@"[^\]]+\r");
            Regex regex6 = new Regex(@"\[\d{2}:\d{2}(?:\]|\.\d{2}\]|\.\d{3}\])");
            Regex regex7 = new Regex(@"\[\d{2}[^\r]+\r");
            List<string> list = new List<string>();
            this.LrcTextString = this.LrcTextString.Replace("|", "｜");
            this.LrcTextString = this.LrcTextString.Replace("，", "，");
            try
            {
                this._title = regex.Match(this.LrcTextString).ToString().Trim();
                this._ar = regex2.Match(this.LrcTextString).ToString().Trim();
                this._album = regex3.Match(this.LrcTextString).ToString().Trim();
                this._by = regex4.Match(this.LrcTextString).ToString().Trim();
                if (!string.IsNullOrEmpty(this._title.Trim()))
                {
                    this._title = Regex.Replace(this._title, "ti", "歌名") + "\r";
                }
                if (!string.IsNullOrEmpty(this._ar))
                {
                    this._ar = Regex.Replace(this._ar, "ar", "歌手") + "\r";
                }
                if (!string.IsNullOrEmpty(this._album))
                {
                    this._album = Regex.Replace(this._album, "al", "专辑") + "\r";
                }
                if (!string.IsNullOrEmpty(this._by))
                {
                    this._by = Regex.Replace(this._by, "by", "出处") + "\r";
                }
                MatchCollection matchs = regex7.Matches(this.LrcTextString);
                foreach (Match match in matchs)
                {
                    string str = regex5.Match(match.ToString()).ToString();
                    if (str.Trim().Length > 1)
                    {
                        MatchCollection matchs2 = regex6.Matches(match.ToString());
                        foreach (Match match2 in matchs2)
                        {
                            list.Add(match2.ToString() + "|" + this.CheckLrc(str));
                        }
                    }
                }
                list.Sort();
                foreach (string str in list)
                {
                    string[] strArray = str.Split(new char[] { '|' });
                    if (strArray.Length == 2)
                    {
                        OneLineLrc item = new OneLineLrc
                        {
                            StartTime = this.FormatTime(strArray[0]),
                            StartTimeStr = strArray[0],
                            LrcText = strArray[1]
                        };
                        this._lrcList.Add(item);
                    }
                }
                for (int i = 0; i < this._lrcList.Count; i++)
                {
                    int num7;
                    int num2 = this._lrcList[i].LrcText.Split(new char[] { ',' }).Count<string>();
                    string str2 = "";
                    if (i < (this._lrcList.Count - 1))
                    {
                        int num3 = (this._lrcList[i + 1].StartTime - this._lrcList[i].StartTime) - 500;
                        int num4 = num3 % num2;
                        int num5 = num3 / num2;
                        int startTime = this._lrcList[i].StartTime;
                        num7 = 0;
                        while (num7 < num2)
                        {
                            if (num7 < (num2 - 1))
                            {
                                startTime += num5;
                                str2 = str2 + startTime.ToString() + ",";
                            }
                            else
                            {
                                str2 = str2 + (((startTime + num5) + num4)).ToString();
                            }
                            num7++;
                        }
                        this._lrcList[i].EndTime = this._lrcList[i + 1].StartTime - 500;
                        this._lrcList[i].MidTime = str2;
                    }
                    else
                    {
                        for (num7 = 0; num7 < num2; num7++)
                        {
                            if (num7 < (num2 - 1))
                            {
                                str2 = str2 + "0,";
                            }
                            else
                            {
                                str2 = str2 + "0";
                            }
                        }
                        this._lrcList[i].EndTime = this._lrcList[i].StartTime + 0x3e8;
                        this._lrcList[i].MidTime = str2;
                    }
                }
                this._isLoad = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void ImportQRC(string fileName)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(fileName);
                string attribute = document.SelectSingleNode("QrcInfos/LyricInfo")["Lyric_1"].GetAttribute("LyricContent");
                Regex regex = new Regex(@"\[ti:[^\r]+\r");
                Regex regex2 = new Regex(@"\[ar:[^\r]+\r");
                Regex regex3 = new Regex(@"\[al:[^\r]+\r");
                Regex regex4 = new Regex(@"\[by:[^\r]+\r");
                Regex regex5 = new Regex(@"\[\d{2,}[^\r]+\r");
                Regex regex6 = new Regex(@"\[[^\]]+\]");
                Regex regex7 = new Regex(@"\(\d{2,},\d{2,}\)");
                MatchCollection matchs = regex5.Matches(attribute);
                foreach (Match match in matchs)
                {
                    string[] strArray;
                    string input = match.ToString();
                    OneLineLrc item = new OneLineLrc();
                    string str3 = regex6.Match(input).ToString();
                    if (str3.Trim().Length > 1)
                    {
                        strArray = str3.Substring(1, str3.Length - 2).Split(new char[] { ',' });
                        item.StartTime = Convert.ToInt32(strArray[0]);
                        item.EndTime = Convert.ToInt32(strArray[0]) + Convert.ToInt32(strArray[1]);
                    }
                    input = Regex.Replace(Regex.Replace(Regex.Replace(input, @"\[[^\]]+\]", ""), "\r", ""), "\r\n", "");
                    MatchCollection matchs2 = regex7.Matches(input);
                    string str4 = "";
                    foreach (Match match2 in matchs2)
                    {
                        strArray = match2.ToString().Substring(1, match2.ToString().Length - 2).Split(new char[] { ',' });
                        str4 = str4 + strArray[0] + ",";
                    }
                    str4 = str4.Substring(0, str4.Length - 1);
                    item.MidTime = str4;
                    input = Regex.Replace(input, @"\(\d{2,},\d{2,}\)", ",");
                    input = input.Substring(0, input.Length - 1);
                    item.LrcText = input;
                    this._lrcList.Add(item);
                }
                this._isLoad = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void InitLrc(string file)
        {
            this.Clear();
            if (File.Exists(file))
            {
                this._fileName = file;
                string fileExt = this.GetFileExt(this._fileName);
                if (fileExt.ToLower() == "qrc")
                {
                    this.ImportQRC(this._fileName);
                }
                else
                {
                    Encoding encoding = this.GetEncoding(this._fileName);
                    Stream stream = null;
                    StreamReader reader = null;
                    try
                    {
                        stream = File.Open(this._fileName, FileMode.Open, FileAccess.Read);
                        reader = new StreamReader(stream, encoding);
                        this.LrcTextString = reader.ReadToEnd().ToString();
                        reader.Close();
                        stream.Close();
                        string str2 = fileExt;
                        if (str2 != null)
                        {
                            if (!(str2 == "LRC"))
                            {
                                if (str2 == "KSC")
                                {
                                    goto Label_00F2;
                                }
                                if (str2 == "HRC")
                                {
                                    goto Label_0101;
                                }
                                if (str2 == "KRC")
                                {
                                    goto Label_0110;
                                }
                            }
                            else
                            {
                                this.ImportLRC(this.LrcTextString);
                            }
                        }
                        return;
                    Label_00F2:
                        this.ImportKSC(this.LrcTextString);
                        return;
                    Label_0101:
                        this.ImportHRC(this.LrcTextString);
                        return;
                    Label_0110:
                        this.ImportKRC(this.LrcTextString);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("打开歌词出错:" + exception.Message);
                        reader.Close();
                        stream.Close();
                        throw exception;
                    }
                }
            }
        }

        private bool IsUTF8Bytes(byte[] data)
        {
            int num = 1;
            for (int i = 0; i < data.Length; i++)
            {
                byte num2 = data[i];
                if (num == 1)
                {
                    if (num2 >= 0x80)
                    {
                        while (((num2 = (byte)(num2 << 1)) & 0x80) != 0)
                        {
                            num++;
                        }
                        if ((num == 1) || (num > 6))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if ((num2 & 0xc0) != 0x80)
                    {
                        return false;
                    }
                    num--;
                }
            }
            if (num > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }

        private string resetLrc(string str)
        {
            string str2 = "";
            string[] strArray = str.Substring(1, str.Length - 2).Split(new char[] { ',' });
            foreach (string str3 in strArray)
            {
                str2 = str2 + str3;
            }
            return str2;
        }


    }
}
