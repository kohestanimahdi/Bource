using Bource.Common.Models;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Common;
using Bource.Models.Data.Tsetmc;
using Bource.Services.Crawlers.Tsetmc.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Tsetmc
{
    public class TseSymbolDataProvider
    {
        private readonly ILogger<TsetmcCrawlerService> logger;
        private readonly TsetmcCrawlerService tsetmcCrawlerService;
        private readonly TseClientService TseClientService;
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;
        private static readonly List<SymbolData> SymbolData = new();
        private static readonly ConcurrentQueue<List<SymbolData>> SymbolDataQueue = new();
        private static readonly Dictionary<long, FillSymbolData> oneTimeData = new();

        public TseSymbolDataProvider(ILoggerFactory loggerFactory, ITsetmcUnitOfWork tsetmcUnitOfWork)
        {
            logger = loggerFactory?.CreateLogger<TsetmcCrawlerService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
        }

        public TseSymbolDataProvider(HttpClient httpClient)
        {
            LoggerFactory loggerFactory = new LoggerFactory();
            logger = new Logger<TsetmcCrawlerService>(loggerFactory);

            tsetmcUnitOfWork = new TsetmcUnitOfWork(new MongoDbSetting { ConnectionString = "mongodb://localhost:27017/", DataBaseName = "BourceInformation" });

            tsetmcCrawlerService = new TsetmcCrawlerService(httpClient);
            TseClientService = new TseClientService();
        }


        public Dictionary<long, FillSymbolData> GetOneTimeData() => oneTimeData;
        public void ClearOneTimeData() => oneTimeData.Clear();
        public static void FillOneTimeData(List<FillSymbolData> data)
        {
            foreach (var d in data)
                if (!oneTimeData.ContainsKey(d.InsCode))
                    oneTimeData[d.InsCode] = d;
        }
        public static void AddSymbolDataRange(List<SymbolData> data)
        {
            SymbolDataQueue.Enqueue(data);

            if (oneTimeData.Any())
                foreach (var d in data)
                {
                    if (oneTimeData.ContainsKey(d.InsCode))
                    {
                        var oneTime = oneTimeData[d.InsCode];
                        d.FillData(oneTime.MonthAverageValue, oneTime.FloatingStock, oneTime.GroupPE);
                    }

                    SymbolData.Add(d);
                }
        }
        public async Task SaveSymbolDataFromQueue(CancellationToken cancellationToken = default(CancellationToken))
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


        public async Task AddOrUpdateSymbols(CancellationToken cancellationToken = default(CancellationToken))
        {
            var tseSymbols = await tsetmcCrawlerService.GetSymbolsAsync(cancellationToken);
            var (tseClientSymbols, _) = await TseClientService.GetSymbolAndSharingAsync();
            var existsSymbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);

            foreach (var symbol in existsSymbols)
            {
                var tseSymbol = tseSymbols.FirstOrDefault(i => i.InsCode == symbol.InsCode);
                var tseClientSymbol = tseClientSymbols.FirstOrDefault(i => i.InsCode == symbol.InsCode);
                if (tseSymbol is not null && tseClientSymbol is not null)
                {
                    //update both
                    symbol.UpdateFromTsetmc(tseSymbol);
                    symbol.UpdateFromTseClient(tseClientSymbol);
                    symbol.ExistInType = Bource.Models.Data.Enums.SymbolExistInType.Both;

                    tseSymbols.Remove(tseSymbol);
                    tseClientSymbols.Remove(tseClientSymbol);
                }
                else if (tseSymbol is null && tseClientSymbol is not null)
                {
                    symbol.UpdateFromTseClient(tseClientSymbol);
                    tseClientSymbols.Remove(tseClientSymbol);
                }
                else if (tseSymbol is not null && tseClientSymbol is null)
                {
                    symbol.UpdateFromTsetmc(tseSymbol);
                    tseSymbols.Remove(tseSymbol);
                }

                await tsetmcUnitOfWork.UpdateSymbolAsync(symbol);
            }

            var symbolsToAdd = new List<Symbol>();
            if (tseSymbols.Any() && tseClientSymbols.Any())
            {
                foreach (var tseSymbol in tseSymbols)
                {
                    var tseClientSymbol = tseClientSymbols.FirstOrDefault(i => i.InsCode == tseSymbol.InsCode);
                    if (tseClientSymbol is not null)
                    {
                        tseSymbol.UpdateFromTseClient(tseClientSymbol);
                        tseSymbol.ExistInType = Bource.Models.Data.Enums.SymbolExistInType.Both;

                        tseClientSymbols.Remove(tseClientSymbol);
                    }
                    symbolsToAdd.Add(tseSymbol);
                }
            }

            if (tseClientSymbols.Any())
                symbolsToAdd.AddRange(tseClientSymbols);

            await tsetmcUnitOfWork.AddSymbolsRangeAsync(symbolsToAdd, cancellationToken);
        }

    }
}
