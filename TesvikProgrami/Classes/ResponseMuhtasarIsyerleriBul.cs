using System;
using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class ResponseMuhtasarIsyerleriBul
    {
        public List<string> Mesajlar { get; set; } = new List<string>();

        public List<MuhtasarIsyeri> MuhtasarIsyerleri { get; set; } = new List<MuhtasarIsyeri>();
        
        public HashSet<string> KayitliOlmayanIsyerleri { get; set; } = new HashSet<string>();

        public bool BaskaSirketMi { get; set; }

        public List<string> HataliSatirlar { get; set; }

        public bool MuhtasardaBirdenFazlaSayfaVar { get; set; }
    }
}
