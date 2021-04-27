using Bource.Common.Models;
using Bource.Data.Informations.Repositories;
using Bource.Models.Data.Common;
using Bource.Models.Data.Tsetmc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.UnitOfWorks
{
    public class TsetmcUnitOfWork : ITsetmcUnitOfWork, ISingletonDependency
    {
        private readonly SymbolGroupRepository symbolGroupRepository;
        private readonly SymbolDataRepository symbolDataRepository;
        private readonly SymbolRepository symbolRepository;

        public TsetmcUnitOfWork(IOptionsSnapshot<ApplicationSetting> options)
        {
            symbolGroupRepository = new SymbolGroupRepository(options.Value.mongoDbSetting);
            symbolDataRepository = new SymbolDataRepository(options.Value.mongoDbSetting);
            symbolRepository = new SymbolRepository(options.Value.mongoDbSetting);
        }

        public TsetmcUnitOfWork(MongoDbSetting mongoDbSetting)
        {
            symbolGroupRepository = new SymbolGroupRepository(mongoDbSetting);
            symbolDataRepository = new SymbolDataRepository(mongoDbSetting);
            symbolRepository = new SymbolRepository(mongoDbSetting);
        }

        public Task AddOrUpdateSymbolGroups(List<SymbolGroup> symbolGroups, CancellationToken cancellationToken = default(CancellationToken))
            => symbolGroupRepository.AddOrUpdateSymbolGroups(symbolGroups, cancellationToken);

        public async Task AddSymbolsIfNotExists(List<Symbol> symbols, CancellationToken cancellationToken = default(CancellationToken))
        {
            var oldSymbols = await symbolRepository.GetAllAsync(cancellationToken);
            foreach (var symbol in symbols)
            {
                var oldSymbol = oldSymbols.SingleOrDefault(i => i.IId == symbol.IId);
                if (oldSymbol is null)
                    await symbolRepository.AddAsync(symbol, cancellationToken);
            }
        }

        public async Task AddSymbolData(List<SymbolData> data, CancellationToken cancellationToken = default(CancellationToken))
        {
            var symbols = data.Select(i => new Symbol
            {
                IId = i.IId,
                Code12 = i.InsCode,
                Name = i.Name,
                Sign = i.Symbol,
                GroupId = i.SymbolGroup
            }).ToList();

            await AddSymbolsIfNotExists(symbols, cancellationToken);

            var todayItems = await symbolDataRepository.Table.Find(i => i.LastUpdate >= DateTime.Today).ToListAsync(cancellationToken);
            if (todayItems is not null && todayItems.Any())
                foreach (var item in data)
                {
                    var symbolData = todayItems.Where(i => i.IId == item.IId).OrderByDescending(i => i.LastUpdate).FirstOrDefault();
                    if (symbolData is null || symbolData.NumberOfTransaction != item.NumberOfTransaction)
                        await symbolDataRepository.AddAsync(item, cancellationToken);
                }
            else
                await symbolDataRepository.AddRangeAsync(data, cancellationToken);
        }
    }
}
