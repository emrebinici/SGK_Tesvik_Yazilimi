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
        public static Sistem7103ListesiResponse Sistemden7103ListesiniCek(Isyerleri isyeri, ProjeGiris projeGiris = null)
        {
            var result = new Sistem7103ListesiResponse();

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
                result.HataMesaji = "10 denemeye rağmen 7103 listesi çekilemedi";
            }

            var kisiler7103 = result.Result;

            var wc7166 = projeGiris;

            string yanit = wc7166.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444719Liste.action;", string.Empty);

            var html7166 = new HtmlAgilityPack.HtmlDocument();

            html7166.LoadHtml(yanit);

            var pencereLinkIdYeni = html7166.GetElementbyId("pencereLinkIdYeni");

            if (pencereLinkIdYeni != null)
            {
                if (pencereLinkIdYeni.OuterHtml.Contains("src=\"https://uyg.sgk.gov.tr/YeniSistem/Isveren/tesvik4447_19_sigortali.action;"))
                {
                    string newUrl = pencereLinkIdYeni.GetAttributeValue("src", "");

                    yanit = wc7166.Get(newUrl, string.Empty);

                    html7166.LoadHtml(yanit);

                    if (yanit != null && (yanit.Contains("<center>4447 GEÇİCİ 19. MADDE KONTROL İŞLEMLERİ</center>") || yanit.Contains("<center>4447/ GEÇİCİ 19. MADDE TANIMLI SİGORTALILAR LİSTESİ</center>")))
                    {

                        var toplamKisiSayisi = Convert.ToInt32(Regex.Match(yanit, "var toplamKayitSay = parseInt\\('(.*)'\\)").Groups[1].Value);

                        List<BasvuruKisiDownload7103> kisiler = new List<BasvuruKisiDownload7103>();
                        List<KeyValuePair<BasvuruKisiDownload7103, string>> silinecekKayitlar = new List<KeyValuePair<BasvuruKisiDownload7103, string>>();
                        List<KeyValuePair<BasvuruKisiDownload7103, string>> bulunanKisiler = new List<KeyValuePair<BasvuruKisiDownload7103, string>>();

                        var satirlar = html7166.DocumentNode.SelectNodes("//table[@class='paginated gradienttable']/tbody/tr");
                        if (satirlar != null)
                        {
                            bool YeniSablon = satirlar.First().ParentNode.ParentNode.InnerText.Contains("Ücret Desteği Tercihi");

                            if (kisiler7103.Count == 0 || (kisiler7103.Count > 0 && !satirlar.First().SelectSingleNode("td[1]/p/text()").GetInnerText().Equals("1")))
                            {
                                foreach (var satir in satirlar)
                                {
                                    kisiler7103.Add(new BasvuruKisiDownload7103
                                    {
                                        TcKimlikNo = satir.SelectSingleNode("td[2]/p/text()").GetInnerText().Trim(),
                                        Sicil = satir.SelectSingleNode("td[3]/p/text()").GetInnerText().Trim(),
                                        Ad = satir.SelectSingleNode("td[4]/p/text()").GetInnerText().Trim(),
                                        Soyad = satir.SelectSingleNode("td[5]/p/text()").GetInnerText().Trim(),
                                        TesvikSuresiBaslangic = satir.SelectSingleNode("td[6]/p/text()").GetInnerText().Trim(),
                                        TesvikSuresiBitis = satir.SelectSingleNode("td[7]/p/text()").GetInnerText().Trim(),
                                        Baz = Convert.ToInt32(Regex.Replace(satir.SelectSingleNode("td[8]/p/text()").GetInnerText().Trim(), "[^0-9]", "")),
                                        UcretDestegiTercihi = YeniSablon ? satir.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim() : string.Empty,
                                        PrimveUcretDestegiIcinBaslangicDonemi = YeniSablon ? satir.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim() : string.Empty,
                                        PrimveUcretDestegiIcinBitisDonemi = YeniSablon ? satir.SelectSingleNode("td[11]/p/text()").GetInnerText().Trim() : string.Empty,
                                        PrimveUcretDestegiIcinIlaveOlunacakSayi = YeniSablon ? satir.SelectSingleNode("td[12]/p/text()").GetInnerText().Trim() : string.Empty,
                                        KanunNo = YeniSablon ? satir.SelectSingleNode("td[13]/p/text()").GetInnerText().Trim() : satir.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim(),
                                        GirisTarihi = YeniSablon ? satir.SelectSingleNode("td[14]/p/text()").GetInnerText().Trim() : satir.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim(),
                                        CikisTarihi = YeniSablon ? satir.SelectSingleNode("td[15]/p/text()").GetInnerText().Trim() : satir.SelectSingleNode("td[11]/p/text()").GetInnerText().Trim(),
                                        IlkTanimlamaTarihi = YeniSablon ? satir.SelectSingleNode("td[16]/p/text()").GetInnerText().Trim() : satir.SelectSingleNode("td[12]/p/text()").GetInnerText()
                                    });
                                }

                                //kişi listesinin tümünü indirmek için tüm sayfalar gezilir.
                                //while (kisiler7103.Count > 0 && kisiler7103.Count % 100 == 0)
                                while (kisiler7103.Count < toplamKisiSayisi)
                                {
                                    string yanitsonraki = wc7166.PostData("https://uyg.sgk.gov.tr/YeniSistem/ListelemManager/sonrakiSayfalarAction.action", "ilkKayitIleriGeriSonKayit=3&herSayfadakiSatirSay=20");

                                    if (yanitsonraki != null && (yanitsonraki.Contains("<center>4447 GEÇİCİ 19. MADDE KONTROL İŞLEMLERİ</center>") || yanitsonraki.Contains("<center>4447/ GEÇİCİ 19. MADDE TANIMLI SİGORTALILAR LİSTESİ</center>")))
                                    {
                                        var htmldevam = new HtmlAgilityPack.HtmlDocument();
                                        htmldevam.LoadHtml(yanitsonraki);

                                        var satirlardevamsayfasi = htmldevam.DocumentNode.SelectNodes("//table[@class='paginated gradienttable']/tbody/tr");
                                        if (satirlardevamsayfasi != null)
                                        {
                                            if (satirlardevamsayfasi.First().SelectSingleNode("td[1]/p/text()").GetInnerText().Equals((kisiler7103.Count + 1).ToString()))
                                            {
                                                foreach (var satirdevam in satirlardevamsayfasi)
                                                {
                                                    kisiler7103.Add(new BasvuruKisiDownload7103
                                                    {
                                                        TcKimlikNo = satirdevam.SelectSingleNode("td[2]/p/text()").GetInnerText().Trim(),
                                                        Sicil = satirdevam.SelectSingleNode("td[3]/p/text()").GetInnerText().Trim(),
                                                        Ad = satirdevam.SelectSingleNode("td[4]/p/text()").GetInnerText().Trim(),
                                                        Soyad = satirdevam.SelectSingleNode("td[5]/p/text()").GetInnerText().Trim(),
                                                        TesvikSuresiBaslangic = satirdevam.SelectSingleNode("td[6]/p/text()").GetInnerText().Trim(),
                                                        TesvikSuresiBitis = satirdevam.SelectSingleNode("td[7]/p/text()").GetInnerText().Trim(),
                                                        Baz = Convert.ToInt32(Regex.Replace(satirdevam.SelectSingleNode("td[8]/p/text()").GetInnerText().Trim(), "[^0-9]", "")),
                                                        UcretDestegiTercihi = YeniSablon ? satirdevam.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim() : string.Empty,
                                                        PrimveUcretDestegiIcinBaslangicDonemi = YeniSablon ? satirdevam.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim() : string.Empty,
                                                        PrimveUcretDestegiIcinBitisDonemi = YeniSablon ? satirdevam.SelectSingleNode("td[11]/p/text()").GetInnerText().Trim() : string.Empty,
                                                        PrimveUcretDestegiIcinIlaveOlunacakSayi = YeniSablon ? satirdevam.SelectSingleNode("td[12]/p/text()").GetInnerText().Trim() : string.Empty,
                                                        KanunNo = YeniSablon ? satirdevam.SelectSingleNode("td[13]/p/text()").GetInnerText().Trim() : satirdevam.SelectSingleNode("td[9]/p/text()").GetInnerText().Trim(),
                                                        GirisTarihi = YeniSablon ? satirdevam.SelectSingleNode("td[14]/p/text()").GetInnerText().Trim() : satirdevam.SelectSingleNode("td[10]/p/text()").GetInnerText().Trim(),
                                                        CikisTarihi = YeniSablon ? satirdevam.SelectSingleNode("td[15]/p/text()").GetInnerText().Trim() : satirdevam.SelectSingleNode("td[11]/p/text()").GetInnerText().Trim(),
                                                        IlkTanimlamaTarihi = YeniSablon ? satirdevam.SelectSingleNode("td[16]/p/text()").GetInnerText().Trim() : satirdevam.SelectSingleNode("td[12]/p/text()").GetInnerText()
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
