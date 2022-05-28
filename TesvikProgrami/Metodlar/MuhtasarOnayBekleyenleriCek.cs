using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static MuhtasarOnaydaBekleyenlerResponse MuhtasarOnayBekleyenleriCek(Isyerleri isyeri, ProjeGiris projeGiris)
        {

            var yeniWebClientOlustur = projeGiris == null;

            if (yeniWebClientOlustur)
            {
                projeGiris = new ProjeGiris(isyeri, Enums.ProjeTurleri.EBildirgeV2);
            }

            var result = new MuhtasarOnaydaBekleyenlerResponse();

            try
            {
                if (!projeGiris.Connected)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        projeGiris.Connect();

                        if (projeGiris.Connected || projeGiris.GirisYapilamiyor) break;

                        Thread.Sleep(1000);
                    }
                }

                if (!projeGiris.Connected)
                {
                    throw new Exception(string.IsNullOrEmpty(projeGiris.GirisYapilamamaNedeni) ? "10 denemeye rağmen sisteme giriş yapılamadı" : projeGiris.GirisYapilamamaNedeni);
                }

                var hataVerenBildirgeler = new Dictionary<Bildirge, string>();

                var sayac = 0;

            tekrarDene:

                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();

                var yanit = projeGiris.Get("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukmuhtasarOnayBekleyenTahakkuklar.action");

                if (yanit.Contains("Muhtasar Bildirge Bulunamadı")) throw new Exception("Muhtasar Bildirge Bulunamadı");

                if (yanit.Contains("Muhtasar Onay Bekleyen Bildirge Listesi"))
                {
                    sayac = 0;

                    html.LoadHtml(yanit);

                    var table = html.DocumentNode.Descendants("table").FirstOrDefault(p => p.GetAttributeValue("class", "").Equals("gradienttable"));

                    var trs = table.Descendants("tr");

                    List<Classes.Bildirge> bildirgeler = new List<Bildirge>();

                    for (int i = 1; i < trs.Count(); i++)
                    {

                        var tds = trs.ElementAt(i).Descendants("td").ToList();

                        string hizmetYilAy = tds[3].InnerText;
                        int yil = Convert.ToInt32(hizmetYilAy.Split('/')[0]);
                        int ay = Convert.ToInt32(hizmetYilAy.Split('/')[1]);

                        var belgeNo = tds[4].InnerText.ToInt().ToString();
                        var kanun = tds[6].InnerText.Split('-')[0].Trim().PadLeft(5, '0');
                        var mahiyet = tds[5].InnerText;

                        var refNo = tds[0].FirstChild.FirstChild.GetAttributeValue("value", "");

                        bildirgeler.Add(new Bildirge
                        {
                            Yil = yil.ToString(),
                            Ay = ay.ToString(),
                            BelgeTuru = belgeNo,
                            Kanun = kanun,
                            Mahiyet = mahiyet,
                            RefNo = refNo,
                            AraciveyaIsveren = isyeri.TaseronNo.ToInt() == 0 ? "Ana İşveren" : String.Format("{0}-{1}",isyeri.TaseronNo.ToInt().ToString().PadLeft(3,'0'), isyeri.Sirketler.SirketAdi),
                            Askida = true
                        });
                    }

                    Parallel.For(0, bildirgeler.Count, new ParallelOptions { MaxDegreeOfParallelism = 20 }, (index) =>
                    {
                        var wc = new ProjeGiris(isyeri, Enums.ProjeTurleri.EBildirgeV2);

                        wc.Connected = true;
                        wc.Cookie = projeGiris.Cookie;

                        var bildirge = bildirgeler[index];

                        var sonuc = MuhtasarOnayBekleyenKisileriCek(ref bildirge, wc);

                        if (sonuc != "OK")
                        {
                            lock (hataVerenBildirgeler)
                            {
                                hataVerenBildirgeler.Add(bildirge, sonuc);
                            }
                        }
                    });

                    var kaydetmeSonucu = Metodlar.MuhtasarOnaydaBekleyenAphbKaydet(isyeri, bildirgeler );

                    result.Durum = kaydetmeSonucu != null;
                    result.HataMesaji = kaydetmeSonucu;
                    result.Result = kaydetmeSonucu;

                    result.HataliBildirgeler = hataVerenBildirgeler;

                    return result;


                }
                else
                {
                    sayac++;

                    if (sayac < 5)
                    {

                        Thread.Sleep(500);
                        goto tekrarDene;
                    }
                    else throw new Exception("5 denemeye rağmen bilgiler çekilemedi");
                }




            }
            catch (Exception ex)
            {
                result.HataMesaji = ex.Message;
                result.Durum = false;
            }
            finally
            {
                if (yeniWebClientOlustur)
                    projeGiris.Disconnect();
            }

            if (yeniWebClientOlustur) projeGiris.Disconnect();

            return result;

        }

        public static string MuhtasarOnayBekleyenKisileriCek(ref Bildirge bildirge, ProjeGiris webclient)
        {
            var sayac = 0;

        TekrarDene:

            var data = webclient.DownloadFilePost("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tilesislemTamam.action", String.Format("bildirgeRefNo={0}&download=true&action%3AtahakkukmuhtasarFisHizmetPdf=Hizmet+Listesi%28PDF%29", bildirge.RefNo));

            var sonuc = System.Text.Encoding.UTF8.GetString(data);

            if (sonuc.Contains("Hata referans numarası")) return "Bildirge Hatalı";

            if (sonuc.StartsWith("%PDF"))
            {
                var reader = new PdfReader(data);

                var pdfOkumaResult = Metodlar.GetPdfAphbKisiList(reader,"Ana İşveren");

                if (!pdfOkumaResult.pdfBildirgeHataliOkunduMu)
                {
                    bildirge.Kisiler.AddRange(pdfOkumaResult.satirlar);

                    return "OK";
                }
                else return pdfOkumaResult.bilgiDondurmekIcin;
            }
            else
            {
                sayac++;

                if (sayac < 3)
                {
                    Thread.Sleep(1000);
                    goto TekrarDene;
                }
                else return "3 denemeye rağmen bildirge çekilemedi";
            }
        }


    }




}
