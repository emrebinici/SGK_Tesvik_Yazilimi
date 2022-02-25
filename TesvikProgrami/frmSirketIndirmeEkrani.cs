using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public partial class frmSirketIndirmeEkrani : Form
    {
        //DialogResult dr = DialogResult.No;

        SirketAphbBasvuruFormuIndirme IndirilenSirket = null;
        frmSirketler frmSirketler = null;

        public frmSirketIndirmeEkrani(SirketAphbBasvuruFormuIndirme indirilenSirket, frmSirketler pfrmSirketler)
        {
            IndirilenSirket = indirilenSirket;
            frmSirketler = pfrmSirketler;

            InitializeComponent();

            int style = NativeWinAPI.GetWindowLong(this.Handle, NativeWinAPI.GWL_EXSTYLE);
            style |= NativeWinAPI.WS_EX_COMPOSITED;
            NativeWinAPI.SetWindowLong(this.Handle, NativeWinAPI.GWL_EXSTYLE, style);

        }



        private void frmIndirmeEkrani_Load(object sender, EventArgs e)
        {
            Guncelle();
        }

        public void Guncelle(IsyeriAphbBasvuruFormuIndirme tamamlanan= null)
        {
            if (tamamlanan == null)
            {

                foreach (var isyerikeyvalue in IndirilenSirket.Isyerleri)
                {

                    var isyeri = isyerikeyvalue.Value;

                    GroupBox grpbx = new GroupBox();
                    grpbx.Text = isyeri.SubeAdi;
                    grpbx.BackColor = System.Drawing.Color.Transparent;
                    grpbx.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
                    grpbx.ForeColor = System.Drawing.Color.White;

                    grpbx.Width = 600;
                    grpbx.Height = 65;
                    grpbx.Name = "grpbx";
                    grpbx.Tag = isyeri;

                    flowpnlIsyerleri.Controls.Add(grpbx);

                    FlowLayoutPanel flowpanel = new FlowLayoutPanel();

                    flowpanel.Dock = DockStyle.Fill;

                    grpbx.Controls.Add(flowpanel);

                    flowpanel.FlowDirection = FlowDirection.LeftToRight;
                    flowpanel.Name = "flowpanel";

                    var tamamlandi = IndirilenSirket.IndirilenIsyerleri[isyeri.IsyeriID].AphbIndirmeleri.All(p => p.IndirmeSonucu.Tamamlandi) &&
                                    IndirilenSirket.IndirilenIsyerleri[isyeri.IsyeriID].BasvuruFormuIndirmeleri.All(p => p.IndirmeSonucu.Tamamlandi);

                    ProgressBar progressBar = tamamlandi ? new NewProgressBar() : new ProgressBar();

                    if (tamamlandi)
                    {
                        progressBar.Style = ProgressBarStyle.Blocks;
                        progressBar.Value = 100;
                    }
                    else
                    {
                        progressBar.Style = ProgressBarStyle.Marquee;
                        progressBar.MarqueeAnimationSpeed = 20;
                    }

                    var basarili = IndirilenSirket.IndirilenIsyerleri[isyeri.IsyeriID].AphbIndirmeleri.All(p => p.IndirmeSonucu.Basarili) &&
                                    IndirilenSirket.IndirilenIsyerleri[isyeri.IsyeriID].BasvuruFormuIndirmeleri.All(p => p.IndirmeSonucu.Basarili);

                    var hataVar = IndirilenSirket.IndirilenIsyerleri[isyeri.IsyeriID].AphbIndirmeleri.Any(p => p.IndirmeSonucu.HataVar) ||
                                    IndirilenSirket.IndirilenIsyerleri[isyeri.IsyeriID].BasvuruFormuIndirmeleri.Any(p => p.IndirmeSonucu.HataVar);

                    if (basarili)
                    {
                        progressBar.ForeColor = System.Drawing.Color.Green;
                    }
                    else if (hataVar)
                    {
                        progressBar.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        progressBar.ForeColor = System.Drawing.Color.Yellow;
                    }

                    var indirilenIsyeri = IndirilenSirket.IndirilenIsyerleri[isyeri.IsyeriID];

                    progressBar.Width = 200;
                    progressBar.Name = "progress";

                    var btnIptal = new Button();
                    btnIptal.BackColor = System.Drawing.Color.DarkRed;
                    btnIptal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    btnIptal.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
                    btnIptal.ForeColor = System.Drawing.Color.White;
                    btnIptal.Margin = new System.Windows.Forms.Padding(2);
                    btnIptal.Size = new System.Drawing.Size(188, 30);
                    btnIptal.Text = "İptal";
                    btnIptal.UseVisualStyleBackColor = false;
                    btnIptal.Click += new System.EventHandler(this.IptalEt);
                    btnIptal.Tag = indirilenIsyeri;
                    btnIptal.Name = "iptal";

                    var btnIsyeriDurumuGoster = new Button();
                    btnIsyeriDurumuGoster.BackColor = System.Drawing.Color.DarkRed;
                    btnIsyeriDurumuGoster.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    btnIsyeriDurumuGoster.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
                    btnIsyeriDurumuGoster.ForeColor = System.Drawing.Color.White;
                    btnIsyeriDurumuGoster.Margin = new System.Windows.Forms.Padding(2);
                    btnIsyeriDurumuGoster.Size = new System.Drawing.Size(188, 30);
                    btnIsyeriDurumuGoster.Text = "İlerleyişi Göster";
                    btnIsyeriDurumuGoster.UseVisualStyleBackColor = false;
                    btnIsyeriDurumuGoster.Click += new System.EventHandler(this.LoglariGoster);
                    btnIsyeriDurumuGoster.Tag = indirilenIsyeri;


                    flowpanel.Controls.Add(progressBar);

                    if (tamamlandi == false)
                    {
                        flowpanel.Controls.Add(btnIptal);
                    }

                    flowpanel.Controls.Add(btnIsyeriDurumuGoster);

                    flowpanel.Controls.Cast<Control>().ToList().ForEach(control =>
                    {
                        control.Anchor = AnchorStyles.Left;
                    });
                }


            }
            else
            {
                var groupboxes = flowpnlIsyerleri.Controls.Find("grpbx", false);

                foreach (GroupBox item in groupboxes)
                {
                    if ((item.Tag as Isyerleri).IsyeriID.Equals(tamamlanan.isyeri.IsyeriID))
                    {
                        var progress = item.Controls.Find("progress", true).FirstOrDefault() as ProgressBar;
                        var btniptal = item.Controls.Find("iptal", true).FirstOrDefault();

                        if (btniptal == null) break;

                        var flowpanel = item.Controls.Find("flowpanel", true).FirstOrDefault();
                        

                        flowpanel.Controls.Remove(progress);
                        flowpanel.Controls.Remove(btniptal);

                        ProgressBar progressBar = new NewProgressBar();
                        progressBar.Value = 100;

                        var basarili = tamamlanan.AphbIndirmeleri.All(p => p.IndirmeSonucu.Basarili) &&
                                        tamamlanan.BasvuruFormuIndirmeleri.All(p => p.IndirmeSonucu.Basarili);

                        var hataVar = tamamlanan.AphbIndirmeleri.Any(p => p.IndirmeSonucu.HataVar) ||
                                        tamamlanan.BasvuruFormuIndirmeleri.Any(p => p.IndirmeSonucu.HataVar);

                        if (basarili)
                        {
                            progressBar.ForeColor = System.Drawing.Color.Green;
                        }
                        else if (hataVar)
                        {
                            progressBar.ForeColor = System.Drawing.Color.Red;
                        }
                        else
                        {
                            progressBar.ForeColor = System.Drawing.Color.Yellow;
                        }
                        progressBar.Width = 200;
                        progressBar.Name = "progress";

                        flowpanel.Controls.Add(progressBar);
                        flowpanel.Controls.SetChildIndex(progressBar, 0);

                        progress.Dispose();
                        btniptal.Dispose();

                        break;
                    }
                }
            }

            var tamamlananSayi = 0;
            foreach (var isyerikeyvalue in IndirilenSirket.Isyerleri)
            {
                var tamamlandi = IndirilenSirket.IndirilenIsyerleri[isyerikeyvalue.Value.IsyeriID].AphbIndirmeleri.All(p => p.IndirmeSonucu.Tamamlandi) &&
                                        IndirilenSirket.IndirilenIsyerleri[isyerikeyvalue.Value.IsyeriID].BasvuruFormuIndirmeleri.All(p => p.IndirmeSonucu.Tamamlandi);

                if (tamamlandi) tamamlananSayi++;
            }


            var groupboxes2 = flowpnlIsyerleri.Controls.Find("grpbx", false);

            if (flowpnlIsyerleri.VerticalScroll.Visible)
            {
                if (tamamlananSayi > 0)
                {
                    flowpnlIsyerleri.ScrollControlIntoView(groupboxes2[tamamlananSayi - 1]);
                }
            }

            btnTumunuIptalEt.Visible = groupboxes2.Length > tamamlananSayi;


        }

        void IptalEt(object sender, EventArgs e)
        {
            var isyeriIndirilen = ((Button)sender).Tag as IsyeriAphbBasvuruFormuIndirme;

            foreach (var item in isyeriIndirilen.AphbIndirmeleri)
            {
                if (item.IndirmeSonucu.Tamamlandi == false)
                {
                    item.Cancel();
                }
            }

            foreach (var item in isyeriIndirilen.BasvuruFormuIndirmeleri)
            {
                if (item.IndirmeSonucu.Tamamlandi == false)
                {
                    item.IslemiIptalEt();
                }
            }
        }

        void LoglariGoster(object sender, EventArgs e)
        {
            var isyeriIndirilen = ((Button)sender).Tag as IsyeriAphbBasvuruFormuIndirme;

            if (isyeriIndirilen.formIndirmeEkrani == null)
            {
                isyeriIndirilen.formIndirmeEkrani = new frmIndirmeEkrani(isyeriIndirilen,this);
                isyeriIndirilen.formIndirmeEkrani.Text = String.Format("{0} - {1}", IndirilenSirket.sirket.SirketAdi, isyeriIndirilen.isyeri.SubeAdi);
            }

            isyeriIndirilen.formIndirmeEkrani.WindowState = FormWindowState.Normal;
            isyeriIndirilen.formIndirmeEkrani.Show();
            isyeriIndirilen.formIndirmeEkrani.Activate();


        }

        public class NewProgressBar : ProgressBar
        {
            public NewProgressBar()
            {
                this.SetStyle(ControlStyles.UserPaint, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Rectangle rec = e.ClipRectangle;

                rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;
                if (ProgressBarRenderer.IsSupported)
                    ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
                rec.Height = rec.Height - 4;

                var brush = this.ForeColor == Color.Yellow ? Brushes.Yellow : this.ForeColor == Color.Red ? Brushes.Red : Brushes.Green;

                e.Graphics.FillRectangle(brush, 2, 2, rec.Width, rec.Height);
            }
        }

        internal static class NativeWinAPI
        {
            internal static readonly int GWL_EXSTYLE = -20;
            internal static readonly int WS_EX_COMPOSITED = 0x02000000;

            [DllImport("user32")]
            internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32")]
            internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        }

        private void frmIndirmeEkrani_Shown(object sender, EventArgs e)
        {
            Guncelle();
        }

        private void frmSirketIndirmeEkrani_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Program.IndirilenSirketler.ContainsKey(IndirilenSirket.sirket.SirketID))
            {
                var formlar=IndirilenSirket.IndirilenIsyerleri.Where(p => p.Value.formIndirmeEkrani != null).Select(p => p.Value.formIndirmeEkrani);
                var count = formlar.Count();
                
                for (int i = 0; i < count; i++)
                {
                    formlar.ElementAt(i).Close();
                }
            }

        }

        private void btnTumunuIptalEt_Click(object sender, EventArgs e)
        {
            IndirilenSirket.TumunuIptalEt();
            btnTumunuIptalEt.Visible = false;
        }
    }
}
