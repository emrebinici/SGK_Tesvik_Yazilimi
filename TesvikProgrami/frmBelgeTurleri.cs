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
    public partial class frmBelgeTurleri: Form
    {
        long eskielementID = 0;

        public frmBelgeTurleri()
        {
            InitializeComponent();
        }

        private void frmBelgeTurleri_Load(object sender, EventArgs e)
        {
            BelgeTurleriDoldur();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (txtBelgeTuruNo.Text.Trim() != "" && 
                txtMalulPrimOraniSigortali.Text != "" && txtMalulPrimOraniIsveren.Text != "" &&
                txtGenelSaglikSigortali.Text != "" && txtGenelSaglikIsveren.Text != "" &&
                txtSosyalDestekSigortali.Text != "" && txtSosyalDestekIsveren.Text != "" &&
                txtIssizlikSigortali.Text != "" && txtIssizlikIsveren.Text != ""
                )
            {
                bool oranlarGecerli = true;

                double MalulYaslilikOraniSigortali=0;

                double MalulYaslilikOraniIsveren =0;

                double GenelSaglikSigortali =0;

                double GenelSaglikIsveren =0;

                double SosyalDestekSigortali =0;

                double SosyalDestekIsveren =0;

                double IssizlikSigortali =0;

                double IssizlikIsveren =0;

                long belgeTuruNo = 0;

                long.TryParse(txtBelgeTuruNo.Text, out belgeTuruNo);

                if (oranlarGecerli) oranlarGecerli = double.TryParse(txtMalulPrimOraniSigortali.Text, out MalulYaslilikOraniSigortali);

                if (oranlarGecerli) oranlarGecerli = double.TryParse(txtMalulPrimOraniIsveren.Text, out MalulYaslilikOraniIsveren);

                if (oranlarGecerli) oranlarGecerli = double.TryParse(txtGenelSaglikSigortali.Text, out GenelSaglikSigortali);

                if (oranlarGecerli) oranlarGecerli = double.TryParse(txtGenelSaglikIsveren.Text, out GenelSaglikIsveren);

                if (oranlarGecerli) oranlarGecerli = double.TryParse(txtSosyalDestekSigortali.Text, out SosyalDestekSigortali);

                if (oranlarGecerli) oranlarGecerli = double.TryParse(txtSosyalDestekIsveren.Text, out SosyalDestekIsveren);

                if (oranlarGecerli) oranlarGecerli = double.TryParse(txtIssizlikSigortali.Text, out IssizlikSigortali);

                if (oranlarGecerli) oranlarGecerli = double.TryParse(txtIssizlikIsveren.Text, out IssizlikIsveren);

                if (oranlarGecerli && belgeTuruNo > 0)
                {

                    using (var dbContext= new DbEntities())
                    {
                        BelgeTurleri belgeTuru = null;

                        if (eskielementID > 0)
                        {
                            belgeTuru = dbContext.BelgeTurleri.FirstOrDefault(p => p.BelgeTuruID.Equals(eskielementID));
                        }

                        bool yeniEklenecek = belgeTuru == null;

                        if  (dbContext.BelgeTurleri.FirstOrDefault(p=> p.BelgeTuruID.Equals(belgeTuruNo) && ! p.BelgeTuruID.Equals(eskielementID)) == null)
                        {
                            belgeTuru = belgeTuru ?? new BelgeTurleri();

                            belgeTuru.BelgeTuruID = belgeTuruNo;
                            belgeTuru.MalulYaslilikOraniSigortali = MalulYaslilikOraniSigortali;
                            belgeTuru.MalulYaslilikOraniIsveren = MalulYaslilikOraniIsveren;
                            belgeTuru.GenelSaglikSigortali = GenelSaglikSigortali;
                            belgeTuru.GenelSaglikIsveren = GenelSaglikIsveren;
                            belgeTuru.SosyalDestekSigortali = SosyalDestekSigortali;
                            belgeTuru.SosyalDestekIsveren = SosyalDestekIsveren;
                            belgeTuru.IssizlikSigortali = IssizlikSigortali;
                            belgeTuru.IssizlikIsveren = IssizlikIsveren;

                            if (yeniEklenecek) dbContext.BelgeTurleri.Add(belgeTuru);

                            dbContext.SaveChanges();

                            BelgeTurleriDoldur();

                            AlanlariTemizle();

                            Program.BelgeTuruOranlari.Clear();

                            Program.BelgeTurleri = dbContext.BelgeTurleri.ToDictionary(x => x.BelgeTuruID, x => x);

                            MessageBox.Show("Kayıt başarılı");

                        }
                        else MessageBox.Show("Aynı belge türü daha önce eklenmiştir", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                else MessageBox.Show("Tüm alanlar sayısal bir değer olmalıdır", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else MessageBox.Show("Zorunlu alanlar boş bırakılamaz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); 


        }

        private void BelgeTurleriDoldur()
        {
            using (var dbContext = new DbEntities())
            {
                dgvBelgeTurleri.AutoGenerateColumns = false;

                dgvBelgeTurleri.DataSource = dbContext.BelgeTurleri.ToList();
            }

        }

        private void dgvBelgeTurleri_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var belgeTuru = (dgvBelgeTurleri.Rows[e.RowIndex].DataBoundItem as BelgeTurleri);

                if (dgvBelgeTurleri.Columns[e.ColumnIndex].Name == "colDuzenle")
                {
                    eskielementID = belgeTuru.BelgeTuruID;

                    txtBelgeTuruNo.Text = belgeTuru.BelgeTuruID.ToString();
                    txtMalulPrimOraniSigortali.Text = belgeTuru.MalulYaslilikOraniSigortali.ToString();
                    txtMalulPrimOraniIsveren.Text = belgeTuru.MalulYaslilikOraniIsveren.ToString();
                    txtGenelSaglikIsveren.Text = belgeTuru.GenelSaglikIsveren.ToString();
                    txtGenelSaglikSigortali.Text = belgeTuru.GenelSaglikSigortali.ToString();
                    txtSosyalDestekSigortali.Text = belgeTuru.SosyalDestekSigortali.ToString();
                    txtSosyalDestekIsveren.Text = belgeTuru.SosyalDestekIsveren.ToString();
                    txtIssizlikSigortali.Text = belgeTuru.IssizlikSigortali.ToString();
                    txtIssizlikIsveren.Text = belgeTuru.IssizlikIsveren.ToString();

                    txtBelgeTuruNo.Enabled = false;

                    lblIptal.Visible = true;
                }
                else if (dgvBelgeTurleri.Columns[e.ColumnIndex].Name == "colSil")
                {
                    if (MessageBox.Show("Silmek istediğinizden emin misiniz", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        using (var dbContext= new DbEntities())
                        {
                            dbContext.BelgeTurleri.Remove(dbContext.BelgeTurleri.FirstOrDefault(p=> p.BelgeTuruID.Equals(belgeTuru.BelgeTuruID)));

                            dbContext.SaveChanges();
                        }

                        BelgeTurleriDoldur();

                        if (eskielementID  > 0)
                        {
                            if (eskielementID.Equals(belgeTuru.BelgeTuruID)) AlanlariTemizle();
                        }
                    }
                }
            }
        }

        private void dgvBelgeTurleri_SelectionChanged(object sender, EventArgs e)
        {
            dgvBelgeTurleri.ClearSelection();
        }

        private void lblIptal_Click(object sender, EventArgs e)
        {
            AlanlariTemizle();
        }

        private void AlanlariTemizle()
        {
            txtBelgeTuruNo.Text = "";

            txtMalulPrimOraniSigortali.Text = "";

            txtMalulPrimOraniIsveren.Text = "";

            txtGenelSaglikSigortali.Text = "";

            txtGenelSaglikIsveren.Text = "";

            txtSosyalDestekSigortali.Text = "";

            txtSosyalDestekIsveren.Text = "";

            txtIssizlikSigortali.Text = "";

            txtIssizlikIsveren.Text = "";

            eskielementID = 0;

            lblIptal.Visible = false;

            txtBelgeTuruNo.Enabled = true;
        }
    }
}
