using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static Sistem7252ListesiResponse Sistemden7252ListesiniCek(Isyerleri isyeri, ProjeGiris projeGiris = null)
        {
            var result = new Sistem7252ListesiResponse();

            if (projeGiris == null) projeGiris = new ProjeGiris(isyeri,Enums.ProjeTurleri.IsverenSistemi);

            if (! projeGiris.Connected)
            {
                for (int i = 0; i < 10; i++)
                {
                    var sonuc= projeGiris.Connect();

                    if (sonuc.Equals("OK") || projeGiris.GirisYapilamiyor) break;
                    else
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            }

            if (!projeGiris.Connected)
            {
                if (!string.IsNullOrEmpty(projeGiris.GirisYapilamamaNedeni)) result.HataMesaji = projeGiris.GirisYapilamamaNedeni;
                else result.HataMesaji = "10 denemeye rağmen sisteme giriş yapılamadı";

                return result;
            }

            var sayac = 0;

        BasaDon:

            if (sayac > 0) System.Threading.Thread.Sleep(1000);

            sayac++;

            if (sayac >= 10)
            {
                result.HataMesaji = "10 denemeye rağmen 7252 listesi çekilemedi";
            }

            var kisiler7252 = result.Result;

            var wc7252 = projeGiris;

            string yanit = wc7252.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444726Liste.action;", string.Empty);

            var html7252 = new HtmlAgilityPack.HtmlDocument();

            html7252.LoadHtml(yanit);

            var pencereLinkIdYeni = html7252.GetElementbyId("pencereLinkIdYeni");

            if (pencereLinkIdYeni != null)
            {
                if (pencereLinkIdYeni.OuterHtml.Contains("src=\"https://uyg.sgk.gov.tr/YeniSistem/Isveren/tesvik4447_26_sigortali.action;"))
                {
                    string newUrl = pencereLinkIdYeni.GetAttributeValue("src", "");

                    yanit = wc7252.Get(newUrl, string.Empty);

                    html7252.LoadHtml(yanit);

                    if (yanit != null && (yanit.Contains("<center>4447 GEÇİCİ 26. MADDE KONTROL İŞLEMLERİ</center>") || yanit.Contains("<center>4447/ GEÇİCİ 26. MADDE TANIMLI SİGORTALILAR LİSTESİ</center>")))
                    {

                        var toplamKisiSayisi = Convert.ToInt32(Regex.Match(yanit, "var toplamKayitSay = parseInt\\('(.*)'\\)").Groups[1].Value);

                        List<BasvuruKisiDownload7252> kisiler = new List<BasvuruKisiDownload7252>();

                        var satirlar = html7252.DocumentNode.SelectNodes("//table[@class='paginated gradienttable']/tbody/tr");
                        if (satirlar != null)
                        {

                            if (kisiler7252.Count == 0 || (kisiler7252.Count > 0 && !satirlar.First().SelectSingleNode("td[1]/p/text()").GetInnerText().Equals("1")))
                            {
                                foreach (var satir in satirlar)
                                {
                                    kisiler7252.Add(new BasvuruKisiDownload7252
                                    {
                                        TcKimlikNo = satir.SelectSingleNode("td[2]/p/text()").GetInnerText().Trim(),
                                        Sicil = satir.SelectSingleNode("td[3]/p/text()").GetInnerText().Trim(),
                                        Ad = satir.SelectSingleNode("td[4]/p/text()").GetInnerText().Trim(),
                                        Soyad = satir.SelectSingleNode("td[5]/p/text()").GetInnerText().Trim(),
                                        TesvikSuresiBaslangic = satir.SelectSingleNode("td[6]/p/text()").GetInnerText().Trim(),
                                        TesvikSuresiBitis = satir.SelectSingleNode("td[7]/p/text()").GetInnerText().Trim(),
                                        KCONUDSonlanmaTarihi = satir.SelectSingleNode("td[8]/p/text()").GetInnerText().Trim(),
                                        OrtalamaGunSayisi = satir.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim(),
                                        KanunNumarası = satir.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim(),
                                        IseGirisTarihi = satir.SelectSingleNode("td[11]/p/text()").GetInnerText().Trim(),
                                        IstenAyrilisTarihi = satir.SelectSingleNode("td[12]/p/text()").GetInnerText().Trim(),
                                        IlkTanimlamaTarihi = satir.SelectSingleNode("td[13]/p/text()").GetInnerText().Trim()
                                    });
                                }

                                //kişi listesinin tümünü indirmek için tüm sayfalar gezilir.
                                while (kisiler7252.Count < toplamKisiSayisi)
                                {
                                    string yanitsonraki = wc7252.PostData("https://uyg.sgk.gov.tr/YeniSistem/ListelemManager/sonrakiSayfalarAction.action", "ilkKayitIleriGeriSonKayit=3&herSayfadakiSatirSay=20");

                                    if (yanitsonraki != null && (yanitsonraki.Contains("<center>4447 GEÇİCİ 26. MADDE KONTROL İŞLEMLERİ</center>") || yanitsonraki.Contains("<center>4447/ GEÇİCİ 26. MADDE TANIMLI SİGORTALILAR LİSTESİ</center>")))
                                    {
                                        var htmldevam = new HtmlAgilityPack.HtmlDocument();
                                        htmldevam.LoadHtml(yanitsonraki);

                                        var satirlardevamsayfasi = htmldevam.DocumentNode.SelectNodes("//table[@class='paginated gradienttable']/tbody/tr");
                                        if (satirlardevamsayfasi != null)
                                        {
                                            if (satirlardevamsayfasi.First().SelectSingleNode("td[1]/p/text()").GetInnerText().Equals((kisiler7252.Count + 1).ToString()))
                                            {
                                                foreach (var satirdevam in satirlardevamsayfasi)
                                                {
                                                    kisiler7252.Add(new BasvuruKisiDownload7252
                                                    {
                                                        TcKimlikNo = satirdevam.SelectSingleNode("td[2]/p/text()").GetInnerText().Trim(),
                                                        Sicil = satirdevam.SelectSingleNode("td[3]/p/text()").GetInnerText().Trim(),
                                                        Ad = satirdevam.SelectSingleNode("td[4]/p/text()").GetInnerText().Trim(),
                                                        Soyad = satirdevam.SelectSingleNode("td[5]/p/text()").GetInnerText().Trim(),
                                                        TesvikSuresiBaslangic = satirdevam.SelectSingleNode("td[6]/p/text()").GetInnerText().Trim(),
                                                        TesvikSuresiBitis = satirdevam.SelectSingleNode("td[7]/p/text()").GetInnerText().Trim(),
                                                        KCONUDSonlanmaTarihi = satirdevam.SelectSingleNode("td[8]/p/text()").GetInnerText().Trim(),
                                                        OrtalamaGunSayisi = satirdevam.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim(),
                                                        KanunNumarası = satirdevam.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim(),
                                                        IseGirisTarihi = satirdevam.SelectSingleNode("td[11]/p/text()").GetInnerText().Trim(),
                                                        IstenAyrilisTarihi = satirdevam.SelectSingleNode("td[12]/p/text()").GetInnerText().Trim(),
                                                        IlkTanimlamaTarihi = satirdevam.SelectSingleNode("td[13]/p/text()").GetInnerText().Trim()
                                                    });
                                                }
                                            }
                                            else break;
                                        }
                                    }
                                    else goto BasaDon;
                                }
                            }
                        }

                        result.Durum = true;

                    }
                    else goto BasaDon;
                }
                else goto BasaDon;
            }
            else goto BasaDon;

            return result;
        }

    }



}
