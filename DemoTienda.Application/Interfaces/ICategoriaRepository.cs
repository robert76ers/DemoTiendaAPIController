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
        Task<IEnumerable<Categoria>> ListAsync(CancellationToken cancellationToken = default);
        Task<Categoria?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Categoria> AddAsync(Categoria entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, Categoria entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
