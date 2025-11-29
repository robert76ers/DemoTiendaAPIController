using DemoTienda.Application.DTOs.Request;
using DemoTienda.Application.DTOs.Response;
using DemoTienda.Application.Interfaces;
using DemoTienda.Domain.Entites;
using DemoTienda.Domain.Exceptions;
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

        public async Task<IEnumerable<CategoriaResponseDTO>> ListAsync(CancellationToken cancellationToken = default) 
        {
            _logger.LogInformation("Listando categorías");

            var items = await _repository.ListAsync(cancellationToken);

            return items.Select(x => new CategoriaResponseDTO
            {
                Id = x.Id,
                Nombre = x.Nombre
            });
        } 

        public async Task<CategoriaResponseDTO> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity =  await _repository.GetByIdAsync(id, cancellationToken);

            if (entity is null)
            {
                throw new CategoriaNoEncontradaException(id);
            }

            return new CategoriaResponseDTO
            {
                Id = entity.Id,
                Nombre = entity.Nombre
            };
        }

        public async Task<CategoriaResponseDTO> AddAsync(CreateCategoriaRequestDTO request, CancellationToken cancellationToken = default)
        {
            var entity = new Categoria
            {
                Nombre = request.Nombre
            };

            var created = await _repository.AddAsync(entity, cancellationToken);

            return new CategoriaResponseDTO
            {
                Id = created.Id,
                Nombre = created.Nombre
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoriaRequestDTO request, CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByIdAsync(id, cancellationToken);

            if (existing is null)
            {
                return false;
            }

            await _repository.UpdateAsync(id, existing, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByIdAsync(id, cancellationToken);

            if (existing is null)
            {
                return false;
            }

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}

