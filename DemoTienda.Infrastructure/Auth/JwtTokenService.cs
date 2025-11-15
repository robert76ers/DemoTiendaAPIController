using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DemoTienda.Application.Interfaces;
using DemoTienda.Application.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace DemoTienda.Infrastructure.Auth
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _settings;

        public JwtTokenService(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }

        public string CreateToken(IEnumerable<Claim> claims)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_settings.Key);
            var securityKey = new SymmetricSecurityKey(keyBytes);

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
