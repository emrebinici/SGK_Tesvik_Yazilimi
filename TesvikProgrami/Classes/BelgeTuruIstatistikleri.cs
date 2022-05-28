using System.Collections.Generic;
using System.Linq;

namespace TesvikProgrami.Classes
{
    public class BelgeTuruIstatistikleri
    {

        public Dictionary<string, Dictionary<string, KanunIstatistik>> KanunGunveUcretleri = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new Dictionary<string, KanunIstatistik>());

        public Dictionary<string, TesvikKanunuIstatistik> TesvikKanunuIstatistikleri = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new TesvikKanunuIstatistik());
 
    }

}
