using BenchmarkDotNet.Attributes;
using DemoTienda.Domain.Entites;
using DemoTiendaAPIController.Data;
using Microsoft.EntityFrameworkCore;

namespace MejoresPracticasLinq
{
    [MemoryDiagnoser]
    public class LinqBenchmarksRecomendados
    {
        private DemoTiendaBenchmarkContext _db = null!;
        private Producto[] _data = null!;
        private List<int> _ids = null!;

        // Escenario consistente con el resto de prácticas
        private int categoriaFiltro = 6;
        private decimal precioMin = 100m;
        private decimal precioMax = 700m;

        [Params(25000, 100_000, 500_000)]
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

            _data = _db.Productos
                       .AsNoTracking()
                       .OrderBy(p => p.Id)
                       .Take(CantidadElementos)
                       .ToArray();

            _ids = _data.Select(p => p.Id).ToList();
        }

        // 1) Any vs Count > 0
        [Benchmark(Description = "EQUIVOCADO: Count(...) > 0")]
        public bool Wrong_CountGreaterThanZero()
            => _data.Count(p =>
                   p.EsActivo &&
                   p.IdCategoria == categoriaFiltro &&
                   p.Precio >= precioMin &&
                   p.Precio <= precioMax) > 0;

        [Benchmark(Description = "CORRECTO: Any(...)")]
        public bool Right_Any()
            => _data.Any(p =>
                   p.EsActivo &&
                   p.IdCategoria == categoriaFiltro &&
                   p.Precio >= precioMin &&
                   p.Precio <= precioMax);

        // 2) First vs FirstOrDefault cuando puede no existir
        [Benchmark(Description = "EQUIVOCADO: First() (lanza si no hay)")]
        public int? Wrong_First_MayThrow()
        {
            try
            {
                return _data.First(p =>
                    p.IdCategoria == categoriaFiltro &&
                    p.Precio > 10_000m).Id;
            }
            catch
            {
                return null;
            }
        }

        [Benchmark(Description = "CORRECTO: FirstOrDefault() + null check")]
        public int? Right_FirstOrDefault_Safe()
            => _data
                .FirstOrDefault(p =>
                    p.IdCategoria == categoriaFiltro &&
                    p.Precio > 10_000m)
                ?.Id;

        // 3) Single vs verificación explícita
        [Benchmark(Description = "EQUIVOCADO: Single() en datos potencialmente duplicados")]
        public int Wrong_Single_MayThrow()
        {
            try
            {
                return _data.Single(p => p.IdCategoria == 1).Id;
            }
            catch
            {
                return -1;
            }
        }

        [Benchmark(Description = "CORRECTO: Where(...).Take(2) + verificación")]
        public int Right_Single_Check()
        {
            var candidates = _data
                .Where(p => p.IdCategoria == 1)
                .Take(1)
                .ToArray();

            return candidates.Length == 1 ? candidates[0].Id : -1;
        }

        // 4) Select vs SelectMany (para colecciones anidadas)
        private List<List<int>> _nested = null!;

        [IterationSetup(Targets = new[] { nameof(Wrong_Select_FlattensPoorly), nameof(Right_SelectMany_Flatten) })]
        public void SetupNested()
        {
            var r = new Random(1);
            _nested = Enumerable.Range(0, 5000)
                .Select(_ => Enumerable.Range(0, r.Next(10, 40)).ToList())
                .ToList();
        }

        [Benchmark(Description = "EQUIVOCADO: Select + flatten manual")]
        public int Wrong_Select_FlattensPoorly()
        {
            var list = new List<int>();
            foreach (var inner in _nested.Select(x => x))
                foreach (var n in inner)
                    list.Add(n);

            return list.Count;
        }

        [Benchmark(Description = "CORRECTO: SelectMany")]
        public int Right_SelectMany_Flatten()
            => _nested.SelectMany(x => x).Count();

        // 5) Contains vs Any con igualdad personalizada
        [Benchmark(Description = "EQUIVOCADO: Contains con objetos complejos")]
        public bool Wrong_Contains_Complex()
        {
            var target = new Producto
            {
                Id = 999999,
                Nombre = "X",
                Descripcion = "Dummy",
                Precio = 1,
                IdCategoria = 1,
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            };

            // Referencia distinta → Contains casi siempre false
            return _data.Contains(target);
        }

        [Benchmark(Description = "CORRECTO: Any con predicado por Id")]
        public bool Right_Any_Predicate()
            => _data.Any(p => p.Id == 999999);

        // 6) OrderBy antes de Where vs filtrar primero
        [Benchmark(Description = "EQUIVOCADO: OrderBy → Where")]
        public int Wrong_OrderBeforeWhere()
            => _data
                .OrderBy(p => p.Precio) // ordena TODO el conjunto
                .Where(p =>
                    p.EsActivo &&
                    p.IdCategoria == categoriaFiltro &&
                    p.Precio > precioMin)
                .Count();

        [Benchmark(Description = "CORRECTO: Where → OrderBy")]
        public int Right_WhereBeforeOrder()
            => _data
                .Where(p =>
                    p.EsActivo &&
                    p.IdCategoria == categoriaFiltro &&
                    p.Precio > precioMin)
                .OrderBy(p => p.Precio) // ordena solo lo filtrado
                .Count();

        [GlobalCleanup]
        public void Cleanup()
        {
            _db?.Dispose();
        }
    }
}
