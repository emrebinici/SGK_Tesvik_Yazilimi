using System;
using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class Cikti
    {
        public string BelgeTuru;

        public string Kanun;

        public Dictionary<Classes.Kisi, string> Kisiler = new Dictionary<Classes.Kisi, string>();

        public bool Asil = true;

        public bool Iptal = false;

        public bool BildirgeOlustur = true;

        public bool ExcelOlustur = true;

        public List<AphbSatir> satirlar = new List<AphbSatir>();

        public List<AphbSatir> muhtasarSatirlar = new List<AphbSatir>();
        
        public List<AphbSatir> muhtasarIptalSatirlar = new List<AphbSatir>();

        public decimal ToplamTutar = 0;

        public decimal Matrah;

        public int Gun;
 
        public bool XmlOlustur = false;

        public string EkBilgiler;

        public bool MinimumTutarKontrolEdilecek = true;

        public decimal Matrah_Tesvik_Verilmeyenler_Dahil;
        
        public int Gun_Tesvik_Verilmeyenler_Dahil;


    }

}
