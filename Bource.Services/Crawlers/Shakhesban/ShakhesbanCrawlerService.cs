using Bource.Common.Models;
using Bource.Common.Utilities;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Common;
using Bource.Models.Data.Tsetmc;
using Bource.Services.Crawlers.Codal360.Models;
using Bource.Services.Crawlers.Shakhesban.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Shakhesban
{
    public class ShakhesbanCrawlerService : IShakhesbanCrawlerService, IScopedDependency
    {
        private string baseUrl => setting.Url;
        private string className => nameof(ShakhesbanCrawlerService);
        private readonly CrawlerSetting setting;
        private readonly ILogger<ShakhesbanCrawlerService> logger;
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;
        private readonly IHttpClientFactory httpClientFactory;

        public ShakhesbanCrawlerService(IOptionsSnapshot<ApplicationSetting> settings, IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory,
            ITsetmcUnitOfWork tsetmcUnitOfWork)
        {
            logger = loggerFactory?.CreateLogger<ShakhesbanCrawlerService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
            this.setting = settings.Value.GetCrawlerSetting(className) ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task GetSymbolPrioritiesAsync(CancellationToken cancellationToken = default)
        {
            using var httpClient = httpClientFactory.CreateClient(className);

            var response = await httpClient.GetAsync("stocks/list-data?limit=10000000&page=1&order_col=info.last_date&order_dir=desc&market=warrant", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in Symbol Priorities");
                return;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            var modelResponse = JsonConvert.DeserializeObject<SymbolPriorityResponse>(json);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(modelResponse.Body);

            var symbolPriorities = new List<SymbolPriority>();

            var symbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);

            var rows = htmlDoc.DocumentNode.SelectNodes("tr");

            if (rows is not null && rows.Any())
            {
                foreach (var row in rows)
                {
                    var symbolName = row.SelectSingleNode("td[@data-col='info.symbol']").GetText();
                    var symbol = symbols.FirstOrDefault(i => StringHelper.ComparePersion(i.Sign, symbolName));
                    if (symbol is null)
                        continue;

                    var priority = new SymbolPriority
                    {
                        CapitalIncreaseTime = row.SelectSingleNode("td[@data-col='etc.opts.tarikh_afzayesh_sarmaye']").GetText().Replace("\n", "").GetAsDateTime(),
                        CapitalIncreasePercent = row.SelectSingleNode("td[@data-col='etc.opts.darsad_afzayesh_sarmaye']").GetText().Replace("\n", "").Replace("%", "").ConvertToDecimal(),
                        EndOfUnderwriting = row.SelectSingleNode("td[@data-col='etc.opts.payan_pazirenevisi']").GetText().Replace("\n", "").GetAsDateTime(),
                        Symbol = symbolName,
                        CreateDate = DateTime.Now,
                        InsCode = symbol.InsCode,
                        InsCodeValue = symbol.InsCodeValue
                    };

                    symbolPriorities.Add(priority);
                }
            }
            if (symbolPriorities.Any())
                await tsetmcUnitOfWork.AddIfNotExistsSymbolPriorityAsync(symbolPriorities, cancellationToken);

        }


        public async Task GetSymbolPrioritiesAsync2(CancellationToken cancellationToken = default)
        {
            using var httpClient = httpClientFactory.CreateClient(className);

            var response = await httpClient.GetAsync("markets/warrant", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in Symbol Priorities");
                return;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var symbolPriorities = new List<SymbolPriority>();

            var symbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);

            var table = htmlDoc.GetElementbyId("table-list");
            var ro = table.SelectNodes("tr");
            var rows = table.SelectNodes("//tr");
            if (rows is not null && rows.Any())
            {
                foreach (var row in rows)
                {
                    var symbolName = row.SelectSingleNode("//td[@data-col='info.symbol']").GetText();
                    var symbol = symbols.FirstOrDefault(i => StringHelper.ComparePersion(i.Sign, symbolName));
                    if (symbol is null)
                        continue;

                    var priority = new SymbolPriority
                    {
                        CapitalIncreaseTime = row.SelectSingleNode("//td[@data-col='etc.opts.tarikh_afzayesh_sarmaye']").GetText().Replace("\n", "").GetAsDateTime(),
                        CapitalIncreasePercent = row.SelectSingleNode("//td[@data-col='etc.opts.darsad_afzayesh_sarmaye']").GetText().Replace("\n", "").Replace("%", "").ConvertToDecimal(),
                        EndOfUnderwriting = row.SelectSingleNode("//td[@data-col='etc.opts.payan_pazirenevisi']").GetText().Replace("\n", "").GetAsDateTime(),
                        Symbol = symbolName,
                        CreateDate = DateTime.Now,
                        InsCode = symbol.InsCode,
                        InsCodeValue = symbol.InsCodeValue
                    };

                    symbolPriorities.Add(priority);
                }
            }

            if (symbolPriorities.Any())
                await tsetmcUnitOfWork.AddIfNotExistsSymbolPriorityAsync(symbolPriorities, cancellationToken);

        }


    }
}
