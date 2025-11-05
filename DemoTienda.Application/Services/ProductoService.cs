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

        public Task<IEnumerable<Producto>> ListAsync() => _repository.ListAsync();
        public Task<Producto> GetAsync(int id) => _repository.GetByIdAsync(id);
        public Task<Producto> AddAsync(Producto producto) => _repository.AddAsync(producto);
        public Task UpdateAsync(int id, Producto producto) => _repository.UpdateAsync(id, producto);
        public Task DeleteAsync(int id) => _repository.DeleteAsync(id);

    }
}
