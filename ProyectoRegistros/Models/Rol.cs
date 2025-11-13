using System;
using System.Collections.Generic;

namespace ProyectoRegistros.Models;

public partial class Rol
{
    public int Id { get; set; }

    public string Rol1 { get; set; } = null!;

    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
}
