using Bource.Common.Models;
using Bource.Models.Data.Enums;
using Bource.Models.Data.FipIran;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories.FipIran
{
    public class FipIranNewsRepository : MongoRepository<FipIranNews>
    {
        public FipIranNewsRepository(MongoDbSetting dbSetting) : base(dbSetting)
        {
        }

        public Task<List<FipIranNews>> GetByDateAsync(DateTime from, FipIranNewsTypes types, CancellationToken cancellationToken = default(CancellationToken))
            => Table.Find(i => i.Time >= from && i.Type == types).ToListAsync(cancellationToken);
    }
}