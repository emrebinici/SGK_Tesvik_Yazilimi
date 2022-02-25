using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        //public static void TumKisileriGetir(DataTable dtaylikliste, out Dictionary<string, List<string>> TesvikVerilenler, out List<KeyValuePair<string, string>> yilveaylar, out Dictionary<KeyValuePair<string, string>, List<KeyValuePair<int, int>>> tumyilveaylar, string Yil, string Ay, string YilBitis, string AyBitis, out DateTime enbuyukay)
        //{
        //    TesvikVerilenler = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new List<string>());

        //    enbuyukay = DateTime.MinValue;

        //    yilveaylar = new List<KeyValuePair<string, string>>();

        //    tumyilveaylar = new Dictionary<KeyValuePair<string, string>, List<KeyValuePair<int, int>>>();

        //    if (Program.TumKisiler == null)
        //    {

        //        TesvikVerilenler = dtaylikliste.AsEnumerable()
        //                                .Where(row => TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.Any(p => row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().Trim().EndsWith(p)))
        //                                .GroupBy(row => TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.FirstOrDefault(t => row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().Trim().EndsWith(t)))
        //                                .ToDictionary(x => x.Key, x => x.Select(row => row[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString().Trim()).Distinct().ToList());

        //        Program.AySatirlari = dtaylikliste.AsEnumerable().GroupBy(row => row[(int)Enums.AphbHucreBilgileri.Yil].ToString() + "-" + row[(int)Enums.AphbHucreBilgileri.Ay].ToString()).ToDictionary(x => x.Key, x => x.ToList());

        //        Program.TumAylar = Program.AySatirlari.Keys.Distinct().ToList();

        //        DateTime baslangic = DateTime.MinValue;

        //        DateTime bitis = DateTime.MaxValue;

        //        if (!string.IsNullOrEmpty(Yil))
        //        {
        //            baslangic = new DateTime(Convert.ToInt32(Yil), string.IsNullOrEmpty(Ay) ? 1 : Convert.ToInt32(Ay), 1);
        //        }

        //        if (!string.IsNullOrEmpty(YilBitis))
        //        {
        //            bitis = new DateTime(Convert.ToInt32(YilBitis), string.IsNullOrEmpty(AyBitis) ? 12 : Convert.ToInt32(AyBitis), 1);
        //        }

        //        if (baslangic > bitis)
        //        {
        //            DateTime temp = baslangic;

        //            baslangic = bitis;

        //            bitis = temp;
        //        }


        //        yilveaylar = Program.TumAylar
        //                    .Where(p => new DateTime(Convert.ToInt32(p.Split('-')[0]), Convert.ToInt32(p.Split('-')[1]), 1) >= baslangic && (new DateTime(Convert.ToInt32(p.Split('-')[0]), Convert.ToInt32(p.Split('-')[1]), 1)) <= bitis)
        //                    .Select(p => new KeyValuePair<string, string>(p.Split('-')[0], p.Split('-')[1]))
        //                    .ToList();


        //        var kisiSatirlari = dtaylikliste.AsEnumerable()
        //                                         .GroupBy(x => x[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString().Trim());

        //        Program.KisilerinSatirlari = kisiSatirlari.ToDictionary(x => x.Key, x => x.GroupBy(row => row[(int)Enums.AphbHucreBilgileri.Yil].ToString() + "-" + row[(int)Enums.AphbHucreBilgileri.Ay].ToString()).ToDictionary(p => p.Key, p => Metodlar.GecerliSatirlariGetir(p.ToList())));
        //        Program.KisilerinSatirlariIptallerDahil = kisiSatirlari.ToDictionary(x => x.Key, x => x.GroupBy(row => row[(int)Enums.AphbHucreBilgileri.Yil].ToString() + "-" + row[(int)Enums.AphbHucreBilgileri.Ay].ToString()).ToDictionary(p => p.Key, p => p.ToList()));

        //        Program.TumKisiler = Program.KisilerinSatirlari.Keys.Select(x => new Kisi { TckimlikNo = x }).ToDictionary(x => x.TckimlikNo, x => x);

        //        if (Program.TumAylar.Count > 0) enbuyukay = Program.TumAylar.Max(x => new DateTime(Convert.ToInt32(x.Split('-')[0]), Convert.ToInt32(x.Split('-')[1]), 1));

        //        tumyilveaylar = Program.TumAylar.ToDictionary(x => new KeyValuePair<string, string>(x.Split('-')[0], x.Split('-')[1]), x => new List<KeyValuePair<int, int>>());
        //    }

        //}

        public static TumKisilerSonuc TumKisileriGetir(DataTable dtaylikliste, string Yil = null, string Ay = null, string YilBitis = null, string AyBitis = null)
        {
            var sonuc = new TumKisilerSonuc();

            sonuc.enbuyukay = DateTime.MinValue;

            sonuc.AySatirlari = dtaylikliste.AsEnumerable().GroupBy(row => row[(int)Enums.AphbHucreBilgileri.Yil].ToString() + "-" + row[(int)Enums.AphbHucreBilgileri.Ay].ToString()).ToDictionary(x => x.Key, x => x.ToList());

            sonuc.TumAylar = sonuc.AySatirlari.Keys.Distinct().ToList();

            sonuc.TesvikVerilenler = dtaylikliste.AsEnumerable()
                                    .Where(row => TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.Any(p => row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().Trim().EndsWith(p)))
                                    .GroupBy(row => TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.FirstOrDefault(t => row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().Trim().EndsWith(t)))
                                    .ToDictionary(x => x.Key, x => x.Select(row => row[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString().Trim()).Distinct().ToList());


            DateTime baslangic = DateTime.MinValue;

            DateTime bitis = DateTime.MaxValue;


            if (!string.IsNullOrEmpty(Yil))
            {
                baslangic = new DateTime(Convert.ToInt32(Yil), string.IsNullOrEmpty(Ay) ? 1 : Convert.ToInt32(Ay), 1);
            }

            if (!string.IsNullOrEmpty(YilBitis))
            {
                bitis = new DateTime(Convert.ToInt32(YilBitis), string.IsNullOrEmpty(AyBitis) ? 12 : Convert.ToInt32(AyBitis), 1);
            }

            if (baslangic > bitis)
            {
                DateTime temp = baslangic;

                baslangic = bitis;

                bitis = temp;
            }


            sonuc.yilveaylar = sonuc.TumAylar
                        .Where(p => new DateTime(Convert.ToInt32(p.Split('-')[0]), Convert.ToInt32(p.Split('-')[1]), 1) >= baslangic && (new DateTime(Convert.ToInt32(p.Split('-')[0]), Convert.ToInt32(p.Split('-')[1]), 1)) <= bitis)
                        .Select(p => new KeyValuePair<string, string>(p.Split('-')[0], p.Split('-')[1]))
                        .ToList();


            var kisiSatirlari = dtaylikliste.AsEnumerable()
                                             .GroupBy(x => x[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString().Trim());

            sonuc.KisilerinSatirlari = kisiSatirlari.ToDictionary(x => x.Key, x => x.GroupBy(row => row[(int)Enums.AphbHucreBilgileri.Yil].ToString() + "-" + row[(int)Enums.AphbHucreBilgileri.Ay].ToString()).ToDictionary(p => p.Key, p => Metodlar.GecerliSatirlariGetir(p.ToList())));
            sonuc.KisilerinSatirlariIptallerDahil = kisiSatirlari.ToDictionary(x => x.Key, x => x.GroupBy(row => row[(int)Enums.AphbHucreBilgileri.Yil].ToString() + "-" + row[(int)Enums.AphbHucreBilgileri.Ay].ToString()).ToDictionary(p => p.Key, p => p.ToList()));

            sonuc.TumKisiler = sonuc.KisilerinSatirlari.Keys.Select(x => new Kisi { TckimlikNo = x , TumKisilerSonuc = sonuc}).ToDictionary(x => x.TckimlikNo, x => x);

            if (sonuc.TumAylar.Count > 0) sonuc.enbuyukay = sonuc.TumAylar.Max(x => new DateTime(Convert.ToInt32(x.Split('-')[0]), Convert.ToInt32(x.Split('-')[1]), 1));

            sonuc.tumyilveaylar = sonuc.TumAylar.ToDictionary(x => new KeyValuePair<string, string>(x.Split('-')[0], x.Split('-')[1]), x => new List<KeyValuePair<int, int>>());

            return sonuc;
        }
    }



}
