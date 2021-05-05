using Bource.Models.Data.Common;
using Bource.Models.Data.Tsetmc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.UnitOfWorks
{
    public interface ITsetmcUnitOfWork
    {
        Task AddCashMarketAtGlance(StockCashMarketAtGlance stockCashMarketAtGlance, OTCCashMarketAtGlance oTCCashMarketAtGlance, CancellationToken cancellationToken = default);
        Task AddOrUpdateSymbolGroups(List<SymbolGroup> symbolGroups, CancellationToken cancellationToken = default);
        Task AddSymbolData(List<SymbolData> data, CancellationToken cancellationToken = default);
        Task AddSymbolsIfNotExists(List<Symbol> symbols, CancellationToken cancellationToken = default);
    }
}
