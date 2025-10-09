using ProyectoRegistros.Models;

namespace ProyectoRegistros.Models.ViewModels
{
    public class MisTalleresViewModel
    {
        public Alumno Alumno { get; set; } = new Alumno();

        public IEnumerable<Taller>? Talleres { get; set; }
    }
}