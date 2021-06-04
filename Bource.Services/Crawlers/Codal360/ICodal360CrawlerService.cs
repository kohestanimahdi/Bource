using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Codal360
{
    public interface ICodal360CrawlerService
    {
        Task UpdateSymbolsCodalImageAsync(CancellationToken cancellationToken = default);
        Task UpdateSymbolsCodalURLAsync(CancellationToken cancellationToken = default);
    }
}