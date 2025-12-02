using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
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
        public async Task<IActionResult> Alumnos(string searchTerm, int? tallerId)
        {
            ViewData["CurrentFilter"] = searchTerm;
            ViewData["CurrentTallerId"] = tallerId;

            ViewBag.Talleres = await _context.Tallers
              .Where(t => t.Estado == 1)
              .OrderBy(t => t.Nombre)
              .Select(t => new { t.Id, t.Nombre })
              .ToListAsync();

            var query = _context.Alumnos
              .Include(a => a.Listatalleres)
                .ThenInclude(lt => lt.IdTallerNavigation)
              .Where(a => a.Estado == 1);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a =>
                            a.Nombre.Contains(searchTerm) ||
                            a.Tutor.Contains(searchTerm) ||
                            a.Padecimientos.Contains(searchTerm)
                        );
            }

            if (tallerId.HasValue && tallerId > 0)
            {
                query = query.Where(a => a.Listatalleres.Any(lt => lt.IdTaller == tallerId.Value));
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.OrderBy(a => a.Nombre.StartsWith(searchTerm) ? 0 : 1).ThenBy(a => a.Nombre);
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

            var alumnosUnicos = alumnosVM
              .GroupBy(a => new { a.Nombre, a.Tutor })
              .Select(g => g.First())
              .ToList();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_AlumnosTabla", alumnosUnicos);
            }

            return View("~/Areas/Admin/Views/Home/Alumnos.cshtml", alumnosUnicos);
        }
        [HttpPost]
        public IActionResult AgregarTaller(NuevoTallerVM vm)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos, revisa los campos del formulario." });
            }
            if (vm.EdadMax.HasValue && vm.EdadMin >= vm.EdadMax.Value)
            {
                return Json(new { success = false, message = "La edad máxima debe ser mayor que la edad mínima." });
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
                    Estado = 1,

                    Inscritos = 0,
                    Año = DateTime.Now.Year,
                    Periodo = vm.Periodo
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
                idUsuario = taller.IdUsuario,

                periodo = taller.Periodo
            });
        }

        [HttpPost]
        public IActionResult EditarTaller(NuevoTallerVM vm)
        {
            ModelState.Remove("IdUsuario");
            if (vm.EdadMax.HasValue && vm.EdadMin >= vm.EdadMax.Value)
            {
                ModelState.AddModelError("EdadMax", "La edad máxima debe ser mayor a la mínima.");
            }

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
                    taller.Periodo = vm.Periodo;
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
                        mensaje = "El alumno ya estaba inscrito en: " + string.Join(", ", talleresDuplicados.Distinct())
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
                            var taller = _context.Tallers.Find(tallerEsperaId);
                            if (taller != null) nuevosRegistradosEnEspera.Add(taller.Nombre);
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
                return Json(new { ok = true, mensaje = "Registro guardado correctamente.", idAlumno = model.Alumno.Id });
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
            if (talleresSeleccionados == null || !talleresSeleccionados.Any())
                return;

            var userIdString = User.FindFirst("Id")?.Value; 
            if (string.IsNullOrEmpty(userIdString))
            {
                throw new Exception("El usuario no tiene el claim IdUsuario. Agrega el claim en el login.");
            }

            int idUsuario = int.Parse(userIdString);
            var alumno = await _context.Alumnos.FirstOrDefaultAsync(a => a.Nombre == nombreAlumno);

            if (alumno == null)
                return;

            var talleres = _context.Tallers.Where(t => talleresSeleccionados.Contains(t.Id)).ToList();
            foreach (var taller in talleres)
            {
                var historial = new Historial
                {
                    IdUsuario = idUsuario,                          
                    IdAlumno = alumno.Id,
                    IdTaller = taller.Id,
                    Fecha = DateTime.Now,
                    Mensaje = $"{User?.FindFirstValue("PrimerNombre")} registró a {nombreAlumno} en {taller.Nombre}"
                };

                _context.Historials.Add(historial);
            }

            await _context.SaveChangesAsync();
            var data = new
            {
                Fecha = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"),
                Profe = User?.FindFirstValue("PrimerNombre"),
                Alumno = nombreAlumno,
                Taller = string.Join(", ", talleres.Select(t => t.Nombre))
            };

            await _hubContext.Clients.All.SendAsync("RecibirRegistro", data);
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
        public IActionResult Configuracion()
        {
            return View();
        }
        public IActionResult RespaldarBD()
        {
            string backupPath = Path.Combine(Path.GetTempPath(), "backup.sql");
            string connection = _context.Database.GetDbConnection().ConnectionString;
            using (var conn = new MySqlConnection(connection))
            {
                using (var cmd = new MySqlCommand())
                {
                    using (var mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ExportToFile(backupPath);
                        conn.Close();
                    }
                }
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(backupPath);
            return File(fileBytes, "application/sql", "RespaldoBD.sql");
        }
        [HttpPost]
        public IActionResult LimpiarTablas([FromBody] List<string> tablas)
        {
            foreach (var tabla in tablas)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw($"DELETE FROM {tabla};");
                    _context.Database.ExecuteSqlRaw($"ALTER TABLE {tabla} AUTO_INCREMENT = 1;");
                }
                catch
                {
                    return Json(new { mensaje = $"Error al limpiar la tabla: {tabla}" });
                }
            }

            return Json(new { mensaje = "Tablas reiniciadas correctamente." });
        }


    }
}
