using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TesvikProgrami.Classes
{
    public class AphbIndir : Indir
    {

        public AphbIndir(long IsyeriId)
        {
            using (var dbContext = new DbEntities())
            {
                SuanYapilanIsyeriAphb = dbContext.Isyerleri.Include("Sirketler").Where(p => p.IsyeriID.Equals(IsyeriId)).FirstOrDefault();
            }

            this.IsyeriId = IsyeriId;

            token = tokenSource.Token;

            task = new Task(() => AphbIndirmeBaslat(), token);

        }

        public DateTime TarihBaslangicAphb = DateTime.MinValue;
        public DateTime TarihBitisAphb = DateTime.MinValue;
        public List<Bildirge> Bildirgeler = new List<Bildirge>();
        public Isyerleri SuanYapilanIsyeriAphb = null;
        int ToplamBildirgeSayisi = 0;
        string hizmetyilAyIndex = "";
        string hizmetyilAyIndexBitis = "";
        ProjeGiris AphbWebClient = null;
        List<string> OncedenIndirilenler = new List<string>();
        public List<string> kullanilanAraciIsyeri = new List<string>();
        public string AraciUnvani = string.Empty;
        public HashSet<string> secilenRadios = new HashSet<string>();
        bool OnaysizlarTamamlandi = true;
        bool EBildirge2OnaysizlarTamamlandi = true;
        bool pdfBildirgeHataliOkunduMu = true;
        string bilgiDondurmekIcin = "";
        public bool BasariylaKaydedildi;
        public Dictionary<string, IndirilecekAphb> hataVerenBildirgeler = new Dictionary<string, IndirilecekAphb>();

        void AphbIndirmeBaslat()
        {

            AphbWebClient = new ProjeGiris(SuanYapilanIsyeriAphb, Enums.ProjeTurleri.EBildirgeV1);

            AphbWebClient.Referer = "https://ebildirge.sgk.gov.tr/WPEB/amp/bildirgeonayla";

            LogEkle(string.Format("'{0}' şirketine ait '{1}' işyeri için Aphb indirilmeye başlanıyor", SuanYapilanIsyeriAphb.Sirketler.SirketAdi, SuanYapilanIsyeriAphb.SubeAdi));

            if (dtMevcutAphb == null)
            {
                string aphbyol = Metodlar.FormBul(SuanYapilanIsyeriAphb, Enums.FormTuru.Aphb);
                if (aphbyol != null)
                {
                    dtMevcutAphb = Metodlar.AylikListeyiYukle(aphbyol);
                }
            }

            if (dtMevcutAphb != null && !dtMevcutAphb.Columns.Contains("Bildirge No"))
            {
                try
                {
                    var aphbyol = Metodlar.FormBul(SuanYapilanIsyeriAphb, Enums.FormTuru.Aphb);

                    if (aphbyol != null) File.Delete(aphbyol);
                }
                catch (Exception)
                {

                }

                using (var dbEntities = new DbEntities())
                {
                    var isyeri = dbEntities.Isyerleri.Find(SuanYapilanIsyeriAphb.IsyeriID);

                    isyeri.Aphb = null;

                    dbEntities.SaveChanges();
                }

                dtMevcutAphb = null;
            }

            if (dtMevcutAphb != null)
            {
                DateTime trh = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);

                var groups = dtMevcutAphb.AsEnumerable()
                                    .Where(row => new DateTime(Convert.ToInt32(row[(int)Enums.AphbHucreBilgileri.Yil].ToString()), Convert.ToInt32(row[(int)Enums.AphbHucreBilgileri.Ay].ToString()), 1) < trh && !row[(int)Enums.AphbHucreBilgileri.OnayDurumu].ToString().Trim().Equals("Onaylanmamış"))
                                     .GroupBy(row =>
                                        row[(int)Enums.AphbHucreBilgileri.BildirgeRefNo].ToString().Trim() + "-" +
                                        //row[(int)Enums.AphbHucreBilgileri.Yil].ToString().Trim() + "-" +
                                        //row[(int)Enums.AphbHucreBilgileri.Ay].ToString().Trim().PadLeft(2, '0') + "-" +
                                        //row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().Trim().PadLeft(5, '0') + "-" +
                                        //row[(int)Enums.AphbHucreBilgileri.Mahiyet].ToString().Trim() + "-" +
                                        //row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString().Trim().PadLeft(2, '0') + "-" +
                                        (row[(int)Enums.AphbHucreBilgileri.Araci].ToString().Trim().Contains("-") ? row[(int)Enums.AphbHucreBilgileri.Araci].ToString().Trim().Split('-')[0] : row[(int)Enums.AphbHucreBilgileri.Araci].ToString().Trim()) + "-" +
                                        row[(int)Enums.AphbHucreBilgileri.OnayDurumu].ToString().Trim()

                );

                OncedenIndirilenler = groups.Select(p => p.Key).ToList();
            }

            AphbSayfayiYukle();
        }

        public void Cancel()
        {
            tokenSource.Cancel();

            var Kaydet = false;

            if (Bildirgeler.Any(p => p.Kisiler.Count > 0))
            {
                Kaydet = MessageBox.Show(String.Format("{0} - {1} işyerinin indirilen bildirgelerini kaydetmek istiyor musunuz?", SuanYapilanIsyeriAphb.Sirketler.SirketAdi, SuanYapilanIsyeriAphb.SubeAdi), "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes;
            }

            AphbSonaErdi(Kaydet);
        }

        void IptalKontrolu()
        {
            token.ThrowIfCancellationRequested();
        }

        void LogEkle(string Mesaj)
        {
            sb.Append(String.Format("[{0}] : {1}{2}", DateTime.Now.ToString(), Mesaj, Environment.NewLine));
            new delLoglariGuncelle(LoglariGuncelle).Invoke();
        }

        private void AphbSayfayiYukle()
        {
            IptalKontrolu();

            string girisCevabi = string.Empty;

            AphbWebClient.Disconnect();

            AphbWebClient = new ProjeGiris(SuanYapilanIsyeriAphb, OnaysizlarTamamlandi ? Enums.ProjeTurleri.EBildirgeV2 : Enums.ProjeTurleri.EBildirgeV1);

            if (!OnaysizlarTamamlandi)
            {
                AphbWebClient.Referer = "https://ebildirge.sgk.gov.tr/WPEB/amp/bildirgeonayla";

                LogEkle("E-Bildirgeye giriş yapılıyor");
            }
            else if (!EBildirge2OnaysizlarTamamlandi)
            {
                LogEkle("E-Bildirge V2 onaysız bildirgelere bakılıyor");
            }
            else LogEkle("E-Bildirge V2 onaylı bildirgelere bakılıyor");

            for (int i = 0; i < 20; i++)
            {
                IptalKontrolu();

                AphbWebClient.Connect();

                if (AphbWebClient.Connected || AphbWebClient.GirisYapilamiyor)
                {
                    break;
                }
                else System.Threading.Thread.Sleep(1000);
            }


            if (!AphbWebClient.Connected)
            {
                if ((AphbWebClient.GirisYapilamamaNedeni.Equals("İşyeri Kanun Kapsamından Çıkmıştır") || AphbWebClient.GirisYapilamamaNedeni.Equals("Is Yeri Iz Olmus")) && !OnaysizlarTamamlandi)
                {
                    LogEkle(String.Format("Sisteme giriş yapılamadı. Nedeni: {0}", AphbWebClient.GirisYapilamamaNedeni));

                    OnaysizlarTamamlandi = true;

                    AphbSayfayiYukle();

                    return;
                }
                else
                {
                    LogEkle(String.Format("Sisteme giriş yapılamadı. Nedeni: {0}", AphbWebClient.GirisYapilamamaNedeni));
                    LogEkle("Aphb indirme sona erdi");

                    if (AphbWebClient.GirisYapilamamaNedeni.Equals("Güvenlik kodu girilmedi"))
                    {
                        IndirmeSonucu.IptalEdildi = true;
                    }
                    else IndirmeSonucu.HataVar = true;

                    AphbSonaErdi(false);

                    return;
                }
            }

            AphbIndirmeBaslangicEkranaGit();
        }

        void AphbIndirmeBaslangicEkranaGit()
        {
            IptalKontrolu();

            if (!OnaysizlarTamamlandi)
            {
            OnaysizBildirgelereGit:

                var response = AphbWebClient.Get("https://ebildirge.sgk.gov.tr/WPEB/amp/bildirgeonayla", "");

                if (response.Equals("LogOut")) return;

                if (response.Contains("mesajiOkudumOnayliyorum"))
                {
                MesajiOnayla:
                    var resp = AphbWebClient.PostData("https://ebildirge.sgk.gov.tr/WPEB/amp/mesajiOkudumOnayliyorum", String.Empty);

                    if (resp.Contains("Aylık Prim ve Hizmet Belgesi Giriş Ana Menü")) goto OnaysizBildirgelereGit;
                    else
                    {
                        Thread.Sleep(500);
                        goto MesajiOnayla;
                    }
                }
                else if (response.Contains("Aylık Prim ve Hizmet Belgesi Kontrol/Onay İşlemi"))
                {
                    HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();

                    html.LoadHtml(response);

                    var forms = html.DocumentNode.Descendants("form").Where(p => !p.Id.Equals("logoutform"));

                    if (forms.Count() > 0)
                    {
                        bool YenidenYukle = false;

                        foreach (var form in forms)
                        {
                            IptalKontrolu();

                            var refno = form.Descendants("input").FirstOrDefault(p => p.GetAttributeValue("name", "").Equals("refno")).GetAttributeValue("value", "");

                            if (form.InnerText.Contains("Aylık Prim ve Hizmet Belgesi Kontrol Edilmemiştir"))
                            {
                            YenidenKontrolEt:

                                var responseKontrol = AphbWebClient.PostData("https://ebildirge.sgk.gov.tr/WPEB/amp/topluKontrolManager", String.Format("refno={0}&oncekiekran=onay&isDelete=", refno));

                                if (responseKontrol.Equals("LogOut")) return;

                                if (responseKontrol.Contains("Kontrol işlemi tamamlanmıştır"))
                                {
                                    YenidenYukle = true;
                                }
                                else goto YenidenKontrolEt;
                            }
                        }
                        if (YenidenYukle) goto OnaysizBildirgelereGit;


                        foreach (var form in forms)
                        {
                            IptalKontrolu();

                            var refno = form.Descendants("input").FirstOrDefault(p => p.GetAttributeValue("name", "").Equals("refno")).GetAttributeValue("value", "");

                            if (form.InnerText.Contains("Aylık Prim ve Hizmet Belgesi Onaylanabilir"))
                            {

                            BildirgeCek:

                                var responseBildirgeBilgileri = AphbWebClient.PostData("https://ebildirge.sgk.gov.tr/WPEB/amp/bordroliste?n=0", String.Format("refno={0}&oncekiekran=onay&isDelete=", refno));

                                if (responseBildirgeBilgileri.Equals("LogOut")) return;

                                if (responseBildirgeBilgileri.Contains("SİGORTALI HİZMET LİSTESİ"))
                                {

                                    Bildirge bildirge = BildirgeCek(responseBildirgeBilgileri, refno);

                                    if (!Bildirgeler.Any(p => p.Equals(bildirge, p)))
                                    {
                                        Bildirgeler.Add(bildirge);

                                        LogEkle(String.Format("Onaylanmamış bildirge kopyalandı. YIL:{0} AY:{1} KANUN:{2} BELGE TÜRÜ:{3} MAHİYET:{4} TOPLAM KİŞİ:{5}", bildirge.Yil, bildirge.Ay, bildirge.Kanun, bildirge.BelgeTuru, bildirge.Mahiyet, bildirge.Kisiler.Count));
                                    }
                                }
                                else goto BildirgeCek;
                            }
                            else if (form.InnerText.Contains("Aylık Prim ve Hizmet Belgesinde Hata Var") || form.InnerText.Contains("Aylık Prim ve Hizmet Belgesinde  Hata Var"))
                            {
                                Bildirge bildirge = new Bildirge();
                                bildirge.Askida = true;
                                bildirge.RefNo = refno;

                                int toplamCalisanSayisi = Convert.ToInt32(form.Descendants("td").FirstOrDefault(p => p.InnerText != null && p.InnerText.Equals("Toplam Çalışan Sayısı")).NextSibling.NextSibling.NextSibling.NextSibling.InnerText.Trim());

                            HataliBildirgeSayfasiniYukle:

                                var responseHatali = AphbWebClient.PostData("https://ebildirge.sgk.gov.tr/WPEB/amp/bildirgegiris", String.Format("refno={0}&oncekiekran=onay&isDelete=", refno));

                                if (responseHatali.Equals("LogOut")) return;

                                if (responseHatali.Contains("Aylık Prim ve Hizmet Belgesi Sigortalı Bilgileri Giriş"))
                                {
                                    HtmlAgilityPack.HtmlDocument htmlHatali = new HtmlAgilityPack.HtmlDocument();

                                    htmlHatali.LoadHtml(responseHatali);

                                    string IsyeriSicil = string.Empty;

                                    string Unvan = string.Empty;

                                    var tdler = htmlHatali.DocumentNode.Descendants("td");

                                    foreach (var item in tdler)
                                    {
                                        if (item.InnerText != null && item.InnerText.Trim() == "SicilNo")
                                        {
                                            IsyeriSicil = item.NextSibling.NextSibling.InnerText;

                                        }
                                        else if (item.InnerText != null && item.InnerText.Trim() == "Ünvanı")
                                        {
                                            Unvan = item.NextSibling.NextSibling.InnerText;

                                        }
                                        else if (item.InnerText != null && item.InnerText == "Yıl-ay")
                                        {
                                            bildirge.Yil = item.NextSibling.NextSibling.InnerText.Split('-')[0].Trim();

                                            int Ay = Sabitler.AyIsimleri.IndexOf(item.NextSibling.NextSibling.InnerText.Split('-')[1].Trim());

                                            if (Ay == -1) Ay = Convert.ToInt32(item.NextSibling.NextSibling.InnerText.Split('-')[1].Trim());

                                            bildirge.Ay = Convert.ToInt32(Ay).ToString();

                                        }
                                        if (item.InnerText != null && item.InnerText == "Belge türü")
                                        {
                                            bildirge.BelgeTuru = item.NextSibling.NextSibling.InnerText.Split('-')[0].Trim();
                                        }
                                        else if (item.InnerText != null && item.InnerText == "Kanun")
                                        {
                                            bildirge.Kanun = !string.IsNullOrEmpty(item.NextSibling.NextSibling.InnerText) ? item.NextSibling.NextSibling.InnerText : "00000";

                                            if (bildirge.Kanun.Contains("-"))
                                            {
                                                bildirge.Kanun = bildirge.Kanun.Split('-')[0];
                                            }
                                            else
                                            {
                                                if (bildirge.Kanun.Trim().Contains(" "))
                                                {
                                                    var temps = bildirge.Kanun.Trim().Split(' ');

                                                    if (bildirge.Kanun.ToLower().Contains("geçersiz"))
                                                    {
                                                        bildirge.Kanun = temps[2];
                                                    }
                                                    else bildirge.Kanun = temps[temps.Length - 1];
                                                }
                                            }
                                        }
                                    }

                                    var tdMahiyet = htmlHatali.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[3]/tr[1]/td[1]/form[1]/table[2]/tr[1]/td[2]");

                                    string Mahiyet = tdMahiyet.InnerText.Contains("ASIL") ? "ASIL" : tdMahiyet.InnerText.Contains("EK") ? "EK" : "IPTAL";

                                    bildirge.Mahiyet = Mahiyet;

                                    if (!IsyeriSicil.Contains("- 000"))
                                    {
                                        string Kod = IsyeriSicil.Split('-')[1].Trim().Split(' ')[0];

                                        bildirge.AraciveyaIsveren = Kod + "-" + Unvan;
                                    }


                                    int pageCurr = 1;

                                SiradakiSayfayaGec:

                                    if (toplamCalisanSayisi > 10)
                                    {
                                    HataliBildirgeSayfasiniYukle2:

                                        responseHatali = AphbWebClient.PostData("https://ebildirge.sgk.gov.tr/WPEB/amp/bildgir", String.Format("windowname=bildirgegiris.jsp&pageCurr={0}&maxRowNoInPage=200&pagecurr={1}&findNextError=", pageCurr, pageCurr > 1 ? pageCurr - 1 : 1));

                                        if (responseHatali.Equals("LogOut")) return;

                                        if (responseHatali.Contains("Aylık Prim ve Hizmet Belgesi Sigortalı Bilgileri Giriş "))
                                        {
                                            htmlHatali.LoadHtml(responseHatali);
                                        }
                                        else goto HataliBildirgeSayfasiniYukle2;
                                    }
                                    else htmlHatali.LoadHtml(responseHatali);


                                    var tables = htmlHatali.DocumentNode.Descendants("table");

                                    foreach (var table in tables)
                                    {
                                        if (table.InnerText != null && table.InnerText.Contains("Sıra") && table.InnerText.Contains("SG No") && !table.InnerText.Contains("SG No(TCK no)"))
                                        {
                                            var trs = table.Descendants("tr");

                                            foreach (var tr in trs)
                                            {
                                                var tds = tr.Descendants("td").ToList();

                                                long tcno = 0;

                                                if (!string.IsNullOrEmpty(tds[3].InnerText.Trim()))
                                                {

                                                    if (long.TryParse(tds[3].InnerText.Trim(), out tcno))
                                                    {
                                                        AphbSatir kisi = new AphbSatir();

                                                        kisi.SiraNo = tds[1].InnerText.Trim();

                                                        kisi.SosyalGuvenlikNo = tds[3].InnerText.Trim();

                                                        kisi.Adi = String.IsNullOrEmpty(tds[4].InnerText) ? "" : tds[4].InnerText.Trim();

                                                        kisi.Soyadi = String.IsNullOrEmpty(tds[5].InnerText) ? "" : tds[5].InnerText.Trim();

                                                        kisi.IlkSoyadi = String.IsNullOrEmpty(tds[6].InnerText) ? "" : tds[6].InnerText.Trim();

                                                        kisi.Gun = String.IsNullOrEmpty(tds[7].InnerText) ? "0" : tds[7].InnerText.Trim();

                                                        kisi.EksikGunSayisi = String.IsNullOrEmpty(tds[8].InnerText) ? "" : tds[8].InnerText.Trim();

                                                        kisi.Ucret = String.IsNullOrEmpty(tds[9].InnerText) ? "0" : tds[9].InnerText.Trim();

                                                        kisi.Ikramiye = String.IsNullOrEmpty(tds[10].InnerText) ? "0" : tds[10].InnerText.Trim();

                                                        kisi.GirisGunu = String.IsNullOrEmpty(tds[12].InnerText) ? "" : tds[12].InnerText.Trim();

                                                        kisi.CikisGunu = String.IsNullOrEmpty(tds[13].InnerText) ? "" : tds[13].InnerText.Trim();

                                                        string EGN = "";

                                                        if (!String.IsNullOrEmpty(tds[14].InnerText.Trim()))
                                                        {
                                                            if (tds[14].InnerText.Contains("-")) EGN = tds[14].InnerText.Split('-')[0];

                                                            else EGN = tds[14].InnerText;

                                                            if (Int32.TryParse(EGN, out int egnkod))
                                                            {
                                                                EGN = egnkod.ToString();
                                                            }
                                                        }

                                                        kisi.EksikGunNedeni = EGN;

                                                        string ICN = "";

                                                        if (!String.IsNullOrEmpty(tds[15].InnerText.Trim()))
                                                        {
                                                            if (tds[15].InnerText.Contains("-")) ICN = tds[15].InnerText.Split('-')[0];

                                                            else ICN = tds[15].InnerText;

                                                            if (Int32.TryParse(ICN, out int icnkod))
                                                            {
                                                                ICN = icnkod.ToString();
                                                            }
                                                        }

                                                        kisi.IstenCikisNedeni = ICN;

                                                        kisi.MeslekKod = String.IsNullOrEmpty(tds[16].InnerText) ? "" : tds[16].InnerText.Trim();

                                                        kisi.SiraNo = ((tds[17].InnerText != null && tds[17].InnerText == "E") ? "*" : "") + kisi.SiraNo;

                                                        bildirge.Kisiler.Add(kisi);
                                                    }
                                                }
                                                else break;
                                            }

                                            if (bildirge.Kisiler.Count < toplamCalisanSayisi)
                                            {
                                                pageCurr++;

                                                goto SiradakiSayfayaGec;
                                            }

                                            if (!Bildirgeler.Any(p => p.Equals(bildirge, p)))
                                            {
                                                Bildirgeler.Add(bildirge);
                                                LogEkle(String.Format("Onaylanmamış hatalı bildirge kopyalandı. YIL:{0} AY:{1} KANUN:{2} BELGE TÜRÜ:{3} MAHİYET:{4} TOPLAM KİŞİ:{5}", bildirge.Yil, bildirge.Ay, bildirge.Kanun, bildirge.BelgeTuru, bildirge.Mahiyet, bildirge.Kisiler.Count));
                                            }

                                            break;
                                        }
                                    }
                                }
                                else goto HataliBildirgeSayfasiniYukle;
                            }
                        }
                    }
                }
                else goto OnaysizBildirgelereGit;

                OnaysizlarTamamlandi = true;

                var onaysizBildirgeSayisi = Bildirgeler.Count(p => p.Kisiler.Count > 0);

                if (onaysizBildirgeSayisi > 0)
                {
                    LogEkle(String.Format("Toplam {0} adet onaylanmamış bildirge kopyalandı", onaysizBildirgeSayisi));
                }
                else
                {
                    LogEkle("Onaylanmamış bildirge bulunmadı.Onaylı bildirgelere geçiliyor");
                }

                AphbSayfayiYukle();
            }
            else if (!EBildirge2OnaysizlarTamamlandi)
            {
                IptalKontrolu();

            EBildirgeV2AnaSayfayaGit:

                var response = AphbWebClient.Get("https://ebildirge.sgk.gov.tr/EBildirgeV2/anasayfa.action", "");

                if (response.Equals("LogOut")) return;

                if (response.Contains("Onay Bekleyen Belgeler"))
                {
                    var html = new HtmlAgilityPack.HtmlDocument();

                    html.LoadHtml(response);

                    var link = "https://ebildirge.sgk.gov.tr" + html.DocumentNode.Descendants("a").FirstOrDefault(p => p.InnerText != null && p.InnerText.Equals("Onay Bekleyen Belgeler")).GetAttributeValue("href", "");

                V2OnaysizBildirgelereGit:

                    response = AphbWebClient.Get(link, "");

                    if (response.Equals("LogOut")) return;

                    if (response.Contains("Onay Bekleyen Bildirge Listesi"))
                    {
                        html.LoadHtml(response);

                        var uyari = html.GetElementbyId("genelUyariCenterTag");

                        if (uyari != null)
                        {
                            if (uyari.InnerText.Contains("Onaylanacak Bildirge Bulunamadı"))
                            {
                                LogEkle(string.Format("{0}", uyari.InnerText));
                            }
                            else goto V2OnaysizBildirgelereGit;
                        }
                        else
                        {
                            IptalKontrolu();

                            {
                                var araciUnvanTd = html.DocumentNode.SelectSingleNode("/html/body/div[3]/table/tr[2]/td/div/div[2]/div/table/tr[1]/td/table/tr[2]/td[2]/table/td[3]");

                                if (araciUnvanTd != null) AraciUnvani = araciUnvanTd.InnerText.Trim();

                                string isyeriSicilNo = html.DocumentNode.SelectSingleNode("//div[@id='contentContainer']/div/table/tr[1]/td/table/tr[2]/td[2]/table/tr[1]/td[@class='p10']/text()").InnerText;

                                var AraciNo = isyeriSicilNo.Split(' ').Reverse().Skip(1).Take(1).FirstOrDefault();

                                string AraciAdi = AraciNo == "000" ? "Ana İşveren" : String.Format("{0}-{1}", AraciNo, AraciUnvani);

                                var onaysizBildirgeListesi = html.DocumentNode.SelectNodes("//table[@class='gradienttable']");

                                var bildirgeinputs = html.DocumentNode.Descendants("input").Where(p => p.GetAttributeValue("name", "").Equals("bildirgeRefNo"));

                                if (bildirgeinputs.Count() > 0)
                                {

                                    DateTime sinir = new DateTime(2020, 1, 1);

                                    List<Bildirge> onaysizbildirgeler = new List<Bildirge>();

                                    foreach (var inputBildirgeRefNo in bildirgeinputs)
                                    {
                                        var parentTd = inputBildirgeRefNo.Ancestors("td").FirstOrDefault();

                                        var parentTr = parentTd.ParentNode.Name.Equals("table") ? parentTd.PreviousSibling.PreviousSibling.PreviousSibling : parentTd.ParentNode;

                                        var tdTahakkuk = parentTd.NextSibling.NextSibling;
                                        var tdHizmetYilAy = tdTahakkuk.NextSibling.NextSibling.NextSibling.NextSibling;

                                        if (!tdTahakkuk.GetInnerText().Equals("A")) continue;

                                        DateTime.TryParse(tdHizmetYilAy.GetInnerText(), out DateTime temp);
                                        if (temp < sinir) continue;


                                        var tdBelgeTuru = tdHizmetYilAy.NextSibling.NextSibling;
                                        var tdMahiyet = tdBelgeTuru.NextSibling.NextSibling;
                                        var tdKanun = tdMahiyet.NextSibling.NextSibling;

                                        onaysizbildirgeler.Add(new Bildirge
                                        {
                                            BelgeTuru = tdBelgeTuru.GetInnerText().Trim(),
                                            Mahiyet = tdMahiyet.GetInnerText().Trim(),
                                            Kanun = tdKanun.GetInnerText().Trim(),
                                            Yil = Convert.ToInt32(tdHizmetYilAy.GetInnerText().Split('/')[0]).ToString(),
                                            Ay = Convert.ToInt32(tdHizmetYilAy.GetInnerText().Split('/')[1]).ToString(),
                                            RefNo = inputBildirgeRefNo.GetAttributeValue("value", ""),
                                            Duzeltilecek = parentTr.GetAttributeValue("class", "").Equals("red") || parentTr.GetAttributeValue("class", "").Equals("mavi")
                                        });
                                    }

                                    int toplamBakilanSayi = 0;

                                    //int ix = 0;


                                    //while (ix < onaysizBildirgeRows.Count)
                                    //{
                                    //    var tds= onaysizBildirgeRows[ix].Descendants("td").ToList();

                                    //    if (tds.Count >= 4 && DateTime.TryParse(tds[3].GetInnerText(), out DateTime temp))
                                    //    {
                                    //        if (temp < sinir || tds[1].GetInnerText().Equals("A") == false)
                                    //        {
                                    //            onaysizBildirgeRows.RemoveAt(ix);
                                    //        }
                                    //        else ix++;
                                    //    }
                                    //    else
                                    //    {
                                    //        onaysizBildirgeRows.RemoveAt(ix);
                                    //    }

                                    //}

                                    //Parallel.For(1, onaysizBildirgeListesi.Count, new ParallelOptions { MaxDegreeOfParallelism = 10, CancellationToken = token }, rowIndex =>
                                    for (int index = 0; index < onaysizbildirgeler.Count; index++)
                                    {
                                        IptalKontrolu();

                                        var AphbWebClientOnaysizBildirge = new ProjeGiris(SuanYapilanIsyeriAphb, Enums.ProjeTurleri.EBildirgeV2);

                                        AphbWebClientOnaysizBildirge.Cookie = AphbWebClient.Cookie;
                                        AphbWebClientOnaysizBildirge.Connected = true;

                                        var onaysizBildirge = onaysizbildirgeler[index];

                                        string hizmetYilAy = onaysizBildirge.Yil + "/" + onaysizBildirge.Ay.ToString().PadLeft(2, '0');
                                        int yil = Convert.ToInt32(onaysizBildirge.Yil);
                                        int ay = Convert.ToInt32(onaysizBildirge.Ay);

                                        string belgeTuru = onaysizBildirge.BelgeTuru;

                                        string belgeMahiyeti = onaysizBildirge.Mahiyet;

                                        belgeMahiyeti = belgeMahiyeti.Equals("İPTAL") ? "IPTAL" : belgeMahiyeti;

                                        string kanunNo = onaysizBildirge.Kanun.Replace("&nbsp;", "");

                                        bool DuzeltilecekBildirge = onaysizBildirge.Duzeltilecek;

                                        if (string.IsNullOrEmpty(kanunNo)) kanunNo = "00000";

                                        if (kanunNo.Contains("-"))
                                        {
                                            kanunNo = kanunNo.Split('-')[0];
                                        }
                                        else
                                        {
                                            if (kanunNo.Trim().Contains(" "))
                                            {
                                                var temps = kanunNo.Trim().Split(' ');

                                                if (kanunNo.ToLower().Contains("geçersiz"))
                                                {
                                                    kanunNo = temps[2];
                                                }
                                                else kanunNo = temps[temps.Length - 1];
                                            }
                                        }

                                        {

                                            var indirilecekAphb = new IndirilecekAphb
                                            {
                                                Araci = AraciAdi,
                                                isyeriSicilNo = isyeriSicilNo,
                                                onaysizBildirge = onaysizBildirge,
                                                HizmetYilAy = hizmetYilAy
                                            };

                                            string bildirgeRefNo = onaysizBildirge.RefNo;

                                            if (!DuzeltilecekBildirge)
                                            {
                                                bool devam = true;

                                                int hataSay = 1;
                                            hatadurumdaDon:
                                                try
                                                {
                                                    int denemeSayisi = 0;

                                                tekrarIndirmeyiDene:

                                                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                                                    string sPostData = String.Format("bildirgeRefNo={0}&download=true&action%3AtahakkukfisHizmetPdf=Hizmet+Listesi%28PDF%29", bildirgeRefNo.Replace(" ", "+"));
                                                    var pdfData = AphbWebClientOnaysizBildirge.DownloadFilePost("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tilesislemTamam.action", sPostData);

                                                    string dosyaicerigi = System.Text.Encoding.UTF8.GetString(pdfData);

                                                    if (!dosyaicerigi.StartsWith("%PDF"))
                                                    {

                                                        if (denemeSayisi < 3)
                                                        {
                                                            denemeSayisi++;

                                                            Thread.Sleep(5000);

                                                            goto tekrarIndirmeyiDene;
                                                        }
                                                        else
                                                        {
                                                            lock (hataVerenBildirgeler)
                                                            {
                                                                if (!hataVerenBildirgeler.ContainsKey(bildirgeRefNo))
                                                                {
                                                                    hataVerenBildirgeler.Add(bildirgeRefNo, indirilecekAphb);
                                                                }
                                                            }

                                                            lock (sb)
                                                            {
                                                                LogEkle(string.Format("{0} --> {1} - {2}/{3}-{4} ", AraciAdi, hizmetYilAy, ++toplamBakilanSayi, onaysizbildirgeler.Count, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti + " bildirgesi 3 denemeye rağmen indirilemedi."));
                                                            }
                                                        }

                                                        devam = false;

                                                    }


                                                    if (devam)
                                                    {
                                                        var pdfTextBuilder = Metodlar.GetPdfText(Metodlar.PdfReaderDondur(pdfData));

                                                        string pdfText = pdfTextBuilder.ToString();

                                                        Bildirge bildirge = BildirgeCekPdften(pdfText, Metodlar.PdfReaderDondur(pdfData), bildirgeRefNo);
                                                        bildirge.Askida = true;
                                                        bildirge.RefNo = bildirgeRefNo;

                                                        lock (Bildirgeler)
                                                        {

                                                            if (!Bildirgeler.Any(p => p.Equals(bildirge, p)))
                                                            {
                                                                Bildirgeler.Add(bildirge);
                                                            }
                                                        }

                                                        lock (sb)
                                                        {
                                                            LogEkle(string.Format("{0} --> {1}/{2}", AraciAdi, ++toplamBakilanSayi, onaysizbildirgeler.Count));
                                                        }

                                                        if (pdfBildirgeHataliOkunduMu)
                                                        {
                                                            lock (hataVerenBildirgeler)
                                                            {
                                                                if (!hataVerenBildirgeler.ContainsKey(bildirgeRefNo))
                                                                {
                                                                    hataVerenBildirgeler.Add(bildirgeRefNo, indirilecekAphb);
                                                                }
                                                            }

                                                            lock (sb)
                                                            {
                                                                LogEkle(string.Format("{0} bildirgede okuma sorunu!  Exceli kontrol ediniz!", hizmetYilAy));
                                                                LogEkle(string.Format(bilgiDondurmekIcin));
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    lock (sb)
                                                    {
                                                        LogEkle(string.Format("{0} ---> {1} bildirgeleri indirilirken hata meydana geldi. Tekrar Deneme: {2}. Hata Mesajı:{3}", AraciAdi, hizmetYilAy, hataSay, ex.Message));
                                                    }

                                                    if (hataSay < 3)
                                                    {
                                                        hataSay++;

                                                        Thread.Sleep(5000);

                                                        goto hatadurumdaDon;
                                                    }
                                                    else
                                                    {
                                                        lock (hataVerenBildirgeler)
                                                        {
                                                            if (!hataVerenBildirgeler.ContainsKey(bildirgeRefNo))
                                                            {
                                                                hataVerenBildirgeler.Add(bildirgeRefNo, indirilecekAphb);
                                                            }
                                                        }
                                                    }

                                                }

                                            }
                                            else
                                            {
                                                var sayac = 0;
                                            DuzeltilecekBildirgeyiAc:

                                                var bildirge = new Bildirge
                                                {
                                                    AraciveyaIsveren = AraciAdi,
                                                    Askida = true,
                                                    Ay = ay.ToString(),
                                                    BelgeTuru = belgeTuru.ToInt().ToString(),
                                                    Kanun = kanunNo,
                                                    Mahiyet = belgeMahiyeti,
                                                    Yil = yil.ToString(),
                                                    RefNo = bildirgeRefNo,
                                                    Kisiler = new List<AphbSatir>(),
                                                };



                                                response = AphbWebClientOnaysizBildirge.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tilesislemTamam.action", String.Format("bildirgeRefNo={0}&action%3Atahakkukduzeltme=D%C3%BCzeltme+Yap&download=true", bildirgeRefNo.Replace(" ", "+")));

                                                if (response.Contains("Sigortalı Bilgileri Giriş"))
                                                {
                                                    sayac = 0;

                                                    var sayfaNo = 1;

                                                SayfayaGit:

                                                    response = AphbWebClientOnaysizBildirge.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/kisiGironyukeleme.action", String.Format("yeniSayfaNo={0}&sayfaSatirSayisi=100", sayfaNo));

                                                    if (response.Contains("Sigortalı Bilgileri Giriş"))
                                                    {
                                                        sayac = 0;

                                                        var html2 = new HtmlAgilityPack.HtmlDocument();

                                                        html2.LoadHtml(response);

                                                        var toplamKayit = html2.DocumentNode.Descendants("span").FirstOrDefault(p => p.GetInnerText().StartsWith("Toplam Kayıt Sayısı")).GetInnerText().Split(':')[1].Trim().Replace(",","").ToInt();

                                                        var table = html2.GetElementbyId("kisiGironyukeleme").Descendants("table").FirstOrDefault();

                                                        var trs = table.Descendants("tr").ToList();

                                                        for (int i = 1; i < trs.Count; i++)
                                                        {
                                                            var tds = trs[i].Descendants("td").ToList();

                                                            var tc = tds[4].GetInnerText().Trim();

                                                            if (!long.TryParse(tc, out long tmp)) break;

                                                            bildirge.Kisiler.Add(new AphbSatir
                                                            {
                                                                SiraNo = (i + (sayfaNo - 1) * 100).ToString(),
                                                                SosyalGuvenlikNo = tds[4].GetInnerText().Trim(),
                                                                Adi = tds[5].GetInnerText().Trim(),
                                                                Soyadi = tds[6].GetInnerText().Trim(),
                                                                IlkSoyadi = tds[7].GetInnerText().Trim(),
                                                                Gun = tds[8].GetInnerText().Trim(),
                                                                EksikGunSayisi = tds[9].GetInnerText().Trim(),
                                                                Ucret = tds[10].GetInnerText().Trim().Replace(".", ""),
                                                                Ikramiye = tds[11].GetInnerText().Trim().Replace(".", ""),
                                                                GirisGunu = tds[13].GetInnerText().Trim(),
                                                                CikisGunu = tds[14].GetInnerText().Trim(),
                                                                EksikGunNedeni = tds[15].GetInnerText().Trim(),
                                                                IstenCikisNedeni = tds[16].GetInnerText().Trim(),
                                                                MeslekKod = tds[17].GetInnerText().Trim(),
                                                            });

                                                        }

                                                        if (bildirge.Kisiler.Count != toplamKayit)
                                                        {
                                                            sayfaNo++;
                                                            goto SayfayaGit;
                                                        }


                                                        lock (Bildirgeler)
                                                        {
                                                            if (!Bildirgeler.Any(p => p.Equals(bildirge, p)))
                                                            {
                                                                Bildirgeler.Add(bildirge);
                                                            }
                                                        }

                                                        lock (sb)
                                                        {
                                                            LogEkle(string.Format("{0} --> {1}/{2}", AraciAdi, ++toplamBakilanSayi, onaysizbildirgeler.Count));
                                                        }

                                                    }
                                                    else
                                                    {
                                                        sayac++;

                                                        if (sayac < 3)
                                                        {
                                                            Thread.Sleep(500);
                                                            goto SayfayaGit;
                                                        }
                                                        else
                                                        {
                                                            lock (sb)
                                                            {
                                                                LogEkle(string.Format("{0} --> {1} - {2}/{3}-{4} ", AraciAdi, hizmetYilAy, ++toplamBakilanSayi, onaysizbildirgeler.Count, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti + " bildirgesi 3 denemeye rağmen indirilemedi."));
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    sayac++;

                                                    if (sayac < 3)
                                                    {
                                                        Thread.Sleep(500);
                                                        goto DuzeltilecekBildirgeyiAc;
                                                    }
                                                    else
                                                    {
                                                        lock (sb)
                                                        {
                                                            LogEkle(string.Format("{0} --> {1} - {2}/{3}-{4} ", AraciAdi, hizmetYilAy, ++toplamBakilanSayi, onaysizbildirgeler.Count, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti + " bildirgesi 3 denemeye rağmen indirilemedi."));
                                                        }
                                                    }
                                                }
                                            }

                                        }


                                    }
                                    //);

                                }

                                if (Bildirgeler.Count == 0)
                                {
                                    LogEkle("Onaysız bildirge bulunamadı");
                                }

                                LogEkle(string.Format("{0}", "-----------------------------------------------------------"));
                            }
                        }

                        LogEkle("E-Bildirge V2 onaylı bildirgelere bakılıyor");

                        EBildirge2OnaysizlarTamamlandi = true;

                        AphbIndirmeBaslangicEkranaGit();
                    }
                    else goto V2OnaysizBildirgelereGit;
                }
                else goto EBildirgeV2AnaSayfayaGit;
            }
            else
            {
            EBildirgeV2AnaSayfayaGit:

                IptalKontrolu();

                var response = AphbWebClient.Get("https://ebildirge.sgk.gov.tr/EBildirgeV2/anasayfa.action", "");

                if (response.Equals("LogOut")) return;

                if (response.Contains("Onaylanmış Belgeler"))
                {
                    var html = new HtmlAgilityPack.HtmlDocument();

                    html.LoadHtml(response);

                    var link = "https://ebildirge.sgk.gov.tr" + html.DocumentNode.Descendants("a").FirstOrDefault(p => p.InnerText != null && p.InnerText.Equals("Onaylanmış Belgeler")).GetAttributeValue("href", "");

                OnaylanmisBildirgelereGit:

                    response = AphbWebClient.Get(link, "");

                    if (response.Equals("LogOut")) return;

                    if (response.Contains("İşlem Yapılacak Bildirge Dönemi Giriş"))
                    {
                        html.LoadHtml(response);

                        var araciUnvanTd = html.DocumentNode.SelectSingleNode("/html/body/div[3]/table/tr[2]/td/div/div[2]/div/table/tr[1]/td/table/tr[2]/td[2]/table/td[3]");

                        if (araciUnvanTd != null) AraciUnvani = araciUnvanTd.InnerText.Trim();

                        Dictionary<string, string> anaIsverenveTaseronlar = new Dictionary<string, string>();

                        List<HtmlAgilityPack.HtmlNode> optionsAraci = null;
                        var araciOpts = html.GetElementbyId("tahakkukonaylanmisTahakkukDonemSecildi_isyeri_internetGosterimAraciNo");
                        if (araciOpts != null)
                        {
                            optionsAraci = araciOpts.Descendants("option").ToList();

                            if (string.IsNullOrEmpty(AraciUnvani))
                            {
                                anaIsverenveTaseronlar = optionsAraci.ToDictionary(x => x.GetAttributeValue("value", ""), x => x.InnerText.Trim().StartsWith("000-") ? "Ana İşveren" : x.InnerText.Trim());
                            }
                            else
                            {
                                anaIsverenveTaseronlar = optionsAraci.Where(p => p.InnerText.Trim().EndsWith(AraciUnvani)).ToDictionary(x => x.GetAttributeValue("value", ""), x => x.InnerText.Trim().StartsWith("000-") ? "Ana İşveren" : x.InnerText.Trim());
                            }

                        }
                        else
                        {
                            if (string.IsNullOrEmpty(AraciUnvani))
                            {
                                anaIsverenveTaseronlar.Add("0", "Ana İşveren");
                            }
                            else
                            {
                                var taseronNo = SuanYapilanIsyeriAphb.TaseronNo;

                                if (int.TryParse(taseronNo, out int aracino))
                                {
                                    if (aracino > 0)
                                    {
                                        anaIsverenveTaseronlar.Add("0", aracino.ToString().PadLeft(3, '0') + "-" + AraciUnvani);
                                    }
                                    else
                                    {
                                        anaIsverenveTaseronlar.Add("0", "Ana İşveren");
                                    }
                                }
                                else MessageBox.Show("Taşeron işyerinin taşeron numarası tespit edilemedi. Lütfen taşeron işyeri bilgilerini kontrol ediniz");
                            }
                        }

                        var baslangic = html.GetElementbyId("tahakkukonaylanmisTahakkukDonemSecildi_hizmet_yil_ay_index");
                        var bitis = html.GetElementbyId("tahakkukonaylanmisTahakkukDonemSecildi_hizmet_yil_ay_index_bitis");

                        bool baslangicSecildi = false;
                        bool bitisSecildi = false;

                        if (baslangic != null)
                        {
                            var options = baslangic.Descendants("option");

                            foreach (var option in options)
                            {
                                if (option.InnerText != "--Lütfen Seçiniz--")
                                {
                                    int yil = Convert.ToInt32(option.InnerText.Split('/')[0]);
                                    int ay = Convert.ToInt32(option.InnerText.Split('/')[1]);

                                    DateTime tarih = new DateTime(yil, ay, 1);

                                    if (tarih <= TarihBitisAphb)
                                    {
                                        baslangicSecildi = true;

                                        hizmetyilAyIndex = option.GetAttributeValue("value", "");

                                        break;
                                    }
                                }
                            }
                        }

                        if (bitis != null)
                        {
                            var optionsbitis = bitis.Descendants("option").ToList();

                            for (int i = optionsbitis.Count - 1; i >= 0; i--)
                            {
                                var option = optionsbitis[i];

                                if (option.InnerText != "--Lütfen Seçiniz--")
                                {
                                    int yil = Convert.ToInt32(option.InnerText.Split('/')[0]);
                                    int ay = Convert.ToInt32(option.InnerText.Split('/')[1]);

                                    DateTime tarih = new DateTime(yil, ay, 1);

                                    if (tarih >= TarihBaslangicAphb)
                                    {
                                        bitisSecildi = true;

                                        hizmetyilAyIndexBitis = option.GetAttributeValue("value", "");
                                        break;
                                    }
                                }
                            }

                        }

                        foreach (var item in anaIsverenveTaseronlar)
                        {
                            IptalKontrolu();

                            var araciIndex = item.Key;
                            string araci = item.Value;

                            if (kullanilanAraciIsyeri.Contains(araciIndex)) continue;

                            if (baslangicSecildi && bitisSecildi)
                            {
                            OnayliBildirgeleriCek:

                                var responseOnayliBildirgeler = AphbWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukonaylanmisTahakkukDonemSecildi.action", String.Format("{0}hizmet_yil_ay_index={1}&hizmet_yil_ay_index_bitis={2}", araciOpts == null ? "" : "isyeri.internetGosterimAraciNo=" + araciIndex + "&", hizmetyilAyIndex, hizmetyilAyIndexBitis));

                                if (responseOnayliBildirgeler.Equals("LogOut")) return;

                                if (responseOnayliBildirgeler.Contains("Onaylı Bildirge Listesi") || (responseOnayliBildirgeler.Contains("Onaylı Bildirge Bulunamadı")))
                                {
                                    var htmlOnayliBildirgeler = new HtmlAgilityPack.HtmlDocument();

                                    htmlOnayliBildirgeler.LoadHtml(responseOnayliBildirgeler);

                                    var uyari = htmlOnayliBildirgeler.GetElementbyId("genelUyariCenterTag");

                                    if (uyari != null)
                                    {
                                        if (uyari.InnerText.Contains("Onaylı Bildirge Bulunamadı") || uyari.InnerText.Contains("Invalid character"))
                                        {
                                            LogEkle(string.Format("{0} -> {1}", araci, uyari.InnerText));

                                            if (!kullanilanAraciIsyeri.Contains(araciIndex))
                                            {
                                                kullanilanAraciIsyeri.Add(araciIndex);
                                            }
                                        }
                                        else goto OnayliBildirgeleriCek;
                                    }
                                    else
                                    {

                                        var onayliBildirgeListesi = htmlOnayliBildirgeler.DocumentNode.SelectNodes("//table[@class='gradienttable']");

                                        string isyeriSicilNo = htmlOnayliBildirgeler.DocumentNode.SelectSingleNode("//div[@id='contentContainer']/div/table/tr[1]/td/table/tr[2]/td[2]/table/tr[1]/td[@class='p10']/text()").InnerText;

                                        int BildirgeSayisi = 0;

                                        int yeniSecilenSayisi = 0;

                                        string Donem = string.Empty;

                                        if (onayliBildirgeListesi != null)
                                        {
                                            int sayOnayliSatir = 0;
                                            var onayliBildirgeRows = onayliBildirgeListesi[0].SelectNodes("tr");

                                            int toplamBakilanSayi = 0;

                                            Parallel.For(2, onayliBildirgeRows.Count, new ParallelOptions { MaxDegreeOfParallelism = 10, CancellationToken = token }, rowIndex =>
                                            {
                                                IptalKontrolu();

                                                var AphbWebClientOnayliBildirge = new ProjeGiris(SuanYapilanIsyeriAphb, Enums.ProjeTurleri.EBildirgeV2);

                                                AphbWebClientOnayliBildirge.Cookie = AphbWebClient.Cookie;
                                                AphbWebClientOnayliBildirge.Connected = true;

                                                var onayliBildirgeRow = onayliBildirgeRows[rowIndex];

                                                string hizmetYilAy = onayliBildirgeRow.SelectSingleNode("td[2]/p/text()").InnerText;
                                                int yil = Convert.ToInt32(hizmetYilAy.Split('/')[0]);
                                                int ay = Convert.ToInt32(hizmetYilAy.Split('/')[1]);

                                                string belgeTuru = onayliBildirgeRow.SelectSingleNode("td[3]/p/text()").InnerText;

                                                string belgeMahiyeti = onayliBildirgeRow.SelectSingleNode("td[4]/p/text()").InnerText;

                                                belgeMahiyeti = belgeMahiyeti.Equals("İPTAL") ? "IPTAL" : belgeMahiyeti;

                                                string kanunNo = onayliBildirgeRow.SelectSingleNode("td[5]/p/text()").InnerText.Replace("&nbsp;", "");

                                                if (string.IsNullOrEmpty(kanunNo)) kanunNo = "00000";

                                                string bildirgeRefNo = Regex.Matches(onayliBildirgeRow.InnerHtml, "'H','(.*)'")[0].Groups[1].Value;

                                                bool eskiKayitVar = false;

                                                //eskiKayitVar = OncedenIndirilenler.Contains(yil.ToString() + "-" + ay.ToString().PadLeft(2, '0') + "-" + kanunNo.Trim().PadLeft(5, '0') + "-" + belgeMahiyeti.Trim() + "-" + belgeTuru.Trim().PadLeft(2, '0') + "-" + (araci.Trim().Contains("-") ? araci.Trim().Split('-')[0] : araci.Trim()) + "-");
                                                eskiKayitVar = OncedenIndirilenler.Contains(bildirgeRefNo.Trim() + "-" + (araci.Trim().Contains("-") ? araci.Trim().Split('-')[0] : araci.Trim()) + "-");


                                                var indirilecekAphb = new IndirilecekAphb
                                                {
                                                    Araci = araci,
                                                    HizmetYilAy = hizmetYilAy,
                                                    isyeriSicilNo = isyeriSicilNo,
                                                    onayliBildirgeRow = onayliBildirgeRow,
                                                    sayOnayliSatir = sayOnayliSatir
                                                };

                                                if (!eskiKayitVar)
                                                {
                                                    Donem = hizmetYilAy;

                                                    bool dahaOnceSecilmisMi = secilenRadios.Contains(hizmetYilAy.Replace("/", "") + "_" + bildirgeRefNo + "_" + sayOnayliSatir);

                                                    if (!dahaOnceSecilmisMi)
                                                    {

                                                        yeniSecilenSayisi++;

                                                        lock (secilenRadios)
                                                        {
                                                            secilenRadios.Add(hizmetYilAy.Replace("/", "") + "_" + bildirgeRefNo + "_" + sayOnayliSatir);
                                                        }

                                                        int hataSay = 1;
                                                    hatadurumdaDon:
                                                        try
                                                        {
                                                            string pathFolder = Application.StartupPath + "\\AphbPDF\\" + isyeriSicilNo.Replace(" ", "").Trim() + "\\" + (araci.Trim().Length > 20 ? araci.Trim().Substring(0, 20).Trim() : araci.Trim()) + "\\" + hizmetYilAy.Replace("/", "_");
                                                            string file = pathFolder + "\\Hizmet_" + hizmetYilAy.Replace("/", "_") + "_" + belgeTuru + "_" + belgeMahiyeti + "_" + kanunNo + "_" + bildirgeRefNo + "_" + sayOnayliSatir + ".pdf";
                                                            string file2 = pathFolder + "\\Hizmet_" + hizmetYilAy.Replace("/", "_") + "_" + belgeTuru + "_" + belgeMahiyeti + "_" + kanunNo + "_" + bildirgeRefNo + "_" + sayOnayliSatir + ".txt";

                                                            if (!Directory.Exists(pathFolder))
                                                            {
                                                                Directory.CreateDirectory(pathFolder);
                                                            }

                                                            bool devam = true;

                                                            bool indir = false;


                                                            StringBuilder pdfTextBuilder = null;

                                                            if (File.Exists(file))
                                                            {
                                                                try
                                                                {
                                                                    pdfTextBuilder = Metodlar.GetPdfText(Metodlar.PdfReaderDondur(file));

                                                                    if (new FileInfo(file).CreationTime <= Sabitler.UCGEklemeTarihi)
                                                                    {
                                                                        throw new Exception("Dosya eski, yenisinin indirilmesi lazım");
                                                                    }
                                                                }
                                                                catch
                                                                {

                                                                    indir = true;

                                                                    pdfTextBuilder = null;

                                                                    File.Delete(file);
                                                                }
                                                            }
                                                            else indir = true;

                                                            if (indir)
                                                            {
                                                                int denemeSayisi = 0;

                                                            tekrarIndirmeyiDene:

                                                                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                                                                string sPostData = "tip=tahakkukonayliFisHizmetPdf&download=true&hizmet_yil_ay_index=" + hizmetyilAyIndex + "&hizmet_yil_ay_index_bitis=" + hizmetyilAyIndexBitis + "&bildirgeRefNo=" + bildirgeRefNo;
                                                                var pdfData = AphbWebClientOnayliBildirge.DownloadFilePost("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/pdfGosterim.action ", sPostData);

                                                                string dosyaicerigi = System.Text.Encoding.UTF8.GetString(pdfData);

                                                                if (!dosyaicerigi.Contains("İlgili Tahakkuk Bilgileri Bulunamadı"))
                                                                {
                                                                    if (dosyaicerigi.StartsWith("%PDF"))
                                                                    {
                                                                        System.IO.File.WriteAllBytes(file, pdfData);

                                                                        BildirgeSayisi++;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (denemeSayisi < 3)
                                                                        {
                                                                            denemeSayisi++;

                                                                            Thread.Sleep(5000);

                                                                            goto tekrarIndirmeyiDene;
                                                                        }
                                                                        else
                                                                        {
                                                                            lock (hataVerenBildirgeler)
                                                                            {
                                                                                if (!hataVerenBildirgeler.ContainsKey(bildirgeRefNo))
                                                                                {
                                                                                    hataVerenBildirgeler.Add(bildirgeRefNo, indirilecekAphb);
                                                                                }
                                                                            }

                                                                            lock (sb)
                                                                            {
                                                                                LogEkle(string.Format("{0} -> {1} - {2}/{3}-{4} ", araci, hizmetYilAy, ++toplamBakilanSayi, onayliBildirgeRows.Count - 2, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti + " bildirgesi 3 denemeye rağmen indirilemedi."));
                                                                            }

                                                                            devam = false;
                                                                        }

                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    devam = false;
                                                                    toplamBakilanSayi++;
                                                                }

                                                            }
                                                            else BildirgeSayisi++;

                                                            if (devam)
                                                            {
                                                                if (pdfTextBuilder == null) pdfTextBuilder = Metodlar.GetPdfText(Metodlar.PdfReaderDondur(file));

                                                                string pdfText = pdfTextBuilder.ToString();

                                                                Bildirge bildirge = BildirgeCekPdften(pdfText, Metodlar.PdfReaderDondur(file), bildirgeRefNo);

                                                                lock (Bildirgeler)
                                                                {

                                                                    if (!Bildirgeler.Any(p => p.Equals(bildirge, p)))
                                                                    {
                                                                        Bildirgeler.Add(bildirge);
                                                                    }
                                                                }

                                                                lock (sb)
                                                                {
                                                                    LogEkle(string.Format("{0} -> {1}/{2}", araci, ++toplamBakilanSayi, onayliBildirgeRows.Count - 2));
                                                                }

                                                                if (pdfBildirgeHataliOkunduMu)
                                                                {
                                                                    lock (hataVerenBildirgeler)
                                                                    {
                                                                        if (!hataVerenBildirgeler.ContainsKey(bildirgeRefNo))
                                                                        {
                                                                            hataVerenBildirgeler.Add(bildirgeRefNo, indirilecekAphb);
                                                                        }
                                                                    }

                                                                    lock (sb)
                                                                    {
                                                                        LogEkle(string.Format("{0} bildirgede okuma sorunu!  Exceli kontrol ediniz!", hizmetYilAy));
                                                                        LogEkle(string.Format(bilgiDondurmekIcin));
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            lock (sb)
                                                            {
                                                                LogEkle(string.Format("{0} bildirgeleri indirilirken hata meydana geldi. Tekrar Deneme: {1}. Hata Mesajı:{2}", hizmetYilAy, hataSay, ex.Message));
                                                            }

                                                            if (hataSay < 3)
                                                            {
                                                                hataSay++;

                                                                Thread.Sleep(5000);

                                                                goto hatadurumdaDon;
                                                            }
                                                            else
                                                            {
                                                                lock (hataVerenBildirgeler)
                                                                {
                                                                    if (!hataVerenBildirgeler.ContainsKey(bildirgeRefNo))
                                                                    {
                                                                        hataVerenBildirgeler.Add(bildirgeRefNo, indirilecekAphb);
                                                                    }
                                                                }
                                                            }

                                                        }

                                                    }
                                                    else toplamBakilanSayi++;
                                                }
                                                else
                                                {
                                                    lock (sb)
                                                    {
                                                        LogEkle(string.Format("{0} -> {1} - {2}/{3} - Daha önceden indirilmiş", araci, hizmetYilAy, ++toplamBakilanSayi, onayliBildirgeRows.Count - 2));
                                                    }
                                                }

                                            });

                                        }

                                        if (yeniSecilenSayisi == 0)
                                        {
                                            LogEkle(string.Format("{0} {1}", araci, "bildirgeleri daha önceden indirilmiş"));
                                        }
                                        else if (BildirgeSayisi == 0)
                                        {
                                            LogEkle(string.Format("{0} -> {1}", araci, Donem + " - Onaylı Bildirge Bulunamadı"));
                                        }

                                        LogEkle(string.Format("{0}", "-----------------------------"));

                                        if (!kullanilanAraciIsyeri.Contains(araciIndex))
                                        {
                                            kullanilanAraciIsyeri.Add(araciIndex);
                                        }
                                    }
                                }
                                else goto OnayliBildirgeleriCek;
                            }
                            else
                            {
                                LogEkle(string.Format("{0}", "Seçilen tarih aralığı bulunamadığı için Aphb indirme işlemi sona erdi"));

                                break;
                            }
                        }

                        if (hataVerenBildirgeler.Count > 0)
                        {
                            var hataDenemeSayisi = 0;

                            while (hataVerenBildirgeler.Count > 0 &&  hataDenemeSayisi < 2)
                            {
                                LogEkle(string.Format("Hata veren {0} bildirge tespit edildi. Bu bildirgeler tekrar denenecek. Tekrar Deneme : {1} ",hataVerenBildirgeler.Count, hataDenemeSayisi+1 ));

                                var index = 0;
                                while (index < hataVerenBildirgeler.Count)
                                {
                                    
                                    var item = hataVerenBildirgeler.ElementAt(index);

                                    var onaysiz = item.Value.onaysizBildirge != null;
                                    var araci = item.Value.Araci;
                                    if (!onaysiz) //Onaylı bildirge ise
                                    {
                                        var sayOnayliSatir = item.Value.sayOnayliSatir;

                                        var onayliBildirgeRow = item.Value.onayliBildirgeRow as HtmlAgilityPack.HtmlNode;

                                        string hizmetYilAy = onayliBildirgeRow.SelectSingleNode("td[2]/p/text()").InnerText;
                                        int yil = Convert.ToInt32(hizmetYilAy.Split('/')[0]);
                                        int ay = Convert.ToInt32(hizmetYilAy.Split('/')[1]);

                                        string belgeTuru = onayliBildirgeRow.SelectSingleNode("td[3]/p/text()").InnerText;

                                        string belgeMahiyeti = onayliBildirgeRow.SelectSingleNode("td[4]/p/text()").InnerText;

                                        belgeMahiyeti = belgeMahiyeti.Equals("İPTAL") ? "IPTAL" : belgeMahiyeti;

                                        string kanunNo = onayliBildirgeRow.SelectSingleNode("td[5]/p/text()").InnerText.Replace("&nbsp;", "");

                                        if (string.IsNullOrEmpty(kanunNo)) kanunNo = "00000";

                                        string bildirgeRefNo = Regex.Matches(onayliBildirgeRow.InnerHtml, "'H','(.*)'")[0].Groups[1].Value;

                                        int hataSay = 1;
                                    hatadurumdaDon:
                                        try
                                        {
                                            string pathFolder = Application.StartupPath + "\\AphbPDF\\" + SuanYapilanIsyeriAphb.IsyeriSicilNo.Replace(" ", "").Trim() + "\\" + (araci.Trim().Length > 20 ? araci.Trim().Substring(0, 20).Trim() : araci.Trim()) + "\\" + hizmetYilAy.Replace("/", "_");
                                            string file = pathFolder + "\\Hizmet_" + hizmetYilAy.Replace("/", "_") + "_" + belgeTuru + "_" + belgeMahiyeti + "_" + kanunNo + "_" + bildirgeRefNo + "_" + sayOnayliSatir + ".pdf";
                                            string file2 = pathFolder + "\\Hizmet_" + hizmetYilAy.Replace("/", "_") + "_" + belgeTuru + "_" + belgeMahiyeti + "_" + kanunNo + "_" + bildirgeRefNo + "_" + sayOnayliSatir + ".txt";

                                            if (!Directory.Exists(pathFolder))
                                            {
                                                Directory.CreateDirectory(pathFolder);
                                            }

                                            bool devam = true;

                                            StringBuilder pdfTextBuilder = null;

                                            int denemeSayisi = 0;

                                        tekrarIndirmeyiDene:

                                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                                            string sPostData = "tip=tahakkukonayliFisHizmetPdf&download=true&hizmet_yil_ay_index=" + hizmetyilAyIndex + "&hizmet_yil_ay_index_bitis=" + hizmetyilAyIndexBitis + "&bildirgeRefNo=" + bildirgeRefNo;
                                            var pdfData = AphbWebClient.DownloadFilePost("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/pdfGosterim.action ", sPostData);

                                            string dosyaicerigi = System.Text.Encoding.UTF8.GetString(pdfData);

                                            if (!dosyaicerigi.Contains("İlgili Tahakkuk Bilgileri Bulunamadı"))
                                            {
                                                if (dosyaicerigi.StartsWith("%PDF"))
                                                {
                                                    if (File.Exists(file)) File.Delete(file);

                                                    System.IO.File.WriteAllBytes(file, pdfData);
                                                }
                                                else
                                                {
                                                    if (denemeSayisi < 3)
                                                    {
                                                        denemeSayisi++;

                                                        Thread.Sleep(5000);

                                                        goto tekrarIndirmeyiDene;
                                                    }
                                                    else
                                                    {

                                                        lock (sb)
                                                        {
                                                            LogEkle(string.Format("{0} -> {1} - {2} ", araci, hizmetYilAy, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti + " bildirgesi 3 denemeye rağmen indirilemedi."));
                                                        }

                                                        devam = false;
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                devam = false;
                                            }



                                            if (devam)
                                            {
                                                if (pdfTextBuilder == null) pdfTextBuilder = Metodlar.GetPdfText(Metodlar.PdfReaderDondur(file));

                                                string pdfText = pdfTextBuilder.ToString();

                                                Bildirge bildirge = BildirgeCekPdften(pdfText, Metodlar.PdfReaderDondur(file), bildirgeRefNo);

                                                lock (Bildirgeler)
                                                {

                                                    if (!Bildirgeler.Any(p => p.Equals(bildirge, p)))
                                                    {
                                                        Bildirgeler.Add(bildirge);
                                                    }
                                                }

                                                lock (sb)
                                                {
                                                    LogEkle(string.Format("{0} -> Hata veren {1} - {2} ", araci, hizmetYilAy, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti + " onaylı bildirgesi indirildi"));
                                                }


                                                if (pdfBildirgeHataliOkunduMu)
                                                {
                                                    lock (sb)
                                                    {
                                                        LogEkle(string.Format("{0} -> Hata veren {1} - {2} ", araci, hizmetYilAy, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti + " onaylı bildirgesi indirildi fakat okunurken hata meydana geldi"));

                                                        LogEkle(string.Format(bilgiDondurmekIcin));
                                                    }
                                                }
                                                else
                                                {
                                                    hataVerenBildirgeler.Remove(bildirgeRefNo);

                                                    continue;
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            lock (sb)
                                            {
                                                LogEkle(string.Format("{0} -> {1} - {2} ", araci, hizmetYilAy, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti + " bildirgesi indirilirken hata meydana geldi."));
                                            }

                                            if (hataSay < 3)
                                            {
                                                hataSay++;

                                                Thread.Sleep(5000);

                                                goto hatadurumdaDon;
                                            }

                                        }
                                    }
                                    else //Onaysız bildirge ise
                                    {
                                        var onaysizBildirge = item.Value.onaysizBildirge;

                                        string hizmetYilAy = onaysizBildirge.Yil + "/" + onaysizBildirge.Ay.ToString().PadLeft(2, '0');
                                        int yil = Convert.ToInt32(onaysizBildirge.Yil);
                                        int ay = Convert.ToInt32(onaysizBildirge.Ay);

                                        string belgeTuru = onaysizBildirge.BelgeTuru;

                                        string belgeMahiyeti = onaysizBildirge.Mahiyet;

                                        belgeMahiyeti = belgeMahiyeti.Equals("İPTAL") ? "IPTAL" : belgeMahiyeti;

                                        string kanunNo = onaysizBildirge.Kanun.Replace("&nbsp;", "");

                                        bool DuzeltilecekBildirge = onaysizBildirge.Duzeltilecek;

                                        if (string.IsNullOrEmpty(kanunNo)) kanunNo = "00000";

                                        if (kanunNo.Contains("-"))
                                        {
                                            kanunNo = kanunNo.Split('-')[0];
                                        }
                                        else
                                        {
                                            if (kanunNo.Trim().Contains(" "))
                                            {
                                                var temps = kanunNo.Trim().Split(' ');

                                                if (kanunNo.ToLower().Contains("geçersiz"))
                                                {
                                                    kanunNo = temps[2];
                                                }
                                                else kanunNo = temps[temps.Length - 1];
                                            }
                                        }

                                        {

                                            string bildirgeRefNo = onaysizBildirge.RefNo;

                                            if (!DuzeltilecekBildirge)
                                            {
                                                bool devam = true;

                                                int hataSay = 1;
                                            hatadurumdaDon:
                                                try
                                                {
                                                    int denemeSayisi = 0;

                                                tekrarIndirmeyiDene:

                                                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                                                    string sPostData = String.Format("bildirgeRefNo={0}&download=true&action%3AtahakkukfisHizmetPdf=Hizmet+Listesi%28PDF%29", bildirgeRefNo.Replace(" ", "+"));
                                                    var pdfData = AphbWebClient.DownloadFilePost("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tilesislemTamam.action", sPostData);

                                                    string dosyaicerigi = System.Text.Encoding.UTF8.GetString(pdfData);

                                                    if (!dosyaicerigi.StartsWith("%PDF"))
                                                    {

                                                        if (denemeSayisi < 3)
                                                        {
                                                            denemeSayisi++;

                                                            Thread.Sleep(5000);

                                                            goto tekrarIndirmeyiDene;
                                                        }
                                                        else
                                                        {
                                                            lock (sb)
                                                            {
                                                                LogEkle(string.Format("{0} --> Hata veren {1} - {2} ", item.Value.Araci, hizmetYilAy, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti + " onaysız bildirgesi 3 denemeye rağmen indirilemedi."));
                                                            }
                                                        }

                                                        devam = false;

                                                    }


                                                    if (devam)
                                                    {
                                                        var pdfTextBuilder = Metodlar.GetPdfText(Metodlar.PdfReaderDondur(pdfData));

                                                        string pdfText = pdfTextBuilder.ToString();

                                                        Bildirge bildirge = BildirgeCekPdften(pdfText, Metodlar.PdfReaderDondur(pdfData), bildirgeRefNo);
                                                        bildirge.Askida = true;
                                                        bildirge.RefNo = bildirgeRefNo;

                                                        lock (Bildirgeler)
                                                        {

                                                            if (!Bildirgeler.Any(p => p.Equals(bildirge, p)))
                                                            {
                                                                Bildirgeler.Add(bildirge);
                                                            }
                                                        }

                                                        lock (sb)
                                                        {
                                                            LogEkle(string.Format("{0} --> Hata veren {1} - {2} ", item.Value.Araci, hizmetYilAy, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti + " onaysız bildirgesi indirildi"));

                                                        }

                                                        if (pdfBildirgeHataliOkunduMu)
                                                        {
                                                            lock (sb)
                                                            {
                                                                LogEkle(string.Format("{0} --> Hata veren {1} - {2} ", item.Value.Araci, hizmetYilAy, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti + " onaysız bildirgesi indirildi fakat okumada hata meydana geldi"));
                                                                LogEkle(string.Format(bilgiDondurmekIcin));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            hataVerenBildirgeler.Remove(bildirgeRefNo);

                                                            continue;
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                    lock (sb)
                                                    {
                                                        LogEkle(string.Format("{0} --> Hata veren {1} - {2} ", item.Value.Araci, hizmetYilAy, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti + " onaysız bildirgesi indirilirken hata meydana geldi"));
                                                    }

                                                    if (hataSay < 3)
                                                    {
                                                        hataSay++;

                                                        Thread.Sleep(5000);

                                                        goto hatadurumdaDon;
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                var sayac = 0;
                                            DuzeltilecekBildirgeyiAc:

                                                var bildirge = new Bildirge
                                                {
                                                    AraciveyaIsveren = item.Value.Araci,
                                                    Askida = true,
                                                    Ay = ay.ToString(),
                                                    BelgeTuru = belgeTuru.ToInt().ToString(),
                                                    Kanun = kanunNo,
                                                    Mahiyet = belgeMahiyeti,
                                                    Yil = yil.ToString(),
                                                    RefNo = bildirgeRefNo,
                                                    Kisiler = new List<AphbSatir>(),
                                                };



                                                response = AphbWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tilesislemTamam.action", String.Format("bildirgeRefNo={0}&action%3Atahakkukduzeltme=D%C3%BCzeltme+Yap&download=true", bildirgeRefNo.Replace(" ", "+")));

                                                if (response.Contains("Sigortalı Bilgileri Giriş"))
                                                {
                                                    sayac = 0;

                                                    var sayfaNo = 1;

                                                SayfayaGit:

                                                    response = AphbWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/kisiGironyukeleme.action", String.Format("yeniSayfaNo={0}&sayfaSatirSayisi=100", sayfaNo));

                                                    if (response.Contains("Sigortalı Bilgileri Giriş"))
                                                    {
                                                        sayac = 0;

                                                        var html2 = new HtmlAgilityPack.HtmlDocument();

                                                        html2.LoadHtml(response);

                                                        var toplamKayit = html2.DocumentNode.Descendants("span").FirstOrDefault(p => p.GetInnerText().StartsWith("Toplam Kayıt Sayısı")).GetInnerText().Split(':')[1].Trim().ToInt();

                                                        var table = html2.GetElementbyId("kisiGironyukeleme").Descendants("table").FirstOrDefault();

                                                        var trs = table.Descendants("tr").ToList();

                                                        for (int i = 1; i < trs.Count; i++)
                                                        {
                                                            var tds = trs[i].Descendants("td").ToList();

                                                            var tc = tds[4].GetInnerText().Trim();

                                                            if (!long.TryParse(tc, out long tmp)) break;

                                                            bildirge.Kisiler.Add(new AphbSatir
                                                            {
                                                                SiraNo = (i + (sayfaNo - 1) * 100).ToString(),
                                                                SosyalGuvenlikNo = tds[4].GetInnerText().Trim(),
                                                                Adi = tds[5].GetInnerText().Trim(),
                                                                Soyadi = tds[6].GetInnerText().Trim(),
                                                                IlkSoyadi = tds[7].GetInnerText().Trim(),
                                                                Gun = tds[8].GetInnerText().Trim(),
                                                                EksikGunSayisi = tds[9].GetInnerText().Trim(),
                                                                Ucret = tds[10].GetInnerText().Trim().Replace(".", ""),
                                                                Ikramiye = tds[11].GetInnerText().Trim().Replace(".", ""),
                                                                GirisGunu = tds[13].GetInnerText().Trim(),
                                                                CikisGunu = tds[14].GetInnerText().Trim(),
                                                                EksikGunNedeni = tds[15].GetInnerText().Trim(),
                                                                IstenCikisNedeni = tds[16].GetInnerText().Trim(),
                                                                MeslekKod = tds[17].GetInnerText().Trim(),
                                                            });

                                                        }

                                                        if (bildirge.Kisiler.Count != toplamKayit)
                                                        {
                                                            sayfaNo++;
                                                            goto SayfayaGit;
                                                        }


                                                        lock (Bildirgeler)
                                                        {
                                                            if (!Bildirgeler.Any(p => p.Equals(bildirge, p)))
                                                            {
                                                                Bildirgeler.Add(bildirge);
                                                            }
                                                        }

                                                        lock (sb)
                                                        {
                                                            LogEkle(string.Format("{0} --> Hata veren {1} - {2} ", item.Value.Araci, hizmetYilAy, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti + " onaysız bildirgesi indirildi"));
                                                        }

                                                    }
                                                    else
                                                    {
                                                        sayac++;

                                                        if (sayac < 3)
                                                        {
                                                            Thread.Sleep(5000);
                                                            goto SayfayaGit;
                                                        }
                                                        else
                                                        {
                                                            lock (sb)
                                                            {
                                                                LogEkle(string.Format("{0} --> Hata veren {1} - {2} ", item.Value.Araci, hizmetYilAy, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti + " onaysız bildirgesi 3 denemeye rağmen indirilemedi"));
                                                            }

                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    sayac++;

                                                    if (sayac < 3)
                                                    {
                                                        Thread.Sleep(5000);
                                                        goto DuzeltilecekBildirgeyiAc;
                                                    }
                                                    else
                                                    {
                                                        lock (sb)
                                                        {
                                                            LogEkle(string.Format("{0} --> Hata veren {1} - {2} ", item.Value.Araci, hizmetYilAy, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti + " onaysız bildirgesi 3 denemeye rağmen indirilemedi"));
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                    }

                                    index++;
                                }


                                hataDenemeSayisi++;
                            }
                        }

                        if (hataVerenBildirgeler.Count > 0)
                        {
                            LogEkle(string.Format("Hata veren {0} bildirge tekrar denenmesine rağmen indirilemedi.", hataVerenBildirgeler.Count));
                            LogEkle(string.Format("Hata veren bildirge şunlar:"));

                            foreach (var item in hataVerenBildirgeler)
                            {
                                var onaysiz = item.Value.onaysizBildirge != null;
                                var araci = item.Value.Araci;

                                if (!onaysiz)
                                {
                                    var onayliBildirgeRow = item.Value.onayliBildirgeRow as HtmlAgilityPack.HtmlNode;

                                    string hizmetYilAy = onayliBildirgeRow.SelectSingleNode("td[2]/p/text()").InnerText;
                                    int yil = Convert.ToInt32(hizmetYilAy.Split('/')[0]);
                                    int ay = Convert.ToInt32(hizmetYilAy.Split('/')[1]);

                                    string belgeTuru = onayliBildirgeRow.SelectSingleNode("td[3]/p/text()").InnerText;

                                    string belgeMahiyeti = onayliBildirgeRow.SelectSingleNode("td[4]/p/text()").InnerText;

                                    belgeMahiyeti = belgeMahiyeti.Equals("İPTAL") ? "IPTAL" : belgeMahiyeti;

                                    string kanunNo = onayliBildirgeRow.SelectSingleNode("td[5]/p/text()").InnerText.Replace("&nbsp;", "");

                                    if (string.IsNullOrEmpty(kanunNo)) kanunNo = "00000";

                                    string bildirgeRefNo = Regex.Matches(onayliBildirgeRow.InnerHtml, "'H','(.*)'")[0].Groups[1].Value;

                                    LogEkle(string.Format("{0} -> {1} - {2} ", araci, hizmetYilAy, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti +" onaylı bildirge"));
                                }
                                else
                                {
                                    var onaysizBildirge = item.Value.onaysizBildirge;

                                    string hizmetYilAy = onaysizBildirge.Yil + "/" + onaysizBildirge.Ay.ToString().PadLeft(2, '0');
                                    int yil = Convert.ToInt32(onaysizBildirge.Yil);
                                    int ay = Convert.ToInt32(onaysizBildirge.Ay);

                                    string belgeTuru = onaysizBildirge.BelgeTuru;

                                    string belgeMahiyeti = onaysizBildirge.Mahiyet;

                                    belgeMahiyeti = belgeMahiyeti.Equals("İPTAL") ? "IPTAL" : belgeMahiyeti;

                                    string kanunNo = onaysizBildirge.Kanun.Replace("&nbsp;", "");

                                    bool DuzeltilecekBildirge = onaysizBildirge.Duzeltilecek;

                                    if (string.IsNullOrEmpty(kanunNo)) kanunNo = "00000";

                                    if (kanunNo.Contains("-"))
                                    {
                                        kanunNo = kanunNo.Split('-')[0];
                                    }
                                    else
                                    {
                                        if (kanunNo.Trim().Contains(" "))
                                        {
                                            var temps = kanunNo.Trim().Split(' ');

                                            if (kanunNo.ToLower().Contains("geçersiz"))
                                            {
                                                kanunNo = temps[2];
                                            }
                                            else kanunNo = temps[temps.Length - 1];
                                        }
                                    }

                                    LogEkle(string.Format("{0} -> {1} - {2} ", araci, hizmetYilAy, "KANUN:" + kanunNo + " BELGE:" + belgeTuru + " MAHİYET:" + belgeMahiyeti + " onaysız bildirge"));
                                }
                            }

                        }

                        AphbSonaErdi(true);

                        return;
                    }
                    else goto OnaylanmisBildirgelereGit;
                }
                else goto EBildirgeV2AnaSayfayaGit;
            }
        }

        public void AphbSonaErdi(bool Kaydet)
        {
            if (Kaydet)
            {
                BildirgeleriExceleYaz(Bildirgeler);
            }

            LogYaz(sb.ToString());

            Task.Factory.StartNew(() =>
            {
                try
                {
                    var wc = new ProjeGiris(SuanYapilanIsyeriAphb, AphbWebClient.proje);
                    wc.Cookie = AphbWebClient.Cookie;

                    wc.Disconnect(true);
                }
                catch { }

            });


            //tokenSource.Cancel();
            ////token.ThrowIfCancellationRequested();
            //if (token.IsCancellationRequested)
            //{
            //    throw new OperationCanceledException(token);
            //}
        }

        //public StringBuilder GetPdfText(PdfReader reader)
        //{
        //    StringBuilder text = new StringBuilder();
        //    for (int page = 1; page <= reader.NumberOfPages; page++)
        //    {
        //        ITextExtractionStrategy strategy = new LocationTextExtractionStrategy();
        //        string currentText = PdfTextExtractor.GetTextFromPage(reader, page, strategy);
        //        text.Append(currentText);
        //    }
        //    reader.Close();
        //    return text;
        //}

        //public PdfReader PdfReaderDondur(string path)
        //{
        //    return new PdfReader(path);
        //}

        //public PdfReader PdfReaderDondur(byte[] bytes)
        //{
        //    return new PdfReader(bytes);
        //}

        //public List<AphbSatir> GetPdfAphbKisiList(PdfReader reader, string araci)
        //{
        //    List<AphbSatir> ahpbKisiler = new List<AphbSatir>();

        //    for (int page = 1; page <= reader.NumberOfPages; page++)
        //    {
        //        iTextSharp.text.Rectangle rectSira = new iTextSharp.text.Rectangle(50, 570, 0, 100);
        //        RenderFilter[] filterSira = { new RegionTextRenderFilter(rectSira) };
        //        ITextExtractionStrategy strategySira = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterSira);
        //        string currentTextSira = PdfTextExtractor.GetTextFromPage(reader, page, strategySira);

        //        iTextSharp.text.Rectangle rectSgno = new iTextSharp.text.Rectangle(70, 570, 50, 100);
        //        RenderFilter[] filterSgno = { new RegionTextRenderFilter(rectSgno) };
        //        ITextExtractionStrategy strategySgno = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterSgno);
        //        string currentTextSgno = PdfTextExtractor.GetTextFromPage(reader, page, strategySgno);

        //        //150, 570, 100, 100 eskileri
        //        iTextSharp.text.Rectangle rectAdi = new iTextSharp.text.Rectangle(150, 570, 100, 100);
        //        RenderFilter[] filterAdi = { new RegionTextRenderFilter(rectAdi) };
        //        ITextExtractionStrategy strategyAdi = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterAdi);
        //        string currentTextAdi = PdfTextExtractor.GetTextFromPage(reader, page, strategyAdi);

        //        //250, 570, 170, 100 eskileri
        //        //iTextSharp.text.Rectangle rectSoyadi = new iTextSharp.text.Rectangle(255, 570, 165, 100);
        //        //RenderFilter[] filterSoyadi = { new RegionTextRenderFilter(rectSoyadi) };
        //        //ITextExtractionStrategy strategySoyadi = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterSoyadi);
        //        //string currentTextSoyadi = PdfTextExtractor.GetTextFromPage(reader, page, strategySoyadi);

        //        iTextSharp.text.Rectangle rectSoyadi = new iTextSharp.text.Rectangle(215, 570, 163, 100);
        //        RenderFilter[] filterSoyadi = { new RegionTextRenderFilter(rectSoyadi) };
        //        ITextExtractionStrategy strategySoyadi = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterSoyadi);
        //        string currentTextSoyadi = PdfTextExtractor.GetTextFromPage(reader, page, strategySoyadi);


        //        //iTextSharp.text.Rectangle rectDiger = new iTextSharp.text.Rectangle(600, 560, 300, 180);
        //        //RenderFilter[] filterDiger = { new RegionTextRenderFilter(rectDiger) };
        //        //ITextExtractionStrategy strategyDiger = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterDiger);
        //        //string currentTextDiger = PdfTextExtractor.GetTextFromPage(reader, page, strategyDiger);


        //        iTextSharp.text.Rectangle rectDiger = new iTextSharp.text.Rectangle(600, 560, 225, 180);
        //        RenderFilter[] filterDiger = { new RegionTextRenderFilter(rectDiger) };
        //        ITextExtractionStrategy strategyDiger = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterDiger);
        //        string currentTextDiger = PdfTextExtractor.GetTextFromPage(reader, page, strategyDiger);


        //        string[] siras = currentTextSira.Split('\n');
        //        string[] sgnos = currentTextSgno.Split('\n');
        //        string[] adis = currentTextAdi.Split('\n');
        //        string[] soyadis = currentTextSoyadi.Split('\n');
        //        string[] digers = currentTextDiger.Split('\n');

        //        for (int i = 0; i < siras.Length; i++)
        //        {
        //            AphbSatir ahpbKisi = new AphbSatir();
        //            ahpbKisi.SiraNo = siras[i];
        //            ahpbKisi.SosyalGuvenlikNo = sgnos[i];
        //            ahpbKisi.Adi = adis[i];
        //            ahpbKisi.Soyadi = soyadis[i].Trim();

        //            //ahpbKisi.Soyadi = soyadis[i].Split(' ')[0] != " " ? soyadis[i].Split(' ')[0] : "######";

        //            if (ahpbKisi.Soyadi == "######" || ahpbKisi.Soyadi == "")
        //            {
        //                pdfBildirgeHataliOkunduMu = true;
        //                bilgiDondurmekIcin = "Üstteki bildirgedeki " + (page) + " sayfası " + i + ". sırasındaki soyad okunamadı !!! " + adis[i] + " isimli";
        //            }

        //            //if (soyadis[i].Split(' ').Length > 1)
        //            //    ahpbKisi.IlkSoyadi = soyadis[i].Split(' ')[1];


        //            //ahpbKisi.Ucret = digers[i].Split(' ')[0];
        //            //ahpbKisi.Ikramiye = digers[i].Split(' ')[1];
        //            //ahpbKisi.Gun = digers[i].Split(' ')[2];
        //            //ahpbKisi.EksikGunSayisi = digers[i].Split(' ')[3];
        //            //ahpbKisi.GirisGunu = digers[i].Split(' ')[4] != "0" ? digers[i].Split(' ')[4].PadLeft(4, '0').Insert(2, "/") : "";
        //            //ahpbKisi.CikisGunu = digers[i].Split(' ')[5] != "0" ? digers[i].Split(' ')[5].PadLeft(4, '0').Insert(2, "/") : "";
        //            //ahpbKisi.IstenCikisNedeni = digers[i].Split(' ')[6] != "0" && digers[i].Split(' ')[6] != "00" ? digers[i].Split(' ')[6] : "";
        //            //ahpbKisi.EksikGunNedeni = digers[i].Split(' ')[7] != "0" && digers[i].Split(' ')[7] != "00" ? digers[i].Split(' ')[7] : "";
        //            //ahpbKisi.MeslekKod = digers[i].Split(' ')[8];

        //            var splits = digers[i].Split(' ').ToList();

        //            while (true)
        //            {
        //                if (!decimal.TryParse(splits[0], out decimal t) || !splits[0].Contains(","))
        //                {
        //                    if (ahpbKisi.IlkSoyadi == null)
        //                        ahpbKisi.IlkSoyadi = splits[0].Trim() + " ";
        //                    else
        //                        ahpbKisi.IlkSoyadi += splits[0].Trim() + " ";
        //                    splits.RemoveAt(0);
        //                    continue;
        //                }
        //                else break;
        //            }

        //            if (ahpbKisi.IlkSoyadi != null)
        //                ahpbKisi.IlkSoyadi = ahpbKisi.IlkSoyadi.Trim();

        //            ahpbKisi.Ucret = splits[0];
        //            ahpbKisi.Ikramiye = splits[1];
        //            ahpbKisi.Gun = splits[2];
        //            ahpbKisi.EksikGunSayisi = splits[3];
        //            ahpbKisi.GirisGunu = splits[4] != "0" ? splits[4].PadLeft(4, '0').Insert(2, "/") : "";
        //            ahpbKisi.CikisGunu = splits[5] != "0" ? splits[5].PadLeft(4, '0').Insert(2, "/") : "";
        //            ahpbKisi.IstenCikisNedeni = splits[6] != "0" && splits[6] != "00" ? splits[6] : "";
        //            ahpbKisi.EksikGunNedeni = splits[7] != "0" && splits[7] != "00" ? splits[7] : "";
        //            ahpbKisi.MeslekKod = splits[8];

        //            ahpbKisi.Araci = araci;
        //            ahpbKisiler.Add(ahpbKisi);
        //        }

        //    }

        //    reader.Close();

        //    return ahpbKisiler;
        //}

        public Bildirge BildirgeCek(string responseHtml, string BildirgeRefNo)
        {
            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();

            html.LoadHtml(responseHtml);

            var tdler = html.DocumentNode.Descendants("td");

            Bildirge bildirge = new Bildirge();

            string Ek = null;

            string Unvani = null;

            string YilAy = null;

            foreach (var item in tdler)
            {
                if (item.InnerText != null && item.InnerText == "İşyeri Sicil No")
                {
                    Ek = item.NextSibling.NextSibling.NextSibling.InnerText.Split('-')[1].Trim();
                }
                else if (item.InnerText != null && item.InnerText == "İşyeri Ünvanı")
                {
                    Unvani = item.NextSibling.NextSibling.InnerText.Trim();
                }
                else if (item.InnerText != null && item.InnerText == "Yıl - Ay")
                {
                    YilAy = item.NextSibling.NextSibling.InnerText.Trim();

                    bildirge.Yil = YilAy.Split('-')[0].Trim();

                    bildirge.Ay = YilAy.Split('-')[1].Trim();

                    if (bildirge.Ay.StartsWith("0")) bildirge.Ay = bildirge.Ay.Replace("0", "");
                }
                else if (item.InnerText != null && item.InnerText == "Belge Çeşidi")
                {
                    bildirge.BelgeTuru = item.NextSibling.NextSibling.InnerText.Split('-')[0].Trim();
                }
                else if (item.InnerText != null && item.InnerText == "Mahiyet")
                {
                    bildirge.Mahiyet = item.NextSibling.NextSibling.NextSibling.InnerText.Trim();
                }
                else if (item.InnerText != null && item.InnerText == "Kanun")
                {
                    bildirge.Kanun = item.NextSibling.NextSibling.InnerText.Trim();

                    bildirge.Kanun = bildirge.Kanun.Replace("(", " (");

                    if (bildirge.Kanun.Split(' ').Length > 1)
                    {
                        bildirge.Kanun = bildirge.Kanun.Split(' ')[0];
                    }
                }
                else if (item.InnerText != null && item.InnerText == "Onay Tarihi")
                {
                    bildirge.Askida = item.NextSibling.NextSibling.InnerText.Trim() == "ONAYLANMAMIŞTIR";
                }
            }

            var tables = html.DocumentNode.Descendants("table");

            foreach (var table in tables)
            {
                if (table.InnerText.Contains("S.Güvenlik No"))
                {
                    var trs = table.Descendants("tr");

                    foreach (var tr in trs)
                    {
                        var tds = tr.Descendants("td").ToList();

                        if (long.TryParse(tds[1].InnerText.Trim(), out long tcno))
                        {
                            AphbSatir kisi = new AphbSatir();

                            kisi.SiraNo = tds[0].InnerText.Trim();

                            kisi.SosyalGuvenlikNo = tds[1].InnerText.Trim();

                            kisi.Adi = tds[2].InnerText.Trim();

                            kisi.Soyadi = tds[3].InnerText.Trim();

                            kisi.IlkSoyadi = tds[4].InnerText.Trim();

                            kisi.Ucret = tds[5].InnerText.Trim();

                            kisi.Ikramiye = tds[6].InnerText.Trim();

                            kisi.Gun = tds[7].InnerText.Trim();

                            kisi.EksikGunSayisi = tds[8].InnerText.Trim();

                            kisi.GirisGunu = tds[9].InnerText.Trim();

                            kisi.CikisGunu = tds[10].InnerText.Trim();

                            kisi.EksikGunNedeni = tds[11].InnerText.Trim();

                            kisi.IstenCikisNedeni = tds[12].InnerText.Trim();

                            kisi.MeslekKod = tds[13].InnerText.Trim();

                            bildirge.Kisiler.Add(kisi);

                        }
                    }

                    break;
                }
            }

            if (Ek != "000")
            {
                bildirge.AraciveyaIsveren = Ek + "-" + Unvani;
            }

            bildirge.RefNo = BildirgeRefNo;

            return bildirge;
        }
        public Bildirge BildirgeCekPdften(string pdfText, PdfReader reader, string bildirgeRefNo)
        {
            Bildirge bildirge = new Bildirge();
            bildirge.Kisiler = new List<AphbSatir>();

            string[] pdfRows = pdfText.Split('\n');

            string IsyeriSicil = pdfRows.FirstOrDefault(q => q.Contains("İşyeri Sicil No"));
            string Unvan = pdfRows.FirstOrDefault(q => q.Contains("İşyeri Ünvanı"));

            string yilAy = pdfRows.FirstOrDefault(q => q.Contains("Yıl - Ay"));
            string belgeCesidi = pdfRows.FirstOrDefault(q => q.Contains("Belge Çeşidi"));
            string mahiyet = pdfRows.FirstOrDefault(q => q.Contains("Mahiyet"));
            string kanun = pdfRows.FirstOrDefault(q => q.Contains("Kanun"));

            if (!string.IsNullOrEmpty(yilAy))
            {
                var sonEleman = yilAy.Split(':').Length - 1;

                var yilay = yilAy.Split(':')[sonEleman].Trim();

                bildirge.Yil = yilay.Split('/')[0].Trim();

                bildirge.Ay = yilay.Split('/')[1].Trim().Length > 2 ? yilay.Split('/')[1].Trim().Substring(0, 2) : yilay.Split('/')[1].Trim();
            }

            if (!string.IsNullOrEmpty(belgeCesidi))
            {
                bildirge.BelgeTuru = belgeCesidi.Split(':')[1].Trim();
            }

            if (!string.IsNullOrEmpty(mahiyet))
            {
                bildirge.Mahiyet = mahiyet.Split(':')[1].Trim();
            }

            if (!string.IsNullOrEmpty(kanun))
            {
                bildirge.Kanun = kanun.Split(':')[1].Contains("-") ? kanun.Split(':')[1].Trim().Substring(0, 5) : kanun.Split(':')[1].Trim();
            }

            bildirge.Askida = false;
            bildirge.RefNo = bildirgeRefNo;


            if (!IsyeriSicil.Contains("/000"))
            {
                bildirge.AraciveyaIsveren = IsyeriSicil.Split('/')[1].Trim() + "-" + Unvan.Split(':')[1].Trim();
            }

            var readResult = Metodlar.GetPdfAphbKisiList(reader, bildirge.AraciveyaIsveren);

            pdfBildirgeHataliOkunduMu = readResult.pdfBildirgeHataliOkunduMu;
            bilgiDondurmekIcin = readResult.bilgiDondurmekIcin;

            if (! readResult.pdfBildirgeHataliOkunduMu)
                bildirge.Kisiler.AddRange(readResult.satirlar);

            return bildirge;

        }
        public string RenderText(TextRenderInfo renderInfo)
        {
            var r = renderInfo;
            return r.GetText();
        }

        void BildirgeleriExceleYaz(List<Bildirge> bildirgeler)
        {
            string aphbyol = Metodlar.FormBul(SuanYapilanIsyeriAphb, Enums.FormTuru.Aphb);

            DataTable dtAphb = new DataTable("Aphb");


            for (int i = 0; i < 27; i++)
            {
                dtAphb.Columns.Add("Column" + i.ToString());
            }

            dtAphb.Columns[0].DataType = typeof(Int32);
            dtAphb.Columns[1].DataType = typeof(Int32);
            dtAphb.Columns[4].DataType = typeof(Int32);
            dtAphb.Columns[11].DataType = typeof(decimal);
            dtAphb.Columns[12].DataType = typeof(decimal);
            dtAphb.Columns[13].DataType = typeof(Int32);

            int BildirgeSayisi = 0;

            bool KaydetmedeHataVar = false;

            int.TryParse(SuanYapilanIsyeriAphb.TaseronNo, out int araciKod);
            string araci = araciKod > 0 ? araciKod.ToString().PadLeft(3, '0')+"-" : "Ana İşveren";


            //var dict = Bildirgeler.GroupBy(p => p.Yil + "-" + p.Ay).ToDictionary(x => x.Key, x => new KeyValuePair<bool, bool>(x.Any(z => !z.Mahiyet.ToUpper().EndsWith("PTAL") && z.Kanun.EndsWith("6486")), x.Any(z => !z.Mahiyet.ToUpper().EndsWith("PTAL") && z.Kanun.PadLeft(5, '0').Equals("05510"))));
            //var dict2 = bildirgeler.GroupBy(p => p.Yil + "-" + p.Ay).ToDictionary(x => x.Key, x => new KeyValuePair<bool, bool>(x.Any(z => !z.Mahiyet.ToUpper().EndsWith("PTAL") && z.Kanun.EndsWith("6486")), x.Any(z => !z.Mahiyet.ToUpper().EndsWith("PTAL") && z.Kanun.PadLeft(5, '0').Equals("05510"))));

            var dict = Bildirgeler.Where(p => p.AraciveyaIsveren.StartsWith(araci)).GroupBy(p => p.Yil + "-" + p.Ay).ToDictionary(x => x.Key, x => new KeyValuePair<bool, bool>(x.Any(z => !z.Mahiyet.ToUpper().EndsWith("PTAL") && z.Kanun.EndsWith("6486")), x.Any(z => !z.Mahiyet.ToUpper().EndsWith("PTAL") && z.Kanun.PadLeft(5, '0').Equals("05510"))));
            var dict2 = bildirgeler.Where(p => p.AraciveyaIsveren.StartsWith(araci)).GroupBy(p => p.Yil + "-" + p.Ay).ToDictionary(x => x.Key, x => new KeyValuePair<bool, bool>(x.Any(z => !z.Mahiyet.ToUpper().EndsWith("PTAL") && z.Kanun.EndsWith("6486")), x.Any(z => !z.Mahiyet.ToUpper().EndsWith("PTAL") && z.Kanun.PadLeft(5, '0').Equals("05510"))));

            bildirgeler.ForEach(bildirge =>
            {

                if (bildirge.Kisiler.Count > 0)
                {
                    BildirgeSayisi++;
                }

                string Kanun = bildirge.Kanun;

                if (bildirge.Askida)
                {
                    if (bildirge.Kanun.EndsWith("6111") || bildirge.Kanun.EndsWith("6645") || bildirge.Kanun.EndsWith("7103") || bildirge.Kanun.EndsWith("2828") || bildirge.Kanun.EndsWith("6322") || bildirge.Kanun.EndsWith("25510"))
                    {
                        bildirge.OrijinalKanunNo = bildirge.Kanun;

                        //Kanun = "05510";

                        string Kanun6486 = Metodlar.Isyeri6486KanunBul(SuanYapilanIsyeriAphb.IsyeriSicilNo);

                        var yilay = bildirge.Yil + "-" + bildirge.Ay;

                        if ((dict.ContainsKey(yilay) && dict[yilay].Key) || (dict2.ContainsKey(yilay) && dict2[yilay].Key))
                        {
                            Kanun = Kanun6486;
                        }
                        else if ((dict.ContainsKey(yilay) && dict[yilay].Value) || (dict2.ContainsKey(yilay) && dict2[yilay].Value))
                        {
                            Kanun = "05510";
                        }
                        else
                        {
                            Kanun = "05510";
                            //Kanun = String.IsNullOrEmpty(Kanun6486) ? "05510" : Kanun6486;
                        }

                        if (bildirge.Kanun.EndsWith("6322") || bildirge.Kanun.EndsWith("25510"))
                        {
                            Kanun = "05510";
                        }
                    }
                }


                foreach (AphbSatir kisi in bildirge.Kisiler)
                {
                    DataRow row = dtAphb.NewRow();

                    row[(int)Enums.AphbHucreBilgileri.Yil] = bildirge.Yil;

                    row[(int)Enums.AphbHucreBilgileri.Ay] = bildirge.Ay;

                    row[(int)Enums.AphbHucreBilgileri.Kanun] = Kanun == "00000" ? "" : Kanun;

                    row[(int)Enums.AphbHucreBilgileri.Mahiyet] = bildirge.Mahiyet;

                    row[(int)Enums.AphbHucreBilgileri.BelgeTuru] = bildirge.BelgeTuru;

                    row[(int)Enums.AphbHucreBilgileri.SiraNo] = kisi.SiraNo;

                    row[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo] = kisi.SosyalGuvenlikNo;

                    row[(int)Enums.AphbHucreBilgileri.Ad] = kisi.Adi;

                    row[(int)Enums.AphbHucreBilgileri.Soyad] = kisi.Soyadi;

                    row[(int)Enums.AphbHucreBilgileri.IlkSoyadi] = kisi.IlkSoyadi;

                    row[(int)Enums.AphbHucreBilgileri.Ucret] = string.IsNullOrEmpty(kisi.Ucret) ? "0" : kisi.Ucret;

                    row[(int)Enums.AphbHucreBilgileri.Ikramiye] = string.IsNullOrEmpty(kisi.Ikramiye) ? "0" : kisi.Ikramiye;

                    row[(int)Enums.AphbHucreBilgileri.Gun] = kisi.Gun;

                    row[(int)Enums.AphbHucreBilgileri.UCG] = kisi.UCG;

                    row[(int)Enums.AphbHucreBilgileri.EksikGun] = kisi.EksikGunSayisi;

                    row[(int)Enums.AphbHucreBilgileri.GirisGunu] = kisi.GirisGunu;

                    row[(int)Enums.AphbHucreBilgileri.CikisGunu] = kisi.CikisGunu;

                    row[(int)Enums.AphbHucreBilgileri.EksikGunSebebi] = kisi.EksikGunNedeni;

                    row[(int)Enums.AphbHucreBilgileri.IstenCikisNedeni] = kisi.IstenCikisNedeni;

                    row[(int)Enums.AphbHucreBilgileri.MeslekKod] = kisi.MeslekKod;

                    row[(int)Enums.AphbHucreBilgileri.Araci] = bildirge.AraciveyaIsveren ?? string.Empty;

                    row[(int)Enums.AphbHucreBilgileri.OnayDurumu] = bildirge.Askida ? "Onaylanmamış" : "";

                    row[(int)Enums.AphbHucreBilgileri.OrijinalKanunNo] = bildirge.OrijinalKanunNo;

                    row[(int)Enums.AphbHucreBilgileri.BildirgeRefNo] = bildirge.RefNo.Trim();

                    dtAphb.Rows.Add(row);
                }
            });

            if (dtAphb.Rows.Count > 0 && bildirgeler.Count > 0)
            {
                LogEkle("Bildirgeler excel dosyasına yazılıyor...Lütfen bekleyiniz");

                if (Metodlar.FormKaydet(SuanYapilanIsyeriAphb, dtAphb, dtMevcutAphb, Enums.FormTuru.Aphb, null) != null)
                {
                    BasariylaKaydedildi = true;

                    ToplamBildirgeSayisi += BildirgeSayisi;
                }
                else
                {
                    KaydetmedeHataVar = true;
                }

            }

            if (!KaydetmedeHataVar)
            {
                if (bildirgeler.Count > 0)
                {
                    LogEkle("Bildirgeler excel dosyasına yazıldı.Eklenen toplam bildirge sayısı: " + BildirgeSayisi);
                }

                LogEkle(String.Format("'{0}' şirketine ait '{1}' işyeri için Aphb indirme tamamlandı.Toplam {2} bildirge kaydedildi", SuanYapilanIsyeriAphb.Sirketler.SirketAdi, SuanYapilanIsyeriAphb.SubeAdi, ToplamBildirgeSayisi));

            }
            else if (KaydetmedeHataVar)
            {
                LogEkle(String.Format("Bildirgeler excel dosyasına tekrar kaydedilirken hata meydana geldi.Dosya kullanımda olabilir.Toplam {0} adet bildirge kaydedilemedi", BildirgeSayisi));
            }
        }

        public static void LogYaz(string log)
        {
            int i = 0;

            while (i <= 5)
            {
                i++;

                try
                {
                    File.AppendAllText(Application.StartupPath + "\\log.txt", log);

                    break;
                }
                catch
                {
                    Thread.Sleep(200);
                }
            }

        }

        public void IslemiIptalEt(bool Kaydet)
        {
            this.Cancel();

            AphbSonaErdi(Kaydet);
        }

    }
}
