namespace TesvikProgrami
{
    partial class frmIsyerleriSec
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lnklblDevam = new System.Windows.Forms.LinkLabel();
            this.chkTumunuSecKaldir = new System.Windows.Forms.CheckBox();
            this.flowpnlIsyerleri = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // lnklblDevam
            // 
            this.lnklblDevam.AutoSize = true;
            this.lnklblDevam.BackColor = System.Drawing.Color.Transparent;
            this.lnklblDevam.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lnklblDevam.LinkColor = System.Drawing.Color.White;
            this.lnklblDevam.Location = new System.Drawing.Point(350, 712);
            this.lnklblDevam.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lnklblDevam.Name = "lnklblDevam";
            this.lnklblDevam.Size = new System.Drawing.Size(219, 33);
            this.lnklblDevam.TabIndex = 17;
            this.lnklblDevam.TabStop = true;
            this.lnklblDevam.Text = "Devam Et >>>";
            this.lnklblDevam.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklblDevam_LinkClicked);
            // 
            // chkTumunuSecKaldir
            // 
            this.chkTumunuSecKaldir.AutoSize = true;
            this.chkTumunuSecKaldir.BackColor = System.Drawing.Color.Transparent;
            this.chkTumunuSecKaldir.Checked = true;
            this.chkTumunuSecKaldir.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTumunuSecKaldir.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.chkTumunuSecKaldir.ForeColor = System.Drawing.Color.White;
            this.chkTumunuSecKaldir.Location = new System.Drawing.Point(25, 31);
            this.chkTumunuSecKaldir.Name = "chkTumunuSecKaldir";
            this.chkTumunuSecKaldir.Size = new System.Drawing.Size(238, 29);
            this.chkTumunuSecKaldir.TabIndex = 19;
            this.chkTumunuSecKaldir.Text = "Tümünü Seç/ Kaldır";
            this.chkTumunuSecKaldir.UseVisualStyleBackColor = false;
            this.chkTumunuSecKaldir.Click += new System.EventHandler(this.chkTumunuSecKaldir_Click);
            // 
            // flowpnlIsyerleri
            // 
            this.flowpnlIsyerleri.AutoScroll = true;
            this.flowpnlIsyerleri.BackColor = System.Drawing.Color.Transparent;
            this.flowpnlIsyerleri.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowpnlIsyerleri.Location = new System.Drawing.Point(25, 86);
            this.flowpnlIsyerleri.Name = "flowpnlIsyerleri";
            this.flowpnlIsyerleri.Size = new System.Drawing.Size(580, 563);
            this.flowpnlIsyerleri.TabIndex = 20;
            this.flowpnlIsyerleri.WrapContents = false;
            // 
            // frmIsyerleriSec
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TesvikProgrami.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(656, 792);
            this.Controls.Add(this.flowpnlIsyerleri);
            this.Controls.Add(this.chkTumunuSecKaldir);
            this.Controls.Add(this.lnklblDevam);
            this.Icon = global::TesvikProgrami.Properties.Resources.iconNew;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "frmIsyerleriSec";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "İşyerlerini Seç";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_FormClosing);
            this.Load += new System.EventHandler(this.frm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.LinkLabel lnklblDevam;
        private System.Windows.Forms.CheckBox chkTumunuSecKaldir;
        private System.Windows.Forms.FlowLayoutPanel flowpnlIsyerleri;
    }
}