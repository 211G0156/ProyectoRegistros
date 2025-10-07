using ProyectoRegistros.Models;

namespace ProyectoRegistros.Areas.Admin.Models.ViewModels
{
    public class TalleresViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Dias { get; set; }
        public int Espacios { get; set; }
        public string Horario { get; set; }
        public string Edad { get; set; }
        public string Profesor { get; set; }
        public decimal Costo { get; set; }
    }



}
