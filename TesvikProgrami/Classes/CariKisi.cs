using System;
using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class CariKisi
    {
        public string TcKimlikNo { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Ilk_Soyad { get; set; }
        public string MeslekKod { get; set; }
        public string Kanun { get; set; }
        public string BelgeTuru { get; set; }
        public int Gun { get; set; } = 30;
        public decimal GunlukOrtalamaUcret { get; set; }
        public string Araci { get; set; }
        public DateTime CikisAyi { get; set; }


    }

}
