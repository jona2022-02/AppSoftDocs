function confirmarEliminacion(usuarioId) {
    // Mostrar el SweetAlert de confirmación
    Swal.fire({
        title: '¿Estás seguro?',
        text: "¡No podrás revertir esto!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            // Si el usuario confirma, hacer la petición POST para eliminar el usuario
            $.ajax({
                url: '/Admin/EliminarUsuarioConfirmado/' + usuarioId, // La URL para la acción de eliminación
                type: 'POST', // Asegurarse de que sea un POST
                success: function () {
                    Swal.fire(
                        '¡Eliminado!',
                        'El usuario ha sido eliminado exitosamente.',
                        'success'
                    ).then(() => {
                        // Redirigir a la lista de usuarios después de la eliminación
                        window.location.href = '/Admin/ListadodeUsuarios'; // Redirigir al listado de usuarios
                    });
                },
                error: function () {
                    Swal.fire(
                        'Error',
                        'Hubo un problema al eliminar el usuario.',
                        'error'
                    );
                }
            });
        } else {
            // Si cancela, no hace nada, solo muestra el mensaje
            Swal.fire(
                'Cancelado',
                'El usuario no ha sido eliminado.',
                'info'
            );
        }
    });
}
