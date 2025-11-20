using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Admin.Models.ViewModels;
using ProyectoRegistros.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProyectoRegistros.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class AlumnosController : Controller
    {
        private readonly ProyectoregistroContext _context;

        public AlumnosController(ProyectoregistroContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm, int? tallerId)
        {
            ViewData["CurrentFilter"] = searchTerm;
            ViewData["CurrentTallerId"] = tallerId;

            ViewBag.Talleres = await _context.Tallers
                .Where(t => t.Estado == 1)
                .OrderBy(t => t.Nombre)
                .Select(t => new { t.Id, t.Nombre })
                .ToListAsync();

            var query = _context.Alumnos
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
                return PartialView("~/Areas/Admin/Views/Home/_AlumnosTabla.cshtml", alumnosUnicos);
            }
            return View("~/Areas/Admin/Views/Home/Alumnos.cshtml", alumnosUnicos);
        }

        [HttpGet]
        public async Task<IActionResult> GetAlumno(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id);
            if (alumno == null) return NotFound();

            return Json(new
            {
                id = alumno.Id,
                nombre = alumno.Nombre,
                tutor = alumno.Tutor,
                numContacto = alumno.NumContacto,
                numSecundario = alumno.NumSecundario,
                padecimientos = alumno.Padecimientos
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditarAlumno(AlumnosViewModel vm)
        {
            var alumno = await _context.Alumnos.FindAsync(vm.Id);
            if (alumno == null) return NotFound();

            alumno.Nombre = vm.Nombre;
            alumno.Tutor = vm.Tutor;
            alumno.NumContacto = vm.NumContacto;
            alumno.NumSecundario = vm.NumSecundario;
            alumno.Padecimientos = vm.Padecimientos;

            _context.Update(alumno);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetTalleresAlumno(int id)
        {
            var talleres = await _context.Listatalleres
                .Include(lt => lt.IdTallerNavigation)
                .Where(lt => lt.IdAlumno == id)
                .Select(lt => new { nombre = lt.IdTallerNavigation.Nombre })
                .ToListAsync();

            return Json(talleres);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarAlumno(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id);
            if (alumno == null) return NotFound();

            alumno.Estado = 0;

             var inscripciones = _context.Listatalleres.Where(l => l.IdAlumno == id);
             _context.Listatalleres.RemoveRange(inscripciones);

            _context.Update(alumno);
            await _context.SaveChangesAsync();

            return Ok("Alumno eliminado correctamente.");
        }
    }
}
