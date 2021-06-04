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
    public class TseSymbolDataProvider : ITseSymbolDataProvider, IScopedDependency
    {
        private readonly ILogger<TsetmcCrawlerService> logger;
        private readonly ITsetmcCrawlerService tsetmcCrawlerService;
        private readonly ITseClientService TseClientService;
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;
        //private static readonly List<SymbolData> SymbolData = new();
        private static readonly ConcurrentBag<SymbolData> SymbolDataBag = new();
        private static readonly ConcurrentQueue<List<SymbolData>> SymbolDataQueue = new();
        private static readonly Dictionary<long, FillSymbolData> oneTimeData = new();

        public TseSymbolDataProvider(ILoggerFactory loggerFactory, ITsetmcUnitOfWork tsetmcUnitOfWork, ITsetmcCrawlerService tsetmcCrawlerService, ITseClientService TseClientService)
        {
            logger = loggerFactory?.CreateLogger<TsetmcCrawlerService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
            this.tsetmcCrawlerService = tsetmcCrawlerService ?? throw new ArgumentNullException(nameof(tsetmcCrawlerService));
            this.TseClientService = TseClientService ?? throw new ArgumentNullException(nameof(TseClientService));
        }



        public Dictionary<long, FillSymbolData> GetOneTimeData() => oneTimeData;
        public void ClearOneTimeData() => oneTimeData.Clear();
        public static void FillOneTimeData(List<FillSymbolData> data)
        {
            foreach (var d in data)
                if (!oneTimeData.ContainsKey(d.InsCode))
                    oneTimeData[d.InsCode] = d;
        }

        public static void AddSymbolDataToQueue(List<SymbolData> data)
            => SymbolDataQueue.Enqueue(data);

        public void SaveSymbolData()
        {
            if (!SymbolDataQueue.IsEmpty)
            {
                List<SymbolData> data;
                if (SymbolDataQueue.TryDequeue(out data))
                {
                    FillSymbolData(data);

                    Task.Run(() => AddSymbolDataToMemory(data));
                    Task.Run(() => AddSymbolDataToDataBase(data));
                }
            }
        }

        private void FillSymbolData(List<SymbolData> data)
        {
            var startTime = DateTime.Now;
            foreach (var d in data)
            {
                if (oneTimeData.ContainsKey(d.InsCode))
                {
                    var oneTime = oneTimeData[d.InsCode];
                    d.FillData(oneTime.MonthAverageValue, oneTime.FloatingStock, oneTime.GroupPE);
                }
            }
            logger.LogInformation($"Mapping data:{(DateTime.Now - startTime).TotalSeconds}");
        }

        private void AddSymbolDataToMemory(List<SymbolData> data)
        {
            var startTime = DateTime.Now;
            foreach (var d in data)
            {
                var lastSymbolData = SymbolDataBag.Where(i => i.InsCode == d.InsCode).OrderByDescending(i => i.LastUpdate).FirstOrDefault();
                if (lastSymbolData is null || !lastSymbolData.Equals(d))
                {
                    SymbolDataBag.Add(d);
                }
            }
            logger.LogInformation($"Add In memory:{(DateTime.Now - startTime).TotalSeconds}");
        }

        private async Task AddSymbolDataToDataBase(List<SymbolData> data)
        {
            var startTime = DateTime.Now;
            await tsetmcUnitOfWork.AddSymbolData(data);
            logger.LogInformation($"Add to database:{(DateTime.Now - startTime).TotalSeconds}");

        }

        #region OldCodeForSaveData
        //public static void AddSymbolDataRange(List<SymbolData> data)
        //{
        //    var startTime = DateTime.Now;

        //    if (oneTimeData.Any())
        //    {
        //        foreach (var d in data)
        //        {
        //            if (oneTimeData.ContainsKey(d.InsCode))
        //            {
        //                var oneTime = oneTimeData[d.InsCode];
        //                d.FillData(oneTime.MonthAverageValue, oneTime.FloatingStock, oneTime.GroupPE);
        //            }

        //            var lastSymbolData = SymbolData.Where(i => i.InsCode == d.InsCode).OrderByDescending(i => i.LastUpdate).FirstOrDefault();

        //            if (lastSymbolData is null || !lastSymbolData.Equals(d))
        //            {
        //                SymbolData.Add(d);
        //            }

        //        }

        //        Console.WriteLine($"Enter to memory list: {(DateTime.Now - startTime).TotalSeconds}");
        //    }

        //    SymbolDataQueue.Enqueue(data);

        //}

        //public async Task SaveSymbolDataFromQueue(CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    while (true)
        //    {
        //        var startDate = DateTime.Now;
        //        if (!SymbolDataQueue.IsEmpty)
        //        {
        //            List<SymbolData> data;
        //            if (SymbolDataQueue.TryDequeue(out data))
        //            {
        //                await tsetmcUnitOfWork.AddSymbolData(data, cancellationToken);
        //                System.Console.WriteLine($"Save Data to database:{(DateTime.Now - startDate).TotalSeconds}");
        //                System.Console.WriteLine($"Count of Queue:{SymbolDataQueue.Count}");
        //            }
        //        }
        //        else
        //            await Task.Delay(TimeSpan.FromSeconds(5));
        //    }
        //}
        #endregion


        public async Task AddOrUpdateSymbols(CancellationToken cancellationToken = default(CancellationToken))
        {
            var tseSymbols = await tsetmcCrawlerService.GetSymbolsAsync(cancellationToken);
            var (tseClientSymbols, _) = await TseClientService.GetSymbolAndSharingAsync();
            var regex = new System.Text.RegularExpressions.Regex(@"[^0-9]+\d{1}$");
            tseSymbols = tseSymbols.Where(p => !regex.IsMatch(p.Sign)).ToList();
            tseClientSymbols = tseClientSymbols.Where(p => !regex.IsMatch(p.Sign)).ToList();

            var existsSymbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);

            int n = 0;
            Common.Utilities.ConsoleHelper.WriteProgressBar(n);

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

                n++;
                Common.Utilities.ConsoleHelper.WriteProgressBar((int)(n * 100.0 / existsSymbols.Count), true);
            }
            Common.Utilities.ConsoleHelper.WriteProgressBar(100, true);

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
