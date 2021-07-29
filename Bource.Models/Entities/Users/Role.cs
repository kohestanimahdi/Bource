using Microsoft.AspNetCore.Identity;

namespace Bource.Models.Entities.Users
{
    public class Role : IdentityRole<int>
    {
        public Role(string name) : base(name)
        {

        }
    }
}
