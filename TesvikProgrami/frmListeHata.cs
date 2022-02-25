using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace TesvikProgrami
{
    public partial class frmListeHata : Form
    {
        DataTable dthatalisatirlar;
        Dictionary<string, DataTable> BasvuruFormlariHataliSatirlar;

        public frmListeHata(DataTable aphbhatalisatirlar, Dictionary<string, DataTable> basvuruFormlariHataliSatirlar)
        {
            dthatalisatirlar = aphbhatalisatirlar;
            BasvuruFormlariHataliSatirlar = basvuruFormlariHataliSatirlar;

            InitializeComponent();
        }

        private void frmListeHata_Load(object sender, EventArgs e)
        {

            foreach (var item in BasvuruFormlariHataliSatirlar)
            {
                var dthatalibasvuru = item.Value;

                if (dthatalibasvuru != null)
                {

                    var kanun = item.Key;

                    var tab = new TabPage(kanun);
                    tab.Name = "tab" + kanun;

                    DataGridView dgv = new DataGridView();
                    dgv.Name = "dgv" + kanun;
                    dgv.Dock = DockStyle.Fill;

                    tab.Controls.Add(dgv);

                    dgv.DataSource = dthatalibasvuru;


                    tabControl1.TabPages.Add(tab);
                }
            }
            

            if (dthatalisatirlar == null)
            {
                tabControl1.TabPages.Remove(tabAphb);
            }
            else dgvAylik.DataSource = dthatalisatirlar;
        }


    }
}
