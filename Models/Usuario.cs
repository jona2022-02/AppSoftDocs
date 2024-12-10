namespace AppSoftDoc.Models
    {
    public class Usuario
        {
        public int IdUsuarios { get; set; } // Clave primaria
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; } // Contraseña
        public int Idrol { get; set; } // Clave foránea hacia Role

        // Relación con la entidad 'Role'
        public Roles Role { get; set; }  // Relación con el rol (Role)

        // Relación con la tabla Archivos
        public ICollection<Archivo> Archivos { get; set; }
        }
    }
