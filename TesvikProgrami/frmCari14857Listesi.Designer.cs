namespace TesvikProgrami
{
    partial class frmCari14857Listesi
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
            this.dgvSirketler = new System.Windows.Forms.DataGridView();
            this.lblAra = new System.Windows.Forms.Label();
            this.txtAra = new System.Windows.Forms.TextBox();
            this.btnTumunuSecKaldir = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusSirketSayisi = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnSecilenleriCari14857ListesindenCikar = new System.Windows.Forms.Button();
            this.colSec = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colSirketAdi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVergiKimlikNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colListedenCikar = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSirketler)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
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
            this.colSirketAdi,
            this.colVergiKimlikNo,
            this.colListedenCikar});
            this.dgvSirketler.Location = new System.Drawing.Point(0, 108);
            this.dgvSirketler.Margin = new System.Windows.Forms.Padding(2);
            this.dgvSirketler.Name = "dgvSirketler";
            this.dgvSirketler.ReadOnly = true;
            this.dgvSirketler.RowTemplate.Height = 24;
            this.dgvSirketler.Size = new System.Drawing.Size(941, 565);
            this.dgvSirketler.TabIndex = 4;
            this.dgvSirketler.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSirketler_CellContentClick);
            this.dgvSirketler.SelectionChanged += new System.EventHandler(this.dgvSirketler_SelectionChanged);
            // 
            // lblAra
            // 
            this.lblAra.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAra.AutoSize = true;
            this.lblAra.BackColor = System.Drawing.Color.Transparent;
            this.lblAra.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblAra.ForeColor = System.Drawing.Color.White;
            this.lblAra.Location = new System.Drawing.Point(640, 68);
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
            this.txtAra.Location = new System.Drawing.Point(682, 65);
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
            this.btnTumunuSecKaldir.Location = new System.Drawing.Point(12, 21);
            this.btnTumunuSecKaldir.Name = "btnTumunuSecKaldir";
            this.btnTumunuSecKaldir.Size = new System.Drawing.Size(282, 31);
            this.btnTumunuSecKaldir.TabIndex = 38;
            this.btnTumunuSecKaldir.Text = "Tümünü Seç / Kaldır";
            this.btnTumunuSecKaldir.UseVisualStyleBackColor = true;
            this.btnTumunuSecKaldir.Click += new System.EventHandler(this.btnTumunuSecKaldir_Click);
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
            // btnSecilenleriCari14857ListesindenCikar
            // 
            this.btnSecilenleriCari14857ListesindenCikar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSecilenleriCari14857ListesindenCikar.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnSecilenleriCari14857ListesindenCikar.ForeColor = System.Drawing.Color.Navy;
            this.btnSecilenleriCari14857ListesindenCikar.Location = new System.Drawing.Point(12, 58);
            this.btnSecilenleriCari14857ListesindenCikar.Name = "btnSecilenleriCari14857ListesindenCikar";
            this.btnSecilenleriCari14857ListesindenCikar.Size = new System.Drawing.Size(282, 31);
            this.btnSecilenleriCari14857ListesindenCikar.TabIndex = 41;
            this.btnSecilenleriCari14857ListesindenCikar.Text = "Seçilenleri Cari 14857 Listesinden Sil";
            this.btnSecilenleriCari14857ListesindenCikar.UseVisualStyleBackColor = true;
            this.btnSecilenleriCari14857ListesindenCikar.Click += new System.EventHandler(this.btnSecilenleriCari14857ListesindenCikar_Click);
            // 
            // colSec
            // 
            this.colSec.FillWeight = 30F;
            this.colSec.HeaderText = "Seç";
            this.colSec.Name = "colSec";
            this.colSec.ReadOnly = true;
            // 
            // colSirketAdi
            // 
            this.colSirketAdi.DataPropertyName = "SirketAdi";
            this.colSirketAdi.FillWeight = 115.1163F;
            this.colSirketAdi.HeaderText = "Şirket Adı";
            this.colSirketAdi.Name = "colSirketAdi";
            this.colSirketAdi.ReadOnly = true;
            // 
            // colVergiKimlikNo
            // 
            this.colVergiKimlikNo.DataPropertyName = "VergiKimlikNo";
            this.colVergiKimlikNo.FillWeight = 115.1163F;
            this.colVergiKimlikNo.HeaderText = "Vergi Kimlik No";
            this.colVergiKimlikNo.Name = "colVergiKimlikNo";
            this.colVergiKimlikNo.ReadOnly = true;
            // 
            // colListedenCikar
            // 
            this.colListedenCikar.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = "Listeden Çıkar";
            this.colListedenCikar.DefaultCellStyle = dataGridViewCellStyle2;
            this.colListedenCikar.HeaderText = "ListedenCikar";
            this.colListedenCikar.MinimumWidth = 100;
            this.colListedenCikar.Name = "colListedenCikar";
            this.colListedenCikar.ReadOnly = true;
            this.colListedenCikar.Text = "Listeden Çıkar";
            // 
            // frmCari14857Listesi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TesvikProgrami.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(942, 697);
            this.Controls.Add(this.btnSecilenleriCari14857ListesindenCikar);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnTumunuSecKaldir);
            this.Controls.Add(this.lblAra);
            this.Controls.Add(this.txtAra);
            this.Controls.Add(this.dgvSirketler);
            this.Icon = global::TesvikProgrami.Properties.Resources.iconNew;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmCari14857Listesi";
            this.Text = "Şirketler";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSirketler_FormClosing);
            this.Load += new System.EventHandler(this.frmSirketler_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSirketler)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvSirketler;
        private System.Windows.Forms.Label lblAra;
        private System.Windows.Forms.TextBox txtAra;
        private System.Windows.Forms.Button btnTumunuSecKaldir;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusSirketSayisi;
        private System.Windows.Forms.Button btnSecilenleriCari14857ListesindenCikar;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSec;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSirketAdi;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVergiKimlikNo;
        private System.Windows.Forms.DataGridViewButtonColumn colListedenCikar;
    }
}