using System;
using System.Collections.Generic;

namespace ProyectoRegistros.Models;

public partial class Listatallere
{
    public int Id { get; set; }

    public int IdAlumno { get; set; }

    public int IdTaller { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string? FechaCita { get; set; }

    public sbyte? Pagado { get; set; }

    public DateTime? FechaPago { get; set; }

    public string? Estado { get; set; }

    public virtual Alumno IdAlumnoNavigation { get; set; } = null!;

    public virtual Taller IdTallerNavigation { get; set; } = null!;
}
