﻿using System.ComponentModel.DataAnnotations;

namespace AppSoftDoc.ViewModels
    {
    public class NuevoUsuarioVM
        {
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre completo no puede exceder los 100 caracteres.")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "La contraseña debe tener entre 3 y 50 caracteres.")]
        public string Clave { get; set; }

        [Required(ErrorMessage = "Confirmar la contraseña es obligatorio.")]
        [DataType(DataType.Password)]
        [Compare("Clave", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarClave { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public int Idrol { get; set; } // Id del rol seleccionado
        }
    }
