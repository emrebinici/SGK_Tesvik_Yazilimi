using System;
using System.Data;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static DataTable AylikListeyiYukle(string file,bool MesajGostersin=true)
        {
            DataSet dsaylikliste = Metodlar.ReadExcelFile(file, MesajGostersin: MesajGostersin);

            DataTable dt = dsaylikliste.Tables[0];

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0][0].ToString().ToUpper() == "YIL")
                {
                    dt.Rows.RemoveAt(0);
                }
            }

            bool UCGVar = dt.Columns.Contains("UÇG");

            if (! UCGVar)
            {
                var dtNew = new DataTable(dt.TableName);

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dtNew.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);

                    if (i == (int)Enums.AphbHucreBilgileri.Gun)
                    {
                        dtNew.Columns.Add("UÇG", typeof(string));
                    }
                }

                foreach (DataRow row in dt.Rows)
                {
                    DataRow newrow = dtNew.NewRow();

                    int i = 0;
                    while(i < dt.Columns.Count)
                    {
                        int colIndex = i;

                        if (i > (int)Enums.AphbHucreBilgileri.Gun)
                        {
                            colIndex = i + 1;
                        }

                        newrow[colIndex] = row[i];
                        
                        i++;
                    }

                    dtNew.Rows.Add(newrow);
                }

                dt = dtNew;
            }

            if (dt.Columns.Count > 5)
            {
                if (dt.Columns[5].DataType == typeof(double))
                {
                    DataTable dtresult = new DataTable(dt.TableName);

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (i != 5)
                        {
                            dtresult.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
                        }
                        else dtresult.Columns.Add(dt.Columns[i].ColumnName, typeof(string));
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        DataRow newrow = dtresult.NewRow();

                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            newrow[i] = row[i];
                        }

                        dtresult.Rows.Add(newrow);
                    }

                    dt = dtresult;
                }
            }


            int j = 0;

            while (j < dt.Rows.Count)
            {
                if (String.IsNullOrEmpty(dt.Rows[j][(int)Enums.AphbHucreBilgileri.TcKimlikNoSosyalGuvenlikNo].ToString()) &&
                    String.IsNullOrEmpty(dt.Rows[j][(int)Enums.AphbHucreBilgileri.Ad].ToString()) &&
                    String.IsNullOrEmpty(dt.Rows[j][(int)Enums.AphbHucreBilgileri.Soyad].ToString()))
                {
                    dt.Rows.RemoveAt(j);
                }
                else
                {
                    dt.Rows[j][(int)Enums.AphbHucreBilgileri.Ay] = Convert.ToInt32(dt.Rows[j][(int)Enums.AphbHucreBilgileri.Ay]);
                    j++;
                }
            }


            return dt;
        }


    }


}
