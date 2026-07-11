using Microsoft.EntityFrameworkCore;
using SuministrosDelEste.Domain.Aggregates.Material;
using SuministrosDelEste.Domain.Aggregates.Material.ValueObjects;
using SuministrosDelEste.Domain.Events;
using SuministrosDelEste.Domain.Shared;
using SuministrosDelEste.Infrastructure.Persistence.Outbox;

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

    /// <summary>Mensajes del patrón Outbox: eventos de dominio pendientes o ya despachados.</summary>
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

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

        // Patrón Outbox: tabla técnica de Infraestructura, no forma parte del lenguaje ubicuo del Dominio.
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("OutboxMessages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TipoEvento).IsRequired().HasMaxLength(300);
            entity.Property(e => e.ContenidoJson).IsRequired();
            entity.Property(e => e.OcurridoEn).IsRequired();
            entity.Property(e => e.UltimoError).HasMaxLength(500);

            // Acelera el polling de OutboxDispatcherService (WHERE ProcesadoEn IS NULL).
            entity.HasIndex(e => e.ProcesadoEn);
        });
    }

    /// <summary>
    /// Antes de cada SaveChangesAsync, captura los Domain Events de todo Aggregate Root con
    /// cambios pendientes y los persiste como OutboxMessage en la MISMA transacción — esto es
    /// lo que hace que el patrón Outbox sea transaccional: el Aggregate y sus eventos se
    /// confirman juntos o no se confirma ninguno.
    ///
    /// Reemplaza la publicación síncrona que antes hacía el Handler directamente contra
    /// IEventPublisher (ver RegistrarMaterialHandler): ahora el despacho real ocurre de forma
    /// asíncrona en OutboxDispatcherService, sin bloquear la petición HTTP y sin riesgo de
    /// perder el evento si el proceso falla justo después de guardar.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        List<OutboxMessage> nuevosMensajes = ChangeTracker
            .Entries<AggregateRoot>()
            .Select(entry => entry.Entity)
            .Where(aggregate => aggregate.DomainEvents.Count > 0)
            .SelectMany(aggregate =>
            {
                List<IDomainEvent> eventos = [.. aggregate.DomainEvents];
                aggregate.ClearDomainEvents(); // evita volver a capturarlos en un SaveChanges posterior
                return eventos;
            })
            .Select(OutboxMessage.DesdeEvento)
            .ToList();

        if (nuevosMensajes.Count > 0)
            OutboxMessages.AddRange(nuevosMensajes);

        return base.SaveChangesAsync(cancellationToken);
    }
}
