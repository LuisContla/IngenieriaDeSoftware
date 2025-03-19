using Microsoft.AspNetCore.Mvc;
using Tarea_3.Data;

namespace Tarea_3.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
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
    }
}
