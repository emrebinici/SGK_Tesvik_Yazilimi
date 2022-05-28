using System.Data.Entity;
using System.Linq;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static Isyerleri TaseronNodanIsyeriBul(Isyerleri isyeri, string TaseronNo)
        {

            using (var dbContext = new DbEntities())
            {
                return isyeri = dbContext.Isyerleri.Where(p=> p.IsyeriSicilNo.Equals(isyeri.IsyeriSicilNo) && p.TaseronNo.Equals(TaseronNo)).Include(p => p.Sirketler).FirstOrDefault();
            }

        }



    }



}
