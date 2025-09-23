using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ProyectoRegistros.Models;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoRegistros.Controllers
{
    public class HomeController:Controller
    {
        //Debe de ir lo de la autenticacion del correo electronico y contraseña, verificar que existan
        public ProyectoregistroContext Context { get; set; }

        public HomeController(ProyectoregistroContext context)
        {
            Context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string correo, string password)
        {
            // Buscar al usuario en la base de datos por correo y contraseña
            var usuario = Context.Usuarios.SingleOrDefault(u => u.Correo == correo && u.Contraseña == password);

            // Verificar si el usuario fue encontrado
            if (usuario != null)
            {
                // Crear una lista de Claims (datos del usuario para la sesión)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim("Id", usuario.Id.ToString())
                };

                // Asignar el rol según IdRol
                string rol;
                if (usuario.IdRol == 1)
                {
                    rol = "Administrador";
                }
                else if (usuario.IdRol == 2)
                {
                    rol = "Profesor";
                }
                else // IdRol 3
                {
                    rol = "Visitante";
                }

                claims.Add(new Claim(ClaimTypes.Role, rol));

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Iniciar la sesión de autenticación
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redirigir al usuario según su rol
                if (usuario.IdRol == 1)
                {
                    // Redirigir al área de Administrador
                    return RedirectToAction("Index", "Admin");
                }
                else if (usuario.IdRol == 2)
                {
                    // Redirigir al área de Profesor
                    return RedirectToAction("Index", "Profesor");
                }
                else // IdRol 3
                {
                    // Redirigir al área de Visitante
                    return RedirectToAction("Index", "Visitante");
                }
            }

            // Si el usuario no fue encontrado, mostrar un error
            ModelState.AddModelError("", "Correo o contraseña incorrectos.");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
