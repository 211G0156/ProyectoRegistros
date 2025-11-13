using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Admin.Models.ViewModels;
using ProyectoRegistros.Models;

namespace ProyectoRegistros.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]

    public class AlumnosController:Controller
    {
        private readonly ProyectoregistroContext _context;

        public AlumnosController(ProyectoregistroContext context)
        {
            _context = context;
        }

        // En tu AlumnosController.cs

        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;

            // 1. INICIA LA CONSULTA, PERO AHORA INCLUYE LOS DATOS RELACIONADOS
            // Esto le dice a EF: "Cuando traigas los Alumnos, trae también sus Listatalleres
            // y de esas listas, trae el IdTallerNavigation (el nombre del taller)".
            var query = _context.Alumnos
                .Include(a => a.Listatalleres)
                    .ThenInclude(lt => lt.IdTallerNavigation)
                .Where(a => a.Estado == 1);

            // 2. APLICA TUS FILTROS (esto se queda igual)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.Nombre.Contains(searchTerm));
                query = query
                    .OrderBy(a => a.Nombre.StartsWith(searchTerm) ? 0 : 1)
                    .ThenBy(a => a.Nombre);
            }
            else
            {
                query = query.OrderBy(a => a.Nombre);
            }

            // 3. EJECUTA LA CONSULTA Y CONSTRUYE EL VIEWMODEL
            // Ahora esto funciona porque 'a.Listatalleres' ya está en la memoria
            // y no necesita hacer una nueva consulta a la BD.
            var alumnosFiltrados = await query.Select(a => new AlumnosViewModel
            {
                Id = a.Id,
                Nombre = a.Nombre,
                Tutor = a.Tutor,
                NumContacto = a.NumContacto,
                NumSecundario = a.NumSecundario,
                Padecimientos = a.Padecimientos,
                Talleres = a.Listatalleres.Select(lt => lt.IdTallerNavigation.Nombre).ToList()
            }).ToListAsync(); // <--- ¡Ya no hay error!

            // 4. Envía la lista YA FILTRADA a la vista
            return View("~/Areas/Admin/Views/Home/Alumnos.cshtml", alumnosFiltrados);
        }

        [HttpPost]
        public IActionResult Agregar(int id)
        {
            return RedirectToAction("Alumnos");
        }

        [HttpPost]
        public IActionResult Editar()
        {
            return RedirectToAction("Alumnos");
        }

        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            return RedirectToAction("Alumnos");
        }
    }
}
