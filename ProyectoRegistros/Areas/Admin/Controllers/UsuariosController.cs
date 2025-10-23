using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Admin.Models.ViewModels;
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

        public IActionResult Index()
        {
            var usuarios = _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .Where(u => u.Estado == 1)
                .Select(u => new UsuariosViewModel
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Correo = u.Correo,
                    NumTel = u.NumTel,
                    RolNombre = u.IdRolNavigation.Rol1,
                })
                .ToList();

            return View("~/Areas/Admin/Views/Home/Usuarios.cshtml", usuarios);
        }


        [HttpPost]
        public IActionResult AgregarUsuario(NuevoUsuarioVM vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = new Usuario
                    {
                        Nombre = vm.Nombre,
                        Correo = vm.Correo,
                        NumTel = vm.NumTel,
                        Contraseña = vm.Contraseña,
                        IdRol = vm.IdRol,
                        Estado = 1
                    };
                    _context.Usuarios.Add(usuario);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
                }
            }
            var usuarios = _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .Where(u => u.Estado == 1)
                .Select(u => new UsuariosViewModel
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Correo = u.Correo,
                    NumTel = u.NumTel,
                    RolNombre = u.IdRolNavigation.Rol1,
                })
                .ToList();

            ViewData["ShowAddModal"] = true;
            return View("Index", usuarios);
        }

        [HttpGet]
        public IActionResult GetUsuario(int id)
        {
            var usuario = _context.Usuarios.Where(u => u.Estado == 1)
                .Include(u => u.IdRolNavigation)
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
        public IActionResult EditarUsuario(NuevoUsuarioVM vm)
        {
            if (ModelState.IsValid)
            {
                var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == vm.Id);
                if(usuario!=null)
                {
                    usuario.Nombre = vm.Nombre;
                    usuario.Correo = vm.Correo;
                    usuario.NumTel = vm.NumTel;
                    usuario.Contraseña = vm.Contraseña;
                    usuario.IdRol = vm.IdRol;

                    _context.Update(usuario);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                return NotFound();

            }
            return View("Index");

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
