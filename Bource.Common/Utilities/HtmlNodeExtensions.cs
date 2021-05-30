using HtmlAgilityPack;
using MD.PersianDateTime.Standard;
using System;
using System.Linq;

namespace Bource.Common.Utilities
{
    public static class HtmlNodeExtensions
    {
        public static double ConvertToNegativePositiveNumber(this HtmlNode node)
            => node.ConvertToDouble() * (node.HasClass("mn") ? -1 : 1);

        public static decimal ConvertToNegativePositiveDecimal(this HtmlNode node)
            => node.ConvertToDecimal() * (node.HasClass("mn") ? -1 : 1);

        public static string GetText(this HtmlNode node)
            => node.InnerText.Replace("&nbsp;", "").Replace('‌', ' ').Trim().FixPersianLetters();

        public static string GetNumberAsText(this string text)
            => text.Trim().FixedNumbersToEn().ToLower().Replace("&nbsp;", "").Replace('(', ' ').Replace(')', ' ').Replace(",", "").Replace("k", "000").Replace("B", "000000000");

        public static DateTime GetAsDateTime(this HtmlNode node, string prefix = "", DateTime? defaultValue = null)
        => node.InnerText.GetAsDateTime(prefix, defaultValue);

        public static DateTime GetAsDateTime(this string text, string prefix = "", DateTime? defaultValue = null)
        {
            PersianDateTime time;
            if (!PersianDateTime.TryParse($"{prefix}{text}", out time))
            {
                if (defaultValue.HasValue)
                    return defaultValue.Value;
                else
                    time = PersianDateTime.Now;
            }

            return time.ToDateTime();
        }

        public static long ConvertToLong(this HtmlNode node)
        {
            try
            {
                return node.SelectSingleNode("text()").InnerText.ConvertToLong();
            }
            catch (NullReferenceException)
            {
                return node.InnerText.ConvertToLong();
            }
        }

        public static decimal ConvertToDecimal(this HtmlNode node)
        {
            try
            {
                return node.SelectSingleNode("text()").InnerText.ConvertToDecimal();
            }
            catch (NullReferenceException)
            {
                return node.InnerText.ConvertToDecimal();
            }
        }

        public static decimal ConvertToDecimal(this string text)
        {
            text = text.GetNumberAsText();
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            decimal result = 0;
            decimal.TryParse(text, out result);
            return result;
        }

        public static decimal RegexConvertToDecimal(this string text)
        {
            text = text.GetNumberAsText();
            var regex = new System.Text.RegularExpressions.Regex(@"\'([0-9]*|([0-9]*.[0-9]*))\'");
            if (!regex.IsMatch(text))
                return 0;

            return regex.Match(text).Value.Replace("\"", "").Replace("\'", "").ConvertToDecimal();
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
            string text;
            if (node.SelectSingleNode("text()") is not null)
                text = node.SelectSingleNode("text()").InnerText.GetNumberAsText();
            else
                text = node.InnerText.GetNumberAsText();

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

        public static string GetQueryString(this HtmlNode node, string key, string baseUrl = "")
        {
            var uri = new Uri(baseUrl + node.SelectSingleNode("a").Attributes["href"].Value);
            var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);

            return queryDictionary[key];
        }
    }
}