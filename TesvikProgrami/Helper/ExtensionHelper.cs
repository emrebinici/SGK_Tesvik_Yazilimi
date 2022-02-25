using System;
using System.Collections.Generic;
using System.Linq;

namespace TesvikProgrami
{
    public static class ExtensionHelper
    {
        public static string GetInnerText(this HtmlAgilityPack.HtmlNode htmlNode)
        {
            return htmlNode != null ? htmlNode.InnerText : "";
        }

        public static string ToTL(this double value)
        {
            return value.ToString("C", new System.Globalization.CultureInfo("TR-tr")).Replace(" TL", "");

        }

        public static string ToTL(this decimal value)
        {
            return value.ToString("C", new System.Globalization.CultureInfo("TR-tr")).Replace(" TL", "");

        }

        public static decimal ToDecimal(this double value)
        {
            return Convert.ToDecimal(value);

        }

        public static long ToLong(this string value)
        {
            return Convert.ToInt64(value);
        }

        public static int ToInt(this string value)
        {
            return Convert.ToInt32(value);
        }

        public static double ToDouble(this string value)
        {
            return Convert.ToDouble(value.Replace("₺", "").Replace(".", "").Trim());
        }

        public static decimal ToDecimalSgk(this string value)
        {
            return Convert.ToDecimal(value.Replace("₺", "").Replace(".","").Trim());
        }

        public static bool ToBool(this string value)
        {
            return Convert.ToBoolean(value);
        }

        public static string AddToken(this string value, HtmlAgilityPack.HtmlDocument html)
        {
            var token = Metodlar.TokenBul(html);

            if (!string.IsNullOrEmpty(token)) return value + "&struts.token.name=token&token=" + token;

            return value;
        }

        public static string AddToken(this string value, string html)
        {
            HtmlAgilityPack.HtmlDocument htmlDom = new HtmlAgilityPack.HtmlDocument();

            htmlDom.LoadHtml(html);

            return value.AddToken(htmlDom);

        }

        public static bool Between(this DateTime input, DateTime date1, DateTime date2)
        {
            return (input >= date1 && input <= date2);
        }

        public static string TurkceKarakterleriDegistir(this string value)
        {
            Dictionary<string, string> liste = new Dictionary<string, string> {
                { "ç","c"},
                { "ğ","g"},
                { "ı","i"},
                { "ö","o"},
                { "ş","s"},
                { "ü","u"},
                { "Ç","C"},
                { "Ğ","G"},
                { "İ","I"},
                { "Ö","O"},
                { "Ş","S"},
                { "Ü","U"}
            };

            foreach (var item in liste)
            {
                value = value.Replace(item.Key, item.Value);
            }

            return value;
        }

        public static string BasvuruFormuAdiGetir(this Enums.BasvuruFormuTurleri value)
        {
            switch (value)
            {
                case Enums.BasvuruFormuTurleri.Bf6111:
                    return "6111";
                case Enums.BasvuruFormuTurleri.Bf687:
                    return "687";
                case Enums.BasvuruFormuTurleri.Bf6645:
                    return "6645";
                case Enums.BasvuruFormuTurleri.Bf7103:
                    return "7103";
                case Enums.BasvuruFormuTurleri.Bf2828:
                    return "2828";
                case Enums.BasvuruFormuTurleri.BfTumu:
                    return "Tüm Teşvikler";
                case Enums.BasvuruFormuTurleri.Bf14857:
                    return "14857";
                case Enums.BasvuruFormuTurleri.Bf7252:
                    return "7252";
                case Enums.BasvuruFormuTurleri.Bf7256:
                    return "7256";
                case Enums.BasvuruFormuTurleri.Bf7316:
                    return "7316";
                case Enums.BasvuruFormuTurleri.Bf3294:
                    return "3294";
                default:
                    return "";
            }
        }

        public static string BoslukluSicilNoyaDonustur(this string value)
        {
            var dizi = new List<string>();
            dizi.Add(value.Substring(0,1));
            dizi.Add(value.Substring(1,4));
            dizi.Add(value.Substring(5,2));
            dizi.Add(value.Substring(7,2));
            dizi.Add(value.Substring(9,7));
            dizi.Add(value.Substring(16,3));
            dizi.Add(value.Substring(19,2));
            dizi.Add(value.Substring(21,2));

            return String.Join(" ",dizi);
        }

        public static T Clone<T>(this T source)
        {
            var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(source);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(serialized);
        }


        public static string TutaraDonustur(this string value)
        {
            if (string.IsNullOrEmpty(value.Trim())) return value;

            var trimmed = value.Trim();

            var noktaLastIndex = trimmed.LastIndexOf('.');
            var virgulLastIndex = trimmed.LastIndexOf(',');

            var ondalikKisim = "00";
            var sayiKisim = trimmed.Replace(".","").Replace(",","");
            
            if (noktaLastIndex > virgulLastIndex)
            {
                var splits = trimmed.Split('.');

                if (splits.Length > 1)
                {
                    if ((splits.Last().Length > 0 && splits.Last().Length < 3) || splits.Last().Length == 4 )
                    {
                        ondalikKisim = splits.Last();

                        if (ondalikKisim.Length == 4 && ondalikKisim.EndsWith("00")) ondalikKisim = ondalikKisim.Substring(0, ondalikKisim.Length - 2);

                        sayiKisim = string.Join("",splits.Take(splits.Length - 1).Select(p=> p.Replace(".","").Replace(",","")));
                    }
                }
            }
            else if (virgulLastIndex > noktaLastIndex)
            {
                var splits = trimmed.Split(',');

                if (splits.Length > 1)
                {
                    if (splits.Last().Length > 0 && splits.Last().Length < 3)
                    {
                        ondalikKisim = splits.Last();
                        sayiKisim = string.Join("", splits.Take(splits.Length - 1).Select(p => p.Replace(".", "").Replace(",", "")));
                    }
                }
            }

            if (sayiKisim == "0" && ondalikKisim == "00") return "0";

            var result = $"{sayiKisim},{ondalikKisim}";

            return result;


        }

    }
}
