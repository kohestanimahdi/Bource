using Bource.Common.Models;
using Bource.Models.Data.Common;

namespace Bource.Data.Informations.Repositories
{
    public class SymbolRepository : MongoRepository<Symbol>
    {
        public SymbolRepository(MongoDbSetting dbSetting)
            : base(dbSetting)
        {

        }
    }
}
