using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TesvikProgrami.Classes
{
    public class TwoCaptcha
    {
        private string ApiKey = "0ce95322b0d2f2752d61bd72de7f9f52";

        private string RequestId;

        public string Hata { get; set; }

        public string Sonuc { get; set; }

        public byte[] Captcha { get; set; }
        public bool IsCaseSensitive { get; set; }

        public TwoCaptcha() { }
        public TwoCaptcha(byte[] captcha, bool isCaseSensitive)
        {
            Captcha = captcha;
            IsCaseSensitive = isCaseSensitive;
        }


        public async void CaptchaSorgula()
        {
            using (var client = new HttpClient())
            {
                using (var content =
                    new MultipartFormDataContent())
                {
                    content.Add(new StreamContent(new MemoryStream(this.Captcha)), "file", "captcha.jpg");

                    var istekSayisi = 0;

                IstekGonder:

                    istekSayisi++;

                    using (
                       var message =
                            client.PostAsync(String.Format("https://2captcha.com/in.php?key={0}&method={1}&regsense={2}", this.ApiKey, "post", Convert.ToInt32(true /*this.IsCaseSensitive*/) ), content))
                    {
                        try
                        {
                            string response = await message.Result.Content.ReadAsStringAsync();

                            if (response.StartsWith("OK|"))
                            {
                                this.RequestId = response.Split('|')[1];

                                var denemeSayisi = 0;

                            SonucuCek:

                                Thread.Sleep(TimeSpan.FromSeconds(5));

                                denemeSayisi++;

                                using (var messageResponse = client.GetAsync(String.Format("https://2captcha.com/res.php?key={0}&action=get&id={1}", this.ApiKey, this.RequestId)))
                                {
                                    string sonuc = await messageResponse.Result.Content.ReadAsStringAsync();

                                    if (sonuc.StartsWith("OK|"))
                                    {
                                        this.Sonuc = sonuc.Split('|')[1];
                                    }
                                    else if (sonuc.Contains("CAPCHA_NOT_READY"))
                                    {
                                        if (denemeSayisi < 10)
                                        {
                                            goto SonucuCek;
                                        }
                                        else this.Hata = "TIMEOUT_RESPONSE";

                                    }
                                    else
                                    {
                                        this.Hata = sonuc;
                                    }
                                }

                            }
                            else if (response.Contains("ERROR_ZERO_BALANCE"))
                            {
                                this.Hata = "HESAPTA_PARA_YOK";
                            }
                            else if (response.Contains("ERROR_ZERO_CAPTCHA_FILESIZE"))
                            {
                                this.Hata = "ERROR_ZERO_CAPTCHA_FILESIZE";
                            }
                            else if (response.Contains("ERROR_TOO_BIG_CAPTCHA_FILESIZE"))
                            {
                                this.Hata = "ERROR_TOO_BIG_CAPTCHA_FILESIZE";
                            }
                            else if (response.Contains("ERROR_WRONG_FILE_EXTENSION"))
                            {
                                this.Hata = "ERROR_WRONG_FILE_EXTENSION";
                            }
                            else if (response.Contains("ERROR_IMAGE_TYPE_NOT_SUPPORTED"))
                            {
                                this.Hata = "ERROR_IMAGE_TYPE_NOT_SUPPORTED";
                            }
                            else if (response.Contains("MAX_USER_TURN"))
                            {

                                if (istekSayisi < 10)
                                {
                                    Thread.Sleep(TimeSpan.FromSeconds(10));

                                    goto IstekGonder;
                                }
                                else this.Hata = "TIMEOUT_REQUEST";

                            }
                            else if (response.Contains("ERROR: 1003"))
                            {
                                if (istekSayisi < 10)
                                {
                                    Thread.Sleep(TimeSpan.FromSeconds(30));

                                    goto IstekGonder;
                                }
                                else this.Hata = "TIMEOUT_REQUEST";
                            }
                            else this.Hata = response;
                        }
                        catch (Exception ex)
                        {
                            this.Hata = ex.Message;
                        }

                    }
                }
            }

            //WebClient wc = new WebClient();

            //NameValueCollection parametreler = new NameValueCollection();
            //parametreler.Add("key", this.ApiKey);
            //parametreler.Add("method", "post");

            //string params= String.Format("key")

            //byte[] bByteArray = Encoding.UTF8.GetBytes(PostData);
            //this.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            ////this.Headers[HttpRequestHeader.CacheControl] = "no-cache";
            //this.Headers[HttpRequestHeader.Referer] = string.IsNullOrEmpty(this.Referer) ? PostUrl.StartsWith("https://uyg.sgk.gov.tr/YeniSistem") ? "https://uyg.sgk.gov.tr/YeniSistem" : this.Referer : this.Referer;
            //this.Headers[HttpRequestHeader.AcceptLanguage] = "tr-TR";
            //var result = this.UploadData(PostUrl, bByteArray);
        }

        public void BadCaptchaReport()
        {
            WebClient wc = new WebClient();

            var denemeSayisi = 0;

        Gonder:

            try
            {
                denemeSayisi++;

                var sonuc= wc.DownloadString(String.Format("https://2captcha.com/res.php?key={0}&action=reportbad&id={1}", this.ApiKey, this.RequestId));

                if (! sonuc.Contains("OK_REPORT_RECORDED"))
                    File.AppendAllText(Path.Combine(Application.StartupPath, "CaptchaHatalar.txt"), sonuc+ Environment.NewLine);
            }
            catch
            {
                if (denemeSayisi < 3)
                {
                    Thread.Sleep(500);

                    goto Gonder;
                }
            }

        }

        public async void GoodCaptchaReport()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    await wc.DownloadStringTaskAsync(String.Format("https://2captcha.com/res.php?key={0}&action=reportgood&id={1}", this.ApiKey, this.RequestId));
                }
            }
            catch { }
        }

        public string BakiyeSorgula()
        {

            WebClient wc = new WebClient();

            var denemeSayisi = 0;

        Gonder:

            try
            {
                denemeSayisi++;

                var result = wc.DownloadString(String.Format("https://2captcha.com/res.php?key={0}&action=getbalance", this.ApiKey));

                if (decimal.TryParse(result, out decimal sonuc))
                {
                    return result;
                }
            }
            catch
            {
                if (denemeSayisi < 3)
                {
                    Thread.Sleep(500);

                    goto Gonder;
                }
            }

            return "-1";
        }
    }
}
