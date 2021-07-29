using Bource.Services.Security.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;

namespace Bource.Services.Security
{
    public interface IJwtService
    {
        AccessToken GenerateAsync<TUser>(TUser user, IEnumerable<Claim> claims) where TUser : IdentityUser<int>;
    }
}