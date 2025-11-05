using System.ComponentModel.DataAnnotations;

namespace DemoTiendaAPIController.Settings
{
    public class ProductoSettings
    {
        [Required, MinLength(2)]
        public string DefaultCurrency { get; init; } = "USD";

        [Range(1, 500)]
        public int MaxResults { get; init; } = 3;

    }
}
