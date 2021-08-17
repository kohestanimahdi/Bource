using Bource.Common.Models;
using Bource.Common.Utilities;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Portal.ViewModels.Dtos.Responses;
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

        /// <summary>
        /// محاسبه میانگین‌های حجم معاملات
        /// </summary>
        /// <param name="insCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AveragesResponse> GetTurnoverAveragesAsync(long insCode, CancellationToken cancellationToken = default)
        {
            AveragesResponse result = null;
            result = await distributedCache.GetValueAsync<AveragesResponse>($"TurnoverAverages-{insCode}", cancellationToken);

            if (result is null)
            {
                var historyPrices = await tsetmcUnitOfWork.GetSymbolDataHistoryAsync(insCode, Models.Data.Enums.ClosingPriceTypes.NoPriceAdjustment, cancellationToken);
                result = new AveragesResponse
                {
                    Day = historyPrices.Any() ? historyPrices.First().QTotTran5J : 0,
                    Week = historyPrices.Count >= 5 ? Math.Round(historyPrices.Take(5).Average(i => i.QTotTran5J)) : 0,
                    Month = historyPrices.Count >= 26 ? Math.Round(historyPrices.Take(26).Average(i => i.QTotTran5J)) : 0,
                    ThreeMonth = historyPrices.Count >= 78 ? Math.Round(historyPrices.Take(78).Average(i => i.QTotTran5J)) : 0,
                    SixMonth = historyPrices.Count >= 156 ? Math.Round(historyPrices.Take(156).Average(i => i.QTotTran5J)) : 0,
                    Year = historyPrices.Count >= 312 ? Math.Round(historyPrices.Take(312).Average(i => i.QTotTran5J)) : 0,
                };

                await distributedCache.SetValueAsync($"TurnoverAverages-{insCode}", result, 5, cancellationToken);
            }

            return result;
        }


    }
}
