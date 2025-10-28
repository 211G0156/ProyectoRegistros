using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoRegistros.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]

    public class AlumnosController:Controller
    {
        public IActionResult Index()
        {
            // irán las consultas db
            return View("Alumnos");
        }

        [HttpPost]
        public IActionResult Agregar(int id)
        {
            return RedirectToAction("Alumnos");
        }

        [HttpPost]
        public IActionResult Editar()
        {
            return RedirectToAction("Alumnos");
        }

        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            return RedirectToAction("Alumnos");
        }
    }
}
