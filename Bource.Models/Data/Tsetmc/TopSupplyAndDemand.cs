using Bource.Models.Data.Enums;
using System;

namespace Bource.Models.Data.Tsetmc
{
    public class TopSupplyAndDemand : MongoDataEntity
    {
        public bool IsSupply { get; set; }
        public MarketType Market { get; set; }

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
        public long Price { get; set; }
        public decimal Volume { get; set; }
        public decimal Value { get; set; }
        public long Count { get; set; }
    }
}