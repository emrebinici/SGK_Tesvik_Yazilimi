namespace TesvikProgrami
{
    partial class frmAsgariUcretler
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAsgariUcretler));
            this.lblDonemBaslangic = new System.Windows.Forms.Label();
            this.lblDonemBitis = new System.Windows.Forms.Label();
            this.dgvAsgariUcretler = new System.Windows.Forms.DataGridView();
            this.colDuzenle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBaslangic = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBitis = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAsgariUcret = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSil = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnKaydet = new System.Windows.Forms.Button();
            this.lblIptal = new System.Windows.Forms.Label();
            this.lblAsgariUcret = new System.Windows.Forms.Label();
            this.txtAsgariUcret = new System.Windows.Forms.TextBox();
            this.dtpBaslangic = new System.Windows.Forms.DateTimePicker();
            this.dtpBitis = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsgariUcretler)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDonemBaslangic
            // 
            this.lblDonemBaslangic.AutoSize = true;
            this.lblDonemBaslangic.BackColor = System.Drawing.Color.Transparent;
            this.lblDonemBaslangic.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblDonemBaslangic.ForeColor = System.Drawing.Color.White;
            this.lblDonemBaslangic.Location = new System.Drawing.Point(305, 16);
            this.lblDonemBaslangic.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDonemBaslangic.Name = "lblDonemBaslangic";
            this.lblDonemBaslangic.Size = new System.Drawing.Size(161, 17);
            this.lblDonemBaslangic.TabIndex = 1;
            this.lblDonemBaslangic.Text = "Dönem Başlangıç: (*)";
            // 
            // lblDonemBitis
            // 
            this.lblDonemBitis.AutoSize = true;
            this.lblDonemBitis.BackColor = System.Drawing.Color.Transparent;
            this.lblDonemBitis.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblDonemBitis.ForeColor = System.Drawing.Color.White;
            this.lblDonemBitis.Location = new System.Drawing.Point(327, 65);
            this.lblDonemBitis.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDonemBitis.Name = "lblDonemBitis";
            this.lblDonemBitis.Size = new System.Drawing.Size(117, 17);
            this.lblDonemBitis.TabIndex = 3;
            this.lblDonemBitis.Text = "Dönem Bitiş (*)";
            // 
            // dgvAsgariUcretler
            // 
            this.dgvAsgariUcretler.AllowUserToAddRows = false;
            this.dgvAsgariUcretler.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightBlue;
            this.dgvAsgariUcretler.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAsgariUcretler.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAsgariUcretler.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAsgariUcretler.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDuzenle,
            this.colBaslangic,
            this.colBitis,
            this.colAsgariUcret,
            this.colSil});
            this.dgvAsgariUcretler.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgvAsgariUcretler.Location = new System.Drawing.Point(0, 217);
            this.dgvAsgariUcretler.Margin = new System.Windows.Forms.Padding(2);
            this.dgvAsgariUcretler.Name = "dgvAsgariUcretler";
            this.dgvAsgariUcretler.ReadOnly = true;
            this.dgvAsgariUcretler.RowTemplate.Height = 24;
            this.dgvAsgariUcretler.Size = new System.Drawing.Size(754, 370);
            this.dgvAsgariUcretler.TabIndex = 4;
            this.dgvAsgariUcretler.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBelgeTurleri_CellContentClick);
            this.dgvAsgariUcretler.SelectionChanged += new System.EventHandler(this.dgvBelgeTurleri_SelectionChanged);
            // 
            // colDuzenle
            // 
            this.colDuzenle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.NullValue = "Düzenle";
            this.colDuzenle.DefaultCellStyle = dataGridViewCellStyle2;
            this.colDuzenle.HeaderText = "";
            this.colDuzenle.Name = "colDuzenle";
            this.colDuzenle.ReadOnly = true;
            this.colDuzenle.Width = 19;
            // 
            // colBaslangic
            // 
            this.colBaslangic.DataPropertyName = "Baslangic";
            this.colBaslangic.HeaderText = "Başlangıç";
            this.colBaslangic.Name = "colBaslangic";
            this.colBaslangic.ReadOnly = true;
            // 
            // colBitis
            // 
            this.colBitis.DataPropertyName = "Bitis";
            this.colBitis.HeaderText = "Bitiş";
            this.colBitis.Name = "colBitis";
            this.colBitis.ReadOnly = true;
            // 
            // colAsgariUcret
            // 
            this.colAsgariUcret.DataPropertyName = "AsgariUcretTutari";
            this.colAsgariUcret.HeaderText = "Asgari Ücret";
            this.colAsgariUcret.Name = "colAsgariUcret";
            this.colAsgariUcret.ReadOnly = true;
            // 
            // colSil
            // 
            this.colSil.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.NullValue = "Sil";
            this.colSil.DefaultCellStyle = dataGridViewCellStyle3;
            this.colSil.HeaderText = "";
            this.colSil.Name = "colSil";
            this.colSil.ReadOnly = true;
            this.colSil.Width = 19;
            // 
            // btnKaydet
            // 
            this.btnKaydet.BackgroundImage = global::TesvikProgrami.Properties.Resources.Kaydet;
            this.btnKaydet.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnKaydet.Location = new System.Drawing.Point(344, 157);
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
            this.lblIptal.Location = new System.Drawing.Point(366, 187);
            this.lblIptal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblIptal.Name = "lblIptal";
            this.lblIptal.Size = new System.Drawing.Size(39, 17);
            this.lblIptal.TabIndex = 6;
            this.lblIptal.Text = "İptal";
            this.lblIptal.Visible = false;
            this.lblIptal.Click += new System.EventHandler(this.lblIptal_Click);
            // 
            // lblAsgariUcret
            // 
            this.lblAsgariUcret.AutoSize = true;
            this.lblAsgariUcret.BackColor = System.Drawing.Color.Transparent;
            this.lblAsgariUcret.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblAsgariUcret.ForeColor = System.Drawing.Color.White;
            this.lblAsgariUcret.Location = new System.Drawing.Point(325, 107);
            this.lblAsgariUcret.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAsgariUcret.Name = "lblAsgariUcret";
            this.lblAsgariUcret.Size = new System.Drawing.Size(121, 17);
            this.lblAsgariUcret.TabIndex = 8;
            this.lblAsgariUcret.Text = "Asgari Ücret:(*)";
            // 
            // txtAsgariUcret
            // 
            this.txtAsgariUcret.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtAsgariUcret.Location = new System.Drawing.Point(289, 127);
            this.txtAsgariUcret.Margin = new System.Windows.Forms.Padding(2);
            this.txtAsgariUcret.Name = "txtAsgariUcret";
            this.txtAsgariUcret.Size = new System.Drawing.Size(193, 23);
            this.txtAsgariUcret.TabIndex = 7;
            // 
            // dtpBaslangic
            // 
            this.dtpBaslangic.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.dtpBaslangic.Location = new System.Drawing.Point(288, 36);
            this.dtpBaslangic.Margin = new System.Windows.Forms.Padding(2);
            this.dtpBaslangic.Name = "dtpBaslangic";
            this.dtpBaslangic.Size = new System.Drawing.Size(194, 23);
            this.dtpBaslangic.TabIndex = 9;
            // 
            // dtpBitis
            // 
            this.dtpBitis.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.dtpBitis.Location = new System.Drawing.Point(288, 83);
            this.dtpBitis.Margin = new System.Windows.Forms.Padding(2);
            this.dtpBitis.Name = "dtpBitis";
            this.dtpBitis.Size = new System.Drawing.Size(194, 23);
            this.dtpBitis.TabIndex = 10;
            // 
            // frmAsgariUcretler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TesvikProgrami.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(754, 587);
            this.Controls.Add(this.dtpBitis);
            this.Controls.Add(this.dtpBaslangic);
            this.Controls.Add(this.lblAsgariUcret);
            this.Controls.Add(this.txtAsgariUcret);
            this.Controls.Add(this.lblIptal);
            this.Controls.Add(this.btnKaydet);
            this.Controls.Add(this.dgvAsgariUcretler);
            this.Controls.Add(this.lblDonemBitis);
            this.Controls.Add(this.lblDonemBaslangic);
            this.Icon = global::TesvikProgrami.Properties.Resources.iconNew;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmAsgariUcretler";
            this.Text = "Asgari Ücretler";
            this.Load += new System.EventHandler(this.frmAsgariUcretler_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsgariUcretler)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblDonemBaslangic;
        private System.Windows.Forms.Label lblDonemBitis;
        private System.Windows.Forms.DataGridView dgvAsgariUcretler;
        private System.Windows.Forms.Button btnKaydet;
        private System.Windows.Forms.Label lblIptal;
        private System.Windows.Forms.Label lblAsgariUcret;
        private System.Windows.Forms.TextBox txtAsgariUcret;
        private System.Windows.Forms.DateTimePicker dtpBaslangic;
        private System.Windows.Forms.DateTimePicker dtpBitis;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDuzenle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBaslangic;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitis;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAsgariUcret;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSil;
    }
}