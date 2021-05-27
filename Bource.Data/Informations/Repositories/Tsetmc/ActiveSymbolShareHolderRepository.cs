using Bource.Common.Models;
using Bource.Models.Data.Tsetmc;

namespace Bource.Data.Informations.Repositories.Tsetmc
{
    public class ActiveSymbolShareHolderRepository : MongoRepository<ActiveSymbolShareHolder>
    {
        public ActiveSymbolShareHolderRepository(MongoDbSetting dbSetting) : base(dbSetting)
        {
        }
    }
}