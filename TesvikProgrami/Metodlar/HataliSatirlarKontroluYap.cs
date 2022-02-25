using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static void HataliSatirlarKontroluYap(Isyerleri isyeri, DataTable dtaylikliste, DataSet dsbasvurulistesi, bool AphbKontrolEt, bool BasvuruFormuKontrolEt,ref DataTable dthatalisatirlar, ref Dictionary<string, DataTable> BasvuruFormlariHataliSatirlar)
        {

            if (AphbKontrolEt)
            {
                #region Aphb Hatalı Satır Kontrolü

                int rownum = 2;

                foreach (DataRow row in dtaylikliste.Rows)
                {
                    string gtarih = row[(int)Enums.AphbHucreBilgileri.GirisGunu].ToString();

                    bool hatavar = false;

                    try
                    {

                        int yiltemp = Convert.ToInt32(row[(int)Enums.AphbHucreBilgileri.Yil].ToString().Trim());

                        int aytemp = Convert.ToInt32(row[(int)Enums.AphbHucreBilgileri.Ay].ToString().Trim());

                        int belgeturutemp = Convert.ToInt32(row[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString().Trim());

                        decimal Ucret = row[(int)Enums.AphbHucreBilgileri.Ucret].ToString().Trim().ToDecimalSgk();

                        decimal Ikramiye = row[(int)Enums.AphbHucreBilgileri.Ikramiye].ToString().Trim().ToDecimalSgk();

                        int kanun = Convert.ToInt32(row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().Trim().PadLeft(5,'0'));

                        int orijinalkanun = Convert.ToInt32(row[(int)Enums.AphbHucreBilgileri.OrijinalKanunNo].ToString().Trim().PadLeft(5,'0'));

                        if (!String.IsNullOrEmpty(gtarih))
                        {
                            try
                            {
                                var AyIcindeIseGirisTarihi = Convert.ToDateTime(gtarih + "/" + yiltemp);
                            }
                            catch
                            {
                                try
                                {
                                    var AyIcindeIseGirisTarihi = DateTime.FromOADate(Convert.ToDouble(gtarih));
                                }
                                catch
                                {
                                    hatavar = true;
                                }
                            }
                        }



                        string cikistarih = row[(int)Enums.AphbHucreBilgileri.CikisGunu].ToString().Trim();

                        if (!String.IsNullOrEmpty(cikistarih))
                        {
                            try
                            {
                                var AyIcindeIstenCikisTarihi = Convert.ToDateTime(cikistarih + "/" + yiltemp);

                            }
                            catch
                            {
                                try
                                {
                                    var AyIcindeIstenCikisTarihi = DateTime.FromOADate(Convert.ToDouble(cikistarih));
                                }
                                catch
                                {

                                    hatavar = true;

                                }
                            }
                        }

                        string EksikGunSayisi = row[(int)Enums.AphbHucreBilgileri.EksikGun].ToString().Trim();

                        if (!String.IsNullOrEmpty(EksikGunSayisi)) Convert.ToInt32(EksikGunSayisi);

                        int gun = Convert.ToInt32(row[(int)Enums.AphbHucreBilgileri.Gun].ToString().Trim());

                        string Araci = row[(int)Enums.AphbHucreBilgileri.Araci].ToString();

                        if (String.IsNullOrEmpty(Araci)) hatavar = true;


                    }
                    catch
                    {
                        hatavar = true;
                    }

                    if (hatavar)
                    {
                        if (dthatalisatirlar == null)
                        {
                            dthatalisatirlar = new DataTable();

                            dthatalisatirlar.Columns.Add("Sube");
                            dthatalisatirlar.Columns.Add("ExcelSatirNo", typeof(int));

                            foreach (DataColumn col in dtaylikliste.Columns)
                            {
                                dthatalisatirlar.Columns.Add(col.ColumnName, col.DataType);
                            }

                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.Yil + 2].ColumnName = "YIL";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.Ay + 2].ColumnName = "AY";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.Kanun + 2].ColumnName = "KANUN";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.Mahiyet + 2].ColumnName = "MAHİYETİ";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.BelgeTuru + 2].ColumnName = "BELGE TÜRÜ";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.SiraNo + 2].ColumnName = "SIRA NO";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo + 2].ColumnName = "S.GÜVENLİK NO";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.Ad + 2].ColumnName = "ADI";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.Soyad + 2].ColumnName = "SOYADI";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.IlkSoyadi + 2].ColumnName = "İLK SOYADI";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.Ucret + 2].ColumnName = "ÜCRET_TL";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.Ikramiye + 2].ColumnName = "İKRAMİYE_TL";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.Gun + 2].ColumnName = "GÜN";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.UCG + 2].ColumnName = "UÇG";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.EksikGun + 2].ColumnName = "EKSİK GÜN SAYISI";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.GirisGunu + 2].ColumnName = "GİRİŞ GÜNÜ";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.CikisGunu + 2].ColumnName = "ÇIKIŞ GÜNÜ";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.EksikGunSebebi + 2].ColumnName = "EGS";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.IstenCikisNedeni + 2].ColumnName = "İÇN";
                            dthatalisatirlar.Columns[(int)Enums.AphbHucreBilgileri.MeslekKod + 2].ColumnName = "MESLEK KOD";

                        }

                        DataRow r = dthatalisatirlar.NewRow();

                        for (int i = 0; i < dtaylikliste.Columns.Count; i++)
                        {
                            r[i + 2] = row[i];
                        }

                        r["Sube"] = isyeri.SubeAdi;
                        r["ExcelSatirNo"] = rownum;

                        dthatalisatirlar.Rows.Add(r);

                    }

                    rownum++;
                }

                #endregion
            }

            if (BasvuruFormuKontrolEt)
            {
                #region Başvuru Formu Hatalı Satırlar Kontrolü

                if (dsbasvurulistesi.Tables.Count > 0)
                {

                    foreach (var kanun in TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar)
                    {
                        DataTable dtbasvuru = dsbasvurulistesi.Tables.Contains(kanun) ? dsbasvurulistesi.Tables[kanun] : null;

                        if (kanun.Equals("7166"))
                        {


                            if (dsbasvurulistesi.Tables.Contains("7103"))
                            {
                                var dt7103 = dsbasvurulistesi.Tables["7103"];

                                dtbasvuru = dt7103.Clone();

                                var rows = dt7103.AsEnumerable().Where(p => Sabitler.BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.UcretDestegiTercihi7103].ToString().Trim().Equals("İSTİYOR")).ToList();

                                foreach (var row in rows)
                                {
                                    var newRow = dtbasvuru.NewRow();

                                    for (int i = 0; i < dtbasvuru.Columns.Count; i++)
                                    {
                                        newRow[i] = row[i];
                                    }
                                }

                            }
                        }

                        if (dtbasvuru != null)
                        {
                            bool v2_6111mi = kanun.Equals("6111") && dtbasvuru.Columns.Contains("İşten Ayrılış Tarihi");

                            var sutunlar = v2_6111mi ? Sabitler.BasvuruFormlariSutunlari["6111-v2"] : (kanun.Equals("6111") ? Sabitler.BasvuruFormlariSutunlari["6111-v1"] : Sabitler.BasvuruFormlariSutunlari[kanun]);

                            for (int j = 0; j < dtbasvuru.Rows.Count; j++)
                            {
                                try
                                {

                                    if (sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.Giris))
                                    {
                                        DateTime giristarihi = Convert.ToDateTime(dtbasvuru.Rows[j][sutunlar[Enums.BasvuruFormuSutunTurleri.Giris]].ToString());
                                    }

                                    if (sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.Baz))
                                    {
                                        int baz = Convert.ToInt32(dtbasvuru.Rows[j][sutunlar[Enums.BasvuruFormuSutunTurleri.Baz]].ToString().Replace(".", ""));
                                    }

                                    if (sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.TesvikSuresi6111v1))
                                    {
                                        var tesviksuresi = dtbasvuru.Rows[j][sutunlar[Enums.BasvuruFormuSutunTurleri.TesvikSuresi6111v1]].ToString();

                                        DateTime tesvikbaslangic = Convert.ToDateTime(tesviksuresi.Split('-')[0].Trim());

                                        DateTime tesvikbitis = Convert.ToDateTime(tesviksuresi.Split('-')[1].Trim());
                                    }

                                    if (sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.TesvikBaslangic))
                                    {
                                        DateTime tarih = Convert.ToDateTime(dtbasvuru.Rows[j][sutunlar[Enums.BasvuruFormuSutunTurleri.TesvikBaslangic]].ToString().Trim());
                                    }

                                    if (sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.TesvikBitis))
                                    {
                                        if (kanun.Equals("14857"))
                                        {
                                            string tesviksuresibitis = dtbasvuru.Rows[j][sutunlar[Enums.BasvuruFormuSutunTurleri.TesvikBitis]].ToString().Trim().Replace(" ", "/");

                                            if (!String.IsNullOrEmpty(tesviksuresibitis))
                                            {
                                                DateTime tesvikbitis = Convert.ToDateTime(tesviksuresibitis);
                                            }
                                        }
                                        else
                                        {
                                            DateTime tarih = Convert.ToDateTime(dtbasvuru.Rows[j][sutunlar[Enums.BasvuruFormuSutunTurleri.TesvikBitis]].ToString().Trim());
                                        }
                                    }

                                    if (sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.Cikis))
                                    {
                                        string cikistarihi = dtbasvuru.Rows[j][sutunlar[Enums.BasvuruFormuSutunTurleri.Cikis]].ToString();

                                        if (!String.IsNullOrEmpty(cikistarihi))
                                        {
                                            DateTime ctarih = Convert.ToDateTime(cikistarihi);
                                        }
                                    }

                                    if (sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi))
                                    {
                                        string tanimlamatarihi = dtbasvuru.Rows[j][sutunlar[Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi]].ToString();

                                        if (kanun.Equals("7103"))
                                        {
                                            DateTime tarih = Convert.ToDateTime(tanimlamatarihi);
                                        }

                                    }
                                }
                                catch
                                {
                                    DataTable dthatalisatirlarbasvuru = BasvuruFormlariHataliSatirlar[kanun];

                                    if (dthatalisatirlarbasvuru == null)
                                    {
                                        dthatalisatirlarbasvuru = new DataTable();

                                        dthatalisatirlarbasvuru.Columns.Add("Sube");
                                        dthatalisatirlarbasvuru.Columns.Add("ExcelSatirNo", typeof(int));


                                        foreach (DataColumn col in dtbasvuru.Columns)
                                        {
                                            dthatalisatirlarbasvuru.Columns.Add(col.ColumnName, col.DataType);
                                        }

                                        BasvuruFormlariHataliSatirlar[kanun] = dthatalisatirlarbasvuru;
                                    }

                                    DataRow r = dthatalisatirlarbasvuru.NewRow();

                                    for (int i = 0; i < dtbasvuru.Columns.Count; i++)
                                    {
                                        r[i + 2] = dtbasvuru.Rows[j][i];
                                    }

                                    r["Sube"] = isyeri.SubeAdi;

                                    r["ExcelSatirNo"] = j + 2;

                                    dthatalisatirlarbasvuru.Rows.Add(r);

                                }
                            }
                        }
                    }
                }
                #endregion
            }

        }



    }



}
