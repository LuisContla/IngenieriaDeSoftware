using Microsoft.AspNetCore.Mvc;
using Práctica_1.Data;

namespace Práctica_1.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetString("UsuarioId");

        if (string.IsNullOrEmpty(userId))
        {
            // Si no hay un usuario logueado, redirige al login
            return RedirectToAction("Login", "Auth");
        }

        // Buscar al usuario en la base de datos
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Id.ToString() == userId);

        if (usuario == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        // Pasar el nombre del usuario a la vista
        ViewBag.UserName = usuario.Nombre;

        return View(); // Regresa la vista principal
    }

    public IActionResult Books()
    {
        var userId = HttpContext.Session.GetString("UsuarioId");

        if (string.IsNullOrEmpty(userId))
        {
            // Si no hay un usuario logueado, redirige al login
            return RedirectToAction("Login", "Auth");
        }

        return View();
    }
}
