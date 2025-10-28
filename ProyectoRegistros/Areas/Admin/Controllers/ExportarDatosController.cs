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

        [HttpPost]
        public IActionResult DescargarDatos(ExportarDatosVM filtros)
        {
            var query = _context.Tallers
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.Listatalleres)
                .Where(t => t.Estado == 1)
                .AsEnumerable(); 

            if (filtros.ProfesoresIds != null && filtros.ProfesoresIds.Any() && !filtros.ProfesoresIds.Contains(0))
                query = query.Where(t => filtros.ProfesoresIds.Contains(t.IdUsuario));

            if (filtros.TalleresIds != null && filtros.TalleresIds.Any() && !filtros.TalleresIds.Contains(0))
                query = query.Where(t => filtros.TalleresIds.Contains(t.Id));

            if (filtros.CantidadAlumnosMax.HasValue)
                query = query.Where(t => t.Listatalleres.Count <= filtros.CantidadAlumnosMax.Value);

            var diasSeleccionados = Request.Form["Dias"].ToList();
            if (diasSeleccionados.Any() && !diasSeleccionados.Contains("Todos"))
            {
                query = query.Where(t =>
                    diasSeleccionados.Any(d =>
                        t.Dias != null &&
                        t.Dias.IndexOf(d, StringComparison.OrdinalIgnoreCase) >= 0
                ));
            }

            var rangosSeleccionados = Request.Form["Horas"].ToList();
            if (rangosSeleccionados.Any() && !rangosSeleccionados.Contains("Todos"))
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

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Talleres");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "Taller";
                worksheet.Cell(currentRow, 2).Value = "Profesor";
                worksheet.Cell(currentRow, 3).Value = "Días";
                worksheet.Cell(currentRow, 4).Value = "Horario";
                worksheet.Cell(currentRow, 5).Value = "Alumnos Inscritos";

                foreach (var taller in talleres)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = taller.Nombre;
                    worksheet.Cell(currentRow, 2).Value = taller.IdUsuarioNavigation?.Nombre ?? "N/A"; // Usamos '?' por seguridad
                    worksheet.Cell(currentRow, 3).Value = taller.Dias;
                    worksheet.Cell(currentRow, 4).Value = $"{taller.HoraInicio:HH\\:mm} - {taller.HoraFinal:HH\\:mm}";
                    worksheet.Cell(currentRow, 5).Value = taller.Listatalleres.Count;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    var fileName = "ReporteDeTalleres.xlsx";

                    return File(content, contentType, fileName);
                }
            }
        }
    }
}
