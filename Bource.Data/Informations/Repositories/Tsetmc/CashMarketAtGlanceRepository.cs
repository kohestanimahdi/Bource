using Bource.Common.Models;
using Bource.Models.Data.Tsetmc;

namespace Bource.Data.Informations.Repositories.Tsetmc
{
    public class CashMarketAtGlanceRepository : MongoRepository<CashMarketAtGlance>
    {
        public CashMarketAtGlanceRepository(MongoDbSetting dbSetting)
            : base(dbSetting)
        {
        }
    }
}