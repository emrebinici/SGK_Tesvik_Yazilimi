using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;

namespace TesvikProgrami.Classes
{
    public class Indir
    {
        public long IsyeriId { get; set; }
        protected CancellationTokenSource tokenSource { get; set; } = new CancellationTokenSource();
        protected CancellationToken token;
        public frmIsyerleri frmIsyerleri { get; set; } = null;
        public frmSirketler frmSirketler { get; set; } = null;
        public frmLog FormLog { get; set; }
        public StringBuilder sb { get; set; } = new StringBuilder();

        public DataTable dtMevcutAphb = null;

        public IsyeriAphbBasvuruFormuIndirme indirilenIsyeri = null;

        protected Task task { get; set; } = null;
        public AphbBfIndirmeSonucu IndirmeSonucu { get; set; } = new AphbBfIndirmeSonucu();

        protected delegate void delLoglariGuncelle();
        protected void LoglariGuncelle()
        {
            if (FormLog != null)
            {
                if (FormLog.lbLog.InvokeRequired)
                {
                    FormLog.Invoke(new delLoglariGuncelle(LoglariGuncelle));
                }
                else
                {
                    if (FormLog != null)
                    {
                        FormLog.LoglariGuncelle(sb);

                    }
                }
            }
        }

        public async Task<AphbBfIndirmeSonucu> Calistir()
        {
            if (task == null) return IndirmeSonucu;

            if (task.Status != TaskStatus.Canceled)
            {
                task.Start();
            }
            else
            {
                IndirmeSonucu.IptalEdildi = true;
                IndirmeSonucu.Tamamlandi = true;
            }

            IndirmeSonucu.Baslatildi = true;

            try
            {
                await task;

                if (this is AphbIndir)
                {
                    IndirmeSonucu.Tamamlandi = true;
                    IndirmeSonucu.Basarili = ((AphbIndir)this).BasariylaKaydedildi;
                }
                else if (this is BasvuruFormuIndir)
                {
                    IndirmeSonucu.Tamamlandi = true;
                    IndirmeSonucu.Basarili = ((BasvuruFormuIndir)this).KaydedilenFormVar;
                }
            }
            catch (OperationCanceledException)
            {
                IndirmeSonucu.IptalEdildi = true;
                IndirmeSonucu.Tamamlandi = true;
            }
            catch (AggregateException)
            {
                IndirmeSonucu.IptalEdildi = true;
                IndirmeSonucu.Tamamlandi = true;
            }
            catch (Exception) {
                IndirmeSonucu.HataVar = true;

            }

            IndirmeBitti(this);

            //if (this.frmIsyerleri != null) this.frmIsyerleri.IndirmeBitti(this);
            //if (this.frmSirketler != null) this.frmSirketler.IndirmeBitti(this);

            return IndirmeSonucu;

        }

