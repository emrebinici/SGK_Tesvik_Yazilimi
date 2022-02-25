using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static BasvuruKisi AktifBasvuruKaydiniGetir(Kisi kisi, string Kanun, int Yil, int Ay)
        {
            int KacDonemeGiriyor = 0;

            BasvuruKisi bk = null;

            if (kisi.KisiBasvuruKayitlari.ContainsKey(Kanun))
            {

                List<BasvuruKisi> basvurusatirlari = kisi.KisiBasvuruKayitlari[Kanun];

                foreach (var basvuru in basvurusatirlari)
                {
                    DateTime date = new DateTime(Yil, Ay, 1);

                    if (basvuru.TesvikDonemiBaslangic <= date && basvuru.TesvikDonemiBitis >= date)
                    {
                        KacDonemeGiriyor++;

                        bk = basvuru;
                    }

                }

                if (KacDonemeGiriyor > 1)
                {
                    bk = basvurusatirlari.OrderByDescending(a => a.GirisTarihi).First();
                }
            }

            return bk;
	    
        }



    }



}
