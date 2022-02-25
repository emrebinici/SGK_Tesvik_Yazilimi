using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TesvikProgrami.Classes;

namespace TesvikProgrami
{
    public static partial class Metodlar
    {
        public static StringBuilder GetPdfText(PdfReader reader)
        {
            StringBuilder text = new StringBuilder();
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                ITextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                string currentText = PdfTextExtractor.GetTextFromPage(reader, page, strategy);
                text.Append(currentText);
            }
            reader.Close();
            return text;
        }

        public static PdfReader PdfReaderDondur(string path)
        {
            return new PdfReader(path);
        }

        public static PdfReader PdfReaderDondur(byte[] bytes)
        {
            return new PdfReader(bytes);
        }

        public static PdfOkuma GetPdfAphbKisiList(PdfReader reader,string araci)
        {
            bool pdfBildirgeHataliOkunduMu = false;
            var bilgiDondurmekIcin = "";

            List<AphbSatir> ahpbKisiler = new List<AphbSatir>();

            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                iTextSharp.text.Rectangle rectSira = new iTextSharp.text.Rectangle(50, 570, 0, 100);
                RenderFilter[] filterSira = { new RegionTextRenderFilter(rectSira) };
                ITextExtractionStrategy strategySira = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterSira);
                string currentTextSira = PdfTextExtractor.GetTextFromPage(reader, page, strategySira);

