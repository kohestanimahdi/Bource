using Bource.Models.Data.Enums;
using Bource.Models.Data.FipIran;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.UnitOfWorks
{
    public interface IFipiranUnitOfWork
    {
        Task AddIfNotExistAssociationAsync(List<FipIranAssociation> fipIranNews, CancellationToken cancellationToken = default);

        Task AddIfNotExistNewsAsync(FipIranNewsTypes type, List<FipIranNews> fipIranNews, CancellationToken cancellationToken = default);
    }
}