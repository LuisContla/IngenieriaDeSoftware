using System.ComponentModel.DataAnnotations;

namespace Tarea_3.Models
{
    public class RegisterViewModel
    {
        public string Usuario { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
    }
}
