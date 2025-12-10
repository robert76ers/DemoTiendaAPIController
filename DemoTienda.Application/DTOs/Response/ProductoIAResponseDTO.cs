namespace DemoTienda.Application.DTOs.Response
{
    public class ProductoIAResponseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string DescripcionIA { get; set; } = string.Empty;
    }
}
