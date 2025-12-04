using DemoTienda.Application.DTOs.Request;
using DemoTienda.Application.DTOs.Response;
using DemoTienda.Application.Interfaces;
using DemoTienda.Domain.Entites;
using DemoTienda.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using MapsterMapper;

namespace DemoTienda.Application.Services
{
    public class CategoriaService
    {
        private readonly ICategoriaRepository _repository;
        private readonly ILogger<CategoriaService> _logger;
        private readonly IMapper _mapper;

        public CategoriaService(ICategoriaRepository repository, ILogger<CategoriaService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoriaResponseDTO>> ListAsync(CancellationToken cancellationToken = default) 
        {
            _logger.LogInformation("Listando categorías");

            var items = await _repository.ListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<CategoriaResponseDTO>>(items);
        } 

        public async Task<CategoriaResponseDTO> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity =  await _repository.GetByIdAsync(id, cancellationToken);

            if (entity is null)
            {
                throw new CategoriaNoEncontradaException(id);
            }

            return _mapper.Map<CategoriaResponseDTO>(entity);
        }

        public async Task<CategoriaResponseDTO> AddAsync(CreateCategoriaRequestDTO request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<Categoria>(request);

            var created = await _repository.AddAsync(entity, cancellationToken);

            return _mapper.Map<CategoriaResponseDTO>(created);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoriaRequestDTO request, CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByIdAsync(id, cancellationToken);

            if (existing is null)
            {
                return false;
            }

            var entityToUpdate = _mapper.Map<Categoria>(request);

            await _repository.UpdateAsync(id, entityToUpdate, cancellationToken);
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

        public async Task<IEnumerable<CategoriaResponseDTO>> AddBulkAsync(
            IEnumerable<CreateCategoriaRequestDTO> requests,
            CancellationToken cancellationToken = default)
        {
            var entities = _mapper.Map<List<Categoria>>(requests);
            await _repository.BulkInsertAsync(entities, cancellationToken);
            return _mapper.Map<IEnumerable<CategoriaResponseDTO>>(entities);
        }
    }
}

