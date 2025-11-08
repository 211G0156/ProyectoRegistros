using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Admin.Models.ViewModels;
using ProyectoRegistros.Models;
using ProyectoRegistros.Services;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ProyectoRegistros.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly ProyectoregistroContext _context;
        private readonly EmailService _emailService;
        public UsuariosController(ProyectoregistroContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
        public async Task<IActionResult> AgregarUsuario(NuevoUsuarioVM vm)
        {
            ModelState.Remove("Contraseña");

            if (ModelState.IsValid)
            {
                try
                {
                    string passwordAleatorio = GenerarPasswordAleatorio();

                    var usuario = new Usuario
                    {
                        Nombre = vm.Nombre,
                        Correo = vm.Correo,
                        NumTel = vm.NumTel,
                        Contraseña = passwordAleatorio, 
                        IdRol = vm.IdRol,
                        Estado = 1
                    };
                    _context.Usuarios.Add(usuario);
                    await _context.SaveChangesAsync(); 

                    var asunto = "Contraseña para el Sistema de Registros";
                    var cuerpo = $"Hola {usuario.Nombre},<br><br>" +
                                 "Se ha creado una cuenta para ti en nuestro sistema.<br>" +
                                 $"Tu contraseña es: <b>{passwordAleatorio}</b><br><br>" +
                                 "Para restablecer la contraseña debes ir con el administrador.";

                    await _emailService.EnviarEmailAsync(usuario.Correo, asunto, cuerpo);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Error al crear usuario: " + ex.Message);

                    System.Diagnostics.Debug.WriteLine("ERROR AL ENVIAR CORREO: " + ex.ToString());
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
            return View("~/Areas/Admin/Views/Home/Usuarios.cshtml", usuarios);
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
                if (usuario != null)
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
    

        private string GenerarPasswordAleatorio(int length = 12)
        {
            const string caracteresValidos = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];
                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(caracteresValidos[(int)(num % (uint)caracteresValidos.Length)]);
                }
            }
            return res.ToString();
        }
    }
}
