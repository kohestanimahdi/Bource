using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.Ifb
{
    public interface IIfbCrawlerService
    {
        Task GetPapersAsync(CancellationToken cancellationToken = default);
    }
}