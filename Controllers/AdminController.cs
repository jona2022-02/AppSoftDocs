using AppSoftDoc.Data;
using AppSoftDoc.Models; // Referencia al modelo Archivo
using AppSoftDoc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AppSoftDoc.Controllers
    {
    [Authorize]
    public class AdminController : Controller
        {
        private readonly AppDBContext _context;

        // Constructor que inicializa el contexto de base de datos
        public AdminController(AppDBContext context)
            {
            _context = context;
            }


        // Método que se ejecuta antes de cualquier acción del controlador para prevenir el cacheo de las páginas
        public override void OnActionExecuting(ActionExecutingContext context)
            {
            Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, proxy-revalidate");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");

            base.OnActionExecuting(context);
            }



         [HttpGet]// Acción para la página principal del administrador
        public IActionResult Index()
            {
            return View();
            }





        // Acción para la mostrar el listado de los Usuarios
        public async Task<IActionResult> ListadodeUsuarios()
            {
            var usuarios = await _context.Usuarios
                .Include(u => u.Role) 
                .ToListAsync();

            return View(usuarios);
            }

        [HttpGet]
        public IActionResult CrearUsuario()
            {
            // Carga los roles desde la base de datos o una fuente estática
            ViewBag.Roles = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Administrador" },
                new SelectListItem { Value = "2", Text = "Programador" },
                new SelectListItem { Value = "3", Text = "Cliente" }
            }, "Value", "Text");

            return View(new NuevoUsuarioVM());
            }

        // Acción para manejar el envío del formulario
        [HttpPost]
        public IActionResult CrearUsuario(NuevoUsuarioVM model)
            {
            if (!ModelState.IsValid)
                {
                // Si hay errores, recargar los roles y volver a mostrar el formulario
                ViewBag.Roles = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "Administrador" },
                    new SelectListItem { Value = "2", Text = "Programador" },
                    new SelectListItem { Value = "3", Text = "Cliente" }
                }, "Value", "Text");
                return View(model);
                }

            // Aquí agregas la lógica para guardar el usuario
            // Por ejemplo: _context.Usuarios.Add(nuevoUsuario); _context.SaveChanges();

            TempData["SuccessMessage"] = "Usuario creado correctamente.";
            return RedirectToAction("ListaUsuarios");
            }




        [HttpGet]
        public IActionResult EditarUsuario(int id)
            {
            var usuario = _context.Usuarios.Include(u => u.Role).FirstOrDefault(u => u.IdUsuarios == id);
            if (usuario == null)
                {
                return NotFound();  // Si no se encuentra el usuario, retornamos un error 404
                }
            return View(usuario);  // Pasamos el modelo con la información del usuario a la vista
            }


        [HttpPost]
       
        public async Task<IActionResult> EditarUsuario(int id, Usuario usuario)
            {
            // Verificar que el ID de la URL y el ID del usuario coinciden
            if (id != usuario.IdUsuarios)
                {
                return NotFound();  // Si no coinciden, devolver un error 404
                }

            if (ModelState.IsValid)
                {
                try
                    {
                    // Actualizar el usuario en el contexto
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();  // Guardar los cambios de forma asincrónica
                    }
                catch (DbUpdateConcurrencyException)
                    {
                    // Si el usuario no existe en la base de datos, devolver un error 404
                    if (!_context.Usuarios.Any(u => u.IdUsuarios == usuario.IdUsuarios))
                        {
                        return NotFound();
                        }
                    else
                        {
                        // Si ocurre otro tipo de error, relanzarlo
                        throw;
                        }
                    }

                return RedirectToAction(nameof(ListadodeUsuarios));  // Redirigir al listado de usuarios
                }

            return View(usuario);  // Si el modelo no es válido, devolver el mismo usuario con los errores
            }







        [HttpPost]
        public IActionResult EliminarUsuarioConfirmado(int id)
            {
            // Buscar al usuario por su ID en la base de datos
            var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuarios == id);

            // Verificar si el usuario no existe
            if (usuario == null)
                {
                // Si el usuario no existe, devolver NotFound (error 404)
                return NotFound();
                }

            // Verificar si la entidad realmente fue recuperada
            System.Diagnostics.Debug.WriteLine($"Usuario encontrado: {usuario.NombreCompleto}");

            // Eliminar el usuario de la base de datos
            _context.Usuarios.Remove(usuario);

            try
                {
                // Guardar los cambios en la base de datos para persistir la eliminación
                _context.SaveChanges();
                // Si la eliminación es exitosa, devolvemos una respuesta positiva
                return Json(new { success = true });
                }
            catch (Exception ex)
                {
                // En caso de error, retornar un mensaje con el error
                return Json(new { success = false, message = ex.Message });
                }
            }





        // Acción para consultar los archivos relacionados con el rol de administrador
        public IActionResult Documentacion()
            {
            // Obtener los archivos asociados a usuarios con rol 1 (Administrador)
            var archivos = _context.Archivos
                .Include(a => a.Usuario)  // Incluir la información del usuario asociado al archivo
                .Where(a => a.Usuario != null && a.Usuario.Idrol == 1) // Asegurarse de que Usuario no sea nulo y que tenga el rol 1 (Administrador)
                .ToList();  // Ejecutar la consulta y convertir a lista

            // Verificar en los logs cuántos archivos se obtienen
            Console.WriteLine($"Archivos encontrados: {archivos.Count}");

            if (archivos.Count == 0)
                {
                // Si no hay archivos, se puede mostrar un mensaje en consola o un mensaje al usuario
                Console.WriteLine("No se encontraron archivos asociados a administradores.");
                }

            return View(archivos); // Pasar los archivos filtrados a la vista
            }



        // Método para descargar el archivo
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






        // Método en el controlador para cargar archivos para un usuario específico
        // Método en el controlador para cargar un archivo para un cliente
        [HttpGet]
        public IActionResult CargarArchivoClientes(int id)
            {
            // Obtener el usuario para el cual se cargará el archivo
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
                {
                return NotFound();
                }

            // Pasar el usuario a la vista
            return View(usuario);
            }

        [HttpPost]
        public async Task<IActionResult> CargarArchivoClientes(int id, IFormFile archivo)
            {
            if (archivo != null && archivo.Length > 0)
                {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario != null)
                    {
                    var archivoNuevo = new Archivo
                        {
                        Nombre = archivo.FileName,
                        Extension = Path.GetExtension(archivo.FileName),
                        ArchivoData = await ConvertToBytes(archivo),
                        IdUsuarios = id
                        };

                    _context.Archivos.Add(archivoNuevo);
                    await _context.SaveChangesAsync();

                    ViewBag.Mensaje = "Archivo subido con éxito.";
                    return RedirectToAction("VerDocumentosClientes", new { id = id });
                    }
                else
                    {
                    ViewBag.Mensaje = "Usuario no encontrado.";
                    }
                }
            else
                {
                ViewBag.Mensaje = "No se ha seleccionado un archivo.";
                }

            return View();
            }








        [HttpGet]
        public async Task<IActionResult> ListarClientes()
            {
            // Obtener la lista de usuarios cuyo Idrol es igual a 3
            var usuarios = await _context.Usuarios
                                          .Where(u => u.Idrol == 3)  // Filtro para Idrol = 3
                                          .ToListAsync();            // Ejecuta la consulta asincrónica

            return View(usuarios);  
            }







        [HttpGet]
        // Método en el controlador para ver los documentos de un cliente
        public async Task<IActionResult> VerDocumentosClientes(int id)
            {
            // Buscar el usuario por su Id
            var usuario = await _context.Usuarios
                .Include(u => u.Archivos)  // Incluir los archivos relacionados con el usuario
                .FirstOrDefaultAsync(u => u.IdUsuarios == id);  // Buscar por ID

            if (usuario == null)
                {
                return NotFound();  // Si no se encuentra el usuario, devolver un error 404
                }

            // Pasar el usuario a la vista
            return View(usuario);
            }

        private async Task<byte[]> ConvertToBytes(IFormFile archivo)
            {
            using (var memoryStream = new MemoryStream())
                {
                await archivo.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
                }
            }

        }
    }
