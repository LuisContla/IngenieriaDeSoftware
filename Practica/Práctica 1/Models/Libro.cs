using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Práctica_1.Models
{
    [Table("Libros")]
    public class Libro
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; }

        [Required]
        [MaxLength(150)]
        public string Autor { get; set; }

        [MaxLength(500)]
        public string Descripcion { get; set; }

        [MaxLength(255)]
        public string ImagenUrl { get; set; }
    }
}
