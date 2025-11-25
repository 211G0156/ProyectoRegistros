using System;
using System.Collections.Generic;

namespace ProyectoRegistros.Models;

public partial class Alumno
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public DateTime FechaCumple { get; set; }

    public int Edad { get; set; }

    public string Direccion { get; set; } = null!;

    public string NumContacto { get; set; } = null!;

    public string NumSecundario { get; set; } = null!;

    public string? Padecimientos { get; set; }

    public string Tutor { get; set; } = null!;

    public string Email { get; set; } = null!;

    public sbyte AtencionPsico { get; set; }

    public sbyte Estado { get; set; }

    public virtual ICollection<Listaespera> Listaesperas { get; set; } = new List<Listaespera>();

    public virtual ICollection<Listatallere> Listatalleres { get; set; } = new List<Listatallere>();
}
