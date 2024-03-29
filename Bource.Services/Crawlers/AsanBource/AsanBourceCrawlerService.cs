﻿using Bource.Common.Models;
using Bource.Common.Utilities;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Common;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.AsanBource
{
    public class AsanBourceCrawlerService : IAsanBourceCrawlerService, IScopedDependency
    {
        private readonly TimeSpan delayBetweenRequests = TimeSpan.FromSeconds(1);
        private readonly ILogger<AsanBourceCrawlerService> logger;
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;
        private readonly IHttpClientFactory httpClientFactory;
        private string className => nameof(AsanBourceCrawlerService);

        public AsanBourceCrawlerService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, ITsetmcUnitOfWork fipiranUnitOfWork)
        {
            logger = loggerFactory?.CreateLogger<AsanBourceCrawlerService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.tsetmcUnitOfWork = fipiranUnitOfWork ?? throw new ArgumentNullException(nameof(fipiranUnitOfWork));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task DownloadSymbolsImageAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            FileExtensions.CreateIfNotExists("Contents/SymbolLogos");

            var symbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);
            var httpClient = httpClientFactory.CreateClient(className);
            foreach (var symbol in symbols)
                await DownloadSymbolImageAsync(httpClient, symbol, cancellationToken);
        }

        private async Task DownloadSymbolImageAsync(HttpClient httpClient, Symbol symbol, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (string.IsNullOrWhiteSpace(symbol.Code12))
                    return;

                var response = await httpClient.GetAsync($"content/SymbolsLogo/{symbol.Code12}.png", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                        logger.LogWarning("Error in Get Symbol Images");

                    return;
                }
                var fileBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

                FileExtensions.DeleteFileIfExists($"Contents/SymbolLogos/{symbol.Code12}.png");
                await File.WriteAllBytesAsync(FileExtensions.GetDirectory($"Contents/SymbolLogos/{symbol.Code12}.png"), fileBytes, cancellationToken);

                if (string.IsNullOrWhiteSpace(symbol.Logo))
                {
                    symbol.Logo = $"Contents/SymbolLogos/{symbol.Code12}.png";
                    await tsetmcUnitOfWork.UpdateSymbolAsync(symbol, cancellationToken);
                }
                await Task.Delay(delayBetweenRequests, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Get Symbol Images");
            }
        }
    }
}