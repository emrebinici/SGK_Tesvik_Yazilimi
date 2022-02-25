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
using TesvikProgrami.Classes;
using System.Data.Entity;

namespace TesvikProgrami
{
    public partial class frmSirketler : Form
    {
        long eskielementID = 0;

        public DialogResult dr = DialogResult.Cancel;

        List<long> secilenler = new List<long>();

        bool Ara = false;

        string SearchParam = null;

        public List<Sirketler> sirketListesi { get; set; }

        public frmSirketler()
        {
            InitializeComponent();
        }

        public frmSirketler(string Search)
        {
            InitializeComponent();

            SearchParam = Search;
        }

        private void frmSirketler_Load(object sender, EventArgs e)
        {
            SirketleriDoldur(chkPasifleriGoster.Checked);

            if (!string.IsNullOrEmpty(SearchParam))
            {
                this.txtAra.Text = SearchParam;

            }

            //if (Ara) Search();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (txtSirketAdi.Text.Trim() != "" && txtVergiKimlikNo.Text != "")
            {

                var sirketAdi = txtSirketAdi.Text.Trim();
                var vergiKimlikNo = txtVergiKimlikNo.Text.Trim();

                using (var dbContext = new DbEntities())
                {
                    Sirketler sirket = null;

                    if (eskielementID > 0)
                    {
                        sirket = dbContext.Sirketler.FirstOrDefault(p => p.SirketID.Equals(eskielementID));
                    }

                    bool yeniEklenecek = sirket == null;

                    if (dbContext.Sirketler.FirstOrDefault(p => (p.SirketAdi.Equals(sirketAdi) || p.VergiKimlikNo.Equals(vergiKimlikNo)) && !p.SirketID.Equals(eskielementID)) == null)
                    {
                        bool sirketKlasoruGuncelle = eskielementID > 0 && sirket != null && !sirket.SirketAdi.Equals(sirketAdi);

                        sirket = sirket ?? new Sirketler();

                        sirket.SirketAdi = sirketAdi;
                        sirket.VergiKimlikNo = vergiKimlikNo;
                        sirket.Aktif = Convert.ToInt64(chkAktif.Checked);

                        if (yeniEklenecek) dbContext.Sirketler.Add(sirket);

                        dbContext.SaveChanges();

                        if (sirketKlasoruGuncelle) Metodlar.SirketPathUpdate(sirket);

                        dr = DialogResult.OK;

                        SirketleriDoldur(chkPasifleriGoster.Checked);

                        if (Ara) Search();

                        AlanlariTemizle();

                        MessageBox.Show("Kayıt başarılı");
                    }
                    else MessageBox.Show("Aynı şirket adı veya vergi nosu daha önce eklenmiştir", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
            else MessageBox.Show("Zorunlu alanlar boş bırakılamaz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SirketleriDoldur(bool PasifleriGoster, int skip = 0, int take = 25)
        {
            if (this.sirketListesi == null)
            {
                using (var dbContext = new DbEntities())
                {
                    this.sirketListesi = dbContext.Sirketler.ToList();
                }
            }


            var sirketler = this.sirketListesi.OrderByDescending(p => secilenler.Contains(p.SirketID) ? 1 : 0).ToList();

            take = secilenler.Count > take ? secilenler.Count : take;

            statusSirketSayisi.Text = String.Format("Toplam Şirket Sayısı : {0}", sirketListesi.Count);

            if (!PasifleriGoster)
            {
                sirketler = sirketler.Where(s => Convert.ToBoolean(s.Aktif)).ToList();
            }

            secilenler.RemoveAll(p => !sirketler.Any(x => x.SirketID.Equals(p)));



            dgvSirketler.AutoGenerateColumns = false;
            dgvSirketler.DataSource = sirketler.Skip(skip).Take(take).ToList();

            if (secilenler.Count > 0)
            {
                foreach (DataGridViewRow row in dgvSirketler.Rows)
                {
                    row.Cells[0].Value = secilenler.Contains((row.DataBoundItem as Sirketler).SirketID);
                }
            }

            txtAra.Focus();

        }

        private void lblIptal_Click(object sender, EventArgs e)
        {
            AlanlariTemizle();
        }

        private void AlanlariTemizle()
        {
            txtSirketAdi.Text = "";

            txtVergiKimlikNo.Text = "";

            chkAktif.Checked = true;

            eskielementID = 0;

            lblIptal.Visible = false;

        }

        private void dgvSirketler_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var sirket = dgvSirketler.Rows[e.RowIndex].DataBoundItem as Sirketler;

                if (dgvSirketler.Columns[e.ColumnIndex].Name == "colDuzenle")
                {
                    eskielementID = sirket.SirketID;

                    txtSirketAdi.Text = sirket.SirketAdi;

                    txtVergiKimlikNo.Text = sirket.VergiKimlikNo;

                    chkAktif.Checked = Convert.ToBoolean(sirket.Aktif);

                    lblIptal.Visible = true;
                }
                else if (dgvSirketler.Columns[e.ColumnIndex].Name == "colSec")
                {
                    if (dgvSirketler.Rows[e.RowIndex].Cells[0].Value == null)
                    {
                        dgvSirketler.Rows[e.RowIndex].Cells[0].Value = true;

                        secilenler.Add(sirket.SirketID);
                    }
                    else
                    {

                        dgvSirketler.Rows[e.RowIndex].Cells[0].Value = !(bool)dgvSirketler.Rows[e.RowIndex].Cells[0].Value;

                        if ((bool)dgvSirketler.Rows[e.RowIndex].Cells[0].Value == true)
                        {
                            secilenler.Add(sirket.SirketID);
                        }
                        else if ((bool)dgvSirketler.Rows[e.RowIndex].Cells[0].Value == false)
                        {
                            secilenler.Remove(sirket.SirketID);
                        }
                    }
                }
                else if (dgvSirketler.Columns[e.ColumnIndex].Name == "colSil")
                {
                    if (MessageBox.Show("Silmek istediğinizden emin misiniz", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        Sil(sirket.SirketID);

                        SirketleriDoldur(chkPasifleriGoster.Checked);

                        if (Ara) Search();

                        MessageBox.Show("Başarıyla silindi");

                    }

                }
                else if (dgvSirketler.Columns[e.ColumnIndex].Name == "colCariTanimla" || dgvSirketler.Columns[e.ColumnIndex].Name == "colGecmisTanimla")
                {

                    if (Program.IndirilenSirketler.ContainsKey(sirket.SirketID))
                    {
                        var indirilenSirket = Program.IndirilenSirketler[sirket.SirketID];

                        if (indirilenSirket.formSirketIndirmeEkrani == null || indirilenSirket.formSirketIndirmeEkrani.IsDisposed)
                        {
                            indirilenSirket.formSirketIndirmeEkrani = new frmSirketIndirmeEkrani(indirilenSirket, this);
                        }

                        indirilenSirket.formSirketIndirmeEkrani.Show();
                        indirilenSirket.formSirketIndirmeEkrani.BringToFront();

                    }
                    else
                    {
                        var cariTanimla= dgvSirketler.Columns[e.ColumnIndex].Name == "colCariTanimla";

                        frmTarihSec formTarihSec = new frmTarihSec(cariTanimla);

                        if (formTarihSec.ShowDialog() == DialogResult.OK)
                        {
                            List<Isyerleri> isyerleri = null;
                            using (var dbContext = new DbEntities())
                            {
                                isyerleri = dbContext.Isyerleri.Include(p => p.Sirketler).Where(p => p.SirketID.Equals(sirket.SirketID) && p.Aktif == 1).ToList();
                            }

                            if (isyerleri.Count > 0)
                            {

                                var formIsyerleriSec = new frmIsyerleriSec(isyerleri, formTarihSec.secenekler);

                                if (formIsyerleriSec.ShowDialog() == DialogResult.OK)
                                {

                                    if (formIsyerleriSec.SeciliIsyerleri.Count > 0)
                                    {

                                        if (Program.IndirilenSirketler.ContainsKey(sirket.SirketID)) Program.IndirilenSirketler.Remove(sirket.SirketID);

                                        Program.IndirilenSirketler.Add(sirket.SirketID, new SirketAphbBasvuruFormuIndirme(sirket));

                                        var indirilensirket = Program.IndirilenSirketler[sirket.SirketID];

                                        foreach (var keyvalue in formIsyerleriSec.SeciliIsyerleri)
                                        {
                                            var isyeri = keyvalue.Key;
                                            var secenekler = keyvalue.Value;

                                            var aphbyol = Metodlar.FormBul(isyeri, Enums.FormTuru.Aphb);

                                            DataTable dtMevcutAphb = null;

                                            if (aphbyol != null)
                                            {
                                                dtMevcutAphb = Metodlar.AylikListeyiYukle(aphbyol);
                                            }

                                            if (secenekler.BasvuruFormuIndirilsin)
                                            {
                                                indirilensirket.IndirilenIsyerleri.Add(isyeri.IsyeriID, new IsyeriAphbBasvuruFormuIndirme());
                                                indirilensirket.Isyerleri.Add(isyeri.IsyeriID, isyeri);
                                                

                                                var indirilenIsyeri = indirilensirket.IndirilenIsyerleri[isyeri.IsyeriID];
                                                indirilenIsyeri.formSirketler = this;

                                                if (secenekler.IndirTumTesvikler)
                                                {
                                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeri.IsyeriID, Enums.BasvuruFormuTurleri.BfTumu, cariTanimla);
                                                    basvuruFormuIndir.incelenecekDonemler = secenekler.incelenecekDonemler;
                                                    basvuruFormuIndir.incelenecekDonemler7103 = secenekler.incelenecekDonemler7103;
                                                    basvuruFormuIndir.incelenecekDonemler2828 = secenekler.incelenecekDonemler2828;
                                                    basvuruFormuIndir.incelenecekDonemler7252 = secenekler.incelenecekDonemler7252;
                                                    basvuruFormuIndir.incelenecekDonemler7256 = secenekler.incelenecekDonemler7256;
                                                    basvuruFormuIndir.incelenecekDonemler7316 = secenekler.incelenecekDonemler7316;
                                                    basvuruFormuIndir.incelenecekDonemler3294 = secenekler.incelenecekDonemler3294;
                                                    basvuruFormuIndir.dtBaslangic6111 = secenekler.BaslangicTum;
                                                    basvuruFormuIndir.dtBaslangic7103 = secenekler.BaslangicTum;
                                                    basvuruFormuIndir.dtBaslangic2828 = secenekler.BaslangicTum;
                                                    basvuruFormuIndir.dtBaslangic7252 = secenekler.BaslangicTum;
                                                    basvuruFormuIndir.dtBaslangic7256 = secenekler.BaslangicTum;
                                                    basvuruFormuIndir.dtBaslangic7316 = secenekler.BaslangicTum;
                                                    basvuruFormuIndir.dtBaslangic3294 = secenekler.BaslangicTum;
                                                    basvuruFormuIndir.dtBitis6111 = secenekler.BitisTum;
                                                    basvuruFormuIndir.dtBitis7103 = secenekler.BitisTum;
                                                    basvuruFormuIndir.dtBitis2828 = secenekler.BitisTum;
                                                    basvuruFormuIndir.dtBitis7252 = secenekler.BitisTum;
                                                    basvuruFormuIndir.dtBitis7256 = secenekler.BitisTum;
                                                    basvuruFormuIndir.dtBitis7316 = secenekler.BitisTum;
                                                    basvuruFormuIndir.dtBitis3294 = secenekler.BitisTum;
                                                    basvuruFormuIndir.frmSirketler = this;
                                                    basvuruFormuIndir.EnBastanTumu = secenekler.EnBastanTumu;
                                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                                }

                                                if (secenekler.Indir6111)
                                                {
                                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeri.IsyeriID, Enums.BasvuruFormuTurleri.Bf6111, cariTanimla);
                                                    basvuruFormuIndir.incelenecekDonemler = secenekler.incelenecekDonemler;
                                                    basvuruFormuIndir.dtBaslangic6111 = secenekler.Baslangic6111;
                                                    basvuruFormuIndir.dtBitis6111 = secenekler.BitisTum;
                                                    basvuruFormuIndir.frmSirketler = this;
                                                    basvuruFormuIndir.EnBastan6111 = secenekler.EnBastan6111;
                                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                                }

                                                if (secenekler.Indir7103)
                                                {
                                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeri.IsyeriID, Enums.BasvuruFormuTurleri.Bf7103, cariTanimla);
                                                    basvuruFormuIndir.incelenecekDonemler7103 = secenekler.incelenecekDonemler7103;
                                                    basvuruFormuIndir.dtBaslangic7103 = secenekler.Baslangic7103;
                                                    basvuruFormuIndir.dtBitis7103 = secenekler.Bitis7103;
                                                    basvuruFormuIndir.frmSirketler = this;
                                                    basvuruFormuIndir.EnBastan7103 = secenekler.EnBastan7103;
                                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                                }

                                                if (secenekler.Indir2828)
                                                {
                                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeri.IsyeriID, Enums.BasvuruFormuTurleri.Bf2828, cariTanimla);
                                                    basvuruFormuIndir.incelenecekDonemler2828 = secenekler.incelenecekDonemler2828;
                                                    basvuruFormuIndir.dtBaslangic2828 = secenekler.Baslangic2828;
                                                    basvuruFormuIndir.dtBitis2828 = secenekler.Bitis2828;
                                                    basvuruFormuIndir.frmSirketler = this;
                                                    basvuruFormuIndir.EnBastan2828 = secenekler.EnBastan2828;
                                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                                }

                                                if (secenekler.Indir7252)
                                                {
                                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeri.IsyeriID, Enums.BasvuruFormuTurleri.Bf7252, cariTanimla);
                                                    basvuruFormuIndir.incelenecekDonemler7252 = secenekler.incelenecekDonemler7252;
                                                    basvuruFormuIndir.dtBaslangic7252 = secenekler.Baslangic7252;
                                                    basvuruFormuIndir.dtBitis7252 = secenekler.Bitis7252;
                                                    basvuruFormuIndir.frmSirketler = this;
                                                    basvuruFormuIndir.EnBastan7252 = secenekler.EnBastan7252;
                                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                                }

                                                if (secenekler.Indir7256)
                                                {
                                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeri.IsyeriID, Enums.BasvuruFormuTurleri.Bf7256, cariTanimla);
                                                    basvuruFormuIndir.incelenecekDonemler7256 = secenekler.incelenecekDonemler7256;
                                                    basvuruFormuIndir.dtBaslangic7256 = secenekler.Baslangic7256;
                                                    basvuruFormuIndir.dtBitis7256 = secenekler.Bitis7256;
                                                    basvuruFormuIndir.frmSirketler = this;
                                                    basvuruFormuIndir.EnBastan7256 = secenekler.EnBastan7256;
                                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                                }

                                                if (secenekler.Indir7316)
                                                {
                                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeri.IsyeriID, Enums.BasvuruFormuTurleri.Bf7316, cariTanimla);
                                                    basvuruFormuIndir.incelenecekDonemler7316 = secenekler.incelenecekDonemler7316;
                                                    basvuruFormuIndir.dtBaslangic7316 = secenekler.Baslangic7316;
                                                    basvuruFormuIndir.dtBitis7316 = secenekler.Bitis7316;
                                                    basvuruFormuIndir.frmSirketler = this;
                                                    basvuruFormuIndir.EnBastan7316 = secenekler.EnBastan7316;
                                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                                }

                                                if (secenekler.Indir3294)
                                                {
                                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeri.IsyeriID, Enums.BasvuruFormuTurleri.Bf3294, cariTanimla);
                                                    basvuruFormuIndir.incelenecekDonemler3294 = secenekler.incelenecekDonemler3294;
                                                    basvuruFormuIndir.dtBaslangic3294 = secenekler.Baslangic3294;
                                                    basvuruFormuIndir.dtBitis3294 = secenekler.Bitis3294;
                                                    basvuruFormuIndir.frmSirketler = this;
                                                    basvuruFormuIndir.EnBastan3294 = secenekler.EnBastan3294;
                                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                                }

                                                if (secenekler.Indir6645)
                                                {
                                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeri.IsyeriID, Enums.BasvuruFormuTurleri.Bf6645, cariTanimla);
                                                    basvuruFormuIndir.dtBaslangic6645 = secenekler.Baslangic6645;
                                                    basvuruFormuIndir.dtBitis6645 = secenekler.Bitis6645;
                                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                                    basvuruFormuIndir.frmSirketler = this;
                                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                                }

                                                if (secenekler.Indir687)
                                                {
                                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeri.IsyeriID, Enums.BasvuruFormuTurleri.Bf687, cariTanimla);
                                                    basvuruFormuIndir.dtBaslangic687 = secenekler.Baslangic687;
                                                    basvuruFormuIndir.dtBitis687 = secenekler.Bitis687;
                                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                                    basvuruFormuIndir.frmSirketler = this;
                                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                                }

                                                if (secenekler.Indir14857)
                                                {
                                                    var basvuruFormuIndir = new BasvuruFormuIndir(isyeri.IsyeriID, Enums.BasvuruFormuTurleri.Bf14857, cariTanimla);
                                                    basvuruFormuIndir.dtBaslangic14857 = secenekler.Baslangic14857;
                                                    basvuruFormuIndir.dtBitis14857 = secenekler.Bitis14857;
                                                    basvuruFormuIndir.indirilenIsyeri = indirilenIsyeri;

                                                    basvuruFormuIndir.frmSirketler = this;
                                                    indirilenIsyeri.BasvuruFormuIndirmeleri.Add(basvuruFormuIndir);
                                                }


                                                indirilenIsyeri.isyeri = indirilenIsyeri.BasvuruFormuIndirmeleri.FirstOrDefault().SuanYapilanIsyeriBasvuru;

                                                indirilenIsyeri.BasvuruFormuIndirmeleri.ForEach(p => p.dtMevcutAphb = dtMevcutAphb);
                                            }

                                            if (secenekler.AphbIndirilsin)
                                            {
                                                var aphbIndir = new AphbIndir(isyeri.IsyeriID);

                                                if (!indirilensirket.IndirilenIsyerleri.ContainsKey(isyeri.IsyeriID))
                                                {
                                                    indirilensirket.IndirilenIsyerleri.Add(isyeri.IsyeriID, new IsyeriAphbBasvuruFormuIndirme());
                                                    indirilensirket.Isyerleri.Add(isyeri.IsyeriID, isyeri);
                                                }
                                                var indirilenIsyeri = indirilensirket.IndirilenIsyerleri[isyeri.IsyeriID];
                                                indirilenIsyeri.formSirketler = this;

                                                aphbIndir.TarihBaslangicAphb = secenekler.BaslangicAphb;
                                                aphbIndir.TarihBitisAphb = secenekler.BitisAphb;
                                                aphbIndir.frmSirketler = this;
                                                aphbIndir.dtMevcutAphb = dtMevcutAphb;
                                                aphbIndir.indirilenIsyeri = indirilenIsyeri;

                                                indirilenIsyeri.AphbIndirmeleri.Add(aphbIndir);

                                                indirilenIsyeri.isyeri = indirilenIsyeri.AphbIndirmeleri.FirstOrDefault().SuanYapilanIsyeriAphb;

                                            }
                                        }

                                        if (Program.IndirilenSirketler.ContainsKey(sirket.SirketID))
                                        {
                                            indirilensirket = Program.IndirilenSirketler[sirket.SirketID];

                                            foreach (var item in indirilensirket.IndirilenIsyerleri)
                                            {
                                                var indirilenisyeri = item.Value;

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

                                                break;
                                            }

                                            var formSirketIndirmeEkrani = new frmSirketIndirmeEkrani(indirilensirket, this);
                                            indirilensirket.formSirketIndirmeEkrani = formSirketIndirmeEkrani;
                                            formSirketIndirmeEkrani.Text = String.Format("{0}", indirilensirket.sirket.SirketAdi);
                                            formSirketIndirmeEkrani.Show();

                                        }
                                    }
                                    else MessageBox.Show("Herhangi bir işyeri seçmediniz", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                            else MessageBox.Show("Şirkete ait aktif bir işyeri bulunamadı", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        }
                    }

                }
            }
        }

        public void IndirmeBitti(Indir indir)
        {
            var indirilensirketdeger = Program.IndirilenSirketler.FirstOrDefault(p => p.Value.IndirilenIsyerleri.ContainsKey(indir.IsyeriId));

            IsyeriAphbBasvuruFormuIndirme tamamlanan = null;

            if (indirilensirketdeger.Value != null)
            {
                var sirketId = indirilensirketdeger.Key;
                var indirilensirket = indirilensirketdeger.Value;
                var indirilenisyeri = indirilensirket.IndirilenIsyerleri[indir.IsyeriId];

                var baslatilmayanIndirmeler = new List<Indir>();
                baslatilmayanIndirmeler.AddRange(indirilenisyeri.AphbIndirmeleri.Where(p => p.IndirmeSonucu.Baslatildi == false && p.IndirmeSonucu.IptalEdildi == false));
                baslatilmayanIndirmeler.AddRange(indirilenisyeri.BasvuruFormuIndirmeleri.Where(p => p.IndirmeSonucu.Baslatildi == false && p.IndirmeSonucu.IptalEdildi == false));

                if (baslatilmayanIndirmeler.Count == 0)
                {
                    if (indirilenisyeri.AphbIndirmeleri.All(p => p.IndirmeSonucu.Tamamlandi) && indirilenisyeri.BasvuruFormuIndirmeleri.All(p => p.IndirmeSonucu.Tamamlandi))
                    {
                        tamamlanan = indirilenisyeri;

                        IsyeriAphbBasvuruFormuIndirme baslamamisIsyeri = null;

                        foreach (var digerindirilenisyeri in indirilensirket.IndirilenIsyerleri)
                        {
                            if (digerindirilenisyeri.Value.AphbIndirmeleri.Count > 0)
                            {
                                foreach (var aphbIndirme in digerindirilenisyeri.Value.AphbIndirmeleri)
                                {
                                    if (aphbIndirme.IndirmeSonucu.Baslatildi == false)
                                    {
                                        baslamamisIsyeri = digerindirilenisyeri.Value;

                                        break;
                                    }
                                }

                                if (baslamamisIsyeri != null) break;
                            }
                            else
                            {
                                foreach (var basvuruFormuIndirme in digerindirilenisyeri.Value.BasvuruFormuIndirmeleri)
                                {
                                    if (basvuruFormuIndirme.IndirmeSonucu.Baslatildi == false)
                                    {
                                        baslamamisIsyeri = digerindirilenisyeri.Value;

                                        break;
                                    }
                                }

                                if (baslamamisIsyeri != null) break;
                            }
                        }

                        if (baslamamisIsyeri != null)
                        {
                            if (baslamamisIsyeri.AphbIndirmeleri.Count > 0)
                            {
                                foreach (var aphbIndirme in baslamamisIsyeri.AphbIndirmeleri)
                                {
                                    _ = aphbIndirme.Calistir();
                                }
                            }
                            else
                            {
                                foreach (var basvuruFormuIndirme in baslamamisIsyeri.BasvuruFormuIndirmeleri)
                                {
                                    _ = basvuruFormuIndirme.Calistir();
                                }
                            }
                        }
                        else
                        {
                            var tamamlandi = indirilensirket.IndirilenIsyerleri.All(x => x.Value.AphbIndirmeleri.All(p => p.IndirmeSonucu.Tamamlandi) && x.Value.BasvuruFormuIndirmeleri.All(p => p.IndirmeSonucu.Tamamlandi));

                            if (tamamlandi)
                            {
                                Program.IndirilenSirketler.Remove(sirketId);
                            }
                        }
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


                if (indirilenisyeri.formIndirmeEkrani != null && !indirilenisyeri.formIndirmeEkrani.IsDisposed)
                {
                    indirilenisyeri.formIndirmeEkrani.Guncelle(indirilenisyeri);
                }

                if (tamamlanan != null)
                {
                    if (indirilensirket.formSirketIndirmeEkrani != null && !indirilensirket.formSirketIndirmeEkrani.IsDisposed)
                    {
                        indirilensirket.formSirketIndirmeEkrani.Guncelle(tamamlanan);
                    }
                }
            }

            dr = DialogResult.OK;
        }

        private void dgvSirketler_SelectionChanged(object sender, EventArgs e)
        {
            dgvSirketler.ClearSelection();
        }

        private void frmSirketler_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.IndirilenSirketler.Count > 0)
            {
                var indirilenSirketAdlari = String.Join(Environment.NewLine, Program.IndirilenSirketler.Select(p => String.Format("{0}{1}", p.Value.sirket.SirketAdi, Environment.NewLine)));

                var iptaledilsin = MessageBox.Show(indirilenSirketAdlari+Environment.NewLine+"Form indirilmesi devam eden şirketler var. Bunları iptal etmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes;

                if (iptaledilsin)
                {
                    var iptaledilecekler = Program.IndirilenSirketler.Select(p => p.Value).ToArray();

                    foreach (var item in iptaledilecekler)
                    {
                        item.TumunuIptalEt();
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }

            DialogResult = dr;
        }

        private void chkPasifleriGoster_CheckedChanged(object sender, EventArgs e)
        {
            SirketleriDoldur(chkPasifleriGoster.Checked);

            if (Ara) Search();

        }

        private void btnPasifYap_Click(object sender, EventArgs e)
        {
            if (secilenler.Count > 0)
            {

                using (var dbContext = new DbEntities())
                {
                    dbContext.Sirketler.Where(p => secilenler.Contains(p.SirketID)).ToList().ForEach(p => p.Aktif = 0);

                    dbContext.SaveChanges();
                }

                secilenler.Clear();

                SirketleriDoldur(chkPasifleriGoster.Checked);

                if (Ara) Search();

            }
            else MessageBox.Show("Herhangi bir şirket seçilmedi", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);


        }

        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            if (txtAra.Text.Length >= 3)
            {
                Ara = true;

                Search();
            }
            else
            {
                if (Ara)
                {
                    Ara = false;

                    this.SirketleriDoldur(chkPasifleriGoster.Checked);
                }

                Ara = false;
            }

        }

        private void Search()
        {
            //if (this.sirketListesi == null)
            //{
            //    using (var dbContext = new DbEntities())
            //    {
            //        this.sirketListesi = dbContext.Sirketler.ToList();
            //    }
            //}

            if (this.sirketListesi.Count > 0)
            {
                var sonuc = this.sirketListesi.Where(
                        a => a.SirketAdi.ToUpper().Contains(txtAra.Text.ToUpper())
                        || a.VergiKimlikNo.ToUpper().Contains(txtAra.Text.ToUpper())
                    ).ToList();

                if (!chkPasifleriGoster.Checked)
                {
                    sonuc = sonuc.Where(a => Convert.ToBoolean(a.Aktif)).ToList();
                }

                dgvSirketler.DataSource = sonuc;

                if (secilenler.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvSirketler.Rows)
                    {
                        row.Cells[0].Value = secilenler.Contains((row.DataBoundItem as Sirketler).SirketID);
                    }
                }

            }
        }

        private void btnTumunuSecKaldir_Click(object sender, EventArgs e)
        {
            if (dgvSirketler.Rows.Count > 0)
            {
                bool sonuncuSecili = !(dgvSirketler.Rows[dgvSirketler.RowCount - 1].Cells[0].Value == null || (bool)dgvSirketler.Rows[dgvSirketler.RowCount - 1].Cells[0].Value == false);

                foreach (DataGridViewRow row in dgvSirketler.Rows)
                {
                    row.Cells[0].Value = !sonuncuSecili;

                    var sirketID = (row.DataBoundItem as Sirketler).SirketID;

                    if (!sonuncuSecili)
                    {
                        if (!secilenler.Contains(sirketID)) secilenler.Add(sirketID);
                    }
                    else
                    {
                        if (secilenler.Contains(sirketID)) secilenler.Remove(sirketID);

                    }


                }

            }
        }

        private void btnSecilenleriSil_Click(object sender, EventArgs e)
        {

            if (secilenler.Count > 0)
            {

                if (MessageBox.Show("Seçili işyerlerini silmek istediğinizden emin misiniz", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    foreach (var item in secilenler)
                    {
                        Sil(item);
                    }

                    secilenler.Clear();

                    SirketleriDoldur(chkPasifleriGoster.Checked);

                    if (Ara) Search();

                    MessageBox.Show("Başarıyla silindi");
                }
            }

        }

        void Sil(long sirketID)
        {

            using (var dbContext = new DbEntities())
            {
                var sirket = dbContext.Sirketler.FirstOrDefault(p => p.SirketID.Equals(sirketID));
                dbContext.Sirketler.Remove(sirket);
                dbContext.SaveChanges();



                dr = DialogResult.OK;

                if (eskielementID > 0 && sirketID.Equals(eskielementID)) AlanlariTemizle();
            }

            string sirketpath = null;

            if (Directory.Exists(Path.Combine(Application.StartupPath, "dosyalar")))
            {
                var paths = Directory.GetDirectories(Path.Combine(Application.StartupPath, "dosyalar"));

                foreach (var item in paths)
                {
                    if (item.EndsWith("-" + sirketID))
                    {
                        sirketpath = item;

                        break;
                    }
                }

                if (sirketpath != null)
                {
                    try
                    {
                        Directory.Delete(sirketpath, true);
                    }
                    catch { }

                }
            }


        }

        private void btnSecilenleriCari14857yeAktar_Click(object sender, EventArgs e)
        {

            List<string> hataverenler = new List<string>();

            if (secilenler.Count > 0)
            {
                using (var dbContext = new DbEntities())
                {
                    foreach (var sirketId in secilenler)
                    {
                        var eklenecek = new Cari14857YapilanSirketler { SirketId = sirketId };

                        bool hatavar = false;

                        try
                        {
                            dbContext.Cari14857YapilanSirketler.Add(eklenecek);
                            dbContext.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException != null && ex.InnerException.InnerException != null)
                            {
                                if (ex.InnerException.InnerException.Message.Contains("UNIQUE constraint failed: Cari14857YapilanSirketler.SirketId"))
                                {

                                }
                                else hatavar = true;
                            }
                            else hatavar = true;

                        }

                        if (hatavar) hataverenler.Add(dbContext.Sirketler.Find(sirketId).SirketAdi);

                    }

                }

                if (hataverenler.Count == 0)
                {
                    secilenler.Clear();

                    SirketleriDoldur(chkPasifleriGoster.Checked);

                    if (Ara) Search();

                    MessageBox.Show("Seçilen şirketler başarıyla Cari 14857 listesine aktarıldı");
                }
                else
                {
                    MessageBox.Show(String.Join(Environment.NewLine, hataverenler) + Environment.NewLine + Environment.NewLine + "Yukarıdaki şirketler cari 14857 listesine hatadan dolayı eklenemedi", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
