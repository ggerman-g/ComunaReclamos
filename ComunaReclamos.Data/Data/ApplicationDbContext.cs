using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ComunaReclamos.Data.Entities;

namespace ComunaReclamos.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Reclamo> Reclamos => Set<Reclamo>();
    public DbSet<FotoReclamo> FotoReclamos => Set<FotoReclamo>();
    public DbSet<HistorialReclamo> HistorialReclamos => Set<HistorialReclamo>();
    public DbSet<CategoriaReclamo> CategoriaReclamos => Set<CategoriaReclamo>();
    public DbSet<Cuadrilla> Cuadrillas => Set<Cuadrilla>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Precarga de categorías iniciales para que la comuna no empiece con tablas vacías
        builder.Entity<CategoriaReclamo>().HasData(
            new CategoriaReclamo { Id = 1, Nombre = "Alumbrado Público", Descripcion = "Falta de luz, lámparas quemadas o cortocircuitos" },
            new CategoriaReclamo { Id = 2, Nombre = "Bacheo y Calles", Descripcion = "Pozos, pavimentación y mantenimiento vial" },
            new CategoriaReclamo { Id = 3, Nombre = "Recolección de Residuos", Descripcion = "Basurales, ramas o recolección demorada" },
            new CategoriaReclamo { Id = 4, Nombre = "Espacios Verdes", Descripcion = "Plazas, poda de árboles y limpieza de terrenos" }
        );
    }
}