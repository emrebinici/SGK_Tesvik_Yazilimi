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
    public partial class frmTarihSecAyarlar : Form
    {
        DialogResult dr = DialogResult.No;

        bool CariTanimla = true;
        public frmTarihSecAyarlar(bool cari)
        {
            CariTanimla = cari;
            InitializeComponent();

        }

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddMonths(1))
                yield return day;
        }

        private void frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = dr;
        }

        private void frm_Load(object sender, EventArgs e)
        {

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

                cmbTarihBaslangicYilAphb.Text = "2011";

                cmbTarihBaslangicAyAphb.SelectedIndex = 2;

                cmbTarihBitisAyAphb.Text = DateTime.Today.ToString("MMMM");

                cmbTarihBitisYilAphb.Text = DateTime.Today.Year.ToString();
            }

            {

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

                cmbTarihBaslangicYil6111.Text = "2011";
                cmbTarihBaslangicAy6111.SelectedIndex = 2;

                cmbTarihBitisAy6111.Text = oncekiAy.ToString("MMMM");
                cmbTarihBitisYil6111.Text = oncekiAy.Year.ToString();

                cmbTarihBaslangicYil687.Text = "2017";
                cmbTarihBaslangicAy687.SelectedIndex = 1;

                cmbTarihBitisAy687.SelectedIndex = 11;
                cmbTarihBitisYil687.Text = "2017";

                cmbTarihBaslangicYil6645.Text = "2015";
                cmbTarihBaslangicAy6645.SelectedIndex = 3;

                cmbTarihBitisAy6645.Text = DateTime.Today.ToString("MMMM");
                cmbTarihBitisYil6645.Text = DateTime.Today.Year.ToString();

                cmbTarihBaslangicYil7103.Text = "2018";
                cmbTarihBaslangicAy7103.SelectedIndex = 0;

                cmbTarihBitisAy7103.Text = oncekiAy.ToString("MMMM");
                cmbTarihBitisYil7103.Text = oncekiAy.Year.ToString();

                cmbTarihBaslangicYil2828.Text = "2014";
                cmbTarihBaslangicAy2828.SelectedIndex = 1;

                cmbTarihBitisAy2828.Text = oncekiAy.ToString("MMMM");
                cmbTarihBitisYil2828.Text = oncekiAy.Year.ToString();

                cmbTarihBaslangicYil7252.Text = "2017";
                cmbTarihBaslangicAy7252.SelectedIndex = 0;

                cmbTarihBitisAy7252.Text = oncekiAy.ToString("MMMM");
                cmbTarihBitisYil7252.Text = oncekiAy.Year.ToString();

                cmbTarihBaslangicYil7256.Text = "2020";
                cmbTarihBaslangicAy7256.SelectedIndex = 11;

                cmbTarihBitisAy7256.Text = DateTime.Today.ToString("MMMM");
                cmbTarihBitisYil7256.Text = DateTime.Today.Year.ToString();

                cmbTarihBaslangicYil7316.Text = "2020";
                cmbTarihBaslangicAy7316.SelectedIndex = 11;

                cmbTarihBitisAy7316.Text = DateTime.Today.ToString("MMMM");
                cmbTarihBitisYil7316.Text = DateTime.Today.Year.ToString();

                cmbTarihBaslangicYil3294.Text = "2020";
                cmbTarihBaslangicAy3294.SelectedIndex = 11;

                cmbTarihBitisAy3294.Text = DateTime.Today.ToString("MMMM");
                cmbTarihBitisYil3294.Text = DateTime.Today.Year.ToString();

                if (CariTanimla)
                {
                    var cariBaslangic= DateTime.Today.AddMonths(-30);

                    cmbTarihBaslangicYilTum.Text = cariBaslangic.Year.ToString();
                    cmbTarihBaslangicAyTum.SelectedIndex = cariBaslangic.Month - 1;
                }
                else
                {
                    cmbTarihBaslangicYilTum.Text = "2011";
                    cmbTarihBaslangicAyTum.SelectedIndex = 2;
                }

                cmbTarihBitisAyTum.Text = oncekiAy.ToString("MMMM");
                cmbTarihBitisYilTum.Text = oncekiAy.Year.ToString();

                cmbTarihBaslangicYil14857.Text = "2013";
                cmbTarihBaslangicAy14857.SelectedIndex = 8;

                cmbTarihBitisAy14857.Text = DateTime.Today.ToString("MMMM");
                cmbTarihBitisYil14857.Text = DateTime.Today.Year.ToString();
            }


            AyarlariDoldur();
        }

        private void AyarlariDoldur()
        {
            bool Kaydet = false;

            using (var dbContext = new DbEntities())
            {
                var anahtarlar = dbContext.Ayarlar.ToList();

                if (CariTanimla)
                {

                    var elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("AphbIndirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "AphbIndirilsin", Deger = "True" });

                        Kaydet = true;

                        chkAphbIndir.Checked = true;
                    }
                    else chkAphbIndir.Checked = elem.Deger.Equals("True");


                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("AphbBaslangicYil"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "AphbBaslangicYil", Deger = "2011" });

                        Kaydet = true;

                        cmbTarihBaslangicYilAphb.Text = "2011";
                    }
                    else cmbTarihBaslangicYilAphb.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("AphbBaslangicAy"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "AphbBaslangicAy", Deger = "3" });

                        Kaydet = true;

                        cmbTarihBaslangicAyAphb.SelectedIndex = 2;
                    }
                    else cmbTarihBaslangicAyAphb.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;


                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BasvuruFormuIndirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BasvuruFormuIndirilsin", Deger = "True" });

                        Kaydet = true;

                        chkBasvuruFormuIndir.Checked = true;
                    }
                    else chkBasvuruFormuIndir.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfTumIndirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfTumIndirilsin", Deger = "True" });

                        Kaydet = true;

                        chkTumTesvikler.Checked = true;
                    }
                    else chkTumTesvikler.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYilTum"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicYilTum", Deger = "2017" });

                        Kaydet = true;

                        cmbTarihBaslangicYilTum.Text = "2017";
                    }
                    else cmbTarihBaslangicYilTum.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAyTum"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicAyTum", Deger = "1" });

                        Kaydet = true;

                        cmbTarihBaslangicAyTum.SelectedIndex = 0;
                    }
                    else cmbTarihBaslangicAyTum.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf6111Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "Bf6111Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk6111.Checked = false;
                    }
                    else chk6111.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil6111"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicYil6111", Deger = "2011" });

                        Kaydet = true;

                        cmbTarihBaslangicYil6111.Text = "2011";
                    }
                    else cmbTarihBaslangicYil6111.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy6111"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicAy6111", Deger = "3" });

                        Kaydet = true;

                        cmbTarihBaslangicAy6111.SelectedIndex = 2;
                    }
                    else cmbTarihBaslangicAy6111.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;


                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf7103Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "Bf7103Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk7103.Checked = false;
                    }
                    else chk7103.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil7103"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicYil7103", Deger = "2018" });

                        Kaydet = true;

                        cmbTarihBaslangicYil7103.Text = "2018";
                    }
                    else cmbTarihBaslangicYil7103.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy7103"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicAy7103", Deger = "1" });

                        Kaydet = true;

                        cmbTarihBaslangicAy7103.SelectedIndex = 0;
                    }
                    else cmbTarihBaslangicAy7103.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;




                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf2828Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "Bf2828Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk2828.Checked = false;
                    }
                    else chk2828.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil2828"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicYil2828", Deger = "2014" });

                        Kaydet = true;

                        cmbTarihBaslangicYil2828.Text = "2014";
                    }
                    else cmbTarihBaslangicYil2828.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy2828"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicAy2828", Deger = "2" });

                        Kaydet = true;

                        cmbTarihBaslangicAy2828.SelectedIndex = 1;
                    }
                    else cmbTarihBaslangicAy2828.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;



                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf7252Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "Bf7252Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk7252.Checked = false;
                    }
                    else chk7252.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil7252"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicYil7252", Deger = "2017" });

                        Kaydet = true;

                        cmbTarihBaslangicYil7252.Text = "2017";
                    }
                    else cmbTarihBaslangicYil7252.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy7252"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicAy7252", Deger = "1" });

                        Kaydet = true;

                        cmbTarihBaslangicAy7252.SelectedIndex = 0;
                    }
                    else cmbTarihBaslangicAy7252.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;


                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf7256Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "Bf7256Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk7256.Checked = false;
                    }
                    else chk7256.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil7256"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicYil7256", Deger = "2020" });

                        Kaydet = true;

                        cmbTarihBaslangicYil7256.Text = "2020";
                    }
                    else cmbTarihBaslangicYil7256.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy7256"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicAy7256", Deger = "12" });

                        Kaydet = true;

                        cmbTarihBaslangicAy7256.SelectedIndex = 0;
                    }
                    else cmbTarihBaslangicAy7256.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf7316Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "Bf7316Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk7316.Checked = false;
                    }
                    else chk7316.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil7316"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicYil7316", Deger = "2020" });

                        Kaydet = true;

                        cmbTarihBaslangicYil7316.Text = "2020";
                    }
                    else cmbTarihBaslangicYil7316.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy7316"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicAy7316", Deger = "12" });

                        Kaydet = true;

                        cmbTarihBaslangicAy7316.SelectedIndex = 0;
                    }
                    else cmbTarihBaslangicAy7316.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;


                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf3294Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "Bf3294Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk3294.Checked = false;
                    }
                    else chk3294.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil3294"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicYil3294", Deger = "2020" });

                        Kaydet = true;

                        cmbTarihBaslangicYil3294.Text = "2020";
                    }
                    else cmbTarihBaslangicYil3294.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy3294"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicAy3294", Deger = "12" });

                        Kaydet = true;

                        cmbTarihBaslangicAy3294.SelectedIndex = 0;
                    }
                    else cmbTarihBaslangicAy3294.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf6645Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "Bf6645Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk6645.Checked = false;
                    }
                    else chk6645.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil6645"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicYil6645", Deger = "2015" });

                        Kaydet = true;

                        cmbTarihBaslangicYil6645.Text = "2015";
                    }
                    else cmbTarihBaslangicYil6645.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy6645"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicAy6645", Deger = "4" });

                        Kaydet = true;

                        cmbTarihBaslangicAy6645.SelectedIndex = 3;
                    }
                    else cmbTarihBaslangicAy6645.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;



                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf687Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "Bf687Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk687.Checked = false;
                    }
                    else chk687.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil687"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicYil687", Deger = "2017" });

                        Kaydet = true;

                        cmbTarihBaslangicYil687.Text = "2017";
                    }
                    else cmbTarihBaslangicYil687.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy687"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicAy687", Deger = "2" });

                        Kaydet = true;

                        cmbTarihBaslangicAy687.SelectedIndex = 1;
                    }
                    else cmbTarihBaslangicAy687.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;


                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf14857Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "Bf14857Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk14857.Checked = false;
                    }
                    else chk14857.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil14857"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicYil14857", Deger = "2013" });

                        Kaydet = true;

                        cmbTarihBaslangicYil14857.Text = "2013";
                    }
                    else cmbTarihBaslangicYil14857.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy14857"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "BfBaslangicAy14857", Deger = "9" });

                        Kaydet = true;

                        cmbTarihBaslangicAy14857.SelectedIndex = 8;
                    }
                    else cmbTarihBaslangicAy14857.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;

                }
                else
                {
                    var elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisAphbIndirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisAphbIndirilsin", Deger = "True" });

                        Kaydet = true;

                        chkAphbIndir.Checked = true;
                    }
                    else chkAphbIndir.Checked = elem.Deger.Equals("True");


                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisAphbBaslangicYil"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisAphbBaslangicYil", Deger = "2011" });

                        Kaydet = true;

                        cmbTarihBaslangicYilAphb.Text = "2011";
                    }
                    else cmbTarihBaslangicYilAphb.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisAphbBaslangicAy"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisAphbBaslangicAy", Deger = "3" });

                        Kaydet = true;

                        cmbTarihBaslangicAyAphb.SelectedIndex = 2;
                    }
                    else cmbTarihBaslangicAyAphb.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;


                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBasvuruFormuIndirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBasvuruFormuIndirilsin", Deger = "True" });

                        Kaydet = true;

                        chkBasvuruFormuIndir.Checked = true;
                    }
                    else chkBasvuruFormuIndir.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfTumIndirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfTumIndirilsin", Deger = "True" });

                        Kaydet = true;

                        chkTumTesvikler.Checked = true;
                    }
                    else chkTumTesvikler.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYilTum"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicYilTum", Deger = "2011" });

                        Kaydet = true;

                        cmbTarihBaslangicYilTum.Text = "2011";
                    }
                    else cmbTarihBaslangicYilTum.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAyTum"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicAyTum", Deger = "3" });

                        Kaydet = true;

                        cmbTarihBaslangicAyTum.SelectedIndex = 2;
                    }
                    else cmbTarihBaslangicAyTum.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf6111Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBf6111Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk6111.Checked = false;
                    }
                    else chk6111.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil6111"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicYil6111", Deger = "2011" });

                        Kaydet = true;

                        cmbTarihBaslangicYil6111.Text = "2011";
                    }
                    else cmbTarihBaslangicYil6111.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy6111"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicAy6111", Deger = "3" });

                        Kaydet = true;

                        cmbTarihBaslangicAy6111.SelectedIndex = 2;
                    }
                    else cmbTarihBaslangicAy6111.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;


                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf7103Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBf7103Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk7103.Checked = false;
                    }
                    else chk7103.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil7103"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicYil7103", Deger = "2018" });

                        Kaydet = true;

                        cmbTarihBaslangicYil7103.Text = "2018";
                    }
                    else cmbTarihBaslangicYil7103.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy7103"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicAy7103", Deger = "1" });

                        Kaydet = true;

                        cmbTarihBaslangicAy7103.SelectedIndex = 0;
                    }
                    else cmbTarihBaslangicAy7103.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;




                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf2828Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBf2828Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk2828.Checked = false;
                    }
                    else chk2828.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil2828"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicYil2828", Deger = "2014" });

                        Kaydet = true;

                        cmbTarihBaslangicYil2828.Text = "2014";
                    }
                    else cmbTarihBaslangicYil2828.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy2828"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicAy2828", Deger = "2" });

                        Kaydet = true;

                        cmbTarihBaslangicAy2828.SelectedIndex = 1;
                    }
                    else cmbTarihBaslangicAy2828.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;



                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf7252Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBf7252Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk7252.Checked = false;
                    }
                    else chk7252.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil7252"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicYil7252", Deger = "2017" });

                        Kaydet = true;

                        cmbTarihBaslangicYil7252.Text = "2017";
                    }
                    else cmbTarihBaslangicYil7252.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy7252"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicAy7252", Deger = "1" });

                        Kaydet = true;

                        cmbTarihBaslangicAy7252.SelectedIndex = 0;
                    }
                    else cmbTarihBaslangicAy7252.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;


                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf7256Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBf7256Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk7256.Checked = false;
                    }
                    else chk7256.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil7256"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicYil7256", Deger = "2020" });

                        Kaydet = true;

                        cmbTarihBaslangicYil7256.Text = "2020";
                    }
                    else cmbTarihBaslangicYil7256.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy7256"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicAy7256", Deger = "12" });

                        Kaydet = true;

                        cmbTarihBaslangicAy7256.SelectedIndex = 0;
                    }
                    else cmbTarihBaslangicAy7256.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;


                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf7316Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBf7316Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk7316.Checked = false;
                    }
                    else chk7316.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil7316"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicYil7316", Deger = "2020" });

                        Kaydet = true;

                        cmbTarihBaslangicYil7316.Text = "2020";
                    }
                    else cmbTarihBaslangicYil7316.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy7316"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicAy7316", Deger = "12" });

                        Kaydet = true;

                        cmbTarihBaslangicAy7316.SelectedIndex = 0;
                    }
                    else cmbTarihBaslangicAy7316.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;


                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf3294Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBf3294Indirilsin", Deger = "False" });

                        Kaydet = true;

                        chk3294.Checked = false;
                    }
                    else chk3294.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil3294"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicYil3294", Deger = "2020" });

                        Kaydet = true;

                        cmbTarihBaslangicYil3294.Text = "2020";
                    }
                    else cmbTarihBaslangicYil3294.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy3294"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicAy3294", Deger = "12" });

                        Kaydet = true;

                        cmbTarihBaslangicAy3294.SelectedIndex = 0;
                    }
                    else cmbTarihBaslangicAy3294.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;


                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf6645Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBf6645Indirilsin", Deger = "True" });

                        Kaydet = true;

                        chk6645.Checked = false;
                    }
                    else chk6645.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil6645"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicYil6645", Deger = "2015" });

                        Kaydet = true;

                        cmbTarihBaslangicYil6645.Text = "2015";
                    }
                    else cmbTarihBaslangicYil6645.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy6645"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicAy6645", Deger = "4" });

                        Kaydet = true;

                        cmbTarihBaslangicAy6645.SelectedIndex = 3;
                    }
                    else cmbTarihBaslangicAy6645.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;



                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf687Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBf687Indirilsin", Deger = "True" });

                        Kaydet = true;

                        chk687.Checked = false;
                    }
                    else chk687.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil687"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicYil687", Deger = "2017" });

                        Kaydet = true;

                        cmbTarihBaslangicYil687.Text = "2017";
                    }
                    else cmbTarihBaslangicYil687.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy687"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicAy687", Deger = "2" });

                        Kaydet = true;

                        cmbTarihBaslangicAy687.SelectedIndex = 1;
                    }
                    else cmbTarihBaslangicAy687.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;


                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf14857Indirilsin"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBf14857Indirilsin", Deger = "True" });

                        Kaydet = true;

                        chk14857.Checked = false;
                    }
                    else chk14857.Checked = elem.Deger.Equals("True");

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil14857"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicYil14857", Deger = "2013" });

                        Kaydet = true;

                        cmbTarihBaslangicYil14857.Text = "2013";
                    }
                    else cmbTarihBaslangicYil14857.Text = elem.Deger;

                    elem = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy14857"));

                    if (elem == null)
                    {
                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "GecmisBfBaslangicAy14857", Deger = "9" });

                        Kaydet = true;

                        cmbTarihBaslangicAy14857.SelectedIndex = 8;
                    }
                    else cmbTarihBaslangicAy14857.SelectedIndex = Convert.ToInt32(elem.Deger) - 1;

                }

                if (Kaydet)
                {
                    dbContext.SaveChanges();
                }
            }

        }

        private void chk687_CheckedChanged(object sender, EventArgs e)
        {

            cmbTarihBaslangicYil687.Enabled = chk687.Checked;
            cmbTarihBaslangicAy687.Enabled = chk687.Checked;

            cmbTarihBitisYil687.Enabled = chk687.Checked;
            cmbTarihBitisAy687.Enabled = chk687.Checked;
        }

        private void chk6111_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil6111.Enabled = chk6111.Checked;
            cmbTarihBaslangicAy6111.Enabled = chk6111.Checked;

            cmbTarihBitisYil6111.Enabled = chk6111.Checked;
            cmbTarihBitisAy6111.Enabled = chk6111.Checked;
        }

        private void chk6645_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil6645.Enabled = chk6645.Checked;
            cmbTarihBaslangicAy6645.Enabled = chk6645.Checked;

            cmbTarihBitisYil6645.Enabled = chk6645.Checked;
            cmbTarihBitisAy6645.Enabled = chk6645.Checked;

        }

        private void chk7103_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil7103.Enabled = chk7103.Checked;
            cmbTarihBaslangicAy7103.Enabled = chk7103.Checked;

            cmbTarihBitisYil7103.Enabled = chk7103.Checked;
            cmbTarihBitisAy7103.Enabled = chk7103.Checked;
        }

        private void chk2828_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil2828.Enabled = chk2828.Checked;
            cmbTarihBaslangicAy2828.Enabled = chk2828.Checked;

            cmbTarihBitisYil2828.Enabled = chk2828.Checked;
            cmbTarihBitisAy2828.Enabled = chk2828.Checked;
        }

        private void chk7252_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil7252.Enabled = chk7252.Checked;
            cmbTarihBaslangicAy7252.Enabled = chk7252.Checked;

            cmbTarihBitisYil7252.Enabled = chk7252.Checked;
            cmbTarihBitisAy7252.Enabled = chk7252.Checked;
        }

        private void chk7256_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil7256.Enabled = chk7256.Checked;
            cmbTarihBaslangicAy7256.Enabled = chk7256.Checked;

            cmbTarihBitisYil7256.Enabled = chk7256.Checked;
            cmbTarihBitisAy7256.Enabled = chk7256.Checked;
        }

        private void chk7316_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil7316.Enabled = chk7316.Checked;
            cmbTarihBaslangicAy7316.Enabled = chk7316.Checked;

            cmbTarihBitisYil7316.Enabled = chk7316.Checked;
            cmbTarihBitisAy7316.Enabled = chk7316.Checked;
        }

        private void chk3294_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil3294.Enabled = chk3294.Checked;
            cmbTarihBaslangicAy3294.Enabled = chk3294.Checked;

            cmbTarihBitisYil3294.Enabled = chk3294.Checked;
            cmbTarihBitisAy3294.Enabled = chk3294.Checked;
        }

        private void chkTumTesvikler_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYilTum.Enabled = chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;
            cmbTarihBaslangicAyTum.Enabled = chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;

            cmbTarihBitisYilTum.Enabled = chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;
            cmbTarihBitisAyTum.Enabled = chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;

            chk6111.Checked = !chkTumTesvikler.Checked;
            chk6111.Enabled = !chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;

            chk7103.Checked = !chkTumTesvikler.Checked;
            chk7103.Enabled = !chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;

            chk2828.Checked = !chkTumTesvikler.Checked;
            chk2828.Enabled = !chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;

            chk7252.Checked = !chkTumTesvikler.Checked;
            chk7252.Enabled = !chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;

            chk7256.Checked = !chkTumTesvikler.Checked;
            chk7256.Enabled = !chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;

            chk7316.Checked = !chkTumTesvikler.Checked;
            chk7316.Enabled = !chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;

            chk3294.Checked = !chkTumTesvikler.Checked;
            chk3294.Enabled = !chkTumTesvikler.Checked && chkBasvuruFormuIndir.Checked;
        }

        private void chk14857_CheckedChanged(object sender, EventArgs e)
        {
            cmbTarihBaslangicYil14857.Enabled = chk14857.Checked;
            cmbTarihBaslangicAy14857.Enabled = chk14857.Checked;

            cmbTarihBitisYil14857.Enabled = chk14857.Checked;
            cmbTarihBitisAy14857.Enabled = chk14857.Checked;
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
                else if (c is CheckBox) ((CheckBox)c).Enabled = chkBasvuruFormuIndir.Checked;
            }

            chkTumTesvikler_CheckedChanged(null, null);
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            using (var dbContext = new DbEntities())
            {
                if (CariTanimla)
                {

                    var anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("AphbIndirilsin"));
                    anahtar.Deger = chkAphbIndir.Checked ? "True" : "False";

                    {

                        DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYilAphb.Text), Convert.ToInt32(cmbTarihBaslangicAyAphb.SelectedIndex + 1), 1);

                        DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYilAphb.Text), Convert.ToInt32(cmbTarihBitisAyAphb.SelectedIndex + 1), 1);

                        if (TarihBaslangic > TarihBitis)
                        {
                            MessageBox.Show("Aphb başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("AphbBaslangicYil"));

                            anahtar.Deger = cmbTarihBaslangicYilAphb.Text;

                            anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("AphbBaslangicAy"));

                            anahtar.Deger = (cmbTarihBaslangicAyAphb.SelectedIndex + 1).ToString();
                        }

                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BasvuruFormuIndirilsin"));
                    anahtar.Deger = chkBasvuruFormuIndir.Checked ? "True" : "False";

                    DateTime ayinIlkGunu = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfTumIndirilsin"));
                    anahtar.Deger = chkTumTesvikler.Checked ? "True" : "False";

                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYilTum.Text), Convert.ToInt32(cmbTarihBaslangicAyTum.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYilTum.Text), Convert.ToInt32(cmbTarihBitisAyTum.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("Tüm teşvikler başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYilTum"));

                        anahtar.Deger = cmbTarihBaslangicYilTum.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAyTum"));

                        anahtar.Deger = (cmbTarihBaslangicAyTum.SelectedIndex + 1).ToString();
                        //}

                    }


                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf6111Indirilsin"));
                    anahtar.Deger = chk6111.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil6111.Text), Convert.ToInt32(cmbTarihBaslangicAy6111.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil6111.Text), Convert.ToInt32(cmbTarihBitisAy6111.SelectedIndex + 1), 1);


                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;
                        //    MessageBox.Show("6111 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil6111"));

                        anahtar.Deger = cmbTarihBaslangicYil6111.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy6111"));

                        anahtar.Deger = (cmbTarihBaslangicAy6111.SelectedIndex + 1).ToString();
                        //}

                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf6645Indirilsin"));
                    anahtar.Deger = chk6645.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil6645.Text), Convert.ToInt32(cmbTarihBaslangicAy6645.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil6645.Text), Convert.ToInt32(cmbTarihBitisAy6645.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("6645 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil6645"));

                        anahtar.Deger = cmbTarihBaslangicYil6645.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy6645"));

                        anahtar.Deger = (cmbTarihBaslangicAy6645.SelectedIndex + 1).ToString();

                        //}
                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf7103Indirilsin"));
                    anahtar.Deger = chk7103.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil7103.Text), Convert.ToInt32(cmbTarihBaslangicAy7103.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil7103.Text), Convert.ToInt32(cmbTarihBitisAy7103.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("7103 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil7103"));

                        anahtar.Deger = cmbTarihBaslangicYil7103.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy7103"));

                        anahtar.Deger = (cmbTarihBaslangicAy7103.SelectedIndex + 1).ToString();
                        //}
                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf687Indirilsin"));
                    anahtar.Deger = chk687.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil687.Text), Convert.ToInt32(cmbTarihBaslangicAy687.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil687.Text), Convert.ToInt32(cmbTarihBitisAy687.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("687 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil687"));

                        anahtar.Deger = cmbTarihBaslangicYil687.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy687"));

                        anahtar.Deger = (cmbTarihBaslangicAy687.SelectedIndex + 1).ToString();
                        //}
                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf2828Indirilsin"));
                    anahtar.Deger = chk2828.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil2828.Text), Convert.ToInt32(cmbTarihBaslangicAy2828.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil2828.Text), Convert.ToInt32(cmbTarihBitisAy2828.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("2828 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil2828"));

                        anahtar.Deger = cmbTarihBaslangicYil2828.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy2828"));

                        anahtar.Deger = (cmbTarihBaslangicAy2828.SelectedIndex + 1).ToString();
                        //}
                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf7252Indirilsin"));
                    anahtar.Deger = chk7252.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil7252.Text), Convert.ToInt32(cmbTarihBaslangicAy7252.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil7252.Text), Convert.ToInt32(cmbTarihBitisAy7252.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("7252 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil7252"));

                        anahtar.Deger = cmbTarihBaslangicYil7252.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy7252"));

                        anahtar.Deger = (cmbTarihBaslangicAy7252.SelectedIndex + 1).ToString();
                        //}
                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf7256Indirilsin"));
                    anahtar.Deger = chk7256.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil7256.Text), Convert.ToInt32(cmbTarihBaslangicAy7256.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil7256.Text), Convert.ToInt32(cmbTarihBitisAy7256.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("7256 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil7256"));

                        anahtar.Deger = cmbTarihBaslangicYil7256.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy7256"));

                        anahtar.Deger = (cmbTarihBaslangicAy7256.SelectedIndex + 1).ToString();
                        //}
                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf7316Indirilsin"));
                    anahtar.Deger = chk7316.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil7316.Text), Convert.ToInt32(cmbTarihBaslangicAy7316.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil7316.Text), Convert.ToInt32(cmbTarihBitisAy7316.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("7316 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil7316"));

                        anahtar.Deger = cmbTarihBaslangicYil7316.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy7316"));

                        anahtar.Deger = (cmbTarihBaslangicAy7316.SelectedIndex + 1).ToString();
                        //}
                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf3294Indirilsin"));
                    anahtar.Deger = chk3294.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil3294.Text), Convert.ToInt32(cmbTarihBaslangicAy3294.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil3294.Text), Convert.ToInt32(cmbTarihBitisAy3294.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("3294 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil3294"));

                        anahtar.Deger = cmbTarihBaslangicYil3294.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy3294"));

                        anahtar.Deger = (cmbTarihBaslangicAy3294.SelectedIndex + 1).ToString();
                        //}
                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("Bf14857Indirilsin"));
                    anahtar.Deger = chk14857.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil14857.Text), Convert.ToInt32(cmbTarihBaslangicAy14857.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil14857.Text), Convert.ToInt32(cmbTarihBitisAy14857.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("14857 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicYil14857"));

                        anahtar.Deger = cmbTarihBaslangicYil14857.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("BfBaslangicAy14857"));

                        anahtar.Deger = (cmbTarihBaslangicAy14857.SelectedIndex + 1).ToString();
                        //}
                    }
                }
                else {
                    var anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisAphbIndirilsin"));
                    anahtar.Deger = chkAphbIndir.Checked ? "True" : "False";

                    {

                        DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYilAphb.Text), Convert.ToInt32(cmbTarihBaslangicAyAphb.SelectedIndex + 1), 1);

                        DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYilAphb.Text), Convert.ToInt32(cmbTarihBitisAyAphb.SelectedIndex + 1), 1);

                        if (TarihBaslangic > TarihBitis)
                        {
                            MessageBox.Show("Aphb başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisAphbBaslangicYil"));

                            anahtar.Deger = cmbTarihBaslangicYilAphb.Text;

                            anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisAphbBaslangicAy"));

                            anahtar.Deger = (cmbTarihBaslangicAyAphb.SelectedIndex + 1).ToString();
                        }

                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBasvuruFormuIndirilsin"));
                    anahtar.Deger = chkBasvuruFormuIndir.Checked ? "True" : "False";

                    DateTime ayinIlkGunu = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfTumIndirilsin"));
                    anahtar.Deger = chkTumTesvikler.Checked ? "True" : "False";

                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYilTum.Text), Convert.ToInt32(cmbTarihBaslangicAyTum.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYilTum.Text), Convert.ToInt32(cmbTarihBitisAyTum.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("Tüm teşvikler başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYilTum"));

                        anahtar.Deger = cmbTarihBaslangicYilTum.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAyTum"));

                        anahtar.Deger = (cmbTarihBaslangicAyTum.SelectedIndex + 1).ToString();
                        //}

                    }


                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf6111Indirilsin"));
                    anahtar.Deger = chk6111.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil6111.Text), Convert.ToInt32(cmbTarihBaslangicAy6111.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil6111.Text), Convert.ToInt32(cmbTarihBitisAy6111.SelectedIndex + 1), 1);


                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;
                        //    MessageBox.Show("6111 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil6111"));

                        anahtar.Deger = cmbTarihBaslangicYil6111.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy6111"));

                        anahtar.Deger = (cmbTarihBaslangicAy6111.SelectedIndex + 1).ToString();
                        //}

                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf6645Indirilsin"));
                    anahtar.Deger = chk6645.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil6645.Text), Convert.ToInt32(cmbTarihBaslangicAy6645.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil6645.Text), Convert.ToInt32(cmbTarihBitisAy6645.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("6645 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil6645"));

                        anahtar.Deger = cmbTarihBaslangicYil6645.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy6645"));

                        anahtar.Deger = (cmbTarihBaslangicAy6645.SelectedIndex + 1).ToString();

                        //}
                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf7103Indirilsin"));
                    anahtar.Deger = chk7103.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil7103.Text), Convert.ToInt32(cmbTarihBaslangicAy7103.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil7103.Text), Convert.ToInt32(cmbTarihBitisAy7103.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("7103 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil7103"));

                        anahtar.Deger = cmbTarihBaslangicYil7103.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy7103"));

                        anahtar.Deger = (cmbTarihBaslangicAy7103.SelectedIndex + 1).ToString();
                        //}
                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf687Indirilsin"));
                    anahtar.Deger = chk687.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil687.Text), Convert.ToInt32(cmbTarihBaslangicAy687.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil687.Text), Convert.ToInt32(cmbTarihBitisAy687.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("687 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil687"));

                        anahtar.Deger = cmbTarihBaslangicYil687.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy687"));

                        anahtar.Deger = (cmbTarihBaslangicAy687.SelectedIndex + 1).ToString();
                        //}
                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf2828Indirilsin"));
                    anahtar.Deger = chk2828.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil2828.Text), Convert.ToInt32(cmbTarihBaslangicAy2828.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil2828.Text), Convert.ToInt32(cmbTarihBitisAy2828.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("2828 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil2828"));

                        anahtar.Deger = cmbTarihBaslangicYil2828.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy2828"));

                        anahtar.Deger = (cmbTarihBaslangicAy2828.SelectedIndex + 1).ToString();
                        //}
                    }


                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf7252Indirilsin"));
                    anahtar.Deger = chk7252.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil7252.Text), Convert.ToInt32(cmbTarihBaslangicAy7252.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil7252.Text), Convert.ToInt32(cmbTarihBitisAy7252.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("7252 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil7252"));

                        anahtar.Deger = cmbTarihBaslangicYil7252.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy7252"));

                        anahtar.Deger = (cmbTarihBaslangicAy7252.SelectedIndex + 1).ToString();
                        //}
                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf7256Indirilsin"));
                    anahtar.Deger = chk7256.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil7256.Text), Convert.ToInt32(cmbTarihBaslangicAy7256.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil7256.Text), Convert.ToInt32(cmbTarihBitisAy7256.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("7256 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil7256"));

                        anahtar.Deger = cmbTarihBaslangicYil7256.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy7256"));

                        anahtar.Deger = (cmbTarihBaslangicAy7256.SelectedIndex + 1).ToString();
                        //}
                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf7316Indirilsin"));
                    anahtar.Deger = chk7316.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil7316.Text), Convert.ToInt32(cmbTarihBaslangicAy7316.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil7316.Text), Convert.ToInt32(cmbTarihBitisAy7316.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("7316 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil7316"));

                        anahtar.Deger = cmbTarihBaslangicYil7316.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy7316"));

                        anahtar.Deger = (cmbTarihBaslangicAy7316.SelectedIndex + 1).ToString();
                        //}
                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf3294Indirilsin"));
                    anahtar.Deger = chk3294.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil3294.Text), Convert.ToInt32(cmbTarihBaslangicAy3294.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil3294.Text), Convert.ToInt32(cmbTarihBitisAy3294.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("3294 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil3294"));

                        anahtar.Deger = cmbTarihBaslangicYil3294.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy3294"));

                        anahtar.Deger = (cmbTarihBaslangicAy3294.SelectedIndex + 1).ToString();
                        //}
                    }

                    anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBf14857Indirilsin"));
                    anahtar.Deger = chk14857.Checked ? "True" : "False";
                    {
                        //DateTime TarihBaslangic = new DateTime(Convert.ToInt32(cmbTarihBaslangicYil14857.Text), Convert.ToInt32(cmbTarihBaslangicAy14857.SelectedIndex + 1), 1);
                        //DateTime TarihBitis = new DateTime(Convert.ToInt32(cmbTarihBitisYil14857.Text), Convert.ToInt32(cmbTarihBitisAy14857.SelectedIndex + 1), 1);

                        //if (TarihBaslangic > TarihBitis)
                        //{
                        //    devam = false;

                        //    MessageBox.Show("14857 başlangıç tarihi bitiş tarihinden büyük olamaz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicYil14857"));

                        anahtar.Deger = cmbTarihBaslangicYil14857.Text;

                        anahtar = dbContext.Ayarlar.FirstOrDefault(p => p.Anahtar.Equals("GecmisBfBaslangicAy14857"));

                        anahtar.Deger = (cmbTarihBaslangicAy14857.SelectedIndex + 1).ToString();
                        //}
                    }

                }

                dbContext.SaveChanges();

                MessageBox.Show("Başarıyla kaydedildi", "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
        }
    }
}
