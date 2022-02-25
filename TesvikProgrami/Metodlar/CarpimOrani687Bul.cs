using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static decimal CarpimOraniBul687(ProjeGiris webclient)
        {
            string responseOnaylanmisBildirgeler = webclient.Get("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukonaylanmisTahakkukDonemBilgileriniYukle.action", string.Empty);

            if (responseOnaylanmisBildirgeler.Contains("İşlem Yapılacak Bildirge Dönemi Giriş"))
            {

                HtmlAgilityPack.HtmlDocument htmlOnaylanmisBildirgeler = new HtmlAgilityPack.HtmlDocument();

                htmlOnaylanmisBildirgeler.LoadHtml(responseOnaylanmisBildirgeler);

                var selectaraci = htmlOnaylanmisBildirgeler.GetElementbyId("tahakkukonaylanmisTahakkukDonemSecildi_isyeri_internetGosterimAraciNo");

                var baslangicselect = htmlOnaylanmisBildirgeler.GetElementbyId("tahakkukonaylanmisTahakkukDonemSecildi_hizmet_yil_ay_index");

                var bitisselect = htmlOnaylanmisBildirgeler.GetElementbyId("tahakkukonaylanmisTahakkukDonemSecildi_hizmet_yil_ay_index_bitis");

                if (baslangicselect != null && bitisselect != null)
                {
                    var enbuyuktarih = baslangicselect.Descendants("option").Where(p => !p.GetAttributeValue("value", "").Equals("-1")).OrderByDescending(p => new DateTime(Convert.ToInt32(p.InnerText.Trim().Split('/')[0]), Convert.ToInt32(p.InnerText.Trim().Split('/')[1]), 1)).First();

                    var enkucuktarih = bitisselect.Descendants("option").Where(p => !p.GetAttributeValue("value", "").Equals("-1")).OrderBy(p => new DateTime(Convert.ToInt32(p.InnerText.Trim().Split('/')[0]), Convert.ToInt32(p.InnerText.Trim().Split('/')[1]), 1)).First();

                    string PostData = selectaraci != null ? "isyeri.internetGosterimAraciNo=0&" : "";

                    PostData += "hizmet_yil_ay_index=" + enbuyuktarih.GetAttributeValue("value", "") + "&hizmet_yil_ay_index_bitis=" + enkucuktarih.GetAttributeValue("value", "");

                    responseOnaylanmisBildirgeler = webclient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukonaylanmisTahakkukDonemSecildi.action", PostData);

                    if (responseOnaylanmisBildirgeler.Contains("Onaylı Bildirge Listesi"))
                    {
                        htmlOnaylanmisBildirgeler.LoadHtml(responseOnaylanmisBildirgeler);

                        var tableOnayliBildirgeler = htmlOnaylanmisBildirgeler.DocumentNode.Descendants("table").FirstOrDefault(p => p.GetAttributeValue("class", "").Equals("gradienttable"));

                        if (tableOnayliBildirgeler != null)
                        {
                            var onayliBildirgeSatirlari = tableOnayliBildirgeler.Descendants("tr");

                            bool bildirgeVarmi2016Yilinda = false;

                            for (int j = 2; j < onayliBildirgeSatirlari.Count(); j++)
                            {
                                var hizmetYilAy = onayliBildirgeSatirlari.ElementAt(j).Descendants("td").ElementAt(1).InnerText.Trim();

                                DateTime dt = new DateTime(Convert.ToInt32(hizmetYilAy.Split('/')[0]), Convert.ToInt32(hizmetYilAy.Split('/')[1]), 1);

                                if (dt.Year == 2016)
                                {
                                    bildirgeVarmi2016Yilinda = true;

                                    break;

                                }
                            }

                            return TesvikHesaplamaSabitleri.CarpimOrani687 / (bildirgeVarmi2016Yilinda ? 1 : 2);
                        }

                    }
                }
            }

            return TesvikHesaplamaSabitleri.CarpimOrani687;
        }



    }



}
