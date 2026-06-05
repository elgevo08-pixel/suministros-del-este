namespace SuministrosDelEste.Domain.Events;

/// <summary>
/// Domain Event: se eleva cuando un nuevo material es registrado en el inventario.
/// Patrón: Domain Event (Observer) — notifica a los suscriptores del sistema.
/// </summary>
public sealed record MaterialRegistradoEvent(
    string NombreMaterial,
    decimal PrecioUnitario
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OcurridoEn { get; } = DateTime.UtcNow;
}
