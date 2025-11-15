using System.Security.Claims;

namespace DemoTienda.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string CreateToken(IEnumerable<Claim> claims);
    }
}
