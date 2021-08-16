using System.Threading;
using System.Threading.Tasks;

namespace Bource.Portal.Services.CalculateServices
{
    public interface ISymbolCalculator
    {
        Task GetTurnoverAveragesAsync(long insCode, CancellationToken cancellationToken = default);
    }
}