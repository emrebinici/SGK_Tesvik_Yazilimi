using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Excel2 = Microsoft.Office.Interop.Excel;

namespace TesvikProgrami.Classes
{
    public class ExceleYaz
    {
        List<object> HafizadanAtilacaklar = new List<object>();

        static bool BfKaydediliyor = false;
        static bool AphbKaydediliyor = false;
        static bool Kaydediliyor7166 = false;

        public bool OkuVeYaz(System.Data.DataTable DisDataGrid, string eskiyol, string filename, DataTable dtEski)
        {
            while (AphbKaydediliyor)
            {
                Thread.Sleep(500);
            }

            AphbKaydediliyor = true;

            bool result = true;

            try
            {
                if (DisDataGrid.Rows.Count > 0)
                {

                    Excel2.Application Excelim = new Excel2.Application();
                    object SalakObje = System.Reflection.Missing.Value;

                    var workbooks = Excelim.Workbooks;

                    Excel2.Workbook CalismaKitabi = workbooks.Open(eskiyol != null ? eskiyol : Path.Combine(Application.StartupPath, "ListeTemplate.xlsx"));
                    Excel2.Worksheet CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                    var cells = CalismaSayfasi.Cells;

                    Excel2.Range last = cells.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Type.Missing);
                    //Excel2.Range range = CalismaSayfasi.get_Range("A1", last);

                    HafizadanAtilacaklar.AddRange(new List<object> { workbooks, CalismaKitabi, CalismaSayfasi, cells, last });

                    int LastRow = last.Row;

                    if (File.Exists(eskiyol))
                    {
                        var trh = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        var trhOncekiAy = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);

                        string Yil = trh.Year.ToString();
                        string Ay = trh.Month.ToString().PadLeft(2, '0');

                        string YilOncekiAy = trhOncekiAy.Year.ToString();
                        string OncekiAy = trhOncekiAy.Month.ToString().PadLeft(2, '0');

                        int baslangicsatiri = -1;

                        int bitissatiri = -1;

                        List<KeyValuePair<int, int>> silinecekdiziler = new List<KeyValuePair<int, int>>();

                        for (int i = 0; i < dtEski.Rows.Count; i++)
                        {
                            string YilTemp = dtEski.Rows[i][0].ToString();

                            string AyTemp = dtEski.Rows[i][1].ToString().PadLeft(2, '0');

                            string OnayDurumu = dtEski.Rows[i][(int)Enums.AphbHucreBilgileri.OnayDurumu].ToString();

                            if (baslangicsatiri == -1)
                            {
                                if ((Yil == YilTemp && Ay == AyTemp) || (YilOncekiAy == YilTemp && OncekiAy == AyTemp) || OnayDurumu.Equals("Onaylanmamış"))
                                {
                                    baslangicsatiri = i;
                                }
                            }
                            else
                            {
                                if ((Yil == YilTemp && Ay == AyTemp) || (YilOncekiAy == YilTemp && OncekiAy == AyTemp) || OnayDurumu.Equals("Onaylanmamış"))
                                {
                                    bitissatiri = i;
                                }
                                else
                                {
                                    if (bitissatiri == -1)
                                    {
                                        bitissatiri = baslangicsatiri;
                                    }

                                    silinecekdiziler.Add(new KeyValuePair<int, int>(baslangicsatiri + 2, bitissatiri + 2));

                                    baslangicsatiri = -1;

                                    bitissatiri = -1;
                                }
                            }
                        }

                        if (baslangicsatiri > -1)
                        {
                            if (bitissatiri == -1)
                            {
                                bitissatiri = baslangicsatiri;
                            }

                            silinecekdiziler.Add(new KeyValuePair<int, int>(baslangicsatiri + 2, bitissatiri + 2));
                        }

                        for (int i = silinecekdiziler.Count - 1; i >= 0; i--)
                        {
                            var pagerows = CalismaSayfasi.Rows;

                            var baslangicrow = pagerows[silinecekdiziler[i].Key];
                            var bitisrow = pagerows[silinecekdiziler[i].Value];

                            Excel2.Range rng = CalismaSayfasi.Range[baslangicrow, bitisrow];

                            HafizadanAtilacaklar.AddRange(new List<object> { baslangicrow, bitisrow, rng, pagerows });

                            rng.Delete(Excel2.XlDeleteShiftDirection.xlShiftUp);

                            LastRow -= (silinecekdiziler[i].Value - silinecekdiziler[i].Key + 1);
                        }
                    }


                    int SatirIndex = LastRow + 1;

                    NewExportExcelV2 excelV2 = new NewExportExcelV2();
                    excelV2.gridControl1.DataSource = DisDataGrid;
                    excelV2.gridView1.Columns[0].Caption = "YIL";
                    excelV2.gridView1.Columns[1].Caption = "AY";
                    excelV2.gridView1.Columns[2].Caption = "KANUN NO";
                    excelV2.gridView1.Columns[3].Caption = "MAHİYETİ";
                    excelV2.gridView1.Columns[4].Caption = "BELGE TÜRÜ";
                    excelV2.gridView1.Columns[5].Caption = "SNO";
                    excelV2.gridView1.Columns[6].Caption = "S.GÜVENLİK NO";
                    excelV2.gridView1.Columns[8].Caption = "ADI";
                    excelV2.gridView1.Columns[9].Caption = "SOYADI";
                    excelV2.gridView1.Columns[10].Caption = "İLK SOYADI";
                    excelV2.gridView1.Columns[11].Caption = "UCRET TL";
                    excelV2.gridView1.Columns[12].Caption = "İKRAMİYE TL";
                    excelV2.gridView1.Columns[13].Caption = "GÜN";
                    excelV2.gridView1.Columns[14].Caption = "EKSİK GS";
                    excelV2.gridView1.Columns[15].Caption = "G.GÜN";
                    excelV2.gridView1.Columns[16].Caption = "Ç.GÜN";
                    excelV2.gridView1.Columns[17].Caption = "EGS";
                    excelV2.gridView1.Columns[18].Caption = "İÇN";
                    excelV2.gridView1.Columns[19].Caption = "MESLEK KOD";
                    excelV2.gridView1.Columns[22].Caption = "ARACI";
                    excelV2.gridView1.Columns[23].Caption = "ONAY DURUMU";
                    excelV2.gridView1.Columns[24].Caption = "ORİJİNAL KANUN NO";
                    excelV2.gridView1.Columns[25].Caption = "BİLDİRGE NO";
                    string geciciDosyaYolu = filename.Insert(filename.IndexOf(".xlsx"), "_GEÇİCİ");
                    excelV2.gridView1.ExportToXlsx(geciciDosyaYolu);


                    var ExcelimV2 = new Excel2.Application();
                    var workbooks2 = ExcelimV2.Workbooks;
                    var CalismaKitabiV2 = workbooks2.Open(geciciDosyaYolu);

                    HafizadanAtilacaklar.AddRange(new List<object> { workbooks2, CalismaKitabiV2 });

                    bool BasariliKaydedildi = false;

                    int hataSayaci = 0;

                    while (hataSayaci < 3)
                    {
                        try
                        {
                            object SalakObjeV2 = System.Reflection.Missing.Value;
                            var CalismaSayfasiV2 = (Excel2.Worksheet)CalismaKitabiV2.ActiveSheet;

                            cells = CalismaSayfasi.Cells;

                            Excel2.Range lastcell = cells.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Type.Missing);

                            var cellsV2 = CalismaSayfasiV2.Cells;

                            Excel2.Range lastV2 = cellsV2.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Type.Missing);

                            //((Excel2.Range)CalismaSayfasi.Cells[1, Sabitler.AylikListeSutunlari[Sabitler.MeslekKod] + 1]).EntireColumn.NumberFormat = "@";

                            var kopyalanacakAlan = CalismaSayfasiV2.Range["A2", lastV2];

                            kopyalanacakAlan.Copy(Type.Missing);

                            Excel2.Range rangeV2 = cells[SatirIndex, 1] as Excel2.Range;
                            rangeV2.Select();
                            CalismaSayfasi.Paste(Type.Missing, Type.Missing);

                            Excel2.Range lastcell2 = cells.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Type.Missing);