        public void IndirmeBitti(Indir indir)
        {
            {
                if (Program.IndirilenIsyerleri.ContainsKey(indir.IsyeriId))
                {
                    var indirilenisyeri = Program.IndirilenIsyerleri[indir.IsyeriId];

                    var baslatilmayanIndirmeler = new List<Indir>();
                    baslatilmayanIndirmeler.AddRange(indirilenisyeri.AphbIndirmeleri.Where(p => p.IndirmeSonucu.Baslatildi == false && p.IndirmeSonucu.IptalEdildi == false));
                    baslatilmayanIndirmeler.AddRange(indirilenisyeri.BasvuruFormuIndirmeleri.Where(p => p.IndirmeSonucu.Baslatildi == false && p.IndirmeSonucu.IptalEdildi == false));

                    if (baslatilmayanIndirmeler.Count == 0)
                    {
                        if (indirilenisyeri.AphbIndirmeleri.All(p => p.IndirmeSonucu.Tamamlandi) && indirilenisyeri.BasvuruFormuIndirmeleri.All(p => p.IndirmeSonucu.Tamamlandi))
                        {

                            var isyerleriDoldur = indirilenisyeri.AphbIndirmeleri.Any(p => p.IndirmeSonucu.Basarili)
                                ||
                             indirilenisyeri.BasvuruFormuIndirmeleri.Any(p => p.IndirmeSonucu.Basarili);

                            Program.IndirilenIsyerleri.Remove(indir.IsyeriId);

                            if (indirilenisyeri.formIsyerleri != null)
                            {
                                indirilenisyeri.formIsyerleri.IsyerleriDoldur();
                            }
                        }

                    }
                    else
                    {
                        if (indirilenisyeri.AphbIndirmeleri.Count > 0 && indirilenisyeri.AphbIndirmeleri.Any(p => p.IndirmeSonucu.Tamamlandi && (p.IndirmeSonucu.IptalEdildi || p.IndirmeSonucu.HataVar)))
                        {
                            foreach (var indirme in baslatilmayanIndirmeler)
                            {
                                if (indirme is BasvuruFormuIndir)
                                {
                                    var bfindir = (BasvuruFormuIndir)indirme;

                                    if (bfindir.IndirmeSonucu.IptalEdildi == false)
                                    {

                                        bfindir.IslemiIptalEt();
                                        bfindir.IndirmeSonucu.Tamamlandi = true;
                                        bfindir.IndirmeSonucu.IptalEdildi = true;
                                    }

                                }
                                else if (indirme is AphbIndir)
                                {
                                    var aphbindir = (AphbIndir)indirme;

                                    if (aphbindir.IndirmeSonucu.IptalEdildi == false)
                                    {
                                        aphbindir.Cancel();
                                        aphbindir.IndirmeSonucu.Tamamlandi = true;
                                        aphbindir.IndirmeSonucu.IptalEdildi = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (indirilenisyeri.AphbIndirmeleri.Count > 0)
                            {
                                using (var dbContext = new DbEntities())
                                {
                                    var isyeri = dbContext.Isyerleri.Include(p => p.Sirketler).Where(p => p.IsyeriID.Equals(indirilenisyeri.isyeri.IsyeriID)).FirstOrDefault();

                                    var aphbyol = Metodlar.FormBul(isyeri, Enums.FormTuru.Aphb);

                                    if (aphbyol != null)
                                    {
                                        var dtAphb = Metodlar.AylikListeyiYukle(aphbyol, false);

                                        foreach (var indirme in baslatilmayanIndirmeler)
                                        {
                                            if (indirme is BasvuruFormuIndir)
                                            {
                                                var bfindir = (BasvuruFormuIndir)indirme;

                                                bfindir.SuanYapilanIsyeriBasvuru = isyeri;
                                                bfindir.dtMevcutAphb = dtAphb;

                                            }
                                        }

                                    }

                                }

                            }

                        }

                        foreach (var indirme in baslatilmayanIndirmeler)
                        {
                            _ = indirme.Calistir();
                        }
                    }

                    if (indirilenisyeri.formIndirmeEkrani.IsDisposed == false)
                    {
                        indirilenisyeri.formIndirmeEkrani.Guncelle(indirilenisyeri);
                    }
                }
            }

            {
                var indirilensirketdeger = Program.IndirilenSirketler.FirstOrDefault(p => p.Value.IndirilenIsyerleri.ContainsKey(indir.IsyeriId));

                IsyeriAphbBasvuruFormuIndirme tamamlanan = null;

                if (indirilensirketdeger.Value != null)
                {
                    var sirketId = indirilensirketdeger.Key;
                    var indirilensirket = indirilensirketdeger.Value;
                    var indirilenisyeri = indirilensirket.IndirilenIsyerleri[indir.IsyeriId];

                    var baslatilmayanIndirmeler = new List<Indir>();
                    baslatilmayanIndirmeler.AddRange(indirilenisyeri.AphbIndirmeleri.Where(p => p.IndirmeSonucu.Baslatildi == false && p.IndirmeSonucu.IptalEdildi == false));
                    baslatilmayanIndirmeler.AddRange(indirilenisyeri.BasvuruFormuIndirmeleri.Where(p => p.IndirmeSonucu.Baslatildi == false && p.IndirmeSonucu.IptalEdildi == false));

                    if (baslatilmayanIndirmeler.Count == 0)
                    {
                        if (indirilenisyeri.AphbIndirmeleri.All(p => p.IndirmeSonucu.Tamamlandi) && indirilenisyeri.BasvuruFormuIndirmeleri.All(p => p.IndirmeSonucu.Tamamlandi))
                        {
                            tamamlanan = indirilenisyeri;

                            IsyeriAphbBasvuruFormuIndirme baslamamisIsyeri = null;

                            foreach (var digerindirilenisyeri in indirilensirket.IndirilenIsyerleri)
                            {
                                if (digerindirilenisyeri.Value.AphbIndirmeleri.Count > 0)
                                {
                                    foreach (var aphbIndirme in digerindirilenisyeri.Value.AphbIndirmeleri)
                                    {
                                        if (aphbIndirme.IndirmeSonucu.Baslatildi == false)
                                        {
                                            baslamamisIsyeri = digerindirilenisyeri.Value;

                                            break;
                                        }
                                    }

                                    if (baslamamisIsyeri != null) break;
                                }
                                else
                                {
                                    foreach (var basvuruFormuIndirme in digerindirilenisyeri.Value.BasvuruFormuIndirmeleri)
                                    {
                                        if (basvuruFormuIndirme.IndirmeSonucu.Baslatildi == false)
                                        {
                                            baslamamisIsyeri = digerindirilenisyeri.Value;

                                            break;
                                        }
                                    }

                                    if (baslamamisIsyeri != null) break;
                                }
                            }

                            if (baslamamisIsyeri != null)
                            {
                                if (baslamamisIsyeri.AphbIndirmeleri.Count > 0)
                                {
                                    foreach (var aphbIndirme in baslamamisIsyeri.AphbIndirmeleri)
                                    {
                                        _ = aphbIndirme.Calistir();
                                    }
                                }
                                else
                                {
                                    foreach (var basvuruFormuIndirme in baslamamisIsyeri.BasvuruFormuIndirmeleri)
                                    {
                                        _ = basvuruFormuIndirme.Calistir();
                                    }
                                }
                            }
                            else
                            {
                                var tamamlandi = indirilensirket.IndirilenIsyerleri.All(x => x.Value.AphbIndirmeleri.All(p => p.IndirmeSonucu.Tamamlandi) && x.Value.BasvuruFormuIndirmeleri.All(p => p.IndirmeSonucu.Tamamlandi));

                                if (tamamlandi)
                                {
                                    Program.IndirilenSirketler.Remove(sirketId);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (indirilenisyeri.AphbIndirmeleri.Count > 0 && indirilenisyeri.AphbIndirmeleri.Any(p => p.IndirmeSonucu.Tamamlandi && (p.IndirmeSonucu.IptalEdildi || p.IndirmeSonucu.HataVar)))
                        {
                            foreach (var indirme in baslatilmayanIndirmeler)
                            {
                                if (indirme is BasvuruFormuIndir)
                                {
                                    var bfindir = (BasvuruFormuIndir)indirme;

                                    if (bfindir.IndirmeSonucu.IptalEdildi == false)
                                    {

                                        bfindir.IslemiIptalEt();
                                        bfindir.IndirmeSonucu.Tamamlandi = true;
                                        bfindir.IndirmeSonucu.IptalEdildi = true;
                                    }

                                }
                                else if (indirme is AphbIndir)
                                {
                                    var aphbindir = (AphbIndir)indirme;

                                    if (aphbindir.IndirmeSonucu.IptalEdildi == false)
                                    {
                                        aphbindir.Cancel();
                                        aphbindir.IndirmeSonucu.Tamamlandi = true;
                                        aphbindir.IndirmeSonucu.IptalEdildi = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (indirilenisyeri.AphbIndirmeleri.Count > 0)
                            {
                                using (var dbContext = new DbEntities())
                                {
                                    var isyeri = dbContext.Isyerleri.Include(p => p.Sirketler).Where(p => p.IsyeriID.Equals(indirilenisyeri.isyeri.IsyeriID)).FirstOrDefault();

                                    var aphbyol = Metodlar.FormBul(isyeri, Enums.FormTuru.Aphb);

                                    if (aphbyol != null)
                                    {
                                        var dtAphb = Metodlar.AylikListeyiYukle(aphbyol, false);

                                        foreach (var indirme in baslatilmayanIndirmeler)
                                        {
                                            if (indirme is BasvuruFormuIndir)
                                            {
                                                var bfindir = (BasvuruFormuIndir)indirme;

                                                bfindir.SuanYapilanIsyeriBasvuru = isyeri;
                                                bfindir.dtMevcutAphb = dtAphb;

                                            }
                                        }

                                    }

                                }

                            }
                        }

                        foreach (var indirme in baslatilmayanIndirmeler)
                        {
                            _ = indirme.Calistir();
                        }
                    }

                    if (indirilenisyeri.formSirketler != null)
                    {
                        indirilenisyeri.formSirketler.dr = System.Windows.Forms.DialogResult.OK;
                    }

                    if (indirilenisyeri.formIndirmeEkrani != null && !indirilenisyeri.formIndirmeEkrani.IsDisposed)
                    {
                        indirilenisyeri.formIndirmeEkrani.Guncelle(indirilenisyeri);
                    }

                    if (tamamlanan != null)
                    {
                        if (indirilensirket.formSirketIndirmeEkrani != null && !indirilensirket.formSirketIndirmeEkrani.IsDisposed)
                        {
                            indirilensirket.formSirketIndirmeEkrani.Guncelle(tamamlanan);
                        }
                    }
                }
            }
        }
    }

}
