using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using Easy.NET.Tools;

namespace Easy.NET.Http
{
    public class HttpRequester
    {
        enum HttpMethod { 
            GET,POST
        }

        public CookieContainer Cookies { get; set; }
        public HttpRequester()
        {
            Cookies = new CookieContainer();
        }

        public string Get(string url,Encoding encoding = null, int timeout = 5000)
        {
            using (var response = GetHttpWebResponse(url, timeout, encoding: encoding))
            {
                if (response == null)
                    return null;

                return GetResponseBody(response, encoding);
            }
        }

        public Stream GetStream(string url, int timeout = 5000)
        {
            var response = GetHttpWebResponse(url, timeout);
            if (response == null)
                return null;

            return response.GetResponseStream();
        }

        public string Post(string url,string postdata, Encoding encoding = null, int timeout = 5000)
        {
            using (var response = GetHttpWebResponse(url, timeout, postdata, encoding, HttpMethod.POST))
            {
                if (response == null)
                    return null;

                return GetResponseBody(response, encoding);
            }
        }

        public Stream PostStream(string url, string postdata, Encoding encoding = null, int timeout = 5000)
        {
            var response = GetHttpWebResponse(url, timeout, postdata, encoding, HttpMethod.POST);
            if (response == null)
                return null;

            return response.GetResponseStream();
        }

        private HttpWebResponse GetHttpWebResponse(string url, int timeout = 5000, string postdata = "",Encoding encoding = null, HttpMethod method = HttpMethod.GET)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            var uri = new Uri(url);
            HttpWebResponse response = null;
            try
            {
                response = new Retry<HttpWebResponse>(() =>
                {
                    var requester = HttpWebRequest.Create(uri) as HttpWebRequest;
                    requester.CookieContainer = Cookies;
                    requester.Method = method.ToString();
                    requester.Headers.Add("Accept-Encoding", "gzip");
                    requester.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                    requester.Timeout = timeout;
                    requester.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:35.0) Gecko/20100101 Firefox/35.0";
                    if (method == HttpMethod.POST)
                    {
                        var buffer = encoding.GetBytes(postdata);
                        requester.ContentLength = buffer.Length;
                        var stream = requester.GetRequestStream();
                        stream.Write(buffer, 0, buffer.Length);
                        stream.Close();
                    }

                    return (HttpWebResponse)requester.GetResponse();

                }).RetryIfException();

                return response;
            }
            catch (Exception ex)
            {
                //TODO: 异常发到server
            }

            return response;
        }

        private string GetResponseBody(HttpWebResponse response, Encoding encoding = null)
        {

            if (response == null)
                return string.Empty;

            if (encoding == null)
                encoding = Encoding.UTF8;

            string responseBody = string.Empty;
            if (response.ContentEncoding.ToLower().Contains("gzip"))
            {
                using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            else if (response.ContentEncoding.ToLower().Contains("deflate"))
            {
                using (DeflateStream stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            else
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
            }
            return responseBody;
        }
    }
}