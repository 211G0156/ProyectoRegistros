using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Models;
using System.Linq;

namespace ProyectoRegistros.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly ProyectoregistroContext _context;

        public UsuariosController(ProyectoregistroContext context)
        {
            _context = context;
        }

        public IActionResult Usuarios()
        {
            var usuarios = _context.Usuarios
                .Where(u => u.Estado == 1)
                .Include(u => u.IdRolNavigation)
                .AsNoTracking()
                .ToList();


            return View("~/Areas/Admin/Views/Home/Usuarios.cshtml", usuarios);
        }


        [HttpPost]
        public IActionResult AgregarUsuario(Usuario nuevo)
        {
            if (ModelState.IsValid)
            {
                nuevo.Estado = 1;

                _context.Usuarios.Add(nuevo);
                _context.SaveChanges();
                return RedirectToAction("Usuarios");
            }

            var usuarios = _context.Usuarios
                .Where(u => u.Estado == 1)
                .Include(u => u.IdRolNavigation)
                .ToList();

            return View("~/Areas/Admin/Views/Home/Usuarios.cshtml", usuarios);
        }



        [HttpGet]
        public IActionResult GetUsuario(int id)
        {
            var usuario = _context.Usuarios
                .Where(u => u.Estado == 1)
                .FirstOrDefault(u => u.Id == id);

            if (usuario == null)
                return NotFound();

            return Json(new
            {
                id = usuario.Id,
                nombre = usuario.Nombre,
                correo = usuario.Correo,
                numTel = usuario.NumTel,
                contraseña = usuario.Contraseña,
                idRol = usuario.IdRol
            });
        }

        [HttpPost]
        public IActionResult EditarUsuario(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var original = _context.Usuarios.Find(usuario.Id);
                if (original != null)
                {
                    original.Nombre = usuario.Nombre;
                    original.Correo = usuario.Correo;
                    original.NumTel = usuario.NumTel;
                    original.Contraseña = usuario.Contraseña;
                    original.IdRol = usuario.IdRol;

                    _context.Update(original);
                    _context.SaveChanges();
                    return RedirectToAction("Usuarios");
                }
                return NotFound();
            }
            var usuarios = _context.Usuarios
                .Where(u => u.Estado == 1)
                .Include(u => u.IdRolNavigation)
                .ToList();

            return View("~/Areas/Admin/Views/Home/Usuarios.cshtml", usuarios);
        }

        [HttpPost]
        public IActionResult EliminarUsuario(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
                return NotFound();

            usuario.Estado = 0;
            _context.SaveChanges();

            if (usuario.Tallers != null && usuario.Tallers.Any())
            {
                return Ok("El usuario tiene talleres registrados. Se aplicó baja lógica.");
            }
            return RedirectToAction("Usuarios");
        }
    }
}
