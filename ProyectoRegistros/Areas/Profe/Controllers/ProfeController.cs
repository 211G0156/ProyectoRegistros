using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var talleres = _context.Tallers.Where(x => x.IdUsuario == idUser).OrderBy(x => x.Nombre).ToList();

            return View(talleres);
        }
        public IActionResult Alumnos()
        {
            var user = User.FindFirstValue("Id");
            var misTalleres = _context.Tallers.Where(x => x.IdUsuario == int.Parse(user)).Select(x => x.Id).ToList();
            var alumno = _context.Listatalleres.Where(x => misTalleres.Contains(x.IdTaller)).Include(a => a.IdAlumnoNavigation)
            .Include(t => t.IdTallerNavigation)
            .OrderBy(x => x.IdAlumnoNavigation.Nombre).ToList();
            return View(alumno);
        }
        [HttpPost]
        public IActionResult EditarAlumno(Alumno alumno)
        {
            var existAlumno = _context.Alumnos.Find(alumno.Id);
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
        [HttpPost]
        public IActionResult EliminarAlumnoEnTaller(int Id, int[] TalleresEliminar)
        {
            if (TalleresEliminar != null && TalleresEliminar.Length > 0)
            {
                foreach (var tallerId in TalleresEliminar)
                {
                    var relacion = _context.Listatalleres.FirstOrDefault(x => x.IdAlumno == Id && x.IdTaller == tallerId);

                    if (relacion != null)
                    {
                        _context.Listatalleres.Remove(relacion);
                    }
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Alumnos");
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
