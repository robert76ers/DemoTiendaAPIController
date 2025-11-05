using DemoTienda.Application.Interfaces;
using DemoTienda.Domain.Entites;

namespace DemoTienda.Application.Services
{
    public class CategoriaService
    {
        private readonly ICategoriaRepository _repository;

        public CategoriaService(ICategoriaRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Categoria>> ListAsync() => _repository.ListAsync();
        public Task<Categoria?> GetAsync(int id) => _repository.GetByIdAsync(id);
        public Task<Categoria> AddAsync(Categoria c) => _repository.AddAsync(c);
        public Task UpdateAsync(int id, Categoria c) => _repository.UpdateAsync(id, c);
        public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
    }
}

