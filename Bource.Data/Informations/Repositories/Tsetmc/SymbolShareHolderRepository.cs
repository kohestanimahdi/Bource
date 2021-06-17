using Bource.Common.Models;
using Bource.Models.Data.Tsetmc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories.Tsetmc
{
    public class SymbolShareHolderRepository : MongoRepository<SymbolShareHolder>
    {
        public SymbolShareHolderRepository(MongoDbSetting dbSetting) : base(dbSetting)
        {
        }

        public async Task AddTodaysSymbolShareHoldersAsync(long insCode, List<SymbolShareHolder> items, CancellationToken cancellationToken = default(CancellationToken))
        {
            var todays = await Table.Find(i => i.CreateDate > DateTime.Today && i.InsCode == insCode).ToListAsync(cancellationToken);
            await DeleteRangeAsync(todays, cancellationToken);
            await AddRangeAsync(items, cancellationToken);
        }
    }
}