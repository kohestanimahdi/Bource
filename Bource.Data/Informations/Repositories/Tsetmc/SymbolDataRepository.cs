﻿using Bource.Common.Models;
using Bource.Models.Data.Tsetmc;
using MongoDB.Driver;
using System;
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
            await base.DeleteRangeAsync(items, cancellationToken);
        }
    }
}