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
    public partial class frmCari14857Listesi : Form
    {

        DialogResult dr = DialogResult.Cancel;

        List<long> secilenler = new List<long>();

        bool Ara = false;

        public List<Sirketler> sirketListesi { get; set; }

        public frmCari14857Listesi()
        {
            InitializeComponent();
        }

        private void frmSirketler_Load(object sender, EventArgs e)
        {
            SirketleriDoldur();
        }

        private void SirketleriDoldur(int skip = 0, int take = int.MaxValue)
        {
            using (var dbContext = new DbEntities())
            {
                this.sirketListesi = dbContext.Cari14857YapilanSirketler.Include(p => p.Sirketler).Select(p => p.Sirketler).ToList();
            }



            var sirketler = this.sirketListesi.OrderByDescending(p => secilenler.Contains(p.SirketID) ? 1 : 0).ToList();

            take = secilenler.Count > take ? secilenler.Count : take;

            statusSirketSayisi.Text = String.Format("Listeye Ekli Toplam Şirket Sayısı : {0}", sirketListesi.Count);

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

        private void dgvSirketler_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var sirket = dgvSirketler.Rows[e.RowIndex].DataBoundItem as Sirketler;

                if (dgvSirketler.Columns[e.ColumnIndex].Name == "colSec")
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
                else if (dgvSirketler.Columns[e.ColumnIndex].Name == "colListedenCikar")
                {
                    using (var dbContext = new DbEntities())
                    {
                        var cari14857 = dbContext.Cari14857YapilanSirketler.FirstOrDefault(p => p.SirketId.Equals(sirket.SirketID));
                        dbContext.Cari14857YapilanSirketler.Remove(cari14857);
                        dbContext.SaveChanges();
                    }

                    SirketleriDoldur();

                    if (Ara) Search();
                }
            }
        }

        private void dgvSirketler_SelectionChanged(object sender, EventArgs e)
        {
            dgvSirketler.ClearSelection();
        }

        private void frmSirketler_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult = dr;
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

                    this.SirketleriDoldur();
                }

                Ara = false;
            }

        }

        private void Search()
        {
            if (this.sirketListesi.Count > 0)
            {
                var sonuc = this.sirketListesi.Where(
                        a => a.SirketAdi.ToUpper().Contains(txtAra.Text.ToUpper())
                        || a.VergiKimlikNo.ToUpper().Contains(txtAra.Text.ToUpper())
                    ).ToList();


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

        private void btnSecilenleriCari14857ListesindenCikar_Click(object sender, EventArgs e)
        {
            if (secilenler.Count > 0)
            {
                using (var dbContext = new DbEntities())
                {
                    foreach (var sirketId in secilenler)
                    {
                        var cari14857 = dbContext.Cari14857YapilanSirketler.FirstOrDefault(p => p.SirketId.Equals(sirketId));

                        dbContext.Cari14857YapilanSirketler.Remove(cari14857);
                    }

                    dbContext.SaveChanges();
                }

                SirketleriDoldur();

                if (Ara) Search();
            }

        }
    }
}
