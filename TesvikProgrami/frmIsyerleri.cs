using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using mshtml;
using SHDocVw;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Tesseract;
using TesvikProgrami.Classes;
using Excel2 = Microsoft.Office.Interop.Excel;
using System.Data.Entity;
using DocumentFormat.OpenXml.Office2013.Drawing.Chart;
//using Microsoft.Win32;

namespace TesvikProgrami
{
    public partial class frmIsyerleri : Form
    {
        #region Global Değişkenler

        //private void SetBrowserFeatureControlKey(string feature, string appName, uint value)
        //{
        //    using (var key = Registry.CurrentUser.CreateSubKey(
        //        String.Concat(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\", feature),
        //        RegistryKeyPermissionCheck.ReadWriteSubTree))
        //    {
        //        key.SetValue(appName, (UInt32)value, RegistryValueKind.DWord);
        //    }
        //}

        //private void SetBrowserFeatureControl()
        //{
        //    // FeatureControl settings are per-process
        //    var fileName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

        //    // make the control is not running inside Visual Studio Designer
        //    if (String.Compare(fileName, "devenv.exe", true) == 0 || String.Compare(fileName, "XDesProc.exe", true) == 0)
        //        return;

        //    
        //    SetBrowserFeatureControlKey("FEATURE_BROWSER_EMULATION", fileName, 9000); // Webpages containing standards-based !DOCTYPE directives are displayed in IE10 Standards mode.
        //    SetBrowserFeatureControlKey("FEATURE_DISABLE_NAVIGATION_SOUNDS", fileName, 1);
        //    SetBrowserFeatureControlKey("FEATURE_WEBOC_POPUPMANAGEMENT", fileName, 1);
        //    SetBrowserFeatureControlKey("FEATURE_BLOCK_INPUT_PROMPTS", fileName, 1);
        //}

        public static bool ilkGelis = true;

        DialogResult dr = DialogResult.Cancel;

        public List<Isyerleri> isyerleriListesi { get; set; }

        bool Ara = false;



        frmSirketler formSirketler = null;

        #endregion

        #region Isyerleri Form İşlemleri

        public frmIsyerleri()
        {
            InitializeComponent();
        }

        private void frmIsyerleri_Load(object sender, EventArgs e)
        {
            IsyerleriDoldur();

            folderBrowserDialogBildirgeYukle.RootFolder = Environment.SpecialFolder.MyComputer;

            folderBrowserDialogBildirgeYukle.SelectedPath = System.IO.Path.Combine(Application.StartupPath, "output");

            txtAra.Focus();
        }

        private void chkPasifleriGoster_CheckedChanged(object sender, EventArgs e)
        {
            IsyerleriDoldur();

        }

        public IsyeriListesiDTO IsyeriListesiResponse(Isyerleri isyeri)
        {

            return new IsyeriListesiDTO
            {
                ID = isyeri.IsyeriID.ToString(),
                SirketID = isyeri.SirketID.ToString(),
                IsyeriSicilNo = isyeri.IsyeriSicilNo + (isyeri.TaseronNo != null && !isyeri.TaseronNo.Equals("000") ? ("-" + isyeri.TaseronNo) : ""),
                SosyalGuvenlikKurumu = isyeri.SosyalGuvenlikKurumu,
                SirketAdi = isyeri.Sirketler.SirketAdi,
                SubeAdi = isyeri.SubeAdi,
                KullaniciAdi = isyeri.KullaniciAdi,
                KullaniciKod = isyeri.KullaniciKod,
                SistemSifresi = isyeri.SistemSifresi,
                IsyeriSifresi = isyeri.IsyeriSifresi,
                BasvuruFormu = isyeri.BasvuruFormu,
                APHB = isyeri.Aphb,
                Aktif = isyeri.Aktif.Equals(0) ? "False" : "True",
                BasvuruListe7166 = isyeri.BasvuruListesi7166
            };


        }

        public void IsyerleriDoldur(bool VeritabanindanCek = true, int skip = 0, int take = 25)
        {
            if (VeritabanindanCek) this.isyerleriListesi = null;

            if (this.isyerleriListesi == null)
            {

                using (var dbContext = new DbEntities())
                {
                    this.isyerleriListesi = dbContext.Isyerleri.Include(p => p.Sirketler).ToList();
                }

            }

            var isyerleri = this.isyerleriListesi.Select(p => IsyeriListesiResponse(p)).ToList();

            if (!chkPasifleriGoster.Checked)
            {
                isyerleri = isyerleri.Where(p => p.Aktif.Equals("True")).ToList();
            }


            if (Ara)
            {
                if (this.isyerleriListesi.Count > 0)
                {
                    var sonuc = this.isyerleriListesi.Where(
                            a => a.Sirketler.SirketAdi.ToUpper().Contains(txtAra.Text.ToUpper())
                            || a.IsyeriSicilNo.ToUpper().Contains(txtAra.Text.ToUpper())
                            || a.SubeAdi.ToUpper().Contains(txtAra.Text.ToUpper())
                        ).Select(p => IsyeriListesiResponse(p)).ToList();

                    if (!chkPasifleriGoster.Checked)
                    {
                        sonuc = sonuc.Where(a => a.Aktif.Equals("True")).ToList();
                    }

                    if (sonuc.Count <= 10)
                    {
                        sonuc.ForEach(p => p.CariAphb = System.IO.Path.GetFileName(Metodlar.FormBul(p, Enums.FormTuru.CariAphb)));
                    }

                    dgvIsyerleri.AutoGenerateColumns = false;

                    dgvIsyerleri.DataSource = sonuc;
                }
            }
            else
            {
                dgvIsyerleri.AutoGenerateColumns = false;

                var isyerleriFiltered = isyerleri.Skip(skip).Take(take).ToList();

                //isyerleriFiltered.ForEach(p => p.CariAphb = System.IO.Path.GetFileName(Metodlar.FormBul(p, Enums.FormTuru.CariAphb)));

                dgvIsyerleri.DataSource = isyerleriFiltered;

            }

            statusIsyeriAdeti.Text = string.Format("Toplam İşyeri Sayısı: {0}", this.isyerleriListesi.Count);


        }

        private void dgvIsyerleri_SelectionChanged(object sender, EventArgs e)
        {
            dgvIsyerleri.ClearSelection();
        }
        private void dgvIsyerleri_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                var row = dgvIsyerleri.Rows[e.RowIndex];

                var isyeri = dgvIsyerleri.Rows[e.RowIndex].DataBoundItem as IsyeriListesiDTO;

                if (row == null)
                {
                    return;
                }

                var isyeriID = Convert.ToInt64(isyeri.ID);

                if (dgvIsyerleri.Columns[e.ColumnIndex].Name == "colDuzenle")
                {
                    frmIsyeriEkle FrmIsyeriEkle = new frmIsyeriEkle { eskielementID = isyeriID };

                    if (FrmIsyeriEkle.ShowDialog() == DialogResult.OK)
                    {
                        dr = DialogResult.OK;

                        IsyerleriDoldur();

                    }

                }
                else if (dgvIsyerleri.Columns[e.ColumnIndex].Name == "colSil")
                {
                    if (MessageBox.Show("Silmek istediğinizden emin misiniz", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        using (var dbContext = new DbEntities())
                        {
                            dbContext.Isyerleri.Remove(dbContext.Isyerleri.Find(isyeriID));

                            dbContext.SaveChanges();
                        }

                        dr = DialogResult.OK;

                        IsyerleriDoldur();

                        var isyeripath = Metodlar.IsyeriKlasorBul(isyeri);

                        if (isyeripath != null)
                        {
                            try
                            {
                                Directory.Delete(isyeripath, true);
                            }
                            catch { }
                        }

                    }
                }
                else if (dgvIsyerleri.Columns[e.ColumnIndex].Name == "colBasvuruFormu")
                {
                    string basvuruyol = Metodlar.FormBul(isyeri, Enums.FormTuru.BasvuruFormu);

                    if (basvuruyol != null)
                    {
                        Process.Start(basvuruyol);
                    }

                }
                else if (dgvIsyerleri.Columns[e.ColumnIndex].Name == "colAPHB")
                {
                    string aphbyol = Metodlar.FormBul(isyeri, Enums.FormTuru.Aphb);

                    if (aphbyol != null)
                    {
                        Process.Start(aphbyol);
                    }

                }
                else if (dgvIsyerleri.Columns[e.ColumnIndex].Name == "colCariAphb")
                {
                    string cariAphb = Metodlar.FormBul(isyeri, Enums.FormTuru.CariAphb);

                    if (cariAphb != null)
                    {
                        Process.Start(cariAphb);
                    }

                }
                else if (dgvIsyerleri.Columns[e.ColumnIndex].Name == "colSirket")
                {
                    string isyeriPath = Metodlar.IsyeriKlasorBul(isyeri, true);

                    Process.Start( new DirectoryInfo(isyeriPath).Parent.FullName);
                }
                else if (dgvIsyerleri.Columns[e.ColumnIndex].Name == "colSubeAdi")
                {
                    string isyeriPath = Metodlar.IsyeriKlasorBul(isyeri, true);

                    Process.Start(isyeriPath);
                }
                else if (dgvIsyerleri.Columns[e.ColumnIndex].Name == "colCariTanimla" || dgvIsyerleri.Columns[e.ColumnIndex].Name == "colGecmisTanimla")
                {

                    if (Program.IndirilenIsyerleri.ContainsKey(isyeriID))
                    {
                        var indirilenIsyeri = Program.IndirilenIsyerleri[isyeriID];

                        if (indirilenIsyeri.formIndirmeEkrani == null || indirilenIsyeri.formIndirmeEkrani.IsDisposed)
                        {
                            indirilenIsyeri.formIndirmeEkrani = new frmIndirmeEkrani(indirilenIsyeri);
                        }

                        indirilenIsyeri.formIndirmeEkrani.Show();
                        indirilenIsyeri.formIndirmeEkrani.BringToFront();

                    }
                    else
                    {
                        bool cariTanimla = dgvIsyerleri.Columns[e.ColumnIndex].Name == "colCariTanimla";

                        frmTarihSec formTarihSec = new frmTarihSec(cariTanimla);

                        if (formTarihSec.ShowDialog() == DialogResult.OK)
                        {

                            if (Program.IndirilenIsyerleri.ContainsKey(isyeriID)) Program.IndirilenIsyerleri.Remove(isyeriID);

                            var aphbyol = Metodlar.FormBul(isyeri, Enums.FormTuru.Aphb);

                            DataTable dtMevcutAphb = null;

                            if (aphbyol != null)
                            {
                                dtMevcutAphb = Metodlar.AylikListeyiYukle(aphbyol);
                            }

                            if (formTarihSec.secenekler.BasvuruFormuIndirilsin)
                            {
                                if (!Program.IndirilenIsyerleri.ContainsKey(isyeriID)) Program.IndirilenIsyerleri.Add(isyeriID, new IsyeriAphbBasvuruFormuIndirme());

                                var indirilenIsyeri = Program.IndirilenIsyerleri[isyeriID];

                                if (formTarihSec.secenekler.IndirTumTesvikler)
                                {
                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeriID, Enums.BasvuruFormuTurleri.BfTumu, cariTanimla);
                                    basvuruFormuIndir.incelenecekDonemler = formTarihSec.secenekler.incelenecekDonemler;
                                    basvuruFormuIndir.incelenecekDonemler7103 = formTarihSec.secenekler.incelenecekDonemler7103;
                                    basvuruFormuIndir.incelenecekDonemler2828 = formTarihSec.secenekler.incelenecekDonemler2828;
                                    basvuruFormuIndir.incelenecekDonemler7252 = formTarihSec.secenekler.incelenecekDonemler7252;
                                    basvuruFormuIndir.incelenecekDonemler7256 = formTarihSec.secenekler.incelenecekDonemler7256;
                                    basvuruFormuIndir.incelenecekDonemler7316 = formTarihSec.secenekler.incelenecekDonemler7316;
                                    basvuruFormuIndir.incelenecekDonemler3294 = formTarihSec.secenekler.incelenecekDonemler3294;
                                    basvuruFormuIndir.dtBaslangic6111 = formTarihSec.secenekler.BaslangicTum;
                                    basvuruFormuIndir.dtBaslangic7103 = formTarihSec.secenekler.BaslangicTum;
                                    basvuruFormuIndir.dtBaslangic2828 = formTarihSec.secenekler.BaslangicTum;
                                    basvuruFormuIndir.dtBaslangic7252 = formTarihSec.secenekler.BaslangicTum;
                                    basvuruFormuIndir.dtBaslangic7256 = formTarihSec.secenekler.BaslangicTum;
                                    basvuruFormuIndir.dtBaslangic7316 = formTarihSec.secenekler.BaslangicTum;
                                    basvuruFormuIndir.dtBaslangic3294 = formTarihSec.secenekler.BaslangicTum;
                                    basvuruFormuIndir.dtBitis6111 = formTarihSec.secenekler.BitisTum;
                                    basvuruFormuIndir.dtBitis7103 = formTarihSec.secenekler.BitisTum;
                                    basvuruFormuIndir.dtBitis2828 = formTarihSec.secenekler.BitisTum;
                                    basvuruFormuIndir.dtBitis7252 = formTarihSec.secenekler.BitisTum;
                                    basvuruFormuIndir.dtBitis7256 = formTarihSec.secenekler.BitisTum;
                                    basvuruFormuIndir.dtBitis7316 = formTarihSec.secenekler.BitisTum;
                                    basvuruFormuIndir.dtBitis3294 = formTarihSec.secenekler.BitisTum;
                                    basvuruFormuIndir.frmIsyerleri = this;
                                    basvuruFormuIndir.EnBastanTumu = formTarihSec.secenekler.EnBastanTumu;
                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                }

                                if (formTarihSec.secenekler.Indir6111)
                                {
                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeriID, Enums.BasvuruFormuTurleri.Bf6111, cariTanimla);
                                    basvuruFormuIndir.incelenecekDonemler = formTarihSec.secenekler.incelenecekDonemler;
                                    basvuruFormuIndir.dtBaslangic6111 = formTarihSec.secenekler.Baslangic6111;
                                    basvuruFormuIndir.dtBitis6111 = formTarihSec.secenekler.Bitis6111;
                                    basvuruFormuIndir.frmIsyerleri = this;
                                    basvuruFormuIndir.EnBastan6111 = formTarihSec.secenekler.EnBastan6111;
                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                }

                                if (formTarihSec.secenekler.Indir7103)
                                {
                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeriID, Enums.BasvuruFormuTurleri.Bf7103, cariTanimla);
                                    basvuruFormuIndir.incelenecekDonemler7103 = formTarihSec.secenekler.incelenecekDonemler7103;
                                    basvuruFormuIndir.dtBaslangic7103 = formTarihSec.secenekler.Baslangic7103;
                                    basvuruFormuIndir.dtBitis7103 = formTarihSec.secenekler.Bitis7103;
                                    basvuruFormuIndir.frmIsyerleri = this;
                                    basvuruFormuIndir.EnBastan7103 = formTarihSec.secenekler.EnBastan7103;
                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                }

                                if (formTarihSec.secenekler.Indir2828)
                                {
                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeriID, Enums.BasvuruFormuTurleri.Bf2828, cariTanimla);
                                    basvuruFormuIndir.incelenecekDonemler2828 = formTarihSec.secenekler.incelenecekDonemler2828;
                                    basvuruFormuIndir.dtBaslangic2828 = formTarihSec.secenekler.Baslangic2828;
                                    basvuruFormuIndir.dtBitis2828 = formTarihSec.secenekler.Bitis2828;
                                    basvuruFormuIndir.frmIsyerleri = this;
                                    basvuruFormuIndir.EnBastan2828 = formTarihSec.secenekler.EnBastan2828;
                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                }

                                if (formTarihSec.secenekler.Indir7252)
                                {
                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeriID, Enums.BasvuruFormuTurleri.Bf7252, cariTanimla);
                                    basvuruFormuIndir.incelenecekDonemler7252 = formTarihSec.secenekler.incelenecekDonemler7252;
                                    basvuruFormuIndir.dtBaslangic7252 = formTarihSec.secenekler.Baslangic7252;
                                    basvuruFormuIndir.dtBitis7252 = formTarihSec.secenekler.Bitis7252;
                                    basvuruFormuIndir.frmIsyerleri = this;
                                    basvuruFormuIndir.EnBastan7252 = formTarihSec.secenekler.EnBastan7252;
                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                }

                                if (formTarihSec.secenekler.Indir7256)
                                {
                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeriID, Enums.BasvuruFormuTurleri.Bf7256, cariTanimla);
                                    basvuruFormuIndir.incelenecekDonemler7256 = formTarihSec.secenekler.incelenecekDonemler7256;
                                    basvuruFormuIndir.dtBaslangic7256 = formTarihSec.secenekler.Baslangic7256;
                                    basvuruFormuIndir.dtBitis7256 = formTarihSec.secenekler.Bitis7256;
                                    basvuruFormuIndir.frmIsyerleri = this;
                                    basvuruFormuIndir.EnBastan7256 = formTarihSec.secenekler.EnBastan7256;
                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                }

