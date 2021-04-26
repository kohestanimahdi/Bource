using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bource.Common.Models
{
    public class ApplicationSetting
    {
        public MongoDbSetting mongoDbSetting { get; set; }
    }

    public class MongoDbSetting
    {
        public string ConnectionString { get; set; }
        public string DataBaseName { get; set; }
    }
}
