using AppSoftDoc.Data;
using AppSoftDoc.Models;
using AppSoftDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace AppSoftDoc.Controllers
    {
    public class AccesoController : Controller
        {

        /*En este controlador se utiliza el metodo de Login, Registrar y Cerrar Sesion*/

        private readonly AppDBContext _appDbContext;
        //utilizamos nuestra Clase AppDBContext
        public AccesoController(AppDBContext appDBContext)
            {
            _appDbContext = appDBContext;

            }
       
        // Acción para el login (GET)
        [HttpGet]
        public IActionResult Login(string? returnUrl)
            {
            return View();
            }

        // Acción para el login (POST)
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM modelo, string? returnUrl)
            {
            if (ModelState.IsValid)
                {
                // Obtener el usuario desde la base de datos
                var usuario = await _appDbContext.Usuarios
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Correo == modelo.Correo);

                if (usuario == null || usuario.Clave != modelo.Clave)
                    {
                    ViewData["Mensaje"] = "Credenciales incorrectas";
                    return View(modelo);
                    }

                // Crear los claims para la autenticación
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuarios.ToString()),
                    new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                    new Claim(ClaimTypes.Email, usuario.Correo),
                    new Claim(ClaimTypes.Role, usuario.Role.Descripcion)
                };


                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                    {
                    IsPersistent = false, // No hacer persistente la cookie
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(15)
                    };

                // Iniciar sesión con las cookies
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                // Si existe ReturnUrl, redirigir a esa página, si no, redirigir según el rol del usuario
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                    return Redirect(returnUrl); // Redirige a la URL original que el usuario quería visitar
                    }
                else
                    {
                    // Redirige según el rol del usuario
                    switch (usuario.Role.Descripcion)
                        {
                        case "Administrador":
                            return RedirectToAction("Index", "Admin");
                        case "Programador":
                            return RedirectToAction("Index", "Programador");
                        case "Cliente":
                            return RedirectToAction("Index", "Cliente");
                        default:
                            ViewData["Mensaje"] = "Rol desconocido";
                            return View(modelo);
                        }
                    }
                }

            // Si el modelo no es válido, regresar la vista con el error
            return View(modelo);
            }

        // Acción para registrarse (GET)
        [HttpGet]
        public IActionResult Registrarse()
            {
            return View();
            }

        // Acción para registrarse (POST)
        [HttpPost]
        public async Task<IActionResult> Registrarse(UsuarioVM modelo)
            {
            if (modelo.Clave != modelo.ConfirmarClave)
                {
                ViewData["Mensaje"] = "Las Contraseñas no coinciden";
                return View();
                }

            // Crear el usuario
            Usuario usuario = new Usuario()
                {
                NombreCompleto = modelo.NombreCompleto,
                Correo = modelo.Correo,
                Clave = modelo.Clave,
                Idrol = 3 //todo USUARIO  que se registre por default sera ID3 que es cliente
                };

            await _appDbContext.Usuarios.AddAsync(usuario);
            await _appDbContext.SaveChangesAsync();

            if (usuario.IdUsuarios != 0)
                return RedirectToAction("Login", "Acceso");

            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View();
            }
        [HttpGet]
        public IActionResult KeepAlive()
            {
            // Esta acción solo se utiliza para mantener viva la sesión. 
            // No hace falta hacer nada especial si estás utilizando SlidingExpiration.
            return Ok();  // Retorna un 200 OK como respuesta a la petición AJAX
            }
        // Acción para cerrarse sesión
        public async Task<IActionResult> Logout()
            {
            // Llamada para cerrar sesión y eliminar la cookie de autenticación
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirige al usuario a la página de inicio
            return RedirectToAction("Login", "Acceso");
            }
        }
    }
