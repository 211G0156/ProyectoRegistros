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
        public async Task<IActionResult> Index(string searchTerm)
        {

            ViewData["CurrentFilter"] = searchTerm;

            IQueryable<Taller> query = _context.Tallers.AsQueryable();

            query = query.Where(x => x.Estado == 1);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                bool isNumeric = int.TryParse(searchTerm, out int edad);

                query = query.Where(t =>
                    t.Nombre.Contains(searchTerm) ||
                    t.Dias.Contains(searchTerm) ||
                    (isNumeric && edad >= t.EdadMin && (t.EdadMax == null || edad <= t.EdadMax))
                );
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query
                   .OrderBy(t => t.Nombre.StartsWith(searchTerm) ? 0 : 1)
                   .ThenBy(t => t.Dias.StartsWith(searchTerm) ? 2 : 3)
                   .ThenBy(t => t.Nombre);
            }
            else
            {
                query = query.OrderBy(t => t.Nombre);
            }

            var talleresVM = await query
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
                .ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_VisitanteTalleresTabla", talleresVM);
            }

            return View(talleresVM);
        }

        public IActionResult RegistroForm()
        {
            return View();
        }
    

    }
}
