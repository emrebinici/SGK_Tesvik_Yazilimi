using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class KanunIstatistik
    {
        public int TaseronluGunSayisi;
        public decimal TaseronluUcret;
        public decimal TaseronluIkramiye;
        public decimal TaseronluToplamUcret { get { return TaseronluUcret + TaseronluIkramiye; } }

        public int TesvikVerilecekGun;
        public decimal TesvikVerilecekUcret;
        public decimal TesvikVerilecekIkramiye;
        public decimal TesvikVerilecekToplamUcret { get { return TesvikVerilecekUcret + TesvikVerilecekIkramiye; } }

        public int Gun;
        public decimal Ucret;
        public decimal Ikramiye;
        public decimal ToplamUcret { get { return Ucret + Ikramiye; } }

        public bool AraciMi = true;

        public List<AphbSatir> satirlar = new List<AphbSatir>();

        public List<AphbSatir> TaseronluSatirlar = new List<AphbSatir>();

        public bool TesvikVerilecek = true;


    }

}
