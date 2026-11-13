using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ComunaReclamos.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Apunta directamente a tu SQL Server Express local
        optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=ComunaReclamos;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}