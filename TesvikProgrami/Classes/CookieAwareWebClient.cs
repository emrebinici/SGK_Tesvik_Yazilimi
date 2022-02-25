using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace TesvikProgrami.Classes
{
    public class CookieAwareWebClient : WebClient
    {
        public CookieContainer CookieContainer { get; set; }
        public string Uri { get; set; }
        public string Referer { get; set; }

        public string ResponseUri { get; set; }

        private string _cookie = null;
        public string Cookie
        {
            get
            {
                if (_cookie == null)
                {
                    _cookie = GetGlobalCookies(Uri);
                }

                return _cookie;
            }
            set { _cookie = value; }
        }

        public int TimeOut = Program.ZamanAsimiSuresi;

        public CookieAwareWebClient()
            : this(new CookieContainer())
        {
        }

        public CookieAwareWebClient(CookieContainer cookies)
        {
            this.CookieContainer = cookies;
        }

        public CookieAwareWebClient(string uri)
        {
            this.Uri = uri;
        }

        public CookieAwareWebClient(System.Windows.Forms.WebBrowser webbrowser)
        {
            this.Uri = webbrowser.Url.AbsoluteUri;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);

            request.Timeout = TimeOut * 1000;

            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = this.CookieContainer;
            }
            HttpWebRequest httpRequest = (HttpWebRequest)request;
            httpRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            
            return httpRequest;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = null;

            response = base.GetWebResponse(request);

            this.ResponseUri = response.ResponseUri.AbsoluteUri;

            return response;
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string pchURL, string pchCookieName,StringBuilder pchCookieData, ref uint pcchCookieData, int dwFlags, IntPtr lpReserved);
        const int INTERNET_COOKIE_HTTPONLY = 0x00002000;

        public string GetGlobalCookies(string uri)
        {
            uint uiDataSize = 2048;
            StringBuilder sbCookieData = new StringBuilder((int)uiDataSize);
            if (InternetGetCookieEx(uri, null, sbCookieData, ref uiDataSize,
                INTERNET_COOKIE_HTTPONLY, IntPtr.Zero) && sbCookieData.Length > 0)
            {
                return sbCookieData.ToString().Replace(";", ",");
            }
            else
            {
                return null;
            }
        }

        public string PostData(string PostUrl, string PostData)
        {
        BasaDon:

            while (this.IsBusy)
            {
                System.Threading.Thread.Sleep(100);
            };

            string sonuc = "";

            int sayac = 0;

        TimeOutOlursaTekrarDene:

            try
            {

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                string sTmpCookieString = this.Cookie;
                sTmpCookieString = sTmpCookieString.Replace("__utma=265521805.1716287094.1519731524.1519731524.1519731524.1, __utmz=265521805.1519731524.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none), ", "");
                byte[] bByteArray = Encoding.UTF8.GetBytes(PostData);
                this.CookieContainer = new CookieContainer();
                this.CookieContainer.SetCookies(new Uri(PostUrl), sTmpCookieString);
                this.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                //this.Headers[HttpRequestHeader.CacheControl] = "no-cache";
                this.Headers[HttpRequestHeader.Referer] = string.IsNullOrEmpty(this.Referer) ? PostUrl.StartsWith("https://uyg.sgk.gov.tr/YeniSistem") ? "https://uyg.sgk.gov.tr/YeniSistem" : this.Referer : this.Referer;
                this.Headers[HttpRequestHeader.AcceptLanguage] = "tr-TR";
                var result = this.UploadData(PostUrl, bByteArray);

                var encoding = "UTF-8";

                var contentType = this.ResponseHeaders[HttpResponseHeader.ContentType];

                if (!string.IsNullOrEmpty(contentType) && contentType.Contains("charset"))
                {
                    encoding = contentType.Split(';').FirstOrDefault(p => p.Contains("charset")).Split('=')[1];
                }

                //byte[] converted = Encoding.Convert(Encoding.GetEncoding("iso-8859-9"),Encoding.UTF8, result);

                sonuc = System.Net.WebUtility.HtmlDecode(System.Text.Encoding.GetEncoding(encoding).GetString(result));
            }
            catch (WebException wex)
            {
                if (wex.Status == WebExceptionStatus.Timeout)
                {
                    sonuc = "Error";

                    sayac++;

                    if (sayac > 2)
                    {
                        this.TimeOut = 100;
                    }

                    if (sayac < 5)
                    {
                        System.Threading.Thread.Sleep(2000);

                        goto TimeOutOlursaTekrarDene;
                    }

                }
                else sonuc = "Error";
            }
            catch (NotSupportedException)
            {
                System.Threading.Thread.Sleep(500);

                goto BasaDon;
            }
            catch
            {
                sonuc = "Error";
            }

            this.TimeOut = Program.ZamanAsimiSuresi;

            return sonuc;
        }

        public string Get(string Url, string QueryString)
        {
            if (!string.IsNullOrEmpty(QueryString)) Url += "?" + QueryString;
        BasaDon:

            while (this.IsBusy)
            {
                System.Threading.Thread.Sleep(100);
            };

            string sonuc = "";

            int sayac = 0;

        TimeOutOlursaTekrarDene:

            try
            {

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                //string sTmpCookieString = GetGlobalCookies(this.Uri);
                string sTmpCookieString = this.Cookie;
                if (sTmpCookieString != null)
                {
                    sTmpCookieString = sTmpCookieString.Replace("__utma=265521805.1716287094.1519731524.1519731524.1519731524.1, __utmz=265521805.1519731524.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none), ", "");
                    //string sPostData = "tip=tahakkukonayliFisHizmetPdf&download=true&hizmet_yil_ay_index=" + "0" + "&hizmet_yil_ay_index_bitis=" + "10" + "&bildirgeRefNo=" + bildirgeRefNo + "&action%3AtahakkukonayliFisHizmetPdf=Hizmet+Listesi.PDF";
                    this.CookieContainer = new CookieContainer();
                    this.CookieContainer.SetCookies(new Uri(Url), sTmpCookieString);
                }

                this.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                
                //this.Headers[HttpRequestHeader.CacheControl] = "no-cache";
                this.Headers[HttpRequestHeader.Referer] = string.IsNullOrEmpty(this.Referer) ? Url.StartsWith("https://uyg.sgk.gov.tr/YeniSistem") ? "https://uyg.sgk.gov.tr/YeniSistem" : this.Referer : this.Referer;
                this.Headers[HttpRequestHeader.AcceptLanguage] = "tr-TR";

                var bytes = this.DownloadData(Url);

                var encoding = "UTF-8";

                var contentType = this.ResponseHeaders[HttpResponseHeader.ContentType];

                if (!string.IsNullOrEmpty(contentType) && contentType.Contains("charset"))
                {
                    encoding = contentType.Split(';').FirstOrDefault(p => p.Contains("charset")).Split('=')[1];
                }

                //byte[] converted = Encoding.Convert(Encoding.GetEncoding("iso-8859-9"),Encoding.UTF8, result);

                sonuc = System.Net.WebUtility.HtmlDecode(System.Text.Encoding.GetEncoding(encoding).GetString(bytes));
            }
            catch (WebException wex)
            {
                if (wex.Status == WebExceptionStatus.Timeout)
                {
                    sonuc = "Error";

                    sayac++;

                    if (sayac > 2)
                    {
                        this.TimeOut = 100;
                    }

                    if (sayac < 5)
                    {
                        System.Threading.Thread.Sleep(2000);

                        goto TimeOutOlursaTekrarDene;
                    }


                }
                else sonuc = "Error";
            }
            catch (NotSupportedException)
            {
                System.Threading.Thread.Sleep(500);

                goto BasaDon;
            }
            catch
            {
                sonuc = "Error";
            }

            this.TimeOut = Program.ZamanAsimiSuresi;

            return sonuc;

        }

        public byte[] DownloadFileGet(string Url, string QueryString)
        {

            while (this.IsBusy)
            {
                System.Threading.Thread.Sleep(100);
            };

            byte[] sonuc = new byte[0];

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                //string sTmpCookieString = GetGlobalCookies(this.Uri);
                string sTmpCookieString = this.Cookie;
                sTmpCookieString = sTmpCookieString.Replace("__utma=265521805.1716287094.1519731524.1519731524.1519731524.1, __utmz=265521805.1519731524.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none), ", "");
                //string sPostData = "tip=tahakkukonayliFisHizmetPdf&download=true&hizmet_yil_ay_index=" + "0" + "&hizmet_yil_ay_index_bitis=" + "10" + "&bildirgeRefNo=" + bildirgeRefNo + "&action%3AtahakkukonayliFisHizmetPdf=Hizmet+Listesi.PDF";
                this.CookieContainer = new CookieContainer();
                this.CookieContainer.SetCookies(new Uri(Url), sTmpCookieString);
                this.Headers[HttpRequestHeader.Referer] = string.IsNullOrEmpty(this.Referer) ? Url.StartsWith("https://uyg.sgk.gov.tr/YeniSistem") ? "https://uyg.sgk.gov.tr/YeniSistem" : this.Referer : this.Referer;
                var bytes = this.DownloadData(Url);
                return bytes;
            }
            catch
            {

            }
            finally
            {
            }

            return sonuc;

        }

        public byte[] DownloadFilePost(string PostUrl, string PostData)
        {
            while (this.IsBusy)
            {
                System.Threading.Thread.Sleep(100);
            };

            byte[] sonuc = new byte[0];

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                string sTmpCookieString = this.Cookie;
                sTmpCookieString = sTmpCookieString.Replace("__utma=265521805.1716287094.1519731524.1519731524.1519731524.1, __utmz=265521805.1519731524.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none), ", "");
                byte[] bByteArray = Encoding.UTF8.GetBytes(PostData);
                this.CookieContainer = new CookieContainer();
                this.CookieContainer.SetCookies(new Uri(PostUrl), sTmpCookieString);
                this.Headers[HttpRequestHeader.Referer] = string.IsNullOrEmpty(this.Referer) ? PostUrl.StartsWith("https://uyg.sgk.gov.tr/YeniSistem") ? "https://uyg.sgk.gov.tr/YeniSistem" : this.Referer : this.Referer;
                this.Headers[HttpRequestHeader.AcceptLanguage] = "tr-TR";
                this.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var result = this.UploadData(PostUrl, bByteArray);

                return result;
            }
            catch
            {

            }
            finally
            {
            }

            return sonuc;

        }
    }

}
