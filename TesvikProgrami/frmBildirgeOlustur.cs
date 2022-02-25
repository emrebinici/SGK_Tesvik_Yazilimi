using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Excel2 = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using System.Data.Entity;
using System.IO.Compression;

namespace TesvikProgrami
{
    public partial class frmBildirgeOlustur : Form
    {
        string ayliklisteyolu = null;

        string basvurulisteyolu = null;

        string cariAphbYolu = null;

        List<object> HafizadanAtilacaklar = new List<object>();

        Dictionary<string, Excel2.Range> IlkSayfaHucreleri = new Dictionary<string, Excel2.Range>();

        Dictionary<string, Excel2.Range> DevamSayfaHucreleri = new Dictionary<string, Excel2.Range>();

        public Isyerleri Isyeri = null;

        bool CariHesapla = true;

        bool FaraziHesapla = false;

        bool GecmisEski = false;

        public frmBildirgeOlustur(bool Cari, bool Farazi, bool _GecmisEski)
        {
            CariHesapla = Cari;

            FaraziHesapla = Farazi;

            GecmisEski = _GecmisEski;

            InitializeComponent();
        }


        /// <summary>
        /// TÜM HESAPLAMALAR BURADA YAPILMAKTADIR !!!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBildirgeOlustur_Click(object sender, EventArgs e)
        {
            btnBildirgeOlustur.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            if (Program.AcikKalanExcellerKapatilsin)
            {
                var excelprocesses = Process.GetProcessesByName("EXCEL");

                foreach (var excelprocess in excelprocesses)
                {
                    if (excelprocess.MainWindowHandle.Equals(IntPtr.Zero))
                    {
                        Metodlar.KillProcessById(excelprocess.Id);
                    }
                }
            }

            bool SadeceIcmal = sender == btnIcmalListesiOlustur;

            progressBar1.Value = 0;

            var output = Path.Combine(Application.StartupPath, "output");

            if (Directory.Exists(output))
            {

                string[] files = Directory.GetFiles(output);

                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }

                string[] dirs = Directory.GetDirectories(output);

                foreach (string dir in dirs)
                {
                    try
                    {
                        Directory.Delete(dir, true);
                    }
                    catch { }
                }
            }

            if (txtAylikCalisanListesiYolu.Text != "" || txtCariAphb.Text != "")
            {
                var tesvikHesapla= new Classes.TesvikHesapla();
                tesvikHesapla.ayliklisteyolu = ayliklisteyolu;
                tesvikHesapla.basvurulisteyolu = basvurulisteyolu;
                tesvikHesapla.cariAphbYolu = cariAphbYolu;
                tesvikHesapla.Isyeri = this.Isyeri;
                tesvikHesapla.CariHesapla = CariHesapla;
                tesvikHesapla.BasvuruYoksaTesvikVerilmesin = FaraziHesapla ? false : Program.BasvuruYoksaTesvikVerilmesin;
                tesvikHesapla.AsgariUcretDestekTutarlariDikkateAlinsin = FaraziHesapla ? false : Program.AsgariUcretDestekTutariDikkateAlinsin.Any(p=> p.Value);
                tesvikHesapla.FaraziHesapla = FaraziHesapla;

                tesvikHesapla.BildirgeOlusturmayaBasla(
                    this,
                    SadeceIcmal
                );

            }
            else MessageBox.Show("Zorunlu alanları boş bırakmayınız", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

            this.Cursor = Cursors.Default;

            progressBar1.Visible = false;
            progressBar1.Value = 0;

            this.Close();

        }
        // btnBildirgeOlustur_Click end;

        public delegate void delProgressGuncelle(int deger);

