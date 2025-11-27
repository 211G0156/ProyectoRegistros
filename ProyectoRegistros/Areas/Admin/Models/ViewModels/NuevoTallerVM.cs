using ProyectoRegistros.Models;

namespace ProyectoRegistros.Areas.Admin.Models.ViewModels
{
    public class NuevoTallerVM
    {
        public int Id { get; set; }
        public string? Periodo { get; set; }
        public string Nombre { get; set; }
        public string Dias { get; set; }
        public int LugaresDisp { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFinal { get; set; }
        public int EdadMin { get; set; }
        public int? EdadMax { get; set; }
        public decimal Costo { get; set; }
        public int IdUsuario { get; set; }
    }
}
