using System;
using System.Data;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static DataTable BasvuruListesi7166Yukle(string file)
        {
            DataSet dsBasvuruListesi7166 = Metodlar.ReadExcelFile(file);

            DataTable dt = dsBasvuruListesi7166.Tables[0];

            int j = 0;

            while (j < dt.Rows.Count)
            {
                if (String.IsNullOrEmpty(dt.Rows[j][(int)Enums.BasvuruListesi7166SutunTurleri.TcKimlikNoSosyalGuvenlikNo].ToString()))
                {
                    dt.Rows.RemoveAt(j);
                }
                else j++;
            }


            return dt;
        }



    }



}
