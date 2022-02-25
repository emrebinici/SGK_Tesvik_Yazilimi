using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TesvikProgrami
{
    public partial class frmTarihSec : Form
    {
        DialogResult dr = DialogResult.No;

        public Classes.FormIndirmeTarihSecenekleri secenekler = new Classes.FormIndirmeTarihSecenekleri();

        public bool VeritabanindanCek = true;

        //public bool IndirTumTesvikler = false;
        //public bool Indir6111 = false;
        //public bool Indir687 = false;
        //public bool Indir6645 = false;
        //public bool Indir7103 = false;
        //public bool Indir2828 = false;
        //public bool Indir14857 = false;

        //public DateTime dtBaslangic687 = DateTime.MinValue;
        //public DateTime dtBitis687 = DateTime.MinValue;

        //public DateTime dtBaslangic6111 = DateTime.MinValue;
        //public DateTime dtBitis6111 = DateTime.MinValue;

        //public DateTime dtBaslangic6645 = DateTime.MinValue;
        //public DateTime dtBitis6645 = DateTime.MinValue;

        //public DateTime dtBaslangic14857 = DateTime.MinValue;
        //public DateTime dtBitis14857 = DateTime.MinValue;

        //public DateTime dtBaslangic2828 = DateTime.MinValue;
        //public DateTime dtBitis2828 = DateTime.MinValue;

        //public DateTime dtBaslangic7103 = DateTime.MinValue;
        //public DateTime dtBitis7103 = DateTime.MinValue;


        //public DateTime TarihBaslangicAphb = DateTime.MinValue;
        //public DateTime TarihBitisAphb = DateTime.MinValue;

        //public bool AphbIndirilsin = false;
        //public bool BasvuruFormuIndirilsin = false;

        //public List<string> incelenecekDonemler = new List<string>();
        //public List<string> incelenecekDonemler7103 = new List<string>();
        //public List<string> incelenecekDonemler2828 = new List<string>();

        bool CariTanimla = true;

        public frmTarihSec(bool Cari)
        {
            CariTanimla = Cari;

            InitializeComponent();

        }

        public frmTarihSec(Classes.FormIndirmeTarihSecenekleri psecenekler)
        {
            InitializeComponent();

            secenekler = psecenekler;

            VeritabanindanCek = false;
        }

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddMonths(1))
                yield return day;
        }

        private void lnklblDevam_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            {
                {

                    DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYilAphb.Text), Convert.ToInt32(cmbTarihBaslangicAyAphb.SelectedIndex + 1), 1);

                    DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYilAphb.Text), Convert.ToInt32(cmbTarihBitisAyAphb.SelectedIndex + 1), 1);

                    if (TarihBaslangic > TarihBitis)
                    {
                        MessageBox.Show("Aphb başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        secenekler.BaslangicAphb = TarihBaslangic;

                        secenekler.BitisAphb = TarihBitis;

                        secenekler.AphbIndirilsin = chkAphbIndir.Checked;

                        dr = DialogResult.OK;

                    }
                }
            }

            //if (chkBasvuruFormuIndir.Checked)
            {

                //if (chkTumTesvikler.Checked || chk6111.Checked || chk687.Checked || chk6645.Checked || chk7103.Checked || chk2828.Checked || chk14857.Checked)
                {
                    bool devam = true;

                    DateTime ayinIlkGunu = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                    //if (chkTumTesvikler.Checked)
                    {
                        DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYilTum.Text), Convert.ToInt32(cmbTarihBaslangicAyTum.SelectedIndex + 1), 1);
                        DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYilTum.Text), Convert.ToInt32(cmbTarihBitisAyTum.SelectedIndex + 1), 1);

                        if (TarihBaslangic > TarihBitis)
                        {
                            if (chkTumTesvikler.Checked)
                            {
                                devam = false;

                                MessageBox.Show("Tüm teşvikler başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            secenekler.BaslangicTum = TarihBaslangic;

                            secenekler.BitisTum = TarihBitis;
                        }

                        if (chkTumTesvikler.Checked)
                        {

                            secenekler.incelenecekDonemler.Clear();
                            secenekler.incelenecekDonemler7103.Clear();
                            secenekler.incelenecekDonemler2828.Clear();
                            secenekler.incelenecekDonemler7252.Clear();
                            secenekler.incelenecekDonemler7256.Clear();
                            secenekler.incelenecekDonemler7316.Clear();
                            secenekler.incelenecekDonemler3294.Clear();

                            foreach (DateTime day in this.EachDay(TarihBaslangic, TarihBitis))
                            {
                                //if (day >= ayinIlkGunu) continue;

                                secenekler.incelenecekDonemler.Add(string.Format("{0}/{1}", day.Year, day.ToString("MM")));
                                secenekler.incelenecekDonemler7103.Add(string.Format("{0}/{1}", day.Year, day.ToString("MM")));
                                secenekler.incelenecekDonemler2828.Add(string.Format("{0}/{1}", day.Year, day.ToString("MM")));
                                secenekler.incelenecekDonemler7252.Add(string.Format("{0}/{1}", day.Year, day.ToString("MM")));
                                secenekler.incelenecekDonemler7256.Add(string.Format("{0}/{1}", day.Year, day.ToString("MM")));
                                secenekler.incelenecekDonemler7316.Add(string.Format("{0}/{1}", day.Year, day.ToString("MM")));
                                secenekler.incelenecekDonemler3294.Add(string.Format("{0}/{1}", day.Year, day.ToString("MM")));
                            }

                            secenekler.incelenecekDonemler.Reverse();
                            secenekler.incelenecekDonemler7103.Reverse();
                            secenekler.incelenecekDonemler2828.Reverse();
                            secenekler.incelenecekDonemler7252.Reverse();
                            secenekler.incelenecekDonemler7256.Reverse();
                            secenekler.incelenecekDonemler7316.Reverse();
                            secenekler.incelenecekDonemler3294.Reverse();

                            secenekler.EnBastanTumu = chkTumuEnBastan.Checked;
                        }
                    }

                    //if (chk6111.Checked)
                    {
                        DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil6111.Text), Convert.ToInt32(cmbTarihBaslangicAy6111.SelectedIndex + 1), 1);
                        DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil6111.Text), Convert.ToInt32(cmbTarihBitisAy6111.SelectedIndex + 1), 1);


                        if (TarihBaslangic > TarihBitis)
                        {
                            if (chk6111.Checked)
                            {
                                devam = false;
                                MessageBox.Show("6111 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                        }
                        else
                        {
                            secenekler.Baslangic6111 = TarihBaslangic;

                            if (TarihBaslangic < new DateTime(2011, 3, 1))
                            {
                                secenekler.Baslangic6111 = new DateTime(2011, 3, 1);
                            }

                            secenekler.Bitis6111 = TarihBitis;

                            if (chk6111.Checked)
                            {

                                secenekler.incelenecekDonemler.Clear();

                                foreach (DateTime day in this.EachDay(TarihBaslangic, TarihBitis))
                                {
                                    if (day >= ayinIlkGunu) continue;

                                    secenekler.incelenecekDonemler.Add(string.Format("{0}/{1}", day.Year, day.ToString("MM")));
                                }

                                secenekler.incelenecekDonemler.Reverse();

                                secenekler.EnBastan6111 = chk6111EnBastan.Checked;
                            }
                        }

                    }

                    //if (chk6645.Checked)
                    {
                        DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil6645.Text), Convert.ToInt32(cmbTarihBaslangicAy6645.SelectedIndex + 1), 1);
                        DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil6645.Text), Convert.ToInt32(cmbTarihBitisAy6645.SelectedIndex + 1), 1);

                        if (TarihBaslangic > TarihBitis)
                        {
                            if (chk6645.Checked)
                            {
                                devam = false;

                                MessageBox.Show("6645 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            secenekler.Baslangic6645 = TarihBaslangic;

                            if (TarihBaslangic < new DateTime(2015, 4, 1))
                            {
                                secenekler.Baslangic6645 = new DateTime(2015, 4, 1);
                            }

                            secenekler.Bitis6645 = TarihBitis;
                        }
                    }

                    //if (chk7103.Checked)
                    {
                        DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil7103.Text), Convert.ToInt32(cmbTarihBaslangicAy7103.SelectedIndex + 1), 1);
                        DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil7103.Text), Convert.ToInt32(cmbTarihBitisAy7103.SelectedIndex + 1), 1);

                        if (TarihBaslangic > TarihBitis)
                        {
                            if (chk7103.Checked)
                            {
                                devam = false;

                                MessageBox.Show("7103 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            secenekler.Baslangic7103 = TarihBaslangic;

                            if (TarihBaslangic < new DateTime(2018, 1, 1))
                            {
                                secenekler.Baslangic7103 = new DateTime(2018, 1, 1);
                            }

                            secenekler.Bitis7103 = TarihBitis;

                            if (chk7103.Checked)
                            {

                                secenekler.incelenecekDonemler7103.Clear();
                                foreach (DateTime day in this.EachDay(TarihBaslangic, TarihBitis))
                                {
                                    if (day >= ayinIlkGunu) continue;

                                    secenekler.incelenecekDonemler7103.Add(string.Format("{0}/{1}", day.Year, day.ToString("MM")));
                                }

                                secenekler.incelenecekDonemler7103.Reverse();

                                secenekler.EnBastan7103 = chk7103EnBastan.Checked;
                            }
                        }
                    }

                    //if (chk687.Checked)
                    {
                        DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil687.Text), Convert.ToInt32(cmbTarihBaslangicAy687.SelectedIndex + 1), 1);
                        DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil687.Text), Convert.ToInt32(cmbTarihBitisAy687.SelectedIndex + 1), 1);

                        if (TarihBaslangic > TarihBitis)
                        {
                            if (chk687.Checked)
                            {
                                devam = false;

                                MessageBox.Show("687 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            secenekler.Baslangic687 = TarihBaslangic;

                            if (TarihBaslangic < new DateTime(2017, 2, 1))
                            {
                                secenekler.Baslangic687 = new DateTime(2017, 2, 1);
                            }

                            secenekler.Bitis687 = TarihBitis;
                        }
                    }

                    //if (chk2828.Checked)
                    {
                        DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil2828.Text), Convert.ToInt32(cmbTarihBaslangicAy2828.SelectedIndex + 1), 1);
                        DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil2828.Text), Convert.ToInt32(cmbTarihBitisAy2828.SelectedIndex + 1), 1);

                        if (TarihBaslangic > TarihBitis)
                        {

                            if (chk2828.Checked)
                            {
                                devam = false;

                                MessageBox.Show("2828 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            }

                        }
                        else
                        {
                            if (chk2828.Checked)
                            {
                                secenekler.incelenecekDonemler2828.Clear();

                                foreach (DateTime day in this.EachDay(TarihBaslangic, TarihBitis))
                                {
                                    if (day >= ayinIlkGunu) continue;

                                    secenekler.incelenecekDonemler2828.Add(string.Format("{0}/{1}", day.Year, day.ToString("MM")));
                                }

                                secenekler.incelenecekDonemler2828.Reverse();

                                secenekler.EnBastan2828 = chk2828EnBastan.Checked;
                            }

                            secenekler.Baslangic2828 = TarihBaslangic;

                            secenekler.Bitis2828 = TarihBitis;
                        }
                    }

                    //if (chk7252.Checked)
                    {
                        DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil7252.Text), Convert.ToInt32(cmbTarihBaslangicAy7252.SelectedIndex + 1), 1);
                        DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil7252.Text), Convert.ToInt32(cmbTarihBitisAy7252.SelectedIndex + 1), 1);

                        if (TarihBaslangic > TarihBitis)
                        {

                            if (chk7252.Checked)
                            {
                                devam = false;

                                MessageBox.Show("7252 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            }

                        }
                        else
                        {
                            if (chk7252.Checked)
                            {
                                secenekler.incelenecekDonemler7252.Clear();

                                foreach (DateTime day in this.EachDay(TarihBaslangic, TarihBitis))
                                {
                                    if (day >= ayinIlkGunu) continue;

                                    secenekler.incelenecekDonemler7252.Add(string.Format("{0}/{1}", day.Year, day.ToString("MM")));
                                }

                                secenekler.incelenecekDonemler7252.Reverse();

                                secenekler.EnBastan7252 = chk7252EnBastan.Checked;
                            }

                            secenekler.Baslangic7252 = TarihBaslangic;

                            secenekler.Bitis7252 = TarihBitis;
                        }
                    }

                    //if (chk7256.Checked)
                    {
                        DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil7256.Text), Convert.ToInt32(cmbTarihBaslangicAy7256.SelectedIndex + 1), 1);
                        DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil7256.Text), Convert.ToInt32(cmbTarihBitisAy7256.SelectedIndex + 1), 1);

                        if (TarihBaslangic > TarihBitis)
                        {

                            if (chk7256.Checked)
                            {
                                devam = false;

                                MessageBox.Show("7256 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            }

                        }
                        else
                        {
                            if (chk7256.Checked)
                            {
                                secenekler.incelenecekDonemler7256.Clear();

                                foreach (DateTime day in this.EachDay(TarihBaslangic, TarihBitis))
                                {
                                    //if (day >= ayinIlkGunu) continue;

                                    secenekler.incelenecekDonemler7256.Add(string.Format("{0}/{1}", day.Year, day.ToString("MM")));
                                }

                                secenekler.incelenecekDonemler7256.Reverse();

                                secenekler.EnBastan7256 = chk7256EnBastan.Checked;
                            }

                            secenekler.Baslangic7256 = TarihBaslangic;

                            secenekler.Bitis7256 = TarihBitis;
                        }
                    }

                    //if (chk7316.Checked)
                    {
                        DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil7316.Text), Convert.ToInt32(cmbTarihBaslangicAy7316.SelectedIndex + 1), 1);
                        DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil7316.Text), Convert.ToInt32(cmbTarihBitisAy7316.SelectedIndex + 1), 1);

                        if (TarihBaslangic > TarihBitis)
                        {

                            if (chk7316.Checked)
                            {
                                devam = false;

                                MessageBox.Show("7316 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            }

                        }
                        else
                        {
                            if (chk7316.Checked)
                            {
                                secenekler.incelenecekDonemler7316.Clear();

                                foreach (DateTime day in this.EachDay(TarihBaslangic, TarihBitis))
                                {
                                    //if (day >= ayinIlkGunu) continue;

                                    secenekler.incelenecekDonemler7316.Add(string.Format("{0}/{1}", day.Year, day.ToString("MM")));
                                }

                                secenekler.incelenecekDonemler7316.Reverse();

                                secenekler.EnBastan7316 = chk7316EnBastan.Checked;
                            }

                            secenekler.Baslangic7316 = TarihBaslangic;

                            secenekler.Bitis7316 = TarihBitis;
                        }
                    }

                    //if (chk3294.Checked)
                    {
                        DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil3294.Text), Convert.ToInt32(cmbTarihBaslangicAy3294.SelectedIndex + 1), 1);
                        DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil3294.Text), Convert.ToInt32(cmbTarihBitisAy3294.SelectedIndex + 1), 1);

                        if (TarihBaslangic > TarihBitis)
                        {

                            if (chk3294.Checked)
                            {
                                devam = false;

                                MessageBox.Show("3294 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            }

                        }
                        else
                        {
                            if (chk3294.Checked)
                            {
                                secenekler.incelenecekDonemler3294.Clear();

                                foreach (DateTime day in this.EachDay(TarihBaslangic, TarihBitis))
                                {
                                    //if (day >= ayinIlkGunu) continue;

                                    secenekler.incelenecekDonemler3294.Add(string.Format("{0}/{1}", day.Year, day.ToString("MM")));
                                }

                                secenekler.incelenecekDonemler3294.Reverse();

                                secenekler.EnBastan3294 = chk3294EnBastan.Checked;
                            }

                            secenekler.Baslangic3294 = TarihBaslangic;

                            secenekler.Bitis3294 = TarihBitis;
                        }
                    }

                    //if (chk14857.Checked)
                    {
                        DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil14857.Text), Convert.ToInt32(cmbTarihBaslangicAy14857.SelectedIndex + 1), 1);
                        DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil14857.Text), Convert.ToInt32(cmbTarihBitisAy14857.SelectedIndex + 1), 1);

                        if (TarihBaslangic > TarihBitis)
                        {
                            if (chk14857.Checked)
                            {
                                devam = false;

                                MessageBox.Show("14857 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                        }
                        else
                        {
                            secenekler.Baslangic14857 = TarihBaslangic;

                            if (TarihBaslangic < new DateTime(2013, 9, 1))
                            {
                                secenekler.Baslangic14857 = new DateTime(2013, 9, 1);
                            }


                            secenekler.Bitis14857 = TarihBitis;
                        }
                    }

                    if (devam)
                    {
                        secenekler.IndirTumTesvikler = chkTumTesvikler.Checked;
                        secenekler.Indir6111 = chk6111.Checked;
                        secenekler.Indir7103 = chk7103.Checked;
                        secenekler.Indir2828 = chk2828.Checked;
                        secenekler.Indir7252 = chk7252.Checked;
                        secenekler.Indir7256 = chk7256.Checked;
                        secenekler.Indir7316 = chk7316.Checked;
                        secenekler.Indir3294 = chk3294.Checked;
                        secenekler.Indir6645 = chk6645.Checked;
                        secenekler.Indir687 = chk687.Checked;
                        secenekler.Indir14857 = chk14857.Checked;
                        secenekler.BasvuruFormuIndirilsin = chkBasvuruFormuIndir.Checked;

                        dr = DialogResult.OK;
                    }
                    else dr = DialogResult.None;

                }

            }

            if (dr == DialogResult.OK) this.Close();

        }
        private void frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = dr;
        }

        private void frm_Load(object sender, EventArgs e)
        {
            List<Ayarlar> ayarlar = new List<Ayarlar>();
            using (var dbContext = new DbEntities())
            {
                ayarlar = dbContext.Ayarlar.ToList();
            }

            if (CariTanimla)
            {
                chkTumuEnBastan.Checked = false;
                chk6111EnBastan.Checked = false;
                chk7103EnBastan.Checked = false;
                chk2828EnBastan.Checked = false;
                chk7252EnBastan.Checked = false;
                chk7256EnBastan.Checked = false;
                chk7316EnBastan.Checked = false;
                chk3294EnBastan.Checked = false;
                chkEnBastan_Click(chkTumuEnBastan, null);
                chkEnBastan_Click(chk6111EnBastan, null);
                chkEnBastan_Click(chk7103EnBastan, null);
                chkEnBastan_Click(chk2828EnBastan, null);
                chkEnBastan_Click(chk7252EnBastan, null);
                chkEnBastan_Click(chk7256EnBastan, null);
                chkEnBastan_Click(chk7316EnBastan, null);
                chkEnBastan_Click(chk3294EnBastan, null);

                chkTumuEnBastan.Visible = false;
                chk6111EnBastan.Visible = false;
                chk7103EnBastan.Visible = false;
                chk2828EnBastan.Visible = false;
                chk7252EnBastan.Visible = false;
                chk7256EnBastan.Visible = false;
                chk7316EnBastan.Visible = false;
                chk3294EnBastan.Visible = false;
            }

            if (VeritabanindanCek)
            {
                if (CariTanimla)
                {

                    var anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("AphbIndirilsin"));
                    chkAphbIndir.Checked = anahtar == null || anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BasvuruFormuIndirilsin"));
                    chkBasvuruFormuIndir.Checked = anahtar == null || anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfTumIndirilsin"));
                    chkTumTesvikler.Checked = anahtar == null || anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf6111Indirilsin"));
                    chk6111.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf7103Indirilsin"));
                    chk7103.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf2828Indirilsin"));
                    chk2828.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf7252Indirilsin"));
                    chk7252.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf7256Indirilsin"));
                    chk7256.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf7316Indirilsin"));
                    chk7316.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf3294Indirilsin"));
                    chk3294.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf6645Indirilsin"));
                    chk6645.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf687Indirilsin"));
                    chk687.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf14857Indirilsin"));
                    chk14857.Checked = anahtar != null && anahtar.Deger.Equals("True");
                }
                else
                {
                    var anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisAphbIndirilsin"));
                    chkAphbIndir.Checked = anahtar == null || anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBasvuruFormuIndirilsin"));
                    chkBasvuruFormuIndir.Checked = anahtar == null || anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfTumIndirilsin"));
                    chkTumTesvikler.Checked = anahtar == null || anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf6111Indirilsin"));
                    chk6111.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf7103Indirilsin"));
                    chk7103.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf2828Indirilsin"));
                    chk2828.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf7252Indirilsin"));
                    chk7252.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf7256Indirilsin"));
                    chk7256.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf7316Indirilsin"));
                    chk7316.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf3294Indirilsin"));
                    chk3294.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf6645Indirilsin"));
                    chk6645.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf687Indirilsin"));
                    chk687.Checked = anahtar != null && anahtar.Deger.Equals("True");

                    anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf14857Indirilsin"));
                    chk14857.Checked = anahtar != null && anahtar.Deger.Equals("True");
                }
            }
            else
            {
                chkAphbIndir.Checked = secenekler.AphbIndirilsin;
                chkBasvuruFormuIndir.Checked = secenekler.BasvuruFormuIndirilsin;
                chkTumTesvikler.Checked = secenekler.IndirTumTesvikler;
                chk6111.Checked = secenekler.Indir6111;
                chk7103.Checked = secenekler.Indir7103;
                chk2828.Checked = secenekler.Indir2828;
                chk7252.Checked = secenekler.Indir7252;
                chk7256.Checked = secenekler.Indir7256;
                chk7316.Checked = secenekler.Indir7316;
                chk3294.Checked = secenekler.Indir3294;
                chk6645.Checked = secenekler.Indir6645;
                chk687.Checked = secenekler.Indir687;
                chk14857.Checked = secenekler.Indir14857;
            }

            {
                DateTime tarih = new DateTime(DateTime.Today.Year, 1, 1);

                List<string> Aylar = new List<string>();

                for (int i = 0; i < 12; i++)
                {
                    Aylar.Add(tarih.AddMonths(i).ToString("MMMM"));

                }

                cmbTarihBaslangicAyAphb.Items.AddRange(Aylar.ToArray());

                cmbTarihBitisAyAphb.Items.AddRange(Aylar.ToArray());

                for (int i = 2008; i <= DateTime.Today.Year; i++)
                {
                    cmbTarihBaslangicYilAphb.Items.Add(i);

                    cmbTarihBitisYilAphb.Items.Add(i);
                }

                if (VeritabanindanCek)
                {
                    if (CariTanimla)
                    {
                        var anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("AphbBaslangicYil"));

                        cmbTarihBaslangicYilAphb.Text = anahtar == null ? "2011" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("AphbBaslangicAy"));

                        cmbTarihBaslangicAyAphb.SelectedIndex = anahtar == null ? 2 : Convert.ToInt32(anahtar.Deger) - 1;
                    }
                    else
                    {
                        var anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisAphbBaslangicYil"));

                        cmbTarihBaslangicYilAphb.Text = anahtar == null ? "2011" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisAphbBaslangicAy"));

                        cmbTarihBaslangicAyAphb.SelectedIndex = anahtar == null ? 2 : Convert.ToInt32(anahtar.Deger) - 1;
                    }

                    cmbTarihBitisAyAphb.Text = DateTime.Today.ToString("MMMM");

                    cmbTarihBitisYilAphb.Text = DateTime.Today.Year.ToString();
                }
                else
                {
                    cmbTarihBaslangicYilAphb.Text = secenekler.BaslangicAphb.Year.ToString();
                    cmbTarihBaslangicAyAphb.SelectedIndex = secenekler.BaslangicAphb.Month - 1;

                    cmbTarihBitisYilAphb.Text = secenekler.BitisAphb.Year.ToString();
                    cmbTarihBitisAyAphb.SelectedIndex = secenekler.BitisAphb.Month - 1;
                }


            }

            {


                chkAphbIndir_CheckedChanged(null, null);
                chkBasvuruFormuIndir_CheckedChanged(null, null);
                chk687_CheckedChanged(sender, e);
                chk6111_CheckedChanged(sender, e);
                chk6645_CheckedChanged(sender, e);
                chk7103_CheckedChanged(sender, e);
                chk2828_CheckedChanged(sender, e);
                chk7252_CheckedChanged(sender, e);
                chk7256_CheckedChanged(sender, e);
                chk7316_CheckedChanged(sender, e);
                chk3294_CheckedChanged(sender, e);
                chkTumTesvikler_CheckedChanged(sender, e);
                chk14857_CheckedChanged(sender, e);


                DateTime oncekiAy = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);

                DateTime tarih = new DateTime(2016, 1, 1);

                List<string> Aylar = new List<string>();

                for (int i = 0; i < 12; i++)
                {
                    Aylar.Add(tarih.AddMonths(i).ToString("MMMM"));
                }

                cmbTarihBaslangicAy6111.Items.AddRange(Aylar.ToArray());
                cmbTarihBitisAy6111.Items.AddRange(Aylar.ToArray());

                cmbTarihBaslangicAy687.Items.AddRange(Aylar.ToArray());
                cmbTarihBitisAy687.Items.AddRange(Aylar.ToArray());

                cmbTarihBaslangicAy6645.Items.AddRange(Aylar.ToArray());
                cmbTarihBitisAy6645.Items.AddRange(Aylar.ToArray());

                cmbTarihBaslangicAy7103.Items.AddRange(Aylar.ToArray());
                cmbTarihBitisAy7103.Items.AddRange(Aylar.ToArray());

                cmbTarihBaslangicAy2828.Items.AddRange(Aylar.ToArray());
                cmbTarihBitisAy2828.Items.AddRange(Aylar.ToArray());

                cmbTarihBaslangicAy7252.Items.AddRange(Aylar.ToArray());
                cmbTarihBitisAy7252.Items.AddRange(Aylar.ToArray());

                cmbTarihBaslangicAy7256.Items.AddRange(Aylar.ToArray());
                cmbTarihBitisAy7256.Items.AddRange(Aylar.ToArray());

                cmbTarihBaslangicAy7316.Items.AddRange(Aylar.ToArray());
                cmbTarihBitisAy7316.Items.AddRange(Aylar.ToArray());

                cmbTarihBaslangicAy3294.Items.AddRange(Aylar.ToArray());
                cmbTarihBitisAy3294.Items.AddRange(Aylar.ToArray());

                cmbTarihBaslangicAyTum.Items.AddRange(Aylar.ToArray());
                cmbTarihBitisAyTum.Items.AddRange(Aylar.ToArray());

                cmbTarihBaslangicAy14857.Items.AddRange(Aylar.ToArray());
                cmbTarihBitisAy14857.Items.AddRange(Aylar.ToArray());

                for (int i = 2011; i <= DateTime.Today.Year; i++)
                {
                    cmbTarihBaslangicYil6111.Items.Add(i);
                    cmbTarihBitisYil6111.Items.Add(i);

                    if (i == 2017)
                    {
                        cmbTarihBaslangicYil687.Items.Add(i);
                        cmbTarihBitisYil687.Items.Add(i);
                    }

                    cmbTarihBaslangicYil6645.Items.Add(i);
                    cmbTarihBitisYil6645.Items.Add(i);

                    cmbTarihBaslangicYil7103.Items.Add(i);
                    cmbTarihBitisYil7103.Items.Add(i);

                    cmbTarihBaslangicYil2828.Items.Add(i);
                    cmbTarihBitisYil2828.Items.Add(i);

                    cmbTarihBaslangicYil7252.Items.Add(i);
                    cmbTarihBitisYil7252.Items.Add(i);

                    cmbTarihBaslangicYil7256.Items.Add(i);
                    cmbTarihBitisYil7256.Items.Add(i);

                    cmbTarihBaslangicYil7316.Items.Add(i);
                    cmbTarihBitisYil7316.Items.Add(i);

                    cmbTarihBaslangicYil3294.Items.Add(i);
                    cmbTarihBitisYil3294.Items.Add(i);

                    cmbTarihBaslangicYilTum.Items.Add(i);
                    cmbTarihBitisYilTum.Items.Add(i);

                    cmbTarihBaslangicYil14857.Items.Add(i);
                    cmbTarihBitisYil14857.Items.Add(i);
                }


                if (VeritabanindanCek)
                {
                    if (CariTanimla)
                    {
                        var anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil6111"));
                        cmbTarihBaslangicYil6111.Text = anahtar == null ? "2011" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy6111"));
                        cmbTarihBaslangicAy6111.SelectedIndex = anahtar == null ? 2 : Convert.ToInt32(anahtar.Deger) - 1;

                        cmbTarihBitisAy6111.Text = oncekiAy.ToString("MMMM");
                        cmbTarihBitisYil6111.Text = oncekiAy.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil687"));
                        cmbTarihBaslangicYil687.Text = anahtar == null ? "2017" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy687"));
                        cmbTarihBaslangicAy687.SelectedIndex = anahtar == null ? 1 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy687.SelectedIndex = 11;
                        cmbTarihBitisYil687.Text = "2017";

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil6645"));
                        cmbTarihBaslangicYil6645.Text = anahtar == null ? "2015" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy6645"));
                        cmbTarihBaslangicAy6645.SelectedIndex = anahtar == null ? 3 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy6645.Text = DateTime.Today.ToString("MMMM");
                        cmbTarihBitisYil6645.Text = DateTime.Today.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil7103"));
                        cmbTarihBaslangicYil7103.Text = anahtar == null ? "2018" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy7103"));
                        cmbTarihBaslangicAy7103.SelectedIndex = anahtar == null ? 0 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy7103.Text = oncekiAy.ToString("MMMM");
                        cmbTarihBitisYil7103.Text = oncekiAy.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil2828"));
                        cmbTarihBaslangicYil2828.Text = anahtar == null ? "2014" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy2828"));
                        cmbTarihBaslangicAy2828.SelectedIndex = anahtar == null ? 1 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy2828.Text = oncekiAy.ToString("MMMM");
                        cmbTarihBitisYil2828.Text = oncekiAy.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil7252"));
                        cmbTarihBaslangicYil7252.Text = anahtar == null ? "2017" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy7252"));
                        cmbTarihBaslangicAy7252.SelectedIndex = anahtar == null ? 1 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy7252.Text = oncekiAy.ToString("MMMM");
                        cmbTarihBitisYil7252.Text = oncekiAy.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil7256"));
                        cmbTarihBaslangicYil7256.Text = anahtar == null ? "2020" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy7256"));
                        cmbTarihBaslangicAy7256.SelectedIndex = anahtar == null ? 11 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy7256.Text = oncekiAy.ToString("MMMM");
                        cmbTarihBitisYil7256.Text = oncekiAy.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil7316"));
                        cmbTarihBaslangicYil7316.Text = anahtar == null ? "2020" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy7316"));
                        cmbTarihBaslangicAy7316.SelectedIndex = anahtar == null ? 11 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy7316.Text = oncekiAy.ToString("MMMM");
                        cmbTarihBitisYil7316.Text = oncekiAy.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil3294"));
                        cmbTarihBaslangicYil3294.Text = anahtar == null ? "2020" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy3294"));
                        cmbTarihBaslangicAy3294.SelectedIndex = anahtar == null ? 11 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy3294.Text = oncekiAy.ToString("MMMM");
                        cmbTarihBitisYil3294.Text = oncekiAy.Year.ToString();

                        var cariBaslangic = DateTime.Today.AddMonths(-30);

                        cmbTarihBaslangicYilTum.Text = cariBaslangic.Year.ToString();
                        cmbTarihBaslangicAyTum.SelectedIndex = cariBaslangic.Month - 1;

                        //anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYilTum"));
                        //cmbTarihBaslangicYilTum.Text = anahtar == null ? "2017" : anahtar.Deger;

                        //anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAyTum"));
                        //cmbTarihBaslangicAyTum.SelectedIndex = anahtar == null ? 0 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAyTum.Text = oncekiAy.ToString("MMMM");
                        cmbTarihBitisYilTum.Text = oncekiAy.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil14857"));
                        cmbTarihBaslangicYil14857.Text = anahtar == null ? "2013" : oncekiAy.AddMonths(-1).Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy14857"));
                        cmbTarihBaslangicAy14857.SelectedIndex = anahtar == null ? 8 : oncekiAy.AddMonths(-1).Month - 1;

                        cmbTarihBitisAy14857.Text = DateTime.Today.ToString("MMMM");
                        cmbTarihBitisYil14857.Text = DateTime.Today.Year.ToString();
                    }
                    else
                    {
                        var anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil6111"));
                        cmbTarihBaslangicYil6111.Text = anahtar == null ? "2011" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy6111"));
                        cmbTarihBaslangicAy6111.SelectedIndex = anahtar == null ? 2 : Convert.ToInt32(anahtar.Deger) - 1;

                        cmbTarihBitisAy6111.Text = oncekiAy.ToString("MMMM");
                        cmbTarihBitisYil6111.Text = oncekiAy.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil687"));
                        cmbTarihBaslangicYil687.Text = anahtar == null ? "2017" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy687"));
                        cmbTarihBaslangicAy687.SelectedIndex = anahtar == null ? 1 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy687.SelectedIndex = 11;
                        cmbTarihBitisYil687.Text = "2017";

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil6645"));
                        cmbTarihBaslangicYil6645.Text = anahtar == null ? "2015" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy6645"));
                        cmbTarihBaslangicAy6645.SelectedIndex = anahtar == null ? 3 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy6645.Text = DateTime.Today.ToString("MMMM");
                        cmbTarihBitisYil6645.Text = DateTime.Today.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil7103"));
                        cmbTarihBaslangicYil7103.Text = anahtar == null ? "2018" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy7103"));
                        cmbTarihBaslangicAy7103.SelectedIndex = anahtar == null ? 0 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy7103.Text = oncekiAy.ToString("MMMM");
                        cmbTarihBitisYil7103.Text = oncekiAy.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil2828"));
                        cmbTarihBaslangicYil2828.Text = anahtar == null ? "2014" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy2828"));
                        cmbTarihBaslangicAy2828.SelectedIndex = anahtar == null ? 1 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy2828.Text = oncekiAy.ToString("MMMM");
                        cmbTarihBitisYil2828.Text = oncekiAy.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil7252"));
                        cmbTarihBaslangicYil7252.Text = anahtar == null ? "2017" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy7252"));
                        cmbTarihBaslangicAy7252.SelectedIndex = anahtar == null ? 1 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy7252.Text = oncekiAy.ToString("MMMM");
                        cmbTarihBitisYil7252.Text = oncekiAy.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil7256"));
                        cmbTarihBaslangicYil7256.Text = anahtar == null ? "2020" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy7256"));
                        cmbTarihBaslangicAy7256.SelectedIndex = anahtar == null ? 11 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy7256.Text = oncekiAy.ToString("MMMM");
                        cmbTarihBitisYil7256.Text = oncekiAy.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil7316"));
                        cmbTarihBaslangicYil7316.Text = anahtar == null ? "2020" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy7316"));
                        cmbTarihBaslangicAy7316.SelectedIndex = anahtar == null ? 11 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy7316.Text = oncekiAy.ToString("MMMM");
                        cmbTarihBitisYil7316.Text = oncekiAy.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil3294"));
                        cmbTarihBaslangicYil3294.Text = anahtar == null ? "2020" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy3294"));
                        cmbTarihBaslangicAy3294.SelectedIndex = anahtar == null ? 11 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy3294.Text = oncekiAy.ToString("MMMM");
                        cmbTarihBitisYil3294.Text = oncekiAy.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYilTum"));
                        cmbTarihBaslangicYilTum.Text = anahtar == null ? "2011" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAyTum"));
                        cmbTarihBaslangicAyTum.SelectedIndex = anahtar == null ? 2 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAyTum.Text = oncekiAy.ToString("MMMM");
                        cmbTarihBitisYilTum.Text = oncekiAy.Year.ToString();

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil14857"));
                        cmbTarihBaslangicYil14857.Text = anahtar == null ? "2013" : anahtar.Deger;

                        anahtar = ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy14857"));
                        cmbTarihBaslangicAy14857.SelectedIndex = anahtar == null ? 8 : anahtar.Deger.ToInt() - 1;

                        cmbTarihBitisAy14857.Text = DateTime.Today.ToString("MMMM");
                        cmbTarihBitisYil14857.Text = DateTime.Today.Year.ToString();
                    }
                }
                else
                {
                    cmbTarihBaslangicYilTum.Text = secenekler.BaslangicTum.Year.ToString();
                    cmbTarihBaslangicAyTum.SelectedIndex = secenekler.BaslangicTum.Month - 1;

                    cmbTarihBitisYilTum.Text = secenekler.BitisTum.Year.ToString();
                    cmbTarihBitisAyTum.SelectedIndex = secenekler.BitisTum.Month - 1;

                    cmbTarihBaslangicYil6111.Text = secenekler.Baslangic6111.Year.ToString();
                    cmbTarihBaslangicAy6111.SelectedIndex = secenekler.Baslangic6111.Month - 1;

                    cmbTarihBitisYil6111.Text = secenekler.Bitis6111.Year.ToString();
                    cmbTarihBitisAy6111.SelectedIndex = secenekler.Bitis6111.Month - 1;

                    cmbTarihBaslangicYil7103.Text = secenekler.Baslangic7103.Year.ToString();
                    cmbTarihBaslangicAy7103.SelectedIndex = secenekler.Baslangic7103.Month - 1;

                    cmbTarihBitisYil7103.Text = secenekler.Bitis7103.Year.ToString();
                    cmbTarihBitisAy7103.SelectedIndex = secenekler.Bitis7103.Month - 1;

                    cmbTarihBaslangicYil2828.Text = secenekler.Baslangic2828.Year.ToString();
                    cmbTarihBaslangicAy2828.SelectedIndex = secenekler.Baslangic2828.Month - 1;

                    cmbTarihBitisYil2828.Text = secenekler.Bitis2828.Year.ToString();
                    cmbTarihBitisAy2828.SelectedIndex = secenekler.Bitis2828.Month - 1;

                    cmbTarihBaslangicYil7252.Text = secenekler.Baslangic7252.Year.ToString();
                    cmbTarihBaslangicAy7252.SelectedIndex = secenekler.Baslangic7252.Month - 1;

                    cmbTarihBitisYil7252.Text = secenekler.Bitis7252.Year.ToString();
                    cmbTarihBitisAy7252.SelectedIndex = secenekler.Bitis7252.Month - 1;

                    cmbTarihBaslangicYil7256.Text = secenekler.Baslangic7256.Year.ToString();
                    cmbTarihBaslangicAy7256.SelectedIndex = secenekler.Baslangic7256.Month - 1;

                    cmbTarihBitisYil7256.Text = secenekler.Bitis7256.Year.ToString();
                    cmbTarihBitisAy7256.SelectedIndex = secenekler.Bitis7256.Month - 1;

                    cmbTarihBaslangicYil7316.Text = secenekler.Baslangic7316.Year.ToString();
                    cmbTarihBaslangicAy7316.SelectedIndex = secenekler.Baslangic7316.Month - 1;

                    cmbTarihBitisYil7316.Text = secenekler.Bitis7316.Year.ToString();
                    cmbTarihBitisAy7316.SelectedIndex = secenekler.Bitis7316.Month - 1;

                    cmbTarihBaslangicYil3294.Text = secenekler.Baslangic3294.Year.ToString();
                    cmbTarihBaslangicAy3294.SelectedIndex = secenekler.Baslangic3294.Month - 1;

                    cmbTarihBitisYil3294.Text = secenekler.Bitis3294.Year.ToString();
                    cmbTarihBitisAy3294.SelectedIndex = secenekler.Bitis3294.Month - 1;

                    cmbTarihBaslangicYil6645.Text = secenekler.Baslangic6645.Year.ToString();
                    cmbTarihBaslangicAy6645.SelectedIndex = secenekler.Baslangic6645.Month - 1;

                    cmbTarihBitisYil6645.Text = secenekler.Bitis6645.Year.ToString();
                    cmbTarihBitisAy6645.SelectedIndex = secenekler.Bitis6645.Month - 1;

                    cmbTarihBaslangicYil687.Text = secenekler.Baslangic687.Year.ToString();
                    cmbTarihBaslangicAy687.SelectedIndex = secenekler.Baslangic687.Month - 1;

                    cmbTarihBitisYil687.Text = secenekler.Bitis687.Year.ToString();
                    cmbTarihBitisAy687.SelectedIndex = secenekler.Bitis687.Month - 1;


                    cmbTarihBaslangicYil14857.Text = secenekler.Baslangic14857.Year.ToString();
                    cmbTarihBaslangicAy14857.SelectedIndex = secenekler.Baslangic14857.Month - 1;

                    if (CariTanimla)
                    {
                        cmbTarihBaslangicYil14857.Text = oncekiAy.AddMonths(-1).Year.ToString();
                        cmbTarihBaslangicAy14857.SelectedIndex = oncekiAy.AddMonths(-1).Month - 1;
                    }


                    cmbTarihBitisYil14857.Text = secenekler.Bitis14857.Year.ToString();
                    cmbTarihBitisAy14857.SelectedIndex = secenekler.Bitis14857.Month - 1;
                }
            }

            if (!VeritabanindanCek)
            {
                lnklblDevam.Text = "Kaydet";
            }
        }

        private void chk687_CheckedChanged(object sender, EventArgs e)
        {

            cmbTarihBaslangicYil687.Enabled = chk687.Checked && chkBasvuruFormuIndir.Checked;
            cmbTarihBaslangicAy687.Enabled = chk687.Checked && chkBasvuruFormuIndir.Checked;

            cmbTarihBitisYil687.Enabled = chk687.Checked && chkBasvuruFormuIndir.Checked;
            cmbTarihBitisAy687.Enabled = chk687.Checked && chkBasvuruFormuIndir.Checked;
        }

        private void chk6111_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil6111.Enabled = chk6111.Checked && chkBasvuruFormuIndir.Checked && !chk6111EnBastan.Checked;
            cmbTarihBaslangicAy6111.Enabled = chk6111.Checked && chkBasvuruFormuIndir.Checked && !chk6111EnBastan.Checked;

            cmbTarihBitisYil6111.Enabled = chk6111.Checked && chkBasvuruFormuIndir.Checked;
            cmbTarihBitisAy6111.Enabled = chk6111.Checked && chkBasvuruFormuIndir.Checked;

            chk6111EnBastan.Enabled = chk6111.Checked && chkBasvuruFormuIndir.Checked && chk6111.Enabled;
        }

        private void chk6645_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil6645.Enabled = chk6645.Checked && chkBasvuruFormuIndir.Checked;
            cmbTarihBaslangicAy6645.Enabled = chk6645.Checked && chkBasvuruFormuIndir.Checked;

            cmbTarihBitisYil6645.Enabled = chk6645.Checked && chkBasvuruFormuIndir.Checked;
            cmbTarihBitisAy6645.Enabled = chk6645.Checked && chkBasvuruFormuIndir.Checked;

        }

        private void chk7103_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil7103.Enabled = chk7103.Checked && chkBasvuruFormuIndir.Checked && !chk7103EnBastan.Checked;
            cmbTarihBaslangicAy7103.Enabled = chk7103.Checked && chkBasvuruFormuIndir.Checked && !chk7103EnBastan.Checked;

            cmbTarihBitisYil7103.Enabled = chk7103.Checked && chkBasvuruFormuIndir.Checked;
            cmbTarihBitisAy7103.Enabled = chk7103.Checked && chkBasvuruFormuIndir.Checked;

            chk7103EnBastan.Enabled = chk7103.Checked && chkBasvuruFormuIndir.Checked && chk7103.Enabled;
        }

        private void chk2828_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil2828.Enabled = chk2828.Checked && chkBasvuruFormuIndir.Checked && !chk2828EnBastan.Checked;
            cmbTarihBaslangicAy2828.Enabled = chk2828.Checked && chkBasvuruFormuIndir.Checked && !chk2828EnBastan.Checked;

            cmbTarihBitisYil2828.Enabled = chk2828.Checked && chkBasvuruFormuIndir.Checked;
            cmbTarihBitisAy2828.Enabled = chk2828.Checked && chkBasvuruFormuIndir.Checked;

            chk2828EnBastan.Enabled = chk2828.Checked && chkBasvuruFormuIndir.Checked && chk2828.Enabled;
        }

        private void chk7252_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil7252.Enabled = chk7252.Checked && chkBasvuruFormuIndir.Checked && !chk7252EnBastan.Checked;
            cmbTarihBaslangicAy7252.Enabled = chk7252.Checked && chkBasvuruFormuIndir.Checked && !chk7252EnBastan.Checked;

            cmbTarihBitisYil7252.Enabled = chk7252.Checked && chkBasvuruFormuIndir.Checked;
            cmbTarihBitisAy7252.Enabled = chk7252.Checked && chkBasvuruFormuIndir.Checked;

            chk7252EnBastan.Enabled = chk7252.Checked && chkBasvuruFormuIndir.Checked && chk7252.Enabled;
        }

        private void chk7256_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil7256.Enabled = chk7256.Checked && chkBasvuruFormuIndir.Checked && !chk7256EnBastan.Checked;
            cmbTarihBaslangicAy7256.Enabled = chk7256.Checked && chkBasvuruFormuIndir.Checked && !chk7256EnBastan.Checked;

            cmbTarihBitisYil7256.Enabled = chk7256.Checked && chkBasvuruFormuIndir.Checked;
            cmbTarihBitisAy7256.Enabled = chk7256.Checked && chkBasvuruFormuIndir.Checked;

            chk7256EnBastan.Enabled = chk7256.Checked && chkBasvuruFormuIndir.Checked && chk7256.Enabled;
        }

        private void chk7316_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil7316.Enabled = chk7316.Checked && chkBasvuruFormuIndir.Checked && !chk7316EnBastan.Checked;
            cmbTarihBaslangicAy7316.Enabled = chk7316.Checked && chkBasvuruFormuIndir.Checked && !chk7316EnBastan.Checked;

            cmbTarihBitisYil7316.Enabled = chk7316.Checked && chkBasvuruFormuIndir.Checked;
            cmbTarihBitisAy7316.Enabled = chk7316.Checked && chkBasvuruFormuIndir.Checked;

            chk7316EnBastan.Enabled = chk7316.Checked && chkBasvuruFormuIndir.Checked && chk7316.Enabled;
        }

        private void chk3294_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil3294.Enabled = chk3294.Checked && chkBasvuruFormuIndir.Checked && !chk3294EnBastan.Checked;
            cmbTarihBaslangicAy3294.Enabled = chk3294.Checked && chkBasvuruFormuIndir.Checked && !chk3294EnBastan.Checked;

            cmbTarihBitisYil3294.Enabled = chk3294.Checked && chkBasvuruFormuIndir.Checked;
            cmbTarihBitisAy3294.Enabled = chk3294.Checked && chkBasvuruFormuIndir.Checked;

            chk3294EnBastan.Enabled = chk3294.Checked && chkBasvuruFormuIndir.Checked && chk3294.Enabled;
        }

        private void chkTumTesvikler_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYilTum.Enabled = chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked && ! chkTumuEnBastan.Checked;
            cmbTarihBaslangicAyTum.Enabled = chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked && !chkTumuEnBastan.Checked;

            cmbTarihBitisYilTum.Enabled = chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;
            cmbTarihBitisAyTum.Enabled = chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;

            chkTumuEnBastan.Enabled = chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked && chkTumTesvikler.Enabled;


            //chk6111.Checked = ! chkTumTesvikler.Checked;
            chk6111.Enabled = !chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;
            chk6111EnBastan.Enabled = chk6111.Enabled && chk6111.Checked;

            //chk7103.Checked = !chkTumTesvikler.Checked;
            chk7103.Enabled = !chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;
            chk7103EnBastan.Enabled = chk7103.Enabled && chk7103.Checked;

            //chk2828.Checked = !chkTumTesvikler.Checked;
            chk2828.Enabled = !chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;
            chk2828EnBastan.Enabled = chk2828.Enabled && chk2828.Checked;

            chk7252.Enabled = !chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;
            chk7252EnBastan.Enabled = chk7252.Enabled && chk7252.Checked;

            chk7256.Enabled = !chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;
            chk7256EnBastan.Enabled = chk7256.Enabled && chk7256.Checked;

            chk7316.Enabled = !chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;
            chk7316EnBastan.Enabled = chk7316.Enabled && chk7316.Checked;

            chk3294.Enabled = !chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;
            chk3294EnBastan.Enabled = chk3294.Enabled && chk3294.Checked;
        }

        private void chk14857_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil14857.Enabled = chk14857.Checked && chkBasvuruFormuIndir.Checked;
            cmbTarihBaslangicAy14857.Enabled = chk14857.Checked && chkBasvuruFormuIndir.Checked;

            cmbTarihBitisYil14857.Enabled = chk14857.Checked && chkBasvuruFormuIndir.Checked;
            cmbTarihBitisAy14857.Enabled = chk14857.Checked && chkBasvuruFormuIndir.Checked;
        }

        private void chkAphbIndir_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control c in grpbxAphbIndir.Controls)
            {
                if (c is ComboBox) ((ComboBox)c).Enabled = chkAphbIndir.Checked;
            }
        }

        private void chkBasvuruFormuIndir_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control c in grpbxBasvuruFormu.Controls)
            {
                if (c is ComboBox) ((ComboBox)c).Enabled = chkBasvuruFormuIndir.Checked;
                else if (c is CheckBox && !((CheckBox)c).Name.EndsWith("EnBastan")) ((CheckBox)c).Enabled = chkBasvuruFormuIndir.Checked;
            }

            chkTumTesvikler_CheckedChanged(null, null);
        }

        private void chkEnBastan_Click(object sender, EventArgs e)
        {
            if (sender == chkTumuEnBastan)
            {
                cmbTarihBaslangicAyTum.Enabled = ! cmbTarihBaslangicAyTum.Enabled;
                cmbTarihBaslangicYilTum.Enabled = ! cmbTarihBaslangicYilTum.Enabled;
            }
            else if (sender == chk6111EnBastan)
            {
                cmbTarihBaslangicAy6111.Enabled = ! cmbTarihBaslangicAy6111.Enabled;
                cmbTarihBaslangicYil6111.Enabled = ! cmbTarihBaslangicYil6111.Enabled;
            }
            else if (sender == chk7103EnBastan)
            {
                cmbTarihBaslangicAy7103.Enabled = ! cmbTarihBaslangicAy7103.Enabled;
                cmbTarihBaslangicYil7103.Enabled = ! cmbTarihBaslangicYil7103.Enabled;
            }
            else if (sender == chk2828EnBastan)
            {
                cmbTarihBaslangicAy2828.Enabled = ! cmbTarihBaslangicAy2828.Enabled;
                cmbTarihBaslangicYil2828.Enabled = !cmbTarihBaslangicYil2828.Enabled;
            }
            else if (sender == chk7252EnBastan)
            {
                cmbTarihBaslangicAy7252.Enabled = ! cmbTarihBaslangicAy7252.Enabled;
                cmbTarihBaslangicYil7252.Enabled = !cmbTarihBaslangicYil7252.Enabled;
            }
            else if (sender == chk7256EnBastan)
            {
                cmbTarihBaslangicAy7256.Enabled = !cmbTarihBaslangicAy7256.Enabled;
                cmbTarihBaslangicYil7256.Enabled = !cmbTarihBaslangicYil7256.Enabled;
            }
            else if (sender == chk7316EnBastan)
            {
                cmbTarihBaslangicAy7316.Enabled = !cmbTarihBaslangicAy7316.Enabled;
                cmbTarihBaslangicYil7316.Enabled = !cmbTarihBaslangicYil7316.Enabled;
            }
            else if (sender == chk3294EnBastan)
            {
                cmbTarihBaslangicAy3294.Enabled = !cmbTarihBaslangicAy3294.Enabled;
                cmbTarihBaslangicYil3294.Enabled = !cmbTarihBaslangicYil3294.Enabled;
            }
        }
    }
}
