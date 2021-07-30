using Bource.Common.Models;
using Bource.Models.Data.Common;
using Bource.Models.Data.Enums;
using Bource.Models.Data.Tsetmc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.UnitOfWorks
{
    public interface ITsetmcUnitOfWork
    {
        Task AddAdtiveSymbolShareHoldersAsync(List<ActiveSymbolShareHolder> items, CancellationToken cancellationToken = default);

        Task AddCapitalIncreaseAsync(long insCode, List<CapitalIncrease> entities, CancellationToken cancellationToken = default);

        Task AddCashMarketAtGlance(CashMarketAtGlance stockCashMarketAtGlance, CashMarketAtGlance oTCCashMarketAtGlance, CancellationToken cancellationToken = default);

        Task AddOrUpdateIndicatorsAsync(List<Indicator> indicators, CancellationToken cancellationToken = default);

        Task AddMarketWatcherMessageIfNotExistsRangeAsync(List<MarketWatcherMessage> messages, CancellationToken cancellationToken = default);

        Task AddNewNaturalAndLegalEntity(long insCode, List<NaturalAndLegalEntity> entities, CancellationToken cancellationToken = default);

        Task AddOrUpdateSymbolAsync(Symbol symbol, CancellationToken cancellationToken = default);

        Task AddOrUpdateSymbolGroups(List<SymbolGroup> symbolGroups, CancellationToken cancellationToken = default);

        Task AddSelectedIndicatorsAsync(List<SelectedIndicator> selectedIndicators, CancellationToken cancellationToken = default);

        Task AddSymbolData(List<SymbolData> data, CancellationToken cancellationToken = default);

        Task AddSymbolsRangeAsync(List<Symbol> symbols, CancellationToken cancellationToken = default);

        Task AddTodaysSymbolShareHoldersAsync(long insCode, List<SymbolShareHolder> items, CancellationToken cancellationToken = default);

        Task AddTopSupplyAndDemandRangeAsync(List<TopSupplyAndDemand> values, CancellationToken cancellationToken = default);

        Task AddValuesOfMarketsIfNotExistsRangeAsync(List<ValueOfMarket> values, CancellationToken cancellationToken = default);

        Task AppendClosingPriceInfoAsync(List<ClosingPriceInfo> infos, CancellationToken cancellationToken = default);

        Task<List<ClosingPriceInfo>> GetClosingPriceInfosAsync(long insCode, ClosingPriceTypes? closingPriceTypes, CancellationToken cancellationToken = default);

        Task<List<Indicator>> GetIndicatorsAsync(CancellationToken cancellationToken = default);

        Task<List<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken = default);

        Task<List<Symbol>> GetSymbolsByTypeAsync(IEnumerable<SymbolExistInType> existInTypes, CancellationToken cancellationToken = default);

        Task UpdateSymbolAsync(Symbol symbol, CancellationToken cancellationToken = default);

        Task<List<SymbolGroup>> GetSymbolGroupsAsync(CancellationToken cancellationToken = default);

        Task UpdateSymbolGroupAsync(SymbolGroup group, CancellationToken cancellationToken = default);
    }
}