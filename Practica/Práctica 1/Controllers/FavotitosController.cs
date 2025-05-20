using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Práctica_1.Data;
using Práctica_1.Models;

namespace Práctica_1.Controllers
{
    public class FavoritosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FavoritosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(int libroId, string returnUrl = null)
        {
            var userId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            int usuarioId = int.Parse(userId);

            bool yaExiste = await _context.Favoritos
                .AnyAsync(f => f.UsuarioId == usuarioId && f.LibroId == libroId);

            if (!yaExiste)
            {
                _context.Favoritos.Add(new Favorito
                {
                    UsuarioId = usuarioId,
                    LibroId = libroId
                });
                await _context.SaveChangesAsync();
            }

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            // Fallback
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario != null && usuario.Rol == 1)
                return RedirectToAction("Books", "Admin");
            else
                return RedirectToAction("Books", "Home");
        }

        public async Task<IActionResult> MisFavoritos()
        {
            var userId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            int usuarioId = int.Parse(userId);

            var favoritos = await _context.Favoritos
                .Where(f => f.UsuarioId == usuarioId)
                .Include(f => f.Libro)
                .ToListAsync();

            return View(favoritos);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            var favorito = await _context.Favoritos.FindAsync(id);
            if (favorito != null)
            {
                _context.Favoritos.Remove(favorito);
                await _context.SaveChangesAsync();
            }

            // Redireccionar de nuevo al perfil según el rol del usuario
            var userId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var usuario = await _context.Usuarios.FindAsync(int.Parse(userId));
            if (usuario != null && usuario.Rol == 1)
                return RedirectToAction("Perfil", "Admin");
            else
                return RedirectToAction("Perfil", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> EliminarPorLibro(int libroId, string returnUrl = null)
        {
            var userId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            int usuarioId = int.Parse(userId);

            var favorito = await _context.Favoritos
                .FirstOrDefaultAsync(f => f.UsuarioId == usuarioId && f.LibroId == libroId);

            if (favorito != null)
            {
                _context.Favoritos.Remove(favorito);
                await _context.SaveChangesAsync();
            }

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            // Fallback según rol
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario != null && usuario.Rol == 1)
                return RedirectToAction("Books", "Admin");
            else
                return RedirectToAction("Books", "Home");
        }



    }
}
