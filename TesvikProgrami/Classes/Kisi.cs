using System;
using System.Collections.Generic;
using System.Linq;

namespace TesvikProgrami.Classes
{
    public class Kisi
    {
        private List<GirisCikisTarihleri> _GirisTarihleri;
        private List<GirisCikisTarihleri> _CikisTarihleri;
        private List<GirisCikisTarihleri> _TaseronluGirisTarihleri;
        private List<GirisCikisTarihleri> _TaseronluCikisTarihleri;
        private List<DateTime> _CalisilanAylarTaseronsuz;
        private List<DateTime> _CalisilanAylarTaseronDahil;
        public TumKisilerSonuc TumKisilerSonuc { get; set; }

        public List<GirisCikisTarihleri> SistemGirisCikislari = new List<GirisCikisTarihleri>();
        public bool SistemGirisCikislariCekildi;

        public Dictionary<string, Dictionary<DateTime, Dictionary<string, int>>> OncedenAlinanTesvikGunleri = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new Dictionary<DateTime, Dictionary<string, int>>());

        public int AyIcindeOncedenAlinanTesvikGunSayisiBul(string tesvikKanun, DateTime yilAy)
        {
            if (this.OncedenAlinanTesvikGunleri[tesvikKanun].ContainsKey(yilAy))
            {
                return this.OncedenAlinanTesvikGunleri[tesvikKanun][yilAy].Sum(p => p.Value);
            }

            return 0;
        }


        public int KisininAlabilecegiGunSayisiniBul(string tesvikKanun, string belgeTuru, int Gun, int Yil, int Ay, string IsyeriSicilNo)
        {
            var ayTarih = new DateTime(Yil, Ay, 1);

            if (this.OncedenAlinanTesvikGunleri[tesvikKanun].ContainsKey(ayTarih))
            {
                if (this.OncedenAlinanTesvikGunleri[tesvikKanun][ayTarih].ContainsKey(belgeTuru))
                {
                    return this.OncedenAlinanTesvikGunleri[tesvikKanun][ayTarih][belgeTuru];
                }
            }
            
            int result = 0;

            var tesvik = Program.TumTesvikler[tesvikKanun];

            if (!tesvik.Basvuru_Formundaki_Baz_Sayisi_Kadar_Gun_Ay_Icinde_TesvikAlabilir)
            {
                result = Gun;
            }
            else
            {

                var basvuruKaydi = Metodlar.AktifBasvuruKaydiniGetir(this, tesvikKanun, Yil, Ay);

                if (basvuruKaydi != null)
                {
                    if (TumKisilerSonuc.KisilerinSatirlari.ContainsKey(this.TckimlikNo))
                    {
                        if (TumKisilerSonuc.KisilerinSatirlari[this.TckimlikNo].ContainsKey(Yil.ToString() + "-" + Ay.ToString()))
                        {
                            var kisiAySatirlari = TumKisilerSonuc.KisilerinSatirlari[this.TckimlikNo][Yil.ToString() + "-" + Ay.ToString()];

                            if (!tesvik.TaseronunAldigiTesvikKotadanDusulsun)
                            {
                                kisiAySatirlari = kisiAySatirlari.Where(p => p[(int)Enums.AphbHucreBilgileri.Araci].ToString().ToLower().Contains("ana işveren")).ToList();
                            }

                            var siraliSatirlar = kisiAySatirlari.OrderByDescending(row => tesvik.BelgeTuruOranBul(Yil, Ay, row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString(), IsyeriSicilNo));

                            var verilebilecekGunSayisi = basvuruKaydi.Baz;

                            foreach (var row in siraliSatirlar)
                            {
                                var gun = row[(int)Enums.AphbHucreBilgileri.Gun].ToString().ToInt();
                                var satirBelgeTuru = row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString();
                                var satirKanun = row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().PadLeft(5, '0');

                                if (tesvik.Kanun.PadLeft(5, '0') == satirKanun || tesvik.AltKanunlar.Contains(satirKanun))
                                {
                                    if (gun < verilebilecekGunSayisi)
                                    {
                                        verilebilecekGunSayisi -= gun;

                                        if (belgeTuru == satirBelgeTuru)
                                            result += gun;
                                    }
                                    else
                                    {
                                        if (belgeTuru == satirBelgeTuru)
                                            result += verilebilecekGunSayisi;

                                        verilebilecekGunSayisi = 0;

                                    }

                                }
                            }

                        }
                    }
                }
                else
                {
                    
                    result = Gun;
                }
            }

            if (!this.OncedenAlinanTesvikGunleri[tesvikKanun].ContainsKey(ayTarih))
                this.OncedenAlinanTesvikGunleri[tesvikKanun].Add(ayTarih, new Dictionary<string, int>());

            if (!this.OncedenAlinanTesvikGunleri[tesvikKanun][ayTarih].ContainsKey(belgeTuru))
                this.OncedenAlinanTesvikGunleri[tesvikKanun][ayTarih].Add(belgeTuru, result);

            return result;

        }

