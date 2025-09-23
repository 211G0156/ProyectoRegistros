using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ProyectoRegistros.Areas.Visitante.Controllers
{
    [Area("Visitante")]
    [Authorize(Roles ="Visitante")]

    public class VisitanteController:Controller
    {
        [Route("/Visitante/Visitante/Index")]
        [Route("/Visitante/Visitante")]
        [Route("/Visitante")]
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
