using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Profe.Models.ViewModels;
using ProyectoRegistros.Models;
using System.Linq;


namespace ProyectoRegistros.Areas.Profe.Controllers
{
    [Area("Profe")]
    [Authorize(Roles ="Profesor")]
    public class ProfeController:Controller 
    {
        private readonly ProyectoregistroContext _context;

        public ProfeController(ProyectoregistroContext context)
        {
            _context = context;
        }


        [Route("/Profe/Profe/Index")]
        [Route("/Profe/Profe")]
        [Route("/Profe")]

        public IActionResult Index()
        {
            var talleres = _context.Tallers
                .Include(t => t.IdUsuarioNavigation)
                .Select(t => new MisTalleresViewModel
                {
                    Nombre = t.Nombre,
                    Dias = t.Dias,
                    Espacios = t.LugaresDisp,
                    Horario = t.HoraInicio.ToString(@"hh\:mm tt") + " - " + t.HoraFinal.ToString(@"hh\:mm tt"),
                    Edad = t.EdadMax.HasValue
                           ? $"{t.EdadMin} a {t.EdadMax.Value} años"
                           : $"{t.EdadMin} en adelante",
                    Costo = t.Costo
                })
                .ToList();

            return View(talleres);
        }
        public IActionResult Alumnos()
        {
            var alumnos = _context.Alumnos.ToList(); // trae los alumnos de la BD
            return View(alumnos);
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
