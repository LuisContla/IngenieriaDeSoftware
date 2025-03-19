using Microsoft.AspNetCore.Mvc;
using Tarea_3.Data;

namespace Tarea_3.Controllers
{
    [Route("api/prueba")]
    [ApiController]
    public class PruebaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PruebaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var usuarios = _context.Usuarios.ToList();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error en la conexión: " + ex.Message);
            }
        }
    }

}
