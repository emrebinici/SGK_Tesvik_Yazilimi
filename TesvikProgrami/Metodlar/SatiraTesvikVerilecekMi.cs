using DevExpress.XtraEditors.Filtering.Templates;
using System;
using System.Linq;
using System.Windows.Forms;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static bool SatiraTesvikVerilecekMi(int yil, int ay, Kisi kisi, Tesvik tesvik, string giris, string cikis, int kisiSatirSayisi, out bool hataliGunVarMi, int gun = -1)
        {
            bool tesvikVerilecek = false;

            hataliGunVarMi = false;

            DateTime AyIcindeIseGirisTarihi = new DateTime(yil, ay, 1);

            DateTime AyIcindeIstenCikisTarihi = new DateTime(yil, ay, 1).AddMonths(1).AddDays(-1);

            DateTime sistemGiris = DateTime.MinValue;
            DateTime sistemCikisi = DateTime.MinValue;

            if (!String.IsNullOrEmpty(giris))
            {
                try
                {
                    AyIcindeIseGirisTarihi = Convert.ToDateTime(giris + "/" + yil);

                    AyIcindeIseGirisTarihi = new DateTime(yil, AyIcindeIseGirisTarihi.Month, AyIcindeIseGirisTarihi.Day);

                }
                catch
                {

                    AyIcindeIseGirisTarihi = DateTime.FromOADate(Convert.ToDouble(giris));

                    AyIcindeIseGirisTarihi = new DateTime(yil, AyIcindeIseGirisTarihi.Month, AyIcindeIseGirisTarihi.Day);
                }

                sistemGiris = AyIcindeIseGirisTarihi;
            }

            if (!String.IsNullOrEmpty(cikis))
            {
                try
                {
                    AyIcindeIstenCikisTarihi = Convert.ToDateTime(cikis + "/" + yil);

                    AyIcindeIstenCikisTarihi = new DateTime(yil, AyIcindeIstenCikisTarihi.Month, AyIcindeIstenCikisTarihi.Day);

                }
                catch
                {

                    AyIcindeIstenCikisTarihi = DateTime.FromOADate(Convert.ToDouble(cikis));

                    AyIcindeIstenCikisTarihi = new DateTime(yil, AyIcindeIstenCikisTarihi.Month, AyIcindeIstenCikisTarihi.Day);
                }

                sistemCikisi = AyIcindeIstenCikisTarihi;
            }

            if (tesvik.BasvuruFormuVar)
            {
                if (kisi.KisiBasvuruKayitlari.ContainsKey(tesvik.Kanun))
                {
                    var basvurular = kisi.KisiBasvuruKayitlari[tesvik.Kanun];

                    bool ilkGunBasvuruDonemiIcinde = false;
                    bool sonGunBasvuruDonemiIcinde = false;

                    bool kontrolYapildi = false;

                    foreach (var basvurudonemi in basvurular)
                    {
                        var bfCikis = basvurudonemi.CikisTarihi;

                        var tesvikDonemiBitis = DateTime.MaxValue;

                        if (basvurudonemi.TesvikDonemiBitis > DateTime.MinValue && basvurudonemi.TesvikDonemiBitis < DateTime.MaxValue) tesvikDonemiBitis = basvurudonemi.TesvikDonemiBitis.AddMonths(1);

                        var enYakinCikis= kisi.CikisTarihleri.OrderBy(p=> p.Tarih).FirstOrDefault(p=> p.Tarih >= basvurudonemi.GirisTarihi && p.Tarih < tesvikDonemiBitis);

                        if (enYakinCikis != null) {

                            if (bfCikis == DateTime.MinValue)
                            {
                                bfCikis = enYakinCikis.Tarih;
                            }
                            else
                            {
                                if (enYakinCikis.Tarih < bfCikis) bfCikis = enYakinCikis.Tarih;
                            }
                        }
                        
                        
                        var basvuruFormuCikis = tesvik.CikistanSonraGiriseTesvikVerilsin ? DateTime.MinValue : bfCikis;

                        var sonTarih = basvuruFormuCikis > DateTime.MinValue 
                                        ?
                                        basvuruFormuCikis
                                        : 
                                            basvurudonemi.TesvikDonemiBitis == DateTime.MaxValue || basvurudonemi.TesvikDonemiBitis == DateTime.MinValue 
                                            ? 
                                            DateTime.MaxValue 
                                            : 
                                            basvurudonemi.TesvikDonemiBitis.AddMonths(1).AddDays(-1);

                        var ayIcindeCikis = AyIcindeIstenCikisTarihi;

                        if (basvuruFormuCikis.Year == AyIcindeIstenCikisTarihi.Year && basvuruFormuCikis.Month == AyIcindeIstenCikisTarihi.Month)
                        {
                            ayIcindeCikis = basvuruFormuCikis < AyIcindeIstenCikisTarihi ? basvuruFormuCikis : AyIcindeIstenCikisTarihi;

                            sistemCikisi = basvuruFormuCikis < AyIcindeIstenCikisTarihi ? basvuruFormuCikis : AyIcindeIstenCikisTarihi;
                        }

                        if (AyIcindeIseGirisTarihi.Between(basvurudonemi.GirisTarihi, sonTarih))
                        {
                            ilkGunBasvuruDonemiIcinde = true;
                        }
                        else if (basvurudonemi.GirisTarihi.Year == yil && basvurudonemi.GirisTarihi.Month == ay)
                        {
                            if (kisiSatirSayisi == 1)
                            {
                                ilkGunBasvuruDonemiIcinde = true;
                            }
                        }


                        if (ayIcindeCikis.Between(basvurudonemi.GirisTarihi, sonTarih))
                        {
                            sonGunBasvuruDonemiIcinde = true;
                        }

                        //if (AyIcindeIseGirisTarihi.Between(basvurudonemi.GirisTarihi, sonTarih) || AyIcindeIstenCikisTarihi.Between(basvurudonemi.GirisTarihi, sonTarih))
                        //{
                        //    tesvikVerilecek = true;

                        //    break;
                        //}

                        if (gun > -1)
                        {

                            var hesaplananGun = Math.Min(30, ayIcindeCikis.Day - AyIcindeIseGirisTarihi.Day + 1);

                            if (sistemGiris == DateTime.MinValue && sistemCikisi == DateTime.MinValue) hesaplananGun = 30;

                            if (AyIcindeIseGirisTarihi.Day == 1 && AyIcindeIseGirisTarihi.AddDays(-AyIcindeIseGirisTarihi.Day + 1).AddMonths(1).AddDays(-1).Day == ayIcindeCikis.Day)
                                hesaplananGun = 30;

                            if (gun > hesaplananGun)
                            {
                                //if (ilkGunBasvuruDonemiIcinde && sonGunBasvuruDonemiIcinde && (tesvik.Kanun == "6111" || tesvik.Kanun == "6645" || tesvik.Kanun == "7252"))
                                //{ 
                                //    //Bu durumda teşvik verilebilir 6111 ve 6645 ve 7252 için. 
                                //}
                                //else
                                //{
                                    tesvikVerilecek = false;
                                    kontrolYapildi = true;
                                //}

                                if (ilkGunBasvuruDonemiIcinde && sonGunBasvuruDonemiIcinde)
                                {
                                    hataliGunVarMi = true;
                                }

                                
                            }
                        }

                        if (ilkGunBasvuruDonemiIcinde && sonGunBasvuruDonemiIcinde) break;

                    }

                    if (!kontrolYapildi)
                    {

                        if (ilkGunBasvuruDonemiIcinde && sonGunBasvuruDonemiIcinde)
                        {
                            tesvikVerilecek = true;
                        }
                    }


                }
            }
            else tesvikVerilecek = true;

            return tesvikVerilecek;

        }



    }



}
