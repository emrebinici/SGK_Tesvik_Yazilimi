using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static string BasvuruYap(Isyerleri isyeri)
        {
            var projeGiris = new ProjeGiris(isyeri, Enums.ProjeTurleri.IsverenSistemi);

            string result = string.Empty;

            if (isyeri.KullaniciKod.Length > 4)
            {
                return "Kullanıcı kodu 4 karakterden fazla";
            }
            else
            {

                int basvuruKaydiYapilamadiSayaci = 0;

            BasvuruKaydiYapilamadiTekrarDene:

                for (int i = 0; i < 5; i++)
                {
                    projeGiris.Connect();

                    if (projeGiris.Connected || projeGiris.GirisYapilamiyor) break;
                    else Thread.Sleep(3000);

                }

                if (! projeGiris.Connected)
                {
                    if (!string.IsNullOrEmpty(projeGiris.GirisYapilamamaNedeni))
                    {
                        return "Sisteme giriş yapılamadı. Nedeni:" + projeGiris.GirisYapilamamaNedeni;
                    }
                    else return "5 denemeye rağmen sisteme giriş yapılamadı";
                }
                else
                {

                    HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();

                    int sayac = 0;

                YenidenDene:

                    string response = projeGiris.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444717Basvuru.action;", "");

                    if (response.Contains("5510/ EK 17. MADDE BAŞVURU İŞLEMLERİ"))
                    {
                        sayac = 0;

                        html.LoadHtml(response);

                        var viewstate = html.GetElementbyId("javax.faces.ViewState");
                        var basvurubuttonId = html.DocumentNode.Descendants("button").FirstOrDefault(p => p.InnerText != null && p.InnerText.Equals("BAŞVURU")).Id;

                        var PostData = "javax.faces.partial.ajax=true&javax.faces.source=" + WebUtility.UrlEncode(basvurubuttonId) + "&javax.faces.partial.execute=" + WebUtility.UrlEncode("@all") + "&javax.faces.partial.render=tableForm+formmessage+isyeriForm+basvuruEkleForm&" + WebUtility.UrlEncode(basvurubuttonId) + "=" + WebUtility.UrlEncode(basvurubuttonId) + "&tableForm=tableForm&dataTableBasvurulistesi_selection=&javax.faces.ViewState=" + WebUtility.UrlEncode(viewstate.GetAttributeValue("value", ""));

                        response = projeGiris.PostData("https://uyg.sgk.gov.tr/IsverenSistemi/pages/genelParametreler/gecici17Basvuru.jsf", PostData);

                        if (response.Contains("<span class=\"ui-button-text\">ONAYLA</span>"))
                        {
                            sayac = 0;

                            html.LoadHtml(response);

                            var onaylaButtonId = html.DocumentNode.Descendants("button").FirstOrDefault(p => p.InnerText != null && p.InnerText.Equals("ONAYLA")).Id;
                            var formId = html.GetElementbyId("basvuruDilekcePanel").Descendants("form").FirstOrDefault().Id;

                            PostData = "javax.faces.partial.ajax=true&javax.faces.source=" + WebUtility.UrlEncode(onaylaButtonId) + "&javax.faces.partial.execute=%40all&javax.faces.partial.render=tableForm+formmessage+isyeriForm+basvuruEkleForm&" + WebUtility.UrlEncode(onaylaButtonId) + "=" + (onaylaButtonId) + "&basvuruEkleForm=basvuruEkleForm&" + WebUtility.UrlEncode(formId) + "=" + WebUtility.UrlEncode(formId) + "&ibanText=&javax.faces.ViewState=" + WebUtility.UrlEncode(viewstate.GetAttributeValue("value", ""));

                            response = projeGiris.PostData("https://uyg.sgk.gov.tr/IsverenSistemi/pages/genelParametreler/gecici17Basvuru.jsf", PostData);

                            if (!response.Equals("Error"))
                            {

                                if (response.Contains("Başvuru kaydınız oluşturulmuştur"))
                                {
                                    return "OK";
                                }
                                else if (response.Contains("Aynı gün içerisinde bir kez başvuru yapabilirsiniz"))
                                {
                                    return "Aynı gün içerisinde bir kez başvuru yapabilirsiniz";
                                }
                                else if (response.Contains("Başvuru kaydınız oluşturulamamıştır"))
                                {
                                    basvuruKaydiYapilamadiSayaci++;

                                    if (basvuruKaydiYapilamadiSayaci < 5)
                                    {
                                        projeGiris.Disconnect();

                                        Thread.Sleep(3000);

                                        goto BasvuruKaydiYapilamadiTekrarDene;
                                    }
                                    else
                                    {
                                        return "5 denemeye rağmen başvuru kaydı yapılamadı uyarısı ile karşılaşıldı";
                                    }
                                }
                            }
                            else
                            {
                                sayac++;

                                if (sayac <= 5)
                                {
                                    Thread.Sleep(3000);
                                    goto YenidenDene;
                                }
                                else
                                {
                                    return "5 denemeye rağmen başvuru kaydı yapılamadı";
                                }
                            }
                        }
                        else
                        {
                            sayac++;

                            if (sayac <= 5)
                            {
                                Thread.Sleep(3000);
                                goto YenidenDene;
                            }
                            else
                            {
                                return "5 denemeye rağmen başvuru kaydı yapılamadı";
                            }
                        }

                    }
                    else
                    {
                        sayac++;

                        if (sayac <= 5)
                        {
                            Thread.Sleep(3000);
                            goto YenidenDene;
                        }
                        else
                        {
                            return "5 denemeye rağmen başvuru kaydı yapılamadı";
                        }
                    }

                    projeGiris.Disconnect();
                }
            }

            return result;
        }
    }



}
