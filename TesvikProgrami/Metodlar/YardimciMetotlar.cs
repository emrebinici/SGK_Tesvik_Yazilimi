using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {

        public static string TokenBul(HtmlAgilityPack.HtmlDocument html)
        {
            var tokeninput = html.DocumentNode.Descendants("input").FirstOrDefault(p => p.GetAttributeValue("name", "").Equals("token"));

            if (tokeninput != null) return tokeninput.GetAttributeValue("value", "");

            return string.Empty;
        }


        public static object DeepCopy(object obj)
        {
            if (obj == null)
                return null;
            Type type = obj.GetType();

            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }
            else if (type.IsArray)
            {
                Type elementType = Type.GetType(
                     type.AssemblyQualifiedName.Replace("[]", string.Empty));
                var array = obj as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    copied.SetValue(DeepCopy(array.GetValue(i)), i);
                }
                return Convert.ChangeType(copied, obj.GetType());
            }
            else if (type.IsClass)
            {

                object toret = Activator.CreateInstance(obj.GetType());
                FieldInfo[] fields = type.GetFields(BindingFlags.Public |
                            BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue == null)
                        continue;
                    field.SetValue(toret, DeepCopy(fieldValue));
                }
                return toret;
            }
            else
                throw new ArgumentException("Unknown type");
        }

        public static void FindAndReplace(Microsoft.Office.Interop.Word.Application doc, object findText, object replaceWithText)
        {
            //options
            object matchCase = true;
            object matchWholeWord = true;
            object matchWildCards = false;
            object matchSoundsLike = false;
            object matchAllWordForms = false;
            object forward = true;
            object format = false;
            object matchKashida = false;
            object matchDiacritics = false;
            object matchAlefHamza = false;
            object matchControl = false;
            object read_only = true;
            object visible = false;
            object replace = Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;
            object wrap = 1;
            //execute find and replace

            try
            {
                doc.Selection.Find.Execute(ref findText, ref matchCase, ref matchWholeWord,
                    ref matchWildCards, ref matchSoundsLike, ref matchAllWordForms, ref forward, ref wrap, ref format, ref replaceWithText, ref replace,
                    ref matchKashida, ref matchDiacritics, ref matchAlefHamza, ref matchControl);

            }
            catch
            {

                try
                {
                    doc.ActiveDocument.Content.Find.Execute(ref findText, ref matchCase, ref matchWholeWord,
                    ref matchWildCards, ref matchSoundsLike, ref matchAllWordForms, ref forward, ref wrap, ref format, ref replaceWithText, ref replace,
                    ref matchKashida, ref matchDiacritics, ref matchAlefHamza, ref matchControl);

                }
                catch
                {
                }
            }
        }

        public static List<int> GetProcessIdsSnapshot(string appname)
        {
            List<int> ProcessIds = new List<int>();
            try
            {
                Process[] Processes = Process.GetProcessesByName(appname);
                for (int n_loop = 0; n_loop < Processes.Length; n_loop++)
                    ProcessIds.Add(Processes[n_loop].Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Process Snapshot not Successful " + ex.ToString(), "Error");
            }

            return ProcessIds;
        }

        public static int GetProcessId(List<int> l_initialProcessIds, List<int> l_finalProcessIds)
        {
            try
            {
                for (int n_loop = 0; n_loop < l_initialProcessIds.Count; n_loop++)
                {
                    int n_PidInitialProcessList = l_initialProcessIds[n_loop];
                    for (int n_innerloop = 0; n_innerloop < l_finalProcessIds.Count; n_innerloop++)
                    {
                        int n_PidFinalProcessList = l_finalProcessIds[n_innerloop];
                        if (n_PidInitialProcessList == n_PidFinalProcessList)
                        {
                            l_finalProcessIds.RemoveAt(n_innerloop);
                            break;
                        }
                    }
                    l_initialProcessIds.RemoveAt(n_loop);
                    n_loop--;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetExcelProcessId() unsuccessful" + ex.ToString(), "Error");
            }

            return l_finalProcessIds[0];
        }

        public static void KillProcessById(int n_processId)
        {
            try
            {
                Process xlProcess = null;
                xlProcess = Process.GetProcessById(n_processId);
                xlProcess.Kill();
            }
            catch
            {

            }
        }

        public static String getWordDocumentPropertyValue(Microsoft.Office.Interop.Word.Document document, string propertyName)
        {
            object builtInProperties = document.BuiltInDocumentProperties;

            Type builtInPropertiesType = builtInProperties.GetType();

            object property = builtInPropertiesType.InvokeMember("Item", BindingFlags.GetProperty, null, builtInProperties, new object[] { propertyName });

            Type propertyType = property.GetType();

            object propertyValue = propertyType.InvokeMember("Value", BindingFlags.GetProperty, null, property, new object[] { });

            return propertyValue.ToString();
        }

        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);

        public static int GetExcelProcessId(Microsoft.Office.Interop.Excel.Application excelApp)
        {
            int id;
            GetWindowThreadProcessId(excelApp.Hwnd, out id);
            return id;
            //return Process.GetProcessById(id);
        }

        public static void HataMesajiGoster(Exception ex, string Mesaj)
        {

            Mesaj += Environment.NewLine + "Hata:" + ex.Message + Environment.NewLine;

            Mesaj += ex.StackTrace + Environment.NewLine;

            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();

            Mesaj += "Hata Dosyası:" + frame.GetFileName() + ", Hata Satırı Numarası:" + line;

            MessageBox.Show(Mesaj, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        public static void DetayliLogYaz(string Mesaj)
        {
            if (Program.DetayliLoglamaYapilsin)
            {
                int sayac = 0;
            Yaz:
                try
                {
                    File.AppendAllText("DetayliLogKayitlari.txt", "[" + DateTime.Now.ToString() + "] : " + Mesaj + Environment.NewLine);
                }
                catch
                {
                    sayac++;
                    Thread.Sleep(200);

                    if (sayac < 5)
                    {
                        goto Yaz;
                    }
                }
            }
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                tb.Columns.Add(prop.Name, prop.PropertyType);
            }

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        public static bool CariAyDahilMi(string BaslangicAy, string BaslangicYil, string BitisAy, string BitisYil)
        {
            DateTime baslangic = DateTime.MinValue;

            DateTime bitis = DateTime.MaxValue;

            if (!string.IsNullOrEmpty(BaslangicYil))
            {
                baslangic = new DateTime(Convert.ToInt32(BaslangicYil), string.IsNullOrEmpty(BaslangicAy) ? 1 : Convert.ToInt32(BaslangicAy), 1);
            }

            if (!string.IsNullOrEmpty(BitisYil))
            {
                bitis = new DateTime(Convert.ToInt32(BitisYil), string.IsNullOrEmpty(BitisAy) ? 12 : Convert.ToInt32(BitisAy), 1);
            }

            if (baslangic > bitis)
            {
                DateTime temp = baslangic;

                baslangic = bitis;

                bitis = temp;
            }

            var simdikiAy = new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
            var oncekiAy = simdikiAy.AddMonths(-1);

            return (oncekiAy >= baslangic && oncekiAy <= bitis);

        }
    }

}
