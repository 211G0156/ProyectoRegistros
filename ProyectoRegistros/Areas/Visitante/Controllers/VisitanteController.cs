using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ProyectoRegistros.Areas.Visitante.Controllers
{
    [Authorize(Roles ="Visitante")]
    public class VisitanteController:Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RegistroForm()
        {
            return View();
        }

    }
}
