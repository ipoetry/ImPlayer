using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lyrics
{
    class Common
    {
        /// <summary>
        /// 从路径中提取文件名+后缀
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string getFileNameFromPath(string path)
        {
            int index = path.LastIndexOf("\\") + 1;
            return path.Substring(index);
        }
        /// <summary>
        /// 从路径中提取文件名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string getTitleFromPath(string path)
        {
            string fileNameWithExtend = getFileNameFromPath(path);
            return fileNameWithExtend.Remove(fileNameWithExtend.IndexOf("."));
                
        }
    }
}
