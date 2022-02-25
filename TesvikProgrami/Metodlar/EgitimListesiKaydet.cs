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
        public static bool EgitimListesiKaydediliyor = false;
        public static string EgitimListesiKaydet(Isyerleri isyeri, DataTable dt)
        {

            while (EgitimListesiKaydediliyor)
            {
                System.Threading.Thread.Sleep(500);
            }

            EgitimListesiKaydediliyor = true;

            var result = true;

            string YeniPath = null;

            try
            {

                var isyeripath = Metodlar.IsyeriKlasorBul(isyeri, true);

                YeniPath = Path.Combine(isyeripath, String.Format("{0} Eğitim Belgesi Verilecekler.xlsx", isyeri.SubeAdi));

                List<object> HafizadanAtilacaklar = new List<object>();

                if (dt.Rows.Count > 0)
                {

                    Excel2.Application Excelim = null;
                    Excel2.Workbook CalismaKitabi = null;
                    Excel2.Worksheet CalismaSayfasi = null;
                    Excel2.Application ExcelimV2 = null;
                    Excel2.Workbook CalismaKitabiV2 = null;
                    Excel2.Worksheet CalismaSayfasiV2 = null;

                    int excelprocessid = 0;
                    int excelprocessid2 = 0;

                    string geciciDosyaYolu = null;
                    try
                    {
                        Excelim = new Excel2.Application();

                        Excelim.Visible = false;
                        Excelim.DisplayAlerts = false;

                        object SalakObje = System.Reflection.Missing.Value;

                        var workbooks = Excelim.Workbooks;

                        CalismaKitabi = workbooks.Open(Path.Combine(Application.StartupPath, "EgitimSablon.xlsx"));
                        CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                        var cells = CalismaSayfasi.Cells;

                        HafizadanAtilacaklar.AddRange(new List<object> { workbooks, CalismaKitabi, CalismaSayfasi, cells });

                        int SatirIndex = 2;

                        ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook();

                        wb.Worksheets.Add(dt);

                        geciciDosyaYolu = YeniPath.Insert(YeniPath.IndexOf(".xlsx"), "_GEÇİCİ_Egitim");

                        wb.SaveAs(geciciDosyaYolu);

                        //NewExportExcelV2 excelV2 = new NewExportExcelV2();
                        //excelV2.gridControl1.DataSource = dt;
                        //string geciciDosyaYolu = YeniPath.Insert(YeniPath.IndexOf(".xlsx"), "_GEÇİCİ_Egitim");
                        //excelV2.gridView1.ExportToXlsx(geciciDosyaYolu);


                        ExcelimV2 = new Excel2.Application();

                        ExcelimV2.Visible = false;
                        ExcelimV2.DisplayAlerts = false;

                        var workbooks2 = ExcelimV2.Workbooks;
                        CalismaKitabiV2 = workbooks2.Open(geciciDosyaYolu);

                        HafizadanAtilacaklar.AddRange(new List<object> { workbooks2, CalismaKitabiV2 });

                        bool BasariliKaydedildi = false;

                        int hataSayaci = 0;

                        while (hataSayaci < 3)
                        {
                            try
                            {
                                object SalakObjeV2 = System.Reflection.Missing.Value;
                                CalismaSayfasiV2 = (Excel2.Worksheet)CalismaKitabiV2.ActiveSheet;

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

                                if (lastV2.Row > lastcell2.Row)
                                {
                                    CalismaKitabi.Close(false);

                                    CalismaKitabi = workbooks.Open(Path.Combine(Application.StartupPath, "EgitimSablon.xlsx"));
                                    CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                                    HafizadanAtilacaklar.AddRange(new List<object> { CalismaKitabi, CalismaSayfasi });

                                    continue;
                                }

                                var tumAlan = CalismaSayfasi.Range["A:L"];

                                tumAlan.NumberFormat = "@";
                                tumAlan.Font.Size = 12;
                                tumAlan.Font.Name = "Times New Roman";
                                var tumAlanBorders = tumAlan.Borders;

                                tumAlanBorders.LineStyle = Excel2.XlLineStyle.xlLineStyleNone;

                                HafizadanAtilacaklar.AddRange(new List<object> { tumAlan, tumAlanBorders });

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

                        excelprocessid = Metodlar.GetExcelProcessId(Excelim);
                        excelprocessid2 = Metodlar.GetExcelProcessId(ExcelimV2);

                        try
                        {
                            if (BasariliKaydedildi)
                            {
                                if (File.Exists(YeniPath)) File.Delete(YeniPath);
                                var eski = Path.Combine(isyeripath, "Eğitim Belgesi Verilecekler.xlsx");
                                if (File.Exists(eski)) File.Delete(eski);


                                CalismaKitabi.SaveAs(YeniPath);

                            }
                            else
                            {
                                result = false;
                            }

                        }
                        catch (Exception ex)
                        {
                            Metodlar.HataMesajiGoster(ex, "Eğitim belgesi verilecekler dosyası kaydedilirken hata oluştu.");

                            result = false;
                        }

                    }
                    catch (Exception ex)
                    {
                        string Mesaj = "Eğitim belgesi verilecekler dosyası hata nedeniyle kaydedilemedi" + Environment.NewLine;

                        HataMesajiGoster(ex, Mesaj);

                        result = false;
                    }
                    finally
                    {
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

                try
                {
                    List<int> oldwords = Metodlar.GetProcessIdsSnapshot("WINWORD");

                    var wordApp = new Microsoft.Office.Interop.Word.Application { Visible = false };

                    wordApp.Visible = false;

                    List<int> newwords = Metodlar.GetProcessIdsSnapshot("WINWORD");

                    var wordprocessid = Metodlar.GetProcessId(oldwords, newwords);

                    object fileName = Path.Combine(System.Windows.Forms.Application.StartupPath, "EgitimBelgesiDilekce.docx");

                    Microsoft.Office.Interop.Word.Document aDoc = wordApp.Documents.Open(fileName, Visible: false, ReadOnly: true);

                    aDoc.Activate();

                    Metodlar.FindAndReplace(wordApp, "(SGM)", isyeri.SosyalGuvenlikKurumu.ToUpper());

                    Metodlar.FindAndReplace(wordApp, "(SICILNO)", isyeri.IsyeriSicilNo.ToUpper());

                    Metodlar.FindAndReplace(wordApp, "(FIRMAADI)", isyeri.Sirketler.SirketAdi);

                    aDoc.SaveAs(Path.Combine(isyeripath, String.Format("{0} 6111 Teşviki Eğitim Belgesi Tanımlama Dilekçesi.docx", isyeri.SubeAdi)));

                    aDoc.Close(false);

                    Metodlar.KillProcessById(wordprocessid);

                }
                catch { }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                EgitimListesiKaydediliyor = false;
            }




            return result ? YeniPath : null;
        }


    }



}
