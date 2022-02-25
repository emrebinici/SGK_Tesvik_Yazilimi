namespace TesvikProgrami
{
    partial class frmSirketIndirmeEkrani
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
            this.flowpnlIsyerleri = new System.Windows.Forms.FlowLayoutPanel();
            this.btnTumunuIptalEt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // flowpnlIsyerleri
            // 
            this.flowpnlIsyerleri.AutoScroll = true;
            this.flowpnlIsyerleri.BackColor = System.Drawing.Color.Transparent;
            this.flowpnlIsyerleri.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowpnlIsyerleri.Location = new System.Drawing.Point(45, 57);
            this.flowpnlIsyerleri.Name = "flowpnlIsyerleri";
            this.flowpnlIsyerleri.Size = new System.Drawing.Size(645, 549);
            this.flowpnlIsyerleri.TabIndex = 0;
            this.flowpnlIsyerleri.WrapContents = false;
            // 
            // btnTumunuIptalEt
            // 
            this.btnTumunuIptalEt.BackColor = System.Drawing.Color.DarkRed;
            this.btnTumunuIptalEt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTumunuIptalEt.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnTumunuIptalEt.ForeColor = System.Drawing.Color.White;
            this.btnTumunuIptalEt.Location = new System.Drawing.Point(44, 11);
            this.btnTumunuIptalEt.Margin = new System.Windows.Forms.Padding(2);
            this.btnTumunuIptalEt.Name = "btnTumunuIptalEt";
            this.btnTumunuIptalEt.Size = new System.Drawing.Size(200, 27);
            this.btnTumunuIptalEt.TabIndex = 40;
            this.btnTumunuIptalEt.Text = "Tümünü İptal Et";
            this.btnTumunuIptalEt.UseVisualStyleBackColor = false;
            this.btnTumunuIptalEt.Click += new System.EventHandler(this.btnTumunuIptalEt_Click);
            // 
            // frmSirketIndirmeEkrani
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TesvikProgrami.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(715, 620);
            this.Controls.Add(this.btnTumunuIptalEt);
            this.Controls.Add(this.flowpnlIsyerleri);
            this.Icon = global::TesvikProgrami.Properties.Resources.iconNew;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "frmSirketIndirmeEkrani";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Şirket İndirme Ekranı";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSirketIndirmeEkrani_FormClosing);
            this.Shown += new System.EventHandler(this.frmIndirmeEkrani_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowpnlIsyerleri;
        private System.Windows.Forms.Button btnTumunuIptalEt;
    }
}