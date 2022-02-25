namespace TesvikProgrami
{
    partial class Captcha
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
            this.txtCaptcha = new System.Windows.Forms.TextBox();
            this.picCaptcha = new System.Windows.Forms.PictureBox();
            this.btnYenile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picCaptcha)).BeginInit();
            this.SuspendLayout();
            // 
            // txtCaptcha
            // 
            this.txtCaptcha.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCaptcha.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtCaptcha.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtCaptcha.Location = new System.Drawing.Point(0, 140);
            this.txtCaptcha.Name = "txtCaptcha";
            this.txtCaptcha.Size = new System.Drawing.Size(266, 40);
            this.txtCaptcha.TabIndex = 1;
            this.txtCaptcha.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCaptcha_KeyPress);
            this.txtCaptcha.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtCaptcha_KeyUp);
            // 
            // picCaptcha
            // 
            this.picCaptcha.Dock = System.Windows.Forms.DockStyle.Top;
            this.picCaptcha.Location = new System.Drawing.Point(0, 0);
            this.picCaptcha.Name = "picCaptcha";
            this.picCaptcha.Size = new System.Drawing.Size(266, 94);
            this.picCaptcha.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picCaptcha.TabIndex = 0;
            this.picCaptcha.TabStop = false;
            // 
            // btnYenile
            // 
            this.btnYenile.BackColor = System.Drawing.Color.DarkCyan;
            this.btnYenile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnYenile.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnYenile.ForeColor = System.Drawing.Color.White;
            this.btnYenile.Location = new System.Drawing.Point(0, 100);
            this.btnYenile.Name = "btnYenile";
            this.btnYenile.Size = new System.Drawing.Size(266, 34);
            this.btnYenile.TabIndex = 2;
            this.btnYenile.Text = "YENİLE";
            this.btnYenile.UseVisualStyleBackColor = false;
            this.btnYenile.Click += new System.EventHandler(this.BtnYenile_Click);
            // 
            // Captcha
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 180);
            this.Controls.Add(this.btnYenile);
            this.Controls.Add(this.txtCaptcha);
            this.Controls.Add(this.picCaptcha);
            this.Name = "Captcha";
            this.Text = "Güvenlik Kodu Girişi";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Captcha_FormClosing);
            this.Load += new System.EventHandler(this.Captcha_Load);
            this.Shown += new System.EventHandler(this.Captcha_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.picCaptcha)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picCaptcha;
        private System.Windows.Forms.TextBox txtCaptcha;
        private System.Windows.Forms.Button btnYenile;
    }
}