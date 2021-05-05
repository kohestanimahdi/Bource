using Bource.Common.Models;
using Bource.Models.Data.Tsetmc;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories
{
    public class SymbolDataRepository : MongoRepository<SymbolData>
    {
        public SymbolDataRepository(MongoDbSetting dbSetting)
            : base(dbSetting)
        {

        }

        public Task<SymbolData> GetLastById(string iid, CancellationToken cancellationToken = default(CancellationToken))
            => Table.Find(i => i.IId == iid).SortByDescending(i => i.LastUpdate).FirstOrDefaultAsync(cancellationToken);
    }
}
