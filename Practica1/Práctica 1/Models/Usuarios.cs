using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Práctica_1.Models
{
    [Table("Usuarios")] // Define el nombre de la tabla en la BD
    public class Usuario
    {
        [Key] // Clave primaria
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(150)]
        public string Correo { get; set; }

        [Required]
        [MaxLength(255)]
        public string Contraseña { get; set; }

        [Required]
        public int Rol { get; set; } // 0 = Usuario normal, 1 = Admin
    }
}
