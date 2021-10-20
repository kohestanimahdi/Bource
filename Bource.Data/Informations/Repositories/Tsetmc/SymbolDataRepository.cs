using Bource.Common.Models;
using Bource.Models.Data.Tsetmc;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories
{
    public class SymbolDataRepository : MongoRepository<SymbolData>
    {
        public SymbolDataRepository(MongoDbSetting dbSetting)
            : base(dbSetting)
        {
            CreateAscendingIndex(nameof(SymbolData.LastUpdate));
        }

        public Task<SymbolData> GetLastById(long insCode, CancellationToken cancellationToken = default(CancellationToken))
            => Table.Find(i => i.InsCode == insCode).SortByDescending(i => i.LastUpdate).FirstOrDefaultAsync(cancellationToken);

        public async Task RemoveOldSymbolDataAsync(DateTime olderThan, CancellationToken cancellationToken = default)
        {
            var items = await Table.Find(i => i.CreateDate < olderThan).ToListAsync(cancellationToken);
            var result = items.GroupBy(i => i.InsCode);
            foreach (var item in result)
            {
                var removedItem = item.OrderByDescending(i => i.LastUpdate).ToList();
                if (removedItem.Count > 1)
                {
                    removedItem = removedItem.Skip(1).ToList();
                    await base.DeleteRangeAsync(removedItem, cancellationToken);
                }
            }

        }
    }
}