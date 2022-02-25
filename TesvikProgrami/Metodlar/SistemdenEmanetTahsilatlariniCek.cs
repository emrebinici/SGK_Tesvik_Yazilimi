using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static string SistemdenEmanetTahsilatlariniCek(Isyerleri isyeri, ref ProjeGiris projeGiris)
        {
            var yeniWebClientOlustur = projeGiris == null;

            if (yeniWebClientOlustur)
            {
                projeGiris = new ProjeGiris(isyeri, Enums.ProjeTurleri.IsverenBorcSorgu);
            }

            string result = string.Empty;

            var denemeSayisi = 0;

        tekrarDene:

            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();

            var yanit = projeGiris.Get("https://uyg.sgk.gov.tr/IsverenBorcSorgu/borc/bankaEmanet.action", string.Empty);

            var bankaIsverenEmanetTahsilatlari = new List<BankaIsverenEmanetTahsilat>();

            var MosipEmanetTahsilatlari = new List<MosipEmanetTahsilat>();

            if (yanit.Contains("BANKA/İŞVEREN EMANET TAHSİLATLARI") || yanit.Contains("MOSİP EMANET TAHSİLATLARI"))
            {
                html.LoadHtml(yanit);

                var tables = html.DocumentNode.Descendants("table");

                DateTime sinir = new DateTime(2011, 3, 1);

                foreach (var table in tables)
                {
                    var firsttd = table.Descendants("td").FirstOrDefault();
                    if (firsttd != null && firsttd.InnerText != null && firsttd.InnerText.Equals("Tahsilat Tarihi"))
                    {
                        if (! table.InnerText.Contains("GÖSTERİLECEK KAYIT YOK"))
                        {
                            var trs = table.Descendants("tr").Skip(1).ToList();

                            trs.ForEach(tr =>
                            {
                                var tds = tr.Descendants("td");

                                var tarih = new DateTime(Convert.ToInt32(tds.ElementAt(1).InnerText.Trim()),Convert.ToInt32(tds.ElementAt(2).InnerText.Trim()),1) ;

                                if (tarih >= sinir)
                                {

                                    bankaIsverenEmanetTahsilatlari.Add(new BankaIsverenEmanetTahsilat
                                    {
                                        TahsilatTarihi = tds.ElementAt(0).InnerText.Trim(),
                                        DonemYil = tds.ElementAt(1).InnerText.Trim(),
                                        DonemAy = tds.ElementAt(2).InnerText.Trim(),
                                        BorcTuru = tds.ElementAt(3).InnerText.Trim(),
                                        TahsilatTutar = tds.ElementAt(4).InnerText.Trim().Replace(".", ""),
                                    });
                                }
                            });
                        }


                    }
                    else if (firsttd != null && firsttd.InnerText != null && firsttd.InnerText.Equals("Bankaya Yatırılma Tarihi"))
                    {
                        if (! table.InnerText.Contains("GÖSTERİLECEK KAYIT YOK"))
                        {
                            var trs = table.Descendants("tr").Skip(1).ToList();

                            trs.ForEach(tr =>
                            {
                                var tds = tr.Descendants("td");

                                var tarih = Convert.ToDateTime(tds.ElementAt(0).InnerText);

                                if (tarih >= sinir)
                                {
                                    if (!tr.GetInnerText().Contains("6661 Asgari Desteği"))
                                    {
                                        MosipEmanetTahsilatlari.Add(new MosipEmanetTahsilat
                                        {
                                            BankayaYatirilmaTarihi = tds.ElementAt(0).InnerText.Trim(),
                                            EmanettekiTahsilatTutari = tds.ElementAt(1).InnerText.Trim().Replace(".", ","),
                                            TahsilatTuru = tds.ElementAt(2).InnerText.Trim()
                                        });
                                    }
                                }

                            });
                        }
                    }
                }

                if (bankaIsverenEmanetTahsilatlari.Count > 0 || MosipEmanetTahsilatlari.Count > 0)
                {
                    if (bankaIsverenEmanetTahsilatlari.Count > 0)
                    {
                        bankaIsverenEmanetTahsilatlari.Add(new BankaIsverenEmanetTahsilat
                        {
                            BorcTuru = "Genel Toplam",
                            TahsilatTutar = bankaIsverenEmanetTahsilatlari.Sum(p => Convert.ToDecimal(p.TahsilatTutar.Replace(".",""))).ToString()
                        });
                    }

                    if (MosipEmanetTahsilatlari.Count > 0)
                    {
                        MosipEmanetTahsilatlari.Add(new MosipEmanetTahsilat
                        {
                            BankayaYatirilmaTarihi = "Genel Toplam",
                            EmanettekiTahsilatTutari = MosipEmanetTahsilatlari.Sum(p => Convert.ToDecimal(p.EmanettekiTahsilatTutari.Replace(".", ","))).ToString()
                        });
                    }

                    result = Metodlar.EmanetTahsilatlariKaydet(isyeri, bankaIsverenEmanetTahsilatlari, MosipEmanetTahsilatlari);
                }
                else
                {
                    try
                    {
                        var path = Path.GetDirectoryName(IsyeriKlasorBul(isyeri));

                        var files = Directory.GetFiles(path, "Emanet*.xlsx");

                        foreach (var file in files)
                        {
                            File.Delete(file);
                        }
                    }
                    catch { }

                    result = "Emanet tahsilat kaydı bulunamadı";
                }

            }
            else
            {
                denemeSayisi++;

                if (denemeSayisi < 10)
                {
                    Thread.Sleep(3000);
                    goto tekrarDene;
                }
                else result = "10 denemeye rağmen bilgiler çekilemedi";
            }





            if (yeniWebClientOlustur)
                projeGiris.Disconnect();

            return result;

        }
    }



}
