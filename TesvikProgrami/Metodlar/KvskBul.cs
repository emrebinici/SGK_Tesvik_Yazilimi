using System;
using System.Windows.Forms;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static decimal KvskBul(int yil, int ay, string IsyeriSicilNo)
        {
            string kisaVadeliSigortaKoluKodu = IsyeriSicilNo.Substring(1, 4);

            decimal kisaVadeliSigortaKoluOrani = 0;

            DateTime dtkisavadeson = new DateTime(2013, 9, 1);

            DateTime yilaygun = new DateTime(yil, ay, 1);

            if (yilaygun >= dtkisavadeson)
            {
                kisaVadeliSigortaKoluOrani = 2;
            }
            else
            {
                var kvsk = Program.KisaVadeliSigortaPrimKoluOranlari.ContainsKey(kisaVadeliSigortaKoluKodu) ? Program.KisaVadeliSigortaPrimKoluOranlari[kisaVadeliSigortaKoluKodu] : null;

                if (kvsk != null)
                {
                    kisaVadeliSigortaKoluOrani = kvsk.PrimOrani.ToDecimal();
                }
                else
                {
                    MessageBox.Show(String.Format("{0} nolu sigortalı kolunun KVSK oranı bulunamadı", kisaVadeliSigortaKoluKodu), "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return kisaVadeliSigortaKoluOrani;
        }



    }



}
