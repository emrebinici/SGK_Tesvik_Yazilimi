using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TesvikProgrami.Classes
{
    public class BasvuruFormuIndir : Indir
    {
        public BasvuruFormuIndir(long IsyeriId, Enums.BasvuruFormuTurleri basvuruFormuTuru, bool cariTanimla)
        {
            using (var dbContext = new DbEntities())
            {
                SuanYapilanIsyeriBasvuru = dbContext.Isyerleri.Include("Sirketler").Where(p => p.IsyeriID.Equals(IsyeriId)).FirstOrDefault();
            }

            this.IsyeriId = IsyeriId;

            this.CariTanimla = cariTanimla;

            bfsira = basvuruFormuTuru;

            switch (basvuruFormuTuru)
            {
                case Enums.BasvuruFormuTurleri.Bf6111:
                    BasvuruWebClient = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.IsverenSistemi);
                    break;
                case Enums.BasvuruFormuTurleri.Bf6645:
                    BasvuruWebClient = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.Bf6645);
                    break;
                case Enums.BasvuruFormuTurleri.Bf687:
                    BasvuruWebClient = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.Bf687);
                    break;
                case Enums.BasvuruFormuTurleri.Bf7103:
                    BasvuruWebClient = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.IsverenSistemi);
                    break;
                case Enums.BasvuruFormuTurleri.Bf2828:
                    BasvuruWebClient = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.IsverenSistemi);
                    break;
                case Enums.BasvuruFormuTurleri.Bf14857:
                    BasvuruWebClient = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.Bf14857);
                    break;
                case Enums.BasvuruFormuTurleri.BfTumu:
                    BasvuruWebClient = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.IsverenSistemi);
                    break;
                case Enums.BasvuruFormuTurleri.Bf7252:
                    BasvuruWebClient = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.IsverenSistemi);
                    break;
                case Enums.BasvuruFormuTurleri.Bf7256:
                    BasvuruWebClient = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.IsverenSistemi);
                    break;
                case Enums.BasvuruFormuTurleri.Bf7316:
                    BasvuruWebClient = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.IsverenSistemi);
                    break;
                case Enums.BasvuruFormuTurleri.Bf3294:
                    BasvuruWebClient = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.IsverenSistemi);
                    break;
                default:
                    break;
            }

            token = tokenSource.Token;

            task = new Task(() => BasvuruFormuIndirmeBaslat(), token);

        }

        int BasarisizDenemeler = 0;
        bool IskurHatasindanDolayiYenidenGirisYapildi = false;
        KeyValuePair<string, DateTime> SuanYapilanKisi687 = new KeyValuePair<string, DateTime>();
        KeyValuePair<string, DateTime> SuanYapilanKisi6645 = new KeyValuePair<string, DateTime>();
        KeyValuePair<string, DateTime> SuanYapilanKisi14857 = new KeyValuePair<string, DateTime>();
        public Isyerleri SuanYapilanIsyeriBasvuru = null;
        public List<BasvuruLog> loglar = new List<BasvuruLog>();
        bool CariTanimla = true;

        public Enums.BasvuruFormuTurleri bfsira;

        List<string> Basvuru6645EkliOlanlar = new List<string>();
        List<string> Basvuru14857EkliOlanlar = new List<string>();
        List<string> Basvuru687EkliOlanlar = new List<string>();
        List<BasvuruKisiDownload687> basvurukisiler687 = new List<BasvuruKisiDownload687>();
        List<BasvuruKisiDownload6111> basvurukisiler6111 = new List<BasvuruKisiDownload6111>();
        List<BasvuruKisiDownload6645> basvurukisiler6645 = new List<BasvuruKisiDownload6645>();
        List<BasvuruKisiDownload14857> basvurukisiler14857 = new List<BasvuruKisiDownload14857>();
        List<BasvuruKisiDownload7103> basvurukisiler7103 = new List<BasvuruKisiDownload7103>();
        List<BasvuruKisiDownload2828> basvurukisiler2828 = new List<BasvuruKisiDownload2828>();
        List<BasvuruKisiDownload7252> basvurukisiler7252 = new List<BasvuruKisiDownload7252>();
        List<BasvuruKisiDownload17256> basvurukisiler17256 = new List<BasvuruKisiDownload17256>();
        List<BasvuruKisiDownload27256> basvurukisiler27256 = new List<BasvuruKisiDownload27256>();
        List<BasvuruKisiDownload7316> basvurukisiler7316 = new List<BasvuruKisiDownload7316>();
        List<BasvuruKisiDownload3294> basvurukisiler3294 = new List<BasvuruKisiDownload3294>();

        public Dictionary<KeyValuePair<string, DateTime>, bool> IslemYapilacaklar6645 = new Dictionary<KeyValuePair<string, DateTime>, bool>();
        public Dictionary<KeyValuePair<string, DateTime>, bool> IslemYapilacaklar687 = new Dictionary<KeyValuePair<string, DateTime>, bool>();
        public Dictionary<KeyValuePair<string, DateTime>, bool> IslemYapilacaklar14857 = new Dictionary<KeyValuePair<string, DateTime>, bool>();

        public List<KeyValuePair<string, DateTime>> YeniIslemYapilanlar6645 = new List<KeyValuePair<string, DateTime>>();
        public List<KeyValuePair<string, DateTime>> YeniIslemYapilanlar14857 = new List<KeyValuePair<string, DateTime>>();
        public List<KeyValuePair<string, DateTime>> YeniIslemYapilanlar687 = new List<KeyValuePair<string, DateTime>>();

        public DateTime dtBaslangic687 = DateTime.MinValue;
        public DateTime dtBitis687 = DateTime.MinValue;

        public DateTime dtBaslangic6111 = DateTime.MinValue;
        public DateTime dtBitis6111 = DateTime.MinValue;

        public DateTime dtBaslangic6645 = DateTime.MinValue;
        public DateTime dtBitis6645 = DateTime.MinValue;

        public DateTime dtBaslangic14857 = DateTime.MinValue;
        public DateTime dtBitis14857 = DateTime.MinValue;

        public DateTime dtBaslangic2828 = DateTime.MinValue;
        public DateTime dtBitis2828 = DateTime.MinValue;

        public DateTime dtBaslangic7103 = DateTime.MinValue;
        public DateTime dtBitis7103 = DateTime.MinValue;

        public DateTime dtBaslangic7252 = DateTime.MinValue;
        public DateTime dtBitis7252 = DateTime.MinValue;

        public DateTime dtBaslangic7256 = DateTime.MinValue;
        public DateTime dtBitis7256 = DateTime.MinValue;

        public DateTime dtBaslangic7316 = DateTime.MinValue;
        public DateTime dtBitis7316 = DateTime.MinValue;

        public DateTime dtBaslangic3294 = DateTime.MinValue;
        public DateTime dtBitis3294 = DateTime.MinValue;

        Stopwatch stopwatch = new Stopwatch();
        public string siradakiIslem { get; set; } = "Giriş Yapılacak";
        public List<string> incelenecekDonemler = new List<string>();
        public List<string> incelenecekDonemler7103 = new List<string>();
        public List<string> incelenecekDonemler2828 = new List<string>();
        public List<string> incelenecekDonemler7252 = new List<string>();
        public List<string> incelenecekDonemler7256 = new List<string>();
        public List<string> incelenecekDonemler7316 = new List<string>();
        public List<string> incelenecekDonemler3294 = new List<string>();
        bool UcretIstiyorDiyalogMesajiAcik = false;
        bool UcretDestegiIstiyorSoruldu = false;
        bool UcretDestegiIstiyor = false;
        bool SistemHatasiVarDiyalogMesajiAcik = false;
        bool SistemHatasiVarDiyalogMesajiSoruldu = false;
        bool IcUygulamayaErisilemediDiyalogMesajiAcik = false;
        bool IcUygulamayaErisilemediDiyalogMesajiSoruldu = false;
        bool IsyeriBilgileriniKontrolEdinizUyarisiDiyalogMesajiAcik = false;
        bool IsyeriBilgileriniKontrolEdinizUyarisiDiyalogMesajiSoruldu = false;
        bool UnrecognizedFieldDiyalogMesajiAcik = false;
        bool UnrecognizedFieldDiyalogMesajiSoruldu = false;
        bool WebServisUyarisiDiyalogMesajiAcik = false;
        bool WebServisUyarisiDiyalogMesajiSoruldu = false;
        bool TooManyOpenFilesUyarisiDiyalogMesajiAcik = false;
        bool TooManyOpenFilesUyarisiDiyalogMesajiSoruldu = false;
        bool AsgariUcretDestekTutarlariCekildi = false;
        public List<BasvuruListesi7166Kisi> basvuruListesi7166Kisiler = new List<BasvuruListesi7166Kisi>();
        bool BasvuruListesi7166KontrolEdildi = false;
        bool AylikCalisanSayilariCekildi = false;
        public Dictionary<BasvuruKisiDownload7103, string> SisteminSildirmedigi7103Kayitlari = new Dictionary<BasvuruKisiDownload7103, string>();
        public List<BasvuruListesi7166Kisi> UcretDestegiIstenecekKisiler = new List<BasvuruListesi7166Kisi>();
        public List<BasvuruListesi7166Kisi> UcretDestegiIstenmeyecekKisiler = new List<BasvuruListesi7166Kisi>();
        public List<BasvuruListesi7166Kisi> BasvuruListesi7166yaEklenecekKisiler = new List<BasvuruListesi7166Kisi>();
        public List<IstenCikisKaydi> YasakliCikisiOlanKisiler = new List<IstenCikisKaydi>();
        public List<IstenCikisKaydi> YasakliCikisinaBakilanKisiler = new List<IstenCikisKaydi>();
        ProjeGiris sigortaliIstenAyrilisProjesiConnect = null;
        public ProjeGiris BasvuruWebClient = null;
        bool BasvuruDonemleriCekildi = false;

        HashSet<KeyValuePair<string, DateTime>> islemYapilanKisiler = new HashSet<KeyValuePair<string, DateTime>>();
        HashSet<KeyValuePair<string, DateTime>> islemiTamamlananKisiler = new HashSet<KeyValuePair<string, DateTime>>();
        HashSet<string> islemYapilanDonemler = new HashSet<string>();
        HashSet<string> islemiTamamlananDonemler = new HashSet<string>();

        HashSet<KeyValuePair<string, DateTime>> TanimlamaYapilanlar6111 = new HashSet<KeyValuePair<string, DateTime>>();
        HashSet<KeyValuePair<string, DateTime>> TanimlamaYapilanlar7103 = new HashSet<KeyValuePair<string, DateTime>>();
        HashSet<KeyValuePair<string, DateTime>> TanimlamaYapilanlar2828 = new HashSet<KeyValuePair<string, DateTime>>();
        HashSet<KeyValuePair<string, DateTime>> TanimlamaYapilanlar7252 = new HashSet<KeyValuePair<string, DateTime>>();
        HashSet<KeyValuePair<string, DateTime>> TanimlamaYapilanlar17256 = new HashSet<KeyValuePair<string, DateTime>>();
        HashSet<KeyValuePair<string, DateTime>> TanimlamaYapilanlar27256 = new HashSet<KeyValuePair<string, DateTime>>();
        HashSet<KeyValuePair<string, DateTime>> TanimlamaYapilanlar7316 = new HashSet<KeyValuePair<string, DateTime>>();
        HashSet<KeyValuePair<string, DateTime>> TanimlamaYapilanlar3294 = new HashSet<KeyValuePair<string, DateTime>>();

        int ListedeBulunmayanKisileriTekrarDenemeSayisi = 0;
        int sonAyTekrarSayisi = 0;
        DateTime oncekiAy = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);

        public bool KaydedilenFormVar = false;

        public TumKisilerSonuc TumKisilerSonuc = null;
        public Dictionary<DateTime, Dictionary<string, long>> AyCalisanSayilari = new Dictionary<DateTime, Dictionary<string, long>>();

        public Dictionary<DateTime, Dictionary<string, long>> AyCalisanSayilariBazHesaplama = new Dictionary<DateTime, Dictionary<string, long>>();


        public bool EnBastanTumu;
        public bool EnBastan6111;
        public bool EnBastan7103;
        public bool EnBastan2828;
        public bool EnBastan7252;
        public bool EnBastan7256;
        public bool EnBastan7316;
        public bool EnBastan3294;

        int maksimumDenemeSayisi_6645_687 = 20;


        void BasvuruLogEkle(string mesaj, int donem = -1, int kisiNo = -1, bool logEkranindaGoster = true)
        {
            lock (loglar)
            {
                loglar.Add(new BasvuruLog { Mesaj = mesaj, Donem = donem, KisiNo = kisiNo, Tarih = DateTime.Now, LogEkranindaGoster = logEkranindaGoster });

                if (logEkranindaGoster)
                {
                    new delLoglariGuncelle(BasvuruLoglariGuncelle).Invoke();
                }
            }
        }


        //public void DegiskenTemizle()
        //{
        //    stopwatch = new Stopwatch();
        //    siradakiIslem = "Giriş Yapılacak";
        //    IcUygulamayaErisilemediDiyalogMesajiAcik = false;
        //    IcUygulamayaErisilemediDiyalogMesajiSoruldu = false;
        //    UcretIstiyorDiyalogMesajiAcik = false;
        //    UcretDestegiIstiyorSoruldu = false;
        //    SistemHatasiVarDiyalogMesajiAcik = false;
        //    SistemHatasiVarDiyalogMesajiSoruldu = false;
        //    IsyeriBilgileriniKontrolEdinizUyarisiDiyalogMesajiAcik = false;
        //    IsyeriBilgileriniKontrolEdinizUyarisiDiyalogMesajiSoruldu = false;
        //    WebServisUyarisiDiyalogMesajiAcik = false;
        //    WebServisUyarisiDiyalogMesajiSoruldu = false;
        //    UnrecognizedFieldDiyalogMesajiAcik = false;
        //    UnrecognizedFieldDiyalogMesajiSoruldu = false;
        //    UcretDestegiIstiyor = false;
        //    AsgariUcretDestekTutarlariCekildi = false;
        //    basvuruListesi7166Kisiler = new List<BasvuruListesi7166Kisi>();
        //    BasvuruListesi7166KontrolEdildi = false;
        //    SisteminSildirmedigi7103Kayitlari = new Dictionary<BasvuruKisiDownload7103, string>();
        //    UcretDestegiIstenecekKisiler = new List<BasvuruListesi7166Kisi>();
        //    UcretDestegiIstenmeyecekKisiler = new List<BasvuruListesi7166Kisi>();
        //    BasvuruListesi7166yaEklenecekKisiler = new List<BasvuruListesi7166Kisi>();
        //    YasakliCikisiOlanKisiler = new List<IstenCikisKaydi>();
        //    YasakliCikisinaBakilanKisiler = new List<IstenCikisKaydi>();
        //    sigortaliIstenAyrilisProjesiConnect = null;
        //    Program.TumKisiler = null;
        //    Program.AySatirlari = null;
        //    Program.TumAylar = null;
        //    Program.KisilerinSatirlari = null;
        //    Program.KisilerinSatirlariIptallerDahil = null;
        //    Program.AyCalisanSayilari = new Dictionary<DateTime, Dictionary<string, long>>();
        //    Program.AyCalisanSayilariBazHesaplama = new Dictionary<DateTime, Dictionary<string, long>>();
        //    BasvuruDonemleriCekildi = false;
        //    BasvuruTicket = null;
        //    Captchas = new Dictionary<int, string>();
        //    CaptchaGosteriliyor = new Dictionary<int, bool>();
        //    BasarisizDenemeler = new Dictionary<int, int>();
        //    loglar = new List<BasvuruLog>();
        //    islemiTamamlananDonemler = new HashSet<string>();
        //    islemYapilanDonemler = new HashSet<string>();
        //    islemYapilanKisiler = new HashSet<KeyValuePair<string, DateTime>>();
        //    islemiTamamlananKisiler = new HashSet<KeyValuePair<string, DateTime>>();
        //    AylikCalisanSayilariCekildi = false;
        //    basvurukisiler14857 = new List<BasvuruKisiDownload14857>();
        //    basvurukisiler6111 = new List<BasvuruKisiDownload6111>();
        //    basvurukisiler6645 = new List<BasvuruKisiDownload6645>();
        //    basvurukisiler687 = new List<BasvuruKisiDownload687>();
        //    basvurukisiler7103 = new List<BasvuruKisiDownload7103>();
        //    basvurukisiler2828 = new List<BasvuruKisiDownload2828>();
        //    Basvuru687EkliOlanlar = new List<string>();
        //    Basvuru6645EkliOlanlar = new List<string>();
        //    Basvuru14857EkliOlanlar = new List<string>();
        //    SuanYapilanKisi14857 = new Dictionary<int, KeyValuePair<string, DateTime>>();
        //    SuanYapilanKisi6645 = new Dictionary<int, KeyValuePair<string, DateTime>>();
        //    SuanYapilanKisi687 = new Dictionary<int, KeyValuePair<string, DateTime>>();
        //    TanimlamaYapilanlar2828 = new HashSet<KeyValuePair<string, DateTime>>();
        //    TanimlamaYapilanlar6111 = new HashSet<KeyValuePair<string, DateTime>>();
        //    TanimlamaYapilanlar7103 = new HashSet<KeyValuePair<string, DateTime>>();
        //    ListedeBulunmayanKisileriTekrarDenemeSayisi = 0;
        //    sonAyTekrarSayisi = 0;
        //    oncekiAy = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
        //}

        void IptalKontrolu()
        {
            token.ThrowIfCancellationRequested();
        }

        void BasvuruFormuIndirmeBaslat()
        {
            try
            {
                IptalKontrolu();

                //try
                //{
                //    if (File.Exists("DetayliLogKayitlari.txt")) File.Delete("DetayliLogKayitlari.txt");
                //}
                //catch { }

                sb = new StringBuilder();

                BasvuruLogEkle("\"" + SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBasvuru.SubeAdi + "\" işyeri için " + bfsira.BasvuruFormuAdiGetir() + " başvuru formu indirilmeye başlanıyor");

                Metodlar.DetayliLogYaz(SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBasvuru.SubeAdi + "\" işyeri için " + bfsira.BasvuruFormuAdiGetir() + " başvuru formu indirilmeye başlanıyor");

                string aphbyol = Metodlar.FormBul(SuanYapilanIsyeriBasvuru, Enums.FormTuru.Aphb);

                bool Devam = true;

                bool AphbBilgileriOkunacak = false;


                if (bfsira == Enums.BasvuruFormuTurleri.Bf687)
                {
                    if (dtMevcutAphb == null)
                    {
                        if (aphbyol != null)
                        {
                            dtMevcutAphb = Metodlar.AylikListeyiYukle(aphbyol);
                        }
                    }

                    if (dtMevcutAphb != null)
                    {
                        foreach (DataRow row in dtMevcutAphb.Rows)
                        {
                            string belgeturu = row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString().Trim();

                            if (!TesvikHesaplamaSabitleri.DestekKapsaminaGirmeyenBelgeTurleri.Contains(belgeturu))
                            {
                                string tckimlikno = row[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString();

                                string Yil = row[(int)Enums.AphbHucreBilgileri.Yil].ToString();

                                string Ay = row[(int)Enums.AphbHucreBilgileri.Ay].ToString();

                                DateTime dttarih = new DateTime(Convert.ToInt32(Yil), Convert.ToInt32(Ay), 1);

                                string anaisveren = row[(int)Enums.AphbHucreBilgileri.Araci].ToString();

                                bool devam = false;

                                if (SuanYapilanIsyeriBasvuru.TaseronNo == "000" || SuanYapilanIsyeriBasvuru.TaseronNo == "0000" || string.IsNullOrEmpty(SuanYapilanIsyeriBasvuru.TaseronNo))
                                {
                                    devam = anaisveren == "Ana İşveren" || anaisveren.ToLower().Contains("ana şirket");
                                }
                                else
                                {
                                    devam = anaisveren.StartsWith(SuanYapilanIsyeriBasvuru.TaseronNo + "-");
                                }

                                if (devam)
                                {

                                    if (dttarih >= dtBaslangic687 && dttarih <= dtBitis687)
                                    {

                                        DateTime AyIcindeIseGirisTarihi = DateTime.MinValue;

                                        string gtarih = row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString().Trim();

                                        if (!String.IsNullOrEmpty(gtarih))
                                        {
                                            try
                                            {
                                                AyIcindeIseGirisTarihi = Convert.ToDateTime(gtarih + "/" + Yil);
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    AyIcindeIseGirisTarihi = DateTime.FromOADate(Convert.ToDouble(gtarih));

                                                    AyIcindeIseGirisTarihi = new DateTime(Convert.ToInt32(Yil), AyIcindeIseGirisTarihi.Month, AyIcindeIseGirisTarihi.Day);
                                                }
                                                catch
                                                {
                                                }
                                            }
                                        }

                                        if (AyIcindeIseGirisTarihi != DateTime.MinValue)
                                        {
                                            if (AyIcindeIseGirisTarihi >= BasvuruFormuIndirmeSabitleri.IseGirisBaslangicTarihi687)
                                            {
                                                KeyValuePair<string, DateTime> kv = new KeyValuePair<string, DateTime>(tckimlikno, AyIcindeIseGirisTarihi);

                                                if (!IslemYapilacaklar687.ContainsKey(kv))
                                                {
                                                    IslemYapilacaklar687.Add(kv, false);
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                        }
                    }

                    if (IslemYapilacaklar687.Count == 0)
                    {
                        BasvuruLogEkle("İncelenecek kişi bulunamadığı için 687 başvuru formu indirme işlemi sonlandırılıyor");

                        Devam = false;

                        BasvuruSonaErdi(false, false, "İncelenecek kişi olmadığından 687 başvuru formu indirme işlemi bitirildi");
                    }
                    else
                    {
                        BasvuruLogEkle("Kriterleri sağlayan " + IslemYapilacaklar687.Count + " kayıt seçildi");

                        string basvuruyol = Metodlar.FormBul(SuanYapilanIsyeriBasvuru, Enums.FormTuru.BasvuruFormu);

                        if (basvuruyol != null)
                        {
                            DataTable dtbasvuru687 = null;

                            DataSet dsbasvuru = Metodlar.BasvuruListesiniYukle(basvuruyol, false);

                            if (dsbasvuru != null && dsbasvuru.Tables.Count > 2) dtbasvuru687 = dsbasvuru.Tables[2];

                            if (dtbasvuru687 != null)
                            {

                                foreach (DataRow row in dtbasvuru687.Rows)
                                {
                                    string TcKimlikNo = row[Sabitler.BasvuruFormlariSutunlari["687"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString();

                                    if (!Basvuru687EkliOlanlar.Contains(TcKimlikNo)) Basvuru687EkliOlanlar.Add(TcKimlikNo);
                                }
                            }
                        }
                    }
                }
                else if (bfsira == Enums.BasvuruFormuTurleri.Bf6645)
                {
                    if (dtMevcutAphb == null)
                    {
                        if (aphbyol != null)
                        {
                            dtMevcutAphb = Metodlar.AylikListeyiYukle(aphbyol);
                        }
                    }

                    if (dtMevcutAphb != null)
                    {
                        foreach (DataRow row in dtMevcutAphb.Rows)
                        {
                            string belgeturu = row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString().Trim();

                            if (!TesvikHesaplamaSabitleri.DestekKapsaminaGirmeyenBelgeTurleri.Contains(belgeturu))
                            {
                                string tckimlikno = row[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString();

                                string Yil = row[(int)Enums.AphbHucreBilgileri.Yil].ToString();

                                string Ay = row[(int)Enums.AphbHucreBilgileri.Ay].ToString();

                                DateTime dttarih = new DateTime(Convert.ToInt32(Yil), Convert.ToInt32(Ay), 1);

                                string anaisveren = row[(int)Enums.AphbHucreBilgileri.Araci].ToString();

                                bool devam = false;

                                if (SuanYapilanIsyeriBasvuru.TaseronNo == "000" || SuanYapilanIsyeriBasvuru.TaseronNo == "0000" || string.IsNullOrEmpty(SuanYapilanIsyeriBasvuru.TaseronNo))
                                {
                                    devam = anaisveren == "Ana İşveren" || anaisveren.ToLower().Contains("ana şirket");
                                }
                                else
                                {
                                    devam = anaisveren.StartsWith(SuanYapilanIsyeriBasvuru.TaseronNo + "-");
                                }

                                if (devam)
                                {
                                    if (dttarih >= dtBaslangic6645 && dttarih <= dtBitis6645)
                                    {

                                        DateTime AyIcindeIseGirisTarihi = DateTime.MinValue;

                                        string gtarih = row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString().Trim();

                                        if (!String.IsNullOrEmpty(gtarih))
                                        {
                                            try
                                            {
                                                AyIcindeIseGirisTarihi = Convert.ToDateTime(gtarih + "/" + Yil);
                                            }
                                            catch
                                            {
                                                try
                                                {

                                                    AyIcindeIseGirisTarihi = DateTime.FromOADate(Convert.ToDouble(gtarih));

                                                    AyIcindeIseGirisTarihi = new DateTime(Convert.ToInt32(Yil), AyIcindeIseGirisTarihi.Month, AyIcindeIseGirisTarihi.Day);
                                                }
                                                catch
                                                {
                                                }
                                            }
                                        }

                                        if (AyIcindeIseGirisTarihi != DateTime.MinValue)
                                        {
                                            if (AyIcindeIseGirisTarihi >= BasvuruFormuIndirmeSabitleri.IseGirisBaslangicTarihi6645 && AyIcindeIseGirisTarihi <= BasvuruFormuIndirmeSabitleri.IseGirisBitisTarihi6645)
                                            {
                                                KeyValuePair<string, DateTime> kv = new KeyValuePair<string, DateTime>(tckimlikno, AyIcindeIseGirisTarihi);

                                                if (!IslemYapilacaklar6645.ContainsKey(kv))
                                                {
                                                    IslemYapilacaklar6645.Add(kv, false);
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                        }
                    }

                    if (IslemYapilacaklar6645.Count == 0)
                    {
                        BasvuruLogEkle("İncelenecek kişi bulunamadığı için 6645 başvuru formu indirme işlemi sonlandırılıyor.");

                        Devam = false;

                        BasvuruSonaErdi(false, false, "İncelenecek kişi olmadığından 6645 başvuru formu indirme işlemi bitirildi");
                    }
                    else
                    {
                        BasvuruLogEkle("Kriterleri sağlayan " + IslemYapilacaklar6645.Count + " kayıt seçildi");


                        string basvuruyol = Metodlar.FormBul(SuanYapilanIsyeriBasvuru, Enums.FormTuru.BasvuruFormu);

                        if (basvuruyol != null)
                        {
                            DataTable dtbasvuru6645 = null;

                            DataSet dsbasvuru = Metodlar.BasvuruListesiniYukle(basvuruyol, false);

                            if (dsbasvuru != null && dsbasvuru.Tables.Count > 1) dtbasvuru6645 = dsbasvuru.Tables[1];

                            if (dtbasvuru6645 != null)
                            {

                                foreach (DataRow row in dtbasvuru6645.Rows)
                                {
                                    string TcKimlikNo = row[Sabitler.BasvuruFormlariSutunlari["6645"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString();

                                    if (!Basvuru6645EkliOlanlar.Contains(TcKimlikNo)) Basvuru6645EkliOlanlar.Add(TcKimlikNo);
                                }
                            }
                        }
                    }
                }
                else if (bfsira == Enums.BasvuruFormuTurleri.Bf14857)
                {
                    if (dtMevcutAphb == null)
                    {
                        if (aphbyol != null)
                        {
                            dtMevcutAphb = Metodlar.AylikListeyiYukle(aphbyol);
                        }
                    }

                    if (dtMevcutAphb != null)
                    {

                        var enBuyukAy = dtMevcutAphb.AsEnumerable().Max(row => new DateTime(row[(int)Enums.AphbHucreBilgileri.Yil].ToString().ToInt(), row[(int)Enums.AphbHucreBilgileri.Ay].ToString().ToInt(), 1));

                        foreach (DataRow row in dtMevcutAphb.Rows)
                        {
                            string belgeturu = row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString().Trim();

                            if (!TesvikHesaplamaSabitleri.DestekKapsaminaGirmeyenBelgeTurleri.Contains(belgeturu))
                            {
                                string tckimlikno = row[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString();

                                string Yil = row[(int)Enums.AphbHucreBilgileri.Yil].ToString();

                                string Ay = row[(int)Enums.AphbHucreBilgileri.Ay].ToString();

                                DateTime dttarih = new DateTime(Convert.ToInt32(Yil), Convert.ToInt32(Ay), 1);

                                string anaisveren = row[(int)Enums.AphbHucreBilgileri.Araci].ToString();

                                bool devam = false;

                                if (SuanYapilanIsyeriBasvuru.TaseronNo == "000" || SuanYapilanIsyeriBasvuru.TaseronNo == "0000" || string.IsNullOrEmpty(SuanYapilanIsyeriBasvuru.TaseronNo))
                                {
                                    devam = anaisveren == "Ana İşveren" || anaisveren.ToLower().Contains("ana şirket");
                                }
                                else
                                {
                                    devam = anaisveren.StartsWith(SuanYapilanIsyeriBasvuru.TaseronNo + "-");
                                }

                                if (devam)
                                {

                                    if ((dttarih >= dtBaslangic14857 && dttarih <= dtBitis14857) || (CariTanimla && dttarih == enBuyukAy))
                                    {

                                        DateTime AyIcindeIseGirisTarihi = DateTime.MinValue;

                                        string gtarih = row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString().Trim();

                                        if (!String.IsNullOrEmpty(gtarih))
                                        {
                                            try
                                            {
                                                AyIcindeIseGirisTarihi = Convert.ToDateTime(gtarih + "/" + Yil);
                                            }
                                            catch
                                            {
                                                try
                                                {

                                                    AyIcindeIseGirisTarihi = DateTime.FromOADate(Convert.ToDouble(gtarih));

                                                    AyIcindeIseGirisTarihi = new DateTime(Convert.ToInt32(Yil), AyIcindeIseGirisTarihi.Month, AyIcindeIseGirisTarihi.Day);
                                                }
                                                catch
                                                {
                                                }
                                            }
                                        }

                                        bool ekle = false;

                                        if (dttarih == BasvuruFormuIndirmeSabitleri.IseGirisBaslangicTarihi14857) ekle = true;

                                        if (AyIcindeIseGirisTarihi != DateTime.MinValue)
                                        {
                                            if (AyIcindeIseGirisTarihi >= BasvuruFormuIndirmeSabitleri.IseGirisBaslangicTarihi14857)
                                            {
                                                ekle = true;
                                            }
                                        }

                                        if (CariTanimla && dttarih == enBuyukAy)
                                        {
                                            ekle = true;
                                        }

                                        if (ekle)
                                        {
                                            KeyValuePair<string, DateTime> kv = new KeyValuePair<string, DateTime>(tckimlikno, AyIcindeIseGirisTarihi);

                                            if (!IslemYapilacaklar14857.ContainsKey(kv))
                                            {
                                                IslemYapilacaklar14857.Add(kv, false);
                                            }
                                        }
                                    }
                                }
                            }


                        }
                    }

                    if (IslemYapilacaklar14857.Count == 0)
                    {
                        if ( /*! CariTanimla || */
                            !indirilenIsyeri.BasvuruFormuIndirmeleri.Any(p =>
                                p.bfsira == Enums.BasvuruFormuTurleri.Bf6111
                                   ||
                                   p.bfsira == Enums.BasvuruFormuTurleri.Bf7103
                                   ||
                                   p.bfsira == Enums.BasvuruFormuTurleri.Bf2828
                                   ||
                                   p.bfsira == Enums.BasvuruFormuTurleri.Bf7252
                                   ||
                                   p.bfsira == Enums.BasvuruFormuTurleri.Bf7256
                                   ||
                                   p.bfsira == Enums.BasvuruFormuTurleri.Bf7316
                                   ||
                                   p.bfsira == Enums.BasvuruFormuTurleri.Bf3294
                                   ||
                                   p.bfsira == Enums.BasvuruFormuTurleri.BfTumu
                             )
                        )
                        {
                            BasvuruLogEkle("İncelenecek kişi bulunamadığı için 14857 başvuru formu indirme işlemi sonlandırılıyor.");

                            Devam = false;

                            BasvuruSonaErdi(false, false, "İncelenecek kişi olmadığından 14857 başvuru formu indirme işlemi bitirildi");
                        }
                        else
                        {
                            BasvuruLogEkle("APHB'de incelenecek kişi bulunamadı");
                        }


                    }
                    else
                    {
                        BasvuruLogEkle("Kriterleri sağlayan " + IslemYapilacaklar14857.Count + " kayıt seçildi");

                        string basvuruyol = Metodlar.FormBul(SuanYapilanIsyeriBasvuru, Enums.FormTuru.BasvuruFormu);

                        if (basvuruyol != null)
                        {
                            DataTable dtbasvuru14857 = null;

                            DataSet dsbasvuru = Metodlar.BasvuruListesiniYukle(basvuruyol, false);

                            if (dsbasvuru != null && dsbasvuru.Tables.Count > 5) dtbasvuru14857 = dsbasvuru.Tables[5];

                            if (dtbasvuru14857 != null)
                            {

                                foreach (DataRow row in dtbasvuru14857.Rows)
                                {
                                    string TcKimlikNo = row[Sabitler.BasvuruFormlariSutunlari["14857"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString();

                                    if (!Basvuru14857EkliOlanlar.Contains(TcKimlikNo)) Basvuru14857EkliOlanlar.Add(TcKimlikNo);
                                }
                            }
                        }
                    }
                }
                else if (bfsira == Enums.BasvuruFormuTurleri.Bf7103 || bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                {
                    var basvuruListesi7166Yol = Metodlar.FormBul(SuanYapilanIsyeriBasvuru, Enums.FormTuru.BasvuruListesi7166);

                    if (basvuruListesi7166Yol != null)
                    {
                        DataTable dtBasvuruListesi7166 = Metodlar.BasvuruListesi7166Yukle(basvuruListesi7166Yol);

                        bool EskiSablon = dtBasvuruListesi7166.Columns.Contains("Silinebiliyor Mu?");


                        if (dtBasvuruListesi7166.Rows.Count > 0)
                        {
                            basvuruListesi7166Kisiler = dtBasvuruListesi7166.AsEnumerable().Select(row => new BasvuruListesi7166Kisi
                            {
                                TckimlikNo = row[(int)Enums.BasvuruListesi7166SutunTurleri.TcKimlikNoSosyalGuvenlikNo].ToString(),
                                Ad = row[(int)Enums.BasvuruListesi7166SutunTurleri.Ad].ToString(),
                                Soyad = row[(int)Enums.BasvuruListesi7166SutunTurleri.Soyad].ToString(),
                                Giris = Convert.ToDateTime(row[(int)Enums.BasvuruListesi7166SutunTurleri.Giris].ToString()),
                                Cikis = row[(int)Enums.BasvuruListesi7166SutunTurleri.Cikis].ToString(),
                                IstenCikisNedeni = row[(int)Enums.BasvuruListesi7166SutunTurleri.IstenCikisNedeni].ToString(),
                                VerilmisMi7103 = EskiSablon ? false : row[(int)Enums.BasvuruListesi7166SutunTurleri.VerilmisMi7103].ToString().Equals("Evet"),
                                UygunlukDurumu = row[(int)Enums.BasvuruListesi7166SutunTurleri.UygunlukDurumu].ToString(),
                                UygunlukDurumuNedeni = row[(int)Enums.BasvuruListesi7166SutunTurleri.UygunlukDurumuNedeni].ToString(),
                            }).ToList();
                        }
                    }

                    AphbBilgileriOkunacak = true;

                    sigortaliIstenAyrilisProjesiConnect = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.SigortaliIstenAyrilis);

                }
                else if (bfsira == Enums.BasvuruFormuTurleri.Bf6111 || bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                {
                    AphbBilgileriOkunacak = true;

                    sigortaliIstenAyrilisProjesiConnect = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.SigortaliIstenAyrilis);
                }

                if (AphbBilgileriOkunacak)
                {
                    if (dtMevcutAphb == null)
                    {
                        if (aphbyol != null)
                        {
                            BasvuruLogEkle("Aphb dosyasındaki bilgiler yükleniyor");

                            dtMevcutAphb = Metodlar.AylikListeyiYukle(aphbyol);

                            BasvuruLogEkle("Aphb dosyasındaki bilgiler yüklendi");
                        }
                        else TumKisilerSonuc = new TumKisilerSonuc();
                    }

                    if (dtMevcutAphb != null)
                    {
                        var TesvikVerilenler = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new List<string>());

                        List<KeyValuePair<string, string>> yilveaylar = new List<KeyValuePair<string, string>>();

                        Dictionary<KeyValuePair<string, string>, List<KeyValuePair<int, int>>> tumyilveaylar = new Dictionary<KeyValuePair<string, string>, List<KeyValuePair<int, int>>>();

                        DateTime enbuyukay = DateTime.MinValue;

                        TumKisilerSonuc = Metodlar.TumKisileriGetir(dtMevcutAphb);
                    }
                }


                if (Devam)
                {
                    BasvuruSayfayiYukle();
                }
            }
            catch (OperationCanceledException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Metodlar.HataMesajiGoster(ex, "Başvuru formu indirme işlemi esnasında beklenmedik bir hata oluştu");

                throw ex;
            }
        }
        private void BasvuruSayfayiYukle()
        {
            IptalKontrolu();

            BasarisizDenemeler = 0;

            string girisCevabi = string.Empty;
            string Mesaj = "";

            BasvuruWebClient.Disconnect();

            Metodlar.DetayliLogYaz("BasvuruSayfaYükleMetodu: Sisteme giriş yapılacak");

            for (int i = 0; i < 5; i++)
            {
                BasvuruWebClient.Connect();

                if (BasvuruWebClient.Connected || BasvuruWebClient.GirisYapilamiyor) break;
            }


            if (!BasvuruWebClient.Connected)
            {
                BasvuruSonaErdi(false, true, string.IsNullOrEmpty(BasvuruWebClient.GirisYapilamamaNedeni) ? "5 denemeye rağmen sisteme giriş yapamadı" : BasvuruWebClient.GirisYapilamamaNedeni);

                if (BasvuruWebClient.GirisYapilamamaNedeni.Equals("Güvenlik kodu girilmedi"))
                {
                    IndirmeSonucu.IptalEdildi = true;
                }
                else IndirmeSonucu.HataVar = true;

                return;
            }

            Metodlar.DetayliLogYaz("BasvuruSayfaYükleMetodu: Sisteme giriş yapıldı");

            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();

            if (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf6111 || bfsira == Enums.BasvuruFormuTurleri.Bf7103 || bfsira == Enums.BasvuruFormuTurleri.Bf2828 || bfsira == Enums.BasvuruFormuTurleri.Bf7252 || bfsira == Enums.BasvuruFormuTurleri.Bf7256 || bfsira == Enums.BasvuruFormuTurleri.Bf7316 || bfsira == Enums.BasvuruFormuTurleri.Bf3294)
            {
                var TarihBaslangic = (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf6111) ? dtBaslangic6111 : bfsira == Enums.BasvuruFormuTurleri.Bf7103 ? dtBaslangic7103 : bfsira == Enums.BasvuruFormuTurleri.Bf2828 ? dtBaslangic2828 : bfsira == Enums.BasvuruFormuTurleri.Bf7252 ? dtBaslangic7252 : bfsira == Enums.BasvuruFormuTurleri.Bf7256 ? dtBaslangic7256 : bfsira == Enums.BasvuruFormuTurleri.Bf7316 ? dtBaslangic7316 : dtBaslangic3294;

                //if (TarihBaslangic < new DateTime(2012, 1, 1))
                if (!CariTanimla)
                {

                    #region Asgari Ücret Destek Tutarlarını Çekme

                    if (!AsgariUcretDestekTutarlariCekildi)
                    {
                        Mesaj = "Asgari ücret destek tutarları çekilecek";

                        BasvuruLogEkle(Mesaj);

                        Metodlar.DetayliLogYaz(Mesaj);

                    AsgariUcretDestekTutarlariCek:

                        var wc = BasvuruWebClient;

                        string yanit = wc.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkMufredat.action;", string.Empty);

                        if (yanit.Equals("LogOut"))
                        {
                            Metodlar.DetayliLogYaz("Web client logout olduğu için asgari ücret destek tutarları indirme işlemi devam etmeyecek");

                            return;
                        }

                        if (yanit.Contains("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır"))
                        {
                            wc.ReConnect();
                            goto AsgariUcretDestekTutarlariCek;
                        }

                        html.LoadHtml(yanit);

                        var pencereLinkIdYeni = html.GetElementbyId("pencereLinkIdYeni");
                        if (pencereLinkIdYeni != null)
                        {
                            string newUrl = pencereLinkIdYeni.Attributes["src"].Value;

                            yanit = wc.Get(newUrl, string.Empty);

                            if (yanit.Equals("LogOut"))
                            {
                                Metodlar.DetayliLogYaz("Web client logout olduğu için asgari ücret destek tutarları indirme işlemi devam etmeyecek");
                                return;
                            }

                            if (yanit.Contains("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır"))
                            {
                                wc.ReConnect();
                                goto AsgariUcretDestekTutarlariCek;
                            }

                            if (yanit.Contains("MÜFREDAT KARTI"))
                            {
                                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                                htmlDoc.LoadHtml(yanit);

                                var yillar = htmlDoc.GetElementbyId("sec").Descendants("option").ToDictionary(p => p.InnerText, p => p.GetAttributeValue("value", ""));

                                var asgariUcretDestekTutarlari = new List<AsgariUcretDestekTutarlari>();

                                foreach (var item in yillar)
                                {
                                    var yil = item.Key.Trim().ToLong();
                                    var val = item.Value;

                                    if (yil < 2016) break;

                                    if (asgariUcretDestekTutarlari.Any(p => p.DonemYil.Equals(yil))) continue;

                                    yanit = wc.PostData("https://uyg.sgk.gov.tr/YeniSistem/Isveren/secimBelirle.action;", "secim=3&borcTur=100&muf_secim=0&baslangicTarih=&bitisTarih=&donem_yil_ay_index_bas=0&donem_yil_ay_index_bit=0&fisTarih=" + DateTime.Today.ToString("dd.MM.yyyy") + "&sec=" + val);

                                    if (yanit.Equals("LogOut"))
                                    {
                                        Metodlar.DetayliLogYaz("Web client logout olduğu için asgari ücret destek tutarları indirme işlemi devam etmeyecek");

                                        return;
                                    }

                                    if (yanit.Contains("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır"))
                                    {
                                        wc.ReConnect();
                                        goto AsgariUcretDestekTutarlariCek;
                                    }

                                    if (yanit.Contains("MÜFREDAT KARTI"))
                                    {
                                        html.LoadHtml(yanit);

                                        var table = html.DocumentNode.Descendants("table").FirstOrDefault(p => p.GetAttributeValue("class", "").Equals("gradienttable") && p.InnerText.Contains("Faydalanılan Gün Sayısı"));

                                        if (table != null)
                                        {
                                            var trs = table.Descendants("tr");
                                            var trsCount = trs.Count();

                                            for (int i = 1; i < trsCount; i++)
                                            {
                                                var tds = trs.ElementAt(i).Descendants("td");

                                                var tutar = tds.ElementAt(4).InnerText.Replace("TL", "").Trim().ToDecimalSgk();

                                                var donemYil = Convert.ToInt64(tds.ElementAt(1).InnerText.Trim());
                                                var donemAy = Convert.ToInt64(tds.ElementAt(2).InnerText.Trim());
                                                var hesaplananGun = Convert.ToInt64(tds.ElementAt(3).InnerText.Trim().Replace(".", "").Replace(",", ""));

                                                var audt = asgariUcretDestekTutarlari.FirstOrDefault(p => p.DonemYil.Equals(donemYil) && p.DonemAy.Equals(donemAy));

                                                if (audt == null)
                                                {
                                                    audt = new AsgariUcretDestekTutarlari
                                                    {
                                                        IsyeriID = SuanYapilanIsyeriBasvuru.IsyeriID,
                                                        DonemYil = donemYil,
                                                        DonemAy = donemAy
                                                    };

                                                    asgariUcretDestekTutarlari.Add(audt);
                                                }

                                                audt.HesaplananGun += (tutar < 0) ? (0 - hesaplananGun) : hesaplananGun;

                                            }



                                        }
                                    }
                                    else goto AsgariUcretDestekTutarlariCek;
                                }

                                if (asgariUcretDestekTutarlari.Count > 0)
                                {

                                    using (var dbContext = new DbEntities())
                                    {
                                        dbContext.AsgariUcretDestekTutarlari.RemoveRange(dbContext.AsgariUcretDestekTutarlari.Where(p => p.IsyeriID.Equals(SuanYapilanIsyeriBasvuru.IsyeriID)));

                                        dbContext.AsgariUcretDestekTutarlari.AddRange(asgariUcretDestekTutarlari);

                                        dbContext.SaveChanges();
                                    }


                                    BasvuruLogEkle("Asgari ücret destek tutarları çekildi");

                                    Metodlar.DetayliLogYaz("Asgari ücret destek tutarları çekildi");
                                }
                                else
                                {
                                    BasvuruLogEkle("Asgari ücret destek tutarı bulunamadı");

                                    Metodlar.DetayliLogYaz("Asgari ücret destek tutarı bulunamadı");
                                }

                                AsgariUcretDestekTutarlariCekildi = true;

                            }
                            else
                            {
                                Metodlar.DetayliLogYaz("pencereLimkIdYeni içinde MÜFREDAT KARTI yazmadığı için geçerli sayfa bulunamadı. Asgari ücret destek tutarları indirme işlemi baştan başlayacak");

                                goto AsgariUcretDestekTutarlariCek;
                            }
                        }
                        else
                        {
                            Metodlar.DetayliLogYaz("pencereLimkIdYeni frame'i bulunamadığı için asgari ücret destek tutarları indirme işlemi baştan başlayacak");

                            goto AsgariUcretDestekTutarlariCek;
                        }


                    }

                    #endregion

                    #region Başvuru Dönemleri Çekme

                    if (!BasvuruDonemleriCekildi && Program.BasvuruDonemleriCekilsin)
                    {
                        Mesaj = "Başvuru dönemleri çekilecek";

                        BasvuruLogEkle(Mesaj);

                        var basvuruDonemleriKayitBulunamadıDenemeSayisi = 0;

                        var tamamlananlar = new HashSet<string>();

                    BasvuruDonemleriCek:

                        var wc = BasvuruWebClient;

                        string yanit = wc.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444717Basvuru.action", string.Empty);

                        if (yanit.Equals("LogOut")) return;

                        if (yanit.Contains("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır"))
                        {
                            wc.ReConnect();

                            BasvuruLogEkle("Başvuru dönemi indirmede \"Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır\" uyarısı ile karşılaşıldı. Çıkış yapılıp tekrar denenecek");

                            goto BasvuruDonemleriCek;
                        }

                        if (yanit.Contains("5510/ EK 17. MADDE BAŞVURU İŞLEMLERİ"))
                        {
                            html.LoadHtml(yanit);

                            var viewstate = html.GetElementbyId("javax.faces.ViewState").GetAttributeValue("value", "");

                            var basvurular = new Dictionary<string, BasvuruDonemleri>();

                            var page = 0;
                            var rows = 20;

                            string icerik = "";

                            while (true)
                            {
                                var sayac = 0;
                            BasvuruSatirlariCek:


                                var data = wc.PostData("https://uyg.sgk.gov.tr/IsverenSistemi/pages/genelParametreler/gecici17Basvuru.jsf", String.Format("javax.faces.partial.ajax=true&javax.faces.source=dataTableBasvurulistesi&javax.faces.partial.execute=dataTableBasvurulistesi&javax.faces.partial.render=dataTableBasvurulistesi&dataTableBasvurulistesi=dataTableBasvurulistesi&dataTableBasvurulistesi_pagination=true&dataTableBasvurulistesi_first={0}&dataTableBasvurulistesi_rows={1}&dataTableBasvurulistesi_encodeFeature=true&tableForm=tableForm&dataTableBasvurulistesi_selection=&javax.faces.ViewState={2}", page, rows, System.Net.WebUtility.UrlEncode(viewstate)));

                                if (data.Contains("<partial-response><changes><update id=\"araciMessages\">"))
                                {
                                    var xdoc = System.Xml.Linq.XDocument.Parse(data);

                                    var htmlContentElement = xdoc.Descendants("update").FirstOrDefault(p => p.Attribute("id").Value == "dataTableBasvurulistesi");

                                    if (htmlContentElement != null && !string.IsNullOrWhiteSpace(htmlContentElement.Value.Trim()))
                                    {
                                        if (htmlContentElement.Value.Contains("Kayıt bulunamadı")) break;

                                        icerik += htmlContentElement.Value;

                                        var matcheskeys = Regex.Matches(htmlContentElement.Value, "data-rk=\"([\\d-]+)\"").Cast<Match>();
                                        var matches = Regex.Matches(htmlContentElement.Value, "<div class=\"ui-dt-c\">([\\d/]+)</div>");


                                        foreach (var item in matcheskeys)
                                        {
                                            basvurular.Add(item.Groups[1].Value, new BasvuruDonemleri());
                                        }

                                        for (int i = 0; i < matcheskeys.Count(); i++)
                                        {
                                            var rowkey = matcheskeys.ElementAt(i).Groups[1].Value;
                                            var basvuru = basvurular[rowkey];
                                            basvuru.BasvuruDonem = matches[i * 3 + 1].Groups[1].Value;
                                        }
                                    }
                                    else break;

                                }
                                else
                                {
                                    sayac++;

                                    if (sayac < 3)
                                    {
                                        Thread.Sleep(1000);
                                        goto BasvuruSatirlariCek;
                                    }
                                    else break;

                                }

                                page += 20;
                            }

                            BasvuruDonemleriCekildi = true;

                            using (var dbContext = new DbEntities())
                            {
                                dbContext.BasvuruDonemleri.RemoveRange(dbContext.BasvuruDonemleri.Where(p => p.IsyeriID.Equals(SuanYapilanIsyeriBasvuru.IsyeriID)));

                                var basvuruCount = basvurular.Count();

                                var basvuruSayisi = 0;

                                for (int i = 0; i < basvuruCount; i++)
                                {
                                    var rowkey = basvurular.ElementAt(i).Key;

                                    if (tamamlananlar.Contains(rowkey)) continue;

                                    var basvuruDonem = basvurular.ElementAt(i).Value;

                                    //if (!tr.InnerText.Contains("Kayıt bulunamadı"))
                                    {

                                        basvuruDonem.IsyeriID = SuanYapilanIsyeriBasvuru.IsyeriID;


                                        //var rowkey = Regex.Match(tr.GetAttributeValue("data-rk", ""), "(-?\\d+)").Groups[1].Value;

                                        //var tds = trs.ElementAt(i).Descendants("td");

                                        //basvuruDonem.BasvuruDonem = tds.ElementAt(1).InnerText;

                                        //BasvuruDonemleriCek3:

                                        var yanitpdf = BasvuruWebClient.PostData("https://uyg.sgk.gov.tr/IsverenSistemi/pages/genelParametreler/gecici17Basvuru.jsf", String.Format("javax.faces.partial.ajax=true&javax.faces.source=dataTableBasvurulistesi&javax.faces.partial.execute=dataTableBasvurulistesi&javax.faces.partial.render=formmessage+tableForm+isyeriForm+basvuruGuncelleForm&javax.faces.behavior.event=rowSelect&javax.faces.partial.event=rowSelect&dataTableBasvurulistesi_instantSelectedRowKey={0}&tableForm=tableForm&dataTableBasvurulistesi_selection={1}&javax.faces.ViewState={2}", rowkey, rowkey, viewstate));

                                        if (yanitpdf.Equals("LogOut")) return;

                                        if (yanitpdf.Contains("PDF DÖKÜMÜ"))
                                        {

                                            HtmlAgilityPack.HtmlDocument htmlpdf = new HtmlAgilityPack.HtmlDocument();

                                            htmlpdf.LoadHtml(yanitpdf);

                                            var linkpdf = htmlpdf.DocumentNode.Descendants("a").FirstOrDefault(p => p.OuterHtml.Contains("internetLinkTesvik444717BasvuruPdf.action"));

                                            if (linkpdf != null)
                                            {
                                                var pdfId = linkpdf.GetAttributeValue("href", "").Split('=')[1];

                                            BasvuruDonemleriCek2:

                                                yanitpdf = BasvuruWebClient.Get("https://uyg.sgk.gov.tr" + linkpdf.GetAttributeValue("href", ""), "");

                                                if (yanitpdf.Equals("LogOut")) return;

                                                if (yanitpdf.Contains("https://uyg.sgk.gov.tr/YeniSistem//gecici17BasvuruHazirlaPdfOlustur.action"))
                                                {
                                                    htmlpdf.LoadHtml(yanitpdf);

                                                    var newUrl = htmlpdf.GetElementbyId("pencereLinkIdYeni").GetAttributeValue("src", "");

                                                BasvuruPdfIndir:
                                                    var pdfData = BasvuruWebClient.DownloadFileGet(newUrl, "");

                                                    if (pdfData.Length > 0)
                                                    {

                                                        var pdfText = GetPdfText(pdfData).ToString().Replace("\n", "");

                                                        var match = Regex.Match(pdfText, ".*BAVURULABLECEK YIL VE AYLAR :(.*)YER SCL NO.*");

                                                        if (!match.Success)
                                                        {
                                                            match = Regex.Match(pdfText, ".*BAŞVURULABİLECEK YIL VE AYLAR :(.*)İŞYERİ SİCİL NO.*");
                                                        }

                                                        if (match.Success)
                                                        {
                                                            var aylar = match.Groups[1].Value.Split('-');

                                                            string basvuruAylar = string.Empty;

                                                            foreach (var item in aylar)
                                                            {
                                                                var yil = item.Split('/')[0];
                                                                var ayUzun = item.Split('/')[1];
                                                                string ay = "";
                                                                if (ayUzun.Contains("Ocak")) ay = "1";
                                                                else if (ayUzun.Contains("ubat")) ay = "2";
                                                                else if (ayUzun.Contains("Mart")) ay = "3";
                                                                else if (ayUzun.Contains("Nisan")) ay = "4";
                                                                else if (ayUzun.Contains("May")) ay = "5";
                                                                else if (ayUzun.Contains("Haziran")) ay = "6";
                                                                else if (ayUzun.Contains("Temmuz")) ay = "7";
                                                                else if (ayUzun.Contains("ustos")) ay = "8";
                                                                else if (ayUzun.Contains("Eyl")) ay = "9";
                                                                else if (ayUzun.Contains("Ekim")) ay = "10";
                                                                else if (ayUzun.Contains("Kas")) ay = "11";
                                                                else if (ayUzun.Contains("Ara")) ay = "12";

                                                                basvuruAylar += yil + "/" + ay + ",";
                                                            }

                                                            basvuruDonem.Aylar = basvuruAylar.Trim(',');

                                                            tamamlananlar.Add(rowkey);
                                                        }

                                                    }
                                                    else
                                                    {
                                                        Thread.Sleep(1000);

                                                        BasvuruLogEkle("Başvuru dönemleri çekilirken pdf dosyası indirilemedi tekrar denenecek");

                                                        goto BasvuruPdfIndir;
                                                    }

                                                }
                                                else
                                                {
                                                    Thread.Sleep(1000);

                                                    BasvuruLogEkle("Başvuru dönemleri çekilirken \"https://uyg.sgk.gov.tr/YeniSistem//gecici17BasvuruHazirlaPdfOlustur.action\" link bulunamadı. Tekrar denenecek");

                                                    File.WriteAllText("BasvuruDonemiTakilma.txt", yanitpdf + Environment.NewLine + Environment.NewLine + Environment.NewLine);

                                                    goto BasvuruDonemleriCek2;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Thread.Sleep(1000);

                                            BasvuruLogEkle("Başvuru dönemi çekilirken sayfada \"PDF DÖKÜMÜ\" metni bulunamadı. Tekrar denenecek");

                                            //String.Format("javax.faces.partial.ajax=true&javax.faces.source=dataTableBasvurulistesi&javax.faces.partial.execute=dataTableBasvurulistesi&javax.faces.partial.render=formmessage+tableForm+isyeriForm+basvuruGuncelleForm&javax.faces.behavior.event=rowSelect&javax.faces.partial.event=rowSelect&dataTableBasvurulistesi_instantSelectedRowKey={0}&tableForm=tableForm&dataTableBasvurulistesi_selection={1}&javax.faces.ViewState={2}", rowkey, rowkey, viewstate)

                                            var log = "Başvuru dönemi çekilirken sayfada \"PDF DÖKÜMÜ\" metni bulunamadı. Tekrar denenecek" + Environment.NewLine + Environment.NewLine;

                                            log += String.Format("javax.faces.partial.ajax=true&javax.faces.source=dataTableBasvurulistesi&javax.faces.partial.execute=dataTableBasvurulistesi&javax.faces.partial.render=formmessage+tableForm+isyeriForm+basvuruGuncelleForm&javax.faces.behavior.event=rowSelect&javax.faces.partial.event=rowSelect&dataTableBasvurulistesi_instantSelectedRowKey={0}&tableForm=tableForm&dataTableBasvurulistesi_selection={1}&javax.faces.ViewState={2}", rowkey, rowkey, viewstate) + Environment.NewLine + Environment.NewLine;

                                            log += yanitpdf + Environment.NewLine + Environment.NewLine + Environment.NewLine;

                                            log += "İçerik" + Environment.NewLine + Environment.NewLine + icerik + Environment.NewLine + Environment.NewLine + Environment.NewLine;

                                            File.WriteAllText("BasvuruDonemiTakilma.txt", log);

                                            goto BasvuruDonemleriCek;
                                        }

                                        dbContext.BasvuruDonemleri.Add(basvuruDonem);

                                        basvuruSayisi++;
                                    }

                                }

                                if (basvuruSayisi == 0)
                                {
                                    basvuruDonemleriKayitBulunamadıDenemeSayisi++;

                                    if (basvuruDonemleriKayitBulunamadıDenemeSayisi < 3)
                                    {
                                        wc.ReConnect();

                                        BasvuruLogEkle("Başvuru dönemleri sayfasında hiç kayıt bulunamadı. Tekrar denenecek");

                                        goto BasvuruDonemleriCek;
                                    }
                                }
                                else
                                {
                                    dbContext.SaveChanges();
                                }

                                BasvuruLogEkle(basvuruSayisi > 0 ? "Başvuru dönemleri çekildi" : "Hiç başvuru yapılmamış");

                            }

                        }
                        else
                        {
                            BasvuruLogEkle("Başvuru dönemleri sayfasında \"5510 / EK 17.MADDE BAŞVURU İŞLEMLERİ\" metni bulunamadı. Sayfa tekrar yüklenecek.");

                            File.WriteAllText("BasvuruDonemiTakilma.txt", yanit + Environment.NewLine + Environment.NewLine + Environment.NewLine);

                            goto BasvuruDonemleriCek;
                        }


                    }

                    #endregion

                    #region Aylik Calisan Sayisini Çekme

                    if (!AylikCalisanSayilariCekildi)
                    {
                        Mesaj = "Aylık çalışan sayıları çekilecek";

                        BasvuruLogEkle(Mesaj);

                        Metodlar.DetayliLogYaz(Mesaj);

                    AylikCalisanSayilariCek:

                        Dictionary<string, KeyValuePair<string, string>> DonemAylikCalisanSayilari = new Dictionary<string, KeyValuePair<string, string>>();

                        var wc = BasvuruWebClient;

                        string yanit = wc.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkCalisanSayisi.action;", string.Empty);

                        if (yanit.Equals("LogOut"))
                        {
                            Metodlar.DetayliLogYaz("Web client logout olduğu için aylık çalışan sayısı çekme işlemi devam etmeyecek");

                            return;
                        }

                        if (yanit.Contains("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır"))
                        {
                            wc.ReConnect();
                            goto AylikCalisanSayilariCek;
                        }

                        html.LoadHtml(yanit);

                        var pencereLinkIdYeni = html.GetElementbyId("pencereLinkIdYeni");
                        if (pencereLinkIdYeni != null)
                        {
                            string newUrl = pencereLinkIdYeni.Attributes["src"].Value;

                            yanit = wc.Get(newUrl, string.Empty);

                            if (yanit.Equals("LogOut"))
                            {
                                Metodlar.DetayliLogYaz("Web client logout olduğu için aylık çalışan sayısı çekme işlemi devam etmeyecek");
                                return;
                            }

                            if (yanit.Contains("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır"))
                            {
                                wc.ReConnect();
                                goto AylikCalisanSayilariCek;
                            }

                            if (yanit.Contains("AYLIK TOPLAM SİGORTALI SAYISI GÖRÜNTÜLEME"))
                            {
                                html.LoadHtml(yanit);

                                var yilayselect = html.GetElementbyId("ortalamaCalisan_sent_donem_yil_ay_index");

                                var yilayselectOptions = yilayselect.Descendants("option");

                                foreach (var yilayselectOption in yilayselectOptions)
                                {
                                    var donem_yil_ay_index = Convert.ToInt32(yilayselectOption.GetAttributeValue("value", ""));

                                    if (donem_yil_ay_index > 0)
                                    {
                                        var tarih = Convert.ToDateTime(yilayselectOption.InnerText);

                                        if (tarih < new DateTime(2011, 3, 1)) continue;

                                        AylikCalisanSayilariCek2:

                                        var yanit2 = wc.PostData("https://uyg.sgk.gov.tr/YeniSistem/Isveren/ortalamaCalisan_sent.action", "donem_yil_ay_index=" + donem_yil_ay_index);

                                        if (yanit2.Equals("LogOut"))
                                        {
                                            Metodlar.DetayliLogYaz("Web client logout olduğu için aylık çalışan sayısı çekme işlemi devam etmeyecek");
                                            return;
                                        }

                                        if (yanit2.Contains("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır"))
                                        {
                                            wc.ReConnect();
                                            goto AylikCalisanSayilariCek;
                                        }

                                        if (yanit2.Contains("AYLIK TOPLAM SİGORTALI SAYISI GÖRÜNTÜLEME"))
                                        {
                                            if (!yanit2.Contains("LİSTELENECEK VERİ BULUNAMAMIŞTIR"))
                                            {
                                                HtmlAgilityPack.HtmlDocument htmlCalisanSayisi = new HtmlAgilityPack.HtmlDocument();

                                                htmlCalisanSayisi.LoadHtml(yanit2);

                                                var geneluyariCenterTag = htmlCalisanSayisi.GetElementbyId("genelUyariCenterTag");

                                                if (geneluyariCenterTag != null && !String.IsNullOrEmpty(geneluyariCenterTag.InnerText)) goto AylikCalisanSayilariCek2;

                                                var tableAylikCalisanSayisi = htmlCalisanSayisi.DocumentNode.Descendants("table").FirstOrDefault(p => p.GetAttributeValue("class", "").Equals("gradienttable") && p.InnerText != null && p.InnerText.Contains("Çalışan Sayısı"));

                                                var trs = tableAylikCalisanSayisi.Descendants("tr");

                                                var trsCount = trs.Count();

                                                string donem = null;

                                                int aylikCalisanSayisiTaseronlu = 0;
                                                int aylikCalisanSayisiTaseronsuz = -1;

                                                for (int i = 1; i < trsCount; i++)
                                                {
                                                    var tr = trs.ElementAt(i);

                                                    var tds = tr.Descendants("td");

                                                    if (tds.Count() == 0) continue;

                                                    var ths = tr.Descendants("th");

                                                    var araci = ths.Count() > 0 ? ths.ElementAt(0).InnerText.Trim() : tds.ElementAt(0).InnerText.Trim();

                                                    var aylikCalisanSayi = (ths.Count() > 0 ? tds.ElementAt(0) : tds.ElementAt(2)).InnerText.Trim().Replace(".", "").Replace(",", "");

                                                    if (ths.Count() > 0)
                                                    {
                                                        aylikCalisanSayisiTaseronlu = Convert.ToInt32(aylikCalisanSayi);
                                                    }

                                                    if (tds.Count() > 1 && DateTime.TryParse(tds.ElementAt(1).InnerText.Trim(), out DateTime date))
                                                    {
                                                        donem = tds.ElementAt(1).InnerText.Trim();
                                                    }

                                                    if (ths.Count() == 0 && !araci.EndsWith("Mükerrer Bildirim"))
                                                    {
                                                        int.TryParse(araci.Split('-')[0].Trim(), out int aracino);
                                                        int.TryParse(SuanYapilanIsyeriBasvuru.TaseronNo, out int taseronno);

                                                        if (aracino == taseronno)
                                                        {
                                                            aylikCalisanSayisiTaseronsuz = Convert.ToInt32(aylikCalisanSayi);
                                                        }
                                                    }
                                                }

                                                if (!DonemAylikCalisanSayilari.ContainsKey(donem))
                                                {
                                                    DonemAylikCalisanSayilari.Add(donem, new KeyValuePair<string, string>(aylikCalisanSayisiTaseronlu.ToString(), aylikCalisanSayisiTaseronsuz.ToString()));
                                                }

                                            }
                                        }
                                        else goto AylikCalisanSayilariCek;

                                    }
                                }

                                using (var dbContext = new DbEntities())
                                {
                                    dbContext.AylikCalisanSayilari.RemoveRange(dbContext.AylikCalisanSayilari.Where(p => p.IsyeriID.Equals(SuanYapilanIsyeriBasvuru.IsyeriID)));

                                    foreach (var item in DonemAylikCalisanSayilari)
                                    {

                                        var ayc = new AylikCalisanSayilari
                                        {
                                            IsyeriID = SuanYapilanIsyeriBasvuru.IsyeriID,
                                            DonemYil = Convert.ToInt64(item.Key.Split('/')[0]),
                                            DonemAy = Convert.ToInt64(item.Key.Split('/')[1]),
                                            CalisanSayisiTaseronlu = Convert.ToInt64(item.Value.Key),
                                            CalisanSayisiTaseronsuz = Convert.ToInt64(item.Value.Value),
                                        };

                                        dbContext.AylikCalisanSayilari.Add(ayc);
                                    }

                                    dbContext.SaveChanges();

                                }

                                BasvuruLogEkle("Aylık çalışan sayıları çekildi");

                                Metodlar.DetayliLogYaz("Aylık çalışan sayıları çekildi");
                            }
                            else
                            {
                                Metodlar.DetayliLogYaz("pencereLinkIdYeni içinde AYLIK TOPLAM SİGORTALI SAYISI GÖRÜNTÜLEME yazmadığı için geçerli sayfa bulunamadı. Aylık çalışan sayısı çekme işlemi işlemi baştan başlayacak");

                                goto AylikCalisanSayilariCek;
                            }
                        }
                        else
                        {
                            Metodlar.DetayliLogYaz("pencereLinkIdYeni frame'i bulunamadığı için aylık çalışan sayısı çekme işlemi baştan başlayacak");

                            goto AylikCalisanSayilariCek;
                        }

                        AylikCalisanSayilariCekildi = true;
                    }

                    #endregion
                }
                else
                {
                    AylikCalisanSayilariCekildi = true;
                    BasvuruDonemleriCekildi = true;
                    AsgariUcretDestekTutarlariCekildi = true;
                }
            }

            #region 7166 Listesi Kontrolü

            if (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf7103)
            {
                if (!BasvuruListesi7166KontrolEdildi)
                {
                    BasvuruListesi7166KontrolEdildi = true;

                    Mesaj = "7166 Listesindeki kişiler kontrol edilecek";

                    BasvuruLogEkle(Mesaj);

                    foreach (var kisi in basvuruListesi7166Kisiler)
                    {
                        if (kisi.UygunlukDurumu.Equals("Uygundur"))
                        {
                            if (!UcretDestegiIstenecekKisiler.Contains(kisi)) UcretDestegiIstenecekKisiler.Add(kisi);

                        }
                        else if (kisi.UygunlukDurumu.Equals("Uygun Değildir"))
                        {
                            if (!UcretDestegiIstenmeyecekKisiler.Contains(kisi)) UcretDestegiIstenmeyecekKisiler.Add(kisi);
                        }
                    }

                    if (basvuruListesi7166Kisiler.Count > 0)
                    {


                    BasvuruListesi7166Kontrolu:

                        List<BasvuruKisiDownload7103> kisiler7103 = new List<BasvuruKisiDownload7103>();

                        var wc7166 = BasvuruWebClient;

                        string yanit = wc7166.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444719Liste.action;", string.Empty);

                        if (yanit.Contains("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır"))
                        {
                            wc7166.ReConnect();
                            goto BasvuruListesi7166Kontrolu;
                        }

                        var html7166 = new HtmlAgilityPack.HtmlDocument();

                        html7166.LoadHtml(yanit);

                        var pencereLinkIdYeni = html7166.GetElementbyId("pencereLinkIdYeni");

                        if (pencereLinkIdYeni != null)
                        {
                            if (pencereLinkIdYeni.OuterHtml.Contains("src=\"https://uyg.sgk.gov.tr/YeniSistem/Isveren/tesvik4447_19_sigortali.action;"))
                            {
                                string newUrl = pencereLinkIdYeni.GetAttributeValue("src", "");

                                yanit = wc7166.Get(newUrl, string.Empty);

                                if (yanit.Contains("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır"))
                                {
                                    wc7166.ReConnect();
                                    goto BasvuruListesi7166Kontrolu;
                                }

                                html7166.LoadHtml(yanit);

                                if (yanit != null && (yanit.Contains("<center>4447 GEÇİCİ 19. MADDE KONTROL İŞLEMLERİ</center>") || yanit.Contains("<center>4447/ GEÇİCİ 19. MADDE TANIMLI SİGORTALILAR LİSTESİ</center>")))
                                {
                                    var toplamKisiSayisi = Convert.ToInt32(Regex.Match(yanit, "var toplamKayitSay = parseInt\\('(.*)'\\)").Groups[1].Value);

                                    List<BasvuruKisiDownload7103> kisiler = new List<BasvuruKisiDownload7103>();
                                    List<KeyValuePair<BasvuruKisiDownload7103, string>> silinecekKayitlar = new List<KeyValuePair<BasvuruKisiDownload7103, string>>();
                                    List<KeyValuePair<BasvuruKisiDownload7103, string>> bulunanKisiler = new List<KeyValuePair<BasvuruKisiDownload7103, string>>();

                                    var satirlar = html7166.DocumentNode.SelectNodes("//table[@class='paginated gradienttable']/tbody/tr");
                                    if (satirlar != null)
                                    {
                                        bool YeniSablon = satirlar.First().ParentNode.ParentNode.InnerText.Contains("Ücret Desteği Tercihi");

                                        if (kisiler7103.Count == 0 || (kisiler7103.Count > 0 && !satirlar.First().SelectSingleNode("td[1]/p/text()").GetInnerText().Equals("1")))
                                        {
                                            foreach (var satir in satirlar)
                                            {
                                                var tcno = satir.SelectSingleNode("td[2]/p/text()").GetInnerText().Trim();

                                                var inputradio = satir.Descendants("input").FirstOrDefault(p => p.GetAttributeValue("type", "").Equals("radio"));

                                                var kayitID = Regex.Match(inputradio.OuterHtml, ".*fielddeger:\"(\\d*).*").Groups[1].Value;

                                                BasvuruKisiDownload7103 kisi7103 = new BasvuruKisiDownload7103();
                                                kisi7103.TcKimlikNo = tcno;

                                                if (YeniSablon)
                                                {
                                                    kisi7103.UcretDestegiTercihi = satir.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim();
                                                    kisi7103.GirisTarihi = satir.SelectSingleNode("td[14]/p/text()").GetInnerText().Trim();
                                                }
                                                else kisi7103.GirisTarihi = satir.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim();

                                                var kisi7166 = basvuruListesi7166Kisiler.FirstOrDefault(p => p.TckimlikNo.Equals(tcno) && p.Giris.Equals(Convert.ToDateTime(kisi7103.GirisTarihi)));

                                                if (kisi7166 != null)
                                                {
                                                    bulunanKisiler.Add(new KeyValuePair<BasvuruKisiDownload7103, string>(kisi7103, kayitID));
                                                }


                                                kisiler7103.Add(kisi7103);
                                            }

                                            //kişi listesinin tümünü indirmek için tüm sayfalar gezilir.
                                            //while (kisiler7103.Count > 0 && kisiler7103.Count % 100 == 0)
                                            while (kisiler7103.Count < toplamKisiSayisi)
                                            {
                                                string yanitsonraki = wc7166.PostData("https://uyg.sgk.gov.tr/YeniSistem/ListelemManager/sonrakiSayfalarAction.action", "ilkKayitIleriGeriSonKayit=3&herSayfadakiSatirSay=20");

                                                if (yanit.Contains("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır"))
                                                {
                                                    wc7166.ReConnect();
                                                    goto BasvuruListesi7166Kontrolu;
                                                }

                                                if (yanitsonraki != null && (yanitsonraki.Contains("<center>4447 GEÇİCİ 19. MADDE KONTROL İŞLEMLERİ</center>") || yanitsonraki.Contains("<center>4447/ GEÇİCİ 19. MADDE TANIMLI SİGORTALILAR LİSTESİ</center>")))
                                                {
                                                    var htmldevam = new HtmlAgilityPack.HtmlDocument();
                                                    htmldevam.LoadHtml(yanitsonraki);

                                                    var satirlardevamsayfasi = htmldevam.DocumentNode.SelectNodes("//table[@class='paginated gradienttable']/tbody/tr");
                                                    if (satirlardevamsayfasi != null)
                                                    {
                                                        if (satirlardevamsayfasi.First().SelectSingleNode("td[1]/p/text()").GetInnerText().Equals((kisiler7103.Count + 1).ToString()))
                                                        {
                                                            foreach (var satirdevam in satirlardevamsayfasi)
                                                            {
                                                                var tcno = satirdevam.SelectSingleNode("td[2]/p/text()").GetInnerText().Trim();

                                                                var inputradio = satirdevam.Descendants("input").FirstOrDefault(p => p.GetAttributeValue("type", "").Equals("radio"));

                                                                var kayitID = Regex.Match(inputradio.OuterHtml, ".*fielddeger:\"(\\d*).*").Groups[1].Value;

                                                                BasvuruKisiDownload7103 kisi7103 = new BasvuruKisiDownload7103();
                                                                kisi7103.TcKimlikNo = tcno;

                                                                if (YeniSablon)
                                                                {
                                                                    kisi7103.UcretDestegiTercihi = satirdevam.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim();
                                                                    kisi7103.GirisTarihi = satirdevam.SelectSingleNode("td[14]/p/text()").GetInnerText().Trim();
                                                                }
                                                                else kisi7103.GirisTarihi = satirdevam.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim();

                                                                var kisi7166 = basvuruListesi7166Kisiler.FirstOrDefault(p => p.TckimlikNo.Equals(tcno) && p.Giris.Equals(Convert.ToDateTime(kisi7103.GirisTarihi)));

                                                                if (kisi7166 != null)
                                                                {
                                                                    bulunanKisiler.Add(new KeyValuePair<BasvuruKisiDownload7103, string>(kisi7103, kayitID));
                                                                }

                                                                kisiler7103.Add(kisi7103);
                                                            }
                                                        }
                                                        else break;
                                                    }
                                                }
                                                else goto BasvuruListesi7166Kontrolu;
                                            }
                                        }
                                    }


                                    if (bulunanKisiler.Count > 0)
                                    {
                                        foreach (var kisi7166 in basvuruListesi7166Kisiler)
                                        {
                                            if (!kisi7166.VerilmisMi7103)
                                            {
                                                foreach (var bulunan in bulunanKisiler)
                                                {
                                                    var iseGirisTarihi = Convert.ToDateTime(bulunan.Key.GirisTarihi);

                                                    if (kisi7166.TckimlikNo.Equals(bulunan.Key.TcKimlikNo) && kisi7166.Giris.Equals(iseGirisTarihi))
                                                    {
                                                        var IseGirisAy = new DateTime(iseGirisTarihi.Year, iseGirisTarihi.Month, 1);

                                                        if (dtBaslangic7103 <= IseGirisAy && dtBitis7103 >= IseGirisAy)
                                                        {
                                                            if (kisi7166.UygunlukDurumu.Equals("Uygundur"))
                                                            {
                                                                if (!bulunan.Key.UcretDestegiTercihi.Equals("İSTİYOR"))
                                                                {
                                                                    if (!silinecekKayitlar.Any(p => p.Value.Equals(bulunan.Value))) silinecekKayitlar.Add(bulunan);
                                                                }
                                                            }
                                                            else if (kisi7166.UygunlukDurumu.Equals("Uygun Değildir"))
                                                            {
                                                                if (bulunan.Key.UcretDestegiTercihi.Equals("İSTİYOR"))
                                                                {
                                                                    if (!silinecekKayitlar.Any(p => p.Value.Equals(bulunan.Value))) silinecekKayitlar.Add(bulunan);
                                                                }
                                                            }
                                                        }

                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        if (silinecekKayitlar.Count > 0)
                                        {
                                        SilinecekleriSil:

                                            int ind = 0;

                                            while (ind < silinecekKayitlar.Count)
                                            {
                                                var kisi7166 = silinecekKayitlar[ind].Key;

                                                yanit = wc7166.PostData("https://uyg.sgk.gov.tr/YeniSistem/Isveren/tesvik4447_19_sigortaliTanimiSil.action", "kayitId=" + silinecekKayitlar[ind].Value);

                                                if (yanit.Contains("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır"))
                                                {
                                                    wc7166.ReConnect();
                                                    goto SilinecekleriSil;
                                                }

                                                if (yanit != null && (yanit.Contains("<center>4447 GEÇİCİ 19. MADDE KONTROL İŞLEMLERİ</center>") || yanit.Contains("<center>4447/ GEÇİCİ 19. MADDE TANIMLI SİGORTALILAR LİSTESİ</center>")))
                                                {
                                                    if (yanit.Contains("İşleminiz Başarılı Bir Şekilde Tamamlanmıştır"))
                                                    {
                                                        Mesaj = String.Format("{0} Tc nolu kişinin {1} giriş tarihli 7103 kaydı başarıyla silinmiştir", kisi7166.TcKimlikNo, kisi7166.GirisTarihi);

                                                        BasvuruLogEkle(Mesaj);
                                                    }
                                                    else //if (yanit.Contains("silme işlemi yapılamaz"))
                                                    {
                                                        var htmlyanit = new HtmlAgilityPack.HtmlDocument();

                                                        htmlyanit.LoadHtml(yanit);

                                                        var uyari = htmlyanit.GetElementbyId("genelUyariCenterTag").InnerText;

                                                        if (!SisteminSildirmedigi7103Kayitlari.Any(p => p.Key.TcKimlikNo.Equals(kisi7166.TcKimlikNo) && p.Key.GirisTarihi.Equals(kisi7166.GirisTarihi)))
                                                        {
                                                            SisteminSildirmedigi7103Kayitlari.Add(kisi7166, uyari);
                                                        }

                                                        Mesaj = String.Format("{0} Tc nolu kişinin {1} giriş tarihli 7103 kaydını sistem sildirmemiştir", kisi7166.TcKimlikNo, kisi7166.GirisTarihi);

                                                        BasvuruLogEkle(Mesaj);
                                                    }

                                                    silinecekKayitlar.RemoveAt(ind);

                                                }
                                                else goto SilinecekleriSil;
                                            }
                                        }
                                    }
                                }
                                else goto BasvuruListesi7166Kontrolu;
                            }
                            else goto BasvuruListesi7166Kontrolu;
                        }
                        else goto BasvuruListesi7166Kontrolu;

                    }

                    Mesaj = "7166 Listesindeki kişilerin kontrol edilmesi tamamlandı";

                    BasvuruLogEkle(Mesaj);
                }
            }

            #endregion

            if (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf6111 || bfsira == Enums.BasvuruFormuTurleri.Bf7103 || bfsira == Enums.BasvuruFormuTurleri.Bf2828 || bfsira == Enums.BasvuruFormuTurleri.Bf7252 || bfsira == Enums.BasvuruFormuTurleri.Bf7256 || bfsira == Enums.BasvuruFormuTurleri.Bf7316 || bfsira == Enums.BasvuruFormuTurleri.Bf3294)
            {
                Mesaj = "Uygulama Başlatıldı";

                BasvuruLogEkle(Mesaj);

                Metodlar.DetayliLogYaz("BasvuruSayfaYükleMetodu: Teşvik Tanımlama Sayfası Açılacak");

                stopwatch.Start();

                siradakiIslem = "Teşvik Tanımlama Açılacak";

                string response = BasvuruWebClient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvikTanimlama.action;", string.Empty);

                YuklenenSayfaninIciniDolas(response);

                return;
            }
            else if (bfsira == Enums.BasvuruFormuTurleri.Bf6645)
            {
                BasvuruFormuIndir6645();
            }
            else if (bfsira == Enums.BasvuruFormuTurleri.Bf687)
            {
                BasvuruFormuIndir687();
            }
            else if (bfsira == Enums.BasvuruFormuTurleri.Bf14857)
            {
                BasvuruFormuIndir14857();
            }

        }
        void BasvuruFormuIndir6645()
        {
            int i = 0;

            while (i < IslemYapilacaklar6645.Count)
            {
                IptalKontrolu();

                var kisi6645 = IslemYapilacaklar6645.ElementAt(i);


                if (!kisi6645.Value)
                {

                    if (SuanYapilanKisi6645.Key == null || !SuanYapilanKisi6645.Key.Equals(kisi6645.Key.Key))
                    {
                        BasvuruLogEkle(kisi6645.Key.Key + " kişisi incelenmeye başladı");

                    }

                    SuanYapilanKisi6645 = kisi6645.Key;

                    string response = null;

                    do
                    {
                        IptalKontrolu();

                        response = BasvuruWebClient.PostData("https://uyg.sgk.gov.tr/Sigortali_Tesvik_4447_15/ActionMultiplexer?aid=IT_4447_15_ISL&islemturu=3", "form_tcno=" + kisi6645.Key.Key + "&form_sskno=&form_ad=&form_soyad=&form_ad=&kayitli_tcno=");

                        IptalKontrolu();

                        if (!response.Contains("4447-Geçici 15. Madde Teşvik Yönetimi"))
                        {
                            BasvuruLogEkle("Geçerli sayfa bulunamadı.Tekrar denenecek");
                        }


                        if (response.ToLower().Contains("error")
                                || (response.ToLower().Contains("hata") && !response.ToLower().Contains("hatalı bildirge verilmemesi"))
                                || response.ToLower().Contains("fail")
                                || response.ToLower().Contains("communication")
                                || response.ToLower().Contains("bağlanılamadı")
                                || response.ToLower().Contains("bağlantı")
                            )
                        {
                            if (!response.Contains("constraints"))
                            {

                                BasarisizDenemeler++;

                                BasvuruLogEkle(SuanYapilanKisi6645.Key + " kişisi hatadan dolayı " + BasarisizDenemeler + ".kez sorgulanıyor");
                            }
                            else
                            {
                                BasarisizDenemeler = 0;

                                break;
                            }
                        }
                        else
                        {
                            BasarisizDenemeler = 0;

                            break;
                        }

                        Thread.Sleep(1000);

                    }
                    while (BasarisizDenemeler <= maksimumDenemeSayisi_6645_687);

                    IptalKontrolu();

                    if (BasarisizDenemeler > maksimumDenemeSayisi_6645_687)
                    {
                        BasvuruLogEkle(SuanYapilanKisi6645.Key + " kişisi " + maksimumDenemeSayisi_6645_687 + " denemeye rağmen hata nedeniyle sorgulanamadı. Sıradaki kişiye geçilecek");
                    }
                    else
                    {
                        HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                        html.LoadHtml(response);

                        var kaydet = html.DocumentNode.SelectNodes("//a").FirstOrDefault(p => p.InnerText.Equals("KAYDET"));

                        if (kaydet != null)
                        {
                            do
                            {
                                IptalKontrolu();

                                response = BasvuruWebClient.PostData("https://uyg.sgk.gov.tr/Sigortali_Tesvik_4447_15/ActionMultiplexer?aid=IT_4447_15_ISL&islemturu=1", "form_tcno=" + kisi6645.Key.Key + "&form_sskno=&form_ad=&form_soyad=&form_ad=&kayitli_tcno=");

                                IptalKontrolu();

                                if (!response.Contains("4447-Geçici 15. Madde Teşvik Yönetimi"))
                                {
                                    BasvuruLogEkle("Geçerli sayfa bulunamadı. Tekrar denenecek");
                                }


                                if (response.ToLower().Contains("error")
                                        || (response.ToLower().Contains("hata") && !response.ToLower().Contains("hatalı bildirge verilmemesi"))
                                        || response.ToLower().Contains("fail")
                                        || response.ToLower().Contains("communication")
                                        || response.ToLower().Contains("bağlanılamadı")
                                        || response.ToLower().Contains("bağlantı")
                                    )
                                {
                                    if (!response.Contains("constraints"))
                                    {

                                        BasarisizDenemeler++;

                                        BasvuruLogEkle(SuanYapilanKisi6645.Key + " kişisi hatadan dolayı " + BasarisizDenemeler + ".kez kaydedilmeye çalışılıyor");
                                    }
                                    else
                                    {

                                        BasarisizDenemeler = 0;

                                        break;
                                    }
                                }
                                else
                                {
                                    BasarisizDenemeler = 0;

                                    break;
                                }

                                Thread.Sleep(1000);

                            }
                            while (BasarisizDenemeler <= maksimumDenemeSayisi_6645_687);

                            IptalKontrolu();

                            if (BasarisizDenemeler > maksimumDenemeSayisi_6645_687)
                            {
                                BasvuruLogEkle(SuanYapilanKisi6645.Key + " kişisi " + maksimumDenemeSayisi_6645_687 + " denemeye rağmen hata nedeniyle kaydedilemedi. Sıradaki kişiye geçilecek");
                            }
                            else
                            {
                                if (response.Contains("SİGORTALININ KAYDI BAŞARILI BİR ŞEKİLDE YAPILMIŞTIR"))
                                {
                                    YeniIslemYapilanlar6645.Add(new KeyValuePair<string, DateTime>(SuanYapilanKisi6645.Key, SuanYapilanKisi6645.Value));

                                    BasvuruLogEkle(SuanYapilanKisi6645.Key + " kişisi onaylanarak 6645 başvuru listesine eklendi");
                                }
                                else if (response.Contains("SORGULANAN SİGORTALI İÇİN BU TEŞVİKTEN YARARLANILMAKTADIR. TEKRAR KAYIT YAPILAMAZ"))
                                {
                                    BasvuruLogEkle(SuanYapilanKisi6645.Key + " kişi 6645 teşviği için daha önceden onaylanmış");
                                }
                            }

                            IslemYapilacaklar6645[SuanYapilanKisi6645] = true;

                        }
                        else
                        {
                            if (response.Contains("constraints"))
                            {
                                BasvuruLogEkle(SuanYapilanKisi6645.Key + " kişisi kişiye özel İşkur hatasından dolayı atlanıyor.Lütfen daha sonra tekrar deneyiniz veya manuel olarak kontrol ediniz");
                            }
                            else if (response.Contains("SORGULANAN SİGORTALI İÇİN BU TEŞVİKTEN YARARLANILMAKTADIR. TEKRAR KAYIT YAPILAMAZ"))
                            {
                                BasvuruLogEkle(SuanYapilanKisi6645.Key + " kişi 6645 teşviği için daha önceden onaylanmış");
                            }
                            else
                            {
                                BasvuruLogEkle(SuanYapilanKisi6645.Key + " kişisi incelendi. 6645 teşviği verilmeyecek.");
                            }

                            IslemYapilacaklar6645[SuanYapilanKisi6645] = true;

                        }
                    }

                }

                i++;
            }


            string responsetumkayitlar = null;

            do
            {
                IptalKontrolu();

                responsetumkayitlar = BasvuruWebClient.Get("https://uyg.sgk.gov.tr/Sigortali_Tesvik_4447_15/ActionMultiplexer?aid=IT_4447_15_ISL&islemturu=5", "");

                IptalKontrolu();

                if (!responsetumkayitlar.Contains("4447-Geçici 15. Madde Teşvik Yönetimi"))
                {
                    BasvuruLogEkle("Geçerli sayfa bulunamadı. Tekrar denenecek");

                    BasarisizDenemeler++;
                }
                else break;

                Thread.Sleep(1000);
            }
            while (BasarisizDenemeler <= maksimumDenemeSayisi_6645_687);


            if (BasarisizDenemeler > maksimumDenemeSayisi_6645_687)
            {
                BasvuruLogEkle(BasarisizDenemeler.ToString() + " denemeye rağmen tüm kayıtlar çekilemedi. Başvuru indirme işlemi sonlandırılıyor");

                BasvuruSonaErdi(false, true, "6645 tüm kayıtlar sayfası yüklenemedi");

            }
            else
            {
                BasvuruLogEkle("Tüm kişilerin incelenmesi tamamlandı. Başvuru listesi çekiliyor");

                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();

                html.LoadHtml(responsetumkayitlar);

                var table = html.GetElementbyId("thkkkTbl1");

                if (table != null)
                {
                    var trs = table.Descendants("tr");

                    foreach (var tr in trs)
                    {
                        var tds = tr.Descendants("td").ToList();

                        if (tds.Count > 1 && tds[1].InnerText != null && long.TryParse(tds[1].InnerText.Trim(), out long tc))
                        {
                            BasvuruKisiDownload6645 basvurukisi = new BasvuruKisiDownload6645();

                            basvurukisi.TcKimlikNo = tds[1].InnerText.Trim();

                            basvurukisi.Sicil = tds[2].InnerText != null ? tds[2].InnerText.Trim() : "";

                            basvurukisi.Ad = tds[3].InnerText != null ? tds[3].InnerText.Trim() : "";

                            basvurukisi.Soyad = tds[4].InnerText != null ? tds[4].InnerText.Trim() : "";

                            basvurukisi.TesvikSuresiBaslangic = tds[5].InnerText != null ? tds[5].InnerText.Trim() : "";

                            basvurukisi.TesvikSuresiBitis = tds[6].InnerText != null ? tds[6].InnerText.Trim() : "";

                            basvurukisi.Baz = tds[7].InnerText != null ? Convert.ToInt32(tds[7].InnerText.Trim()) : 0;

                            basvurukisi.Aktif = tds[8].InnerText != null ? tds[8].InnerText.Trim() : "";

                            basvurukisi.GirisTarihi = tds[9].InnerText != null ? Convert.ToDateTime(tds[9].InnerText.Trim()).ToString("dd.MM.yyyy") : "";

                            basvurukisi.IslemTarihi = tds[10].InnerText != null ? Convert.ToDateTime(tds[10].InnerText.Trim()).ToString("dd.MM.yyyy") : "";

                            foreach (var item in YeniIslemYapilanlar6645)
                            {
                                if (item.Key == basvurukisi.TcKimlikNo)
                                {
                                    basvurukisi.YeniIslemYapildi = true;

                                    break;
                                }
                            }

                            basvurukisiler6645.Add(basvurukisi);


                        }
                    }
                }

                BasvuruSonaErdi(true, false, null);

            }



        }
        void BasvuruFormuIndir687()
        {
            int i = 0;

            while (i < IslemYapilacaklar687.Count)
            {
                IptalKontrolu();

                var kisi687 = IslemYapilacaklar687.ElementAt(i);


                if (!kisi687.Value)
                {

                    if (SuanYapilanKisi687.Key == null || !SuanYapilanKisi687.Key.Equals(kisi687.Key.Key))
                    {
                        BasvuruLogEkle(kisi687.Key.Key + " kişisini incelemeye başladı");
                    }

                    SuanYapilanKisi687 = kisi687.Key;

                    bool Eski687 = kisi687.Key.Value < BasvuruFormuIndirmeSabitleri.Yeni687;

                    string response = null;

                    bool sistemHatasi = false;

                    do
                    {
                        IptalKontrolu();


                        if (Eski687)
                        {
                            response = BasvuruWebClient.PostData("https://uyg.sgk.gov.tr/Sigortali_Tesvik_687_KHK/ActionMultiplexer?aid=I_687_KHK_ISL&islemturu=21", "form_tcno=" + kisi687.Key.Key);
                        }
                        else response = BasvuruWebClient.PostData("https://uyg.sgk.gov.tr/Sigortali_Tesvik_687_KHK/ActionMultiplexer?aid=I_687_KHK_ISLY&islemturu=21", "form_tcno=" + kisi687.Key.Key);

                        IptalKontrolu();

                        if (!response.Contains("687 KHK Sigortalı Tanımlama"))
                        {
                            BasvuruLogEkle("Geçerli sayfa bulunamadı. Tekrar denenecek");
                        }

                        if (response.Contains("İşlemleriniz Sistem Tarafından Sonlandırıldı"))
                        {
                            sistemHatasi = true;

                            break;
                        }

                        if (response.ToLower().Contains("error")
                                || (Regex.IsMatch(response.ToLower(), ".*hata[^a-z].*") && !response.ToLower().Contains("hatalı bildirge verilmemesi"))
                                || response.ToLower().Contains("fail")
                                || response.ToLower().Contains("communication")
                                || response.ToLower().Contains("bağlanılamadı")
                                || response.ToLower().Contains("bağlantı")
                            )
                        {
                            BasarisizDenemeler++;

                            BasvuruLogEkle(SuanYapilanKisi687.Key + " kişisi hatadan dolayı " + BasarisizDenemeler + ".kez sorgulanıyor");

                        }
                        else
                        {
                            BasarisizDenemeler = 0;

                            break;
                        }

                        Thread.Sleep(1000);

                    }
                    while (BasarisizDenemeler <= maksimumDenemeSayisi_6645_687);

                    IptalKontrolu();


                    if (BasarisizDenemeler > maksimumDenemeSayisi_6645_687)
                    {
                        BasvuruLogEkle(SuanYapilanKisi687.Key + " kişisi " + maksimumDenemeSayisi_6645_687 + " denemeye rağmen hata nedeniyle sorgulanamadı. Sıradaki kişiye geçilecek");
                    }
                    else if (sistemHatasi)
                    {
                        BasvuruLogEkle(SuanYapilanKisi687.Key + " kişisi sistem hatasından dolayı sorgulanamadı. Sıradaki kişiye geçilecek");
                    }
                    else
                    {

                        HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                        html.LoadHtml(response);

                        var kaydet = html.DocumentNode.SelectNodes("//a").FirstOrDefault(p => p.InnerText.Equals("KAYDET"));

                        if (kaydet != null)
                        {
                            sistemHatasi = false;

                            do
                            {
                                IptalKontrolu();


                                if (Eski687)
                                {
                                    response = BasvuruWebClient.PostData("https://uyg.sgk.gov.tr/Sigortali_Tesvik_687_KHK/ActionMultiplexer?aid=I_687_KHK_ISL&islemturu=3", "form_tcno=" + kisi687.Key.Key + "&form_sskno=&form_ad=&form_soyad=&form_ad=&kayitli_tcno=");
                                }
                                else response = BasvuruWebClient.PostData("https://uyg.sgk.gov.tr/Sigortali_Tesvik_687_KHK/ActionMultiplexer?aid=I_687_KHK_ISLY&islemturu=3", "form_tcno=" + kisi687.Key.Key + "&form_sskno=&form_ad=&form_soyad=&form_ad=&kayitli_tcno=");


                                IptalKontrolu();

                                if (!response.Contains("687 KHK Sigortalı Tanımlama"))
                                {
                                    BasvuruLogEkle("Geçerli sayfa bulunamadı. Tekrar denenecek");
                                }

                                if (response.Contains("İşlemleriniz Sistem Tarafından Sonlandırıldı"))
                                {
                                    sistemHatasi = true;

                                    break;
                                }


                                if (response.ToLower().Contains("error")
                                        || (response.ToLower().Contains("hata") && !response.ToLower().Contains("hatalı bildirge verilmemesi"))
                                        || response.ToLower().Contains("fail")
                                        || response.ToLower().Contains("communication")
                                        || response.ToLower().Contains("bağlanılamadı")
                                        || response.ToLower().Contains("bağlantı")
                                    )
                                {
                                    BasarisizDenemeler++;

                                    BasvuruLogEkle(SuanYapilanKisi687.Key + " kişisi hatadan dolayı " + BasarisizDenemeler + ".kez kaydedilmeye çalışılıyor");
                                }
                                else
                                {
                                    BasarisizDenemeler = 0;

                                    break;
                                }

                                Thread.Sleep(1000);

                            }
                            while (BasarisizDenemeler <= maksimumDenemeSayisi_6645_687);

                            IptalKontrolu();

                            if (BasarisizDenemeler > maksimumDenemeSayisi_6645_687)
                            {
                                BasvuruLogEkle(SuanYapilanKisi687.Key + " kişisi " + maksimumDenemeSayisi_6645_687 + " denemeye rağmen hata nedeniyle kaydedilemedi. Sıradaki kişiye geçilecek");
                            }
                            else if (sistemHatasi)
                            {
                                BasvuruLogEkle(SuanYapilanKisi687.Key + " kişisi sistem hatasından dolayı kaydedilemedi. Sıradaki kişiye geçilecek");
                            }
                            else
                            {
                                if (response.Contains("KAYDETME İŞLEMİNİZ BAŞARI İLE GERÇEKLEŞTİRİLMİŞTİR"))
                                {
                                    YeniIslemYapilanlar687.Add(new KeyValuePair<string, DateTime>(SuanYapilanKisi687.Key, SuanYapilanKisi687.Value));

                                    BasvuruLogEkle(SuanYapilanKisi687.Key + " kişisi onaylanarak " + (Eski687 ? "687" : "1687") + " başvuru listesine eklendi");
                                }
                                else if (response.Contains("SİGORTALI TANIMLAMASI MEVCUTTUR"))
                                {
                                    BasvuruLogEkle(SuanYapilanKisi687.Key + " kişi " + (Eski687 ? "687" : "1687") + " teşviği için daha önceden onaylanmış");
                                }
                            }

                            IslemYapilacaklar687[SuanYapilanKisi687] = true;

                        }
                        else
                        {
                            if (response.Contains("SİGORTALI TANIMLAMASI MEVCUTTUR"))
                            {
                                BasvuruLogEkle(SuanYapilanKisi687.Key + " kişi " + (Eski687 ? "687" : "1687") + " teşviği için daha önceden onaylanmış");
                            }
                            else
                            {
                                BasvuruLogEkle(SuanYapilanKisi687.Key + " kişisi incelendi. " + (Eski687 ? "687" : "1687") + " teşviği verilmeyecek.");
                            }

                            IslemYapilacaklar687[SuanYapilanKisi687] = true;

                        }
                    }

                }

                i++;
            }

            IptalKontrolu();

            string responsetumkayitlar = null;

            BasarisizDenemeler = 0;

            do
            {
                IptalKontrolu();

                responsetumkayitlar = BasvuruWebClient.PostData("https://uyg.sgk.gov.tr/Sigortali_Tesvik_687_KHK/ActionMultiplexer?aid=I_687_KHK_ISL&islemturu=5", "onayDurumuKodu=&onayDurumuKodu2=&orderSelection=1&kayitli_tcno=");

                //if (responsetumkayitlar.Contains("İşlemleriniz Sistem Tarafından Sonlandırıldı"))
                //{
                //    BasvuruWebClient.ReConnect();

                //    continue;
                //}

                if (!responsetumkayitlar.Contains("687 KHK Sigortalı Tanımlama"))
                {
                    BasvuruLogEkle("Geçerli sayfa bulunamadı. Tekrar denenecek");

                    BasarisizDenemeler++;
                }
                else break;

                Thread.Sleep(1000);
            }
            while (BasarisizDenemeler <= maksimumDenemeSayisi_6645_687);

            if (BasarisizDenemeler > maksimumDenemeSayisi_6645_687)
            {
                BasvuruLogEkle(BasarisizDenemeler.ToString() + " denemeye rağmen tüm kayıtlar çekilemedi. Başvuru indirme işlemi sonlandırılıyor");

                BasvuruSonaErdi(false, true, "687 tüm kayıtlar sayfası yüklenemedi");

            }
            else
            {
                BasvuruLogEkle("Tüm kişilerin incelenmesi tamamlandı. Başvuru listesi çekiliyor");

                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();

                html.LoadHtml(responsetumkayitlar);

                var table = html.GetElementbyId("thkkkTbl");

                if (table != null)
                {
                    var trs = table.Descendants("tr");

                    foreach (var tr in trs)
                    {
                        var tds = tr.Descendants("td").ToList();

                        if (tds.Count > 2 && tds[2].InnerText != null && long.TryParse(tds[2].InnerText.Trim(), out long tc))
                        {
                            BasvuruKisiDownload687 basvurukisi = new BasvuruKisiDownload687();

                            basvurukisi.TcKimlikNo = tds[2].InnerText.Trim();

                            basvurukisi.Ad = tds[3].InnerText != null ? tds[3].InnerText.Trim() : "";

                            basvurukisi.Soyad = tds[4].InnerText != null ? tds[4].InnerText.Trim() : "";

                            basvurukisi.TesvikSuresiBaslangic = tds[5].InnerText != null ? tds[5].InnerText.Trim() : "";

                            basvurukisi.TesvikSuresiBitis = tds[6].InnerText != null ? tds[6].InnerText.Trim() : "";

                            basvurukisi.GirisTarihi = tds[7].InnerText != null ? tds[7].InnerText.Trim() : "";

                            basvurukisi.Baz = tds[8].InnerText != null ? Convert.ToInt32(tds[8].InnerText.Trim()) : 0;

                            basvurukisi.Aktif = tds[9].InnerText != null ? tds[9].InnerText.Trim() : "";

                            basvurukisi.KanunNo = Convert.ToDateTime(basvurukisi.GirisTarihi) < BasvuruFormuIndirmeSabitleri.Yeni687 ? "687" : "1687";

                            //basvurukisi.KanunNo = tds[13].InnerText != null ? tds[13].InnerText.Trim() : "";

                            foreach (var item in YeniIslemYapilanlar687)
                            {
                                if (item.Key == basvurukisi.TcKimlikNo)
                                {
                                    basvurukisi.YeniIslemYapildi = true;

                                    break;
                                }
                            }

                            basvurukisiler687.Add(basvurukisi);


                        }
                    }
                }

                BasvuruSonaErdi(true, false, null);
            }
        }
        void BasvuruFormuIndir14857()
        {

            bool sonAyIseGirenlerDolasiliyor = false;

        BasaDon:

            int i = 0;

            while (i < IslemYapilacaklar14857.Count)
            {
                IptalKontrolu();

                var kisi14857 = IslemYapilacaklar14857.ElementAt(i);

                if (!kisi14857.Value)
                {

                    if (SuanYapilanKisi14857.Key == null || !SuanYapilanKisi14857.Key.Equals(kisi14857.Key.Key))
                    {
                        BasvuruLogEkle(kisi14857.Key.Key + " kişisi incelenmeye başlandı (" + (i + 1).ToString() + "/" + IslemYapilacaklar14857.Count + ")");
                    }

                    SuanYapilanKisi14857 = kisi14857.Key;

                    string response = null;

                    do
                    {
                        IptalKontrolu();

                        response = BasvuruWebClient.PostData("https://uyg.sgk.gov.tr/Sigortali_Tesvik_4a/ActionMultiplexer?aid=IT_OZR_SIG_ISL&islemturu=3", "form_tcno=" + kisi14857.Key.Key + "&form_sskno=&form_ad=&form_soyad=&form_ad=&kayitli_tcno=");

                        IptalKontrolu();

                        if (!response.Contains("Engelli Teşvik Yönetimi"))
                        {
                            BasvuruLogEkle("Geçerli sayfa bulunamadı. Tekrar denenecek");

                            continue;
                        }


                        if (response.ToLower().Contains("error")
                                || (response.ToLower().Contains("hata") && !response.ToLower().Contains("hatalı bildirge verilmemesi"))
                                || response.ToLower().Contains("fail")
                                || response.ToLower().Contains("communication")
                                || response.ToLower().Contains("bağlanılamadı")
                                || response.ToLower().Contains("bağlantı")
                            )
                        {

                            File.WriteAllText(System.IO.Path.Combine(Application.StartupPath, "hata14857.txt"), response);

                            if (!response.Contains("constraints"))
                            {

                                BasarisizDenemeler++;

                                if (BasarisizDenemeler <= maksimumDenemeSayisi_6645_687)
                                {
                                    BasvuruLogEkle(SuanYapilanKisi14857.Key + " kişisi hatadan dolayı " + BasarisizDenemeler + ".kez sorgulanıyor");

                                    if (BasarisizDenemeler >= 3)
                                    {
                                        if (response.Contains("İŞKUR Web Servis Sisteminde Hata Meydana Geldi"))
                                        {
                                            if (!IskurHatasindanDolayiYenidenGirisYapildi)
                                            {
                                                BasvuruLogEkle(SuanYapilanKisi14857.Key + " kişisi İŞKUR web servis hatasından dolayı " + BasarisizDenemeler + ".kez sorgulanamadı. Yeniden giriş yapılacak");

                                                BasarisizDenemeler = 0;

                                                BasvuruWebClient.ReConnect();

                                                IskurHatasindanDolayiYenidenGirisYapildi = true;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                BasarisizDenemeler = 0;

                                break;
                            }
                        }
                        else
                        {
                            BasarisizDenemeler = 0;

                            break;
                        }


                        Thread.Sleep(2000);

                    }
                    while (BasarisizDenemeler <= maksimumDenemeSayisi_6645_687);

                    IptalKontrolu();

                    if (BasarisizDenemeler > maksimumDenemeSayisi_6645_687)
                    {
                        BasvuruLogEkle(SuanYapilanKisi14857.Key + " kişisi " + maksimumDenemeSayisi_6645_687 + " denemeye rağmen hata nedeniyle sorgulanamadı. Sıradaki kişiye geçilecek");

                        BasarisizDenemeler = 0;
                    }
                    else
                    {
                        BasarisizDenemeler = 0;

                        HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                        html.LoadHtml(response);

                        var kaydet = html.DocumentNode.SelectNodes("//a").FirstOrDefault(p => p.InnerText.Equals("KAYDET"));

                        if (kaydet != null)
                        {
                            do
                            {
                                IptalKontrolu();

                                var ssknoinput = html.GetElementbyId("form_sskno");
                                var adinput = html.GetElementbyId("form_ad");
                                var soyadinput = html.GetElementbyId("form_soyad");

                                string sskno = string.Empty;
                                string ad = string.Empty;
                                string soyad = string.Empty;

                                if (ssknoinput != null) sskno = System.Net.WebUtility.UrlEncode(ssknoinput.GetAttributeValue("value", ""));
                                if (adinput != null) ad = System.Net.WebUtility.UrlEncode(adinput.GetAttributeValue("value", ""));
                                if (soyadinput != null) soyad = System.Net.WebUtility.UrlEncode(soyadinput.GetAttributeValue("value", ""));

                                response = BasvuruWebClient.PostData("https://uyg.sgk.gov.tr/Sigortali_Tesvik_4a/ActionMultiplexer?aid=IT_OZR_SIG_ISL&islemturu=1", "form_tcno=" + kisi14857.Key.Key + "&form_sskno=" + sskno + "&form_ad=" + ad + "&form_soyad=" + soyad);

                                IptalKontrolu();

                                if (!response.Contains("Engelli Teşvik Yönetimi"))
                                {
                                    BasvuruLogEkle("Geçerli sayfa bulunamadı. Tekrar denenecek");
                                }


                                if (response.ToLower().Contains("error")
                                        || (response.ToLower().Contains("hata") && !response.ToLower().Contains("hatalı bildirge verilmemesi"))
                                        || response.ToLower().Contains("fail")
                                        || response.ToLower().Contains("communication")
                                        || response.ToLower().Contains("bağlanılamadı")
                                        || response.ToLower().Contains("bağlantı")
                                    )
                                {
                                    if (!response.Contains("constraints"))
                                    {

                                        BasarisizDenemeler++;

                                        if (BasarisizDenemeler <= maksimumDenemeSayisi_6645_687)
                                        {
                                            BasvuruLogEkle(SuanYapilanKisi14857.Key + " kişisi hatadan dolayı " + BasarisizDenemeler + ".kez kaydedilmeye çalışılıyor");
                                        }
                                    }
                                    else
                                    {

                                        BasarisizDenemeler = 0;

                                        break;
                                    }
                                }
                                else
                                {
                                    BasarisizDenemeler = 0;

                                    break;
                                }

                                Thread.Sleep(1000);

                            }
                            while (BasarisizDenemeler <= maksimumDenemeSayisi_6645_687);

                            IptalKontrolu();

                            if (BasarisizDenemeler > maksimumDenemeSayisi_6645_687)
                            {
                                BasvuruLogEkle(SuanYapilanKisi14857.Key + " kişisi " + maksimumDenemeSayisi_6645_687 + " denemeye rağmen hata nedeniyle kaydedilemedi. Sıradaki kişiye geçilecek");

                                BasarisizDenemeler = 0;

                            }
                            else
                            {
                                if (response.Contains("SİGORTALIYI KAYDETME İŞLEMİNİZ BAŞARIYLA GERÇEKLEŞTİRİLMİŞTİR"))
                                {
                                    YeniIslemYapilanlar14857.Add(new KeyValuePair<string, DateTime>(SuanYapilanKisi14857.Key, SuanYapilanKisi14857.Value));

                                    BasvuruLogEkle(SuanYapilanKisi14857.Key + " kişisi onaylanarak 14857 başvuru listesine eklendi");
                                }
                                else if (response.Contains("HERHANGi BİR KAYIT İŞLEMİ GERÇEKLEŞTİRİLMEMİŞTİR. LÜTFEN KAYDETMEK İSTEDİĞİNİZ BELGELER İLE AŞAĞIDA LİSTELENEN VE KAYITLI OLAN BELGELERİ KARŞILAŞTIRINIZ"))
                                {
                                    BasvuruLogEkle(SuanYapilanKisi14857.Key + " kişisi 14857 teşviği için daha önceden onaylanmış");
                                }
                            }

                            IslemYapilacaklar14857[SuanYapilanKisi14857] = true;

                        }
                        else
                        {
                            if (response.Contains("constraints"))
                            {
                                BasvuruLogEkle(SuanYapilanKisi14857.Key + " kişisi kişiye özel İşkur hatasından dolayı atlanıyor.Lütfen daha sonra tekrar deneyiniz veya manuel olarak kontrol ediniz");
                            }
                            else if (response.Contains("SORGULANAN SİGORTALI İÇİN BU TEŞVİKTEN YARARLANILMAKTADIR. TEKRAR KAYIT YAPILAMAZ"))
                            {
                                BasvuruLogEkle(SuanYapilanKisi14857.Key + " kişi 14857 teşviği için daha önceden onaylanmış");
                            }
                            else
                            {
                                BasvuruLogEkle(SuanYapilanKisi14857.Key + " kişisi incelendi. 14857 teşviği verilmeyecek.");
                            }

                            IslemYapilacaklar14857[SuanYapilanKisi14857] = true;
                        }
                    }

                }

                i++;
            }

            if (!sonAyIseGirenlerDolasiliyor)
            {

                if (indirilenIsyeri.BasvuruFormuIndirmeleri.Any(p =>
                    p.bfsira == Enums.BasvuruFormuTurleri.Bf6111
                    ||
                    p.bfsira == Enums.BasvuruFormuTurleri.Bf7103
                    ||
                    p.bfsira == Enums.BasvuruFormuTurleri.Bf2828
                    ||
                    p.bfsira == Enums.BasvuruFormuTurleri.Bf7252
                    ||
                    p.bfsira == Enums.BasvuruFormuTurleri.Bf7256
                    ||
                    p.bfsira == Enums.BasvuruFormuTurleri.Bf7316
                    ||
                    p.bfsira == Enums.BasvuruFormuTurleri.Bf3294
                    ||
                    p.bfsira == Enums.BasvuruFormuTurleri.BfTumu
                    ))
                {

                    BasvuruLogEkle("Aphbdeki kişilerin sorgulanması tamamlandı. İşveren sisteminde son ay işe girenlerin sorgulanmasına başlanacak.");

                    while (!indirilenIsyeri.SonAyIseGirenlerCekildi && !indirilenIsyeri.DigerBasvuruIndirmeleriBittiMi(this))
                    {
                        IptalKontrolu();

                        Thread.Sleep(5000);
                    }

                    if (indirilenIsyeri.SonAyIseGirenlerListesi.Count > 0)
                    {
                        IslemYapilacaklar14857.Clear();

                        indirilenIsyeri.SonAyIseGirenlerListesi.ForEach(p =>
                        {
                            var kv = new KeyValuePair<string, DateTime>(p, DateTime.MinValue);

                            if (!IslemYapilacaklar14857.ContainsKey(kv))
                            {
                                IslemYapilacaklar14857.Add(kv, false);
                            }
                        });

                        BasvuruLogEkle("İşveren sisteminde son ay işe girenlerin listesi çekildi");

                        sonAyIseGirenlerDolasiliyor = true;

                        goto BasaDon;
                    }
                    else
                    {
                        BasvuruLogEkle("İşveren sisteminde son ay işe girenlerin listesi boş veya liste çekilemedi. Son aya işe girenler sorgulanmadan başvuru formu listesinin tamamı çekilecek.");
                    }
                }
            }

            string responsetumkayitlar = null;

            BasarisizDenemeler = 0;

            do
            {
                IptalKontrolu();

                responsetumkayitlar = BasvuruWebClient.Get("https://uyg.sgk.gov.tr/Sigortali_Tesvik_4a/ActionMultiplexer?aid=IT_OZR_SIG_ISL&islemturu=5", "");

                IptalKontrolu();

                if (!responsetumkayitlar.Contains("Engelli Teşvik Yönetimi"))
                {
                    BasvuruLogEkle("Geçerli sayfa bulunamadı. Tekrar denenecek");

                    BasarisizDenemeler++;
                }
                else break;

                Thread.Sleep(1000);
            }
            while (BasarisizDenemeler <= maksimumDenemeSayisi_6645_687);

            IptalKontrolu();


            if (BasarisizDenemeler > maksimumDenemeSayisi_6645_687)
            {
                BasvuruLogEkle((BasarisizDenemeler - 1).ToString() + " denemeye rağmen tüm kayıtlar çekilemedi. Başvuru indirme işlemi sonlandırılıyor");

                BasvuruSonaErdi(false, true, "14857 tüm kayıtlar sayfası yüklenemedi");

            }
            else
            {
                BasvuruLogEkle("Tüm kişilerin incelenmesi tamamlandı. Başvuru listesi çekiliyor");

                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();

                html.LoadHtml(responsetumkayitlar);

                var table = html.GetElementbyId("thkkkTbl");

                if (table != null)
                {
                    var trs = table.Descendants("tr");

                    foreach (var tr in trs)
                    {
                        var tds = tr.Descendants("td").ToList();

                        if (tds.Count > 1 && tds[0].InnerText != null && long.TryParse(tds[0].InnerText.Trim(), out long tc))
                        {
                            BasvuruKisiDownload14857 basvurukisi = new BasvuruKisiDownload14857();

                            basvurukisi.TcKimlikNo = tds[0].InnerText.Trim();

                            basvurukisi.Sicil = tds[1].InnerText != null ? tds[1].InnerText.Trim() : "";

                            basvurukisi.Ad = tds[2].InnerText != null ? tds[2].InnerText.Trim() : "";

                            basvurukisi.Soyad = tds[3].InnerText != null ? tds[3].InnerText.Trim() : "";

                            basvurukisi.TesvikSuresiBaslangic = tds[4].InnerText != null ? tds[4].InnerText.Trim() : "";

                            var bitis = "";

                            if (tds[5].InnerText != null && !tds[5].InnerText.Trim().Equals("- / -"))
                            {
                                if (!tds[5].InnerText.Trim().Contains("/"))
                                {
                                    bitis = String.Join(" / ", tds[5].InnerText.Trim().Split(' ').Select(p => p.Trim()));
                                }
                                else bitis = tds[5].InnerText.Trim();
                            }

                            basvurukisi.TesvikSuresiBitis = bitis;

                            basvurukisi.RaporNo = tds[6].InnerText != null ? tds[6].InnerText.Trim() : "";

                            basvurukisi.OzurOrani = tds[7].InnerText != null ? tds[7].InnerText.Trim() : "";

                            foreach (var item in YeniIslemYapilanlar14857)
                            {
                                if (item.Key == basvurukisi.TcKimlikNo)
                                {
                                    basvurukisi.YeniIslemYapildi = true;

                                    break;
                                }
                            }

                            basvurukisiler14857.Add(basvurukisi);


                        }
                    }
                }

                BasvuruSonaErdi(true, false, null);
            }
        }

        HtmlAgilityPack.HtmlNode DonemBul(List<HtmlAgilityPack.HtmlNode> options)
        {
            var donemSayisi = options.Count();

            for (int donemindex = 0; donemindex < donemSayisi; donemindex++)
            {
                bool incelenecekAy = false;

                var option = options.ElementAt(donemindex);

                if (bfsira == Enums.BasvuruFormuTurleri.Bf6111)
                {
                    incelenecekAy = incelenecekDonemler.Contains(option.InnerText);
                }
                else if (bfsira == Enums.BasvuruFormuTurleri.Bf7103)
                {
                    incelenecekAy = incelenecekDonemler7103.Contains(option.InnerText);
                }
                else if (bfsira == Enums.BasvuruFormuTurleri.Bf2828)
                {
                    incelenecekAy = incelenecekDonemler2828.Contains(option.InnerText);

                }
                else if (bfsira == Enums.BasvuruFormuTurleri.Bf7252)
                {
                    incelenecekAy = incelenecekDonemler7252.Contains(option.InnerText);
                }
                else if (bfsira == Enums.BasvuruFormuTurleri.Bf7256)
                {
                    incelenecekAy = incelenecekDonemler7256.Contains(option.InnerText);
                }
                else if (bfsira == Enums.BasvuruFormuTurleri.Bf7316)
                {
                    incelenecekAy = incelenecekDonemler7316.Contains(option.InnerText);
                }
                else if (bfsira == Enums.BasvuruFormuTurleri.Bf3294)
                {
                    incelenecekAy = incelenecekDonemler3294.Contains(option.InnerText);
                }
                else if (bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                {

                    incelenecekAy = incelenecekDonemler.Contains(option.InnerText)
                                || incelenecekDonemler7103.Contains(option.InnerText)
                                || incelenecekDonemler2828.Contains(option.InnerText)
                                || incelenecekDonemler7252.Contains(option.InnerText)
                                || incelenecekDonemler7256.Contains(option.InnerText)
                                || incelenecekDonemler7316.Contains(option.InnerText)
                                || incelenecekDonemler3294.Contains(option.InnerText);
                }

                if (incelenecekAy)
                {

                    lock (islemiTamamlananDonemler)
                    {
                        if (!islemiTamamlananDonemler.Contains(option.InnerText))
                        {
                            lock (islemYapilanDonemler)
                            {
                                if (!islemYapilanDonemler.Contains(option.InnerText))
                                {
                                    islemYapilanDonemler.Add(option.InnerText);

                                    return option;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }
        HtmlAgilityPack.HtmlNode KisiBul(List<HtmlAgilityPack.HtmlNode> satirlar)
        {

            for (int i = 1; i < satirlar.Count - 1; i++)
            {
                var satir = satirlar.ElementAt(i);

                var tcno = satir.SelectSingleNode("td[3]/p/text()").GetInnerText();

                var iseGirisTarihi = Convert.ToDateTime(satir.SelectSingleNode("td[8]/p/text()").GetInnerText());

                var tcNoveIseGirisTarihi = new KeyValuePair<string, DateTime>(tcno, iseGirisTarihi);

                lock (islemiTamamlananKisiler)
                {
                    if (!islemiTamamlananKisiler.Contains(tcNoveIseGirisTarihi))
                    {
                        lock (islemYapilanKisiler)
                        {
                            if (!islemYapilanKisiler.Contains(tcNoveIseGirisTarihi))
                            {
                                islemYapilanKisiler.Add(tcNoveIseGirisTarihi);

                                return satir;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public void YuklenenSayfaninIciniDolas(string ResponseHtml)
        {
            IptalKontrolu();

            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();

            string Mesaj = "";

            var webclient = BasvuruWebClient;

            html.LoadHtml(ResponseHtml);

            bool gecerliSayfaBulundu = GecerliSayfaOlupOlmadiginiKontrolEt(ResponseHtml, siradakiIslem, ref webclient) == Enums.GecerliSayfaSonuclari.Gecerli;

            if (!gecerliSayfaBulundu) return;

            try
            {
                #region SİGORTALI TANIMLAMA AŞAMALARI;                

                if (siradakiIslem.Equals("Uygulama Başlayacak"))
                {
                    if (ResponseHtml.Contains("UYGULAMA BAŞLATILDI"))
                    {
                        Mesaj = "Uygulama Başlatıldı";

                        BasvuruLogEkle(Mesaj);

                        stopwatch.Start();
                        siradakiIslem = "Teşvik Tanımlama Açılacak";

                        Metodlar.DetayliLogYaz("Teşvik tanımlama sayfası açılacak");

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvikTanimlama.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                }

                if (siradakiIslem.Equals("Teşvik Tanımlama Açılacak"))
                {

                    var pencereLinkIdYeni = html.GetElementbyId("pencereLinkIdYeni");
                    if (pencereLinkIdYeni != null)
                    {

                        siradakiIslem = "Dönem Seçilecek";
                        string newUrl = pencereLinkIdYeni.Attributes["src"].Value;

                        string response = webclient.Get(newUrl, string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                }

                if (siradakiIslem.Equals("Dönem Seçilecek"))
                {
                    Metodlar.DetayliLogYaz("Teşvik tanımlama sayfası açıldı");

                    var dropdown = html.GetElementbyId("tesvikTanimlama_donem_yil_ay_index");

                    if (dropdown != null)
                    {
                        var options = dropdown.SelectNodes("option").ToList();

                        options.RemoveAt(0);

                        options = options.OrderByDescending(o => Convert.ToDateTime(o.InnerText)).ToList();

                        var donemSayisi = options.Count();

                        if (bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                        {
                            if (this.EnBastanTumu)
                            {
                                var donemler = options.Select(p => Convert.ToDateTime(p.InnerText)).Where(p => p <= this.dtBitis6111).Select(date => string.Format("{0}/{1}", date.Year, date.ToString("MM"))).ToList();

                                incelenecekDonemler.Clear();
                                incelenecekDonemler.AddRange(donemler);

                                incelenecekDonemler7103.Clear();
                                incelenecekDonemler7103.AddRange(donemler);

                                incelenecekDonemler2828.Clear();
                                incelenecekDonemler2828.AddRange(donemler);

                                incelenecekDonemler7252.Clear();
                                incelenecekDonemler7252.AddRange(donemler);

                                incelenecekDonemler7256.Clear();
                                incelenecekDonemler7256.AddRange(donemler);

                                incelenecekDonemler7316.Clear();
                                incelenecekDonemler7316.AddRange(donemler);

                                incelenecekDonemler3294.Clear();
                                incelenecekDonemler3294.AddRange(donemler);
                            }
                        }
                        else if (bfsira == Enums.BasvuruFormuTurleri.Bf6111)
                        {
                            if (this.EnBastan6111)
                            {
                                var donemler = options.Select(p => Convert.ToDateTime(p.InnerText)).Where(p => p <= this.dtBitis6111).Select(date => string.Format("{0}/{1}", date.Year, date.ToString("MM"))).ToList();

                                incelenecekDonemler.Clear();
                                incelenecekDonemler.AddRange(donemler);
                            }
                        }
                        else if (bfsira == Enums.BasvuruFormuTurleri.Bf7103)
                        {
                            if (this.EnBastan7103)
                            {
                                var donemler = options.Select(p => Convert.ToDateTime(p.InnerText)).Where(p => p <= this.dtBitis7103).Select(date => string.Format("{0}/{1}", date.Year, date.ToString("MM"))).ToList();

                                incelenecekDonemler7103.Clear();
                                incelenecekDonemler7103.AddRange(donemler);
                            }
                        }
                        else if (bfsira == Enums.BasvuruFormuTurleri.Bf2828)
                        {
                            if (this.EnBastan2828)
                            {
                                var donemler = options.Select(p => Convert.ToDateTime(p.InnerText)).Where(p => p <= this.dtBitis2828).Select(date => string.Format("{0}/{1}", date.Year, date.ToString("MM"))).ToList();

                                incelenecekDonemler2828.Clear();
                                incelenecekDonemler2828.AddRange(donemler);
                            }
                        }
                        else if (bfsira == Enums.BasvuruFormuTurleri.Bf7252)
                        {
                            if (this.EnBastan7252)
                            {
                                var donemler = options.Select(p => Convert.ToDateTime(p.InnerText)).Where(p => p <= this.dtBitis7252).Select(date => string.Format("{0}/{1}", date.Year, date.ToString("MM"))).ToList();

                                incelenecekDonemler7252.Clear();
                                incelenecekDonemler7252.AddRange(donemler);
                            }
                        }
                        else if (bfsira == Enums.BasvuruFormuTurleri.Bf7256)
                        {
                            if (this.EnBastan7256)
                            {
                                var donemler = options.Select(p => Convert.ToDateTime(p.InnerText)).Where(p => p <= this.dtBitis7256).Select(date => string.Format("{0}/{1}", date.Year, date.ToString("MM"))).ToList();

                                incelenecekDonemler7256.Clear();
                                incelenecekDonemler7256.AddRange(donemler);
                            }
                        }
                        else if (bfsira == Enums.BasvuruFormuTurleri.Bf7316)
                        {
                            if (this.EnBastan7316)
                            {
                                var donemler = options.Select(p => Convert.ToDateTime(p.InnerText)).Where(p => p <= this.dtBitis7316).Select(date => string.Format("{0}/{1}", date.Year, date.ToString("MM"))).ToList();

                                incelenecekDonemler7316.Clear();
                                incelenecekDonemler7316.AddRange(donemler);
                            }
                        }
                        else if (bfsira == Enums.BasvuruFormuTurleri.Bf3294)
                        {
                            if (this.EnBastan3294)
                            {
                                var donemler = options.Select(p => Convert.ToDateTime(p.InnerText)).Where(p => p <= this.dtBitis3294).Select(date => string.Format("{0}/{1}", date.Year, date.ToString("MM"))).ToList();

                                incelenecekDonemler3294.Clear();
                                incelenecekDonemler3294.AddRange(donemler);
                            }
                        }

                        var incelenecekler = bfsira == Enums.BasvuruFormuTurleri.Bf6111 ? incelenecekDonemler : bfsira == Enums.BasvuruFormuTurleri.Bf7103 ? incelenecekDonemler7103 : bfsira == Enums.BasvuruFormuTurleri.Bf2828 ? incelenecekDonemler2828 : bfsira == Enums.BasvuruFormuTurleri.Bf7252 ? incelenecekDonemler7252 : bfsira == Enums.BasvuruFormuTurleri.Bf7256 ? incelenecekDonemler7256 : bfsira == Enums.BasvuruFormuTurleri.Bf7316 ? incelenecekDonemler7316 : bfsira == Enums.BasvuruFormuTurleri.Bf3294 ? incelenecekDonemler3294 : incelenecekDonemler;

                        var maxDegreeOfParallelismdonem = ListedeBulunmayanKisileriTekrarDenemeSayisi > 0 ? 1 : Math.Min(Math.Min(options.Count(), Program.DonemIslemciSayisi), incelenecekler.Count);

                        islemYapilanDonemler.Clear();
                        islemYapilanKisiler.Clear();

                        bool YenidenBaglaniliyor = false;

                        List<Task> donemTasks = new List<Task>();
                        List<ProjeGiris> allKisiWebClients = new List<ProjeGiris>();

                        bool donemIslemciYeniGirisYapilsin = Program.DonemIslemcisiYeniGirisYapsin;

                        if (ListedeBulunmayanKisileriTekrarDenemeSayisi > 0) donemIslemciYeniGirisYapilsin = false;

                        for (int i = 0; i < maxDegreeOfParallelismdonem; i++)
                        {
                            var task = Task.Factory.StartNew(() =>
                            {

                                bool TesvikTanimlamSayfasiAcilacakDonem = true;

                            DonemEnBasaDon:

                                var DonemWebClient = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.IsverenSistemi);

                                if (!donemIslemciYeniGirisYapilsin)
                                {
                                    DonemWebClient = webclient.Clone();
                                    //DonemWebClient.Connected = true;
                                    //DonemWebClient.Cookie = webclient.Cookie;
                                }

                                int sayac1 = 0;

                            TesvikTanimlamaSayfasiAc:

                                if (TesvikTanimlamSayfasiAcilacakDonem)
                                {
                                    TesvikTanimlamSayfasiAcilacakDonem = false;

                                    var rsp = DonemWebClient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvikTanimlama.action;", string.Empty);

                                    if (rsp != null && rsp.Contains("id=\"pencereLinkIdYeni\"") && rsp.Contains("src=\"https://uyg.sgk.gov.tr/YeniSistem/Isveren/tesvikTanimlama.action;"))
                                    {

                                        HtmlAgilityPack.HtmlDocument htmlrsp = new HtmlAgilityPack.HtmlDocument();
                                        htmlrsp.LoadHtml(rsp);

                                        var pencereLinkIdYeni = htmlrsp.GetElementbyId("pencereLinkIdYeni");
                                        if (pencereLinkIdYeni != null)
                                        {
                                            string newUrl = pencereLinkIdYeni.Attributes["src"].Value;

                                            DonemWebClient.Get(newUrl, string.Empty);
                                        }
                                    }
                                    else
                                    {
                                        if (!rsp.Contains("Parent was not null"))
                                        {
                                            sayac1++;

                                            if (sayac1 < 3)
                                            {
                                                goto TesvikTanimlamaSayfasiAc;
                                            }
                                        }
                                    }
                                }

                                try
                                {
                                    while (true)
                                    {

                                        HtmlAgilityPack.HtmlNode option = DonemBul(options);

                                        IptalKontrolu();

                                        if (option != null)
                                        {
                                            int donemindex = options.IndexOf(option);

                                            if (ListedeBulunmayanKisileriTekrarDenemeSayisi > 0) donemindex = -1;

                                            var donem = option.InnerText;

                                            Mesaj = donem + " Dönemi Seçildi";

                                            BasvuruLogEkle(Mesaj, donemindex, -1, false);

                                            Metodlar.DetayliLogYaz(Mesaj);

                                            var optionvalue = option.GetAttributeValue("value", "");

                                            int donemsayac = 0;

                                            bool AyIcindeKisiVar = false;

                                            HashSet<KeyValuePair<string, DateTime>> kisilerveIseGirisTarihleri = new HashSet<KeyValuePair<string, DateTime>>();

                                        DonemKisileriGetir:

                                            string responsedonem = DonemWebClient.PostData("https://uyg.sgk.gov.tr/YeniSistem/Isveren/tesvikTanimlama.action", "iseGirisSirali=false&tcDonemSorgu=0&donem_yil_ay_index=" + optionvalue + "&action%3AuygunSigortaliBilgileriDonem=Sorgula&tcKimlikNo=0");

                                            var gecerliSayfaSorgusu = GecerliSayfaOlupOlmadiginiKontrolEt(responsedonem, "Sıra Seçilecek", ref webclient, false);

                                            if (gecerliSayfaSorgusu == Enums.GecerliSayfaSonuclari.Iptal) return;
                                            else if (gecerliSayfaSorgusu == Enums.GecerliSayfaSonuclari.Gecersiz)
                                            {
                                                donemsayac++;

                                                if (donemsayac < 20)
                                                {
                                                    Metodlar.DetayliLogYaz(donem + " ayı sorgulandıktan sonra geçerli bir sayfa bulunamadı." + (donemsayac + 1) + ".kez denenecek");

                                                    Thread.Sleep(1000);

                                                    goto DonemKisileriGetir;
                                                }
                                            }
                                            else if (gecerliSayfaSorgusu == Enums.GecerliSayfaSonuclari.UzunSureliIslemYapilamadiUyarisi)
                                            {
                                                if (!Program.DonemIslemcisiYeniGirisYapsin)
                                                {

                                                    while (YenidenBaglaniliyor) { Thread.Sleep(200); }

                                                    if (!YenidenBaglaniliyor)
                                                    {
                                                        YenidenBaglaniliyor = true;

                                                        lock (webclient)
                                                        {
                                                            if (webclient.oturumId.Equals(DonemWebClient.oturumId))
                                                            {
                                                                webclient.ReConnect();
                                                            }
                                                        }
                                                    }

                                                    YenidenBaglaniliyor = false;
                                                }
                                                else DonemWebClient.Disconnect();

                                                lock (islemYapilanDonemler)
                                                {
                                                    if (islemYapilanDonemler.Contains(donem))
                                                    {
                                                        islemYapilanDonemler.Remove(donem);
                                                    }
                                                }

                                                TesvikTanimlamSayfasiAcilacakDonem = true;

                                                goto DonemEnBasaDon;
                                            }
                                            if (gecerliSayfaSorgusu == Enums.GecerliSayfaSonuclari.Gecerli)
                                            {

                                                if (responsedonem != null && !responsedonem.Contains("İstenen Bilgilere Göre Herhangi Bir Kayıt gelmemiştir"))
                                                {

                                                    HtmlAgilityPack.HtmlDocument htmldonem = new HtmlAgilityPack.HtmlDocument();
                                                    htmldonem.LoadHtml(responsedonem);

                                                    var siraCheckboxlari = htmldonem.DocumentNode.SelectNodes("//input[@name='iseGirisMapIndex']");

                                                    if (siraCheckboxlari.Count() == 0)
                                                    {

                                                        BasvuruLogEkle(donem + " içinde kayıtlı kişi bulunamadı", donemindex, -1, false);

                                                        Metodlar.DetayliLogYaz(donem + " içinde kayıtlı kişi yok");

                                                        if (!indirilenIsyeri.SonAyIseGirenlerCekildi)
                                                        {
                                                            if (Convert.ToDateTime(donem).Equals(oncekiAy))
                                                            {
                                                                lock (indirilenIsyeri.SonAyIseGirenlerListesi)
                                                                {
                                                                    indirilenIsyeri.SonAyIseGirenlerCekildi = true;
                                                                    indirilenIsyeri.SonAyIseGirenlerListesi.Clear();
                                                                }
                                                            }
                                                        }

                                                    }
                                                    else if (siraCheckboxlari.Count() > 0)
                                                    {

                                                        AyIcindeKisiVar = true;

                                                        var satirlar = htmldonem.DocumentNode.SelectNodes("//table[@class='gradienttable']/tr").ToList();

                                                        if (!indirilenIsyeri.SonAyIseGirenlerCekildi)
                                                        {
                                                            if (Convert.ToDateTime(donem).Equals(oncekiAy))
                                                            {
                                                                var sonAyIseGirenler = new HashSet<string>();

                                                                for (int satir_index = 1; satir_index < satirlar.Count - 1; satir_index++)
                                                                {
                                                                    var satir = satirlar.ElementAt(satir_index);

                                                                    var tcno = satir.SelectSingleNode("td[3]/p/text()").GetInnerText();

                                                                    sonAyIseGirenler.Add(tcno);
                                                                }

                                                                lock (indirilenIsyeri.SonAyIseGirenlerListesi)
                                                                {
                                                                    indirilenIsyeri.SonAyIseGirenlerCekildi = true;
                                                                    indirilenIsyeri.SonAyIseGirenlerListesi = sonAyIseGirenler.ToList();
                                                                }
                                                            }
                                                        }



                                                        var kazancIdinput = htmldonem.GetElementbyId("kazancId");

                                                        var kazanc = "0";

                                                        if (kazancIdinput != null) kazanc = kazancIdinput.GetAttributeValue("value", "");

                                                        var kisiSayisi = siraCheckboxlari.Count();

                                                        var maxDegreeOfParallelism = ListedeBulunmayanKisileriTekrarDenemeSayisi > 0 ? 1 : Math.Min(siraCheckboxlari.Count(), Program.KisiIslemciSayisi);

                                                        List<Task> kisiTasks = new List<Task>();

                                                        bool kisiIslemciYeniGirisYapilsin = Program.KisiIslemcisiYeniGirisYapsin;

                                                        if (ListedeBulunmayanKisileriTekrarDenemeSayisi > 0)
                                                        {
                                                            kisiIslemciYeniGirisYapilsin = false;
                                                        }

                                                        for (int j = 0; j < maxDegreeOfParallelism; j++)
                                                        {
                                                            var taskKisi = Task.Factory.StartNew(() =>
                                                            {
                                                                bool TesvikTanimlamaSayfasiAcilacak = kisiIslemciYeniGirisYapilsin;

                                                            //bool YenidenBaglaniyorKisiIslem = false;

                                                            KisiEnBasaDon:

                                                                ProjeGiris kisiWebClient = null;

                                                                if (!kisiIslemciYeniGirisYapilsin)
                                                                {
                                                                    kisiWebClient = DonemWebClient.Clone();
                                                                    //kisiWebClient = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.IsverenSistemi);
                                                                    //kisiWebClient.Connected = true;
                                                                    //kisiWebClient.Cookie = DonemWebClient.Cookie;
                                                                    kisiWebClient.Kullanimda = true;
                                                                }
                                                                else
                                                                {
                                                                    lock (allKisiWebClients)
                                                                    {
                                                                        kisiWebClient = allKisiWebClients.FirstOrDefault(p => !p.Kullanimda);

                                                                        if (kisiWebClient == null)
                                                                        {
                                                                            kisiWebClient = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.IsverenSistemi);
                                                                            allKisiWebClients.Add(kisiWebClient);
                                                                        }

                                                                        kisiWebClient.Kullanimda = true;

                                                                    }
                                                                }

                                                                int sayac2 = 0;
                                                            TesvikTanimlamaSayfasiAc2:

                                                                if (TesvikTanimlamaSayfasiAcilacak)
                                                                {
                                                                    TesvikTanimlamaSayfasiAcilacak = false;

                                                                    var rsp2 = kisiWebClient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvikTanimlama.action;", string.Empty);

                                                                    if (rsp2 != null && rsp2.Contains("id=\"pencereLinkIdYeni\"") && rsp2.Contains("src=\"https://uyg.sgk.gov.tr/YeniSistem/Isveren/tesvikTanimlama.action;"))
                                                                    {

                                                                        HtmlAgilityPack.HtmlDocument htmlrsp = new HtmlAgilityPack.HtmlDocument();
                                                                        htmlrsp.LoadHtml(rsp2);

                                                                        var pencereLinkIdYeni = htmlrsp.GetElementbyId("pencereLinkIdYeni");
                                                                        if (pencereLinkIdYeni != null)
                                                                        {
                                                                            string newUrl = pencereLinkIdYeni.Attributes["src"].Value;

                                                                            kisiWebClient.Get(newUrl, string.Empty);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (!rsp2.Contains("Parent was not null"))
                                                                        {
                                                                            sayac2++;

                                                                            if (sayac2 < 3)
                                                                            {
                                                                                goto TesvikTanimlamaSayfasiAc2;
                                                                            }
                                                                        }
                                                                    }
                                                                }

                                                                try
                                                                {
                                                                    while (true)
                                                                    {
                                                                        var kisisatir = KisiBul(satirlar);
                                                                        if (kisisatir != null)
                                                                        {

                                                                            int sistemHatasiYenidenDenemeSayisi = 0;

                                                                        yenidenDene:

                                                                            IptalKontrolu();

                                                                            int kisiindex = satirlar.IndexOf(kisisatir) - 1;

                                                                            var kisiSira = kisiindex;

                                                                            if (ListedeBulunmayanKisileriTekrarDenemeSayisi > 0) kisiindex = -1;

                                                                            var tcno = kisisatir.SelectSingleNode("td[3]/p/text()").GetInnerText().Trim();

                                                                            var kimlikBulunamadiUyarisiVar = kisisatir.InnerText.Contains("KİMLİK") && kisisatir.InnerText.Contains("BULUNAMADI");

                                                                            var yabanciTcNoMu = tcno.StartsWith("99");

                                                                            var iseGirisTarihi = Convert.ToDateTime(kisisatir.SelectSingleNode("td[8]/p/text()").GetInnerText());

                                                                            var tcNoveIseGirisTarihi = new KeyValuePair<string, DateTime>(tcno, iseGirisTarihi);

                                                                            if (!kisilerveIseGirisTarihleri.Contains(tcNoveIseGirisTarihi)) kisilerveIseGirisTarihleri.Add(tcNoveIseGirisTarihi);

                                                                            Mesaj = tcno + " kişisinin " + iseGirisTarihi.ToString("dd.MM.yyyy") + " girişi inceleniyor";

                                                                            BasvuruLogEkle(Mesaj, donemindex, kisiindex, false);

                                                                            Metodlar.DetayliLogYaz(Mesaj);

                                                                            string iseGirisMapIndexValue = siraCheckboxlari.ElementAt(kisiSira).Attributes["value"].Value;

                                                                            var egitimDurumu = EgitimBelgesiAdlari.EgitimBelgesiTurleriAdlari[Program.BfEgitimBelgesi].Equals("Tümü") ? "9" : Program.BfEgitimBelgesi.ToString();

                                                                            if (ListedeBulunmayanKisileriTekrarDenemeSayisi == 4)
                                                                            {
                                                                                egitimDurumu = "1";
                                                                            }

                                                                            if (Program.BfEgitimBelgesi == 11) //7252-7256 seçiliyse ayarlarda 
                                                                            {
                                                                                if (donemindex > 1)
                                                                                {
                                                                                    egitimDurumu = "1";
                                                                                }
                                                                                else
                                                                                {
                                                                                    egitimDurumu = "9";
                                                                                }
                                                                            }



                                                                            bool UcretDestegiIstensinMi = UcretDestegiIstiyorSoruldu ? UcretDestegiIstiyor : Program.BfIndirmeUcretDestegiIstensin;

                                                                            bool IstenecekveyaIstenmeyecekListesindeVar = false;

                                                                            if (UcretDestegiIstenecekKisiler.Any(p => p.TckimlikNo.Equals(tcno) && p.Giris.Equals(iseGirisTarihi)))
                                                                            {
                                                                                UcretDestegiIstensinMi = true;
                                                                                IstenecekveyaIstenmeyecekListesindeVar = true;
                                                                            }
                                                                            else if (UcretDestegiIstenmeyecekKisiler.Any(p => p.TckimlikNo.Equals(tcno) && p.Giris.Equals(iseGirisTarihi)))
                                                                            {
                                                                                UcretDestegiIstensinMi = false;
                                                                                IstenecekveyaIstenmeyecekListesindeVar = true;
                                                                            }

                                                                            Metodlar.DetayliLogYaz(tcno + " teşvikleri sorgulanıyor");

                                                                            int sayacKisi = 0;

                                                                            int sistemselHataUyarisiSayac = 0;
                                                                            int icUygulamaHatasiSayac = 0;
                                                                            int isyeriBilgileriniKontrolEdinizSayac = 0;
                                                                            int webServisHatasiSayac = 0;
                                                                            int tooManyOpenFilesHatasiSayac = 0;
                                                                            int unRecognizedFieldHatasiSayac = 0;

                                                                        KisininHakEttigiTesvikleriBul:

                                                                            while (UcretIstiyorDiyalogMesajiAcik || SistemHatasiVarDiyalogMesajiAcik || IcUygulamayaErisilemediDiyalogMesajiAcik || IsyeriBilgileriniKontrolEdinizUyarisiDiyalogMesajiAcik || WebServisUyarisiDiyalogMesajiAcik || UnrecognizedFieldDiyalogMesajiAcik || TooManyOpenFilesUyarisiDiyalogMesajiAcik)
                                                                            {
                                                                                Thread.Sleep(1000);
                                                                            }

                                                                            var response = kisiWebClient.PostData("https://uyg.sgk.gov.tr/YeniSistem/Isveren/uygunSigortaliBilgileriDonem.action", "iseGirisSirali=false&tcDonemSorgu=1&kolayIsveren=false&donem_yil_ay_index=" + optionvalue + "&tcKimlikNo=0&egitimDurumu=" + egitimDurumu + "&kazanc=" + System.Net.WebUtility.UrlEncode(kazanc) + "&ucretDestegiTalebiVarMi=" + (UcretDestegiIstensinMi ? "true" : "false") + (iseGirisTarihi >= new DateTime(2020, 12, 1) ? "&sigortaliBasvuruTarihi=" + iseGirisTarihi.ToString("dd.MM.yyyy") + "&sigortaliDurum=1" : "") + "&iseGirisMapIndex=" + iseGirisMapIndexValue + "&action%3AsigortaliSorgula=Devam");

                                                                            var gecerliSayfaSorgusuKisiTesvikleri = GecerliSayfaOlupOlmadiginiKontrolEt(response, "KİŞİNİN TÜM TEŞVİKLERİ", ref webclient, false);

                                                                            if (gecerliSayfaSorgusuKisiTesvikleri == Enums.GecerliSayfaSonuclari.Iptal) return;
                                                                            else if (gecerliSayfaSorgusuKisiTesvikleri == Enums.GecerliSayfaSonuclari.Gecersiz)
                                                                            {
                                                                                sayacKisi++;

                                                                                if (sayacKisi < 20)
                                                                                {
                                                                                    Metodlar.DetayliLogYaz(tcno + " kişisinin teşvik sorgusunda geçerli bir sayfa bulunamadı." + (sayacKisi + 1) + ".kez denenecek");

                                                                                    Thread.Sleep(1000);

                                                                                    goto KisininHakEttigiTesvikleriBul;
                                                                                }
                                                                            }
                                                                            else if (gecerliSayfaSorgusuKisiTesvikleri == Enums.GecerliSayfaSonuclari.UzunSureliIslemYapilamadiUyarisi)
                                                                            {
                                                                                if (!Program.KisiIslemcisiYeniGirisYapsin)
                                                                                {
                                                                                    while (YenidenBaglaniliyor) { Thread.Sleep(200); }

                                                                                    if (!YenidenBaglaniliyor)
                                                                                    {
                                                                                        YenidenBaglaniliyor = true;

                                                                                        lock (DonemWebClient)
                                                                                        {
                                                                                            var wc = Program.DonemIslemcisiYeniGirisYapsin ? DonemWebClient : webclient;

                                                                                            if (wc.oturumId.Equals(kisiWebClient.oturumId))
                                                                                            {
                                                                                                wc.ReConnect();

                                                                                                DonemWebClient.Cookie = wc.Cookie;

                                                                                                //allClients.ForEach(p => p.Cookie = webclient.Cookie);
                                                                                            }

                                                                                            kisiWebClient.Cookie = wc.Cookie;
                                                                                        }
                                                                                    }

                                                                                    YenidenBaglaniliyor = false;
                                                                                }
                                                                                else kisiWebClient.Disconnect();

                                                                                lock (islemYapilanKisiler)
                                                                                {
                                                                                    if (islemYapilanKisiler.Contains(tcNoveIseGirisTarihi))
                                                                                    {
                                                                                        islemYapilanKisiler.Remove(tcNoveIseGirisTarihi);
                                                                                    }
                                                                                }

                                                                                TesvikTanimlamaSayfasiAcilacak = true;

                                                                                goto KisiEnBasaDon;
                                                                            }
                                                                            else if (gecerliSayfaSorgusuKisiTesvikleri == Enums.GecerliSayfaSonuclari.Gecerli)
                                                                            {

                                                                                if (response.Contains("Unrecognized field"))
                                                                                {
                                                                                    unRecognizedFieldHatasiSayac++;

                                                                                    if (unRecognizedFieldHatasiSayac < 1)
                                                                                    {
                                                                                        BasvuruLogEkle(String.Format("{0} kişisinin {1} tarihli girişinin sorgu sonucunda \"Unrecognized field\" hatası ile karşılaşıldı.{2}. kez denenecek", tcno, iseGirisTarihi.ToString("dd.MM.yyyy"), unRecognizedFieldHatasiSayac + 1), donemindex, kisiindex, false);

                                                                                        Thread.Sleep(TimeSpan.FromMilliseconds(500));

                                                                                        goto KisininHakEttigiTesvikleriBul;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        BasvuruLogEkle(String.Format("{0} kişisinin {1} tarihli girişi sorgulanırken \"Unrecognized field\" hatası ile karşılaşıldı", tcno, iseGirisTarihi.ToString("dd.MM.yyyy")), donemindex, kisiindex, true);

                                                                                        if (!UnrecognizedFieldDiyalogMesajiSoruldu)
                                                                                        {
                                                                                            UnrecognizedFieldDiyalogMesajiSoruldu = true;

                                                                                            UnrecognizedFieldDiyalogMesajiAcik = true;

                                                                                            bool unrecognizedFieldHatasindanSonraDevamEdilsinMi = MessageBox.Show("\"Unrecognized field\" hatası mevcut. Devam edilsin mi?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

                                                                                            UnrecognizedFieldDiyalogMesajiAcik = false;

                                                                                            if (!unrecognizedFieldHatasindanSonraDevamEdilsinMi)
                                                                                            {

                                                                                                IslemiIptalEt();

                                                                                                return;
                                                                                            }

                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {

                                                                                    HtmlAgilityPack.HtmlDocument htmltesviksonuclari = new HtmlAgilityPack.HtmlDocument();
                                                                                    htmltesviksonuclari.LoadHtml(response);

                                                                                    var tumTablelar = htmltesviksonuclari.DocumentNode.SelectNodes("//table");
                                                                                    int tesviksayisi = 0;

                                                                                    var satirlartesviksonuclari = htmltesviksonuclari.DocumentNode.SelectNodes("//table[@class='gradienttable']/tr");

                                                                                    var satirtesviksonuclari = satirlartesviksonuclari[1];

                                                                                    var islemYapilanTc = satirtesviksonuclari.SelectSingleNode("td[1]/p/text()").GetInnerText();

                                                                                    int basariliSayisi = 0;

                                                                                    List<string> tesvikTanimlanacakKanunlar = new List<string>();
                                                                                    string isegiristarihi = null;
                                                                                    string meslekkodu = null;
                                                                                    string kazanc_miktari = "-1";
                                                                                    string sigortaliBasvuruTarihi = null;
                                                                                    string sigortaliHizmetDurumu = null;

                                                                                    if (tumTablelar.Count() > 0)
                                                                                    {
                                                                                        bool sistemHatasiUyarisiVar = false;
                                                                                        bool icUygulamayaEriselemediUyarisiVar = false;
                                                                                        bool isyeriBilgileriniKontrolEdinizUyarisiVar = false;
                                                                                        bool webServisUyarisiVar = false;
                                                                                        bool tooManyOpenFilesUyarisiVar = false;

                                                                                        bool dataTableBulundu = false;

                                                                                        foreach (var table in tumTablelar)
                                                                                        {

                                                                                            if (table.Id == "dataTable")
                                                                                            {
                                                                                                dataTableBulundu = true;

                                                                                                if (table.InnerText.Contains("6111") && (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf6111))
                                                                                                {
                                                                                                    var tablolar = table.Descendants("tr").FirstOrDefault(p => p.Id.Equals("tablolar"));

                                                                                                    var th = tablolar.Descendants("th").FirstOrDefault();

                                                                                                    if (th.OuterHtml.Contains("background:#b3ffd9"))
                                                                                                    {
                                                                                                        var tesvikdonemi = table.ChildNodes.Where(p => p.InnerText != null && p.InnerText.Contains("Başlangıç-Bitiş Dönemi")).FirstOrDefault().InnerText;

                                                                                                        if (!tesvikdonemi.Contains("Tespit edilemedi.-Tespit edilemedi"))
                                                                                                        {
                                                                                                            tesviksayisi++;

                                                                                                            var formSigortaliKaydet = htmltesviksonuclari.GetElementbyId("sigortaliKaydet");

                                                                                                            isegiristarihi = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_iseGirisTarihi")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            meslekkodu = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_meslekKodu")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            kazanc_miktari = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_kazanc")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            string kanunNo = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("kanunNoSecimId")).FirstOrDefault().Attributes["value"].Value;

                                                                                                            if (String.IsNullOrEmpty(kanunNo)) kanunNo = "6111";

                                                                                                            tesvikTanimlanacakKanunlar.Add(kanunNo);
                                                                                                        }

                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (table.InnerText.Contains("meslek kodu bulunamamıştır") || table.InnerText.Contains("Eğitim belgesi ile meslek kodu uyuşmamaktadır"))
                                                                                                        {
                                                                                                            if (Program.BfEgitimBelgesi == 10) //Ayarlarda tümü seçili ise
                                                                                                            {
                                                                                                                if (!egitimDurumu.Equals("1"))
                                                                                                                {
                                                                                                                    switch (egitimDurumu)
                                                                                                                    {
                                                                                                                        case "9":
                                                                                                                            egitimDurumu = "8";
                                                                                                                            break;
                                                                                                                        case "8":
                                                                                                                            egitimDurumu = "7";
                                                                                                                            break;
                                                                                                                        case "7":
                                                                                                                            egitimDurumu = "6";
                                                                                                                            break;
                                                                                                                        case "6":
                                                                                                                            egitimDurumu = "5";
                                                                                                                            break;
                                                                                                                        case "5":
                                                                                                                            egitimDurumu = "4";
                                                                                                                            break;
                                                                                                                        case "4":
                                                                                                                            egitimDurumu = "1";
                                                                                                                            break;
                                                                                                                        default:
                                                                                                                            egitimDurumu = "1";
                                                                                                                            break;
                                                                                                                    }

                                                                                                                    goto KisininHakEttigiTesvikleriBul;
                                                                                                                }
                                                                                                            }
                                                                                                            else if (Program.BfEgitimBelgesi == 9) //Ayarlarda mesleki yeterlilik belgesi seçili ise
                                                                                                            {
                                                                                                                if (!egitimDurumu.Equals("1"))
                                                                                                                {
                                                                                                                    switch (egitimDurumu)
                                                                                                                    {
                                                                                                                        case "9":
                                                                                                                            egitimDurumu = "8";
                                                                                                                            break;
                                                                                                                        case "8":
                                                                                                                            egitimDurumu = "1";
                                                                                                                            break;
                                                                                                                        default:
                                                                                                                            egitimDurumu = "1";
                                                                                                                            break;
                                                                                                                    }

                                                                                                                    goto KisininHakEttigiTesvikleriBul;
                                                                                                                }
                                                                                                            }
                                                                                                            else if (Program.BfEgitimBelgesi == 11) //Ayarlarda Bf Eğitim Belgesinde 7252-7256 seçili ise
                                                                                                            {
                                                                                                                if (!egitimDurumu.Equals("1"))
                                                                                                                {
                                                                                                                    switch (egitimDurumu)
                                                                                                                    {
                                                                                                                        case "9":
                                                                                                                            egitimDurumu = "8";
                                                                                                                            break;
                                                                                                                        case "8":
                                                                                                                            egitimDurumu = "1";
                                                                                                                            break;
                                                                                                                        default:
                                                                                                                            egitimDurumu = "1";
                                                                                                                            break;
                                                                                                                    }

                                                                                                                    goto KisininHakEttigiTesvikleriBul;
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (!egitimDurumu.Equals("1"))
                                                                                                                {
                                                                                                                    egitimDurumu = "1";

                                                                                                                    goto KisininHakEttigiTesvikleriBul;
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }

                                                                                                    //6111 hata tanımlamaları
                                                                                                    if (table.InnerText.Contains("Sistemsel bir hata oluştu")) sistemHatasiUyarisiVar = true;
                                                                                                    if (table.InnerText.Contains("İşyeri bilgilerini doğru girdiğinizden emin olunuz")) isyeriBilgileriniKontrolEdinizUyarisiVar = true;
                                                                                                    if (table.InnerText.ToLower().Contains("iç uygulamaya erişilemedi") || table.InnerText.ToLower().Contains("iç uygulamaya, (işkur kaydı sorgu sonucu) erişilemedi daha sonra tekrar deneyiniz.") || table.InnerText.Contains("İç uygulamaya, (ISKUR kaydı sorgu sonucu) erişilemedi daha sonra tekrar deneyiniz.") || table.InnerText.ToLower().Contains("erişilemedi") || table.InnerText.ToLower().Contains("sonra") || table.InnerText.ToLower().Contains("tekrar") || table.InnerText.ToLower().Contains("deneyiniz")) icUygulamayaEriselemediUyarisiVar = true;
                                                                                                    if (table.InnerText.Contains("Web Servis Şu Anda Çalışmıyor")) webServisUyarisiVar = true;
                                                                                                    if (table.InnerText.ToLower().Contains("too many open files")) tooManyOpenFilesUyarisiVar = true;
                                                                                                }
                                                                                                else if ((table.InnerText.Contains("17103") || table.InnerText.Contains("27103")) && (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf7103))
                                                                                                {
                                                                                                    var tablolar = table.Descendants("tr").FirstOrDefault(p => p.Id.Equals("tablolar"));

                                                                                                    var th = tablolar.Descendants("th").FirstOrDefault();

                                                                                                    if (th.OuterHtml.Contains("background:#b3ffd9"))
                                                                                                    {
                                                                                                        var tesvikdonemi = table.ChildNodes.Where(p => p.InnerText != null && p.InnerText.Contains("Başlangıç-Bitiş Dönemi")).FirstOrDefault().InnerText;

                                                                                                        if (!tesvikdonemi.Contains("Tespit edilemedi.-Tespit edilemedi"))
                                                                                                        {

                                                                                                            if (Program.BfIndirmeUcretDestegiIstensin && !UcretDestegiIstiyorSoruldu && !IstenecekveyaIstenmeyecekListesindeVar)
                                                                                                            {
                                                                                                                UcretDestegiIstiyorSoruldu = true;

                                                                                                                UcretIstiyorDiyalogMesajiAcik = true;

                                                                                                                UcretDestegiIstiyor = MessageBox.Show("Yeni tanımlama yapılacak 7103 teşviği bulundu. Bu işyeri için ücret desteği istensin mi?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

                                                                                                                UcretDestegiIstensinMi = UcretDestegiIstiyor;

                                                                                                                UcretIstiyorDiyalogMesajiAcik = false;

                                                                                                                goto KisininHakEttigiTesvikleriBul;
                                                                                                            }

                                                                                                            tesviksayisi++;

                                                                                                            var formSigortaliKaydet = htmltesviksonuclari.GetElementbyId("sigortaliKaydet");

                                                                                                            isegiristarihi = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_iseGirisTarihi")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            meslekkodu = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_meslekKodu")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            kazanc_miktari = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_kazanc")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            string kanunNo = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("kanunNoSecimId")).FirstOrDefault().Attributes["value"].Value;

                                                                                                            if (String.IsNullOrEmpty(kanunNo)) kanunNo = th.InnerText.Contains("17103") ? "17103" : "27103";

                                                                                                            tesvikTanimlanacakKanunlar.Add(kanunNo);
                                                                                                        }

                                                                                                    }
                                                                                                    //17103 hata tanımlamaları
                                                                                                    if (table.InnerText.Contains("Sistemsel bir hata oluştu")) sistemHatasiUyarisiVar = true;
                                                                                                    if (table.InnerText.ToLower().Contains("iç uygulamaya erişilemedi") || table.InnerText.ToLower().Contains("iç uygulamaya, (işkur kaydı sorgu sonucu) erişilemedi daha sonra tekrar deneyiniz.") || table.InnerText.Contains("İç uygulamaya, (ISKUR kaydı sorgu sonucu) erişilemedi daha sonra tekrar deneyiniz.") || table.InnerText.ToLower().Contains("erişilemedi") || table.InnerText.ToLower().Contains("sonra") || table.InnerText.ToLower().Contains("tekrar") || table.InnerText.ToLower().Contains("deneyiniz")) icUygulamayaEriselemediUyarisiVar = true;
                                                                                                    if (table.InnerText.Contains("Web Servis Şu Anda Çalışmıyor")) webServisUyarisiVar = true;
                                                                                                    if (table.InnerText.ToLower().Contains("too many open files")) tooManyOpenFilesUyarisiVar = true;


                                                                                                }
                                                                                                else if (table.InnerText.Contains("2828") && (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf2828))
                                                                                                {
                                                                                                    var tablolar = table.Descendants("tr").FirstOrDefault(p => p.Id.Equals("tablolar"));

                                                                                                    var th = tablolar.Descendants("th").FirstOrDefault();


                                                                                                    if (th.OuterHtml.Contains("background:#b3ffd9"))
                                                                                                    {
                                                                                                        var tesvikdonemi = table.ChildNodes.Where(p => p.InnerText != null && p.InnerText.Contains("Başlangıç-Bitiş Dönemi")).FirstOrDefault().InnerText;

                                                                                                        if (!tesvikdonemi.Contains("Tespit edilemedi.-Tespit edilemedi"))
                                                                                                        {
                                                                                                            tesviksayisi++;

                                                                                                            var formSigortaliKaydet = htmltesviksonuclari.GetElementbyId("sigortaliKaydet");

                                                                                                            isegiristarihi = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_iseGirisTarihi")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            meslekkodu = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_meslekKodu")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            kazanc_miktari = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_kazanc")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            string kanunNo = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("kanunNoSecimId")).FirstOrDefault().Attributes["value"].Value;

                                                                                                            if (String.IsNullOrEmpty(kanunNo)) kanunNo = "2828";

                                                                                                            tesvikTanimlanacakKanunlar.Add(kanunNo);
                                                                                                        }

                                                                                                    }
                                                                                                    //2828 hata tanımlamaları
                                                                                                    if (table.InnerText.Contains("Sistemsel bir hata oluştu")) sistemHatasiUyarisiVar = true;
                                                                                                    if (table.InnerText.ToLower().Contains("iç uygulamaya erişilemedi") || table.InnerText.ToLower().Contains("iç uygulamaya, (işkur kaydı sorgu sonucu) erişilemedi daha sonra tekrar deneyiniz.") || table.InnerText.Contains("İç uygulamaya, (ISKUR kaydı sorgu sonucu) erişilemedi daha sonra tekrar deneyiniz.") || table.InnerText.ToLower().Contains("erişilemedi") || table.InnerText.ToLower().Contains("sonra") || table.InnerText.ToLower().Contains("tekrar") || table.InnerText.ToLower().Contains("deneyiniz")) icUygulamayaEriselemediUyarisiVar = true;
                                                                                                    if (table.InnerText.Contains("Web Servis Şu Anda Çalışmıyor")) webServisUyarisiVar = true;
                                                                                                    if (table.InnerText.ToLower().Contains("too many open files")) tooManyOpenFilesUyarisiVar = true;
                                                                                                }
                                                                                                else if (table.InnerText.Contains("7252") && (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf7252))
                                                                                                {
                                                                                                    if (CariTanimla && bfsira == Enums.BasvuruFormuTurleri.BfTumu) continue;

                                                                                                    var tablolar = table.Descendants("tr").FirstOrDefault(p => p.Id.Equals("tablolar"));

                                                                                                    var th = tablolar.Descendants("th").FirstOrDefault();


                                                                                                    if (th.OuterHtml.Contains("background:#b3ffd9"))
                                                                                                    {
                                                                                                        var tesvikdonemi = table.ChildNodes.Where(p => p.InnerText != null && p.InnerText.Contains("Başlangıç-Bitiş Dönemi")).FirstOrDefault().InnerText;

                                                                                                        if (!tesvikdonemi.Contains("Tespit edilemedi.-Tespit edilemedi"))
                                                                                                        {
                                                                                                            tesviksayisi++;

                                                                                                            var formSigortaliKaydet = htmltesviksonuclari.GetElementbyId("sigortaliKaydet");

                                                                                                            isegiristarihi = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_iseGirisTarihi")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            meslekkodu = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_meslekKodu")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            kazanc_miktari = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_kazanc")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            string kanunNo = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("kanunNoSecimId")).FirstOrDefault().Attributes["value"].Value;

                                                                                                            if (String.IsNullOrEmpty(kanunNo)) kanunNo = "7252";

                                                                                                            tesvikTanimlanacakKanunlar.Add(kanunNo);
                                                                                                        }

                                                                                                    }
                                                                                                    //7252 hata tanımlamaları
                                                                                                    if (table.InnerText.Contains("Sistemsel bir hata oluştu")) sistemHatasiUyarisiVar = true;
                                                                                                    if (table.InnerText.ToLower().Contains("iç uygulamaya erişilemedi") || table.InnerText.ToLower().Contains("iç uygulamaya, (işkur kaydı sorgu sonucu) erişilemedi daha sonra tekrar deneyiniz.") || table.InnerText.Contains("İç uygulamaya, (ISKUR kaydı sorgu sonucu) erişilemedi daha sonra tekrar deneyiniz.") || table.InnerText.ToLower().Contains("erişilemedi") || table.InnerText.ToLower().Contains("sonra") || table.InnerText.ToLower().Contains("tekrar") || table.InnerText.ToLower().Contains("deneyiniz")) icUygulamayaEriselemediUyarisiVar = true;
                                                                                                    if (table.InnerText.Contains("Web Servis Şu Anda Çalışmıyor")) webServisUyarisiVar = true;
                                                                                                    if (table.InnerText.ToLower().Contains("too many open files")) tooManyOpenFilesUyarisiVar = true;
                                                                                                }
                                                                                                else if ((/*table.InnerText.Contains("17256") ||*/ table.InnerText.Contains("27256")) && (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf7256))
                                                                                                {
                                                                                                    if (CariTanimla && bfsira == Enums.BasvuruFormuTurleri.BfTumu) continue;

                                                                                                    var tablolar = table.Descendants("tr").FirstOrDefault(p => p.Id.Equals("tablolar"));

                                                                                                    var th = tablolar.Descendants("th").FirstOrDefault();


                                                                                                    if (th.OuterHtml.Contains("background:#b3ffd9"))
                                                                                                    {
                                                                                                        var tesvikdonemi = table.ChildNodes.Where(p => p.InnerText != null && p.InnerText.Contains("Başlangıç-Bitiş Dönemi")).FirstOrDefault().InnerText;

                                                                                                        if (!tesvikdonemi.Contains("Tespit edilemedi.-Tespit edilemedi"))
                                                                                                        {
                                                                                                            tesviksayisi++;

                                                                                                            var formSigortaliKaydet = htmltesviksonuclari.GetElementbyId("sigortaliKaydet");

                                                                                                            isegiristarihi = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_iseGirisTarihi")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            meslekkodu = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_meslekKodu")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            kazanc_miktari = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_kazanc")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            string kanunNo = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("kanunNoSecimId")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            sigortaliBasvuruTarihi = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_sigortaliBasvuruTarihi")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            sigortaliHizmetDurumu = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_sigortaliDurum")).FirstOrDefault().Attributes["value"].Value;

                                                                                                            //if (String.IsNullOrEmpty(kanunNo)) kanunNo = th.InnerText.Contains("17256") ? "17256" : "27256";
                                                                                                            if (String.IsNullOrEmpty(kanunNo)) kanunNo = "27256";

                                                                                                            tesvikTanimlanacakKanunlar.Add(kanunNo);
                                                                                                        }

                                                                                                    }
                                                                                                    //27256 hata tanımlamaları
                                                                                                    if (table.InnerText.Contains("Sistemsel bir hata oluştu")) sistemHatasiUyarisiVar = true;
                                                                                                    if (table.InnerText.ToLower().Contains("iç uygulamaya erişilemedi") || table.InnerText.ToLower().Contains("iç uygulamaya, (işkur kaydı sorgu sonucu) erişilemedi daha sonra tekrar deneyiniz.") || table.InnerText.Contains("İç uygulamaya, (ISKUR kaydı sorgu sonucu) erişilemedi daha sonra tekrar deneyiniz.") || table.InnerText.ToLower().Contains("erişilemedi") || table.InnerText.ToLower().Contains("sonra") || table.InnerText.ToLower().Contains("tekrar") || table.InnerText.ToLower().Contains("deneyiniz")) icUygulamayaEriselemediUyarisiVar = true;
                                                                                                    if (table.InnerText.Contains("Web Servis Şu Anda Çalışmıyor")) webServisUyarisiVar = true;
                                                                                                    if (table.InnerText.ToLower().Contains("too many open files")) tooManyOpenFilesUyarisiVar = true;


                                                                                                }
                                                                                                else if (table.InnerText.Contains("7316") && (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf7316))
                                                                                                {
                                                                                                    if (CariTanimla && bfsira == Enums.BasvuruFormuTurleri.BfTumu) continue;

                                                                                                    var tablolar = table.Descendants("tr").FirstOrDefault(p => p.Id.Equals("tablolar"));

                                                                                                    var th = tablolar.Descendants("th").FirstOrDefault();


                                                                                                    if (th.OuterHtml.Contains("background:#b3ffd9"))
                                                                                                    {
                                                                                                        var tesvikdonemi = table.ChildNodes.Where(p => p.InnerText != null && p.InnerText.Contains("Başlangıç-Bitiş Dönemi")).FirstOrDefault().InnerText;

                                                                                                        if (!tesvikdonemi.Contains("Tespit edilemedi.-Tespit edilemedi"))
                                                                                                        {
                                                                                                            tesviksayisi++;

                                                                                                            var formSigortaliKaydet = htmltesviksonuclari.GetElementbyId("sigortaliKaydet");

                                                                                                            isegiristarihi = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_iseGirisTarihi")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            meslekkodu = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_meslekKodu")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            kazanc_miktari = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_kazanc")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            string kanunNo = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("kanunNoSecimId")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            sigortaliBasvuruTarihi = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_sigortaliBasvuruTarihi")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            sigortaliHizmetDurumu = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_sigortaliDurum")).FirstOrDefault().Attributes["value"].Value;

                                                                                                            if (String.IsNullOrEmpty(kanunNo)) kanunNo = "7316";

                                                                                                            tesvikTanimlanacakKanunlar.Add(kanunNo);
                                                                                                        }

                                                                                                    }
                                                                                                    //7316 hata tanımlamaları
                                                                                                    if (table.InnerText.Contains("Sistemsel bir hata oluştu")) sistemHatasiUyarisiVar = true;
                                                                                                    if (table.InnerText.ToLower().Contains("iç uygulamaya erişilemedi") || table.InnerText.ToLower().Contains("iç uygulamaya, (işkur kaydı sorgu sonucu) erişilemedi daha sonra tekrar deneyiniz.") || table.InnerText.Contains("İç uygulamaya, (ISKUR kaydı sorgu sonucu) erişilemedi daha sonra tekrar deneyiniz.") || table.InnerText.ToLower().Contains("erişilemedi") || table.InnerText.ToLower().Contains("sonra") || table.InnerText.ToLower().Contains("tekrar") || table.InnerText.ToLower().Contains("deneyiniz")) icUygulamayaEriselemediUyarisiVar = true;
                                                                                                    if (table.InnerText.Contains("Web Servis Şu Anda Çalışmıyor")) webServisUyarisiVar = true;
                                                                                                    if (table.InnerText.ToLower().Contains("too many open files")) tooManyOpenFilesUyarisiVar = true;


                                                                                                }
                                                                                                else if (table.InnerText.Contains("3294") && (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf3294))
                                                                                                {
                                                                                                    var tablolar = table.Descendants("tr").FirstOrDefault(p => p.Id.Equals("tablolar"));

                                                                                                    var th = tablolar.Descendants("th").FirstOrDefault();


                                                                                                    if (th.OuterHtml.Contains("background:#b3ffd9"))
                                                                                                    {
                                                                                                        var tesvikdonemi = table.ChildNodes.Where(p => p.InnerText != null && p.InnerText.Contains("Başlangıç-Bitiş Dönemi")).FirstOrDefault().InnerText;

                                                                                                        if (!tesvikdonemi.Contains("Tespit edilemedi.-Tespit edilemedi"))
                                                                                                        {
                                                                                                            tesviksayisi++;

                                                                                                            var formSigortaliKaydet = htmltesviksonuclari.GetElementbyId("sigortaliKaydet");

                                                                                                            isegiristarihi = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_iseGirisTarihi")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            meslekkodu = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_meslekKodu")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            kazanc_miktari = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_kazanc")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            string kanunNo = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("kanunNoSecimId")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            sigortaliBasvuruTarihi = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_sigortaliBasvuruTarihi")).FirstOrDefault().Attributes["value"].Value;
                                                                                                            sigortaliHizmetDurumu = formSigortaliKaydet.ChildNodes.Where(p => p.Id.Equals("sigortaliKaydet_sigortaliDurum")).FirstOrDefault().Attributes["value"].Value;

                                                                                                            if (String.IsNullOrEmpty(kanunNo)) kanunNo = "3294";

                                                                                                            tesvikTanimlanacakKanunlar.Add(kanunNo);
                                                                                                        }

                                                                                                    }
                                                                                                    //3294 hata tanımlamaları
                                                                                                    if (table.InnerText.Contains("Sistemsel bir hata oluştu")) sistemHatasiUyarisiVar = true;
                                                                                                    if (table.InnerText.ToLower().Contains("iç uygulamaya erişilemedi") || table.InnerText.ToLower().Contains("iç uygulamaya, (işkur kaydı sorgu sonucu) erişilemedi daha sonra tekrar deneyiniz.") || table.InnerText.Contains("İç uygulamaya, (ISKUR kaydı sorgu sonucu) erişilemedi daha sonra tekrar deneyiniz.") || table.InnerText.ToLower().Contains("erişilemedi") || table.InnerText.ToLower().Contains("sonra") || table.InnerText.ToLower().Contains("tekrar") || table.InnerText.ToLower().Contains("deneyiniz")) icUygulamayaEriselemediUyarisiVar = true;
                                                                                                    if (table.InnerText.Contains("Web Servis Şu Anda Çalışmıyor")) webServisUyarisiVar = true;
                                                                                                    if (table.InnerText.ToLower().Contains("too many open files")) tooManyOpenFilesUyarisiVar = true;


                                                                                                }
                                                                                            }
                                                                                        }

                                                                                        //bool hatadanDolayiKisiAtlanacak = false;

                                                                                        if (sistemHatasiUyarisiVar)
                                                                                        {
                                                                                            sistemselHataUyarisiSayac++;

                                                                                            if (sistemselHataUyarisiSayac < 10)
                                                                                            {
                                                                                                BasvuruLogEkle(String.Format("{0} kişisinin {1} tarihli girişinin sorgu sonucunda \"Sistemsel bir hata oluştu\" uyarısı ile karşılaşıldı.{2}. kez denenecek", tcno, iseGirisTarihi.ToString("dd.MM.yyyy"), sistemselHataUyarisiSayac + 1), donemindex, kisiindex, false);

                                                                                                Thread.Sleep(TimeSpan.FromMilliseconds(500));

                                                                                                goto KisininHakEttigiTesvikleriBul;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                //hatadanDolayiKisiAtlanacak = true;

                                                                                                BasvuruLogEkle(String.Format("{0} kişisinin {1} tarihli girişi 10 defa sorgulanmasına rağmen \"Sistemsel bir hata oluştu\" uyarısı ile karşılaşıldı", tcno, iseGirisTarihi.ToString("dd.MM.yyyy")), donemindex, kisiindex, true);

                                                                                                if (!SistemHatasiVarDiyalogMesajiSoruldu)
                                                                                                {
                                                                                                    SistemHatasiVarDiyalogMesajiSoruldu = true;

                                                                                                    SistemHatasiVarDiyalogMesajiAcik = true;

                                                                                                    var frmOnayEkrani = new frmOnay("\"Sistemsel bir hata oluştu\" uyarısı mevcut. Devam edilsin mi?");

                                                                                                    var frmGostermeZamani = DateTime.Now;

                                                                                                    bool SistemHatasiUyarisindenSonraDevamEdilsinMi = true;

                                                                                                    bool bekle120Saniye = true;

                                                                                                    var task120SaniyeBekle = Task.Run(() =>
                                                                                                    {
                                                                                                        while (bekle120Saniye)
                                                                                                        {
                                                                                                            if (DateTime.Now.Subtract(frmGostermeZamani).TotalSeconds >= 120)
                                                                                                            {
                                                                                                                try
                                                                                                                {
                                                                                                                    if (frmOnayEkrani != null)
                                                                                                                    {
                                                                                                                        SistemHatasiUyarisindenSonraDevamEdilsinMi = true;

                                                                                                                        frmOnayEkrani.Invoke((Action)(() =>
                                                                                                                        {
                                                                                                                            frmOnayEkrani.Kapat();
                                                                                                                        }));
                                                                                                                    }
                                                                                                                    else break;
                                                                                                                }
                                                                                                                catch (Exception ex)
                                                                                                                {
                                                                                                                    Debug.WriteLine(ex);
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                Thread.Sleep(1000);
                                                                                                            }
                                                                                                        }
                                                                                                    });

                                                                                                    SistemHatasiUyarisindenSonraDevamEdilsinMi = frmOnayEkrani.ShowDialog() == DialogResult.Yes;

                                                                                                    bekle120Saniye = false;

                                                                                                    SistemHatasiVarDiyalogMesajiAcik = false;

                                                                                                    frmOnayEkrani = null;

                                                                                                    if (!SistemHatasiUyarisindenSonraDevamEdilsinMi)
                                                                                                    {
                                                                                                        IslemiIptalEt();

                                                                                                        return;
                                                                                                    }

                                                                                                }
                                                                                            }
                                                                                        }

                                                                                        if (icUygulamayaEriselemediUyarisiVar)
                                                                                        {
                                                                                            if (!yabanciTcNoMu && !kimlikBulunamadiUyarisiVar)
                                                                                            {
                                                                                                icUygulamaHatasiSayac++;

                                                                                                if (icUygulamaHatasiSayac < 10)
                                                                                                {
                                                                                                    BasvuruLogEkle(String.Format("{0} kişisinin {1} tarihli girişinin sorgu sonucunda \"İç uygulamaya erişilemedi\" uyarısı ile karşılaşıldı.{2}. kez denenecek", tcno, iseGirisTarihi.ToString("dd.MM.yyyy"), icUygulamaHatasiSayac + 1), donemindex, kisiindex, false);

                                                                                                    Thread.Sleep(TimeSpan.FromMilliseconds(500));

                                                                                                    goto KisininHakEttigiTesvikleriBul;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    //hatadanDolayiKisiAtlanacak = true;

                                                                                                    BasvuruLogEkle(String.Format("{0} kişisinin {1} tarihli girişi 10 defa sorgulanmasına rağmen \"İç uygulamaya erişilemedi\" uyarısı ile karşılaşıldı", tcno, iseGirisTarihi.ToString("dd.MM.yyyy")), donemindex, kisiindex, true);

                                                                                                    if (!IcUygulamayaErisilemediDiyalogMesajiSoruldu)
                                                                                                    {
                                                                                                        IcUygulamayaErisilemediDiyalogMesajiSoruldu = true;

                                                                                                        IcUygulamayaErisilemediDiyalogMesajiAcik = true;

                                                                                                        var frmOnayEkrani = new frmOnay("\"İç uygulamaya erişilemedi\" uyarısı mevcut. Devam edilsin mi?");

                                                                                                        var frmGostermeZamani = DateTime.Now;

                                                                                                        bool icUygulamayaErisilemediUyarisindenSonraDevamEdilsinMi = true;

                                                                                                        bool bekle120Saniye = true;

                                                                                                        var task120SaniyeBekle = Task.Run(() =>
                                                                                                        {
                                                                                                            while (bekle120Saniye)
                                                                                                            {
                                                                                                                if (DateTime.Now.Subtract(frmGostermeZamani).TotalSeconds >= 120)
                                                                                                                {
                                                                                                                    try
                                                                                                                    {
                                                                                                                        if (frmOnayEkrani != null)
                                                                                                                        {
                                                                                                                            icUygulamayaErisilemediUyarisindenSonraDevamEdilsinMi = true;

                                                                                                                            frmOnayEkrani.Invoke((Action)(() =>
                                                                                                                            {
                                                                                                                                frmOnayEkrani.Kapat();
                                                                                                                            }));
                                                                                                                        }
                                                                                                                        else break;
                                                                                                                    }
                                                                                                                    catch (Exception ex)
                                                                                                                    {
                                                                                                                        Debug.WriteLine(ex);
                                                                                                                    }
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    Thread.Sleep(1000);
                                                                                                                }
                                                                                                            }
                                                                                                        });

                                                                                                        icUygulamayaErisilemediUyarisindenSonraDevamEdilsinMi = frmOnayEkrani.ShowDialog() == DialogResult.Yes;

                                                                                                        bekle120Saniye = false;

                                                                                                        IcUygulamayaErisilemediDiyalogMesajiAcik = false;

                                                                                                        frmOnayEkrani = null;

                                                                                                        if (!icUygulamayaErisilemediUyarisindenSonraDevamEdilsinMi)
                                                                                                        {
                                                                                                            IslemiIptalEt();

                                                                                                            return;
                                                                                                        }

                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }

                                                                                        if (isyeriBilgileriniKontrolEdinizUyarisiVar)
                                                                                        {
                                                                                            isyeriBilgileriniKontrolEdinizSayac++;

                                                                                            if (isyeriBilgileriniKontrolEdinizSayac < 10)
                                                                                            {
                                                                                                BasvuruLogEkle(String.Format("{0} kişisinin {1} tarihli girişinin sorgu sonucunda \"İşyeri bilgilerini doğru girdiğinizden emin olunuz\" uyarısı ile karşılaşıldı.{2}. kez denenecek", tcno, iseGirisTarihi.ToString("dd.MM.yyyy"), isyeriBilgileriniKontrolEdinizSayac + 1), donemindex, kisiindex, false);

                                                                                                Thread.Sleep(TimeSpan.FromMilliseconds(500));

                                                                                                goto KisininHakEttigiTesvikleriBul;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                //hatadanDolayiKisiAtlanacak = true;

                                                                                                BasvuruLogEkle(String.Format("{0} kişisinin {1} tarihli girişi 10 defa sorgulanmasına rağmen \"İşyeri bilgilerini doğru girdiğinizden emin olunuz\" uyarısı ile karşılaşıldı", tcno, iseGirisTarihi.ToString("dd.MM.yyyy")), donemindex, kisiindex, true);

                                                                                                if (!IsyeriBilgileriniKontrolEdinizUyarisiDiyalogMesajiSoruldu)
                                                                                                {
                                                                                                    IsyeriBilgileriniKontrolEdinizUyarisiDiyalogMesajiSoruldu = true;

                                                                                                    IsyeriBilgileriniKontrolEdinizUyarisiDiyalogMesajiAcik = true;

                                                                                                    var frmOnayEkrani = new frmOnay("\"İşyeri bilgilerini doğru girdiğinizden emin olunuz\" uyarısı mevcut. Devam edilsin mi?");

                                                                                                    var frmGostermeZamani = DateTime.Now;

                                                                                                    bool IsyeriBilgileriniKontrolEdinizUyarisindenSonraDevamEdilsinMi = true;

                                                                                                    bool bekle120Saniye = true;

                                                                                                    var task120SaniyeBekle = Task.Run(() =>
                                                                                                    {
                                                                                                        while (bekle120Saniye)
                                                                                                        {
                                                                                                            if (DateTime.Now.Subtract(frmGostermeZamani).TotalSeconds >= 120)
                                                                                                            {
                                                                                                                try
                                                                                                                {
                                                                                                                    if (frmOnayEkrani != null)
                                                                                                                    {
                                                                                                                        IsyeriBilgileriniKontrolEdinizUyarisindenSonraDevamEdilsinMi = true;

                                                                                                                        frmOnayEkrani.Invoke((Action)(() =>
                                                                                                                        {
                                                                                                                            frmOnayEkrani.Kapat();
                                                                                                                        }));
                                                                                                                    }
                                                                                                                    else break;
                                                                                                                }
                                                                                                                catch (Exception ex)
                                                                                                                {
                                                                                                                    Debug.WriteLine(ex);
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                Thread.Sleep(1000);
                                                                                                            }
                                                                                                        }
                                                                                                    });

                                                                                                    IsyeriBilgileriniKontrolEdinizUyarisindenSonraDevamEdilsinMi = frmOnayEkrani.ShowDialog() == DialogResult.Yes;

                                                                                                    bekle120Saniye = false;

                                                                                                    IsyeriBilgileriniKontrolEdinizUyarisiDiyalogMesajiAcik = false;

                                                                                                    frmOnayEkrani = null;

                                                                                                    if (!IsyeriBilgileriniKontrolEdinizUyarisindenSonraDevamEdilsinMi)
                                                                                                    {
                                                                                                        IslemiIptalEt();

                                                                                                        return;
                                                                                                    }

                                                                                                }
                                                                                            }
                                                                                        }

                                                                                        if (webServisUyarisiVar)
                                                                                        {
                                                                                            webServisHatasiSayac++;

                                                                                            if (webServisHatasiSayac < 10)
                                                                                            {
                                                                                                BasvuruLogEkle(String.Format("{0} kişisinin {1} tarihli girişinin sorgu sonucunda \"Web Servis Şu Anda Çalışmıyor\" uyarısı ile karşılaşıldı.{2}. kez denenecek", tcno, iseGirisTarihi.ToString("dd.MM.yyyy"), webServisHatasiSayac + 1), donemindex, kisiindex, false);

                                                                                                Thread.Sleep(TimeSpan.FromMilliseconds(500));

                                                                                                goto KisininHakEttigiTesvikleriBul;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                //hatadanDolayiKisiAtlanacak = true;

                                                                                                BasvuruLogEkle(String.Format("{0} kişisinin {1} tarihli girişi 10 defa sorgulanmasına rağmen \"Web Servis Şu Anda Çalışmıyor\" uyarısı ile karşılaşıldı", tcno, iseGirisTarihi.ToString("dd.MM.yyyy")), donemindex, kisiindex, true);

                                                                                                if (!WebServisUyarisiDiyalogMesajiSoruldu)
                                                                                                {
                                                                                                    WebServisUyarisiDiyalogMesajiSoruldu = true;

                                                                                                    WebServisUyarisiDiyalogMesajiAcik = true;

                                                                                                    var frmOnayEkrani = new frmOnay("\"Web Servis Şu Anda Çalışmıyor\" uyarısı mevcut. Devam edilsin mi?");

                                                                                                    var frmGostermeZamani = DateTime.Now;

                                                                                                    bool WebServisUyarisindenSonraDevamEdilsinMi = true;

                                                                                                    bool bekle120Saniye = true;

                                                                                                    var task120SaniyeBekle = Task.Run(() =>
                                                                                                    {
                                                                                                        while (bekle120Saniye)
                                                                                                        {
                                                                                                            if (DateTime.Now.Subtract(frmGostermeZamani).TotalSeconds >= 120)
                                                                                                            {
                                                                                                                try
                                                                                                                {
                                                                                                                    if (frmOnayEkrani != null)
                                                                                                                    {
                                                                                                                        WebServisUyarisindenSonraDevamEdilsinMi = true;

                                                                                                                        frmOnayEkrani.Invoke((Action)(() =>
                                                                                                                        {
                                                                                                                            frmOnayEkrani.Kapat();
                                                                                                                        }));
                                                                                                                    }
                                                                                                                    else break;
                                                                                                                }
                                                                                                                catch (Exception ex)
                                                                                                                {
                                                                                                                    Debug.WriteLine(ex);
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                Thread.Sleep(1000);
                                                                                                            }
                                                                                                        }
                                                                                                    });

                                                                                                    WebServisUyarisindenSonraDevamEdilsinMi = frmOnayEkrani.ShowDialog() == DialogResult.Yes;

                                                                                                    bekle120Saniye = false;

                                                                                                    WebServisUyarisiDiyalogMesajiAcik = false;

                                                                                                    frmOnayEkrani = null;

                                                                                                    if (!WebServisUyarisindenSonraDevamEdilsinMi)
                                                                                                    {
                                                                                                        IslemiIptalEt();

                                                                                                        return;
                                                                                                    }

                                                                                                }
                                                                                            }
                                                                                        }

                                                                                        if (tooManyOpenFilesUyarisiVar)
                                                                                        {
                                                                                            tooManyOpenFilesHatasiSayac++;

                                                                                            if (tooManyOpenFilesHatasiSayac < 10)
                                                                                            {
                                                                                                BasvuruLogEkle(String.Format("{0} kişisinin {1} tarihli girişinin sorgu sonucunda \"Too many open files\" uyarısı ile karşılaşıldı.{2}. kez denenecek", tcno, iseGirisTarihi.ToString("dd.MM.yyyy"), webServisHatasiSayac + 1), donemindex, kisiindex, false);

                                                                                                Thread.Sleep(TimeSpan.FromMilliseconds(500));

                                                                                                goto KisininHakEttigiTesvikleriBul;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                //hatadanDolayiKisiAtlanacak = true;

                                                                                                BasvuruLogEkle(String.Format("{0} kişisinin {1} tarihli girişi 10 defa sorgulanmasına rağmen \"Too many open files\" uyarısı ile karşılaşıldı", tcno, iseGirisTarihi.ToString("dd.MM.yyyy")), donemindex, kisiindex, true);

                                                                                                if (!TooManyOpenFilesUyarisiDiyalogMesajiSoruldu)
                                                                                                {
                                                                                                    TooManyOpenFilesUyarisiDiyalogMesajiSoruldu = true;

                                                                                                    TooManyOpenFilesUyarisiDiyalogMesajiAcik = true;

                                                                                                    var frmOnayEkrani = new frmOnay("\"Too many open files\" uyarısı mevcut. Devam edilsin mi?");

                                                                                                    var frmGostermeZamani = DateTime.Now;

                                                                                                    bool tooManyOpenFilesUyarisindenSonraDevamEdilsinMi = true;

                                                                                                    bool bekle120Saniye = true;

                                                                                                    var task120SaniyeBekle = Task.Run(() =>
                                                                                                    {
                                                                                                        while (bekle120Saniye)
                                                                                                        {
                                                                                                            if (DateTime.Now.Subtract(frmGostermeZamani).TotalSeconds >= 120)
                                                                                                            {
                                                                                                                try
                                                                                                                {
                                                                                                                    if (frmOnayEkrani != null)
                                                                                                                    {
                                                                                                                        tooManyOpenFilesUyarisindenSonraDevamEdilsinMi = true;

                                                                                                                        frmOnayEkrani.Invoke((Action)(() =>
                                                                                                                        {
                                                                                                                            frmOnayEkrani.Kapat();
                                                                                                                        }));
                                                                                                                    }
                                                                                                                    else break;
                                                                                                                }
                                                                                                                catch (Exception ex)
                                                                                                                {
                                                                                                                    Debug.WriteLine(ex);
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                Thread.Sleep(1000);
                                                                                                            }
                                                                                                        }
                                                                                                    });

                                                                                                    tooManyOpenFilesUyarisindenSonraDevamEdilsinMi = frmOnayEkrani.ShowDialog() == DialogResult.Yes;

                                                                                                    bekle120Saniye = false;

                                                                                                    TooManyOpenFilesUyarisiDiyalogMesajiAcik = false;

                                                                                                    frmOnayEkrani = null;

                                                                                                    if (!tooManyOpenFilesUyarisindenSonraDevamEdilsinMi)
                                                                                                    {
                                                                                                        IslemiIptalEt();

                                                                                                        return;
                                                                                                    }

                                                                                                }
                                                                                            }
                                                                                        }


                                                                                        if (tesviksayisi == 0)
                                                                                        {

                                                                                            if (dataTableBulundu)
                                                                                            {
                                                                                                lock (islemiTamamlananKisiler)
                                                                                                {
                                                                                                    islemiTamamlananKisiler.Add(tcNoveIseGirisTarihi);
                                                                                                }
                                                                                            }

                                                                                            BasvuruLogEkle(tcno + " kişisinin " + iseGirisTarihi.ToString("dd.MM.yyyy") + " girişine teşvik verilmeyecek", donemindex, kisiindex, false);

                                                                                            Metodlar.DetayliLogYaz(tcno + " kişisinin " + iseGirisTarihi.ToString("dd.MM.yyyy") + " girişine teşvik verilmeyecek");

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            foreach (var kanun in tesvikTanimlanacakKanunlar)
                                                                                            {
                                                                                                int sayacKaydet = 0;

                                                                                            TesvikKaydet:

                                                                                                var responsekaydet = kisiWebClient.PostData("https://uyg.sgk.gov.tr/YeniSistem/Isveren/sigortaliKaydet.action", "donem_yil_ay_index=" + optionvalue + "&tcDonemSorgu=1&tcKimlikNo=" + islemYapilanTc + "&iseGirisTarihi=" + isegiristarihi + "&egitimDurumu=" + egitimDurumu + (string.IsNullOrEmpty(sigortaliBasvuruTarihi) ? "" : "&sigortaliBasvuruTarihi=" + sigortaliBasvuruTarihi + "&sigortaliDurum=1") + "&meslekKodu=" + meslekkodu + "&kanunNo=" + kanun + "&kazanc=" + kazanc_miktari + "%2C00&kolayIsveren=false&ucretDestegiTalebiVarMi=" + (UcretDestegiIstensinMi ? "true" : "false"));

                                                                                                var gecerliSayfaSorgusuKaydetYaniti = GecerliSayfaOlupOlmadiginiKontrolEt(responsekaydet, "Sıra Seçilecek", ref webclient, false);

                                                                                                if (gecerliSayfaSorgusuKaydetYaniti == Enums.GecerliSayfaSonuclari.Iptal) return;
                                                                                                else if (gecerliSayfaSorgusuKaydetYaniti == Enums.GecerliSayfaSonuclari.Gecersiz)
                                                                                                {
                                                                                                    sayacKaydet++;

                                                                                                    if (sayacKaydet < 20)
                                                                                                    {
                                                                                                        Metodlar.DetayliLogYaz(tcno + " kişisinin " + iseGirisTarihi.ToString("dd.MM.yyyy") + " girişinin hakettiği teşvikler kaydedilirken geçerli bir sayfa bulunamadı." + (sayacKaydet + 1) + ".kez denenecek");

                                                                                                        Thread.Sleep(1000);

                                                                                                        goto TesvikKaydet;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        BasvuruLogEkle(islemYapilanTc + " kişisinin " + iseGirisTarihi.ToString("dd.MM.yyyy") + " hakettiği teşvikler 20 kez ardarda kaydedilemedi.Sıradaki kişiye geçilecek", donemindex, kisiindex);

                                                                                                        Metodlar.DetayliLogYaz(islemYapilanTc + " kişisinin " + iseGirisTarihi.ToString("dd.MM.yyyy") + " hakettiği teşvikler 20 kez ardarda kaydedilemedi.Sıradaki kişiye geçilecek");
                                                                                                    }
                                                                                                }
                                                                                                else if (gecerliSayfaSorgusuKaydetYaniti == Enums.GecerliSayfaSonuclari.UzunSureliIslemYapilamadiUyarisi)
                                                                                                {
                                                                                                    if (!Program.KisiIslemcisiYeniGirisYapsin)
                                                                                                    {
                                                                                                        while (YenidenBaglaniliyor) { Thread.Sleep(200); }

                                                                                                        if (!YenidenBaglaniliyor)
                                                                                                        {
                                                                                                            YenidenBaglaniliyor = true;

                                                                                                            lock (DonemWebClient)
                                                                                                            {
                                                                                                                var wc = Program.DonemIslemcisiYeniGirisYapsin ? DonemWebClient : webclient;

                                                                                                                if (wc.oturumId.Equals(kisiWebClient.oturumId))
                                                                                                                {
                                                                                                                    wc.ReConnect();

                                                                                                                    DonemWebClient.Cookie = wc.Cookie;
                                                                                                                }

                                                                                                                kisiWebClient.Cookie = wc.Cookie;
                                                                                                            }
                                                                                                        }

                                                                                                        YenidenBaglaniliyor = false;
                                                                                                    }
                                                                                                    else kisiWebClient.Disconnect();

                                                                                                    lock (islemYapilanKisiler)
                                                                                                    {
                                                                                                        if (islemYapilanKisiler.Contains(tcNoveIseGirisTarihi))
                                                                                                        {
                                                                                                            islemYapilanKisiler.Remove(tcNoveIseGirisTarihi);
                                                                                                        }
                                                                                                    }

                                                                                                    TesvikTanimlamaSayfasiAcilacak = true;

                                                                                                    goto KisiEnBasaDon;
                                                                                                }
                                                                                                else if (gecerliSayfaSorgusuKaydetYaniti == Enums.GecerliSayfaSonuclari.Gecerli)
                                                                                                {
                                                                                                    bool yenidenDenenecek = false;

                                                                                                    //foreach (var kanun in tesvikTanimlanacakKanunlar)
                                                                                                    {
                                                                                                        if ((kanun.Equals("6111") && (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf6111))
                                                                                                                ||
                                                                                                                (kanun.EndsWith("7103") && (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf7103))
                                                                                                                ||
                                                                                                                (kanun.EndsWith("2828") && (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf2828))
                                                                                                                ||
                                                                                                                (kanun.EndsWith("7252") && (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf7252))
                                                                                                                ||
                                                                                                                (kanun.EndsWith("7256") && (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf7256))
                                                                                                                ||
                                                                                                                (kanun.EndsWith("7316") && (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf7316))
                                                                                                                ||
                                                                                                                (kanun.EndsWith("3294") && (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf3294))
                                                                                                        )
                                                                                                        {
                                                                                                            if (responsekaydet.Contains(kanun + ": NUMARALI KANUN TEŞVİK KAYIT İŞLEMİNİZ BAŞARILI"))
                                                                                                            {
                                                                                                                Mesaj = islemYapilanTc + " kişisine " + kanun + " tanımlaması yapıldı.";

                                                                                                                if (kanun.Equals("6111") && !egitimDurumu.Equals("1") && !egitimDurumu.Equals("2"))
                                                                                                                {
                                                                                                                    Mesaj += String.Format(" {0} seçilerek tanımlama yapıldı", EgitimBelgesiAdlari.EgitimBelgesiTurleriAdlari[Convert.ToInt32(egitimDurumu)]);
                                                                                                                }

                                                                                                                basariliSayisi++;

                                                                                                                TanimlamaYapilanlaraEkle(kanun, tcNoveIseGirisTarihi);

                                                                                                                BasvuruLogEkle(Mesaj, donemindex, kisiindex);
                                                                                                            }
                                                                                                            if (responsekaydet.Contains(kanun + ": İşyeri imalat ya da bilişim sektöründe olmadığından teşvikten faydalanılamaz"))
                                                                                                            {
                                                                                                                Mesaj = islemYapilanTc + " kişisine " + kanun + " tanımlaması yapılamadı. Hata: İşyeri imalat ya da bilişim sektöründe olmadığından teşvikten faydalanılamaz";

                                                                                                                basariliSayisi++;

                                                                                                                BasvuruLogEkle(Mesaj, donemindex, kisiindex);
                                                                                                            }
                                                                                                            else if (kanun.EndsWith("7256") && responsekaydet.Contains(String.Format("Bu sigortalı için {0} sayılı Kanun kapsamında tanımlama vardır", kanun)))
                                                                                                            {
                                                                                                                Mesaj = islemYapilanTc + " kişisine " + kanun + " tanımlaması daha önceden yapılmış";

                                                                                                                basariliSayisi++;

                                                                                                                TanimlamaYapilanlaraEkle(kanun, tcNoveIseGirisTarihi);

                                                                                                                BasvuruLogEkle(Mesaj, donemindex, kisiindex);
                                                                                                            }
                                                                                                            else if (kanun.EndsWith("7316") && responsekaydet.Contains(String.Format("Bu sigortalı için {0} sayılı Kanun kapsamında tanımlama vardır", kanun)))
                                                                                                            {
                                                                                                                Mesaj = islemYapilanTc + " kişisine " + kanun + " tanımlaması daha önceden yapılmış";

                                                                                                                basariliSayisi++;

                                                                                                                TanimlamaYapilanlaraEkle(kanun, tcNoveIseGirisTarihi);

                                                                                                                BasvuruLogEkle(Mesaj, donemindex, kisiindex);
                                                                                                            }
                                                                                                            else if (kanun.EndsWith("3294") && responsekaydet.Contains(String.Format("Bu sigortalı için {0} sayılı Kanun kapsamında tanımlama vardır", kanun)))
                                                                                                            {
                                                                                                                Mesaj = islemYapilanTc + " kişisine " + kanun + " tanımlaması daha önceden yapılmış";

                                                                                                                basariliSayisi++;

                                                                                                                TanimlamaYapilanlaraEkle(kanun, tcNoveIseGirisTarihi);

                                                                                                                BasvuruLogEkle(Mesaj, donemindex, kisiindex);
                                                                                                            }
                                                                                                            else if (responsekaydet.Contains(kanun + ": Sistem hatası oluştu"))
                                                                                                            {

                                                                                                                sistemHatasiYenidenDenemeSayisi++;

                                                                                                                if (sistemHatasiYenidenDenemeSayisi < 5 || SistemHatasiVarDiyalogMesajiSoruldu)
                                                                                                                {
                                                                                                                    Mesaj = islemYapilanTc + " kişisine " + kanun + " tanımlaması yapılırken sistem hatası oluştu uyarısı var." + sistemHatasiYenidenDenemeSayisi + ". deneme";

                                                                                                                    BasvuruLogEkle(Mesaj, donemindex, kisiindex);

                                                                                                                    Thread.Sleep(TimeSpan.FromMilliseconds(2000));

                                                                                                                    yenidenDenenecek = true;
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    //hatadanDolayiKisiAtlanacak = true;

                                                                                                                    BasvuruLogEkle(String.Format("{0} kişisinin {1} tarihli girişi 5 defa sorgulanmasına rağmen \"Sistemsel bir hata oluştu\" uyarısı ile karşılaşıldı", islemYapilanTc, tcNoveIseGirisTarihi.Value.ToString("dd.MM.yyyy")), donemindex, kisiindex, true);

                                                                                                                    if (!SistemHatasiVarDiyalogMesajiSoruldu)
                                                                                                                    {
                                                                                                                        SistemHatasiVarDiyalogMesajiSoruldu = true;

                                                                                                                        SistemHatasiVarDiyalogMesajiAcik = true;

                                                                                                                        var frmOnayEkrani = new frmOnay("\"Sistemsel bir hata oluştu\" uyarısı mevcut. Devam edilsin mi?");

                                                                                                                        var frmGostermeZamani = DateTime.Now;

                                                                                                                        bool SistemHatasiUyarisindenSonraDevamEdilsinMi = true;

                                                                                                                        bool bekle120Saniye = true;

                                                                                                                        var task120SaniyeBekle = Task.Run(() =>
                                                                                                                        {
                                                                                                                            while (bekle120Saniye)
                                                                                                                            {
                                                                                                                                if (DateTime.Now.Subtract(frmGostermeZamani).TotalSeconds >= 120)
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        if (frmOnayEkrani != null)
                                                                                                                                        {
                                                                                                                                            SistemHatasiUyarisindenSonraDevamEdilsinMi = true;

                                                                                                                                            frmOnayEkrani.Invoke((Action)(() =>
                                                                                                                                            {
                                                                                                                                                frmOnayEkrani.Kapat();
                                                                                                                                            }));
                                                                                                                                        }
                                                                                                                                        else break;
                                                                                                                                    }
                                                                                                                                    catch (Exception ex)
                                                                                                                                    {
                                                                                                                                        Debug.WriteLine(ex);
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    Thread.Sleep(1000);
                                                                                                                                }
                                                                                                                            }
                                                                                                                        });

                                                                                                                        SistemHatasiUyarisindenSonraDevamEdilsinMi = frmOnayEkrani.ShowDialog() == DialogResult.Yes;

                                                                                                                        bekle120Saniye = false;

                                                                                                                        SistemHatasiVarDiyalogMesajiAcik = false;

                                                                                                                        frmOnayEkrani = null;

                                                                                                                        if (!SistemHatasiUyarisindenSonraDevamEdilsinMi)
                                                                                                                        {
                                                                                                                            IslemiIptalEt();

                                                                                                                            return;
                                                                                                                        }
                                                                                                                        else yenidenDenenecek = true;

                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                yenidenDenenecek = true;

                                                                                                                Mesaj = islemYapilanTc + " kişisi " + kanun + " hakettiği halde tanımlaması yapılamadı. Tekrar denenecek";

                                                                                                                BasvuruLogEkle(Mesaj, donemindex, kisiindex, false);
                                                                                                            }

                                                                                                        }

                                                                                                    }

                                                                                                    if (yenidenDenenecek) goto yenidenDene;

                                                                                                }
                                                                                            }

                                                                                            if (basariliSayisi == tesvikTanimlanacakKanunlar.Count)
                                                                                            {
                                                                                                lock (islemiTamamlananKisiler)
                                                                                                {
                                                                                                    islemiTamamlananKisiler.Add(tcNoveIseGirisTarihi);
                                                                                                }
                                                                                            }
                                                                                        }

                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        Metodlar.DetayliLogYaz(tcno + " kişisinin teşvik sorgu sonuçları sayfası boş");
                                                                                    }
                                                                                }

                                                                                //lock (islemiTamamlananKisiler)
                                                                                //{
                                                                                //    islemiTamamlananKisiler.Add(tcNoveIseGirisTarihi);
                                                                                //}

                                                                            }
                                                                            else
                                                                            {
                                                                                BasvuruLogEkle(tcno + " kişisinin teşvik sorgusu 20 kez ardarda yapılamadı.Sıradaki kişiye geçilecek", donemindex, kisiindex);

                                                                                Metodlar.DetayliLogYaz(tcno + "kişisinin teşvik sorgusu 20 kez ardarda yapılamadı.Sıradaki kişiye geçilecek");
                                                                            }
                                                                        }
                                                                        else break;
                                                                    }

                                                                }
                                                                catch (OperationCanceledException)
                                                                {
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    throw ex;
                                                                }
                                                                finally
                                                                {
                                                                    if (Program.KisiIslemcisiYeniGirisYapsin)
                                                                    {
                                                                        kisiWebClient.Kullanimda = false;
                                                                    }
                                                                }


                                                            }, token);

                                                            kisiTasks.Add(taskKisi);
                                                        }

                                                        Task.WaitAll(kisiTasks.ToArray());

                                                    }

                                                }
                                                else
                                                {
                                                    BasvuruLogEkle(donem + " döneminde kayıtlı kişi bulunamadı", donemindex);

                                                    Metodlar.DetayliLogYaz(donem + " döneminde kayıtlı kişi yok");

                                                    if (!indirilenIsyeri.SonAyIseGirenlerCekildi)
                                                    {
                                                        if (Convert.ToDateTime(donem).Equals(oncekiAy))
                                                        {
                                                            lock (indirilenIsyeri.SonAyIseGirenlerListesi)
                                                            {
                                                                indirilenIsyeri.SonAyIseGirenlerCekildi = true;
                                                                indirilenIsyeri.SonAyIseGirenlerListesi.Clear();
                                                            }
                                                        }
                                                    }
                                                }

                                                if (AyIcindeKisiVar && Convert.ToDateTime(donem).Equals(oncekiAy))
                                                {
                                                    if (sonAyTekrarSayisi == 0)
                                                    {
                                                        sonAyTekrarSayisi++;

                                                        lock (islemiTamamlananKisiler)
                                                        {
                                                            islemiTamamlananKisiler.RemoveWhere(p => kisilerveIseGirisTarihleri.Contains(p));
                                                        }

                                                        lock (islemYapilanKisiler)
                                                        {
                                                            islemYapilanKisiler.RemoveWhere(p => kisilerveIseGirisTarihleri.Contains(p));
                                                        }

                                                        goto DonemKisileriGetir;
                                                    }
                                                }

                                                lock (islemiTamamlananDonemler)
                                                {
                                                    islemiTamamlananDonemler.Add(donem);

                                                    BasvuruLogEkle(donem + " dönemi tamamlandı", donemindex);

                                                    Metodlar.DetayliLogYaz(donem + " dönemi tamamlandı");
                                                }
                                            }
                                            else
                                            {
                                                BasvuruLogEkle(donem + " sorgusu 20 kez ardarda yapılamadı.Sıradaki döneme geçilecek", donemindex);

                                                Metodlar.DetayliLogYaz(donem + " sorgusu 20 kez ardarda yapılamadı.Sıradaki döneme geçilecek");
                                            }


                                        }
                                        else break;
                                    }

                                }
                                catch (OperationCanceledException) { }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                                finally
                                {
                                    if (Program.DonemIslemcisiYeniGirisYapsin)
                                    {
                                        DonemWebClient.Disconnect();
                                    }
                                }


                            }, token);

                            donemTasks.Add(task);
                        }

                        Task.WaitAll(donemTasks.ToArray());

                        if (Program.KisiIslemcisiYeniGirisYapsin)
                        {
                            allKisiWebClients.ForEach(p => p.Disconnect());
                        }
                    }

                    if (bfsira == Enums.BasvuruFormuTurleri.BfTumu || bfsira == Enums.BasvuruFormuTurleri.Bf6111)
                    {

                        siradakiIslem = "4447/GEÇİCİ 10.MADDE LİSTELEME/SİLME AÇILACAK";

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444710Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                    else if (bfsira == Enums.BasvuruFormuTurleri.Bf7103)
                    {

                        siradakiIslem = "4447/GEÇİCİ 19.MADDE LİSTELEME/SİLME AÇILACAK";

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444719Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                    else if (bfsira == Enums.BasvuruFormuTurleri.Bf2828)
                    {

                        siradakiIslem = "2828/EK 1.MADDE LİSTELEME/SİLME AÇILACAK";

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik44472828Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                    else if (bfsira == Enums.BasvuruFormuTurleri.Bf7252)
                    {

                        siradakiIslem = "4447/GEÇİCİ 26.MADDE LİSTELEME/SİLME AÇILACAK";

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444726Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                    else if (bfsira == Enums.BasvuruFormuTurleri.Bf7256)
                    {

                        siradakiIslem = "4447/GEÇİCİ 28.MADDE LİSTELEME/SİLME AÇILACAK";

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444728Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                    else if (bfsira == Enums.BasvuruFormuTurleri.Bf7316)
                    {

                        siradakiIslem = "4447/GEÇİCİ 30.MADDE LİSTELEME/SİLME AÇILACAK";

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444730Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                    else if (bfsira == Enums.BasvuruFormuTurleri.Bf3294)
                    {

                        siradakiIslem = "3294/SOSYAL YARDIM ALANLARIN İSTİHDAMI LİSTELEME/SİLME AÇILACAK";

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik3294Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                }

                #endregion

                #region LİSTENİN İNDİRİLME AŞAMALARI;

                if (siradakiIslem == "4447/GEÇİCİ 10.MADDE LİSTELEME/SİLME AÇILACAK")
                {
                    Mesaj = "6111 listesi indiriliyor";

                    BasvuruLogEkle(Mesaj);

                    Metodlar.DetayliLogYaz(Mesaj);

                    var pencereLinkIdYeni = html.GetElementbyId("pencereLinkIdYeni");
                    if (pencereLinkIdYeni != null)
                    {
                        siradakiIslem = "Kişi Listesi Alınacak 6111";
                        string newUrl = pencereLinkIdYeni.GetAttributeValue("src", "");

                        string response = webclient.Get(newUrl, string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;

                    }
                }

                if (siradakiIslem == "Kişi Listesi Alınacak 6111")
                {

                    var satirlar = html.DocumentNode.SelectNodes("//table[@class='paginated gradienttable']/tbody/tr");
                    if (satirlar != null)
                    {
                        if (basvurukisiler6111.Count == 0 || (basvurukisiler6111.Count > 0 && !satirlar.First().SelectSingleNode("td[1]/p/text()").GetInnerText().Equals("1")))
                        {

                            foreach (var satir in satirlar)
                            {
                                basvurukisiler6111.Add(new BasvuruKisiDownload6111
                                {
                                    TcKimlikNo = satir.SelectSingleNode("td[2]/p/text()").GetInnerText().Trim(),
                                    Sicil = satir.SelectSingleNode("td[3]/p/text()").GetInnerText().Trim(),
                                    Ad = satir.SelectSingleNode("td[4]/p/text()").GetInnerText().Trim(),
                                    Soyad = satir.SelectSingleNode("td[5]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresiBaslangic = satir.SelectSingleNode("td[6]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresiBitis = satir.SelectSingleNode("td[7]/p/text()").GetInnerText().Trim(),
                                    Baz = Convert.ToInt32(Regex.Replace(satir.SelectSingleNode("td[8]/p/text()").GetInnerText().Trim(), "[^0-9]", "")),
                                    GirisTarihi = satir.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim(),
                                    CikisTarihi = satir.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim(),
                                    Durum = satir.SelectSingleNode("td[12]/p/text()").GetInnerText().Trim()
                                });

                            }

                            var reg = Regex.Match(ResponseHtml, "toplamKayitSay = parseInt\\('(.*)'\\);");
                            if (reg.Success)
                            {
                                var toplamKayit = reg.Groups[1].Value.ToInt();

                                if (basvurukisiler6111.Count < toplamKayit)
                                {
                                    string responsesonraki = webclient.PostData("https://uyg.sgk.gov.tr/YeniSistem/ListelemManager/sonrakiSayfalarAction.action", "ilkKayitIleriGeriSonKayit=3&herSayfadakiSatirSay=20");

                                    YuklenenSayfaninIciniDolas(responsesonraki);

                                    return;
                                }
                            }
                        }

                        //kişi listesinin tümünü indirmek için tüm sayfalar gezilir.


                        TanimlamaYapilanlar6111.RemoveWhere(p => basvurukisiler6111.Any(x => p.Key.Equals(x.TcKimlikNo) && p.Value.Equals(Convert.ToDateTime(x.GirisTarihi))));

                        Mesaj = "6111 listesinden " + basvurukisiler6111.Count + " kişi kaydedildi";

                        Metodlar.DetayliLogYaz(Mesaj);
                    }
                    else
                    {
                        Mesaj = "6111 listesinde kaydedilecek kişi yok";
                    }


                    if (bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                    {
                        BasvuruLogEkle(Mesaj);

                        siradakiIslem = "4447/GEÇİCİ 19.MADDE LİSTELEME/SİLME AÇILACAK";

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444719Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                    else if (bfsira == Enums.BasvuruFormuTurleri.Bf6111)
                    {
                        BasvuruLogEkle(Mesaj);

                        siradakiIslem = "İşlem Tamamlandı";

                        webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/logout.jsp", "");
                    }
                }

                if (siradakiIslem == "4447/GEÇİCİ 19.MADDE LİSTELEME/SİLME AÇILACAK")
                {
                    Mesaj = "7103 listesi indiriliyor";

                    BasvuruLogEkle(Mesaj);

                    Metodlar.DetayliLogYaz(Mesaj);

                    var pencereLinkIdYeni = html.GetElementbyId("pencereLinkIdYeni");
                    if (pencereLinkIdYeni != null)
                    {
                        siradakiIslem = "Kişi Listesi Alınacak 7103";
                        string newUrl = pencereLinkIdYeni.GetAttributeValue("src", "");

                        string response = webclient.Get(newUrl, string.Empty);
                        YuklenenSayfaninIciniDolas(response);
                        return;
                    }
                }

                if (siradakiIslem == "Kişi Listesi Alınacak 7103")
                {

                    var satirlar = html.DocumentNode.SelectNodes("//table[@class='paginated gradienttable']/tbody/tr");
                    if (satirlar != null)
                    {
                        bool YeniSablon = satirlar.First().ParentNode.ParentNode.InnerText.Contains("Ücret Desteği Tercihi");

                        if (basvurukisiler7103.Count == 0 || (basvurukisiler7103.Count > 0 && !satirlar.First().SelectSingleNode("td[1]/p/text()").GetInnerText().Equals("1")))
                        {
                            foreach (var satir in satirlar)
                            {
                                basvurukisiler7103.Add(new BasvuruKisiDownload7103
                                {
                                    TcKimlikNo = satir.SelectSingleNode("td[2]/p/text()").GetInnerText().Trim(),
                                    Sicil = satir.SelectSingleNode("td[3]/p/text()").GetInnerText().Trim(),
                                    Ad = satir.SelectSingleNode("td[4]/p/text()").GetInnerText().Trim(),
                                    Soyad = satir.SelectSingleNode("td[5]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresiBaslangic = satir.SelectSingleNode("td[6]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresiBitis = satir.SelectSingleNode("td[7]/p/text()").GetInnerText().Trim(),
                                    Baz = Convert.ToInt32(Regex.Replace(satir.SelectSingleNode("td[8]/p/text()").GetInnerText().Trim(), "[^0-9]", "")),
                                    UcretDestegiTercihi = YeniSablon ? satir.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim() : string.Empty,
                                    PrimveUcretDestegiIcinBaslangicDonemi = YeniSablon ? satir.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim() : string.Empty,
                                    PrimveUcretDestegiIcinBitisDonemi = YeniSablon ? satir.SelectSingleNode("td[11]/p/text()").GetInnerText().Trim() : string.Empty,
                                    PrimveUcretDestegiIcinIlaveOlunacakSayi = YeniSablon ? satir.SelectSingleNode("td[12]/p/text()").GetInnerText().Trim() : string.Empty,
                                    KanunNo = YeniSablon ? satir.SelectSingleNode("td[13]/p/text()").GetInnerText().Trim() : satir.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim(),
                                    GirisTarihi = YeniSablon ? satir.SelectSingleNode("td[14]/p/text()").GetInnerText().Trim() : satir.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim(),
                                    CikisTarihi = YeniSablon ? satir.SelectSingleNode("td[15]/p/text()").GetInnerText().Trim() : satir.SelectSingleNode("td[11]/p/text()").GetInnerText().Trim(),
                                    IlkTanimlamaTarihi = YeniSablon ? satir.SelectSingleNode("td[16]/p/text()").GetInnerText().Trim() : satir.SelectSingleNode("td[12]/p/text()").GetInnerText()
                                }); ;
                            }

                            var reg = Regex.Match(ResponseHtml, "toplamKayitSay = parseInt\\('(.*)'\\);");
                            if (reg.Success)
                            {
                                var toplamKayit = reg.Groups[1].Value.ToInt();

                                if (basvurukisiler7103.Count < toplamKayit)
                                {
                                    string responsesonraki = webclient.PostData("https://uyg.sgk.gov.tr/YeniSistem/ListelemManager/sonrakiSayfalarAction.action", "ilkKayitIleriGeriSonKayit=3&herSayfadakiSatirSay=20");

                                    YuklenenSayfaninIciniDolas(responsesonraki);

                                    return;
                                }
                            }
                        }

                        TanimlamaYapilanlar7103.RemoveWhere(p => basvurukisiler7103.Any(x => p.Key.Equals(x.TcKimlikNo) && p.Value.Equals(Convert.ToDateTime(x.GirisTarihi))));

                        if (TanimlamaYapilanlar7103.Count == 0 || ListedeBulunmayanKisileriTekrarDenemeSayisi >= 4)
                        {
                            Dictionary<int, long> bazlar = new Dictionary<int, long>();

                            try
                            {
                                bazlar = basvurukisiler7103.Where(p => !string.IsNullOrEmpty(p.PrimveUcretDestegiIcinIlaveOlunacakSayi) && !p.PrimveUcretDestegiIcinIlaveOlunacakSayi.Contains("-") && !p.PrimveUcretDestegiIcinIlaveOlunacakSayi.Equals("Bilinmiyor")).GroupBy(x => Convert.ToDateTime(x.GirisTarihi).Year).ToDictionary(x => x.Key, x => Convert.ToInt64(x.FirstOrDefault().PrimveUcretDestegiIcinIlaveOlunacakSayi.Replace(".", "").Replace(",", "")));
                            }
                            catch
                            {
                                File.WriteAllText("7103_Hata.txt", Newtonsoft.Json.JsonConvert.SerializeObject(basvurukisiler7103));

                                MessageBox.Show("7103 listesi kaydedilirken hata meydana geldi. Lütfen program klasörü içindeki 7103_Hata ismindeki dosyayı yazılımcıya gönderiniz");
                            }

                            if (YeniSablon && Program.Liste7166Cikarilsin)
                            {
                                foreach (var kisi7103 in basvurukisiler7103)
                                {
                                    DateTime iseGirisTarihi = Convert.ToDateTime(kisi7103.GirisTarihi);
                                    DateTime cikisTarihi = DateTime.MinValue;
                                    if (!string.IsNullOrEmpty(kisi7103.CikisTarihi))
                                    {
                                        cikisTarihi = Convert.ToDateTime(kisi7103.CikisTarihi);
                                    }


                                    if (iseGirisTarihi >= Program.TumTesvikler["7166"].TesvikBaslamaZamani && iseGirisTarihi < new DateTime(2019, 5, 1))
                                    {

                                        DateTime yasakliCikisiIstenAyrilisTarihi = DateTime.MinValue;
                                        IstenCikisKaydi enYakinCikis = null;

                                        string istenCikisNedeni = "";

                                        bool YasakliCikisVarMi = false;

                                        bool iseGirisindenItibarenSonGarantiTariheKadarSureGecmis = true;

                                        var yasakliCikisinaBakilanKayit = YasakliCikisinaBakilanKisiler.FirstOrDefault(p => p.TcKimlikNo.Equals(kisi7103.TcKimlikNo) && p.iseGirisTarihi.Equals(iseGirisTarihi));

                                        if (yasakliCikisinaBakilanKayit == null)
                                        {
                                            var basvuruListe7166Kaydi = basvuruListesi7166Kisiler.FirstOrDefault(p => p.TckimlikNo.Equals(kisi7103.TcKimlikNo) && p.Giris.Equals(iseGirisTarihi) && p.UygunlukDurumuNedeni.Contains("Yasaklı koddan çıkış yapılmış"));

                                            if (basvuruListe7166Kaydi == null)
                                            {
                                                bool SigortaliIseGirisveAyrilisProjesindenSorulacak = true;

                                                if (SigortaliIseGirisveAyrilisProjesindenSorulacak)
                                                {
                                                    var sonTarih = iseGirisTarihi.AddMonths(Program.TumTesvikler["7166"].GirisTarihindenItibarenSuKadarAyIcindeIstenCikildiysaTesvikVerilmesin);

                                                    //10 gün geriye dönük işten ayrılış verilebileceği için 10,haftassonu için de 2 ekliyoruz. 1 de garanti olsun diye.
                                                    var sonTarihGarantiSinir = sonTarih.AddDays(10 + 2 + 1);

                                                    var istenCikisDenemeSayisiSayac = 0;

                                                IstenCikislariListele:

                                                    var yanit = sigortaliIstenAyrilisProjesiConnect.PostData("https://uyg.sgk.gov.tr/SigortaliTescil/amp/sigortaliTescilAction", "jobid=ayrilissorgulasonuc&tkrVno=&kimlikno=" + kisi7103.TcKimlikNo);

                                                    if (yanit.Contains("Sigortalı  İşten  Ayrılış Kayıtları"))
                                                    {
                                                        if (!yanit.Contains("İsten Ayrilis Kayidi Bulunmamaktadir"))
                                                        {
                                                            HtmlAgilityPack.HtmlDocument htmlyanit = new HtmlAgilityPack.HtmlDocument();
                                                            htmlyanit.LoadHtml(yanit);

                                                            var form1 = htmlyanit.GetElementbyId("form1");

                                                            if (form1 != null)
                                                            {
                                                                var table = form1.Descendants("table").FirstOrDefault();

                                                                if (table != null)
                                                                {
                                                                    var trs = table.Descendants("tr").Where(p => p.OuterHtml != null && p.OuterHtml.Contains("javascript:do_view"));

                                                                    List<IstenCikisKaydi> cikislar = new List<IstenCikisKaydi>();

                                                                    foreach (var tr in trs)
                                                                    {
                                                                        IstenCikisKaydi ick = new IstenCikisKaydi();
                                                                        ick.TcKimlikNo = kisi7103.TcKimlikNo;
                                                                        ick.iseGirisTarihi = iseGirisTarihi;

                                                                        var istenayrilistarihi = Convert.ToDateTime(tr.Descendants("td").ElementAt(1).InnerText.Trim());
                                                                        ick.istenCikisTarihi = istenayrilistarihi;

                                                                        ick.doViewNumber = Regex.Match(tr.OuterHtml, ".*do_view\\((\\d+)\\)").Groups[1].Value;

                                                                        cikislar.Add(ick);
                                                                    }

                                                                    Classes.IstenCikisKaydi ilkcikis = null;

                                                                    if (cikisTarihi != DateTime.MinValue)
                                                                    {
                                                                        ilkcikis = cikislar.OrderBy(p => p.istenCikisTarihi).FirstOrDefault(p => p.istenCikisTarihi.Equals(cikisTarihi) && p.istenCikisTarihi <= sonTarih);

                                                                    }
                                                                    else
                                                                    {
                                                                        ilkcikis = cikislar.OrderBy(p => p.istenCikisTarihi).FirstOrDefault(p => p.istenCikisTarihi >= iseGirisTarihi && p.istenCikisTarihi <= sonTarih);

                                                                    }

                                                                    enYakinCikis = cikislar.OrderBy(p => p.istenCikisTarihi).FirstOrDefault(p => p.istenCikisTarihi >= iseGirisTarihi);

                                                                    if (ilkcikis != null)
                                                                    {
                                                                        var istenCikisDenemeSayisi = 0;

                                                                    istenCikisNedeniBul:

                                                                        yanit = sigortaliIstenAyrilisProjesiConnect.PostData("https://uyg.sgk.gov.tr/SigortaliTescil/amp/sigortaliTescilAction", "jobid=reshow&tkrVno=" + ilkcikis.doViewNumber + "&kimlikno=");

                                                                        if (yanit.Contains("Sigortalının İşten Ayrılış Nedeni (Kodu)"))
                                                                        {
                                                                            htmlyanit.LoadHtml(yanit);

                                                                            var istencikisnedeni = Convert.ToInt32(htmlyanit.DocumentNode.Descendants("td").FirstOrDefault(td => td.InnerText != null && td.InnerText.Trim().Equals("Sigortalının İşten Ayrılış Nedeni (Kodu)")).NextSibling.InnerText).ToString();

                                                                            if (Program.TumTesvikler["7166"].IstenCikisYasakliKodlar.Contains(istencikisnedeni))
                                                                            {
                                                                                if (!YasakliCikisiOlanKisiler.Any(p => p.TcKimlikNo.Equals(kisi7103.TcKimlikNo) && p.iseGirisTarihi.Equals(iseGirisTarihi)))
                                                                                {
                                                                                    YasakliCikisiOlanKisiler.Add(ilkcikis);
                                                                                }

                                                                                YasakliCikisVarMi = true;

                                                                                yasakliCikisiIstenAyrilisTarihi = ilkcikis.istenCikisTarihi;

                                                                            }

                                                                            istenCikisNedeni = istencikisnedeni;
                                                                        }
                                                                        else
                                                                        {
                                                                            istenCikisDenemeSayisi++;

                                                                            BasvuruLogEkle("İşten çıkış sayfası yüklenemedi. Yeniden denenecek");

                                                                            if (istenCikisDenemeSayisi > 3)
                                                                            {
                                                                                BasvuruLogEkle("İşten çıkış sayfası yüklenemediğinden yeniden giriş yapılacak");
                                                                                sigortaliIstenAyrilisProjesiConnect.ReConnect();
                                                                                istenCikisDenemeSayisi = 0;
                                                                            }

                                                                            Thread.Sleep(1000);
                                                                            goto istenCikisNedeniBul;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (DateTime.Today <= sonTarihGarantiSinir)
                                                                        {
                                                                            iseGirisindenItibarenSonGarantiTariheKadarSureGecmis = false;
                                                                        }
                                                                    }

                                                                    if (!YasakliCikisinaBakilanKisiler.Any(p => p.TcKimlikNo.Equals(kisi7103.TcKimlikNo) && p.iseGirisTarihi.Equals(iseGirisTarihi)))
                                                                    {
                                                                        YasakliCikisinaBakilanKisiler.Add(new IstenCikisKaydi { TcKimlikNo = kisi7103.TcKimlikNo, iseGirisTarihi = iseGirisTarihi });
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (DateTime.Today <= sonTarihGarantiSinir)
                                                            {
                                                                iseGirisindenItibarenSonGarantiTariheKadarSureGecmis = false;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        istenCikisDenemeSayisiSayac++;

                                                        BasvuruLogEkle("İşten çıkış sayfası yüklenemedi. Yeniden denenecek");

                                                        if (istenCikisDenemeSayisiSayac > 3)
                                                        {
                                                            BasvuruLogEkle("İşten çıkış sayfası yüklenemediğinden yeniden giriş yapılacak");
                                                            sigortaliIstenAyrilisProjesiConnect.ReConnect();
                                                            istenCikisDenemeSayisiSayac = 0;
                                                        }

                                                        Thread.Sleep(1000);
                                                        goto IstenCikislariListele;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                YasakliCikisVarMi = true;

                                                if (!string.IsNullOrEmpty(basvuruListe7166Kaydi.Cikis))
                                                {
                                                    yasakliCikisiIstenAyrilisTarihi = Convert.ToDateTime(basvuruListe7166Kaydi.Cikis);
                                                    istenCikisNedeni = basvuruListe7166Kaydi.IstenCikisNedeni;
                                                }

                                            }
                                        }
                                        else
                                        {
                                            var yasakliCikisiOlanKayit = YasakliCikisiOlanKisiler.FirstOrDefault(p => p.TcKimlikNo.Equals(kisi7103.TcKimlikNo) && p.iseGirisTarihi.Equals(iseGirisTarihi));

                                            if (yasakliCikisiOlanKayit != null)
                                            {
                                                YasakliCikisVarMi = true;
                                                yasakliCikisiIstenAyrilisTarihi = yasakliCikisiOlanKayit.istenCikisTarihi;
                                                istenCikisNedeni = yasakliCikisiOlanKayit.istenCikisNedeni;
                                            }
                                            else istenCikisNedeni = yasakliCikisinaBakilanKayit.istenCikisNedeni;

                                        }

                                        if (!BasvuruListesi7166yaEklenecekKisiler.Any(p => p.TckimlikNo.Equals(kisi7103.TcKimlikNo) && p.Giris.Equals(iseGirisTarihi)))
                                        {
                                            if (TumKisilerSonuc == null)
                                            {
                                                if (dtMevcutAphb != null)
                                                {
                                                    var TesvikVerilenler = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new List<string>());

                                                    List<KeyValuePair<string, string>> yilveaylar = new List<KeyValuePair<string, string>>();

                                                    Dictionary<KeyValuePair<string, string>, List<KeyValuePair<int, int>>> tumyilveaylar = new Dictionary<KeyValuePair<string, string>, List<KeyValuePair<int, int>>>();

                                                    DateTime enbuyukay = DateTime.MinValue;

                                                    TumKisilerSonuc = Metodlar.TumKisileriGetir(dtMevcutAphb);
                                                }
                                            }

                                            bool BazaTakiliyor = false;

                                            if (TumKisilerSonuc.TumKisiler != null)
                                            {
                                                bool enAzBirAydaBazaTakilmiyor = false;

                                                for (int i = 0; i < 3; i++)
                                                {
                                                    DateTime tarih = iseGirisTarihi.AddMonths(i);
                                                    tarih = new DateTime(tarih.Year, tarih.Month, 1);

                                                    if (!AyCalisanSayilari.ContainsKey(tarih))
                                                    {
                                                        Metodlar.AylikCalisanHesapla(tarih.Year.ToString(), tarih.Month.ToString(), TumKisilerSonuc, ref AyCalisanSayilari, ref AyCalisanSayilariBazHesaplama);
                                                    }

                                                    var aydaCalisanSayisi7166 = AyCalisanSayilari[tarih]["7166"];

                                                    if (!bazlar.ContainsKey(iseGirisTarihi.Year))
                                                    {
                                                        bazlar.Add(iseGirisTarihi.Year, Metodlar.BazHesapla(tarih.Year, tarih.Month, "7166", TumKisilerSonuc, ref AyCalisanSayilari, ref AyCalisanSayilariBazHesaplama));
                                                    }

                                                    var hesaplananBaz = bazlar[iseGirisTarihi.Year];

                                                    if (aydaCalisanSayisi7166 > -1 && hesaplananBaz > -1)
                                                    {

                                                        if (aydaCalisanSayisi7166 > hesaplananBaz)
                                                        {
                                                            enAzBirAydaBazaTakilmiyor = true;

                                                            break;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        enAzBirAydaBazaTakilmiyor = true;

                                                        break;
                                                    }
                                                }

                                                if (!enAzBirAydaBazaTakilmiyor) BazaTakiliyor = true;
                                            }

                                            string uygunlukDurumuNedeni = string.Empty;
                                            if (YasakliCikisVarMi) uygunlukDurumuNedeni += "Yasaklı koddan çıkış yapılmış";
                                            if (BazaTakiliyor) uygunlukDurumuNedeni += ", Baza takılıyor";

                                            var tesvikBitis = Convert.ToDateTime(kisi7103.TesvikSuresiBitis);

                                            var son7103Tarihi = enYakinCikis != null ? enYakinCikis.istenCikisTarihi < tesvikBitis ? enYakinCikis.istenCikisTarihi : tesvikBitis : tesvikBitis;
                                            var baslangic7103Tarihi = new DateTime(iseGirisTarihi.Year, iseGirisTarihi.Month, 1);

                                            var Varmi7103 = false;

                                            if (TumKisilerSonuc.KisilerinSatirlari != null && TumKisilerSonuc.KisilerinSatirlari.ContainsKey(kisi7103.TcKimlikNo))
                                            {
                                                var kisiaylar = TumKisilerSonuc.KisilerinSatirlari[kisi7103.TcKimlikNo];

                                                foreach (var item in kisiaylar.Values)
                                                {
                                                    foreach (var item2 in item)
                                                    {
                                                        var kanun = item2[(int)Enums.AphbHucreBilgileri.Kanun].ToString();

                                                        if (kanun.EndsWith("7103"))
                                                        {
                                                            var satirYil = Convert.ToInt32(item2[(int)Enums.AphbHucreBilgileri.Yil]);
                                                            var satirAy = Convert.ToInt32(item2[(int)Enums.AphbHucreBilgileri.Ay]);

                                                            var trh = new DateTime(satirYil, satirAy, 1);

                                                            if (trh >= baslangic7103Tarihi && trh <= son7103Tarihi)
                                                            {
                                                                Varmi7103 = true;
                                                                break;
                                                            }
                                                        }

                                                    }

                                                    if (Varmi7103) break;
                                                }
                                            }

                                            BasvuruListesi7166yaEklenecekKisiler.Add(new BasvuruListesi7166Kisi
                                            {
                                                Ad = kisi7103.Ad,
                                                Soyad = kisi7103.Soyad,
                                                TckimlikNo = kisi7103.TcKimlikNo,
                                                Giris = iseGirisTarihi,
                                                VerilmisMi7103 = Varmi7103,
                                                UygunlukDurumu = !YasakliCikisVarMi && !BazaTakiliyor ? (iseGirisindenItibarenSonGarantiTariheKadarSureGecmis ? "Uygundur" : "") : "Uygun Değildir",
                                                UygunlukDurumuNedeni = uygunlukDurumuNedeni.Trim(',').Trim(),
                                                Cikis = yasakliCikisiIstenAyrilisTarihi > DateTime.MinValue ? yasakliCikisiIstenAyrilisTarihi.ToString("dd.MM.yyyy") : !string.IsNullOrEmpty(kisi7103.CikisTarihi) ? kisi7103.CikisTarihi : "",
                                                IstenCikisNedeni = istenCikisNedeni
                                            });
                                        }
                                    }
                                }
                            }
                        }

                        Mesaj = "7103 listesinden " + basvurukisiler7103.Count + " kişi kaydedildi";

                        Metodlar.DetayliLogYaz(Mesaj);

                    }
                    else
                    {
                        Mesaj = "7103 listesinde kaydedilecek kişi yok";
                    }


                    if (bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                    {
                        BasvuruLogEkle(Mesaj);

                        siradakiIslem = "2828/EK 1.MADDE LİSTELEME/SİLME AÇILACAK";

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik44472828Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                    else if (bfsira == Enums.BasvuruFormuTurleri.Bf7103)
                    {
                        BasvuruLogEkle(Mesaj);

                        siradakiIslem = "İşlem Tamamlandı";

                        webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/logout.jsp", "");
                    }
                }

                if (siradakiIslem == "2828/EK 1.MADDE LİSTELEME/SİLME AÇILACAK")
                {
                    Mesaj = "2828 listesi indiriliyor";

                    BasvuruLogEkle(Mesaj);

                    Metodlar.DetayliLogYaz(Mesaj);


                    var pencereLinkIdYeni = html.GetElementbyId("pencereLinkIdYeni");
                    if (pencereLinkIdYeni != null)
                    {
                        siradakiIslem = "Kişi Listesi Alınacak 2828";
                        string newUrl = pencereLinkIdYeni.GetAttributeValue("src", "");

                        string response = webclient.Get(newUrl, string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                }

                if (siradakiIslem == "Kişi Listesi Alınacak 2828")
                {
                    var satirlar = html.DocumentNode.SelectNodes("//table[@class='paginated gradienttable']/tbody/tr");
                    if (satirlar != null)
                    {
                        if (basvurukisiler2828.Count == 0 || (basvurukisiler2828.Count > 0 && !satirlar.First().SelectSingleNode("td[1]/p/text()").GetInnerText().Equals("1")))
                        {


                            foreach (var satir in satirlar)
                            {

                                basvurukisiler2828.Add(new BasvuruKisiDownload2828
                                {
                                    TcKimlikNo = satir.SelectSingleNode("td[2]/p/text()").GetInnerText().Trim(),
                                    Sicil = satir.SelectSingleNode("td[3]/p/text()").GetInnerText().Trim(),
                                    Ad = satir.SelectSingleNode("td[4]/p/text()").GetInnerText().Trim(),
                                    Soyad = satir.SelectSingleNode("td[5]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresiBaslangic = satir.SelectSingleNode("td[6]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresiBitis = satir.SelectSingleNode("td[7]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresi = satir.SelectSingleNode("td[8]/p/text()").GetInnerText().Trim(),
                                    GirisTarihi = satir.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim(),
                                    CikisTarihi = satir.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim(),
                                    IlkTanimlamaTarihi = satir.SelectSingleNode("td[11]/p/text()").GetInnerText().Trim()
                                });


                            }

                            var reg = Regex.Match(ResponseHtml, "toplamKayitSay = parseInt\\('(.*)'\\);");
                            if (reg.Success)
                            {
                                var toplamKayit = reg.Groups[1].Value.ToInt();

                                if (basvurukisiler2828.Count < toplamKayit)
                                {
                                    string responsesonraki = webclient.PostData("https://uyg.sgk.gov.tr/YeniSistem/ListelemManager/sonrakiSayfalarAction.action", "ilkKayitIleriGeriSonKayit=3&herSayfadakiSatirSay=20");

                                    YuklenenSayfaninIciniDolas(responsesonraki);

                                    return;
                                }
                            }
                        }

                        TanimlamaYapilanlar2828.RemoveWhere(p => basvurukisiler2828.Any(x => p.Key.Equals(x.TcKimlikNo) && p.Value.Equals(Convert.ToDateTime(x.GirisTarihi))));

                        Mesaj = "2828 listesinden " + basvurukisiler2828.Count + " kişi kaydedildi";
                    }
                    else
                    {
                        Mesaj = "2828 listesinde kaydedilecek kişi yok";
                    }


                    if (bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                    {
                        BasvuruLogEkle(Mesaj);

                        Metodlar.DetayliLogYaz(Mesaj);

                        string response = null;

                        if (CariTanimla)
                        {
                            siradakiIslem = "3294/SOSYAL YARDIM ALANLARIN İSTİHDAMI LİSTELEME/SİLME AÇILACAK";

                            response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik3294Liste.action;", string.Empty);
                        }
                        else
                        {
                            siradakiIslem = "4447/GEÇİCİ 26.MADDE LİSTELEME/SİLME AÇILACAK";

                            response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444726Liste.action;", string.Empty);
                        }

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                    else if (bfsira == Enums.BasvuruFormuTurleri.Bf2828)
                    {
                        BasvuruLogEkle(Mesaj);

                        siradakiIslem = "İşlem Tamamlandı";

                        webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/logout.jsp", "");
                    }
                }

                if (siradakiIslem == "4447/GEÇİCİ 26.MADDE LİSTELEME/SİLME AÇILACAK")
                {
                    Mesaj = "7252 listesi indiriliyor";

                    BasvuruLogEkle(Mesaj);

                    Metodlar.DetayliLogYaz(Mesaj);


                    var pencereLinkIdYeni = html.GetElementbyId("pencereLinkIdYeni");
                    if (pencereLinkIdYeni != null)
                    {
                        siradakiIslem = "Kişi Listesi Alınacak 7252";
                        string newUrl = pencereLinkIdYeni.GetAttributeValue("src", "");

                        string response = webclient.Get(newUrl, string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                }

                if (siradakiIslem == "Kişi Listesi Alınacak 7252")
                {
                    var satirlar = html.DocumentNode.SelectNodes("//table[@class='paginated gradienttable']/tbody/tr");
                    if (satirlar != null)
                    {
                        if (basvurukisiler7252.Count == 0 || (basvurukisiler7252.Count > 0 && !satirlar.First().SelectSingleNode("td[1]/p/text()").GetInnerText().Equals("1")))
                        {


                            foreach (var satir in satirlar)
                            {

                                basvurukisiler7252.Add(new BasvuruKisiDownload7252
                                {
                                    TcKimlikNo = satir.SelectSingleNode("td[2]/p/text()").GetInnerText().Trim(),
                                    Sicil = satir.SelectSingleNode("td[3]/p/text()").GetInnerText().Trim(),
                                    Ad = satir.SelectSingleNode("td[4]/p/text()").GetInnerText().Trim(),
                                    Soyad = satir.SelectSingleNode("td[5]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresiBaslangic = satir.SelectSingleNode("td[6]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresiBitis = satir.SelectSingleNode("td[7]/p/text()").GetInnerText().Trim(),
                                    KCONUDSonlanmaTarihi = satir.SelectSingleNode("td[8]/p/text()").GetInnerText().Trim(),
                                    OrtalamaGunSayisi = satir.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim(),
                                    KanunNumarası = satir.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim(),
                                    IseGirisTarihi = satir.SelectSingleNode("td[11]/p/text()").GetInnerText().Trim(),
                                    IstenAyrilisTarihi = satir.SelectSingleNode("td[12]/p/text()").GetInnerText().Trim(),
                                    IlkTanimlamaTarihi = satir.SelectSingleNode("td[13]/p/text()").GetInnerText().Trim()
                                });


                            }

                            var reg = Regex.Match(ResponseHtml, "toplamKayitSay = parseInt\\('(.*)'\\);");
                            if (reg.Success)
                            {
                                var toplamKayit = reg.Groups[1].Value.ToInt();

                                if (basvurukisiler7252.Count < toplamKayit)
                                {
                                    string responsesonraki = webclient.PostData("https://uyg.sgk.gov.tr/YeniSistem/ListelemManager/sonrakiSayfalarAction.action", "ilkKayitIleriGeriSonKayit=3&herSayfadakiSatirSay=20");

                                    YuklenenSayfaninIciniDolas(responsesonraki);

                                    return;
                                }
                            }
                        }

                        TanimlamaYapilanlar7252.RemoveWhere(p => basvurukisiler7252.Any(x => p.Key.Equals(x.TcKimlikNo) && p.Value.Equals(Convert.ToDateTime(x.IseGirisTarihi))));

                        Mesaj = "7252 listesinden " + basvurukisiler7252.Count + " kişi kaydedildi";
                    }
                    else
                    {
                        Mesaj = "7252 listesinde kaydedilecek kişi yok";
                    }

                    if (bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                    {
                        BasvuruLogEkle(Mesaj);

                        Metodlar.DetayliLogYaz(Mesaj);

                        //siradakiIslem = "4447/GEÇİCİ 27.MADDE LİSTELEME/SİLME AÇILACAK";

                        //string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444727Liste.action;", string.Empty);

                        siradakiIslem = "4447/GEÇİCİ 28.MADDE LİSTELEME/SİLME AÇILACAK";

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444728Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                    else if (bfsira == Enums.BasvuruFormuTurleri.Bf7252)
                    {
                        BasvuruLogEkle(Mesaj);

                        siradakiIslem = "İşlem Tamamlandı";

                        webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/logout.jsp", "");
                    }
                }

                if (siradakiIslem == "4447/GEÇİCİ 27.MADDE LİSTELEME/SİLME AÇILACAK")
                {
                    Mesaj = "17256 listesi indiriliyor";

                    BasvuruLogEkle(Mesaj);

                    Metodlar.DetayliLogYaz(Mesaj);


                    var pencereLinkIdYeni = html.GetElementbyId("pencereLinkIdYeni");
                    if (pencereLinkIdYeni != null)
                    {
                        siradakiIslem = "Kişi Listesi Alınacak 17256";
                        string newUrl = pencereLinkIdYeni.GetAttributeValue("src", "");

                        string response = webclient.Get(newUrl, string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                }

                if (siradakiIslem == "Kişi Listesi Alınacak 17256")
                {
                    var satirlar = html.DocumentNode.SelectNodes("//table[@class='paginated gradienttable']/tbody/tr");
                    if (satirlar != null)
                    {
                        if (basvurukisiler17256.Count == 0 || (basvurukisiler17256.Count > 0 && !satirlar.First().SelectSingleNode("td[1]/p/text()").GetInnerText().Equals("1")))
                        {


                            foreach (var satir in satirlar)
                            {

                                basvurukisiler17256.Add(new BasvuruKisiDownload17256
                                {
                                    TcKimlikNo = satir.SelectSingleNode("td[2]/p/text()").GetInnerText().Trim(),
                                    Sicil = satir.SelectSingleNode("td[3]/p/text()").GetInnerText().Trim(),
                                    Ad = satir.SelectSingleNode("td[4]/p/text()").GetInnerText().Trim(),
                                    Soyad = satir.SelectSingleNode("td[5]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresiBaslangic = satir.SelectSingleNode("td[6]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresiBitis = satir.SelectSingleNode("td[7]/p/text()").GetInnerText().Trim(),
                                    KanunNumarası = satir.SelectSingleNode("td[8]/p/text()").GetInnerText().Trim(),
                                    IseGirisTarihi = satir.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim(),
                                    IstenAyrilisTarihi = satir.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim(),
                                    SigortalininIsyerineBasvuruTarihi = satir.SelectSingleNode("td[11]/p/text()").GetInnerText().Trim(),
                                    SigortaliIcinTercihDurumu = satir.SelectSingleNode("td[12]/p/span/text()").GetInnerText().Trim(),
                                    IlkTanimlamaTarihi = satir.SelectSingleNode("td[13]/p/text()").GetInnerText().Trim()
                                });


                            }

                            var reg = Regex.Match(ResponseHtml, "toplamKayitSay = parseInt\\('(.*)'\\);");
                            if (reg.Success)
                            {
                                var toplamKayit = reg.Groups[1].Value.ToInt();

                                if (basvurukisiler17256.Count < toplamKayit)
                                {
                                    string responsesonraki = webclient.PostData("https://uyg.sgk.gov.tr/YeniSistem/ListelemManager/sonrakiSayfalarAction.action", "ilkKayitIleriGeriSonKayit=3&herSayfadakiSatirSay=20");

                                    YuklenenSayfaninIciniDolas(responsesonraki);

                                    return;
                                }
                            }

                        }

                        TanimlamaYapilanlar17256.RemoveWhere(p => basvurukisiler17256.Any(x => p.Key.Equals(x.TcKimlikNo) && p.Value.Equals(Convert.ToDateTime(x.IseGirisTarihi))));

                        Mesaj = "17256 listesinden " + basvurukisiler17256.Count + " kişi kaydedildi";
                    }
                    else
                    {
                        Mesaj = "17256 listesinde kaydedilecek kişi yok";
                    }

                    BasvuruLogEkle(Mesaj);

                    Metodlar.DetayliLogYaz(Mesaj);

                    siradakiIslem = "4447/GEÇİCİ 28.MADDE LİSTELEME/SİLME AÇILACAK";

                    string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444728Liste.action;", string.Empty);

                    YuklenenSayfaninIciniDolas(response);

                    return;


                }

                if (siradakiIslem == "4447/GEÇİCİ 28.MADDE LİSTELEME/SİLME AÇILACAK")
                {
                    Mesaj = "27256 listesi indiriliyor";

                    BasvuruLogEkle(Mesaj);

                    Metodlar.DetayliLogYaz(Mesaj);

                    var pencereLinkIdYeni = html.GetElementbyId("pencereLinkIdYeni");
                    if (pencereLinkIdYeni != null)
                    {
                        siradakiIslem = "Kişi Listesi Alınacak 27256";
                        string newUrl = pencereLinkIdYeni.GetAttributeValue("src", "");

                        string response = webclient.Get(newUrl, string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                }

                if (siradakiIslem == "Kişi Listesi Alınacak 27256")
                {
                    var satirlar = html.DocumentNode.SelectNodes("//table[@class='paginated gradienttable']/tbody/tr");
                    if (satirlar != null)
                    {
                        if (basvurukisiler27256.Count == 0 || (basvurukisiler27256.Count > 0 && !satirlar.First().SelectSingleNode("td[1]/p/text()").GetInnerText().Equals("1")))
                        {


                            foreach (var satir in satirlar)
                            {

                                basvurukisiler27256.Add(new BasvuruKisiDownload27256
                                {
                                    TcKimlikNo = satir.SelectSingleNode("td[2]/p/text()").GetInnerText().Trim(),
                                    Sicil = satir.SelectSingleNode("td[3]/p/text()").GetInnerText().Trim(),
                                    Ad = satir.SelectSingleNode("td[4]/p/text()").GetInnerText().Trim(),
                                    Soyad = satir.SelectSingleNode("td[5]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresiBaslangic = satir.SelectSingleNode("td[6]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresiBitis = satir.SelectSingleNode("td[7]/p/text()").GetInnerText().Trim(),
                                    IlaveOlunmasiGerekenSayi = satir.SelectSingleNode("td[8]/p/text()").GetInnerText().Trim(),
                                    KanunNumarası = satir.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim(),
                                    IseGirisTarihi = satir.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim(),
                                    IstenAyrilisTarihi = satir.SelectSingleNode("td[11]/p/text()").GetInnerText().Trim(),
                                    IlkTanimlamaTarihi = satir.SelectSingleNode("td[12]/p/text()").GetInnerText().Trim()
                                });


                            }

                            var reg = Regex.Match(ResponseHtml, "toplamKayitSay = parseInt\\('(.*)'\\);");
                            if (reg.Success)
                            {
                                var toplamKayit = reg.Groups[1].Value.ToInt();

                                if (basvurukisiler27256.Count < toplamKayit)
                                {
                                    string responsesonraki = webclient.PostData("https://uyg.sgk.gov.tr/YeniSistem/ListelemManager/sonrakiSayfalarAction.action", "ilkKayitIleriGeriSonKayit=3&herSayfadakiSatirSay=20");

                                    YuklenenSayfaninIciniDolas(responsesonraki);

                                    return;
                                }
                            }


                        }

                        TanimlamaYapilanlar27256.RemoveWhere(p => basvurukisiler27256.Any(x => p.Key.Equals(x.TcKimlikNo) && p.Value.Equals(Convert.ToDateTime(x.IseGirisTarihi))));

                        Mesaj = "27256 listesinden " + basvurukisiler27256.Count + " kişi kaydedildi";
                    }
                    else
                    {
                        Mesaj = "27256 listesinde kaydedilecek kişi yok";
                    }


                    BasvuruLogEkle(Mesaj);

                    Metodlar.DetayliLogYaz(Mesaj);

                    if (bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                    {
                        string response = null;

                        if (CariTanimla)
                        {
                            siradakiIslem = "3294/SOSYAL YARDIM ALANLARIN İSTİHDAMI LİSTELEME/SİLME AÇILACAK";

                            response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik3294Liste.action;", string.Empty);
                        }
                        else
                        {
                            siradakiIslem = "4447/GEÇİCİ 30.MADDE LİSTELEME/SİLME AÇILACAK";

                            response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444730Liste.action;", string.Empty);
                        }



                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                    else if (bfsira == Enums.BasvuruFormuTurleri.Bf7256)
                    {

                        siradakiIslem = "İşlem Tamamlandı";

                        webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/logout.jsp", "");
                    }

                }

                if (siradakiIslem == "4447/GEÇİCİ 30.MADDE LİSTELEME/SİLME AÇILACAK")
                {
                    Mesaj = "7316 listesi indiriliyor";

                    BasvuruLogEkle(Mesaj);

                    Metodlar.DetayliLogYaz(Mesaj);

                    var pencereLinkIdYeni = html.GetElementbyId("pencereLinkIdYeni");
                    if (pencereLinkIdYeni != null)
                    {
                        siradakiIslem = "Kişi Listesi Alınacak 7316";

                        string newUrl = pencereLinkIdYeni.GetAttributeValue("src", "");

                        string response = webclient.Get(newUrl, string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                }


                if (siradakiIslem == "Kişi Listesi Alınacak 7316")
                {
                    var satirlar = html.DocumentNode.SelectNodes("//table[@class='paginated gradienttable']/tbody/tr");
                    if (satirlar != null)
                    {
                        if (basvurukisiler7316.Count == 0 || (basvurukisiler7316.Count > 0 && !satirlar.First().SelectSingleNode("td[1]/p/text()").GetInnerText().Equals("1")))
                        {


                            foreach (var satir in satirlar)
                            {

                                basvurukisiler7316.Add(new BasvuruKisiDownload7316
                                {
                                    TcKimlikNo = satir.SelectSingleNode("td[2]/p/text()").GetInnerText().Trim(),
                                    Sicil = satir.SelectSingleNode("td[3]/p/text()").GetInnerText().Trim(),
                                    Ad = satir.SelectSingleNode("td[4]/p/text()").GetInnerText().Trim(),
                                    Soyad = satir.SelectSingleNode("td[5]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresiBaslangic = satir.SelectSingleNode("td[6]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresiBitis = satir.SelectSingleNode("td[7]/p/text()").GetInnerText().Trim(),
                                    KanunNumarası = satir.SelectSingleNode("td[8]/p/text()").GetInnerText().Trim(),
                                    IseGirisTarihi = satir.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim(),
                                    IstenAyrilisTarihi = satir.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim(),
                                    IlkTanimlamaTarihi = satir.SelectSingleNode("td[11]/p/text()").GetInnerText().Trim()
                                });


                            }

                            var reg = Regex.Match(ResponseHtml, "toplamKayitSay = parseInt\\('(.*)'\\);");
                            if (reg.Success)
                            {
                                var toplamKayit = reg.Groups[1].Value.ToInt();

                                if (basvurukisiler7316.Count < toplamKayit)
                                {
                                    string responsesonraki = webclient.PostData("https://uyg.sgk.gov.tr/YeniSistem/ListelemManager/sonrakiSayfalarAction.action", "ilkKayitIleriGeriSonKayit=3&herSayfadakiSatirSay=20");

                                    YuklenenSayfaninIciniDolas(responsesonraki);

                                    return;
                                }
                            }

                        }

                        TanimlamaYapilanlar7316.RemoveWhere(p => basvurukisiler7316.Any(x => p.Key.Equals(x.TcKimlikNo) && p.Value.Equals(Convert.ToDateTime(x.IseGirisTarihi))));

                        Mesaj = "7316 listesinden " + basvurukisiler7316.Count + " kişi kaydedildi";
                    }
                    else
                    {
                        Mesaj = "7316 listesinde kaydedilecek kişi yok";
                    }

                    BasvuruLogEkle(Mesaj);

                    Metodlar.DetayliLogYaz(Mesaj);

                    if (bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                    {
                        siradakiIslem = "3294/SOSYAL YARDIM ALANLARIN İSTİHDAMI LİSTELEME/SİLME AÇILACAK";

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik3294Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                    else if (bfsira == Enums.BasvuruFormuTurleri.Bf7316)
                    {
                        siradakiIslem = "İşlem Tamamlandı";

                        webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/logout.jsp", "");
                    }
                }

                if (siradakiIslem == "3294/SOSYAL YARDIM ALANLARIN İSTİHDAMI LİSTELEME/SİLME AÇILACAK")
                {
                    Mesaj = "3294 listesi indiriliyor";

                    BasvuruLogEkle(Mesaj);

                    Metodlar.DetayliLogYaz(Mesaj);

                    var pencereLinkIdYeni = html.GetElementbyId("pencereLinkIdYeni");
                    if (pencereLinkIdYeni != null)
                    {
                        siradakiIslem = "Kişi Listesi Alınacak 3294";
                        string newUrl = pencereLinkIdYeni.GetAttributeValue("src", "");

                        string response = webclient.Get(newUrl, string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return;
                    }
                }

                if (siradakiIslem == "Kişi Listesi Alınacak 3294")
                {
                    var satirlar = html.DocumentNode.SelectNodes("//table[@class='paginated gradienttable']/tbody/tr");
                    if (satirlar != null)
                    {
                        if (basvurukisiler3294.Count == 0 || (basvurukisiler3294.Count > 0 && !satirlar.First().SelectSingleNode("td[1]/p/text()").GetInnerText().Equals("1")))
                        {
                            foreach (var satir in satirlar)
                            {

                                basvurukisiler3294.Add(new BasvuruKisiDownload3294
                                {
                                    TcKimlikNo = satir.SelectSingleNode("td[2]/p/text()").GetInnerText().Trim(),
                                    Sicil = satir.SelectSingleNode("td[3]/p/text()").GetInnerText().Trim(),
                                    Ad = satir.SelectSingleNode("td[4]/p/text()").GetInnerText().Trim(),
                                    Soyad = satir.SelectSingleNode("td[5]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresiBaslangic = satir.SelectSingleNode("td[6]/p/text()").GetInnerText().Trim(),
                                    TesvikSuresiBitis = satir.SelectSingleNode("td[7]/p/text()").GetInnerText().Trim(),
                                    OrtalamaSigortaliSayisi = satir.SelectSingleNode("td[8]/p/text()").GetInnerText().Trim(),
                                    IseGirisTarihi = satir.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim(),
                                    IstenAyrilisTarihi = satir.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim(),
                                    IlkTanimlamaTarihi = satir.SelectSingleNode("td[11]/p/text()").GetInnerText().Trim()
                                });


                            }

                            var reg = Regex.Match(ResponseHtml, "toplamKayitSay = parseInt\\('(.*)'\\);");
                            if (reg.Success)
                            {
                                var toplamKayit = reg.Groups[1].Value.ToInt();

                                if (basvurukisiler3294.Count < toplamKayit)
                                {
                                    string responsesonraki = webclient.PostData("https://uyg.sgk.gov.tr/YeniSistem/ListelemManager/sonrakiSayfalarAction.action", "ilkKayitIleriGeriSonKayit=3&herSayfadakiSatirSay=20");

                                    YuklenenSayfaninIciniDolas(responsesonraki);

                                    return;
                                }
                            }
                        }

                        TanimlamaYapilanlar3294.RemoveWhere(p => basvurukisiler3294.Any(x => p.Key.Equals(x.TcKimlikNo) && p.Value.Equals(Convert.ToDateTime(x.IseGirisTarihi))));

                        Mesaj = "3294 listesinden " + basvurukisiler3294.Count + " kişi kaydedildi";
                    }
                    else
                    {
                        Mesaj = "3294 listesinde kaydedilecek kişi yok";
                    }

                    BasvuruLogEkle(Mesaj);

                    Metodlar.DetayliLogYaz(Mesaj);

                    if (bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                    {
                        siradakiIslem = "İşlem Tamamlandı";

                        webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/logout.jsp", "");
                    }
                    else if (bfsira == Enums.BasvuruFormuTurleri.Bf3294)
                    {
                        siradakiIslem = "İşlem Tamamlandı";

                        webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/logout.jsp", "");
                    }
                }

                if (siradakiIslem == "İşlem Tamamlandı")
                {

                    if (TanimlamaYapilanlar2828.Count > 0 || TanimlamaYapilanlar6111.Count > 0 || TanimlamaYapilanlar7103.Count > 0 || TanimlamaYapilanlar7252.Count > 0 || TanimlamaYapilanlar17256.Count > 0 || TanimlamaYapilanlar27256.Count > 0 || TanimlamaYapilanlar7316.Count > 0 || TanimlamaYapilanlar3294.Count > 0)
                    {

                        ListedeBulunmayanKisileriTekrarDenemeSayisi++;

                        foreach (var item in TanimlamaYapilanlar6111)
                        {
                            BasvuruLogEkle(String.Format("{0} tc nolu kişinin {1} girişine 6111 tanımlandı dediği halde kişi listesinde yok", item.Key, item.Value.ToString("dd.MM.yyyy")));
                        }

                        foreach (var item in TanimlamaYapilanlar7103)
                        {
                            BasvuruLogEkle(String.Format("{0} tc nolu kişinin {1} girişine 7103 tanımlandı dediği halde kişi listesinde yok", item.Key, item.Value.ToString("dd.MM.yyyy")));
                        }


                        foreach (var item in TanimlamaYapilanlar2828)
                        {
                            BasvuruLogEkle(String.Format("{0} tc nolu kişinin {1} girişine 2828 tanımlandı dediği halde kişi listesinde yok", item.Key, item.Value.ToString("dd.MM.yyyy")));
                        }

                        foreach (var item in TanimlamaYapilanlar7252)
                        {
                            BasvuruLogEkle(String.Format("{0} tc nolu kişinin {1} girişine 7252 tanımlandı dediği halde kişi listesinde yok", item.Key, item.Value.ToString("dd.MM.yyyy")));
                        }

                        foreach (var item in TanimlamaYapilanlar17256)
                        {
                            BasvuruLogEkle(String.Format("{0} tc nolu kişinin {1} girişine 17256 tanımlandı dediği halde kişi listesinde yok", item.Key, item.Value.ToString("dd.MM.yyyy")));
                        }

                        foreach (var item in TanimlamaYapilanlar27256)
                        {
                            BasvuruLogEkle(String.Format("{0} tc nolu kişinin {1} girişine 27256 tanımlandı dediği halde kişi listesinde yok", item.Key, item.Value.ToString("dd.MM.yyyy")));
                        }

                        foreach (var item in TanimlamaYapilanlar7316)
                        {
                            BasvuruLogEkle(String.Format("{0} tc nolu kişinin {1} girişine 7316 tanımlandı dediği halde kişi listesinde yok", item.Key, item.Value.ToString("dd.MM.yyyy")));
                        }

                        foreach (var item in TanimlamaYapilanlar3294)
                        {
                            BasvuruLogEkle(String.Format("{0} tc nolu kişinin {1} girişine 3294 tanımlandı dediği halde kişi listesinde yok", item.Key, item.Value.ToString("dd.MM.yyyy")));
                        }

                        if (ListedeBulunmayanKisileriTekrarDenemeSayisi < 5)
                        {

                            BasvuruLogEkle(String.Format("Tanımlama yapıldı dediği halde kişi listesinde bulunmayan {0} kayıt tespit edildi. Bu kişiler tekrar denenecek. {1}. deneme", TanimlamaYapilanlar6111.Count + TanimlamaYapilanlar2828.Count + TanimlamaYapilanlar7103.Count + TanimlamaYapilanlar7252.Count + TanimlamaYapilanlar17256.Count + TanimlamaYapilanlar27256.Count + TanimlamaYapilanlar7316.Count + TanimlamaYapilanlar3294.Count, ListedeBulunmayanKisileriTekrarDenemeSayisi));

                            islemiTamamlananKisiler.RemoveWhere(p => TanimlamaYapilanlar2828.Any(x => p.Key.Equals(x.Key) && p.Value.Equals(x.Value))
                                                                     || TanimlamaYapilanlar6111.Any(x => p.Key.Equals(x.Key) && p.Value.Equals(x.Value))
                                                                     || TanimlamaYapilanlar7103.Any(x => p.Key.Equals(x.Key) && p.Value.Equals(x.Value))
                                                                     || TanimlamaYapilanlar7252.Any(x => p.Key.Equals(x.Key) && p.Value.Equals(x.Value))
                                                                     || TanimlamaYapilanlar27256.Any(x => p.Key.Equals(x.Key) && p.Value.Equals(x.Value))
                                                                     || TanimlamaYapilanlar17256.Any(x => p.Key.Equals(x.Key) && p.Value.Equals(x.Value))
                                                                     || TanimlamaYapilanlar7316.Any(x => p.Key.Equals(x.Key) && p.Value.Equals(x.Value))
                                                                     || TanimlamaYapilanlar3294.Any(x => p.Key.Equals(x.Key) && p.Value.Equals(x.Value))
                                                                     );
                            islemiTamamlananDonemler.RemoveWhere(p => TanimlamaYapilanlar2828.Any(x => String.Format("{0}/{1}", x.Value.Year, x.Value.Month.ToString().PadLeft(2, '0')).Equals(p))
                                                                    || TanimlamaYapilanlar6111.Any(x => String.Format("{0}/{1}", x.Value.Year, x.Value.Month.ToString().PadLeft(2, '0')).Equals(p))
                                                                    || TanimlamaYapilanlar7252.Any(x => String.Format("{0}/{1}", x.Value.Year, x.Value.Month.ToString().PadLeft(2, '0')).Equals(p))
                                                                    || TanimlamaYapilanlar17256.Any(x => String.Format("{0}/{1}", x.Value.Year, x.Value.Month.ToString().PadLeft(2, '0')).Equals(p))
                                                                    || TanimlamaYapilanlar27256.Any(x => String.Format("{0}/{1}", x.Value.Year, x.Value.Month.ToString().PadLeft(2, '0')).Equals(p))
                                                                    || TanimlamaYapilanlar7103.Any(x => String.Format("{0}/{1}", x.Value.Year, x.Value.Month.ToString().PadLeft(2, '0')).Equals(p))
                                                                    || TanimlamaYapilanlar7316.Any(x => String.Format("{0}/{1}", x.Value.Year, x.Value.Month.ToString().PadLeft(2, '0')).Equals(p))
                                                                    || TanimlamaYapilanlar3294.Any(x => String.Format("{0}/{1}", x.Value.Year, x.Value.Month.ToString().PadLeft(2, '0')).Equals(p))
                                                                    );

                            TanimlamaYapilanlar2828.Clear();
                            TanimlamaYapilanlar6111.Clear();
                            TanimlamaYapilanlar7103.Clear();
                            TanimlamaYapilanlar7252.Clear();
                            TanimlamaYapilanlar17256.Clear();
                            TanimlamaYapilanlar27256.Clear();
                            TanimlamaYapilanlar7316.Clear();
                            TanimlamaYapilanlar3294.Clear();

                            basvurukisiler2828.Clear();
                            basvurukisiler6111.Clear();
                            basvurukisiler7103.Clear();
                            basvurukisiler7252.Clear();
                            basvurukisiler17256.Clear();
                            basvurukisiler27256.Clear();
                            basvurukisiler7316.Clear();
                            basvurukisiler3294.Clear();

                            siradakiIslem = "Teşvik Tanımlama Açılacak";

                            string response = BasvuruWebClient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvikTanimlama.action;", string.Empty);

                            YuklenenSayfaninIciniDolas(response);

                            return;
                        }
                        else
                        {
                            BasvuruLogEkle(String.Format("Tanımlama yapıldı dediği halde kişi listesinde bulunmayan {0} kayıt tespit edildi. Bu kişiler 3 denemeye rağmen kişi listesinde bulunamadı. Excele kaydetme işlemine geçilecek", TanimlamaYapilanlar6111.Count + TanimlamaYapilanlar2828.Count + TanimlamaYapilanlar7103.Count + TanimlamaYapilanlar7252.Count + TanimlamaYapilanlar17256.Count + TanimlamaYapilanlar27256.Count + TanimlamaYapilanlar7316.Count + TanimlamaYapilanlar3294.Count));
                        }
                    }

                    //tüm listeyi kaydet
                    throw new IslemTamamException(siradakiIslem);
                }

                #endregion
            }
            catch (IslemTamamException ex)
            {
                stopwatch.Stop();

                string mesaj = ex.Message + " Tamamlanma süresi: " + stopwatch.Elapsed;
                BasvuruLogEkle(mesaj);

                this.BasvuruSonaErdi(true, false, "");
            }
            catch (OperationCanceledException ex)
            {
                throw ex;
            }
            catch (AggregateException ae)
            {
                ae.Handle(ex =>
                {
                    Metodlar.HataMesajiGoster(ex, "BF İndirme Hatası");

                    return false;
                });

                //throw ex;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                string mesaj = "Hata: " + ex.Message;
                BasvuruLogEkle(mesaj);

                Metodlar.HataMesajiGoster(ex, "Tüm teşvikler başvuru formu indirme işleminde hata meydana geldi");

                //DegiskenTemizle();
            }


        }
        public Enums.GecerliSayfaSonuclari GecerliSayfaOlupOlmadiginiKontrolEt(string ResponseHtml, string SiradakiIslem, ref ProjeGiris webclient, bool UzunSuredirIslemYapilmadiUyarisindaSayfaYenidenYuklensin = true)
        {

            if (ResponseHtml.Equals("Error"))
            {
                BasvuruLogEkle("Sayfa yüklenirken hata meydana geldi");

                Metodlar.DetayliLogYaz("Sayfa yüklenirken hata meydana geldi. İnternet bağlantınız olmayabilir veya SGK sistemi çalışmıyor olabilir.");
            }
            else if (ResponseHtml.Equals("Cancelled"))
            {
                Metodlar.DetayliLogYaz("İstek zamanaşımına uğradı");

                return Enums.GecerliSayfaSonuclari.Iptal;
            }
            else if (ResponseHtml.Equals("LogOut"))
            {
                Metodlar.DetayliLogYaz("Geçerli Sayfa Kontrolü: Web client cevabı LogOut olduğu için devam edilmeyecek");

                return Enums.GecerliSayfaSonuclari.Iptal;
            }

            bool sayfaBulundu = false;

            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(ResponseHtml);

            if (ResponseHtml != null && ResponseHtml.Contains("İşveren Sistemi Kullanıcı Girişi"))
            {

                SiradakiIslem = "Giriş Yapılacak";

                sayfaBulundu = true;
            }
            else if (ResponseHtml.Contains("UYGULAMA BAŞLATILDI"))
            {
                sayfaBulundu = true;

                SiradakiIslem = "Uygulama Başlayacak";
            }
            else if (ResponseHtml != null && ResponseHtml.Contains("id=\"pencereLinkIdYeni\"") && html.GetElementbyId("pencereLinkIdYeni").OuterHtml.Contains("src=\"https://uyg.sgk.gov.tr/YeniSistem/Isveren/tesvikTanimlama.action;"))
            {
                if (SiradakiIslem.Equals("Teşvik Tanımlama Açılacak"))
                {
                    sayfaBulundu = true;
                }
            }
            else if (ResponseHtml.Contains("<center>TEŞVİK SORGU SONUÇLARI</center>"))
            {
                if (SiradakiIslem.Equals("KİŞİNİN TÜM TEŞVİKLERİ"))
                {
                    sayfaBulundu = true;
                }
            }
            else if (html.GetElementbyId("pencereLinkIdYeni") != null)
            {
                if (SiradakiIslem == "4447/GEÇİCİ 10.MADDE LİSTELEME/SİLME AÇILACAK" && html.GetElementbyId("pencereLinkIdYeni").OuterHtml.Contains("src=\"https://uyg.sgk.gov.tr/YeniSistem/Isveren/tesvik4447_10.action;"))
                {
                    sayfaBulundu = true;
                }
                else if (SiradakiIslem == "4447/GEÇİCİ 19.MADDE LİSTELEME/SİLME AÇILACAK" && html.GetElementbyId("pencereLinkIdYeni").OuterHtml.Contains("src=\"https://uyg.sgk.gov.tr/YeniSistem/Isveren/tesvik4447_19_sigortali.action;"))
                {
                    sayfaBulundu = true;
                }
                else if (SiradakiIslem == "2828/EK 1.MADDE LİSTELEME/SİLME AÇILACAK" && html.GetElementbyId("pencereLinkIdYeni").OuterHtml.Contains("src=\"https://uyg.sgk.gov.tr/YeniSistem/Isveren/tesvik2828_ek1.action;"))
                {
                    sayfaBulundu = true;
                }
                else if (SiradakiIslem == "4447/GEÇİCİ 26.MADDE LİSTELEME/SİLME AÇILACAK" && html.GetElementbyId("pencereLinkIdYeni").OuterHtml.Contains("src=\"https://uyg.sgk.gov.tr/YeniSistem/Isveren/tesvik4447_26_sigortali.action;"))
                {
                    sayfaBulundu = true;
                }
                else if (SiradakiIslem == "4447/GEÇİCİ 27.MADDE LİSTELEME/SİLME AÇILACAK" && html.GetElementbyId("pencereLinkIdYeni").OuterHtml.Contains("src=\"https://uyg.sgk.gov.tr/YeniSistem/Isveren/tesvik4447_27_sigortali.action;"))
                {
                    sayfaBulundu = true;
                }
                else if (SiradakiIslem == "4447/GEÇİCİ 28.MADDE LİSTELEME/SİLME AÇILACAK" && html.GetElementbyId("pencereLinkIdYeni").OuterHtml.Contains("src=\"https://uyg.sgk.gov.tr/YeniSistem/Isveren/tesvik4447_28_sigortali.action;"))
                {
                    sayfaBulundu = true;
                }
                else if (SiradakiIslem == "4447/GEÇİCİ 30.MADDE LİSTELEME/SİLME AÇILACAK" && html.GetElementbyId("pencereLinkIdYeni").OuterHtml.Contains("src=\"https://uyg.sgk.gov.tr/YeniSistem/Isveren/tesvik4447_30_sigortali.action;"))
                {
                    sayfaBulundu = true;
                }
                else if (SiradakiIslem == "3294/SOSYAL YARDIM ALANLARIN İSTİHDAMI LİSTELEME/SİLME AÇILACAK" && html.GetElementbyId("pencereLinkIdYeni").OuterHtml.Contains("src=\"https://uyg.sgk.gov.tr/YeniSistem/Isveren/tesvik3294_sigortali.action;"))
                {
                    sayfaBulundu = true;
                }
            }
            else if (ResponseHtml != null && (ResponseHtml.Contains("<center>4447 GEÇİCİ 10. MADDE TANIMLI SİGORTALILAR LİSTESİ</center>") || ResponseHtml.Contains("<center>4447. GEÇİCİ 10. MD.(6111) T.C. KİMLİK İÇİN ORTALAMA SAYISI</center>")))
            {
                if (SiradakiIslem == "Kişi Listesi Alınacak 6111")
                {
                    sayfaBulundu = true;
                }
            }
            else if (ResponseHtml != null && (ResponseHtml.Contains("<center>4447 GEÇİCİ 19. MADDE KONTROL İŞLEMLERİ</center>") || ResponseHtml.Contains("<center>4447/ GEÇİCİ 19. MADDE TANIMLI SİGORTALILAR LİSTESİ</center>")))
            {
                if (SiradakiIslem == "Kişi Listesi Alınacak 7103")
                {
                    sayfaBulundu = true;
                }
            }
            else if (ResponseHtml != null && (ResponseHtml.Contains("<center>2828 SAYILI KANUN EK 1.MADDE TANIMLI SİGORTALILAR LİSTESİ</center>") || ResponseHtml.Contains("<center>2828 SAYILI KANUN EK1. MADDE TANIMLI SİGORTALILAR LİSTESİ</center>")))
            {

                if (SiradakiIslem == "Kişi Listesi Alınacak 2828")
                {
                    sayfaBulundu = true;
                }

            }
            else if (ResponseHtml != null && (ResponseHtml.Contains("<center>4447 GEÇİCİ 26. MADDE KONTROL İŞLEMLERİ</center>") || ResponseHtml.Contains("<center>4447/ GEÇİCİ 26. MADDE TANIMLI SİGORTALILAR LİSTESİ</center>")))
            {

                if (SiradakiIslem == "Kişi Listesi Alınacak 7252")
                {
                    sayfaBulundu = true;
                }

            }
            else if (ResponseHtml != null && (ResponseHtml.Contains("<center>4447 GEÇİCİ 27. MADDE KONTROL İŞLEMLERİ</center>") || ResponseHtml.Contains("<center>4447/ GEÇİCİ 27. MADDE TANIMLI SİGORTALILAR LİSTESİ</center>")))
            {

                if (SiradakiIslem == "Kişi Listesi Alınacak 17256")
                {
                    sayfaBulundu = true;
                }

            }
            else if (ResponseHtml != null && (ResponseHtml.Contains("<center>4447 GEÇİCİ 28. MADDE KONTROL İŞLEMLERİ</center>") || ResponseHtml.Contains("<center>4447/ GEÇİCİ 28. MADDE TANIMLI SİGORTALILAR LİSTESİ</center>")))
            {

                if (SiradakiIslem == "Kişi Listesi Alınacak 27256")
                {
                    sayfaBulundu = true;
                }
            }
            else if (ResponseHtml != null && (ResponseHtml.Contains("<center>4447 GEÇİCİ 30. MADDE KONTROL İŞLEMLERİ</center>") || ResponseHtml.Contains("<center>4447/ GEÇİCİ 30. MADDE TANIMLI SİGORTALILAR LİSTESİ</center>")))
            {

                if (SiradakiIslem == "Kişi Listesi Alınacak 7316")
                {
                    sayfaBulundu = true;
                }
            }
            else if (ResponseHtml != null && (ResponseHtml.Contains("<center>3294.MADDE KONTROL İŞLEMLERİ</center>") || ResponseHtml.Contains("<center>3294.MADDE TANIMLI SİGORTALILAR LİSTESİ</center>") || ResponseHtml.Contains("3294/ SOSYAL YARDIM ALANLARIN İSTİHDAMI")))
            {

                if (SiradakiIslem == "Kişi Listesi Alınacak 3294")
                {
                    sayfaBulundu = true;
                }
            }
            else if (ResponseHtml != null && ResponseHtml.Contains("<center>TEŞVİK TANIMLAMA</center>"))
            {
                if (SiradakiIslem.Equals("Dönem Seçilecek"))
                {
                    if (html.GetElementbyId("tesvikTanimlama_donem_yil_ay_index") != null)
                    {
                        var dropdown = html.GetElementbyId("tesvikTanimlama_donem_yil_ay_index");

                        var options = dropdown.SelectNodes("option");

                        foreach (var option in options)
                        {
                            if (option.Attributes["value"].Value == "0" && option.Attributes["selected"].Value == "selected")
                            {

                                sayfaBulundu = true;

                                break;
                            }
                        }
                    }
                }

                if (SiradakiIslem.Equals("Sıra Seçilecek"))
                {
                    if (ResponseHtml != null && ResponseHtml.Contains("İstenen Bilgilere Göre Herhangi Bir Kayıt gelmemiştir"))
                    {
                        sayfaBulundu = true;
                    }
                    else if (html.DocumentNode.SelectNodes("//input[@name='iseGirisMapIndex']") != null && html.DocumentNode.SelectNodes("//input[@name='iseGirisMapIndex']").Count() > 0)
                    {
                        sayfaBulundu = true;
                    }

                }
            }

            if (!sayfaBulundu)
            {
                if (!SiradakiIslem.Equals("İşlem Tamamlandı"))
                {

                    if (ResponseHtml.Contains("Uzun süredir işlem yapmadığınızdan işyeri bilgileri sıfırlanmıştır"))
                    {
                        if (UzunSuredirIslemYapilmadiUyarisindaSayfaYenidenYuklensin == false)
                        {
                            return Enums.GecerliSayfaSonuclari.UzunSureliIslemYapilamadiUyarisi;
                        }
                        else webclient.ReConnect();
                    }

                    Metodlar.DetayliLogYaz("Geçerli bir sayfa bulunamadı. Takılan sayfa yeniden yüklenecek");

                    if (SiradakiIslem == "4447/GEÇİCİ 10.MADDE LİSTELEME/SİLME AÇILACAK" || SiradakiIslem == "Kişi Listesi Alınacak 6111")
                    {
                        siradakiIslem = "4447/GEÇİCİ 10.MADDE LİSTELEME/SİLME AÇILACAK";

                        Metodlar.DetayliLogYaz("Geçerli sayfa bulunamadı .4447/GEÇİCİ 10.MADDE LİSTELEME/SİLME AÇILACAK");

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444710Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return Enums.GecerliSayfaSonuclari.Iptal;

                    }
                    else if (SiradakiIslem == "4447/GEÇİCİ 19.MADDE LİSTELEME/SİLME AÇILACAK" || SiradakiIslem == "Kişi Listesi Alınacak 7103")
                    {
                        siradakiIslem = "4447/GEÇİCİ 19.MADDE LİSTELEME/SİLME AÇILACAK";

                        Metodlar.DetayliLogYaz("Geçerli sayfa bulunamadı.4447/GEÇİCİ 19.MADDE LİSTELEME/SİLME AÇILACAK");

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444719Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return Enums.GecerliSayfaSonuclari.Iptal;

                    }
                    else if (SiradakiIslem == "2828/EK 1.MADDE LİSTELEME/SİLME AÇILACAK" || SiradakiIslem == "Kişi Listesi Alınacak 2828")
                    {
                        siradakiIslem = "2828/EK 1.MADDE LİSTELEME/SİLME AÇILACAK";

                        Metodlar.DetayliLogYaz("Geçerli sayfa bulunamadı.2828/EK 1.MADDE LİSTELEME/SİLME AÇILACAK");

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik44472828Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return Enums.GecerliSayfaSonuclari.Iptal;

                    }
                    else if (SiradakiIslem == "4447/GEÇİCİ 26.MADDE LİSTELEME/SİLME AÇILACAK" || SiradakiIslem == "Kişi Listesi Alınacak 7252")
                    {
                        siradakiIslem = "4447/GEÇİCİ 26.MADDE LİSTELEME/SİLME AÇILACAK";

                        Metodlar.DetayliLogYaz("Geçerli sayfa bulunamadı.4447/GEÇİCİ 26.MADDE LİSTELEME/SİLME AÇILACAK");

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444726Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return Enums.GecerliSayfaSonuclari.Iptal;

                    }
                    else if (SiradakiIslem == "4447/GEÇİCİ 27.MADDE LİSTELEME/SİLME AÇILACAK" || SiradakiIslem == "Kişi Listesi Alınacak 17256")
                    {
                        siradakiIslem = "4447/GEÇİCİ 27.MADDE LİSTELEME/SİLME AÇILACAK";

                        Metodlar.DetayliLogYaz("Geçerli sayfa bulunamadı.4447/GEÇİCİ 27.MADDE LİSTELEME/SİLME AÇILACAK");

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444727Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return Enums.GecerliSayfaSonuclari.Iptal;

                    }
                    else if (SiradakiIslem == "4447/GEÇİCİ 28.MADDE LİSTELEME/SİLME AÇILACAK" || SiradakiIslem == "Kişi Listesi Alınacak 27256")
                    {
                        siradakiIslem = "4447/GEÇİCİ 28.MADDE LİSTELEME/SİLME AÇILACAK";

                        Metodlar.DetayliLogYaz("Geçerli sayfa bulunamadı.4447/GEÇİCİ 28.MADDE LİSTELEME/SİLME AÇILACAK");

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444728Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return Enums.GecerliSayfaSonuclari.Iptal;

                    }
                    else if (SiradakiIslem == "4447/GEÇİCİ 30.MADDE LİSTELEME/SİLME AÇILACAK" || SiradakiIslem == "Kişi Listesi Alınacak 7316")
                    {
                        siradakiIslem = "4447/GEÇİCİ 30.MADDE LİSTELEME/SİLME AÇILACAK";

                        Metodlar.DetayliLogYaz("Geçerli sayfa bulunamadı.4447/GEÇİCİ 30.MADDE LİSTELEME/SİLME AÇILACAK");

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444730Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return Enums.GecerliSayfaSonuclari.Iptal;

                    }
                    else if (SiradakiIslem == "3294/SOSYAL YARDIM ALANLARIN İSTHİDAMI LİSTELEME/SİLME AÇILACAK" || SiradakiIslem == "Kişi Listesi Alınacak 3294")
                    {
                        siradakiIslem = "3294/SOSYAL YARDIM ALANLARIN İSTHİDAMI LİSTELEME/SİLME AÇILACAK";

                        Metodlar.DetayliLogYaz("Geçerli sayfa bulunamadı.3294/SOSYAL YARDIM ALANLARIN İSTHİDAMI LİSTELEME/SİLME AÇILACAK");

                        string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik3294Liste.action;", string.Empty);

                        YuklenenSayfaninIciniDolas(response);

                        return Enums.GecerliSayfaSonuclari.Iptal;

                    }
                    else if (SiradakiIslem == "Giriş Yapılacak" || SiradakiIslem == "Uygulama Başlayacak" || SiradakiIslem == "Teşvik Tanımlama Açılacak")
                    {
                        bool UygulamaBaslayacak = true;

                        if (SiradakiIslem == "Giriş Yapılacak")
                        {
                            if (ResponseHtml == null || !ResponseHtml.Contains("UYGULAMA BAŞLATILDI"))
                            {
                                UygulamaBaslayacak = false;
                            }
                        }

                        if (UygulamaBaslayacak)
                        {
                            siradakiIslem = "Teşvik Tanımlama Açılacak";

                            Metodlar.DetayliLogYaz("Geçerli sayfa bulunamadı. Sisteme giriş yapıldığı için doğrudan Teşvik Tanımlama Açılacak");

                            string response = webclient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvikTanimlama.action;", string.Empty);

                            YuklenenSayfaninIciniDolas(response);

                            return Enums.GecerliSayfaSonuclari.Iptal;
                        }
                        else
                        {
                            Metodlar.DetayliLogYaz("Geçerli sayfa bulunamadığından sisteme giriş yapılacak");

                            BasvuruSayfayiYukle();

                            return Enums.GecerliSayfaSonuclari.Iptal;
                        }
                    }
                }
            }

            return sayfaBulundu ? Enums.GecerliSayfaSonuclari.Gecerli : Enums.GecerliSayfaSonuclari.Gecersiz;
        }

        void BasvuruSonaErdi(bool Kaydet, bool HataVarmi, string hatamesaji)
        {

            Dictionary<string, DataTable> DataTables = new Dictionary<string, DataTable>();

            string KanunNo = string.Empty;

            switch (bfsira)
            {
                case Enums.BasvuruFormuTurleri.Bf6111:
                    KanunNo = "6111";
                    break;
                case Enums.BasvuruFormuTurleri.Bf687:
                    KanunNo = "687";
                    break;
                case Enums.BasvuruFormuTurleri.Bf6645:
                    KanunNo = "6645";
                    break;
                case Enums.BasvuruFormuTurleri.Bf7103:
                    KanunNo = "7103";
                    break;
                case Enums.BasvuruFormuTurleri.Bf2828:
                    KanunNo = "2828";
                    break;
                case Enums.BasvuruFormuTurleri.BfTumu:
                    KanunNo = "Tüm Teşvikler";
                    break;
                case Enums.BasvuruFormuTurleri.Bf14857:
                    KanunNo = "14857";
                    break;
                case Enums.BasvuruFormuTurleri.Bf7252:
                    KanunNo = "7252";
                    break;
                case Enums.BasvuruFormuTurleri.Bf7256:
                    KanunNo = "7256";
                    break;
                case Enums.BasvuruFormuTurleri.Bf7316:
                    KanunNo = "7316";
                    break;
                case Enums.BasvuruFormuTurleri.Bf3294:
                    KanunNo = "3294";
                    break;
                default:
                    break;
            }

            if (Kaydet)
            {

                if (bfsira == Enums.BasvuruFormuTurleri.Bf6111 || bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                {

                    DataTable dtbasvuru = new DataTable("dtBasvuruFormu");

                    foreach (var item in Sabitler.BasvuruFormlariSutunlari["6111-v2"])
                    {

                        DataColumn column = new DataColumn();
                        column.DataType = typeof(string);

                        dtbasvuru.Columns.Add(column);
                    }

                    DataTables.Add("6111", dtbasvuru);
                }

                if (bfsira == Enums.BasvuruFormuTurleri.Bf7103 || bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                {
                    DataTable dtbasvuru = new DataTable("dtBasvuruFormu7103");

                    foreach (var item in Sabitler.BasvuruFormlariSutunlari["7103"])
                    {

                        DataColumn column = new DataColumn();
                        column.DataType = typeof(string);

                        column.AllowDBNull = true;
                        dtbasvuru.Columns.Add(column);
                    }

                    DataTables.Add("7103", dtbasvuru);
                }

                if (bfsira == Enums.BasvuruFormuTurleri.Bf2828 || bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                {
                    DataTable dtbasvuru = new DataTable("dtBasvuruFormu2828");

                    foreach (var item in Sabitler.BasvuruFormlariSutunlari["2828"])
                    {

                        DataColumn column = new DataColumn();
                        column.DataType = typeof(string);

                        column.AllowDBNull = true;
                        dtbasvuru.Columns.Add(column);
                    }

                    DataTables.Add("2828", dtbasvuru);
                }


                if (bfsira == Enums.BasvuruFormuTurleri.Bf7252 || (bfsira == Enums.BasvuruFormuTurleri.BfTumu && !CariTanimla))
                {
                    DataTable dtbasvuru = new DataTable("dtBasvuruFormu7252");

                    foreach (var item in Sabitler.BasvuruFormlariSutunlari["7252"])
                    {

                        DataColumn column = new DataColumn();
                        column.DataType = typeof(string);

                        column.AllowDBNull = true;
                        dtbasvuru.Columns.Add(column);
                    }

                    DataTables.Add("7252", dtbasvuru);
                }

                if (bfsira == Enums.BasvuruFormuTurleri.Bf7256 || (bfsira == Enums.BasvuruFormuTurleri.BfTumu && !CariTanimla))
                {
                    //DataTable dtbasvuru = new DataTable("dtBasvuruFormu17256");

                    //foreach (var item in Sabitler.BasvuruFormlariSutunlari["17256"])
                    //{

                    //    DataColumn column = new DataColumn();
                    //    column.DataType = typeof(string);

                    //    column.AllowDBNull = true;
                    //    dtbasvuru.Columns.Add(column);
                    //}

                    //DataTables.Add("17256", dtbasvuru);

                    DataTable dtbasvuru27256 = new DataTable("dtBasvuruFormu27256");

                    foreach (var item in Sabitler.BasvuruFormlariSutunlari["27256"])
                    {

                        DataColumn column = new DataColumn();
                        column.DataType = typeof(string);

                        column.AllowDBNull = true;
                        dtbasvuru27256.Columns.Add(column);
                    }

                    DataTables.Add("27256", dtbasvuru27256);
                }

                if (bfsira == Enums.BasvuruFormuTurleri.Bf7316 || (bfsira == Enums.BasvuruFormuTurleri.BfTumu && !CariTanimla))
                {
                    DataTable dtbasvuru7316 = new DataTable("dtBasvuruFormu7316");

                    foreach (var item in Sabitler.BasvuruFormlariSutunlari["7316"])
                    {

                        DataColumn column = new DataColumn();
                        column.DataType = typeof(string);

                        column.AllowDBNull = true;
                        dtbasvuru7316.Columns.Add(column);
                    }

                    DataTables.Add("7316", dtbasvuru7316);
                }

                if (bfsira == Enums.BasvuruFormuTurleri.Bf3294 || bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                {
                    DataTable dtbasvuru3294 = new DataTable("dtBasvuruFormu3294");

                    foreach (var item in Sabitler.BasvuruFormlariSutunlari["3294"])
                    {

                        DataColumn column = new DataColumn();
                        column.DataType = typeof(string);

                        column.AllowDBNull = true;
                        dtbasvuru3294.Columns.Add(column);
                    }

                    DataTables.Add("3294", dtbasvuru3294);
                }

                if (bfsira == Enums.BasvuruFormuTurleri.Bf687)
                {
                    DataTable dtbasvuru = new DataTable("dtBasvuruFormu687");

                    foreach (var item in Sabitler.BasvuruFormlariSutunlari["687"])
                    {

                        DataColumn column = new DataColumn();

                        if (item.Key == Enums.BasvuruFormuSutunTurleri.KanunNo)
                        {
                            column.ColumnName = "KANUN NO";
                        }


                        column.DataType = typeof(string); ;

                        dtbasvuru.Columns.Add(column);
                    }

                    DataTables.Add("687", dtbasvuru);
                }

                if (bfsira == Enums.BasvuruFormuTurleri.Bf6645)
                {
                    DataTable dtbasvuru = new DataTable("dtBasvuruFormu6645");

                    foreach (var item in Sabitler.BasvuruFormlariSutunlari["6645"])
                    {

                        DataColumn column = new DataColumn();

                        column.DataType = typeof(string);

                        dtbasvuru.Columns.Add(column);
                    }

                    DataTables.Add("6645", dtbasvuru);
                }

                if (bfsira == Enums.BasvuruFormuTurleri.Bf14857)
                {
                    DataTable dtbasvuru = new DataTable("dtBasvuruFormu14857");

                    foreach (var item in Sabitler.BasvuruFormlariSutunlari["14857"])
                    {
                        DataColumn column = new DataColumn();

                        column.DataType = typeof(string);

                        dtbasvuru.Columns.Add(column);
                    }

                    DataTables.Add("14857", dtbasvuru);
                }

                DataTable dteski6645 = null;
                DataTable dteski687 = null;
                DataTable dteski14857 = null;

                string basvuruyol = Metodlar.FormBul(SuanYapilanIsyeriBasvuru, Enums.FormTuru.BasvuruFormu);

                Dictionary<string, Dictionary<string, string>> AraciSutunDegerleri = new Dictionary<string, Dictionary<string, string>>();

                Dictionary<string, Dictionary<string, string>> Kayitlar7256VerilsinMi = new Dictionary<string, Dictionary<string, string>>();

                if (basvuruyol != null)
                {
                    DataSet ds = Metodlar.BasvuruListesiniYukle(basvuruyol, false);

                    DataTable dtbasvuru = null;

                    if (bfsira == Enums.BasvuruFormuTurleri.Bf6111 || bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                    {
                        dtbasvuru = ds.Tables[0];

                        if (dtbasvuru.Columns.Contains("Aracı"))
                        {
                            var sutundegerleri = dtbasvuru.AsEnumerable().Where(row => !string.IsNullOrEmpty(row["Aracı"].ToString())).Select(p => new KeyValuePair<string, string>(p[0].ToString(), p["Aracı"].ToString()));

                            if (sutundegerleri.Count() > 0) AraciSutunDegerleri.Add("6111", new Dictionary<string, string>());

                            foreach (var item in sutundegerleri)
                            {
                                if (!AraciSutunDegerleri["6111"].ContainsKey(item.Key))
                                {
                                    AraciSutunDegerleri["6111"].Add(item.Key, item.Value);
                                }
                            }
                        }
                    }

                    if (bfsira == Enums.BasvuruFormuTurleri.Bf687)
                    {
                        dtbasvuru = ds.Tables.Count > 2 ? ds.Tables[2] : null;

                        dteski687 = dtbasvuru;

                        if (dtbasvuru != null && dtbasvuru.Columns.Contains("Aracı"))
                        {
                            var sutundegerleri = dtbasvuru.AsEnumerable().Where(row => !string.IsNullOrEmpty(row["Aracı"].ToString())).Select(p => new KeyValuePair<string, string>(p[0].ToString(), p["Aracı"].ToString()));

                            if (sutundegerleri.Count() > 0) AraciSutunDegerleri.Add("687", new Dictionary<string, string>());

                            foreach (var item in sutundegerleri)
                            {
                                if (!AraciSutunDegerleri["687"].ContainsKey(item.Key))
                                {
                                    AraciSutunDegerleri["687"].Add(item.Key, item.Value);
                                }
                            }
                        }
                    }

                    if (bfsira == Enums.BasvuruFormuTurleri.Bf6645)
                    {
                        dtbasvuru = ds.Tables.Count > 1 ? ds.Tables[1] : null;

                        dteski6645 = dtbasvuru;

                        if (dtbasvuru != null && dtbasvuru.Columns.Contains("Aracı"))
                        {
                            var sutundegerleri = dtbasvuru.AsEnumerable().Where(row => !string.IsNullOrEmpty(row["Aracı"].ToString())).Select(p => new KeyValuePair<string, string>(p[0].ToString(), p["Aracı"].ToString()));

                            if (sutundegerleri.Count() > 0) AraciSutunDegerleri.Add("6645", new Dictionary<string, string>());

                            foreach (var item in sutundegerleri)
                            {
                                if (!AraciSutunDegerleri["6645"].ContainsKey(item.Key))
                                {
                                    AraciSutunDegerleri["6645"].Add(item.Key, item.Value);
                                }
                            }
                        }
                    }

                    if (bfsira == Enums.BasvuruFormuTurleri.Bf7103 || bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                    {
                        dtbasvuru = ds.Tables.Count > 3 ? ds.Tables[3] : null;

                        if (dtbasvuru != null && dtbasvuru.Columns.Contains("Aracı"))
                        {
                            var sutundegerleri = dtbasvuru.AsEnumerable().Where(row => !string.IsNullOrEmpty(row["Aracı"].ToString())).Select(p => new KeyValuePair<string, string>(p[0].ToString(), p["Aracı"].ToString()));

                            if (sutundegerleri.Count() > 0) AraciSutunDegerleri.Add("7103", new Dictionary<string, string>());

                            foreach (var item in sutundegerleri)
                            {
                                if (!AraciSutunDegerleri["7103"].ContainsKey(item.Key))
                                {
                                    AraciSutunDegerleri["7103"].Add(item.Key, item.Value);
                                }
                            }
                        }
                    }

                    if (bfsira == Enums.BasvuruFormuTurleri.Bf2828 || bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                    {
                        dtbasvuru = ds.Tables.Count > 4 ? ds.Tables[4] : null;

                        if (dtbasvuru != null && dtbasvuru.Columns.Contains("Aracı"))
                        {
                            var sutundegerleri = dtbasvuru.AsEnumerable().Where(row => !string.IsNullOrEmpty(row["Aracı"].ToString())).Select(p => new KeyValuePair<string, string>(p[0].ToString(), p["Aracı"].ToString()));

                            if (sutundegerleri.Count() > 0) AraciSutunDegerleri.Add("2828", new Dictionary<string, string>());

                            foreach (var item in sutundegerleri)
                            {
                                if (!AraciSutunDegerleri["2828"].ContainsKey(item.Key))
                                {
                                    AraciSutunDegerleri["2828"].Add(item.Key, item.Value);
                                }
                            }
                        }

                    }

                    if (bfsira == Enums.BasvuruFormuTurleri.Bf14857)
                    {
                        dtbasvuru = ds.Tables.Count > 5 ? ds.Tables[5] : null;

                        dteski14857 = dtbasvuru;

                        if (dtbasvuru != null && dtbasvuru.Columns.Contains("Aracı"))
                        {
                            var sutundegerleri = dtbasvuru.AsEnumerable().Where(row => !string.IsNullOrEmpty(row["Aracı"].ToString())).Select(p => new KeyValuePair<string, string>(p[0].ToString(), p["Aracı"].ToString()));

                            if (sutundegerleri.Count() > 0) AraciSutunDegerleri.Add("14857", new Dictionary<string, string>());

                            foreach (var item in sutundegerleri)
                            {
                                if (!AraciSutunDegerleri["14857"].ContainsKey(item.Key))
                                {
                                    AraciSutunDegerleri["14857"].Add(item.Key, item.Value);
                                }
                            }
                        }

                    }

                    if (bfsira == Enums.BasvuruFormuTurleri.Bf7252 || (bfsira == Enums.BasvuruFormuTurleri.BfTumu && !CariTanimla))
                    {
                        dtbasvuru = ds.Tables.Count > 6 ? ds.Tables[6] : null;

                        if (dtbasvuru != null && dtbasvuru.Columns.Contains("Aracı"))
                        {
                            var sutundegerleri = dtbasvuru.AsEnumerable().Where(row => !string.IsNullOrEmpty(row["Aracı"].ToString())).Select(p => new KeyValuePair<string, string>(p[0].ToString(), p["Aracı"].ToString()));

                            if (sutundegerleri.Count() > 0) AraciSutunDegerleri.Add("7252", new Dictionary<string, string>());

                            foreach (var item in sutundegerleri)
                            {
                                if (!AraciSutunDegerleri["7252"].ContainsKey(item.Key))
                                {
                                    AraciSutunDegerleri["7252"].Add(item.Key, item.Value);
                                }
                            }
                        }

                    }

                    if (bfsira == Enums.BasvuruFormuTurleri.Bf7256 || (bfsira == Enums.BasvuruFormuTurleri.BfTumu && !CariTanimla))
                    {
                        dtbasvuru = ds.Tables.Count > 7 ? ds.Tables[7] : null;

                        if (dtbasvuru != null && dtbasvuru.Columns.Contains("Aracı"))
                        {
                            var sutundegerleri = dtbasvuru.AsEnumerable().Where(row => !string.IsNullOrEmpty(row["Aracı"].ToString())).Select(p => new KeyValuePair<string, string>(p[0].ToString(), p["Aracı"].ToString()));

                            if (sutundegerleri.Count() > 0) AraciSutunDegerleri.Add("17256", new Dictionary<string, string>());

                            foreach (var item in sutundegerleri)
                            {
                                if (!AraciSutunDegerleri["17256"].ContainsKey(item.Key))
                                {
                                    AraciSutunDegerleri["17256"].Add(item.Key, item.Value);
                                }
                            }
                        }

                        if (dtbasvuru != null && dtbasvuru.Columns.Contains("Teşvik Verilsin"))
                        {
                            var sutundegerleri = dtbasvuru.AsEnumerable().Where(row => !string.IsNullOrEmpty(row["Teşvik Verilsin"].ToString())).Select(p => new KeyValuePair<string, string>(p[0].ToString(), p["Teşvik Verilsin"].ToString()));

                            if (sutundegerleri.Count() > 0) Kayitlar7256VerilsinMi.Add("17256", new Dictionary<string, string>());

                            foreach (var item in sutundegerleri)
                            {
                                if (!Kayitlar7256VerilsinMi["17256"].ContainsKey(item.Key))
                                {
                                    Kayitlar7256VerilsinMi["17256"].Add(item.Key, item.Value);
                                }
                            }
                        }
                    }

                    if (bfsira == Enums.BasvuruFormuTurleri.Bf7256 || (bfsira == Enums.BasvuruFormuTurleri.BfTumu && !CariTanimla))
                    {
                        dtbasvuru = ds.Tables.Count > 8 ? ds.Tables[8] : null;

                        if (dtbasvuru != null && dtbasvuru.Columns.Contains("Aracı"))
                        {
                            var sutundegerleri = dtbasvuru.AsEnumerable().Where(row => !string.IsNullOrEmpty(row["Aracı"].ToString())).Select(p => new KeyValuePair<string, string>(p[0].ToString(), p["Aracı"].ToString()));

                            if (sutundegerleri.Count() > 0) AraciSutunDegerleri.Add("27256", new Dictionary<string, string>());

                            foreach (var item in sutundegerleri)
                            {
                                if (!AraciSutunDegerleri["27256"].ContainsKey(item.Key))
                                {
                                    AraciSutunDegerleri["27256"].Add(item.Key, item.Value);
                                }
                            }
                        }

                        if (dtbasvuru != null && dtbasvuru.Columns.Contains("Teşvik Verilsin"))
                        {
                            var sutundegerleri = dtbasvuru.AsEnumerable().Where(row => !string.IsNullOrEmpty(row["Teşvik Verilsin"].ToString())).Select(p => new KeyValuePair<string, string>(p[0].ToString(), p["Teşvik Verilsin"].ToString()));

                            if (sutundegerleri.Count() > 0) Kayitlar7256VerilsinMi.Add("27256", new Dictionary<string, string>());

                            foreach (var item in sutundegerleri)
                            {
                                if (!Kayitlar7256VerilsinMi["27256"].ContainsKey(item.Key))
                                {
                                    Kayitlar7256VerilsinMi["27256"].Add(item.Key, item.Value);
                                }
                            }
                        }
                    }

                    if (bfsira == Enums.BasvuruFormuTurleri.Bf7316 || (bfsira == Enums.BasvuruFormuTurleri.BfTumu && !CariTanimla))
                    {
                        dtbasvuru = ds.Tables.Count > 9 ? ds.Tables[9] : null;

                        if (dtbasvuru != null && dtbasvuru.Columns.Contains("Aracı"))
                        {
                            var sutundegerleri = dtbasvuru.AsEnumerable().Where(row => !string.IsNullOrEmpty(row["Aracı"].ToString())).Select(p => new KeyValuePair<string, string>(p[0].ToString(), p["Aracı"].ToString()));

                            if (sutundegerleri.Count() > 0) AraciSutunDegerleri.Add("7316", new Dictionary<string, string>());

                            foreach (var item in sutundegerleri)
                            {
                                if (!AraciSutunDegerleri["7316"].ContainsKey(item.Key))
                                {
                                    AraciSutunDegerleri["7316"].Add(item.Key, item.Value);
                                }
                            }
                        }
                    }


                    if (bfsira == Enums.BasvuruFormuTurleri.Bf3294 || bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                    {
                        dtbasvuru = ds.Tables.Count > 10 ? ds.Tables[10] : null;

                        if (dtbasvuru != null && dtbasvuru.Columns.Contains("Aracı"))
                        {
                            var sutundegerleri = dtbasvuru.AsEnumerable().Where(row => !string.IsNullOrEmpty(row["Aracı"].ToString())).Select(p => new KeyValuePair<string, string>(p[0].ToString(), p["Aracı"].ToString()));

                            if (sutundegerleri.Count() > 0) AraciSutunDegerleri.Add("3294", new Dictionary<string, string>());

                            foreach (var item in sutundegerleri)
                            {
                                if (!AraciSutunDegerleri["3294"].ContainsKey(item.Key))
                                {
                                    AraciSutunDegerleri["3294"].Add(item.Key, item.Value);
                                }
                            }
                        }
                    }
                }


                DataTable dtEgitim = new DataTable();

                if (bfsira == Enums.BasvuruFormuTurleri.Bf6111 || bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                {
                    DataTable dtbasvuru = DataTables["6111"];

                    dtbasvuru.Rows.Clear();

                    dtEgitim = Metodlar.ReadExcelFile(System.IO.Path.Combine(Application.StartupPath, "EgitimSablon.xlsx"), MesajGostersin: false).Tables[0];

                    var sutunlar6111 = Sabitler.BasvuruFormlariSutunlari["6111-v2"];

                    var iseGirisSiraliListe = basvurukisiler6111.OrderBy(p => Convert.ToDateTime(p.GirisTarihi));

                    var tumaylar = new List<DateTime>();

                    if (TumKisilerSonuc.TumAylar != null && TumKisilerSonuc.TumAylar.Count > 0)
                    {
                        tumaylar = TumKisilerSonuc.TumAylar.Select(p => new DateTime(Convert.ToInt32(p.Split('-')[0]), Convert.ToInt32(p.Split('-')[1]), 1)).ToList();
                    }

                    if (Program.EgitimListesiOlusturulsun)
                    {
                        BasvuruLogEkle("6111 Eğitim belgesi listesi sorgulanıyor");
                    }

                    bool SistemeGirisYapamadi = false;

                    foreach (var basvurukisi in iseGirisSiraliListe)
                    {
                        var girisTarihi = Convert.ToDateTime(basvurukisi.GirisTarihi);

                        if (Program.EgitimListesiOlusturulsun && !SistemeGirisYapamadi)
                        {
                            bool EgitimListesineEklenecek = true;

                            if (!basvurukisi.Durum.Equals("ONAYSIZ"))
                            {
                                if (TumKisilerSonuc.TumKisiler != null && TumKisilerSonuc.TumKisiler.ContainsKey(basvurukisi.TcKimlikNo))
                                {
                                    var kisi = TumKisilerSonuc.TumKisiler[basvurukisi.TcKimlikNo];

                                    if (basvurukisi.TesvikSuresiBitis.Equals("0000/00")) continue;

                                    var tesvikBitisSon = Convert.ToDateTime(basvurukisi.TesvikSuresiBitis).AddMonths(1);

                                    if (!string.IsNullOrEmpty(basvurukisi.CikisTarihi))
                                    {
                                        var cikis = Convert.ToDateTime(basvurukisi.CikisTarihi);
                                        if (!kisi.CikisTarihleri.Any(p => p.Tarih.Date.Equals(cikis.Date)))
                                        {
                                            kisi.CikisTarihleri.Add(new GirisCikisTarihleri
                                            {
                                                Tarih = cikis.Date
                                            });
                                        }
                                    }

                                    if (kisi.CikisTarihleri.Any(p => p.Tarih >= girisTarihi && p.Tarih < tesvikBitisSon) == false)
                                    {
                                        if (TumKisilerSonuc.KisilerinSatirlari != null && TumKisilerSonuc.KisilerinSatirlari.ContainsKey(basvurukisi.TcKimlikNo))
                                        {
                                            if (TumKisilerSonuc.KisilerinSatirlari[basvurukisi.TcKimlikNo].Count > 0)
                                            {
                                                var tesvikDonemindenSonrakiEnYakinCikis = kisi.CikisTarihleri.OrderBy(p => p.Tarih).FirstOrDefault(p => p.Tarih >= tesvikBitisSon);

                                                var son = DateTime.MaxValue;

                                                if (tesvikDonemindenSonrakiEnYakinCikis != null)
                                                {
                                                    son = tesvikDonemindenSonrakiEnYakinCikis.Tarih.Date;
                                                }


                                                var aylar = TumKisilerSonuc.KisilerinSatirlari[basvurukisi.TcKimlikNo]
                                                            .Where(p => p.Value.Count > 0);

                                                if (aylar.Count() > 0)
                                                {

                                                    var enSonAy = tumaylar.Max();

                                                    var bakilacakAylar = new List<DateTime>();

                                                    if (enSonAy >= tesvikBitisSon)
                                                    {
                                                        bakilacakAylar = tumaylar.Where(p => p >= tesvikBitisSon && p <= son).ToList();

                                                        EgitimListesineEklenecek = false;

                                                        bakilacakAylar = bakilacakAylar.OrderBy(p => p).ToList();

                                                        foreach (var bakilacakAy in bakilacakAylar)
                                                        {
                                                            var tarihKey = bakilacakAy.Year.ToString() + "-" + bakilacakAy.Month.ToString();

                                                            if (TumKisilerSonuc.KisilerinSatirlari[basvurukisi.TcKimlikNo].ContainsKey(tarihKey))
                                                            {
                                                                var aySatirlari = TumKisilerSonuc.KisilerinSatirlari[basvurukisi.TcKimlikNo][tarihKey];

                                                                if (aySatirlari.Any(p => Program.TumTesvikler["6111"].DestekKapsaminaGirmeyenBelgeTurleri.Contains(p[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString()) == false && (Convert.ToDecimal(p[(int)Enums.AphbHucreBilgileri.Ucret]) + Convert.ToDecimal(p[(int)Enums.AphbHucreBilgileri.Ikramiye])) > 0))
                                                                {
                                                                    EgitimListesineEklenecek = true;

                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }

                                                }
                                                else EgitimListesineEklenecek = false;
                                            }
                                            else EgitimListesineEklenecek = false;
                                        }
                                        else EgitimListesineEklenecek = false;
                                    }
                                    else EgitimListesineEklenecek = false;
                                }
                                else EgitimListesineEklenecek = false;
                            }

                            if (EgitimListesineEklenecek)
                            {
                                var meslekAdiVeKodu = Metodlar.SistemdenMeslekKoduBul(SuanYapilanIsyeriBasvuru, basvurukisi.TcKimlikNo, girisTarihi, ref sigortaliIstenAyrilisProjesiConnect);

                                if (meslekAdiVeKodu.Equals("Sisteme giriş yapamadı"))
                                {
                                    SistemeGirisYapamadi = true;

                                    BasvuruLogEkle(String.Format("Sisteme giriş yapamadığı için 6111 eğitim belgesi listesi oluşturulmayacak", dtEgitim.Rows.Count));
                                }

                                if (!SistemeGirisYapamadi)
                                {

                                    var meslekKod = meslekAdiVeKodu.Split('-').Last();
                                    var meslekAdi = meslekAdiVeKodu.Replace(meslekKod, "").Trim('-').Trim();

                                    var newRow = dtEgitim.NewRow();

                                    newRow[(int)Enums.EgitimFormuHucreBilgileri.TcKimlikNo] = basvurukisi.TcKimlikNo;
                                    newRow[(int)Enums.EgitimFormuHucreBilgileri.Ad] = basvurukisi.Ad;
                                    newRow[(int)Enums.EgitimFormuHucreBilgileri.Soyad] = basvurukisi.Soyad;
                                    newRow[(int)Enums.EgitimFormuHucreBilgileri.BaslangicDonemi] = basvurukisi.TesvikSuresiBaslangic;
                                    newRow[(int)Enums.EgitimFormuHucreBilgileri.BitisDonemi] = basvurukisi.TesvikSuresiBitis;
                                    newRow[(int)Enums.EgitimFormuHucreBilgileri.OrtalamaSigortaliSayisi] = basvurukisi.Baz;
                                    newRow[(int)Enums.EgitimFormuHucreBilgileri.IseGirisTarihi] = basvurukisi.GirisTarihi;
                                    newRow[(int)Enums.EgitimFormuHucreBilgileri.IstenAyrilisTarihi] = basvurukisi.CikisTarihi;
                                    newRow[(int)Enums.EgitimFormuHucreBilgileri.OnayDurumu] = basvurukisi.Durum;
                                    newRow[(int)Enums.EgitimFormuHucreBilgileri.MeslekKodu] = meslekKod;
                                    newRow[(int)Enums.EgitimFormuHucreBilgileri.MeslekAdi] = meslekAdi;

                                    dtEgitim.Rows.Add(newRow);
                                }
                            }
                        }

                        DataRow row = null;

                        row = dtbasvuru.NewRow();

                        row[sutunlar6111[Enums.BasvuruFormuSutunTurleri.TcKimlikNo]] = basvurukisi.TcKimlikNo;
                        row[sutunlar6111[Enums.BasvuruFormuSutunTurleri.Sicil]] = basvurukisi.Sicil;
                        row[sutunlar6111[Enums.BasvuruFormuSutunTurleri.Ad]] = basvurukisi.Ad;
                        row[sutunlar6111[Enums.BasvuruFormuSutunTurleri.Soyad]] = basvurukisi.Soyad;
                        row[sutunlar6111[Enums.BasvuruFormuSutunTurleri.TesvikBaslangic]] = basvurukisi.TesvikSuresiBaslangic;
                        row[sutunlar6111[Enums.BasvuruFormuSutunTurleri.TesvikBitis]] = basvurukisi.TesvikSuresiBitis;
                        row[sutunlar6111[Enums.BasvuruFormuSutunTurleri.Baz]] = basvurukisi.Baz;
                        row[sutunlar6111[Enums.BasvuruFormuSutunTurleri.Giris]] = basvurukisi.GirisTarihi;
                        row[sutunlar6111[Enums.BasvuruFormuSutunTurleri.Cikis]] = basvurukisi.CikisTarihi;
                        row[sutunlar6111[Enums.BasvuruFormuSutunTurleri.OnayDurumu]] = basvurukisi.Durum;

                        if (AraciSutunDegerleri.ContainsKey("6111") && AraciSutunDegerleri["6111"].ContainsKey(basvurukisi.TcKimlikNo))
                        {
                            row[sutunlar6111[Enums.BasvuruFormuSutunTurleri.Araci]] = AraciSutunDegerleri["6111"][basvurukisi.TcKimlikNo];
                        }
                        else row[sutunlar6111[Enums.BasvuruFormuSutunTurleri.Araci]] = string.Empty;

                        dtbasvuru.Rows.Add(row);

                    }

                    if (Program.EgitimListesiOlusturulsun)
                    {
                        if (dtEgitim.Rows.Count > 0)
                        {
                            BasvuruLogEkle(String.Format("6111 Eğitim belgesi listesine eklenecek {0} kayıt bulundu", dtEgitim.Rows.Count));
                        }
                        else
                        {
                            BasvuruLogEkle("6111 Eğitim belgesi listesine eklenecek kayıt bulunamadı");
                        }
                    }
                }

                if (bfsira == Enums.BasvuruFormuTurleri.Bf687)
                {
                    var iseGirisSiraliListe = basvurukisiler687.OrderBy(p => Convert.ToDateTime(p.GirisTarihi));

                    foreach (var basvurukisi in iseGirisSiraliListe)
                    {
                        DataTable dtbasvuru = DataTables["687"];

                        bool OncedenDosyadaVar = false;

                        if (dteski687 != null)
                        {
                            foreach (DataRow drow in dteski687.Rows)
                            {
                                if (drow[Sabitler.BasvuruFormlariSutunlari["687"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString() == basvurukisi.TcKimlikNo && Convert.ToDateTime(drow[Sabitler.BasvuruFormlariSutunlari["687"][Enums.BasvuruFormuSutunTurleri.Giris]]) == Convert.ToDateTime(basvurukisi.GirisTarihi))
                                {
                                    OncedenDosyadaVar = true;

                                    break;
                                }
                            }
                        }

                        DataRow row = null;

                        row = dtbasvuru.NewRow();

                        row[Sabitler.BasvuruFormlariSutunlari["687"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]] = basvurukisi.TcKimlikNo;

                        row[Sabitler.BasvuruFormlariSutunlari["687"][Enums.BasvuruFormuSutunTurleri.Ad]] = basvurukisi.Ad;

                        row[Sabitler.BasvuruFormlariSutunlari["687"][Enums.BasvuruFormuSutunTurleri.Soyad]] = basvurukisi.Soyad;

                        row[Sabitler.BasvuruFormlariSutunlari["687"][Enums.BasvuruFormuSutunTurleri.TesvikBaslangic]] = basvurukisi.TesvikSuresiBaslangic;

                        row[Sabitler.BasvuruFormlariSutunlari["687"][Enums.BasvuruFormuSutunTurleri.TesvikBitis]] = basvurukisi.TesvikSuresiBitis;

                        row[Sabitler.BasvuruFormlariSutunlari["687"][Enums.BasvuruFormuSutunTurleri.Giris]] = basvurukisi.GirisTarihi;

                        row[Sabitler.BasvuruFormlariSutunlari["687"][Enums.BasvuruFormuSutunTurleri.Baz]] = basvurukisi.Baz;

                        row[Sabitler.BasvuruFormlariSutunlari["687"][Enums.BasvuruFormuSutunTurleri.AktifMi]] = basvurukisi.Aktif;

                        row[Sabitler.BasvuruFormlariSutunlari["687"][Enums.BasvuruFormuSutunTurleri.Durum]] = String.IsNullOrEmpty(basvurukisi.Durum) ? (basvurukisi.YeniIslemYapildi ? "Kişi sorgulanarak yeni eklendi" : OncedenDosyadaVar ? "Daha önceden dosyada ekli" : "Daha önceden onaylanmış") : basvurukisi.Durum;

                        row[Sabitler.BasvuruFormlariSutunlari["687"][Enums.BasvuruFormuSutunTurleri.KanunNo]] = basvurukisi.KanunNo;

                        if (AraciSutunDegerleri.ContainsKey("687") && AraciSutunDegerleri["687"].ContainsKey(basvurukisi.TcKimlikNo))
                        {
                            row[Sabitler.BasvuruFormlariSutunlari["687"][Enums.BasvuruFormuSutunTurleri.Araci]] = AraciSutunDegerleri["687"][basvurukisi.TcKimlikNo];
                        }
                        else row[Sabitler.BasvuruFormlariSutunlari["687"][Enums.BasvuruFormuSutunTurleri.Araci]] = string.Empty;

                        dtbasvuru.Rows.Add(row);
                    }

                }

                if (bfsira == Enums.BasvuruFormuTurleri.Bf6645)
                {
                    DataTable dtbasvuru = DataTables["6645"];

                    var iseGirisSiraliListe = basvurukisiler6645.OrderBy(p => Convert.ToDateTime(p.GirisTarihi));

                    foreach (var basvurukisi in iseGirisSiraliListe)
                    {

                        bool OncedenDosyadaEkli = false;

                        if (dteski6645 != null)
                        {
                            foreach (DataRow drow in dtbasvuru.Rows)
                            {
                                if (drow[Sabitler.BasvuruFormlariSutunlari["6645"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString() == basvurukisi.TcKimlikNo && drow[Sabitler.BasvuruFormlariSutunlari["6645"][Enums.BasvuruFormuSutunTurleri.AktifMi]].ToString() == basvurukisi.Aktif)
                                {
                                    OncedenDosyadaEkli = true;

                                    break;
                                }
                            }
                        }

                        DataRow row = null;

                        row = dtbasvuru.NewRow();

                        row[Sabitler.BasvuruFormlariSutunlari["6645"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]] = basvurukisi.TcKimlikNo;

                        row[Sabitler.BasvuruFormlariSutunlari["6645"][Enums.BasvuruFormuSutunTurleri.Sicil]] = basvurukisi.Sicil;

                        row[Sabitler.BasvuruFormlariSutunlari["6645"][Enums.BasvuruFormuSutunTurleri.Ad]] = basvurukisi.Ad;

                        row[Sabitler.BasvuruFormlariSutunlari["6645"][Enums.BasvuruFormuSutunTurleri.Soyad]] = basvurukisi.Soyad;

                        row[Sabitler.BasvuruFormlariSutunlari["6645"][Enums.BasvuruFormuSutunTurleri.TesvikBaslangic]] = basvurukisi.TesvikSuresiBaslangic;

                        row[Sabitler.BasvuruFormlariSutunlari["6645"][Enums.BasvuruFormuSutunTurleri.TesvikBitis]] = basvurukisi.TesvikSuresiBitis;

                        row[Sabitler.BasvuruFormlariSutunlari["6645"][Enums.BasvuruFormuSutunTurleri.Baz]] = basvurukisi.Baz;

                        row[Sabitler.BasvuruFormlariSutunlari["6645"][Enums.BasvuruFormuSutunTurleri.AktifMi]] = basvurukisi.Aktif;

                        row[Sabitler.BasvuruFormlariSutunlari["6645"][Enums.BasvuruFormuSutunTurleri.Giris]] = basvurukisi.GirisTarihi;

                        row[Sabitler.BasvuruFormlariSutunlari["6645"][Enums.BasvuruFormuSutunTurleri.IslemTarihi]] = basvurukisi.IslemTarihi;

                        row[Sabitler.BasvuruFormlariSutunlari["6645"][Enums.BasvuruFormuSutunTurleri.Durum]] = String.IsNullOrEmpty(basvurukisi.Durum) ? (basvurukisi.YeniIslemYapildi ? "Kişi sorgulanarak yeni eklendi" : OncedenDosyadaEkli ? "Daha önceden dosyada ekli" : "Daha önceden onaylanmış") : basvurukisi.Durum;

                        if (AraciSutunDegerleri.ContainsKey("6645") && AraciSutunDegerleri["6645"].ContainsKey(basvurukisi.TcKimlikNo))
                        {
                            row[Sabitler.BasvuruFormlariSutunlari["6645"][Enums.BasvuruFormuSutunTurleri.Araci]] = AraciSutunDegerleri["6645"][basvurukisi.TcKimlikNo];
                        }
                        else row[Sabitler.BasvuruFormlariSutunlari["6645"][Enums.BasvuruFormuSutunTurleri.Araci]] = string.Empty;

                        dtbasvuru.Rows.Add(row);
                    }

                }

                if (bfsira == Enums.BasvuruFormuTurleri.Bf14857)
                {
                    DataTable dtbasvuru = DataTables["14857"];

                    foreach (var basvurukisi in basvurukisiler14857)
                    {
                        bool OncedenDosyadaEkli = false;

                        if (dteski14857 != null)
                        {
                            foreach (DataRow drow in dtbasvuru.Rows)
                            {
                                if (drow[Sabitler.BasvuruFormlariSutunlari["14857"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString() == basvurukisi.TcKimlikNo)
                                {
                                    OncedenDosyadaEkli = true;

                                    break;
                                }
                            }
                        }

                        DataRow row = null;

                        row = dtbasvuru.NewRow();

                        row[Sabitler.BasvuruFormlariSutunlari["14857"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]] = basvurukisi.TcKimlikNo;

                        row[Sabitler.BasvuruFormlariSutunlari["14857"][Enums.BasvuruFormuSutunTurleri.Sicil]] = basvurukisi.Sicil;

                        row[Sabitler.BasvuruFormlariSutunlari["14857"][Enums.BasvuruFormuSutunTurleri.Ad]] = basvurukisi.Ad;

                        row[Sabitler.BasvuruFormlariSutunlari["14857"][Enums.BasvuruFormuSutunTurleri.Soyad]] = basvurukisi.Soyad;

                        row[Sabitler.BasvuruFormlariSutunlari["14857"][Enums.BasvuruFormuSutunTurleri.TesvikBaslangic]] = basvurukisi.TesvikSuresiBaslangic;

                        row[Sabitler.BasvuruFormlariSutunlari["14857"][Enums.BasvuruFormuSutunTurleri.TesvikBitis]] = basvurukisi.TesvikSuresiBitis;

                        row[Sabitler.BasvuruFormlariSutunlari["14857"][Enums.BasvuruFormuSutunTurleri.RaporNo]] = basvurukisi.RaporNo;

                        row[Sabitler.BasvuruFormlariSutunlari["14857"][Enums.BasvuruFormuSutunTurleri.OzurOrani]] = basvurukisi.OzurOrani;

                        row[Sabitler.BasvuruFormlariSutunlari["14857"][Enums.BasvuruFormuSutunTurleri.Durum]] = String.IsNullOrEmpty(basvurukisi.Durum) ? (basvurukisi.YeniIslemYapildi ? "Kişi sorgulanarak yeni eklendi" : OncedenDosyadaEkli ? "Daha önceden dosyada ekli" : "Daha önceden onaylanmış") : basvurukisi.Durum;

                        if (AraciSutunDegerleri.ContainsKey("14857") && AraciSutunDegerleri["14857"].ContainsKey(basvurukisi.TcKimlikNo))
                        {
                            row[Sabitler.BasvuruFormlariSutunlari["14857"][Enums.BasvuruFormuSutunTurleri.Araci]] = AraciSutunDegerleri["14857"][basvurukisi.TcKimlikNo];
                        }
                        else row[Sabitler.BasvuruFormlariSutunlari["14857"][Enums.BasvuruFormuSutunTurleri.Araci]] = string.Empty;

                        dtbasvuru.Rows.Add(row);
                    }

                }

                if (bfsira == Enums.BasvuruFormuTurleri.Bf7103 || bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                {
                    DataTable dtbasvuru = DataTables["7103"];

                    dtbasvuru.Rows.Clear();

                    var iseGirisSiraliListe = basvurukisiler7103.OrderBy(p => Convert.ToDateTime(p.GirisTarihi));

                    foreach (var basvurukisi in iseGirisSiraliListe)
                    {

                        List<DataRow> rows = new List<DataRow>();

                        DataRow row = null;

                        if (rows.Count == 0)
                        {

                            row = dtbasvuru.NewRow();

                            row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]] = basvurukisi.TcKimlikNo;

                            row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.Sicil]] = basvurukisi.Sicil;

                            row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.Ad]] = basvurukisi.Ad;

                            row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.Soyad]] = basvurukisi.Soyad;

                            row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.TesvikBaslangic]] = basvurukisi.TesvikSuresiBaslangic;

                            row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.TesvikBitis]] = basvurukisi.TesvikSuresiBitis;

                            row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.Baz]] = basvurukisi.Baz;

                            row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.KanunNo]] = basvurukisi.KanunNo;

                            row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.Giris]] = basvurukisi.GirisTarihi;

                            if (!string.IsNullOrEmpty(basvurukisi.CikisTarihi))
                                row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.Cikis]] = basvurukisi.CikisTarihi;
                            else
                                row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.Cikis]] = DBNull.Value;

                            if (!string.IsNullOrEmpty(basvurukisi.IlkTanimlamaTarihi))

                                row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi]] = basvurukisi.IlkTanimlamaTarihi;
                            else
                                row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi]] = DBNull.Value;

                            if (AraciSutunDegerleri.ContainsKey("7103") && AraciSutunDegerleri["7103"].ContainsKey(basvurukisi.TcKimlikNo))
                            {
                                row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.Araci]] = AraciSutunDegerleri["7103"][basvurukisi.TcKimlikNo];
                            }
                            else row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.Araci]] = string.Empty;


                            row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.UcretDestegiTercihi7103]] = basvurukisi.UcretDestegiTercihi;
                            row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.PrimveUcretDestegiIcinBaslangicDonemi]] = basvurukisi.PrimveUcretDestegiIcinBaslangicDonemi;
                            row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.PrimveUcretDestegiIcinBitisDonemi]] = basvurukisi.PrimveUcretDestegiIcinBitisDonemi;
                            row[Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.PrimveUcretDestegiIcinIlaveOlunacakSayi]] = basvurukisi.PrimveUcretDestegiIcinIlaveOlunacakSayi;

                            dtbasvuru.Rows.Add(row);

                        }
                    }

                }

                if (bfsira == Enums.BasvuruFormuTurleri.Bf2828 || bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                {
                    DataTable dtbasvuru = DataTables["2828"];

                    dtbasvuru.Rows.Clear();

                    var iseGirisSiraliListe = basvurukisiler2828.OrderBy(p => Convert.ToDateTime(p.GirisTarihi));

                    foreach (var basvurukisi in iseGirisSiraliListe)
                    {

                        List<DataRow> rows = new List<DataRow>();

                        DataRow row = null;

                        if (rows.Count == 0)
                        {

                            row = dtbasvuru.NewRow();

                            row[Sabitler.BasvuruFormlariSutunlari["2828"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]] = basvurukisi.TcKimlikNo;

                            row[Sabitler.BasvuruFormlariSutunlari["2828"][Enums.BasvuruFormuSutunTurleri.Sicil]] = basvurukisi.Sicil;

                            row[Sabitler.BasvuruFormlariSutunlari["2828"][Enums.BasvuruFormuSutunTurleri.Ad]] = basvurukisi.Ad;

                            row[Sabitler.BasvuruFormlariSutunlari["2828"][Enums.BasvuruFormuSutunTurleri.Soyad]] = basvurukisi.Soyad;

                            row[Sabitler.BasvuruFormlariSutunlari["2828"][Enums.BasvuruFormuSutunTurleri.TesvikBaslangic]] = basvurukisi.TesvikSuresiBaslangic;

                            row[Sabitler.BasvuruFormlariSutunlari["2828"][Enums.BasvuruFormuSutunTurleri.TesvikBitis]] = basvurukisi.TesvikSuresiBitis;

                            row[Sabitler.BasvuruFormlariSutunlari["2828"][Enums.BasvuruFormuSutunTurleri.TesvikSuresi2828]] = basvurukisi.TesvikSuresi;

                            row[Sabitler.BasvuruFormlariSutunlari["2828"][Enums.BasvuruFormuSutunTurleri.Giris]] = basvurukisi.GirisTarihi;

                            if (!string.IsNullOrEmpty(basvurukisi.CikisTarihi))
                                row[Sabitler.BasvuruFormlariSutunlari["2828"][Enums.BasvuruFormuSutunTurleri.Cikis]] = basvurukisi.CikisTarihi;
                            else
                                row[Sabitler.BasvuruFormlariSutunlari["2828"][Enums.BasvuruFormuSutunTurleri.Cikis]] = DBNull.Value;

                            if (!string.IsNullOrEmpty(basvurukisi.IlkTanimlamaTarihi))

                                row[Sabitler.BasvuruFormlariSutunlari["2828"][Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi]] = basvurukisi.IlkTanimlamaTarihi;
                            else
                                row[Sabitler.BasvuruFormlariSutunlari["2828"][Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi]] = DBNull.Value;

                            if (AraciSutunDegerleri.ContainsKey("2828") && AraciSutunDegerleri["2828"].ContainsKey(basvurukisi.TcKimlikNo))
                            {
                                row[Sabitler.BasvuruFormlariSutunlari["2828"][Enums.BasvuruFormuSutunTurleri.Araci]] = AraciSutunDegerleri["2828"][basvurukisi.TcKimlikNo];
                            }
                            else row[Sabitler.BasvuruFormlariSutunlari["2828"][Enums.BasvuruFormuSutunTurleri.Araci]] = string.Empty;

                            dtbasvuru.Rows.Add(row);

                        }
                    }

                }

                if (bfsira == Enums.BasvuruFormuTurleri.Bf7252 || (bfsira == Enums.BasvuruFormuTurleri.BfTumu && !CariTanimla))
                {
                    DataTable dtbasvuru = DataTables["7252"];

                    dtbasvuru.Rows.Clear();

                    var iseGirisSiraliListe = basvurukisiler7252.OrderBy(p => Convert.ToDateTime(p.IseGirisTarihi));

                    foreach (var basvurukisi in iseGirisSiraliListe)
                    {

                        List<DataRow> rows = new List<DataRow>();

                        DataRow row = null;

                        if (rows.Count == 0)
                        {

                            row = dtbasvuru.NewRow();

                            row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]] = basvurukisi.TcKimlikNo;

                            row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.Sicil]] = basvurukisi.Sicil;

                            row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.Ad]] = basvurukisi.Ad;

                            row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.Soyad]] = basvurukisi.Soyad;

                            row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.TesvikBaslangic]] = basvurukisi.TesvikSuresiBaslangic;

                            row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.TesvikBitis]] = basvurukisi.TesvikSuresiBitis;

                            row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.KCUNUDSonlanmaTarihi]] = basvurukisi.KCONUDSonlanmaTarihi;

                            row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.Baz]] = basvurukisi.OrtalamaGunSayisi;

                            row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.KanunNo]] = basvurukisi.KanunNumarası;

                            row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.Giris]] = basvurukisi.IseGirisTarihi;

                            if (!string.IsNullOrEmpty(basvurukisi.IstenAyrilisTarihi))
                                row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.Cikis]] = basvurukisi.IstenAyrilisTarihi;
                            else
                                row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.Cikis]] = DBNull.Value;

                            if (!string.IsNullOrEmpty(basvurukisi.IlkTanimlamaTarihi))

                                row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi]] = basvurukisi.IlkTanimlamaTarihi;
                            else
                                row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi]] = DBNull.Value;

                            if (AraciSutunDegerleri.ContainsKey("7252") && AraciSutunDegerleri["7252"].ContainsKey(basvurukisi.TcKimlikNo))
                            {
                                row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.Araci]] = AraciSutunDegerleri["7252"][basvurukisi.TcKimlikNo];
                            }
                            else row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.Araci]] = string.Empty;

                            dtbasvuru.Rows.Add(row);

                        }
                    }

                }

                //if (bfsira == Enums.BasvuruFormuTurleri.Bf7256 || bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                //{
                //    DataTable dtbasvuru = DataTables["17256"];

                //    dtbasvuru.Rows.Clear();

                //    var iseGirisSiraliListe = basvurukisiler17256.OrderBy(p => Convert.ToDateTime(p.IseGirisTarihi));

                //    foreach (var basvurukisi in iseGirisSiraliListe)
                //    {

                //        List<DataRow> rows = new List<DataRow>();

                //        DataRow row = null;

                //        if (rows.Count == 0)
                //        {

                //            row = dtbasvuru.NewRow();

                //            row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]] = basvurukisi.TcKimlikNo;

                //            row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.Sicil]] = basvurukisi.Sicil;

                //            row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.Ad]] = basvurukisi.Ad;

                //            row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.Soyad]] = basvurukisi.Soyad;

                //            row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.TesvikBaslangic]] = basvurukisi.TesvikSuresiBaslangic;

                //            row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.TesvikBitis]] = basvurukisi.TesvikSuresiBitis;

                //            row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.KanunNo]] = basvurukisi.KanunNumarası;

                //            row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.Giris]] = basvurukisi.IseGirisTarihi;

                //            if (!string.IsNullOrEmpty(basvurukisi.IstenAyrilisTarihi))
                //                row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.Cikis]] = basvurukisi.IstenAyrilisTarihi;
                //            else
                //                row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.Cikis]] = DBNull.Value;

                //            if (!string.IsNullOrEmpty(basvurukisi.IlkTanimlamaTarihi))

                //                row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi]] = basvurukisi.IlkTanimlamaTarihi;
                //            else
                //                row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi]] = DBNull.Value;

                //            if (!string.IsNullOrEmpty(basvurukisi.SigortalininIsyerineBasvuruTarihi))

                //                row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.SigortalininIsyerineBasvuruTarihi]] = basvurukisi.SigortalininIsyerineBasvuruTarihi;
                //            else
                //                row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.SigortalininIsyerineBasvuruTarihi]] = DBNull.Value;

                //            if (!string.IsNullOrEmpty(basvurukisi.SigortaliIcinTercihDurumu))

                //                row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.SigortaliIcinTercihDurumu]] = basvurukisi.SigortaliIcinTercihDurumu;
                //            else
                //                row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.SigortaliIcinTercihDurumu]] = DBNull.Value;

                //            if (AraciSutunDegerleri.ContainsKey("17256") && AraciSutunDegerleri["17256"].ContainsKey(basvurukisi.TcKimlikNo))
                //            {
                //                row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.Araci]] = AraciSutunDegerleri["17256"][basvurukisi.TcKimlikNo];
                //            }
                //            else row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.Araci]] = string.Empty;

                //            if (Kayitlar7256VerilsinMi.ContainsKey("17256") && Kayitlar7256VerilsinMi["17256"].ContainsKey(basvurukisi.TcKimlikNo))
                //            {
                //                row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.VerilsinMi7256]] = Kayitlar7256VerilsinMi["17256"][basvurukisi.TcKimlikNo];
                //            }
                //            else row[Sabitler.BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.Araci]] = string.Empty;

                //            dtbasvuru.Rows.Add(row);

                //        }
                //    }

                //}

                if (bfsira == Enums.BasvuruFormuTurleri.Bf7256 || (bfsira == Enums.BasvuruFormuTurleri.BfTumu && !CariTanimla))
                {
                    DataTable dtbasvuru = DataTables["27256"];

                    dtbasvuru.Rows.Clear();

                    var iseGirisSiraliListe = basvurukisiler27256.OrderBy(p => Convert.ToDateTime(p.IseGirisTarihi));

                    foreach (var basvurukisi in iseGirisSiraliListe)
                    {

                        List<DataRow> rows = new List<DataRow>();

                        DataRow row = null;

                        if (rows.Count == 0)
                        {

                            row = dtbasvuru.NewRow();

                            row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]] = basvurukisi.TcKimlikNo;

                            row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.Sicil]] = basvurukisi.Sicil;

                            row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.Ad]] = basvurukisi.Ad;

                            row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.Soyad]] = basvurukisi.Soyad;

                            row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.TesvikBaslangic]] = basvurukisi.TesvikSuresiBaslangic;

                            row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.TesvikBitis]] = basvurukisi.TesvikSuresiBitis;

                            row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.KanunNo]] = basvurukisi.KanunNumarası;

                            row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.Giris]] = basvurukisi.IseGirisTarihi;

                            row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.Baz]] = basvurukisi.IlaveOlunmasiGerekenSayi;

                            if (!string.IsNullOrEmpty(basvurukisi.IstenAyrilisTarihi))
                                row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.Cikis]] = basvurukisi.IstenAyrilisTarihi;
                            else
                                row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.Cikis]] = DBNull.Value;

                            if (!string.IsNullOrEmpty(basvurukisi.IlkTanimlamaTarihi))

                                row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi]] = basvurukisi.IlkTanimlamaTarihi;
                            else
                                row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi]] = DBNull.Value;

                            if (AraciSutunDegerleri.ContainsKey("27256") && AraciSutunDegerleri["27256"].ContainsKey(basvurukisi.TcKimlikNo))
                            {
                                row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.Araci]] = AraciSutunDegerleri["27256"][basvurukisi.TcKimlikNo];
                            }
                            else row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.Araci]] = string.Empty;

                            if (Kayitlar7256VerilsinMi.ContainsKey("27256") && Kayitlar7256VerilsinMi["27256"].ContainsKey(basvurukisi.TcKimlikNo))
                            {
                                row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.VerilsinMi7256]] = Kayitlar7256VerilsinMi["27256"][basvurukisi.TcKimlikNo];
                            }
                            else row[Sabitler.BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.Araci]] = string.Empty;

                            dtbasvuru.Rows.Add(row);

                        }
                    }

                }

                if (bfsira == Enums.BasvuruFormuTurleri.Bf7316 || (bfsira == Enums.BasvuruFormuTurleri.BfTumu && !CariTanimla))
                {
                    DataTable dtbasvuru = DataTables["7316"];

                    dtbasvuru.Rows.Clear();

                    var iseGirisSiraliListe = basvurukisiler7316.OrderBy(p => Convert.ToDateTime(p.IseGirisTarihi));

                    foreach (var basvurukisi in iseGirisSiraliListe)
                    {

                        List<DataRow> rows = new List<DataRow>();

                        DataRow row = null;

                        if (rows.Count == 0)
                        {

                            row = dtbasvuru.NewRow();

                            row[Sabitler.BasvuruFormlariSutunlari["7316"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]] = basvurukisi.TcKimlikNo;

                            row[Sabitler.BasvuruFormlariSutunlari["7316"][Enums.BasvuruFormuSutunTurleri.Sicil]] = basvurukisi.Sicil;

                            row[Sabitler.BasvuruFormlariSutunlari["7316"][Enums.BasvuruFormuSutunTurleri.Ad]] = basvurukisi.Ad;

                            row[Sabitler.BasvuruFormlariSutunlari["7316"][Enums.BasvuruFormuSutunTurleri.Soyad]] = basvurukisi.Soyad;

                            row[Sabitler.BasvuruFormlariSutunlari["7316"][Enums.BasvuruFormuSutunTurleri.TesvikBaslangic]] = basvurukisi.TesvikSuresiBaslangic;

                            row[Sabitler.BasvuruFormlariSutunlari["7316"][Enums.BasvuruFormuSutunTurleri.TesvikBitis]] = basvurukisi.TesvikSuresiBitis;

                            row[Sabitler.BasvuruFormlariSutunlari["7316"][Enums.BasvuruFormuSutunTurleri.KanunNo]] = basvurukisi.KanunNumarası;

                            row[Sabitler.BasvuruFormlariSutunlari["7316"][Enums.BasvuruFormuSutunTurleri.Giris]] = basvurukisi.IseGirisTarihi;

                            if (!string.IsNullOrEmpty(basvurukisi.IstenAyrilisTarihi))
                                row[Sabitler.BasvuruFormlariSutunlari["7316"][Enums.BasvuruFormuSutunTurleri.Cikis]] = basvurukisi.IstenAyrilisTarihi;
                            else
                                row[Sabitler.BasvuruFormlariSutunlari["7316"][Enums.BasvuruFormuSutunTurleri.Cikis]] = DBNull.Value;

                            if (!string.IsNullOrEmpty(basvurukisi.IlkTanimlamaTarihi))

                                row[Sabitler.BasvuruFormlariSutunlari["7316"][Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi]] = basvurukisi.IlkTanimlamaTarihi;
                            else
                                row[Sabitler.BasvuruFormlariSutunlari["7316"][Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi]] = DBNull.Value;

                            if (AraciSutunDegerleri.ContainsKey("7316") && AraciSutunDegerleri["7316"].ContainsKey(basvurukisi.TcKimlikNo))
                            {
                                row[Sabitler.BasvuruFormlariSutunlari["7316"][Enums.BasvuruFormuSutunTurleri.Araci]] = AraciSutunDegerleri["7316"][basvurukisi.TcKimlikNo];
                            }
                            else row[Sabitler.BasvuruFormlariSutunlari["7316"][Enums.BasvuruFormuSutunTurleri.Araci]] = string.Empty;


                            dtbasvuru.Rows.Add(row);

                        }
                    }

                }

                if (bfsira == Enums.BasvuruFormuTurleri.Bf3294 || bfsira == Enums.BasvuruFormuTurleri.BfTumu)
                {
                    DataTable dtbasvuru = DataTables["3294"];

                    dtbasvuru.Rows.Clear();

                    var iseGirisSiraliListe = basvurukisiler3294.OrderBy(p => Convert.ToDateTime(p.IseGirisTarihi));

                    foreach (var basvurukisi in iseGirisSiraliListe)
                    {

                        List<DataRow> rows = new List<DataRow>();

                        DataRow row = null;

                        if (rows.Count == 0)
                        {

                            row = dtbasvuru.NewRow();

                            row[Sabitler.BasvuruFormlariSutunlari["3294"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]] = basvurukisi.TcKimlikNo;

                            row[Sabitler.BasvuruFormlariSutunlari["3294"][Enums.BasvuruFormuSutunTurleri.Sicil]] = basvurukisi.Sicil;

                            row[Sabitler.BasvuruFormlariSutunlari["3294"][Enums.BasvuruFormuSutunTurleri.Ad]] = basvurukisi.Ad;

                            row[Sabitler.BasvuruFormlariSutunlari["3294"][Enums.BasvuruFormuSutunTurleri.Soyad]] = basvurukisi.Soyad;

                            row[Sabitler.BasvuruFormlariSutunlari["3294"][Enums.BasvuruFormuSutunTurleri.TesvikBaslangic]] = basvurukisi.TesvikSuresiBaslangic;

                            row[Sabitler.BasvuruFormlariSutunlari["3294"][Enums.BasvuruFormuSutunTurleri.TesvikBitis]] = basvurukisi.TesvikSuresiBitis;

                            row[Sabitler.BasvuruFormlariSutunlari["3294"][Enums.BasvuruFormuSutunTurleri.Baz]] = basvurukisi.OrtalamaSigortaliSayisi;

                            row[Sabitler.BasvuruFormlariSutunlari["3294"][Enums.BasvuruFormuSutunTurleri.Giris]] = basvurukisi.IseGirisTarihi;

                            if (!string.IsNullOrEmpty(basvurukisi.IstenAyrilisTarihi))
                                row[Sabitler.BasvuruFormlariSutunlari["3294"][Enums.BasvuruFormuSutunTurleri.Cikis]] = basvurukisi.IstenAyrilisTarihi;
                            else
                                row[Sabitler.BasvuruFormlariSutunlari["3294"][Enums.BasvuruFormuSutunTurleri.Cikis]] = DBNull.Value;

                            if (!string.IsNullOrEmpty(basvurukisi.IlkTanimlamaTarihi))

                                row[Sabitler.BasvuruFormlariSutunlari["3294"][Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi]] = basvurukisi.IlkTanimlamaTarihi;
                            else
                                row[Sabitler.BasvuruFormlariSutunlari["3294"][Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi]] = DBNull.Value;

                            if (AraciSutunDegerleri.ContainsKey("3294") && AraciSutunDegerleri["3294"].ContainsKey(basvurukisi.TcKimlikNo))
                            {
                                row[Sabitler.BasvuruFormlariSutunlari["3294"][Enums.BasvuruFormuSutunTurleri.Araci]] = AraciSutunDegerleri["3294"][basvurukisi.TcKimlikNo];
                            }
                            else row[Sabitler.BasvuruFormlariSutunlari["3294"][Enums.BasvuruFormuSutunTurleri.Araci]] = string.Empty;


                            dtbasvuru.Rows.Add(row);

                        }
                    }

                }

                foreach (var item in DataTables)
                {
                    KanunNo = item.Key;

                    if (KanunNo == "7252" && bfsira == Enums.BasvuruFormuTurleri.BfTumu && CariTanimla) continue;

                    DataTable dtbasvuru = item.Value;

                    DataView dv = dtbasvuru.DefaultView;

                    if (dv.Count > 0 || KanunNo == "7252")
                    {
                        BasvuruLogEkle("Excele kaydetme başlatıldı.Lütfen bekleyiniz.");

                        if (Metodlar.FormKaydet(SuanYapilanIsyeriBasvuru, dv.ToTable(), null, Enums.FormTuru.BasvuruFormu, KanunNo) != null)
                        {
                            KaydedilenFormVar = true;

                            BasvuruLogEkle(KanunNo + " Başvuru formu başarılı bir şekilde kaydedildi.");

                            if (!HataVarmi)
                            {
                                BasvuruLogEkle("\"" + SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBasvuru.SubeAdi + "\" işyeri için " + KanunNo + " başvuru formu indirme tamamlandı. Toplam " + (dtbasvuru.Rows.Count) + " kişi başvuru formuna eklendi");
                            }
                            else
                            {
                                BasvuruLogEkle("\"" + SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBasvuru.SubeAdi + "\" işyeri için " + KanunNo + " başvuru formu indirme işlemi \"Hata\" nedeniyle sonlandırıldı. Şu ana kadar toplam " + (dtbasvuru.Rows.Count) + " kişi başvuru formuna eklendi" + (hatamesaji != null ? ". Hata Mesajı:" + hatamesaji : ""));
                            }
                        }
                        else
                        {
                            if (!HataVarmi)
                            {
                                BasvuruLogEkle("\"" + SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBasvuru.SubeAdi + "\" işyeri için " + KanunNo + " başvuru formu indirme tamamlandı fakat başvuru formunu kaydedilemedi  ");
                            }
                            else
                            {
                                BasvuruLogEkle("\"" + SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBasvuru.SubeAdi + "\" işyeri için " + KanunNo + " başvuru formu indirme işlemi \"Hata\" nedeniyle sonlandırıldı.Aynı zamanda indirilen kayıtlar başvuru formuna kaydedilemedi" + (hatamesaji != null ? ". Hata Mesajı:" + hatamesaji : ""));
                            }
                        }

                        if (KanunNo == "7103")
                        {
                            if (BasvuruListesi7166yaEklenecekKisiler.Count > 0)
                            {

                                DataTable dt7166 = new DataTable("dt7166Listesi");

                                var sutunlar7166 = Enum.GetNames(typeof(Enums.BasvuruListesi7166SutunTurleri));

                                foreach (var sutun in sutunlar7166)
                                {

                                    DataColumn column = new DataColumn();

                                    column.DataType = typeof(string);

                                    column.AllowDBNull = true;

                                    dt7166.Columns.Add(column);
                                }

                                BasvuruListesi7166yaEklenecekKisiler = BasvuruListesi7166yaEklenecekKisiler.OrderByDescending(p => p.Giris).ToList();

                                int sira = 1;

                                foreach (var kisi7166 in BasvuruListesi7166yaEklenecekKisiler)
                                {
                                    string SilinememeNedeni = "";

                                    var silinemeyenKayit = SisteminSildirmedigi7103Kayitlari.FirstOrDefault(p => p.Key.TcKimlikNo.Equals(kisi7166.TckimlikNo) && Convert.ToDateTime(p.Key.GirisTarihi).Equals(kisi7166.Giris));

                                    if (silinemeyenKayit.Key != null)
                                    {

                                        SilinememeNedeni = silinemeyenKayit.Value.Contains("ilgili kanundan bildirimde bulunulduğundan") ? "Onaylı belge var" : silinemeyenKayit.Value.Contains("ilgili kanundan onay bekleyen bildirge bulunulduğundan") ? "Onaysız belge var" : silinemeyenKayit.Value;
                                    }

                                    string UygunlukDurumu = "";
                                    string UygunlukDurumuNedeni = "";

                                    if (kisi7166.UygunlukDurumu.Equals("Uygun Değildir"))
                                    {
                                        UygunlukDurumu = kisi7166.UygunlukDurumu;
                                        UygunlukDurumuNedeni = kisi7166.UygunlukDurumuNedeni;
                                    }
                                    else
                                    {
                                        UygunlukDurumu = kisi7166.UygunlukDurumu;
                                        UygunlukDurumuNedeni = kisi7166.UygunlukDurumuNedeni;

                                    }

                                    DataRow row = dt7166.NewRow();

                                    row[(int)Enums.BasvuruListesi7166SutunTurleri.SiraNo] = sira.ToString();
                                    row[(int)Enums.BasvuruListesi7166SutunTurleri.TcKimlikNoSosyalGuvenlikNo] = kisi7166.TckimlikNo;
                                    row[(int)Enums.BasvuruListesi7166SutunTurleri.Ad] = kisi7166.Ad;
                                    row[(int)Enums.BasvuruListesi7166SutunTurleri.Soyad] = kisi7166.Soyad;
                                    row[(int)Enums.BasvuruListesi7166SutunTurleri.Giris] = kisi7166.Giris.ToString("dd.MM.yyyy");

                                    if (!string.IsNullOrEmpty(kisi7166.Cikis))
                                    {
                                        row[(int)Enums.BasvuruListesi7166SutunTurleri.Cikis] = Convert.ToDateTime(kisi7166.Cikis).ToString("dd.MM.yyyy");
                                    }

                                    row[(int)Enums.BasvuruListesi7166SutunTurleri.IstenCikisNedeni] = kisi7166.IstenCikisNedeni;
                                    row[(int)Enums.BasvuruListesi7166SutunTurleri.UygunlukDurumu] = UygunlukDurumu; //UygunlukDurumu.Equals("Uygundur") && (eskikayit == null || string.IsNullOrEmpty(eskikayit.UygunlukDurumu)) ? "" : UygunlukDurumu;
                                    row[(int)Enums.BasvuruListesi7166SutunTurleri.UygunlukDurumuNedeni] = UygunlukDurumuNedeni;
                                    row[(int)Enums.BasvuruListesi7166SutunTurleri.VerilmisMi7103] = kisi7166.VerilmisMi7103 ? "Evet" : "Hayır";
                                    row[(int)Enums.BasvuruListesi7166SutunTurleri.SilinememeNedeni] = SilinememeNedeni;

                                    dt7166.Rows.Add(row);

                                    sira++;
                                }

                                BasvuruLogEkle("7166 Listesi kaydediliyor.Lütfen bekleyiniz.");

                                if (Metodlar.FormKaydet(SuanYapilanIsyeriBasvuru, dt7166, null, Enums.FormTuru.BasvuruListesi7166, "7166") != null)
                                {
                                    BasvuruLogEkle("7166 listesi başarılı bir şekilde kaydedildi.");

                                    BasvuruLogEkle("\"" + SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBasvuru.SubeAdi + "\" işyeri için 7166 listesi kaydedildi. Toplam " + (dt7166.Rows.Count) + " kişi listeye eklendi");

                                }
                                else
                                {
                                    BasvuruLogEkle("\"" + SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBasvuru.SubeAdi + "\" işyeri için 7166 listesi kaydedilemedi");
                                }

                            }
                        }
                        else if (KanunNo == "6111")
                        {
                            if (dtEgitim != null && dtEgitim.Rows.Count > 0)
                            {
                                BasvuruLogEkle("6111 Eğitim belgesi verilecekler listesi kaydediliyor.Lütfen bekleyiniz.");

                                if (Metodlar.EgitimListesiKaydet(SuanYapilanIsyeriBasvuru, dtEgitim) != null)
                                {
                                    BasvuruLogEkle("6111 Eğitim belgesi verilecekler listesi başarılı bir şekilde kaydedildi.");

                                    BasvuruLogEkle("\"" + SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBasvuru.SubeAdi + "\" işyeri için 6111 Eğitim belgesi verilecekler listesi kaydedildi. Toplam " + (dtEgitim.Rows.Count) + " kişi listeye eklendi");

                                }
                                else
                                {
                                    BasvuruLogEkle("\"" + SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBasvuru.SubeAdi + "\" işyeri için 6111 Eğitim belgesi verilecekler listesi kaydedilemedi");
                                }
                            }
                        }
                        else if (KanunNo == "7252")
                        {
                            if (dv.Count > 0)
                            {

                                BasvuruLogEkle("7252 listesi kaydediliyor.Lütfen bekleyiniz.");

                                if (Metodlar.Liste7252Kaydet(SuanYapilanIsyeriBasvuru, dtbasvuru) != null)
                                {
                                    BasvuruLogEkle("7252 listesi başarılı bir şekilde kaydedildi.");

                                    BasvuruLogEkle("\"" + SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBasvuru.SubeAdi + "\" işyeri için 7252 listesi kaydedildi. Toplam " + (dtbasvuru.Rows.Count) + " kişi listeye eklendi");

                                }
                                else
                                {
                                    BasvuruLogEkle("\"" + SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBasvuru.SubeAdi + "\" işyeri için 7252 listesi kaydedilemedi");
                                }
                            }
                            else
                            {
                                var silme7252Sonuc = Metodlar.Liste7252Kaydet(SuanYapilanIsyeriBasvuru, dtbasvuru);

                                if (silme7252Sonuc != null && silme7252Sonuc == "7252 listesi silindi")
                                {
                                    BasvuruLogEkle("7252 listesi başarılı bir şekilde silindi");
                                }

                            }
                        }
                    }
                    else
                    {
                        if (!HataVarmi)
                        {
                            BasvuruLogEkle("\"" + SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBasvuru.SubeAdi + "\" işyeri için " + KanunNo + " başvuru formu indirme tamamlandı. Toplam " + (dtbasvuru.Rows.Count) + " kişi başvuru formuna eklendi");
                        }
                        else
                        {
                            BasvuruLogEkle("\"" + SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBasvuru.SubeAdi + "\" işyeri için " + KanunNo + " başvuru formu indirme işlemi \"Hata\" nedeniyle sonlandırıldı. Şu ana kadar toplam " + (dtbasvuru.Rows.Count) + " kişi başvuru formuna eklendi" + (hatamesaji != null ? ". Hata Mesajı:" + hatamesaji : ""));
                        }
                    }
                }
            }
            else
            {
                if (!HataVarmi)
                {
                    BasvuruLogEkle("\"" + SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBasvuru.SubeAdi + "\" işyeri için " + KanunNo + " başvuru formu indirme işlemi başvuru formu kaydedilmeden tamamlandı. ");
                }
                else
                {
                    BasvuruLogEkle("\"" + SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBasvuru.SubeAdi + "\" işyeri için " + KanunNo + " başvuru formu indirme işlemi başvuru formu \"Hata\" nedeniyle kaydedilmeden tamamlandı" + (hatamesaji != null ? ". Hata Mesajı:" + hatamesaji : ""));
                }

            }

            LogYaz(loglar);


            Task.Factory.StartNew(() =>
            {
                var bvc = new ProjeGiris(SuanYapilanIsyeriBasvuru, BasvuruWebClient.proje);
                bvc.Cookie = BasvuruWebClient.Cookie;

                try
                {
                    bvc.Disconnect(true);
                }
                catch { }

                BasvuruWebClient.Bitti = true;

                if (sigortaliIstenAyrilisProjesiConnect != null)
                {
                    var sia = new ProjeGiris(SuanYapilanIsyeriBasvuru, Enums.ProjeTurleri.SigortaliIstenAyrilis);
                    sia.Cookie = sigortaliIstenAyrilisProjesiConnect.Cookie;

                    try
                    {
                        sia.Disconnect(true);
                    }
                    catch { }

                    sigortaliIstenAyrilisProjesiConnect.Bitti = true;

                }
            });


            try
            {
                tokenSource.Cancel();
            }
            catch { }

            //DegiskenTemizle();

            Metodlar.DetayliLogYaz("Başvuru sona erdi");
        }

        public static void LogYaz(List<BasvuruLog> loglar)
        {
            int i = 0;

            var donemgruplari = loglar.Where(p => !p.Donem.Equals(-1)).GroupBy(p => p.Donem).OrderBy(p => p.Key);

            var logs = loglar.Where(p => p.Donem.Equals(-1)).ToList();

            foreach (var item in donemgruplari)
            {
                var donemlogs = item.Where(p => p.KisiNo.Equals(-1)).OrderBy(p => p.Tarih).ToList();

                var donemEnKucukTarih = donemlogs.Min(p => p.Tarih);

                var kisigruplari = item.Where(p => !p.KisiNo.Equals(-1)).GroupBy(p => p.KisiNo).OrderBy(p => p.Key);

                foreach (var kisilog in kisigruplari)
                {
                    var kisilogs = kisilog.OrderBy(p => p.Tarih).ToList();
                    var kisiEnKucukTarih = kisilogs.Min(p => p.Tarih);

                    var kisiindex = donemlogs.FindIndex(p => p.Tarih > kisiEnKucukTarih && p.KisiNo.Equals(-1));

                    if (kisiindex == -1)
                    {
                        donemlogs.AddRange(kisilogs);
                    }
                    else
                    {
                        donemlogs.InsertRange(kisiindex, kisilogs);
                    }
                }

                var donemindex = logs.FindIndex(p => p.Tarih > donemEnKucukTarih && p.Donem.Equals(-1));

                if (donemindex == -1)
                {
                    logs.AddRange(donemlogs);
                }
                else logs.InsertRange(donemindex, donemlogs);
            }

            while (i <= 5)
            {
                i++;

                try
                {
                    var log = String.Join(Environment.NewLine, logs.Select(p => String.Format("{0} : {1}", p.Tarih, p.Mesaj)));

                    File.WriteAllText(Application.StartupPath + "\\log.txt", log);

                    break;
                }
                catch
                {
                    Thread.Sleep(200);
                }
            }

        }

        void BasvuruLoglariGuncelle()
        {
            if (FormLog != null)
            {

                if (FormLog.lbLog.InvokeRequired)
                {
                    FormLog.Invoke(new delLoglariGuncelle(BasvuruLoglariGuncelle));
                }
                else
                {
                    var donemgruplari = loglar.Where(p => !p.Donem.Equals(-1)).GroupBy(p => p.Donem).OrderBy(p => p.Key);

                    var logs = loglar.Where(p => p.Donem.Equals(-1)).ToList();

                    foreach (var item in donemgruplari)
                    {
                        var donemlogs = item.Where(p => p.KisiNo.Equals(-1)).OrderBy(p => p.Tarih).ToList();

                        var donemEnKucukTarih = donemlogs.Min(p => p.Tarih);

                        var kisigruplari = item.Where(p => !p.KisiNo.Equals(-1)).GroupBy(p => p.KisiNo).OrderBy(p => p.Key);

                        foreach (var kisilog in kisigruplari)
                        {
                            var kisilogs = kisilog.OrderBy(p => p.Tarih).ToList();
                            var kisiEnKucukTarih = kisilogs.Min(p => p.Tarih);

                            var kisiindex = donemlogs.FindIndex(p => p.Tarih > kisiEnKucukTarih && p.KisiNo.Equals(-1));

                            if (kisiindex == -1)
                            {
                                donemlogs.AddRange(kisilogs);
                            }
                            else
                            {
                                donemlogs.InsertRange(kisiindex, kisilogs);
                            }
                        }

                        var donemindex = logs.FindIndex(p => p.Tarih > donemEnKucukTarih && p.Donem.Equals(-1));

                        if (donemindex == -1)
                        {
                            logs.AddRange(donemlogs);
                        }
                        else logs.InsertRange(donemindex, donemlogs);
                    }

                    FormLog.LoglariGuncelle(ref logs);

                }
            }

        }

        public void IslemiIptalEt()
        {
            tokenSource.Cancel();

            BasvuruSonaErdi(false, false, "iptal");
        }

        public StringBuilder GetPdfText(byte[] data)
        {
            StringBuilder text = new StringBuilder();
            PdfReader reader = new PdfReader(data);
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                ITextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                string currentText = PdfTextExtractor.GetTextFromPage(reader, page, strategy);
                text.Append(currentText);
            }
            reader.Close();
            return text;
        }

        void TanimlamaYapilanlaraEkle(string kanun, KeyValuePair<string, DateTime> tcNoveIseGirisTarihi)
        {
            if (kanun.Equals("6111")) TanimlamaYapilanlar6111.Add(tcNoveIseGirisTarihi);
            else if (kanun.EndsWith("7103")) TanimlamaYapilanlar7103.Add(tcNoveIseGirisTarihi);
            else if (kanun.EndsWith("2828")) TanimlamaYapilanlar2828.Add(tcNoveIseGirisTarihi);
            else if (kanun.EndsWith("7252")) TanimlamaYapilanlar7252.Add(tcNoveIseGirisTarihi);
            else if (kanun.EndsWith("17256")) TanimlamaYapilanlar17256.Add(tcNoveIseGirisTarihi);
            else if (kanun.EndsWith("27256")) TanimlamaYapilanlar27256.Add(tcNoveIseGirisTarihi);
            else if (kanun.EndsWith("7316")) TanimlamaYapilanlar7316.Add(tcNoveIseGirisTarihi);
            else if (kanun.EndsWith("3294")) TanimlamaYapilanlar3294.Add(tcNoveIseGirisTarihi);
        }

    }
}
