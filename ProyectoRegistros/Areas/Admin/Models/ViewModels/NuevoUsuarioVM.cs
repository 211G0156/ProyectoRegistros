namespace ProyectoRegistros.Areas.Admin.Models.ViewModels
{
    public class NuevoUsuarioVM
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string NumTel { get; set; }
        public string Contraseña { get; set; }
        public int IdRol { get; set; }

    }
}
