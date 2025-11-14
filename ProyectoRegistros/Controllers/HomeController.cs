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
        [HttpGet]
        public async Task<IActionResult> TalleresDisponibles(int? edad)
        {
            IQueryable<Taller> query = Context.Tallers.AsQueryable();

            query = query.Where(t => t.Estado == 1);

            if (edad.HasValue)
            {
                int e = edad.Value;

                query = query
                    .Where(t => e >= t.EdadMin && (t.EdadMax == null || e <= t.EdadMax))
                    .OrderBy(t => Math.Abs(e - t.EdadMin))
                    .ThenBy(t => t.EdadMin)
                    .ThenBy(t => t.Nombre);
            }
            else
            {
                query = query.OrderBy(t => t.EdadMin).ThenBy(t => t.Nombre);
            }


            var talleres = await query
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
                .ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_TalleresTabla", talleres);
            }

            return View(talleres);
        }
        [HttpPost]
        public async Task<IActionResult> Index(string correo, string password)
        {
            var usuario = Context.Usuarios.SingleOrDefault(u => u.Correo == correo && u.Contraseña == password && u.Estado == 1);
            if (usuario != null)
            {

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
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim("Id", usuario.Id.ToString()),
                    new Claim("Rol", rol),
                    new Claim("PrimerNombre", usuario.Nombre.Split(' ')[0])
                };


                claims.Add(new Claim(ClaimTypes.Role, rol));

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                if (usuario.IdRol == 1)
                {
                    return RedirectToAction("Index", "Admin" , new {area = "Admin"});
                }
                else if (usuario.IdRol == 2)
                {
                    return RedirectToAction("Index", "Profe", new { area = "Profe" });
                }
                else
                {
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
