using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Tse
{
    public interface ITseCrawlerService
    {
        Task GetPapersAsync(CancellationToken cancellationToken = default);
    }
}