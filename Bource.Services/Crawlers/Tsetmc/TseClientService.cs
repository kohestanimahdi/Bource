using Bource.Common.Models;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Common;
using Bource.Models.Data.Enums;
using Bource.Models.Data.Tsetmc;
using Bource.Services.Crawlers.Tsetmc.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TseClient;

namespace Bource.Services.Crawlers.Tsetmc
{
    public class TseClientService : IScopedDependency
    {
        private readonly ILogger<TseClientService> logger;
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;
        private readonly TseClient.WebServiceTseClientSoap tseClientSoap;

        public TseClientService(ILoggerFactory loggerFactory, ITsetmcUnitOfWork tsetmcUnitOfWork)
        {
            logger = loggerFactory?.CreateLogger<TseClientService>() ?? throw new ArgumentNullException(nameof(loggerFactory));

            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));

            tseClientSoap = new WebServiceTseClientSoapClient(WebServiceTseClientSoapClient.EndpointConfiguration.WebServiceTseClientSoap);
        }

        public TseClientService()
        {
            tsetmcUnitOfWork = new TsetmcUnitOfWork(new MongoDbSetting { ConnectionString = "mongodb://localhost:27017/", DataBaseName = "BourceInformation" });

            tseClientSoap = new WebServiceTseClientSoapClient(WebServiceTseClientSoapClient.EndpointConfiguration.WebServiceTseClientSoap);
        }

        public async Task<(List<Symbol>, List<TseShareInfo>)> GetSymbolAndSharingAsync()
        {
            var result = await tseClientSoap.InstrumentAndShareAsync(new InstrumentAndShareRequest
            {
                Body = new InstrumentAndShareRequestBody
                {
                    DEven = 0,
                    LastID = 0
                }
            });

            var text = result.Body.InstrumentAndShareResult.Split('@');
            var symbolsText = text[0];
            var sharesText = text[1];

            var responseSymbols = new List<Symbol>();
            var responseShareInfos = new List<TseShareInfo>();
            if (!string.IsNullOrWhiteSpace(symbolsText) && symbolsText != "*")
            {
                string[] array = symbolsText.Split(';');
                foreach (var item in array)
                {
                    string[] row = item.Split(',');
                    Symbol symbol = new Symbol(row);
                    responseSymbols.Add(symbol);
                }
            }
            if (!string.IsNullOrEmpty(sharesText))
            {
                string[] array2 = sharesText.Split(';');
                for (int j = 0; j < array2.Length; j++)
                {
                    TseShareInfo tseShareInfo = new(array2[j]);
                    responseShareInfos.Add(tseShareInfo);
                }

            }

            return (responseSymbols, responseShareInfos);
        }
        public async Task GetInsturmentsClosingPriceAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var (symbols, shareInfos) = await GetSymbolAndSharingAsync();

            List<Task> tasks = new();
            int n = 0;
            int count = symbols.Count / 15;

            while (n < symbols.Count)
            {
                var subSymbols = symbols.Skip(n).Take(count).ToList();
                tasks.Add(Task.Run(() => GetInsturmentsClosingPricesAsync(subSymbols, shareInfos, cancellationToken)));
                n += count;
            }

            await Task.WhenAll(tasks);

        }

        private async Task GetInsturmentsClosingPricesAsync(List<Symbol> symbols, List<TseShareInfo> shareInfos, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 0)
        {
            try
            {
                var requestText = string.Join(';', symbols.Select(i => $"{i.InsCode},0,0"));

                string inscodeCompress = Common.Utilities.ApplicationHelpers.TseClientCompress(requestText);

                var insturmentClosingPriceObject = await tseClientSoap.DecompressAndGetInsturmentClosingPriceAsync(
                    new DecompressAndGetInsturmentClosingPriceRequest
                    {
                        Body = new DecompressAndGetInsturmentClosingPriceRequestBody
                        {
                            insCodes = inscodeCompress
                        }
                    });

                var insturmentClosingPrice = insturmentClosingPriceObject.Body.DecompressAndGetInsturmentClosingPriceResult;

                if (insturmentClosingPrice.Equals("*"))
                    return;

                foreach (string text in insturmentClosingPrice.Split('@'))
                {
                    if (string.IsNullOrEmpty(text))
                        continue;

                    List<ClosingPriceInfo> cpList = new();
                    string[] items = text.Split(';');

                    foreach (var row in items)
                    {
                        try
                        {
                            ClosingPriceInfo closingPriceInfo = new(row);

                            cpList.Add(closingPriceInfo);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "UpdateClosingPrices[Row:" + row + "]");
                            continue;
                        }
                    }

                    if (cpList.Any())
                    {
                        var symbol = symbols.FirstOrDefault(i => i.InsCode == cpList.First().InsCode);
                        if (symbol is not null)
                        {
                            var capitalIncreaseAndProfitItems = ConvertToClosingPriceType(symbol, cpList, shareInfos, ClosingPriceTypes.CapitalIncreaseAndProfit);
                            var capitalIncreaseItems = ConvertToClosingPriceType(symbol, cpList, shareInfos, ClosingPriceTypes.CapitalIncrease);
                            cpList.ForEach(i => i.Type = ClosingPriceTypes.NoPriceAdjustment);

                            await tsetmcUnitOfWork.AppendClosingPriceInfoAsync(cpList.Where(i => i.ZTotTran != 0).ToList(), cancellationToken);
                            await tsetmcUnitOfWork.AppendClosingPriceInfoAsync(capitalIncreaseAndProfitItems.Where(i => i.ZTotTran != 0).ToList(), cancellationToken);
                            await tsetmcUnitOfWork.AppendClosingPriceInfoAsync(capitalIncreaseItems.Where(i => i.ZTotTran != 0).ToList(), cancellationToken);
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (numberOfTries < 2)
                    await GetInsturmentsClosingPricesAsync(symbols, shareInfos, cancellationToken, numberOfTries++);
                else
                    throw;
            }

        }

        public List<ClosingPriceInfo> ConvertToClosingPriceType(Symbol symbol, List<ClosingPriceInfo> cp, List<TseShareInfo> tseShares, ClosingPriceTypes types)
        {
            List<ClosingPriceInfo> list = new List<ClosingPriceInfo>();
            decimal num2 = 1m;
            list.Add(cp[cp.Count - 1]);
            double num3 = 0.0;
            if (types == ClosingPriceTypes.CapitalIncreaseAndProfit)
            {
                for (int num4 = cp.Count - 2; num4 >= 0; num4--)
                {
                    if (cp[num4].PClosing != cp[num4 + 1].PriceYesterday)
                    {
                        num3 += 1.0;
                    }
                }
            }
            if ((types == ClosingPriceTypes.CapitalIncreaseAndProfit && num3 / (double)cp.Count < 0.08) || types == ClosingPriceTypes.CapitalIncrease)
            {
                int i;
                for (i = cp.Count - 2; i >= 0; i--)
                {
                    if (types == ClosingPriceTypes.CapitalIncreaseAndProfit && cp[i].PClosing != cp[i + 1].PriceYesterday)
                    {
                        num2 = num2 * cp[i + 1].PriceYesterday / cp[i].PClosing;
                    }
                    else if (types == ClosingPriceTypes.CapitalIncrease && cp[i].PClosing != cp[i + 1].PriceYesterday && tseShares.Exists((TseShareInfo p) => p.InsCode == symbol.InsCode && p.DEven == cp[i + 1].DEven))
                    {
                        num2 *= tseShares.Find((TseShareInfo p) => p.InsCode == symbol.InsCode && p.DEven == cp[i + 1].DEven).NumberOfShareOld / tseShares.Find((TseShareInfo p) => p.InsCode == symbol.InsCode && p.DEven == cp[i + 1].DEven).NumberOfShareNew;
                    }
                    ClosingPriceInfo closingPriceInfo = new ClosingPriceInfo();
                    closingPriceInfo.InsCode = cp[i].InsCode;
                    closingPriceInfo.DEven = cp[i].DEven;
                    closingPriceInfo.PClosing = Math.Round(num2 * cp[i].PClosing, 2);
                    closingPriceInfo.PDrCotVal = Math.Round(num2 * cp[i].PDrCotVal, 2);
                    closingPriceInfo.ZTotTran = cp[i].ZTotTran;
                    closingPriceInfo.QTotTran5J = cp[i].QTotTran5J;
                    closingPriceInfo.QTotCap = cp[i].QTotCap;
                    closingPriceInfo.PriceMin = Math.Round(num2 * cp[i].PriceMin);
                    closingPriceInfo.PriceMax = Math.Round(num2 * cp[i].PriceMax);
                    closingPriceInfo.PriceYesterday = Math.Round(num2 * cp[i].PriceYesterday);
                    closingPriceInfo.PriceFirst = Math.Round(num2 * cp[i].PriceFirst, 2);
                    closingPriceInfo.Type = types;
                    list.Add(closingPriceInfo);
                }
                cp.Clear();
                for (int num5 = list.Count - 1; num5 >= 0; num5--)
                {
                    cp.Add(list[num5]);
                }
            }

            return cp;
        }
    }
}