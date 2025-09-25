namespace ProyectoRegistros.Areas.Admin.Models.ViewModels
{
    public class AlumnosViewModel
    {
        public int Id { get; set; }  // Para editar/eliminar luego
        public string Nombre { get; set; }
        public string Tutor { get; set; }
        public string NumContacto { get; set; }
        public string NumSecundario { get; set; }
        public string Padecimientos { get; set; }

        public List<string> Talleres { get; set; } = new List<string>();
    }
}
