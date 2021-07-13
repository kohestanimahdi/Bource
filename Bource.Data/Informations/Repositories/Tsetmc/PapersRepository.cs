using Bource.Common.Models;
using Bource.Models.Data.Enums;
using Bource.Models.Data.Tsetmc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories.Tsetmc
{
    public class PapersRepository : MongoRepository<Papers>
    {
        public PapersRepository(MongoDbSetting dbSetting) : base(dbSetting)
        {
        }

        public Task<Papers> GetByTypeAsync(PapersTypes papersTypes, CancellationToken cancellationToken = default(CancellationToken))
            => Table.Find(i => i.Type == papersTypes).FirstOrDefaultAsync(cancellationToken);
    }
}
