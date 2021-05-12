using Bource.Models.Data.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Bource.Models.Data.Tsetmc
{
    public class ValueOfMarket : MongoDataEntity
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }

        public decimal Value { get; set; }
        public MarketType Market { get; set; }
    }
}