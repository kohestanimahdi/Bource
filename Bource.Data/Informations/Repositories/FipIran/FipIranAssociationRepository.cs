using Bource.Common.Models;
using Bource.Models.Data.FipIran;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories.FipIran
{
    public class FipIranAssociationRepository : MongoRepository<FipIranAssociation>
    {
        public FipIranAssociationRepository(MongoDbSetting dbSetting) : base(dbSetting)
        {
        }

        public Task<List<FipIranAssociation>> GetByDateAsync(DateTime from, CancellationToken cancellationToken = default(CancellationToken))
            => Table.Find(i => i.Time >= from).ToListAsync(cancellationToken);
    }
}