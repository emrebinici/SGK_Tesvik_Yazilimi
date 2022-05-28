using System;
using System.Collections.Generic;
using System.Linq;

namespace TesvikProgrami.Classes
{
    public class Tesvik
    {
        public string Kanun = null;

        List<string> BelgeTuruOraniHesaplamadaEklenecekAlanlar = new List<string>();
        public Dictionary<string, DonusturulecekKanun> DonusturulecekKanunlar = new Dictionary<string, DonusturulecekKanun>();
        public List<string> DestekKapsaminaGirmeyenBelgeTurleri = new List<string>();
        public List<string> DestekKapsaminaGirenBelgeTurleri = new List<string>();
        public bool AylikCalisanaTaseronDahilEdilsin = true;
        public DateTime dtKurulusTarihi;
        public List<int> BildirgeOlanYillar;
        public bool BazYil;
        public bool BazAy;
        public DateTime TesvikBaslamaZamani;
        public DateTime TesvikBitisZamani;
        public bool TaseronunAldigiTesvikKotadanDusulsun = false;
        public List<string> AyIcindeVarsaHicKimseyeTesvikVerilmeyecekKanunlar = new List<string>();
        public bool AyIcinde_Varsa_Hic_Kimseye_Tesvik_Verilmeyecek_Kanunlar_Icin_Cari_Ayda_Orijinal_Kanuna_Da_Bakilsin;
        public bool ArdArda2AyYoksaKisiCikmisKabulEdilsin = false;
        public int KurulustanBuKadarAySonraTesvikVermeyeBaslasin = -1;
        public bool CikistanSonraGiriseTesvikVerilsin = false;
        public bool AyIcindeGiristenOnceCikisVarsaVeyaAyIcindeBirdenFazlaGirisVarsaTesvikVerilmesin = false;
        public bool BasvuruFormuVar = true;
        public bool ToplamUcretiSifirOlanlaraTesvikVerilsin = false;
        public bool GunuSifirOlanlaraTesvikVerilsin = false;
        public List<string> AltKanunlar = new List<string>();

        /// <summary>
        /// Kişinin ay içinde o belge türünde bu yasaklı kanunlardan bir bildirimi varsa o kişiye bu teşvik verilmeyecek
        /// </summary>
        public List<YasakliKanun> YasakliKanunlar = new List<YasakliKanun>();
        public bool TaseronaTesvikVerilsin = false;
        public bool AsgariUcretDestekTutarlariDikkateAlinsin = false;
        public bool TekTesvikHakedipGunSayisiAyarlardakiMinimumGunSayisindanBuyukOlanlaraOncelikVerilsin = false;
        public bool TesvikMiktariAyniIseUcretiDusukOlanaVerilsin = false;
        public KeyValuePair<DateTime, int> AylikCalisanKisitlamaKalkmaTarihiveKisitlamaSayisi = new KeyValuePair<DateTime, int>(DateTime.MinValue, 0);
        public bool VerilipVerilmeyecegiKullaniciyaSorulsun = false;
        public bool VerilipVerilmeyecegiKullaniciyaSoruldu = false;
        public bool VerilipVerilmeyecegiKullaniciCevabi = false;
        public DateTime BazHesaplamaBaslangicTarihi = DateTime.MinValue;
        public int BazHesaplamaGeriyeGidilecekAySayisi = 0;
        public bool AltKanunIlkTanimlamaTarihiEnYakinOlaninKanunNosuOlsun = false;
        public bool AltKanunPlakaKodunaGoreBelirlenecek = false;
        public Dictionary<string, List<string>> AltKanunPlakaKodlari = new Dictionary<string, List<string>>();
        public bool TesvikAlipBasvuruFormundaOlmayanKisilerKontrolEdilecek = true;
        public bool BasvuruFormunaBakildiktanSonraAltKanunBosIseTesvikVerilmesin = false;
        public string IcmalBaslik;
        public string UstYaziMetni;
        public bool TesvikAlabilir = true;
        //public bool AyIcindeDahaOncedenAlinanBaskaTesvikVarsaTesvikVerilmesin = true;
        public bool KanunIcmalindeGunGosterilsin = true; // false olursa kanun icmalinde ikinci sutunda gün yerine matrah yazılacak
        public int GirisTarihindenItibarenSuKadarAyIcindeIstenCikildiysaTesvikVerilmesin = -1;
        public List<string> IstenCikisYasakliKodlar = new List<string>();
        public List<string> altTesvikler = new List<string>();
        public DateTime KurulusTarihiBuTarihtenBuyukveyeEsitseTesvikVerilmesin;
        public List<int> BuYillardaHicBildirgeYoksaTesvikVerilmesin = new List<int>();
        public List<string> BazHesaplamadaDikkateAlinacakBelgeTurleri = new List<string>();
        public bool BazHesaplamadaOrtalamaAlinsin = true;
        public bool BorcluAydaTesvikVerilsin = false;
        public bool DonusenlerIcmaldenDusulsun = true;
        public bool TesvikVerilirseDigerTesviklerIptalEdilecek = false;
        public bool Basvuru_Formundaki_Baz_Sayisi_Kadar_Gun_Ay_Icinde_TesvikAlabilir = false;
        public List<string> TesvikVerilmeyecekIsKoluKodlari = new List<string>();
        public List<string> TesvikVerilmeyecekEksikGunNedenleri = new List<string>();
        public List<string> UyariVerilecekEksikGunNedenleri = new List<string>();
        public Dictionary<string, string> SatirBolundugundeDegistirilecekEksikGunKodlari = new Dictionary<string, string>();
        public string SatirBolundugundeEksikGunYoksaYazacakEksikGunNedeni;
        public bool CarideDonusenlerIcmaldenDusulsun = true;
        public DateTime BasvuruFormuGirisTarihiBuTarihtenBuyukveyaEsitseTesvikVerilmeyecek;
        public List<DateTime> GeriyeDonukBelirtilenAylardakiHizmetBildirimlerininBiriYasakliIseTesvikVerilmeyecek = new List<DateTime>();
        public List<DateTime> GeriyeDonukBelirtilenAylardakiHizmetBildirimlerininBiriBelirtilenEksikGunKodlarindanOlmali = new List<DateTime>();
        public List<string> GeriyeDonukBelirtilenAylardakiHizmetBildirimlerininBiriBuEksikGunKodlarindanOlmali = new List<string>();
        public List<string> CariAydaEksikGunKoduBunlardanBiriIseKisiyeDigerBelgeTurlerideDahilTesvikVerilmeyecek = new List<string>();
        public bool CarideKiyaslamaYaparkenDigerTesviklerMahsupsuzTutarUzerindenKiyaslanacak = false;
        public List<string> AyIcindeKisideBuBelgeTurlerindenVarsaTesvikVerilmeyecek = new List<string>();
        public List<string> GecmisAydaEksikGunKoduBunlardanBiriIseKisiyeDigerBelgeTurlerideDahilTesvikVerilmeyecek = new List<string>();
        public List<string> BuKanunlardanBiriDonusturulurkenAsgariUcretDestegiBozulupBozulmadiginaBakilmayacak = new List<string>();
        public bool AsgariUcretDestegiKapsaminda=true;


        private string _AltKanun;
        public string AltKanun
        {
            get { return _AltKanun; }
            set
            {
                _AltKanun = value;

                if (_AltKanun != null)
                {

                    if (_AltKanun.Equals("17103"))
                    {
                        TesvikMiktariAyniIseUcretiDusukOlanaVerilsin = false;
                    }
                    else if (_AltKanun.Equals("27103"))
                    {
                        TesvikMiktariAyniIseUcretiDusukOlanaVerilsin = true;
                    }
                }

            }
        }

        public List<string> IcmalCikartilacakAltKanunlar = new List<string>();

