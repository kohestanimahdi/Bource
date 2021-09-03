using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Shakhesban
{
    public interface IShakhesbanCrawlerService
    {
        Task GetSymbolPrioritiesAsync(CancellationToken cancellationToken = default);
    }
}