using DemoTienda.Application.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DemoTienda.Application.DTOs.Response;
using DemoTienda.Domain.Exceptions;
using DemoTienda.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace DemoTienda.Infrastructure.Storage
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(IOptions<AzureStorageSettings> options)
        {
            var settings = options.Value;

            var blobServiceClient = new BlobServiceClient(settings.ConnectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(settings.ContainerName);
        }

        public async Task<ArchivoBlobResponseDTO> UploadAsync(string fileName, Stream content, string contentType, CancellationToken cancellationToken = default)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);

            var headers = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            await blobClient.UploadAsync(content, new BlobUploadOptions { HttpHeaders = headers }, cancellationToken);

            var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);

            return new ArchivoBlobResponseDTO
            {
                FileName = fileName,
                Url = blobClient.Uri.ToString(),
                Size = properties.Value.ContentLength,
                ContentType = properties.Value.ContentType ?? contentType
            };
        }

        public async Task<(Stream Content, string ContentType)> DownloadAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);

            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                throw new FileNotFoundInBlobException(fileName);
            }

            var response = await blobClient.DownloadAsync(cancellationToken: cancellationToken);

            var memoryStream = new MemoryStream();
            await response.Value.Content.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;

            var contentType = response.Value.Details.ContentType ?? "application/octet-stream";

            return (memoryStream, contentType);
        }

        public async Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
        }
    }
}
