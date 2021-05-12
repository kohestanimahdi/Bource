using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Bource.Models.Data
{
    public interface IMongoDataEntity
    {
    }

    public class MongoDataEntity : IMongoDataEntity
    {
        public MongoDataEntity()
        {
            CreateDate = DateTime.Now;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateDate { get; set; }
    }
}