using System;

namespace TesvikProgrami.Classes
{
    public class GirisCikisTarihleri
    {
        public GirisCikisTarihleri() { }

        public DateTime Tarih { get; set; }

        public string Yil;

        public string Ay;

        public string Kanun;

        public string belgeturu;

        public string Mahiyet;

        public string Araci { get; set; }

        public string IstenCikisNedeni { get; set; }

        public bool GirisMi { get; set; }
    }

}
