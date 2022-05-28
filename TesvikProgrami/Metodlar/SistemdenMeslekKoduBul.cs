using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static string SistemdenMeslekKoduBul(Isyerleri isyeri, string TcKimlik, DateTime girisTarihi, ref ProjeGiris projeGiris)
        {
            var yeniWebClientOlustur = projeGiris == null;

            if (yeniWebClientOlustur)
            {
                projeGiris = new ProjeGiris(isyeri, Enums.ProjeTurleri.SigortaliIstenAyrilis);
            }

            string result = string.Empty;

            if (projeGiris.GirisYapilamiyor)
            {
                result = "Sisteme giriş yapamadı";
                return result;
            }

            var denemeSayisi = 0;

        IseGirisleriListele:

            var yanit = projeGiris.PostData("https://uyg.sgk.gov.tr/SigortaliTescil/amp/sigortaliTescilAction", "jobid=sorgula&tkrVno=&kimlikno=" + TcKimlik);

            if (projeGiris.GirisYapilamiyor)
            {
                result = "Sisteme giriş yapamadı";
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

                        IseGirisKaydi giris = null;

                        foreach (var tr in trs)
                        {
                            IseGirisKaydi igk = new IseGirisKaydi();

                            igk.TcKimlikNo = TcKimlik;

                            var iseGirisTarihi = Convert.ToDateTime(tr.Descendants("td").ElementAt(4).InnerText.Trim());

                            if (iseGirisTarihi.Equals(girisTarihi))
                            {
                                igk.doViewNumber = Regex.Match(tr.OuterHtml, ".*do_view\\((\\d+)\\)").Groups[1].Value;

                                giris = igk;

                                break;
                            }
                        }


                        if (giris != null)
                        {
                        MeslekKoduBul:


                            yanit = projeGiris.PostData("https://uyg.sgk.gov.tr/SigortaliTescil/amp/sigortaliTescilAction", "jobid=reshowisegiris&tkrVno=" + giris.doViewNumber + "&kimlikno=" + giris.TcKimlikNo);

                            if (yanit.Contains("Meslek Adı ve Kodu"))
                            {
                                htmlyanit.LoadHtml(yanit);

                                result = htmlyanit.DocumentNode.Descendants("td").FirstOrDefault(td => td.InnerText != null && td.InnerText.Trim().Equals("Meslek Adı ve Kodu")).NextSibling.NextSibling.InnerText.ToString().Trim();
                            }
                            else
                            {
                                denemeSayisi++;

                                if (denemeSayisi < 3)
                                {

                                    if (yanit.Contains("Sigortali bilgisi bulunamadı"))
                                    {
                                        result = "Bulunamadı";
                                    }
                                    else
                                    {

                                        if (yanit.Contains("Sistemden güvenli çıkış yapıldı"))
                                        {
                                            projeGiris = new ProjeGiris(isyeri, Enums.ProjeTurleri.SigortaliIstenAyrilis);

                                            goto IseGirisleriListele;
                                        }

                                        Thread.Sleep(1000);

                                        goto MeslekKoduBul;
                                    }
                                }
                                else result = "Bulunamadı";
                            }
                        }
                        else result = "Bulunamadı";
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
                else result = "Bulunamadı";

            }

            if (yeniWebClientOlustur)
                projeGiris.Disconnect();

            return result.Equals("-") ? "Bulunamadı" : result;

        }



    }



}
