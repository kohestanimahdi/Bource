using Bource.Portal.ViewModels.Dtos.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Portal.Services.CalculateServices
{
    public interface ISymbolCalculator
    {
        Task<AveragesResponse> GetTurnoverAveragesAsync(long insCode, CancellationToken cancellationToken = default);
    }
}