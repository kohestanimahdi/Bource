using Bource.Common.Models;
using Bource.Data.Informations.Repositories;
using Bource.Data.Informations.Repositories.Tsetmc;
using Bource.Models.Data.Common;
using Bource.Models.Data.Enums;
using Bource.Models.Data.Tsetmc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.UnitOfWorks
{
    public class TsetmcUnitOfWork : ITsetmcUnitOfWork, IScopedDependency
    {
        private readonly SymbolGroupRepository symbolGroupRepository;
        private readonly SymbolDataRepository symbolDataRepository;
        private readonly SymbolRepository symbolRepository;
        private readonly CashMarketAtGlanceRepository cashMarketAtGlanceRepository;
        private readonly MarketWatcherMessageRepository marketWatcherMessageRepository;
        private readonly ValueOfMarketRepository valueOfMarketRepository;
        private readonly TopSupplyAndDemandRepository topSupplyAndDemandRepository;
        private readonly NaturalAndLegalEntityRepository naturalAndLegalEntityRepository;
        private readonly CapitalIncreaseRepository capitalIncrease;
        private readonly IndicatorRepository indicatorRepository;
        private readonly SelectedIndicatorRepository selectedIndicatorRepository;
        private readonly ClosingPriceInfoRepository closingPriceInfoRepository;
        private readonly SymbolShareHolderRepository symbolShareHolderRepository;
        private readonly ActiveSymbolShareHolderRepository activeSymbolShareHolderRepository;

        public TsetmcUnitOfWork(ApplicationSetting setting)
        {
            symbolGroupRepository = new(setting.mongoDbSetting);
            symbolDataRepository = new(setting.mongoDbSetting);
            symbolRepository = new(setting.mongoDbSetting);
            cashMarketAtGlanceRepository = new(setting.mongoDbSetting);
            marketWatcherMessageRepository = new(setting.mongoDbSetting);
            valueOfMarketRepository = new(setting.mongoDbSetting);
            topSupplyAndDemandRepository = new(setting.mongoDbSetting);
            naturalAndLegalEntityRepository = new(setting.mongoDbSetting);
            capitalIncrease = new(setting.mongoDbSetting);
            indicatorRepository = new(setting.mongoDbSetting);
            selectedIndicatorRepository = new(setting.mongoDbSetting);
            closingPriceInfoRepository = new(setting.mongoDbSetting);
            symbolShareHolderRepository = new(setting.mongoDbSetting);
            activeSymbolShareHolderRepository = new(setting.mongoDbSetting);
        }

        public async Task AddSelectedIndicatorsAsync(List<SelectedIndicator> selectedIndicators, CancellationToken cancellationToken = default(CancellationToken))
        {
            var indicators = selectedIndicators.Select(i => new Indicator(i.Title, i.InsCode)).ToList();
            await indicatorRepository.AddIfNotExistsAsync(indicators, cancellationToken);
            await selectedIndicatorRepository.AddRangeAsync(selectedIndicators, cancellationToken);
        }

        public Task<List<Indicator>> GetIndicatorsAsync(CancellationToken cancellationToken = default(CancellationToken))
            => indicatorRepository.GetAllAsync(cancellationToken);

        public Task AddOrUpdateIndicatorsAsync(List<Indicator> indicators, CancellationToken cancellationToken = default(CancellationToken))
            => indicatorRepository.AddOrUpdateAsync(indicators, cancellationToken);

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

        public Task AddCashMarketAtGlance(CashMarketAtGlance stockCashMarketAtGlance, CashMarketAtGlance oTCCashMarketAtGlance, CancellationToken cancellationToken = default(CancellationToken))
        => cashMarketAtGlanceRepository.AddRangeAsync(new CashMarketAtGlance[] { stockCashMarketAtGlance, oTCCashMarketAtGlance }, cancellationToken);

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

        //public async Task AddSymbolData(List<SymbolData> data, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    var startDate = DateTime.Now;

        //    var todayItems = await symbolDataRepository.Table.Find(i => i.LastUpdate >= DateTime.Today).ToListAsync(cancellationToken);

        //    logger.LogInformation($"get  Data from database: { (DateTime.Now - startDate).TotalSeconds}");

        //    List<SymbolData> itemsToSave = new();

        //    if (todayItems is not null && todayItems.Any())
        //    {
        //        startDate = DateTime.Now;
        //        foreach (var item in data)
        //        {
        //            var symbolData = todayItems.Where(i => i.InsCode == item.InsCode).OrderByDescending(i => i.LastUpdate).FirstOrDefault();

        //            if (symbolData is null || !symbolData.Equals(item))
        //            {
        //                itemsToSave.Add(item);
        //            }
        //        }

        //    }
        //    else
        //        itemsToSave = data;

        //    await symbolDataRepository.AddRangeAsync(itemsToSave, cancellationToken);
        //}

        public Task AddSymbolData(List<SymbolData> data, CancellationToken cancellationToken = default(CancellationToken))
        => symbolDataRepository.AddRangeAsync(data, cancellationToken);

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

        public Task AddTodaysSymbolShareHoldersAsync(long insCode, List<SymbolShareHolder> items, CancellationToken cancellationToken = default(CancellationToken))
        => symbolShareHolderRepository.AddTodaysSymbolShareHoldersAsync(insCode, items, cancellationToken);

        public Task AddAdtiveSymbolShareHoldersAsync(List<ActiveSymbolShareHolder> items, CancellationToken cancellationToken = default(CancellationToken))
        => activeSymbolShareHolderRepository.AddRangeAsync(items, cancellationToken);

        public Task<List<ClosingPriceInfo>> GetClosingPriceInfosAsync(long insCode, ClosingPriceTypes? closingPriceTypes, CancellationToken cancellationToken = default(CancellationToken))
            => closingPriceInfoRepository.Table.Find(i => i.InsCode == insCode && (closingPriceTypes == null || i.Type == closingPriceTypes)).ToListAsync(cancellationToken);


    }
}