using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TesvikProgrami.Classes;

namespace TesvikProgrami.Classes
{
    public partial class TesvikHesapla
    {
        public Dictionary<string, Tesvik> TesvikBilgileriGetir(string yil
            , string ay
            , DataTable dtaylikliste
            , Isyerleri isyeri
            , out bool KayitYok
            , Dictionary<KeyValuePair<string, string>, List<KeyValuePair<int, int>>> tumyilaylar
            , out List<Cikti> ciktilar
        )
        {
            Dictionary<string, Dictionary<string, List<string>>> yasaklilar = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new Dictionary<string, List<string>>());

            DateTime yilAy = new DateTime(Convert.ToInt32(yil), Convert.ToInt32(ay), 1);

            int Yil = Convert.ToInt32(yil);

            int Ay = Convert.ToInt32(ay);

            var Tesvikler = TumTesvikler;

            bool BasvuruDonemiOlmadigindanTesvikVerilmeyecek = false;

            var isKoluKodu = isyeri.IsyeriSicilNo.Substring(1, 4);

            if (BasvuruYoksaTesvikVerilmesin)
            {
                DateTime buAyinIlkGunu = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                if (yilAy < buAyinIlkGunu.AddMonths(-6))
                {
                    if (!isyeri.BasvuruDonemleri.Any(p => p.Aylar != null && p.Aylar.Split(',').Any(x => Convert.ToDateTime(x).Equals(yilAy))))
                    {
                        if (yilAy <= new DateTime(2018, 3, 1))
                        {
                            if (!isyeri.BasvuruDonemleri.Any(p => Convert.ToDateTime(p.BasvuruDonem) <= new DateTime(2018, 6, 1)))
                            {
                                BasvuruDonemiOlmadigindanTesvikVerilmeyecek = true;
                            }
                        }
                        else BasvuruDonemiOlmadigindanTesvikVerilmeyecek = true;
                    }
                }
            }

            int sayac = 0;

            List<DataRow> kayitlar = TumKisilerSonuc.AySatirlari[yil + "-" + ay];

            List<string> ayIcindeSatiriOlanKisiler = TumKisilerSonuc.KisilerinSatirlari.Where(p => p.Value.ContainsKey(yil + "-" + ay) && p.Value[yil + "-" + ay].Count > 0).Select(p => p.Key).ToList();

            bool OrijinalKanunNoSutunuVar = dtaylikliste.Columns.Count > (int)Enums.AphbHucreBilgileri.OrijinalKanunNo;

            bool BorcluAyMi = isyeri.BorcluAylar.Any(p => Convert.ToDateTime(p.BorcluAy).Equals(yilAy));

