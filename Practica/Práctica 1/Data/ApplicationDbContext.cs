using Microsoft.EntityFrameworkCore;
using Práctica_1.Models;

namespace Práctica_1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Favorito> Favoritos { get; set; }

        public DbSet<Libro> Libros { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
