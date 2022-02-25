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
    public partial class frmIndirmeEkrani : Form
    {
        //DialogResult dr = DialogResult.No;

        IsyeriAphbBasvuruFormuIndirme IndirilenIsyeri = null;

        public frmSirketIndirmeEkrani frmSirketIndirmeEkrani = null;

        public frmIndirmeEkrani(IsyeriAphbBasvuruFormuIndirme indirilenIsyeri , frmSirketIndirmeEkrani frmsirketindirme = null)
        {
            IndirilenIsyeri = indirilenIsyeri;

            frmSirketIndirmeEkrani = frmsirketindirme;

            InitializeComponent();

            int style = NativeWinAPI.GetWindowLong(this.Handle, NativeWinAPI.GWL_EXSTYLE);
            style |= NativeWinAPI.WS_EX_COMPOSITED;
            NativeWinAPI.SetWindowLong(this.Handle, NativeWinAPI.GWL_EXSTYLE, style);

        }

        private void frmIndirmeEkrani_Load(object sender, EventArgs e)
        {
            Guncelle();
        }

        public void Guncelle(IsyeriAphbBasvuruFormuIndirme tamamlanan = null)
        {
            if (tamamlanan == null)
            {
                flowpnlIndirmeler.Controls.Clear();

                var indirmeler = new List<Indir>();

                indirmeler.AddRange(IndirilenIsyeri.AphbIndirmeleri);
                indirmeler.AddRange(IndirilenIsyeri.BasvuruFormuIndirmeleri);

                foreach (var indir in indirmeler)
                {
                    FlowLayoutPanel flowpanel = new FlowLayoutPanel();

                    flowpanel.Width = 800;
                    flowpanel.Height = 50;
                    flowpanel.Tag = indir;
                    flowpanel.Name = "flowpanel";

                    flowpnlIndirmeler.Controls.Add(flowpanel);

                    flowpanel.FlowDirection = FlowDirection.LeftToRight;

                    Label lbl = new Label();
                    lbl.Width = 200;
                    lbl.Text = indir is AphbIndir ? "Aphb" : (indir as BasvuruFormuIndir).bfsira.BasvuruFormuAdiGetir();
                    lbl.BackColor = System.Drawing.Color.Transparent;
                    lbl.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
                    lbl.ForeColor = System.Drawing.Color.White;


                    ProgressBar progressBar = indir.IndirmeSonucu.Tamamlandi ? new NewProgressBar() : new ProgressBar();

                    if (indir.IndirmeSonucu.Tamamlandi)
                    {
                        progressBar.Style = ProgressBarStyle.Blocks;
                        progressBar.Value = 100;
                    }
                    else
                    {
                        progressBar.Style = ProgressBarStyle.Marquee;
                        progressBar.MarqueeAnimationSpeed = 20;
                    }

                    if (indir.IndirmeSonucu.Basarili)
                    {
                        progressBar.ForeColor = System.Drawing.Color.Green;
                    }
                    else if (indir.IndirmeSonucu.HataVar)
                    {
                        progressBar.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        progressBar.ForeColor = System.Drawing.Color.Yellow;
                    }

                    if (indir.IndirmeSonucu.HataVar)
                    {
                        progressBar.ForeColor = System.Drawing.Color.Red;
                    }

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
                    btnIptal.Tag = indir;
                    btnIptal.Name = "iptal";


                    var btnLoglariGoster = new Button();
                    btnLoglariGoster.BackColor = System.Drawing.Color.DarkRed;
                    btnLoglariGoster.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    btnLoglariGoster.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
                    btnLoglariGoster.ForeColor = System.Drawing.Color.White;
                    btnLoglariGoster.Margin = new System.Windows.Forms.Padding(2);
                    btnLoglariGoster.Size = new System.Drawing.Size(188, 30);
                    btnLoglariGoster.Text = "İlerleyişi Göster";
                    btnLoglariGoster.UseVisualStyleBackColor = false;
                    btnLoglariGoster.Click += new System.EventHandler(this.LoglariGoster);
                    btnLoglariGoster.Tag = indir;


                    flowpanel.Controls.Add(lbl);
                    flowpanel.Controls.Add(progressBar);

                    if (indir.IndirmeSonucu.Tamamlandi == false)
                    {
                        flowpanel.Controls.Add(btnIptal);
                    }

                    flowpanel.Controls.Add(btnLoglariGoster);

                    flowpanel.Controls.Cast<Control>().ToList().ForEach(control =>
                    {
                        control.Anchor = AnchorStyles.Left;
                    });
                }
            }
            else
            {
                var flowpanels = flowpnlIndirmeler.Controls.Find("flowpanel", false);

                foreach (var item in tamamlanan.AphbIndirmeleri)
                {
                    if (item.IndirmeSonucu.Tamamlandi)
                    {
                        
                        var fp = flowpanels.FirstOrDefault(p => p.Tag is AphbIndir);

                        var btniptal = fp.Controls.Find("iptal", false).FirstOrDefault();

                        if (btniptal == null) continue;

                        var progress = fp.Controls.Find("progress", false).FirstOrDefault() as ProgressBar;
                        
                        fp.Controls.Remove(progress);
                        fp.Controls.Remove(btniptal);

                        ProgressBar progressBar = new NewProgressBar();
                        progressBar.Value = 100;

                        var basarili = item.IndirmeSonucu.Basarili;

                        var hataVar = item.IndirmeSonucu.HataVar;

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

                        fp.Controls.Add(progressBar);
                        fp.Controls.SetChildIndex(progressBar, 1);

                        progress.Dispose();
                        btniptal.Dispose();


                    }
                }

                foreach (var item in tamamlanan.BasvuruFormuIndirmeleri)
                {
                    if (item.IndirmeSonucu.Tamamlandi)
                    {
                        var fp = flowpanels.FirstOrDefault(p => p.Tag is BasvuruFormuIndir && (p.Tag as BasvuruFormuIndir).bfsira.Equals(item.bfsira));
                        var btniptal = fp.Controls.Find("iptal", false).FirstOrDefault();

                        if (btniptal == null) continue;

                        var progress = fp.Controls.Find("progress", false).FirstOrDefault() as ProgressBar;
                        

                        fp.Controls.Remove(progress);
                        fp.Controls.Remove(btniptal);

                        ProgressBar progressBar = new NewProgressBar();
                        progressBar.Value = 100;

                        var basarili = item.IndirmeSonucu.Basarili;

                        var hataVar = item.IndirmeSonucu.HataVar;

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

                        fp.Controls.Add(progressBar);
                        fp.Controls.SetChildIndex(progressBar, 1);

                        progress.Dispose();
                        btniptal.Dispose();


                    }
                }
            }

            btnTumunuIptalEt.Visible = IndirilenIsyeri.AphbIndirmeleri.Any(p => p.IndirmeSonucu.Tamamlandi == false) || IndirilenIsyeri.BasvuruFormuIndirmeleri.Any(p => p.IndirmeSonucu.Tamamlandi == false);
        }

        void IptalEt(object sender, EventArgs e)
        {
            if ((sender as Button).Tag is AphbIndir)
            {
                ((sender as Button).Tag as AphbIndir).Cancel();
            }
            else if ((sender as Button).Tag is BasvuruFormuIndir)
            {
                ((sender as Button).Tag as BasvuruFormuIndir).IslemiIptalEt();
            }
        }

        void LoglariGoster(object sender, EventArgs e)
        {
            var indir = (sender as Button).Tag as Indir;

            if (indir.FormLog == null || indir.FormLog.IsDisposed)
            {
                if (indir is AphbIndir)
                {
                    var aphbindir = indir as AphbIndir;
                    indir.FormLog = new frmLog(indir.sb);
                    indir.FormLog.Text = String.Format("Aphb İndirme - {0} - {1}", aphbindir.SuanYapilanIsyeriAphb.Sirketler.SirketAdi, aphbindir.SuanYapilanIsyeriAphb.SubeAdi);
                }
                else
                {
                    var bfindir = indir as BasvuruFormuIndir;
                    indir.FormLog = new frmLog(ref bfindir.loglar);
                    indir.FormLog.Text = String.Format("Başvuru Formu İndirme - {0} - {1}  ({2})", bfindir.SuanYapilanIsyeriBasvuru.Sirketler.SirketAdi, bfindir.SuanYapilanIsyeriBasvuru.SubeAdi, bfindir.bfsira.BasvuruFormuAdiGetir());

                }
            }

            indir.FormLog.WindowState = FormWindowState.Maximized;
            indir.FormLog.Show();
            indir.FormLog.Activate();


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

        private void frmIndirmeEkrani_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.frmSirketIndirmeEkrani != null)
            {
                e.Cancel = true;
                this.Hide();
            }

        }

        private void btnTumunuIptalEt_Click(object sender, EventArgs e)
        {
            IndirilenIsyeri.TumunuIptalEt();
            btnTumunuIptalEt.Visible = false;
        }
    }
}
