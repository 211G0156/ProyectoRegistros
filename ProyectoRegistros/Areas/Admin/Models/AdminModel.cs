namespace ProyectoRegistros.Areas.Admin.Models
{
    public class AdminModel
    {
        public class Taller
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Profesor { get; set; }
            public string Días { get; set; }
            public string Edad { get; set; }

            public string Horario { get; set; }
            public decimal Costo { get; set; }
            public int Cupo { get; set; }
            public List<Alumno> Alumnos { get; set; } = new();
        }

        public class Alumno
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public int Edad { get; set; }
            public string Tutor { get; set; }

            public string Telefono { get; set; }
            public string Telefono2 { get; set; }

            public bool Pago { get; set; }

            public string Padecimientos { get; set; }
            public string Talleres { get; set; }
        }

        public class Profesor
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public List<Taller> Talleres { get; set; } = new();
        }

    }
}
