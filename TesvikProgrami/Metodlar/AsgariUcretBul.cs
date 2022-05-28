using System;
using System.Linq;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static decimal AsgariUcretBul(int yil, int ay)
        {

            if (Program.AsgariUcretler.ContainsKey(yil + "-" + ay))
            {
                return Program.AsgariUcretler[yil + "-" + ay];
            }
            else
            {
                decimal AsgariUcret = 0;

                using (var dbContext = new DbEntities())
                {
                    DateTime yilay = new DateTime(yil, ay, 1);

                    var asgariUcretBilgileri = dbContext.AsgariUcretler.ToList();

                    var asgariUcretBilgisi = asgariUcretBilgileri.FirstOrDefault(p => Convert.ToDateTime(p.Baslangic) <= yilay && yilay <= Convert.ToDateTime(p.Bitis));

                    if (asgariUcretBilgisi != null)
                    {
                        AsgariUcret = Convert.ToDecimal(asgariUcretBilgisi.AsgariUcretTutari);
                    }
                }

                if (AsgariUcret == 0)
                {
                    throw new Exception(yil + " yılına ait asgari ücret girilmemiş. Lütfen yapmakta olduğunuz işlemi iptal edip asgari ücret tutarı girildikten sonra yeniden deneyiniz");
                }

                Program.AsgariUcretler.Add(yil + "-" + ay, AsgariUcret);

                return AsgariUcret;
            }
        }


    }


}
