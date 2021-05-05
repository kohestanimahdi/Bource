using Bource.Models.Data.Enums;

namespace Bource.Models.Data.Tsetmc
{
    public class TopSupplyAndDemand : MongoDataEntity
    {
        public bool IsSupply { get; set; }
        public MarketType Market { get; set; }
        public string IId { get; set; }
        public string Symbol { get; set; }
        public long Price { get; set; }
        public decimal Volume { get; set; }
        public decimal Value { get; set; }
        public long Count { get; set; }
    }
}
