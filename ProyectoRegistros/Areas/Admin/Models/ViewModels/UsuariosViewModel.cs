namespace ProyectoRegistros.Areas.Admin.Models.ViewModels
{
    public class UsuariosViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string NumTel { get; set; } = null!;
        public string Contraseña { get; set; } = null!;
        public string RolNombre { get; set; } = null!;
    }
}
