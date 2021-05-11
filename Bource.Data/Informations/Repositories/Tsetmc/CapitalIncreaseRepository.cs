using Bource.Common.Models;
using Bource.Models.Data.Tsetmc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories.Tsetmc
{
    public class CapitalIncreaseRepository : MongoRepository<CapitalIncrease>
    {
        public CapitalIncreaseRepository(MongoDbSetting dbSetting) : base(dbSetting)
        {
        }

        public Task<List<CapitalIncrease>> GetCapitalIncreaseOfSymbolAsync(string iid, CancellationToken cancellationToken = default(CancellationToken))
            => Table.Find(i => i.IId == iid).ToListAsync(cancellationToken);
    }
}
