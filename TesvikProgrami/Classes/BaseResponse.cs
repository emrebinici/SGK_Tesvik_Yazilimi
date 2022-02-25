using System;
using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class BaseResponse
    {
        public bool Durum { get; set; } = true;
        public string HataMesaji { get; set; }
        public string Result { get; set; }
    }

}
