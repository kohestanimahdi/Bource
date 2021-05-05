using Bource.Common.Models;
using Bource.Models.Data.Tsetmc;

namespace Bource.Data.Informations.Repositories.Tsetmc
{
    public class OTCCashMarketAtGlanceRepository : MongoRepository<OTCCashMarketAtGlance>
    {
        public OTCCashMarketAtGlanceRepository(MongoDbSetting dbSetting)
            : base(dbSetting)
        {

        }
    }
}
