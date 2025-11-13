using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProyectoRegistros.Areas.Admin.Models.ViewModels;
using ProyectoRegistros.Models;

namespace ProyectoRegistros.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class ExportarDatosController:Controller
    {

        private readonly ProyectoregistroContext _context;

        public ExportarDatosController(ProyectoregistroContext context)
        {
            _context = context;
        }
        private void CargarViewBags()
        {
            ViewBag.Profesores = _context.Usuarios
                .Where(u => u.IdRol == 2 && u.Estado == 1)
                .OrderBy(u => u.Nombre)
                .ToList();

            ViewBag.Talleres = _context.Tallers
                .Where(t => t.Estado == 1)
                .OrderBy(t => t.Nombre)
                .ToList();

            ViewBag.Dias = _context.Tallers
                .Where(t => t.Estado == 1 && t.Dias != null && t.Dias != "")
                .Select(t => t.Dias)
                .Distinct()
                .ToList();

            ViewBag.Horas = _context.Tallers
                .Where(t => t.Estado == 1)
                .Select(t => new
                {
                    Inicio = t.HoraInicio.ToString("HH:mm"),
                    Fin = t.HoraFinal.ToString("HH:mm")
                })
                .AsEnumerable()
                .Select(h => $"{h.Inicio} - {h.Fin}")
                .Distinct()
                .ToList();
        }


        [HttpGet]
        public IActionResult Index()
        {

            ViewBag.Profesores = _context.Usuarios
                .Where(u => u.IdRol == 2 && u.Estado == 1)
                .OrderBy(u => u.Nombre)
                .ToList();

            ViewBag.Talleres = _context.Tallers
                .Where(t => t.Estado == 1)
                .OrderBy(t => t.Nombre)
                .ToList();

            ViewBag.Dias = _context.Tallers
                .Where(t => t.Estado == 1)
                .Select(t => t.Dias)
                .Distinct()
                .ToList();

            ViewBag.Horas = _context.Tallers
                .Where(t => t.Estado == 1)
                .Select(t => new { t.HoraInicio, t.HoraFinal })
                .Distinct()
                .AsEnumerable()
                .Select(h => $"{h.HoraInicio:HH\\:mm} - {h.HoraFinal:HH\\:mm}")
                .Prepend("Todos")
                .ToList();

            return View("~/Areas/Admin/Views/Home/ExportarDatos.cshtml", new ExportarDatosVM());
        }

        private bool RangoCoincide(TimeOnly inicioTaller, TimeOnly finTaller, TimeOnly inicioFiltro, TimeOnly finFiltro)
        {
            bool tallerCruzaMedianoche = inicioTaller > finTaller;
            bool filtroCruzaMedianoche = inicioFiltro > finFiltro;

            if (!tallerCruzaMedianoche && !filtroCruzaMedianoche)
                return inicioTaller >= inicioFiltro && finTaller <= finFiltro;

            if (tallerCruzaMedianoche && !filtroCruzaMedianoche)
                return false;

            if (!tallerCruzaMedianoche && filtroCruzaMedianoche)
                return false;

            TimeSpan inicioT = inicioTaller.ToTimeSpan();
            TimeSpan finT = finTaller.ToTimeSpan() + TimeSpan.FromHours(24);

            TimeSpan inicioF = inicioFiltro.ToTimeSpan();
            TimeSpan finF = finFiltro.ToTimeSpan() + TimeSpan.FromHours(24);

            return inicioT >= inicioF && finT <= finF;
        }


        [HttpPost]
        public IActionResult DescargarDatos(ExportarDatosVM filtros)
        {
            var diasSeleccionados = Request.Form["Dias"].ToList();
            var rangosSeleccionados = Request.Form["Horas"].ToList();

            bool filtroLlenado =
                (filtros.ProfesoresIds != null && filtros.ProfesoresIds.Any()) ||
                (filtros.TalleresIds != null && filtros.TalleresIds.Any()) ||
                filtros.CantidadAlumnosMax.HasValue ||
                diasSeleccionados.Any() ||
                rangosSeleccionados.Any();

            if (!filtroLlenado)
            {
                TempData["ErrorExportar"] = "Debes seleccionar al menos un filtro para generar el reporte.";
                return RedirectToAction("Index");
            }

            var query = _context.Tallers
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.Listatalleres)
                    .ThenInclude(lt => lt.IdAlumnoNavigation)
                .Where(t => t.Estado == 1)
                .AsEnumerable();

            if (filtros.ProfesoresIds != null && filtros.ProfesoresIds.Any() && !filtros.ProfesoresIds.Contains(0))
                query = query.Where(t => t.IdUsuario.HasValue && filtros.ProfesoresIds.Contains(t.IdUsuario.Value));

            if (filtros.TalleresIds != null && filtros.TalleresIds.Any() && !filtros.TalleresIds.Contains(0))
                query = query.Where(t => filtros.TalleresIds.Contains(t.Id));

            if (filtros.CantidadAlumnosMax.HasValue)
                query = query.Where(t => t.Listatalleres.Count <= filtros.CantidadAlumnosMax.Value);

            if (diasSeleccionados.Any() && !diasSeleccionados.Contains("Todos"))
            {
                query = query.Where(t =>
                    diasSeleccionados.Any(d =>
                        t.Dias != null &&
                        t.Dias.IndexOf(d, StringComparison.OrdinalIgnoreCase) >= 0
                ));
            }

            if (rangosSeleccionados.Any() && !rangosSeleccionados.Contains("Todos"))
            {
                query = query.Where(t =>
                    rangosSeleccionados.Any(rango =>
                    {
                        var partes = rango.Split('-');
                        if (partes.Length != 2) return false;
                        if (TimeOnly.TryParse(partes[0].Trim(), out var inicio) && TimeOnly.TryParse(partes[1].Trim(), out var fin))
                        {
                            return RangoCoincide(t.HoraInicio, t.HoraFinal, inicio, fin);
                        }
                        return false;
                    })
                );
            }

            var talleres = query.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Talleres");
                var currentRow = 1;

                foreach (var taller in talleres)
                {
                    var tallerHeaderCell = worksheet.Cell(currentRow, 1);
                    tallerHeaderCell.Value = $"Taller: {taller.Nombre} | " +
                                             $"Profesor: {taller.IdUsuarioNavigation?.Nombre ?? "N/A"} | " +
                                             $"Horario: {taller.Dias} ({taller.HoraInicio:HH\\:mm} - {taller.HoraFinal:HH\\:mm}) | " +
                                             $"Inscritos: {taller.Listatalleres.Count}";
                    var tallerHeaderRange = worksheet.Range(currentRow, 1, currentRow, 12);
                    tallerHeaderRange.Merge();
                    tallerHeaderRange.Style.Font.Bold = true;
                    tallerHeaderRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    tallerHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "#";
                    worksheet.Cell(currentRow, 2).Value = "Nombre";
                    worksheet.Cell(currentRow, 3).Value = "Edad";
                    worksheet.Cell(currentRow, 4).Value = "Fecha de cumpleaños";
                    worksheet.Cell(currentRow, 5).Value = "Padecimientos";
                    worksheet.Cell(currentRow, 6).Value = "Tutor";
                    worksheet.Cell(currentRow, 7).Value = "Teléfono";
                    worksheet.Cell(currentRow, 8).Value = "Número de respaldo";
                    worksheet.Cell(currentRow, 9).Value = "Dirección";
                    worksheet.Cell(currentRow, 10).Value = "Email";
                    worksheet.Cell(currentRow, 11).Value = "Atención psicopedagógica";
                    worksheet.Cell(currentRow, 12).Value = "Pago";
                    worksheet.Row(currentRow).Style.Font.Bold = true;
                    currentRow++;

                    int studentCounter = 1;
                    var inscritosOrdenados = taller.Listatalleres
                        .OrderBy(lt => lt.Pagado == 1 ? 1 : 2)
                        .ThenBy(lt => lt.IdAlumnoNavigation?.Nombre);

                    foreach (var inscripcion in inscritosOrdenados)
                    {
                        var alumno = inscripcion.IdAlumnoNavigation;
                        if (alumno == null) continue;

                        worksheet.Cell(currentRow, 1).Value = studentCounter++;
                        worksheet.Cell(currentRow, 2).Value = alumno.Nombre;
                        worksheet.Cell(currentRow, 3).Value = alumno.Edad;
                        worksheet.Cell(currentRow, 4).Value = alumno.FechaCumple;
                        worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "dd/MM/yyyy";
                        worksheet.Cell(currentRow, 5).Value = alumno.Padecimientos;
                        worksheet.Cell(currentRow, 6).Value = alumno.Tutor;
                        worksheet.Cell(currentRow, 7).Value = alumno.NumContacto;
                        worksheet.Cell(currentRow, 8).Value = alumno.NumSecundario;
                        worksheet.Cell(currentRow, 9).Value = alumno.Direccion;
                        worksheet.Cell(currentRow, 10).Value = alumno.Email;
                        worksheet.Cell(currentRow, 11).Value = alumno.AtencionPsico == 1 ? "Si" : "No";
                        worksheet.Cell(currentRow, 12).Value = inscripcion.Pagado == 1 ? "Si" : "No";

                        if (inscripcion.Pagado != 1)
                        {
                            worksheet.Range(currentRow, 1, currentRow, 12).Style.Fill.BackgroundColor = XLColor.Yellow;
                            worksheet.Row(currentRow).Style.Font.FontColor = XLColor.Red;
                        }
                        currentRow++;
                    }
                    currentRow++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    var fileName = "ReporteDetalladoTalleres.xlsx";

                    return File(content, contentType, fileName);
                }
            }
        }
    }
}
