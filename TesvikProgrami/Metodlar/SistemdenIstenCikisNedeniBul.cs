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
        public static string SistemdenIstenCikisNedeniBul(Isyerleri isyeri, string TcKimlik, DateTime cikisTarihi)
        {
            var sigortaliIstenAyrilisProjesiConnect = new ProjeGiris(isyeri, Enums.ProjeTurleri.SigortaliIstenAyrilis);

            string result = string.Empty;

        IstenCikislariListele:

            var yanit = sigortaliIstenAyrilisProjesiConnect.PostData("https://uyg.sgk.gov.tr/SigortaliTescil/amp/sigortaliTescilAction", "jobid=ayrilissorgulasonuc&tkrVno=&kimlikno=" + TcKimlik);

            if (yanit.Contains("Sigortalı  İşten  Ayrılış Kayıtları"))
            {
                if (!yanit.Contains("İsten Ayrilis Kayidi Bulunmamaktadir"))
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
                                IstenCikisKaydi ick = new IstenCikisKaydi();
                                ick.TcKimlikNo = TcKimlik;

                                var istenayrilistarihi = Convert.ToDateTime(tr.Descendants("td").ElementAt(1).InnerText.Trim());
                                ick.istenCikisTarihi = istenayrilistarihi;

                                ick.doViewNumber = Regex.Match(tr.OuterHtml, ".*do_view\\((\\d+)\\)").Groups[1].Value;

                                cikislar.Add(ick);
                            }


                            var ilkcikis = cikislar.OrderBy(p => p.istenCikisTarihi).FirstOrDefault(p => p.istenCikisTarihi.Equals(cikisTarihi));

                            if (ilkcikis != null)
                            {
                            istenCikisNedeniBul:

                                yanit = sigortaliIstenAyrilisProjesiConnect.PostData("https://uyg.sgk.gov.tr/SigortaliTescil/amp/sigortaliTescilAction", "jobid=reshow&tkrVno=" + ilkcikis.doViewNumber + "&kimlikno=");

                                if (yanit.Contains("Sigortalının İşten Ayrılış Nedeni (Kodu)"))
                                {
                                    htmlyanit.LoadHtml(yanit);

                                    var istencikisnedeni = Convert.ToInt32(htmlyanit.DocumentNode.Descendants("td").FirstOrDefault(td => td.InnerText != null && td.InnerText.Trim().Equals("Sigortalının İşten Ayrılış Nedeni (Kodu)")).NextSibling.InnerText).ToString();

                                    result = istencikisnedeni;
                                }
                                else
                                {
                                    Thread.Sleep(1000);
                                    goto istenCikisNedeniBul;
                                }
                            }
                        }
                    }
                }
                else result = "Bulunamadı";
            }
            else
            {
                Thread.Sleep(1000);
                goto IstenCikislariListele;
            }

            sigortaliIstenAyrilisProjesiConnect.Bitti = true;

            return result;

        }



    }



}
