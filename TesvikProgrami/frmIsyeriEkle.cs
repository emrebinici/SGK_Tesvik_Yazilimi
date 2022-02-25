using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data.Entity;

namespace TesvikProgrami
{
    public partial class frmIsyeriEkle : Form
    {
        public long eskielementID = 0;

        string AktifPanel = null;

        DialogResult dr = DialogResult.Cancel;

        DialogResult drdialogisyeri = DialogResult.None;

        DialogResult drdialogsifre = DialogResult.None;

        bool IsyeriDialogGosterilecek = false;

        bool SifreDialogGosterilecek = false;

        bool SifreBilgileriKopyalandi = false;

        bool SubeAdiKopyalandi = false;

        List<Sirketler> sirketler = null;

        bool KayitEkleme = true;

        public frmIsyeriEkle()
        {
            InitializeComponent();
        }

        private void frmIsyeriEkle_Load(object sender, EventArgs e)
        {
            SirketleriDoldur();

            pnlIsveren.Visible = false;

            pnlIsveren.Dock = DockStyle.Fill;

            pnlAltIsveren.Visible = false;

            pnlAltIsveren.Dock = DockStyle.Fill;

            pnlBorcluAyBilgileri.Visible = false;

            pnlBorcluAyBilgileri.Dock = DockStyle.Fill;

            pnlIsyeriBilgileri.Dock = DockStyle.Fill;

            AktifPanel = pnlIsyeriBilgileri.Name;

            DateTime tarih = new DateTime(2011, 1, 1);
            DateTime son = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            int i = 1;

            int currentYear = 0;

            int rowCount = 0;

            while (son >= tarih)
            {
                if (son.Year != currentYear)
                {
                    currentYear = son.Year;

                    Label lblyil = new Label();
                    lblyil.Name = "lblyil" + currentYear;
                    lblyil.Font = new Font(lblyil.Font.FontFamily, 12, FontStyle.Bold);
                    lblyil.ForeColor = Color.White;
                    lblyil.Text = currentYear.ToString();
                    lblyil.AutoSize = false;
                    lblyil.Anchor = AnchorStyles.None;
                    lblyil.TextAlign = ContentAlignment.MiddleLeft;

                    rowCount++;

                    tlpBorcluAylar.Controls.Add(lblyil, 0, rowCount - 1);


                }

                CheckBox chk = new CheckBox();
                chk.Name = "chkBorcluAy" + i;
                Font f = new Font(chk.Font.FontFamily, 12, FontStyle.Bold);
                chk.Font = f;
                chk.ForeColor = Color.White;
                chk.Text = son.Month.ToString();
                chk.Anchor = AnchorStyles.None;
                chk.Top = 10;
                chk.Tag = son;
                tlpBorcluAylar.Controls.Add(chk, son.Month, rowCount - 1);

                i++;

                son = son.AddMonths(-1);
            }

            IsyeriBilgileriDoldur();

            KayitEkleme = eskielementID == 0;
        }

