using DemoTienda.Application.DTOs.Request;
using DemoTienda.Application.DTOs.Response;
using DemoTienda.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoTienda.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CatalogRead")]
    public class CategoriasController : ControllerBase
    {
        private readonly CategoriaService _service;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(CategoriaService service, ILogger<CategoriasController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene la lista de categorías.
        /// </summary>
        /// <response code="200">Lista de categorías devuelta correctamente.</response>
        /// <response code="401">El usuario no está autenticado.</response>
        /// <response code="403">El usuario no tiene permisos para leer categorías.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoriaResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<CategoriaResponseDTO>>> Get()
        {
            _logger.LogInformation("GET /api/categorias");

            var response = await _service.ListAsync();

            return Ok(response);
        }

        /// <summary>
        /// Obtiene una categoría por identificador.
        /// </summary>
        /// <param name="id">Identificador de la categoría.</param>
        /// <response code="200">Categoría encontrada.</response>
        /// <response code="404">Categoría no encontrada.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoriaResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoriaResponseDTO>> GetById(int id)
        {
            var response = await _service.GetAsync(id);

            if (response is null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Categoría no encontrada",
                    Detail = $"No existe una categoría con Id {id}.",
                    Instance = HttpContext.Request.Path
                });
            }

            return Ok(response);
        }

        /// <summary>
        /// Crea una nueva categoría.
        /// </summary>
        /// <param name="request">Datos de la categoría a crear.</param>
        /// <response code="201">Categoría creada correctamente.</response>
        /// <response code="400">Datos de entrada inválidos.</response>
        /// <response code="401">Usuario no autenticado.</response>
        /// <response code="403">Usuario sin permisos de escritura.</response>
        [HttpPost]
        [Authorize(Policy = "CatalogWrite")]
        [ProducesResponseType(typeof(CategoriaResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CategoriaResponseDTO>> Post([FromBody] CreateCategoriaRequestDTO request)
        {
            var created = await _service.AddAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza una categoría existente.
        /// </summary>
        /// <param name="id">Identificador de la categoría.</param>
        /// <param name="request">Datos a actualizar.</param>
        /// <response code="204">Categoría actualizada correctamente.</response>
        /// <response code="400">Datos inválidos.</response>
        /// <response code="404">Categoría no encontrada.</response>
        [HttpPut("{id:int}")]
        [Authorize(Policy = "CatalogWrite")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateCategoriaRequestDTO request)
        {
            var updated = await _service.UpdateAsync(id, request);

            if (!updated)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Categoría no encontrada",
                    Detail = $"No existe una categoría con Id {id}.",
                    Instance = HttpContext.Request.Path
                });
            }

            return NoContent();
        }

        /// <summary>
        /// Elimina una categoría.
        /// </summary>
        /// <param name="id">Identificador de la categoría.</param>
        /// <response code="204">Categoría eliminada correctamente.</response>
        /// <response code="404">Categoría no encontrada.</response>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "CatalogWrite")]
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
                    Title = "Categoría no encontrada",
                    Detail = $"No existe una categoría con Id {id}.",
                    Instance = HttpContext.Request.Path
                });
            }

            return NoContent();
        }
    }
}
