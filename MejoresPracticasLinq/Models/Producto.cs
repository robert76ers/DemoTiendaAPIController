using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MejoresPracticasLinq.Models
{
    public sealed class Producto
    {
        public int Id { get; init; }
        public string Nombre { get; init; } = string.Empty;
        public decimal Precio { get; init; }
        public int CategoriaId { get; init; }
        public bool Activo { get; init; }
    }

}
