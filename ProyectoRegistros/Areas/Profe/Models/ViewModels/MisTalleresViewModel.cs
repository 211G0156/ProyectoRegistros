using ProyectoRegistros.Models;

namespace ProyectoRegistros.Areas.Profe.Models.ViewModels
{
    public class MisTalleresViewModel
    {
        //public string Nombre { get; set; }
        //public string Dias { get; set; }
        //public int Espacios { get; set; }
        //public string Horario { get; set; }
        //public string Edad { get; set; }
        //public decimal Costo { get; set; }
        public Alumno Alumno { get; set; } = new Alumno();

        public IEnumerable<Taller>? Talleres { get; set; }
    }
}
