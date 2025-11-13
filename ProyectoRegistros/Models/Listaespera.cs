using System;
using System.Collections.Generic;

namespace ProyectoRegistros.Models;

public partial class Listaespera
{
    public int Id { get; set; }

    public int IdAlumno { get; set; }

    public int IdTaller { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Alumno IdAlumnoNavigation { get; set; } = null!;

    public virtual Taller IdTallerNavigation { get; set; } = null!;
}
