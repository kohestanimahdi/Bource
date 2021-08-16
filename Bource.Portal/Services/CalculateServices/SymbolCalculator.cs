using Bource.Common.Models;
using Bource.Data.Informations.UnitOfWorks;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Portal.Services.CalculateServices
{
    public class SymbolCalculator : ISymbolCalculator, IScopedDependency
    {
        private readonly IDistributedCache distributedCache;
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;

        public SymbolCalculator(IDistributedCache distributedCache, ITsetmcUnitOfWork tsetmcUnitOfWork)
        {
            this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
        }

        public async Task GetTurnoverAveragesAsync(long insCode, CancellationToken cancellationToken = default)
        {
            // should restore from cache and save for 5 minutes

            var historyPrices = await tsetmcUnitOfWork.GetSymbolDataHistoryAsync(insCode, Models.Data.Enums.ClosingPriceTypes.NoPriceAdjustment, cancellationToken);
            var result = new
            {
                Day = historyPrices.Any() ? historyPrices.First().QTotTran5J : 0,
                Week = historyPrices.Count >= 5 ? historyPrices.Take(5).Average(i => i.QTotTran5J) : 0,
                Month = historyPrices.Count >= 26 ? historyPrices.Take(26).Average(i => i.QTotTran5J) : 0,
                ThreeMonth = historyPrices.Count >= 78 ? historyPrices.Take(78).Average(i => i.QTotTran5J) : 0,
                SixMonth = historyPrices.Count >= 156 ? historyPrices.Take(156).Average(i => i.QTotTran5J) : 0,
                Year = historyPrices.Count >= 312 ? historyPrices.Take(312).Average(i => i.QTotTran5J) : 0,
            };
        }
    }
}
