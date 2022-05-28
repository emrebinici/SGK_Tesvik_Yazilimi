using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static string MuhtasarXmldenCariOlustur(
            ref Classes.MuhtasarIsyeri muhtasarIsyeri,
            ref DataTable dtAphb,
            DataTable dt14857,
            out string MuhtasardaVerilecek6486,
            ref bool muhtasarda26322_5510a_donusturulsun_mu,
            ref bool muhtasarda26322_5510a_donusturulmesi_soruldu,
            ref bool muhtasarda_kanun_nosuzdan_5510a_donusturulsun_mu,
            ref bool muhtasarda_kanun_nosuzdan_5510a_donusturulmesi_soruldu
        )
        {

            MuhtasardaVerilecek6486 = null;

            var dtCariAphb = dtAphb.Clone();

            var aracilar = dtAphb.AsEnumerable().Select(row => row[(int)Enums.AphbHucreBilgileri.Araci].ToString()).Distinct();

            var referanslar = new Dictionary<DataRow, XElement>();
            var referanslarnetsis = new Dictionary<DataRow, NetsisSatir>();
            var referanslarnetsisexcel = new Dictionary<DataRow, NetsisSatir>();

            var yil =0;
            var ay = 0;

            var kisiler = muhtasarIsyeri.kisiler;

            //var yilaylar = kisiler.Select(p => p.Yil + "-" + p.Ay).Distinct().ToList();

            //var yilAysatirlari = dtAphb.AsEnumerable()
            //    .Where(row => yilaylar.Contains((row[(int)Enums.AphbHucreBilgileri.Yil].ToString() + "-" + row[(int)Enums.AphbHucreBilgileri.Ay].ToString())) && row[(int)Enums.AphbHucreBilgileri.Mahiyet].ToString().EndsWith("PTAL") == false)
            //    .GroupBy(row => row[(int)Enums.AphbHucreBilgileri.Yil].ToString() + "-" + row[(int)Enums.AphbHucreBilgileri.Ay].ToString()).ToDictionary(x => x.Key, x => x);

            //var dict = yilAysatirlari.ToDictionary(x => x.Key,
            //                            x => new KeyValuePair<bool, bool>(x.Value.Any(row => !row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().EndsWith("6486")),
            //                                                               x.Value.Any(row => row[(int)Enums.AphbHucreBilgileri.Kanun].ToString().Equals("05510"))
            //                                                              )
            //                            );


            //var yilay = muhtasarIsyeri.Yil + "-" + muhtasarIsyeri.Ay;
            //var plakakodu = muhtasarIsyeri.Isyeri.IsyeriSicilNo.Substring(16, 3);

            //if (dict.ContainsKey(yilay))
            //{
            //    if (TesvikHesaplamaSabitleri.Iller46486.Contains(plakakodu)) MuhtasardaVerilecek6486 = "46486";
            //    else if (TesvikHesaplamaSabitleri.Iller56486.Contains(plakakodu)) MuhtasardaVerilecek6486 = "56486";
            //    else if (TesvikHesaplamaSabitleri.Iller66486.Contains(plakakodu)) MuhtasardaVerilecek6486 = "66486";
            //}

            var Cari14857deEkli = false;

            using (var dbContext = new DbEntities())
            {
                var sirketId = muhtasarIsyeri.Isyeri.SirketID;
                Cari14857deEkli = dbContext.Cari14857YapilanSirketler.FirstOrDefault(p => p.SirketId.Equals(sirketId)) != null;
            }

            var muhtasardaVerilecek6486 = "";

            var kanun6486 = kisiler.FirstOrDefault(p => p.Kanun.EndsWith("6486"))?.Kanun;

            var _7252Verilenler = kisiler.Where(p => p.Kanun.EndsWith("7252") && (p.Mahiyet.Equals("ASIL") || p.Mahiyet.Equals("EK"))).Select(p => p.SosyalGuvenlikNo).ToList();

            Dictionary<string, List<BasvuruKisi>> dict14857 = null;

            foreach (var p in kisiler)
            {
                p.OrijinalKanun = "";

                var _7252Var = _7252Verilenler.Contains(p.SosyalGuvenlikNo);

                var tumKanunlarinIcindeYok = !Sabitler.tumKanunlar.Contains(p.Kanun.PadLeft(5, '0'));

                var BasvuruListesi14857IcindeYok = false;

                if (p.Kanun.Equals("14857"))
                {
                    if (dt14857 != null)
                    {
                        var cariTarih = new DateTime(p.Yil.ToInt(), p.Ay.ToInt(),1);

                        if (dict14857 == null)
                        {
                            var basvuruKayitlariGruplar = dt14857.AsEnumerable().GroupBy(row => row[(int)Sabitler.BasvuruFormlariSutunlari["14857"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString()).ToDictionary(x => x.Key, x => x.ToList());

                            foreach (var kv in basvuruKayitlariGruplar)
                            {
                                var tc = kv.Key;

                                foreach (var basvuruSatir in kv.Value)
                                {
                                    var tesvikbaslangic = Convert.ToDateTime(basvuruSatir[(int)Sabitler.BasvuruFormlariSutunlari["14857"][Enums.BasvuruFormuSutunTurleri.TesvikBaslangic]].ToString().Trim());

                                    var tesvikbitis = DateTime.MaxValue;

                                    var bitis14857 = basvuruSatir[(int)Sabitler.BasvuruFormlariSutunlari["14857"][Enums.BasvuruFormuSutunTurleri.TesvikBitis]].ToString();

                                    if (!string.IsNullOrEmpty(bitis14857))
                                    {
                                        if (bitis14857.Contains("/"))
                                        {
                                            tesvikbitis = Convert.ToDateTime(String.Join("/", bitis14857.Split('/').Select(x => x.Trim())));
                                        }
                                        else tesvikbitis = Convert.ToDateTime(String.Join("/", bitis14857.Split(' ').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x))));
                                    }

                                    if (dict14857 == null) dict14857 = new Dictionary<string, List<BasvuruKisi>>();

                                    if (!dict14857.ContainsKey(tc)) dict14857.Add(tc, new List<BasvuruKisi>());

                                    dict14857[tc].Add(new BasvuruKisi { 
                                        TcKimlikNo = tc,
                                        TesvikDonemiBaslangic= tesvikbaslangic,
                                        TesvikDonemiBitis= tesvikbitis
                                    });
                                }
                            }
                        }


                        if (dict14857 == null || !dict14857.ContainsKey(p.SosyalGuvenlikNo)) BasvuruListesi14857IcindeYok = true;
                        else
                        {
                            if (! dict14857[p.SosyalGuvenlikNo].Any(x => x.TesvikDonemiBaslangic <= cariTarih && cariTarih <= x.TesvikDonemiBitis))
                                BasvuruListesi14857IcindeYok = true;
                        }

                    }
                    else BasvuruListesi14857IcindeYok = true;
                }

                if (p.Kanun.EndsWith("6111")
                || p.Kanun.EndsWith("6645")
                || p.Kanun.EndsWith("7103")
                || p.Kanun.EndsWith("2828")
                || p.Kanun.EndsWith("6322")
                || p.Kanun.EndsWith("25510")
                || p.Kanun.EndsWith("7252")
                || p.Kanun.EndsWith("7256")
                || p.Kanun.EndsWith("7316")
                || p.Kanun.EndsWith("3294")
                || (Cari14857deEkli && p.Kanun.Equals("14857"))
                || (p.Kanun.Equals("00000") || string.IsNullOrEmpty(p.Kanun))
                || tumKanunlarinIcindeYok
                || BasvuruListesi14857IcindeYok
            )
                {
                    bool Donustur = true;

                    if (p.Kanun == "26322")
                    {
                        if (!muhtasarda26322_5510a_donusturulmesi_soruldu)
                        {
                            muhtasarda26322_5510a_donusturulsun_mu = DialogResult.Yes == MessageBox.Show("Muhtasar dosyasında 26322 var. 5510'a dönüştürülsün mü?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            muhtasarda26322_5510a_donusturulmesi_soruldu = true;
                        }

                        if (!muhtasarda26322_5510a_donusturulsun_mu) Donustur = false;
                    }

                    if (p.Kanun.Equals("00000") || string.IsNullOrEmpty(p.Kanun))
                    {
                        if (!TesvikHesaplamaSabitleri.DestekKapsaminaGirmeyenBelgeTurleri.Contains(p.BelgeTuru))
                        {
                            if (!_7252Var)
                            {
                                if (!muhtasarda_kanun_nosuzdan_5510a_donusturulmesi_soruldu)
                                {
                                    muhtasarda_kanun_nosuzdan_5510a_donusturulsun_mu = DialogResult.Yes == MessageBox.Show("Muhtasar dosyasında kanun nosuz bildirim var. " + (string.IsNullOrEmpty(kanun6486) ? "5510" : kanun6486) + "'a dönüştürülsün mü?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                    muhtasarda_kanun_nosuzdan_5510a_donusturulmesi_soruldu = true;
                                }

                                if (!muhtasarda_kanun_nosuzdan_5510a_donusturulsun_mu) Donustur = false;
                            }
                        }
                        else
                            Donustur = false;
                    }


                    if (Donustur)
                    {

                        p.OrijinalKanun = p.Kanun.PadLeft(5, '0');

                        if (!string.IsNullOrEmpty(kanun6486))
                        {
                            p.Kanun = kanun6486;
                        }
                        else
                            p.Kanun = "05510";
                    }

                }

                if (p.Araci.Equals("Ana İşveren") == false)
                {
                    var araci = aracilar.FirstOrDefault(x => x.StartsWith(p.Araci));

                    if (araci != null)
                    {
                        p.Araci = araci;
                    }
                }

                if (p.OrijinalKanun.EndsWith("6111")
                    ||
                    p.OrijinalKanun.EndsWith("7103")
                    ||
                    p.OrijinalKanun.EndsWith("2828")
                    ||
                    p.OrijinalKanun.EndsWith("6645")
                    ||
                    p.OrijinalKanun.EndsWith("7252")
                    ||
                    p.OrijinalKanun.EndsWith("7256")
                    ||
                    p.OrijinalKanun.EndsWith("7316")
                    ||
                    p.OrijinalKanun.EndsWith("3294")
                    ||
                    p.OrijinalKanun.Equals("00000")
                    ||
                    tumKanunlarinIcindeYok
                    ||
                    BasvuruListesi14857IcindeYok
                )
                {
                    if (p.xElement != null)
                    {
                        p.xElement.Element("kanun").SetValue(p.Kanun.PadLeft(5, '0'));
                    }

                    if (p.NetsisBilgiler != null)
                    {
                        p.NetsisBilgiler.netsisBilgiler[(int)Enums.NetsisHucreBilgileri.Kanun] = p.Kanun.PadLeft(5, '0');
                    }

                    if (p.NetsisBilgilerExcel != null)
                    {
                        p.NetsisBilgilerExcel.netsisBilgiler[(int)Enums.NetsisHucreBilgileri.Kanun] = p.Kanun.PadLeft(5, '0');
                    }
                }

                if (p.Kanun.EndsWith("6486")) muhtasardaVerilecek6486 = p.Kanun;
            }

            MuhtasardaVerilecek6486 = muhtasardaVerilecek6486;

            var bildirgeGruplari = kisiler.GroupBy(p => p.Yil + "|" + p.Ay + "|" + p.Kanun + "|" + p.BelgeTuru + "|" + p.Mahiyet + "|" + p.Araci).ToDictionary(x => x.Key, x => x);

            foreach (var bildirge in bildirgeGruplari)
            {
                var sira = 1;

                foreach (var satir in bildirge.Value)
                {
                    var carirow = dtCariAphb.NewRow();

                    var satirYil = satir.Yil.ToInt();
                    var satirAy = satir.Ay.ToInt();

                    yil = satirYil;
                    ay = satirAy;

                    carirow[(int)Enums.AphbHucreBilgileri.Yil] = satirYil;
                    carirow[(int)Enums.AphbHucreBilgileri.Ay] = satirAy;
                    carirow[(int)Enums.AphbHucreBilgileri.Kanun] = satir.Kanun;
                    carirow[(int)Enums.AphbHucreBilgileri.BelgeTuru] = satir.BelgeTuru;
                    carirow[(int)Enums.AphbHucreBilgileri.Mahiyet] = satir.Mahiyet;
                    carirow[(int)Enums.AphbHucreBilgileri.SiraNo] = sira++;
                    carirow[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo] = satir.SosyalGuvenlikNo;
                    carirow[(int)Enums.AphbHucreBilgileri.Ad] = satir.Adi;
                    carirow[(int)Enums.AphbHucreBilgileri.Soyad] = satir.Soyadi;
                    carirow[(int)Enums.AphbHucreBilgileri.IlkSoyadi] = satir.IlkSoyadi;

                    if (satir.Gun == "0")
                    {
                        carirow[(int)Enums.AphbHucreBilgileri.Ucret] = string.IsNullOrEmpty(satir.Ucret) ? "0" : satir.Ucret;
                        carirow[(int)Enums.AphbHucreBilgileri.Ikramiye] = string.IsNullOrEmpty(satir.Ikramiye) ? "0" : satir.Ikramiye;
                    }
                    else
                    {
                        carirow[(int)Enums.AphbHucreBilgileri.Ucret] = satir.Ucret;
                        carirow[(int)Enums.AphbHucreBilgileri.Ikramiye] = satir.Ikramiye;
                    }

                    carirow[(int)Enums.AphbHucreBilgileri.Gun] = satir.Gun;
                    carirow[(int)Enums.AphbHucreBilgileri.UCG] = satir.UCG;
                    carirow[(int)Enums.AphbHucreBilgileri.EksikGun] = satir.EksikGunSayisi;
                    carirow[(int)Enums.AphbHucreBilgileri.GirisGunu] = satir.GirisGunu;
                    carirow[(int)Enums.AphbHucreBilgileri.CikisGunu] = satir.CikisGunu;
                    carirow[(int)Enums.AphbHucreBilgileri.EksikGunSebebi] = satir.EksikGunNedeni;
                    carirow[(int)Enums.AphbHucreBilgileri.IstenCikisNedeni] = satir.IstenCikisNedeni;
                    carirow[(int)Enums.AphbHucreBilgileri.MeslekKod] = satir.MeslekKod;
                    carirow[(int)Enums.AphbHucreBilgileri.Araci] = satir.Araci;
                    carirow[(int)Enums.AphbHucreBilgileri.OnayDurumu] = satir.OnayDurumu;
                    carirow[(int)Enums.AphbHucreBilgileri.OrijinalKanunNo] = satir.OrijinalKanun;

                    dtCariAphb.Rows.Add(carirow);

                    if (satir.xElement != null) referanslar.Add(carirow, satir.xElement);
                    if (satir.NetsisBilgiler != null) referanslarnetsis.Add(carirow, satir.NetsisBilgiler);
                    if (satir.NetsisBilgilerExcel != null) referanslarnetsisexcel.Add(carirow, satir.NetsisBilgilerExcel);
                }

            }


            var eklenecekSatirlar = dtCariAphb.AsEnumerable()
                                            .OrderByDescending(p => p[(int)Enums.AphbHucreBilgileri.Araci])
                                            .ThenBy(p => p[(int)Enums.AphbHucreBilgileri.BelgeTuru])
                                            .ThenByDescending(p => p[(int)Enums.AphbHucreBilgileri.OrijinalKanunNo])
                                            .ThenBy(p => p[(int)Enums.AphbHucreBilgileri.Mahiyet]);

            var siraNo = eklenecekSatirlar.Count();

            var SiraNo2 = 1;

            var dtCari = dtCariAphb.Clone();

            foreach (var eklenecekSatir in eklenecekSatirlar)
            {
                var newRow = dtAphb.NewRow();

                for (int i = 0; i < dtAphb.Columns.Count; i++)
                {
                    newRow[i] = eklenecekSatir[i];
                }

                newRow[(int)Enums.AphbHucreBilgileri.SiraNo] = siraNo--;

                dtAphb.Rows.InsertAt(newRow, 0);

                if (referanslar.ContainsKey(eklenecekSatir))
                {
                    muhtasarIsyeri.SatirReferanslari.Add(newRow, referanslar[eklenecekSatir]);

                }

                if (referanslarnetsis.ContainsKey(eklenecekSatir))
                {
                    muhtasarIsyeri.SatirReferanslariNetsis.Add(newRow, referanslarnetsis[eklenecekSatir]);
                }

                if (referanslarnetsisexcel.ContainsKey(eklenecekSatir))
                {
                    muhtasarIsyeri.SatirReferanslariNetsisExcel.Add(newRow, referanslarnetsisexcel[eklenecekSatir]);
                }


                var cariRow = dtCari.NewRow();

                for (int i = 0; i < dtCari.Columns.Count; i++)
                {
                    cariRow[i] = eklenecekSatir[i];
                }

                cariRow[(int)Enums.AphbHucreBilgileri.SiraNo] = SiraNo2++;

                dtCari.Rows.Add(cariRow);

            }

            dtCariAphb = dtCari;

            muhtasarIsyeri.CariAphb = dtCariAphb;

            muhtasarIsyeri.Yil = yil;
            muhtasarIsyeri.Ay = ay;

            Metodlar.CariAphbKaydet(muhtasarIsyeri.Isyeri, dtCariAphb, new DateTime(yil, ay, 1));

            return "OK";
        }


    }



}