                            HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasiV2,cells, lastcell, cellsV2, lastV2, kopyalanacakAlan, rangeV2, lastcell2 });


                            if (LastRow + lastV2.Row - 1 > lastcell2.Row)
                            {
                                CalismaKitabi.Close(false);

                                CalismaKitabi = workbooks.Open(eskiyol != null ? eskiyol : Path.Combine(Application.StartupPath, "ListeTemplate.xlsx"));
                                CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                                HafizadanAtilacaklar.AddRange(new List<object> { CalismaKitabi, CalismaSayfasi });

                                continue;
                            }

                            var tumAlan = CalismaSayfasi.Range["A:X"];

                            tumAlan.NumberFormat = "@";

                            //CalismaSayfasi.Range["A:K"].NumberFormat = "@";
                            //CalismaSayfasi.Range["N:X"].NumberFormat = "@";

                            //CalismaSayfasi.Range["A:X"].Font.Size = 12;


                            var cellsYeni = CalismaSayfasi.Cells;

                            var lastcellSon = cellsYeni.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Type.Missing);

                            var tumAlanSon = CalismaSayfasi.Range["A2", lastcellSon];

                            var tumAlanBorders = tumAlanSon.Borders;

                            tumAlanBorders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                            dynamic allDataRange = CalismaSayfasi.UsedRange;

                            var columns = allDataRange.Columns;

                            var onayDurumuSutun = columns[24];

                            var YilSutun = columns[1];

                            var AySutun = columns[2];

                            var BelgeTuruSutun = columns[(int)Enums.AphbHucreBilgileri.BelgeTuru + 1];

                            var AraciSutun = columns[(int)Enums.AphbHucreBilgileri.Araci + 1];

                            //var sonuc = allDataRange.Sort(AraciSutun, Excel2.XlSortOrder.xlDescending, BelgeTuruSutun, Type.Missing, Excel2.XlSortOrder.xlAscending, Type.Missing, Type.Missing, Excel2.XlYesNoGuess.xlYes);

                            //sonuc = allDataRange.Sort(onayDurumuSutun, Excel2.XlSortOrder.xlDescending, YilSutun, Type.Missing, Excel2.XlSortOrder.xlDescending, AySutun, Excel2.XlSortOrder.xlDescending, Excel2.XlYesNoGuess.xlYes);

                            var sonuc = allDataRange.Sort(onayDurumuSutun, Excel2.XlSortOrder.xlDescending, BelgeTuruSutun, Type.Missing, Excel2.XlSortOrder.xlAscending, Type.Missing, Type.Missing, Excel2.XlYesNoGuess.xlYes);

                            sonuc = allDataRange.Sort(YilSutun, Excel2.XlSortOrder.xlDescending, AySutun, Type.Missing, Excel2.XlSortOrder.xlDescending, AraciSutun, Excel2.XlSortOrder.xlDescending, Excel2.XlYesNoGuess.xlYes);

                            HafizadanAtilacaklar.AddRange(new List<object> { tumAlan, cellsYeni, lastcellSon, tumAlanSon, tumAlanBorders, allDataRange, columns, onayDurumuSutun, YilSutun, AySutun, BelgeTuruSutun, AraciSutun });

                            BasariliKaydedildi = true;

                            break;

                        }
                        catch (Exception ex)
                        {
                            hataSayaci++;

                            frmIsyerleri.LogYaz("HATA OLUŞTU:" + ex.Message + Environment.NewLine);

                        }
                    }

                    var allcells2 = CalismaSayfasi.Cells;

                    var cellOnayDurumu = (Excel2.Range)allcells2[1, (int)Enums.AphbHucreBilgileri.OnayDurumu + 1];

                    cellOnayDurumu.Copy(System.Reflection.Missing.Value);

                    var cellOrijinalKanunNo = (Excel2.Range)allcells2[1, (int)Enums.AphbHucreBilgileri.OrijinalKanunNo + 1];

                    cellOrijinalKanunNo.PasteSpecial(Excel2.XlPasteType.xlPasteFormats, Excel2.XlPasteSpecialOperation.xlPasteSpecialOperationNone, false, false);

                    cellOrijinalKanunNo.Value2 = "Orijinal Kanun No";

                    cellOnayDurumu.Copy(System.Reflection.Missing.Value);

                    var cellBildirgeNo = (Excel2.Range)allcells2[1, (int)Enums.AphbHucreBilgileri.BildirgeRefNo + 1];

                    cellBildirgeNo.PasteSpecial(Excel2.XlPasteType.xlPasteFormats, Excel2.XlPasteSpecialOperation.xlPasteSpecialOperationNone, false, false);

                    cellBildirgeNo.Value2 = "Bildirge No";

                    HafizadanAtilacaklar.AddRange(new List<object> { allcells2, cellOnayDurumu, cellOrijinalKanunNo , cellBildirgeNo});

                    try
                    {
                        var ilkhucre = allcells2[1, 2] as Excel2.Range;

                        ilkhucre.Select();

                        HafizadanAtilacaklar.AddRange(new List<object> { ilkhucre });
                    }
                    catch { }



                    Excelim.Visible = false;
                    Excelim.DisplayAlerts = false;
                    ExcelimV2.Visible = false;
                    ExcelimV2.DisplayAlerts = false;

                    int excelprocessid = Metodlar.GetExcelProcessId(Excelim);
                    int excelprocessid2 = Metodlar.GetExcelProcessId(ExcelimV2);

                    try
                    {
                        if (BasariliKaydedildi)
                        {
                            if (eskiyol == null)
                            {
                                if (File.Exists(filename)) File.Delete(filename);

                                CalismaKitabi.SaveAs(filename);
                            }
                            else
                            {
                                CalismaKitabi.Save();
                            }
                        }
                        else
                        {
                            result = false;
                        }

                    }
                    catch (Exception ex)
                    {
                        Metodlar.HataMesajiGoster(ex, "Aphb dosyası kaydedilirken hata oluştu.");

                        result = false;
                    }


                    CalismaKitabi.Close(false);
                    CalismaKitabiV2.Close(false);

                    HafizadanAtilacaklar.Reverse();

                    int j = 0;

                    while (j < HafizadanAtilacaklar.Count())
                    {
                        try
                        {
                            var item = HafizadanAtilacaklar.ElementAt(j);

                            Marshal.FinalReleaseComObject(item);

                            item = null;

                        }
                        catch
                        {
                        }

                        j++;
                    }


                    Excelim.Quit();
                    Marshal.FinalReleaseComObject(Excelim);
                    ExcelimV2.Quit();
                    Marshal.FinalReleaseComObject(ExcelimV2);

                    Metodlar.KillProcessById(excelprocessid);

                    Metodlar.KillProcessById(excelprocessid2);

                    if (File.Exists(geciciDosyaYolu))
                    {
                        File.Delete(geciciDosyaYolu);
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                AphbKaydediliyor = false;
            }

            return result;
        }


        public bool AphbAySil(string eskiyol, string Yil, string Ay, bool OnaysizlarSilinsin, bool OnaylilarSilinsin)
        {
            Excel2.Application Excelim;
            Excel2.Workbook CalismaKitabi;
            Excel2.Worksheet CalismaSayfasi;

            bool result = true;

            if (File.Exists(eskiyol))
            {

                DataTable dt = Metodlar.AylikListeyiYukle(eskiyol);

                Excelim = new Excel2.Application();
                object SalakObje = System.Reflection.Missing.Value;

                var workbooks = Excelim.Workbooks;

                CalismaKitabi = workbooks.Open(eskiyol);
                CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                HafizadanAtilacaklar.AddRange(new List<object> { workbooks, CalismaKitabi, CalismaSayfasi });

                int baslangicsatiri = -1;

                int bitissatiri = -1;

                List<KeyValuePair<int, int>> silinecekdiziler = new List<KeyValuePair<int, int>>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string YilTemp = dt.Rows[i][0].ToString();

                    string AyTemp = dt.Rows[i][1].ToString().PadLeft(2, '0');

                    string OnayTemp = dt.Rows[i][23].ToString();

                    string AyPad = Ay.PadLeft(2, '0');

                    if (baslangicsatiri == -1)
                    {
                        if (OnaysizlarSilinsin && OnaylilarSilinsin)
                        {
                            if (Yil == YilTemp && AyPad == AyTemp)
                            {
                                baslangicsatiri = i;
                            }
                        }
                        else if (OnaylilarSilinsin)
                        {
                            if (Yil == YilTemp && AyPad == AyTemp && string.IsNullOrEmpty(OnayTemp))
                            {
                                baslangicsatiri = i;
                            }
                        }
                        else
                        {
                            if (Yil == YilTemp && AyPad == AyTemp && OnayTemp == "Onaylanmamış")
                            {
                                baslangicsatiri = i;
                            }
                        }
                    }
                    else
                    {
                        if (OnaysizlarSilinsin && OnaylilarSilinsin)
                        {
                            if (Yil == YilTemp && AyPad == AyTemp)
                            {
                                bitissatiri = i;
                            }
                            else
                            {
                                if (bitissatiri == -1)
                                {
                                    bitissatiri = baslangicsatiri;
                                }

                                silinecekdiziler.Add(new KeyValuePair<int, int>(baslangicsatiri + 2, bitissatiri + 2));

                                baslangicsatiri = -1;

                                bitissatiri = -1;
                            }
                        }
                        else if (OnaylilarSilinsin)
                        {
                            if (Yil == YilTemp && AyPad == AyTemp && string.IsNullOrEmpty(OnayTemp))
                            {
                                bitissatiri = i;
                            }
                            else
                            {
                                if (bitissatiri == -1)
                                {
                                    bitissatiri = baslangicsatiri;
                                }

                                silinecekdiziler.Add(new KeyValuePair<int, int>(baslangicsatiri + 2, bitissatiri + 2));

                                baslangicsatiri = -1;

                                bitissatiri = -1;
                            }
                        }
                        else
                        {
                            if (Yil == YilTemp && AyPad == AyTemp && OnayTemp == "Onaylanmamış")
                            {
                                bitissatiri = i;
                            }
                            else
                            {
                                if (bitissatiri == -1)
                                {
                                    bitissatiri = baslangicsatiri;
                                }

                                silinecekdiziler.Add(new KeyValuePair<int, int>(baslangicsatiri + 2, bitissatiri + 2));

                                baslangicsatiri = -1;

                                bitissatiri = -1;
                            }
                        }

                    }

                }

                if (baslangicsatiri > -1)
                {
                    if (bitissatiri == -1)
                    {
                        bitissatiri = baslangicsatiri;
                    }

                    silinecekdiziler.Add(new KeyValuePair<int, int>(baslangicsatiri + 2, bitissatiri + 2));
                }

                for (int i = silinecekdiziler.Count - 1; i >= 0; i--)
                {

                    var baslangicrow = CalismaSayfasi.Rows[silinecekdiziler[i].Key];
                    var bitisrow = CalismaSayfasi.Rows[silinecekdiziler[i].Value];

                    Excel2.Range rng = CalismaSayfasi.Range[baslangicrow, bitisrow];

                    HafizadanAtilacaklar.AddRange(new List<object> { baslangicrow, bitisrow, rng });

                    rng.Delete(Excel2.XlDeleteShiftDirection.xlShiftUp);
                }

                Excelim.Visible = false;

                Excelim.DisplayAlerts = false;

                int excelprocessid = Metodlar.GetExcelProcessId(Excelim);

                try
                {
                    CalismaKitabi.Save();
                }
                catch
                {
                    result = false;
                }

                CalismaKitabi.Close(false);

                HafizadanAtilacaklar.Reverse();

                int j = 0;

                while (j < HafizadanAtilacaklar.Count)
                {
                    try
                    {
                        var item = HafizadanAtilacaklar.ElementAt(j);

                        Marshal.FinalReleaseComObject(item);

                        item = null;

                    }
                    catch
                    {
                    }

                    j++;
                }


                Excelim.Quit();
                Marshal.FinalReleaseComObject(Excelim);

                Metodlar.KillProcessById(excelprocessid);
            }

            return result;

        }

        /// <summary>
        /// 6) SON ADIM; İNDİRİLEN BİLGİLER Başvuru excele kayıt edildiği metod.
        /// </summary>
        /// <param name="DisDataGrid"></param>
        /// <param name="filename"></param>
        /// <param name="KanunNo"></param>
        /// <returns></returns>
        public bool BasvuruOkuVeYaz(System.Data.DataTable DisDataGrid, string filename, string KanunNo)
        {
            while (BfKaydediliyor)
            {
                Thread.Sleep(500);
            }

            BfKaydediliyor = true;
            bool result = false;

            try
            {
                Excel2.Application Excelim;
                Excel2.Workbook CalismaKitabi;
                Excel2.Worksheet CalismaSayfasi;

                if (DisDataGrid.Rows.Count > 0 || KanunNo=="7252")
                {

                    Excelim = new Excel2.Application();
                    object SalakObje = System.Reflection.Missing.Value;

                    bool DosyaMevcutMu = File.Exists(filename);

                    var workbooks = Excelim.Workbooks;

                    CalismaKitabi = workbooks.Open(DosyaMevcutMu ? filename : Path.Combine(Application.StartupPath, "BasvuruTemplate.xlsx"));

                    int SayfaNo = 1;

                    if (KanunNo == "6111") SayfaNo = 1;
                    else if (KanunNo == "687") SayfaNo = 3;
                    else if (KanunNo == "7103") SayfaNo = 4;
                    else if (KanunNo == "2828") SayfaNo = 5;
                    else if (KanunNo == "14857") SayfaNo = 6;
                    else if (KanunNo == "7252") SayfaNo = 7;
                    else if (KanunNo == "17256") SayfaNo = 8;
                    else if (KanunNo == "27256") SayfaNo = 9;
                    else if (KanunNo == "7316") SayfaNo = 10;
                    else if (KanunNo == "3294") SayfaNo = 11;
                    else SayfaNo = 2;

                    var sheets = CalismaKitabi.Sheets;

                    HafizadanAtilacaklar.AddRange(new List<object> { workbooks, CalismaKitabi, sheets });

                    if (sheets.Count < 11)
                    {
                        var sayi = 11 - sheets.Count;

                        for (int i = 0; i < sayi; i++)
                        {
                            var sheet1 = sheets[1];

                            var lastsheet = sheets[sheets.Count];

                            sheet1.Copy(Type.Missing, lastsheet);

                            var cells = sheets[sheets.Count].Cells;

                            Excel2.Range last = cells.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Type.Missing);

                            HafizadanAtilacaklar.AddRange(new List<object> { lastsheet, sheet1, cells, last });

                            if (last.Row > 1)
                            {
                                var lastsheet2 = sheets[sheets.Count] as Excel2.Worksheet;

                                Excel2.Range range = lastsheet2.get_Range("A2", last);
                                range.Delete(Excel2.XlDeleteShiftDirection.xlShiftUp);

                                HafizadanAtilacaklar.AddRange(new List<object> { lastsheet2, range });
                            }


                            if (sheets.Count == 4) //7103 bf
                            {
                                var sheet7103 = sheets[4] as Excel2.Worksheet;
                                var cells7103 = sheet7103.Cells;
                                var cellTc7103 = cells7103[1, 2] as Excel2.Range;
                                var cellSicilNo7103 = cells7103[1, 3] as Excel2.Range;
                                var cellAd7103 = cells7103[1, 4] as Excel2.Range;
                                var cellSoyad7103 = cells7103[1, 5] as Excel2.Range;
                                var cellBaslangicDonemi7103 = cells7103[1, 6] as Excel2.Range;
                                var cellBitisDonemi7103 = cells7103[1, 7] as Excel2.Range;
                                var cellBaz7103 = cells7103[1, 8] as Excel2.Range;
                                var cellUcretDestegiTercihi7103 = cells7103[1, 9] as Excel2.Range;
                                var cellPrimveUcretDestegiIcinBaslangicDonemi7103 = cells7103[1, 10] as Excel2.Range;
                                var cellPrimveUcretDestegiIcinBitisDonemi7103 = cells7103[1, 11] as Excel2.Range;
                                var cellPrimveUcretDestegiIcinIlaveOlunacakSayi7103 = cells7103[1, 12] as Excel2.Range;
                                var cellKanun7103 = cells7103[1, 13] as Excel2.Range;
                                var cellIseGiris7103 = cells7103[1, 14] as Excel2.Range;
                                var cellIstenAyrilis7103 = cells7103[1, 15] as Excel2.Range;
                                var cellIlkTanimlama7103 = cells7103[1, 16] as Excel2.Range;
                                var cellAraci7103 = cells7103[1, 17] as Excel2.Range;
                                var cellBos7103 = cells7103[1, 18] as Excel2.Range;
                                var cellGirisBakildi7103 = cells7103[1, 19] as Excel2.Range;
                                var cellBos27103 = cells7103[1, 20] as Excel2.Range;
                                var cellBos37103 = cells7103[1, 21] as Excel2.Range;
                                cellTc7103.Value2 = "TC No";
                                cellSicilNo7103.Value2 = "SicilNo";
                                cellAd7103.Value2 = "Ad";
                                cellSoyad7103.Value2 = "Soyad";
                                cellBaslangicDonemi7103.Value2 = "Başlangıç Dönemi";
                                cellBitisDonemi7103.Value2 = "Bitiş Dönemi";
                                cellBaz7103.Value2 = "Ortalama Sigortalı Sayısı";
                                cellUcretDestegiTercihi7103.Value2 = "Ücret Desteği Tercihi";
                                cellPrimveUcretDestegiIcinBaslangicDonemi7103.Value2 = "Prim ve Ücret Desteği İçin Başlangıç Dönemi";
                                cellPrimveUcretDestegiIcinBitisDonemi7103.Value2 = "Prim ve Ücret Desteği İçin Bitiş Dönemi";
                                cellPrimveUcretDestegiIcinIlaveOlunacakSayi7103.Value2 = "Prim ve Ücret Desteği İçin İlave Olunacak Sayı";
                                cellKanun7103.Value2 = "Kanun No";
                                cellIseGiris7103.Value2 = "İşe Giriş Tarihi";
                                cellIstenAyrilis7103.Value2 = "İşten Ayrılış Tarihi";
                                cellIlkTanimlama7103.Value2 = "İlk Tanımlama Tarihi";
                                cellAraci7103.Value2 = "Aracı";
                                cellBos7103.Value2 = "";
                                cellGirisBakildi7103.Value2 = "giriş bakıldı";
                                cellBos27103.Value2 = "";
                                cellBos37103.Value2 = "";

                                sheet7103.Name = "7103";

                                HafizadanAtilacaklar.AddRange(new List<object> { sheet7103, cells7103, cellTc7103, cellSicilNo7103, cellAd7103, cellSoyad7103, cellBaslangicDonemi7103, cellBitisDonemi7103, cellBaz7103, cellUcretDestegiTercihi7103, cellPrimveUcretDestegiIcinBaslangicDonemi7103, cellPrimveUcretDestegiIcinBitisDonemi7103, cellPrimveUcretDestegiIcinIlaveOlunacakSayi7103, cellKanun7103, cellIseGiris7103, cellIstenAyrilis7103, cellIlkTanimlama7103, cellAraci7103, cellBos7103, cellGirisBakildi7103, cellBos27103, cellBos37103 });
                            }

                            if (sheets.Count == 5) //2828 bf
                            {
                                var sheet2828 = sheets[5] as Excel2.Worksheet;
                                var cells2828 = sheet2828.Cells;
                                var cellTc = cells2828[1, 2] as Excel2.Range;
                                var cellSicilNo = cells2828[1, 3] as Excel2.Range;
                                var cellAd = cells2828[1, 4] as Excel2.Range;
                                var cellSoyad = cells2828[1, 5] as Excel2.Range;
                                var cellBaslangicDonemi = cells2828[1, 6] as Excel2.Range;
                                var cellBitisDonemi = cells2828[1, 7] as Excel2.Range;
                                var cellTesvikSuresi = cells2828[1, 8] as Excel2.Range;
                                var cellIseGiris = cells2828[1, 9] as Excel2.Range;
                                var cellIstenAyrilis = cells2828[1, 10] as Excel2.Range;
                                var cellIlkTanimlama = cells2828[1, 11] as Excel2.Range;
                                var cellAraci = cells2828[1, 12] as Excel2.Range;
                                var cellBos = cells2828[1, 13] as Excel2.Range;
                                var cellGirisBakildi = cells2828[1, 14] as Excel2.Range;
                                var cellBos2 = cells2828[1, 15] as Excel2.Range;
                                var cellBos3 = cells2828[1, 16] as Excel2.Range;
                                cellTc.Value2 = "TC No";
                                cellSicilNo.Value2 = "SicilNo";
                                cellAd.Value2 = "Ad";
                                cellSoyad.Value2 = "Soyad";
                                cellBaslangicDonemi.Value2 = "Başlangıç Dönemi";
                                cellBitisDonemi.Value2 = "Bitiş Dönemi";
                                cellTesvikSuresi.Value2 = "Teşvik Süresi";
                                cellIseGiris.Value2 = "İşe Giriş Tarihi";
                                cellIstenAyrilis.Value2 = "İşten Ayrılış Tarihi";
                                cellIlkTanimlama.Value2 = "İlk Tanımlama Tarihi";
                                cellAraci.Value2 = "Aracı";
                                cellBos.Value2 = "";
                                cellGirisBakildi.Value2 = "giriş bakıldı";
                                cellBos2.Value2 = "";
                                cellBos3.Value2 = "";

                                sheet2828.Name = "2828";

                                var cellSon = cells2828[100, 15] as Excel2.Range;

                                cellSon.Value2 = " ";

                                HafizadanAtilacaklar.AddRange(new List<object> { sheet2828, cells2828, cellTc, cellSicilNo, cellAd, cellSoyad, cellBaslangicDonemi, cellBitisDonemi, cellTesvikSuresi, cellIseGiris, cellIstenAyrilis, cellIlkTanimlama, cellAraci, cellBos, cellGirisBakildi, cellBos2, cellBos3, cellSon });

                            }

                            if (sheets.Count == 6) //14587 bf
                            {
                                var sheet14857 = sheets[6] as Excel2.Worksheet;
                                var cells14857 = sheet14857.Cells;
                                var cellTc = cells14857[1, 2] as Excel2.Range;
                                var cellSicilNo = cells14857[1, 3] as Excel2.Range;
                                var cellAd = cells14857[1, 4] as Excel2.Range;
                                var cellSoyad = cells14857[1, 5] as Excel2.Range;
                                var cellBaslangicDonemi = cells14857[1, 6] as Excel2.Range;
                                var cellBitisDonemi = cells14857[1, 7] as Excel2.Range;
                                var cellRaporNo = cells14857[1, 8] as Excel2.Range;
                                var cellOzurOrani = cells14857[1, 9] as Excel2.Range;
                                var cellDurum = cells14857[1, 10] as Excel2.Range;
                                var cellAraci = cells14857[1, 11] as Excel2.Range;
                                var cellBos = cells14857[1, 12] as Excel2.Range;
                                var cellGirisBakildi = cells14857[1, 13] as Excel2.Range;
                                var cellBos2 = cells14857[1, 14] as Excel2.Range;
                                var cellBos3 = cells14857[1, 15] as Excel2.Range;
                                cellTc.Value2 = "TC No";
                                cellSicilNo.Value2 = "SicilNo";
                                cellAd.Value2 = "Ad";
                                cellSoyad.Value2 = "Soyad";
                                cellBaslangicDonemi.Value2 = "Başlangıç Dönemi";
                                cellBitisDonemi.Value2 = "Bitiş Dönemi";
                                cellRaporNo.Value2 = "Rapor No";
                                cellOzurOrani.Value2 = "Özür Oranı";
                                cellDurum.Value2 = "Durum";
                                cellAraci.Value2 = "Aracı";
                                cellBos.Value2 = "";
                                cellGirisBakildi.Value2 = "giriş bakıldı";
                                cellBos2.Value2 = "";
                                cellBos3.Value2 = "";

                                sheet14857.Name = "14857";

                                var cellSon = cells14857[100, 15] as Excel2.Range;

                                cellSon.Value2 = " ";

                                HafizadanAtilacaklar.AddRange(new List<object> { sheet14857, cells14857, cellTc, cellSicilNo, cellAd, cellSoyad, cellBaslangicDonemi, cellBitisDonemi, cellRaporNo, cellOzurOrani, cellDurum, cellAraci, cellBos, cellGirisBakildi, cellBos2, cellBos3, cellSon });

                            }

                            if (sheets.Count == 7) //7252 bf
                            {
                                var sheet7252 = sheets[7] as Excel2.Worksheet;
                                var cells7252 = sheet7252.Cells;
                                var cellTc = cells7252[1, 2] as Excel2.Range;
                                var cellSicilNo = cells7252[1, 3] as Excel2.Range;
                                var cellAd = cells7252[1, 4] as Excel2.Range;
                                var cellSoyad = cells7252[1, 5] as Excel2.Range;
                                var cellBaslangicDonemi = cells7252[1, 6] as Excel2.Range;
                                var cellBitisDonemi = cells7252[1, 7] as Excel2.Range;
                                var cellKCONUDSonlanmaTarihi = cells7252[1, 8] as Excel2.Range;
                                var cellOrtalamaGunSayisi = cells7252[1, 9] as Excel2.Range;
                                var cellKanunNo = cells7252[1, 10] as Excel2.Range;
                                var cellGiris = cells7252[1, 11] as Excel2.Range;
                                var cellCikis = cells7252[1, 12] as Excel2.Range;
                                var cellIlkTanimlamaTarihi = cells7252[1, 13] as Excel2.Range;
                                var cellAraci = cells7252[1, 14] as Excel2.Range;
                                var cellBos = cells7252[1, 15] as Excel2.Range;
                                var cellGirisBakildi = cells7252[1, 16] as Excel2.Range;
                                var cellBos2 = cells7252[1, 17] as Excel2.Range;
                                var cellBos3 = cells7252[1, 18] as Excel2.Range;
                                cellTc.Value2 = "TC No";
                                cellSicilNo.Value2 = "SicilNo";
                                cellAd.Value2 = "Ad";
                                cellSoyad.Value2 = "Soyad";
                                cellBaslangicDonemi.Value2 = "Başlangıç Dönemi";
                                cellBitisDonemi.Value2 = "Bitiş Dönemi";
                                cellKCONUDSonlanmaTarihi.Value2 = "KÇÖ/NÜD Sonlanma Tarihi";
                                cellOrtalamaGunSayisi.Value2 = "Ortalama Gün Sayısı";
                                cellKanunNo.Value2 = "Kanun";
                                cellGiris.Value2 = "İşe Giriş Tarihi";
                                cellCikis.Value2 = "İşten Ayrılış Tarihi";
                                cellIlkTanimlamaTarihi.Value2 = "İlk Tanımlama Tarihi";
                                cellAraci.Value2 = "Aracı";
                                cellBos.Value2 = "";
                                cellGirisBakildi.Value2 = "giriş bakıldı";
                                cellBos2.Value2 = "";
                                cellBos3.Value2 = "";

                                sheet7252.Name = "7252";

                                var cellSon = cells7252[100, 18] as Excel2.Range;

                                cellSon.Value2 = " ";

                                HafizadanAtilacaklar.AddRange(new List<object> { sheet7252, cells7252, cellTc, cellSicilNo, cellAd, cellSoyad, cellBaslangicDonemi, cellBitisDonemi, cellKCONUDSonlanmaTarihi, cellOrtalamaGunSayisi, cellKanunNo, cellGiris, cellCikis, cellIlkTanimlamaTarihi, cellAraci, cellBos, cellGirisBakildi, cellBos2, cellBos3, cellSon });

                            }

                            if (sheets.Count == 8) //17256 bf
                            {
                                var sheet17256 = sheets[8] as Excel2.Worksheet;
                                var cells17256 = sheet17256.Cells;
                                var cellTc = cells17256[1, 2] as Excel2.Range;
                                var cellSicilNo = cells17256[1, 3] as Excel2.Range;
                                var cellAd = cells17256[1, 4] as Excel2.Range;
                                var cellSoyad = cells17256[1, 5] as Excel2.Range;
                                var cellBaslangicDonemi = cells17256[1, 6] as Excel2.Range;
                                var cellBitisDonemi = cells17256[1, 7] as Excel2.Range;
                                var cellKanunNo = cells17256[1, 8] as Excel2.Range;
                                var cellGiris = cells17256[1, 9] as Excel2.Range;
                                var cellCikis = cells17256[1, 10] as Excel2.Range;
                                var cellSigortalininIsyerineBasvuruTarihi = cells17256[1, 11] as Excel2.Range;
                                var cellSigortaliIcinTercihDurumu = cells17256[1, 12] as Excel2.Range;
                                var cellIlkTanimlamaTarihi = cells17256[1, 13] as Excel2.Range;
                                var cellVerilsin7256 = cells17256[1, 14] as Excel2.Range;
                                var cellAraci = cells17256[1, 15] as Excel2.Range;
                                var cellBos = cells17256[1, 16] as Excel2.Range;
                                var cellGirisBakildi = cells17256[1, 17] as Excel2.Range;
                                var cellBos2 = cells17256[1, 18] as Excel2.Range;
                                var cellBos3 = cells17256[1, 19] as Excel2.Range;
                                cellTc.Value2 = "TC No";
                                cellSicilNo.Value2 = "SicilNo";
                                cellAd.Value2 = "Ad";
                                cellSoyad.Value2 = "Soyad";
                                cellBaslangicDonemi.Value2 = "Başlangıç Dönemi";
                                cellBitisDonemi.Value2 = "Bitiş Dönemi";
                                cellKanunNo.Value2 = "Kanun";
                                cellSigortalininIsyerineBasvuruTarihi.Value2 = "Sigortalının İşyerine Başvuru Tarihi";
                                cellSigortaliIcinTercihDurumu.Value2 = "Sigortalı İçin Tercih Durumu";
                                cellGiris.Value2 = "İşe Giriş Tarihi";
                                cellCikis.Value2 = "İşten Ayrılış Tarihi";
                                cellIlkTanimlamaTarihi.Value2 = "İlk Tanımlama Tarihi";
                                cellVerilsin7256.Value2 = "Teşvik Verilsin";
                                cellAraci.Value2 = "Aracı";
                                cellBos.Value2 = "";
                                cellGirisBakildi.Value2 = "giriş bakıldı";
                                cellBos2.Value2 = "";
                                cellBos3.Value2 = "";

                                sheet17256.Name = "17256";

                                var cellSon = cells17256[100, 18] as Excel2.Range;

                                cellSon.Value2 = " ";

                                HafizadanAtilacaklar.AddRange(new List<object> { sheet17256, cells17256, cellTc, cellSicilNo, cellAd, cellSoyad, cellBaslangicDonemi, cellBitisDonemi, cellKanunNo, cellGiris, cellCikis, cellSigortalininIsyerineBasvuruTarihi,cellSigortaliIcinTercihDurumu, cellIlkTanimlamaTarihi,cellVerilsin7256, cellAraci, cellBos, cellGirisBakildi, cellBos2, cellBos3, cellSon });

                            }

                            if (sheets.Count == 9) //27256 bf
                            {
                                var sheet27256 = sheets[9] as Excel2.Worksheet;
                                var cells27256 = sheet27256.Cells;
                                var cellTc = cells27256[1, 2] as Excel2.Range;
                                var cellSicilNo = cells27256[1, 3] as Excel2.Range;
                                var cellAd = cells27256[1, 4] as Excel2.Range;
                                var cellSoyad = cells27256[1, 5] as Excel2.Range;
                                var cellBaslangicDonemi = cells27256[1, 6] as Excel2.Range;
                                var cellBitisDonemi = cells27256[1, 7] as Excel2.Range;
                                var cellIlaveOlunmasiGerekenSayi = cells27256[1, 8] as Excel2.Range;
                                var cellKanunNo = cells27256[1, 9] as Excel2.Range;
                                var cellGiris = cells27256[1, 10] as Excel2.Range;
                                var cellCikis = cells27256[1, 11] as Excel2.Range;
                                var cellIlkTanimlamaTarihi = cells27256[1, 12] as Excel2.Range;
                                var cellVerilsin7256 = cells27256[1, 13] as Excel2.Range;
                                var cellAraci = cells27256[1, 14] as Excel2.Range;
                                var cellBos = cells27256[1, 15] as Excel2.Range;
                                var cellGirisBakildi = cells27256[1, 17] as Excel2.Range;
                                var cellBos2 = cells27256[1, 18] as Excel2.Range;
                                var cellBos3 = cells27256[1, 19] as Excel2.Range;
                                cellTc.Value2 = "TC No";
                                cellSicilNo.Value2 = "SicilNo";
                                cellAd.Value2 = "Ad";
                                cellSoyad.Value2 = "Soyad";
                                cellBaslangicDonemi.Value2 = "Başlangıç Dönemi";
                                cellBitisDonemi.Value2 = "Bitiş Dönemi";
                                cellIlaveOlunmasiGerekenSayi.Value2 = "İlave Olunması Gereken Sayı";
                                cellKanunNo.Value2 = "Kanun";
                                cellGiris.Value2 = "İşe Giriş Tarihi";
                                cellCikis.Value2 = "İşten Ayrılış Tarihi";
                                cellIlkTanimlamaTarihi.Value2 = "İlk Tanımlama Tarihi";
                                cellVerilsin7256.Value2 = "Teşvik Verilsin";
                                cellAraci.Value2 = "Aracı";
                                cellBos.Value2 = "";
                                cellGirisBakildi.Value2 = "giriş bakıldı";
                                cellBos2.Value2 = "";
                                cellBos3.Value2 = "";

                                sheet27256.Name = "27256";

                                var cellSon = cells27256[100, 18] as Excel2.Range;

                                cellSon.Value2 = " ";

                                HafizadanAtilacaklar.AddRange(new List<object> { sheet27256, cells27256, cellTc, cellSicilNo, cellAd, cellSoyad, cellBaslangicDonemi, cellBitisDonemi, cellIlaveOlunmasiGerekenSayi, cellKanunNo, cellGiris, cellCikis, cellIlkTanimlamaTarihi, cellVerilsin7256, cellAraci, cellBos, cellGirisBakildi, cellBos2, cellBos3, cellSon });

                            }

                            if (sheets.Count == 10) //7316 bf
                            {
                                var sheet7316 = sheets[10] as Excel2.Worksheet;
                                var cells7316 = sheet7316.Cells;
                                var cellTc = cells7316[1, 2] as Excel2.Range;
                                var cellSicilNo = cells7316[1, 3] as Excel2.Range;
                                var cellAd = cells7316[1, 4] as Excel2.Range;
                                var cellSoyad = cells7316[1, 5] as Excel2.Range;
                                var cellBaslangicDonemi = cells7316[1, 6] as Excel2.Range;
                                var cellBitisDonemi = cells7316[1, 7] as Excel2.Range;
                                var cellKanunNo = cells7316[1, 8] as Excel2.Range;
                                var cellGiris = cells7316[1, 9] as Excel2.Range;
                                var cellCikis = cells7316[1, 10] as Excel2.Range;
                                var cellIlkTanimlamaTarihi = cells7316[1, 11] as Excel2.Range;
                                var cellAraci = cells7316[1, 12] as Excel2.Range;
                                var cellBos = cells7316[1, 13] as Excel2.Range;
                                var cellGirisBakildi = cells7316[1, 15] as Excel2.Range;
                                var cellBos2 = cells7316[1, 16] as Excel2.Range;
                                var cellBos3 = cells7316[1, 17] as Excel2.Range;
                                cellTc.Value2 = "TC No";
                                cellSicilNo.Value2 = "SicilNo";
                                cellAd.Value2 = "Ad";
                                cellSoyad.Value2 = "Soyad";
                                cellBaslangicDonemi.Value2 = "Başlangıç Dönemi";
                                cellBitisDonemi.Value2 = "Bitiş Dönemi";
                                cellKanunNo.Value2 = "Kanun";
                                cellGiris.Value2 = "İşe Giriş Tarihi";
                                cellCikis.Value2 = "İşten Ayrılış Tarihi";
                                cellIlkTanimlamaTarihi.Value2 = "İlk Tanımlama Tarihi";
                                cellAraci.Value2 = "Aracı";
                                cellBos.Value2 = "";
                                cellGirisBakildi.Value2 = "giriş bakıldı";
                                cellBos2.Value2 = "";
                                cellBos3.Value2 = "";

                                sheet7316.Name = "7316";

                                var cellSon = cells7316[100, 18] as Excel2.Range;

                                cellSon.Value2 = " ";

                                HafizadanAtilacaklar.AddRange(new List<object> { sheet7316, cells7316, cellTc, cellSicilNo, cellAd, cellSoyad, cellBaslangicDonemi, cellBitisDonemi, cellKanunNo, cellGiris, cellCikis, cellIlkTanimlamaTarihi, cellAraci, cellBos, cellGirisBakildi, cellBos2, cellBos3, cellSon });

                            }

                            if (sheets.Count == 11) //3294 bf
                            {
                                var sheet3294 = sheets[11] as Excel2.Worksheet;
                                var cells3294 = sheet3294.Cells;
                                var cellTc = cells3294[1, 2] as Excel2.Range;
                                var cellSicilNo = cells3294[1, 3] as Excel2.Range;
                                var cellAd = cells3294[1, 4] as Excel2.Range;
                                var cellSoyad = cells3294[1, 5] as Excel2.Range;
                                var cellBaslangicDonemi = cells3294[1, 6] as Excel2.Range;
                                var cellBitisDonemi = cells3294[1, 7] as Excel2.Range;
                                var cellOrtalamaSigortaliSayisi = cells3294[1, 8] as Excel2.Range;
                                var cellGiris = cells3294[1, 9] as Excel2.Range;
                                var cellCikis = cells3294[1, 10] as Excel2.Range;
                                var cellIlkTanimlamaTarihi = cells3294[1, 11] as Excel2.Range;
                                var cellAraci = cells3294[1, 12] as Excel2.Range;
                                var cellBos = cells3294[1, 13] as Excel2.Range;
                                var cellGirisBakildi = cells3294[1, 15] as Excel2.Range;
                                var cellBos2 = cells3294[1, 16] as Excel2.Range;
                                var cellBos3 = cells3294[1, 17] as Excel2.Range;
                                cellTc.Value2 = "TC No";
                                cellSicilNo.Value2 = "SicilNo";
                                cellAd.Value2 = "Ad";
                                cellSoyad.Value2 = "Soyad";
                                cellBaslangicDonemi.Value2 = "Başlangıç Dönemi";
                                cellBitisDonemi.Value2 = "Bitiş Dönemi";
                                cellOrtalamaSigortaliSayisi.Value2 = "Ortalama Sigortalı Sayısı";
                                cellGiris.Value2 = "İşe Giriş Tarihi";
                                cellCikis.Value2 = "İşten Ayrılış Tarihi";
                                cellIlkTanimlamaTarihi.Value2 = "İlk Tanımlama Tarihi";
                                cellAraci.Value2 = "Aracı";
                                cellBos.Value2 = "";
                                cellGirisBakildi.Value2 = "giriş bakıldı";
                                cellBos2.Value2 = "";
                                cellBos3.Value2 = "";

                                sheet3294.Name = "3294";

                                var cellSon = cells3294[100, 18] as Excel2.Range;

                                cellSon.Value2 = " ";

                                HafizadanAtilacaklar.AddRange(new List<object> { sheet3294, cells3294, cellTc, cellSicilNo, cellAd, cellSoyad, cellBaslangicDonemi, cellBitisDonemi, cellOrtalamaSigortaliSayisi, cellGiris, cellCikis, cellIlkTanimlamaTarihi, cellAraci, cellBos, cellGirisBakildi, cellBos2, cellBos3, cellSon });

                            }

                        }

                        //}

                    }

                    if (KanunNo == "6111")
                    {
                        var sheet6111 = sheets[1] as Excel2.Worksheet;
                        var cells6111 = sheet6111.Cells;
                        var cellTc = cells6111[1, 2] as Excel2.Range;
                        var cellSicilNo = cells6111[1, 3] as Excel2.Range;
                        var cellAd = cells6111[1, 4] as Excel2.Range;
                        var cellSoyad = cells6111[1, 5] as Excel2.Range;
                        var cellBaslangicDonemi = cells6111[1, 6] as Excel2.Range;
                        var cellBitisDonemi = cells6111[1, 7] as Excel2.Range;
                        var cellBaz = cells6111[1, 8] as Excel2.Range;
                        var cellIseGiris = cells6111[1, 9] as Excel2.Range;
                        var cellIstenAyrilis = cells6111[1, 10] as Excel2.Range;
                        var cellIlkTanimlama = cells6111[1, 11] as Excel2.Range;
                        var cellOnayDurumu = cells6111[1, 12] as Excel2.Range;
                        var cellAraci = cells6111[1, 13] as Excel2.Range;
                        var cellBos = cells6111[1, 14] as Excel2.Range;
                        var cellGirisBakildi = cells6111[1, 15] as Excel2.Range;
                        var cellBos2 = cells6111[1, 16] as Excel2.Range;
                        var cellBos3 = cells6111[1, 17] as Excel2.Range;

                        cellTc.Value2 = "T.C. Kimlik No";
                        cellSicilNo.Value2 = "Sicil Numarası";
                        cellAd.Value2 = "Ad";
                        cellSoyad.Value2 = "Soyad";
                        cellBaslangicDonemi.Value2 = "Başlangıç Dönemi";
                        cellBitisDonemi.Value2 = "Bitiş Dönemi";
                        cellBaz.Value2 = "Ortalama Sigortalı Sayısı";
                        cellIseGiris.Value2 = "İşe Giriş Tarihi";
                        cellIstenAyrilis.Value2 = "İşten Ayrılış Tarihi";
                        cellIlkTanimlama.Value2 = "İlk Tanımlama Tarihi";
                        cellOnayDurumu.Value2 = "Onay Durumu";
                        cellAraci.Value2 = "Aracı";
                        cellBos.Value2 = "";
                        cellGirisBakildi.Value2 = "giriş bakıldı";
                        cellBos2.Value2 = "";
                        cellBos3.Value2 = "";

                        HafizadanAtilacaklar.AddRange(new List<object> { sheet6111, cells6111, cellTc, cellSicilNo, cellAd, cellSoyad, cellBaslangicDonemi, cellBitisDonemi, cellBaz, cellIseGiris, cellIstenAyrilis, cellIlkTanimlama, cellOnayDurumu, cellAraci, cellBos, cellGirisBakildi, cellBos2, cellBos3 });

                    }

                    if (KanunNo == "7103")
                    {
                        var sheet7103 = sheets[4] as Excel2.Worksheet;
                        var cells7103 = sheet7103.Cells;
                        var cellTc7103 = cells7103[1, 2] as Excel2.Range;
                        var cellSicilNo7103 = cells7103[1, 3] as Excel2.Range;
                        var cellAd7103 = cells7103[1, 4] as Excel2.Range;
                        var cellSoyad7103 = cells7103[1, 5] as Excel2.Range;
                        var cellBaslangicDonemi7103 = cells7103[1, 6] as Excel2.Range;
                        var cellBitisDonemi7103 = cells7103[1, 7] as Excel2.Range;
                        var cellBaz7103 = cells7103[1, 8] as Excel2.Range;
                        var cellUcretDestegiTercihi7103 = cells7103[1, 9] as Excel2.Range;
                        var cellPrimveUcretDestegiIcinBaslangicDonemi7103 = cells7103[1, 10] as Excel2.Range;
                        var cellPrimveUcretDestegiIcinBitisDonemi7103 = cells7103[1, 11] as Excel2.Range;
                        var cellPrimveUcretDestegiIcinIlaveOlunacakSayi7103 = cells7103[1, 12] as Excel2.Range;
                        var cellKanun7103 = cells7103[1, 13] as Excel2.Range;
                        var cellIseGiris7103 = cells7103[1, 14] as Excel2.Range;
                        var cellIstenAyrilis7103 = cells7103[1, 15] as Excel2.Range;
                        var cellIlkTanimlama7103 = cells7103[1, 16] as Excel2.Range;
                        var cellAraci7103 = cells7103[1, 17] as Excel2.Range;
                        var cellBos7103 = cells7103[1, 18] as Excel2.Range;
                        var cellGirisBakildi7103 = cells7103[1, 19] as Excel2.Range;
                        var cellBos27103 = cells7103[1, 20] as Excel2.Range;
                        var cellBos37103 = cells7103[1, 21] as Excel2.Range;
                        cellTc7103.Value2 = "TC No";
                        cellSicilNo7103.Value2 = "SicilNo";
                        cellAd7103.Value2 = "Ad";
                        cellSoyad7103.Value2 = "Soyad";
                        cellBaslangicDonemi7103.Value2 = "Başlangıç Dönemi";
                        cellBitisDonemi7103.Value2 = "Bitiş Dönemi";
                        cellBaz7103.Value2 = "Ortalama Sigortalı Sayısı";
                        cellUcretDestegiTercihi7103.Value2 = "Ücret Desteği Tercihi";
                        cellPrimveUcretDestegiIcinBaslangicDonemi7103.Value2 = "Prim ve Ücret Desteği İçin Başlangıç Dönemi";
                        cellPrimveUcretDestegiIcinBitisDonemi7103.Value2 = "Prim ve Ücret Desteği İçin Bitiş Dönemi";
                        cellPrimveUcretDestegiIcinIlaveOlunacakSayi7103.Value2 = "Prim ve Ücret Desteği İçin İlave Olunacak Sayı";
                        cellKanun7103.Value2 = "Kanun No";
                        cellIseGiris7103.Value2 = "İşe Giriş Tarihi";
                        cellIstenAyrilis7103.Value2 = "İşten Ayrılış Tarihi";
                        cellIlkTanimlama7103.Value2 = "İlk Tanımlama Tarihi";
                        cellAraci7103.Value2 = "Aracı";
                        cellBos7103.Value2 = "";
                        cellGirisBakildi7103.Value2 = "giriş bakıldı";
                        cellBos27103.Value2 = "";
                        cellBos37103.Value2 = "";

                        sheet7103.Name = "7103";

                        HafizadanAtilacaklar.AddRange(new List<object> { sheet7103, cells7103, cellTc7103, cellSicilNo7103, cellAd7103, cellSoyad7103, cellBaslangicDonemi7103, cellBitisDonemi7103, cellBaz7103, cellUcretDestegiTercihi7103, cellPrimveUcretDestegiIcinBaslangicDonemi7103, cellPrimveUcretDestegiIcinBitisDonemi7103, cellPrimveUcretDestegiIcinIlaveOlunacakSayi7103, cellKanun7103, cellIseGiris7103, cellIstenAyrilis7103, cellIlkTanimlama7103, cellAraci7103, cellBos7103, cellGirisBakildi7103, cellBos27103, cellBos37103 });

                    }

                    CalismaSayfasi = (Excel2.Worksheet)sheets[SayfaNo];
                    CalismaSayfasi.Activate();

                    HafizadanAtilacaklar.Add(CalismaSayfasi);


                    //if (DosyaMevcutMu)
                    //{
                    var usedrange = CalismaSayfasi.UsedRange;

                    var allcells = CalismaSayfasi.Cells;

                    Excel2.Range lastrow = usedrange.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                    Excel2.Range lastrow2 = allcells[lastrow.Row + 1, lastrow.Column];

                    HafizadanAtilacaklar.AddRange(new List<object> { usedrange, allcells, lastrow, lastrow2 });

                    if (lastrow.Row > 1)
                    {

                        Excel2.Range range = CalismaSayfasi.get_Range("A2", lastrow2);

                        var entirerow = range.EntireRow;

                        entirerow.Delete(Excel2.XlDeleteShiftDirection.xlShiftUp);

                        HafizadanAtilacaklar.AddRange(new List<object> { range, entirerow });
                    }

                    Excel2.Application ExcelimV2 = null;
                    Excel2.Workbook CalismaKitabiV2 = null;

                    string geciciDosyaYolu = null;

                    bool _10_Kere_Kaydedilemedi = false;

                    if (DisDataGrid.Rows.Count > 0)
                    {

                        ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook();

                        wb.Worksheets.Add(DisDataGrid);

                        geciciDosyaYolu = Path.Combine(Application.StartupPath, "temp", Path.GetFileName( filename.Insert(filename.IndexOf(".xlsx"), "_"+ Guid.NewGuid().ToString())));

                        wb.SaveAs(geciciDosyaYolu);


                        //NewExportExcelV2 excelV2 = new NewExportExcelV2();
                        //excelV2.gridControl1.DataSource = DisDataGrid;

                        //string geciciDosyaYolu = filename.Insert(filename.IndexOf(".xlsx"), "_GEÇİCİ");
                        //excelV2.gridView1.ExportToXlsx(geciciDosyaYolu);


                        ExcelimV2 = new Excel2.Application();
                        var workbooks2 = ExcelimV2.Workbooks;
                        CalismaKitabiV2 = workbooks2.Open(geciciDosyaYolu);

                        HafizadanAtilacaklar.AddRange(new List<object> { workbooks2, CalismaKitabiV2 });

                        int hataSayaci = 0;

                        bool kaydetmeBasarili = false;

                        while (hataSayaci < 10)
                        {
                            try
                            {
                                object SalakObjeV2 = System.Reflection.Missing.Value;
                                var CalismaSayfasiV2 = (Excel2.Worksheet)CalismaKitabiV2.ActiveSheet;

                                var usedrange4 = CalismaSayfasi.UsedRange;

                                Excel2.Range lastcell = usedrange4.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                                var usedrange2 = CalismaSayfasiV2.UsedRange;

                                Excel2.Range lastV2 = usedrange2.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                                var tumAlanV2 = CalismaSayfasiV2.Range["A2", lastV2];

                                tumAlanV2.Copy(Type.Missing);

                                var tumAlan = CalismaSayfasi.Range["A:X"];

                                tumAlan.NumberFormat = "@";

                                Excel2.Range rangeV2 = allcells[2, 2] as Excel2.Range;
                                rangeV2.Select();
                                CalismaSayfasi.Paste(Type.Missing, Type.Missing);

                                var usedrange3 = CalismaSayfasi.UsedRange;

                                Excel2.Range lastcell2 = usedrange3.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                                HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasiV2, usedrange4, lastcell, usedrange2, lastV2, tumAlanV2, tumAlan, rangeV2, usedrange3, lastcell2 });

                                if (lastcell.Row + lastV2.Row - 1 > lastcell2.Row)
                                {
                                    CalismaKitabi.Close(false);

                                    CalismaKitabi = workbooks.Open(DosyaMevcutMu ? filename : Path.Combine(Application.StartupPath, "BasvuruTemplate.xlsx"));
                                    CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                                    HafizadanAtilacaklar.AddRange(new List<object> { CalismaKitabi, CalismaSayfasi });

                                    continue;
                                }


                                //CalismaSayfasi.Range["A:K"].NumberFormat = "@";
                                //CalismaSayfasi.Range["N:X"].NumberFormat = "@";

                                var tumAlan3 = CalismaSayfasi.Range["A:X"];

                                var font = tumAlan3.Font;

                                font.Size = 12;
                                font.Name = "Times New Roman";

                                HafizadanAtilacaklar.AddRange(new List<object> { tumAlan3, font });

                                kaydetmeBasarili = true;

                                break;

                            }
                            catch (Exception ex)
                            {
                                hataSayaci++;

                                Thread.Sleep(1000);

                                frmIsyerleri.LogYaz("HATA OLUŞTU:" + ex.Message + Environment.NewLine);
                            }
                        }

                        if (!kaydetmeBasarili) _10_Kere_Kaydedilemedi = true;


                        if (KanunNo == "6111")
                        {
                            var sheets2 = CalismaKitabi.Sheets;

                            var sheet6111 = sheets2[1];

                            var columns = sheet6111.Columns;

                            var columns6111 = columns("A:M");

                            columns6111.AutoFit();

                            var cells6111 = sheet6111.Cells;

                            var cellAraci = cells6111[1, Sabitler.BasvuruFormlariSutunlari["6111-v2"][Enums.BasvuruFormuSutunTurleri.Araci] + 2] as Excel2.Range;

                            cellAraci.Value2 = "Aracı";

                            var cellBos = cells6111[1, Sabitler.BasvuruFormlariSutunlari["6111-v2"][Enums.BasvuruFormuSutunTurleri.Araci] + 2 + 1] as Excel2.Range;

                            cellBos.Value2 = "";

                            var cellGirisBakildi = cells6111[1, Sabitler.BasvuruFormlariSutunlari["6111-v2"][Enums.BasvuruFormuSutunTurleri.Araci] + 2 + 2] as Excel2.Range;

                            cellGirisBakildi.Value2 = "giriş bakıldı";

                            HafizadanAtilacaklar.AddRange(new List<object> { sheets2, sheet6111, columns, columns6111, cells6111, cellAraci, cellBos, cellGirisBakildi });


                        }
                        else if (KanunNo == "7103")
                        {
                            var sheets2 = CalismaKitabi.Sheets;

                            var sheet7103 = sheets2[4];

                            var columns = sheet7103.Columns;

                            var columns7103 = columns("A:Z");

                            columns7103.AutoFit();

                            var cellAraci = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2] as Excel2.Range;
                            cellAraci.Value2 = "Aracı";

                            var cellBos = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2 + 1] as Excel2.Range;
                            cellBos.Value2 = "";

                            var cellGirisBakildi = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2 + 2] as Excel2.Range;
                            cellGirisBakildi.Value2 = "giriş bakıldı";

                            HafizadanAtilacaklar.AddRange(new List<object> { sheets2, sheet7103, columns, columns7103, cellAraci, cellBos, cellGirisBakildi });

                        }
                        else if (KanunNo == "7252")
                        {
                            var sheets2 = CalismaKitabi.Sheets;

                            var sheet7252 = sheets2[7];

                            var columns = sheet7252.Columns;

                            var columns7252 = columns("A:P");

                            columns7252.AutoFit();

                            var cellAraci = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2] as Excel2.Range;
                            cellAraci.Value2 = "Aracı";

                            var cellBos = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2 + 1] as Excel2.Range;
                            cellBos.Value2 = "";

                            var cellGirisBakildi = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2 + 2] as Excel2.Range;
                            cellGirisBakildi.Value2 = "giriş bakıldı";

                            HafizadanAtilacaklar.AddRange(new List<object> { sheets2, sheet7252, columns, columns7252, cellAraci, cellBos, cellGirisBakildi });

                        }
                        else if (KanunNo == "17256")
                        {
                            var sheets2 = CalismaKitabi.Sheets;

                            var sheet17256 = sheets2[8];

                            var columns = sheet17256.Columns;

                            var columns17256 = columns("A:P");

                            columns17256.AutoFit();

                            var cellAraci = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2] as Excel2.Range;
                            cellAraci.Value2 = "Aracı";

                            var cellBos = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2 + 1] as Excel2.Range;
                            cellBos.Value2 = "";

                            var cellGirisBakildi = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2 + 2] as Excel2.Range;
                            cellGirisBakildi.Value2 = "giriş bakıldı";

                            HafizadanAtilacaklar.AddRange(new List<object> { sheets2, sheet17256, columns, columns17256, cellAraci, cellBos, cellGirisBakildi });

                        }
                        else if (KanunNo == "27256")
                        {
                            var sheets2 = CalismaKitabi.Sheets;

                            var sheet27256 = sheets2[9];

                            var columns = sheet27256.Columns;

                            var columns27256 = columns("A:P");

                            columns27256.AutoFit();

                            var cellAraci = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2] as Excel2.Range;
                            cellAraci.Value2 = "Aracı";

                            var cellBos = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2 + 1] as Excel2.Range;
                            cellBos.Value2 = "";

                            var cellGirisBakildi = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2 + 2] as Excel2.Range;
                            cellGirisBakildi.Value2 = "giriş bakıldı";

                            HafizadanAtilacaklar.AddRange(new List<object> { sheets2, sheet27256, columns, columns27256, cellAraci, cellBos, cellGirisBakildi });

                        }
                        else if (KanunNo == "7316")
                        {
                            var sheets2 = CalismaKitabi.Sheets;

                            var sheet7316 = sheets2[9];

                            var columns = sheet7316.Columns;

                            var columns7316 = columns("A:P");

                            columns7316.AutoFit();

                            var cellAraci = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2] as Excel2.Range;
                            cellAraci.Value2 = "Aracı";

                            var cellBos = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2 + 1] as Excel2.Range;
                            cellBos.Value2 = "";

                            var cellGirisBakildi = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2 + 2] as Excel2.Range;
                            cellGirisBakildi.Value2 = "giriş bakıldı";

                            HafizadanAtilacaklar.AddRange(new List<object> { sheets2, sheet7316, columns, columns7316, cellAraci, cellBos, cellGirisBakildi });

                        }
                        else if (KanunNo == "3294")
                        {
                            var sheets2 = CalismaKitabi.Sheets;

                            var sheet3294 = sheets2[10];

                            var columns = sheet3294.Columns;

                            var columns3294 = columns("A:P");

                            columns3294.AutoFit();

                            var cellAraci = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2] as Excel2.Range;
                            cellAraci.Value2 = "Aracı";

                            var cellBos = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2 + 1] as Excel2.Range;
                            cellBos.Value2 = "";

                            var cellGirisBakildi = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2 + 2] as Excel2.Range;
                            cellGirisBakildi.Value2 = "giriş bakıldı";

                            HafizadanAtilacaklar.AddRange(new List<object> { sheets2, sheet3294, columns, columns3294, cellAraci, cellBos, cellGirisBakildi });

                        }
                        else
                        {
                            var cellDurum = Sabitler.BasvuruFormlariSutunlari[KanunNo].ContainsKey(Enums.BasvuruFormuSutunTurleri.Durum) ? allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Durum] + 2] as Excel2.Range : null;

                            if (cellDurum != null) cellDurum.Value2 = "Durum";

                            var cellAraci = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2] as Excel2.Range;
                            cellAraci.Value2 = "Aracı";

                            var cellBos = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2 + 1] as Excel2.Range;
                            cellBos.Value2 = "";

                            var cellGirisBakildi = allcells[1, Sabitler.BasvuruFormlariSutunlari[KanunNo][Enums.BasvuruFormuSutunTurleri.Araci] + 2 + 2] as Excel2.Range;
                            cellGirisBakildi.Value2 = "giriş bakıldı";

                            HafizadanAtilacaklar.AddRange(new List<object> { cellDurum, cellAraci, cellBos, cellGirisBakildi });

                        }


                        var sheetall = CalismaKitabi.Sheets;

                        HafizadanAtilacaklar.Add(sheetall);

                        if (sheetall.Count >= 4)
                        {
                            var sheet7103 = sheetall[4];

                            sheet7103.Activate();

                            HafizadanAtilacaklar.Add(sheet7103);
                        }
                    }

                    Excelim.Visible = false;
                    Excelim.DisplayAlerts = false;

                    if (ExcelimV2 != null)
                    {
                        ExcelimV2.Visible = false;
                        ExcelimV2.DisplayAlerts = false;
                    }

                    int excelprocessid = Metodlar.GetExcelProcessId(Excelim);
                    int excelprocessid2 = ExcelimV2 == null ? 0 : Metodlar.GetExcelProcessId(ExcelimV2);

                    try
                    {

                        if (_10_Kere_Kaydedilemedi)
                            throw new Exception("10 denemeye rağmen başvuru formu exceline bilgiler kopyalanamadı");

                        if (File.Exists(filename))
                        {
                            CalismaKitabi.Save();
                        }
                        else
                        {
                            CalismaKitabi.SaveAs(filename);

                        }

                        result = true;
                    }
                    catch (Exception ex)
                    {
                        Metodlar.HataMesajiGoster(ex, KanunNo + " başvuru dosyası kaydedilirken hata oluştu.");

                        result = false;
                    }

                    CalismaKitabi.Close(false);
                    if (CalismaKitabiV2 != null)
                    {
                        CalismaKitabiV2.Close(false);
                    }

                    HafizadanAtilacaklar.Reverse();

                    int j = 0;

                    while (j < HafizadanAtilacaklar.Count())
                    {
                        try
                        {
                            var item = HafizadanAtilacaklar.ElementAt(j);

                            if (item != null)
                            {

                                Marshal.FinalReleaseComObject(item);
                            }

                            item = null;

                        }
                        catch
                        {
                        }

                        j++;
                    }


                    Excelim.Quit();
                    Marshal.FinalReleaseComObject(Excelim);
                    if (ExcelimV2 != null)
                    {
                        ExcelimV2.Quit();
                        Marshal.FinalReleaseComObject(ExcelimV2);
                    }


                    Metodlar.KillProcessById(excelprocessid);

                    if (excelprocessid2 > 0) Metodlar.KillProcessById(excelprocessid2);

                    if (geciciDosyaYolu != null)
                    {
                        try
                        {
                            if (File.Exists(geciciDosyaYolu))
                            {
                                File.Delete(geciciDosyaYolu);
                            }
                        }
                        catch { }
                    }


                    // Biçimlendirme
                    /*
                    CalismaSayfasi.get_Range("A1", "J1").EntireColumn.AutoFit();
                    CalismaSayfasi.get_Range("A1", "J1").Font.Bold = true;
                    CalismaSayfasi.get_Range("A1", "J1").RowHeight = 16;
                    CalismaSayfasi.get_Range("A1", "J1").Rows.VerticalAlignment = 2;
                    */

                    //DisDataGrid.AllowUserToAddRows = true ;
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                BfKaydediliyor = false;
            }

            return result;
        }

        public bool BasvuruListesi7166Kaydet(System.Data.DataTable DisDataGrid, string filename)
        {
            while (Kaydediliyor7166)
            {
                Thread.Sleep(500);
            }

            Kaydediliyor7166 = true;
            bool result = false;
            try
            {
                Excel2.Application Excelim;
                Excel2.Workbook CalismaKitabi;
                Excel2.Worksheet CalismaSayfasi;



                if (DisDataGrid.Rows.Count > 0)
                {
                    Excelim = new Excel2.Application();
                    object SalakObje = System.Reflection.Missing.Value;

                    bool DosyaMevcutMu = File.Exists(filename);

                    var workbooks = Excelim.Workbooks;

                    CalismaKitabi = workbooks.Open(DosyaMevcutMu ? filename : Path.Combine(Application.StartupPath, "Sablon7166.xlsx"));

                    int SayfaNo = 1;

                    var sheets = CalismaKitabi.Sheets;

                    HafizadanAtilacaklar.AddRange(new List<object> { workbooks, CalismaKitabi, sheets });

                    CalismaSayfasi = (Excel2.Worksheet)sheets[SayfaNo];
                    CalismaSayfasi.Activate();

                    HafizadanAtilacaklar.Add(CalismaSayfasi);

                    var usedrange = CalismaSayfasi.UsedRange;

                    var allcells = CalismaSayfasi.Cells;

                    Excel2.Range lastrow = usedrange.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                    Excel2.Range lastrow2 = allcells[lastrow.Row + 1, lastrow.Column];

                    Excel2.Range col7103Verilmis = allcells[1, (int)Enums.BasvuruListesi7166SutunTurleri.VerilmisMi7103 + 1];
                    col7103Verilmis.Value2 = "7103 Verilmiş Mi?";

                    HafizadanAtilacaklar.AddRange(new List<object> { usedrange, allcells, lastrow, lastrow2, col7103Verilmis });

                    if (lastrow.Row > 1)
                    {

                        Excel2.Range range = CalismaSayfasi.get_Range("A2", lastrow2);

                        var entirerow = range.EntireRow;

                        entirerow.Delete(Excel2.XlDeleteShiftDirection.xlShiftUp);

                        HafizadanAtilacaklar.AddRange(new List<object> { range, entirerow });
                    }


                    ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook();

                    wb.Worksheets.Add(DisDataGrid);

                    string geciciDosyaYolu = filename.Insert(filename.IndexOf(".xlsx"), "_GEÇİCİ");

                    wb.SaveAs(geciciDosyaYolu);



                    //NewExportExcelV2 excelV2 = new NewExportExcelV2();
                    //excelV2.gridControl1.DataSource = DisDataGrid;
                    //string geciciDosyaYolu = filename.Insert(filename.IndexOf(".xlsx"), "_GEÇİCİ");
                    //excelV2.gridView1.ExportToXlsx(geciciDosyaYolu);


                    var ExcelimV2 = new Excel2.Application();
                    var workbooks2 = ExcelimV2.Workbooks;
                    var CalismaKitabiV2 = workbooks2.Open(geciciDosyaYolu);

                    HafizadanAtilacaklar.AddRange(new List<object> { workbooks2, CalismaKitabiV2 });

                    int hataSayaci = 0;

                    while (hataSayaci < 3)
                    {
                        try
                        {
                            object SalakObjeV2 = System.Reflection.Missing.Value;
                            var CalismaSayfasiV2 = (Excel2.Worksheet)CalismaKitabiV2.ActiveSheet;

                            var usedrange4 = CalismaSayfasi.UsedRange;

                            Excel2.Range lastcell = usedrange4.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                            var usedrange2 = CalismaSayfasiV2.UsedRange;

                            Excel2.Range lastV2 = usedrange2.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                            var tumAlanV2 = CalismaSayfasiV2.Range["A2", lastV2];

                            tumAlanV2.Copy(Type.Missing);

                            var tumAlan = CalismaSayfasi.Range["A:X"];

                            tumAlan.NumberFormat = "@";

                            Excel2.Range rangeV2 = allcells[2, 1] as Excel2.Range;
                            rangeV2.Select();
                            CalismaSayfasi.Paste(Type.Missing, Type.Missing);

                            var usedrange3 = CalismaSayfasi.UsedRange;

                            Excel2.Range lastcell2 = usedrange3.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                            HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasiV2, usedrange4, lastcell, usedrange2, lastV2, tumAlanV2, tumAlan, rangeV2, usedrange3, lastcell2 });

                            if (lastcell.Row + lastV2.Row - 1 > lastcell2.Row)
                            {
                                CalismaKitabi.Close(false);

                                CalismaKitabi = workbooks.Open(DosyaMevcutMu ? filename : Path.Combine(Application.StartupPath, "Sablon7166.xlsx"));
                                CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                                HafizadanAtilacaklar.AddRange(new List<object> { CalismaKitabi, CalismaSayfasi });

                                continue;
                            }

                            var tumAlan3 = CalismaSayfasi.Range["A:X"];

                            var font = tumAlan3.Font;

                            font.Size = 12;
                            font.Name = "Times New Roman";

                            HafizadanAtilacaklar.AddRange(new List<object> { tumAlan3, font });

                            break;

                        }
                        catch (Exception ex)
                        {
                            hataSayaci++;

                            frmIsyerleri.LogYaz("HATA OLUŞTU:" + ex.Message + Environment.NewLine);

                        }
                    }

                    var sheetall = CalismaKitabi.Sheets;

                    HafizadanAtilacaklar.Add(sheetall);

                    Excelim.Visible = false;
                    Excelim.DisplayAlerts = false;

                    ExcelimV2.Visible = false;
                    ExcelimV2.DisplayAlerts = false;

                    int excelprocessid = Metodlar.GetExcelProcessId(Excelim);
                    int excelprocessid2 = Metodlar.GetExcelProcessId(ExcelimV2);

                    try
                    {
                        if (File.Exists(filename))
                        {
                            CalismaKitabi.Save();
                        }
                        else
                        {
                            CalismaKitabi.SaveAs(filename);

                        }

                        result = true;
                    }
                    catch (Exception ex)
                    {
                        Metodlar.HataMesajiGoster(ex, "7166 listesi kaydedilirken hata oluştu.");

                        result = false;
                    }

                    CalismaKitabi.Close(false);
                    CalismaKitabiV2.Close(false);

                    HafizadanAtilacaklar.Reverse();

                    int j = 0;

                    while (j < HafizadanAtilacaklar.Count())
                    {
                        try
                        {
                            var item = HafizadanAtilacaklar.ElementAt(j);

                            if (item != null)
                            {

                                Marshal.FinalReleaseComObject(item);
                            }

                            item = null;

                        }
                        catch
                        {
                        }

                        j++;
                    }


                    Excelim.Quit();
                    Marshal.FinalReleaseComObject(Excelim);
                    ExcelimV2.Quit();
                    Marshal.FinalReleaseComObject(ExcelimV2);

                    Metodlar.KillProcessById(excelprocessid);

                    Metodlar.KillProcessById(excelprocessid2);

                    if (File.Exists(geciciDosyaYolu))
                    {
                        File.Delete(geciciDosyaYolu);
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

                Kaydediliyor7166 = false;
            }




            return result;
        }
    }

}
