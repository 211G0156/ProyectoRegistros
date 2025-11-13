using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Profe.Models;
using ProyectoRegistros.Areas.Profe.Models.ViewModels;
using ProyectoRegistros.Hubs;
using ProyectoRegistros.Models;
using System.Linq;
using System.Security.Claims;


namespace ProyectoRegistros.Areas.Profe.Controllers
{
    [Area("Profe")]
    [Authorize(Roles = "Profesor")]
    public class ProfeController : Controller
    {   
        // contexto para el historial
        private readonly IHubContext<HistorialHub> _hubContext;
        private readonly ProyectoregistroContext _context;

        public ProfeController(ProyectoregistroContext context, IHubContext<HistorialHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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
            var idUser = int.Parse(User.FindFirstValue("Id"));
            var talleres = _context.Listatalleres.Where(x => x.IdAlumno == alumnoId && x.IdTallerNavigation.IdUsuario == idUser).Select(t => new
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
        public async Task<IActionResult> RegistroForm(MisTalleresViewModel model, List<int> TalleresSeleccionados)
        {
            var alumnoExistente = _context.Alumnos.FirstOrDefault(a => a.Nombre == model.Alumno.Nombre && a.Tutor == model.Alumno.Tutor);
            if (alumnoExistente == null)
            {
                model.Alumno.Estado = 1;
                _context.Alumnos.Add(model.Alumno);
                _context.SaveChanges();
            }
            else
            {
                model.Alumno = alumnoExistente;
            }
            bool pagado = false;
            var pagadoForm = Request.Form["Pagado"].FirstOrDefault();
            if (!string.IsNullOrEmpty(pagadoForm) && (pagadoForm == "true" || pagadoForm == "true"))
            {
                pagado = true;
            }

            List<string> talleresDuplicados = new();
            if (TalleresSeleccionados != null && TalleresSeleccionados.Any())
            {
                foreach (var tallerId in TalleresSeleccionados)
                {
                    var taller = _context.Tallers.FirstOrDefault(t => t.Id == tallerId);
                    if (taller == null) continue;

                    bool yaInscrito = _context.Listatalleres.Any(x => x.IdAlumno == model.Alumno.Id && x.IdTaller == tallerId);

                    if (yaInscrito)
                    {
                        talleresDuplicados.Add(taller.Nombre);
                        continue; 
                    }

                    bool esAtencion = taller.Nombre.ToLower().Contains("atencion psicopedagogica");
                    string fechaCita = null;

                    if (esAtencion)
                    {
                        var dias = Request.Form[$"Dias_{tallerId}"];
                        var horaInicio = Request.Form[$"HoraInicio_{tallerId}"];
                        var horaFinal = Request.Form[$"HoraFinal_{tallerId}"];
                        fechaCita = $"{dias} {horaInicio} {horaFinal}".Trim();
                        model.Alumno.AtencionPsico = 1;
                    }
                        var nuevoRegistro = new Listatalleres
                        {
                            IdAlumno = model.Alumno.Id,
                            IdTaller = taller.Id,
                            FechaRegistro = DateTime.Now,
                            FechaCita = fechaCita,
                            Pagado = (sbyte)(pagado ? 1 : 0),
                            FechaPago = pagado ? DateTime.Now : null,
                        };
                        _context.Listatalleres.Add(nuevoRegistro);
                    
                }
                    _context.SaveChanges();
            }
            model.Talleres = _context.Tallers.Where(x => x.Estado == 1).ToList();
            await EnviarNotificacionHub(model.Alumno.Nombre, TalleresSeleccionados);

            if (talleresDuplicados.Any())
            {
                return Json(new {ok =  false, mensaje = "El alumno ya estaba inscrito en: " + string.Join(", ", talleresDuplicados) });
            }
            return RedirectToAction("RegistroForm");
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
        [HttpGet]
        public IActionResult GenerarRecibo(int idAlumno)
        {
            var alumno = _context.Alumnos.FirstOrDefault(a => a.Id == idAlumno);
            var talleres = _context.Listatalleres.Where(l => l.IdAlumno == idAlumno).Select(l => l.IdTaller).ToList();

            if (alumno == null)
                return NotFound();

            var viewModel = new MisTalleresViewModel
            {
                Alumno = alumno,
                Talleres = _context.Tallers.Where(x => x.Estado == 1).ToList()
            };

            return View(viewModel);
        }


        private async Task EnviarNotificacionHub(string nombreAlumno, List<int> talleresSeleccionados)
        {
            if (talleresSeleccionados == null || !talleresSeleccionados.Any()) return;

            var usuario = User?.FindFirstValue("PrimerNombre") ?? "Desconocido";
            var talleres = _context.Tallers.Where(t => talleresSeleccionados.Contains(t.Id)).Select(t => t.Nombre).ToList();

            var data = new
            {
                Fecha = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"),
                Profe = usuario,
                Alumno = nombreAlumno,
                Taller = string.Join(", ", talleres)
            };
            await _hubContext.Clients.All.SendAsync("RecibirHistorial", data);
        }


    }
}
