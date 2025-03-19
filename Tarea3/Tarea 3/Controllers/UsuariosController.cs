using Microsoft.AspNetCore.Mvc;
using Tarea_3.Data;
using Tarea_3.Models;
using System.Linq;

namespace Tarea_3.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/usuarios
        [HttpGet]
        public IActionResult GetUsuarios()
        {
            var usuarios = _context.Usuarios.ToList();
            return Ok(usuarios);
        }
    }
}
