using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static string CariDonemKisileriAPHByeEkle(Isyerleri isyeri, ref DataTable dtAphb, out DataTable dtCariAphb)
        {
            var simdikiAy = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var oncekiAy = simdikiAy.AddMonths(-1);

            var rows = dtAphb.AsEnumerable();

            var cariAy = DateTime.MinValue;

            dtCariAphb = dtAphb.Clone();

            var tumaylar = rows.Select(row => row[(int)Enums.AphbHucreBilgileri.Yil].ToString() + "/" + row[(int)Enums.AphbHucreBilgileri.Ay].ToString());

            var aySatirlari = rows.GroupBy(row => row[(int)Enums.AphbHucreBilgileri.Yil].ToString() + "-" + row[(int)Enums.AphbHucreBilgileri.Ay].ToString()).ToDictionary(x => x.Key, x => x.ToList());

            //string Kanun6486 = null;

            if (tumaylar.Count() > 0)
            {
                var enbuyukay = tumaylar.Max(x => Convert.ToDateTime(x));

                //var enbuyukAySatirlari = aySatirlari[enbuyukay.Year.ToString() + "-" + enbuyukay.Month.ToString()];

                //var row6486 = enbuyukAySatirlari.FirstOrDefault(row => row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().EndsWith("6486"));

                //if (row6486 != null)
                //{
                //    Kanun6486 = row6486[(int)Enums.AphbHucreBilgileri.Kanun].ToString();
                //}

                cariAy = enbuyukay.AddMonths(1);
            }

            if (cariAy == DateTime.MinValue) cariAy = oncekiAy;

            var sonrakiAy = cariAy.AddMonths(1);

            Classes.CariKisiler cariKisiler = new Classes.CariKisiler();

            var cariKisilerDosyaYolu = Metodlar.FormBul(isyeri, Enums.FormTuru.Kisiler);

            Dictionary<string, List<Classes.IseGirisCikisKaydi>> kisilerGirisCikisKayitlari = new Dictionary<string, List<Classes.IseGirisCikisKaydi>>();


            #region En Son Onaylı Aphbden Kişilerin Çekilmesi

            var TumAySatirlari = rows.GroupBy(row => row[(int)Enums.AphbHucreBilgileri.Yil].ToString() + "-" + row[(int)Enums.AphbHucreBilgileri.Ay].ToString()).ToDictionary(x => x.Key, x => x.ToList());

            var TumAylar = TumAySatirlari.Keys.Distinct().ToList();

            var enbuyukAy = TumAylar.Max(p => Convert.ToDateTime(p));

            var kisiSatirlari = rows.GroupBy(x => x[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString().Trim());

            var KisilerinSatirlari = kisiSatirlari.ToDictionary(x => x.Key, x => x.GroupBy(row => row[(int)Enums.AphbHucreBilgileri.Yil].ToString() + "-" + row[(int)Enums.AphbHucreBilgileri.Ay].ToString()).ToDictionary(p => p.Key, p => Metodlar.GecerliSatirlariGetir(p.ToList(), true)));

            var birOncekiAy = cariAy.AddMonths(-1);

            var tarihKey = birOncekiAy.Year.ToString() + "-" + birOncekiAy.Month.ToString();

            bool devam = true;

            if (!TumAySatirlari.ContainsKey(tarihKey))
            {
                devam = MessageBox.Show(String.Format("{0} yılı {1}. ayının onaylı bildirgeleri çekilmemiş. Onaylı bildirgeler indirilmeden sadece işe giriş çıkış kayıtlarına bakılarak Cari Aphb oluşturulsun mu?", birOncekiAy.Year, birOncekiAy.Month), "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            }

            var AphbdeAyIcindeCikanKisiler = new List<string>();

            if (devam)
            {
                if (TumAySatirlari.ContainsKey(tarihKey))
                {
                    var gruplar = TumAySatirlari[tarihKey]
                        .GroupBy(row => row[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString())
                        .ToDictionary(x => x.Key,
                                      x => x.GroupBy(row => row[(int)Enums.AphbHucreBilgileri.Kanun].ToString() + "-" + row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString()+"-"+ row[(int)Enums.AphbHucreBilgileri.Araci].ToString())
                                            .ToDictionary(m => m.Key, m => m.ToList())
                                     );

                    var AyIcindekiTcler = TumAySatirlari[tarihKey]
                         .Select(row => row[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString())
                         .Distinct()
                         .Where(tc => KisilerinSatirlari.ContainsKey(tc)
                                && KisilerinSatirlari[tc].ContainsKey(tarihKey)
                                && KisilerinSatirlari[tc][tarihKey].Count > 0
                                && string.IsNullOrEmpty(KisilerinSatirlari[tc][tarihKey].FirstOrDefault()[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString())
                         );

                    AphbdeAyIcindeCikanKisiler = TumAySatirlari[tarihKey]
                         .Select(row => row[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString())
                         .Distinct()
                         .Where(tc => KisilerinSatirlari.ContainsKey(tc)
                                && KisilerinSatirlari[tc].ContainsKey(tarihKey)
                                && KisilerinSatirlari[tc][tarihKey].Count > 0
                                && string.IsNullOrEmpty(KisilerinSatirlari[tc][tarihKey].FirstOrDefault()[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString()) == false
                         ).ToList();

                    var kisiler = new List<Classes.CariKisi>();

                    foreach (var tc in AyIcindekiTcler)
                    {
                        if (gruplar.ContainsKey(tc))
                        {
                            var values = gruplar[tc];

                            foreach (var item in values)
                            {
                                var splits = item.Key.Split('-');
                                var kanun = splits[0];
                                var belgeTuru = splits[1];

                                if (!string.IsNullOrEmpty(kanun))
                                {
                                    if (new string[] { "02828", "06111", "17103", "27103", "06645" }.Contains(kanun.PadLeft(5, '0')))
                                    {
                                        kanun = "05510";
                                    }
                                }

                                var gun = 0;
                                var ucret = 0m;

                                if (item.Value.Count > 0)
                                {
                                    gun = item.Value.Sum(row => Convert.ToInt32(row[(int)Enums.AphbHucreBilgileri.Gun]));
                                    ucret = item.Value.Sum(row => Convert.ToDecimal(row[(int)Enums.AphbHucreBilgileri.Ucret]) + Convert.ToDecimal(row[(int)Enums.AphbHucreBilgileri.Ikramiye]));

                                }

                                var gunlukOrtalamaUcret = gun == 0 ? Convert.ToDecimal(Metodlar.AsgariUcretBul(cariAy.Year, cariAy.Month)) : Math.Round(ucret / gun, 2);

                                kisiler.Add(new Classes.CariKisi
                                {
                                    TcKimlikNo = tc,
                                    Ad = item.Value.FirstOrDefault()[(int)Enums.AphbHucreBilgileri.Ad].ToString(),
                                    Soyad = item.Value.FirstOrDefault()[(int)Enums.AphbHucreBilgileri.Soyad].ToString(),
                                    Ilk_Soyad = item.Value.FirstOrDefault()[(int)Enums.AphbHucreBilgileri.IlkSoyadi].ToString(),
                                    MeslekKod = item.Value.FirstOrDefault()[(int)Enums.AphbHucreBilgileri.MeslekKod].ToString(),
                                    Kanun = kanun,
                                    BelgeTuru = belgeTuru,
                                    Gun = gun,
                                    GunlukOrtalamaUcret = gunlukOrtalamaUcret,
                                    Araci = item.Value.FirstOrDefault()[(int)Enums.AphbHucreBilgileri.Araci].ToString()
                                });

                            }
                        }
                    }

                    cariKisiler.Kisiler = kisiler;
                }
            }

            #endregion

            if (devam)
            {
                cariKisiler.IseGirisCikisKayitlari.Clear();

                var aracilar = cariKisiler.Kisiler.Select(p => p.Araci).Distinct().ToList();

                var baslangic = cariAy.AddDays(-15);
                var bitis = cariAy.AddMonths(1).AddDays(14);

                List<DateTime> tarihler = new List<DateTime>();

                while (baslangic < bitis)
                {
                    tarihler.Add(baslangic);
                    baslangic = baslangic.AddDays(15);
                }

                foreach (var araci in aracilar)
                {
                    Int32.TryParse(araci.Split('-')[0].Trim(), out int tempTaseronNo);

                    var taseronNo = tempTaseronNo.ToString().PadLeft(3, '0');

                    var sorgulanacakIsyeri = isyeri.TaseronNo.Equals(taseronNo) ? isyeri : Metodlar.TaseronNodanIsyeriBul(isyeri, taseronNo);

                    if (sorgulanacakIsyeri != null)
                    {
                        var araciAdi = sorgulanacakIsyeri.TaseronNo.Equals("000") ? "Ana İşveren" : sorgulanacakIsyeri.TaseronNo + "-" + sorgulanacakIsyeri.Sirketler.SirketAdi;

                        var projeGiris = new Classes.ProjeGiris(sorgulanacakIsyeri, Enums.ProjeTurleri.SigortaliIstenAyrilis);

                        var girisCevabi = string.Empty;

                        var denemeSayisi = 0;

                        do
                        {
                            girisCevabi = projeGiris.Connect();

                            if (girisCevabi.Equals("Kullanıcı adı veya şifreleriniz hatalıdır")
                            || girisCevabi.Equals("5 denemeye rağmen vergi kimlik numarası doğrulaması gerçekleştirilemedi")
                            || girisCevabi.Equals("İşyeri Kanun Kapsamından Çıkmıştır")
                            || girisCevabi.Equals("Is Yeri Iz Olmus")
                            || girisCevabi.Equals("işyeri hesabı PASİF olduğu için sisteme giriş yapamadı")
                            || girisCevabi.Equals("Vekalet Süresi Dolmuştur")
                             )
                            {
                                return String.Format("{0}-{1} işyeri için Sigortalı İşe Giriş ve İşten Ayrılış projesine bağlanılamadığından cari dönem için giriş çıkış bilgileri çekilemedi. Cari ay hesaplanmadan devam edilsin mi?" + Environment.NewLine + Environment.NewLine + "Giriş Hatası: " + girisCevabi, sorgulanacakIsyeri.Sirketler.SirketAdi, sorgulanacakIsyeri.SubeAdi);
                            }

                            denemeSayisi++;
                        }
                        while (!girisCevabi.Equals("OK") && denemeSayisi < 3);

                        if (girisCevabi.Equals("OK"))
                        {
                            foreach (var sorgulanacakTarih in tarihler)
                            {
                                var bas = sorgulanacakTarih;
                                var bit = sorgulanacakTarih.AddDays(14) > bitis ? bitis : sorgulanacakTarih.AddDays(14);

                            AralikSorgula:
                                var response = projeGiris.PostData("https://uyg.sgk.gov.tr/SigortaliTescil/amp/sigortaliTescilAction", String.Format("jobid=hareketListele&tkrVno=&tx_hrktBasTarGG={0}&tx_hrktBasTarAA={1}&tx_hrktBasTarYY={2}&tx_hrktBitTarGG={3}&tx_hrktBitTarAA={4}&tx_hrktBitTarYY={5}", bas.Day.ToString().PadLeft(2, '0'), bas.Month.ToString().PadLeft(2, '0'), bas.Year.ToString(), bit.Day.ToString().PadLeft(2, '0'), bit.Month.ToString().PadLeft(2, '0'), bit.Year.ToString()));

                                if (response.Contains("Mesai saatleri içinde liste alamazsınız"))
                                {
                                    return String.Format("Mesai saati olduğundan dolayı cari dönem için giriş çıkış bilgileri çekilemedi. Cari ay hesaplanmadan devam edilsin mi?");

                                }
                                else if (response.Contains("Listelenecek kayıt bulunamadı"))
                                {
                                    continue;
                                }
                                else if (response.Contains("Kimlik No") && response.Contains("İşlem Tarihi"))
                                {
                                    HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                                    html.LoadHtml(response);

                                    var excellink = html.DocumentNode.Descendants("a").FirstOrDefault(p => p.InnerText != null && p.InnerText.Trim().Equals("Excel"));

                                    var toplam = html.DocumentNode.Descendants("span").FirstOrDefault(p => p.InnerText != null && p.InnerText.Trim().Contains("kayıt bulundu")).InnerText;

                                    var toplamSayi = Convert.ToInt32(Regex.Match(toplam, "(.*) kayıt bulundu.*").Groups[1].Value.Trim().Replace(",", "").Replace(".", "").Replace("Bir", "1"));

                                    var veriler = projeGiris.Get(String.Format("{0}{1}", "https://uyg.sgk.gov.tr", excellink.GetAttributeValue("href", "")), "");

                                    var girisCikislar = veriler.Split('\n').Skip(1).Select(p => p.Split('\t')).Select(p => new Classes.IseGirisCikisKaydi
                                    {
                                        TcKimlikNo = p[0].Trim().Trim('"'),
                                        AdSoyad = Regex.Replace(p[1].Trim().Trim('"'), "\\W+", " ").Trim(),
                                        Turu = p[2].Trim().Trim('"'),
                                        Tarih = Convert.ToDateTime(p[3].Trim().Trim('"')),
                                        IslemTuru = p[6].Trim().Trim('"'),
                                        IslemSaati = Convert.ToDateTime(p[7].Trim().Trim('"') + " " + p[8].Trim().Trim('"')),
                                        Araci = araciAdi
                                    }).ToList();



                                    if (toplamSayi == girisCikislar.Count())
                                    {
                                        girisCikislar.RemoveAll(p => p.Turu.Equals("Giriş") && !Metodlar.SistemdenGirisBul(sorgulanacakIsyeri, p.TcKimlikNo, p.Tarih, ref projeGiris));

                                        cariKisiler.IseGirisCikisKayitlari.AddRange(girisCikislar);
                                    }
                                    else
                                    {
                                        System.Threading.Thread.Sleep(500);
                                        goto AralikSorgula;
                                    }
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(500);
                                    goto AralikSorgula;
                                }
                            }
                            projeGiris.Disconnect();
                        }
                        else return String.Format("{0}-{1} işyeri için Sigortalı İşe Giriş ve İşten Ayrılış projesine 3 denemeye rağmen bağlanılamadığından cari dönem için giriş çıkış bilgileri çekilemedi. Cari ay hesaplanmadan devam edilsin mi?" + Environment.NewLine + Environment.NewLine + "Giriş Hatası: " + girisCevabi, sorgulanacakIsyeri.Sirketler.SirketAdi, sorgulanacakIsyeri.SubeAdi);
                    }
                }

                cariKisiler.SorgulananDonem = cariAy;

                cariKisiler.SorgulamaTarihi = DateTime.Today;

                cariKisiler.IseGirisCikisKayitlari = cariKisiler.IseGirisCikisKayitlari.Where(p => p.Tarih >= cariKisiler.SorgulananDonem && p.Tarih < sonrakiAy).ToList();

                kisilerGirisCikisKayitlari = cariKisiler.IseGirisCikisKayitlari.GroupBy(p => p.TcKimlikNo).ToDictionary(x => x.Key, x => Metodlar.GirisCikisGecerliSatirlariGetir(x.ToList()));

                cariKisiler.Kisiler.RemoveAll(p => p.CikisAyi > DateTime.MinValue && p.CikisAyi < cariKisiler.SorgulananDonem);

                foreach (var kisi in kisilerGirisCikisKayitlari)
                {
                    var sonKayit = kisi.Value.FirstOrDefault();

                    if (sonKayit != null)
                    {

                        if (sonKayit.Turu.Equals("Giriş"))
                        {
                            var ekliKisi = cariKisiler.Kisiler.FirstOrDefault(p => p.TcKimlikNo.Equals(sonKayit.TcKimlikNo));

                            if (ekliKisi == null)
                            {
                                var cariKisi = new Classes.CariKisi
                                {
                                    TcKimlikNo = sonKayit.TcKimlikNo,
                                    BelgeTuru = "1",
                                    Kanun = "05510",
                                    GunlukOrtalamaUcret = Convert.ToDecimal(Metodlar.AsgariUcretBul(cariAy.Year, cariAy.Month)),
                                    Soyad = sonKayit.AdSoyad.Split(' ').Last().Trim(),
                                    Ad = sonKayit.AdSoyad.Replace(sonKayit.AdSoyad.Split(' ').Last(), "").Trim(),
                                    Araci = sonKayit.Araci,
                                    MeslekKod = "0000.00"
                                };

                                cariKisiler.Kisiler.Add(cariKisi);

                            }
                            else
                            {
                                var kisiler = cariKisiler.Kisiler.Where(p => p.TcKimlikNo.Equals(sonKayit.TcKimlikNo));

                                foreach (var item in kisiler)
                                {
                                    item.CikisAyi = DateTime.MinValue;
                                    item.Araci = sonKayit.Araci;
                                }


                            }
                        }
                        else if (sonKayit.Turu.Equals("Çıkış"))
                        {
                            var silinecekKisi = cariKisiler.Kisiler.FirstOrDefault(p => p.TcKimlikNo.Equals(sonKayit.TcKimlikNo));

                            if (silinecekKisi == null)
                            {

                                if ((AphbdeAyIcindeCikanKisiler.Contains(sonKayit.TcKimlikNo) || cariKisiler.Kisiler.Any(p => p.TcKimlikNo.Equals(sonKayit.TcKimlikNo) == false)) && kisi.Value.All(p => p.Turu == "Çıkış"))
                                {

                                }
                                else
                                {
                                    silinecekKisi = new Classes.CariKisi
                                    {
                                        TcKimlikNo = sonKayit.TcKimlikNo,
                                        BelgeTuru = "1",
                                        Kanun = "05510",
                                        GunlukOrtalamaUcret = Convert.ToDecimal(Metodlar.AsgariUcretBul(cariAy.Year, cariAy.Month)),
                                        Soyad = sonKayit.AdSoyad.Split(' ').Last().Trim(),
                                        Ad = sonKayit.AdSoyad.Replace(sonKayit.AdSoyad.Split(' ').Last(), "").Trim(),
                                        Araci = sonKayit.Araci,
                                        MeslekKod = "0000.00"
                                    };

                                    cariKisiler.Kisiler.Add(silinecekKisi);
                                }

                            }

                            var kisiler = cariKisiler.Kisiler.Where(p => p.TcKimlikNo.Equals(sonKayit.TcKimlikNo));

                            foreach (var item in kisiler)
                            {
                                item.CikisAyi = cariAy;
                            }
                        }
                    }

                }

                Metodlar.CariKisileriKaydet(isyeri, cariKisiler);
            }


            var siraNo = 1;

            var kisiTcler = cariKisiler.Kisiler
                                .GroupBy(p => p.TcKimlikNo)
                                .ToDictionary(
                                                x => x.Key,
                                                x => x.OrderBy(p => string.IsNullOrEmpty(p.Kanun) ? 0 : (p.Kanun.Equals("05510") || p.Kanun.EndsWith("6486")) ? 1 : 2)
                                                    .ThenByDescending(p => Convert.ToInt32(p.BelgeTuru))
                                                    .ToList()
                                             );


            foreach (var item in kisiTcler)
            {
                var gun = 30;

                if (!kisilerGirisCikisKayitlari.ContainsKey(item.Key))
                {
                    var eklenenSatirlar = new List<DataRow>();

                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        var kisi = item.Value[i];

                        if (gun == 0) break;

                        if (kisi.CikisAyi > DateTime.MinValue && kisi.CikisAyi < cariAy) break;

                        var alinanGun = 0;

                        if (kisi.Gun > 0)
                        {
                            alinanGun = gun >= kisi.Gun ? kisi.Gun : gun;

                            gun -= alinanGun;
                            kisi.Gun -= alinanGun;

                            var newRow = dtCariAphb.NewRow();
                            newRow[(int)Enums.AphbHucreBilgileri.Yil] = cariAy.Year.ToString();
                            newRow[(int)Enums.AphbHucreBilgileri.Ay] = cariAy.Month.ToString();
                            newRow[(int)Enums.AphbHucreBilgileri.Kanun] = kisi.Kanun;
                            newRow[(int)Enums.AphbHucreBilgileri.BelgeTuru] = kisi.BelgeTuru;
                            newRow[(int)Enums.AphbHucreBilgileri.Mahiyet] = "ASIL";
                            newRow[(int)Enums.AphbHucreBilgileri.SiraNo] = siraNo++;
                            newRow[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo] = kisi.TcKimlikNo;
                            newRow[(int)Enums.AphbHucreBilgileri.Ad] = kisi.Ad;
                            newRow[(int)Enums.AphbHucreBilgileri.Soyad] = kisi.Soyad;
                            newRow[(int)Enums.AphbHucreBilgileri.IlkSoyadi] = kisi.Ilk_Soyad;
                            newRow[(int)Enums.AphbHucreBilgileri.MeslekKod] = kisi.MeslekKod;
                            newRow[(int)Enums.AphbHucreBilgileri.Gun] = alinanGun;
                            newRow[(int)Enums.AphbHucreBilgileri.EksikGun] = 0;
                            newRow[(int)Enums.AphbHucreBilgileri.Ucret] = Math.Round(alinanGun * kisi.GunlukOrtalamaUcret, 2);
                            newRow[(int)Enums.AphbHucreBilgileri.Ikramiye] = 0;
                            newRow[(int)Enums.AphbHucreBilgileri.Araci] = kisi.Araci;

                            dtCariAphb.Rows.Add(newRow);

                            eklenenSatirlar.Add(newRow);

                        }
                    }

                    if (gun > 0)
                    {
                        var kisi = item.Value.LastOrDefault();

                        var oldRow = eklenenSatirlar.FirstOrDefault(row => row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().Equals(kisi.Kanun) &&
                                                                           row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString().Equals(kisi.BelgeTuru));

                        if (oldRow != null)
                        {
                            var yeniGun = Convert.ToInt32(oldRow[(int)Enums.AphbHucreBilgileri.Gun]) + gun;
                            oldRow[(int)Enums.AphbHucreBilgileri.Gun] = yeniGun;
                            oldRow[(int)Enums.AphbHucreBilgileri.Ucret] = Math.Round(yeniGun * kisi.GunlukOrtalamaUcret, 2);
                        }
                        else
                        {

                            var newRow = dtCariAphb.NewRow();
                            newRow[(int)Enums.AphbHucreBilgileri.Yil] = cariAy.Year.ToString();
                            newRow[(int)Enums.AphbHucreBilgileri.Ay] = cariAy.Month.ToString();
                            newRow[(int)Enums.AphbHucreBilgileri.Kanun] = kisi.Kanun;
                            newRow[(int)Enums.AphbHucreBilgileri.BelgeTuru] = kisi.BelgeTuru;
                            newRow[(int)Enums.AphbHucreBilgileri.Mahiyet] = "ASIL";
                            newRow[(int)Enums.AphbHucreBilgileri.SiraNo] = siraNo++;
                            newRow[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo] = kisi.TcKimlikNo;
                            newRow[(int)Enums.AphbHucreBilgileri.Ad] = kisi.Ad;
                            newRow[(int)Enums.AphbHucreBilgileri.Soyad] = kisi.Soyad;
                            newRow[(int)Enums.AphbHucreBilgileri.IlkSoyadi] = kisi.Ilk_Soyad;
                            newRow[(int)Enums.AphbHucreBilgileri.MeslekKod] = kisi.MeslekKod;
                            newRow[(int)Enums.AphbHucreBilgileri.Gun] = gun;
                            newRow[(int)Enums.AphbHucreBilgileri.EksikGun] = 0;
                            newRow[(int)Enums.AphbHucreBilgileri.Ucret] = Math.Round(gun * kisi.GunlukOrtalamaUcret, 2);
                            newRow[(int)Enums.AphbHucreBilgileri.Ikramiye] = 0;
                            newRow[(int)Enums.AphbHucreBilgileri.Araci] = kisi.Araci;

                            dtCariAphb.Rows.Add(newRow);
                        }
                    }
                }
            }

            var AyinSonGunu = sonrakiAy.AddDays(-1).Day;

            foreach (var kisiKayitlari in kisilerGirisCikisKayitlari)
            {

                var tc = kisiKayitlari.Key;

                if (!kisiTcler.ContainsKey(tc)) continue;

                var araciGruplar = kisiKayitlari.Value.GroupBy(p => p.Araci).ToDictionary(x => x.Key, x => x.ToList());

                foreach (var araci in araciGruplar)
                {
                    var eklenenSatirlar = new List<DataRow>();

                    var islemYapilanlar = new HashSet<int>();

                    var i = 0;

                    List<IseGirisCikisCalismaDonemi> gunler = new List<IseGirisCikisCalismaDonemi>();

                    while (i < araci.Value.Count)
                    {
                        if (!islemYapilanlar.Contains(i))
                        {
                            var kayit = araci.Value[i];

                            var gun = 0;
                            var giris = DateTime.MinValue;
                            var cikis = DateTime.MinValue;

                            if (kayit.Turu.Equals("Giriş"))
                            {
                                giris = kayit.Tarih;

                                var sonTarih = AyinSonGunu;

                                if (i > 0)
                                {
                                    var oncekiKayit = araci.Value[i - 1];

                                    if (oncekiKayit.Turu.Equals("Çıkış"))
                                    {
                                        sonTarih = oncekiKayit.Tarih.Day;

                                        cikis = oncekiKayit.Tarih;

                                        islemYapilanlar.Add(i - 1);
                                    }
                                    else sonTarih = kayit.Tarih.Day - 1;
                                }

                                gun = (sonTarih - kayit.Tarih.Day) + 1;

                            }
                            else if (kayit.Turu.Equals("Çıkış"))
                            {
                                cikis = kayit.Tarih;

                                var ilkTarih = cariAy.Day;

                                if (i < araci.Value.Count - 1)
                                {
                                    var sonrakiKayit = araci.Value[i + 1];

                                    if (sonrakiKayit.Turu.Equals("Giriş"))
                                    {
                                        ilkTarih = sonrakiKayit.Tarih.Day;

                                        giris = sonrakiKayit.Tarih;

                                        islemYapilanlar.Add(i + 1);
                                    }
                                    else ilkTarih = kayit.Tarih.Day + 1;
                                }

                                gun = (kayit.Tarih.Day - ilkTarih) + 1;

                            }

                            gun = Math.Min(30, gun);

                            if (gun > 0)
                            {
                                gunler.Add(new IseGirisCikisCalismaDonemi
                                {
                                    Gun = gun,
                                    Giris = giris,
                                    Cikis = cikis,
                                });
                            }

                            islemYapilanlar.Add(i);
                        }

                        i++;
                    }

                    var kisiler = kisiTcler[tc].Where(p => p.Araci.Equals(araci.Key)).ToList();

                    if (kisiler.Count > 0)
                    {
                        gunler = gunler.OrderByDescending(p => p.Gun).ToList();

                        foreach (var item in gunler)
                        {
                            var gun = item.Gun;

                            for (int j = 0; j < kisiler.Count; j++)
                            {
                                var kisi = kisiler[j];

                                if (gun == 0) break;

                                var alinanGun = 0;

                                alinanGun = gun >= kisi.Gun ? kisi.Gun : gun;

                                gun -= alinanGun;
                                kisi.Gun -= alinanGun;

                                var newRow = dtCariAphb.NewRow();
                                newRow[(int)Enums.AphbHucreBilgileri.Yil] = cariAy.Year.ToString();
                                newRow[(int)Enums.AphbHucreBilgileri.Ay] = cariAy.Month.ToString();
                                newRow[(int)Enums.AphbHucreBilgileri.Kanun] = kisi.Kanun;
                                newRow[(int)Enums.AphbHucreBilgileri.BelgeTuru] = kisi.BelgeTuru;
                                newRow[(int)Enums.AphbHucreBilgileri.Mahiyet] = "ASIL";
                                newRow[(int)Enums.AphbHucreBilgileri.SiraNo] = siraNo++;
                                newRow[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo] = kisi.TcKimlikNo;
                                newRow[(int)Enums.AphbHucreBilgileri.Ad] = kisi.Ad;
                                newRow[(int)Enums.AphbHucreBilgileri.Soyad] = kisi.Soyad;
                                newRow[(int)Enums.AphbHucreBilgileri.IlkSoyadi] = kisi.Ilk_Soyad;
                                newRow[(int)Enums.AphbHucreBilgileri.MeslekKod] = kisi.MeslekKod;
                                newRow[(int)Enums.AphbHucreBilgileri.Gun] = alinanGun;
                                newRow[(int)Enums.AphbHucreBilgileri.EksikGun] = 0;
                                newRow[(int)Enums.AphbHucreBilgileri.Ucret] = Math.Round(alinanGun * kisi.GunlukOrtalamaUcret, 2);
                                newRow[(int)Enums.AphbHucreBilgileri.Ikramiye] = 0;
                                newRow[(int)Enums.AphbHucreBilgileri.Araci] = kisi.Araci;

                                if (item.Giris > DateTime.MinValue)
                                {
                                    newRow[(int)Enums.AphbHucreBilgileri.GirisGunu] = item.Giris.Day.ToString().PadLeft(2, '0') + "/" + item.Giris.Month.ToString().PadLeft(2, '0');
                                }

                                if (item.Cikis > DateTime.MinValue)
                                {
                                    newRow[(int)Enums.AphbHucreBilgileri.CikisGunu] = item.Cikis.Day.ToString().PadLeft(2, '0') + "/" + item.Cikis.Month.ToString().PadLeft(2, '0');
                                }

                                dtCariAphb.Rows.Add(newRow);

                                eklenenSatirlar.Add(newRow);

                            }

                            if (gun > 0)
                            {
                                var kisi = kisiler.LastOrDefault();

                                var oldRow = eklenenSatirlar.FirstOrDefault(row => row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().Equals(kisi.Kanun) &&
                                                                           row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString().Equals(kisi.BelgeTuru));

                                if (oldRow != null)
                                {
                                    var yeniGun = Convert.ToInt32(oldRow[(int)Enums.AphbHucreBilgileri.Gun]) + gun;
                                    oldRow[(int)Enums.AphbHucreBilgileri.Gun] = yeniGun;
                                    oldRow[(int)Enums.AphbHucreBilgileri.Ucret] = Math.Round(yeniGun * kisi.GunlukOrtalamaUcret, 2);
                                }
                                else
                                {

                                    var newRow = dtCariAphb.NewRow();
                                    newRow[(int)Enums.AphbHucreBilgileri.Yil] = cariAy.Year.ToString();
                                    newRow[(int)Enums.AphbHucreBilgileri.Ay] = cariAy.Month.ToString();
                                    newRow[(int)Enums.AphbHucreBilgileri.Kanun] = kisi.Kanun;
                                    newRow[(int)Enums.AphbHucreBilgileri.BelgeTuru] = kisi.BelgeTuru;
                                    newRow[(int)Enums.AphbHucreBilgileri.Mahiyet] = "ASIL";
                                    newRow[(int)Enums.AphbHucreBilgileri.SiraNo] = siraNo++;
                                    newRow[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo] = kisi.TcKimlikNo;
                                    newRow[(int)Enums.AphbHucreBilgileri.Ad] = kisi.Ad;
                                    newRow[(int)Enums.AphbHucreBilgileri.Soyad] = kisi.Soyad;
                                    newRow[(int)Enums.AphbHucreBilgileri.IlkSoyadi] = kisi.Ilk_Soyad;
                                    newRow[(int)Enums.AphbHucreBilgileri.MeslekKod] = kisi.MeslekKod;
                                    newRow[(int)Enums.AphbHucreBilgileri.Gun] = gun;
                                    newRow[(int)Enums.AphbHucreBilgileri.EksikGun] = 0;
                                    newRow[(int)Enums.AphbHucreBilgileri.Ucret] = Math.Round(gun * kisi.GunlukOrtalamaUcret, 2);
                                    newRow[(int)Enums.AphbHucreBilgileri.Ikramiye] = 0;
                                    newRow[(int)Enums.AphbHucreBilgileri.Araci] = kisi.Araci;

                                    dtCariAphb.Rows.Add(newRow);
                                }
                            }

                        }
                    }


                }
            }

            var eklenecekSatirlar = dtCariAphb.AsEnumerable()
                                            .OrderByDescending(p => p[(int)Enums.AphbHucreBilgileri.Araci])
                                            .ThenBy(p => p[(int)Enums.AphbHucreBilgileri.BelgeTuru])
                                            .ThenByDescending(p => p[(int)Enums.AphbHucreBilgileri.Kanun]);

            siraNo = eklenecekSatirlar.Count();

            var SiraNo2 = 1;

            var dtCari = dtCariAphb.Clone();

            foreach (var eklenecekSatir in eklenecekSatirlar)
            {
                var newRow = dtAphb.NewRow();

                for (int i = 0; i < dtAphb.Columns.Count; i++)
                {
                    newRow[i] = eklenecekSatir[i];
                }

                newRow[(int)Enums.AphbHucreBilgileri.SiraNo] = siraNo--;

                dtAphb.Rows.InsertAt(newRow, 0);


                var cariRow = dtCari.NewRow();

                for (int i = 0; i < dtCari.Columns.Count; i++)
                {
                    cariRow[i] = eklenecekSatir[i];
                }

                cariRow[(int)Enums.AphbHucreBilgileri.SiraNo] = SiraNo2++;

                dtCari.Rows.Add(cariRow);
            }

            dtCariAphb = dtCari;

            Metodlar.CariAphbKaydet(isyeri, dtCariAphb, cariAy);

            return "OK";
        }

        public class IseGirisCikisCalismaDonemi
        {
            public int Gun { get; set; }
            public DateTime Giris { get; set; }
            public DateTime Cikis { get; set; }

        }
    }



}