        public void ProgressGuncelle(int deger)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new delProgressGuncelle(ProgressGuncelle));
            }
            else
            {
                progressBar1.Value = deger;
            }
        }

        private void btnBasvuruFormuGozat_Click(object sender, EventArgs e)
        {
            if (ofdBasvuruFormu.ShowDialog() == DialogResult.OK)
            {
                txtBasvuruFormuYolu.Text = Path.GetFileName(ofdBasvuruFormu.FileName);

                basvurulisteyolu = ofdBasvuruFormu.FileName;

                lnklblBasvuru.Visible = true;
            }
        }

        private void btnAylikCalisanListesiGozat_Click(object sender, EventArgs e)
        {
            if (ofdAylikListe.ShowDialog() == DialogResult.OK)
            {
                txtAylikCalisanListesiYolu.Text = Path.GetFileName(ofdAylikListe.FileName);

                ayliklisteyolu = ofdAylikListe.FileName;

                lnklblAphb.Visible = true;
            }
        }

        private void btnCariAphbGozat_Click(object sender, EventArgs e)
        {
            if (ofdCariAphb.ShowDialog() == DialogResult.OK)
            {
                txtCariAphb.Text = Path.GetFileName(ofdCariAphb.FileName);

                cariAphbYolu = ofdCariAphb.FileName;

                lnklblTemizleCariAphb.Visible = true;
            }
        }

        //void IsyeriDoldur()
        //{
        //    List<Isyerleri> isyerleri = null;

        //    if (cmbSirket.SelectedValue.ToString() != "0")
        //    {

        //        var seciliSirketID = (cmbSirket.SelectedItem as Sirketler).SirketID;

        //        using (var dbContext = new DbEntities())
        //        {
        //            isyerleri = dbContext.Isyerleri
        //                .Include(p => p.Sirketler)
        //                .Include(p => p.AylikCalisanSayilari)
        //                .Include(p => p.AsgariUcretDestekTutarlari)
        //                .Include(p => p.BasvuruDonemleri)
        //                .Include(p => p.BorcluAylar)
        //                .Where(p => p.SirketID.Equals(seciliSirketID) && p.Aktif.Equals(1))
        //                .OrderBy(p => p.SubeAdi).ToList();

        //            isyerleri = isyerleri.OrderBy(p => p.SubeAdi, StringComparer.Create(new System.Globalization.CultureInfo("tr-TR"), true)).ToList();

        //        }
        //    }
        //    else
        //    {
        //        if (!File.Exists(ofdBasvuruFormu.FileName)) txtBasvuruFormuYolu.Text = string.Empty;
        //        if (!File.Exists(ofdAylikListe.FileName)) txtAylikCalisanListesiYolu.Text = string.Empty;
        //    }

        //    isyerleri = isyerleri ?? new List<Isyerleri>();

        //    isyerleri.Insert(0, new Isyerleri
        //    {
        //        IsyeriID = 0,
        //        SubeAdi = "Seçiniz",
        //        IsyeriSicilNo = "Seçiniz"
        //    });

        //    cmbIsyeri.DisplayMember = "SubeAdi";

        //    cmbIsyeri.ValueMember = "IsyeriID";

        //    cmbIsyeri.DataSource = isyerleri;

        //    if (isyerleri.Count == 2) cmbIsyeri.SelectedValue = isyerleri.FirstOrDefault(p => p.IsyeriID > 0).IsyeriID;
        //}

        //void SirketleriDoldur()
        //{
        //    using (var dbContext = new DbEntities())
        //    {
        //        var sirketler = dbContext.Sirketler.Where(p => p.Aktif.Equals(1)).OrderBy(p => p.SirketAdi).ToList();

        //        sirketler.Insert(0, new Sirketler
        //        {
        //            SirketID = 0,
        //            SirketAdi = "Seçiniz",
        //            VergiKimlikNo = ""
        //        });


        //        cmbSirket.DisplayMember = "SirketAdi";

        //        cmbSirket.ValueMember = "SirketID";

        //        cmbSirket.DataSource = sirketler;
        //    }

        //}


        private void frmBildirgeOlustur_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;


            //SirketleriDoldur();

            //IsyeriDoldur();

            var aphb= Metodlar.FormBul(Isyeri,Enums.FormTuru.Aphb);
            var basvuruformu= Metodlar.FormBul(Isyeri,Enums.FormTuru.BasvuruFormu);

            if (aphb != null)
            {
                txtAylikCalisanListesiYolu.Text = Path.GetFileName(aphb);
                ayliklisteyolu = aphb;
            }
            else
            {
                txtAylikCalisanListesiYolu.Text = "";
                ayliklisteyolu = null;
            }

            if (basvuruformu != null)
            {
                txtBasvuruFormuYolu.Text = Path.GetFileName(basvuruformu);
                basvurulisteyolu = basvuruformu;
            }
            else
            {
                txtBasvuruFormuYolu.Text = "";
                basvurulisteyolu = null;
            }

            if (CariHesapla || (!FaraziHesapla && ! GecmisEski && Program.Son6AyGecmisHesaplansin))
            {
                var baslangic = DateTime.Now.AddMonths(-6);

                txtYil.Text = baslangic.Year.ToString();
                txtAy.Text = baslangic.Month.ToString();
            }

            ofdBasvuruFormu.FileName = "";
            ofdAylikListe.FileName = "";

            if (!CariHesapla)
            {
                if (!string.IsNullOrEmpty(aphb) && !string.IsNullOrEmpty(basvuruformu))
                {
                    btnBildirgeOlustur.Enabled = false;

                    Task.Delay(1000).ContinueWith((task) =>
                    {
                        this.Invoke(new delBildirgeOlusturmayaBasla(BildirgeOlusturmayaBasla));
                    });
                }
            }
        }

        void BildirgeOlusturmayaBasla()
        {
            btnBildirgeOlustur_Click(btnBildirgeOlustur, null);
        }

        delegate void delBildirgeOlusturmayaBasla();

        private void lnklblBasvuru_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBasvuruFormuYolu.Text))
            {
                txtBasvuruFormuYolu.Text = "";
            }

            ofdBasvuruFormu.FileName = "";

            lnklblBasvuru.Visible = false;
        }

        private void lnklblAphb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAylikCalisanListesiYolu.Text))
            {
                txtAylikCalisanListesiYolu.Text = "";
            }

            lnklblAphb.Visible = false;
        }

        private void lnklblCariAphb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCariAphb.Text))
            {
                txtCariAphb.Text = "";
            }

            ofdAylikListe.FileName = "";

            cariAphbYolu = null;

            lnklblTemizleCariAphb.Visible = false;
        }

    }
}
