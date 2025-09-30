using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MySql.EntityFrameworkCore;
using ProyectoRegistros.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProyectoregistroContext>(options =>
    options.UseMySql("server=localhost;database=proyectoregistro;user=root;password=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.43-mysql")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/";
    });

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSession();

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
app.UseSession();
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
