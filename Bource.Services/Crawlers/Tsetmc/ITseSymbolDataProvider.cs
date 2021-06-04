using Bource.Services.Crawlers.Tsetmc.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Tsetmc
{
    public interface ITseSymbolDataProvider
    {
        Task AddOrUpdateSymbols(CancellationToken cancellationToken = default);
        void ClearOneTimeData();
        Dictionary<long, FillSymbolData> GetOneTimeData();
        void SaveSymbolData();
    }
}