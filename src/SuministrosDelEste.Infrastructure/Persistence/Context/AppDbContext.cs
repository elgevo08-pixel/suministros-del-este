using Microsoft.EntityFrameworkCore;
using SuministrosDelEste.Domain.Aggregates.Material;
using SuministrosDelEste.Domain.Aggregates.Material.ValueObjects;

namespace SuministrosDelEste.Infrastructure.Persistence.Context;

/// <summary>
/// DbContext de EF Core para el ERP Suministros del Este.
/// Configura la persistencia del Aggregate Root Material con conversiones para Value Objects.
///
/// Patrón Hexagonal: Secondary Adapter — implementa el acceso a datos que el Dominio define
/// mediante la interfaz IMaterialRepository.
///
/// Nota de diseño: HasConversion permite que EF Core almacene tipos primitivos en SQL
/// y los rehidrate como Value Objects al leer. Las propiedades con private set son
/// accesibles por EF Core mediante reflexión en tiempo de ejecución.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Material> Materiales { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Material>(entity =>
        {
            entity.ToTable("Materiales");
            entity.HasKey(e => e.MaterialId);

            // La colección de Domain Events no se persiste en BD
            entity.Ignore(e => e.DomainEvents);

            // Value Object NombreMaterial ↔ string (columna Nombre)
            entity.Property(e => e.Nombre)
                  .HasConversion(
                      n => n.Valor,
                      v => NombreMaterial.Crear(v))
                  .IsRequired()
                  .HasMaxLength(100)
                  .HasColumnName("Nombre");

            // Value Object Stock (actual) ↔ decimal(18,4)
            entity.Property(e => e.StockActual)
                  .HasConversion(
                      s => s.Valor,
                      v => Stock.Crear(v))
                  .HasColumnType("decimal(18, 4)")
                  .HasColumnName("StockActual");

            // Value Object Stock (mínimo) ↔ decimal(18,4)
            entity.Property(e => e.StockMinimo)
                  .HasConversion(
                      s => s.Valor,
                      v => Stock.Crear(v))
                  .HasColumnType("decimal(18, 4)")
                  .HasColumnName("StockMinimo");

            // Value Object Precio ↔ decimal(18,2)
            entity.Property(e => e.PrecioUnitario)
                  .HasConversion(
                      p => p.Valor,
                      v => Precio.Crear(v))
                  .HasColumnType("decimal(18, 2)")
                  .HasColumnName("PrecioUnitario");

            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.FechaRegistro).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        });
    }
}
