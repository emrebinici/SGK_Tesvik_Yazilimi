using System;
using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class KisiTesvik
    {
        public KisiTesvik(Classes.Kisi kisi, string kanun, Dictionary<string, decimal> tesviktutarlari)
        {

            Kisi = kisi;

            Kanun = kanun;

            TesvikTutarlari = tesviktutarlari;
        }

        public Classes.Kisi Kisi;

        public string Kanun;

        public Dictionary<string,decimal> TesvikTutarlari;

    }

}
