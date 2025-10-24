namespace ProyectoRegistros.Areas.Admin.Models.ViewModels
{
    public class ExportarDatosVM
    {
        public List<int>? ProfesoresIds { get; set; } = new();
        public List<int>? TalleresIds { get; set; } = new();
        public int? CantidadAlumnosMax { get; set; }
        public string? Dia { get; set; }
        public string? HoraInicio { get; set; }
        public string? HoraFinal { get; set; }
    }
}
