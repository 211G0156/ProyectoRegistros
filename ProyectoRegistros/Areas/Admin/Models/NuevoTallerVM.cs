using ProyectoRegistros.Models;

namespace ProyectoRegistros.Areas.Admin.Models
{
    public class NuevoTallerVM
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Lugares_Disp { get; set; }
        public string Dias { get; set; }
        public TimeOnly Hora_inicio { get; set; }
        public TimeOnly Hora_final { get; set; }
        public int Edad_min { get; set; }
        public int? Edad_max { get; set; }
        public sbyte Estado { get; set; }
        public decimal Costo { get; set; }
        public int IdUsuario { get; set; }
        public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
    }
}
