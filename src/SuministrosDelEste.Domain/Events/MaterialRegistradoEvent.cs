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
    // init (no solo get) permite que el patrón Outbox reconstruya el evento exacto desde su
    // JSON persistido (OutboxMessage.Deserializar) preservando el EventId y la fecha originales;
    // para toda construcción normal via new(...), el valor por defecto sigue aplicando igual.
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OcurridoEn { get; init; } = DateTime.UtcNow;
}
