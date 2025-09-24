using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoRegistros.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class UsuariosController:Controller
    {
        public IActionResult Index()
        {
            //irán las consultas db
            return View("Alumnos");
        }

        [HttpPost]
        public IActionResult Agregar(/* parámetros del usuario */)
        {
            return RedirectToAction("Usuarios");
        }

        [HttpPost]
        public IActionResult Editar(/* parámetros del usuario */)
        {
            return RedirectToAction("Usuarios");
        }

        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            return RedirectToAction("Usuarios");
        }
    }
}
