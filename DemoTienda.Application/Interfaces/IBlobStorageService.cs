using DemoTienda.Application.DTOs.Response;

namespace DemoTienda.Application.Interfaces
{
    public interface IBlobStorageService
    {
        Task<ArchivoBlobResponseDTO> UploadAsync(
            string fileName,
            Stream content,
            string contentType,
            CancellationToken cancellationToken = default);

        Task<(Stream Content, string ContentType)> DownloadAsync(
            string fileName,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string fileName,
            CancellationToken cancellationToken = default);
    }
}
