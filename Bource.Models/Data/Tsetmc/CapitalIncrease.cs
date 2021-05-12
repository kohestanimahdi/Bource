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

        public CapitalIncrease(string iid, HtmlNodeCollection nodes)
        {
            Date = nodes[0].GetAsDateTime();
            NewStock = nodes[1].GetAttributeValueAsDecimal();
            OldStock = nodes[2].GetAttributeValueAsDecimal();
            IId = iid;
        }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }

        public decimal NewStock { get; set; }
        public decimal OldStock { get; set; }

        public string IId { get; set; }
    }
}