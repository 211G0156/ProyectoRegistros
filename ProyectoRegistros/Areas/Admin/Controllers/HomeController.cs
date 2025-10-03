using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Admin.Models.ViewModels;
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
            public IActionResult Alumnos()
        {
            var alumnos = _context.Alumnos
                .Include(a => a.Listatalleres)
                    .ThenInclude(lt => lt.IdTallerNavigation)
                        .ThenInclude(t => t.IdUsuarioNavigation)
                .Select(a => new AlumnosViewModel
                {
                    Id = a.Id,
                    Nombre = a.Nombre,
                    Tutor = a.Tutor,
                    NumContacto = a.NumContacto,
                    NumSecundario = a.NumSecundario,
                    Padecimientos = a.Padecimientos,
                    Talleres = a.Listatalleres
                        .Select(lt => lt.IdTallerNavigation.Nombre)
                        .ToList()
                })
                .ToList();

            return View(alumnos);
        }
        [HttpGet]
        public IActionResult GetTaller(int id)
        {
            var taller = _context.Tallers
                .Include(t => t.IdUsuarioNavigation)
                .FirstOrDefault(t => t.Id == id);

            if (taller == null)
                return NotFound();

            return Json(new
            {
                id = taller.Id,
                nombre = taller.Nombre,
                dias = taller.Dias,
                espacios = taller.LugaresDisp,
                horaInicio = taller.HoraInicio.ToString(@"hh\\:mm"),
                horaFinal = taller.HoraFinal.ToString(@"hh\\:mm"),
                edadMin = taller.EdadMin,
                edadMax = taller.EdadMax,
                costo = taller.Costo,
                idUsuario = taller.IdUsuario,
                profesor = taller.IdUsuarioNavigation?.Nombre
            });
        }

        [HttpPost]
        public IActionResult EditarTaller([FromBody] Taller taller)
        {
            if (taller == null)
                return BadRequest();

            var dbTaller = _context.Tallers.FirstOrDefault(t => t.Id == taller.Id);
            if (dbTaller == null)
                return NotFound();

            dbTaller.Nombre = taller.Nombre;
            dbTaller.Dias = taller.Dias;
            dbTaller.LugaresDisp = taller.LugaresDisp;
            dbTaller.HoraInicio = taller.HoraInicio;
            dbTaller.HoraFinal = taller.HoraFinal;
            dbTaller.EdadMin = taller.EdadMin;
            dbTaller.EdadMax = taller.EdadMax;
            dbTaller.Costo = taller.Costo;
            dbTaller.IdUsuario = taller.IdUsuario;

            _context.SaveChanges();

            return Json(new { success = true });
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
            var usuarios = _context.Usuarios.ToList(); // trae los usuarios de la BD
            return View(usuarios);
        }

    }
}
