using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Models;
using ProyectoRegistros.Models.ViewModels;
using System.Linq;
using System.Security.Claims;
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
        public IActionResult TalleresDisponibles()
        {
            var talleres = Context.Tallers
                .Include(t => t.IdUsuarioNavigation)
                .Select(t => new TalleresViewModels
                {
                    Nombre = t.Nombre,
                    Dias = t.Dias,
                    Espacios = t.LugaresDisp,
                    Horario = t.HoraInicio.ToString(@"hh\:mm tt") + " - " + t.HoraFinal.ToString(@"hh\:mm tt"),
                    Edad = t.EdadMax.HasValue
                           ? $"{t.EdadMin} a {t.EdadMax.Value} años"
                           : $"{t.EdadMin} en adelante",
                    Profesor = t.IdUsuarioNavigation.Nombre,
                    Costo = t.Costo
                })
                .ToList();

            return View(talleres);
        }


        [HttpPost]
        public async Task<IActionResult> Index(string correo, string password)
        {
            var usuario = Context.Usuarios.SingleOrDefault(u => u.Correo == correo && u.Contraseña == password);

            if (usuario != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim("Id", usuario.Id.ToString())
                };


                string rol;
                if (usuario.IdRol == 1)
                {
                    rol = "Administrador";
                }
                else if (usuario.IdRol == 2)
                {
                    rol = "Profesor";
                }
                else
                {
                    rol = "Visitante";
                }

                claims.Add(new Claim(ClaimTypes.Role, rol));

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                // Redirigir al usuario según su rol
                if (usuario.IdRol == 1)
                {
                    // Redirigir al área de Administrador
                    return RedirectToAction("Index", "Admin" , new {area = "Admin"});
                }
                else if (usuario.IdRol == 2)
                {
                    // Redirigir al área de Profesor
                    return RedirectToAction("Index", "Profe", new { area = "Profe" });
                }
                else // IdRol 3
                {
                    // Redirigir al área de Visitante
                    return RedirectToAction("Index", "Visitante", new { area = "Visitante" });
                }
            }

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
