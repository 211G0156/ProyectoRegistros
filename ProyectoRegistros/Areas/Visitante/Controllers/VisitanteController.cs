using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Visitante.Models.ViewModels;
using ProyectoRegistros.Hubs;
using ProyectoRegistros.Models;
using ProyectoRegistros.Models.ViewModels;
using System.Linq;
using System.Security.Claims;



namespace ProyectoRegistros.Areas.Visitante.Controllers
{
    [Area("Visitante")]
    [Authorize(Roles ="Visitante")]

    public class VisitanteController:Controller
    {
        // contexto para el historial
        private readonly IHubContext<HistorialHub> _hubContext;
        private readonly ProyectoregistroContext _context;

        public VisitanteController(ProyectoregistroContext context, IHubContext<HistorialHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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
                    pagado = true;

                bool seleccionoAlgo = (TalleresSeleccionados != null && TalleresSeleccionados.Any()) || (ListaEsperaSeleccionada != null && ListaEsperaSeleccionada.Any());

                if (!seleccionoAlgo)
                    return Json(new { ok = false, mensaje = "No se seleccionaron talleres." });

                HashSet<string> talleresDuplicados = new();
                if (TalleresSeleccionados != null)
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

                        _context.Listatalleres.Add(new Listatallere
                        {
                            IdAlumno = model.Alumno.Id,
                            IdTaller = taller.Id,
                            FechaRegistro = DateTime.Now,
                            FechaCita = fechaCita,
                            Pagado = 1,
                            //  Pagado = (sbyte)(pagado ? 1 : 0),
                            FechaPago = pagado ? DateTime.Now : null,
                        });
                        taller.LugaresDisp -= 1; // disminuir uno al registrar
                    }
                }


                if (ListaEsperaSeleccionada != null && ListaEsperaSeleccionada.Any())
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

                if (talleresDuplicados.Any())
                    return Json(new { ok = false, mensaje = "El alumno ya estaba inscrito en: " + string.Join(", ", talleresDuplicados) });

                return Json(new { ok = true, mensaje = "Registro guardado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, mensaje = "Error al registrar: " + ex.Message });
            }
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
