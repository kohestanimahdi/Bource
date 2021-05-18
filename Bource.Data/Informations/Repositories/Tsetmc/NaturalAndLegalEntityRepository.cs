using Bource.Common.Models;
using Bource.Models.Data.Tsetmc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories.Tsetmc
{
    public class NaturalAndLegalEntityRepository : MongoRepository<NaturalAndLegalEntity>
    {
        public NaturalAndLegalEntityRepository(MongoDbSetting dbSetting) : base(dbSetting)
        {
        }

        public Task<List<NaturalAndLegalEntity>> GetNaturalAndLegalEntityOfSymbolAsync(long insCode, CancellationToken cancellationToken = default(CancellationToken))
            => Table.Find(i => i.InsCode == insCode).ToListAsync(cancellationToken);
    }
}