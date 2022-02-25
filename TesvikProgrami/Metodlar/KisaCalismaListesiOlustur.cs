using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        public static BaseResponse KisaCalismaListesiOlustur(Isyerleri isyeri)
        {

            var result = new BaseResponse();

            try
            {
                var aphbyol = Metodlar.FormBul(isyeri, Enums.FormTuru.Aphb);
                var bfyol = Metodlar.FormBul(isyeri, Enums.FormTuru.BasvuruFormu);

                if (aphbyol != null)
                {
                    var dtAphb = Metodlar.AylikListeyiYukle(aphbyol);
                    
                    DataSet dsBasvuru = null;

                    if (bfyol != null)
                        dsBasvuru = Metodlar.BasvuruListesiniYukle(bfyol);

                    var tumKisiler = Metodlar.TumKisileriGetir(dtAphb);

                    var sEnbuyukAy = tumKisiler.enbuyukay.Year.ToString() + "-" + tumKisiler.enbuyukay.Month.ToString();

                    var baslangicTarih = DateTime.Today.AddMonths(-2);

                    var cariay = DateTime.Today;
                    var oncekiAy = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);

                    var kisilerbakilacak60 = tumKisiler.AySatirlari.Where(p => p.Key.Equals(sEnbuyukAy)).SelectMany(p => p.Value.Select(row => row[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString())).Distinct();

                    var yasakliBelgeTurleri = new List<string> { "2", "7", "19", "22", "42", "43", "46", "47", "48", "49", "50" };

                    var uygunOlanlar60 = new Dictionary<string, List<GirisCikisTarihleri>>();
                    var uygunOlmayanlar60 = new HashSet<string>();
                    var sistemdenBakilamayanlar60 = new HashSet<string>();
                    var toplamGunSayilari = new Dictionary<string, int>();
                    var kisiBilgileri = new Dictionary<string, Kisi>();
                    var projeGiris = new ProjeGiris(isyeri, Enums.ProjeTurleri.SigortaliIstenAyrilis);


                    foreach (var tc in kisilerbakilacak60)
                    {
                        if (tumKisiler.KisilerinSatirlari.ContainsKey(tc))
                        {
                            var kisiaylar = tumKisiler.KisilerinSatirlari[tc];

                            var aylarVar = true;

                            if (tumKisiler.enbuyukay >= baslangicTarih.AddDays(-baslangicTarih.Day + 1))
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    var ayIlkGun = DateTime.Today.AddMonths(-i);
                                    ayIlkGun = new DateTime(ayIlkGun.Year, ayIlkGun.Month, 1);
                                    if (tumKisiler.enbuyukay >= ayIlkGun)
                                    {
                                        if (!kisiaylar.ContainsKey(ayIlkGun.Year + "-" + ayIlkGun.Month))
                                        {
                                            aylarVar = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            else aylarVar = false;

                            var rows = kisiaylar[sEnbuyukAy];

                            if (rows.Count > 0)
                            {
                                var siralirows = rows.OrderByDescending(row =>
                                    string.IsNullOrEmpty(row[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString()) && string.IsNullOrEmpty(row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString())
                                    ?
                                    DateTime.MinValue
                                    :
                                        string.IsNullOrEmpty(row[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString())
                                        ?
                                        Convert.ToDateTime(row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString())
                                        :
                                        Convert.ToDateTime(row[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString()));

                                var belgeTuru = siralirows.FirstOrDefault()[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString();
                                var ad = siralirows.FirstOrDefault()[(int)Enums.AphbHucreBilgileri.Ad].ToString();
                                var soyad = siralirows.FirstOrDefault()[(int)Enums.AphbHucreBilgileri.Soyad].ToString();

                                kisiBilgileri.Add(tc, new Kisi
                                {
                                    TckimlikNo = tc,
                                    Ad = ad,
                                    Soyad = soyad
                                });

                                if (!yasakliBelgeTurleri.Contains(belgeTuru) && aylarVar)
                                {
                                    var kisi = tumKisiler.TumKisiler[tc];

                                    var girisTarihleri = kisi.TaseronluGirisTarihleri;
                                    girisTarihleri.ForEach(a => a.GirisMi = true);

                                    var cikisTarihleri = kisi.TaseronluCikisTarihleri;
                                    cikisTarihleri.ForEach(a => a.GirisMi = false);

                                    var girisCikisTarihleri = new List<GirisCikisTarihleri>();

                                    girisCikisTarihleri.AddRange(girisTarihleri);
                                    girisCikisTarihleri.AddRange(cikisTarihleri);

                                    var siraliGirisCikislar = girisCikisTarihleri.OrderByDescending(p => p.Tarih).ThenByDescending(p => p.GirisMi ? 0 : 1).ToList();

                                    bool kisi60SartiniSagliyor = true;

                                    for (int i = 0; i < siraliGirisCikislar.Count; i++)
                                    {
                                        var girisCikis = siraliGirisCikislar[i];

                                        if (girisCikis.Tarih.Date < baslangicTarih)
                                        {
                                            if (!girisCikis.GirisMi && i == 0)
                                            {
                                                if (tumKisiler.enbuyukay.Year == girisCikis.Tarih.Year && tumKisiler.enbuyukay.Month == girisCikis.Tarih.Month)
                                                {
                                                    kisi60SartiniSagliyor = false;
                                                }
                                            }

                                            break;
                                        }

                                        if (girisCikis.GirisMi)
                                        {
                                            if (girisCikis.Tarih.Date > baslangicTarih)
                                            {
                                                if (!cikisTarihleri.Any(x => x.Tarih.Date.Equals(girisCikis.Tarih.Date.AddDays(-1))))
                                                {
                                                    kisi60SartiniSagliyor = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (girisCikis.Tarih.Date >= baslangicTarih)
                                            {
                                                if (!girisTarihleri.Any(x => x.Tarih.Date.Equals(girisCikis.Tarih.Date.AddDays(1))))
                                                {
                                                    kisi60SartiniSagliyor = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }


                                    if (kisi60SartiniSagliyor)
                                    {
                                        uygunOlanlar60.Add(tc, siraliGirisCikislar);
                                    }
                                    else uygunOlmayanlar60.Add(tc);
                                }
                                else uygunOlmayanlar60.Add(tc);
                            }
                            else uygunOlmayanlar60.Add(tc);
                        }
                        else uygunOlmayanlar60.Add(tc);
                    }

                    var index = 0;

                    while (index < uygunOlanlar60.Count)
                    {
                        var tc = uygunOlanlar60.ElementAt(index).Key;
                        var girisCikislar = uygunOlanlar60.ElementAt(index).Value;

                        //var sistemGirisCikislar = new KisiGirisCikislariResponse
                        //{
                        //    Durum = true,
                        //    girisCikislar = new List<GirisCikisTarihleri> {
                        //   new GirisCikisTarihleri {
                        //       Tarih= new DateTime(2020,3,10),
                        //       GirisMi=true
                        //   },
                        //   new GirisCikisTarihleri {
                        //       Tarih= new DateTime(2020,3,10),
                        //       GirisMi=false
                        //   },
                        //   new GirisCikisTarihleri {
                        //       Tarih= new DateTime(2020,3,11),
                        //       GirisMi=true
                        //   },
                        //   new GirisCikisTarihleri {
                        //       Tarih= new DateTime(2020,3,9),
                        //       GirisMi=false
                        //   }

                        //}
                        //};

                        var sistemGirisCikislar = Metodlar.SistemdenKisininGirisCikislariniBul(isyeri, tc, ref projeGiris);

                        if (sistemGirisCikislar.Durum)
                        {

                            if (sistemGirisCikislar.girisCikislar.Count > 0)
                            {
                                bool yeniEklenenVar = false;

                                foreach (var item in sistemGirisCikislar.girisCikislar)
                                {
                                    if (!girisCikislar.Any(p => p.Tarih.Date == item.Tarih.Date && p.GirisMi == item.GirisMi))
                                    {
                                        yeniEklenenVar = true;
                                        girisCikislar.Add(item);
                                    }
                                }

                                if (yeniEklenenVar)
                                {
                                    var siraliGirisCikislar = girisCikislar.OrderByDescending(p => p.Tarih).ThenByDescending(p => p.GirisMi ? 0 : 1).ToList();

                                    bool kisi60SartiniSagliyor = true;

                                    for (int i = 0; i < siraliGirisCikislar.Count; i++)
                                    {
                                        var girisCikis = siraliGirisCikislar[i];

                                        if (girisCikis.Tarih.Date < baslangicTarih)
                                        {
                                            //if (!girisCikis.GirisMi && i == 0) kisi60SartiniSagliyor = false;

                                            break;
                                        }

                                        if (girisCikis.GirisMi)
                                        {
                                            if (girisCikis.Tarih.Date > baslangicTarih)
                                            {
                                                if (!girisCikislar.Any(x => x.GirisMi == false && x.Tarih.Date.Equals(girisCikis.Tarih.Date.AddDays(-1))))
                                                {
                                                    kisi60SartiniSagliyor = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (girisCikis.Tarih.Date >= baslangicTarih)
                                            {
                                                if (!girisCikislar.Any(x => x.GirisMi && x.Tarih.Date.Equals(girisCikis.Tarih.Date.AddDays(1))))
                                                {
                                                    kisi60SartiniSagliyor = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (!kisi60SartiniSagliyor)
                                    {
                                        index--;

                                        uygunOlmayanlar60.Add(tc);

                                        uygunOlanlar60.Remove(tc);
                                    }

                                }
                            }
                        }
                        else sistemdenBakilamayanlar60.Add(tc);

                        index++;
                    }

                    var asgari450günBelgeTurleri = new HashSet<string> { "1", "4", "5", "6", "12", "14", "20", "29", "32", "35", "39", "52", "53", "54", "55 " };

                    if (uygunOlanlar60.Count() > 0)
                    {
                        var baslangic = DateTime.Today.AddYears(-3).AddDays(1);


                        foreach (var keyValuePair in uygunOlanlar60)
                        {
                            var tc = keyValuePair.Key;
                            var toplamGun = 0;

                            if (tumKisiler.TumKisiler.ContainsKey(tc))
                            {
                                var kisiAylar = tumKisiler.KisilerinSatirlari[tc];

                                var kisiortalama = 1m;

                                foreach (var item in kisiAylar)
                                {
                                    var ay = new DateTime(item.Key.Split('-')[0].ToInt(), item.Key.Split('-')[1].ToInt(), 1);

                                    var rows = item.Value;

                                    var temp = toplamGun;

                                    if (ay > baslangic)
                                    {
                                        foreach (var row in rows)
                                        {
                                            var belgeTuru = row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString();

                                            if (asgari450günBelgeTurleri.Contains(belgeTuru))
                                            {
                                                var gun = row[(int)Enums.AphbHucreBilgileri.Gun].ToString().ToInt();

                                                toplamGun += gun;
                                            }
                                        }

                                    }
                                    else if (ay.Year == baslangic.Year && ay.Month == baslangic.Month)
                                    {
                                        var maxDay = Math.Min(ay.AddMonths(1).AddDays(-1).Day - baslangic.Day + 1, 30);

                                        var siralirows = rows.OrderByDescending(row =>
                                            string.IsNullOrEmpty(row[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString()) && string.IsNullOrEmpty(row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString())
                                            ?
                                            DateTime.MinValue
                                            :
                                                string.IsNullOrEmpty(row[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString())
                                                ?
                                                Convert.ToDateTime(row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString())
                                                :
                                                Convert.ToDateTime(row[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString()));

                                        foreach (var r in siralirows)
                                        {
                                            var girisGunu = r[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString();
                                            var cikisGunu = r[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString();
                                            var gun = r[(int)Enums.AphbHucreBilgileri.Gun].ToString().ToInt();
                                            var yil = r[(int)Enums.AphbHucreBilgileri.Yil].ToString().ToInt();
                                            var belgeTuru = r[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString();


                                            if (string.IsNullOrEmpty(cikisGunu))
                                            {
                                                var gk = new DateTime(ay.Year, ay.Month, 1);

                                                if (!string.IsNullOrEmpty(girisGunu))
                                                {
                                                    var g = Convert.ToDateTime(girisGunu);
                                                    var t = new DateTime(yil, g.Month, g.Day);
                                                    gk = t;

                                                    if (gk <= baslangic)
                                                    {
                                                        gun = Math.Min(ay.AddMonths(1).AddDays(-1).Day - baslangic.Day + 1, 30);
                                                    }
                                                }
                                                else
                                                {
                                                    gun -= baslangic.Day - 1;
                                                }

                                                if (asgari450günBelgeTurleri.Contains(belgeTuru))
                                                {
                                                    if (gun >= maxDay)
                                                    {
                                                        toplamGun += maxDay;
                                                        maxDay = 0;
                                                    }
                                                    else
                                                    {
                                                        toplamGun += gun;
                                                        maxDay -= gun;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var ck = Convert.ToDateTime(cikisGunu);
                                                var t = new DateTime(yil, ck.Month, ck.Day);

                                                if (t >= baslangic)
                                                {
                                                    var gk = baslangic;

                                                    if (!string.IsNullOrEmpty(girisGunu))
                                                    {
                                                        var g = Convert.ToDateTime(girisGunu);
                                                        g = new DateTime(yil, g.Month, g.Day);
                                                        if (g > baslangic) gk = g;
                                                    }

                                                    var aradakiGun = (t.Day - gk.Day) + 1;

                                                    if (asgari450günBelgeTurleri.Contains(belgeTuru))
                                                    {
                                                        if (aradakiGun >= maxDay)
                                                        {
                                                            toplamGun += maxDay;
                                                            maxDay = 0;
                                                        }
                                                        else
                                                        {
                                                            toplamGun += aradakiGun;
                                                            maxDay -= aradakiGun;
                                                        }
                                                    }
                                                }
                                                else break;
                                            }

                                            if (maxDay == 0) break;

                                        }
                                    }

                                    var fark = Convert.ToDecimal(toplamGun - temp);

                                    if (tumKisiler.enbuyukay == ay)
                                    {
                                        kisiortalama = fark / 30;
                                    }

                                }

                                if (!kisiAylar.ContainsKey(cariay.Year + "-" + cariay.Month))
                                {
                                    toplamGun += (int)Math.Round(Math.Min(DateTime.Today.Day, 30) * kisiortalama, 0, MidpointRounding.AwayFromZero);
                                }

                                if (!kisiAylar.ContainsKey(oncekiAy.Year + "-" + oncekiAy.Month))
                                {
                                    toplamGun += (int)Math.Round(30 * kisiortalama, 0, MidpointRounding.AwayFromZero);
                                }

                            }

                            toplamGunSayilari.Add(tc, toplamGun);

                        }
                    }

                    var kisaCalismaListesi = new List<KisaCalismaRow>();

                    var tumkisiler = uygunOlanlar60.Select(p => p.Key).ToList();
                    tumkisiler.AddRange(uygunOlmayanlar60);

                    var basvuruKisiler6111 = new List<BasvuruKisi>();
                    var basvuruKisiler7103 = new List<BasvuruKisi>();
                    var kisilerGunlukCalisma = new Dictionary<string, decimal?>();
                    int? kisaCalismaGunSayisi = 83;

                    if (uygunOlanlar60.Count() > 0)
                    {
                        if (dsBasvuru != null)
                        {
                            if (dsBasvuru.Tables.Contains("6111"))
                            {
                                var dt6111 = dsBasvuru.Tables["6111"];
                                basvuruKisiler6111 = dt6111.AsEnumerable().Where(p=> p[(int)Sabitler.BasvuruFormlariSutunlari["6111-v2"][Enums.BasvuruFormuSutunTurleri.TesvikBitis]].ToString().Equals("0000/00") == false).Select(row => new BasvuruKisi
                                {
                                    TcKimlikNo = row[(int)Sabitler.BasvuruFormlariSutunlari["6111-v2"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString(),
                                    TesvikDonemiBitis = Convert.ToDateTime(row[(int)Sabitler.BasvuruFormlariSutunlari["6111-v2"][Enums.BasvuruFormuSutunTurleri.TesvikBitis]].ToString()),
                                    Baz = row[(int)Sabitler.BasvuruFormlariSutunlari["6111-v2"][Enums.BasvuruFormuSutunTurleri.Baz]].ToString().ToInt()
                                }).ToList();
                            }
                            if (dsBasvuru.Tables.Contains("7103"))
                            {
                                var dt7103 = dsBasvuru.Tables["7103"];
                                basvuruKisiler7103 = dt7103.AsEnumerable().Select(row => new BasvuruKisi
                                {
                                    TcKimlikNo = row[(int)Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString(),
                                    TesvikDonemiBitis = Convert.ToDateTime(row[(int)Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.TesvikBitis]].ToString()),
                                    Baz = row[(int)Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.Baz]].ToString().ToInt()
                                }).ToList();
                            }
                        }

                        var trh = DateTime.Today.AddYears(-1).AddDays(1);
                        trh = new DateTime(trh.Year, trh.Month, 1);

                        //var sorgulanacakTumKisiler = tumKisiler.AySatirlari.Where(p => new DateTime(p.Key.Split('-')[0].ToInt(), p.Key.Split('-')[1].ToInt(), 1) >= trh).SelectMany(p => p.Value.Select(row => row[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString())).Distinct();

                        var baslangic = DateTime.Today.AddYears(-1).AddDays(1);

                        foreach (var kv in uygunOlanlar60)
                        {
                            var tc = kv.Key;
                            var toplamUcret = 0m;
                            var toplamGun = 0;
                            var kisiGunlukUcret = 0m;

                            if (tumKisiler.TumKisiler.ContainsKey(tc))
                            {
                                var kisiAylar = tumKisiler.KisilerinSatirlari[tc];

                                var kisiortalama = 1m;

                                foreach (var item in kisiAylar)
                                {
                                    var ay = new DateTime(item.Key.Split('-')[0].ToInt(), item.Key.Split('-')[1].ToInt(), 1);

                                    var rows = item.Value;

                                    var temp = toplamGun;
                                    var temp2 = toplamUcret;

                                    if (ay > baslangic)
                                    {
                                        foreach (var row in rows)
                                        {
                                            var belgeTuru = row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString();

                                            if (asgari450günBelgeTurleri.Contains(belgeTuru))
                                            {
                                                var gun = row[(int)Enums.AphbHucreBilgileri.Gun].ToString().ToInt();
                                                var ucret = row[(int)Enums.AphbHucreBilgileri.Ucret].ToString().ToDecimalSgk();
                                                var ikramiye = row[(int)Enums.AphbHucreBilgileri.Ikramiye].ToString().ToDecimalSgk();

                                                toplamGun += gun;
                                                toplamUcret += ucret + ikramiye;
                                            }
                                        }

                                    }
                                    else if (ay.Year == baslangic.Year && ay.Month == baslangic.Month)
                                    {
                                        var maxDay = Math.Min(ay.AddMonths(1).AddDays(-1).Day - baslangic.Day + 1, 30);

                                        var siralirows = rows.OrderByDescending(row =>
                                            string.IsNullOrEmpty(row[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString()) && string.IsNullOrEmpty(row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString())
                                            ?
                                            DateTime.MinValue
                                            :
                                                string.IsNullOrEmpty(row[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString())
                                                ?
                                                Convert.ToDateTime(row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString())
                                                :
                                                Convert.ToDateTime(row[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString()));

                                        foreach (var r in siralirows)
                                        {
                                            var girisGunu = r[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString();
                                            var cikisGunu = r[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString();
                                            var gun = r[(int)Enums.AphbHucreBilgileri.Gun].ToString().ToInt();
                                            var yil = r[(int)Enums.AphbHucreBilgileri.Yil].ToString().ToInt();
                                            var belgeTuru = r[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString();
                                            var ucret = r[(int)Enums.AphbHucreBilgileri.Ucret].ToString().ToDecimalSgk();
                                            var ikramiye = r[(int)Enums.AphbHucreBilgileri.Ikramiye].ToString().ToDecimalSgk();
                                            var gunlukucret = 0m;

                                            if (gun > 0)
                                            {
                                                gunlukucret = (ucret + ikramiye) / gun;
                                            }


                                            if (string.IsNullOrEmpty(cikisGunu))
                                            {
                                                var gk = new DateTime(ay.Year, ay.Month, 1);

                                                if (!string.IsNullOrEmpty(girisGunu))
                                                {
                                                    var g = Convert.ToDateTime(girisGunu);
                                                    var t = new DateTime(yil, g.Month, g.Day);
                                                    gk = t;

                                                    if (gk <= baslangic)
                                                    {
                                                        gun = Math.Min(ay.AddMonths(1).AddDays(-1).Day - baslangic.Day + 1, 30);
                                                    }
                                                }
                                                else
                                                {
                                                    gun -= baslangic.Day - 1;
                                                }

                                                if (asgari450günBelgeTurleri.Contains(belgeTuru))
                                                {
                                                    if (gun >= maxDay)
                                                    {
                                                        toplamGun += maxDay;
                                                        toplamUcret += Math.Round(maxDay * gunlukucret, 2, MidpointRounding.AwayFromZero);
                                                        maxDay = 0;
                                                    }
                                                    else
                                                    {
                                                        toplamGun += gun;
                                                        toplamUcret += Math.Round(gun * gunlukucret, 2, MidpointRounding.AwayFromZero);
                                                        maxDay -= gun;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var ck = Convert.ToDateTime(cikisGunu);
                                                var t = new DateTime(yil, ck.Month, ck.Day);

                                                if (t >= baslangic)
                                                {
                                                    var gk = baslangic;

                                                    if (!string.IsNullOrEmpty(girisGunu))
                                                    {
                                                        var g = Convert.ToDateTime(girisGunu);
                                                        g = new DateTime(yil, g.Month, g.Day);
                                                        if (g > baslangic) gk = g;
                                                    }

                                                    var aradakiGun = (t.Day - gk.Day) + 1;

                                                    if (asgari450günBelgeTurleri.Contains(belgeTuru))
                                                    {
                                                        if (aradakiGun >= maxDay)
                                                        {
                                                            toplamGun += maxDay;
                                                            toplamUcret += Math.Round(maxDay * gunlukucret, 2, MidpointRounding.AwayFromZero);
                                                            maxDay = 0;
                                                        }
                                                        else
                                                        {
                                                            toplamGun += aradakiGun;
                                                            toplamUcret += Math.Round(aradakiGun * gunlukucret, 2, MidpointRounding.AwayFromZero);
                                                            maxDay -= aradakiGun;
                                                        }
                                                    }
                                                }
                                                else break;
                                            }

                                            if (maxDay == 0) break;
                                        }
                                    }

                                    var farkgun = Convert.ToDecimal(toplamGun - temp);
                                    var farkucret = toplamUcret - temp2;

                                    if (tumKisiler.enbuyukay == ay)
                                    {
                                        kisiortalama = farkgun / 30;

                                        if (farkgun > 0)
                                        {
                                            kisiGunlukUcret = farkucret / farkgun;
                                        }

                                    }

                                }

                                if (uygunOlanlar60.ContainsKey(tc))
                                {

                                    if (!kisiAylar.ContainsKey(cariay.Year + "-" + cariay.Month))
                                    {
                                        var eklenecekgun = (int)Math.Round(Math.Min(DateTime.Today.Day, 30) * kisiortalama, 0, MidpointRounding.AwayFromZero);
                                        toplamGun += eklenecekgun;
                                        toplamUcret += Math.Round(eklenecekgun * kisiGunlukUcret, 2, MidpointRounding.AwayFromZero);
                                    }

                                    if (!kisiAylar.ContainsKey(oncekiAy.Year + "-" + oncekiAy.Month))
                                    {
                                        var eklenecekgun = (int)Math.Round(30 * kisiortalama, 0, MidpointRounding.AwayFromZero);
                                        toplamGun += eklenecekgun;
                                        toplamUcret += Math.Round(eklenecekgun * kisiGunlukUcret, 2, MidpointRounding.AwayFromZero);
                                    }
                                }

                            }

                            decimal? gunlukOrtalamaKazanc = 0m;

                            if (toplamGun > 0)
                            {
                                gunlukOrtalamaKazanc = (toplamUcret / toplamGun) * 0.6m;
                                gunlukOrtalamaKazanc -= gunlukOrtalamaKazanc * 0.00759m;

                                var gunlukAsgariOrtalamaKazanc = Metodlar.AsgariUcretBul(DateTime.Today.Year, 1) * 1.5m;
                                gunlukAsgariOrtalamaKazanc -= gunlukAsgariOrtalamaKazanc * 0.00759m;

                                gunlukOrtalamaKazanc = Math.Round(Math.Min(gunlukOrtalamaKazanc.Value, gunlukAsgariOrtalamaKazanc), 2);
                            }

                            kisilerGunlukCalisma.Add(tc, gunlukOrtalamaKazanc);
                        }
                    }

                    var ayinIlkGunu = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                    foreach (var item in tumkisiler)
                    {
                        var kisi = kisiBilgileri[item];

                        var son60gun = uygunOlanlar60.ContainsKey(kisi.TckimlikNo) ? sistemdenBakilamayanlar60.Contains(kisi.TckimlikNo) ? "Sistemden giriş çıkışlara bakılamadı" : "Sağlıyor" : "Sağlamıyor";
                        var son3YildaGunSayisi = toplamGunSayilari.ContainsKey(kisi.TckimlikNo) ? toplamGunSayilari[kisi.TckimlikNo] : -1;
                        var basvuru6111 = basvuruKisiler6111.FirstOrDefault(p => p.TcKimlikNo == kisi.TckimlikNo);
                        var basvuru7103 = basvuruKisiler7103.FirstOrDefault(p => p.TcKimlikNo == kisi.TckimlikNo);
                        var uygunlukDurumu = son60gun.Equals("Sağlıyor") && son3YildaGunSayisi >= 450 ? "Uygun" : "Uygun değil";

                        bool Uygun = uygunlukDurumu.Equals("Uygun");

                        kisaCalismaListesi.Add(new KisaCalismaRow
                        {
                            TcNo = kisi.TckimlikNo,
                            Ad = kisi.Ad,
                            Soyad = kisi.Soyad,
                            Son60Gun = son60gun,
                            Son3YildaGunSayisi = son3YildaGunSayisi != -1 ? son3YildaGunSayisi.ToString() : "-",
                            KisaCalismaUygunlukDurumu = uygunlukDurumu,
                            GunlukKisaCalismaOdenegi = son60gun == "Sağlıyor" ? kisilerGunlukCalisma[kisi.TckimlikNo] : null,
                            KisaCalismaGunSayisi= son60gun == "Sağlıyor" ? kisaCalismaGunSayisi : null,
                            TesvikSuresiBitisAyi6111 = basvuru6111 != null && uygunlukDurumu == "Uygun" && basvuru6111.TesvikDonemiBitis >= ayinIlkGunu ? basvuru6111.TesvikDonemiBitis.Year + "/" + basvuru6111.TesvikDonemiBitis.Month.ToString().PadLeft(2, '0') : "",
                            TesvikOrtalama6111 = basvuru6111 != null && uygunlukDurumu == "Uygun" && basvuru6111.TesvikDonemiBitis >= ayinIlkGunu ? basvuru6111.Baz.ToString() : "",
                            TesvikSuresiBitisAyi7103 = basvuru7103 != null && uygunlukDurumu == "Uygun" && basvuru7103.TesvikDonemiBitis >= ayinIlkGunu ? basvuru7103.TesvikDonemiBitis.Year + "/" + basvuru7103.TesvikDonemiBitis.Month.ToString().PadLeft(2, '0') : "",
                            TesvikOrtalama7103 = basvuru7103 != null && uygunlukDurumu == "Uygun" && basvuru7103.TesvikDonemiBitis >= ayinIlkGunu ? basvuru7103.Baz.ToString() : "",

                        });

                    }

                    if (kisaCalismaListesi.Count > 0)
                    {
                        result.Result = Metodlar.KisaCalismaKaydet(isyeri, kisaCalismaListesi);
                    }


                }
                else
                {
                    result.Durum = false;
                    result.HataMesaji = "Aphb formu bulunamadı";
                }

                return result;

            }
            catch (Exception ex)
            {
                result.HataMesaji = ex.Message;
                result.Durum = false;
            }
            finally
            {

            }

            return result;

        }
    }



}
