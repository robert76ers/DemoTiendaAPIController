using DemoTienda.Application.DTOs.Request;
using DemoTienda.Application.DTOs.Response;
using DemoTienda.Application.Services;
using DemoTiendaAPIController.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;

namespace DemoTienda.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ProductoService _service;
        private readonly ProductoSettings _cfg;
        private readonly IDescripcionProductoIAService _descripcionProductoIAService;

        public ProductoController(
            ProductoService service,
            IOptionsSnapshot<ProductoSettings> cfg,
            IDescripcionProductoIAService descripcionProductoIAService)
        {
            _service = service;
            _cfg = cfg.Value;
            _descripcionProductoIAService = descripcionProductoIAService;
        }

        /// <summary>
        /// Obtiene la lista de productos.
        /// </summary>
        /// <returns>Lista de productos.</returns>
        /// <response code="200">Lista de productos devuelta correctamente.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductoResponseDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductoResponseDTO>>> Get()
        {
            var response = await _service.ListAsync();
            return Ok(response);
        }

        /// <summary>
        /// Obtiene un producto por su identificador.
        /// </summary>
        /// <param name="id">Identificador del producto.</param>
        /// <response code="200">Producto encontrado.</response>
        /// <response code="404">No existe un producto con el Id especificado.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductoResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductoResponseDTO>> GetById(int id)
        {
            var response = await _service.GetAsync(id);

            if (response is null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Producto no encontrado",
                    Detail = $"No existe un producto con Id {id}.",
                    Instance = HttpContext.Request.Path
                });
            }

            return Ok(response);
        }

        /// <summary>
        /// Crea un nuevo producto.
        /// </summary>
        /// <param name="request">Datos del producto a crear.</param>
        /// <response code="201">Producto creado correctamente.</response>
        /// <response code="400">Datos de entrada inválidos.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProductoResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductoResponseDTO>> Post([FromBody] CreateProductoRequestDTO request)
        {
            var created = await _service.AddAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created); // 201
        }

        /// <summary>
        /// Actualiza un producto existente.
        /// </summary>
        /// <param name="id">Identificador del producto.</param>
        /// <param name="request">Datos a actualizar.</param>
        /// <response code="204">Producto actualizado correctamente.</response>
        /// <response code="400">Datos de entrada inválidos.</response>
        /// <response code="404">Producto no encontrado.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateProductoRequestDTO request)
        {
            var updated = await _service.UpdateAsync(id, request);

            if (!updated)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Producto no encontrado",
                    Detail = $"No existe un producto con Id {id}.",
                    Instance = HttpContext.Request.Path
                });
            }

            return NoContent();
        }

        /// <summary>
        /// Elimina un producto.
        /// </summary>
        /// <param name="id">Identificador del producto.</param>
        /// <response code="204">Producto eliminado correctamente.</response>
        /// <response code="404">Producto no encontrado.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Producto no encontrado",
                    Detail = $"No existe un producto con Id {id}.",
                    Instance = HttpContext.Request.Path
                });
            }

            return NoContent();
        }

        /// <summary>
        /// Obtiene la configuración por defecto de productos.
        /// </summary>
        /// <response code="200">Configuración devuelta correctamente.</response>
        [HttpGet("config")]
        [ProducesResponseType(typeof(ProductoSettings), StatusCodes.Status200OK)]
        public ActionResult<ProductoSettings> Config() => Ok(_cfg);

        /// <summary>
        /// Devuelve Nombre, Precio y una descripción generada por un LLM local (maximo 300 caracteres)
        /// </summary>
        [HttpGet("{id}/descripcionia")]
        [ProducesResponseType(typeof(ProductoIAResponseDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProductoIAResponseDTO>> GetDescripcionIA(int id)
        {
            var producto = await _service.GetAsync(id);

            if (producto is null)
                return NotFound();

            var descripcion = await _descripcionProductoIAService
                .GenerarDescripcionAsync(producto.Nombre, producto.Precio);

            var response = new ProductoIAResponseDTO
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                DescripcionIA = descripcion
            };

            return Ok(response);

        }
    }
}
