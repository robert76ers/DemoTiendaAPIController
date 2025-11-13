using DemoTienda.Application.Interfaces;
using DemoTienda.Domain.Entites;
using DemoTienda.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DemoTienda.Infrastructure.Repositories
{
    public sealed class ProductoRepository : IProductoRepository
    {
        private readonly DemoTiendaContext _db;
        public ProductoRepository(DemoTiendaContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Producto>> ListAsync() =>
            await _db.Productos
                     .AsNoTracking()
                     .ToListAsync();

        public Task<Producto?> GetByIdAsync(int id) =>
            _db.Productos
               .AsNoTracking()
               .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Producto> AddAsync(Producto entity)
        {
            entity.FechaCreacion = DateTime.UtcNow;
            _db.Productos.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(int id, Producto entity)
        {
            var existing = await _db.Productos.FindAsync(id);
            if (existing is null) return;

            existing.Nombre = entity.Nombre;
            existing.Descripcion = entity.Descripcion;
            existing.Precio = entity.Precio;
            existing.IdCategoria = entity.IdCategoria;
            existing.EsActivo = entity.EsActivo;

            _db.Productos.Update(existing);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Productos.FindAsync(id);
            if (entity is null) return;

            _db.Productos.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
