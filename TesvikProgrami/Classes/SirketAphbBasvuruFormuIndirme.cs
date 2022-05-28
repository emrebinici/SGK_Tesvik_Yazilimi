using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class SirketAphbBasvuruFormuIndirme
    {
        public SirketAphbBasvuruFormuIndirme(Sirketler val)
        {
            this.sirket = val;
        }

        public Sirketler sirket { get; set; }
        public Dictionary<long, IsyeriAphbBasvuruFormuIndirme> IndirilenIsyerleri { get; set; } = new Dictionary<long, IsyeriAphbBasvuruFormuIndirme>();
        public Dictionary<long, Isyerleri> Isyerleri { get; set; } = new Dictionary<long, Isyerleri>();
        public frmSirketIndirmeEkrani formSirketIndirmeEkrani { get; set; }

        public void TumunuIptalEt()
        {
            foreach (var item in IndirilenIsyerleri)
            {
                item.Value.TumunuIptalEt();
            }
        }
    }

}
