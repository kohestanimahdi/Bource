using Bource.Common.Models;
using Bource.Common.Utilities;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Common;
using Bource.Models.Data.Enums;
using Bource.Models.Data.Tsetmc;
using Bource.Services.Crawlers.Tsetmc.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Tsetmc
{
    public class TsetmcCrawlerService : ITsetmcCrawlerService, IScopedDependency
    {


        #region Properties
        private readonly int numberOfTries = 5;
        private readonly TimeSpan delayBetweenTimeouts = TimeSpan.FromSeconds(1);
        private readonly bool throwExceptions = false;

        private readonly HttpClient httpClient;
        private readonly ILogger<TsetmcCrawlerService> logger;
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;
        private string baseUrl => httpClient.BaseAddress.ToString();
        #endregion

        #region Constructors
        public TsetmcCrawlerService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, ITsetmcUnitOfWork tsetmcUnitOfWork)
        {
            logger = loggerFactory?.CreateLogger<TsetmcCrawlerService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            httpClient = httpClientFactory?.CreateClient(nameof(TsetmcCrawlerService)) ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
        }

        #endregion

        #region نمادها

        /// <summary>
        /// ویرایش اطلاعات کامل هر نماد
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateSymbolsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var symbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);
            await Common.Utilities.ApplicationHelpers.DoFunctionsOFListWithMultiTask<Symbol>(symbols, UpdateSymbolAsync, cancellationToken, 10);
        }

        private async Task UpdateSymbolAsync(Symbol symbol, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 0)
        {
            try
            {
                await GetSymbolInstructionAsync(symbol, cancellationToken);
                await GetSymbolInformationAsync(symbol, cancellationToken);
                await tsetmcUnitOfWork.UpdateSymbolAsync(symbol, cancellationToken);
            }
            catch (Exception ex)
            {
                if (numberOfTries < this.numberOfTries)
                {
                    if (numberOfTries == this.numberOfTries - 1)
                        await Task.Delay(delayBetweenTimeouts);

                    await UpdateSymbolAsync(symbol, cancellationToken, numberOfTries + 1);
                }
                else
                {
                    logger.LogError(ex, "");

                    if (throwExceptions)
                        throw;
                }
            }

        }
        public async Task<List<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var symbols = new List<Symbol>();

            var response = await GetLatestSymbolsResponseAsync(cancellationToken);
            if (response is null)
                return symbols;

            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            var pageComponents = result.Split("@");
            var symbolsData = pageComponents[2];

            var symbolsSplitted = symbolsData.Split(";");
            foreach (var item in symbolsSplitted)
            {
                var symbol = new SymbolData(item, DateTime.Now).GetSymbol();
                symbols.Add(symbol);
            }

            return symbols;
        }

        private async Task GetSymbolInformationAsync(Symbol symbol, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 0)
        {
            try
            {
                var response = await httpClient.GetAsync($"Loader.aspx?Partree=15131M&i={symbol.InsCode}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError($"Error in get Symbol Information {symbol.InsCode}");
                    return;
                }
                var html = await response.Content.ReadAsStringAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(html))
                    return;

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var table = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='table1']");
                if (table is null)
                    return;

                var rows = table.SelectNodes("tbody/tr/td");

                if (rows is null || rows.Count <= 2)
                    return;

                symbol.UpdateInforamtion(rows);
            }
            catch (Exception ex)
            {
                if (numberOfTries < this.numberOfTries)
                {
                    if (numberOfTries == this.numberOfTries - 1)
                        await Task.Delay(delayBetweenTimeouts);

                    await GetSymbolInstructionAsync(symbol, cancellationToken, numberOfTries + 1);
                }
                else
                {
                    logger.LogError(ex, "");

                    if (throwExceptions)
                        throw;
                }
            }
        }
        private async Task GetSymbolInstructionAsync(Symbol symbol, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 0)
        {
            try
            {
                var response = await httpClient.GetAsync($"Loader.aspx?Partree=15131V&s={symbol.Sign}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError($"Error in get Symbol Instruction {symbol.InsCode}");
                    return;
                }
                var html = await response.Content.ReadAsStringAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(html))
                    return;

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var table = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='table1']");
                if (table is null)
                    return;

                var rows = table.SelectNodes("tbody/tr/td");

                if (rows is null || rows.Count <= 2)
                    return;

                symbol.Introduction = new(rows);
            }
            catch (Exception ex)
            {
                if (numberOfTries < this.numberOfTries)
                {
                    if (numberOfTries == this.numberOfTries - 1)
                        await Task.Delay(delayBetweenTimeouts);

                    await GetSymbolInstructionAsync(symbol, cancellationToken, numberOfTries + 1);
                }
                else
                {
                    logger.LogError(ex, "");

                    if (throwExceptions)
                        throw;
                }
            }
        }
        #endregion

        #region حقیقی و حقوقی

        /// <summary>
        /// دریافت اطلاعات حقیقی و حقوقی هر نماد
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task GetAllNaturalAndLegalEntityAsync(CancellationToken cancellationToken = default(CancellationToken))
        {

            var symbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);

            await Common.Utilities.ApplicationHelpers.DoFunctionsWithProgressBar<Symbol>(symbols, GetNaturalAndLegalEntityAsync, cancellationToken);
        }

        private async Task GetNaturalAndLegalEntityAsync(Symbol symbol, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 0)
        {
            try
            {
                List<NaturalAndLegalEntity> entities = new();

                var response = await httpClient.GetAsync($"tsev2/data/clienttype.aspx?i={symbol.InsCode}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError("Error in get Batural And LegalEntity");
                    return;
                }

                var result = await response.Content.ReadAsStringAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(result))
                    return;

                var rows = result.Split(";");
                foreach (var row in rows)
                {
                    entities.Add(new NaturalAndLegalEntity(symbol.InsCode, row.Split(",")));
                }

                await tsetmcUnitOfWork.AddNewNaturalAndLegalEntity(symbol.InsCode, entities, cancellationToken);
            }
            catch (Exception ex)
            {
                if (numberOfTries < this.numberOfTries)
                {
                    if (numberOfTries == this.numberOfTries - 1)
                        await Task.Delay(delayBetweenTimeouts);

                    await GetNaturalAndLegalEntityAsync(symbol, cancellationToken, numberOfTries + 1);
                }
                else
                {
                    logger.LogError(ex, "");

                    if (throwExceptions)
                        throw;
                }
            }
        }
        #endregion

        #region صنعت

        /// <summary>
        /// دریافت نام صنایع
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task GetOrUpdateSymbolGroupsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await httpClient.GetAsync("Loader.aspx?ParTree=111C1213", cancellationToken);

            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error in get symbol groups", html);
                return;
            }

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var trs = htmlDoc.DocumentNode.SelectNodes("//table/tr");

            var groups = new List<SymbolGroup>();

            for (int i = 1; i < trs.Count; i++)
            {
                var tds = trs[i].SelectNodes("td");
                groups.Add(new SymbolGroup
                {
                    Title = tds[1].InnerText.FixPersianLetters(),
                    Code = tds[0].InnerText.FixedNumbersToEn()
                });
            }

            await tsetmcUnitOfWork.AddOrUpdateSymbolGroups(groups, cancellationToken);
        }
        #endregion

        #region  در یک نگاه نماد

        public async Task ScheduleLatestSymbolData()
        {
            await FillOneTimeDataAsync();
            while (DateTime.Now.Hour >= 8 && DateTime.Now.Minute >= 30 && DateTime.Now.Hour <= 13)
            {
                try
                {
                    var time = DateTime.Now;
                    GetLatestSymbolDataAsync().GetAwaiter().GetResult();
                    var delay = DateTime.Now - time;
                    if (delay < TimeSpan.FromSeconds(1))
                        Task.Delay(TimeSpan.FromSeconds(1) - delay).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "");
                }
            }
        }

        /// <summary>
        /// دریافت اطلاعات لحظه ای هر نماد
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task GetLatestSymbolDataAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var startTime = DateTime.Now;

            var response = await GetLatestSymbolsResponseAsync(cancellationToken);
            if (response is null)
                return;

            var result = await response.Content.ReadAsStringAsync(cancellationToken);

            var lastModified = response.Content.Headers.LastModified?.LocalDateTime ?? DateTime.Now;

            var pageComponents = result.Split("@");

            var symbolsData = pageComponents[2];
            var transactionsData = pageComponents[3];

            var transactionsDataDictionary = transactionsData.Split(";").GroupBy(i => i.Split(",")[0]).ToDictionary(i => Convert.ToInt64(i.Key), j => j.ToList());

            var symbolsSplitted = symbolsData.Split(";");

            var data = new List<SymbolData>();

            foreach (var item in symbolsSplitted)
            {
                var symbolData = new SymbolData(item, lastModified);
                symbolData.FillTransactions(transactionsDataDictionary[symbolData.InsCode]);
                data.Add(symbolData);
            }

            await GetLatestClientSymbolDataAsync(data, cancellationToken);

            logger.LogInformation($"Get Datas From Tse:{(DateTime.Now - startTime).TotalSeconds}");


            //startTime = DateTime.Now;
            // افزودن به لیست دیتاهای امروز و صف برای ذخیره سازی
            TseSymbolDataProvider.AddSymbolDataToQueue(data);

            //logger.LogInformation($"Save To Queue:{(DateTime.Now - startTime).TotalSeconds}");

        }

        /// <summary>
        /// اطلاعاتی که روزانه یک بار از هر نماد آپدیت می‌شوند
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task FillOneTimeDataAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var symbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);

            var fillSymbolData = symbols.Select(i => new FillSymbolData(i.InsCode)).ToList();

            await Common.Utilities.ApplicationHelpers.DoFunctionsOFListWithMultiTask(fillSymbolData, FillSymbolDataAsync, cancellationToken, 15);

            TseSymbolDataProvider.FillOneTimeData(fillSymbolData);
        }

        private async Task FillSymbolDataAsync(FillSymbolData symboldata, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 0)
        {
            try
            {
                var response = await httpClient.GetAsync($"loader.aspx?ParTree=151311&i={symboldata.InsCode}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError("Error in fill symbol data");
                    return;
                }

                var result = await response.Content.ReadAsStringAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(result))
                    return;

                symboldata.FillDataFromPage(result);
            }
            catch (Exception ex)
            {
                if (numberOfTries < this.numberOfTries)
                {
                    if (numberOfTries == this.numberOfTries - 1)
                        await Task.Delay(delayBetweenTimeouts);

                    await FillSymbolDataAsync(symboldata, cancellationToken, numberOfTries + 1);
                }
                else
                {
                    logger.LogError(ex, "");

                    if (throwExceptions)
                        throw;
                }
            }

        }
        private async Task GetLatestClientSymbolDataAsync(List<SymbolData> symbols, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await httpClient.GetAsync("tsev2/data/ClientTypeAll.aspx", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error in get client symbol data");
                return;
            }

            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            var clientValues = result.Split(";");
            foreach (var item in clientValues)
            {
                var columns = item.Split(",");
                var symbol = symbols.FirstOrDefault(i => i.InsCode == Convert.ToInt64(columns[0]));
                if (symbol is null)
                    continue;

                symbol.FillClientValues(columns);
            }
        }
        private async Task<HttpResponseMessage> GetLatestSymbolsResponseAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await httpClient.GetAsync("tsev2/data/MarketWatchPlus.aspx", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error in get symbol data");
                return null;
            }

            return response;
        }

        #endregion

        #region بازار نقدی در یک نگاه

        public async Task GetMarketAtGlanceAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await httpClient.GetAsync("Loader.aspx?ParTree=15", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error in Get Market At Glance");
                return;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var stockCashMarketAtGlance = GetStockCashMarketAtGlance(htmlDoc);
            var OTCCashMarketAtGlance = GetOTCCashMarketAtGlance(htmlDoc);

            await tsetmcUnitOfWork.AddCashMarketAtGlance(stockCashMarketAtGlance, OTCCashMarketAtGlance, cancellationToken);
        }

        private StockCashMarketAtGlance GetStockCashMarketAtGlance(HtmlDocument htmlDoc)
        {
            var node = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='TseTab1Elm']");

            var TseTab1ElmDoc = new HtmlDocument();
            TseTab1ElmDoc.LoadHtml(node.InnerHtml);

            var tables = TseTab1ElmDoc.DocumentNode.SelectNodes("//table[@class='table1']");

            StockCashMarketAtGlance stockCashMarketAtGlance = null;
            if (tables.Any())
            {
                var trs = tables.First().SelectNodes("tbody/tr/td");
                if (trs.Any())
                {
                    stockCashMarketAtGlance = new StockCashMarketAtGlance
                    {
                        CreateDate = DateTime.Now,
                        Status = trs[1].GetText(),
                        OverallIndex = trs[3].FirstChild.ConvertToDecimal(),
                        OverallIndexChange = trs[3].SelectSingleNode("div").ConvertToNegativePositiveDecimal(),
                        OverallIndexEqualWeight = trs[5].FirstChild.ConvertToDecimal(),
                        OverallIndexEqualWeightChange = trs[5].SelectSingleNode("div").ConvertToNegativePositiveDecimal(),
                        ValueOfMarket = trs[7].GetAttributeValueAsDecimal(),
                        Time = trs[9].GetAsDateTime("14"),
                        NumberOfTransaction = trs[11].ConvertToDecimal(),
                        ValueOfTransaction = trs[13].GetAttributeValueAsDecimal(),
                        Turnover = trs[15].GetAttributeValueAsDecimal(),
                    };
                }
            }

            return stockCashMarketAtGlance;
        }

        private OTCCashMarketAtGlance GetOTCCashMarketAtGlance(HtmlDocument htmlDoc)
        {
            var node = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='IfbTab1Elm']");

            var TseTab1ElmDoc = new HtmlDocument();
            TseTab1ElmDoc.LoadHtml(node.InnerHtml);

            var tables = TseTab1ElmDoc.DocumentNode.SelectNodes("//table[@class='table1']");

            OTCCashMarketAtGlance stockCashMarketAtGlance = null;
            if (tables.Any())
            {
                var trs = tables.First().SelectNodes("tbody/tr/td");
                if (trs.Any())
                {
                    stockCashMarketAtGlance = new OTCCashMarketAtGlance
                    {
                        CreateDate = DateTime.Now,
                        Status = trs[1].GetText(),
                        OverallIndex = trs[3].FirstChild.ConvertToDecimal(),
                        OverallIndexChange = trs[3].SelectSingleNode("div").ConvertToNegativePositiveDecimal(),
                        ValueOfFirstAndSecondMarket = trs[5].GetAttributeValueAsDecimal(),

                        Time = trs[7].GetAsDateTime("14"),
                        NumberOfTransaction = trs[9].ConvertToDecimal(),
                        ValueOfTransaction = trs[11].GetAttributeValueAsDecimal(),
                        Turnover = trs[13].GetAttributeValueAsDecimal(),
                    };
                }
            }

            return stockCashMarketAtGlance;
        }

        #endregion

        #region پیغام‌های ناظر

        /// <summary>
        /// دریافت پیغام‌های ناظر بورس و فرابورس
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task GetMarketWatcherMessage(CancellationToken cancellationToken = default(CancellationToken))
        {
            var stockMessages = await GetMarketWatcherMessage(MarketType.Stock, cancellationToken);
            var otcMessages = await GetMarketWatcherMessage(MarketType.OTC, cancellationToken);

            if (stockMessages.Any())
                await tsetmcUnitOfWork.AddMarketWatcherMessageIfNotExistsRangeAsync(stockMessages, cancellationToken);

            if (otcMessages.Any())
                await tsetmcUnitOfWork.AddMarketWatcherMessageIfNotExistsRangeAsync(otcMessages, cancellationToken);
        }

        private async Task<List<MarketWatcherMessage>> GetMarketWatcherMessage(MarketType market, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<MarketWatcherMessage> messages = new();
            var response = await httpClient.GetAsync($"Loader.aspx?Partree=151313&Flow={(byte)market}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error in Market Watcher Message");
                return messages;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var trs = htmlDoc.DocumentNode.SelectNodes("//table[@class='table1']/tbody/tr");

            for (int i = 0; i < trs.Count - 1; i += 2)
            {
                var ths = trs[i].SelectNodes("th");
                messages.Add(new MarketWatcherMessage
                {
                    CreateDate = DateTime.Now,
                    Market = market,
                    Title = ths[0].GetText(),
                    Time = ths[1].GetAsDateTime("14"),
                    Description = trs[i + 1].GetText()
                });
            }

            return messages;
        }

        #endregion

        #region ارزش بازار

        /// <summary>
        /// دریافت ارزش بازار بورس و فرابورس
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task GetValueOfMarketAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var stockValueOfMarkets = await GetValueOfMarketAsync(MarketType.Stock, cancellationToken);
            var otcValueOfMarkets = await GetValueOfMarketAsync(MarketType.OTC, cancellationToken);

            if (stockValueOfMarkets.Any())
                await tsetmcUnitOfWork.AddValuesOfMarketsIfNotExistsRangeAsync(stockValueOfMarkets, cancellationToken);

            if (otcValueOfMarkets.Any())
                await tsetmcUnitOfWork.AddValuesOfMarketsIfNotExistsRangeAsync(otcValueOfMarkets, cancellationToken);
        }

        private async Task<List<ValueOfMarket>> GetValueOfMarketAsync(MarketType market, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<ValueOfMarket> values = new();
            var response = await httpClient.GetAsync($"Loader.aspx?Partree=15131N&Flow={(byte)market}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error in Get value Of Market");
                return values;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var tds = htmlDoc.DocumentNode.SelectNodes("//table[@class='table1']/tbody/tr/td");

            for (int i = 0; i < tds.Count - 1; i += 2)
            {
                values.Add(new ValueOfMarket
                {
                    CreateDate = DateTime.Now,
                    Market = market,
                    Date = tds[i].GetAsDateTime("14"),
                    Value = tds[i + 1].GetAttributeValueAsDecimal()
                });
            }

            return values;
        }

        #endregion

        #region عرضه و تقاضا

        /// <summary>
        /// دریافت اطلاعات بیشترین عرضه و تقاضای بورس و فرابورس
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task GetTopSupplyAndDemandAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var stockTopSupplyAndDemand = await GetTopSupplyAndDemandAsync(MarketType.Stock, cancellationToken);
            var otcTopSupplyAndDemand = await GetTopSupplyAndDemandAsync(MarketType.OTC, cancellationToken);

            if (stockTopSupplyAndDemand.Any())
                await tsetmcUnitOfWork.AddTopSupplyAndDemandRangeAsync(stockTopSupplyAndDemand, cancellationToken);

            if (otcTopSupplyAndDemand.Any())
                await tsetmcUnitOfWork.AddTopSupplyAndDemandRangeAsync(otcTopSupplyAndDemand, cancellationToken);
        }

        private async Task<List<TopSupplyAndDemand>> GetTopSupplyAndDemandAsync(MarketType market, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<TopSupplyAndDemand> values = new();
            var response = await httpClient.GetAsync($"Loader.aspx?Partree=151318&Flow={(byte)market}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error in Get Top Supply And Demand");
                return values;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var tables = htmlDoc.DocumentNode.SelectNodes("//table[@class='table1']");

            var demandTable = tables.First();
            var supplyTable = tables.Skip(1).First();

            var demandtrs = demandTable.SelectNodes("tbody/tr");
            var supplytrs = supplyTable.SelectNodes("tbody/tr");
            foreach (var item in demandtrs)
            {
                var tds = item.SelectNodes("td");

                values.Add(new TopSupplyAndDemand
                {
                    CreateDate = DateTime.Now,
                    Market = market,
                    Symbol = tds[0].GetText(),
                    Price = tds[1].ConvertToLong(),
                    Volume = tds[2].GetAttributeValueAsDecimal(),
                    Value = tds[3].GetAttributeValueAsDecimal(),
                    Count = tds[4].ConvertToLong(),
                    InsCode = Convert.ToInt64(tds[0].GetQueryString("i", baseUrl)),
                    IsSupply = false
                });
            }

            foreach (var item in supplytrs)
            {
                var tds = item.SelectNodes("td");
                values.Add(new TopSupplyAndDemand
                {
                    CreateDate = DateTime.Now,
                    Market = market,
                    Symbol = tds[0].GetText(),
                    Price = tds[1].ConvertToLong(),
                    Volume = tds[2].GetAttributeValueAsDecimal(),
                    Value = tds[3].GetAttributeValueAsDecimal(),
                    Count = tds[4].ConvertToLong(),
                    InsCode = Convert.ToInt64(tds[0].GetQueryString("i", baseUrl)),
                    IsSupply = true
                });
            }

            return values;
        }

        #endregion

        #region افزایش سرمایه

        /// <summary>
        /// دریافت اطلاعات افزایش سرمایه هر نماد
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task GetAllCapitalIncreaseAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var symbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);

            await Common.Utilities.ApplicationHelpers.DoFunctionsOFListWithMultiTask(symbols, GetCapitalIncreaseAsync, cancellationToken, 10);
        }

        private async Task GetCapitalIncreaseAsync(Symbol symbol, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 0)
        {
            try
            {
                var response = await httpClient.GetAsync($"Loader.aspx?Partree=15131H&i={symbol.InsCode}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError($"Error in Get Capital Increase {symbol.InsCode}");
                    return;
                }

                var html = await response.Content.ReadAsStringAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(html))
                    return;

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var tables = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='table1']");

                var trs = tables.SelectNodes("tbody/tr");
                if (trs is null)
                    return;

                List<CapitalIncrease> entities = new();
                foreach (var tr in trs)
                {
                    var tds = tr.SelectNodes("td");
                    entities.Add(new CapitalIncrease(symbol.InsCode, tds));
                }

                await tsetmcUnitOfWork.AddCapitalIncreaseAsync(symbol.InsCode, entities, cancellationToken);
            }
            catch (Exception ex)
            {
                if (numberOfTries < this.numberOfTries)
                {
                    if (numberOfTries == this.numberOfTries - 1)
                        await Task.Delay(delayBetweenTimeouts);

                    await GetCapitalIncreaseAsync(symbol, cancellationToken, numberOfTries + 1);
                }
                else
                {
                    logger.LogError(ex, "");

                    if (throwExceptions)
                        throw;
                }
            }
        }
        #endregion

        #region شاخص‌ها

        /// <summary>
        /// دریافت لحظه ای شاخص های منتخب
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task GetSelectedIndicatorAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await httpClient.GetAsync($"Loader.aspx?Partree=151315&Flow=1", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error in Get Selected Indicator");
                return;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(html))
                return;

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var tables = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='table1']");

            var trs = tables.SelectNodes("tbody/tr");
            if (trs is null)
                return;

            List<SelectedIndicator> entities = new();
            foreach (var tr in trs)
            {
                var tds = tr.SelectNodes("td");
                var insCode = Convert.ToInt64(tds[0].GetQueryString("i", baseUrl));
                entities.Add(new SelectedIndicator(insCode, tds));
            }

            await tsetmcUnitOfWork.AddSelectedIndicatorsAsync(entities, cancellationToken);
        }

        #endregion

        #region سهامداران

        public async Task GetSymbolsShareHoldersAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var symbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);
            await Common.Utilities.ApplicationHelpers.DoFunctionsOFListWithMultiTask(symbols, GetSymbolShareHoldersAsync, cancellationToken);

        }

        private async Task GetSymbolShareHoldersAsync(Symbol symbol, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(symbol.CompanyCode))
                    return;

                var response = await httpClient.GetAsync($"Loader.aspx?Partree=15131T&c={symbol.CompanyCode}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError("Error in Get Symbol Share Holders");
                    return;
                }

                var html = await response.Content.ReadAsStringAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(html))
                    return;

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var table = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='table1']");
                if (table is null)
                    return;

                var rows = table.SelectNodes("tbody/tr");
                if (rows is null)
                    return;

                var items = new List<SymbolShareHolder>();
                foreach (var row in rows)
                    items.Add(new SymbolShareHolder(symbol.InsCode, row));

                if (items.Any())
                    await tsetmcUnitOfWork.AddTodaysSymbolShareHoldersAsync(symbol.InsCode, items, cancellationToken);
            }
            catch (Exception ex)
            {
                if (numberOfTries < this.numberOfTries)
                {
                    if (numberOfTries == this.numberOfTries - 1)
                        await Task.Delay(delayBetweenTimeouts);

                    await GetSymbolShareHoldersAsync(symbol, cancellationToken, numberOfTries + 1);
                }
                else
                {
                    logger.LogError(ex, "");

                    if (throwExceptions)
                        throw;
                }
            }
        }

        public async Task GetChangeOfSharesOfActiveShareHoldersAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await httpClient.GetAsync($"Loader.aspx?ParTree=15131I&t=0", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error in Get Active Symbol Share Holders");
                return;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(html))
                return;

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var table = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='table1']");
            if (table is null)
                return;

            var header = table.SelectNodes("thead/tr/th");
            if (header is null || header.Count < 2)
                return;

            var date = header[1].GetAsDateTime();
            date = new DateTime(date.Year, date.Month, date.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            var rows = table.SelectNodes("tbody/tr");
            if (rows is null)
                return;

            var items = new List<ActiveSymbolShareHolder>();
            ActiveSymbolShareHolder selectedItem = null;
            foreach (var row in rows)
            {
                var tds = row.SelectNodes("td");
                var div = tds[0].SelectSingleNode("div");
                if (div is not null)
                {
                    if (selectedItem is not null)
                        items.Add(selectedItem);

                    selectedItem = new ActiveSymbolShareHolder
                    {
                        InsCode = Convert.ToInt64(div.GetQueryString("i", baseUrl)),
                        SymbolName = div.GetText(),
                        Time = date,
                        Companies = new List<ActiveSymbolShareHolderCompany>()
                    };
                }

                selectedItem.Companies.Add(new ActiveSymbolShareHolderCompany
                {
                    Name = tds[0].SelectSingleNode("li").GetText(),
                    Share = tds[1].ConvertToDecimal(),
                    ShareChange = tds[1].SelectSingleNode("div")?.ConvertToNegativePositiveDecimal() ?? 0
                });
            }

            if (selectedItem is not null)
                items.Add(selectedItem);

            if (items.Any())
                await tsetmcUnitOfWork.AddAdtiveSymbolShareHoldersAsync(items, cancellationToken);
        }
        #endregion
    }
}