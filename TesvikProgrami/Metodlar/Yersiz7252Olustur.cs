using System;
using System.Data;
using System.Linq;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static Classes.Yersiz7252BulSonuc Yersiz7252Olustur(Isyerleri isyeri)
        {
            var sonuc = new Classes.Yersiz7252BulSonuc();

            var aphbyol = Metodlar.FormBul(isyeri, Enums.FormTuru.Aphb);
            var bfyol = Metodlar.FormBul(isyeri, Enums.FormTuru.BasvuruFormu);

            if (string.IsNullOrEmpty(aphbyol)) return new Classes.Yersiz7252BulSonuc { Durum = false, HataMesaji = "Aphb dosyası bulunamadı" };
            if (string.IsNullOrEmpty(bfyol)) return new Classes.Yersiz7252BulSonuc { Durum = false, HataMesaji = "Başvuru formu bulunamadı" };

            var dtaylikliste = Metodlar.AylikListeyiYukle(aphbyol);
            var dsBasvurular = Metodlar.BasvuruListesiniYukle(bfyol);
            var tumkisiler= Metodlar.TumKisileriGetir(dtaylikliste);

            if (dsBasvurular.Tables.IndexOf("7252") == -1) return new Classes.Yersiz7252BulSonuc { Durum = false, HataMesaji = "7252 başvuru formu bulunamadı" };

            var araci = "ana işveren";
            
            if (isyeri.TaseronNo.ToInt() > 0)
            {
                araci = isyeri.TaseronNo.ToInt().ToString().PadLeft(3,'0');
            }

            var onceden7252Verilenler = dtaylikliste
                .AsEnumerable()
                .Where(row => row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().EndsWith("7252") && row[(int)Enums.AphbHucreBilgileri.Araci].ToString().ToLower().StartsWith(araci))
                .Select(row=> row[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString().Trim())
                .Distinct()
                .ToList();

            if (onceden7252Verilenler.Count == 0) return new Classes.Yersiz7252BulSonuc { Durum = false, HataMesaji = "Yersiz 7252 teşviki bulunmamaktadır" };

            var dt7252Basvuru = dsBasvurular.Tables["7252"];

            var basvuruKisiler7252= dt7252Basvuru
                .AsEnumerable()
                .GroupBy(row => row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString())
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(row => new Classes.BasvuruKisi
                    {
                        TcKimlikNo = row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString(),
                        Ad = row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.Ad]].ToString(),
                        Soyad = row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.Soyad]].ToString(),
                        TesvikDonemiBaslangic = Convert.ToDateTime(row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.TesvikBaslangic]].ToString()),
                        TesvikDonemiBitis = Convert.ToDateTime(row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.TesvikBitis]].ToString()),
                        GirisTarihi = Convert.ToDateTime(row[Sabitler.BasvuruFormlariSutunlari["7252"][Enums.BasvuruFormuSutunTurleri.Giris]].ToString()),
                    })
            );

            DataTable sonuclar = new DataTable("Yersiz 7252 Listesi");
            sonuclar.Columns.Add("SiraNo");
            sonuclar.Columns.Add("TcKimlikNo");
            sonuclar.Columns.Add("Ad");
            sonuclar.Columns.Add("Soyad");
            sonuclar.Columns.Add("2020_8");
            sonuclar.Columns.Add("2020_9");
            sonuclar.Columns.Add("2020_10");
            sonuclar.Columns.Add("2020_11");
            sonuclar.Columns.Add("2020_12");
            sonuclar.Columns.Add("2021_1");
            sonuclar.Columns.Add("2021_2");
            sonuclar.Columns.Add("2021_3");
            sonuclar.Columns.Add("2021_4");
            sonuclar.Columns.Add("2021_5");
            sonuclar.Columns.Add("2021_6");

            var tarihBaslangic = new DateTime(2020, 8, 1);
            var tarihBitis = new DateTime(2021, 6, 1);

            var siraNo = 1;

            foreach (var tc in onceden7252Verilenler)
            {
                var kisi = tumkisiler.TumKisiler[tc];

                DataRow kisiNewRow = null; 

                kisi.KisiBasvuruKayitlari.Add("7252", new System.Collections.Generic.List<Classes.BasvuruKisi>());

                if (basvuruKisiler7252.ContainsKey(tc))
                {
                    kisi.KisiBasvuruKayitlari["7252"].AddRange(basvuruKisiler7252[tc]);
                }

                var kisiAylar = tumkisiler.KisilerinSatirlari[tc];

                for (int i = 0; i <= 10; i++)
                {
                    var ay = tarihBaslangic.AddMonths(i);

                    var ayKey = ay.Year.ToString() + "-" + ay.Month.ToString();
                    var ayKey2 = ay.Year.ToString() + "_" + ay.Month.ToString();

                    if (kisiAylar.ContainsKey(ayKey))
                    {
                        var aySatirlari = kisiAylar[ayKey];

                        var satir7252 = aySatirlari.FirstOrDefault(row => row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().EndsWith("7252") && row[(int)Enums.AphbHucreBilgileri.Araci].ToString().ToLower().StartsWith(araci));

                        if (satir7252 != null)
                        {
                            var aktifBasvuru = Metodlar.AktifBasvuruKaydiniGetir(kisi, "7252", ay.Year, ay.Month);

                            if (aktifBasvuru == null)
                            {
                                if (kisiNewRow == null)
                                {
                                    kisiNewRow = sonuclar.NewRow();

                                    kisiNewRow["SiraNo"] = siraNo++;
                                    kisiNewRow["TcKimlikNo"] = tc;
                                    kisiNewRow["Ad"] = satir7252[(int)Enums.AphbHucreBilgileri.Ad];
                                    kisiNewRow["Soyad"] = satir7252[(int)Enums.AphbHucreBilgileri.Soyad];
                                }

                                kisiNewRow[ayKey2] = "X";
                            }
                        }
                    }
                }

                if (kisiNewRow != null)
                    sonuclar.Rows.Add(kisiNewRow);
               
            }

            if (sonuclar.Rows.Count == 0) return new Classes.Yersiz7252BulSonuc { Durum = false, HataMesaji = "Yersiz 7252 teşviki bulunmamaktadır" };

            sonuc.Kisiler = sonuclar;

            var kaydedilenDosyaYolu= Metodlar.Yersiz7252Kaydet(isyeri, sonuclar);

            if (string.IsNullOrEmpty(kaydedilenDosyaYolu))
            {
                sonuc.Durum = false;
                sonuc.HataMesaji = "Yersiz 7252 listesi kaydedilemedi";

            }
            else
                sonuc.Result = kaydedilenDosyaYolu;

            return sonuc;
        }



    }



}