                iTextSharp.text.Rectangle rectSgno = new iTextSharp.text.Rectangle(70, 570, 50, 100);
                RenderFilter[] filterSgno = { new RegionTextRenderFilter(rectSgno) };
                ITextExtractionStrategy strategySgno = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterSgno);
                string currentTextSgno = PdfTextExtractor.GetTextFromPage(reader, page, strategySgno);

                //150, 570, 100, 100 eskileri
                iTextSharp.text.Rectangle rectAdi = new iTextSharp.text.Rectangle(150, 570, 100, 100);
                RenderFilter[] filterAdi = { new RegionTextRenderFilter(rectAdi) };
                ITextExtractionStrategy strategyAdi = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterAdi);
                string currentTextAdi = PdfTextExtractor.GetTextFromPage(reader, page, strategyAdi);

                //250, 570, 170, 100 eskileri
                //iTextSharp.text.Rectangle rectSoyadi = new iTextSharp.text.Rectangle(255, 570, 165, 100);
                //RenderFilter[] filterSoyadi = { new RegionTextRenderFilter(rectSoyadi) };
                //ITextExtractionStrategy strategySoyadi = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterSoyadi);
                //string currentTextSoyadi = PdfTextExtractor.GetTextFromPage(reader, page, strategySoyadi);

                iTextSharp.text.Rectangle rectSoyadi = new iTextSharp.text.Rectangle(215, 570, 163, 100);
                RenderFilter[] filterSoyadi = { new RegionTextRenderFilter(rectSoyadi) };
                ITextExtractionStrategy strategySoyadi = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterSoyadi);
                string currentTextSoyadi = PdfTextExtractor.GetTextFromPage(reader, page, strategySoyadi);


                //iTextSharp.text.Rectangle rectDiger = new iTextSharp.text.Rectangle(600, 560, 300, 180);
                //RenderFilter[] filterDiger = { new RegionTextRenderFilter(rectDiger) };
                //ITextExtractionStrategy strategyDiger = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterDiger);
                //string currentTextDiger = PdfTextExtractor.GetTextFromPage(reader, page, strategyDiger);


                iTextSharp.text.Rectangle rectDiger = new iTextSharp.text.Rectangle(600, 560, 225, 180);
                RenderFilter[] filterDiger = { new RegionTextRenderFilter(rectDiger) };
                ITextExtractionStrategy strategyDiger = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterDiger);
                string currentTextDiger = PdfTextExtractor.GetTextFromPage(reader, page, strategyDiger);


                string[] siras = currentTextSira.Split('\n');
                string[] sgnos = currentTextSgno.Split('\n');
                string[] adis = currentTextAdi.Split('\n');
                string[] soyadis = currentTextSoyadi.Split('\n');
                string[] digers = currentTextDiger.Split('\n');

                for (int i = 0; i < siras.Length; i++)
                {
                    AphbSatir ahpbKisi = new AphbSatir();
                    ahpbKisi.SiraNo = siras[i];
                    ahpbKisi.SosyalGuvenlikNo = sgnos[i];
                    ahpbKisi.Adi = adis[i];
                    ahpbKisi.Soyadi = soyadis[i].Trim();

                    //ahpbKisi.Soyadi = soyadis[i].Split(' ')[0] != " " ? soyadis[i].Split(' ')[0] : "######";

                    if (ahpbKisi.Soyadi == "######" || ahpbKisi.Soyadi == "")
                    {
                        pdfBildirgeHataliOkunduMu = true;
                        bilgiDondurmekIcin = "Üstteki bildirgedeki " + (page) + " sayfası " + i + ". sırasındaki soyad okunamadı !!! " + adis[i] + " isimli";
                    }

                    //if (soyadis[i].Split(' ').Length > 1)
                    //    ahpbKisi.IlkSoyadi = soyadis[i].Split(' ')[1];


                    //ahpbKisi.Ucret = digers[i].Split(' ')[0];
                    //ahpbKisi.Ikramiye = digers[i].Split(' ')[1];
                    //ahpbKisi.Gun = digers[i].Split(' ')[2];
                    //ahpbKisi.EksikGunSayisi = digers[i].Split(' ')[3];
                    //ahpbKisi.GirisGunu = digers[i].Split(' ')[4] != "0" ? digers[i].Split(' ')[4].PadLeft(4, '0').Insert(2, "/") : "";
                    //ahpbKisi.CikisGunu = digers[i].Split(' ')[5] != "0" ? digers[i].Split(' ')[5].PadLeft(4, '0').Insert(2, "/") : "";
                    //ahpbKisi.IstenCikisNedeni = digers[i].Split(' ')[6] != "0" && digers[i].Split(' ')[6] != "00" ? digers[i].Split(' ')[6] : "";
                    //ahpbKisi.EksikGunNedeni = digers[i].Split(' ')[7] != "0" && digers[i].Split(' ')[7] != "00" ? digers[i].Split(' ')[7] : "";
                    //ahpbKisi.MeslekKod = digers[i].Split(' ')[8];

                    var splits = digers[i].Split(' ').ToList();

                    while (true)
                    {
                        if (!decimal.TryParse(splits[0], out decimal t) || !splits[0].Contains(","))
                        {
                            if (ahpbKisi.IlkSoyadi == null)
                                ahpbKisi.IlkSoyadi = splits[0].Trim() + " ";
                            else
                                ahpbKisi.IlkSoyadi += splits[0].Trim() + " ";
                            splits.RemoveAt(0);
                            continue;
                        }
                        else break;
                    }

                    if (ahpbKisi.IlkSoyadi != null)
                        ahpbKisi.IlkSoyadi = ahpbKisi.IlkSoyadi.Trim();

                    ahpbKisi.Ucret = splits[0];
                    ahpbKisi.Ikramiye = splits[1];
                    ahpbKisi.Gun = splits[2];
                    ahpbKisi.UCG = splits[3];
                    ahpbKisi.EksikGunSayisi = splits[4];
                    ahpbKisi.GirisGunu = splits[5] != "0" ? splits[5].PadLeft(4, '0').Insert(2, "/") : "";
                    ahpbKisi.CikisGunu = splits[6] != "0" ? splits[6].PadLeft(4, '0').Insert(2, "/") : "";
                    ahpbKisi.IstenCikisNedeni = splits[7] != "0" && splits[7] != "00" ? splits[7] : "";
                    ahpbKisi.EksikGunNedeni = splits[8] != "0" && splits[8] != "00" ? splits[8] : "";
                    ahpbKisi.MeslekKod = splits[9];
                    ahpbKisi.Araci = araci;
                    ahpbKisiler.Add(ahpbKisi);
                }

            }

            reader.Close();

            return new PdfOkuma { 
                satirlar = ahpbKisiler,
                bilgiDondurmekIcin = bilgiDondurmekIcin,
                pdfBildirgeHataliOkunduMu = pdfBildirgeHataliOkunduMu

            };
        }



    }



}
