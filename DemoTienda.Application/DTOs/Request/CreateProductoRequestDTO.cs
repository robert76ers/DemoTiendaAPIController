using System.ComponentModel.DataAnnotations;

namespace DemoTienda.Application.DTOs.Request
{
    public class CreateProductoRequestDTO
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [Range(0.01, double.MaxValue)]
        public decimal Precio { get; set; }
        [Range(0, int.MaxValue)]
        public int IdCategoria { get; set; }
    }
}
