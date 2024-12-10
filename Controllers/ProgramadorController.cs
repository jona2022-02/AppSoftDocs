using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using AppSoftDoc.Data;
using AppSoftDoc.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace AppSoftDoc.Controllers
    {
    [Authorize]
    public class ProgramadorController : Controller
        {
        private readonly AppDBContext _context;

        // Constructor
        public ProgramadorController(AppDBContext context)
            {
            _context = context;
            }

        public override void OnActionExecuting(ActionExecutingContext context)
            {
            // Agregar los encabezados para deshabilitar la caché para que al momento de cerrar sesión no pueda regresar a la pagina anterior en las flechas que aparece en el navegador
            Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, proxy-revalidate");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");

            base.OnActionExecuting(context); // No olvidar llamar al método base
            }

        // Acción para la página principal del programador
        public IActionResult Index()
            {
            return View();
            }

        // Acción para ver documentos solo del administrador (Id=1)
        public async Task<IActionResult> VerDocumentos()
            {
            // Obtener los archivos asociados a usuarios con rol 1 (Administrador)
            var documentos = await _context.Archivos
                .Include(a => a.Usuario)  // Incluir la relación con el usuario asociado al archivo
                .Where(a => a.Usuario != null && a.Usuario.Idrol == 1) // Asegurarse de que Usuario no sea nulo y que tenga el rol 1 (Administrador)
                .ToListAsync();  // Ejecutar la consulta y convertir a lista

            // Verificar en los logs cuántos documentos se obtienen
            Console.WriteLine($"Documentos encontrados: {documentos.Count}");

            // Si no hay documentos, podemos mostrar un mensaje en consola o en el usuario
            if (documentos.Count == 0)
                {
                // Si no se encuentran documentos asociados a administradores
                Console.WriteLine("No se encontraron documentos asociados a administradores.");
                }

            return View(documentos); // Pasar los documentos filtrados a la vista
            }


        // Acción para mostrar la vista de carga de documentos (solo si el rol es Programador)
        [HttpGet]
        public IActionResult SubirDocumento()
            {
            // Solo el programador puede acceder a esta vista, ya que no estamos verificando el rol aquí
            return View();
            }

        // Acción POST para subir un documento (el archivo se asigna al Administrador, IdUsuarios = 1)
        // Acción POST para subir un documento
        [HttpPost]
        public async Task<IActionResult> SubirDocumento(IFormFile archivo)
            {
            if (archivo != null && archivo.Length > 0)
                {
                // Leer el archivo y guardarlo como byte[]
                using (var memoryStream = new MemoryStream())
                    {
                    await archivo.CopyToAsync(memoryStream);
                    var archivoData = memoryStream.ToArray();

                    // Buscar un usuario con el rol de Administrador (Idrol = 1)
                    var administrador = await _context.Usuarios
                        .FirstOrDefaultAsync(u => u.Idrol == 1); // Buscar el primer usuario con rol 1 (Administrador)

                    if (administrador == null)
                        {
                        // Si no existe un usuario con el rol de administrador, retornar un error
                        return NotFound("No se encontró un administrador para asignar el archivo.");
                        }

                    // Crear un nuevo objeto Archivo, asociándolo al usuario administrador
                    var nuevoArchivo = new Archivo
                        {
                        Nombre = archivo.FileName,
                        Extension = Path.GetExtension(archivo.FileName),
                        ArchivoData = archivoData,
                        IdUsuarios = administrador.IdUsuarios // Asignamos el archivo al administrador encontrado
                        };

                    // Guardar el archivo en la base de datos
                    _context.Archivos.Add(nuevoArchivo);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(VerDocumentos)); // Redirigir al listado de documentos
                    }
                }

            return BadRequest("No se seleccionó ningún archivo.");
            }


        // Acción para descargar un archivo específico (solo los archivos del administrador)
        [HttpGet]
        public async Task<IActionResult> DescargarArchivo(int id)
            {
            var archivo = await _context.Archivos
                .FirstOrDefaultAsync(a => a.IdArchivo == id);  // Obtener el archivo por su ID

            if (archivo == null)
                {
                return NotFound(); // Si el archivo no existe, retornar NotFound
                }

            // Retornar el archivo como una descarga
            return File(archivo.ArchivoData, "application/octet-stream", archivo.Nombre);
            }

        // Acción para eliminar un archivo (solo los archivos del Administrador)
        [HttpPost, ActionName("EliminarArchivo")]
     
        public async Task<IActionResult> EliminarArchivo(int id)
            {
            var archivo = await _context.Archivos
                .FirstOrDefaultAsync(a => a.IdArchivo == id && a.Usuario.IdUsuarios == 1); // Verificar que el archivo pertenece al Administrador

            if (archivo == null)
                {
                return NotFound(); // Si no se encuentra el archivo o no es del administrador, devolver NotFound
                }

            _context.Archivos.Remove(archivo); // Eliminar el archivo
            await _context.SaveChangesAsync(); // Guardar cambios

            return RedirectToAction(nameof(VerDocumentos)); // Redirigir al listado de documentos
            }
        }
    }
