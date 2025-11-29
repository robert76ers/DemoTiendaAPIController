using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTienda.Domain.Exceptions
{
    public class CategoriaNoEncontradaException : DomainException
    {
        public int IdCategoria { get; }

        public CategoriaNoEncontradaException(int idCategoria)
            : base($"La categoría con Id {idCategoria} no fue encontrada.")
        {
            IdCategoria = idCategoria;
        }
    }
}
