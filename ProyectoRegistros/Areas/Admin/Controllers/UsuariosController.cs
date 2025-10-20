using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Admin.Models.ViewModels;
using ProyectoRegistros.Models;

namespace ProyectoRegistros.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class UsuariosController:Controller
    {
        private readonly ProyectoregistroContext _context;

        public UsuariosController(ProyectoregistroContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var usuarios = _context.Usuarios
                .Where(u => u.Estado == 1)
                .OrderBy(u => u.Nombre)
                .ToList();

            return View(usuarios);  
        }

        [HttpPost]
        public IActionResult AgregarUsuario(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usuarios = new Usuario
                    {
                        Nombre = usuario.Nombre,
                        Correo = usuario.Correo,
                        NumTel = usuario.NumTel,
                        Contraseña = usuario.Contraseña,
                        IdRol = usuario.IdRol,
                        Estado = 1
                    };
                    _context.Usuarios.Add(usuario);
                    _context.SaveChanges();
                    return RedirectToAction("Usuarios");
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
                }
            }

            ViewData["ShowAddModal"] = true;
            return View("Usuarios", usuario);
        }

        [HttpGet]
        public IActionResult GetUsuario(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
                return NotFound();

            return Json(new
            {
                id = usuario.Id,
                nombre = usuario.Nombre,
                correo = usuario.Correo,
                numTel = usuario.NumTel,
                contraseña = usuario.Contraseña
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
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View("Usuarios", _context.Usuarios.ToList());
        }

        [HttpPost]
        public IActionResult EliminarUsuario(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
                return NotFound();
            usuario.Estado = 0;
            _context.SaveChanges();
            return Ok("Usuario eliminado correctamente.");
        }
    }
}