        public Tesvik(string pKanun, bool DonusturulecekKanunlaraBakilsin = true)
        {
            Kanun = pKanun;

            if (Kanun.Equals("6111"))
            {
                BelgeTuruOraniHesaplamadaEklenecekAlanlar = new List<string> { "MalulYaslilikOraniIsveren", "GenelSaglikIsveren" };
                DestekKapsaminaGirmeyenBelgeTurleri = new List<string> { "2", "7", "12", "14", "19", "20", "21", "22", "23", "25", "28", "39", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "90", "91", "92" };
                DonusturulecekKanunlar = new Dictionary<string, DonusturulecekKanun> { { "05510", new DonusturulecekKanun { DonusturulecekKanunNo = "05510" } },
                                                                                       { "46486", new DonusturulecekKanun { DonusturulecekKanunNo = "46486"} },
                                                                                       { "56486", new DonusturulecekKanun { DonusturulecekKanunNo = "56486" } },
                                                                                       { "66486", new DonusturulecekKanun { DonusturulecekKanunNo = "66486"  } },
                                                                                     };
                BazYil = true;
                BazAy = true;
                TesvikBaslamaZamani = new DateTime(2011, 3, 1);
                TaseronunAldigiTesvikKotadanDusulsun = true;
                ArdArda2AyYoksaKisiCikmisKabulEdilsin = true;
                //AyIcindeGiristenOnceCikisVarsaVeyaAyIcindeBirdenFazlaGirisVarsaTesvikVerilmesin = true;
                GunuSifirOlanlaraTesvikVerilsin = true;
                ToplamUcretiSifirOlanlaraTesvikVerilsin = false;
                YasakliKanunlar = new List<YasakliKanun> { new YasakliKanun { Kanun = "05746", GunveyaKazancSifirdanBuyukOlmali = true, KullaniciyaSorulsun = true, TaseronGunveKazancinaBakilsin = false },
                                                           new YasakliKanun { Kanun = "15746", GunveyaKazancSifirdanBuyukOlmali = true, KullaniciyaSorulsun = true, TaseronGunveKazancinaBakilsin = false }};
                BazHesaplamaGeriyeGidilecekAySayisi = 6;
                IcmalBaslik = "6111 SAYILI KANUN KAPSAMINDAKİ TEŞVİKTEN";
                UstYaziMetni = "6111 sayılı kanun ile 4447 sayılı kanunun geçici 10. maddesine eklenen";
                KanunIcmalindeGunGosterilsin = false;
            }
            else if (Kanun.Equals("7103"))
            {
                BelgeTuruOraniHesaplamadaEklenecekAlanlar = new List<string> { "MalulYaslilikOraniSigortali",
                                                                               "MalulYaslilikOraniIsveren",
                                                                               "GenelSaglikSigortali",
                                                                               "GenelSaglikIsveren",
                                                                               "SosyalDestekSigortali",
                                                                               "SosyalDestekIsveren",
                                                                               "IssizlikSigortali",
                                                                               "IssizlikIsveren"
                };
                DestekKapsaminaGirmeyenBelgeTurleri = new List<string> { "2", "7", "12", "14", "19", "20", "21", "22", "23", "25", "28", "39", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "90", "91", "92" };
                DonusturulecekKanunlar = new Dictionary<string, DonusturulecekKanun> { { "05510", new DonusturulecekKanun { DonusturulecekKanunNo = "05510" } },
                                                                                       { "46486", new DonusturulecekKanun { DonusturulecekKanunNo = "46486" } },
                                                                                       { "56486", new DonusturulecekKanun { DonusturulecekKanunNo = "56486" } },
                                                                                       { "66486", new DonusturulecekKanun { DonusturulecekKanunNo = "66486" } },
                                                                                     };
                AylikCalisanaTaseronDahilEdilsin = false;
                BazYil = true;
                BazAy = false;
                GunuSifirOlanlaraTesvikVerilsin = false;
                ToplamUcretiSifirOlanlaraTesvikVerilsin = false;
                KurulustanBuKadarAySonraTesvikVermeyeBaslasin = 3;
                AltKanunlar = new List<string> { "17103", "27103" };
                YasakliKanunlar = new List<YasakliKanun> { new YasakliKanun { Kanun = "05746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = true, TaseronGunveKazancinaBakilsin = false },
                                                           new YasakliKanun { Kanun = "15746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = true, TaseronGunveKazancinaBakilsin = false }};
                AsgariUcretDestekTutarlariDikkateAlinsin = true;
                TekTesvikHakedipGunSayisiAyarlardakiMinimumGunSayisindanBuyukOlanlaraOncelikVerilsin = true;
                BazHesaplamaGeriyeGidilecekAySayisi = 12;
                AltKanunIlkTanimlamaTarihiEnYakinOlaninKanunNosuOlsun = true;
                BasvuruFormunaBakildiktanSonraAltKanunBosIseTesvikVerilmesin = true;
                IcmalBaslik = "7103 SAYILI KANUN KAPSAMINDAKİ TEŞVİKTEN";
                UstYaziMetni = "7103 sayılı kanun ile 4447 sayılı kanunun geçici 19. maddesine eklenen";
                BuKanunlardanBiriDonusturulurkenAsgariUcretDestegiBozulupBozulmadiginaBakilmayacak = new List<string> { "17256", "27256" };
                AsgariUcretDestegiKapsaminda = false;

            }
            else if (Kanun.Equals("6645"))
            {
                BelgeTuruOraniHesaplamadaEklenecekAlanlar = new List<string> { "MalulYaslilikOraniIsveren", "GenelSaglikIsveren" };
                DestekKapsaminaGirmeyenBelgeTurleri = new List<string> { "2", "7", "12", "14", "19", "20", "21", "22", "23", "25", "28", "39", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "90", "91", "92" };
                DonusturulecekKanunlar = new Dictionary<string, DonusturulecekKanun> { { "05510", new DonusturulecekKanun { DonusturulecekKanunNo = "05510"  } },
                                                                                       { "46486", new DonusturulecekKanun { DonusturulecekKanunNo = "46486"  } },
                                                                                       { "56486", new DonusturulecekKanun { DonusturulecekKanunNo = "56486"  } },
                                                                                       { "66486", new DonusturulecekKanun { DonusturulecekKanunNo = "66486"  } },
                                                                                     };
                BazYil = true;
                BazAy = false;
                TaseronunAldigiTesvikKotadanDusulsun = true;
                ArdArda2AyYoksaKisiCikmisKabulEdilsin = true;
                //AyIcindeGiristenOnceCikisVarsaVeyaAyIcindeBirdenFazlaGirisVarsaTesvikVerilmesin = true;
                GunuSifirOlanlaraTesvikVerilsin = false;
                ToplamUcretiSifirOlanlaraTesvikVerilsin = true;
                YasakliKanunlar = new List<YasakliKanun> { new YasakliKanun { Kanun = "05746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = true, TaseronGunveKazancinaBakilsin = false },
                                                           new YasakliKanun { Kanun = "15746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = true, TaseronGunveKazancinaBakilsin = false }};
                BazHesaplamaGeriyeGidilecekAySayisi = 12;
                TesvikMiktariAyniIseUcretiDusukOlanaVerilsin = true;
                IcmalBaslik = "6645 SAYILI KANUN KAPSAMINDAKİ TEŞVİKTEN";
                UstYaziMetni = "6645 sayılı kanun ile 4447 sayılı kanunun geçici 15. maddesine eklenen";

            }
            else if (Kanun.Equals("687"))
            {
                BelgeTuruOraniHesaplamadaEklenecekAlanlar = new List<string> { "MalulYaslilikOraniIsveren", "GenelSaglikIsveren" };
                DestekKapsaminaGirmeyenBelgeTurleri = new List<string> { "2", "7", "12", "14", "19", "20", "21", "22", "23", "25", "28", "39", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "90", "91", "92" };
                DonusturulecekKanunlar = new Dictionary<string, DonusturulecekKanun> { { "05510", new DonusturulecekKanun { DonusturulecekKanunNo = "05510" } },
                                                                                       { "46486", new DonusturulecekKanun { DonusturulecekKanunNo = "46486" } },
                                                                                       { "56486", new DonusturulecekKanun { DonusturulecekKanunNo = "56486" } },
                                                                                       { "66486", new DonusturulecekKanun { DonusturulecekKanunNo = "66486" } },
                                                                                     };
                BazAy = false;
                BazYil = false;
                TaseronunAldigiTesvikKotadanDusulsun = true;
                KurulustanBuKadarAySonraTesvikVermeyeBaslasin = 3;
                //AyIcindeGiristenOnceCikisVarsaVeyaAyIcindeBirdenFazlaGirisVarsaTesvikVerilmesin = true;
                AltKanunlar = new List<string> { "00687", "01687" };
                GunuSifirOlanlaraTesvikVerilsin = false;
                ToplamUcretiSifirOlanlaraTesvikVerilsin = true;
                YasakliKanunlar = new List<YasakliKanun> { new YasakliKanun { Kanun = "05746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = true, TaseronGunveKazancinaBakilsin = false },
                                                           new YasakliKanun { Kanun = "15746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = true, TaseronGunveKazancinaBakilsin = false }};
                AsgariUcretDestekTutarlariDikkateAlinsin = true;
                TekTesvikHakedipGunSayisiAyarlardakiMinimumGunSayisindanBuyukOlanlaraOncelikVerilsin = true;
                TesvikMiktariAyniIseUcretiDusukOlanaVerilsin = true;
                BazHesaplamaBaslangicTarihi = new DateTime(2017, 1, 1);
                BazHesaplamaGeriyeGidilecekAySayisi = 1;
                TesvikAlipBasvuruFormundaOlmayanKisilerKontrolEdilecek = false;
                IcmalBaslik = "687 SAYILI KHK KAPSAMINDAKİ TEŞVİKTEN";
                UstYaziMetni = "6111 sayılı kanun ile 4447 sayılı kanunun geçici 17. maddesine eklenen";
                AsgariUcretDestegiKapsaminda = false;

            }
            else if (Kanun.Equals("2828"))
            {
                BelgeTuruOraniHesaplamadaEklenecekAlanlar = new List<string> { "MalulYaslilikOraniSigortali",
                                                                               "MalulYaslilikOraniIsveren",
                                                                               "GenelSaglikSigortali",
                                                                               "GenelSaglikIsveren",
                                                                               "SosyalDestekSigortali",
                                                                               "SosyalDestekIsveren",
                                                                               "IssizlikSigortali",
                                                                               "IssizlikIsveren"
                };
                DestekKapsaminaGirmeyenBelgeTurleri = new List<string> { "2", "7", "12", "14", "19", "20", "21", "22", "23", "25", "28", "39", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "90", "91", "92" };
                DonusturulecekKanunlar = new Dictionary<string, DonusturulecekKanun> { { "05510", new DonusturulecekKanun { DonusturulecekKanunNo = "05510" } },
                                                                                       { "46486", new DonusturulecekKanun { DonusturulecekKanunNo = "46486" } },
                                                                                       { "56486", new DonusturulecekKanun { DonusturulecekKanunNo = "56486" } },
                                                                                       { "66486", new DonusturulecekKanun { DonusturulecekKanunNo = "66486" } }
                                                                                     };
                BazAy = false;
                BazYil = false;
                //AyIcindeGiristenOnceCikisVarsaVeyaAyIcindeBirdenFazlaGirisVarsaTesvikVerilmesin = true;
                GunuSifirOlanlaraTesvikVerilsin = false;
                ToplamUcretiSifirOlanlaraTesvikVerilsin = true;
                YasakliKanunlar = new List<YasakliKanun> { new YasakliKanun { Kanun = "05746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = true, TaseronGunveKazancinaBakilsin = false },
                                                           new YasakliKanun { Kanun = "15746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = true, TaseronGunveKazancinaBakilsin = false }};
                IcmalBaslik = "2828 SAYILI KANUN KAPSAMINDAKİ TEŞVİKTEN";
                UstYaziMetni = "2828 sayılı kanunun";
                BorcluAydaTesvikVerilsin = true;
            }
            else if (Kanun.Equals("14857"))
            {
                BelgeTuruOraniHesaplamadaEklenecekAlanlar = new List<string> { "MalulYaslilikOraniIsveren", "GenelSaglikIsveren" };
                DestekKapsaminaGirmeyenBelgeTurleri = new List<string> { "2", "4", "7", "12", "14", "19", "20", "21", "22", "23", "25", "28", "35", "39", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "90", "91", "92" };
                DonusturulecekKanunlar = new Dictionary<string, DonusturulecekKanun> { { "05510", new DonusturulecekKanun { DonusturulecekKanunNo = "05510"  } },
                                                                                       { "46486", new DonusturulecekKanun { DonusturulecekKanunNo = "46486"  } },
                                                                                       { "56486", new DonusturulecekKanun { DonusturulecekKanunNo = "56486"  } },
                                                                                       { "66486", new DonusturulecekKanun { DonusturulecekKanunNo = "66486"  } },
                                                                                     };
                BazAy = false;
                BazYil = false;
                TesvikBaslamaZamani = new DateTime(2013, 9, 1);
                TaseronunAldigiTesvikKotadanDusulsun = true;
                CikistanSonraGiriseTesvikVerilsin = true;
                GunuSifirOlanlaraTesvikVerilsin = false;
                ToplamUcretiSifirOlanlaraTesvikVerilsin = true;
                YasakliKanunlar = new List<YasakliKanun> { new YasakliKanun { Kanun = "05746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = true, TaseronGunveKazancinaBakilsin = false },
                                                           new YasakliKanun { Kanun = "15746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = true, TaseronGunveKazancinaBakilsin = false }};
                IcmalBaslik = "14857 SAYILI KANUN KAPSAMINDAKİ TEŞVİKTEN";
                UstYaziMetni = "14857 sayılı kanunun";
                //AyIcindeDahaOncedenAlinanBaskaTesvikVarsaTesvikVerilmesin = false;
                BorcluAydaTesvikVerilsin = true;
            }
            else if (Kanun.Equals("6486"))
            {
                DestekKapsaminaGirenBelgeTurleri = new List<string> { "1", "4", "5", "6", "13", "20", "21", "29", "30", "32", "33", "35", "36" };
                DonusturulecekKanunlar = new Dictionary<string, DonusturulecekKanun> { { "05510", new DonusturulecekKanun { DonusturulecekKanunNo = "05510" } } };
                AyIcindeVarsaHicKimseyeTesvikVerilmeyecekKanunlar = new List<string> { "25510", "16322", "26322" };
                AyIcinde_Varsa_Hic_Kimseye_Tesvik_Verilmeyecek_Kanunlar_Icin_Cari_Ayda_Orijinal_Kanuna_Da_Bakilsin = true;
                CikistanSonraGiriseTesvikVerilsin = true;
                BasvuruFormuVar = false;
                AltKanunlar = new List<string> { "46486", "56486", "66486" };
                GunuSifirOlanlaraTesvikVerilsin = true;
                ToplamUcretiSifirOlanlaraTesvikVerilsin = true;
                TesvikBaslamaZamani = new DateTime(2013, 1, 1);
                TesvikBitisZamani = new DateTime(2022, 1, 1);
                AylikCalisanKisitlamaKalkmaTarihiveKisitlamaSayisi = new KeyValuePair<DateTime, int>(new DateTime(2016, 3, 1), 10);
                VerilipVerilmeyecegiKullaniciyaSorulsun = true;
                AltKanunPlakaKodunaGoreBelirlenecek = true;
                AltKanunPlakaKodlari = new Dictionary<string, List<string>> {
                                                                                { "46486" , new List<string> { "003", "005", "008", "074", "019", "081", "023", "024", "031", "070", "037", "071", "040", "043", "044", "050", "053", "058", "061", "064" } },
                                                                                { "56486" , new List<string> { "002", "068", "069", "018", "025", "028", "029", "046", "079", "051", "052", "080", "057", "060", "062", "066" } },
                                                                                { "66486" , new List<string> { "004", "075", "072", "012", "013", "021", "030", "076", "036", "047", "049", "056", "063", "073", "065" } }
                                                                            };
                BasvuruFormunaBakildiktanSonraAltKanunBosIseTesvikVerilmesin = true;
                IcmalBaslik = "6486 SAYILI KANUN KAPSAMINDAKİ TEŞVİKTEN";
                UstYaziMetni = "6486 sayılı kanun ile 5510 sayılı kanunun 81. maddesinin ikinci fıkrasına eklenen";
                //AyIcindeDahaOncedenAlinanBaskaTesvikVarsaTesvikVerilmesin = false;

            }
            else if (Kanun.Equals("7166"))
            {
                BelgeTuruOraniHesaplamadaEklenecekAlanlar = new List<string> { "MalulYaslilikOraniSigortali",
                                                                               "MalulYaslilikOraniIsveren",
                                                                               "GenelSaglikSigortali",
                                                                               "GenelSaglikIsveren",
                                                                               "SosyalDestekSigortali",
                                                                               "SosyalDestekIsveren",
                                                                               "IssizlikSigortali",
                                                                               "IssizlikIsveren"
                };
                DestekKapsaminaGirmeyenBelgeTurleri = new List<string> { "2", "7", "12", "14", "19", "20", "21", "22", "23", "25", "28", "39", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "90", "91", "92" };
                DonusturulecekKanunlar = new Dictionary<string, DonusturulecekKanun> { { "05510", new DonusturulecekKanun { DonusturulecekKanunNo = "05510" } },
                                                                                       { "46486", new DonusturulecekKanun { DonusturulecekKanunNo = "46486" } },
                                                                                       { "56486", new DonusturulecekKanun { DonusturulecekKanunNo = "56486" } },
                                                                                       { "66486", new DonusturulecekKanun { DonusturulecekKanunNo = "66486" } },
                                                                                     };
                AylikCalisanaTaseronDahilEdilsin = false;
                BazYil = true;
                BazAy = false;
                TesvikBaslamaZamani = new DateTime(2019, 2, 1);
                GunuSifirOlanlaraTesvikVerilsin = false;
                ToplamUcretiSifirOlanlaraTesvikVerilsin = false;
                AltKanunlar = new List<string> { "17103", "27103" };
                YasakliKanunlar = new List<YasakliKanun> { new YasakliKanun { Kanun = "05746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = true, TaseronGunveKazancinaBakilsin = false },
                                                           new YasakliKanun { Kanun = "15746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = true, TaseronGunveKazancinaBakilsin = false }};
                AsgariUcretDestekTutarlariDikkateAlinsin = true;
                BazHesaplamaGeriyeGidilecekAySayisi = 12;
                AltKanunIlkTanimlamaTarihiEnYakinOlaninKanunNosuOlsun = true;
                BasvuruFormunaBakildiktanSonraAltKanunBosIseTesvikVerilmesin = true;
                IcmalBaslik = "7166 SAYILI KANUN KAPSAMINDAKİ TEŞVİKTEN";
                UstYaziMetni = "7166 sayılı kanun ile 4447 sayılı kanunun geçici 19. maddesine eklenen";
                GirisTarihindenItibarenSuKadarAyIcindeIstenCikildiysaTesvikVerilmesin = 9;
                IstenCikisYasakliKodlar = new List<string> { "1", "4", "5", "15", "16", "17", "19", "20", "22", "25", "34" };
                altTesvikler = new List<string> { "7103" };
                KurulusTarihiBuTarihtenBuyukveyeEsitseTesvikVerilmesin = new DateTime(2019, 1, 1);
                BuYillardaHicBildirgeYoksaTesvikVerilmesin = new List<int> { 2018 };
                BazHesaplamadaDikkateAlinacakBelgeTurleri = new List<string> { "1", "4", "5", "6", "13", "14", "20", "24", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "52", "53", "54", "55" };
                BazHesaplamadaOrtalamaAlinsin = false;
                DonusenlerIcmaldenDusulsun = false;
                BuKanunlardanBiriDonusturulurkenAsgariUcretDestegiBozulupBozulmadiginaBakilmayacak = new List<string> { "17256", "27256" };
                AsgariUcretDestegiKapsaminda = false;

            }
            else if (Kanun.Equals("6322/25510"))
            {
                BelgeTuruOraniHesaplamadaEklenecekAlanlar = new List<string> { "MalulYaslilikOraniIsveren", "GenelSaglikIsveren" };
                DestekKapsaminaGirmeyenBelgeTurleri = new List<string> { "2", "7", "12", "14", "19", "20", "21", "22", "23", "25", "28", "39", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "90", "91", "92" };
                DonusturulecekKanunlar = new Dictionary<string, DonusturulecekKanun> { { "05510", new DonusturulecekKanun { DonusturulecekKanunNo = "05510" } },
                                                                                       { "46486", new DonusturulecekKanun { DonusturulecekKanunNo = "46486" } },
                                                                                       { "56486", new DonusturulecekKanun { DonusturulecekKanunNo = "56486" } },
                                                                                       { "66486", new DonusturulecekKanun { DonusturulecekKanunNo = "66486" } },
                                                                                     };
                GunuSifirOlanlaraTesvikVerilsin = false;
                ToplamUcretiSifirOlanlaraTesvikVerilsin = false;
                AltKanunlar = new List<string> { "16322", "26322", "25510" };
                BasvuruFormuVar = false;
                TesvikAlipBasvuruFormundaOlmayanKisilerKontrolEdilecek = false;
                TesvikAlabilir = false;
                //AyIcindeDahaOncedenAlinanBaskaTesvikVarsaTesvikVerilmesin = false;

            }
            else if (Kanun.Equals("7252"))
            {
                BelgeTuruOraniHesaplamadaEklenecekAlanlar = new List<string> { "MalulYaslilikOraniSigortali",
                                                                               "MalulYaslilikOraniIsveren",
                                                                               "GenelSaglikSigortali",
                                                                               "GenelSaglikIsveren",
                                                                               "SosyalDestekSigortali",
                                                                               "SosyalDestekIsveren",
                                                                               "IssizlikSigortali",
                                                                               "IssizlikIsveren"
                };
                DestekKapsaminaGirmeyenBelgeTurleri = new List<string> { "2", "7", "12", "14", "19", "20", "21", "22", "23", "25", "28", "39", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "90", "91", "92" };
                DonusturulecekKanunlar = new Dictionary<string, DonusturulecekKanun> { { "00000", new DonusturulecekKanun { DonusturulecekKanunNo = "00000"  } },
                                                                                       { "05510", new DonusturulecekKanun { DonusturulecekKanunNo = "05510"  } },
                                                                                       { "46486", new DonusturulecekKanun { DonusturulecekKanunNo = "46486"  } },
                                                                                       { "56486", new DonusturulecekKanun { DonusturulecekKanunNo = "56486"  } },
                                                                                       { "66486", new DonusturulecekKanun { DonusturulecekKanunNo = "66486"  } },
                };
                IcmalBaslik = "7252 SAYILI KANUN KAPSAMINDAKİ TEŞVİKTEN";
                UstYaziMetni = "7252 sayılı kanun ile 4447 sayılı kanunun geçici 26. maddesine eklenen";
                BorcluAydaTesvikVerilsin = true;
                TesvikVerilirseDigerTesviklerIptalEdilecek = true;
                Basvuru_Formundaki_Baz_Sayisi_Kadar_Gun_Ay_Icinde_TesvikAlabilir = true;
                TesvikVerilmeyecekIsKoluKodlari = new List<string> { "9700" };
                TesvikVerilmeyecekEksikGunNedenleri = new List<string> { "17" };
                //UyariVerilecekEksikGunNedenleri = new List<string> { "18", "27", "28", "29" };
                SatirBolundugundeDegistirilecekEksikGunKodlari = new Dictionary<string, string> {
                   { "01", "12" } ,
                   { "02", "12" } ,
                   { "03", "12" } ,
                   { "04", "12" } ,
                   { "05", "12" } ,
                   { "06", "12" } ,
                   { "07", "12" } ,
                   { "08", "12" } ,
                   { "09", "12" } ,
                   { "10", "12" } ,
                   { "11", "12" } ,
                   { "12", "12" } ,
                   { "13", "12" } ,
                   { "15", "12" } ,
                   { "16", "12" } ,
                   { "19", "12" } ,
                   { "20", "12" } ,
                   { "21", "12" } ,
                   { "22", "12" } ,
                   { "23", "24" } ,
                   { "24", "24" } ,
                   { "25", "25" } ,
                   { "26", "26" } ,
                };
                SatirBolundugundeEksikGunYoksaYazacakEksikGunNedeni = "25";
                CarideDonusenlerIcmaldenDusulsun = false;
                GeriyeDonukBelirtilenAylardakiHizmetBildirimlerininBiriYasakliIseTesvikVerilmeyecek = new List<DateTime> { new DateTime(2020, 3, 1), new DateTime(2020, 4, 1), new DateTime(2020, 5, 1), new DateTime(2020, 6, 1) };
                //BasvuruFormuGirisTarihiBuTarihtenBuyukveyaEsitseTesvikVerilmeyecek = new DateTime(2020, 7, 1);
                CikistanSonraGiriseTesvikVerilsin = true;
                //GeriyeDonukBelirtilenAylardakiHizmetBildirimlerininBiriBelirtilenEksikGunKodlarindanOlmali = new List<DateTime> { new DateTime(2020, 3, 1), new DateTime(2020, 4, 1), new DateTime(2020, 5, 1), new DateTime(2020, 6, 1) };
                //GeriyeDonukBelirtilenAylardakiHizmetBildirimlerininBiriBuEksikGunKodlarindanOlmali = new List<string> { "18", "27", "28", "29" };
                CariAydaEksikGunKoduBunlardanBiriIseKisiyeDigerBelgeTurlerideDahilTesvikVerilmeyecek = new List<string> { "18", "27", "28", "29" };
                GecmisAydaEksikGunKoduBunlardanBiriIseKisiyeDigerBelgeTurlerideDahilTesvikVerilmeyecek = new List<string> { "18", "27", "28", "29" };
                YasakliKanunlar = new List<YasakliKanun> { new YasakliKanun { Kanun = "05746", GunveyaKazancSifirdanBuyukOlmali = true, KullaniciyaSorulsun = false, TaseronGunveKazancinaBakilsin = false },
                                                           new YasakliKanun { Kanun = "15746", GunveyaKazancSifirdanBuyukOlmali = true, KullaniciyaSorulsun = false, TaseronGunveKazancinaBakilsin = false }};
                CarideKiyaslamaYaparkenDigerTesviklerMahsupsuzTutarUzerindenKiyaslanacak = true;
                AyIcindeKisideBuBelgeTurlerindenVarsaTesvikVerilmeyecek = new List<string> { "2" };
                AsgariUcretDestekTutarlariDikkateAlinsin = true;
                AsgariUcretDestegiKapsaminda = false;
            }
            else if (Kanun.Equals("5510"))
            {
                DestekKapsaminaGirmeyenBelgeTurleri = new List<string> { "2", "7", "12", "14", "19", "20", "21", "22", "23", "25", "28", "39", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "90", "91", "92" };
                DonusturulecekKanunlar = new Dictionary<string, DonusturulecekKanun> { { "07252", new DonusturulecekKanun { DonusturulecekKanunNo = "07252" } } };
                CikistanSonraGiriseTesvikVerilsin = true;
                BasvuruFormuVar = false;
                GunuSifirOlanlaraTesvikVerilsin = true;
                ToplamUcretiSifirOlanlaraTesvikVerilsin = true;
                IcmalBaslik = "5510 SAYILI KANUN KAPSAMINDAKİ TEŞVİKTEN";
                UstYaziMetni = "";
            }
            else if (Kanun.Equals("17256"))
            {
                DestekKapsaminaGirmeyenBelgeTurleri = new List<string> { "2", "7", "12", "14", "19", "20", "21", "22", "23", "25", "28", "39", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "90", "91", "92" };
                DonusturulecekKanunlar = new Dictionary<string, DonusturulecekKanun> { { "05510", new DonusturulecekKanun { DonusturulecekKanunNo = "05510" } },
                                                                                       { "46486", new DonusturulecekKanun { DonusturulecekKanunNo = "46486" } },
                                                                                       { "56486", new DonusturulecekKanun { DonusturulecekKanunNo = "56486" } },
                                                                                       { "66486", new DonusturulecekKanun { DonusturulecekKanunNo = "66486" } },
                                                                                     };
                AylikCalisanaTaseronDahilEdilsin = false;
                BazYil = false;
                BazAy = false;
                GunuSifirOlanlaraTesvikVerilsin = false;
                ToplamUcretiSifirOlanlaraTesvikVerilsin = false;
                YasakliKanunlar = new List<YasakliKanun> { new YasakliKanun { Kanun = "05746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = false, TaseronGunveKazancinaBakilsin = false },
                                                           new YasakliKanun { Kanun = "15746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = false, TaseronGunveKazancinaBakilsin = false }};
                AsgariUcretDestekTutarlariDikkateAlinsin = true;
                CikistanSonraGiriseTesvikVerilsin = true;
                BorcluAydaTesvikVerilsin = true;
                CarideKiyaslamaYaparkenDigerTesviklerMahsupsuzTutarUzerindenKiyaslanacak = true;
                CarideDonusenlerIcmaldenDusulsun = false;
                AyIcindeVarsaHicKimseyeTesvikVerilmeyecekKanunlar = new List<string> { "27256" };
                IcmalBaslik = "7256 SAYILI KANUN KAPSAMINDAKİ TEŞVİKTEN";
                UstYaziMetni = "7256 sayılı kanun ile 4447 sayılı kanunun geçici 27. maddesine eklenen";
                AsgariUcretDestegiKapsaminda = false;
                TesvikAlabilir = false;
            }
            else if (Kanun.Equals("27256"))
            {
                DestekKapsaminaGirmeyenBelgeTurleri = new List<string> { "2", "7", "12", "14", "19", "20", "21", "22", "23", "25", "28", "39", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "90", "91", "92" };
                DonusturulecekKanunlar = new Dictionary<string, DonusturulecekKanun> { { "05510", new DonusturulecekKanun { DonusturulecekKanunNo = "05510" } },
                                                                                       { "46486", new DonusturulecekKanun { DonusturulecekKanunNo = "46486" } },
                                                                                       { "56486", new DonusturulecekKanun { DonusturulecekKanunNo = "56486" } },
                                                                                       { "66486", new DonusturulecekKanun { DonusturulecekKanunNo = "66486" } },
                                                                                     };
                AylikCalisanaTaseronDahilEdilsin = false;
                BazYil = false;
                BazAy = false;
                GunuSifirOlanlaraTesvikVerilsin = false;
                ToplamUcretiSifirOlanlaraTesvikVerilsin = false;
                YasakliKanunlar = new List<YasakliKanun> { new YasakliKanun { Kanun = "05746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = false, TaseronGunveKazancinaBakilsin = false },
                                                           new YasakliKanun { Kanun = "15746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = false, TaseronGunveKazancinaBakilsin = false }};
                AsgariUcretDestekTutarlariDikkateAlinsin = true;
                CikistanSonraGiriseTesvikVerilsin = true;
                BorcluAydaTesvikVerilsin = true;
                CarideKiyaslamaYaparkenDigerTesviklerMahsupsuzTutarUzerindenKiyaslanacak = true;
                CarideDonusenlerIcmaldenDusulsun = false;
                AyIcindeVarsaHicKimseyeTesvikVerilmeyecekKanunlar = new List<string> { "17256" };
                IcmalBaslik = "7256 SAYILI KANUN KAPSAMINDAKİ TEŞVİKTEN";
                UstYaziMetni = "7256 sayılı kanun ile 4447 sayılı kanunun geçici 28. maddesine eklenen";
                TesvikMiktariAyniIseUcretiDusukOlanaVerilsin = true;
                AsgariUcretDestegiKapsaminda = false;
            }
            else if (Kanun.Equals("7316"))
            {
                BelgeTuruOraniHesaplamadaEklenecekAlanlar = new List<string> { "MalulYaslilikOraniSigortali",
                                                                               "MalulYaslilikOraniIsveren",
                                                                               "GenelSaglikSigortali",
                                                                               "GenelSaglikIsveren",
                                                                               "SosyalDestekSigortali",
                                                                               "SosyalDestekIsveren",
                                                                               "IssizlikSigortali",
                                                                               "IssizlikIsveren"
                };

                DestekKapsaminaGirmeyenBelgeTurleri = new List<string> { "2", "7", "12", "13" , "14", "19", "20", "21", "22", "23", "24", "28", "30" , "31" , "33", "34", "36", "37" , "41", "42", "43", "45", "46", "47", "48", "49", "50", "51" , "56" , "57" , "90", "91", "92" };
                DonusturulecekKanunlar = new Dictionary<string, DonusturulecekKanun> { { "05510", new DonusturulecekKanun { DonusturulecekKanunNo = "05510" } },
                                                                                       { "46486", new DonusturulecekKanun { DonusturulecekKanunNo = "46486" } },
                                                                                       { "56486", new DonusturulecekKanun { DonusturulecekKanunNo = "56486" } },
                                                                                       { "66486", new DonusturulecekKanun { DonusturulecekKanunNo = "66486" } },
                                                                                     };
                AylikCalisanaTaseronDahilEdilsin = false;
                BazYil = false;
                BazAy = false;
                GunuSifirOlanlaraTesvikVerilsin = false;
                ToplamUcretiSifirOlanlaraTesvikVerilsin = false;
                YasakliKanunlar = new List<YasakliKanun> { new YasakliKanun { Kanun = "05746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = false, TaseronGunveKazancinaBakilsin = false },
                                                           new YasakliKanun { Kanun = "15746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = false, TaseronGunveKazancinaBakilsin = false }};
                AsgariUcretDestekTutarlariDikkateAlinsin = true;
                CikistanSonraGiriseTesvikVerilsin = true;
                BorcluAydaTesvikVerilsin = false;
                CarideKiyaslamaYaparkenDigerTesviklerMahsupsuzTutarUzerindenKiyaslanacak = true;
                CarideDonusenlerIcmaldenDusulsun = false;
                IcmalBaslik = "7316 SAYILI KANUN KAPSAMINDAKİ TEŞVİKTEN";
                UstYaziMetni = "7316 sayılı kanun ile 4447 sayılı kanunun geçici 30. maddesine eklenen";
                TesvikMiktariAyniIseUcretiDusukOlanaVerilsin = true;
                AsgariUcretDestegiKapsaminda = false;
            }
            else if (Kanun.Equals("3294"))
            {
                BelgeTuruOraniHesaplamadaEklenecekAlanlar = new List<string> { "MalulYaslilikOraniIsveren", "GenelSaglikIsveren" };
                DestekKapsaminaGirmeyenBelgeTurleri = new List<string> { "2", "7", "12", "14", "19", "20", "21", "22", "23", "25", "28", "39", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "90", "91", "92" };
                DonusturulecekKanunlar = new Dictionary<string, DonusturulecekKanun> { { "05510", new DonusturulecekKanun { DonusturulecekKanunNo = "05510"  } },
                                                                                       { "46486", new DonusturulecekKanun { DonusturulecekKanunNo = "46486"  } },
                                                                                       { "56486", new DonusturulecekKanun { DonusturulecekKanunNo = "56486"  } },
                                                                                       { "66486", new DonusturulecekKanun { DonusturulecekKanunNo = "66486"  } },
                                                                                     };
                BazYil = true;
                BazAy = false;
                AylikCalisanaTaseronDahilEdilsin = false;
                GunuSifirOlanlaraTesvikVerilsin = false;
                ToplamUcretiSifirOlanlaraTesvikVerilsin = true;
                YasakliKanunlar = new List<YasakliKanun> { new YasakliKanun { Kanun = "05746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = true, TaseronGunveKazancinaBakilsin = false },
                                                           new YasakliKanun { Kanun = "15746", GunveyaKazancSifirdanBuyukOlmali = false, KullaniciyaSorulsun = true, TaseronGunveKazancinaBakilsin = false }};
                BazHesaplamaGeriyeGidilecekAySayisi = 12;
                BorcluAydaTesvikVerilsin = false;
                TesvikMiktariAyniIseUcretiDusukOlanaVerilsin = true;
                TekTesvikHakedipGunSayisiAyarlardakiMinimumGunSayisindanBuyukOlanlaraOncelikVerilsin = true;
                IcmalBaslik = "3294 SAYILI KANUN KAPSAMINDAKİ TEŞVİKTEN";
                UstYaziMetni = "3294 sayılı kanun ile";

            }

            if (DonusturulecekKanunlaraBakilsin && Program.SeciliKanunlarDonusturulsun)
            {
                bool icmaldenDusulsun = !Kanun.Equals("7166");

                var donusturulecekKanunBilgileri = Program.DonusturulecekKanunlar.Where(p => p.TesvikKanunNo.Equals(Kanun));

                if (donusturulecekKanunBilgileri.Count() > 0)
                {
                    this.DonusturulecekKanunlar.Clear();

                    foreach (var dk in donusturulecekKanunBilgileri)
                    {
                        List<string> donusturulecekKanunlarListesi = new List<string>();

                        if (TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.Contains(dk.DonusturulenKanunNo.ToInt().ToString()))
                        {
                            if (!Program.tempTesvikler.ContainsKey(dk.DonusturulenKanunNo.ToInt().ToString()))
                            {
                                Program.tempTesvikler.Add(dk.DonusturulenKanunNo.ToInt().ToString(), new Tesvik(dk.DonusturulenKanunNo.ToInt().ToString()));
                            }

                            Tesvik t = Program.tempTesvikler[dk.DonusturulenKanunNo.ToInt().ToString()];

                            if (t.AltKanunlar.Count > 0)
                            {
                                donusturulecekKanunlarListesi = t.AltKanunlar;
                            }
                            else donusturulecekKanunlarListesi.Add(dk.DonusturulenKanunNo.PadLeft(5, '0'));
                        }
                        else donusturulecekKanunlarListesi.Add(dk.DonusturulenKanunNo.PadLeft(5, '0'));

                        foreach (var dkNo in donusturulecekKanunlarListesi)
                        {
                            this.DonusturulecekKanunlar.Add(dkNo, new DonusturulecekKanun { DonusturulecekKanunNo = dkNo, SadeceCari = Convert.ToBoolean(dk.SadeceCari) });
                        }
                    }
                }
            }
        }

        public decimal BelgeTuruOranBul(int yil, int ay, string belgeturu, string IsyeriSicilNo)
        {
            int belgeturuno = Convert.ToInt32(belgeturu);

            string toplanilacakAlanlar = String.Join("-", this.BelgeTuruOraniHesaplamadaEklenecekAlanlar);

            var anahtar = belgeturuno.ToString() + "-" + toplanilacakAlanlar;

            decimal toplamoran = 0;

            if (Program.BelgeTuruOranlari.ContainsKey(anahtar))
            {
                toplamoran = Program.BelgeTuruOranlari[anahtar];
            }
            else
            {

                var belgeTuruBilgileri = Program.BelgeTurleri.ContainsKey(belgeturuno) ? Program.BelgeTurleri[belgeturuno] : null;

                if (belgeTuruBilgileri != null)
                {
                    foreach (var item in BelgeTuruOraniHesaplamadaEklenecekAlanlar)
                    {
                        if (item.Equals("GenelSaglikIsveren")) toplamoran += belgeTuruBilgileri.GenelSaglikIsveren.ToDecimal();
                        else if (item.Equals("GenelSaglikSigortali")) toplamoran += belgeTuruBilgileri.GenelSaglikSigortali.ToDecimal();
                        else if (item.Equals("MalulYaslilikOraniIsveren")) toplamoran += belgeTuruBilgileri.MalulYaslilikOraniIsveren.ToDecimal();
                        else if (item.Equals("MalulYaslilikOraniSigortali")) toplamoran += belgeTuruBilgileri.MalulYaslilikOraniSigortali.ToDecimal();
                        else if (item.Equals("SosyalDestekIsveren")) toplamoran += belgeTuruBilgileri.SosyalDestekIsveren.ToDecimal();
                        else if (item.Equals("SosyalDestekSigortali")) toplamoran += belgeTuruBilgileri.SosyalDestekSigortali.ToDecimal();
                        else if (item.Equals("IssizlikIsveren")) toplamoran += belgeTuruBilgileri.IssizlikIsveren.ToDecimal();
                        else if (item.Equals("IssizlikSigortali")) toplamoran += belgeTuruBilgileri.IssizlikSigortali.ToDecimal();
                    }

                    Program.BelgeTuruOranlari.Add(anahtar, toplamoran);
                }
                else
                {
                    if (!this.DestekKapsaminaGirmeyenBelgeTurleri.Contains(belgeturu))
                    {
                        throw new Exception(belgeturu + " nolu belge türü için oranlar girilmemiş.Lütfen yapmakta olduğunuz işlemi iptal edip belge türünü ekledikten sonra yeniden deneyiniz");
                    }
                }
            }

            decimal kvsk = Metodlar.KvskBul(yil, ay, IsyeriSicilNo);

            return toplamoran + kvsk;
        }

        public IcmalHesaplamaResult IcmalHesaplama(Kisi kisi, int yil, int ay, string belgeturu, string IsyeriSicilNo, bool AltTesviklerDahil, bool CariAyMi, Dictionary<string, Tesvik> TumTesvikler, bool AltTesvikHesaplaniyor = false, Tesvik ustTesvik = null, bool KiyasIcin=false, bool AsgariUcretDestegiVar=false)
        {
            DateTime yilAy = new DateTime(yil, ay, 1);

            int ayinSonGunu = yilAy.AddMonths(1).AddDays(-1).Day;

            var belgeTuruIstatistik = kisi.AyIstatikleri[yilAy][belgeturu];

            var tesvikKanunIstatistik = belgeTuruIstatistik.TesvikKanunuIstatistikleri[this.Kanun];

            //if (tesvikKanunIstatistik.IcmalHesaplamaSonuclari != null) return tesvikKanunIstatistik.IcmalHesaplamaSonuclari;

            var toplamIcmalEkside = false;

            var bk = Metodlar.AktifBasvuruKaydiniGetir(kisi, this.Kanun, yil, ay);

            string kanun = null;

            if (!string.IsNullOrEmpty(this.AltKanun))
            {
                kanun = this.AltKanun;
            }
            else
            {
                if (bk != null)
                {
                    kanun = bk.Kanun;
                }
                else
                {
                    kanun = this.Kanun;
                }
            }

            kanun = kanun.PadLeft(5, '0');

            bool kanunlardanBiriBaskaTesvikAlmayiEngelliyor = false;

            var toplamGunSayisi = 0;

            if (this.TesvikVerilirseDigerTesviklerIptalEdilecek)
            {
                kanunlardanBiriBaskaTesvikAlmayiEngelliyor = true;

                var verilebilecekGunSayisi = this.Basvuru_Formundaki_Baz_Sayisi_Kadar_Gun_Ay_Icinde_TesvikAlabilir ? (bk.Baz - kisi.AyIcindeOncedenAlinanTesvikGunSayisiBul(this.Kanun, yilAy)) : 30;

                var belgeTurleriIstatistikleri = kisi.AyIstatikleri[yilAy];

                var siraliBelgeTuruIstatistikleri = belgeTurleriIstatistikleri.OrderByDescending(p => this.BelgeTuruOranBul(yil, ay, p.Key, IsyeriSicilNo));

                decimal dusulecekToplamTesvikTutari = 0;

                bool EksikGunVar = false;

                foreach (var kv in belgeTurleriIstatistikleri)
                {
                    var belgeTuruTemp = kv.Key;
                    var belgeTuruIst = kv.Value;

                    foreach (var kv2 in belgeTuruIst.KanunGunveUcretleri[this.Kanun])
                    {
                        var kanunNo = kv2.Key;
                        var kanunGun = kv2.Value;

                        foreach (var satir in kanunGun.satirlar)
                        {
                            if (this.Kanun != kanunNo.ToInt().ToString() && !this.AltKanunlar.Contains(kanunNo))
                            {
                                if (satir.Kanun.EndsWith("7103"))
                                {
                                    var bfk = Metodlar.AktifBasvuruKaydiniGetir(kisi, "7166", yil, ay);

                                    if (bfk != null)
                                    {
                                        dusulecekToplamTesvikTutari += DonusturulecekKanun.DusulecekMiktarHesapla("7166", satir.Gun.ToInt(), satir.Ucret.ToDecimalSgk() + satir.Ikramiye.ToDecimalSgk(), yil, ay, belgeTuruTemp, IsyeriSicilNo, this.DonusenlerIcmaldenDusulsun, TumTesvikler)["7166"].BagliKanunlarDahilDusulecekTutar;
                                    }
                                }

                                dusulecekToplamTesvikTutari += DonusturulecekKanun.DusulecekMiktarHesapla(kanunNo, satir.Gun.ToInt(), satir.Ucret.ToDecimalSgk() + satir.Ikramiye.ToDecimalSgk(), yil, ay, belgeTuruTemp, IsyeriSicilNo, this.DonusenlerIcmaldenDusulsun, TumTesvikler)[kanunNo].BagliKanunlarDahilDusulecekTutar;
                            }
                            //var AyIcindeIseGirisTarihi = DateTime.MinValue;

                            //if (!string.IsNullOrEmpty(satir.GirisGunu))
                            //{

                            //        var kisiyil = satir.Yil;

                            //        try
                            //        {
                            //            AyIcindeIseGirisTarihi = Convert.ToDateTime(satir.GirisGunu + "/" + kisiyil);

                            //            AyIcindeIseGirisTarihi = new DateTime(Convert.ToInt32(kisiyil), AyIcindeIseGirisTarihi.Month, AyIcindeIseGirisTarihi.Day);

                            //        }
                            //        catch
                            //        {
                            //            try
                            //            {
                            //                AyIcindeIseGirisTarihi = DateTime.FromOADate(Convert.ToDouble(satir.GirisGunu));

                            //                AyIcindeIseGirisTarihi = new DateTime(Convert.ToInt32(kisiyil), AyIcindeIseGirisTarihi.Month, AyIcindeIseGirisTarihi.Day);
                            //            }
                            //            catch (Exception)
                            //            {

                            //            }

                            //        }

                            //}

                            //var AyIcindeCikisTarihi = DateTime.MinValue;

                            //if (!string.IsNullOrEmpty(satir.CikisGunu))
                            //{

                            //    var kisiyil = satir.Yil;

                            //    try
                            //    {
                            //        AyIcindeCikisTarihi = Convert.ToDateTime(satir.CikisGunu + "/" + kisiyil);

                            //        AyIcindeCikisTarihi = new DateTime(Convert.ToInt32(kisiyil), AyIcindeCikisTarihi.Month, AyIcindeCikisTarihi.Day);

                            //    }
                            //    catch
                            //    {
                            //        try
                            //        {
                            //            AyIcindeCikisTarihi = DateTime.FromOADate(Convert.ToDouble(satir.CikisGunu));

                            //            AyIcindeCikisTarihi = new DateTime(Convert.ToInt32(kisiyil), AyIcindeCikisTarihi.Month, AyIcindeCikisTarihi.Day);
                            //        }
                            //        catch (Exception)
                            //        {

                            //        }

                            //    }

                            //}

                            //if (AyIcindeCikisTarihi == DateTime.MinValue && AyIcindeIseGirisTarihi == DateTime.MinValue)
                            //{
                            //    if (satir.Gun.ToInt() < 30)
                            //    {
                            //        EksikGunVar = true;
                            //    }
                            //}
                            //else
                            //{
                            //    var aradakiGunSayisi = Math.Min(30,  (AyIcindeCikisTarihi == DateTime.MinValue ? ayinSonGunu : AyIcindeCikisTarihi.Day) - (AyIcindeIseGirisTarihi == DateTime.MinValue ? 1 : AyIcindeIseGirisTarihi.Day) + 1);

                            //    if (satir.Gun.ToInt() < aradakiGunSayisi)
                            //    {
                            //        EksikGunVar = true;
                            //    }
                            //}

                            toplamGunSayisi += satir.Gun.ToInt();
                        }
                    }
                }

                var tarihler = new List<GirisCikisTarihleri>();
                tarihler.AddRange(kisi.GirisTarihleri);
                tarihler.AddRange(kisi.CikisTarihleri);

                var ayIcindekiTarihler = tarihler.Where(p => p.Tarih.Year == yil && p.Tarih.Month == ay).OrderBy(p => p.Tarih);

                var hesaplananGunSayisi = 0;
                var oncekiGiris = DateTime.MinValue;

                foreach (var ayIcindekiTarih in ayIcindekiTarihler)
                {
                    if (ayIcindekiTarih.GirisMi == false)
                    {
                        if (oncekiGiris == DateTime.MinValue) hesaplananGunSayisi += ayIcindekiTarih.Tarih.Day;
                        else
                        {
                            hesaplananGunSayisi += (ayIcindekiTarih.Tarih.Day - oncekiGiris.Day + 1);
                        }

                        oncekiGiris = DateTime.MinValue;
                    }
                    else
                    {
                        if (oncekiGiris == DateTime.MinValue)
                        {
                            oncekiGiris = ayIcindekiTarih.Tarih;
                        }
                    }
                }

                if (oncekiGiris != DateTime.MinValue)
                {
                    hesaplananGunSayisi += (ayinSonGunu - oncekiGiris.Day + 1);
                }

                if (ayIcindekiTarihler.Count() == 0) hesaplananGunSayisi = 30;

                hesaplananGunSayisi = Math.Min(30, hesaplananGunSayisi);

                if (hesaplananGunSayisi > toplamGunSayisi) EksikGunVar = true;

                decimal toplamTesvikMiktari = 0;

                for (int i = 0; i < siraliBelgeTuruIstatistikleri.Count(); i++)
                {
                    var belgeTuruTemp = siraliBelgeTuruIstatistikleri.ElementAt(i).Key;
                    var belgeTuruIst = siraliBelgeTuruIstatistikleri.ElementAt(i).Value;

                    bool belgeTuruTesvikAlabilir = false;

                    if (belgeTuruIst.TesvikKanunuIstatistikleri[this.Kanun].TesvikAlabilir)
                    {
                        var kanunGunleri = belgeTuruIst.KanunGunveUcretleri[this.Kanun];

                        var yeniGunveUcretler = kanunGunleri.ToDictionary(x => x.Key, x => new KanunIstatistik());

                        //var bolunenSatirVar = false;

                        var yazilacakEksikGunNedeni = "";

                        for (int j = 0; j < kanunGunleri.Count; j++)
                        {
                            var kanunNo = kanunGunleri.ElementAt(j).Key;

                            if (this.DonusturulecekKanunlar.ContainsKey(kanunNo))
                            {
                                var kanunIst = kanunGunleri.ElementAt(j).Value;

                                for (int z = 0; z < kanunIst.satirlar.Count; z++)
                                {
                                    var satir = kanunIst.satirlar[z];

                                    if (satir.TesvikVerilecekMi)
                                    {
                                        if (verilebilecekGunSayisi > 0)
                                        {
                                            belgeTuruTesvikAlabilir = true;

                                            if (satir.Gun.ToInt() <= verilebilecekGunSayisi)
                                            {
                                                verilebilecekGunSayisi -= satir.Gun.ToInt();

                                                toplamTesvikMiktari += Metodlar.TesvikTutariHesapla(this.Kanun, satir.Gun.ToInt(), satir.Ucret.ToDecimalSgk() + satir.Ikramiye.ToDecimalSgk(), yil, ay, belgeTuruTemp, IsyeriSicilNo, TumTesvikler);

                                                yeniGunveUcretler[kanunNo].Gun += satir.Gun.ToInt();
                                                yeniGunveUcretler[kanunNo].Ucret += satir.Ucret.ToDecimalSgk();
                                                yeniGunveUcretler[kanunNo].Ikramiye += satir.Ikramiye.ToDecimalSgk();

                                            }
                                            else
                                            {
                                                if (this.Basvuru_Formundaki_Baz_Sayisi_Kadar_Gun_Ay_Icinde_TesvikAlabilir)
                                                {
                                                    satir.satirBolunecek = true;

                                                    satir.BolunecekSatirlar = new List<AphbSatir>();

                                                    var tesvikVerilecekSatir = satir.Clone();
                                                    tesvikVerilecekSatir.Gun = verilebilecekGunSayisi.ToString();
                                                    tesvikVerilecekSatir.Ucret = Math.Round(verilebilecekGunSayisi * ((satir.Ucret.ToDecimalSgk()) / satir.Gun.ToInt()), 2).ToString().Replace(".", ",");
                                                    tesvikVerilecekSatir.Ikramiye = Math.Round(verilebilecekGunSayisi * ((satir.Ikramiye.ToDecimalSgk()) / satir.Gun.ToInt()), 2).ToString().Replace(".", ",");
                                                    tesvikVerilecekSatir.EksikGunSayisi = (ayinSonGunu - verilebilecekGunSayisi).ToString();
                                                    tesvikVerilecekSatir.TesvikVerilecekMi = true;
                                                    tesvikVerilecekSatir.Kanun = kanun;
                                                    tesvikVerilecekSatir.TesvikHesaplanacakGun = verilebilecekGunSayisi.ToString();
                                                    tesvikVerilecekSatir.BolunenSatirMi = true;
                                                    tesvikVerilecekSatir.BolunecekSatir = satir.Clone();

                                                    satir.BolunecekSatirlar.Add(tesvikVerilecekSatir);

                                                    yeniGunveUcretler[kanunNo].Gun += tesvikVerilecekSatir.Gun.ToInt();
                                                    yeniGunveUcretler[kanunNo].Ucret += tesvikVerilecekSatir.Ucret.ToDecimalSgk();
                                                    yeniGunveUcretler[kanunNo].Ikramiye += tesvikVerilecekSatir.Ikramiye.ToDecimalSgk();

                                                    toplamTesvikMiktari += Metodlar.TesvikTutariHesapla(this.Kanun, tesvikVerilecekSatir.Gun.ToInt(), tesvikVerilecekSatir.Ucret.ToDecimalSgk() + tesvikVerilecekSatir.Ikramiye.ToDecimalSgk(), yil, ay, belgeTuruTemp, IsyeriSicilNo, TumTesvikler);

                                                    var tesvikVerilmeyecekSatir = satir.Clone();
                                                    tesvikVerilmeyecekSatir.Gun = (satir.Gun.ToInt() - verilebilecekGunSayisi).ToString();
                                                    tesvikVerilmeyecekSatir.Ucret = (satir.Ucret.ToDecimalSgk() - tesvikVerilecekSatir.Ucret.ToDecimalSgk()).ToString().Replace(".", ",");
                                                    tesvikVerilmeyecekSatir.Ikramiye = (satir.Ikramiye.ToDecimalSgk() - tesvikVerilecekSatir.Ikramiye.ToDecimalSgk()).ToString().Replace(".", ",");
                                                    tesvikVerilmeyecekSatir.EksikGunSayisi = (ayinSonGunu - tesvikVerilmeyecekSatir.Gun.ToInt()).ToString();
                                                    tesvikVerilmeyecekSatir.TesvikVerilecekMi = false;
                                                    tesvikVerilmeyecekSatir.Kanun = "00000";
                                                    tesvikVerilmeyecekSatir.TesvikHesaplanacakGun = "0";
                                                    tesvikVerilmeyecekSatir.BolunenSatirMi = true;
                                                    tesvikVerilmeyecekSatir.BolunecekSatir = satir.Clone();

                                                    satir.BolunecekSatirlar.Add(tesvikVerilmeyecekSatir);

                                                    if (/*!string.IsNullOrWhiteSpace(satir.EksikGunNedeni) && */ EksikGunVar && toplamGunSayisi < 30)
                                                    {
                                                        if (this.SatirBolundugundeDegistirilecekEksikGunKodlari.ContainsKey(satir.EksikGunNedeni.Trim().PadLeft(2, '0')))
                                                        {
                                                            tesvikVerilecekSatir.EksikGunNedeni = this.SatirBolundugundeDegistirilecekEksikGunKodlari[satir.EksikGunNedeni.Trim().PadLeft(2, '0')];
                                                            tesvikVerilmeyecekSatir.EksikGunNedeni = tesvikVerilecekSatir.EksikGunNedeni;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        tesvikVerilecekSatir.EksikGunNedeni = this.SatirBolundugundeEksikGunYoksaYazacakEksikGunNedeni;
                                                        tesvikVerilmeyecekSatir.EksikGunNedeni = tesvikVerilecekSatir.EksikGunNedeni;
                                                    }


                                                    verilebilecekGunSayisi = 0;

                                                    //bolunenSatirVar = true;

                                                    yazilacakEksikGunNedeni = tesvikVerilecekSatir.EksikGunNedeni;

                                                }

                                            }
                                        }
                                        else
                                        {
                                            satir.TesvikVerilecekMi = false;
                                            satir.TesvikHesaplanacakGun = "0";
                                        }

                                    }
                                    else
                                    {
                                        satir.TesvikHesaplanacakGun = "0";
                                    }

                                }
                            }
                        }

                        for (int j = 0; j < kanunGunleri.Count; j++)
                        {
                            var kanunNo = kanunGunleri.ElementAt(j).Key;
                            kanunGunleri[kanunNo].TesvikVerilecekGun = yeniGunveUcretler[kanunNo].Gun;
                            kanunGunleri[kanunNo].TesvikVerilecekUcret = yeniGunveUcretler[kanunNo].Ucret;
                            kanunGunleri[kanunNo].TesvikVerilecekIkramiye = yeniGunveUcretler[kanunNo].Ikramiye;

                            if (!belgeTuruTesvikAlabilir)
                            {
                                kanunGunleri[kanunNo].TesvikVerilecek = false;

                                kanunGunleri[kanunNo].satirlar.ForEach(p =>
                                {
                                    p.TesvikVerilecekMi = false;
                                    p.TesvikHesaplanacakGun = "0";
                                });
                            }

                            //if (bolunenSatirVar)
                            //{
                            //    var satirlar = kanunGunleri[kanunNo].satirlar;

                            //    for (int m = 0; m < satirlar.Count; m++)
                            //    {
                            //        if (!satirlar[m].TesvikVerilecekMi)
                            //        {
                            //            satirlar[m].EksikGunNedeni = yazilacakEksikGunNedeni;
                            //        }
                            //    }

                            //}
                        }
                    }

                    if (!belgeTuruTesvikAlabilir)
                    {
                        belgeTuruIst.TesvikKanunuIstatistikleri[this.Kanun].TesvikAlabilir = false;
                        var kanunGunleri = belgeTuruIst.KanunGunveUcretleri[this.Kanun];

                        foreach (var item in kanunGunleri)
                        {
                            item.Value.satirlar.ForEach(p =>
                            {
                                p.TesvikVerilecekMi = false;
                                p.TesvikHesaplanacakGun = "0";
                            });
                        }
                    }
                }

                if (KiyasIcin)
                {
                    if (! CariAyMi && this.Kanun == "7252" && AsgariUcretDestegiVar)
                    {
                        toplamTesvikMiktari -= toplamGunSayisi * 2.5m;
                    }
                }

                toplamIcmalEkside = dusulecekToplamTesvikTutari > toplamTesvikMiktari;
            }



            decimal AsgariUcret = Metodlar.AsgariUcretBul(yil, ay);

            int tesvikVerilecekToplamGun = 0;
            int toplamGun = 0;
            decimal toplamUcret = 0;
            decimal toplamIkramiye = 0;

            Dictionary<DonusturulecekKanun, Dictionary<string, TesvikTutariIstatistik>> sonuc = new Dictionary<DonusturulecekKanun, Dictionary<string, TesvikTutariIstatistik>>();
            Dictionary<DonusturulecekKanun, Dictionary<string, TesvikTutariIstatistik>> sonucTumu = new Dictionary<DonusturulecekKanun, Dictionary<string, TesvikTutariIstatistik>>();

            Dictionary<string, KeyValuePair<DonusturulecekKanun, Classes.KanunIstatistik>> donusturulecekKanunlar = new Dictionary<string, KeyValuePair<DonusturulecekKanun, Classes.KanunIstatistik>>();



            foreach (var item in belgeTuruIstatistik.KanunGunveUcretleri[this.Kanun])
            {
                var donusturulecekKanunNo = item.Key;

                var kanunIstatistik = item.Value;

                //if (donusturulecekKanunNo.EndsWith("7103"))
                //{
                //    //if (!this.Kanun.Equals("7166") && !(ustTesvik != null && ustTesvik.Kanun.Equals("7166")))
                //    //{
                //        if (kisi.KisiBasvuruKayitlari.ContainsKey("7166"))
                //        {
                //            foreach (var bk in kisi.KisiBasvuruKayitlari["7166"])
                //            {
                //                if (yilAy >= bk.TesvikDonemiBaslangic && yilAy <= bk.TesvikDonemiBitis)
                //                {
                //                    if (this.dtKurulusTarihi < Program.TumTesvikler["7166"].KurulusTarihiBuTarihtenBuyukveyeEsitseTesvikVerilmesin && Program.TumTesvikler["7166"].BuYillardaHicBildirgeYoksaTesvikVerilmesin.All(p => Program.TumTesvikler["7166"].BildirgeOlanYillar.Contains(p)))
                //                    {
                //                        donusturulecekKanunNo = "07166";

                //                        break;
                //                    }
                //                }
                //            }
                //        }
                //    //}
                //}

                var hesaplanacakKanun = kanun;

                if (donusturulecekKanunNo.EndsWith(this.Kanun))
                {
                    hesaplanacakKanun = donusturulecekKanunNo;
                }

                bool TesvikIcinde5510Var = TesvikHesaplamaSabitleri.BagliKanunlarIcmalHesaplama.ContainsKey(hesaplanacakKanun) && TesvikHesaplamaSabitleri.BagliKanunlarIcmalHesaplama[hesaplanacakKanun].Contains("05510");

                var tesvikDonusturulecekKanunlar = AltTesvikHesaplaniyor ? ustTesvik.DonusturulecekKanunlar : this.DonusturulecekKanunlar;

                if (tesvikDonusturulecekKanunlar.ContainsKey(donusturulecekKanunNo))
                {
                    DonusturulecekKanun donusturulecekKanun = tesvikDonusturulecekKanunlar[donusturulecekKanunNo];

                    bool devam = true;

                    if (donusturulecekKanun.SadeceCari)
                    {
                        if (!CariAyMi)
                        {
                            devam = false;
                        }
                    }

                    if (devam)
                    {
                        decimal tempIcmal = 0m;

                        if (hesaplanacakKanun.Equals("06111"))
                        {
                            tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                        }
                        else if (hesaplanacakKanun.Equals("00687"))
                        {
                            tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                        }
                        else if (hesaplanacakKanun.Equals("01687"))
                        {
                            tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                        }
                        else if (hesaplanacakKanun.Equals("06645"))
                        {
                            tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                        }
                        else if (hesaplanacakKanun.Equals("17103"))
                        {
                            if (this.Kanun.Equals("7103"))
                            {
                                tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                            }
                            else if (this.Kanun.Equals("7166"))
                            {
                                tempIcmal += Metodlar.TesvikTutariHesapla("07166", kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                            }

                        }
                        else if (hesaplanacakKanun.Equals("27103"))
                        {
                            if (this.Kanun.Equals("7103"))
                            {
                                tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                            }
                            else if (this.Kanun.Equals("7166"))
                            {
                                tempIcmal += Metodlar.TesvikTutariHesapla("07166", kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                            }

                        }
                        else if (hesaplanacakKanun.Equals("02828"))
                        {
                            tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                        }
                        else if (hesaplanacakKanun.Equals("14857"))
                        {
                            tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                        }
                        else if (hesaplanacakKanun.EndsWith("6486"))
                        {
                            tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                        }
                        else if (hesaplanacakKanun.Equals("16322"))
                        {
                            tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                        }
                        else if (hesaplanacakKanun.Equals("26322"))
                        {
                            tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                        }
                        else if (hesaplanacakKanun.Equals("25510"))
                        {
                            tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                        }
                        else if (hesaplanacakKanun.Equals("07252"))
                        {
                            tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);

                            if (KiyasIcin && !CariAyMi && AsgariUcretDestegiVar)
                            {
                                tempIcmal -= kanunIstatistik.Gun * 2.5m;
                            }
                        }
                        else if (hesaplanacakKanun.Equals("05510"))
                        {
                            tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                        }
                        else if (hesaplanacakKanun.Equals("17256"))
                        {
                            tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                        }
                        else if (hesaplanacakKanun.Equals("27256"))
                        {
                            tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                        }
                        else if (hesaplanacakKanun.Equals("07316"))
                        {
                            tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                        }
                        else if (hesaplanacakKanun.Equals("03294"))
                        {
                            tempIcmal += Metodlar.TesvikTutariHesapla(hesaplanacakKanun, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);
                        }

                        donusturulecekKanunlar.Add(donusturulecekKanunNo, new KeyValuePair<DonusturulecekKanun, Classes.KanunIstatistik>(donusturulecekKanun, kanunIstatistik));

                        var Tutar5510 = Metodlar.TesvikTutariHesapla("05510", kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, TumTesvikler);

                        var Tutar5510AsgariUcretUzerinden = kanunIstatistik.TesvikVerilecekGun * AsgariUcret * 0.05m;

                        if (TesvikIcinde5510Var)
                        {

                            if (donusturulecekKanunNo.Equals("00000"))
                            {
                                tempIcmal += Tutar5510AsgariUcretUzerinden;
                            }
                            else
                            {
                                tempIcmal += Tutar5510;
                            }
                        }

                        tesvikVerilecekToplamGun += kanunIstatistik.TesvikVerilecekGun;
                        toplamGun += kanunIstatistik.Gun;
                        toplamUcret += kanunIstatistik.TesvikVerilecekUcret;
                        toplamIkramiye += kanunIstatistik.TesvikVerilecekIkramiye;

                        Dictionary<string, DusulecekTutarIstastistik> dusulecekMiktar = null;


                        if (this.TesvikVerilirseDigerTesviklerIptalEdilecek)
                        {
                            dusulecekMiktar = DonusturulecekKanun.DusulecekMiktarHesapla(donusturulecekKanunNo, kanunIstatistik.Gun, kanunIstatistik.ToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, CariAyMi ? this.CarideDonusenlerIcmaldenDusulsun : this.DonusenlerIcmaldenDusulsun, TumTesvikler);
                        }
                        else
                        {
                            var dusulecekGun = kanunIstatistik.TesvikVerilecekGun;
                            var dusulecekToplamUcret = kanunIstatistik.TesvikVerilecekToplamUcret;

                            var donusturulenTesvik = TumTesvikler.FirstOrDefault(p => p.Key.PadLeft(5, '0') == donusturulecekKanunNo || p.Value.AltKanunlar.Contains(donusturulecekKanunNo)).Value;

                            if (donusturulenTesvik != null)
                            {
                                //if (!donusturulenTesvik.TesvikVerilirseDigerTesviklerIptalEdilecek)
                                //{
                                if (donusturulenTesvik.Basvuru_Formundaki_Baz_Sayisi_Kadar_Gun_Ay_Icinde_TesvikAlabilir)
                                {
                                    dusulecekGun = kisi.KisininAlabilecegiGunSayisiniBul(donusturulenTesvik.Kanun, belgeturu, kanunIstatistik.TesvikVerilecekGun, yil, ay, IsyeriSicilNo);

                                    dusulecekToplamUcret = kanunIstatistik.Gun > 0 ? (kanunIstatistik.ToplamUcret / kanunIstatistik.Gun * dusulecekGun) : 0;

                                    /*
                                    var aktifBasvuru = Metodlar.AktifBasvuruKaydiniGetir(kisi, donusturulenTesvik.Kanun, yil, ay);

                                    if (aktifBasvuru != null)
                                    {

                                        int verilebilecekToplamGunSayisi = aktifBasvuru.Baz ;

                                        var belgeTurleriIstatistikleri = kisi.AyIstatikleri[yilAy];

                                        var siraliBelgeTuruIstatistikleri = belgeTurleriIstatistikleri.OrderByDescending(p => donusturulenTesvik.BelgeTuruOranBul(yil, ay, p.Key, IsyeriSicilNo));

                                        foreach (var kvBelgeTuru in siraliBelgeTuruIstatistikleri)
                                        {
                                            string bt = kvBelgeTuru.Key;

                                            var belgeIst = kvBelgeTuru.Value;

                                            if (belgeIst.KanunGunveUcretleri[this.Kanun].ContainsKey(donusturulecekKanunNo))
                                            {
                                                var kanungun = belgeIst.KanunGunveUcretleri[this.Kanun][donusturulecekKanunNo];

                                                if (kanungun.TesvikVerilecekGun > 0)
                                                {

                                                    if (kanungun.TesvikVerilecekGun > verilebilecekToplamGunSayisi)
                                                    {
                                                        if (belgeturu == bt)
                                                        {
                                                            dusulecekToplamUcret = (kanungun.TesvikVerilecekToplamUcret / kanungun.TesvikVerilecekGun ) * verilebilecekToplamGunSayisi;
                                                            dusulecekGun = verilebilecekToplamGunSayisi;
                                                        }

                                                        verilebilecekToplamGunSayisi = 0;
                                                    }
                                                    else
                                                    {
                                                        if (belgeturu == bt)
                                                        {
                                                            dusulecekToplamUcret = kanungun.TesvikVerilecekToplamUcret;
                                                            dusulecekGun = kanungun.TesvikVerilecekGun;
                                                        }

                                                        verilebilecekToplamGunSayisi -= kanungun.TesvikVerilecekGun;
                                                    }
                                                }
                                                else
                                                {
                                                    dusulecekGun = 0;
                                                    dusulecekToplamUcret = 0;
                                                }


                                            }

                                        }
                                    }
                                    */
                                }
                                //}
                                //else
                                //{

                                //var ayIstatistikTumu = kisi.AyIstatikleri[yilAy];

                                //foreach (var kv in ayIstatistikTumu)
                                //{
                                //    var dusulecekBelgeTuru = kv.Key;
                                //    var dusulecekBelgeTuruIstatistik = kv.Value;

                                //    foreach (var kv2 in dusulecekBelgeTuruIstatistik.KanunGunveUcretleri[this.Kanun])
                                //    {
                                //        string ayIcindekiKanun = kv2.Key;
                                //        var kanunIst = kv2.Value;

                                //        var tesvikAyIcindekiKanun = TumTesvikler.FirstOrDefault(p => p.Key == ayIcindekiKanun.ToInt().ToString() || p.Value.AltKanunlar.Contains(ayIcindekiKanun)).Value;

                                //        if (tesvikAyIcindekiKanun != null)
                                //        {
                                //            if (tesvikAyIcindekiKanun.Kanun == donusturulenTesvik.Kanun)
                                //            {
                                //                var dusulecekGun2 = kisi.KisininAlabilecegiGunSayisiniBul(donusturulenTesvik.Kanun, dusulecekBelgeTuru, kanunIst.TesvikVerilecekGun, yil, ay, IsyeriSicilNo);

                                //                var dusulecekToplamUcret2 = kanunIst.Gun > 0 ? (kanunIst.ToplamUcret / kanunIst.Gun * dusulecekGun2) : 0;

                                //                var dusulecekTutarlar = DonusturulecekKanun.DusulecekMiktarHesapla(ayIcindekiKanun, dusulecekGun2, dusulecekToplamUcret2, yil, ay, dusulecekBelgeTuru, IsyeriSicilNo, CariAyMi ? this.CarideDonusenlerIcmaldenDusulsun : this.DonusenlerIcmaldenDusulsun, TumTesvikler);

                                //                toplamDusulecekTutar += dusulecekTutarlar[ayIcindekiKanun].BagliKanunlarDahilDusulecekTutar;
                                //            }
                                //        }

                                //    }
                                //}
                                //}

                                if (donusturulenTesvik.TesvikVerilirseDigerTesviklerIptalEdilecek)
                                {
                                    kanunlardanBiriBaskaTesvikAlmayiEngelliyor = true;
                                }
                            }

                            dusulecekMiktar = DonusturulecekKanun.DusulecekMiktarHesapla(donusturulecekKanunNo, dusulecekGun, dusulecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, CariAyMi ? this.CarideDonusenlerIcmaldenDusulsun : this.DonusenlerIcmaldenDusulsun, TumTesvikler);

                            /*
                            if (donusturulecekKanunNo.EndsWith("7252") && KiyasIcin && !CariAyMi)
                            {
                                foreach (var dm in dusulecekMiktar)
                                {

                                    dm.Value.BagliKanunlarDahilDusulecekTutar -= kanunIstatistik.Gun * 2.5d;
                                    dm.Value.BagliKanunlarHaricDusulecekTutar -= kanunIstatistik.Gun * 2.5d;
                                }
                            }
                            */

                        }


                        sonuc.Add(donusturulecekKanun, new Dictionary<string, TesvikTutariIstatistik>());

                        var bagliKanunlar = sonuc[donusturulecekKanun];

                        foreach (var dm in dusulecekMiktar)
                        {
                            bagliKanunlar.Add(dm.Key, new TesvikTutariIstatistik
                            {
                                Mahsupsuz = tempIcmal,
                                MahsupluTutarBagliKanunlarDahil = tempIcmal - dm.Value.BagliKanunlarDahilDusulecekTutar,
                                MahsupMiktariBagliKanunlarDahil = dm.Value.BagliKanunlarDahilDusulecekTutar,
                                MahsupluTutarBagliKanunlarHaric = tempIcmal - dm.Value.BagliKanunlarHaricDusulecekTutar,
                                MahsupMiktariBagliKanunlarHaric = dm.Value.BagliKanunlarHaricDusulecekTutar
                            });
                        }
                    }
                }
                else
                {
                    if (this.TesvikVerilirseDigerTesviklerIptalEdilecek)
                    {
                        if (this.Kanun != donusturulecekKanunNo.ToInt().ToString() && !this.AltKanunlar.Contains(donusturulecekKanunNo))
                        {


                            var donusturulenTesvik = TumTesvikler.FirstOrDefault(p => p.Key.PadLeft(5, '0') == donusturulecekKanunNo || p.Value.AltKanunlar.Contains(donusturulecekKanunNo)).Value;

                            if (donusturulenTesvik != null)
                            {
                                if (donusturulenTesvik.TesvikVerilirseDigerTesviklerIptalEdilecek)
                                {
                                    kanunlardanBiriBaskaTesvikAlmayiEngelliyor = true;
                                }
                            }


                            var dusulecekMiktar = DonusturulecekKanun.DusulecekMiktarHesapla(donusturulecekKanunNo, kanunIstatistik.Gun, kanunIstatistik.ToplamUcret, yil, ay, belgeturu, IsyeriSicilNo, CariAyMi ? this.CarideDonusenlerIcmaldenDusulsun : this.DonusenlerIcmaldenDusulsun, TumTesvikler);

                            /*
                            if (donusturulecekKanunNo.EndsWith("7252") && KiyasIcin && !CariAyMi)
                            {
                                foreach (var dm in dusulecekMiktar)
                                {

                                    dm.Value.BagliKanunlarDahilDusulecekTutar -= kanunIstatistik.Gun * 2.5d;
                                    dm.Value.BagliKanunlarHaricDusulecekTutar -= kanunIstatistik.Gun * 2.5d;
                                }
                            }
                            */

                            var dk = new DonusturulecekKanun { DonusturulecekKanunNo = donusturulecekKanunNo };

                            sonuc.Add(dk, new Dictionary<string, TesvikTutariIstatistik>());

                            var bagliKanunlar = sonuc[dk];

                            var tempIcmal = 0m;
                            var kiyaslamadaMutlakaDusulecekTutar = 0m;

                            if (KiyasIcin && !CariAyMi && this.Kanun == "7252" && AsgariUcretDestegiVar)
                            {
                                tempIcmal -= kanunIstatistik.Gun * 2.5m;

                                kiyaslamadaMutlakaDusulecekTutar = kanunIstatistik.Gun * 2.5m;
                            }

                            foreach (var dm in dusulecekMiktar)
                            {

                                bagliKanunlar.Add(dm.Key, new TesvikTutariIstatistik
                                {
                                    Mahsupsuz = tempIcmal,
                                    MahsupluTutarBagliKanunlarDahil = tempIcmal - dm.Value.BagliKanunlarDahilDusulecekTutar,
                                    MahsupMiktariBagliKanunlarDahil = dm.Value.BagliKanunlarDahilDusulecekTutar,
                                    MahsupluTutarBagliKanunlarHaric = tempIcmal - dm.Value.BagliKanunlarHaricDusulecekTutar,
                                    MahsupMiktariBagliKanunlarHaric = dm.Value.BagliKanunlarHaricDusulecekTutar,
                                    KiyaslamadaMutlakaDusulecekTutar = kiyaslamadaMutlakaDusulecekTutar
                                });
                            }
                        }

                        //donusturulecekKanunlar.Add(donusturulecekKanunNo, new KeyValuePair<DonusturulecekKanun, Classes.KanunIstatistik>(new DonusturulecekKanun { 
                        //    DonusturulecekKanunNo = donusturulecekKanunNo
                        //}, kanunIstatistik));
                    }

                    kanunIstatistik.TesvikVerilecek = false;
                }

            }

            //foreach (var item in donusturulecekKanunlar)
            //{
            //    var donusturulecekKanunNo = item.Key;

            //    var donusturulecekKanun = item.Value.Key;

            //    var kanunIstatistik = item.Value.Value;

            //    toplamGun += kanunIstatistik.TesvikVerilecekGun;
            //    toplamUcret += kanunIstatistik.TesvikVerilecekUcret;
            //    toplamIkramiye += kanunIstatistik.TesvikVerilecekIkramiye;

            //    var dusulecekMiktar = donusturulecekKanun.DusulecekMiktarHesapla(donusturulecekKanunNo, kanunIstatistik.TesvikVerilecekGun, kanunIstatistik.TesvikVerilecekToplamUcret, yil, ay, belgeturu, IsyeriSicilNo);

            //    sonuc.Add(donusturulecekKanun, new Dictionary<string, TesvikTutariIstatistik>());

            //    var bagliKanunlar = sonuc[donusturulecekKanun];

            //    bool icmalEkside = (dusulecekMiktar.Any(p => (icmal - p.Value.BagliKanunlarDahilDusulecekTutar) < 0));

            //    foreach (var dm in dusulecekMiktar)
            //    {

            //        bagliKanunlar.Add(dm.Key, new TesvikTutariIstatistik
            //        {
            //            Mahsupsuz = icmal,
            //            MahsupluTutarBagliKanunlarDahil = icmal - dm.Value.BagliKanunlarDahilDusulecekTutar,
            //            MahsupMiktariBagliKanunlarDahil = dm.Value.BagliKanunlarDahilDusulecekTutar,
            //            MahsupluTutarBagliKanunlarHaric = icmal - dm.Value.BagliKanunlarHaricDusulecekTutar,
            //            MahsupMiktariBagliKanunlarHaric = dm.Value.BagliKanunlarHaricDusulecekTutar
            //        });
            //    }
            //}


            if (AltTesviklerDahil)
            {
                foreach (var altTesvikKanunNo in altTesvikler)
                {
                    var altTesvik = Program.TumTesvikler[altTesvikKanunNo];

                    var altTesvikSonuc = altTesvik.IcmalHesaplama(kisi, yil, ay, belgeturu, IsyeriSicilNo, AltTesviklerDahil, CariAyMi, TumTesvikler, true, this,KiyasIcin, AsgariUcretDestegiVar);

                    if (altTesvikSonuc.KanunlardanBiriBaskaTesvikAlmayiEngelliyor)
                    {
                        kanunlardanBiriBaskaTesvikAlmayiEngelliyor = true;
                    }

                    foreach (var item in altTesvikSonuc.icmaller)
                    {
                        var bagliKanunlar = sonuc.FirstOrDefault(p => p.Key.DonusturulecekKanunNo.Equals(item.Key.DonusturulecekKanunNo)).Value;

                        if (bagliKanunlar == null) sonuc.Add(item.Key, new Dictionary<string, TesvikTutariIstatistik>());

                        bagliKanunlar = sonuc.FirstOrDefault(p => p.Key.DonusturulecekKanunNo.Equals(item.Key.DonusturulecekKanunNo)).Value;

                        foreach (var item2 in item.Value)
                        {
                            if (!bagliKanunlar.ContainsKey(item2.Key)) bagliKanunlar.Add(item2.Key, new TesvikTutariIstatistik());

                            var tti = bagliKanunlar[item2.Key];

                            tti.MahsupluTutarBagliKanunlarDahil += item2.Value.MahsupluTutarBagliKanunlarDahil;
                            tti.MahsupluTutarBagliKanunlarHaric += item2.Value.MahsupluTutarBagliKanunlarHaric;
                            tti.MahsupMiktariBagliKanunlarDahil += item2.Value.MahsupMiktariBagliKanunlarDahil;
                            tti.MahsupMiktariBagliKanunlarHaric += item2.Value.MahsupMiktariBagliKanunlarHaric;
                            tti.Mahsupsuz += item2.Value.Mahsupsuz;
                        }
                    }

                }
            }

            if (!AltTesvikHesaplaniyor)
            {

                List<DonusturulecekKanun> icmaliEksideOlanlar = new List<DonusturulecekKanun>();

                foreach (var item in sonuc)
                {
                    if (toplamIcmalEkside || !this.TesvikVerilirseDigerTesviklerIptalEdilecek)
                    {
                        if (item.Value[item.Key.DonusturulecekKanunNo].MahsupluTutarBagliKanunlarDahil < 0)
                        {
                            icmaliEksideOlanlar.Add(item.Key);

                            if (donusturulecekKanunlar.ContainsKey(item.Key.DonusturulecekKanunNo))
                            {
                                var kanunIstatistik = donusturulecekKanunlar[item.Key.DonusturulecekKanunNo].Value;

                                kanunIstatistik.TesvikVerilecek = false;

                                tesvikVerilecekToplamGun -= kanunIstatistik.TesvikVerilecekGun;
                                toplamGun -= kanunIstatistik.Gun;
                                toplamUcret -= kanunIstatistik.TesvikVerilecekUcret;
                                toplamIkramiye -= kanunIstatistik.TesvikVerilecekIkramiye;
                            }
                        }
                    }
                }

                sonucTumu = sonuc.ToDictionary(x => x.Key, x => x.Value);

                icmaliEksideOlanlar.ForEach(p =>
                {
                    sonuc.Remove(p);
                });

                tesvikKanunIstatistik.TesvikVerilecekGun = tesvikVerilecekToplamGun;
                tesvikKanunIstatistik.ToplamGun = toplamGun;
                tesvikKanunIstatistik.Ucret = toplamUcret;
                tesvikKanunIstatistik.Ikramiye = toplamIkramiye;
            }

            tesvikKanunIstatistik.IcmalHesaplamaSonuclari = sonuc;

            return new IcmalHesaplamaResult { icmaller = sonuc, tumIcmaller = sonucTumu , ToplamIcmalEkside = toplamIcmalEkside, KanunlardanBiriBaskaTesvikAlmayiEngelliyor = kanunlardanBiriBaskaTesvikAlmayiEngelliyor };
        }

        public decimal GunlukKazancSiniriGetir(int Yil, int Ay, string IsyeriSicilNo)
        {
            var asgariucret = Metodlar.AsgariUcretBul(Yil, Ay);

            var belgeturuorani = BelgeTuruOranBul(Yil, Ay, "1", IsyeriSicilNo);

            return asgariucret * 100 / belgeturuorani;
        }

        public Dictionary<string, int> BazSayilari = new Dictionary<string, int>();

        public Dictionary<DateTime, TesvikAyIstatistik> TesvikAyIstatistikleri = new Dictionary<DateTime, TesvikAyIstatistik>();

        public void TesvikAyIstatistikSil(DateTime dt)
        {
            if (TesvikAyIstatistikleri.ContainsKey(dt)) TesvikAyIstatistikleri.Remove(dt);
        }
    }
}
