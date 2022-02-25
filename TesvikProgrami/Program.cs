using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Linq;
using System.Management;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        public static Dictionary<string, Classes.Tesvik> tempTesvikler = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new Classes.Tesvik(x, false));

        public static Dictionary<string, Classes.Tesvik> TumTesvikler = null;

        public static Dictionary<string, bool> AsgariUcretDestekTutariDikkateAlinsin = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => false);

        public static bool BildirgelerOnaylansin = false;

        public static bool AcikKalanExcellerKapatilsin = false;

        public static bool BfIndirmeUcretDestegiIstensin = false;

        public static bool BasvuruYoksaTesvikVerilmesin = true;

        public static bool SeciliKanunlarDonusturulsun = false;

        public static int MinimumGunSayisi = 20;

        public static int BfEgitimBelgesi = 8;

        public static decimal BildirgeMinimumTutar = 36;

        public static Dictionary<string, decimal> AsgariUcretler = new Dictionary<string, decimal>();

        public static Dictionary<string, decimal> BelgeTuruOranlari = new Dictionary<string, decimal>();

        public static int ZamanAsimiSuresi = 100;

        public static bool OtomatikGuvenlikKoduGirilecekIsverenSistemi = false;
        public static bool OtomatikGuvenlikKoduGirilecek6645 = false;
        public static bool OtomatikGuvenlikKoduGirilecek14857 = false;
        public static bool OtomatikGuvenlikKoduGirilecek687 = false;
        public static bool OtomatikGuvenlikKoduGirilecekEBildirgeV2 = false;

        public static bool CaptchaGosteriliyor = false;

        public static bool DonemIslemcisiYeniGirisYapsin = false;
        public static bool KisiIslemcisiYeniGirisYapsin = false;
        public static int DonemIslemciSayisi = 1;
        public static int KisiIslemciSayisi = 1;
        public static bool DetayliLoglamaYapilsin = true;
        public static bool GuvenlikKoduCozdur = false;
        public static bool OncekiBildirgelerIptalEdilsin = true;
        public static bool EgitimListesiOlusturulsun = false;
        public static bool CariAphbOlusturulsun = true;
        public static bool Liste7166Cikarilsin = true;
        public static bool Son6AyGecmisHesaplansin = true;
        public static bool BasvuruDonemleriCekilsin = false;

        public static Dictionary<long, BelgeTurleri> BelgeTurleri = new Dictionary<long, BelgeTurleri>();
        public static Dictionary<string, KisaVadeliSigortaPrimKoluOranlari> KisaVadeliSigortaPrimKoluOranlari = new Dictionary<string, KisaVadeliSigortaPrimKoluOranlari>();
        public static List<DonusturulecekKanunlar> DonusturulecekKanunlar = new List<DonusturulecekKanunlar>();
 
        public static Dictionary<long, IsyeriAphbBasvuruFormuIndirme> IndirilenIsyerleri = new Dictionary<long, IsyeriAphbBasvuruFormuIndirme>();
        public static Dictionary<long, SirketAphbBasvuruFormuIndirme> IndirilenSirketler = new Dictionary<long, SirketAphbBasvuruFormuIndirme>();

        [STAThread]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static void Main()
        {
            // Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += new ThreadExceptionEventHandler(Form1_UIThreadException);

            // Set the unhandled exception mode to force all Windows Forms errors to go through 
            // our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event. 
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //db.Load(Sabitler.xmlpath);

            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = (System.IO.Path.GetDirectoryName(executable));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);

            string cpuSerialNumber = string.Empty;
            string hardDiskSerialNumber = string.Empty;
            string anaKartSeriNo = string.Empty;

            //string query = "SELECT * FROM Win32_BaseBoard";

            //ManagementObjectSearcher searcher =
            //    new ManagementObjectSearcher(query);
            //foreach (ManagementObject info in searcher.Get())
            //{
            //    anaKartSeriNo = info.GetPropertyValue("SerialNumber").ToString();
            //}


            //string query = "Select * FROM Win32_Processor";
            //var searcher =
            //    new ManagementObjectSearcher(query);
            //foreach (ManagementObject info in searcher.Get())
            //{
            //    cpuSerialNumber = info.GetPropertyValue("ProcessorId").ToString();
            //}

            //query = "Select * FROM Win32_DiskDrive";
            //searcher =
            //    new ManagementObjectSearcher(query);
            //foreach (ManagementObject info in searcher.Get())
            //{
            //    hardDiskSerialNumber = info.GetPropertyValue("SerialNumber").ToString();
            //}

            //String firstMacAddress = System.Net.NetworkInformation.NetworkInterface
            //.GetAllNetworkInterfaces()
            //.Where(nic => nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up && nic.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
            //.Select(nic => nic.GetPhysicalAddress().ToString())
            //.FirstOrDefault();

            List<Ayarlar> anahtarlar = new List<Ayarlar>();

            using (var dbContext = new DbEntities())
            {
                anahtarlar = dbContext.Ayarlar.ToList();

                Program.BelgeTurleri = dbContext.BelgeTurleri.ToDictionary(x=> x.BelgeTuruID, x=> x);
                Program.KisaVadeliSigortaPrimKoluOranlari = dbContext.KisaVadeliSigortaPrimKoluOranlari.ToDictionary(x => x.KisaVadeliSigortaKoluKodu.PadLeft(4, '0'), x => x);
                Program.DonusturulecekKanunlar = dbContext.DonusturulecekKanunlar.ToList();

                if (!anahtarlar.Any(p => p.Anahtar.Equals("Cari14857YapilanSirketlerTablosuOlusturuldu")))
                {
                    var sql = @"CREATE TABLE Cari14857YapilanSirketler (

                                Id    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,

                                SirketId  INTEGER NOT NULL,

                                FOREIGN KEY(SirketId) REFERENCES Sirketler(SirketID) ON UPDATE CASCADE ON DELETE CASCADE
                               );

                                CREATE UNIQUE INDEX indexCari14857YapilanSirketler_SirketId ON Cari14857YapilanSirketler(
                                    SirketId
                                );";

                    try
                    {
                        dbContext.Database.ExecuteSqlCommand(sql);

                        dbContext.Ayarlar.Add(new Ayarlar { Anahtar = "Cari14857YapilanSirketlerTablosuOlusturuldu", Deger = "True" });

                        dbContext.SaveChanges();
                    }
                    catch { }
                    
                }

            }

            var SeciliKanunlarDonusturulsun = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("SeciliKanunlarDonusturulsun"));

            if (SeciliKanunlarDonusturulsun != null) Program.SeciliKanunlarDonusturulsun = SeciliKanunlarDonusturulsun.Deger.Equals("True");

            TumTesvikler = TesvikHesaplamaSabitleri.TesvikVerilecekKanunlar.ToDictionary(x => x, x => new Classes.Tesvik(x));

            foreach (var item in Program.TumTesvikler)
            {
                if (item.Value.AsgariUcretDestekTutarlariDikkateAlinsin)
                {
                    var auda = anahtarlar.FirstOrDefault(p=> p.Anahtar.Equals("AsgariUcretDestekTutariDikkateAlinsin" + item.Key));

                    if (auda != null)
                    {
                        Program.AsgariUcretDestekTutariDikkateAlinsin[item.Key] = auda.Deger.Equals("True");
                    }
                    else
                    {
                        Program.AsgariUcretDestekTutariDikkateAlinsin[item.Key] = true;
                    }
                }
            }

            var MinimumGunSayisi = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("MinimumGunSayisi"));

            if (MinimumGunSayisi != null) Program.MinimumGunSayisi = Convert.ToInt32(MinimumGunSayisi.Deger);


            var BildirgeMinimumTutar = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BildirgeMinimumTutar"));

            if (BildirgeMinimumTutar != null) Program.BildirgeMinimumTutar = BildirgeMinimumTutar.Deger.ToDecimalSgk();


            var BildirgelerOnaylansin = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BildirgelerOnaylansin"));

            if (BildirgelerOnaylansin != null) Program.BildirgelerOnaylansin = BildirgelerOnaylansin.Deger.Equals("True");


            var AcikKalanExcellerKapansin = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("AcikKalanExcellerKapansin"));

            if (AcikKalanExcellerKapansin != null) Program.AcikKalanExcellerKapatilsin = AcikKalanExcellerKapansin.Deger.Equals("True");


            var BfIndirmeUcretDestegiIstensin = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfIndirmeUcretDestegiIstensin"));

            if (BfIndirmeUcretDestegiIstensin != null) Program.BfIndirmeUcretDestegiIstensin = BfIndirmeUcretDestegiIstensin.Deger.Equals("True");


            var BasvuruYoksaTesvikVerilmesin = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BasvuruYoksaTesvikVerilmesin"));

            if (BasvuruYoksaTesvikVerilmesin != null) Program.BasvuruYoksaTesvikVerilmesin = BasvuruYoksaTesvikVerilmesin.Deger.Equals("True");


            var ZamanasimiSuresi = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("ZamanasimiSuresi"));

            if (ZamanasimiSuresi != null) Program.ZamanAsimiSuresi = Convert.ToInt32(ZamanasimiSuresi.Deger);


            var IsverenSistemiGuvenlikKoduGirisi = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("IsverenSistemiGuvenlikKoduGirisi"));

            if (IsverenSistemiGuvenlikKoduGirisi != null) Program.OtomatikGuvenlikKoduGirilecekIsverenSistemi = IsverenSistemiGuvenlikKoduGirisi.Deger.Equals("True");


            var GuvenlikKoduGirisi6645 = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GuvenlikKoduGirisi6645"));

            if (GuvenlikKoduGirisi6645 != null) Program.OtomatikGuvenlikKoduGirilecek6645 = GuvenlikKoduGirisi6645.Deger.Equals("True");


            var GuvenlikKoduGirisi687 = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GuvenlikKoduGirisi687"));

            if (GuvenlikKoduGirisi687 != null) Program.OtomatikGuvenlikKoduGirilecek687 = GuvenlikKoduGirisi687.Deger.Equals("True");


            var EBildirgeV2GuvenlikKoduGirisi = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("EBildirgeV2GuvenlikKoduGirisi"));

            if (EBildirgeV2GuvenlikKoduGirisi != null) Program.OtomatikGuvenlikKoduGirilecekEBildirgeV2 = EBildirgeV2GuvenlikKoduGirisi.Deger.Equals("True");


            var GuvenlikKoduGirisi14857 = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GuvenlikKoduGirisi14857"));

            if (GuvenlikKoduGirisi14857 != null) Program.OtomatikGuvenlikKoduGirilecek14857 = GuvenlikKoduGirisi14857.Deger.Equals("True");


            var DonemIslemcisiYeniGirisYapsin = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("DonemIslemcisiYeniGirisYapsin"));

            if (DonemIslemcisiYeniGirisYapsin != null) Program.DonemIslemcisiYeniGirisYapsin = DonemIslemcisiYeniGirisYapsin.Deger.Equals("True");


            var KisiIslemcisiYeniGirisYapsin = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("KisiIslemcisiYeniGirisYapsin"));

            if (KisiIslemcisiYeniGirisYapsin != null) Program.KisiIslemcisiYeniGirisYapsin = KisiIslemcisiYeniGirisYapsin.Deger.Equals("True");


            var DetayliLoglamaYapilsin = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("DetayliLoglamaYapilsin"));

            if (DetayliLoglamaYapilsin != null) Program.DetayliLoglamaYapilsin = DetayliLoglamaYapilsin.Deger.Equals("True");


            var DonemIslemciSayisi = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("DonemIslemciSayisi"));

            if (DonemIslemciSayisi != null) Program.DonemIslemciSayisi = Convert.ToInt32(DonemIslemciSayisi.Deger);


            var KisiIslemciSayisi = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("KisiIslemciSayisi"));

            if (KisiIslemciSayisi != null) Program.KisiIslemciSayisi = Convert.ToInt32(KisiIslemciSayisi.Deger);

            var GuvenlikKoduCozdur = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("GuvenlikKoduCozdur"));

            if (GuvenlikKoduCozdur != null) Program.GuvenlikKoduCozdur = GuvenlikKoduCozdur.Deger.Equals("True");

            var bfEgitimBelgesi = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BfEgitimBelgesi"));

            if (bfEgitimBelgesi != null) Program.BfEgitimBelgesi = Convert.ToInt32(bfEgitimBelgesi.Deger);

            var OncekiBildirgelerIptalEdilsin = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("OncekiBildirgelerIptalEdilsin"));

            if (OncekiBildirgelerIptalEdilsin != null) Program.OncekiBildirgelerIptalEdilsin = OncekiBildirgelerIptalEdilsin.Deger.Equals("True");

            var EgitimListesiOlusturulsun = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("EgitimListesiOlusturulsun"));

            if (EgitimListesiOlusturulsun != null) Program.EgitimListesiOlusturulsun = EgitimListesiOlusturulsun.Deger.Equals("True");

            var CariAphbOlusturulsun = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("CariAphbOlusturulsun"));

            if (CariAphbOlusturulsun != null) Program.CariAphbOlusturulsun = CariAphbOlusturulsun.Deger.Equals("True");

            var Liste7166Cikarilsin = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("7166ListesiCikarilsin"));

            if (Liste7166Cikarilsin != null) Program.Liste7166Cikarilsin = Liste7166Cikarilsin.Deger.Equals("True");

            var Son6AyGecmisHesaplansin = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("Son6AyGecmisHesaplansin"));

            if (Son6AyGecmisHesaplansin != null) Program.Son6AyGecmisHesaplansin = Son6AyGecmisHesaplansin.Deger.Equals("True");

            var BasvuruDonemleriCekilsin = anahtarlar.FirstOrDefault(p => p.Anahtar.Equals("BasvuruDonemleriCekilsin"));

            if (BasvuruDonemleriCekilsin != null) Program.BasvuruDonemleriCekilsin = BasvuruDonemleriCekilsin.Deger.Equals("True");

            Application.Run(new frmIsyerleri());
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether 
        // or not they wish to abort execution. 

        private static void Form1_UIThreadException(object sender, ThreadExceptionEventArgs t)
        {

            DialogResult result = DialogResult.Cancel;
            try
            {
                result = ShowThreadExceptionDialog("Hata", t.Exception);
            }
            catch
            {
                try
                {
                    MessageBox.Show("Ölümcül hata",
                        "Ölümcül hata", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }

            Application.Exit();

        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether 
        // or not they wish to abort execution. 
        // NOTE: This exception cannot be kept from terminating the application - it can only  
        // log the event, and inform the user about it.  
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;

            try
            {
                string Mesaj = Environment.NewLine + "Hata:" + ex.Message + Environment.NewLine;

                Mesaj += ex.StackTrace + Environment.NewLine;

                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();

                Mesaj += "Hata Dosyası:" + frame.GetFileName() + ", Hata Satırı Numarası:" + line;

                //string errorMsg = "Bilinmeyen bir hata meydana geldi.";

                File.AppendAllText(Application.StartupPath + "\\hatalar.txt", Mesaj + Environment.NewLine);

                //File.AppendAllText(Application.StartupPath + "\\hatalar.txt", "Stack Trace:" + ex.StackTrace + Environment.NewLine);

                // Since we can't prevent the app from terminating, log this to the event log. 
                if (!EventLog.SourceExists("TesvikProgrami"))
                {
                    EventLog.CreateEventSource("TesvikProgrami", "Application");
                }

                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog();
                myLog.Source = "TesvikProgrami";
                myLog.WriteEntry(Mesaj);

            }
            catch { }

            Metodlar.HataMesajiGoster(ex, "Bilinmeyen bir hata meydana geldi");

        }

        // Creates the error message and displays it. 
        private static DialogResult ShowThreadExceptionDialog(string title, Exception e)
        {
            string Mesaj = Environment.NewLine + "Hata:" + e.Message + Environment.NewLine;

            Mesaj += e.StackTrace + Environment.NewLine;

            var st = new StackTrace(e, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();

            Mesaj += "Hata Dosyası:" + frame.GetFileName() + ", Hata Satırı Numarası:" + line + " Tarih:" + DateTime.Now;


            try
            {
                File.AppendAllText(Application.StartupPath + "\\hatalar.txt", Mesaj + Environment.NewLine);

                // Since we can't prevent the app from terminating, log this to the event log. 
                if (!EventLog.SourceExists("TesvikProgrami"))
                {
                    EventLog.CreateEventSource("TesvikProgrami", "Application");
                }

                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog();
                myLog.Source = "TesvikProgrami";
                myLog.WriteEntry(Mesaj);
            }
            catch
            {

            }


            return MessageBox.Show(Mesaj, title, MessageBoxButtons.OK,
                MessageBoxIcon.Stop);
        }
    }
}
