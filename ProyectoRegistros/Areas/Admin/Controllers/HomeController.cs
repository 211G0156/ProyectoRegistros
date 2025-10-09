using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Admin.Models;
using ProyectoRegistros.Areas.Admin.Models.ViewModels;
using ProyectoRegistros.Areas.Profe.Models;
using ProyectoRegistros.Models;
using ProyectoRegistros.Models.ViewModels;
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
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Dias = t.Dias,
                    Espacios = t.LugaresDisp,
                    Horario = t.HoraInicio.ToString("HH:mm") + " - " + t.HoraFinal.ToString("HH:mm"),
                    Edad = t.EdadMax.HasValue
                           ? $"{t.EdadMin} a {t.EdadMax.Value} años"
                           : $"{t.EdadMin} en adelante",
                    Profesor = t.IdUsuarioNavigation.Nombre,
                    Costo = t.Costo
                })
                .ToList();

            ViewBag.Profesores = _context.Usuarios
    .Where(u => u.IdRol == 2)
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
                try
                {
                    var taller = new Taller
                    {
                        Nombre = vm.Nombre,
                        Dias = vm.Dias,
                        LugaresDisp = vm.LugaresDisp,
                        HoraInicio = vm.HoraInicio,
                        HoraFinal = vm.HoraFinal,
                        EdadMin = vm.EdadMin,
                        EdadMax = vm.EdadMax,
                        Costo = vm.Costo,
                        IdUsuario = vm.IdUsuario,
                        Estado = 1 
                    };

                    _context.Tallers.Add(taller);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
                }
            }

            ViewBag.Profesores = _context.Usuarios.Where(u => u.IdRol == 2).ToList();
            var talleres = _context.Tallers
                .Include(t => t.IdUsuarioNavigation)
                .Select(t => new TalleresViewModel
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Dias = t.Dias,
                    Espacios = t.LugaresDisp,
                    Horario = t.HoraInicio.ToString("HH:mm") + " - " + t.HoraFinal.ToString("HH:mm"),
                    Edad = t.EdadMax.HasValue
                           ? $"{t.EdadMin} a {t.EdadMax.Value} años"
                           : $"{t.EdadMin} en adelante",
                    Profesor = t.IdUsuarioNavigation.Nombre,
                    Costo = t.Costo
                })
                .ToList();

            ViewData["ShowAddModal"] = true;
            return View("Index", talleres);
        }


        [HttpGet]
        public IActionResult EditarTaller(int id)
        {
            var taller = _context.Tallers.Find(id);
            if (taller == null) return NotFound();

            var vm = new NuevoTallerVM
            {
                Id = taller.Id,
                Nombre = taller.Nombre,
                Dias = taller.Dias,
                LugaresDisp = taller.LugaresDisp,
                HoraInicio = taller.HoraInicio,
                HoraFinal = taller.HoraFinal,
                EdadMin = taller.EdadMin,
                EdadMax = taller.EdadMax,
                Costo = taller.Costo,
                IdUsuario = taller.IdUsuario
            };

            ViewBag.Profesores = _context.Usuarios.Where(u => u.IdRol == 2).ToList();
            return View(vm);
        }

        [HttpPost]
        public IActionResult EditarTaller(NuevoTallerVM vm)
        {
            if (ModelState.IsValid)
            {
                var taller = _context.Tallers.Find(vm.Id);
                if (taller != null)
                {
                    taller.Nombre = vm.Nombre;
                    taller.Dias = vm.Dias;
                    taller.LugaresDisp = vm.LugaresDisp;
                    taller.HoraInicio = vm.HoraInicio;
                    taller.HoraFinal = vm.HoraFinal;
                    taller.EdadMin = vm.EdadMin;
                    taller.EdadMax = vm.EdadMax;
                    taller.Costo = vm.Costo;
                    taller.IdUsuario = vm.IdUsuario;

                    _context.Update(taller);
                    _context.SaveChanges();
                    return View("Index", taller);
                }
                return NotFound();
            }
            ViewBag.Profesores = _context.Usuarios.Where(u => u.IdRol == 2).ToList();
            return View("Index");
        }


        [HttpPost]
        public IActionResult EliminarTaller(int? id)
        {
            if (!id.HasValue) return BadRequest("Falta el id del taller.");
            var taller = _context.Tallers.Find(id.Value);
            if (taller != null)
            {
                _context.Tallers.Remove(taller);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return NotFound();
        }
        [HttpGet]
        public IActionResult RegistroForm()
        {
            var viewModel = new MisTalleresViewModel
            {
                Alumno = new Alumno(),
                Talleres = _context.Tallers.Where(x=> x.LugaresDisp > 0 && x.Estado == 1).ToList()
            };
            return View(viewModel);
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
