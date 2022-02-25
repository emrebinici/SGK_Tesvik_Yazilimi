using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static Dictionary<DataRow, string> SatirlarinIptalKarsiliklariniBul(List<DataRow> satirlar, List<DataRow> iptallerDahil, string iptalVarsayilanKanun)
        {
            var result = new Dictionary<DataRow, string>();

            for (int i = 0; i < satirlar.Count; i++)
            {
                bool bulundu = false;

                for (int j = 0; j < iptallerDahil.Count; j++)
                {
                    if (satirlar[i][(int)Enums.AphbHucreBilgileri.Yil].Equals(iptallerDahil[j][(int)Enums.AphbHucreBilgileri.Yil])
                        && satirlar[i][(int)Enums.AphbHucreBilgileri.Ay].Equals(iptallerDahil[j][(int)Enums.AphbHucreBilgileri.Ay])
                        && satirlar[i][(int)Enums.AphbHucreBilgileri.BelgeTuru].Equals(iptallerDahil[j][(int)Enums.AphbHucreBilgileri.BelgeTuru])
                        && satirlar[i][(int)Enums.AphbHucreBilgileri.Araci].Equals(iptallerDahil[j][(int)Enums.AphbHucreBilgileri.Araci])
                        && satirlar[i][(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].Equals(iptallerDahil[j][(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo])
                        && satirlar[i][(int)Enums.AphbHucreBilgileri.Gun].Equals(iptallerDahil[j][(int)Enums.AphbHucreBilgileri.Gun])
                        && satirlar[i][(int)Enums.AphbHucreBilgileri.GirisGunu] == (iptallerDahil[j][(int)Enums.AphbHucreBilgileri.GirisGunu])
                        && satirlar[i][(int)Enums.AphbHucreBilgileri.CikisGunu] == (iptallerDahil[j][(int)Enums.AphbHucreBilgileri.CikisGunu])
                        && (Convert.ToDecimal(satirlar[i][(int)Enums.AphbHucreBilgileri.Ucret]) + Convert.ToDecimal(satirlar[i][(int)Enums.AphbHucreBilgileri.Ikramiye])).Equals(Convert.ToDecimal(iptallerDahil[j][(int)Enums.AphbHucreBilgileri.Ucret]) + Convert.ToDecimal(iptallerDahil[j][(int)Enums.AphbHucreBilgileri.Ikramiye]))
                        && string.IsNullOrEmpty(iptallerDahil[j][(int)Enums.AphbHucreBilgileri.Kanun].ToString())
                        )
                    {

                        bulundu = true;

                        result.Add(satirlar[i], "00000");

                        break;
                    }
                }

                if (!bulundu)
                {
                    result.Add(satirlar[i], iptalVarsayilanKanun);
                }

            }

            return result;
        }


    }



}
