using Bource.Models.Data.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Services.Crawlers.FipIran
{
    public interface IFipiranCrawlerService
    {
        Task GetAssociations(CancellationToken cancellationToken = default);

        Task GetNews(FipIranNewsTypes type, CancellationToken cancellationToken = default);
    }
}