﻿using Bource.Common.Utilities;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Bource.Models.Data.Tsetmc
{
    public class SymbolShareHolder : MongoDataEntity
    {
        public SymbolShareHolder()
        {

        }
        public SymbolShareHolder(long insCode, HtmlNode row)
        {
            var tds = row.SelectNodes("td");
            InsCode = insCode;
            Name = tds[0].GetText();
            Share = tds[1].GetAttributeValueAsDecimal();
            Percent = tds[2].ConvertToDouble();
            ShareChange = tds[3].ConvertToDecimal();

            var onClick = row.Attributes["onclick"].Value;
            var regex = new Regex("\'[0-9]*,.*\'");
            var match = regex.Match(onClick);
            if (match.Success)
                ShareId = match.Value.Replace("\'", "").Split(',')[0];
        }
        public long InsCode { get; set; }
        public string Name { get; set; }
        public string ShareId { get; set; }
        public decimal Share { get; set; }
        public double Percent { get; set; }
        public decimal ShareChange { get; set; }
    }
}