using Bource.Common.Models;
using Bource.Common.Utilities;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Common;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Tsetmc
{
    public class TsetmcCrawlerService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<TsetmcCrawlerService> logger;
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;

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

            tsetmcUnitOfWork = new TsetmcUnitOfWork(new MongoDbSetting { ConnectionString = "mongodb://localhost:27017/", DataBaseName = "BourceInformation" });
        }

        public async Task GetOrUpdateSymbolGroups(CancellationToken cancellationToken = default(CancellationToken))
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
    }
}
