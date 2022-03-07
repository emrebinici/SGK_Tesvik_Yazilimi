using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Xml;
using System.Xml.Linq;
using Excel2 = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

namespace TesvikProgrami.Classes
{
    public partial class TesvikHesapla
    {
        public string ayliklisteyolu = null;

        public string basvurulisteyolu = null;

        public string cariAphbYolu = null;

        List<object> HafizadanAtilacaklar = new List<object>();

        Dictionary<string, Excel2.Range> IlkSayfaHucreleri = new Dictionary<string, Excel2.Range>();

        Dictionary<string, Excel2.Range> DevamSayfaHucreleri = new Dictionary<string, Excel2.Range>();

        public Isyerleri Isyeri = null;

        public Dictionary<DateTime, Dictionary<string, long>> AyCalisanSayilari = new Dictionary<DateTime, Dictionary<string, long>>();

        public Dictionary<DateTime, Dictionary<string, long>> AyCalisanSayilariBazHesaplama = new Dictionary<DateTime, Dictionary<string, long>>();

        public Dictionary<string, Dictionary<DateTime, List<string>>> BasvuruListesiBazGruplari = new Dictionary<string, Dictionary<DateTime, List<string>>>();

        public List<Kisi> BasvuruListesindeOlanKisiler = new List<Kisi>();

        public List<BasvuruListesi7166Kisi> BasvuruListesindeOlanKisiler7166 = new List<BasvuruListesi7166Kisi>();

        public List<DataRow> PasifOlanlar6645 = new List<DataRow>();

        public DataTable dthatalisatirlar = null;

        public Dictionary<string, Dictionary<Enums.BasvuruFormuSutunTurleri, int>> BasvuruFormlariSutunlari = new Dictionary<string, Dictionary<Enums.BasvuruFormuSutunTurleri, int>>();

        public Dictionary<string, DataTable> BasvuruFormlariHataliSatirlar = new Dictionary<string, DataTable>();

        public DateTime CariAy;

        public bool CariAphbOtomatikOlusturuldu;

        public Dictionary<string, Classes.Tesvik> TumTesvikler = null;

        public Dictionary<DataRow, XElement> SatirReferanslari = new Dictionary<DataRow, System.Xml.Linq.XElement>();
        public Dictionary<DataRow, NetsisSatir> SatirReferanslariNetsis = new Dictionary<DataRow, NetsisSatir>();
        public Dictionary<DataRow, NetsisSatir> SatirReferanslariNetsisExcel = new Dictionary<DataRow, NetsisSatir>();
        public Dictionary<string, XDocument> xmller = new Dictionary<string, System.Xml.Linq.XDocument>();
        public Dictionary<string, List<string[]>> netsisBildirgeler = new Dictionary<string, List<string[]>>();
        public Dictionary<string, List<string[]>> netsisBildirgelerExcel = new Dictionary<string, List<string[]>>();
        public TumKisilerSonuc TumKisilerSonuc;
        public string txtYil;
        public bool SirketCari14857ListesindeVarMi;
        int muhtasarYil = 0;
        int muhtasarAy = 0;
        DateTime muhtasarTarih;
        public string MuhtasardaVerilecek6486 = null;
        public string isyeriSavePath;
        public bool CariHesapla = false;
        public bool BasvuruYoksaTesvikVerilmesin = true;
        public bool FaraziHesapla = false;
        public bool AsgariUcretDestekTutarlariDikkateAlinsin = true;

        public Dictionary<string, Dictionary<string, bool>> EksikGunuKodundanDolayiUyariVerilenKisiler = new Dictionary<string, Dictionary<string, bool>>();

        public Dictionary<DateTime, decimal> AsgariUcretDestegiIcmalleri = new Dictionary<DateTime, decimal>();

        public Dictionary<Isyerleri, HashSet<string>> hataliGunuOlanKisiler = new Dictionary<Isyerleri, HashSet<string>>();

        public HashSet<DataRow> Liste14857 = new HashSet<DataRow>();

        public bool CariAyMi(int yil, int ay)
        {
            return CariAyMi(new DateTime(yil, ay, 1));
        }
        public bool CariAyMi(DateTime yilAy)
        {
            if (CariHesapla)
            {
                if (muhtasarYil == yilAy.Year && muhtasarAy == yilAy.Month)
                    return true;
                else if (CariAy == yilAy)
                    return true;
            }

            return false;
        }

