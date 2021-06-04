using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.AsanBource
{
    public interface IAsanBourceCrawlerService
    {
        Task DownloadSymbolsImageAsync(CancellationToken cancellationToken = default);
    }
}