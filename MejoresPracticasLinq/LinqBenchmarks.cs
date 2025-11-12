using BenchmarkDotNet.Attributes;
using DemoTienda.Domain.Entites;
using DemoTiendaAPIController.Data;
using Microsoft.EntityFrameworkCore;

namespace MejoresPracticasLinq
{
    [MemoryDiagnoser]
    public class LinqBenchmarks
    {
        private DemoTiendaBenchmarkContext _db = null!;
        private Producto[] _data = null!;

        private int categoriaFiltro = 6;
        private decimal precioMin = 100m;
        private decimal precioMax = 700m;

        [Params(5000, 50000, 500_000)]
        public int CantidadElementos { get; set; }

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

            // Cargamos una muestra de datos desde la BD a memoria.
            _data = _db.Productos
                       .AsNoTracking()
                       .OrderBy(p => p.Id)
                       .Take(CantidadElementos)
                       .ToArray();
        }

        // 1) Filtrar + seleccionar campos
        [Benchmark(Description = "SIN LINQ: foreach + if + contador")]
        public int SinLinq_FilterSelect()
        {
            var count = 0;
            foreach (var p in _data)
            {
                if (p.EsActivo
                    && p.IdCategoria == categoriaFiltro
                    && p.Precio >= precioMin
                    && p.Precio <= precioMax)
                {
                    count++;
                }
            }
            return count;
        }

        [Benchmark(Description = "CON LINQ: Where + Select + Count")]
        public int ConLinq_FilterSelect()
            => _data
                .Where(p => p.EsActivo
                          && p.IdCategoria == categoriaFiltro
                          && p.Precio >= precioMin
                          && p.Precio <= precioMax)
                .Select(p => new { p.Id, p.Nombre })
                .Count();

        // 2) Agrupación por categoría
        [Benchmark(Description = "SIN LINQ: diccionario agrupado")]
        public decimal SinLinq_GroupSum()
        {
            var dict = new Dictionary<int, decimal>();

            foreach (var p in _data)
            {
                if (!p.EsActivo) continue;

                if (!dict.TryGetValue(p.IdCategoria, out var sum))
                    dict[p.IdCategoria] = p.Precio;
                else
                    dict[p.IdCategoria] = sum + p.Precio;
            }

            decimal total = 0;
            foreach (var kv in dict)
                total += kv.Value;

            return total;
        }

        [Benchmark(Description = "CON LINQ: GroupBy + Sum")]
        public decimal ConLinq_GroupSum()
            => _data
                .Where(p => p.EsActivo)
                .GroupBy(p => p.IdCategoria)
                .Select(g => g.Sum(x => x.Precio))
                .Sum();

        // 3) Ordenamiento + top N
        [Benchmark(Description = "SIN LINQ: Array.Sort + for")]
        public decimal SinLinq_SortTopN()
        {
            var copy = (Producto[])_data.Clone();
            Array.Sort(copy, (a, b) => b.Precio.CompareTo(a.Precio));

            decimal acc = 0;
            for (int i = 0; i < 100 && i < copy.Length; i++)
                acc += copy[i].Precio;

            return acc;
        }

        [Benchmark(Description = "CON LINQ: OrderByDescending + Take")]
        public decimal ConLinq_SortTopN()
            => _data
                .OrderByDescending(p => p.Precio)
                .Take(100)
                .Sum(p => p.Precio);

        // 4) Buenas prácticas: múltiples enumeraciones vs materialización única

        [Benchmark(Description = "CON LINQ (mala práctica: múltiples enumeraciones)")]
        public decimal ConLinq_MultipleEnumeration()
        {
            var query = _data.Where(p => p.EsActivo
                                       && p.IdCategoria == categoriaFiltro
                                       && p.Precio >= precioMin
                                       && p.Precio <= precioMax);

            // Se recorre la misma consulta varias veces
            var count = query.Count();
            var sum = query.Sum(p => p.Precio);

            return sum + count;
        }

        [Benchmark(Description = "CON LINQ (buena práctica: materialización única)")]
        public decimal ConLinq_SingleMaterialization()
        {
            var list = _data.Where(p => p.EsActivo
                                      && p.IdCategoria == categoriaFiltro
                                      && p.Precio >= precioMin
                                      && p.Precio <= precioMax).ToList();

            var count = list.Count;
            var sum = list.Sum(p => p.Precio);

            return sum + count;
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _db?.Dispose();
        }
    }
}
