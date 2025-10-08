using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Admin.Models;
using ProyectoRegistros.Areas.Admin.Models.ViewModels;
using ProyectoRegistros.Areas.Profe.Models;
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


        [HttpPost]
        public IActionResult EditarTaller(NuevoTallerVM vm)
        {
            if (ModelState.IsValid)
            {
                var EditTaller = _context.Tallers.Find(vm.Id);
                if(EditTaller!=null)
                {
                    EditTaller.Nombre = vm.Nombre;
                    EditTaller.LugaresDisp = vm.LugaresDisp;
                    EditTaller.Dias = vm.Dias;
                    EditTaller.HoraInicio = vm.HoraInicio;
                    EditTaller.HoraFinal = vm.HoraFinal;
                    EditTaller.EdadMin = vm.EdadMin;
                    EditTaller.EdadMax = vm.EdadMax;
                    EditTaller.Costo = vm.Costo;
                    EditTaller.IdUsuario = vm.IdUsuario;

                    _context.Update(EditTaller);
                    _context.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            return View(vm);

        }

        [HttpPost]
        public IActionResult EliminarTaller(TallerEliminarRequest request)
        {
            if (request.TalleresEliminar != null && request.TalleresEliminar.Length > 0)
            {
                foreach (var taller in request.TalleresEliminar)
                {
                    var buscar = _context.Listatalleres.FirstOrDefault(x => x.IdTaller == taller && x.IdAlumno == request.Id);
                    if (buscar != null)
                    {
                        _context.Listatalleres.Remove(buscar);
                    }
                }
                _context.SaveChanges();
            }
            return Ok(new { success = true });
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
