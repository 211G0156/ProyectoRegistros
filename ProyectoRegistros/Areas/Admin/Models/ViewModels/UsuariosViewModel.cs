namespace ProyectoRegistros.Areas.Admin.Models.ViewModels
{
    public class UsuariosViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Correo { get; set; } = "";

        public string Telefono { get; set; } = "";
        public string Contraseña { get; set; } = "";
    }
}
