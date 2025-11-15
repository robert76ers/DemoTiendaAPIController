using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTienda.Application.DTOs.Auth
{
    public class AuthDTO
    {
        public record RegisterRequest(
            string Email,
            string Password,
            string? FullName);

        public record LoginRequest(
            string Email,
            string Password);

        public record AuthResponse(
            string AccessToken);
    }
}
