using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace TesvikProgrami.Classes
{
    public class ProjeGiris : CookieAwareWebClient
    {
        bool KullaniciyaSor = false;
        public Enums.ProjeTurleri proje;
        Isyerleri isyeri;
        public bool Connected = false;
        int CaptchaUzunlugu = 0;
        string CookieUri;
        string CaptchaUri;
        string LogOutUri;
        List<string> IlkGirisMetni = new List<string>();
        string HataliKullaniciAdiMetni;
        string HataliGuvenlikKoduMetni;
        string GirisSayfasiMetni;
        string OtomatikBosCaptchaGirmeAyariEtiketAdi;
        string LoginPostUri;
        string LoginParams;
        public string Ticket;
        bool GirisYapiliyor = false;
        bool DosyayaYaziliyor = false;
        public bool Kullanimda = false;
        bool CaseSensitiveCaptcha = false;
        bool CaptchaToUpper = false;
        public Guid oturumId { get; set; } = new Guid();

        public bool GirisYapilamiyor = false;
        public string GirisYapilamamaNedeni = null;

        public bool CaptchaGirilecek = true;

        bool _bitti;
        public bool Bitti
        {
            get { return _bitti; }
            set
            {
                _bitti = value;

                //if (_bitti)
                //{
                //    this.Disconnect();
                //}
            }
        }

        public ProjeGiris(Isyerleri pIsyeri, Enums.ProjeTurleri projeTuru)
        {
            switch (projeTuru)
            {
                case Enums.ProjeTurleri.IsverenSistemi:
                    this.CookieUri = "https://uyg.sgk.gov.tr/IsverenSistemi/PG";
                    this.CaptchaUri = "https://uyg.sgk.gov.tr/IsverenSistemi/PG";
                    this.LogOutUri = "https://uyg.sgk.gov.tr/IsverenSistemi/logout.jsp";
                    this.KullaniciyaSor = true;
                    this.CaptchaUzunlugu = 5;
                    this.IlkGirisMetni = new List<string> { "UYGULAMA BAŞLATILDI" };
                    this.HataliKullaniciAdiMetni = "Kullanıcı adı veya şifreleriniz hatalıdır";
                    this.HataliGuvenlikKoduMetni = "Güvenlik Anahtarı hatalıdır";
                    this.GirisSayfasiMetni = "İşveren Sistemi Kullanıcı Girişi";
                    this.OtomatikBosCaptchaGirmeAyariEtiketAdi = "IsverenSistemiGuvenlikKoduGirisi";
                    this.LoginPostUri = "https://uyg.sgk.gov.tr/IsverenSistemi/login/kullaniciIlkKontrollerGiris.action";
                    this.LoginParams = "username={0}&isyeri_kod={1}&password={2}&isyeri_sifre={3}&isyeri_guvenlik={4}";
                    break;
                case Enums.ProjeTurleri.EBildirgeV2:
                    this.CookieUri = "https://ebildirge.sgk.gov.tr/EBildirgeV2/PG";
                    this.CaptchaUri = "https://ebildirge.sgk.gov.tr/EBildirgeV2/PG";
                    this.LogOutUri = "https://ebildirge.sgk.gov.tr/EBildirgeV2/logout.jsp";
                    this.KullaniciyaSor = true;
                    this.CaptchaUzunlugu = 5;
                    this.IlkGirisMetni = new List<string> { "Aylık Prim Hizmet Belgesi Girişi" };
                    this.HataliKullaniciAdiMetni = "Kullanıcı adı veya şifreleriniz hatalıdır";
                    this.HataliGuvenlikKoduMetni = "Güvenlik Anahtarı hatalıdır";
                    this.GirisSayfasiMetni = "E-BildirgeV2 Kullanıcı Girişi";
                    this.OtomatikBosCaptchaGirmeAyariEtiketAdi = "EBildirgeV2GuvenlikKoduGirisi";
                    this.LoginPostUri = "https://ebildirge.sgk.gov.tr/EBildirgeV2/login/kullaniciIlkKontrollerGiris.action";
                    this.LoginParams = "username={0}&isyeri_kod={1}&password={2}&isyeri_sifre={3}&isyeri_guvenlik={4}";
                    this.Referer = "https://ebildirge.sgk.gov.tr/EBildirgeV2";
                    break;
                case Enums.ProjeTurleri.Bf6645:
                    this.CookieUri = "https://uyg.sgk.gov.tr/Sigortali_Tesvik_4447_15/login.jsp";
                    this.CaptchaUri = "https://uyg.sgk.gov.tr/Sigortali_Tesvik_4447_15/captcha";
                    this.LogOutUri = "https://uyg.sgk.gov.tr/Sigortali_Tesvik_4447_15/ActionMultiplexer?aid=LogOutControl";
                    this.KullaniciyaSor = true;
                    this.CaptchaUzunlugu = 5;
                    this.IlkGirisMetni = new List<string> { "HOŞ GELDİNİZ" };
                    this.HataliKullaniciAdiMetni = "Lütfen verilerinizi kontrol ederek tekrar deneyiniz";
                    this.HataliGuvenlikKoduMetni = "Güvenlik Resmini Hatalı Girdiniz";
                    this.GirisSayfasiMetni = "Sisteme Giriş";
                    this.OtomatikBosCaptchaGirmeAyariEtiketAdi = "GuvenlikKoduGirisi6645";
                    this.LoginPostUri = "https://uyg.sgk.gov.tr/Sigortali_Tesvik_4447_15/ActionMultiplexer?aid=LoginControl";
                    this.LoginParams = "j_username={0}&isyeri_kod={1}&j_password={2}&isyeri_sifre={3}&captcha_image={4}&Submit=TAMAM";
                    this.CaseSensitiveCaptcha = true;
                    this.CaptchaToUpper = true;
                    break;
                case Enums.ProjeTurleri.Bf687:
                    this.CookieUri = "https://uyg.sgk.gov.tr/Sigortali_Tesvik_687_KHK/login.jsp";
                    this.CaptchaUri = "https://uyg.sgk.gov.tr/Sigortali_Tesvik_687_KHK/captcha";
                    this.LogOutUri = "https://uyg.sgk.gov.tr/Sigortali_Tesvik_687_KHK/ActionMultiplexer?aid=LogOutControl";
                    this.KullaniciyaSor = true;
                    this.CaptchaUzunlugu = 5;
                    this.IlkGirisMetni = new List<string> { "HOŞ GELDİNİZ" };
                    this.HataliKullaniciAdiMetni = "Lütfen verilerinizi kontrol ederek tekrar deneyiniz";
                    this.HataliGuvenlikKoduMetni = "Güvenlik Resmini Hatalı Girdiniz";
                    this.GirisSayfasiMetni = "Sisteme Giriş";
                    this.OtomatikBosCaptchaGirmeAyariEtiketAdi = "GuvenlikKoduGirisi687";
                    this.LoginPostUri = "https://uyg.sgk.gov.tr/Sigortali_Tesvik_687_KHK/ActionMultiplexer?aid=LoginControl";
                    //this.LoginParams = "j_username={0}&isyeri_kod={1}&j_password={2}&isyeri_sifre={3}&captcha_image={4}&Submit=TAMAM";
                    this.LoginParams = "j_username={0}&isyeri_kod={1}&j_password={2}&isyeri_sifre={3}&captcha_image={4}&Submit=TAMAM";
                    this.CaseSensitiveCaptcha = true;
                    this.CaptchaToUpper = true;
                    this.CaptchaGirilecek = true;
                    break;
                case Enums.ProjeTurleri.Bf14857:
                    this.CookieUri = "https://uyg.sgk.gov.tr/Sigortali_Tesvik_4a/login.jsp";
                    this.CaptchaUri = "https://uyg.sgk.gov.tr/Sigortali_Tesvik_4a/captcha";
                    this.LogOutUri = "https://uyg.sgk.gov.tr/Sigortali_Tesvik_4a/ActionMultiplexer?aid=LogOutControl";
                    this.KullaniciyaSor = true;
                    this.CaptchaUzunlugu = 5;
                    this.IlkGirisMetni = new List<string> { "HOŞ GELDİNİZ" };
                    this.HataliKullaniciAdiMetni = "Lütfen verilerinizi kontrol ederek tekrar deneyiniz";
                    this.HataliGuvenlikKoduMetni = "Güvenlik Resmini Hatalı Girdiniz";
                    this.GirisSayfasiMetni = "Sisteme Giriş";
                    this.OtomatikBosCaptchaGirmeAyariEtiketAdi = "GuvenlikKoduGirisi14857";
                    this.LoginPostUri = "https://uyg.sgk.gov.tr/Sigortali_Tesvik_4a/ActionMultiplexer?aid=LoginControl";
                    this.LoginParams = "j_username={0}&isyeri_kod={1}&j_password={2}&isyeri_sifre={3}&captcha_image={4}&Submit=TAMAM";
                    this.CaptchaToUpper = true;
                    this.CaseSensitiveCaptcha = true;
                    break;
                case Enums.ProjeTurleri.EBildirgeV1:
                    //this.CookieUri = "https://ebildirge.sgk.gov.tr/WPEB/amp/loginldap";
                    this.CookieUri = "https://ebildirge.sgk.gov.tr/WPEB/PG";
                    this.CaptchaUri = "https://ebildirge.sgk.gov.tr/WPEB/PG";
                    this.LogOutUri = "https://ebildirge.sgk.gov.tr/WPEB/logoutExitPage";
                    this.KullaniciyaSor = false;
                    this.CaptchaUzunlugu = 4;
                    //this.IlkGirisMetni = "WPEB/amp/ToAnaMenu";
                    this.IlkGirisMetni = new List<string> { "Aylık Prim ve Hizmet Belgesi Giriş Ana Menü", "yeni vekaletin ilgili Sigorta Müdürlüğüne götürülmemesi halinde" };
                    this.HataliKullaniciAdiMetni = "Lütfen verilerinizi kontrol ederek tekrar deneyiniz";
                    this.HataliGuvenlikKoduMetni = "Güvenlik Anahtarı hatalıdır";
                    this.GirisSayfasiMetni = "E-Bildirge Kullanıcı Girişi";
                    this.LoginPostUri = "https://ebildirge.sgk.gov.tr/WPEB/amp/loginldap";
                    this.LoginParams = "j_username={0}&isyeri_kod={1}&j_password={2}&isyeri_sifre={3}&isyeri_guvenlik={4}&btnSubmit=Giri%FE";
                    break;
                case Enums.ProjeTurleri.SigortaliIstenAyrilis:
                    this.CookieUri = "https://uyg.sgk.gov.tr/SigortaliTescil/PG";
                    this.CaptchaUri = "https://uyg.sgk.gov.tr/SigortaliTescil/PG";
                    this.LogOutUri = "https://uyg.sgk.gov.tr/SigortaliTescil/logoutExitPage";
                    this.KullaniciyaSor = false;
                    this.CaptchaUzunlugu = 4;
                    this.IlkGirisMetni = new List<string> { "SİGORTALI İŞE GİRİŞ-AYRILIŞ BİLDİRGELERİ", "yeni vekaletin ilgili Sigorta Müdürlüğüne götürülmemesi halinde" };
                    this.HataliKullaniciAdiMetni = "Lütfen verilerinizi kontrol ederek tekrar deneyiniz";
                    this.HataliGuvenlikKoduMetni = "Güvenlik Anahtarı hatalıdır";
                    this.GirisSayfasiMetni = "E-Bildirge Kullanıcı Girişi";
                    this.LoginPostUri = "https://uyg.sgk.gov.tr/SigortaliTescil/amp/loginldap";
                    this.LoginParams = "j_username={0}&isyeri_kod={1}&j_password={2}&isyeri_sifre={3}&isyeri_guvenlik={4}&buttonOK=Giri%FE";
                    break;
                case Enums.ProjeTurleri.IsverenBorcSorgu:
                    this.CookieUri = "https://uyg.sgk.gov.tr/IsverenBorcSorgu/";
                    this.CaptchaUri = "https://uyg.sgk.gov.tr/IsverenBorcSorgu/simpleCaptcha.png";
                    this.LogOutUri = "https://uyg.sgk.gov.tr/IsverenBorcSorgu/login/userLogout.action";
                    this.KullaniciyaSor = true;
                    this.CaptchaUzunlugu = 5;
                    this.IlkGirisMetni = new List<string> { "/IsverenBorcSorgu/donemselBorc.action" };
                    this.HataliKullaniciAdiMetni = "Lütfen verilerinizi kontrol ederek tekrar deneyiniz";
                    this.HataliGuvenlikKoduMetni = "Güvenlik kodunu doğru giriniz";
                    this.GirisSayfasiMetni = "Kullanıcı Giriş Bilgileri";
                    this.LoginPostUri = "https://uyg.sgk.gov.tr/IsverenBorcSorgu/login/userLogin.action";
                    this.LoginParams = "basvuru.tcKimlikNo={0}&basvuru.isyeriKodu={1}&basvuru.sistemSifre={2}&basvuru.isyeriSifre={3}&captchaStr={4}";
                    this.CaseSensitiveCaptcha = true;
                    this.CaptchaToUpper = true;
                    break;
                default:
                    break;
            }

            isyeri = pIsyeri;
            proje = projeTuru;
        }

        public string Connect()
        {
            if (!this.GirisYapiliyor)
            {
                int captchaSayac = 0;

            YenidenDene:

                this.GirisYapiliyor = true;

                LogYaz("Web Client sisteme giriş yapacak");

                if (_bitti)
                {
                    LogYaz("Web Client bitti olarak ayarlandığı için sisteme giriş yapılmayacak");

                    this.GirisYapiliyor = false;

                    return "LogOut";
                }

                try
                {
                    while (this.IsBusy) { System.Threading.Thread.Sleep(100); }

                    this.Headers[HttpRequestHeader.CacheControl] = "no-cache";

                    this.CookieContainer = new CookieContainer();

                    var data = this.DownloadData(this.CookieUri);

                    this.Cookie = String.Join(",", this.ResponseHeaders["Set-Cookie"].Split(';').SelectMany(p => p.Split(',')).Where(p => p.Contains("=") && !new List<string> { "path", "domain" }.Contains(p.Split('=')[0].Trim().ToLower())));

                    Bitmap cloneCaptcha = null;

                    string captchaKodu = null;

                    TwoCaptcha twoCaptcha = null;

                    bool TwoCaptchaMi = false;

                    bool OtomatikCaptchaGirilecek = false;

                    if (this.CaptchaGirilecek)
                    {

                        if (!this.CookieUri.Equals(this.CaptchaUri))
                        {
                            LogYaz("Web Client güvenlik resmini indirecek");
                            while (this.IsBusy) { System.Threading.Thread.Sleep(100); }
                            this.Headers[HttpRequestHeader.CacheControl] = "no-cache";
                            data = this.DownloadData(CaptchaUri);
                            LogYaz("Web Client güvenlik resmini indirdi");
                        }

                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            Image captchaImage = Image.FromStream(ms);

                            cloneCaptcha = new Bitmap(captchaImage.Width, captchaImage.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                            using (Graphics gr = Graphics.FromImage(cloneCaptcha))
                                gr.DrawImage(captchaImage, new Rectangle(0, 0, cloneCaptcha.Width, cloneCaptcha.Height));
                        }


                        switch (proje)
                        {
                            case Enums.ProjeTurleri.IsverenSistemi:
                                OtomatikCaptchaGirilecek = Program.OtomatikGuvenlikKoduGirilecekIsverenSistemi;
                                break;
                            case Enums.ProjeTurleri.EBildirgeV2:
                                OtomatikCaptchaGirilecek = Program.OtomatikGuvenlikKoduGirilecekEBildirgeV2;
                                break;
                            case Enums.ProjeTurleri.Bf6645:
                                OtomatikCaptchaGirilecek = Program.OtomatikGuvenlikKoduGirilecek6645;
                                break;
                            case Enums.ProjeTurleri.Bf687:
                                OtomatikCaptchaGirilecek = Program.OtomatikGuvenlikKoduGirilecek687;
                                break;
                            case Enums.ProjeTurleri.Bf14857:
                                OtomatikCaptchaGirilecek = Program.OtomatikGuvenlikKoduGirilecek14857;
                                break;
                            default:
                                break;
                        }


                        if (!OtomatikCaptchaGirilecek)
                        {
                            if (!KullaniciyaSor)
                            {

                                Tesseract.TesseractEngine eng = new Tesseract.TesseractEngine("tessdata", "eng");

                                var page = eng.Process(cloneCaptcha, Tesseract.PageSegMode.SingleWord);

                                var sonuc = page.GetText();

                                captchaKodu = sonuc.Length > this.CaptchaUzunlugu ? sonuc.Substring(0, this.CaptchaUzunlugu) : sonuc;
                            }
                            else
                            {

                                bool CaptchaFormuAc = true;

                                if (Program.GuvenlikKoduCozdur)
                                {
                                    captchaSayac++;

                                    twoCaptcha = new TwoCaptcha(data, this.CaseSensitiveCaptcha);

                                    twoCaptcha.CaptchaSorgula();

                                    if (String.IsNullOrEmpty(twoCaptcha.Hata))
                                    {
                                        captchaKodu = this.CaptchaToUpper ? twoCaptcha.Sonuc.ToUpper() : twoCaptcha.Sonuc;

                                        TwoCaptchaMi = !String.IsNullOrEmpty(twoCaptcha.Sonuc);

                                        CaptchaFormuAc = String.IsNullOrEmpty(twoCaptcha.Sonuc);
                                    }

                                    if (!string.IsNullOrEmpty(twoCaptcha.Hata) || string.IsNullOrEmpty(twoCaptcha.Sonuc))
                                    {
                                        File.AppendAllText(Path.Combine(Application.StartupPath, "CaptchaHatalar.txt"), (string.IsNullOrEmpty(twoCaptcha.Hata) ? "Bilinmeyen hata" : twoCaptcha.Hata) + Environment.NewLine);

                                        if (captchaSayac < 5)
                                        {
                                            Thread.Sleep(1000);

                                            goto YenidenDene;
                                        }
                                    }
                                }

                                if (CaptchaFormuAc)
                                {
                                    using (var frmCaptcha = new Captcha())
                                    {
                                        frmCaptcha.captcha = cloneCaptcha;
                                        frmCaptcha.TopMost = true;
                                        if (frmCaptcha.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                        {

                                            if (String.IsNullOrEmpty(frmCaptcha.ReturnValue)) goto YenidenDene;

                                            captchaKodu = frmCaptcha.ReturnValue;
                                        }
                                        else
                                        {
                                            Connected = false;

                                            this.GirisYapiliyor = false;

                                            this.GirisYapilamiyor = true;

                                            this.GirisYapilamamaNedeni = "Güvenlik kodu girilmedi";

                                            return "Güvenlik kodu girilmedi";
                                        }
                                    }
                                }
                            }

                        }
                        else captchaKodu = new String('1', this.CaptchaUzunlugu);

                    }

                    if (!this.CaptchaGirilecek || !String.IsNullOrEmpty(captchaKodu))
                    {

                        if (String.IsNullOrEmpty(captchaKodu)) captchaKodu = "";

                        LogYaz("Web Client login urlsine istek gönderecek");

                        string response = base.PostData(this.LoginPostUri, String.Format(this.LoginParams, isyeri.KullaniciAdi.Trim(), isyeri.KullaniciKod.Trim(), isyeri.SistemSifresi.Trim(), isyeri.IsyeriSifresi.Trim(), captchaKodu.Trim()));

                        LogYaz("Web Client login urlsine istek gönderildi");

                        if (response.Contains("VERGİ KİMLİK NUMARASI DOĞRULAMA") && this.proje == Enums.ProjeTurleri.EBildirgeV1)
                        {
                            Connected = true;

                            this.GirisYapiliyor = false;

                            int denemeSayisi = 0;

                        YenidenDene2:

                            data = this.GetData("https://ebildirge.sgk.gov.tr/WPEB/simpleCaptcha.png", "");

                            cloneCaptcha = null;

                            using (MemoryStream ms = new MemoryStream(data))
                            {
                                Image captchaImage = Image.FromStream(ms);

                                cloneCaptcha = new Bitmap(captchaImage.Width, captchaImage.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                                using (Graphics gr = Graphics.FromImage(cloneCaptcha))
                                    gr.DrawImage(captchaImage, new Rectangle(0, 0, cloneCaptcha.Width, cloneCaptcha.Height));
                            }

                            captchaKodu = null;

                            twoCaptcha = null;

                            TwoCaptchaMi = false;

                            captchaSayac = 0;

                            bool CaptchaFormuAc = true;

                            if (Program.GuvenlikKoduCozdur)
                            {
                                captchaSayac++;

                                twoCaptcha = new TwoCaptcha(data, true);

                                twoCaptcha.CaptchaSorgula();

                                if (String.IsNullOrEmpty(twoCaptcha.Hata))
                                {
                                    captchaKodu = this.CaptchaToUpper ? twoCaptcha.Sonuc.ToUpper() : twoCaptcha.Sonuc;

                                    TwoCaptchaMi = !String.IsNullOrEmpty(twoCaptcha.Sonuc);

                                    CaptchaFormuAc = String.IsNullOrEmpty(twoCaptcha.Sonuc);
                                }

                                if (!string.IsNullOrEmpty(twoCaptcha.Hata) || string.IsNullOrEmpty(twoCaptcha.Sonuc))
                                {
                                    File.AppendAllText(Path.Combine(Application.StartupPath, "CaptchaHatalar.txt"), (string.IsNullOrEmpty(twoCaptcha.Hata) ? "Bilinmeyen hata" : twoCaptcha.Hata) + Environment.NewLine);

                                    if (captchaSayac < 5)
                                    {
                                        Thread.Sleep(1000);

                                        goto YenidenDene2;
                                    }
                                }
                            }

                            if (CaptchaFormuAc)
                            {
                                Captcha frmCaptcha = new Captcha();
                                frmCaptcha.captcha = cloneCaptcha;
                                frmCaptcha.TopMost = true;
                                frmCaptcha.Text = "Vergi No Doğrulama";
                                if (frmCaptcha.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {

                                    if (String.IsNullOrEmpty(frmCaptcha.ReturnValue)) goto YenidenDene2;

                                    captchaKodu = frmCaptcha.ReturnValue;
                                }
                                else
                                {
                                    Connected = false;

                                    this.GirisYapiliyor = false;

                                    this.GirisYapilamiyor = true;

                                    this.GirisYapilamamaNedeni = "Güvenlik kodu girilmedi";

                                    return "Güvenlik kodu girilmedi";
                                }
                            }

                            if (!String.IsNullOrEmpty(captchaKodu))
                            {
                                response = this.PostData("https://ebildirge.sgk.gov.tr/WPEB/amp/vknUyumBeyan", string.Format("hdnsecim=0&vknUyumluMu=on&captchaVknUyum={0}&btnUyumlu=DEVAM", captchaKodu));

                                if (response.Contains("Hatalı güvenlik kodu"))
                                {
                                    goto YenidenDene2;
                                }
                                else if (this.IlkGirisMetni.Any(p => response.Contains(p)))
                                {
                                    Connected = true;

                                    this.GirisYapiliyor = false;

                                    if (TwoCaptchaMi)
                                    {
                                        twoCaptcha.GoodCaptchaReport();
                                    }

                                    return "OK";
                                }
                                else
                                {
                                    denemeSayisi++;

                                    if (denemeSayisi < 5)
                                    {
                                        goto YenidenDene2;
                                    }
                                    else
                                    {
                                        Connected = false;

                                        this.GirisYapiliyor = false;

                                        this.GirisYapilamiyor = true;

                                        this.GirisYapilamamaNedeni = "5 denemeye rağmen vergi kimlik numarası doğrulaması gerçekleştirilemedi";

                                        return "5 denemeye rağmen vergi kimlik numarası doğrulaması gerçekleştirilemedi";
                                    }
                                }
                            }
                        }

                        if (this.proje == Enums.ProjeTurleri.IsverenBorcSorgu)
                        {
                            if (this.ResponseUri.Equals("https://uyg.sgk.gov.tr/IsverenBorcSorgu/borc/donemselBorc.action"))
                            {
                                LogYaz("Web Client sisteme başarıyla giriş yaptı");

                                Connected = true;

                                this.GirisYapiliyor = false;

                                if (TwoCaptchaMi)
                                {
                                    twoCaptcha.GoodCaptchaReport();
                                }

                                return "OK";
                            }
                        }
                        else if (this.IlkGirisMetni.Any(p => response.Contains(p)))
                        {
                            LogYaz("Web Client sisteme başarıyla giriş yaptı");

                            Connected = true;

                            this.GirisYapiliyor = false;

                            if (TwoCaptchaMi)
                            {
                                twoCaptcha.GoodCaptchaReport();
                            }

                            if (proje == Enums.ProjeTurleri.IsverenSistemi)
                            {
                                this.Cookie += ",panelMenu-menuForm%3ApanelMenuId=menuForm%3AanaMenu1%2CmenuForm%3AanaMenu1subMenu2";
                            }

                            return "OK";
                        }
                        else if (response.Contains(this.HataliKullaniciAdiMetni))
                        {
                            LogYaz("Web Client hatalı kullanıcı adı veya şifre nedeniyle sisteme giriş yapamadı");

                            Connected = false;

                            this.GirisYapiliyor = false;

                            this.GirisYapilamiyor = true;

                            this.GirisYapilamamaNedeni = "Kullanıcı adı veya şifreleriniz hatalıdır";

                            return "Kullanıcı adı veya şifreleriniz hatalıdır";
                        }
                        else if (response.Contains("İşyeri Kanun Kapsamından Çıkmış"))
                        {
                            CookieAwareWebClient wc = (CookieAwareWebClient)this;
                            if (wc.Get("https://uyg.sgk.gov.tr/SigortaliTescil/amp/ToAnaMenu", string.Empty).Contains("İŞE GİRİŞ GÖRÜNTÜLEME"))
                            {
                                LogYaz("Web Client sisteme başarıyla giriş yaptı");

                                Connected = true;

                                this.GirisYapiliyor = false;

                                return "OK";
                            }
                            else
                            {

                                LogYaz("Web Client işyeri kanun kapsamından çıkarıldığı için sisteme giriş yapamadı");

                                Connected = false;

                                this.GirisYapiliyor = false;

                                this.GirisYapilamiyor = true;

                                this.GirisYapilamamaNedeni = "İşyeri Kanun Kapsamından Çıkmıştır";

                                return "İşyeri Kanun Kapsamından Çıkmıştır";
                            }
                        }
                        else if (response.Contains("Is Yeri Iz Olmus") || response.Contains("İşyeri İz Olmuş"))
                        {
                            LogYaz("Web Client işyeri Iz olduğu için sisteme giriş yapamadı");

                            Connected = false;

                            this.GirisYapiliyor = false;

                            this.GirisYapilamiyor = true;

                            this.GirisYapilamamaNedeni = "Is Yeri Iz Olmus";

                            return "Is Yeri Iz Olmus";
                        }
                        else if (response.Contains("Şifreniz doğru fakat ebildirge hesabınız PASIF durumdadır"))
                        {
                            LogYaz("Web Client işyeri hesabı PASİF olduğu için sisteme giriş yapamadı");

                            Connected = false;

                            this.GirisYapiliyor = false;

                            this.GirisYapilamiyor = true;

                            this.GirisYapilamamaNedeni = "işyeri hesabı PASİF olduğu için sisteme giriş yapamadı";

                            return "işyeri hesabı PASİF olduğu için sisteme giriş yapamadı";
                        }
                        else if (response.Contains("İşyeri dosyanızın kanun kapsamından çıkmış olması nedeniyle giriş yapılamaz"))
                        {
                            LogYaz("Web Client işyeri kanun kapsamından çıkarıldığı için sisteme giriş yapamadı");

                            Connected = false;

                            this.GirisYapiliyor = false;

                            this.GirisYapilamiyor = true;

                            this.GirisYapilamamaNedeni = "İşyeri Kanun Kapsamından Çıkmıştır";

                            return "İşyeri Kanun Kapsamından Çıkmıştır";
                        }
                        else if (response.Contains("Vekalet Süresi Dolmuştur"))
                        {
                            LogYaz("Web Client işyeri kanun kapsamından çıkarıldığı için sisteme giriş yapamadı");

                            Connected = false;

                            this.GirisYapiliyor = false;

                            this.GirisYapilamiyor = true;

                            this.GirisYapilamamaNedeni = "Vekalet Süresi Dolmuştur";

                            return "Vekalet Süresi Dolmuştur";
                        }
                        else if (response.Contains("vefat ettiğinden kullanıcı kodu ve şifreleri iptal edilmiştir"))
                        {
                            LogYaz("Web Client kullanıcı vefat ettiği için sisteme giriş yapamadı");

                            Connected = false;

                            this.GirisYapiliyor = false;

                            this.GirisYapilamiyor = true;

                            this.GirisYapilamamaNedeni = "Kullanıcı Vefat Etmiştir";

                            return "Kullanıcı Vefat Etmiştir";
                        }
                        else if (response.Contains("HATA : NAKİL İŞYERİ"))
                        {
                            LogYaz("Web Client nakil işyeri hatasından dolayı sisteme giriş yapamadı");

                            Connected = false;

                            this.GirisYapiliyor = false;

                            this.GirisYapilamiyor = true;

                            this.GirisYapilamamaNedeni = "Nakil İşyeri";

                            return "Nakil İşyeri";
                        }
                        else if (response.Contains("Şifre bilgileri içerisinde Türkçe harf olmamalidir"))
                        {
                            LogYaz("Web Client Şifre bilgileri içerisinde Türkçe harf olmamalidir hatası nedeniyle sisteme giriş yapamadı");

                            Connected = false;

                            this.GirisYapiliyor = false;

                            this.GirisYapilamiyor = true;

                            this.GirisYapilamamaNedeni = "Şifre bilgileri içerisinde Türkçe harf olmamalidir";

                            return "Şifre bilgileri içerisinde Türkçe harf olmamalidir";
                        }
                        else if (response.Contains("IllegalStateException"))
                        {
                            LogYaz("Web Client IllegalStateException hatası nedeniyle sisteme giriş yapamadı.Sistemden logout olunacak.");

                            Connected = false;

                            try
                            {
                                string yanit = base.Get(this.LogOutUri, "");

                                if (!yanit.Equals("Error"))
                                {

                                    LogYaz("Web Client IllegalStateException hatası nedeniyle sistemden başarıyla çıkıldı");
                                }
                                else LogYaz("Web Client IllegalStateException hatası nedeniyle sistemden çıkarken hata meydana geldi");

                            }
                            catch
                            {
                                LogYaz("Web Client IllegalStateException hatası nedeniyle sistemden çıkarken hata meydana geldi");
                            }

                            LogYaz("Web Client IllegalStateException hatası nedeniyle sisteme yeniden giriş yapılacak");

                            goto YenidenDene;
                        }
                        else if (response.Contains(this.HataliGuvenlikKoduMetni))
                        {
                            LogYaz("Web Client hatalı güvenlik kodu nedeniyle sisteme giriş yapamadı");

                            if (TwoCaptchaMi)
                            {
                                twoCaptcha.BadCaptchaReport();
                            }
                            else if (OtomatikCaptchaGirilecek)
                            {
                                try
                                {
                                    using (var dbContext = new DbEntities())
                                    {
                                        var item = dbContext.Ayarlar.Find(this.OtomatikBosCaptchaGirmeAyariEtiketAdi);

                                        if (item != null)
                                        {
                                            item.Deger = "False";

                                            dbContext.SaveChanges();
                                        }
                                    }
                                }
                                catch
                                {
                                }

                                switch (proje)
                                {
                                    case Enums.ProjeTurleri.IsverenSistemi:
                                        Program.OtomatikGuvenlikKoduGirilecekIsverenSistemi = false;
                                        break;
                                    case Enums.ProjeTurleri.EBildirgeV2:
                                        Program.OtomatikGuvenlikKoduGirilecekEBildirgeV2 = false;
                                        break;
                                    case Enums.ProjeTurleri.Bf6645:
                                        Program.OtomatikGuvenlikKoduGirilecek6645 = false;
                                        break;
                                    case Enums.ProjeTurleri.Bf687:
                                        Program.OtomatikGuvenlikKoduGirilecek687 = false;
                                        break;
                                    case Enums.ProjeTurleri.Bf14857:
                                        Program.OtomatikGuvenlikKoduGirilecek14857 = false;
                                        break;
                                    default:
                                        break;
                                }

                            }

                            Connected = false;

                            LogYaz("Web Client hatalı güvenlik kodu nedeniyle yeniden sisteme giriş yapacak");

                            goto YenidenDene;
                        }
                        else if (response.Contains("Sistem Tablosundan veri çekilirken hata oluştu"))
                        {
                            LogYaz("Web Client Sistem Tablosundan veri çekilirken hata oluşması nedeniyle sisteme giriş yapamadı");

                            Connected = false;

                            goto YenidenDene;
                        }
                    }


                }
                catch (WebException)
                {
                }
                catch (Exception ex)
                {
                    LogYaz("Web Client sisteme giriş yaparken bilinmeyen bir hata meydana geldi" + "Hata: " + ex.Message);
                }

                this.GirisYapiliyor = false;

                Connected = false;

                return "Error";
            }
            else
            {
                while (this.GirisYapiliyor) { System.Threading.Thread.Sleep(100); }

                return this.Connected ? "OK" : "Error";
            }
        }

        public void Disconnect(bool BagliOlmasadaCikisYap = false)
        {
            if (Connected || BagliOlmasadaCikisYap)
            {
                LogYaz("Web Client Disconnect metodu ile sistemden çıkış yapacak");

                try
                {
                    string yanit = base.Get(this.LogOutUri, "");

                    if (!yanit.Equals("Error"))
                    {
                        LogYaz("Web Client Disconnect metodu ile sistemden başarıyla çıkış yapıldı");
                    }
                    else LogYaz("Web Client Disconnect metodu ile sistemden başarıyla çıkış yaparken hata meydana geldi");
                }
                catch (Exception ex)
                {
                    LogYaz("Web Client Disconnect metodu ile sistemden başarıyla çıkış yaparken bilinmeyen bir hata meydana geldi. Hata" + ex.Message);
                }

            }

            oturumId = new Guid();

            Connected = false;

        }

        new public string PostData(string PostUrl, string PostData)
        {
            var result = "Error";

            if (_bitti)
            {
                LogYaz("Web Client bitti olarak ayarlandığı için post isteği gönderilmeyecek." + PostUrl + " Param:" + PostData);

                result = "LogOut";
            }

            LogYaz("Web Client post isteği gönderecek." + PostUrl + " Param:" + PostData);

            if (!this.Connected)
            {
                LogYaz("Web Client sisteme bağlı olmadığı için post isteğinden önce sisteme giriş yapacak" + PostUrl + " Param:" + PostData);

                this.Connect();

            }

            if (this.Connected)
            {
                result = base.PostData(PostUrl, PostData);

                if (result.Contains(this.GirisSayfasiMetni))
                {
                    LogYaz("Post isteğinden dönen cevap giriş ekranı olduğu için web client sistemden atılmış. Tekrar giriş yapılıp post isteği tekrarlanacak" + PostUrl + " Param:" + PostData);

                    this.ReConnect();

                    if (this.Connected)
                    {
                        LogYaz("Post isteğinden dönen cevap giriş ekranı olduğu için web client sistemden atılmıştı. Tekrar giriş yapıldı. Post isteği tekrarlanacak" + PostUrl + " Param:" + PostData);

                        result = base.PostData(PostUrl, PostData);
                    }
                    else LogYaz("Post isteğinden dönen cevap giriş ekranı olduğu için web client sistemden atılmıştı. Tekrar giriş yapılamadı. Post isteği yapılamadı" + PostUrl + " Param:" + PostData);
                }
                //else if (result.Contains("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır"))
                //{
                //    LogYaz("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır. Tekrar giriş yapılıp post isteği tekrarlanacak" + PostUrl + " Param:" + PostData);

                //    this.ReConnect();

                //    if (this.Connected)
                //    {
                //        LogYaz("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştı. Tekrar giriş yapıldı. Post isteği tekrarlanacak" + PostUrl + " Param:" + PostData);

                //        result = base.PostData(PostUrl, PostData);

                //        return result;
                //    }
                //    else LogYaz("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştı. Tekrar giriş yapılamadı. Post isteği yapılamadı" + PostUrl + " Param:" + PostData);

                //}

                else if (result.Contains("IllegalStateException"))
                {
                    LogYaz("Post isteğinden dönen cevapta IllegalStateException hatası vardı. Tekrar giriş yapılıp post isteği tekrarlanacak" + PostUrl + " Param:" + PostData);

                    this.ReConnect();

                    if (this.Connected)
                    {
                        LogYaz("Post isteğinden dönen cevapta IllegalStateException hatası vardı. Tekrar giriş yapıldı. Post isteği tekrarlanacak" + PostUrl + " Param:" + PostData);

                        result = base.PostData(PostUrl, PostData);
                    }
                    else LogYaz("Post isteğinden dönen cevap giriş ekranı olduğu için web client sistemden atılmıştı. Tekrar giriş yapılamadı. Post isteği yapılamadı" + PostUrl + " Param:" + PostData);

                }
                else
                {
                    LogYaz("Web Client post isteğini başarıyla gönderdi" + PostUrl + " Param:" + PostData);
                }

            }
            else
            {
                LogYaz("Web Client sisteme bağlı olmadığı için post isteği gerçekleştirilemedi" + PostUrl + " Param:" + PostData);
            }

            if (this.ResponseHeaders != null && this.ResponseHeaders.AllKeys != null && this.ResponseHeaders.AllKeys.Contains("Set-Cookie"))
            {
                var newCookies = this.ResponseHeaders["Set-Cookie"].Split(';').SelectMany(p => p.Split(',')).Where(p => p.Contains("=") && !new List<string> { "path", "domain" }.Contains(p.Split('=')[0].Trim().ToLower())).ToDictionary(x => x.Split('=')[0], x => x.Replace(x.Split('=')[0] + "=", ""));

                var oldCookies = this.Cookie.Split(',').ToDictionary(x => x.Split('=')[0], x => x.Replace(x.Split('=')[0] + "=", ""));

                var cookieKaydet = false;

                foreach (var newcookie in newCookies)
                {
                    if (oldCookies.ContainsKey(newcookie.Key))
                    {
                        if (!oldCookies[newcookie.Key].Equals(newcookie.Value))
                        {
                            cookieKaydet = true;

                            oldCookies[newcookie.Key] = newcookie.Value;
                        }
                    }
                    else
                    {
                        cookieKaydet = true;

                        oldCookies.Add(newcookie.Key, newcookie.Value);
                    }
                }

                if (cookieKaydet)
                {
                    this.Cookie = String.Join(",", oldCookies.Select(p => p.Key + "=" + p.Value));
                }

            }


            return result;

        }

        new public string Get(string Url, string QueryString = "")
        {
            var result = "Error";

            if (_bitti)
            {
                LogYaz("Web Client bitti olarak ayarlandığı için get isteği gönderilmeyecek." + Url + " OueryString:" + QueryString);

                result = "LogOut";
            }

            LogYaz("Web Client get isteği gönderecek." + Url + " QueryString:" + QueryString);

            if (!this.Connected)
            {
                LogYaz("Web Client sisteme bağlı olmadığı için get isteğinden önce sisteme giriş yapacak" + Url + " QueryString:" + QueryString);

                this.Connect();
            }

            if (this.Connected)
            {
                result = base.Get(Url, QueryString);

                if (result.Contains(this.GirisSayfasiMetni))
                {
                    LogYaz("Get isteğinden dönen cevap giriş ekranı olduğu için web client sistemden atılmış. Tekrar giriş yapılıp get isteği tekrarlanacak" + Url + " QueryString:" + QueryString);

                    this.ReConnect();

                    if (this.Connected)
                    {
                        LogYaz("Get isteğinden dönen cevap giriş ekranı olduğu için web client sistemden atılmıştı. Tekrar giriş yapıldı. Get isteği tekrarlanacak" + Url + " QueryString:" + QueryString);

                        result = base.Get(Url, QueryString);

                    }
                    else LogYaz("Get isteğinden dönen cevap giriş ekranı olduğu için web client sistemden atılmıştı. Tekrar giriş yapılamadı. Get isteği yapılamadı" + Url + " QueryString:" + QueryString);
                }
                //else if (result.Contains("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır"))
                //{
                //    LogYaz("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır. Tekrar giriş yapılıp get isteği tekrarlanacak" + Url + " QueryString:" + QueryString);

                //    this.ReConnect();

                //    if (this.Connected)
                //    {
                //        LogYaz("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştı. Tekrar giriş yapıldı. Get isteği tekrarlanacak" + Url + " QueryString:" + QueryString);

                //        result = base.Get(Url, QueryString);

                //        return result;
                //    }
                //    else LogYaz("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştı. Tekrar giriş yapılamadı. Get isteği yapılamadı" + Url + " QueryString:" + QueryString);

                //}

                else if (result.Contains("IllegalStateException"))
                {
                    this.ReConnect();

                    if (this.Connected)
                    {
                        LogYaz("Get isteğinden dönen cevapta IllegalStateException hatası vardı. Tekrar giriş yapıldı. Get isteği tekrarlanacak" + Url + " QueryString:" + QueryString);

                        result = base.Get(Url, QueryString);
                    }
                    else LogYaz("Get isteğinden dönen cevapta IllegalStateException hatası vardı. Tekrar giriş yapılamadı. Get isteği yapılamadı" + Url + " QueryString:" + QueryString);

                }
                else
                {
                    LogYaz("Web Client get isteğini başarıyla gönderdi." + Url + " QueryString:" + QueryString);
                }

            }
            else
            {
                LogYaz("Web Client sisteme bağlı olmadığı için get isteği gerçekleştirilemedi" + Url + " QueryString:" + QueryString);
            }

            if (this.ResponseHeaders != null && this.ResponseHeaders.AllKeys != null && this.ResponseHeaders.AllKeys.Contains("Set-Cookie"))
            {
                var newCookies = this.ResponseHeaders["Set-Cookie"].Split(';').SelectMany(p => p.Split(',')).Where(p => p.Contains("=") && !new List<string> { "path", "domain" }.Contains(p.Split('=')[0].Trim().ToLower())).ToDictionary(x => x.Split('=')[0], x => x.Replace(x.Split('=')[0] + "=", ""));

                var oldCookies = this.Cookie.Split(',').ToDictionary(x => x.Split('=')[0], x => x.Replace(x.Split('=')[0] + "=", ""));

                var cookieKaydet = false;

                foreach (var newcookie in newCookies)
                {
                    if (oldCookies.ContainsKey(newcookie.Key))
                    {
                        if (!oldCookies[newcookie.Key].Equals(newcookie.Value))
                        {
                            cookieKaydet = true;

                            oldCookies[newcookie.Key] = newcookie.Value;
                        }
                    }
                    else
                    {
                        cookieKaydet = true;

                        oldCookies.Add(newcookie.Key, newcookie.Value);
                    }
                }

                if (cookieKaydet)
                {
                    this.Cookie = String.Join(",", oldCookies.Where(p => !string.IsNullOrEmpty(p.Key)).Select(p => p.Key + "=" + p.Value));
                }

            }

            return result;

        }

        public byte[] GetData(string Url, string QueryString)
        {
            byte[] result = null;

            if (_bitti)
            {
                LogYaz("Web Client bitti olarak ayarlandığı için getData isteği gönderilmeyecek." + Url + " OueryString:" + QueryString);

                result = null;
            }

            LogYaz("Web Client getData isteği gönderecek." + Url + " QueryString:" + QueryString);

            if (!this.Connected)
            {
                LogYaz("Web Client sisteme bağlı olmadığı için getData isteğinden önce sisteme giriş yapacak" + Url + " QueryString:" + QueryString);

                this.Connect();
            }

            if (this.Connected)
            {
                result = base.DownloadFileGet(Url, QueryString);

                LogYaz("Web Client getData isteğini başarıyla gönderdi." + Url + " QueryString:" + QueryString);


            }
            else
            {
                LogYaz("Web Client sisteme bağlı olmadığı için getData isteği gerçekleştirilemedi" + Url + " QueryString:" + QueryString);
            }

            if (this.ResponseHeaders != null && this.ResponseHeaders.AllKeys != null && this.ResponseHeaders.AllKeys.Contains("Set-Cookie"))
            {
                var newCookies = this.ResponseHeaders["Set-Cookie"].Split(';').SelectMany(p => p.Split(',')).Where(p => p.Contains("=") && !new List<string> { "path", "domain" }.Contains(p.Split('=')[0].Trim().ToLower())).ToDictionary(x => x.Split('=')[0], x => x.Replace(x.Split('=')[0] + "=", ""));

                var oldCookies = this.Cookie.Split(',').ToDictionary(x => x.Split('=')[0], x => x.Replace(x.Split('=')[0] + "=", ""));

                var cookieKaydet = false;

                foreach (var newcookie in newCookies)
                {
                    if (oldCookies.ContainsKey(newcookie.Key))
                    {
                        if (!oldCookies[newcookie.Key].Equals(newcookie.Value))
                        {
                            cookieKaydet = true;

                            oldCookies[newcookie.Key] = newcookie.Value;
                        }
                    }
                    else
                    {
                        cookieKaydet = true;

                        oldCookies.Add(newcookie.Key, newcookie.Value);
                    }
                }

                if (cookieKaydet)
                {
                    this.Cookie = String.Join(",", oldCookies.Select(p => p.Key + "=" + p.Value));
                }

            }

            return result;

        }

        void LogYaz(string Mesaj)
        {
            if (!this.DosyayaYaziliyor)
            {
                this.DosyayaYaziliyor = true;

                Metodlar.DetayliLogYaz(Mesaj);

                this.DosyayaYaziliyor = false;
            }
            else
            {
                System.Threading.Thread.Sleep(200);

                LogYaz(Mesaj);
            }
        }

        public void ReConnect()
        {
            this.Disconnect();
            this.Connect();
        }

        public ProjeGiris Clone()
        {
            return new ProjeGiris(this.isyeri, this.proje)
            {
                Cookie = this.Cookie,
                Connected = this.Connected,
                oturumId = this.oturumId,
                Ticket = this.Ticket
            };
        }
    }
}
