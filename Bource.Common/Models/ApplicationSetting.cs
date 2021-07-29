using System.Collections.Generic;
using System.Linq;

namespace Bource.Common.Models
{
    public class ApplicationSetting
    {
        public MongoDbSetting mongoDbSetting { get; set; }
        public List<CrawlerSetting> CrawlerSettings { get; set; }
        public JwtSetting JwtSettings { get; set; }
        public TokenSetting TokenSettings { get; set; }
        public IdentitySetting IdentitySettings { get; set; }





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

    public class JwtSetting
    {
        public string SecretKey { get; set; }
        public string Encryptkey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int NotBeforeMinutes { get; set; }
        public int ExpirationDate { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public bool SaveToken { get; set; }
        public bool RequireSignedTokens { get; set; }
        public bool RequireExpirationTime { get; set; }
        public bool ValidateLifetime { get; set; }
        public bool ValidateAudience { get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
    }

    public class TokenSetting
    {
        public string Secret { get; set; }
        public int GenerationPeriod { get; set; }
        public int ExpirationPeriod { get; set; }
        public int TokenLength { get; set; }
    }

    public class IdentitySetting
    {
        public bool PasswordRequireDigit { get; set; }
        public int PasswordRequiredLength { get; set; }
        public bool PasswordRequireNonAlphanumic { get; set; }
        public bool PasswordRequireUppercase { get; set; }
        public bool PasswordRequireLowercase { get; set; }
        public bool RequireUniqueEmail { get; set; }
    }
}