﻿using Bource.Models.Data.Common;
using Bource.Models.Data.Tsetmc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.UnitOfWorks
{
    public interface ITsetmcUnitOfWork
    {
        Task AddCapitalIncreaseAsync(string iid, List<CapitalIncrease> entities, CancellationToken cancellationToken = default);

        Task AddCashMarketAtGlance(StockCashMarketAtGlance stockCashMarketAtGlance, OTCCashMarketAtGlance oTCCashMarketAtGlance, CancellationToken cancellationToken = default);

        Task AddMarketWatcherMessageIfNotExistsRangeAsync(List<MarketWatcherMessage> messages, CancellationToken cancellationToken = default);

        Task AddNewNaturalAndLegalEntity(string iid, List<NaturalAndLegalEntity> entities, CancellationToken cancellationToken = default);

        Task AddOrUpdateSymbolGroups(List<SymbolGroup> symbolGroups, CancellationToken cancellationToken = default);
        Task AddSelectedIndicatorsAsync(List<SelectedIndicator> selectedIndicators, CancellationToken cancellationToken = default);
        Task AddSymbolData(List<SymbolData> data, CancellationToken cancellationToken = default);

        Task AddSymbolsIfNotExists(List<Symbol> symbols, CancellationToken cancellationToken = default);

        Task AddTopSupplyAndDemandRangeAsync(List<TopSupplyAndDemand> values, CancellationToken cancellationToken = default);

        Task AddValuesOfMarketsIfNotExistsRangeAsync(List<ValueOfMarket> values, CancellationToken cancellationToken = default);

        Task<List<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken = default);

        Task UpdateSymbolsAsync(Symbol symbol, CancellationToken cancellationToken = default);
    }
}