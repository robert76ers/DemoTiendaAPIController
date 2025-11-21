using DemoTienda.Application.DTOs.Request;
using DemoTienda.Application.DTOs.Response;
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

        public async Task<IEnumerable<CategoriaResponseDTO>> ListAsync() 
        {
            _logger.LogInformation("Listando categorías");

            var items = await _repository.ListAsync();

            return items.Select(x => new CategoriaResponseDTO
            {
                Id = x.Id,
                Nombre = x.Nombre
            });
        } 

        public async Task<CategoriaResponseDTO?> GetAsync(int id)
        {
            var entity =  await _repository.GetByIdAsync(id);

            if (entity is null)
            {
                return null;
            }

            return new CategoriaResponseDTO
            {
                Id = entity.Id,
                Nombre = entity.Nombre
            };
        }

        public async Task<CategoriaResponseDTO> AddAsync(CreateCategoriaRequestDTO request)
        {
            var entity = new Categoria
            {
                Nombre = request.Nombre
            };

            var created = await _repository.AddAsync(entity);

            return new CategoriaResponseDTO
            {
                Id = created.Id,
                Nombre = created.Nombre
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoriaRequestDTO request)
        {
            var existing = await _repository.GetByIdAsync(id);

            if (existing is null)
            {
                return false;
            }

            await _repository.UpdateAsync(id, existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);

            if (existing is null)
            {
                return false;
            }

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}

