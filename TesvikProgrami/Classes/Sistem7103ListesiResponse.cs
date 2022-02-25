using System;
using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class Sistem7103ListesiResponse
    {
        public bool Durum { get; set; } = true;
        public string HataMesaji { get; set; }
        public List<Classes.BasvuruKisiDownload7103> Result { get; set; } = new List<BasvuruKisiDownload7103>();
    }

}
