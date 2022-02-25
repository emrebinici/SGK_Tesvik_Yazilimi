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
        public static bool CariAphbKaydediliyor = false;
        public static string CariAphbKaydet(Isyerleri isyeri, DataTable dt, DateTime tarih)
        {
            while (CariAphbKaydediliyor)
            {
                System.Threading.Thread.Sleep(500);
            }

            CariAphbKaydediliyor = true;

            try
            {
                var isyeripath = Metodlar.IsyeriKlasorBul(isyeri, true);

                string YeniPath = Path.Combine(isyeripath, String.Format("Cari Aphb-{0}-{1}.xlsx", tarih.Year, tarih.Month));

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

                        Excel2.Workbook CalismaKitabi = workbooks.Open(Path.Combine(Application.StartupPath, "ListeTemplate.xlsx"));
                        Excel2.Worksheet CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                        var cells = CalismaSayfasi.Cells;

                        Excel2.Range last = cells.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Type.Missing);
                        //Excel2.Range range = CalismaSayfasi.get_Range("A1", last);

                        HafizadanAtilacaklar.AddRange(new List<object> { workbooks, CalismaKitabi, CalismaSayfasi, cells, last });

                        int LastRow = last.Row;

                        int SatirIndex = LastRow + 1;

                        NewExportExcelV2 excelV2 = new NewExportExcelV2();
                        excelV2.gridControl1.DataSource = dt;
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
                        string geciciDosyaYolu = YeniPath.Insert(YeniPath.IndexOf(".xlsx"), "_GEÇİCİ");
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

                                    CalismaKitabi = workbooks.Open(Path.Combine(Application.StartupPath, "ListeTemplate.xlsx"));
                                    CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                                    HafizadanAtilacaklar.AddRange(new List<object> { CalismaKitabi, CalismaSayfasi });

                                    continue;
                                }

                                var tumAlan = CalismaSayfasi.Range["A:X"];

                                tumAlan.NumberFormat = "@";

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

                                var sonuc = allDataRange.Sort(AraciSutun, Excel2.XlSortOrder.xlDescending, BelgeTuruSutun, Type.Missing, Excel2.XlSortOrder.xlAscending, Type.Missing, Type.Missing, Excel2.XlYesNoGuess.xlYes);

                                sonuc = allDataRange.Sort(onayDurumuSutun, Excel2.XlSortOrder.xlDescending, YilSutun, Type.Missing, Excel2.XlSortOrder.xlDescending, AySutun, Excel2.XlSortOrder.xlDescending, Excel2.XlYesNoGuess.xlYes);

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

                        HafizadanAtilacaklar.AddRange(new List<object> { allcells2, cellOnayDurumu, cellOrijinalKanunNo });

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

                                CalismaKitabi.SaveAs(YeniPath);

                            }
                            else
                            {
                                result = false;
                            }

                        }
                        catch (Exception ex)
                        {
                            Metodlar.HataMesajiGoster(ex, "Cari aphb dosyası kaydedilirken hata oluştu.");

                            result = false;
                        }

                        if (result)
                        {
                           var eskiCariDosyalar = Directory.GetFiles(isyeripath, "Cari Aphb-*.xlsx");

                            foreach (var eskiCari in eskiCariDosyalar)
                            {
                                if (eskiCari == YeniPath) continue;

                                try {
                                    File.Delete(eskiCari);
                                } catch { }
                            }
                            
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
                    string Mesaj = "Cari Aphb listesi hata nedeniyle kaydedilemedi" + Environment.NewLine;

                    HataMesajiGoster(ex, Mesaj);
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally {
                CariAphbKaydediliyor = false;
            }



            return null;
        }


    }



}
