using ComunaReclamos.Data;
using ComunaReclamos.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ComunaReclamos.Web.Pages.Reclamos;

public class SeguirModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public SeguirModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string? Codigo { get; set; }

    public Reclamo? Reclamo { get; private set; }
    public string? Mensaje { get; private set; }

    public async Task OnGetAsync()
    {
        var codigoNormalizado = Codigo?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(codigoNormalizado))
        {
            return;
        }

        Reclamo = await _context.Reclamos
            .AsNoTracking()
            .Include(reclamo => reclamo.CategoriaReclamo)
            .Include(reclamo => reclamo.Historial)
            .FirstOrDefaultAsync(reclamo => reclamo.CodigoSeguimiento == codigoNormalizado);

        if (Reclamo is null)
        {
            Mensaje = "No encontramos un reclamo con ese código. Revisá que esté escrito correctamente.";
        }
    }
}
