using Bource.Common.Models;
using Bource.Common.Utilities;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Common;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.AsanBource
{
    public class AsanBourceCrawlerService : IAsanBourceCrawlerService, IScopedDependency
    {
        private string baseUrl => httpClient.BaseAddress.ToString();
        private readonly HttpClient httpClient;
        private readonly ILogger<AsanBourceCrawlerService> logger;
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;

        public AsanBourceCrawlerService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, ITsetmcUnitOfWork fipiranUnitOfWork)
        {
            logger = loggerFactory?.CreateLogger<AsanBourceCrawlerService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            httpClient = httpClientFactory?.CreateClient(nameof(AsanBourceCrawlerService)) ?? throw new ArgumentNullException(nameof(httpClientFactory));

            httpClient.BaseAddress = new Uri(baseUrl);

            this.tsetmcUnitOfWork = fipiranUnitOfWork ?? throw new ArgumentNullException(nameof(fipiranUnitOfWork));
        }


        public async Task DownloadSymbolsImageAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Common.Utilities.FileExtensions.CreateIfNotExists("Contents/SymbolLogos");

            var symbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);
            await Common.Utilities.ApplicationHelpers.DoFunctionsWithProgressBar(symbols, DownloadSymbolImageAsync, cancellationToken);
        }

        private async Task DownloadSymbolImageAsync(Symbol symbol, CancellationToken cancellationToken = default(CancellationToken), int numberOfTries = 0)
        {
            if (string.IsNullOrWhiteSpace(symbol.Code12))
                return;

            var response = await httpClient.GetAsync($"content/SymbolsLogo/{symbol.Code12}.png", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error in Get Symbol Images");
                return;
            }
            var fileBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

            FileExtensions.DeleteFileIfExists($"Contents/SymbolLogos/{symbol.Code12}.png");
            await File.WriteAllBytesAsync(FileExtensions.GetDirectory($"Contents/SymbolLogos/{symbol.Code12}.png"), fileBytes);

            symbol.Logo = $"Contents/SymbolLogos/{symbol.Code12}.png";
            await tsetmcUnitOfWork.UpdateSymbolAsync(symbol);
        }
    }
}
