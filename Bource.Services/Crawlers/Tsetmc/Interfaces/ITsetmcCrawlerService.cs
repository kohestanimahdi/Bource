using Bource.Models.Data.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Tsetmc
{
    public interface ITsetmcCrawlerService
    {
        Task CompleteSymbolData(CancellationToken cancellationToken = default);
        Task FillOneTimeDataAsync(CancellationToken cancellationToken = default);

        Task GetAllCapitalIncreaseAsync(CancellationToken cancellationToken = default);

        Task GetAllNaturalAndLegalEntityAsync(CancellationToken cancellationToken = default);

        Task GetChangeOfSharesOfActiveShareHoldersAsync(CancellationToken cancellationToken = default);

        //Task GetIndicators(CancellationToken cancellationToken = default);
        Task GetMarketAtGlanceScheduleEverySecondAsync(CancellationToken cancellationToken = default);

        Task GetMarketWatcherMessage(CancellationToken cancellationToken = default);

        Task GetOrUpdateSymbolGroupsAsync(CancellationToken cancellationToken = default);

        Task GetSelectedIndicatorEverySecondAsync(CancellationToken cancellationToken = default);

        Task<List<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken = default);

        Task GetSymbolsOfIndicatorsAsync(CancellationToken cancellationToken = default);

        Task GetSymbolsShareHoldersAsync(CancellationToken cancellationToken = default);

        Task GetTopSupplyAndDemandAsync(CancellationToken cancellationToken = default);

        Task GetTopSupplyAndDemandEverySecondAsync(CancellationToken cancellationToken = default);

        Task GetValueOfMarketAsync(CancellationToken cancellationToken = default);

        Task ScheduleLatestSymbolDataEverySecondAsync(CancellationToken cancellationToken = default);

        Task SetMarketStatus(bool? status, CancellationToken cancellationToken = default);

        Task UpdateSymbolsAsync(CancellationToken cancellationToken = default);
    }
}