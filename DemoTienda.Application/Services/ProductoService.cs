using DemoTienda.Application.DTOs.Request;
using DemoTienda.Application.DTOs.Response;
using DemoTienda.Application.Interfaces;
using DemoTienda.Domain.Entites;

namespace DemoTienda.Application.Services
{
    public class ProductoService
    {
        private readonly IProductoRepository _repository;

        public ProductoService(IProductoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductoResponseDTO>> ListAsync()
        {
            var items = await _repository.ListAsync();

            return items.Select(p => new ProductoResponseDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                IdCategoria = p.IdCategoria
            });
        }

        public async Task<ProductoResponseDTO?> GetAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                return null;

            return new ProductoResponseDTO
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Precio = entity.Precio,
                IdCategoria = entity.IdCategoria
            };
        }

        public async Task<ProductoResponseDTO> AddAsync(CreateProductoRequestDTO request)
        {
            var entity = new Producto
            {
                Nombre = request.Nombre,
                Precio = request.Precio,
                IdCategoria = request.IdCategoria
            };

            var created = await _repository.AddAsync(entity);

            return new ProductoResponseDTO
            {
                Id = created.Id,
                Nombre = created.Nombre,
                Precio = created.Precio,
                IdCategoria = created.IdCategoria
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateProductoRequestDTO request)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
                return false;

            existing.Nombre = request.Nombre;
            existing.Precio = request.Precio;
            existing.IdCategoria = request.IdCategoria;

            await _repository.UpdateAsync(id, existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
                return false;

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
