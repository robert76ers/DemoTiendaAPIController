using DemoTienda.Application.Interfaces;
using DemoTienda.Domain.Entites;
using Microsoft.Extensions.Logging;

namespace DemoTienda.Application.Services
{
    public class CategoriaService
    {
        private readonly ICategoriaRepository _repository;
        private readonly ILogger<CategoriaService> _logger;

        public CategoriaService(ICategoriaRepository repository, ILogger<CategoriaService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public Task<IEnumerable<Categoria>> ListAsync() 
        {
            _logger.LogInformation("Listando categorías");
            return _repository.ListAsync();
        } 
        public Task<Categoria?> GetAsync(int id) => _repository.GetByIdAsync(id);
        public Task<Categoria> AddAsync(Categoria c) => _repository.AddAsync(c);
        public Task UpdateAsync(int id, Categoria c) => _repository.UpdateAsync(id, c);
        public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
    }
}

