namespace AppSoftDoc.Models
    {
    public class Archivo
        {
        public int IdArchivo { get; set; } // Clave primaria
        public string Nombre { get; set; }

        // Mantener el nombre 'ArchivoData' en el modelo
        public byte[] ArchivoData { get; set; } // Datos binarios del archivo

        public string Extension { get; set; } // Extensión del archivo
        public int IdUsuarios { get; set; } // Clave foránea a Usuario
        public Usuario Usuario { get; set; } // Relación con el usuario
        }
    }
