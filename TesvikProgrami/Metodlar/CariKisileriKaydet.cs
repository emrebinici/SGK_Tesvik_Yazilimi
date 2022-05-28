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
        public static string CariKisileriKaydet(Isyerleri isyeri, Classes.CariKisiler cariKisiler)
        {

            var isyeripath = Metodlar.IsyeriKlasorBul(isyeri, true);

            var tarih = Convert.ToDateTime(cariKisiler.SorgulananDonem);

            string YeniPath = Path.Combine(isyeripath, String.Format("Kişiler-{0}-{1}.xlsx", tarih.Year, tarih.Month));

            List<object> HafizadanAtilacaklar = new List<object>();

            try
            {
                Excel2.Application Excelim;
                Excel2.Workbook CalismaKitabi;
                Excel2.Worksheet CalismaSayfasi;

                bool result = false;

                var ds = Metodlar.ReadExcelFile(Path.Combine(Application.StartupPath, "SablonKisiler.xlsx"), MesajGostersin: false);
                var dtKisiler = ds.Tables[0];
                var dtIseGirisCikislar = ds.Tables[1];

                foreach (var kisi in cariKisiler.Kisiler)
                {
                    var newRow = dtKisiler.NewRow();
                    newRow[(int)Enums.CariKisilerHucreBilgileri.TcKimlikNoSosyalGuvenlikNo] = kisi.TcKimlikNo;
                    newRow[(int)Enums.CariKisilerHucreBilgileri.Ad] = kisi.Ad;
                    newRow[(int)Enums.CariKisilerHucreBilgileri.Soyad] = kisi.Soyad;
                    newRow[(int)Enums.CariKisilerHucreBilgileri.IlkSoyadi] = kisi.Ilk_Soyad;
                    newRow[(int)Enums.CariKisilerHucreBilgileri.Kanun] = kisi.Kanun;
                    newRow[(int)Enums.CariKisilerHucreBilgileri.BelgeTuru] = kisi.BelgeTuru;
                    newRow[(int)Enums.CariKisilerHucreBilgileri.Gun] = kisi.Gun;
                    newRow[(int)Enums.CariKisilerHucreBilgileri.GunlukOrtalamaUcret] = kisi.GunlukOrtalamaUcret;
                    newRow[(int)Enums.CariKisilerHucreBilgileri.MeslekKod] = kisi.MeslekKod;
                    newRow[(int)Enums.CariKisilerHucreBilgileri.Araci] = kisi.Araci;
                    newRow[(int)Enums.CariKisilerHucreBilgileri.CikisAyi] = kisi.CikisAyi > DateTime.MinValue ? kisi.CikisAyi.ToString("yyyy/MM") : string.Empty;

                    dtKisiler.Rows.Add(newRow);

                }

                foreach (var kayit in cariKisiler.IseGirisCikisKayitlari)
                {
                    var newRow = dtIseGirisCikislar.NewRow();
                    newRow[(int)Enums.CariKisilerIseGirisCikisHucreBilgileri.TcKimlikNo] = kayit.TcKimlikNo;
                    newRow[(int)Enums.CariKisilerIseGirisCikisHucreBilgileri.AdSoyad] = kayit.AdSoyad;
                    newRow[(int)Enums.CariKisilerIseGirisCikisHucreBilgileri.Turu] = kayit.Turu;
                    newRow[(int)Enums.CariKisilerIseGirisCikisHucreBilgileri.Tarih] = kayit.Tarih.ToString("dd.MM.yyyy");
                    newRow[(int)Enums.CariKisilerIseGirisCikisHucreBilgileri.IslemTuru] = kayit.IslemTuru;
                    newRow[(int)Enums.CariKisilerIseGirisCikisHucreBilgileri.IslemSaati] = kayit.IslemSaati.ToString("dd.MM.yyyy HH:mm");
                    newRow[(int)Enums.CariKisilerIseGirisCikisHucreBilgileri.Araci] = kayit.Araci;

                    dtIseGirisCikislar.Rows.Add(newRow);

                }

                if (dtKisiler.Rows.Count > 0 || dtIseGirisCikislar.Rows.Count > 0)
                {
                    Excelim = new Excel2.Application();

                    Excelim.Visible = false;
                    Excelim.DisplayAlerts = false;

                    object SalakObje = System.Reflection.Missing.Value;

                    var workbooks = Excelim.Workbooks;

                    CalismaKitabi = workbooks.Open(Path.Combine(Application.StartupPath, "SablonKisiler.xlsx"));

                    var sheets = CalismaKitabi.Sheets;

                    HafizadanAtilacaklar.AddRange(new List<object> { workbooks, CalismaKitabi, sheets });

                    var ExcelimV2 = new Excel2.Application();

                    ExcelimV2.Visible = false;
                    ExcelimV2.DisplayAlerts = false;

                    var workbooks2 = ExcelimV2.Workbooks;

                    if (dtKisiler.Rows.Count > 0)
                    {
                        int SayfaNo = 1;

                        CalismaSayfasi = (Excel2.Worksheet)sheets[SayfaNo];
                        CalismaSayfasi.Activate();

                        HafizadanAtilacaklar.Add(CalismaSayfasi);

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

                        wb.Worksheets.Add(dtKisiler);

                        string geciciDosyaYolu = YeniPath.Insert(YeniPath.IndexOf(".xlsx"), "_GEÇİCİ_Kisiler");

                        wb.SaveAs(geciciDosyaYolu);



                        //NewExportExcelV2 excelV2 = new NewExportExcelV2();
                        //excelV2.gridControl1.DataSource = DisDataGrid;
                        //string geciciDosyaYolu = filename.Insert(filename.IndexOf(".xlsx"), "_GEÇİCİ");
                        //excelV2.gridView1.ExportToXlsx(geciciDosyaYolu);


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

                                    CalismaKitabi = workbooks.Open(Path.Combine(Application.StartupPath, "SablonKisiler.xlsx"));
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

                        CalismaKitabiV2.Close(false);

                        if (File.Exists(geciciDosyaYolu))
                        {
                            File.Delete(geciciDosyaYolu);
                        }
                    }

                    if (dtIseGirisCikislar.Rows.Count > 0)
                    {
                        int SayfaNo = 2;

                        CalismaSayfasi = (Excel2.Worksheet)sheets[SayfaNo];
                        CalismaSayfasi.Activate();

                        HafizadanAtilacaklar.Add(CalismaSayfasi);

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

                        wb.Worksheets.Add(dtIseGirisCikislar);

                        string geciciDosyaYolu = YeniPath.Insert(YeniPath.IndexOf(".xlsx"), "_GEÇİCİ_IseGirisiCikislar");

                        wb.SaveAs(geciciDosyaYolu);



                        //NewExportExcelV2 excelV2 = new NewExportExcelV2();
                        //excelV2.gridControl1.DataSource = DisDataGrid;
                        //string geciciDosyaYolu = filename.Insert(filename.IndexOf(".xlsx"), "_GEÇİCİ");
                        //excelV2.gridView1.ExportToXlsx(geciciDosyaYolu);


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

                                    CalismaKitabi = workbooks.Open(Path.Combine(Application.StartupPath, "SablonKisiler.xlsx"));
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

                        CalismaKitabiV2.Close(false);

                        if (File.Exists(geciciDosyaYolu))
                        {
                            File.Delete(geciciDosyaYolu);
                        }
                    }

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
                        var path = Path.GetDirectoryName(YeniPath);

                        var files = Directory.GetFiles(path, "Kişiler-*.xlsx");

                        foreach (var file in files)
                        {
                            File.Delete(file);
                        }

                        CalismaKitabi.SaveAs(YeniPath);

                        result = true;
                    }
                    catch (Exception ex)
                    {
                        Metodlar.HataMesajiGoster(ex, "Cari Kişiler listesi kaydedilirken hata oluştu.");

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
                string Mesaj = "Cari Kişiler listesi hata nedeniyle kaydedilemedi" + Environment.NewLine;

                HataMesajiGoster(ex, Mesaj);
            }

            return null;
        }


    }


}
