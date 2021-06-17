﻿using Bource.Common.Models;
using Bource.Common.Utilities;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Common;
using Bource.Models.Data.Enums;
using Bource.Models.Data.Tsetmc;
using Bource.Services.Crawlers.Tsetmc.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private readonly HttpClient httpClient;
        private readonly ILogger<TseCrawlerService> logger;
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;


        #endregion

        #region Constructors
        public TseCrawlerService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, ITsetmcUnitOfWork tsetmcUnitOfWork, IDistributedCache distributedCache)
        {
            logger = loggerFactory?.CreateLogger<TseCrawlerService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            httpClient = httpClientFactory?.CreateClient(nameof(TseCrawlerService)) ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
        }

        #endregion

        //public async Task Update
    }
}