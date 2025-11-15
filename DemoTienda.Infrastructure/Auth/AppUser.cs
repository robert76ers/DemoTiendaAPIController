using Microsoft.AspNetCore.Identity;

namespace DemoTienda.Infrastructure.Auth
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
