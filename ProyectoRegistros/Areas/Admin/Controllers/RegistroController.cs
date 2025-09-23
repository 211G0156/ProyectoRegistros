using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoRegistros.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]

    public class RegistroController:Controller
    {

    }
}
