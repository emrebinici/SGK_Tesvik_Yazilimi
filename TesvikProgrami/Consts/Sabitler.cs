using System;
using System.Collections.Generic;
using System.Text;

namespace TesvikProgrami
{
    public static class Sabitler
    {
        public const string xmlpath = "veri.xml";

        public const string KisaVadeliSigortaPrimKoluOranlariPath = "KisaVadeliSigortaPrimKoluOranlari.xml";

        public static Dictionary<string, Dictionary<Enums.BasvuruFormuSutunTurleri, int>> BasvuruFormlariSutunlari = new Dictionary<string, Dictionary<Enums.BasvuruFormuSutunTurleri, int>>
        {
            { "6111-v1",new Dictionary<Enums.BasvuruFormuSutunTurleri, int> {
                            { Enums.BasvuruFormuSutunTurleri.TcKimlikNo, 0},
                            { Enums.BasvuruFormuSutunTurleri.Sicil, 1 } ,
                            { Enums.BasvuruFormuSutunTurleri.Ad , 2} ,
                            { Enums.BasvuruFormuSutunTurleri.Soyad,3},
                            { Enums.BasvuruFormuSutunTurleri.DogumTarihi,4},
                            { Enums.BasvuruFormuSutunTurleri.Cinsiyet,5 },
                            { Enums.BasvuruFormuSutunTurleri.TesvikSuresi6111v1,6},
                            { Enums.BasvuruFormuSutunTurleri.Baz,7},
                            { Enums.BasvuruFormuSutunTurleri.Giris,8 },
                            { Enums.BasvuruFormuSutunTurleri.OnayDurumu,9 },
                            { Enums.BasvuruFormuSutunTurleri.GirisAyi,10 }
                        }
            },
            { "6111-v2",new Dictionary<Enums.BasvuruFormuSutunTurleri, int> {
                            { Enums.BasvuruFormuSutunTurleri.TcKimlikNo, 0},
                            { Enums.BasvuruFormuSutunTurleri.Sicil, 1 } ,
                            { Enums.BasvuruFormuSutunTurleri.Ad , 2} ,
                            { Enums.BasvuruFormuSutunTurleri.Soyad,3},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBaslangic,4},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBitis,5 },
                            { Enums.BasvuruFormuSutunTurleri.Baz,6},
                            { Enums.BasvuruFormuSutunTurleri.Giris,7},
                            { Enums.BasvuruFormuSutunTurleri.Cikis,8 },
                            { Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi,9 },
                            { Enums.BasvuruFormuSutunTurleri.OnayDurumu,10 },
                            { Enums.BasvuruFormuSutunTurleri.Araci, 11}
                        }
            },
            { "6645",new Dictionary<Enums.BasvuruFormuSutunTurleri, int> {
                            { Enums.BasvuruFormuSutunTurleri.TcKimlikNo, 0},
                            { Enums.BasvuruFormuSutunTurleri.Sicil,1},
                            { Enums.BasvuruFormuSutunTurleri.Ad , 2} ,
                            { Enums.BasvuruFormuSutunTurleri.Soyad,3},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBaslangic,4},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBitis,5},
                            { Enums.BasvuruFormuSutunTurleri.Baz,6},
                            { Enums.BasvuruFormuSutunTurleri.AktifMi,7 },
                            { Enums.BasvuruFormuSutunTurleri.Giris,8 },
                            { Enums.BasvuruFormuSutunTurleri.IslemTarihi,9},
                            { Enums.BasvuruFormuSutunTurleri.Durum , 10},
                            { Enums.BasvuruFormuSutunTurleri.Araci, 11 }

                        }
            },
            { "687",new Dictionary<Enums.BasvuruFormuSutunTurleri, int> {
                        { Enums.BasvuruFormuSutunTurleri.TcKimlikNo, 0},
                        { Enums.BasvuruFormuSutunTurleri.Ad , 1} ,
                        { Enums.BasvuruFormuSutunTurleri.Soyad,2},
                        { Enums.BasvuruFormuSutunTurleri.TesvikBaslangic,3},
                        { Enums.BasvuruFormuSutunTurleri.TesvikBitis,4},
                        { Enums.BasvuruFormuSutunTurleri.Giris,5 },
                        { Enums.BasvuruFormuSutunTurleri.Baz,6},
                        { Enums.BasvuruFormuSutunTurleri.AktifMi,7},
                        { Enums.BasvuruFormuSutunTurleri.KanunNo,8},
                        { Enums.BasvuruFormuSutunTurleri.Durum , 9},
                        { Enums.BasvuruFormuSutunTurleri.Araci, 10}
                   }
            },
            { "1687",new Dictionary<Enums.BasvuruFormuSutunTurleri, int> {
                        { Enums.BasvuruFormuSutunTurleri.TcKimlikNo, 0},
                        { Enums.BasvuruFormuSutunTurleri.Ad , 1} ,
                        { Enums.BasvuruFormuSutunTurleri.Soyad,2},
                        { Enums.BasvuruFormuSutunTurleri.TesvikBaslangic,3},
                        { Enums.BasvuruFormuSutunTurleri.TesvikBitis,4},
                        { Enums.BasvuruFormuSutunTurleri.Giris,5 },
                        { Enums.BasvuruFormuSutunTurleri.Baz,6},
                        { Enums.BasvuruFormuSutunTurleri.AktifMi,7},
                        { Enums.BasvuruFormuSutunTurleri.KanunNo,8},
                        { Enums.BasvuruFormuSutunTurleri.Durum , 9},
                        { Enums.BasvuruFormuSutunTurleri.Araci, 10}
                   }
            },
            { "14857",new Dictionary<Enums.BasvuruFormuSutunTurleri, int> {
                            { Enums.BasvuruFormuSutunTurleri.TcKimlikNo, 0},
                            { Enums.BasvuruFormuSutunTurleri.Sicil,1},
                            { Enums.BasvuruFormuSutunTurleri.Ad , 2} ,
                            { Enums.BasvuruFormuSutunTurleri.Soyad,3},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBaslangic,4},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBitis,5},
                            { Enums.BasvuruFormuSutunTurleri.RaporNo,6},
                            { Enums.BasvuruFormuSutunTurleri.OzurOrani,7 },
                            { Enums.BasvuruFormuSutunTurleri.Durum , 8},
                            { Enums.BasvuruFormuSutunTurleri.Araci, 9 }

                        }
            },
            { "7103",new Dictionary<Enums.BasvuruFormuSutunTurleri, int> {
                            { Enums.BasvuruFormuSutunTurleri.TcKimlikNo, 0},
                            { Enums.BasvuruFormuSutunTurleri.Sicil,1},
                            { Enums.BasvuruFormuSutunTurleri.Ad , 2} ,
                            { Enums.BasvuruFormuSutunTurleri.Soyad,3},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBaslangic,4},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBitis,5},
                            { Enums.BasvuruFormuSutunTurleri.Baz,6},
                            { Enums.BasvuruFormuSutunTurleri.UcretDestegiTercihi7103,7},
                            { Enums.BasvuruFormuSutunTurleri.PrimveUcretDestegiIcinBaslangicDonemi,8},
                            { Enums.BasvuruFormuSutunTurleri.PrimveUcretDestegiIcinBitisDonemi,9},
                            { Enums.BasvuruFormuSutunTurleri.PrimveUcretDestegiIcinIlaveOlunacakSayi,10},
                            { Enums.BasvuruFormuSutunTurleri.KanunNo,11 },
                            { Enums.BasvuruFormuSutunTurleri.Giris,12 },
                            { Enums.BasvuruFormuSutunTurleri.Cikis,13 },
                            { Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi,14},
                            { Enums.BasvuruFormuSutunTurleri.Araci , 15}

                      }
            },
            { "7166",new Dictionary<Enums.BasvuruFormuSutunTurleri, int> {
                { Enums.BasvuruFormuSutunTurleri.TcKimlikNo, 0},
                { Enums.BasvuruFormuSutunTurleri.Sicil,1},
                { Enums.BasvuruFormuSutunTurleri.Ad , 2} ,
                { Enums.BasvuruFormuSutunTurleri.Soyad,3},
                { Enums.BasvuruFormuSutunTurleri.TesvikBaslangic,8},
                { Enums.BasvuruFormuSutunTurleri.TesvikBitis,9},
                { Enums.BasvuruFormuSutunTurleri.Baz,10},
                { Enums.BasvuruFormuSutunTurleri.UcretDestegiTercihi7103,7},
                { Enums.BasvuruFormuSutunTurleri.PrimveUcretDestegiIcinBaslangicDonemi,8},
                { Enums.BasvuruFormuSutunTurleri.PrimveUcretDestegiIcinBitisDonemi,9},
                { Enums.BasvuruFormuSutunTurleri.PrimveUcretDestegiIcinIlaveOlunacakSayi,10},
                { Enums.BasvuruFormuSutunTurleri.KanunNo,11 },
                { Enums.BasvuruFormuSutunTurleri.Giris,12 },
                { Enums.BasvuruFormuSutunTurleri.Cikis,13 },
                { Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi,14},
                { Enums.BasvuruFormuSutunTurleri.Araci , 15}

                }
            },
            { "2828",new Dictionary<Enums.BasvuruFormuSutunTurleri, int> {
                            { Enums.BasvuruFormuSutunTurleri.TcKimlikNo, 0},
                            { Enums.BasvuruFormuSutunTurleri.Sicil,1},
                            { Enums.BasvuruFormuSutunTurleri.Ad , 2} ,
                            { Enums.BasvuruFormuSutunTurleri.Soyad,3},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBaslangic,4},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBitis,5},
                            { Enums.BasvuruFormuSutunTurleri.TesvikSuresi2828,6},
                            { Enums.BasvuruFormuSutunTurleri.Giris,7 },
                            { Enums.BasvuruFormuSutunTurleri.Cikis,8 },
                            { Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi,9},
                            { Enums.BasvuruFormuSutunTurleri.Araci , 10 }

                      }
            },
            { "7252",new Dictionary<Enums.BasvuruFormuSutunTurleri, int> {
                            { Enums.BasvuruFormuSutunTurleri.TcKimlikNo, 0},
                            { Enums.BasvuruFormuSutunTurleri.Sicil,1},
                            { Enums.BasvuruFormuSutunTurleri.Ad , 2} ,
                            { Enums.BasvuruFormuSutunTurleri.Soyad,3},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBaslangic,4},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBitis,5},
                            { Enums.BasvuruFormuSutunTurleri.KCUNUDSonlanmaTarihi,6},
                            { Enums.BasvuruFormuSutunTurleri.Baz,7},
                            { Enums.BasvuruFormuSutunTurleri.KanunNo,8},
                            { Enums.BasvuruFormuSutunTurleri.Giris,9 },
                            { Enums.BasvuruFormuSutunTurleri.Cikis,10 },
                            { Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi,11},
                            { Enums.BasvuruFormuSutunTurleri.Araci , 12 }

                      }
            },
            { "17256",new Dictionary<Enums.BasvuruFormuSutunTurleri, int> {
                            { Enums.BasvuruFormuSutunTurleri.TcKimlikNo, 0},
                            { Enums.BasvuruFormuSutunTurleri.Sicil,1},
                            { Enums.BasvuruFormuSutunTurleri.Ad , 2} ,
                            { Enums.BasvuruFormuSutunTurleri.Soyad,3},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBaslangic,4},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBitis,5},
                            { Enums.BasvuruFormuSutunTurleri.KanunNo,6},
                            { Enums.BasvuruFormuSutunTurleri.Giris,7 },
                            { Enums.BasvuruFormuSutunTurleri.Cikis,8 },
                            { Enums.BasvuruFormuSutunTurleri.SigortalininIsyerineBasvuruTarihi,9 },
                            { Enums.BasvuruFormuSutunTurleri.SigortaliIcinTercihDurumu,10 },
                            { Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi,11},
                            { Enums.BasvuruFormuSutunTurleri.VerilsinMi7256,12},
                            { Enums.BasvuruFormuSutunTurleri.Araci , 13 }

                      }
            },
            { "27256",new Dictionary<Enums.BasvuruFormuSutunTurleri, int> {
                            { Enums.BasvuruFormuSutunTurleri.TcKimlikNo, 0},
                            { Enums.BasvuruFormuSutunTurleri.Sicil, 1},
                            { Enums.BasvuruFormuSutunTurleri.Ad, 2} ,
                            { Enums.BasvuruFormuSutunTurleri.Soyad,3},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBaslangic,4},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBitis,5},
                            { Enums.BasvuruFormuSutunTurleri.Baz,6},
                            { Enums.BasvuruFormuSutunTurleri.KanunNo,7},
                            { Enums.BasvuruFormuSutunTurleri.Giris,8 },
                            { Enums.BasvuruFormuSutunTurleri.Cikis,9 },
                            { Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi,10},
                            { Enums.BasvuruFormuSutunTurleri.VerilsinMi7256 , 11 },
                            { Enums.BasvuruFormuSutunTurleri.Araci , 12 }
                      }
            },
            { "7316",new Dictionary<Enums.BasvuruFormuSutunTurleri, int> {
                            { Enums.BasvuruFormuSutunTurleri.TcKimlikNo, 0},
                            { Enums.BasvuruFormuSutunTurleri.Sicil, 1},
                            { Enums.BasvuruFormuSutunTurleri.Ad, 2} ,
                            { Enums.BasvuruFormuSutunTurleri.Soyad,3},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBaslangic,4},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBitis,5},
                            { Enums.BasvuruFormuSutunTurleri.KanunNo,6},
                            { Enums.BasvuruFormuSutunTurleri.Giris,7 },
                            { Enums.BasvuruFormuSutunTurleri.Cikis,8 },
                            { Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi,9},
                            { Enums.BasvuruFormuSutunTurleri.Araci , 10 }
                      }
            },
            { "3294",new Dictionary<Enums.BasvuruFormuSutunTurleri, int> {
                            { Enums.BasvuruFormuSutunTurleri.TcKimlikNo, 0},
                            { Enums.BasvuruFormuSutunTurleri.Sicil, 1},
                            { Enums.BasvuruFormuSutunTurleri.Ad, 2} ,
                            { Enums.BasvuruFormuSutunTurleri.Soyad,3},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBaslangic,4},
                            { Enums.BasvuruFormuSutunTurleri.TesvikBitis,5},
                            { Enums.BasvuruFormuSutunTurleri.Baz,6},
                            { Enums.BasvuruFormuSutunTurleri.Giris,7 },
                            { Enums.BasvuruFormuSutunTurleri.Cikis,8 },
                            { Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi,9},
                            { Enums.BasvuruFormuSutunTurleri.Araci , 10 }
                      }
            }

        };

        public static List<string> AyIsimleri = new List<string>() { "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };

        public const int SW_MAXIMISE = 3;

        public static DateTime UCGEklemeTarihi = new DateTime(2021, 6, 3);

        public static HashSet<string> tumKanunlar = new HashSet<string> {
            "00000",
            "05510",
            "25510",
            "16322",
            "26322",
            "17103",
            "27103",
            "06486",
            "46486",
            "56486",
            "66486",
            "05746",
            "15746",
            "14857",
            "03294",
            "02828",
            "06111",
            "15921",
            "25225",
            "55225",
            "06645",
            "07252",
            "17256",
            "27256",
            "07316"
        };

        

    }

}
