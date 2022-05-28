using ExcelDataReader;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static DataSet ReadExcelFile(string file,bool MesajGostersin = true,bool useHeaderRow=true)
        {
            DataSet ds = new DataSet();


            //Mevcut açık excel varsa kapatılmalı öncesinde! 
            //

            var denemeSayisi = 0;

        YenidenDene:
            denemeSayisi++;

            try
            {
                using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read))
                {

                    string extension = Path.GetExtension(file);

                    //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)

                    IExcelDataReader excelReader = extension.ToLower() == ".xlsx" ? ExcelReaderFactory.CreateOpenXmlReader(stream) : ExcelReaderFactory.CreateBinaryReader(stream);

                    {
                        ds = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = useHeaderRow
                                
                            }
                            
                        });

                        excelReader.Close();

                        try
                        {
                            excelReader.Dispose();
                        }
                        catch { }

                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Invalid file signature.")
                {
                    if (file.Contains("\\temp\\"))
                    {
                        var path = Path.GetDirectoryName(file);
                        try
                        {
                            var newFileNameTemp = Path.Combine(path, Path.GetFileNameWithoutExtension(file)+ ".xlsx");
                            File.Copy(file, newFileNameTemp);

                            file = newFileNameTemp;

                            goto YenidenDene;
                        }
                        catch {
                            if (MesajGostersin)
                            {
                                MessageBox.Show("Excel dosyası hatalı olduğu için okunamadı" + Environment.NewLine + Environment.NewLine + "Dosya:" + Environment.NewLine + Environment.NewLine + file, "Dosya Okuma Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                    }
                }

                if (MesajGostersin)
                {
                    MessageBox.Show("Lütfen açık olan excel dosyasını kapattıktan sonra TAMAM tuşuna basarak tekrar deneyiniz" + Environment.NewLine + Environment.NewLine + "Dosya:" + Environment.NewLine + Environment.NewLine + file, "Dosya Okuma Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    System.Threading.Thread.Sleep(500);
                }

                Metodlar.DetayliLogYaz("Lütfen açık olan excel dosyasını kapattıktan sonra TAMAM tuşuna basarak tekrar deneyiniz" + Environment.NewLine + Environment.NewLine + "Dosya: " + Environment.NewLine + Environment.NewLine + file);

                goto YenidenDene;
            }

            return ds;

            /*
            string connectionString = GetExcelConnectionString(file);

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;

                // Get all Sheets in Excel File
                DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                // Loop through all Sheets to get data
                foreach (DataRow dr in dtSheet.Rows)
                {
                    string sheetName = dr["TABLE_NAME"].ToString();

                    if (!sheetName.EndsWith("$"))
                        continue;

                    // Get all rows from the Sheet
                    cmd.CommandText = "SELECT * FROM [" + sheetName + "]";

                    DataTable dt = new DataTable();
                    dt.TableName = sheetName;

                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(dt);

                    ds.Tables.Add(dt);
                }

                cmd = null;
                conn.Close();
            }
            
            return ds;
            */
        }


    }


}
