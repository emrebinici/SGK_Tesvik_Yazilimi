using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class IptalBildirgeIstatikleri : BildirgeYuklemeIcmal
    {
        public int bulunanKisiSayisi = 0;
        public int basariliKisiSayisi = 0;
        public bool Tamamlandi = false;
        public string IptalKanun;
        public string Mahiyet;
        public string IslemTarihi;
        public List<string> iptalKisiler;
        public decimal IptalEdilecekKisilerEkranindanHesaplananTesvikTutari;
        public bool Basarili;
    }
}
