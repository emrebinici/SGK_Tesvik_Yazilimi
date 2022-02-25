namespace TesvikProgrami
{
    partial class frmIndirmeEkrani
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
            this.flowpnlIndirmeler = new System.Windows.Forms.FlowLayoutPanel();
            this.btnTumunuIptalEt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // flowpnlIndirmeler
            // 
            this.flowpnlIndirmeler.BackColor = System.Drawing.Color.Transparent;
            this.flowpnlIndirmeler.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowpnlIndirmeler.Location = new System.Drawing.Point(45, 43);
            this.flowpnlIndirmeler.Name = "flowpnlIndirmeler";
            this.flowpnlIndirmeler.Size = new System.Drawing.Size(835, 523);
            this.flowpnlIndirmeler.TabIndex = 0;
            // 
            // btnTumunuIptalEt
            // 
            this.btnTumunuIptalEt.BackColor = System.Drawing.Color.DarkRed;
            this.btnTumunuIptalEt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTumunuIptalEt.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnTumunuIptalEt.ForeColor = System.Drawing.Color.White;
            this.btnTumunuIptalEt.Location = new System.Drawing.Point(680, 582);
            this.btnTumunuIptalEt.Margin = new System.Windows.Forms.Padding(2);
            this.btnTumunuIptalEt.Name = "btnTumunuIptalEt";
            this.btnTumunuIptalEt.Size = new System.Drawing.Size(200, 27);
            this.btnTumunuIptalEt.TabIndex = 41;
            this.btnTumunuIptalEt.Text = "Tümünü İptal Et";
            this.btnTumunuIptalEt.UseVisualStyleBackColor = false;
            this.btnTumunuIptalEt.Click += new System.EventHandler(this.btnTumunuIptalEt_Click);
            // 
            // frmIndirmeEkrani
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TesvikProgrami.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(918, 620);
            this.Controls.Add(this.btnTumunuIptalEt);
            this.Controls.Add(this.flowpnlIndirmeler);
            this.Icon = global::TesvikProgrami.Properties.Resources.iconNew;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "frmIndirmeEkrani";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "İndirme Ekranı";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmIndirmeEkrani_FormClosing);
            this.Shown += new System.EventHandler(this.frmIndirmeEkrani_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowpnlIndirmeler;
        private System.Windows.Forms.Button btnTumunuIptalEt;
    }
}