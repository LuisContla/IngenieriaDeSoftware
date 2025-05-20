using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Práctica_1.Models
{
    [Table("Favoritos")]
    public class Favorito
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int LibroId { get; set; }

        // Relaciones de navegación
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [ForeignKey("LibroId")]
        public Libro Libro { get; set; }
    }
}
