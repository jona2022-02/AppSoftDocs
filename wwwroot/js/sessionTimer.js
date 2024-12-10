document.addEventListener("DOMContentLoaded", function () {
    const sessionDuration = 30 * 1000; // Duración total de la sesión en milisegundos (30 segundos)
    const warningTime = 5 * 1000; // Mostrar alerta los últimos 5 segundos
    let sessionExpireTime = Date.now() + sessionDuration; // Tiempo de expiración inicial de la sesión
    let countdownInterval;
    let swalInstance;

    // Función para reiniciar el temporizador cuando el usuario está activo
    function resetSessionTimer() {
        sessionExpireTime = Date.now() + sessionDuration;
        clearInterval(countdownInterval); // Limpiar cualquier intervalo anterior
        startCountdown(); // Reiniciar la cuenta regresiva
    }

    // Iniciar la cuenta regresiva
    function startCountdown() {
        countdownInterval = setInterval(() => {
            const timeLeft = sessionExpireTime - Date.now();

            if (timeLeft <= warningTime && timeLeft > 0) {
                if (!swalInstance) { // Si no hay ningún alerta activo, muestra uno nuevo
                    // Mostrar SweetAlert con cuenta regresiva
                    swalInstance = Swal.fire({
                        title: 'Tu sesión está por expirar',
                        html: `<p>Quedan <strong id="countdown">${Math.ceil(timeLeft / 1000)}</strong> segundos para que tu sesión expire.</p>`,
                        icon: 'warning',
                        showCancelButton: true, // Mostrar el botón de cancelar
                        confirmButtonText: 'Seguir con la sesión', // Botón para continuar con la sesión
                        cancelButtonText: 'Cerrar sesión', // Botón para cerrar la sesión
                        allowOutsideClick: false, // Deshabilitar clic fuera del modal
                        didOpen: () => {
                            const countdownElement = document.getElementById('countdown');
                            const countdownUpdate = setInterval(() => {
                                const newTimeLeft = sessionExpireTime - Date.now();
                                countdownElement.innerText = Math.ceil(newTimeLeft / 1000);

                                if (newTimeLeft <= 0) {
                                    clearInterval(countdownUpdate);
                                    Swal.close();  // Cerramos el alert solo cuando se haya cumplido el tiempo
                                    window.location.href = '/Acceso/Logout'; // Cerrar sesión cuando la cuenta llegue a cero
                                }
                            }, 1000);
                        }
                    }).then((result) => {
                        if (result.isConfirmed) {
                            // Renovar la sesión sin recargar la página
                            $.get('/Acceso/KeepAlive', () => {
                                // Actualizar el tiempo de expiración sin recargar la página
                                sessionExpireTime = Date.now() + sessionDuration;
                            });
                        } else if (result.isDismissed) {
                            // Detener el intervalo y redirigir a la página de logout
                            clearInterval(countdownInterval);
                            window.location.href = '/Acceso/Logout'; // Cerrar sesión
                        }
                    });
                }
            }

            // Si el tiempo expira, cierra sesión automáticamente
            if (timeLeft <= 0) {
                clearInterval(countdownInterval);
                if (swalInstance) {
                    Swal.close(); // Cerrar el SweetAlert si está abierto
                }
                window.location.href = '/Acceso/Logout'; // Cierre de sesión automático
            }
        }, 1000);
    }

    // Detectar la inactividad del usuario
    document.addEventListener('mousemove', resetSessionTimer);
    document.addEventListener('keypress', resetSessionTimer);
    document.addEventListener('click', resetSessionTimer);
    document.addEventListener('scroll', resetSessionTimer);

    // Iniciar el temporizador de sesión
    startCountdown();
});
