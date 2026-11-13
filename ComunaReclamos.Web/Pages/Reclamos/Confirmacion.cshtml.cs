using ComunaReclamos.Data;
using ComunaReclamos.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ComunaReclamos.Web.Pages.Reclamos;

public class ConfirmacionModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ConfirmacionModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Reclamo Reclamo { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(string? codigo)
    {
        var codigoNormalizado = codigo?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(codigoNormalizado))
        {
            return NotFound();
        }

        var reclamo = await _context.Reclamos
            .AsNoTracking()
            .Include(item => item.Historial)
            .FirstOrDefaultAsync(item => item.CodigoSeguimiento == codigoNormalizado);

        if (reclamo is null)
        {
            return NotFound();
        }

        Reclamo = reclamo;
        return Page();
    }

    public bool EstadoAlcanzado(string estado)
    {
        var orden = new[] { "Recibido", "En revisión", "En curso", "Resuelto" };
        var estadoActual = Array.IndexOf(orden, Reclamo.Estado);
        var estadoConsultado = Array.IndexOf(orden, estado);
        return estadoActual >= 0 && estadoConsultado >= 0 && estadoConsultado <= estadoActual;
    }
}
