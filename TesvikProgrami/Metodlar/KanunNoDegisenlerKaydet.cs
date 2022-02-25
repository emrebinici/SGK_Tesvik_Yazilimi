using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TesvikProgrami.Classes;
using Excel2 = Microsoft.Office.Interop.Excel;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {

        public static bool KanunNoDegisenlerKaydediliyor  = false;
        public static string KanunNoDegisenlerKaydet(Dictionary<AphbSatir, KeyValuePair<string,string>> kanunNoDegisenler, string directory, Isyerleri isyeri, string yil, string ay)
        {
            while (KanunNoDegisenlerKaydediliyor)
            {
                System.Threading.Thread.Sleep(500);
            }

            KanunNoDegisenlerKaydediliyor = true;

            try
            {
                string YeniPath =  Path.Combine(directory, "Kanun Numarası Değiştirilenler.xlsx");

                List<object> HafizadanAtilacaklar = new List<object>();

                try
                {


                    Excel2.Application Excelim;
                    Excel2.Workbook CalismaKitabi;
                    Excel2.Worksheet CalismaSayfasi;

                    bool result = false;

                    DataTable dt = new DataTable("Kanun_No_Degisenler");
                    dt.Columns.Add("Sira", typeof(int));
                    dt.Columns.Add("SosyalGuvenlikSicilNumarasi");
                    dt.Columns.Add("Adi");
                    dt.Columns.Add("Soyadi");
                    dt.Columns.Add("BelgeTuru");
                    dt.Columns.Add("EskiKanun");
                    dt.Columns.Add("YeniKanun");


                    int sirano = 1;

                    var siraliListe = kanunNoDegisenler.OrderBy(p => p.Key.SosyalGuvenlikNo);

                    foreach (var kv in siraliListe)
                    {
                        var row = dt.NewRow();
                        var satir = kv.Key;
                        var eskiKanun = kv.Value.Key;
                        var yeniKanun = kv.Value.Value;

                        row[0] = sirano++;
                        row[1] = satir.SosyalGuvenlikNo;
                        row[2] = satir.Adi;
                        row[3] = satir.Soyadi;
                        row[4] = satir.BelgeTuru.PadLeft(2,'0');
                        row[5] = eskiKanun;
                        row[6] = yeniKanun;

                        dt.Rows.Add(row);
                    }


                    if (dt.Rows.Count > 0)
                    {
                       
                        Excelim = new Excel2.Application();

                        Excelim.Visible = false;
                        Excelim.DisplayAlerts = false;

                        object SalakObje = System.Reflection.Missing.Value;

                        var workbooks = Excelim.Workbooks;

                        CalismaKitabi = workbooks.Add();

                        var sheets = CalismaKitabi.Sheets;

                        HafizadanAtilacaklar.AddRange(new List<object> { workbooks, CalismaKitabi, sheets });

                        var ExcelimV2 = new Excel2.Application();

                        ExcelimV2.Visible = false;
                        ExcelimV2.DisplayAlerts = false;

                        var workbooks2 = ExcelimV2.Workbooks;

                        var GenelToplamHucreleri = new List<string>();

                        #region Kaydet 
                        int SayfaNo = 1;

                        CalismaSayfasi = (Excel2.Worksheet)sheets[SayfaNo];
                        CalismaSayfasi.Activate();
                        CalismaSayfasi.Name = "Kanun Numarası Değiştirilenler";
                        Excel2.Worksheet sheet2 = null;
                        Excel2.Worksheet sheet3 = null;

                        if (CalismaKitabi.Sheets.Count == 3)
                        {
                            sheet2 = CalismaKitabi.Sheets[2] as Excel2.Worksheet;
                            sheet3 = CalismaKitabi.Sheets[3] as Excel2.Worksheet;

                            sheet3.Delete();
                            sheet2.Delete();
                        }


                        HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasi , sheet2, sheet3});

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


                        ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook();

                        wb.Worksheets.Add(dt);

                        string geciciDosyaYolu = YeniPath.Insert(YeniPath.IndexOf(".xlsx"), "_GEÇİCİ_KanunNoDegisenler");

                        wb.SaveAs(geciciDosyaYolu);

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

                                var rangeIsverenAdiLabel = CalismaSayfasi.Range[CalismaSayfasi.Cells[1, 1], CalismaSayfasi.Cells[1, 2]] as Excel2.Range;
                                rangeIsverenAdiLabel.Merge();
                                rangeIsverenAdiLabel.Value2 = "İşveren adı";

                                var rangeIsverenAdi = CalismaSayfasi.Range[CalismaSayfasi.Cells[1, 3], CalismaSayfasi.Cells[1, 7]] as Excel2.Range;
                                rangeIsverenAdi.Merge();
                                rangeIsverenAdi.Value2 = isyeri.Sirketler.SirketAdi;

                                var rangeIsyeriNoLabel = CalismaSayfasi.Range[CalismaSayfasi.Cells[2, 1], CalismaSayfasi.Cells[2, 2]] as Excel2.Range;
                                rangeIsyeriNoLabel.Merge();
                                rangeIsyeriNoLabel.Value2 = "İşyeri no";

                                var rangeIsyeriNo= CalismaSayfasi.Range[CalismaSayfasi.Cells[2,3], CalismaSayfasi.Cells[2, 7]] as Excel2.Range;
                                rangeIsyeriNo.Merge();
                                rangeIsyeriNo.Value2 = isyeri.IsyeriSicilNo.BoslukluSicilNoyaDonustur();

                                var rangeBeyannameYilAyLabel = CalismaSayfasi.Range[CalismaSayfasi.Cells[3, 1], CalismaSayfasi.Cells[3, 2]] as Excel2.Range;
                                rangeBeyannameYilAyLabel.Merge();
                                rangeBeyannameYilAyLabel.Value2 = "Beyanamenin ait olduğu yıl/ay";

                                var rangeBeyannameYilAy = CalismaSayfasi.Range[CalismaSayfasi.Cells[3, 3], CalismaSayfasi.Cells[3, 7]] as Excel2.Range;
                                rangeBeyannameYilAy.Merge();
                                rangeBeyannameYilAy.Value2 = yil  +"/"+ay.PadLeft(2,'0');

                                var rangeSigortaliSayisiLabel = CalismaSayfasi.Range[CalismaSayfasi.Cells[4, 1], CalismaSayfasi.Cells[4, 2]] as Excel2.Range;
                                rangeSigortaliSayisiLabel.Merge();
                                rangeSigortaliSayisiLabel.Value2 = "Sigortalı Sayısı";

                                var rangeSigortaliSayisi = CalismaSayfasi.Range[CalismaSayfasi.Cells[4, 3], CalismaSayfasi.Cells[4, 7]] as Excel2.Range;
                                rangeSigortaliSayisi.Merge();
                                rangeSigortaliSayisi.NumberFormat = "@";
                                rangeSigortaliSayisi.Value2 = kanunNoDegisenler.Count;


                                var rangeSira= CalismaSayfasi.Cells[5, 1] as Excel2.Range;
                                var rangeSosyalGuvenlikNo= CalismaSayfasi.Cells[5, 2] as Excel2.Range;
                                var rangeAd = CalismaSayfasi.Cells[5, 3] as Excel2.Range;
                                var rangeSoyad = CalismaSayfasi.Cells[5, 4] as Excel2.Range;
                                var rangeBelgeTuru = CalismaSayfasi.Cells[5, 5] as Excel2.Range;
                                var rangeEskiKanun = CalismaSayfasi.Cells[5, 6] as Excel2.Range;
                                var rangeYeniKanun = CalismaSayfasi.Cells[5, 7] as Excel2.Range;

                                rangeSira.Value2 = "Sıra No";
                                rangeSosyalGuvenlikNo.Value2 = "T.C. Numarası";
                                rangeAd.Value2 = "Adı";
                                rangeSoyad.Value2 = "Soyadı";
                                rangeBelgeTuru.Value2 = "Belge Türü";
                                rangeEskiKanun.Value2 = "Eski kanun numarası";
                                rangeYeniKanun.Value2 = "Yeni kanun numarası";

                                var rangeIsyeriBilgileri = CalismaSayfasi.get_Range("A1","C4");

                                rangeIsyeriBilgileri.HorizontalAlignment = Excel2.XlHAlign.xlHAlignLeft;
                                rangeIsyeriBilgileri.RowHeight = 20;
                                rangeIsyeriBilgileri.NumberFormat = "@";


                                var rangeheader = CalismaSayfasi.Range[rangeSira, rangeYeniKanun];

                                var rangeLabellar = CalismaSayfasi.Range[rangeIsverenAdiLabel, rangeSigortaliSayisiLabel];

                                rangeLabellar.Font.Bold = true;

                                rangeheader.Style.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;
                                rangeheader.Style.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;
                                rangeheader.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;
                                rangeheader.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var interior = rangeheader.Interior;

                                interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(197, 217, 241));


                                CalismaSayfasi.Columns["B:G"].ColumnWidth = 20;

                                rangeSira.WrapText = true;
                                rangeSosyalGuvenlikNo.WrapText = true;
                                rangeAd.WrapText = true;
                                rangeSoyad.WrapText = true;
                                rangeBelgeTuru.WrapText = true;
                                rangeEskiKanun.WrapText = true;
                                rangeYeniKanun.WrapText = true;

                                var ilkSatir = rangeSira.EntireRow;

                                var borders = rangeheader.Borders;

                                var fontHeader = rangeheader.Font;
                                fontHeader.Bold = true;

                                borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                Excel2.Range rangeV2 = allcells[6, 1] as Excel2.Range;
                                CalismaSayfasi.Activate();

                                rangeV2.Select();
                                CalismaSayfasi.Paste(Type.Missing, Type.Missing);

                                var usedrange3 = CalismaSayfasi.UsedRange;

                                Excel2.Range lastcell2 = usedrange3.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                                HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasiV2, usedrange4, lastcell, usedrange2, lastV2, tumAlanV2, rangeIsverenAdiLabel, rangeIsverenAdi, rangeIsyeriNoLabel,rangeIsyeriNo, rangeBeyannameYilAyLabel,rangeBeyannameYilAy, rangeBelgeTuru, rangeSigortaliSayisi,rangeSigortaliSayisiLabel, rangeSira,rangeSosyalGuvenlikNo,rangeAd, rangeSoyad, rangeBelgeTuru,rangeEskiKanun,rangeYeniKanun ,ilkSatir,rangeIsyeriBilgileri, rangeLabellar, rangeheader, interior, borders, fontHeader, rangeV2, usedrange3, lastcell2 });

                                if (lastcell.Row + lastV2.Row - 1 > lastcell2.Row)
                                {
                                    CalismaKitabi.Close(false);

                                    CalismaKitabi = workbooks.Add();
                                    CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                                    HafizadanAtilacaklar.AddRange(new List<object> { CalismaKitabi, CalismaSayfasi });

                                    continue;
                                }

                                var cellsSiraNoTcNo = CalismaSayfasi.Range["A6", "B" + lastcell2.Row];
                                cellsSiraNoTcNo.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var cellsBelgeTuru = CalismaSayfasi.Range["E6", "G" + lastcell2.Row];
                                cellsBelgeTuru.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var cellsYeniKanun = CalismaSayfasi.Range["G6", "G" + lastcell2.Row];
                                var fontYeniKanun = cellsYeniKanun.Font;
                                fontYeniKanun.Bold = true;

                                var cellIlk = CalismaSayfasi.Cells[1, 1] as Excel2.Range;
                                cellIlk.Select();

                                var tumAlan3 = CalismaSayfasi.UsedRange;

                                tumAlan3.RowHeight = 15.60;

                                var font = tumAlan3.Font;

                                font.Size = 12;
                                font.Name = "Times New Roman";

                                var borders2 = tumAlan3.Borders;
                                borders2.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                HafizadanAtilacaklar.AddRange(new List<object> { cellsSiraNoTcNo,cellsBelgeTuru , cellIlk, tumAlan3, fontYeniKanun, font, borders2 });

                                break;

                            }
                            catch (Exception ex)
                            {
                                hataSayaci++;

                                frmIsyerleri.LogYaz("HATA OLUŞTU:" + ex.Message + Environment.NewLine);

                            }
                        }

                        CalismaKitabiV2.Close(false);

                        if (File.Exists(geciciDosyaYolu))
                        {
                            File.Delete(geciciDosyaYolu);
                        }

                        #endregion

                        var sheetall = CalismaKitabi.Sheets;
                        var ws = (Excel2.Worksheet)sheetall[1];
                        ws.Activate();

                        HafizadanAtilacaklar.AddRange(new List<object> { sheetall, ws });

                        Excelim.Visible = false;
                        Excelim.DisplayAlerts = false;

                        ExcelimV2.Visible = false;
                        ExcelimV2.DisplayAlerts = false;

                        int excelprocessid = Metodlar.GetExcelProcessId(Excelim);
                        int excelprocessid2 = Metodlar.GetExcelProcessId(ExcelimV2);

                        try
                        {
                            if (File.Exists(YeniPath)) File.Delete(YeniPath);

                            CalismaKitabi.SaveAs(YeniPath);

                            result = true;
                        }
                        catch (Exception ex)
                        {
                            Metodlar.HataMesajiGoster(ex, "Kanun numarası değiştirilenler listesi kaydedilirken hata oluştu.");

                            result = false;
                        }

                        CalismaKitabi.Close(false);

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
                    }

                    if (result) return YeniPath;

                }
                catch (Exception ex)
                {
                    string Mesaj = "Kanun numarası değiştirilenler listesi hata nedeniyle kaydedilemedi" + Environment.NewLine;

                    HataMesajiGoster(ex, Mesaj);
                }
            }
            catch (Exception)
            {
                KanunNoDegisenlerKaydediliyor = false;

                throw;
            }
            finally
            {
                KanunNoDegisenlerKaydediliyor = false;
            }

            KanunNoDegisenlerKaydediliyor = false;

            return null;
        }


    }



}
