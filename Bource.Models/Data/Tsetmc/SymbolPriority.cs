using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Bource.Models.Data.Tsetmc
{
    public class SymbolPriority : MongoDataEntity
    {
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
        public string Symbol { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CapitalIncreaseTime { get; set; }
        public decimal CapitalIncreasePercent { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime EndOfUnderwriting { get; set; }


        public override bool Equals(object obj)
        {
            if (obj is SymbolPriority symbolPriority)
                return InsCode == symbolPriority.InsCode && CapitalIncreaseTime == symbolPriority.CapitalIncreaseTime;

            return false;
        }
    }
}
