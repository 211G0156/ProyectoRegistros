using Microsoft.AspNetCore.SignalR;

namespace ProyectoRegistros.Hubs
{
    public class HistorialHub : Hub
    {
        public async Task EnviarRegistro(string mensaje)
        {
            await Clients.All.SendAsync("RecibirRegistro", mensaje);
        }
    }
}
