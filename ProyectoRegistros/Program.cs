var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapAreaControllerRoute(
    name: "Areas",
    areaName: "Admin",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}"
    );

app.MapAreaControllerRoute(
    name: "Areas",
    areaName: "Profe",
    pattern: "{area:exists}/{controller=Profe}/{action=Index}/{id?}"
    );

app.MapAreaControllerRoute(
    name: "Areas",
    areaName: "Visitante",
    pattern: "{area:exists}/{controller=Visitante}/{action=Index}/{id?}"
);


app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();