        public Kisi()
        {

        }

        public string TckimlikNo;

        public string Ad;

        public string Soyad;

        public string IlkSoyad;

        public string MeslekKod;

        public Dictionary<string, List<BasvuruKisi>> KisiBasvuruKayitlari = new Dictionary<string, List<BasvuruKisi>>();

        public List<BasvuruListesi7166Kisi> BasvuruListesi7166Kayitlari = new List<BasvuruListesi7166Kisi>();

        public Dictionary<DateTime, Dictionary<string, BelgeTuruIstatistikleri>> AyIstatikleri = new Dictionary<DateTime, Dictionary<string, BelgeTuruIstatistikleri>>();

        public List<GirisCikisTarihleri> GirisTarihleri
        {
            get
            {
                if (_GirisTarihleri == null)
                {
                    GirisCikisTarihleriniOlustur();
                }

                return _GirisTarihleri;
            }
            set
            {
                _GirisTarihleri = value;
            }
        }

        public List<GirisCikisTarihleri> CikisTarihleri
        {
            get
            {
                if (_CikisTarihleri == null)
                {
                    GirisCikisTarihleriniOlustur();
                }

                return _CikisTarihleri;
            }
            set
            {
                _CikisTarihleri = value;
            }
        }

        public List<GirisCikisTarihleri> TaseronluGirisTarihleri
        {
            get
            {
                if (_TaseronluGirisTarihleri == null)
                {
                    GirisCikisTarihleriniOlustur();
                }

                return _TaseronluGirisTarihleri;
            }
            set
            {
                _TaseronluGirisTarihleri = value;
            }
        }

        public List<GirisCikisTarihleri> TaseronluCikisTarihleri
        {
            get
            {
                if (_TaseronluCikisTarihleri == null)
                {
                    GirisCikisTarihleriniOlustur();
                }

                return _TaseronluCikisTarihleri;
            }
            set
            {
                _TaseronluCikisTarihleri = value;
            }
        }

        public List<DateTime> CalisilanAylarTaseronDahil
        {
            get
            {
                if (_CalisilanAylarTaseronDahil == null)
                {
                    if (TumKisilerSonuc != null)
                    {
                        _CalisilanAylarTaseronDahil = TumKisilerSonuc.KisilerinSatirlari[this.TckimlikNo].Where(p => p.Value.Count > 0).Select(r => new DateTime(Convert.ToInt32(r.Value.FirstOrDefault()[(int)Enums.AphbHucreBilgileri.Yil]), Convert.ToInt32(r.Value.FirstOrDefault()[(int)Enums.AphbHucreBilgileri.Ay]), 1)).Distinct().ToList();

                        if (_CalisilanAylarTaseronDahil.Count > 0) _CalisilanAylarTaseronDahil.Sort();
                    }


                }

                return _CalisilanAylarTaseronDahil;
            }
            set
            {
                _CalisilanAylarTaseronDahil = value;
            }
        }

        public List<DateTime> CalisilanAylarTaseronsuz
        {
            get
            {
                if (_CalisilanAylarTaseronsuz == null)
                {
                    if (TumKisilerSonuc != null)
                    {
                        _CalisilanAylarTaseronsuz = TumKisilerSonuc.KisilerinSatirlariIptallerDahil[this.TckimlikNo].Where(p => p.Value.Count > 0 && p.Value.Any(row => new List<string> { "ana işveren", "ana şirket" }.Contains(row[(int)Enums.AphbHucreBilgileri.Araci].ToString().Trim().ToLower()))).Select(r => Convert.ToDateTime(r.Key.Replace("-", "/"))).Distinct().ToList();

                        if (_CalisilanAylarTaseronsuz.Count > 0) _CalisilanAylarTaseronsuz.Sort();
                    }


                }

                return _CalisilanAylarTaseronsuz;
            }
            set
            {
                _CalisilanAylarTaseronsuz = value;
            }
        }

