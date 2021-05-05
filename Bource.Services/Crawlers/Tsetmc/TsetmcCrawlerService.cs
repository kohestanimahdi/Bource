﻿using Bource.Common.Models;
using Bource.Common.Utilities;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Common;
using Bource.Models.Data.Tsetmc;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using MD.PersianDateTime.Standard;

namespace Bource.Services.Crawlers.Tsetmc
{
    public class TsetmcCrawlerService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<TsetmcCrawlerService> logger;
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;
        private static readonly ConcurrentQueue<List<SymbolData>> SymbolDataQueue = new();
        public TsetmcCrawlerService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, ITsetmcUnitOfWork tsetmcUnitOfWork)
        {
            logger = loggerFactory?.CreateLogger<TsetmcCrawlerService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            httpClient = httpClientFactory?.CreateClient() ?? throw new ArgumentNullException(nameof(httpClientFactory));

            httpClient.BaseAddress = new Uri("http://www.tsetmc.com/");
            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
        }

        public TsetmcCrawlerService(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            httpClient.BaseAddress = new Uri("http://www.tsetmc.com/");
            httpClient.Timeout = TimeSpan.FromSeconds(1.5);

            tsetmcUnitOfWork = new TsetmcUnitOfWork(new MongoDbSetting { ConnectionString = "mongodb://localhost:27017/", DataBaseName = "BourceInformation" });

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

        public async Task GetCashMarketAtGlanceAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await httpClient.GetAsync("Loader.aspx?ParTree=15", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error in get client symbol data");
                Console.WriteLine("Error in get client symbol data");
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
                    PersianDateTime time;
                    if (!PersianDateTime.TryParse($"14{trs[9].InnerText}", out time))
                        time = PersianDateTime.Now;

                    stockCashMarketAtGlance = new StockCashMarketAtGlance
                    {
                        CreateDate = DateTime.Now,
                        Status = trs[1].GetText(),
                        OverallIndex = trs[3].FirstChild.ConvertToDecimal(),
                        OverallIndexChange = trs[3].SelectSingleNode("div").ConvertToNegativePositiveDecimal(),
                        OverallIndexEqualWeight = trs[5].FirstChild.ConvertToDecimal(),
                        OverallIndexEqualWeightChange = trs[5].SelectSingleNode("div").ConvertToNegativePositiveDecimal(),
                        ValueOfMarket = trs[7].SelectSingleNode("div").Attributes["title"].Value.ConvertToDecimal(),
                        Time = time.ToDateTime(),
                        NumberOfTransaction = trs[11].ConvertToDecimal(),
                        ValueOfTransaction = trs[13].SelectSingleNode("div").Attributes["title"].Value.ConvertToDecimal(),
                        Turnover = trs[15].SelectSingleNode("div").Attributes["title"].Value.ConvertToDecimal(),
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
                    PersianDateTime time;
                    if (!PersianDateTime.TryParse($"14{trs[7].InnerText}", out time))
                        time = PersianDateTime.Now;

                    stockCashMarketAtGlance = new OTCCashMarketAtGlance
                    {
                        CreateDate = DateTime.Now,
                        Status = trs[1].GetText(),
                        OverallIndex = trs[3].FirstChild.ConvertToDecimal(),
                        OverallIndexChange = trs[3].SelectSingleNode("div").ConvertToNegativePositiveDecimal(),
                        ValueOfFirstAndSecondMarket = trs[5].SelectSingleNode("div").Attributes["title"].Value.ConvertToDecimal(),

                        Time = time.ToDateTime(),
                        NumberOfTransaction = trs[9].ConvertToDecimal(),
                        ValueOfTransaction = trs[11].SelectSingleNode("div").Attributes["title"].Value.ConvertToDecimal(),
                        Turnover = trs[13].SelectSingleNode("div").Attributes["title"].Value.ConvertToDecimal(),
                    };
                }

            }

            return stockCashMarketAtGlance;
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
    }
}
