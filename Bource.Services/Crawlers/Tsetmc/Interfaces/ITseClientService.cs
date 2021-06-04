﻿using Bource.Models.Data.Common;
using Bource.Models.Data.Enums;
using Bource.Models.Data.Tsetmc;
using Bource.Services.Crawlers.Tsetmc.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Tsetmc
{
    public interface ITseClientService
    {
        List<ClosingPriceInfo> ConvertToClosingPriceType(Symbol symbol, List<ClosingPriceInfo> cp, List<TseShareInfo> tseShares, ClosingPriceTypes types);
        Task GetInsturmentsClosingPriceAsync(CancellationToken cancellationToken = default);
        Task<(List<Symbol>, List<TseShareInfo>)> GetSymbolAndSharingAsync();
    }
}