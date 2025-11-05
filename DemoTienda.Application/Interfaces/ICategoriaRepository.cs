using DemoTienda.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTienda.Application.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> ListAsync();
        Task<Categoria?> GetByIdAsync(int id);
        Task<Categoria> AddAsync(Categoria entity);
        Task UpdateAsync(int id, Categoria entity);
        Task DeleteAsync(int id);
    }
}
