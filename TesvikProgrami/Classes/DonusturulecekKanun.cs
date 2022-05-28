using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class DonusturulecekKanun
    {
        public DonusturulecekKanun()
        {

        }

        public string DonusturulecekKanunNo;

        public decimal DusulecekTutar = 0;

        public bool SadeceCari = false;

        public static Dictionary<string, DusulecekTutarIstastistik> DusulecekMiktarHesapla(string kanunNo, int gun, decimal toplamUcret, int yil, int ay, string belgeTuru, string IsyeriSicilNo, bool IcmaldenDusulsun,Dictionary<string,Tesvik> TumTesvikler = null, decimal CarpimOrani687 = -1)
        {
            if (TumTesvikler == null) TumTesvikler = Program.TumTesvikler;

            Dictionary<string, DusulecekTutarIstastistik> sonuc = new Dictionary<string, DusulecekTutarIstastistik>();

            decimal BagliKanunlarHaricDusulecekTutar = 0;

            BagliKanunlarHaricDusulecekTutar = IcmaldenDusulsun ? Metodlar.TesvikTutariHesapla(kanunNo, gun, toplamUcret, yil, ay, belgeTuru, IsyeriSicilNo,TumTesvikler, CarpimOrani687) : 0;

            decimal BagliKanunlarDahilDusulecekTutar = BagliKanunlarHaricDusulecekTutar;
            var bagliKanunlar = Metodlar.BagliKanunlariGetir(kanunNo);

            if (bagliKanunlar.Count > 0)
            {

                foreach (var bagliKanun in bagliKanunlar)
                {
                    var bagliKanunTutarlari = DusulecekMiktarHesapla(bagliKanun, gun, toplamUcret, yil, ay, belgeTuru, IsyeriSicilNo, IcmaldenDusulsun,TumTesvikler, CarpimOrani687);
            
                    foreach (var item in bagliKanunTutarlari)
                    {
                        if (!sonuc.ContainsKey(item.Key)) sonuc.Add(item.Key, new DusulecekTutarIstastistik());

                        sonuc[item.Key].BagliKanunlarDahilDusulecekTutar = item.Value.BagliKanunlarDahilDusulecekTutar;
                        sonuc[item.Key].BagliKanunlarHaricDusulecekTutar = item.Value.BagliKanunlarHaricDusulecekTutar;

                        BagliKanunlarDahilDusulecekTutar += item.Value.BagliKanunlarDahilDusulecekTutar;
                    }

                }
            }


            if (!sonuc.ContainsKey(kanunNo)) sonuc.Add(kanunNo, new DusulecekTutarIstastistik());

            sonuc[kanunNo].BagliKanunlarDahilDusulecekTutar = BagliKanunlarDahilDusulecekTutar;
            sonuc[kanunNo].BagliKanunlarHaricDusulecekTutar = BagliKanunlarHaricDusulecekTutar;

            return sonuc;

        }
    }

}
