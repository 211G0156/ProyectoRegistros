namespace ProyectoRegistros.Services
{
    using System.Threading.Tasks;
    public interface EmailService
    {
        Task EnviarEmailAsync(string destinatario, string asunto, string cuerpo);
    }
}
