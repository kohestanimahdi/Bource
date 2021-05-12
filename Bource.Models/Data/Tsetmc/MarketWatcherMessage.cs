using Bource.Models.Data.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Bource.Models.Data.Tsetmc
{
    public class MarketWatcherMessage : MongoDataEntity
    {
        public MarketType Market { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is MarketWatcherMessage message)
            {
                return Title == message.Title && Description == message.Description && Time == message.Time && Market == message.Market;
            }
            return false;
        }
    }
}