using ComunaReclamos.Data;
using ComunaReclamos.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ComunaReclamos.Web.Pages.Reclamos;

public class CrearModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public CrearModel(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [BindProperty]
    public Reclamo Reclamo { get; set; } = new();

    [BindProperty]
    public List<IFormFile> FotosAdjuntas { get; set; } = new();

    public List<SelectListItem> CategoriasLista { get; set; } = new();

    [TempData]
    public string? CodigoGenerado { get; set; }

    public async Task OnGetAsync(string? categoria)
    {
        await CargarCategoriasAsync();
        var categoriaSeleccionada = CategoriasLista.FirstOrDefault(item =>
            string.Equals(item.Text, categoria, StringComparison.OrdinalIgnoreCase) ||
            (!string.IsNullOrWhiteSpace(categoria) && item.Text.Contains(categoria, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(categoria) && categoria.Contains(item.Text, StringComparison.OrdinalIgnoreCase)));

        if (int.TryParse(categoriaSeleccionada?.Value, out var categoriaId))
        {
            Reclamo.CategoriaReclamoId = categoriaId;
        }
        else if (CategoriasLista.Count > 0 && int.TryParse(CategoriasLista[0].Value, out var primeraCategoriaId))
        {
            Reclamo.CategoriaReclamoId = primeraCategoriaId;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ModelState.Remove("Reclamo.CodigoSeguimiento");
        ModelState.Remove("Reclamo.Estado");
        ModelState.Remove("Reclamo.FechaCreacion");
        ModelState.Remove("Reclamo.Latitud");
        ModelState.Remove("Reclamo.Longitud");
        ModelState.Remove("Reclamo.DatosContacto");
        ModelState.Remove("Reclamo.OcultarNombre");
        ModelState.Remove("Reclamo.CuadrillaId");
        ModelState.Remove("Reclamo.Urgencia");
        ModelState.Remove("Reclamo.NotaInterna");

        if (Reclamo.CategoriaReclamoId <= 0 || !await _context.CategoriaReclamos.AnyAsync(categoria => categoria.Id == Reclamo.CategoriaReclamoId))
        {
            ModelState.AddModelError("Reclamo.CategoriaReclamoId", "Elegí un tipo de problema.");
        }

        var fotosValidas = FotosAdjuntas.Where(foto => foto.Length > 0).ToList();
        if (fotosValidas.Count > 3)
        {
            ModelState.AddModelError(nameof(FotosAdjuntas), "Podés adjuntar hasta 3 fotos.");
        }

        // Validar tamaño de fotos (máximo 5MB por foto)
        foreach (var foto in fotosValidas)
        {
            if (foto.Length > 5 * 1024 * 1024) // 5MB
            {
                ModelState.AddModelError(nameof(FotosAdjuntas), $"La foto '{foto.FileName}' es muy grande (máximo 5MB).");
                break;
            }

            // Validar tipo de archivo
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(foto.FileName).ToLower();
            if (!extensionesPermitidas.Contains(extension))
            {
                ModelState.AddModelError(nameof(FotosAdjuntas), $"El formato '{extension}' no está permitido. Usá JPG, PNG o GIF.");
                break;
            }
        }

        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync();
            return Page();
        }

        try
        {
            // 1. Generar código único y datos iniciales
            Reclamo.CodigoSeguimiento = GenerarCodigoUnico();
            Reclamo.FechaCreacion = DateTime.Now;
            Reclamo.Estado = "Recibido";
            Reclamo.Urgencia = "Media";
            Reclamo.DatosContacto = $"{Reclamo.NombreVecino} - {Reclamo.TelefonoContacto}";

            _context.Reclamos.Add(Reclamo);
            await _context.SaveChangesAsync(); // Guardamos para obtener el Reclamo.Id

            // 2. Guardar las fotos adjuntas (hasta tres)
            if (fotosValidas.Count > 0)
            {
                var carpetaUploads = Path.Combine(_environment.WebRootPath, "uploads", "reclamos");
                Directory.CreateDirectory(carpetaUploads);

                foreach (var fotoAdjunta in fotosValidas)
                {
                    var extension = Path.GetExtension(fotoAdjunta.FileName).ToLower();
                    var nombreArchivo = $"{Reclamo.CodigoSeguimiento}_{Guid.NewGuid().ToString("N")[..8]}{extension}";
                    var rutaCompleta = Path.Combine(carpetaUploads, nombreArchivo);

                    using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        await fotoAdjunta.CopyToAsync(stream);
                    }

                    _context.FotoReclamos.Add(new FotoReclamo
                    {
                        ReclamoId = Reclamo.Id,
                        RutaArchivo = $"/uploads/reclamos/{nombreArchivo}",
                        FechaSubida = DateTime.Now
                    });
                }
            }

            // 3. Registrar el primer paso en el historial
            var historial = new HistorialReclamo
            {
                ReclamoId = Reclamo.Id,
                EstadoAnterior = "-",
                EstadoNuevo = "Recibido",
                ComentarioVecino = "Reclamo ingresado por el vecino desde la web.",
                FechaCambio = DateTime.Now
            };
            _context.HistorialReclamos.Add(historial);

            await _context.SaveChangesAsync();

            // Redirigir a confirmación con el código
            return RedirectToPage("/Reclamos/Confirmacion", new { codigo = Reclamo.CodigoSeguimiento });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Hubo un error al guardar el reclamo: {ex.Message}");
            await CargarCategoriasAsync();
            return Page();
        }
    }

    private async Task CargarCategoriasAsync()
    {
        var categorias = await _context.CategoriaReclamos.OrderBy(categoria => categoria.Id).ToListAsync();
        CategoriasLista = categorias
            .Select(categoria => new SelectListItem(categoria.Nombre, categoria.Id.ToString()))
            .ToList();
    }

    private static string GenerarCodigoUnico()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Sin O, 0, I, 1 para evitar confusiones
        var random = Random.Shared;
        var parte1 = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
        var parte2 = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
        return $"{parte1}-{parte2}";
    }
}