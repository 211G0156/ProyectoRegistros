using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoRegistros.Areas.Profe.Controllers
{
    [Area("Profe")]
    [Authorize(Roles ="Profesor")]
    public class ProfeController:Controller 
    {

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Alumnos()
        {
            return View();
        }

        public IActionResult ExportarDatos()
        {
            return View();
        }

        public IActionResult RegistroForm()
        {
            return View();
        }
    }
}
