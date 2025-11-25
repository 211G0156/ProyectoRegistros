using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Profe.Models;
using ProyectoRegistros.Hubs;
using ProyectoRegistros.Models;
using ProyectoRegistros.Models.ViewModels;
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

        public async Task<IActionResult> Index(string searchTerm)
        {
            var user = User.FindFirstValue("Id");
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewData["CurrentFilter"] = searchTerm;
            var idUser = int.Parse(user);

            IQueryable<Taller> query = _context.Tallers.AsQueryable();

            query = query.Where(x => x.IdUsuario == idUser && x.Estado == 1);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                bool isNumeric = int.TryParse(searchTerm, out int edad);

                query = query.Where(t => t.Nombre.Contains(searchTerm) || t.Dias.Contains(searchTerm) ||
                    (isNumeric && edad >= t.EdadMin && (t.EdadMax == null || edad <= t.EdadMax))
                );
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.OrderBy(t => t.Nombre.StartsWith(searchTerm) ? 0 : 1).ThenBy(t => t.Dias.StartsWith(searchTerm) ? 2 : 3).ThenBy(t => t.Nombre);
            }
            else
            {
                query = query.OrderBy(t => t.Nombre);
            }

            var talleres = await query.ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProfeTalleresTabla", talleres);
            }

            return View(talleres);
        }
        //public async Task<IActionResult> Alumnos(string searchTerm, int? idTaller)
        //{
        //    var user = User.FindFirstValue("Id");
        //    if (user == null) return RedirectToAction("Index", "Login");

        //    ViewData["CurrentFilter"] = searchTerm;

        //    var misTalleres = _context.Tallers
        //        .Where(x => x.IdUsuario == int.Parse(user))
        //        .Select(x => x.Id)
        //        .ToList();

            IQueryable<Listatallere> query = _context.Listatalleres
                .Include(a => a.IdAlumnoNavigation)
                .Include(t => t.IdTallerNavigation)
                .Where(x => misTalleres.Contains(x.IdTaller));

        //    if (idTaller.HasValue)
        //    {
        //        query = query.Where(x => x.IdTaller == idTaller);
        //        ViewData["TallerFiltrado"] = idTaller;
        //    }

        //    if (!string.IsNullOrEmpty(searchTerm))
        //    {
        //        query = query.Where(x =>
        //            x.IdAlumnoNavigation.Nombre.Contains(searchTerm) ||
        //            x.IdAlumnoNavigation.Padecimientos.Contains(searchTerm) ||
        //            x.IdAlumnoNavigation.Tutor.Contains(searchTerm)
        //        );
        //    }

        //    var alumnosConTalleres = query
        //        .AsEnumerable()
        //        .GroupBy(a => a.IdAlumnoNavigation.Id)
        //        .Select(g => new
        //        {
        //            Alumno = g.First().IdAlumnoNavigation,
        //            Talleres = g.Select(t => t.IdTallerNavigation).ToList()
        //        })
        //        .OrderBy(x => x.Alumno.Nombre)
        //        .ToList();

        //    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        //        return PartialView("_ProfeAlumnosTabla", alumnosConTalleres);

        //    return View(alumnosConTalleres);
        //}

        [HttpPost]
        public IActionResult EditarAlumno(Alumno alumno)
        {
            var existAlumno = _context.Alumnos.Find(alumno.Id);
            if (existAlumno != null)
            {
                existAlumno.Nombre = alumno.Nombre;
                existAlumno.Tutor = alumno.Tutor;
                existAlumno.NumContacto = alumno.NumContacto;
                existAlumno.NumSecundario = alumno.NumSecundario;
                existAlumno.Padecimientos = alumno.Padecimientos;

                _context.Update(existAlumno);
                _context.SaveChanges();

                return RedirectToAction("Alumnos");
            }
            return View(alumno);
        }
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
                        lugares.LugaresDisp++;
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
        public async Task<IActionResult> RegistroForm(MisTalleresViewModel model, List<int> TalleresSeleccionados, List<int> ListaEsperaSeleccionada)
        {
            try
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
                var pagadoForm = Request.Form["PagadoHidden"].FirstOrDefault();
                if (!string.IsNullOrEmpty(pagadoForm) && pagadoForm == "true")
                {
                    pagado = true;
                }


                bool seleccionoAlgo = (TalleresSeleccionados != null && TalleresSeleccionados.Any()) || (ListaEsperaSeleccionada != null && ListaEsperaSeleccionada.Any());

                if (!seleccionoAlgo)
                    return Json(new { ok = false, mensaje = "No se seleccionaron talleres." });

                List<string> talleresDuplicados = new();
                if (TalleresSeleccionados != null)
                {
                    foreach (var tallerId in TalleresSeleccionados)
                    {
                        bool yaInscrito = _context.Listatalleres.Any(x => x.IdAlumno == model.Alumno.Id && x.IdTaller == tallerId);
                        if (yaInscrito)
                        {
                            var taller = _context.Tallers.Find(tallerId);
                            if (taller != null)
                                talleresDuplicados.Add(taller.Nombre);

                        }
                    }
                }
                if (talleresDuplicados.Any())
                {
                    return Json(new
                    {
                        ok = false,
                        mensaje = "El alumno ya estaba inscrito en: " + string.Join(", ", talleresDuplicados)
                    });
                }

                if (TalleresSeleccionados != null)
                {
                    foreach (var tallerId in TalleresSeleccionados)
                    {
                        var taller = _context.Tallers.FirstOrDefault(t => t.Id == tallerId);
                        if (taller == null) continue;

                        if (_context.Listaesperas.Any(x => x.IdAlumno == model.Alumno.Id && x.IdTaller == tallerId))
                        {
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

                        _context.Listatalleres.Add(new Listatallere
                        {
                            IdAlumno = model.Alumno.Id,
                            IdTaller = taller.Id,
                            FechaRegistro = DateTime.Now,
                            FechaCita = fechaCita,
                            Pagado =  (sbyte)(pagado ? 1 : 0),
                            FechaPago = pagado ? DateTime.Now : null,
                            Estado = "Activo"
                        });

                        // disminuye el lugar hasta que ya se confirmó todo
                        taller.LugaresDisp -= 1;
                    }
                }
                var nuevosRegistradosEnEspera = new List<string>();
                if (ListaEsperaSeleccionada != null)
                {
                    foreach (var tallerEsperaId in ListaEsperaSeleccionada)
                    {
                        bool yaEnLista = _context.Listaesperas.Any(x => x.IdAlumno == model.Alumno.Id && x.IdTaller == tallerEsperaId);
                        if (!yaEnLista)
                        {
                            _context.Listaesperas.Add(new Listaespera
                            {
                                IdAlumno = model.Alumno.Id,
                                IdTaller = tallerEsperaId,
                                FechaRegistro = DateTime.Now,
                                Estado = "En espera"
                            });
                        }
                    }
                }


                _context.SaveChanges();

                await EnviarNotificacionHub(model.Alumno.Nombre, TalleresSeleccionados);
                if (nuevosRegistradosEnEspera.Any())
                {
                    return Json(new
                    {
                        ok = false,
                        listaEspera = true,
                        mensaje = "El alumno fue registrado en lista de espera de: " + string.Join(", ", nuevosRegistradosEnEspera)
                    });
                }
                else if (ListaEsperaSeleccionada != null && ListaEsperaSeleccionada.Any())
                {
                    return Json(new
                    {
                        ok = false,
                        listaEsperaDuplicada = true,
                        mensaje = "El alumno ya estaba registrado en lista de espera."
                    });
                }
                return Json(new { ok = true,  idAlumno = model.Alumno.Id });
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { ok = false, mensaje = "Error al registrar: " + detalle });
            }

        }



        [HttpPost]
        public IActionResult ActualizarPago(int idAlumno, bool pagado)
        {
            var talleres = _context.Listatalleres.Where(l => l.IdAlumno == idAlumno && l.FechaRegistro.Date == DateTime.Today).ToList();  //checar
            foreach (var t in talleres)
            {
                t.Pagado = 1;
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

        [HttpGet]
        public IActionResult BuscarAlumno(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return Json(null);

            var alumno = _context.Alumnos.FirstOrDefault(a => a.Nombre.Contains(nombre));

            if (alumno == null)
                return Json(null);

            return Json(new
            {
                alumno.Id,
                alumno.Nombre,
                alumno.FechaCumple,
                alumno.Direccion,
                alumno.Edad,
                alumno.NumContacto,
                alumno.Padecimientos,
                alumno.Tutor,
                alumno.Email,
                alumno.NumSecundario
            });
        }


    }
}
