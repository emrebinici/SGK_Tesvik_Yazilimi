using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static string Isyeri6486KanunBul(string sicilNo)
        {
            string plakakodu = sicilNo.Substring(16, 3);

            string Kanun6486 = string.Empty;

            if (TesvikHesaplamaSabitleri.Iller46486.Contains(plakakodu)) Kanun6486 = "46486";
            else if (TesvikHesaplamaSabitleri.Iller56486.Contains(plakakodu)) Kanun6486 = "56486";
            else if (TesvikHesaplamaSabitleri.Iller66486.Contains(plakakodu)) Kanun6486 = "66486";

            return Kanun6486;

        }



    }



}
