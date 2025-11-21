using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Areas.Admin.Models.ViewModels;
using ProyectoRegistros.Models;
using ProyectoRegistros.Services;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
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

        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;

            var query = _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .Where(u => u.Estado == 1);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.Nombre.Contains(searchTerm))
                    .OrderBy(u => u.Nombre.StartsWith(searchTerm) ? 0 : 1)
                    .ThenBy(u => u.Nombre);
            }
            else
            {
                query = query.OrderBy(u => u.Nombre);
            }

            var usuariosVM = await query.Select(u => new UsuariosViewModel
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Correo = u.Correo,
                NumTel = u.NumTel,
                RolNombre = u.IdRolNavigation.Rol1,
            }).ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("~/Areas/Admin/Views/Home/_UsuariosTabla.cshtml", usuariosVM);
            }

            return View("~/Areas/Admin/Views/Home/Usuarios.cshtml", usuariosVM);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarUsuario(NuevoUsuarioVM vm)
        {
            ModelState.Remove("Contraseña");

            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Error en los datos del formulario." });

            try
            {
                string nombreNormalizado = RemoveAccents(vm.Nombre).ToUpper();

                var usuarioInactivo = await _context.Usuarios
                    .Where(u => u.Estado == 0)
                    .ToListAsync();

                var match = usuarioInactivo.FirstOrDefault(u =>
                    RemoveAccents(u.Nombre).ToUpper() == nombreNormalizado
                );

                if (match != null)
                {
                    return Json(new
                    {
                        success = "reactivar",
                        id = match.Id,
                        nombre = match.Nombre,
                        message = $"El usuario '{match.Nombre}' existe pero está inactivo. ¿Deseas reactivarlo?"
                    });
                }

                bool existe = await _context.Usuarios
                    .AnyAsync(u => u.Estado == 1 &&
                                   (u.Nombre == vm.Nombre || u.Correo == vm.Correo));

                if (existe)
                {
                    return Json(new { success = false, message = "Ya existe un usuario activo con ese nombre o correo electrónico." });
                }

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
                var cuerpo = $"Hola {usuario.Nombre}, haz sido añadido al sistema<br><br>" +
                             $"Tu contraseña es: <b>{passwordAleatorio}</b><br>" +
                             $" Para restablecer tu contraseña debes ir directamente con el Administrador.";

                await _emailService.EnviarEmailAsync(usuario.Correo, asunto, cuerpo);

                return Json(new { success = true, message = "Usuario agregado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al crear usuario: " + ex.Message });
            }
        }

        private string RemoveAccents(string input)
        {
            return new string(input
                .Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray())
                .Normalize(NormalizationForm.FormC);
        }

        [HttpGet]
        public IActionResult GetUsuario(int id)
        {
            var usuario = _context.Usuarios
                .Where(u => u.Estado == 1)
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
                idRol = usuario.IdRol
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditarUsuario(NuevoUsuarioVM vm)
        {
            ModelState.Remove("Contraseña");

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == vm.Id);

            if (usuario == null)
                return Json(new { success = false, message = "Usuario no encontrado." });

            bool duplicado = await _context.Usuarios
                .AnyAsync(u => u.Id != vm.Id &&
                               (u.Nombre == vm.Nombre || u.Correo == vm.Correo));

            if (duplicado)
            {
                return Json(new { success = false, message = "Ya existe otro usuario con ese nombre o correo electrónico." });
            }

            int usuarioActualId = int.Parse(User.FindFirst("Id").Value);

            bool correoCambio = usuario.Correo != vm.Correo;
            bool cambioRol = usuario.IdRol != vm.IdRol;

            if (usuarioActualId == vm.Id && cambioRol)
            {
                return Json(new
                {
                    success = false,
                    message = "No puedes cambiar tu propio rol."
                });
            }

            try
            {
                usuario.Nombre = vm.Nombre;
                usuario.NumTel = vm.NumTel;
                usuario.IdRol = vm.IdRol;

                usuario.Correo = vm.Correo;

                if (!string.IsNullOrEmpty(vm.Contraseña))
                    usuario.Contraseña = vm.Contraseña;

                if (correoCambio)
                {
                    string nuevaPass = GenerarPasswordAleatorio();
                    usuario.Contraseña = nuevaPass;

                    await _emailService.EnviarEmailAsync(
                        vm.Correo,
                        "Tu correo ha sido actualizado",
                        $"Hola {usuario.Nombre},<br><br>Tu correo electrónico fue actualizado.<br>" +
                        $"Tu nueva contraseña es: <b>{nuevaPass}</b><br>" +
                        $"Para restablecer tu contraseña debes acudir con el Administrador."
                    );
                }

                _context.Update(usuario);
                await _context.SaveChangesAsync();

                if (usuarioActualId == usuario.Id && (correoCambio || cambioRol))
                {
                    await HttpContext.SignOutAsync("Cookies");

                    return Json(new
                    {
                        success = true,
                        logout = true,
                        message = "Tu información fue actualizada. Por seguridad debes iniciar sesión nuevamente."
                    });
                }

                return Json(new { success = true, message = "Usuario editado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al editar usuario: " + ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> EliminarUsuario(int id, int? reemplazoId)
        {
            var usuarioActualId = int.Parse(User.FindFirst("Id").Value);

            if (usuarioActualId == id)
            {
                return BadRequest("No puedes eliminar tu propio usuario.");
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Tallers)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound("El usuario no existe.");

            if (usuario.IdRol == 2)
            {
                var talleres = _context.Tallers.Where(t => t.IdUsuario == id).ToList();

                foreach (var t in talleres)
                {
                    if (reemplazoId.HasValue)
                        t.IdUsuario = reemplazoId.Value;
                    else
                        t.IdUsuario = null;
                }

                await _context.SaveChangesAsync();
            }

            usuario.Estado = 0;
            await _context.SaveChangesAsync();

            return Ok("Usuario eliminado correctamente.");
        }


        private async Task ReautenticarUsuario(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim("Id", usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.IdRolNavigation.Rol1)
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal);
        }
        [HttpPost]
        public async Task<IActionResult> ReactivarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return Json(new { success = false, message = "El usuario no existe." });

            usuario.Estado = 1;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Usuario reactivado correctamente." });
        }


        private string GenerarPasswordAleatorio(int length = 12)
        {
            const string caracteresValidos = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            StringBuilder res = new StringBuilder();
            using (var rng = new RNGCryptoServiceProvider())
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
