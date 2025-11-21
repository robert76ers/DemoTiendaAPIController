using System.ComponentModel.DataAnnotations;

namespace DemoTienda.Application.DTOs.Request
{
    public class UpdateCategoriaRequestDTO
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
    }
}
