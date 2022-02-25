using System;
using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class TesvikKanunuIstatistik
    {
        public int TesvikVerilecekGun;
        public int ToplamGun;
        public decimal Ucret;
        public decimal Ikramiye;
        public decimal ToplamUcret { get { return Ucret + Ikramiye; } }
        public bool TesvikAlabilir = true;
        public bool AraciMi = true;
        public List<AphbSatir> satirlar = new List<AphbSatir>();

        public int TaseronluGunSayisi;
        public decimal TaseronluUcret;
        public decimal TaseronluIkramiye;
        public decimal TaseronluToplamUcret { get { return TaseronluUcret + TaseronluIkramiye; } }
        public List<AphbSatir> TaseronluSatirlar = new List<AphbSatir>();

        public Dictionary<DonusturulecekKanun, Dictionary<string, TesvikTutariIstatistik>> IcmalHesaplamaSonuclari;

    }

}
