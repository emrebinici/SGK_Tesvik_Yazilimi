using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Excel2 = Microsoft.Office.Interop.Excel;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {

        public static bool Yersiz7252Kaydediiliyor  = false;
        public static string Yersiz7252Kaydet(Isyerleri isyeri, DataTable dt)
        {
            while (Yersiz7252Kaydediiliyor)
            {
                System.Threading.Thread.Sleep(500);
            }

            Yersiz7252Kaydediiliyor = true;

            try
            {
                var tarih = DateTime.Now.ToString("dd-MM-yyyy");

                string YeniPath = Path.Combine(Metodlar.IsyeriKlasorBul(isyeri, true), String.Format("Yersiz 7252_{0}.xlsx", tarih));

                List<object> HafizadanAtilacaklar = new List<object>();

                try
                {


                    Excel2.Application Excelim;
                    Excel2.Workbook CalismaKitabi;
                    Excel2.Worksheet CalismaSayfasi;

                    bool result = false;

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
                        CalismaSayfasi.Name = "Yersiz 7252";
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

                        string geciciDosyaYolu = YeniPath.Insert(YeniPath.IndexOf(".xlsx"), "_GEÇİCİ_Yersiz7252");

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

                                var rangeIsverenAdi = CalismaSayfasi.Range[CalismaSayfasi.Cells[1, 3], CalismaSayfasi.Cells[1, 15]] as Excel2.Range;
                                rangeIsverenAdi.Merge();
                                rangeIsverenAdi.Value2 = isyeri.Sirketler.SirketAdi;

                                var rangeIsyeriNoLabel = CalismaSayfasi.Range[CalismaSayfasi.Cells[2, 1], CalismaSayfasi.Cells[2, 2]] as Excel2.Range;
                                rangeIsyeriNoLabel.Merge();
                                rangeIsyeriNoLabel.Value2 = "İşyeri no";

                                var rangeIsyeriNo= CalismaSayfasi.Range[CalismaSayfasi.Cells[2,3], CalismaSayfasi.Cells[2, 15]] as Excel2.Range;
                                rangeIsyeriNo.Merge();
                                rangeIsyeriNo.Value2 = isyeri.IsyeriSicilNo.BoslukluSicilNoyaDonustur();

                                var rangeSira= CalismaSayfasi.Cells[3, 1] as Excel2.Range;
                                var rangeSosyalGuvenlikNo= CalismaSayfasi.Cells[3, 2] as Excel2.Range;
                                var rangeAd = CalismaSayfasi.Cells[3, 3] as Excel2.Range;
                                var rangeSoyad = CalismaSayfasi.Cells[3, 4] as Excel2.Range;
                                var range2020_08 = CalismaSayfasi.Cells[3, 5] as Excel2.Range;
                                var range2020_09 = CalismaSayfasi.Cells[3, 6] as Excel2.Range;
                                var range2020_10 = CalismaSayfasi.Cells[3, 7] as Excel2.Range;
                                var range2020_11 = CalismaSayfasi.Cells[3, 8] as Excel2.Range;
                                var range2020_12 = CalismaSayfasi.Cells[3, 9] as Excel2.Range;
                                var range2021_01 = CalismaSayfasi.Cells[3, 10] as Excel2.Range;
                                var range2021_02 = CalismaSayfasi.Cells[3, 11] as Excel2.Range;
                                var range2021_03 = CalismaSayfasi.Cells[3, 12] as Excel2.Range;
                                var range2021_04 = CalismaSayfasi.Cells[3, 13] as Excel2.Range;
                                var range2021_05 = CalismaSayfasi.Cells[3, 14] as Excel2.Range;
                                var range2021_06 = CalismaSayfasi.Cells[3, 15] as Excel2.Range;

                                rangeSira.Value2 = "Sıra No";
                                rangeSosyalGuvenlikNo.Value2 = "T.C. Numarası";
                                rangeAd.Value2 = "Adı";
                                rangeSoyad.Value2 = "Soyadı";
                                range2020_08.Value2 = "2020/08";
                                range2020_09.Value2 = "2020/09";
                                range2020_10.Value2 = "2020/10";
                                range2020_11.Value2 = "2020/11";
                                range2020_12.Value2 = "2020/12";
                                range2021_01.Value2 = "2021/01";
                                range2021_02.Value2 = "2021/02";
                                range2021_03.Value2 = "2021/03";
                                range2021_04.Value2 = "2021/04";
                                range2021_05.Value2 = "2021/05";
                                range2021_06.Value2 = "2021/06";

                                var rangeIsyeriBilgileri = CalismaSayfasi.get_Range("A1","C2") as Excel2.Range;

                                rangeIsyeriBilgileri.HorizontalAlignment = Excel2.XlHAlign.xlHAlignLeft;
                                rangeIsyeriBilgileri.RowHeight = 20;
                                rangeIsyeriBilgileri.NumberFormat = "@";


                                var rangeheader = CalismaSayfasi.Range[rangeSira, range2021_06] as Excel2.Range;

                                var rangeLabellar = CalismaSayfasi.Range[rangeIsverenAdiLabel, rangeIsyeriNoLabel] as Excel2.Range;

                                rangeLabellar.Font.Bold = true;

                                rangeheader.Style.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;
                                rangeheader.Style.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var interior = rangeheader.Interior;

                                interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(197, 217, 241));


                                CalismaSayfasi.Columns["B:D"].ColumnWidth = 20;

                                rangeSira.WrapText = true;
                                rangeSosyalGuvenlikNo.WrapText = true;
                                rangeAd.WrapText = true;
                                rangeSoyad.WrapText = true;


                                var ilkSatir = rangeSira.EntireRow;
                                ilkSatir.RowHeight = 30;

                                var borders = rangeheader.Borders;

                                var fontHeader = rangeheader.Font;
                                fontHeader.Bold = true;

                                borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                Excel2.Range rangeV2 = allcells[4, 1] as Excel2.Range;
                                CalismaSayfasi.Activate();

                                rangeV2.Select();
                                CalismaSayfasi.Paste(Type.Missing, Type.Missing);

                                var usedrange3 = CalismaSayfasi.UsedRange;

                                Excel2.Range lastcell2 = usedrange3.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                                HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasiV2, usedrange4, lastcell, usedrange2, lastV2, tumAlanV2, rangeIsverenAdiLabel, rangeIsverenAdi, rangeIsyeriNoLabel,rangeIsyeriNo, rangeSira,rangeSosyalGuvenlikNo,rangeAd, rangeSoyad, range2020_08,range2020_09,range2020_10,range2020_11,range2020_12,range2021_01,range2021_02,range2021_03,range2021_04,range2021_05,range2021_06 ,ilkSatir,rangeIsyeriBilgileri, rangeLabellar, rangeheader, interior, borders, fontHeader, rangeV2, usedrange3, lastcell2 });

                                if (lastcell.Row + lastV2.Row - 1 > lastcell2.Row)
                                {
                                    CalismaKitabi.Close(false);

                                    CalismaKitabi = workbooks.Add();
                                    CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                                    HafizadanAtilacaklar.AddRange(new List<object> { CalismaKitabi, CalismaSayfasi });

                                    continue;
                                }

                                var cellsSiraNoTcNo = CalismaSayfasi.Range["A4", "B" + lastcell2.Row] as Excel2.Range;
                                cellsSiraNoTcNo.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var cells2020_08 = CalismaSayfasi.Range["E4", "E" + lastcell2.Row] as Excel2.Range;
                                cells2020_08.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var cells2020_09 = CalismaSayfasi.Range["F4", "F" + lastcell2.Row] as Excel2.Range;
                                cells2020_09.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var cells2020_10 = CalismaSayfasi.Range["G4", "G" + lastcell2.Row] as Excel2.Range;
                                cells2020_10.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var cells2020_11 = CalismaSayfasi.Range["H4", "H" + lastcell2.Row] as Excel2.Range;
                                cells2020_11.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var cells2020_12 = CalismaSayfasi.Range["I4", "I" + lastcell2.Row] as Excel2.Range;
                                cells2020_12.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var cells2021_01 = CalismaSayfasi.Range["J4", "J" + lastcell2.Row] as Excel2.Range;
                                cells2021_01.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var cells2021_02 = CalismaSayfasi.Range["K4", "K" + lastcell2.Row] as Excel2.Range;
                                cells2021_02.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var cells2021_03 = CalismaSayfasi.Range["L4", "L" + lastcell2.Row] as Excel2.Range;
                                cells2021_03.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var cells2021_04 = CalismaSayfasi.Range["M4", "M" + lastcell2.Row] as Excel2.Range;
                                cells2021_04.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var cells2021_05 = CalismaSayfasi.Range["N4", "N" + lastcell2.Row] as Excel2.Range;
                                cells2021_05.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var cells2021_06 = CalismaSayfasi.Range["O4", "O" + lastcell2.Row] as Excel2.Range;
                                cells2021_06.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;


                                var cellIlk = CalismaSayfasi.Cells[1, 1] as Excel2.Range;
                                cellIlk.Select();

                                var tumAlan3 = CalismaSayfasi.UsedRange;

                                var font = tumAlan3.Font;

                                font.Size = 12;
                                font.Name = "Times New Roman";

                                var borders2 = tumAlan3.Borders;
                                borders2.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                HafizadanAtilacaklar.AddRange(new List<object> { cellsSiraNoTcNo,cells2020_08,cells2020_09,cells2020_10,cells2020_11,cells2020_12,cells2021_01,cells2021_02,cells2021_03,cells2021_04,cells2021_05,cells2021_06, cellIlk, tumAlan3, font, borders2 });

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
                            var eskiDosya = Metodlar.FormBul(isyeri, Enums.FormTuru.Yersiz7252);
                            
                            if (File.Exists(YeniPath)) File.Delete(YeniPath);

                            if (File.Exists(eskiDosya)) File.Delete(eskiDosya);

                            CalismaKitabi.SaveAs(YeniPath);

                            result = true;
                        }
                        catch (Exception ex)
                        {
                            Metodlar.HataMesajiGoster(ex, "Yersiz 7252 listesi kaydedilirken hata oluştu.");

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
                    string Mesaj = "Yersiz 7252 listesi hata nedeniyle kaydedilemedi" + Environment.NewLine;

                    HataMesajiGoster(ex, Mesaj);
                }
            }
            catch (Exception)
            {
                Yersiz7252Kaydediiliyor = false;

                throw;
            }
            finally
            {
                Yersiz7252Kaydediiliyor = false;
            }

            Yersiz7252Kaydediiliyor = false;

            return null;
        }


    }



}
