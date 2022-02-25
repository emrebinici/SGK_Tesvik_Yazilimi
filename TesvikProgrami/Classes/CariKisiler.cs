using System;
using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class CariKisiler
    {
        public List<CariKisi> Kisiler { get; set; } = new List<CariKisi>();

        public List<IseGirisCikisKaydi> IseGirisCikisKayitlari = new List<IseGirisCikisKaydi>();
        
        public DateTime SorgulananDonem { get; set; }
        
        public DateTime SorgulamaTarihi { get; set; }



    }

}
