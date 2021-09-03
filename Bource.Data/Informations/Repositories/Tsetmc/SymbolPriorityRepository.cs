using Bource.Common.Models;
using Bource.Models.Data.Tsetmc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories.Tsetmc
{
    public class SymbolPriorityRepository : MongoRepository<SymbolPriority>
    {
        public SymbolPriorityRepository(MongoDbSetting dbSetting) : base(dbSetting)
        {
        }

        public async Task AddIfNotExistsAsync(List<SymbolPriority> symbolPriorities, CancellationToken cancellationToken = default)
        {
            var existsSymbolPriorities = await GetAllAsync(cancellationToken);
            foreach (var symbolPriority in symbolPriorities)
            {
                if (!existsSymbolPriorities.Any(i => i.Equals(symbolPriority)))
                    await AddAsync(symbolPriority);
            }
        }
    }
}

