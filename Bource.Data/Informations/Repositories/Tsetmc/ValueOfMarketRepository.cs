using Bource.Common.Models;
using Bource.Models.Data.Enums;
using Bource.Models.Data.Tsetmc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories.Tsetmc
{
    public class ValueOfMarketRepository : MongoRepository<ValueOfMarket>
    {
        public ValueOfMarketRepository(MongoDbSetting dbSetting) : base(dbSetting)
        {
        }

        public Task<List<ValueOfMarket>> GetValueOfMarketAsync(MarketType market, CancellationToken cancellationToken = default(CancellationToken))
        => Table.Find(i => i.Market == market).ToListAsync(cancellationToken);
    }
}