        public void BildirgeOlusturmayaBasla(
            frmBildirgeOlustur formBildirgeOlustur,
            bool SadeceIcmal
            )
        {
            bool CariYilMi = !string.IsNullOrEmpty(formBildirgeOlustur.txtYil.Text);

            txtYil = formBildirgeOlustur.txtYil.Text;

            string cariKlasor = null;
            List<string> cariKlasorler = new List<string>();

            bool DevamEdilsinMi = true;

            var hesaplanacakIsyerleri = new List<Isyerleri>();

            var seciliIsyeri = Isyeri;

            hesaplanacakIsyerleri.Add(seciliIsyeri);

            var responseMuhtasarIsyerleriBul = new ResponseMuhtasarIsyerleriBul();

            Dictionary<long, DataTable> AphbListeleri = new Dictionary<long, DataTable>();
            Dictionary<long, DataSet> BasvuruFormuListeleri = new Dictionary<long, DataSet>();
            //Dictionary<long, TumKisilerSonuc> TumKisilerListeleri = new Dictionary<long, TumKisilerSonuc>();

            if (!string.IsNullOrEmpty(formBildirgeOlustur.txtCariAphb.Text))
            {
                if (!string.IsNullOrEmpty(cariAphbYolu))
                {
                    var guid = Guid.NewGuid().ToString();

                    var dosyaismi = guid;

                    string filename = null;

                    var klasor = Directory.CreateDirectory(Path.Combine("temp", guid));

                    if (Path.GetExtension(cariAphbYolu).ToLower().EndsWith(".xml"))
                    {
                        filename = Path.GetFileName(cariAphbYolu);
                    }
                    else if (Path.GetExtension(cariAphbYolu).ToLower().EndsWith(".txt"))
                    {
                        filename = Path.GetFileName(cariAphbYolu);
                    }
                    else if (Path.GetExtension(cariAphbYolu).ToLower().EndsWith(".xls"))
                    {
                        filename = Path.GetFileName(cariAphbYolu);
                    }
                    else if (Path.GetExtension(cariAphbYolu).ToLower().EndsWith(".xlsx"))
                    {
                        filename = Path.GetFileName(cariAphbYolu);
                    }
                    else
                    {
                        filename = Path.GetFileName(cariAphbYolu) + (Path.GetExtension(cariAphbYolu).ToLower().EndsWith(".zip") ? "" : ".zip");
                    }

                    var dest = Path.Combine(klasor.FullName, filename);

                    File.Copy(cariAphbYolu, dest);

                    if (filename.EndsWith(".zip"))
                    {
                        ZipFile.ExtractToDirectory(dest, Path.Combine(klasor.FullName, guid), System.Text.Encoding.GetEncoding("ibm857"));
                    }

                    cariKlasor = klasor.FullName;

                    responseMuhtasarIsyerleriBul = Metodlar.MuhtasarXmldenIsyerleriniBul(seciliIsyeri, klasor.FullName, formBildirgeOlustur.ofdAylikListe.FileName, formBildirgeOlustur.ofdBasvuruFormu.FileName);


                    if (responseMuhtasarIsyerleriBul.BaskaSirketMi)
                    {
                        DevamEdilsinMi = false;

                        if (Path.GetExtension(cariAphbYolu).ToLower().EndsWith(".xml"))
                        {
                            MessageBox.Show("Yüklenen muhtasar xml başka bir şirkete ait", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (Path.GetExtension(cariAphbYolu).ToLower().EndsWith(".txt"))
                        {
                            MessageBox.Show("Yüklenen netsis bildirge başka bir şirkete ait", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (Path.GetExtension(cariAphbYolu).ToLower().StartsWith(".xls"))
                        {
                            MessageBox.Show("Yüklenen netsis excel bildirge başka bir şirkete ait", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (responseMuhtasarIsyerleriBul.MuhtasardaBirdenFazlaSayfaVar)
                    {
                        DevamEdilsinMi = false;
                    }

                    if (responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.Count > 0)
                    {
                        if (!responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.Any(p => p.Isyeri.IsyeriID == seciliIsyeri.IsyeriID))
                        {
                            if (new frmOnay("Şirkete ait başka şubeyi yüklediniz. Devam edilsin mi?").ShowDialog() == DialogResult.Cancel)
                            {
                                DevamEdilsinMi = false;
                            }
                        }
                    }


                    var hataliKayitIcerenIsyerleri = responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.Where(p => p.hataliKisiler != null);
                    var hataliIsyeriSayisi = hataliKayitIcerenIsyerleri.Count();

                    if (responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.Count > 0)
                    {
                        HashSet<string> eksikOlanSubeler = new HashSet<string>();

                        using (var dbContext = new DbEntities())
                        {
                            var sirket = dbContext.Sirketler
                                               .Include(p => p.Isyerleri)
                                               .Where(p => p.SirketID == Isyeri.SirketID)
                                               .FirstOrDefault();

                            foreach (var isy in sirket.Isyerleri)
                            {
                                if (Convert.ToBoolean(isy.Aktif))
                                {
                                    if (!responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.Any(x => x.Isyeri.IsyeriID == isy.IsyeriID))
                                    {
                                        var aphbYol = Metodlar.FormBul(isy, Enums.FormTuru.Aphb);

                                        if (aphbYol != null)
                                        {
                                            DataTable dtaylikliste = Metodlar.AylikListeyiYukle(aphbYol);

                                            var isyeriTumKisiler = Metodlar.TumKisileriGetir(dtaylikliste,
                                            formBildirgeOlustur.txtYil.Text.Trim(),
                                            formBildirgeOlustur.txtAy.Text.Trim(),
                                            formBildirgeOlustur.txtYilBitis.Text.Trim(),
                                            formBildirgeOlustur.txtAyBitis.Text.Trim());

                                            if (!Metodlar.SonAydaHerkesCikmisMi(isyeriTumKisiler))
                                            {
                                                eksikOlanSubeler.Add(isy.SubeAdi);
                                            }

                                        }
                                        else
                                            eksikOlanSubeler.Add(isy.SubeAdi);

                                    }
                                }
                            }

                            if (eksikOlanSubeler.Count > 0)
                            {
                                responseMuhtasarIsyerleriBul.Mesajlar.Add(Environment.NewLine + "Dosyası Bulunamayan Şubeler:");

                                foreach (var item in eksikOlanSubeler)
                                {
                                    responseMuhtasarIsyerleriBul.Mesajlar.Add(Environment.NewLine + item);
                                }
                            }
                        }

                        var taseronlar = responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.Where(p => p.Isyeri.TaseronNo.ToInt() > 0);

                        if (taseronlar.Count() > 0)
                        {
                            responseMuhtasarIsyerleriBul.Mesajlar.Add(Environment.NewLine + "Dosyada Bulunan Taşeronlar:");

                            foreach (var taseronIsyeri in taseronlar)
                            {
                                responseMuhtasarIsyerleriBul.Mesajlar.Add(Environment.NewLine + String.Format("{0} - {1} , Sicil: {2} , TaşeronNo: {3}", taseronIsyeri.Isyeri.Sirketler.SirketAdi, taseronIsyeri.Isyeri.SubeAdi, taseronIsyeri.Isyeri.IsyeriSicilNo, taseronIsyeri.Isyeri.TaseronNo));
                            }
                        }



                        if (hataliKayitIcerenIsyerleri.Count() > 0)
                        {
                            var hatamesajlari = "";

                            hatamesajlari += "HATALI KAYIT İÇEREN İŞYERLERİ" + Environment.NewLine;

                            foreach (var hataliIsyeri in hataliKayitIcerenIsyerleri)
                            {
                                hatamesajlari += Environment.NewLine + String.Format("{0} - {1} , Sicil = {2} ,TaşeronNo={3}", hataliIsyeri.Isyeri.Sirketler.SirketAdi, hataliIsyeri.Isyeri.SubeAdi, hataliIsyeri.Isyeri.IsyeriSicilNo, hataliIsyeri.Isyeri.TaseronNo) + Environment.NewLine;

                                foreach (var hatalKisiMesaj in hataliIsyeri.hataliKisiler)
                                {
                                    hatamesajlari += hatalKisiMesaj + Environment.NewLine;
                                }
                            }

                            if (new frmOnay(hatamesajlari).ShowDialog() == DialogResult.Cancel)
                            {
                                DevamEdilsinMi = false;
                            }
                            else
                            {
                                responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.RemoveAll(p => p.hataliKisiler != null && !p.hataliKisiler.Any(x => x.Contains("Geçersiz kanun no")));
                            }

                        }

                    }

                    if (responseMuhtasarIsyerleriBul.HataliSatirlar != null)
                    {
                        var hatamesajlari = "";

                        hatamesajlari += "HATALI KAYITLAR" + Environment.NewLine + Environment.NewLine;

                        foreach (var hataliSatir in responseMuhtasarIsyerleriBul.HataliSatirlar)
                        {
                            hatamesajlari += hataliSatir + Environment.NewLine;
                        }

                        if (new frmOnay(hatamesajlari).ShowDialog() == DialogResult.Cancel)
                        {
                            DevamEdilsinMi = false;
                        }

                    }


                    if (DevamEdilsinMi && responseMuhtasarIsyerleriBul.Mesajlar.Count > 0)
                    {
                        if (new frmOnay(String.Join(Environment.NewLine, responseMuhtasarIsyerleriBul.Mesajlar)).ShowDialog() == DialogResult.Cancel)
                        {
                            DevamEdilsinMi = false;
                        }

                    }

                    if (DevamEdilsinMi && responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.Count == 0)
                    {
                        DevamEdilsinMi = false;

                        if (responseMuhtasarIsyerleriBul.KayitliOlmayanIsyerleri.Count > 0)
                        {
                            MessageBox.Show("Muhtasar için yüklenen dosyadaki hiç bir işyeri sizde kayıtlı değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            if (hataliIsyeriSayisi == 0 && responseMuhtasarIsyerleriBul.HataliSatirlar == null)
                            {
                                MessageBox.Show("Muhtasar için yüklenen dosyanın içeriği okunamadı", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                MessageBox.Show("İşyerinde hatalı kayıtlar olduğu için teşvik hesaplanamadı", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                    }

                    if (DevamEdilsinMi)
                    {

                        var eklenecekIsyerleri = responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.Where(p => !p.Isyeri.IsyeriID.Equals(seciliIsyeri.IsyeriID)).Select(p => p.Isyeri);
                        if (eklenecekIsyerleri.Count() > 0) hesaplanacakIsyerleri.AddRange(eklenecekIsyerleri);

                        var seciliIsyeriMuhtasar = responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.FirstOrDefault(p => p.Isyeri.IsyeriID.Equals(seciliIsyeri.IsyeriID));

                        if (seciliIsyeriMuhtasar != null && seciliIsyeriMuhtasar.hataliKisiler != null && !seciliIsyeriMuhtasar.hataliKisiler.Any(x => x.Contains("Geçersiz kanun no")))
                        {
                            hesaplanacakIsyerleri.Remove(seciliIsyeri);
                        }

                    }
                }
            }

            if (DevamEdilsinMi)
            {

                List<string> mesajlar = new List<string>();

                if (!FaraziHesapla && !Program.BasvuruYoksaTesvikVerilmesin && formBildirgeOlustur.txtYil.Text.Trim().Equals(string.Empty) && formBildirgeOlustur.txtAy.Text.Trim().Equals(string.Empty) && formBildirgeOlustur.txtYilBitis.Text.Trim().Equals(string.Empty) && formBildirgeOlustur.txtAyBitis.Text.Trim().Equals(string.Empty))
                {
                    mesajlar.Add("Ayarlarda başvuru yoksa teşvik verilmesin seçeneği aktif değil");
                }


                if (Program.TumTesvikler.Any(p => p.Value.AsgariUcretDestekTutarlariDikkateAlinsin && Program.AsgariUcretDestekTutariDikkateAlinsin[p.Key] == false) && !FaraziHesapla)
                {
                    mesajlar.Add("Asgari ücret destek tutarları desteği en az bir teşvik için pasif hale getirilmiş");
                }

                foreach (var hesaplanacakIsyeri in hesaplanacakIsyerleri)
                {
                    if (Program.TumTesvikler.Any(p => p.Value.AsgariUcretDestekTutarlariDikkateAlinsin && Program.AsgariUcretDestekTutariDikkateAlinsin[p.Key]) && !FaraziHesapla && hesaplanacakIsyeri.AsgariUcretDestekTutarlari.Count == 0 && !CariYilMi)
                    {
                        mesajlar.Add(String.Format("{0} - {1} işyerinin asgari ücret destek tutarları çekilmemiş.", hesaplanacakIsyeri.Sirketler.SirketAdi, hesaplanacakIsyeri.SubeAdi));
                    }

                    if (!FaraziHesapla && Program.BasvuruYoksaTesvikVerilmesin && hesaplanacakIsyeri.BasvuruDonemleri.Count == 0 && !CariYilMi)
                    {
                        mesajlar.Add(String.Format("{0} - {1} işyerinin başvuru dönemleri çekilmemiş.", hesaplanacakIsyeri.Sirketler.SirketAdi, hesaplanacakIsyeri.SubeAdi));
                    }

                    if (!CariYilMi && hesaplanacakIsyeri.AylikCalisanSayilari.Count == 0)
                    {
                        mesajlar.Add(String.Format("{0} - {1} işyerinin aylık çalışan sayıları çekilmemiş.", hesaplanacakIsyeri.Sirketler.SirketAdi, hesaplanacakIsyeri.SubeAdi));
                    }


                }

                if (mesajlar.Count > 0)
                {
                    DevamEdilsinMi = new frmOnay(String.Join(Environment.NewLine + Environment.NewLine, mesajlar)).ShowDialog() == DialogResult.Yes;
                }



                if (DevamEdilsinMi)
                {
                    mesajlar.Clear();

                    foreach (var hesaplanacakIsyeri in hesaplanacakIsyerleri)
                    {
                        var muhtasarIsyeri = responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.FirstOrDefault(p => p.Isyeri.IsyeriID.Equals(hesaplanacakIsyeri.IsyeriID));

                        var aphbYol = muhtasarIsyeri != null ? muhtasarIsyeri.Aphb : ayliklisteyolu;
                        var basvuruFormuYol = muhtasarIsyeri != null ? muhtasarIsyeri.BasvuruFormu : basvurulisteyolu;

                        if (string.IsNullOrEmpty(basvuruFormuYol)) basvuruFormuYol = Path.Combine(Application.StartupPath, "BasvuruTemplate.xlsx");
                        DataSet dsbasvurulistesi = Metodlar.BasvuruListesiniYukle(basvuruFormuYol);

                        aphbYol = string.IsNullOrEmpty(aphbYol) ? Path.Combine(Application.StartupPath, "ListeTemplate.xlsx") : aphbYol;
                        DataTable dtaylikliste = Metodlar.AylikListeyiYukle(aphbYol);

                        AphbListeleri.Add(hesaplanacakIsyeri.IsyeriID, dtaylikliste);
                        BasvuruFormuListeleri.Add(hesaplanacakIsyeri.IsyeriID, dsbasvurulistesi);

                        //var isyeriTumKisiler = Metodlar.TumKisileriGetir(dtaylikliste,
                        //    formBildirgeOlustur.txtYil.Text.Trim(),
                        //    formBildirgeOlustur.txtAy.Text.Trim(),
                        //    formBildirgeOlustur.txtYilBitis.Text.Trim(),
                        //    formBildirgeOlustur.txtAyBitis.Text.Trim());

                        //TumKisilerListeleri.Add(hesaplanacakIsyeri.IsyeriID, isyeriTumKisiler);

                        Dictionary<string, DataTable> BasvuruFormlari = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => dsbasvurulistesi.Tables.IndexOf(x) > -1 ? dsbasvurulistesi.Tables[x] : null);

                        BasvuruFormlariSutunlari = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => dsbasvurulistesi.Tables.IndexOf(x) > -1 ? (x.Equals("6111") ? (dsbasvurulistesi.Tables[x].Columns.Contains("İşten Ayrılış Tarihi") ? Sabitler.BasvuruFormlariSutunlari["6111-v2"] : Sabitler.BasvuruFormlariSutunlari["6111-v1"]) : Sabitler.BasvuruFormlariSutunlari[x]) : new Dictionary<Enums.BasvuruFormuSutunTurleri, int>());

                        var dtbasvurulistesi = BasvuruFormlari["6111"];

                        if (dtbasvurulistesi != null && dtbasvurulistesi.AsEnumerable().Any(p => p[BasvuruFormlariSutunlari["6111"][Enums.BasvuruFormuSutunTurleri.OnayDurumu]].ToString().Equals("ONAYSIZ")))
                        {
                            mesajlar.Add(String.Format("{0} - {1} işyerinin onaysız 6111 başvuru kaydı var", hesaplanacakIsyeri.Sirketler.SirketAdi, hesaplanacakIsyeri.SubeAdi));
                        }

                        /*
                        if (BasvuruFormlari.ContainsKey("17256"))
                        {
                            var dtbasvurulistesi17256 = BasvuruFormlari["17256"];

                            if (dtbasvurulistesi17256 != null && dtbasvurulistesi17256.AsEnumerable().Any(p => string.IsNullOrEmpty(p[BasvuruFormlariSutunlari["17256"][Enums.BasvuruFormuSutunTurleri.VerilsinMi7256]].ToString())))
                            {
                                mesajlar.Add(String.Format("{0} - {1} işyerininin 17256 başvuru formunda teşvik verilip verilmeyeceği belirsiz kişiler var", hesaplanacakIsyeri.Sirketler.SirketAdi, hesaplanacakIsyeri.SubeAdi));
                            }
                        }
                        

                        if (BasvuruFormlari.ContainsKey("27256"))
                        {
                            var dtbasvurulistesi27256 = BasvuruFormlari["27256"];

                            if (dtbasvurulistesi27256 != null && dtbasvurulistesi27256.AsEnumerable().Any(p => string.IsNullOrEmpty(p[BasvuruFormlariSutunlari["27256"][Enums.BasvuruFormuSutunTurleri.VerilsinMi7256]].ToString())))
                            {
                                mesajlar.Add(String.Format("{0} - {1} işyerininin 27256 başvuru formunda teşvik verilip verilmeyeceği belirsiz kişiler var", hesaplanacakIsyeri.Sirketler.SirketAdi, hesaplanacakIsyeri.SubeAdi));
                            }
                        }
                        */

                        if (dtaylikliste != null)
                        {
                            var kanunNosuzKisiler = dtaylikliste.AsEnumerable().Where(p => p[(int)Enums.AphbHucreBilgileri.OnayDurumu].ToString().Equals("Onaylanmamış") && string.IsNullOrEmpty(p[(int)Enums.AphbHucreBilgileri.Kanun].ToString()) && TesvikHesaplamaSabitleri.DestekKapsaminaGirmeyenBelgeTurleri.Contains(p[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString()) == false);

                            foreach (var row in kanunNosuzKisiler)
                            {
                                var tc = row[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString();

                                bool Kisinin7252Varmi = dtaylikliste.AsEnumerable().Any(p => p[(int)Enums.AphbHucreBilgileri.OnayDurumu].ToString().Equals("Onaylanmamış") && p[(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString().Equals(tc) && p[(int)Enums.AphbHucreBilgileri.Kanun].ToString().EndsWith("7252") && TesvikHesaplamaSabitleri.DestekKapsaminaGirmeyenBelgeTurleri.Contains(p[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString()) == false);

                                if (!Kisinin7252Varmi)
                                {
                                    mesajlar.Add(String.Format("{0} - {1} işyerinin onaysız bildirgelerinde kanun nosuz verilen bildirge var", hesaplanacakIsyeri.Sirketler.SirketAdi, hesaplanacakIsyeri.SubeAdi));

                                    break;
                                }
                            }

                            var hepsiTaseronMu = dtaylikliste.Rows.Count > 0 && !dtaylikliste.AsEnumerable().Any(p => p[(int)Enums.AphbHucreBilgileri.Araci].ToString().Equals("Ana İşveren"));

                            if (hepsiTaseronMu)
                            {
                                mesajlar.Add(String.Format("{0} - {1} işyerinin APHB bildirgelerinin tamamı taşeron", hesaplanacakIsyeri.Sirketler.SirketAdi, hesaplanacakIsyeri.SubeAdi));
                            }

                        }

                        if (muhtasarIsyeri != null)
                        {
                            var kanunNosuzKisiler = muhtasarIsyeri.kisiler.Where(p => string.IsNullOrEmpty(p.Kanun) && TesvikHesaplamaSabitleri.DestekKapsaminaGirmeyenBelgeTurleri.Contains(p.BelgeTuru) == false);

                            foreach (var satir in kanunNosuzKisiler)
                            {

                                bool Kisinin7252Varmi = muhtasarIsyeri.kisiler.Any(p => p.Kanun.EndsWith("7252") && p.SosyalGuvenlikNo.Equals(satir.SosyalGuvenlikNo) && TesvikHesaplamaSabitleri.DestekKapsaminaGirmeyenBelgeTurleri.Contains(p.BelgeTuru) == false);

                                if (!Kisinin7252Varmi)
                                {
                                    mesajlar.Add(String.Format("{0} - {1} işyerinin muhtasar bildirgelerinde kanun nosuz verilen bildirge var", hesaplanacakIsyeri.Sirketler.SirketAdi, hesaplanacakIsyeri.SubeAdi));

                                    break;
                                }
                            }

                        }
                    }

                    if (mesajlar.Count > 0)
                    {
                        DevamEdilsinMi = new frmOnay(String.Join(Environment.NewLine + Environment.NewLine, mesajlar)).ShowDialog() == DialogResult.Yes;
                    }
                }

                if (DevamEdilsinMi)
                {
                    Excel2.Application MyApp = null;

                    Excel2.Workbook BildirgeWorkBook = null;

                    int excelprocessid = 0;

                    Microsoft.Office.Interop.Word.Application wordApp = null;

                    int wordprocessid = 0;

                    List<string> hatalar = new List<string>();

                    List<string> basariliolanlar = new List<string>();

                    //var MuhtasarGenelXmlOlusturulacak = false;

                    var hesaplananIsyeriSira = 0;

                    SirketCari14857ListesindeVarMi = Metodlar.SirketCari14857IcindeVarMi(Isyeri.SirketID);

                    if (responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.Count > 0)
                    {
                        var muh = responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.FirstOrDefault();
                        muhtasarYil = muh.Yil;
                        muhtasarAy = muh.Ay;
                    }

                    var tumIsyerleriIcmaller = hesaplanacakIsyerleri.ToDictionary(x => x, x => TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(p => p, p => 0m));

                    foreach (var item in tumIsyerleriIcmaller)
                    {
                        item.Value.Add("Tumu", 0m);
                    }

                    bool hataOlustu = false;
                    bool muhtasarda26322_5510a_donusturulsun_mu = false;
                    bool muhtasarda26322_5510a_donusturulmesi_soruldu = false;
                    bool muhtasarda_kanun_nosuzdan_5510a_donusturulsun_mu = false;
                    bool muhtasarda_kanun_nosuzdan_5510a_donusturulmesi_soruldu = false;


                    foreach (var AktifIsyeri in hesaplanacakIsyerleri)
                    {
                        hesaplananIsyeriSira++;

                        isyeriSavePath = string.IsNullOrEmpty(cariAphbYolu) ? Path.Combine(Application.StartupPath, "output") : Path.Combine(Application.StartupPath, "output", AktifIsyeri.SubeAdi);

                        var muhtasarIsyeri = responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.FirstOrDefault(p => p.Isyeri.IsyeriID.Equals(AktifIsyeri.IsyeriID));

                        var dsbasvurulistesi = BasvuruFormuListeleri[AktifIsyeri.IsyeriID];

                        var dtaylikliste = AphbListeleri[AktifIsyeri.IsyeriID];

                        var dtbasvurulistesi = dsbasvurulistesi.Tables["6111"];

                        AyCalisanSayilari = new Dictionary<DateTime, Dictionary<string, long>>();

                        AyCalisanSayilariBazHesaplama = new Dictionary<DateTime, Dictionary<string, long>>();

                        BasvuruListesiBazGruplari = new Dictionary<string, Dictionary<DateTime, List<string>>>();

                        BasvuruListesindeOlanKisiler = new List<Classes.Kisi>();

                        BasvuruListesindeOlanKisiler7166 = new List<BasvuruListesi7166Kisi>();

                        PasifOlanlar6645 = new List<DataRow>();

                        dthatalisatirlar = null;

                        BasvuruFormlariHataliSatirlar.Clear();

                        TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ForEach(p => BasvuruFormlariHataliSatirlar.Add(p, null));

                        TumTesvikler = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new Tesvik(x));

                        IlkSayfaHucreleri = new Dictionary<string, Excel2.Range>();

                        DevamSayfaHucreleri = new Dictionary<string, Excel2.Range>();

                        HafizadanAtilacaklar = new List<object>();

                        AsgariUcretDestegiIcmalleri = new Dictionary<DateTime, decimal>();

                        CariAy = DateTime.MinValue;

                        CariAphbOtomatikOlusturuldu = false;

                        Liste14857 = new HashSet<DataRow>();

                        EksikGunuKodundanDolayiUyariVerilenKisiler = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new Dictionary<string, bool>());

                        Dictionary<string, DataTable> BasvuruFormlari = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => dsbasvurulistesi.Tables.IndexOf(x) > -1 ? dsbasvurulistesi.Tables[x] : null);

                        BasvuruFormlariSutunlari = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => dsbasvurulistesi.Tables.IndexOf(x) > -1 ? (x.Equals("6111") ? (dsbasvurulistesi.Tables[x].Columns.Contains("İşten Ayrılış Tarihi") ? Sabitler.BasvuruFormlariSutunlari["6111-v2"] : Sabitler.BasvuruFormlariSutunlari["6111-v1"]) : Sabitler.BasvuruFormlariSutunlari[x]) : new Dictionary<Enums.BasvuruFormuSutunTurleri, int>());

                        if (BasvuruFormlari["7103"] != null)
                        {
                            DataTable dt7166 = BasvuruFormlari["7103"].Clone();

                            var rows = BasvuruFormlari["7103"].AsEnumerable().Where(p => p[BasvuruFormlariSutunlari["7103"][Enums.BasvuruFormuSutunTurleri.UcretDestegiTercihi7103]].ToString().Trim().Equals("İSTİYOR")).ToList();

                            foreach (var row in rows)
                            {
                                var newRow = dt7166.NewRow();

                                for (int i = 0; i < dt7166.Columns.Count; i++)
                                {
                                    newRow[i] = row[i];
                                }

                                dt7166.Rows.Add(newRow);
                            }

                            BasvuruFormlari["7166"] = dt7166;
                        }

                        if (BasvuruFormlari["7166"] != null) BasvuruFormlariSutunlari["7166"] = Sabitler.BasvuruFormlariSutunlari["7166"];

                        SatirReferanslari = new Dictionary<DataRow, XElement>();

                        SatirReferanslariNetsis = new Dictionary<DataRow, NetsisSatir>();

                        SatirReferanslariNetsisExcel = new Dictionary<DataRow, NetsisSatir>();

                        xmller = new Dictionary<string, XDocument>();
                        netsisBildirgeler = new Dictionary<string, List<string[]>>();
                        netsisBildirgelerExcel = new Dictionary<string, List<string[]>>();

                        if (CariYilMi)
                        {
                            DataTable dtCari = null;

                            if (!string.IsNullOrEmpty(cariAphbYolu))
                            {

                                if (muhtasarIsyeri != null)
                                {
                                    formBildirgeOlustur.Cursor = Cursors.WaitCursor;

                                    var muhtasardaVerilecek6486 = "";

                                    Metodlar.MuhtasarXmldenCariOlustur(ref muhtasarIsyeri, ref dtaylikliste, BasvuruFormlari["14857"], out muhtasardaVerilecek6486, ref muhtasarda26322_5510a_donusturulsun_mu, ref muhtasarda26322_5510a_donusturulmesi_soruldu, ref muhtasarda_kanun_nosuzdan_5510a_donusturulsun_mu, ref muhtasarda_kanun_nosuzdan_5510a_donusturulmesi_soruldu);

                                    if (!string.IsNullOrEmpty(muhtasardaVerilecek6486)) MuhtasardaVerilecek6486 = muhtasardaVerilecek6486;

                                    formBildirgeOlustur.Cursor = Cursors.Default;

                                    SatirReferanslari = muhtasarIsyeri.SatirReferanslari;
                                    SatirReferanslariNetsis = muhtasarIsyeri.SatirReferanslariNetsis;
                                    SatirReferanslariNetsisExcel = muhtasarIsyeri.SatirReferanslariNetsisExcel;
                                    xmller = muhtasarIsyeri.xmller;
                                    netsisBildirgeler = muhtasarIsyeri.netsisBildirgeler;
                                    netsisBildirgelerExcel = muhtasarIsyeri.netsisBildirgelerExcel;
                                    muhtasarTarih = new DateTime(muhtasarIsyeri.Yil, muhtasarIsyeri.Ay, 1);
                                }

                            }
                            else if (Program.CariAphbOlusturulsun)
                            {
                                formBildirgeOlustur.Cursor = Cursors.WaitCursor;

                                var sonuc = Metodlar.CariDonemKisileriAPHByeEkle(AktifIsyeri, ref dtaylikliste, out dtCari);

                                formBildirgeOlustur.Cursor = Cursors.Default;

                                if (sonuc != "OK" && !sonuc.Equals("Cari aya ait onaylı bildirgeler indirildiği için Cari Aphb oluşturulmayacak"))
                                {
                                    DevamEdilsinMi = MessageBox.Show(sonuc, "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                                }
                                else
                                {
                                    if (dtCari.Rows.Count > 0) CariAphbOtomatikOlusturuldu = true;
                                }
                            }
                        }

                        string plakakodu = AktifIsyeri.IsyeriSicilNo.Substring(16, 3);

                        foreach (var tesvikItem in TumTesvikler)
                        {
                            var tesvik = tesvikItem.Value;

                            if (tesvik.AltKanunPlakaKodunaGoreBelirlenecek)
                            {
                                tesvik.AltKanun = tesvik.AltKanunPlakaKodlari.FirstOrDefault(p => p.Value.Contains(plakakodu)).Key;
                            }
                        }

                        DateTime enbuyukay = DateTime.MinValue;

                        //TumKisilerSonuc = TumKisilerListeleri[AktifIsyeri.IsyeriID];

                        TumKisilerSonuc = Metodlar.TumKisileriGetir(dtaylikliste,
                            formBildirgeOlustur.txtYil.Text.Trim(),
                            formBildirgeOlustur.txtAy.Text.Trim(),
                            formBildirgeOlustur.txtYilBitis.Text.Trim(),
                            formBildirgeOlustur.txtAyBitis.Text.Trim());

                        var TesvikVerilenler = TumKisilerSonuc.TesvikVerilenler;

                        var yilveaylar = TumKisilerSonuc.yilveaylar;

                        var tumyilveaylar = TumKisilerSonuc.tumyilveaylar;

                        enbuyukay = TumKisilerSonuc.enbuyukay;

                        CariAy = enbuyukay;

                        string IsyeriAd = AktifIsyeri.Sirketler.SirketAdi;

                        string IsyeriVergiNo = AktifIsyeri.Sirketler.VergiKimlikNo;

                        string IsyeriSicilNo = AktifIsyeri.IsyeriSicilNo;

                        string IsyeriSosyalGuvenlikKurumu = AktifIsyeri.SosyalGuvenlikKurumu;

                        bool basvurudevam = true;

                        #region Başvuru Formları Kontrolü
                        if (formBildirgeOlustur.chkBasvuruformuKontrolu.Checked)
                        {
                            Metodlar.HataliSatirlarKontroluYap(AktifIsyeri, null, dsbasvurulistesi, false, true, ref dthatalisatirlar, ref BasvuruFormlariHataliSatirlar);

                            Dictionary<KeyValuePair<int, int>, int> bazlistesi = new Dictionary<KeyValuePair<int, int>, int>();

                            List<KeyValuePair<int, int>> hatalibazaylar = new List<KeyValuePair<int, int>>();

                            List<string> hataligiristarihi = new List<string>();

                            List<string> giristarihieskiolanlar = new List<string>();

                            List<string> ArdardaGirisveyaCikisiOlanlar = new List<string>();

                            List<string> Yas18denOnceBirdenFazlaGirisiOlanlar = new List<string>();

                            List<string> basvurulistesindeolmayanlar = new List<string>();

                            List<string> HataliTesvikDuzeltilecekler = new List<string>();

                            List<string> HataliIseBaslamaTarihleriDuzeltilecekler = new List<string>();

                            List<DataRow> DuzeltilecekSatirlar = new List<DataRow>();

                            List<string> BirdenFazla18OncesiDuzeltilecekler = new List<string>();

                            Dictionary<KeyValuePair<int, int>, List<string>> bazihataliolanlar = new Dictionary<KeyValuePair<int, int>, List<string>>();

                            bool DuzeltilecekKayitVar = false;

                            int sayac = 1;

                            do
                            {
                                basvurudevam = true;

                                DuzeltilecekKayitVar = false;

                                bazlistesi = new Dictionary<KeyValuePair<int, int>, int>();

                                hatalibazaylar = new List<KeyValuePair<int, int>>();

                                hataligiristarihi = new List<string>();

                                giristarihieskiolanlar = new List<string>();

                                ArdardaGirisveyaCikisiOlanlar = new List<string>();

                                Yas18denOnceBirdenFazlaGirisiOlanlar = new List<string>();

                                basvurulistesindeolmayanlar = new List<string>();

                                HataliTesvikDuzeltilecekler = new List<string>();

                                HataliIseBaslamaTarihleriDuzeltilecekler = new List<string>();

                                BirdenFazla18OncesiDuzeltilecekler = new List<string>();

                                DuzeltilecekSatirlar = new List<DataRow>();

                                bazihataliolanlar = new Dictionary<KeyValuePair<int, int>, List<string>>();

                                if (BasvuruFormlariHataliSatirlar["6111"] == null)
                                {

                                    bool v2_6111mi = dtbasvurulistesi.Columns.Contains("İşten Ayrılış Tarihi");

                                    var sutunlar6111 = v2_6111mi ? Sabitler.BasvuruFormlariSutunlari["6111-v2"] : Sabitler.BasvuruFormlariSutunlari["6111-v1"];

                                    for (int j = 0; j < dtbasvurulistesi.Rows.Count; j++)
                                    {
                                        try
                                        {
                                            string tcno = dtbasvurulistesi.Rows[j][sutunlar6111[Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString().Trim();

                                            string adsoyad = dtbasvurulistesi.Rows[j][sutunlar6111[Enums.BasvuruFormuSutunTurleri.Ad]].ToString().Trim() + " " + dtbasvurulistesi.Rows[j][sutunlar6111[Enums.BasvuruFormuSutunTurleri.Soyad]].ToString().Trim();

                                            DateTime giristarihi = Convert.ToDateTime(dtbasvurulistesi.Rows[j][sutunlar6111[Enums.BasvuruFormuSutunTurleri.Giris]].ToString());

                                            int baz = Convert.ToInt32(dtbasvurulistesi.Rows[j][sutunlar6111[Enums.BasvuruFormuSutunTurleri.Baz]].ToString().Replace(".", ""));

                                            string tesviksuresi = null;

                                            if (!v2_6111mi)
                                            {
                                                tesviksuresi = dtbasvurulistesi.Rows[j][sutunlar6111[Enums.BasvuruFormuSutunTurleri.TesvikSuresi6111v1]].ToString();
                                            }
                                            else
                                            {
                                                tesviksuresi = dtbasvurulistesi.Rows[j][sutunlar6111[Enums.BasvuruFormuSutunTurleri.TesvikBaslangic]].ToString() + "-" + dtbasvurulistesi.Rows[j][sutunlar6111[Enums.BasvuruFormuSutunTurleri.TesvikBitis]].ToString();
                                            }

                                            DateTime tesvikbaslangic = Convert.ToDateTime(tesviksuresi.Split('-')[0].Trim());

                                            DateTime tesvikbitis = Convert.ToDateTime(tesviksuresi.Split('-')[1].Trim());

                                            if (v2_6111mi)
                                            {

                                                string cikistarihi = dtbasvurulistesi.Rows[j][sutunlar6111[Enums.BasvuruFormuSutunTurleri.Cikis]].ToString();

                                                if (!String.IsNullOrEmpty(cikistarihi))
                                                {
                                                    DateTime ctarih = Convert.ToDateTime(cikistarihi);
                                                }
                                            }


                                            if (formBildirgeOlustur.chkBasvuruformuKontrolu.Checked)
                                            {

                                                KeyValuePair<int, int> yearmonth = new KeyValuePair<int, int>(giristarihi.Year, giristarihi.Month);

                                                if (bazlistesi.ContainsKey(yearmonth))
                                                {
                                                    int tempbaz = bazlistesi[yearmonth];

                                                    if (tempbaz != baz)
                                                    {
                                                        if (!hatalibazaylar.Contains(yearmonth)) hatalibazaylar.Add(yearmonth);
                                                    }
                                                }
                                                else
                                                {
                                                    bazlistesi.Add(yearmonth, baz);
                                                }

                                                if (TesvikVerilenler.ContainsKey("6111") && TesvikVerilenler["6111"].Contains(tcno))
                                                {
                                                    TesvikVerilenler["6111"].Remove(tcno);
                                                }

                                                if (TumKisilerSonuc.TumKisiler != null)
                                                {
                                                    Classes.Kisi kisi = TumKisilerSonuc.TumKisiler.ContainsKey(tcno) ? TumKisilerSonuc.TumKisiler[tcno] : null;

                                                    if (kisi != null)
                                                    {
                                                        bool hataligiris = true;

                                                        bool b = false;

                                                        bool IseGirisAyindaDahaBuyukIseGirisTarihiVar = false;

                                                        List<DateTime> yas18denOnceGirisler = new List<DateTime>();

                                                        List<DateTime> tesvikbaslamaGirisleri = new List<DateTime>();

                                                        List<DateTime> IseGirisBaslamaTarihleri = new List<DateTime>();

                                                        foreach (var tarihler in kisi.GirisTarihleri)
                                                        {
                                                            DateTime gt = tarihler.Tarih;

                                                            if (gt.Year == giristarihi.Year && gt.Month == giristarihi.Month)
                                                            {
                                                                b = true;

                                                                IseGirisBaslamaTarihleri.Add(gt);

                                                                if (gt.Day == giristarihi.Day) hataligiris = false;
                                                                else
                                                                {
                                                                    if (gt.Day > giristarihi.Day) IseGirisAyindaDahaBuyukIseGirisTarihiVar = true;
                                                                }
                                                            }


                                                            if (tesvikbaslangic.Year == gt.Year && tesvikbaslangic.Month == gt.Month)
                                                            {
                                                                tesvikbaslamaGirisleri.Add(gt);
                                                            }
                                                        }

                                                        if (b && hataligiris)
                                                        {
                                                            if (IseGirisBaslamaTarihleri.Count > 1)
                                                            {
                                                                hataligiristarihi.Add((j + 2).ToString() + ". sıradaki " + tcno + " " + adsoyad);
                                                            }
                                                            else if (IseGirisBaslamaTarihleri.Count == 1)
                                                            {

                                                                HataliIseBaslamaTarihleriDuzeltilecekler.Add("Başvuru Formu Giriş Tarihi Farklı: " + (j + 2).ToString() + ". sıradaki " + tcno + " " + adsoyad + "- Eski Giriş Tarihi:" + giristarihi.ToString("dd.MM.yyyy") + " - Yeni Giriş Tarihi:" + IseGirisBaslamaTarihleri[0].ToString("dd.MM.yyyy"));

                                                                DataRow rowupdate = dtbasvurulistesi.NewRow();

                                                                for (int i = 0; i < dtbasvurulistesi.Columns.Count; i++)
                                                                {
                                                                    rowupdate[i] = dtbasvurulistesi.Rows[j][i];
                                                                }


                                                                rowupdate[sutunlar6111[Enums.BasvuruFormuSutunTurleri.Giris]] = IseGirisBaslamaTarihleri[0].ToString("dd.MM.yyyy");

                                                                DuzeltilecekSatirlar.Add(rowupdate);


                                                            }
                                                        }

                                                        if (IseGirisAyindaDahaBuyukIseGirisTarihiVar)
                                                        {
                                                            giristarihieskiolanlar.Add((j + 2).ToString() + ". sıradaki " + tcno + " " + adsoyad);
                                                        }

                                                        List<KeyValuePair<DateTime, string>> lst = new List<KeyValuePair<DateTime, string>>();

                                                        foreach (var item in kisi.GirisTarihleri)
                                                        {
                                                            lst.Add(new KeyValuePair<DateTime, string>(item.Tarih, "Giriş"));
                                                        }


                                                        bool IseGirisEklenmis = false;

                                                        foreach (var item in lst)
                                                        {
                                                            if (item.Key == giristarihi && item.Value == "Giriş")
                                                            {
                                                                IseGirisEklenmis = true;

                                                                break;
                                                            }
                                                        }

                                                        if (!IseGirisEklenmis)
                                                        {
                                                            lst.Add(new KeyValuePair<DateTime, string>(giristarihi, "Giriş"));
                                                        }

                                                        foreach (var item in kisi.CikisTarihleri)
                                                        {
                                                            bool AyniGirisTarihiVar = false;

                                                            foreach (var item2 in lst)
                                                            {
                                                                if (item2.Key == item.Tarih && item2.Value == "Giriş")
                                                                {
                                                                    AyniGirisTarihiVar = true;

                                                                    break;
                                                                }
                                                            }

                                                            if (AyniGirisTarihiVar)
                                                            {
                                                                lst.Add(new KeyValuePair<DateTime, string>(item.Tarih.AddHours(1), "Çıkış"));
                                                            }
                                                            else lst.Add(new KeyValuePair<DateTime, string>(item.Tarih, "Çıkış"));
                                                        }

                                                        lst.Sort((x, y) => x.Key.CompareTo(y.Key));


                                                        List<KeyValuePair<DateTime, DateTime>> calismadonemleri = new List<KeyValuePair<DateTime, DateTime>>();

                                                        DateTime CalismaBaslangic = giristarihi;

                                                        DateTime CalismaBitis = DateTime.MaxValue;

                                                        foreach (var giristarihleri in kisi.GirisTarihleri)
                                                        {
                                                            DateTime dtgiris = giristarihleri.Tarih;

                                                            DateTime enyakincikis = DateTime.MaxValue;

                                                            foreach (var cikistarihleri in kisi.CikisTarihleri)
                                                            {
                                                                DateTime dtcikis = cikistarihleri.Tarih;

                                                                if (dtcikis >= dtgiris)
                                                                {
                                                                    if (dtcikis < enyakincikis) enyakincikis = dtcikis;
                                                                }
                                                            }

                                                            if (giristarihi >= dtgiris && giristarihi <= enyakincikis)
                                                            {
                                                                CalismaBaslangic = giristarihi;

                                                                CalismaBitis = enyakincikis;
                                                            }
                                                        }

                                                        bool ArdardaGirisveyaCikisVar = false;

                                                        for (int i = 0; i < lst.Count - 1; i++)
                                                        {
                                                            if (lst[i].Value == lst[i + 1].Value)
                                                            {
                                                                if ((lst[i].Key >= tesvikbaslangic && lst[i].Key <= tesvikbitis) || (lst[i + 1].Key >= tesvikbaslangic && lst[i + 1].Key <= tesvikbitis))
                                                                {
                                                                    if ((lst[i].Key > CalismaBaslangic && lst[i].Key < CalismaBitis) || (lst[i + 1].Key > CalismaBaslangic && lst[i + 1].Key < CalismaBitis))
                                                                    {
                                                                        ArdardaGirisveyaCikisVar = true;

                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        if (ArdardaGirisveyaCikisVar)
                                                        {
                                                            ArdardaGirisveyaCikisiOlanlar.Add((j + 2).ToString() + ". sıradaki " + tcno + " " + adsoyad);
                                                        }

                                                        if (yas18denOnceGirisler.Count > 1)
                                                        {
                                                            Yas18denOnceBirdenFazlaGirisiOlanlar.Add((j + 2).ToString() + ". sıradaki " + tcno + " " + adsoyad);

                                                            Dictionary<DateTime, Dictionary<string, int>> ayniaykisiler = new Dictionary<DateTime, System.Collections.Generic.Dictionary<string, int>>();

                                                            bool HataVar = false;

                                                            foreach (var item in yas18denOnceGirisler)
                                                            {
                                                                foreach (DataRow rowbasvurubaz in dtbasvurulistesi.Rows)
                                                                {
                                                                    try
                                                                    {
                                                                        DateTime isegiristarihi = Convert.ToDateTime(rowbasvurubaz[sutunlar6111[Enums.BasvuruFormuSutunTurleri.Giris]]);

                                                                        string baztcno = rowbasvurubaz[sutunlar6111[Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString();

                                                                        int ayniaybaz = Convert.ToInt32(rowbasvurubaz[sutunlar6111[Enums.BasvuruFormuSutunTurleri.Baz].ToString().Replace(".", "")]);

                                                                        DateTime dtbaz = new DateTime(item.Year, item.Month, 1);

                                                                        if (item.Year == isegiristarihi.Year && item.Month == isegiristarihi.Month)
                                                                        {
                                                                            if (!ayniaykisiler.ContainsKey(dtbaz)) ayniaykisiler.Add(dtbaz, new Dictionary<string, int>());

                                                                            if (!ayniaykisiler[dtbaz].ContainsKey(baztcno)) ayniaykisiler[dtbaz].Add(baztcno, ayniaybaz);
                                                                        }

                                                                    }
                                                                    catch
                                                                    {

                                                                        HataVar = true;

                                                                        break;
                                                                    }

                                                                }

                                                                if (HataVar) break;
                                                            }

                                                            if (!HataVar)
                                                            {
                                                                List<long> tumbazlar = new List<long>();

                                                                Dictionary<long, DateTime> bazgunleri = new Dictionary<long, DateTime>();

                                                                bool AyniBazVarIkiFarkliAyda = false;

                                                                foreach (var item in ayniaykisiler)
                                                                {
                                                                    List<int> bazlar = new List<int>();

                                                                    long aybazi = -1;

                                                                    foreach (var item2 in item.Value)
                                                                    {
                                                                        if (!bazlar.Contains(item2.Value))
                                                                        {


                                                                            if (item2.Key == tcno)
                                                                            {
                                                                                bool bulundu = false;

                                                                                foreach (var item3 in ayniaykisiler)
                                                                                {
                                                                                    if (item3.Key == item.Key) continue;

                                                                                    List<int> baztemp = new List<int>();

                                                                                    foreach (var item4 in item3.Value)
                                                                                    {
                                                                                        if (!baztemp.Contains(item4.Value)) baztemp.Add(item4.Value);
                                                                                    }

                                                                                    if (baztemp.Count == 1)
                                                                                    {
                                                                                        if (baz == baztemp[0])
                                                                                        {
                                                                                            bulundu = true;

                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                    else if (baztemp.Count == 0)
                                                                                    {
                                                                                        bool HataVar2 = false;

                                                                                        for (int i = 1; i <= 6; i++)
                                                                                        {
                                                                                            DateTime dttemp = item.Key.AddMonths(-i);

                                                                                            if (!tumyilveaylar.ContainsKey(new KeyValuePair<string, string>(dttemp.Year.ToString(), dttemp.Month.ToString())))
                                                                                            {
                                                                                                HataVar2 = true;

                                                                                                break;
                                                                                            }
                                                                                        }

                                                                                        if (HataVar2) break;
                                                                                        else
                                                                                        {
                                                                                            if (dthatalisatirlar == null
                                                                                                && BasvuruFormlariHataliSatirlar.All(p => p.Value == null))
                                                                                            {
                                                                                                Dictionary<KeyValuePair<string, string>, int> temp2 = new Dictionary<KeyValuePair<string, string>, int>();

                                                                                                List<Classes.Icmal> temp = new List<Classes.Icmal>();

                                                                                                aybazi = Metodlar.BazHesapla(item.Key.Year, item.Key.Month, "6111", TumKisilerSonuc, ref AyCalisanSayilari, ref AyCalisanSayilariBazHesaplama);

                                                                                                if (aybazi == baz)
                                                                                                {
                                                                                                    bulundu = true;

                                                                                                    break;
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                HataVar2 = true;

                                                                                                break;
                                                                                            }
                                                                                        }

                                                                                    }
                                                                                }

                                                                                if (!bulundu)
                                                                                {
                                                                                    bazlar.Add(item2.Value);
                                                                                }
                                                                            }
                                                                            else bazlar.Add(item2.Value);
                                                                        }
                                                                    }


                                                                    if (bazlar.Count > 1)
                                                                    {
                                                                        HataVar = true;

                                                                        break;
                                                                    }
                                                                    else if (bazlar.Count == 0 || (bazlar.Count == 1 && item.Value.Count == 1 && item.Value.ContainsKey(tcno)))
                                                                    {
                                                                        for (int i = 1; i <= 6; i++)
                                                                        {
                                                                            DateTime dttemp = item.Key.AddMonths(-i);

                                                                            if (!tumyilveaylar.ContainsKey(new KeyValuePair<string, string>(dttemp.Year.ToString(), dttemp.Month.ToString())))
                                                                            {
                                                                                HataVar = true;

                                                                                break;
                                                                            }
                                                                        }

                                                                        if (HataVar) break;
                                                                        else
                                                                        {
                                                                            if (dthatalisatirlar == null
                                                                                && BasvuruFormlariHataliSatirlar.All(p => p.Value == null))
                                                                            {
                                                                                Dictionary<KeyValuePair<string, string>, int> temp2 = new Dictionary<KeyValuePair<string, string>, int>();

                                                                                List<Classes.Icmal> temp = new List<Classes.Icmal>();

                                                                                aybazi = Metodlar.BazHesapla(item.Key.Year, item.Key.Month, "6111", TumKisilerSonuc, ref AyCalisanSayilari, ref AyCalisanSayilariBazHesaplama);
                                                                            }
                                                                            else
                                                                            {
                                                                                HataVar = true;

                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    else if (bazlar.Count == 1)
                                                                    {
                                                                        aybazi = bazlar[0];
                                                                    }

                                                                    if (!HataVar)
                                                                    {
                                                                        if (aybazi > -1)
                                                                        {
                                                                            if (tumbazlar.Contains(aybazi))
                                                                            {
                                                                                AyniBazVarIkiFarkliAyda = true;
                                                                            }
                                                                            else
                                                                            {
                                                                                tumbazlar.Add(aybazi);

                                                                                bazgunleri.Add(aybazi, item.Key);
                                                                            }
                                                                        }
                                                                    }
                                                                }

                                                                if (!HataVar && !AyniBazVarIkiFarkliAyda)
                                                                {
                                                                    foreach (var item in bazgunleri)
                                                                    {
                                                                        if (item.Key == baz)
                                                                        {
                                                                            DateTime dogruisegiris = DateTime.MinValue;

                                                                            foreach (var item2 in yas18denOnceGirisler)
                                                                            {
                                                                                if (item2.Year == item.Value.Year && item.Value.Month == item2.Month)
                                                                                {
                                                                                    dogruisegiris = item2;
                                                                                }
                                                                            }

                                                                            if (dogruisegiris != DateTime.MinValue)
                                                                            {

                                                                                Yas18denOnceBirdenFazlaGirisiOlanlar.Remove((j + 2).ToString() + ". sıradaki " + tcno + " " + adsoyad);

                                                                                if (dogruisegiris != giristarihi)
                                                                                {
                                                                                    BirdenFazla18OncesiDuzeltilecekler.Add("18 Yaşından Önce Birden Fazla Girişi Olan: " + (j + 2).ToString() + ". sıradaki " + tcno + " " + adsoyad + "- Eski Giriş Tarihi:" + giristarihi.ToString("dd.MM.yyyy") + " - Yeni Giriş Tarihi:" + dogruisegiris.ToString("dd.MM.yyyy"));

                                                                                    DataRow rowupdate = dtbasvurulistesi.NewRow();

                                                                                    for (int i = 0; i < dtbasvurulistesi.Columns.Count; i++)
                                                                                    {
                                                                                        rowupdate[i] = dtbasvurulistesi.Rows[j][i];
                                                                                    }

                                                                                    rowupdate[sutunlar6111[Enums.BasvuruFormuSutunTurleri.Giris]] = dogruisegiris.ToString("dd.MM.yyyy");

                                                                                    DuzeltilecekSatirlar.Add(rowupdate);
                                                                                }
                                                                            }

                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            if (sayac == 1)
                                            {
                                                var dthatalibasvuru = BasvuruFormlariHataliSatirlar["6111"];

                                                if (dthatalibasvuru == null)
                                                {
                                                    dthatalibasvuru = new DataTable();

                                                    dthatalibasvuru.Columns.Add("ExcelSatirNo", typeof(int));

                                                    foreach (DataColumn col in dtbasvurulistesi.Columns)
                                                    {
                                                        dthatalibasvuru.Columns.Add(col.ColumnName, col.DataType);
                                                    }

                                                    BasvuruFormlariHataliSatirlar["6111"] = dthatalibasvuru;
                                                }

                                                DataRow r = dthatalibasvuru.NewRow();

                                                for (int i = 0; i < dtbasvurulistesi.Columns.Count; i++)
                                                {
                                                    r[i + 1] = dtbasvurulistesi.Rows[j][i];
                                                }

                                                r["ExcelSatirNo"] = j + 2;

                                                dthatalibasvuru.Rows.Add(r);
                                            }
                                        }

                                    }


                                    if (hatalibazaylar.Count > 0)
                                    {
                                        for (int j = 0; j < dtbasvurulistesi.Rows.Count; j++)
                                        {

                                            string tcno = dtbasvurulistesi.Rows[j][sutunlar6111[Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString();

                                            string adsoyad = dtbasvurulistesi.Rows[j][sutunlar6111[Enums.BasvuruFormuSutunTurleri.Ad]].ToString().Trim() + " " + dtbasvurulistesi.Rows[j][sutunlar6111[Enums.BasvuruFormuSutunTurleri.Soyad]].ToString().Trim();

                                            DateTime giristarihi = Convert.ToDateTime(dtbasvurulistesi.Rows[j][sutunlar6111[Enums.BasvuruFormuSutunTurleri.Giris]].ToString());

                                            KeyValuePair<int, int> yearmonth = new KeyValuePair<int, int>(giristarihi.Year, giristarihi.Month);

                                            int baz = Convert.ToInt32(dtbasvurulistesi.Rows[j][sutunlar6111[Enums.BasvuruFormuSutunTurleri.Baz]].ToString().Replace(".", ""));

                                            if (hatalibazaylar.Contains(yearmonth))
                                            {
                                                if (!bazihataliolanlar.ContainsKey(yearmonth)) bazihataliolanlar.Add(yearmonth, new List<string>());

                                                bazihataliolanlar[yearmonth].Add((j + 2).ToString() + ".sıradaki " + tcno + " " + adsoyad.ToString() + " Giriş Tarihi:" + giristarihi.Month.ToString() + "/" + giristarihi.Year.ToString() + " Baz:" + baz);
                                            }
                                        }
                                    }

                                }

                                basvurudevam = true;

                                if (bazihataliolanlar.Count > 0
                                    || hataligiristarihi.Count > 0
                                    || giristarihieskiolanlar.Count > 0
                                    || ArdardaGirisveyaCikisiOlanlar.Count > 0
                                    || Yas18denOnceBirdenFazlaGirisiOlanlar.Count > 0
                                    || HataliTesvikDuzeltilecekler.Count > 0
                                    || HataliIseBaslamaTarihleriDuzeltilecekler.Count > 0
                                    )
                                {
                                    string hatamesaji = "";

                                    if (bazihataliolanlar.Count > 0)
                                    {
                                        hatamesaji += Environment.NewLine + "Bazı Hatalı Olanlar:" + Environment.NewLine + Environment.NewLine;

                                        var enumerator = bazihataliolanlar.GetEnumerator();

                                        while (enumerator.MoveNext())
                                        {

                                            hatamesaji += String.Join(Environment.NewLine, enumerator.Current.Value.ToArray());

                                            hatamesaji += Environment.NewLine;
                                        }
                                    }

                                    if (hataligiristarihi.Count > 0)
                                    {
                                        hatamesaji += Environment.NewLine + "Başvuru Giriş Tarihi Farklı Olanlar:" + Environment.NewLine + Environment.NewLine;

                                        hatamesaji += String.Join(Environment.NewLine, hataligiristarihi.ToArray());

                                        hatamesaji += Environment.NewLine;
                                    }

                                    if (giristarihieskiolanlar.Count > 0)
                                    {
                                        hatamesaji += Environment.NewLine + "İşe Giriş Tarihi Aynı Ay İçinde Eski Olanlar:" + Environment.NewLine + Environment.NewLine;

                                        hatamesaji += String.Join(Environment.NewLine, giristarihieskiolanlar.ToArray());

                                        hatamesaji += Environment.NewLine;
                                    }

                                    if (ArdardaGirisveyaCikisiOlanlar.Count > 0)
                                    {
                                        hatamesaji += Environment.NewLine + "Ardarda Girişi veya Çıkışı Bulunanlar" + Environment.NewLine + Environment.NewLine;

                                        hatamesaji += String.Join(Environment.NewLine, ArdardaGirisveyaCikisiOlanlar.ToArray());

                                        hatamesaji += Environment.NewLine;
                                    }

                                    if (Yas18denOnceBirdenFazlaGirisiOlanlar.Count > 0)
                                    {
                                        hatamesaji += Environment.NewLine + "18 Yaşından Önce Birden Fazla Girişi Olanlar:" + Environment.NewLine + Environment.NewLine;

                                        hatamesaji += String.Join(Environment.NewLine, Yas18denOnceBirdenFazlaGirisiOlanlar.ToArray());

                                        hatamesaji += Environment.NewLine;
                                    }

                                    if (HataliIseBaslamaTarihleriDuzeltilecekler.Count > 0 || HataliTesvikDuzeltilecekler.Count > 0 || BirdenFazla18OncesiDuzeltilecekler.Count > 0)
                                    {
                                        DuzeltilecekKayitVar = true;

                                        hatamesaji += Environment.NewLine + "DÜZELTİLEBİLECEKLER:" + Environment.NewLine + Environment.NewLine;

                                        if (HataliTesvikDuzeltilecekler.Count > 0)
                                        {
                                            hatamesaji += String.Join(Environment.NewLine, HataliTesvikDuzeltilecekler.ToArray());
                                            hatamesaji += Environment.NewLine;
                                        }

                                        if (HataliIseBaslamaTarihleriDuzeltilecekler.Count > 0)
                                        {
                                            hatamesaji += String.Join(Environment.NewLine, HataliIseBaslamaTarihleriDuzeltilecekler.ToArray());

                                            hatamesaji += Environment.NewLine;
                                        }

                                        if (BirdenFazla18OncesiDuzeltilecekler.Count > 0)
                                        {
                                            hatamesaji += String.Join(Environment.NewLine, BirdenFazla18OncesiDuzeltilecekler.ToArray());

                                            hatamesaji += Environment.NewLine;
                                        }
                                    }


                                    foreach (var tesvikItem in TumTesvikler)
                                    {
                                        var kanun = tesvikItem.Key;

                                        var tesvik = tesvikItem.Value;

                                        if (tesvik.TesvikAlipBasvuruFormundaOlmayanKisilerKontrolEdilecek)
                                        {
                                            if (TesvikVerilenler.ContainsKey(kanun) && TesvikVerilenler[kanun].Count > 0)
                                            {
                                                hatamesaji += Environment.NewLine + "Başvuru Listesinde Bulunmayan " + kanun + " Teşviği Verilenler:" + Environment.NewLine + Environment.NewLine;

                                                hatamesaji += String.Join(Environment.NewLine, TesvikVerilenler[kanun].ToArray());

                                                hatamesaji += Environment.NewLine;
                                            }
                                        }
                                    }

                                    DialogResult dr = new frmMesaj(hatamesaji).ShowDialog();

                                    basvurudevam = dr == DialogResult.Retry || dr == DialogResult.Yes;

                                    if (dr == DialogResult.Yes)
                                    {
                                        foreach (var rowupdated in DuzeltilecekSatirlar)
                                        {
                                            foreach (DataRow rowupdate in dtbasvurulistesi.Rows)
                                            {
                                                if (rowupdate[Sabitler.BasvuruFormlariSutunlari["6111-v2"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString() == rowupdated[Sabitler.BasvuruFormlariSutunlari["6111-v2"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString())
                                                {
                                                    for (int i = 0; i < dtbasvurulistesi.Columns.Count; i++)
                                                    {
                                                        rowupdate[i] = rowupdated[i];
                                                    }

                                                    break;
                                                }
                                            }

                                        }

                                        if (Metodlar.FormKaydet(AktifIsyeri, dtbasvurulistesi, null, Enums.FormTuru.BasvuruFormu, "6111") != null)
                                        {
                                            MessageBox.Show("Başvuru formu başarılı bir şekilde güncellendi");
                                        }
                                        else MessageBox.Show("Başvuru formu güncellenirken hata meydana geldi. Dosya kullanımda olabilir.");

                                    }
                                    else
                                    {
                                        DuzeltilecekKayitVar = false;
                                    }
                                }


                                sayac++;
                            }
                            while (DuzeltilecekKayitVar);

                        }
                        #endregion

                        if (basvurudevam)
                        {
                            DateTime dtkurulustarihi = DateTime.MaxValue;

                            foreach (var item in tumyilveaylar)
                            {
                                DateTime dttemp = new DateTime(Convert.ToInt32(item.Key.Key), Convert.ToInt32(item.Key.Value), 1);

                                if (dttemp < dtkurulustarihi)
                                {
                                    dtkurulustarihi = dttemp;
                                }

                            }

                            var bildirgeOlanYillar = tumyilveaylar.Select(p => Convert.ToInt32(p.Key.Key)).Distinct().ToList();

                            foreach (var item in TumTesvikler)
                            {
                                item.Value.dtKurulusTarihi = dtkurulustarihi;
                                item.Value.BildirgeOlanYillar = bildirgeOlanYillar;
                            }

                            try
                            {
                                #region Bildirgeleri Oluşturma

                                var BfdeHataliSatirVar = false;

                                try
                                {
                                    #region Başvuru Formlarını İçini Okuma

                                    //var IstenCikisiKoduEksikOlanKisiler7166Icin = "";

                                    var istenCikisWebClient = new ProjeGiris(AktifIsyeri, Enums.ProjeTurleri.SigortaliIstenAyrilis);

                                    foreach (var item in BasvuruFormlari)
                                    {
                                        DataTable dtbasvuru = item.Value;

                                        var Kanun = item.Key;

                                        var tesvik = TumTesvikler[Kanun];

                                        Dictionary<Enums.BasvuruFormuSutunTurleri, int> sutunlar = null;

                                        if (dtbasvuru != null)
                                        {
                                            var basvuruKisiler = new List<BasvuruKisi>();

                                            sutunlar = BasvuruFormlariSutunlari[Kanun];

                                            int girissutunu = sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.Giris) ? sutunlar[Enums.BasvuruFormuSutunTurleri.Giris] : -1;
                                            int bazsutunu = sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.Baz) ? sutunlar[Enums.BasvuruFormuSutunTurleri.Baz] : -1;
                                            int cikissutunu = sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.Cikis) ? sutunlar[Enums.BasvuruFormuSutunTurleri.Cikis] : -1;
                                            int tesvikdonemibaslangicsutunu = sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.TesvikBaslangic) ? sutunlar[Enums.BasvuruFormuSutunTurleri.TesvikBaslangic] : -1;
                                            int tesvikdonemibitissutunu = sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.TesvikBitis) ? sutunlar[Enums.BasvuruFormuSutunTurleri.TesvikBitis] : -1;
                                            int KanunNo = sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.KanunNo) ? sutunlar[Enums.BasvuruFormuSutunTurleri.KanunNo] : -1;
                                            int ilkTanimlamaTarihisutunu = sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi) ? sutunlar[Enums.BasvuruFormuSutunTurleri.IlkTanimlamaTarihi] : -1;

                                            if (sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.TesvikSuresi6111v1))
                                            {
                                                tesvikdonemibaslangicsutunu = sutunlar[Enums.BasvuruFormuSutunTurleri.TesvikSuresi6111v1];
                                                tesvikdonemibitissutunu = sutunlar[Enums.BasvuruFormuSutunTurleri.TesvikSuresi6111v1];
                                            }

                                            DateTime enYakinTanimlamaTarihi = DateTime.MinValue;
                                            DateTime enYakinIseGirisTarihi = DateTime.MinValue;

                                            foreach (DataRow rowbasvuru in dtbasvuru.Rows)
                                            {
                                                bool bfAraci = false;

                                                if (dtbasvuru.Columns.Contains("Aracı"))
                                                {
                                                    bfAraci = !String.IsNullOrEmpty(rowbasvuru["Aracı"].ToString()) && !rowbasvuru["Aracı"].ToString().ToLower().Contains("ana işveren") && !rowbasvuru["Aracı"].ToString().ToLower().Contains("ana şirket");
                                                }

                                                if (!bfAraci)
                                                {
                                                    bool devam = true;

                                                    if (Kanun.Equals("6645"))
                                                    {
                                                        string AktifMi = rowbasvuru[sutunlar[Enums.BasvuruFormuSutunTurleri.AktifMi]].ToString().Trim();

                                                        if (!AktifMi.Equals("AKTİF"))
                                                        {
                                                            devam = false;

                                                            PasifOlanlar6645.Add(rowbasvuru);
                                                        }
                                                    }
                                                    else if (Kanun.EndsWith("7256"))
                                                    {
                                                        string AktifMi = rowbasvuru[sutunlar[Enums.BasvuruFormuSutunTurleri.VerilsinMi7256]].ToString().Trim();

                                                        if (!AktifMi.ToLower().Equals("evet"))
                                                        {
                                                            devam = false;
                                                        }
                                                    }

                                                    if (devam)
                                                    {

                                                        string tcno = rowbasvuru[sutunlar[Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString().Trim();

                                                        DateTime tesvikbaslangic = DateTime.MinValue;
                                                        DateTime tesvikbitis = DateTime.MinValue;

                                                        int tesvikBaslangicYil = 0;

                                                        if (rowbasvuru[tesvikdonemibaslangicsutunu].GetType() == typeof(DateTime))
                                                        {
                                                            tesvikBaslangicYil = ((DateTime)rowbasvuru[tesvikdonemibaslangicsutunu]).Year;
                                                        }
                                                        else
                                                        {
                                                            tesvikBaslangicYil = Convert.ToInt32(rowbasvuru[tesvikdonemibaslangicsutunu].ToString().Split('-')[0].Split('/')[0].Trim());
                                                        }

                                                        if (!tesvikBaslangicYil.Equals(0))
                                                        {

                                                            if (sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.TesvikSuresi6111v1))
                                                            {
                                                                tesvikbaslangic = Convert.ToDateTime(rowbasvuru[tesvikdonemibaslangicsutunu].ToString().Split('-')[0].Trim());
                                                                tesvikbitis = Convert.ToDateTime(rowbasvuru[tesvikdonemibitissutunu].ToString().Split('-')[1].Trim());
                                                            }
                                                            else if (Kanun.Equals("14857"))
                                                            {
                                                                tesvikbaslangic = Convert.ToDateTime(rowbasvuru[tesvikdonemibaslangicsutunu].ToString().Trim());

                                                                var bitis14857 = rowbasvuru[tesvikdonemibitissutunu].ToString();

                                                                if (!string.IsNullOrEmpty(bitis14857))
                                                                {
                                                                    if (bitis14857.Contains("/"))
                                                                    {
                                                                        tesvikbitis = Convert.ToDateTime(String.Join("/", bitis14857.Split('/').Select(p => p.Trim())));
                                                                    }
                                                                    else tesvikbitis = Convert.ToDateTime(String.Join("/", bitis14857.Split(' ').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p))));
                                                                }
                                                                else tesvikbitis = DateTime.MaxValue;
                                                            }
                                                            else
                                                            {
                                                                if (!rowbasvuru[tesvikdonemibaslangicsutunu].ToString().Trim().Equals("0000/00"))
                                                                {
                                                                    tesvikbaslangic = Convert.ToDateTime(rowbasvuru[tesvikdonemibaslangicsutunu].ToString().Trim());
                                                                    tesvikbitis = Convert.ToDateTime(rowbasvuru[tesvikdonemibitissutunu].ToString().Trim());
                                                                }
                                                            }
                                                        }


                                                        DateTime giristarihi = girissutunu > -1 ? Convert.ToDateTime(rowbasvuru[girissutunu]) : DateTime.MinValue;

                                                        string BasvuruKanun = null;

                                                        if (KanunNo > -1)
                                                        {
                                                            var bk = rowbasvuru[sutunlar[Enums.BasvuruFormuSutunTurleri.KanunNo]].ToString();

                                                            if (!string.IsNullOrEmpty(bk))
                                                            {
                                                                BasvuruKanun = bk.PadLeft(5, '0');

                                                                if (Kanun.Equals("687") || Kanun.Equals("1687"))
                                                                {
                                                                    if (String.IsNullOrEmpty(BasvuruKanun))
                                                                    {
                                                                        BasvuruKanun = giristarihi >= new DateTime(2017, 6, 1) ? "01687" : "00687";
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (Kanun.Equals("687") || Kanun.Equals("1687"))
                                                                {
                                                                    BasvuruKanun = giristarihi >= new DateTime(2017, 6, 1) ? "01687" : "00687";
                                                                }
                                                            }
                                                        }
                                                        else BasvuruKanun = Kanun;

                                                        if (ilkTanimlamaTarihisutunu > -1)
                                                        {
                                                            if (tesvik.AltKanunIlkTanimlamaTarihiEnYakinOlaninKanunNosuOlsun)
                                                            {
                                                                DateTime tarih = Convert.ToDateTime(rowbasvuru[ilkTanimlamaTarihisutunu]);

                                                                if (tarih > enYakinTanimlamaTarihi || (tarih == enYakinTanimlamaTarihi && giristarihi > enYakinIseGirisTarihi))
                                                                {
                                                                    enYakinTanimlamaTarihi = tarih;

                                                                    enYakinIseGirisTarihi = giristarihi;

                                                                    if (!tesvik.Kanun.Equals("7166"))
                                                                    {
                                                                        tesvik.AltKanun = BasvuruKanun;
                                                                    }

                                                                    if (tesvik.Kanun.Equals("7103"))
                                                                    {
                                                                        tesvik.AltKanun = BasvuruKanun;

                                                                        if (TumTesvikler.ContainsKey("7166")) TumTesvikler["7166"].AltKanun = BasvuruKanun;
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        if (TumKisilerSonuc.TumKisiler.ContainsKey(tcno))
                                                        {
                                                            var kisi = TumKisilerSonuc.TumKisiler[tcno];

                                                            BasvuruKisi basvuruKisi = new BasvuruKisi
                                                            {
                                                                TcKimlikNo = tcno,
                                                                GirisTarihi = giristarihi,
                                                                Baz = bazsutunu > -1 ? Convert.ToInt32(rowbasvuru[bazsutunu].ToString().Replace(".", "")) : 0,
                                                                CikisTarihi = cikissutunu > -1 ? (!string.IsNullOrEmpty(rowbasvuru[cikissutunu].ToString()) ? Convert.ToDateTime(rowbasvuru[cikissutunu].ToString()) : DateTime.MinValue) : DateTime.MinValue,
                                                                TesvikDonemiBaslangic = tesvikbaslangic,
                                                                TesvikDonemiBitis = tesvikbitis,
                                                                Kanun = BasvuruKanun
                                                            };

                                                            if (!kisi.KisiBasvuruKayitlari.ContainsKey(Kanun)) kisi.KisiBasvuruKayitlari.Add(Kanun, new List<Classes.BasvuruKisi>());

                                                            kisi.KisiBasvuruKayitlari[Kanun].Add(basvuruKisi);

                                                            if (!BasvuruListesindeOlanKisiler.Contains(kisi)) BasvuruListesindeOlanKisiler.Add(kisi);

                                                            bool sistemCikislarinaBakilacak = false;

                                                            if (Kanun == "6111")
                                                            {
                                                                basvuruKisiler.Add(basvuruKisi);

                                                                if (basvuruKisi.CikisTarihi == DateTime.MinValue) sistemCikislarinaBakilacak = true;
                                                            }
                                                            else if (Kanun == "6645" || Kanun == "687" || Kanun == "3294")
                                                            {
                                                                sistemCikislarinaBakilacak = true;
                                                            }

                                                            if (sistemCikislarinaBakilacak && !CariHesapla)
                                                            {
                                                                if (!kisi.SistemGirisCikislariCekildi && basvuruKisi.CikisTarihi == DateTime.MinValue)
                                                                {
                                                                    var cikislarResponse = Metodlar.SistemdenKisininGirisCikislariniBul(AktifIsyeri, kisi.TckimlikNo, ref istenCikisWebClient);

                                                                    if (cikislarResponse.Durum)
                                                                    {
                                                                        kisi.SistemGirisCikislariCekildi = true;
                                                                        kisi.SistemGirisCikislari = cikislarResponse.girisCikislar.OrderBy(p => p.Tarih).ThenBy(p => p.GirisMi ? 0 : 1).ToList();
                                                                    }
                                                                    else
                                                                    {
                                                                        MessageBox.Show("En az bir kişinin sistemden çıkışlarına bakılamadı. Lütfen internet bağlantınızın veya sistemin çalışıp çalışmadığını kontrol ediniz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                                                        return;
                                                                    }
                                                                }

                                                                if (kisi.SistemGirisCikislariCekildi)
                                                                {
                                                                    foreach (var girisCikis in kisi.SistemGirisCikislari)
                                                                    {
                                                                        if (!girisCikis.GirisMi)
                                                                        {
                                                                            var baslangic = basvuruKisi.GirisTarihi > DateTime.MinValue ? basvuruKisi.GirisTarihi : basvuruKisi.TesvikDonemiBaslangic;
                                                                            var bitis = basvuruKisi.CikisTarihi > DateTime.MinValue ? basvuruKisi.CikisTarihi : basvuruKisi.TesvikDonemiBitis == DateTime.MaxValue || basvuruKisi.TesvikDonemiBitis == DateTime.MinValue ? DateTime.MaxValue : basvuruKisi.TesvikDonemiBitis.AddMonths(1).AddDays(-1);

                                                                            if (baslangic <= girisCikis.Tarih && girisCikis.Tarih <= bitis)
                                                                            {
                                                                                basvuruKisi.CikisTarihi = girisCikis.Tarih;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                }

                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            var temprows = dtbasvuru
                                                 .AsEnumerable()
                                                 .Where(row => !row.Table.Columns.Contains("Aracı") || (String.IsNullOrEmpty(row["Aracı"].ToString()) || row["Aracı"].ToString().ToLower().Contains("ana işveren") || row["Aracı"].ToString().ToLower().Contains("ana şirket")))
                                                 .GroupBy(row => new DateTime(TumTesvikler[Kanun].BazYil ? Convert.ToDateTime(row[sutunlar[Enums.BasvuruFormuSutunTurleri.Giris]]).Year : 1, TumTesvikler[Kanun].BazAy ? Convert.ToDateTime(row[sutunlar[Enums.BasvuruFormuSutunTurleri.Giris]]).Month : 1, 1));


                                            var tempbazlar = temprows
                                                                .ToDictionary(x => x.Key,
                                                                                x => x.Select(row => row[sutunlar[Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString()).ToList());

                                            BasvuruListesiBazGruplari.Add(Kanun, tempbazlar);


                                            tesvik.BazSayilari = temprows
                                                                   .ToDictionary(x => String.Format("{0}-{1}", TumTesvikler[Kanun].BazYil ? x.Key.Year.ToString() : "1", TumTesvikler[Kanun].BazAy ? x.Key.Month.ToString() : "1"),
                                                                                 x => sutunlar.ContainsKey(Enums.BasvuruFormuSutunTurleri.Baz) ? Convert.ToInt32(x.FirstOrDefault()[sutunlar[Enums.BasvuruFormuSutunTurleri.Baz]].ToString().Replace(".", "")) : 0);


                                            if (Kanun == "6111" && basvuruKisiler.Count > 0)
                                            {
                                                var EnYuksekBazlar = basvuruKisiler.GroupBy(p => p.GirisTarihi.AddDays(-p.GirisTarihi.Day + 1)).ToDictionary(x => x.Key, x => x.Max(p => p.Baz));

                                                basvuruKisiler.ForEach(p =>
                                                {
                                                    p.Baz = EnYuksekBazlar[p.GirisTarihi.AddDays(-p.GirisTarihi.Day + 1)];
                                                });


                                            }


                                        }

                                    }

                                    var altKanunuEksikOlanTesvikler = TumTesvikler.Where(t => t.Value.AltKanunIlkTanimlamaTarihiEnYakinOlaninKanunNosuOlsun && String.IsNullOrEmpty(t.Value.AltKanun) && BasvuruFormlari[t.Key] != null && BasvuruFormlari[t.Key].Rows.Count > 0);

                                    if (altKanunuEksikOlanTesvikler.Count() > 0)
                                    {
                                        var altKanunuEksikOlanTesvikKanunNolari = String.Join(",", altKanunuEksikOlanTesvikler.Select(p => p.Key));

                                        MessageBox.Show(altKanunuEksikOlanTesvikKanunNolari + " kanun numaraları tespit edilemedi. Başvuru formunda en yakın tarihli tanımlama yapılan kişi yok veya Kanun No alanı hatalı", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                        return;
                                    }

                                    istenCikisWebClient.Disconnect();

                                    #endregion
                                }
                                catch (FormatException)
                                {
                                    BfdeHataliSatirVar = true;

                                    Metodlar.HataliSatirlarKontroluYap(AktifIsyeri, dtaylikliste, dsbasvurulistesi, true, true, ref dthatalisatirlar, ref BasvuruFormlariHataliSatirlar);

                                    if (dthatalisatirlar != null || BasvuruFormlariHataliSatirlar.Any(p => p.Value != null))
                                    {
                                        if (DialogResult.OK == MessageBox.Show("Başvuru veya Aylık listede hatalı satırlar olduğu için devam edilemedi"))
                                        {
                                            new frmListeHata(dthatalisatirlar, BasvuruFormlariHataliSatirlar).ShowDialog();
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Başvuru veya Aylık listede hatalı satırlar var fakat hatalı satırlar tespit edilemedi. Lütfen yazılımcı ile irtabata geçin");

                                        throw new Exception("Başvuru veya Aylık listede hatalı satırlar olduğu için devam edilemedi");
                                    }
                                }
                                catch (InvalidCastException)
                                {
                                    BfdeHataliSatirVar = true;

                                    Metodlar.HataliSatirlarKontroluYap(AktifIsyeri, dtaylikliste, dsbasvurulistesi, true, true, ref dthatalisatirlar, ref BasvuruFormlariHataliSatirlar);

                                    if (dthatalisatirlar != null || BasvuruFormlariHataliSatirlar.Any(p => p.Value != null))
                                    {
                                        if (DialogResult.OK == MessageBox.Show("Başvuru veya Aylık listede hatalı satırlar olduğu için devam edilemedi"))
                                        {
                                            new frmListeHata(dthatalisatirlar, BasvuruFormlariHataliSatirlar).ShowDialog();
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Başvuru veya Aylık listede hatalı satırlar var fakat hatalı satırlar tespit edilemedi. Lütfen yazılımcı ile irtabata geçin");

                                        throw new Exception("Başvuru veya Aylık listede hatalı satırlar olduğu için devam edilemedi");
                                    }
                                }

                                #region 7166 Listesini Okuma

                                var basvuruListesi7166Yol = Metodlar.FormBul(AktifIsyeri, Enums.FormTuru.BasvuruListesi7166);

                                if (basvuruListesi7166Yol != null)
                                {
                                    DataTable dtBasvuruListesi7166 = Metodlar.BasvuruListesi7166Yukle(basvuruListesi7166Yol);

                                    bool EskiSablon = dtBasvuruListesi7166.Columns.Contains("Silinebiliyor Mu?");

                                    if (dtBasvuruListesi7166.Rows.Count > 0)
                                    {
                                        var basvuruListesi7166Kisiler = dtBasvuruListesi7166.AsEnumerable().Select(row => new BasvuruListesi7166Kisi
                                        {
                                            TckimlikNo = row[(int)Enums.BasvuruListesi7166SutunTurleri.TcKimlikNoSosyalGuvenlikNo].ToString(),
                                            Ad = row[(int)Enums.BasvuruListesi7166SutunTurleri.Ad].ToString(),
                                            Soyad = row[(int)Enums.BasvuruListesi7166SutunTurleri.Soyad].ToString(),
                                            Giris = Convert.ToDateTime(row[(int)Enums.BasvuruListesi7166SutunTurleri.Giris].ToString()),
                                            Cikis = row[(int)Enums.BasvuruListesi7166SutunTurleri.Cikis].ToString(),
                                            IstenCikisNedeni = row[(int)Enums.BasvuruListesi7166SutunTurleri.IstenCikisNedeni].ToString(),
                                            VerilmisMi7103 = EskiSablon ? false : row[(int)Enums.BasvuruListesi7166SutunTurleri.VerilmisMi7103].ToString().Equals("Evet"),
                                            UygunlukDurumu = row[(int)Enums.BasvuruListesi7166SutunTurleri.UygunlukDurumu].ToString(),
                                            UygunlukDurumuNedeni = row[(int)Enums.BasvuruListesi7166SutunTurleri.UygunlukDurumuNedeni].ToString(),
                                        }).ToList();

                                        foreach (var basvuru7166 in basvuruListesi7166Kisiler)
                                        {
                                            if (TumKisilerSonuc.TumKisiler.ContainsKey(basvuru7166.TckimlikNo))
                                            {
                                                var kisi = TumKisilerSonuc.TumKisiler[basvuru7166.TckimlikNo];

                                                kisi.BasvuruListesi7166Kayitlari.Add(basvuru7166);
                                            }
                                        }

                                    }
                                }
                                #endregion

                                List<Classes.Cikti> tumCiktilar = new List<Classes.Cikti>();

                                int index = 0;

                                formBildirgeOlustur.progressBar1.Visible = true;

                                formBildirgeOlustur.Cursor = Cursors.WaitCursor;

                                try
                                {
                                    if (!BfdeHataliSatirVar)
                                    {
                                        foreach (KeyValuePair<string, string> yilveay in yilveaylar)
                                        {
                                            //var seciliYil = Convert.ToInt32(yilveay.Key);
                                            //var seciliAy = Convert.ToInt32(yilveay.Value);

                                            //if (muhtasarIsyeri != null && muhtasarIsyeri.Isyeri.IsyeriID != seciliIsyeri.IsyeriID)
                                            //{
                                            //    if (seciliYil != muhtasarIsyeri.Yil || seciliAy != muhtasarIsyeri.Ay) continue;
                                            //}

                                            List<Classes.Cikti> ayCiktilari = new List<Classes.Cikti>();

                                            BildirgeOlustur(
                                                 yilveay,
                                                 dtaylikliste,
                                                 AktifIsyeri,
                                                 tumyilveaylar,
                                                 ref MyApp,
                                                 ref BildirgeWorkBook,
                                                 ref excelprocessid,
                                                 AktifIsyeri,
                                                 IsyeriSicilNo,
                                                 enbuyukay,
                                                 SadeceIcmal,
                                                 ref hatalar,
                                                 ref basariliolanlar,
                                                 IsyeriAd,
                                                 IsyeriVergiNo,
                                                 ref index,
                                                 yilveaylar,
                                                 out ayCiktilari,
                                                 hesaplananIsyeriSira,
                                                 hesaplanacakIsyerleri.Count,
                                                 ref formBildirgeOlustur,
                                                 muhtasarIsyeri
                                                 );

                                            //if (ayCiktilari.Count > 0)
                                            //{
                                            //    if (muhtasarIsyeri != null)
                                            //    {
                                            //        if (seciliAy == muhtasarIsyeri.Ay && seciliYil == muhtasarIsyeri.Yil)
                                            //        {
                                            //            //MuhtasarGenelXmlOlusturulacak = true;
                                            //            muhtasarYil = muhtasarIsyeri.Yil;
                                            //            muhtasarAy = muhtasarIsyeri.Ay;
                                            //        }
                                            //    }

                                            //}

                                            tumCiktilar.AddRange(ayCiktilari);

                                        }
                                    }
                                }
                                catch (FormatException)
                                {
                                    Metodlar.HataliSatirlarKontroluYap(AktifIsyeri, dtaylikliste, dsbasvurulistesi, true, true, ref dthatalisatirlar, ref BasvuruFormlariHataliSatirlar);

                                    if (dthatalisatirlar != null || BasvuruFormlariHataliSatirlar.Any(p => p.Value != null))
                                    {
                                        if (DialogResult.OK == MessageBox.Show("Başvuru veya Aylık listede hatalı satırlar olduğu için devam edilemedi"))
                                        {
                                            new frmListeHata(dthatalisatirlar, BasvuruFormlariHataliSatirlar).ShowDialog();
                                        }

                                        throw new Exception("Başvuru veya Aylık listede hatalı satırlar olduğu için devam edilemedi");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Başvuru veya Aylık listede hatalı satırlar var fakat hatalı satırlar tespit edilemedi. Lütfen yazılımcı ile irtabata geçin");

                                        throw new Exception("Başvuru veya Aylık listede hatalı satırlar olduğu için devam edilemedi");
                                    }


                                }
                                catch (InvalidCastException)
                                {
                                    Metodlar.HataliSatirlarKontroluYap(AktifIsyeri, dtaylikliste, dsbasvurulistesi, true, true, ref dthatalisatirlar, ref BasvuruFormlariHataliSatirlar);

                                    if (dthatalisatirlar != null || BasvuruFormlariHataliSatirlar.Any(p => p.Value != null))
                                    {
                                        if (DialogResult.OK == MessageBox.Show("Başvuru veya Aylık listede hatalı satırlar olduğu için devam edilemedi"))
                                        {
                                            new frmListeHata(dthatalisatirlar, BasvuruFormlariHataliSatirlar).ShowDialog();
                                        }

                                        throw new Exception("Başvuru veya Aylık listede hatalı satırlar olduğu için devam edilemedi");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Başvuru veya Aylık listede hatalı satırlar var fakat hatalı satırlar tespit edilemedi. Lütfen yazılımcı ile irtabata geçin");

                                        throw new Exception("Başvuru veya Aylık listede hatalı satırlar olduğu için devam edilemedi");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Metodlar.HataMesajiGoster(ex, "Bildirge oluşturulurken hata meydana geldi");
                                }



                                #endregion

                                #region İcmalleri Oluşturma

                                List<string> OlusturulacakIcmaller = TumTesvikler.Where(t => t.Value.TesvikAyIstatistikleri.Any(p => p.Value.TesvikAlacakVar)).Select(p => p.Key).ToList();

                                int ToplamTesvikTuru = OlusturulacakIcmaller.Count;

                                var icmalCikartilacakAylar = yilveaylar.Select(p => new DateTime(Convert.ToInt32(p.Key), Convert.ToInt32(p.Value), 1));

                                #region Genel Icmal

                                if (ToplamTesvikTuru > 0)
                                {

                                    var tesvikYillar = TumTesvikler.ToDictionary(x => x.Key, x => new SortedDictionary<int, SortedDictionary<int, Classes.Icmal>>());

                                    SortedDictionary<int, SortedDictionary<int, int>> tumyillar = new SortedDictionary<int, SortedDictionary<int, int>>();

                                    List<string> MahsupYapilacakIcmalKanunlari = new List<string>();

                                    foreach (var tesvikItem in TumTesvikler)
                                    {
                                        var kanun = tesvikItem.Key;

                                        var tesvik = tesvikItem.Value;

                                        foreach (var tarih in icmalCikartilacakAylar)
                                        {
                                            var kanunYillar = tesvikYillar[kanun];

                                            if (!kanunYillar.ContainsKey(tarih.Year)) kanunYillar.Add(tarih.Year, new SortedDictionary<int, Classes.Icmal>());

                                            SortedDictionary<int, Classes.Icmal> aylar = kanunYillar[tarih.Year];

                                            Classes.Icmal icmal = null;

                                            if (tesvik.TesvikAyIstatistikleri.ContainsKey(tarih))
                                            {
                                                icmal = tesvik.TesvikAyIstatistikleri[tarih].Icmal;
                                            }

                                            if (!aylar.ContainsKey(tarih.Month)) aylar.Add(tarih.Month, icmal);

                                            if (!tumyillar.ContainsKey(tarih.Year)) tumyillar.Add(tarih.Year, new SortedDictionary<int, int>());

                                            SortedDictionary<int, int> aylar2 = tumyillar[tarih.Year];

                                            if (!aylar2.ContainsKey(tarih.Month)) aylar2.Add(tarih.Month, 0);
                                        }

                                        foreach (var item in tesvikItem.Value.TesvikAyIstatistikleri)
                                        {
                                            var tarih = item.Key;
                                            var tesvikAyIstatistikleri = item.Value;

                                            foreach (var icmalTutarItem in tesvikAyIstatistikleri.Icmal.Tutarlar)
                                            {
                                                //if (icmalTutarItem.Value >= 0)
                                                {
                                                    if (!MahsupYapilacakIcmalKanunlari.Contains(icmalTutarItem.Key)) MahsupYapilacakIcmalKanunlari.Add(icmalTutarItem.Key);
                                                }
                                            }

                                        }
                                    }

                                    var TesvikVerilecekKanunlar = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.Select(p => p).ToList();
                                    if (!OlusturulacakIcmaller.Contains("6322/25510")) TesvikVerilecekKanunlar.Remove("6322/25510");
                                    if (!OlusturulacakIcmaller.Contains("5510")) TesvikVerilecekKanunlar.Remove("5510");
                                    if (!OlusturulacakIcmaller.Contains("14857")) TesvikVerilecekKanunlar.Remove("14857");

                                    var icmalDegerYazilacakAlanlar = TumTesvikler.ToDictionary(x => x.Key, x => new Dictionary<string, Excel2.Range>());
                                    icmalDegerYazilacakAlanlar.Add("Tumu", new Dictionary<string, Excel2.Range>());

                                    //var toplamIcmalTutarlari = icmalDegerYazilacakAlanlar.ToDictionary(x => x.Key, x => 0.0);

                                    Excel2.Workbook MyBook = null;

                                    Excel2.Worksheet MySheet = null;

                                    var workbooks = MyApp.Workbooks;

                                    Genel.IcmalKaydediliyorKontrolu();

                                    MyBook = workbooks.Open(Path.Combine(Application.StartupPath, "Icmal.xlsx"));

                                    MySheet = (Excel2.Worksheet)MyBook.Sheets[1]; // Explicit cast is not required here

                                    HafizadanAtilacaklar.AddRange(new List<object> { workbooks, MyBook, MySheet });

                                    var IcmalIsyeriAd = MySheet.Range[IcmalOlusturmaSabitleri.IcmalIsyeriAd];
                                    var IcmalBaslik1 = MySheet.Range[IcmalOlusturmaSabitleri.IcmalBaslik1];
                                    var IcmalBaslik2 = MySheet.Range[IcmalOlusturmaSabitleri.IcmalBaslik2];
                                    var IcmalIsyeriSicil = MySheet.Range[IcmalOlusturmaSabitleri.IcmalIsyeriSicil];


                                    IcmalIsyeriAd.Value2 = IsyeriAd.ToUpper();

                                    IcmalBaslik1.Value2 = IcmalOlusturmaSabitleri.IcmalBaslik1Tum;

                                    IcmalBaslik2.Value2 = IcmalOlusturmaSabitleri.IcmalBaslik2Tum;

                                    IcmalIsyeriSicil.Value2 = " " + IsyeriSicilNo;

                                    try
                                    {
                                        List<string> isyerisicils = new List<string>();

                                        isyerisicils.Add(IsyeriSicilNo.Substring(0, 1));

                                        isyerisicils.Add(IsyeriSicilNo.Substring(1, 4));

                                        isyerisicils.Add(IsyeriSicilNo.Substring(5, 2));

                                        isyerisicils.Add(IsyeriSicilNo.Substring(7, 2));

                                        isyerisicils.Add(IsyeriSicilNo.Substring(9, 7));

                                        isyerisicils.Add(IsyeriSicilNo.Substring(16, 3));

                                        isyerisicils.Add(IsyeriSicilNo.Substring(19, 2));

                                        string isyerisicilno = String.Join(" ", isyerisicils.ToArray()).Trim();

                                        isyerisicilno += "-" + IsyeriSicilNo.Substring(21, 2);

                                        IcmalIsyeriSicil.Value2 = isyerisicilno;

                                    }
                                    catch
                                    {

                                    }

                                    HafizadanAtilacaklar.AddRange(new List<object> { IcmalIsyeriAd, IcmalBaslik1, IcmalBaslik2, IcmalIsyeriSicil });

                                    List<Excel2.Range> headers = new List<Excel2.Range>();

                                    List<Excel2.Range> rows = new List<Excel2.Range>();

                                    List<Excel2.Range> yiltoplamlari = new List<Excel2.Range>();

                                    int Satir = IcmalOlusturmaSabitleri.IcmalBaslangicSatir;

                                    int CiftSutun = IcmalOlusturmaSabitleri.IcmalCiftBaslangicSutun;

                                    var enumeratoryil = tumyillar.GetEnumerator();

                                    int i = 0;

                                    while (enumeratoryil.MoveNext())
                                    {
                                        int yil = enumeratoryil.Current.Key;

                                        int Sutun = CiftSutun;

                                        var headerDonem = MySheet.Cells[Satir + i * 15, Sutun] as Excel2.Range;

                                        headerDonem.Value2 = "DÖNEM";

                                        int sira = 1;

                                        for (int p = 0; p < TesvikVerilecekKanunlar.Count; p++)
                                        {
                                            var headerKanun = MySheet.Cells[Satir + i * 15, Sutun + sira] as Excel2.Range;

                                            headerKanun.Value2 = TesvikVerilecekKanunlar[p];

                                            HafizadanAtilacaklar.Add(headerKanun);

                                            sira++;
                                        }

                                        var headerTumu = MySheet.Cells[Satir + i * 15, Sutun + sira] as Excel2.Range;

                                        headerTumu.Value2 = "TÜMÜ";

                                        var baslangic = MySheet.Cells[Satir + i * 15, Sutun];

                                        var bitis = MySheet.Cells[Satir + i * 15, Sutun + sira];

                                        var allheaders = MySheet.Range[baslangic, bitis];

                                        headers.Add(allheaders);

                                        HafizadanAtilacaklar.AddRange(new List<object> { headerDonem, headerTumu, allheaders, baslangic, bitis });

                                        var enumeratoray = enumeratoryil.Current.Value.GetEnumerator();

                                        int j = 1;

                                        while (enumeratoray.MoveNext())
                                        {
                                            int ay = enumeratoray.Current.Key;

                                            Excel2.Range cellyilay = MySheet.Cells[Satir + j + i * 15, Sutun] as Excel2.Range;

                                            cellyilay.Value2 = yil.ToString() + "/" + ay.ToString();

                                            int sira2 = 1;

                                            for (int p = 0; p < TesvikVerilecekKanunlar.Count; p++)
                                            {
                                                var kanun = TesvikVerilecekKanunlar[p];

                                                var cellayKanunTutari = MySheet.Cells[Satir + j + i * 15, Sutun + sira2] as Excel2.Range;

                                                icmalDegerYazilacakAlanlar[kanun].Add(yil + "-" + ay, cellayKanunTutari);

                                                HafizadanAtilacaklar.Add(cellayKanunTutari);

                                                sira2++;
                                            }

                                            //Excel2.Range cell6111 = MySheet.Cells[Satir + j + i * 15, Sutun + 1] as Excel2.Range;

                                            //cell6111.Value2 = Metodlar.ToTL(icmal6111Tutar).ToString();

                                            //Excel2.Range cell6645 = MySheet.Cells[Satir + j + i * 15, Sutun + 2] as Excel2.Range;

                                            //cell6645.Value2 = Metodlar.ToTL(icmal6645Tutar).ToString();

                                            //Excel2.Range cell687 = MySheet.Cells[Satir + j + i * 15, Sutun + 3] as Excel2.Range;

                                            //cell687.Value2 = Metodlar.ToTL(icmal687Tutar).ToString();

                                            //Excel2.Range cell6486 = MySheet.Cells[Satir + j + i * 15, Sutun + 4] as Excel2.Range;

                                            //cell6486.Value2 = Metodlar.ToTL(icmal6486Tutar).ToString();

                                            //Excel2.Range cell7103 = MySheet.Cells[Satir + j + i * 15, Sutun + 5] as Excel2.Range;

                                            //cell7103.Value2 = Metodlar.ToTL(icmal7103Tutar).ToString();

                                            //Excel2.Range cell2828 = MySheet.Cells[Satir + j + i * 15, Sutun + 6] as Excel2.Range;

                                            //cell2828.Value2 = Metodlar.ToTL(icmal2828Tutar).ToString();

                                            //Excel2.Range cell14857 = MySheet.Cells[Satir + j + i * 15, Sutun + 7] as Excel2.Range;

                                            //cell14857.Value2 = Metodlar.ToTL(icmal14857Tutar).ToString();

                                            Excel2.Range cellAyToplam = MySheet.Cells[Satir + j + i * 15, Sutun + sira2] as Excel2.Range;

                                            icmalDegerYazilacakAlanlar["Tumu"].Add(yil + "-" + ay, cellAyToplam);

                                            //cellAyToplam.Value2 = Metodlar.ToTL(icmal6111Tutar + icmal6645Tutar + icmal687Tutar + icmal6486Tutar + icmal7103Tutar + icmal2828Tutar + icmal14857Tutar).ToString();

                                            HafizadanAtilacaklar.AddRange(new List<object> { cellyilay, cellAyToplam });

                                            //rows.AddRange(new List<Excel2.Range> { cellyilay, cell6111, cell6645, cell687, cell6486, cell7103, cell2828, cellAyToplam });

                                            j++;
                                        }

                                        var baslangicrow = MySheet.Cells[Satir + 1 + i * 15, Sutun];

                                        var bitisrow = MySheet.Cells[Satir + j - 1 + i * 15, Sutun + TesvikVerilecekKanunlar.Count + 1];

                                        var tumsatir = MySheet.Range[baslangicrow, bitisrow];

                                        rows.Add(tumsatir);

                                        HafizadanAtilacaklar.AddRange(new List<object> { baslangicrow, bitisrow, tumsatir });

                                        Excel2.Range cellyilToplami = MySheet.Cells[Satir + j + i * 15, Sutun] as Excel2.Range;

                                        cellyilToplami.Value2 = "Yıl toplamı";

                                        sira = 1;

                                        for (int p = 0; p < TesvikVerilecekKanunlar.Count; p++)
                                        {
                                            var cellyilKanun = MySheet.Cells[Satir + j + i * 15, Sutun + sira] as Excel2.Range;

                                            var kanun = TesvikVerilecekKanunlar[p];

                                            icmalDegerYazilacakAlanlar[kanun].Add(yil.ToString(), cellyilKanun);

                                            HafizadanAtilacaklar.Add(cellyilKanun);

                                            sira++;
                                        }

                                        var cellYilTumu = MySheet.Cells[Satir + j + i * 15, Sutun + sira] as Excel2.Range;
                                        icmalDegerYazilacakAlanlar["Tumu"].Add(yil.ToString(), cellYilTumu);

                                        var baslangicyil = MySheet.Cells[Satir + j + i * 15, Sutun];

                                        var bitisyil = MySheet.Cells[Satir + j + i * 15, Sutun + TesvikVerilecekKanunlar.Count + 1];

                                        var yiltoplamitumsatir = MySheet.Range[baslangicyil, bitisyil];

                                        yiltoplamlari.Add(yiltoplamitumsatir);

                                        HafizadanAtilacaklar.AddRange(new List<object> { cellyilToplami, yiltoplamitumsatir, baslangicyil, bitisyil });

                                        i++;
                                    }

                                    foreach (Excel2.Range r in headers)
                                    {
                                        var font = r.Font;

                                        font.Bold = true;

                                        font.Name = "Times New Roman";

                                        font.Size = 10;

                                        r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                                        r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                        var interior = r.Interior;

                                        interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(234, 241, 221));

                                        var borders = r.Borders;

                                        borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                        HafizadanAtilacaklar.AddRange(new List<object> { font, interior, r, borders });
                                    }

                                    foreach (Excel2.Range r in rows)
                                    {
                                        var font = r.Font;

                                        font.Bold = false;

                                        font.Name = "Times New Roman";

                                        font.Size = 10;

                                        r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                                        r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignRight;

                                        var interior = r.Interior;

                                        interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(197, 217, 241));

                                        var borders = r.Borders;

                                        borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                        HafizadanAtilacaklar.AddRange(new List<object> { font, interior, r, borders });

                                    }

                                    foreach (Excel2.Range r in yiltoplamlari)
                                    {
                                        var font = r.Font;

                                        font.Bold = true;

                                        font.Name = "Times New Roman";

                                        font.Size = 10;

                                        r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                                        r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignRight;

                                        //r.BorderAround(Excel2.XlLineStyle.xlContinuous);

                                        var borders = r.Borders;

                                        borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                        HafizadanAtilacaklar.AddRange(new List<object> { font, r, borders });

                                    }

                                    int str = Satir + i * 15;

                                    int stn = CiftSutun;

                                    List<Excel2.Range> ranges = new List<Excel2.Range>();

                                    var rangebaslangic = MySheet.Cells[str, stn];

                                    var rangebitis = MySheet.Cells[str + 6, stn + TesvikVerilecekKanunlar.Count - 1];

                                    Excel2.Range range = MySheet.Range[rangebaslangic, rangebitis];

                                    int siratum = 0;

                                    for (int p = 0; p < TesvikVerilecekKanunlar.Count; p++)
                                    {
                                        var rangeKanunHeader = (Excel2.Range)MySheet.Cells[str + 7, stn + siratum];
                                        var rangeKanunTumu = (Excel2.Range)MySheet.Cells[str + 8, stn + siratum];

                                        var kanun = TesvikVerilecekKanunlar[p];

                                        rangeKanunHeader.Value2 = kanun;

                                        icmalDegerYazilacakAlanlar[kanun].Add("Tumu", rangeKanunTumu);

                                        HafizadanAtilacaklar.Add(rangeKanunHeader);
                                        HafizadanAtilacaklar.Add(rangeKanunTumu);

                                        siratum++;
                                    }

                                    var rangetutarbaslangic = MySheet.Cells[str + 9, stn];

                                    var rangetutarbitis = MySheet.Cells[str + 12, stn + TesvikVerilecekKanunlar.Count - 1];

                                    Excel2.Range rangetutar = MySheet.Range[rangetutarbaslangic, rangetutarbitis];

                                    icmalDegerYazilacakAlanlar["Tumu"].Add("Tumu", rangetutar);

                                    var allranges = MySheet.Range[rangebaslangic, rangetutarbitis];

                                    ranges = new List<Excel2.Range> { allranges };

                                    HafizadanAtilacaklar.AddRange(new List<object> { range, rangetutar, allranges, rangebaslangic, rangebitis, rangetutarbaslangic, rangetutarbitis });

                                    range.Merge();

                                    range.Value2 = "Teşvik kapsamında işveren tarafından iade alınacak olan toplam prim tutarı(kanuni faiz hariç)";

                                    rangetutar.Merge();

                                    foreach (var rng in ranges)
                                    {
                                        var font = rng.Font;

                                        font.Bold = true;

                                        font.Name = "Times New Roman";

                                        font.Size = 10;

                                        rng.WrapText = true;

                                        rng.VerticalAlignment = 2;

                                        rng.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                        var interior = rng.Interior;

                                        interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(217, 151, 149));

                                        var borders = rng.Borders;

                                        borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                        rng.BorderAround(Excel2.XlLineStyle.xlContinuous, Excel2.XlBorderWeight.xlMedium);

                                        HafizadanAtilacaklar.AddRange(new List<object> { font, rng, interior, borders });
                                    }

                                    var fonttutar = rangetutar.Font;

                                    fonttutar.Size = 15;

                                    HafizadanAtilacaklar.Add(fonttutar);

                                    var dusulecekKanunlar = new List<string>();

                                    foreach (var mahsupKanun in MahsupYapilacakIcmalKanunlari)
                                    {
                                        var bagliKanunlar = Metodlar.BagliKanunlariGetir(mahsupKanun);

                                        dusulecekKanunlar.AddRange(bagliKanunlar);

                                        dusulecekKanunlar.Add(mahsupKanun);
                                    }

                                    dusulecekKanunlar = dusulecekKanunlar.Distinct().ToList();

                                    if (!Directory.Exists(isyeriSavePath)) Directory.CreateDirectory(isyeriSavePath);

                                    try
                                    {
                                        var tumToplamlar = TesvikVerilecekKanunlar.ToDictionary(x => x, x => 0.0m);

                                        enumeratoryil = tumyillar.GetEnumerator();

                                        i = 0;

                                        while (enumeratoryil.MoveNext())
                                        {
                                            var yilToplamlari = TesvikVerilecekKanunlar.ToDictionary(x => x, x => 0.0m);

                                            int yil = enumeratoryil.Current.Key;

                                            int j = 1;

                                            var enumeratoray = enumeratoryil.Current.Value.GetEnumerator();

                                            while (enumeratoray.MoveNext())
                                            {
                                                int ay = enumeratoray.Current.Key;

                                                var kanunAyIcmalleri = tesvikYillar.ToDictionary(x => x.Key, x => x.Value.ContainsKey(yil) && x.Value[yil].ContainsKey(ay) ? x.Value[yil][ay] : null);

                                                decimal ayTumTesviklerToplam = 0;

                                                var muhtasarAyMi = muhtasarYil == yil && muhtasarAy == ay;

                                                if (muhtasarAyMi)
                                                {

                                                    if (tumIsyerleriIcmaller.ContainsKey(AktifIsyeri)) tumIsyerleriIcmaller.Remove(AktifIsyeri);

                                                    tumIsyerleriIcmaller.Add(AktifIsyeri, Program.TumTesvikler.ToDictionary(x => x.Key, x => 0m));
                                                }

                                                foreach (var kanun in TesvikVerilecekKanunlar)
                                                {
                                                    Classes.Icmal icmal = kanunAyIcmalleri[kanun];

                                                    decimal IcmalTutar = 0;

                                                    if (icmal != null && icmal.Tutarlar.Count > 0)
                                                    {
                                                        IcmalTutar = icmal.TutarlarBagliKanunlarMahsupluTutarlar["00000"];

                                                        //if (icmal.TutarlarBagliKanunlarMahsupTutarlari.ContainsKey(mahsupKanun))
                                                        //{
                                                        //    IcmalTutar -= icmal.TutarlarBagliKanunlarMahsupTutarlari[mahsupKanun];
                                                        //}

                                                        foreach (var dusulecekKanun in dusulecekKanunlar)
                                                        {
                                                            if (icmal.TutarlarBagliKanunlarMahsupTutarlari.ContainsKey(dusulecekKanun))
                                                            {
                                                                IcmalTutar -= icmal.TutarlarBagliKanunlarMahsupTutarlari[dusulecekKanun];
                                                            }
                                                        }

                                                    }

                                                    icmalDegerYazilacakAlanlar[kanun][yil + "-" + ay].Value2 = IcmalTutar.ToTL();

                                                    ayTumTesviklerToplam += IcmalTutar.ToTL().Replace("₺", "").ToDecimalSgk();

                                                    yilToplamlari[kanun] += IcmalTutar.ToTL().Replace("₺", "").ToDecimalSgk();
                                                    tumToplamlar[kanun] += IcmalTutar.ToTL().Replace("₺", "").ToDecimalSgk();

                                                    if (muhtasarAyMi)
                                                    {

                                                        if (tumIsyerleriIcmaller[AktifIsyeri].ContainsKey(kanun)) tumIsyerleriIcmaller[AktifIsyeri].Remove(kanun);

                                                        tumIsyerleriIcmaller[AktifIsyeri].Add(kanun, IcmalTutar);
                                                    }

                                                }

                                                if (muhtasarAyMi)
                                                {
                                                    if (tumIsyerleriIcmaller[AktifIsyeri].ContainsKey("Tumu")) tumIsyerleriIcmaller[AktifIsyeri].Remove("Tumu");

                                                    tumIsyerleriIcmaller[AktifIsyeri].Add("Tumu", ayTumTesviklerToplam);
                                                }

                                                icmalDegerYazilacakAlanlar["Tumu"][yil + "-" + ay].Value2 = ayTumTesviklerToplam.ToTL();

                                                j++;
                                            }

                                            decimal yiltumTesviklerToplam = 0;

                                            foreach (var kanun in TesvikVerilecekKanunlar)
                                            {
                                                icmalDegerYazilacakAlanlar[kanun][yil.ToString()].Value2 = yilToplamlari[kanun].ToTL();

                                                yiltumTesviklerToplam += yilToplamlari[kanun].ToTL().Replace("₺", "").ToDecimalSgk();
                                            }

                                            icmalDegerYazilacakAlanlar["Tumu"][yil.ToString()].Value2 = yiltumTesviklerToplam.ToTL();

                                            i++;
                                        }

                                        decimal tumTesviklerToplam = 0;

                                        //if (tumIsyerleriIcmaller.ContainsKey(AktifIsyeri)) tumIsyerleriIcmaller.Remove(AktifIsyeri);

                                        //tumIsyerleriIcmaller.Add(AktifIsyeri, Program.TumTesvikler.ToDictionary(x => x.Key, x => 0d));

                                        foreach (var kanun in TesvikVerilecekKanunlar)
                                        {
                                            icmalDegerYazilacakAlanlar[kanun]["Tumu"].Value2 = tumToplamlar[kanun].ToTL();

                                            //if (tumIsyerleriIcmaller[AktifIsyeri].ContainsKey(kanun)) tumIsyerleriIcmaller[AktifIsyeri].Remove(kanun);

                                            //tumIsyerleriIcmaller[AktifIsyeri].Add(kanun, tumToplamlar[kanun]);

                                            tumTesviklerToplam += tumToplamlar[kanun].ToTL().Replace("₺", "").ToDecimalSgk();
                                        }

                                        //if (tumIsyerleriIcmaller[AktifIsyeri].ContainsKey("Tumu")) tumIsyerleriIcmaller[AktifIsyeri].Remove("Tumu");

                                        //tumIsyerleriIcmaller[AktifIsyeri].Add("Tumu", tumTesviklerToplam);

                                        icmalDegerYazilacakAlanlar["Tumu"]["Tumu"].Value2 = tumTesviklerToplam.ToTL();

                                        MyBook.SaveAs(Path.Combine(isyeriSavePath, "Icmal Genel.xlsx"));

                                        basariliolanlar.Add(Path.Combine(isyeriSavePath, "Icmal Genel.xlsx"));

                                    }
                                    catch
                                    {
                                        hatalar.Add(Path.Combine(isyeriSavePath, "Icmal Genel.xlsx"));
                                    }

                                    MyBook.Close(false);

                                    Genel.IcmalKaydediliyorKilidiniKaldir();
                                }

                                #endregion

                                var tumUstyaziaylar = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => "");

                                #region Kanun Icmal Dosyası Oluşturma

                                foreach (var kanun in OlusturulacakIcmaller)
                                {
                                    var tesvik = TumTesvikler[kanun];

                                    string ustyaziaylar = tumUstyaziaylar[kanun];

                                    string IcmalBaslik1 = tesvik.IcmalBaslik;

                                    string IcmalBaslik2 = IcmalOlusturmaSabitleri.IcmalBaslik2Metin; ;

                                    List<string> MahsupYapilacakIcmalKanunlari = new List<string>();

                                    SortedDictionary<int, SortedDictionary<int, Classes.Icmal>> yillar = new SortedDictionary<int, SortedDictionary<int, Classes.Icmal>>();

                                    foreach (var tarih in icmalCikartilacakAylar)
                                    {

                                        if (!yillar.ContainsKey(tarih.Year)) yillar.Add(tarih.Year, new SortedDictionary<int, Classes.Icmal>());

                                        SortedDictionary<int, Classes.Icmal> aylar = yillar[tarih.Year];

                                        Classes.Icmal icmal = null;

                                        if (tesvik.TesvikAyIstatistikleri.ContainsKey(tarih))
                                        {
                                            icmal = tesvik.TesvikAyIstatistikleri[tarih].Icmal;
                                        }

                                        if (!aylar.ContainsKey(tarih.Month)) aylar.Add(tarih.Month, icmal);

                                    }

                                    foreach (var item in tesvik.TesvikAyIstatistikleri)
                                    {
                                        DateTime tarih = item.Key;
                                        var tesvikAyIstatistik = item.Value;

                                        int Yil = tarih.Year;

                                        int Ay = tarih.Month;

                                        foreach (var icmalTutarItem in tesvikAyIstatistik.Icmal.Tutarlar)
                                        {
                                            //if (icmalTutarItem.Value >= 0)
                                            {
                                                if (!MahsupYapilacakIcmalKanunlari.Contains(icmalTutarItem.Key)) MahsupYapilacakIcmalKanunlari.Add(icmalTutarItem.Key);
                                            }
                                        }

                                        if (tesvikAyIstatistik.Icmal.Tutarlar.Any(p => p.Value > 0))
                                        {
                                            ustyaziaylar += Yil.ToString() + "/" + Ay.ToString() + ",";
                                        }
                                    }

                                    var icmalDegerYazilacakAlanlar = new Dictionary<string, Excel2.Range>();

                                    var workbooks = MyApp.Workbooks;

                                    Genel.IcmalKaydediliyorKontrolu();

                                    Excel2.Workbook MyBook = workbooks.Open(Path.Combine(Application.StartupPath, "Icmal.xlsx")); ;

                                    Excel2.Worksheet MySheet = (Excel2.Worksheet)MyBook.Sheets[1]; // Explicit cast is not required here;

                                    var IcmalIsyeriAdKanun = MySheet.Range[IcmalOlusturmaSabitleri.IcmalIsyeriAd];
                                    var IcmalBaslik1Kanun = MySheet.Range[IcmalOlusturmaSabitleri.IcmalBaslik1];
                                    var IcmalBaslik2Kanun = MySheet.Range[IcmalOlusturmaSabitleri.IcmalBaslik2];
                                    var IcmalIsyeriSicilKanun = MySheet.Range[IcmalOlusturmaSabitleri.IcmalIsyeriSicil];

                                    IcmalIsyeriAdKanun.Value2 = IsyeriAd.ToUpper();

                                    IcmalBaslik1Kanun.Value2 = IcmalBaslik1;

                                    IcmalBaslik2Kanun.Value2 = IcmalBaslik2;

                                    IcmalIsyeriSicilKanun.Value2 = " " + IsyeriSicilNo;

                                    try
                                    {
                                        List<string> isyerisicils = new List<string>();

                                        isyerisicils.Add(IsyeriSicilNo.Substring(0, 1));

                                        isyerisicils.Add(IsyeriSicilNo.Substring(1, 4));

                                        isyerisicils.Add(IsyeriSicilNo.Substring(5, 2));

                                        isyerisicils.Add(IsyeriSicilNo.Substring(7, 2));

                                        isyerisicils.Add(IsyeriSicilNo.Substring(9, 7));

                                        isyerisicils.Add(IsyeriSicilNo.Substring(16, 3));

                                        isyerisicils.Add(IsyeriSicilNo.Substring(19, 2));

                                        string isyerisicilno = String.Join(" ", isyerisicils.ToArray()).Trim();

                                        isyerisicilno += "-" + IsyeriSicilNo.Substring(21, 2);

                                        IcmalIsyeriSicilKanun.Value2 = isyerisicilno;

                                    }
                                    catch
                                    {

                                    }

                                    HafizadanAtilacaklar.AddRange(new List<object> { workbooks, MyBook, MySheet, IcmalIsyeriAdKanun, IcmalBaslik1Kanun, IcmalBaslik2Kanun, IcmalIsyeriSicilKanun });

                                    List<Excel2.Range> headers = new List<Excel2.Range>();

                                    List<Excel2.Range> rows = new List<Excel2.Range>();

                                    List<Excel2.Range> yiltoplamlari = new List<Excel2.Range>();

                                    int Satir = IcmalOlusturmaSabitleri.IcmalBaslangicSatir;

                                    int CiftSutun = IcmalOlusturmaSabitleri.IcmalCiftBaslangicSutun;

                                    int TekSutun = IcmalOlusturmaSabitleri.IcmalTekBaslangicSutun;

                                    var enumeratoryil = yillar.GetEnumerator();

                                    int i = 0;

                                    while (enumeratoryil.MoveNext())
                                    {
                                        int yil = enumeratoryil.Current.Key;

                                        bool Cift = i % 2 == 0;

                                        int Sutun = Cift ? CiftSutun : TekSutun;

                                        Excel2.Range headerDonem = MySheet.Cells[Satir + (i / 2) * 15, Sutun] as Excel2.Range;

                                        headerDonem.Value2 = "DÖNEM";

                                        Excel2.Range headerGun = MySheet.Cells[Satir + (i / 2) * 15, Sutun + 1] as Excel2.Range;

                                        if (!tesvik.KanunIcmalindeGunGosterilsin) headerGun.Value2 = "MATRAH";
                                        else headerGun.Value2 = "GÜN";

                                        Excel2.Range headerTutar = MySheet.Cells[Satir + (i / 2) * 15, Sutun + 2] as Excel2.Range;

                                        headerTutar.Value2 = "TUTAR";

                                        var headerrow = MySheet.Range[headerDonem, headerTutar];

                                        headers.Add(headerrow);
                                        //headers.AddRange(new List<Excel2.Range> { headerDonem, headerGun, headerTutar });
                                        HafizadanAtilacaklar.AddRange(new List<object> { headerDonem, headerGun, headerTutar, headerrow });

                                        var enumeratoray = enumeratoryil.Current.Value.GetEnumerator();

                                        int j = 1;

                                        while (enumeratoray.MoveNext())
                                        {
                                            int ay = enumeratoray.Current.Key;

                                            Classes.Icmal icmal = enumeratoray.Current.Value;

                                            Excel2.Range cellyilay = MySheet.Cells[Satir + j + (i / 2) * 15, Sutun] as Excel2.Range;

                                            cellyilay.Value2 = yil.ToString() + "/" + ay.ToString();

                                            Excel2.Range cellGun = MySheet.Cells[Satir + j + (i / 2) * 15, Sutun + 1] as Excel2.Range;

                                            if (!tesvik.KanunIcmalindeGunGosterilsin) cellGun.Value2 = (icmal == null ? 0 : icmal.Matrah).ToTL();
                                            else cellGun.Value2 = icmal == null ? "0" : icmal.PrimOdenenGunSayisi.ToString();

                                            Excel2.Range cellTutar = MySheet.Cells[Satir + j + (i / 2) * 15, Sutun + 2] as Excel2.Range;

                                            icmalDegerYazilacakAlanlar.Add(yil + "-" + ay + "-GUNVEYAMATRAH", cellGun);
                                            icmalDegerYazilacakAlanlar.Add(yil + "-" + ay + "-TUTAR", cellTutar);

                                            HafizadanAtilacaklar.AddRange(new List<object> { cellyilay, cellGun, cellTutar });
                                            //rows.AddRange(new List<Excel2.Range> { cellyilay, cellGun, cellTutar });

                                            j++;
                                        }

                                        var baslangicyil = MySheet.Cells[Satir + 1 + (i / 2) * 15, Sutun];
                                        var bitisyil = MySheet.Cells[Satir + j - 1 + (i / 2) * 15, Sutun + 2];

                                        var allrows = MySheet.Range[baslangicyil, bitisyil];

                                        rows.Add(allrows);

                                        HafizadanAtilacaklar.AddRange(new List<object> { allrows, baslangicyil, bitisyil });

                                        Excel2.Range cellyiltoplam = MySheet.Cells[Satir + j + (i / 2) * 15, Sutun] as Excel2.Range;

                                        cellyiltoplam.Value2 = "Yıl toplamı";

                                        Excel2.Range cellYilToplamGun = MySheet.Cells[Satir + j + (i / 2) * 15, Sutun + 1] as Excel2.Range;

                                        Excel2.Range cellYilToplamTutar = MySheet.Cells[Satir + j + (i / 2) * 15, Sutun + 2] as Excel2.Range;

                                        icmalDegerYazilacakAlanlar.Add(yil + "-GUNVEYAMATRAH", cellYilToplamGun);
                                        icmalDegerYazilacakAlanlar.Add(yil + "-TUTAR", cellYilToplamTutar);

                                        var yiltoplamrow = MySheet.Range[cellyiltoplam, cellYilToplamTutar];

                                        yiltoplamlari.Add(yiltoplamrow);
                                        HafizadanAtilacaklar.AddRange(new List<object> { cellyiltoplam, cellYilToplamGun, cellYilToplamTutar, yiltoplamrow });

                                        i++;

                                    }


                                    foreach (Excel2.Range r in headers)
                                    {
                                        var font = r.Font;

                                        font.Bold = true;

                                        font.Name = "Times New Roman";

                                        font.Size = 12;

                                        r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                                        r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                        var interior = r.Interior;

                                        interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(234, 241, 221));

                                        //r.BorderAround(Excel2.XlLineStyle.xlContinuous);

                                        var borders = r.Borders;

                                        borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                        HafizadanAtilacaklar.AddRange(new List<object> { r, font, interior, borders });

                                    }

                                    foreach (Excel2.Range r in rows)
                                    {
                                        var font = r.Font;

                                        font.Bold = false;

                                        font.Name = "Times New Roman";

                                        font.Size = 12;

                                        r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                                        r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignRight;

                                        var interior = r.Interior;

                                        interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(197, 217, 241));

                                        var borders = r.Borders;

                                        borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                        HafizadanAtilacaklar.AddRange(new List<object> { r, font, interior, borders });

                                    }

                                    foreach (Excel2.Range r in yiltoplamlari)
                                    {
                                        var font = r.Font;

                                        font.Bold = true;

                                        font.Name = "Times New Roman";

                                        font.Size = 12;

                                        r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                                        r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignRight;

                                        var borders = r.Borders;

                                        borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                        HafizadanAtilacaklar.AddRange(new List<object> { r, font, borders });
                                    }


                                    int str = Satir + (i / 2) * 15;

                                    int stn = i % 2 == 0 ? CiftSutun : TekSutun;

                                    var rangebaslangic = MySheet.Cells[str, stn];

                                    var rangebitis = MySheet.Cells[str + 6, stn + 2];

                                    Excel2.Range range = MySheet.Range[rangebaslangic, rangebitis];

                                    var rangetutarbaslangic = MySheet.Cells[str + 7, stn];

                                    var rangetutarbitis = MySheet.Cells[str + 10, stn + 2];

                                    Excel2.Range rangetutar = MySheet.Range[rangetutarbaslangic, rangetutarbitis];

                                    icmalDegerYazilacakAlanlar.Add("Tumu", rangetutar);

                                    range.Merge();

                                    range.Value2 = "Teşvik kapsamında işveren tarafından iade alınacak olan toplam prim tutarı(kanuni faiz hariç)";

                                    var fontbaslik = range.Font;

                                    fontbaslik.Bold = true;

                                    fontbaslik.Name = "Times New Roman";

                                    fontbaslik.Size = 12;

                                    range.WrapText = true;

                                    range.VerticalAlignment = 2;

                                    range.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                    var interiorbaslik = range.Interior;

                                    interiorbaslik.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(217, 151, 149));

                                    range.BorderAround(Excel2.XlLineStyle.xlContinuous, Excel2.XlBorderWeight.xlMedium);

                                    rangetutar.Merge();

                                    var fonttutar = rangetutar.Font;

                                    fonttutar.Bold = true;

                                    fonttutar.Name = "Times New Roman";

                                    fonttutar.Size = 12;

                                    rangetutar.WrapText = true;

                                    rangetutar.VerticalAlignment = 2;

                                    rangetutar.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                    var interiortutar = rangetutar.Interior;

                                    interiortutar.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(217, 151, 149));

                                    rangetutar.BorderAround(Excel2.XlLineStyle.xlContinuous, Excel2.XlBorderWeight.xlMedium);

                                    HafizadanAtilacaklar.AddRange(new List<object> { range, rangetutar, fontbaslik, fonttutar, interiorbaslik, interiortutar, rangebaslangic, rangebitis, rangetutarbaslangic, rangetutarbitis });

                                    var dusulecekKanunlar = new List<string>();

                                    foreach (var mahsupKanun in MahsupYapilacakIcmalKanunlari)
                                    {
                                        var bagliKanunlar = Metodlar.BagliKanunlariGetir(mahsupKanun);

                                        dusulecekKanunlar.AddRange(bagliKanunlar);

                                        dusulecekKanunlar.Add(mahsupKanun);
                                    }

                                    dusulecekKanunlar = dusulecekKanunlar.Distinct().ToList();


                                    if (!Directory.Exists(isyeriSavePath)) Directory.CreateDirectory(isyeriSavePath);

                                    try
                                    {
                                        decimal ToplamTutar = 0;

                                        enumeratoryil = yillar.GetEnumerator();

                                        i = 0;

                                        while (enumeratoryil.MoveNext())
                                        {
                                            decimal YilToplam = 0m;
                                            decimal YilToplamMatrah = 0m;
                                            int YilToplamGun = 0;

                                            int yil = enumeratoryil.Current.Key;

                                            int j = 1;

                                            var enumeratoray = enumeratoryil.Current.Value.GetEnumerator();

                                            while (enumeratoray.MoveNext())
                                            {
                                                int ay = enumeratoray.Current.Key;

                                                DateTime tarih = new DateTime(yil, ay, 1);

                                                Classes.Icmal icmal = tesvik.TesvikAyIstatistikleri.ContainsKey(tarih) ? tesvik.TesvikAyIstatistikleri[tarih].Icmal : null;

                                                decimal IcmalTutar = 0m;
                                                decimal IcmalMatrah = 0m;
                                                int IcmalGun = 0;

                                                if (icmal != null && icmal.Tutarlar.Count > 0)
                                                {
                                                    IcmalTutar = icmal.TutarlarBagliKanunlarMahsupluTutarlar["00000"];

                                                    //if (icmal.TutarlarBagliKanunlarMahsupTutarlari.ContainsKey(mahsupKanun))
                                                    //{
                                                    //    IcmalTutar -= icmal.TutarlarBagliKanunlarMahsupTutarlari[mahsupKanun];
                                                    //}

                                                    foreach (var dusulecekKanun in dusulecekKanunlar)
                                                    {
                                                        if (icmal.TutarlarBagliKanunlarMahsupTutarlari.ContainsKey(dusulecekKanun))
                                                        {
                                                            IcmalTutar -= icmal.TutarlarBagliKanunlarMahsupTutarlari[dusulecekKanun];
                                                        }
                                                    }

                                                    IcmalMatrah = icmal.Matrah;
                                                    IcmalGun = icmal.PrimOdenenGunSayisi;

                                                }

                                                icmalDegerYazilacakAlanlar[yil + "-" + ay + "-TUTAR"].Value2 = IcmalTutar.ToTL();
                                                icmalDegerYazilacakAlanlar[yil + "-" + ay + "-GUNVEYAMATRAH"].Value2 = tesvik.KanunIcmalindeGunGosterilsin ? IcmalGun.ToString() : IcmalMatrah.ToTL();

                                                YilToplam += IcmalTutar.ToTL().Replace("₺", "").ToDecimalSgk();
                                                ToplamTutar += IcmalTutar.ToTL().Replace("₺", "").ToDecimalSgk();

                                                YilToplamMatrah += IcmalMatrah;
                                                YilToplamGun += IcmalGun;


                                                j++;
                                            }

                                            icmalDegerYazilacakAlanlar[yil.ToString() + "-TUTAR"].Value2 = YilToplam.ToTL();
                                            icmalDegerYazilacakAlanlar[yil.ToString() + "-GUNVEYAMATRAH"].Value2 = tesvik.KanunIcmalindeGunGosterilsin ? YilToplamGun.ToString() : YilToplamMatrah.ToTL();

                                            i++;
                                        }


                                        icmalDegerYazilacakAlanlar["Tumu"].Value2 = ToplamTutar.ToTL();

                                        MyBook.SaveAs(Path.Combine(isyeriSavePath, String.Format("Icmal {0}.xlsx", kanun.Replace("/", "-"))));

                                        basariliolanlar.Add(Path.Combine(isyeriSavePath, String.Format("Icmal {0}.xlsx", kanun.Replace("/", "-"))));
                                    }
                                    catch
                                    {
                                        hatalar.Add(Path.Combine(isyeriSavePath, String.Format("Icmal {0}.xlsx", kanun.Replace("/", ""))));
                                    }

                                    MyBook.Close(false);

                                    Genel.IcmalKaydediliyorKilidiniKaldir();

                                    #region Aylık Çalışan


                                    if (formBildirgeOlustur.chkAylikCalisan.Checked && tesvik.BasvuruFormuVar && BasvuruFormlariSutunlari[kanun].ContainsKey(Enums.BasvuruFormuSutunTurleri.Baz) && !tesvik.Basvuru_Formundaki_Baz_Sayisi_Kadar_Gun_Ay_Icinde_TesvikAlabilir)
                                    {
                                        var workbooksaylikcalisan = MyApp.Workbooks;

                                        Excel2.Workbook MyBookAylikCalisan = workbooksaylikcalisan.Open(Path.Combine(Application.StartupPath, "AylikCalisanIcmal.xlsx"));

                                        Excel2.Worksheet MySheetAylikCalisan = (Excel2.Worksheet)MyBookAylikCalisan.Sheets[1]; // Explicit cast is not required here

                                        var IcmalIsyeriAdAylikCalisan = MySheetAylikCalisan.Range[IcmalOlusturmaSabitleri.IcmalIsyeriAd];

                                        var IcmalIsyeriSicil = MySheetAylikCalisan.Range[IcmalOlusturmaSabitleri.IcmalIsyeriSicil];

                                        IcmalIsyeriAdAylikCalisan.Value2 = IsyeriAd.ToUpper();

                                        IcmalIsyeriSicil.Value2 = " " + IsyeriSicilNo;

                                        try
                                        {
                                            List<string> isyerisicils = new List<string>();

                                            isyerisicils.Add(IsyeriSicilNo.Substring(0, 1));

                                            isyerisicils.Add(IsyeriSicilNo.Substring(1, 4));

                                            isyerisicils.Add(IsyeriSicilNo.Substring(5, 2));

                                            isyerisicils.Add(IsyeriSicilNo.Substring(7, 2));

                                            isyerisicils.Add(IsyeriSicilNo.Substring(9, 7));

                                            isyerisicils.Add(IsyeriSicilNo.Substring(16, 3));

                                            isyerisicils.Add(IsyeriSicilNo.Substring(19, 2));

                                            string isyerisicilno = String.Join(" ", isyerisicils.ToArray()).Trim();

                                            isyerisicilno += "-" + IsyeriSicilNo.Substring(21, 2);

                                            IcmalIsyeriSicil.Value2 = isyerisicilno;

                                        }
                                        catch
                                        {

                                        }

                                        HafizadanAtilacaklar.AddRange(new List<object> { workbooksaylikcalisan, MyBookAylikCalisan, MySheetAylikCalisan, IcmalIsyeriAdAylikCalisan, IcmalIsyeriSicil });

                                        headers = new List<Excel2.Range>();

                                        rows = new List<Excel2.Range>();

                                        List<Excel2.Range> bazrows = new List<Excel2.Range>();

                                        List<Excel2.Range> yellows = new List<Excel2.Range>();

                                        List<Excel2.Range> reds = new List<Excel2.Range>();

                                        List<Excel2.Range> grays = new List<Excel2.Range>();

                                        yiltoplamlari = new List<Excel2.Range>();

                                        Satir = IcmalOlusturmaSabitleri.IcmalBaslangicSatir;

                                        CiftSutun = IcmalOlusturmaSabitleri.IcmalCiftBaslangicSutun;

                                        TekSutun = IcmalOlusturmaSabitleri.IcmalTekBaslangicSutun;

                                        enumeratoryil = yillar.GetEnumerator();

                                        i = 0;

                                        int lastrownum = 0;

                                        while (enumeratoryil.MoveNext())
                                        {
                                            int yil = enumeratoryil.Current.Key;

                                            bool Cift = i % 2 == 0;

                                            int Sutun2 = Cift ? CiftSutun : TekSutun;

                                            Excel2.Range headerDonem = MySheetAylikCalisan.Cells[Satir + (i / 2) * 15, Sutun2] as Excel2.Range;

                                            headerDonem.Value2 = "DÖNEM";

                                            Excel2.Range headerAylikCalisan = MySheetAylikCalisan.Cells[Satir + (i / 2) * 15, Sutun2 + 1] as Excel2.Range;

                                            headerAylikCalisan.Value2 = "PERSONEL SAYISI";

                                            Excel2.Range headerHesaplananBazSayisi = MySheetAylikCalisan.Cells[Satir + (i / 2) * 15, Sutun2 + 2] as Excel2.Range;

                                            headerHesaplananBazSayisi.Value2 = "HESAPLANAN BAZ SAYISI";

                                            Excel2.Range headerSgkBazSayisi = MySheetAylikCalisan.Cells[Satir + (i / 2) * 15, Sutun2 + 3] as Excel2.Range;

                                            headerSgkBazSayisi.Value2 = "SGK BAZ SAYISI";

                                            var allheaders = MySheetAylikCalisan.Range[headerDonem, headerSgkBazSayisi];

                                            headers.Add(allheaders);

                                            HafizadanAtilacaklar.AddRange(new List<object> { headerDonem, headerAylikCalisan, headerHesaplananBazSayisi, headerSgkBazSayisi, allheaders });

                                            //headers.AddRange(new List<Excel2.Range> { headerDonem, headerAylikCalisan, headerHesaplananBazSayisi, headerSgkBazSayisi });

                                            var enumeratoray = enumeratoryil.Current.Value.GetEnumerator();

                                            int j = 1;

                                            while (enumeratoray.MoveNext())
                                            {
                                                int ay = enumeratoray.Current.Key;

                                                Excel2.Range cellYilAy = MySheetAylikCalisan.Cells[Satir + j + (i / 2) * 15, Sutun2] as Excel2.Range;

                                                cellYilAy.Value2 = yil.ToString() + "/" + ay.ToString();

                                                Excel2.Range cellAylikCalisan = MySheetAylikCalisan.Cells[Satir + j + (i / 2) * 15, Sutun2 + 1] as Excel2.Range;

                                                DateTime tarih = new DateTime(yil, ay, 1);

                                                cellAylikCalisan.Value2 = AyCalisanSayilari.ContainsKey(tarih) && AyCalisanSayilari[tarih].ContainsKey(kanun) ? AyCalisanSayilari[tarih][kanun].ToString() : null;

                                                long AyBazSayisi = Metodlar.BazHesapla(yil, ay, kanun, TumKisilerSonuc, ref AyCalisanSayilari, ref AyCalisanSayilariBazHesaplama);

                                                Excel2.Range cellHesaplananBaz = MySheetAylikCalisan.Cells[Satir + j + (i / 2) * 15, Sutun2 + 2] as Excel2.Range;

                                                cellHesaplananBaz.Value2 = AyBazSayisi.ToString();

                                                var bazanahtar = (tesvik.BazYil ? yil.ToString() : "1") + "-" + (tesvik.BazAy ? ay.ToString() : "1");

                                                int SGKBazSayisi = tesvik.BazSayilari.ContainsKey(bazanahtar) ? tesvik.BazSayilari[bazanahtar] : -1;

                                                Excel2.Range cellSgkBaz = MySheetAylikCalisan.Cells[Satir + j + (i / 2) * 15, Sutun2 + 3] as Excel2.Range;

                                                cellSgkBaz.Value2 = SGKBazSayisi != -1 ? SGKBazSayisi.ToString() : "";

                                                HafizadanAtilacaklar.AddRange(new List<object> { cellYilAy, cellAylikCalisan, cellHesaplananBaz, cellSgkBaz });

                                                if (SGKBazSayisi == -1)
                                                {
                                                    grays.Add(cellHesaplananBaz);
                                                }
                                                else if (AyBazSayisi > SGKBazSayisi)
                                                {
                                                    reds.Add(cellHesaplananBaz);
                                                }
                                                else if (AyBazSayisi < SGKBazSayisi)
                                                {
                                                    yellows.Add(cellHesaplananBaz);
                                                }

                                                lastrownum = (Satir + j + (i / 2) * 15) > lastrownum ? (Satir + j + (i / 2) * 15) : lastrownum;

                                                j++;
                                            }

                                            var baslangicrowaylikcalisan = MySheetAylikCalisan.Cells[Satir + 1 + (i / 2) * 15, Sutun2];
                                            var bitisrowaylikcalisan = MySheetAylikCalisan.Cells[Satir + j - 1 + (i / 2) * 15, Sutun2 + 3];

                                            var allrow = MySheetAylikCalisan.Range[baslangicrowaylikcalisan, bitisrowaylikcalisan];

                                            rows.Add(allrow);

                                            HafizadanAtilacaklar.AddRange(new List<object> { allrow, baslangicrowaylikcalisan, bitisrowaylikcalisan });

                                            i++;
                                        }


                                        stn = CiftSutun;

                                        int rownum = lastrownum + 2;

                                        var cell1 = MySheetAylikCalisan.Cells[rownum, stn] as Excel2.Range;

                                        cell1.Value2 = "";

                                        reds.Add(cell1);

                                        var cellHesaplananBazSayisiYuksek = MySheetAylikCalisan.Cells[rownum, stn + 1] as Excel2.Range;

                                        cellHesaplananBazSayisiYuksek.Value2 = "Hesaplanan baz sayısı SGK baz sayısından yüksek";

                                        var cell2 = MySheetAylikCalisan.Cells[rownum + 1, stn] as Excel2.Range;

                                        cell2.Value2 = "";

                                        yellows.Add(cell2);

                                        var cellHesaplananBazSayisiDusuk = MySheetAylikCalisan.Cells[rownum + 1, stn + 1] as Excel2.Range;

                                        cellHesaplananBazSayisiDusuk.Value2 = "Hesaplanan baz sayısı SGK baz sayısından düşük";

                                        var cell3 = MySheetAylikCalisan.Cells[rownum + 2, stn] as Excel2.Range;

                                        cell3.Value2 = "";

                                        grays.Add(cell3);

                                        var cellSgkBazSayisiBulunamadi = MySheetAylikCalisan.Cells[rownum + 2, stn + 1] as Excel2.Range;

                                        cellSgkBazSayisiBulunamadi.Value2 = "SGK baz sayısı bulunamadı";

                                        HafizadanAtilacaklar.AddRange(new List<object> { cell1, cell2, cell3, cellHesaplananBazSayisiYuksek, cellHesaplananBazSayisiDusuk, cellSgkBazSayisiBulunamadi });

                                        foreach (Excel2.Range r in headers)
                                        {
                                            var font = r.Font;

                                            font.Bold = true;

                                            font.Name = "Times New Roman";

                                            font.Size = 12;

                                            r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                                            r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                            var interior = r.Interior;

                                            interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(234, 241, 221));

                                            var borders = r.Borders;

                                            borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                            HafizadanAtilacaklar.AddRange(new List<object> { r, font, interior, borders });


                                        }

                                        foreach (Excel2.Range r in rows)
                                        {
                                            var font = r.Font;

                                            font.Bold = false;

                                            font.Name = "Times New Roman";

                                            font.Size = 12;

                                            r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                                            r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignRight;

                                            var interior = r.Interior;

                                            interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(197, 217, 241));

                                            var borders = r.Borders;

                                            borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                            HafizadanAtilacaklar.AddRange(new List<object> { r, font, interior, borders });


                                        }

                                        foreach (Excel2.Range r in yiltoplamlari)
                                        {
                                            var font = r.Font;

                                            font.Bold = true;

                                            font.Name = "Times New Roman";

                                            font.Size = 12;

                                            r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                                            r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignRight;

                                            var borders = r.Borders;

                                            borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                            HafizadanAtilacaklar.AddRange(new List<object> { r, font, borders });
                                        }

                                        foreach (Excel2.Range r in yellows)
                                        {
                                            var interior = r.Interior;

                                            interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);

                                            HafizadanAtilacaklar.Add(interior);
                                        }

                                        foreach (Excel2.Range r in reds)
                                        {
                                            var interior = r.Interior;

                                            interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);

                                            HafizadanAtilacaklar.Add(interior);
                                        }

                                        foreach (Excel2.Range r in grays)
                                        {
                                            var interior = r.Interior;

                                            interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);

                                            HafizadanAtilacaklar.Add(interior);
                                        }

                                        if (!Directory.Exists(isyeriSavePath)) Directory.CreateDirectory(isyeriSavePath);

                                        try
                                        {
                                            MyBookAylikCalisan.SaveAs(Path.Combine(isyeriSavePath, String.Format("Aylık Çalışan Personel Sayısı İcmal {0}.xlsx", kanun)));

                                        }
                                        catch
                                        {
                                            hatalar.Add(Path.Combine(isyeriSavePath, String.Format("Aylık Çalışan Personel Sayısı İcmal {0}.xlsx", kanun)));
                                        }

                                        MyBookAylikCalisan.Close(false);

                                    }

                                    #endregion
                                }

                                #endregion

                                #region Asgari Ücret Desteği Icmali Oluşturma

                                if (AsgariUcretDestegiIcmalleri.Any(p => p.Value > 0))
                                {
                                    foreach (var kv in AsgariUcretDestegiIcmalleri)
                                    {

                                        string IcmalBaslik1 = "BOZULAN ASGARİ ÜCRET DESTEĞİNİ GÖSTERİR İCMAL";

                                        List<string> MahsupYapilacakIcmalKanunlari = new List<string>();

                                        SortedDictionary<int, SortedDictionary<int, Classes.Icmal>> yillar = new SortedDictionary<int, SortedDictionary<int, Classes.Icmal>>();

                                        foreach (var audi in AsgariUcretDestegiIcmalleri)
                                        {
                                            var tarih = audi.Key;

                                            if (!yillar.ContainsKey(tarih.Year)) yillar.Add(tarih.Year, new SortedDictionary<int, Classes.Icmal>());

                                            SortedDictionary<int, Classes.Icmal> aylar = yillar[tarih.Year];

                                            Classes.Icmal icmal = new Icmal
                                            {
                                                Matrah = audi.Value
                                            };

                                            if (!aylar.ContainsKey(tarih.Month)) aylar.Add(tarih.Month, icmal);

                                        }


                                        var icmalDegerYazilacakAlanlar = new Dictionary<string, Excel2.Range>();

                                        if (MyApp == null)
                                        {

                                            MyApp = new Excel2.Application();

                                            MyApp.Visible = false;

                                            MyApp.DisplayAlerts = false;

                                            excelprocessid = Metodlar.GetExcelProcessId(MyApp);
                                        }

                                        var workbooks = MyApp.Workbooks;

                                        Genel.IcmalKaydediliyorKontrolu();

                                        Excel2.Workbook MyBook = workbooks.Open(Path.Combine(Application.StartupPath, "Icmal.xlsx")); ;

                                        Excel2.Worksheet MySheet = (Excel2.Worksheet)MyBook.Sheets[1]; // Explicit cast is not required here;

                                        var IcmalIsyeriAdKanun = MySheet.Range[IcmalOlusturmaSabitleri.IcmalIsyeriAd];
                                        var IcmalBaslik1Kanun = MySheet.Range[IcmalOlusturmaSabitleri.IcmalBaslik1];
                                        var IcmalBaslik2Kanun = MySheet.Range[IcmalOlusturmaSabitleri.IcmalBaslik2];
                                        var IcmalIsyeriSicilKanun = MySheet.Range[IcmalOlusturmaSabitleri.IcmalIsyeriSicil];

                                        IcmalIsyeriAdKanun.Value2 = IsyeriAd.ToUpper();

                                        IcmalBaslik1Kanun.Value2 = IcmalBaslik1;
                                        IcmalBaslik2Kanun.Value2 = "";

                                        IcmalIsyeriSicilKanun.Value2 = " " + IsyeriSicilNo;

                                        try
                                        {
                                            List<string> isyerisicils = new List<string>();

                                            isyerisicils.Add(IsyeriSicilNo.Substring(0, 1));

                                            isyerisicils.Add(IsyeriSicilNo.Substring(1, 4));

                                            isyerisicils.Add(IsyeriSicilNo.Substring(5, 2));

                                            isyerisicils.Add(IsyeriSicilNo.Substring(7, 2));

                                            isyerisicils.Add(IsyeriSicilNo.Substring(9, 7));

                                            isyerisicils.Add(IsyeriSicilNo.Substring(16, 3));

                                            isyerisicils.Add(IsyeriSicilNo.Substring(19, 2));

                                            string isyerisicilno = String.Join(" ", isyerisicils.ToArray()).Trim();

                                            isyerisicilno += "-" + IsyeriSicilNo.Substring(21, 2);

                                            IcmalIsyeriSicilKanun.Value2 = isyerisicilno;

                                        }
                                        catch
                                        {

                                        }

                                        HafizadanAtilacaklar.AddRange(new List<object> { workbooks, MyBook, MySheet, IcmalIsyeriAdKanun, IcmalBaslik1Kanun, IcmalBaslik2Kanun, IcmalIsyeriSicilKanun });

                                        List<Excel2.Range> headers = new List<Excel2.Range>();

                                        List<Excel2.Range> rows = new List<Excel2.Range>();

                                        List<Excel2.Range> yiltoplamlari = new List<Excel2.Range>();

                                        int Satir = IcmalOlusturmaSabitleri.IcmalBaslangicSatir;

                                        int CiftSutun = IcmalOlusturmaSabitleri.IcmalCiftBaslangicSutun;

                                        int TekSutun = IcmalOlusturmaSabitleri.IcmalTekBaslangicSutun;

                                        var enumeratoryil = yillar.GetEnumerator();

                                        int i = 0;

                                        while (enumeratoryil.MoveNext())
                                        {
                                            int yil = enumeratoryil.Current.Key;

                                            bool Cift = i % 2 == 0;

                                            int Sutun = Cift ? CiftSutun : TekSutun;

                                            Excel2.Range headerDonem = MySheet.Cells[Satir + (i / 2) * 15, Sutun] as Excel2.Range;

                                            headerDonem.Value2 = "DÖNEM";

                                            Excel2.Range headerTutar = MySheet.Cells[Satir + (i / 2) * 15, Sutun + 1] as Excel2.Range;

                                            headerTutar.Value2 = "TUTAR (TL)";

                                            var headerrow = MySheet.Range[headerDonem, headerTutar];

                                            headers.Add(headerrow);
                                            //headers.AddRange(new List<Excel2.Range> { headerDonem, headerGun, headerTutar });
                                            HafizadanAtilacaklar.AddRange(new List<object> { headerDonem, headerTutar, headerrow });

                                            var enumeratoray = enumeratoryil.Current.Value.GetEnumerator();

                                            int j = 1;

                                            while (enumeratoray.MoveNext())
                                            {
                                                int ay = enumeratoray.Current.Key;

                                                Classes.Icmal icmal = enumeratoray.Current.Value;

                                                Excel2.Range cellyilay = MySheet.Cells[Satir + j + (i / 2) * 15, Sutun] as Excel2.Range;

                                                cellyilay.Value2 = yil.ToString() + "/" + ay.ToString();

                                                Excel2.Range cellTutar = MySheet.Cells[Satir + j + (i / 2) * 15, Sutun + 1] as Excel2.Range;

                                                icmalDegerYazilacakAlanlar.Add(yil + "-" + ay + "-TUTAR", cellTutar);

                                                HafizadanAtilacaklar.AddRange(new List<object> { cellyilay, cellTutar });

                                                j++;
                                            }

                                            var baslangicyil = MySheet.Cells[Satir + 1 + (i / 2) * 15, Sutun];
                                            var bitisyil = MySheet.Cells[Satir + j - 1 + (i / 2) * 15, Sutun + 1];

                                            var allrows = MySheet.Range[baslangicyil, bitisyil];

                                            rows.Add(allrows);

                                            HafizadanAtilacaklar.AddRange(new List<object> { allrows, baslangicyil, bitisyil });

                                            Excel2.Range cellyiltoplam = MySheet.Cells[Satir + j + (i / 2) * 15, Sutun] as Excel2.Range;

                                            cellyiltoplam.Value2 = "Yıl toplamı";

                                            Excel2.Range cellYilToplamTutar = MySheet.Cells[Satir + j + (i / 2) * 15, Sutun + 1] as Excel2.Range;

                                            icmalDegerYazilacakAlanlar.Add(yil + "-TUTAR", cellYilToplamTutar);

                                            var yiltoplamrow = MySheet.Range[cellyiltoplam, cellYilToplamTutar];

                                            yiltoplamlari.Add(yiltoplamrow);
                                            HafizadanAtilacaklar.AddRange(new List<object> { cellyiltoplam, cellYilToplamTutar, yiltoplamrow });

                                            i++;

                                        }


                                        foreach (Excel2.Range r in headers)
                                        {
                                            var font = r.Font;

                                            font.Bold = true;

                                            font.Name = "Times New Roman";

                                            font.Size = 12;

                                            r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                                            r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                            var interior = r.Interior;

                                            interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(234, 241, 221));

                                            //r.BorderAround(Excel2.XlLineStyle.xlContinuous);

                                            var borders = r.Borders;

                                            borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                            HafizadanAtilacaklar.AddRange(new List<object> { r, font, interior, borders });

                                        }

                                        foreach (Excel2.Range r in rows)
                                        {
                                            var font = r.Font;

                                            font.Bold = false;

                                            font.Name = "Times New Roman";

                                            font.Size = 12;

                                            r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                                            r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignRight;

                                            var interior = r.Interior;

                                            interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(197, 217, 241));

                                            var borders = r.Borders;

                                            borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                            HafizadanAtilacaklar.AddRange(new List<object> { r, font, interior, borders });

                                        }

                                        foreach (Excel2.Range r in yiltoplamlari)
                                        {
                                            var font = r.Font;

                                            font.Bold = true;

                                            font.Name = "Times New Roman";

                                            font.Size = 12;

                                            r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                                            r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignRight;

                                            var borders = r.Borders;

                                            borders.LineStyle = Excel2.XlLineStyle.xlContinuous;

                                            HafizadanAtilacaklar.AddRange(new List<object> { r, font, borders });
                                        }


                                        int str = Satir + (i / 2) * 15;

                                        int stn = i % 2 == 0 ? CiftSutun : TekSutun;

                                        var rangebaslangic = MySheet.Cells[str, stn];

                                        var rangebitis = MySheet.Cells[str + 2, stn + 1];

                                        Excel2.Range range = MySheet.Range[rangebaslangic, rangebitis];

                                        var rangetutarbaslangic = MySheet.Cells[str + 3, stn];

                                        var rangetutarbitis = MySheet.Cells[str + 5, stn + 1];

                                        Excel2.Range rangetutar = MySheet.Range[rangetutarbaslangic, rangetutarbitis];

                                        icmalDegerYazilacakAlanlar.Add("Tumu", rangetutar);

                                        range.Merge();

                                        range.Value2 = "Toplam (TL)";

                                        var fontbaslik = range.Font;

                                        fontbaslik.Bold = true;

                                        fontbaslik.Name = "Times New Roman";

                                        fontbaslik.Size = 12;

                                        range.WrapText = true;

                                        range.VerticalAlignment = 2;

                                        range.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                        var interiorbaslik = range.Interior;

                                        interiorbaslik.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(217, 151, 149));

                                        range.BorderAround(Excel2.XlLineStyle.xlContinuous, Excel2.XlBorderWeight.xlMedium);

                                        rangetutar.Merge();

                                        var fonttutar = rangetutar.Font;

                                        fonttutar.Bold = true;

                                        fonttutar.Name = "Times New Roman";

                                        fonttutar.Size = 12;

                                        rangetutar.WrapText = true;

                                        rangetutar.VerticalAlignment = 2;

                                        rangetutar.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                                        var interiortutar = rangetutar.Interior;

                                        interiortutar.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(217, 151, 149));

                                        rangetutar.BorderAround(Excel2.XlLineStyle.xlContinuous, Excel2.XlBorderWeight.xlMedium);

                                        HafizadanAtilacaklar.AddRange(new List<object> { range, rangetutar, fontbaslik, fonttutar, interiorbaslik, interiortutar, rangebaslangic, rangebitis, rangetutarbaslangic, rangetutarbitis });


                                        if (!Directory.Exists(isyeriSavePath)) Directory.CreateDirectory(isyeriSavePath);

                                        try
                                        {
                                            decimal ToplamTutar = 0;

                                            enumeratoryil = yillar.GetEnumerator();

                                            i = 0;

                                            while (enumeratoryil.MoveNext())
                                            {
                                                decimal YilToplam = 0;

                                                int yil = enumeratoryil.Current.Key;

                                                int j = 1;

                                                var enumeratoray = enumeratoryil.Current.Value.GetEnumerator();

                                                while (enumeratoray.MoveNext())
                                                {
                                                    int ay = enumeratoray.Current.Key;

                                                    DateTime tarih = new DateTime(yil, ay, 1);

                                                    Classes.Icmal icmal = yillar.ContainsKey(tarih.Year) ? yillar[tarih.Year][tarih.Month] : null;

                                                    decimal IcmalTutar = 0;

                                                    if (icmal != null)
                                                    {
                                                        IcmalTutar = icmal.Matrah;

                                                    }

                                                    icmalDegerYazilacakAlanlar[yil + "-" + ay + "-TUTAR"].Value2 = IcmalTutar.ToTL();

                                                    YilToplam += IcmalTutar.ToTL().Replace("₺", "").ToDecimalSgk();
                                                    ToplamTutar += IcmalTutar.ToTL().Replace("₺", "").ToDecimalSgk();

                                                    j++;
                                                }

                                                icmalDegerYazilacakAlanlar[yil.ToString() + "-TUTAR"].Value2 = YilToplam.ToTL();

                                                i++;
                                            }


                                            icmalDegerYazilacakAlanlar["Tumu"].Value2 = ToplamTutar.ToTL();

                                            MyBook.SaveAs(Path.Combine(isyeriSavePath, String.Format("Bozulan Asgari Ücret Desteği İcmali.xlsx")));

                                            basariliolanlar.Add(Path.Combine(isyeriSavePath, String.Format("Bozulan Asgari Ücret Desteği İcmali.xlsx")));
                                        }
                                        catch
                                        {
                                            hatalar.Add(Path.Combine(isyeriSavePath, String.Format("Bozulan Asgari Ücret Desteği İcmali.xlsx")));
                                        }

                                        MyBook.Close(false);

                                        Genel.IcmalKaydediliyorKilidiniKaldir();
                                    }
                                }

                                #endregion
                                #endregion

                                #region Word Belgelerini Oluşturma

                                if (formBildirgeOlustur.chkUstYazi.Checked)
                                {

                                    if (!SadeceIcmal && ToplamTesvikTuru > 0)
                                    {
                                        if (wordApp == null)
                                        {
                                            List<int> oldwords = Metodlar.GetProcessIdsSnapshot("WINWORD");

                                            wordApp = new Microsoft.Office.Interop.Word.Application { Visible = false };

                                            wordApp.Visible = false;

                                            List<int> newwords = Metodlar.GetProcessIdsSnapshot("WINWORD");

                                            wordprocessid = Metodlar.GetProcessId(oldwords, newwords);
                                        }


                                        List<string> yilaylar = TumTesvikler.SelectMany(p => p.Value.TesvikAyIstatistikleri).Select(p => p.Key.Year + "/" + p.Key.Month.ToString().PadLeft(2, '0')).Distinct().ToList();

                                        string Kanunlar = "";

                                        string Kanunlar2 = "";

                                        foreach (var tesvikItem in TumTesvikler)
                                        {

                                            var kanunNo = tesvikItem.Key;

                                            var kanunNolar = tumCiktilar.Where(p => !p.Iptal && p.ExcelOlustur && p.Kanun.EndsWith(kanunNo)).Select(p => p.Kanun).Distinct();

                                            if (kanunNolar.Count() > 0)
                                            {

                                                Kanunlar += "," + String.Join(",", kanunNolar);

                                                Kanunlar2 += "," + kanunNo;
                                            }

                                        }

                                        string IptalKanunlar = String.Join(",", TumTesvikler.SelectMany(p => p.Value.TesvikAyIstatistikleri).SelectMany(p => p.Value.Icmal.Tutarlar).Select(p => p.Key).Distinct());


                                        string outputname = "Üst Yazı " + Kanunlar2.Trim(',').Replace(",", "+") + ".docx";

                                        WordOlustur(wordApp, IsyeriSosyalGuvenlikKurumu, IsyeriAd, IsyeriSicilNo, tumCiktilar, Kanunlar.Trim(','), IptalKanunlar, "UstYazi.docx", outputname, yilaylar, ref hatalar, isyeriSavePath);

                                    }
                                }

                                #endregion

                            }
                            catch (Exception ex)
                            {
                                if (ex.Message != "Başvuru veya Aylık listede hatalı satırlar olduğu için devam edilemedi")
                                {
                                    if (!SadeceIcmal)
                                    {
                                        Metodlar.HataMesajiGoster(ex, "Bildirge oluşturulurken hata meydana geldi");
                                    }
                                    else Metodlar.HataMesajiGoster(ex, "İcmal oluşturulurken hata meydana geldi");
                                }

                                hataOlustu = true;

                                break;
                            }
                            finally
                            {
                                if (BildirgeWorkBook != null)
                                {
                                    BildirgeWorkBook.Close(false);

                                    BildirgeWorkBook = null;
                                }

                                if (Directory.Exists(isyeriSavePath))
                                {
                                    var dirs = Directory.GetDirectories(isyeriSavePath);

                                    foreach (var dir in dirs)
                                    {
                                        if (dir.EndsWith("Ay Teşvik Çalışması"))
                                        {
                                            cariKlasorler.Add(dir);

                                            var kanunNoDegisenler = new Dictionary<AphbSatir, KeyValuePair<string, string>>();

                                            if (muhtasarIsyeri != null)
                                            {
                                                foreach (var muhtasarKisi in muhtasarIsyeri.kisiler)
                                                {
                                                    var yeniKanun = "";
                                                    if (muhtasarKisi.xElement != null)
                                                    {
                                                        yeniKanun = muhtasarKisi.xElement.Element("kanun").Value;
                                                    }
                                                    else if (muhtasarKisi.NetsisBilgiler != null)
                                                    {
                                                        yeniKanun = muhtasarKisi.NetsisBilgiler.netsisBilgiler[(int)Enums.NetsisHucreBilgileri.Kanun];
                                                    }
                                                    else if (muhtasarKisi.NetsisBilgilerExcel != null)
                                                    {
                                                        yeniKanun = muhtasarKisi.NetsisBilgilerExcel.netsisBilgiler[(int)Enums.NetsisHucreBilgileri.Kanun];
                                                    }

                                                    var eskiKanun = muhtasarKisi.MuhtasarOrijinalKanun.PadLeft(5, '0');
                                                    yeniKanun = yeniKanun.PadLeft(5, '0');

                                                    if (eskiKanun != yeniKanun)
                                                    {
                                                        kanunNoDegisenler.Add(muhtasarKisi, new KeyValuePair<string, string>(eskiKanun, yeniKanun));
                                                    }
                                                }
                                            }

                                            if (kanunNoDegisenler.Count > 0)
                                            {
                                                var ilkSatir = kanunNoDegisenler.First().Key;
                                                Metodlar.KanunNoDegisenlerKaydet(kanunNoDegisenler, dir, AktifIsyeri, ilkSatir.Yil, ilkSatir.Ay);
                                            }

                                            break;
                                        }
                                    }




                                }
                            }
                        }
                    }

                    if (wordApp != null) wordApp.Quit();

                    if (wordprocessid > 0)
                    {
                        Metodlar.KillProcessById(wordprocessid);
                    }


                    if (MyApp != null)
                    {

                        if (BildirgeWorkBook != null)
                        {
                            try
                            {
                                BildirgeWorkBook.Close(false);
                            }
                            catch { }
                        }

                        HafizadanAtilacaklar.Reverse();

                        int i = 0;

                        while (i < HafizadanAtilacaklar.Count())
                        {
                            try
                            {
                                var item = HafizadanAtilacaklar.ElementAt(i);

                                Marshal.FinalReleaseComObject(item);

                                item = null;

                            }
                            catch
                            {
                            }

                            i++;
                        }


                        MyApp.Quit();
                        Marshal.FinalReleaseComObject(MyApp);

                    }

                    if (excelprocessid > 0)
                    {
                        Metodlar.KillProcessById(excelprocessid);
                    }

                    if (!hataOlustu)
                    {

                        string Mesaj = String.Format("{0} başarılı bir şekilde oluşturuldu. Programla aynı klasörde bulunan output klasöründen {1} görüntüleyebilirsiniz.", SadeceIcmal ? "İcmaller" : "Bildirgeler", SadeceIcmal ? "icmalleri" : "bildirgeleri");

                        string Uyari = Environment.NewLine + Environment.NewLine + "Aşağıdaki dosyalar kaydedilemedi:" + Environment.NewLine + Environment.NewLine;

                        if (hatalar.Count > 0)
                        {
                            foreach (string hata in hatalar)
                            {
                                Uyari += Path.GetFileName(hata) + Environment.NewLine;
                            }

                            Uyari += Environment.NewLine + "Eğer ilgili dosyalar açıksa lütfen kapatıp aynı işlemi tekrar deneyiniz";

                            if (basariliolanlar.Count > 0)
                            {
                                Mesaj = String.Format("Bazı {0} başarılı oluşturabildi. Programla aynı klasörde bulunan output klasöründen {1} görüntüleyebilirsiniz.", SadeceIcmal ? "icmaller" : "bildirgeler", SadeceIcmal ? "icmalleri" : "bildirgeleri");
                            }
                            else Mesaj = String.Format("{0} kaydedilemedi.", SadeceIcmal ? "İcmaller" : "Bildirgeler");
                        }
                        else Uyari = "";


                        var tumxmller = responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.SelectMany(p => p.xmller.Select(x => x.Value)).Distinct();

                        if (tumxmller.Count() > 0)
                        {
                            //var klasor = Path.Combine(Application.StartupPath, "output", String.Format("{0}-{1} Teşvik Çalışması", muhtasarYil, muhtasarAy.ToString().PadLeft(2, '0')));
                            var klasor = Path.Combine(Application.StartupPath, "output");

                            if (!Directory.Exists(klasor)) Directory.CreateDirectory(klasor);

                            if (cariKlasorler.Count == 1 && responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.Count == 1)
                                klasor = cariKlasorler.First();

                            var sira = 1;

                            foreach (var xml in tumxmller)
                            {
                                xml.Save(Path.Combine(klasor, String.Format("{0} MuhSgk-{1}.xml", seciliIsyeri.Sirketler.SirketAdi.TurkceKarakterleriDegistir(), sira)));
                                sira++;
                            }

                        }

                        var tumnetsisBildirgeler = responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.SelectMany(p => p.netsisBildirgeler).GroupBy(p => p.Key).ToDictionary(x => x.Key, x => x.Select(p => p.Value).FirstOrDefault());

                        if (tumnetsisBildirgeler.Count() > 0)
                        {
                            var klasor = Path.Combine(Application.StartupPath, "output");

                            if (!Directory.Exists(klasor)) Directory.CreateDirectory(klasor);

                            var sira = 1;

                            if (tumnetsisBildirgeler.Count > 1)
                            {
                                klasor = Directory.CreateDirectory(Path.Combine(klasor, String.Format("{0} MuhSgk", seciliIsyeri.Sirketler.SirketAdi.TurkceKarakterleriDegistir()))).FullName;
                            }
                            else
                            {
                                if (cariKlasorler.Count == 1 && responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.Count == 1)
                                    klasor = cariKlasorler.First();
                            }

                            foreach (var item in tumnetsisBildirgeler)
                            {
                                var dosyaadi = tumnetsisBildirgeler.Count > 1 ? Path.GetFileName(item.Key) : String.Format("{0} MuhSgk.txt", seciliIsyeri.Sirketler.SirketAdi.TurkceKarakterleriDegistir());
                                var netsisBildirge = item.Value;
                                var netsisIcerik = String.Join("\r\n", netsisBildirge.Select(x => String.Join("\t", x)));
                                File.WriteAllText(Path.Combine(klasor, String.Format("{0}", dosyaadi)), netsisIcerik, System.Text.Encoding.GetEncoding("iso-8859-9"));
                                sira++;
                            }

                            if (tumnetsisBildirgeler.Count > 1)
                            {
                                ZipFile.CreateFromDirectory(klasor, Path.Combine(Application.StartupPath, "output", String.Format("{0} MuhSgk.zip", seciliIsyeri.Sirketler.SirketAdi.TurkceKarakterleriDegistir())), CompressionLevel.Fastest, false, System.Text.Encoding.GetEncoding("ibm857"));

                                Directory.Delete(klasor, true);
                            }
                        }

                        var tumnetsisBildirgelerExcel = responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.SelectMany(p => p.netsisBildirgelerExcel).GroupBy(p => p.Key).ToDictionary(x => x.Key, x => x.Select(p => p.Value).FirstOrDefault());

                        if (tumnetsisBildirgelerExcel.Count() > 0)
                        {
                            var klasor = Path.Combine(Application.StartupPath, "output");

                            if (!Directory.Exists(klasor)) Directory.CreateDirectory(klasor);

                            var sira = 1;

                            if (tumnetsisBildirgelerExcel.Count > 1)
                            {
                                klasor = Directory.CreateDirectory(Path.Combine(klasor, String.Format("{0} MuhSgk", seciliIsyeri.Sirketler.SirketAdi.TurkceKarakterleriDegistir()))).FullName;
                            }
                            else
                            {
                                if (cariKlasorler.Count == 1 && responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.Count == 1)
                                    klasor = cariKlasorler.First();
                            }

                            foreach (var item in tumnetsisBildirgelerExcel)
                            {
                                var dosyaadi = tumnetsisBildirgeler.Count > 1 ? Path.GetFileName(item.Key) : String.Format("{0} MuhSgk{1}", seciliIsyeri.Sirketler.SirketAdi.TurkceKarakterleriDegistir(), Path.GetExtension(item.Key));
                                var netsisBildirgeExcel = item.Value;

                                DataTable dtNetsisExcel = new DataTable("NetsisExcel");

                                for (int i = 0; i < netsisBildirgeExcel[0].Length; i++)
                                {
                                    dtNetsisExcel.Columns.Add();
                                }

                                foreach (var satir in netsisBildirgeExcel)
                                {
                                    var newRow = dtNetsisExcel.NewRow();

                                    for (int i = 0; i < satir.Length; i++)
                                    {
                                        if (i >= dtNetsisExcel.Columns.Count) break;

                                        newRow[i] = satir[i];
                                    }

                                    dtNetsisExcel.Rows.Add(newRow);
                                }

                                Metodlar.NetsisExcelKaydet(dtNetsisExcel, Path.Combine(klasor, dosyaadi));

                                var dosyaAdiTxt = tumnetsisBildirgeler.Count > 1 ? Path.GetFileName(item.Key) : String.Format("{0} MuhSgk{1}", seciliIsyeri.Sirketler.SirketAdi.TurkceKarakterleriDegistir(), ".txt");

                                var netsistxtIcerikSb = new StringBuilder();

                                foreach (var row in dtNetsisExcel.AsEnumerable())
                                {
                                    for (int i = 0; i < dtNetsisExcel.Columns.Count; i++)
                                    {
                                        netsistxtIcerikSb.Append(row[i].ToString() + "\t");
                                    }

                                    netsistxtIcerikSb.Append("\r\n");
                                }

                                var netsisIcerik = netsistxtIcerikSb.ToString();
                                netsisIcerik = netsisIcerik.EndsWith("\r\n") ? netsisIcerik.Substring(0, netsisIcerik.Length - 2) : netsisIcerik;

                                File.WriteAllText(Path.Combine(klasor, String.Format("{0}", dosyaAdiTxt)), netsisIcerik, System.Text.Encoding.GetEncoding("iso-8859-9"));

                                sira++;
                            }

                            if (tumnetsisBildirgeler.Count > 1)
                            {
                                ZipFile.CreateFromDirectory(klasor, Path.Combine(Application.StartupPath, "output", String.Format("{0} MuhSgk.zip", seciliIsyeri.Sirketler.SirketAdi.TurkceKarakterleriDegistir())), CompressionLevel.Fastest, false, System.Text.Encoding.GetEncoding("ibm857"));

                                Directory.Delete(klasor, true);
                            }
                        }

                        if (tumxmller.Count() > 0 || tumnetsisBildirgeler.Count > 0 || tumnetsisBildirgelerExcel.Count > 0)
                        {
                            if (basariliolanlar.Count > 0)
                            {
                                var klasor = Path.Combine(Application.StartupPath, "output");

                                if (cariKlasorler.Count == 1 && responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.Count == 1)
                                    klasor = cariKlasorler.First();

                                var muhtasarIcmalPath = Path.Combine(klasor, String.Format("{0} icmal-.xlsx", new DateTime(2020, muhtasarAy, 1).ToString("MMMM")));
                                Metodlar.MuhtasarCokluIcmalKaydet(tumIsyerleriIcmaller, muhtasarIcmalPath);


                                foreach (var cariPath in cariKlasorler)
                                {
                                    if (File.Exists(cariPath + ".zip"))
                                    {
                                        File.Delete(cariPath + ".zip");
                                    }

                                    ZipFile.CreateFromDirectory(cariPath, cariPath + ".zip", System.IO.Compression.CompressionLevel.Fastest, false, System.Text.Encoding.UTF8);
                                }



                                if (responseMuhtasarIsyerleriBul.MuhtasarIsyerleri.Count > 1)
                                {
                                    var outputKlasoru = Path.Combine(Application.StartupPath, "output");

                                    var isyeriKlasorleri = Directory.GetDirectories(outputKlasoru);

                                    if (isyeriKlasorleri.Length > 0)
                                    {
                                        var tesvikCalismasiKlasoru = Path.Combine(outputKlasoru, $"{ CariAy.ToString("MMMM") } ayı teşvik çalışması");

                                        Directory.CreateDirectory(tesvikCalismasiKlasoru);

                                        foreach (var isyeriKlasor in isyeriKlasorleri)
                                        {
                                            var cariFolder = Directory.GetDirectories(isyeriKlasor).FirstOrDefault(p => Path.GetFileName(p).StartsWith(CariAy.ToString("yyyy-MM")));

                                            if (cariFolder != null)
                                            {
                                                var newIsyeriFolder = Path.Combine(tesvikCalismasiKlasoru, Path.GetFileName(isyeriKlasor));
                                                Directory.CreateDirectory(newIsyeriFolder);

                                                var isyeriFiles = Directory.GetFiles(cariFolder);

                                                foreach (var item in isyeriFiles)
                                                {
                                                    File.Copy(item, Path.Combine(newIsyeriFolder, Path.GetFileName(item)));
                                                }
                                            }
                                        }

                                        var outputDosyalari = Directory.GetFiles(outputKlasoru);

                                        foreach (var item in outputDosyalari)
                                        {
                                            File.Move(item, Path.Combine(tesvikCalismasiKlasoru, Path.GetFileName(item)));
                                        }

                                        if (Directory.GetFiles(tesvikCalismasiKlasoru).Length > 0)
                                        {
                                            ZipFile.CreateFromDirectory(tesvikCalismasiKlasoru, Path.Combine(outputKlasoru, Path.GetFileName(tesvikCalismasiKlasoru) + ".zip"), CompressionLevel.Fastest, false, Encoding.UTF8);
                                        }
                                        else Directory.Delete(tesvikCalismasiKlasoru, true);

                                    }
                                }
                            }
                        }


                        formBildirgeOlustur.Cursor = Cursors.Default;

                        formBildirgeOlustur.progressBar1.Visible = false;

                        formBildirgeOlustur.progressBar1.Value = 0;

                        if (hataliGunuOlanKisiler.Count > 0)
                        {
                            var hataliGunuOlanKisilerMesaj = "Aşağıdaki kişilerin Başvuru Formu kayıtları ile APHB kayıtları tutarsız olduğu için bu kişiler bu teşvikler için gözardı edildi" + Environment.NewLine + Environment.NewLine;

                            foreach (var item in hataliGunuOlanKisiler)
                            {
                                hataliGunuOlanKisilerMesaj += item.Key.Sirketler.SirketAdi + " - " + item.Key.SubeAdi;

                                hataliGunuOlanKisilerMesaj += Environment.NewLine + "------------------------------------------------------------------------------" + Environment.NewLine;

                                foreach (var kisiMesaj in item.Value)
                                {
                                    hataliGunuOlanKisilerMesaj += kisiMesaj + Environment.NewLine;
                                }

                            }

                            new frmMesaj(hataliGunuOlanKisilerMesaj, true).ShowDialog();
                        }

                        if (basariliolanlar.Count > 0 && hatalar.Count > 0)
                        {
                            MessageBox.Show(Mesaj + Uyari, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else if (basariliolanlar.Count == 0 && hatalar.Count > 0)
                        {
                            MessageBox.Show(Mesaj + Uyari, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (basariliolanlar.Count > 0 && hatalar.Count == 0)
                        {
                            MessageBox.Show(Mesaj);
                        }
                        else
                        {
                            MessageBox.Show("Teşvik verilecek kimse bulunamadı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        if (basariliolanlar.Count > 0 || tumxmller.Count() > 0)
                        {
                            Process.Start(Path.Combine(Application.StartupPath, "output"));
                        }
                    }
                    else
                    {

                        formBildirgeOlustur.Cursor = Cursors.Default;

                        formBildirgeOlustur.progressBar1.Visible = false;

                        formBildirgeOlustur.progressBar1.Value = 0;
                    }

                }
            }

            try
            {
                if (!string.IsNullOrEmpty(cariKlasor))
                    Directory.Delete(cariKlasor, true);
            }
            catch { }
        }

        void BildirgeOlustur(KeyValuePair<string, string> yilveay,
                    DataTable dtaylikliste,
                    Isyerleri AktifIsyeri,
                    Dictionary<KeyValuePair<string, string>, List<KeyValuePair<int, int>>> tumyilveaylar,
                    ref Excel2.Application MyApp,
                    ref Excel2.Workbook BildirgeWorkBook,
                    ref int excelprocessid,
                    Isyerleri isyeri,
                    string IsyeriSicilNo,
                    DateTime enbuyukay,
                    bool SadeceIcmal,
                    ref List<string> hatalar,
                    ref List<string> basariliolanlar,
                    string IsyeriAd,
                    string IsyeriVergiNo,
                    ref int index,
                    List<KeyValuePair<string, string>> yilveaylar,
                    out List<Classes.Cikti> ciktilar,
                    int hesaplanacakIsyeriSira,
                    int hesaplanacakIsyerleriCount,
                    ref frmBildirgeOlustur formIsyerleri,
                    MuhtasarIsyeri muhtasarIsyeri

    )
        {
            bool devamet = true;

            do
            {
                devamet = true;

                string Yil = yilveay.Key;

                string Ay = yilveay.Value;

                bool Tarih2020veSonrasi = new DateTime(Convert.ToInt32(Yil), Convert.ToInt32(Ay), 1) >= new DateTime(2020, 1, 1);

                bool KayitYok = false;

                ciktilar = new List<Classes.Cikti>();

                TesvikBilgileriGetir(Yil
                    , Ay
                    , dtaylikliste
                    , AktifIsyeri
                    , out KayitYok
                    , tumyilveaylar
                    , out ciktilar);

                foreach (var cikti in ciktilar)
                {
                    cikti.muhtasarSatirlar.ForEach(p =>
                    {
                        if (Liste14857.Contains(p.IlgiliSatir)) Liste14857.Remove(p.IlgiliSatir);
                    });

                    cikti.muhtasarIptalSatirlar.ForEach(p =>
                    {
                        if (Liste14857.Contains(p.IlgiliSatir)) Liste14857.Remove(p.IlgiliSatir);
                    });
                }

                //if (KayitYok) MessageBox.Show("Seçtiğiniz döneme ait kayıt bulunamamıştır", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if ((dthatalisatirlar != null && dthatalisatirlar.Rows.Count > 0)
                    ||
                    BasvuruFormlariHataliSatirlar.Any(p => p.Value != null)
                    )
                {

                    return;
                }


                if (ciktilar.Count > 0)
                {

                    var liste7252 = Metodlar.FormBul(isyeri, Enums.FormTuru.Liste7252);

                    if (MyApp == null)
                    {

                        MyApp = new Excel2.Application();

                        MyApp.Visible = false;

                        MyApp.DisplayAlerts = false;

                        excelprocessid = Metodlar.GetExcelProcessId(MyApp);
                    }

                    string IsverenAdSoyad = isyeri.IsverenAdSoyad;

                    string IsverenTcKimlikNo = isyeri.Sirketler.VergiKimlikNo;

                    string IsverenUnvan = isyeri.IsverenUnvan;

                    string IsverenAdres = isyeri.IsverenAdres;

                    string IsverenSemt = isyeri.IsverenSemt;

                    string IsverenIlce = isyeri.IsverenIlce;

                    string IsverenIl = isyeri.IsverenIl;

                    string IsverenDisKapiNo = isyeri.IsverenDisKapiNo;

                    string IsverenIcKapiNo = isyeri.IsverenIcKapiNo;

                    string IsverenPostaKodu = isyeri.IsverenPostaKodu;

                    string IsverenTelefon = isyeri.IsverenTelefon;

                    string IsverenEposta = isyeri.IsverenEposta;

                    string AltIsverenAdSoyad = isyeri.AltIsverenAdSoyad;

                    string AltIsverenTcKimlikNo = isyeri.AltIsverenTcKimlikNo;

                    string AltIsverenUnvan = isyeri.AltIsverenUnvan;

                    string AltIsverenAdres = isyeri.AltIsverenAdres;

                    string AltIsverenSemt = isyeri.AltIsverenSemt;

                    string AltIsverenIlce = isyeri.AltIsverenIlce;

                    string AltIsverenIl = isyeri.AltIsverenIl;

                    string AltIsverenDisKapiNo = isyeri.AltIsverenDisKapiNo;

                    string AltIsverenIcKapiNo = isyeri.AltIsverenIcKapiNo;

                    string AltIsverenPostaKodu = isyeri.AltIsverenPostaKodu;

                    string AltIsverenTelefon = isyeri.AltIsverenTelefon;

                    string AltIsverenEposta = isyeri.AltIsverenEposta;

                    string kisaVadeliSigortaKoluKodu = IsyeriSicilNo.Substring(1, 4);

                    decimal kisaVadeliSigortaKoluOrani = Metodlar.KvskBul(Convert.ToInt32(Yil), Convert.ToInt32(Ay), IsyeriSicilNo);

                    decimal AsgariUcret = Metodlar.AsgariUcretBul(Convert.ToInt32(Yil), Convert.ToInt32(Ay));

                    int index2 = 0;

                    Excel2.Worksheet IlkSayfa = null;

                    Excel2.Worksheet DevamSheet = null;

                    Excel2.Worksheet MySheet = null;

                    int BaslangicNo = BildirgeOlusturmaSabitleri.IlkSigortaliSayi;

                    foreach (Classes.Cikti cikti in ciktilar)
                    {

                        List<Classes.AphbSatir> sayfasatirlari = new List<Classes.AphbSatir>();

                        var kisilistesi = cikti.Kisiler;

                        string belgeturu = cikti.BelgeTuru;

                        int ToplamPrimGunSayisi = cikti.Gun_Tesvik_Verilmeyenler_Dahil;

                        decimal ToplamKazanc = cikti.Matrah_Tesvik_Verilmeyenler_Dahil;

                        int AyIcindeIseGirenler = cikti.satirlar.Count(p => !String.IsNullOrEmpty(p.GirisGunu.Trim()));

                        int AyIcindeIstenCikanlar = cikti.satirlar.Count(p => !String.IsNullOrEmpty(p.CikisGunu.Trim()));

                        decimal MalullukPrimOraniSigortali = 0;

                        decimal MalullukPrimOraniIsveren = 0;

                        decimal GenelSaglikSigortasiPrimOraniSigortali = 0;

                        decimal GenelSaglikSigortasiPrimOraniIsveren = 0;

                        decimal SosyalGuvenlikDestekPrimOraniSigortali = 0;

                        decimal SosyalGuvenlikDestekPrimOraniIsveren = 0;

                        decimal IssizlikSigortasiPrimOraniSigortali = 0;

                        decimal IssizlikSigortasiPrimOraniIsveren = 0;

                        var belgeTuruBilgileri = Program.BelgeTurleri.ContainsKey(Convert.ToInt64(belgeturu)) ? Program.BelgeTurleri[Convert.ToInt64(belgeturu)] : null;

                        if (belgeTuruBilgileri != null)
                        {
                            MalullukPrimOraniSigortali = Convert.ToDecimal(belgeTuruBilgileri.MalulYaslilikOraniSigortali);

                            MalullukPrimOraniIsveren = Convert.ToDecimal(belgeTuruBilgileri.MalulYaslilikOraniIsveren);

                            GenelSaglikSigortasiPrimOraniSigortali = Convert.ToDecimal(belgeTuruBilgileri.GenelSaglikSigortali);

                            GenelSaglikSigortasiPrimOraniIsveren = Convert.ToDecimal(belgeTuruBilgileri.GenelSaglikIsveren);

                            SosyalGuvenlikDestekPrimOraniSigortali = Convert.ToDecimal(belgeTuruBilgileri.SosyalDestekSigortali);

                            SosyalGuvenlikDestekPrimOraniIsveren = Convert.ToDecimal(belgeTuruBilgileri.SosyalDestekIsveren);

                            IssizlikSigortasiPrimOraniSigortali = Convert.ToDecimal(belgeTuruBilgileri.IssizlikSigortali);

                            IssizlikSigortasiPrimOraniIsveren = Convert.ToDecimal(belgeTuruBilgileri.IssizlikIsveren);
                        }

                        string isyerisicilno = IsyeriSicilNo.Replace("-", "");

                        decimal GenelToplamPrimOrani = kisaVadeliSigortaKoluOrani + MalullukPrimOraniSigortali + MalullukPrimOraniIsveren + GenelSaglikSigortasiPrimOraniSigortali + GenelSaglikSigortasiPrimOraniIsveren + SosyalGuvenlikDestekPrimOraniSigortali + SosyalGuvenlikDestekPrimOraniIsveren + IssizlikSigortasiPrimOraniSigortali + IssizlikSigortasiPrimOraniIsveren;

                        decimal GenelToplamIcmalPrimOrani = kisaVadeliSigortaKoluOrani + ((MalullukPrimOraniIsveren - 5) < 0 ? 0 : (MalullukPrimOraniIsveren - 5)) + GenelSaglikSigortasiPrimOraniIsveren;

                        if (cikti.BildirgeOlustur)
                        {
                            if (!SadeceIcmal)
                            {
                                if (cikti.ExcelOlustur)
                                {

                                    if (muhtasarYil == Yil.ToInt() && muhtasarAy == Ay.ToInt() && !cikti.Iptal)
                                    {

                                        if (!Directory.Exists(isyeriSavePath)) Directory.CreateDirectory(isyeriSavePath);

                                        var cariAyKlasoruneKaydet = CariAyMi(Yil.ToInt(), Ay.ToInt()); // enbuyukay.Year.ToString() == Yil && enbuyukay.Month.ToString().PadLeft(2, '0') == Ay.PadLeft(2, '0');

                                        if (!cariAyKlasoruneKaydet || string.IsNullOrEmpty(txtYil))
                                        {
                                            string dir = Path.Combine(isyeriSavePath, Yil + "-" + Ay.PadLeft(2, '0'));

                                            if (!Directory.Exists(dir))
                                            {
                                                Directory.CreateDirectory(dir);
                                            }

                                            try
                                            {
                                                var savepathTekSayfa = Path.Combine(dir, Yil + "-" + Ay.PadLeft(2, '0') + " Dönemi " + cikti.Kanun + " " + (cikti.Iptal ? "İptal" : cikti.Asil ? "Asıl" : "Ek") + " Bildirge - Belge " + belgeturu + ".xlsx");

                                                var liste14857 = Liste14857.Where(p => p[(int)Enums.AphbHucreBilgileri.Kanun].ToString() == cikti.Kanun && p[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString() == cikti.BelgeTuru).ToList();

                                                Metodlar.TekSayfaBildirgeOlustur(cikti, isyeri, Yil, Ay, savepathTekSayfa, liste14857);

                                                if (cikti.Kanun.EndsWith("7252") && !cikti.Iptal)
                                                {
                                                    if (!string.IsNullOrEmpty(liste7252))
                                                    {
                                                        if (!File.Exists(Path.Combine(dir, Path.GetFileName(liste7252))))
                                                        {
                                                            File.Copy(liste7252, Path.Combine(dir, Path.GetFileName(liste7252)));
                                                        }
                                                    }
                                                }

                                                basariliolanlar.Add(Path.Combine(dir, Yil + "-" + Ay.PadLeft(2, '0') + " Dönemi " + cikti.Kanun + " " + (cikti.Iptal ? "İptal" : cikti.Asil ? "Asıl" : "Ek") + " Bildirge - Belge " + belgeturu + ".xlsx"));

                                            }
                                            catch
                                            {
                                                hatalar.Add(Path.Combine(dir, Yil + "-" + Ay.PadLeft(2, '0') + " Dönemi " + cikti.Kanun + " " + (cikti.Iptal ? "İptal" : cikti.Asil ? "Asıl" : "Ek") + " Bildirge - Belge " + belgeturu + ".xlsx"));
                                            }


                                        }

                                        if (cariAyKlasoruneKaydet)
                                        {

                                            string dir2 = Path.Combine(isyeriSavePath, Yil + "-" + Ay.PadLeft(2, '0') + ". Ay Teşvik Çalışması");

                                            if (!Directory.Exists(dir2))
                                            {
                                                Directory.CreateDirectory(dir2);
                                            }

                                            try
                                            {

                                                var savepathTekSayfa = Path.Combine(dir2, Yil + "-" + Ay.PadLeft(2, '0') + " Dönemi " + cikti.Kanun + " " + (cikti.Iptal ? "İptal" : cikti.Asil ? "Asıl" : "Ek") + " Bildirge - Belge " + belgeturu + ".xlsx");

                                                var liste14857 = Liste14857.Where(p => p[(int)Enums.AphbHucreBilgileri.Kanun].ToString() == cikti.Kanun && p[(int)Enums.AphbHucreBilgileri.BelgeTuru].ToString() == cikti.BelgeTuru).ToList();

                                                Metodlar.TekSayfaBildirgeOlustur(cikti, isyeri, Yil, Ay, savepathTekSayfa, liste14857);

                                                if (cikti.Kanun.EndsWith("7252") && !cikti.Iptal)
                                                {
                                                    if (!string.IsNullOrEmpty(liste7252))
                                                    {
                                                        if (!File.Exists(Path.Combine(dir2, Path.GetFileName(liste7252))))
                                                        {
                                                            File.Copy(liste7252, Path.Combine(dir2, Path.GetFileName(liste7252)));
                                                        }
                                                    }
                                                }

                                                basariliolanlar.Add(Path.Combine(dir2, Yil + "-" + Ay.PadLeft(2, '0') + " Dönemi " + cikti.Kanun + " " + (cikti.Iptal ? "İptal" : cikti.Asil ? "Asıl" : "Ek") + " Bildirge - Belge " + belgeturu + ".xlsx"));

                                            }
                                            catch
                                            {
                                                hatalar.Add(Path.Combine(dir2, Yil + "-" + Ay.PadLeft(2, '0') + " Dönemi " + cikti.Kanun + " " + (cikti.Iptal ? "İptal" : cikti.Asil ? "Asıl" : "Ek") + " Bildirge - Belge " + belgeturu + ".xlsx"));
                                            }
                                        }
                                    }
                                    else
                                    {

                                        if (BildirgeWorkBook == null)
                                        {
                                            IlkSayfaHucreleri.Clear();

                                            DevamSayfaHucreleri.Clear();

                                            var workbooks = MyApp.Workbooks;

                                            BildirgeWorkBook = workbooks.Open(Path.Combine(Application.StartupPath, "Ek9Ilk.xls"));

                                            IlkSayfa = (Excel2.Worksheet)BildirgeWorkBook.Sheets[1];

                                            DevamSheet = ((Excel2.Worksheet)BildirgeWorkBook.Sheets[2]);

                                            HafizadanAtilacaklar.AddRange(new List<object> { workbooks, BildirgeWorkBook, IlkSayfa, DevamSheet });

                                            for (int m = 1; m <= 7; m++)
                                            {
                                                IlkSayfaHucreleri.Add("A" + BaslangicNo.ToString(), IlkSayfa.Range["A" + BaslangicNo.ToString()]);
                                                IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliSosyalGuvenlikSicilNo] + BaslangicNo.ToString(), IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliSosyalGuvenlikSicilNo] + BaslangicNo.ToString()]);
                                                IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliAdi] + BaslangicNo.ToString(), IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliAdi] + BaslangicNo.ToString()]);
                                                IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliSoyadi] + BaslangicNo.ToString(), IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliSoyadi] + BaslangicNo.ToString()]);
                                                IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIlkSoyadi] + BaslangicNo.ToString(), IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIlkSoyadi] + BaslangicNo.ToString()]);
                                                IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliPrimOdemeGunu] + BaslangicNo.ToString(), IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliPrimOdemeGunu] + BaslangicNo.ToString()]);
                                                IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliUcret] + BaslangicNo.ToString(), IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliUcret] + BaslangicNo.ToString()]);
                                                IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIkramiye] + BaslangicNo.ToString(), IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIkramiye] + BaslangicNo.ToString()]);
                                                IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIseGirisTarihiAy] + BaslangicNo.ToString(), IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIseGirisTarihiAy] + BaslangicNo.ToString()]);
                                                IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIseGirisTarihiGun] + BaslangicNo.ToString(), IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIseGirisTarihiGun] + BaslangicNo.ToString()]);
                                                IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIstenCikisTarihiAy] + BaslangicNo.ToString(), IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIstenCikisTarihiAy] + BaslangicNo.ToString()]);
                                                IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIstenCikisTarihiGun] + BaslangicNo.ToString(), IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIstenCikisTarihiGun] + BaslangicNo.ToString()]);
                                                IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliEksikGunSayisi] + BaslangicNo.ToString(), IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliEksikGunSayisi] + BaslangicNo.ToString()]);
                                                IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliEksikGunNedeni] + BaslangicNo.ToString(), IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliEksikGunNedeni] + BaslangicNo.ToString()]);
                                                IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIstenCikisNedeni] + BaslangicNo.ToString(), IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIstenCikisNedeni] + BaslangicNo.ToString()]);

                                                BaslangicNo += BildirgeOlusturmaSabitleri.IlkSigortaliArtis;
                                            }

                                            BaslangicNo = BildirgeOlusturmaSabitleri.DevamSayfasiIlkSigortaliSayi;

                                            for (int m = 1; m <= 25; m++)
                                            {

                                                DevamSayfaHucreleri.Add("A" + BaslangicNo.ToString(), DevamSheet.Range["A" + BaslangicNo.ToString()]);
                                                DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliSosyalGuvenlikSicilNo] + BaslangicNo.ToString(), DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliSosyalGuvenlikSicilNo] + BaslangicNo.ToString()]);
                                                DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliAdi] + BaslangicNo.ToString(), DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliAdi] + BaslangicNo.ToString()]);
                                                DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliSoyadi] + BaslangicNo.ToString(), DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliSoyadi] + BaslangicNo.ToString()]);
                                                DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIlkSoyadi] + BaslangicNo.ToString(), DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIlkSoyadi] + BaslangicNo.ToString()]);
                                                DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliPrimOdemeGunu] + BaslangicNo.ToString(), DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliPrimOdemeGunu] + BaslangicNo.ToString()]);
                                                DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliUcret] + BaslangicNo.ToString(), DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliUcret] + BaslangicNo.ToString()]);
                                                DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIkramiye] + BaslangicNo.ToString(), DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIkramiye] + BaslangicNo.ToString()]);
                                                DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIseGirisTarihiAy] + BaslangicNo.ToString(), DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIseGirisTarihiAy] + BaslangicNo.ToString()]);
                                                DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIseGirisTarihiGun] + BaslangicNo.ToString(), DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIseGirisTarihiGun] + BaslangicNo.ToString()]);
                                                DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIstenCikisTarihiAy] + BaslangicNo.ToString(), DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIstenCikisTarihiAy] + BaslangicNo.ToString()]);
                                                DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIstenCikisTarihiGun] + BaslangicNo.ToString(), DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIstenCikisTarihiGun] + BaslangicNo.ToString()]);
                                                DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliEksikGunSayisi] + BaslangicNo.ToString(), DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliEksikGunSayisi] + BaslangicNo.ToString()]);
                                                DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliEksikGunNedeni] + BaslangicNo.ToString(), DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliEksikGunNedeni] + BaslangicNo.ToString()]);
                                                DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIstenCikisNedeni] + BaslangicNo.ToString(), DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIstenCikisNedeni] + BaslangicNo.ToString()]);

                                                BaslangicNo += BildirgeOlusturmaSabitleri.IlkSigortaliArtis;
                                            }

                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SayfaNo], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SayfaNo]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Isveren], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Isveren]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenTckimlikNo], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenTckimlikNo]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenAdiSoyadi], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenAdiSoyadi]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenUnvan], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenUnvan]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenAdres], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenAdres]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenSemt], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenSemt]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIlce], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIlce]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIl], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIl]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenDisKapiNo], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenDisKapiNo]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIcKapiNo], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIcKapiNo]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenPostaKodu], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenPostaKodu]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenTelefon], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenTelefon]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenEposta], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenEposta]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsveren], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsveren]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliyiDevirAlan], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliyiDevirAlan]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenTckimlikNo], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenTckimlikNo]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenAdiSoyadi], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenAdiSoyadi]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenUnvan], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenUnvan]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenAdres], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenAdres]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenSemt], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenSemt]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIlce], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIlce]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIl], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIl]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenDisKapiNo], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenDisKapiNo]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIcKapiNo], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIcKapiNo]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenPostaKodu], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenPostaKodu]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenTelefon], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenTelefon]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenEposta], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenEposta]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BelgeYil], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BelgeYil]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BelgeAy], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BelgeAy]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Asil], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Asil]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Ek], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Ek]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Iptal], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Iptal]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BildirgeBelgeTuru], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BildirgeBelgeTuru]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BildirgeKanun], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BildirgeKanun]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.ToplamSayfaSayisi], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.ToplamSayfaSayisi]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliSayisi], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliSayisi]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.PrimOdemeGunSayisi], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.PrimOdemeGunSayisi]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AyIcindeIseGirenler], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AyIcindeIseGirenler]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AyIcindeIstenCikanlar], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AyIcindeIstenCikanlar]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.KisaVadeliSigortaKollariPrimiToplamKazanc], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.KisaVadeliSigortaKollariPrimiToplamKazanc]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.MalullukYaslilikSigortasiToplamKazanc], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.MalullukYaslilikSigortasiToplamKazanc]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelSaglikSigortasiPrimiToplamKazanc], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelSaglikSigortasiPrimiToplamKazanc]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SosyalGuvenlikDestekPrimiToplamKazanc], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SosyalGuvenlikDestekPrimiToplamKazanc]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IssizlikSigortasiPrimiToplamKazanc], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IssizlikSigortasiPrimiToplamKazanc]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelToplamKazanc], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelToplamKazanc]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.KisaVadeliSigortaKollariPrimiOrani], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.KisaVadeliSigortaKollariPrimiOrani]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.MalullukYaslilikSigortasiPrimOrani], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.MalullukYaslilikSigortasiPrimOrani]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelSaglikSigortasiPrimOrani], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelSaglikSigortasiPrimOrani]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SosyalGuvenlikDestekPrimOrani], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SosyalGuvenlikDestekPrimOrani]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IssizlikSigortasiPrimOrani], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IssizlikSigortasiPrimOrani]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelToplamPrimOrani], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelToplamPrimOrani]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.KisaVadeliSigortaKollariPrimTutari], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.KisaVadeliSigortaKollariPrimTutari]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.MalullukYaslilikSigortasiPrimTutari], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.MalullukYaslilikSigortasiPrimTutari]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelSaglikSigortasiPrimTutari], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelSaglikSigortasiPrimTutari]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SosyalGuvenlikDestekPrimTutari], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SosyalGuvenlikDestekPrimTutari]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IssizlikSigortasiPrimTutari], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IssizlikSigortasiPrimTutari]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelToplamPrimTutari], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelToplamPrimTutari]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaToplamPrimeEsasKazanc], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaToplamPrimeEsasKazanc]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaToplamPrimOdemeGunu], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaToplamPrimOdemeGunu]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaToplamSigortaliSayisi], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaToplamSigortaliSayisi]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaAciklama], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaAciklama]]);
                                            IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaBilgiler], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaBilgiler]]);


                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SayfaNo], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SayfaNo]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamBuSayfaToplamPrimeEsasKazanc], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamBuSayfaToplamPrimeEsasKazanc]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamBuSayfaToplamPrimOdemeGunu], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamBuSayfaToplamPrimOdemeGunu]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamBuSayfaToplamSigortaliSayisi], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamBuSayfaToplamSigortaliSayisi]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamOncekiSayfaToplamPrimeEsasKazanc], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamOncekiSayfaToplamPrimeEsasKazanc]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamOncekiSayfaToplamPrimOdemeGunu], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamOncekiSayfaToplamPrimOdemeGunu]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamOncekiSayfaToplamSigortaliSayisi], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamOncekiSayfaToplamSigortaliSayisi]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamTumSayfalarToplamPrimeEsasKazanc], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamTumSayfalarToplamPrimeEsasKazanc]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamTumSayfalarToplamPrimOdemeGunu], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamTumSayfalarToplamPrimOdemeGunu]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamTumSayfalarToplamSigortaliSayisi], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamTumSayfalarToplamSigortaliSayisi]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamSayfaAciklama], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamSayfaAciklama]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Isveren], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Isveren]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenTckimlikNo], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenTckimlikNo]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenAdiSoyadi], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenAdiSoyadi]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenUnvan], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenUnvan]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenAdres], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenAdres]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenSemt], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenSemt]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIlce], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIlce]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIl], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIl]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenDisKapiNo], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenDisKapiNo]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIcKapiNo], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIcKapiNo]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenPostaKodu], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenPostaKodu]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenTelefon], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenTelefon]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenEposta], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenEposta]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsveren], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsveren]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliyiDevirAlan], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliyiDevirAlan]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenTckimlikNo], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenTckimlikNo]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenAdiSoyadi], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenAdiSoyadi]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenUnvan], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenUnvan]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenAdres], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenAdres]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenSemt], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenSemt]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIlce], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIlce]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIl], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIl]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenDisKapiNo], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenDisKapiNo]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIcKapiNo], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIcKapiNo]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenPostaKodu], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenPostaKodu]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenTelefon], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenTelefon]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenEposta], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenEposta]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BelgeYil], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BelgeYil]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BelgeAy], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BelgeAy]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Asil], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Asil]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Ek], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Ek]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Iptal], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Iptal]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BildirgeBelgeTuru], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BildirgeBelgeTuru]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BildirgeKanun], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BildirgeKanun]]);
                                            DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.ToplamSayfaSayisi], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.ToplamSayfaSayisi]]);


                                            for (int j = 0; j < isyerisicilno.Length; j++)
                                            {
                                                if (j > 25) break;

                                                IlkSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[(Enums.BildirgeHucreleri)Enum.Parse(typeof(Enums.BildirgeHucreleri), "IsyeriSicil" + (j + 1).ToString())], IlkSayfa.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[(Enums.BildirgeHucreleri)Enum.Parse(typeof(Enums.BildirgeHucreleri), "IsyeriSicil" + (j + 1).ToString())]]);
                                                DevamSayfaHucreleri.Add(BildirgeOlusturmaSabitleri.ExcelHucreleri[(Enums.BildirgeHucreleri)Enum.Parse(typeof(Enums.BildirgeHucreleri), "IsyeriSicil" + (j + 1).ToString())], DevamSheet.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[(Enums.BildirgeHucreleri)Enum.Parse(typeof(Enums.BildirgeHucreleri), "IsyeriSicil" + (j + 1).ToString())]]);
                                            }

                                            for (int j = 0; j < isyerisicilno.Length; j++)
                                            {
                                                if (j > 25) break;

                                                IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[(Enums.BildirgeHucreleri)Enum.Parse(typeof(Enums.BildirgeHucreleri), "IsyeriSicil" + (j + 1).ToString())]].Value2 = isyerisicilno[j].ToString();
                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[(Enums.BildirgeHucreleri)Enum.Parse(typeof(Enums.BildirgeHucreleri), "IsyeriSicil" + (j + 1).ToString())]].Value2 = isyerisicilno[j].ToString();
                                            }

                                            IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Isveren]].Value2 = "X";

                                            IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenTckimlikNo]].Value2 = IsverenTcKimlikNo;

                                            IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenAdiSoyadi]].Value2 = IsverenAdSoyad;

                                            IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenUnvan]].Value2 = IsverenUnvan;

                                            IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenAdres]].Value2 = IsverenAdres;

                                            IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenSemt]].Value2 = "Semt:" + IsverenSemt;

                                            IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIlce]].Value2 = "İlçe:" + IsverenIlce;

                                            IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIl]].Value2 = "İl:" + IsverenIl;

                                            IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenDisKapiNo]].Value2 = "Dış Kapı No:" + IsverenDisKapiNo;

                                            IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIcKapiNo]].Value2 = "İç Kapı No:" + IsverenIcKapiNo;

                                            IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenPostaKodu]].Value2 = "Posta Kodu:" + IsverenPostaKodu;

                                            IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenTelefon]].Value2 = IsverenTelefon;

                                            IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenEposta]].Value2 = IsverenEposta;

                                            if (String.IsNullOrEmpty(IsverenAdSoyad))
                                            {

                                                IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsveren]].Value2 = "X";

                                                IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenTckimlikNo]].Value2 = AltIsverenTcKimlikNo;

                                                IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenAdiSoyadi]].Value2 = AltIsverenAdSoyad;

                                                IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenUnvan]].Value2 = AltIsverenUnvan;

                                                IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenAdres]].Value2 = AltIsverenAdres;

                                                IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenSemt]].Value2 = "Semt:" + AltIsverenSemt;

                                                IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIlce]].Value2 = "İlçe:" + AltIsverenIlce;

                                                IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIl]].Value2 = "İl:" + AltIsverenIl;

                                                IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenDisKapiNo]].Value2 = "Dış Kapı No:" + AltIsverenDisKapiNo;

                                                IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIcKapiNo]].Value2 = "İç Kapı No:" + AltIsverenIcKapiNo;

                                                IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenPostaKodu]].Value2 = "Posta Kodu:" + AltIsverenPostaKodu;

                                                IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenTelefon]].Value2 = AltIsverenTelefon;

                                                IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenEposta]].Value2 = AltIsverenEposta;
                                            }


                                            DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Isveren]].Value2 = "X";

                                            DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenTckimlikNo]].Value2 = IsverenTcKimlikNo;

                                            DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenAdiSoyadi]].Value2 = IsverenAdSoyad;

                                            DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenUnvan]].Value2 = IsverenUnvan;

                                            DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenAdres]].Value2 = IsverenAdres;

                                            DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenSemt]].Value2 = "Semt:" + IsverenSemt;

                                            DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIlce]].Value2 = "İlçe:" + IsverenIlce;

                                            DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIl]].Value2 = "İl:" + IsverenIl;

                                            DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenDisKapiNo]].Value2 = "Dış Kapı No:" + IsverenDisKapiNo;

                                            DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIcKapiNo]].Value2 = "İç Kapı No:" + IsverenIcKapiNo;

                                            DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenPostaKodu]].Value2 = "Posta Kodu:" + IsverenPostaKodu;

                                            DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenTelefon]].Value2 = IsverenTelefon;

                                            DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenEposta]].Value2 = IsverenEposta;

                                            if (String.IsNullOrEmpty(IsverenAdSoyad))
                                            {

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsveren]].Value2 = "X";

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenTckimlikNo]].Value2 = AltIsverenTcKimlikNo;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenAdiSoyadi]].Value2 = AltIsverenAdSoyad;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenUnvan]].Value2 = AltIsverenUnvan;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenAdres]].Value2 = AltIsverenAdres;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenSemt]].Value2 = "Semt:" + AltIsverenSemt;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIlce]].Value2 = "İlçe:" + AltIsverenIlce;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIl]].Value2 = "İl:" + AltIsverenIl;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenDisKapiNo]].Value2 = "Dış Kapı No:" + AltIsverenDisKapiNo;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIcKapiNo]].Value2 = "İç Kapı No:" + AltIsverenIcKapiNo;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenPostaKodu]].Value2 = "Posta Kodu:" + AltIsverenPostaKodu;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenTelefon]].Value2 = AltIsverenTelefon;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenEposta]].Value2 = AltIsverenEposta;
                                            }

                                            HafizadanAtilacaklar.AddRange(IlkSayfaHucreleri.Values);
                                            HafizadanAtilacaklar.AddRange(DevamSayfaHucreleri.Values);

                                        }
                                        else
                                        {
                                            IlkSayfa = BildirgeWorkBook.Sheets[1];

                                            DevamSheet = BildirgeWorkBook.Sheets[2];

                                            HafizadanAtilacaklar.AddRange(new List<object> { IlkSayfa, DevamSheet });
                                        }

                                        List<Classes.AphbSatir> satirlar = cikti.satirlar;

                                        int SayfaSayisi = 0;

                                        if (satirlar.Count >= 7)
                                        {
                                            SayfaSayisi = (satirlar.Count - 7) % 25 == 0 ? ((satirlar.Count - 7) / 25) + 1 : ((satirlar.Count - 7) / 25) + 2;
                                        }
                                        else
                                        {
                                            if (satirlar.Count > 0) SayfaSayisi = 1;
                                        }

                                        var eskitoplamSayfa = IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.ToplamSayfaSayisi]].Value2;

                                        var eskiTc = DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliSosyalGuvenlikSicilNo] + BildirgeOlusturmaSabitleri.DevamSayfasiIlkSigortaliSayi.ToString()].Value2;

                                        IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.ToplamSayfaSayisi]].Value2 = SayfaSayisi.ToString();

                                        DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.ToplamSayfaSayisi]].Value2 = SayfaSayisi.ToString();

                                        IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BelgeYil]].Value2 = Yil;

                                        IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BelgeAy]].Value2 = Ay;

                                        IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Asil]].Value2 = cikti.Iptal ? "" : cikti.Asil ? "X" : "";

                                        IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Ek]].Value2 = cikti.Iptal ? "" : !cikti.Asil ? "X" : "";

                                        IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Iptal]].Value2 = cikti.Iptal ? "X" : "";

                                        IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BildirgeBelgeTuru]].Value2 = belgeturu;

                                        IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BildirgeKanun]].Value2 = cikti.Kanun;

                                        IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaBilgiler]].Value2 = cikti.EkBilgiler;


                                        DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BelgeYil]].Value2 = Yil;

                                        DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BelgeAy]].Value2 = Ay;

                                        DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Asil]].Value2 = cikti.Iptal ? "" : cikti.Asil ? "X" : "";

                                        DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Ek]].Value2 = cikti.Iptal ? "" : !cikti.Asil ? "X" : "";

                                        DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Iptal]].Value2 = cikti.Iptal ? "X" : "";

                                        DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BildirgeBelgeTuru]].Value2 = belgeturu;

                                        DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BildirgeKanun]].Value2 = cikti.Kanun;


                                        for (int j = 0; j < isyerisicilno.Length; j++)
                                        {
                                            if (j > 25) break;

                                            IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[(Enums.BildirgeHucreleri)Enum.Parse(typeof(Enums.BildirgeHucreleri), "IsyeriSicil" + (j + 1).ToString())]].Value2 = isyerisicilno[j].ToString();
                                            DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[(Enums.BildirgeHucreleri)Enum.Parse(typeof(Enums.BildirgeHucreleri), "IsyeriSicil" + (j + 1).ToString())]].Value2 = isyerisicilno[j].ToString();
                                        }

                                        int SayfaNo = 1;

                                        Dictionary<int, List<Classes.AphbSatir>> tumSayfaSatirlari = new Dictionary<int, List<Classes.AphbSatir>>();

                                        int take = 7;

                                        int skip = 0;

                                        while (skip < satirlar.Count)
                                        {
                                            take = SayfaNo == 1 ? 7 : 25;

                                            var rows = satirlar.Skip(skip).Take(take).ToList();

                                            if (rows.Count == 0) break;

                                            tumSayfaSatirlari.Add(SayfaNo, rows);

                                            skip += rows.Count;

                                            SayfaNo++;
                                        }

                                        int sayfasayisi = BildirgeWorkBook.Sheets.Count;

                                        for (int i = sayfasayisi; i >= 3; i--)
                                        {
                                            var sheet = (Excel2.Worksheet)BildirgeWorkBook.Sheets[i];

                                            HafizadanAtilacaklar.Add(sheet);

                                            sheet.Delete();
                                        }

                                        if (tumSayfaSatirlari.Count() == 1)
                                        {
                                            //if (eskitoplamSayfa != null && Convert.ToInt32(eskitoplamSayfa) > 1)
                                            if (eskiTc != null)
                                            {
                                                foreach (var item in DevamSayfaHucreleri)
                                                {
                                                    item.Value.Value2 = null;
                                                }

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenSemt]].Value2 = "Semt:";
                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenSemt]].Value2 = "Semt:";
                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIlce]].Value2 = "İlçe:";
                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIlce]].Value2 = "İlçe:";
                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenDisKapiNo]].Value2 = "Dış Kapı No:";
                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenDisKapiNo]].Value2 = "Dış Kapı No:";
                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIl]].Value2 = "İl:";
                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIl]].Value2 = "İl:";
                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIcKapiNo]].Value2 = "İç Kapı No:";
                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIcKapiNo]].Value2 = "İç Kapı No:";
                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenPostaKodu]].Value2 = "Posta Kodu:";
                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenPostaKodu]].Value2 = "Posta Kodu:";
                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenEposta]].Value2 = "                                                                  @";
                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenEposta]].Value2 = "                                                                  @";

                                            }
                                        }
                                        else
                                        {
                                            if (eskiTc == null)
                                            {
                                                #region Devam Sayfası İşveren Bilgilerini Yazma

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Isveren]].Value2 = "X";

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenTckimlikNo]].Value2 = IsverenTcKimlikNo;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenAdiSoyadi]].Value2 = IsverenAdSoyad;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenUnvan]].Value2 = IsverenUnvan;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenAdres]].Value2 = IsverenAdres;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenSemt]].Value2 = "Semt:" + IsverenSemt;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIlce]].Value2 = "İlçe:" + IsverenIlce;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIl]].Value2 = "İl:" + IsverenIl;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenDisKapiNo]].Value2 = "Dış Kapı No:" + IsverenDisKapiNo;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenIcKapiNo]].Value2 = "İç Kapı No:" + IsverenIcKapiNo;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenPostaKodu]].Value2 = "Posta Kodu:" + IsverenPostaKodu;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenTelefon]].Value2 = IsverenTelefon;

                                                DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IsverenEposta]].Value2 = IsverenEposta;

                                                if (String.IsNullOrEmpty(IsverenAdSoyad))
                                                {

                                                    DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsveren]].Value2 = "X";

                                                    DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenTckimlikNo]].Value2 = AltIsverenTcKimlikNo;

                                                    DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenAdiSoyadi]].Value2 = AltIsverenAdSoyad;

                                                    DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenUnvan]].Value2 = AltIsverenUnvan;

                                                    DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenAdres]].Value2 = AltIsverenAdres;

                                                    DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenSemt]].Value2 = "Semt:" + AltIsverenSemt;

                                                    DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIlce]].Value2 = "İlçe:" + AltIsverenIlce;

                                                    DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIl]].Value2 = "İl:" + AltIsverenIl;

                                                    DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenDisKapiNo]].Value2 = "Dış Kapı No:" + AltIsverenDisKapiNo;

                                                    DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenIcKapiNo]].Value2 = "İç Kapı No:" + AltIsverenIcKapiNo;

                                                    DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenPostaKodu]].Value2 = "Posta Kodu:" + AltIsverenPostaKodu;

                                                    DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenTelefon]].Value2 = AltIsverenTelefon;

                                                    DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AltIsverenEposta]].Value2 = AltIsverenEposta;
                                                }

                                                #endregion
                                            }
                                        }

                                        IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaAciklama]].Value2 = "…. sayfadan ibaret bu belgede yazılı bilgilerin işyeri defter ve kayıtlarına uygun olduğunu beyan ve kabul ederiz.";
                                        DevamSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamSayfaAciklama]].Value2 = "…. sayfadan ibaret bu belgede yazılı bilgilerin işyeri defter ve kayıtlarına uygun olduğunu beyan ve kabul ederiz.";

                                        var sayfaIstatistikleri = tumSayfaSatirlari.ToDictionary(x => x.Key, x => new KeyValuePair<decimal, int>(x.Value.Sum(p => (p.Ucret.ToDecimalSgk() + p.Ikramiye.ToDecimalSgk())), x.Value.Sum(p => Convert.ToInt32(p.Gun))));

                                        var tumSayfaSatirlariReversed = tumSayfaSatirlari.Reverse();

                                        foreach (var item in tumSayfaSatirlariReversed)
                                        {
                                            SayfaNo = item.Key;

                                            sayfasatirlari = item.Value;

                                            bool IlkSayfaMi = false;

                                            if (SayfaNo == 1)
                                            {
                                                MySheet = IlkSayfa;

                                                IlkSayfaMi = true;
                                            }
                                            else
                                            {
                                                MySheet = DevamSheet;
                                            }

                                            HafizadanAtilacaklar.Add(MySheet);

                                            var sayfaHucreleri = IlkSayfaMi ? IlkSayfaHucreleri : DevamSayfaHucreleri;

                                            HafizadanAtilacaklar.AddRange(sayfaHucreleri.Values);

                                            try
                                            {
                                                sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SayfaNo]].Value2 = SayfaNo.ToString();

                                                if (IlkSayfaMi)
                                                {

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliSayisi]].Value2 = satirlar.Count.ToString();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.PrimOdemeGunSayisi]].Value2 = ToplamPrimGunSayisi.ToString();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AyIcindeIseGirenler]].Value2 = AyIcindeIseGirenler.ToString();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.AyIcindeIstenCikanlar]].Value2 = AyIcindeIstenCikanlar.ToString();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.KisaVadeliSigortaKollariPrimiToplamKazanc]].Value2 = ToplamKazanc.ToTL();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.MalullukYaslilikSigortasiToplamKazanc]].Value2 = ToplamKazanc.ToTL();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelSaglikSigortasiPrimiToplamKazanc]].Value2 = ToplamKazanc.ToTL();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SosyalGuvenlikDestekPrimiToplamKazanc]].Value2 = ToplamKazanc.ToTL();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IssizlikSigortasiPrimiToplamKazanc]].Value2 = ToplamKazanc.ToTL();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelToplamKazanc]].Value2 = ToplamKazanc.ToTL();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.KisaVadeliSigortaKollariPrimiOrani]].Value2 = kisaVadeliSigortaKoluOrani.ToString();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.MalullukYaslilikSigortasiPrimOrani]].Value2 = (MalullukPrimOraniSigortali + MalullukPrimOraniIsveren).ToString();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelSaglikSigortasiPrimOrani]].Value2 = (GenelSaglikSigortasiPrimOraniSigortali + GenelSaglikSigortasiPrimOraniIsveren).ToString();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SosyalGuvenlikDestekPrimOrani]].Value2 = (SosyalGuvenlikDestekPrimOraniSigortali + SosyalGuvenlikDestekPrimOraniIsveren).ToString();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IssizlikSigortasiPrimOrani]].Value2 = (IssizlikSigortasiPrimOraniSigortali + IssizlikSigortasiPrimOraniIsveren).ToString();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelToplamPrimOrani]].Value2 = GenelToplamPrimOrani.ToString();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.KisaVadeliSigortaKollariPrimTutari]].Value2 = (ToplamKazanc * kisaVadeliSigortaKoluOrani / 100).ToTL();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.MalullukYaslilikSigortasiPrimTutari]].Value2 = (ToplamKazanc * (MalullukPrimOraniSigortali + MalullukPrimOraniIsveren) / 100).ToTL();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelSaglikSigortasiPrimTutari]].Value2 = (ToplamKazanc * (GenelSaglikSigortasiPrimOraniSigortali + GenelSaglikSigortasiPrimOraniIsveren) / 100).ToTL();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SosyalGuvenlikDestekPrimTutari]].Value2 = (ToplamKazanc * (SosyalGuvenlikDestekPrimOraniSigortali + SosyalGuvenlikDestekPrimOraniIsveren) / 100).ToTL();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IssizlikSigortasiPrimTutari]].Value2 = (ToplamKazanc * (IssizlikSigortasiPrimOraniSigortali + IssizlikSigortasiPrimOraniIsveren) / 100).ToTL();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.GenelToplamPrimTutari]].Value2 = (ToplamKazanc * GenelToplamPrimOrani / 100).ToTL();
                                                }

                                                //BaslangicNo = IlkSayfaMi ? BildirgeOlusturmaSabitleri.IlkSigortaliSayi : BildirgeOlusturmaSabitleri.DevamSayfasiIlkSigortaliSayi;

                                                int sayfakisisayisi = IlkSayfaMi ? 7 : 25;

                                                Parallel.For(0, sayfakisisayisi, j =>
                                                {
                                                    try
                                                    {
                                                        int BaslangicSira = (IlkSayfaMi ? BildirgeOlusturmaSabitleri.IlkSigortaliSayi : BildirgeOlusturmaSabitleri.DevamSayfasiIlkSigortaliSayi) + j * BildirgeOlusturmaSabitleri.IlkSigortaliArtis;


                                                        string TcKimlikNo = null, Ad = null, Soyad = null, IlkSoyad = null, GunSayisi = null,
                                                            Ucret = null, Ikramiye = null, AyIcindeIseGirisTarihiAy = null, AyIcindeIseGirisTarihiGun = null,
                                                            AyIcindeIstenCikisTarihiAy = null, AyIcindeIstenCikisTarihiGun = null, EksikGunSayisi = null,
                                                            EksikGunNedeni = null, IstenCikisNedeni = null, DonusturulenKanun = null, TesvikKanun = null, YeniGun = "", DonusturulecekGun = "";

                                                        if (j < sayfasatirlari.Count)
                                                        {

                                                            TcKimlikNo = sayfasatirlari[j].SosyalGuvenlikNo;

                                                            Ad = sayfasatirlari[j].Adi;

                                                            Soyad = sayfasatirlari[j].Soyadi;

                                                            IlkSoyad = sayfasatirlari[j].IlkSoyadi;

                                                            GunSayisi = sayfasatirlari[j].Gun;

                                                            Ucret = sayfasatirlari[j].Ucret.ToDecimalSgk().ToTL();

                                                            Ikramiye = sayfasatirlari[j].Ikramiye.ToDecimalSgk().ToTL();

                                                            DateTime AyIcindeIseGirisTarihi = DateTime.MinValue;

                                                            string gtarih = sayfasatirlari[j].GirisGunu;

                                                            if (!String.IsNullOrEmpty(gtarih))
                                                            {
                                                                try
                                                                {
                                                                    AyIcindeIseGirisTarihi = Convert.ToDateTime(gtarih + "/" + Yil);

                                                                }
                                                                catch
                                                                {
                                                                    AyIcindeIseGirisTarihi = DateTime.FromOADate(Convert.ToDouble(gtarih));

                                                                    AyIcindeIseGirisTarihi = new DateTime(Convert.ToInt32(Yil), AyIcindeIseGirisTarihi.Month, AyIcindeIseGirisTarihi.Day);

                                                                }
                                                            }

                                                            DateTime AyIcindeIstenCikisTarihi = DateTime.MinValue;

                                                            string cikistarih = sayfasatirlari[j].CikisGunu;

                                                            if (!String.IsNullOrEmpty(cikistarih))
                                                            {
                                                                try
                                                                {
                                                                    AyIcindeIstenCikisTarihi = Convert.ToDateTime(cikistarih + "/" + Yil);

                                                                }
                                                                catch
                                                                {
                                                                    AyIcindeIstenCikisTarihi = DateTime.FromOADate(Convert.ToDouble(cikistarih));

                                                                    AyIcindeIstenCikisTarihi = new DateTime(Convert.ToInt32(Yil), AyIcindeIstenCikisTarihi.Month, AyIcindeIstenCikisTarihi.Day);
                                                                }
                                                            }

                                                            AyIcindeIseGirisTarihiAy = AyIcindeIseGirisTarihi != DateTime.MinValue ? AyIcindeIseGirisTarihi.Month.ToString() : "";

                                                            AyIcindeIseGirisTarihiGun = AyIcindeIseGirisTarihi != DateTime.MinValue ? AyIcindeIseGirisTarihi.Day.ToString() : "";

                                                            AyIcindeIstenCikisTarihiAy = AyIcindeIstenCikisTarihi != DateTime.MinValue ? AyIcindeIstenCikisTarihi.Month.ToString() : "";

                                                            AyIcindeIstenCikisTarihiGun = AyIcindeIstenCikisTarihi != DateTime.MinValue ? AyIcindeIstenCikisTarihi.Day.ToString() : "";

                                                            EksikGunNedeni = sayfasatirlari[j].EksikGunNedeni;

                                                            EksikGunSayisi = !String.IsNullOrEmpty(sayfasatirlari[j].EksikGunSayisi) && Convert.ToInt32(sayfasatirlari[j].EksikGunSayisi) > 0 ? sayfasatirlari[j].EksikGunSayisi.ToString() : "";

                                                            IstenCikisNedeni = sayfasatirlari[j].IstenCikisNedeni;

                                                            DonusturulenKanun = sayfasatirlari[j].Kanun;

                                                            TesvikKanun = cikti.Kisiler.FirstOrDefault(p => p.Key.TckimlikNo.Equals(TcKimlikNo)).Value;

                                                            if (sayfasatirlari[j].satirBolunecek)
                                                            {
                                                                var bolunenSatir = sayfasatirlari[j].BolunecekSatirlar.FirstOrDefault(p => p.TesvikVerilecekMi);

                                                                YeniGun = bolunenSatir.HesaplananGun;
                                                            }
                                                            else YeniGun = sayfasatirlari[j].HesaplananGun;

                                                            DonusturulecekGun = sayfasatirlari[j].HesaplananDonusecekGun;

                                                        }

                                                        sayfaHucreleri["A" + BaslangicSira.ToString()].Value2 = (DonusturulenKanun != null && TesvikKanun != null) ? (DonusturulenKanun + "-" + TesvikKanun + "-" + YeniGun + "-" + DonusturulecekGun) : null;

                                                        sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliSosyalGuvenlikSicilNo] + BaslangicSira.ToString()].Value2 = TcKimlikNo;

                                                        sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliAdi] + BaslangicSira.ToString()].Value2 = Ad;

                                                        sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliSoyadi] + BaslangicSira.ToString()].Value2 = Soyad;

                                                        sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIlkSoyadi] + BaslangicSira.ToString()].Value2 = IlkSoyad;

                                                        sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliPrimOdemeGunu] + BaslangicSira.ToString()].Value2 = GunSayisi;

                                                        sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliUcret] + BaslangicSira.ToString()].Value2 = Ucret;

                                                        sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIkramiye] + BaslangicSira.ToString()].Value2 = Ikramiye;

                                                        sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIseGirisTarihiAy] + BaslangicSira.ToString()].Value2 = AyIcindeIseGirisTarihiAy;

                                                        sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIseGirisTarihiGun] + BaslangicSira.ToString()].Value2 = AyIcindeIseGirisTarihiGun;

                                                        sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIstenCikisTarihiAy] + BaslangicSira.ToString()].Value2 = AyIcindeIstenCikisTarihiAy;

                                                        sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIstenCikisTarihiGun] + BaslangicSira.ToString()].Value2 = AyIcindeIstenCikisTarihiGun;

                                                        sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliEksikGunSayisi] + BaslangicSira.ToString()].Value2 = EksikGunSayisi;

                                                        sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliEksikGunNedeni] + BaslangicSira.ToString()].Value2 = EksikGunNedeni;

                                                        sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIstenCikisNedeni] + BaslangicSira.ToString()].Value2 = IstenCikisNedeni;

                                                        //BaslangicSira += BildirgeOlusturmaSabitleri.IlkSigortaliArtis;
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Metodlar.HataMesajiGoster(ex, "Hata meydana geldi");
                                                        throw ex;
                                                    }
                                                });

                                                var BuSayfaToplamKazanc = sayfaIstatistikleri[SayfaNo].Key;

                                                var BuSayfaToplamGun = sayfaIstatistikleri[SayfaNo].Value;

                                                if (IlkSayfaMi)
                                                {

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaToplamPrimeEsasKazanc]].Value2 = BuSayfaToplamKazanc.ToTL();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaToplamPrimOdemeGunu]].Value2 = BuSayfaToplamGun.ToString();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaToplamSigortaliSayisi]].Value2 = sayfasatirlari.Count.ToString();

                                                    IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaAciklama]].Value2 = IlkSayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaAciklama]].Value2.ToString().Replace("….", SayfaSayisi.ToString());


                                                }
                                                else
                                                {
                                                    var AraToplamKazanc = sayfaIstatistikleri.Where(p => p.Key < SayfaNo).Sum(p => p.Value.Key);

                                                    var AraToplamPrimGunu = sayfaIstatistikleri.Where(p => p.Key < SayfaNo).Sum(p => p.Value.Value);

                                                    var AraToplamSigortaliSayisi = tumSayfaSatirlari.Where(p => p.Key < SayfaNo).Sum(p => p.Value.Count);

                                                    sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamBuSayfaToplamPrimeEsasKazanc]].Value2 = sayfaIstatistikleri[SayfaNo].Key.ToTL();

                                                    sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamBuSayfaToplamPrimOdemeGunu]].Value2 = sayfaIstatistikleri[SayfaNo].Value.ToString();

                                                    sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamBuSayfaToplamSigortaliSayisi]].Value2 = sayfasatirlari.Count.ToString();

                                                    sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamOncekiSayfaToplamPrimeEsasKazanc]].Value2 = AraToplamKazanc.ToTL();

                                                    sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamOncekiSayfaToplamPrimOdemeGunu]].Value2 = AraToplamPrimGunu.ToString();

                                                    sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamOncekiSayfaToplamSigortaliSayisi]].Value2 = AraToplamSigortaliSayisi.ToString();

                                                    sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamTumSayfalarToplamPrimeEsasKazanc]].Value2 = (BuSayfaToplamKazanc + AraToplamKazanc).ToTL();

                                                    sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamTumSayfalarToplamPrimOdemeGunu]].Value2 = (BuSayfaToplamGun + AraToplamPrimGunu).ToString();

                                                    sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamTumSayfalarToplamSigortaliSayisi]].Value2 = (sayfasatirlari.Count + AraToplamSigortaliSayisi).ToString();

                                                    sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamSayfaAciklama]].Value2 = sayfaHucreleri[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamSayfaAciklama]].Value2.ToString().Replace("….", SayfaSayisi.ToString());
                                                }

                                                if (SayfaNo > 2)
                                                {
                                                    var sheetdevam = BildirgeWorkBook.Sheets[2];

                                                    MySheet.Copy(After: sheetdevam);

                                                    HafizadanAtilacaklar.Add(sheetdevam);

                                                    var sheet = BildirgeWorkBook.Sheets[3];

                                                    sheet.Name = SayfaNo.ToString() + ".Sayfa";

                                                    HafizadanAtilacaklar.Add(sheet);

                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Metodlar.HataMesajiGoster(ex, "");
                                            }
                                        }

                                        try
                                        {
                                            Excel2.Worksheet sheet = (Excel2.Worksheet)BildirgeWorkBook.Sheets[1];

                                            sheet.Activate();

                                            sheet.Select(Type.Missing);

                                            HafizadanAtilacaklar.Add(sheet);
                                        }
                                        catch { }



                                        if (!Directory.Exists(isyeriSavePath)) Directory.CreateDirectory(isyeriSavePath);

                                        var cariAyKlasoruneKaydet = CariAyMi(Yil.ToInt(), Ay.ToInt()); //  enbuyukay.Year.ToString() == Yil && enbuyukay.Month.ToString().PadLeft(2, '0') == Ay.PadLeft(2, '0');

                                        if (!cariAyKlasoruneKaydet || string.IsNullOrEmpty(txtYil))
                                        {
                                            string dir = Path.Combine(isyeriSavePath, Yil + "-" + Ay.PadLeft(2, '0'));

                                            if (!Directory.Exists(dir))
                                            {
                                                Directory.CreateDirectory(dir);
                                            }

                                            try
                                            {
                                                var savepath = Path.Combine(dir, Yil + "-" + Ay.PadLeft(2, '0') + " Dönemi " + cikti.Kanun + " " + (cikti.Iptal ? "İptal" : cikti.Asil ? "Asıl" : "Ek") + " Bildirge - Belge " + belgeturu + ".xls");

                                                BildirgeWorkBook.SaveAs(savepath);

                                                if (cikti.Kanun.EndsWith("7252") && !cikti.Iptal)
                                                {
                                                    if (!string.IsNullOrEmpty(liste7252))
                                                    {
                                                        if (!File.Exists(Path.Combine(dir, Path.GetFileName(liste7252))))
                                                        {
                                                            File.Copy(liste7252, Path.Combine(dir, Path.GetFileName(liste7252)));
                                                        }
                                                    }
                                                }

                                                basariliolanlar.Add(Path.Combine(dir, Yil + "-" + Ay.PadLeft(2, '0') + " Dönemi " + cikti.Kanun + " " + (cikti.Iptal ? "İptal" : cikti.Asil ? "Asıl" : "Ek") + " Bildirge - Belge " + belgeturu + ".xls"));

                                            }
                                            catch
                                            {
                                                hatalar.Add(Path.Combine(dir, Yil + "-" + Ay.PadLeft(2, '0') + " Dönemi " + cikti.Kanun + " " + (cikti.Iptal ? "İptal" : cikti.Asil ? "Asıl" : "Ek") + " Bildirge - Belge " + belgeturu + ".xls"));
                                            }


                                        }

                                        if (!cikti.Iptal && cariAyKlasoruneKaydet)
                                        {

                                            string dir2 = Path.Combine(isyeriSavePath, Yil + "-" + Ay.PadLeft(2, '0') + ". Ay Teşvik Çalışması");

                                            if (!Directory.Exists(dir2))
                                            {
                                                Directory.CreateDirectory(dir2);
                                            }

                                            try
                                            {
                                                var savepath = Path.Combine(dir2, Yil + "-" + Ay.PadLeft(2, '0') + " Donemi " + cikti.Kanun + " " + (cikti.Iptal ? "Iptal" : cikti.Asil ? "Asil" : "Ek") + " Bildirge - Belge " + belgeturu + ".xls");

                                                BildirgeWorkBook.SaveAs(savepath);

                                                if (cikti.Kanun.EndsWith("7252") && !cikti.Iptal)
                                                {
                                                    if (!string.IsNullOrEmpty(liste7252))
                                                    {
                                                        if (!File.Exists(Path.Combine(dir2, Path.GetFileName(liste7252))))
                                                        {
                                                            File.Copy(liste7252, Path.Combine(dir2, Path.GetFileName(liste7252)));
                                                        }
                                                    }
                                                }

                                                basariliolanlar.Add(Path.Combine(dir2, Yil + "-" + Ay.PadLeft(2, '0') + " Dönemi " + cikti.Kanun + " " + (cikti.Iptal ? "İptal" : cikti.Asil ? "Asıl" : "Ek") + " Bildirge - Belge " + belgeturu + ".xls"));

                                            }
                                            catch
                                            {
                                                hatalar.Add(Path.Combine(dir2, Yil + "-" + Ay.PadLeft(2, '0') + " Dönemi " + cikti.Kanun + " " + (cikti.Iptal ? "İptal" : cikti.Asil ? "Asıl" : "Ek") + " Bildirge - Belge " + belgeturu + ".xls"));
                                            }
                                        }

                                    }

                                }

                                if (cikti.XmlOlustur && CariAphbOtomatikOlusturuldu == false)
                                {
                                    #region Xml Çıktısı

                                    if (string.IsNullOrEmpty(cariAphbYolu))
                                    {

                                        XmlDocument bildirgexml = new XmlDocument();

                                        bildirgexml.Load(Application.StartupPath + "/" + "bildirge.xml");

                                        XmlNode isyerinode = bildirgexml.GetElementsByTagName("ISYERI")[0];

                                        isyerinode.Attributes["ISYERISICIL"].Value = IsyeriSicilNo.Remove(IsyeriSicilNo.Length - 2, 2);

                                        isyerinode.Attributes["KONTROLNO"].Value = IsyeriSicilNo.Substring(IsyeriSicilNo.Length - 2, 2);

                                        isyerinode.Attributes["ISYERIARACINO"].Value = "000";

                                        isyerinode.Attributes["ISYERIUNVAN"].Value = IsyeriAd.Length <= 50 ? IsyeriAd : IsyeriAd.Substring(0, 50);

                                        isyerinode.Attributes["ISYERIADRES"].Value = IsverenAdres.Length <= 50 ? IsverenAdres : IsverenAdres.Substring(0, 50); ;

                                        isyerinode.Attributes["ISYERIVERGINO"].Value = IsyeriVergiNo;

                                        XmlNode bordro = bildirgexml.GetElementsByTagName("BORDRO")[0];

                                        bordro.Attributes["DONEMAY"].Value = Ay.PadLeft(2, '0');

                                        bordro.Attributes["DONEMYIL"].Value = Yil;

                                        bordro.Attributes["BELGEMAHIYET"].Value = (cikti.Iptal ? "İ" : cikti.Asil ? "A" : "E");

                                        XmlNode bildirgeler = bildirgexml.GetElementsByTagName("BILDIRGELER")[0];

                                        bildirgeler.Attributes["BELGETURU"].Value = belgeturu.PadLeft(2, '0');

                                        bildirgeler.Attributes["KANUN"].Value = cikti.Kanun.PadLeft(5, '0');

                                        XmlNode sigortalilar = bildirgexml.GetElementsByTagName("SIGORTALILAR")[0];

                                        sigortalilar.RemoveAll();

                                        int SiraNo = 1;

                                        foreach (var satir in cikti.satirlar)
                                        {
                                            XmlElement elem = bildirgexml.CreateElement("SIGORTALI");

                                            XmlAttribute sirano = bildirgexml.CreateAttribute("SIRA");

                                            sirano.Value = (SiraNo).ToString();

                                            SiraNo++;

                                            elem.Attributes.Append(sirano);

                                            XmlAttribute tckno = bildirgexml.CreateAttribute("TCKNO");

                                            tckno.Value = satir.SosyalGuvenlikNo;

                                            elem.Attributes.Append(tckno);

                                            XmlAttribute ad = bildirgexml.CreateAttribute("AD");

                                            ad.Value = satir.Adi;

                                            elem.Attributes.Append(ad);

                                            XmlAttribute soyad = bildirgexml.CreateAttribute("SOYAD");

                                            soyad.Value = satir.Soyadi;

                                            elem.Attributes.Append(soyad);

                                            if (satir.IlkSoyadi != null && !String.IsNullOrEmpty(satir.IlkSoyadi))
                                            {

                                                XmlAttribute ilksoyad = bildirgexml.CreateAttribute("ILKSOYAD");

                                                ilksoyad.Value = satir.IlkSoyadi;

                                                elem.Attributes.Append(ilksoyad);
                                            }



                                            XmlAttribute pek = bildirgexml.CreateAttribute("PEK");

                                            pek.Value = (satir.Ucret.ToDecimalSgk() + satir.Ikramiye.ToDecimalSgk()).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

                                            elem.Attributes.Append(pek);

                                            if (satir.Ikramiye.ToDecimalSgk() > 0)
                                            {
                                                XmlAttribute ikramiye = bildirgexml.CreateAttribute("PRIM_IKRAMIYE");

                                                ikramiye.Value = satir.Ikramiye.ToDecimalSgk().ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

                                                elem.Attributes.Append(ikramiye);

                                            }

                                            XmlAttribute gun = bildirgexml.CreateAttribute("GUN");

                                            gun.Value = satir.Gun.ToString();

                                            elem.Attributes.Append(gun);

                                            if (!String.IsNullOrEmpty(satir.GirisGunu))
                                            {
                                                DateTime AyIcindeIseGirisTarihi = DateTime.MinValue;

                                                try
                                                {
                                                    AyIcindeIseGirisTarihi = Convert.ToDateTime(satir.GirisGunu + "/" + Yil);

                                                    AyIcindeIseGirisTarihi = new DateTime(Convert.ToInt32(Yil), AyIcindeIseGirisTarihi.Month, AyIcindeIseGirisTarihi.Day);

                                                }
                                                catch
                                                {

                                                    AyIcindeIseGirisTarihi = DateTime.FromOADate(Convert.ToDouble(satir.GirisGunu));

                                                    AyIcindeIseGirisTarihi = new DateTime(Convert.ToInt32(Yil), AyIcindeIseGirisTarihi.Month, AyIcindeIseGirisTarihi.Day);

                                                }

                                                XmlAttribute girisgun = bildirgexml.CreateAttribute("GIRISGUN");

                                                girisgun.Value = AyIcindeIseGirisTarihi.Day.ToString().PadLeft(2, '0') + AyIcindeIseGirisTarihi.Month.ToString().PadLeft(2, '0');

                                                elem.Attributes.Append(girisgun);
                                            }

                                            if (!String.IsNullOrEmpty(satir.EksikGunSayisi) && satir.EksikGunSayisi != "0")
                                            {
                                                XmlAttribute eksikgunnedeni = bildirgexml.CreateAttribute("EKSIKGUNNEDENI");

                                                eksikgunnedeni.Value = satir.EksikGunNedeni.ToString();

                                                elem.Attributes.Append(eksikgunnedeni);


                                                XmlAttribute eksikgun = bildirgexml.CreateAttribute("EKSIKGUNSAYISI");

                                                eksikgun.Value = satir.EksikGunSayisi.ToString();

                                                elem.Attributes.Append(eksikgun);

                                            }

                                            if (!String.IsNullOrEmpty(satir.CikisGunu))
                                            {
                                                DateTime AyIcindeCikisTarihi = DateTime.MinValue;

                                                try
                                                {
                                                    AyIcindeCikisTarihi = Convert.ToDateTime(satir.CikisGunu + "/" + Yil);

                                                    AyIcindeCikisTarihi = new DateTime(Convert.ToInt32(Yil), AyIcindeCikisTarihi.Month, AyIcindeCikisTarihi.Day);

                                                }
                                                catch
                                                {

                                                    AyIcindeCikisTarihi = DateTime.FromOADate(Convert.ToDouble(satir.CikisGunu));

                                                    AyIcindeCikisTarihi = new DateTime(Convert.ToInt32(Yil), AyIcindeCikisTarihi.Month, AyIcindeCikisTarihi.Day);
                                                }


                                                XmlAttribute cikisgun = bildirgexml.CreateAttribute("CIKISGUN");

                                                cikisgun.Value = AyIcindeCikisTarihi.Day.ToString().PadLeft(2, '0') + AyIcindeCikisTarihi.Month.ToString().PadLeft(2, '0');

                                                elem.Attributes.Append(cikisgun);

                                                XmlAttribute cikisnedeni = bildirgexml.CreateAttribute("ISTENCIKISNEDENI");

                                                cikisnedeni.Value = satir.IstenCikisNedeni;

                                                elem.Attributes.Append(cikisnedeni);
                                            }

                                            XmlAttribute meslekkod = bildirgexml.CreateAttribute("MESLEKKOD");

                                            meslekkod.Value = satir.MeslekKod;

                                            elem.Attributes.Append(meslekkod);

                                            sigortalilar.AppendChild(elem);

                                            if (!String.IsNullOrEmpty(satir.SiraNo) && satir.SiraNo.StartsWith("*"))
                                            {
                                                XmlAttribute rapcalisti = bildirgexml.CreateAttribute("RAPCALISTI");

                                                rapcalisti.Value = "2";

                                                elem.Attributes.Append(rapcalisti);
                                            }

                                        }

                                        if (!Directory.Exists(Path.Combine(Application.StartupPath, "output"))) Directory.CreateDirectory(Path.Combine(Application.StartupPath, "output"));

                                        if (!Directory.Exists(isyeriSavePath)) Directory.CreateDirectory(isyeriSavePath);

                                        string dir = Path.Combine(isyeriSavePath, Yil + "-" + Ay.PadLeft(2, '0') + ". Ay Teşvik Çalışması");

                                        if (!Directory.Exists(dir))
                                        {
                                            Directory.CreateDirectory(dir);
                                        }

                                        bildirgexml.Save(Path.Combine(dir, Yil + "-" + Ay.PadLeft(2, '0') + " Donemi " + cikti.Kanun + " " + (cikti.Iptal ? "Iptal" : cikti.Asil ? "Asil" : "Ek") + " Bildirge - Belge " + belgeturu + ".xml"));

                                        if (cikti.Kanun.EndsWith("7252") && !cikti.Iptal)
                                        {
                                            if (!string.IsNullOrEmpty(liste7252))
                                            {
                                                if (!File.Exists(Path.Combine(dir, Path.GetFileName(liste7252))))
                                                {
                                                    File.Copy(liste7252, Path.Combine(dir, Path.GetFileName(liste7252)));
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        foreach (var aphbSatir in cikti.muhtasarSatirlar)
                                        {
                                            //if (aphbSatir.satirBolunecek)
                                            //{
                                            //    var xmlSatir = aphbSatir.xElement;

                                            //    if (xmlSatir != null)
                                            //    {
                                            //        for (int i = 0; i < aphbSatir.BolunecekSatirlar.Count; i++)
                                            //        {
                                            //            var aphbSatirBolunecek = aphbSatir.BolunecekSatirlar[i];

                                            //            var xmlSatirCopy = i == 0 ? xmlSatir : xmlSatir.Clone();

                                            //            xmlSatirCopy.Element("kanun").SetValue(aphbSatirBolunecek.Kanun);
                                            //            xmlSatirCopy.Element("gun").SetValue(aphbSatirBolunecek.Gun);
                                            //            xmlSatirCopy.Element("hakEdilenUcret").SetValue(aphbSatirBolunecek.Ucret.Replace(",", "."));
                                            //            xmlSatirCopy.Element("primIkramiye").SetValue(aphbSatirBolunecek.Ikramiye.Replace(",", "."));

                                            //            if (xmlSatirCopy.Element("eksikGunSayisi") == null)
                                            //            {
                                            //                xmlSatirCopy.Element("primIkramiye").AddAfterSelf(new XElement("eksikGunSayisi", aphbSatirBolunecek.EksikGunSayisi));
                                            //            }
                                            //            else
                                            //            {
                                            //                xmlSatirCopy.Element("eksikGunSayisi").SetValue(aphbSatirBolunecek.EksikGunSayisi);
                                            //            }

                                            //            if (xmlSatirCopy.Element("eksikGunNedeni") == null)
                                            //            {
                                            //                xmlSatirCopy.Element("eksikGunSayisi").AddAfterSelf(new XElement("eksikGunNedeni", aphbSatirBolunecek.EksikGunNedeni));
                                            //            }
                                            //            else
                                            //            {
                                            //                xmlSatirCopy.Element("eksikGunNedeni").SetValue(aphbSatirBolunecek.EksikGunNedeni);
                                            //            }


                                            //            if (i > 0)
                                            //            {
                                            //                if (xmlSatirCopy.Element("gvMuafMi") != null)
                                            //                {
                                            //                    xmlSatirCopy.Element("gvMuafMi").SetValue("");
                                            //                }

                                            //                if (xmlSatirCopy.Element("agi") != null)
                                            //                {
                                            //                    xmlSatirCopy.Element("agi").SetValue("");
                                            //                }

                                            //                if (xmlSatirCopy.Element("ilgiliDonemGvMatrah") != null)
                                            //                {
                                            //                    xmlSatirCopy.Element("ilgiliDonemGvMatrah").SetValue("");
                                            //                }

                                            //                if (xmlSatirCopy.Element("gvKesinti") != null)
                                            //                {
                                            //                    xmlSatirCopy.Element("gvKesinti").SetValue("");
                                            //                }

                                            //                xmlSatir.AddAfterSelf(xmlSatirCopy);
                                            //            }

                                            //        }
                                            //    }

                                            //    var netsisSatir = aphbSatir.NetsisBilgiler ?? aphbSatir.NetsisBilgilerExcel;

                                            //    if (netsisSatir != null)
                                            //    {
                                            //        for (int i = 0; i < aphbSatir.BolunecekSatirlar.Count; i++)
                                            //        {
                                            //            var aphbSatirBolunecek = aphbSatir.BolunecekSatirlar[i];

                                            //            var netsisCopy = i == 0 ? netsisSatir.netsisBilgiler : netsisSatir.netsisBilgiler.Select(p => p).ToArray();

                                            //            netsisCopy[(int)Enums.NetsisHucreBilgileri.Kanun] = aphbSatirBolunecek.Kanun;
                                            //            netsisCopy[(int)Enums.NetsisHucreBilgileri.Gun] = aphbSatirBolunecek.Gun;
                                            //            netsisCopy[(int)Enums.NetsisHucreBilgileri.Ucret] = aphbSatirBolunecek.Ucret;
                                            //            netsisCopy[(int)Enums.NetsisHucreBilgileri.Ikramiye] = aphbSatirBolunecek.Ikramiye;
                                            //            netsisCopy[(int)Enums.NetsisHucreBilgileri.EksikGunSayisi] = aphbSatirBolunecek.EksikGunSayisi;
                                            //            netsisCopy[(int)Enums.NetsisHucreBilgileri.EksikGunNedeni] = aphbSatirBolunecek.EksikGunNedeni;


                                            //            if (i > 0)
                                            //            {
                                            //                netsisCopy[(int)Enums.NetsisHucreBilgileri.GelirVergisindenMuafMi] = "";
                                            //                netsisCopy[(int)Enums.NetsisHucreBilgileri.AsgariGecimIndirimi] = "";
                                            //                netsisCopy[(int)Enums.NetsisHucreBilgileri.GelirVergisiEngellilikOrani] = "";
                                            //                netsisCopy[(int)Enums.NetsisHucreBilgileri.GelirVergisiKesintisi] = "";
                                            //                netsisCopy[(int)Enums.NetsisHucreBilgileri.IlgiliDonemeAitGelirVergisiMatrahi] = "";

                                            //                netsisSatir.NetsisFile.Insert(netsisSatir.NetsisFile.IndexOf(netsisSatir.netsisBilgiler) + 1, netsisCopy);
                                            //            }

                                            //        }
                                            //    }
                                            //}
                                            //else
                                            //{
                                            if (aphbSatir.xElement != null)
                                            {
                                                aphbSatir.xElement.Element("kanun").SetValue(cikti.Kanun);
                                            }
                                            else if (aphbSatir.NetsisBilgiler != null)
                                            {
                                                aphbSatir.NetsisBilgiler.netsisBilgiler[(int)Enums.NetsisHucreBilgileri.Kanun] = cikti.Kanun;
                                            }
                                            else if (aphbSatir.NetsisBilgilerExcel != null)
                                            {
                                                aphbSatir.NetsisBilgilerExcel.netsisBilgiler[(int)Enums.NetsisHucreBilgileri.Kanun] = cikti.Kanun;
                                            }

                                            //}
                                        }

                                        foreach (var aphbSatir in cikti.muhtasarIptalSatirlar)
                                        {
                                            if (aphbSatir.xElement != null)
                                            {
                                                aphbSatir.xElement.Element("kanun").SetValue("00000");
                                            }
                                            else if (aphbSatir.NetsisBilgiler != null)
                                            {
                                                aphbSatir.NetsisBilgiler.netsisBilgiler[(int)Enums.NetsisHucreBilgileri.Kanun] = "00000";
                                            }
                                            else if (aphbSatir.NetsisBilgilerExcel != null)
                                            {
                                                aphbSatir.NetsisBilgilerExcel.netsisBilgiler[(int)Enums.NetsisHucreBilgileri.Kanun] = "00000";
                                            }

                                        }



                                        //foreach (var satir in cikti.satirlar)
                                        //{
                                        //    if (satir.xElement != null)
                                        //    {
                                        //        satir.xElement.Element("kanun").SetValue(cikti.Kanun);
                                        //    }

                                        //    if (satir.NetsisBilgiler != null)
                                        //    {
                                        //        satir.NetsisBilgiler[(int)Enums.NetsisHucreBilgileri.Kanun] = cikti.Kanun;
                                        //    }

                                        //    if (satir.NetsisBilgilerExcel != null)
                                        //    {
                                        //        satir.NetsisBilgilerExcel[(int)Enums.NetsisHucreBilgileri.Kanun] = cikti.Kanun;
                                        //    }
                                        //}
                                    }

                                    #endregion
                                }

                            }

                            index2++;

                            formIsyerleri.ProgressGuncelle(Convert.ToInt32(100 * (hesaplanacakIsyeriSira - 1) / (double)hesaplanacakIsyerleriCount) + Convert.ToInt32((Convert.ToInt32((100 * ((double)index / yilveaylar.Count)) + (((double)100 / yilveaylar.Count) * ((double)index2 / ciktilar.Count))) / hesaplanacakIsyerleriCount)));

                        }
                    }

                    if (devamet)
                    {
                        var isyeriSavePath = Path.Combine(Application.StartupPath, "output", isyeri.SubeAdi);

                        string dircari = Path.Combine(isyeriSavePath, isyeri.SubeAdi + " " + Yil + "-" + Ay.PadLeft(2, '0') + ". Ay Teşvik Çalışması");

                        //if (Tarih2020veSonrasi && muhtasarYil == Yil.ToInt() && muhtasarAy == Ay.ToInt())
                        //{
                        //    if (xmller.Count > 0)
                        //    {
                        //        if (!Directory.Exists(Path.Combine(Application.StartupPath, "output"))) Directory.CreateDirectory(Path.Combine(Application.StartupPath, "output"));

                        //        if (!Directory.Exists(isyeriSavePath)) Directory.CreateDirectory(isyeriSavePath);

                        //        if (!Directory.Exists(dircari))
                        //        {
                        //            Directory.CreateDirectory(dircari);
                        //        }

                        //        var isyeriSiraNo = isyeri.IsyeriSicilNo.Substring(9, 7);
                        //        var isyeriplaka = isyeri.IsyeriSicilNo.Substring(16, 3);
                        //        var araciNo = Convert.ToInt32(isyeri.TaseronNo).ToString().PadLeft(3, '0');


                        //        var sira = 1;
                        //        foreach (var item in xmller)
                        //        {
                        //            var filename = String.Format("{0} {1}-{2} MuhSgk-{3}.xml", isyeri.SubeAdi.TurkceKarakterleriDegistir(), Yil, Ay.PadLeft(2, '0'), sira);

                        //            XDocument doc = new XDocument(item.Value);

                        //            var sgkCalisanBilgileri = doc.Descendants("sgkCalisanBilgileri").FirstOrDefault();

                        //            var calisanKisilerListe = sgkCalisanBilgileri.Descendants("sgkCalisanBilgisi");

                        //            var ind = 0;

                        //            while (ind < calisanKisilerListe.Count())
                        //            {
                        //                var calisan = calisanKisilerListe.ElementAt(ind);
                        //                var siraNo = calisan.Element("isyeriSiraNo").Value;
                        //                var plaka = calisan.Element("isyeriIlKod").Value;
                        //                var araci = calisan.Element("isyeriAraciSiraNo") == null ? isyeri.TaseronNo : calisan.Element("isyeriAraciSiraNo").Value;

                        //                if (!siraNo.Equals(isyeriSiraNo) || !plaka.Equals(isyeriplaka) || !araci.Equals(araciNo)) calisan.Remove();
                        //                else ind++;
                        //            }


                        //            doc.Save(Path.Combine(dircari, filename));


                        //            //item.Value.Save(item.Key);

                        //            sira++;
                        //            //if (!Directory.Exists(zippath))
                        //            //    Directory.CreateDirectory(zippath);

                        //            //item.Value.Save(Path.Combine(zippath, filename));
                        //        }

                        //        //var d = Directory.CreateDirectory(Path.Combine(zippath, "beyanname-inf"));
                        //        //File.Copy(Path.Combine("beyanname-inf", "beyanname.mf"), Path.Combine(d.FullName, "beyanname.mf"));

                        //        //System.IO.Compression.ZipFile.CreateFromDirectory(zippath, Path.Combine(dircari, "bildirgeler.zip"), System.IO.Compression.CompressionLevel.Fastest, false, System.Text.Encoding.UTF8);

                        //        //Directory.Delete(zippath, true);
                        //    }
                        //}

                        DirectoryInfo di = new DirectoryInfo(dircari);

                        if (di.Exists && di.GetFiles().Count() > 0)
                        {
                            var last = ciktilar.Count > 0 ? ciktilar.LastOrDefault(p => p.ExcelOlustur) : null;

                            if (last != null && !last.Iptal)
                            {
                                BildirgeWorkBook.Close(false);

                                BildirgeWorkBook = null;
                            }


                            //if (File.Exists(Path.Combine(isyeriSavePath, Yil + "-" + Ay.PadLeft(2, '0') + ". Ay Teşvik Çalışması.zip")))
                            //{
                            //    File.Delete(Path.Combine(isyeriSavePath, Yil + "-" + Ay.PadLeft(2, '0') + ". Ay Teşvik Çalışması.zip"));
                            //}

                            //System.IO.Compression.ZipFile.CreateFromDirectory(dircari, Path.Combine(isyeriSavePath, Yil + "-" + Ay.PadLeft(2, '0') + ". Ay Teşvik Çalışması.zip"), System.IO.Compression.CompressionLevel.Fastest, false, System.Text.Encoding.UTF8);

                        }
                    }
                }
                //else
                //{
                //    if (Tarih2020veSonrasi)
                //    {
                //        var isyeriSiraNo = isyeri.IsyeriSicilNo.Substring(9, 7);
                //        var isyeriplaka = isyeri.IsyeriSicilNo.Substring(16, 3);
                //        var araciNo = Convert.ToInt32(isyeri.TaseronNo).ToString().PadLeft(3, '0');

                //        if (xmller.Count > 0)
                //        {
                //            if (!Directory.Exists(Path.Combine(Application.StartupPath, "output"))) Directory.CreateDirectory(Path.Combine(Application.StartupPath, "output"));

                //            var isyeriSavePath = Path.Combine(Application.StartupPath, "output", isyeri.SubeAdi);

                //            if (!Directory.Exists(isyeriSavePath)) Directory.CreateDirectory(isyeriSavePath);

                //            //string dircari = Path.Combine(isyeriSavePath, Yil + "-" + Ay.PadLeft(2, '0') + ". Ay Teşvik Çalışması");

                //            //dircari = Path.Combine(isyeriSavePath, Yil + "-" + Ay.PadLeft(2, '0') + ". Ay Teşvik Çalışması");

                //            //if (!Directory.Exists(dircari))
                //            //{
                //            //    Directory.CreateDirectory(dircari);
                //            //}

                //            //foreach (var item in SatirReferanslari)
                //            //{
                //            //    var orijinalKanun= item.Key[(int)Enums.AphbHucreBilgileri.OrijinalKanunNo].ToString();

                //            //    if (orijinalKanun.EndsWith("6111") || orijinalKanun.EndsWith("7103") || orijinalKanun.EndsWith("2828") || orijinalKanun.EndsWith("6645"))
                //            //    {
                //            //        var kanun = item.Key[(int)Enums.AphbHucreBilgileri.Kanun].ToString();

                //            //        item.Value.Element("kanun").SetValue(kanun);
                //            //    }
                //            //}

                //            var sira = 1;
                //            foreach (var item in xmller)
                //            {
                //                var filename = String.Format("{0} {1}-{2} MuhSgk-{3}.xml", isyeri.SubeAdi.TurkceKarakterleriDegistir(), Yil, Ay.PadLeft(2, '0'), sira);

                //                XDocument doc = new XDocument(item.Value);

                //                var sgkCalisanBilgileri = doc.Descendants("sgkCalisanBilgileri").FirstOrDefault();

                //                var calisanKisilerListe = sgkCalisanBilgileri.Descendants("sgkCalisanBilgisi");

                //                var ind = 0;

                //                while (ind < calisanKisilerListe.Count())
                //                {
                //                    var calisan = calisanKisilerListe.ElementAt(ind);
                //                    var siraNo = calisan.Element("isyeriSiraNo").Value;
                //                    var plaka = calisan.Element("isyeriIlKod").Value;
                //                    var araci = calisan.Element("isyeriAraciSiraNo") == null ? isyeri.TaseronNo : calisan.Element("isyeriAraciSiraNo").Value;

                //                    if (!siraNo.Equals(isyeriSiraNo) || !plaka.Equals(isyeriplaka) || !araci.Equals(araciNo)) calisan.Remove();
                //                    else ind++;
                //                }


                //                doc.Save(Path.Combine(isyeriSavePath, filename));


                //                //item.Value.Save(item.Key);

                //                sira++;
                //                //if (!Directory.Exists(zippath))
                //                //    Directory.CreateDirectory(zippath);

                //                //item.Value.Save(Path.Combine(zippath, filename));
                //            }

                //            //var d = Directory.CreateDirectory(Path.Combine(zippath, "beyanname-inf"));
                //            //File.Copy(Path.Combine("beyanname-inf", "beyanname.mf"), Path.Combine(d.FullName, "beyanname.mf"));

                //            //System.IO.Compression.ZipFile.CreateFromDirectory(zippath, Path.Combine(dircari, "bildirgeler.zip"), System.IO.Compression.CompressionLevel.Fastest, false, System.Text.Encoding.UTF8);

                //            //Directory.Delete(zippath, true);
                //        }
                //    }

                //}
            }
            while (!devamet);

            index++;

            formIsyerleri.ProgressGuncelle(Convert.ToInt32(100 * (hesaplanacakIsyeriSira - 1) / (double)hesaplanacakIsyerleriCount) + Convert.ToInt32(100 * ((double)index / yilveaylar.Count) / hesaplanacakIsyerleriCount));

        }

        void WordOlustur(Word.Application wordApp, string IsyeriSosyalGuvenlikKurumu, string IsyeriAd, string IsyeriSicilNo, List<Classes.Cikti> ciktilar, string Kanunlar, string IptalKanunlar, string sablon, string outputname, List<string> yilaylar, ref List<string> hatalar, string isyeriSavePath)
        {
            try
            {

                object fileName = Path.Combine(System.Windows.Forms.Application.StartupPath, sablon);

                Microsoft.Office.Interop.Word.Document aDoc = wordApp.Documents.Open(fileName, Visible: false, ReadOnly: true);

                aDoc.Activate();

                Metodlar.FindAndReplace(wordApp, "(İŞLEM GÖRDÜĞÜ SGK MERKEZİ)", IsyeriSosyalGuvenlikKurumu.ToUpper());

                Metodlar.FindAndReplace(wordApp, "(ŞİRKET/İŞYERİ ADI)", IsyeriAd.ToUpper());

                Metodlar.FindAndReplace(wordApp, "(İŞYERİ NO)", IsyeriSicilNo.ToUpper());

                Metodlar.FindAndReplace(wordApp, "(IPTAL KANUN)", IptalKanunlar.ToLower());

                var Metinler = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => "");
                var MetinIptaller = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => "");

                List<string> yaziyaEklenecekKanunlar = new List<string>();

                foreach (var item in Metinler)
                {
                    var kanun = item.Key;

                    var kanunNolar = String.Join(" ve ", ciktilar.Where(p => !p.Iptal && p.ExcelOlustur && p.Kanun.EndsWith(kanun)).Select(p => p.Kanun).Distinct());

                    if (kanunNolar.Count() > 0)
                    {
                        yaziyaEklenecekKanunlar.Add(kanun);

                        var Iptaller = String.Join(",", TumTesvikler[item.Key].TesvikAyIstatistikleri.SelectMany(p => p.Value.Icmal.Tutarlar).Select(p => p.Key).Distinct());

                        var KanunMetin = String.Format("{0}Yazı ekinde bulunan sigortalılarımız, {1} sigorta primi teşviki kapsamında bulunmasına rağmen yasal süresinde verilen aylık prim ve hizmet belgelerinde, {2} sayılı kanun türü seçilerek bildirilmesi gerektiği halde sehven, {3} sayılı kanun türü seçilerek bildirilmiştir. Bu sigortalılar için ilgili kanunlara göre düzenlenen iptal/asıl aylık prim ve hizmet belgeleri yazı ekinde sunulmuştur.", "\r\n\t", TumTesvikler[item.Key].UstYaziMetni, kanunNolar, Iptaller);

                        var KanunMetinIptal = String.Format("{0} sayılı kanundan yararlandırılması gereken sigortalılar için {1} sayılı kanuna göre düzenlenen “iptal”,", kanunNolar, Iptaller.Trim(',').Replace(", ", " ve "));

                        for (int i = 0; i < KanunMetin.Length; i += 200)
                        {
                            string metin = KanunMetin.Substring(i);

                            if (metin.Length > 200) metin = metin.Substring(0, 200);

                            if (metin.Length < 200)
                            {

                            }
                            else metin += String.Format("({0})", kanun);

                            Metodlar.FindAndReplace(wordApp, String.Format("({0})", kanun), metin);
                        }

                        Metodlar.FindAndReplace(wordApp, String.Format("({0}Iptal)", kanun), KanunMetinIptal);

                    }
                    else
                    {
                        Metodlar.FindAndReplace(wordApp, String.Format("({0})", kanun), "");
                        Metodlar.FindAndReplace(wordApp, String.Format("({0}Iptal)", kanun), "");
                    }
                }


                string Kanun1 = String.Join(",", yaziyaEklenecekKanunlar);

                Metodlar.FindAndReplace(wordApp, "(KANUN1)", Kanun1);

                Metodlar.FindAndReplace(wordApp, "(KANUN2)", Kanunlar.Trim(','));


                yilaylar.Sort();

                string aylar = String.Join(",", yilaylar).Trim(',');

                int sira = 0;

                List<string> aylarliste = new List<string>();

                while (true)
                {
                    if (aylar.Length - sira >= 100)
                    {
                        aylarliste.Add(aylar.Substring(sira, 100));
                    }
                    else
                    {
                        aylarliste.Add(aylar.Substring(sira, aylar.Length - sira));

                        break;
                    }

                    sira += 100;
                }

                for (int m = 0; m < aylarliste.Count; m++)
                {
                    Metodlar.FindAndReplace(wordApp, "(TEŞVİK İÇİN BİLDİRGE HAZIRLANAN AYLAR)", aylarliste[m] + (m == aylarliste.Count - 1 ? "" : "(TEŞVİK İÇİN BİLDİRGE HAZIRLANAN AYLAR)"));

                }

                //Metodlar.FindAndReplace(wordApp, "(TEŞVİK İÇİN BİLDİRGE HAZIRLANAN AYLAR)", ustyaziaylar.Trim(','));

                aDoc.Repaginate();

                try
                {
                    int numberOfPages = Convert.ToInt32(Metodlar.getWordDocumentPropertyValue(aDoc, "Number Of Pages"));

                    if (numberOfPages > 1)
                    {
                        int fontSize = 13;

                        while (true)
                        {
                            if (fontSize < 10) break;

                            var start = aDoc.Content.Start;
                            var end = aDoc.Content.End;

                            var docRange = aDoc.Range(start, end);

                            docRange.Select();

                            docRange.Font.Size = fontSize;

                            aDoc.Repaginate();

                            numberOfPages = Convert.ToInt32(Metodlar.getWordDocumentPropertyValue(aDoc, "Number Of Pages"));

                            if (numberOfPages == 1) break;

                            fontSize--;


                        }

                    }

                }
                catch
                {
                }

                if (!Directory.Exists(isyeriSavePath)) Directory.CreateDirectory(isyeriSavePath);

                aDoc.SaveAs(Path.Combine(isyeriSavePath, outputname));

                aDoc.Close(false);

            }
            catch
            {
                hatalar.Add(Path.Combine(isyeriSavePath, outputname));
            }

        }

    }

}
