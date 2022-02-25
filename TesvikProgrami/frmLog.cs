using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace TesvikProgrami
{
    public partial class frmLog : Form
    {
        frmIsyerleri formIsyerleri = null;
        frmAyarlar formAyarlar = null;

        DialogResult dr = DialogResult.None;

        public frmLog(frmIsyerleri frmisyerleri)
        {
            InitializeComponent();

            this.formIsyerleri = frmisyerleri;
        }

        public frmLog(frmAyarlar frmAyarlar)
        {
            InitializeComponent();

            this.formAyarlar = frmAyarlar;
        }

        public frmLog(StringBuilder sb)
        {
            InitializeComponent();

            lbLog.Items.Clear();

            var splits = sb.ToString().Replace(Environment.NewLine,"|").Split('|');

            if (splits.Length > 0)
            {
                lbLog.Items.AddRange(splits);

                Guncelle();

            }
        }

        public frmLog(ref List<Classes.BasvuruLog> loglar)
        {
            InitializeComponent();

            lbLog.Items.Clear();

            loglar.ForEach(p => {
                p.LogEkranindaGosterildi = false;
            });
            

            LoglariGuncelle(ref loglar);
        }

        public void Guncelle()
        {
            try
            {
                lbLog.Refresh();

                int visibleItems = lbLog.ClientSize.Height / lbLog.ItemHeight;

                lbLog.TopIndex = Math.Max(lbLog.Items.Count - visibleItems + 1, 0);
            }
            catch
            {

            }
        }


        public void LoglariGuncelle(StringBuilder sb)
        {
            string Log = sb.ToString();

            if (!String.IsNullOrEmpty(Log))
            {

                Log = Log.Substring(0, Log.LastIndexOf(Environment.NewLine));

                if (Log.Contains(Environment.NewLine))
                {
                    Log = Log.Substring(Log.LastIndexOf(Environment.NewLine));

                    Log = Log.Replace(Environment.NewLine, "");

                }

                lbLog.Items.Add(Log);

                Guncelle();

            }
        }
        public void LoglariGuncelle(ref List<Classes.BasvuruLog> loglar)
        {

            try
            {

                //var logs = loglar.Select(p => String.Format("{0} : {1}", p.Tarih.ToString("dd.MM.yy HH:mm"), p.Mesaj)).ToList();

                //lbLog.DataSource = logs;

                //lbLog.Refresh();

                var eskiloglar = loglar.Where(p => p.LogEkranindaGosterildi).ToList();

                var yeniloglar = loglar.Where(p => !p.LogEkranindaGosterildi && p.LogEkranindaGoster).ToList();

                //yeniloglar.ForEach(p=> lbLog.Items.Add(String.Format("{0} : {1}", p.Tarih.ToString("dd.MM.yy HH:mm"), p.Mesaj)));

                yeniloglar.ForEach(p => p.LogEkranindaGosterildi = true);

                //lbLog.Refresh();

                int visibleItems = lbLog.ClientSize.Height / lbLog.ItemHeight;

                var yenilogIlk = yeniloglar.FirstOrDefault();

                var index =  eskiloglar.FindIndex(p => p.Donem > yenilogIlk.Donem);

                if (index == -1 || yenilogIlk.Donem.Equals(-1))
                {
                    yeniloglar.ForEach(p => lbLog.Items.Add(String.Format("[{0}] : {1}", p.Tarih.ToString("dd.MM.yyyy HH:mm:ss"), p.Mesaj)));
                }
                else
                {
                    yeniloglar.Reverse();
                    yeniloglar.ForEach(p => lbLog.Items.Insert(index, String.Format("[{0}] : {1}", p.Tarih.ToString("dd.MM.yyyy HH:mm:ss"), p.Mesaj)));
                }

                //lbLog.Items.Insert(sonTarihIndex, String.Format("{0} : {1}", loglar[sonTarihIndex].Tarih.ToString("dd.MM.yy HH:mm"), loglar[sonTarihIndex].Mesaj));

                //int visibleItems = lbLog.ClientSize.Height / lbLog.ItemHeight;

                //int ortaEleman = visibleItems / 2;

                //if (lbLog.Items.Count > visibleItems)
                //{
                //    if (sonTarihIndex > ortaEleman)
                //    {
                //        lbLog.TopIndex = sonTarihIndex - ortaEleman;
                //    }
                //}

                //lbLog.SelectedIndex = sonTarihIndex;

                lbLog.TopIndex = Math.Max(lbLog.Items.Count - visibleItems + 1, 0);

            }
            catch
            {

            }

        }

        private void frmLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.formIsyerleri != null)
            {
                //if (this.formIsyerleri.BasvuruFormuIndiriliyor)
                //{
                //    if (e.CloseReason == CloseReason.UserClosing)
                //    {

                //        if (MessageBox.Show("Devam eden işlemi iptal etmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                //        {
                //            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);

                //            formIsyerleri.BasvuruFormuIndirilen.IslemiIptalEt();
                //        }
                //        else
                //        {
                //            e.Cancel = true;
                //        }
                //    }
                //    else formIsyerleri.BasvuruFormuIndirilen.IslemiIptalEt();

                //    dr = DialogResult.OK;
                //}
                //else if (this.formIsyerleri.AphbIndiriliyor)
                //{
                //    if (e.CloseReason == CloseReason.UserClosing)
                //    {

                //        if (MessageBox.Show("Devam eden işlemi iptal etmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                //        {
                //            bool Kaydet = false;

                //            foreach (var item in formIsyerleri.AphbIndirilenIsyeri.Bildirgeler)
                //            {
                //                if (item.Kisiler.Count > 0)
                //                {
                //                    Kaydet = MessageBox.Show("İndirilen bildirgeleri excele kaydetmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

                //                    break;
                //                }
                //            }

                //            formIsyerleri.AphbIndirilenIsyeri.IslemiIptalEt(Kaydet);

                //            if (Kaydet) dr = DialogResult.OK;
                //            else dr = DialogResult.No;
                //        }
                //        else
                //        {
                //            e.Cancel = true;

                //            dr = DialogResult.No;
                //        }
                //    }
                //    else
                //    {
                //        formIsyerleri.AphbIndirilenIsyeri.IslemiIptalEt(false);

                //        dr = DialogResult.No;
                //    }

                //}
                if (this.formIsyerleri.BildirgeYuklemeYapiliyor)
                {
                    if (e.CloseReason == CloseReason.UserClosing)
                    {

                        if (MessageBox.Show("Devam eden işlemi iptal etmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            //InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);

                            formIsyerleri.BildirgeYuklemeIslemiIptalEt();
                        }
                        else
                        {
                            e.Cancel = true;
                        }
                    }
                    else formIsyerleri.BildirgeYuklemeIslemiIptalEt();

                    dr = DialogResult.No;

                }
                else dr = DialogResult.OK;

                this.DialogResult = dr;
            }
            else if (this.formAyarlar != null)
            {
                if (this.formAyarlar.TesvikBasvuruYapiliyor)
                {
                    if (e.CloseReason == CloseReason.UserClosing)
                    {

                        if (MessageBox.Show("Devam eden işlemi iptal etmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            //InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);

                            formAyarlar.IslemiIptalEt();
                        }
                        else
                        {
                            e.Cancel = true;
                        }
                    }
                    else formAyarlar.IslemiIptalEt();
                }
            }
            else
            {
                e.Cancel= true;
                this.Hide();
            }

        }

        private void frmLog_Shown(object sender, EventArgs e)
        {
            Guncelle();
        }
    }
}
