using DemoTienda.Application.Services;
using DemoTienda.Domain.Entites;
using DemoTiendaAPIController.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DemoTienda.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ProductoService _service;
        private readonly ProductoSettings _cfg;

        public ProductoController(ProductoService service, IOptionsSnapshot<ProductoSettings> cfg)
        {
            _service = service;
            _cfg = cfg.Value;
        }

        // GET: api/Productos
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var items = await _service.ListAsync();
            return Ok(items);
        }

        // GET: api/Productos/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        // POST: api/Productos
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Producto request)
        {
            var created = await _service.AddAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/Productos/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] Producto request)
        {
            await _service.UpdateAsync(id, request);
            return NoContent();
        }

        // DELETE: api/Productos/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/Producto/config
        [HttpGet("config")]
        public IActionResult Config() => Ok(_cfg);

    }
}
