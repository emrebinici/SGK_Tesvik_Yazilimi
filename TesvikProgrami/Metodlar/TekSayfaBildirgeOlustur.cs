using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TesvikProgrami.Classes;
using Excel2 = Microsoft.Office.Interop.Excel;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {

        public static bool TekSayfaExcelOlusturIcinKaydediliyor  = false;
        public static string TekSayfaBildirgeOlustur(Cikti cikti, Isyerleri isyeri, string Yil, string Ay, string savepath, List<DataRow> liste14857)
        {
            while (TekSayfaExcelOlusturIcinKaydediliyor)
            {
                System.Threading.Thread.Sleep(500);
            }

            TekSayfaExcelOlusturIcinKaydediliyor = true;

            try
            {
                string YeniPath = savepath;

                List<object> HafizadanAtilacaklar = new List<object>();

                try
                {
                    Excel2.Application Excelim;
                    Excel2.Workbook CalismaKitabi;
                    Excel2.Worksheet CalismaSayfasi;

                    var ciktiToplamGun = cikti.Gun_Tesvik_Verilmeyenler_Dahil;
                    var ciktiToplamUcret = cikti.Matrah_Tesvik_Verilmeyenler_Dahil;
                    var sigortaliSayisi = cikti.Kisiler.Count;

                    bool result = false;

                    DataTable dt = new DataTable( cikti.Kanun + " Cikti");
                    dt.Columns.Add("Sira", typeof(int));
                    dt.Columns.Add("SosyalGuvenlikSicilNumarasi");
                    dt.Columns.Add("Adi");
                    dt.Columns.Add("Soyadi");
                    dt.Columns.Add("Gun", typeof(int));
                    dt.Columns.Add("Ucret",typeof(decimal));
                    dt.Columns.Add("Ikramiye",typeof(decimal));

                    int sirano = 1;

                    foreach (var satir in cikti.satirlar)
                    {
                        var row = dt.NewRow();

                        row[0] = sirano++;
                        row[1] = satir.SosyalGuvenlikNo;
                        row[2] = satir.Adi;
                        row[3] = satir.Soyadi;
                        row[4] = satir.Gun.ToInt();
                        row[5] = satir.Ucret.ToDecimalSgk();
                        row[6] = satir.Ikramiye.ToDecimalSgk();

                        dt.Rows.Add(row);
                    }

                    foreach (var satir in liste14857)
                    {
                        var row = dt.NewRow();

                        row[0] = sirano++;
                        row[1] = satir[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString();
                        row[2] = satir[(int)Enums.AphbHucreBilgileri.Ad].ToString();
                        row[3] = satir[(int)Enums.AphbHucreBilgileri.Soyad].ToString();
                        row[4] = satir[(int)Enums.AphbHucreBilgileri.Gun].ToString().ToInt();
                        row[5] = satir[(int)Enums.AphbHucreBilgileri.Ucret].ToString().ToDecimalSgk();
                        row[6] = satir[(int)Enums.AphbHucreBilgileri.Ikramiye].ToString().ToDecimalSgk();

                        ciktiToplamGun += (int)row[4];
                        ciktiToplamUcret += (decimal)row[5] + (decimal)row[6];

                        if (! cikti.Kisiler.Any(p=> p.Key.TckimlikNo == row[1].ToString()))
                        {
                            sigortaliSayisi++;
                        }

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
                        CalismaSayfasi.Name = cikti.Kanun.PadLeft(5,'0');
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

                        string geciciDosyaYolu = YeniPath.Insert(YeniPath.IndexOf(".xlsx"), "_GEÇİCİ_TekSayfaBildirge");

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
                                rangeBeyannameYilAy.Value2 = Yil+"/"+Ay.PadLeft(2,'0');

                                var rangeBelgeTuruLabel = CalismaSayfasi.Range[CalismaSayfasi.Cells[4, 1], CalismaSayfasi.Cells[4, 2]] as Excel2.Range;
                                rangeBelgeTuruLabel.Merge();
                                rangeBelgeTuruLabel.Value2 = "Belge Türü";

                                var rangeBelgeTuru = CalismaSayfasi.Range[CalismaSayfasi.Cells[4, 3], CalismaSayfasi.Cells[4, 7]] as Excel2.Range;
                                rangeBelgeTuru.Merge();
                                rangeBelgeTuru.NumberFormat = "@";
                                rangeBelgeTuru.Value2 = cikti.BelgeTuru.PadLeft(2,'0');

                                var rangeKanunLabel = CalismaSayfasi.Range[CalismaSayfasi.Cells[5, 1], CalismaSayfasi.Cells[5, 2]] as Excel2.Range;
                                rangeKanunLabel.Merge();
                                rangeKanunLabel.Value2 = "Kanun Numarası";

                                var rangeKanun = CalismaSayfasi.Range[CalismaSayfasi.Cells[5, 3], CalismaSayfasi.Cells[5, 7]] as Excel2.Range;
                                rangeKanun.Merge();
                                rangeKanun.NumberFormat = "@";
                                rangeKanun.Value2 = cikti.Kanun.PadLeft(5,'0');

                                var rangeBelgeMahiyetiLabel = CalismaSayfasi.Range[CalismaSayfasi.Cells[6, 1], CalismaSayfasi.Cells[6, 2]] as Excel2.Range;
                                rangeBelgeMahiyetiLabel.Merge();
                                rangeBelgeMahiyetiLabel.Value2 = "Belge mahiyeti";

                                var rangeBelgeMahiyeti = CalismaSayfasi.Range[CalismaSayfasi.Cells[6, 3], CalismaSayfasi.Cells[6, 7]] as Excel2.Range;
                                rangeBelgeMahiyeti.Merge();
                                rangeBelgeMahiyeti.Value2 = cikti.Iptal ? "İptal" : cikti.Asil ? "Asıl" : "Ek";

                                var rangeSigortaliSayisiLabel = CalismaSayfasi.Range[CalismaSayfasi.Cells[7, 1], CalismaSayfasi.Cells[7, 2]] as Excel2.Range;
                                rangeSigortaliSayisiLabel.Merge();
                                rangeSigortaliSayisiLabel.Value2 = "Sigortalı sayısı";

                                var rangeSigortaliSayisi = CalismaSayfasi.Range[CalismaSayfasi.Cells[7, 3], CalismaSayfasi.Cells[7, 7]] as Excel2.Range;
                                rangeSigortaliSayisi.Merge();
                                rangeSigortaliSayisi.Value2 = sigortaliSayisi;

                                var rangeGunLabel = CalismaSayfasi.Range[CalismaSayfasi.Cells[8, 1], CalismaSayfasi.Cells[8, 2]] as Excel2.Range;
                                rangeGunLabel.Merge();
                                rangeGunLabel.Value2 = "Prim ödeme gün sayısı toplamı";

                                var rangePrimGunSayisi= CalismaSayfasi.Range[CalismaSayfasi.Cells[8, 3], CalismaSayfasi.Cells[8, 7]] as Excel2.Range;
                                rangePrimGunSayisi.Merge();
                                rangePrimGunSayisi.Value2 = ciktiToplamGun;

                                var rangeKazancLabel = CalismaSayfasi.Range[CalismaSayfasi.Cells[9, 1], CalismaSayfasi.Cells[9, 2]] as Excel2.Range;
                                rangeKazancLabel.Merge();
                                rangeKazancLabel.Value2 = "Prime esas kazanç toplamı";

                                var rangeKazanc = CalismaSayfasi.Range[CalismaSayfasi.Cells[9, 3], CalismaSayfasi.Cells[9, 7]] as Excel2.Range;
                                rangeKazanc.Merge();
                                rangeKazanc.Value2 = ciktiToplamUcret.ToTL().Replace("₺", "");


                                var rangeSira= CalismaSayfasi.Cells[10, 1] as Excel2.Range;
                                var rangeSosyalGuvenlikNo= CalismaSayfasi.Cells[10, 2] as Excel2.Range;
                                var rangeAd = CalismaSayfasi.Cells[10, 3] as Excel2.Range;
                                var rangeSoyad = CalismaSayfasi.Cells[10, 4] as Excel2.Range;
                                var rangeGun = CalismaSayfasi.Cells[10, 5] as Excel2.Range;
                                var rangeUcret = CalismaSayfasi.Cells[10, 6] as Excel2.Range;
                                var rangeIkramiye = CalismaSayfasi.Cells[10, 7] as Excel2.Range;

                                rangeSira.Value2 = "Sıra No";
                                rangeSosyalGuvenlikNo.Value2 = "T.C. Numarası";
                                rangeAd.Value2 = "Adı";
                                rangeSoyad.Value2 = "Soyadı";
                                rangeGun.Value2 = "Prim Ödeme Günü";
                                rangeUcret.Value2 = "Hak Edilen Ücret";
                                rangeIkramiye.Value2 = "Prim İkramiye";

                                var rangeCiktiBilgileri = CalismaSayfasi.get_Range("A1","C9") as Excel2.Range;

                                rangeCiktiBilgileri.HorizontalAlignment = Excel2.XlHAlign.xlHAlignLeft;
                                rangeCiktiBilgileri.RowHeight = 20;
                                rangeCiktiBilgileri.NumberFormat = "@";


                                var rangeheader = CalismaSayfasi.Range[rangeSira, rangeIkramiye] as Excel2.Range;

                                var rangeLabellar = CalismaSayfasi.Range[rangeIsverenAdiLabel, rangeKazancLabel] as Excel2.Range;

                                rangeLabellar.Font.Bold = true;

                                rangeheader.Style.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;
                                rangeheader.Style.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var interior = rangeheader.Interior;

                                interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(197, 217, 241));


                                CalismaSayfasi.Columns["B:G"].ColumnWidth = 20;

                                rangeSira.WrapText = true;
                                rangeSosyalGuvenlikNo.WrapText = true;
                                rangeAd.WrapText = true;
                                rangeSoyad.WrapText = true;
                                rangeGun.WrapText = true;
                                rangeUcret.WrapText = true;
                                rangeIkramiye.WrapText = true;

                                var ilkSatir = rangeSira.EntireRow;
                                ilkSatir.RowHeight = 30;

                                var borders = rangeheader.Borders;

                                var fontHeader = rangeheader.Font;
                                fontHeader.Bold = true;

                                borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                Excel2.Range rangeV2 = allcells[11, 1] as Excel2.Range;
                                CalismaSayfasi.Activate();

                                rangeV2.Select();
                                CalismaSayfasi.Paste(Type.Missing, Type.Missing);

                                var usedrange3 = CalismaSayfasi.UsedRange;

                                Excel2.Range lastcell2 = usedrange3.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                                HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasiV2, usedrange4, lastcell, usedrange2, lastV2, tumAlanV2, rangeIsverenAdiLabel, rangeIsverenAdi, rangeIsyeriNoLabel,rangeIsyeriNo, rangeBeyannameYilAyLabel,rangeBeyannameYilAy, rangeBelgeTuruLabel,rangeBelgeTuru,rangeKanunLabel,rangeKanun, rangeBelgeMahiyetiLabel,rangeBelgeMahiyeti,rangeSigortaliSayisi,rangeSigortaliSayisiLabel,rangeGunLabel, rangePrimGunSayisi, rangeKazancLabel,rangeKazanc, rangeSira,rangeSosyalGuvenlikNo,rangeAd, rangeSoyad, rangeGun,rangeUcret,rangeIkramiye ,ilkSatir,rangeCiktiBilgileri, rangeLabellar, rangeheader, interior, borders, fontHeader, rangeV2, usedrange3, lastcell2 });

                                if (lastcell.Row + lastV2.Row - 1 > lastcell2.Row)
                                {
                                    CalismaKitabi.Close(false);

                                    CalismaKitabi = workbooks.Add();
                                    CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                                    HafizadanAtilacaklar.AddRange(new List<object> { CalismaKitabi, CalismaSayfasi });

                                    continue;
                                }

                                var cellsSiraNoTcNo = CalismaSayfasi.Range["A11", "B" + lastcell2.Row] as Excel2.Range;
                                cellsSiraNoTcNo.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                var cellsGun = CalismaSayfasi.Range["E11", "E" + lastcell2.Row] as Excel2.Range;
                                cellsGun.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;
                                cellsGun.NumberFormat = "0";

                                var cellsUcretIkramiye = CalismaSayfasi.Range["F11", "G" + lastcell2.Row] as Excel2.Range;
                                cellsUcretIkramiye.HorizontalAlignment = Excel2.XlHAlign.xlHAlignRight;
                                cellsUcretIkramiye.NumberFormat = "#,##0.00";

                                rangePrimGunSayisi.Select();

                                var rangeBizimVerdiklerimiz = CalismaSayfasi.Range["A11", "G" + (11 + cikti.satirlar.Count - 1)] as Excel2.Range;
                                var font4 = rangeBizimVerdiklerimiz.Font;
                                font4.Bold = true;

                                //rangePrimGunSayisi.Formula = String.Format("=SUM(E11:E{0})", (lastcell2.Row).ToString());
                                //rangeKazanc.Formula = String.Format("=SUM(F11:G{0})", (lastcell2.Row).ToString());


                                var cellIlk = CalismaSayfasi.Cells[1, 1] as Excel2.Range;
                                cellIlk.Select();

                                var tumAlan3 = CalismaSayfasi.UsedRange;

                                var font = tumAlan3.Font;

                                font.Size = 12;
                                font.Name = "Times New Roman";

                                var borders2 = tumAlan3.Borders;
                                borders2.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                HafizadanAtilacaklar.AddRange(new List<object> { cellsSiraNoTcNo,cellsGun,cellsUcretIkramiye, cellIlk, tumAlan3, font, borders2, rangeBizimVerdiklerimiz, font4 });

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
                            Metodlar.HataMesajiGoster(ex, cikti.Kanun+ " tek sayfa bildirgesi kaydedilirken hata oluştu.");

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
                    string Mesaj = cikti.Kanun+ " tek sayfa bildirgesi hata nedeniyle kaydedilemedi" + Environment.NewLine;

                    HataMesajiGoster(ex, Mesaj);
                }
            }
            catch (Exception)
            {
                TekSayfaExcelOlusturIcinKaydediliyor = false;

                throw;
            }
            finally
            {
                TekSayfaExcelOlusturIcinKaydediliyor = false;
            }

            TekSayfaExcelOlusturIcinKaydediliyor = false;

            return null;
        }


    }



}
