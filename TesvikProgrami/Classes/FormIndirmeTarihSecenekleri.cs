using System;
using System.Collections.Generic;

namespace TesvikProgrami.Classes
{
    public class FormIndirmeTarihSecenekleri
    {
        public bool AphbIndirilsin { get; set; }
        public bool BasvuruFormuIndirilsin { get; set; }
        public DateTime BaslangicAphb { get; set; }
        public DateTime BitisAphb { get; set; }


        public bool IndirTumTesvikler { get; set; }
        public DateTime BaslangicTum { get; set; }
        public DateTime BitisTum { get; set; }
        public bool EnBastanTumu { get; set; }


        public bool Indir6111 { get; set; }
        public DateTime Baslangic6111 { get; set; }
        public DateTime Bitis6111 { get; set; }
        public bool EnBastan6111 { get; set; }

        public bool Indir7103 { get; set; }
        public DateTime Baslangic7103 { get; set; }
        public DateTime Bitis7103 { get; set; }
        public bool EnBastan7103 { get; set; }

        public bool Indir2828 { get; set; }
        public DateTime Baslangic2828 { get; set; }
        public DateTime Bitis2828 { get; set; }
        public bool EnBastan2828 { get; set; }

        public bool Indir7252{ get; set; }
        public DateTime Baslangic7252{ get; set; }
        public DateTime Bitis7252 { get; set; }
        public bool EnBastan7252 { get; set; }

        public bool Indir7256 { get; set; }
        public DateTime Baslangic7256 { get; set; }
        public DateTime Bitis7256 { get; set; }
        public bool EnBastan7256 { get; set; }

        public bool Indir7316{ get; set; }
        public DateTime Baslangic7316 { get; set; }
        public DateTime Bitis7316 { get; set; }
        public bool EnBastan7316 { get; set; }

        public bool Indir3294 { get; set; }
        public DateTime Baslangic3294 { get; set; }
        public DateTime Bitis3294 { get; set; }
        public bool EnBastan3294 { get; set; }


        public bool Indir6645 { get; set; }
        public DateTime Baslangic6645 { get; set; }
        public DateTime Bitis6645 { get; set; }


        public bool Indir687 { get; set; }
        public DateTime Baslangic687 { get; set; }
        public DateTime Bitis687 { get; set; }


        public bool Indir14857 { get; set; }
        public DateTime Baslangic14857 { get; set; }
        public DateTime Bitis14857 { get; set; }

        public List<string> incelenecekDonemler = new List<string>();
        public List<string> incelenecekDonemler7103 = new List<string>();
        public List<string> incelenecekDonemler2828 = new List<string>();
        public List<string> incelenecekDonemler7252 = new List<string>();
        public List<string> incelenecekDonemler7256 = new List<string>();
        public List<string> incelenecekDonemler7316 = new List<string>();
        public List<string> incelenecekDonemler3294 = new List<string>();

        public FormIndirmeTarihSecenekleri Clone()
        {
            var clone = new FormIndirmeTarihSecenekleri
            {
                AphbIndirilsin = this.AphbIndirilsin,
                BasvuruFormuIndirilsin = this.BasvuruFormuIndirilsin,
                BaslangicAphb = this.BaslangicAphb,
                BitisAphb = this.BitisAphb,
                IndirTumTesvikler = this.IndirTumTesvikler,
                BaslangicTum = this.BaslangicTum,
                BitisTum = this.BitisTum,
                Indir6111 = this.Indir6111,
                Baslangic6111 = this.Baslangic6111,
                Bitis6111 = this.Bitis6111,
                Indir7103 = this.Indir7103,
                Baslangic7103 = this.Baslangic7103,
                Bitis7103 = this.Bitis7103,
                Indir2828 = this.Indir2828,
                Baslangic2828 = this.Baslangic2828,
                Bitis2828 = this.Bitis2828,
                Indir7252 = this.Indir7252,
                Baslangic7252 = this.Baslangic7252,
                Bitis7252 = this.Bitis7252,
                Baslangic7256 = this.Baslangic7256,
                Bitis7256 = this.Bitis7256,
                Baslangic7316 = this.Baslangic7316,
                Bitis7316 = this.Bitis7316,
                Baslangic3294 = this.Baslangic3294,
                Bitis3294 = this.Bitis3294,
                Indir6645 = this.Indir6645,
                Baslangic6645 = this.Baslangic6645,
                Bitis6645 = this.Bitis6645,
                Indir687 = this.Indir687,
                Baslangic687 = this.Baslangic687,
                Bitis687 = this.Bitis687,
                Indir14857 = this.Indir14857,
                Baslangic14857 = this.Baslangic14857,
                Bitis14857 = this.Bitis14857,
                EnBastanTumu = this.EnBastanTumu,
                EnBastan6111 = this.EnBastan6111,
                EnBastan7103 = this.EnBastan7103,
                EnBastan2828 = this.EnBastan2828,
                EnBastan7252 = this.EnBastan7252,
                EnBastan7256 = this.EnBastan7256,
                EnBastan7316 = this.EnBastan7316,
                EnBastan3294 = this.EnBastan3294,
            };

            if (this.incelenecekDonemler != null)
            {
                clone.incelenecekDonemler = new List<string>();
                clone.incelenecekDonemler.AddRange(this.incelenecekDonemler);
            }

            if (this.incelenecekDonemler7103 != null)
            {
                clone.incelenecekDonemler7103 = new List<string>();
                clone.incelenecekDonemler7103.AddRange(this.incelenecekDonemler7103);
            }

            if (this.incelenecekDonemler2828 != null)
            {
                clone.incelenecekDonemler2828 = new List<string>();
                clone.incelenecekDonemler2828.AddRange(this.incelenecekDonemler2828);
            }


            if (this.incelenecekDonemler7252 != null)
            {
                clone.incelenecekDonemler7252= new List<string>();
                clone.incelenecekDonemler7252.AddRange(this.incelenecekDonemler7252);
            }

            if (this.incelenecekDonemler7256 != null)
            {
                clone.incelenecekDonemler7256 = new List<string>();
                clone.incelenecekDonemler7256.AddRange(this.incelenecekDonemler7256);
            }

            if (this.incelenecekDonemler7316 != null)
            {
                clone.incelenecekDonemler7316 = new List<string>();
                clone.incelenecekDonemler7316.AddRange(this.incelenecekDonemler7316);
            }

            if (this.incelenecekDonemler3294 != null)
            {
                clone.incelenecekDonemler3294 = new List<string>();
                clone.incelenecekDonemler3294.AddRange(this.incelenecekDonemler3294);
            }

            return clone;
        }

    }

}
