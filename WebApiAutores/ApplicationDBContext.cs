using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores
{
    //ApplicationDBContext: Solo es un nombre al azar que se le dió a la clase.
    //Esta es la clase Central de EF Core por la cuál configuramos las tablas de nuestra base de datos.
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AutorLibro>()
                .HasKey(al => new { al.AutorId, al.LibroId });
        }

        //Dset<Autor>: Crea una tabla con los campos de la clase Autor
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<AutorLibro> AutoresLibros { get; set;}
    }
}
