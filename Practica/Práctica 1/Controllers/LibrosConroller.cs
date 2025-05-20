using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Práctica_1.Data;
using Práctica_1.Models;

namespace Práctica_1.Controllers
{
    public class LibrosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LibrosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var libros = await _context.Libros.ToListAsync();
            return View(libros);
        }

        // GET: Libros/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Libros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Libro libro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(libro);
                await _context.SaveChangesAsync();
                return RedirectToAction("Books", "Admin");
            }
            return View(libro);
        }

        // GET: Libros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var libro = await _context.Libros.FindAsync(id);
            if (libro == null) return NotFound();

            return View(libro);
        }

        // POST: Libros/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Libro libro)
        {
            if (id != libro.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(libro);
                await _context.SaveChangesAsync();
                return RedirectToAction("Books", "Admin");
            }
            return View(libro);
        }

        // GET: Libros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var libro = await _context.Libros.FirstOrDefaultAsync(m => m.Id == id);
            if (libro == null) return NotFound();

            return View(libro);
        }

        // POST: Libros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();
            return RedirectToAction("Books", "Admin");
        }
        public async Task<IActionResult> Detalles(int id)
        {
            var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
            if (libro == null)
                return NotFound();

            var userId = HttpContext.Session.GetString("UsuarioId");
            bool enFavoritos = false;

            if (!string.IsNullOrEmpty(userId))
            {
                int usuarioId = int.Parse(userId);
                enFavoritos = await _context.Favoritos
                    .AnyAsync(f => f.UsuarioId == usuarioId && f.LibroId == id);
            }

            ViewBag.EnFavoritos = enFavoritos;
            ViewBag.UsuarioId = userId;

            return View(libro);
        }


    }

}
