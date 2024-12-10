using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using AppSoftDoc.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

// Configuraci�n de la cadena de conexi�n y DbContext
builder.Services.AddDbContext<AppDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL"));
});

// Agregar autenticaci�n con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Login";         // Ruta para el login
        options.LogoutPath = "/Acceso/Logout";       // Ruta para el logout
        options.ExpireTimeSpan = TimeSpan.FromMinutes(2);  // Tiempo de expiraci�n de la cookie (2 minutos)
        options.SlidingExpiration = true;  // La cookie se extiende si el usuario sigue activo
        options.Cookie.HttpOnly = true;   // Asegura que la cookie solo sea accesible por HTTP
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;  // Usar cookies seguras (dependiendo de tu entorno)
    });

// Agregar autorizaci�n (si necesitas roles espec�ficos)
builder.Services.AddAuthorization();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
    {
    app.UseExceptionHandler("/Home/Error");
    }

app.UseStaticFiles();
app.UseRouting();

// Middleware de autenticaci�n y autorizaci�n
app.UseAuthentication();
app.UseAuthorization();

// Configuraci�n de rutas de controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