                                if (formTarihSec.secenekler.Indir7316)
                                {
                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeriID, Enums.BasvuruFormuTurleri.Bf7316, cariTanimla);
                                    basvuruFormuIndir.incelenecekDonemler7316 = formTarihSec.secenekler.incelenecekDonemler7316;
                                    basvuruFormuIndir.dtBaslangic7316 = formTarihSec.secenekler.Baslangic7316;
                                    basvuruFormuIndir.dtBitis7316 = formTarihSec.secenekler.Bitis7316;
                                    basvuruFormuIndir.frmIsyerleri = this;
                                    basvuruFormuIndir.EnBastan7316 = formTarihSec.secenekler.EnBastan7316;
                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                }

                                if (formTarihSec.secenekler.Indir3294)
                                {
                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeriID, Enums.BasvuruFormuTurleri.Bf3294, cariTanimla);
                                    basvuruFormuIndir.incelenecekDonemler3294 = formTarihSec.secenekler.incelenecekDonemler3294;
                                    basvuruFormuIndir.dtBaslangic3294 = formTarihSec.secenekler.Baslangic3294;
                                    basvuruFormuIndir.dtBitis3294 = formTarihSec.secenekler.Bitis3294;
                                    basvuruFormuIndir.frmIsyerleri = this;
                                    basvuruFormuIndir.EnBastan3294 = formTarihSec.secenekler.EnBastan3294;
                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                }

                                if (formTarihSec.secenekler.Indir6645)
                                {
                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeriID, Enums.BasvuruFormuTurleri.Bf6645, cariTanimla);
                                    basvuruFormuIndir.dtBaslangic6645 = formTarihSec.secenekler.Baslangic6645;
                                    basvuruFormuIndir.dtBitis6645 = formTarihSec.secenekler.Bitis6645;
                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                    basvuruFormuIndir.frmIsyerleri = this;
                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                }

                                if (formTarihSec.secenekler.Indir687)
                                {
                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeriID, Enums.BasvuruFormuTurleri.Bf687, cariTanimla);
                                    basvuruFormuIndir.dtBaslangic687 = formTarihSec.secenekler.Baslangic687;
                                    basvuruFormuIndir.dtBitis687 = formTarihSec.secenekler.Bitis687;
                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                    basvuruFormuIndir.frmIsyerleri = this;
                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                }

                                if (formTarihSec.secenekler.Indir14857)
                                {
                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeriID, Enums.BasvuruFormuTurleri.Bf14857, cariTanimla);
                                    basvuruFormuIndir.dtBaslangic14857 = formTarihSec.secenekler.Baslangic14857;
                                    basvuruFormuIndir.dtBitis14857 = formTarihSec.secenekler.Bitis14857;
                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                    basvuruFormuIndir.frmIsyerleri = this;
                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                }


                                indirilenIsyeri.isyeri = indirilenIsyeri.BasvuruFormuIndirmeleri.FirstOrDefault().SuanYapilanIsyeriBasvuru;

                                indirilenIsyeri.BasvuruFormuIndirmeleri.ForEach(p => p.dtMevcutAphb = dtMevcutAphb);

                                //FormLog = new frmLog(this);

                                //basvuruFormuIndir.FormLog = FormLog;

                                //FormLog.Text = "Başvuru Formu İndirme (" + basvuruFormuIndir.SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi + " - " + basvuruFormuIndir.SuanYapilanIsyeriBasvuru.SubeAdi + ")";

                            }

                            if (formTarihSec.secenekler.AphbIndirilsin)
                            {
                                var aphbIndir = new AphbIndir(isyeriID);

                                if (!Program.IndirilenIsyerleri.ContainsKey(isyeriID)) Program.IndirilenIsyerleri.Add(isyeriID, new IsyeriAphbBasvuruFormuIndirme());

                                var indirilenIsyeri = Program.IndirilenIsyerleri[isyeriID];

                                aphbIndir.TarihBaslangicAphb = formTarihSec.secenekler.BaslangicAphb;
                                aphbIndir.TarihBitisAphb = formTarihSec.secenekler.BitisAphb;
                                aphbIndir.frmIsyerleri = this;
                                aphbIndir.dtMevcutAphb = dtMevcutAphb;
                                aphbIndir.indirilenIsyeri = indirilenIsyeri;

                                indirilenIsyeri.AphbIndirmeleri.Add(aphbIndir);

                                indirilenIsyeri.isyeri = indirilenIsyeri.AphbIndirmeleri.FirstOrDefault().SuanYapilanIsyeriAphb;

                                //FormLog = new frmLog(this);
                                //FormLog.Text = "APHB İndirme (" + aphbIndir.SuanYapilanIsyeriAphb.Sirketler.SirketAdi + " - " + aphbIndir.SuanYapilanIsyeriAphb.SubeAdi + ")";
                            }


                            if (Program.IndirilenIsyerleri.ContainsKey(isyeriID))
                            {
                                var indirilenisyeri = Program.IndirilenIsyerleri[isyeriID];
                                indirilenisyeri.formIsyerleri = this;

                                if (indirilenisyeri.AphbIndirmeleri.Count > 0)
                                {
                                    foreach (var aphbIndirme in indirilenisyeri.AphbIndirmeleri)
                                    {
                                        _ = aphbIndirme.Calistir();
                                    }
                                }
                                else
                                {
                                    foreach (var basvuruFormuIndirme in indirilenisyeri.BasvuruFormuIndirmeleri)
                                    {
                                        _ = basvuruFormuIndirme.Calistir();
                                    }
                                }

                                var formIndirmeEkrani = new frmIndirmeEkrani(indirilenisyeri);
                                indirilenisyeri.formIndirmeEkrani = formIndirmeEkrani;
                                formIndirmeEkrani.Text = String.Format("{0} - {1}", indirilenisyeri.isyeri.Sirketler.SirketAdi, indirilenisyeri.isyeri.SubeAdi);
                                formIndirmeEkrani.Show();
                            }
                        }
                    }

                }
                else if (dgvIsyerleri.Columns[e.ColumnIndex].Name == "colBildirgeYukle")
                {
                    if (!BildirgeYuklemeYapiliyor)
                    {
                        bool devam = true;

                        if (Program.OncekiBildirgelerIptalEdilsin == false)
                        {
                            devam = MessageBox.Show("Ayarlar sayfasında önceki bildirgeler iptal edilsin seçeneği işaretli değil. İşleme devam denmeden yeni belge oluştur ile işlem yapılacak.  Bu ayarla devam etmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                        }

                        if (devam)
                        {
                            if (folderBrowserDialogBildirgeYukle.ShowDialog() == DialogResult.OK)
                            {

                                YuklenecekBildirgeler = Directory.GetFiles(folderBrowserDialogBildirgeYukle.SelectedPath, "*Bildirge*.xls", SearchOption.AllDirectories).Where(f => !f.ToUpper().Contains("PTAL")).ToList();

                                if (YuklenecekBildirgeler.Count > 0)
                                {
                                    YuklenecekBildirgeler = YuklenecekBildirgeler.GroupBy(a => System.IO.Path.GetFileName(a).Replace("Dönemi", "Donemi").Replace("dönemi", "Donemi").Replace("Asıl", "Asil").Replace("ASIL", "Asil")).Select(p => p.First()).ToList();

                                    YuklenecekBildirgeler = YuklenecekBildirgeler
                                                                .OrderBy(p => new DateTime(System.IO.Path.GetFileName(p).Split(' ')[0].Split('-')[0].ToInt(), System.IO.Path.GetFileName(p).Split(' ')[0].Split('-')[1].ToInt(), 1))
                                                                .ThenBy(p => p.Contains("6322") || p.Contains("25510") ? 1 : 0)
                                                                .ToList();

                                    FormLog = new frmLog(this);

                                    BildirgeYuklemeyiBaslat(Convert.ToInt64(isyeri.ID));

                                    FormLog.Text = "Bildirge Yükleme (" + SuanYapilanIsyeriBildirgeYukleme.Sirketler.SirketAdi + " - " + SuanYapilanIsyeriBildirgeYukleme.SubeAdi + ")";

                                    FormLog.ShowDialog();
                                }
                                else
                                {
                                    MessageBox.Show("Seçtiğiniz klasörde yüklenecek bildirge bulunamadı", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }

                            }
                        }
                    }
                    else MessageBox.Show("Devam eden işlem olduğu için yeni bir işlem başlatamazsınız", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else if (dgvIsyerleri.Columns[e.ColumnIndex].Name == "colGit")
                {
                    var link = dgvIsyerleri.Rows[e.RowIndex].Cells["colLinkler"].FormattedValue;

                    string KullaniciAdi = isyeri.KullaniciAdi;

                    string KullaniciKod = isyeri.KullaniciKod;

                    string SistemSifresi = isyeri.SistemSifresi;

                    string IsyeriSifresi = isyeri.IsyeriSifresi;

                    if (link.Equals("E-Bildirge") || link.Equals("İşe Giriş-İşten Ayrılış"))
                    {
                        string url = "";

                        if (link.Equals("E-Bildirge"))
                        {
                            url = "https://ebildirge.sgk.gov.tr/WPEB/amp/loginldap";
                        }
                        else if (link.Equals("İşe Giriş-İşten Ayrılış"))
                        {
                            url = "https://uyg.sgk.gov.tr/SigortaliTescil/amp/loginldap";
                        }

                        string Captcha = "";

                        SHDocVw.InternetExplorer ie = null;

                    yenidenDene:
                        try
                        {
                            ie = new SHDocVw.InternetExplorer();
                        }
                        catch
                        {

                            Thread.Sleep(2000);

                            goto yenidenDene;
                        }

                        ie.Silent = true;

                        IWebBrowser2 wb = (IWebBrowser2)ie.Application;

                        Metodlar.ShowWindow((IntPtr)ie.HWND, Sabitler.SW_MAXIMISE);

                        wb.Visible = true;


                        wb.Navigate(url);
                        while (wb.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE) { System.Threading.Thread.Sleep(100); }


                        try
                        {
                            if (ie != null)
                            {

                                var inputs = ((HTMLDocument)ie.Document).getElementsByTagName("input");

                                HTMLButtonElement btnsubmit = null;

                                HTMLTextAreaElement inputcaptcha = null;

                                foreach (var input in inputs)
                                {
                                    if (((HTMLTextAreaElement)input).name == "j_username") ((HTMLTextAreaElement)input).value = KullaniciAdi;
                                    else if (((HTMLTextAreaElement)input).name == "isyeri_kod") ((HTMLTextAreaElement)input).value = KullaniciKod;
                                    else if (((HTMLTextAreaElement)input).name == "j_password") ((HTMLTextAreaElement)input).value = SistemSifresi;
                                    else if (((HTMLTextAreaElement)input).name == "isyeri_sifre") ((HTMLTextAreaElement)input).value = IsyeriSifresi;
                                    else if (((HTMLTextAreaElement)input).name == "isyeri_guvenlik")
                                    {
                                        ((HTMLTextAreaElement)input).value = Captcha;

                                        inputcaptcha = ((HTMLTextAreaElement)input);
                                    }
                                    else if (((HTMLButtonElement)input).name == "btnSubmit") btnsubmit = ((HTMLButtonElement)input);

                                }

                                inputcaptcha.focus();

                            }
                        }
                        catch
                        {

                        }



                    }
                    else if (link.Equals("E-Bildirge V2"))
                    {
                        SHDocVw.InternetExplorer ie = null;

                    yenidenDene:
                        try
                        {

                            ie = new SHDocVw.InternetExplorer();
                        }
                        catch
                        {
                            Thread.Sleep(2000);

                            goto yenidenDene;
                        }

                        try
                        {

                            IWebBrowser2 wb = (IWebBrowser2)ie.Application;

                            Metodlar.ShowWindow((IntPtr)ie.HWND, Sabitler.SW_MAXIMISE);

                            wb.Visible = true;

                            wb.Navigate("https://ebildirge.sgk.gov.tr/EBildirgeV2");

                            while (wb.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE) { System.Threading.Thread.Sleep(100); }
                            var document = wb.Document;


                            if (ie != null)
                            {

                                var inputs = ((HTMLDocument)ie.Document).getElementsByTagName("input");

                                var buttons = ((HTMLDocument)ie.Document).getElementsByTagName("button");

                                HTMLTextAreaElement inputcaptcha = null;

                                DispHTMLButtonElement btnsubmit = null;

                                foreach (var input in inputs)
                                {
                                    if (((HTMLTextAreaElement)input).name == "username") ((HTMLTextAreaElement)input).value = KullaniciAdi;
                                    else if (((HTMLTextAreaElement)input).name == "isyeri_kod") ((HTMLTextAreaElement)input).value = KullaniciKod;
                                    else if (((HTMLTextAreaElement)input).name == "password") ((HTMLTextAreaElement)input).value = SistemSifresi;
                                    else if (((HTMLTextAreaElement)input).name == "isyeri_sifre") ((HTMLTextAreaElement)input).value = IsyeriSifresi;
                                    else if (((HTMLTextAreaElement)input).name == "isyeri_guvenlik")
                                    {
                                        inputcaptcha = ((HTMLTextAreaElement)input);
                                    }
                                }

                                foreach (var btn in buttons)
                                {
                                    if (((DispHTMLButtonElement)btn).className == "button button-block")
                                    {
                                        btnsubmit = ((DispHTMLButtonElement)btn);

                                        break;
                                    }
                                }

                                if (btnsubmit != null && Program.OtomatikGuvenlikKoduGirilecekEBildirgeV2)
                                {
                                    btnsubmit.click();
                                }
                                else
                                {

                                    inputcaptcha.focus();
                                }

                            }
                        }
                        catch { }

                    }
                    else if (link.Equals("İşveren Sistemi"))
                    {
                        SHDocVw.InternetExplorer ie = null;

                    yenidenDene:
                        try
                        {

                            ie = new SHDocVw.InternetExplorer();

                        }
                        catch
                        {
                            Thread.Sleep(2000);
                            goto yenidenDene;
                        }

                        try
                        {

                            IWebBrowser2 wb = (IWebBrowser2)ie.Application;

                            Metodlar.ShowWindow((IntPtr)ie.HWND, Sabitler.SW_MAXIMISE);

                            wb.Visible = true;


                            wb.Navigate("https://uyg.sgk.gov.tr/IsverenSistemi");

                            while (wb.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE) { System.Threading.Thread.Sleep(100); }
                            var document = wb.Document;


                            if (ie != null)
                            {

                                var inputs = ((HTMLDocument)ie.Document).getElementsByTagName("input");

                                var buttons = ((HTMLDocument)ie.Document).getElementsByTagName("button");

                                HTMLTextAreaElement inputcaptcha = null;

                                DispHTMLButtonElement btnsubmit = null;

                                foreach (var input in inputs)
                                {
                                    if (((HTMLTextAreaElement)input).name == "username") ((HTMLTextAreaElement)input).value = KullaniciAdi;
                                    else if (((HTMLTextAreaElement)input).name == "isyeri_kod") ((HTMLTextAreaElement)input).value = KullaniciKod;
                                    else if (((HTMLTextAreaElement)input).name == "password") ((HTMLTextAreaElement)input).value = SistemSifresi;
                                    else if (((HTMLTextAreaElement)input).name == "isyeri_sifre") ((HTMLTextAreaElement)input).value = IsyeriSifresi;
                                    else if (((HTMLTextAreaElement)input).name == "isyeri_guvenlik")
                                    {
                                        inputcaptcha = ((HTMLTextAreaElement)input);
                                    }
                                }


                                foreach (var btn in buttons)
                                {
                                    if (((DispHTMLButtonElement)btn).className == "button button-block")
                                    {
                                        btnsubmit = ((DispHTMLButtonElement)btn);

                                        break;
                                    }
                                }

                                if (btnsubmit != null && Program.OtomatikGuvenlikKoduGirilecekIsverenSistemi)
                                {
                                    btnsubmit.click();
                                }
                                else
                                {
                                    inputcaptcha.focus();
                                }

                            }
                        }
                        catch { }
                    }
                    //else if (link.Equals("Yersiz Teşvik"))
                    //{
                    //    SHDocVw.InternetExplorer ie = null;

                    //yenidenDene:
                    //    try
                    //    {

                    //        ie = new SHDocVw.InternetExplorer();
                    //    }
                    //    catch
                    //    {
                    //        Thread.Sleep(2000);

                    //        goto yenidenDene;
                    //    }

                    //    try
                    //    {

                    //        IWebBrowser2 wb = (IWebBrowser2)ie.Application;

                    //        Metodlar.ShowWindow((IntPtr)ie.HWND, Sabitler.SW_MAXIMISE);

                    //        wb.Visible = true;

                    //        wb.Navigate("https://uyg.sgk.gov.tr/TesvikYersizFaydalanma/");

                    //        while (wb.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE) { System.Threading.Thread.Sleep(100); }
                    //        var document = wb.Document;


                    //        if (ie != null)
                    //        {

                    //            try
                    //            {

                    //                var inputs = ((HTMLDocument)ie.Document).getElementsByTagName("input");

                    //                HTMLTextAreaElement inputcaptcha = null;

                    //                foreach (var input in inputs)
                    //                {
                    //                    if (((HTMLTextAreaElement)input).id == "userLogin_basvuru_tcKimlikNo") ((HTMLTextAreaElement)input).value = KullaniciAdi;
                    //                    else if (((HTMLTextAreaElement)input).id == "userLogin_basvuru_isyeriKodu") ((HTMLTextAreaElement)input).value = KullaniciKod;
                    //                    else if (((HTMLTextAreaElement)input).id == "userLogin_basvuru_sistemSifre") ((HTMLTextAreaElement)input).value = SistemSifresi;
                    //                    else if (((HTMLTextAreaElement)input).id == "userLogin_basvuru_isyeriSifre") ((HTMLTextAreaElement)input).value = IsyeriSifresi;
                    //                    else if (((HTMLTextAreaElement)input).id == "userLogin_captchaStrLogin")
                    //                    {
                    //                        inputcaptcha = ((HTMLTextAreaElement)input);
                    //                    }
                    //                }

                    //                inputcaptcha.focus();
                    //            }
                    //            catch { }
                    //        }
                    //    }
                    //    catch { }
                    //}
                    else if (link.Equals("6645") || link.Equals("687") || link.Equals("14857"))
                    {
                        SHDocVw.InternetExplorer ie = null;

                    yenidenDene:
                        try
                        {

                            ie = new SHDocVw.InternetExplorer();

                        }
                        catch
                        {
                            Thread.Sleep(2000);
                            goto yenidenDene;
                        }

                        try
                        {

                            IWebBrowser2 wb = (IWebBrowser2)ie.Application;

                            Metodlar.ShowWindow((IntPtr)ie.HWND, Sabitler.SW_MAXIMISE);

                            wb.Visible = true;


                            if (link.Equals("6645"))
                            {
                                wb.Navigate("https://uyg.sgk.gov.tr/Sigortali_Tesvik_4447_15/login.jsp");
                            }
                            else if (link.Equals("687"))
                            {
                                wb.Navigate("https://uyg.sgk.gov.tr/Sigortali_Tesvik_687_KHK/login.jsp");
                            }
                            else if (link.Equals("14857"))
                            {
                                wb.Navigate("https://uyg.sgk.gov.tr/Sigortali_Tesvik_4a/login.jsp");
                            }

                            while (wb.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE) { System.Threading.Thread.Sleep(100); }
                            var document = wb.Document;


                            if (ie != null)
                            {

                                var inputs = ((HTMLDocument)ie.Document).getElementsByTagName("input");

                                var buttons = ((HTMLDocument)ie.Document).getElementsByTagName("button");

                                HTMLTextAreaElement inputcaptcha = null;

                                HTMLButtonElement btnsubmit = null;

                                bool captchaOtomatikGirilecek = (link.Equals("6645") && Program.OtomatikGuvenlikKoduGirilecek6645) || (link.Equals("687") && Program.OtomatikGuvenlikKoduGirilecek687) || (link.Equals("14857") && Program.OtomatikGuvenlikKoduGirilecek14857);


                                foreach (var input in inputs)
                                {
                                    if (((HTMLTextAreaElement)input).name == "j_username") ((HTMLTextAreaElement)input).value = KullaniciAdi;
                                    else if (((HTMLTextAreaElement)input).name == "isyeri_kod") ((HTMLTextAreaElement)input).value = KullaniciKod;
                                    else if (((HTMLTextAreaElement)input).name == "j_password") ((HTMLTextAreaElement)input).value = SistemSifresi;
                                    else if (((HTMLTextAreaElement)input).name == "isyeri_sifre") ((HTMLTextAreaElement)input).value = IsyeriSifresi;
                                    else if (((HTMLTextAreaElement)input).value == "TAMAM") btnsubmit = (HTMLButtonElement)input;
                                    else if (((HTMLTextAreaElement)input).name == "captcha_image")
                                    {
                                        inputcaptcha = ((HTMLTextAreaElement)input);

                                        if (captchaOtomatikGirilecek)
                                        {
                                            inputcaptcha.value = "11111";
                                        }
                                    }

                                }

                                if (btnsubmit != null && captchaOtomatikGirilecek)
                                {
                                    btnsubmit.click();
                                }
                                else
                                {
                                    if (inputcaptcha != null) inputcaptcha.focus();
                                }

                            }
                        }
                        catch { }
                    }
                    else if (link.Equals("Vizite"))
                    {
                        SHDocVw.InternetExplorer ie = null;

                    yenidenDene:
                        try
                        {

                            ie = new SHDocVw.InternetExplorer();

                        }
                        catch
                        {
                            Thread.Sleep(2000);
                            goto yenidenDene;
                        }

                        try
                        {

                            IWebBrowser2 wb = (IWebBrowser2)ie.Application;

                            Metodlar.ShowWindow((IntPtr)ie.HWND, Sabitler.SW_MAXIMISE);

                            wb.Visible = true;

                            wb.Navigate("https://uyg.sgk.gov.tr/vizite/welcome.do");


                            while (wb.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE) { System.Threading.Thread.Sleep(100); }
                            var document = wb.Document;


                            if (ie != null)
                            {

                                var inputs = ((HTMLDocument)ie.Document).getElementsByTagName("input");

                                HTMLTextAreaElement inputcaptcha = null;

                                foreach (var input in inputs)
                                {
                                    if (((HTMLTextAreaElement)input).name == "kullaniciAdi") ((HTMLTextAreaElement)input).value = KullaniciAdi;
                                    else if (((HTMLTextAreaElement)input).name == "isyeriKodu") ((HTMLTextAreaElement)input).value = KullaniciKod;
                                    else if (((HTMLTextAreaElement)input).name == "isyeriSifresi") ((HTMLTextAreaElement)input).value = IsyeriSifresi;
                                    else if (((HTMLTextAreaElement)input).name == "guvenlikKodu")
                                    {
                                        inputcaptcha = ((HTMLTextAreaElement)input);
                                    }
                                }
                                
                                 if (inputcaptcha != null) inputcaptcha.focus();

                            }
                        }
                        catch { }
                    }
                    else if (link.Equals("Aylık Çalışan Sayıları"))
                    {
                        using (var dbContext = new DbEntities())
                        {

                            var isyeri2 = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriID)).Include(p => p.AylikCalisanSayilari).FirstOrDefault();

                            if (isyeri2.AylikCalisanSayilari.Count > 0)
                            {
                                var formDataGoster = new frmDataGoster();
                                formDataGoster.Text = "Aylık Çalışan Sayıları";
                                formDataGoster.Width = 400;

                                var data = isyeri2.AylikCalisanSayilari.Select(p => new AylikCalisanDTO { Donem = p.DonemYil + "/" + p.DonemAy, AylikCalisan = String.Format("Taşeronlu={0} , Taşeronsuz={1}", p.CalisanSayisiTaseronlu, p.CalisanSayisiTaseronsuz != -1 ? p.CalisanSayisiTaseronsuz.ToString() : "YOK") }).OrderByDescending(p => Convert.ToDateTime(p.Donem)).ToList();

                                formDataGoster.dgvData.ColumnHeadersVisible = false;

                                formDataGoster.dgvData.DataSource = data;

                                formDataGoster.dgvData.Columns.Cast<DataGridViewColumn>().ToList().ForEach(p => p.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells);

                                formDataGoster.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("Aylık çalışan sayıları mevcut değil", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    else if (link.Equals("Asgari Ücret Destek Tutarları"))
                    {
                        using (var dbContext = new DbEntities())
                        {

                            var isyeri2 = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriID)).Include(p => p.AsgariUcretDestekTutarlari).FirstOrDefault();

                            if (isyeri2.AsgariUcretDestekTutarlari.Count > 0)
                            {
                                var formDataGoster = new frmDataGoster { Text = "Asgari Ücret Destek Tutarları", Width = 400 };

                                var data = isyeri2.AsgariUcretDestekTutarlari.Select(p => new AsgariUcretDTO { Donem = p.DonemYil + "/" + p.DonemAy, Gun = p.HesaplananGun.ToString() }).OrderByDescending(p => Convert.ToDateTime(p.Donem)).ToList();

                                formDataGoster.dgvData.ColumnHeadersVisible = false;

                                formDataGoster.dgvData.DataSource = data;

                                formDataGoster.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("Asgari ücret destek tutarı kaydı yok", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    else if (link.Equals("Başvurular"))
                    {
                        using (var dbContext = new DbEntities())
                        {

                            var isyeri2 = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriID)).Include(p => p.BasvuruDonemleri).FirstOrDefault();

                            if (isyeri2.BasvuruDonemleri.Count > 0)
                            {
                                var formDataGoster = new frmDataGoster { Text = "Başvurular", Width = 500 };

                                var data = isyeri2.BasvuruDonemleri.Select(p => new BasvuruDonemDTO { Tarih = p.BasvuruDonem, Aylar = !string.IsNullOrEmpty(p.Aylar) ? String.Join(" - ", p.Aylar.Split(',')) : null }).OrderByDescending(p => Convert.ToDateTime(p.Tarih)).ToList();

                                formDataGoster.dgvData.ColumnHeadersVisible = false;

                                formDataGoster.dgvData.DataSource = data;

                                formDataGoster.dgvData.Columns.Cast<DataGridViewColumn>().ToList().ForEach(p => p.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells);

                                formDataGoster.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("Başvuru kaydı yok", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }



                    }
                    else if (link.Equals("Borçlu Aylar"))
                    {
                        using (var dbContext = new DbEntities())
                        {
                            var isyeri2 = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriID)).Include(p => p.BorcluAylar).FirstOrDefault();

                            if (isyeri2.BorcluAylar.Count > 0)
                            {
                                var formDataGoster = new frmDataGoster();
                                formDataGoster.Text = "Borçlu Aylar";
                                formDataGoster.Width = 300;

                                var data = isyeri2.BorcluAylar.Select(p => new BorcluAyDTO { Donem = p.BorcluAy }).OrderByDescending(p => Convert.ToDateTime(p.Donem)).ToList();

                                formDataGoster.dgvData.ColumnHeadersVisible = false;

                                formDataGoster.dgvData.DataSource = data;

                                formDataGoster.ShowDialog();
                            }
                            else MessageBox.Show("Borçlu ay bilgisi yok", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else if (link.Equals("Klasör Aç"))
                    {
                        Process.Start(Metodlar.IsyeriKlasorBul(isyeri, true));
                    }
                    else if (link.Equals("Cari Aphb Aç"))
                    {
                        using (var dbContext = new DbEntities())
                        {
                            var isyeriId = Convert.ToInt64(isyeri.ID);

                            var isyeriDb = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriID)).Include(p => p.Sirketler).FirstOrDefault();

                            string cariAphb = Metodlar.FormBul(isyeriDb, Enums.FormTuru.CariAphb);

                            bool devam = true;

                            if (cariAphb == null)
                            {
                                devam = MessageBox.Show("Cari Aphb bulunamadı. Cari Aphb oluşturulsun mu?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                            }
                            else
                            {
                                var splits = System.IO.Path.GetFileNameWithoutExtension(cariAphb).Split('-');
                                var tarih = new DateTime(splits[1].ToInt(), splits[2].ToInt(), 1);
                                var cariTarih = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);

                                if (tarih < cariTarih)
                                {
                                    devam = MessageBox.Show(String.Format("Cari Aphb bulundu fakat {0} ayına ait. Cari Aphb güncellenerek oluşturulsun mu?", tarih.Year.ToString() + "-" + tarih.Month.ToString()), "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                                }
                                else
                                {
                                    devam = false;
                                }

                                if (!devam) Process.Start(cariAphb);
                            }

                            if (devam)
                            {
                                var aphb = Metodlar.FormBul(isyeri, Enums.FormTuru.Aphb);

                                DataTable dtAphb = null;

                                if (aphb == null)
                                {
                                    devam = MessageBox.Show("Aphb dosyası bulunamadı. Aphb dosyası olmadan yeni tescil edilmiş işyeri olarak devam edilsin mi?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

                                    if (devam) dtAphb = Metodlar.ReadExcelFile(System.IO.Path.Combine(Application.StartupPath, "ListeTemplate.xlsx"), MesajGostersin: false).Tables[0];
                                }
                                else
                                {
                                    Cursor.Current = Cursors.WaitCursor;

                                    dtAphb = Metodlar.ReadExcelFile(aphb).Tables[0];
                                }

                                if (devam)
                                {
                                    Cursor.Current = Cursors.WaitCursor;

                                    DataTable dtcari = null;

                                    var sonuc = Metodlar.CariDonemKisileriAPHByeEkle(isyeriDb, ref dtAphb, out dtcari);

                                    Cursor.Current = Cursors.Default;

                                    if (sonuc.Equals("OK"))
                                    {
                                        cariAphb = Metodlar.FormBul(isyeriDb, Enums.FormTuru.CariAphb);

                                        if (cariAphb != null)
                                        {
                                            Process.Start(cariAphb);
                                        }
                                    }
                                    else if (sonuc.Equals("Cari aya ait onaylı bildirgeler mevcut olduğu için Cari Aphb oluşturulmadı"))
                                    {
                                        MessageBox.Show(sonuc, "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                    }
                                    else MessageBox.Show(sonuc, "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }

                            }

                        }


                    }
                    else if (link.Equals("Cari Kişiler Aç"))
                    {
                        using (var dbContext = new DbEntities())
                        {
                            var isyeriId = Convert.ToInt64(isyeri.ID);

                            var isyeriDb = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriId)).Include(p => p.Sirketler).FirstOrDefault();

                            string cariKisiler = Metodlar.FormBul(isyeriDb, Enums.FormTuru.Kisiler);

                            bool devam = true;

                            if (cariKisiler == null)
                            {
                                devam = MessageBox.Show("Cari Kişiler dosyası bulunamadı. Cari Kişiler oluşturulsun mu?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                            }
                            else
                            {
                                var splits = System.IO.Path.GetFileNameWithoutExtension(cariKisiler).Split('-');
                                var tarih = new DateTime(splits[1].ToInt(), splits[2].ToInt(), 1);
                                var cariTarih = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);

                                if (tarih < cariTarih)
                                {
                                    devam = MessageBox.Show(String.Format("Cari Kişiler dosyası bulundu fakat {0} ayına ait. Cari Kişiler dosyası güncellenerek oluşturulsun mu?", tarih.Year.ToString() + "-" + tarih.Month.ToString()), "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                                }
                                else
                                {
                                    devam = false;
                                }

                                if (!devam) Process.Start(cariKisiler);
                            }

                            if (devam)
                            {
                                var aphb = Metodlar.FormBul(isyeri, Enums.FormTuru.Aphb);

                                DataTable dtAphb = null;

                                if (aphb == null)
                                {
                                    devam = MessageBox.Show("Aphb dosyası bulunamadı. Aphb dosyası olmadan yeni tescil edilmiş işyeri olarak devam edilsin mi?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

                                    if (devam) dtAphb = Metodlar.ReadExcelFile(System.IO.Path.Combine(Application.StartupPath, "ListeTemplate.xlsx"), MesajGostersin: false).Tables[0];
                                }
                                else
                                {
                                    Cursor.Current = Cursors.WaitCursor;

                                    dtAphb = Metodlar.ReadExcelFile(aphb).Tables[0];
                                }

                                if (devam)
                                {
                                    Cursor.Current = Cursors.WaitCursor;

                                    DataTable dtcari = null;

                                    var sonuc = Metodlar.CariDonemKisileriAPHByeEkle(isyeriDb, ref dtAphb, out dtcari);

                                    Cursor.Current = Cursors.Default;

                                    if (sonuc.Equals("OK"))
                                    {
                                        cariKisiler = Metodlar.FormBul(isyeriDb, Enums.FormTuru.Kisiler);

                                        if (cariKisiler != null)
                                        {
                                            Process.Start(cariKisiler);
                                        }
                                    }
                                    else if (sonuc.Equals("Cari aya ait onaylı bildirgeler mevcut olduğu için Cari Kişiler dosyası oluşturulmadı"))
                                    {
                                        MessageBox.Show(sonuc, "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                    }
                                    else MessageBox.Show(sonuc, "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else MessageBox.Show("Aphb dosyası bulunamadı. Cari Kişiler dosyasının otomatik oluşturulması için Aphb dosyası mevcut olmalıdır. Lütfen güncel Aphb dosyasını yükledikten sonra tekrar deneyiniz", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                        }

                    }
                    else if (link.Equals("Cari Kişileri Sil"))
                    {

                        string cariKisiler = Metodlar.FormBul(isyeri, Enums.FormTuru.Kisiler);

                        if (cariKisiler == null)
                        {
                            MessageBox.Show("Cari Kişiler dosyası bulunamadı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            try
                            {
                                File.Delete(cariKisiler);

                                MessageBox.Show("Cari Kişiler dosyası başarıyla silindi", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Cari Kişiler dosyası silinirken hata meydana geldi." + Environment.NewLine + Environment.NewLine + "Hata Mesajı: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                        }
                    }
                    else if (link.Equals("Cari Aphb Sil"))
                    {

                        string cariAphb = Metodlar.FormBul(isyeri, Enums.FormTuru.CariAphb);

                        if (cariAphb == null)
                        {
                            MessageBox.Show("Cari Aphb dosyası bulunamadı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            try
                            {
                                File.Delete(cariAphb);

                                MessageBox.Show("Cari Aphb dosyası başarıyla silindi", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Cari Aphb dosyası silinirken hata meydana geldi." + Environment.NewLine + Environment.NewLine + "Hata Mesajı: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                        }
                    }
                    else if (link.Equals("Eğitim Belgesi Verilecekler"))
                    {
                        var egitimBelgesiVerilecekler = Metodlar.FormBul(isyeri, Enums.FormTuru.EgitimListesi);

                        if (egitimBelgesiVerilecekler != null) Process.Start(egitimBelgesiVerilecekler);
                        else MessageBox.Show("Eğitim belgesi verilecekler listesi bulunamadı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (link.Equals("Emanet Tahsilatları"))
                    {
                        using (var dbContext = new DbEntities())
                        {
                            var isyeriId = Convert.ToInt64(isyeri.ID);

                            var isyeriDb = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriID)).Include(p => p.Sirketler).FirstOrDefault();

                            ProjeGiris projeGiris = null;

                            Cursor = Cursors.WaitCursor;

                            var sonuc = Metodlar.SistemdenEmanetTahsilatlariniCek(isyeriDb, ref projeGiris);

                            Cursor = Cursors.Default;

                            if (sonuc != null)
                            {
                                if (sonuc.Equals("Emanet tahsilat kaydı bulunamadı") || sonuc.Equals("3 denemeye rağmen bilgiler çekilemedi"))
                                {
                                    MessageBox.Show(sonuc, "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else Process.Start(sonuc);
                            }
                        }


                    }
                    else if (link.Equals("Müfredat Kartı"))
                    {
                        using (var dbContext = new DbEntities())
                        {
                            var isyeriId = Convert.ToInt64(isyeri.ID);

                            var isyeriDb = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriID)).Include(p => p.Sirketler).FirstOrDefault();

                            ProjeGiris projeGiris = null;

                            Cursor = Cursors.WaitCursor;

                            var sonuc = Metodlar.SistemdenMufredatKartiCek(isyeriDb, ref projeGiris);

                            Cursor = Cursors.Default;

                            if (sonuc.Durum == false)
                            {
                                MessageBox.Show(sonuc.HataMesaji, "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                var isyeriKlasor = Metodlar.IsyeriKlasorBul(isyeri);

                                if (isyeriKlasor != null)
                                {

                                    var icmaller = Directory.GetFiles(isyeriKlasor, "Bildirgelerin İcmali*.xlsx");

                                    var bildirgeicmal = icmaller.Select(p => new FileInfo(p)).Where(p => DateTime.Now.Subtract(p.LastWriteTime).TotalMinutes <= 60).OrderBy(p => p.LastWriteTime).FirstOrDefault();

                                    if (bildirgeicmal != null) Process.Start(bildirgeicmal.FullName);
                                }

                                Process.Start(sonuc.Result);
                            }
                        }
                    }
                    else if (link.Equals("Bildirgelerin İcmali"))
                    {
                        using (var dbContext = new DbEntities())
                        {
                            var isyeriId = Convert.ToInt64(isyeri.ID);

                            var isyeriDb = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriID)).Include(p => p.Sirketler).FirstOrDefault();

                            ProjeGiris projeGiris = null;

                            Cursor = Cursors.WaitCursor;

                            var sonuc = Metodlar.SistemdenBildirgelerinIcmaliniCek(isyeriDb, projeGiris);

                            Cursor = Cursors.Default;

                            if (sonuc.Durum == false)
                            {
                                MessageBox.Show(sonuc.HataMesaji, "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else Process.Start(sonuc.Result);
                        }
                    }
                    else if (link.Equals("Teşvik Başvurusu Yap"))
                    {
                        using (var dbContext = new DbEntities())
                        {
                            var isyeriId = Convert.ToInt64(isyeri.ID);

                            var isyeriDb = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriID)).Include(p => p.Sirketler).FirstOrDefault();

                            Cursor = Cursors.WaitCursor;

                            var sonuc = Metodlar.BasvuruYap(isyeriDb);

                            Cursor = Cursors.Default;

                            if (sonuc != "OK")
                            {
                                MessageBox.Show("Başvuru yapılamadı. Açıklama:" + sonuc, "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else MessageBox.Show("Başvuru başarıyla yapıldı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else if (link.Equals("Kısa Çalışma"))
                    {
                        using (var dbContext = new DbEntities())
                        {
                            var isyeriId = Convert.ToInt64(isyeri.ID);

                            var isyeriDb = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriID)).Include(p => p.Sirketler).FirstOrDefault();

                            var aphb = Metodlar.FormBul(isyeriDb, Enums.FormTuru.Aphb);
                            var bf = Metodlar.FormBul(isyeriDb, Enums.FormTuru.BasvuruFormu);

                            var hataMesaji = "";

                            if (aphb == null) hataMesaji += "Aphb dosyası bulunamadı" + Environment.NewLine;
                            else
                            {
                                if (DateTime.Now.Subtract(new FileInfo(aphb).LastWriteTime).TotalHours >= 24)
                                    hataMesaji += "Aphb dosyası güncel değil" + Environment.NewLine;
                            }


                            if (bf == null) hataMesaji += "Başvuru formu bulunamadı" + Environment.NewLine;
                            else
                            {
                                if (DateTime.Now.Subtract(new FileInfo(bf).LastWriteTime).TotalHours >= 24)
                                    hataMesaji += "Başvuru formu güncel değil" + Environment.NewLine;
                            }

                            bool devam = true;

                            if (aphb == null)
                            {
                                devam = false;
                                MessageBox.Show(hataMesaji, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(hataMesaji))
                                {
                                    hataMesaji += Environment.NewLine + "Devam etmek istiyor musunuz?";

                                    devam = MessageBox.Show(hataMesaji, "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                                }
                            }


                            if (devam)
                            {
                                Cursor = Cursors.WaitCursor;

                                var sonuc = Metodlar.KisaCalismaListesiOlustur(isyeriDb);

                                Cursor = Cursors.Default;

                                if (sonuc.Durum)
                                {
                                    if (!string.IsNullOrEmpty(sonuc.Result)) Process.Start(sonuc.Result);
                                }
                                else
                                {
                                    MessageBox.Show("Kısa çalışma listesi oluşturulamadı. Hata Mesajı:" + sonuc.HataMesaji, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                        }
                    }
                    else if (link.Equals("Aphb İcmal"))
                    {
                        using (var dbContext = new DbEntities())
                        {
                            var isyeriId = Convert.ToInt64(isyeri.ID);

                            var isyeriDb = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriID)).Include(p => p.Sirketler).FirstOrDefault();

                            Cursor = Cursors.WaitCursor;

                            var sonuc = Metodlar.AphbIcmalOlustur(isyeriDb);

                            Cursor = Cursors.Default;

                            if (sonuc.Durum == false)
                            {
                                MessageBox.Show(sonuc.HataMesaji, "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else Process.Start(sonuc.Result);
                        }
                    }
                    else if (link.Equals("7252 Yersiz"))
                    {
                        using (var dbContext = new DbEntities())
                        {
                            var isyeriId = Convert.ToInt64(isyeri.ID);

                            var isyeriDb = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriID)).Include(p => p.Sirketler).FirstOrDefault();

                            Cursor = Cursors.WaitCursor;

                            var sonuc = Metodlar.Yersiz7252Olustur(isyeriDb);

                            Cursor = Cursors.Default;

                            if (sonuc.Durum == false)
                            {
                                MessageBox.Show(sonuc.HataMesaji, "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else Process.Start(sonuc.Result);
                        }
                    }
                    else if (link.Equals("Muhtasar Onay Bekleyenler"))
                    {
                        using (var dbContext = new DbEntities())
                        {
                            var isyeriId = Convert.ToInt64(isyeri.ID);

                            var isyeriDb = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriID)).Include(p => p.Sirketler).FirstOrDefault();

                            ProjeGiris projeGiris = null;

                            Cursor = Cursors.WaitCursor;

                            var sonuc = Metodlar.MuhtasarOnayBekleyenleriCek(isyeriDb, projeGiris);

                            Cursor = Cursors.Default;

                            if (sonuc.Durum == false)
                            {
                                MessageBox.Show(sonuc.HataMesaji, "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else Process.Start(sonuc.Result);
                        }
                    }
                }
                else if (
                    dgvIsyerleri.Columns[e.ColumnIndex].Name == "colGecmisHesapla" ||
                    dgvIsyerleri.Columns[e.ColumnIndex].Name == "colCariHesapla" ||
                    dgvIsyerleri.Columns[e.ColumnIndex].Name == "colGecmisEski" ||
                    dgvIsyerleri.Columns[e.ColumnIndex].Name == "colFaraziGecmisHesapla")
                {
                    var frmBildirgeOlustur = new frmBildirgeOlustur(
                        dgvIsyerleri.Columns[e.ColumnIndex].Name == "colCariHesapla",
                        dgvIsyerleri.Columns[e.ColumnIndex].Name == "colFaraziGecmisHesapla",
                        dgvIsyerleri.Columns[e.ColumnIndex].Name == "colGecmisEski"
                        );

                    using (var dbcontext = new DbEntities())
                    {
                        frmBildirgeOlustur.Isyeri = dbcontext.Isyerleri
                        .Include(p => p.Sirketler)
                        .Include(p => p.AylikCalisanSayilari)
                        .Include(p => p.AsgariUcretDestekTutarlari)
                        .Include(p => p.BasvuruDonemleri)
                        .Include(p => p.BorcluAylar)
                        .Where(p => p.IsyeriID.Equals(isyeriID)).FirstOrDefault();

                    }

                    frmBildirgeOlustur.Text = string.Format("{0} - {1}", frmBildirgeOlustur.Isyeri.Sirketler.SirketAdi, frmBildirgeOlustur.Isyeri.SubeAdi);

                    frmBildirgeOlustur.ShowDialog();
                }
            }
        }
        public void IndirmeBitti(Indir indir)
        {
            if (Program.IndirilenIsyerleri.ContainsKey(indir.IsyeriId))
            {
                var indirilenisyeri = Program.IndirilenIsyerleri[indir.IsyeriId];

                var baslatilmayanIndirmeler = new List<Indir>();
                baslatilmayanIndirmeler.AddRange(indirilenisyeri.AphbIndirmeleri.Where(p => p.IndirmeSonucu.Baslatildi == false && p.IndirmeSonucu.IptalEdildi == false));
                baslatilmayanIndirmeler.AddRange(indirilenisyeri.BasvuruFormuIndirmeleri.Where(p => p.IndirmeSonucu.Baslatildi == false && p.IndirmeSonucu.IptalEdildi == false));

                if (baslatilmayanIndirmeler.Count == 0)
                {
                    if (indirilenisyeri.AphbIndirmeleri.All(p => p.IndirmeSonucu.Tamamlandi) && indirilenisyeri.BasvuruFormuIndirmeleri.All(p => p.IndirmeSonucu.Tamamlandi))
                    {

                        var isyerleriDoldur = indirilenisyeri.AphbIndirmeleri.Any(p => p.IndirmeSonucu.Basarili)
                            ||
                         indirilenisyeri.BasvuruFormuIndirmeleri.Any(p => p.IndirmeSonucu.Basarili);

                        Program.IndirilenIsyerleri.Remove(indir.IsyeriId);

                        IsyerleriDoldur();
                    }

                }
                else
                {
                    if (indirilenisyeri.AphbIndirmeleri.Count > 0 && indirilenisyeri.AphbIndirmeleri.Any(p => p.IndirmeSonucu.Tamamlandi && (p.IndirmeSonucu.IptalEdildi || p.IndirmeSonucu.HataVar)))
                    {
                        foreach (var indirme in baslatilmayanIndirmeler)
                        {
                            if (indirme is BasvuruFormuIndir)
                            {
                                var bfindir = (BasvuruFormuIndir)indirme;

                                if (bfindir.IndirmeSonucu.IptalEdildi == false)
                                {

                                    bfindir.IslemiIptalEt();
                                    bfindir.IndirmeSonucu.Tamamlandi = true;
                                    bfindir.IndirmeSonucu.IptalEdildi = true;
                                }

                            }
                            else if (indirme is AphbIndir)
                            {
                                var aphbindir = (AphbIndir)indirme;

                                if (aphbindir.IndirmeSonucu.IptalEdildi == false)
                                {
                                    aphbindir.Cancel();
                                    aphbindir.IndirmeSonucu.Tamamlandi = true;
                                    aphbindir.IndirmeSonucu.IptalEdildi = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (indirilenisyeri.AphbIndirmeleri.Count > 0)
                        {
                            using (var dbContext = new DbEntities())
                            {
                                var isyeri = dbContext.Isyerleri.Include(p => p.Sirketler).Where(p => p.IsyeriID.Equals(indirilenisyeri.isyeri.IsyeriID)).FirstOrDefault();

                                var aphbyol = Metodlar.FormBul(isyeri, Enums.FormTuru.Aphb);

                                if (aphbyol != null)
                                {
                                    var dtAphb = Metodlar.AylikListeyiYukle(aphbyol, false);

                                    foreach (var indirme in baslatilmayanIndirmeler)
                                    {
                                        if (indirme is BasvuruFormuIndir)
                                        {
                                            var bfindir = (BasvuruFormuIndir)indirme;

                                            bfindir.SuanYapilanIsyeriBasvuru = isyeri;
                                            bfindir.dtMevcutAphb = dtAphb;

                                        }
                                    }

                                }

                            }

                        }


                    }

                    foreach (var indirme in baslatilmayanIndirmeler)
                    {
                        _ = indirme.Calistir();
                    }
                }

                if (indirilenisyeri.formIndirmeEkrani.IsDisposed == false)
                {
                    indirilenisyeri.formIndirmeEkrani.Guncelle(indirilenisyeri);
                }
            }
        }

        private void btnIsyeriEkle_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == new frmIsyeriEkle().ShowDialog())
            {
                dr = DialogResult.OK;

                IsyerleriDoldur();

            }
        }

        private void frmIsyerleri_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.IndirilenIsyerleri.Count > 0)
            {
                var indirilenIsyerleriAdlari = String.Join(Environment.NewLine, Program.IndirilenIsyerleri.Select(p => String.Format("{0} - {1}{2}", p.Value.isyeri.Sirketler.SirketAdi, p.Value.isyeri.SubeAdi, Environment.NewLine)));

                var iptaledilsin = MessageBox.Show(indirilenIsyerleriAdlari + Environment.NewLine + "Form indirilmesi devam eden işyerleri var. Bunları iptal etmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes;

                if (iptaledilsin)
                {
                    var iptaledilecekler = Program.IndirilenIsyerleri.ToArray();

                    foreach (var item in iptaledilecekler)
                    {
                        item.Value.TumunuIptalEt();
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }

            DialogResult = dr;
        }

        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            if (txtAra.Text.Length >= 3)
            {
                Ara = true;

                this.IsyerleriDoldur(false);
            }
            else
            {
                if (Ara)
                {
                    Ara = false;

                    this.IsyerleriDoldur(false);
                }

                Ara = false;
            }
        }

        private void sagTusAphb_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Sil")
            {
                var rowIndex = Convert.ToInt32(sagTusAphb.Tag);

                var aphb = dgvIsyerleri.Rows[rowIndex].Cells["colAphb"].Value.ToString();

                if (!String.IsNullOrEmpty(aphb))
                {
                    var dbrow = dgvIsyerleri.Rows[rowIndex].DataBoundItem as IsyeriListesiDTO;

                    string aphbyol = Metodlar.FormBul(dbrow, Enums.FormTuru.Aphb);

                    bool Kaydet = false;

                    if (!String.IsNullOrEmpty(aphbyol))
                    {
                        try
                        {
                            File.Delete(aphbyol);

                            Kaydet = true;

                        }
                        catch (Exception ex)
                        {
                            Metodlar.HataMesajiGoster(ex, "Aphb siinemedi hata meydana geldi");
                        }
                    }
                    else
                    {
                        Kaydet = true;
                    }

                    if (Kaydet)
                    {
                        using (var dbContext = new DbEntities())
                        {
                            var isyeri = dbContext.Isyerleri.Find(Convert.ToInt64(dbrow.ID));

                            isyeri.Aphb = null;

                            dbContext.SaveChanges();
                        }

                        dgvIsyerleri.Rows[rowIndex].Cells["colAphb"].Value = string.Empty;

                        dr = DialogResult.OK;

                    }
                }
            }
            else if (e.ClickedItem.Text == "Klasörden Güncelle")
            {
                var rowIndex = Convert.ToInt32(sagTusAphb.Tag);

                var dbrow = dgvIsyerleri.Rows[rowIndex].DataBoundItem as IsyeriListesiDTO;

                var isyeripath = Metodlar.IsyeriKlasorBul(dbrow);

                if (isyeripath != null)
                {
                    var files = Directory.GetFiles(isyeripath);

                    var fileaphb = files.FirstOrDefault(p => System.IO.Path.GetFileName(p).ToLower().Contains("aphb"));

                    if (fileaphb != null)
                    {
                        var aphbAdi = System.IO.Path.GetFileName(fileaphb);

                        using (var dbContext = new DbEntities())
                        {
                            var isyeri = dbContext.Isyerleri.Find(Convert.ToInt64(dbrow.ID));

                            isyeri.Aphb = aphbAdi;

                            dbContext.SaveChanges();
                        }


                        dgvIsyerleri.Rows[rowIndex].Cells["colAphb"].Value = aphbAdi;

                        dr = DialogResult.OK;

                        MessageBox.Show("İşyeri klasöründe Aphb dosyası bulunarak başarıyla güncellendi", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        MessageBox.Show("İşyeri klasöründe Aphb dosyası bulunamadı", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }

            }
            else if (e.ClickedItem.Text == "Yükle")
            {
                if (ofdAphbYukle.ShowDialog() == DialogResult.OK)
                {
                    var rowIndex = Convert.ToInt32(sagTusAphb.Tag);

                    var dbrow = dgvIsyerleri.Rows[rowIndex].DataBoundItem as IsyeriListesiDTO;

                    using (var dbContext = new DbEntities())
                    {
                        var isyeriID = Convert.ToInt64(dbrow.ID);

                        var isyeri = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriID)).Include(p => p.Sirketler).FirstOrDefault();

                        var sonuc = Metodlar.FormKaydet(isyeri, ofdAphbYukle.FileName, Enums.FormTuru.Aphb);

                        if (sonuc != null)
                        {
                            dgvIsyerleri.Rows[rowIndex].Cells["colAphb"].Value = System.IO.Path.GetFileName(ofdAphbYukle.FileName);

                            MessageBox.Show("Aphb dosyası başarılı bir şekilde yüklendi", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Aphb dosyası yüklenirken hata meydana geldi", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }


                }
            }

        }

        private void dgvIsyerleri_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (dgvIsyerleri.Columns[e.ColumnIndex].Name == "colAPHB")
                {
                    sagTusAphb.Tag = e.RowIndex;
                }
                else if (dgvIsyerleri.Columns[e.ColumnIndex].Name == "colBasvuruFormu")
                {
                    sagTusBf.Tag = e.RowIndex;
                }
                else if (dgvIsyerleri.Columns[e.ColumnIndex].Name == "colBasvuruListesi7166")
                {
                    sagTus7166.Tag = e.RowIndex;
                }
            }
        }

        private void sagTusBf_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Sil")
            {
                var rowIndex = Convert.ToInt32(sagTusBf.Tag);

                var bf = dgvIsyerleri.Rows[rowIndex].Cells["colBasvuruFormu"].Value.ToString();

                if (!String.IsNullOrEmpty(bf))
                {
                    var dbrow = dgvIsyerleri.Rows[rowIndex].DataBoundItem as IsyeriListesiDTO;

                    string bfyol = Metodlar.FormBul(dbrow, Enums.FormTuru.BasvuruFormu);

                    bool Kaydet = false;

                    if (!String.IsNullOrEmpty(bfyol))
                    {
                        try
                        {
                            File.Delete(bfyol);

                            Kaydet = true;

                        }
                        catch (Exception ex)
                        {
                            Metodlar.HataMesajiGoster(ex, "Başvuru formu siinemedi hata meydana geldi");
                        }
                    }
                    else
                    {
                        Kaydet = true;
                    }

                    if (Kaydet)
                    {

                        using (var dbContext = new DbEntities())
                        {
                            var isyeri = dbContext.Isyerleri.Find(Convert.ToInt64(dbrow.ID));

                            isyeri.BasvuruFormu = null;

                            dbContext.SaveChanges();
                        }

                        dgvIsyerleri.Rows[rowIndex].Cells["colBasvuruFormu"].Value = string.Empty;

                        dr = DialogResult.OK;

                    }
                }
            }
            else if (e.ClickedItem.Text == "Klasörden Güncelle")
            {
                var rowIndex = Convert.ToInt32(sagTusBf.Tag);

                var dbrow = dgvIsyerleri.Rows[rowIndex].DataBoundItem as IsyeriListesiDTO;

                var isyeripath = Metodlar.IsyeriKlasorBul(dbrow);

                if (isyeripath != null)
                {
                    var files = Directory.GetFiles(isyeripath);

                    var fileBf = files.FirstOrDefault(p => (System.IO.Path.GetFileName(p).ToLower().Contains("başvuru") || System.IO.Path.GetFileName(p).ToLower().Contains("basvuru")) && System.IO.Path.GetFileName(p).ToLower().Contains("form"));

                    if (fileBf != null)
                    {
                        var bfAdi = System.IO.Path.GetFileName(fileBf);

                        using (var dbContext = new DbEntities())
                        {
                            var isyeri = dbContext.Isyerleri.Find(Convert.ToInt64(dbrow.ID));

                            isyeri.BasvuruFormu = bfAdi;

                            dbContext.SaveChanges();
                        }

                        dgvIsyerleri.Rows[rowIndex].Cells["colBasvuruFormu"].Value = bfAdi;

                        dr = DialogResult.OK;

                        MessageBox.Show("İşyeri klasöründe başvuru formu bulunarak başarıyla güncellendi", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        MessageBox.Show("İşyeri klasöründe başvuru formu bulunamadı", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }

            }
            else if (e.ClickedItem.Text == "Yükle")
            {
                if (ofdBf.ShowDialog() == DialogResult.OK)
                {
                    var rowIndex = Convert.ToInt32(sagTusBf.Tag);

                    var dbrow = dgvIsyerleri.Rows[rowIndex].DataBoundItem as IsyeriListesiDTO;

                    using (var dbContext = new DbEntities())
                    {
                        var isyeriID = Convert.ToInt64(dbrow.ID);

                        var isyeri = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriID)).Include(p => p.Sirketler).FirstOrDefault();

                        var sonuc = Metodlar.FormKaydet(isyeri, ofdBf.FileName, Enums.FormTuru.BasvuruFormu);

                        if (sonuc != null)
                        {
                            dgvIsyerleri.Rows[rowIndex].Cells["colBasvuruFormu"].Value = System.IO.Path.GetFileName(ofdBf.FileName);

                            MessageBox.Show("Başvuru formu başarılı bir şekilde yüklendi", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Başvuru formu yüklenirken hata meydana geldi", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                }
            }
        }

        private void sagTus7166_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Sil")
            {
                var rowIndex = Convert.ToInt32(sagTus7166.Tag);

                var basvurulistesi7166 = dgvIsyerleri.Rows[rowIndex].Cells["colBasvuruListesi7166"].Value.ToString();

                if (!String.IsNullOrEmpty(basvurulistesi7166))
                {
                    var dbrow = dgvIsyerleri.Rows[rowIndex].DataBoundItem as IsyeriListesiDTO;

                    string basvurulistesi7166Yol = Metodlar.FormBul(dbrow, Enums.FormTuru.BasvuruListesi7166);

                    bool Kaydet = false;

                    if (!String.IsNullOrEmpty(basvurulistesi7166Yol))
                    {
                        try
                        {
                            File.Delete(basvurulistesi7166Yol);

                            Kaydet = true;

                        }
                        catch (Exception ex)
                        {
                            Metodlar.HataMesajiGoster(ex, "7166 Başvuru Listesi silinemedi hata meydana geldi");
                        }
                    }
                    else
                    {
                        Kaydet = true;
                    }

                    if (Kaydet)
                    {

                        using (var dbContext = new DbEntities())
                        {
                            var isyeri = dbContext.Isyerleri.Find(Convert.ToInt64(dbrow.ID));

                            isyeri.BasvuruListesi7166 = null;

                            dbContext.SaveChanges();
                        }

                        dgvIsyerleri.Rows[rowIndex].Cells["colBasvuruListesi7166"].Value = string.Empty;

                        dr = DialogResult.OK;

                    }
                }
            }
            else if (e.ClickedItem.Text == "Klasörden Güncelle")
            {
                var rowIndex = Convert.ToInt32(sagTus7166.Tag);

                var dbrow = dgvIsyerleri.Rows[rowIndex].DataBoundItem as IsyeriListesiDTO;

                var isyeripath = Metodlar.IsyeriKlasorBul(dbrow);

                if (isyeripath != null)
                {
                    var files = Directory.GetFiles(isyeripath);

                    var file7166 = files.FirstOrDefault(p => System.IO.Path.GetFileName(p).ToLower().Contains("7166"));

                    if (file7166 != null)
                    {
                        var Ad7166 = System.IO.Path.GetFileName(file7166);

                        using (var dbContext = new DbEntities())
                        {
                            var isyeri = dbContext.Isyerleri.Find(Convert.ToInt64(dbrow.ID));

                            isyeri.BasvuruListesi7166 = Ad7166;

                            dbContext.SaveChanges();
                        }

                        dgvIsyerleri.Rows[rowIndex].Cells["colBasvuruListesi7166"].Value = Ad7166;

                        dr = DialogResult.OK;

                        MessageBox.Show("İşyeri klasöründe 7166 listesi bulunarak başarıyla güncellendi", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        MessageBox.Show("İşyeri klasöründe 7166 listesi bulunamadı", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }

            }
            else if (e.ClickedItem.Text == "Yükle")
            {
                if (ofd7166.ShowDialog() == DialogResult.OK)
                {
                    var rowIndex = Convert.ToInt32(sagTus7166.Tag);

                    var dbrow = dgvIsyerleri.Rows[rowIndex].DataBoundItem as IsyeriListesiDTO;

                    using (var dbContext = new DbEntities())
                    {
                        var isyeriID = Convert.ToInt64(dbrow.ID);

                        var isyeri = dbContext.Isyerleri.Where(p => p.IsyeriID.Equals(isyeriID)).Include(p => p.Sirketler).FirstOrDefault();

                        var sonuc = Metodlar.FormKaydet(isyeri, ofd7166.FileName, Enums.FormTuru.BasvuruListesi7166);

                        if (sonuc != null)
                        {
                            dgvIsyerleri.Rows[rowIndex].Cells["colBasvuruListesi7166"].Value = System.IO.Path.GetFileName(ofd7166.FileName);

                            MessageBox.Show("7166 Başvuru listesi başarılı bir şekilde yüklendi", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("7166 Başvuru listesi yüklenirken hata meydana geldi", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                }
            }
        }

        private void belgeTürleriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmBelgeTurleri().ShowDialog();
        }

        private void asgariÜcretlerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmAsgariUcretler().ShowDialog();
        }

        private void ayarlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmAyarlar().ShowDialog();
        }

        private void formCariIndirmeAyarlariToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmTarihSecAyarlar(true).ShowDialog();
        }

        private void formGecmisIndirmeAyarlariToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmTarihSecAyarlar(false).ShowDialog();
        }

        private void btnSirketler_Click(object sender, EventArgs e)
        {
            this.formSirketler = new frmSirketler();

            if (this.formSirketler.ShowDialog() == DialogResult.OK)
            {
                this.formSirketler = null;

                this.IsyerleriDoldur();
            }

        }

        private void txtSirketAra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (txtSirketAra.Text.Length >= 3)
                {
                    this.formSirketler = new frmSirketler(txtSirketAra.Text);

                    if (this.formSirketler.ShowDialog() == DialogResult.OK)
                    {
                        this.formSirketler = null;

                        this.IsyerleriDoldur();
                    }
                }
            }
        }

        private void cari14857ListesiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmCari14857Listesi().ShowDialog();
        }

        #endregion

        delegate void delLoglariGuncelle();

        void LoglariGuncelle()
        {
            if (BildirgeYuklemeYapiliyor)
            {
                if (FormLog != null)
                {
                    if (FormLog.lbLog.InvokeRequired)
                    {
                        this.Invoke(new delLoglariGuncelle(LoglariGuncelle));
                    }
                    else
                    {
                        if (FormLog != null)
                        {
                            FormLog.LoglariGuncelle(sb);

                        }
                    }
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
                    File.AppendAllText(Application.StartupPath + "\\log.txt", log);

                    break;
                }
                catch
                {
                    Thread.Sleep(200);
                }
            }

        }

        #region Bildirge Yükle

        Isyerleri SuanYapilanIsyeriBildirgeYukleme = null;
        public bool BildirgeYuklemeYapiliyor = false;
        List<string> YuklenecekBildirgeler = new List<string>();
        Dictionary<int, Thread> BildirgeYuklemeThreads = new Dictionary<int, Thread>();
        string siradakiIslemBildirgeYukleme = "Giriş Yapılacak";
        int seciliYuklenecekBildirge = 0;
        string secilenIptalEdilecekBildirge = null;
        int secilenIptalEdilecekBildirgeSira = 0;
        string secilenIptalEdilecekBildirgeKanunNo = null;
        int secilenIptalEdilecekBildirgeGunSayisi = 0;
        decimal secilenIptalEdilecekBildirgeToplamUcret = 0;
        Dictionary<string, AphbSatir> secilenIptalEdilecekBildirgeKisiler = new Dictionary<string, AphbSatir>();
        string tekrarSecilenIptalBildirgeRefNo = null;
        List<string> HataliSecilenIptalBildirgeleri = new List<string>();
        Bildirge SuanYuklenenBildirge = null;
        Dictionary<string, IptalBildirgeIstatikleri> islemYapilacakIptalBildirgeleri = new Dictionary<string, IptalBildirgeIstatikleri>();

        Dictionary<string, List<string>> HataliKisiler = new Dictionary<string, List<string>>();
        List<string> AylarIslemDevamYeniOlusturListesi = new List<string>();
        Excel2.Application excelBildirgeYukleme = null;
        int yenidenYuklemeDenemeSayisiBildirgeYukleme = 0;
        int basariliEklenenSayisi = 0;
        Dictionary<int, BildirgeYuklemeIcmal> BildirgeIcmaller = new Dictionary<int, BildirgeYuklemeIcmal>();
        Dictionary<int, BildirgeYuklemeIcmal> YuklenenIcmaller = new Dictionary<int, BildirgeYuklemeIcmal>();
        Dictionary<int, Bildirge> YuklenecekBildirgeBilgileri = new Dictionary<int, Bildirge>();
        bool BildirgelereBakildi687Icin = false;
        bool IkiyeBolunecek687TesvikTutari = false;
        Dictionary<string, List<string>> EksikVeyaHataliGirilenBildirgeler = new Dictionary<string, List<string>>();
        string OnaylanacakBildirgeRefNo = null;
        bool EskiOnaydaBekleyenlerSilindi = false;
        Dictionary<string, List<string>> kisilerinSatirNolari = new Dictionary<string, List<string>>();
        bool EkOlarakEklenecek = false;
        bool AsilOlarakEklenecek = false;
        ProjeGiris BildirgeWebClient = null;
        string BildirgeTicket = null;
        StringBuilder sb = new StringBuilder();
        frmLog FormLog = null;
        public Dictionary<int, string> Captchas = new Dictionary<int, string>();
        bool BildirgeYuklemeBasvuruFormuIndirildi = false;
        DataTable BildirgeYuklemeBasvuruForm6111 = null;

        void DegiskenTemizleBildirgeYukleme()
        {
            siradakiIslemBildirgeYukleme = "Giriş Yapılacak";
            SuanYuklenenBildirge = null;
            seciliYuklenecekBildirge = 0;
            secilenIptalEdilecekBildirge = null;
            secilenIptalEdilecekBildirgeSira = 0;
            secilenIptalEdilecekBildirgeGunSayisi = 0;
            secilenIptalEdilecekBildirgeToplamUcret = 0;
            secilenIptalEdilecekBildirgeKisiler = new Dictionary<string, AphbSatir>();
            tekrarSecilenIptalBildirgeRefNo = null;
            HataliSecilenIptalBildirgeleri = new List<string>();
            islemYapilacakIptalBildirgeleri = new Dictionary<string, IptalBildirgeIstatikleri>();
            HataliKisiler = new Dictionary<string, List<string>>();
            AylarIslemDevamYeniOlusturListesi = new List<string>();
            yenidenYuklemeDenemeSayisiBildirgeYukleme = 0;
            basariliEklenenSayisi = 0;
            BildirgeIcmaller = new Dictionary<int, BildirgeYuklemeIcmal>();
            YuklenenIcmaller = new Dictionary<int, BildirgeYuklemeIcmal>();
            YuklenecekBildirgeBilgileri = new Dictionary<int, Bildirge>();
            BildirgelereBakildi687Icin = false;
            IkiyeBolunecek687TesvikTutari = false;
            EksikVeyaHataliGirilenBildirgeler = new Dictionary<string, List<string>>();
            OnaylanacakBildirgeRefNo = null;
            EskiOnaydaBekleyenlerSilindi = false;
            kisilerinSatirNolari = new Dictionary<string, List<string>>();
            EkOlarakEklenecek = false;
            AsilOlarakEklenecek = false;
            BildirgeTicket = null;
            BildirgeYuklemeBasvuruFormuIndirildi = false;
            BildirgeYuklemeBasvuruForm6111 = null;
        }

        public void BildirgeYuklemeyiBaslat(long IsyeriID)
        {

            BildirgeYuklemeYapiliyor = true;

            DegiskenTemizleBildirgeYukleme();

            BildirgeTicket = Guid.NewGuid().ToString();

            BildirgeYuklemeThreads = new Dictionary<int, Thread>();

            Captchas = new Dictionary<int, string>();

            using (var dbContext = new DbEntities())
            {
                SuanYapilanIsyeriBildirgeYukleme = dbContext.Isyerleri.Include("Sirketler").Where(p => p.IsyeriID.Equals(IsyeriID)).FirstOrDefault();
            }

            BildirgeWebClient = new ProjeGiris(SuanYapilanIsyeriBildirgeYukleme, Enums.ProjeTurleri.EBildirgeV2);

            BildirgeWebClient.Ticket = BildirgeTicket;

            sb = new StringBuilder();

            sb.Append("[" + DateTime.Now.ToString() + "] : \"" + SuanYapilanIsyeriBildirgeYukleme.Sirketler.SirketAdi + "\" şirketine ait \"" + SuanYapilanIsyeriBildirgeYukleme.SubeAdi + "\" işyeri için Bildirge yüklenmeye başlanıyor" + Environment.NewLine); new delLoglariGuncelle(LoglariGuncelle).Invoke();

            sb.Append("[" + DateTime.Now.ToString() + "] : " + "Toplam " + YuklenecekBildirgeler.Count + " bildirge bulundu" + Environment.NewLine); new delLoglariGuncelle(LoglariGuncelle).Invoke();

            for (int i = 0; i < YuklenecekBildirgeler.Count; i++)
            {
                BildirgeIcmaller.Add(i, new BildirgeYuklemeIcmal());

                YuklenenIcmaller.Add(i, new BildirgeYuklemeIcmal());

                YuklenecekBildirgeBilgileri.Add(i, new Bildirge());
            }

            //var yuklenecekbildirge = SiradakiBildirgeyeGec();

            //if (yuklenecekbildirge == null)
            //{
            //    BildirgeYuklemeSonaErdi(false);
            //}
            //else
            //{
            for (int i = 0; i < 1; i++)
            {
                Captchas.Add(i, null);

                //Thread thread = new Thread(new ThreadStart(BildirgeYuklemeSayfayiYukle), 10000000);

                Thread thread = new Thread(() =>
                {
                    BildirgeYuklemeSayfayiYukle();

                });

                thread.IsBackground = true;

                thread.Name = "thread" + i.ToString() + "&ticket=" + BildirgeTicket;

                BildirgeYuklemeThreads.Add(i, thread);

                thread.Start();

            }
            //}

        }

        void BildirgeYuklemeSayfayiYukle()
        {
            string ticket = null;

            if (Thread.CurrentThread.Name != null && System.Threading.Thread.CurrentThread.Name.Contains("ticket"))
            {
                ticket = System.Threading.Thread.CurrentThread.Name.Split('&')[1].Split('=')[1];
            }

            if (!BildirgeYuklemeYapiliyor || (ticket != null && !ticket.Equals(BildirgeTicket))) return;


            string girisCevabi = string.Empty;

            BildirgeWebClient.Disconnect();

            bool Baglanildi = false;

            do
            {
                if (!BildirgeYuklemeYapiliyor || (ticket != null && !ticket.Equals(BildirgeTicket))) return;

                if (girisCevabi.Equals("Error"))
                {
                    BildirgeWebClient = new ProjeGiris(SuanYapilanIsyeriBildirgeYukleme, Enums.ProjeTurleri.EBildirgeV2);

                    BildirgeWebClient.Ticket = BildirgeTicket;

                    Thread.Sleep(200);
                }

                girisCevabi = BildirgeWebClient.Connect();

                if (girisCevabi.Equals("LogOut"))
                {
                    return;
                }

                if (girisCevabi.Equals("Kullanıcı adı veya şifreleriniz hatalıdır")
                    || girisCevabi.Equals("İşyeri Kanun Kapsamından Çıkmıştır")
                    || girisCevabi.Equals("Is Yeri Iz Olmus")
                    || girisCevabi.Equals("işyeri hesabı PASİF olduğu için sisteme giriş yapamadı")
                    || girisCevabi.Equals("Vekalet Süresi Dolmuştur")
                    || girisCevabi.Equals("Güvenlik kodu girilmedi")
                     )
                    break;

            }
            while (!girisCevabi.Equals("OK"));

            if (girisCevabi.Equals("OK")) Baglanildi = true;

            if (!Baglanildi)
            {
                sb.Append(string.Format("[" + DateTime.Now.ToString() + "] : Sisteme giriş yapılamadı. Nedeni: {0}", girisCevabi) + Environment.NewLine); new delLoglariGuncelle(LoglariGuncelle).Invoke();
                sb.Append(string.Format("[" + DateTime.Now.ToString() + "] : Bildirge yükleme işlemi sona erdi", girisCevabi) + Environment.NewLine); new delLoglariGuncelle(LoglariGuncelle).Invoke();

                BildirgeYuklemeSonaErdi(false);

                return;
            }

            BildirgeYuklemeBaslangicEkranaGit();
        }

        private void BildirgeYuklemeBaslangicEkranaGit()
        {
            string ticket = null;

            if (Thread.CurrentThread.Name != null && System.Threading.Thread.CurrentThread.Name.Contains("ticket"))
            {
                ticket = System.Threading.Thread.CurrentThread.Name.Split('&')[1].Split('=')[1];
            }

            if (!BildirgeYuklemeYapiliyor || (ticket != null && !ticket.Equals(BildirgeTicket))) return;

            if (!EskiOnaydaBekleyenlerSilindi)
            {
                MesajYaz("Onayda bekleyen bildirge varsa onlar silinecek");
                OnaydaBekleyenleriSil();
                MesajYaz("Onayda bekleyen bildirgelerin silinmesi tamamlandı");
            }

            var basariliYuklenenVarMi = false;

            try
            {
                MesajYaz(new string('-', 280));

                for (seciliYuklenecekBildirge = 0; seciliYuklenecekBildirge < YuklenecekBildirgeler.Count; seciliYuklenecekBildirge++)
                {
                    if (!BildirgeYuklemeYapiliyor || (ticket != null && !ticket.Equals(BildirgeTicket))) return;

                    string Mesaj = String.Format("{0} bildirgesi işleme alınıyor  ({1}/{2})", System.IO.Path.GetFileNameWithoutExtension(YuklenecekBildirgeler[seciliYuklenecekBildirge]), seciliYuklenecekBildirge + 1, YuklenecekBildirgeler.Count);

                    MesajYaz(Mesaj);

                    SuanYuklenenBildirge = BildirgeBilgileriniAl(YuklenecekBildirgeler[seciliYuklenecekBildirge], out string hataMesaji);

                    if (SuanYuklenenBildirge != null && !SuanYuklenenBildirge.Mahiyet.Equals("İPTAL"))
                    {
                        secilenIptalEdilecekBildirgeKanunNo = null;

                        secilenIptalEdilecekBildirgeGunSayisi = 0;

                        secilenIptalEdilecekBildirgeToplamUcret = 0;

                        secilenIptalEdilecekBildirgeKisiler = new Dictionary<string, AphbSatir>();

                        tekrarSecilenIptalBildirgeRefNo = null;

                        HataliSecilenIptalBildirgeleri = new List<string>();

                        HataliKisiler = new Dictionary<string, List<string>>();

                        basariliEklenenSayisi = 0;

                        OnaylanacakBildirgeRefNo = null;

                        EkOlarakEklenecek = false;

                        AsilOlarakEklenecek = false;

                        kisilerinSatirNolari = new Dictionary<string, List<string>>();

                        islemYapilacakIptalBildirgeleri = new Dictionary<string, IptalBildirgeIstatikleri>();

                        secilenIptalEdilecekBildirge = null;

                        secilenIptalEdilecekBildirgeSira = 0;

                        var basariliIptalEdilenKisilerYatirimTesviki = new List<string>();

                        var yuklenecekBildirgeYatirimTesvikiMi = false;

                        if (SuanYuklenenBildirge.Kanun.EndsWith("6322") || SuanYuklenenBildirge.Kanun.Equals("25510"))
                        {
                            basariliIptalEdilenKisilerYatirimTesviki = YuklenenIcmaller.Where(p => p.Value.yilay.Key == SuanYuklenenBildirge.Yil && p.Value.yilay.Value == SuanYuklenenBildirge.Ay).SelectMany(p => p.Value.Kisiler.Where(a => a.Key == SuanYuklenenBildirge.Kanun).SelectMany(a => a.Value)).ToList();

                            yuklenecekBildirgeYatirimTesvikiMi = true;
                        }

                        #region Bildirge Icmal Tutarı Hesapla

                        decimal toplamIcmal = 0;

                        int toplamGun = 0;

                        decimal toplamUcret = 0;

                        int Yil = Convert.ToInt32(SuanYuklenenBildirge.Yil);
                        int Ay = Convert.ToInt32(SuanYuklenenBildirge.Ay);

                        var bildirgeTcleri = SuanYuklenenBildirge.Kisiler.Select(k => k.SosyalGuvenlikNo).Distinct();

                        var tesvik = Program.TumTesvikler.FirstOrDefault(p => p.Key.PadLeft(5, '0').Equals(SuanYuklenenBildirge.Kanun) || p.Value.AltKanunlar.Contains(SuanYuklenenBildirge.Kanun)).Value;

                        foreach (var tc in bildirgeTcleri)
                        {
                            int kisiToplamGun = SuanYuklenenBildirge.Kisiler.Where(k => k.SosyalGuvenlikNo.Equals(tc)).Sum(p => Convert.ToInt32(p.HesaplananGun));

                            //var tesvikKanunNo = yuklenecekBildirge.Kisiler.FirstOrDefault(k => k.SosyalGuvenlikNo.Equals(tc)).TesvikKanunNo;

                            decimal kisiToplamUcret = SuanYuklenenBildirge.Kisiler.Where(k => k.SosyalGuvenlikNo.Equals(tc)).Sum(p => p.HesaplananUcret.ToDecimalSgk() + p.HesaplananIkramiye.ToDecimalSgk());

                            toplamGun += kisiToplamGun;

                            toplamUcret += kisiToplamUcret;

                            var CarpimOrani687 = TesvikHesaplamaSabitleri.CarpimOrani687;

                            if (!string.IsNullOrEmpty(SuanYuklenenBildirge.EkBilgiler))
                            {
                                var deger = SuanYuklenenBildirge.EkBilgiler.Trim(';').Split(';').FirstOrDefault(p => p.Contains("CarpimOrani687"));

                                if (!string.IsNullOrEmpty(deger)) CarpimOrani687 = Convert.ToDecimal(deger.Split('=')[1]);
                            }

                            toplamIcmal += Metodlar.TesvikTutariHesapla(SuanYuklenenBildirge.Kanun, kisiToplamGun, kisiToplamUcret, Yil, Ay, SuanYuklenenBildirge.BelgeTuru, SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo, null, CarpimOrani687);

                            var kisiSatirlari = SuanYuklenenBildirge.Kisiler.Where(k => k.SosyalGuvenlikNo.Equals(tc));


                            foreach (var kisiSatir in kisiSatirlari)
                            {
                                //var tesvik = Program.TumTesvikler[kisiSatir.TesvikKanunNo];
                                var DonusturulenKanun = kisiSatir.Kanun;
                                var kanunGun = Convert.ToInt32(kisiSatir.HesaplananGun);
                                var kanunUcret = kisiSatir.HesaplananUcret.ToDecimalSgk() + kisiSatir.HesaplananIkramiye.ToDecimalSgk();

                                if (tesvik != null)
                                {
                                    var dk = tesvik.DonusturulecekKanunlar.FirstOrDefault(p => p.Key.Equals(DonusturulenKanun)).Value;

                                    if (dk != null)
                                    {
                                        if (TesvikHesaplamaSabitleri.BagliKanunlarIcmalHesaplama.ContainsKey(SuanYuklenenBildirge.Kanun) && TesvikHesaplamaSabitleri.BagliKanunlarIcmalHesaplama[SuanYuklenenBildirge.Kanun].Contains("05510"))
                                        {
                                            if (DonusturulenKanun.Equals("00000"))
                                            {
                                                toplamIcmal += kanunGun * Metodlar.AsgariUcretBul(Yil, Ay) * 0.05m;
                                            }
                                            else toplamIcmal += Metodlar.TesvikTutariHesapla("05510", kanunGun, kanunUcret, Yil, Ay, SuanYuklenenBildirge.BelgeTuru, SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo);
                                        }

                                        var dusulecekTutar = DonusturulecekKanun.DusulecekMiktarHesapla(DonusturulenKanun, kisiSatir.HesaplananDonusecekGun.ToInt(), kisiSatir.HesaplananDonusecekToplamUcret, Yil, Ay, SuanYuklenenBildirge.BelgeTuru, SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo, tesvik.DonusenlerIcmaldenDusulsun, null, CarpimOrani687)[DonusturulenKanun].BagliKanunlarDahilDusulecekTutar;

                                        toplamIcmal -= dusulecekTutar;
                                    }
                                }
                                else
                                {
                                    if (SuanYuklenenBildirge.Kanun.Equals("00000"))
                                    {

                                        var dusulecekTutar = DonusturulecekKanun.DusulecekMiktarHesapla(DonusturulenKanun, kisiSatir.HesaplananDonusecekGun.ToInt(), kisiSatir.HesaplananDonusecekToplamUcret, Yil, Ay, SuanYuklenenBildirge.BelgeTuru, SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo, true, null, CarpimOrani687)[DonusturulenKanun].BagliKanunlarDahilDusulecekTutar;

                                        toplamIcmal -= dusulecekTutar;
                                    }
                                }
                            }

                        }

                        BildirgeIcmaller[seciliYuklenecekBildirge].Tutar = toplamIcmal;
                        BildirgeIcmaller[seciliYuklenecekBildirge].PrimOdenenGunSayisi = toplamGun;
                        BildirgeIcmaller[seciliYuklenecekBildirge].Matrah = toplamUcret;
                        BildirgeIcmaller[seciliYuklenecekBildirge].yilay = new KeyValuePair<string, string>(SuanYuklenenBildirge.Yil, SuanYuklenenBildirge.Ay);
                        BildirgeIcmaller[seciliYuklenecekBildirge].Kanun = SuanYuklenenBildirge.Kanun;

                        #endregion

                        YuklenecekBildirgeBilgileri[seciliYuklenecekBildirge] = SuanYuklenenBildirge;

                        var html = new HtmlAgilityPack.HtmlDocument();

                    #region Dönem Seçilecek
                    AnaSayfayaGit:

                        string response = BildirgeWebClient.Get("https://ebildirge.sgk.gov.tr/EBildirgeV2/anasayfa.action", string.Empty);

                        if (!response.Contains("Aylık Prim Hizmet Belgesi Girişi"))
                        {
                            if (!BildirgeYuklemeYapiliyor || (ticket != null && !ticket.Equals(BildirgeTicket))) return;

                            Thread.Sleep(1000);

                            goto AnaSayfayaGit;
                        }

                        html.LoadHtml(response);

                        siradakiIslemBildirgeYukleme = "Dönem Seçilecek";

                        response = BildirgeWebClient.Get("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkuknormalTahakkukDonemBilgileriniYukle.action", string.Empty.AddToken(html));

                        var gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(response, siradakiIslemBildirgeYukleme);

                        if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz || gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto AnaSayfayaGit;

                        if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal) continue;

                        html.LoadHtml(response);

                        var donemyilselect = html.GetElementbyId("tahakkukdonemSecildi_hizmet_yil_ay_index");

                        var option = donemyilselect.Descendants("option").FirstOrDefault(o => o.InnerText.Equals(SuanYuklenenBildirge.Yil + "/" + SuanYuklenenBildirge.Ay.PadLeft(2, '0')));

                        if (option != null)
                        {
                            #region Select Kutusunda İptal Seçilecek

                            siradakiIslemBildirgeYukleme = "Select Kutusunda İptal Seçilecek";

                            var hizmet_yil_ay_index = option.GetAttributeValue("value", string.Empty);

                        SelectKutusundaIptalSeçilecek:

                            response = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukdonemSecildi.action", ("hizmet_yil_ay_index=" + hizmet_yil_ay_index).AddToken(html));

                            gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(response, siradakiIslemBildirgeYukleme);

                            if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto AnaSayfayaGit;

                            if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz) goto SelectKutusundaIptalSeçilecek;

                            if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal) continue;

                            html.LoadHtml(response);

                            var belgeMahiyeti = html.GetElementbyId("belgeMahiyetiId");

                            var optionIptal = belgeMahiyeti.Descendants("option").FirstOrDefault(o => o.InnerText.Equals("IPTAL"));

                            if (optionIptal != null)
                            {
                                #region İptal Bildirge Seçilecek

                                siradakiIslemBildirgeYukleme = "İptal Bildirge Seçilecek";

                                var valueiptal = optionIptal.GetAttributeValue("value", "");

                            IptalBildirgeSecilecek:

                                response = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukbelgeIslemleri.action", "belgeMahiyeti=" + valueiptal + "&belge_tur_index=0&tahakkukNeden=1003" + string.Empty.AddToken(html));

                                //Cari Dönem için \"(D)Belge Türü ve/veya Kanun Numarası Değişikliği\" tahakkuk nedeninden işlem yapılamaz
                                if (response.Contains("Cari Dönem için \"(D)Belge Türü ve/veya Kanun Numarası Değişikliği\" tahakkuk nedeninden işlem yapılamaz"))
                                {
                                    YuklenenBildirgeHataMesajiYaz("Cari Dönem için D tahakkuk nedeninden işlem yapılamaz", seciliYuklenecekBildirge);
                                    continue;
                                }

                                if (response.Contains("belge değişikliğini ayın 27'si ve sonrasında yapınız"))
                                {
                                    YuklenenBildirgeHataMesajiYaz("SGK Hata Mesajı: " + "Belge değişikliğini ayın 27'si ve sonrasında yapınız", seciliYuklenecekBildirge);

                                    continue;
                                }


                                gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(response, siradakiIslemBildirgeYukleme);

                                if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto AnaSayfayaGit;

                                if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz) goto IptalBildirgeSecilecek;

                                if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal) continue;

                                html.LoadHtml(response);

                                if (response.Contains("Dönemine Ait Herhangi Bir Tahakkuk Bulunamamıştır"))
                                {
                                    YuklenenBildirgeHataMesajiYaz("Bildirgenin ait olduğu dönemde iptal edilebilecek herhangi bir tahakkuk bulunamadı", seciliYuklenecekBildirge);

                                    continue;

                                }
                                else
                                {
                                    var radios = html.DocumentNode.Descendants("input").Where(i => i.GetAttributeValue("type", "").Equals("radio"));

                                    var iptalyapilacakradios = new Dictionary<HtmlAgilityPack.HtmlNode, string>();

                                    var iptaledilecekler = new Dictionary<string, Dictionary<HtmlAgilityPack.HtmlNode, string>>();

                                    var iptalKanunNolari = SuanYuklenenBildirge.Kisiler.Select(k => k.Kanun).Distinct().ToList();

                                    decimal CarpimOrani687 = TesvikHesaplamaSabitleri.CarpimOrani687;

                                    if (!string.IsNullOrEmpty(SuanYuklenenBildirge.EkBilgiler) && SuanYuklenenBildirge.EkBilgiler.Contains("CarpimOrani687"))
                                    {
                                        var deger = SuanYuklenenBildirge.EkBilgiler.Trim(';').Split(';').FirstOrDefault(p => p.Contains("CarpimOrani687"));

                                        if (!string.IsNullOrEmpty(deger)) CarpimOrani687 = Convert.ToDecimal(deger.Split('=')[1]);
                                    }

                                    #region Bildirgede Dönüştürülen Kanunların Olduğu Bildirgeleri Bulma

                                    foreach (var radio in radios)
                                    {
                                        var trbaslik = radio.Ancestors("table").ElementAt(0).ParentNode.ParentNode.PreviousSibling.PreviousSibling.InnerText.Replace("\r", "").Replace("\t", "").Replace("\n", "").Replace(" ", "");

                                        foreach (var iptalKanun in iptalKanunNolari)
                                        {
                                            var iptalKanunNo = string.Empty;

                                            if (!iptalKanun.Equals("00000"))
                                            {
                                                iptalKanunNo = Convert.ToInt32(iptalKanun).ToString();

                                                iptalKanunNo += "KanunNolu";
                                            }

                                            var iptalBulundu = false;

                                            if (string.IsNullOrEmpty(iptalKanunNo))
                                            {
                                                if (trbaslik.Equals(Convert.ToInt32(SuanYuklenenBildirge.BelgeTuru).ToString() + "BelgeÇeşidiOlanTahakkuklar")) iptalBulundu = true;
                                            }
                                            else
                                            {
                                                if ((trbaslik.StartsWith(iptalKanunNo) && trbaslik.Contains(Convert.ToInt32(SuanYuklenenBildirge.BelgeTuru).ToString() + "BelgeÇeşidiOlanTahakkuklar"))) iptalBulundu = true;

                                            }

                                            if (iptalBulundu)
                                            {
                                                var trs = radio.Ancestors("tr").First().ParentNode.Descendants("tr").Skip(1);

                                                if (!iptaledilecekler.ContainsKey(trbaslik))
                                                {
                                                    iptaledilecekler.Add(trbaslik, new Dictionary<HtmlAgilityPack.HtmlNode, string>());

                                                    foreach (var tr in trs)
                                                    {
                                                        var tds = tr.Descendants("td");

                                                        var bulunanradio = tr.Descendants("input").Where(i => i.GetAttributeValue("type", "").Equals("radio")).FirstOrDefault();

                                                        if (bulunanradio == null)
                                                        {
                                                            bulunanradio = HtmlAgilityPack.HtmlNode.CreateNode("a");
                                                        }

                                                        var belgeMahiyeti2 = tds.ElementAt(3).InnerText.Trim();
                                                        var kisiSayisi = tds.ElementAt(4).InnerText.Trim();
                                                        var toplamgun = tds.ElementAt(5).InnerText.Trim();
                                                        var toplamprim = tds.ElementAt(6).InnerText.Trim();

                                                        var item = belgeMahiyeti2 + "-" + kisiSayisi + "-" + toplamgun + "-" + toplamprim + "-" + iptalKanun;

                                                        if (!iptaledilecekler[trbaslik].ContainsKey(bulunanradio)) iptaledilecekler[trbaslik].Add(bulunanradio, item);
                                                    }
                                                }

                                                break;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Bulunan İptal Yapılacak Bildirgelerden İptallerin Asılları Götürmesi

                                    foreach (var item in iptaledilecekler)
                                    {
                                        List<int> silinecekler = new List<int>();

                                        for (int i = 0; i < item.Value.Count; i++)
                                        {
                                            var iptaledilecekbildirge = item.Value.ElementAt(i);

                                            if (iptaledilecekbildirge.Value.StartsWith("I-"))
                                            {

                                                silinecekler.Add(i);

                                                bool bulundu = false;

                                                for (int j = 0; j < item.Value.Count; j++)
                                                {
                                                    var iptaledilecekbildirge2 = item.Value.ElementAt(j);

                                                    if (silinecekler.Contains(j)) continue;

                                                    if (iptaledilecekbildirge2.Value.StartsWith("A-"))
                                                    {
                                                        if (iptaledilecekbildirge.Value.Substring(2).Equals(iptaledilecekbildirge2.Value.Substring(2)))
                                                        {
                                                            bulundu = true;

                                                            silinecekler.Add(j);

                                                            break;
                                                        }
                                                    }
                                                }

                                                if (!bulundu)
                                                {
                                                    for (int j = 0; j < item.Value.Count; j++)
                                                    {
                                                        var iptaledilecekbildirge2 = item.Value.ElementAt(j);

                                                        if (silinecekler.Contains(j)) continue;

                                                        if (iptaledilecekbildirge2.Value.StartsWith("E-"))
                                                        {
                                                            if (iptaledilecekbildirge.Value.Substring(2).Equals(iptaledilecekbildirge2.Value.Substring(2)))
                                                            {
                                                                bulundu = true;

                                                                silinecekler.Add(j);

                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        silinecekler = silinecekler.Distinct().ToList();

                                        for (int i = 0; i < item.Value.Count; i++)
                                        {

                                            if (!silinecekler.Contains(i))
                                            {
                                                var iptalKanun = item.Value.ElementAt(i).Value.Split('-')[item.Value.ElementAt(i).Value.Split('-').Length - 1];

                                                iptalyapilacakradios.Add(item.Value.ElementAt(i).Key, iptalKanun);
                                            }
                                        }

                                    }

                                    #endregion

                                    if (iptalyapilacakradios.Count == 0)
                                    {
                                        MesajYaz("İptal için seçilecek bildirge bulunamamıştır");

                                        continue;
                                    }
                                    else if (iptalyapilacakradios.Count > 0)
                                    {

                                        var radioIlk = iptalyapilacakradios.ElementAt(0).Key;

                                        var radioValueIlk = iptalyapilacakradios.ElementAt(0).Key.GetAttributeValue("value", "");

                                        var donusturulecekKanunNoIlk = iptalyapilacakradios.ElementAt(0).Value;

                                        if (!islemYapilacakIptalBildirgeleri.ContainsKey(radioValueIlk))
                                        {
                                            var tr = radioIlk.Ancestors("tr").FirstOrDefault();

                                            var tds = tr.Descendants("td");

                                            string mahiyet = tds.ElementAt(3).InnerText.Trim().Equals("A") ? "ASIL" : "EK";
                                            string islemtarihi = tds.ElementAt(2).InnerText.Trim();
                                            string iptalkanun = donusturulecekKanunNoIlk;

                                            islemYapilacakIptalBildirgeleri.Add(radioValueIlk, new IptalBildirgeIstatikleri { Mahiyet = mahiyet, IslemTarihi = islemtarihi, IptalKanun = iptalkanun });
                                        }

                                        bool yeniIptalBelgesiGonderilemezUyarisiVar = false;

                                        #region Birden Fazla İptal Edilecek Bildirge Varsa En Kazançlısını Seçme

                                        if (iptalyapilacakradios.Count > 1)
                                        {

                                            foreach (var iyr in iptalyapilacakradios)
                                            {
                                                var radio = iyr.Key;

                                                var donusturulecekKanunNo = iyr.Value;

                                                var radioValue = radio.GetAttributeValue("value", "");

                                            IptalEdilecekKisilerSayfasiniYukle:

                                                string responsehizmetler = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukhizmetleriGetir.action", "th_tahakkuk_index=" + radioValue);

                                                if (responsehizmetler.Contains("Söz konusu belgenin işlemleri tamamlanmadan yeni bir IPTAL<br>belge gönderilemez"))
                                                {
                                                    yeniIptalBelgesiGonderilemezUyarisiVar = true;

                                                    break;
                                                }

                                                bool yeniBelgeOlustur = !Program.OncekiBildirgelerIptalEdilsin;

                                                if (responsehizmetler.Contains("Bu bilgilere sahip bildirge önceden oluşturulmuş"))
                                                {
                                                    if (Program.OncekiBildirgelerIptalEdilsin)
                                                    {
                                                        yeniBelgeOlustur = AylarIslemDevamYeniOlusturListesi.Contains(SuanYuklenenBildirge.Yil + "/" + SuanYuklenenBildirge.Ay.PadLeft(2, '0'));
                                                    }

                                                    var postData = yeniBelgeOlustur ? "action%3AtahakkukhizmetleriGetirYenidenGirisIkinciIptal=Yeni+%C4%B0ptal+Belge+Olu%C5%9Ftur" : "action%3AtahakkukhizmetleriGetirYenidenGiris=%C4%B0%C5%9Fleme+Devam";

                                                    responsehizmetler = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/.action", postData);
                                                }

                                                if
                                                (
                                                    responsehizmetler.Contains("Sistem Hatası") ||
                                                    responsehizmetler.Contains("Uyumsuz veri tespiti yapıldı") ||
                                                    responsehizmetler.Contains("Bu Belgenin aslı Yok") ||
                                                    responsehizmetler.Contains("Bu belgede iptal edilebilecek kişi yoktur") ||
                                                    responsehizmetler.Contains("Bu belgede iptal edilebilecek Kişi yoktur") ||
                                                    responsehizmetler.Contains("Onaylanmamış İptal Belgeniz vardır") ||
                                                    (!yeniBelgeOlustur && responsehizmetler.Contains("The requested URL was rejected")) ||
                                                    (!yeniBelgeOlustur && responsehizmetler.Contains("Belge Silinmesinde beklenmeyen bir durum oluştu lütfen tekrar deneyiniz"))
                                                )
                                                {
                                                    continue;
                                                }


                                                gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(responsehizmetler, "İptal Edilecek Kişiler Seçilecek");

                                                if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal) continue;

                                                if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto AnaSayfayaGit;

                                                if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz) goto IptalEdilecekKisilerSayfasiniYukle;

                                                var htmliptalkisiler = new HtmlAgilityPack.HtmlDocument();
                                                htmliptalkisiler.LoadHtml(responsehizmetler);

                                                var formtileislemTamam = htmliptalkisiler.GetElementbyId("tilesislemTamam");

                                                if (formtileislemTamam != null)
                                                {
                                                    var table = formtileislemTamam.Descendants("table").First();

                                                    decimal toplamTesvikTutari = 0;

                                                    foreach (var tc in bildirgeTcleri)
                                                    {
                                                        if (HataliKisiler.ContainsKey(radioValue) && HataliKisiler[radioValue].Contains(tc)) continue;

                                                        var td = table.Descendants("td").FirstOrDefault(t => t.InnerText.Trim().Equals(tc));

                                                        if (td != null)
                                                        {
                                                            if (td.ParentNode.Descendants("span").All(s => s.GetAttributeValue("style", "").Equals("background-color:")))
                                                            {
                                                                var gun = Convert.ToInt32(td.ParentNode.Descendants("td").ElementAt(7).InnerText.Trim());

                                                                var toplamucret = td.ParentNode.Descendants("td").ElementAt(11).InnerText.Trim().ToDecimalSgk();

                                                                var kisi = SuanYuklenenBildirge.Kisiler.FirstOrDefault(k => k.SosyalGuvenlikNo.Equals(tc) && k.Kanun.Equals(donusturulecekKanunNo));

                                                                if (kisi != null)
                                                                {
                                                                    var tesvikKanunNo = kisi.TesvikKanunNo;
                                                                    var iptalKanunNo = kisi.Kanun;

                                                                    if (gun < kisi.HesaplananGun.ToInt())
                                                                    {
                                                                        kisi.TesvikHesaplanacakGun = gun.ToString();
                                                                    }

                                                                    if (gun < kisi.HesaplananDonusecekGun.ToInt())
                                                                    {
                                                                        kisi.DonusturulecekHesaplanacakGun = gun.ToString();
                                                                    }

                                                                    var tesvikHesaplanacakGun = kisi.HesaplananGun.ToInt();
                                                                    var tesvikHesaplanacakToplamUcret = kisi.HesaplananToplamUcret;
                                                                    var donusecekHesaplananGun = kisi.HesaplananDonusecekGun.ToInt();
                                                                    var donusecekToplamUcret = kisi.HesaplananDonusecekToplamUcret;

                                                                    List<Tesvik> tesvikler = new List<Tesvik>();

                                                                    var tesvikAsil = Program.TumTesvikler.FirstOrDefault(p => p.Key.Equals(tesvikKanunNo) || p.Value.AltKanunlar.Contains(tesvikKanunNo)).Value;

                                                                    if (tesvikAsil != null)
                                                                    {
                                                                        tesvikler.Add(tesvikAsil);

                                                                        if (tesvikAsil.altTesvikler.Count > 0)
                                                                        {
                                                                            tesvikler.AddRange(tesvikAsil.altTesvikler.Select(p => Program.TumTesvikler[p]));
                                                                        }
                                                                    }


                                                                    foreach (var tesvik2 in tesvikler)
                                                                    {
                                                                        var KanunNo = tesvik2.Kanun.PadLeft(5, '0');

                                                                        if (tesvik2.AltKanunlar.Count > 0)
                                                                        {
                                                                            if (tesvik2.AltKanunlar.Contains(SuanYuklenenBildirge.Kanun))
                                                                            {
                                                                                KanunNo = SuanYuklenenBildirge.Kanun;
                                                                            }
                                                                            else if (!string.IsNullOrEmpty(tesvik2.AltKanun))
                                                                            {
                                                                                KanunNo = tesvik2.AltKanun;
                                                                            }
                                                                        }

                                                                        if (tesvik2.Kanun.Equals("7166")) KanunNo = "07166";

                                                                        toplamTesvikTutari += Metodlar.TesvikTutariHesapla(KanunNo, tesvikHesaplanacakGun, tesvikHesaplanacakToplamUcret, Yil, Ay, SuanYuklenenBildirge.BelgeTuru, SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo, null, CarpimOrani687);

                                                                        //var tesvik2 = Program.TumTesvikler.FirstOrDefault(p => SuanYuklenenBildirge.Kanun.Equals(p.Key.PadLeft(5, '0')) || p.Value.AltKanunlar.Contains(SuanYuklenenBildirge.Kanun.PadLeft(5, '0'))).Value;

                                                                        var dk = tesvik2.DonusturulecekKanunlar.FirstOrDefault(p => p.Key.EndsWith(donusturulecekKanunNo)).Value;

                                                                        if (dk != null)
                                                                        {
                                                                            if (TesvikHesaplamaSabitleri.BagliKanunlarIcmalHesaplama.ContainsKey(KanunNo) && TesvikHesaplamaSabitleri.BagliKanunlarIcmalHesaplama[KanunNo].Contains("05510"))
                                                                            {
                                                                                if (donusturulecekKanunNo.Equals("00000"))
                                                                                {
                                                                                    toplamTesvikTutari += tesvikHesaplanacakGun * Metodlar.AsgariUcretBul(Yil, Ay) * 0.05m;
                                                                                }
                                                                                else toplamTesvikTutari += Metodlar.TesvikTutariHesapla("05510", tesvikHesaplanacakGun, tesvikHesaplanacakToplamUcret, Yil, Ay, SuanYuklenenBildirge.BelgeTuru, SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo);
                                                                            }

                                                                            var dusulecekTutar = DonusturulecekKanun.DusulecekMiktarHesapla(dk.DonusturulecekKanunNo, donusecekHesaplananGun, donusecekToplamUcret, Yil, Ay, SuanYuklenenBildirge.BelgeTuru, SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo, tesvik2.DonusenlerIcmaldenDusulsun, null, CarpimOrani687)[dk.DonusturulecekKanunNo].BagliKanunlarDahilDusulecekTutar;

                                                                            toplamTesvikTutari -= dusulecekTutar;
                                                                        }
                                                                    }

                                                                }

                                                            }

                                                        }
                                                    }

                                                    if (toplamTesvikTutari > 0)
                                                    {

                                                        if (!islemYapilacakIptalBildirgeleri.ContainsKey(radioValue))
                                                        {
                                                            var tr = radio.Ancestors("tr").FirstOrDefault();

                                                            var tds = tr.Descendants("td");

                                                            string mahiyet = tds.ElementAt(3).InnerText.Trim().Equals("A") ? "ASIL" : "EK";
                                                            string islemtarihi = tds.ElementAt(2).InnerText.Trim();
                                                            string iptalkanun = donusturulecekKanunNo;

                                                            islemYapilacakIptalBildirgeleri.Add(radioValue, new IptalBildirgeIstatikleri { Mahiyet = mahiyet, IslemTarihi = islemtarihi, IptalKanun = iptalkanun, IptalEdilecekKisilerEkranindanHesaplananTesvikTutari = toplamTesvikTutari });
                                                        }
                                                        else islemYapilacakIptalBildirgeleri[radioValue].IptalEdilecekKisilerEkranindanHesaplananTesvikTutari = toplamTesvikTutari;
                                                    }


                                                }
                                            }


                                            //if (SuanYuklenenBildirge.Mahiyet.Equals("EK"))
                                            //{
                                            //    if (islemYapilacakIptalBildirgeleri.Count > 0)
                                            //    {
                                            //        islemYapilacakIptalBildirgeleri = islemYapilacakIptalBildirgeleri.OrderByDescending(p => p.Value.IptalEdilecekKisilerEkranindanHesaplananTesvikTutari).Take(1).ToDictionary(x => x.Key, x => x.Value);
                                            //    }
                                            //}

                                        }


                                        #endregion

                                        if (!yeniIptalBelgesiGonderilemezUyarisiVar)
                                        {
                                            #region İptal Edilecek Kişiler Seçilecek

                                            secilenIptalEdilecekBildirge = null;

                                            secilenIptalEdilecekBildirgeSira = 0;

                                            while (islemYapilacakIptalBildirgeleri.Any(p => !p.Value.Tamamlandi) && islemYapilacakIptalBildirgeleri.Count(p => p.Value.Basarili) < (SuanYuklenenBildirge.Mahiyet.Equals("EK") ? 1 : 2))
                                            {

                                                secilenIptalEdilecekBildirge = islemYapilacakIptalBildirgeleri.OrderByDescending(p => p.Value.IptalEdilecekKisilerEkranindanHesaplananTesvikTutari).FirstOrDefault(p => !p.Value.Tamamlandi).Key;

                                                if (secilenIptalEdilecekBildirge != null)
                                                {
                                                    secilenIptalEdilecekBildirgeSira++;

                                                    AsilOlarakEklenecek = false;

                                                DonemSecilecek:
                                                AnaSayfayaGit2:
                                                    response = BildirgeWebClient.Get("https://ebildirge.sgk.gov.tr/EBildirgeV2/anasayfa.action", string.Empty);

                                                    if (!response.Contains("Aylık Prim Hizmet Belgesi Girişi"))
                                                    {
                                                        if (!BildirgeYuklemeYapiliyor || (ticket != null && !ticket.Equals(BildirgeTicket))) return;

                                                        Thread.Sleep(1000);

                                                        goto AnaSayfayaGit2;
                                                    }

                                                    html.LoadHtml(response);

                                                    KarsiligiOlmayanIptalleriSil();

                                                    siradakiIslemBildirgeYukleme = "Dönem Seçilecek";

                                                    response = BildirgeWebClient.Get("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkuknormalTahakkukDonemBilgileriniYukle.action", string.Empty.AddToken(html));

                                                    gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(response, siradakiIslemBildirgeYukleme);

                                                    if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz) goto DonemSecilecek;

                                                    if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto AnaSayfayaGit2;

                                                    if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal)
                                                    {
                                                        islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                        continue;
                                                    }

                                                    html.LoadHtml(response);

                                                    donemyilselect = html.GetElementbyId("tahakkukdonemSecildi_hizmet_yil_ay_index");

                                                    option = donemyilselect.Descendants("option").FirstOrDefault(o => o.InnerText.Equals(SuanYuklenenBildirge.Yil + "/" + SuanYuklenenBildirge.Ay.PadLeft(2, '0')));

                                                    if (option != null)
                                                    {

                                                        siradakiIslemBildirgeYukleme = "Select Kutusunda İptal Seçilecek";

                                                        hizmet_yil_ay_index = option.GetAttributeValue("value", string.Empty);

                                                    SelectKutusundaIptalSeçilecek2:

                                                        response = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukdonemSecildi.action", ("hizmet_yil_ay_index=" + hizmet_yil_ay_index).AddToken(html));

                                                        gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(response, siradakiIslemBildirgeYukleme);

                                                        if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz) goto SelectKutusundaIptalSeçilecek2;

                                                        if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto AnaSayfayaGit2;

                                                        if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal)
                                                        {
                                                            islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                            continue;
                                                        }

                                                        html.LoadHtml(response);

                                                        belgeMahiyeti = html.GetElementbyId("belgeMahiyetiId");

                                                        optionIptal = belgeMahiyeti.Descendants("option").FirstOrDefault(o => o.InnerText.Equals("IPTAL"));

                                                        if (optionIptal != null)
                                                        {

                                                            siradakiIslemBildirgeYukleme = "İptal Bildirge Seçilecek";

                                                            valueiptal = optionIptal.GetAttributeValue("value", "");

                                                        IptalBildirgeSecilecek2:
                                                            response = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukbelgeIslemleri.action", "belgeMahiyeti=" + valueiptal + "&belge_tur_index=0&tahakkukNeden=1003".AddToken(html));

                                                            gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(response, siradakiIslemBildirgeYukleme);

                                                            if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz) goto IptalBildirgeSecilecek2;

                                                            if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto AnaSayfayaGit2;

                                                            if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal)
                                                            {
                                                                islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                continue;
                                                            }

                                                            if (response.Contains("belge değişikliğini ayın 27'si ve sonrasında yapınız"))
                                                            {
                                                                YuklenenBildirgeHataMesajiYaz("SGK Hata Mesajı: " + "Belge değişikliğini ayın 27'si ve sonrasında yapınız", seciliYuklenecekBildirge);

                                                                break;
                                                            }
                                                            
                                                            html.LoadHtml(response);


                                                            siradakiIslemBildirgeYukleme = "İptal Edilecek Kişiler Seçilecek";

                                                            secilenIptalEdilecekBildirgeKanunNo = islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].IptalKanun;

                                                        IptalEdilecekKisilerSayfasiniYukle:

                                                            response = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukhizmetleriGetir.action", "th_tahakkuk_index=" + secilenIptalEdilecekBildirge.AddToken(html));

                                                            if (response.Contains("Bu bilgilere sahip bildirge önceden oluşturulmuş"))
                                                            {
                                                                siradakiIslemBildirgeYukleme = "Yeni Belge Oluştur veya İşleme Devam Seçilecek";

                                                                bool yeniBelgeOlustur = Program.OncekiBildirgelerIptalEdilsin ? AylarIslemDevamYeniOlusturListesi.Contains(SuanYuklenenBildirge.Yil + "/" + SuanYuklenenBildirge.Ay.PadLeft(2, '0')) : true;

                                                                var postDatayeniBelgeOlustur = yeniBelgeOlustur ? "action%3AtahakkukhizmetleriGetirYenidenGirisIkinciIptal=Yeni+%C4%B0ptal+Belge+Olu%C5%9Ftur" : "action%3AtahakkukhizmetleriGetirYenidenGiris=%C4%B0%C5%9Fleme+Devam";

                                                                siradakiIslemBildirgeYukleme = "İptal Edilecek Kişiler Seçilecek";

                                                            IptalEdilecekSayfasiniYukleYeniBelgeOlustur:

                                                                response = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/.action", postDatayeniBelgeOlustur.AddToken(response));

                                                                if (!yeniBelgeOlustur && response.Contains("The requested URL was rejected"))
                                                                {
                                                                    MesajYaz("Yeni İptal Belge Oluştur-İşleme Devam ekranında işleme devam butonuna tıklanınca sistemden kaynaklı hata verdiği için sıradaki bildirgeye geçilecek");


                                                                    islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                    continue;
                                                                }
                                                                else if (!yeniBelgeOlustur && response.Contains("Belge Silinmesinde beklenmeyen bir durum oluştu lütfen tekrar deneyiniz"))
                                                                {
                                                                    MesajYaz("SGK Hata Mesajı: Belge Silinmesinde beklenmeyen bir durum oluştu lütfen tekrar deneyiniz");

                                                                    islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                    continue;
                                                                }
                                                                else
                                                                {

                                                                    gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(response, siradakiIslemBildirgeYukleme);

                                                                    if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal)
                                                                    {
                                                                        islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                        continue;
                                                                    }

                                                                    if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz) goto IptalEdilecekSayfasiniYukleYeniBelgeOlustur;

                                                                    if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto AnaSayfayaGit2;

                                                                    if (!AylarIslemDevamYeniOlusturListesi.Contains(SuanYuklenenBildirge.Yil + "/" + SuanYuklenenBildirge.Ay.PadLeft(2, '0')))
                                                                    {
                                                                        AylarIslemDevamYeniOlusturListesi.Add(SuanYuklenenBildirge.Yil + "/" + SuanYuklenenBildirge.Ay.PadLeft(2, '0'));
                                                                    }

                                                                }
                                                            }

                                                            gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(response, siradakiIslemBildirgeYukleme);

                                                            if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal)
                                                            {
                                                                islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                continue;
                                                            }

                                                            if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz) goto IptalEdilecekKisilerSayfasiniYukle;

                                                            if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto AnaSayfayaGit2;

                                                            html.LoadHtml(response);

                                                        HataliKisiVarTekrarDene:

                                                            var formtileislemTamam = html.GetElementbyId("tilesislemTamam");

                                                            var table = formtileislemTamam.Descendants("table").First();

                                                            var trs = table.Descendants("tr").ToList();

                                                            string postData = "__checkbox_=true&";

                                                            int secilenKisiSayisi = 0;

                                                            int bildirgedenBulunanKisiSayisi = 0;

                                                            var trsCount = trs.Count();

                                                            var yatirimTesvikiKota = basariliIptalEdilenKisilerYatirimTesviki.Count();

                                                            foreach (var kisi in SuanYuklenenBildirge.Kisiler)
                                                            {
                                                                if (yuklenecekBildirgeYatirimTesvikiMi)
                                                                {
                                                                    if (yatirimTesvikiKota <= 0)
                                                                    {
                                                                        continue;
                                                                    }
                                                                }

                                                                HtmlAgilityPack.HtmlNode tr = null;

                                                                if (kisilerinSatirNolari.ContainsKey(kisi.SosyalGuvenlikNo))
                                                                {
                                                                    tr = trs.ElementAt(Convert.ToInt32(kisilerinSatirNolari[kisi.SosyalGuvenlikNo].First()));

                                                                    if (!tr.InnerText.Contains(kisi.SosyalGuvenlikNo))
                                                                    {
                                                                        tr = null;

                                                                        kisilerinSatirNolari = new Dictionary<string, List<string>>();
                                                                    }
                                                                }

                                                                if (tr == null)
                                                                {
                                                                    tr = trs.FirstOrDefault(p => p.InnerText.Contains(kisi.SosyalGuvenlikNo));
                                                                }

                                                                if (tr != null)
                                                                {
                                                                    bildirgedenBulunanKisiSayisi++;

                                                                    string hiddenvalue = null;

                                                                    string checkboxvalue = null;

                                                                    var tds = tr.Descendants("td");

                                                                    var istenCikisTarihi = tds.ElementAt(13).InnerText;
                                                                    var istenCikisNedeni = tds.ElementAt(15).InnerText;

                                                                    if (kisilerinSatirNolari.ContainsKey(kisi.SosyalGuvenlikNo))
                                                                    {
                                                                        hiddenvalue = kisilerinSatirNolari[kisi.SosyalGuvenlikNo][1];

                                                                        checkboxvalue = kisilerinSatirNolari[kisi.SosyalGuvenlikNo][2];
                                                                    }
                                                                    else
                                                                    {

                                                                        var hidden = tr.Descendants("input").FirstOrDefault(p => p.GetAttributeValue("name", "").Equals("__checkbox_seciliSigortaliCbox"));

                                                                        var checkbox = tr.Descendants("input").FirstOrDefault(p => p.GetAttributeValue("name", "").Equals("seciliSigortaliCbox"));

                                                                        hiddenvalue = hidden.GetAttributeValue("value", "");

                                                                        checkboxvalue = checkbox.GetAttributeValue("value", "");
                                                                    }

                                                                    postData += "__checkbox_seciliSigortaliCbox=" + hiddenvalue + "&";

                                                                    if (tr.Descendants("span").All(s => s.GetAttributeValue("style", "").Equals("background-color:") || s.GetAttributeValue("style", "").Equals("background-color:#bbffbb")))
                                                                    {
                                                                        if (!HataliKisiler.ContainsKey(secilenIptalEdilecekBildirge) || !HataliKisiler[secilenIptalEdilecekBildirge].Contains(kisi.SosyalGuvenlikNo))
                                                                        {
                                                                            if ((string.IsNullOrEmpty(istenCikisTarihi) && string.IsNullOrEmpty(istenCikisNedeni))
                                                                                || (!string.IsNullOrEmpty(istenCikisTarihi) && !string.IsNullOrEmpty(istenCikisNedeni)))
                                                                            {
                                                                                postData += "seciliSigortaliCbox=" + checkboxvalue + "&";

                                                                                secilenKisiSayisi++;

                                                                                yatirimTesvikiKota--;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (!HataliKisiler.ContainsKey(secilenIptalEdilecekBildirge)) HataliKisiler.Add(secilenIptalEdilecekBildirge, new List<string>());

                                                                                if (!HataliKisiler[secilenIptalEdilecekBildirge].Contains(kisi.SosyalGuvenlikNo)) HataliKisiler[secilenIptalEdilecekBildirge].Add(kisi.SosyalGuvenlikNo);
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {

                                                                        if (!HataliKisiler.ContainsKey(secilenIptalEdilecekBildirge)) HataliKisiler.Add(secilenIptalEdilecekBildirge, new List<string>());

                                                                        if (!HataliKisiler[secilenIptalEdilecekBildirge].Contains(kisi.SosyalGuvenlikNo)) HataliKisiler[secilenIptalEdilecekBildirge].Add(kisi.SosyalGuvenlikNo);
                                                                    }

                                                                    if (!kisilerinSatirNolari.ContainsKey(kisi.SosyalGuvenlikNo)) kisilerinSatirNolari.Add(kisi.SosyalGuvenlikNo, new List<string> { trs.IndexOf(tr).ToString(), hiddenvalue, checkboxvalue });

                                                                }
                                                            }

                                                            islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].bulunanKisiSayisi = bildirgedenBulunanKisiSayisi;

                                                            if (secilenKisiSayisi > 0)
                                                            {

                                                                #region İptal Edilecek Kişilerin Kontrolü Yapılacak

                                                                siradakiIslemBildirgeYukleme = "İptal Edilecek Kişilerin Kontrolü Yapılacak";

                                                                postData += "action%3AtahakkukiptalBelgeKisi=Se%C3%A7ili+Ki%C5%9Fileri+%C4%B0ptal+Et";

                                                            IptalEdilecekKisilerinKontroluYapilacak:

                                                                response = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tilesislemTamam.action ", postData.AddToken(html));

                                                                html.LoadHtml(response);

                                                                if (html.GetElementbyId("genelUyariCenterTag") != null && !String.IsNullOrEmpty(html.GetElementbyId("genelUyariCenterTag").InnerText))
                                                                {
                                                                    if (html.DocumentNode.Descendants("input").Any(p => p.Id.Trim().Equals("tilesislemTamam_tahakkukiptalBelgeKisi")))
                                                                    {
                                                                        siradakiIslemBildirgeYukleme = "İptal Edilecek Kişiler Seçilecek";

                                                                        goto HataliKisiVarTekrarDene;
                                                                    }
                                                                }

                                                                gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(response, siradakiIslemBildirgeYukleme);

                                                                if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal)
                                                                {
                                                                    islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                    continue;
                                                                }

                                                                if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz) goto IptalEdilecekKisilerinKontroluYapilacak;

                                                                if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto AnaSayfayaGit2;

                                                                var tableKisilerinKontrolu = html.DocumentNode.Descendants().FirstOrDefault(t => t.GetAttributeValue("class", "").Equals("gradienttable"));

                                                                var trsKisilerinKontrolu = tableKisilerinKontrolu.Descendants("tr");

                                                                bool hatalivar = false;

                                                                int toplamgun = 0;

                                                                decimal toplamtutar = 0;

                                                                Dictionary<string, AphbSatir> seciliKisiler = new Dictionary<string, AphbSatir>();

                                                                for (int i = 1; i < trsKisilerinKontrolu.Count(); i++)
                                                                {
                                                                    var spans = trsKisilerinKontrolu.ElementAt(i).Descendants("span");
                                                                    if (!spans.All(s => s.GetAttributeValue("style", "").Equals("background-color:#bbffbb")))
                                                                    {
                                                                        var hataliTc = trsKisilerinKontrolu.ElementAt(i).Descendants("td").ElementAt(2).InnerText;

                                                                        if (!HataliKisiler.ContainsKey(secilenIptalEdilecekBildirge)) HataliKisiler.Add(secilenIptalEdilecekBildirge, new List<string>());

                                                                        if (!HataliKisiler[secilenIptalEdilecekBildirge].Contains(hataliTc))
                                                                        {
                                                                            HataliKisiler[secilenIptalEdilecekBildirge].Add(hataliTc);

                                                                            hatalivar = true;
                                                                        }

                                                                    }
                                                                    else
                                                                    {
                                                                        var gun = Convert.ToInt32(spans.ElementAt(5).InnerText.Trim());

                                                                        var toplamucret = spans.ElementAt(9).InnerText.Trim().ToDecimalSgk();
                                                                        var ucret = spans.ElementAt(7).InnerText.Trim();
                                                                        var ikramiye = spans.ElementAt(8).InnerText.Trim();

                                                                        var tcno = spans.ElementAt(1).InnerText.Trim();
                                                                        var cikisgunu = spans.ElementAt(11).InnerText.Trim();
                                                                        var girisgunu = spans.ElementAt(10).InnerText.Trim();

                                                                        seciliKisiler.Add(tcno, new AphbSatir { CikisGunu = cikisgunu, GirisGunu = girisgunu, Gun = gun.ToString(), Ucret = ucret, Ikramiye = ikramiye });

                                                                        if (gun > 0 || toplamucret > 0)
                                                                        {

                                                                            toplamgun += gun;

                                                                            toplamtutar += toplamucret;
                                                                        }

                                                                    }
                                                                }

                                                                if (hatalivar)
                                                                {
                                                                    goto DonemSecilecek;

                                                                }
                                                                else
                                                                {
                                                                    secilenIptalEdilecekBildirgeGunSayisi = toplamgun;

                                                                    secilenIptalEdilecekBildirgeToplamUcret = toplamtutar;

                                                                    secilenIptalEdilecekBildirgeKisiler = seciliKisiler;
                                                                }

                                                                kisilerinSatirNolari = new Dictionary<string, List<string>>();

                                                                if (seciliKisiler.Count > 0)
                                                                {
                                                                    #region Bildirge Mahiyet ve Belge Türü Seçilecek

                                                                    siradakiIslemBildirgeYukleme = "Bildirge Mahiyet ve Belge Türü Seçilecek";

                                                                BildirgeMahiyetVeTuruSecilecek:

                                                                    response = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tilesislemTamam.action", "action%3AtahakkukbelgeDegisiklikYeniBelgeBilgiIslemleri=Yeni+Belge+%C4%B0%C5%9Flemlerine+Ge%C3%A7".AddToken(html));

                                                                    gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(response, siradakiIslemBildirgeYukleme);

                                                                    if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal)
                                                                    {
                                                                        islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                        continue;
                                                                    }

                                                                    if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz) goto BildirgeMahiyetVeTuruSecilecek;

                                                                    if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto AnaSayfayaGit2;

                                                                    var belgeMahiyetiMahiyetSecme = AsilOlarakEklenecek ? "1" : islemYapilacakIptalBildirgeleri.Count(p => p.Value.Tamamlandi) > 0 || EkOlarakEklenecek ? "2" : SuanYuklenenBildirge.Mahiyet.Equals("ASIL") ? "1" : "2";

                                                                    var belgeTuru = Convert.ToInt32(SuanYuklenenBildirge.BelgeTuru);

                                                                    html.LoadHtml(response);

                                                                    var selectBelgeTurleri = html.GetElementbyId("tahakkukbelgeIslemleri_belge_tur_index");

                                                                    if (selectBelgeTurleri.Descendants("option").Any(p => p.GetAttributeValue("value", "").Equals(belgeTuru.ToString())))
                                                                    {

                                                                        #region Bildirge Kanun Seçilecek

                                                                        siradakiIslemBildirgeYukleme = "Bildirge Kanun Seçilecek";

                                                                    BildirgeKanunSecilecek:

                                                                        response = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukbelgeIslemleri.action", "belgeMahiyeti=" + belgeMahiyetiMahiyetSecme + "&belge_tur_index=" + belgeTuru + "&tahakkukNeden=1003".AddToken(response));

                                                                        gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(response, siradakiIslemBildirgeYukleme);

                                                                        if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal)
                                                                        {
                                                                            islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                            continue;
                                                                        }

                                                                        if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz) goto BildirgeKanunSecilecek;

                                                                        if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto AnaSayfayaGit2;

                                                                        var bildirgeKanun = SuanYuklenenBildirge.Kanun.PadLeft(5, '0');

                                                                        var aranacakDeger = "'" + bildirgeKanun + "':{cevap:''.length == 0";

                                                                        var searchpattern = "'" + bildirgeKanun + "':{cevap:.*mesaj:(.*)}";

                                                                        if (response.Contains(aranacakDeger) || bildirgeKanun == "00000")
                                                                        {
                                                                            #region İptal Edilecek Bildirge Tekrar Seçilecek

                                                                            siradakiIslemBildirgeYukleme = "İptal Edilecek Bildirge Tekrar Seçilecek";

                                                                            var kanunParam = bildirgeKanun;

                                                                            if (bildirgeKanun == "00000") kanunParam = "0";

                                                                            IptalEdilecekBildirgeTekrarSecilecek:
                                                                            response = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukkanunSecildi.action", "kanun_no=" + kanunParam.AddToken(response));

                                                                            //TODO: Alt satırdaki sayfa içeriği "Bu bilgilere sahip bildirge önceden belge değişkiliği olarak verilmiş" olarak kontrol edilmeli belki. Alt satırdaki if kontrolü yanlış olabilir.
                                                                            if (response.Contains("Bu bilgilere sahip bildirge önceden oluşturulmuştur") || response.Contains("Bu bilgilere sahip bildirge önceden belge değişkiliği olarak verilmiş"))
                                                                            {
                                                                                if (islemYapilacakIptalBildirgeleri.Count(p => p.Value.Tamamlandi) == 0 && Program.OncekiBildirgelerIptalEdilsin)
                                                                                {
                                                                                    //TODO: Ay içinde onaylanan bildirgenin kendiliğinden silinmesi durumunda yüklenen bildirge ikinci veya sonraki ise aşağıdaki kodda işleme devam butonu tıklanmayacak. Kendiliğinden silinme hatası devam ediyorsa nedeni alt satırdaki işleme devam tıklanması olabilir.
                                                                                    response = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/.action", "action%3AsgkTahakkukGirisbelgeDegisiklikTahakkuklariGetirYenidenGiris=İşleme+Devam".AddToken(response));
                                                                                }
                                                                                else
                                                                                {

                                                                                    IptalBildirgeHataMesajlariniEkle("\"Bu bilgilere sahip bildirge önceden oluşturulmuştur\" veya \"Bu bilgilere sahip bildirge önceden belge değişkiliği olarak verilmiş\" uyarısı olduğu için sıradaki bildirgeye geçilecek", seciliYuklenecekBildirge, secilenIptalEdilecekBildirgeSira);

                                                                                    islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;

                                                                                    break;
                                                                                }
                                                                            }
                                                                            else if (response.Contains("2018/3 ve önceki dönemler için 01.06.2018 ve önceki tarihlerde yapılmış başvuru kaydınız vardır, toplu iş sözleşmesi kapsamında olan dönemler için sistem alt yapısında gerekli güncellemelerin yapılmasından sonra bildirgeleriniz sisteme yüklenebilecektir"))
                                                                            {

                                                                                YuklenenBildirgeHataMesajiYaz("SGK Hata Mesajı: 2018/3 ve önceki dönemler için 01.06.2018 ve önceki tarihlerde yapılmış başvuru kaydınız vardır, toplu iş sözleşmesi kapsamında olan dönemler için sistem alt yapısında gerekli güncellemelerin yapılmasından sonra bildirgeleriniz sisteme yüklenebilecektir", seciliYuklenecekBildirge);

                                                                                break;
                                                                            }
                                                                            else if (response.Contains("oplu iş sözleşmesi kapsamında olan dönemler için sistem alt yapısında gerekli güncellemelerin yapılmasından sonra bildirgeleriniz sisteme yüklenebilecektir"))
                                                                            {

                                                                                YuklenenBildirgeHataMesajiYaz("SGK Hata Mesajı: Toplu iş sözleşmesi kapsamında olan dönemler için sistem alt yapısında gerekli güncellemelerin yapılmasından sonra bildirgeleriniz sisteme yüklenebilecektir", seciliYuklenecekBildirge);

                                                                                break;
                                                                            }
                                                                            else if (response.Contains("ynı kanun numarasından SGM onayını bekleyen 1.ek belgenizin onaylanmasından sonra işleme devam etmeniz gerekmektedir"))
                                                                            {

                                                                                YuklenenBildirgeHataMesajiYaz("SGK Hata Mesajı: Aynı kanun numarasından SGM onayını bekleyen 1.ek belgenizin onaylanmasından sonra işleme devam etmeniz gerekmektedir", seciliYuklenecekBildirge);

                                                                                break;
                                                                            }

                                                                            gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(response, siradakiIslemBildirgeYukleme);

                                                                            if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal)
                                                                            {
                                                                                islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                                continue;
                                                                            }

                                                                            if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz) goto IptalEdilecekBildirgeTekrarSecilecek;

                                                                            if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto AnaSayfayaGit2;

                                                                            if (response.Contains("Bu Belgenin aslı Daha Önce Girilmiş(Ek belge girişi yapabilirsiniz)"))
                                                                            {
                                                                                EkOlarakEklenecek = true;

                                                                                goto DonemSecilecek;
                                                                            }
                                                                            else if (response.Contains("Bu Belgenin aslı Yok"))
                                                                            {
                                                                                AsilOlarakEklenecek = true;

                                                                                goto DonemSecilecek;
                                                                            }

                                                                            html.LoadHtml(response);


                                                                            var iptalRadios = html.DocumentNode.Descendants("input").Where(i => i.GetAttributeValue("name", "").Equals("bildirgeRefNo"));

                                                                            HtmlAgilityPack.HtmlNode secilecekIptalBildirge = null;

                                                                            foreach (var iptalRadio in iptalRadios)
                                                                            {
                                                                                var tds = iptalRadio.Ancestors("tr").First().Descendants("td");

                                                                                var yilAy = tds.ElementAt(2).InnerText.Trim();

                                                                                var belgeTuruIptal = tds.ElementAt(4).InnerText.Trim();

                                                                                var kanun = tds.ElementAt(6).InnerText.Trim().Split('-')[0];

                                                                                var kisiSayisi = Convert.ToInt32(tds.ElementAt(7).InnerText.Trim());

                                                                                var gun = Convert.ToInt32(tds.ElementAt(8).InnerText.Trim());

                                                                                var toplamucret = tds.ElementAt(9).InnerText.Replace("TL", "").Trim().ToDecimalSgk();

                                                                                if ((SuanYuklenenBildirge.Yil + "/" + SuanYuklenenBildirge.Ay.PadLeft(2, '0')).Equals(yilAy)
                                                                                    && belgeTuruIptal.Equals(SuanYuklenenBildirge.BelgeTuru.PadLeft(2, '0'))
                                                                                    && (secilenIptalEdilecekBildirgeKanunNo.EndsWith(kanun) || secilenIptalEdilecekBildirgeKanunNo == kanun.PadLeft(5, '0'))
                                                                                    && gun.Equals(secilenIptalEdilecekBildirgeGunSayisi)
                                                                                    && Math.Round(toplamucret, 2).Equals(Math.Round(secilenIptalEdilecekBildirgeToplamUcret, 2))
                                                                                    )
                                                                                {
                                                                                    secilecekIptalBildirge = iptalRadio;

                                                                                    break;
                                                                                }

                                                                            }

                                                                            if (secilecekIptalBildirge != null)
                                                                            {
                                                                                var bildirgeRefNo = secilecekIptalBildirge.GetAttributeValue("value", "");

                                                                                if (!HataliSecilenIptalBildirgeleri.Contains(bildirgeRefNo))
                                                                                {

                                                                                    tekrarSecilenIptalBildirgeRefNo = bildirgeRefNo;

                                                                                    #region Bildirge Kişilerin Kontrolü Yapılacak

                                                                                    siradakiIslemBildirgeYukleme = "Bildirge Kişilerin Kontrolü Yapılacak";

                                                                                BildirgeKisilerinKontroluYapilacak:
                                                                                    response = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tilesislemTamam.action", "bildirgeRefNo=" + bildirgeRefNo + "&action%3AtahakkukbelgeDegisiklikIptalBelgeSecimi=Se%C3%A7ili+Belge+%C4%B0%C3%A7in+%C4%B0%C5%9Flem+Yap".AddToken(html));

                                                                                    if (response.Contains("Sigortalıya Ek bildirge verebilmek için, sigortalıyı diğer belgelerden iptal ediniz"))
                                                                                    {
                                                                                        html.LoadHtml(response);

                                                                                        var uyari = html.GetElementbyId("genelUyariCenterTag");

                                                                                        var tcler = Regex.Match(uyari.InnerText.Trim(), "(.*) TC'li").Groups[1].Value.Split(',').Select(p => p.Trim()).ToList();

                                                                                        if (!HataliKisiler.ContainsKey(secilenIptalEdilecekBildirge)) HataliKisiler.Add(secilenIptalEdilecekBildirge, new List<string>());

                                                                                        foreach (var hataliTc in tcler)
                                                                                        {
                                                                                            if (!HataliKisiler[secilenIptalEdilecekBildirge].Contains(hataliTc))
                                                                                            {
                                                                                                HataliKisiler[secilenIptalEdilecekBildirge].Add(hataliTc);

                                                                                            }
                                                                                        }

                                                                                        goto DonemSecilecek;

                                                                                    }

                                                                                    gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(response, siradakiIslemBildirgeYukleme);

                                                                                    if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal)
                                                                                    {
                                                                                        islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                                        continue;
                                                                                    }

                                                                                    if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz) goto BildirgeKisilerinKontroluYapilacak;

                                                                                    if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto AnaSayfayaGit2;

                                                                                    html.LoadHtml(response);

                                                                                    if (response.Contains("TC'ler:"))
                                                                                    {
                                                                                        var hataliTCler = html.GetElementbyId("genelUyariCenterTag").InnerText.Trim().Substring(html.GetElementbyId("genelUyariCenterTag").InnerText.Trim().IndexOf("TC'ler:") + 7).Trim('.').Split(',').Select(p => p.Trim());

                                                                                        bool yeniHataliTcEklendi = false;

                                                                                        if (hataliTCler.Any(h => !string.IsNullOrEmpty(h) && !secilenIptalEdilecekBildirgeKisiler.ContainsKey(h)))
                                                                                        {
                                                                                            if (!HataliSecilenIptalBildirgeleri.Contains(tekrarSecilenIptalBildirgeRefNo))
                                                                                            {
                                                                                                HataliSecilenIptalBildirgeleri.Add(tekrarSecilenIptalBildirgeRefNo);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {

                                                                                            if (hataliTCler.All(p => p.Equals("0")) || hataliTCler.All(p => string.IsNullOrEmpty(p)))
                                                                                            {
                                                                                                if (!BildirgeYuklemeBasvuruFormuIndirildi)
                                                                                                {
                                                                                                    var basvuruformu = Metodlar.FormBul(SuanYapilanIsyeriBildirgeYukleme, Enums.FormTuru.BasvuruFormu);
                                                                                                    if (basvuruformu != null)
                                                                                                    {
                                                                                                        var ds = Metodlar.BasvuruListesiniYukle(basvuruformu);

                                                                                                        if (ds.Tables.Contains("6111"))
                                                                                                        {
                                                                                                            BildirgeYuklemeBasvuruForm6111 = ds.Tables["6111"];
                                                                                                        }
                                                                                                    }

                                                                                                    BildirgeYuklemeBasvuruFormuIndirildi = true;
                                                                                                }

                                                                                                if (BildirgeYuklemeBasvuruForm6111 != null)
                                                                                                {
                                                                                                    var elenecekTcler =
                                                                                                        BildirgeYuklemeBasvuruForm6111
                                                                                                        .AsEnumerable()
                                                                                                        .Where(p => p[Sabitler.BasvuruFormlariSutunlari["6111-v2"][Enums.BasvuruFormuSutunTurleri.OnayDurumu]].ToString().Trim().Equals("İŞTEN ÇIKMIŞ"))
                                                                                                        .Select(p => p[Sabitler.BasvuruFormlariSutunlari["6111-v2"][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString().Trim())
                                                                                                        .Distinct();

                                                                                                    if (elenecekTcler.Count() > 0)
                                                                                                    {
                                                                                                        hataliTCler = elenecekTcler;
                                                                                                    }
                                                                                                }
                                                                                            }

                                                                                            foreach (var hataliTc in hataliTCler)
                                                                                            {
                                                                                                if (!string.IsNullOrEmpty(hataliTc))
                                                                                                {

                                                                                                    if (!HataliKisiler.ContainsKey(secilenIptalEdilecekBildirge)) HataliKisiler.Add(secilenIptalEdilecekBildirge, new List<string>());

                                                                                                    if (!HataliKisiler[secilenIptalEdilecekBildirge].Contains(hataliTc))
                                                                                                    {
                                                                                                        HataliKisiler[secilenIptalEdilecekBildirge].Add(hataliTc);

                                                                                                        yeniHataliTcEklendi = true;
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }

                                                                                        if (hataliTCler.All(p => p.Equals("0")))
                                                                                        {
                                                                                            IptalBildirgeHataMesajlariniEkle("TC'ler: 0 mesajı yazdığından dolayı hatalı kişi tespit edilemediğinden sıradaki iptal bildirgeye geçilecek", seciliYuklenecekBildirge, secilenIptalEdilecekBildirgeSira);

                                                                                            islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                                        }
                                                                                        else if (hataliTCler.All(p => string.IsNullOrEmpty(p)))
                                                                                        {
                                                                                            var genelUyariCenterTag = html.GetElementbyId("genelUyariCenterTag");

                                                                                            IptalBildirgeHataMesajlariniEkle(genelUyariCenterTag.InnerText + "mesajındaki hatalı kişiler tespit edilemediğinden sıradaki iptal bildirgeye geçilecek", seciliYuklenecekBildirge, secilenIptalEdilecekBildirgeSira);

                                                                                            islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (!yeniHataliTcEklendi)
                                                                                            {
                                                                                                var genelUyariCenterTag = html.GetElementbyId("genelUyariCenterTag");

                                                                                                IptalBildirgeHataMesajlariniEkle(genelUyariCenterTag.InnerText + "mesajındaki hatalı kişiler tespit edilemediğinden sıradaki iptal bildirgeye geçilecek", seciliYuklenecekBildirge, secilenIptalEdilecekBildirgeSira);

                                                                                                islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                                            }
                                                                                            else
                                                                                                goto DonemSecilecek;
                                                                                        }
                                                                                    }
                                                                                    else if (html.GetElementbyId("genelUyariCenterTag") != null && html.GetElementbyId("genelUyariCenterTag").InnerText.Contains("6-Kısmi İstihdam"))
                                                                                    {
                                                                                        var hataliTCler = html.GetElementbyId("genelUyariCenterTag").InnerText.Trim().Substring(0, html.GetElementbyId("genelUyariCenterTag").InnerText.Trim().IndexOf("TCK no lu")).Trim().Split(',').Select(p => p.Trim());

                                                                                        if (hataliTCler.Any(h => !secilenIptalEdilecekBildirgeKisiler.ContainsKey(h)))
                                                                                        {
                                                                                            if (!HataliSecilenIptalBildirgeleri.Contains(tekrarSecilenIptalBildirgeRefNo))
                                                                                            {
                                                                                                HataliSecilenIptalBildirgeleri.Add(tekrarSecilenIptalBildirgeRefNo);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            foreach (var hataliTc in hataliTCler)
                                                                                            {
                                                                                                if (!string.IsNullOrEmpty(hataliTc))
                                                                                                {

                                                                                                    if (!HataliKisiler.ContainsKey(secilenIptalEdilecekBildirge)) HataliKisiler.Add(secilenIptalEdilecekBildirge, new List<string>());

                                                                                                    if (!HataliKisiler[secilenIptalEdilecekBildirge].Contains(hataliTc))
                                                                                                    {
                                                                                                        HataliKisiler[secilenIptalEdilecekBildirge].Add(hataliTc);

                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }

                                                                                        goto DonemSecilecek;


                                                                                    }
                                                                                    else if (response.Contains("eksik Gun Sebebi Giriniz"))
                                                                                    {
                                                                                        var hataliTCler = html.GetElementbyId("genelUyariCenterTag").InnerText.Trim().Substring(0, html.GetElementbyId("genelUyariCenterTag").InnerText.Trim().IndexOf("TCK no lu")).Trim().Split(',').Select(p => p.Trim());

                                                                                        if (hataliTCler.Any(h => !secilenIptalEdilecekBildirgeKisiler.ContainsKey(h)))
                                                                                        {
                                                                                            if (!HataliSecilenIptalBildirgeleri.Contains(tekrarSecilenIptalBildirgeRefNo))
                                                                                            {
                                                                                                HataliSecilenIptalBildirgeleri.Add(tekrarSecilenIptalBildirgeRefNo);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            foreach (var hataliTc in hataliTCler)
                                                                                            {
                                                                                                if (!string.IsNullOrEmpty(hataliTc))
                                                                                                {

                                                                                                    if (!HataliKisiler.ContainsKey(secilenIptalEdilecekBildirge)) HataliKisiler.Add(secilenIptalEdilecekBildirge, new List<string>());

                                                                                                    if (!HataliKisiler[secilenIptalEdilecekBildirge].Contains(hataliTc))
                                                                                                    {
                                                                                                        HataliKisiler[secilenIptalEdilecekBildirge].Add(hataliTc);

                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }

                                                                                        goto DonemSecilecek;

                                                                                    }
                                                                                    else if (response.Contains("İşten Ayrılış bildirgesi bulunamadı"))
                                                                                    {

                                                                                        var uyari = html.GetElementbyId("genelUyariCenterTag").InnerText.Trim();

                                                                                        var reg = Regex.Match(uyari, "Sigortalının(.*)tarihli.*");

                                                                                        if (reg.Success)
                                                                                        {
                                                                                            var hatalicikis = Convert.ToDateTime(reg.Groups[1].Value.Trim());
                                                                                            var hatalicikisdeger = hatalicikis.Day.ToString().PadLeft(2, '0') + "/" + hatalicikis.Month.ToString().PadLeft(2, '0');

                                                                                            var hataliTCler = secilenIptalEdilecekBildirgeKisiler.Where(p => p.Value.CikisGunu.Equals(hatalicikisdeger)).Select(p => p.Key);

                                                                                            bool hataliKisiBulundu = false;
                                                                                            var hataliSecilenBildirge = false;

                                                                                            if (hataliTCler.Any(h => !secilenIptalEdilecekBildirgeKisiler.ContainsKey(h)))
                                                                                            {
                                                                                                if (!HataliSecilenIptalBildirgeleri.Contains(tekrarSecilenIptalBildirgeRefNo))
                                                                                                {
                                                                                                    HataliSecilenIptalBildirgeleri.Add(tekrarSecilenIptalBildirgeRefNo);
                                                                                                    hataliSecilenBildirge = true;
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                foreach (var hataliTc in hataliTCler)
                                                                                                {
                                                                                                    if (!string.IsNullOrEmpty(hataliTc))
                                                                                                    {

                                                                                                        if (!HataliKisiler.ContainsKey(secilenIptalEdilecekBildirge)) HataliKisiler.Add(secilenIptalEdilecekBildirge, new List<string>());

                                                                                                        if (!HataliKisiler[secilenIptalEdilecekBildirge].Contains(hataliTc))
                                                                                                        {
                                                                                                            HataliKisiler[secilenIptalEdilecekBildirge].Add(hataliTc);

                                                                                                            hataliKisiBulundu = true;
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }

                                                                                            if (hataliKisiBulundu || hataliSecilenBildirge)
                                                                                                goto DonemSecilecek;
                                                                                            else
                                                                                            {
                                                                                                kisilerinSatirNolari = new Dictionary<string, List<string>>();

                                                                                                IptalBildirgeHataMesajlariniEkle("İşten Ayrılış bildirgesi bulunamadı uyarısı var fakat hata veren kişi bulunamadığı için sıradaki bildirgeye geçilecek", seciliYuklenecekBildirge, secilenIptalEdilecekBildirgeSira);

                                                                                                islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                                            }


                                                                                        }
                                                                                    }
                                                                                    else if (response.Contains("işe Giriş bildirgesi bulunamadı"))
                                                                                    {

                                                                                        var uyari = html.GetElementbyId("genelUyariCenterTag").InnerText.Trim();

                                                                                        var reg = Regex.Match(uyari, "Sigortalının(.*)tarihli.*");

                                                                                        if (reg.Success)
                                                                                        {
                                                                                            var hataligiris = Convert.ToDateTime(reg.Groups[1].Value.Trim());
                                                                                            var hataligirisdeger = hataligiris.Day.ToString().PadLeft(2, '0') + "/" + hataligiris.Month.ToString().PadLeft(2, '0');

                                                                                            var hataliTCler = secilenIptalEdilecekBildirgeKisiler.Where(p => p.Value.GirisGunu.Equals(hataligirisdeger)).Select(p => p.Key);

                                                                                            if (hataliTCler.Any(h => !secilenIptalEdilecekBildirgeKisiler.ContainsKey(h)))
                                                                                            {
                                                                                                if (!HataliSecilenIptalBildirgeleri.Contains(tekrarSecilenIptalBildirgeRefNo))
                                                                                                {
                                                                                                    HataliSecilenIptalBildirgeleri.Add(tekrarSecilenIptalBildirgeRefNo);
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                foreach (var hataliTc in hataliTCler)
                                                                                                {
                                                                                                    if (!string.IsNullOrEmpty(hataliTc))
                                                                                                    {

                                                                                                        if (!HataliKisiler.ContainsKey(secilenIptalEdilecekBildirge)) HataliKisiler.Add(secilenIptalEdilecekBildirge, new List<string>());

                                                                                                        if (!HataliKisiler[secilenIptalEdilecekBildirge].Contains(hataliTc))
                                                                                                        {
                                                                                                            HataliKisiler[secilenIptalEdilecekBildirge].Add(hataliTc);

                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }

                                                                                            goto DonemSecilecek;

                                                                                        }
                                                                                    }
                                                                                    else if (response.Contains("eksik Gün Hatalı Girildi") || response.Contains("eksik Gun Hatalı Girildi"))
                                                                                    {

                                                                                        var uyari = html.GetElementbyId("genelUyariCenterTag").InnerText.Trim();

                                                                                        var reg = Regex.Match(uyari, "(.*)TCK no.*");

                                                                                        if (reg.Success)
                                                                                        {
                                                                                            var hataliTCler = reg.Groups[1].Value.Trim().Split(',').Select(p => p.Trim()).ToList();

                                                                                            if (hataliTCler.Any(h => !secilenIptalEdilecekBildirgeKisiler.ContainsKey(h)))
                                                                                            {
                                                                                                if (!HataliSecilenIptalBildirgeleri.Contains(tekrarSecilenIptalBildirgeRefNo))
                                                                                                {
                                                                                                    HataliSecilenIptalBildirgeleri.Add(tekrarSecilenIptalBildirgeRefNo);
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                foreach (var hataliTc in hataliTCler)
                                                                                                {
                                                                                                    if (!string.IsNullOrEmpty(hataliTc))
                                                                                                    {

                                                                                                        if (!HataliKisiler.ContainsKey(secilenIptalEdilecekBildirge)) HataliKisiler.Add(secilenIptalEdilecekBildirge, new List<string>());

                                                                                                        if (!HataliKisiler[secilenIptalEdilecekBildirge].Contains(hataliTc))
                                                                                                        {
                                                                                                            HataliKisiler[secilenIptalEdilecekBildirge].Add(hataliTc);

                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }

                                                                                            goto DonemSecilecek;

                                                                                        }
                                                                                    }
                                                                                    else if (Regex.IsMatch(response, ".*Sigortalının Toplam Tutarı .*dan Az Olamaz"))
                                                                                    {

                                                                                        if (SuanYuklenenBildirge.BelgeTuru == "4" || SuanYuklenenBildirge.BelgeTuru == "35")
                                                                                        {

                                                                                            var uyari = html.GetElementbyId("genelUyariCenterTag").InnerText.Trim();

                                                                                            var asgariucret = Metodlar.AsgariUcretBul(Yil, Ay);

                                                                                            var hataliTCler = secilenIptalEdilecekBildirgeKisiler.Where(p => (p.Value.Ucret.ToDecimalSgk() + p.Value.Ikramiye.ToDecimalSgk()) < (p.Value.Gun.ToInt() * Convert.ToDecimal(asgariucret) * 2)).Select(p => p.Key);

                                                                                            if (hataliTCler.Any(h => !secilenIptalEdilecekBildirgeKisiler.ContainsKey(h)))
                                                                                            {
                                                                                                if (!HataliSecilenIptalBildirgeleri.Contains(tekrarSecilenIptalBildirgeRefNo))
                                                                                                {
                                                                                                    HataliSecilenIptalBildirgeleri.Add(tekrarSecilenIptalBildirgeRefNo);
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                foreach (var hataliTc in hataliTCler)
                                                                                                {
                                                                                                    if (!string.IsNullOrEmpty(hataliTc))
                                                                                                    {

                                                                                                        if (!HataliKisiler.ContainsKey(secilenIptalEdilecekBildirge)) HataliKisiler.Add(secilenIptalEdilecekBildirge, new List<string>());

                                                                                                        if (!HataliKisiler[secilenIptalEdilecekBildirge].Contains(hataliTc))
                                                                                                        {
                                                                                                            HataliKisiler[secilenIptalEdilecekBildirge].Add(hataliTc);

                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }

                                                                                            goto DonemSecilecek;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            IptalBildirgeHataMesajlariniEkle("Sigortalının Toplam Tutarı ...'dan Az Olamaz hatası olduğu için sıradaki iptal bildirgeye geçilecek", seciliYuklenecekBildirge, secilenIptalEdilecekBildirgeSira);

                                                                                            islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                                        }

                                                                                    }
                                                                                    else if (response.Contains("gun Sayısını Hatalı Girildi"))
                                                                                    {

                                                                                        var uyari = html.GetElementbyId("genelUyariCenterTag").InnerText.Trim();

                                                                                        var reg = Regex.Match(uyari, "(.*)TCK no.*");

                                                                                        if (reg.Success)
                                                                                        {
                                                                                            var hataliTCler = reg.Groups[1].Value.Trim().Split(',').Select(p => p.Trim()).ToList();

                                                                                            if (hataliTCler.Any(h => !secilenIptalEdilecekBildirgeKisiler.ContainsKey(h)))
                                                                                            {
                                                                                                if (!HataliSecilenIptalBildirgeleri.Contains(tekrarSecilenIptalBildirgeRefNo))
                                                                                                {
                                                                                                    HataliSecilenIptalBildirgeleri.Add(tekrarSecilenIptalBildirgeRefNo);
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                foreach (var hataliTc in hataliTCler)
                                                                                                {
                                                                                                    if (!string.IsNullOrEmpty(hataliTc))
                                                                                                    {

                                                                                                        if (!HataliKisiler.ContainsKey(secilenIptalEdilecekBildirge)) HataliKisiler.Add(secilenIptalEdilecekBildirge, new List<string>());

                                                                                                        if (!HataliKisiler[secilenIptalEdilecekBildirge].Contains(hataliTc))
                                                                                                        {
                                                                                                            HataliKisiler[secilenIptalEdilecekBildirge].Add(hataliTc);

                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }

                                                                                            goto DonemSecilecek;

                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {

                                                                                        var tableKisilerinKontroluSon = html.DocumentNode.Descendants().FirstOrDefault(t => t.GetAttributeValue("class", "").Equals("gradienttable"));

                                                                                        var trsKisilerinKontroluSon = tableKisilerinKontroluSon.Descendants("tr");

                                                                                        hatalivar = false;

                                                                                        toplamIcmal = 0;

                                                                                        toplamGun = 0;

                                                                                        toplamUcret = 0;


                                                                                        List<string> kisiler = new List<string>();

                                                                                        for (int i = 1; i < trsKisilerinKontroluSon.Count(); i++)
                                                                                        {
                                                                                            if (!trsKisilerinKontroluSon.ElementAt(i).Descendants("span").All(s => s.GetAttributeValue("style", "").Equals("background-color:#bbffbb")))
                                                                                            {
                                                                                                var hataliTc = trsKisilerinKontroluSon.ElementAt(i).Descendants("td").ElementAt(2).InnerText;

                                                                                                if (!HataliKisiler.ContainsKey(secilenIptalEdilecekBildirge)) HataliKisiler.Add(secilenIptalEdilecekBildirge, new List<string>());

                                                                                                if (!HataliKisiler[secilenIptalEdilecekBildirge].Contains(hataliTc))
                                                                                                {
                                                                                                    HataliKisiler[secilenIptalEdilecekBildirge].Add(hataliTc);

                                                                                                    hatalivar = true;
                                                                                                }

                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                var kisiToplamGun = Convert.ToInt32(trsKisilerinKontroluSon.ElementAt(i).Descendants("td").ElementAt(6).InnerText.Trim());
                                                                                                var kisiToplamUcret = trsKisilerinKontroluSon.ElementAt(i).Descendants("td").ElementAt(10).InnerText.Trim().ToDecimalSgk();

                                                                                                var TcNo = trsKisilerinKontroluSon.ElementAt(i).Descendants("td").ElementAt(2).InnerText.Trim();

                                                                                                var kisi = SuanYuklenenBildirge.Kisiler.FirstOrDefault(k => k.SosyalGuvenlikNo.Equals(TcNo) && k.Kanun.Equals(secilenIptalEdilecekBildirgeKanunNo));

                                                                                                var tesvikHesaplanacakGun = kisiToplamGun;
                                                                                                var tesvikHesaplanacakToplamUcret = kisiToplamUcret;
                                                                                                var donusecekHesaplananGun = kisiToplamGun;
                                                                                                var donusecekToplamUcret = kisiToplamUcret;

                                                                                                if (kisi != null)
                                                                                                {
                                                                                                    var tesvikKanunNo = kisi.TesvikKanunNo;
                                                                                                    var iptalKanunNo = kisi.Kanun;

                                                                                                    if (kisiToplamGun < kisi.HesaplananGun.ToInt())
                                                                                                    {
                                                                                                        kisi.TesvikHesaplanacakGun = kisiToplamGun.ToString();
                                                                                                    }

                                                                                                    if (kisiToplamGun < kisi.HesaplananDonusecekGun.ToInt())
                                                                                                    {
                                                                                                        kisi.DonusturulecekHesaplanacakGun = kisiToplamGun.ToString();
                                                                                                    }

                                                                                                    tesvikHesaplanacakGun = kisi.HesaplananGun.ToInt();
                                                                                                    tesvikHesaplanacakToplamUcret = kisi.HesaplananToplamUcret;
                                                                                                    donusecekHesaplananGun = kisi.HesaplananDonusecekGun.ToInt();
                                                                                                    donusecekToplamUcret = kisi.HesaplananDonusecekToplamUcret;
                                                                                                }


                                                                                                toplamGun += kisiToplamGun;

                                                                                                toplamUcret += kisiToplamUcret;

                                                                                                CarpimOrani687 = TesvikHesaplamaSabitleri.CarpimOrani687;

                                                                                                if (!string.IsNullOrEmpty(SuanYuklenenBildirge.EkBilgiler) && SuanYuklenenBildirge.EkBilgiler.Contains("CarpimOrani687"))
                                                                                                {
                                                                                                    var deger = SuanYuklenenBildirge.EkBilgiler.Trim(';').Split(';').FirstOrDefault(p => p.Contains("CarpimOrani687"));

                                                                                                    if (!string.IsNullOrEmpty(deger)) CarpimOrani687 = Convert.ToDecimal(deger.Split('=')[1]);
                                                                                                }
                                                                                                else
                                                                                                {

                                                                                                    if (SuanYuklenenBildirge.Kanun.PadLeft(5, '0').Equals("00687"))
                                                                                                    {
                                                                                                        if (!BildirgelereBakildi687Icin)
                                                                                                        {
                                                                                                            string responseOnaylanmisBildirgeler = BildirgeWebClient.Get("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukonaylanmisTahakkukDonemBilgileriniYukle.action", string.Empty);

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

                                                                                                                    responseOnaylanmisBildirgeler = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukonaylanmisTahakkukDonemSecildi.action", PostData);

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

                                                                                                                            if (!bildirgeVarmi2016Yilinda)
                                                                                                                            {
                                                                                                                                IkiyeBolunecek687TesvikTutari = true;
                                                                                                                            }
                                                                                                                        }

                                                                                                                        BildirgelereBakildi687Icin = true;
                                                                                                                    }
                                                                                                                    else if (responseOnaylanmisBildirgeler.Contains("Onaylı Bildirge Bulunamadı"))
                                                                                                                    {
                                                                                                                        BildirgelereBakildi687Icin = true;
                                                                                                                    }
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    BildirgelereBakildi687Icin = true;
                                                                                                                }
                                                                                                            }

                                                                                                            if (BildirgelereBakildi687Icin)
                                                                                                            {
                                                                                                                if (SuanYuklenenBildirge.Kanun.PadLeft(5, '0').Equals("00687"))
                                                                                                                {
                                                                                                                    CarpimOrani687 = IkiyeBolunecek687TesvikTutari ? TesvikHesaplamaSabitleri.CarpimOrani687 / 2 : TesvikHesaplamaSabitleri.CarpimOrani687;
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }

                                                                                                toplamIcmal += Metodlar.TesvikTutariHesapla(SuanYuklenenBildirge.Kanun, tesvikHesaplanacakGun, tesvikHesaplanacakToplamUcret, Yil, Ay, SuanYuklenenBildirge.BelgeTuru, SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo, null, CarpimOrani687);

                                                                                                tesvik = Program.TumTesvikler.FirstOrDefault(p => SuanYuklenenBildirge.Kanun.Equals(p.Key.PadLeft(5, '0')) || p.Value.AltKanunlar.Contains(SuanYuklenenBildirge.Kanun.PadLeft(5, '0'))).Value;

                                                                                                var DonusturulenKanun = secilenIptalEdilecekBildirgeKanunNo;

                                                                                                if (tesvik != null)
                                                                                                {
                                                                                                    var dk = tesvik.DonusturulecekKanunlar.FirstOrDefault(p => p.Key.EndsWith(DonusturulenKanun)).Value;

                                                                                                    if (dk != null)
                                                                                                    {
                                                                                                        if (TesvikHesaplamaSabitleri.BagliKanunlarIcmalHesaplama.ContainsKey(SuanYuklenenBildirge.Kanun) && TesvikHesaplamaSabitleri.BagliKanunlarIcmalHesaplama[SuanYuklenenBildirge.Kanun].Contains("05510"))
                                                                                                        {
                                                                                                            if (DonusturulenKanun.Equals("00000"))
                                                                                                            {
                                                                                                                toplamIcmal += tesvikHesaplanacakGun * Metodlar.AsgariUcretBul(Yil, Ay) * 0.05m;
                                                                                                            }
                                                                                                            else toplamIcmal += Metodlar.TesvikTutariHesapla("05510", tesvikHesaplanacakGun, tesvikHesaplanacakToplamUcret, Yil, Ay, SuanYuklenenBildirge.BelgeTuru, SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo);
                                                                                                        }

                                                                                                        var dusulecekTutar = DonusturulecekKanun.DusulecekMiktarHesapla(dk.DonusturulecekKanunNo, donusecekHesaplananGun, donusecekToplamUcret, Yil, Ay, SuanYuklenenBildirge.BelgeTuru, SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo, tesvik.DonusenlerIcmaldenDusulsun, null, CarpimOrani687)[dk.DonusturulecekKanunNo].BagliKanunlarDahilDusulecekTutar;

                                                                                                        toplamIcmal -= dusulecekTutar;
                                                                                                    }
                                                                                                }
                                                                                                else if (SuanYuklenenBildirge.Kanun.Equals("00000"))
                                                                                                {

                                                                                                    var dusulecekTutar = DonusturulecekKanun.DusulecekMiktarHesapla(DonusturulenKanun, donusecekHesaplananGun, donusecekToplamUcret, Yil, Ay, SuanYuklenenBildirge.BelgeTuru, SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo, true, null, CarpimOrani687)[DonusturulenKanun].BagliKanunlarDahilDusulecekTutar;

                                                                                                    toplamIcmal -= dusulecekTutar;
                                                                                                }


                                                                                                kisiler.Add(TcNo);
                                                                                            }
                                                                                        }

                                                                                        if (hatalivar)
                                                                                        {
                                                                                            goto DonemSecilecek;

                                                                                        }
                                                                                        else
                                                                                        {

                                                                                            islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tutar = toplamIcmal;
                                                                                            islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].PrimOdenenGunSayisi = toplamGun;
                                                                                            islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Matrah = toplamUcret;
                                                                                            islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Kanun = SuanYuklenenBildirge.Kanun;
                                                                                            islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].yilay = new KeyValuePair<string, string>(SuanYuklenenBildirge.Yil, SuanYuklenenBildirge.Ay);
                                                                                            islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].iptalKisiler = kisiler;
                                                                                        }

                                                                                        islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].basariliKisiSayisi = trsKisilerinKontroluSon.Count() - 1;

                                                                                        #region Onaylanacaklar Ekranı

                                                                                        siradakiIslemBildirgeYukleme = "Onaylanacaklar Ekranı";

                                                                                    OnaylanacaklarEkrani:
                                                                                        response = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tilesislemTamam.action", "action%3AtahakkukonayBekleyenTahakkuklar=Onayla+Ekran%C4%B1na+Ge%C3%A7".AddToken(html));

                                                                                        gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(response, siradakiIslemBildirgeYukleme);

                                                                                        if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal)
                                                                                        {
                                                                                            islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                                            continue;
                                                                                        }

                                                                                        if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz) goto OnaylanacaklarEkrani;

                                                                                        if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto AnaSayfayaGit2;

                                                                                        html.LoadHtml(response);

                                                                                        trs = html.GetElementbyId("onayBekleyenTahakkuklarForm").Descendants("table").FirstOrDefault().Descendants("tr").Skip(1).ToList();

                                                                                        bool bulundu = false;

                                                                                        HtmlAgilityPack.HtmlNode trYuklenenBildirge = null;

                                                                                        foreach (var tr in trs)
                                                                                        {
                                                                                            var tds = tr.Descendants("td");

                                                                                            if (tds.ElementAt(3).InnerText.Trim().Equals(SuanYuklenenBildirge.Yil + "/" + SuanYuklenenBildirge.Ay.PadLeft(2, '0'))
                                                                                              && tds.ElementAt(4).InnerText.Trim().Equals(SuanYuklenenBildirge.BelgeTuru.PadLeft(2, '0'))
                                                                                              && tds.ElementAt(5).InnerText.Trim().Equals(AsilOlarakEklenecek ? "ASIL" : islemYapilacakIptalBildirgeleri.Count(p => p.Value.Tamamlandi) > 0 || EkOlarakEklenecek ? "EK" : SuanYuklenenBildirge.Mahiyet)
                                                                                              && (tds.ElementAt(6).InnerText.Trim().StartsWith(SuanYuklenenBildirge.Kanun.PadLeft(5, '0')) || (SuanYuklenenBildirge.Kanun == "00000" && tds.ElementAt(6).InnerText.Trim() == ""))
                                                                                              && tds.ElementAt(7).InnerText.Trim().Equals(islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].basariliKisiSayisi.ToString())
                                                                                              && tds.ElementAt(8).InnerText.Trim().Equals(islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].PrimOdenenGunSayisi.ToString())
                                                                                              && Math.Round(tds.ElementAt(9).InnerText.Replace("TL", "").Trim().ToDecimalSgk(), 2).Equals(Math.Round(islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Matrah, 2))
                                                                                              )
                                                                                            {
                                                                                                bulundu = true;

                                                                                                trYuklenenBildirge = tr;

                                                                                                break;

                                                                                            }
                                                                                        }

                                                                                        if (bulundu)
                                                                                        {
                                                                                            var tds = trYuklenenBildirge.Descendants("td");

                                                                                            if (!AylarIslemDevamYeniOlusturListesi.Contains(SuanYuklenenBildirge.Yil + "/" + SuanYuklenenBildirge.Ay.PadLeft(2, '0')))
                                                                                            {
                                                                                                AylarIslemDevamYeniOlusturListesi.Add(SuanYuklenenBildirge.Yil + "/" + SuanYuklenenBildirge.Ay.PadLeft(2, '0'));
                                                                                            }

                                                                                            bool bildirgeBasariliYuklendi = false;

                                                                                            if (!Program.BildirgelerOnaylansin)
                                                                                            {
                                                                                                bildirgeBasariliYuklendi = true;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                var radio = tds.ElementAt(0).Descendants("input").FirstOrDefault(p => p.GetAttributeValue("type", "").Equals("radio"));

                                                                                                var onclick = radio.GetAttributeValue("onclick", "");

                                                                                                var bildirgeNo = onclick.Split(',')[3].Trim();

                                                                                                var iptalBildirge = trs.FirstOrDefault(t => t.InnerHtml.Contains("," + bildirgeNo + ",") && !t.Equals(trYuklenenBildirge));

                                                                                                var tdler = iptalBildirge.Descendants("td");

                                                                                                var iptalradio = tdler.ElementAt(0).Descendants("input").FirstOrDefault(p => p.GetAttributeValue("type", "").Equals("radio"));

                                                                                                OnaylanacakBildirgeRefNo = iptalradio.GetAttributeValue("value", "");

                                                                                            #region Bildirge Onaylanacak

                                                                                            BildirgeOnaylanacak:
                                                                                                siradakiIslemBildirgeYukleme = "Bildirge Onaylanacak";

                                                                                                response = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tilesislemTamam.action", "bildirgeRefNo=" + OnaylanacakBildirgeRefNo.Replace(" ", "+") + "&action%3Atahakkukonayla=Bildirge+Onayla&download=true".AddToken(html));

                                                                                                gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(response, siradakiIslemBildirgeYukleme);

                                                                                                if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal)
                                                                                                {
                                                                                                    islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                                                    continue;
                                                                                                }

                                                                                                if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz) goto BildirgeOnaylanacak;

                                                                                                if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto BildirgeOnaylanacak;

                                                                                                html.LoadHtml(response);

                                                                                            #region BildirgeOnaylandi

                                                                                            BildirgeOnaylandi:

                                                                                                siradakiIslemBildirgeYukleme = "Bildirge Onaylandı";

                                                                                                response = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tilesislemTamam.action", "bildirgeRefNo=" + OnaylanacakBildirgeRefNo + "&onayChk=true&__checkbox_onayChk=true&isyeri_sifre=" + SuanYapilanIsyeriBildirgeYukleme.IsyeriSifresi + "&action%3AtahakkukonaylaInternet=Onayla".AddToken(html));

                                                                                                gecerliSayfaKontrolu = GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(response, siradakiIslemBildirgeYukleme);

                                                                                                if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Iptal)
                                                                                                {
                                                                                                    islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;

                                                                                                    continue;
                                                                                                }

                                                                                                if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.Gecersiz) goto BildirgeOnaylandi;

                                                                                                if (gecerliSayfaKontrolu == Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin) goto BildirgeOnaylandi;

                                                                                                if (response.Contains("İşleminiz Başarılı Bir Şekilde Tamamlanmıştır") && response.Contains("Aylık Prim Hizmet Belgesi Girişi"))
                                                                                                {
                                                                                                    basariliYuklenenVarMi = true;
                                                                                                    bildirgeBasariliYuklendi = true;
                                                                                                }
                                                                                                else if (yenidenYuklemeDenemeSayisiBildirgeYukleme >= 3)
                                                                                                {
                                                                                                    yenidenYuklemeDenemeSayisiBildirgeYukleme++;

                                                                                                    if (yenidenYuklemeDenemeSayisiBildirgeYukleme < 5) goto BildirgeOnaylandi;
                                                                                                    else
                                                                                                    {
                                                                                                        IptalBildirgeHataMesajlariniEkle("5 denemeye rağmen bildirge onaylanamadı. Sıradaki bildirgeye geçiliyor", seciliYuklenecekBildirge, secilenIptalEdilecekBildirgeSira);
                                                                                                    }

                                                                                                }




                                                                                                #endregion


                                                                                                #endregion

                                                                                            }

                                                                                            if (bildirgeBasariliYuklendi)
                                                                                            {

                                                                                                if (islemYapilacakIptalBildirgeleri.ContainsKey(secilenIptalEdilecekBildirge))
                                                                                                {
                                                                                                    if (!islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi)
                                                                                                    {
                                                                                                        islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;

                                                                                                        islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Basarili = true;

                                                                                                        var iptalistatistik = islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge];

                                                                                                        var toplamIptalBildirgeSayisi = islemYapilacakIptalBildirgeleri.Count;
                                                                                                        var tamamlananIptalBildirgeSayisi = islemYapilacakIptalBildirgeleri.Count(p => p.Value.Tamamlandi);
                                                                                                        var islemtarihi = iptalistatistik.IslemTarihi;
                                                                                                        var mahiyet = iptalistatistik.Mahiyet;
                                                                                                        var iptalKanun = iptalistatistik.IptalKanun;

                                                                                                        YuklenenIcmaller[seciliYuklenecekBildirge].Tutar += iptalistatistik.Tutar;
                                                                                                        YuklenenIcmaller[seciliYuklenecekBildirge].PrimOdenenGunSayisi += iptalistatistik.PrimOdenenGunSayisi;
                                                                                                        YuklenenIcmaller[seciliYuklenecekBildirge].Matrah += iptalistatistik.Matrah;
                                                                                                        YuklenenIcmaller[seciliYuklenecekBildirge].Kanun = SuanYuklenenBildirge.Kanun;
                                                                                                        YuklenenIcmaller[seciliYuklenecekBildirge].yilay = new KeyValuePair<string, string>(SuanYuklenenBildirge.Yil, SuanYuklenenBildirge.Ay);

                                                                                                        if (!YuklenenIcmaller[seciliYuklenecekBildirge].Kisiler.ContainsKey(iptalKanun)) YuklenenIcmaller[seciliYuklenecekBildirge].Kisiler.Add(iptalKanun, iptalistatistik.iptalKisiler);
                                                                                                        else
                                                                                                        {
                                                                                                            YuklenenIcmaller[seciliYuklenecekBildirge].Kisiler[iptalKanun].AddRange(iptalistatistik.iptalKisiler);
                                                                                                        }


                                                                                                        basariliEklenenSayisi += iptalistatistik.basariliKisiSayisi;

                                                                                                        if (!Program.BildirgelerOnaylansin)
                                                                                                        {
                                                                                                            Mesaj = String.Format("({0}/{1}) {2} işlem tarihli {3} {4}'dan dönüştürülen bildirge onaylanmaya hazır bir şekilde yüklendi. Başarılı eklenen sayısı={5}/{6}", tamamlananIptalBildirgeSayisi, toplamIptalBildirgeSayisi, islemtarihi, mahiyet, iptalKanun, iptalistatistik.basariliKisiSayisi, iptalistatistik.bulunanKisiSayisi);
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            Mesaj = String.Format("({0}/{1}) {2} işlem tarihli {3} {4}'dan dönüştürülen bildirge başarıyla onaylandı. Başarılı eklenen sayısı={5}/{6}", tamamlananIptalBildirgeSayisi, toplamIptalBildirgeSayisi, islemtarihi, mahiyet, iptalKanun, iptalistatistik.basariliKisiSayisi, iptalistatistik.bulunanKisiSayisi);
                                                                                                        }

                                                                                                        MesajYaz(Mesaj);

                                                                                                        if (HataliKisiler.ContainsKey(secilenIptalEdilecekBildirge))
                                                                                                        {
                                                                                                            MesajYaz("Hatalı kişi sayısı =" + HataliKisiler[secilenIptalEdilecekBildirge].Count());
                                                                                                            MesajYaz("Hatalı kişiler: " + String.Join(",", HataliKisiler[secilenIptalEdilecekBildirge]));
                                                                                                            //IptalBildirgeHataMesajlariniEkle("Hatalı kişi sayısı =" + HataliKisiler[secilenIptalEdilecekBildirge].Count(), seciliYuklenecekBildirge, secilenIptalEdilecekBildirgeSira);

                                                                                                            //IptalBildirgeHataMesajlariniEkle("Hatalı kişiler: " + String.Join(",", HataliKisiler[secilenIptalEdilecekBildirge]), seciliYuklenecekBildirge, secilenIptalEdilecekBildirgeSira);

                                                                                                        }

                                                                                                        BildirgeYuklemeIcmalOlustur(false);

                                                                                                    }
                                                                                                }


                                                                                            }

                                                                                        }
                                                                                        else //if (bulundu)
                                                                                        {
                                                                                            IptalBildirgeHataMesajlariniEkle("Yüklenen bildirge onaylanacaklar ekranında bulunamadı", seciliYuklenecekBildirge, secilenIptalEdilecekBildirgeSira);

                                                                                            islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;

                                                                                        }


                                                                                        #endregion
                                                                                    }


                                                                                    #endregion

                                                                                }

                                                                            }


                                                                            #endregion


                                                                        }
                                                                        else
                                                                        {
                                                                            var match = Regex.Match(response, searchpattern);
                                                                            var msg = match.Groups[1].Value.Trim('\'');

                                                                            YuklenenBildirgeHataMesajiYaz("SGK Hata Mesajı: " + msg, seciliYuklenecekBildirge);

                                                                            break;

                                                                        }

                                                                        #endregion

                                                                    }
                                                                    else
                                                                    {
                                                                        YuklenenBildirgeHataMesajiYaz("Yüklenmek istenen belge numarası belge türünü seçme sayfasında mevcut olmadığı için sıradaki bildirgeye geçilecek", seciliYuklenecekBildirge);

                                                                        break;
                                                                    }
                                                                    #endregion
                                                                }
                                                                else
                                                                {
                                                                    kisilerinSatirNolari = new Dictionary<string, List<string>>();

                                                                    IptalBildirgeHataMesajlariniEkle("Kişilerin tümü hatalı olduğu için veya iptal edilebilecekler arasında olmadığı için sıradaki bildirgeye geçilecek", seciliYuklenecekBildirge, secilenIptalEdilecekBildirgeSira);

                                                                    islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                                }

                                                                #endregion

                                                            }
                                                            else
                                                            {
                                                                kisilerinSatirNolari = new Dictionary<string, List<string>>();

                                                                IptalBildirgeHataMesajlariniEkle("Kişilerin tümü hatalı olduğu için veya iptal edilebilecekler arasında olmadığı için sıradaki bildirgeye geçilecek", seciliYuklenecekBildirge, secilenIptalEdilecekBildirgeSira);

                                                                islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge].Tamamlandi = true;
                                                            }
                                                        }
                                                    }
                                                }

                                            }

                                            #endregion
                                        }
                                        else
                                        {
                                            YuklenenBildirgeHataMesajiYaz("Aynı Kanun/Belge numarası ile sisteme kaydedilmiş ve sosyal güvenlik il müdürlüğünce / sosyal güvenlik merkezince işlemi devam eden IPTAL belge bulunmaktadır.Söz konusu belgenin işlemleri tamamlanmadan yeni bir IPTAL belge gönderilemez", seciliYuklenecekBildirge);
                                        }

                                    }
                                }

                                #endregion
                            }
                            else
                            {
                                YuklenenBildirgeHataMesajiYaz("Belge mahiyeti seçme kutucuğunda IPTAL seçeneği olmadığından sıradaki bildirgeye geçilecek", seciliYuklenecekBildirge);
                            }

                            #endregion
                        }
                        else
                        {
                            YuklenenBildirgeHataMesajiYaz("Bildirgenin ait olduğu ay bilgisi sistemde gözükmediğinden sıradaki bildirgeye geçiliyor", seciliYuklenecekBildirge);
                        }
                        #endregion
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(hataMesaji))
                        {
                            YuklenenBildirgeHataMesajiYaz(hataMesaji, seciliYuklenecekBildirge);
                        }

                    }

                    if (islemYapilacakIptalBildirgeleri.Count(p => !p.Value.Tamamlandi) == 0 && islemYapilacakIptalBildirgeleri.Any(p => p.Value.Basarili) && YuklenenIcmaller[seciliYuklenecekBildirge].Tutar > 0)
                    {
                        var iptaller = String.Join(",", islemYapilacakIptalBildirgeleri.Where(p => p.Value.Basarili).Select(p => p.Value.Mahiyet + " " + p.Value.IptalKanun).ToList());

                        if (!Program.BildirgelerOnaylansin)
                        {
                            Mesaj = System.IO.Path.GetFileNameWithoutExtension(YuklenecekBildirgeler[seciliYuklenecekBildirge]) + " bildirgesi " + iptaller + " " + (iptaller.Contains(",") ? "kanunlarından" : "kanunundan") + " dönüştürülerek onaylanmaya hazır bir şekilde yüklendi. ";
                        }
                        else
                        {
                            Mesaj = System.IO.Path.GetFileNameWithoutExtension(YuklenecekBildirgeler[seciliYuklenecekBildirge]) + " bildirgesi " + iptaller + " " + (iptaller.Contains(",") ? "kanunlarından" : "kanunundan") + " dönüştürülerek başarıyla onaylandı. ";
                        }

                        int kisisayisi = SuanYuklenenBildirge.Kisiler.Select(k => k.SosyalGuvenlikNo + "-" + k.Kanun).Distinct().Count();

                        Mesaj += "Başarılı eklenen kişi sayısı = " + basariliEklenenSayisi + "/" + kisisayisi;

                        MesajYaz(Mesaj);

                        if (basariliEklenenSayisi < kisisayisi)
                        {
                            YuklenenBildirgeHataMesajiYaz("EKSİK GİRİLEN KİŞİ SAYISI : " + (kisisayisi - basariliEklenenSayisi), seciliYuklenecekBildirge);

                            YuklenenBildirgeHataMesajiYaz("GİRİLEMEYEN KİŞİLER :" + String.Join(",", SuanYuklenenBildirge.Kisiler.Where(p => !islemYapilacakIptalBildirgeleri.Any(t => t.Value.iptalKisiler != null && t.Value.iptalKisiler.Contains(p.SosyalGuvenlikNo) && t.Value.IptalKanun.Equals(p.Kanun))).GroupBy(p => p.Kanun).Select(x => x.Key + "(" + String.Join(",", x.Select(k => k.SosyalGuvenlikNo).Distinct()) + ")")), seciliYuklenecekBildirge);

                        }
                    }

                    MesajYaz(new string('-', 280));

                }

                try
                {
                    OnaydaBekleyenleriSil();
                }
                catch { }

                throw new IslemTamamException(siradakiIslemBildirgeYukleme);
            }
            catch (IslemTamamException)
            {
                if (basariliYuklenenVarMi)
                {
                    MesajYaz("Başvuru yapılacak");
                    var basvuruSonuc = Metodlar.BasvuruYap(SuanYapilanIsyeriBildirgeYukleme);
                    if (basvuruSonuc == "OK")
                    {
                        MesajYaz("Başvuru yapıldı");
                    }
                    else MesajYaz("Başvuru yapılamadı. Nedeni:" + basvuruSonuc);
                }

                this.BildirgeYuklemeSonaErdi();
            }
            catch (Exception ex)
            {
                MesajYaz("Hata:" + ex.Message);

                Metodlar.HataMesajiGoster(ex, "Bildirge yükleme işleminde hata meydana geldi");

            }
        }

        public Enums.GecerliSayfaSonuclari GecerliSayfaOlupOlmadiginiKontrolEtBildirgeYukleme(string ResponseHtml, string SiradakiIslem)
        {

            string Mesaj = string.Empty;

            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(ResponseHtml);

            if (ResponseHtml.Equals("Error"))
            {
                yenidenYuklemeDenemeSayisiBildirgeYukleme++;

                if (yenidenYuklemeDenemeSayisiBildirgeYukleme > 20)
                {
                    yenidenYuklemeDenemeSayisiBildirgeYukleme = 0;

                    BildirgeWebClient.Disconnect();

                    return Enums.GecerliSayfaSonuclari.YenidenGirisYapilsin;
                }

                Thread.Sleep(1000);

                return Enums.GecerliSayfaSonuclari.Gecersiz;
            }
            else
            {

                var genelUyariCenterTag = html.GetElementbyId("genelUyariCenterTag");

                if (genelUyariCenterTag != null && !String.IsNullOrEmpty(genelUyariCenterTag.InnerText.Trim()))
                {
                    if (!genelUyariCenterTag.InnerText.Trim().Contains("Bu bilgilere sahip bildirge önceden oluşturulmuş")
                        && !genelUyariCenterTag.InnerText.Trim().Contains("İşten Ayrılış bildirgesi bulunamadı")
                        && !genelUyariCenterTag.InnerText.Trim().Contains("işe Giriş bildirgesi bulunamadı")
                        && !genelUyariCenterTag.InnerText.Trim().Contains("TC'ler:")
                        && !genelUyariCenterTag.InnerText.Trim().Contains("İşleminiz Başarılı Bir Şekilde Tamamlanmıştır.")
                        && !genelUyariCenterTag.InnerText.Trim().Contains("eksik Gün Hatalı Girildi")
                        && !genelUyariCenterTag.InnerText.Trim().Contains("eksik Gun Hatalı Girildi")
                        && !genelUyariCenterTag.InnerText.Trim().Contains("günden fazla bildirim yapamazsınız")
                        && !genelUyariCenterTag.InnerText.Trim().Contains("eksik Gun Sebebi Giriniz")
                        && !genelUyariCenterTag.InnerText.Trim().Contains("Kullanıcı Bilgilerini Giriniz")
                        && !genelUyariCenterTag.InnerText.Trim().Contains("6-Kısmi İstihdam")
                        && !genelUyariCenterTag.InnerText.Trim().Contains("Bu Belgenin aslı Daha Önce Girilmiş(Ek belge girişi yapabilirsiniz)")
                        && (!genelUyariCenterTag.InnerText.Trim().Contains("Bu Belgenin aslı Yok") || (genelUyariCenterTag.InnerText.Trim().Contains("Bu Belgenin aslı Yok") && SiradakiIslem.Equals("İptal Edilecek Kişiler Seçilecek")))
                        && !genelUyariCenterTag.InnerText.Trim().Contains("İşten Çıkış Sebebini Giriniz")
                        //İşten Çıkış Sebebini Giriniz
                        && !Regex.IsMatch(genelUyariCenterTag.InnerText.Trim(), "Sigortalının Toplam Tutarı .*dan Az Olamaz")
                        && !genelUyariCenterTag.InnerText.Trim().Contains("gun Sayısını Hatalı Girildi")
                        && !genelUyariCenterTag.InnerText.Trim().Contains("gun Sayısını Hatalı Girildi")
                        )
                    {
                        Mesaj = "SGK Hata Mesajı: " + genelUyariCenterTag.InnerText.Trim().Replace("<br>", "");

                        if (secilenIptalEdilecekBildirge != null && islemYapilacakIptalBildirgeleri.ContainsKey(secilenIptalEdilecekBildirge))
                        {
                            IptalBildirgeHataMesajlariniEkle(Mesaj, seciliYuklenecekBildirge, secilenIptalEdilecekBildirgeSira);
                        }
                        else
                        {
                            YuklenenBildirgeHataMesajiYaz(Mesaj, seciliYuklenecekBildirge);
                        }

                        return Enums.GecerliSayfaSonuclari.Iptal;

                    }
                }

                bool sayfaBulundu = false;

                if (ResponseHtml.Contains("Aylık Prim Hizmet Belgesi Girişi"))
                {
                    if (ResponseHtml.Contains("İşleminiz Başarılı Bir Şekilde Tamamlanmıştır"))
                    {
                        if (SiradakiIslem.Equals("Bildirge Onaylandı"))
                        {
                            sayfaBulundu = true;
                        }
                    }
                    else
                    {
                        sayfaBulundu = true;

                        if (!siradakiIslemBildirgeYukleme.Equals("Dönem Seçme Sayfası Açılacak"))
                        {
                            siradakiIslemBildirgeYukleme = "Dönem Seçme Sayfası Açılacak";
                        }
                    }

                }
                else if (ResponseHtml.Contains("TC'ler:"))
                {
                    if (SiradakiIslem.Equals("Bildirge Kişilerin Kontrolü Yapılacak"))
                    {
                        sayfaBulundu = true;
                    }

                }
                else if (ResponseHtml.Contains("6-Kısmi İstihdam"))
                {
                    if (SiradakiIslem.Equals("Bildirge Kişilerin Kontrolü Yapılacak"))
                    {
                        sayfaBulundu = true;
                    }
                }
                else if (ResponseHtml.Contains("Bu Belgenin aslı Daha Önce Girilmiş(Ek belge girişi yapabilirsiniz) !!!"))
                {
                    if (SiradakiIslem.Equals("İptal Edilecek Bildirge Tekrar Seçilecek"))
                    {
                        sayfaBulundu = true;
                    }
                }
                else if (ResponseHtml.Contains("Bu Belgenin aslı Yok"))
                {
                    if (SiradakiIslem.Equals("İptal Edilecek Bildirge Tekrar Seçilecek"))
                    {
                        sayfaBulundu = true;
                    }
                }
                else if (ResponseHtml.Contains("Söz konusu belgenin işlemleri tamamlanmadan yeni bir IPTAL<br>belge gönderilemez"))
                {
                    if (SiradakiIslem.Equals("İptal Edilecek Kişiler Seçilecek"))
                    {
                        sayfaBulundu = true;
                    }
                }
                else if (ResponseHtml.Contains("Onaylanmamış İptal Belgeniz vardır"))
                {
                    if (SiradakiIslem.Equals("İptal Edilecek Kişiler Seçilecek"))
                    {
                        sayfaBulundu = true;
                    }
                }
                else if (ResponseHtml.Contains("belge değişikliğini ayın 27'si ve sonrasında yapınız"))
                {
                    if (SiradakiIslem.Equals("İptal Bildirge Seçilecek"))
                    {
                        sayfaBulundu = true;
                    }
                }
                else if (ResponseHtml.Contains("İşlem Yapılacak Bildirge Dönemi Giriş"))
                {
                    if (SiradakiIslem.Equals("Dönem Seçilecek"))
                    {
                        sayfaBulundu = true;
                    }
                    else if (SiradakiIslem.Equals("Bildirge Kişilerin Kontrolü Yapılacak") && ResponseHtml.Contains("eksik Gun Sebebi Giriniz"))
                    {
                        sayfaBulundu = true;
                    }
                    else if (SiradakiIslem.Equals("Bildirge Kişilerin Kontrolü Yapılacak") && ResponseHtml.Contains("İşten Ayrılış bildirgesi bulunamadı"))
                    {
                        sayfaBulundu = true;
                    }
                    else if (SiradakiIslem.Equals("Bildirge Kişilerin Kontrolü Yapılacak") && ResponseHtml.Contains("işe Giriş bildirgesi bulunamadı"))
                    {
                        sayfaBulundu = true;
                    }
                    else if (SiradakiIslem.Equals("Bildirge Kişilerin Kontrolü Yapılacak") && (ResponseHtml.Contains("eksik Gün Hatalı Girildi") || ResponseHtml.Contains("eksik Gun Hatalı Girildi")))
                    {
                        sayfaBulundu = true;
                    }
                    else if (SiradakiIslem.Equals("Bildirge Kişilerin Kontrolü Yapılacak") && Regex.IsMatch(genelUyariCenterTag.InnerText.Trim(), "Sigortalının Toplam Tutarı .*dan Az Olamaz"))
                    {
                        sayfaBulundu = true;
                    }
                    else if (SiradakiIslem.Equals("Bildirge Kişilerin Kontrolü Yapılacak") && ResponseHtml.Contains("gun Sayısını Hatalı Girildi"))
                    {
                        sayfaBulundu = true;
                    }

                }
                else if (ResponseHtml != null && ResponseHtml.Contains("Belge İşlemleri") && ResponseHtml.Contains("Belge Mahiyeti"))
                {
                    if (SiradakiIslem.Equals("Select Kutusunda İptal Seçilecek") || SiradakiIslem.Equals("Bildirge Mahiyet ve Belge Türü Seçilecek"))
                    {
                        sayfaBulundu = true;
                    }
                }
                else if (ResponseHtml.Contains("Bu belgede iptal edilebilecek Kişi yoktur"))
                {
                    if (SiradakiIslem.Equals("İptal Edilecek Kişiler Seçilecek"))
                    {
                        sayfaBulundu = true;
                    }
                }
                else if (ResponseHtml.Contains("İptal İşlem Yapılacak Tahakkuk Girişi"))
                {
                    if (SiradakiIslem.Equals("İptal Bildirge Seçilecek"))
                    {
                        sayfaBulundu = true;
                    }
                }
                else if (ResponseHtml.Contains("Bu bilgilere sahip bildirge önceden oluşturulmuş"))
                {
                    if (SiradakiIslem.Equals("Yeni Belge Oluştur veya İşleme Devam Seçilecek"))
                    {
                        sayfaBulundu = true;
                    }
                }
                else if (ResponseHtml.Contains("İşlemi Yapılacak Sigortalı Bilgileri Giriş"))
                {
                    if (SiradakiIslem.Equals("İptal Edilecek Kişiler Seçilecek"))
                    {
                        if (html.DocumentNode.Descendants("input").Any(p => p.Id.Trim().Equals("tilesislemTamam_tahakkukiptalBelgeKisi")))
                        {
                            sayfaBulundu = true;
                        }

                    }

                    if (SiradakiIslem.Equals("İptal Edilecek Kişilerin Kontrolü Yapılacak"))
                    {
                        if (html.DocumentNode.Descendants("input").Any(p => p.Id.Trim().Equals("tilesislemTamam_tahakkukbelgeDegisiklikYeniBelgeBilgiIslemleri")))
                        {
                            sayfaBulundu = true;
                        }
                        else
                        {
                            if (genelUyariCenterTag != null && !String.IsNullOrEmpty(genelUyariCenterTag.InnerText.Trim()))
                            {
                                if (html.DocumentNode.Descendants("input").Any(p => p.Id.Trim().Equals("tilesislemTamam_tahakkukiptalBelgeKisi")))
                                {
                                    sayfaBulundu = true;
                                }
                            }

                        }

                    }

                    if (SiradakiIslem.Equals("Bildirge Kişilerin Kontrolü Yapılacak"))
                    {
                        sayfaBulundu = true;
                    }
                }
                else if (ResponseHtml.Contains("Kanun İşlemleri"))
                {
                    if (SiradakiIslem.Equals("Bildirge Kanun Seçilecek"))
                    {
                        sayfaBulundu = true;
                    }
                }
                else if (ResponseHtml.Contains("Belge türü ve-veya Kanun numarası değişikliği İşlemi İçin İptal Bildirge Listesi"))
                {
                    if (SiradakiIslem.Equals("İptal Edilecek Bildirge Tekrar Seçilecek"))
                    {
                        sayfaBulundu = true;
                    }
                }
                else if (ResponseHtml.Contains("Onay Bekleyen Bildirge Listesi"))
                {
                    if (SiradakiIslem.Equals("Onaylanacaklar Ekranı"))
                    {
                        sayfaBulundu = true;
                    }
                }
                else if (ResponseHtml.Contains("Bildirge Onaylama"))
                {
                    if (SiradakiIslem.Equals("Bildirge Onaylanacak"))
                    {
                        sayfaBulundu = true;
                    }
                }
                else if (ResponseHtml.Contains("İşleminiz Başarılı Bir Şekilde Tamamlanmıştır") && ResponseHtml.Contains("Aylık Prim Hizmet Belgesi Girişi "))
                {
                    if (SiradakiIslem.Equals("Bildirge Onaylandı"))
                    {
                        sayfaBulundu = true;
                    }
                }

                if (!sayfaBulundu)
                {
                    yenidenYuklemeDenemeSayisiBildirgeYukleme++;

                    if (yenidenYuklemeDenemeSayisiBildirgeYukleme > 5)
                    {
                        BildirgeWebClient.Disconnect();

                        yenidenYuklemeDenemeSayisiBildirgeYukleme = 0;
                    }
                }
                else yenidenYuklemeDenemeSayisiBildirgeYukleme = 0;

                return sayfaBulundu ? Enums.GecerliSayfaSonuclari.Gecerli : Enums.GecerliSayfaSonuclari.Gecersiz;

            }
        }

        void YuklenenBildirgeHataMesajiYaz(string Mesaj, int seciliYuklenecekBildirge)
        {
            MesajYaz(Mesaj);

            HataMesajiEkle(Mesaj, seciliYuklenecekBildirge);
        }

        void IptalBildirgeHataMesajlariniEkle(string Mesaj, int seciliYuklenecekBildirge, int secilenIptalEdilecekBildirgeSira)
        {
            var msj = IptalEdilecekBildirgeHataMesajiOlustur(secilenIptalEdilecekBildirge, secilenIptalEdilecekBildirgeSira);

            MesajYaz(msj);

            HataMesajiEkle(Mesaj, seciliYuklenecekBildirge);

            MesajYaz(String.Concat("Nedeni = ", Mesaj));

            HataMesajiEkle(Mesaj, seciliYuklenecekBildirge);
        }

        void HataMesajiEkle(string Mesaj, int seciliYuklenecekBildirge)
        {
            if (!EksikVeyaHataliGirilenBildirgeler.ContainsKey(YuklenecekBildirgeler[seciliYuklenecekBildirge]))
            {
                EksikVeyaHataliGirilenBildirgeler.Add(YuklenecekBildirgeler[seciliYuklenecekBildirge], new List<string>());
            }

            if (!EksikVeyaHataliGirilenBildirgeler[YuklenecekBildirgeler[seciliYuklenecekBildirge]].Contains(Mesaj))
                EksikVeyaHataliGirilenBildirgeler[YuklenecekBildirgeler[seciliYuklenecekBildirge]].Add(Mesaj);
        }

        string IptalEdilecekBildirgeHataMesajiOlustur(string secilenIptalEdilecekBildirge, int siraNo)
        {
            var iptalIstatistik = islemYapilacakIptalBildirgeleri[secilenIptalEdilecekBildirge];

            return String.Format("({0}/{1}) {2} işlem tarihli {3} {4}'dan dönüşüm gerçekleştirilemedi. İptal bildirgede bulunan kişi sayısı={5}", siraNo, islemYapilacakIptalBildirgeleri.Count, iptalIstatistik.IslemTarihi, iptalIstatistik.Mahiyet, iptalIstatistik.IptalKanun, iptalIstatistik.bulunanKisiSayisi);

        }

        void MesajYaz(string Mesaj)
        {
            sb.Append("[" + DateTime.Now.ToString() + "] : " + Mesaj + Environment.NewLine);
            new delLoglariGuncelle(LoglariGuncelle).Invoke();
        }

        void OnaydaBekleyenleriSil()
        {
            int silmeDenemeSayisi = 0;

            bool IptallerSilindi = false;

        EskiOnayBekleyenleriSil:

            string resp = BildirgeWebClient.Get("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukonayBekleyenTahakkuklar.action", string.Empty);

            if (resp.Contains("Belge türü ve-veya Kanun numarası değişikliği İşlemi İçin İptal Bildirge Listesi"))
            {
                resp = BildirgeWebClient.Get("https://ebildirge.sgk.gov.tr/EBildirgeV2/anasayfa.action", string.Empty);
                resp = BildirgeWebClient.Get("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukonayBekleyenTahakkuklar.action", string.Empty.AddToken(resp));
            }

            if (resp.Contains("Onay Bekleyen Bildirge Listesi"))
            {
                var html = new HtmlAgilityPack.HtmlDocument();

                html.LoadHtml(resp);

                var frm = html.GetElementbyId("onayBekleyenTahakkuklarForm");

                if (frm != null)
                {
                    var table = frm.Descendants("table").FirstOrDefault();

                    if (table != null)
                    {

                        var trs = table.Descendants("tr").Skip(1);

                        int denemeSayisi = 0;

                        foreach (var tr in trs)
                        {
                            var tds = tr.Descendants("td");

                            var radio = tds.ElementAt(0).Descendants("input").FirstOrDefault(p => p.GetAttributeValue("type", "").Equals("radio"));

                            var mahiyet = tds.ElementAt(5).InnerText.Trim();

                            var tahakkuknedeni = tds.ElementAt(1).InnerText.Trim();

                            if (!tahakkuknedeni.Equals("D")) continue;

                            if ((!IptallerSilindi && mahiyet.Equals("İPTAL")) || (IptallerSilindi && !mahiyet.Equals("İPTAL")))
                            {

                            //if (EskiOnaydaBekleyenlerSilindi)
                            //{
                            //    var onclick = radio.GetAttributeValue("onclick", "");

                            //    var bildirgeNo = onclick.Split(',')[3].Trim();

                            //    var asilVeyaEkBildirge = trs.FirstOrDefault(t => t.InnerHtml.Contains("," + bildirgeNo + ",") && !t.Equals(tr));

                            //    if (asilVeyaEkBildirge != null) continue;
                            //}


                            silmeyiDene:

                                var silinecekBildirgeRefNo = radio.GetAttributeValue("value", "");

                                resp = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tilesislemTamam.action", "bildirgeRefNo=" + silinecekBildirgeRefNo.Replace(" ", "+") + "&action%3Atahakkuksil=Bildirgeyi+Sil&download=true");

                                if (!resp.Contains("Silme İşlemi Başarılı Bir şekilde Tamamlanmıştır"))
                                {
                                    if (denemeSayisi < 3)
                                    {

                                        Thread.Sleep(1000);

                                        denemeSayisi++;

                                        goto silmeyiDene;
                                    }

                                }
                            }
                        }

                        if (!IptallerSilindi)
                        {

                            IptallerSilindi = true;

                            //if (!EskiOnaydaBekleyenlerSilindi)
                            {
                                goto EskiOnayBekleyenleriSil;
                            }
                        }
                    }
                }

            }
            else
            {
                if (silmeDenemeSayisi < 3)
                {
                    Thread.Sleep(1000);

                    silmeDenemeSayisi++;

                    goto EskiOnayBekleyenleriSil;
                }
            }

            EskiOnaydaBekleyenlerSilindi = true;

        }

        void KarsiligiOlmayanIptalleriSil()
        {
            int silmeDenemeSayisi = 0;

        EskiOnayBekleyenleriSil:
            string resp = BildirgeWebClient.Get("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukonayBekleyenTahakkuklar.action", string.Empty);

            if (resp.Contains("Belge türü ve-veya Kanun numarası değişikliği İşlemi İçin İptal Bildirge Listesi"))
            {
                resp = BildirgeWebClient.Get("https://ebildirge.sgk.gov.tr/EBildirgeV2/anasayfa.action", string.Empty);
                resp = BildirgeWebClient.Get("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tahakkukonayBekleyenTahakkuklar.action", string.Empty.AddToken(resp));
            }



            if (resp.Contains("Onay Bekleyen Bildirge Listesi"))
            {
                var html = new HtmlAgilityPack.HtmlDocument();

                html.LoadHtml(resp);

                var frm = html.GetElementbyId("onayBekleyenTahakkuklarForm");

                if (frm != null)
                {
                    var table = frm.Descendants("table").FirstOrDefault();

                    if (table != null)
                    {

                        var trs = table.Descendants("tr").Skip(1);

                        int denemeSayisi = 0;

                        foreach (var tr in trs)
                        {
                            var tds = tr.Descendants("td");

                            var radio = tds.ElementAt(0).Descendants("input").FirstOrDefault(p => p.GetAttributeValue("type", "").Equals("radio"));

                            var mahiyet = tds.ElementAt(5).InnerText.Trim();

                            if (mahiyet.Equals("İPTAL"))
                            {


                                var onclick = radio.GetAttributeValue("onclick", "");

                                var bildirgeNo = onclick.Split(',')[3].Trim();

                                if (bildirgeNo != "0")
                                {

                                    var asilVeyaEkBildirge = trs.FirstOrDefault(t => t.InnerHtml.Contains("," + bildirgeNo + ",") && !t.Equals(tr));

                                    if (asilVeyaEkBildirge != null) continue;
                                }


                            silmeyiDene:

                                var silinecekBildirgeRefNo = radio.GetAttributeValue("value", "");

                                resp = BildirgeWebClient.PostData("https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/tilesislemTamam.action", "bildirgeRefNo=" + silinecekBildirgeRefNo.Replace(" ", "+") + "&action%3Atahakkuksil=Bildirgeyi+Sil&download=true");

                                if (!resp.Contains("Silme İşlemi Başarılı Bir şekilde Tamamlanmıştır"))
                                {
                                    if (denemeSayisi < 3)
                                    {

                                        Thread.Sleep(1000);

                                        denemeSayisi++;

                                        goto silmeyiDene;
                                    }

                                }
                            }
                        }

                    }
                }

            }
            else
            {
                if (silmeDenemeSayisi < 3)
                {
                    Thread.Sleep(1000);

                    silmeDenemeSayisi++;

                    goto EskiOnayBekleyenleriSil;
                }
            }
        }

        Bildirge BildirgeBilgileriniAl(string file, out string hata)
        {
            Bildirge bildirge = null;

            hata = null;

            if (File.Exists(file))
            {
                List<object> HafizadanAtilacaklar = new List<object>();

                bildirge = new Bildirge();

                bildirge.Kisiler = new List<AphbSatir>();

                excelBildirgeYukleme = new Excel2.Application();

                int excelprocessid = Metodlar.GetExcelProcessId(excelBildirgeYukleme);

                var workkbooks = excelBildirgeYukleme.Workbooks;

                Excel2.Workbook CalismaKitabi = workkbooks.Open(file);

                string isyeriSicilNo = string.Empty;

                var sheets = CalismaKitabi.Sheets;

                HafizadanAtilacaklar.AddRange(new List<object> { workkbooks, CalismaKitabi, sheets });

                for (int i = 1; i <= sheets.Count; i++)
                {
                    Excel2.Worksheet CalismaSayfasi = (Excel2.Worksheet)sheets[i];

                    HafizadanAtilacaklar.AddRange(new List<object> { CalismaSayfasi });

                    if (i == 1)
                    {
                        for (int j = 0; j < 25; j++)
                        {
                            if (j > 25) break;

                            var cell = CalismaSayfasi.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[(Enums.BildirgeHucreleri)Enum.Parse(typeof(Enums.BildirgeHucreleri), "IsyeriSicil" + (j + 1).ToString())]] as Excel2.Range;

                            isyeriSicilNo += cell.Value2;

                            HafizadanAtilacaklar.AddRange(new List<object> { cell });
                        }

                        isyeriSicilNo = isyeriSicilNo.Trim();

                        if (!SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo.Substring(9,10).Equals(isyeriSicilNo.Substring(9,10)))
                        {
                            hata = "Seçilen işyeri ile bildirgedeki işyeri sicil numarası uyuşmamaktadır";

                            return null;
                        }

                        var cellYil = CalismaSayfasi.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BelgeYil]] as Excel2.Range;
                        var cellAy = CalismaSayfasi.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BelgeAy]] as Excel2.Range;
                        var cellAsil = CalismaSayfasi.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Asil]] as Excel2.Range;
                        var cellEk = CalismaSayfasi.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.Ek]] as Excel2.Range;
                        var cellBelgeTuru = CalismaSayfasi.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BildirgeBelgeTuru]] as Excel2.Range;
                        var cellKanun = CalismaSayfasi.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.BildirgeKanun]] as Excel2.Range;
                        var cellEkBilgiler = CalismaSayfasi.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaBilgiler]] as Excel2.Range;

                        bildirge.Yil = cellYil.Value2.ToString().Trim();
                        bildirge.Ay = cellAy.Value2.ToString().Trim();
                        var mahiyet = cellAsil.Value2;
                        if (mahiyet != null && mahiyet.ToString().Equals("X")) mahiyet = "ASIL";
                        else
                        {
                            mahiyet = cellEk.Value2;
                            if (mahiyet != null && mahiyet.ToString().Equals("X")) mahiyet = "EK";
                            else mahiyet = "İPTAL";
                        }

                        bildirge.Mahiyet = mahiyet.ToString();
                        bildirge.BelgeTuru = cellBelgeTuru.Value2.ToString().Trim();
                        bildirge.Kanun = cellKanun.Value2.ToString().Trim();
                        bildirge.EkBilgiler = cellEkBilgiler.Value2 != null ? cellEkBilgiler.Value2.ToString().Trim() : null;

                        HafizadanAtilacaklar.AddRange(new List<object> { cellYil, cellAy, cellAsil, cellEk, cellBelgeTuru, cellKanun, cellEkBilgiler });

                    }

                    int BaslangicNo = i == 1 ? BildirgeOlusturmaSabitleri.IlkSigortaliSayi : BildirgeOlusturmaSabitleri.DevamSayfasiIlkSigortaliSayi;

                    double kisisayisi = 0;

                    if (i == 1)
                    {
                        var cellKisiSayisi = CalismaSayfasi.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.IlkSayfaToplamSigortaliSayisi]] as Excel2.Range;

                        kisisayisi = cellKisiSayisi.Value2;

                        HafizadanAtilacaklar.AddRange(new List<object> { cellKisiSayisi });
                    }
                    else
                    {
                        var cellKisiSayisi = CalismaSayfasi.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.DevamBuSayfaToplamSigortaliSayisi]] as Excel2.Range;

                        if (cellKisiSayisi.Value2 != null)
                        {
                            kisisayisi = cellKisiSayisi.Value2;
                        }

                        HafizadanAtilacaklar.AddRange(new List<object> { cellKisiSayisi });
                    }


                    for (int j = 0; j < kisisayisi; j++)
                    {
                        var cellEkBilgiler = CalismaSayfasi.Range["A" + (BaslangicNo + (j * BildirgeOlusturmaSabitleri.IlkSigortaliArtis)).ToString()] as Excel2.Range;
                        var cellSosyalGuvenlikNo = CalismaSayfasi.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliSosyalGuvenlikSicilNo] + (BaslangicNo + (j * BildirgeOlusturmaSabitleri.IlkSigortaliArtis)).ToString()] as Excel2.Range;
                        var cellAdi = CalismaSayfasi.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliAdi] + (BaslangicNo + j * BildirgeOlusturmaSabitleri.IlkSigortaliArtis).ToString()] as Excel2.Range;
                        var cellSoyadi = CalismaSayfasi.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliSoyadi] + (BaslangicNo + j * BildirgeOlusturmaSabitleri.IlkSigortaliArtis).ToString()] as Excel2.Range;
                        var cellUcret = CalismaSayfasi.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliUcret] + (BaslangicNo + j * BildirgeOlusturmaSabitleri.IlkSigortaliArtis).ToString()] as Excel2.Range;
                        var cellGun = CalismaSayfasi.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliPrimOdemeGunu] + (BaslangicNo + j * BildirgeOlusturmaSabitleri.IlkSigortaliArtis).ToString()] as Excel2.Range;
                        var cellIkramiye = CalismaSayfasi.Range[BildirgeOlusturmaSabitleri.ExcelHucreleri[Enums.BildirgeHucreleri.SigortaliIkramiye] + (BaslangicNo + j * BildirgeOlusturmaSabitleri.IlkSigortaliArtis).ToString()] as Excel2.Range;

                        bildirge.Kisiler.Add(new AphbSatir
                        {
                            SosyalGuvenlikNo = cellSosyalGuvenlikNo.Value2.ToString().Trim(),
                            Adi = cellAdi.Value2.ToString().Trim(),
                            Soyadi = cellSoyadi.Value2.ToString().Trim(),
                            Ucret = cellUcret.Value2.ToString().Trim(),
                            Gun = cellGun.Value2.ToString().Trim(),
                            Ikramiye = cellIkramiye.Value2.ToString().Trim(),
                            Kanun = cellEkBilgiler.Value2 != null ? cellEkBilgiler.Value2.ToString().Split('-')[0].Trim() : null,
                            TesvikKanunNo = cellEkBilgiler.Value2 != null ? cellEkBilgiler.Value2.ToString().Split('-')[1].Trim() : null,
                            TesvikHesaplanacakGun = cellEkBilgiler.Value2 != null && cellEkBilgiler.Value2.ToString().Split('-').Length >= 3 ? cellEkBilgiler.Value2.ToString().Split('-')[2].Trim() : null,
                            DonusturulecekHesaplanacakGun = cellEkBilgiler.Value2 != null && cellEkBilgiler.Value2.ToString().Split('-').Length >= 4 ? cellEkBilgiler.Value2.ToString().Split('-')[3].Trim() : null,
                        });

                        HafizadanAtilacaklar.AddRange(new List<object> { cellSosyalGuvenlikNo, cellAdi, cellSoyadi, cellUcret, cellGun, cellIkramiye, cellEkBilgiler });
                    }

                }

                CalismaKitabi.Close(false);

                HafizadanAtilacaklar.Reverse();

                int z = 0;

                while (z < HafizadanAtilacaklar.Count())
                {
                    try
                    {
                        var item = HafizadanAtilacaklar.ElementAt(z);

                        Marshal.FinalReleaseComObject(item);

                        item = null;

                    }
                    catch
                    {
                    }

                    z++;
                }


                excelBildirgeYukleme.Quit();
                Marshal.FinalReleaseComObject(excelBildirgeYukleme);

                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
                //GC.WaitForPendingFinalizers();

                Metodlar.KillProcessById(excelprocessid);
            }
            else hata = "Bildirge dosyası bulunamadı";

            return bildirge;
        }

        void BildirgeYuklemeSonaErdi(bool icmalOlustur = true)
        {
            MesajYaz("Bildirge yükleme işlemi sona erdi");


            if (icmalOlustur)
            {
                MesajYaz("İcmal oluşturuluyor");
                BildirgeYuklemeIcmalOlustur(false);
                BildirgeYuklemeIcmalOlustur(true);
            }

            BildirgeYuklemeYapiliyor = false;

            BildirgeTicket = null;

            Task.Factory.StartNew(() =>
            {
                var wc = new ProjeGiris(SuanYapilanIsyeriBildirgeYukleme, BildirgeWebClient.proje);
                wc.Cookie = BildirgeWebClient.Cookie;

                try
                {
                    wc.Disconnect(true);
                }
                catch { }

            });

            LogYaz(sb.ToString());

        }

        public void BildirgeYuklemeIslemiIptalEt()
        {
            BildirgeYuklemeSonaErdi();
        }

        void BildirgeYuklemeIcmalOlustur(bool ExcelMi = true)
        {
            bool DosyaVar = false;

            string icmaldosya = null;

            Excel2.Application excelApp = ExcelMi ? new Excel2.Application() : null;

            if (excelApp != null) excelApp.DisplayAlerts = false;

            Excel2.Workbook MyBook = null;

            Excel2.Worksheet MySheet = null;

            Excel2.Worksheet MySheetHatalar = null;

            Excel2.Worksheet MySheetEskiIcmal = null;

            StringBuilder icmalSb = new StringBuilder();
            int SutunToplamKarakterSayisi = 20;
            int AltCizgiSayisi = 500;

            var eskiicmal = Directory.GetFiles(folderBrowserDialogBildirgeYukle.SelectedPath, "*Icmal Genel*.xlsx", SearchOption.AllDirectories);

            if (eskiicmal.Length > 0)
            {
                DosyaVar = true;

                icmaldosya = eskiicmal[0];
            }

            if (ExcelMi)
            {
                if (DosyaVar)
                {

                    MyBook = excelApp.Workbooks.Open(icmaldosya);

                    MySheetEskiIcmal = MyBook.Sheets[1];

                    MySheet = MyBook.Sheets.Add(After: MyBook.Sheets[MyBook.Sheets.Count]);

                    MySheet.Name = "Yüklenen İcmal2";

                    int j = 1;

                    while (j <= MyBook.Sheets.Count - 1)
                    {
                        if ((MyBook.Sheets[j] as Excel2.Worksheet).Name.Equals("Yüklenen İcmal"))
                        {

                            (MyBook.Sheets[j] as Excel2.Worksheet).Delete();

                        }
                        else if ((MyBook.Sheets[j] as Excel2.Worksheet).Name.Equals("Yüklenen Bildirge Hataları"))
                        {

                            (MyBook.Sheets[j] as Excel2.Worksheet).Delete();

                        }
                        else j++;
                    }

                    MyBook.Sheets[MyBook.Sheets.Count].Name = "Yüklenen İcmal";

                    MySheet.Activate();

                    if (EksikVeyaHataliGirilenBildirgeler.Count > 0)
                    {

                        MySheetHatalar = MyBook.Sheets.Add(After: MyBook.Sheets[MyBook.Sheets.Count]);

                        MySheetHatalar.Name = "Yüklenen Bildirge Hataları";
                    }

                }
                else
                {
                    Genel.IcmalKaydediliyorKontrolu();

                    icmaldosya = System.IO.Path.Combine(Application.StartupPath, "Icmal.xlsx");

                    MyBook = excelApp.Workbooks.Open(icmaldosya);

                    MySheet = (MyBook.Sheets[1] as Excel2.Worksheet);

                    MySheet.Name = "Yüklenen İcmal";

                    if (EksikVeyaHataliGirilenBildirgeler.Count > 0)
                    {
                        MySheetHatalar = MyBook.Sheets.Add(After: MyBook.Sheets[MyBook.Sheets.Count]);

                        MySheetHatalar = (MyBook.Sheets[2] as Excel2.Worksheet);
                        MySheetHatalar.Name = "Yüklenen Bildirge Hataları";
                    }

                    MySheet.Activate();

                }
            }


            var yillar = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new SortedDictionary<int, SortedDictionary<int, decimal>>());
            yillar.Add("Tumu", new SortedDictionary<int, SortedDictionary<int, decimal>>());

            var yillarYuklenen = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new SortedDictionary<int, SortedDictionary<int, decimal>>());
            yillarYuklenen.Add("Tumu", new SortedDictionary<int, SortedDictionary<int, decimal>>());


            for (int j = 0; j < BildirgeIcmaller.Count; j++)
            {

                var icmal = BildirgeIcmaller[j];



                if (icmal.Kanun != null)
                {

                    int Yil = Convert.ToInt32(icmal.yilay.Key);

                    int Ay = Convert.ToInt32(icmal.yilay.Value);

                    var tesvik = Program.TumTesvikler.FirstOrDefault(p => p.Key.PadLeft(5, '0').Equals(icmal.Kanun.PadLeft(5, '0')) || p.Value.AltKanunlar.Contains(icmal.Kanun.PadLeft(5, '0'))).Value;

                    if (tesvik != null)
                    {
                        if (!yillar[tesvik.Kanun].ContainsKey(Yil)) yillar[tesvik.Kanun].Add(Yil, new SortedDictionary<int, decimal>());

                        SortedDictionary<int, decimal> aylar = yillar[tesvik.Kanun][Yil];

                        if (!aylar.ContainsKey(Ay)) aylar.Add(Ay, 0);


                        aylar[Ay] += icmal.Tutar.ToTL().Replace("₺", "").ToDecimalSgk();

                        if (tesvik.Kanun.Equals("7103"))
                        {
                            var bildirge = YuklenecekBildirgeBilgileri[j];

                            decimal icmal7166Tutar = 0;

                            foreach (var kisiSatir in bildirge.Kisiler)
                            {
                                if (kisiSatir.TesvikKanunNo.Equals("7166"))
                                {
                                    var tesvik7166 = Program.TumTesvikler["7166"];

                                    var DonusturulenKanun = kisiSatir.Kanun;

                                    var kanunGun = kisiSatir.HesaplananGun.ToInt();
                                    var kanunUcret = kisiSatir.HesaplananUcret.ToDecimalSgk() + kisiSatir.HesaplananIkramiye.ToDecimalSgk();

                                    icmal7166Tutar += Metodlar.TesvikTutariHesapla("07166", kanunGun, kanunUcret, Yil, Ay, bildirge.BelgeTuru, SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo);

                                    var dk = tesvik7166.DonusturulecekKanunlar.FirstOrDefault(p => p.Key.Equals(DonusturulenKanun)).Value;

                                    if (dk != null)
                                    {
                                        var dusulecekTutar = DonusturulecekKanun.DusulecekMiktarHesapla(DonusturulenKanun, kisiSatir.HesaplananDonusecekGun.ToInt(), kisiSatir.HesaplananDonusecekToplamUcret, Yil, Ay, bildirge.BelgeTuru, SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo, tesvik7166.DonusenlerIcmaldenDusulsun, null, TesvikHesaplamaSabitleri.CarpimOrani687)[DonusturulenKanun].BagliKanunlarDahilDusulecekTutar;

                                        icmal7166Tutar -= dusulecekTutar;
                                    }
                                }
                            }

                            if (!yillar["7166"].ContainsKey(Yil)) yillar["7166"].Add(Yil, new SortedDictionary<int, decimal>());

                            SortedDictionary<int, decimal> aylar7166 = yillar["7166"][Yil];

                            if (!aylar7166.ContainsKey(Ay)) aylar7166.Add(Ay, 0);

                            aylar7166[Ay] += icmal7166Tutar.ToTL().Replace("₺", "").ToDecimalSgk();

                        }
                    }


                    if (!yillar["Tumu"].ContainsKey(Yil)) yillar["Tumu"].Add(Yil, new SortedDictionary<int, decimal>());

                    SortedDictionary<int, decimal> aylar2 = yillar["Tumu"][Yil];

                    if (!aylar2.ContainsKey(Ay)) aylar2.Add(Ay, 0);
                }

            }

            for (int j = 0; j < YuklenenIcmaller.Count; j++)
            {
                var icmal = YuklenenIcmaller[j];

                //var toplamTutar = Var6486 ? icmal.Tutar6486 : icmal.Tutar;
                //var toplamTutar = icmal.Tutar;

                int Yil = Convert.ToInt32(icmal.yilay.Key);

                int Ay = Convert.ToInt32(icmal.yilay.Value);

                if (!string.IsNullOrEmpty(icmal.Kanun))
                {
                    var tesvik = Program.TumTesvikler.FirstOrDefault(p => icmal.Kanun.PadLeft(5, '0').Equals(p.Key.PadLeft(5, '0')) || p.Value.AltKanunlar.Contains(icmal.Kanun.PadLeft(5, '0'))).Value;

                    if (tesvik != null)
                    {
                        if (!yillarYuklenen[tesvik.Kanun].ContainsKey(Yil)) yillarYuklenen[tesvik.Kanun].Add(Yil, new SortedDictionary<int, decimal>());

                        SortedDictionary<int, decimal> aylar = yillarYuklenen[tesvik.Kanun][Yil];

                        if (!aylar.ContainsKey(Ay)) aylar.Add(Ay, 0);

                        //if (eskiIcmaldeAyBulundu)
                        //{
                        //    aylar[Ay] = icmal.Tutar;
                        //}
                        //else aylar[Ay] += icmal.Tutar;

                        aylar[Ay] += icmal.Tutar.ToTL().Replace("₺", "").ToDecimalSgk();

                        if (tesvik.Kanun.Equals("7103"))
                        {
                            var bildirge = YuklenecekBildirgeBilgileri[j];

                            decimal icmal7166TutarYuklenen = 0;

                            var tesvik7166 = Program.TumTesvikler["7166"];

                            foreach (var kisiSatir in bildirge.Kisiler)
                            {
                                if (kisiSatir.TesvikKanunNo.Equals("7166"))
                                {
                                    var DonusturulenKanun = kisiSatir.Kanun;

                                    if (icmal.Kisiler.Any(p => DonusturulenKanun.EndsWith(p.Key) && p.Value.Contains(kisiSatir.SosyalGuvenlikNo)))
                                    {
                                        var kanunGun = kisiSatir.HesaplananGun.ToInt();
                                        var kanunUcret = kisiSatir.HesaplananUcret.ToDecimalSgk() + kisiSatir.HesaplananIkramiye.ToDecimalSgk();

                                        icmal7166TutarYuklenen += Metodlar.TesvikTutariHesapla("07166", kanunGun, kanunUcret, Yil, Ay, bildirge.BelgeTuru, SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo);

                                        var dk = tesvik7166.DonusturulecekKanunlar.FirstOrDefault(p => p.Key.Equals(DonusturulenKanun)).Value;

                                        if (dk != null)
                                        {
                                            var dusulecekTutar = DonusturulecekKanun.DusulecekMiktarHesapla(DonusturulenKanun, kisiSatir.HesaplananDonusecekGun.ToInt(), kisiSatir.HesaplananDonusecekToplamUcret, Yil, Ay, bildirge.BelgeTuru, SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo, tesvik7166.DonusenlerIcmaldenDusulsun, null, TesvikHesaplamaSabitleri.CarpimOrani687)[DonusturulenKanun].BagliKanunlarDahilDusulecekTutar;

                                            icmal7166TutarYuklenen -= dusulecekTutar;
                                        }
                                    }
                                }
                            }

                            if (!yillarYuklenen["7166"].ContainsKey(Yil)) yillarYuklenen["7166"].Add(Yil, new SortedDictionary<int, decimal>());

                            SortedDictionary<int, decimal> aylar7166 = yillarYuklenen["7166"][Yil];

                            if (!aylar7166.ContainsKey(Ay)) aylar7166.Add(Ay, 0);

                            aylar7166[Ay] += icmal7166TutarYuklenen.ToTL().Replace("₺", "").ToDecimalSgk();
                        }
                    }
                }

                if (!yillarYuklenen["Tumu"].ContainsKey(Yil)) yillarYuklenen["Tumu"].Add(Yil, new SortedDictionary<int, decimal>());

                SortedDictionary<int, decimal> aylar2 = yillarYuklenen["Tumu"][Yil];

                if (!aylar2.ContainsKey(Ay)) aylar2.Add(Ay, 0);

            }

            var toplamTutarBildirge = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => 0.0m);
            toplamTutarBildirge.Add("Tumu", 0.0m);

            var toplamTutarYuklenen = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => 0.0m);
            toplamTutarYuklenen.Add("Tumu", 0.0m);

            var TesvikVerilecekKanunlar = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.Select(p => p).ToList();
            if (yillar["6322/25510"].Count == 0) TesvikVerilecekKanunlar.Remove("6322/25510");
            if (yillar["5510"].Count == 0) TesvikVerilecekKanunlar.Remove("5510");

            var IsyeriBilgileri = (SuanYapilanIsyeriBildirgeYukleme.Sirketler.SirketAdi + " - " + SuanYapilanIsyeriBildirgeYukleme.SubeAdi).ToUpper() + Environment.NewLine;

            try
            {
                List<string> isyerisicils = new List<string> {
                    SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo.Substring(0, 1),
                    SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo.Substring(1, 4),
                    SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo.Substring(5, 2),
                    SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo.Substring(7, 2),
                    SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo.Substring(9, 7),
                    SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo.Substring(16, 3),
                    SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo.Substring(19, 2),
                };

                string isyerisicilno = String.Join(" ", isyerisicils.ToArray()).Trim();

                isyerisicilno += "-" + SuanYapilanIsyeriBildirgeYukleme.IsyeriSicilNo.Substring(21, 2);

                IsyeriBilgileri += isyerisicilno + " SİCİL NOLU İŞYERİ" + Environment.NewLine;


            }
            catch
            {

            }

            IsyeriBilgileri += IcmalOlusturmaSabitleri.IcmalBaslik1Tum + " " + IcmalOlusturmaSabitleri.IcmalBaslik2Tum;

            if (ExcelMi)
            {

                Excel2.Range rangeIsyeriBilgileri = MySheet.Range[MySheet.Cells[2, 1], MySheet.Cells[5, 15]];

                rangeIsyeriBilgileri.Merge();

                rangeIsyeriBilgileri.Value = IsyeriBilgileri;

                rangeIsyeriBilgileri.Font.Bold = true;

                rangeIsyeriBilgileri.Font.Name = "Times New Roman";

                rangeIsyeriBilgileri.Font.Size = 12;

                rangeIsyeriBilgileri.WrapText = true;

                rangeIsyeriBilgileri.VerticalAlignment = 2;

                rangeIsyeriBilgileri.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                rangeIsyeriBilgileri.BorderAround(Excel2.XlLineStyle.xlContinuous, Excel2.XlBorderWeight.xlMedium);
            }
            else
            {
                icmalSb.Append(IsyeriBilgileri + Environment.NewLine + new String('_', AltCizgiSayisi) + Environment.NewLine);
            }


            List<Excel2.Range> headers = new List<Excel2.Range>();

            List<Excel2.Range> rows = new List<Excel2.Range>();

            List<Excel2.Range> yiltoplamlari = new List<Excel2.Range>();

            List<Excel2.Range> eksikYuklenenler = new List<Excel2.Range>();

            int Satir = IcmalOlusturmaSabitleri.IcmalBaslangicSatir;

            int CiftSutun = IcmalOlusturmaSabitleri.IcmalCiftBaslangicSutun;
            int TekSutun = TesvikVerilecekKanunlar.Count + 2;

            var enumeratoryil = yillar["Tumu"].GetEnumerator();

            int i = 0;

            while (enumeratoryil.MoveNext())
            {
                int yil = enumeratoryil.Current.Key;

                if (yil == 0) continue;

                var yilToplamTutarBildirge = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => 0.0m);
                yilToplamTutarBildirge.Add("Tumu", 0.0m);

                var yilToplamTutarYuklenen = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => 0.0m);
                yilToplamTutarYuklenen.Add("Tumu", 0.0m);

                int Sutun = CiftSutun;

                if (ExcelMi)
                {
                    MySheet.Cells[Satir + i * 15, Sutun] = "DÖNEM";

                    for (int sira = 0; sira < TesvikVerilecekKanunlar.Count; sira++)
                    {

                        MySheet.Cells[Satir + i * 15, Sutun + sira * 2 + 1] = TesvikVerilecekKanunlar[sira];

                        MySheet.Cells[Satir + i * 15, Sutun + sira * 2 + 2] = "Yüklenen " + TesvikVerilecekKanunlar[sira];

                        headers.Add((Excel2.Range)MySheet.Cells[Satir + i * 15, Sutun + sira * 2 + 1]);

                        headers.Add((Excel2.Range)MySheet.Cells[Satir + i * 15, Sutun + sira * 2 + 2]);

                    }

                    MySheet.Cells[Satir + i * 15, Sutun + TesvikVerilecekKanunlar.Count * 2 + 1] = "TÜMÜ";

                    MySheet.Cells[Satir + i * 15, Sutun + TesvikVerilecekKanunlar.Count * 2 + 2] = "YÜKLENEN TÜMÜ";


                    headers.Add((Excel2.Range)MySheet.Cells[Satir + i * 15, Sutun]);
                    headers.Add((Excel2.Range)MySheet.Cells[Satir + i * 15, Sutun + TesvikVerilecekKanunlar.Count * 2 + 1]);
                    headers.Add((Excel2.Range)MySheet.Cells[Satir + i * 15, Sutun + TesvikVerilecekKanunlar.Count * 2 + 2]);
                }
                else
                {
                    icmalSb.Append("DÖNEM".PadRight(SutunToplamKarakterSayisi, ' '));

                    for (int sira = 0; sira < TesvikVerilecekKanunlar.Count; sira++)
                    {
                        icmalSb.Append(TesvikVerilecekKanunlar[sira].PadRight(SutunToplamKarakterSayisi, ' '));
                        icmalSb.Append(("Yüklenen " + TesvikVerilecekKanunlar[sira]).PadRight(SutunToplamKarakterSayisi, ' '));
                    }

                    icmalSb.Append("TÜMÜ".PadRight(SutunToplamKarakterSayisi, ' '));
                    icmalSb.Append("YÜKLENEN TÜMÜ".PadRight(SutunToplamKarakterSayisi, ' '));

                    icmalSb.Append(Environment.NewLine);
                    icmalSb.Append(new String('_', AltCizgiSayisi));
                    icmalSb.Append(Environment.NewLine);
                }


                var enumeratoray = enumeratoryil.Current.Value.GetEnumerator();

                int j = 1;

                while (enumeratoray.MoveNext())
                {
                    int ay = enumeratoray.Current.Key;

                    var ayToplamTutarBildirge = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => yillar[x].ContainsKey(yil) && yillar[x][yil].ContainsKey(ay) ? yillar[x][yil][ay] : 0.0m);
                    ayToplamTutarBildirge.Add("Tumu", 0.0m);

                    var ayToplamTutarYuklenen = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => yillarYuklenen[x].ContainsKey(yil) && yillarYuklenen[x][yil].ContainsKey(ay) ? yillarYuklenen[x][yil][ay] : 0.0m);
                    ayToplamTutarYuklenen.Add("Tumu", 0.0m);

                    if (ExcelMi)
                    {

                        MySheet.Cells[Satir + j + i * 15, Sutun] = yil.ToString() + "/" + ay.ToString();

                        for (int sira = 0; sira < TesvikVerilecekKanunlar.Count; sira++)
                        {
                            var tesvikKanun = TesvikVerilecekKanunlar[sira];

                            var bildirgeTutar = ayToplamTutarBildirge[tesvikKanun];
                            var yuklenenTutar = ayToplamTutarYuklenen[tesvikKanun];

                            MySheet.Cells[Satir + j + i * 15, Sutun + sira * 2 + 1] = bildirgeTutar.ToTL();
                            MySheet.Cells[Satir + j + i * 15, Sutun + sira * 2 + 2] = yuklenenTutar.ToTL();

                            if (!bildirgeTutar.ToTL().Equals(yuklenenTutar.ToTL()) && Math.Floor(bildirgeTutar) > Math.Floor(yuklenenTutar))
                            {
                                eksikYuklenenler.Add((Excel2.Range)MySheet.Cells[Satir + j + i * 15, Sutun + sira * 2 + 2]);
                            }

                            rows.Add((Excel2.Range)MySheet.Cells[Satir + j + i * 15, Sutun + sira * 2 + 1]);
                            rows.Add((Excel2.Range)MySheet.Cells[Satir + j + i * 15, Sutun + sira * 2 + 2]);

                            yilToplamTutarBildirge[tesvikKanun] += bildirgeTutar.ToTL().Replace("₺", "").ToDecimalSgk();
                            yilToplamTutarYuklenen[tesvikKanun] += yuklenenTutar.ToTL().Replace("₺", "").ToDecimalSgk();

                            yilToplamTutarBildirge["Tumu"] += bildirgeTutar.ToTL().Replace("₺", "").ToDecimalSgk();
                            yilToplamTutarYuklenen["Tumu"] += yuklenenTutar.ToTL().Replace("₺", "").ToDecimalSgk();
                        }

                        var ayBildirgeTutari = ayToplamTutarBildirge.Where(p => !p.Key.Equals("Tumu")).Sum(p => p.Value.ToTL().Replace("₺", "").ToDecimalSgk());
                        var ayYuklenen = ayToplamTutarYuklenen.Where(p => !p.Key.Equals("Tumu")).Sum(p => p.Value.ToTL().Replace("₺", "").ToDecimalSgk());

                        MySheet.Cells[Satir + j + i * 15, Sutun + TesvikVerilecekKanunlar.Count * 2 + 1] = ayBildirgeTutari.ToTL();
                        MySheet.Cells[Satir + j + i * 15, Sutun + TesvikVerilecekKanunlar.Count * 2 + 2] = ayYuklenen.ToTL();

                        if (!ayBildirgeTutari.ToTL().Equals(ayYuklenen.ToTL()) && Math.Floor(ayBildirgeTutari) > Math.Floor(ayYuklenen))
                        {
                            eksikYuklenenler.Add((Excel2.Range)MySheet.Cells[Satir + j + i * 15, Sutun + TesvikVerilecekKanunlar.Count * 2 + 2]);
                        }

                        rows.Add((Excel2.Range)MySheet.Cells[Satir + j + i * 15, Sutun]);
                        rows.Add((Excel2.Range)MySheet.Cells[Satir + j + i * 15, Sutun + TesvikVerilecekKanunlar.Count * 2 + 1]);
                        rows.Add((Excel2.Range)MySheet.Cells[Satir + j + i * 15, Sutun + TesvikVerilecekKanunlar.Count * 2 + 2]);
                    }
                    else
                    {
                        icmalSb.Append((yil.ToString() + "/" + ay.ToString()).PadRight(SutunToplamKarakterSayisi, ' '));

                        for (int sira = 0; sira < TesvikVerilecekKanunlar.Count; sira++)
                        {
                            var tesvikKanun = TesvikVerilecekKanunlar[sira];

                            var bildirgeTutar = ayToplamTutarBildirge[tesvikKanun];
                            var yuklenenTutar = ayToplamTutarYuklenen[tesvikKanun];

                            string yuklenenTutarString = yuklenenTutar.ToTL();

                            if (!bildirgeTutar.ToTL().Equals(yuklenenTutar.ToTL()) && Math.Floor(bildirgeTutar) > Math.Floor(yuklenenTutar))
                            {
                                yuklenenTutarString = "*" + yuklenenTutarString;
                            }

                            icmalSb.Append(bildirgeTutar.ToTL().PadRight(SutunToplamKarakterSayisi, ' '));
                            icmalSb.Append(yuklenenTutarString.PadRight(SutunToplamKarakterSayisi, ' '));

                            yilToplamTutarBildirge[tesvikKanun] += bildirgeTutar.ToTL().Replace("₺", "").ToDecimalSgk();
                            yilToplamTutarYuklenen[tesvikKanun] += yuklenenTutar.ToTL().Replace("₺", "").ToDecimalSgk();

                            yilToplamTutarBildirge["Tumu"] += bildirgeTutar.ToTL().Replace("₺", "").ToDecimalSgk();
                            yilToplamTutarYuklenen["Tumu"] += yuklenenTutar.ToTL().Replace("₺", "").ToDecimalSgk();
                        }

                        var ayBildirgeTutari = ayToplamTutarBildirge.Where(p => !p.Key.Equals("Tumu")).Sum(p => p.Value.ToTL().Replace("₺", "").ToDecimalSgk());
                        var ayYuklenen = ayToplamTutarYuklenen.Where(p => !p.Key.Equals("Tumu")).Sum(p => p.Value.ToTL().Replace("₺", "").ToDecimalSgk());

                        string ayYuklenenString = ayYuklenen.ToTL().ToString();

                        if (!ayBildirgeTutari.ToTL().Equals(ayYuklenen.ToTL()) && Math.Floor(ayBildirgeTutari) > Math.Floor(ayYuklenen))
                        {
                            ayYuklenenString = "*" + ayYuklenenString;
                        }

                        icmalSb.Append(ayBildirgeTutari.ToTL().PadRight(SutunToplamKarakterSayisi, ' '));
                        icmalSb.Append(ayYuklenenString.PadRight(SutunToplamKarakterSayisi, ' '));

                        icmalSb.Append(Environment.NewLine);
                        icmalSb.Append(new String('_', AltCizgiSayisi));
                        icmalSb.Append(Environment.NewLine);

                    }

                    j++;
                }

                if (ExcelMi)
                {

                    MySheet.Cells[Satir + j + i * 15, Sutun] = "Yıl toplamı";

                    for (int no = 0; no < TesvikVerilecekKanunlar.Count; no++)
                    {
                        var tesvikKanun = TesvikVerilecekKanunlar[no];
                        var bildirgeYilTutar = yilToplamTutarBildirge[tesvikKanun];
                        var yuklenenYilTutar = yilToplamTutarYuklenen[tesvikKanun];

                        MySheet.Cells[Satir + j + i * 15, Sutun + no * 2 + 1] = bildirgeYilTutar.ToTL();
                        MySheet.Cells[Satir + j + i * 15, Sutun + no * 2 + 2] = yuklenenYilTutar.ToTL();

                        if (!bildirgeYilTutar.ToTL().Equals(yuklenenYilTutar.ToTL()) && Math.Floor(bildirgeYilTutar) > Math.Floor(yuklenenYilTutar))
                        {
                            eksikYuklenenler.Add((Excel2.Range)MySheet.Cells[Satir + j + i * 15, Sutun + no * 2 + 2]);
                        }


                        toplamTutarBildirge[tesvikKanun] += bildirgeYilTutar.ToTL().Replace("₺", "").ToDecimalSgk();
                        toplamTutarYuklenen[tesvikKanun] += yuklenenYilTutar.ToTL().Replace("₺", "").ToDecimalSgk();

                        toplamTutarBildirge["Tumu"] += bildirgeYilTutar.ToTL().Replace("₺", "").ToDecimalSgk();
                        toplamTutarYuklenen["Tumu"] += yuklenenYilTutar.ToTL().Replace("₺", "").ToDecimalSgk();

                        yiltoplamlari.Add((Excel2.Range)MySheet.Cells[Satir + j + i * 15, Sutun + no * 2 + 1]);
                        yiltoplamlari.Add((Excel2.Range)MySheet.Cells[Satir + j + i * 15, Sutun + no * 2 + 2]);
                    }

                    var yilBildirgeTutari = yilToplamTutarBildirge.Where(p => !p.Key.Equals("Tumu")).Sum(p => p.Value.ToTL().Replace("₺", "").ToDecimalSgk());
                    var yilYuklenen = yilToplamTutarYuklenen.Where(p => !p.Key.Equals("Tumu")).Sum(p => p.Value.ToTL().Replace("₺", "").ToDecimalSgk());

                    MySheet.Cells[Satir + j + i * 15, Sutun + TesvikVerilecekKanunlar.Count * 2 + 1] = yilBildirgeTutari.ToTL();
                    MySheet.Cells[Satir + j + i * 15, Sutun + TesvikVerilecekKanunlar.Count * 2 + 2] = yilYuklenen.ToTL();

                    if (!yilBildirgeTutari.ToTL().Equals(yilYuklenen.ToTL()) && Math.Floor(yilBildirgeTutari) > Math.Floor(yilYuklenen))
                    {
                        eksikYuklenenler.Add((Excel2.Range)MySheet.Cells[Satir + j + i * 15, Sutun + TesvikVerilecekKanunlar.Count * 2 + 2]);
                    }

                    yiltoplamlari.Add((Excel2.Range)MySheet.Cells[Satir + j + i * 15, Sutun]);
                    yiltoplamlari.Add((Excel2.Range)MySheet.Cells[Satir + j + i * 15, Sutun + TesvikVerilecekKanunlar.Count * 2 + 1]);
                    yiltoplamlari.Add((Excel2.Range)MySheet.Cells[Satir + j + i * 15, Sutun + TesvikVerilecekKanunlar.Count * 2 + 2]);
                }
                else
                {
                    icmalSb.Append("Yıl toplamı".ToString().PadRight(SutunToplamKarakterSayisi, ' '));

                    for (int no = 0; no < TesvikVerilecekKanunlar.Count; no++)
                    {
                        var tesvikKanun = TesvikVerilecekKanunlar[no];
                        var bildirgeYilTutar = yilToplamTutarBildirge[tesvikKanun];
                        var yuklenenYilTutar = yilToplamTutarYuklenen[tesvikKanun];
                        var yuklenenYilTutarString = yuklenenYilTutar.ToTL();

                        if (!bildirgeYilTutar.ToTL().Equals(yuklenenYilTutar.ToTL()) && Math.Floor(bildirgeYilTutar) > Math.Floor(yuklenenYilTutar))
                        {
                            yuklenenYilTutarString = "*" + yuklenenYilTutarString;
                        }

                        icmalSb.Append(bildirgeYilTutar.ToTL().PadRight(SutunToplamKarakterSayisi, ' '));
                        icmalSb.Append(yuklenenYilTutarString.PadRight(SutunToplamKarakterSayisi, ' '));

                        toplamTutarBildirge[tesvikKanun] += bildirgeYilTutar.ToTL().Replace("₺", "").ToDecimalSgk();
                        toplamTutarYuklenen[tesvikKanun] += yuklenenYilTutar.ToTL().Replace("₺", "").ToDecimalSgk();

                        toplamTutarBildirge["Tumu"] += bildirgeYilTutar.ToTL().Replace("₺", "").ToDecimalSgk();
                        toplamTutarYuklenen["Tumu"] += yuklenenYilTutar.ToTL().Replace("₺", "").ToDecimalSgk();
                    }

                    var yilBildirgeTutari = yilToplamTutarBildirge.Where(p => !p.Key.Equals("Tumu")).Sum(p => p.Value.ToTL().Replace("₺", "").ToDecimalSgk());
                    var yilYuklenen = yilToplamTutarYuklenen.Where(p => !p.Key.Equals("Tumu")).Sum(p => p.Value.ToTL().Replace("₺", "").ToDecimalSgk());
                    string yilYuklenenString = yilYuklenen.ToTL().ToString();

                    if (!yilBildirgeTutari.ToTL().Equals(yilYuklenen.ToTL()) && Math.Floor(yilBildirgeTutari) > Math.Floor(yilYuklenen))
                    {
                        yilYuklenenString = "*" + yilYuklenenString;
                    }

                    icmalSb.Append(yilBildirgeTutari.ToTL().PadRight(SutunToplamKarakterSayisi, ' '));
                    icmalSb.Append(yilYuklenenString.PadRight(SutunToplamKarakterSayisi, ' '));

                    for (int x = 0; x < 2; x++)
                    {
                        icmalSb.Append(Environment.NewLine);
                        icmalSb.Append(new String('_', AltCizgiSayisi));
                        icmalSb.Append(Environment.NewLine);
                    }
                }

                i++;

            }

            if (ExcelMi)
            {

                foreach (Excel2.Range r in headers)
                {
                    r.Font.Bold = true;

                    r.Font.Name = "Times New Roman";

                    r.Font.Size = 10;

                    r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                    r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                    r.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(234, 241, 221));

                    r.BorderAround(Excel2.XlLineStyle.xlContinuous);

                }

                foreach (Excel2.Range r in rows)
                {
                    r.Font.Bold = false;

                    r.Font.Name = "Times New Roman";

                    r.Font.Size = 10;

                    r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                    r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignRight;

                    r.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(197, 217, 241));

                    r.BorderAround(Excel2.XlLineStyle.xlContinuous);

                }

                foreach (Excel2.Range r in yiltoplamlari)
                {
                    r.Font.Bold = true;

                    r.Font.Name = "Times New Roman";

                    r.Font.Size = 10;

                    r.VerticalAlignment = Excel2.XlVAlign.xlVAlignCenter;

                    r.HorizontalAlignment = Excel2.XlHAlign.xlHAlignRight;

                    r.BorderAround(Excel2.XlLineStyle.xlContinuous);


                }
            }

            int str = Satir + i * 15;

            List<Excel2.Range> ranges = new List<Excel2.Range>();

            {
                int stn = CiftSutun;

                if (ExcelMi)
                {

                    Excel2.Range range = MySheet.Range[MySheet.Cells[str, stn], MySheet.Cells[str + 6, stn + TesvikVerilecekKanunlar.Count - 1]];

                    for (int sira = 0; sira < TesvikVerilecekKanunlar.Count; sira++)
                    {
                        var tesvikKanun = TesvikVerilecekKanunlar[sira];

                        Excel2.Range rangeheader = (Excel2.Range)MySheet.Cells[str + 7, stn + sira];
                        Excel2.Range rangeKanun = (Excel2.Range)MySheet.Cells[str + 8, stn + sira];

                        rangeheader.Value2 = tesvikKanun;
                        rangeKanun.Value2 = toplamTutarBildirge[tesvikKanun].ToTL();

                        ranges.Add(rangeheader);
                        ranges.Add(rangeKanun);
                    }

                    Excel2.Range rangetutar = MySheet.Range[MySheet.Cells[str + 9, stn], MySheet.Cells[str + 12, stn + TesvikVerilecekKanunlar.Count - 1]];

                    ranges.AddRange(new List<Excel2.Range> { range, rangetutar });

                    range.Merge();

                    range.Value = "Teşvik kapsamında işveren tarafından iade alınacak olan toplam prim tutarı(kanuni faiz hariç)";

                    rangetutar.Merge();

                    rangetutar.Value = toplamTutarBildirge["Tumu"].ToTL();

                    rangetutar.Font.Size = 15;
                }
                else
                {
                    icmalSb.Append(Environment.NewLine);
                    icmalSb.Append(new String('_', AltCizgiSayisi));
                    icmalSb.Append(Environment.NewLine);
                    icmalSb.Append("BİLDİRGE TOPLAM İCMAL");
                    icmalSb.Append(Environment.NewLine);
                    icmalSb.Append(new String('_', AltCizgiSayisi));
                    icmalSb.Append(Environment.NewLine);

                    for (int sira = 0; sira < TesvikVerilecekKanunlar.Count; sira++)
                    {
                        var tesvikKanun = TesvikVerilecekKanunlar[sira];

                        icmalSb.Append(tesvikKanun.PadRight(SutunToplamKarakterSayisi, ' '));
                    }

                    icmalSb.Append("TÜMÜ".PadRight(SutunToplamKarakterSayisi, ' '));

                    icmalSb.Append(Environment.NewLine);
                    icmalSb.Append(new String('_', AltCizgiSayisi));
                    icmalSb.Append(Environment.NewLine);

                    for (int sira = 0; sira < TesvikVerilecekKanunlar.Count; sira++)
                    {
                        var tesvikKanun = TesvikVerilecekKanunlar[sira];
                        icmalSb.Append(toplamTutarBildirge[tesvikKanun].ToTL().PadRight(SutunToplamKarakterSayisi, ' '));
                    }

                    icmalSb.Append(toplamTutarBildirge["Tumu"].ToTL().PadRight(SutunToplamKarakterSayisi, ' '));
                }
            }

            {
                if (ExcelMi)
                {
                    int stn = TekSutun;

                    Excel2.Range range = MySheet.Range[MySheet.Cells[str, stn], MySheet.Cells[str + 6, stn + TesvikVerilecekKanunlar.Count - 1]];

                    for (int sira = 0; sira < TesvikVerilecekKanunlar.Count; sira++)
                    {
                        var tesvikKanun = TesvikVerilecekKanunlar[sira];

                        Excel2.Range rangeheader = (Excel2.Range)MySheet.Cells[str + 7, stn + sira];
                        Excel2.Range rangeKanun = (Excel2.Range)MySheet.Cells[str + 8, stn + sira];

                        rangeheader.Value2 = tesvikKanun;
                        rangeKanun.Value2 = toplamTutarYuklenen[tesvikKanun].ToTL();

                        ranges.Add(rangeheader);
                        ranges.Add(rangeKanun);
                    }


                    Excel2.Range rangetutar = MySheet.Range[MySheet.Cells[str + 9, stn], MySheet.Cells[str + 12, stn + TesvikVerilecekKanunlar.Count - 1]];

                    ranges.AddRange(new List<Excel2.Range> { range, rangetutar });

                    range.Merge();

                    range.Value = "Yüklenen İcmal Toplam Prim Tutarı(kanuni faiz hariç)";

                    rangetutar.Merge();

                    rangetutar.Value = toplamTutarYuklenen["Tumu"].ToTL();

                    rangetutar.Font.Size = 15;
                }
                else
                {
                    icmalSb.Append(Environment.NewLine);
                    icmalSb.Append(Environment.NewLine);
                    icmalSb.Append(new String('_', AltCizgiSayisi));
                    icmalSb.Append(Environment.NewLine);
                    icmalSb.Append("YÜKLENEN TOPLAM İCMAL");
                    icmalSb.Append(Environment.NewLine);
                    icmalSb.Append(new String('_', AltCizgiSayisi));
                    icmalSb.Append(Environment.NewLine);

                    for (int sira = 0; sira < TesvikVerilecekKanunlar.Count; sira++)
                    {
                        var tesvikKanun = TesvikVerilecekKanunlar[sira];

                        icmalSb.Append(tesvikKanun.PadRight(SutunToplamKarakterSayisi, ' '));
                    }
                    icmalSb.Append("TÜMÜ".PadRight(SutunToplamKarakterSayisi, ' '));

                    icmalSb.Append(Environment.NewLine);
                    icmalSb.Append(new String('_', AltCizgiSayisi));
                    icmalSb.Append(Environment.NewLine);

                    for (int sira = 0; sira < TesvikVerilecekKanunlar.Count; sira++)
                    {
                        var tesvikKanun = TesvikVerilecekKanunlar[sira];

                        string toplamTutarYuklenenString = toplamTutarYuklenen[tesvikKanun].ToTL();

                        if (!toplamTutarBildirge[tesvikKanun].ToTL().Equals(toplamTutarYuklenen[tesvikKanun].ToTL()) && Math.Floor(toplamTutarBildirge[tesvikKanun]) > Math.Floor(toplamTutarYuklenen[tesvikKanun]))
                        {
                            toplamTutarYuklenenString = "*" + toplamTutarYuklenenString;
                        }

                        icmalSb.Append(toplamTutarYuklenenString.PadRight(SutunToplamKarakterSayisi, ' '));
                    }

                    string toplamTutarYuklenenTumuString = toplamTutarYuklenen["Tumu"].ToTL();

                    if (!toplamTutarBildirge["Tumu"].ToTL().Equals(toplamTutarYuklenen["Tumu"].ToTL()) && Math.Floor(toplamTutarBildirge["Tumu"]) > Math.Floor(toplamTutarYuklenen["Tumu"]))
                    {
                        toplamTutarYuklenenTumuString = "*" + toplamTutarYuklenenTumuString;
                    }

                    icmalSb.Append(toplamTutarYuklenenTumuString.PadRight(SutunToplamKarakterSayisi, ' '));
                }
            }

            if (ExcelMi)
            {
                foreach (var rng in ranges)
                {
                    rng.Font.Bold = true;

                    rng.Font.Name = "Times New Roman";

                    rng.Font.Size = 10;

                    rng.WrapText = true;

                    rng.VerticalAlignment = 2;

                    rng.HorizontalAlignment = Excel2.XlHAlign.xlHAlignCenter;

                    rng.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(217, 151, 149));

                    rng.BorderAround(Excel2.XlLineStyle.xlContinuous, Excel2.XlBorderWeight.xlMedium);
                }

                foreach (var rng in eksikYuklenenler)
                {
                    rng.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(237, 147, 160));
                }
            }


            #region Hataların Excelde Başka bir Sayfaya Yazdırılması

            if (EksikVeyaHataliGirilenBildirgeler.Count > 0)
            {

                if (ExcelMi)
                {
                    int satir = 1;

                    foreach (var hatalibildirge in EksikVeyaHataliGirilenBildirgeler)
                    {
                        MySheetHatalar.Cells[satir, 1] = System.IO.Path.GetFileNameWithoutExtension(hatalibildirge.Key);
                        MySheetHatalar.Cells[satir, 2] = String.Join(" --- ", hatalibildirge.Value);

                        satir++;
                    }

                    MySheetHatalar.Activate();

                    MyBook.ActiveSheet.Columns("A:Z").AutoFit();
                }
                else
                {
                    icmalSb.Append(Environment.NewLine);
                    icmalSb.Append(new String('_', AltCizgiSayisi));
                    icmalSb.Append(Environment.NewLine);
                    icmalSb.Append(Environment.NewLine);
                    icmalSb.Append("HATALAR");
                    icmalSb.Append(Environment.NewLine);
                    icmalSb.Append(new String('_', AltCizgiSayisi));
                    icmalSb.Append(Environment.NewLine);


                    foreach (var hatalibildirge in EksikVeyaHataliGirilenBildirgeler)
                    {
                        icmalSb.Append(System.IO.Path.GetFileNameWithoutExtension(hatalibildirge.Key) + " = " + String.Join(" --- ", hatalibildirge.Value));

                        icmalSb.Append(Environment.NewLine);
                        icmalSb.Append(new String('_', AltCizgiSayisi));
                        icmalSb.Append(Environment.NewLine);
                    }
                }
            }

            #endregion

            try
            {
                if (ExcelMi)
                {
                    MySheet.Activate();

                    MyBook.ActiveSheet.Columns("B:Z").AutoFit();

                    if (!DosyaVar)
                    {
                        icmaldosya = System.IO.Path.Combine(System.IO.Path.Combine(folderBrowserDialogBildirgeYukle.SelectedPath, "Icmal Genel.xlsx"));

                        MyBook.SaveAs(icmaldosya);

                        MyBook.Close(false);

                        excelApp.Quit();

                        Genel.IcmalKaydediliyorKilidiniKaldir();

                        Process.Start(icmaldosya);
                    }
                    else
                    {
                        MyBook.Save();

                        excelApp.Visible = true;
                    }

                    string mesaj = "İcmal dosyası başarılı bir şekilde oluşturuldu";

                    sb.Append("[" + DateTime.Now.ToString() + "] : " + mesaj + Environment.NewLine);
                    new delLoglariGuncelle(LoglariGuncelle).Invoke();


                }
                else
                {
                    string path = System.IO.Path.Combine(folderBrowserDialogBildirgeYukle.SelectedPath, "Icmal Genel.txt");

                    if (DosyaVar)
                    {
                        path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(icmaldosya), "Icmal Genel.txt");
                    }

                    File.WriteAllText(path, icmalSb.ToString());
                }



            }
            catch (Exception ex)
            {
                if (!DosyaVar && ExcelMi)
                    Genel.IcmalKaydediliyorKilidiniKaldir();

                Metodlar.HataMesajiGoster(ex, "Bildirge yükleme icmal dosyayı kaydedilirken hata meydana geldi");
            }
            finally
            {
                if (!DosyaVar && ExcelMi)
                    Genel.IcmalKaydediliyorKilidiniKaldir();
            }
        }



        #endregion

    }

}
