using Bource.Common.Models;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Bource.Services.Security
{
    public class OneTimeToken : IOneTimeToken, IScopedDependency
    {
        private readonly string Secret;
        private readonly TimeSpan GenerationPeriod;
        private readonly TimeSpan ExpirationPeriod;
        private readonly int TokenLength;
        private readonly SHA1Managed sha1;

        public OneTimeToken(IOptionsSnapshot<ApplicationSetting> settings)
        {
            Secret = settings.Value.TokenSettings.Secret;
            GenerationPeriod = TimeSpan.FromMinutes(settings.Value.TokenSettings.GenerationPeriod);
            ExpirationPeriod = TimeSpan.FromMinutes(settings.Value.TokenSettings.ExpirationPeriod);
            TokenLength = settings.Value.TokenSettings.TokenLength;
            sha1 = new SHA1Managed();
        }

        public string GenerateToken(params string[] identifiers)
        {
            return _generateToken(identifiers, DateTime.Now.Ticks / GenerationPeriod.Ticks);
        }

        private string _generateToken(string[] identifiers, long time)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(Secret);
            stringBuilder.Append(",").Append(time);
            foreach (var identifier in identifiers)
            {
                stringBuilder.Append(",").Append(identifier);
            }
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
            int result = BitConverter.ToInt32(hash, 0);
            int truncatedResult = Math.Abs(result % (int)(Math.Pow(10, TokenLength) - Math.Pow(10, TokenLength - 1)) + (int)Math.Pow(10, TokenLength - 1));
            return truncatedResult.ToString();
        }

        public bool ValidateToken(string token, params string[] identifiers)
        {
            var currentTime = DateTime.Now.Ticks / GenerationPeriod.Ticks;
            var startTime = DateTime.Now.Subtract(ExpirationPeriod).Ticks / GenerationPeriod.Ticks;

            for (var time = currentTime; time >= startTime; time--)
            {
                if (_generateToken(identifiers, time) == token)
                    return true;
            }

            return false;
        }
    }
}
