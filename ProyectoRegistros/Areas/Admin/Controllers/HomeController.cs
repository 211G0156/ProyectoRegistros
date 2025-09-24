using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoRegistros.Models;
using System.Linq;

namespace ProyectoRegistros.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles ="Administrador")]
    public class HomeController:Controller
    {
        private readonly ProyectoregistroContext _context;

        public HomeController(ProyectoregistroContext context)
        {
            _context = context;
        }

        [Route("/Admin/Admin/Index")]
        [Route("/Admin/Admin")]
        [Route("/Admin")]


        public IActionResult Index()
        {
            var talleres = _context.Tallers.ToList(); // trae los talleres de la BD
            return View(talleres);
        }
        public IActionResult Alumnos()
        {
            return View();
        }

        public IActionResult RegistroForm()
        {
            return View();
        }
        public IActionResult ExportarDatos()
        {
            return View();
        }
        public IActionResult Usuarios()
        {
            return View();
        }

    }
}
