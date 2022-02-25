using System;
using System.Collections.Generic;
using System.Linq;

namespace TesvikProgrami.Classes
{
    public class IsyeriAphbBasvuruFormuIndirme
    {
        public Isyerleri isyeri { get; set; }
        public List<AphbIndir> AphbIndirmeleri { get; set; } = new List<AphbIndir>();
        public List<BasvuruFormuIndir> BasvuruFormuIndirmeleri { get; set; } = new List<BasvuruFormuIndir>();
        public frmIndirmeEkrani formIndirmeEkrani { get; set; }
        public frmIsyerleri formIsyerleri { get; set; }
        public frmSirketler formSirketler { get; set; }
        public bool SonAyIseGirenlerCekildi {get;set;}
        public List<string> SonAyIseGirenlerListesi { get; set; } = new List<string>();
        public bool DigerBasvuruIndirmeleriBittiMi(BasvuruFormuIndir except = null)
        {
            return this.BasvuruFormuIndirmeleri.Where(p => p != except).All(p => p.IndirmeSonucu.Tamamlandi);
        }

        public void TumunuIptalEt()
        {
            //var aphbIptalEdildi = false;
            foreach (var item in AphbIndirmeleri)
            {
                if (item.IndirmeSonucu.IptalEdildi == false && item.IndirmeSonucu.Tamamlandi == false)
                {
                    item.Cancel();
                    item.IndirmeSonucu.Tamamlandi = true;
                    item.IndirmeSonucu.IptalEdildi = true;
                }
            }

            foreach (var item in BasvuruFormuIndirmeleri)
            {
                if (item.IndirmeSonucu.IptalEdildi == false && item.IndirmeSonucu.Tamamlandi == false)
                {
                    item.IslemiIptalEt();
                    item.IndirmeSonucu.Tamamlandi = true;
                    item.IndirmeSonucu.IptalEdildi = true;
                }
            }

        }
    }

}
