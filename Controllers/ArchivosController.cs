using AppSoftDoc.Data;
using AppSoftDoc.Models;
using AppSoftDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

public class ArchivosController : Controller
    {
    private readonly AppDBContext _context;

    public ArchivosController(AppDBContext context)
        {
        _context = context;
        }

    // Acción para cargar la lista de usuarios (Index)
    public IActionResult Index()
        {
        var usuarios = _context.Usuarios
            .Include(u => u.Role)  // Cargar el rol asociado
            .ToList();

        return View(usuarios);
        }

    // Acción para ver los documentos de un usuario específico (VerDocumentos)
    public IActionResult VerDocumentos(int usuarioId)
        {
        // Asegurarse de que el usuario existe y cargar los archivos asociados
        var usuario = _context.Usuarios
            .Include(u => u.Role)  // Cargar el rol
            .Include(u => u.Archivos)  // Cargar los archivos asociados
            .FirstOrDefault(u => u.IdUsuarios == usuarioId);

        if (usuario == null)
            {
            return NotFound();
            }

        var viewModel = new VerDocumentosVM
            {
            UsuarioId = usuarioId,
            Archivos = usuario.Archivos.ToList()  // Pasamos los archivos del usuario
            };

        // Devolvemos la vista completa en lugar de una vista parcial
        return View(viewModel);
        }

    // Acción para subir un archivo
    [HttpPost]
    public IActionResult SubirArchivo(int usuarioId, IFormFile archivo, string nombreArchivo, string extension)
        {
        if (archivo != null && archivo.Length > 0)
            {
            var archivoNuevo = new Archivo
                {
                Nombre = nombreArchivo,
                Extension = extension,
                ArchivoData = ConvertToBytes(archivo),
                IdUsuarios = usuarioId
                };

            _context.Archivos.Add(archivoNuevo);
            _context.SaveChanges();
            }

        return RedirectToAction("VerDocumentos", new { usuarioId });
        }

    // Acción para descargar el archivo
    public IActionResult DescargarArchivo(int id)
        {
        var archivo = _context.Archivos.FirstOrDefault(a => a.IdArchivo == id);
        if (archivo != null)
            {
            return File(archivo.ArchivoData, "application/octet-stream", archivo.Nombre);
            }

        return NotFound();
        }

    private byte[] ConvertToBytes(IFormFile file)
        {
        using (var memoryStream = new MemoryStream())
            {
            file.CopyTo(memoryStream);
            return memoryStream.ToArray();
            }
        }
    }