        private void Kaydet()
        {
            var isyeriSicilNo = txtIsyeriSicilNo.Text.Trim();
            var taseronNo = txtTaseron.Text.Trim();
            var subeAdi = txtSubeAdi.Text.Trim();
            var sirketID = Convert.ToInt64(cmbSirket.SelectedValue);

            Isyerleri isyeri = null;

            using (var dbContext = new DbEntities())
            {
                if (eskielementID > 0) isyeri = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(eskielementID)).Include(p => p.Sirketler).FirstOrDefault();

                bool yeniEklenecek = isyeri == null;

                if (dbContext.Isyerleri.Where(p => ((p.IsyeriSicilNo.Equals(isyeriSicilNo) && p.TaseronNo.Equals(taseronNo)) || (p.SirketID.Equals(sirketID) && p.SubeAdi.Equals(subeAdi))) && !p.IsyeriID.Equals(eskielementID)).FirstOrDefault() == null)
                {
                    if (sirketID.Equals(-1))
                    {
                        var yeniSirket = (Sirketler)cmbSirket.SelectedItem;

                        var eskiSirket = dbContext.Sirketler.FirstOrDefault(p => p.SirketAdi.Equals(yeniSirket.SirketAdi) || p.VergiKimlikNo.Equals(yeniSirket.VergiKimlikNo));

                        if (eskiSirket != null)
                        {
                            eskiSirket.SirketAdi = yeniSirket.SirketAdi;
                            eskiSirket.VergiKimlikNo = yeniSirket.VergiKimlikNo;
                            sirketID = eskiSirket.SirketID;
                        }
                        else
                        {
                            var sirket = new Sirketler();
                            sirket.SirketAdi = yeniSirket.SirketAdi;
                            sirket.VergiKimlikNo = yeniSirket.VergiKimlikNo;
                            sirket.Aktif = 1;

                            dbContext.Sirketler.Add(sirket);

                            dbContext.SaveChanges();

                            sirketID = sirket.SirketID;
                        }
                    }

                    if (sirketID > -1)
                    {
                        isyeri = isyeri ?? new Isyerleri();

                        isyeri.SirketID = sirketID;
                        isyeri.SubeAdi = subeAdi;
                        isyeri.IsyeriSicilNo = isyeriSicilNo;
                        isyeri.TaseronNo = taseronNo;
                        isyeri.Aktif = Convert.ToDecimal(chkAktif.Checked);
                        isyeri.SosyalGuvenlikKurumu = txtSosyalGuvenlikKurumu.Text.Trim();
                        isyeri.KullaniciAdi = txtKullaniciAdi.Text;
                        isyeri.KullaniciKod = txtKullaniciKod.Text;
                        isyeri.SistemSifresi = txtSistemSifresi.Text;
                        isyeri.IsyeriSifresi = txtIsyeriSifresi.Text;

                        string eskiBasvuruFormu = isyeri.BasvuruFormu;
                        string eskiBasvuruFormuYol = null;

                        if (!string.IsNullOrEmpty(eskiBasvuruFormu))
                        {
                            eskiBasvuruFormuYol = Metodlar.FormBul(isyeri, Enums.FormTuru.BasvuruFormu);
                        }

                        if (txtBasvuruFormu.Text != "")
                        {
                            if (File.Exists(ofdBasvuruFormu.FileName))
                            {
                                isyeri.BasvuruFormu = Path.GetFileName(ofdBasvuruFormu.FileName.Trim());
                            }
                        }
                        else isyeri.BasvuruFormu = null;

                        string eskiAphb = isyeri.Aphb;
                        string eskiAphbyol = null;

                        if (!string.IsNullOrEmpty(eskiAphb))
                        {
                            eskiAphbyol = Metodlar.FormBul(isyeri, Enums.FormTuru.Aphb);
                        }

                        if (txtAPHB.Text != "")
                        {
                            if (File.Exists(ofdAPHB.FileName))
                            {
                                isyeri.Aphb = Path.GetFileName(ofdAPHB.FileName.Trim());
                            }
                        }
                        else isyeri.Aphb = null;

                        string eskiBasvuruListesi7166 = isyeri.BasvuruListesi7166;
                        string eskiBasvuruListesi7166yol = null;

                        if (!string.IsNullOrEmpty(eskiBasvuruListesi7166))
                        {
                            eskiBasvuruListesi7166yol = Metodlar.FormBul(isyeri, Enums.FormTuru.BasvuruListesi7166);
                        }

                        if (txt7166Listesi.Text != "")
                        {
                            if (File.Exists(ofdBasvuruListesi7166.FileName))
                            {
                                isyeri.BasvuruListesi7166 = Path.GetFileName(ofdBasvuruListesi7166.FileName.Trim());
                            }
                        }
                        else isyeri.BasvuruListesi7166 = null;

                        isyeri.IsverenAdSoyad = txtIsverenAdSoyad.Text.Trim();
                        isyeri.IsverenUnvan = txtIsverenUnvan.Text.Trim();
                        isyeri.IsverenSemt = txtIsverenSemt.Text.Trim();
                        isyeri.IsverenIlce = txtIsverenIlce.Text.Trim();
                        isyeri.IsverenIl = txtIsverenIl.Text.Trim();
                        isyeri.IsverenDisKapiNo = txtIsverenDisKapiNo.Text.Trim();
                        isyeri.IsverenIcKapiNo = txtIsverenIcKapiNo.Text.Trim();
                        isyeri.IsverenPostaKodu = txtIsverenPostaKodu.Text.Trim();
                        isyeri.IsverenAdres = txtIsverenAdres.Text.Trim();
                        isyeri.IsverenTelefon = txtIsverenTelefon.Text.Trim();
                        isyeri.IsverenEposta = txtIsverenEposta.Text.Trim();

                        isyeri.AltIsverenAdSoyad = txtAltIsverenAdSoyad.Text.Trim();
                        isyeri.AltIsverenUnvan = txtAltIsverenUnvan.Text.Trim();
                        isyeri.AltIsverenSemt = txtAltIsverenSemt.Text.Trim();
                        isyeri.AltIsverenIlce = txtAltIsverenIlce.Text.Trim();
                        isyeri.AltIsverenIl = txtAltIsverenIl.Text.Trim();
                        isyeri.AltIsverenDisKapiNo = txtAltIsverenDisKapiNo.Text.Trim();
                        isyeri.AltIsverenIcKapiNo = txtAltIsverenIcKapiNo.Text.Trim();
                        isyeri.AltIsverenPostaKodu = txtAltIsverenPostaKodu.Text.Trim();
                        isyeri.AltIsverenTcKimlikNo = txtAltIsverenTcKimlikNo.Text.Trim();
                        isyeri.AltIsverenAdres = txtAltIsverenAdres.Text.Trim();
                        isyeri.AltIsverenTelefon = txtAltIsverenTelefon.Text.Trim();
                        isyeri.AltIsverenEposta = txtAltIsverenEposta.Text.Trim();

                        if (isyeri.IsyeriID > 0)
                        {
                            dbContext.BorcluAylar.RemoveRange(dbContext.BorcluAylar.Where(p => p.IsyeriID.Equals(isyeri.IsyeriID)));
                        }

                        if (yeniEklenecek) dbContext.Isyerleri.Add(isyeri);

                        dbContext.SaveChanges();

                        isyeri = dbContext.Isyerleri.Include(p => p.Sirketler).Where(p => p.IsyeriID.Equals(isyeri.IsyeriID)).FirstOrDefault();

                        var isyeriID = isyeri.IsyeriID;

                        var chkBorcluAylar = tlpBorcluAylar.Controls.Cast<Control>().Where(c => c is CheckBox && ((CheckBox)c).Checked);

                        foreach (var chk in chkBorcluAylar)
                        {
                            var borcluay = new BorcluAylar();
                            var ay = (DateTime)((CheckBox)chk).Tag;
                            borcluay.BorcluAy = ay.Year.ToString() + "/" + ay.Month.ToString();
                            borcluay.IsyeriID = isyeriID;

                            dbContext.BorcluAylar.Add(borcluay);
                        }

                        bool BasvuruBasarili = true;

                        if (txtBasvuruFormu.Text != "")
                        {
                            if (File.Exists(ofdBasvuruFormu.FileName))
                            {
                                if (Metodlar.FormKaydet(isyeri, ofdBasvuruFormu.FileName, Enums.FormTuru.BasvuruFormu) == null)
                                {
                                    if (!string.IsNullOrEmpty(eskiBasvuruFormu))
                                    {
                                        isyeri.BasvuruFormu = eskiBasvuruFormu;
                                    }
                                    else isyeri.BasvuruFormu = null;

                                    BasvuruBasarili = false;
                                }
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(eskiBasvuruFormuYol))
                            {
                                try
                                {
                                    File.Delete(eskiBasvuruFormuYol);
                                }
                                catch { }
                            }
                        }


                        bool APHBbasarili = true;

                        if (txtAPHB.Text != "")
                        {
                            if (File.Exists(ofdAPHB.FileName))
                            {
                                if (Metodlar.FormKaydet(isyeri, ofdAPHB.FileName, Enums.FormTuru.Aphb) == null)
                                {
                                    if (!string.IsNullOrEmpty(eskiAphb))
                                    {
                                        isyeri.Aphb = eskiAphb;

                                    }
                                    else isyeri.Aphb = null;

                                    APHBbasarili = false;
                                }
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(eskiAphbyol))
                            {
                                try
                                {
                                    File.Delete(eskiAphbyol);
                                }
                                catch { }
                            }
                        }

                        bool basvuruListesi7166Basarili = true;

                        if (txt7166Listesi.Text != "")
                        {

                            if (File.Exists(ofdBasvuruListesi7166.FileName))
                            {
                                if (Metodlar.FormKaydet(isyeri, ofdBasvuruListesi7166.FileName, Enums.FormTuru.BasvuruListesi7166) == null)
                                {
                                    if (!string.IsNullOrEmpty(eskiBasvuruListesi7166))
                                    {
                                        isyeri.BasvuruListesi7166 = eskiBasvuruListesi7166;
                                    }
                                    else isyeri.BasvuruListesi7166 = null;

                                    basvuruListesi7166Basarili = false;
                                }
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(eskiBasvuruListesi7166yol))
                            {
                                try
                                {
                                    File.Delete(eskiBasvuruListesi7166yol);
                                }
                                catch { }
                            }
                        }

                        dbContext.SaveChanges();

                        if (eskielementID > 0)
                        {
                            Metodlar.IsyeriPathUpdate(isyeri);
                        }

                        dr = DialogResult.OK;

                        if (BasvuruBasarili && APHBbasarili && basvuruListesi7166Basarili)
                        {
                            MessageBox.Show("Kayıt başarılı");
                        }
                        else
                        {
                            string mesajIcerik = string.Empty;

                            if (!BasvuruBasarili) mesajIcerik += "Başvuru Formu ,";
                            if (!APHBbasarili) mesajIcerik += "Aphb ,";
                            if (!basvuruListesi7166Basarili) mesajIcerik += "7166 Başvuru Listesi ,";

                            MessageBox.Show(String.Format("İşyeri kaydedildi fakat {0} kaydedilemedi. Yüklemeye çalıştığınız dosyalar kullanımda ise lütfen kapattıktan sonra tekrar deneyiniz", mesajIcerik.Trim(',')), "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                else MessageBox.Show("Aynı işyeri daha önce eklenmiştir", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void IsyeriBilgileriDoldur()
        {
            if (eskielementID > 0)
            {

                using (var dbContext = new DbEntities())
                {
                    var isyeri = dbContext.Isyerleri
                        .Where(p => p.IsyeriID.Equals(eskielementID))
                        .Include(p => p.Sirketler)
                        .Include(p => p.BorcluAylar)
                        .FirstOrDefault();

                    if (isyeri != null)
                    {
                        cmbSirket.SelectedValue = isyeri.SirketID;
                        txtSubeAdi.Text = isyeri.SubeAdi;
                        chkAktif.Checked = Convert.ToBoolean(isyeri.Aktif);
                        txtIsyeriSicilNo.Text = isyeri.IsyeriSicilNo;
                        txtTaseron.Text = isyeri.TaseronNo;
                        txtSosyalGuvenlikKurumu.Text = isyeri.SosyalGuvenlikKurumu;
                        txtKullaniciAdi.Text = isyeri.KullaniciAdi;
                        txtKullaniciKod.Text = isyeri.KullaniciKod;
                        txtSistemSifresi.Text = isyeri.SistemSifresi;
                        txtIsyeriSifresi.Text = isyeri.IsyeriSifresi;

                        if (Metodlar.FormBul(isyeri, Enums.FormTuru.BasvuruFormu) == null) txtBasvuruFormu.Text = "";
                        else txtBasvuruFormu.Text = isyeri.BasvuruFormu;
                        lnklblBasvuru.Visible = !string.IsNullOrEmpty(txtBasvuruFormu.Text);

                        if (Metodlar.FormBul(isyeri, Enums.FormTuru.Aphb) == null) txtAPHB.Text = "";
                        else txtAPHB.Text = isyeri.Aphb;
                        lnklblAphb.Visible = !string.IsNullOrEmpty(txtAPHB.Text);

                        if (Metodlar.FormBul(isyeri, Enums.FormTuru.BasvuruListesi7166) == null) txt7166Listesi.Text = "";
                        else txt7166Listesi.Text = isyeri.BasvuruListesi7166;
                        lnklbl7166.Visible = !string.IsNullOrEmpty(txt7166Listesi.Text);

                        txtIsverenAdSoyad.Text = isyeri.IsverenAdSoyad;
                        txtIsverenUnvan.Text = isyeri.IsverenUnvan;
                        txtIsverenSemt.Text = isyeri.IsverenSemt;
                        txtIsverenIlce.Text = isyeri.IsverenIlce;
                        txtIsverenIl.Text = isyeri.IsverenIl;
                        txtIsverenDisKapiNo.Text = isyeri.IsverenDisKapiNo;
                        txtIsverenIcKapiNo.Text = isyeri.IsverenIcKapiNo;
                        txtIsverenPostaKodu.Text = isyeri.IsverenPostaKodu;
                        txtIsverenTelefon.Text = isyeri.IsverenTelefon;
                        txtIsverenAdres.Text = isyeri.IsverenAdres;
                        txtIsverenEposta.Text = isyeri.IsverenEposta;

                        txtAltIsverenAdSoyad.Text = isyeri.AltIsverenAdSoyad;
                        txtAltIsverenUnvan.Text = isyeri.AltIsverenUnvan;
                        txtAltIsverenSemt.Text = isyeri.AltIsverenSemt;
                        txtAltIsverenIlce.Text = isyeri.AltIsverenIlce;
                        txtAltIsverenIl.Text = isyeri.AltIsverenIl;
                        txtAltIsverenDisKapiNo.Text = isyeri.AltIsverenDisKapiNo;
                        txtAltIsverenIcKapiNo.Text = isyeri.AltIsverenIcKapiNo;
                        txtAltIsverenPostaKodu.Text = isyeri.AltIsverenPostaKodu;
                        txtAltIsverenTelefon.Text = isyeri.AltIsverenTelefon;
                        txtAltIsverenAdres.Text = isyeri.AltIsverenAdres;
                        txtAltIsverenEposta.Text = isyeri.AltIsverenEposta;
                        txtAltIsverenTcKimlikNo.Text = isyeri.AltIsverenTcKimlikNo;

                        var chkBorcluAylar = tlpBorcluAylar.Controls.Cast<Control>();

                        foreach (var item in isyeri.BorcluAylar)
                        {
                            var ay = Convert.ToDateTime(item.BorcluAy);

                            var chkBorcluAy = chkBorcluAylar.FirstOrDefault(p => p.Tag != null && ((DateTime)p.Tag).Equals(ay));

                            if (chkBorcluAy != null)
                            {
                                (chkBorcluAy as CheckBox).Checked = true;
                            }
                        }
                    }
                    else
                    {
                        eskielementID = 0;

                        MessageBox.Show("Görüntülemeye çalıştığını< işyeri daha önceden silindiği için işyeri bilgileri çekilemedi", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnIleri_Click(object sender, EventArgs e)
        {
            bool Geri = true;

            if (AktifPanel == "pnlIsyeriBilgileri")
            {
                if (txtIsyeriSicilNo.Text.Trim() != "" && txtSubeAdi.Text.Trim() != "")
                {

                    pnlIsyeriBilgileri.Visible = false;

                    pnlIsveren.Visible = true;

                    btnIsyeriBilgileriniAl.Visible = false;

                    AktifPanel = "pnlIsveren";
                }
                else
                {
                    MessageBox.Show("Zorunlu alanlar boş bırakılamaz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Geri = false;
                }
            }
            else if (AktifPanel == "pnlIsveren")
            {
                pnlIsveren.Visible = false;

                pnlAltIsveren.Visible = true;

                AktifPanel = "pnlAltIsveren";
            }
            else if (AktifPanel == "pnlAltIsveren")
            {
                pnlAltIsveren.Visible = false;

                pnlBorcluAyBilgileri.Visible = true;

                AktifPanel = "pnlBorcluAyBilgileri";

                btnIleri.BackgroundImage = global::TesvikProgrami.Properties.Resources.Kaydet1;
            }
            else if (AktifPanel == "pnlBorcluAyBilgileri")
            {
                Kaydet();
            }

            btnGeri.Visible = Geri;
        }

        private void btnGeri_Click(object sender, EventArgs e)
        {

            if (AktifPanel == "pnlIsveren")
            {
                pnlIsveren.Visible = false;

                pnlIsyeriBilgileri.Visible = true;

                AktifPanel = "pnlIsyeriBilgileri";

                btnGeri.Visible = false;

                btnIsyeriBilgileriniAl.Visible = true;
            }
            else if (AktifPanel == "pnlAltIsveren")
            {
                pnlAltIsveren.Visible = false;

                pnlIsveren.Visible = true;

                AktifPanel = "pnlIsveren";
            }
            else if (AktifPanel == "pnlBorcluAyBilgileri")
            {
                pnlBorcluAyBilgileri.Visible = false;

                pnlAltIsveren.Visible = true;

                AktifPanel = "pnlAltIsveren";

                btnIleri.BackgroundImage = global::TesvikProgrami.Properties.Resources.İleri;

            }


        }

        private void frmIsyeriEkle_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = dr;
        }

        void SirketleriDoldur()
        {
            if (sirketler == null)
            {
                using (var dbContext = new DbEntities())
                {
                    sirketler = dbContext.Sirketler.OrderBy(p => p.SirketAdi).ToList();

                }

                sirketler.Insert(0, new Sirketler
                {
                    SirketID = 0,
                    SirketAdi = "Seçiniz",
                    VergiKimlikNo = "-1"
                });

            }

            cmbSirket.DisplayMember = "SirketAdi";

            cmbSirket.ValueMember = "SirketID";

            cmbSirket.DataSource = sirketler;
        }

        private void btnIsyeriBilgileriniAl_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(txtKullaniciAdi.Text)
                 && !string.IsNullOrEmpty(txtKullaniciKod.Text)
                 && !string.IsNullOrEmpty(txtSistemSifresi.Text)
                 && !string.IsNullOrEmpty(txtIsyeriSifresi.Text))
            {
                var isyeri = new Isyerleri { KullaniciAdi = txtKullaniciAdi.Text.Trim(), KullaniciKod = txtKullaniciKod.Text.Trim(), SistemSifresi = txtSistemSifresi.Text, IsyeriSifresi = txtIsyeriSifresi.Text };
                Classes.ProjeGiris projeGiris = new Classes.ProjeGiris(isyeri, Enums.ProjeTurleri.EBildirgeV1);

                var girisCevabi = string.Empty;

                var denemeSayisi = 0;
                do
                {
                    girisCevabi = projeGiris.Connect();

                    if (girisCevabi.Equals("Kullanıcı adı veya şifreleriniz hatalıdır")
                    || girisCevabi.Equals("5 denemeye rağmen vergi kimlik numarası doğrulaması gerçekleştirilemedi")
                    || girisCevabi.Equals("İşyeri Kanun Kapsamından Çıkmıştır")
                    || girisCevabi.Equals("Is Yeri Iz Olmus")
                    || girisCevabi.Equals("işyeri hesabı PASİF olduğu için sisteme giriş yapamadı")
                    || girisCevabi.Equals("Vekalet Süresi Dolmuştur")
                     )
                    {
                        MessageBox.Show(girisCevabi, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }

                    denemeSayisi++;
                }
                while (!girisCevabi.Equals("OK") && denemeSayisi < 3);

                if (projeGiris.Connected)
                {
                    var response = projeGiris.Get("https://ebildirge.sgk.gov.tr/WPEB/amp/ToAnaMenu", "");

                    if (response.Contains("Aylık Prim ve Hizmet Belgesi Giriş Ana Menü"))
                    {
                        HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();

                        html.LoadHtml(response);

                        var sicil = html.DocumentNode.SelectSingleNode("/html/body/table[3]/tr/td/table/tr[2]/td[2]/table[1]/tr/td[1]/table/tr[2]/td[3]").InnerText.Trim();
                        var SirketAdi = html.DocumentNode.SelectSingleNode("/html/body/table[3]/tr/td/table/tr[2]/td[2]/table[1]/tr/td[1]/table/tr[3]/td[3]").InnerText.Trim();
                        var Adres = html.DocumentNode.SelectSingleNode("/html/body/table[3]/tr/td/table/tr[2]/td[2]/table[1]/tr/td[1]/table/tr[4]/td[3]").InnerText.Trim();
                        Adres = String.Join(" ", Adres.Split(' ').Where(p => !string.IsNullOrEmpty(p)));
                        var sosyalGuvenlikKurumu = html.DocumentNode.SelectSingleNode("/html/body/table[3]/tr/td/table/tr[2]/td[2]/table[1]/tr/td[1]/table/tr[5]/td[3]").InnerText.Trim();
                        var VergiKimlikNo = html.DocumentNode.SelectSingleNode("/html/body/table[3]/tr/td/table/tr[2]/td[2]/table[1]/tr/td[1]/table/tr[6]/td[3]/font/table/tr[1]/td[6]").InnerText.Trim();

                        var matches = Regex.Match(sicil, "([\\d-]*)\\W/\\W(\\d*)");

                        var SicilNo = matches.Groups[1].Value.Replace("-", "");
                        var TaseronNo = matches.Groups[2].Value;
                        var SosyalGuvenlikKurumu = Regex.Match(sosyalGuvenlikKurumu, ".*SGK\\W(.*)\\WSOS").Groups[1].Value;

                        long SirketID = -1;

                        var eskiSirket = sirketler.FirstOrDefault(p => p.VergiKimlikNo.Equals(VergiKimlikNo));

                        if (eskiSirket != null)
                        {
                            SirketID = eskiSirket.SirketID;
                        }
                        else
                        {

                            sirketler.Add(new Sirketler
                            {
                                SirketID = -1,
                                SirketAdi = SirketAdi,
                                VergiKimlikNo = VergiKimlikNo
                            });

                            cmbSirket.DataSource = sirketler.OrderBy(p => p.SirketAdi).ToList();
                        }

                        if (eskielementID > 0)
                        {

                            foreach (Control c in pnlIsyeriBilgileri.Controls)
                            {
                                if (c.Equals(txtKullaniciAdi) || c.Equals(txtKullaniciKod) || c.Equals(txtSistemSifresi) || c.Equals(txtIsyeriSifresi)) continue;
                                if (c is TextBox) c.Text = "";
                            }

                            foreach (Control c in gbIsveren.Controls)
                            {
                                if (c is TextBox) c.Text = "";
                            }

                            foreach (Control c in gbAltIsveren.Controls)
                            {
                                if (c is TextBox) c.Text = "";
                            }

                            foreach (Control c in gbSigortaliyiDevirAlan.Controls)
                            {
                                if (c is TextBox) c.Text = "";
                            }
                        }

                        cmbSirket.SelectedValue = SirketID;

                        txtIsverenAdSoyad.Text = cmbSirket.Text;

                        txtIsyeriSicilNo.Text = SicilNo;

                        txtTaseron.Text = TaseronNo;

                        txtSosyalGuvenlikKurumu.Text = SosyalGuvenlikKurumu;

                        txtIsverenAdres.Text = Adres;

                    }

                    projeGiris.Disconnect();
                }
                else MessageBox.Show("Üç denemeye rağmen sisteme bağlanılamadı. İşyeri bilgileri çekilemedi", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else MessageBox.Show("İşyeri bilgilerini çekebilmek için lütfen ilk olarak kullanıcı adı ve şifre alanlarını doldurunuz");
        }

        private void btnBasvuruFormuGozat_Click(object sender, EventArgs e)
        {
            if (ofdBasvuruFormu.ShowDialog() == DialogResult.OK)
            {
                txtBasvuruFormu.Text = Path.GetFileName(ofdBasvuruFormu.FileName);
            }

            lnklblBasvuru.Visible = txtBasvuruFormu.Text != "";
        }

        private void btnAPHBGozat_Click(object sender, EventArgs e)
        {
            if (ofdAPHB.ShowDialog() == DialogResult.OK)
            {
                txtAPHB.Text = Path.GetFileName(ofdAPHB.FileName);
            }

            lnklblAphb.Visible = txtAPHB.Text != "";
        }

        private void lnklblBasvuru_Click(object sender, EventArgs e)
        {
            txtBasvuruFormu.Text = "";

            lnklblBasvuru.Visible = false;
        }

        private void lnklblAphb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtAPHB.Text = "";

            lnklblAphb.Visible = false;
        }

        private void frmIsyeriEkle_Activated(object sender, EventArgs e)
        {

            if (Clipboard.ContainsText())
            {

                string isyeribilgileri = Clipboard.GetText(TextDataFormat.Text);

                if (isyeribilgileri.Contains("Sicil No") && isyeribilgileri.Contains("Vergi Daire"))
                {
                    bool devam = true;

                    if (!IsyeriDialogGosterilecek)
                    {
                        if (!KayitEkleme && drdialogisyeri == DialogResult.None)
                        {
                            IsyeriDialogGosterilecek = true;

                            this.Activated -= frmIsyeriEkle_Activated;

                            drdialogisyeri = MessageBox.Show("Kopyalanan işyeri bilgisinin açtığınız işyeri için kullanılmasını istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            this.Activated += frmIsyeriEkle_Activated;
                        }
                    }

                    if (!KayitEkleme && drdialogisyeri == DialogResult.No) devam = false;

                    if (devam)
                    {

                        drdialogisyeri = DialogResult.Yes;

                        string SicilNo = isyeribilgileri.Substring(isyeribilgileri.IndexOf("Sicil No") + "Sicil No".Length, isyeribilgileri.IndexOf("Ünvan") - isyeribilgileri.IndexOf("Sicil No") - "Sicil No".Length);

                        SicilNo = SicilNo.Replace("\t", "");

                        SicilNo = SicilNo.Contains(" : ") ? SicilNo.Replace(" : ", "") : SicilNo;

                        SicilNo = SicilNo.Contains("\r\n") ? SicilNo.Replace("\r\n", " ").Trim() : SicilNo;

                        string TaseronNo = SicilNo.Contains("/") ? SicilNo.Substring(SicilNo.IndexOf("/") + 1).Trim() : "000";

                        TaseronNo = TaseronNo.Contains(" ") ? TaseronNo.Split(' ')[0] : TaseronNo;

                        SicilNo = SicilNo.Contains("/") ? SicilNo.Substring(0, SicilNo.IndexOf("/")).Trim() : SicilNo;

                        SicilNo = SicilNo.Contains("-") ? SicilNo.Replace("-", "") : SicilNo;

                        string Unvan = isyeribilgileri.Substring(isyeribilgileri.IndexOf("Ünvan") + "Ünvan".Length, isyeribilgileri.IndexOf("Adresi") - isyeribilgileri.IndexOf("Ünvan") - "Ünvan".Length);

                        Unvan = Unvan.Replace("\t", "");

                        Unvan = Unvan.Contains(" : ") ? Unvan.Replace(" : ", "") : Unvan;

                        Unvan = Unvan.Contains("\r\n") ? Unvan.Replace("\r\n", " ").Trim() : Unvan;

                        string SGKSube = isyeribilgileri.Substring(isyeribilgileri.IndexOf("SGM kod-ad") + "SGM kod-ad".Length, isyeribilgileri.IndexOf("Tescil tipi") - isyeribilgileri.IndexOf("SGM kod-ad") - "SGM kod-ad".Length);

                        SGKSube = SGKSube.Replace("\t", "");

                        SGKSube = SGKSube.Contains(" : ") ? SGKSube.Replace(" : ", "") : SGKSube;

                        SGKSube = SGKSube.Contains("\r\n") ? SGKSube.Replace("\r\n", " ").Trim() : SGKSube;

                        SGKSube = SGKSube.Substring(SGKSube.IndexOf("SGK ") + 4, SGKSube.IndexOf(" ", SGKSube.IndexOf("SGK ") + 4) - SGKSube.IndexOf("SGK ") - 4);

                        string Adresi = isyeribilgileri.Substring(isyeribilgileri.IndexOf("Adresi") + "Adresi".Length, isyeribilgileri.IndexOf("SGM kod-ad") - isyeribilgileri.IndexOf("Adresi") - "Adresi".Length);

                        Adresi = Adresi.Replace("\t", "");

                        Adresi = Adresi.Contains(" : ") ? Adresi.Replace(" : ", "") : Adresi;

                        Adresi = Adresi.Contains("\r\n") ? Adresi.Replace("\r\n", " ").Trim() : Adresi;

                        string VergiKimlikNo = isyeribilgileri.Substring(isyeribilgileri.IndexOf("Vergi Kimlik No") + "Vergi Kimlik No".Length, isyeribilgileri.IndexOf("Vergi Daire") - isyeribilgileri.IndexOf("Vergi Kimlik No") - "Vergi Kimlik No".Length);

                        VergiKimlikNo = VergiKimlikNo.Replace("\t", "");

                        VergiKimlikNo = VergiKimlikNo.Contains(":") ? VergiKimlikNo.Replace(":", "") : VergiKimlikNo;

                        VergiKimlikNo = VergiKimlikNo.Contains("\r\n") ? VergiKimlikNo.Replace("\r\n", " ").Trim() : VergiKimlikNo;

                        long SirketID = -1;

                        if (sirketler == null) SirketleriDoldur();

                        var eskiSirket = sirketler.FirstOrDefault(p => p.VergiKimlikNo.Equals(VergiKimlikNo));

                        if (eskiSirket != null)
                        {
                            SirketID = eskiSirket.SirketID;
                        }
                        else
                        {

                            sirketler.Add(new Sirketler
                            {
                                SirketID = -1,
                                SirketAdi = Unvan,
                                VergiKimlikNo = VergiKimlikNo
                            });

                            cmbSirket.DataSource = sirketler.OrderBy(p => p.SirketAdi).ToList();
                        }


                        foreach (Control c in pnlIsyeriBilgileri.Controls)
                        {
                            if (SifreBilgileriKopyalandi && (c.Name == "txtKullaniciAdi" || c.Name == "txtKullaniciKod" || c.Name == "txtSistemSifresi" || c.Name == "txtIsyeriSifresi")) continue;

                            if (SubeAdiKopyalandi && c.Name == "txtSubeAdi") continue;

                            if (c is TextBox) c.Text = "";
                        }

                        foreach (Control c in gbIsveren.Controls)
                        {
                            if (c is TextBox) c.Text = "";
                        }

                        foreach (Control c in gbAltIsveren.Controls)
                        {
                            if (c is TextBox) c.Text = "";
                        }

                        foreach (Control c in gbSigortaliyiDevirAlan.Controls)
                        {
                            if (c is TextBox) c.Text = "";
                        }


                        cmbSirket.SelectedValue = SirketID;

                        txtIsverenAdSoyad.Text = cmbSirket.Text;

                        txtIsyeriSicilNo.Text = SicilNo;

                        txtTaseron.Text = TaseronNo;

                        txtSosyalGuvenlikKurumu.Text = SGKSube;

                        txtIsverenAdres.Text = Adresi;
                    }

                }
                else
                {
                    string kullanicibilgileri = Clipboard.GetText();

                    kullanicibilgileri = kullanicibilgileri.Replace("\r\n", "\t").Replace("\t\t", "\t").Trim('\t');

                    var splits = kullanicibilgileri.Split('\t');

                    if (splits.Length == 4 || splits.Length == 5)
                    {
                        if ((splits.Length == 4 && splits[0].Length == 11) || (splits.Length == 5 && splits[1].Length == 11))
                        {
                            bool devam = true;

                            if (!SifreDialogGosterilecek)
                            {
                                if (!KayitEkleme && txtKullaniciAdi.Text != "" && drdialogsifre == DialogResult.None)
                                {

                                    int i = splits.Length == 5 ? 1 : 0;

                                    var kullaniciAdi = splits[i];
                                    var kullaniciKod = splits[i + 1];
                                    var sistemSifresi = splits[i + 2];
                                    var isyeriSifresi = splits[i + 3];

                                    if (!txtKullaniciAdi.Text.Equals(kullaniciAdi) || !txtKullaniciKod.Text.Equals(kullaniciKod) || !txtSistemSifresi.Text.Equals(sistemSifresi) || !txtIsyeriSifresi.Text.Equals(isyeriSifresi))
                                    {

                                        SifreDialogGosterilecek = true;

                                        this.Activated -= frmIsyeriEkle_Activated;

                                        drdialogsifre = MessageBox.Show("Kopyalanan şifre bilgilerinin açtığınız işyeri için kullanılmasını istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                        this.Activated += frmIsyeriEkle_Activated;
                                    }
                                }
                            }
                            else
                            {
                                SifreDialogGosterilecek = false;
                            }

                            if (!KayitEkleme && drdialogsifre == DialogResult.No) devam = false;

                            if (devam)
                            {

                                int i = 0;

                                if (splits.Length == 5)
                                {
                                    txtSubeAdi.Text = splits[i];

                                    SubeAdiKopyalandi = true;

                                    i++;
                                }

                                var kullaniciAdi = splits[i];
                                var kullaniciKod = splits[i + 1];
                                var sistemSifresi = splits[i + 2];
                                var isyeriSifresi = splits[i + 3];

                                txtKullaniciAdi.Text = splits[i];

                                txtKullaniciKod.Text = splits[i + 1];

                                txtSistemSifresi.Text = splits[i + 2];

                                txtIsyeriSifresi.Text = splits[i + 3];

                                SifreBilgileriKopyalandi = true;
                            }
                        }
                    }
                }
            }
        }

        private void btn7166Gozat_Click(object sender, EventArgs e)
        {
            if (ofdBasvuruListesi7166.ShowDialog() == DialogResult.OK)
            {
                txt7166Listesi.Text = Path.GetFileName(ofdBasvuruListesi7166.FileName);
            }

            lnklbl7166.Visible = txt7166Listesi.Text != "";
        }

        private void lnklbl7166_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txt7166Listesi.Text = "";

            lnklbl7166.Visible = false;
        }

        private void tlpBorcluAylar_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Row <= DateTime.Today.Year - 2011)
            {
                e.Graphics.DrawLine(Pens.White, new Point(e.CellBounds.Location.X, e.CellBounds.Location.Y + e.CellBounds.Height), new Point(e.CellBounds.Right, e.CellBounds.Bottom));
            }

        }
    }
}
