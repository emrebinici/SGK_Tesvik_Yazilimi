using System;
using System.Collections.Generic;
using System.Linq;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static List<string> BagliKanunlariGetir(string Kanun)
        {
            var bagliKanunlar = new List<string>();

            if (TesvikHesaplamaSabitleri.BagliKanunlarIcmalHesaplama.ContainsKey(Kanun))
            {
                var items = TesvikHesaplamaSabitleri.BagliKanunlarIcmalHesaplama[Kanun];

                foreach (var bagliKanun in items)
                {
                    if (!bagliKanunlar.Contains(bagliKanun))
                    {
                        bagliKanunlar.Add(bagliKanun);
                    }

                    if (TesvikHesaplamaSabitleri.BagliKanunlarIcmalHesaplama.ContainsKey(bagliKanun))
                    {

                        var altitems = BagliKanunlariGetir(bagliKanun);

                        foreach (var altitem in altitems)
                        {
                            if (!bagliKanunlar.Contains(altitem))
                            {
                                bagliKanunlar.Add(altitem);
                            }
                        }

                    }
                }
            }

            return bagliKanunlar.Distinct().ToList();
        }



    }



}
