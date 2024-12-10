namespace AppSoftDoc.ViewModels
    {
    public class VerDocumentosVM
        {
        public int UsuarioId { get; set; } // ID del usuario
        public string NombreArchivo { get; set; } // Nombre del archivo
        public string Extension { get; set; } // Extensión del archivo
        public IFormFile ArchivoData { get; set; } // Archivo subido
        public IEnumerable<AppSoftDoc.Models.Archivo> Archivos { get; set; } // Lista de archivos
        }
    }
