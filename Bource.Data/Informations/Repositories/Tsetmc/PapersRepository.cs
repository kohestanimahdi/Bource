using Bource.Common.Models;
using Bource.Models.Data.Tsetmc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bource.Data.Informations.Repositories.Tsetmc
{
    public class PapersRepository : MongoRepository<Papers>
    {
        public PapersRepository(MongoDbSetting dbSetting) : base(dbSetting)
        {
        }
    }
}
