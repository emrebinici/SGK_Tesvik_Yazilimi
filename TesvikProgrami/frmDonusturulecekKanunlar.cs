using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TesvikProgrami
{
    public partial class frmDonusturulecekKanunlar : Form
    {
        bool Kaydedilsin = false;

        List<string> TesvikVerilecekKanunlar = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.Where(p => !p.Equals("6322/25510")).ToList();

        List<string> DonusturulecekTesvikler = null;

        public frmDonusturulecekKanunlar()
        {
            InitializeComponent();
        }

        private void frmDonusturulecekKanunlar_Load(object sender, EventArgs e)
        {

            DonusturulecekTesvikler = TesvikVerilecekKanunlar.Select(p => p.PadLeft(5,'0')).ToList();

            DonusturulecekTesvikler.Add("16322");
            DonusturulecekTesvikler.Add("26322");
            DonusturulecekTesvikler.Add("25510");
            DonusturulecekTesvikler.Add("85615");
            DonusturulecekTesvikler.Add("05615");
            DonusturulecekTesvikler.Add("05084");
            DonusturulecekTesvikler.Add("85084");
            DonusturulecekTesvikler.Add("00000");

            this.Location = new Point(0, 100);
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            tlp.Controls.Clear();
            tlp.ColumnStyles.Clear();
            tlp.RowStyles.Clear();
            tlp.ColumnCount = DonusturulecekTesvikler.Count;
            tlp.RowCount = TesvikVerilecekKanunlar.Count;

            for (int x = 0; x < tlp.ColumnCount; x++)
            {
                //First add a column
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150f));

                for (int y = 0; y < tlp.RowCount; y++)
                {
                    //Next, add a row.  Only do this when once, when creating the first column
                    if (x == 0)
                    {
                        tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100 / (TesvikVerilecekKanunlar.Count + 1)));
                        //tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    }

                    var siraliTesvikler = DonusturulecekTesvikler.OrderBy(p => p.Equals(TesvikVerilecekKanunlar[y].PadLeft(5,'0')) ? -1 : DonusturulecekTesvikler.IndexOf(p)).ToList();

                    if (x == 0)
                    {
                        Label lbl = new Label();
                        lbl.Name = "lbl" + TesvikVerilecekKanunlar[y] + "-" + siraliTesvikler[x];
                        lbl.Font = new Font(lbl.Font.FontFamily, 15, FontStyle.Bold | FontStyle.Underline);
                        lbl.ForeColor = Color.White;
                        lbl.Anchor = AnchorStyles.None;
                        lbl.AutoSize = false;
                        lbl.Text = siraliTesvikler[x].ToInt().ToString();
                        tlp.Controls.Add(lbl, x, y);
                    }
                    else
                    {
                        FlowLayoutPanel flp = new FlowLayoutPanel();
                        flp.FlowDirection = FlowDirection.TopDown;

                        //flp.Anchor = AnchorStyles.None;
                        flp.Dock = DockStyle.Fill;

                        CheckBox chk = new CheckBox();
                        chk.Name = "chk" + TesvikVerilecekKanunlar[y] + "-" + siraliTesvikler[x];
                        chk.Font = new Font(chk.Font.FontFamily, 12, FontStyle.Bold);
                        chk.ForeColor = Color.White;
                        chk.Anchor = AnchorStyles.Left;
                        chk.AutoSize = false;
                        chk.Text = siraliTesvikler[x].ToInt() == 0 ? "00000" : siraliTesvikler[x].ToInt().ToString();
                        chk.Click += chkClick;
                        //tlp.Controls.Add(chk, x, y);

                        CheckBox chkSadeceCari = new CheckBox();
                        chkSadeceCari.Name = "chkSadeceCari" + TesvikVerilecekKanunlar[y] + "-" + siraliTesvikler[x];
                        chkSadeceCari.Font = new Font(chkSadeceCari.Font.FontFamily, 12, FontStyle.Bold);
                        chkSadeceCari.ForeColor = Color.White;
                        chkSadeceCari.Anchor = AnchorStyles.Left;
                        chkSadeceCari.AutoSize = true;
                        chkSadeceCari.Text = "Sadece Cari";
                        chkSadeceCari.Enabled = false;
                        chkSadeceCari.Click += chkSadeceCariClick;
                        //chk.Dock = DockStyle.Fill;

                        flp.Controls.Add(chk);
                        flp.Controls.Add(chkSadeceCari);

                        tlp.Controls.Add(flp, x, y);
                    }
                }
            }

            Doldur();

        }

        void chkClick(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;

            var chkSadeceCari = ((chk.Parent as FlowLayoutPanel).Controls.Find("chkSadeceCari" + chk.Name.Replace("chk", ""), false).FirstOrDefault() as CheckBox);

            chkSadeceCari.Enabled = chk.Checked;

            Kaydedilsin = true;
        }

        void chkSadeceCariClick(object sender, EventArgs e)
        {
            Kaydedilsin = true;
        }

        private void tlp_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.White, new Point(e.CellBounds.Location.X, e.CellBounds.Location.Y + e.CellBounds.Height), new Point(e.CellBounds.Right, e.CellBounds.Bottom));
            if (e.Column == 0)
            {
                e.Graphics.DrawLine(Pens.White, new Point(e.CellBounds.Right - 10, e.CellBounds.Location.Y), new Point(e.CellBounds.Right - 10, e.CellBounds.Location.Y + e.CellBounds.Height));

            }

        }

        private void frmDonusturulecekKanunlar_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Kaydedilsin) Kaydet();
        }

        private void Doldur()
        {

            using (var dbContext = new DbEntities())
            {
                var list = dbContext.DonusturulecekKanunlar.ToList();

                foreach (var dk in list)
                {
                    var donusturulecekKanunNo = dk.DonusturulenKanunNo.PadLeft(5,'0');
                    var tesvikKanun = dk.TesvikKanunNo;
                    var chk = tlp.Controls.Find("chk" + tesvikKanun + "-" + donusturulecekKanunNo, true).FirstOrDefault() as CheckBox;
                    var chkSadeceCari = tlp.Controls.Find("chkSadeceCari" + tesvikKanun + "-" + donusturulecekKanunNo, true).FirstOrDefault() as CheckBox;

                    chk.Checked = true;

                    chkSadeceCari.Checked = Convert.ToBoolean(dk.SadeceCari);

                    chkSadeceCari.Enabled = true;
                }

            }
        }

        void Kaydet()
        {

            using (var dbContext = new DbEntities())
            {
                var list = dbContext.DonusturulecekKanunlar.ToList();

                foreach (var tesvikKanun in TesvikVerilecekKanunlar)
                {
                    foreach (var donusturulecekKanunNo in DonusturulecekTesvikler)
                    {
                        var donusturulecekKanunSon = donusturulecekKanunNo.PadLeft(5, '0');

                        if (tesvikKanun.Equals(donusturulecekKanunNo.ToInt().ToString()) || tesvikKanun.Equals(donusturulecekKanunSon)) continue;

                        var chkbox = tlp.Controls.Find("chk" + tesvikKanun + "-" + donusturulecekKanunSon, true).FirstOrDefault() as CheckBox;
                        var chkSadeceCari = tlp.Controls.Find("chkSadeceCari" + tesvikKanun + "-" + donusturulecekKanunSon, true).FirstOrDefault() as CheckBox;

                        if (chkbox.Checked)
                        {
                            var dk = list.FirstOrDefault(p => p.TesvikKanunNo.Equals(tesvikKanun) && (p.DonusturulenKanunNo.Equals(donusturulecekKanunSon) || p.DonusturulenKanunNo.Equals(donusturulecekKanunNo.ToInt().ToString())));

                            bool yeniEklenecek = dk == null;

                            dk = dk ?? new DonusturulecekKanunlar();

                            dk.TesvikKanunNo = tesvikKanun;
                            dk.DonusturulenKanunNo = donusturulecekKanunSon;
                            dk.SadeceCari = Convert.ToDecimal(chkSadeceCari.Checked);

                            if (yeniEklenecek) dbContext.DonusturulecekKanunlar.Add(dk);
                        }
                        else
                        {
                            var dk = list.FirstOrDefault(p => p.TesvikKanunNo.Equals(tesvikKanun) && (p.DonusturulenKanunNo.Equals(donusturulecekKanunSon) || p.DonusturulenKanunNo.Equals(donusturulecekKanunNo.ToInt().ToString())));

                            if (dk != null)
                            {
                                dbContext.DonusturulecekKanunlar.Remove(dk);
                            }
                        }
                    }
                }

                dbContext.SaveChanges();

                Program.DonusturulecekKanunlar = dbContext.DonusturulecekKanunlar.ToList();
            }

            Program.TumTesvikler = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new Classes.Tesvik(x));

            MessageBox.Show("Dönüştürülecek kanunlar başarıyla kaydedildi");

        }
    }
}
