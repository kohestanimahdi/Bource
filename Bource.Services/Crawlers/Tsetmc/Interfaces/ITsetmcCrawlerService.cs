﻿using Bource.Models.Data.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Tsetmc
{
    public interface ITsetmcCrawlerService
    {
        Task FillOneTimeDataAsync(CancellationToken cancellationToken = default);
        Task GetAllCapitalIncreaseAsync(CancellationToken cancellationToken = default);
        Task GetAllNaturalAndLegalEntityAsync(CancellationToken cancellationToken = default);
        Task GetChangeOfSharesOfActiveShareHoldersAsync(CancellationToken cancellationToken = default);
        Task GetLatestSymbolDataAsync(CancellationToken cancellationToken = default);
        Task GetMarketAtGlanceAsync(CancellationToken cancellationToken = default);
        Task GetMarketWatcherMessage(CancellationToken cancellationToken = default);
        Task GetOrUpdateSymbolGroupsAsync(CancellationToken cancellationToken = default);
        Task GetSelectedIndicatorAsync(CancellationToken cancellationToken = default);
        Task<List<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken = default);
        Task GetSymbolsShareHoldersAsync(CancellationToken cancellationToken = default);
        Task GetTopSupplyAndDemandAsync(CancellationToken cancellationToken = default);
        Task GetValueOfMarketAsync(CancellationToken cancellationToken = default);
        Task UpdateSymbolsAsync(CancellationToken cancellationToken = default);
    }
}