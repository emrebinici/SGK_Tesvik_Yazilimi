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
     
        public static bool MufredatKartlariKaydediliyor = false;
        public static string MufredatKartlariniKaydet(Isyerleri isyeri, List<Classes.MufredatKarti> mufredatKartlari, Classes.BildirgeIcmaliResponse bildirgeIcmalleri)
        {
            while (MufredatKartlariKaydediliyor)
            {
                System.Threading.Thread.Sleep(500);
            }

            MufredatKartlariKaydediliyor = true;

            try
            {
                var isyeripath = Metodlar.IsyeriKlasorBul(isyeri, true);

                string YeniPath = Path.Combine(isyeripath, String.Format("Müfredat Kartı {0}.xlsx", DateTime.Today.ToString("dd.MM.yyyy")));

                List<object> HafizadanAtilacaklar = new List<object>();

                try
                {
                    Excel2.Application Excelim;
                    Excel2.Workbook CalismaKitabi;
                    Excel2.Worksheet CalismaSayfasi;
                    Excel2.Worksheet CalismaSayfasiTahakkuk;

                    bool result = false;

                    var dt = new DataTable("mufredatKartlari");
                    dt.Columns.Add("Sira");
                    dt.Columns.Add("Kod");
                    dt.Columns.Add("IslemTarihi",typeof(DateTime));
                    dt.Columns.Add("YilAy");
                    dt.Columns.Add("TahsilatPostaTarihi", typeof(DateTime));
                    dt.Columns.Add("BelgeMahiyeti");
                    dt.Columns.Add("BelgeCesit_TahakkukSekli");
                    dt.Columns.Add("BorcTur");
                    dt.Columns.Add("Kanun");
                    dt.Columns.Add("PEKTutari",typeof(decimal));
                    dt.Columns.Add("THKTutari",typeof(decimal));
                    dt.Columns.Add("Indirim",typeof(decimal));
                    dt.Columns.Add("Indirim5510_5073PekTutari",typeof(decimal));
                    dt.Columns.Add("THSTutari",typeof(decimal));
                    dt.Columns.Add("GZ",typeof(decimal));

                    var sira = 0;

                    foreach (var item in mufredatKartlari)
                    {
                        sira++;
                        var newRow = dt.NewRow();
                        newRow[0] = item.TuruncuArkaPlan ? sira.ToString() + "*" : sira.ToString() ;
                        newRow[(int)Enums.MufredatKartiHucreBilgileri.Kod] = item.Kod;
                        if (!string.IsNullOrEmpty(item.IslemTarihi))
                        {
                            newRow[(int)Enums.MufredatKartiHucreBilgileri.IslemTarihi] = item.IslemTarihi;
                        }
                        newRow[(int)Enums.MufredatKartiHucreBilgileri.YilAy] = item.YilAy;
                        if (!string.IsNullOrEmpty(item.TahsilatPostaTarihi))
                        {
                            newRow[(int)Enums.MufredatKartiHucreBilgileri.TahsilatPostaTarihi] = item.TahsilatPostaTarihi;
                        }

                        newRow[(int)Enums.MufredatKartiHucreBilgileri.BelgeMahiyeti] = item.BelgeMahiyeti;
                        newRow[(int)Enums.MufredatKartiHucreBilgileri.BelgeCesit_TahakkukSekli] = item.BelgeCesit_TahakkukSekli;
                        newRow[(int)Enums.MufredatKartiHucreBilgileri.BorcTur] = item.BorcTur;
                        newRow[(int)Enums.MufredatKartiHucreBilgileri.Kanun] = item.Kanun;
                        
                        if (!string.IsNullOrEmpty(item.PEKTutari))
                        {
                            newRow[(int)Enums.MufredatKartiHucreBilgileri.PEKTutari] = item.PEKTutari.ToDecimalSgk();
                        }

                        if (!string.IsNullOrEmpty(item.THKTutari))
                        {
                            newRow[(int)Enums.MufredatKartiHucreBilgileri.THKTutari] = item.THKTutari.ToDecimalSgk();
                        }

                        if (!string.IsNullOrEmpty(item.Indirim))
                        {
                            newRow[(int)Enums.MufredatKartiHucreBilgileri.Indirim] = item.Indirim.ToDecimalSgk();
                        }

                        if (!string.IsNullOrEmpty(item.Indirim5510_5073PekTutari))
                        {
                            newRow[(int)Enums.MufredatKartiHucreBilgileri.Indirim5510_5073PekTutari] = item.Indirim5510_5073PekTutari.ToDecimalSgk();
                        }

                        if (!string.IsNullOrEmpty(item.THSTutari))
                        {
                            newRow[(int)Enums.MufredatKartiHucreBilgileri.THSTutari] = item.THSTutari.ToDecimalSgk();
                        }

                        if (!string.IsNullOrEmpty(item.GZ))
                        {
                            newRow[(int)Enums.MufredatKartiHucreBilgileri.GZ] = item.GZ.ToDecimalSgk();
                        }

                        dt.Rows.Add(newRow);

                    }


                    if (dt.Rows.Count > 0)
                    {
                        Excelim = new Excel2.Application();

                        Excelim.Visible = false;
                        Excelim.DisplayAlerts = false;

                        object SalakObje = System.Reflection.Missing.Value;

                        var workbooks = Excelim.Workbooks;

                        CalismaKitabi = workbooks.Open(Path.Combine(Application.StartupPath, "MufredatSablon.xlsx"));

                        var sheets = CalismaKitabi.Sheets;

                        HafizadanAtilacaklar.AddRange(new List<object> { workbooks, CalismaKitabi, sheets });

                        var ExcelimV2 = new Excel2.Application();

                        ExcelimV2.Visible = false;
                        ExcelimV2.DisplayAlerts = false;

                        var workbooks2 = ExcelimV2.Workbooks;

                        var GenelToplamHucreleri = new List<string>();

                        if (dt.Rows.Count > 0)
                        {
                            int SayfaNo = 1;

                            CalismaSayfasi = (Excel2.Worksheet)sheets[SayfaNo];
                            CalismaSayfasi.Activate();
                            CalismaSayfasiTahakkuk = (Excel2.Worksheet)sheets[2];


                            HafizadanAtilacaklar.Add(CalismaSayfasi);

                            var usedrange = CalismaSayfasi.UsedRange;

                            var allcells = CalismaSayfasi.Cells;

                            Excel2.Range lastrow = usedrange.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                            Excel2.Range lastrow2 = allcells[lastrow.Row + 1, lastrow.Column];

                            HafizadanAtilacaklar.AddRange(new List<object> { usedrange, allcells, lastrow, lastrow2 });

                            //if (lastrow.Row > 1)
                            //{

                            //    Excel2.Range range = CalismaSayfasi.get_Range("A2", lastrow2);

                            //    var entirerow = range.EntireRow;

                            //    entirerow.Delete(Excel2.XlDeleteShiftDirection.xlShiftUp);

                            //    HafizadanAtilacaklar.AddRange(new List<object> { range, entirerow });
                            //}


                            ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook();

                            wb.Worksheets.Add(dt);

                            string geciciDosyaYolu = YeniPath.Insert(YeniPath.IndexOf(".xlsx"), "_GEÇİCİ_Mufredat");

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

                                    var colTutar = CalismaSayfasi.Range["E:E"];

                                    var rangeSira= CalismaSayfasi.Cells[1, 1] as Excel2.Range;
                                    var rangeKod = CalismaSayfasi.Cells[1, 2] as Excel2.Range;
                                    var rangeIslemTarihi = CalismaSayfasi.Cells[1, 3] as Excel2.Range;
                                    var rangeYilAy = CalismaSayfasi.Cells[1, 4] as Excel2.Range;
                                    var rangeTahsilatPostaTarihi = CalismaSayfasi.Cells[1, 5] as Excel2.Range;
                                    var rangeBelgeMahiyeti = CalismaSayfasi.Cells[1, 6] as Excel2.Range;
                                    var rangeBelgeCesit_TahakkukSekli = CalismaSayfasi.Cells[1, 7] as Excel2.Range;
                                    var rangeBorcTur = CalismaSayfasi.Cells[1, 8] as Excel2.Range;
                                    var rangeKanun = CalismaSayfasi.Cells[1, 9] as Excel2.Range;
                                    var rangePEKTutari = CalismaSayfasi.Cells[1, 10] as Excel2.Range;
                                    var rangeTHKTutari = CalismaSayfasi.Cells[1, 11] as Excel2.Range;
                                    var rangeIndirim = CalismaSayfasi.Cells[1, 12] as Excel2.Range;
                                    var rangeIndirim5510_5073PekTutari = CalismaSayfasi.Cells[1, 13] as Excel2.Range;
                                    var rangeTHSTutari = CalismaSayfasi.Cells[1, 14] as Excel2.Range;
                                    var rangeGZ = CalismaSayfasi.Cells[1, 15] as Excel2.Range;

                                    rangeSira.Value2 = "No";
                                    rangeKod.Value2 = "Kod";
                                    rangeIslemTarihi.Value2 = "İşlem Tarihi";
                                    rangeYilAy.Value2 = "Yıl/Ay";
                                    rangeTahsilatPostaTarihi.Value2 = "Tahsilat /Posta Tarihi";
                                    rangeBelgeMahiyeti.Value2 = "Belge Mah.";
                                    rangeBelgeCesit_TahakkukSekli.Value2 = "Belge Çeşit / Tah. Şekli";
                                    rangeBorcTur.Value2 = "Borç Tür";
                                    rangeKanun.Value2 = "Kanun";
                                    rangePEKTutari.Value2 = "PEK. Tut.";
                                    rangeTHKTutari.Value2 = "THK. Tut.";
                                    rangeIndirim.Value2 = "İndirim";
                                    rangeIndirim5510_5073PekTutari.Value2 = "5510 İndirim/ 5073 Pek Tut.";
                                    rangeTHSTutari.Value2 = "THS. Tut.";
                                    rangeGZ.Value2 = "GZ";
                                    

                                    var rangeheader = CalismaSayfasi.Range[rangeSira, rangeGZ] as Excel2.Range;

                                    var borders = rangeheader.Borders;

                                    var fontHeader = rangeheader.Font;
                                    fontHeader.Bold = true;

                                    borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                    Excel2.Range rangeV2 = allcells[2, 1] as Excel2.Range;
                                    rangeV2.Select();
                                    CalismaSayfasi.Paste(Type.Missing, Type.Missing);

                                    var usedrange3 = CalismaSayfasi.UsedRange;

                                    Excel2.Range lastcell2 = usedrange3.SpecialCells(Excel2.XlCellType.xlCellTypeLastCell, Excel2.XlSpecialCellsValue.xlTextValues);

                                    HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasiV2, usedrange4, lastcell, usedrange2, lastV2, tumAlanV2, tumAlan, colTutar,rangeSira, rangeKod,rangeIslemTarihi, rangeYilAy, rangeTahsilatPostaTarihi, rangeBelgeMahiyeti,rangeBelgeCesit_TahakkukSekli,rangeBorcTur,rangeKanun,rangePEKTutari,rangeTHKTutari,rangeIndirim,rangeIndirim5510_5073PekTutari,rangeTHKTutari,rangeGZ, rangeheader, borders, fontHeader, rangeV2, usedrange3, lastcell2 });

                                    if (lastcell.Row + lastV2.Row - 1 != lastcell2.Row)
                                    {
                                        CalismaKitabi.Close(false);

                                        CalismaKitabi = workbooks.Open(Path.Combine(Application.StartupPath, "MufredatSablon.xlsx"));
                                        CalismaSayfasi = (Excel2.Worksheet)CalismaKitabi.ActiveSheet;

                                        HafizadanAtilacaklar.AddRange(new List<object> { CalismaKitabi, CalismaSayfasi });

                                        continue;
                                    }

                                    var allcells3 = CalismaSayfasi.Cells; 

                                    var toplamRowNumber = lastcell2.Row + 1;

                                    var rangeToplamlar= allcells3[toplamRowNumber, 9] as Excel2.Range;
                                    rangeToplamlar.Value2 = "TOPLAM";

                                    var rangeToplamPEK = allcells3[toplamRowNumber, 10] as Excel2.Range;
                                    rangeToplamPEK.Formula = String.Format("=SUM(J2:J{0}", (toplamRowNumber - 1).ToString());
                                    GenelToplamHucreleri.Add(String.Format("I{0}", toplamRowNumber));

                                    var rangeToplamTHK = allcells3[toplamRowNumber, 11] as Excel2.Range;
                                    rangeToplamTHK.Formula = String.Format("=SUM(K2:K{0}", (toplamRowNumber - 1).ToString());
                                    GenelToplamHucreleri.Add(String.Format("K{0}", toplamRowNumber));

                                    var rangeToplamIndirim = allcells3[toplamRowNumber, 12] as Excel2.Range;
                                    rangeToplamIndirim.Formula = String.Format("=SUM(L2:L{0}", (toplamRowNumber - 1).ToString());
                                    GenelToplamHucreleri.Add(String.Format("L{0}", toplamRowNumber));

                                    var rangeToplamIndirim5510_5073PekTutari = allcells3[toplamRowNumber, 13] as Excel2.Range;
                                    rangeToplamIndirim5510_5073PekTutari.Formula = String.Format("=SUM(M2:M{0}", (toplamRowNumber- 1).ToString());
                                    GenelToplamHucreleri.Add(String.Format("M{0}", toplamRowNumber));

                                    var rangeToplamTHSTutari = allcells3[toplamRowNumber, 14] as Excel2.Range;
                                    rangeToplamTHSTutari.Formula = String.Format("=SUM(N2:N{0}", (toplamRowNumber - 1).ToString());
                                    GenelToplamHucreleri.Add(String.Format("N{0}", toplamRowNumber));


                                    var rangeToplamGZ = allcells3[toplamRowNumber, 15] as Excel2.Range;
                                    rangeToplamGZ.Formula = String.Format("=SUM(O2:O{0}", (toplamRowNumber - 1).ToString());
                                    GenelToplamHucreleri.Add(String.Format("O{0}", toplamRowNumber));

                                    var rangebas2 = allcells3[toplamRowNumber, 9] as Excel2.Range;
                                    var rangebit2 = allcells3[toplamRowNumber, 15] as Excel2.Range;
                                    var rngtoplamlar = CalismaSayfasi.Range[rangebas2, rangebit2] as Excel2.Range;

                                    var rngtoplamlarfont = rngtoplamlar.Font;
                                    rngtoplamlarfont.Bold = true;

                                    var rangebas = allcells3[2, 10] as Excel2.Range;
                                    var rangebit = allcells3[toplamRowNumber, 15] as Excel2.Range;
                                    var rngtutar = CalismaSayfasi.Range[rangebas, rangebit] as Excel2.Range;
                                    rngtutar.NumberFormat = "#,##0.00";


                                    for (int i = 1; i < toplamRowNumber; i++)
                                    {
                                        var ilkhucre = allcells3[i, 1] as Excel2.Range;

                                        if (ilkhucre.Value2.EndsWith("*"))
                                        {
                                            var sonhucre = allcells3[i, 15] as Excel2.Range;
                                            var hucreler = CalismaSayfasi.Range[ilkhucre, sonhucre] as Excel2.Range;
                                            var interior = hucreler.Interior;
                                            interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(222, 184, 135));

                                            HafizadanAtilacaklar.AddRange(new List<object> { sonhucre, hucreler,interior, });
                                        }

                                        HafizadanAtilacaklar.Add(ilkhucre);
                                    }


                                    var tumAlan3 = CalismaSayfasi.Range["A:O"];
                                    var allcolumns = CalismaSayfasi.Columns["A:O"];

                                    allcolumns.AutoFit();

                                    var cols = allcolumns.Columns;

                                    HafizadanAtilacaklar.Add(cols);

                                    foreach (Excel2.Range column in cols)
                                    {
                                        column.ColumnWidth = (double)column.ColumnWidth + 2;

                                        HafizadanAtilacaklar.Add(column);
                                    }

                                    var font = tumAlan3.Font;
                                    font.Size = 12;
                                    font.Name = "Times New Roman";

                                    var borders2 = usedrange3.Borders;
                                    borders2.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                    HafizadanAtilacaklar.AddRange(new List<object> {allcells3, rangeToplamlar, rangeToplamPEK,rangeToplamTHK, rangeToplamIndirim, rangeToplamIndirim5510_5073PekTutari, rangeToplamTHSTutari, rangeToplamGZ, rangebas2,rangebit2,rngtoplamlar,rngtoplamlarfont, rangebas, rangebit, rngtutar, tumAlan3, allcolumns, font, borders2 });

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

                            var ToplamIndirim6111_6645 = 0m;
                            var ToplamIndirim7103_687_7252_27256_7316 = 0m;
                            var ToplamPEK7103_687_7252_27256_7316 = 0m;
                            var ToplamIptal = 0m;
                            var IptalPEKTutari5615_5084 = 0m;

                            var kayitlar6111_6645 = mufredatKartlari
                                .Where(p =>
                                     new List<string> { "06111", "06645", "14857", "02828", "16322", "26322", "25510", "46486", "56486", "66486", "03294" }.Contains(p.Kanun.PadLeft(5, '0'))
                                     && string.IsNullOrEmpty(p.TahsilatPostaTarihi) == false
                                     && p.Kod.Equals("AKTİF")
                                     && (p.BelgeMahiyeti.Equals("A") || p.BelgeMahiyeti.Equals("E")));
                            
                            if (kayitlar6111_6645.Count() > 0)
                            {
                                ToplamIndirim6111_6645 = kayitlar6111_6645.Sum(p => string.IsNullOrEmpty(p.Indirim) ? 0 : p.Indirim.ToDecimalSgk());
                            }


                            var kayitlar687_7103_7252_27256_7316 = mufredatKartlari
                                .Where(p =>
                                     new List<string> { "00687", "01687", "17103", "27103" ,"07252","27256","07316"}.Contains(p.Kanun.PadLeft(5, '0'))
                                     && string.IsNullOrEmpty(p.TahsilatPostaTarihi) == false
                                     && p.Kod.Equals("AKTİF")
                                     && (p.BelgeMahiyeti.Equals("A") || p.BelgeMahiyeti.Equals("E")));

                            if (kayitlar687_7103_7252_27256_7316.Count() > 0)
                            {
                                ToplamIndirim7103_687_7252_27256_7316= kayitlar687_7103_7252_27256_7316.Sum(p => string.IsNullOrEmpty(p.Indirim) ? 0 : p.Indirim.ToDecimalSgk());
                                ToplamPEK7103_687_7252_27256_7316= kayitlar687_7103_7252_27256_7316.Sum(p => string.IsNullOrEmpty(p.PEKTutari) ? 0 : p.PEKTutari.ToDecimalSgk());
                            }


                            var iptalKayitlari = mufredatKartlari
                                .Where(p =>
                                     ! string.IsNullOrEmpty(p.Kanun) && p.Kanun.PadLeft(5, '0').Equals("05510") == false
                                     && string.IsNullOrEmpty(p.TahsilatPostaTarihi) == false
                                     && p.Kod.Equals("AKTİF")
                                     && p.BelgeMahiyeti.Equals("I"));

                            if (iptalKayitlari.Count() > 0)
                            { 
                                ToplamIptal= iptalKayitlari.Sum(p => string.IsNullOrEmpty(p.Indirim) ? 0 : p.Indirim.ToDecimalSgk());
                            }

                            var kayitlar5615_5084 = iptalKayitlari
                                .Where(p => new List<string> { "05615", "85615", "05084", "85084" }.Contains(p.Kanun.PadLeft(5, '0')));

                            if (kayitlar5615_5084.Count() > 0)
                            {
                                IptalPEKTutari5615_5084 =  kayitlar5615_5084.Sum(p => string.IsNullOrEmpty(p.PEKTutari) ? 0 : p.PEKTutari.ToDecimalSgk()) * 0.05m;
                            }

                            var alan6111_6645 = CalismaSayfasiTahakkuk.Range["B2"];
                            var alan687_7103_7252_27256_7316 = CalismaSayfasiTahakkuk.Range["E2"];
                            var alan687_7103_7252_27256_7316_Pek = CalismaSayfasiTahakkuk.Range["E4"];
                            var alan5615_5084_Iptal_Pek = CalismaSayfasiTahakkuk.Range["E6"];
                            var alanIptallerToplamı = CalismaSayfasiTahakkuk.Range["E7"];
                            var alanIcmalToplami = CalismaSayfasiTahakkuk.Range["B11"];
                            var alan7166Toplami = CalismaSayfasiTahakkuk.Range["G2"];

                            alan6111_6645.Value2 = ToplamIndirim6111_6645; 
                            alan687_7103_7252_27256_7316.Value2 = ToplamIndirim7103_687_7252_27256_7316;
                            alan687_7103_7252_27256_7316_Pek.Value2 = ToplamPEK7103_687_7252_27256_7316;
                            alan5615_5084_Iptal_Pek.Value2 = IptalPEKTutari5615_5084;
                            alanIptallerToplamı.Value2 = ToplamIptal;

                            if (bildirgeIcmalleri.Durum)
                            {
                                if (bildirgeIcmalleri.Onaylilar["7166"].Count > 0)
                                {
                                    alan7166Toplami.Value2 = bildirgeIcmalleri.Onaylilar["7166"].Sum(p => p.Tutar);
                                }

                                var tumicmaller = bildirgeIcmalleri.Onaylilar.SelectMany(p => p.Value);

                                if (tumicmaller.Count() > 0)
                                {
                                    alanIcmalToplami.Value2 = tumicmaller.Sum(p => p.Tutar);
                                }
                            }

                            HafizadanAtilacaklar.AddRange(new List<object> { alan6111_6645, alan687_7103_7252_27256_7316, alan687_7103_7252_27256_7316_Pek, alan5615_5084_Iptal_Pek, alanIptallerToplamı, alanIcmalToplami, alan7166Toplami });
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

                            var files = Directory.GetFiles(path, "Müfredat Kartı*.xlsx");

                            foreach (var file in files)
                            {
                                File.Delete(file);
                            }

                            CalismaKitabi.SaveAs(YeniPath);

                            result = true;
                        }
                        catch (Exception ex)
                        {
                            Metodlar.HataMesajiGoster(ex, "Müfredat kartları kaydedilirken hata oluştu.");

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
                    string Mesaj = "Müfredat kartları hata nedeniyle kaydedilemedi" + Environment.NewLine;

                    HataMesajiGoster(ex, Mesaj);
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                MufredatKartlariKaydediliyor = false;
            }



            return null;
        }


    }


}
