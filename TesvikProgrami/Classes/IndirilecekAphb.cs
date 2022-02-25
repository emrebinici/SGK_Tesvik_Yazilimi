using System;
using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class IndirilecekAphb
    {
        public string Araci { get; set; }
        public string isyeriSicilNo { get; set; }
        public Bildirge onaysizBildirge { get; set; }
        public HtmlAgilityPack.HtmlNode onayliBildirgeRow { get; set; }
        public string HizmetYilAy { get; set; }
        public int sayOnayliSatir { get; set; }

    }
}
