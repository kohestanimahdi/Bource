using Bource.Data.Informations.UnitOfWorks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Ifb
{
    public class IfbCrawlerService
    {
        #region Properties

        protected readonly int numberOfTries = 5;
        protected readonly TimeSpan delayBetweenTimeouts = TimeSpan.FromSeconds(1);
        protected readonly bool throwExceptions = false;
        protected readonly HttpClient httpClient;
        protected readonly ILogger<IfbCrawlerService> logger;
        protected readonly ITsetmcUnitOfWork tsetmcUnitOfWork;

        #endregion Properties

        #region Constructors

        public IfbCrawlerService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, ITsetmcUnitOfWork tsetmcUnitOfWork, IDistributedCache distributedCache)
        {
            logger = loggerFactory?.CreateLogger<IfbCrawlerService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            httpClient = httpClientFactory?.CreateClient(nameof(IfbCrawlerService)) ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
        }

        #endregion Constructors

        //public async Task Get
    }
}
