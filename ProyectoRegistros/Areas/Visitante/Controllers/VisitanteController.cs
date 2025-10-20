using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Visitante.Models.ViewModels;
using ProyectoRegistros.Models;
using System.Linq;



namespace ProyectoRegistros.Areas.Visitante.Controllers
{
    [Area("Visitante")]
    [Authorize(Roles ="Visitante")]

    public class VisitanteController:Controller
    {
        private readonly ProyectoregistroContext _context;

        public VisitanteController(ProyectoregistroContext context)
        {
            _context = context;
        }

        [Route("/Visitante/Visitante/Index")]
        [Route("/Visitante/Visitante")]
        [Route("/Visitante")]
        public IActionResult Index()
        {
            var talleres = _context.Tallers
                .Include(t => t.IdUsuarioNavigation)
                .Select(t => new TalleresViewModel
                {
                    Nombre = t.Nombre,
                    Dias = t.Dias,
                    Espacios = t.LugaresDisp,
                    Horario = t.HoraInicio.ToString(@"hh\:mm tt") + " - " + t.HoraFinal.ToString(@"hh\:mm tt"),
                    Edad = t.EdadMax.HasValue
                           ? $"{t.EdadMin} a {t.EdadMax.Value} años"
                           : $"{t.EdadMin} en adelante",
                    Profesor = t.IdUsuarioNavigation.Nombre,
                    Costo = t.Costo
                })
                .ToList();

            return View(talleres);
        }

        public IActionResult RegistroForm()
        {
            return View();
        }

    }
}
