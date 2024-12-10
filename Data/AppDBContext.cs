using Microsoft.EntityFrameworkCore;
using AppSoftDoc.Models;

namespace AppSoftDoc.Data
    {
    public class AppDBContext : DbContext
        {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        // DbSets para las tablas
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Archivo> Archivos { get; set; }

        // Configuración adicional en el modelo, si es necesario
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
            base.OnModelCreating(modelBuilder);

            // Configuración de la tabla Roles
            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasKey(r => r.Idrol); // Clave primaria
                entity.Property(r => r.Descripcion)
                      .HasMaxLength(50)
                      .IsRequired();  // Configuración de la columna Descripcion
            });

            // Configuración de la tabla Usuarios
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.IdUsuarios); // Clave primaria
                entity.Property(u => u.NombreCompleto)
                      .HasMaxLength(100)
                      .IsRequired(); // Nombre del usuario
                entity.Property(u => u.Correo)
                      .HasMaxLength(100)
                      .IsRequired()
                      .IsUnicode(); // Correo del usuario
                entity.Property(u => u.Clave)
                      .HasMaxLength(50)
                      .IsRequired(); // Contraseña del usuario

                // Relación con Roles
                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(u => u.Idrol)
                      .OnDelete(DeleteBehavior.Restrict); // Restrict cuando se elimina un rol
            });

            // Configuración de la tabla Archivos
            modelBuilder.Entity<Archivo>(entity =>
            {
                entity.HasKey(f => f.IdArchivo); // Clave primaria
                entity.Property(f => f.Nombre)
                      .HasMaxLength(100); // Nombre del archivo
                entity.Property(f => f.Extension)
                      .HasMaxLength(50); // Extensión del archivo

                entity.Property(f => f.ArchivoData)
                      .HasColumnName("Archivo")  // Nombre en la base de datos
                      .HasColumnType("varbinary(max)");  // Tipo de dato varbinary para almacenar el archivo

                // Relación con Usuario
                entity.HasOne(f => f.Usuario)
                      .WithMany(u => u.Archivos)
                      .HasForeignKey(f => f.IdUsuarios)
                      .OnDelete(DeleteBehavior.Cascade); // Eliminar los archivos cuando el usuario se elimina
            });
            }
        }
    }
