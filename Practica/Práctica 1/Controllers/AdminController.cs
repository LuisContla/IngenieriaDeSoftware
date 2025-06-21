using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Práctica_1.Models;
using Práctica_1.Data;

namespace Práctica_1.Controllers
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

            // Verificar si el usuario es un administrador
            if (usuario.Rol != 1)  // 1 significa Administrador
            {
                return View("AccessDenied");
                //return RedirectToAction("Index", "Home"); // Si no es admin, redirige a la página principal
            }

            // Pasar el nombre del usuario a la vista
            ViewBag.UserName = usuario.Nombre;

            return View(); // Regresa la vista principal
        }

        // Acción para obtener todos los usuarios y mostrarlos
        public async Task<IActionResult> Users()
        {
            var userId = HttpContext.Session.GetString("UsuarioId");
                

            if (string.IsNullOrEmpty(userId))
            {
                // Si no hay un usuario logueado, redirige al login
                return RedirectToAction("Login", "Auth");
            }

            // Buscar al usuario en la base de datos
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (usuario == null || usuario.Rol != 1)  // Solo permitir a administradores ver los usuarios
            {
                return View("AccessDenied");
                //return RedirectToAction("Index", "Home");
            }

            // Obtener la lista de usuarios directamente desde la base de datos
            var usuarios = await _context.Usuarios.ToListAsync();

            // Pasar los usuarios a la vista
            return View(usuarios);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string nombre, string correo, string contraseña, int rol)
        {
            // Validar si el correo ya está registrado
            var existingUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "El correo ya está registrado.";
                return View("Users", await _context.Usuarios.ToListAsync());
            }

            // Crear un nuevo usuario
            var nuevoUsuario = new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                Contraseña = contraseña,  // Aquí deberías encriptar la contraseña antes de guardarla
                Rol = rol
            };

            // Guardar el usuario en la base de datos
            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // Redirigir de nuevo a la vista de usuarios con la lista actualizada
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(userId))
            {
                // Si no hay un usuario logueado, redirige al login
                return RedirectToAction("Login", "Auth");
            }

            // Buscar al usuario que realiza la acción de eliminación (el administrador actual)
            var usuarioAdmin = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            // Buscar al usuario que se desea eliminar
            var usuarioAEliminar = await _context.Usuarios.FindAsync(id);

            if (usuarioAEliminar == null)
            {
                // Si no se encuentra el usuario a eliminar, redirigir o mostrar un mensaje de error
                return RedirectToAction("Users");
            }

            // Verificar si el usuario es el mismo que está intentando eliminar
            if (usuarioAdmin != null && usuarioAdmin.Id == usuarioAEliminar.Id)
            {
                // Si el admin intenta eliminarse a sí mismo, mostrar un error
                ViewBag.ErrorMessage = "No puedes eliminar tu propia cuenta.";
                var usuarios = await _context.Usuarios.ToListAsync();
                return View("Users", usuarios); // Vuelve a mostrar la vista de usuarios con el mensaje de error
            }

            // Eliminar al usuario
            _context.Usuarios.Remove(usuarioAEliminar);
            await _context.SaveChangesAsync();

            // Redirigir de nuevo a la lista de usuarios
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> EditUser(int id)
        {
            var userId = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Buscar al usuario en la base de datos por el ID
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return RedirectToAction("Users");
            }

            // Devolver la vista parcial con el modelo del usuario
            return PartialView("_EditUser", usuario); // Este método debe devolver la vista parcial correctamente
        }


        [HttpPost]
        public async Task<IActionResult> EditUser(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var usuarioExistente = await _context.Usuarios.FindAsync(usuario.Id);

                if (usuarioExistente != null)
                {
                    usuarioExistente.Nombre = usuario.Nombre;
                    usuarioExistente.Correo = usuario.Correo;
                    usuarioExistente.Contraseña = usuario.Contraseña; // Aquí podrías aplicar encriptación si es necesario
                    usuarioExistente.Rol = usuario.Rol;

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Users");
            }

            return View(usuario); // Si hay errores de validación, regresa a la vista de edición
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
            if (usuario == null || usuario.Rol != 1)
                return View("AccessDenied");

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

            if(usuario.Rol != 1)
                return View("AccessDenied");

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
}

