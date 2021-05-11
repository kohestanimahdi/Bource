using Bource.Common.Models;
using Bource.Data.Informations.Repositories;
using Bource.Data.Informations.Repositories.Tsetmc;
using Bource.Models.Data.Common;
using Bource.Models.Data.Tsetmc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.UnitOfWorks
{
    public class TsetmcUnitOfWork : ITsetmcUnitOfWork, ISingletonDependency
    {
        private static DateTime LastSymbolSave = DateTime.Now;

        private readonly SymbolGroupRepository symbolGroupRepository;
        private readonly SymbolDataRepository symbolDataRepository;
        private readonly SymbolRepository symbolRepository;
        private readonly StockCashMarketAtGlanceRepository stockCashMarketAtGlanceRepository;
        private readonly OTCCashMarketAtGlanceRepository OTCCashMarketAtGlanceRepository;
        private readonly MarketWatcherMessageRepository marketWatcherMessageRepository;
        private readonly ValueOfMarketRepository valueOfMarketRepository;
        private readonly TopSupplyAndDemandRepository topSupplyAndDemandRepository;
        private readonly NaturalAndLegalEntityRepository naturalAndLegalEntityRepository;
        private readonly CapitalIncreaseRepository capitalIncrease;

        public TsetmcUnitOfWork(IOptionsSnapshot<ApplicationSetting> options)
        {
            symbolGroupRepository = new(options.Value.mongoDbSetting);
            symbolDataRepository = new(options.Value.mongoDbSetting);
            symbolRepository = new(options.Value.mongoDbSetting);
            stockCashMarketAtGlanceRepository = new(options.Value.mongoDbSetting);
            OTCCashMarketAtGlanceRepository = new(options.Value.mongoDbSetting);
            marketWatcherMessageRepository = new(options.Value.mongoDbSetting);
            valueOfMarketRepository = new(options.Value.mongoDbSetting);
            topSupplyAndDemandRepository = new(options.Value.mongoDbSetting);
            naturalAndLegalEntityRepository = new(options.Value.mongoDbSetting);
            capitalIncrease = new(options.Value.mongoDbSetting);
        }

        public TsetmcUnitOfWork(MongoDbSetting mongoDbSetting)
        {
            symbolGroupRepository = new(mongoDbSetting);
            symbolDataRepository = new(mongoDbSetting);
            symbolRepository = new(mongoDbSetting);
            stockCashMarketAtGlanceRepository = new(mongoDbSetting);
            OTCCashMarketAtGlanceRepository = new(mongoDbSetting);
            marketWatcherMessageRepository = new(mongoDbSetting);
            valueOfMarketRepository = new(mongoDbSetting);
            topSupplyAndDemandRepository = new(mongoDbSetting);
            naturalAndLegalEntityRepository = new(mongoDbSetting);
            capitalIncrease = new(mongoDbSetting);
        }

        public async Task AddMarketWatcherMessageIfNotExistsRangeAsync(List<MarketWatcherMessage> messages, CancellationToken cancellationToken = default(CancellationToken))
        {
            var existMessages = await marketWatcherMessageRepository.GetTodayMessagesAsync(messages.First().Market, cancellationToken);
            List<MarketWatcherMessage> messagesToAdd = new();

            foreach (var item in messages)
            {
                if (!existMessages.Any(i => i.Equals(item)))
                    messagesToAdd.Add(item);
            }

            if (messagesToAdd.Any())
                await marketWatcherMessageRepository.AddRangeAsync(messagesToAdd, cancellationToken);

        }
        public async Task AddValuesOfMarketsIfNotExistsRangeAsync(List<ValueOfMarket> values, CancellationToken cancellationToken = default(CancellationToken))
        {
            var existValues = await valueOfMarketRepository.GetValueOfMarketAsync(values.First().Market, cancellationToken);
            List<ValueOfMarket> valuesToAdd = new();

            foreach (var item in values)
            {
                if (!existValues.Any(i => i.Date == item.Date))
                    valuesToAdd.Add(item);
            }

            if (valuesToAdd.Any())
                await valueOfMarketRepository.AddRangeAsync(valuesToAdd, cancellationToken);

        }
        public async Task AddTopSupplyAndDemandRangeAsync(List<TopSupplyAndDemand> values, CancellationToken cancellationToken = default(CancellationToken))
        {

            if (values.Any())
            {
                var existValues = await topSupplyAndDemandRepository.GetTodayTopSupplyAndDemandAsync(values.First().Market, cancellationToken);
                await topSupplyAndDemandRepository.DeleteRangeAsync(existValues, cancellationToken);
                await topSupplyAndDemandRepository.AddRangeAsync(values, cancellationToken);
            }

        }
        public Task AddOrUpdateSymbolGroups(List<SymbolGroup> symbolGroups, CancellationToken cancellationToken = default(CancellationToken))
            => symbolGroupRepository.AddOrUpdateSymbolGroups(symbolGroups, cancellationToken);

        public async Task AddCashMarketAtGlance(StockCashMarketAtGlance stockCashMarketAtGlance, OTCCashMarketAtGlance oTCCashMarketAtGlance, CancellationToken cancellationToken = default(CancellationToken))
        {
            await stockCashMarketAtGlanceRepository.AddAsync(stockCashMarketAtGlance, cancellationToken);
            await OTCCashMarketAtGlanceRepository.AddAsync(oTCCashMarketAtGlance, cancellationToken);
        }

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

        public Task UpdateSymbolsAsync(Symbol symbol, CancellationToken cancellationToken = default(CancellationToken))
        => symbolRepository.UpdateAsync(symbol, cancellationToken);

        public async Task AddSymbolData(List<SymbolData> data, CancellationToken cancellationToken = default(CancellationToken))
        {
            var startDate = DateTime.Now;
            int i = 0;

            var todayItems = await symbolDataRepository.Table.Find(i => i.LastUpdate >= DateTime.Today).ToListAsync(cancellationToken);

            System.Console.WriteLine($"get  Data1 from db { (DateTime.Now - startDate).TotalSeconds}");

            List<SymbolData> itemsToSave = new();


            if (todayItems is not null && todayItems.Any())
            {
                startDate = DateTime.Now;
                foreach (var item in data)
                {
                    var symbolData = todayItems.Where(i => i.IId == item.IId).OrderByDescending(i => i.LastUpdate).FirstOrDefault();

                    if (symbolData is null || !symbolData.Equals(item))
                    {
                        itemsToSave.Add(item);
                        i++;
                    }

                }

                System.Console.WriteLine($"add to list { (DateTime.Now - startDate).TotalSeconds}");
            }
            else
            {
                startDate = DateTime.Now;

                var symbols = data.Select(i => new Symbol
                {
                    IId = i.IId,
                    Code12 = i.InsCode,
                    Name = i.Name,
                    Sign = i.Symbol,
                    GroupId = i.SymbolGroup
                }).ToList();

                i = data.Count;
                itemsToSave = data;
                await AddSymbolsIfNotExists(symbols, cancellationToken);
                System.Console.WriteLine($"save symbols { (DateTime.Now - startDate).TotalSeconds}");

            }

            startDate = DateTime.Now;
            await symbolDataRepository.AddRangeAsync(itemsToSave, cancellationToken);
            System.Console.WriteLine($"save symbol data 1 { (DateTime.Now - startDate).TotalSeconds}");

            Console.WriteLine($"Count of data: {i}");
        }

        public async Task AddSymbolData2(List<SymbolData> data, CancellationToken cancellationToken = default(CancellationToken))
        {
            var startDate = DateTime.Now;

            if (DateTime.Now - LastSymbolSave > TimeSpan.FromHours(1))
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
                System.Console.WriteLine($"save symbols { (DateTime.Now - startDate).TotalSeconds}");
                startDate = DateTime.Now;
            }
            int i = 0;

            var itemsToSave = new ConcurrentBag<SymbolData>();

            //var tasks = new List<Task>();

            data.AsParallel().ForAll(item =>
            {
                var symbolData = symbolDataRepository.GetLastById(item.IId, cancellationToken).GetAwaiter().GetResult();

                if (symbolData is null || !symbolData.Equals(item))
                {
                    itemsToSave.Add(item);
                    i++;
                }
            });
            //foreach (var item in data)
            //{
            //    tasks.Add(Task.Run(() =>
            //    {

            //    }));
            //}
            //await Task.WhenAll(tasks);

            System.Console.WriteLine($"add to list { (DateTime.Now - startDate).TotalSeconds}");

            startDate = DateTime.Now;
            await symbolDataRepository.AddRangeAsync(itemsToSave, cancellationToken);
            System.Console.WriteLine($"save symbol data 1 { (DateTime.Now - startDate).TotalSeconds}");

            Console.WriteLine($"Count of data: {i}");
        }

        public Task<List<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken = default(CancellationToken))
            => symbolRepository.GetAllAsync(cancellationToken);

        public async Task AddNewNaturalAndLegalEntity(string iid, List<NaturalAndLegalEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            var existsItems = await naturalAndLegalEntityRepository.GetNaturalAndLegalEntityOfSymbolAsync(iid, cancellationToken);
            List<NaturalAndLegalEntity> itemToSave = new();

            foreach (var entity in entities)
            {
                if (!existsItems.Any(i => i.Date.Date == entity.Date.Date))
                    itemToSave.Add(entity);
            }

            await naturalAndLegalEntityRepository.AddRangeAsync(itemToSave, cancellationToken);
        }

        public async Task AddCapitalIncreaseAsync(string iid, List<CapitalIncrease> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            var existsItems = await capitalIncrease.GetCapitalIncreaseOfSymbolAsync(iid, cancellationToken);
            List<CapitalIncrease> itemToSave = new();

            foreach (var entity in entities)
            {
                if (!existsItems.Any(i => i.Date.Date == entity.Date.Date))
                    itemToSave.Add(entity);
            }

            await capitalIncrease.AddRangeAsync(itemToSave, cancellationToken);
        }
    }
}
