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
                .Select(t => new
                {
                    Inicio = t.HoraInicio,
                    Final = t.HoraFinal
                })
                .AsEnumerable()
                .Select(h => $"{h.Inicio:hh\\:mm} - {h.Final:hh\\:mm}")
                .Distinct()
                .ToList();

            return View("~/Areas/Admin/Views/Home/ExportarDatos.cshtml");
        }

        [HttpPost]
        public IActionResult DescargarDatos(ExportarDatosVM filtros)
        {
            var query = _context.Tallers
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.Listatalleres)
                .Where(t => t.Estado == 1)
                .AsEnumerable();

            if (filtros.ProfesoresIds != null && filtros.ProfesoresIds.Any())
                query = query.Where(t => filtros.ProfesoresIds.Contains(t.IdUsuario));

            if (filtros.TalleresIds != null && filtros.TalleresIds.Any())
                query = query.Where(t => filtros.TalleresIds.Contains(t.Id));

            if (filtros.CantidadAlumnosMax.HasValue)
                query = query.Where(t => t.Listatalleres.Count <= filtros.CantidadAlumnosMax.Value);

            var diasSeleccionados = Request.Form["Dias"].ToList();
            if (diasSeleccionados.Any())
                query = query.Where(t => diasSeleccionados.Any(d => t.Dias.Contains(d)));

            var rangosSeleccionados = Request.Form["Horas"].ToList();
            if (rangosSeleccionados.Any())
            {
                query = query.Where(t =>
                    rangosSeleccionados.Any(rango =>
                    {
                        var partes = rango.Split('-');
                        if (partes.Length != 2) return false;

                        if (TimeOnly.TryParse(partes[0].Trim(), out var inicio) &&
                            TimeOnly.TryParse(partes[1].Trim(), out var fin))
                        {
                            return t.HoraInicio >= inicio && t.HoraFinal <= fin;
                        }
                        return false;
                    })
                );
            }

            var talleres = query.ToList();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Talleres");

            ws.Cell(1, 1).Value = "Taller";
            ws.Cell(1, 2).Value = "Profesor";
            ws.Cell(1, 3).Value = "Cupos Disponibles";
            ws.Cell(1, 4).Value = "Días";
            ws.Cell(1, 5).Value = "Hora Inicio";
            ws.Cell(1, 6).Value = "Hora Final";
            ws.Cell(1, 7).Value = "Alumnos Inscritos";

            ws.Range("A1:G1").Style.Font.Bold = true;
            ws.Range("A1:G1").Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 2;
            foreach (var t in talleres)
            {
                ws.Cell(row, 1).Value = t.Nombre;
                ws.Cell(row, 2).Value = t.IdUsuarioNavigation?.Nombre ?? "Sin asignar";
                ws.Cell(row, 3).Value = t.LugaresDisp;
                ws.Cell(row, 4).Value = t.Dias;
                ws.Cell(row, 5).Value = t.HoraInicio.ToString("HH:mm");
                ws.Cell(row, 6).Value = t.HoraFinal.ToString("HH:mm");
                ws.Cell(row, 7).Value = t.Listatalleres.Count;
                row++;
            }

            ws.Columns().AdjustToContents();

            var wsResumen = workbook.Worksheets.Add("Resumen");
            wsResumen.Cell("A1").Value = "Resumen de Exportación";
            wsResumen.Cell("A1").Style.Font.Bold = true;
            wsResumen.Cell("A1").Style.Font.FontSize = 14;

            wsResumen.Cell("A3").Value = "Total Talleres:";
            wsResumen.Cell("B3").Value = talleres.Count;

            wsResumen.Cell("A4").Value = "Total Alumnos:";
            wsResumen.Cell("B4").Value = talleres.Sum(t => t.Listatalleres.Count);

            wsResumen.Cell("A5").Value = "Total Profesores:";
            wsResumen.Cell("B5").Value = talleres
                .Select(t => t.IdUsuarioNavigation?.Nombre)
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .Count();

            wsResumen.Cell("A6").Value = "Fecha Exportación:";
            wsResumen.Cell("B6").Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            wsResumen.Range("A3:A6").Style.Font.Bold = true;
            wsResumen.Columns().AdjustToContents();
            wsResumen.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            wsResumen.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ExportacionTalleres.xlsx"
            );
        }
    }
}
