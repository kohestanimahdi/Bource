using Bource.Common.Models;
using Bource.Data.Informations.Repositories;
using Bource.Data.Informations.Repositories.Tsetmc;
using Bource.Models.Data.Common;
using Bource.Models.Data.Enums;
using Bource.Models.Data.Tsetmc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IndicatorRepository indicatorRepository;
        private readonly SelectedIndicatorRepository selectedIndicatorRepository;
        private readonly ClosingPriceInfoRepository closingPriceInfoRepository;

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
            indicatorRepository = new(options.Value.mongoDbSetting);
            selectedIndicatorRepository = new(options.Value.mongoDbSetting);
            closingPriceInfoRepository = new(options.Value.mongoDbSetting);
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
            indicatorRepository = new(mongoDbSetting);
            selectedIndicatorRepository = new(mongoDbSetting);
            closingPriceInfoRepository = new(mongoDbSetting);
        }

        public async Task AddSelectedIndicatorsAsync(List<SelectedIndicator> selectedIndicators, CancellationToken cancellationToken = default(CancellationToken))
        {
            var indicators = selectedIndicators.Select(i => new Indicator { InsCode = i.InsCode, Title = i.Title }).ToList();
            await indicatorRepository.AddIfNotExistsAsync(indicators, cancellationToken);

            await selectedIndicatorRepository.AddRangeAsync(selectedIndicators, cancellationToken);
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

        public async Task AddOrUpdateSymbolAsync(Symbol symbol, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (await symbolRepository.Table.Find(i => i.InsCode == symbol.InsCode).AnyAsync(cancellationToken))
                await symbolRepository.UpdateAsync(symbol, cancellationToken);
            else
                await symbolRepository.AddAsync(symbol, cancellationToken);
        }

        public Task AddSymbolsRangeAsync(List<Symbol> symbols, CancellationToken cancellationToken = default(CancellationToken))
        => symbolRepository.AddRangeAsync(symbols, cancellationToken);

        public Task UpdateSymbolAsync(Symbol symbol, CancellationToken cancellationToken = default(CancellationToken))
            => symbolRepository.UpdateAsync(symbol, cancellationToken);

        public async Task AddSymbolData(List<SymbolData> data, CancellationToken cancellationToken = default(CancellationToken))
        {
            var startDate = DateTime.Now;

            var todayItems = await symbolDataRepository.Table.Find(i => i.LastUpdate >= DateTime.Today).ToListAsync(cancellationToken);

            Console.WriteLine($"get  Data1 from db { (DateTime.Now - startDate).TotalSeconds}");

            List<SymbolData> itemsToSave = new();

            if (todayItems is not null && todayItems.Any())
            {
                startDate = DateTime.Now;
                foreach (var item in data)
                {
                    var symbolData = todayItems.Where(i => i.InsCode == item.InsCode).OrderByDescending(i => i.LastUpdate).FirstOrDefault();

                    if (symbolData is null || !symbolData.Equals(item))
                    {
                        itemsToSave.Add(item);
                    }
                }

                System.Console.WriteLine($"add to list { (DateTime.Now - startDate).TotalSeconds}");
            }
            else
                itemsToSave = data;


            startDate = DateTime.Now;
            await symbolDataRepository.AddRangeAsync(itemsToSave, cancellationToken);
            System.Console.WriteLine($"save symbol data 1 { (DateTime.Now - startDate).TotalSeconds}");

            Console.WriteLine($"Count of data: {itemsToSave.Count}");
        }

        public Task<List<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken = default(CancellationToken))
            => symbolRepository.GetAllAsync(cancellationToken);

        public Task<List<Symbol>> GetSymbolsByTypeAsync(IEnumerable<SymbolExistInType> existInTypes, CancellationToken cancellationToken = default(CancellationToken))
            => symbolRepository.Table.Find(i => existInTypes.Contains(i.ExistInType)).ToListAsync(cancellationToken);

        public async Task AddNewNaturalAndLegalEntity(long insCode, List<NaturalAndLegalEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            var existsItems = await naturalAndLegalEntityRepository.GetNaturalAndLegalEntityOfSymbolAsync(insCode, cancellationToken);
            List<NaturalAndLegalEntity> itemToSave = new();

            foreach (var entity in entities)
            {
                if (!existsItems.Any(i => i.Date.Date == entity.Date.Date))
                    itemToSave.Add(entity);
            }

            await naturalAndLegalEntityRepository.AddRangeAsync(itemToSave, cancellationToken);
        }

        public async Task AddCapitalIncreaseAsync(long insCode, List<CapitalIncrease> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            var existsItems = await capitalIncrease.GetCapitalIncreaseOfSymbolAsync(insCode, cancellationToken);
            List<CapitalIncrease> itemToSave = new();

            foreach (var entity in entities)
            {
                if (!existsItems.Any(i => i.Date.Date == entity.Date.Date))
                    itemToSave.Add(entity);
            }

            await capitalIncrease.AddRangeAsync(itemToSave, cancellationToken);
        }

        public async Task AppendClosingPriceInfoAsync(List<ClosingPriceInfo> infos, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!infos.Any()) return;

            long InsCode = infos.First().InsCode;
            if (infos.Any(i => i.InsCode != InsCode))
                throw new ArgumentException("All InsCodes must be same...");

            var closingPriceTypes = infos.First().Type;
            if (infos.Any(i => i.Type != closingPriceTypes))
                throw new ArgumentException("All Types must be same...");

            await closingPriceInfoRepository.AppendAsync(InsCode, closingPriceTypes, infos, cancellationToken);
        }
    }
}