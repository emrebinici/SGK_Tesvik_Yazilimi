using System;
using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class TumKisilerSonuc
    {
        public Dictionary<string, List<System.Data.DataRow>> AySatirlari = null;

        public List<string> TumAylar = null;

        public Dictionary<string, Dictionary<string, List<System.Data.DataRow>>> KisilerinSatirlari = null;

        public Dictionary<string, Dictionary<string, List<System.Data.DataRow>>> KisilerinSatirlariIptallerDahil = null;

        public  Dictionary<string, Kisi> TumKisiler = null;

        public DateTime enbuyukay { get; set; }
        public Dictionary<KeyValuePair<string, string>, List<KeyValuePair<int, int>>> tumyilveaylar { get; set; } = new Dictionary<KeyValuePair<string, string>, List<KeyValuePair<int, int>>>();

        public List<KeyValuePair<string, string>> yilveaylar { get; set; } = new List<KeyValuePair<string, string>>();

        public Dictionary<string, List<string>> TesvikVerilenler { get; set; } = new Dictionary<string, List<string>>();
    }

}
