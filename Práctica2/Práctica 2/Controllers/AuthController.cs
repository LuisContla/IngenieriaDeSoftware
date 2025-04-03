using Microsoft.AspNetCore.Mvc;
using Práctica_2.Models;
using Práctica_2.Data;

namespace Práctica_2.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = _context.Usuarios.FirstOrDefault(u => u.Nombre == model.Usuario);

                if (usuario != null)
                {
                    // Validar la contraseña
                    if (usuario.Contraseña == model.Contraseña) // En producción, usa hashing
                    {
                        // Iniciar sesión
                        HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());

                        if (usuario.Rol == 1) // Administrador
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Contraseña incorrecta."; // Establece un mensaje de error
                    }
                }
                else
                {
                    ViewBag.Error = "El usuario no existe."; // Establece un mensaje si el usuario no se encuentra
                }
            }
            else
            {
                ViewBag.Error = "Por favor, verifica los datos ingresados."; // Establece un mensaje si el modelo no es válido
            }

            return View(model); // Regresa la vista con el mensaje de error
        }

        // GET: Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuarioExistente = _context.Usuarios.FirstOrDefault(u => u.Nombre == model.Usuario || u.Correo == model.Correo);

                if (usuarioExistente != null)
                {
                    // Si ya existe el usuario o correo, asignamos el mensaje de error
                    ViewBag.Error = "El usuario o el correo ya están registrados.";
                    return View(model);
                }

                // Crear un nuevo usuario
                var nuevoUsuario = new Usuario
                {
                    Nombre = model.Usuario,
                    Correo = model.Correo,
                    Contraseña = model.Contraseña, // En producción, usa hashing
                    Rol = 0 // Asumimos que el rol por defecto es normal
                };

                _context.Usuarios.Add(nuevoUsuario);
                _context.SaveChanges();

                // Si la cuenta se crea exitosamente, asignamos el mensaje de éxito
                ViewBag.SuccessMessage = "Cuenta creada exitosamente.";
                return RedirectToAction("Login"); // Redirigimos al login después de la creación
            }

            // Si el modelo no es válido, regresamos con los errores.
            return View(model);
        }

        // Logout
        [HttpPost]
        public IActionResult Logout()
        {
            // Elimina la sesión del usuario
            HttpContext.Session.Clear();

            // Redirige al Login después de cerrar sesión
            return RedirectToAction("Login", "Auth");
        }

    }
}
