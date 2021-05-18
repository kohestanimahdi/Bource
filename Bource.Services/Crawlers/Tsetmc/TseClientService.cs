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

        public static string Compress(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            MemoryStream stream = new MemoryStream();
            using (var stream2 = new GZipStream(stream, CompressionMode.Compress, true))
            {
                stream2.Write(bytes, 0, bytes.Length);
            }
            stream.Position = 0L;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            byte[] dst = new byte[buffer.Length + 4];
            Buffer.BlockCopy(buffer, 0, dst, 4, buffer.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(bytes.Length), 0, dst, 0, 4);
            return Convert.ToBase64String(dst);
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

        public async Task GetInsturmentsClosingPriceAsync2(CancellationToken cancellationToken = default(CancellationToken))
        {
            var Insturments = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);
            var num = 0;
            var num2 = 0;
            long[][] array2 = new long[Insturments.Count][];
            int num3 = 0;
            foreach (var selectedInstrument in Insturments)
            {

                int num4 = 0;

                if ((selectedInstrument.YMarNSC != "NO" || num4 != num) && (selectedInstrument.YMarNSC != "ID" || num4 != num2))
                {
                    array2[num3] = new long[3];
                    array2[num3][0] = 0;
                    array2[num3][1] = 0;
                    array2[num3][2] = ((selectedInstrument.YMarNSC != "NO") ? 1 : 0);
                    num3++;
                }
            }

            int num5 = ((num3 % 20 != 0) ? (num3 / 20 + 1) : (num3 / 20));
            for (int i = 0; i < num5; i++)
            {
                int num6 = ((i < num5 - 1) ? 20 : (num3 % 20));
                long[][] array3 = new long[num6][];
                for (int j = 0; j < num6; j++)
                {
                    array3[j] = new long[3];
                    array3[j][0] = array2[i * 20 + j][0];
                    array3[j][1] = array2[i * 20 + j][1];
                    array3[j][2] = array2[i * 20 + j][2];
                }
                string text2 = "";
                long[][] array4 = array3;
                foreach (long[] array5 in array4)
                {
                    object obj = text2;
                    text2 = string.Concat(obj, array5[0], ",", array5[1], ",", array5[2], ";");
                }
                text2 = text2.Substring(0, text2.Length - 1);
                var insturmentClosingPriceObject = await tseClientSoap.DecompressAndGetInsturmentClosingPriceAsync(
                    new DecompressAndGetInsturmentClosingPriceRequest
                    {
                        Body = new DecompressAndGetInsturmentClosingPriceRequestBody
                        {
                            insCodes = Compress(text2)
                        }
                    });

                var insturmentClosingPrice = insturmentClosingPriceObject.Body.DecompressAndGetInsturmentClosingPriceResult;

                if (insturmentClosingPrice.Equals("*"))
                {
                    continue;
                }

                string[] array7 = insturmentClosingPrice.Split('@');
                foreach (string text3 in array7)
                {
                    if (string.IsNullOrEmpty(text3))
                    {
                        continue;
                    }
                    List<ClosingPriceInfo> cpList = new();
                    string[] array8 = text3.Split(';');
                    for (int m = 0; m < array8.Length; m++)
                    {
                        ClosingPriceInfo closingPriceInfo = new();
                        try
                        {
                            string[] array9 = array8[m].Split(',');
                            closingPriceInfo.InsCode = Convert.ToInt64(array9[0].ToString());
                            closingPriceInfo.DEven = Convert.ToInt32(array9[1].ToString());
                            closingPriceInfo.PClosing = Convert.ToDecimal(array9[2].ToString());
                            closingPriceInfo.PDrCotVal = Convert.ToDecimal(array9[3].ToString());
                            closingPriceInfo.ZTotTran = Convert.ToDecimal(array9[4].ToString());
                            closingPriceInfo.QTotTran5J = Convert.ToDecimal(array9[5].ToString());
                            closingPriceInfo.QTotCap = Convert.ToDecimal(array9[6].ToString());
                            closingPriceInfo.PriceMin = Convert.ToDecimal(array9[7].ToString());
                            closingPriceInfo.PriceMax = Convert.ToDecimal(array9[8].ToString());
                            closingPriceInfo.PriceYesterday = Convert.ToDecimal(array9[9].ToString());
                            closingPriceInfo.PriceFirst = Convert.ToDecimal(array9[10].ToString());
                            cpList.Add(closingPriceInfo);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "UpdateClosingPrices[Row:" + array8[m] + "]");
                            continue;
                        }
                    }
                    //FileService.WriteClosingPrices(cpList);

                }

            }
        }

        public async Task GetInsturmentsClosingPriceAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var (symbols, shareInfos) = await GetSymbolAndSharingAsync();

            foreach (var item in symbols)
            {
                string inscodeCompress = Compress($"{item.InsCode},0,0");

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
                    continue;

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

                    var capitalIncreaseAndProfitItems = ConvertToClosingPriceType(item, cpList, shareInfos, ClosingPriceTypes.CapitalIncreaseAndProfit);
                    var capitalIncreaseItems = ConvertToClosingPriceType(item, cpList, shareInfos, ClosingPriceTypes.CapitalIncrease);
                    cpList.ForEach(i => i.Type = ClosingPriceTypes.NoPriceAdjustment);

                    await tsetmcUnitOfWork.AppendClosingPriceInfoAsync(cpList);
                    await tsetmcUnitOfWork.AppendClosingPriceInfoAsync(capitalIncreaseAndProfitItems);
                    await tsetmcUnitOfWork.AppendClosingPriceInfoAsync(capitalIncreaseItems);
                }
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