using System;
using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class BazBilgileri
    {
        public BazBilgileri()
        {
            BazveDonemler = new List<Classes.BazBilgisi>();
        }

        public List<Classes.BazBilgisi> BazveDonemler { get; set; }

        public void Ekle(Classes.BazBilgisi bazbilgisi)
        {
            bool AynisiVar = false;

            foreach (var item in BazveDonemler)
            {
                if (bazbilgisi.Donem == item.Donem /*&& bazbilgisi.Baz == item.Baz*/)
                {
                    AynisiVar = true;
                }
            }

            if (!AynisiVar) BazveDonemler.Add(bazbilgisi);
        }

        public Classes.BazBilgisi Bul(DateTime donem/*, int baz*/)
        {
            foreach (var item in BazveDonemler)
            {
                if (item.Donem == donem /*&& item.Baz == baz*/)
                {
                    return item;
                }
            }

            Classes.BazBilgisi bazbilgisi = new Classes.BazBilgisi(donem/*, baz*/);

            BazveDonemler.Add(bazbilgisi);

            return bazbilgisi;
        }
    }

}
