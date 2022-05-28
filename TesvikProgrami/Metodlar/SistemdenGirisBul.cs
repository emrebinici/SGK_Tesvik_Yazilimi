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
        public static bool SistemdenGirisBul(Isyerleri isyeri, string TcKimlik, DateTime girisTarihi, ref ProjeGiris projeGiris)
        {
            var yeniWebClientOlustur = projeGiris == null;

            if (yeniWebClientOlustur)
            {
                projeGiris = new ProjeGiris(isyeri, Enums.ProjeTurleri.SigortaliIstenAyrilis);
            }

            var denemeSayisi = 0;

        IseGirisleriListele:

            var yanit = projeGiris.PostData("https://uyg.sgk.gov.tr/SigortaliTescil/amp/sigortaliTescilAction", "jobid=sorgula&tkrVno=&kimlikno=" + TcKimlik);

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

                        return giris != null;
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

            }

            if (yeniWebClientOlustur)
                projeGiris.Disconnect();

            return false;

        }



    }



}
