namespace TesvikProgrami
{
    partial class frmSirketler
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.txtSirketAdi = new System.Windows.Forms.TextBox();
            this.lblKisaVadeliSigortaKoluKodu = new System.Windows.Forms.Label();
            this.dgvSirketler = new System.Windows.Forms.DataGridView();
            this.btnKaydet = new System.Windows.Forms.Button();
            this.lblIptal = new System.Windows.Forms.Label();
            this.lblVergiKimlikNo = new System.Windows.Forms.Label();
            this.txtVergiKimlikNo = new System.Windows.Forms.TextBox();
            this.chkPasifleriGoster = new System.Windows.Forms.CheckBox();
            this.chkAktif = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPasifYap = new System.Windows.Forms.Button();
            this.lblAra = new System.Windows.Forms.Label();
            this.txtAra = new System.Windows.Forms.TextBox();
            this.btnTumunuSecKaldir = new System.Windows.Forms.Button();
            this.btnSecilenleriSil = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusSirketSayisi = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnSecilenleriCari14857yeAktar = new System.Windows.Forms.Button();
            this.colSec = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colDuzenle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSirketAdi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVergiKimlikNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGecmisTanimla = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colCariTanimla = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colSil = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSirketler)).BeginInit();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSirketAdi
            // 
            this.txtSirketAdi.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtSirketAdi.Location = new System.Drawing.Point(16, 20);
            this.txtSirketAdi.Margin = new System.Windows.Forms.Padding(2);
            this.txtSirketAdi.Name = "txtSirketAdi";
            this.txtSirketAdi.Size = new System.Drawing.Size(254, 24);
            this.txtSirketAdi.TabIndex = 0;
            // 
            // lblKisaVadeliSigortaKoluKodu
            // 
            this.lblKisaVadeliSigortaKoluKodu.AutoSize = true;
            this.lblKisaVadeliSigortaKoluKodu.BackColor = System.Drawing.Color.Transparent;
            this.lblKisaVadeliSigortaKoluKodu.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblKisaVadeliSigortaKoluKodu.ForeColor = System.Drawing.Color.White;
            this.lblKisaVadeliSigortaKoluKodu.Location = new System.Drawing.Point(14, 0);
            this.lblKisaVadeliSigortaKoluKodu.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblKisaVadeliSigortaKoluKodu.Name = "lblKisaVadeliSigortaKoluKodu";
            this.lblKisaVadeliSigortaKoluKodu.Size = new System.Drawing.Size(79, 17);
            this.lblKisaVadeliSigortaKoluKodu.TabIndex = 1;
            this.lblKisaVadeliSigortaKoluKodu.Text = "Şirket Adı:";
            // 
            // dgvSirketler
            // 
            this.dgvSirketler.AllowUserToAddRows = false;
            this.dgvSirketler.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightBlue;
            this.dgvSirketler.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSirketler.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSirketler.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSirketler.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSirketler.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSec,
            this.colDuzenle,
            this.colSirketAdi,
            this.colVergiKimlikNo,
            this.colGecmisTanimla,
            this.colCariTanimla,
            this.colSil});
            this.dgvSirketler.Location = new System.Drawing.Point(0, 284);
            this.dgvSirketler.Margin = new System.Windows.Forms.Padding(2);
            this.dgvSirketler.Name = "dgvSirketler";
            this.dgvSirketler.ReadOnly = true;
            this.dgvSirketler.RowTemplate.Height = 24;
            this.dgvSirketler.Size = new System.Drawing.Size(941, 389);
            this.dgvSirketler.TabIndex = 4;
            this.dgvSirketler.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSirketler_CellContentClick);
            this.dgvSirketler.SelectionChanged += new System.EventHandler(this.dgvSirketler_SelectionChanged);
            // 
            // btnKaydet
            // 
            this.btnKaydet.BackgroundImage = global::TesvikProgrami.Properties.Resources.Kaydet;
            this.btnKaydet.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnKaydet.Location = new System.Drawing.Point(98, 128);
            this.btnKaydet.Margin = new System.Windows.Forms.Padding(2);
            this.btnKaydet.Name = "btnKaydet";
            this.btnKaydet.Size = new System.Drawing.Size(82, 26);
            this.btnKaydet.TabIndex = 3;
            this.btnKaydet.UseVisualStyleBackColor = true;
            this.btnKaydet.Click += new System.EventHandler(this.btnKaydet_Click);
            // 
            // lblIptal
            // 
            this.lblIptal.AutoSize = true;
            this.lblIptal.BackColor = System.Drawing.Color.Transparent;
            this.lblIptal.Font = new System.Drawing.Font("Tahoma", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblIptal.ForeColor = System.Drawing.Color.White;
            this.lblIptal.Location = new System.Drawing.Point(122, 156);
            this.lblIptal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblIptal.Name = "lblIptal";
            this.lblIptal.Size = new System.Drawing.Size(41, 17);
            this.lblIptal.TabIndex = 4;
            this.lblIptal.Text = "İptal";
            this.lblIptal.Visible = false;
            this.lblIptal.Click += new System.EventHandler(this.lblIptal_Click);
            // 
            // lblVergiKimlikNo
            // 
            this.lblVergiKimlikNo.AutoSize = true;
            this.lblVergiKimlikNo.BackColor = System.Drawing.Color.Transparent;
            this.lblVergiKimlikNo.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblVergiKimlikNo.ForeColor = System.Drawing.Color.White;
            this.lblVergiKimlikNo.Location = new System.Drawing.Point(14, 51);
            this.lblVergiKimlikNo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblVergiKimlikNo.Name = "lblVergiKimlikNo";
            this.lblVergiKimlikNo.Size = new System.Drawing.Size(117, 17);
            this.lblVergiKimlikNo.TabIndex = 8;
            this.lblVergiKimlikNo.Text = "Vergi Kimlik No:";
            // 
            // txtVergiKimlikNo
            // 
            this.txtVergiKimlikNo.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtVergiKimlikNo.Location = new System.Drawing.Point(16, 71);
            this.txtVergiKimlikNo.Margin = new System.Windows.Forms.Padding(2);
            this.txtVergiKimlikNo.Name = "txtVergiKimlikNo";
            this.txtVergiKimlikNo.Size = new System.Drawing.Size(254, 24);
            this.txtVergiKimlikNo.TabIndex = 1;
            // 
            // chkPasifleriGoster
            // 
            this.chkPasifleriGoster.AutoSize = true;
            this.chkPasifleriGoster.BackColor = System.Drawing.Color.Transparent;
            this.chkPasifleriGoster.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.chkPasifleriGoster.ForeColor = System.Drawing.Color.White;
            this.chkPasifleriGoster.Location = new System.Drawing.Point(231, 254);
            this.chkPasifleriGoster.Name = "chkPasifleriGoster";
            this.chkPasifleriGoster.Size = new System.Drawing.Size(129, 21);
            this.chkPasifleriGoster.TabIndex = 9;
            this.chkPasifleriGoster.Text = "Pasifleri Göster";
            this.chkPasifleriGoster.UseVisualStyleBackColor = false;
            this.chkPasifleriGoster.CheckedChanged += new System.EventHandler(this.chkPasifleriGoster_CheckedChanged);
            // 
            // chkAktif
            // 
            this.chkAktif.AutoSize = true;
            this.chkAktif.BackColor = System.Drawing.Color.Transparent;
            this.chkAktif.Checked = true;
            this.chkAktif.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAktif.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.chkAktif.ForeColor = System.Drawing.Color.White;
            this.chkAktif.Location = new System.Drawing.Point(17, 102);
            this.chkAktif.Name = "chkAktif";
            this.chkAktif.Size = new System.Drawing.Size(59, 21);
            this.chkAktif.TabIndex = 2;
            this.chkAktif.Text = "Aktif";
            this.chkAktif.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.txtSirketAdi);
            this.panel1.Controls.Add(this.chkAktif);
            this.panel1.Controls.Add(this.lblKisaVadeliSigortaKoluKodu);
            this.panel1.Controls.Add(this.btnKaydet);
            this.panel1.Controls.Add(this.lblVergiKimlikNo);
            this.panel1.Controls.Add(this.lblIptal);
            this.panel1.Controls.Add(this.txtVergiKimlikNo);
            this.panel1.Location = new System.Drawing.Point(325, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(296, 187);
            this.panel1.TabIndex = 11;
            // 
            // btnPasifYap
            // 
            this.btnPasifYap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPasifYap.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnPasifYap.ForeColor = System.Drawing.Color.Navy;
            this.btnPasifYap.Location = new System.Drawing.Point(4, 248);
            this.btnPasifYap.Name = "btnPasifYap";
            this.btnPasifYap.Size = new System.Drawing.Size(221, 31);
            this.btnPasifYap.TabIndex = 12;
            this.btnPasifYap.Text = "Seçilenleri Pasif Yap";
            this.btnPasifYap.UseVisualStyleBackColor = true;
            this.btnPasifYap.Click += new System.EventHandler(this.btnPasifYap_Click);
            // 
            // lblAra
            // 
            this.lblAra.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAra.AutoSize = true;
            this.lblAra.BackColor = System.Drawing.Color.Transparent;
            this.lblAra.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblAra.ForeColor = System.Drawing.Color.White;
            this.lblAra.Location = new System.Drawing.Point(645, 257);
            this.lblAra.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAra.Name = "lblAra";
            this.lblAra.Size = new System.Drawing.Size(37, 17);
            this.lblAra.TabIndex = 37;
            this.lblAra.Text = "Ara:";
            // 
            // txtAra
            // 
            this.txtAra.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAra.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtAra.Location = new System.Drawing.Point(687, 254);
            this.txtAra.Margin = new System.Windows.Forms.Padding(2);
            this.txtAra.Name = "txtAra";
            this.txtAra.Size = new System.Drawing.Size(249, 24);
            this.txtAra.TabIndex = 5;
            this.txtAra.TextChanged += new System.EventHandler(this.txtAra_TextChanged);
            // 
            // btnTumunuSecKaldir
            // 
            this.btnTumunuSecKaldir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTumunuSecKaldir.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnTumunuSecKaldir.ForeColor = System.Drawing.Color.Navy;
            this.btnTumunuSecKaldir.Location = new System.Drawing.Point(4, 211);
            this.btnTumunuSecKaldir.Name = "btnTumunuSecKaldir";
            this.btnTumunuSecKaldir.Size = new System.Drawing.Size(221, 31);
            this.btnTumunuSecKaldir.TabIndex = 38;
            this.btnTumunuSecKaldir.Text = "Tümünü Seç / Kaldır";
            this.btnTumunuSecKaldir.UseVisualStyleBackColor = true;
            this.btnTumunuSecKaldir.Click += new System.EventHandler(this.btnTumunuSecKaldir_Click);
            // 
            // btnSecilenleriSil
            // 
            this.btnSecilenleriSil.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSecilenleriSil.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnSecilenleriSil.ForeColor = System.Drawing.Color.Navy;
            this.btnSecilenleriSil.Location = new System.Drawing.Point(4, 174);
            this.btnSecilenleriSil.Name = "btnSecilenleriSil";
            this.btnSecilenleriSil.Size = new System.Drawing.Size(221, 31);
            this.btnSecilenleriSil.TabIndex = 39;
            this.btnSecilenleriSil.Text = "Seçilenleri Sil";
            this.btnSecilenleriSil.UseVisualStyleBackColor = true;
            this.btnSecilenleriSil.Click += new System.EventHandler(this.btnSecilenleriSil_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusSirketSayisi});
            this.statusStrip1.Location = new System.Drawing.Point(0, 675);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(942, 22);
            this.statusStrip1.TabIndex = 40;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusSirketSayisi
            // 
            this.statusSirketSayisi.Name = "statusSirketSayisi";
            this.statusSirketSayisi.Size = new System.Drawing.Size(118, 17);
            this.statusSirketSayisi.Text = "toolStripStatusLabel1";
            // 
            // btnSecilenleriCari14857yeAktar
            // 
            this.btnSecilenleriCari14857yeAktar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSecilenleriCari14857yeAktar.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnSecilenleriCari14857yeAktar.ForeColor = System.Drawing.Color.Navy;
            this.btnSecilenleriCari14857yeAktar.Location = new System.Drawing.Point(3, 136);
            this.btnSecilenleriCari14857yeAktar.Name = "btnSecilenleriCari14857yeAktar";
            this.btnSecilenleriCari14857yeAktar.Size = new System.Drawing.Size(222, 31);
            this.btnSecilenleriCari14857yeAktar.TabIndex = 41;
            this.btnSecilenleriCari14857yeAktar.Text = "Cari 14857 ye Aktar";
            this.btnSecilenleriCari14857yeAktar.UseVisualStyleBackColor = true;
            this.btnSecilenleriCari14857yeAktar.Click += new System.EventHandler(this.btnSecilenleriCari14857yeAktar_Click);
            // 
            // colSec
            // 
            this.colSec.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colSec.FillWeight = 30F;
            this.colSec.HeaderText = "Seç";
            this.colSec.Name = "colSec";
            this.colSec.ReadOnly = true;
            this.colSec.Width = 30;
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
            // colSirketAdi
            // 
            this.colSirketAdi.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colSirketAdi.DataPropertyName = "SirketAdi";
            this.colSirketAdi.HeaderText = "Şirket Adı";
            this.colSirketAdi.Name = "colSirketAdi";
            this.colSirketAdi.ReadOnly = true;
            this.colSirketAdi.Width = 77;
            // 
            // colVergiKimlikNo
            // 
            this.colVergiKimlikNo.DataPropertyName = "VergiKimlikNo";
            this.colVergiKimlikNo.HeaderText = "Vergi Kimlik No";
            this.colVergiKimlikNo.Name = "colVergiKimlikNo";
            this.colVergiKimlikNo.ReadOnly = true;
            // 
            // colGecmisTanimla
            // 
            this.colGecmisTanimla.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.NullValue = "Geçmiş Tanımla";
            this.colGecmisTanimla.DefaultCellStyle = dataGridViewCellStyle3;
            this.colGecmisTanimla.HeaderText = "Geçmiş Tanımla";
            this.colGecmisTanimla.MinimumWidth = 100;
            this.colGecmisTanimla.Name = "colGecmisTanimla";
            this.colGecmisTanimla.ReadOnly = true;
            this.colGecmisTanimla.Text = "Cari Tanımla";
            // 
            // colCariTanimla
            // 
            this.colCariTanimla.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.NullValue = "Cari Tanımla";
            this.colCariTanimla.DefaultCellStyle = dataGridViewCellStyle4;
            this.colCariTanimla.HeaderText = "Cari Tanımla";
            this.colCariTanimla.MinimumWidth = 100;
            this.colCariTanimla.Name = "colCariTanimla";
            this.colCariTanimla.ReadOnly = true;
            this.colCariTanimla.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colCariTanimla.Text = "Cari Tanımla";
            // 
            // colSil
            // 
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.NullValue = "Sil";
            this.colSil.DefaultCellStyle = dataGridViewCellStyle5;
            this.colSil.FillWeight = 1F;
            this.colSil.HeaderText = "";
            this.colSil.MinimumWidth = 30;
            this.colSil.Name = "colSil";
            this.colSil.ReadOnly = true;
            // 
            // frmSirketler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TesvikProgrami.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(942, 697);
            this.Controls.Add(this.btnSecilenleriCari14857yeAktar);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnSecilenleriSil);
            this.Controls.Add(this.btnTumunuSecKaldir);
            this.Controls.Add(this.lblAra);
            this.Controls.Add(this.txtAra);
            this.Controls.Add(this.btnPasifYap);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chkPasifleriGoster);
            this.Controls.Add(this.dgvSirketler);
            this.Icon = global::TesvikProgrami.Properties.Resources.iconNew;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmSirketler";
            this.Text = "Şirketler";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSirketler_FormClosing);
            this.Load += new System.EventHandler(this.frmSirketler_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSirketler)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSirketAdi;
        private System.Windows.Forms.Label lblKisaVadeliSigortaKoluKodu;
        private System.Windows.Forms.DataGridView dgvSirketler;
        private System.Windows.Forms.Button btnKaydet;
        private System.Windows.Forms.Label lblIptal;
        private System.Windows.Forms.Label lblVergiKimlikNo;
        private System.Windows.Forms.TextBox txtVergiKimlikNo;
        private System.Windows.Forms.CheckBox chkPasifleriGoster;
        private System.Windows.Forms.CheckBox chkAktif;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnPasifYap;
        private System.Windows.Forms.Label lblAra;
        private System.Windows.Forms.TextBox txtAra;
        private System.Windows.Forms.Button btnTumunuSecKaldir;
        private System.Windows.Forms.Button btnSecilenleriSil;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusSirketSayisi;
        private System.Windows.Forms.Button btnSecilenleriCari14857yeAktar;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSec;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDuzenle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSirketAdi;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVergiKimlikNo;
        private System.Windows.Forms.DataGridViewButtonColumn colGecmisTanimla;
        private System.Windows.Forms.DataGridViewButtonColumn colCariTanimla;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSil;
    }
}