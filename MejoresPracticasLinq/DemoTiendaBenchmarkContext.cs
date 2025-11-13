using DemoTienda.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace MejoresPracticasLinq
{
    internal class DemoTiendaBenchmarkContext : DemoTiendaContext
    {
        public DemoTiendaBenchmarkContext(DbContextOptions<DemoTiendaContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }
    }
}

