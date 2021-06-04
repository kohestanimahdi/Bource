using System.Collections.Generic;

namespace Bource.Common.Models
{
    public class ApplicationSetting
    {
        public MongoDbSetting mongoDbSetting { get; set; }
        public List<CrawlerSetting> CrawlerSettings { get; set; }
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