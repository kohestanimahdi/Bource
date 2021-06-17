using System.Collections.Generic;
using System.Linq;

namespace Bource.Common.Models
{
    public class ApplicationSetting
    {
        public MongoDbSetting mongoDbSetting { get; set; }
        public List<CrawlerSetting> CrawlerSettings { get; set; }

        public CrawlerSetting GetCrawlerSetting(string key)
            => CrawlerSettings?.FirstOrDefault(i => i.Key == key);
    }

    public class MongoDbSetting
    {
        public string ConnectionString { get; set; }
        public string DataBaseName { get; set; }
    }

    public class CrawlerSetting
    {
        public string Key { get; set; }
        public string Url { get; set; }
        public int Timeout { get; set; }
    }
}