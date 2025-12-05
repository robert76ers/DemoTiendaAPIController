using DemoTienda.Application.Interfaces;
using DemoTienda.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DemoTienda.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArchivosController : ControllerBase
    {
        private readonly IBlobStorageService _blobStorageService;

        public ArchivosController(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        /// <summary>
        /// Sube un archivo al Blob Storage.
        /// </summary>
        [HttpPost("upload")]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Debe seleccionar un archivo válido.");
            }

            using var stream = file.OpenReadStream();

            var result = await _blobStorageService.UploadAsync(
                file.FileName,
                stream,
                file.ContentType,
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Descarga un archivo desde Blob Storage.
        /// </summary>
        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> Download(string fileName, CancellationToken cancellationToken)
        {
            try
            {
                var (content, contentType) = await _blobStorageService.DownloadAsync(fileName, cancellationToken);

                return File(content, contentType, fileName);
            }
            catch (FileNotFoundInBlobException)
            {
                return NotFound($"El archivo '{fileName}' no existe.");
            }
        }

        /// <summary>
        /// Elimina un archivo de Blob Storage.
        /// </summary>
        [HttpDelete("{fileName}")]
        public async Task<IActionResult> Delete(string fileName, CancellationToken cancellationToken)
        {
            await _blobStorageService.DeleteAsync(fileName, cancellationToken);

            return NoContent();
        }

    }
}
