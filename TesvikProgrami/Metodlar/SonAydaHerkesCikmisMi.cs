using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;
using System.Linq;
using TesvikProgrami.Classes;
using TesvikProgrami.Enums;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static bool SonAydaHerkesCikmisMi(TumKisilerSonuc tumKisiler)
        {
            var result = false;
            if (tumKisiler != null)
            {
                if (tumKisiler.enbuyukay > DateTime.MinValue)
                {
                    var enbuyukAyString = tumKisiler.enbuyukay.Year.ToString() + "-" + tumKisiler.enbuyukay.Month.ToString();
                    if (tumKisiler.AySatirlari.ContainsKey(enbuyukAyString))
                    {
                        var aySatirlari = tumKisiler.AySatirlari[enbuyukAyString];

                        var kisiler = aySatirlari.Select(row => row[(int)AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString());

                        var hepsiCikmis = true;

                        foreach (var kisiTc in kisiler)
                        {
                            var kisi = tumKisiler.TumKisiler[kisiTc];

                            var tumgirisCikislar = new List<GirisCikisTarihleri>();

                            tumgirisCikislar.AddRange(kisi.GirisTarihleri);
                            tumgirisCikislar.AddRange(kisi.CikisTarihleri);
                            var tumGirisCikislarSirali= tumgirisCikislar.OrderByDescending(p => p.Tarih);

                            if (tumGirisCikislarSirali.Count() > 0)
                            {
                                var sonTarih = tumGirisCikislarSirali.FirstOrDefault();

                                if (sonTarih.GirisMi)
                                {
                                    hepsiCikmis = false;
                                }
                            }
                            else hepsiCikmis = false;

                            if (!hepsiCikmis) break;
                        }

                        return hepsiCikmis;

                    }
                }
            }

            return result;


        }

    }



}
