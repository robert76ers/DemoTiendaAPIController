using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTienda.Application.Settings
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public string Key { get; set; } = default!;
        public int ExpirationMinutes { get; set; } = 60;
    }
}
