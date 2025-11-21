using System.ComponentModel.DataAnnotations;

namespace DemoTienda.Application.DTOs.Request
{
    public class CreateCategoriaRequestDTO
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
    }
}
