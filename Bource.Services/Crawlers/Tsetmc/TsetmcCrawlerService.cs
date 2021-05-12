using Bource.Common.Models;
using Bource.Common.Utilities;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Common;
using Bource.Models.Data.Enums;
using Bource.Models.Data.Tsetmc;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Tsetmc
{
    public class TsetmcCrawlerService : IScopedDependency
    {
        private string baseUrl { get; init; }
        private readonly HttpClient httpClient;
        private readonly ILogger<TsetmcCrawlerService> logger;
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;
        private static readonly ConcurrentQueue<List<SymbolData>> SymbolDataQueue = new();

        public TsetmcCrawlerService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, ITsetmcUnitOfWork tsetmcUnitOfWork)
        {
            logger = loggerFactory?.CreateLogger<TsetmcCrawlerService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            httpClient = httpClientFactory?.CreateClient() ?? throw new ArgumentNullException(nameof(httpClientFactory));
            baseUrl = "http://www.tsetmc.com/";

            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(2);

            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
        }

        public TsetmcCrawlerService(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            baseUrl = "http://www.tsetmc.com/";

            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(2);

            tsetmcUnitOfWork = new TsetmcUnitOfWork(new MongoDbSetting { ConnectionString = "mongodb://localhost:27017/", DataBaseName = "BourceInformation" });
        }

        public async Task UpdateSymbolAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var symbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);
            foreach (var symbol in symbols)
            {
                await GetSymbolInstructionAsync(symbol, cancellationToken);
                await GetSymbolInformationAsync(symbol, cancellationToken);
                await tsetmcUnitOfWork.UpdateSymbolsAsync(symbol, cancellationToken);
            }
        }

        private async Task GetSymbolInformationAsync(Symbol symbol, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 0)
        {
            try
            {
                var response = await httpClient.GetAsync($"Loader.aspx?Partree=15131M&i={symbol.IId}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError("Error in get Symbol Information");
                    Console.WriteLine("Error in get Symbol Information");
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
            catch
            {
                if (numberOfTries < 2)
                    await GetSymbolInstructionAsync(symbol, cancellationToken, numberOfTries++);
                else
                    throw;
            }
        }

        private async Task GetSymbolInstructionAsync(Symbol symbol, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 0)
        {
            try
            {
                var response = await httpClient.GetAsync($"Loader.aspx?Partree=15131V&s={symbol.Sign}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError("Error in get SymbolInstruction");
                    Console.WriteLine("Error in get SymbolInstruction");
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
            catch
            {
                if (numberOfTries < 2)
                    await GetSymbolInstructionAsync(symbol, cancellationToken, numberOfTries++);
                else
                    throw;
            }
        }

        public async Task GetAllNaturalAndLegalEntityAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var startDate = DateTime.Now;

            var symbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);

            System.Console.WriteLine($"Get symbols :{ (DateTime.Now - startDate).TotalSeconds}");
            startDate = DateTime.Now;

            foreach (var symbol in symbols)
            {
                await GetNaturalAndLegalEntityAsync(symbol.IId, cancellationToken);
            }

            System.Console.WriteLine($"save NaturalAndLegalEntity :{ (DateTime.Now - startDate).TotalSeconds}");
        }

        private async Task GetNaturalAndLegalEntityAsync(string iid, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 0)
        {
            try
            {
                List<NaturalAndLegalEntity> entities = new();

                var response = await httpClient.GetAsync($"tsev2/data/clienttype.aspx?i={iid}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError("Error in get Batural And LegalEntity");
                    Console.WriteLine("Error in get Batural And LegalEntity");
                    return;
                }
                var result = await response.Content.ReadAsStringAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(result))
                    return;

                var rows = result.Split(";");
                foreach (var row in rows)
                {
                    entities.Add(new NaturalAndLegalEntity(iid, row.Split(",")));
                }

                await tsetmcUnitOfWork.AddNewNaturalAndLegalEntity(iid, entities, cancellationToken);
            }
            catch
            {
                if (numberOfTries < 2)
                    await GetNaturalAndLegalEntityAsync(iid, cancellationToken, numberOfTries++);
                else
                    throw;
            }
        }

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

        private async Task GetLatestClientSymbolDataAsync(List<SymbolData> symbols, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await httpClient.GetAsync("tsev2/data/ClientTypeAll.aspx", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error in get client symbol data");
                Console.WriteLine("Error in get client symbol data");
                return;
            }

            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            var clientValues = result.Split(";");
            foreach (var item in clientValues)
            {
                var columns = item.Split(",");
                var symbol = symbols.FirstOrDefault(i => i.IId == columns[0]);
                if (symbol is null)
                    continue;

                symbol.FillClientValues(columns);
            }
        }

        public async Task GetLatestSymbolDataAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var startDate = DateTime.Now;

            var response = await httpClient.GetAsync("tsev2/data/MarketWatchPlus.aspx", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error in get symbol data");
                Console.WriteLine("Error in get symbol data");
                return;
            }

            var result = await response.Content.ReadAsStringAsync(cancellationToken);

            var lastModified = response.Content.Headers.LastModified?.LocalDateTime ?? DateTime.Now;

            var pageComponents = result.Split("@");

            var symbolsData = pageComponents[2];
            var transactionsData = pageComponents[3];

            var transactionsDataDictionary = transactionsData.Split(";").GroupBy(i => i.Split(",")[0]).ToDictionary(i => i.Key, j => j.ToList());

            var symbolsSplitted = symbolsData.Split(";");

            var data = new List<SymbolData>();

            System.Console.WriteLine($"Get symbol Data:{ (DateTime.Now - startDate).TotalSeconds}");
            startDate = DateTime.Now;

            foreach (var item in symbolsSplitted)
            {
                var symbolData = new SymbolData(item, lastModified);
                symbolData.FillTransactions(transactionsDataDictionary[symbolData.IId]);

                data.Add(symbolData);
            }
            System.Console.WriteLine($"Get symbol transactions Data:{ (DateTime.Now - startDate).TotalSeconds}");
            startDate = DateTime.Now;
            await GetLatestClientSymbolDataAsync(data, cancellationToken);
            System.Console.WriteLine($"Get symbol client Data:{ (DateTime.Now - startDate).TotalSeconds}");
            startDate = DateTime.Now;

            SymbolDataQueue.Enqueue(data);

            System.Console.WriteLine($"Save Data to queue:{(DateTime.Now - startDate).TotalSeconds}");
        }

        public async Task GetMarketAtGlanceAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await httpClient.GetAsync("Loader.aspx?ParTree=15", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error in Get Market At Glance ");
                Console.WriteLine("Error in Get Market At Glance ");
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
                Console.WriteLine("Error in Market Watcher Message");
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
                Console.WriteLine("Error in Get value Of Market");
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
                Console.WriteLine("Error in Get Top Supply And Demand");
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
                var uri = new Uri(baseUrl + tds[0].SelectSingleNode("a").Attributes["href"].Value);
                var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);
                values.Add(new TopSupplyAndDemand
                {
                    CreateDate = DateTime.Now,
                    Market = market,
                    Symbol = tds[0].GetText(),
                    Price = tds[1].ConvertToLong(),
                    Volume = tds[2].GetAttributeValueAsDecimal(),
                    Value = tds[3].GetAttributeValueAsDecimal(),
                    Count = tds[4].ConvertToLong(),
                    IId = queryDictionary["i"],
                    IsSupply = false
                });
            }

            foreach (var item in supplytrs)
            {
                var tds = item.SelectNodes("td");
                var uri = new Uri(baseUrl + tds[0].SelectSingleNode("a").Attributes["href"].Value);
                var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);
                values.Add(new TopSupplyAndDemand
                {
                    CreateDate = DateTime.Now,
                    Market = market,
                    Symbol = tds[0].GetText(),
                    Price = tds[1].ConvertToLong(),
                    Volume = tds[2].GetAttributeValueAsDecimal(),
                    Value = tds[3].GetAttributeValueAsDecimal(),
                    Count = tds[4].ConvertToLong(),
                    IId = queryDictionary["i"],
                    IsSupply = true
                });
            }

            return values;
        }

        public async Task SaveSymbolData(CancellationToken cancellationToken = default(CancellationToken))
        {
            while (true)
            {
                var startDate = DateTime.Now;
                if (!SymbolDataQueue.IsEmpty)
                {
                    List<SymbolData> data;
                    if (SymbolDataQueue.TryDequeue(out data))
                    {
                        await tsetmcUnitOfWork.AddSymbolData(data, cancellationToken);
                        System.Console.WriteLine($"Save Data to database:{(DateTime.Now - startDate).TotalSeconds}");
                    }
                }
                else
                    await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        public async Task SaveAllSymbolData(CancellationToken cancellationToken = default(CancellationToken))
        {
            while (!SymbolDataQueue.IsEmpty)
            {
                var startDate = DateTime.Now;
                List<SymbolData> data;
                if (SymbolDataQueue.TryDequeue(out data))
                {
                    await tsetmcUnitOfWork.AddSymbolData(data, cancellationToken);
                    System.Console.WriteLine($"Save Data to database:{(DateTime.Now - startDate).TotalSeconds}");
                }
            }
        }

        public async Task GetAllCapitalIncreaseAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var symbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);

            var startDate = DateTime.Now;
            foreach (var symbol in symbols)
            {
                await GetCapitalIncreaseAsync(symbol.IId, cancellationToken);
            }

            System.Console.WriteLine($"save CapitalIncrease :{ (DateTime.Now - startDate).TotalSeconds}");
        }

        private async Task GetCapitalIncreaseAsync(string iid, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 0)
        {
            try
            {
                List<CapitalIncrease> entities = new();

                var response = await httpClient.GetAsync($"Loader.aspx?Partree=15131H&i={iid}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError("Error in Get Capital Increase");
                    Console.WriteLine("Error in Get Capital Increase");
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

                foreach (var tr in trs)
                {
                    var tds = tr.SelectNodes("td");
                    entities.Add(new CapitalIncrease(iid, tds));
                }

                await tsetmcUnitOfWork.AddCapitalIncreaseAsync(iid, entities, cancellationToken);
            }
            catch
            {
                if (numberOfTries < 2)
                    await GetCapitalIncreaseAsync(iid, cancellationToken, numberOfTries++);
                else
                    throw;
            }
        }
    }
}