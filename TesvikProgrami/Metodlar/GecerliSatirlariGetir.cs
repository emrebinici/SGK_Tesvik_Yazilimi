using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static List<DataRow> GecerliSatirlariGetir(List<DataRow> satirlar, bool sirala = false, string Araci = null, bool asliveyaEkiOlmayanIptalSilinsin = true)
        {
            List<int> eklenenenler = new List<int>();

            for (int i = 0; i < satirlar.Count; i++)
            {
                if (Araci != null)
                {
                    if (!satirlar[i][(int)Enums.AphbHucreBilgileri.Araci].ToString().StartsWith(Araci))
                    {
                        eklenenenler.Add(i);

                        continue;
                    }
                }


                if (satirlar[i][(int)Enums.AphbHucreBilgileri.Mahiyet].ToString().ToUpper().EndsWith("PTAL"))
                {
                    bool bulundu = false;

                    for (int j = 0; j < satirlar.Count; j++)
                    {
                        if (eklenenenler.Contains(j)) continue;

                        if (satirlar[i][(int)Enums.AphbHucreBilgileri.Yil].Equals(satirlar[j][(int)Enums.AphbHucreBilgileri.Yil])
                            && satirlar[i][(int)Enums.AphbHucreBilgileri.Ay].Equals(satirlar[j][(int)Enums.AphbHucreBilgileri.Ay])
                            && satirlar[i][(int)Enums.AphbHucreBilgileri.Kanun].ToString().PadLeft(5, '0').Equals(satirlar[j][(int)Enums.AphbHucreBilgileri.Kanun].ToString().PadLeft(5, '0'))
                            && satirlar[i][(int)Enums.AphbHucreBilgileri.BelgeTuru].Equals(satirlar[j][(int)Enums.AphbHucreBilgileri.BelgeTuru])
                            && satirlar[i][(int)Enums.AphbHucreBilgileri.Araci].Equals(satirlar[j][(int)Enums.AphbHucreBilgileri.Araci])
                            && satirlar[i][(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].Equals(satirlar[j][(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo])
                            && satirlar[i][(int)Enums.AphbHucreBilgileri.Gun].Equals(satirlar[j][(int)Enums.AphbHucreBilgileri.Gun])
                            && (Convert.ToDecimal(satirlar[i][(int)Enums.AphbHucreBilgileri.Ucret]) + Convert.ToDecimal(satirlar[i][(int)Enums.AphbHucreBilgileri.Ikramiye])).Equals(Convert.ToDecimal(satirlar[j][(int)Enums.AphbHucreBilgileri.Ucret]) + Convert.ToDecimal(satirlar[j][(int)Enums.AphbHucreBilgileri.Ikramiye]))
                            //&& satirlar[i][(int)Enums.AphbHucreBilgileri.Ikramiye].Equals(satirlar[j][(int)Enums.AphbHucreBilgileri.Ikramiye])
                            && satirlar[j][(int)Enums.AphbHucreBilgileri.Mahiyet].ToString().ToUpper().Equals("ASIL"))
                        {
                            eklenenenler.Add(j);

                            bulundu = true;

                            break;
                        }
                    }

                    if (!bulundu)
                    {
                        for (int j = 0; j < satirlar.Count; j++)
                        {
                            if (eklenenenler.Contains(j)) continue;

                            if (satirlar[i][(int)Enums.AphbHucreBilgileri.Yil].Equals(satirlar[j][(int)Enums.AphbHucreBilgileri.Yil])
                                && satirlar[i][(int)Enums.AphbHucreBilgileri.Ay].Equals(satirlar[j][(int)Enums.AphbHucreBilgileri.Ay])
                                && satirlar[i][(int)Enums.AphbHucreBilgileri.Kanun].ToString().PadLeft(5, '0').Equals(satirlar[j][(int)Enums.AphbHucreBilgileri.Kanun].ToString().PadLeft(5, '0'))
                                && satirlar[i][(int)Enums.AphbHucreBilgileri.BelgeTuru].Equals(satirlar[j][(int)Enums.AphbHucreBilgileri.BelgeTuru])
                                && satirlar[i][(int)Enums.AphbHucreBilgileri.Araci].Equals(satirlar[j][(int)Enums.AphbHucreBilgileri.Araci])
                                && satirlar[i][(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].Equals(satirlar[j][(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo])
                                && satirlar[i][(int)Enums.AphbHucreBilgileri.Gun].Equals(satirlar[j][(int)Enums.AphbHucreBilgileri.Gun])
                                && (Convert.ToDecimal(satirlar[i][(int)Enums.AphbHucreBilgileri.Ucret]) + Convert.ToDecimal(satirlar[i][(int)Enums.AphbHucreBilgileri.Ikramiye])).Equals(Convert.ToDecimal(satirlar[j][(int)Enums.AphbHucreBilgileri.Ucret]) + Convert.ToDecimal(satirlar[j][(int)Enums.AphbHucreBilgileri.Ikramiye]))
                                //&& satirlar[i][(int)Enums.AphbHucreBilgileri.Ikramiye].Equals(satirlar[j][(int)Enums.AphbHucreBilgileri.Ikramiye])
                                && satirlar[j][(int)Enums.AphbHucreBilgileri.Mahiyet].ToString().Trim().ToUpper().Equals("EK"))
                            {
                                eklenenenler.Add(j);

                                bulundu = true;

                                break;
                            }
                        }
                    }

                    if (asliveyaEkiOlmayanIptalSilinsin)
                    {
                        eklenenenler.Add(i);
                    }
                    else
                    {
                        if (bulundu)
                        {
                            eklenenenler.Add(i);
                        }
                    }
                }
            }

            var sonuc = satirlar.Where((p, ind) => !eklenenenler.Contains(ind)).ToList();

            if (sirala)
            {
                sonuc = sonuc.OrderByDescending(row => string.IsNullOrEmpty(row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString()) ? DateTime.MinValue : new DateTime(Convert.ToInt32(row[(int)Enums.AphbHucreBilgileri.Yil]), Convert.ToInt32(row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString().Split('/')[1]), Convert.ToInt32(row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString().Split('/')[0]))).ToList();
            }

            return sonuc;
        }


    }



}
