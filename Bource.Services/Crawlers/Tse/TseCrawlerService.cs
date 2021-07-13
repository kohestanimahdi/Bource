using Bource.Common.Models;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Enums;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Tse
{
    public class TseCrawlerService : IScopedDependency
    {
        #region Properties

        private readonly int numberOfTries = 5;
        private readonly TimeSpan delayBetweenTimeouts = TimeSpan.FromSeconds(1);
        private readonly bool throwExceptions = false;
        private readonly ILogger<TseCrawlerService> logger;
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;
        private readonly IHttpClientFactory httpClientFactory;
        private string className => nameof(TseCrawlerService);

        #endregion Properties

        #region Constructors

        public TseCrawlerService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, ITsetmcUnitOfWork tsetmcUnitOfWork)
        {
            logger = loggerFactory?.CreateLogger<TseCrawlerService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
        }

        #endregion Constructors

        public async Task GetPapersAsync(CancellationToken cancellationToken = default(CancellationToken))
        {

        }

        private async Task GetPapersAsync(PapersTypes papersTypes, CancellationToken cancellationToken = default(CancellationToken))
        {
            using var httpClient = httpClientFactory.CreateClient(className);

            var response = await httpClient.GetAsync($"json/Listing/ListingByName{(int)papersTypes}.json", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in GetPapers {Enum.GetName<PapersTypes>(papersTypes)}");
                return;
            }

            var papers = (await tsetmcUnitOfWork.GetPaperByTypeAsync(papersTypes, cancellationToken)) ?? new Models.Data.Tsetmc.Papers();

            papers.Title = Enum.GetName<PapersTypes>(papersTypes);
            papers.Type = papersTypes;

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

            await tsetmcUnitOfWork.AddOrUpdateIndicatorsAsync(Indicators, cancellationToken);
        }
    }
}