namespace AppSoftDoc.Models
	{
	public class Roles
		{
        public int Idrol { get; set; }  // Clave primaria
        public string Descripcion { get; set; }  // Descripción del rol (Ej: Administrador, Programador, Cliente)

        // Relación: Un rol puede estar asociado a varios usuarios
        public ICollection<Usuario> Usuarios { get; set; }
        }
	}
