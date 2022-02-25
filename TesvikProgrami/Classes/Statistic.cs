using System;
using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class Statistic
    {
        public Dictionary<string,decimal> TesvikTutarlari { get; set; }
        public decimal ToplamUcret { get; set; }
        public decimal TesvikVerilecekToplamGun { get; set; }
        public decimal ToplamGun { get; set; }
    }
}
