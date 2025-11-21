namespace DemoTienda.Application.DTOs.Response
{
    public class ProductoResponseDTO
    {
        public int Id { get; init; }
        public string Nombre { get; init; } = string.Empty;
        public decimal Precio { get; init; }
        public int IdCategoria { get; init; }
    }
}
