using Bource.Common.Models;
using Bource.Models.Data.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories
{
    public class SymbolGroupRepository : MongoRepository<SymbolGroup>
    {
        public SymbolGroupRepository(MongoDbSetting dbSetting)
            : base(dbSetting)
        {

        }
    }
}
