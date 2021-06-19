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
        public Indicator(string title, long insCode)
        {
            Title = title;
            InsCode = insCode;
            Symbols = new();
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

        public void AddOrUpdateSymbol(IndicatorSymbol symbol)
        {
            if (Symbols is null)
                Symbols = new();

            if (!Symbols.Any(i => i.InsCode == symbol.InsCode))
                Symbols.Add(symbol);
        }
    }

    public class IndicatorSymbol
    {
        public IndicatorSymbol(string sign, string name, long insCode)
        {
            Sign = sign;
            Name = name;
            InsCode = insCode;
        }
        public IndicatorSymbol(Symbol symbol)
            : this(symbol.Sign, symbol.Name, symbol.InsCode)
        {
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