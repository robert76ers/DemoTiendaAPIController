using BenchmarkDotNet.Attributes;
using DemoTienda.Domain.Entites;
using DemoTiendaAPIController.Data;
using Microsoft.EntityFrameworkCore;

namespace MejoresPracticasLinq
{
    [MemoryDiagnoser]
    public class IQueryableVsIEnumerableBenchmarks
    {
        private DemoTiendaBenchmarkContext _db = null!;

        [Params(1,2)]
        public int IdCategoria { get; set; }

        private decimal precioMin = 100m;
        private decimal precioMax = 700m;

        [GlobalSetup]
        public void Setup()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DemoTiendaContext>();
            optionsBuilder.UseSqlServer(
                "Server=DESKTOP-383HIFO\\SQLEXPRESS;" +
                "Database=DemoTienda;" +
                "Trusted_Connection=True;" +
                "TrustServerCertificate=True");

            _db = new DemoTiendaBenchmarkContext(optionsBuilder.Options);
        }

        [Benchmark(Description = "EF: Filtrar en BD (IQueryable)")]
        public List<Producto> EF_FiltrarEnBD()
        {
            return _db.Productos
                      .AsNoTracking()
                      .Where(p => p.IdCategoria == IdCategoria
                               && p.Precio >= precioMin
                               && p.Precio <= precioMax)
                      .ToList();
        }

        [Benchmark(Description = "EF: Traer todo y filtrar en memoria (IEnumerable)")]
        public List<Producto> EF_TraerTodoYFiltrarEnMemoria()
        {
            var todos = _db.Productos
                           .AsNoTracking()
                           .ToList();

            return todos
                .Where(p => p.IdCategoria == IdCategoria
                         && p.Precio >= precioMin
                         && p.Precio <= precioMax)
                .ToList();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _db?.Dispose();
        }
    }
}
