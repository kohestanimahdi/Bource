using Bource.Common.Utilities;
using Bource.Models.Data.Common;
using HtmlAgilityPack;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bource.Models.Data.Tsetmc
{
    public class Indicator : MongoDataEntity
    {
        public Indicator()
        {

        }
        public Indicator(HtmlNodeCollection tds, long insCode, HtmlNodeCollection symbolsNodes, List<Symbol> symbols)
        {
            Title = tds[0].GetText();
            InsCode = insCode;

            Symbols = new();
            if (symbolsNodes is not null)
                foreach (var tr in symbolsNodes)
                {
                    var sign = tr.SelectNodes("td")[0].GetText();
                    var symbol = symbols.FirstOrDefault(i => i.Sign.Equals(sign));
                    if (symbol is not null)
                        Symbols.Add(new IndicatorSymbol(symbol));
                }
        }
        public string Title { get; set; }

        public long InsCode
        {
            get
            {
                return Convert.ToInt64(InsCodeValue);
            }
            set
            {
                InsCodeValue = value.ToString();
            }
        }

        public string InsCodeValue { get; set; }
        public List<IndicatorSymbol> Symbols { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Indicator indicator)
                return Title == indicator.Title && InsCode == indicator.InsCode;
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class IndicatorSymbol
    {
        public IndicatorSymbol(Symbol symbol)
        {
            Sign = symbol.Sign;
            Name = symbol.Name;
            InsCode = symbol.InsCode;
            InsCodeValue = symbol.InsCodeValue;
        }

        public string Sign { get; set; }
        public string Name { get; set; }
        public long InsCode
        {
            get
            {
                return Convert.ToInt64(InsCodeValue);
            }
            set
            {
                InsCodeValue = value.ToString();
            }
        }

        public string InsCodeValue { get; set; }
    }
}