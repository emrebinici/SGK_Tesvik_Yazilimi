using System;
using System.Data;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {

        public static DataSet BasvuruListesiniYukle(string file,bool MesajGostersin=true)
        {

            DataSet dsbasvurulistesi = Metodlar.ReadExcelFile(file, MesajGostersin);

            int j = 0;

            while (j < dsbasvurulistesi.Tables.Count)
            {

                DataTable dt = dsbasvurulistesi.Tables[j];

                int i = 0;

                string Kanun = dt.TableName;
                if (Kanun.Equals("6111")) Kanun = "6111-v2";

                while (i < dt.Columns.Count)
                {
                    if (dt.Columns[i].ColumnName.StartsWith("Column") || dt.Columns[i].ColumnName.Equals("NO"))
                    {
                        dt.Columns.RemoveAt(i);

                        i--;
                    }

                    i++;
                }

                if (dt.Columns.Count > 0)
                {
                    i = 0;

                    while (i < dt.Rows.Count)
                    {
                        if (String.IsNullOrEmpty(dt.Rows[i][Sabitler.BasvuruFormlariSutunlari[Kanun][Enums.BasvuruFormuSutunTurleri.TcKimlikNo]].ToString()))
                        {
                            dt.Rows.RemoveAt(i);
                        }
                        else i++;
                    }

                    j++;
                }
                else
                {
                    dsbasvurulistesi.Tables.RemoveAt(j);
                }
            }

            return dsbasvurulistesi;
        }


    }



}
