using System;
using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class Sistem7252ListesiResponse
    {
        public bool Durum { get; set; } = true;
        public string HataMesaji { get; set; }
        public List<Classes.BasvuruKisiDownload7252> Result { get; set; } = new List<BasvuruKisiDownload7252>();
    }

}
