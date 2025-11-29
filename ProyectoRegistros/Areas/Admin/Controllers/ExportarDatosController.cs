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

            ViewBag.Años = _context.Tallers
                .Where(t => t.Año != null)
                .Select(t => t.Año)
                .Distinct()
                .OrderByDescending(a => a)
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
            var diasSeleccionados = Request.Form["Dias"].ToList() ?? new List<string>();
            var rangosSeleccionados = Request.Form["Horas"].ToList() ?? new List<string>();

            bool filtroLlenado =
                (filtros.ProfesoresIds != null && filtros.ProfesoresIds.Any()) ||
                (filtros.TalleresIds != null && filtros.TalleresIds.Any()) ||
                filtros.CantidadAlumnosMax.HasValue ||
                filtros.AñoSeleccionado.HasValue ||
                !string.IsNullOrEmpty(filtros.PeriodoSeleccionado) ||
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
                .Include(t => t.Listaesperas)
                    .ThenInclude(le => le.IdAlumnoNavigation)
                .AsQueryable();

            bool esBusquedaHistorica = filtros.AñoSeleccionado.HasValue || !string.IsNullOrEmpty(filtros.PeriodoSeleccionado);

            if (!esBusquedaHistorica)
            {
                query = query.Where(t => t.Estado == 1);
            }

            if (filtros.AñoSeleccionado.HasValue)
                query = query.Where(t => t.Año == filtros.AñoSeleccionado.Value);

            if (!string.IsNullOrEmpty(filtros.PeriodoSeleccionado))
                query = query.Where(t => t.Periodo == filtros.PeriodoSeleccionado);

            if (filtros.ProfesoresIds != null && !filtros.ProfesoresIds.Contains(0) && filtros.ProfesoresIds.Any())
                query = query.Where(t => t.IdUsuario.HasValue && filtros.ProfesoresIds.Contains(t.IdUsuario.Value));

            if (filtros.TalleresIds != null && !filtros.TalleresIds.Contains(0) && filtros.TalleresIds.Any())
                query = query.Where(t => filtros.TalleresIds.Contains(t.Id));

            var talleres = query.ToList();

            if (filtros.CantidadAlumnosMax.HasValue)
            {
                talleres = talleres.Where(t => t.Listatalleres.Count == filtros.CantidadAlumnosMax.Value).ToList();
            }

            if (diasSeleccionados.Any() && !diasSeleccionados.Contains("Todos"))
            {
                talleres = talleres.Where(t =>
                    diasSeleccionados.Any(d => t.Dias != null && t.Dias.Contains(d, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (rangosSeleccionados.Any() && !rangosSeleccionados.Contains("Todos"))
            {
                talleres = talleres.Where(t =>
                    rangosSeleccionados.Any(rango =>
                    {
                        var partes = rango.Split('-');
                        if (partes.Length != 2) return false;
                        var inicioStr = partes[0].Trim();
                        var finStr = partes[1].Trim();
                        if (TimeOnly.TryParse(inicioStr, out var inicio) && TimeOnly.TryParse(finStr, out var fin))
                        {
                            return RangoCoincide(t.HoraInicio, t.HoraFinal, inicio, fin);
                        }
                        return false;
                    })
                ).ToList();
            }

            if (!talleres.Any())
            {
                TempData["ErrorExportar"] = "No se encontraron talleres con esos filtros.";
                return RedirectToAction("Index");
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Talleres");
                var currentRow = 1;

                string titulo = esBusquedaHistorica
                    ? $"REPORTE HISTÓRICO - {filtros.PeriodoSeleccionado} {filtros.AñoSeleccionado}"
                    : "REPORTE DE TALLERES ACTUALES";

                worksheet.Cell(currentRow, 1).Value = titulo;
                worksheet.Range(currentRow, 1, currentRow, 12).Merge().Style.Font.FontSize = 14;
                currentRow += 2;

                foreach (var taller in talleres)
                {
                    string estadoTxt = taller.Estado == 0 ? "(ELIMINADO)" : "";
                    var info = $"Taller: {taller.Nombre} {estadoTxt} | Prof: {taller.IdUsuarioNavigation?.Nombre ?? "N/A"} | " +
                               $"Horario: {taller.Dias} ({taller.HoraInicio:HH\\:mm}-{taller.HoraFinal:HH\\:mm}) | " +
                               $"Inscritos: {taller.Listatalleres.Count}/{taller.LugaresDisp}";

                    var header = worksheet.Cell(currentRow, 1);
                    header.Value = info;
                    var rng = worksheet.Range(currentRow, 1, currentRow, 12);
                    rng.Merge();
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.BackgroundColor = taller.Estado == 0 ? XLColor.LightSalmon : XLColor.LightGray;
                    rng.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = "#";
                    worksheet.Cell(currentRow, 2).Value = "Nombre";
                    worksheet.Cell(currentRow, 3).Value = "Edad";
                    worksheet.Cell(currentRow, 4).Value = "Cumpleaños";
                    worksheet.Cell(currentRow, 5).Value = "Padecimientos";
                    worksheet.Cell(currentRow, 6).Value = "Tutor";
                    worksheet.Cell(currentRow, 7).Value = "Teléfono";
                    worksheet.Cell(currentRow, 8).Value = "Respaldo";
                    worksheet.Cell(currentRow, 9).Value = "Dirección";
                    worksheet.Cell(currentRow, 10).Value = "Email";
                    worksheet.Cell(currentRow, 11).Value = "Psicopedagógica";
                    worksheet.Cell(currentRow, 12).Value = "Pago";
                    worksheet.Row(currentRow).Style.Font.Bold = true;
                    currentRow++;

                    int cont = 1;
                    foreach (var ins in taller.Listatalleres.OrderBy(x => x.Pagado == 1 ? 1 : 2).ThenBy(x => x.IdAlumnoNavigation?.Nombre))
                    {
                        var alum = ins.IdAlumnoNavigation;
                        if (alum == null) continue;

                        worksheet.Cell(currentRow, 1).Value = cont++;
                        worksheet.Cell(currentRow, 2).Value = alum.Nombre;
                        worksheet.Cell(currentRow, 3).Value = alum.Edad;
                        worksheet.Cell(currentRow, 4).Value = alum.FechaCumple;
                        worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "dd/MM/yyyy";
                        worksheet.Cell(currentRow, 5).Value = alum.Padecimientos;
                        worksheet.Cell(currentRow, 6).Value = alum.Tutor;
                        worksheet.Cell(currentRow, 7).Value = alum.NumContacto;
                        worksheet.Cell(currentRow, 8).Value = alum.NumSecundario;
                        worksheet.Cell(currentRow, 9).Value = alum.Direccion;
                        worksheet.Cell(currentRow, 10).Value = alum.Email;
                        worksheet.Cell(currentRow, 11).Value = alum.AtencionPsico == 1 ? "Si" : "No";
                        worksheet.Cell(currentRow, 12).Value = ins.Pagado == 1 ? "Si" : "No";

                        if (ins.Pagado != 1)
                        {
                            worksheet.Range(currentRow, 1, currentRow, 12).Style.Fill.BackgroundColor = XLColor.Yellow;
                            worksheet.Row(currentRow).Style.Font.FontColor = XLColor.Red;
                        }
                        currentRow++;
                    }

                    if (taller.Listaesperas != null && taller.Listaesperas.Any())
                    {
                        currentRow++;
                        var wHeader = worksheet.Cell(currentRow, 1);
                        wHeader.Value = $"LISTA DE ESPERA ({taller.Listaesperas.Count})";

                        var rngEspera = worksheet.Range(currentRow, 1, currentRow, 12);
                        rngEspera.Merge();
                        rngEspera.Style.Font.Bold = true;
                        rngEspera.Style.Fill.BackgroundColor = XLColor.LightYellow;
                        rngEspera.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        currentRow++;

                        worksheet.Cell(currentRow, 1).Value = "#";
                        worksheet.Cell(currentRow, 2).Value = "Nombre";
                        worksheet.Cell(currentRow, 3).Value = "Fecha Registro";
                        worksheet.Cell(currentRow, 4).Value = "Teléfono";
                        worksheet.Row(currentRow).Style.Font.Italic = true;
                        currentRow++;

                        int wCont = 1;
                        foreach (var esp in taller.Listaesperas.OrderBy(e => e.FechaRegistro))
                        {
                            var alumEsp = esp.IdAlumnoNavigation;
                            if (alumEsp == null) continue;

                            worksheet.Cell(currentRow, 1).Value = wCont++;
                            worksheet.Cell(currentRow, 2).Value = alumEsp.Nombre;
                            worksheet.Cell(currentRow, 3).Value = esp.FechaRegistro;
                            worksheet.Cell(currentRow, 3).Style.NumberFormat.Format = "dd/MM/yyyy HH:mm";
                            worksheet.Cell(currentRow, 4).Value = alumEsp.NumContacto;
                            currentRow++;
                        }
                    }
                    currentRow += 2;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteTalleres.xlsx");
                }
            }
        }
    }
}

