using System;
using System.Collections.Generic;

namespace ProyectoRegistros.Models;

public partial class Taller
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int LugaresDisp { get; set; }

    public string Dias { get; set; } = null!;

    public sbyte ConCita { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFinal { get; set; }

    public int EdadMin { get; set; }

    public int? EdadMax { get; set; }

    public sbyte Estado { get; set; }

    public decimal Costo { get; set; }

    public int IdUsuario { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Listaespera> Listaespera { get; set; } = new List<Listaespera>();

    public virtual ICollection<Listatalleres> Listatalleres { get; set; } = new List<Listatalleres>();
}
