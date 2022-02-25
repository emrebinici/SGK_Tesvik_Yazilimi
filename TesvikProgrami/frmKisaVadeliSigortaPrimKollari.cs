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
    public partial class frmKisaVadeliSigortaPrimKollari : Form
    {
        long eskielementID = 0;


        public frmKisaVadeliSigortaPrimKollari()
        {
            InitializeComponent();
        }

        private void frmKisaVadeliSigortaPrimKollari_Load(object sender, EventArgs e)
        {
            KisaVadeliSigortaPrimKollariDoldur();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (txtKisaVadeliSigortaKoluKodu.Text.Trim() != "" && txtPrimOrani.Text.Trim() != "")
            {
                double PrimOrani;

                bool gecerlioran = double.TryParse(txtPrimOrani.Text.Replace(".", ","), out PrimOrani);

                if (!gecerlioran) gecerlioran = double.TryParse(txtPrimOrani.Text.Replace(",", "."), out PrimOrani);

                if (gecerlioran)
                {
                    var sigortaKolu = txtKisaVadeliSigortaKoluKodu.Text;

                    using (var dbContext = new DbEntities())
                    {
                        KisaVadeliSigortaPrimKoluOranlari kvsk = null;

                        if (eskielementID > 0)
                        {
                            kvsk = dbContext.KisaVadeliSigortaPrimKoluOranlari.FirstOrDefault(p => p.ID.Equals(eskielementID));
                        }

                        bool yeniEklenecek = kvsk == null;

                        if (dbContext.KisaVadeliSigortaPrimKoluOranlari.FirstOrDefault(p => p.KisaVadeliSigortaKoluKodu.Equals(sigortaKolu) && !p.ID.Equals(eskielementID)) == null)
                        {
                            kvsk = kvsk ?? new KisaVadeliSigortaPrimKoluOranlari();

                            kvsk.KisaVadeliSigortaKoluKodu = sigortaKolu;
                            kvsk.PrimOrani = PrimOrani;

                            if (yeniEklenecek) dbContext.KisaVadeliSigortaPrimKoluOranlari.Add(kvsk);

                            dbContext.SaveChanges();

                            KisaVadeliSigortaPrimKollariDoldur();

                            Program.KisaVadeliSigortaPrimKoluOranlari = dbContext.KisaVadeliSigortaPrimKoluOranlari.ToDictionary(x => x.KisaVadeliSigortaKoluKodu.PadLeft(4, '0'), x => x);

                            AlanlariTemizle();

                            MessageBox.Show("Kayıt başarılı");

                        }
                        else MessageBox.Show("Aynı kısa vadeli sigorta kolu daha önce eklenmiştir", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                else MessageBox.Show("Prim oranı sayısal bir değer olmalıdır", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else MessageBox.Show("Zorunlu alanlar boş bırakılamaz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void KisaVadeliSigortaPrimKollariDoldur()
        {
            using (var dbContext = new DbEntities())
            {
                var list = dbContext.KisaVadeliSigortaPrimKoluOranlari.ToList();

                if (txtAra.Text.Length >= 3 && txtAra.Text != "Ara")
                {
                    list = list.Where(p => p.KisaVadeliSigortaKoluKodu.Equals(txtAra.Text)).ToList();
                }

                dgvKisaVadeliSigortaKollari.AutoGenerateColumns = false;
                dgvKisaVadeliSigortaKollari.DataSource = list;
            }

        }

        private void lblIptal_Click(object sender, EventArgs e)
        {
            AlanlariTemizle();
        }

        private void AlanlariTemizle()
        {
            txtKisaVadeliSigortaKoluKodu.Text = "";

            txtPrimOrani.Text = "";

            eskielementID = 0;

            lblIptal.Visible = false;
        }

        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            KisaVadeliSigortaPrimKollariDoldur();
        }

        private void txtAra_Enter(object sender, EventArgs e)
        {
            if (txtAra.Text == "Ara")
            {
                txtAra.Text = "";

                txtAra.ForeColor = Color.FromKnownColor(KnownColor.Black);
            }
        }

        private void txtAra_Leave(object sender, EventArgs e)
        {
            if (txtAra.Text == "")
            {
                txtAra.Text = "Ara";

                txtAra.ForeColor = Color.FromKnownColor(KnownColor.DarkGray);
            }
        }

        private void dgvKisaVadeliSigortaKollari_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var kvsk = dgvKisaVadeliSigortaKollari.Rows[e.RowIndex].DataBoundItem as KisaVadeliSigortaPrimKoluOranlari;

                if (dgvKisaVadeliSigortaKollari.Columns[e.ColumnIndex].Name == "colDuzenle")
                {
                    eskielementID = kvsk.ID;

                    txtKisaVadeliSigortaKoluKodu.Text = kvsk.KisaVadeliSigortaKoluKodu;

                    txtPrimOrani.Text = kvsk.PrimOrani.ToString();

                    lblIptal.Visible = true;
                }
                else if (dgvKisaVadeliSigortaKollari.Columns[e.ColumnIndex].Name == "colSil")
                {
                    if (MessageBox.Show("Silmek istediğinizden emin misiniz", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        using (var dbContext = new DbEntities())
                        {
                            dbContext.KisaVadeliSigortaPrimKoluOranlari.Remove(dbContext.KisaVadeliSigortaPrimKoluOranlari.FirstOrDefault(p => p.ID.Equals(kvsk.ID)));

                            dbContext.SaveChanges();
                        }

                        KisaVadeliSigortaPrimKollariDoldur();

                        if (eskielementID > 0 && kvsk.ID.Equals(eskielementID)) AlanlariTemizle();
                    }

                }
            }
        }

        private void dgvKisaVadeliSigortaKollari_SelectionChanged(object sender, EventArgs e)
        {
            dgvKisaVadeliSigortaKollari.ClearSelection();
        }

    }
}
