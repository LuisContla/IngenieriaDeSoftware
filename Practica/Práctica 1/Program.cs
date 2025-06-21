using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Práctica_1.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de la base de datos con Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()  // Habilitar reintentos en caso de fallos transitorios
    )
);

// Habilitar sesiones
builder.Services.AddDistributedMemoryCache(); // Usa una cach� en memoria para las sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Duraci�n de la sesi�n
    options.Cookie.HttpOnly = true;  // La cookie solo es accesible desde HTTP
    options.Cookie.IsEssential = true;  // Necesario para el funcionamiento de sesiones
});

// Configuraci�n de autenticaci�n con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Redirige si no est� autenticado
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Redirige si no tiene permisos
    });

// Agregar servicios para MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuraci�n de middleware
app.UseStaticFiles();
app.UseRouting();

// Habilitar autenticaci�n, autorizaci�n y sesiones en la aplicaci�n
app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); // Habilita el manejo de sesiones

// Rutas del controlador
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
