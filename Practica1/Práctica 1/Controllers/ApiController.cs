using Microsoft.AspNetCore.Mvc;
using Práctica_1.Data;

namespace Práctica_1.Controllers
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
