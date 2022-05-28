using System;
using System.Collections.Generic;

namespace TesvikProgrami
{
    public static class TesvikHesaplamaSabitleri
    {

        public static List<string> DestekKapsaminaGirmeyenBelgeTurleri = new List<string> { "2", "7", "12", "14", "19", "20", "21", "22", "23", "25", "28", "39", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "90", "91", "92" };

        public static List<string> AylikCalisanSayisiveBazdaYasakliBelgeTurleri = new List<string> { "7", "07", "19", "22", "23", "25", "42", "43", "44", "46", "47", "49", "50" };

        public static DateTime AylikCalisanKisitlamaKalkmaTarihi6486 = new DateTime(2016, 3, 1);

        public static List<string> AyIcindeOnaylanmamisBildirgelerdeVarsaXmlCikartilmayacakKanunNolar = new List<string> { "16322", "26322", "25510" };

        public const decimal AsgariUcretCarpimOrani = 0.06m;

        public static List<string> Iller46486 = new List<string> { "003", "005", "008", "074", "019", "081", "023", "024", "031", "070", "037", "071", "040", "043", "044", "050", "053", "058", "061", "064" };

        public static List<string> Iller56486 = new List<string> { "002", "068", "069", "018", "025", "028", "029", "046", "079", "051", "052", "080", "057", "060", "062", "066" };

        public static List<string> Iller66486 = new List<string> { "004", "075", "072", "012", "013", "021", "030", "076", "036", "047", "049", "056", "063", "073", "065" };

        public const decimal ToplamUcretSiniri6111ve687 = 3775;

        public const decimal CarpimOrani687 = 22.22m;

        public const decimal CarpimMiktari7166 = 67.36m;

        public static List<string> TesvikVerilecekKanunlar = new List<string> { "6111", "6645", "687", "6486", "7103", "2828", "14857", "7166", "6322/25510" , "7252", "5510","17256","27256","7316","3294"};

        public static Dictionary<string, List<string>> BagliKanunlarIcmalHesaplama = new Dictionary<string, List<string>> {
                                                                                                                                { "05084", new List<string> { "00000"} },
                                                                                                                                { "85084", new List<string> { "00000"} },
                                                                                                                                { "85615", new List<string> { "00000"} },
                                                                                                                                { "05615", new List<string> { "00000"} },
                                                                                                                                { "07166", new List<string> { "00000"} },
                                                                                                                                { "17103", new List<string> { "00000"} },
                                                                                                                                { "27103", new List<string> { "00000"} },
                                                                                                                                { "00687", new List<string> { "00000"} },
                                                                                                                                { "01687", new List<string> { "00000"} },
                                                                                                                                { "07252", new List<string> { "00000"} },
                                                                                                                                { "17256", new List<string> { "00000"} },
                                                                                                                                { "27256", new List<string> { "00000"} },
                                                                                                                                { "07316", new List<string> { "00000"} },
                                                                                                                                { "06111", new List<string> { "05510"} },
                                                                                                                                { "06645", new List<string> { "05510"} },
                                                                                                                                { "02828", new List<string> { "05510"} },
                                                                                                                                { "14857", new List<string> { "05510"} },
                                                                                                                                { "46486", new List<string> { "05510"} },
                                                                                                                                { "56486", new List<string> { "05510"} },
                                                                                                                                { "66486", new List<string> { "05510"} },
                                                                                                                                { "16322", new List<string> { "05510"} },
                                                                                                                                { "26322", new List<string> { "05510"} },
                                                                                                                                { "25510", new List<string> { "05510"} },
                                                                                                                                { "03294", new List<string> { "05510"} },
                                                                                                                                { "05510", new List<string> { "00000"} },
                                                                                                                                { "00000", new List<string> { } },
                                                                                                                           };

        public static HashSet<string> AsgariUcretDestegiKapsamiDisindakiKanunlar = new HashSet<string> { "17103", "27103", "00687", "01687", "07252", "17256", "27256", "07316" , "07166" };

        public static Dictionary<int, decimal> AsgariUcretDestegiKatsayilari = new Dictionary<int, decimal> { { 2017, 3.33m }, { 2018, 3.33m }, { 2019, 5m }, { 2020, 2.5m }, { 2021, 2.5m } };

        public static decimal CarpimSabiti7256_2021OcakVeSonrasi = 53.67m;

        public static decimal CarpimSabiti7256_2021OcakOncesi = 44.15m;

        public static HashSet<int> MuhtasardaBuBelgelerinHaricindeUyariVerilecek = new HashSet<int> { 1, 4, 5, 6, 13, 14, 20, 24, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 52, 53, 54, 55, 2 , 48};

    }

}
