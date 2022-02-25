using System;
using System.Collections.Generic;
using System.Linq;

namespace TesvikProgrami.Classes
{
    public class BazBilgisi
    {
        public BazBilgisi(DateTime donem/*,int baz*/)
        {
            OncedenTesvikAlanlar = new List<string>();

            OncedenTesvikAlanlarTaseronsuz = new List<string>();

            Kisiler = new List<KeyValuePair<Classes.Kisi, Statistic>>();

            KisiBelgeTurleri = new Dictionary<string, List<string>>();

            Donem = donem;

        }
        public DateTime Donem { get; set; }
        public List<KeyValuePair<Classes.Kisi, Statistic>> Kisiler { get; set; }
        public Dictionary<string, List<string>> KisiBelgeTurleri { get; set; }
        public List<string> OncedenTesvikAlanlar { get; set; }
        public List<string> OncedenTesvikAlanlarTaseronsuz { get; set; }
        public long AydaCalisanSayisi { get; set; }


        public long TesvikAlabilecekKisiSayisi { get { return AydaCalisanSayisi /*- Baz*/ - OncedenTesvikAlanlar.Count; } }

        public void KisiEkle(Classes.Kisi kisi, Dictionary<string,decimal> miktarlar, TesvikKanunuIstatistik tesvikKanunuIstatistik, string belgeturu)
        {
            Classes.Kisi eklenecekkisi = null;

            KeyValuePair<Classes.Kisi, Statistic> kisistatistik = new KeyValuePair<Classes.Kisi, Statistic>();

            foreach (var item in Kisiler)
            {
                if (item.Key.TckimlikNo == kisi.TckimlikNo)
                {
                    eklenecekkisi = item.Key;

                    kisistatistik = item;

                    break;

                }
            }


            if (eklenecekkisi == null)
            {
                Kisiler.Add(new KeyValuePair<Classes.Kisi, Statistic>(kisi, new Statistic { TesvikTutarlari = miktarlar, ToplamUcret = tesvikKanunuIstatistik.ToplamUcret, TesvikVerilecekToplamGun = tesvikKanunuIstatistik.TesvikVerilecekGun, ToplamGun = tesvikKanunuIstatistik.ToplamGun}));
            }
            else
            {
                //kisistatistik.Value.TesvikTutari += miktar;
                //kisistatistik.Value.ToplamGun += toplamGun;
                //kisistatistik.Value.ToplamUcret += toplamUcret;
                if (!KisiBelgeTurleri.ContainsKey(kisi.TckimlikNo) || !KisiBelgeTurleri[kisi.TckimlikNo].Contains(belgeturu))
                {
                    Kisiler.Remove(kisistatistik);

                    var yeniTesvikTutarlari = kisistatistik.Value.TesvikTutarlari.ToDictionary(x => x.Key, x => x.Value);

                    foreach (var kv in miktarlar)
                    {
                        yeniTesvikTutarlari[kv.Key] += kv.Value;
                    }


                    Kisiler.Add(new KeyValuePair<Classes.Kisi, Statistic>(kisi, new Statistic { 
                        TesvikTutarlari = yeniTesvikTutarlari, 
                        ToplamUcret = tesvikKanunuIstatistik.ToplamUcret + kisistatistik.Value.ToplamUcret, 
                        TesvikVerilecekToplamGun = tesvikKanunuIstatistik.TesvikVerilecekGun + kisistatistik.Value.TesvikVerilecekToplamGun,
                        ToplamGun = tesvikKanunuIstatistik.ToplamGun + kisistatistik.Value.ToplamGun
                    }));
                }
            }

            //Kisiler.Sort((first, next) => { return first.Value.TesvikTutari.CompareTo(next.Value.TesvikTutari) == 0 ? (next.Value.ToplamUcret.CompareTo(first.Value.ToplamUcret)) : first.Value.TesvikTutari.CompareTo(next.Value.TesvikTutari) == 1 ? -1 : 1; });

            if (belgeturu != null)
            {
                if (KisiBelgeTurleri.ContainsKey(kisi.TckimlikNo))
                {
                    if (!KisiBelgeTurleri[kisi.TckimlikNo].Contains(belgeturu)) KisiBelgeTurleri[kisi.TckimlikNo].Add(belgeturu);
                }
                else KisiBelgeTurleri.Add(kisi.TckimlikNo, new List<string> { belgeturu });
            }
        }

