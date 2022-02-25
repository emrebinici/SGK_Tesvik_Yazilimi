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

        public static bool Liste7252Kaydediliyor = false;
        public static string Liste7252Kaydet (Isyerleri isyeri, DataTable dt)
        {
            while (Liste7252Kaydediliyor)
            {
                System.Threading.Thread.Sleep(500);
            }

            Liste7252Kaydediliyor = true;

            try
            {
                var tarih = DateTime.Now.ToString("ddMMyyyy_HHmmss");

                string YeniPath = Path.Combine( Metodlar.IsyeriKlasorBul(isyeri,true),  String.Format("7252 teşvikine tanımlananların listesi_{0}.xlsx",tarih));

                List<object> HafizadanAtilacaklar = new List<object>();

                try
                {


                    Excel2.Application Excelim;
                    Excel2.Workbook CalismaKitabi;
                    Excel2.Worksheet CalismaSayfasi;

                    bool result = false;


                    if (dt.Rows.Count > 0)
                    {
                        dt.Columns.RemoveAt(12);

                        var dtNew = new DataTable("7252");
                        dtNew.Columns.Add("SiraNo");

                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            dtNew.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
                        }

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var newRow = dtNew.NewRow();
                            newRow[0] = i + 1;

                            for (int z = 0; z < dt.Columns.Count; z++)
                            {
                                newRow[z + 1] = dt.Rows[i][z];
                            }

                            dtNew.Rows.Add(newRow);
                        }

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
                        CalismaSayfasi.Name = "İşveren İntra";
                        Excel2.Worksheet sheet2 = null;
                        Excel2.Worksheet sheet3 = null;

                        if (CalismaKitabi.Sheets.Count == 3)
                        {
                            sheet2 = CalismaKitabi.Sheets[2] as Excel2.Worksheet;
                            sheet3 = CalismaKitabi.Sheets[3] as Excel2.Worksheet;

                            sheet3.Delete();
                            sheet2.Delete();
                        }


                        HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasi, sheet2, sheet3 });

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

                        wb.Worksheets.Add(dtNew);

                        string geciciDosyaYolu = YeniPath.Insert(YeniPath.IndexOf(".xlsx"), "_GEÇİCİ_7252Listesi");

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

                                var rangeBaslik = CalismaSayfasi.Range["A1", "M1"] as Excel2.Range;

                                rangeBaslik.Merge();
                                rangeBaslik.HorizontalAlignment = Excel2.XlHAlign.xlHAlignLeft;
                                rangeBaslik.Value2 = String.Format("{0} {1} İŞYERİNE AİT 4447/26. MADDE <br>", isyeri.IsyeriSicilNo.BoslukluSicilNoyaDonustur(), isyeri.Sirketler.SirketAdi);

                                var fontBaslik = rangeBaslik.Font;

                                fontBaslik.Bold = true;
                                var ilkRow = rangeBaslik.EntireRow;
                                ilkRow.RowHeight = 50;

                                var rangeSiraNo = CalismaSayfasi.Cells[2, 1] as Excel2.Range;
                                rangeSiraNo.Value2 = "NO";

                                var rangeTcNo = CalismaSayfasi.Cells[2, 2] as Excel2.Range;
                                rangeTcNo.Value2 = "TC No";
                                rangeTcNo.ColumnWidth = 16;

                                var rangeSicilNo = CalismaSayfasi.Cells[2, 3] as Excel2.Range;
                                rangeSicilNo.Value2 = "Sicil No";
                                rangeSicilNo.ColumnWidth = 16;

                                var rangeAd = CalismaSayfasi.Cells[2, 4] as Excel2.Range;
                                rangeAd.Value2 = "Ad";

                                var rangeSoyad = CalismaSayfasi.Cells[2, 5] as Excel2.Range;
                                rangeSoyad.Value2 = "Soyad";

                                var rangeBaslangic = CalismaSayfasi.Cells[2, 6] as Excel2.Range;
                                rangeBaslangic.Value2 = "Başlangıç Dönemi";

                                var rangeBitis = CalismaSayfasi.Cells[2, 7] as Excel2.Range;
                                rangeBitis.Value2 = "Bitiş Dönemi";

                                var rangeKCONUD = CalismaSayfasi.Cells[2, 8] as Excel2.Range;
                                rangeKCONUD.Value2 = "KÇÖ/NÜD Sonlanma Tarihi";
                                rangeKCONUD.ColumnWidth = 12;

                                var rangeOrtalamaGunSayisi = CalismaSayfasi.Cells[2, 9] as Excel2.Range;
                                rangeOrtalamaGunSayisi.Value2 = "Ortalama Gün Sayısı";
                                rangeOrtalamaGunSayisi.ColumnWidth = 4;

                                var rangeKanun = CalismaSayfasi.Cells[2, 10] as Excel2.Range;
                                rangeKanun.Value2 = "Kanun Numarası";
                                rangeKanun.ColumnWidth = 6;

                                var rangeIseGirisTarihi = CalismaSayfasi.Cells[2, 11] as Excel2.Range;
                                rangeIseGirisTarihi.Value2 = "İşe Giriş Tarihi";
                                rangeIseGirisTarihi.ColumnWidth = 12;

                                var rangeIstenAyrilisTarihi = CalismaSayfasi.Cells[2, 12] as Excel2.Range;
                                rangeIstenAyrilisTarihi.Value2 = "İşten Ayrılış Tarihi";
                                rangeIstenAyrilisTarihi.ColumnWidth = 12;

                                var rangeIlkTanimlamaTarihi = CalismaSayfasi.Cells[2, 13] as Excel2.Range;
                                rangeIlkTanimlamaTarihi.Value2 = "İlk Tanımlama Tarihi";
                                rangeIlkTanimlamaTarihi.ColumnWidth = 12;


                                Excel2.Range rangeV2 = allcells[3, 1] as Excel2.Range;
                                CalismaSayfasi.Activate();

                                rangeV2.Select();
                                CalismaSayfasi.Paste(Type.Missing, Type.Missing);

                                var usedrange3 = CalismaSayfasi.UsedRange;

                                Excel2.Range lastcell2 = usedrange3.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                                HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasiV2, usedrange4, lastcell, usedrange2, lastV2, tumAlanV2, rangeBaslik, fontBaslik, ilkRow, rangeSiraNo, rangeTcNo, rangeSicilNo, rangeAd, rangeSoyad, rangeBaslangic, rangeBitis, rangeKCONUD, rangeOrtalamaGunSayisi, rangeKanun, rangeIseGirisTarihi, rangeIstenAyrilisTarihi, rangeIlkTanimlamaTarihi, rangeV2, usedrange3, lastcell2 });

                                if (lastcell.Row + lastV2.Row - 1 > lastcell2.Row)
                                {
                                    CalismaKitabi.Close(false);

                                    CalismaKitabi = workbooks.Add();
                                    CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                                    HafizadanAtilacaklar.AddRange(new List<object> { CalismaKitabi, CalismaSayfasi });

                                    continue;
                                }


                                var cellIlk = CalismaSayfasi.Cells[1, 1] as Excel2.Range;
                                cellIlk.Select();

                                var tumAlan3 = CalismaSayfasi.UsedRange;

                                //var font = tumAlan3.Font;

                                //font.Size = 12;
                                //font.Name = "Times New Roman";

                                var borders2 = tumAlan3.Borders;
                                borders2.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                rangeSiraNo.EntireColumn.AutoFit();
                                rangeAd.EntireColumn.AutoFit();
                                rangeSoyad.EntireColumn.AutoFit();

                                HafizadanAtilacaklar.AddRange(new List<object> { cellIlk, tumAlan3, borders2 });

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

                            var eskiDosya = Metodlar.FormBul(isyeri, Enums.FormTuru.Liste7252);

                            if (eskiDosya != null) File.Delete(eskiDosya);

                            CalismaKitabi.SaveAs(YeniPath);

                            result = true;
                        }
                        catch (Exception ex)
                        {
                            Metodlar.HataMesajiGoster(ex, "7252 listesi kaydedilirken hata oluştu.");

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
                    else
                    {
                        try
                        {
                            if (File.Exists(YeniPath))
                            {
                                File.Delete(YeniPath);
                                
                                return "7252 listesi silindi";
                            }
                        }
                        finally {
                            
                        }

                        return "7252 listesi silinmedi";
                    }

                    if (result) return YeniPath;

                }
                catch (Exception ex)
                {
                    string Mesaj ="7252 listesi hata nedeniyle kaydedilemedi" + Environment.NewLine;

                    HataMesajiGoster(ex, Mesaj);
                }
            }
            catch (Exception)
            {
                Liste7252Kaydediliyor = false;

                throw;
            }
            finally
            {
                Liste7252Kaydediliyor = false;
            }

            Liste7252Kaydediliyor = false;

            return null;
        }


    }



}
