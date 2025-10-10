using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoRegistros.Areas.Admin.Models.ViewModels;
using ProyectoRegistros.Models;

namespace ProyectoRegistros.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class UsuariosController:Controller
    {
        private readonly ProyectoregistroContext _context;

        public UsuariosController(ProyectoregistroContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            //irán las consultas db
            return View("Alumnos");
        }

        [HttpPost]
        public IActionResult AgregarUsuario(UsuariosViewModel vm)
        {
            return RedirectToAction("Usuarios");
        }

        [HttpPost]
        public IActionResult EditarUsuario(/* parámetros del usuario */)
        {
            return RedirectToAction("Usuarios");
        }

        [HttpPost]
        public IActionResult EliminarUsuario(int id)
        {
            return RedirectToAction("Usuarios");
        }
    }
}