        public void KisiEkle(string tckimlikno, Dictionary<string, decimal> miktarlar, decimal toplamUcret, decimal toplamGun, string belgeturu)
        {
            Classes.Kisi eklenecekkisi = null;

            KeyValuePair<Classes.Kisi, Statistic> kisistatistik = new KeyValuePair<Classes.Kisi, Statistic>();

            foreach (var item in Kisiler)
            {
                if (item.Key.TckimlikNo == tckimlikno)
                {
                    eklenecekkisi = item.Key;

                    kisistatistik = item;

                    break;

                }
            }

            if (eklenecekkisi == null)
            {
                eklenecekkisi = new Classes.Kisi();

                eklenecekkisi.TckimlikNo = tckimlikno;

                Kisiler.Add(new KeyValuePair<Classes.Kisi, Statistic>(eklenecekkisi, new Statistic { TesvikTutarlari = miktarlar, TesvikVerilecekToplamGun = toplamGun, ToplamUcret = toplamUcret }));
            }
            else
            {

                //kisistatistik.Value.TesvikTutari += miktar;
                //kisistatistik.Value.ToplamGun += toplamGun;
                //kisistatistik.Value.ToplamUcret += toplamUcret;

                if (!KisiBelgeTurleri.ContainsKey(tckimlikno) || !KisiBelgeTurleri[tckimlikno].Contains(belgeturu))
                {
                    Kisiler.Remove(kisistatistik);

                    var yeniTesvikTutarlari = kisistatistik.Value.TesvikTutarlari.ToDictionary(x => x.Key, x => x.Value);

                    foreach (var kv in miktarlar)
                    {
                        yeniTesvikTutarlari[kv.Key] += kv.Value;
                    }

                    Kisiler.Add(new KeyValuePair<Classes.Kisi, Statistic>(kisistatistik.Key, new Statistic { 
                        TesvikTutarlari = yeniTesvikTutarlari, 
                        ToplamUcret = toplamUcret + kisistatistik.Value.ToplamUcret, 
                        TesvikVerilecekToplamGun = toplamGun + kisistatistik.Value.TesvikVerilecekToplamGun
                    }));
                }
            }

            //Kisiler.Sort((first, next) => { return first.Value.TesvikTutari.CompareTo(next.Value.TesvikTutari) == 0 ? (next.Value.ToplamUcret.CompareTo(first.Value.ToplamUcret)) : first.Value.TesvikTutari.CompareTo(next.Value.TesvikTutari) == 1 ? -1 : 1; });

            if (belgeturu != null)
            {
                if (KisiBelgeTurleri.ContainsKey(tckimlikno))
                {
                    if (!KisiBelgeTurleri[tckimlikno].Contains(belgeturu)) KisiBelgeTurleri[tckimlikno].Add(belgeturu);
                }
                else KisiBelgeTurleri.Add(tckimlikno, new List<string> { belgeturu });
            }
        }

        public void OncedenTesvikAlanEkle(string tckimlikno, bool Taseron)
        {
            if (!OncedenTesvikAlanlar.Contains(tckimlikno)) OncedenTesvikAlanlar.Add(tckimlikno);

            if (!Taseron)
            {
                if (!OncedenTesvikAlanlarTaseronsuz.Contains(tckimlikno)) OncedenTesvikAlanlarTaseronsuz.Add(tckimlikno);
            }
        }
    }

}
