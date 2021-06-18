using Bource.Common.Models;
using Bource.Common.Utilities;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Common;
using Bource.Models.Data.Enums;
using Bource.Models.Data.FipIran;
using Bource.Models.Data.Tsetmc;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.FipIran
{
    public class FipiranCrawlerService : IFipiranCrawlerService, IScopedDependency
    {


        private readonly ILogger<FipiranCrawlerService> logger;
        private readonly IFipiranUnitOfWork fipiranUnitOfWork;
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;
        private readonly IHttpClientFactory httpClientFactory;
        private string className => nameof(FipiranCrawlerService);

        public FipiranCrawlerService(
            IHttpClientFactory httpClientFactory,
            ILoggerFactory loggerFactory,
            IFipiranUnitOfWork fipiranUnitOfWork,
            ITsetmcUnitOfWork tsetmcUnitOfWork)
        {
            logger = loggerFactory?.CreateLogger<FipiranCrawlerService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.fipiranUnitOfWork = fipiranUnitOfWork ?? throw new ArgumentNullException(nameof(fipiranUnitOfWork));
            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
        }

        public async Task GetNews(FipIranNewsTypes type, CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (type)
            {
                case FipIranNewsTypes.WorldOfEconomy:
                    await GetNews("News?Cat=0&Feeder=4", type, cancellationToken);
                    break;

                case FipIranNewsTypes.AssembliesAndCompanies:
                    await GetNews("News?Cat=4&Feeder=0", type, cancellationToken);
                    break;

                default:
                    break;
            }
        }

        private async Task GetNews(string url, FipIranNewsTypes type, CancellationToken cancellationToken = default(CancellationToken))
        {
            using var httpClient = httpClientFactory.CreateClient(className);

            var response = await httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in Get News Of FipIran {url}");
                return;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var news = new List<FipIranNews>();

            var divs = htmlDoc.DocumentNode.SelectNodes("//div[@data-children='.item']/div");

            if (news is not null)
                foreach (var div in divs)
                    news.Add(new FipIranNews(div, type));

            await fipiranUnitOfWork.AddIfNotExistNewsAsync(type, news, cancellationToken);
        }

        public async Task GetAssociations(CancellationToken cancellationToken = default(CancellationToken))
        {
            using var httpClient = httpClientFactory.CreateClient(className);

            var response = await httpClient.GetAsync("Codal/Invitation", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in Get News Of FipIran Codal/Invitation");
                return;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var items = new List<FipIranAssociation>();

            var trs = htmlDoc.DocumentNode.SelectNodes("//table[@id='list']/tbody/tr");

            if (trs is not null)
                foreach (var tr in trs)
                    items.Add(new FipIranAssociation(tr));

            await fipiranUnitOfWork.AddIfNotExistAssociationAsync(items, cancellationToken);
        }

        public async Task GetSubjectSymbols(CancellationToken cancellationToken = default(CancellationToken))
        {
            var symbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);
            await ApplicationHelpers.DoFunctionsOFListWithMultiTask(symbols, httpClientFactory, className, GetSubjectSymbol, cancellationToken);
        }

        private async Task GetSubjectSymbol(Symbol symbol, HttpClient httpClient, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 5)
        {
            var response = await httpClient.GetAsync($"Symbol/CompanyInfoIndex?symbolpara={symbol.Sign}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in get subject of symbol in fip iran");
                return;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var i = htmlDoc.DocumentNode.SelectSingleNode("//i[@class='fa fa-file-text']");
            if (i is null)
                return;

            var item = i.ParentNode.SelectSingleNode("div/p");
            if (item is null)
                return;

            symbol.Subject = item.GetText();

            await tsetmcUnitOfWork.UpdateSymbolAsync(symbol, cancellationToken);
        }

        public async Task GetIndicators(CancellationToken cancellationToken = default(CancellationToken))
        {
            using var httpClient = httpClientFactory.CreateClient(className);

            var response = await httpClient.GetAsync($"AllIndex/IndicesRevenue", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in Indicators");
                return;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var trs = htmlDoc.DocumentNode.SelectNodes("//table[@id='list']/tbody/tr");
            var symbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);

            List<Indicator> Indicators = new();
            if (trs is not null)
                foreach (var tr in trs)
                {
                    var tds = tr.SelectNodes("td");
                    var insCode = Convert.ToInt64(tds[0].GetQueryString("LVal30", httpClient.BaseAddress.ToString()));
                    var indicatorSymbols = await GetIndicatorSymbols(insCode, httpClient, cancellationToken);
                    Indicator indicator = new(tds, insCode, indicatorSymbols, symbols);
                    Indicators.Add(indicator);
                }

            await tsetmcUnitOfWork.AddIndicatorsAsync(Indicators, cancellationToken);
        }

        private async Task<HtmlNodeCollection> GetIndicatorSymbols(long insCode, HttpClient httpClient, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await httpClient.GetAsync($"IndexDetails/_IndexInstrument?Lval30={insCode}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in Indicators");
                return null;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            return htmlDoc.DocumentNode.SelectNodes("//table[@id='list1']/tbody/tr");
        }
    }
}