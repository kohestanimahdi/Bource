using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Bource.Models.Data.Common
{
    public class SymbolSubGroup : DataEntity
    {
        public string Title { get; set; }
        public int Code { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string SymbolGroupId { get; set; }
    }
}
