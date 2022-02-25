using System;
using System.Collections.Generic;
using System.Data;

namespace TesvikProgrami.Classes
{
    public class AphbSatir
    {
        public AphbSatir() { }
        public string SosyalGuvenlikNo { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string IlkSoyadi { get; set; }
        public string Ucret { get; set; }
        public string Ikramiye { get; set; }
        public string Gun { get; set; }
        public string UCG { get; set; }
        public string EksikGunSayisi { get; set; }
        public string GirisGunu { get; set; }
        public string CikisGunu { get; set; }
        public string EksikGunNedeni { get; set; }
        public string IstenCikisNedeni { get; set; }
        public string MeslekKod { get; set; }
        public string SiraNo { get; set; }
        public string Araci { get; set; }
        public string Mahiyet { get; set; }
        public string OnayDurumu { get; set; }
        public string BelgeTuru { get; set; }
        public string Kanun { get; set; }
        public string OrijinalKanun { get; set; }
        public string TesvikKanunNo { get; set; }
        public bool TesvikVerilecekMi { get; set; } = true;
        public string Yil { get; set; }
        public string Ay { get; set; }
        public System.Xml.Linq.XElement xElement { get; set; }
        public NetsisSatir NetsisBilgiler { get; set; }
        public NetsisSatir  NetsisBilgilerExcel { get; set; }
        public DateTime BildirgeIlkKayitTarihi { get; set; }
        public bool satirBolunecek { get; set; }
        public List<AphbSatir> BolunecekSatirlar { get; set; }
        public string BildirgeRefNo { get; set; }
        public string TesvikHesaplanacakGun { get; set; }
        public string HesaplananGun => string.IsNullOrEmpty(TesvikHesaplanacakGun) ? this.Gun :  this.TesvikHesaplanacakGun;
        public string HesaplananUcret => string.IsNullOrEmpty(TesvikHesaplanacakGun) ? this.Ucret : this.Gun.ToInt() > 0 ? (this.Ucret.ToDecimalSgk() / this.Gun.ToInt() * this.TesvikHesaplanacakGun.ToInt()).ToString() : this.satirBolunecek && this.BolunecekSatir.Gun.ToInt() > 0 ? "0" : this.Ucret;
        public string HesaplananIkramiye => string.IsNullOrEmpty(TesvikHesaplanacakGun) ? this.Ikramiye : this.Gun.ToInt() > 0 ? (this.Ikramiye.ToDecimalSgk() / this.Gun.ToInt() * this.TesvikHesaplanacakGun.ToInt()).ToString() : this.satirBolunecek && this.BolunecekSatir.Gun.ToInt() > 0 ? "0" : this.Ikramiye;
        public decimal HesaplananToplamUcret => this.HesaplananUcret.ToDecimalSgk() + this.HesaplananIkramiye.ToDecimalSgk();
        public string DonusturulecekHesaplanacakGun { get; set; }
        public string HesaplananDonusecekGun => string.IsNullOrEmpty(DonusturulecekHesaplanacakGun) ? this.Gun : this.DonusturulecekHesaplanacakGun;
        public string HesaplananDonusecekUcret => string.IsNullOrEmpty(DonusturulecekHesaplanacakGun) ? this.Ucret : this.Gun.ToInt() > 0 ? (this.Ucret.ToDecimalSgk() / this.Gun.ToInt() * this.DonusturulecekHesaplanacakGun.ToInt()).ToString() : this.satirBolunecek && this.BolunecekSatir.Gun.ToInt() > 0 ? "0" : this.Ucret;
        public string HesaplananDonusecekIkramiye => string.IsNullOrEmpty(DonusturulecekHesaplanacakGun) ? this.Ikramiye : this.Gun.ToInt() > 0 ? (this.Ikramiye.ToDecimalSgk() / this.Gun.ToInt() * this.DonusturulecekHesaplanacakGun.ToInt()).ToString() : this.satirBolunecek && this.BolunecekSatir.Gun.ToInt() > 0 ? "0" : this.Ikramiye;
        public decimal HesaplananDonusecekToplamUcret => this.HesaplananDonusecekUcret.ToDecimalSgk() + HesaplananDonusecekIkramiye.ToDecimalSgk();
        public bool BolunenSatirMi { get; set; }
        public AphbSatir BolunecekSatir { get; set; }
        public string MuhtasarOrijinalKanun { get; set; }
        public DataRow IlgiliSatir { get; set; }

        public AphbSatir Clone()
        {
            var clone = new AphbSatir();
            clone.SosyalGuvenlikNo = this.SosyalGuvenlikNo;
            clone.Adi = this.Adi;
            clone.Soyadi = this.Soyadi;
            clone.IlkSoyadi = this.IlkSoyadi;
            clone.Ucret = this.Ucret;
            clone.Ikramiye = this.Ikramiye;
            clone.Gun = this.Gun;
            clone.EksikGunSayisi = this.EksikGunSayisi;
            clone.GirisGunu = this.GirisGunu;
            clone.CikisGunu = this.CikisGunu;
            clone.EksikGunNedeni = this.EksikGunNedeni;
            clone.IstenCikisNedeni = this.IstenCikisNedeni;
            clone.MeslekKod = this.MeslekKod;
            clone.SiraNo = this.SiraNo;
            clone.Araci = this.Araci;
            clone.Mahiyet = this.Mahiyet;
            clone.OnayDurumu = this.OnayDurumu;
            clone.BelgeTuru = this.BelgeTuru;
            clone.Kanun = this.Kanun;
            clone.OrijinalKanun = this.OrijinalKanun;
            clone.TesvikKanunNo = this.TesvikKanunNo;
            clone.TesvikVerilecekMi = this.TesvikVerilecekMi;
            clone.Yil = this.Yil;
            clone.Ay = this.Ay;
            clone.xElement = this.xElement;
            clone.NetsisBilgiler = this.NetsisBilgiler;
            clone.NetsisBilgilerExcel = this.NetsisBilgilerExcel;
            clone.BildirgeIlkKayitTarihi = this.BildirgeIlkKayitTarihi;
            clone.satirBolunecek = this.satirBolunecek;
            clone.BolunecekSatirlar = this.BolunecekSatirlar;
            clone.BildirgeRefNo = this.BildirgeRefNo;
            clone.TesvikHesaplanacakGun = this.TesvikHesaplanacakGun;
            clone.DonusturulecekHesaplanacakGun = this.DonusturulecekHesaplanacakGun;
            clone.BolunenSatirMi = this.BolunenSatirMi;
            clone.BolunecekSatir = this.BolunecekSatir;
            clone.MuhtasarOrijinalKanun = this.MuhtasarOrijinalKanun;
            clone.IlgiliSatir = this.IlgiliSatir;

            return clone;
        }
    }

}
