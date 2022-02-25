using System;
using System.Drawing;
using System.Windows.Forms;

namespace TesvikProgrami
{
    public partial class Captcha : Form
    {

        public string ReturnValue { get; set; }        

        public Bitmap captcha;

        frmIsyerleri frmisyerleri = null;

        DialogResult dr = DialogResult.Cancel;

        int sira = -1;
        public Captcha(frmIsyerleri _frmisyerleri,int _sira)
        {
            InitializeComponent();

            frmisyerleri = _frmisyerleri;

            sira = _sira;
        }

        public Captcha()
        {
            InitializeComponent();
        }

        private void Captcha_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.BringToFront();
        }

        private void btnDevam_Click(object sender, EventArgs e)
        {
            if (txtCaptcha.Text != "")
            {
                if (frmisyerleri != null) frmisyerleri.Captchas[sira] = txtCaptcha.Text;

                dr = DialogResult.OK;

                this.ReturnValue = txtCaptcha.Text;
                this.Close();
            }
        }

        private void Captcha_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = dr;

            Program.CaptchaGosteriliyor = false;

        }


        private void txtCaptcha_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnDevam_Click(null, null);
            }

        }

        private void txtCaptcha_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar("ı"))
            {
                int i = txtCaptcha.SelectionStart;

                txtCaptcha.Text = txtCaptcha.Text.Insert(txtCaptcha.SelectionStart, "I");

                txtCaptcha.SelectionStart = i + 1;

                e.Handled = true;
            }
        }

        private void Captcha_Shown(object sender, EventArgs e)
        {
            picCaptcha.Image = captcha;

            txtCaptcha.Focus();

            Program.CaptchaGosteriliyor = true;

            this.TopMost = true;
            this.BringToFront();
        }

        private void BtnYenile_Click(object sender, EventArgs e)
        {
            dr = DialogResult.OK;

            this.Close();
        }
    }
}
