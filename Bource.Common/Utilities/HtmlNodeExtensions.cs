using HtmlAgilityPack;
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


        public static decimal ConvertToDecimal(this HtmlNode node)
        {
            return node.InnerText.ConvertToDecimal();

        }

        public static decimal ConvertToDecimal(this string text)
        {
            text = text.FixedNumbersToEn().ToLower().Replace("&nbsp;", "").Replace('(', ' ').Replace(')', ' ').Replace(",", "").Replace("k", "000").Trim();
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            decimal result = 0;
            decimal.TryParse(text, out result);
            return result;

        }


        public static double ConvertToDouble(this HtmlNode node)
        {
            string text = node.InnerText.Replace('(', ' ').Replace(')', ' ').Replace(",", "").Trim();
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            return Convert.ToDouble(text);

        }
    }
}
