using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static BildirgeIcmaliResponse AphbIcmalOlustur(Isyerleri isyeri)
        {

            var result = new BildirgeIcmaliResponse();

            var aphbYol = Metodlar.FormBul(isyeri, Enums.FormTuru.Aphb);

            var AraciNo = isyeri.TaseronNo.ToInt() == 0 ? "Ana İşveren" : isyeri.TaseronNo.ToInt().ToString().PadLeft(3, '0');

            try
            {
                if (string.IsNullOrEmpty(aphbYol)) throw new Exception("Aphb dosyası bulunamadı");

                var dtAphb = Metodlar.AylikListeyiYukle(aphbYol);

                var tumKisiler = Metodlar.TumKisileriGetir(dtAphb);

                var tumTesvikIcmalleri = Program.TumTesvikler.ToDictionary(x => x.Key, x => new List<BildirgeYuklemeIcmal>());

                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();

                decimal CarpimOrani687 = -1;

                var kisiler7166 = new List<BasvuruKisiDownload7103>();
                var kisiler7252 = new List<BasvuruKisiDownload7252>();

                var kisiler7166Cekildi = false;
                var kisiler7252Cekildi = false;

                var kanun6486 = Metodlar.Isyeri6486KanunBul(isyeri.IsyeriSicilNo);
                var iptalVarsayilanKanun = string.IsNullOrEmpty(kanun6486) ? "05510" : kanun6486;

                DateTime baslangic = new DateTime(2011, 3, 1);
                foreach (var yilay in tumKisiler.yilveaylar)
                {
                    var tarih = new DateTime(yilay.Key.ToInt(), yilay.Value.ToInt(), 1);

                    var ay = yilay.Key + "-" + yilay.Value;

                    if (tarih < baslangic) continue;

                    var kota7252 = new Dictionary<string, int>();

                    foreach (var kisi in tumKisiler.TumKisiler)
                    {
                        var tc = kisi.Key;

                        if (tumKisiler.KisilerinSatirlari.ContainsKey(tc))
                        {
                            if (tumKisiler.KisilerinSatirlari[tc].ContainsKey(ay))
                            {
                                var kisilerinSatirlari = tumKisiler.KisilerinSatirlari[tc][ay].Where(row => row[(int)Enums.AphbHucreBilgileri.Araci].ToString().StartsWith(AraciNo)).ToList();

                                var iptallerDahil = tumKisiler.KisilerinSatirlariIptallerDahil[tc][ay];

                                var satirlarveIptalleri = Metodlar.SatirlarinIptalKarsiliklariniBul(kisilerinSatirlari, iptallerDahil, iptalVarsayilanKanun);

                                foreach (var item in satirlarveIptalleri)
                                {
                                    var asil = item.Key;
                                    var iptalKanun = item.Value;

                                    var kanun = string.IsNullOrEmpty(asil[(int)Enums.AphbHucreBilgileri.OrijinalKanunNo].ToString()) ? asil[(int)Enums.AphbHucreBilgileri.Kanun].ToString() : asil[(int)Enums.AphbHucreBilgileri.OrijinalKanunNo].ToString();

                                    kanun = kanun.PadLeft(5, '0');

                                    var gun = asil[(int)Enums.AphbHucreBilgileri.Gun].ToString();
                                    var ucret = asil[(int)Enums.AphbHucreBilgileri.Ucret].ToString();
                                    var ikramiye = asil[(int)Enums.AphbHucreBilgileri.Ikramiye].ToString();
                                    var belgeTuru = asil[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString();

                                    if (kanun.EndsWith("687") && CarpimOrani687 == -1)
                                    {
                                        CarpimOrani687 = TesvikHesaplamaSabitleri.CarpimOrani687;

                                        if (!tumKisiler.yilveaylar.Any(p => p.Key.Equals("2016"))) CarpimOrani687 /= 2;
                                    }

                                    if (kanun.EndsWith("7103"))
                                    {
                                        if (tarih >= Program.TumTesvikler["7166"].TesvikBaslamaZamani && tarih < Program.TumTesvikler["7166"].TesvikBaslamaZamani.AddMonths(5))
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

                                            var _7166listesindeVar = kisiler7166.Any(x =>
                                                                                tc.Equals(x.TcKimlikNo) &&
                                                                                tarih >= Convert.ToDateTime(x.PrimveUcretDestegiIcinBaslangicDonemi) &&
                                                                                tarih <= Convert.ToDateTime(x.PrimveUcretDestegiIcinBitisDonemi)
                                                                                );



                                            if (_7166listesindeVar)
                                            {
                                                BildirgeYuklemeIcmal icmal7166 = null;

                                                if (tumTesvikIcmalleri.ContainsKey("7166"))
                                                    icmal7166 = tumTesvikIcmalleri["7166"].FirstOrDefault(p => p.yilay.Key == yilay.Key && p.yilay.Value == yilay.Value);

                                                if (icmal7166 == null)
                                                {
                                                    icmal7166 = new BildirgeYuklemeIcmal
                                                    {
                                                        Kanun = "7166",
                                                        Matrah = 0,
                                                        PrimOdenenGunSayisi = 0,
                                                        Tutar = 0,
                                                        yilay = yilay
                                                    };

                                                    tumTesvikIcmalleri["7166"].Add(icmal7166);
                                                }

                                                icmal7166.Tutar += Metodlar.TesvikTutariHesapla("07166", gun.ToInt(), ucret.ToDecimalSgk() + ikramiye.ToDecimalSgk(), tarih.Year, tarih.Month, belgeTuru, isyeri.IsyeriSicilNo, null, CarpimOrani687);
                                                icmal7166.PrimOdenenGunSayisi += gun.ToInt();
                                            }
                                        }
                                    }

                                    if (kanun.EndsWith("7252"))
                                    {
                                        if (!kisiler7252Cekildi)
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
                                                        IseGirisTarihi = row[(int)Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.Giris]].ToString(),
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
                                                    MessageBox.Show("Sistemden 7252 listesi çekilemedi. 7252 bildirgesindeki kişilerin ortalama gün sayısına bakılmaksızın icmal oluşturulacak" + Environment.NewLine + Environment.NewLine + "Hata Mesajı:" + response.HataMesaji, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                }
                                                else
                                                {
                                                    if (response.Result != null)
                                                    {
                                                        kisiler7252 = response.Result;
                                                    }
                                                }
                                            }

                                            kisiler7252Cekildi = true;
                                        }

                                        var kisi7252BasvuruFormuKaydi = kisiler7252.FirstOrDefault(x =>
                                                                            tc.Equals(x.TcKimlikNo) &&
                                                                            tarih >= Convert.ToDateTime(x.TesvikSuresiBaslangic) &&
                                                                            tarih <= Convert.ToDateTime(x.TesvikSuresiBitis)
                                                                            );



                                        if (kisi7252BasvuruFormuKaydi != null)
                                        {
                                            if (!kota7252.ContainsKey(tc))
                                                kota7252.Add(tc, kisi7252BasvuruFormuKaydi.OrtalamaGunSayisi.ToInt());

                                            if (kota7252[tc] < gun.ToInt())
                                            {
                                                gun = kota7252[tc].ToString();
                                                kota7252[tc] = 0;
                                            }
                                            else
                                            {
                                                kota7252[tc] -= gun.ToInt();
                                            }
                                        }

                                    }

                                    var tesvik = Program.TumTesvikler.FirstOrDefault(p => p.Value.Kanun.PadLeft(5, '0').Equals(kanun) || p.Value.AltKanunlar.Contains(kanun));

                                    if (tesvik.Value == null) continue;

                                    var tesvikTutari = 0m;
                                    //tesvikTutari += Metodlar.TesvikTutariHesapla(kanun, gun.ToInt(), ucret.ToDecimalSgk() + ikramiye.ToDecimalSgk(), tarih.Year, tarih.Month, belgeTuru, isyeri.IsyeriSicilNo, null, CarpimOrani687);


                                    BildirgeYuklemeIcmal icmal = null;

                                    if (tumTesvikIcmalleri.ContainsKey(tesvik.Key))
                                        icmal = tumTesvikIcmalleri[tesvik.Key].FirstOrDefault(p => p.yilay.Key == yilay.Key && p.yilay.Value == yilay.Value);

                                    if (icmal == null)
                                    {
                                        icmal = new BildirgeYuklemeIcmal
                                        {
                                            Kanun = kanun,
                                            Matrah = 0,
                                            PrimOdenenGunSayisi = 0,
                                            Tutar = 0,
                                            yilay = yilay
                                        };
                                        tumTesvikIcmalleri[tesvik.Key].Add(icmal);
                                    }

                                    //if (tesvik.Key == "05510" || tesvik.Key == "5510" || tesvik.Key == "6111" || tesvik.Key == "06111")
                                    //{
                                    //    icmal.Tutar += 0m;
                                    //}

                                    //else
                                    //{
                                    icmal.Tutar += tesvikTutari;

                                    //}


                                    icmal.PrimOdenenGunSayisi += gun.ToInt();

                                    iptalKanun = iptalKanun.PadLeft(5, '0');

                                    var DonusturulenKanun = iptalKanun;
                                    var kanunGun = Convert.ToInt32(gun);
                                    var kanunUcret = ucret.ToDecimalSgk() + ikramiye.ToDecimalSgk();

                                    //Ücret toplamlarının %0.05 i 5510 teşvikine eşitlendi

                                    BildirgeYuklemeIcmal icmal5510 = tumTesvikIcmalleri["5510"].FirstOrDefault(p => p.yilay.Key == yilay.Key && p.yilay.Value == yilay.Value);
                                    if (icmal5510 == null)
                                    {
                                        icmal5510 = new BildirgeYuklemeIcmal
                                        {
                                            Kanun = "5510",
                                            Matrah = 0,
                                            PrimOdenenGunSayisi = 0,
                                            yilay = yilay,
                                            Tutar = 0
                                        };
                                        tumTesvikIcmalleri["5510"].Add(icmal5510);
                                    }

                                    icmal5510.Tutar += kanunUcret * 0.05m;
                                    icmal5510.PrimOdenenGunSayisi += gun.ToInt();

                                    //aphb 6111 toplamının %15,5 i 6111 teşviki
                                    if (kanun.EndsWith("6111"))
                                    {
                                        BildirgeYuklemeIcmal icmal6111 = tumTesvikIcmalleri["6111"].FirstOrDefault(p => p.yilay.Key == yilay.Key && p.yilay.Value == yilay.Value);
                                        if (icmal6111 == null)
                                        {
                                            icmal6111 = new BildirgeYuklemeIcmal
                                            {
                                                Kanun = "6111",
                                                Matrah = 0,
                                                PrimOdenenGunSayisi = 0,
                                                yilay = yilay,
                                                Tutar = 0
                                            };
                                            tumTesvikIcmalleri["6111"].Add(icmal6111);
                                        }

                                        icmal6111.Tutar += Metodlar.TesvikTutariHesapla(kanun, gun.ToInt(), ucret.ToDecimalSgk() + ikramiye.ToDecimalSgk(), tarih.Year, tarih.Month, belgeTuru, isyeri.IsyeriSicilNo, null, CarpimOrani687);
                                        icmal6111.PrimOdenenGunSayisi += gun.ToInt();

                                    }
                                    ////aphb 17103 teşviki hesabı
                                    if (kanun.EndsWith("7103"))
                                    {
                                        BildirgeYuklemeIcmal icmal7103 = tumTesvikIcmalleri["7103"].FirstOrDefault(p => p.yilay.Key == yilay.Key && p.yilay.Value == yilay.Value);
                                        if (icmal7103 == null)
                                        {
                                            icmal7103 = new BildirgeYuklemeIcmal
                                            {
                                                Kanun = "7103",
                                                Matrah = 0,
                                                PrimOdenenGunSayisi = 0,
                                                yilay = yilay,
                                                Tutar = 0
                                            };
                                            tumTesvikIcmalleri["7103"].Add(icmal7103);
                                        }
                                        icmal7103.Tutar += Metodlar.TesvikTutariHesapla(kanun, gun.ToInt(), ucret.ToDecimalSgk() + ikramiye.ToDecimalSgk(), tarih.Year, tarih.Month, belgeTuru, isyeri.IsyeriSicilNo, null, CarpimOrani687);
                                        icmal7103.PrimOdenenGunSayisi += gun.ToInt();
                                    }
                                    //aphb 6645 teşviki hesabı
                                    if (kanun.EndsWith("6645"))
                                    {
                                        BildirgeYuklemeIcmal icmal6645 = tumTesvikIcmalleri["6645"].FirstOrDefault(p => p.yilay.Key == yilay.Key && p.yilay.Value == yilay.Value);
                                        if (icmal6645 == null)
                                        {
                                            icmal6645 = new BildirgeYuklemeIcmal
                                            {
                                                Kanun = "6645",
                                                Matrah = 0,
                                                PrimOdenenGunSayisi = 0,
                                                yilay = yilay,
                                                Tutar = 0
                                            };
                                            tumTesvikIcmalleri["6645"].Add(icmal6645);
                                        }
                                        icmal6645.Tutar += Metodlar.TesvikTutariHesapla(kanun, gun.ToInt(), ucret.ToDecimalSgk() + ikramiye.ToDecimalSgk(), tarih.Year, tarih.Month, belgeTuru, isyeri.IsyeriSicilNo, null, CarpimOrani687);
                                        icmal6645.PrimOdenenGunSayisi += gun.ToInt();

                                    }

                                    ////4857 hesaplama
                                    if (kanun.EndsWith("4857"))
                                    {
                                        BildirgeYuklemeIcmal icmal4857 = tumTesvikIcmalleri["14857"].FirstOrDefault(p => p.yilay.Key == yilay.Key && p.yilay.Value == yilay.Value);
                                        if (icmal4857 == null)
                                        {
                                            icmal4857 = new BildirgeYuklemeIcmal
                                            {
                                                Kanun = "4857",
                                                Matrah = 0,
                                                PrimOdenenGunSayisi = 0,
                                                yilay = yilay,
                                                Tutar = 0
                                            };
                                            tumTesvikIcmalleri["4857"].Add(icmal4857);
                                        }
                                        icmal4857.Tutar += Metodlar.TesvikTutariHesapla(kanun, gun.ToInt(), ucret.ToDecimalSgk() + ikramiye.ToDecimalSgk(), tarih.Year, tarih.Month, belgeTuru, isyeri.IsyeriSicilNo, null, CarpimOrani687);
                                        icmal4857.PrimOdenenGunSayisi += gun.ToInt();
                                    }

                                    //6322 teşvik hesaplaması
                                    if (kanun.EndsWith("6322"))
                                    {
                                        BildirgeYuklemeIcmal icmal6322 = tumTesvikIcmalleri["6322/25510"].FirstOrDefault(p => p.yilay.Key == yilay.Key && p.yilay.Value == yilay.Value);
                                        if (icmal6322 == null)
                                        {
                                            icmal6322 = new BildirgeYuklemeIcmal
                                            {
                                                Kanun = "6322",
                                                Matrah = 0,
                                                PrimOdenenGunSayisi = 0,
                                                yilay = yilay,
                                                Tutar = 0
                                            };
                                            tumTesvikIcmalleri["6322"].Add(icmal6322);
                                        }
                                        icmal6322.Tutar += Metodlar.TesvikTutariHesapla("16322", gun.ToInt(), ucret.ToDecimalSgk() + ikramiye.ToDecimalSgk(), tarih.Year, tarih.Month, belgeTuru, isyeri.IsyeriSicilNo, null, CarpimOrani687);
                                        icmal6322.PrimOdenenGunSayisi += gun.ToInt();
                                    }

                                    //6486 teşviki
                                    if (kanun.EndsWith("6486") || kanun.EndsWith("6111") || kanun.EndsWith("6645") || kanun.EndsWith("14857") || kanun.EndsWith("7103"))
                                    {
                                        BildirgeYuklemeIcmal icmal6486 = tumTesvikIcmalleri["6486"].FirstOrDefault(p => p.yilay.Key == yilay.Key && p.yilay.Value == yilay.Value);
                                        if (icmal6486 == null)
                                        {
                                            icmal6486 = new BildirgeYuklemeIcmal
                                            {
                                                Kanun = "6486",
                                                Matrah = 0,
                                                PrimOdenenGunSayisi = 0,
                                                yilay = yilay,
                                                Tutar = 0
                                            };
                                            tumTesvikIcmalleri["6486"].Add(icmal6486);
                                        }

                                        icmal6486.Tutar += Metodlar.TesvikTutariHesapla("6486", gun.ToInt(), ucret.ToDecimalSgk() + ikramiye.ToDecimalSgk(), tarih.Year, tarih.Month, belgeTuru, isyeri.IsyeriSicilNo, null, CarpimOrani687);
                                        icmal6486.PrimOdenenGunSayisi += gun.ToInt();

                                    }

                                    var dk = tesvik.Value.DonusturulecekKanunlar.FirstOrDefault(p => p.Key.Equals(DonusturulenKanun)).Value;

                                    if (dk == null)
                                    {
                                        DonusturulenKanun = "05510";

                                        dk = tesvik.Value.DonusturulecekKanunlar.FirstOrDefault(p => p.Key.Equals(DonusturulenKanun)).Value;
                                    }

                                    if (dk != null)
                                    {
                                        if (TesvikHesaplamaSabitleri.BagliKanunlarIcmalHesaplama.ContainsKey(kanun) && TesvikHesaplamaSabitleri.BagliKanunlarIcmalHesaplama[kanun].Contains("05510"))
                                        {
                                            if (DonusturulenKanun.Equals("00000"))
                                            {
                                                icmal.Tutar += kanunGun * Metodlar.AsgariUcretBul(tarih.Year, tarih.Month) * 0.05m;
                                            }
                                            else icmal.Tutar += Metodlar.TesvikTutariHesapla("05510", kanunGun, kanunUcret, tarih.Year, tarih.Month, belgeTuru, isyeri.IsyeriSicilNo);
                                        }

                                        var dusulecekTutarlar = DonusturulecekKanun.DusulecekMiktarHesapla(DonusturulenKanun, kanunGun, kanunUcret, tarih.Year, tarih.Month, belgeTuru, isyeri.IsyeriSicilNo, tesvik.Value.DonusenlerIcmaldenDusulsun, null, CarpimOrani687)[DonusturulenKanun];
                                        var dusulecekTutar = dusulecekTutarlar.BagliKanunlarDahilDusulecekTutar;
                                        icmal.Tutar -= dusulecekTutar;

                                        //if (DonusturulenKanun.EndsWith("6486"))
                                        //{

                                        //    BildirgeYuklemeIcmal icmal6486 = null;

                                        //    if (tumTesvikIcmalleri.ContainsKey("6486"))
                                        //        icmal6486 = tumTesvikIcmalleri["6486"].FirstOrDefault(p => p.yilay.Key == yilay.Key && p.yilay.Value == yilay.Value);

                                        //    if (icmal6486 == null)
                                        //    {
                                        //        icmal6486 = new BildirgeYuklemeIcmal
                                        //        {
                                        //            Kanun = DonusturulenKanun,
                                        //            Matrah = 0,
                                        //            PrimOdenenGunSayisi = 0,
                                        //            Tutar = 0,
                                        //            yilay = yilay
                                        //        };

                                        //        tumTesvikIcmalleri["6486"].Add(icmal6486);
                                        //    }

                                        //    icmal6486.Tutar += dusulecekTutarlar.BagliKanunlarHaricDusulecekTutar;
                                        //    icmal6486.PrimOdenenGunSayisi += gun.ToInt();
                                        //}

                                    }

                                }
                            }
                        }
                    }
                }

                var kaydetmeSonucu = Metodlar.AphbIcmalKaydet(isyeri, tumTesvikIcmalleri);



                result.Durum = kaydetmeSonucu.Durum;
                result.HataMesaji = kaydetmeSonucu.HataMesaji;
                result.Result = kaydetmeSonucu.Result;

                result.Tumu = tumTesvikIcmalleri;

                return result;

            }
            catch (Exception ex)
            {
                result.HataMesaji = ex.Message;
                result.Durum = false;
            }

            return result;

        }
    }



}
