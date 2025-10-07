using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Admin.Models;
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

        [HttpPost]
        public IActionResult AgregarTaller(NuevoTallerVM vm)
        {
            if (ModelState.IsValid)
            {
                var nuevoTaller = new Taller
                {
                    Nombre = vm.Nombre,
                    LugaresDisp = vm.Lugares_Disp,
                    Dias = vm.Dias,
                    HoraInicio = vm.Hora_inicio,
                    HoraFinal = vm.Hora_final,
                    EdadMin = vm.Edad_min,
                    EdadMax = vm.Edad_max,
                    Estado = vm.Estado,
                    Costo = vm.Costo,
                    IdUsuario = vm.IdUsuario
                };

                _context.Tallers.Add(nuevoTaller);
                _context.SaveChanges();

            }


            return RedirectToAction("Index");
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
