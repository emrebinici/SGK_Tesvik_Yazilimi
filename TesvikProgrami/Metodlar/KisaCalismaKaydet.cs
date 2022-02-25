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

        public static bool KisaCalismaKaydediliyor = false;
        public static string KisaCalismaKaydet(Isyerleri isyeri, List<KisaCalismaRow> liste)
        {
            while (KisaCalismaKaydediliyor)
            {
                System.Threading.Thread.Sleep(500);
            }

            KisaCalismaKaydediliyor = true;

            try
            {
                var isyeripath = Metodlar.IsyeriKlasorBul(isyeri, true);

                string YeniPath = Path.Combine(isyeripath, String.Format("Kısa Çalışma {0}.xlsx", DateTime.Today.ToString("dd.MM.yyyy")));

                List<object> HafizadanAtilacaklar = new List<object>();

                try
                {
                    Excel2.Application Excelim;
                    Excel2.Workbook CalismaKitabi;
                    Excel2.Worksheet CalismaSayfasi;

                    bool result = false;

                    var dt = new DataTable("uygunOlanlar60");
                    dt.Columns.Add("Sira");
                    dt.Columns.Add("tc");
                    dt.Columns.Add("Ad");
                    dt.Columns.Add("Soyad");
                    dt.Columns.Add("son60gun");
                    dt.Columns.Add("son3yildagunsayisi");
                    dt.Columns.Add("uygunlukdurumu");
                    dt.Columns.Add("gunlukortalama",typeof(decimal));
                    dt.Columns.Add("kisacalismagunsayisi", typeof(int));
                    dt.Columns.Add("toplamkisacalismaodenegi", typeof(decimal));
                    dt.Columns.Add("6111tesvikbitisayi");
                    dt.Columns.Add("6111tesvikortalamasi");
                    dt.Columns.Add("7103tesvikbitisayi");
                    dt.Columns.Add("7103tesvikortalamasi");

                    var sira = 0;

                    liste = liste.OrderBy(p => p.KisaCalismaUygunlukDurumu).ToList();

                    foreach (var item in liste)
                    {
                        sira++;
                        var newRow = dt.NewRow();
                        newRow[0] = item.KisaCalismaUygunlukDurumu == "Uygun" ? sira.ToString() + "*" : sira.ToString();
                        newRow[(int)Enums.KisaCalismaHucreBilgileri.TcKimlikNo] = item.TcNo;
                        newRow[(int)Enums.KisaCalismaHucreBilgileri.Ad] = item.Ad;
                        newRow[(int)Enums.KisaCalismaHucreBilgileri.Soyad] = item.Soyad;
                        newRow[(int)Enums.KisaCalismaHucreBilgileri.Son60Gun] = item.Son60Gun;
                        newRow[(int)Enums.KisaCalismaHucreBilgileri.Son3yildaGunSayisi] = item.Son3YildaGunSayisi;
                        newRow[(int)Enums.KisaCalismaHucreBilgileri.KisaCalismaUygunlukDurumu] = item.KisaCalismaUygunlukDurumu;
                        if (item.GunlukKisaCalismaOdenegi.HasValue)
                            newRow[(int)Enums.KisaCalismaHucreBilgileri.GunlukKisaCalismaOdenegi] = item.GunlukKisaCalismaOdenegi;
                        if (item.KisaCalismaGunSayisi.HasValue)
                            newRow[(int)Enums.KisaCalismaHucreBilgileri.KisaCalismaGunSayisi] = item.KisaCalismaGunSayisi;
                        newRow[(int)Enums.KisaCalismaHucreBilgileri.TesvikSuresiBitisAyi6111] = item.TesvikSuresiBitisAyi6111;
                        newRow[(int)Enums.KisaCalismaHucreBilgileri.TesvikOrtalama6111] = item.TesvikOrtalama6111;
                        newRow[(int)Enums.KisaCalismaHucreBilgileri.TesvikSuresiBitisAyi7103] = item.TesvikSuresiBitisAyi7103;
                        newRow[(int)Enums.KisaCalismaHucreBilgileri.TesvikOrtalama7103] = item.TesvikOrtalama7103;

                        dt.Rows.Add(newRow);

                    }


                    if (dt.Rows.Count > 0)
                    {
                        Excelim = new Excel2.Application();

                        Excelim.Visible = false;
                        Excelim.DisplayAlerts = false;

                        object SalakObje = System.Reflection.Missing.Value;

                        var workbooks = Excelim.Workbooks;

                        CalismaKitabi = workbooks.Open(Path.Combine(Application.StartupPath, "KisaCalismaSablon.xlsx"));

                        var sheets = CalismaKitabi.Sheets;

                        HafizadanAtilacaklar.AddRange(new List<object> { workbooks, CalismaKitabi, sheets });

                        var ExcelimV2 = new Excel2.Application();

                        ExcelimV2.Visible = false;
                        ExcelimV2.DisplayAlerts = false;

                        var workbooks2 = ExcelimV2.Workbooks;

                        var GenelToplamHucreleri = new List<string>();

                        if (dt.Rows.Count > 0)
                        {
                            string isyerisicilNo = "";
                            string sirketadi = isyeri.Sirketler.SirketAdi+" ŞİRKETİNE AİT";

                            try
                            {
                                List<string> isyerisicils = new List<string>();

                                isyerisicils.Add(isyeri.IsyeriSicilNo.Substring(0, 1));

                                isyerisicils.Add(isyeri.IsyeriSicilNo.Substring(1, 4));

                                isyerisicils.Add(isyeri.IsyeriSicilNo.Substring(5, 2));

                                isyerisicils.Add(isyeri.IsyeriSicilNo.Substring(7, 2));

                                isyerisicils.Add(isyeri.IsyeriSicilNo.Substring(9, 7));

                                isyerisicils.Add(isyeri.IsyeriSicilNo.Substring(16, 3));

                                isyerisicils.Add(isyeri.IsyeriSicilNo.Substring(19, 2));

                                isyerisicilNo = String.Join(" ", isyerisicils.ToArray()).Trim();

                                isyerisicilNo += "-" + isyeri.IsyeriSicilNo.Substring(21, 2);

                            }
                            catch { }

                            int SayfaNo = 1;

                            CalismaSayfasi = (Excel2.Worksheet)sheets[SayfaNo];
                            CalismaSayfasi.Activate();

                            Excel2.Range rngsirketadi = CalismaSayfasi.Range["A2"];
                            Excel2.Range rngisyeriSicil = CalismaSayfasi.Range["A3"];
                            rngsirketadi.Value2 = sirketadi;
                            rngisyeriSicil.Value2 = isyerisicilNo;

                            HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasi, rngsirketadi, rngisyeriSicil });

                            var usedrange = CalismaSayfasi.UsedRange;

                            var allcells = CalismaSayfasi.Cells;

                            HafizadanAtilacaklar.AddRange(new List<object> { usedrange, allcells });

                            ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook();

                            wb.Worksheets.Add(dt);

                            string geciciDosyaYolu = YeniPath.Insert(YeniPath.IndexOf(".xlsx"), "_GEÇİCİ_KisaCalisma");

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

                                    var tumAlan = CalismaSayfasi.Range["A:N"];

                                    Excel2.Range rangeV2 = allcells[7, 1] as Excel2.Range;
                                    rangeV2.Select();
                                    CalismaSayfasi.Paste(Type.Missing, Type.Missing);

                                    var usedrange3 = CalismaSayfasi.UsedRange;

                                    Excel2.Range lastcell2 = usedrange3.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                                    HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasiV2, usedrange4, lastcell, usedrange2, lastV2, tumAlanV2, tumAlan, rangeV2, usedrange3, lastcell2 });

                                    if ((lastcell2.Row - 7 + 1 + 1) != lastV2.Row)
                                    {
                                        CalismaKitabi.Close(false);

                                        CalismaKitabi = workbooks.Open(Path.Combine(Application.StartupPath, "KisaCalismaSablon.xlsx"));
                                        CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                                        HafizadanAtilacaklar.AddRange(new List<object> { CalismaKitabi, CalismaSayfasi });

                                        continue;
                                    }

                                    var allcells3 = CalismaSayfasi.Cells;

                                    var sonSatir = lastcell2.Row;

                                    for (int i = 7; i <= sonSatir; i++)
                                    {
                                        var ilkhucre = allcells3[i, 1] as Excel2.Range;
                                        var toplamhucre= allcells3[i, 10] as Excel2.Range;
                                        var sonhucre = allcells3[i, 14] as Excel2.Range;
                                        var hucreler = CalismaSayfasi.Range[ilkhucre, sonhucre] as Excel2.Range;
                                        var interior = hucreler.Interior;
                                        interior.Color = ilkhucre.Value2.EndsWith("*") ? System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(178, 252, 144)) : System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(251, 252, 144));

                                        toplamhucre.Formula = String.Format("=PRODUCT(H{0},I{1})",i,i);
                                        toplamhucre.NumberFormat = "#,##0.00";

                                        HafizadanAtilacaklar.AddRange(new List<object> { toplamhucre , sonhucre, hucreler, interior, });

                                        ilkhucre.Value2 = ilkhucre.Value2.ToString().Replace("*", "");

                                        HafizadanAtilacaklar.Add(ilkhucre);
                                    }

                                    var tumAlan3 = CalismaSayfasi.Range["A:O"];

                                    var font = tumAlan3.Font;
                                    font.Size = 12;
                                    font.Name = "Times New Roman";

                                    var rangeKisiler = CalismaSayfasi.Range["A7", lastcell2];

                                    var borders2 = rangeKisiler.Borders;
                                    borders2.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                    HafizadanAtilacaklar.AddRange(new List<object> { allcells3, tumAlan3, font,rangeKisiler, borders2 });

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

                            var files = Directory.GetFiles(path, "Kısa Çalışma*.xlsx");

                            foreach (var file in files)
                            {
                                File.Delete(file);
                            }

                            CalismaKitabi.SaveAs(YeniPath);

                            result = true;
                        }
                        catch (Exception ex)
                        {
                            Metodlar.HataMesajiGoster(ex, "Kısa çalışma listesi kaydedilirken hata oluştu.");

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
                    string Mesaj = "Kısa çalışma listesi hata nedeniyle kaydedilemedi" + Environment.NewLine;

                    HataMesajiGoster(ex, Mesaj);
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                KisaCalismaKaydediliyor = false;
            }

            KisaCalismaKaydediliyor = false;

            return null;
        }


    }



}
