using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using AppSoftDoc.Data;
using AppSoftDoc.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace AppSoftDoc.Controllers
    {
    [Authorize(Roles = "Cliente")] // Solo los usuarios con el rol "Cliente" pueden acceder
    public class ClienteController : Controller
        {
        private readonly AppDBContext _context;

        public ClienteController(AppDBContext context)
            {
            _context = context; // Inicializa el contexto de base de datos
            }

        // Evitar el uso de caché para páginas sensibles
        public override void OnActionExecuting(ActionExecutingContext context)
            {
            Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, proxy-revalidate");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");
            base.OnActionExecuting(context);
            }

        // Método para mostrar los documentos del usuario logueado
        [HttpGet]
        public async Task<IActionResult> MisDocumentos()
            {
            // Obtener el Id del usuario logueado
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioId))
                {
                return Unauthorized(); // Usuario no autenticado
                }

            // Convertir el Id del usuario a entero
            if (!int.TryParse(usuarioId, out int idUsuario))
                {
                return Unauthorized(); // Error en la conversión del ID
                }

            // Filtrar los archivos relacionados con el usuario logueado
            var archivos = await _context.Archivos
                .Where(a => a.IdUsuarios == idUsuario) // Filtra solo por el usuario logueado
                .ToListAsync();

            if (archivos == null || !archivos.Any())
                {
                ViewBag.Mensaje = "No tienes documentos asociados.";
                }

            return View(archivos); // Pasar los archivos a la vista
            }

        // Método para descargar el archivo
        [HttpGet]
        public async Task<IActionResult> DescargarArchivo(int id)
            {
            var archivo = await _context.Archivos
                .FirstOrDefaultAsync(a => a.IdArchivo == id);

            if (archivo == null)
                {
                return NotFound(); // Archivo no encontrado
                }

            return File(archivo.ArchivoData, "application/octet-stream", archivo.Nombre);
            }

        // Página de inicio o índice
        public IActionResult Index()
            {
            return View();
            }
        }
    }
