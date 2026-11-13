using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ComunaReclamos.Web.Pages.Admin;

public class LoginModel : PageModel
{
    private readonly IConfiguration _configuration;

    public LoginModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [BindProperty]
    public string Usuario { get; set; } = string.Empty;

    [BindProperty]
    public string Contrasena { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public string? Error { get; private set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var usuarioConfigurado = _configuration["Admin:Username"];
        var contrasenaConfigurada = _configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(usuarioConfigurado) || string.IsNullOrWhiteSpace(contrasenaConfigurada))
        {
            Error = "El acceso administrativo todavía no está configurado. Configure Admin__Username y Admin__Password.";
            return Page();
        }

        if (!string.Equals(Usuario, usuarioConfigurado, StringComparison.OrdinalIgnoreCase) ||
            !CoincideEnTiempoConstante(Contrasena, contrasenaConfigurada))
        {
            Error = "Usuario o contraseña incorrectos.";
            return Page();
        }

        var identidad = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, usuarioConfigurado),
                new Claim(ClaimTypes.Role, "Admin")
            ],
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidad),
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(4)
            });

        if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
        {
            return LocalRedirect(ReturnUrl);
        }

        return RedirectToPage("/Admin/Index");
    }

    private static bool CoincideEnTiempoConstante(string valorIngresado, string valorConfigurado)
    {
        var hashIngresado = SHA256.HashData(Encoding.UTF8.GetBytes(valorIngresado));
        var hashConfigurado = SHA256.HashData(Encoding.UTF8.GetBytes(valorConfigurado));
        return CryptographicOperations.FixedTimeEquals(hashIngresado, hashConfigurado);
    }
}
