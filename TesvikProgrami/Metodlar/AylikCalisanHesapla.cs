using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static Dictionary<string, Dictionary<string, long>> AylikCalisanHesapla(
            string yil, 
            string ay, 
            TumKisilerSonuc tumKisiler,
            ref Dictionary<DateTime, Dictionary<string, long>> AyCalisanSayilari,
            ref Dictionary<DateTime, Dictionary<string, long>> AyCalisanSayilariBazHesaplama
            )
        {

            var sonuc = new Dictionary<string, Dictionary<string, long>>();

            sonuc.Add("AylikCalisan", TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => (long)-1));
            sonuc.Add("AylikCalisanBaz", TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => (long)-1));

            List<string> ayIcindeSatiriOlanKisiler = tumKisiler.KisilerinSatirlari.Where(p => p.Value.ContainsKey(yil + "-" + ay) && p.Value[yil + "-" + ay].Count > 0).Select(p => p.Key).ToList();

            DateTime yilAy = new DateTime(Convert.ToInt32(yil), Convert.ToInt32(ay), 1);

            KeyValuePair<string, string> yilveay = new KeyValuePair<string, string>(yil, ay);

            List<Kisi> kisiler = new List<Kisi>();

            if (ayIcindeSatiriOlanKisiler.Count > 0)
            {
                foreach (var item in sonuc)
                {
                    TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ForEach(p => item.Value[p]++);
                }

                foreach (var secilenKisi in ayIcindeSatiriOlanKisiler)
                {
                    Kisi kisi = tumKisiler.TumKisiler[secilenKisi];

                    kisiler.Add(kisi);

                    if (!kisi.AyIstatikleri.ContainsKey(yilAy))
                    {

                        var kisiSatirlari = tumKisiler.KisilerinSatirlari[secilenKisi][yil + "-" + ay];

                        foreach (var row in kisiSatirlari)
                        {

                            if (String.IsNullOrEmpty(kisi.Ad))
                            {
                                string adi = row[(int)Enums.AphbHucreBilgileri.Ad].ToString().Trim();

                                kisi.Ad = adi;

                                string soyadi = row[(int)Enums.AphbHucreBilgileri.Soyad].ToString().Trim();

                                soyadi = soyadi == "True" ? "DOĞRU" : soyadi == "False" ? "YANLIŞ" : soyadi;

                                kisi.Soyad = soyadi;

                                string ilksoyadi = row[(int)Enums.AphbHucreBilgileri.IlkSoyadi].ToString().Trim();

                                kisi.IlkSoyad = ilksoyadi;

                                string meslekkod = row[(int)Enums.AphbHucreBilgileri.MeslekKod].ToString().Trim();

                                kisi.MeslekKod = meslekkod;
                            }

                            if (!kisi.AyIstatikleri.ContainsKey(yilAy)) kisi.AyIstatikleri.Add(yilAy, new Dictionary<string, BelgeTuruIstatistikleri>());

                            string belgeturu = row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString().Trim();

                            string Kanun = row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().Trim().PadLeft(5, '0');
                            string OrijinalKanun = row[(int)Enums.AphbHucreBilgileri.OrijinalKanunNo].ToString().Trim();

                            if (!string.IsNullOrEmpty(OrijinalKanun))
                            {
                                OrijinalKanun = OrijinalKanun.PadLeft(5, '0');
                            }

                            if (!kisi.AyIstatikleri[yilAy].ContainsKey(belgeturu)) kisi.AyIstatikleri[yilAy].Add(belgeturu, new BelgeTuruIstatistikleri());

                            BelgeTuruIstatistikleri belgeturuIstatistik = kisi.AyIstatikleri[yilAy][belgeturu];

                            decimal Ucret = row[(int)Enums.AphbHucreBilgileri.Ucret].ToString().Trim().ToDecimalSgk();

                            decimal Ikramiye = row[(int)Enums.AphbHucreBilgileri.Ikramiye].ToString().Trim().ToDecimalSgk();

                            string Araci = row[(int)Enums.AphbHucreBilgileri.Araci].ToString();

                            if (String.IsNullOrEmpty(Araci)) throw new FormatException("Aracı sütunu boş");

                            bool TaseronSatiriMi = !Araci.ToLower().Contains("ana şirket") && !Araci.ToLower().Contains("ana işveren");

                            foreach (var item in belgeturuIstatistik.TesvikKanunuIstatistikleri)
                            {
                                var donusturulecekKanunNo = Kanun;

                                if (Program.TumTesvikler[item.Key].DonusturulecekKanunlar.Any(p => p.Key.Equals(OrijinalKanun)))
                                {
                                    donusturulecekKanunNo = OrijinalKanun;
                                }

                                bool DonusturulecekKanun = Program.TumTesvikler[item.Key].DonusturulecekKanunlar.ContainsKey(donusturulecekKanunNo);

                                if (!TaseronSatiriMi && DonusturulecekKanun) belgeturuIstatistik.TesvikKanunuIstatistikleri[item.Key].AraciMi = false;
                            }

                            string EksikGunSayisi = row[(int)Enums.AphbHucreBilgileri.EksikGun].ToString().Trim();
                            string mahiyet = row[(int)Enums.AphbHucreBilgileri.Mahiyet].ToString().Trim();
                            string EksikGunNedeni = row[(int)Enums.AphbHucreBilgileri.EksikGunSebebi].ToString().Trim();
                            string IstenCikisNedeni = row[(int)Enums.AphbHucreBilgileri.IstenCikisNedeni].ToString().Trim();
                            string siraNo = row[(int)Enums.AphbHucreBilgileri.SiraNo].ToString().Trim();
                            string GirisGunu = row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString().Trim();
                            string CikisGunu = row[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString().Trim();

                            int eksikgun = -1;

                            if (!String.IsNullOrEmpty(EksikGunSayisi)) eksikgun = Convert.ToInt32(EksikGunSayisi);

                            int gun = Convert.ToInt32(row[(int)Enums.AphbHucreBilgileri.Gun].ToString().Trim());

                            Dictionary<string, bool> satiraTesvikVerilecekler = Program.TumTesvikler.ToDictionary(x => x.Key, x => true);

                            foreach (var kgu in belgeturuIstatistik.KanunGunveUcretleri)
                            {
                                KanunIstatistik kanunIstatikleri = new KanunIstatistik();

                                var tesvik = Program.TumTesvikler[kgu.Key];

                                var satiraTesvikVerilecekMi = SatiraTesvikVerilecekMi(yilAy.Year, yilAy.Month, kisi, tesvik, GirisGunu, CikisGunu, kisiSatirlari.Count,out bool hataliGunVarmi);

                                satiraTesvikVerilecekler[tesvik.Kanun] = satiraTesvikVerilecekMi;

                                var donusturulecekKanunNo = Kanun;

                                if (tesvik.DonusturulecekKanunlar.Any(p => p.Key.Equals(OrijinalKanun)))
                                {
                                    if (OrijinalKanun.Equals("14857"))
                                    {
                                        donusturulecekKanunNo = OrijinalKanun;
                                    }
                                }

                                if (!belgeturuIstatistik.KanunGunveUcretleri[tesvik.Kanun].ContainsKey(donusturulecekKanunNo)) belgeturuIstatistik.KanunGunveUcretleri[tesvik.Kanun].Add(donusturulecekKanunNo, kanunIstatikleri);
                                else kanunIstatikleri = belgeturuIstatistik.KanunGunveUcretleri[tesvik.Kanun][donusturulecekKanunNo];

                                AphbSatir satir = new AphbSatir
                                {
                                    Adi = kisi.Ad,
                                    CikisGunu = row[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString(),
                                    EksikGunNedeni = EksikGunNedeni,
                                    EksikGunSayisi = EksikGunSayisi,
                                    GirisGunu = row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString(),
                                    Gun = row[(int)Enums.AphbHucreBilgileri.Gun].ToString(),
                                    UCG = row[(int)Enums.AphbHucreBilgileri.UCG].ToString(),
                                    Ikramiye = row[(int)Enums.AphbHucreBilgileri.Ikramiye].ToString(),
                                    IlkSoyadi = kisi.IlkSoyad,
                                    IstenCikisNedeni = IstenCikisNedeni,
                                    MeslekKod = kisi.MeslekKod,
                                    SiraNo = siraNo,
                                    SosyalGuvenlikNo = kisi.TckimlikNo,
                                    Soyadi = kisi.Soyad,
                                    Ucret = row[(int)Enums.AphbHucreBilgileri.Ucret].ToString(),
                                    Araci = Araci,
                                    Mahiyet = mahiyet,
                                    OnayDurumu = row[(int)Enums.AphbHucreBilgileri.OnayDurumu].ToString(),
                                    Kanun = donusturulecekKanunNo,
                                    OrijinalKanun = OrijinalKanun,
                                    TesvikVerilecekMi= satiraTesvikVerilecekMi,
                                    BelgeTuru= belgeturu
                                };



                                if (!TaseronSatiriMi)
                                {
                                    if (satiraTesvikVerilecekMi)
                                    {
                                        kanunIstatikleri.TesvikVerilecekGun += gun;

                                        kanunIstatikleri.TesvikVerilecekUcret += Ucret;

                                        kanunIstatikleri.TesvikVerilecekIkramiye += Ikramiye;
                                    }


                                    kanunIstatikleri.Gun += gun;

                                    kanunIstatikleri.Ucret += Ucret;

                                    kanunIstatikleri.Ikramiye += Ikramiye;


                                    kanunIstatikleri.satirlar.Add(satir);

                                    kanunIstatikleri.AraciMi = false;
                                }

                                kanunIstatikleri.TaseronluGunSayisi += gun;

                                kanunIstatikleri.TaseronluUcret += Ucret;

                                kanunIstatikleri.TaseronluIkramiye += Ikramiye;


                                kanunIstatikleri.TaseronluSatirlar.Add(satir);

                            }


                            foreach (var item in belgeturuIstatistik.TesvikKanunuIstatistikleri)
                            {
                                string tesvikKanun = item.Key;

                                var tesvik = Program.TumTesvikler[tesvikKanun];

                                var donusturulecekKanunNo = Kanun;

                                if (tesvik.DonusturulecekKanunlar.Any(p => p.Key.Equals(OrijinalKanun)))
                                {
                                    donusturulecekKanunNo = OrijinalKanun;
                                }

                                bool DonusturulecekKanun = tesvik.DonusturulecekKanunlar.ContainsKey(donusturulecekKanunNo);

                                //AphbSatir satir = new AphbSatir
                                //{
                                //    Adi = kisi.Ad,
                                //    CikisGunu = row[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString(),
                                //    EksikGunNedeni = EksikGunNedeni,
                                //    EksikGunSayisi = EksikGunSayisi,
                                //    GirisGunu = row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString(),
                                //    Gun = row[(int)Enums.AphbHucreBilgileri.Gun].ToString(),
                                //    Ikramiye = row[(int)Enums.AphbHucreBilgileri.Ikramiye].ToString(),
                                //    IlkSoyadi = kisi.IlkSoyad,
                                //    IstenCikisNedeni = IstenCikisNedeni,
                                //    MeslekKod = kisi.MeslekKod,
                                //    SiraNo = siraNo,
                                //    SosyalGuvenlikNo = kisi.TckimlikNo,
                                //    Soyadi = kisi.Soyad,
                                //    Ucret = row[(int)Enums.AphbHucreBilgileri.Ucret].ToString(),
                                //    Araci = Araci,
                                //    Mahiyet = mahiyet,
                                //    OnayDurumu = row[(int)Enums.AphbHucreBilgileri.OnayDurumu].ToString(),
                                //    Kanun = donusturulecekKanunNo,
                                //    OrijinalKanun = OrijinalKanun,
                                //};

                                if (DonusturulecekKanun)
                                {
                                    if (satiraTesvikVerilecekler[tesvikKanun])
                                    {
                                        if (!TaseronSatiriMi)
                                        {
                                            belgeturuIstatistik.TesvikKanunuIstatistikleri[tesvikKanun].TesvikVerilecekGun += gun;

                                            belgeturuIstatistik.TesvikKanunuIstatistikleri[tesvikKanun].Ucret += Ucret;

                                            belgeturuIstatistik.TesvikKanunuIstatistikleri[tesvikKanun].Ikramiye += Ikramiye;

                                            //belgeturuIstatistik.TesvikKanunuIstatistikleri[tesvikKanun].satirlar.Add(satir);

                                        }

                                        belgeturuIstatistik.TesvikKanunuIstatistikleri[tesvikKanun].TaseronluGunSayisi += gun;

                                        belgeturuIstatistik.TesvikKanunuIstatistikleri[tesvikKanun].TaseronluUcret += Ucret;

                                        belgeturuIstatistik.TesvikKanunuIstatistikleri[tesvikKanun].TaseronluIkramiye += Ikramiye;

                                        //belgeturuIstatistik.TesvikKanunuIstatistikleri[tesvikKanun].TaseronluSatirlar.Add(satir);
                                    }
                                }
                            }


                        }
                    }

                    if (kisi.AyIstatikleri.ContainsKey(yilAy))
                    {
                        var belgeTurleriIstatistik = kisi.AyIstatikleri[yilAy];

                        bool AylikCalisanTaseronsuzaDahilEdilecekMi = (belgeTurleriIstatistik.Any(p => !TesvikHesaplamaSabitleri.AylikCalisanSayisiveBazdaYasakliBelgeTurleri.Contains(p.Key) && p.Value.KanunGunveUcretleri.Any(k => k.Value.Any(x => x.Value.satirlar.Count > 0))));
                        bool AylikCalisanTaseronluDahilEdilecekMi = belgeTurleriIstatistik.Any(p => !TesvikHesaplamaSabitleri.AylikCalisanSayisiveBazdaYasakliBelgeTurleri.Contains(p.Key));

                        foreach (var t in Program.TumTesvikler)
                        {
                            var tesvik = t.Value;

                            if (tesvik.BazHesaplamadaDikkateAlinacakBelgeTurleri.Count > 0)
                            {
                                if (belgeTurleriIstatistik.Any(p => tesvik.BazHesaplamadaDikkateAlinacakBelgeTurleri.Contains(p.Key) && (tesvik.AylikCalisanaTaseronDahilEdilsin || p.Value.KanunGunveUcretleri[tesvik.Kanun].Any(k => k.Value.satirlar.Count > 0)))) sonuc["AylikCalisanBaz"][tesvik.Kanun]++;
                            }
                            else
                            {
                                if (belgeTurleriIstatistik.Any(p => (tesvik.AylikCalisanaTaseronDahilEdilsin && AylikCalisanTaseronluDahilEdilecekMi) || (!tesvik.AylikCalisanaTaseronDahilEdilsin && AylikCalisanTaseronsuzaDahilEdilecekMi))) sonuc["AylikCalisanBaz"][tesvik.Kanun]++;
                            }

                            if (belgeTurleriIstatistik.Any(p => (tesvik.AylikCalisanaTaseronDahilEdilsin && AylikCalisanTaseronluDahilEdilecekMi) || (!tesvik.AylikCalisanaTaseronDahilEdilsin && AylikCalisanTaseronsuzaDahilEdilecekMi))) sonuc["AylikCalisan"][tesvik.Kanun]++;

                        }



                    }
                }
            }

            if (!AyCalisanSayilari.ContainsKey(yilAy)) AyCalisanSayilari.Add(yilAy, null);
            AyCalisanSayilari[yilAy] = sonuc["AylikCalisan"];

            if (!AyCalisanSayilariBazHesaplama.ContainsKey(yilAy)) AyCalisanSayilariBazHesaplama.Add(yilAy, null);
            AyCalisanSayilariBazHesaplama[yilAy] = sonuc["AylikCalisanBaz"];

            return sonuc;
        }


    }


}
