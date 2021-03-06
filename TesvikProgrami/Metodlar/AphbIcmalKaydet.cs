using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Excel2 = Microsoft.Office.Interop.Excel;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {

        public static Classes.BaseResponse AphbIcmalKaydet(Isyerleri isyeri, Dictionary<string, List<Classes.BildirgeYuklemeIcmal>> tumu)
        {
            Genel.IcmalKaydediliyorKontrolu();

            var response = new Classes.BaseResponse();

            try
            {
                var isyeripath = Metodlar.IsyeriKlasorBul(isyeri, true);

                string YeniPath = Path.Combine(isyeripath, String.Format("Aphb İcmal {0}.xlsx", DateTime.Today.ToString("dd.MM.yyyy")));

                List<object> HafizadanAtilacaklar = new List<object>();

                try
                {
                    bool result = false;

                    var icmalCikartilacaklar = new Dictionary<string, Dictionary<string, List<Classes.BildirgeYuklemeIcmal>>>();
                    icmalCikartilacaklar.Add("Tümü", tumu);

                    if (icmalCikartilacaklar.Count > 0)
                    {
                        var Excelim = new Excel2.Application();

                        Excelim.Visible = false;
                        Excelim.DisplayAlerts = false;

                        Excel2.Workbook MyBook = null;

                        Excel2.Worksheet MySheet = null;

                        object SalakObje = System.Reflection.Missing.Value;

                        var workbooks = Excelim.Workbooks;

                        MyBook = workbooks.Open(Path.Combine(Application.StartupPath, "Icmal.xlsx"));

                        MySheet = (Excel2.Worksheet)MyBook.Sheets[1];

                        HafizadanAtilacaklar.AddRange(new List<object> { workbooks ,MyBook, MySheet });

                        var siraNo = 1;

                        for (int i = 1; i < icmalCikartilacaklar.Count; i++)
                        {
                            var sheetcopy = MyBook.Sheets[siraNo];

                            MySheet.Copy(After: sheetcopy);

                            HafizadanAtilacaklar.Add(sheetcopy);

                            var sheet = MyBook.Sheets[siraNo+1];

                            HafizadanAtilacaklar.Add(sheet);

                            siraNo++;
                        }

                        var sheets = MyBook.Sheets;

                        HafizadanAtilacaklar.Add(sheets);


                        for (int no = 0; no < icmalCikartilacaklar.Count; no++)
                        {
                            var sayfano = no + 1;

                            MySheet = sheets[sayfano];

                            MySheet.Name = icmalCikartilacaklar.ElementAt(no).Key;

                            var icmalTuru = icmalCikartilacaklar.ElementAt(no).Key;

                            HafizadanAtilacaklar.Add(MySheet);

                            var kayitlar = icmalCikartilacaklar.ElementAt(no).Value;

                            List<string> OlusturulacakIcmaller = kayitlar.Where(p => p.Value.Count > 0).Select(p => p.Key).ToList();

                            int ToplamTesvikTuru = OlusturulacakIcmaller.Count;

                            var icmalCikartilacakAylar = kayitlar.SelectMany(p => p.Value.Select(x => x.yilay)).Select(p => new DateTime(Convert.ToInt32(p.Key), Convert.ToInt32(p.Value), 1));

                            #region Genel Icmal

                            if (ToplamTesvikTuru > 0)
                            {

                                var tesvikYillar = Program.TumTesvikler.ToDictionary(x => x.Key, x => new SortedDictionary<int, SortedDictionary<int, Classes.BildirgeYuklemeIcmal>>());

                                SortedDictionary<int, SortedDictionary<int, int>> tumyillar = new SortedDictionary<int, SortedDictionary<int, int>>();

                                List<string> MahsupYapilacakIcmalKanunlari = new List<string>();

                                foreach (var tesvikItem in Program.TumTesvikler)
                                {
                                    var kanun = tesvikItem.Key;

                                    var tesvik = tesvikItem.Value;

                                    foreach (var tarih in icmalCikartilacakAylar)
                                    {
                                        var kanunYillar = tesvikYillar[kanun];

                                        if (!kanunYillar.ContainsKey(tarih.Year)) kanunYillar.Add(tarih.Year, new SortedDictionary<int, Classes.BildirgeYuklemeIcmal>());

                                        SortedDictionary<int, Classes.BildirgeYuklemeIcmal> aylar = kanunYillar[tarih.Year];

                                        Classes.BildirgeYuklemeIcmal icmal = null;

                                        if (kayitlar.ContainsKey(kanun))
                                        {
                                            var icmaller= kayitlar[kanun].Where(p => p.yilay.Key.Equals(tarih.Year.ToString()) && p.yilay.Value.Equals(tarih.Month.ToString()));
                                            if (icmaller.Count() > 0)
                                            {
                                                icmal = new Classes.BildirgeYuklemeIcmal();

                                                foreach (var yukluicmal in icmaller)
                                                {
                                                    icmal.Matrah += yukluicmal.Matrah;
                                                    icmal.PrimOdenenGunSayisi += yukluicmal.PrimOdenenGunSayisi;
                                                    icmal.Tutar += yukluicmal.Tutar;
                                                }
                                            }
                                        }

                                        if (!aylar.ContainsKey(tarih.Month)) aylar.Add(tarih.Month, icmal);

                                        if (!tumyillar.ContainsKey(tarih.Year)) tumyillar.Add(tarih.Year, new SortedDictionary<int, int>());

                                        SortedDictionary<int, int> aylar2 = tumyillar[tarih.Year];

                                        if (!aylar2.ContainsKey(tarih.Month)) aylar2.Add(tarih.Month, 0);
                                    }
                                }

                                var TesvikVerilecekKanunlar = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.Select(p => p).ToList();
                                if (!OlusturulacakIcmaller.Contains("6322/25510")) TesvikVerilecekKanunlar.Remove("6322/25510");
                                if (!OlusturulacakIcmaller.Contains("5510")) TesvikVerilecekKanunlar.Remove("5510");

                                var icmalDegerYazilacakAlanlar = Program.TumTesvikler.ToDictionary(x => x.Key, x => new Dictionary<string, Excel2.Range>());
                                icmalDegerYazilacakAlanlar.Add("Tumu", new Dictionary<string, Excel2.Range>());

                                //var toplamIcmalTutarlari = icmalDegerYazilacakAlanlar.ToDictionary(x => x.Key, x => 0.0);

                                var IcmalIsyeriAd = MySheet.Range[IcmalOlusturmaSabitleri.IcmalIsyeriAd];
                                var IcmalBaslik1 = MySheet.Range[IcmalOlusturmaSabitleri.IcmalBaslik1];
                                var IcmalBaslik2 = MySheet.Range[IcmalOlusturmaSabitleri.IcmalBaslik2];
                                var IcmalIsyeriSicil = MySheet.Range[IcmalOlusturmaSabitleri.IcmalIsyeriSicil];


                                IcmalIsyeriAd.Value2 = isyeri.SubeAdi.ToUpper();

                                IcmalBaslik1.Value2 = IcmalOlusturmaSabitleri.IcmalBaslik1Tum;

                                IcmalBaslik2.Value2 = IcmalOlusturmaSabitleri.IcmalBaslik2Tum;

                                IcmalIsyeriSicil.Value2 = " " + isyeri.IsyeriSicilNo;

                                try
                                {
                                    List<string> isyerisicils = new List<string>();

                                    isyerisicils.Add(isyeri.IsyeriSicilNo.Substring(0, 1));

                                    isyerisicils.Add(isyeri.IsyeriSicilNo.Substring(1, 4));

                                    isyerisicils.Add(isyeri.IsyeriSicilNo.Substring(5, 2));

                                    isyerisicils.Add(isyeri.IsyeriSicilNo.Substring(7, 2));

                                    isyerisicils.Add(isyeri.IsyeriSicilNo.Substring(9, 7));

                                    isyerisicils.Add(isyeri.IsyeriSicilNo.Substring(16, 3));

                                    isyerisicils.Add(isyeri.IsyeriSicilNo.Substring(19, 2));

                                    string isyerisicilno = String.Join(" ", isyerisicils.ToArray()).Trim();

                                    isyerisicilno += "-" + isyeri.IsyeriSicilNo.Substring(21, 2);

                                    IcmalIsyeriSicil.Value2 = isyerisicilno;

                                }
                                catch
                                {

                                }

                                HafizadanAtilacaklar.AddRange(new List<object> { IcmalIsyeriAd, IcmalBaslik1, IcmalBaslik2, IcmalIsyeriSicil });

                                List<Excel2.Range> headers = new List<Excel2.Range>();

                                List<Excel2.Range> rows = new List<Excel2.Range>();

                                List<Excel2.Range> yiltoplamlari = new List<Excel2.Range>();

                                int Satir = IcmalOlusturmaSabitleri.IcmalBaslangicSatir;

                                int CiftSutun = IcmalOlusturmaSabitleri.IcmalCiftBaslangicSutun;

                                var enumeratoryil = tumyillar.GetEnumerator();

                                int i = 0;

                                while (enumeratoryil.MoveNext())
                                {
                                    int yil = enumeratoryil.Current.Key;

                                    int Sutun = CiftSutun;

                                    var headerDonem = MySheet.Cells[Satir + i * 15, Sutun] as Excel2.Range;

                                    headerDonem.Value2 = "DÖNEM";

                                    int sira = 1;

                                    for (int p = 0; p < TesvikVerilecekKanunlar.Count; p++)
                                    {
                                        var headerKanun = MySheet.Cells[Satir + i * 15, Sutun + sira] as Excel2.Range;

                                        headerKanun.Value2 = TesvikVerilecekKanunlar[p];

                                        HafizadanAtilacaklar.Add(headerKanun);

                                        sira++;
                                    }

                                    var headerTumu = MySheet.Cells[Satir + i * 15, Sutun + sira] as Excel2.Range;

                                    headerTumu.Value2 = "TÜMÜ";

                                    var baslangic = MySheet.Cells[Satir + i * 15, Sutun];

                                    var bitis = MySheet.Cells[Satir + i * 15, Sutun + sira];

                                    var allheaders = MySheet.Range[baslangic, bitis];

                                    headers.Add(allheaders);

                                    HafizadanAtilacaklar.AddRange(new List<object> { headerDonem, headerTumu, allheaders, baslangic, bitis });

                                    var enumeratoray = enumeratoryil.Current.Value.GetEnumerator();

                                    int j = 1;

                                    while (enumeratoray.MoveNext())
                                    {
                                        int ay = enumeratoray.Current.Key;

                                        Excel2.Range cellyilay = MySheet.Cells[Satir + j + i * 15, Sutun] as Excel2.Range;

                                        cellyilay.Value2 = yil.ToString() + "/" + ay.ToString();

                                        int sira2 = 1;

                                        for (int p = 0; p < TesvikVerilecekKanunlar.Count; p++)
                                        {
                                            var kanun = TesvikVerilecekKanunlar[p];

                                            var cellayKanunTutari = MySheet.Cells[Satir + j + i * 15, Sutun + sira2] as Excel2.Range;

                                            icmalDegerYazilacakAlanlar[kanun].Add(yil + "-" + ay, cellayKanunTutari);

                                            HafizadanAtilacaklar.Add(cellayKanunTutari);

                                            sira2++;
                                        }


                                        Excel2.Range cellAyToplam = MySheet.Cells[Satir + j + i * 15, Sutun + sira2] as Excel2.Range;

                                        icmalDegerYazilacakAlanlar["Tumu"].Add(yil + "-" + ay, cellAyToplam);

                                        HafizadanAtilacaklar.AddRange(new List<object> { cellyilay, cellAyToplam });

                                        j++;
                                    }

                                    var baslangicrow = MySheet.Cells[Satir + 1 + i * 15, Sutun];

                                    var bitisrow = MySheet.Cells[Satir + j - 1 + i * 15, Sutun + TesvikVerilecekKanunlar.Count + 1];

                                    var tumsatir = MySheet.Range[baslangicrow, bitisrow];

                                    rows.Add(tumsatir);

                                    HafizadanAtilacaklar.AddRange(new List<object> { baslangicrow, bitisrow, tumsatir });

                                    Excel2.Range cellyilToplami = MySheet.Cells[Satir + j + i * 15, Sutun] as Excel2.Range;

                                    cellyilToplami.Value2 = "Yıl toplamı";

                                    sira = 1;

                                    for (int p = 0; p < TesvikVerilecekKanunlar.Count; p++)
                                    {
                                        var cellyilKanun = MySheet.Cells[Satir + j + i * 15, Sutun + sira] as Excel2.Range;

                                        var kanun = TesvikVerilecekKanunlar[p];

                                        icmalDegerYazilacakAlanlar[kanun].Add(yil.ToString(), cellyilKanun);

                                        HafizadanAtilacaklar.Add(cellyilKanun);

                                        sira++;
                                    }

                                    var cellYilTumu = MySheet.Cells[Satir + j + i * 15, Sutun + sira] as Excel2.Range;
                                    icmalDegerYazilacakAlanlar["Tumu"].Add(yil.ToString(), cellYilTumu);

                                    var baslangicyil = MySheet.Cells[Satir + j + i * 15, Sutun];

                                    var bitisyil = MySheet.Cells[Satir + j + i * 15, Sutun + TesvikVerilecekKanunlar.Count + 1];

                                    var yiltoplamitumsatir = MySheet.Range[baslangicyil, bitisyil];

                                    yiltoplamlari.Add(yiltoplamitumsatir);

                                    HafizadanAtilacaklar.AddRange(new List<object> { cellyilToplami, yiltoplamitumsatir, baslangicyil, bitisyil });

                                    i++;
                                }

                                foreach (Excel2.Range r in headers)
                                {
                                    var font = r.Font;

                                    font.Bold = true;

                                    font.Name = "Times New Roman";

                                    font.Size = 10;

                                    r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                                    r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                    var interior = r.Interior;

                                    interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(234, 241, 221));

                                    var borders = r.Borders;

                                    borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                    HafizadanAtilacaklar.AddRange(new List<object> { font, interior, r, borders });
                                }

                                foreach (Excel2.Range r in rows)
                                {
                                    var font = r.Font;

                                    font.Bold = false;

                                    font.Name = "Times New Roman";

                                    font.Size = 10;

                                    r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                                    r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignRight;

                                    var interior = r.Interior;

                                    interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(197, 217, 241));

                                    var borders = r.Borders;

                                    borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                    HafizadanAtilacaklar.AddRange(new List<object> { font, interior, r, borders });

                                }

                                foreach (Excel2.Range r in yiltoplamlari)
                                {
                                    var font = r.Font;

                                    font.Bold = true;

                                    font.Name = "Times New Roman";

                                    font.Size = 10;

                                    r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                                    r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignRight;

                                    //r.BorderAround(Excel2.XlLineStyle.xlContinuous);

                                    var borders = r.Borders;

                                    borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                    HafizadanAtilacaklar.AddRange(new List<object> { font, r, borders });

                                }

                                int str = Satir + i * 15;

                                int stn = CiftSutun;

                                List<Excel2.Range> ranges = new List<Excel2.Range>();

                                var rangebaslangic = MySheet.Cells[str, stn];

                                var rangebitis = MySheet.Cells[str + 6, stn + TesvikVerilecekKanunlar.Count - 1];

                                Excel2.Range range = MySheet.Range[rangebaslangic, rangebitis];

                                int siratum = 0;

                                for (int p = 0; p < TesvikVerilecekKanunlar.Count; p++)
                                {
                                    var rangeKanunHeader = (Excel2.Range)MySheet.Cells[str + 7, stn + siratum];
                                    var rangeKanunTumu = (Excel2.Range)MySheet.Cells[str + 8, stn + siratum];

                                    var kanun = TesvikVerilecekKanunlar[p];

                                    rangeKanunHeader.Value2 = kanun;

                                    icmalDegerYazilacakAlanlar[kanun].Add("Tumu", rangeKanunTumu);

                                    HafizadanAtilacaklar.Add(rangeKanunHeader);
                                    HafizadanAtilacaklar.Add(rangeKanunTumu);

                                    siratum++;
                                }

                                var rangetutarbaslangic = MySheet.Cells[str + 9, stn];

                                var rangetutarbitis = MySheet.Cells[str + 12, stn + TesvikVerilecekKanunlar.Count - 1];

                                Excel2.Range rangetutar = MySheet.Range[rangetutarbaslangic, rangetutarbitis];

                                icmalDegerYazilacakAlanlar["Tumu"].Add("Tumu", rangetutar);

                                var allranges = MySheet.Range[rangebaslangic, rangetutarbitis];

                                ranges = new List<Excel2.Range> { allranges };

                                HafizadanAtilacaklar.AddRange(new List<object> { range, rangetutar, allranges, rangebaslangic, rangebitis, rangetutarbaslangic, rangetutarbitis });

                                range.Merge();

                                range.Value2 = "Teşvik kapsamında işveren tarafından iade alınacak olan toplam prim tutarı(kanuni faiz hariç)";

                                rangetutar.Merge();

                                foreach (var rng in ranges)
                                {
                                    var font = rng.Font;

                                    font.Bold = true;

                                    font.Name = "Times New Roman";

                                    font.Size = 10;

                                    rng.WrapText = true;

                                    rng.VerticalAlignment = 2;

                                    rng.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                    var interior = rng.Interior;

                                    interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(217, 151, 149));

                                    var borders = rng.Borders;

                                    borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                    rng.BorderAround(Excel2.XlLineStyle.xlContinuous, Excel2.XlBorderWeight.xlMedium);

                                    HafizadanAtilacaklar.AddRange(new List<object> { font, rng, interior, borders });
                                }

                                var fonttutar = rangetutar.Font;

                                fonttutar.Size = 15;

                                HafizadanAtilacaklar.Add(fonttutar);

                                var tumToplamlar = TesvikVerilecekKanunlar.ToDictionary(x => x, x => 0.0m);

                                enumeratoryil = tumyillar.GetEnumerator();

                                i = 0;

                                while (enumeratoryil.MoveNext())
                                {
                                    var yilToplamlari = TesvikVerilecekKanunlar.ToDictionary(x => x, x => 0.0m);

                                    int yil = enumeratoryil.Current.Key;

                                    int j = 1;

                                    var enumeratoray = enumeratoryil.Current.Value.GetEnumerator();

                                    while (enumeratoray.MoveNext())
                                    {
                                        int ay = enumeratoray.Current.Key;

                                        var kanunAyIcmalleri = tesvikYillar.ToDictionary(x => x.Key, x => x.Value.ContainsKey(yil) && x.Value[yil].ContainsKey(ay) ? x.Value[yil][ay] : null);

                                        decimal ayTumTesviklerToplam = 0;

                                        foreach (var kanun in TesvikVerilecekKanunlar)
                                        {
                                            Classes.BildirgeYuklemeIcmal icmal = kanunAyIcmalleri[kanun];

                                            var icmalTutar = icmal != null ? icmal.Tutar : 0;

                                            icmalDegerYazilacakAlanlar[kanun][yil + "-" + ay].Value2 = icmalTutar.ToTL();

                                            ayTumTesviklerToplam += icmalTutar.ToTL().Replace("₺", "").ToDecimalSgk();

                                            yilToplamlari[kanun] += icmalTutar.ToTL().Replace("₺", "").ToDecimalSgk();
                                            tumToplamlar[kanun] += icmalTutar.ToTL().Replace("₺", "").ToDecimalSgk();

                                        }

                                        icmalDegerYazilacakAlanlar["Tumu"][yil + "-" + ay].Value2 = ayTumTesviklerToplam.ToTL();

                                        j++;
                                    }

                                    decimal yiltumTesviklerToplam = 0;

                                    foreach (var kanun in TesvikVerilecekKanunlar)
                                    {
                                        icmalDegerYazilacakAlanlar[kanun][yil.ToString()].Value2 = yilToplamlari[kanun].ToTL();

                                        yiltumTesviklerToplam += yilToplamlari[kanun].ToTL().Replace("₺", "").ToDecimalSgk();
                                    }

                                    icmalDegerYazilacakAlanlar["Tumu"][yil.ToString()].Value2 = yiltumTesviklerToplam.ToTL();

                                    i++;
                                }

                                decimal tumTesviklerToplam = 0;

                                foreach (var kanun in TesvikVerilecekKanunlar)
                                {
                                    icmalDegerYazilacakAlanlar[kanun]["Tumu"].Value2 = tumToplamlar[kanun].ToTL();

                                    tumTesviklerToplam += tumToplamlar[kanun].ToTL().Replace("₺", "").ToDecimalSgk() ;
                                }

                                icmalDegerYazilacakAlanlar["Tumu"]["Tumu"].Value2 = tumTesviklerToplam.ToTL();
                            }

                            #endregion
                        }

                        var sheetall = MyBook.Sheets;
                        var ws = (Excel2.Worksheet)sheetall[1];
                        ws.Activate();

                        HafizadanAtilacaklar.AddRange(new List<object> { sheetall, ws });

                        Excelim.Visible = false;
                        Excelim.DisplayAlerts = false;

                        int excelprocessid = Metodlar.GetExcelProcessId(Excelim);


                        try
                        {
                            var path = Path.GetDirectoryName(YeniPath);

                            var files = Directory.GetFiles(path, "Aphb İcmal*.xlsx");

                            foreach (var file in files)
                            {
                                File.Delete(file);
                            }

                            MyBook.SaveAs(YeniPath);

                            result = true;
                        }
                        catch (Exception ex)
                        {
                            Metodlar.HataMesajiGoster(ex, "Aphb icmali kaydedilirken hata oluştu.");

                            result = false;
                        }

                        MyBook.Close(false);


                        HafizadanAtilacaklar.Reverse();

                        int m = 0;

                        while (m < HafizadanAtilacaklar.Count())
                        {
                            try
                            {
                                var item = HafizadanAtilacaklar.ElementAt(m);

                                if (item != null)
                                {

                                    Marshal.FinalReleaseComObject(item);
                                }

                                item = null;

                            }
                            catch
                            {
                            }

                            m++;
                        }


                        Excelim.Quit();
                        Marshal.FinalReleaseComObject(Excelim);


                        Metodlar.KillProcessById(excelprocessid);

                    }

                    if (result)
                    {
                        response.Result = YeniPath;
                        response.Durum = true;

                        return response;
                    }

                }
                catch (Exception ex)
                {
                    string Mesaj = "Aphb icmali hata nedeniyle kaydedilemedi" + Environment.NewLine;

                    HataMesajiGoster(ex, Mesaj);
                }
            }
            catch (Exception ex)
            {
                Genel.IcmalKaydediliyorKilidiniKaldir();

                throw ex;
            }
            finally
            {
                Genel.IcmalKaydediliyorKilidiniKaldir();
            }

            Genel.IcmalKaydediliyorKilidiniKaldir();

            return response;
        }


    }


}
