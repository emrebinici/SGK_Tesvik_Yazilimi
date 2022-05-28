using System;
using System.Collections.Generic;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static decimal BelgeTuruOranBul(int yil, int ay, string belgeturu, string IsyeriSicilNo, List<string> BelgeTuruOraniHesaplamadaEklenecekAlanlar)
        {
            int belgeturuno = Convert.ToInt32(belgeturu);

            string toplanilacakAlanlar = String.Join("-", BelgeTuruOraniHesaplamadaEklenecekAlanlar);

            var anahtar = belgeturuno.ToString() + "-" + toplanilacakAlanlar;

            decimal toplamoran = 0;

            if (Program.BelgeTuruOranlari.ContainsKey(anahtar))
            {
                toplamoran = Program.BelgeTuruOranlari[anahtar];
            }
            else
            {
                var belgeTuruBilgileri = Program.BelgeTurleri.ContainsKey(belgeturuno) ? Program.BelgeTurleri[belgeturuno] : null;

                if (belgeTuruBilgileri != null)
                {
                    foreach (var item in BelgeTuruOraniHesaplamadaEklenecekAlanlar)
                    {
                        if (item.Equals("GenelSaglikIsveren")) toplamoran += belgeTuruBilgileri.GenelSaglikIsveren.ToDecimal();
                        else if (item.Equals("GenelSaglikSigortali")) toplamoran += belgeTuruBilgileri.GenelSaglikSigortali.ToDecimal();
                        else if (item.Equals("MalulYaslilikOraniIsveren")) toplamoran += belgeTuruBilgileri.MalulYaslilikOraniIsveren.ToDecimal();
                        else if (item.Equals("MalulYaslilikOraniSigortali")) toplamoran += belgeTuruBilgileri.MalulYaslilikOraniSigortali.ToDecimal();
                        else if (item.Equals("SosyalDestekIsveren")) toplamoran += belgeTuruBilgileri.SosyalDestekIsveren.ToDecimal();
                        else if (item.Equals("SosyalDestekSigortali")) toplamoran += belgeTuruBilgileri.SosyalDestekSigortali.ToDecimal();
                        else if (item.Equals("IssizlikIsveren")) toplamoran += belgeTuruBilgileri.IssizlikIsveren.ToDecimal();
                        else if (item.Equals("IssizlikSigortali")) toplamoran += belgeTuruBilgileri.IssizlikSigortali.ToDecimal();
                    }

                    Program.BelgeTuruOranlari.Add(anahtar, toplamoran);
                }
                else
                {
                    if (!TesvikHesaplamaSabitleri.DestekKapsaminaGirmeyenBelgeTurleri.Contains(belgeturu))
                    {
                        throw new Exception(belgeturu + " nolu belge türü için oranlar girilmemiş.Lütfen yapmakta olduğunuz işlemi iptal edip belge türünü ekledikten sonra yeniden deneyiniz");
                    }
                }
            }

            decimal kvsk = Metodlar.KvskBul(yil, ay, IsyeriSicilNo);

            return toplamoran + kvsk;
        }


    }


}
