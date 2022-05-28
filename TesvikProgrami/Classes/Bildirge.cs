using System;
using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class Bildirge : EqualityComparer<Bildirge>
    {
        public Bildirge()
        {
            Kisiler = new List<AphbSatir>();

            AraciveyaIsveren = "Ana İşveren";

            Askida = false;
        }
        public string Yil { get; set; }
        public string Ay { get; set; }
        public string BelgeTuru { get; set; }
        public string Mahiyet { get; set; }
        public string Kanun { get; set; }
        public string AraciveyaIsveren { get; set; }
        public List<AphbSatir> Kisiler { get; set; }
        public bool Askida { get; set; }
        public string OrijinalKanunNo { get; set; }
        public string EkBilgiler { get; set; }
        public string RefNo { get; set; }
        public int ToplamGun { get; set; }
        public decimal ToplamUcret { get; set; }
        public DateTime ilkKayitTarihi { get; set; }
        public bool Duzeltilecek { get; set; }

        public override bool Equals(Bildirge b1, Bildirge b2)
        {
            if (b1 == null && b2 == null)
                return true;
            else if (b1 == null || b2 == null)
                return false;

            if (b1.Askida == b2.Askida && b1.RefNo.Equals(b2.RefNo) && b1.AraciveyaIsveren.Equals(b2.AraciveyaIsveren))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode(Bildirge bildrige)
        {
            return base.GetHashCode();
        }

    }

}
