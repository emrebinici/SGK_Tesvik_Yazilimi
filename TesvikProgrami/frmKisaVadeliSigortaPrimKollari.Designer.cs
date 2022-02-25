namespace TesvikProgrami
{
    partial class frmKisaVadeliSigortaPrimKollari
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmKisaVadeliSigortaPrimKollari));
            this.txtKisaVadeliSigortaKoluKodu = new System.Windows.Forms.TextBox();
            this.lblKisaVadeliSigortaKoluKodu = new System.Windows.Forms.Label();
            this.lblPrimOrani = new System.Windows.Forms.Label();
            this.txtPrimOrani = new System.Windows.Forms.TextBox();
            this.dgvKisaVadeliSigortaKollari = new System.Windows.Forms.DataGridView();
            this.colDuzenle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colKisaVadeliSigortaKoluKodu = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrimOrani = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSil = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnKaydet = new System.Windows.Forms.Button();
            this.lblIptal = new System.Windows.Forms.Label();
            this.txtAra = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKisaVadeliSigortaKollari)).BeginInit();
            this.SuspendLayout();
            // 
            // txtKisaVadeliSigortaKoluKodu
            // 
            this.txtKisaVadeliSigortaKoluKodu.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtKisaVadeliSigortaKoluKodu.Location = new System.Drawing.Point(228, 42);
            this.txtKisaVadeliSigortaKoluKodu.Margin = new System.Windows.Forms.Padding(2);
            this.txtKisaVadeliSigortaKoluKodu.Name = "txtKisaVadeliSigortaKoluKodu";
            this.txtKisaVadeliSigortaKoluKodu.Size = new System.Drawing.Size(254, 23);
            this.txtKisaVadeliSigortaKoluKodu.TabIndex = 0;
            // 
            // lblKisaVadeliSigortaKoluKodu
            // 
            this.lblKisaVadeliSigortaKoluKodu.AutoSize = true;
            this.lblKisaVadeliSigortaKoluKodu.BackColor = System.Drawing.Color.Transparent;
            this.lblKisaVadeliSigortaKoluKodu.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblKisaVadeliSigortaKoluKodu.ForeColor = System.Drawing.Color.White;
            this.lblKisaVadeliSigortaKoluKodu.Location = new System.Drawing.Point(226, 22);
            this.lblKisaVadeliSigortaKoluKodu.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblKisaVadeliSigortaKoluKodu.Name = "lblKisaVadeliSigortaKoluKodu";
            this.lblKisaVadeliSigortaKoluKodu.Size = new System.Drawing.Size(253, 17);
            this.lblKisaVadeliSigortaKoluKodu.TabIndex = 1;
            this.lblKisaVadeliSigortaKoluKodu.Text = "Kısa Vadeli Sigorta Kolu Kodu: (*)";
            // 
            // lblPrimOrani
            // 
            this.lblPrimOrani.AutoSize = true;
            this.lblPrimOrani.BackColor = System.Drawing.Color.Transparent;
            this.lblPrimOrani.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblPrimOrani.ForeColor = System.Drawing.Color.White;
            this.lblPrimOrani.Location = new System.Drawing.Point(226, 80);
            this.lblPrimOrani.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPrimOrani.Name = "lblPrimOrani";
            this.lblPrimOrani.Size = new System.Drawing.Size(113, 17);
            this.lblPrimOrani.TabIndex = 3;
            this.lblPrimOrani.Text = "Prim Oranı: (*)";
            // 
            // txtPrimOrani
            // 
            this.txtPrimOrani.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtPrimOrani.Location = new System.Drawing.Point(228, 100);
            this.txtPrimOrani.Margin = new System.Windows.Forms.Padding(2);
            this.txtPrimOrani.Name = "txtPrimOrani";
            this.txtPrimOrani.Size = new System.Drawing.Size(254, 23);
            this.txtPrimOrani.TabIndex = 2;
            // 
            // dgvKisaVadeliSigortaKollari
            // 
            this.dgvKisaVadeliSigortaKollari.AllowUserToAddRows = false;
            this.dgvKisaVadeliSigortaKollari.AllowUserToDeleteRows = false;
            this.dgvKisaVadeliSigortaKollari.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvKisaVadeliSigortaKollari.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvKisaVadeliSigortaKollari.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDuzenle,
            this.colKisaVadeliSigortaKoluKodu,
            this.colPrimOrani,
            this.colSil});
            this.dgvKisaVadeliSigortaKollari.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgvKisaVadeliSigortaKollari.Location = new System.Drawing.Point(0, 217);
            this.dgvKisaVadeliSigortaKollari.Margin = new System.Windows.Forms.Padding(2);
            this.dgvKisaVadeliSigortaKollari.Name = "dgvKisaVadeliSigortaKollari";
            this.dgvKisaVadeliSigortaKollari.ReadOnly = true;
            this.dgvKisaVadeliSigortaKollari.RowTemplate.Height = 24;
            this.dgvKisaVadeliSigortaKollari.Size = new System.Drawing.Size(754, 370);
            this.dgvKisaVadeliSigortaKollari.TabIndex = 4;
            this.dgvKisaVadeliSigortaKollari.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvKisaVadeliSigortaKollari_CellContentClick);
            this.dgvKisaVadeliSigortaKollari.SelectionChanged += new System.EventHandler(this.dgvKisaVadeliSigortaKollari_SelectionChanged);
            // 
            // colDuzenle
            // 
            this.colDuzenle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.NullValue = "Düzenle";
            this.colDuzenle.DefaultCellStyle = dataGridViewCellStyle1;
            this.colDuzenle.HeaderText = "";
            this.colDuzenle.Name = "colDuzenle";
            this.colDuzenle.ReadOnly = true;
            this.colDuzenle.Width = 19;
            // 
            // colKisaVadeliSigortaKoluKodu
            // 
            this.colKisaVadeliSigortaKoluKodu.DataPropertyName = "KisaVadeliSigortaKoluKodu";
            this.colKisaVadeliSigortaKoluKodu.HeaderText = "Kısa Vadeli Sigorta Kolu Kodu";
            this.colKisaVadeliSigortaKoluKodu.Name = "colKisaVadeliSigortaKoluKodu";
            this.colKisaVadeliSigortaKoluKodu.ReadOnly = true;
            // 
            // colPrimOrani
            // 
            this.colPrimOrani.DataPropertyName = "PrimOrani";
            this.colPrimOrani.HeaderText = "Prim Oranı";
            this.colPrimOrani.Name = "colPrimOrani";
            this.colPrimOrani.ReadOnly = true;
            // 
            // colSil
            // 
            this.colSil.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.NullValue = "Sil";
            this.colSil.DefaultCellStyle = dataGridViewCellStyle2;
            this.colSil.HeaderText = "";
            this.colSil.Name = "colSil";
            this.colSil.ReadOnly = true;
            this.colSil.Width = 19;
            // 
            // btnKaydet
            // 
            this.btnKaydet.BackgroundImage = global::TesvikProgrami.Properties.Resources.Kaydet;
            this.btnKaydet.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnKaydet.Location = new System.Drawing.Point(284, 140);
            this.btnKaydet.Margin = new System.Windows.Forms.Padding(2);
            this.btnKaydet.Name = "btnKaydet";
            this.btnKaydet.Size = new System.Drawing.Size(82, 26);
            this.btnKaydet.TabIndex = 5;
            this.btnKaydet.UseVisualStyleBackColor = true;
            this.btnKaydet.Click += new System.EventHandler(this.btnKaydet_Click);
            // 
            // lblIptal
            // 
            this.lblIptal.AutoSize = true;
            this.lblIptal.BackColor = System.Drawing.Color.Transparent;
            this.lblIptal.Font = new System.Drawing.Font("Tahoma", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblIptal.ForeColor = System.Drawing.Color.White;
            this.lblIptal.Location = new System.Drawing.Point(308, 176);
            this.lblIptal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblIptal.Name = "lblIptal";
            this.lblIptal.Size = new System.Drawing.Size(39, 17);
            this.lblIptal.TabIndex = 6;
            this.lblIptal.Text = "İptal";
            this.lblIptal.Visible = false;
            this.lblIptal.Click += new System.EventHandler(this.lblIptal_Click);
            // 
            // txtAra
            // 
            this.txtAra.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtAra.ForeColor = System.Drawing.Color.DarkGray;
            this.txtAra.Location = new System.Drawing.Point(0, 197);
            this.txtAra.Margin = new System.Windows.Forms.Padding(2);
            this.txtAra.Name = "txtAra";
            this.txtAra.Size = new System.Drawing.Size(68, 23);
            this.txtAra.TabIndex = 7;
            this.txtAra.Text = "Ara";
            this.txtAra.TextChanged += new System.EventHandler(this.txtAra_TextChanged);
            this.txtAra.Enter += new System.EventHandler(this.txtAra_Enter);
            this.txtAra.Leave += new System.EventHandler(this.txtAra_Leave);
            // 
            // frmKisaVadeliSigortaPrimKollari
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TesvikProgrami.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(754, 587);
            this.Controls.Add(this.txtAra);
            this.Controls.Add(this.lblIptal);
            this.Controls.Add(this.btnKaydet);
            this.Controls.Add(this.dgvKisaVadeliSigortaKollari);
            this.Controls.Add(this.lblPrimOrani);
            this.Controls.Add(this.txtPrimOrani);
            this.Controls.Add(this.lblKisaVadeliSigortaKoluKodu);
            this.Controls.Add(this.txtKisaVadeliSigortaKoluKodu);
            this.Icon = global::TesvikProgrami.Properties.Resources.iconNew;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmKisaVadeliSigortaPrimKollari";
            this.Text = "Sigorta Kolları Prim Oranları";
            this.Load += new System.EventHandler(this.frmKisaVadeliSigortaPrimKollari_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvKisaVadeliSigortaKollari)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtKisaVadeliSigortaKoluKodu;
        private System.Windows.Forms.Label lblKisaVadeliSigortaKoluKodu;
        private System.Windows.Forms.Label lblPrimOrani;
        private System.Windows.Forms.TextBox txtPrimOrani;
        private System.Windows.Forms.DataGridView dgvKisaVadeliSigortaKollari;
        private System.Windows.Forms.Button btnKaydet;
        private System.Windows.Forms.Label lblIptal;
        private System.Windows.Forms.TextBox txtAra;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDuzenle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colKisaVadeliSigortaKoluKodu;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrimOrani;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSil;
    }
}