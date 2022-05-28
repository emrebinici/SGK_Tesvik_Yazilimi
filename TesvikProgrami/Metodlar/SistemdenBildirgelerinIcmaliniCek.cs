using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static BildirgeIcmaliResponse SistemdenBildirgelerinIcmaliniCek(Isyerleri isyeri, ProjeGiris projeGiris)
        {

            var yeniWebClientOlustur = projeGiris == null;

            if (yeniWebClientOlustur)
            {
                projeGiris = new ProjeGiris(isyeri, Enums.ProjeTurleri.EBildirgeV2);
            }

            var result = new BildirgeIcmaliResponse();

            try
            {
                if (!projeGiris.Connected)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        projeGiris.Connect();

                        if (projeGiris.Connected || projeGiris.GirisYapilamiyor) break;

                        Thread.Sleep(1000);
                    }
                }

                if (!projeGiris.Connected)
                {
                    throw new Exception(string.IsNullOrEmpty(projeGiris.GirisYapilamamaNedeni) ? "10 denemeye rağmen sisteme giriş yapılamadı" : projeGiris.GirisYapilamamaNedeni);
                }

                var onaylitesvikIcmalleri = Program.TumTesvikler.ToDictionary(x => x.Key, x => new List<BildirgeYuklemeIcmal>());
                var onaysiztesvikIcmalleri = Program.TumTesvikler.ToDictionary(x => x.Key, x => new List<BildirgeYuklemeIcmal>());
                var onayliveOnaysiztesvikIcmalleri = Program.TumTesvikler.ToDictionary(x => x.Key, x => new List<BildirgeYuklemeIcmal>());
                var hataVerenBildirgeler = new Dictionary<Bildirge, string>();

                var sayac = 0;

            tekrarDene:

                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();

                var yanit = projeGiris.Get("https://ebildirge.sgk.gov.tr/EBildirgeV2/subeOnay/subeOnayislemIsyeriDurumBigileriGetir.action");

                if (yanit.Contains("Şube Onay İşlem Yapılan Belge Durum Bilgisi Girişi"))
                {
                    sayac = 0;

                    html.LoadHtml(yanit);

                    var token = html.DocumentNode.Descendants("input").FirstOrDefault(p => p.GetAttributeValue("name", "").Equals("struts.token.name")).GetAttributeValue("value", "");

                tekrarDene2:
                    yanit = projeGiris.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/subeOnay/tilesislemTamam.action", String.Format("struts.token.name=token&token={0}&sgmdekiSubeOnayBelgeleri=false&islemDurumTip=0&action%3AsubeOnayislemDurumunaGoreIsyeriTahakkuklariGetir=Belgeleri+Getir", token));

                    if (yanit.Contains("Kayıt Bulunamadı") || yanit.Contains("İlk Kayıt Tarihi"))
                    {
                        if (yanit.Contains("Kayıt Bulunamadı")) throw new Exception("V2 ekranında D kodlu herhangi bir kayıt bulunmamaktadır");

                        html.LoadHtml(yanit);

                        token = html.DocumentNode.Descendants("input").FirstOrDefault(p => p.GetAttributeValue("name", "").Equals("struts.token.name")).GetAttributeValue("value", "");

                        var table = html.DocumentNode.Descendants("table").FirstOrDefault(p => p.GetAttributeValue("class", "").Equals("gradienttable"));

                        var trs = table.Descendants("tr");

                        List<Classes.Bildirge> bildirgeler = new List<Bildirge>();

                        for (int i = 1; i < trs.Count(); i++)
                        {
                            if (!trs.ElementAt(i).InnerText.Contains("İşlem Yapılması Bekleniyor") && !trs.ElementAt(i).InnerText.Contains("Belge Onaylanmıştır") && !trs.ElementAt(i).InnerText.Contains("İşlemler Devam Ediyor")) continue;

                            bool askida = trs.ElementAt(i).InnerText.Contains("İşlem Yapılması Bekleniyor") || trs.ElementAt(i).InnerText.Contains("İşlemler Devam Ediyor");

                            var tds = trs.ElementAt(i).Descendants("td").ToList();

                            var belgeNo = tds[7].InnerText.ToInt().ToString();
                            var kanun = tds[8].InnerText.PadLeft(5, '0');
                            var tur = tds[9].InnerText.Trim();
                            var mahiyet = tds[6].InnerText;
                            var araci = tds[4].InnerText.Trim();
                            var ilkKayitTarihi = Convert.ToDateTime(tds[10].InnerText);

                            var taseronNo = isyeri.TaseronNo.ToInt().ToString().PadLeft(3, '0');

                            if (!araci.Equals(taseronNo)) continue;

                            if (!tur.Equals("D")) continue;

                            if (!mahiyet.Equals("IPTAL"))
                            {
                                var tesvik = Program.TumTesvikler.FirstOrDefault(p => p.Value.Kanun.PadLeft(5, '0').Equals(kanun) || p.Value.AltKanunlar.Contains(kanun)).Value;

                                if (tesvik == null) continue;

                                if (
                                        (
                                            tesvik.DestekKapsaminaGirmeyenBelgeTurleri.Count > 0 && tesvik.DestekKapsaminaGirmeyenBelgeTurleri.Contains(belgeNo) == false
                                        )
                                        ||
                                        (
                                            tesvik.DestekKapsaminaGirenBelgeTurleri.Count > 0 && tesvik.DestekKapsaminaGirenBelgeTurleri.Contains(belgeNo)
                                        )
                                   )
                                {
                                    //Belge türü teşvik verilebilecek belge türlerinden biri ise.
                                }
                                else continue;
                            }
                            else if (TesvikHesaplamaSabitleri.BagliKanunlarIcmalHesaplama.ContainsKey(kanun) == false) continue;

                            var donem = tds[5].InnerText;
                            var refNo = Regex.Match(tds[14].FirstChild.FirstChild.GetAttributeValue("onclick", ""), ".*aphbBelgeGetir(.*)").Groups[1].Value.Trim('(').Trim(')');

                            bildirgeler.Add(new Bildirge
                            {
                                Yil = donem.Split('/')[0],
                                Ay = donem.Split('/')[1].ToInt().ToString(),
                                BelgeTuru = belgeNo,
                                Kanun = kanun,
                                Mahiyet = mahiyet,
                                AraciveyaIsveren = tds[4].InnerText,
                                RefNo = refNo,
                                Askida = askida,
                                ilkKayitTarihi = ilkKayitTarihi
                            });
                        }

                        Parallel.For(0, bildirgeler.Count, new ParallelOptions { MaxDegreeOfParallelism = 20 }, (index) =>
                        {
                            var wc = new ProjeGiris(isyeri, Enums.ProjeTurleri.EBildirgeV2);

                            wc.Connected = true;
                            wc.Cookie = projeGiris.Cookie;

                            var bildirge = bildirgeler[index];

                            var sonuc = KisileriCek(ref bildirge, wc, token);

                            if (sonuc != "OK")
                            {
                                lock (hataVerenBildirgeler)
                                {
                                    hataVerenBildirgeler.Add(bildirge, sonuc);
                                }
                            }
                        });


                        var kisiler7252 = new List<BasvuruKisiDownload7252>();

                        var gruplar = bildirgeler.GroupBy(p => p.Yil + "-" + p.Ay + "-" + p.BelgeTuru).ToDictionary(x => x.Key, x => x.ToList());

                        var kisiGunler7252 = new Dictionary<string, Dictionary<string, List<AphbSatir>>>();
                        var olasiIptaller7252 = new Dictionary<string, List<AphbSatir>>();

                        var tesvik7252 = Program.TumTesvikler["7252"];

                        if (bildirgeler.Any(p => p.Kanun.EndsWith("7252")))
                        {

                            var basvuruFormu = Metodlar.FormBul(isyeri, Enums.FormTuru.BasvuruFormu);

                            if (basvuruFormu != null)
                            {
                                var ds = Metodlar.BasvuruListesiniYukle(basvuruFormu);

                                if (ds.Tables.Contains("7252"))
                                {
                                    var dt = ds.Tables["7252"];

                                    kisiler7252 = dt.AsEnumerable().Select(row => new BasvuruKisiDownload7252
                                    {
                                        TcKimlikNo = row[(int)Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString(),
                                        Sicil = row[(int)Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.Sicil]].ToString(),
                                        OrtalamaGunSayisi = row[(int)Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.Baz]].ToString(),
                                        TesvikSuresiBaslangic = row[(int)Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.TesvikBaslangic]].ToString(),
                                        TesvikSuresiBitis = row[(int)Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.TesvikBitis]].ToString()
                                    }).ToList();

                                }
                            }
                            else
                            {
                                var response = Metodlar.Sistemden7252ListesiniCek(isyeri);

                                if (!string.IsNullOrEmpty(response.HataMesaji))
                                {
                                    MessageBox.Show("Sistemden 7252 listesi çekilemedi." + Environment.NewLine + Environment.NewLine + "Hata Mesajı:" + response.HataMesaji, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else
                                {
                                    if (response.Result != null)
                                    {
                                        kisiler7252 = response.Result.ToList();
                                    }
                                }
                            }


                            var ayYilGrupBildirgeler = bildirgeler.GroupBy(p => p.Yil + "-" + p.Ay).ToDictionary(x => x.Key, x => x.ToList());
                            //var ayYilGrupIptaller = bildirgeler.Where(p=> ! p.Mahiyet.Equals("ASIL") && ! p.Mahiyet.Equals("EK")).GroupBy(p => p.Yil + "-" + p.Ay).ToDictionary(x => x.Key, x => x.ToList());

                            foreach (var kvAyYilGrup in ayYilGrupBildirgeler)
                            {
                                var yilAy = kvAyYilGrup.Key;

                                foreach (var bildirge7252 in kvAyYilGrup.Value)
                                {
                                    if (bildirge7252.Kanun.EndsWith("7252"))
                                    {
                                        foreach (var kisi7252 in bildirge7252.Kisiler)
                                        {
                                            if (!kisiGunler7252.ContainsKey(kisi7252.SosyalGuvenlikNo))
                                            {
                                                kisiGunler7252.Add(kisi7252.SosyalGuvenlikNo, new Dictionary<string, List<AphbSatir>>());
                                            }

                                            if (!kisiGunler7252[kisi7252.SosyalGuvenlikNo].ContainsKey(yilAy))
                                            {
                                                kisiGunler7252[kisi7252.SosyalGuvenlikNo].Add(yilAy, new List<AphbSatir>());
                                            }

                                            kisiGunler7252[kisi7252.SosyalGuvenlikNo][yilAy].Add(new AphbSatir
                                            {
                                                SosyalGuvenlikNo = kisi7252.SosyalGuvenlikNo,
                                                BelgeTuru = bildirge7252.BelgeTuru,
                                                Mahiyet = bildirge7252.Mahiyet,
                                                Kanun = bildirge7252.Kanun,
                                                Yil = bildirge7252.Yil,
                                                Ay = bildirge7252.Ay,
                                                Gun = kisi7252.Gun,
                                                Ucret = kisi7252.Ucret,
                                                Ikramiye = kisi7252.Ikramiye,
                                                BildirgeIlkKayitTarihi = bildirge7252.ilkKayitTarihi,
                                                OnayDurumu = bildirge7252.Askida ? "Onaysız" : "Onaylı",
                                                BildirgeRefNo = bildirge7252.RefNo
                                            });
                                        }
                                    }
                                }
                            }

                            var hataGruplari7252 = hataVerenBildirgeler.Where(p => p.Key.Kanun.EndsWith("7252") && p.Key.Mahiyet.EndsWith("PTAL")).GroupBy(p => p.Key.Yil + "-" + p.Key.Ay).ToDictionary(x => x.Key, x => x.Select(z => z.Key).ToList());

                            foreach (var kvAyYilGrup in hataGruplari7252)
                            {
                                var yilAy = kvAyYilGrup.Key;

                                foreach (var bildirge7252 in kvAyYilGrup.Value)
                                {
                                    if (bildirge7252.Kanun.EndsWith("7252"))
                                    {


                                        if (!olasiIptaller7252.ContainsKey(yilAy))
                                        {
                                            olasiIptaller7252.Add(yilAy, new List<AphbSatir>());
                                        }

                                        olasiIptaller7252[yilAy].Add(new AphbSatir
                                        {
                                            BelgeTuru = bildirge7252.BelgeTuru,
                                            Mahiyet = bildirge7252.Mahiyet,
                                            Kanun = bildirge7252.Kanun,
                                            Yil = bildirge7252.Yil,
                                            Ay = bildirge7252.Ay,
                                            Gun = "0",
                                            BildirgeIlkKayitTarihi = bildirge7252.ilkKayitTarihi,
                                            OnayDurumu = bildirge7252.Askida ? "Onaysız" : "Onaylı",
                                            BildirgeRefNo = bildirge7252.RefNo
                                        }) ;

                                    }
                                }
                            }


                            foreach (var kvKisi7252 in kisiGunler7252)
                            {
                                var tc = kvKisi7252.Key;
                                var aylar = kvKisi7252.Value;

                                var basvuruKayitlari7252 = kisiler7252.Where(p => p.TcKimlikNo == tc || p.Sicil == tc);

                                if (basvuruKayitlari7252.Count() > 0)
                                {
                                    foreach (var kvYilAy in aylar)
                                    {
                                        var yilAy = kvYilAy.Key;
                                        var yil = yilAy.Split('-')[0].ToInt();
                                        var ay = yilAy.Split('-')[1].ToInt();
                                        var satirlar = kvYilAy.Value;
                                        var tarihYilAy = new DateTime(yil, ay, 1);

                                        var gecerliBasvuruKayitlari = basvuruKayitlari7252.Where(p => Convert.ToDateTime(p.TesvikSuresiBaslangic) <= tarihYilAy && tarihYilAy <= Convert.ToDateTime(p.TesvikSuresiBitis));

                                        if (gecerliBasvuruKayitlari.Count() > 0)
                                        {
                                            for (int i = 0; i < satirlar.Count; i++)
                                            {
                                                var satir = satirlar[i];

                                                if (satir.Gun == "-1") continue;

                                                if (satir.Mahiyet.EndsWith("PTAL"))
                                                {
                                                    bool asilBulundu = false;
                                                    bool iptalBulundu = false;

                                                    for (int j = 0; j < satirlar.Count; j++)
                                                    {
                                                        if (i == j) continue;

                                                        var satir2 = satirlar[j];

                                                        if (satir2.Gun == "-1") continue;

                                                        if (
                                                            satir2.Mahiyet.Equals("ASIL") &&
                                                            satir.SosyalGuvenlikNo.Equals(satir2.SosyalGuvenlikNo) &&
                                                            satir.BelgeTuru.Equals(satir2.BelgeTuru) &&
                                                            satir.Kanun.Equals(satir2.Kanun) &&
                                                            satir.Gun.Equals(satir2.Gun) &&
                                                            satir.Ucret.Equals(satir2.Ucret) &&
                                                            satir.Ikramiye.Equals(satir2.Ikramiye) &&
                                                            satir.BildirgeIlkKayitTarihi.Equals(satir2.BildirgeIlkKayitTarihi) &&
                                                            satir.OnayDurumu.Equals(satir2.OnayDurumu)
                                                           )
                                                        {
                                                            asilBulundu = true;
                                                            iptalBulundu = true;
                                                            satir2.Gun = "-1";
                                                            break;
                                                        }
                                                    }

                                                    if (!asilBulundu)
                                                    {
                                                        for (int j = 0; j < satirlar.Count; j++)
                                                        {
                                                            if (i == j) continue;

                                                            var satir2 = satirlar[j];

                                                            if (
                                                                satir2.Mahiyet.Equals("EK") &&
                                                                satir.SosyalGuvenlikNo.Equals(satir2.SosyalGuvenlikNo) &&
                                                                satir.BelgeTuru.Equals(satir2.BelgeTuru) &&
                                                                satir.Kanun.Equals(satir2.Kanun) &&
                                                                satir.Gun.Equals(satir2.Gun) &&
                                                                satir.Ucret.Equals(satir2.Ucret) &&
                                                                satir.Ikramiye.Equals(satir2.Ikramiye) &&
                                                                satir.BildirgeIlkKayitTarihi.Equals(satir2.BildirgeIlkKayitTarihi) &&
                                                                satir.OnayDurumu.Equals(satir2.OnayDurumu)
                                                                )
                                                            {
                                                                iptalBulundu = true;
                                                                satir2.Gun = "-1";
                                                                break;
                                                            }
                                                        }
                                                    }

                                                    if (iptalBulundu)
                                                    {
                                                        satir.Gun = "-1";
                                                    }
                                                }
                                            }

                                            var groups = satirlar.GroupBy(p => p.Mahiyet.Equals("ASIL") || p.Mahiyet.Equals("EK")).ToDictionary(x => x.Key, x => x.ToList());

                                            foreach (var kvGroup in groups)
                                            {
                                                var verilebilecekGunSayisi = gecerliBasvuruKayitlari.Sum(p => p.OrtalamaGunSayisi.ToInt());

                                                bool asilVeyaEk = kvGroup.Key;

                                                var grupsatirlari = kvGroup.Value.OrderByDescending(p => tesvik7252.BelgeTuruOranBul(yil, ay, p.BelgeTuru, isyeri.IsyeriSicilNo));

                                                for (int i = 0; i < grupsatirlari.Count(); i++)
                                                {
                                                    var grupSatir = grupsatirlari.ElementAt(i);

                                                    if (grupSatir.Gun != "-1")
                                                    {
                                                        if (verilebilecekGunSayisi < grupSatir.Gun.ToInt())
                                                        {
                                                            grupSatir.Gun = verilebilecekGunSayisi.ToString();
                                                        }
                                                        else
                                                        {
                                                            verilebilecekGunSayisi -= grupSatir.Gun.ToInt();
                                                        }
                                                    }
                                                    else grupSatir.Gun = "0";
                                                }

                                                if (!asilVeyaEk)
                                                {
                                                    if (olasiIptaller7252.ContainsKey(yilAy))
                                                    {
                                                        var olasiIptalSatiri= olasiIptaller7252[yilAy].FirstOrDefault();

                                                        olasiIptalSatiri.Gun = verilebilecekGunSayisi.ToString();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        decimal CarpimOrani687 = -1;

                        var kisiler7166 = new List<BasvuruKisiDownload7103>();

                        var kisiler7166Cekildi = false;

                        foreach (var item in gruplar)
                        {
                            var grupbildirgeler = item.Value;

                            var iptalKisiler = grupbildirgeler
                                    .Where(p => p.Mahiyet.Equals("IPTAL"))
                                    .SelectMany(p => p.Kisiler)
                                    .GroupBy(p => p.SosyalGuvenlikNo)
                                    .ToDictionary(x => x.Key, x => x.ToList());

                            foreach (var bildirge in grupbildirgeler)
                            {
                                if (bildirge.Mahiyet.Equals("ASIL") || bildirge.Mahiyet.Equals("EK"))
                                {
                                    if (bildirge.Kanun.EndsWith("687") && CarpimOrani687 == -1)
                                    {
                                        CarpimOrani687 = CarpimOraniBul687(projeGiris);
                                    }

                                    var bildirgeTarih = new DateTime(bildirge.Yil.ToInt(), bildirge.Ay.ToInt(), 1);

                                    if (bildirge.Kanun.EndsWith("7103"))
                                    {
                                        if (bildirgeTarih >= Program.TumTesvikler["7166"].TesvikBaslamaZamani && bildirgeTarih < Program.TumTesvikler["7166"].TesvikBaslamaZamani.AddMonths(5))
                                        {
                                            if (!kisiler7166Cekildi)
                                            {
                                                var basvuruFormu = Metodlar.FormBul(isyeri, Enums.FormTuru.BasvuruFormu);

                                                if (basvuruFormu != null)
                                                {
                                                    var ds = Metodlar.BasvuruListesiniYukle(basvuruFormu);

                                                    if (ds.Tables.Contains("7103"))
                                                    {
                                                        var dt = ds.Tables["7103"];

                                                        if (dt.Columns.Contains("Prim ve Ücret Desteği İçin İlave Olunacak Sayı"))
                                                        {
                                                            kisiler7166 = dt.AsEnumerable().Where(row => row[(int)Sabitler.BasvuruFormlariSutunlari["7166"][Enums.BasvuruFormuSutunTurleri.UcretDestegiTercihi7103]].ToString().Equals("İSTİYOR")).Select(row => new BasvuruKisiDownload7103
                                                            {
                                                                TcKimlikNo = row[(int)Sabitler.BasvuruFormlariSutunlari["7166"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString(),
                                                                Sicil = row[(int)Sabitler.BasvuruFormlariSutunlari["7166"][Enums.BasvuruFormuSutunTurleri.Sicil]].ToString(),
                                                                PrimveUcretDestegiIcinBaslangicDonemi = row[(int)Sabitler.BasvuruFormlariSutunlari["7166"][Enums.BasvuruFormuSutunTurleri.PrimveUcretDestegiIcinBaslangicDonemi]].ToString(),
                                                                PrimveUcretDestegiIcinBitisDonemi = row[(int)Sabitler.BasvuruFormlariSutunlari["7166"][Enums.BasvuruFormuSutunTurleri.PrimveUcretDestegiIcinBitisDonemi]].ToString(),
                                                                PrimveUcretDestegiIcinIlaveOlunacakSayi = row[(int)Sabitler.BasvuruFormlariSutunlari["7166"][Enums.BasvuruFormuSutunTurleri.PrimveUcretDestegiIcinIlaveOlunacakSayi]].ToString(),
                                                            }).ToList();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    var response = Metodlar.Sistemden7103ListesiniCek(isyeri);

                                                    if (!string.IsNullOrEmpty(response.HataMesaji))
                                                    {
                                                        MessageBox.Show("Sistemden 7103 listesi çekilemedi. 7103 bildirgesindeki kişilerin 7166 hakedip etmediğine bakılmaksızın hepsine 7103 verilmiş varsayılarak icmal oluşturulacak" + Environment.NewLine + Environment.NewLine + "Hata Mesajı:" + response.HataMesaji, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                    }
                                                    else
                                                    {
                                                        if (response.Result != null)
                                                        {
                                                            kisiler7166 = response.Result.Where(p => p.UcretDestegiTercihi.Equals("İSTİYOR")).ToList();
                                                        }
                                                    }
                                                }

                                                kisiler7166Cekildi = true;
                                            }

                                            var bulunanKisiler = bildirge.Kisiler.Where(p => kisiler7166.Any(x =>
                                                                                                            (p.SosyalGuvenlikNo.Length == 11 ? p.SosyalGuvenlikNo.Equals(x.TcKimlikNo) : p.SosyalGuvenlikNo.Equals(x.Sicil))
                                                                                                            &&
                                                                                                            bildirgeTarih >= Convert.ToDateTime(x.PrimveUcretDestegiIcinBaslangicDonemi)
                                                                                                            &&
                                                                                                            bildirgeTarih <= Convert.ToDateTime(x.PrimveUcretDestegiIcinBitisDonemi)
                                                                                                            )

                                            );

                                            if (bulunanKisiler.Count() > 0)
                                            {
                                                var icmal7166 = new BildirgeYuklemeIcmal
                                                {
                                                    Kanun = "7166",
                                                    Matrah = 0,
                                                    PrimOdenenGunSayisi = 0,
                                                    Tutar = 0,
                                                    yilay = new KeyValuePair<string, string>(bildirge.Yil, bildirge.Ay),
                                                };

                                                foreach (var bulunanKisi in bulunanKisiler)
                                                {
                                                    icmal7166.Tutar += Metodlar.TesvikTutariHesapla("07166", bulunanKisi.Gun.ToInt(), bulunanKisi.Ucret.ToDecimalSgk() + bulunanKisi.Ikramiye.ToDecimalSgk(), bildirge.Yil.ToInt(), bildirge.Ay.ToInt(), bildirge.BelgeTuru, isyeri.IsyeriSicilNo, null, CarpimOrani687);
                                                    icmal7166.PrimOdenenGunSayisi += bulunanKisi.Gun.ToInt();
                                                }

                                                if (bildirge.Askida)
                                                {
                                                    onaysiztesvikIcmalleri["7166"].Add(icmal7166);
                                                }
                                                else onaylitesvikIcmalleri["7166"].Add(icmal7166);

                                                onayliveOnaysiztesvikIcmalleri["7166"].Add(icmal7166);
                                            }

                                        }
                                    }

                                    var tesvik = Program.TumTesvikler.FirstOrDefault(p => p.Value.Kanun.PadLeft(5, '0').Equals(bildirge.Kanun) || p.Value.AltKanunlar.Contains(bildirge.Kanun));

                                    var tesvikTutari = 0m;

                                    if (bildirge.Kanun.EndsWith("7252"))
                                    {
                                        foreach (var kisi in bildirge.Kisiler)
                                        {
                                            if (kisiGunler7252.ContainsKey(kisi.SosyalGuvenlikNo))
                                            {
                                                if (kisiGunler7252[kisi.SosyalGuvenlikNo].ContainsKey(bildirge.Yil + "-" + bildirge.Ay))
                                                {
                                                    var bildirgeSatirlari = kisiGunler7252[kisi.SosyalGuvenlikNo][bildirge.Yil + "-" + bildirge.Ay].Where(p => p.BildirgeRefNo == bildirge.RefNo);

                                                    foreach (var bildirgeSatir in bildirgeSatirlari)
                                                    {
                                                        tesvikTutari += Metodlar.TesvikTutariHesapla(bildirge.Kanun, bildirgeSatir.Gun.ToInt(), bildirgeSatir.Ucret.ToDecimalSgk() + bildirgeSatir.Ikramiye.ToDecimalSgk(), bildirge.Yil.ToInt(), bildirge.Ay.ToInt(), bildirge.BelgeTuru, isyeri.IsyeriSicilNo, null, CarpimOrani687);
                                                    }
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        foreach (var kisi in bildirge.Kisiler)
                                        {
                                            tesvikTutari += Metodlar.TesvikTutariHesapla(bildirge.Kanun, kisi.Gun.ToInt(), kisi.Ucret.ToDecimalSgk() + kisi.Ikramiye.ToDecimalSgk(), bildirge.Yil.ToInt(), bildirge.Ay.ToInt(), bildirge.BelgeTuru, isyeri.IsyeriSicilNo, null, CarpimOrani687);
                                        }
                                    }

                                    var icmal = new BildirgeYuklemeIcmal
                                    {
                                        Kanun = bildirge.Kanun,
                                        Matrah = bildirge.ToplamUcret,
                                        PrimOdenenGunSayisi = bildirge.ToplamGun,
                                        Tutar = tesvikTutari,
                                        yilay = new KeyValuePair<string, string>(bildirge.Yil, bildirge.Ay),
                                    };

                                    var bildirgedekiKisilerinIptalleri = new List<AphbSatir>();


                                    foreach (var bildirgeKisi in bildirge.Kisiler)
                                    {
                                        var iptalKarsiligiYok = false;

                                        if (iptalKisiler.ContainsKey(bildirgeKisi.SosyalGuvenlikNo))
                                        {
                                            var iptalSatirlar = iptalKisiler[bildirgeKisi.SosyalGuvenlikNo];

                                            var iptalsatir = iptalSatirlar.Where(p => bildirgeKisi.SosyalGuvenlikNo.Equals(p.SosyalGuvenlikNo) && bildirgeKisi.Gun.Equals(p.Gun) && bildirgeKisi.Ucret.Equals(p.Ucret) && bildirgeKisi.Ikramiye.Equals(p.Ikramiye) && p.BildirgeIlkKayitTarihi == bildirge.ilkKayitTarihi).OrderByDescending(p => p.BildirgeIlkKayitTarihi).FirstOrDefault();

                                            if (iptalsatir != null)
                                            {
                                                if (iptalsatir.Kanun.EndsWith("7252"))
                                                {
                                                    if (kisiGunler7252.ContainsKey(iptalsatir.SosyalGuvenlikNo))
                                                    {
                                                        if (kisiGunler7252[iptalsatir.SosyalGuvenlikNo].ContainsKey(iptalsatir.Yil + "-" + iptalsatir.Ay))
                                                        {
                                                            var bulunanIptalSatir = kisiGunler7252[iptalsatir.SosyalGuvenlikNo][iptalsatir.Yil + "-" + iptalsatir.Ay]
                                                                .Where(p => p.Mahiyet.EndsWith("PTAL") && p.BelgeTuru.Equals(bildirgeKisi.BelgeTuru) && bildirgeKisi.SosyalGuvenlikNo.Equals(p.SosyalGuvenlikNo) && p.BildirgeIlkKayitTarihi == bildirge.ilkKayitTarihi).OrderByDescending(p => p.BildirgeIlkKayitTarihi).FirstOrDefault();

                                                            if (bulunanIptalSatir != null)
                                                            {
                                                                if (iptalsatir.Gun.ToInt() > 0)
                                                                {
                                                                    iptalsatir.Ucret = (iptalsatir.Ucret.ToDecimalSgk() / iptalsatir.Gun.ToInt() * bulunanIptalSatir.Gun.ToInt()).ToString();
                                                                    iptalsatir.Ikramiye = (iptalsatir.Ikramiye.ToDecimalSgk() / iptalsatir.Gun.ToInt() * bulunanIptalSatir.Gun.ToInt()).ToString();
                                                                }

                                                                iptalsatir.Gun = bulunanIptalSatir.Gun;
                                                            }

                                                            bildirgedekiKisilerinIptalleri.Add(iptalsatir);

                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    bildirgedekiKisilerinIptalleri.Add(iptalsatir);
                                                }
                                            }
                                            else
                                            {
                                                iptalKarsiligiYok = true;
                                            }
                                        }
                                        else
                                        {
                                            iptalKarsiligiYok = true;
                                        }

                                        if (iptalKarsiligiYok)
                                        {
                                            var olasiIptaller = hataVerenBildirgeler.Where(p => p.Key.Yil.Equals(bildirge.Yil) && p.Key.Ay.Equals(bildirge.Ay) && p.Key.BelgeTuru.Equals(bildirge.BelgeTuru) && p.Key.Mahiyet.EndsWith("PTAL") && p.Key.Askida == bildirge.Askida);

                                            if (olasiIptaller.Count() == 1)
                                            {
                                                var olasiIptal = olasiIptaller.FirstOrDefault();

                                                if (olasiIptal.Key.Kanun.EndsWith("7252"))
                                                {
                                                    if (olasiIptaller7252.ContainsKey(olasiIptal.Key.Yil + "-" + olasiIptal.Key.Ay))
                                                    {

                                                        var olasiIptalSatir = olasiIptaller7252[olasiIptal.Key.Yil + "-" + olasiIptal.Key.Ay].FirstOrDefault();

                                                        bildirgedekiKisilerinIptalleri.Add(new AphbSatir
                                                        {
                                                            Kanun = olasiIptal.Key.Kanun,
                                                            Gun = olasiIptalSatir.Gun,
                                                            Ucret = bildirgeKisi.Gun.ToInt() > 0 ?  (bildirgeKisi.Ucret.ToDecimalSgk() / bildirgeKisi.Gun.ToInt() * olasiIptalSatir.Gun.ToInt()).ToString() : "0" ,
                                                            Ikramiye = bildirgeKisi.Gun.ToInt() > 0 ?  (bildirgeKisi.Ikramiye.ToDecimalSgk() / bildirgeKisi.Gun.ToInt() * olasiIptalSatir.Gun.ToInt()).ToString() : "0" ,
                                                            BelgeTuru = bildirge.BelgeTuru

                                                        });
                                                    }
                                                }
                                                else
                                                {

                                                    bildirgedekiKisilerinIptalleri.Add(new AphbSatir
                                                    {
                                                        Kanun = olasiIptaller.First().Key.Kanun,
                                                        Gun = bildirgeKisi.Gun,
                                                        Ucret = bildirgeKisi.Ucret,
                                                        Ikramiye = bildirgeKisi.Ikramiye,
                                                        BelgeTuru = bildirge.BelgeTuru

                                                    });
                                                }

                                                icmal.IptalVarsayimIleBulundu = true;
                                            }
                                            else
                                                icmal.IptaliBulunamayanVar = true;
                                        }
                                    }

                                    foreach (var iptalKisi in bildirgedekiKisilerinIptalleri)
                                    {
                                        if (iptalKisi.Kanun.EndsWith("687") && CarpimOrani687 == -1)
                                        {
                                            CarpimOrani687 = CarpimOraniBul687(projeGiris);
                                        }

                                        //var tesvik = Program.TumTesvikler[kisiSatir.TesvikKanunNo];
                                        var DonusturulenKanun = iptalKisi.Kanun;
                                        var kanunGun = Convert.ToInt32(iptalKisi.Gun);
                                        var kanunUcret = iptalKisi.Ucret.ToDecimalSgk() + iptalKisi.Ikramiye.ToDecimalSgk();

                                        var dk = tesvik.Value.DonusturulecekKanunlar.FirstOrDefault(p => p.Key.Equals(DonusturulenKanun)).Value;

                                        if (dk != null)
                                        {
                                            if (TesvikHesaplamaSabitleri.BagliKanunlarIcmalHesaplama.ContainsKey(bildirge.Kanun) && TesvikHesaplamaSabitleri.BagliKanunlarIcmalHesaplama[bildirge.Kanun].Contains("05510"))
                                            {
                                                if (DonusturulenKanun.Equals("00000"))
                                                {
                                                    icmal.Tutar += kanunGun * Metodlar.AsgariUcretBul(bildirge.Yil.ToInt(), bildirge.Ay.ToInt()) * 0.05m;
                                                }
                                                else icmal.Tutar += Metodlar.TesvikTutariHesapla("05510", kanunGun, kanunUcret, bildirge.Yil.ToInt(), bildirge.Ay.ToInt(), bildirge.BelgeTuru, isyeri.IsyeriSicilNo);
                                            }

                                            var dusulecekTutar = DonusturulecekKanun.DusulecekMiktarHesapla(DonusturulenKanun, kanunGun, kanunUcret, bildirge.Yil.ToInt(), bildirge.Ay.ToInt(), bildirge.BelgeTuru, isyeri.IsyeriSicilNo, tesvik.Value.DonusenlerIcmaldenDusulsun, null, CarpimOrani687)[DonusturulenKanun].BagliKanunlarDahilDusulecekTutar;

                                            icmal.Tutar -= dusulecekTutar;
                                        }
                                    }

                                    if (bildirge.Askida)
                                    {
                                        onaysiztesvikIcmalleri[tesvik.Key].Add(icmal);
                                    }
                                    else onaylitesvikIcmalleri[tesvik.Key].Add(icmal);

                                    onayliveOnaysiztesvikIcmalleri[tesvik.Key].Add(icmal);
                                }
                            }
                        }
                    }
                    else
                    {
                        sayac++;

                        if (sayac < 5)
                        {
                            Thread.Sleep(500);
                            goto tekrarDene2;
                        }
                        else throw new Exception("5 denemeye rağmen bilgiler çekilemedi");
                    }
                }
                else
                {
                    sayac++;

                    if (sayac < 5)
                    {

                        Thread.Sleep(500);
                        goto tekrarDene;
                    }
                    else throw new Exception("5 denemeye rağmen bilgiler çekilemedi");
                }

                if (!onayliveOnaysiztesvikIcmalleri.Any(p => p.Value.Count > 0)) throw new Exception("Kayıt bulunmadı");

                var kaydetmeSonucu = Metodlar.BildirgelerGenelIcmalKaydet(isyeri, onaylitesvikIcmalleri, onaysiztesvikIcmalleri, onayliveOnaysiztesvikIcmalleri, hataVerenBildirgeler);

                result.Durum = kaydetmeSonucu.Durum;
                result.HataMesaji = kaydetmeSonucu.HataMesaji;
                result.Result = kaydetmeSonucu.Result;

                result.Onaylilar = onaylitesvikIcmalleri;
                result.Onaysizlar = onaysiztesvikIcmalleri;
                result.Tumu = onayliveOnaysiztesvikIcmalleri;
                result.HataliBildirgeler = hataVerenBildirgeler;

                return result;

            }
            catch (Exception ex)
            {
                result.HataMesaji = ex.Message;
                result.Durum = false;
            }
            finally
            {
                if (yeniWebClientOlustur)
                    projeGiris.Disconnect();
            }

            if (yeniWebClientOlustur) projeGiris.Disconnect();

            return result;

        }

        public static string KisileriCek(ref Bildirge bildirge, ProjeGiris webclient, string token)
        {
            var sayac = 0;

        TekrarDene:

            var data = webclient.DownloadFilePost("https://ebildirge.sgk.gov.tr/EBildirgeV2/subeOnay/subeOnaybildirgeAphbOlustur.action", String.Format("struts.token.name=token&token={0}&kayitIds={1}", token, bildirge.RefNo));

            var sonuc = System.Text.Encoding.UTF8.GetString(data);

            if (sonuc.Contains("Hata referans numarası")) return "Bildirge Hatalı";

            if (sonuc.StartsWith("%PDF"))
            {
                var reader = new PdfReader(data);

                var toplamSigortaliSayisi = 0;

                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    var ilkTcNo = "";
                    var ilkUcret = "";

                    var kaymaMiktari = 0f;

                    if (page == 1)
                    {
                        do
                        {
                            iTextSharp.text.Rectangle rectIlkSgkNo = new iTextSharp.text.Rectangle(310f + kaymaMiktari, 50f, 300f + kaymaMiktari, 100f);
                            RenderFilter[] filterIlkSgkNo = { new RegionTextRenderFilter(rectIlkSgkNo) };
                            ITextExtractionStrategy strategyIlkSgkNo = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterIlkSgkNo);
                            ilkTcNo = PdfTextExtractor.GetTextFromPage(reader, page, strategyIlkSgkNo);

                            if (!long.TryParse(ilkTcNo, out long t))
                            {
                                kaymaMiktari += 10f;
                            }

                        } while (!long.TryParse(ilkTcNo, out long t2));
                    }
                    else
                    {
                        if (toplamSigortaliSayisi == bildirge.Kisiler.Count)
                        {
                            break;
                        }

                        do
                        {
                            iTextSharp.text.Rectangle rectIlkSgkNo = new iTextSharp.text.Rectangle(170f + kaymaMiktari, 50f, 160f + kaymaMiktari, 100f);
                            RenderFilter[] filterIlkSgkNo = { new RegionTextRenderFilter(rectIlkSgkNo) };
                            ITextExtractionStrategy strategyIlkSgkNo = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterIlkSgkNo);
                            ilkTcNo = PdfTextExtractor.GetTextFromPage(reader, page, strategyIlkSgkNo);

                            bool gecersiz = false;

                            if (!long.TryParse(ilkTcNo, out long t))
                            {
                                gecersiz = true;
                            }

                            if (!gecersiz)
                            {
                                iTextSharp.text.Rectangle rectIlkUcret = new iTextSharp.text.Rectangle(170f + kaymaMiktari, 420f, 160f + kaymaMiktari, 520f);
                                RenderFilter[] filterIlkUcret = { new RegionTextRenderFilter(rectIlkUcret) };
                                ITextExtractionStrategy strategyIlkUcret = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterIlkUcret);
                                ilkUcret = PdfTextExtractor.GetTextFromPage(reader, page, strategyIlkUcret);

                                if (!decimal.TryParse(ilkUcret.Replace("TL", "").Trim(), out decimal t3))
                                {
                                    gecersiz = true;
                                }
                            }

                            if (gecersiz) kaymaMiktari += 5f;

                        } while (!long.TryParse(ilkTcNo, out long t2) && !decimal.TryParse(ilkUcret, out decimal t4));
                    }

                    var altsinirDegeri = "";
                    var altsinir = page == 1 ? 410f : 450f;
                    var sayac2 = 0;
                    do
                    {
                        sayac2++;

                        iTextSharp.text.Rectangle rectAltSinir = new iTextSharp.text.Rectangle(altsinir + 10 + kaymaMiktari, 360f, altsinir + kaymaMiktari, 380f);
                        RenderFilter[] filterAltSinir = { new RegionTextRenderFilter(rectAltSinir) };
                        ITextExtractionStrategy strategyAltSinir = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterAltSinir);
                        altsinirDegeri = PdfTextExtractor.GetTextFromPage(reader, page, strategyAltSinir);
                        if (!altsinirDegeri.StartsWith("Prim Ödeme"))
                        {
                            altsinir += 10f;
                        }

                    } while (!altsinirDegeri.StartsWith("Prim Ödeme"));



                    iTextSharp.text.Rectangle rectToplamSayi = new iTextSharp.text.Rectangle(180f + kaymaMiktari, 200f, 170f + kaymaMiktari, 250f); //Prim Ödeme Gün Sayısı
                    RenderFilter[] filterToplamSayi = { new RegionTextRenderFilter(rectToplamSayi) };
                    ITextExtractionStrategy strategyToplamSayi = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterToplamSayi);
                    string currentTextToplamSayi = PdfTextExtractor.GetTextFromPage(reader, page, strategyToplamSayi);

                    iTextSharp.text.Rectangle rectToplamGun = new iTextSharp.text.Rectangle(190f + kaymaMiktari, 200f, 180f + kaymaMiktari, 250f); //Prim Ödeme Gün Sayısı
                    RenderFilter[] filterToplamGun = { new RegionTextRenderFilter(rectToplamGun) };
                    ITextExtractionStrategy strategyToplamGun = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterToplamGun);
                    string currentTextToplamgun = PdfTextExtractor.GetTextFromPage(reader, page, strategyToplamGun);

                    iTextSharp.text.Rectangle rectToplamUcret = new iTextSharp.text.Rectangle(250f + kaymaMiktari, 360f, 240f + kaymaMiktari, 580f); // Ücretler toplamı
                    RenderFilter[] filterToplamUcret = { new RegionTextRenderFilter(rectToplamUcret) };
                    ITextExtractionStrategy strategyToplamUcret = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterToplamUcret);
                    string currentTextToplamUcret = PdfTextExtractor.GetTextFromPage(reader, page, strategyToplamUcret);

                    iTextSharp.text.Rectangle rectSgkNo = page == 1 ? new iTextSharp.text.Rectangle(altsinir + kaymaMiktari, 50f, 300f + kaymaMiktari, 100f) : new iTextSharp.text.Rectangle(altsinir + kaymaMiktari, 50f, 160f + kaymaMiktari, 100f);
                    RenderFilter[] filterSgkNo = { new RegionTextRenderFilter(rectSgkNo) };
                    ITextExtractionStrategy strategySgkNo = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterSgkNo);
                    string currentTextSgkNo = PdfTextExtractor.GetTextFromPage(reader, page, strategySgkNo);

                    iTextSharp.text.Rectangle rectGunler = page == 1 ? new iTextSharp.text.Rectangle(altsinir + kaymaMiktari, 360f, 300f + kaymaMiktari, 380f) : new iTextSharp.text.Rectangle(altsinir + kaymaMiktari, 367f, 160f + kaymaMiktari, 380f);
                    RenderFilter[] filterGunler = { new RegionTextRenderFilter(rectGunler) };
                    ITextExtractionStrategy strategyGunler = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterGunler);
                    string currentTextGunler = PdfTextExtractor.GetTextFromPage(reader, page, strategyGunler);

                    iTextSharp.text.Rectangle rectUcret = page == 1 ? new iTextSharp.text.Rectangle(altsinir + kaymaMiktari, 420f, 300f + kaymaMiktari, 520f) : new iTextSharp.text.Rectangle(altsinir + kaymaMiktari, 420f, 160f + kaymaMiktari, 520f);
                    RenderFilter[] filterUcret = { new RegionTextRenderFilter(rectUcret) };
                    ITextExtractionStrategy strategyUcret = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterUcret);
                    string currentTextUcret = PdfTextExtractor.GetTextFromPage(reader, page, strategyUcret);

                    iTextSharp.text.Rectangle rectIkramiye = page == 1 ? new iTextSharp.text.Rectangle(altsinir + kaymaMiktari, 520f, 300f + kaymaMiktari, 580f) : new iTextSharp.text.Rectangle(altsinir + kaymaMiktari, 520f, 160f + kaymaMiktari, 580f);
                    RenderFilter[] filterIkramiye = { new RegionTextRenderFilter(rectIkramiye) };
                    ITextExtractionStrategy strategyIkramiye = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterIkramiye);
                    string currentTextIkramiye = PdfTextExtractor.GetTextFromPage(reader, page, strategyIkramiye);


                    string[] sgnos = currentTextSgkNo.Split('\n');
                    string[] gunler = currentTextGunler.Split('\n');
                    string[] ucretler = currentTextUcret.Split('\n');
                    string[] ikramiyeler = currentTextIkramiye.Split('\n');

                    try
                    {
                        List<string[]> bilgiler = new List<string[]> { sgnos, gunler, ucretler, ikramiyeler };

                        foreach (var item in sgnos)
                        {
                            Convert.ToInt64(item);
                        }

                        foreach (var item in gunler)
                        {
                            item.Trim().ToInt();
                        }

                        foreach (var item in ucretler)
                        {
                            item.Trim().Replace("TL", "").Trim().ToDecimalSgk();
                        }

                        foreach (var item in ikramiyeler)
                        {
                            item.Trim().Replace("TL", "").Trim().ToDecimalSgk();
                        }

                        if (bilgiler.Select(p => p.Length).Distinct().Count() > 1)
                        {
                            throw new Exception("Bildirgeden çekilen bilgilerde tutarsızlık var");
                        }

                        if (page == 1)
                        {
                            toplamSigortaliSayisi = currentTextToplamSayi.Trim().ToInt();
                            bildirge.ToplamGun = currentTextToplamgun.Trim().ToInt();
                            bildirge.ToplamUcret = currentTextToplamUcret.Replace("TL", "").Trim().ToDecimalSgk();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("YIL: {0} , AY: {1} , KANUN: {2} , BELGE: {3} , MAHİYET: {4} {5} bildirgenin okunmasında hata var", bildirge.Yil, bildirge.Ay, bildirge.Kanun, bildirge.BelgeTuru, bildirge.Mahiyet, bildirge.Askida ? "ONAYSIZ" : "ONAYLI"), "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        throw ex;
                    }


                    for (int i = 0; i < sgnos.Length; i++)
                    {
                        bildirge.Kisiler.Add(new AphbSatir
                        {
                            SosyalGuvenlikNo = sgnos[i].Trim(),
                            Gun = gunler[i].Trim(),
                            Ucret = ucretler[i].Replace("TL", "").Trim(),
                            Ikramiye = ikramiyeler[i].Replace("TL", "").Trim(),
                            Kanun = bildirge.Kanun,
                            BelgeTuru = bildirge.BelgeTuru,
                            BildirgeIlkKayitTarihi = bildirge.ilkKayitTarihi
                        });
                    }

                    if (page == 1)
                    {
                        bildirge.ToplamGun = currentTextToplamgun.Trim().ToInt();
                        bildirge.ToplamUcret = currentTextToplamUcret.Replace("TL", "").Trim().ToDecimalSgk();
                    }
                }

                if (toplamSigortaliSayisi != bildirge.Kisiler.Count)
                {
                    MessageBox.Show(String.Format("YIL: {0} , AY: {1} , KANUN: {2} , BELGE: {3} , MAHİYET: {4} {5} bildirgenin okunmasında hata var", bildirge.Yil, bildirge.Ay, bildirge.Kanun, bildirge.BelgeTuru, bildirge.Mahiyet, bildirge.Askida ? "ONAYSIZ" : "ONAYLI"), "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    throw new Exception("Bildirge okunmasında hata var");

                }

                reader.Close();

                return "OK";
            }
            else
            {
                sayac++;

                if (sayac < 3)
                {
                    Thread.Sleep(1000);
                    goto TekrarDene;
                }
                else return "3 denemeye rağmen bildirge çekilemedi";
            }
        }


    }



}
