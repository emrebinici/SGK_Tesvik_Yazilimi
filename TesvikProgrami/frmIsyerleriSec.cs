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
    public partial class frmIsyerleriSec : Form
    {
        DialogResult dr = DialogResult.No;

        public Dictionary<Isyerleri,Classes.FormIndirmeTarihSecenekleri> isyerleri = null;
        public Dictionary<Isyerleri, Classes.FormIndirmeTarihSecenekleri> SeciliIsyerleri = null;

        public frmIsyerleriSec(List<Isyerleri> pisyerleri, Classes.FormIndirmeTarihSecenekleri tarihsecenekleri)
        {
            InitializeComponent();

            isyerleri = pisyerleri.ToDictionary(x=> x, x=> tarihsecenekleri.Clone());

        }

        private void lnklblDevam_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var chklst = flowpnlIsyerleri.Controls.Find("chk", true).Cast<CheckBox>();

            if (chklst.Any(p=> p.Checked))
            {
                SeciliIsyerleri = chklst.Where(p => p.Checked).Select(x => x.Tag as Isyerleri).ToDictionary(x => x, x=> isyerleri[x]);

                dr = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show("En az bir işyeri seçmelisiniz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        private void frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = dr;
        }

        private void frm_Load(object sender, EventArgs e)
        {

            flowpnlIsyerleri.Controls.Clear();


            foreach (var keyvalue in isyerleri)
            {
                var isyeri = keyvalue.Key;

                FlowLayoutPanel flowisyeri = new FlowLayoutPanel();
                flowisyeri.FlowDirection = FlowDirection.LeftToRight;
                flowisyeri.BackColor = System.Drawing.Color.Transparent;
                flowisyeri.Width = 530;
                flowisyeri.Height = 50;


                flowpnlIsyerleri.Controls.Add(flowisyeri);

                CheckBox chk = new CheckBox();
                chk.Tag = isyeri;
                chk.Text = isyeri.SubeAdi;
                chk.BackColor = System.Drawing.Color.Transparent;
                chk.ForeColor= Color.White;
                chk.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
                chk.Name = "chk";
                chk.Click += this.chk_Changed;
                chk.Width = 300;
                chk.Height = 50;
                chk.Checked = true;

                var btnTarihSecenekleri = new Button();
                btnTarihSecenekleri.BackColor = System.Drawing.Color.DarkRed;
                btnTarihSecenekleri.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                btnTarihSecenekleri.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
                btnTarihSecenekleri.ForeColor = System.Drawing.Color.White;
                btnTarihSecenekleri.Margin = new System.Windows.Forms.Padding(2);
                btnTarihSecenekleri.Size = new System.Drawing.Size(188, 30);
                btnTarihSecenekleri.Text = "Tarih Seçenekleri";
                btnTarihSecenekleri.UseVisualStyleBackColor = false;
                btnTarihSecenekleri.Click += new System.EventHandler(this.tarihSecenekleri_Click);
                btnTarihSecenekleri.Tag = isyeri;

                flowisyeri.Controls.Add(chk);
                flowisyeri.Controls.Add(btnTarihSecenekleri);

                flowisyeri.Controls.Cast<Control>().ToList().ForEach(control => {
                    control.Anchor = AnchorStyles.Left;
                });
            }

        }

        private void chkTumunuSecKaldir_Click(object sender, EventArgs e)
        {
            var chklst = flowpnlIsyerleri.Controls.Find("chk",true).Cast<CheckBox>().ToList();

            chklst.ForEach(p => p.Checked = chkTumunuSecKaldir.Checked);
        }

        private void chk_Changed(object sender, EventArgs e)
        {
            var chklst = flowpnlIsyerleri.Controls.Find("chk", true).Cast<CheckBox>().ToList();

            chkTumunuSecKaldir.Checked = chklst.Count(p => p.Checked) == isyerleri.Count;
        }

        private void tarihSecenekleri_Click(object sender, EventArgs e)
        {
            var isyeri = (sender as Button).Tag as Isyerleri;
            
            var frmsecenekler = new frmTarihSec(isyerleri[isyeri]);

            if (frmsecenekler.ShowDialog() == DialogResult.OK)
            {
                isyerleri[isyeri] = frmsecenekler.secenekler;
            }

        }
    }
}
