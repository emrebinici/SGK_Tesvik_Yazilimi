using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace TesvikProgrami
{
    public partial class frmMesaj : Form
    {
        string Mesaj = "";
        bool SadeceKapatButonu = false;

        public frmMesaj(string mesaj, bool sadeceKapatButonu=false)
        {
            Mesaj = mesaj;

            SadeceKapatButonu = sadeceKapatButonu;

            InitializeComponent();
        }

        private void frmMesaj_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = Mesaj;

            if (Mesaj.Contains("DÜZELTİLEBİLECEKLER")) btnDuzelt.Visible = true;

            if (SadeceKapatButonu)
            {
                btnIptal.Text = "Kapat";
                btnIptal.Left = 23;
                btnYoksay.Visible = false;
                btnDuzelt.Visible = false;
            }
        }

        private void btnYoksay_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Retry;
        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            this.Close();
        }

        private void btnDuzelt_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }
    }
}
