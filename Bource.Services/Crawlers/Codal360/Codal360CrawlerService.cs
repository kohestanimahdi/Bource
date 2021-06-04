using Bource.Common.Models;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Common;
using Bource.Models.Data.Enums;
using Bource.Models.Data.FipIran;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bource.Common.Utilities;
using System.Linq;
using Bource.Services.Crawlers.Codal360.Models;

namespace Bource.Services.Crawlers.Codal360
{
    public class Codal360CrawlerService : ICodal360CrawlerService, IScopedDependency
    {
        private string baseUrl => httpClient.BaseAddress.ToString();
        private readonly HttpClient httpClient;
        private readonly ILogger<Codal360CrawlerService> logger;
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;
        public Codal360CrawlerService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, ITsetmcUnitOfWork tsetmcUnitOfWork)
        {
            logger = loggerFactory?.CreateLogger<Codal360CrawlerService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            httpClient = httpClientFactory?.CreateClient(nameof(Codal360CrawlerService)) ?? throw new ArgumentNullException(nameof(httpClientFactory));

            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
        }



        public async Task UpdateSymbolsCodalURLAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var symbols = (await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken)).Where(i => string.IsNullOrWhiteSpace(i.CodalURL)).ToList();

            await ApplicationHelpers.DoFunctionsOFListWithMultiTask<Symbol>(symbols, UpdateSymbolCodalURLAsync, cancellationToken);
        }
        public async Task UpdateSymbolsCodalImageAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var symbols = (await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken)).Where(i => string.IsNullOrWhiteSpace(i.Logo) && !string.IsNullOrWhiteSpace(i.CodalURL)).ToList();

            await ApplicationHelpers.DoFunctionsOFListWithMultiTask<Symbol>(symbols, UpdateSymbolCodalImageAsync, cancellationToken);
        }

        private async Task UpdateSymbolCodalURLAsync(Symbol symbol, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 0)
        {
            try
            {
                var response = await httpClient.GetAsync($"fa/search_symbol/?q={symbol.Sign.Trim()}", cancellationToken);
                var companyName = symbol.CompanyName?.Split('.')[^1]?.Trim();

                if (!response.IsSuccessStatusCode)
                {
                    response = await httpClient.GetAsync($"fa/search_symbol/?q={companyName}", cancellationToken);
                    if (!response.IsSuccessStatusCode)
                    {
                        logger.LogError($"Error in Get Codal Url {symbol.InsCode}");
                    }


                }

                var result = await response.Content.ReadAsStringAsync(cancellationToken);

                var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CodalSearchSymbolResponse>>(result);
                if (responseObject is null || !responseObject.Any())
                {
                    response = await httpClient.GetAsync($"fa/search_symbol/?q={companyName}", cancellationToken);
                    if (!response.IsSuccessStatusCode)
                    {
                        logger.LogError($"Error in Get Codal Url {symbol.InsCode}");
                        return;
                    }
                    result = await response.Content.ReadAsStringAsync(cancellationToken);
                    responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CodalSearchSymbolResponse>>(result);
                }
                if (responseObject is null || !responseObject.Any())
                    return;

                var item = responseObject.FirstOrDefault(i => i.Symbol.FixPersianLetters().Equals(symbol.Sign.FixPersianLetters()) || i.Title.FixPersianLetters().Equals(companyName.FixPersianLetters()));
                if (item is null)
                    return;

                item.UpdateSymbol(symbol, baseUrl);
                await tsetmcUnitOfWork.UpdateSymbolAsync(symbol, cancellationToken);
            }
            catch (Exception)
            {
                if (numberOfTries < 2)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    await UpdateSymbolCodalURLAsync(symbol, cancellationToken, numberOfTries + 1);
                }
                else
                    throw;
            }
        }

        private async Task UpdateSymbolCodalURLAsync2(Symbol symbol, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 0)
        {
            if (string.IsNullOrWhiteSpace(symbol.CompanyName))
                return;
            var companyName = symbol.CompanyName.Split('.')[^1];

            var response = await httpClient.GetAsync($"fa/publishers?symbol={companyName.Replace(' ', '+')}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in Get Codal Url {symbol.InsCode}");
                return;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var tds = htmlDoc.DocumentNode.SelectNodes("//tbody[@id='template-container']/tr");
            if (tds is null)
                return;

            var trs = tds[0].SelectNodes("td/a");
            if (trs is null || !trs.Any())
                return;

            var href = trs[0].Attributes["href"].Value;

            var url = new Uri(baseUrl + (href.StartsWith("/") ? href.Remove(0, 1) : href));
            symbol.CodalURL = url.ToString();
            await tsetmcUnitOfWork.UpdateSymbolAsync(symbol, cancellationToken);
        }

        private async Task UpdateSymbolCodalImageAsync(Symbol symbol, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 0)
        {

            var response = await httpClient.GetAsync(symbol.CodalURL, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in Get Codal Image {symbol.InsCode}");
                return;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var image = htmlDoc.DocumentNode.SelectSingleNode("//img[@alt='لوگوی شرکت']");
            if (image is null)
                return;

            var source = image.Attributes["src"].Value;
            if (source.Equals("/media//logo/logo.png")) return;

            var url = new Uri(baseUrl + (source.StartsWith("/") ? source.Remove(0, 1) : source));
            symbol.Logo = url.ToString();
            await tsetmcUnitOfWork.UpdateSymbolAsync(symbol, cancellationToken);
        }
    }
}
