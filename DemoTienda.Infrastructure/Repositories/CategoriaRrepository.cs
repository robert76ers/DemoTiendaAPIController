using DemoTienda.Application.Interfaces;
using DemoTienda.Domain.Entites;
using DemoTienda.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DemoTienda.Infrastructure.Repositories
{
    public sealed class CategoriaRepository : ICategoriaRepository
    {
        private readonly DemoTiendaContext _db;
        public CategoriaRepository(DemoTiendaContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Categoria>> ListAsync(CancellationToken cancellationToken = default) =>
            await _db.Categorias.AsNoTracking().ToListAsync(cancellationToken);

        public Task<Categoria?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
            _db.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public async Task<Categoria> AddAsync(Categoria entity, CancellationToken cancellationToken = default)
        {
            _db.Categorias.Add(entity);
            await _db.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task UpdateAsync(int id, Categoria entity, CancellationToken cancellationToken = default)
        {
            var existing = await _db.Categorias.FindAsync(id, cancellationToken);
            if (existing is null) return;

            existing.Nombre = entity.Nombre;
            existing.Descripcion = entity.Descripcion;
            existing.EsActiva = entity.EsActiva;

            _db.Categorias.Update(existing);
            await _db.SaveChangesAsync(cancellationToken);
        }


        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _db.Categorias.FindAsync(id, cancellationToken);
            if (entity is null) return;
            _db.Categorias.Remove(entity);
            await _db.SaveChangesAsync(cancellationToken);
        }

    }

}
