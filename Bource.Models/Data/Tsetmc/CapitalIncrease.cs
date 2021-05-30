using Bource.Common.Utilities;
using HtmlAgilityPack;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Bource.Models.Data.Tsetmc
{
    public class CapitalIncrease : MongoDataEntity
    {
        public CapitalIncrease()
        {
        }

        public CapitalIncrease(long insCode, HtmlNodeCollection nodes)
        {
            Date = nodes[0].GetAsDateTime();
            NewStock = nodes[1].GetAttributeValueAsDecimal();
            OldStock = nodes[2].GetAttributeValueAsDecimal();
            InsCode = insCode;
        }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }

        public decimal NewStock { get; set; }
        public decimal OldStock { get; set; }

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