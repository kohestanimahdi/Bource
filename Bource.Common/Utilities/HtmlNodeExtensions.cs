using HtmlAgilityPack;
using MD.PersianDateTime.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bource.Common.Utilities
{
    public static class HtmlNodeExtensions
    {
        public static double ConvertToNegativePositiveNumber(this HtmlNode node)
            => node.ConvertToDouble() * (node.HasClass("mn") ? -1 : 1);

        public static decimal ConvertToNegativePositiveDecimal(this HtmlNode node)
            => node.ConvertToDecimal() * (node.HasClass("mn") ? -1 : 1);

        public static string GetText(this HtmlNode node)
            => node.InnerText.Replace("&nbsp;", "").Trim().FixPersianLetters();

        public static string GetNumberAsText(this string text)
            => text.Trim().FixedNumbersToEn().ToLower().Replace("&nbsp;", "").Replace('(', ' ').Replace(')', ' ').Replace(",", "").Replace("k", "000").Replace("B", "000000000");

        public static DateTime GetAsDateTime(this HtmlNode node)
        {
            PersianDateTime time;
            if (!PersianDateTime.TryParse($"14{node.InnerText}", out time))
                time = PersianDateTime.Now;

            return time.ToDateTime();
        }
        public static long ConvertToLong(this HtmlNode node)
        => node.InnerText.ConvertToLong();

        public static decimal ConvertToDecimal(this HtmlNode node)
        => node.InnerText.ConvertToDecimal();

        public static decimal ConvertToDecimal(this string text)
        {
            text = text.GetNumberAsText();
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            decimal result = 0;
            decimal.TryParse(text, out result);
            return result;

        }

        public static long ConvertToLong(this string text)
        {
            text = text.GetNumberAsText();
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            long result = 0;
            Int64.TryParse(text, out result);
            return result;

        }

        //public static 

        public static double ConvertToDouble(this HtmlNode node)
        {
            string text = node.InnerText.GetNumberAsText();
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            return Convert.ToDouble(text);

        }

        public static decimal GetAttributeValueAsDecimal(this HtmlNode node, string attributeName = "title")
        {

            var chNode = node.SelectSingleNode("div");

            if (chNode is not null && chNode.Attributes.Any(i => i.Name == attributeName))
                return chNode.Attributes[attributeName].Value.ConvertToDecimal();

            return node.ConvertToDecimal();
        }
    }
}
