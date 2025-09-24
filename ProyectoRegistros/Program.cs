using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using ProyectoRegistros.Models;
using MySql.EntityFrameworkCore; // Importa el proveedor de MySQL

var builder = WebApplication.CreateBuilder(args);

// **SOLUCIÓN: Agregar el contexto de la base de datos para MySQL**
// El método correcto es UseMySQL
builder.Services.AddDbContext<ProyectoregistroContext>(options =>
    options.UseMySql("server=localhost;database=proyectoregistro;user=root;password=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.43-mysql")));

// Configuración de la autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/";
    });

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "AdminArea",
    areaName: "Admin",
    pattern: "Admin/{controller=Admin}/{action=Index}/{id?}"
);

app.MapAreaControllerRoute(
    name: "ProfeArea",
    areaName: "Profe",
    pattern: "Profe/{controller=Profe}/{action=Index}/{id?}"
);

app.MapAreaControllerRoute(
    name: "VisitanteArea",
    areaName: "Visitante",
    pattern: "Visitante/{controller=Visitante}/{action=Index}/{id?}"
);

app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();
