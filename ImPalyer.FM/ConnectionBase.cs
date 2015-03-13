using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
namespace ImPlayer.FM
{
    /// <summary>
    /// 网络连接基础类
    /// </summary>
    public class ConnectionBase
    {
        /// <summary>
        /// Cookie
        /// </summary>
        public static CookieContainer Cookie;
        /// <summary>
        /// 默认HTTP头：UserAgent
        /// </summary>
        public static string DefaultUserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
        /// <summary>
        /// 默认HTTP头：accept
        /// </summary>
        public static string DefaultAccept = "text/html, application/xhtml+xml, */*";
        /// <summary>
        /// 默认HTTP头：contentType
        /// </summary>
        public static string DefaultContentType = "application/x-www-form-urlencoded";
        /// <summary>
        /// 默认编码
        /// </summary>
        public static Encoding DefaultEncoding = Encoding.UTF8;
        /// <summary>
        /// 自定义浏览器标志
        /// </summary>
        public string UserAgent, Accept, ContentType;
        /// <summary>
        /// 编码
        /// </summary>
        public Encoding Encoding;
        /// <summary>
        /// 是否抛出异常
        /// </summary>
        public bool ThrowException;

        static ConnectionBase()
        {
            if (!LoadCookies()) ClearCookie();
        }

        public ConnectionBase(bool throwException, string userAgent, string accept, string contentType, Encoding encoding)
        {
            ThrowException = throwException;
            UserAgent = userAgent;
            Accept = accept;
            ContentType = contentType;
            Encoding = encoding;
        }

        public ConnectionBase(bool throwException = false)
            : this(DefaultEncoding, throwException)
        {
        }

        public ConnectionBase(Encoding encoding, bool throwException = false)
            : this(throwException, DefaultUserAgent, DefaultAccept, DefaultContentType, encoding)
        {
        }
        /// <summary>
        /// 用Post方法发送请求
        /// </summary>
        /// <param name="postUri">请求的地址</param>
        /// <param name="accept">Accept头</param>
        /// <param name="referer">Referer头</param>
        /// <param name="contentType">ContentType头</param>
        /// <param name="content">请求正文</param>
        /// <returns>
        /// 响应正文
        /// </returns>
        public string Post(string postUri, string accept, string referer, string contentType, byte[] content)
        {
            string file = null;

            try
            {
                HttpWebRequest request = WebRequest.Create(postUri) as HttpWebRequest;
                request.Accept = accept;
                request.AllowAutoRedirect = true;
                request.ContentLength = content.Length;
                request.ContentType = contentType;
                request.CookieContainer = Cookie;
                request.KeepAlive = true;
                request.Method = "POST";
                request.Referer = referer;
                request.UserAgent = UserAgent;
                request.ServicePoint.Expect100Continue = false;
                using (Stream requestStream = request.GetRequestStream())
                    requestStream.Write(content, 0, content.Length);
                using (HttpWebResponse responce = request.GetResponse() as HttpWebResponse)
                using (StreamReader sr = new StreamReader(responce.GetResponseStream(), Encoding))
                    file = sr.ReadToEnd();

                HttpClient client = new HttpClient();


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                if (ThrowException)
                    throw;
            }

            return file;
        }
        /// <summary>
        /// 用Post方法发送请求
        /// </summary>
        /// <param name="postUri">请求的地址</param>
        /// <param name="content">请求正文</param>
        /// <returns>响应正文</returns>
        public string Post(string postUri, byte[] content)
        {
            return Post(postUri, null, content);
        }
        /// <summary>
        /// 用Post方法发送请求
        /// </summary>
        /// <param name="postUri">请求的地址</param>
        /// <param name="referer">Referer头</param>
        /// <param name="content">请求正文</param>
        /// <returns>响应正文</returns>
        public string Post(string postUri, string referer, byte[] content)
        {
            return Post(postUri, Accept, referer, ContentType, content);
        }
        /// <summary>
        /// 用Get方法发送请求
        /// </summary>
        /// <param name="getUri">请求的地址</param>
        /// <param name="accept">Accept头</param>
        /// <param name="referer">Referer头</param>
        /// <returns>
        /// 响应正文
        /// </returns>
        public string Get(string getUri, string accept, string referer)
        {
            string file = null;

            try
            {
                HttpWebRequest request = WebRequest.Create(getUri) as HttpWebRequest;
                request.Accept = accept;
                request.AllowAutoRedirect = true;
                request.CookieContainer = Cookie;
                request.KeepAlive = true;
                request.Method = "GET";
                request.Referer = referer;
                request.UserAgent = UserAgent;
                using (HttpWebResponse responce = request.GetResponse() as HttpWebResponse)
                using (StreamReader sr = new StreamReader(responce.GetResponseStream(), Encoding))
                    file = sr.ReadToEnd();


                //HttpClient client = new HttpClient(new HttpClientHandler {  CookieContainer=Cookie, AllowAutoRedirect=true});
                //file = await client.GetStringAsync(getUri);
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                if (ThrowException)
                    throw;
            }

            return file;
        }


        /// <summary>
        /// 用Get方法发送请求
        /// </summary>
        /// <param name="getUri">请求的地址</param>
        /// <returns>响应正文</returns>
        public string Get(string getUri)
        {
            return Get(getUri, Accept, null);
        }

        /// <summary>
        /// 用Get方法发送请求
        /// </summary>
        /// <param name="getUri">请求的地址</param>
        /// <param name="referer">Referer头</param>
        /// <returns>响应正文</returns>
        public string Get(string getUri, string referer)
        {
            return Get(getUri, Accept, referer);
        }
        /// <summary>
        /// 读取Cookies
        /// </summary>
        /// <returns>成功与否</returns>
        public static bool LoadCookies()
        {
            try
            {
               // Cookie = BinarySerializeHelper.Deserialize<CookieContainer>(Paths.CookiesFile);
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 保存Cookies
        /// </summary>
        /// <returns>成功与否</returns>
        public static bool SaveCookies()
        {
            try
            {
                //BinarySerializeHelper.Serialize(Paths.CookiesFile, Cookie);
            }

            catch
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 根据请求的URL和参数构造一个新的URL
        /// </summary>
        /// <param name="baseUrl">请求URL</param>
        /// <param name="parameters">参数</param>
        /// <returns>新的URL</returns>
        public static string ConstructUrlWithParameters(string baseUrl, Parameters parameters)
        {
            if (parameters == null || parameters.Count() == 0)
                return baseUrl;
            return baseUrl + "?" + parameters;
        }

        /// <summary>
        /// 清除Cookie
        /// </summary>
        public static void ClearCookie()
        {
            Cookie = new CookieContainer(1000, 1000, 100000);
        }

        class PCHttpClienHanlder : HttpClientHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("User-Agent", DefaultUserAgent ); 
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}