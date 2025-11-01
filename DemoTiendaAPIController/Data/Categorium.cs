using System;
using System.Collections.Generic;

namespace DemoTiendaAPIController.Data;

public partial class Categorium
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool EsActiva { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
