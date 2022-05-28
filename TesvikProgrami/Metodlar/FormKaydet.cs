using System;
using System.Data;
using System.IO;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static string FormKaydet(Isyerleri isyeri, string fileName, Enums.FormTuru formTuru)
        {
            var isyeripath = IsyeriKlasorBul(isyeri, true);

            string eskiyol = FormBul(isyeri, formTuru);

            string YeniPath = Path.Combine(isyeripath, Path.GetFileName(fileName));

            bool basarili = false;

            try
            {
                if (File.Exists(YeniPath))
                {
                    File.Copy(fileName, YeniPath, true);

                    basarili = true;
                }
                else
                {
                    File.Copy(fileName, YeniPath);

                    basarili = true;

                    if (!String.IsNullOrEmpty(eskiyol)) File.Delete(eskiyol);
                }


            }
            catch (Exception)
            {

            }

            if (basarili) return "OK";

            return null;
        }

        public static string FormKaydet(Isyerleri isyeri, DataTable dt, DataTable dtEski, Enums.FormTuru formTuru, string KanunNo)
        {
            string eskiyol = Metodlar.FormBul(isyeri, formTuru);

            var isyeripath = Metodlar.IsyeriKlasorBul(isyeri, true);

            string YeniPath = Path.Combine(isyeripath, eskiyol != null ? Path.GetFileName(eskiyol) : formTuru == Enums.FormTuru.BasvuruFormu ? "Başvuru Formu.xlsx" : formTuru == Enums.FormTuru.Aphb ? "Aphb.xlsx" : "7166 Listesi.xlsx");

            bool basarili = false;

            try
            {
                Classes.ExceleYaz excel = new Classes.ExceleYaz();

                if (File.Exists(YeniPath))
                {

                    int i = 1;

                    while (!basarili)
                    {
                        if (i > 1)
                            YeniPath = Path.GetFileNameWithoutExtension(YeniPath) + i.ToString() + Path.GetExtension(YeniPath);

                        basarili = formTuru == Enums.FormTuru.BasvuruFormu ? excel.BasvuruOkuVeYaz(dt, YeniPath, KanunNo) :
                                   formTuru == Enums.FormTuru.Aphb ? excel.OkuVeYaz(dt, eskiyol, YeniPath, dtEski)
                                                             : excel.BasvuruListesi7166Kaydet(dt, YeniPath);

                        i++;
                    }

                }
                else
                {
                    if (formTuru == Enums.FormTuru.BasvuruFormu) excel.BasvuruOkuVeYaz(dt, YeniPath, KanunNo);
                    else if (formTuru == Enums.FormTuru.Aphb) excel.OkuVeYaz(dt, eskiyol, YeniPath, dtEski);
                    else excel.BasvuruListesi7166Kaydet(dt, YeniPath);
                }

                using (var dbContext = new DbEntities())
                {
                    isyeri = dbContext.Isyerleri.Find(isyeri.IsyeriID);

                    if (isyeri != null)
                    {
                        if (formTuru == Enums.FormTuru.BasvuruFormu)
                            isyeri.BasvuruFormu = Path.GetFileName(YeniPath);
                        else if (formTuru == Enums.FormTuru.Aphb)
                            isyeri.Aphb = Path.GetFileName(YeniPath);
                        else
                            isyeri.BasvuruListesi7166 = Path.GetFileName(YeniPath);

                        dbContext.SaveChanges();
                    }

                }

                return YeniPath;

            }
            catch (Exception ex)
            {
                string Mesaj = (formTuru == Enums.FormTuru.BasvuruFormu ? KanunNo + " başvuru formu" : formTuru == Enums.FormTuru.Aphb ? "Aphb dosyası" : "7166 listesi") + " hata nedeniyle kaydedilemedi" + Environment.NewLine;

                HataMesajiGoster(ex, Mesaj);
            }

            return null;
        }

    }

}
