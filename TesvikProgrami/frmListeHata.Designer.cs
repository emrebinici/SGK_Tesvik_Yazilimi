namespace TesvikProgrami
{
    partial class frmListeHata
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmListeHata));
            this.dgvAylik = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabAphb = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAylik)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabAphb.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvAylik
            // 
            this.dgvAylik.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAylik.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAylik.Location = new System.Drawing.Point(3, 3);
            this.dgvAylik.Margin = new System.Windows.Forms.Padding(2, 20, 2, 2);
            this.dgvAylik.Name = "dgvAylik";
            this.dgvAylik.RowTemplate.Height = 24;
            this.dgvAylik.Size = new System.Drawing.Size(1034, 606);
            this.dgvAylik.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabAphb);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1048, 638);
            this.tabControl1.TabIndex = 3;
            // 
            // tabAphb
            // 
            this.tabAphb.Controls.Add(this.dgvAylik);
            this.tabAphb.Location = new System.Drawing.Point(4, 22);
            this.tabAphb.Name = "tabAphb";
            this.tabAphb.Padding = new System.Windows.Forms.Padding(3);
            this.tabAphb.Size = new System.Drawing.Size(1040, 612);
            this.tabAphb.TabIndex = 0;
            this.tabAphb.Text = "APHB";
            this.tabAphb.UseVisualStyleBackColor = true;
            // 
            // frmListeHata
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TesvikProgrami.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1048, 638);
            this.Controls.Add(this.tabControl1);
            this.Icon = global::TesvikProgrami.Properties.Resources.iconNew;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmListeHata";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hatalı Satırlar";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmListeHata_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAylik)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabAphb.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvAylik;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabAphb;
    }
}