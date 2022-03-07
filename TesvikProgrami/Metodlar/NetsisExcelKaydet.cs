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
        public static bool NetsisExcelKaydediliyor = false;
        public static string NetsisExcelKaydet(DataTable dt, string savepath)
        {
            while (NetsisExcelKaydediliyor)
            {
                System.Threading.Thread.Sleep(500);
            }

            NetsisExcelKaydediliyor = true;

            try
            {
                string YeniPath = savepath;

                List<object> HafizadanAtilacaklar = new List<object>();

                var result = true;

                try
                {

                    if (dt.Rows.Count > 0)
                    {

                        Excel2.Application Excelim = new Excel2.Application();

                        Excelim.Visible = false;
                        Excelim.DisplayAlerts = false;

                        object SalakObje = System.Reflection.Missing.Value;

                        var workbooks = Excelim.Workbooks;

                        Excel2.Workbook CalismaKitabi = dt.Columns.Count == 33 ? workbooks.Open(Path.Combine(Application.StartupPath, "NetsisExcelSablon33.xls")) : workbooks.Open(Path.Combine(Application.StartupPath, "NetsisExcelSablon36.xls"));
                        Excel2.Worksheet CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                        var cells = CalismaSayfasi.Cells;

                        Excel2.Range last = cells.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Type.Missing);
                        //Excel2.Range range = CalismaSayfasi.get_Range("A1", last);

                        HafizadanAtilacaklar.AddRange(new List<object> { workbooks, CalismaKitabi, CalismaSayfasi, cells, last });

                        int LastRow = last.Row;

                        int SatirIndex = LastRow + 1;

                        NewExportExcelV2 excelV2 = new NewExportExcelV2();
                        excelV2.gridControl1.DataSource = dt;
                        string geciciDosyaYolu = Path.Combine(Path.GetDirectoryName(YeniPath),YeniPath.Insert(YeniPath.IndexOf(".xls") == -1 ? YeniPath.IndexOf(".XLS") : YeniPath.IndexOf(".xls"), "_GEÇİCİ_NetsisExcel"));
                        excelV2.gridView1.ExportToXlsx(geciciDosyaYolu);


                        var ExcelimV2 = new Excel2.Application();

                        ExcelimV2.Visible = false;
                        ExcelimV2.DisplayAlerts = false;

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

                                Excel2.Range lastcell = cells.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Type.Missing);

                                var cellsV2 = CalismaSayfasiV2.Cells;

                                Excel2.Range lastV2 = cellsV2.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Type.Missing);

                                var kopyalanacakAlan = CalismaSayfasiV2.Range["A2", lastV2];

                                kopyalanacakAlan.Copy(Type.Missing);

                                Excel2.Range rangeV2 = cells[SatirIndex, 1] as Excel2.Range;
                                rangeV2.Select();
                                CalismaSayfasi.Paste(Type.Missing, Type.Missing);

                                Excel2.Range lastcell2 = cells.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Type.Missing);

                                HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasiV2, lastcell, cellsV2, lastV2, kopyalanacakAlan, rangeV2, lastcell2 });

                                if (LastRow + lastV2.Row - 1 > lastcell2.Row)
                                {
                                    CalismaKitabi.Close(false);

                                    CalismaKitabi = dt.Columns.Count == 33 ? workbooks.Open(Path.Combine(Application.StartupPath, "NetsisExcelSablon33.xls")) : workbooks.Open(Path.Combine(Application.StartupPath, "NetsisExcelSablon36.xls"));
                                    CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                                    HafizadanAtilacaklar.AddRange(new List<object> { CalismaKitabi, CalismaSayfasi });

                                    continue;
                                }

                                var tumAlan = CalismaSayfasi.Range["A:AF"];

                                tumAlan.NumberFormat = "@";

                                var font = tumAlan.Font;
                                font.Size = 10;

                                var cellsYeni = CalismaSayfasi.Cells;

                                var lastcellSon = cellsYeni.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Type.Missing);

                                var tumAlanSon = CalismaSayfasi.Range["A2", lastcellSon];

                                var tumAlanBorders = tumAlanSon.Borders;

                                tumAlanBorders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                HafizadanAtilacaklar.AddRange(new List<object> { tumAlan,font, cellsYeni, lastcellSon, tumAlanSon, tumAlanBorders});
                                
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


                        HafizadanAtilacaklar.AddRange(new List<object> { allcells2 });

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
                                if (File.Exists(YeniPath)) File.Delete(YeniPath);

                                var ext = Path.GetExtension(YeniPath).ToLower();

                                if (ext == ".xlsx")
                                {
                                    CalismaKitabi.SaveAs(YeniPath, Excel2.XlFileFormat.xlOpenXMLWorkbook);
                                }
                                else
                                     CalismaKitabi.SaveAs(YeniPath);

                            }
                            else
                            {
                                result = false;
                            }

                        }
                        catch (Exception ex)
                        {
                            Metodlar.HataMesajiGoster(ex, "Netsis excel dosyası kaydedilirken hata oluştu.");

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

                    if (result) return YeniPath;

                }
                catch (Exception ex)
                {
                    string Mesaj = "Netsis excel listesi hata nedeniyle kaydedilemedi" + Environment.NewLine;

                    HataMesajiGoster(ex, Mesaj);
                }
            }
            catch (Exception)
            {
                NetsisExcelKaydediliyor = false;

                throw;
            }
            finally {
                NetsisExcelKaydediliyor = false;
            }

            NetsisExcelKaydediliyor = false;

            return null;
        }


    }



}