        public int AlinabilecekTesvikSayisi(DateTime yilAy, bool BasvuruFormuOlmayanlarDahil)
        {
            int tesviksayisi = 0;

            if (this.AyIstatikleri.ContainsKey(yilAy))
            {
                var ayIstatistik = this.AyIstatikleri[yilAy];

                var tesvikverilecekKanunlar = ayIstatistik.SelectMany(p => p.Value.TesvikKanunuIstatistikleri).Where(p => p.Value.TesvikAlabilir && (Program.TumTesvikler[p.Key].BasvuruFormuVar || BasvuruFormuOlmayanlarDahil));

                if (tesvikverilecekKanunlar.Count() > 0)
                {
                    var kanunlar = tesvikverilecekKanunlar.Select(p => p.Key).Distinct();

                    tesviksayisi = kanunlar.Count();
                }

            }

            return tesviksayisi;


        }

        void GirisCikisTarihleriniOlustur()
        {
            _GirisTarihleri = new List<GirisCikisTarihleri>();
            _CikisTarihleri = new List<GirisCikisTarihleri>();
            _TaseronluCikisTarihleri = new List<GirisCikisTarihleri>();
            _TaseronluGirisTarihleri = new List<GirisCikisTarihleri>();

            if (TumKisilerSonuc == null) return;

            var satirlar = TumKisilerSonuc.KisilerinSatirlari[this.TckimlikNo].SelectMany(p => p.Value);

            foreach (var row in satirlar)
            {
                string Kanun = row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().Trim().PadLeft(5,'0');

                string Araci = row[(int)Enums.AphbHucreBilgileri.Araci].ToString(); ;

                bool AraciMi = !String.IsNullOrEmpty(Araci) && !Araci.ToLower().Contains("ana şirket") && !Araci.ToLower().Contains("ana işveren");

                DateTime AyIcindeIseGirisTarihi = DateTime.MinValue;

                DateTime AyIcindeIstenCikisTarihi = DateTime.MinValue;

                string kisiyil = row[(int)Enums.AphbHucreBilgileri.Yil].ToString().Trim();

                string kisiay = row[(int)Enums.AphbHucreBilgileri.Ay].ToString().Trim();

                string kisiKanun = row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().Trim().PadLeft(5,'0');

                if (kisiKanun == "5510") kisiKanun = "05510";
                else if (kisiKanun == "6111") kisiKanun = "06111";
                else if (kisiKanun == "6645") kisiKanun = "06645";
                else if (kisiKanun == "2828") kisiKanun = "02828";
                else if (kisiKanun == "687" || kisiKanun == "0687") kisiKanun = "00687";
                else if (kisiKanun == "1687") kisiKanun = "01687";
                else if (String.IsNullOrEmpty(Kanun)) Kanun = "00000";

                string kisiBelgeTuru = row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString().Trim();

                string kisiMahiyet = row[(int)Enums.AphbHucreBilgileri.Mahiyet].ToString().Trim();

                string gtarih = row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString().Trim();

                string istenCikisNedeni = row[(int)Enums.AphbHucreBilgileri.IstenCikisNedeni].ToString().Trim();

                if (!String.IsNullOrEmpty(gtarih))
                {
                    try
                    {
                        AyIcindeIseGirisTarihi = Convert.ToDateTime(gtarih + "/" + kisiyil);

                        AyIcindeIseGirisTarihi = new DateTime(Convert.ToInt32(kisiyil), AyIcindeIseGirisTarihi.Month, AyIcindeIseGirisTarihi.Day);

                    }
                    catch
                    {
                        try
                        {
                            AyIcindeIseGirisTarihi = DateTime.FromOADate(Convert.ToDouble(gtarih));

                            AyIcindeIseGirisTarihi = new DateTime(Convert.ToInt32(kisiyil), AyIcindeIseGirisTarihi.Month, AyIcindeIseGirisTarihi.Day);
                        }
                        catch (Exception)
                        {
                            AyIcindeIseGirisTarihi = new DateTime(kisiyil.ToInt(), kisiay.ToInt(), 1);
                        }

                    }
                }


                string ctarih = row[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString().Trim();

                if (!String.IsNullOrEmpty(ctarih))
                {
                    try
                    {
                        AyIcindeIstenCikisTarihi = Convert.ToDateTime(ctarih + "/" + kisiyil);

                        AyIcindeIstenCikisTarihi = new DateTime(Convert.ToInt32(kisiyil), AyIcindeIstenCikisTarihi.Month, AyIcindeIstenCikisTarihi.Day);

                    }
                    catch
                    {

                        try
                        {
                            AyIcindeIstenCikisTarihi = DateTime.FromOADate(Convert.ToDouble(ctarih));

                            AyIcindeIstenCikisTarihi = new DateTime(Convert.ToInt32(kisiyil), AyIcindeIstenCikisTarihi.Month, AyIcindeIstenCikisTarihi.Day);
                        }
                        catch (Exception)
                        {
                            AyIcindeIstenCikisTarihi = new DateTime(kisiyil.ToInt(), kisiay.ToInt(), 1).AddMonths(1).AddDays(-1);
                        }

                    }
                }

                if (AyIcindeIseGirisTarihi != DateTime.MinValue)
                {

                    GirisCikisTarihleri gcttemp = new GirisCikisTarihleri();

                    gcttemp.Yil = kisiyil;

                    gcttemp.Ay = kisiay;

                    gcttemp.Kanun = kisiKanun;

                    gcttemp.belgeturu = kisiBelgeTuru;

                    gcttemp.Mahiyet = kisiMahiyet;

                    gcttemp.Tarih = AyIcindeIseGirisTarihi;

                    gcttemp.Araci = Araci;

                    gcttemp.GirisMi = true;

                    if (gcttemp.Mahiyet.ToUpper() == "ASIL")
                    {
                        if (!AraciMi)
                        {

                            int i = 0;

                            bool IptalVar = false;

                            while (i < this._GirisTarihleri.Count)
                            {
                                GirisCikisTarihleri gct = this._GirisTarihleri[i];

                                if (gct.Yil == gcttemp.Yil && gct.Ay == gcttemp.Ay && gct.Kanun == gcttemp.Kanun && gct.belgeturu == gcttemp.belgeturu && gct.Araci == gcttemp.Araci && gct.Mahiyet.ToUpper().EndsWith("PTAL"))
                                {
                                    this._GirisTarihleri.RemoveAt(i);

                                    IptalVar = true;

                                    break;
                                }
                                else i++;
                            }

                            if (!IptalVar)
                            {
                                this._GirisTarihleri.Add(gcttemp);
                            }
                        }

                        int j = 0;

                        bool IptalVarTaseron = false;

                        while (j < this._TaseronluGirisTarihleri.Count)
                        {
                            GirisCikisTarihleri gct = this._TaseronluGirisTarihleri[j];

                            if (gct.Yil == gcttemp.Yil && gct.Ay == gcttemp.Ay && gct.Kanun == gcttemp.Kanun && gct.belgeturu == gcttemp.belgeturu && gct.Araci == gcttemp.Araci && gct.Mahiyet.ToUpper().EndsWith("PTAL"))
                            {
                                this._TaseronluGirisTarihleri.RemoveAt(j);

                                IptalVarTaseron = true;

                                break;
                            }
                            else j++;
                        }

                        if (!IptalVarTaseron)
                        {
                            this._TaseronluGirisTarihleri.Add(gcttemp);
                        }

                    }
                    else if (kisiMahiyet.ToUpper().EndsWith("PTAL"))
                    {
                        if (!AraciMi)
                        {

                            int i = 0;

                            bool AsilVar = false;

                            while (i < this._GirisTarihleri.Count)
                            {
                                GirisCikisTarihleri gct = this._GirisTarihleri[i];

                                if (gct.Yil == gcttemp.Yil && gct.Ay == gcttemp.Ay && gct.Kanun == gcttemp.Kanun && gct.belgeturu == gcttemp.belgeturu && gct.Araci == gcttemp.Araci && gct.Mahiyet.ToUpper() == "ASIL")
                                {
                                    this._GirisTarihleri.RemoveAt(i);

                                    AsilVar = true;

                                    break;
                                }
                                else i++;
                            }

                            if (!AsilVar)
                            {
                                this._GirisTarihleri.Add(gcttemp);
                            }
                        }

                        int j = 0;

                        bool AsilVarTaseron = false;

                        while (j < this._TaseronluGirisTarihleri.Count)
                        {
                            GirisCikisTarihleri gct = this._TaseronluGirisTarihleri[j];

                            if (gct.Yil == gcttemp.Yil && gct.Ay == gcttemp.Ay && gct.Kanun == gcttemp.Kanun && gct.belgeturu == gcttemp.belgeturu && gct.Araci == gcttemp.Araci && gct.Mahiyet.ToUpper() == "ASIL")
                            {
                                this._TaseronluGirisTarihleri.RemoveAt(j);

                                AsilVarTaseron = true;

                                break;
                            }
                            else j++;
                        }

                        if (!AsilVarTaseron)
                        {
                            this._TaseronluGirisTarihleri.Add(gcttemp);
                        }
                    }
                    else
                    {
                        if (!AraciMi) this._GirisTarihleri.Add(gcttemp);

                        this._TaseronluGirisTarihleri.Add(gcttemp);
                    }
                }

                if (AyIcindeIstenCikisTarihi != DateTime.MinValue)
                {

                    GirisCikisTarihleri gcttemp = new GirisCikisTarihleri();

                    gcttemp.Yil = kisiyil;

                    gcttemp.Ay = kisiay;

                    gcttemp.Kanun = kisiKanun;

                    gcttemp.belgeturu = kisiBelgeTuru;

                    gcttemp.Mahiyet = kisiMahiyet;

                    gcttemp.Tarih = new DateTime(Convert.ToInt32(kisiyil), AyIcindeIstenCikisTarihi.Month, AyIcindeIstenCikisTarihi.Day);

                    gcttemp.IstenCikisNedeni = istenCikisNedeni;

                    if (gcttemp.Mahiyet.ToUpper() == "ASIL")
                    {
                        if (!AraciMi)
                        {

                            int i = 0;

                            bool IptalVar = false;

                            while (i < this._CikisTarihleri.Count)
                            {
                                GirisCikisTarihleri gct = this._CikisTarihleri[i];

                                if (gct.Yil == gcttemp.Yil && gct.Ay == gcttemp.Ay && gct.Kanun == gcttemp.Kanun && gct.belgeturu == gcttemp.belgeturu && gct.Mahiyet.ToUpper().EndsWith("PTAL"))
                                {
                                    this._CikisTarihleri.RemoveAt(i);

                                    IptalVar = true;

                                    break;
                                }
                                else i++;
                            }

                            if (!IptalVar)
                            {
                                this._CikisTarihleri.Add(gcttemp);
                            }
                        }

                        int j = 0;

                        bool IptalVarTaseron = false;

                        while (j < this._TaseronluCikisTarihleri.Count)
                        {
                            GirisCikisTarihleri gct = this._TaseronluCikisTarihleri[j];

                            if (gct.Yil == gcttemp.Yil && gct.Ay == gcttemp.Ay && gct.Kanun == gcttemp.Kanun && gct.belgeturu == gcttemp.belgeturu && gct.Mahiyet.ToUpper().EndsWith("PTAL"))
                            {
                                this._TaseronluCikisTarihleri.RemoveAt(j);

                                IptalVarTaseron = true;

                                break;
                            }
                            else j++;
                        }

                        if (!IptalVarTaseron)
                        {
                            this._TaseronluCikisTarihleri.Add(gcttemp);
                        }
                    }
                    else if (kisiMahiyet.ToUpper().EndsWith("PTAL"))
                    {
                        if (!AraciMi)
                        {

                            int i = 0;

                            bool AsilVar = false;

                            while (i < this._CikisTarihleri.Count)
                            {
                                GirisCikisTarihleri gct = this._CikisTarihleri[i];

                                if (gct.Yil == gcttemp.Yil && gct.Ay == gcttemp.Ay && gct.Kanun == gcttemp.Kanun && gct.belgeturu == gcttemp.belgeturu && gct.Mahiyet.ToUpper() == "ASIL")
                                {
                                    this._CikisTarihleri.RemoveAt(i);

                                    AsilVar = true;

                                    break;
                                }
                                else i++;
                            }

                            if (!AsilVar)
                            {
                                this._CikisTarihleri.Add(gcttemp);
                            }
                        }

                        int j = 0;

                        bool AsilVarTaseron = false;

                        while (j < this._TaseronluCikisTarihleri.Count)
                        {
                            GirisCikisTarihleri gct = this._TaseronluCikisTarihleri[j];

                            if (gct.Yil == gcttemp.Yil && gct.Ay == gcttemp.Ay && gct.Kanun == gcttemp.Kanun && gct.belgeturu == gcttemp.belgeturu && gct.Mahiyet.ToUpper() == "ASIL")
                            {
                                this._TaseronluCikisTarihleri.RemoveAt(j);

                                AsilVarTaseron = true;

                                break;
                            }
                            else j++;
                        }

                        if (!AsilVarTaseron)
                        {
                            this._TaseronluCikisTarihleri.Add(gcttemp);
                        }
                    }
                    else
                    {
                        if (!AraciMi) this._CikisTarihleri.Add(gcttemp);

                        this._TaseronluCikisTarihleri.Add(gcttemp);
                    }

                }
            }

            int ind = 0;

            List<DateTime> giristarihleri = new List<DateTime>();

            while (ind < this._GirisTarihleri.Count)
            {
                if (this._GirisTarihleri[ind].Mahiyet.EndsWith("PTAL")) this._GirisTarihleri.RemoveAt(ind);
                else
                {
                    if (giristarihleri.Contains(this._GirisTarihleri[ind].Tarih))
                    {
                        this._GirisTarihleri.RemoveAt(ind);
                    }
                    else
                    {
                        giristarihleri.Add(this._GirisTarihleri[ind].Tarih);

                        ind++;
                    }
                }

            }

            ind = 0;

            List<DateTime> cikistarihleri = new List<DateTime>();

            while (ind < this._CikisTarihleri.Count)
            {
                if (this._CikisTarihleri[ind].Mahiyet.EndsWith("PTAL")) this._CikisTarihleri.RemoveAt(ind);
                else
                {
                    if (cikistarihleri.Contains(this._CikisTarihleri[ind].Tarih))
                    {
                        this._CikisTarihleri.RemoveAt(ind);
                    }
                    else
                    {
                        cikistarihleri.Add(this._CikisTarihleri[ind].Tarih);

                        ind++;
                    }
                }
            }

            ind = 0;

            List<DateTime> taseronlugiristarihleri = new List<DateTime>();

            while (ind < this._TaseronluGirisTarihleri.Count)
            {
                if (this._TaseronluGirisTarihleri[ind].Mahiyet.EndsWith("PTAL")) this._TaseronluGirisTarihleri.RemoveAt(ind);
                else
                {
                    if (taseronlugiristarihleri.Contains(this._TaseronluGirisTarihleri[ind].Tarih))
                    {
                        this._TaseronluGirisTarihleri.RemoveAt(ind);
                    }
                    else
                    {
                        taseronlugiristarihleri.Add(this._TaseronluGirisTarihleri[ind].Tarih);

                        ind++;
                    }
                }

            }

            ind = 0;

            List<DateTime> taseronlucikistarihleri = new List<DateTime>();

            while (ind < this._TaseronluCikisTarihleri.Count)
            {
                if (this._TaseronluCikisTarihleri[ind].Mahiyet.EndsWith("PTAL")) this._TaseronluCikisTarihleri.RemoveAt(ind);
                else
                {
                    if (taseronlucikistarihleri.Contains(this._TaseronluCikisTarihleri[ind].Tarih))
                    {
                        this._TaseronluCikisTarihleri.RemoveAt(ind);
                    }
                    else
                    {
                        taseronlucikistarihleri.Add(this._TaseronluCikisTarihleri[ind].Tarih);

                        ind++;
                    }
                }
            }

            foreach (var gt in this._GirisTarihleri)
            {
                foreach (var ct in this._CikisTarihleri)
                {
                    if (gt.Tarih == ct.Tarih)
                    {
                        ct.Tarih = ct.Tarih.AddHours(1);
                    }
                }
            }
        }
    }

}
