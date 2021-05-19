using Bource.Common.Models;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Enums;
using Bource.Models.Data.FipIran;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.FipIran
{
    public class FipiranCrawlerService
    {
        private string baseUrl { get; init; }
        private readonly HttpClient httpClient;
        private readonly ILogger<FipiranCrawlerService> logger;
        private readonly IFipiranUnitOfWork fipiranUnitOfWork;

        public FipiranCrawlerService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, IFipiranUnitOfWork fipiranUnitOfWork)
        {
            logger = loggerFactory?.CreateLogger<FipiranCrawlerService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            httpClient = httpClientFactory?.CreateClient() ?? throw new ArgumentNullException(nameof(httpClientFactory));
            baseUrl = "http://www.fipiran.com/";

            httpClient.BaseAddress = new Uri(baseUrl);

            this.fipiranUnitOfWork = fipiranUnitOfWork ?? throw new ArgumentNullException(nameof(fipiranUnitOfWork));
        }

        public FipiranCrawlerService(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            baseUrl = "http://www.fipiran.com/";

            httpClient.BaseAddress = new Uri(baseUrl);
            LoggerFactory loggerFactory = new LoggerFactory();
            logger = new Logger<FipiranCrawlerService>(loggerFactory);
            fipiranUnitOfWork = new FipiranUnitOfWork(new MongoDbSetting { ConnectionString = "mongodb://localhost:27017/", DataBaseName = "BourceInformation" });
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
            var response = await httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in Get News Of FipIran {url}");
                Console.WriteLine($"Error in Get News Of FipIran {url}");
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
            var response = await httpClient.GetAsync("Codal/Invitation", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in Get News Of FipIran Codal/Invitation");
                Console.WriteLine($"Error in Get News Of FipIran Codal/Invitation");
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
    }
}