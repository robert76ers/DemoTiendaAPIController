using MejoresPracticasLinq.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MejoresPracticasLinq
{
    public static class DataFactory
    {
        public static Producto[] CreateProductos(int count, int seed = 123)
        {
            var rand = new Random(seed);
            var data = new Producto[count];
            for (int i = 0; i < count; i++)
            {
                data[i] = new Producto
                {
                    Id = i + 1,
                    Nombre = "Prod-" + (i + 1),
                    Precio = (decimal)(rand.NextDouble() * 2000.0),
                    CategoriaId = rand.Next(1, 50),
                    Activo = rand.Next(0, 2) == 1
                };
            }
            return data;
        }
    }

}
