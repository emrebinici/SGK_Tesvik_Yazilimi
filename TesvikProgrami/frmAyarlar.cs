using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Threading;
using System.Net;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Xml.Linq;

namespace TesvikProgrami
{
    public partial class frmAyarlar : Form
    {
        public frmAyarlar()
        {
            InitializeComponent();
        }

        private void frmAyarlar_Load(object sender, EventArgs e)
        {
            foreach (var item in Program.TumTesvikler)
            {
                if (item.Value.AsgariUcretDestekTutarlariDikkateAlinsin)
                {
                    CheckBox chk = new CheckBox();
                    chk.Name = "chkAsgariUcretDestekTutarlari" + item.Key;
                    chk.Click += chk_CheckedChanged;
                    chk.Text = item.Key;
                    chk.Tag = item.Key;

                    flowLayoutPanelAsgariUcretDestekTutarlari.Controls.Add(chk);
                }
            }

            cmbEgitimBelgeTurleri.ValueMember = "Key";
            cmbEgitimBelgeTurleri.DisplayMember = "Value";
            cmbEgitimBelgeTurleri.DataSource = EgitimBelgesiAdlari.EgitimBelgesiTurleriAdlari.ToList();

            AyarlariDoldur();


        }

        private void AyarlariDoldur()
        {
            bool Kaydet = false;

            using (var dbContext = new DbEntities())
            {

                var anahtarlar = dbContext.Ayarlar.ToList();

                var elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("IsverenSistemiGuvenlikKoduGirisi"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "IsverenSistemiGuvenlikKoduGirisi", Deger = "False" });

                    Kaydet = true;

                    chkIsverenSistemi.Checked = false;
                }
                else chkIsverenSistemi.Checked = elem.Deger.Equals("True");


                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GuvenlikKoduGirisi6645"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GuvenlikKoduGirisi6645", Deger = "False" });

                    Kaydet = true;

                    chk6645.Checked = false;
                }
                else chk6645.Checked = elem.Deger.Equals("True");


                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GuvenlikKoduGirisi687"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GuvenlikKoduGirisi687", Deger = "False" });

                    Kaydet = true;

                    chk687.Checked = false;
                }
                else chk687.Checked = elem.Deger.Equals("True");


                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("EBildirgeV2GuvenlikKoduGirisi"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "EBildirgeV2GuvenlikKoduGirisi", Deger = "False" });

                    Kaydet = true;

                    chkEBildirgeV2.Checked = false;
                }
                else chkEBildirgeV2.Checked = elem.Deger.Equals("True");


                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GuvenlikKoduGirisi14857"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GuvenlikKoduGirisi14857", Deger = "False" });

                    Kaydet = true;

                    chk14857.Checked = false;
                }
                else chk14857.Checked = elem.Deger.Equals("True");


                foreach (var item in Program.TumTesvikler)
                {

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("AsgariUcretDestekTutariDikkateAlinsin" + item.Key));

                    if (elem == null && item.Value.AsgariUcretDestekTutarlariDikkateAlinsin)
                    {
                        elem = new Ayarlar { Anahtar = "AsgariUcretDestekTutariDikkateAlinsin" + item.Key, Deger = "True" };
                        dbContext.Ayarlar.Add(elem);
                        Kaydet = true;
                    }

                    CheckBox chk = flowLayoutPanelAsgariUcretDestekTutarlari.Controls.Find("chkAsgariUcretDestekTutarlari" + item.Key, true).FirstOrDefault() as CheckBox;

                    if (chk != null)
                    {
                        chk.Checked = elem.Deger.Equals("True");
                    }


                }

                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("MinimumGunSayisi"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "MinimumGunSayisi", Deger = "20" });

                    Kaydet = true;

                }
                else txtMinimumGunSayisi.Text = elem.Deger;


                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BildirgeMinimumTutar"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BildirgeMinimumTutar", Deger = "36" });

                    Kaydet = true;

                }
                else txtBildirgeMinimumTutar.Text = elem.Deger;


                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BildirgelerOnaylansin"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BildirgelerOnaylansin", Deger = "False" });

                    Kaydet = true;

                }
                else chkBildirgelerOnaylansin.Checked = elem.Deger.Equals("True");


                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("AcikKalanExcellerKapansin"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "AcikKalanExcellerKapansin", Deger = "False" });

                    Kaydet = true;

                }
                else chkArkaPlanExcelleriKapatilsin.Checked = elem.Deger.Equals("True");


                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("ZamanasimiSuresi"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "ZamanasimiSuresi", Deger = "100" });

                    Kaydet = true;

                }
                else txtZamanasimiSuresi.Text = elem.Deger;


                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfIndirmeUcretDestegiIstensin"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfIndirmeUcretDestegiIstensin", Deger = "False" });

                    Kaydet = true;

                }
                else chkBfIndirmeUcretDestegiIstensin.Checked = elem.Deger.Equals("True");


                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BasvuruYoksaTesvikVerilmesin"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BasvuruYoksaTesvikVerilmesin", Deger = "True" });

                    Kaydet = true;
                }
                else chkBasvuruYoksaTesvikVerilmesin.Checked = elem.Deger.Equals("True");


                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("SeciliKanunlarDonusturulsun"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "SeciliKanunlarDonusturulsun", Deger = "False" });

                    Kaydet = true;
                }
                else chkSeciliKanunlarDonusturulsun.Checked = elem.Deger.Equals("True");


                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("DonemIslemcisiYeniGirisYapsin"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "DonemIslemcisiYeniGirisYapsin", Deger = "False" });

                    Kaydet = true;
                }
                else chkDonemIslemcisiYeniGirisYapsin.Checked = elem.Deger.Equals("True");


                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("KisiIslemcisiYeniGirisYapsin"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "KisiIslemcisiYeniGirisYapsin", Deger = "False" });

                    Kaydet = true;
                }
                else chkKisiIslemcisiYeniGirisYapsin.Checked = elem.Deger.Equals("True");

                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("DetayliLoglamaYapilsin"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "DetayliLoglamaYapilsin", Deger = "True" });

                    Kaydet = true;
                }
                else chkDetayliLoglamaYapilsin.Checked = elem.Deger.Equals("True");

                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("DonemIslemciSayisi"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "DonemIslemciSayisi", Deger = "1" });

                    Kaydet = true;
                }
                else txtDonemIslemciSayisi.Text = elem.Deger;

                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("KisiIslemciSayisi"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "KisiIslemciSayisi", Deger = "1" });

                    Kaydet = true;
                }
                else txtKisiIslemciSayisi.Text = elem.Deger;

                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GuvenlikKoduCozdur"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GuvenlikKoduCozdur", Deger = "False" });

                    Kaydet = true;
                }
                else chkGuvenlikKoduCozdur.Checked = elem.Deger.Equals("True");

                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfEgitimBelgesi"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfEgitimBelgesi", Deger = "8" });

                    cmbEgitimBelgeTurleri.SelectedValue = 8;

                    Kaydet = true;
                }
                else cmbEgitimBelgeTurleri.SelectedValue = Convert.ToInt32(elem.Deger);

                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("OncekiBildirgelerIptalEdilsin"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "OncekiBildirgelerIptalEdilsin", Deger = "True" });

                    Kaydet = true;
                }
                else chkOncekiBildirgelerIptalEdilsin.Checked = elem.Deger.Equals("True");

                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("EgitimListesiOlusturulsun"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "EgitimListesiOlusturulsun", Deger = "False" });

                    Kaydet = true;
                }
                else chkEgitimListesiOlusturulsun.Checked = elem.Deger.Equals("True");

                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("CariAphbOlusturulsun"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "CariAphbOlusturulsun", Deger = "True" });

                    Kaydet = true;
                }
                else chkCariAphbOlusturulsun.Checked = elem.Deger.Equals("True");

                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("7166ListesiCikarilsin"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "7166ListesiCikarilsin", Deger = "True" });

                    Kaydet = true;
                }
                else chk7166ListesiCikarilsin.Checked = elem.Deger.Equals("True");

                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("Son6AyGecmisHesaplansin"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "Son6AyGecmisHesaplansin", Deger = "True" });

                    Kaydet = true;
                }
                else chkSon6AyGecmisHesaplansin.Checked = elem.Deger.Equals("True");

                elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BasvuruDonemleriCekilsin"));

                if (elem == null)
                {
                    dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BasvuruDonemleriCekilsin", Deger = "False" });

                    Kaydet = true;
                }
                else chkBasvuruDonemleriCekilsin.Checked = elem.Deger.Equals("True");

                if (Kaydet)
                {
                    dbContext.SaveChanges();
                }
            }

        }

        private void btnSecilenleriSil_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Başvuru formlarını silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                var sirketklasorleri = Directory.GetDirectories(Path.Combine(Application.StartupPath, "dosyalar"));

                var isyerleriIDleri = sirketklasorleri.SelectMany(d => Directory.GetDirectories(d).Select(a => a.Split('-')[a.Split('-').Length - 1])).ToList();

                var isyerleriklasorleri = sirketklasorleri.SelectMany(d => Directory.GetDirectories(d).Select(a => a)).ToList();

                List<string> hatalar = new List<string>();

                int toplamSilinenSayisi = 0;


                using (var dbContext = new DbEntities())
                {
                    var isyerleri = dbContext.Isyerleri.Include(p => p.Sirketler).ToList();

                    progressBar1.Visible = isyerleri.Count > 0;

                    for (int i = 0; i < isyerleri.Count; i++)
                    {
                        var isyeri = isyerleri[i];

                        if (isyerleriIDleri.Contains(isyeri.IsyeriID.ToString()))
                        {
                            var bfyol = Metodlar.FormBul(isyeri, Enums.FormTuru.BasvuruFormu);

                            if (!String.IsNullOrEmpty(bfyol))
                            {
                                try
                                {
                                    File.Delete(bfyol);

                                    toplamSilinenSayisi++;

                                    isyeri.BasvuruFormu = null;

                                }
                                catch (Exception)
                                {
                                    hatalar.Add(bfyol);
                                }
                            }
                            else
                            {

                                isyeri.BasvuruFormu = null;
                            }

                            string isyeripath = isyerleriklasorleri.FirstOrDefault(p => p.EndsWith("-" + isyeri.IsyeriID));

                            if (!string.IsNullOrEmpty(isyeripath))
                            {
                                var files = Directory.GetFiles(isyeripath);

                                foreach (var item in files)
                                {
                                    if ((item.ToLower().Contains("başvuru") || item.ToLower().Contains("basvuru")) && item.ToLower().Contains("form"))
                                    {
                                        try
                                        {
                                            File.Delete(item);

                                            toplamSilinenSayisi++;
                                        }
                                        catch
                                        {
                                            if (!hatalar.Contains(item)) hatalar.Add(item);
                                        }
                                    }
                                }
                            }
                        }
                        else isyeri.BasvuruFormu = null;

                        progressBar1.Value = (int)Math.Round(((double)i / isyerleri.Count) * 100);

                    }

                    dbContext.SaveChanges();
                }

                string Mesaj = "Toplam Silinen Dosya Sayısı: " + toplamSilinenSayisi + Environment.NewLine + Environment.NewLine;

                if (hatalar.Count > 0)
                {
                    Mesaj += "Aşağıdaki dosyalar silinemedi.Dosyalar kullanımda olabilir." + Environment.NewLine + string.Join(Environment.NewLine, hatalar);

                }

                MessageBox.Show(Mesaj, "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);

                progressBar1.Visible = false;
            }
        }

        private void Btn7166FormlariniSil_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("7166 formlarını silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                var sirketklasorleri = Directory.GetDirectories(Path.Combine(Application.StartupPath, "dosyalar"));

                var isyerleriIDleri = sirketklasorleri.SelectMany(d => Directory.GetDirectories(d).Select(a => a.Split('-')[a.Split('-').Length - 1])).ToList();

                var isyerleriklasorleri = sirketklasorleri.SelectMany(d => Directory.GetDirectories(d).Select(a => a)).ToList();

                List<string> hatalar = new List<string>();

                int toplamSilinenSayisi = 0;

                using (var dbContext = new DbEntities())
                {
                    var isyerleri = dbContext.Isyerleri.Include(p => p.Sirketler).ToList();

                    progressBar1.Visible = isyerleri.Count > 0;

                    for (int i = 0; i < isyerleri.Count; i++)
                    {
                        var isyeri = isyerleri[i];

                        if (isyerleriIDleri.Contains(isyeri.IsyeriID.ToString()))
                        {
                            var yol7166 = Metodlar.FormBul(isyeri, Enums.FormTuru.BasvuruListesi7166);

                            if (!String.IsNullOrEmpty(yol7166))
                            {
                                try
                                {
                                    File.Delete(yol7166);

                                    toplamSilinenSayisi++;

                                    isyeri.BasvuruListesi7166 = null;

                                }
                                catch (Exception)
                                {
                                    hatalar.Add(yol7166);
                                }
                            }
                            else
                            {

                                isyeri.BasvuruListesi7166 = null;
                            }

                            string isyeripath = isyerleriklasorleri.FirstOrDefault(p => p.EndsWith("-" + isyeri.IsyeriID));

                            if (!string.IsNullOrEmpty(isyeripath))
                            {
                                var files = Directory.GetFiles(isyeripath);

                                foreach (var item in files)
                                {
                                    if (item.ToLower().Contains("7166"))
                                    {
                                        try
                                        {
                                            File.Delete(item);

                                            toplamSilinenSayisi++;
                                        }
                                        catch
                                        {
                                            if (!hatalar.Contains(item)) hatalar.Add(item);
                                        }
                                    }
                                }
                            }
                        }
                        else isyeri.BasvuruListesi7166 = null;

                        progressBar1.Value = (int)Math.Round(((double)i / isyerleri.Count) * 100);

                    }

                    dbContext.SaveChanges();

                }


                string Mesaj = "Toplam Silinen Dosya Sayısı: " + toplamSilinenSayisi + Environment.NewLine + Environment.NewLine;

                if (hatalar.Count > 0)
                {
                    Mesaj += "Aşağıdaki dosyalar silinemedi.Dosyalar kullanımda olabilir." + Environment.NewLine + string.Join(Environment.NewLine, hatalar);

                }

                MessageBox.Show(Mesaj, "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);

                progressBar1.Visible = false;
            }

        }

        private void chk_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Enabled)
            {
                (sender as CheckBox).Enabled = false;

                Ayarlar elem = null;

                using (var dbContext = new DbEntities())
                {
                    var list = dbContext.Ayarlar.ToList();

                    if (sender == chkIsverenSistemi)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("IsverenSistemiGuvenlikKoduGirisi"));

                        Program.OtomatikGuvenlikKoduGirilecekIsverenSistemi = chkIsverenSistemi.Checked;
                    }
                    else if (sender == chk6645)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("GuvenlikKoduGirisi6645"));
                        Program.OtomatikGuvenlikKoduGirilecek6645 = chk6645.Checked;
                    }
                    else if (sender == chk687)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("GuvenlikKoduGirisi687"));
                        Program.OtomatikGuvenlikKoduGirilecek687 = chk687.Checked;

                    }
                    else if (sender == chkEBildirgeV2)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("EBildirgeV2GuvenlikKoduGirisi"));
                        Program.OtomatikGuvenlikKoduGirilecekEBildirgeV2 = chkEBildirgeV2.Checked;

                    }
                    else if (sender == chk14857)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("GuvenlikKoduGirisi14857"));
                        Program.OtomatikGuvenlikKoduGirilecek14857 = chk14857.Checked;

                    }
                    else if (sender != null && (sender as CheckBox).Name.StartsWith("chkAsgariUcretDestekTutarlari"))
                    {
                        foreach (var item in Program.TumTesvikler)
                        {
                            var kanun = item.Key;

                            var tesvik = item.Value;

                            var chk = flowLayoutPanelAsgariUcretDestekTutarlari.Controls.Find("chkAsgariUcretDestekTutarlari" + kanun, true).FirstOrDefault() as CheckBox;

                            if (chk != null)
                            {
                                if (chk.Checked)
                                {

                                    var eleman = list.FirstOrDefault(p => p.Anahtar.Equals("AsgariUcretDestekTutariDikkateAlinsin" + item.Key));

                                    if (eleman == null)
                                    {
                                        eleman = new Ayarlar();
                                        eleman.Anahtar = "AsgariUcretDestekTutariDikkateAlinsin" + item.Key;

                                        dbContext.Ayarlar.Add(eleman);
                                    }

                                    eleman.Deger = "True";
                                }
                                else
                                {
                                    var eleman = list.FirstOrDefault(p => p.Anahtar.Equals("AsgariUcretDestekTutariDikkateAlinsin" + item.Key));

                                    if (eleman != null)
                                    {
                                        eleman.Deger = "False";
                                        //dbContext.Ayarlar.Remove(eleman);
                                    }
                                }

                                Program.AsgariUcretDestekTutariDikkateAlinsin[kanun] = chk.Checked;

                            }
                        }

                    }
                    else if (sender == chkBildirgelerOnaylansin)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("BildirgelerOnaylansin"));
                        Program.BildirgelerOnaylansin = chkBildirgelerOnaylansin.Checked;
                    }
                    else if (sender == chkArkaPlanExcelleriKapatilsin)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("AcikKalanExcellerKapansin"));
                        Program.AcikKalanExcellerKapatilsin = chkArkaPlanExcelleriKapatilsin.Checked;
                    }
                    else if (sender == chkBfIndirmeUcretDestegiIstensin)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("BfIndirmeUcretDestegiIstensin"));
                        Program.BfIndirmeUcretDestegiIstensin = chkBfIndirmeUcretDestegiIstensin.Checked;
                    }
                    else if (sender == chkBasvuruYoksaTesvikVerilmesin)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("BasvuruYoksaTesvikVerilmesin"));
                        Program.BasvuruYoksaTesvikVerilmesin = chkBasvuruYoksaTesvikVerilmesin.Checked;
                    }
                    else if (sender == chkSeciliKanunlarDonusturulsun)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("SeciliKanunlarDonusturulsun"));

                        Program.SeciliKanunlarDonusturulsun = chkSeciliKanunlarDonusturulsun.Checked;

                        Program.TumTesvikler = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new Classes.Tesvik(x));

                    }
                    else if (sender == chkDonemIslemcisiYeniGirisYapsin)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("DonemIslemcisiYeniGirisYapsin"));
                        Program.DonemIslemcisiYeniGirisYapsin = chkDonemIslemcisiYeniGirisYapsin.Checked;
                    }
                    else if (sender == chkKisiIslemcisiYeniGirisYapsin)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("KisiIslemcisiYeniGirisYapsin"));

                        Program.KisiIslemcisiYeniGirisYapsin = chkKisiIslemcisiYeniGirisYapsin.Checked;
                    }
                    else if (sender == chkDetayliLoglamaYapilsin)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("DetayliLoglamaYapilsin"));
                        Program.DetayliLoglamaYapilsin = chkDetayliLoglamaYapilsin.Checked;
                    }
                    else if (sender == chkGuvenlikKoduCozdur)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("GuvenlikKoduCozdur"));
                        Program.GuvenlikKoduCozdur = chkGuvenlikKoduCozdur.Checked;
                    }
                    else if (sender == chkOncekiBildirgelerIptalEdilsin)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("OncekiBildirgelerIptalEdilsin"));

                        Program.OncekiBildirgelerIptalEdilsin = chkOncekiBildirgelerIptalEdilsin.Checked;
                    }
                    else if (sender == chkEgitimListesiOlusturulsun)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("EgitimListesiOlusturulsun"));

                        Program.EgitimListesiOlusturulsun = chkEgitimListesiOlusturulsun.Checked;
                    }
                    else if (sender == chkCariAphbOlusturulsun)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("CariAphbOlusturulsun"));

                        Program.CariAphbOlusturulsun = chkCariAphbOlusturulsun.Checked;
                    }
                    else if (sender == chk7166ListesiCikarilsin)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("7166ListesiCikarilsin"));

                        Program.Liste7166Cikarilsin = chk7166ListesiCikarilsin.Checked;
                    }
                    else if (sender == chkSon6AyGecmisHesaplansin)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("Son6AyGecmisHesaplansin"));

                        Program.Son6AyGecmisHesaplansin = chkSon6AyGecmisHesaplansin.Checked;
                    }
                    else if (sender == chkBasvuruDonemleriCekilsin)
                    {
                        elem = list.FirstOrDefault(p => p.Anahtar.Equals("BasvuruDonemleriCekilsin"));

                        Program.BasvuruDonemleriCekilsin = chkBasvuruDonemleriCekilsin.Checked;
                    }

                    if (!(sender as CheckBox).Name.StartsWith("chkAsgariUcretDestekTutarlari"))
                    {
                        elem.Deger = (sender as CheckBox).Checked ? "True" : "False";
                    }

                    dbContext.SaveChanges();
                }

                (sender as CheckBox).Enabled = true;
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            int i = 0;

            if (Int32.TryParse(txtMinimumGunSayisi.Text.Trim(), out i))
            {
                using (var dbContext = new DbEntities())
                {
                    var elem = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("MinimumGunSayisi"));
                    elem.Deger = txtMinimumGunSayisi.Text;

                    dbContext.SaveChanges();
                }

                Program.MinimumGunSayisi = i;

                MessageBox.Show("Kaydedildi", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir sayı giriniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnBildirgeMinimumTutar_Click(object sender, EventArgs e)
        {
            decimal i = 0;

            if (decimal.TryParse(txtBildirgeMinimumTutar.Text.Trim(), out i))
            {

                using (var dbContext = new DbEntities())
                {
                    var elem = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BildirgeMinimumTutar"));

                    elem.Deger = txtBildirgeMinimumTutar.Text;

                    dbContext.SaveChanges();
                }

                Program.BildirgeMinimumTutar = i;

                MessageBox.Show("Kaydedildi", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir sayı giriniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void btnZamanasimiKaydet_Click(object sender, EventArgs e)
        {
            int i = 0;

            if (Int32.TryParse(txtZamanasimiSuresi.Text.Trim(), out i))
            {
                using (var dbContext = new DbEntities())
                {
                    var elem = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("ZamanasimiSuresi"));

                    elem.Deger = txtZamanasimiSuresi.Text;

                    dbContext.SaveChanges();
                }

                Program.ZamanAsimiSuresi = i;

                MessageBox.Show("Kaydedildi", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir sayı giriniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #region İşyeri Teşvik Başvuruları

        Dictionary<int, Thread> Threads = new Dictionary<int, Thread>();

        List<Isyerleri> isyerleri = null;

        int seciliIsyeri = 0;

        List<string> hataliIsyerleri = new List<string>();

        List<string> basariliIsyerleri = new List<string>();

        frmLog FrmLog = null;

        StringBuilder sb = new StringBuilder();

        Dictionary<int, List<string>> Mesajlar = new Dictionary<int, List<string>>();

        List<string> BakilanIsyerleri = new List<string>();

        int baslangic = 1;

        int bitis = Int32.MaxValue;

        int son = 0;

        public bool TesvikBasvuruYapiliyor = false;

        private void btnTesvikBasvuru_Click(object sender, EventArgs e)
        {
            if (!TesvikBasvuruYapiliyor)
            {
                if (MessageBox.Show("Tüm işyerleri için teşvik başvurusu yapmak istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    progressBar1.Value = 0;

                    seciliIsyeri = 0;

                    baslangic = 1;

                    bitis = Int32.MaxValue;

                    son = 0;

                    TesvikBasvuruYapiliyor = true;

                    Mesajlar = new Dictionary<int, List<string>>();

                    sb = new StringBuilder();

                    hataliIsyerleri = new List<string>();

                    basariliIsyerleri = new List<string>();

                    BakilanIsyerleri = new List<string>();

                    FrmLog = new frmLog(this);

                    Threads = new Dictionary<int, Thread>();

                    using (var dbContext = new DbEntities())
                    {
                        isyerleri = dbContext.Isyerleri.Include(p => p.Sirketler).ToList();
                    }

                    if (File.Exists("BakilanIsyerleri.txt"))
                    {
                        FileInfo fi = new FileInfo("BakilanIsyerleri.txt");

                        if (DateTime.UtcNow.Subtract(fi.LastWriteTimeUtc).TotalDays >= 7)
                        {
                            try
                            {
                                fi.Delete();
                            }
                            catch { }
                        }
                        else
                        {
                            BakilanIsyerleri = File.ReadAllLines("BakilanIsyerleri.txt").ToList();
                        }

                    }

                    if (!String.IsNullOrEmpty(txtBaslangic.Text.Trim()))
                    {
                        baslangic = Convert.ToInt32(txtBaslangic.Text);

                        if (baslangic > 0)
                        {
                            seciliIsyeri = baslangic - 1;
                        }
                    }

                    if (!String.IsNullOrEmpty(txtBitis.Text.Trim()))
                    {
                        bitis = Convert.ToInt32(txtBitis.Text);
                    }

                    son = Math.Min(isyerleri.Count, bitis);

                    for (int i = 0; i < 1; i++)
                    {

                        Thread thread = new Thread(() =>
                        {
                            BasvuruSayfayiYukle();
                        });

                        thread.IsBackground = true;

                        thread.Name = "thread" + i.ToString();

                        Threads.Add(i, thread);

                        thread.Start();

                    }


                    FrmLog.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Devam eden başvuru işlemi bulunduğundan yeni bir işlem başlatılamıyor", "Onay", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BasvuruSayfayiYukle(int sira = -1)
        {
            if (sira == -1)
            {
                sira = Convert.ToInt32(System.Threading.Thread.CurrentThread.Name.Split('-')[0].Replace("thread", ""));
            }

            List<string> tamamlananIsyerleri = new List<string>();

            try
            {
                Parallel.For(baslangic - 1, son, new ParallelOptions { MaxDegreeOfParallelism = 10 }, i =>
                  {
                      if (TesvikBasvuruYapiliyor)
                      {
                          lock (Mesajlar)
                          {
                              Mesajlar.Add(i, new List<string>());
                          }

                          string IsyeriId = isyerleri[i].IsyeriID.ToString();

                          var isyeri = isyerleri[i];

                          if (BakilanIsyerleri.Contains(IsyeriId))
                          {
                              lock (Mesajlar)
                              {
                                  Mesajlar[i].Add("[" + DateTime.Now.ToString() + "] : " + isyeri.Sirketler.SirketAdi + "-" + isyeri.SubeAdi + " daha önceden bakılmış" + Environment.NewLine);
                                  //sb.Append("[" + DateTime.Now.ToString() + "] : " + isyeri.SirketAdi + "-" + isyeri.SubeAdi + " daha önceden bakılmış.(" + (i + 1) + "/" + son + ")" + Environment.NewLine); new delLoglariGuncelle(LoglariGuncelle).Invoke();
                              }
                          }
                          else
                          {

                              if (isyeri.KullaniciKod.Length > 4)
                              {
                                  lock (Mesajlar)
                                  {
                                      Mesajlar[i].Add("[" + DateTime.Now.ToString() + "] : " + isyeri.Sirketler.SirketAdi + "-" + isyeri.SubeAdi + ". Kullanıcı kodu 4 karakterden fazla.Siradaki işyerine geçilecek" + Environment.NewLine);
                                      //sb.Append("[" + DateTime.Now.ToString() + "] : " + isyeri.SirketAdi + "-" + isyeri.SubeAdi + ". Kullanıcı kodu 4 karakterden fazla.Siradaki işyerine geçilecek (" + (i + 1) + "/" + son + ")" + Environment.NewLine); new delLoglariGuncelle(LoglariGuncelle).Invoke();
                                  }

                                  lock (hataliIsyerleri)
                                  {
                                      hataliIsyerleri.Add(isyeri.Sirketler.SirketAdi + "-" + isyeri.SubeAdi + "-" + isyeri.IsyeriSicilNo + " Kullanıcı kodu 4 karakterden fazla");
                                  }
                              }
                              else
                              {
                                  lock (Mesajlar)
                                  {
                                      Mesajlar[i].Add("[" + DateTime.Now.ToString() + "] : " + isyeri.Sirketler.SirketAdi + "-" + isyeri.SubeAdi + " işleme alınıyor." + Environment.NewLine);
                                      //sb.Append("[" + DateTime.Now.ToString() + "] : " + isyeri.SirketAdi + "-" + isyeri.SubeAdi + " işleme alınıyor.(" + (i + 1) + "/" + son + ")" + Environment.NewLine); new delLoglariGuncelle(LoglariGuncelle).Invoke();
                                  }

                                  int basvuruKaydiYapilamadiSayaci = 0;

                              BasvuruKaydiYapilamadiTekrarDene:

                                  Classes.ProjeGiris webClient = new Classes.ProjeGiris(isyeri, Enums.ProjeTurleri.IsverenSistemi);

                                  string girisCevabi = null;

                                  int girisDenemeSayisi = 0;

                                  do
                                  {
                                      girisDenemeSayisi++;

                                      girisCevabi = webClient.Connect();

                                      if (girisCevabi.Equals("Kullanıcı adı veya şifreleriniz hatalıdır")
                                          || girisCevabi.Equals("İşyeri Kanun Kapsamından Çıkmıştır")
                                          || girisCevabi.Equals("Is Yeri Iz Olmus")
                                          || girisCevabi.Equals("işyeri hesabı PASİF olduğu için sisteme giriş yapamadı")
                                          || girisCevabi.Equals("Vekalet Süresi Dolmuştur")
                                          || girisCevabi.Equals("Şifre bilgileri içerisinde Türkçe harf olmamalidir")
                                           )
                                          break;

                                      if (girisCevabi.Equals("Error"))
                                      {
                                          lock (Mesajlar)
                                          {
                                              Mesajlar[i].Add("[" + DateTime.Now.ToString() + "] : " + isyeri.Sirketler.SirketAdi + "-" + isyeri.SubeAdi + " " + girisDenemeSayisi + ". kez giriş yapılamadı." + Environment.NewLine);
                                          }
                                          //sb.Append("[" + DateTime.Now.ToString() + "] : " + isyeri.SirketAdi + "-" + isyeri.SubeAdi + " " + girisDenemeSayisi + ". kez giriş yapılamadı." + Environment.NewLine); new delLoglariGuncelle(LoglariGuncelle).Invoke();

                                          if (girisDenemeSayisi >= 3)
                                          {
                                              break;
                                          }

                                      }
                                  }
                                  while (!girisCevabi.Equals("OK"));

                                  if (girisDenemeSayisi >= 3)
                                  {
                                      lock (Mesajlar)
                                      {
                                          Mesajlar[i].Add("[" + DateTime.Now.ToString() + "] : " + isyeri.Sirketler.SirketAdi + "-" + isyeri.SubeAdi + " " + girisDenemeSayisi + " kez giriş yapılamadı. Sıradaki işyerine geçilecek" + Environment.NewLine);
                                          //sb.Append("[" + DateTime.Now.ToString() + "] : " + isyeri.SirketAdi + "-" + isyeri.SubeAdi + " " + girisDenemeSayisi + " kez giriş yapılamadı. Sıradaki işyerine geçilecek" + Environment.NewLine); new delLoglariGuncelle(LoglariGuncelle).Invoke();
                                      }
                                  }
                                  else
                                  {

                                      if (girisCevabi.Equals("OK"))
                                      {
                                          HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();

                                          int sayac = 0;

                                      YenidenDene:

                                          string response = webClient.Get("https://uyg.sgk.gov.tr/IsverenSistemi/internetLinkTesvik444717Basvuru.action;", "");

                                          if (response.Contains("5510/ EK 17. MADDE BAŞVURU İŞLEMLERİ"))
                                          {
                                              html.LoadHtml(response);

                                              var viewstate = html.GetElementbyId("javax.faces.ViewState");
                                              var basvurubuttonId = html.DocumentNode.Descendants("button").FirstOrDefault(p => p.InnerText != null && p.InnerText.Equals("BAŞVURU")).Id;

                                              var PostData = "javax.faces.partial.ajax=true&javax.faces.source=" + WebUtility.UrlEncode(basvurubuttonId) + "&javax.faces.partial.execute=" + WebUtility.UrlEncode("@all") + "&javax.faces.partial.render=tableForm+formmessage+isyeriForm+basvuruEkleForm&" + WebUtility.UrlEncode(basvurubuttonId) + "=" + WebUtility.UrlEncode(basvurubuttonId) + "&tableForm=tableForm&dataTableBasvurulistesi_selection=&javax.faces.ViewState=" + WebUtility.UrlEncode(viewstate.GetAttributeValue("value", ""));

                                              response = webClient.PostData("https://uyg.sgk.gov.tr/IsverenSistemi/pages/genelParametreler/gecici17Basvuru.jsf", PostData);

                                              if (response.Contains("<span class=\"ui-button-text\">ONAYLA</span>"))
                                              {
                                                  html.LoadHtml(response);

                                                  var onaylaButtonId = html.DocumentNode.Descendants("button").FirstOrDefault(p => p.InnerText != null && p.InnerText.Equals("ONAYLA")).Id;
                                                  var formId = html.GetElementbyId("basvuruDilekcePanel").Descendants("form").FirstOrDefault().Id;

                                                  PostData = "javax.faces.partial.ajax=true&javax.faces.source=" + WebUtility.UrlEncode(onaylaButtonId) + "&javax.faces.partial.execute=%40all&javax.faces.partial.render=tableForm+formmessage+isyeriForm+basvuruEkleForm&" + WebUtility.UrlEncode(onaylaButtonId) + "=" + (onaylaButtonId) + "&basvuruEkleForm=basvuruEkleForm&" + WebUtility.UrlEncode(formId) + "=" + WebUtility.UrlEncode(formId) + "&ibanText=&javax.faces.ViewState=" + WebUtility.UrlEncode(viewstate.GetAttributeValue("value", ""));

                                                  response = webClient.PostData("https://uyg.sgk.gov.tr/IsverenSistemi/pages/genelParametreler/gecici17Basvuru.jsf", PostData);

                                                  if (!response.Equals("Error"))
                                                  {

                                                      if (response.Contains("Başvuru kaydınız oluşturulmuştur"))
                                                      {
                                                          lock (Mesajlar)
                                                          {
                                                              Mesajlar[i].Add("[" + DateTime.Now.ToString() + "] : " + isyeri.Sirketler.SirketAdi + "-" + isyeri.SubeAdi + "-" + isyeri.IsyeriSicilNo + " başarıyla eklendi" + Environment.NewLine);
                                                          }

                                                          basariliIsyerleri.Add(isyeri.Sirketler.SirketAdi + "-" + isyeri.SubeAdi + "-" + isyeri.IsyeriSicilNo);

                                                          BakilanIsyerlerineEkle(isyeri.IsyeriID.ToString());
                                                      }
                                                      else if (response.Contains("Aynı gün içerisinde bir kez başvuru yapabilirsiniz"))
                                                      {
                                                          lock (Mesajlar)
                                                          {
                                                              Mesajlar[i].Add("[" + DateTime.Now.ToString() + "] : " + isyeri.Sirketler.SirketAdi + "-" + isyeri.SubeAdi + "-" + isyeri.IsyeriSicilNo + ". Aynı gün içerisinde bir kez başvuru yapabilirsiniz" + Environment.NewLine);
                                                          }

                                                          BakilanIsyerlerineEkle(isyeri.IsyeriID.ToString());
                                                      }
                                                      else if (response.Contains("Başvuru kaydınız oluşturulamamıştır"))
                                                      {
                                                          basvuruKaydiYapilamadiSayaci++;

                                                          if (basvuruKaydiYapilamadiSayaci < 3)
                                                          {
                                                              lock (Mesajlar)
                                                              {
                                                                  Mesajlar[i].Add("[" + DateTime.Now.ToString() + "] : " + isyeri.Sirketler.SirketAdi + "-" + isyeri.SubeAdi + "-" + isyeri.IsyeriSicilNo + " başvuru kaydı yapılamadı. " + basvuruKaydiYapilamadiSayaci + ".deneme" + Environment.NewLine);
                                                              }

                                                              webClient.Disconnect();

                                                              goto BasvuruKaydiYapilamadiTekrarDene;
                                                          }
                                                          else
                                                          {
                                                              Mesajlar[i].Add("[" + DateTime.Now.ToString() + "] : " + isyeri.Sirketler.SirketAdi + "-" + isyeri.SubeAdi + "-" + isyeri.IsyeriSicilNo + " 3 denemeye rağmen başvuru kaydı yapılamadı uyarısı çıkıyor.Sıradaki işyerine geçilecek " + Environment.NewLine);
                                                          }
                                                      }
                                                  }
                                                  else
                                                  {
                                                      sayac++;

                                                      if (sayac <= 3)
                                                      {
                                                          goto YenidenDene;
                                                      }
                                                      else
                                                      {
                                                          lock (Mesajlar)
                                                          {
                                                              Mesajlar[i].Add("[" + DateTime.Now.ToString() + "] : --> " + isyeri.Sirketler.SirketAdi + "-" + isyeri.SubeAdi + "-" + isyeri.IsyeriSicilNo + " 3 denemeye rağmen işlem yapılamadı." + Environment.NewLine);
                                                              //sb.Append("[" + DateTime.Now.ToString() + "] : --> " + isyeri.SirketAdi + "-" + isyeri.SubeAdi + "-" + isyeri.IsyeriSicilNo + " 3 denemeye rağmen işlem yapılamadı." + Environment.NewLine); new delLoglariGuncelle(LoglariGuncelle).Invoke();
                                                          }

                                                          lock (hataliIsyerleri)
                                                          {
                                                              hataliIsyerleri.Add(isyeri.Sirketler.SirketAdi + "-" + isyeri.SubeAdi + "-" + isyeri.IsyeriSicilNo + "3 denemeye rağmen işlem yapılırken hata meydana geldi");
                                                          }
                                                      }
                                                  }
                                              }
                                              else goto YenidenDene;

                                          }
                                          else goto YenidenDene;

                                          webClient.Disconnect();

                                      }
                                      else if (girisCevabi.Equals("Error"))
                                      {
                                          lock (Mesajlar)
                                          {
                                              Mesajlar[i].Add("[" + DateTime.Now.ToString() + "] : " + isyeri.Sirketler.SirketAdi + "-" + isyeri.SubeAdi + " hatadan dolayı sisteme giriş yapılamadı.Sıradaki işyerine geçilecek." + Environment.NewLine);
                                              //sb.Append("[" + DateTime.Now.ToString() + "] : " + isyeri.SirketAdi + "-" + isyeri.SubeAdi + " hatadan dolayı sisteme giriş yapılamadı.Sıradaki işyerine geçilecek.(" + (i + 1) + "/" + son + ")" + Environment.NewLine); new delLoglariGuncelle(LoglariGuncelle).Invoke();
                                          }
                                      }
                                      else
                                      {
                                          lock (Mesajlar)
                                          {
                                              Mesajlar[i].Add("[" + DateTime.Now.ToString() + "] : " + isyeri.Sirketler.SirketAdi + "-" + isyeri.SubeAdi + " \"" + girisCevabi + "\" hatasından dolayı sisteme giriş yapılamadı.Sıradaki işyerine geçilecek." + Environment.NewLine);
                                              //sb.Append("[" + DateTime.Now.ToString() + "] : " + isyeri.SirketAdi + "-" + isyeri.SubeAdi + " \"" + girisCevabi + "\" hatasından dolayı sisteme giriş yapılamadı.Sıradaki işyerine geçilecek.(" + (i + 1) + "/" + son + ")" + Environment.NewLine); new delLoglariGuncelle(LoglariGuncelle).Invoke();
                                          }

                                          BakilanIsyerlerineEkle(isyeri.IsyeriID.ToString());

                                      }
                                  }
                              }
                          }

                          lock (tamamlananIsyerleri)
                          {
                              tamamlananIsyerleri.Add(isyeri.IsyeriID.ToString());
                          }

                          lock (sb)
                          {
                              sb.Append(Mesajlar[i].Last().Replace(Environment.NewLine, "") + " (" + (tamamlananIsyerleri.Count) + "/" + (son - baslangic + 1) + ")" + Environment.NewLine); new delLoglariGuncelle(LoglariGuncelle).Invoke();
                          }
                      }
                  }
                );


                if (TesvikBasvuruYapiliyor)
                {
                    SonaErdi();
                }
            }
            catch (Exception ex)
            {
                Metodlar.HataMesajiGoster(ex, "Teşvik başvurusunda hata meydana geldi");
            }
        }

        void BakilanIsyerlerineEkle(string isyeriID)
        {
            int i = 0;

            while (i <= 5)
            {
                i++;

                try
                {
                    File.AppendAllText("BakilanIsyerleri.txt", isyeriID + Environment.NewLine);

                    break;
                }
                catch
                {
                    Thread.Sleep(200);
                }
            }
        }

        public static void LogYaz(string log)
        {
            int i = 0;

            while (i <= 5)
            {
                i++;

                try
                {
                    File.AppendAllText(Application.StartupPath + "\\BasvuruYapilanIsyerleri.txt", log);

                    break;
                }
                catch
                {
                    Thread.Sleep(200);
                }
            }

        }

        delegate void delLoglariGuncelle();

        void LoglariGuncelle()
        {
            if (TesvikBasvuruYapiliyor)
            {

                if (FrmLog.lbLog.InvokeRequired)
                {
                    this.Invoke(new delLoglariGuncelle(LoglariGuncelle));
                }
                else
                {
                    if (FrmLog != null)
                    {
                        FrmLog.LoglariGuncelle(sb);
                    }
                }
            }

        }

        void SonaErdi()
        {

            TesvikBasvuruYapiliyor = false;

            sb.Append("[" + DateTime.Now.ToString() + "] : İşyerleri teşvik başvurusu tamamlandı" + Environment.NewLine); new delLoglariGuncelle(LoglariGuncelle).Invoke();

            MessageBox.Show("İşyerleri teşvik başvurusu tamamlandı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (hataliIsyerleri.Count > 0)
            {
                MessageBox.Show(String.Join(Environment.NewLine + Environment.NewLine, hataliIsyerleri), "Hatalı İşyerleri", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            var mesajlar = Mesajlar.SelectMany(p => p.Value);

            if (mesajlar.Count() > 0)
            {
                var log = String.Join("", mesajlar);

                LogYaz(log);
            }
        }

        public void IslemiIptalEt()
        {
            SonaErdi();
        }

        #endregion

        private void btnDonusturulecekKanunlar_Click(object sender, EventArgs e)
        {
            new frmDonusturulecekKanunlar().ShowDialog();
        }

        private void BtnKısaVadeliSigortaPrimKoluOranlari_Click(object sender, EventArgs e)
        {
            //ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook("Ek9Ilk.xlsx");

            //wb.Worksheet(1).Cell("A2").Value = "deneme";

            //wb.SaveAs("deneme.xlsx");

            new frmKisaVadeliSigortaPrimKollari().ShowDialog();
        }

        private void BtnDonemIslemciSayisiKaydet_Click(object sender, EventArgs e)
        {
            int i = 0;

            if (Int32.TryParse(txtDonemIslemciSayisi.Text.Trim(), out i))
            {

                using (var dbContext = new DbEntities())
                {
                    var elem = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("DonemIslemciSayisi"));

                    elem.Deger = txtDonemIslemciSayisi.Text;

                    dbContext.SaveChanges();
                }

                Program.DonemIslemciSayisi = i;

                MessageBox.Show("Kaydedildi", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir sayı giriniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnKisiIslemciSayisiKaydet_Click(object sender, EventArgs e)
        {
            int i = 0;

            if (Int32.TryParse(txtKisiIslemciSayisi.Text.Trim(), out i))
            {
                using (var dbContext = new DbEntities())
                {
                    var elem = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("KisiIslemciSayisi"));

                    elem.Deger = txtKisiIslemciSayisi.Text;

                    dbContext.SaveChanges();
                }

                Program.KisiIslemciSayisi = i;

                MessageBox.Show("Kaydedildi", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir sayı giriniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnXmlDosyasındanAktar_Click(object sender, EventArgs e)
        {

            #region İlk Yöntem

            ofdXml.InitialDirectory = Application.StartupPath;

            if (File.Exists(Path.Combine(Application.StartupPath, "veri.xml")))
            {
                ofdXml.FileName = Path.Combine(Application.StartupPath, "veri.xml");
            }

            if (ofdXml.ShowDialog() == DialogResult.OK)
            {
                var secilidosyaKlasoru = new FileInfo(ofdXml.FileName).Directory.FullName;

                XDocument xml = XDocument.Load(ofdXml.FileName);

                if (xml.Root.Name.LocalName.Equals("TesvikProgrami"))
                {
                    progressBar1.Value = 0;
                    progressBar1.Visible = true;
                    progressBar1.Refresh();

                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

                    stopwatch.Start();

                    var sirketKlasorleri = Directory.GetDirectories(Path.Combine(secilidosyaKlasoru, "dosyalar"));
                    var isyeriKlasorleri = sirketKlasorleri.SelectMany(p => Directory.GetDirectories(p));

                    var yeniDosyalarPath = Path.Combine(Application.StartupPath, "dosyalar");

                    if (Directory.Exists(yeniDosyalarPath)) Directory.Delete(yeniDosyalarPath, true);

                    if (!Directory.Exists(yeniDosyalarPath)) Directory.CreateDirectory(yeniDosyalarPath);

                    var eskisirketler = xml.Root.Element("Sirketler").Elements("Sirket");

                    var eskiIsyerleri = xml.Root.Element("Isyerleri").Elements("Isyeri");

                    var toplamIsyeriSayisi = eskiIsyerleri.Count();

                    int tamamlananToplamIsyeriSayisi = 0;

                    using (var dbContext = new DbEntities())
                    {
                        dbContext.Database.ExecuteSqlCommand("delete from Sirketler; delete from BelgeTurleri; delete from AsgariUcretler; delete from Ayarlar; delete from DonusturulecekKanunlar; delete from KisaVadeliSigortaPrimKoluOranlari; delete from sqlite_sequence;");

                        var belgeTurleri = xml.Root.Element("BelgeTurleri").Elements("BelgeTuru");

                        foreach (var belgeTuru in belgeTurleri)
                        {
                            dbContext.BelgeTurleri.Add(new BelgeTurleri
                            {
                                BelgeTuruID = belgeTuru.Element("BelgeTuruNo").Value.ToLong(),
                                MalulYaslilikOraniSigortali = belgeTuru.Element("MalulYaslilikOraniSigortali").Value.ToDouble(),
                                MalulYaslilikOraniIsveren = belgeTuru.Element("MalulYaslilikOraniIsveren").Value.ToDouble(),
                                GenelSaglikSigortali = belgeTuru.Element("GenelSaglikSigortali").Value.ToDouble(),
                                GenelSaglikIsveren = belgeTuru.Element("GenelSaglikIsveren").Value.ToDouble(),
                                SosyalDestekSigortali = belgeTuru.Element("SosyalDestekSigortali").Value.ToDouble(),
                                SosyalDestekIsveren = belgeTuru.Element("SosyalDestekIsveren").Value.ToDouble(),
                                IssizlikSigortali = belgeTuru.Element("IssizlikSigortali").Value.ToDouble(),
                                IssizlikIsveren = belgeTuru.Element("IssizlikIsveren").Value.ToDouble(),
                            });
                        }

                        dbContext.SaveChanges();

                        progressBar1.Value = 3;

                        var asgariUcretler = xml.Root.Element("AsgariUcretler").Elements("AsgariUcret");

                        foreach (var asgariUcret in asgariUcretler)
                        {
                            dbContext.AsgariUcretler.Add(new AsgariUcretler
                            {
                                Baslangic = asgariUcret.Element("Baslangic").Value,
                                Bitis = asgariUcret.Element("Bitis").Value,
                                AsgariUcretTutari = asgariUcret.Element("AsgariUcretTutari").Value.ToDouble()
                            });
                        }

                        dbContext.SaveChanges();

                        progressBar1.Value = 6;

                        var tesvikler = xml.Root.Element("Ayarlar").Element("DonusturulecekKanunlar").Elements("Tesvik");

                        foreach (var tesvik in tesvikler)
                        {
                            var donusturulecekKanunlar = tesvik.Elements("DonusturulecekKanun");

                            var tesvikKanun = tesvik.Attribute("Kanun").Value;

                            foreach (var dk in donusturulecekKanunlar)
                            {
                                dbContext.DonusturulecekKanunlar.Add(new DonusturulecekKanunlar
                                {
                                    TesvikKanunNo = tesvikKanun,
                                    DonusturulenKanunNo = dk.Attribute("Kanun").Value,
                                    SadeceCari = dk.Attribute("SadeceCari").Value.ToBool() ? 1 : 0
                                });
                            }
                        }

                        dbContext.SaveChanges();

                        progressBar1.Value = 10;

                        var xmlKvsk = XDocument.Load(Path.Combine(secilidosyaKlasoru, "KisaVadeliSigortaPrimKoluOranlari.xml"));

                        var kvsklar = xmlKvsk.Element("KisaVadeliSigortaKollari").Elements("KisaVadeliSigortaKolu");

                        foreach (var kvsk in kvsklar)
                        {
                            dbContext.KisaVadeliSigortaPrimKoluOranlari.Add(new KisaVadeliSigortaPrimKoluOranlari
                            {
                                KisaVadeliSigortaKoluKodu = kvsk.Element("KisaVadeliSigortaKoluKodu").Value,
                                PrimOrani = kvsk.Element("PrimOrani").Value.ToDouble(),
                            });
                        }

                        dbContext.SaveChanges();

                        var ayarlar = xml.Root.Element("Ayarlar");

                        var anahtarlar = new List<string> {
                            "ZamanasimiSuresi",
                            "IsverenSistemiGuvenlikKoduGirisi",
                            "GuvenlikKoduGirisi6645",
                            "GuvenlikKoduGirisi687",
                            "GuvenlikKoduGirisi14857",
                            "EBildirgeV2GuvenlikKoduGirisi",
                            "MinimumGunSayisi",
                            "BildirgeMinimumTutar",
                            "BildirgelerOnaylansin",
                            "AcikKalanExcellerKapansin",
                            "BfIndirmeUcretDestegiIstensin",
                            "BasvuruYoksaTesvikVerilmesin",
                            "SeciliKanunlarDonusturulsun",
                            "DonemIslemcisiYeniGirisYapsin",
                            "KisiIslemcisiYeniGirisYapsin",
                            "DetayliLoglamaYapilsin",
                            "DonemIslemciSayisi",
                            "KisiIslemciSayisi"
                        };

                        anahtarlar.AddRange(Program.TumTesvikler.Select(p => String.Concat("AsgariUcretDestekTutariDikkateAlinsin", p.Key)));

                        foreach (var anahtar in anahtarlar)
                        {

                            XElement elem = null;

                            if (anahtar.StartsWith("AsgariUcretDestekTutariDikkateAlinsin"))
                            {
                                if (ayarlar.Element("AsgariUcretDestekTutariDikkateAlinsin") != null && ayarlar.Element("AsgariUcretDestekTutariDikkateAlinsin").Descendants().Count() > 0)
                                {
                                    elem = ayarlar.Element("AsgariUcretDestekTutariDikkateAlinsin").Descendants().FirstOrDefault(p => p.Value.Equals(anahtar.Replace("AsgariUcretDestekTutariDikkateAlinsin", "")));
                                }
                            }
                            else elem = ayarlar.Element(anahtar);

                            if (elem != null)
                            {
                                dbContext.Ayarlar.Add(new Ayarlar
                                {
                                    Anahtar = anahtar,
                                    Deger = anahtar.StartsWith("AsgariUcretDestekTutariDikkateAlinsin") ? "True" : ayarlar.Element(anahtar).Value
                                });
                            }
                        }


                        dbContext.SaveChanges();

                        progressBar1.Value = 20;

                        Dictionary<long, Sirketler> yeniSirketler = new Dictionary<long, Sirketler>();

                        foreach (var item in eskisirketler)
                        {
                            var EskiSirketID = item.Element("ID").Value.ToLong();

                            var yeniSirket = new Sirketler
                            {
                                SirketAdi = item.Element("SirketAdi").Value,
                                VergiKimlikNo = item.Element("VergiKimlikNo").Value,
                                Aktif = item.Element("Aktif").Value.ToBool() ? 1 : 0
                            };

                            yeniSirketler.Add(EskiSirketID, yeniSirket);
                        }

                        dbContext.Sirketler.AddRange(yeniSirketler.Values);

                        dbContext.SaveChanges();

                        Dictionary<long, Isyerleri> yeniIsyerleri = new Dictionary<long, Isyerleri>();

                        foreach (var eskiIsyeri in eskiIsyerleri)
                        {
                            var EskiSirketID = eskiIsyeri.Element("SirketID").Value.ToLong();
                            var EskiIsyeriID = eskiIsyeri.Element("ID").Value.ToLong();

                            var yeniSirket = yeniSirketler[EskiSirketID];

                            var isyeriEskiPath = isyeriKlasorleri.FirstOrDefault(x => x.EndsWith("-" + eskiIsyeri.Element("ID").Value));

                            bool BasvuruFormuVar = false;
                            bool AphbVar = false;
                            bool BasvuruListesi7166Var = false;

                            if (isyeriEskiPath != null)
                            {
                                var basvuruFormu = eskiIsyeri.Element("BasvuruFormu").Value;
                                var aphb = eskiIsyeri.Element("APHB").Value;
                                var basvuruListesi7166 = eskiIsyeri.Element("BasvuruListesi7166").Value;

                                if (!string.IsNullOrEmpty(basvuruFormu))
                                {
                                    BasvuruFormuVar = File.Exists(Path.Combine(isyeriEskiPath, basvuruFormu));
                                }

                                if (!string.IsNullOrEmpty(aphb))
                                {
                                    AphbVar = File.Exists(Path.Combine(isyeriEskiPath, aphb));
                                }

                                if (!string.IsNullOrEmpty(basvuruListesi7166))
                                {
                                    BasvuruListesi7166Var = File.Exists(Path.Combine(isyeriEskiPath, basvuruListesi7166));
                                }

                            }

                            var taseronXElement = eskiIsyeri.Element("TaseronNo");

                            var TaseronNo = (taseronXElement == null ? "000" : string.IsNullOrEmpty(taseronXElement.Value) ? "000" : taseronXElement.Value).ToInt().ToString().PadLeft(3, '0');

                            var isverenXElement = eskiIsyeri.Element("Isveren");
                            var altIsverenXElement = eskiIsyeri.Element("AltIsveren");

                            var yeniIsyeri = new Isyerleri
                            {
                                SirketID = yeniSirket.SirketID,
                                SubeAdi = eskiIsyeri.Element("SubeAdi").Value,
                                Aktif = eskiIsyeri.Element("Aktif") == null ? 1 : eskiIsyeri.Element("Aktif").Value.ToBool() ? 1 : 0,
                                IsyeriSicilNo = eskiIsyeri.Element("IsyeriSicilNo").Value,
                                TaseronNo = TaseronNo,
                                SosyalGuvenlikKurumu = eskiIsyeri.Element("SosyalGuvenlikKurumu").Value,
                                KullaniciAdi = eskiIsyeri.Element("KullaniciAdi").Value,
                                KullaniciKod = eskiIsyeri.Element("KullaniciKod").Value,
                                SistemSifresi = eskiIsyeri.Element("SistemSifresi").Value,
                                IsyeriSifresi = eskiIsyeri.Element("IsyeriSifresi").Value,
                                BasvuruFormu = BasvuruFormuVar ? eskiIsyeri.Element("BasvuruFormu").Value : null,
                                Aphb = AphbVar ? eskiIsyeri.Element("APHB").Value : null,
                                BasvuruListesi7166 = BasvuruListesi7166Var ? eskiIsyeri.Element("BasvuruListesi7166").Value : null,
                                IsverenAdSoyad = isverenXElement.Element("AdSoyad").Value,
                                IsverenUnvan = isverenXElement.Element("Unvan").Value,
                                IsverenAdres = isverenXElement.Element("Adres").Value,
                                IsverenSemt = isverenXElement.Element("Semt").Value,
                                IsverenIlce = isverenXElement.Element("Ilce").Value,
                                IsverenIl = isverenXElement.Element("Il").Value,
                                IsverenDisKapiNo = isverenXElement.Element("DisKapiNo").Value,
                                IsverenIcKapiNo = isverenXElement.Element("IcKapiNo").Value,
                                IsverenPostaKodu = isverenXElement.Element("PostaKodu").Value,
                                IsverenTelefon = isverenXElement.Element("Telefon").Value,
                                IsverenEposta = isverenXElement.Element("Eposta").Value,
                                AltIsverenTcKimlikNo = altIsverenXElement.Element("TcKimlikNo").Value,
                                AltIsverenAdSoyad = altIsverenXElement.Element("AdSoyad").Value,
                                AltIsverenUnvan = altIsverenXElement.Element("Unvan").Value,
                                AltIsverenAdres = altIsverenXElement.Element("Adres").Value,
                                AltIsverenSemt = altIsverenXElement.Element("Semt").Value,
                                AltIsverenIlce = altIsverenXElement.Element("Ilce").Value,
                                AltIsverenIl = altIsverenXElement.Element("Il").Value,
                                AltIsverenDisKapiNo = altIsverenXElement.Element("DisKapiNo").Value,
                                AltIsverenIcKapiNo = altIsverenXElement.Element("IcKapiNo").Value,
                                AltIsverenPostaKodu = altIsverenXElement.Element("PostaKodu").Value,
                                AltIsverenTelefon = altIsverenXElement.Element("Telefon").Value,
                                AltIsverenEposta = altIsverenXElement.Element("Eposta").Value,
                            };

                            yeniIsyerleri.Add(EskiIsyeriID, yeniIsyeri);
                        }

                        dbContext.Isyerleri.AddRange(yeniIsyerleri.Values);

                        dbContext.SaveChanges();

                        List<AsgariUcretDestekTutarlari> asgariUcretDestekTutarlariList = new List<AsgariUcretDestekTutarlari>();
                        List<AylikCalisanSayilari> aylikCalisanSayilariList = new List<AylikCalisanSayilari>();
                        List<BorcluAylar> borcluAylarList = new List<BorcluAylar>();
                        List<BasvuruDonemleri> basvuruDonemleriList = new List<BasvuruDonemleri>();


                        foreach (var eskiIsyeri in eskiIsyerleri)
                        {
                            var EskiSirketID = eskiIsyeri.Element("SirketID").Value.ToLong();

                            var EskiIsyeriID = eskiIsyeri.Element("ID").Value.ToLong();

                            var yeniSirket = yeniSirketler[EskiSirketID];

                            var yeniSirketPath = yeniSirket.SirketAdi.Replace("/", "").Replace("\\", "") + "-" + yeniSirket.SirketID;

                            var isyeriEskiPath = isyeriKlasorleri.FirstOrDefault(x => x.EndsWith("-" + eskiIsyeri.Element("ID").Value));

                            var yeniIsyeri = yeniIsyerleri[EskiIsyeriID];

                            if (isyeriEskiPath != null)
                            {
                                var isyeriDosyalari = Directory.GetFiles(isyeriEskiPath);

                                if (isyeriDosyalari.Count() > 0)
                                {
                                    if (!Directory.Exists(Path.Combine(yeniDosyalarPath, yeniSirketPath))) Directory.CreateDirectory(Path.Combine(yeniDosyalarPath, yeniSirketPath));

                                    var yeniIsyeriPath = Path.Combine(yeniDosyalarPath, yeniSirketPath, yeniIsyeri.SubeAdi.Replace("\\", "").Replace("/", "") + "-" + yeniIsyeri.IsyeriID);

                                    if (!Directory.Exists(yeniIsyeriPath)) Directory.CreateDirectory(yeniIsyeriPath);

                                    foreach (var eskiDosya in isyeriDosyalari)
                                    {
                                        try
                                        {
                                            File.Copy(eskiDosya, Path.Combine(yeniIsyeriPath, Path.GetFileName(eskiDosya)), true);
                                        }
                                        catch { }
                                    }
                                }
                            }



                            if (eskiIsyeri.Element("AsgariUcretDestekTutarlari") != null)
                            {
                                var asgariUcretDestekTutarlari = eskiIsyeri.Element("AsgariUcretDestekTutarlari").Elements("AsgariUcretDestekTutari");

                                foreach (var asgariUcretDestekTutari in asgariUcretDestekTutarlari)
                                {
                                    asgariUcretDestekTutarlariList.Add(new AsgariUcretDestekTutarlari
                                    {
                                        IsyeriID = yeniIsyeri.IsyeriID,
                                        DonemYil = asgariUcretDestekTutari.Element("DonemYil").Value.ToLong(),
                                        DonemAy = asgariUcretDestekTutari.Element("DonemAy").Value.ToLong(),
                                        HesaplananGun = asgariUcretDestekTutari.Element("HesaplananGun").Value.Replace(",", "").Replace(".", "").ToLong(),
                                    });
                                }
                            }

                            if (eskiIsyeri.Element("AylikCalisanSayilari") != null)
                            {

                                var aylikCalisanSayilari = eskiIsyeri.Element("AylikCalisanSayilari").Elements("AylikCalisanSayisi");

                                foreach (var aylikCalisanSayisi in aylikCalisanSayilari)
                                {
                                    aylikCalisanSayilariList.Add(new AylikCalisanSayilari
                                    {
                                        IsyeriID = yeniIsyeri.IsyeriID,
                                        DonemYil = aylikCalisanSayisi.Element("DonemYil").Value.ToLong(),
                                        DonemAy = aylikCalisanSayisi.Element("DonemAy").Value.ToLong(),
                                        CalisanSayisiTaseronlu = aylikCalisanSayisi.Element("CalisanSayisi").Value.Replace(",", "").Replace(".", "").ToLong(),
                                        CalisanSayisiTaseronsuz = aylikCalisanSayisi.Element("CalisanSayisiTaseronsuz") == null ? -1 : aylikCalisanSayisi.Element("CalisanSayisiTaseronsuz").Value.Replace(",", "").Replace(".", "").ToLong(),
                                    });
                                }
                            }

                            if (eskiIsyeri.Element("BorcluAyBilgileri") != null)
                            {

                                var borcluAyBilgileri = eskiIsyeri.Element("BorcluAyBilgileri").Elements("BorcluAy");

                                foreach (var borcluayBilgisi in borcluAyBilgileri)
                                {
                                    borcluAylarList.Add(new BorcluAylar
                                    {
                                        IsyeriID = yeniIsyeri.IsyeriID,
                                        BorcluAy = borcluayBilgisi.Value
                                    });
                                }
                            }


                            if (eskiIsyeri.Element("BasvuruDonemleri") != null)
                            {
                                var basvuruDonemleri = eskiIsyeri.Element("BasvuruDonemleri").Elements("BasvuruDonem");

                                foreach (var basvuruDonem in basvuruDonemleri)
                                {
                                    basvuruDonemleriList.Add(new BasvuruDonemleri
                                    {
                                        IsyeriID = yeniIsyeri.IsyeriID,
                                        BasvuruDonem = basvuruDonem.Value,
                                        Aylar = basvuruDonem.Attribute("aylar") == null ? null : basvuruDonem.Attribute("aylar").Value
                                    });
                                }
                            }

                            tamamlananToplamIsyeriSayisi++;

                            var progress = (int)(((double)tamamlananToplamIsyeriSayisi / toplamIsyeriSayisi) * 100 * 0.7) + 20;

                            if (progress > progressBar1.Value)
                            {
                                progressBar1.Value = progress;

                                progressBar1.Refresh();
                            }


                        }

                        progressBar1.Value = 90;


                        dbContext.AsgariUcretDestekTutarlari.AddRange(asgariUcretDestekTutarlariList);
                        dbContext.AylikCalisanSayilari.AddRange(aylikCalisanSayilariList);
                        dbContext.BorcluAylar.AddRange(borcluAylarList);
                        dbContext.BasvuruDonemleri.AddRange(basvuruDonemleriList);

                        dbContext.SaveChanges();


                        progressBar1.Value = 95;


                        //var eskidosyalarPath = Path.Combine(secilidosyaKlasoru, "eskidosyalar");

                        //var dosyalarPath = Path.Combine(secilidosyaKlasoru, "dosyalar");

                        //var yeniDosyalarYolu = Path.Combine(Application.StartupPath, "yenidosyalar");

                        //if (Directory.Exists(eskidosyalarPath)) Directory.Delete(eskidosyalarPath, true);

                        //if (Directory.Exists(dosyalarPath)) Directory.Move(dosyalarPath, eskidosyalarPath);

                        //if (Directory.Exists(yeniDosyalarYolu)) Directory.Move(yeniDosyalarYolu, Path.Combine(Application.StartupPath, "dosyalar"));


                        progressBar1.Value = 100;


                        stopwatch.Stop();

                        MessageBox.Show("Xml dosyasından veri aktarımı tamamlandı.", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }

                }
                else MessageBox.Show("Yanlış bir xml dosyası seçtiniz. Lütfen dosya içeriği TesvikProgrami ile başlayan bir veri dosyası seçiniz");

            }

            #endregion

        }

        private void BtnBakiyeSorgula_Click(object sender, EventArgs e)
        {
            Classes.TwoCaptcha twoCaptcha = new Classes.TwoCaptcha();

            var bakiye = twoCaptcha.BakiyeSorgula();

            if (bakiye == "-1")
            {
                MessageBox.Show("Bakiye sorgulanırken hata meydana geldi. Lütfen daha sonra tekrar deneyiniz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(String.Format("Kalan Bakiye : {0} $", bakiye), "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


        }

        private void cmbEgitimBelgeTurleri_SelectionChangeCommitted(object sender, EventArgs e)
        {
            using (var dbContext = new DbEntities())
            {
                var elem = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfEgitimBelgesi"));
                elem.Deger = cmbEgitimBelgeTurleri.SelectedValue.ToString();
                dbContext.SaveChanges();
            }

            Program.BfEgitimBelgesi = Convert.ToInt32(cmbEgitimBelgeTurleri.SelectedValue);

            MessageBox.Show("Kaydedildi", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAphbSil_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Aphb dosyalarını (Cari aphbler dahil) silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                var sirketklasorleri = Directory.GetDirectories(Path.Combine(Application.StartupPath, "dosyalar"));

                var isyerleriIDleri = sirketklasorleri.SelectMany(d => Directory.GetDirectories(d).Select(a => a.Split('-')[a.Split('-').Length - 1])).ToList();

                var isyerleriklasorleri = sirketklasorleri.SelectMany(d => Directory.GetDirectories(d).Select(a => a)).ToList();

                List<string> hatalar = new List<string>();

                int toplamSilinenSayisi = 0;


                using (var dbContext = new DbEntities())
                {
                    var isyerleri = dbContext.Isyerleri.Include(p => p.Sirketler).ToList();

                    progressBar1.Visible = isyerleri.Count > 0;

                    for (int i = 0; i < isyerleri.Count; i++)
                    {
                        var isyeri = isyerleri[i];

                        if (isyerleriIDleri.Contains(isyeri.IsyeriID.ToString()))
                        {
                            var aphbyol = Metodlar.FormBul(isyeri, Enums.FormTuru.Aphb);

                            if (!String.IsNullOrEmpty(aphbyol))
                            {
                                try
                                {
                                    File.Delete(aphbyol);

                                    toplamSilinenSayisi++;

                                    isyeri.Aphb = null;

                                }
                                catch (Exception)
                                {
                                    hatalar.Add(aphbyol);
                                }
                            }
                            else
                            {

                                isyeri.Aphb = null;
                            }

                            string isyeripath = isyerleriklasorleri.FirstOrDefault(p => p.EndsWith("-" + isyeri.IsyeriID));

                            if (!string.IsNullOrEmpty(isyeripath))
                            {
                                var files = Directory.GetFiles(isyeripath);

                                foreach (var item in files)
                                {
                                    if (item.ToLower().Contains("aphb") && !item.ToLower().Contains("icmal"))
                                    {
                                        try
                                        {
                                            File.Delete(item);

                                            toplamSilinenSayisi++;
                                        }
                                        catch
                                        {
                                            if (!hatalar.Contains(item)) hatalar.Add(item);
                                        }
                                    }
                                }
                            }
                        }
                        else isyeri.Aphb = null;

                        progressBar1.Value = (int)Math.Round(((double)i / isyerleri.Count) * 100);

                    }

                    var digerAphbler = Directory.GetFiles(Path.Combine(Application.StartupPath, "dosyalar"), "*aphb*", SearchOption.AllDirectories);

                    foreach (var item in digerAphbler)
                    {
                        if (item.ToLower().Contains("aphb") && !item.ToLower().Contains("icmal"))
                        {
                            try
                            {
                                File.Delete(item);

                                toplamSilinenSayisi++;
                            }
                            catch
                            {
                                if (!hatalar.Contains(item)) hatalar.Add(item);
                            }
                        }
                    }

                    dbContext.SaveChanges();
                }

                string Mesaj = "Toplam Silinen Dosya Sayısı: " + toplamSilinenSayisi + Environment.NewLine + Environment.NewLine;

                if (hatalar.Count > 0)
                {
                    Mesaj += "Aşağıdaki dosyalar silinemedi.Dosyalar kullanımda olabilir." + Environment.NewLine + string.Join(Environment.NewLine, hatalar);

                }

                MessageBox.Show(Mesaj, "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);

                progressBar1.Visible = false;
            }
        }
    }



}
