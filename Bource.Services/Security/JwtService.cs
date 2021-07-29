using Bource.Common.Models;
using Bource.Services.Security.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bource.Services.Security
{
    public class JwtService : IJwtService, IScopedDependency
    {
        private readonly ApplicationSetting _ApplicationSettings;

        public JwtService(IOptionsSnapshot<ApplicationSetting> settings)
        {
            this._ApplicationSettings = settings.Value;
        }

        public AccessToken GenerateAsync<TUser>(TUser user, IEnumerable<Claim> claims)
            where TUser : IdentityUser<int>
        {
            var secretKey = Encoding.UTF8.GetBytes(_ApplicationSettings.JwtSettings.SecretKey);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

            var encryptionkey = Encoding.UTF8.GetBytes(_ApplicationSettings.JwtSettings.Encryptkey); //must be 16 character
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _ApplicationSettings.JwtSettings.Issuer,
                Audience = _ApplicationSettings.JwtSettings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(_ApplicationSettings.JwtSettings.NotBeforeMinutes),
                Expires = DateTime.Now.AddDays(_ApplicationSettings.JwtSettings.ExpirationDate),
                SigningCredentials = signingCredentials,
                Subject = new ClaimsIdentity(claims),
                EncryptingCredentials = encryptingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);

            return new AccessToken(securityToken);
        }
    }
}