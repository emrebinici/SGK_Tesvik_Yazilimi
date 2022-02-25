using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static KisiGirisCikislariResponse SistemdenKisininGirisCikislariniBul(Isyerleri isyeri, string TcKimlik, ref ProjeGiris projeGiris)
        {
            var result = new KisiGirisCikislariResponse();

            var yeniWebClientOlustur = projeGiris == null;

            if (yeniWebClientOlustur)
            {
                projeGiris = new ProjeGiris(isyeri, Enums.ProjeTurleri.SigortaliIstenAyrilis);
            }

            var denemeSayisi = 0;

        IseGirisleriListele:

            var yanit = projeGiris.PostData("https://uyg.sgk.gov.tr/SigortaliTescil/amp/sigortaliTescilAction", "jobid=sorgula&tkrVno=&kimlikno=" + TcKimlik);

            if (projeGiris.GirisYapilamiyor)
            {
                result.Durum = false;
                result.HataMesaji = projeGiris.GirisYapilamamaNedeni;
                return result;
            }

            bool tekrarDene = false;

            if (yanit.Contains("Sigortalı  İşe Giriş Kayıtları"))
            {
                HtmlAgilityPack.HtmlDocument htmlyanit = new HtmlAgilityPack.HtmlDocument();
                htmlyanit.LoadHtml(yanit);

                var form1 = htmlyanit.GetElementbyId("form1");

                if (form1 != null)
                {
                    var table = form1.Descendants("table").FirstOrDefault();

                    if (table != null)
                    {
                        var trs = table.Descendants("tr").Where(p => p.OuterHtml != null && p.OuterHtml.Contains("javascript:do_view"));

                        foreach (var tr in trs)
                        {
                            var iseGirisTarihi = Convert.ToDateTime(tr.Descendants("td").ElementAt(4).InnerText.Trim());

                            result.girisCikislar.Add(new GirisCikisTarihleri
                            {
                                Tarih = iseGirisTarihi,
                                GirisMi = true,
                            });
                        }
                    }
                    else tekrarDene = true;
                }
                else tekrarDene = true;
            }
            else tekrarDene = true;

            if (tekrarDene)
            {
                denemeSayisi++;

                if (denemeSayisi < 3)
                {
                    if (yanit.Contains("Sistemden güvenli çıkış yapıldı"))
                    {
                        projeGiris = new ProjeGiris(isyeri, Enums.ProjeTurleri.SigortaliIstenAyrilis);
                    }

                    Thread.Sleep(1000);
                    goto IseGirisleriListele;
                }
                else
                {
                    result.Durum = false;
                    result.HataMesaji = "3 denemeye rağmen kişinin girişlerine bakılamadı";
                    return result;
                }
            }


            denemeSayisi = 0;

        IstenCikislariListele:

            tekrarDene = false;

            yanit = projeGiris.PostData("https://uyg.sgk.gov.tr/SigortaliTescil/amp/sigortaliTescilAction", "jobid=ayrilissorgulasonuc&tkrVno=&kimlikno=" + TcKimlik);

            if (yanit.Contains("Sigortalı  İşten  Ayrılış Kayıtları"))
            {
                if (! yanit.Contains("İsten Ayrilis Kayidi Bulunmamaktadir"))
                {
                    HtmlAgilityPack.HtmlDocument htmlyanit = new HtmlAgilityPack.HtmlDocument();
                    htmlyanit.LoadHtml(yanit);

                    var form1 = htmlyanit.GetElementbyId("form1");

                    if (form1 != null)
                    {
                        var table = form1.Descendants("table").FirstOrDefault();

                        if (table != null)
                        {
                            var trs = table.Descendants("tr").Where(p => p.OuterHtml != null && p.OuterHtml.Contains("javascript:do_view"));

                            List<IstenCikisKaydi> cikislar = new List<IstenCikisKaydi>();

                            foreach (var tr in trs)
                            {
                                var istenayrilistarihi = Convert.ToDateTime(tr.Descendants("td").ElementAt(1).InnerText.Trim());
                                result.girisCikislar.Add(new GirisCikisTarihleri
                                {
                                    Tarih = istenayrilistarihi,
                                    GirisMi = false
                                });

                            }
                        }
                        else tekrarDene = true;
                    }
                    else tekrarDene = true;
                }
            }
            else tekrarDene = true;


            if (tekrarDene)
            {
                denemeSayisi++;

                if (denemeSayisi < 3)
                {

                    if (yanit.Contains("Sistemden güvenli çıkış yapıldı"))
                    {
                        projeGiris = new ProjeGiris(isyeri, Enums.ProjeTurleri.SigortaliIstenAyrilis);
                    }

                    Thread.Sleep(1000);
                    goto IstenCikislariListele;
                }
                else
                {
                    result.Durum = false;
                    result.HataMesaji = "3 denemeye rağmen kişinin çıkışlarına bakılamadı";
                    return result;
                }
            }


            if (yeniWebClientOlustur)
                projeGiris.Disconnect();

            result.Durum = true;
            return result;

        }



    }



}
