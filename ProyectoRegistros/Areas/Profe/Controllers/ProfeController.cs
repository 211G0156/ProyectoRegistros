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
                    var lugares = _context.Tallers.FirstOrDefault(x => x.Id == taller);

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
                Talleres = _context.Tallers.Where(x => x.Estado == 1).ToList()
            };
            if (viewModel.Talleres == null)
            {
                viewModel.Talleres = new List<Taller>();
            }
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult RegistroForm(MisTalleresViewModel model, List<int> TalleresSeleccionados)
        {
            if (ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);

                Console.WriteLine("ERRORES: " + string.Join(", ", errores));
            }
            var alumnoExistente = _context.Alumnos.FirstOrDefault(a => a.Nombre == model.Alumno.Nombre);

            if (alumnoExistente == null)
            {
                model.Alumno.Estado = 1;
                _context.Alumnos.Add(model.Alumno);
                _context.SaveChanges();
            }
            else
            {
                model.Alumno = alumnoExistente;
                _context.Update(alumnoExistente);
                _context.SaveChanges();
            }
            bool pagado = false;
            var pagadoForm = Request.Form["Pagado"].FirstOrDefault();
            if (!string.IsNullOrEmpty(pagadoForm) && (pagadoForm == "true" || pagadoForm == "True"))
            {
                pagado = true;
            }

            if (TalleresSeleccionados != null && TalleresSeleccionados.Any())
            {
                var listaTalleres = new List<Listatallere>();
                foreach (var tallerId in TalleresSeleccionados)
                {
                    var taller = _context.Tallers.FirstOrDefault(t => t.Id == tallerId);
                    if (taller != null)
                    {
                        listaTalleres.Add(new Listatallere
                        {
                            IdAlumno = model.Alumno.Id,
                            IdTaller = taller.Id,
                            FechaRegistro = DateTime.Now,
                            FechaCita = taller.Dias,
                            Pagado = (sbyte)(pagado ? 1 : 0),
                            FechaPago = pagado ? DateTime.Now : null,
                        });
                    }
                }
                _context.Listatalleres.AddRange(listaTalleres);
                _context.SaveChanges();
            }
            model.Talleres = _context.Tallers.Where(x => x.Estado == 1).ToList();
            return View(model);
        }
        
        [HttpPost]
        public IActionResult ActualizarPago(int idAlumno, bool pagado)
        {
            var talleres = _context.Listatalleres.Where(l => l.IdAlumno == idAlumno).ToList();
            foreach (var t in talleres)
            {
                t.Pagado = (sbyte)(pagado ? 1 : 0);
                t.FechaPago = pagado ? DateTime.Now : null;
            }

            _context.SaveChanges();

            return Json(new { success = true });
        }
    }
}
