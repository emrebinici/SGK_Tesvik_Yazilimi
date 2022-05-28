using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace TesvikProgrami.Classes
{
    public partial class TesvikHesapla
    {
        public List<KisiTesvik> TesvikKisileriGetir(Dictionary<string, BazBilgileri> bazBilgileri, Dictionary<string, List<string>> Yasaklilar, int Yil, int Ay, Dictionary<string, int> KendiKanunuHaricindekiGunToplami, long AsgariUcretDestekTutariGunSayisi)
        {
            List<KisiTesvik> KisiTesvikSayilari = new List<KisiTesvik>();

            Dictionary<string, Dictionary<string, decimal>> GunToplamlari = TumTesvikler.ToDictionary(x => x.Key, x => new Dictionary<string, decimal>());

            var tesvikler = TumTesvikler.OrderBy(p => p.Key.Equals("7166") ? 0 : 1);

            foreach (var tesvikitem in tesvikler)
            {
                var kanun = tesvikitem.Key;

                var gunToplamKanun = kanun.Equals("7166") || kanun.Equals("7252") || kanun.EndsWith("7256") || kanun.EndsWith("7316") ? "7103" : kanun;

                var tesvik = tesvikitem.Value;

                DateTime yilAy = new DateTime(Yil, Ay, 1);

                var TesviginKendiKanunuHaricindekiGunToplami = KendiKanunuHaricindekiGunToplami.ContainsKey(kanun) ? KendiKanunuHaricindekiGunToplami[kanun] : 0;

                int i = 0;

                if (bazBilgileri.ContainsKey(kanun))
                {
                    var kanunBazBilgileri = bazBilgileri[kanun];

                    while (i < kanunBazBilgileri.BazveDonemler.Count)
                    {
                        if (kanunBazBilgileri.BazveDonemler[i].KisiBelgeTurleri.Count == 0)
                        {
                            kanunBazBilgileri.BazveDonemler.RemoveAt(i);
                        }
                        else
                        {
                            int TesvikAlan = 0;

                            var bazDonemi = kanunBazBilgileri.BazveDonemler[i];

                            for (int j = 0; j < bazDonemi.Kisiler.Count; j++)
                            {
                                var kisi = bazDonemi.Kisiler[j].Key;

                                var kisiIstatistik = bazDonemi.Kisiler[j].Value;

                                if (Yasaklilar.ContainsKey(kanun) && Yasaklilar[kanun].Contains(kisi.TckimlikNo)) continue;

                                BasvuruKisi AktifBasvuru = Metodlar.AktifBasvuruKaydiniGetir(kisi, kanun, Yil, Ay);

                                if (tesvik.BasvuruFormuVar && AktifBasvuru == null) continue;

                                bool devam = false;

                                if (tesvik.Basvuru_Formundaki_Baz_Sayisi_Kadar_Gun_Ay_Icinde_TesvikAlabilir)
                                {
                                    if (tesvik.BasvuruFormuVar)
                                    {
                                        devam = true;
                                    }
                                }
                                else
                                {
                                    if (!tesvik.BasvuruFormuVar || !BasvuruFormlariSutunlari[kanun].ContainsKey(Enums.BasvuruFormuSutunTurleri.Baz) || TesvikAlan < bazDonemi.TesvikAlabilecekKisiSayisi - AktifBasvuru.Baz || bazDonemi.OncedenTesvikAlanlarTaseronsuz.Contains(kisi.TckimlikNo))
                                    {
                                        devam = true;
                                    }
                                }

                                if (devam)
                                {

                                    var toplamGun = kisiIstatistik.ToplamGun;

                                    if (AsgariUcretDestekTutarlariDikkateAlinsin && tesvik.AsgariUcretDestekTutarlariDikkateAlinsin && Program.AsgariUcretDestekTutariDikkateAlinsin[kanun])
                                    {
                                        if (TesviginKendiKanunuHaricindekiGunToplami - AsgariUcretDestekTutariGunSayisi <= 0) devam = false;


                                        if (tesvik.TesvikVerilirseDigerTesviklerIptalEdilecek)
                                        {
                                            toplamGun = 0;

                                            foreach (var kv in kisi.AyIstatikleri[yilAy])
                                            {
                                                var belgeTuru = kv.Key;
                                                var belgeTuruIstastistik = kv.Value;

                                                var kanunIstatistikleri = belgeTuruIstastistik.KanunGunveUcretleri[kanun];

                                                foreach (var kv2 in kanunIstatistikleri)
                                                {
                                                    var ayIcindeKanun = kv2.Key;
                                                    var kanunIstatistik = kv2.Value;

                                                    if (!kanunIstatistik.AraciMi)
                                                    {
                                                        var verilmeyenSatirKanun = tesvik.Kanun == "7252" ? "07252" : "00000";

                                                        if (verilmeyenSatirKanun != ayIcindeKanun)
                                                        {
                                                            toplamGun += kanunIstatistik.Gun;
                                                        }
                                                    }
                                                }
                                            }
                                        }


                                        var kanunGunToplami = GunToplamlari[gunToplamKanun].Where(p => !p.Key.Equals(kisi.TckimlikNo)).Sum(p => p.Value);

                                        if (kanunGunToplami + toplamGun > TesviginKendiKanunuHaricindekiGunToplami - AsgariUcretDestekTutariGunSayisi)
                                        {
                                            devam = false;
                                        }

                                        if (!devam)
                                        {
                                            if (tesvik.BuKanunlardanBiriDonusturulurkenAsgariUcretDestegiBozulupBozulmadiginaBakilmayacak.Count > 0)
                                            {
                                                toplamGun = 0;

                                                foreach (var kv in kisi.AyIstatikleri[yilAy])
                                                {
                                                    var belgeTuru = kv.Key;
                                                    var belgeTuruIstastistik = kv.Value;

                                                    var kanunIstatistikleri = belgeTuruIstastistik.KanunGunveUcretleri[kanun];

                                                    foreach (var kv2 in kanunIstatistikleri)
                                                    {
                                                        var ayIcindeKanun = kv2.Key;
                                                        var kanunIstatistik = kv2.Value;

                                                        if (kanunIstatistik.TesvikVerilecek)
                                                        {
                                                            if (tesvik.BuKanunlardanBiriDonusturulurkenAsgariUcretDestegiBozulupBozulmadiginaBakilmayacak.Contains(ayIcindeKanun))
                                                            {
                                                                devam = true;

                                                                toplamGun += kanunIstatistik.Gun;
                                                            }
                                                        }
                                                    }
                                                }

                                                if (devam)
                                                {
                                                    foreach (var kv in kisi.AyIstatikleri[yilAy])
                                                    {
                                                        var belgeTuru = kv.Key;
                                                        var belgeTuruIstastistik = kv.Value;

                                                        var kanunIstatistikleri = belgeTuruIstastistik.KanunGunveUcretleri[kanun];

                                                        foreach (var kv2 in kanunIstatistikleri)
                                                        {
                                                            var ayIcindeKanun = kv2.Key;
                                                            var kanunIstatistik = kv2.Value;

                                                            if (! tesvik.BuKanunlardanBiriDonusturulurkenAsgariUcretDestegiBozulupBozulmadiginaBakilmayacak.Contains(ayIcindeKanun))
                                                            {
                                                                kanunIstatistik.TesvikVerilecek = false;
                                                            }
                                                            
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (devam)
                                    {

                                        KisiTesvik kisitesvik = new KisiTesvik(kisi, kanun, kisiIstatistik.TesvikTutarlari);

                                        if (!KisiTesvikSayilari.Any(p => p.Kanun.Equals(kanun) && p.Kisi.TckimlikNo.Equals(kisitesvik.Kisi.TckimlikNo))) KisiTesvikSayilari.Add(kisitesvik);

                                        if (!GunToplamlari[gunToplamKanun].ContainsKey(kisi.TckimlikNo))
                                        {
                                            GunToplamlari[gunToplamKanun].Add(kisi.TckimlikNo, toplamGun);
                                        }

                                        //GunToplamlari[gunToplamKanun] += kisiIstatistik.ToplamGun;

                                        TesvikAlan++;
                                    }
                                }
                            }

                            i++;
                        }
                    }
                }
            }

            return KisiTesvikSayilari;

        }

    }



}
