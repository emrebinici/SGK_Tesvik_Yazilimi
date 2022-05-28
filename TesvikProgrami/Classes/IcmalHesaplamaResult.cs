using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class IcmalHesaplamaResult
    {
        public Dictionary<DonusturulecekKanun, Dictionary<string, TesvikTutariIstatistik>> icmaller = new Dictionary<DonusturulecekKanun, Dictionary<string, TesvikTutariIstatistik>>();

        public Dictionary<DonusturulecekKanun, Dictionary<string, TesvikTutariIstatistik>> tumIcmaller = new Dictionary<DonusturulecekKanun, Dictionary<string, TesvikTutariIstatistik>>();

        public bool ToplamIcmalEkside;

        public bool KanunlardanBiriBaskaTesvikAlmayiEngelliyor = false;

    }

}
