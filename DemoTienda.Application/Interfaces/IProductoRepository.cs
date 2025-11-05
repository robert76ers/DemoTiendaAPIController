using DemoTienda.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTienda.Application.Interfaces
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> ListAsync();
        Task<Producto?> GetByIdAsync(int id);
        Task<Producto> AddAsync(Producto entity);
        Task UpdateAsync(int id, Producto entity);
        Task DeleteAsync(int id);
    }
}
