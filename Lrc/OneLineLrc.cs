using System;
namespace Lrc
{
    public class OneLineLrc
    {
        private int endTime;
        private string lrcText;
        private string midTime;
        private int startTime;
        private string startTimeStr;

        public OneLineLrc()
        {
            this.startTimeStr = "";
            this.startTime = 0;
            this.lrcText = "";
            this.endTime = 0;
            this.midTime = "";
        }

        public OneLineLrc(int s, string sStr, string t, int e)
        {
            this.startTimeStr = "";
            this.startTime = 0;
            this.lrcText = "";
            this.endTime = 0;
            this.midTime = "";
            this.startTime = s;
            this.startTimeStr = sStr;
            this.lrcText = t;
            this.endTime = e;
        }

        public int EndTime
        {
            get
            {
                return this.endTime;
            }
            set
            {
                this.endTime = value;
            }
        }

        public string LrcText
        {
            get
            {
                return this.lrcText;
            }
            set
            {
                this.lrcText = value;
            }
        }

        public string MidTime
        {
            get
            {
                return this.midTime;
            }
            set
            {
                this.midTime = value;
            }
        }

        public int StartTime
        {
            get
            {
                return this.startTime;
            }
            set
            {
                this.startTime = value;
            }
        }

        public string StartTimeStr
        {
            get
            {
                return this.startTimeStr;
            }
            set
            {
                this.startTimeStr = value;
            }
        }
    }
}
