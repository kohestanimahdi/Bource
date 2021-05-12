using Bource.Common.Utilities;
using HtmlAgilityPack;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Bource.Models.Data.FipIran
{
    public class FipIranAssociation : MongoDataEntity
    {
        public FipIranAssociation()
        {
        }

        public FipIranAssociation(HtmlNode node)
        {
            var tds = node.SelectNodes("td");
            Symbol = tds[0].GetText();
            PublishDate = tds[1].GetAsDateTime();
            Title = tds[2].GetText();
            Time = $"{tds[3].GetText()} {tds[4].GetText()}".GetAsDateTime();
            Address = tds[5].GetText();
        }

        public string Symbol { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime PublishDate { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }

        public string Title { get; set; }
        public string Address { get; set; }
    }
}