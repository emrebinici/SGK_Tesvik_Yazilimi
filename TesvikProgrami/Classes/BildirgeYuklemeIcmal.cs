using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class BildirgeYuklemeIcmal
    {
        public KeyValuePair<string, string> yilay = new KeyValuePair<string, string>();

        public decimal Matrah;

        public decimal Tutar;

        public int PrimOdenenGunSayisi;

        public string Kanun;

        public Dictionary<string, List<string>> Kisiler = new Dictionary<string, List<string>>();

        public bool IptaliBulunamayanVar { get; set; }
        public bool IptalVarsayimIleBulundu { get; set; }
    }
}
