namespace TesvikProgrami
{
    partial class frmOnay
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
            this.btnYoksay = new System.Windows.Forms.Button();
            this.btnIptal = new System.Windows.Forms.Button();
            this.txtMesaj = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnYoksay
            // 
            this.btnYoksay.BackColor = System.Drawing.Color.DarkRed;
            this.btnYoksay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnYoksay.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnYoksay.ForeColor = System.Drawing.Color.White;
            this.btnYoksay.Location = new System.Drawing.Point(23, 26);
            this.btnYoksay.Margin = new System.Windows.Forms.Padding(2);
            this.btnYoksay.Name = "btnYoksay";
            this.btnYoksay.Size = new System.Drawing.Size(188, 45);
            this.btnYoksay.TabIndex = 1;
            this.btnYoksay.Text = "Devam Et";
            this.btnYoksay.UseVisualStyleBackColor = false;
            this.btnYoksay.Click += new System.EventHandler(this.btnYoksay_Click);
            // 
            // btnIptal
            // 
            this.btnIptal.BackColor = System.Drawing.Color.DarkRed;
            this.btnIptal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIptal.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnIptal.ForeColor = System.Drawing.Color.White;
            this.btnIptal.Location = new System.Drawing.Point(254, 26);
            this.btnIptal.Margin = new System.Windows.Forms.Padding(2);
            this.btnIptal.Name = "btnIptal";
            this.btnIptal.Size = new System.Drawing.Size(188, 45);
            this.btnIptal.TabIndex = 2;
            this.btnIptal.Text = "İptal";
            this.btnIptal.UseVisualStyleBackColor = false;
            this.btnIptal.Click += new System.EventHandler(this.btnIptal_Click);
            // 
            // txtMesaj
            // 
            this.txtMesaj.BackColor = System.Drawing.Color.Snow;
            this.txtMesaj.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtMesaj.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtMesaj.Location = new System.Drawing.Point(0, 92);
            this.txtMesaj.Name = "txtMesaj";
            this.txtMesaj.Size = new System.Drawing.Size(1047, 495);
            this.txtMesaj.TabIndex = 4;
            this.txtMesaj.Text = "";
            // 
            // frmOnay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TesvikProgrami.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1047, 587);
            this.Controls.Add(this.txtMesaj);
            this.Controls.Add(this.btnIptal);
            this.Controls.Add(this.btnYoksay);
            this.Icon = global::TesvikProgrami.Properties.Resources.iconNew;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmOnay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Uyarı";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmOnay_FormClosing);
            this.Load += new System.EventHandler(this.frmMesaj_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnYoksay;
        private System.Windows.Forms.Button btnIptal;
        private System.Windows.Forms.RichTextBox txtMesaj;
    }
}