using System;
using System.Collections.Generic;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static decimal TesvikTutariHesapla(string Kanun, int Gun, decimal toplamUcret, int yil, int ay, string belgeTuru, string IsyeriSicilNo,Dictionary<string,Classes.Tesvik> TumTesvikler = null, decimal CarpimOrani687 = -1)
        {
            decimal result = 0;

            if (TumTesvikler == null) TumTesvikler = Program.TumTesvikler;

            Kanun = Kanun.PadLeft(5, '0');

            if (Kanun.Equals("05510"))
            {
                result = toplamUcret * 0.05m;
            }
            else if (Kanun.EndsWith("6486"))
            {
                result = Gun * Metodlar.AsgariUcretBul(yil, ay) * TesvikHesaplamaSabitleri.AsgariUcretCarpimOrani;
                //2021 12.ay dahil sonrasında 6486 dahil edilmiyor
                if ((yil >= 2022 && ay >= 1) || (yil == 2021 && ay == 12))
                {
                    result = 0;

                }
            }
            else if (Kanun.Equals("06645") || Kanun.Equals("16322") || Kanun.Equals("25510"))
            {
                result = Gun * Metodlar.AsgariUcretBul(yil, ay) * (TumTesvikler["6645"].BelgeTuruOranBul(yil, ay, belgeTuru, IsyeriSicilNo) - 5) / 100;
            }
            else if (Kanun.Equals("06111"))
            {
                //result = toplamUcret * (TumTesvikler["6111"].BelgeTuruOranBul(yil, ay, belgeTuru, IsyeriSicilNo) - 5) / 100;
                result = toplamUcret * (TumTesvikler["6111"].BelgeTuruOranBul(yil, ay, belgeTuru, IsyeriSicilNo) - 5) / 100;

            }
            else if (Kanun.Equals("00687"))
            {
                var tesvik = TumTesvikler["687"];

                if (CarpimOrani687 == -1)
                {
                    CarpimOrani687 = TesvikHesaplamaSabitleri.CarpimOrani687;

                    if (tesvik.dtKurulusTarihi >= new DateTime(2017, 1, 1) || !tesvik.BildirgeOlanYillar.Contains(2016)) CarpimOrani687 = CarpimOrani687 / 2;
                }

                result = Gun * CarpimOrani687;
            }
            else if (Kanun.Equals("01687"))
            {
                result = Gun * TesvikHesaplamaSabitleri.CarpimOrani687;
            }
            else if (Kanun.Equals("17103"))
            {
                var tesvik = TumTesvikler["7103"];

                decimal gunlukkazanc = 0;

                if (Gun > 0)
                {
                    gunlukkazanc = toplamUcret / Gun;
                }

                decimal GunlukKazancSiniri = tesvik.GunlukKazancSiniriGetir(yil, ay, IsyeriSicilNo);

                bool GunlukKazancSiniriGecti = gunlukkazanc > GunlukKazancSiniri;

                var belgeTuruOrani = tesvik.BelgeTuruOranBul(yil, ay, belgeTuru, IsyeriSicilNo);

                if (GunlukKazancSiniriGecti)
                {
                    belgeTuruOrani = tesvik.BelgeTuruOranBul(yil, ay, "1", IsyeriSicilNo);
                }

                if (GunlukKazancSiniriGecti)
                {
                    result = Gun * GunlukKazancSiniri * belgeTuruOrani / 100;
                }
                else
                {
                    result = toplamUcret * belgeTuruOrani / 100;
                }
            }
            else if (Kanun.Equals("27103"))
            {
                result = Gun * Metodlar.AsgariUcretBul(yil, ay) * TumTesvikler["7103"].BelgeTuruOranBul(yil, ay, belgeTuru, IsyeriSicilNo) / 100;
            }
            else if (Kanun.Equals("02828"))
            {
                result = Gun * Metodlar.AsgariUcretBul(yil, ay) * (TumTesvikler["2828"].BelgeTuruOranBul(yil, ay, belgeTuru, IsyeriSicilNo) - 5) / 100;
            }
            else if (Kanun.Equals("14857"))
            {
                result = Gun * Metodlar.AsgariUcretBul(yil, ay) * (TumTesvikler["14857"].BelgeTuruOranBul(yil, ay, belgeTuru, IsyeriSicilNo) - 5) / 100;
            }
            else if (Kanun.Equals("85615") || Kanun.Equals("85084"))
            {
                result = Gun * Metodlar.AsgariUcretBul(yil, ay) * (TumTesvikler["14857"].BelgeTuruOranBul(yil, ay, belgeTuru, IsyeriSicilNo)) / 100 * 0.8m;
            }
            else if (Kanun.Equals("05615") || Kanun.Equals("05084"))
            {
                result = Gun * Metodlar.AsgariUcretBul(yil, ay) * (TumTesvikler["14857"].BelgeTuruOranBul(yil, ay, belgeTuru, IsyeriSicilNo)) / 100;
            }
            else if (Kanun.EndsWith("07166"))
            {
                result = Gun * TesvikHesaplamaSabitleri.CarpimMiktari7166;
            }
            else if (Kanun.Equals("26322"))
            {
                var BelgeTuruOraniHesaplamadaEklenecekAlanlar = new List<string> {
                    "MalulYaslilikOraniSigortali",
                    "MalulYaslilikOraniIsveren",
                    "GenelSaglikSigortali",
                    "GenelSaglikIsveren",
                    "SosyalDestekSigortali",
                    "SosyalDestekIsveren"
                };

                result = Gun * Metodlar.AsgariUcretBul(yil, ay) * (Metodlar.BelgeTuruOranBul(yil, ay, belgeTuru, IsyeriSicilNo, BelgeTuruOraniHesaplamadaEklenecekAlanlar) - 5) / 100;
            }
            else if (Kanun.EndsWith("07252"))
            {
                result = Gun * Metodlar.AsgariUcretBul(yil, ay) * TumTesvikler["7252"].BelgeTuruOranBul(yil, ay, belgeTuru, IsyeriSicilNo) / 100;
            }
            else if (Kanun.EndsWith("17256"))
            {
                var tarih = new DateTime(yil, ay, 1);

                if (tarih >= new DateTime(2021, 1, 1))
                    result = Gun * TesvikHesaplamaSabitleri.CarpimSabiti7256_2021OcakVeSonrasi;
                else
                    result = Gun * TesvikHesaplamaSabitleri.CarpimSabiti7256_2021OcakOncesi;


            }
            else if (Kanun.EndsWith("27256"))
            {
                var tarih = new DateTime(yil, ay, 1);

                if (tarih >= new DateTime(2021, 1, 1))
                    result = Gun * TesvikHesaplamaSabitleri.CarpimSabiti7256_2021OcakVeSonrasi;
                else
                    result = Gun * TesvikHesaplamaSabitleri.CarpimSabiti7256_2021OcakOncesi;
            }
            else if (Kanun.EndsWith("07316"))
            {
                result = Gun * Metodlar.AsgariUcretBul(yil, ay) * TumTesvikler["7316"].BelgeTuruOranBul(yil, ay, belgeTuru, IsyeriSicilNo) / 100;
            }
            else if (Kanun.Equals("03294"))
            {
                result = Gun * Metodlar.AsgariUcretBul(yil, ay) * (TumTesvikler["3294"].BelgeTuruOranBul(yil, ay, belgeTuru, IsyeriSicilNo) - 5) / 100;
            }
            else if (Kanun.Equals("00000"))
            {
                result = 0m;
            }
            else
            {
                throw new Exception(Kanun + " kanun nolu teşviğin tutarının hesaplanması programda kayıtlı değil");
            }

            return result;
        }

    }



}
