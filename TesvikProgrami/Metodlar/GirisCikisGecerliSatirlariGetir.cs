using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static List<IseGirisCikisKaydi> GirisCikisGecerliSatirlariGetir(List<IseGirisCikisKaydi> satirlar)
        {
            List<int> eklenenenler = new List<int>();

            for (int i = 0; i < satirlar.Count; i++)
            {
                if (satirlar[i].IslemTuru.Equals("Silme"))
                {
                    var kayitSilindi = false;

                    for (int j = 0; j < satirlar.Count; j++)
                    {
                        if (i == j) continue;

                        if (eklenenenler.Contains(j)) continue;

                        if (satirlar[i].Tarih.Equals(satirlar[j].Tarih)
                            && satirlar[i].Turu.Equals(satirlar[j].Turu)
                            && satirlar[i].TcKimlikNo.Equals(satirlar[j].TcKimlikNo)
                            && satirlar[i].Araci.Equals(satirlar[j].Araci)
                            && (satirlar[j].IslemTuru.Equals("Kayıt") || satirlar[j].IslemTuru.Equals("Güncelleme"))
                        )
                        {
                            if (satirlar[j].IslemTuru.Equals("Kayıt"))
                            {
                                if (kayitSilindi == false)
                                {
                                    eklenenenler.Add(j);

                                    kayitSilindi = true;
                                }
                            }
                            else eklenenenler.Add(j);
                        }
                    }

                    eklenenenler.Add(i);
                }
            }

            var sonuc= satirlar.Where((p, ind) => !eklenenenler.Contains(ind)).OrderByDescending(p=> p.Tarih).ThenByDescending(p=> p.IslemSaati).ToList();

            sonuc= sonuc.GroupBy(p => p.Tarih.ToShortDateString() + "-" + p.TcKimlikNo + "-" + p.Turu + "-" + p.Araci).Where(p=> p.Count() > 0).Select(p => p.FirstOrDefault()).ToList();

            return sonuc;
        }


    }



}
