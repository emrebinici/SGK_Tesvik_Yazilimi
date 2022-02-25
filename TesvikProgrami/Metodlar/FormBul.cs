using System;
using System.IO;
using System.Linq;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static string FormBul(Isyerleri isyeri, Enums.FormTuru formTuru)
        {
            var isyeripath = IsyeriKlasorBul(isyeri);

            string file = null;

            switch (formTuru)
            {
                case Enums.FormTuru.BasvuruFormu:
                    file = isyeri.BasvuruFormu;
                    break;
                case Enums.FormTuru.Aphb:
                    file = isyeri.Aphb;
                    break;
                case Enums.FormTuru.BasvuruListesi7166:
                    file = isyeri.BasvuruListesi7166;
                    break;
                case Enums.FormTuru.Kisiler:
                    file = "Kisiler.xlsx";
                    break;
                case Enums.FormTuru.CariAphb:
                    file = "Cari Aphb.xlsx";
                    break;
                case Enums.FormTuru.EgitimListesi:
                    file = "Eğitim Belgesi Verilecekler.xlsx";
                    break;
                case Enums.FormTuru.EmanetTahsilatlari:
                    file = "Emanet Tahsilatları.xlsx";
                    break;
                case Enums.FormTuru.BildirgelerinIcmali:
                    file = "Bildirgelerin İcmali.xlsx";
                    break;
                case Enums.FormTuru.Liste7252:
                    file = "7252 teşvikine tanımlananların listesi.xlsx";
                    break;
                case Enums.FormTuru.Yersiz7252:
                    file = "Yersiz 7252.xlsx";
                    break;
                default:
                    break;
            }

            if (isyeripath != null && file != null)
            {
                if (formTuru == Enums.FormTuru.Kisiler)
                {
                    var dosyalar = Directory.GetFiles(isyeripath, "Kişiler-*.xlsx");

                    if (dosyalar.Length > 0)
                    {
                        var sonTarih = dosyalar.Select(p => System.IO.Path.GetFileNameWithoutExtension(p).Split('-')).Max(p => new DateTime(p[1].ToInt(), p[2].ToInt(), 1));

                        return Path.Combine(isyeripath, String.Format("Kişiler-{0}-{1}.xlsx", sonTarih.Year, sonTarih.Month));
                    }
                }
                else if (formTuru == Enums.FormTuru.CariAphb)
                {
                    var dosyalar = Directory.GetFiles(isyeripath, "Cari Aphb-*.xlsx");

                    if (dosyalar.Length > 0)
                    {
                        var sonTarih = dosyalar.Select(p => System.IO.Path.GetFileNameWithoutExtension(p).Split('-')).Max(p => new DateTime(p[1].ToInt(), p[2].ToInt(), 1));

                        return Path.Combine(isyeripath, String.Format("Cari Aphb-{0}-{1}.xlsx", sonTarih.Year, sonTarih.Month));
                    }
                }
                else if (formTuru == Enums.FormTuru.EgitimListesi)
                {
                    var dosyalar = Directory.GetFiles(isyeripath, "*Eğitim Belgesi Verilecekler.xlsx");

                    if (dosyalar.Length > 0)
                    {
                        return dosyalar[0];
                    }
                }
                else if (formTuru == Enums.FormTuru.EmanetTahsilatlari)
                {
                    var dosyalar = Directory.GetFiles(isyeripath, "Emanet*.xlsx");

                    if (dosyalar.Length > 0)
                    {
                        return dosyalar[0];
                    }
                }
                else if (formTuru == Enums.FormTuru.BildirgelerinIcmali)
                {
                    var dosyalar = Directory.GetFiles(isyeripath, "Bildirgelerin İcmali*.xlsx");

                    if (dosyalar.Length > 0)
                    {
                        return dosyalar[0];
                    }
                }
                else if (formTuru == Enums.FormTuru.Liste7252)
                {
                    var dosyalar = Directory.GetFiles(isyeripath, "7252 teşvikine tanımlananların listesi*.xlsx");

                    if (dosyalar.Length > 0)
                    {
                        return dosyalar[0];
                    }
                }
                else if (formTuru == Enums.FormTuru.Yersiz7252)
                {
                    var dosyalar = Directory.GetFiles(isyeripath, "Yersiz 7252*.xlsx");

                    if (dosyalar.Length > 0)
                    {
                        return dosyalar[0];
                    }
                }
                else
                {
                    if (File.Exists(Path.Combine(isyeripath, file)))
                    {
                        return Path.Combine(isyeripath, file);
                    }
                }

            }

            return null;
        }
        public static string FormBul(IsyeriListesiDTO isyeri, Enums.FormTuru formTuru)
        {
            var isyeripath = IsyeriKlasorBul(isyeri);

            string file = null;

            switch (formTuru)
            {
                case Enums.FormTuru.BasvuruFormu:
                    file = isyeri.BasvuruFormu;
                    break;
                case Enums.FormTuru.Aphb:
                    file = isyeri.APHB;
                    break;
                case Enums.FormTuru.BasvuruListesi7166:
                    file = isyeri.BasvuruListe7166;
                    break;
                case Enums.FormTuru.Kisiler:
                    file = "Kisiler.xlsx";
                    break;
                case Enums.FormTuru.CariAphb:
                    file = "Cari Aphb.xlsx";
                    break;
                case Enums.FormTuru.EgitimListesi:
                    file = "Eğitim Belgesi Verilecekler.xlsx";
                    break;
                case Enums.FormTuru.EmanetTahsilatlari:
                    file = "Emanet Tahsilatları.xlsx";
                    break;
                case Enums.FormTuru.BildirgelerinIcmali:
                    file = "Bildirgelerin İcmali.xlsx";
                    break;
                case Enums.FormTuru.Liste7252:
                    file = "7252 teşvikine tanımlananların listesi.xlsx";
                    break;
                case Enums.FormTuru.Yersiz7252:
                    file = "Yersiz 7252.xlsx";
                    break;
                default:
                    break;
            }

            if (isyeripath != null && file != null)
            {
                if (formTuru == Enums.FormTuru.Kisiler)
                {
                    var dosyalar = Directory.GetFiles(isyeripath, "Kişiler-*.xlsx");

                    if (dosyalar.Length > 0)
                    {
                        var sonTarih = dosyalar.Select(p => System.IO.Path.GetFileNameWithoutExtension(p).Split('-')).Max(p => new DateTime(p[1].ToInt(), p[2].ToInt(), 1));

                        return Path.Combine(isyeripath, String.Format("Kişiler-{0}-{1}.xlsx", sonTarih.Year, sonTarih.Month));
                    }
                }
                else if (formTuru == Enums.FormTuru.CariAphb)
                {
                    var dosyalar = Directory.GetFiles(isyeripath, "Cari Aphb-*.xlsx");

                    if (dosyalar.Length > 0)
                    {
                        var sonTarih = dosyalar.Select(p => System.IO.Path.GetFileNameWithoutExtension(p).Split('-')).Max(p => new DateTime(p[1].ToInt(), p[2].ToInt(), 1));

                        return Path.Combine(isyeripath, String.Format("Cari Aphb-{0}-{1}.xlsx", sonTarih.Year, sonTarih.Month));
                    }
                }
                else if (formTuru == Enums.FormTuru.EgitimListesi)
                {
                    var dosyalar = Directory.GetFiles(isyeripath, "*Eğitim Belgesi Verilecekler.xlsx");

                    if (dosyalar.Length > 0)
                    {
                        return dosyalar[0];
                    }
                }
                else if (formTuru == Enums.FormTuru.EmanetTahsilatlari)
                {
                    var dosyalar = Directory.GetFiles(isyeripath, "Emanet*.xlsx");

                    if (dosyalar.Length > 0)
                    {
                        return dosyalar[0];
                    }
                }
                else if (formTuru == Enums.FormTuru.BildirgelerinIcmali)
                {
                    var dosyalar = Directory.GetFiles(isyeripath, "Bildirgelerin İcmali*.xlsx");

                    if (dosyalar.Length > 0)
                    {
                        return dosyalar[0];
                    }
                }
                else if (formTuru == Enums.FormTuru.Liste7252)
                {
                    var dosyalar = Directory.GetFiles(isyeripath, "7252 teşvikine tanımlananların listesi*.xlsx");

                    if (dosyalar.Length > 0)
                    {
                        return dosyalar[0];
                    }
                }
                else if (formTuru == Enums.FormTuru.Yersiz7252)
                {
                    var dosyalar = Directory.GetFiles(isyeripath, "Yersiz 7252*.xlsx");

                    if (dosyalar.Length > 0)
                    {
                        return dosyalar[0];
                    }
                }
                else
                {
                    if (File.Exists(Path.Combine(isyeripath, file)))
                    {
                        return Path.Combine(isyeripath, file);
                    }
                }

            }

            return null;
        }



    }



}
