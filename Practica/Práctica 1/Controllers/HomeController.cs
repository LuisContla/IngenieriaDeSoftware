using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Práctica_1.Data;
using Práctica_1.Models;

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

    public async Task<IActionResult> Books(string search, int page = 1, int pageSize = 14)
    {
        var userId = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Auth");

        int usuarioId = int.Parse(userId);

        // Consulta base
        var query = _context.Libros.AsQueryable();

        // Aplicar filtro de búsqueda
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(l =>
                l.Titulo.Contains(search) ||
                l.Autor.Contains(search) ||
                l.Descripcion.Contains(search));
        }

        int totalLibros = await query.CountAsync();

        var libros = await query
            .OrderBy(l => l.Titulo)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Libros favoritos del usuario
        var favoritos = await _context.Favoritos
            .Where(f => f.UsuarioId == usuarioId)
            .Select(f => f.LibroId)
            .ToListAsync();

        ViewBag.Favoritos = favoritos;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalLibros / pageSize);
        ViewBag.Search = search;

        return View(libros);
    }



    public async Task<IActionResult> Perfil()
    {
        var userId = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Auth");

        int usuarioId = int.Parse(userId);

        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null || usuario.Rol != 0)
            return RedirectToAction("Index");

        var favoritos = await _context.Favoritos
            .Where(f => f.UsuarioId == usuarioId)
            .Include(f => f.Libro)
            .ToListAsync();

        var viewModel = new PerfilViewModel
        {
            Usuario = usuario,
            Favoritos = favoritos
        };

        return View("Perfil", viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> EditarPerfil()
    {
        var userId = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Auth");

        var usuario = await _context.Usuarios.FindAsync(int.Parse(userId));
        if (usuario == null)
            return NotFound();

        return View(usuario); // Usa una vista llamada EditarPerfil.cshtml
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarPerfil(Usuario modelo)
    {
        var sessionId = HttpContext.Session.GetString("UsuarioId");
        if (sessionId == null || modelo.Id.ToString() != sessionId)
            return Unauthorized();

        var usuario = await _context.Usuarios.FindAsync(modelo.Id);
        if (usuario == null)
            return NotFound();

        usuario.Nombre = modelo.Nombre;
        usuario.Correo = modelo.Correo;

        if (!string.IsNullOrWhiteSpace(modelo.Contraseña))
        {
            usuario.Contraseña = modelo.Contraseña; // Opcional: encriptar si fuera necesario
        }

        await _context.SaveChangesAsync();

        if (usuario.Rol == 1)
            return RedirectToAction("Perfil", "Admin");
        else
            return RedirectToAction("Perfil", "Home");
    }


}
