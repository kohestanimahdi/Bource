using Bource.Common.Models;
using Bource.Data.Informations.UnitOfWorks;
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
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;
        private static readonly List<SymbolData> SymbolData = new();
        private static readonly ConcurrentQueue<List<SymbolData>> SymbolDataQueue = new();
        private static readonly Dictionary<string, FillSymbolData> oneTimeData = new();

        public TseSymbolDataProvider(ILoggerFactory loggerFactory, ITsetmcUnitOfWork tsetmcUnitOfWork)
        {
            logger = loggerFactory?.CreateLogger<TsetmcCrawlerService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
        }

        public TseSymbolDataProvider()
        {
            LoggerFactory loggerFactory = new LoggerFactory();
            logger = new Logger<TsetmcCrawlerService>(loggerFactory);

            tsetmcUnitOfWork = new TsetmcUnitOfWork(new MongoDbSetting { ConnectionString = "mongodb://localhost:27017/", DataBaseName = "BourceInformation" });
        }


        public Dictionary<string, FillSymbolData> GetOneTimeData() => oneTimeData;
        public void ClearOneTimeData() => oneTimeData.Clear();
        public static void FillOneTimeData(List<FillSymbolData> data)
        {
            foreach (var d in data)
                if (!oneTimeData.ContainsKey(d.IId))
                    oneTimeData[d.IId] = d;
        }
        public static void AddSymbolDataRange(List<SymbolData> data)
        {
            SymbolDataQueue.Enqueue(data);

            if (oneTimeData.Any())
                foreach (var d in data)
                {
                    if (oneTimeData.ContainsKey(d.IId))
                    {
                        var oneTime = oneTimeData[d.IId];
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
    }
}
