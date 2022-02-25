using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace TesvikProgrami
{
    public partial class frmAsgariUcretler : Form
    {
        long eskielementID = 0;

        public frmAsgariUcretler()
        {
            InitializeComponent();
        }

        private void frmAsgariUcretler_Load(object sender, EventArgs e)
        {
            AsgariUcretleriDoldur();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (txtAsgariUcret.Text.Trim() != "")
            {
                double AsgariUcret;

                bool gecerlioran = Double.TryParse(txtAsgariUcret.Text.Replace(".", ","), out AsgariUcret);

                if (!gecerlioran) gecerlioran = Double.TryParse(txtAsgariUcret.Text.Replace(",", "."), out AsgariUcret);

                if (gecerlioran)
                {

                    using (var dbContext = new DbEntities())
                    {
                        var asgariUcret = eskielementID == 0 ? new AsgariUcretler() : dbContext.AsgariUcretler.Find(eskielementID);

                        asgariUcret.Baslangic = dtpBaslangic.Value.ToShortDateString();
                        asgariUcret.Bitis = dtpBitis.Value.ToShortDateString();
                        asgariUcret.AsgariUcretTutari = AsgariUcret;

                        if (eskielementID == 0) dbContext.AsgariUcretler.Add(asgariUcret);

                        dbContext.SaveChanges();
                    }

                    AsgariUcretleriDoldur();

                    AlanlariTemizle();

                    Program.AsgariUcretler.Clear();

                    MessageBox.Show("Kayıt başarılı");


                }
                else MessageBox.Show("Asgari ücret sayısal bir değer olmalıdır", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }
            else MessageBox.Show("Zorunlu alanlar boş bırakılamaz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void AsgariUcretleriDoldur()
        {
            dgvAsgariUcretler.AutoGenerateColumns = false;

            using (var dbContext = new DbEntities())
            {
                dgvAsgariUcretler.DataSource = dbContext.AsgariUcretler.ToList();
            }

        }

        private void dgvBelgeTurleri_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                
                var secilenAsgariUcret = dgvAsgariUcretler.Rows[e.RowIndex].DataBoundItem as AsgariUcretler;

                if (dgvAsgariUcretler.Columns[e.ColumnIndex].Name == "colDuzenle")
                {
                    eskielementID = secilenAsgariUcret.ID;

                    dtpBaslangic.Value = Convert.ToDateTime(secilenAsgariUcret.Baslangic);

                    dtpBitis.Value = Convert.ToDateTime(secilenAsgariUcret.Bitis);

                    txtAsgariUcret.Text = secilenAsgariUcret.AsgariUcretTutari.ToString();

                    lblIptal.Visible = true;
                }
                else if (dgvAsgariUcretler.Columns[e.ColumnIndex].Name == "colSil")
                {
                    if (MessageBox.Show("Silmek istediğinizden emin misiniz", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        using (var dbContext = new DbEntities())
                        {
                            dbContext.AsgariUcretler.Remove(dbContext.AsgariUcretler.Find(secilenAsgariUcret.ID));

                            dbContext.SaveChanges();
                        }

                        if (eskielementID > 0)
                        {
                            if (eskielementID.Equals(secilenAsgariUcret.ID)) AlanlariTemizle();
                        }

                        AsgariUcretleriDoldur();
                    }

                }
            }
        }

        private void dgvBelgeTurleri_SelectionChanged(object sender, EventArgs e)
        {
            dgvAsgariUcretler.ClearSelection();
        }

        private void lblIptal_Click(object sender, EventArgs e)
        {
            AlanlariTemizle();
        }

        private void AlanlariTemizle()
        {
            txtAsgariUcret.Text = "";

            eskielementID = 0;

            lblIptal.Visible = false;
        }
    }
}
