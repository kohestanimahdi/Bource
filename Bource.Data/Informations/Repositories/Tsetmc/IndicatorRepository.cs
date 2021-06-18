using Bource.Common.Models;
using Bource.Models.Data.Tsetmc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories.Tsetmc
{
    public class IndicatorRepository : MongoRepository<Indicator>
    {
        public IndicatorRepository(MongoDbSetting dbSetting) : base(dbSetting)
        {
        }

        public async Task AddOrUpdateAsync(List<Indicator> indicators, CancellationToken cancellationToken = default(CancellationToken))
        {
            var oldIndicators = await GetAllAsync(cancellationToken);
            foreach (var indicator in indicators)
            {
                if (!oldIndicators.Any(i => i.Equals(indicator)))
                    await AddAsync(indicator, cancellationToken);
                else
                    await UpdateAsync(indicator, cancellationToken);
            }
        }
    }
}