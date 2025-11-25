using System;
using System.Collections.Generic;

namespace ProyectoRegistros.Models;

public partial class Historial
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public int IdAlumno { get; set; }

    public int IdTaller { get; set; }

    public DateTime Fecha { get; set; }

    public string Mensaje { get; set; } = null!;

    public virtual Alumno IdAlumnoNavigation { get; set; } = null!;

    public virtual Taller IdTallerNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
