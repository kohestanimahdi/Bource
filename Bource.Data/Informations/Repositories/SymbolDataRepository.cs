using Bource.Common.Models;
using Bource.Models.Data.Tsetmc;

namespace Bource.Data.Informations.Repositories
{
    public class SymbolDataRepository : MongoRepository<SymbolData>
    {
        public SymbolDataRepository(MongoDbSetting dbSetting)
            : base(dbSetting)
        {

        }
    }
}
