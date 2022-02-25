using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static bool SirketCari14857IcindeVarMi(long SirketId)
        {
            using (var dbContext= new DbEntities())
            {
                return dbContext.Cari14857YapilanSirketler.Where(p => p.SirketId.Equals(SirketId)).Count() > 0;
            }
        }



    }



}
