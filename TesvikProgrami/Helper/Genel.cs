using System;
using System.Collections.Generic;
using System.IO;

namespace TesvikProgrami
{
    public static class Genel
    {
        public static void IcmalKaydediliyorKontrolu()
        {
            var lockFile = Path.Combine("temp", "IcmalKaydediliyor.txt");

            while (File.Exists(lockFile))
            {
                FileInfo fi = new FileInfo(lockFile);

                if (DateTime.UtcNow.Subtract(fi.CreationTimeUtc).TotalSeconds < 30)
                {
                    System.Threading.Thread.Sleep(500);
                }
                else
                {
                    IcmalKaydediliyorKilidiniKaldir();

                    break;
                }
            }

            try
            {
                if (!Directory.Exists("temp"))
                    Directory.CreateDirectory("temp");

                File.WriteAllText(Path.Combine("temp", "IcmalKaydediliyor.txt"),"1");
            }
            catch { }

        }

        public static void IcmalKaydediliyorKilidiniKaldir()
        {
            var lockFile = Path.Combine("temp", "IcmalKaydediliyor.txt");

            if (File.Exists(lockFile))
            {
                try
                {
                    File.Delete(lockFile);
                }
                catch { }

            }

        }
    }

}
