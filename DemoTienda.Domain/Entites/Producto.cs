using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DemoTienda.Domain.Entites;

public partial class Producto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public int IdCategoria { get; set; }

    public bool EsActivo { get; set; }

    public DateTime FechaCreacion { get; set; }

    [JsonIgnore]
    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;
}
