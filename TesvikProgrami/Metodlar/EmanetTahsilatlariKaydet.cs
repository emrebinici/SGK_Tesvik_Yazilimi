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
     
        public static bool EmanetKaydediliyor=false;
        public static string EmanetTahsilatlariKaydet(Isyerleri isyeri, List<Classes.BankaIsverenEmanetTahsilat> bankaIsverenEmanetTahsilatlari, List<Classes.MosipEmanetTahsilat> mosipEmanetTahsilatlari)
        {
            while (EmanetKaydediliyor)
            {
                System.Threading.Thread.Sleep(500);
            }

            EmanetKaydediliyor = true;

            try
            {
                var isyeripath = Metodlar.IsyeriKlasorBul(isyeri, true);

                string YeniPath = Path.Combine(isyeripath, String.Format("Emanet {0}.xlsx", DateTime.Today.ToString("dd.MM.yyyy")));

                List<object> HafizadanAtilacaklar = new List<object>();

                try
                {
                    Excel2.Application Excelim;
                    Excel2.Workbook CalismaKitabi;
                    Excel2.Worksheet CalismaSayfasi;

                    bool result = false;

                    var ds = Metodlar.ReadExcelFile(Path.Combine(Application.StartupPath, "SablonEmanet.xlsx"), MesajGostersin: false);
                    var dtBankaIsverenEmanetTahsilatlari = ds.Tables[0];
                    dtBankaIsverenEmanetTahsilatlari.Columns.RemoveAt(4);
                    dtBankaIsverenEmanetTahsilatlari.Columns.Add("Tahsilat Tutarı", typeof(decimal));

                    foreach (var item in bankaIsverenEmanetTahsilatlari)
                    {
                        var newRow = dtBankaIsverenEmanetTahsilatlari.NewRow();
                        newRow[(int)Enums.BankaIsverenEmanetHucreBilgileri.TahsilatTarihi] = item.TahsilatTarihi;
                        newRow[(int)Enums.BankaIsverenEmanetHucreBilgileri.DonemYil] = item.DonemYil;
                        newRow[(int)Enums.BankaIsverenEmanetHucreBilgileri.DonemAy] = item.DonemAy;
                        newRow[(int)Enums.BankaIsverenEmanetHucreBilgileri.BorcTuru] = item.BorcTuru;
                        newRow[(int)Enums.BankaIsverenEmanetHucreBilgileri.TahsilatTutar] = Convert.ToDecimal(item.TahsilatTutar);

                        dtBankaIsverenEmanetTahsilatlari.Rows.Add(newRow);

                    }

                    var dtMosipEmanetTahsilatlari = new DataTable("Mosip");

                    dtMosipEmanetTahsilatlari.Columns.Clear();
                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 1) dtMosipEmanetTahsilatlari.Columns.Add("col" + i, typeof(decimal));
                        else dtMosipEmanetTahsilatlari.Columns.Add("col" + i, typeof(string));
                    }


                    foreach (var kayit in mosipEmanetTahsilatlari)
                    {
                        var newRow = dtMosipEmanetTahsilatlari.NewRow();
                        newRow[(int)Enums.MosipEmanetHucreBilgileri.BankayaYatirilmaTarihi] = kayit.BankayaYatirilmaTarihi;
                        newRow[(int)Enums.MosipEmanetHucreBilgileri.EmanettekiTahsilatTutari] = Convert.ToDecimal(kayit.EmanettekiTahsilatTutari);
                        newRow[(int)Enums.MosipEmanetHucreBilgileri.TahsilatTuru] = kayit.TahsilatTuru;


                        dtMosipEmanetTahsilatlari.Rows.Add(newRow);

                    }

                    if (dtBankaIsverenEmanetTahsilatlari.Rows.Count > 0 || dtMosipEmanetTahsilatlari.Rows.Count > 0)
                    {
                        Excelim = new Excel2.Application();

                        Excelim.Visible = false;
                        Excelim.DisplayAlerts = false;

                        object SalakObje = System.Reflection.Missing.Value;

                        var workbooks = Excelim.Workbooks;

                        CalismaKitabi = workbooks.Open(Path.Combine(Application.StartupPath, "SablonEmanet.xlsx"));

                        var sheets = CalismaKitabi.Sheets;

                        HafizadanAtilacaklar.AddRange(new List<object> { workbooks, CalismaKitabi, sheets });

                        var ExcelimV2 = new Excel2.Application();

                        ExcelimV2.Visible = false;
                        ExcelimV2.DisplayAlerts = false;

                        var workbooks2 = ExcelimV2.Workbooks;

                        var GenelToplamHucreleri = new List<string>();

                        #region Isveren Banka Kaydet 
                        if (dtBankaIsverenEmanetTahsilatlari.Rows.Count > 0)
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

                            wb.Worksheets.Add(dtBankaIsverenEmanetTahsilatlari);

                            string geciciDosyaYolu = YeniPath.Insert(YeniPath.IndexOf(".xlsx"), "_GEÇİCİ_BankaIsveren");

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

                                    //tumAlan.NumberFormat = "@";

                                    var colTutar = CalismaSayfasi.Range["E:E"];

                                    //colTutar.NumberFormat = "#.##0,00";

                                    var rangebaslangic = CalismaSayfasi.Cells[1, 1];

                                    var rangebitis = CalismaSayfasi.Cells[1, 5];

                                    Excel2.Range rangeLabel = CalismaSayfasi.Range[rangebaslangic, rangebitis];
                                    rangeLabel.Merge();
                                    var fontLabel = rangeLabel.Font;
                                    fontLabel.Bold = true;
                                    rangeLabel.Value2 = "BANKA İŞVEREN EMANET TAHSİLATLARI";

                                    var rangeTahsilatTarihi = CalismaSayfasi.Cells[2, 1] as Excel2.Range;
                                    var rangeDonemYil = CalismaSayfasi.Cells[2, 2] as Excel2.Range;
                                    var rangeDonemAy = CalismaSayfasi.Cells[2, 3] as Excel2.Range;
                                    var rangeBorcTuru = CalismaSayfasi.Cells[2, 4] as Excel2.Range;
                                    var rangeTahsilatTutari = CalismaSayfasi.Cells[2, 5] as Excel2.Range;

                                    rangeTahsilatTarihi.Value2 = "Tahsilat Tarihi";
                                    rangeDonemYil.Value2 = "Dönem Yıl";
                                    rangeDonemAy.Value2 = "Dönem Ay";
                                    rangeBorcTuru.Value2 = "Borç Türü";
                                    rangeTahsilatTutari.Value2 = "TahsilatTutar";

                                    var rangeheader = CalismaSayfasi.Range[rangeTahsilatTarihi, rangeTahsilatTutari] as Excel2.Range;

                                    var borders = rangeheader.Borders;

                                    var fontHeader = rangeheader.Font;
                                    fontHeader.Bold = true;

                                    borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                    Excel2.Range rangeV2 = allcells[3, 1] as Excel2.Range;
                                    CalismaSayfasi.Activate();

                                    rangeV2.Select();
                                    CalismaSayfasi.Paste(Type.Missing, Type.Missing);

                                    var usedrange3 = CalismaSayfasi.UsedRange;

                                    Excel2.Range lastcell2 = usedrange3.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                                    HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasiV2, usedrange4, lastcell, usedrange2, lastV2, tumAlanV2, tumAlan, colTutar, rangebaslangic, rangebitis, rangeLabel, fontLabel, rangeTahsilatTarihi, rangeDonemYil, rangeDonemAy, rangeBorcTuru, rangeTahsilatTutari, rangeheader, borders, fontHeader, rangeV2, usedrange3, lastcell2 });

                                    if (lastcell.Row + lastV2.Row - 1 > lastcell2.Row)
                                    {
                                        CalismaKitabi.Close(false);

                                        CalismaKitabi = workbooks.Open(Path.Combine(Application.StartupPath, "SablonEmanet.xlsx"));
                                        CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                                        HafizadanAtilacaklar.AddRange(new List<object> { CalismaKitabi, CalismaSayfasi });

                                        continue;
                                    }

                                    var rangeToplam = CalismaSayfasi.Cells[lastcell2.Row, 5] as Excel2.Range;
                                    rangeToplam.Formula = String.Format("=SUM(E3:E{0}", (lastcell2.Row - 1).ToString());
                                    GenelToplamHucreleri.Add(String.Format("E{0}", lastcell2.Row));

                                    var rangebas = CalismaSayfasi.Cells[3, 5] as Excel2.Range;
                                    var rangebit = CalismaSayfasi.Cells[lastcell2.Row, 5] as Excel2.Range;
                                    var rngtutar = CalismaSayfasi.Range[rangebas, rangebit] as Excel2.Range;
                                    rngtutar.NumberFormat = "#,##0.00";

                                    var tumAlan3 = CalismaSayfasi.Range["A:X"];

                                    var font = tumAlan3.Font;

                                    font.Size = 12;
                                    font.Name = "Times New Roman";

                                    HafizadanAtilacaklar.AddRange(new List<object> { rangeToplam, rangebas, rangebit, rngtutar, tumAlan3, font });

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
                        #endregion
                        #region MosipKaydet

                        if (dtMosipEmanetTahsilatlari.Rows.Count > 0)
                        {
                            CalismaSayfasi = (Excel2.Worksheet)sheets[1];

                            var usedrange = CalismaSayfasi.UsedRange;

                            var allcells = CalismaSayfasi.Cells;

                            Excel2.Range lastrow = usedrange.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);
                            Excel2.Range lastrow2 = null;

                            if (bankaIsverenEmanetTahsilatlari.Count == 0)
                            {
                                if (lastrow.Row > 1)
                                {
                                    Excel2.Range range = CalismaSayfasi.get_Range("A2", lastrow);

                                    var entirerow = range.EntireRow;

                                    entirerow.Delete(Excel2.XlDeleteShiftDirection.xlShiftUp);

                                    HafizadanAtilacaklar.AddRange(new List<object> { range, entirerow });
                                }
                                lastrow2 = allcells[2, 1];
                            }
                            else lastrow2 = allcells[lastrow.Row + 3, lastrow.Column];


                            HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasi, usedrange, allcells, lastrow, lastrow2 });

                            ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook();

                            wb.Worksheets.Add(dtMosipEmanetTahsilatlari);

                            string geciciDosyaYolu = YeniPath.Insert(YeniPath.IndexOf(".xlsx"), "_GEÇİCİ_Mosip");

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

                                    var tumAlan = CalismaSayfasi.Range["A:X"];

                                    //tumAlan.NumberFormat = "@";

                                    var TahsilatTutarColumn = CalismaSayfasi.Range["E:E"];

                                    //TahsilatTutarColumn.NumberFormat = "#.##0,00";

                                    var MosipTutar = CalismaSayfasi.Range["B:B"];


                                    //MosipTutar.NumberFormat = "#.##0,00";

                                    var rangebaslangic = CalismaSayfasi.Cells[lastrow2.Row - 1, 1];

                                    var rangebitis = CalismaSayfasi.Cells[lastrow2.Row - 1, 5];

                                    Excel2.Range rangeLabel = CalismaSayfasi.Range[rangebaslangic, rangebitis];
                                    rangeLabel.Merge();
                                    var fontLabel = rangeLabel.Font;
                                    fontLabel.Bold = true;
                                    rangeLabel.Value2 = "MOSİP EMANET TAHSİLATLARI";

                                    var rowNum = bankaIsverenEmanetTahsilatlari.Count == 0 ? 2 : lastrow2.Row;

                                    var rangeBankayaYatirilmaTarihi = CalismaSayfasi.Cells[rowNum, 1] as Excel2.Range;
                                    var rangeEmanettekiTahsilatTutari = CalismaSayfasi.Cells[rowNum, 2] as Excel2.Range;
                                    var rangeTahsilatTuru = CalismaSayfasi.Cells[rowNum, 3] as Excel2.Range;


                                    rangeBankayaYatirilmaTarihi.Value2 = "Bankaya Yatırılma Tarihi";
                                    rangeEmanettekiTahsilatTutari.Value2 = "Emanetteki Tahsilat Tutarı";
                                    rangeTahsilatTuru.Value2 = "Tahsilat Türü";

                                    var rangeheader = CalismaSayfasi.Range[rangeBankayaYatirilmaTarihi, rangeTahsilatTuru] as Excel2.Range;

                                    var borders = rangeheader.Borders;

                                    var fontHeader = rangeheader.Font;
                                    fontHeader.Bold = true;

                                    borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                    var allcells2 = CalismaSayfasi.Cells;

                                    Excel2.Range rangeV2 = allcells2[lastrow2.Row + 1, 1] as Excel2.Range;
                                    CalismaSayfasi.Activate();

                                    rangeV2.Select();
                                    CalismaSayfasi.Paste(Type.Missing, Type.Missing);

                                    var usedrange3 = CalismaSayfasi.UsedRange;

                                    Excel2.Range lastcell2 = usedrange3.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                                    HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasiV2, usedrange4, lastcell, usedrange2, lastV2, tumAlanV2, tumAlan, TahsilatTutarColumn, MosipTutar, rangebaslangic, rangebitis, rangeLabel, fontLabel, rangeBankayaYatirilmaTarihi, rangeEmanettekiTahsilatTutari, rangeTahsilatTuru, rangeheader, fontHeader, borders, allcells2, rangeV2, usedrange3, lastcell2 });

                                    if (lastcell.Row + lastV2.Row + (bankaIsverenEmanetTahsilatlari.Count == 0 ? 0 : 2) > lastcell2.Row)
                                    {
                                        CalismaKitabi.Close(false);

                                        CalismaKitabi = workbooks.Open(Path.Combine(Application.StartupPath, "SablonEmanet.xlsx"));

                                        var sayfalar = CalismaKitabi.Sheets;

                                        CalismaSayfasi = sayfalar[1];

                                        HafizadanAtilacaklar.AddRange(new List<object> { CalismaKitabi, sayfalar, CalismaSayfasi });

                                        continue;
                                    }



                                    var tutarcells = MosipTutar.Cells;
                                    tutarcells.HorizontalAlignment = Excel2.XlHAlign.xlHAlignLeft;

                                    var rangebas = CalismaSayfasi.Cells[lastrow2.Row + 1, 2] as Excel2.Range;
                                    var rangebit = CalismaSayfasi.Cells[lastrow2.Row + 1 + dtMosipEmanetTahsilatlari.Rows.Count, 2] as Excel2.Range;
                                    var rngtutar = CalismaSayfasi.Range[rangebas, rangebit] as Excel2.Range;
                                    rngtutar.NumberFormat = "#,##0.00";

                                    var rangeToplam = CalismaSayfasi.Cells[lastcell2.Row, 2] as Excel2.Range;
                                    rangeToplam.Formula = String.Format("=SUM(B{0}:B{1})", lastrow2.Row + 1, lastrow2.Row + dtMosipEmanetTahsilatlari.Rows.Count - 1);
                                    GenelToplamHucreleri.Add(String.Format("B{0}", lastcell2.Row));

                                    var tumAlan3 = CalismaSayfasi.Range["A:X"];

                                    var font = tumAlan3.Font;

                                    font.Size = 12;
                                    font.Name = "Times New Roman";

                                    HafizadanAtilacaklar.AddRange(new List<object> { tutarcells, rangebas, rangebit, rngtutar, rangeToplam, tumAlan3, font, });

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
                        #endregion


                        {
                            CalismaSayfasi = sheets[1];
                            var usedrange = CalismaSayfasi.UsedRange;

                            Excel2.Range lastcell = usedrange.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                            var cellToplamLabel = CalismaSayfasi.Cells[lastcell.Row + 4, 1] as Excel2.Range;
                            cellToplamLabel.Value2 = "Genel Toplam (Mosip + İşveren Banka)";

                            var cellToplam = CalismaSayfasi.Cells[lastcell.Row + 4, 2] as Excel2.Range;
                            cellToplam.Formula = String.Format("=SUM({0})", String.Join(",", GenelToplamHucreleri));
                            cellToplam.NumberFormat = "#,##0.00";


                            //var lastisveren = bankaIsverenEmanetTahsilatlari.LastOrDefault();
                            //var lastmosip = mosipEmanetTahsilatlari.LastOrDefault();

                            //var toplamTutar = (lastisveren != null ? Convert.ToDecimal(lastisveren.TahsilatTutar) : 0m) + (lastmosip != null ? Convert.ToDecimal(lastmosip.EmanettekiTahsilatTutari) : 0m);

                            //cellToplam.Value2 = toplamTutar;

                            var font = cellToplamLabel.Font;
                            font.Bold = true;

                            var font2 = cellToplam.Font;
                            font2.Bold = true;

                            HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasi, usedrange, lastcell, cellToplamLabel, cellToplam, font, font2 });
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

                            var files = Directory.GetFiles(path, "Emanet*.xlsx");

                            foreach (var file in files)
                            {
                                File.Delete(file);
                            }

                            CalismaKitabi.SaveAs(YeniPath);

                            result = true;
                        }
                        catch (Exception ex)
                        {
                            Metodlar.HataMesajiGoster(ex, "Emanet Tahsilatları kaydedilirken hata oluştu.");

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
                    string Mesaj = "Emanet Tahsilatlarılistesi hata nedeniyle kaydedilemedi" + Environment.NewLine;

                    HataMesajiGoster(ex, Mesaj);
                }
            }
            catch (Exception)
            {
                EmanetKaydediliyor = false;

                throw;
            }
            finally
            {
                EmanetKaydediliyor = false;
            }

            EmanetKaydediliyor = false;

            return null;
        }


    }



}
