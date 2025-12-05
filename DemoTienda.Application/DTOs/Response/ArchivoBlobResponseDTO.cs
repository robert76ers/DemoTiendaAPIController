namespace DemoTienda.Application.DTOs.Response
{
    public class ArchivoBlobResponseDTO
    {
        public string FileName { get; set; } = default!;
        public string Url { get; set; } = default!;
        public long Size { get; set; }
        public string ContentType { get; set; } = default!;
    }
}
