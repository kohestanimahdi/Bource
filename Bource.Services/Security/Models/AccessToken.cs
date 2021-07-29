using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Bource.Services.Security.Models
{
    public class AccessToken
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }
        //public string refresh_token { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        public AccessToken(JwtSecurityToken securityToken)
        {
            Token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            TokenType = "Bearer";
            ExpiresIn = (int)(securityToken.ValidTo - DateTime.UtcNow).TotalSeconds;
        }
    }
}
