using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Admin.Models;
using ProyectoRegistros.Areas.Admin.Models.ViewModels;
using ProyectoRegistros.Areas.Profe.Models;
using ProyectoRegistros.Hubs;
using ProyectoRegistros.Models;
using ProyectoRegistros.Models.ViewModels;
using System.Linq;
using System.Security.Claims;

namespace ProyectoRegistros.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class HomeController : Controller
    {
        // contexto para el historial
        private readonly IHubContext<HistorialHub> _hubContext;
        private readonly ProyectoregistroContext _context;

        public HomeController(ProyectoregistroContext context, IHubContext<HistorialHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [Route("/Admin/Admin/Index")]
        [Route("/Admin/Admin")]
        [Route("/Admin")]
        public async Task<IActionResult> Index(string searchTerm, bool recienAgregados = false)
        {
            ViewData["CurrentFilter"] = searchTerm;
            ViewData["RecienAgregadosChecked"] = recienAgregados ? "checked" : "";

            var query = _context.Tallers
                .Include(t => t.IdUsuarioNavigation)
                .Where(t => t.Estado == 1);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                bool isNumeric = int.TryParse(searchTerm, out int edad);

                query = query.Where(t =>
                    t.Nombre.Contains(searchTerm) ||
                    t.Dias.Contains(searchTerm) ||
                    (isNumeric && edad >= t.EdadMin && (t.EdadMax == null || edad <= t.EdadMax))
                );
            }

            if (recienAgregados)
            {
                query = query.OrderByDescending(t => t.Id);
            }
            else if (!string.IsNullOrEmpty(searchTerm))
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

            var talleresVM = await query.Select(t => new TalleresViewModel
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
            .ToListAsync();

            ViewBag.Profesores = await _context.Usuarios
                .Where(u => u.IdRol == 2)
                .ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_TalleresTabla", talleresVM);
            }

            return View(talleresVM);
        }
        public async Task<IActionResult> Alumnos(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;

            var query = _context.Alumnos
                .Include(a => a.Listatalleres)
                    .ThenInclude(lt => lt.IdTallerNavigation)
                .Where(a => a.Estado == 1);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.Nombre.Contains(searchTerm))
                    .OrderBy(a => a.Nombre.StartsWith(searchTerm) ? 0 : 1)
                    .ThenBy(a => a.Nombre);
            }
            else
            {
                query = query.OrderBy(a => a.Nombre);
            }

            var alumnosVM = await query.Select(a => new AlumnosViewModel
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
            .ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_AlumnosTabla", alumnosVM);
            }

            return View(alumnosVM);
        }

        [HttpPost]
        public IActionResult AgregarTaller(NuevoTallerVM vm)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos, revisa los campos del formulario." });
            }

            try
            {
                var talleresProfe = _context.Tallers
                    .Where(t => t.IdUsuario == vm.IdUsuario && t.Estado == 1)
                    .ToList();

                foreach (var t in talleresProfe)
                {
                    var diasExistentes = t.Dias.ToLower()
                        .Split(',')
                        .Select(d => d.Trim())
                        .ToList();

                    var diasNuevo = vm.Dias.ToLower()
                        .Split(',')
                        .Select(d => d.Trim())
                        .ToList();

                    bool mismoDia = diasExistentes.Intersect(diasNuevo).Any();
                    bool traslapeHoras = vm.HoraInicio < t.HoraFinal && vm.HoraFinal > t.HoraInicio;

                    if (mismoDia && traslapeHoras)
                    {
                        return Json(new
                        {
                            success = false,
                            message = $"El profesor ya tiene un taller en ese horario: \"{t.Nombre}\" ({t.HoraInicio:hh\\:mm} - {t.HoraFinal:hh\\:mm})."
                        });
                    }
                }

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

                return Json(new { success = true, message = "Taller agregado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }


        [HttpGet]
        public IActionResult GetTaller(int id)
        {
            var taller = _context.Tallers.
                Where(t => t.Estado==1)
                .Include(t => t.IdUsuarioNavigation)
                .FirstOrDefault(t => t.Id == id);

            if (taller == null)
                return NotFound();

            return Json(new
            {
                id = taller.Id,
                nombre = taller.Nombre,
                dias = taller.Dias,
                lugaresDisp = taller.LugaresDisp,
                horaInicio = taller.HoraInicio.ToString(@"hh\:mm"),
                horaFinal = taller.HoraFinal.ToString(@"hh\:mm"),
                edadMin = taller.EdadMin,
                edadMax = taller.EdadMax,
                costo = taller.Costo,
                idUsuario = taller.IdUsuario
            });
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
                    return RedirectToAction("Index");
                }
                return NotFound();
            }
            ViewBag.Profesores = _context.Usuarios.Where(u => u.IdRol == 2).ToList();
            return View("Index");
        }

        [HttpGet]
        public IActionResult VerificarTaller(int id)
        {
            bool tieneAlumnos = _context.Listatalleres.Any(l => l.IdTaller == id);
            return Json(new { tieneAlumnos });
        }

        [HttpPost]
        public IActionResult EliminarTaller(int id)
        {
            var taller = _context.Tallers
                .Include(t => t.Listatalleres)
                .FirstOrDefault(t => t.Id == id);

            if (taller == null)
                return NotFound();

            taller.Estado = 0;
            _context.SaveChanges();

            if (taller.Listatalleres != null && taller.Listatalleres.Any())
            {
                return Ok("El taller tiene alumnos registrados. Se aplicó baja lógica.");
            }

            return Ok("Taller eliminado correctamente.");
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
        public IActionResult ExportarDatos()
        {
            return View();
        }
        public async Task<IActionResult> Usuarios(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;

            var query = _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .Where(u => u.Estado == 1);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.Nombre.Contains(searchTerm))
                    .OrderBy(u => u.Nombre.StartsWith(searchTerm) ? 0 : 1)
                    .ThenBy(u => u.Nombre);
            }
            else
            {
                query = query.OrderBy(u => u.Nombre);
            }

            var usuariosVM = await query.Select(u => new UsuariosViewModel
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Correo = u.Correo,
                NumTel = u.NumTel,
                RolNombre = u.IdRolNavigation.Rol1,
            })
            .ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_UsuariosTabla", usuariosVM);
            }

            return View("~/Areas/Admin/Views/Home/Usuarios.cshtml", usuariosVM);
        }

        // m queda pendiente organizarlos bn 

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

                        _context.Listatalleres.Add(new Listatalleres
                        {
                            IdAlumno = model.Alumno.Id,
                            IdTaller = taller.Id,
                            FechaRegistro = DateTime.Now,
                            FechaCita = fechaCita,
                            Pagado = (sbyte)(pagado ? 1 : 0),
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
