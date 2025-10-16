using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Profe.Models;
using ProyectoRegistros.Areas.Profe.Models.ViewModels;
using ProyectoRegistros.Models;
using System.Linq;
using System.Security.Claims;


namespace ProyectoRegistros.Areas.Profe.Controllers
{
    [Area("Profe")]
    [Authorize(Roles = "Profesor")]
    public class ProfeController : Controller
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
            //var talleres = _context.Tallers
            //    .Include(t => t.IdUsuarioNavigation)
            //    .Select(t => new MisTalleresViewModel
            //    {
            //        Nombre = t.Nombre,
            //        Dias = t.Dias,
            //        Espacios = t.LugaresDisp,
            //        Horario = t.HoraInicio.ToString(@"hh\:mm tt") + " - " + t.HoraFinal.ToString(@"hh\:mm tt"),
            //        Edad = t.EdadMax.HasValue
            //               ? $"{t.EdadMin} a {t.EdadMax.Value} años"
            //               : $"{t.EdadMin} en adelante",
            //        Costo = t.Costo
            //    })
            //    .ToList();
            var user = User.FindFirstValue("Id");
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var idUser = int.Parse(user);
            var talleres = _context.Taller.Where(x => x.IdUsuario == idUser).OrderBy(x => x.Nombre).ToList();

            return View(talleres);
        }
        public IActionResult Alumnos()
        {
            var user = User.FindFirstValue("Id");
            var misTalleres = _context.Taller.Where(x => x.IdUsuario == int.Parse(user)).Select(x => x.Id).ToList();
            var alumno = _context.Listatalleres.Where(x => misTalleres.Contains(x.IdTaller)).Include(a => a.IdAlumnoNavigation)
            .Include(t => t.IdTallerNavigation)
            .OrderBy(x => x.IdAlumnoNavigation.Nombre).ToList();

            // Agrupar los talleres por alumno
            var alumnosConTalleres = alumno.GroupBy(a => a.IdAlumnoNavigation.Id).Select(g => new
            {
                Alumno = g.FirstOrDefault().IdAlumnoNavigation,
                Talleres = g.Select(t => t.IdTallerNavigation).ToList()
            }).ToList();

            return View(alumnosConTalleres);
        }
        [HttpPost]
        public IActionResult EditarAlumno(Alumno alumno)
        {
            var existAlumno = _context.Alumno.Find(alumno.Id);
            if (existAlumno != null)
            {
                existAlumno.Nombre=alumno.Nombre;
                existAlumno.Tutor=alumno.Tutor;
                existAlumno.NumContacto=alumno.NumContacto;
                existAlumno.NumSecundario=alumno.NumSecundario;
                existAlumno.Padecimientos=alumno.Padecimientos;

                _context.Update(existAlumno);
                _context.SaveChanges();

                return RedirectToAction("Alumnos");
            }
            return View(alumno);
        }
        // traerme los talleres en los que esta registrado el alumno
        [HttpGet("Profe/FuncionTraerTalleres/{alumnoId}")]
        public IActionResult FuncionTraerTalleres(int alumnoId)
        {
            var talleres = _context.Listatalleres.Where(x => x.IdAlumno == alumnoId).Select(t => new
            {
                id = t.IdTaller,
                nombre = t.IdTallerNavigation.Nombre
            }).ToList();

            return Json(talleres);
        }
        [HttpPost]
        public IActionResult eliminarTallerDelAlumno(TallerEliminarRequest request)
        {
            if (request.TalleresEliminar != null && request.TalleresEliminar.Length > 0)
            {
                foreach (var taller in request.TalleresEliminar)
                {
                    var buscar = _context.Listatalleres.FirstOrDefault(x => x.IdTaller == taller && x.IdAlumno == request.Id);
                    var lugares = _context.Taller.FirstOrDefault(x => x.Id == taller);

                    if (buscar != null)
                    {
                        lugares.LugaresDisp++;  // si se elimina se libera el espacio
                        _context.Listatalleres.Remove(buscar);
                    }
                }
                _context.SaveChanges();
            }
            return Ok(new { success = true });
        }
        public IActionResult ExportarDatos()
        {
            return View();
        }
        [HttpGet]
        public IActionResult RegistroForm()
        {
            var viewModel = new MisTalleresViewModel
            {
                Alumno = new Alumno(),
                Talleres = _context.Taller.Where(x=> x.LugaresDisp > 0 && x.Estado == 1).ToList()
            };
            return View(viewModel);
        }
    }
}
