using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;

namespace HttpLibrary
{
    public class RequestState
    {
        public HttpWebRequest request = null;
        public HttpWebResponse response = null;
    }

    public class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = 60 * 1000;
            return w;
        }
    }

    public static class HttpHelper
    {
        public static void DownloadRemoteFile(string uri, string fileName)
        {
            using (MyWebClient client = new MyWebClient())
            {
                client.DownloadFile(uri, fileName);
            }
        }

        public static void DownloadRemoteImageFile(string uri, string fileName)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Check that the remote file was found. The ContentType
            // check is performed since a request for a non-existent
            // image file might be redirected to a 404-page, which would
            // yield the StatusCode "OK", even though the image was not
            // found.
            if ((response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Moved ||
                response.StatusCode == HttpStatusCode.Redirect) &&
                response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {

                // if the remote file was found, download oit
                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = File.OpenWrite(fileName))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);
                }
            }
        }

        private static string DecompressGzip_my(Stream in_InputStream)
        {
            Stream lv_OutputStream = new MemoryStream();
            byte[] lv_Buffer = new byte[4096];

            try
            {
                //using (GZipStream lv_gzip = new GZipStream(in_InputStream, CompressionMode.Decompress))
                //{
                //    int i;
                //    while ((i = lv_gzip.Read(lv_Buffer, 0, lv_Buffer.Length)) != 0)
                //    {
                //        lv_OutputStream.Write(lv_Buffer, 0, i);
                //    }
                //}
                lv_OutputStream.Position = 0;
                using (StreamReader sr = new StreamReader(in_InputStream))
                {
                    string str = sr.ReadToEnd();
                    return str;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string DecompressDeflate_my(Stream in_InputStream)
        {
            Stream lv_OutputStream = new MemoryStream();
            byte[] lv_Buffer = new byte[4096];
            try
            {
                //using (DeflateStream lv_Deflate = new DeflateStream(in_InputStream, CompressionMode.Decompress))
                //{
                //    int i;
                //    while ((i = lv_Deflate.Read(lv_Buffer, 0, lv_Buffer.Length)) != 0)
                //    {
                //        lv_OutputStream.Write(lv_Buffer, 0, i);
                //    }
                //}
                //lv_OutputStream.Position = 0;
                using (StreamReader sr = new StreamReader(in_InputStream))
                {
                    string str = sr.ReadToEnd();
                    return str;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string DecompressGzip(Stream in_InputStream)
        {
            Stream lv_OutputStream = new MemoryStream();
            byte[] lv_Buffer = new byte[4096];

            try
            {
                using (GZipStream lv_gzip = new GZipStream(in_InputStream, CompressionMode.Decompress))
                {
                    int i;
                    while ((i = lv_gzip.Read(lv_Buffer, 0, lv_Buffer.Length)) != 0)
                    {
                        lv_OutputStream.Write(lv_Buffer, 0, i);
                    }
                }
                lv_OutputStream.Position = 0;
                using (StreamReader sr = new StreamReader(lv_OutputStream))
                {
                    string str = sr.ReadToEnd();
                    return str;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string DecompressDeflate(Stream in_InputStream)
        {
            Stream lv_OutputStream = new MemoryStream();
            byte[] lv_Buffer = new byte[4096];
            try
            {
                using (DeflateStream lv_Deflate = new DeflateStream(in_InputStream, CompressionMode.Decompress))
                {
                    int i;
                    while ((i = lv_Deflate.Read(lv_Buffer, 0, lv_Buffer.Length)) != 0)
                    {
                        lv_OutputStream.Write(lv_Buffer, 0, i);
                    }
                }
                lv_OutputStream.Position = 0;
                using (StreamReader sr = new StreamReader(lv_OutputStream))
                {
                    string str = sr.ReadToEnd();
                    return str;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static string GetWebPageResponseAsync(string url,
            string postdata,
            string referer,
            CookieContainer cookieContainer,
            out IAsyncResult async)
        {
            async = null;
            if (cookieContainer == null) throw new ArgumentNullException("cookieContainer");
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                //request.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                request.Headers["Accept-Language"] = "en-us";
                //request.Headers["Accept-Encoding"] = "gzip, deflate";
                request.Headers["Accept-Encoding"] = "gzip, deflate, peerdist";
                if (!string.IsNullOrEmpty(referer))
                {
                    request.Referer = referer;
                }
                request.AllowAutoRedirect = true;// allowAutoRedirect;
                request.CookieContainer = cookieContainer;
                request.KeepAlive = true;
                //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; SV1; .NET CLR 1.1.4322; InfoPath.1)";
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; InfoPath.3; MS-RTC LM 8) chromeframe/5.0.375.3";
                if (!string.IsNullOrEmpty(postdata))
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.Method = "POST";
                    byte[] b = ASCIIEncoding.ASCII.GetBytes(postdata);
                    request.ContentLength = b.Length;
                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(b, 0, b.Length);
                        reqStream.Flush();
                    }
                }
                else
                {
                    request.Method = "GET";
                }


                RequestState myRequestState = new RequestState();
                myRequestState.request = request;
                async = request.BeginGetResponse(new AsyncCallback(ResponseCallback), myRequestState);
                // Console.WriteLine("BeginGetResponse: {0}", async.GetHashCode());

                return null;
                // Wait for request to complete
                //async.AsyncWaitHandle.WaitOne();
                //    //HttpWebResponse response = myRequestState.response = (HttpWebResponse)myRequestState.request.EndGetResponse(async);
                //    Console.WriteLine("EndGetResponse: {0}", async.GetHashCode());

                //    using (Stream stream = response.GetResponseStream())
                //    {
                //        string data = null;
                //        if (response.ContentEncoding.IndexOf("gzip") > -1)
                //        {
                //            data = DecompressGzip(stream);
                //        }
                //        else if (response.ContentEncoding.IndexOf("deflate") > -1)
                //        {
                //            data = DecompressDeflate(stream);
                //        }
                //        else
                //        {
                //            using (StreamReader sr = new StreamReader(stream))
                //            {
                //                data = sr.ReadToEnd();
                //            }
                //        }
                //        //foreach (Cookie cookieValue in response.Cookies)
                //        //{
                //        //    Console.WriteLine("Cookie: " + cookieValue.ToString());
                //        //}
                //        return data;
                //    }
                //}
            }

            catch (IOException)
            {
                return null;
            }
            catch (WebException)
            {
                return null;
            }

        }


        static int counter = 0;
        static object lockCounter = new object();

        static void ResponseCounter()
        {
            lock (lockCounter)
            {
                counter++;

            }

        }

        static void ResponseCallback(IAsyncResult ar)
        {
            //Console.WriteLine("ResponseCallback: {0}", ar.GetHashCode());
            RequestState myRequestState = (RequestState)ar.AsyncState;
            HttpWebResponse response = myRequestState.response = (HttpWebResponse)myRequestState.request.EndGetResponse(ar);
            //Console.WriteLine("EndGetResponse: {0}", ar.GetHashCode());

            //using (Stream stream = response.GetResponseStream())
            //{
            //    string data = null;
            //    //if (response.ContentEncoding.IndexOf("gzip") > -1)
            //    //{
            //    //    data = DecompressGzip(stream);
            //    //}
            //    //else if (response.ContentEncoding.IndexOf("deflate") > -1)
            //    //{
            //    //    data = DecompressDeflate(stream);
            //    //}
            //    //else
            //    {
            //        using (StreamReader sr = new StreamReader(stream))
            //        {
            //            //data = sr.ReadToEnd();
            //        }
            //    }
            //    //foreach (Cookie cookieValue in response.Cookies)
            //    //{
            //    //    Console.WriteLine("Cookie: " + cookieValue.ToString());
            //    //}
            //    Console.WriteLine("Got data");

            //}
            ResponseCounter();

        }

        public static int GetCounter()
        {
            lock (lockCounter) { return counter; }
        }

        public static string GetWebPageResponse(string url,
            string postdata,
            string referer,
            CookieContainer cookieContainer)
        {
            return GetWebPageResponse(url,
                postdata,
                referer,
                cookieContainer,
                true);
        }

        public static string GetWebPageResponse(string url,
            string postdata,
            string referer,
            CookieContainer cookieContainer,
            bool allowAutoRedirect)
        {
            if (cookieContainer == null) throw new ArgumentNullException("cookieContainer");
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                //request.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                request.Headers["Accept-Language"] = "en-us";
                //request.Headers["Accept-Encoding"] = "gzip, deflate";
                request.Headers["Accept-Encoding"] = "gzip, deflate, peerdist";
                if (!string.IsNullOrEmpty(referer))
                {
                    request.Referer = referer;
                }
                request.AllowAutoRedirect = allowAutoRedirect;
                request.CookieContainer = cookieContainer;
                request.KeepAlive = true;
                //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; SV1; .NET CLR 1.1.4322; InfoPath.1)";
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; InfoPath.3; MS-RTC LM 8) chromeframe/5.0.375.3";
                if (!string.IsNullOrEmpty(postdata))
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.Method = "POST";
                    byte[] b = ASCIIEncoding.ASCII.GetBytes(postdata);
                    request.ContentLength = b.Length;
                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(b, 0, b.Length);
                        reqStream.Flush();
                    }
                }
                else
                {
                    request.Method = "GET";
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        string data = null;
                        if (response.ContentEncoding.IndexOf("gzip") > -1)
                        {
                            data = DecompressGzip(stream);
                        }
                        else if (response.ContentEncoding.IndexOf("deflate") > -1)
                        {
                            data = DecompressDeflate(stream);
                        }
                        else
                        {
                            using (StreamReader sr = new StreamReader(stream))
                            {
                                data = sr.ReadToEnd();
                            }
                        }
                        //foreach (Cookie cookieValue in response.Cookies)
                        //{
                        //    Console.WriteLine("Cookie: " + cookieValue.ToString());
                        //}
                        return data;
                    }
                }
            }
            catch (IOException)
            {
                return null;
            }
            catch (WebException)
            {
                return null;
            }
        }
    }
}
