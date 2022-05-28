using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static void SirketPathUpdate(Sirketler sirket)
        {

            string YeniSirketAdi = sirket.SirketAdi.Replace("/", "").Replace("\\", ""); ;

            if (!String.IsNullOrEmpty(YeniSirketAdi))
            {
                var path = Directory.GetDirectories("dosyalar").FirstOrDefault(p => p.EndsWith("-" + sirket.SirketID));

                if (!string.IsNullOrEmpty(path))
                {
                    try
                    {
                        DirectoryInfo di = new DirectoryInfo(path);

                        string YeniPath = Path.Combine(di.Parent.FullName, YeniSirketAdi + "-" + sirket.SirketID);

                        if (!Directory.Exists(YeniPath)) Directory.Move(path, YeniPath);
                    }
                    catch
                    {

                    }
                }
            }


        }

        public static void IsyeriPathUpdate(Isyerleri isyeri)
        {

            //var yeniisyeripath = IsyeriKlasorBul(isyeri, true);

            string YeniSubeAdi = isyeri.SubeAdi.Replace("/", "").Replace("\\", "");
            string YeniSirketAdi = isyeri.Sirketler.SirketAdi.Replace("/", "").Replace("\\", "") + "-" + isyeri.SirketID;

            string[] sirketpaths = Directory.GetDirectories("dosyalar", "*", SearchOption.TopDirectoryOnly);

            foreach (var sirketpath in sirketpaths)
            {

                var isyeripath = Directory.GetDirectories(sirketpath).FirstOrDefault(p => p.EndsWith("-" + isyeri.IsyeriID));

                if (isyeripath != null)
                {
                    isyeripath = Path.Combine(Application.StartupPath, isyeripath);

                    try
                    {
                        string YeniPath = Path.Combine(Application.StartupPath, "dosyalar", YeniSirketAdi, YeniSubeAdi + "-" + isyeri.IsyeriID);

                        DirectoryInfo di = new DirectoryInfo(YeniPath);

                        if (!di.Parent.Exists) Directory.CreateDirectory(di.Parent.FullName);

                        if (!Directory.Exists(YeniPath)) Directory.Move(isyeripath, YeniPath);
                    }
                    catch { }

                    break;
                }

            }



        }


    }


}
