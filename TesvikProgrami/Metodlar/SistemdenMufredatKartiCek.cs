using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static BaseResponse SistemdenMufredatKartiCek(Isyerleri isyeri, ref ProjeGiris projeGiris)
        {
            var yeniWebClientOlustur = projeGiris == null;

            if (yeniWebClientOlustur)
            {
                projeGiris = new ProjeGiris(isyeri, Enums.ProjeTurleri.IsverenSistemi);
            }

            var result = new BaseResponse();

            List<MufredatKarti> mufredatKartlari = new List<MufredatKarti>();

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

                var baslangic = new DateTime(2018, 6, 1);
                var bitis = DateTime.Today;

                var sayac = 0;

            tekrarDene:

                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();

                var yanit = projeGiris.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkMufredat.action;");

                if (yanit.Contains("https://uyg.sgk.gov.tr/YeniSistem/Isveren/mufredatGoruntuleme.action"))
                {
                    sayac = 0;

                    html.LoadHtml(yanit);
                    var link = html.DocumentNode.Descendants("iframe").FirstOrDefault(p => p.GetAttributeValue("src", "") != null && p.GetAttributeValue("src", "").Contains("mufredatGoruntuleme.action")).GetAttributeValue("src", "");

                tekrarDene2:
                    yanit = projeGiris.Get(link);

                    if (yanit.Contains("<center>MÜFREDAT KARTI</center>"))
                    {
                        sayac = 0;
                    tekrarDene3:

                        yanit = projeGiris.PostData("https://uyg.sgk.gov.tr/YeniSistem/Isveren/secimBelirle.action", String.Format("secim=1&borcTur=100&muf_secim=0&baslangicTarih={0}&bitisTarih={1}&donem_yil_ay_index_bas=0&donem_yil_ay_index_bit=0&fisTarih={2}", baslangic.ToString("dd.MM.yyyy"), bitis.ToString("dd.MM.yyyy"), bitis.ToString("dd.MM.yyyy")));

                        if (yanit.Contains("Tarihleri Arasında Hareket Bulunamadı") || yanit.Contains("Excel listesi sadece ekranda gördüğünüz verilerden oluşacaktır") || yanit.Contains("LİSTELENECEK VERİ BULUNAMAMIŞTIR"))
                        {
                            if (yanit.Contains("Excel listesi sadece ekranda gördüğünüz verilerden oluşacaktır"))
                            {

                                html.LoadHtml(yanit);
                                var tablo = html.GetElementbyId("tablo");

                                var trs = tablo.Descendants("tr");

                                foreach (var tr in trs)
                                {
                                    var tds = tr.Descendants("td").ToList();

                                    mufredatKartlari.Add(new MufredatKarti
                                    {
                                        Kod = tds[0].GetInnerText().Trim(),
                                        IslemTarihi = tds[1].GetInnerText().Trim(),
                                        YilAy = tds[2].GetInnerText().Trim().Replace("\r", "").Replace("\t", "").Replace("\n", ""),
                                        TahsilatPostaTarihi = tds[3].GetInnerText().Trim(),
                                        BelgeMahiyeti = tds[4].GetInnerText().Trim(),
                                        BelgeCesit_TahakkukSekli = tds[5].GetInnerText().Trim(),
                                        BorcTur = tds[6].GetInnerText().Trim(),
                                        Kanun = tds[7].GetInnerText().Trim(),
                                        PEKTutari = tds[8].GetInnerText().Trim(),
                                        THKTutari = tds[9].GetInnerText().Trim(),
                                        Indirim = tds[10].GetInnerText().Trim(),
                                        Indirim5510_5073PekTutari = tds[11].GetInnerText().Trim(),
                                        THSTutari = tds[12].GetInnerText().Trim(),
                                        GZ = tds[13].GetInnerText().Trim(),
                                        TuruncuArkaPlan = tr.OuterHtml.Contains("#DEB887")
                                    });
                                }



                            //    sayac = 0;

                            //    var sayfano = 1;

                            //SonrakiSayfa:

                            //    yanit = projeGiris.PostData("https://uyg.sgk.gov.tr/YeniSistem/Isveren/secimBelirle.action", String.Format("secim=1&muf_secim=0&donemleSorgu=&borcTur=100&baslangicTarih={0}&bitisTarih={1}&sayfa_no={2}&geri=2&donem_yil_ay_index_bas=0&donem_yil_ay_index_bit=0", baslangic, bitis, sayfano));

                            //    if (yanit.Contains("Tarihleri Arasında Hareket Bulunamadı") || yanit.Contains("Excel listesi sadece ekranda gördüğünüz verilerden oluşacaktır"))
                            //    {
                            //        if (yanit.Contains("Excel listesi sadece ekranda gördüğünüz verilerden oluşacaktır"))
                            //        {
                            //            html.LoadHtml(yanit);
                            //            tablo = html.GetElementbyId("tablo");

                            //            trs = tablo.Descendants("tr");
                            //            foreach (var tr in trs)
                            //            {
                            //                var tds = tr.Descendants("td").ToList();

                            //                mufredatKartlari.Add(new MufredatKarti
                            //                {
                            //                    Kod = tds[0].GetInnerText().Trim(),
                            //                    IslemTarihi = tds[1].GetInnerText().Trim(),
                            //                    YilAy = tds[2].GetInnerText().Trim().Replace("\r", "").Replace("\t", "").Replace("\n", ""),
                            //                    TahsilatPostaTarihi = tds[3].GetInnerText().Trim(),
                            //                    BelgeMahiyeti = tds[4].GetInnerText().Trim(),
                            //                    BelgeCesit_TahakkukSekli = tds[5].GetInnerText().Trim(),
                            //                    BorcTur = tds[6].GetInnerText().Trim(),
                            //                    Kanun = tds[7].GetInnerText().Trim(),
                            //                    PEKTutari = tds[8].GetInnerText().Trim(),
                            //                    THKTutari = tds[9].GetInnerText().Trim(),
                            //                    Indirim = tds[10].GetInnerText().Trim(),
                            //                    Indirim5510_5073PekTutari = tds[11].GetInnerText().Trim(),
                            //                    THSTutari = tds[12].GetInnerText().Trim(),
                            //                    GZ = tds[13].GetInnerText().Trim(),
                            //                    TuruncuArkaPlan= tr.OuterHtml.Contains("OLMC")
                            //                });
                            //            }

                            //            sayfano++;

                            //            goto SonrakiSayfa;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        sayac++;

                            //        if (sayac < 5)
                            //        {
                            //            Thread.Sleep(500);
                            //            goto SonrakiSayfa;
                            //        }
                            //        else throw new Exception("5 denemeye rağmen bilgiler çekilemedi");
                            //    }


                            }

                            baslangic = baslangic.AddYears(1).AddDays(1);

                            if (baslangic <= bitis)
                            {
                                goto tekrarDene3;
                            }
                        }
                        else
                        {
                            sayac++;

                            if (sayac < 5)
                            {
                                Thread.Sleep(500);
                                goto tekrarDene3;
                            }
                            else throw new Exception("5 denemeye rağmen bilgiler çekilemedi");
                        }
                    }
                    else
                    {
                        sayac++;

                        if (sayac < 5)
                        {
                            Thread.Sleep(500);
                            goto tekrarDene2;
                        }
                        else throw new Exception("5 denemeye rağmen bilgiler çekilemedi");
                    }
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

                if (mufredatKartlari.Count == 0) throw new Exception("Kayıt bulunmadı");


                if (mufredatKartlari.Count > 0)
                {
                    //var grps = mufredatKartlari.GroupBy(p => p.Kod + "-" + p.IslemTarihi + "-" + p.YilAy + "-" + p.TahsilatPostaTarihi + "-" + p.BelgeMahiyeti + "-" + p.BelgeCesit_TahakkukSekli + "-" + p.BorcTur + "-" + p.Kanun + "-" + p.PEKTutari + "-" + p.THKTutari + "-" + p.Indirim + "-" + p.Indirim5510_5073PekTutari + "-" + p.THSTutari + "-" + p.GZ).ToDictionary(x => x.Key, x => x.ToList());

                    //var mukerrerler = grps.Where(p => p.Value.Count > 1).ToList();

                    //mufredatKartlari = grps.Select(p => p.Value.FirstOrDefault()).ToList();

                    var bildirgeIcmalSonuc = Metodlar.SistemdenBildirgelerinIcmaliniCek(isyeri, null);

                    if (bildirgeIcmalSonuc.Durum == false)
                    {
                        MessageBox.Show("Bildirge icmalleri çekilirken hata meydana geldi. Tahakkuk oluşturulurken icmal ile ilgili alanlar doldurulmadan devam edilecek" + Environment.NewLine + Environment.NewLine + "Hata:" + bildirgeIcmalSonuc.HataMesaji, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    var sonuc = Metodlar.MufredatKartlariniKaydet(isyeri, mufredatKartlari,bildirgeIcmalSonuc);

                    if (sonuc == null) throw new Exception("Müfredat kartı exceli kaydedilemedi");
                    else result.Result = sonuc;

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

            return result;

        }
    }



}
