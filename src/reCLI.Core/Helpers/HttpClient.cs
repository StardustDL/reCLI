using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace reCLI.Core.Helpers
{
    public class HttpClient
    {
        #region Static

        public static async Task<string> GET(string url)
        {
            HttpWebRequest request = (HttpWebRequest)System.Net.HttpWebRequest.Create(url);
            using (var response = (HttpWebResponse)(await request.GetResponseAsync()))
            {
                // 请求成功的状态码：200
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            return await reader.ReadToEndAsync();
                        }
                    }
                }
                return null;
            }
        }

        public static async Task<string> POST(string url, string data = "")
        {
            HttpWebRequest http = (HttpWebRequest)WebRequest.Create(url);
            http.Method = "POST";
            http.ContentType = "application/x-www-form-urlencoded";
            using (Stream sd = http.GetRequestStream())
            using (StreamWriter sw = new StreamWriter(sd))
                sw.Write(data);

            HttpWebResponse res = (HttpWebResponse)await http.GetResponseAsync();
            using (Stream s = res.GetResponseStream())
            using (StreamReader rs = new StreamReader(s))
                return await rs.ReadToEndAsync();
        }

        #endregion

        private CookieContainer cookie = new CookieContainer();
        public Encoding Encoding { get; private set; }
        private HttpWebResponse response;

        public HttpClient(string _Encoding="utf-8")
        {
            Encoding = Encoding.GetEncoding(_Encoding);
        }

        public async Task<string> Get(string url, string _cookie = null)
        {
            HttpWebRequest http = (HttpWebRequest)WebRequest.Create(url);
            //http.Headers.Add("Connection", "keep-alive");
            http.Method = "GET";
            if (_cookie == null) http.CookieContainer = cookie;
            else http.Headers.Add("Cookie", _cookie);
            HttpWebResponse res = (HttpWebResponse)await http.GetResponseAsync();

            response = res;
            using (Stream s = res.GetResponseStream())
            using (StreamReader rs = new StreamReader(s, Encoding))
                return await rs.ReadToEndAsync();
        }

        public async Task<Stream> GetStream(string url, string _cookie = null)
        {
            HttpWebRequest http = (HttpWebRequest)WebRequest.Create(url);
            http.Headers.Add("Connection", "keep-alive");
            http.Method = "GET";
            if (_cookie == null) http.CookieContainer = cookie;
            else http.Headers.Add("Cookie", _cookie);
            HttpWebResponse res = (HttpWebResponse)await http.GetResponseAsync();
            response = res;
            return res.GetResponseStream();
        }

        public async Task<string> Post(string url, string data = "")
        {
            HttpWebRequest http = (HttpWebRequest)WebRequest.Create(url);
            http.CookieContainer = cookie;
            http.Method = "POST";
            http.ContentType = "application/x-www-form-urlencoded";
            using (Stream sd = http.GetRequestStream())
            using (StreamWriter sw = new StreamWriter(sd))
                sw.Write(data);

            HttpWebResponse res = (HttpWebResponse)await http.GetResponseAsync();
            response = res;
            using (Stream s = res.GetResponseStream())
            using (StreamReader rs = new StreamReader(s, Encoding))
                return rs.ReadToEnd();
        }

        public void DealCookie()
        {
            string[] h = response.Headers.GetValues("Set-Cookie");
            if (h == null) return;
            foreach (string c in h)
            {
                cookie.SetCookies(response.ResponseUri, c);
            }
        }

        string UrlEncode(string str)
        {
            return System.Web.HttpUtility.UrlEncode(str, Encoding);
        }
    }
}
