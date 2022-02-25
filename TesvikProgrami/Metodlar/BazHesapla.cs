using System;
using System.Collections.Generic;
using System.Linq;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static long BazHesapla(
            int yil, 
            int ay, 
            string Kanun,
            TumKisilerSonuc tumKisilerSonuc,
            ref Dictionary<DateTime, Dictionary<string, long>> AyCalisanSayilari,
            ref Dictionary<DateTime, Dictionary<string, long>> AyCalisanSayilariBazHesaplama
            )
        {
            DateTime dt = DateTime.MinValue;

            Tesvik tesvik = Program.TumTesvikler[Kanun];

            if (tesvik.BazHesaplamaBaslangicTarihi > DateTime.MinValue) dt = tesvik.BazHesaplamaBaslangicTarihi;
            else
            {
                dt = new DateTime(yil, tesvik.BazAy ? ay : 1, 1);
            }

            int geriyeGidilecekAy = tesvik.BazHesaplamaGeriyeGidilecekAySayisi;

            var aylar = new List<long>();


            for (int i = 1; i <= geriyeGidilecekAy; i++)
            {
                DateTime dt2 = dt.AddMonths(0 - i);

                bool bulundu = false;

                if (AyCalisanSayilariBazHesaplama.ContainsKey(dt2))
                {
                    if (AyCalisanSayilariBazHesaplama[dt2].ContainsKey(Kanun))
                    {

                        bulundu = true;

                        var aydaCalisanSayisi = AyCalisanSayilariBazHesaplama[dt2][Kanun];

                        if (aydaCalisanSayisi > -1)
                        {
                            aylar.Add(aydaCalisanSayisi);
                        }
                    }
                }


                if (!bulundu)
                {
                    var ayCalisanSayilari = Metodlar.AylikCalisanHesapla(dt2.Year.ToString(), dt2.Month.ToString(), tumKisilerSonuc, ref AyCalisanSayilari,ref AyCalisanSayilariBazHesaplama)["AylikCalisanBaz"];

                    var aydaCalisanSayisi = AyCalisanSayilariBazHesaplama[dt2][Kanun];

                    if (aydaCalisanSayisi > -1)
                    {
                        aylar.Add(aydaCalisanSayisi);
                    }
                }
            }

            if (aylar.Count == 0)
            {
                return 0;
            }
            else
            {
                if (tesvik.BazHesaplamadaOrtalamaAlinsin)
                {
                    return (long)Math.Round((double)aylar.Sum() / aylar.Count, MidpointRounding.AwayFromZero);
                }
                else return aylar.Min();
            }
        }



    }



}