            Dictionary<string, Dictionary<YasakliKanun, List<Kisi>>> AyIcindeYasakliKanunuOlanKisiler = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new Dictionary<YasakliKanun, List<Kisi>>());
            Dictionary<string, HashSet<Kisi>> AyIcindeYasakliBelgesiOlanKisiler = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new HashSet<Kisi>());

            var isyeriAsgariUcretBilgisi = isyeri.AsgariUcretDestekTutarlari.FirstOrDefault(p => p.DonemYil.Equals(Yil) && p.DonemAy.Equals(Ay));

            bool asgariUcretDestegiBuAyVeriliyor = isyeriAsgariUcretBilgisi != null && isyeriAsgariUcretBilgisi.HesaplananGun > 0;

        enBasaDon:

            sayac++;

            ciktilar = new List<Cikti>();

            var asgariUcretDestegiKapsamindakiGunSayisi = 0;

            var asgariUcretDestegiKapsamiDisindaTesvikVerilenGunSayisi = 0;


            if (sayac > 1)
            {
                TumKisilerSonuc.TumKisiler.Where(p => p.Value.AyIstatikleri.ContainsKey(yilAy)).ToList().ForEach(p => p.Value.AyIstatikleri.Remove(yilAy));
                TumKisilerSonuc.TumKisiler.ToList().ForEach(p => p.Value.OncedenAlinanTesvikGunleri = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x=> x, x=> new Dictionary<DateTime, Dictionary<string, int>>()));

                TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ForEach(p => TumTesvikler[p].TesvikAyIstatistikSil(yilAy));
            }

            Dictionary<string, bool> AyIcindeVarsaTesvikVerilmeyecekKanunTuruBulunanlar = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => false);
            Dictionary<string, Dictionary<string, string>> ay_Icinde_Tesvik_Alip_Eksik_Gun_Kodundan_Uyari_Verilmesi_Gereken_Kisiler = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new Dictionary<string, string>());

            var KendiKanunuHaricindekiGunToplami = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => 0);

            bool IptalXmlveTesvikAlamayanXmlCikartilmayacak = false;

            KayitYok = false;

            if (kayitlar.Count > 0)
            {
                if (dthatalisatirlar == null && BasvuruFormlariHataliSatirlar.All(p => p.Value == null))
                {
                    DateTime dtkurulustarihi = tumyilaylar.Select(p => new DateTime(Convert.ToInt32(p.Key.Key), Convert.ToInt32(p.Key.Value), 1)).Min();
                    DateTime enbuyukay = tumyilaylar.Select(p => new DateTime(Convert.ToInt32(p.Key.Key), Convert.ToInt32(p.Key.Value), 1)).Max();

                    var OncedenTesvikAlanlar = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new Dictionary<Kisi, List<KanunIstatistik>>());

                    var Onceden7252Alanlar = new Dictionary<Kisi, int>();

                    List<YasakliKanun> sorulacakYasakliKanunlar = new List<YasakliKanun>();

                    long AydaCalisanPersonelSayisiTaseronlu = 0;

                    long AydaCalisanPersonelSayisiTaseronsuz = 0;

                    List<Kisi> kisiler = new List<Kisi>();

                    foreach (var secilenKisi in ayIcindeSatiriOlanKisiler)
                    {
                        Kisi kisi = TumKisilerSonuc.TumKisiler[secilenKisi];

                        kisiler.Add(kisi);

                        if (!kisi.AyIstatikleri.ContainsKey(yilAy))
                        {

                            var kisiSatirlari = TumKisilerSonuc.KisilerinSatirlari[secilenKisi][yil + "-" + ay];

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

                                if (!string.IsNullOrEmpty(OrijinalKanun)) OrijinalKanun = OrijinalKanun.PadLeft(5, '0');

                                if (!kisi.AyIstatikleri[yilAy].ContainsKey(belgeturu)) kisi.AyIstatikleri[yilAy].Add(belgeturu, new BelgeTuruIstatistikleri());

                                BelgeTuruIstatistikleri belgeturuIstatistik = kisi.AyIstatikleri[yilAy][belgeturu];

                                decimal Ucret = row[(int)Enums.AphbHucreBilgileri.Ucret].ToString().Trim().ToDecimalSgk();

                                decimal Ikramiye = row[(int)Enums.AphbHucreBilgileri.Ikramiye].ToString().Trim().ToDecimalSgk();

                                string Araci = row[(int)Enums.AphbHucreBilgileri.Araci].ToString();

                                if (String.IsNullOrEmpty(Araci)) throw new FormatException("Aracı sütunu boş");

                                bool TaseronSatiriMi = !Araci.ToLower().Contains("ana şirket") && !Araci.ToLower().Contains("ana işveren");

                                if (Kanun.EndsWith("7103"))
                                {

                                    if (kisi.KisiBasvuruKayitlari.ContainsKey("7166"))
                                    {
                                        foreach (var bk in kisi.KisiBasvuruKayitlari["7166"])
                                        {
                                            if (yilAy >= bk.TesvikDonemiBaslangic && yilAy <= bk.TesvikDonemiBitis)
                                            {

                                                if (TumTesvikler["7166"].dtKurulusTarihi < TumTesvikler["7166"].KurulusTarihiBuTarihtenBuyukveyeEsitseTesvikVerilmesin && TumTesvikler["7166"].BuYillardaHicBildirgeYoksaTesvikVerilmesin.All(p => TumTesvikler["7166"].BildirgeOlanYillar.Contains(p)))
                                                {

                                                    var basvuru7166Kaydi = kisi.BasvuruListesi7166Kayitlari.FirstOrDefault(p => p.Giris == bk.GirisTarihi);

                                                    if (basvuru7166Kaydi == null || !basvuru7166Kaydi.UygunlukDurumu.Trim().Equals("Uygun Değildir"))
                                                    {
                                                        Kanun = "07166";
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                foreach (var item in belgeturuIstatistik.TesvikKanunuIstatistikleri)
                                {
                                    var donusturulecekKanunNo = Kanun;

                                    if (Tesvikler[item.Key].DonusturulecekKanunlar.Any(p => p.Key.Equals(OrijinalKanun)))
                                    {
                                        donusturulecekKanunNo = OrijinalKanun;
                                    }

                                    bool DonusturulecekKanun = Tesvikler[item.Key].DonusturulecekKanunlar.ContainsKey(donusturulecekKanunNo);

                                    if (!TaseronSatiriMi && DonusturulecekKanun) belgeturuIstatistik.TesvikKanunuIstatistikleri[item.Key].AraciMi = false;
                                }

                                string EksikGunSayisi = row[(int)Enums.AphbHucreBilgileri.EksikGun].ToString().Trim();
                                string mahiyet = row[(int)Enums.AphbHucreBilgileri.Mahiyet].ToString().Trim();
                                string EksikGunNedeni = row[(int)Enums.AphbHucreBilgileri.EksikGunSebebi].ToString().Trim();
                                string IstenCikisNedeni = row[(int)Enums.AphbHucreBilgileri.IstenCikisNedeni].ToString().Trim();
                                string siraNo = row[(int)Enums.AphbHucreBilgileri.SiraNo].ToString().Trim();
                                string CikisGunu = row[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString().Trim();
                                string GirisGunu = row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString().Trim();

                                int eksikgun = -1;

                                if (!String.IsNullOrEmpty(EksikGunSayisi)) eksikgun = Convert.ToInt32(EksikGunSayisi);

                                int gun = Convert.ToInt32(row[(int)Enums.AphbHucreBilgileri.Gun].ToString().Trim());

                                if (gun > 30) gun = 30;

                                Dictionary<string, bool> satiraTesvikVerilecekler = TumTesvikler.ToDictionary(x => x.Key, x => true);

                                foreach (var kgu in belgeturuIstatistik.KanunGunveUcretleri)
                                {
                                    KanunIstatistik kanunIstatikleri = new KanunIstatistik();

                                    var tesvik = TumTesvikler[kgu.Key];

                                    var satiraTesvikVerilecekMi = Metodlar.SatiraTesvikVerilecekMi(Yil, Ay, kisi, tesvik, GirisGunu, CikisGunu, kisiSatirlari.Count, out bool hataliGunVarMi, gun);

                                    if (hataliGunVarMi)
                                    {
                                        if (!hataliGunuOlanKisiler.ContainsKey(isyeri))
                                            hataliGunuOlanKisiler.Add(isyeri, new HashSet<string>());

                                        hataliGunuOlanKisiler[isyeri].Add(String.Format("{0} tc nolu {1} {2} kişisinin {3} başvuru kaydı", kisi.TckimlikNo, kisi.Ad, kisi.Soyad, kgu.Key ));
                                    }

                                    if (tesvik.TesvikVerilmeyecekEksikGunNedenleri.Contains(EksikGunNedeni.Trim().PadLeft(2, '0')))
                                    {
                                        satiraTesvikVerilecekMi = false;
                                    }

                                    //if (!string.IsNullOrEmpty(GirisGunu) || !string.IsNullOrEmpty(CikisGunu))
                                    //{
                                    //   satiraTesvikVerilecekMi = SatiraTesvikVerilecekMi(Yil, Ay, kisi, tesvik, GirisGunu, CikisGunu);
                                    //}

                                    if (tesvik.Kanun == "6322/25510")
                                    {
                                        if (OrijinalKanun.EndsWith("6322") || OrijinalKanun.EndsWith("25510"))
                                        {
                                            satiraTesvikVerilecekMi = false;
                                        }
                                    }

                                    satiraTesvikVerilecekler[tesvik.Kanun] = satiraTesvikVerilecekMi;

                                    var donusturulecekKanunNo = Kanun;

                                    if (!SirketCari14857ListesindeVarMi)
                                    {
                                        if (tesvik.DonusturulecekKanunlar.Any(p => p.Key.Equals(OrijinalKanun)))
                                        {
                                            if (OrijinalKanun.Equals("14857"))
                                            {
                                                donusturulecekKanunNo = OrijinalKanun;
                                            }
                                        }
                                    }

                                    if (!belgeturuIstatistik.KanunGunveUcretleri[tesvik.Kanun].ContainsKey(donusturulecekKanunNo)) belgeturuIstatistik.KanunGunveUcretleri[tesvik.Kanun].Add(donusturulecekKanunNo, kanunIstatikleri);
                                    else kanunIstatikleri = belgeturuIstatistik.KanunGunveUcretleri[tesvik.Kanun][donusturulecekKanunNo];

                                    AphbSatir satir = new AphbSatir
                                    {
                                        Adi = kisi.Ad,
                                        CikisGunu = CikisGunu,
                                        EksikGunNedeni = EksikGunNedeni,
                                        EksikGunSayisi = EksikGunSayisi,
                                        GirisGunu = GirisGunu,
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
                                        TesvikVerilecekMi = satiraTesvikVerilecekMi,
                                        BelgeTuru = belgeturu,
                                        xElement = SatirReferanslari.ContainsKey(row) ? SatirReferanslari[row] : null,
                                        NetsisBilgiler = SatirReferanslariNetsis.ContainsKey(row) ? SatirReferanslariNetsis[row] : null,
                                        NetsisBilgilerExcel = SatirReferanslariNetsisExcel.ContainsKey(row) ? SatirReferanslariNetsisExcel[row] : null,
                                        IlgiliSatir = row
                                    };

                                    if (satir.Kanun == "14857" && muhtasarYil == Yil && muhtasarAy == Ay )
                                    {
                                        Liste14857.Add(row);
                                    }

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

                                    var tesvikKanunuIstatistik = item.Value;

                                    var tesvik = Tesvikler[tesvikKanun];

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
                                        if (satiraTesvikVerilecekler[tesvik.Kanun])
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
                                        }

                                        //belgeturuIstatistik.TesvikKanunuIstatistikleri[tesvikKanun].TaseronluSatirlar.Add(satir);
                                    }

                                    if (tesvik.DestekKapsaminaGirmeyenBelgeTurleri.Count > 0 && tesvik.DestekKapsaminaGirmeyenBelgeTurleri.Contains(belgeturu))
                                    {
                                        tesvikKanunuIstatistik.TesvikAlabilir = false;
                                    }

                                    if (tesvik.DestekKapsaminaGirenBelgeTurleri.Count > 0 && !tesvik.DestekKapsaminaGirenBelgeTurleri.Contains(belgeturu))
                                    {
                                        tesvikKanunuIstatistik.TesvikAlabilir = false;
                                    }


                                    if (tesvik.DestekKapsaminaGirmeyenBelgeTurleri.Count > 0 && tesvik.DestekKapsaminaGirmeyenBelgeTurleri.Contains(belgeturu))
                                    {
                                        tesvikKanunuIstatistik.TesvikAlabilir = false;
                                    }

                                    if (tesvik.DestekKapsaminaGirenBelgeTurleri.Count > 0 && !tesvik.DestekKapsaminaGirenBelgeTurleri.Contains(belgeturu))
                                    {
                                        tesvikKanunuIstatistik.TesvikAlabilir = false;
                                    }


                                    if (tesvik.TesvikBaslamaZamani > yilAy)
                                    {
                                        tesvikKanunuIstatistik.TesvikAlabilir = false;
                                    }

                                    if (tesvik.TesvikBitisZamani != DateTime.MinValue && yilAy >= tesvik.TesvikBitisZamani)
                                    {
                                        tesvikKanunuIstatistik.TesvikAlabilir = false;
                                    }

                                    if (tesvik.BasvuruFormuVar)
                                    {
                                        bool TesvikAlabilir = false;

                                        if (kisi.KisiBasvuruKayitlari.ContainsKey(tesvikKanun))
                                        {

                                            foreach (var basvuru in kisi.KisiBasvuruKayitlari[tesvikKanun])
                                            {
                                                if (yilAy >= basvuru.TesvikDonemiBaslangic && yilAy <= basvuru.TesvikDonemiBitis)
                                                {
                                                    if (
                                                        tesvik.BasvuruFormuGirisTarihiBuTarihtenBuyukveyaEsitseTesvikVerilmeyecek == DateTime.MinValue
                                                        ||
                                                        basvuru.GirisTarihi < tesvik.BasvuruFormuGirisTarihiBuTarihtenBuyukveyaEsitseTesvikVerilmeyecek)
                                                    {


                                                        TesvikAlabilir = true;

                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        if (!TesvikAlabilir)
                                        {
                                            tesvikKanunuIstatistik.TesvikAlabilir = false;
                                        }

                                        if (TesvikAlabilir && tesvikKanun.Equals("7166"))
                                        {
                                            belgeturuIstatistik.TesvikKanunuIstatistikleri["7103"].TesvikAlabilir = false;
                                        }
                                    }

                                    if (tesvik.BasvuruFormunaBakildiktanSonraAltKanunBosIseTesvikVerilmesin)
                                    {
                                        if (String.IsNullOrEmpty(tesvik.AltKanun))
                                        {
                                            tesvikKanunuIstatistik.TesvikAlabilir = false;
                                        }
                                    }

                                    if (!TaseronSatiriMi)
                                    {

                                        if (tesvik.AyIcindeVarsaHicKimseyeTesvikVerilmeyecekKanunlar.Contains(donusturulecekKanunNo))
                                        {
                                            AyIcindeVarsaTesvikVerilmeyecekKanunTuruBulunanlar[tesvikKanun] = true;

                                            tesvikKanunuIstatistik.TesvikAlabilir = false;
                                        }

                                        if (tesvik.AyIcinde_Varsa_Hic_Kimseye_Tesvik_Verilmeyecek_Kanunlar_Icin_Cari_Ayda_Orijinal_Kanuna_Da_Bakilsin)
                                        {
                                            if (tesvik.AyIcindeVarsaHicKimseyeTesvikVerilmeyecekKanunlar.Contains(OrijinalKanun))
                                            {
                                                AyIcindeVarsaTesvikVerilmeyecekKanunTuruBulunanlar[tesvikKanun] = true;

                                                tesvikKanunuIstatistik.TesvikAlabilir = false;
                                            }
                                        }
                                    }

                                    if (!tesvik.TesvikAlabilir)
                                    {
                                        tesvikKanunuIstatistik.TesvikAlabilir = false;
                                    }

                                    foreach (var yk in tesvik.YasakliKanunlar)
                                    {
                                        if (yk.Kanun.Equals(donusturulecekKanunNo))
                                        {
                                            bool devam = true;

                                            if (yk.TaseronGunveKazancinaBakilsin)
                                            {
                                                if (yk.GunveyaKazancSifirdanBuyukOlmali)
                                                {
                                                    if (gun <= 0 && (Ucret + Ikramiye) <= 0)
                                                    {
                                                        devam = false;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (!TaseronSatiriMi)
                                                {
                                                    if (yk.GunveyaKazancSifirdanBuyukOlmali)
                                                    {
                                                        if (gun <= 0 && (Ucret + Ikramiye) <= 0)
                                                        {
                                                            devam = false;
                                                        }
                                                    }
                                                }
                                                else devam = false;
                                            }

                                            if (devam)
                                            {
                                                var yKanun = AyIcindeYasakliKanunuOlanKisiler[tesvikKanun].FirstOrDefault(p => p.Key.Kanun.Equals(yk.Kanun));

                                                if (yKanun.Key == null)
                                                {
                                                    //AyIcindeYasakliKanunuOlanKisiler[tesvikKanun].Add(new YasakliKanun { Kanun = yk.Kanun, KullaniciyaSorulsun = yk.KullaniciyaSorulsun, KullaniciyaSoruldu = yk.KullaniciyaSoruldu, KullaniciCevabi = yk.KullaniciCevabi }, new List<Kisi>());
                                                    AyIcindeYasakliKanunuOlanKisiler[tesvikKanun].Add(yk, new List<Kisi>());
                                                }

                                                yKanun = AyIcindeYasakliKanunuOlanKisiler[tesvikKanun].FirstOrDefault(p => p.Key.Kanun.Equals(yk.Kanun));

                                                if (!yKanun.Value.Contains(kisi)) yKanun.Value.Add(kisi);

                                                if (!yk.KullaniciyaSorulsun) tesvikKanunuIstatistik.TesvikAlabilir = false;
                                            }

                                        }
                                    }

                                    foreach (var yasakliBelgeTuru in tesvik.AyIcindeKisideBuBelgeTurlerindenVarsaTesvikVerilmeyecek)
                                    {
                                        if (belgeturu == yasakliBelgeTuru)
                                        {
                                            AyIcindeYasakliBelgesiOlanKisiler[tesvikKanun].Add(kisi);

                                            tesvikKanunuIstatistik.TesvikAlabilir = false;
                                        }
                                    }

                                    if (BasvuruDonemiOlmadigindanTesvikVerilmeyecek)
                                    {
                                        if (tesvik.Kanun.Equals("7103"))
                                        {
                                            if (yilAy < new DateTime(2018, 1, 1) || yilAy > new DateTime(2018, 5, 1))
                                            {
                                                tesvikKanunuIstatistik.TesvikAlabilir = false;
                                            }
                                        }
                                        else tesvikKanunuIstatistik.TesvikAlabilir = false;
                                    }

                                    if (!tesvik.BorcluAydaTesvikVerilsin)
                                    {
                                        if (BorcluAyMi)
                                        {
                                            tesvikKanunuIstatistik.TesvikAlabilir = false;
                                        }
                                    }

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {
                                        if (tesvik.TesvikVerilmeyecekIsKoluKodlari.Contains(isKoluKodu))
                                        {
                                            tesvikKanunuIstatistik.TesvikAlabilir = false;
                                        }
                                    }


                                }

                                if (!TesvikHesaplamaSabitleri.DestekKapsaminaGirmeyenBelgeTurleri.Contains(belgeturu))
                                {
                                    if (! TesvikHesaplamaSabitleri.AsgariUcretDestegiKapsamiDisindakiKanunlar.Contains(Kanun))
                                    {
                                        if (!TaseronSatiriMi)
                                        {
                                            asgariUcretDestegiKapsamindakiGunSayisi += gun;
                                        }
                                    }
                                }

                            }
                        }

                        if (kisi.AyIstatikleri.ContainsKey(yilAy))
                        {
                            var belgeTurleriIstatistik = kisi.AyIstatikleri[yilAy];

                            if (belgeTurleriIstatistik.Any(p => !TesvikHesaplamaSabitleri.AylikCalisanSayisiveBazdaYasakliBelgeTurleri.Contains(p.Key)))
                            {
                                AydaCalisanPersonelSayisiTaseronlu++;
                            }

                            if (belgeTurleriIstatistik.Any(p => !TesvikHesaplamaSabitleri.AylikCalisanSayisiveBazdaYasakliBelgeTurleri.Contains(p.Key) && p.Value.KanunGunveUcretleri.Any(k => k.Value.Any(x => x.Value.satirlar.Count > 0)))) AydaCalisanPersonelSayisiTaseronsuz++;

                            if (OrijinalKanunNoSutunuVar)
                            {
                                if (belgeTurleriIstatistik.Any(p => p.Value.KanunGunveUcretleri.Any(x => x.Value.Any(k => k.Value.satirlar.Any(j => TesvikHesaplamaSabitleri.AyIcindeOnaylanmamisBildirgelerdeVarsaXmlCikartilmayacakKanunNolar.Contains(j.OrijinalKanun))))))
                                {
                                    IptalXmlveTesvikAlamayanXmlCikartilmayacak = true;
                                }
                            }


                            foreach (var belgeTuruIstatistikItem in belgeTurleriIstatistik)
                            {
                                var belgeTuruIstatistik = belgeTuruIstatistikItem.Value;

                                var belgeturu = belgeTuruIstatistikItem.Key;

                                foreach (var kgu in belgeTuruIstatistik.KanunGunveUcretleri)
                                {
                                    var tesvikKanunNo = kgu.Key;

                                    foreach (var item in kgu.Value)
                                    {
                                        var KanunIstatistik = item.Value;

                                        string ayIcindekiKanun = Convert.ToInt32(item.Key).ToString();

                                        var tesvik = Tesvikler.FirstOrDefault(p => p.Key.Equals(ayIcindekiKanun) || p.Value.AltKanunlar.Any(x => x.Equals(item.Key))).Value;

                                        if (tesvik != null)
                                        {

                                            if (tesvik.TaseronunAldigiTesvikKotadanDusulsun || !KanunIstatistik.AraciMi)
                                            {
                                                var oncedenAlinanKanun = tesvik.Kanun;

                                                //if (tesvik.Kanun.Equals("7103"))
                                                //{
                                                //    if (kisi.KisiBasvuruKayitlari.ContainsKey("7166"))
                                                //    {
                                                //        foreach (var bk in kisi.KisiBasvuruKayitlari["7166"])
                                                //        {
                                                //            if (yilAy >= bk.TesvikDonemiBaslangic && yilAy <= bk.TesvikDonemiBitis)
                                                //            {
                                                //                if (dtkurulustarihi < Tesvikler["7166"].KurulusTarihiBuTarihtenBuyukveyeEsitseTesvikVerilmesin && Program.TumTesvikler["7166"].BuYillardaHicBildirgeYoksaTesvikVerilmesin.All(p => Program.TumTesvikler["7166"].BildirgeOlanYillar.Contains(p)))
                                                //                {
                                                //                    oncedenAlinanKanun = "7166";

                                                //                    break;
                                                //                }
                                                //            }
                                                //        }
                                                //    }
                                                //}

                                                if (!OncedenTesvikAlanlar[oncedenAlinanKanun].ContainsKey(kisi)) OncedenTesvikAlanlar[oncedenAlinanKanun].Add(kisi, new List<KanunIstatistik>());

                                                if (!OncedenTesvikAlanlar[oncedenAlinanKanun][kisi].Contains(KanunIstatistik)) OncedenTesvikAlanlar[oncedenAlinanKanun][kisi].Add(KanunIstatistik);
                                                
                                                if (tesvik.Basvuru_Formundaki_Baz_Sayisi_Kadar_Gun_Ay_Icinde_TesvikAlabilir)
                                                {
                                                    var oncedenAlinanGunSayisi= kisi.KisininAlabilecegiGunSayisiniBul(tesvik.Kanun, belgeturu, KanunIstatistik.Gun, Yil, Ay, isyeri.IsyeriSicilNo);

                                                    if (!kisi.OncedenAlinanTesvikGunleri[tesvik.Kanun].ContainsKey(yilAy)) 
                                                        kisi.OncedenAlinanTesvikGunleri[tesvik.Kanun].Add(yilAy, new Dictionary<string, int>());

                                                    if (!kisi.OncedenAlinanTesvikGunleri[tesvik.Kanun][yilAy].ContainsKey(belgeturu))
                                                        kisi.OncedenAlinanTesvikGunleri[tesvik.Kanun][yilAy].Add(belgeturu, oncedenAlinanGunSayisi);
                                                }
                                            }


                                        }

                                        if (AsgariUcretDestekTutarlariDikkateAlinsin && Tesvikler[tesvikKanunNo].AsgariUcretDestekTutarlariDikkateAlinsin && Program.AsgariUcretDestekTutariDikkateAlinsin[tesvikKanunNo])
                                        {
                                            if (!KanunIstatistik.AraciMi)
                                            {
                                                if (!TesvikHesaplamaSabitleri.DestekKapsaminaGirmeyenBelgeTurleri.Contains(belgeturu))
                                                {
                                                    if (!ayIcindekiKanun.EndsWith(tesvikKanunNo))
                                                    {
                                                        var eklenecekGun = KanunIstatistik.Gun;

                                                        /*
                                                        if (ayIcindekiKanun.EndsWith("7252"))
                                                        {
                                                            eklenecekGun = kisi.KisininAlabilecegiGunSayisiniBul("7252", belgeturu, KanunIstatistik.Gun, Yil, Ay, isyeri.IsyeriSicilNo);
                                                        }
                                                        */

                                                        //if (tesvikKanunNo.Equals("7103") || tesvikKanunNo.Equals("7166") || tesvikKanunNo.Equals("7252") || tesvikKanunNo.EndsWith("7256"))
                                                        if (! Tesvikler[tesvikKanunNo].AsgariUcretDestegiKapsaminda)
                                                        {
                                                            //if (!ayIcindekiKanun.EndsWith("7103") && !ayIcindekiKanun.EndsWith("7166") && !ayIcindekiKanun.EndsWith("7256"))
                                                            if (! TesvikHesaplamaSabitleri.AsgariUcretDestegiKapsamiDisindakiKanunlar.Contains(ayIcindekiKanun.PadLeft(5,'0')))
                                                            {
                                                                //if (ayIcindekiKanun.EndsWith("7252"))
                                                                //{
                                                                    //KendiKanunuHaricindekiGunToplami[tesvikKanunNo] += KanunIstatistik.Gun - eklenecekGun;
                                                                //}
                                                                //else
                                                                //{
                                                                    KendiKanunuHaricindekiGunToplami[tesvikKanunNo] += eklenecekGun;
                                                                //}
                                                            }
                                                        }
                                                        else
                                                            KendiKanunuHaricindekiGunToplami[tesvikKanunNo] += eklenecekGun;
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }

                    if (yilAy < new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1))
                    {
                        var ayc = isyeri.AylikCalisanSayilari.FirstOrDefault(p => p.DonemYil.Equals(Yil) && p.DonemAy.Equals(Ay));

                        if (ayc != null)
                        {
                            if (ayc.CalisanSayisiTaseronsuz > -1)
                            {
                                AydaCalisanPersonelSayisiTaseronlu = ayc.CalisanSayisiTaseronlu;
                                AydaCalisanPersonelSayisiTaseronsuz = ayc.CalisanSayisiTaseronsuz;
                            }
                        }
                    }

                    if (!AyCalisanSayilari.ContainsKey(yilAy)) AyCalisanSayilari.Add(yilAy, new Dictionary<string, long>());

                    var ayCalisanSayilari = AyCalisanSayilari[yilAy];

                    if (!AyCalisanSayilariBazHesaplama.ContainsKey(yilAy)) AyCalisanSayilariBazHesaplama.Add(yilAy, new Dictionary<string, long>());

                    var ayCalisanSayilariBazHesaplama = AyCalisanSayilariBazHesaplama[yilAy];

                    foreach (var item in Tesvikler)
                    {
                        if (item.Value.BazHesaplamadaDikkateAlinacakBelgeTurleri.Count > 0)
                        {

                        }
                        else
                        {
                            if (!ayCalisanSayilariBazHesaplama.ContainsKey(item.Key)) ayCalisanSayilariBazHesaplama.Add(item.Key, 0);

                            ayCalisanSayilariBazHesaplama[item.Key] = item.Value.AylikCalisanaTaseronDahilEdilsin ? AydaCalisanPersonelSayisiTaseronlu : AydaCalisanPersonelSayisiTaseronsuz;
                        }

                        if (!ayCalisanSayilari.ContainsKey(item.Key)) ayCalisanSayilari.Add(item.Key, 0);

                        ayCalisanSayilari[item.Key] = item.Value.AylikCalisanaTaseronDahilEdilsin ? AydaCalisanPersonelSayisiTaseronlu : AydaCalisanPersonelSayisiTaseronsuz;
                    }

                    var enAzBirTesvigiHakedenler = kisiler
                                                    .Where(k => k.AyIstatikleri.ContainsKey(yilAy) &&
                                                                k.AyIstatikleri[yilAy].Any(p => p.Value.TesvikKanunuIstatistikleri.Any(x => x.Value.TesvikAlabilir)));

                    foreach (Kisi kisi in enAzBirTesvigiHakedenler)
                    {
                        if (kisi.AyIstatikleri.ContainsKey(yilAy))
                        {
                            var CalismaDonemleri = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new List<KeyValuePair<DateTime, DateTime>>());

                            foreach (var item in kisi.KisiBasvuruKayitlari)
                            {
                                var Kanun = item.Key;

                                var tesvik = Tesvikler[Kanun];

                                if (!tesvik.CikistanSonraGiriseTesvikVerilsin)
                                {

                                    var basvuru = Metodlar.AktifBasvuruKaydiniGetir(kisi, Kanun, Convert.ToInt32(yil), Convert.ToInt32(ay));

                                    if (basvuru != null)
                                    {
                                        var CikisTarihleri = new List<GirisCikisTarihleri>();

                                        CikisTarihleri.AddRange(kisi.CikisTarihleri);

                                        if (basvuru.CikisTarihi != DateTime.MinValue)
                                        {
                                            if (CikisTarihleri.Count(ct => ct.Tarih == basvuru.CikisTarihi) == 0)
                                            {

                                                CikisTarihleri.Add(new GirisCikisTarihleri
                                                {
                                                    Tarih = basvuru.CikisTarihi
                                                });
                                            }
                                        }

                                        if (tesvik.ArdArda2AyYoksaKisiCikmisKabulEdilsin)
                                        {
                                            foreach (var tarih in kisi.CalisilanAylarTaseronsuz)
                                            {

                                                if (!kisi.CalisilanAylarTaseronsuz.Contains(tarih.AddMonths(1)) && !kisi.CalisilanAylarTaseronsuz.Contains(tarih.AddMonths(2)))
                                                {
                                                    if (tarih.AddMonths(1) == basvuru.TesvikDonemiBaslangic)
                                                    {
                                                        CikisTarihleri.Add(new GirisCikisTarihleri
                                                        {
                                                            Tarih = basvuru.TesvikDonemiBaslangic.AddMonths(1)
                                                        });

                                                        break;
                                                    }
                                                    else if (tarih.AddMonths(1) > basvuru.TesvikDonemiBaslangic)
                                                    {
                                                        CikisTarihleri.Add(new GirisCikisTarihleri
                                                        {
                                                            Tarih = tarih.AddMonths(1)
                                                        });

                                                        break;
                                                    }

                                                }
                                            }
                                        }


                                        DateTime CalismaBaslangic = basvuru.GirisTarihi > DateTime.MinValue ? basvuru.GirisTarihi : basvuru.TesvikDonemiBaslangic;

                                        DateTime CalismaBitis = DateTime.MaxValue;

                                        if (CalismaBitis == DateTime.MaxValue)
                                        {
                                            DateTime enyakincikis = DateTime.MaxValue;

                                            foreach (var cikistarihleri in CikisTarihleri)
                                            {
                                                DateTime dtcikis = cikistarihleri.Tarih;

                                                if (dtcikis >= CalismaBaslangic)
                                                {
                                                    if (dtcikis < enyakincikis) enyakincikis = dtcikis;
                                                }
                                            }

                                            CalismaBitis = enyakincikis;
                                        }

                                        CalismaDonemleri[Kanun].Add(new KeyValuePair<DateTime, DateTime>(CalismaBaslangic, CalismaBitis));

                                    }
                                }
                            }

                            var ayIstatistik = kisi.AyIstatikleri[yilAy];

                            List<string> AyIcindeOlanYasakliKanunlar = new List<string>();

                            var enumerator2 = ayIstatistik.GetEnumerator();

                            while (enumerator2.MoveNext())
                            {
                                BelgeTuruIstatistikleri belgeTuruIstatistik = enumerator2.Current.Value;

                                string belgeturu = enumerator2.Current.Key;

                                bool AyIcindeGiristenOnceCikisVarsaVeyaAyIcindeBirdenFazlaGiriseBakildiMi = false;
                                bool AyIcindeGiristenOnceCikisVarsaVeyaAyIcindeBirdenFazlaGirisVarMi = false;

                                foreach (var item in belgeTuruIstatistik.TesvikKanunuIstatistikleri)
                                {
                                    var Kanun = item.Key;

                                    var tesvik = Tesvikler[Kanun];

                                    var tesvikKanunuIstatistik = item.Value;

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {
                                        if (!tesvik.CikistanSonraGiriseTesvikVerilsin)
                                        {
                                            bool TesvikAlabilir = false;

                                            foreach (var calismadonemi in CalismaDonemleri[Kanun])
                                            {
                                                DateTime CalismaBaslangic = calismadonemi.Key;

                                                DateTime CalismaBitis = calismadonemi.Value;


                                                if (yilAy >= CalismaBaslangic && yilAy <= CalismaBitis)
                                                {
                                                    TesvikAlabilir = true;

                                                    break;
                                                }
                                                else
                                                {
                                                    if ((yilAy.Month == CalismaBaslangic.Month && yilAy.Year == CalismaBaslangic.Year)
                                                        || (yilAy.Month == CalismaBitis.Month && yilAy.Year == CalismaBitis.Year))
                                                    {
                                                        TesvikAlabilir = true;

                                                        break;
                                                    }
                                                    else
                                                    {

                                                    }
                                                }
                                            }

                                            if (!TesvikAlabilir)
                                            {
                                                tesvikKanunuIstatistik.TesvikAlabilir = false;
                                            }
                                        }
                                    }

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {

                                        if (tesvik.KurulustanBuKadarAySonraTesvikVermeyeBaslasin > -1)
                                        {

                                            if (yilAy < dtkurulustarihi.AddMonths(tesvik.KurulustanBuKadarAySonraTesvikVermeyeBaslasin))
                                            {
                                                tesvikKanunuIstatistik.TesvikAlabilir = false;
                                            }
                                        }
                                    }

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {

                                        if (tesvik.KurulusTarihiBuTarihtenBuyukveyeEsitseTesvikVerilmesin > DateTime.MinValue)
                                        {

                                            if (dtkurulustarihi >= tesvik.KurulusTarihiBuTarihtenBuyukveyeEsitseTesvikVerilmesin)
                                            {
                                                tesvikKanunuIstatistik.TesvikAlabilir = false;
                                            }
                                        }
                                    }

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {

                                        if (tesvik.BuYillardaHicBildirgeYoksaTesvikVerilmesin.Count > 0)
                                        {
                                            if (tesvik.BuYillardaHicBildirgeYoksaTesvikVerilmesin.Any(p => !tesvik.BildirgeOlanYillar.Contains(p)))
                                            {
                                                tesvikKanunuIstatistik.TesvikAlabilir = false;
                                            }

                                        }
                                    }

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {
                                        if (tesvik.AylikCalisanKisitlamaKalkmaTarihiveKisitlamaSayisi.Key > DateTime.MinValue)
                                        {
                                            if (yilAy < tesvik.AylikCalisanKisitlamaKalkmaTarihiveKisitlamaSayisi.Key
                                                && (tesvik.AylikCalisanaTaseronDahilEdilsin ? AydaCalisanPersonelSayisiTaseronlu : AydaCalisanPersonelSayisiTaseronsuz) < tesvik.AylikCalisanKisitlamaKalkmaTarihiveKisitlamaSayisi.Value)
                                            {
                                                tesvikKanunuIstatistik.TesvikAlabilir = false;
                                            }
                                        }
                                    }

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {
                                        if (tesvik.VerilipVerilmeyecegiKullaniciyaSorulsun && tesvik.VerilipVerilmeyecegiKullaniciyaSoruldu && !tesvik.VerilipVerilmeyecegiKullaniciCevabi)
                                        {
                                            tesvikKanunuIstatistik.TesvikAlabilir = false;
                                        }
                                    }

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {

                                        if (!tesvik.TaseronaTesvikVerilsin)
                                        {
                                            if (tesvikKanunuIstatistik.AraciMi)
                                            {
                                                tesvikKanunuIstatistik.TesvikAlabilir = false;
                                            }
                                        }
                                    }

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {

                                        if (tesvik.AyIcindeGiristenOnceCikisVarsaVeyaAyIcindeBirdenFazlaGirisVarsaTesvikVerilmesin)
                                        {
                                            int girissayisi = 0;

                                            foreach (GirisCikisTarihleri giristarihi in kisi.GirisTarihleri)
                                            {

                                                if (giristarihi.Ay.Equals(yilAy.Month.ToString()) && giristarihi.Yil.Equals(yilAy.Year.ToString()))
                                                {
                                                    girissayisi++;
                                                }
                                            }

                                            if (girissayisi > 1)
                                            {
                                                tesvikKanunuIstatistik.TesvikAlabilir = false;
                                            }

                                        }
                                    }

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {
                                        var kanunYasaklilari = yasaklilar[Kanun];

                                        var anahtar = yil + "-" + ay + "-" + belgeturu;

                                        if (kanunYasaklilari.ContainsKey(anahtar) && kanunYasaklilari[anahtar].Contains(kisi.TckimlikNo))
                                        {
                                            tesvikKanunuIstatistik.TesvikAlabilir = false;
                                        }

                                    }

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {
                                        var EksikGunYasaklilari = EksikGunuKodundanDolayiUyariVerilenKisiler[Kanun];

                                        if (EksikGunYasaklilari.ContainsKey(kisi.TckimlikNo) && EksikGunYasaklilari[kisi.TckimlikNo] == false)
                                        {
                                            tesvikKanunuIstatistik.TesvikAlabilir = false;
                                        }

                                    }

                                    if (AyIcindeVarsaTesvikVerilmeyecekKanunTuruBulunanlar[Kanun])
                                    {
                                        tesvikKanunuIstatistik.TesvikAlabilir = false;
                                    }

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {
                                        if (tesvik.GirisTarihindenItibarenSuKadarAyIcindeIstenCikildiysaTesvikVerilmesin > -1)
                                        {
                                            var basvuru = Metodlar.AktifBasvuruKaydiniGetir(kisi, Kanun, Convert.ToInt32(yil), Convert.ToInt32(ay));

                                            if (basvuru != null)
                                            {
                                                var kisiBasvuruKayit7166 = kisi.BasvuruListesi7166Kayitlari.FirstOrDefault(p => p.Giris == basvuru.GirisTarihi);

                                                if (kisiBasvuruKayit7166 != null)
                                                {
                                                    if (kisiBasvuruKayit7166.UygunlukDurumu == "Uygun Değildir")
                                                    {
                                                        tesvikKanunuIstatistik.TesvikAlabilir = false;
                                                    }
                                                }
                                                else
                                                {

                                                    var cikisTarihleri = kisi.CikisTarihleri;

                                                    if (basvuru.CikisTarihi != DateTime.MinValue)
                                                    {
                                                        if (!cikisTarihleri.Any(p => p.Tarih.Date.Equals(basvuru.CikisTarihi.Date))) cikisTarihleri.Add(new GirisCikisTarihleri { Tarih = basvuru.CikisTarihi });
                                                    }

                                                    var sonTarih = basvuru.GirisTarihi.AddMonths(tesvik.GirisTarihindenItibarenSuKadarAyIcindeIstenCikildiysaTesvikVerilmesin);

                                                    var ilkcikis = kisi.CikisTarihleri.OrderBy(p => p.Tarih).FirstOrDefault(p => p.Tarih >= basvuru.GirisTarihi && p.Tarih <= sonTarih);

                                                    var istenCikisNedeni = string.Empty;

                                                    if (ilkcikis != null)
                                                    {
                                                        if (!String.IsNullOrEmpty(ilkcikis.IstenCikisNedeni))
                                                        {
                                                            istenCikisNedeni = Convert.ToInt32(ilkcikis.IstenCikisNedeni).ToString();
                                                        }
                                                        else
                                                        {
                                                            istenCikisNedeni = Metodlar.SistemdenIstenCikisNedeniBul(isyeri, kisi.TckimlikNo, ilkcikis.Tarih);
                                                        }
                                                    }
                                                    if (ilkcikis != null && (tesvik.IstenCikisYasakliKodlar.Count == 0 || tesvik.IstenCikisYasakliKodlar.Contains(istenCikisNedeni)))
                                                    {
                                                        tesvikKanunuIstatistik.TesvikAlabilir = false;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {

                                        if (!belgeTuruIstatistik.KanunGunveUcretleri[tesvik.Kanun].Any(p => p.Value.TesvikVerilecek
                                                       && (tesvik.DonusturulecekKanunlar.ContainsKey(p.Key) && (CariAyMi(yilAy) || !tesvik.DonusturulecekKanunlar[p.Key].SadeceCari))
                                                       &&
                                                       (
                                                               (
                                                                   tesvik.GunuSifirOlanlaraTesvikVerilsin || p.Value.TesvikVerilecekGun > 0
                                                               )
                                                               &&
                                                               (
                                                                   tesvik.ToplamUcretiSifirOlanlaraTesvikVerilsin || p.Value.TesvikVerilecekToplamUcret > 0
                                                               )
                                                       )
                                                 )
                                    )
                                        {
                                            tesvikKanunuIstatistik.TesvikAlabilir = false;
                                        }
                                    }

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {
                                        if (tesvik.AyIcindeGiristenOnceCikisVarsaVeyaAyIcindeBirdenFazlaGirisVarsaTesvikVerilmesin)
                                        {
                                            if (!AyIcindeGiristenOnceCikisVarsaVeyaAyIcindeBirdenFazlaGiriseBakildiMi)
                                            {
                                                AyIcindeGiristenOnceCikisVarsaVeyaAyIcindeBirdenFazlaGiriseBakildiMi = true;

                                                foreach (var cikistarihleri in kisi.CikisTarihleri)
                                                {
                                                    DateTime dtcikis = cikistarihleri.Tarih;

                                                    if (dtcikis.Year == yilAy.Year && dtcikis.Month == yilAy.Month)
                                                    {
                                                        foreach (var giristarihleri in kisi.GirisTarihleri)
                                                        {
                                                            DateTime dtgiris = giristarihleri.Tarih;

                                                            if (dtgiris.Year == yilAy.Year && dtgiris.Month == yilAy.Month)
                                                            {
                                                                if (dtcikis <= dtgiris)
                                                                {
                                                                    AyIcindeGiristenOnceCikisVarsaVeyaAyIcindeBirdenFazlaGirisVarMi = true;

                                                                    break;
                                                                }
                                                            }
                                                        }

                                                    }
                                                }
                                            }

                                            if (AyIcindeGiristenOnceCikisVarsaVeyaAyIcindeBirdenFazlaGirisVarMi)
                                            {
                                                tesvikKanunuIstatistik.TesvikAlabilir = false;
                                            }
                                        }
                                    }

                                    //foreach (var kgu in belgeTuruIstatistik.KanunGunveUcretleri[tesvik.Kanun])
                                    //{
                                    //    var kanun = Convert.ToInt32(kgu.Key).ToString();

                                    //    var tesvik3 = Tesvikler.FirstOrDefault(p => p.Key.Equals(kanun) || p.Value.AltKanunlar.Contains(kgu.Key)).Value;

                                    //    if (tesvik3 != null)
                                    //    {
                                    //        bool SadeceTaseronlu = kgu.Value.AraciMi;

                                    //        if (!SadeceTaseronlu)
                                    //        {
                                    //            foreach (var tki in belgeTuruIstatistik.TesvikKanunuIstatistikleri)
                                    //            {
                                    //                if (tesvik3.Kanun.Equals(tki.Key)) continue;

                                    //                var tesvik2 = Tesvikler[tki.Key];

                                    //                if (tesvik2.AyIcindeDahaOncedenAlinanBaskaTesvikVarsaTesvikVerilmesin)
                                    //                {
                                    //                    if (!tesvik2.DonusturulecekKanunlar.ContainsKey(kgu.Key))
                                    //                    {
                                    //                        if (tesvik2.altTesvikler.Contains(tesvik3.Kanun)) continue;

                                    //                        tki.Value.TesvikAlabilir = false;
                                    //                    }
                                    //                }
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            if (tesvik3.TaseronunAldigiTesvikKotadanDusulsun)
                                    //            {
                                    //                belgeTuruIstatistik.TesvikKanunuIstatistikleri[tesvik3.Kanun].TesvikAlabilir = false;
                                    //            }
                                    //        }

                                    //    }
                                    //}

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {
                                        var yasakliKanunuOlanlar = AyIcindeYasakliKanunuOlanKisiler[Kanun];

                                        foreach (var ykItem in yasakliKanunuOlanlar)
                                        {
                                            YasakliKanun yk = ykItem.Key;

                                            if (!yk.KullaniciyaSorulsun || (yk.KullaniciyaSoruldu && !yk.KullaniciCevabi))
                                            {
                                                var yasakliKanunuOlanKisiler = ykItem.Value;

                                                if (yasakliKanunuOlanKisiler.Contains(kisi))
                                                {
                                                    tesvikKanunuIstatistik.TesvikAlabilir = false;
                                                }
                                            }

                                        }
                                    }

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {
                                        var yasakliBelgesiOlanlar = AyIcindeYasakliBelgesiOlanKisiler[Kanun];

                                        if (yasakliBelgesiOlanlar.Contains(kisi))
                                        {
                                            tesvikKanunuIstatistik.TesvikAlabilir = false;
                                        }

                                    }


                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {
                                        if (tesvik.GeriyeDonukBelirtilenAylardakiHizmetBildirimlerininBiriYasakliIseTesvikVerilmeyecek.Count > 0)
                                        {
                                            foreach (var ayTarihi in tesvik.GeriyeDonukBelirtilenAylardakiHizmetBildirimlerininBiriYasakliIseTesvikVerilmeyecek)
                                            {
                                                var ayKey = ayTarihi.Year.ToString() + "-" + ayTarihi.Month.ToString();

                                                if (TumKisilerSonuc.KisilerinSatirlari.ContainsKey(kisi.TckimlikNo))
                                                {
                                                    var kisiaylari = TumKisilerSonuc.KisilerinSatirlari[kisi.TckimlikNo];

                                                    if (kisiaylari.ContainsKey(ayKey))
                                                    {
                                                        var kisiAySatirlari = kisiaylari[ayKey];

                                                        if (kisiAySatirlari.Any(prow => tesvik.DestekKapsaminaGirmeyenBelgeTurleri.Contains(prow[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString().ToInt().ToString())))
                                                        {
                                                            tesvikKanunuIstatistik.TesvikAlabilir = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }


                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {
                                        if (tesvik.GeriyeDonukBelirtilenAylardakiHizmetBildirimlerininBiriBelirtilenEksikGunKodlarindanOlmali.Count > 0)
                                        {
                                            var gecerliEksikGunSebebiBulundu = false;

                                            foreach (var ayTarihi in tesvik.GeriyeDonukBelirtilenAylardakiHizmetBildirimlerininBiriBelirtilenEksikGunKodlarindanOlmali)
                                            {
                                                var ayKey = ayTarihi.Year.ToString() + "-" + ayTarihi.Month.ToString();

                                                if (TumKisilerSonuc.KisilerinSatirlari.ContainsKey(kisi.TckimlikNo))
                                                {
                                                    var kisiaylari = TumKisilerSonuc.KisilerinSatirlari[kisi.TckimlikNo];

                                                    if (kisiaylari.ContainsKey(ayKey))
                                                    {
                                                        var kisiAySatirlari = kisiaylari[ayKey];

                                                        if (kisiAySatirlari.Any(prow => tesvik.GeriyeDonukBelirtilenAylardakiHizmetBildirimlerininBiriBuEksikGunKodlarindanOlmali.Contains(prow[(int)Enums.AphbHucreBilgileri.EksikGunSebebi].ToString().PadLeft(2, '0'))))
                                                        {
                                                            gecerliEksikGunSebebiBulundu = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }

                                            if (!gecerliEksikGunSebebiBulundu)
                                            {
                                                tesvikKanunuIstatistik.TesvikAlabilir = false;
                                            }
                                        }
                                    }

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {
                                        if (tesvik.CariAydaEksikGunKoduBunlardanBiriIseKisiyeDigerBelgeTurlerideDahilTesvikVerilmeyecek.Count > 0)
                                        {

                                            if (CariAyMi(yilAy))
                                            {
                                                var ayKey = yilAy.Year.ToString() + "-" + yilAy.Month.ToString();

                                                if (TumKisilerSonuc.KisilerinSatirlari.ContainsKey(kisi.TckimlikNo))
                                                {
                                                    var kisiaylari = TumKisilerSonuc.KisilerinSatirlari[kisi.TckimlikNo];

                                                    if (kisiaylari.ContainsKey(ayKey))
                                                    {
                                                        var kisiAySatirlari = kisiaylari[ayKey];

                                                        if (kisiAySatirlari.Any(prow => tesvik.CariAydaEksikGunKoduBunlardanBiriIseKisiyeDigerBelgeTurlerideDahilTesvikVerilmeyecek.Contains(prow[(int)Enums.AphbHucreBilgileri.EksikGunSebebi].ToString().PadLeft(2, '0'))))
                                                        {
                                                            tesvikKanunuIstatistik.TesvikAlabilir = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (tesvikKanunuIstatistik.TesvikAlabilir)
                                    {
                                        if (tesvik.GecmisAydaEksikGunKoduBunlardanBiriIseKisiyeDigerBelgeTurlerideDahilTesvikVerilmeyecek.Count > 0)
                                        {

                                            if (! CariAyMi(yilAy))
                                            {
                                                var ayKey = yilAy.Year.ToString() + "-" + yilAy.Month.ToString();

                                                if (TumKisilerSonuc.KisilerinSatirlari.ContainsKey(kisi.TckimlikNo))
                                                {
                                                    var kisiaylari = TumKisilerSonuc.KisilerinSatirlari[kisi.TckimlikNo];

                                                    if (kisiaylari.ContainsKey(ayKey))
                                                    {
                                                        var kisiAySatirlari = kisiaylari[ayKey];

                                                        if (kisiAySatirlari.Any(prow => tesvik.GecmisAydaEksikGunKoduBunlardanBiriIseKisiyeDigerBelgeTurlerideDahilTesvikVerilmeyecek.Contains(prow[(int)Enums.AphbHucreBilgileri.EksikGunSebebi].ToString().PadLeft(2, '0'))))
                                                        {
                                                            tesvikKanunuIstatistik.TesvikAlabilir = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }



                                }
                            }



                            //foreach (var item in AyIcindeYasakliKanunuOlanKisiler)
                            //{
                            //    var kanun = item.Key;

                            //    foreach (var ykItem in item.Value)
                            //    {
                            //        YasakliKanun yk = ykItem.Key;

                            //        var yasakliKanunuOlanKisiler = ykItem.Value;

                            //        if (!yk.KullaniciyaSorulsun || (yk.KullaniciyaSoruldu && !yk.KullaniciCevabi))
                            //        {
                            //            if (yasakliKanunuOlanKisiler.Contains(kisi))
                            //            {
                            //                foreach (var ai in ayIstatistik)
                            //                {
                            //                    ai.Value.TesvikKanunuIstatistikleri[kanun].TesvikAlabilir = false;
                            //                }
                            //            }
                            //        }

                            //    }
                            //}

                        }

                    }



                    #region Yeni Yöntem

                    //Dictionary<string, BazBilgileri> bazBilgileri = Tesvikler.Where(p => p.Value.BasvuruFormuVar).ToDictionary(x => x.Key, x => new BazBilgileri());
                    Dictionary<string, BazBilgileri> bazBilgileri = Tesvikler.ToDictionary(x => x.Key, x => new BazBilgileri());

                    Dictionary<string, List<UnutulanKisi>> UnutulanKisiler = new Dictionary<string, List<UnutulanKisi>>();

                    foreach (var bazbilgisiitem in bazBilgileri)
                    {
                        var kanun = bazbilgisiitem.Key;

                        var bazbilgileri = bazbilgisiitem.Value;

                        if (BasvuruListesiBazGruplari.ContainsKey(kanun))
                        {
                            var bazgruplari = BasvuruListesiBazGruplari[kanun];

                            foreach (var item in bazgruplari)
                            {
                                var baztarihi = item.Key;

                                BazBilgisi bazbilgisi = bazbilgileri.Bul(baztarihi);

                                bazbilgisi.AydaCalisanSayisi = Tesvikler[kanun].AylikCalisanaTaseronDahilEdilsin ? AydaCalisanPersonelSayisiTaseronlu : AydaCalisanPersonelSayisiTaseronsuz;

                                var kiyaslamaTutarlari = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => 0m);

                                item.Value.ForEach(tc => bazbilgisi.KisiEkle(tc, kiyaslamaTutarlari, 0, 0, null));

                                bazbilgileri.Ekle(bazbilgisi);

                            }
                        }
                    }

                    foreach (var item in OncedenTesvikAlanlar)
                    {
                        var kanun = item.Key;

                        var tesvik = Tesvikler[kanun];

                        var KanununOncedenTesvikAlanlari = item.Value;

                        foreach (var kota in KanununOncedenTesvikAlanlari)
                        {
                            var kisi = kota.Key;

                            var kanunIstatistik = kota.Value.First();

                            bool Taseronlu = kanunIstatistik.TaseronluSatirlar.Any(p => new List<string> { "ana işveren", "ana şirket" }.Contains(p.Araci.Trim().ToLower()) == false);

                            BasvuruKisi basvuru = Metodlar.AktifBasvuruKaydiniGetir(kisi, kanun, Yil, Ay);

                            BazBilgisi bazbilgisi = null;

                            if (basvuru == null)
                            {
                                if (kanun.Equals("6645"))
                                {
                                    DataRow row = PasifOlanlar6645.FirstOrDefault(p => p[BasvuruFormlariSutunlari[kanun][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString().Equals(kisi.TckimlikNo));

                                    if (row != null)
                                    {
                                        DateTime dtgiris = Convert.ToDateTime(row[BasvuruFormlariSutunlari[kanun][Enums.BasvuruFormuSutunTurleri.Giris]]);

                                        bazbilgisi = bazBilgileri[kanun].Bul(new DateTime(tesvik.BazYil ? dtgiris.Year : 1, tesvik.BazAy ? dtgiris.Month : 1, 1));
                                    }
                                }

                            }
                            else
                            {
                                bazbilgisi = bazBilgileri[kanun].Bul(new DateTime(tesvik.BazYil ? basvuru.GirisTarihi.Year : 1, tesvik.BazAy ? basvuru.GirisTarihi.Month : 1, 1));
                            }

                            if (bazbilgisi != null)
                            {
                                bazbilgisi.OncedenTesvikAlanEkle(kisi.TckimlikNo, Taseronlu);

                                if (!Taseronlu)
                                {

                                    foreach (var ayistatistikitem in kisi.AyIstatikleri[yilAy])
                                    {
                                        var belgeturuIstatistik2 = ayistatistikitem.Value;

                                        if (belgeturuIstatistik2.TesvikKanunuIstatistikleri[kanun].TesvikAlabilir)
                                        {
                                            if (!UnutulanKisiler.ContainsKey(kisi.TckimlikNo)) UnutulanKisiler.Add(kisi.TckimlikNo, new List<UnutulanKisi>());

                                            var UnutulanKisiBilgileri = UnutulanKisiler[kisi.TckimlikNo];

                                            UnutulanKisiBilgileri.Add(new UnutulanKisi
                                            {
                                                TcKimlikNo = kisi.TckimlikNo,
                                                TesvikTuru = kanun,
                                                BelgeTuru = ayistatistikitem.Key,
                                                BelgeTuruIstatistik = belgeturuIstatistik2,
                                                bazBilgisi = bazbilgisi,
                                                Baz = basvuru.Baz
                                            });
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (BasvuruFormlariSutunlari[kanun].ContainsKey(Enums.BasvuruFormuSutunTurleri.Baz))
                                {

                                    var satir = (tesvik.TaseronunAldigiTesvikKotadanDusulsun ? kanunIstatistik.TaseronluSatirlar : kanunIstatistik.satirlar).FirstOrDefault();

                                    if (satir != null)
                                    {

                                        var t = (tesvik.TaseronunAldigiTesvikKotadanDusulsun ? kisi.TaseronluGirisTarihleri : kisi.GirisTarihleri).Where(p => p.Tarih < new DateTime(Yil, Ay, 1).AddMonths(1) && p.Araci == satir.Araci);

                                        if (t.Count() > 0)
                                        {
                                            DateTime dtgiris = t.Max(p => p.Tarih);

                                            DateTime baztarih = new DateTime(tesvik.BazYil ? dtgiris.Year : 1, tesvik.BazAy ? dtgiris.Month : 1, 1);

                                            BazBilgisi bazbilgisi2 = bazBilgileri[kanun].Bul(baztarih);

                                            bazbilgisi2.AydaCalisanSayisi = tesvik.AylikCalisanaTaseronDahilEdilsin ? AydaCalisanPersonelSayisiTaseronlu : AydaCalisanPersonelSayisiTaseronsuz;

                                            var kiyaslamaTutarlari = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => 0m);

                                            bazbilgisi2.KisiEkle(kisi.TckimlikNo, kiyaslamaTutarlari, 0, 0, null);

                                            bazBilgileri[kanun].Ekle(bazbilgisi2);

                                            bazbilgisi2.OncedenTesvikAlanEkle(kisi.TckimlikNo, Taseronlu);

                                        }
                                    }
                                }
                            }

                        }

                    }

                    var EnazBirTesvigiHakedenler = kisiler
                                                    .Where(k => k.AyIstatikleri.ContainsKey(yilAy) &&
                                                                k.AyIstatikleri[yilAy].Any(p => p.Value.TesvikKanunuIstatistikleri.Any(x => x.Value.TesvikAlabilir)));

                    //var EnazBirTesvigiHakedenler = BasvuruListesindeOlanKisiler
                    //                                .Where(k => k.AyIstatikleri.ContainsKey(yilAy) &&
                    //                                            k.AyIstatikleri[yilAy]
                    //                                            .Any(p => p.Value.TesvikKanunuIstatistikleri.Any(x => x.Value.TesvikAlabilir && Tesvikler[x.Key].BasvuruFormuVar)));

                    foreach (var kisi in EnazBirTesvigiHakedenler)
                    {

                        var icmalSonuclari = new Dictionary<string, Dictionary<string, IcmalHesaplamaResult>>();

                        foreach (var ayIstatistik in kisi.AyIstatikleri[yilAy])
                        {
                            string belgeturu = ayIstatistik.Key;

                            BelgeTuruIstatistikleri belgeTuruIstatistik = ayIstatistik.Value;

                            foreach (var item in belgeTuruIstatistik.TesvikKanunuIstatistikleri)
                            {
                                var kanun = item.Key;

                                var tesvik = Tesvikler[kanun];

                                var tesvikKanunuIstatistik = item.Value;

                                if (tesvikKanunuIstatistik.TesvikAlabilir)
                                {
                                    if (bazBilgileri.ContainsKey(kanun))
                                    {
                                        //var basvuru = Metodlar.AktifBasvuruKaydiniGetir(kisi, kanun, Yil, Ay);

                                        //BazBilgisi bazbilgisi = bazBilgileri[kanun].Bul(new DateTime(tesvik.BazYil ? basvuru.GirisTarihi.Year : 1, tesvik.BazAy ? basvuru.GirisTarihi.Month : 1, 1));

                                        var tesviktutarlari = tesvik.IcmalHesaplama(kisi, Yil, Ay, belgeturu, isyeri.IsyeriSicilNo, true, CariAyMi(yilAy), TumTesvikler,KiyasIcin:true, AsgariUcretDestegiVar : asgariUcretDestegiBuAyVeriliyor);

                                        if (!icmalSonuclari.ContainsKey(belgeturu)) icmalSonuclari.Add(belgeturu,  new Dictionary<string, IcmalHesaplamaResult>() );

                                        icmalSonuclari[belgeturu].Add(kanun, tesviktutarlari);

                                        //if (!tesviktutarlari.ToplamIcmalEkside)
                                        //{
                                        //    if (tesviktutarlari.icmaller.Count > 0)
                                        //    {
                                        //        bool ayIcindeTesvikAlmayaEngelBaskaTesvikVar = false;

                                        //        var tumKanunIstatistikleri = kisi.AyIstatikleri[yilAy].Select(p => p.Value.KanunGunveUcretleri[kanun]);

                                        //        foreach (var kv in tumKanunIstatistikleri)
                                        //        {
                                        //            foreach (var kv2 in kv)
                                        //            {
                                        //                string ayIcindekiKanun = kv2.Key;
                                        //                var kanunIst = kv2.Value;

                                        //                var tesvikAyIcindekiKanun = TumTesvikler.FirstOrDefault(p => p.Key == ayIcindekiKanun.ToInt().ToString() || p.Value.AltKanunlar.Contains(ayIcindekiKanun)).Value;

                                        //                if (tesvikAyIcindekiKanun != null)
                                        //                {
                                        //                    if (tesvikAyIcindekiKanun.Kanun != tesvik.Kanun)
                                        //                    {
                                        //                        if (tesvikAyIcindekiKanun.TesvikVerilirseDigerTesviklerIptalEdilecek)
                                        //                        {
                                        //                            if (!kanunIst.TesvikVerilecek || kanunIst.satirlar.Any(p => !p.TesvikVerilecekMi))
                                        //                            {
                                        //                                ayIcindeTesvikAlmayaEngelBaskaTesvikVar = true;
                                        //                                break;
                                        //                            }
                                        //                        }
                                        //                    }
                                        //                }

                                        //            }

                                        //            if (ayIcindeTesvikAlmayaEngelBaskaTesvikVar) break;
                                        //        }

                                        //        if (!ayIcindeTesvikAlmayaEngelBaskaTesvikVar)
                                        //        {
                                        //            var kiyaslamaTutarlari = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => 0d);

                                        //            foreach (var digerTesvikkv in TumTesvikler)
                                        //            {
                                        //                var digerTesvikKanun = digerTesvikkv.Key;
                                        //                var digerTesvik = digerTesvikkv.Value;

                                        //                if (digerTesvikKanun != kanun)
                                        //                {
                                        //                    if (CariAyMi(yilAy) && digerTesvik.CarideKiyaslamaYaparkenDigerTesviklerMahsupsuzTutarUzerindenKiyaslanacak)
                                        //                    {
                                        //                        kiyaslamaTutarlari[digerTesvikKanun] = tesviktutarlari.icmaller.Sum(p => p.Value[p.Key.DonusturulecekKanunNo].Mahsupsuz);
                                        //                    }
                                        //                    else
                                        //                    {
                                        //                        kiyaslamaTutarlari[digerTesvikKanun] = tesviktutarlari.icmaller.Sum(p => p.Value[p.Key.DonusturulecekKanunNo].MahsupluTutarBagliKanunlarDahil);
                                        //                    }
                                        //                }
                                        //                else kiyaslamaTutarlari[digerTesvikKanun] = tesviktutarlari.icmaller.Sum(p => p.Value[p.Key.DonusturulecekKanunNo].MahsupluTutarBagliKanunlarDahil);

                                        //            }

                                        //            //var tesviktutari = tesviktutarlari.icmaller.Sum(p => p.Value[p.Key.DonusturulecekKanunNo].MahsupluTutarBagliKanunlarDahil);

                                        //            if (!tesvik.TesvikVerilirseDigerTesviklerIptalEdilecek)
                                        //            {
                                        //                if ((tesvik.GunuSifirOlanlaraTesvikVerilsin && tesvik.ToplamUcretiSifirOlanlaraTesvikVerilsin && kiyaslamaTutarlari[kanun] >= 0) || kiyaslamaTutarlari[kanun] > 0)
                                        //                {
                                        //                    bazbilgisi.KisiEkle(kisi, kiyaslamaTutarlari, tesvikKanunuIstatistik.ToplamUcret, tesvikKanunuIstatistik.Gun, belgeturu);
                                        //                }
                                        //            }
                                        //            else
                                        //            {
                                        //                bazbilgisi.KisiEkle(kisi, kiyaslamaTutarlari, tesvikKanunuIstatistik.ToplamUcret, tesvikKanunuIstatistik.Gun, belgeturu);
                                        //            }
                                        //        }
                                        //    }
                                        //}
                                        
                                    }

                                }

                            }
                        }

                        foreach (var ayIstatistik in kisi.AyIstatikleri[yilAy])
                        {
                            string belgeturu = ayIstatistik.Key;

                            BelgeTuruIstatistikleri belgeTuruIstatistik = ayIstatistik.Value;

                            foreach (var item in belgeTuruIstatistik.TesvikKanunuIstatistikleri)
                            {
                                var kanun = item.Key;

                                var tesvik = Tesvikler[kanun];

                                var tesvikKanunuIstatistik = item.Value;

                                if (tesvikKanunuIstatistik.TesvikAlabilir)
                                {
                                    if (bazBilgileri.ContainsKey(kanun))
                                    {
                                        var basvuru = Metodlar.AktifBasvuruKaydiniGetir(kisi, kanun, Yil, Ay);

                                        BazBilgisi bazbilgisi = bazBilgileri[kanun].Bul(new DateTime(tesvik.BazYil ? basvuru.GirisTarihi.Year : 1, tesvik.BazAy ? basvuru.GirisTarihi.Month : 1, 1));

                                        if (icmalSonuclari.ContainsKey(belgeturu))
                                        {
                                            if (icmalSonuclari[belgeturu].ContainsKey(kanun))
                                            {
                                                var tesviktutarlari = icmalSonuclari[belgeturu][kanun];

                                                bool devam = true;

                                                if (tesviktutarlari.KanunlardanBiriBaskaTesvikAlmayiEngelliyor)
                                                {
                                                    decimal toplamIcmalTutari = 0;
                                                    
                                                    foreach (var kv in icmalSonuclari)
                                                    {
                                                        if (kv.Value.ContainsKey(kanun))
                                                        {
                                                            toplamIcmalTutari +=  kv.Value[kanun].tumIcmaller.Sum(p => p.Value[p.Key.DonusturulecekKanunNo].MahsupluTutarBagliKanunlarDahil);
                                                        }
                                                    }

                                                    if (toplamIcmalTutari <= 0) devam = false;
                                                }

                                                if (devam)
                                                {
                                                    if (!tesviktutarlari.ToplamIcmalEkside)
                                                    {

                                                        var icmaller = tesviktutarlari.icmaller.Where(p => p.Value[p.Key.DonusturulecekKanunNo].MahsupluTutarBagliKanunlarDahil > 0 );
                                                        
                                                        var kiyaslamadaDusulmesiGerekenIcmaller= tesviktutarlari.tumIcmaller.Where(p => p.Value[p.Key.DonusturulecekKanunNo].KiyaslamadaMutlakaDusulecekTutar > 0);

                                                        if (icmaller.Count() > 0 || tesvik.TesvikVerilirseDigerTesviklerIptalEdilecek)
                                                        {
                                                            bool ayIcindeTesvikAlmayaEngelBaskaTesvikVar = false;

                                                            var tumKanunIstatistikleri = kisi.AyIstatikleri[yilAy].Select(p => p.Value.KanunGunveUcretleri[kanun]);

                                                            foreach (var kv in tumKanunIstatistikleri)
                                                            {
                                                                foreach (var kv2 in kv)
                                                                {
                                                                    string ayIcindekiKanun = kv2.Key;
                                                                    var kanunIst = kv2.Value;

                                                                    var tesvikAyIcindekiKanun = TumTesvikler.FirstOrDefault(p => p.Key == ayIcindekiKanun.ToInt().ToString() || p.Value.AltKanunlar.Contains(ayIcindekiKanun)).Value;

                                                                    if (tesvikAyIcindekiKanun != null)
                                                                    {
                                                                        if (tesvikAyIcindekiKanun.Kanun != tesvik.Kanun)
                                                                        {
                                                                            if (tesvikAyIcindekiKanun.TesvikVerilirseDigerTesviklerIptalEdilecek)
                                                                            {
                                                                                if (!kanunIst.TesvikVerilecek || kanunIst.satirlar.Any(p => !p.TesvikVerilecekMi))
                                                                                {
                                                                                    ayIcindeTesvikAlmayaEngelBaskaTesvikVar = true;
                                                                                    break;
                                                                                }
                                                                            }
                                                                        }
                                                                    }

                                                                }

                                                                if (ayIcindeTesvikAlmayaEngelBaskaTesvikVar) break;
                                                            }

                                                            if (!ayIcindeTesvikAlmayaEngelBaskaTesvikVar)
                                                            {
                                                                var kiyaslamaTutarlari = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => 0m);

                                                                foreach (var digerTesvikkv in TumTesvikler)
                                                                {
                                                                    var digerTesvikKanun = digerTesvikkv.Key;
                                                                    var digerTesvik = digerTesvikkv.Value;

                                                                    if (digerTesvikKanun != kanun)
                                                                    {
                                                                        if (CariAyMi(yilAy) && digerTesvik.CarideKiyaslamaYaparkenDigerTesviklerMahsupsuzTutarUzerindenKiyaslanacak)
                                                                        {
                                                                            kiyaslamaTutarlari[digerTesvikKanun] = icmaller.Sum(p => p.Value[p.Key.DonusturulecekKanunNo].Mahsupsuz);
                                                                        }
                                                                        else
                                                                        {
                                                                            kiyaslamaTutarlari[digerTesvikKanun] = icmaller.Sum(p => p.Value[p.Key.DonusturulecekKanunNo].MahsupluTutarBagliKanunlarDahil);
                                                                        }
                                                                    }
                                                                    else kiyaslamaTutarlari[digerTesvikKanun] = icmaller.Sum(p => p.Value[p.Key.DonusturulecekKanunNo].MahsupluTutarBagliKanunlarDahil);

                                                                    if (kiyaslamadaDusulmesiGerekenIcmaller.Count() > 0)
                                                                    {
                                                                        kiyaslamaTutarlari[digerTesvikKanun] -= kiyaslamadaDusulmesiGerekenIcmaller.Sum(p => p.Value[p.Key.DonusturulecekKanunNo].KiyaslamadaMutlakaDusulecekTutar);
                                                                    }

                                                                }

                                                                //var tesviktutari = tesviktutarlari.icmaller.Sum(p => p.Value[p.Key.DonusturulecekKanunNo].MahsupluTutarBagliKanunlarDahil);

                                                                if (!tesvik.TesvikVerilirseDigerTesviklerIptalEdilecek)
                                                                {
                                                                    if ((tesvik.GunuSifirOlanlaraTesvikVerilsin && tesvik.ToplamUcretiSifirOlanlaraTesvikVerilsin && kiyaslamaTutarlari[kanun] >= 0) || kiyaslamaTutarlari[kanun] > 0)
                                                                    {
                                                                        bazbilgisi.KisiEkle(kisi, kiyaslamaTutarlari, tesvikKanunuIstatistik, belgeturu);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    bazbilgisi.KisiEkle(kisi, kiyaslamaTutarlari, tesvikKanunuIstatistik, belgeturu);
                                                                }
                                                            }
                                                            else tesvikKanunuIstatistik.TesvikAlabilir = false;
                                                        }
                                                        else tesvikKanunuIstatistik.TesvikAlabilir = false;
                                                    }
                                                    else tesvikKanunuIstatistik.TesvikAlabilir = false;
                                                }
                                                else tesvikKanunuIstatistik.TesvikAlabilir = false; 
                                            }
                                        }

                                        //var tesviktutarlari = tesvik.IcmalHesaplama(kisi, Yil, Ay, belgeturu, isyeri.IsyeriSicilNo, true, CariAyMi(yilAy), TumTesvikler);
                                        
                                    }

                                }

                            }
                        }
                    }

                    List<KisiTesvik> EnKazancliTesvikSayilari = new List<KisiTesvik>();

                    Dictionary<string, List<string>> Yasaklilar = new Dictionary<string, List<string>>();

                    bool BirdenFazlaTesvikVar = false;

                    decimal enFazlaKazanc = 0;

                    int mm = 0;

                    foreach (var bazBilgisiItem in bazBilgileri)
                    {
                        var kanun = bazBilgisiItem.Key;

                        var tesvik = Tesvikler[kanun];

                        if (tesvik.BasvuruFormuVar && !tesvik.Basvuru_Formundaki_Baz_Sayisi_Kadar_Gun_Ay_Icinde_TesvikAlabilir && BasvuruFormlariSutunlari[kanun].ContainsKey(Enums.BasvuruFormuSutunTurleri.Baz))
                        {

                            var kanunBazBilgileri = bazBilgisiItem.Value;

                            foreach (var bazdonem in kanunBazBilgileri.BazveDonemler)
                            {
                                foreach (var bazdonemkisi in bazdonem.Kisiler)
                                {
                                    BasvuruKisi bk = Metodlar.AktifBasvuruKaydiniGetir(bazdonemkisi.Key, kanun, Yil, Ay);

                                    if (bk != null)
                                    {
                                        if (bazdonem.TesvikAlabilecekKisiSayisi <= bk.Baz)
                                        {
                                            if (bazdonemkisi.Key.AyIstatikleri.ContainsKey(yilAy))
                                            {
                                                var ayIstatistik = bazdonemkisi.Key.AyIstatikleri[yilAy];

                                                foreach (var item in ayIstatistik)
                                                {
                                                    item.Value.TesvikKanunuIstatistikleri[kanun].TesvikAlabilir = false;
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }

                    long AsgariUcretDestekTutariGunSayisi = 0;

                    if (AsgariUcretDestekTutarlariDikkateAlinsin)
                    {
                        var asgariUcretBilgisi = isyeri.AsgariUcretDestekTutarlari.FirstOrDefault(p => p.DonemYil.Equals(Yil) && p.DonemAy.Equals(Ay));

                        if (asgariUcretBilgisi != null)
                        {
                            AsgariUcretDestekTutariGunSayisi = asgariUcretBilgisi.HesaplananGun;
                        }
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        List<KisiTesvik> KisiTesvikSayilari = new List<KisiTesvik>();

                        Yasaklilar = new Dictionary<string, List<string>>();

                        BirdenFazlaTesvikVar = false;

                        foreach (var bazBilgisiItem in bazBilgileri)
                        {
                            var kanunBazBilgileri = bazBilgisiItem.Value;

                            var tesvik = Tesvikler[bazBilgisiItem.Key];

                            //if ((i == 1 && tesvik.TekTesvikHakedipGunSayisiAyarlardakiMinimumGunSayisindanBuyukOlanlaraOncelikVerilsin) || tesvik.TesvikMiktariAyniIseUcretiDusukOlanaVerilsin)
                            //{
                            mm = 0;

                            while (mm < kanunBazBilgileri.BazveDonemler.Count)
                            {
                                BazBilgisi bb = kanunBazBilgileri.BazveDonemler[mm];

                                bb.Kisiler = bb.Kisiler.OrderByDescending(p => (
                                                                                    (i == 1 && tesvik.TekTesvikHakedipGunSayisiAyarlardakiMinimumGunSayisindanBuyukOlanlaraOncelikVerilsin)
                                                                                    ?
                                                                                    (
                                                                                        (p.Key.AlinabilecekTesvikSayisi(yilAy, false) == 1 && p.Value.TesvikVerilecekToplamGun >= Program.MinimumGunSayisi)
                                                                                        ? p.Value.TesvikTutarlari[tesvik.Kanun] * 20000
                                                                                        :
                                                                                        (
                                                                                            p.Key.AlinabilecekTesvikSayisi(yilAy, false) > 1
                                                                                            ? p.Value.TesvikTutarlari[tesvik.Kanun] * 10000
                                                                                            : p.Value.TesvikTutarlari[tesvik.Kanun]
                                                                                        )
                                                                                    )
                                                                                    : p.Value.TesvikTutarlari[tesvik.Kanun]
                                                                                )
                                                                           )
                                                                           .ThenBy(p => p.Key.AlinabilecekTesvikSayisi(yilAy, false))
                                                                           .ThenBy(p => tesvik.TesvikMiktariAyniIseUcretiDusukOlanaVerilsin ? p.Value.ToplamUcret : (0 - p.Value.ToplamUcret))
                                                                           .ToList();

                                mm++;
                            }

                            //}

                        }


                        KisiTesvikSayilari = TesvikKisileriGetir(bazBilgileri, Yasaklilar, Yil, Ay, KendiKanunuHaricindekiGunToplami, AsgariUcretDestekTutariGunSayisi);

                        do
                        {
                            BirdenFazlaTesvikVar = false;

                            //KisiTesvikSayilari.Sort((first, next) => {

                            //    var ilkKisi = first.Kisi.TckimlikNo;
                            //    var sonKisi = next.Kisi.TckimlikNo;

                            //    return  
                            //        first.Kanun.EndsWith("7103") && next.Kanun.EndsWith("7252") 
                            //        ? 
                            //        -1 
                            //        : 
                            //            first.Kanun.EndsWith("7252") && next.Kanun.EndsWith("7103") 
                            //            ? 
                            //            1
                            //            :
                            //            next.TesvikTutarlari[first.Kanun].CompareTo(first.TesvikTutarlari[next.Kanun]);
                            //            //next.TesvikTutari.CompareTo(first.TesvikTutari); });
                            //});


                            bool YenidenHesaplanacak = false;

                            int birdenFazlaTesvikHakedenSayisi = KisiTesvikSayilari.GroupBy(p => p.Kisi.TckimlikNo).Count(p => p.Count() > 1);

                            if (birdenFazlaTesvikHakedenSayisi > 0)
                            {

                                var tesvikHakedenler = KisiTesvikSayilari.Select(p => p.Kisi).Distinct();

                                foreach (var kisi in tesvikHakedenler)
                                {

                                    List<KisiTesvik> kisinintesvikleri = KisiTesvikSayilari.Where(p => p.Kisi.TckimlikNo.Equals(kisi.TckimlikNo)).ToList();


                                    if (kisinintesvikleri.Count > 1)
                                    {
                                        kisinintesvikleri.Sort((first, next) =>
                                        {

                                            if (first.Kanun.EndsWith("7103") && next.Kanun.EndsWith("7252"))
                                            {
                                                return -1;
                                            }
                                            else if (first.Kanun.EndsWith("7252") && next.Kanun.EndsWith("7103"))
                                            {
                                                return 1;
                                            }
                                            else if (first.Kanun.EndsWith("7103") && next.Kanun.EndsWith("7256"))
                                            {
                                                return -1;
                                            }
                                            else if (first.Kanun.EndsWith("7256") && next.Kanun.EndsWith("7103"))
                                            {
                                                return 1;
                                            }

                                            else if (first.Kanun.EndsWith("7103") && next.Kanun.EndsWith("7316"))
                                            {
                                                return -1;
                                            }
                                            else if (first.Kanun.EndsWith("7316") && next.Kanun.EndsWith("7103"))
                                            {
                                                return 1;
                                            }
                                            else
                                            {
                                                var nextTutar = next.TesvikTutarlari[first.Kanun];
                                                var firstTutar = first.TesvikTutarlari[next.Kanun];

                                                var sonuc = nextTutar.CompareTo(firstTutar);

                                                return sonuc;
                                            }

                                            //return
                                            //    first.Kanun.EndsWith("7103") && next.Kanun.EndsWith("7252")
                                            //    ?
                                            //    -1
                                            //    :
                                            //        first.Kanun.EndsWith("7252") && next.Kanun.EndsWith("7103")
                                            //        ?
                                            //        1
                                            //        :
                                            //        next.TesvikTutarlari[first.Kanun].CompareTo(first.TesvikTutarlari[next.Kanun]);
                                            //next.TesvikTutari.CompareTo(first.TesvikTutari); });
                                        });

                                        var verilecekKanun = kisinintesvikleri.FirstOrDefault().Kanun;

                                        foreach (var item in Tesvikler)
                                        {
                                            var kanun = item.Key;

                                            if (!item.Key.Equals(verilecekKanun))
                                            {
                                                if (!Yasaklilar.ContainsKey(kanun)) Yasaklilar.Add(kanun, new List<string>());

                                                Yasaklilar[kanun].Add(kisinintesvikleri.FirstOrDefault().Kisi.TckimlikNo);
                                            }
                                        }

                                        YenidenHesaplanacak = true;

                                        break;
                                        //if (birdenFazlaTesvikHakedenSayisi <= 10) break;
                                    }
                                }

                                if (YenidenHesaplanacak)
                                {
                                    KisiTesvikSayilari = TesvikKisileriGetir(bazBilgileri, Yasaklilar, Yil, Ay, KendiKanunuHaricindekiGunToplami, AsgariUcretDestekTutariGunSayisi);

                                    BirdenFazlaTesvikVar = KisiTesvikSayilari.GroupBy(p => p.Kisi.TckimlikNo).Any(x => x.Count() > 1);

                                }

                            }

                        }
                        while (BirdenFazlaTesvikVar);

                        decimal toplamTesvikKazanci = KisiTesvikSayilari.Sum(p => p.TesvikTutarlari[p.Kanun]);

                        if (toplamTesvikKazanci > enFazlaKazanc)
                        {
                            EnKazancliTesvikSayilari = KisiTesvikSayilari;

                            enFazlaKazanc = toplamTesvikKazanci;
                        }
                    }

                    foreach (var kisi in EnazBirTesvigiHakedenler)
                    {

                        List<string> belgeturleri = new List<string>();

                        string Kanun = null;

                        var kisiTesvikBilgileri = EnKazancliTesvikSayilari.FirstOrDefault(p => p.Kisi.Equals(kisi));

                        if (kisiTesvikBilgileri != null)
                        {
                            Kanun = kisiTesvikBilgileri.Kanun;

                            BazBilgileri bazbilgileri = bazBilgileri[Kanun];


                            foreach (var bazbilgisi in bazbilgileri.BazveDonemler)
                            {
                                if (bazbilgisi.KisiBelgeTurleri.ContainsKey(kisi.TckimlikNo))
                                {
                                    belgeturleri = bazbilgisi.KisiBelgeTurleri[kisi.TckimlikNo];

                                    break;
                                }
                            }

                        }

                        foreach (var ayIstatistik in kisi.AyIstatikleri[yilAy])
                        {
                            var belgeturuIstatistik = ayIstatistik.Value;

                            var belgeturu = ayIstatistik.Key;

                            if (!belgeturleri.Contains(belgeturu))
                            {
                                foreach (var item in belgeturuIstatistik.TesvikKanunuIstatistikleri)
                                {
                                    var tesvikKanunIstatistik = item.Value;

                                    if (Tesvikler[item.Key].BasvuruFormuVar)
                                    {
                                        tesvikKanunIstatistik.TesvikAlabilir = false;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var item in belgeturuIstatistik.TesvikKanunuIstatistikleri)
                                {
                                    var tesvikKanunIstatistik = item.Value;

                                    if (!Kanun.Equals(item.Key))
                                    {
                                        tesvikKanunIstatistik.TesvikAlabilir = false;
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #region Önceden Tesvik Bildirimi Eksik Yapılan Kişilere Teşvik Verme

                    foreach (var item in UnutulanKisiler)
                    {
                        foreach (var unutulankisi in item.Value)
                        {
                            var tesvik = Tesvikler[unutulankisi.TesvikTuru];

                            if (! tesvik.TesvikVerilirseDigerTesviklerIptalEdilecek && ! tesvik.Basvuru_Formundaki_Baz_Sayisi_Kadar_Gun_Ay_Icinde_TesvikAlabilir)
                            {
                                if (tesvik.BasvuruFormuVar && BasvuruFormlariSutunlari[tesvik.Kanun].ContainsKey(Enums.BasvuruFormuSutunTurleri.Baz))
                                {

                                    if (unutulankisi.bazBilgisi.AydaCalisanSayisi > (unutulankisi.Baz + unutulankisi.bazBilgisi.OncedenTesvikAlanlar.Where(a => a != unutulankisi.TcKimlikNo).Count()))
                                    {
                                        unutulankisi.BelgeTuruIstatistik.TesvikKanunuIstatistikleri[unutulankisi.TesvikTuru].TesvikAlabilir = true;
                                    }

                                }
                                else
                                {

                                    unutulankisi.BelgeTuruIstatistik.TesvikKanunuIstatistikleri[unutulankisi.TesvikTuru].TesvikAlabilir = true;
                                }
                            }

                        }
                    }

                    #endregion

                    #region Tesvik Listesi Oluşturma

                    ciktilar = new List<Cikti>();

                    var AyIcindekiKanunveBelgeler = TumKisilerSonuc.AySatirlari[yil + "-" + ay].Where(row => row[(int)Enums.AphbHucreBilgileri.Araci].ToString().ToLower().Equals("ana işveren")).Select(row => row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString() + "-" + row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().PadLeft(5, '0')).Distinct();
                    var AyIcindekiKanunveBelgelerOnaysızlarHaric = TumKisilerSonuc.AySatirlari[Yil + "-" + Ay].Where(row => row[(int)Enums.AphbHucreBilgileri.Araci].ToString().ToLower().Equals("ana işveren") && !row[(int)Enums.AphbHucreBilgileri.OnayDurumu].ToString().Trim().Equals("Onaylanmamış")).Select(row => row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString() + "-" + row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().PadLeft(5, '0')).Distinct();

                    var tesvikAlamayanlar = new List<Kisi>();
                    Dictionary<string, List<Kisi>> DonusturulenKisiler6322ve25510Dan = new Dictionary<string, List<Kisi>>();
                    Dictionary<string, List<object>> tesvikVerilmeyenSatirlar = new Dictionary<string, List<object>>();

                    foreach (Kisi kisi in kisiler)
                    {
                        bool TesvikAlindi = false;

                        var tesvikDigerlerininIptalDurumlari = TumTesvikler.ToDictionary(x => x.Key, x => kisi.AyIstatikleri[yilAy].Any(p => p.Value.TesvikKanunuIstatistikleri[x.Key].TesvikAlabilir && x.Value.TesvikVerilirseDigerTesviklerIptalEdilecek));

                        var enumerator = kisi.AyIstatikleri[yilAy].GetEnumerator();

                        while (enumerator.MoveNext())
                        {
                            string belgeturu = enumerator.Current.Key;

                            BelgeTuruIstatistikleri belgeTuruIstatistik = enumerator.Current.Value;

                            foreach (var item in belgeTuruIstatistik.TesvikKanunuIstatistikleri)
                            {
                                var kanun = item.Key;

                                var tesvik = Tesvikler[kanun];

                                var tesvikKanunIstatistik = item.Value;

                                var ciktiKanun = kanun.PadLeft(5, '0');

                                if (tesvik.AltKanunlar.Count > 0)
                                {
                                    if (String.IsNullOrEmpty(tesvik.AltKanun))
                                    {
                                        if (tesvik.BasvuruFormuVar && BasvuruFormlariSutunlari[kanun].ContainsKey(Enums.BasvuruFormuSutunTurleri.KanunNo))
                                        {
                                            var basvuru = Metodlar.AktifBasvuruKaydiniGetir(kisi, kanun, Yil, Ay);

                                            if (basvuru != null)
                                            {
                                                ciktiKanun = basvuru.Kanun.PadLeft(5, '0');
                                            }
                                        }
                                    }
                                    else ciktiKanun = tesvik.AltKanun.PadLeft(5, '0');
                                }

                                if (tesvikKanunIstatistik.TesvikAlabilir)
                                {
                                    List<Tesvik> tesvikIstatistikleri = new List<Tesvik>();

                                    tesvikIstatistikleri.Add(tesvik);

                                    foreach (var altTesvikKanunNo in tesvik.altTesvikler)
                                    {
                                        tesvikIstatistikleri.Add(Tesvikler[altTesvikKanunNo]);
                                    }

                                    TesvikAlindi = true;

                                    int tesvikVerilecekToplamGun = 0;
                                    int toplamGun = 0;
                                    decimal toplamUcret = 0;

                                    var cikti = ciktilar.FirstOrDefault(p => p.BelgeTuru.Equals(belgeturu) && p.Kanun.Equals(ciktiKanun) && !p.Iptal);

                                    if (cikti == null)
                                    {
                                        cikti = new Cikti();
                                        cikti.BelgeTuru = belgeturu;
                                        cikti.Kanun = ciktiKanun;
                                        cikti.Asil = !AyIcindekiKanunveBelgeler.Contains(belgeturu + "-" + ciktiKanun);
                                        cikti.Iptal = false;
                                        cikti.XmlOlustur = enbuyukay.CompareTo(yilAy) == 0;
                                        ciktilar.Add(cikti);

                                        if (kanun.Equals("687"))
                                        {
                                            var CarpimOrani687 = TesvikHesaplamaSabitleri.CarpimOrani687;

                                            if (tesvik.dtKurulusTarihi >= new DateTime(2017, 1, 1) || !tesvik.BildirgeOlanYillar.Contains(2016)) CarpimOrani687 = CarpimOrani687 / 2;

                                            cikti.EkBilgiler = string.IsNullOrEmpty(cikti.EkBilgiler) ? "CarpimOrani687=" + CarpimOrani687.ToString() : cikti.EkBilgiler + ";CarpimOrani687=" + CarpimOrani687.ToString();
                                        }
                                    }

                                    if (!cikti.Kisiler.ContainsKey(kisi)) cikti.Kisiler.Add(kisi, kanun);

                                    if (tesvik.YasakliKanunlar.Count > 0)
                                    {
                                        var sorulacakkanunlar = AyIcindeYasakliKanunuOlanKisiler[kanun].Where(p => p.Key.KullaniciyaSorulsun && !p.Key.KullaniciyaSoruldu && p.Value.Contains(kisi)).Select(p => p.Key);

                                        foreach (var sk in sorulacakkanunlar)
                                        {
                                            if (!sorulacakYasakliKanunlar.Any(p => p.Kanun.Equals(sk.Kanun))) sorulacakYasakliKanunlar.Add(sk);
                                        }
                                    }

                                    var verilebilecekGunSayisi7252 = -1;

                                    foreach (var kgu in belgeTuruIstatistik.KanunGunveUcretleri[kanun])
                                    {
                                        var KanunIstatistik = kgu.Value;

                                        var AyIcindekiKanun = kgu.Key;

                                        bool kanunSatirlarininGeregiYapildi = false;

                                        if (tesvik.DonusturulecekKanunlar.ContainsKey(AyIcindekiKanun))
                                        {
                                            if (!KanunIstatistik.AraciMi && KanunIstatistik.TesvikVerilecek)
                                            {
                                                List<AphbSatir> ciktiyaEklenecekSatirlar = new List<AphbSatir>();
                                                List<AphbSatir> iptalEdilecekSatirlar = new List<AphbSatir>();

                                                foreach (var satir in KanunIstatistik.satirlar)
                                                {
                                                    if (satir.TesvikVerilecekMi)
                                                    {
                                                        //if (satir.satirBolunecek)
                                                        //{
                                                        //    foreach (var boluneceksatir in satir.BolunecekSatirlar)
                                                        //    {
                                                        //        if (boluneceksatir.TesvikVerilecekMi)
                                                        //        {
                                                        //            ciktiyaEklenecekSatirlar.Add(boluneceksatir);

                                                        //            //var ciktiVerilenSatirAsilEk = ciktilar.FirstOrDefault(p => p.BelgeTuru.Equals(belgeturu) && p.Kanun.Equals(AyIcindekiKanun) && !p.Iptal);

                                                        //            //if (ciktiVerilenSatirAsilEk == null)
                                                        //            //{
                                                        //            //    ciktiVerilenSatirAsilEk = new Cikti();
                                                        //            //    ciktiVerilenSatirAsilEk.BelgeTuru = belgeturu;
                                                        //            //    ciktiVerilenSatirAsilEk.Kanun = ciktiKanun;
                                                        //            //    ciktiVerilenSatirAsilEk.Asil = !AyIcindekiKanunveBelgeler.Contains(belgeturu + "-" + AyIcindekiKanun);
                                                        //            //    ciktiVerilenSatirAsilEk.Iptal = false;
                                                        //            //    ciktiVerilenSatirAsilEk.XmlOlustur = enbuyukay.CompareTo(yilAy) == 0;
                                                        //            //    ciktilar.Add(ciktiVerilenSatirAsilEk);

                                                        //            //    ciktiVerilenSatirAsilEk.satirlar.Add(boluneceksatir);
                                                        //            //}
                                                        //        }
                                                        //        else
                                                        //        {
                                                        //            var ciktiVerilmeyenSatirAsilEk = ciktilar.FirstOrDefault(p => p.BelgeTuru.Equals(belgeturu) && p.Kanun.Equals("00000") && !p.Iptal);

                                                        //            if (ciktiVerilmeyenSatirAsilEk == null)
                                                        //            {
                                                        //                ciktiVerilmeyenSatirAsilEk = new Cikti();
                                                        //                ciktiVerilmeyenSatirAsilEk.BelgeTuru = belgeturu;
                                                        //                ciktiVerilmeyenSatirAsilEk.Kanun = "00000";
                                                        //                ciktiVerilmeyenSatirAsilEk.Asil = CariAy == yilAy ? true : !AyIcindekiKanunveBelgeler.Contains(belgeturu + "-00000");
                                                        //                ciktiVerilmeyenSatirAsilEk.Iptal = false;
                                                        //                ciktiVerilmeyenSatirAsilEk.XmlOlustur = enbuyukay.CompareTo(yilAy) == 0;
                                                        //                ciktilar.Add(ciktiVerilmeyenSatirAsilEk);
                                                        //            }

                                                        //            ciktiVerilmeyenSatirAsilEk.satirlar.Add(boluneceksatir);

                                                        //            ciktiVerilmeyenSatirAsilEk.Matrah += Convert.ToDouble(boluneceksatir.Ucret.ToDecimalSgk() + boluneceksatir.Ikramiye.ToDecimalSgk());
                                                        //            ciktiVerilmeyenSatirAsilEk.Gun += boluneceksatir.Gun.ToInt();
                                                        //        }
                                                        //    }
                                                        //}
                                                        //else
                                                        //{
                                                        ciktiyaEklenecekSatirlar.Add(satir);
                                                        //}

                                                        if (tesvik.UyariVerilecekEksikGunNedenleri.Contains(satir.EksikGunNedeni.Trim().PadLeft(2, '0')))
                                                        {
                                                            if (!ay_Icinde_Tesvik_Alip_Eksik_Gun_Kodundan_Uyari_Verilmesi_Gereken_Kisiler[tesvik.Kanun].ContainsKey(satir.SosyalGuvenlikNo))
                                                            {
                                                                ay_Icinde_Tesvik_Alip_Eksik_Gun_Kodundan_Uyari_Verilmesi_Gereken_Kisiler[tesvik.Kanun].Add(satir.SosyalGuvenlikNo, "");
                                                            }
                                                            ay_Icinde_Tesvik_Alip_Eksik_Gun_Kodundan_Uyari_Verilmesi_Gereken_Kisiler[tesvik.Kanun][satir.SosyalGuvenlikNo] += (satir.EksikGunNedeni + ",");
                                                        }

                                                        if (satir.Kanun.EndsWith("7252"))
                                                        {
                                                            if (verilebilecekGunSayisi7252 == -1)
                                                            {
                                                                verilebilecekGunSayisi7252 = kisi.KisininAlabilecegiGunSayisiniBul("7252", belgeturu, satir.Gun.ToInt(), Yil, Ay, isyeri.IsyeriSicilNo);
                                                            }

                                                            if (satir.Gun.ToInt() < verilebilecekGunSayisi7252)
                                                            {
                                                                satir.DonusturulecekHesaplanacakGun = satir.Gun;
                                                                verilebilecekGunSayisi7252 -= satir.Gun.ToInt();
                                                            }
                                                            else
                                                            {
                                                                satir.DonusturulecekHesaplanacakGun = verilebilecekGunSayisi7252.ToString();
                                                                verilebilecekGunSayisi7252 = 0;
                                                            }

                                                        }

                                                        cikti.muhtasarSatirlar.Add(satir);

                                                        cikti.Gun_Tesvik_Verilmeyenler_Dahil += satir.Gun.ToInt();
                                                        cikti.Matrah_Tesvik_Verilmeyenler_Dahil += satir.Ucret.ToDecimalSgk() + satir.Ikramiye.ToDecimalSgk();

                                                        iptalEdilecekSatirlar.Add(satir);
                                                    }
                                                    else
                                                    {
                                                        if (tesvik.TesvikVerilirseDigerTesviklerIptalEdilecek)
                                                        {

                                                            var verilmeyenSatirKanun = tesvik.Kanun == "7252" ? "07252" : "00000";

                                                            if (verilmeyenSatirKanun != AyIcindekiKanun)
                                                            {

                                                                cikti.muhtasarIptalSatirlar.Add(satir);

                                                                var ciktiVerilmeyenSatirAsilEk = ciktilar.FirstOrDefault(p => p.BelgeTuru.Equals(belgeturu) && p.Kanun.Equals(verilmeyenSatirKanun) && !p.Iptal);

                                                                if (ciktiVerilmeyenSatirAsilEk == null)
                                                                {
                                                                    ciktiVerilmeyenSatirAsilEk = new Cikti();
                                                                    ciktiVerilmeyenSatirAsilEk.BelgeTuru = belgeturu;
                                                                    ciktiVerilmeyenSatirAsilEk.Kanun = verilmeyenSatirKanun;
                                                                    ciktiVerilmeyenSatirAsilEk.Asil = CariAyMi(yilAy) ? true : !AyIcindekiKanunveBelgeler.Contains(belgeturu + "-" + verilmeyenSatirKanun);
                                                                    ciktiVerilmeyenSatirAsilEk.Iptal = false;
                                                                    ciktiVerilmeyenSatirAsilEk.XmlOlustur = enbuyukay.CompareTo(yilAy) == 0;

                                                                    ciktilar.Add(ciktiVerilmeyenSatirAsilEk);
                                                                }

                                                                satir.TesvikVerilecekMi = false;
                                                                satir.TesvikHesaplanacakGun = "0";

                                                                if (verilmeyenSatirKanun != "00000")
                                                                    if (!ciktiVerilmeyenSatirAsilEk.Kisiler.ContainsKey(kisi)) ciktiVerilmeyenSatirAsilEk.Kisiler.Add(kisi, verilmeyenSatirKanun);

                                                                ciktiVerilmeyenSatirAsilEk.satirlar.Add(satir);

                                                                if (verilmeyenSatirKanun != "07252")
                                                                    ciktiVerilmeyenSatirAsilEk.MinimumTutarKontrolEdilecek = false;

                                                                if (tesvik.Kanun != "7252")
                                                                {
                                                                    ciktiVerilmeyenSatirAsilEk.Matrah += satir.Ucret.ToDecimalSgk() + satir.Ikramiye.ToDecimalSgk();
                                                                    ciktiVerilmeyenSatirAsilEk.Gun += satir.Gun.ToInt();
                                                                }
                                                                
                                                                ciktiVerilmeyenSatirAsilEk.Gun_Tesvik_Verilmeyenler_Dahil += satir.Gun.ToInt();
                                                                ciktiVerilmeyenSatirAsilEk.Matrah_Tesvik_Verilmeyenler_Dahil += satir.Ucret.ToDecimalSgk() + satir.Ikramiye.ToDecimalSgk();

                                                                iptalEdilecekSatirlar.Add(satir);
                                                            }

                                                        }
                                                        else
                                                        {
                                                            satir.TesvikHesaplanacakGun = "0";

                                                            if (!tesvikVerilmeyenSatirlar.ContainsKey(kisi.TckimlikNo))
                                                            {
                                                                tesvikVerilmeyenSatirlar.Add(kisi.TckimlikNo, new List<object>());
                                                            }

                                                            tesvikVerilmeyenSatirlar[kisi.TckimlikNo].Add(satir);
                                                        }
                                                    }
                                                }

                                                //var ciktisatirlar = KanunIstatistik.satirlar.Where(p => p.TesvikVerilecekMi);

                                                cikti.satirlar.AddRange(ciktiyaEklenecekSatirlar);

                                                bool devam = true;

                                                if (AyIcindekiKanun.Equals("05510") || AyIcindekiKanun.EndsWith("6486"))
                                                {
                                                    if (IptalXmlveTesvikAlamayanXmlCikartilmayacak) devam = false;
                                                }

                                                if (devam)
                                                {

                                                    var ciktiIptal = ciktilar.FirstOrDefault(p => p.BelgeTuru.Equals(belgeturu) && p.Kanun.Equals(AyIcindekiKanun) && p.Iptal);

                                                    if (ciktiIptal == null)
                                                    {
                                                        ciktiIptal = new Cikti();
                                                        ciktiIptal.BelgeTuru = belgeturu;
                                                        ciktiIptal.Kanun = AyIcindekiKanun;
                                                        ciktiIptal.Asil = false;
                                                        ciktiIptal.Iptal = true;
                                                        ciktiIptal.XmlOlustur = false;
                                                        ciktilar.Add(ciktiIptal);
                                                    }

                                                    ciktiIptal.satirlar.AddRange(iptalEdilecekSatirlar);

                                                    ciktiIptal.Matrah += KanunIstatistik.TesvikVerilecekToplamUcret;
                                                    ciktiIptal.Gun += KanunIstatistik.TesvikVerilecekGun;

                                                    foreach (var satir in iptalEdilecekSatirlar)
                                                    {
                                                        ciktiIptal.Gun_Tesvik_Verilmeyenler_Dahil += satir.Gun.ToInt();
                                                        ciktiIptal.Matrah_Tesvik_Verilmeyenler_Dahil += satir.Ucret.ToDecimalSgk() + satir.Ikramiye.ToDecimalSgk();

                                                        if (TesvikHesaplamaSabitleri.AsgariUcretDestegiKapsamiDisindakiKanunlar.Contains(AyIcindekiKanun))
                                                        {
                                                            asgariUcretDestegiKapsamiDisindaTesvikVerilenGunSayisi -= satir.Gun.ToInt();
                                                        }
                                                    }
                                                }

                                                tesvikVerilecekToplamGun += KanunIstatistik.TesvikVerilecekGun;
                                                toplamUcret += KanunIstatistik.TesvikVerilecekToplamUcret;
                                                toplamGun += KanunIstatistik.Gun;


                                                cikti.Matrah += KanunIstatistik.TesvikVerilecekToplamUcret;
                                                cikti.Gun += KanunIstatistik.TesvikVerilecekGun;



                                                if (AyIcindekiKanun.Equals("16322") || AyIcindekiKanun.Equals("26322") || AyIcindekiKanun.Equals("25510"))
                                                {

                                                    if (!DonusturulenKisiler6322ve25510Dan.ContainsKey(AyIcindekiKanun)) DonusturulenKisiler6322ve25510Dan.Add(AyIcindekiKanun, new List<Kisi>());

                                                    if (!DonusturulenKisiler6322ve25510Dan[AyIcindekiKanun].Contains(kisi))
                                                    {
                                                        DonusturulenKisiler6322ve25510Dan[AyIcindekiKanun].Add(kisi);
                                                    }

                                                }
                                                else
                                                {
                                                    var orijinalKanunlar = KanunIstatistik.satirlar.Where(p => p.TesvikVerilecekMi).Select(p => p.OrijinalKanun);

                                                    var kno = orijinalKanunlar.FirstOrDefault(p => p.Equals("16322") || p.Equals("26322") || p.Equals("25510"));

                                                    if (kno != null)
                                                    {
                                                        if (!DonusturulenKisiler6322ve25510Dan.ContainsKey(kno)) DonusturulenKisiler6322ve25510Dan.Add(kno, new List<Kisi>());

                                                        if (!DonusturulenKisiler6322ve25510Dan[kno].Contains(kisi))
                                                        {
                                                            DonusturulenKisiler6322ve25510Dan[kno].Add(kisi);
                                                        }
                                                    }
                                                }

                                                kanunSatirlarininGeregiYapildi = true;
                                            }
                                        }

                                        
                                        if (!kanunSatirlarininGeregiYapildi && !KanunIstatistik.AraciMi)
                                        {
                                            if (tesvik.TesvikVerilirseDigerTesviklerIptalEdilecek)
                                            {

                                                var verilmeyenSatirKanun = tesvik.Kanun == "7252" ? "07252" : "00000";

                                                if (verilmeyenSatirKanun != AyIcindekiKanun)
                                                {

                                                    var ciktiIptal = ciktilar.FirstOrDefault(p => p.BelgeTuru.Equals(belgeturu) && p.Kanun.Equals(AyIcindekiKanun) && p.Iptal);

                                                    if (ciktiIptal == null)
                                                    {
                                                        ciktiIptal = new Cikti();
                                                        ciktiIptal.BelgeTuru = belgeturu;
                                                        ciktiIptal.Kanun = AyIcindekiKanun;
                                                        ciktiIptal.Asil = false;
                                                        ciktiIptal.Iptal = true;
                                                        ciktiIptal.XmlOlustur = false;
                                                        ciktilar.Add(ciktiIptal);
                                                    }

                                                    ciktiIptal.satirlar.AddRange(KanunIstatistik.satirlar);

                                                    cikti.muhtasarIptalSatirlar.AddRange(KanunIstatistik.satirlar);

                                                    toplamGun += KanunIstatistik.Gun;

                                                    ciktiIptal.Matrah += KanunIstatistik.ToplamUcret;
                                                    ciktiIptal.Gun += KanunIstatistik.Gun;

                                                    ciktiIptal.Matrah_Tesvik_Verilmeyenler_Dahil += KanunIstatistik.ToplamUcret;
                                                    ciktiIptal.Gun_Tesvik_Verilmeyenler_Dahil += KanunIstatistik.Gun;

                                                    if (TesvikHesaplamaSabitleri.AsgariUcretDestegiKapsamiDisindakiKanunlar.Contains(AyIcindekiKanun))
                                                    {
                                                        asgariUcretDestegiKapsamiDisindaTesvikVerilenGunSayisi -= KanunIstatistik.Gun;
                                                    }

                                                    var ciktiVerilmeyenSatirAsilEk = ciktilar.FirstOrDefault(p => p.BelgeTuru.Equals(belgeturu) && p.Kanun.Equals(verilmeyenSatirKanun) && !p.Iptal);

                                                    if (ciktiVerilmeyenSatirAsilEk == null)
                                                    {
                                                        ciktiVerilmeyenSatirAsilEk = new Cikti();
                                                        ciktiVerilmeyenSatirAsilEk.BelgeTuru = belgeturu;
                                                        ciktiVerilmeyenSatirAsilEk.Kanun = verilmeyenSatirKanun;
                                                        ciktiVerilmeyenSatirAsilEk.Asil = CariAyMi(yilAy) ? true : !AyIcindekiKanunveBelgeler.Contains(belgeturu + "-" + verilmeyenSatirKanun);
                                                        ciktiVerilmeyenSatirAsilEk.Iptal = false;
                                                        ciktiVerilmeyenSatirAsilEk.XmlOlustur = enbuyukay.CompareTo(yilAy) == 0;

                                                        ciktilar.Add(ciktiVerilmeyenSatirAsilEk);
                                                    }

                                                    //if (verilmeyenSatirKanun != "00000")
                                                        if (!ciktiVerilmeyenSatirAsilEk.Kisiler.ContainsKey(kisi)) ciktiVerilmeyenSatirAsilEk.Kisiler.Add(kisi, verilmeyenSatirKanun);

                                                    KanunIstatistik.satirlar.ForEach(p =>
                                                    {
                                                        p.TesvikVerilecekMi = false;
                                                        p.TesvikHesaplanacakGun = "0";
                                                    });

                                                    ciktiVerilmeyenSatirAsilEk.satirlar.AddRange(KanunIstatistik.satirlar);
                                                    ciktiVerilmeyenSatirAsilEk.MinimumTutarKontrolEdilecek = false;

                                                    if (tesvik.Kanun != "7252")
                                                    {
                                                        ciktiVerilmeyenSatirAsilEk.Matrah += KanunIstatistik.ToplamUcret;
                                                        ciktiVerilmeyenSatirAsilEk.Gun += KanunIstatistik.Gun;
                                                    }

                                                    ciktiVerilmeyenSatirAsilEk.Matrah_Tesvik_Verilmeyenler_Dahil += KanunIstatistik.ToplamUcret;
                                                    ciktiVerilmeyenSatirAsilEk.Gun_Tesvik_Verilmeyenler_Dahil += KanunIstatistik.Gun;

                                                    if (AyIcindekiKanun.Equals("16322") || AyIcindekiKanun.Equals("26322") || AyIcindekiKanun.Equals("25510"))
                                                    {

                                                        if (!DonusturulenKisiler6322ve25510Dan.ContainsKey(AyIcindekiKanun)) DonusturulenKisiler6322ve25510Dan.Add(AyIcindekiKanun, new List<Kisi>());

                                                        if (!DonusturulenKisiler6322ve25510Dan[AyIcindekiKanun].Contains(kisi))
                                                        {
                                                            DonusturulenKisiler6322ve25510Dan[AyIcindekiKanun].Add(kisi);
                                                        }

                                                    }
                                                    else
                                                    {
                                                        var orijinalKanunlar = KanunIstatistik.satirlar.Where(p => p.TesvikVerilecekMi).Select(p => p.OrijinalKanun);

                                                        var kno = orijinalKanunlar.FirstOrDefault(p => p.Equals("16322") || p.Equals("26322") || p.Equals("25510"));

                                                        if (kno != null)
                                                        {
                                                            if (!DonusturulenKisiler6322ve25510Dan.ContainsKey(kno)) DonusturulenKisiler6322ve25510Dan.Add(kno, new List<Kisi>());

                                                            if (!DonusturulenKisiler6322ve25510Dan[kno].Contains(kisi))
                                                            {
                                                                DonusturulenKisiler6322ve25510Dan[kno].Add(kisi);
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                        
                                    }

                                    foreach (var tesvik2 in tesvikIstatistikleri)
                                    {

                                        bool altTesvikMi = !tesvik2.Kanun.Equals(kanun);

                                        var kisininIcmalleri = tesvik2.IcmalHesaplama(kisi, Yil, Ay, belgeturu, isyeri.IsyeriSicilNo, false, CariAyMi(yilAy), TumTesvikler, altTesvikMi, altTesvikMi ? TumTesvikler[kanun] : null, false, asgariUcretDestegiBuAyVeriliyor);

                                        TesvikAyIstatistik tesvikAyIstatistik = null;

                                        foreach (var kisiIcmaller in kisininIcmalleri.icmaller)
                                        {
                                            var donusturulenKanun = kisiIcmaller.Key;

                                            var donusturulenKanunIcmalTutarlari = kisiIcmaller.Value;

                                            var donusturulenKanunNo = donusturulenKanun.DonusturulecekKanunNo;

                                            if (!tesvik2.DonusenlerIcmaldenDusulsun) donusturulenKanunNo = "00000";

                                            if (donusturulenKanunIcmalTutarlari[donusturulenKanunNo].MahsupluTutarBagliKanunlarDahil > 0)
                                            {
                                                if (!tesvik2.TesvikAyIstatistikleri.ContainsKey(yilAy)) tesvik2.TesvikAyIstatistikleri.Add(yilAy, new TesvikAyIstatistik());

                                                tesvikAyIstatistik = tesvik2.TesvikAyIstatistikleri[yilAy];

                                                if (!tesvikAyIstatistik.Icmal.Tutarlar.ContainsKey(donusturulenKanunNo)) tesvikAyIstatistik.Icmal.Tutarlar.Add(donusturulenKanunNo, 0);
                                                tesvikAyIstatistik.Icmal.Tutarlar[donusturulenKanunNo] += donusturulenKanunIcmalTutarlari[donusturulenKanunNo].MahsupluTutarBagliKanunlarDahil;

                                                foreach (var dkit in donusturulenKanunIcmalTutarlari)
                                                {
                                                    if (!tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupluTutarlar.ContainsKey(dkit.Key)) tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupluTutarlar.Add(dkit.Key, 0);

                                                    tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupluTutarlar[dkit.Key] += dkit.Value.MahsupluTutarBagliKanunlarDahil;

                                                    if (!tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupTutarlari.ContainsKey(dkit.Key)) tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupTutarlari.Add(dkit.Key, 0);

                                                    tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupTutarlari[dkit.Key] += dkit.Value.MahsupMiktariBagliKanunlarHaric;
                                                }
                                            }
                                        }

                                        if (tesvikAyIstatistik != null)
                                        {
                                            tesvikAyIstatistik.TesvikAlacakVar = true;
                                            tesvikAyIstatistik.Icmal.Matrah += toplamUcret;
                                            tesvikAyIstatistik.Icmal.PrimOdenenGunSayisi += tesvikVerilecekToplamGun;

                                            if (!tesvik2.AsgariUcretDestegiKapsaminda)
                                            {
                                                asgariUcretDestegiKapsamiDisindaTesvikVerilenGunSayisi += toplamGun;
                                            }

                                            if (!tesvikAyIstatistik.TesvikAlanKisiler.Contains(kisi)) tesvikAyIstatistik.TesvikAlanKisiler.Add(kisi);

                                            cikti.ToplamTutar += kisininIcmalleri.icmaller.Sum(p => p.Value[p.Key.DonusturulecekKanunNo].MahsupluTutarBagliKanunlarDahil);
                                        }

                                    }

                                    if (cikti.satirlar.Any(p => p.Kanun.EndsWith("687")))
                                    {
                                        var CarpimOrani687 = TesvikHesaplamaSabitleri.CarpimOrani687;

                                        if (tesvik.dtKurulusTarihi >= new DateTime(2017, 1, 1) || !tesvik.BildirgeOlanYillar.Contains(2016)) CarpimOrani687 = CarpimOrani687 / 2;

                                        if (cikti.EkBilgiler == null || !cikti.EkBilgiler.Contains("CarpimOrani687"))
                                        {
                                            cikti.EkBilgiler = string.IsNullOrEmpty(cikti.EkBilgiler) ? "CarpimOrani687=" + CarpimOrani687.ToString() : cikti.EkBilgiler + ";CarpimOrani687=" + CarpimOrani687.ToString();
                                        }
                                    }

                                    break;
                                }
                                else
                                {
                                    
                                    if (tesvikDigerlerininIptalDurumlari[kanun])
                                    {
                                        foreach (var kgu in belgeTuruIstatistik.KanunGunveUcretleri[kanun])
                                        {
                                            var KanunIstatistik = kgu.Value;

                                            var AyIcindekiKanun = kgu.Key;

                                            if (!KanunIstatistik.AraciMi)
                                            {
                                                var verilmeyenSatirKanun = tesvik.Kanun == "7252" && AyIcindekiKanun == "07252" ? "07252" : "00000";
                                                //var verilmeyenSatirKanun = "00000";

                                                if (verilmeyenSatirKanun != AyIcindekiKanun)
                                                {

                                                    var ciktiIptal = ciktilar.FirstOrDefault(p => p.BelgeTuru.Equals(belgeturu) && p.Kanun.Equals(AyIcindekiKanun) && p.Iptal);

                                                    if (ciktiIptal == null)
                                                    {
                                                        ciktiIptal = new Cikti();
                                                        ciktiIptal.BelgeTuru = belgeturu;
                                                        ciktiIptal.Kanun = AyIcindekiKanun;
                                                        ciktiIptal.Asil = false;
                                                        ciktiIptal.Iptal = true;
                                                        ciktiIptal.XmlOlustur = false;
                                                        ciktilar.Add(ciktiIptal);
                                                    }

                                                    ciktiIptal.satirlar.AddRange(KanunIstatistik.satirlar);

                                                    var cikti = ciktilar.FirstOrDefault(p => p.Kanun.Equals(ciktiKanun) && !p.Iptal);

                                                    if (cikti != null)
                                                    {
                                                        cikti.muhtasarIptalSatirlar.AddRange(KanunIstatistik.satirlar);
                                                    }

                                                    ciktiIptal.Matrah += KanunIstatistik.ToplamUcret;
                                                    ciktiIptal.Gun += KanunIstatistik.Gun;

                                                    ciktiIptal.Matrah_Tesvik_Verilmeyenler_Dahil += KanunIstatistik.ToplamUcret;
                                                    ciktiIptal.Gun_Tesvik_Verilmeyenler_Dahil += KanunIstatistik.Gun;

                                                    var ciktiVerilmeyenSatirAsilEk = ciktilar.FirstOrDefault(p => p.BelgeTuru.Equals(belgeturu) && p.Kanun.Equals(verilmeyenSatirKanun) && !p.Iptal);

                                                    if (ciktiVerilmeyenSatirAsilEk == null)
                                                    {
                                                        ciktiVerilmeyenSatirAsilEk = new Cikti();
                                                        ciktiVerilmeyenSatirAsilEk.BelgeTuru = belgeturu;
                                                        ciktiVerilmeyenSatirAsilEk.Kanun = verilmeyenSatirKanun;
                                                        ciktiVerilmeyenSatirAsilEk.Asil = CariAyMi(yilAy) ? true : !AyIcindekiKanunveBelgeler.Contains(belgeturu + "-" + verilmeyenSatirKanun);
                                                        ciktiVerilmeyenSatirAsilEk.Iptal = false;
                                                        ciktiVerilmeyenSatirAsilEk.XmlOlustur = enbuyukay.CompareTo(yilAy) == 0;

                                                        ciktilar.Add(ciktiVerilmeyenSatirAsilEk);
                                                    }

                                                    List<Tesvik> tesvikIstatistikleri = new List<Tesvik>();

                                                    tesvikIstatistikleri.Add(tesvik);

                                                    foreach (var altTesvikKanunNo in tesvik.altTesvikler)
                                                    {
                                                        tesvikIstatistikleri.Add(Tesvikler[altTesvikKanunNo]);
                                                    }

                                                    foreach (var tesvik2 in tesvikIstatistikleri)
                                                    {
                                                        bool altTesvikMi = !tesvik2.Kanun.Equals(kanun);

                                                        TesvikAyIstatistik tesvikAyIstatistik = null;

                                                        var donusturulenKanunNo = AyIcindekiKanun;

                                                        if (!tesvik2.DonusenlerIcmaldenDusulsun) donusturulenKanunNo = "00000";
                                                       
                                                        if (!tesvik2.TesvikAyIstatistikleri.ContainsKey(yilAy)) tesvik2.TesvikAyIstatistikleri.Add(yilAy, new TesvikAyIstatistik());

                                                        tesvikAyIstatistik = tesvik2.TesvikAyIstatistikleri[yilAy];

                                                        var dusulecekKanunlar= DonusturulecekKanun.DusulecekMiktarHesapla(AyIcindekiKanun, KanunIstatistik.Gun, KanunIstatistik.ToplamUcret, Yil, Ay, belgeturu, isyeri.IsyeriSicilNo, true, TumTesvikler);

                                                        if (!tesvikAyIstatistik.Icmal.Tutarlar.ContainsKey(donusturulenKanunNo)) tesvikAyIstatistik.Icmal.Tutarlar.Add(donusturulenKanunNo, 0);
                                                        tesvikAyIstatistik.Icmal.Tutarlar[donusturulenKanunNo] -= dusulecekKanunlar[donusturulenKanunNo].BagliKanunlarDahilDusulecekTutar;


                                                        foreach (var dkit in dusulecekKanunlar)
                                                        {
                                                            if (!tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupluTutarlar.ContainsKey(dkit.Key)) tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupluTutarlar.Add(dkit.Key, 0);

                                                            tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupluTutarlar[dkit.Key] -= dkit.Value.BagliKanunlarDahilDusulecekTutar;

                                                            if (!tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupTutarlari.ContainsKey(dkit.Key)) tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupTutarlari.Add(dkit.Key, 0);

                                                            tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupTutarlari[dkit.Key] += Metodlar.TesvikTutariHesapla(dkit.Key, KanunIstatistik.Gun, KanunIstatistik.ToplamUcret, Yil,Ay,belgeturu, isyeri.IsyeriSicilNo, TumTesvikler);
                                                        }

                                                    }

                                                    //if (verilmeyenSatirKanun != "00000")
                                                    if (!ciktiVerilmeyenSatirAsilEk.Kisiler.ContainsKey(kisi)) ciktiVerilmeyenSatirAsilEk.Kisiler.Add(kisi, verilmeyenSatirKanun);


                                                    KanunIstatistik.satirlar.ForEach(p =>
                                                    {
                                                        p.TesvikVerilecekMi = false;
                                                        p.TesvikHesaplanacakGun = "0";
                                                    });

                                                    ciktiVerilmeyenSatirAsilEk.satirlar.AddRange(KanunIstatistik.satirlar);
                                                    ciktiVerilmeyenSatirAsilEk.MinimumTutarKontrolEdilecek = false;

                                                    if (tesvik.Kanun != "7252")
                                                    {

                                                        ciktiVerilmeyenSatirAsilEk.Matrah += KanunIstatistik.ToplamUcret;
                                                        ciktiVerilmeyenSatirAsilEk.Gun += KanunIstatistik.Gun;
                                                    }

                                                    ciktiVerilmeyenSatirAsilEk.Matrah_Tesvik_Verilmeyenler_Dahil += KanunIstatistik.ToplamUcret;
                                                    ciktiVerilmeyenSatirAsilEk.Gun_Tesvik_Verilmeyenler_Dahil += KanunIstatistik.Gun;

                                                    if (!tesvik.AsgariUcretDestegiKapsaminda)
                                                    {
                                                        asgariUcretDestegiKapsamiDisindaTesvikVerilenGunSayisi += KanunIstatistik.Gun;
                                                    }
    

                                                    if (AyIcindekiKanun.Equals("16322") || AyIcindekiKanun.Equals("26322") || AyIcindekiKanun.Equals("25510"))
                                                    {

                                                        if (!DonusturulenKisiler6322ve25510Dan.ContainsKey(AyIcindekiKanun)) DonusturulenKisiler6322ve25510Dan.Add(AyIcindekiKanun, new List<Kisi>());

                                                        if (!DonusturulenKisiler6322ve25510Dan[AyIcindekiKanun].Contains(kisi))
                                                        {
                                                            DonusturulenKisiler6322ve25510Dan[AyIcindekiKanun].Add(kisi);
                                                        }

                                                    }
                                                    else
                                                    {
                                                        var orijinalKanunlar = KanunIstatistik.satirlar.Where(p => p.TesvikVerilecekMi).Select(p => p.OrijinalKanun);

                                                        var kno = orijinalKanunlar.FirstOrDefault(p => p.Equals("16322") || p.Equals("26322") || p.Equals("25510"));

                                                        if (kno != null)
                                                        {
                                                            if (!DonusturulenKisiler6322ve25510Dan.ContainsKey(kno)) DonusturulenKisiler6322ve25510Dan.Add(kno, new List<Kisi>());

                                                            if (!DonusturulenKisiler6322ve25510Dan[kno].Contains(kisi))
                                                            {
                                                                DonusturulenKisiler6322ve25510Dan[kno].Add(kisi);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    
                                }
                            }
                        }

                        if (!TesvikAlindi)
                        {
                            if (!tesvikAlamayanlar.Contains(kisi)) tesvikAlamayanlar.Add(kisi);

                            //var satirlar = belgeTuruIstatistik.KanunGunveUcretleri.SelectMany(p => p.Value.Select(x => x)).SelectMany(p => p.Value.satirlar).Where(p=> p.OnayDurumu.Equals("Onaylanmamış"));

                            //foreach (var tki in belgeTuruIstatistik.TesvikKanunuIstatistikleri)
                            //{
                            //    var tesvik = Tesvikler[tki.Key];

                            //    if (!tesvik.DestekKapsaminaGirmeyenBelgeTurleri.Contains(belgeturu))
                            //    {

                            //        if (!tki.Value.AraciMi)
                            //        {
                            //            foreach (var kgu in belgeTuruIstatistik.KanunGunveUcretleri[tesvik.Kanun])
                            //            {
                            //                var kanunIstatistik = kgu.Value;

                            //                var ayIcindekiKanun = kgu.Key;

                            //                if (!kanunIstatistik.AraciMi)
                            //                {
                            //                    if (tesvik.DonusturulecekKanunlar.ContainsKey(ayIcindekiKanun))
                            //                    {

                            //                        foreach (var item in kanunIstatistik.satirlar)
                            //                        {
                            //                            if (item.OnayDurumu.Equals("Onaylanmamış"))
                            //                            {
                            //                                if (new List<string> { "ana işveren","ana şirket"}.Contains(item.Araci.ToLower()))
                            //                                {
                            //                                    var cikti = ciktilar.FirstOrDefault(p => p.BelgeTuru.Equals(belgeturu) && p.Kanun.Equals(ayIcindekiKanun) && !p.Iptal && !p.ExcelOlustur && p.XmlOlustur);

                            //                                    if (cikti == null)
                            //                                    {
                            //                                        cikti = new Cikti();
                            //                                        cikti.BelgeTuru = belgeturu;
                            //                                        cikti.Kanun = ayIcindekiKanun;
                            //                                        cikti.Asil = !AyIcindekiKanunveBelgelerOnaysızlarHaric.Contains(belgeturu + "-" + ayIcindekiKanun);
                            //                                        cikti.ExcelOlustur = false;
                            //                                        cikti.XmlOlustur = true;
                            //                                        ciktilar.Add(cikti);
                            //                                    }

                            //                                    if (!cikti.Kisiler.ContainsKey(kisi)) cikti.Kisiler.Add(kisi, ayIcindekiKanun);

                            //                                    if (!cikti.satirlar.Any(p => p.SosyalGuvenlikNo.Equals(item.SosyalGuvenlikNo) && p.Mahiyet.Equals(item.Mahiyet))) cikti.satirlar.Add(item);

                            //                                }
                            //                            }

                            //                        }

                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                        }

                    }

                    if (!CariAyMi(yilAy))
                    {
                        if (DonusturulenKisiler6322ve25510Dan.Any(p => p.Value.Count > 0))
                        {
                            Dictionary<Kisi, Statistic> verilebilecekKisiler6322 = new Dictionary<Kisi, Statistic>();

                            foreach (var kisi in tesvikAlamayanlar)
                            {
                                if (kisi.AyIstatikleri.ContainsKey(yilAy))
                                {
                                    var belgeTurleriIstatistikleri = kisi.AyIstatikleri[yilAy];

                                    foreach (var item in belgeTurleriIstatistikleri)
                                    {
                                        var belgeTuru = item.Key;
                                        var belgeTuruIstatistik = item.Value;

                                        if (!Tesvikler["6322/25510"].DestekKapsaminaGirmeyenBelgeTurleri.Contains(belgeTuru))
                                        {
                                            if (belgeTuruIstatistik.KanunGunveUcretleri["6322/25510"].Any(p => p.Value.TesvikVerilecek && Tesvikler["6322/25510"].DonusturulecekKanunlar.ContainsKey(p.Key)))
                                            {

                                                bool devam = true;

                                                var anahtar = Yil + "-" + Ay + "-" + belgeTuru;

                                                if (yasaklilar["6322/25510"].ContainsKey(anahtar))
                                                {
                                                    if (yasaklilar["6322/25510"][anahtar].Contains(kisi.TckimlikNo))
                                                    {
                                                        devam = false;
                                                    }
                                                }

                                                if (devam)
                                                {

                                                    if (!verilebilecekKisiler6322.ContainsKey(kisi)) verilebilecekKisiler6322.Add(kisi, new Statistic());

                                                    verilebilecekKisiler6322[kisi].TesvikVerilecekToplamGun += belgeTuruIstatistik.TesvikKanunuIstatistikleri["6322/25510"].TesvikVerilecekGun;
                                                    verilebilecekKisiler6322[kisi].ToplamUcret += belgeTuruIstatistik.TesvikKanunuIstatistikleri["6322/25510"].ToplamUcret;
                                                }
                                            }

                                        }
                                    }
                                }
                            }

                            if (verilebilecekKisiler6322.Count > 0)
                            {

                                var siraliListe = verilebilecekKisiler6322.OrderByDescending(p => p.Value.TesvikVerilecekToplamGun).ThenBy(p => p.Value.ToplamUcret);

                                int verilenKisiSayisi = 0;

                                int verilecekKisiSayisi = DonusturulenKisiler6322ve25510Dan.Sum(p => p.Value.Count);

                                var verilenSayilar = DonusturulenKisiler6322ve25510Dan.ToDictionary(x => x.Key, x => 0);

                                foreach (var item in siraliListe)
                                {
                                    if (verilenKisiSayisi < verilecekKisiSayisi)
                                    {
                                        if (item.Value.TesvikVerilecekToplamGun > 0)
                                        {
                                            var VerilecekKanun = DonusturulenKisiler6322ve25510Dan.FirstOrDefault(p => p.Value.Count > verilenSayilar[p.Key]).Key;

                                            verilenSayilar[VerilecekKanun]++;

                                            verilenKisiSayisi++;

                                            var kanun = VerilecekKanun.PadLeft(5, '0');

                                            var kisi = item.Key;

                                            tesvikAlamayanlar.Remove(kisi);

                                            var tesvik = Tesvikler["6322/25510"];

                                            tesvik.AltKanun = kanun;

                                            foreach (var ayi in kisi.AyIstatikleri[yilAy])
                                            {
                                                var belgeturu = ayi.Key;

                                                if (!Tesvikler["6322/25510"].DestekKapsaminaGirmeyenBelgeTurleri.Contains(belgeturu))
                                                {

                                                    bool verilmeyecekBelgeTuruMu = false;

                                                    var anahtar = Yil + "-" + Ay + "-" + belgeturu;

                                                    if (yasaklilar["6322/25510"].ContainsKey(anahtar))
                                                    {
                                                        if (yasaklilar["6322/25510"][anahtar].Contains(kisi.TckimlikNo))
                                                        {
                                                            verilmeyecekBelgeTuruMu = true;
                                                        }
                                                    }

                                                    if (!verilmeyecekBelgeTuruMu)
                                                    {

                                                        var belgeTuruIstatistik = ayi.Value;

                                                        List<Tesvik> tesvikIstatistikleri = new List<Tesvik>();

                                                        tesvikIstatistikleri.Add(tesvik);

                                                        foreach (var altTesvikKanunNo in tesvik.altTesvikler)
                                                        {
                                                            tesvikIstatistikleri.Add(Tesvikler[altTesvikKanunNo]);
                                                        }

                                                        int tesvikVerilecekToplamGun = 0;
                                                        int toplamGun = 0;
                                                        decimal toplamUcret = 0;

                                                        var ciktiKanun = kanun;

                                                        var cikti = ciktilar.FirstOrDefault(p => p.BelgeTuru.Equals(belgeturu) && p.Kanun.Equals(ciktiKanun) && !p.Iptal);

                                                        if (cikti == null)
                                                        {
                                                            cikti = new Cikti();
                                                            cikti.BelgeTuru = belgeturu;
                                                            cikti.Kanun = ciktiKanun;
                                                            cikti.Asil = !AyIcindekiKanunveBelgeler.Contains(belgeturu + "-" + ciktiKanun);
                                                            cikti.Iptal = false;
                                                            cikti.XmlOlustur = enbuyukay.CompareTo(yilAy) == 0;
                                                            ciktilar.Add(cikti);

                                                        }

                                                        if (!cikti.Kisiler.ContainsKey(kisi)) cikti.Kisiler.Add(kisi, kanun);

                                                        if (tesvik.YasakliKanunlar.Count > 0)
                                                        {
                                                            var sorulacakkanunlar = AyIcindeYasakliKanunuOlanKisiler[tesvik.Kanun].Where(p => p.Key.KullaniciyaSorulsun && !p.Key.KullaniciyaSoruldu && p.Value.Contains(kisi)).Select(p => p.Key);

                                                            foreach (var sk in sorulacakkanunlar)
                                                            {
                                                                if (!sorulacakYasakliKanunlar.Any(p => p.Kanun.Equals(sk.Kanun))) sorulacakYasakliKanunlar.Add(sk);
                                                            }
                                                        }

                                                        foreach (var kgu in belgeTuruIstatistik.KanunGunveUcretleri[tesvik.Kanun])
                                                        {
                                                            var KanunIstatistik = kgu.Value;

                                                            var AyIcindekiKanun = kgu.Key;

                                                            if (tesvik.DonusturulecekKanunlar.ContainsKey(AyIcindekiKanun))
                                                            {
                                                                if (!KanunIstatistik.AraciMi && KanunIstatistik.TesvikVerilecek)
                                                                {
                                                                    var ciktisatirlar = KanunIstatistik.satirlar.Where(p => p.TesvikVerilecekMi);

                                                                    cikti.satirlar.AddRange(ciktisatirlar);

                                                                    cikti.muhtasarSatirlar.AddRange(ciktisatirlar);

                                                                    if (ciktisatirlar.Count() < KanunIstatistik.satirlar.Count)
                                                                    {
                                                                        if (!tesvikVerilmeyenSatirlar.ContainsKey(kisi.TckimlikNo))
                                                                        {
                                                                            tesvikVerilmeyenSatirlar.Add(kisi.TckimlikNo, new List<object>());
                                                                            tesvikVerilmeyenSatirlar[kisi.TckimlikNo].AddRange(KanunIstatistik.satirlar.Where(p => !p.TesvikVerilecekMi));
                                                                        }
                                                                    }

                                                                    bool devam = true;

                                                                    if (AyIcindekiKanun.Equals("05510") || AyIcindekiKanun.EndsWith("6486"))
                                                                    {
                                                                        if (IptalXmlveTesvikAlamayanXmlCikartilmayacak) devam = false;
                                                                    }

                                                                    if (devam)
                                                                    {
                                                                        var ciktiIptal = ciktilar.FirstOrDefault(p => p.BelgeTuru.Equals(belgeturu) && p.Kanun.Equals(AyIcindekiKanun) && p.Iptal);

                                                                        if (ciktiIptal == null)
                                                                        {
                                                                            ciktiIptal = new Cikti();
                                                                            ciktiIptal.BelgeTuru = belgeturu;
                                                                            ciktiIptal.Kanun = AyIcindekiKanun;
                                                                            ciktiIptal.Asil = false;
                                                                            ciktiIptal.Iptal = true;
                                                                            ciktiIptal.XmlOlustur = false;
                                                                            ciktilar.Add(ciktiIptal);
                                                                        }

                                                                        var kisitesvikVerilmeyenSatirlar = KanunIstatistik.satirlar.Where(p => p.TesvikVerilecekMi);

                                                                        ciktiIptal.satirlar.AddRange(kisitesvikVerilmeyenSatirlar);

                                                                        ciktiIptal.Matrah += KanunIstatistik.TesvikVerilecekToplamUcret;
                                                                        ciktiIptal.Gun += KanunIstatistik.TesvikVerilecekGun;

                                                                        foreach (var satir in kisitesvikVerilmeyenSatirlar)
                                                                        {
                                                                            ciktiIptal.Matrah_Tesvik_Verilmeyenler_Dahil += satir.Ucret.ToDecimalSgk() + satir.Ikramiye.ToDecimalSgk();
                                                                            ciktiIptal.Gun_Tesvik_Verilmeyenler_Dahil += satir.Gun.ToInt();

                                                                            if (TesvikHesaplamaSabitleri.AsgariUcretDestegiKapsamiDisindakiKanunlar.Contains(AyIcindekiKanun))
                                                                            {
                                                                                asgariUcretDestegiKapsamiDisindaTesvikVerilenGunSayisi -= satir.Gun.ToInt(); 
                                                                            }
                                                                        }
                                                                    }

                                                                    tesvikVerilecekToplamGun += KanunIstatistik.TesvikVerilecekGun;
                                                                    toplamUcret += KanunIstatistik.TesvikVerilecekToplamUcret;
                                                                    toplamGun += KanunIstatistik.Gun;


                                                                    cikti.Matrah += KanunIstatistik.TesvikVerilecekToplamUcret;
                                                                    cikti.Gun += KanunIstatistik.TesvikVerilecekGun;

                                                                    foreach (var ciktisatir in ciktisatirlar)
                                                                    {
                                                                        cikti.Matrah_Tesvik_Verilmeyenler_Dahil += ciktisatir.Ucret.ToDecimalSgk() + ciktisatir.Ikramiye.ToDecimalSgk();
                                                                        cikti.Gun_Tesvik_Verilmeyenler_Dahil += ciktisatir.Gun.ToInt();
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        foreach (var tesvik2 in tesvikIstatistikleri)
                                                        {

                                                            bool altTesvikMi = !tesvik2.Kanun.Equals(tesvik.Kanun);

                                                            var kisininIcmalleri = tesvik2.IcmalHesaplama(kisi, Yil, Ay, belgeturu, isyeri.IsyeriSicilNo, false, CariAyMi(yilAy), TumTesvikler, altTesvikMi, altTesvikMi ? TumTesvikler[tesvik.Kanun] : null, false, asgariUcretDestegiBuAyVeriliyor);

                                                            TesvikAyIstatistik tesvikAyIstatistik = null;

                                                            foreach (var kisiIcmaller in kisininIcmalleri.icmaller)
                                                            {
                                                                var donusturulenKanun = kisiIcmaller.Key;

                                                                var donusturulenKanunIcmalTutarlari = kisiIcmaller.Value;

                                                                var donusturulenKanunNo = donusturulenKanun.DonusturulecekKanunNo;

                                                                if (!tesvik2.DonusenlerIcmaldenDusulsun) donusturulenKanunNo = "00000";

                                                                if (donusturulenKanunIcmalTutarlari[donusturulenKanunNo].MahsupluTutarBagliKanunlarDahil > 0)
                                                                {
                                                                    if (!tesvik2.TesvikAyIstatistikleri.ContainsKey(yilAy)) tesvik2.TesvikAyIstatistikleri.Add(yilAy, new TesvikAyIstatistik());

                                                                    tesvikAyIstatistik = tesvik2.TesvikAyIstatistikleri[yilAy];

                                                                    if (!tesvikAyIstatistik.Icmal.Tutarlar.ContainsKey(donusturulenKanunNo)) tesvikAyIstatistik.Icmal.Tutarlar.Add(donusturulenKanunNo, 0);
                                                                    tesvikAyIstatistik.Icmal.Tutarlar[donusturulenKanunNo] += donusturulenKanunIcmalTutarlari[donusturulenKanunNo].MahsupluTutarBagliKanunlarDahil;

                                                                    foreach (var dkit in donusturulenKanunIcmalTutarlari)
                                                                    {
                                                                        if (!tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupluTutarlar.ContainsKey(dkit.Key)) tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupluTutarlar.Add(dkit.Key, 0);

                                                                        tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupluTutarlar[dkit.Key] += dkit.Value.MahsupluTutarBagliKanunlarDahil;

                                                                        if (!tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupTutarlari.ContainsKey(dkit.Key)) tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupTutarlari.Add(dkit.Key, 0);

                                                                        tesvikAyIstatistik.Icmal.TutarlarBagliKanunlarMahsupTutarlari[dkit.Key] += dkit.Value.MahsupMiktariBagliKanunlarHaric;
                                                                    }
                                                                }
                                                            }

                                                            if (tesvikAyIstatistik != null)
                                                            {
                                                                if (!yilAy.Equals(muhtasarTarih))
                                                                {
                                                                    tesvikAyIstatistik.TesvikAlacakVar = true;
                                                                    tesvikAyIstatistik.Icmal.Matrah += toplamUcret;
                                                                    tesvikAyIstatistik.Icmal.PrimOdenenGunSayisi += tesvikVerilecekToplamGun;

                                                                    if (!tesvik2.AsgariUcretDestegiKapsaminda)
                                                                    {
                                                                        asgariUcretDestegiKapsamiDisindaTesvikVerilenGunSayisi += toplamGun;
                                                                    }

                                                                    if (!tesvikAyIstatistik.TesvikAlanKisiler.Contains(kisi)) tesvikAyIstatistik.TesvikAlanKisiler.Add(kisi);

                                                                    cikti.ToplamTutar += kisininIcmalleri.icmaller.Sum(p => p.Value[p.Key.DonusturulecekKanunNo].MahsupluTutarBagliKanunlarDahil);
                                                                }

                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else break;
                                }
                            }
                        }
                    }

                    bool XmlCikartilacak = enbuyukay.CompareTo(yilAy) == 0 && ciktilar.Count(p => p.ExcelOlustur) > 0;

                    if (XmlCikartilacak)
                    {
                        foreach (var kisi in tesvikAlamayanlar)
                        {
                            if (kisi.AyIstatikleri.ContainsKey(yilAy))
                            {
                                var kisiSatirlari = TumKisilerSonuc.KisilerinSatirlari[kisi.TckimlikNo][yil + "-" + ay].Where(p => new List<string> { "ana işveren", "ana şirket" }.Contains(p[(int)Enums.AphbHucreBilgileri.Araci].ToString().ToLower()));

                                if (!tesvikVerilmeyenSatirlar.ContainsKey(kisi.TckimlikNo))
                                {
                                    tesvikVerilmeyenSatirlar.Add(kisi.TckimlikNo, new List<object>());
                                    tesvikVerilmeyenSatirlar[kisi.TckimlikNo].AddRange(kisiSatirlari);
                                }

                                //foreach (var row in kisiSatirlari)
                                //{
                                //    var belgeTuru = row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString();
                                //    var kanun = row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().PadLeft(5, '0');

                                //    if (!TesvikHesaplamaSabitleri.DestekKapsaminaGirmeyenBelgeTurleri.Contains(belgeTuru))
                                //    {
                                //        bool bulundu = false;

                                //        var iptalKanun = string.Empty;

                                //        var OrijinalKanun = row[(int)Enums.AphbHucreBilgileri.OrijinalKanunNo].ToString();

                                //        if (OrijinalKanun.Equals("14857"))
                                //        {
                                //            //if (ciktilar.Any(p => p.Kanun.Equals(OrijinalKanun)))
                                //            //{
                                //            bulundu = true;

                                //            iptalKanun = OrijinalKanun;
                                //            //}
                                //        }

                                //        if (!bulundu)
                                //        {
                                //            //if (ciktilar.Any(p => p.Kanun.Equals(kanun)))
                                //            //{
                                //            //if (string.IsNullOrEmpty(OrijinalKanun))
                                //            //{

                                //            if (kanun.Equals("05510") || kanun.EndsWith("6486") || kanun.Equals("14857"))
                                //            {

                                //                bool devam = true;

                                //                if (kanun.Equals("05510") || kanun.EndsWith("6486"))
                                //                {
                                //                    if (IptalXmlveTesvikAlamayanXmlCikartilmayacak) devam = false;
                                //                }

                                //                if (devam)
                                //                {

                                //                    bulundu = true;

                                //                    iptalKanun = kanun;
                                //                }
                                //            }
                                //            //}
                                //            //}
                                //        }

                                //        if (bulundu)
                                //        {

                                //            var cikti = ciktilar.FirstOrDefault(p => p.BelgeTuru.Equals(belgeTuru) && p.Kanun.Equals(iptalKanun) && !p.Iptal && !p.ExcelOlustur && p.XmlOlustur);

                                //            if (cikti == null)
                                //            {
                                //                cikti = new Cikti();
                                //                cikti.BelgeTuru = belgeTuru;
                                //                cikti.Kanun = iptalKanun;
                                //                cikti.Asil = !AyIcindekiKanunveBelgelerOnaysızlarHaric.Contains(belgeTuru + "-" + iptalKanun);
                                //                cikti.ExcelOlustur = false;
                                //                cikti.XmlOlustur = true;
                                //                ciktilar.Add(cikti);

                                //                var cikti2 = ciktilar.FirstOrDefault(p => p.BelgeTuru.Equals(belgeTuru) && p.Kanun.Equals(iptalKanun) && !p.Iptal && p.ExcelOlustur && p.XmlOlustur);

                                //                if (cikti2 != null)
                                //                {
                                //                    foreach (var kisi2 in cikti2.Kisiler)
                                //                    {
                                //                        cikti.Kisiler.Add(kisi2.Key, kisi2.Value);
                                //                    }

                                //                    cikti.satirlar.AddRange(cikti2.satirlar);
                                //                }
                                //            }

                                //            if (!cikti.Kisiler.ContainsKey(kisi)) cikti.Kisiler.Add(kisi, iptalKanun);

                                //            AphbSatir satir = new AphbSatir
                                //            {
                                //                Adi = kisi.Ad,
                                //                CikisGunu = row[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString(),
                                //                EksikGunNedeni = row[(int)Enums.AphbHucreBilgileri.EksikGunSebebi].ToString(),
                                //                EksikGunSayisi = row[(int)Enums.AphbHucreBilgileri.EksikGun].ToString(),
                                //                GirisGunu = row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString(),
                                //                Gun = row[(int)Enums.AphbHucreBilgileri.Gun].ToString(),
                                //                Ikramiye = row[(int)Enums.AphbHucreBilgileri.Ikramiye].ToString(),
                                //                IlkSoyadi = kisi.IlkSoyad,
                                //                IstenCikisNedeni = row[(int)Enums.AphbHucreBilgileri.IstenCikisNedeni].ToString(),
                                //                MeslekKod = kisi.MeslekKod,
                                //                SiraNo = row[(int)Enums.AphbHucreBilgileri.SiraNo].ToString(),
                                //                SosyalGuvenlikNo = kisi.TckimlikNo,
                                //                Soyadi = kisi.Soyad,
                                //                Ucret = row[(int)Enums.AphbHucreBilgileri.Ucret].ToString(),
                                //                Araci = row[(int)Enums.AphbHucreBilgileri.Araci].ToString(),
                                //                Mahiyet = row[(int)Enums.AphbHucreBilgileri.Mahiyet].ToString(),
                                //                OnayDurumu = row[(int)Enums.AphbHucreBilgileri.OnayDurumu].ToString(),
                                //                Kanun = kanun,
                                //                OrijinalKanun = OrijinalKanun
                                //            };

                                //            //if (!cikti.satirlar.Any(p => p.SosyalGuvenlikNo.Equals(satir.SosyalGuvenlikNo) && p.Kanun.Equals(satir.Kanun) && p.Mahiyet.Equals(satir.Mahiyet)))
                                //            //{
                                //            cikti.satirlar.Add(satir);
                                //            //}
                                //        }
                                //    }


                                //}

                            }

                        }
                    }
                    else tesvikVerilmeyenSatirlar.Clear();


                    foreach (var item in tesvikVerilmeyenSatirlar)
                    {
                        var kisi = TumKisilerSonuc.TumKisiler[item.Key];

                        var kisiSatirlari = item.Value;

                        foreach (var kisiSatir in kisiSatirlari)
                        {

                            var kanun = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).Kanun.PadLeft(5, '0') : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.Kanun].ToString().PadLeft(5, '0');
                            var OrijinalKanun = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).OrijinalKanun : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.OrijinalKanunNo].ToString();
                            var CikisGunu = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).CikisGunu : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString();
                            var EksikGunNedeni = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).EksikGunNedeni : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.EksikGunSebebi].ToString();
                            var EksikGunSayisi = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).EksikGunSayisi : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.EksikGun].ToString();
                            var GirisGunu = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).GirisGunu : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString();
                            var Gun = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).Gun : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.Gun].ToString();
                            var UCG = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).UCG : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.UCG].ToString();
                            var Ikramiye = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).Ikramiye : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.Ikramiye].ToString();
                            var IstenCikisNedeni = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).IstenCikisNedeni : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.IstenCikisNedeni].ToString();
                            var MeslekKod = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).MeslekKod : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.MeslekKod].ToString();
                            var SiraNo = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).SiraNo : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.SiraNo].ToString();
                            var Ucret = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).Ucret : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.Ucret].ToString();
                            var Araci = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).Araci : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.Araci].ToString();
                            var Mahiyet = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).Mahiyet : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.Mahiyet].ToString();
                            var OnayDurumu = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).OnayDurumu : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.OnayDurumu].ToString();
                            var belgeTuru = kisiSatir is AphbSatir ? ((AphbSatir)kisiSatir).BelgeTuru : ((DataRow)kisiSatir)[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString();


                            if (!TesvikHesaplamaSabitleri.DestekKapsaminaGirmeyenBelgeTurleri.Contains(belgeTuru))
                            {
                                bool bulundu = false;

                                var iptalKanun = string.Empty;

                                if (OrijinalKanun.Equals("14857"))
                                {
                                    //if (ciktilar.Any(p => p.Kanun.Equals(OrijinalKanun)))
                                    //{
                                    bulundu = true;

                                    iptalKanun = OrijinalKanun;
                                    //}
                                }

                                if (!bulundu)
                                {
                                    //if (ciktilar.Any(p => p.Kanun.Equals(kanun)))
                                    //{
                                    //if (string.IsNullOrEmpty(OrijinalKanun))
                                    //{

                                    if (kanun.Equals("05510") || kanun.EndsWith("6486") || kanun.Equals("14857"))
                                    {

                                        bool devam = true;

                                        if (kanun.Equals("05510") || kanun.EndsWith("6486"))
                                        {
                                            if (IptalXmlveTesvikAlamayanXmlCikartilmayacak) devam = false;
                                        }

                                        if (devam)
                                        {

                                            bulundu = true;

                                            iptalKanun = kanun;
                                        }
                                    }
                                    //}
                                    //}
                                }

                                if (bulundu)
                                {

                                    var cikti = ciktilar.FirstOrDefault(p => p.BelgeTuru.Equals(belgeTuru) && p.Kanun.Equals(iptalKanun) && !p.Iptal && !p.ExcelOlustur && p.XmlOlustur);

                                    if (cikti == null)
                                    {
                                        cikti = new Cikti();
                                        cikti.BelgeTuru = belgeTuru;
                                        cikti.Kanun = iptalKanun;
                                        cikti.Asil = !AyIcindekiKanunveBelgelerOnaysızlarHaric.Contains(belgeTuru + "-" + iptalKanun);
                                        cikti.ExcelOlustur = false;
                                        cikti.XmlOlustur = true;
                                        ciktilar.Add(cikti);

                                        var cikti2 = ciktilar.FirstOrDefault(p => p.BelgeTuru.Equals(belgeTuru) && p.Kanun.Equals(iptalKanun) && !p.Iptal && p.ExcelOlustur && p.XmlOlustur);

                                        if (cikti2 != null)
                                        {
                                            foreach (var kisi2 in cikti2.Kisiler)
                                            {
                                                cikti.Kisiler.Add(kisi2.Key, kisi2.Value);
                                            }

                                            cikti.satirlar.AddRange(cikti2.satirlar);
                                        }
                                    }

                                    if (!cikti.Kisiler.ContainsKey(kisi)) cikti.Kisiler.Add(kisi, iptalKanun);

                                    AphbSatir satir = new AphbSatir
                                    {
                                        Adi = kisi.Ad,
                                        CikisGunu = CikisGunu,
                                        EksikGunNedeni = EksikGunNedeni,
                                        EksikGunSayisi = EksikGunSayisi,
                                        GirisGunu = GirisGunu,
                                        Gun = Gun,
                                        Ikramiye = Ikramiye,
                                        IlkSoyadi = kisi.IlkSoyad,
                                        IstenCikisNedeni = IstenCikisNedeni,
                                        MeslekKod = kisi.MeslekKod,
                                        SiraNo = SiraNo,
                                        SosyalGuvenlikNo = kisi.TckimlikNo,
                                        Soyadi = kisi.Soyad,
                                        Ucret = Ucret,
                                        Araci = Araci,
                                        Mahiyet = Mahiyet,
                                        OnayDurumu = OnayDurumu,
                                        Kanun = kanun,
                                        OrijinalKanun = OrijinalKanun,
                                    };

                                    //if (!cikti.satirlar.Any(p => p.SosyalGuvenlikNo.Equals(satir.SosyalGuvenlikNo) && p.Kanun.Equals(satir.Kanun) && p.Mahiyet.Equals(satir.Mahiyet)))
                                    //{
                                    cikti.satirlar.Add(satir);
                                    //}
                                }
                            }


                        }
                    }


                    bool BasaDonulecek = false;

                    if (ciktilar.Count(p => p.ExcelOlustur) > 0)
                    {
                        //Cari ay ise bildirge minimum tutarına bakmayacağız.
                        if (muhtasarTarih == DateTime.MinValue || yilAy < muhtasarTarih)
                        {
                            var iptaller = ciktilar.Where(p => p.Iptal);

                            int m = 0;

                            while (m < ciktilar.Count)
                            {
                                Cikti cikti = ciktilar[m];

                                if (cikti.Kanun.Equals("00000") == false)
                                {
                                    if (!cikti.Iptal && cikti.ExcelOlustur && cikti.MinimumTutarKontrolEdilecek && cikti.ToplamTutar < Program.BildirgeMinimumTutar)
                                    {
                                        var tesvik = Tesvikler.FirstOrDefault(p => p.Key.PadLeft(5, '0').Equals(cikti.Kanun) || p.Value.AltKanunlar.Any(x => x.PadLeft(5, '0').Equals(cikti.Kanun)));

                                        bool devam = true;

                                        foreach (var iptalCikti in iptaller)
                                        {
                                            var tesvikIptal = Tesvikler.FirstOrDefault(p => p.Key.PadLeft(5, '0').Equals(iptalCikti.Kanun) || p.Value.AltKanunlar.Any(x => x.PadLeft(5, '0').Equals(iptalCikti.Kanun)));

                                            if (tesvikIptal.Value != null)
                                            {
                                                if (tesvikIptal.Value.TesvikVerilirseDigerTesviklerIptalEdilecek)
                                                {
                                                    if (iptalCikti.satirlar.Any(x => cikti.Kisiler.Any(z => x.SosyalGuvenlikNo == z.Key.TckimlikNo)))
                                                    {
                                                        if (ciktilar.Where(p => !p.Iptal && p.Kanun == cikti.Kanun).Any(p => p.ToplamTutar >= Program.BildirgeMinimumTutar))
                                                        {
                                                            devam = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (devam)
                                        {

                                            if (!yasaklilar.ContainsKey(tesvik.Key)) yasaklilar.Add(tesvik.Key, new Dictionary<string, List<string>>());

                                            var anahtar = Yil + "-" + Ay + "-" + cikti.BelgeTuru;

                                            if (!yasaklilar[tesvik.Key].ContainsKey(anahtar)) yasaklilar[tesvik.Key].Add(anahtar, new List<string>());

                                            yasaklilar[tesvik.Key][anahtar].AddRange(cikti.Kisiler.Select(p => p.Key.TckimlikNo));

                                            BasaDonulecek = true;
                                        }
                                    }
                                }

                                m++;
                            }

                            if (BasaDonulecek) goto enBasaDon;
                        }


                        foreach (var item in TumTesvikler)
                        {
                            var tesvik = item.Value;

                            if (tesvik.VerilipVerilmeyecegiKullaniciyaSorulsun && !tesvik.VerilipVerilmeyecegiKullaniciyaSoruldu)
                            {
                                if (ciktilar.Any(p => p.Iptal == false
                                                        &&
                                                        p.ExcelOlustur
                                                        &&
                                                        (
                                                            p.Kanun.Equals(tesvik.Kanun.PadLeft(5, '0'))
                                                            || tesvik.AltKanunlar.Any(x => x.PadLeft(5, '0').Equals(p.Kanun))
                                                        )
                                                 )
                                   )
                                {
                                    var sormayaGerekVar = true;

                                    if (tesvik.Kanun == "6486")
                                    {
                                        if (muhtasarTarih.Year == Yil && muhtasarTarih.Month == Ay && !string.IsNullOrEmpty(MuhtasardaVerilecek6486))
                                        {
                                            sormayaGerekVar = false;
                                        }
                                    }

                                    if (sormayaGerekVar)
                                    {
                                        tesvik.VerilipVerilmeyecegiKullaniciCevabi = MessageBox.Show(tesvik.Kanun + " teşviği hakedenler var. Verilsin mi?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                                        tesvik.VerilipVerilmeyecegiKullaniciyaSoruldu = true;

                                        if (!tesvik.VerilipVerilmeyecegiKullaniciCevabi) BasaDonulecek = true;
                                    }
                                }
                            }

                            if (ay_Icinde_Tesvik_Alip_Eksik_Gun_Kodundan_Uyari_Verilmesi_Gereken_Kisiler[tesvik.Kanun].Count > 0)
                            {
                                var sorulmasiGerekenKisiler = ay_Icinde_Tesvik_Alip_Eksik_Gun_Kodundan_Uyari_Verilmesi_Gereken_Kisiler[tesvik.Kanun];

                                var dahaOnceSorulanKisiler = EksikGunuKodundanDolayiUyariVerilenKisiler[tesvik.Kanun];

                                var sorulmasiGerekenKisilerSon = sorulmasiGerekenKisiler.Where(p => !dahaOnceSorulanKisiler.ContainsKey(p.Key));

                                if (sorulmasiGerekenKisilerSon.Count() > 0)
                                {
                                    var mesaj = "Aşağıdaki kişilere 7252 teşviği verilsin mi? " + Environment.NewLine + "Teşvik verilmesini isterseniz devam et butonuna tıklayınız" + Environment.NewLine + Environment.NewLine;

                                    mesaj += String.Join(Environment.NewLine, sorulmasiGerekenKisilerSon.Select(p => String.Format("Tc No: {0} , Eksik Gün Nedeni: {1}", p.Key, p.Value.Trim(','))));

                                    var onaySonucu = new frmOnay(mesaj).ShowDialog() == DialogResult.Yes;

                                    foreach (var sorulmasiGerekenKisi in sorulmasiGerekenKisilerSon)
                                    {
                                        dahaOnceSorulanKisiler.Add(sorulmasiGerekenKisi.Key, onaySonucu);
                                    }

                                    if (!onaySonucu)
                                    {
                                        BasaDonulecek = true;
                                    }
                                }
                            }
                        }

                        foreach (var yk in sorulacakYasakliKanunlar)
                        {
                            yk.KullaniciyaSoruldu = true;
                            yk.KullaniciCevabi = MessageBox.Show(yk.Kanun + " teşviği daha önceden verilip başka teşvik hak eden kişiler var. Bu kişilere teşvik verilsin mi?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

                            var yasaklikanunlar = AyIcindeYasakliKanunuOlanKisiler.SelectMany(p => p.Value.Keys);

                            foreach (var item in yasaklikanunlar)
                            {
                                if (item.Kanun.Equals(yk.Kanun))
                                {
                                    item.KullaniciyaSoruldu = true;

                                    item.KullaniciCevabi = yk.KullaniciCevabi;
                                }
                            }

                            if (!yk.KullaniciCevabi) BasaDonulecek = true;
                        }



                        if (BasaDonulecek) goto enBasaDon;
                    }
                    else
                    {
                        ciktilar.RemoveAll(p => p.XmlOlustur && !p.ExcelOlustur && !p.Iptal);
                    }

                    #endregion
                }
            }
            else
            {
                KayitYok = true;

                if (!AyCalisanSayilari.ContainsKey(yilAy)) AyCalisanSayilari.Add(yilAy, new Dictionary<string, long>());

                var ayCalisanSayilari = AyCalisanSayilari[yilAy];

                if (!AyCalisanSayilariBazHesaplama.ContainsKey(yilAy)) AyCalisanSayilariBazHesaplama.Add(yilAy, new Dictionary<string, long>());

                var ayCalisanSayilariBazHesaplama = AyCalisanSayilariBazHesaplama[yilAy];

                foreach (var item in Tesvikler)
                {

                    if (!ayCalisanSayilari.ContainsKey(item.Key)) ayCalisanSayilari.Add(item.Key, -1);

                    ayCalisanSayilari[item.Key] = -1;

                    if (!ayCalisanSayilariBazHesaplama.ContainsKey(item.Key)) ayCalisanSayilariBazHesaplama.Add(item.Key, -1);

                    ayCalisanSayilariBazHesaplama[item.Key] = -1;

                }
            }

            if (TesvikHesaplamaSabitleri.AsgariUcretDestegiKatsayilari.ContainsKey(Yil))
            {
                if (asgariUcretDestegiKapsamiDisindaTesvikVerilenGunSayisi > 0)
                {

                    long isyeriAsgariUcretDestekTutariGunSayisi = 0;

                    if (isyeriAsgariUcretBilgisi != null)
                    {
                        isyeriAsgariUcretDestekTutariGunSayisi = isyeriAsgariUcretBilgisi.HesaplananGun;
                    }

                    var fazladanOdenenAsgariUcretTutari = (isyeriAsgariUcretDestekTutariGunSayisi - (asgariUcretDestegiKapsamindakiGunSayisi - asgariUcretDestegiKapsamiDisindaTesvikVerilenGunSayisi)) * TesvikHesaplamaSabitleri.AsgariUcretDestegiKatsayilari[Yil];

                    if (!AsgariUcretDestegiIcmalleri.ContainsKey(yilAy)) AsgariUcretDestegiIcmalleri.Add(yilAy, 0m);

                    AsgariUcretDestegiIcmalleri[yilAy] = Math.Max(0, fazladanOdenenAsgariUcretTutari);
                }
            }

            return Tesvikler;
        }



    }



}
