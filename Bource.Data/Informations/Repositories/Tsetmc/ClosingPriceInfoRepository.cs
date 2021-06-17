using Bource.Common.Models;
using Bource.Models.Data.Enums;
using Bource.Models.Data.Tsetmc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories.Tsetmc
{
    public class ClosingPriceInfoRepository : MongoRepository<ClosingPriceInfo>
    {
        public ClosingPriceInfoRepository(MongoDbSetting dbSetting) : base(dbSetting)
        {
        }

        public async Task AppendAsync(long InsCode, ClosingPriceTypes closingPriceTypes, List<ClosingPriceInfo> infos, CancellationToken cancellationToken = default(CancellationToken))
        {
            var lastItem = await Table.Find(i => i.Type == closingPriceTypes && i.InsCode == InsCode).SortByDescending(i => i.DEven).FirstOrDefaultAsync(cancellationToken);
            var savedItem = infos.Where(i => i.DEven > (lastItem?.DEven ?? 0)).ToList();
            await AddRangeAsync(savedItem, cancellationToken);
        }
    }
}