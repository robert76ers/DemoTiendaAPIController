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

        public async Task<IEnumerable<Categoria>> ListAsync() =>
            await _db.Categorias.AsNoTracking().ToListAsync();

        public Task<Categoria?> GetByIdAsync(int id) =>
            _db.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Categoria> AddAsync(Categoria entity)
        {
            _db.Categorias.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(int id, Categoria entity)
        {
            var existing = await _db.Categorias.FindAsync(id);
            if (existing is null) return;

            existing.Nombre = entity.Nombre;
            existing.Descripcion = entity.Descripcion;
            existing.EsActiva = entity.EsActiva;

            _db.Categorias.Update(existing);
            await _db.SaveChangesAsync();
        }


        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Categorias.FindAsync(id);
            if (entity is null) return;
            _db.Categorias.Remove(entity);
            await _db.SaveChangesAsync();
        }

    }

}
