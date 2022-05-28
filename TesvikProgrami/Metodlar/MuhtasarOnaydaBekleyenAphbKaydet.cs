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
        public static bool MuhtasarOnaydaBekleyenAphbKaydediliyor = false;
        public static string MuhtasarOnaydaBekleyenAphbKaydet(Isyerleri isyeri, List<Bildirge> bildirgeler)
        {
            while (MuhtasarOnaydaBekleyenAphbKaydediliyor)
            {
                System.Threading.Thread.Sleep(500);
            }

            MuhtasarOnaydaBekleyenAphbKaydediliyor = true;

            try
            {
                var isyeripath = Metodlar.IsyeriKlasorBul(isyeri, true);

                string YeniPath = Path.Combine(isyeripath, String.Format("Muhtasar Onay Bekleyenler-{0}.xlsx", DateTime.Now.ToString("dd-MM-yyyy")));

                List<object> HafizadanAtilacaklar = new List<object>();

                var result = true;

                try
                {
                    DataTable dt = new DataTable("Aphb");


                    for (int i = 0; i < 27; i++)
                    {
                        dt.Columns.Add("Column" + i.ToString());
                    }

                    dt.Columns[0].DataType = typeof(Int32);
                    dt.Columns[1].DataType = typeof(Int32);
                    dt.Columns[4].DataType = typeof(Int32);
                    dt.Columns[11].DataType = typeof(decimal);
                    dt.Columns[12].DataType = typeof(decimal);
                    dt.Columns[13].DataType = typeof(Int32);


                    bildirgeler.ForEach(bildirge =>
                    {


                        string Kanun = bildirge.Kanun;


                        foreach (AphbSatir kisi in bildirge.Kisiler)
                        {
                            DataRow row = dt.NewRow();

                            row[(int)Enums.AphbHucreBilgileri.Yil] = bildirge.Yil;

                            row[(int)Enums.AphbHucreBilgileri.Ay] = bildirge.Ay;

                            row[(int)Enums.AphbHucreBilgileri.Kanun] = Kanun == "00000" ? "" : Kanun;

                            row[(int)Enums.AphbHucreBilgileri.Mahiyet] = bildirge.Mahiyet;

                            row[(int)Enums.AphbHucreBilgileri.BelgeTuru] = bildirge.BelgeTuru;

                            row[(int)Enums.AphbHucreBilgileri.SiraNo] = kisi.SiraNo;

                            row[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo] = kisi.SosyalGuvenlikNo;

                            row[(int)Enums.AphbHucreBilgileri.Ad] = kisi.Adi;

                            row[(int)Enums.AphbHucreBilgileri.Soyad] = kisi.Soyadi;

                            row[(int)Enums.AphbHucreBilgileri.IlkSoyadi] = kisi.IlkSoyadi;

                            row[(int)Enums.AphbHucreBilgileri.Ucret] = string.IsNullOrEmpty(kisi.Ucret) ? "0" : kisi.Ucret;

                            row[(int)Enums.AphbHucreBilgileri.Ikramiye] = string.IsNullOrEmpty(kisi.Ikramiye) ? "0" : kisi.Ikramiye;

                            row[(int)Enums.AphbHucreBilgileri.Gun] = kisi.Gun;

                            row[(int)Enums.AphbHucreBilgileri.UCG] = kisi.UCG;

                            row[(int)Enums.AphbHucreBilgileri.EksikGun] = kisi.EksikGunSayisi;

                            row[(int)Enums.AphbHucreBilgileri.GirisGunu] = kisi.GirisGunu;

                            row[(int)Enums.AphbHucreBilgileri.CikisGunu] = kisi.CikisGunu;

                            row[(int)Enums.AphbHucreBilgileri.EksikGunSebebi] = kisi.EksikGunNedeni;

                            row[(int)Enums.AphbHucreBilgileri.IstenCikisNedeni] = kisi.IstenCikisNedeni;

                            row[(int)Enums.AphbHucreBilgileri.MeslekKod] = kisi.MeslekKod;

                            row[(int)Enums.AphbHucreBilgileri.Araci] = bildirge.AraciveyaIsveren ?? string.Empty;

                            row[(int)Enums.AphbHucreBilgileri.OnayDurumu] = bildirge.Askida ? "Onaylanmamış" : "";

                            row[(int)Enums.AphbHucreBilgileri.OrijinalKanunNo] = bildirge.OrijinalKanunNo;

                            row[(int)Enums.AphbHucreBilgileri.BildirgeRefNo] = bildirge.RefNo.Trim();

                            dt.Rows.Add(row);
                        }
                    });


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
                            Metodlar.HataMesajiGoster(ex, "Muhtasar onayda bekleyenler dosyası kaydedilirken hata oluştu.");

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
                    string Mesaj = "Muhtasar onayda bekleyenler dosyası hata nedeniyle kaydedilemedi" + Environment.NewLine;

                    HataMesajiGoster(ex, Mesaj);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally {
                MuhtasarOnaydaBekleyenAphbKaydediliyor = false;
            }

            return null;
        }


    }


}
