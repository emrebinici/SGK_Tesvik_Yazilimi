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
    public partial class frmOnay : Form
    {
        string Mesaj = "";

        public frmOnay(string mesaj)
        {
            Mesaj = mesaj;

            InitializeComponent();
        }

        private void frmMesaj_Load(object sender, EventArgs e)
        {
            txtMesaj.Text = Mesaj;

        }

        private void btnYoksay_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;

            this.Close();
        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            this.Close();
        }

        private void frmOnay_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.Yes) this.DialogResult = DialogResult.Cancel;
        }

        public void Kapat()
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}
