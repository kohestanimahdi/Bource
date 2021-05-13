using Bource.Common.Models;
using Bource.Models.Data.Tsetmc;

namespace Bource.Data.Informations.Repositories.Tsetmc
{
    public class SelectedIndicatorRepository : MongoRepository<SelectedIndicator>
    {
        public SelectedIndicatorRepository(MongoDbSetting dbSetting) : base(dbSetting)
        {
        }
    }
}
