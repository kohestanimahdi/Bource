using Bource.Common.Models;
using Bource.Models.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories
{
    public class SymbolGroupRepository : MongoRepository<SymbolGroup>
    {
        public SymbolGroupRepository(MongoDbSetting dbSetting)
            : base(dbSetting)
        {

        }

        public async Task AddOrUpdateSymbolGroups(List<SymbolGroup> symbolGroups, CancellationToken cancellationToken = default(CancellationToken))
        {
            var allGroups = await GetAllAsync(cancellationToken);

            foreach (var newGroup in symbolGroups)
            {
                var group = allGroups.FirstOrDefault(i => i.Code == newGroup.Code);
                if (group is not null)
                {
                    group.Title = newGroup.Title;
                    await UpdateAsync(group, cancellationToken);
                }
                else
                    await AddAsync(newGroup);
            }
        }
    }
}
