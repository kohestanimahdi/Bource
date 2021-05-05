using Bource.Common.Models;
using Bource.Models.Data.Tsetmc;

namespace Bource.Data.Informations.Repositories.Tsetmc
{
    public class StockCashMarketAtGlanceRepository : MongoRepository<StockCashMarketAtGlance>
    {
        public StockCashMarketAtGlanceRepository(MongoDbSetting dbSetting)
            : base(dbSetting)
        {

        }
    }
}
