using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static Classes.ResponseMuhtasarIsyerleriBul MuhtasarXmldenIsyerleriniBul(Isyerleri isyeri, string klasor, string secilenAphb, string secilenBf)
        {
            var result = new Classes.ResponseMuhtasarIsyerleriBul();

            //result.CariAphb = Metodlar.AylikListeyiYukle(Path.Combine(Application.StartupPath, "ListeTemplate.xlsx"));

            var hataverenler = new HashSet<string>();

            #region Xml Muhtasar

            {
                var bildirgeler = Directory.GetFiles(klasor, "*.xml", SearchOption.AllDirectories);

                //var aracilar = dtAphb.AsEnumerable().Select(row => row[(int)Enums.AphbHucreBilgileri.Araci].ToString()).Distinct();

                var referanslar = new Dictionary<DataRow, XElement>();

                foreach (var bildirgeDosya in bildirgeler)
                {
                    XDocument doc = null;

                    try
                    {
                        doc = XDocument.Load(bildirgeDosya);
                    }
                    catch (Exception ex)
                    {
                        if (ex is XmlException)
                        {
                            if (ex.Message.Contains("bitiş etiketiyle eşleşmiyor"))
                            {
                                if (result.HataliSatirlar == null) result.HataliSatirlar = new List<string>();
                                result.HataliSatirlar.Add("XML dosyası hatalı: " + ex.Message + Environment.NewLine);
                            }
                            else if (ex.Message.Contains("geçersiz bir karakter"))
                            {
                                if (result.HataliSatirlar == null) result.HataliSatirlar = new List<string>();
                                result.HataliSatirlar.Add("XML dosyası hatalı: " + ex.Message + Environment.NewLine);
                            }
                            else
                            {
                                hataverenler.Add(bildirgeDosya);
                            }

                            continue;
                        }

                    }


                    //xmller.Add(bildirgeDosya,doc);

                    var yil = "";
                    var ay = "";

                    var calisanlar = doc.Descendants("sgkCalisanBilgisi").ToList();

                    calisanlar.ForEach(p =>
                    {

                        p.Element("kanun").SetValue(p.Element("kanun").Value.PadLeft(5, '0'));

                        if (p.Element("isyeriAraciSiraNo") != null)
                        {
                            if (!string.IsNullOrEmpty(p.Element("isyeriAraciSiraNo").Value))
                            {
                                int.TryParse(p.Element("isyeriAraciSiraNo").Value, out int araciNo);

                                p.Element("isyeriAraciSiraNo").SetValue(araciNo.ToString().PadLeft(3, '0'));
                            }
                            else
                                p.Element("isyeriAraciSiraNo").SetValue(isyeri.TaseronNo.PadLeft(3, '0'));

                        }
                        else
                        {
                            //p.Element("isyeriAraciSiraNo").SetValue(isyeri.TaseronNo.PadLeft(3, '0'));
                        }

                        if (p.Element("belgeTuru") != null)
                        {
                            var belgeTuru = p.Element("belgeTuru").Value.ToLower();

                            if (belgeTuru.Contains("tüm sig"))
                                belgeTuru = "01";
                            else if (belgeTuru.Contains("sosyal güvenlik destek primi") || belgeTuru.Contains("sos.güv.des"))
                                belgeTuru = "02";

                            p.Element("belgeTuru").SetValue(belgeTuru.PadLeft(2, '0'));

                            if (int.TryParse(belgeTuru, out int belgeTuruInt))
                            {
                                if (!TesvikHesaplamaSabitleri.MuhtasardaBuBelgelerinHaricindeUyariVerilecek.Contains(belgeTuruInt))
                                {
                                    var tc = (p.Element("tckno") ?? p.Element("sigortaliSicil")).Value;

                                    result.Mesajlar.Add(String.Format("Xml dosyasında {0} tc nolu kişinin {1} nolu belge türü tanımlı listede yok", tc, belgeTuruInt) + Environment.NewLine);
                                }
                            }
                            else
                            {

                                var tc = (p.Element("tckno") ?? p.Element("sigortaliSicil")).Value;

                                result.HataliSatirlar.Add(String.Format("Xml dosyasında {0} tc nolu kişinin belge türü hatalı. Belge Türü: {1}", tc, belgeTuru) + Environment.NewLine);
                            }
                        }


                        if (p.Element("isyeriYeniSubeKod") != null)
                        {
                            p.Element("isyeriYeniSubeKod").SetValue(p.Element("isyeriYeniSubeKod").Value.PadLeft(2, '0'));
                        }


                        if (p.Element("isyeriEskiSubeKod") != null)
                        {
                            p.Element("isyeriEskiSubeKod").SetValue(p.Element("isyeriEskiSubeKod").Value.PadLeft(2, '0'));
                        }


                        if (p.Element("isyeriSiraNo") != null)
                        {
                            p.Element("isyeriSiraNo").SetValue(p.Element("isyeriSiraNo").Value.PadLeft(7, '0'));
                        }
                        else
                        {
                            if (result.HataliSatirlar == null) result.HataliSatirlar = new List<string>();

                            var tc = (p.Element("tckno") ?? p.Element("sigortaliSicil")).Value;

                            result.HataliSatirlar.Add(String.Format("Xml dosyasında {0} tc nolu kişinin isyeriSiraNo bilgisi eksik", tc) + Environment.NewLine);
                        }

                        if (p.Element("isyeriIlKod") != null)
                        {
                            if (!string.IsNullOrWhiteSpace(p.Element("isyeriIlKod").Value.Trim()))
                            {
                                p.Element("isyeriIlKod").SetValue(p.Element("isyeriIlKod").Value.PadLeft(3, '0'));
                            }
                        }
                        //else
                        //{
                        //    if (result.HataliSatirlar == null) result.HataliSatirlar = new List<string>();

                        //    var tc = (p.Element("tckno") ?? p.Element("sigortaliSicil")).Value;

                        //    result.HataliSatirlar.Add(String.Format("Xml dosyasında {0} tc nolu kişinin isyeriIlKod bilgisi eksik", tc) + Environment.NewLine);
                        //}

                        if (p.Element("hizmetAy") != null)
                        {
                            p.Element("hizmetAy").SetValue(p.Element("hizmetAy").Value.PadLeft(2, '0'));
                        }

                        if (p.Element("gun") != null)
                        {
                            int.TryParse(p.Element("gun").Value, out int tempgun);

                            if (tempgun > 30) tempgun = 30;

                            p.Element("gun").SetValue(tempgun.ToString());

                        }

                        if (string.IsNullOrEmpty(yil))
                        {
                            if (p.Element("hizmetYil") != null && !string.IsNullOrEmpty(p.Element("hizmetYil").Value))
                            {
                                yil = p.Element("hizmetYil").Value;
                            }
                        }

                        if (string.IsNullOrEmpty(ay))
                        {
                            if (p.Element("hizmetAy") != null && !string.IsNullOrEmpty(p.Element("hizmetAy").Value))
                            {
                                ay = p.Element("hizmetAy").Value.ToInt().ToString();
                            }
                        }
                    });

                    if (result.HataliSatirlar == null)
                    {

                        var isyerlerininKisileriRaw = calisanlar
                            .GroupBy(x =>
                                x.Element("isyeriSiraNo").Value.Trim() + "-" +
                                ((x.Element("isyeriIlKod") == null || string.IsNullOrWhiteSpace(x.Element("isyeriIlKod").Value.Trim())) ? isyeri.IsyeriSicilNo.Substring(16, 3) : x.Element("isyeriIlKod").Value).Trim() + "-" +
                                ((x.Element("isyeriAraciSiraNo") == null || string.IsNullOrWhiteSpace(x.Element("isyeriAraciSiraNo").Value.Trim())) ? isyeri.TaseronNo : x.Element("isyeriAraciSiraNo").Value.Trim()).PadLeft(3, '0')
                            )
                            .ToDictionary(x => x.Key, x => x.ToList());

                        var isyerlerininKisileri = new Dictionary<string, List<Classes.AphbSatir>>();
                        var isyerleriHatalar = new Dictionary<string, List<string>>();

                        foreach (var kv in isyerlerininKisileriRaw)
                        {
                            isyerlerininKisileri.Add(kv.Key, new List<AphbSatir>());

                            foreach (var elem in kv.Value)
                            {
                                var aphbSatir = new Classes.AphbSatir();
                                try
                                {
                                    aphbSatir.SosyalGuvenlikNo = (elem.Element("tckno") ?? elem.Element("sigortaliSicil"))?.Value;

                                    if (long.TryParse(aphbSatir.SosyalGuvenlikNo, out long temptc))
                                    {
                                        if (temptc.ToString().Length < 11)
                                            throw new Exception("Geçersiz tc");
                                    }
                                    else throw new Exception("Geçersiz tc");

                                    var kanun = elem.Element("kanun").Value;

                                    aphbSatir.Yil = yil;
                                    aphbSatir.Ay = ay;
                                    if (elem.Element("kanun").Value == "46486" || elem.Element("kanun").Value == "56486" || elem.Element("kanun").Value == "66486")
                                    {
                                        elem.Element("kanun").Value = "05510";
                                        kanun = "05510";
                                    }
                                    aphbSatir.Kanun = kanun.Equals("00000") ? "" : kanun;
                                    aphbSatir.BelgeTuru = Convert.ToInt32(elem.Element("belgeTuru").Value).ToString();
                                    aphbSatir.Mahiyet = elem.Element("belgeMahiyet").Value.ToLower().StartsWith("a") || string.IsNullOrEmpty(elem.Element("belgeMahiyet").Value.Trim()) ? "ASIL" : elem.Element("belgeMahiyet").Value.ToLower().StartsWith("e") ? "EK" : "İPTAL";
                                    aphbSatir.Adi = elem.Element("ad").Value;
                                    aphbSatir.Soyadi = elem.Element("soyad").Value;
                                    aphbSatir.Gun = elem.Element("gun").Value;
                                    aphbSatir.UCG = elem.Element("uzakCalismaGun") == null ? "0" : elem.Element("uzakCalismaGun").Value;
                                    aphbSatir.Ucret = elem.Element("hakEdilenUcret").Value.TutaraDonustur();
                                    aphbSatir.Ikramiye = elem.Element("primIkramiye") != null ? elem.Element("primIkramiye").Value.TutaraDonustur() : "0";
                                    aphbSatir.EksikGunSayisi = elem.Element("eksikGunSayisi") != null ? elem.Element("eksikGunSayisi").Value : "";
                                    aphbSatir.EksikGunNedeni = elem.Element("eksikGunNedeni") != null ? elem.Element("eksikGunNedeni").Value : "";
                                    aphbSatir.MeslekKod = elem.Element("meslekKod") != null ? elem.Element("meslekKod").Value : "";
                                    aphbSatir.GirisGunu = (elem.Element("iseGirisGun") != null && elem.Element("iseGirisAy") != null && !string.IsNullOrEmpty(elem.Element("iseGirisGun").Value) && !string.IsNullOrEmpty(elem.Element("iseGirisAy").Value)) ? elem.Element("iseGirisGun").Value + "/" + elem.Element("iseGirisAy").Value : "";
                                    aphbSatir.CikisGunu = (elem.Element("istenCikisGun") != null && elem.Element("istenCikisAy") != null && !string.IsNullOrEmpty(elem.Element("istenCikisGun").Value) && !string.IsNullOrEmpty(elem.Element("istenCikisAy").Value)) ? elem.Element("istenCikisGun").Value + "/" + elem.Element("istenCikisAy").Value : "";
                                    aphbSatir.IstenCikisNedeni = elem.Element("istenCikisNedeni") != null ? elem.Element("istenCikisNedeni").Value : "";
                                    aphbSatir.Araci = (elem.Element("isyeriAraciSiraNo") == null ? isyeri.TaseronNo : elem.Element("isyeriAraciSiraNo").Value).Equals("000") ? "Ana İşveren" : elem.Element("isyeriAraciSiraNo").Value;
                                    aphbSatir.OnayDurumu = "Onaylanmamış";
                                    aphbSatir.xElement = elem;
                                    aphbSatir.MuhtasarOrijinalKanun = elem.Element("kanun").Value;

                                    isyerlerininKisileri[kv.Key].Add(aphbSatir);

                                    if (!Sabitler.tumKanunlar.Contains(kanun.PadLeft(5, '0')))
                                        throw new Exception("Geçersiz kanun no");
                                }
                                catch (Exception ex)
                                {
                                    if (!isyerleriHatalar.ContainsKey(kv.Key))
                                        isyerleriHatalar.Add(kv.Key, new List<string>());

                                    var tc = (elem.Element("tckno") ?? elem.Element("sigortaliSicil"))?.Value;
                                    string adsoyad = null;
                                    if (string.IsNullOrEmpty(tc))
                                    {
                                        var ad = elem.Element("ad")?.Value;
                                        var soyad = elem.Element("soyad")?.Value;

                                        if (!string.IsNullOrEmpty(ad) && !string.IsNullOrEmpty(soyad))
                                            adsoyad = ad + " " + soyad;

                                    }


                                    if (string.IsNullOrEmpty(tc))
                                    {
                                        var aciklama = "Geçersiz tc";

                                        if (!string.IsNullOrEmpty(adsoyad))
                                        {
                                            isyerleriHatalar[kv.Key].Add(String.Format("{0} kişisinin bilgileri hatalı.{1}", adsoyad, aciklama));
                                        }
                                        else
                                        {
                                            isyerleriHatalar[kv.Key].Add(String.Format("Kişinin bilgileri hatalı.{0} {1}", elem.ToString(), aciklama));
                                        }
                                    }
                                    else
                                    {
                                        var aciklama = ex.Message == "Geçersiz tc" || ex.Message == "Geçersiz kanun no" ? ex.Message : "";

                                        isyerleriHatalar[kv.Key].Add(String.Format("{0} tc nolu kişinin bilgileri hatalı.{1}", tc, aciklama));
                                    }
                                }
                            }
                        }

                        foreach (var bulunanIsyeri in isyerlerininKisileri)
                        {
                            var splits = bulunanIsyeri.Key.Split('-');
                            var isyeriSiraNo = splits[0].PadLeft(7, '0');
                            var plakaNo = splits[1].PadLeft(3, '0');
                            var araciNo = Convert.ToInt32(splits[2]).ToString().PadLeft(3, '0');
                            var bulunanisyeriNovePlaka = isyeriSiraNo + plakaNo;
                            var muhtasarIsyeri = result.MuhtasarIsyerleri.FirstOrDefault(p => p.Isyeri.IsyeriSicilNo.Substring(9, 10).Equals(bulunanisyeriNovePlaka) && p.Isyeri.TaseronNo.Equals(araciNo));

                            var kisiler = bulunanIsyeri.Value;

                            if (muhtasarIsyeri == null)
                            {
                                var eklenecekIsyeri = new Classes.MuhtasarIsyeri();
                                using (var dbContext = new DbEntities())
                                {
                                    var dbIsyeri = dbContext.Isyerleri.SqlQuery("SELECT * FROM Isyerleri WHERE IsyeriSicilNo like @Ara AND TaseronNo=@TaseronNo", new System.Data.SQLite.SQLiteParameter("@Ara", "%" + bulunanisyeriNovePlaka + "%"), new SQLiteParameter("@TaseronNo", araciNo)).FirstOrDefault(p => p.IsyeriSicilNo.Substring(9, 10).Equals(bulunanisyeriNovePlaka) && p.Aktif.Equals(1));

                                    if (dbIsyeri != null)
                                    {
                                        dbIsyeri = dbContext.Isyerleri
                                                    .Include(p => p.Sirketler)
                                                    .Include(p => p.AylikCalisanSayilari)
                                                    .Include(p => p.AsgariUcretDestekTutarlari)
                                                    .Include(p => p.BasvuruDonemleri)
                                                    .Include(p => p.BorcluAylar)
                                                    .Where(p => p.IsyeriID == dbIsyeri.IsyeriID)
                                                    .FirstOrDefault();

                                        if (dbIsyeri.SirketID != isyeri.SirketID)
                                        {
                                            if (dbIsyeri.IsyeriSicilNo.Equals(isyeri.IsyeriSicilNo) == false)
                                            {
                                                return new Classes.ResponseMuhtasarIsyerleriBul { BaskaSirketMi = true };
                                            }
                                        }

                                        eklenecekIsyeri.Isyeri = dbIsyeri;

                                        muhtasarIsyeri = eklenecekIsyeri;

                                        result.MuhtasarIsyerleri.Add(eklenecekIsyeri);
                                    }
                                    else result.KayitliOlmayanIsyerleri.Add(isyeriSiraNo + "-" + plakaNo + "-" + araciNo);
                                }
                            }

                            if (muhtasarIsyeri == null)
                            {
                                result.Mesajlar.Add(String.Format("Muhtasar xmlden bulunan {0} isyeri sıra nolu plaka kodu {1} taşeron nosu {2} olan işyeri sizde kayıtlı değil{3}", isyeriSiraNo, plakaNo, araciNo, Environment.NewLine));
                            }
                            else
                            {
                                if (isyerleriHatalar.ContainsKey(bulunanIsyeri.Key) && isyerleriHatalar[bulunanIsyeri.Key].Count > 0)
                                {
                                    if (muhtasarIsyeri.hataliKisiler == null) muhtasarIsyeri.hataliKisiler = new List<string>();

                                    muhtasarIsyeri.hataliKisiler.AddRange(isyerleriHatalar[bulunanIsyeri.Key]);
                                }

                                muhtasarIsyeri.kisiler.AddRange(kisiler);

                                if (kisiler.Count > 0)
                                {
                                    var ilkKisi = kisiler.FirstOrDefault();
                                    muhtasarIsyeri.Yil = ilkKisi.Yil.ToInt();
                                    muhtasarIsyeri.Ay = ilkKisi.Ay.ToInt();
                                }

                                muhtasarIsyeri.xmller.Add(bildirgeDosya, doc);

                                var Aphb = Metodlar.FormBul(muhtasarIsyeri.Isyeri, Enums.FormTuru.Aphb);
                                var Bf = Metodlar.FormBul(muhtasarIsyeri.Isyeri, Enums.FormTuru.BasvuruFormu);

                                bool fileDialogIleSecilenAphbMi = false;
                                bool fileDialogIleSecilenBfMi = false;

                                if (muhtasarIsyeri.Isyeri.IsyeriID.Equals(isyeri.IsyeriID))
                                {
                                    if (!string.IsNullOrEmpty(secilenAphb))
                                    {
                                        Aphb = secilenAphb;
                                        fileDialogIleSecilenAphbMi = true;
                                    }

                                    if (!string.IsNullOrEmpty(secilenBf))
                                    {
                                        Bf = secilenBf;
                                        fileDialogIleSecilenBfMi = true;
                                    }
                                }


                                if (Aphb != null)
                                {
                                    FileInfo fi = new FileInfo(Aphb);

                                    if (DateTime.Now.Subtract(fi.LastWriteTime).TotalHours > 24)
                                    {
                                        if (fileDialogIleSecilenAphbMi)
                                        {
                                            result.Mesajlar.Add(String.Format("Muhtasar xmlden bulunan {0} - {1} işyeri için seçilen Aphb dosyası en son {2} tarihinde kaydedilmiş. Güncel olmayabilir.{3}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, fi.LastWriteTime.ToString("dd.MM.yyyy HH:mm"), Environment.NewLine));
                                        }
                                        else result.Mesajlar.Add(String.Format("Muhtasar xmlden bulunan {0} - {1} işyerinin Aphb dosyası en son {2} tarihinde kaydedilmiş. Güncel olmayabilir.{3}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, fi.LastWriteTime.ToString("dd.MM.yyyy HH:mm"), Environment.NewLine));
                                    }
                                    else muhtasarIsyeri.AphbGuncel = true;

                                    muhtasarIsyeri.Aphb = Aphb;
                                }
                                else
                                {
                                    result.Mesajlar.Add(String.Format("Muhtasar xmlden bulunan {0} - {1} işyerinin Aphb dosyası bulunamadı{2}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, Environment.NewLine));
                                }



                                if (Bf != null)
                                {
                                    FileInfo fi = new FileInfo(Bf);

                                    if (DateTime.Now.Subtract(fi.LastWriteTime).TotalHours > 24)
                                    {
                                        if (fileDialogIleSecilenBfMi)
                                        {
                                            result.Mesajlar.Add(String.Format("Muhtasar xmlden bulunan {0} - {1} işyeri için seçilen Başvuru formu en son {2} tarihinde kaydedilmiş. Güncel olmayabilir.{3}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, fi.LastWriteTime.ToString("dd.MM.yyyy HH:mm"), Environment.NewLine));
                                        }
                                        else result.Mesajlar.Add(String.Format("Muhtasar xmlden bulunan {0} - {1} işyerinin Başvuru formu en son {2} tarihinde kaydedilmiş. Güncel olmayabilir.{3}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, fi.LastWriteTime.ToString("dd.MM.yyyy HH:mm"), Environment.NewLine));
                                    }
                                    else muhtasarIsyeri.BfGuncel = true;

                                    muhtasarIsyeri.BasvuruFormu = Bf;
                                }
                                else
                                {
                                    result.Mesajlar.Add(String.Format("Muhtasar xmlden bulunan {0} - {1} işyerinin Başvuru formu bulunamadı{2}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, Environment.NewLine));
                                }
                            }

                        }
                    }
                }
            }

            #endregion

            #region Netsis Txt Bildirge
            {
                var bildirgeler = Directory.GetFiles(klasor, "*.txt", SearchOption.AllDirectories).ToList();

                //Okunamayan xml dosyalarını netsis txt olarak okumayı tekrar deneiyoruz
                foreach (var item in hataverenler)
                {
                    if (!bildirgeler.Contains(item)) bildirgeler.Add(item);
                }

                //var aracilar = dtAphb.AsEnumerable().Select(row => row[(int)Enums.AphbHucreBilgileri.Araci].ToString()).Distinct();

                var referanslar = new Dictionary<DataRow, int>();

                foreach (var bildirgeDosya in bildirgeler)
                {
                    //var icerik = System.Text.Encoding.GetEncoding("iso-8859-9").GetString(File.ReadAllBytes(bildirgeDosya))
                    var icerik = File.ReadAllText(bildirgeDosya, Encoding.GetEncoding("iso-8859-9"))
                                .Replace("\r\n", "|")
                                .Replace("\n", "|")
                                .Split('|')
                                .Select(p => p/*.Trim('\t')*/.Split('\t').Select(x => x.Trim()).ToArray())
                                .Where(p => p.Length > 0 && !p.All(x => string.IsNullOrEmpty(x.Trim())))
                                .ToList();
                    //xmller.Add(bildirgeDosya,doc);

                    var calisanlar = new List<string[]>();
                    var hataliSatirlar = new HashSet<string[]>();
                    //var satirHucreSiralari = new Dictionary<string[], Dictionary<Enums.NetsisHucreBilgileri, int>>();
                    var yiliEksikSatirlar = new HashSet<string[]>();
                    var tumHataliSatirlar = new HashSet<string[]>();

                    var yil = "";
                    var ay = "";

                    var silinecekSatirlar = new List<string[]>();

                    for (int i = 0; i < icerik.Count; i++)
                    {
                        var satir = icerik[i];

                        //var hucreSiralari = new Dictionary<Enums.NetsisHucreBilgileri, int>();

                        //satirHucreSiralari.Add(satir, hucreSiralari);

                        //foreach (var item in Enum.GetValues(typeof(Enums.NetsisHucreBilgileri)))
                        //{
                        //    hucreSiralari.Add((Enums.NetsisHucreBilgileri)item, Convert.ToInt32(item));
                        //}

                        if (satir.Length > (int)Enums.NetsisHucreBilgileri.TcKimlikNo)
                        {
                            long tctemp = 0;
                            int tcKaymaMiktari = 0;
                            var tcSira = (int)Enums.NetsisHucreBilgileri.TcKimlikNo;
                            while (tcKaymaMiktari + tcSira < satir.Length)
                            {
                                long.TryParse(satir[tcSira + tcKaymaMiktari], out tctemp);


                                if (tctemp > 0 && tctemp.ToString().Length >= 9)
                                {
                                    if (tcSira + tcKaymaMiktari + 1 < satir.Length)
                                    {
                                        long.TryParse(satir[tcSira + tcKaymaMiktari + 1], out long tc2);

                                        if (tctemp != tc2 && tc2 == 0) break;
                                    }
                                    else
                                        break;
                                }

                                tcKaymaMiktari++;
                            }

                            if (tctemp > 0)
                            {
                                satir = satir.Skip(tcKaymaMiktari).ToArray();

                                //if (tcKaymaMiktari > 0)
                                //{
                                //    for (int j = 0; j < hucreSiralari.Count; j++)
                                //    {
                                //        var kv = hucreSiralari.ElementAt(j);

                                //        hucreSiralari[kv.Key] += tcKaymaMiktari;

                                //    }
                                //}

                                var belgeTuru = satir[(int)Enums.NetsisHucreBilgileri.BelgeTuru].ToString().ToLower();

                                var gecerliBelgeTuru = Int32.TryParse(belgeTuru, out int bt) && bt > 0 && bt < 100;

                                if (!gecerliBelgeTuru)
                                {

                                    if (belgeTuru.Contains("tüm sig") || belgeTuru.Contains("sosyal güvenlik destek primi") || belgeTuru.Contains("sos.güv.des"))
                                        gecerliBelgeTuru = true;
                                }

                                if (gecerliBelgeTuru)
                                {

                                    var yeniTcSiraNo = (int)Enums.NetsisHucreBilgileri.TcKimlikNo;

                                    if (satir.Length > yeniTcSiraNo + 3)
                                    {

                                        if (Int32.TryParse(satir[yeniTcSiraNo + 3], out int gun) && gun >= 0 && gun <= 31)
                                        {
                                            //Tc kimlik nodan 3 sütun sonrası gün bilgisi içeriyor ise bir şey yapmıyoruz
                                        }
                                        else if (Int32.TryParse(satir[yeniTcSiraNo + 4], out int gun2) && gun2 >= 0 && gun2 <= 31)
                                        {
                                            //Tc kimlik nodan 4 sütun sonrası gün bilgisi içeriyor ise arada ilk soyadı sütunu var demek ki bu nedenle hücre sıralarını 1 kaydırıyoruz.

                                            var satirBilgileri = satir.ToList();
                                            satirBilgileri.RemoveAt((int)Enums.NetsisHucreBilgileri.Gun);
                                            satir = satirBilgileri.ToArray();

                                            //for (int j = 0; j < hucreSiralari.Count; j++)
                                            //{
                                            //    var kv = hucreSiralari.ElementAt(j);

                                            //    if (kv.Value > yeniTcSiraNo + 2)
                                            //    {
                                            //        hucreSiralari[kv.Key] += 1;
                                            //    }
                                            //}
                                        }

                                        bool UCGVar = false;

                                        if (satir.Length >= 33)
                                        {

                                            if (satir[(int)Enums.NetsisHucreBilgileri.UCG].Contains(".") || satir[(int)Enums.NetsisHucreBilgileri.UCG].Contains(","))
                                            {
                                                UCGVar = false;
                                            }
                                            else UCGVar = true;

                                            //UCGVar =    int.TryParse(satir[(int)Enums.NetsisHucreBilgileri.Gun], out _)
                                            //            &&
                                            //            int.TryParse(satir[(int)Enums.NetsisHucreBilgileri.Ucret].Replace(".", "").Replace(",", ""), out _)
                                            //            &&
                                            //            int.TryParse(satir[(int)Enums.NetsisHucreBilgileri.Ikramiye].Replace(".", "").Replace(",", ""), out _);

                                        }




                                        if (! UCGVar)
                                        {
                                            var satirBilgileri = satir.ToList();
                                            satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.Gun + 1, "0");
                                            satir = satirBilgileri.Take(33).ToArray();
                                        }


                                        var yilSira = (int)Enums.NetsisHucreBilgileri.Yil;

                                        var yilBulundu = false;

                                        for (int z = yilSira - 3; z < yilSira + 3; z++)
                                        {
                                            if (z < 0 || z > (satir.Length - 1)) continue;

                                            if (int.TryParse(satir[z], out int yilt) && (yilt == DateTime.Today.Year || yilt == (DateTime.Today.Year - 1)))
                                            {
                                                yilBulundu = true;

                                                if (string.IsNullOrEmpty(yil))
                                                {
                                                    yil = yilt.ToString();
                                                    ay = satir[z - 1].ToString();
                                                }


                                                if ((z - (int)Enums.NetsisHucreBilgileri.Ikramiye) == 10)
                                                {
                                                    //Giriş günü ve çıkış günü birleşik olarak yazıldıysa gün ve ay ayrı sütünlarda değilde tek bir sütunda yazıyorsa


                                                    var giris = satir[(int)Enums.NetsisHucreBilgileri.IseGirisGun].Trim();
                                                    var cikis = satir[(int)Enums.NetsisHucreBilgileri.IseGirisGun + 1].Trim();

                                                    var satirBilgileri = satir.ToList();

                                                    if (giris.Length == 4)
                                                    {
                                                        satirBilgileri[(int)Enums.NetsisHucreBilgileri.IseGirisGun] = giris.Substring(0, 2);
                                                        satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.IseGirisAy, giris.Substring(2, 2));

                                                    }
                                                    else
                                                    {
                                                        satirBilgileri[(int)Enums.NetsisHucreBilgileri.IseGirisGun] = string.Empty;
                                                        satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.IseGirisAy, string.Empty);
                                                    }

                                                    if (cikis.Length == 4)
                                                    {
                                                        satirBilgileri[(int)Enums.NetsisHucreBilgileri.CikisGun] = cikis.Substring(0, 2);
                                                        satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.CikisAy, cikis.Substring(2, 2));
                                                    }
                                                    else
                                                    {
                                                        satirBilgileri[(int)Enums.NetsisHucreBilgileri.CikisGun] = string.Empty;
                                                        satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.CikisAy, string.Empty);
                                                    }

                                                    satir = satirBilgileri.ToArray();


                                                    //for (int j = 0; j < hucreSiralari.Count; j++)
                                                    //{
                                                    //    var kv = hucreSiralari.ElementAt(j);

                                                    //    if (kv.Value > hucreSiralari[Enums.NetsisHucreBilgileri.IseGirisGun] + 1)
                                                    //    {
                                                    //        hucreSiralari[kv.Key] -= 2;
                                                    //    }
                                                    //}

                                                }

                                                if ((z - (int)Enums.NetsisHucreBilgileri.Ikramiye) == 10 || (z - (int)Enums.NetsisHucreBilgileri.Ikramiye) == 12)
                                                {
                                                    calisanlar.Add(satir);
                                                }

                                                break;
                                            }
                                        }

                                        if (!yilBulundu)
                                        {
                                            if ((yilSira - (int)Enums.NetsisHucreBilgileri.Ikramiye) == 10)
                                            {
                                                //Giriş günü ve çıkış günü birleşik olarak yazıldıysa gün ve ay ayrı sütünlarda değilde tek bir sütunda yazıyorsa


                                                var giris = satir[(int)Enums.NetsisHucreBilgileri.IseGirisGun].Trim();
                                                var cikis = satir[(int)Enums.NetsisHucreBilgileri.IseGirisGun + 1].Trim();

                                                var satirBilgileri = satir.ToList();

                                                if (giris.Length == 4)
                                                {
                                                    satirBilgileri[(int)Enums.NetsisHucreBilgileri.IseGirisGun] = giris.Substring(0, 2);
                                                    satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.IseGirisAy, giris.Substring(2, 2));

                                                }
                                                else
                                                {
                                                    satirBilgileri[(int)Enums.NetsisHucreBilgileri.IseGirisGun] = string.Empty;
                                                    satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.IseGirisAy, string.Empty);
                                                }

                                                if (cikis.Length == 4)
                                                {
                                                    satirBilgileri[(int)Enums.NetsisHucreBilgileri.CikisGun] = cikis.Substring(0, 2);
                                                    satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.CikisAy, cikis.Substring(2, 2));
                                                }
                                                else
                                                {
                                                    satirBilgileri[(int)Enums.NetsisHucreBilgileri.CikisGun] = string.Empty;
                                                    satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.CikisAy, string.Empty);
                                                }

                                                satir = satirBilgileri.ToArray();

                                                //for (int j = 0; j < hucreSiralari.Count; j++)
                                                //{
                                                //    var kv = hucreSiralari.ElementAt(j);

                                                //    if (kv.Value > hucreSiralari[Enums.NetsisHucreBilgileri.IseGirisGun] + 1)
                                                //    {
                                                //        hucreSiralari[kv.Key] -= 2;
                                                //    }
                                                //}
                                            }

                                            if (!string.IsNullOrEmpty(yil))
                                            {

                                                if ((yilSira - (int)Enums.NetsisHucreBilgileri.Ikramiye) == 10 || (yilSira - (int)Enums.NetsisHucreBilgileri.Ikramiye) == 12)
                                                {
                                                    calisanlar.Add(satir);
                                                }
                                            }
                                            else yiliEksikSatirlar.Add(satir);
                                        }


                                    }

                                    if (!TesvikHesaplamaSabitleri.MuhtasardaBuBelgelerinHaricindeUyariVerilecek.Contains(bt))
                                    {
                                        result.Mesajlar.Add(String.Format("Netsis txt dosyasında {0} tc nolu kişinin {1} nolu belge türü tanımlı listede yok", tctemp, bt) + Environment.NewLine);
                                    }
                                }

                                if (!calisanlar.Contains(satir))
                                {
                                    if (!yiliEksikSatirlar.Contains(satir))
                                    {

                                        if (result.HataliSatirlar == null) result.HataliSatirlar = new List<string>();

                                        tumHataliSatirlar.Add(satir);

                                    }
                                }

                            }
                            else
                            {
                                if (calisanlar.Count > 0 && satir.Where(x => !string.IsNullOrEmpty(x.Trim())).Count() >= 20)
                                {
                                    if (result.HataliSatirlar == null) result.HataliSatirlar = new List<string>();

                                    tumHataliSatirlar.Add(satir);
                                }
                                else
                                {
                                    silinecekSatirlar.Add(satir);
                                }
                            }

                        }

                        icerik[i] = satir;
                    }

                    icerik.RemoveAll(p => silinecekSatirlar.Contains(p));

                    if (!string.IsNullOrEmpty(yil))
                    {
                        foreach (var yiliEksikSatir in yiliEksikSatirlar)
                        {
                            calisanlar.Add(yiliEksikSatir);
                        }
                    }
                    else
                    {
                        foreach (var yiliEksikSatir in yiliEksikSatirlar)
                        {
                            if (result.HataliSatirlar == null) result.HataliSatirlar = new List<string>();

                            result.HataliSatirlar.Add(String.Join("\t", yiliEksikSatir));
                        }
                    }

                    foreach (var hataliSatir in tumHataliSatirlar)
                    {
                        if (result.HataliSatirlar == null) result.HataliSatirlar = new List<string>();

                        result.HataliSatirlar.Add(String.Join("\t", hataliSatir));
                    }

                    //calisanlar = icerik.Where(p => p.Length == 32 && long.TryParse(p[(int)Enums.NetsisHucreBilgileri.TcKimlikNo], out long tctemp)).ToList();

                    calisanlar.ForEach(p =>
                    {

                        p[(int)Enums.NetsisHucreBilgileri.Kanun] = p[(int)Enums.NetsisHucreBilgileri.Kanun].PadLeft(5, '0');

                        if (!string.IsNullOrEmpty(p[(int)Enums.NetsisHucreBilgileri.AraciNo]))
                        {
                            int.TryParse(p[(int)Enums.NetsisHucreBilgileri.AraciNo], out int araciNo);

                            p[(int)Enums.NetsisHucreBilgileri.AraciNo] = araciNo.ToString().PadLeft(3, '0');

                        }
                        else
                            p[(int)Enums.NetsisHucreBilgileri.AraciNo] = isyeri.TaseronNo.PadLeft(3, '0');

                        var belgeTuru = p[(int)Enums.NetsisHucreBilgileri.BelgeTuru].ToLower();

                        if (belgeTuru.Contains("tüm sig"))
                            belgeTuru = "01";
                        else if (belgeTuru.Contains("sosyal güvenlik destek primi") || belgeTuru.Contains("sos.güv.des"))
                            belgeTuru = "02";



                        p[(int)Enums.NetsisHucreBilgileri.BelgeTuru] = belgeTuru.PadLeft(2, '0');
                        p[(int)Enums.NetsisHucreBilgileri.YeniUniteKodu] = p[(int)Enums.NetsisHucreBilgileri.YeniUniteKodu].PadLeft(2, '0');
                        p[(int)Enums.NetsisHucreBilgileri.EskiUniteKodu] = p[(int)Enums.NetsisHucreBilgileri.EskiUniteKodu].PadLeft(2, '0');
                        p[(int)Enums.NetsisHucreBilgileri.IsyeriSiraNo] = p[(int)Enums.NetsisHucreBilgileri.IsyeriSiraNo].PadLeft(7, '0');

                        if (!string.IsNullOrWhiteSpace(p[(int)Enums.NetsisHucreBilgileri.IsyeriIlKodu].Trim()))
                        {
                            p[(int)Enums.NetsisHucreBilgileri.IsyeriIlKodu] = p[(int)Enums.NetsisHucreBilgileri.IsyeriIlKodu].PadLeft(3, '0');
                        }

                        p[(int)Enums.NetsisHucreBilgileri.Ay] = p[(int)Enums.NetsisHucreBilgileri.Ay].PadLeft(2, '0');

                        int.TryParse(p[(int)Enums.NetsisHucreBilgileri.Gun], out int tempgun);
                        if (tempgun > 30) tempgun = 30;
                        p[(int)Enums.NetsisHucreBilgileri.Gun] = tempgun.ToString();

                        if (string.IsNullOrEmpty(yil))
                        {
                            if (!string.IsNullOrEmpty(p[(int)Enums.NetsisHucreBilgileri.Yil].ToString()))
                            {
                                yil = p[(int)Enums.NetsisHucreBilgileri.Yil].ToString();
                            }
                        }

                        if (string.IsNullOrEmpty(ay))
                        {
                            if (!string.IsNullOrEmpty(p[(int)Enums.NetsisHucreBilgileri.Ay].ToString()))
                            {
                                ay = p[(int)Enums.NetsisHucreBilgileri.Ay].ToInt().ToString();
                            }
                        }
                    });


                    var isyerlerininKisileriRaw = calisanlar
                       .GroupBy(x =>
                              x[(int)Enums.NetsisHucreBilgileri.IsyeriSiraNo].Trim() + "-" +
                              x[(int)Enums.NetsisHucreBilgileri.IsyeriIlKodu].Trim() + "-" +
                              x[(int)Enums.NetsisHucreBilgileri.AraciNo].Trim()
                        )
                        .ToDictionary(x => x.Key, x => x.ToList());

                    var isyerlerininKisileri = new Dictionary<string, List<AphbSatir>>();
                    var isyerleriHatalar = new Dictionary<string, List<string>>();

                    foreach (var kv in isyerlerininKisileriRaw)
                    {
                        isyerlerininKisileri.Add(kv.Key, new List<AphbSatir>());

                        foreach (var p in kv.Value)
                        {
                            var aphbSatir = new AphbSatir();

                            try
                            {
                                if (hataliSatirlar.Contains(p))
                                    throw new Exception();

                                aphbSatir.SosyalGuvenlikNo = p[(int)Enums.NetsisHucreBilgileri.TcKimlikNo];

                                if (long.TryParse(aphbSatir.SosyalGuvenlikNo, out long tctemp))
                                {
                                    if (tctemp.ToString().Length != 11) throw new Exception("Geçersiz tc");
                                }
                                else throw new Exception("Geçersiz tc");


                                var kanun = p[(int)Enums.NetsisHucreBilgileri.Kanun];

                                aphbSatir.Yil = yil;
                                aphbSatir.Ay = ay;
                                if (p[(int)Enums.NetsisHucreBilgileri.Kanun].ToString() == "46486" || p[(int)Enums.NetsisHucreBilgileri.Kanun].ToString() == "56486" || p[(int)Enums.NetsisHucreBilgileri.Kanun].ToString() == "66486")
                                {
                                    p[(int)Enums.NetsisHucreBilgileri.Kanun] = "05510";
                                    kanun = "05510";
                                }
                                aphbSatir.Kanun = kanun.Equals("00000") ? "" : kanun;
                                aphbSatir.BelgeTuru = Convert.ToInt32(p[(int)Enums.NetsisHucreBilgileri.BelgeTuru]).ToString();
                                aphbSatir.Mahiyet = p[(int)Enums.NetsisHucreBilgileri.Mahiyet].ToString().ToLower().StartsWith("a") || string.IsNullOrEmpty(p[(int)Enums.NetsisHucreBilgileri.Mahiyet].Trim()) ? "ASIL" : p[(int)Enums.NetsisHucreBilgileri.Mahiyet].ToString().ToLower().StartsWith("e") ? "EK" : "İPTAL";
                                aphbSatir.SosyalGuvenlikNo = p[(int)Enums.NetsisHucreBilgileri.TcKimlikNo];
                                aphbSatir.Adi = p[(int)Enums.NetsisHucreBilgileri.Ad];
                                aphbSatir.Soyadi = p[(int)Enums.NetsisHucreBilgileri.Soyad];
                                aphbSatir.Gun = p[(int)Enums.NetsisHucreBilgileri.Gun];
                                aphbSatir.UCG = p[(int)Enums.NetsisHucreBilgileri.UCG];
                                aphbSatir.Ucret = p[(int)Enums.NetsisHucreBilgileri.Ucret].TutaraDonustur();
                                aphbSatir.Ikramiye = string.IsNullOrEmpty(p[(int)Enums.NetsisHucreBilgileri.Ikramiye]) ? "0" : p[(int)Enums.NetsisHucreBilgileri.Ikramiye].TutaraDonustur();
                                aphbSatir.EksikGunSayisi = p[(int)Enums.NetsisHucreBilgileri.EksikGunSayisi];
                                aphbSatir.EksikGunNedeni = p[(int)Enums.NetsisHucreBilgileri.EksikGunNedeni];
                                aphbSatir.MeslekKod = p[(int)Enums.NetsisHucreBilgileri.MeslekKod];
                                aphbSatir.GirisGunu = (!string.IsNullOrWhiteSpace(p[(int)Enums.NetsisHucreBilgileri.IseGirisAy].Trim()) && !string.IsNullOrWhiteSpace(p[(int)Enums.NetsisHucreBilgileri.IseGirisGun].Trim())) ? p[(int)Enums.NetsisHucreBilgileri.IseGirisGun].Trim().PadLeft(2, '0') + "/" + p[(int)Enums.NetsisHucreBilgileri.IseGirisAy].Trim().PadLeft(2, '0') : "";
                                aphbSatir.CikisGunu = (!string.IsNullOrWhiteSpace(p[(int)Enums.NetsisHucreBilgileri.CikisGun].Trim()) && !string.IsNullOrWhiteSpace(p[(int)Enums.NetsisHucreBilgileri.CikisAy].Trim())) ? p[(int)Enums.NetsisHucreBilgileri.CikisGun].Trim().PadLeft(2, '0') + "/" + p[(int)Enums.NetsisHucreBilgileri.CikisAy].Trim().PadLeft(2, '0') : "";
                                aphbSatir.IstenCikisNedeni = p[(int)Enums.NetsisHucreBilgileri.IstenCikisNedeni];
                                aphbSatir.Araci = p[(int)Enums.NetsisHucreBilgileri.AraciNo].Equals("000") ? "Ana İşveren" : p[(int)Enums.NetsisHucreBilgileri.AraciNo];
                                aphbSatir.OnayDurumu = "Onaylanmamış";
                                aphbSatir.NetsisBilgiler = new NetsisSatir { NetsisFile = icerik, netsisBilgiler = p };
                                aphbSatir.MuhtasarOrijinalKanun = p[(int)Enums.NetsisHucreBilgileri.Kanun];

                                isyerlerininKisileri[kv.Key].Add(aphbSatir);

                                if (!Sabitler.tumKanunlar.Contains(kanun.PadLeft(5, '0')))
                                    throw new Exception("Geçersiz kanun no");
                            }
                            catch (Exception ex)
                            {
                                if (!isyerleriHatalar.ContainsKey(kv.Key))
                                    isyerleriHatalar.Add(kv.Key, new List<string>());

                                var tc = p[(int)Enums.NetsisHucreBilgileri.TcKimlikNo];

                                var aciklama = ex.Message == "Geçersiz tc" || ex.Message == "Geçersiz kanun no" ? ex.Message : "";

                                isyerleriHatalar[kv.Key].Add(String.Format("{0} tc nolu kişinin bilgileri hatalı.{1}", tc, aciklama));

                            }
                        }
                    }

                    foreach (var bulunanIsyeri in isyerlerininKisileri)
                    {
                        var splits = bulunanIsyeri.Key.Split('-');
                        var isyeriSiraNo = splits[0].PadLeft(7, '0');
                        var plakaNo = string.IsNullOrWhiteSpace(splits[1].Trim()) ? isyeri.IsyeriSicilNo.Substring(16, 3) : splits[1].PadLeft(3, '0');
                        var araciNo = (string.IsNullOrWhiteSpace(splits[2].Trim()) ? isyeri.TaseronNo : splits[2]).ToInt().ToString().PadLeft(3, '0');
                        var bulunanisyeriNovePlaka = isyeriSiraNo + plakaNo;
                        var muhtasarIsyeri = result.MuhtasarIsyerleri.FirstOrDefault(p => p.Isyeri.IsyeriSicilNo.Substring(9, 10).Equals(bulunanisyeriNovePlaka) && p.Isyeri.TaseronNo.Equals(araciNo));

                        var kisiler = bulunanIsyeri.Value;

                        if (muhtasarIsyeri == null)
                        {
                            var eklenecekIsyeri = new Classes.MuhtasarIsyeri();
                            using (var dbContext = new DbEntities())
                            {
                                var dbIsyeri = dbContext.Isyerleri.SqlQuery("SELECT * FROM Isyerleri WHERE IsyeriSicilNo like @Ara  AND TaseronNo=@TaseronNo", new System.Data.SQLite.SQLiteParameter("@Ara", "%" + bulunanisyeriNovePlaka + "%"), new SQLiteParameter("@TaseronNo", araciNo)).FirstOrDefault(p => p.IsyeriSicilNo.Substring(9, 10).Equals(bulunanisyeriNovePlaka) && p.Aktif.Equals(1));


                                if (dbIsyeri != null)
                                {
                                    dbIsyeri = dbContext.Isyerleri
                                                .Include(p => p.Sirketler)
                                                .Include(p => p.AylikCalisanSayilari)
                                                .Include(p => p.AsgariUcretDestekTutarlari)
                                                .Include(p => p.BasvuruDonemleri)
                                                .Include(p => p.BorcluAylar)
                                                .Where(p => p.IsyeriID == dbIsyeri.IsyeriID)
                                                .FirstOrDefault();

                                    if (dbIsyeri.SirketID != isyeri.SirketID)
                                    {
                                        if (dbIsyeri.IsyeriSicilNo.Equals(isyeri.IsyeriSicilNo) == false)
                                        {
                                            return new Classes.ResponseMuhtasarIsyerleriBul { BaskaSirketMi = true };
                                        }
                                    }

                                    eklenecekIsyeri.Isyeri = dbIsyeri;

                                    muhtasarIsyeri = eklenecekIsyeri;

                                    result.MuhtasarIsyerleri.Add(eklenecekIsyeri);
                                }
                                else result.KayitliOlmayanIsyerleri.Add(isyeriSiraNo + "-" + plakaNo + "-" + araciNo);
                            }
                        }

                        if (muhtasarIsyeri == null)
                        {
                            result.Mesajlar.Add(String.Format("Muhtasar netsis dosyasından bulunan {0} isyeri sıra nolu plaka kodu {1}  taşeron nosu {2} olan işyeri sizde kayıtlı değil{3}", isyeriSiraNo, plakaNo, araciNo, Environment.NewLine));
                        }
                        else
                        {
                            if (isyerleriHatalar.ContainsKey(bulunanIsyeri.Key) && isyerleriHatalar[bulunanIsyeri.Key].Count > 0)
                            {
                                if (muhtasarIsyeri.hataliKisiler == null) muhtasarIsyeri.hataliKisiler = new List<string>();

                                muhtasarIsyeri.hataliKisiler.AddRange(isyerleriHatalar[bulunanIsyeri.Key]);
                            }

                            muhtasarIsyeri.kisiler.AddRange(kisiler);

                            if (kisiler.Count > 0)
                            {
                                var ilkKisi = kisiler.FirstOrDefault();
                                muhtasarIsyeri.Yil = ilkKisi.Yil.ToInt();
                                muhtasarIsyeri.Ay = ilkKisi.Ay.ToInt();
                            }

                            muhtasarIsyeri.netsisBildirgeler.Add(bildirgeDosya, icerik);

                            var Aphb = Metodlar.FormBul(muhtasarIsyeri.Isyeri, Enums.FormTuru.Aphb);
                            var Bf = Metodlar.FormBul(muhtasarIsyeri.Isyeri, Enums.FormTuru.BasvuruFormu);

                            bool fileDialogIleSecilenAphbMi = false;
                            bool fileDialogIleSecilenBfMi = false;

                            if (muhtasarIsyeri.Isyeri.IsyeriID.Equals(isyeri.IsyeriID))
                            {
                                if (!string.IsNullOrEmpty(secilenAphb))
                                {
                                    Aphb = secilenAphb;
                                    fileDialogIleSecilenAphbMi = true;
                                }

                                if (!string.IsNullOrEmpty(secilenBf))
                                {
                                    Bf = secilenBf;
                                    fileDialogIleSecilenBfMi = true;
                                }
                            }


                            if (Aphb != null)
                            {
                                FileInfo fi = new FileInfo(Aphb);

                                if (DateTime.Now.Subtract(fi.LastWriteTime).TotalHours > 24)
                                {
                                    if (fileDialogIleSecilenAphbMi)
                                    {
                                        result.Mesajlar.Add(String.Format("Muhtasar netsis dosyasından bulunan {0} - {1} işyeri için seçilen Aphb dosyası en son {2} tarihinde kaydedilmiş. Güncel olmayabilir.{3}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, fi.LastWriteTime.ToString("dd.MM.yyyy HH:mm"), Environment.NewLine));
                                    }
                                    else result.Mesajlar.Add(String.Format("Muhtasar netsis dosyasından bulunan {0} - {1} işyerinin Aphb dosyası en son {2} tarihinde kaydedilmiş. Güncel olmayabilir.{3}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, fi.LastWriteTime.ToString("dd.MM.yyyy HH:mm"), Environment.NewLine));
                                }
                                else muhtasarIsyeri.AphbGuncel = true;

                                muhtasarIsyeri.Aphb = Aphb;
                            }
                            else
                            {
                                result.Mesajlar.Add(String.Format("Muhtasar netsis dosyasından bulunan {0} - {1} işyerinin Aphb dosyası bulunamadı{2}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, Environment.NewLine));
                            }



                            if (Bf != null)
                            {
                                FileInfo fi = new FileInfo(Bf);

                                if (DateTime.Now.Subtract(fi.LastWriteTime).TotalHours > 24)
                                {
                                    if (fileDialogIleSecilenBfMi)
                                    {
                                        result.Mesajlar.Add(String.Format("Muhtasar netsis dosyasından bulunan {0} - {1} işyeri için seçilen Başvuru formu en son {2} tarihinde kaydedilmiş. Güncel olmayabilir.{3}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, fi.LastWriteTime.ToString("dd.MM.yyyy HH:mm"), Environment.NewLine));
                                    }
                                    else result.Mesajlar.Add(String.Format("Muhtasar netsis dosyasından bulunan {0} - {1} işyerinin Başvuru formu en son {2} tarihinde kaydedilmiş. Güncel olmayabilir.{3}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, fi.LastWriteTime.ToString("dd.MM.yyyy HH:mm"), Environment.NewLine));
                                }
                                else muhtasarIsyeri.BfGuncel = true;

                                muhtasarIsyeri.BasvuruFormu = Bf;
                            }
                            else
                            {
                                result.Mesajlar.Add(String.Format("Muhtasar netsis dosyasından bulunan {0} - {1} işyerinin Başvuru formu bulunamadı{2}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, Environment.NewLine));
                            }
                        }

                    }
                }
            }
            #endregion

            #region Netsis Excel Bildirge Yeni
            {
                var bildirgeler = Directory.GetFiles(klasor, "*.xls*", SearchOption.AllDirectories).ToList();

                var referanslar = new Dictionary<DataRow, DataRow>();

                foreach (var bildirgeDosya in bildirgeler)
                {
                    var excelFileName = bildirgeDosya;

                    var ds = Metodlar.ReadExcelFile(excelFileName, useHeaderRow: false);

                    if (ds.Tables.Count > 1)
                    {

                        if (new frmOnay("Muhtasar dosyasında birden fazla sayfa bulundu. Yüklenen muhtasar içinde yüklenecek bilgiler ilk sayfada olmalıdır. Eğer excelde birden fazla sayfa göremiyorsanız sayfa gizlenmiş olabilir. Sayfaların olduğu alanda sağ tuşa tıklayarak Göster deyip gizli sayfaları açabilirsiniz. Devam edilsin mi?").ShowDialog() == DialogResult.Cancel)
                        {
                            result.MuhtasardaBirdenFazlaSayfaVar = true;

                            //result.Mesajlar.Add("Muhtasar dosyasında birden fazla sayfa bulundu. Yüklenen muhtasar içinde yüklenecek bilgiler ilk sayfada olmalıdır. Eğer excelde birden fazla sayfa göremiyorsanız sayfa gizlenmiş olabilir. Sayfaların olduğu alanda sağ tuşa tıklayarak Göster deyip gizli sayfaları açabilirsiniz");
                            return result;
                        }

                    }

                    DataTable dt = ds.Tables[0];

                    bool UCGVar = false;

                    if (dt.Rows.Count > 0)
                    {
                        var firstRow = dt.Rows[0];

                        var ilkSatirdaTcVar = false;

                        for (int z = 0; z < dt.Columns.Count; z++)
                        {
                            if (long.TryParse(firstRow[z].ToString(), out long temptc))
                            {
                                if (temptc.ToString().Length >= 9)
                                {
                                    ilkSatirdaTcVar = true;
                                    break;
                                }
                            }
                        }

                        if (!ilkSatirdaTcVar)
                        {
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                if (firstRow[j].ToString().ToLower() == "ucg" || firstRow[j].ToString().ToLower() == "uzaktan çalışma gün")
                                {
                                    UCGVar = true;
                                    break;
                                }
                            }

                            dt.Rows.Remove(firstRow);

                        }

                        if (dt.Columns.Count == 33)
                        {
                            UCGVar = true;
                        }


                        int i = 0;

                        while (i < dt.Rows.Count)
                        {
                            bool enAzBirHucreDolu = false;
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString().Trim()))
                                {
                                    enAzBirHucreDolu = true;
                                    break;
                                }
                            }

                            if (enAzBirHucreDolu) i++;
                            else
                            {
                                dt.Rows.RemoveAt(i);
                            }
                        }

                        if (dt.Columns.Count > (int)Enums.NetsisHucreBilgileri.TcKimlikNo && dt.Rows.Count > 0)
                        {
                            var newfirstRow = dt.Rows[0];

                            long tctemp = 0;
                            int tcKaymaMiktari = 0;
                            var tcSira = (int)Enums.NetsisHucreBilgileri.TcKimlikNo;
                            while (tcKaymaMiktari + tcSira < dt.Columns.Count)
                            {
                                long.TryParse(newfirstRow[tcSira + tcKaymaMiktari].ToString(), out tctemp);


                                if (tctemp > 0 && tctemp.ToString().Length >= 9)
                                {
                                    if (tcSira + tcKaymaMiktari + 1 < dt.Columns.Count)
                                    {
                                        long.TryParse(newfirstRow[tcSira + tcKaymaMiktari + 1].ToString(), out long tc2);

                                        if (tctemp != tc2 && tc2 == 0) break;
                                    }
                                    else
                                        break;
                                }

                                tcKaymaMiktari++;
                            }

                            //Tc kayma miktarı kadar Datatable'ın başından sütun siliyoruz
                            for (int j = 0; j < tcKaymaMiktari; j++)
                            {
                                dt.Columns.RemoveAt(0);
                            }
                        }

                    }

                    var satirlar = new List<string[]>();

                    foreach (var row in dt.AsEnumerable())
                    {
                        var arr = new string[dt.Columns.Count];
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            arr[i] = row[i].ToString();
                        }

                        satirlar.Add(arr);
                    }



                    var hataliSatirlar = new HashSet<string[]>();
                    var yiliEksikSatirlar = new HashSet<string[]>();
                    var tumHataliSatirlar = new HashSet<string[]>();

                    var yil = "";
                    var ay = "";
                    var calisanlar = new List<string[]>();

                    for (int satirIndex = 0; satirIndex < satirlar.Count; satirIndex++)
                    {
                        var satir = satirlar[satirIndex];

                        if (satir.Length > (int)Enums.NetsisHucreBilgileri.TcKimlikNo)
                        {
                            long tctemp = 0;
                            int tcKaymaMiktari = 0;
                            var tcSira = (int)Enums.NetsisHucreBilgileri.TcKimlikNo;
                            while (tcKaymaMiktari + tcSira < satir.Length)
                            {
                                long.TryParse(satir[tcSira + tcKaymaMiktari].ToString(), out tctemp);


                                if (tctemp > 0 && tctemp.ToString().Length >= 9)
                                {
                                    if (tcSira + tcKaymaMiktari + 1 < satir.Length)
                                    {
                                        long.TryParse(satir[tcSira + tcKaymaMiktari + 1].ToString(), out long tc2);

                                        if (tctemp != tc2 && tc2 == 0) break;
                                    }
                                    else
                                        break;
                                }

                                tcKaymaMiktari++;
                            }

                            if (tctemp > 0)
                            {
                                satir = satir.Skip(tcKaymaMiktari).ToArray();

                                //if (tcKaymaMiktari > 0)
                                //{
                                //    for (int j = 0; j < hucreSiralari.Count; j++)
                                //    {
                                //        var kv = hucreSiralari.ElementAt(j);

                                //        hucreSiralari[kv.Key] += tcKaymaMiktari;

                                //    }
                                //}

                                var belgeTuru = satir[(int)Enums.NetsisHucreBilgileri.BelgeTuru].ToString().ToLower();

                                var gecerliBelgeTuru = Int32.TryParse(belgeTuru, out int bt) && bt > 0 && bt < 100;

                                if (!gecerliBelgeTuru)
                                {

                                    if (belgeTuru.Contains("tüm sig") || belgeTuru.Contains("sosyal güvenlik destek primi") || belgeTuru.Contains("sos.güv.des"))
                                        gecerliBelgeTuru = true;
                                }

                                if (gecerliBelgeTuru)
                                {

                                    var yeniTcSiraNo = (int)Enums.NetsisHucreBilgileri.TcKimlikNo;

                                    if (dt.Columns.Count > yeniTcSiraNo + 3)
                                    {
                                        if (Int32.TryParse(satir[yeniTcSiraNo + 3].ToString(), out int gun) && gun >= 0 && gun <= 31)
                                        {
                                            //Tc kimlik nodan 3 sütun sonrası gün bilgisi içeriyor ise bir şey yapmıyoruz
                                        }
                                        else if (Int32.TryParse(satir[yeniTcSiraNo + 4].ToString(), out int gun2) && gun2 >= 0 && gun2 <= 31)
                                        {
                                            //Tc kimlik nodan 4 sütun sonrası gün bilgisi içeriyor ise arada ilk soyadı sütunu var demek ki bu nedenle hücre sıralarını 1 kaydırıyoruz.

                                            var satirBilgileri = satir.ToList();
                                            satirBilgileri.RemoveAt((int)Enums.NetsisHucreBilgileri.Gun);
                                            satir = satirBilgileri.ToArray();

                                            //for (int j = 0; j < hucreSiralari.Count; j++)
                                            //{
                                            //    var kv = hucreSiralari.ElementAt(j);

                                            //    if (kv.Value > yeniTcSiraNo + 2)
                                            //    {
                                            //        hucreSiralari[kv.Key] += 1;
                                            //    }
                                            //}
                                        }

                                        if (!UCGVar)
                                        {
                                            var satirBilgileri = satir.ToList();
                                            satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.Gun + 1, "0");
                                            satir = satirBilgileri.ToArray();
                                        }

                                        var yilSira = (int)Enums.NetsisHucreBilgileri.Yil;

                                        var yilBulundu = false;

                                        for (int z = yilSira - 3; z < yilSira + 3; z++)
                                        {
                                            if (z < 0 || z > (dt.Columns.Count - 1)) continue;

                                            if (int.TryParse(satir[z].ToString(), out int yilt) && (yilt == DateTime.Today.Year || yilt == (DateTime.Today.Year - 1)))
                                            {
                                                yilBulundu = true;

                                                if (string.IsNullOrEmpty(yil))
                                                {
                                                    yil = yilt.ToString();
                                                    ay = satir[z - 1].ToString();
                                                }


                                                if ((z - (int)Enums.NetsisHucreBilgileri.Ikramiye) == 10)
                                                {
                                                    //Giriş günü ve çıkış günü birleşik olarak yazıldıysa gün ve ay ayrı sütünlarda değilde tek bir sütunda yazıyorsa


                                                    var giris = satir[(int)Enums.NetsisHucreBilgileri.IseGirisGun].ToString().Trim();
                                                    var cikis = satir[(int)Enums.NetsisHucreBilgileri.IseGirisGun + 1].ToString().Trim();

                                                    var satirBilgileri = satir.ToList();

                                                    if (giris.Length == 4)
                                                    {
                                                        satirBilgileri[(int)Enums.NetsisHucreBilgileri.IseGirisGun] = giris.Substring(0, 2);
                                                        satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.IseGirisAy, giris.Substring(2, 2));

                                                    }
                                                    else
                                                    {
                                                        satirBilgileri[(int)Enums.NetsisHucreBilgileri.IseGirisGun] = string.Empty;
                                                        satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.IseGirisAy, string.Empty);
                                                    }

                                                    if (cikis.Length == 4)
                                                    {
                                                        satirBilgileri[(int)Enums.NetsisHucreBilgileri.CikisGun] = cikis.Substring(0, 2);
                                                        satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.CikisAy, cikis.Substring(2, 2));
                                                    }
                                                    else
                                                    {
                                                        satirBilgileri[(int)Enums.NetsisHucreBilgileri.CikisGun] = string.Empty;
                                                        satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.CikisAy, string.Empty);
                                                    }

                                                    satir = satirBilgileri.ToArray();


                                                }

                                                if ((z - (int)Enums.NetsisHucreBilgileri.Ikramiye) == 10 || (z - (int)Enums.NetsisHucreBilgileri.Ikramiye) == 12)
                                                {
                                                    calisanlar.Add(satir);
                                                }

                                                break;
                                            }
                                        }

                                        if (!yilBulundu)
                                        {
                                            if ((yilSira - (int)Enums.NetsisHucreBilgileri.Ikramiye) == 10)
                                            {
                                                //Giriş günü ve çıkış günü birleşik olarak yazıldıysa gün ve ay ayrı sütünlarda değilde tek bir sütunda yazıyorsa


                                                var giris = satir[(int)Enums.NetsisHucreBilgileri.IseGirisGun].ToString().Trim();
                                                var cikis = satir[(int)Enums.NetsisHucreBilgileri.IseGirisGun + 1].ToString().Trim();

                                                var satirBilgileri = satir.ToList();

                                                if (giris.Length == 4)
                                                {
                                                    satirBilgileri[(int)Enums.NetsisHucreBilgileri.IseGirisGun] = giris.Substring(0, 2);
                                                    satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.IseGirisAy, giris.Substring(2, 2));

                                                }
                                                else
                                                {
                                                    satirBilgileri[(int)Enums.NetsisHucreBilgileri.IseGirisGun] = string.Empty;
                                                    satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.IseGirisAy, string.Empty);
                                                }

                                                if (cikis.Length == 4)
                                                {
                                                    satirBilgileri[(int)Enums.NetsisHucreBilgileri.CikisGun] = cikis.Substring(0, 2);
                                                    satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.CikisAy, cikis.Substring(2, 2));
                                                }
                                                else
                                                {
                                                    satirBilgileri[(int)Enums.NetsisHucreBilgileri.CikisGun] = string.Empty;
                                                    satirBilgileri.Insert((int)Enums.NetsisHucreBilgileri.CikisAy, string.Empty);
                                                }

                                                satir = satirBilgileri.ToArray();

                                            }


                                            if (!string.IsNullOrEmpty(yil))
                                            {

                                                if ((yilSira - (int)Enums.NetsisHucreBilgileri.Ikramiye) == 10 || (yilSira - (int)Enums.NetsisHucreBilgileri.Ikramiye) == 12)
                                                {
                                                    calisanlar.Add(satir);
                                                }
                                            }
                                            else yiliEksikSatirlar.Add(satir);
                                        }


                                    }


                                    if (!TesvikHesaplamaSabitleri.MuhtasardaBuBelgelerinHaricindeUyariVerilecek.Contains(bt))
                                    {
                                        result.Mesajlar.Add(String.Format("Netsis excel dosyasında {0} tc nolu kişinin {1} nolu belge türü tanımlı listede yok", tctemp, bt) + Environment.NewLine);
                                    }
                                }

                                if (!calisanlar.Contains(satir))
                                {
                                    if (!yiliEksikSatirlar.Contains(satir))
                                    {

                                        if (result.HataliSatirlar == null) result.HataliSatirlar = new List<string>();

                                        tumHataliSatirlar.Add(satir);
                                    }
                                }
                            }
                            else
                            {
                                if (calisanlar.Count > 0)
                                {
                                    int doluHucreSayisi = 0;
                                    for (int i = 0; i < dt.Columns.Count; i++)
                                    {
                                        if (!string.IsNullOrEmpty(satir[i].ToString().Trim())) doluHucreSayisi++;
                                    }

                                    if (doluHucreSayisi >= 20)
                                    {
                                        if (result.HataliSatirlar == null) result.HataliSatirlar = new List<string>();

                                        tumHataliSatirlar.Add(satir);
                                    }
                                }
                            }
                        }

                        satirlar[satirIndex] = satir;
                    }

                    if (!string.IsNullOrEmpty(yil))
                    {
                        foreach (var yiliEksikSatir in yiliEksikSatirlar)
                        {
                            calisanlar.Add(yiliEksikSatir);
                        }
                    }
                    else
                    {
                        foreach (var yiliEksikSatir in yiliEksikSatirlar)
                        {
                            if (result.HataliSatirlar == null) result.HataliSatirlar = new List<string>();

                            var hataMesaj = "";
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                hataMesaj += yiliEksikSatir[i].ToString() + "\t";
                            }
                            result.HataliSatirlar.Add(hataMesaj);
                        }
                    }

                    foreach (var hataliSatir in tumHataliSatirlar)
                    {
                        if (result.HataliSatirlar == null) result.HataliSatirlar = new List<string>();
                        var hataMesaj = "";
                        for (int i = 0; i < hataliSatir.Length; i++)
                        {
                            hataMesaj += hataliSatir[i].ToString() + "\t";
                        }
                        result.HataliSatirlar.Add(hataMesaj);
                    }

                    //calisanlar = icerik.Where(p => p.Length == 32 && long.TryParse(p[(int)Enums.NetsisHucreBilgileri.TcKimlikNo], out long tctemp)).ToList();

                    calisanlar.ForEach(p =>
                    {
                        p[(int)Enums.NetsisHucreBilgileri.Kanun] = p[(int)Enums.NetsisHucreBilgileri.Kanun].ToString().PadLeft(5, '0');

                        if (!string.IsNullOrEmpty(p[(int)Enums.NetsisHucreBilgileri.AraciNo].ToString()))
                        {
                            int.TryParse(p[(int)Enums.NetsisHucreBilgileri.AraciNo].ToString(), out int araciNo);

                            p[(int)Enums.NetsisHucreBilgileri.AraciNo] = araciNo.ToString().PadLeft(3, '0');

                        }
                        else
                            p[(int)Enums.NetsisHucreBilgileri.AraciNo] = isyeri.TaseronNo.PadLeft(3, '0');


                        var belgeTuru = p[(int)Enums.NetsisHucreBilgileri.BelgeTuru].ToLower();

                        if (belgeTuru.Contains("tüm sig"))
                            belgeTuru = "01";
                        else if (belgeTuru.Contains("sosyal güvenlik destek primi") || belgeTuru.Contains("sos.güv.des"))
                            belgeTuru = "02";

                        p[(int)Enums.NetsisHucreBilgileri.BelgeTuru] = belgeTuru.PadLeft(2, '0');
                        p[(int)Enums.NetsisHucreBilgileri.YeniUniteKodu] = p[(int)Enums.NetsisHucreBilgileri.YeniUniteKodu].ToString().PadLeft(2, '0');
                        p[(int)Enums.NetsisHucreBilgileri.EskiUniteKodu] = p[(int)Enums.NetsisHucreBilgileri.EskiUniteKodu].ToString().PadLeft(2, '0');
                        p[(int)Enums.NetsisHucreBilgileri.IsyeriSiraNo] = p[(int)Enums.NetsisHucreBilgileri.IsyeriSiraNo].ToString().PadLeft(7, '0');

                        if (!string.IsNullOrWhiteSpace(p[(int)Enums.NetsisHucreBilgileri.IsyeriIlKodu].ToString()))
                        {
                            p[(int)Enums.NetsisHucreBilgileri.IsyeriIlKodu] = p[(int)Enums.NetsisHucreBilgileri.IsyeriIlKodu].ToString().PadLeft(3, '0');
                        }


                        p[(int)Enums.NetsisHucreBilgileri.Ay] = p[(int)Enums.NetsisHucreBilgileri.Ay].ToString().PadLeft(2, '0');

                        int.TryParse(p[(int)Enums.NetsisHucreBilgileri.Gun].ToString(), out int tempgun);

                        if (tempgun > 30) tempgun = 30;

                        p[(int)Enums.NetsisHucreBilgileri.Gun] = tempgun.ToString();


                        if (string.IsNullOrEmpty(yil))
                        {
                            if (!string.IsNullOrEmpty(p[(int)Enums.NetsisHucreBilgileri.Yil].ToString()))
                            {
                                yil = p[(int)Enums.NetsisHucreBilgileri.Yil].ToString();
                            }
                        }

                        if (string.IsNullOrEmpty(ay))
                        {
                            if (!string.IsNullOrEmpty(p[(int)Enums.NetsisHucreBilgileri.Ay].ToString()))
                            {
                                ay = p[(int)Enums.NetsisHucreBilgileri.Ay].ToString().ToInt().ToString();
                            }
                        }
                    });


                    var isyerlerininKisileriRaw = calisanlar
                       .GroupBy(x =>
                              x[(int)Enums.NetsisHucreBilgileri.IsyeriSiraNo].ToString().Trim() + "-" +
                              x[(int)Enums.NetsisHucreBilgileri.IsyeriIlKodu].ToString().Trim() + "-" +
                              x[(int)Enums.NetsisHucreBilgileri.AraciNo].ToString().Trim()
                        )
                        .ToDictionary(x => x.Key, x => x.ToList());

                    var isyerlerininKisileri = new Dictionary<string, List<AphbSatir>>();
                    var isyerleriHatalar = new Dictionary<string, List<string>>();

                    foreach (var kv in isyerlerininKisileriRaw)
                    {
                        isyerlerininKisileri.Add(kv.Key, new List<AphbSatir>());

                        foreach (var p in kv.Value)
                        {
                            var aphbSatir = new AphbSatir();

                            try
                            {
                                if (hataliSatirlar.Contains(p))
                                    throw new Exception();

                                aphbSatir.SosyalGuvenlikNo = p[(int)Enums.NetsisHucreBilgileri.TcKimlikNo].ToString();

                                if (long.TryParse(aphbSatir.SosyalGuvenlikNo, out long tctemp))
                                {
                                    if (tctemp.ToString().Length != 11) throw new Exception("Geçersiz tc");
                                }
                                else throw new Exception("Geçersiz tc");

                                var kanun = p[(int)Enums.NetsisHucreBilgileri.Kanun].ToString();

                                aphbSatir.Yil = yil;
                                aphbSatir.Ay = ay;
                                if (p[(int)Enums.NetsisHucreBilgileri.Kanun].ToString() == "46486" || p[(int)Enums.NetsisHucreBilgileri.Kanun].ToString() == "56486" || p[(int)Enums.NetsisHucreBilgileri.Kanun].ToString() == "66486")
                                {
                                    p[(int)Enums.NetsisHucreBilgileri.Kanun] = "05510";
                                    kanun = "05510";
                                }
                              
                                aphbSatir.Kanun = kanun.Equals("00000") ? "" : kanun;
                                aphbSatir.BelgeTuru = Convert.ToInt32(p[(int)Enums.NetsisHucreBilgileri.BelgeTuru]).ToString();
                                aphbSatir.Mahiyet = p[(int)Enums.NetsisHucreBilgileri.Mahiyet].ToString().ToLower().StartsWith("a") || string.IsNullOrEmpty(p[(int)Enums.NetsisHucreBilgileri.Mahiyet].ToString().Trim()) ? "ASIL" : p[(int)Enums.NetsisHucreBilgileri.Mahiyet].ToString().ToLower().StartsWith("e") ? "EK" : "İPTAL";
                                aphbSatir.Adi = p[(int)Enums.NetsisHucreBilgileri.Ad].ToString().Trim();
                                aphbSatir.Soyadi = p[(int)Enums.NetsisHucreBilgileri.Soyad].ToString().Trim();
                                aphbSatir.Gun = p[(int)Enums.NetsisHucreBilgileri.Gun].ToString();
                                aphbSatir.UCG = p[(int)Enums.NetsisHucreBilgileri.UCG].ToString();
                                aphbSatir.Ucret = p[(int)Enums.NetsisHucreBilgileri.Ucret].ToString().TutaraDonustur();
                                aphbSatir.Ikramiye = string.IsNullOrEmpty(p[(int)Enums.NetsisHucreBilgileri.Ikramiye].ToString()) ? "0" : p[(int)Enums.NetsisHucreBilgileri.Ikramiye].ToString().TutaraDonustur();
                                aphbSatir.EksikGunSayisi = p[(int)Enums.NetsisHucreBilgileri.EksikGunSayisi].ToString();
                                aphbSatir.EksikGunNedeni = p[(int)Enums.NetsisHucreBilgileri.EksikGunNedeni].ToString();
                                aphbSatir.MeslekKod = p[(int)Enums.NetsisHucreBilgileri.MeslekKod].ToString();
                                aphbSatir.GirisGunu = (!string.IsNullOrWhiteSpace(p[(int)Enums.NetsisHucreBilgileri.IseGirisGun].Trim()) && !string.IsNullOrWhiteSpace(p[(int)Enums.NetsisHucreBilgileri.IseGirisAy].Trim())) ? p[(int)Enums.NetsisHucreBilgileri.IseGirisGun].Trim().PadLeft(2, '0') + "/" + p[(int)Enums.NetsisHucreBilgileri.IseGirisAy].Trim().PadLeft(2, '0') : "";
                                aphbSatir.CikisGunu = (!string.IsNullOrWhiteSpace(p[(int)Enums.NetsisHucreBilgileri.CikisGun].Trim()) && !string.IsNullOrWhiteSpace(p[(int)Enums.NetsisHucreBilgileri.CikisAy].Trim())) ? p[(int)Enums.NetsisHucreBilgileri.CikisGun].Trim().PadLeft(2, '0') + "/" + p[(int)Enums.NetsisHucreBilgileri.CikisAy].Trim().PadLeft(2, '0') : "";
                                aphbSatir.IstenCikisNedeni = p[(int)Enums.NetsisHucreBilgileri.IstenCikisNedeni].ToString();
                                aphbSatir.Araci = p[(int)Enums.NetsisHucreBilgileri.AraciNo].ToString().Equals("000") ? "Ana İşveren" : p[(int)Enums.NetsisHucreBilgileri.AraciNo].ToString();
                                aphbSatir.OnayDurumu = "Onaylanmamış";
                                aphbSatir.NetsisBilgilerExcel = new NetsisSatir { NetsisFile = satirlar, netsisBilgiler = p };
                                aphbSatir.MuhtasarOrijinalKanun = p[(int)Enums.NetsisHucreBilgileri.Kanun].ToString();

                                isyerlerininKisileri[kv.Key].Add(aphbSatir);

                                if (!Sabitler.tumKanunlar.Contains(kanun.PadLeft(5, '0')))
                                    throw new Exception("Geçersiz kanun no");
                            }
                            catch (Exception ex)
                            {
                                if (!isyerleriHatalar.ContainsKey(kv.Key))
                                    isyerleriHatalar.Add(kv.Key, new List<string>());

                                var tc = p[(int)Enums.NetsisHucreBilgileri.TcKimlikNo].ToString();

                                var aciklama = ex.Message == "Geçersiz tc" || ex.Message == "Geçersiz kanun no" ? ex.Message : "";

                                isyerleriHatalar[kv.Key].Add(String.Format("{0} tc nolu kişinin bilgileri hatalı.{1}", tc, aciklama));

                            }
                        }
                    }

                    foreach (var bulunanIsyeri in isyerlerininKisileri)
                    {
                        var splits = bulunanIsyeri.Key.Split('-');
                        var isyeriSiraNo = splits[0].PadLeft(7, '0');
                        var plakaNo = string.IsNullOrWhiteSpace(splits[1].Trim()) ? isyeri.IsyeriSicilNo.Substring(16, 3) : splits[1].PadLeft(3, '0');
                        var araciNo = (string.IsNullOrEmpty(splits[2]) ? isyeri.TaseronNo : splits[2]).ToInt().ToString().PadLeft(3, '0');
                        var bulunanisyeriNovePlaka = isyeriSiraNo + plakaNo;
                        var muhtasarIsyeri = result.MuhtasarIsyerleri.FirstOrDefault(p => p.Isyeri.IsyeriSicilNo.Substring(9, 10).Equals(bulunanisyeriNovePlaka) && p.Isyeri.TaseronNo.Equals(araciNo));

                        var kisiler = bulunanIsyeri.Value;

                        if (muhtasarIsyeri == null)
                        {
                            var eklenecekIsyeri = new Classes.MuhtasarIsyeri();
                            using (var dbContext = new DbEntities())
                            {
                                var dbIsyeri = dbContext.Isyerleri.SqlQuery("SELECT * FROM Isyerleri WHERE IsyeriSicilNo like @Ara  AND TaseronNo=@TaseronNo", new System.Data.SQLite.SQLiteParameter("@Ara", "%" + bulunanisyeriNovePlaka + "%"), new SQLiteParameter("@TaseronNo", araciNo)).FirstOrDefault(p => p.IsyeriSicilNo.Substring(9, 10).Equals(bulunanisyeriNovePlaka) && p.Aktif.Equals(1));


                                if (dbIsyeri != null)
                                {
                                    dbIsyeri = dbContext.Isyerleri
                                                .Include(p => p.Sirketler)
                                                .Include(p => p.AylikCalisanSayilari)
                                                .Include(p => p.AsgariUcretDestekTutarlari)
                                                .Include(p => p.BasvuruDonemleri)
                                                .Include(p => p.BorcluAylar)
                                                .Where(p => p.IsyeriID == dbIsyeri.IsyeriID)
                                                .FirstOrDefault();

                                    if (dbIsyeri.SirketID != isyeri.SirketID)
                                    {
                                        if (dbIsyeri.IsyeriSicilNo.Equals(isyeri.IsyeriSicilNo) == false)
                                        {
                                            return new Classes.ResponseMuhtasarIsyerleriBul { BaskaSirketMi = true };
                                        }
                                    }

                                    eklenecekIsyeri.Isyeri = dbIsyeri;

                                    muhtasarIsyeri = eklenecekIsyeri;

                                    result.MuhtasarIsyerleri.Add(eklenecekIsyeri);
                                }
                                else result.KayitliOlmayanIsyerleri.Add(isyeriSiraNo + "-" + plakaNo + "-" + araciNo);
                            }
                        }

                        if (muhtasarIsyeri == null)
                        {
                            result.Mesajlar.Add(String.Format("Muhtasar netsis excel dosyasından bulunan {0} isyeri sıra nolu plaka kodu {1} taşeron nosu {2} olan işyeri sizde kayıtlı değil{3}", isyeriSiraNo, plakaNo, araciNo, Environment.NewLine));
                        }
                        else
                        {
                            if (isyerleriHatalar.ContainsKey(bulunanIsyeri.Key) && isyerleriHatalar[bulunanIsyeri.Key].Count > 0)
                            {
                                if (muhtasarIsyeri.hataliKisiler == null) muhtasarIsyeri.hataliKisiler = new List<string>();

                                muhtasarIsyeri.hataliKisiler.AddRange(isyerleriHatalar[bulunanIsyeri.Key]);
                            }

                            muhtasarIsyeri.kisiler.AddRange(kisiler);

                            if (kisiler.Count > 0)
                            {
                                var ilkKisi = kisiler.FirstOrDefault();
                                muhtasarIsyeri.Yil = ilkKisi.Yil.ToInt();
                                muhtasarIsyeri.Ay = ilkKisi.Ay.ToInt();
                            }

                            muhtasarIsyeri.netsisBildirgelerExcel.Add(bildirgeDosya, satirlar);

                            var Aphb = Metodlar.FormBul(muhtasarIsyeri.Isyeri, Enums.FormTuru.Aphb);
                            var Bf = Metodlar.FormBul(muhtasarIsyeri.Isyeri, Enums.FormTuru.BasvuruFormu);

                            bool fileDialogIleSecilenAphbMi = false;
                            bool fileDialogIleSecilenBfMi = false;

                            if (muhtasarIsyeri.Isyeri.IsyeriID.Equals(isyeri.IsyeriID))
                            {
                                if (!string.IsNullOrEmpty(secilenAphb))
                                {
                                    Aphb = secilenAphb;
                                    fileDialogIleSecilenAphbMi = true;
                                }

                                if (!string.IsNullOrEmpty(secilenBf))
                                {
                                    Bf = secilenBf;
                                    fileDialogIleSecilenBfMi = true;
                                }
                            }


                            if (Aphb != null)
                            {
                                FileInfo fi = new FileInfo(Aphb);

                                if (DateTime.Now.Subtract(fi.LastWriteTime).TotalHours > 24)
                                {
                                    if (fileDialogIleSecilenAphbMi)
                                    {
                                        result.Mesajlar.Add(String.Format("Muhtasar netsis excel dosyasından bulunan {0} - {1} işyeri için seçilen Aphb dosyası en son {2} tarihinde kaydedilmiş. Güncel olmayabilir.{3}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, fi.LastWriteTime.ToString("dd.MM.yyyy HH:mm"), Environment.NewLine));
                                    }
                                    else result.Mesajlar.Add(String.Format("Muhtasar netsis excel dosyasından bulunan {0} - {1} işyerinin Aphb dosyası en son {2} tarihinde kaydedilmiş. Güncel olmayabilir.{3}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, fi.LastWriteTime.ToString("dd.MM.yyyy HH:mm"), Environment.NewLine));
                                }
                                else muhtasarIsyeri.AphbGuncel = true;

                                muhtasarIsyeri.Aphb = Aphb;
                            }
                            else
                            {
                                result.Mesajlar.Add(String.Format("Muhtasar netsis excel dosyasından bulunan {0} - {1} işyerinin Aphb dosyası bulunamadı{2}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, Environment.NewLine));
                            }



                            if (Bf != null)
                            {
                                FileInfo fi = new FileInfo(Bf);

                                if (DateTime.Now.Subtract(fi.LastWriteTime).TotalHours > 24)
                                {
                                    if (fileDialogIleSecilenBfMi)
                                    {
                                        result.Mesajlar.Add(String.Format("Muhtasar netsis excel dosyasından bulunan {0} - {1} işyeri için seçilen Başvuru formu en son {2} tarihinde kaydedilmiş. Güncel olmayabilir.{3}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, fi.LastWriteTime.ToString("dd.MM.yyyy HH:mm"), Environment.NewLine));
                                    }
                                    else result.Mesajlar.Add(String.Format("Muhtasar netsis excel dosyasından bulunan {0} - {1} işyerinin Başvuru formu en son {2} tarihinde kaydedilmiş. Güncel olmayabilir.{3}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, fi.LastWriteTime.ToString("dd.MM.yyyy HH:mm"), Environment.NewLine));
                                }
                                else muhtasarIsyeri.BfGuncel = true;

                                muhtasarIsyeri.BasvuruFormu = Bf;
                            }
                            else
                            {
                                result.Mesajlar.Add(String.Format("Muhtasar netsis dosyasından bulunan {0} - {1} işyerinin Başvuru formu bulunamadı{2}", muhtasarIsyeri.Isyeri.Sirketler.SirketAdi, muhtasarIsyeri.Isyeri.SubeAdi, Environment.NewLine));
                            }
                        }

                    }
                }
            }
            #endregion

            return result;
        }
    }
}
