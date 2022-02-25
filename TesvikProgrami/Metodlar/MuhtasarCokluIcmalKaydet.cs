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

        public static bool MuhtasarCokluIcmalKaydediliyor = false;
        public static string MuhtasarCokluIcmalKaydet(Dictionary<Isyerleri, Dictionary<string, decimal>> tumIsyerleriIcmal, string savepath)
        {
            while (MuhtasarCokluIcmalKaydediliyor)
            {
                System.Threading.Thread.Sleep(500);
            }

            MuhtasarCokluIcmalKaydediliyor = true;

            try
            {
                string YeniPath = savepath;

                List<object> HafizadanAtilacaklar = new List<object>();

                try
                {

                    if (tumIsyerleriIcmal.All(p => int.TryParse(p.Key.SubeAdi, out int temp)))
                    {
                        tumIsyerleriIcmal = tumIsyerleriIcmal.OrderBy(p => p.Key.SubeAdi.ToInt()).ToDictionary(x => x.Key, x => x.Value);
                    }
                    else tumIsyerleriIcmal = tumIsyerleriIcmal.OrderBy(p => p.Key.SubeAdi).ToDictionary(x => x.Key, x => x.Value);



                    Excel2.Application Excelim;
                    Excel2.Workbook CalismaKitabi;
                    Excel2.Worksheet CalismaSayfasi;

                    bool result = false;

                    DataTable dt = new DataTable("Muhtasar Çoklu İcmal");
                    dt.Columns.Add("Sira", typeof(int));
                    dt.Columns.Add("Sirket");
                    dt.Columns.Add("Sube");
                    dt.Columns.Add("IsyeriNo");
                    dt.Columns.Add("6111", typeof(decimal));
                    dt.Columns.Add("7103", typeof(decimal));
                    dt.Columns.Add("3294", typeof(decimal));

                    if (tumIsyerleriIcmal.Any(p => p.Value.ContainsKey("17256") && p.Value["17256"] > 0))
                    {
                        dt.Columns.Add("17256", typeof(decimal));
                    }

                    if (tumIsyerleriIcmal.Any(p => p.Value.ContainsKey("27256") && p.Value["27256"] > 0))
                    {
                        dt.Columns.Add("27256", typeof(decimal));
                    }

                    if (tumIsyerleriIcmal.Any(p => p.Value.ContainsKey("7316") && p.Value["7316"] > 0))
                    {
                        dt.Columns.Add("7316", typeof(decimal));
                    }

                    if (tumIsyerleriIcmal.Any(p => p.Value.ContainsKey("14857") && p.Value["14857"] > 0))
                    {
                        dt.Columns.Add("4857", typeof(decimal));
                    }

                    dt.Columns.Add("6645", typeof(decimal));
                    dt.Columns.Add("2828", typeof(decimal));

                    if (tumIsyerleriIcmal.Any(p => p.Value.ContainsKey("6486") && p.Value["6486"] > 0))
                    {
                        dt.Columns.Add("6486", typeof(decimal));
                    }

                    if (tumIsyerleriIcmal.Any(p => p.Value.ContainsKey("6322/25510") && p.Value["6322/25510"] > 0))
                    {
                        dt.Columns.Add("6322", typeof(decimal));
                    }

                    

                    dt.Columns.Add("Tumu", typeof(decimal));

                    int sirano = 1;

                    foreach (var item in tumIsyerleriIcmal)
                    {
                        var isyeri = item.Key;
                        var icmaller = item.Value;

                        var row = dt.NewRow();

                        row[0] = sirano++;
                        row[1] = isyeri.Sirketler.SirketAdi;
                        row[2] = isyeri.SubeAdi;
                        row[3] = isyeri.IsyeriSicilNo.Substring(9, 7);
                        row[4] = icmaller["6111"];
                        row[5] = icmaller["7103"];
                        row[6] = icmaller["3294"];

                        if (icmaller.ContainsKey("17256") && icmaller["17256"] > 0)
                        {
                            row["17256"] = icmaller["17256"];
                        }
                        else
                        {
                            if (dt.Columns.Contains("17256"))
                            {
                                row["17256"] = 0m;
                            }
                        }

                        if (icmaller.ContainsKey("27256") && icmaller["27256"] > 0)
                        {
                            row["27256"] = icmaller["27256"];
                        }
                        else
                        {
                            if (dt.Columns.Contains("27256"))
                            {
                                row["27256"] = 0m;
                            }
                        }


                        if (icmaller.ContainsKey("7316") && icmaller["7316"] > 0)
                        {
                            row["7316"] = icmaller["7316"];
                        }
                        else
                        {
                            if (dt.Columns.Contains("7316"))
                            {
                                row["7316"] = 0m;
                            }
                        }

                        if (icmaller.ContainsKey("14857") && icmaller["14857"] > 0)
                        {
                            row["4857"] = icmaller["14857"];
                        }
                        else
                        {
                            if (dt.Columns.Contains("4857"))
                            {
                                row["4857"] = 0m;
                            }
                        }

                        
                        row["6645"] = icmaller["6645"];
                        row["2828"] = icmaller["2828"];

                        if (icmaller.ContainsKey("6486") && icmaller["6486"] > 0)
                        {
                            row["6486"] = icmaller["6486"];
                        }
                        else
                        {
                            if (dt.Columns.Contains("6486"))
                            {
                                row["6486"] = 0m;
                            }
                        }

                        if (icmaller.ContainsKey("6322/25510") && icmaller["6322/25510"] > 0)
                        {
                            row["6322"] = icmaller["6322/25510"];
                        }
                        else
                        {
                            if (dt.Columns.Contains("6322"))
                            {
                                row["6322"] = 0m;
                            }
                        }

                        row["Tumu"] = icmaller["Tumu"];

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

                        wb.Worksheets.Add(dt);

                        string geciciDosyaYolu = YeniPath.Insert(YeniPath.IndexOf(".xlsx"), "_GEÇİCİ_MuhtasarCokluIcmal");

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

                                var rangeSira= CalismaSayfasi.Cells[1, 1] as Excel2.Range;
                                var rangeSirket= CalismaSayfasi.Cells[1, 2] as Excel2.Range;
                                var rangeSubeAdi = CalismaSayfasi.Cells[1, 3] as Excel2.Range;
                                var rangeIsyeriNo = CalismaSayfasi.Cells[1, 4] as Excel2.Range;
                                var range6111 = CalismaSayfasi.Cells[1, 5] as Excel2.Range;
                                var range7103 = CalismaSayfasi.Cells[1, 6] as Excel2.Range;
                                var range3294 = CalismaSayfasi.Cells[1, 7] as Excel2.Range;

                                var lastcolNum = 8;

                                Excel2.Range range17256 = null;

                                if (dt.Columns.Contains("17256"))
                                {
                                    range17256 = CalismaSayfasi.Cells[1, lastcolNum++] as Excel2.Range;
                                }

                                Excel2.Range range27256 = null;

                                if (dt.Columns.Contains("27256"))
                                {
                                    range27256 = CalismaSayfasi.Cells[1, lastcolNum++] as Excel2.Range;
                                }

                                Excel2.Range range7316 = null;

                                if (dt.Columns.Contains("7316"))
                                {
                                    range7316 = CalismaSayfasi.Cells[1, lastcolNum++] as Excel2.Range;
                                }

                                Excel2.Range range4857 = null;

                                if (dt.Columns.Contains("4857"))
                                {
                                    range4857 = CalismaSayfasi.Cells[1, lastcolNum++] as Excel2.Range;
                                }

                                var range6645 = CalismaSayfasi.Cells[1, lastcolNum++] as Excel2.Range;
                                var range2828 = CalismaSayfasi.Cells[1, lastcolNum++] as Excel2.Range;


                                Excel2.Range range6486 = null;
                                if (dt.Columns.Contains("6486"))
                                { 
                                    range6486 = CalismaSayfasi.Cells[1, lastcolNum++] as Excel2.Range;
                                }

                                Excel2.Range range6322 = null;

                                if (dt.Columns.Contains("6322"))
                                {
                                    range6322 = CalismaSayfasi.Cells[1, lastcolNum++] as Excel2.Range;
                                }

                                var rangeTumu = CalismaSayfasi.Cells[1, lastcolNum] as Excel2.Range;


                                rangeSirket.Value2 = "Şirket";
                                rangeSubeAdi.Value2 = "Şube";
                                rangeIsyeriNo.Value2 = "İşyeri No";
                                range6111.Value2 = "6111 teşviki iade tutarı";
                                range7103.Value2 = "7103 teşviki iade tutarı";
                                range3294.Value2 = "3294 teşviki iade tutarı";
                                if (range17256 != null)  range17256.Value2 = "17256 teşviki iade tutarı";
                                if (range27256 != null)  range27256.Value2 = "27256 teşviki iade tutarı";
                                if (range7316 != null)  range7316.Value2 = "7316 teşviki iade tutarı";
                                if (range4857 != null)  range4857.Value2 = "4857 teşviki iade tutarı";
                                range6645.Value2 = "6645 teşviki iade tutarı";
                                range2828.Value2 = "2828 teşviki iade tutarı";
                                if (range6486 != null) range6486.Value2 = "6486 teşviki iade tutarı";
                                if (range6322 != null) range6322.Value2 = "6322/25510 teşviki iade tutarı";
                                rangeTumu.Value2 = "Toplam teşvik iade tutarı";

                                var rangeSiraColumn = rangeSira.EntireColumn;
                                rangeSiraColumn.ColumnWidth = 3;

                                var rangeSirketColumn = rangeSirket.EntireColumn;
                                rangeSirketColumn.ColumnWidth = 50;

                                var rangeSubeAdiColumn = rangeSubeAdi.EntireColumn;
                                rangeSubeAdiColumn.ColumnWidth = 15;
                                rangeSubeAdiColumn.Style.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var rangeIsyeriNoColumn = rangeIsyeriNo.EntireColumn;
                                rangeIsyeriNoColumn.ColumnWidth = 15;
                                rangeIsyeriNoColumn.Style.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var rangeTutarColumns = (Excel2.Range)CalismaSayfasi.Columns["E:L"];
                                rangeTutarColumns.ColumnWidth = 12;

                                var ilkSatir = range6111.EntireRow;
                                ilkSatir.RowHeight = 50;


                                var rangeheader = CalismaSayfasi.Range[rangeSira, rangeTumu] as Excel2.Range;

                                rangeheader.Style.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;
                                rangeheader.Style.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                range6111.WrapText = true;
                                range7103.WrapText = true;
                                range3294.WrapText = true;
                                if (range17256 != null) range17256.WrapText = true;
                                if (range27256 != null) range27256.WrapText = true;
                                if (range7316 != null) range7316.WrapText = true;
                                if (range4857 != null) range4857.WrapText = true;
                                range6645.WrapText = true;
                                range2828.WrapText = true;
                                rangeTumu.WrapText = true;
                                if (range6486 != null) range6486.WrapText = true;
                                if (range6322 != null) range6322.WrapText = true;
                                

                                var borders = rangeheader.Borders;

                                var fontHeader = rangeheader.Font;
                                fontHeader.Bold = true;

                                borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                Excel2.Range rangeV2 = allcells[2, 1] as Excel2.Range;
                                CalismaSayfasi.Activate();

                                rangeV2.Select();
                                CalismaSayfasi.Paste(Type.Missing, Type.Missing);

                                var usedrange3 = CalismaSayfasi.UsedRange;

                                Excel2.Range lastcell2 = usedrange3.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                                HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasiV2, usedrange4, lastcell, usedrange2, lastV2, tumAlanV2, rangeSira,rangeSirket,rangeSubeAdi, rangeIsyeriNo, range6111, range7103,range17256,range27256, range7316, range3294, range4857,range6645,range2828,range6486,range6322,rangeTumu,rangeSiraColumn,rangeSirketColumn ,ilkSatir, rangeSubeAdiColumn,rangeIsyeriNoColumn,rangeTutarColumns, rangeheader, borders, fontHeader, rangeV2, usedrange3, lastcell2 });

                                if (lastcell.Row + lastV2.Row - 1 > lastcell2.Row)
                                {
                                    CalismaKitabi.Close(false);

                                    CalismaKitabi = workbooks.Add();
                                    CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                                    HafizadanAtilacaklar.AddRange(new List<object> { CalismaKitabi, CalismaSayfasi });

                                    continue;
                                }

                                var rangeGenelToplamLabel = CalismaSayfasi.Cells[lastcell2.Row+1, 4] as Excel2.Range;
                                rangeGenelToplamLabel.Value2 = "Genel Toplam :";

                                var rangeToplam6111 = CalismaSayfasi.Cells[lastcell2.Row + 1, 5] as Excel2.Range;
                                rangeToplam6111.Formula = String.Format("=SUM(E2:E{0}", (lastcell2.Row ).ToString());
                                GenelToplamHucreleri.Add(String.Format("E{0}", lastcell2.Row + 1));

                                var rangeToplam7103 = CalismaSayfasi.Cells[lastcell2.Row + 1, 6] as Excel2.Range;
                                rangeToplam7103.Formula = String.Format("=SUM(F2:F{0}", (lastcell2.Row).ToString());
                                GenelToplamHucreleri.Add(String.Format("F{0}", lastcell2.Row + 1));

                                var rangeToplam3294 = CalismaSayfasi.Cells[lastcell2.Row + 1, 7] as Excel2.Range;
                                rangeToplam3294.Formula = String.Format("=SUM(G2:G{0}", (lastcell2.Row).ToString());
                                GenelToplamHucreleri.Add(String.Format("G{0}", lastcell2.Row + 1));

                                lastcolNum = 8;
                                var lastLetterAsciCode = (int)'H';
                                var lastLetter = "H";

                                Excel2.Range rangeToplam17256 = null;
                                if (dt.Columns.Contains("17256"))
                                {
                                    rangeToplam17256 = CalismaSayfasi.Cells[lastcell2.Row + 1, lastcolNum++] as Excel2.Range;
                                    rangeToplam17256.Formula = String.Format("=SUM({0}2:{1}{2}", lastLetter, lastLetter, (lastcell2.Row).ToString());
                                    GenelToplamHucreleri.Add(String.Format("{0}{1}", lastLetter, lastcell2.Row + 1));

                                    lastLetter = Convert.ToChar(++lastLetterAsciCode).ToString();
                                }

                                Excel2.Range rangeToplam27256 = null;
                                if (dt.Columns.Contains("27256"))
                                {
                                    rangeToplam27256 = CalismaSayfasi.Cells[lastcell2.Row + 1, lastcolNum++] as Excel2.Range;
                                    rangeToplam27256.Formula = String.Format("=SUM({0}2:{1}{2}", lastLetter, lastLetter, (lastcell2.Row).ToString());
                                    GenelToplamHucreleri.Add(String.Format("{0}{1}", lastLetter, lastcell2.Row + 1));

                                    lastLetter = Convert.ToChar(++lastLetterAsciCode).ToString();
                                }

                                Excel2.Range rangeToplam7316 = null;
                                if (dt.Columns.Contains("7316"))
                                {
                                    rangeToplam7316 = CalismaSayfasi.Cells[lastcell2.Row + 1, lastcolNum++] as Excel2.Range;
                                    rangeToplam7316.Formula = String.Format("=SUM({0}2:{1}{2}", lastLetter, lastLetter, (lastcell2.Row).ToString());
                                    GenelToplamHucreleri.Add(String.Format("{0}{1}", lastLetter, lastcell2.Row + 1));

                                    lastLetter = Convert.ToChar(++lastLetterAsciCode).ToString();
                                }

                                Excel2.Range rangeToplam4857 = null;
                                if (dt.Columns.Contains("4857"))
                                {
                                    rangeToplam4857 = CalismaSayfasi.Cells[lastcell2.Row + 1, lastcolNum++] as Excel2.Range;
                                    rangeToplam4857.Formula = String.Format("=SUM({0}2:{1}{2}", lastLetter, lastLetter, (lastcell2.Row).ToString());
                                    GenelToplamHucreleri.Add(String.Format("{0}{1}", lastLetter, lastcell2.Row + 1));

                                    lastLetter = Convert.ToChar(++lastLetterAsciCode).ToString();
                                }

                                var rangeToplam6645 = CalismaSayfasi.Cells[lastcell2.Row + 1, lastcolNum++] as Excel2.Range;
                                rangeToplam6645.Formula = String.Format("=SUM({0}2:{1}{2}", lastLetter, lastLetter, (lastcell2.Row).ToString());
                                GenelToplamHucreleri.Add(String.Format("{0}{1}", lastLetter, lastcell2.Row + 1));

                                lastLetter = Convert.ToChar(++lastLetterAsciCode).ToString();

                                var rangeToplam2828= CalismaSayfasi.Cells[lastcell2.Row + 1, lastcolNum++] as Excel2.Range;
                                rangeToplam2828.Formula = String.Format("=SUM({0}2:{1}{2}", lastLetter, lastLetter, (lastcell2.Row).ToString());
                                GenelToplamHucreleri.Add(String.Format("{0}{1}", lastLetter, lastcell2.Row + 1));

                                lastLetter = Convert.ToChar(++lastLetterAsciCode).ToString();


                                Excel2.Range rangeToplam6486 = null;
                                if (dt.Columns.Contains("6486"))
                                {
                                    
                                    rangeToplam6486 = CalismaSayfasi.Cells[lastcell2.Row + 1, lastcolNum++] as Excel2.Range;
                                    rangeToplam6486.Formula = String.Format("=SUM({0}2:{1}{2}", lastLetter,lastLetter, (lastcell2.Row).ToString());
                                    GenelToplamHucreleri.Add(String.Format("{0}{1}",lastLetter, lastcell2.Row + 1));

                                    lastLetter = Convert.ToChar(++lastLetterAsciCode).ToString();
                                }

                                Excel2.Range rangeToplam6322 = null;
                                if (dt.Columns.Contains("6322"))
                                {
                                    rangeToplam6322 = CalismaSayfasi.Cells[lastcell2.Row + 1, lastcolNum++] as Excel2.Range;
                                    rangeToplam6322.Formula = String.Format("=SUM({0}2:{1}{2}", lastLetter, lastLetter, (lastcell2.Row).ToString());
                                    GenelToplamHucreleri.Add(String.Format("{0}{1}", lastLetter, lastcell2.Row + 1));

                                    lastLetter = Convert.ToChar(++lastLetterAsciCode).ToString();
                                }

                                var rangeToplamTumu = CalismaSayfasi.Cells[lastcell2.Row + 1, lastcolNum++] as Excel2.Range;
                                rangeToplamTumu.Formula = String.Format("=SUM({0}2:{1}{2}", lastLetter, lastLetter, (lastcell2.Row).ToString());
                                GenelToplamHucreleri.Add(String.Format("{0}{1}", lastLetter, lastcell2.Row + 1));
                                var rangeToplamColumn = rangeToplamTumu.EntireColumn as Excel2.Range;
                                rangeToplamColumn.ColumnWidth = 15;


                                var rangebas = CalismaSayfasi.Cells[2, 5] as Excel2.Range;
                                var rangebit = CalismaSayfasi.Cells[lastcell2.Row + 1, dt.Columns.Count] as Excel2.Range;
                                var rngtutar = CalismaSayfasi.Range[rangebas, rangebit] as Excel2.Range;
                                rngtutar.NumberFormat = "#,##0.00";
                                rngtutar.HorizontalAlignment = Excel2.XlHAlign.xlHAlignRight;

                                var fontGenelToplam = rangeGenelToplamLabel.Font;
                                fontGenelToplam.Bold = true;

                                var fonttutar = rngtutar.Font;
                                fonttutar.Bold = true;

                                rangeSubeAdiColumn.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;
                                rangeIsyeriNoColumn.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;
                                rangeSiraColumn.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var tumAlan3 = CalismaSayfasi.UsedRange;

                                var font = tumAlan3.Font;

                                font.Size = 12;
                                font.Name = "Times New Roman";

                                var borders2 = tumAlan3.Borders;
                                borders2.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                HafizadanAtilacaklar.AddRange(new List<object> { rangeGenelToplamLabel, rangeToplam6111,rangeToplam7103,rangeToplam17256,rangeToplam27256, rangeToplam7316, rangeToplam3294, rangeToplam4857, rangeToplam6645, rangeToplam2828, rangeToplam6486, rangeToplam6322, rangeToplamTumu, rangeToplamColumn, rangebas, rangebit, rngtutar,fontGenelToplam, fonttutar, tumAlan3, font, borders2 });

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
                            Metodlar.HataMesajiGoster(ex, "Muhtasar Çoklu icmali kaydedilirken hata oluştu.");

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
                    string Mesaj = "Muhtasar çoklu icmali hata nedeniyle kaydedilemedi" + Environment.NewLine;

                    HataMesajiGoster(ex, Mesaj);
                }
            }
            catch (Exception)
            {
                MuhtasarCokluIcmalKaydediliyor = false;

                throw;
            }
            finally
            {
                MuhtasarCokluIcmalKaydediliyor = false;
            }

            MuhtasarCokluIcmalKaydediliyor = false;

            return null;
        }


    }



}
