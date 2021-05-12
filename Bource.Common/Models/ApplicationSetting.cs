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