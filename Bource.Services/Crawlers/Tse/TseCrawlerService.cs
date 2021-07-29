using Bource.Common.Models;
using Bource.Common.Utilities;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Common;
using Bource.Models.Data.Enums;
using Bource.Services.Crawlers.Tse.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Tse
{
    public class TseCrawlerService : IScopedDependency, ITseCrawlerService
    {
        #region Properties

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
            var symbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);
            await GetPapersAsync(PapersTypes.Cash, symbols, cancellationToken);
            await GetPapersAsync(PapersTypes.Future, symbols, cancellationToken);
            await GetPapersAsync(PapersTypes.Option, symbols, cancellationToken);
            await GetPapersAsync(PapersTypes.Debt, symbols, cancellationToken);
            await GetPapersAsync(PapersTypes.ETF, symbols, cancellationToken);
            await GetPapersAsync(PapersTypes.TradeOption, symbols, cancellationToken);
        }

        private async Task GetPapersAsync(PapersTypes papersTypes, List<Symbol> symbols, CancellationToken cancellationToken = default(CancellationToken))
        {
            using var httpClient = httpClientFactory.CreateClient(className);

            var response = await httpClient.GetAsync($"json/Listing/ListingByName{(int)papersTypes}.json", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in GetPapers {Enum.GetName<PapersTypes>(papersTypes)}");
                return;
            }
            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            var companies = JsonConvert.DeserializeObject<GetPapersResponse>(result);

            var allCompanies = companies.Companies.SelectMany(i => i.Companies).ToList();

            foreach (var company in allCompanies)
            {
                var companySymbols = symbols.Where(i => i.Code12 == company.CompanyCode).ToList();
                if (companySymbols is null || !companySymbols.Any())
                    logger.LogWarning($"Symbol not found in get symbol paper | {company.CompanyCode} | {company.Sign}");
                else
                {
                    companySymbols.ForEach(symbol =>
                    {
                        symbol.PaperTitle = papersTypes.ToDisplay();
                        symbol.PaperType = papersTypes;
                        symbol.Status = company.Status;
                        tsetmcUnitOfWork.UpdateSymbolAsync(symbol).GetAwaiter().GetResult();
                    });
                }
            }
        }
    }
}