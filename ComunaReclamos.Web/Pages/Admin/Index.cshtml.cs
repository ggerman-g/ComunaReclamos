using ComunaReclamos.Data;
using ComunaReclamos.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ComunaReclamos.Web.Pages.Admin;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private static readonly string[] EstadosPermitidos = ["Recibido", "En revisión", "En curso", "Resuelto", "Rechazado"];
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Reclamo> Reclamos { get; private set; } = [];
    public List<Cuadrilla> Cuadrillas { get; private set; } = [];
    public Reclamo? Seleccionado { get; private set; }
    public int Nuevos { get; private set; }
    public int EnCurso { get; private set; }
    public int ResueltosMes { get; private set; }
    public string? EstadoFiltro { get; private set; }

    [TempData]
    public string? Mensaje { get; set; }

    public async Task OnGetAsync(int? id, string? estado)
    {
        EstadoFiltro = estado;
        Nuevos = await _context.Reclamos.CountAsync(reclamo => reclamo.Estado == "Recibido");
        EnCurso = await _context.Reclamos.CountAsync(reclamo => reclamo.Estado == "En revisión" || reclamo.Estado == "En curso");
        var primerDiaMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        ResueltosMes = await _context.Reclamos.CountAsync(reclamo =>
            reclamo.Historial.Any(evento => evento.EstadoNuevo == "Resuelto" && evento.FechaCambio >= primerDiaMes));

        var consulta = _context.Reclamos
            .AsNoTracking()
            .Include(reclamo => reclamo.CategoriaReclamo)
            .Include(reclamo => reclamo.Cuadrilla)
            .OrderByDescending(reclamo => reclamo.FechaCreacion)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(estado) && EstadosPermitidos.Contains(estado, StringComparer.Ordinal))
        {
            consulta = consulta.Where(reclamo => reclamo.Estado == estado).OrderByDescending(reclamo => reclamo.FechaCreacion);
        }

        Reclamos = await consulta.ToListAsync();
        Cuadrillas = await _context.Cuadrillas.AsNoTracking().OrderBy(cuadrilla => cuadrilla.Nombre).ToListAsync();

        var reclamoId = id ?? Reclamos.FirstOrDefault()?.Id;
        if (reclamoId.HasValue)
        {
            Seleccionado = await _context.Reclamos
                .AsNoTracking()
                .Include(reclamo => reclamo.CategoriaReclamo)
                .Include(reclamo => reclamo.Cuadrilla)
                .Include(reclamo => reclamo.Fotos)
                .Include(reclamo => reclamo.Historial)
                .FirstOrDefaultAsync(reclamo => reclamo.Id == reclamoId.Value);
        }
    }

    public async Task<IActionResult> OnPostActualizarAsync(int reclamoId, string estado, int? cuadrillaId, string? comentarioVecino, string? notaInterna, string? estadoFiltro)
    {
        if (!EstadosPermitidos.Contains(estado, StringComparer.Ordinal))
        {
            Mensaje = "El estado elegido no es válido.";
            return RedirectToPage(new { id = reclamoId, estado = estadoFiltro });
        }

        if (comentarioVecino?.Length > 500 || notaInterna?.Length > 1000)
        {
            Mensaje = "El comentario público admite hasta 500 caracteres y la nota interna hasta 1000.";
            return RedirectToPage(new { id = reclamoId, estado = estadoFiltro });
        }

        var reclamo = await _context.Reclamos
            .Include(item => item.Historial)
            .FirstOrDefaultAsync(item => item.Id == reclamoId);

        if (reclamo is null)
        {
            return NotFound();
        }

        if (cuadrillaId.HasValue && !await _context.Cuadrillas.AnyAsync(cuadrilla => cuadrilla.Id == cuadrillaId.Value))
        {
            Mensaje = "La cuadrilla seleccionada no existe.";
            return RedirectToPage(new { id = reclamoId, estado = estadoFiltro });
        }

        var cambioEstado = !string.Equals(reclamo.Estado, estado, StringComparison.Ordinal);
        var comentario = comentarioVecino?.Trim();
        if (cambioEstado && estado == "Resuelto" && string.IsNullOrWhiteSpace(comentario))
        {
            Mensaje = "Para confirmar que se resolvió, agregá un comentario visible para el vecino.";
            return RedirectToPage(new { id = reclamoId, estado = estadoFiltro });
        }

        var estadoAnterior = reclamo.Estado;
        reclamo.Estado = estado;
        reclamo.CuadrillaId = cuadrillaId;
        reclamo.NotaInterna = string.IsNullOrWhiteSpace(notaInterna) ? null : notaInterna.Trim();

        if (cambioEstado || !string.IsNullOrWhiteSpace(comentario))
        {
            _context.HistorialReclamos.Add(new HistorialReclamo
            {
                ReclamoId = reclamo.Id,
                EstadoAnterior = estadoAnterior,
                EstadoNuevo = estado,
                ComentarioVecino = !string.IsNullOrWhiteSpace(comentario)
                    ? comentario
                    : $"El estado del reclamo cambió a {estado}.",
                FechaCambio = DateTime.Now
            });
        }

        await _context.SaveChangesAsync();
        Mensaje = cambioEstado && estado == "Resuelto"
            ? "Solución confirmada y visible para el vecino."
            : "Reclamo actualizado.";
        return RedirectToPage(new { id = reclamoId, estado = estadoFiltro });
    }

    public async Task<IActionResult> OnPostSalirAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/Admin/Login");
    }
}
