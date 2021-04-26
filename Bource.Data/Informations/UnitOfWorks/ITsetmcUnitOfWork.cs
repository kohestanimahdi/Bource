using Bource.Models.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Data.Informations.UnitOfWorks
{
    public interface ITsetmcUnitOfWork
    {
        Task AddOrUpdateSymbolGroups(List<SymbolGroup> symbolGroups, CancellationToken cancellationToken = default);
    }
}
