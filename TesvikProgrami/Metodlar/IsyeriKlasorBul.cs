using System.IO;
using System.Linq;
using System.Windows.Forms;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static string IsyeriKlasorBul(Isyerleri isyeri, bool YeniOlusturulsun = false)
        {
            var subeadi = isyeri.SubeAdi;
            long sirketid = isyeri.SirketID;
            string sirketadi = isyeri.Sirketler.SirketAdi;

            string sirketpath = null;

            if (!Directory.Exists(Path.Combine(Application.StartupPath, "dosyalar"))) Directory.CreateDirectory(Path.Combine(Application.StartupPath, "dosyalar"));

            var paths = Directory.GetDirectories(Path.Combine(Application.StartupPath, "dosyalar"));

            sirketpath = paths.FirstOrDefault(p => p.EndsWith("-" + sirketid));

            if (sirketpath == null)
            {
                if (YeniOlusturulsun)
                {
                    sirketpath = Path.Combine(Path.Combine(Application.StartupPath, "dosyalar"), sirketadi.Replace("/", "").Replace("\\", "") + "-" + sirketid);

                    if (!Directory.Exists(sirketpath)) Directory.CreateDirectory(sirketpath);
                }
            }

            string isyeripath = null;

            if (sirketpath != null)
            {
                var isyeripaths = Directory.GetDirectories(sirketpath);

                isyeripath = isyeripaths.FirstOrDefault(p => p.EndsWith("-" + isyeri.IsyeriID));

                if (YeniOlusturulsun && isyeripath == null)
                {
                    isyeripath = Path.Combine(sirketpath, subeadi.Replace("/", "").Replace("\\", "") + "-" + isyeri.IsyeriID);

                    if (!Directory.Exists(isyeripath)) Directory.CreateDirectory(isyeripath);
                }
            }

            return isyeripath;
        }
        public static string IsyeriKlasorBul(IsyeriListesiDTO isyeri, bool YeniOlusturulsun = false)
        {
            var subeadi = isyeri.SubeAdi;
            string sirketid = isyeri.SirketID;
            string sirketadi = isyeri.SirketAdi;

            string sirketpath = null;

            if (!Directory.Exists(Path.Combine(Application.StartupPath, "dosyalar"))) Directory.CreateDirectory(Path.Combine(Application.StartupPath, "dosyalar"));

            var paths = Directory.GetDirectories(Path.Combine(Application.StartupPath, "dosyalar"));

            sirketpath = paths.FirstOrDefault(p => p.EndsWith("-" + sirketid));

            if (sirketpath == null)
            {
                if (YeniOlusturulsun)
                {
                    sirketpath = Path.Combine(Path.Combine(Application.StartupPath, "dosyalar"), sirketadi.Replace("/", "").Replace("\\", "") + "-" + sirketid);

                    if (!Directory.Exists(sirketpath)) Directory.CreateDirectory(sirketpath);
                }
            }

            string isyeripath = null;

            if (sirketpath != null)
            {
                var isyeripaths = Directory.GetDirectories(sirketpath);

                isyeripath = isyeripaths.FirstOrDefault(p => p.EndsWith("-" + isyeri.ID));

                if (YeniOlusturulsun && isyeripath == null)
                {
                    isyeripath = Path.Combine(sirketpath, subeadi.Replace("/", "").Replace("\\", "") + "-" + isyeri.ID);

                    if (!Directory.Exists(isyeripath)) Directory.CreateDirectory(isyeripath);
                }
            }

            return isyeripath;
        }



    }



}
